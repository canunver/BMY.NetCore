// Takip İşlemleri (AlacakTakip/AlacakTakipListe): solda sorgu, ortada dosyalar, sağda seçilen dosyanın bilgileri.
//   Listele (eski btnListele_Click), Liste Yazdır: Kısa / Uzun Liste Excel (btnYazdir_Click), Durum Değiştir: işaretli dosyalar
//   (btnDurum_Click), Dosya Kapat penceresi (wndDosyaKapat), Yeni Dosya (Temizle), Kaydet (btnKaydet_Click), Hesap Hareket,
//   Ödeme Bilgileri (tebliğden sonra), Toplu Ödeme Emri, Muavin Defteri pencereleri, kimlikten ad / adres (imgKimlik_Click),
//   kişi / firma seçimi. Dosya no tıklanınca dosya forma gelir (eski DetayAc / AlanlariDoldur), durum tıklanınca tarihçesi açılır.
// Uçlar: /api/alacaktakip/takip/* (Istemci/AlacakTakip/AlacakTakipListeUclari.cs).
$(function () {
    'use strict';

    if (!$('#grdListe').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var U = 'api/alacaktakip/takip/';
    var tanim = { ego: false, firmaZorunlu: false, muhasebe: '', birim: '', aktifYil: new Date().getFullYear() };
    var acikDurum = 0;     // forma okunan dosyanın durum kodu (eski hdnDurum)
    var sonKriter = null;  // son listelemenin ölçütleri (Kaydet / durum değişikliğinden sonra yeniden listelenir)

    var tutarBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // ---------------------------------------------------------------- açılır menüler (Bootstrap JS yüklenmiyor)

    $('.atl-arac').on('click', '.dropdown-toggle', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var $g = $(this).closest('.btn-group');
        var acik = $g.hasClass('open');
        $('.atl-arac .btn-group.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false');
        if (!acik) $g.addClass('open').children('.dropdown-toggle').attr('aria-expanded', 'true');
    });
    $(document).on('click', function () {
        $('.atl-arac .btn-group.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false');
    });

    // ---------------------------------------------------------------- tutar

    function tutarOku(s) {
        s = $.trim(s || '');
        if (!s) return 0;
        // Türkçe biçim: binlik nokta, ondalık virgül (eski formatTutar)
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = parseFloat(s.replace(/[^0-9.\-]/g, ''));
        return isNaN(n) ? NaN : n;
    }
    function tutarYaz(n) { return n == null || n === '' ? '' : tutarBicim.format(n); }

    $('#txtBorc').on('change', function () {
        var n = tutarOku($(this).val());
        if (!isNaN(n) && $.trim($(this).val()) !== '') $(this).val(tutarYaz(n));
    });

    function tlKarsilikYaz(t) {
        $('#lblTLKarsilik').text(t ? tutarYaz(t) + ' TL' : '');
    }

    // ---------------------------------------------------------------- muhasebe / birim

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', ad: 'lblBirimAd' },
        kurumsalGizle: true
    });

    // ---------------------------------------------------------------- liste

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'dosyaNo', headerName: 'Yıl/Dosya', width: 110, cellClass: 'atl-baglanti',
                cellRenderer: function (p) { var a = document.createElement('a'); a.href = '#'; a.textContent = p.value || ''; a.className = 'atl-dosya'; return a; } },
            { field: 'borcluAd', headerName: 'Borçlu Adı', flex: 1, minWidth: 160, tooltipField: 'borcluAd' },
            { field: 'borc', headerName: 'Borç', width: 120, type: 'rightAligned', colId: 'borc',
                valueFormatter: function (p) { return p.value == null ? '' : tutarYaz(p.value); } },
            { field: 'paraBirimi', headerName: 'Para Birimi', width: 70, colId: 'paraBirimi' },
            { field: 'durum', headerName: 'Durumu', width: 140, tooltipValueGetter: function () { return 'Tarihçeyi görmek için tıklayınız'; },
                cellRenderer: function (p) { var a = document.createElement('a'); a.href = '#'; a.textContent = p.value || ''; a.className = 'atl-tarihce'; return a; } }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        getRowId: function (p) { return p.data.yil + '/' + p.data.dosyaID; },
        overlayNoRowsTemplate: '<span>Sorgu alanlarını doldurup Listele\'ye basınız.</span>',
        pagination: true,
        paginationPageSize: 250,
        paginationPageSizeSelector: false,
        onCellClicked: function (e) {
            var t = e.event && e.event.target;
            if (!t || t.tagName !== 'A') return;
            e.event.preventDefault();
            if ($(t).hasClass('atl-dosya')) detayAc(e.data);
            else if ($(t).hasClass('atl-tarihce')) tarihceGoster(e.data);
        }
    });

    function kriter() {
        return {
            yil: parseInt($('#sYil').val(), 10) || 0,
            durum: parseInt($('#sDurum').val(), 10) || 0,
            muhasebe: $.trim($('#sMuhasebe').val()),
            dosyaNo: $.trim($('#sDosyaNo').val()),
            kimlikNo: $.trim($('#sKimlikNo').val()),
            borcluAd: $.trim($('#sBorcluAd').val()),
            islemYapan: $.trim($('#sIslemYapan').val())
        };
    }

    function listele(k, bulunamadiGoster) {
        k = k || kriter();
        sonKriter = k;
        $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        return Sgb.al(U + 'liste', k)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var s = v.satirlar || [];
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun bilgi bulunamadı.</span>');
                grid.setGridOption('rowData', s);
                $('#lblSayi').text(s.length ? '(' + s.length + ')' : '');
                if (!s.length && bulunamadiGoster) mesaj('Verilen kriterlere uygun bilgi bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $('#btnListele').prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#btnListele').on('click', function () { listele(null, true); });
    $('#frmSorgu').on('submit', function (e) { e.preventDefault(); listele(null, true); });

    // ---------------------------------------------------------------- form

    var tarihAlanlari = {
        borcDogusTarihi: '#txtBorcDogus', faizBaslangicTarihi: '#txtFaizBaslangic', borcTespitTarihi: '#txtBorcTespit',
        tebligTarihi: '#txtTeblig', borcDegerlendirmeTarihi: '#txtBorcDeger', taksitBaslangicTarihi: '#txtTaksitBaslangic',
        gelenYaziTarih: '#txtYaziGelTarih', saymanligaGidenYaziTarih: '#txtSayGitTarih', kisiyiGidenYaziTarih: '#txtKisiGitTarih'
    };
    var metinAlanlari = {
        saymanlikDosyaNo: '#txtSDosyaNo', kimlikNo: '#txtMernisNo', borcluAd: '#txtBorcluAd', borcluAdres: '#txtBorcluAdres',
        borcSebep: '#txtBorcSebep', gelenYaziYer: '#txtYaziGelYer', gelenYaziSayi: '#txtYaziGelSayi',
        saymanligaGidenYaziSayi: '#txtSayGitSayi', kisiyeGidenYaziSayi: '#txtKisiGitSayi'
    };

    function odemeGoster() { $('#btnOdeme').prop('hidden', !(parseInt($('#txtDosyaNo').val(), 10) > 0 && acikDurum >= 6)); }

    // eski Temizle (Yeni Dosya)
    function temizle() {
        $('#frmDosya')[0].reset();
        $.each(metinAlanlari, function (ad, s) { $(s).val(''); });
        $.each(tarihAlanlari, function (ad, s) { $(s).val(''); });
        $('#txtYil').val(String(tanim.aktifYil));
        $('#txtDosyaNo, #txtBorc, #txtZamanAsimi, #txtIslemTarihi, #txtIslemYapan').val('');
        $('#ddlParaBirimi').prop('selectedIndex', 0);
        $('#ddlFaizTuru').val('');
        muhBirim.ata({ muhasebe: tanim.muhasebe, birim: tanim.birim });
        if (tanim.muhasebe) muhBirim.yenile();
        tlKarsilikYaz(0);
        acikDurum = 0;
        $('#lblDurumAd').prop('hidden', true).text('');
        odemeGoster();
    }

    // eski DetayAc + AlanlariDoldur
    function detayAc(s) {
        Sgb.al(U + 'oku', { yil: s.yil, dosyaNo: s.dosyaID })
            .done(function (d) {
                if (d.hata) { mesaj(d.hata, 'danger'); return; }
                temizle();
                $('#txtYil').val(String(d.yil));
                $('#txtDosyaNo').val(String(d.dosyaNo));
                acikDurum = d.durumKod || 0;
                $('#lblDurumAd').text(d.durumAd || '').prop('hidden', !d.durumAd);
                $.each(metinAlanlari, function (ad, sec) { $(sec).val(d[ad] || ''); });
                $.each(tarihAlanlari, function (ad, sec) { $(sec).val(d[ad] || ''); });
                muhBirim.ata({ muhasebe: d.muhasebe, birim: d.birim, birimAd: d.birimAd || d.muhasebeAd });
                $('#txtBorc').val(tutarYaz(d.borc));
                $('#ddlParaBirimi').val(String(d.paraBirimi || 1));
                tlKarsilikYaz(d.tlKarsilik);
                $('#ddlFaizTuru').val(d.faizTur || '');
                $('#txtZamanAsimi').val(d.zamanAsimiBaslangicYili || '');
                $('#txtIslemTarihi').val(d.sonDurumTarihi ? Sgb.tarih(d.sonDurumTarihi) : '');
                $('#txtIslemYapan').val(d.islemYapan || '');
                odemeGoster();
            })
            .fail(hataGoster);
    }

    $('#btnYeni').on('click', temizle);

    $('#btnKaydet').on('click', function () {
        var mb = muhBirim.deger();
        var borc = tutarOku($('#txtBorc').val());
        var i = {
            yil: parseInt($('#txtYil').val(), 10) || tanim.aktifYil,
            dosyaNo: parseInt($('#txtDosyaNo').val(), 10) || 0,
            muhasebe: mb.muhasebe ? String(mb.muhasebe) : '',
            birim: mb.birim,
            borc: isNaN(borc) ? 0 : borc,
            paraBirimi: parseInt($('#ddlParaBirimi').val(), 10) || 1,
            faizTur: $('#ddlFaizTuru').val() || '',
            zamanAsimiBaslangicYili: parseInt($('#txtZamanAsimi').val(), 10) || 0
        };
        $.each(metinAlanlari, function (ad, s) { i[ad] = $.trim($(s).val()); });
        $.each(tarihAlanlari, function (ad, s) { i[ad] = $(s).val() || ''; });
        if (isNaN(borc)) { mesaj('Borç miktarı yanlış yazılmış.', 'warning'); $('#txtBorc').trigger('focus'); return; }
        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder(U + 'kaydet', i)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                if (s.dosyaNo) $('#txtDosyaNo').val(String(s.dosyaNo));
                if (s.yil) $('#txtYil').val(String(s.yil));
                tanim.muhasebe = i.muhasebe;
                tanim.birim = i.birim;
                tlKarsilikYaz(s.tlKarsilik);
                mesaj(s.bilgi, 'success');
                listele(sonKriter || kriter(), false);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- kimlik / kişi-firma

    $('#btnKimlik').on('click', function () {
        var tc = $.trim($('#txtMernisNo').val());
        if (!tc) { mesaj('T.C. kimlik numarasını yazınız.', 'warning'); $('#txtMernisNo').trigger('focus'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.al(U + 'kimlik', { tc: tc })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); $('#txtBorcluAd').val(''); return; }
                $('#txtBorcluAd').val(v.ad || '');
                $('#txtBorcluAdres').val(v.adres || '');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // eski FirmaBilgisiAc / FirmaBilgisiGoster
    $('#btnFirmaSec').on('click', function () {
        Sgb.firmaSec({
            adres: U + 'firma/ara',
            muhasebe: muhBirim.deger().muhasebe || 0,
            ara: $.trim($('#txtMernisNo').val()),
            secilince: function (f) {
                $('#txtBorcluAd').val(f.ad || '');
                $('#txtMernisNo').val(f.vergiNo || '');
                $('#txtBorcluAdres').val(f.adres || '');
            }
        });
    });

    // ---------------------------------------------------------------- durum değiştir / dosya kapat

    function isaretliler() {
        return $.map(grid.getSelectedRows(), function (r) { return { yil: r.yil, dosyaNo: r.dosyaID }; });
    }

    function durumGonder(durum, aciklama) {
        var dosyalar = isaretliler();
        if (!dosyalar.length) { mesaj('İşlem yapılacak herhangi bir dosya seçilmemiş!', 'warning'); return $.Deferred().reject().promise(); }
        if (!window.confirm('İşlem yapılacak. Onaylıyor musunuz?')) return $.Deferred().reject().promise();
        return Sgb.gonder(U + 'durum', { durum: durum, aciklama: aciklama || '', dosyalar: dosyalar })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                listele(sonKriter || kriter(), false);
            })
            .fail(hataGoster);
    }

    $('.atl-arac').on('click', '[data-durum]', function (e) {
        e.preventDefault();
        durumGonder(parseInt($(this).data('durum'), 10), '');
    });

    var dlgKapat = document.getElementById('dlgDosyaKapat');
    $(dlgKapat).on('click', '[data-kapat]', function () { dlgKapat.close(); });
    $('#lnkDosyaKapat').on('click', function (e) {
        e.preventDefault();
        if (!isaretliler().length) { mesaj('İşlem yapılacak herhangi bir dosya seçilmemiş!', 'warning'); return; }
        $('#txtKapatAciklama').val('');
        dlgKapat.showModal();
    });
    $('#btnDosyaKapat').on('click', function () {
        durumGonder(parseInt($('#ddlKapatDurum').val(), 10) || 0, $.trim($('#txtKapatAciklama').val()))
            .done(function (s) { if (s && s.basarili) dlgKapat.close(); });
    });

    // ---------------------------------------------------------------- yazdır

    $('.atl-arac').on('click', '[data-yazdir]', function (e) {
        e.preventDefault();
        var k = $.extend({ sablon: parseInt($(this).data('yazdir'), 10) }, kriter());
        Sgb.indir(U + 'yazdir', k);
    });

    // ---------------------------------------------------------------- pencereler

    function dosyaSecili() {
        var no = parseInt($('#txtDosyaNo').val(), 10) || 0;
        if (!no) { mesaj('Lütfen önce listeden bir dosya seçiniz.', 'warning'); return 0; }
        return no;
    }

    function tarihceGoster(s) {
        Sgb.pencere('../AlacakTakip/DosyaTarihce?' + $.param({ yil: s.yil, dosyaNo: s.dosyaID }), 'Tarihçe: ' + s.dosyaNo);
    }

    $('#btnHesapHareket').on('click', function () {
        var no = dosyaSecili();
        if (!no) return;
        Sgb.pencere('../AlacakTakip/HesapHareket?' + $.param({ yil: $('#txtYil').val(), dosyaNo: no }), 'Hesap Hareket');
    });

    // eski OdemeAc: ödeme bilgisi tebliğ aşamasından sonra girilir
    $('#btnOdeme').on('click', function () {
        var no = dosyaSecili();
        if (!no) return;
        if (acikDurum < 6) { mesaj('Ödeme Bilgisi Tebliğ Aşamasından sonra girilebilir.', 'warning'); return; }
        Sgb.pencere('../AlacakTakip/Odeme?' + $.param({ yil: $('#txtYil').val(), dosyaNo: no, paraBirimi: $('#ddlParaBirimi').val() }), 'Ödeme Bilgileri',
            function () { listele(sonKriter || kriter(), false); });
    });

    $('#btnTopluOdemeEmri').on('click', function () {
        Sgb.pencere('../AlacakTakip/AlacakTakipOdemeEmri', 'Toplu Ödeme Emri Oluştur', function () { listele(sonKriter || kriter(), false); });
    });

    $('#btnMuavin').on('click', function () {
        Sgb.pencere('../ButceMuhasebe/MuavinDefteriAltHesapBazindaRapor?' + $.param({ hesapKod: '140.01', altHesapKod: $.trim($('#txtMernisNo').val()), yil: 0 }),
            'Muavin Defteri Listesi');
    });

    // ---------------------------------------------------------------- açılış

    function secenekYaz(sel, liste, bosMetin) {
        var $s = $(sel).empty();
        if (bosMetin !== undefined) $('<option value=""></option>').text(bosMetin).appendTo($s);
        $.each(liste || [], function (i, x) { $('<option></option>').attr('value', x.kod).text(x.ad).appendTo($s); });
    }

    Sgb.al(U + 'tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            tanim = v;
            var yillar = [];
            for (var y = v.ilkYil; y <= v.aktifYil; y++) yillar.push(y);
            Sgb.yilKutusu('#sYil', yillar, v.aktifYil);
            secenekYaz('#sDurum', v.durumlar);
            $('#sDurum').val('999');
            secenekYaz('#ddlKapatDurum', v.kapatDurumlar);
            secenekYaz('#ddlFaizTuru', v.faizTurleri, '');
            secenekYaz('#ddlParaBirimi', v.paraBirimleri);
            if (v.firmaZorunlu) $('#txtMernisNo').prop('readonly', true).addClass('okuma').attr('title', 'Kişi / firma listesinden seçiniz');
            if (v.ego) {
                // EGO: borç muhasebeden gelir (eski EGOEkranIslemleri)
                grid.setColumnsVisible(['borc', 'paraBirimi'], false);
                $('#txtBorc').prop('readonly', true).addClass('okuma');
            }
            temizle();
            listele({ yil: v.aktifYil, durum: 999 }, false); // eski sayfa açılışta bu yılın tamamlanmamış dosyalarını listeliyordu
        })
        .fail(hataGoster);
});
