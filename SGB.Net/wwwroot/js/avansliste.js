// Avans Kapatma (Avans Listesi), ButceMuhasebe/AvansListeYeni (eski aynı adlı Ext.NET sayfası): kıstaslarla kişi / firma bazında
// verilen, alınan, kalan avanslar listelenir; Yazdır Excel'e döker (MUH/AvansListesi.xlt).
// Uçlar: /api/avans/tanim, /api/avans/liste, /api/avans/liste/yazdir (Muhasebe/AvansUclari.cs). Birim seçimi Sgb.muhasebeBirim.
$(function () {
    'use strict';

    if (!$('.avans-liste').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/avans/';
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function para(x) { return x.value === null || x.value === undefined || x.value === '' ? '' : tutar2.format(Number(x.value) || 0); }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim' },
        yil: function () { return $('#txtYil').val(); }
    });

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'muhasebe', headerName: 'Muhasebe Adı', minWidth: 140, flex: 2 },
            { field: 'birim', headerName: 'Birim Adı', minWidth: 140, flex: 2 },
            { field: 'tCVno', headerName: 'TC Vno', minWidth: 110, flex: 1 },
            { field: 'adSoyadUnvan', headerName: 'İlgili', minWidth: 200, flex: 3 },
            { field: 'verilen', headerName: 'Verilen', minWidth: 120, flex: 1, type: 'rightAligned', valueFormatter: para },
            { field: 'alinan', headerName: 'Alınan', minWidth: 120, flex: 1, type: 'rightAligned', valueFormatter: para },
            { field: 'kalan', headerName: 'Kalan', minWidth: 120, flex: 1, type: 'rightAligned', valueFormatter: para }
        ],
        overlayNoRowsTemplate: '<span>Kıstasları girip Listele\'ye basınız.</span>'
    });

    function kriter() {
        var mb = muhBirim.deger();
        return {
            yil: parseInt($('#txtYil').val(), 10) || 0,
            muhasebe: mb.muhasebe || '',
            birim: mb.birim,
            kimlikNo: $.trim($('#txtKimlikNo').val()),
            ad: $.trim($('#txtAd').val())
        };
    }

    function ozetYaz(satirlar) {
        if (!satirlar || !satirlar.length) { $('#lblOzet').text(''); return; }
        var v = 0, a = 0;
        $.each(satirlar, function (i, r) { v += Number(r.verilen) || 0; a += Number(r.alinan) || 0; });
        $('#lblOzet').text(satirlar.length + ' kayıt. Verilen ' + tutar2.format(v) + ', Alınan ' + tutar2.format(a) + ', Kalan ' + tutar2.format(v - a));
    }

    function listele() {
        var k = kriter();
        if (!k.yil) { mesaj('Anahtar alanları girmelisiniz.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', k)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
                ozetYaz(v.satirlar);
                if (v.bilgi) mesaj(v.bilgi, 'info');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); $d.prop('disabled', false); });
    }

    $('#btnListele').on('click', listele);
    $('#frmKriter').on('keydown', 'input', function (e) {
        if (e.key === 'Enter' && !$(this).closest('#muhBirim').length) { e.preventDefault(); listele(); }
    });

    $('#btnYazdir').on('click', function () {
        var k = kriter();
        if (!k.yil) { mesaj('Anahtar alanları girmelisiniz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir(UC + 'liste/yazdir', k).then(function () { $d.prop('disabled', false); }, function () { $d.prop('disabled', false); });
    });

    Sgb.al(UC + 'tanim')
        .done(function (t) { Sgb.yilKutusu('#txtYil', t.yillar, t.yil); })
        .fail(hataGoster);

    // Tablo sayfanın kalan yüksekliğini doldurur.
    function yay() {
        var el = document.getElementById('grdListe');
        if (!el || el.offsetParent === null) return;
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(260, Math.floor(window.innerHeight - ust - 24)) + 'px';
    }
    $(window).on('resize', yay);
    yay();
});
