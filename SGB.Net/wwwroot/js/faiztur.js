// Faiz Türü Tanımlama (AlacakTakip/TanimFaizTur): yıl, faiz türü adı ve oranı. Bul seçili yıla ve yazılan ada göre listeler;
// listeden tıklanan faiz türü forma gelir (eski VeriGoster). Kaydet yeni faiz türü ekler ya da seçileni günceller; kayıt yalnızca
// içinde bulunulan yıla yapılabilir (iş kuralı). Temizle formu boşaltır.
$(function () {
    'use strict';

    if (!$('#grdFaizTur').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var oranBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnAra, #btnTemizle').prop('disabled', evet); }

    var grid = Sgb.grid('#grdFaizTur', {
        columnDefs: [
            { field: 'ad', headerName: 'Oran Adı', flex: 1, minWidth: 220, tooltipField: 'ad' },
            { field: 'oran', headerName: 'Oran', width: 130, type: 'rightAligned',
                valueFormatter: function (p) { return p.value == null ? '' : oranBicim.format(p.value); } }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        overlayNoRowsTemplate: '<span>Yılı seçip Bul\'a basınız.</span>',
        onRowClicked: function (e) { veriGoster(e.data); }
    });

    function yil() { return parseInt($('#txtYil').val(), 10) || 0; }

    // eski VeriGoster(kod, ad, oran)
    function veriGoster(o) {
        $('#hdnKod').val(o ? o.kod : '');
        $('#txtAd').val(o ? o.ad : '');
        $('#txtOran').val(o && o.oran != null ? o.oran : '');
        if (!o) grid.deselectAll();
    }

    // eski Ara(yil, ad)
    function ara(ad, secilecekKod) {
        if (!yil()) { mesaj('Yıl bilgisi girilmemiş!', 'warning'); $('#txtYil').trigger('focus'); return $.Deferred().reject().promise(); }
        grid.setGridOption('loading', true);
        return Sgb.al('api/alacaktakip/faiztur/liste', { yil: yil(), ad: ad || '' })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); grid.setGridOption('rowData', []); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                if (v.bilgi) mesaj(v.bilgi, 'info');
                if (secilecekKod) {
                    var n = grid.getRowNode(String(secilecekKod));
                    if (n) { n.setSelected(true); grid.ensureNodeVisible(n); }
                }
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    $('#frmFaizTur').on('submit', function (e) {
        e.preventDefault();
        ara($.trim($('#txtAd').val()));
    });

    $('#btnTemizle').on('click', function () {
        veriGoster(null);
        $('#txtAd').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        var oranMetin = $.trim($('#txtOran').val());
        var istek = {
            yil: yil(),
            kod: parseInt($('#hdnKod').val(), 10) || 0,
            ad: $.trim($('#txtAd').val()),
            oran: oranMetin === '' ? 0 : parseFloat(oranMetin)
        };
        if (!istek.yil) { mesaj('Yıl bilgisi girilmemiş!', 'warning'); $('#txtYil').trigger('focus'); return; }
        if (!istek.ad) { mesaj('Ad bilgisi girilmemiş!', 'warning'); $('#txtAd').trigger('focus'); return; }
        if (isNaN(istek.oran)) { mesaj('Oran bilgisi girilmemiş veya girilen değer yanlış!', 'warning'); $('#txtOran').trigger('focus'); return; }
        mesgul(true);
        Sgb.gonder('api/alacaktakip/faiztur/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                ara('', istek.kod);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // Açılış: yıl kutusu (eski sayfadaki gibi 2006'dan bu yıla), seçili yıl bu yıl. Liste Bul'a basılınca okunur.
    Sgb.al('api/alacaktakip/faiztur/tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            Sgb.yilKutusu('#txtYil', v.yillar, v.aktifYil);
        })
        .fail(hataGoster);
});
