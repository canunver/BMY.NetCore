// Günlük Nakit İhtiyacı (ButceUygulama/GunlukNakitIhtiyaci): seçilen muhasebe birimi için tarihin 5 iş günü öncesinden 5 iş günü
// sonrasına günlük nakit ihtiyacı toplamları. Hafta sonu olmayan ileri tarihli günlerde Kaydet tuşu detay penceresini açar;
// tutarlar harcama türlerine (Maaş, SGK, Vergi ...) göre girilir. Yazdır, bütün birimlerin Excel raporunu indirir.
// Son seçilen birim tarayıcıda saklanır (eski sayfadaki birim çerezi gibi); yoksa kullanıcının muhasebe birimi gelir.
$(function () {
    'use strict';

    if (!$('#grdGunler').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var BIRIM_ANAHTAR = 'GunlukNakit_Birim';
    var IPUCU = '<span>Birim ve tarih seçip Listele\'ye basınız.</span>';
    var acik = null;          // listelenmiş ölçüt { muhasebe, birim }: detay ve Kaydet bunu kullanır
    var seciliSatir = null;   // detay penceresi açılan günün satırı
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş ya da geçersiz -> 0 (eski parseNumber).
    function sayiCoz(metin) {
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        var n = parseFloat(s);
        return isFinite(n) ? n : 0;
    }

    // ---------------------------------------------------------------- birim

    function birimSakla(d) {
        try {
            if (d && d.muhasebe > 0 && d.birim) window.localStorage.setItem(BIRIM_ANAHTAR, JSON.stringify(d));
        } catch (e) { /* tarayıcı saklamaya izin vermiyorsa önemsiz */ }
    }

    function birimOku() {
        try {
            var d = JSON.parse(window.localStorage.getItem(BIRIM_ANAHTAR) || 'null');
            return d && d.muhasebe > 0 && d.birim ? d : null;
        } catch (e) { return null; }
    }

    // Birim değişince listelenmiş günler başka birimin olur: liste boşaltılır.
    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        degisince: function () { if (acik) listeyiBosalt(); }
    });

    // ---------------------------------------------------------------- günler tablosu

    function islemHucresi(p) {
        if (!p.data || !p.data.girilir) return '';
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-primary btn-xs';
        b.title = 'Tutarları gir / değiştir';
        b.innerHTML = '<span class="glyphicon glyphicon-floppy-disk" aria-hidden="true"></span><span class="sr-only">Kaydet</span>';
        b.addEventListener('click', function () { detayAc(p.node); });
        return b;
    }

    var grid = Sgb.grid('#grdGunler', {
        columnDefs: [
            { field: 'tarih', headerName: 'Tarih', width: 120, sortable: false, valueFormatter: function (p) { return Sgb.tarih(p.value); } },
            { field: 'gun', headerName: 'Gün', width: 120, sortable: false },
            {
                field: 'toplam', headerName: 'Toplam', width: 160, sortable: false, type: 'rightAligned', cellClass: 'sayi', headerClass: 'sayi',
                valueFormatter: function (p) { return p.value == null ? '' : para.format(p.value); }
            },
            { headerName: 'İşlemler', width: 90, sortable: false, cellRenderer: islemHucresi }
        ],
        getRowId: function (p) { return p.data.tarih; },
        getRowClass: function (p) { return p.data && !p.data.girilir ? 'kapali-gun' : ''; },
        overlayNoRowsTemplate: IPUCU
    });

    function listeyiBosalt() {
        acik = null;
        grid.setGridOption('rowData', []);
        grid.setGridOption('overlayNoRowsTemplate', IPUCU);
    }

    // Listele (eski gunlukListeOlustur).
    function listele() {
        var d = muhBirim.deger();
        var tarih = $('#txtTarih').val();
        if (d.muhasebe <= 0 || d.birim.length !== 3) { mesaj('Lütfen birim seçiniz!', 'warning'); return; }
        if (!tarih) { mesaj('Lütfen tarih giriniz!', 'warning'); return; }
        var o = { tarih: tarih, muhasebe: d.muhasebe, birim: d.birim };
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/gunluknakit/liste', o)
            .done(function (v) {
                if (v.hata) { listeyiBosalt(); mesaj(v.hata, 'danger'); return; }
                birimSakla(d);
                acik = { muhasebe: o.muhasebe, birim: o.birim };
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                grid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#btnListele').on('click', listele);

    // Yazdır (eski yazdirGunlukNakitIhtiyaci): seçilen tarih çevresinde bütün birimler.
    $('#btnYazdir').on('click', function () {
        var tarih = $('#txtTarih').val();
        if (!tarih) { mesaj('Lütfen tarih giriniz!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/gunluknakit/rapor', { tarih: tarih }).then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- detay penceresi

    var dlgDetay = document.getElementById('dlgDetay');

    function alanlariKur(turler) {
        var $alan = $('#detayAlanlar').empty();
        $.each(turler || [], function (i, t) {
            var kimlik = 'txtTur' + t.kod;
            $('<div class="detay-satir"></div>')
                .append($('<label></label>').attr('for', kimlik).text(t.ad))
                .append($('<input type="text" class="form-control input-sm tutar" inputmode="decimal" autocomplete="off" />')
                    .attr({ id: kimlik, 'data-tur': t.kod }))
                .appendTo($alan);
        });
    }

    function toplamHesapla() {
        var toplam = 0;
        $('#detayAlanlar .tutar').each(function () { toplam += sayiCoz($(this).val()); });
        $('#txtToplam').val(para.format(toplam));
    }

    function alanlariTemizle() {
        $('#detayAlanlar .tutar').val('');
        toplamHesapla();
    }

    $('#detayAlanlar').on('input', '.tutar', toplamHesapla);

    // Eski blur: sıfır olmayan tutar biçimlenir, sıfır boş bırakılır.
    $('#detayAlanlar').on('blur', '.tutar', function () {
        var n = sayiCoz($(this).val());
        $(this).val(n !== 0 ? para.format(n) : '');
    });

    // Detay (eski GunlukNakitDetayModal + GunlukNakitDetayGetir): günün kayıtlı tutarları okunur.
    function detayAc(node) {
        if (!acik || !node || !node.data) return;
        seciliSatir = node;
        alanlariTemizle();
        $('#lblDetayGun').text(Sgb.tarih(node.data.tarih) + ' ' + node.data.gun);
        $('#btnKaydet').prop('disabled', true);
        dlgDetay.showModal();
        Sgb.al('api/gunluknakit/detay', $.extend({ tarih: node.data.tarih }, acik))
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $.each(v.tutarlar || [], function (i, t) {
                    var n = Number(t.Tutar) || 0;
                    $('#txtTur' + t.HarcamaTur).val(n !== 0 ? para.format(n) : '');
                });
                toplamHesapla();
                $('#btnKaydet').prop('disabled', false);
                $('#detayAlanlar .tutar').first().trigger('focus');
            })
            .fail(hataGoster);
    }

    // Kaydet (eski KaydetGunlukNakit): bütün türler gönderilir; iş kuralı günün kayıtlarını silip sıfırdan büyükleri yazar.
    $('#btnKaydet').on('click', function () {
        if (!acik || !seciliSatir) { mesaj('Seçili kayıt yok', 'warning'); return; }
        var tutarlar = [];
        $('#detayAlanlar .tutar').each(function () {
            tutarlar.push({ HarcamaTur: Number($(this).attr('data-tur')), Tutar: sayiCoz($(this).val()) });
        });
        var node = seciliSatir;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/gunluknakit/kaydet', { Tarih: node.data.tarih, Muhasebe: acik.muhasebe, Birim: acik.birim, Tutarlar: tutarlar })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata || 'Kayıt işlemi yapılamadı!', 'danger'); return; }
                mesaj(s.bilgi, 'success');
                node.setDataValue('toplam', s.toplam);
                dlgDetay.close();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });
    $(dlgDetay).on('close', function () { seciliSatir = null; });

    // ---------------------------------------------------------------- açılış

    // Açılış (eski TarihAyarla + loadBirimFromCookie): tarih yarın, birim son seçilen ya da kullanıcının birimi. Liste boş başlar.
    Sgb.al('api/gunluknakit/tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            $('#txtTarih').val(v.tarih);
            alanlariKur(v.turler);
            var b = birimOku();
            if (b) muhBirim.ata(b);
            else if (v.muhasebe) muhBirim.ata({ muhasebe: v.muhasebe, birim: v.birim, kurumsal: v.kurumsal, birimAd: v.birimAd });
        })
        .fail(hataGoster);
});
