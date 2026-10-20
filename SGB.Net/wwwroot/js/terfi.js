// Personel terfi ekranları (wwwroot/js/terfi.js), uçlar /api/terfi/* (Istemci/InsanKaynaklari/TerfiUclari.cs):
//   data-sayfa="hakkazanan": InsanKaynaklari/TerfiyeHakKazananlar (eski TerfiyeHakKazananlar.aspx)
//       Listele, Yazdır (Excel), Terfi Onayına Gönder (işaretli satırlar + hizmet sebebi).
//   data-sayfa="onay":       InsanKaynaklari/PersonelTerfiOnaylama (eski PersonelTerfiOnaylama.aspx)
//       Listele, Yazdır (Excel, durum seçilmeli), Terfiyi Onayla / Terfiyi Reddet (işaretli, beklemedeki satırlar).
// Tablo salt okunurdur; açılışta boş gelir, Listele ile okunur.
$(function () {
    'use strict';

    var $kok = $('.terfi');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var ONAY = $kok.data('sayfa') === 'onay';
    var UC = 'api/terfi/';
    var grid;

    function bildir(metin, tur) {
        if (!metin) return;
        // Birden çok satırlı sonuç (satır satır hatalar) uyarı kutusunda kalır; tek satırlar bildirimle gösterilir.
        if (metin.indexOf('\n') >= 0) { Sgb.mesaj('#tfMesaj', metin, tur); yay(); return; }
        Sgb.bildirim(metin, tur);
    }
    function mesajTemizle() { Sgb.mesaj('#tfMesaj', ''); yay(); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        bildir(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }

    // ---------------------------------------------------------------- yükseklik: tablo sayfanın kalan yüksekliğini doldurur

    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var kap = el.closest('.sayfa-icerik') || document.body, cs = getComputedStyle(kap);
        var kaydirir = /(auto|scroll)/.test(cs.overflowY);
        var kr = kap.getBoundingClientRect(), kay = kaydirir ? kap.scrollTop : 0;
        var gorunen = kaydirir ? kap.clientHeight : window.innerHeight - kr.top;
        var ust = el.getBoundingClientRect().top - kr.top + kay;
        var son = 0;
        $(kap).children().each(function () {
            if (this.offsetParent === null && getComputedStyle(this).position !== 'fixed') return;
            son = Math.max(son, this.getBoundingClientRect().bottom - kr.top + kay + parseFloat(getComputedStyle(this).marginBottom || 0));
        });
        son += parseFloat(cs.paddingBottom || 0);
        var bosluk = Math.max(0, son - (ust + el.offsetHeight));
        el.style.height = Math.max(enAz, Math.floor(gorunen - ust - bosluk - 1)) + 'px';
    }
    function yay() { yuksekligeYay(document.getElementById('grdListe'), 260); }
    $(window).on('resize', yay);

    $('<div id="tfMesaj" class="alert tf-mesaj" role="alert" hidden></div>').insertBefore('#grdListe');

    $('#btnYardim').on('click', function () {
        var $m = $('#tfYardim'), acik = $m.prop('hidden');
        $m.prop('hidden', !acik);
        $(this).attr('aria-expanded', acik ? 'true' : 'false');
        yay();
    });

    // ---------------------------------------------------------------- tablo

    // Kadroya ulaşılmış satırda (eski RENKATA) kadro, kazanılmış hak ve emekliye esas hücreleri yeşil zeminli.
    function renkli(p) { return !!(p.data && p.data.renkAta); }

    function metin(alan, baslik, gen) {
        return { field: alan, headerName: baslik, width: gen, tooltipField: alan };
    }
    function sayi(alan, baslik, gen, renk) {
        var s = { field: alan, headerName: baslik, width: gen || 70, cellClass: 'tf-orta' };
        if (renk) s.cellClassRules = { 'tf-renk': renkli };
        return s;
    }
    function uclu(onEk, renk) {
        return [
            sayi(onEk + 'Derece', 'Derece', 70, renk),
            sayi(onEk + 'Kademe', 'Kademe', 72, renk),
            sayi(onEk + 'Tarih', 'Tarih', 96, renk)
        ];
    }

    function gridKur() {
        var kisi = [metin('tipAd', 'Terfi Tipi', 130)];
        if (ONAY) kisi.push(metin('durumAd', 'Terfi Durumu', 110), sayi('yilAy', 'Yıl Ay', 80));
        kisi.push(
            metin('kimlikNo', 'Kimlik No', 115),
            metin('ad', 'Ad Soyad', 170),
            metin('sicilNo', 'Kurum Sicil No', 110),
            metin('birim', 'Birim', 180),
            metin('unvan', 'Unvanı', 150));
        if (ONAY) kisi.push(metin('sebepAd', 'Hizmet Sebebi', 150));
        kisi.push(metin('ogrenim', 'Öğrenim Türü', 130));

        var renk = !ONAY;
        grid = Sgb.grid('#grdListe', {
            columnDefs: [
                { headerName: 'Kişi Bilgileri', headerClass: 'tf-grup', children: kisi },
                { headerName: 'Kadro', headerClass: 'tf-grup', children: [sayi('kadroDerece', 'Derece', 70, renk), sayi('kadroTarih', 'Tarih', 96, renk)] },
                { headerName: 'Kazanılmış Hak', headerClass: 'tf-grup', children: uclu('kazanilmisHak', renk) },
                { headerName: 'Emekliye Esas', headerClass: 'tf-grup', children: uclu('emekliyeEsas', renk) },
                { headerName: 'Ödemeye Esas', headerClass: 'tf-grup', children: uclu('odemeyeEsas', false) },
                { headerName: ONAY ? 'Yükselebileceği' : 'Yükseleceği', headerClass: 'tf-grup', children: [sayi('yukselecegiDerece', 'Derece', 90)] }
            ],
            rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
            getRowId: function (p) { return String(ONAY ? p.data.kod : p.data.pKadroKod); },
            tooltipShowDelay: 400,
            overlayNoRowsTemplate: '<span class="text-muted">' +
                (ONAY ? 'Durum ve tip seçip Listele\'ye basın.' : 'Yıl, ay ve tip seçip Listele\'ye basın.') + '</span>'
        });
    }

    function doldur(sec, liste, bos) {
        var $s = $(sec).empty();
        if (bos !== undefined) $('<option value="0"></option>').text(bos).appendTo($s);
        $.each(liste || [], function (i, x) { $('<option></option>').val(x.kod).text(x.ad).appendTo($s); });
    }

    function kriter() {
        return ONAY
            ? { durum: parseInt($('#cmbDurum').val(), 10) || 0, tip: parseInt($('#cmbTip').val(), 10) || 0 }
            : { yil: parseInt($('#txtYil').val(), 10) || 0, ay: parseInt($('#cmbAy').val(), 10) || 0, tip: parseInt($('#cmbTip').val(), 10) || 0 };
    }

    function kriterDenetle(k) {
        if (ONAY) return '';
        var h = [];
        if (k.yil <= 0) h.push('Yıl bilgisini seçmelisiniz.');
        if (k.ay <= 0) h.push('Ay bilgisini seçmelisiniz.');
        if (k.tip <= 0) h.push('Tip bilgisini seçmelisiniz.');
        return h.join(' ');
    }

    // ---------------------------------------------------------------- listele / yazdır

    var okunanKriter = null; // listelenen tablonun kriterleri (Terfi Onayına Gönder bu dönemle yapılır)

    function listele() {
        var k = kriter(), h = kriterDenetle(k);
        if (h) { bildir(h, 'warning'); return $.Deferred().reject().promise(); }
        mesajTemizle();
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        return Sgb.al(UC + (ONAY ? 'onay/liste' : 'hakkazanan/liste'), k)
            .done(function (v) {
                if (!v.basarili) { bildir(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                okunanKriter = k;
                if (!(v.satirlar || []).length) bildir('Kriterlere uyan kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmKriter').on('submit', function (e) { e.preventDefault(); listele(); });

    $('#btnYazdir').on('click', function () {
        var k = kriter(), h = kriterDenetle(k);
        if (ONAY && k.durum <= 0) h = 'Durum bilgisini seçmelisiniz.';
        if (h) { bildir(h, 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir(UC + (ONAY ? 'onay/yazdir' : 'hakkazanan/yazdir'), k).then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- işlemler

    function sonucGoster(v) {
        if (v.hata) bildir(v.hata, v.basarili ? 'warning' : 'danger');
        else bildir(v.bilgi, 'success');
    }

    $('#btnGonder').on('click', function () {
        var secili = grid.getSelectedRows();
        if (!secili.length) { bildir('Listeden herhangi bir başvuru seçilmediği için işlem yapılamadı.', 'warning'); return; }
        var sebep = parseInt($('#cmbSebep').val(), 10) || 0;
        if (sebep <= 0) { bildir('Atama gerekçesi (hizmet sebebi) seçiniz!', 'warning'); $('#cmbSebep').trigger('focus'); return; }
        if (!okunanKriter) return;
        if (!window.confirm('Seçili ' + secili.length + ' kayıt terfi onayına gönderilecek. Onaylıyor musunuz?')) return;

        mesajTemizle();
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'hakkazanan/gonder', {
            yil: okunanKriter.yil, ay: okunanKriter.ay, tip: okunanKriter.tip, sebepKod: sebep,
            pKadroKodlar: $.map(secili, function (d) { return d.pKadroKod; })
        })
            .done(function (v) { sonucGoster(v); if (v.basarili) grid.deselectAll(); })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    function onayIslem(onay) {
        var secili = grid.getSelectedRows();
        if (!secili.length) { bildir('Listeden herhangi bir başvuru seçilmediği için işlem yapılamadı.', 'warning'); return; }
        var soru = 'Seçili ' + secili.length + ' terfi kaydı ' + (onay ? 'onaylanacak' : 'reddedilecek') + '. Onaylıyor musunuz?';
        if (!window.confirm(soru)) return;

        mesajTemizle();
        var $b = $('#btnOnayla, #btnReddet').prop('disabled', true);
        Sgb.gonder(UC + 'onay/islem', { onay: onay, kodlar: $.map(secili, function (d) { return d.kod; }) })
            .done(function (v) {
                // Eski sayfa da işlemden sonra yeniden listeliyordu.
                listele().always(function () { sonucGoster(v); });
            })
            .fail(hataGoster)
            .always(function () { $b.prop('disabled', false); });
    }
    $('#btnOnayla').on('click', function () { onayIslem(true); });
    $('#btnReddet').on('click', function () { onayIslem(false); });

    // ---------------------------------------------------------------- açılış

    gridKur();
    yay();

    Sgb.al(UC + 'secenekler')
        .done(function (v) {
            if (!v.basarili) { bildir(v.hata, 'danger'); return; }
            if (ONAY) {
                doldur('#cmbDurum', v.durumlar, 'Tümü');
                $('#cmbDurum').val('1'); // beklemede olanlar
                doldur('#cmbTip', v.tipler, 'Tümü');
            } else {
                Sgb.yilKutusu('#txtYil', [v.yil - 10, v.yil + 1], v.yil);
                doldur('#cmbAy', v.aylar);
                $('#cmbAy').val(String(v.ay));
                doldur('#cmbTip', v.tipler);
                $('#cmbTip').val('3'); // K.H.A. ve E.K.E.A.
                doldur('#cmbSebep', v.sebepler, '');
            }
        })
        .fail(hataGoster);
});
