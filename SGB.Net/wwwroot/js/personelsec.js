// Sgb.personelSec: ortak personel seçimi penceresi (eski InsanKaynaklari/ListePersonelSec.aspx).
// Pencere sayfaya ilk kullanımda JS ile eklenir; sayfanın cshtml'ine bir şey konmaz. Uçlar: /api/personelsec/* (InsanKaynaklari/PersonelSecimUclari.cs).
// Görünüm: css/personelsec.css (+ tanim.css pencere görünümü). AG Grid ve ortak.js gerekir.
// Bileşeni yeni bir sayfa kullanacaksa o sayfanın menü adresi PersonelSecimUclari.UstSayfalar'a eklenir (yetki oradan gelir).
//
// Kullanım:
//   Sgb.personelSec({
//       baslik:    'Personel Seç',
//       coklu:     false,          // eski cokluSec=1: onay kutularıyla birden çok personel; "Seç" ve "Seç Kapat" düğmeleri
//       secKapat:  true,           // tek seçimde personel seçilince pencere kapansın mı (eski secKapat=1)
//       muhasebe:  '', birim: '',  // eski muhasebe / birim (3 hane): o birim önerilir, sistem yöneticisi değilse değiştirilemez
//       perTip:    '',             // eski perTip: ";" ile ayrılmış personel türleri
//       tarih:     '', tarih2: '', // eski tarih / tarih2 (yyyy-MM-dd): o aralıkta aktif olanlar
//       secilince: function (satirlar) { }   // seçilen personel dizisi; false dönerse pencere açık kalır
//   });
// Aktarılan satır: { kod, TCKIMLIKNO, AD, SOYAD, adSoyad, UNVANKOD, UNVANAD, KADRODERECE, EKGOSTERGE, EPOSTA, sicilNo, sskSicilNo,
//   bankaHesapNo, sinifKodu, personelTip, birimAd, odemeEsasDerece/Kademe/Gosterge/EkGosterge, emekliEsasDerece/Kademe/Gosterge/EkGosterge }
(function (Sgb, $) {
    'use strict';

    var UC = 'api/personelsec/';
    var o = {};
    var $p = null, pencere = null, grid = null, agac = null;
    var birim = { kod: 0, ad: '', kilitli: false };

    function hata(xhr, metin) { if (!xhr || xhr.status !== 401) Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : (metin || 'Personel listesi okunamadı.'), 'danger'); }

    function kur() {
        $p = $(
            '<dialog class="tanim-pencere personel-sec" aria-labelledby="psBaslik">' +
            '  <div class="pencere-baslik">' +
            '    <h2 id="psBaslik">Personel Seç</h2>' +
            '    <button type="button" class="close" data-ps-kapat aria-label="Kapat"><span aria-hidden="true">&times;</span></button>' +
            '  </div>' +
            '  <div class="pencere-govde">' +
            '    <div class="ps-kriter">' +
            '      <div class="ps-alan">' +
            '        <label for="psBirimAd">Birimi</label>' +
            '        <div class="ps-yan">' +
            '          <input type="text" id="psBirimAd" class="form-control input-sm" readonly placeholder="Tümü" />' +
            '          <button type="button" class="btn btn-default btn-sm" data-ps-birim-ac aria-expanded="false" title="Birim seç" aria-label="Birim seç"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></button>' +
            '          <button type="button" class="btn btn-default btn-sm" data-ps-birim-sil title="Birimi kaldır" aria-label="Birimi kaldır"><span class="glyphicon glyphicon-remove" aria-hidden="true"></span></button>' +
            '        </div>' +
            '      </div>' +
            '      <div class="ps-birim-panel" hidden>' +
            '        <div class="ps-yan"><input type="search" class="form-control input-sm" data-ps-birim-ara placeholder="Birim adında ara (en az 3 harf), Enter" aria-label="Birim ara" /></div>' +
            '        <ul class="ps-agac" aria-label="Birimler"></ul>' +
            '      </div>' +
            '      <div class="ps-alan">' +
            '        <label for="psAra">Arama Kriteri</label>' +
            '        <div class="ps-yan">' +
            '          <input type="search" id="psAra" class="form-control input-sm" placeholder="Ad, soyad, T.C. kimlik no, sicil no" />' +
            '          <button type="button" class="btn btn-primary btn-sm" data-ps-listele><span class="glyphicon glyphicon-list" aria-hidden="true"></span> Listele</button>' +
            '          <button type="button" class="btn btn-default btn-sm" data-ps-sec hidden><span class="glyphicon glyphicon-arrow-right" aria-hidden="true"></span> Seç</button>' +
            '          <button type="button" class="btn btn-default btn-sm" data-ps-sec-kapat hidden><span class="glyphicon glyphicon-ok" aria-hidden="true"></span> Seç Kapat</button>' +
            '        </div>' +
            '      </div>' +
            '    </div>' +
            '    <div class="ps-grid" aria-label="Personel Listesi"></div>' +
            '    <p class="ps-not"></p>' +
            '  </div>' +
            '</dialog>').appendTo('body');
        pencere = $p[0];

        $p.on('click', '[data-ps-kapat]', function () { pencere.close(); });
        $p.on('click', '[data-ps-listele]', listele);
        $p.find('#psAra').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); listele(); } });
        $p.on('click', '[data-ps-sec]', function () { secilenleriAktar(false); });
        $p.on('click', '[data-ps-sec-kapat]', function () { secilenleriAktar(true); });

        $p.on('click', '[data-ps-birim-ac]', function () {
            if (birim.kilitli) return;
            var $pn = $p.find('.ps-birim-panel'), ac = $pn.prop('hidden');
            $pn.prop('hidden', !ac);
            $(this).attr('aria-expanded', ac ? 'true' : 'false');
            if (ac && !agac) agacKur();
            if (ac) $p.find('[data-ps-birim-ara]').trigger('focus');
        });
        $p.on('click', '[data-ps-birim-sil]', function () { if (!birim.kilitli) birimYaz(0, ''); });
        $p.find('[data-ps-birim-ara]').on('keydown', function (e) {
            if (e.key !== 'Enter') return;
            e.preventDefault();
            var a = $.trim($(this).val());
            if (a && a.length < 3) { Sgb.bildirim('Aramak için en az 3 harf yazınız.', 'warning'); return; }
            agac.yenile();
        });

        grid = Sgb.grid($p.find('.ps-grid')[0], {
            columnDefs: [
                {
                    colId: 'sec', headerName: '', width: 44, sortable: false, resizable: false,
                    cellRenderer: function (x) {
                        var b = document.createElement('button');
                        b.type = 'button';
                        b.className = 'btn btn-link btn-xs ps-sec-dugme';
                        b.title = 'Seç';
                        b.setAttribute('aria-label', 'Seç: ' + (x.data.adSoyad || ''));
                        b.innerHTML = '<span class="glyphicon glyphicon-arrow-right" aria-hidden="true"></span>';
                        b.addEventListener('click', function () { tekSec(x.data); });
                        return b;
                    }
                },
                { field: 'TCKIMLIKNO', headerName: 'T.C. Kimlik No', width: 115 },
                { field: 'AD', headerName: 'Adı', width: 120 },
                { field: 'SOYAD', headerName: 'Soyadı', width: 120 },
                { field: 'UNVANAD', headerName: 'Unvan', flex: 1, minWidth: 140, tooltipField: 'UNVANAD' },
                { field: 'EPOSTA', headerName: 'ePosta', width: 160, tooltipField: 'EPOSTA' }
            ],
            getRowId: function (x) { return String(x.data.kod); },
            onRowDoubleClicked: function (e) { if (!o.coklu && e.data) tekSec(e.data); },
            overlayNoRowsTemplate: '<span class="text-muted">Arama ölçütlerini girip Listele\'ye basınız.</span>'
        });
    }

    function agacKur() {
        agac = Sgb.agac($p.find('.ps-agac')[0], {
            yukle: function (ust) {
                var ara = ust ? '' : $.trim($p.find('[data-ps-birim-ara]').val());
                return Sgb.al(UC + 'orgagac', ust ? { ust: ust.kod } : { ust: 0, ara: ara }).then(function (v) {
                    if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return []; }
                    return v.satirlar || [];
                });
            },
            secilince: function (d) {
                birimYaz(d.kod, d.birimAd || d.ad);
                $p.find('.ps-birim-panel').prop('hidden', true);
                $p.find('[data-ps-birim-ac]').attr('aria-expanded', 'false');
            },
            ilkAcik: true,
            bosMetin: 'Birim bulunamadı.',
            hata: function (xhr) { hata(xhr, 'Birimler okunamadı.'); }
        });
    }

    function birimYaz(kod, ad) {
        birim.kod = kod || 0;
        birim.ad = ad || '';
        $p.find('#psBirimAd').val(birim.ad).attr('title', birim.ad);
    }

    function listele() {
        var p = { ara: $.trim($p.find('#psAra').val()) };
        // Arama metni birim adlarında da aranır (iş kuralı); kök birim adında geçen kısa metin tüm personeli getirir.
        if (!birim.kod && p.ara.length > 0 && p.ara.length < 3) { Sgb.bildirim('Aramak için en az 3 harf yazınız ya da birim seçiniz.', 'warning'); return; }
        if (birim.kod) p.birim = birim.kod;
        if (o.perTip) p.perTip = o.perTip;
        if (o.tarih) { p.tarih = o.tarih; p.tarih2 = o.tarih2 || o.tarih; }
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', p).done(function (v) {
            if (v.hata) { Sgb.bildirim(v.hata, 'danger'); grid.setGridOption('rowData', []); return; }
            grid.setGridOption('rowData', v.satirlar || []);
        }).fail(function (xhr) { hata(xhr); grid.setGridOption('rowData', []); })
            .always(function () { grid.setGridOption('loading', false); });
    }

    function aktar(satirlar, kapat) {
        if (!satirlar.length) { Sgb.bildirim('Lütfen personel seçiniz.', 'warning'); return; }
        var sonuc = o.secilince ? o.secilince(satirlar) : undefined;
        if (sonuc === false) return;
        if (kapat) pencere.close();
    }

    // Eski PersonelBilgiAktar: satırdaki ok (tek seçim).
    function tekSec(satir) { aktar([satir], o.secKapat !== false); }

    // Eski CokluSecKapat / CokluSecKapatma
    function secilenleriAktar(kapat) { aktar(grid.getSelectedRows(), kapat); }

    Sgb.personelSec = function (ozellikler) {
        o = $.extend({ baslik: 'Personel Seç', coklu: false, secKapat: true }, ozellikler || {});
        if (!$p) kur();

        $p.find('#psBaslik').text(o.baslik);
        var coklu = !!o.coklu;
        grid.setGridOption('rowSelection', coklu
            ? { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true }
            : { mode: 'singleRow', checkboxes: false, enableClickSelection: false });
        grid.setColumnsVisible(['sec'], !coklu);
        grid.deselectAll();
        $p.find('[data-ps-sec], [data-ps-sec-kapat]').prop('hidden', !coklu);
        $p.find('.ps-not').text(coklu
            ? 'Personeli işaretleyip Seç (pencere açık kalır) ya da Seç Kapat düğmesine basınız.'
            : 'Seçmek için satırdaki ok düğmesine ya da satıra çift tıklayınız.');

        // Önerilen birim (eski muhasebe + birim parametreleri)
        birim.kilitli = false;
        $p.find('[data-ps-birim-ac], [data-ps-birim-sil]').prop('disabled', false);
        $p.find('.ps-birim-panel').prop('hidden', true);
        if (o.muhasebe && o.birim) {
            Sgb.al(UC + 'birim', { muhasebe: o.muhasebe, birim: o.birim }).done(function (v) {
                if (!v || !v.kod) return;
                birimYaz(v.kod, v.ad);
                birim.kilitli = !!v.kilitli;
                $p.find('[data-ps-birim-ac], [data-ps-birim-sil]').prop('disabled', birim.kilitli);
            });
        }

        if (!pencere.open) pencere.showModal();
        $p.find('#psAra').trigger('focus');
    };
})(window.Sgb, jQuery);
