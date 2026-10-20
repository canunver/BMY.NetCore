// Faiz Türü Tanımlama (AlacakTakip/TanimFaizTur). Faiz türünün anahtarı yıl + kod (kod her yıl için sayaçla verilir); ekranda
// ikisi birlikte "yıl-kod" (ör. 2026-3) gösterilir. Solda bütün yılların faiz türleri listelenir (açılışta okunur, Ara ile süzülür);
// tıklanan faiz türü sağdaki forma gelir. Yeni: içinde bulunulan yıl gelir, faiz türü ve oran elle girilir. Kaydet yeni faiz türü
// ekler ya da seçilenin adını / oranını günceller. Kayıt yalnızca içinde bulunulan yıla yapılabilir (sunucudaki iş kuralı).
$(function () {
    'use strict';

    if (!$('#grdFaizTur').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var oranBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var aktifYil = new Date().getFullYear();

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnYeni').prop('disabled', evet); }
    function yilKod(yil, kod) { return yil + '-' + kod; }

    var grid = Sgb.grid('#grdFaizTur', {
        columnDefs: [
            { colId: 'yilKod', headerName: 'Kod', width: 110, sort: 'desc',
                valueGetter: function (p) { return p.data ? yilKod(p.data.yil, p.data.kod) : ''; },
                // yıl, sonra kod sayısal sıralansın (2026-10, 2026-9'dan sonra gelsin)
                comparator: function (a, b, na, nb) {
                    var x = na.data, y = nb.data;
                    return x.yil !== y.yil ? x.yil - y.yil : x.kod - y.kod;
                } },
            { field: 'ad', headerName: 'Faiz Türü', flex: 1, minWidth: 220, tooltipField: 'ad' },
            { field: 'oran', headerName: 'Oran', width: 120, type: 'rightAligned',
                valueFormatter: function (p) { return p.value == null ? '' : oranBicim.format(p.value); } }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return yilKod(p.data.yil, p.data.kod); },
        overlayNoRowsTemplate: '<span>Faiz türü yok.</span>',
        onRowClicked: function (e) { veriGoster(e.data); }
    });

    // Forma faiz türünü yazar; o boşsa yeni kayıt (içinde bulunulan yıl, kod kayıtta verilir).
    function veriGoster(o) {
        $('#hdnYil').val(o ? o.yil : aktifYil);
        $('#hdnKod').val(o ? o.kod : '');
        $('#txtYilKod').val(o ? yilKod(o.yil, o.kod) : aktifYil + '-(yeni)');
        $('#txtAd').val(o ? o.ad : '');
        $('#txtOran').val(o && o.oran != null ? o.oran : '');
        if (!o) grid.deselectAll();
    }

    function listele(secilecek) {
        grid.setGridOption('loading', true);
        return Sgb.al('api/alacaktakip/faiztur/liste', { yil: 0, ad: '' })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); grid.setGridOption('rowData', []); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                if (secilecek) {
                    var n = grid.getRowNode(secilecek);
                    if (n) { n.setSelected(true); grid.ensureNodeVisible(n); veriGoster(n.data); }
                }
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    $('#txtSuz').on('input', function () { grid.setGridOption('quickFilterText', this.value); });

    $('#btnYeni').on('click', function () {
        veriGoster(null);
        $('#txtAd').trigger('focus');
    });

    $('#frmFaizTur').on('submit', function (e) {
        e.preventDefault();
        var oranMetin = $.trim($('#txtOran').val());
        var istek = {
            yil: parseInt($('#hdnYil').val(), 10) || aktifYil,
            kod: parseInt($('#hdnKod').val(), 10) || 0,
            ad: $.trim($('#txtAd').val()),
            oran: oranMetin === '' ? 0 : parseFloat(oranMetin)
        };
        if (!istek.ad) { mesaj('Faiz türü girilmemiş!', 'warning'); $('#txtAd').trigger('focus'); return; }
        if (isNaN(istek.oran)) { mesaj('Oran bilgisi girilmemiş veya girilen değer yanlış!', 'warning'); $('#txtOran').trigger('focus'); return; }
        mesgul(true);
        Sgb.gonder('api/alacaktakip/faiztur/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                // yeni kayıtta kod sunucuda verilir: aynı yıl ve addaki en büyük kodlu satır seçilir
                listele(istek.kod ? yilKod(istek.yil, istek.kod) : null).done(function () {
                    if (istek.kod) return;
                    var son = null;
                    grid.forEachNode(function (n) {
                        if (n.data.yil === istek.yil && n.data.ad === istek.ad && (!son || n.data.kod > son.data.kod)) son = n;
                    });
                    if (son) { son.setSelected(true); grid.ensureNodeVisible(son); veriGoster(son.data); }
                });
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // Açılış: içinde bulunulan yıl (sunucudan), liste okunur, form yeni kayda hazır.
    Sgb.al('api/alacaktakip/faiztur/tanim')
        .done(function (v) { if (v && v.aktifYil) aktifYil = v.aktifYil; })
        .always(function () { veriGoster(null); listele(); });
});
