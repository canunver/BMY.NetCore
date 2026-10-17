// İmza Bilgileri Tanımlama (ButceMuhasebe/Imza): seçilen belge türü ve muhasebe birim kodu için imza yerleri.
// Ad soyad, ünvan / rütbe, görev ünvanı, kullanılmıyor ve tarih yaz tabloda girilir. Uçlar: /api/muhtanim/imza/*.
$(function () {
    'use strict';

    if (!$('#grdImza').length) return;

    var girilir = { editable: true, cellClass: 'mt-girilir' };
    var tablo = Sgb.grid('#grdImza', {
        columnDefs: [
            { field: 'deger', headerName: '#', width: 60, cellClass: 'mt-sari' },
            { field: 'imzaYeri', headerName: 'İmza Yeri', width: 240, cellClass: 'mt-sari' },
            $.extend({ field: 'kullanilmiyor', headerName: 'Kullanılmıyor', width: 120, cellDataType: 'boolean' }, girilir),
            $.extend({ field: 'adSoyad', headerName: 'Ad Soyad', flex: 1, minWidth: 180 }, girilir),
            $.extend({ field: 'unvanRutbe', headerName: 'Ünvan/Rütbe', flex: 1, minWidth: 160 }, girilir),
            $.extend({ field: 'gorevUnvani', headerName: 'Görev Ünvanı', flex: 1, minWidth: 160 }, girilir),
            $.extend({ field: 'tarihYaz', headerName: 'Tarih Yaz', width: 100, cellDataType: 'boolean' }, girilir)
        ],
        defaultColDef: { sortable: false },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true
    });

    function listele() {
        Sgb.mt.oku('api/muhtanim/imza/liste', { belge: $('#imBelge').val(), birim: $.trim($('#imBirim').val()) }, tablo).done(function (v) {
            tablo.setGridOption('rowData', v.satirlar || []);
        });
    }

    $('#imBelge').on('change', listele);
    $('#btnListele').on('click', listele);

    $('#btnKaydet').on('click', function () {
        tablo.stopEditing();
        var satirlar = [];
        tablo.forEachNode(function (n) { satirlar.push(n.data); });
        Sgb.mt.islem('api/muhtanim/imza/kaydet', {
            belge: parseInt($('#imBelge').val(), 10) || 0,
            birim: $.trim($('#imBirim').val()),
            satirlar: satirlar
        });
    });

    listele();
});
