// KBS Aktarım Tanımlama (ButceMuhasebe/TanimKBSAktarim): yılın KBS aktarım hesapları (göster / gösterme, hesap / fon / fin. kod
// uzunlukları) tabloda düzenlenir; Detay İşlem penceresinde detay hesap kodları girilir. Uçlar: /api/muhtanim/kbs/*.
$(function () {
    'use strict';

    if (!$('#grdKbs').length) return;

    var GOSTER = { 0: 'Göster', 1: 'Gösterme' };
    var sayiKolon = { editable: true, cellClass: 'mt-girilir', width: 150, type: 'rightAligned', cellEditor: 'agNumberCellEditor', cellEditorParams: { min: 0, precision: 0 } };

    var tablo = Sgb.grid('#grdKbs', {
        columnDefs: [
            { field: 'HESAPKOD', headerName: 'Hesap Kodu', width: 160, cellClass: 'mt-sari' },
            { field: 'HESAPADI', headerName: 'Hesap Adı', flex: 1, minWidth: 220, cellClass: 'mt-sari', tooltipField: 'HESAPADI' },
            {
                field: 'GOSTER', headerName: 'Göster', width: 120, editable: true, cellClass: 'mt-girilir',
                cellEditor: 'agSelectCellEditor', cellEditorParams: { values: [0, 1] },
                valueFormatter: function (p) { return GOSTER[p.value] || ''; },
                valueParser: function (p) { return parseInt(p.newValue, 10) || 0; }
            },
            $.extend({ field: 'HESAPUZUNLUK', headerName: 'Hesap Uzunluğu' }, sayiKolon),
            $.extend({ field: 'FONUZUNLUK', headerName: 'Fon.Kod Uzunluğu' }, sayiKolon),
            $.extend({ field: 'FINUZUNLUK', headerName: 'Fin.Kod Uzunluğu' }, sayiKolon)
        ],
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true
    });

    function yil() { return parseInt($('#kbsYil').val(), 10) || Sgb.mt.buYil; }

    function listele() {
        Sgb.mt.oku('api/muhtanim/kbs/liste', { yil: yil() }, tablo).done(function (v) {
            tablo.setGridOption('rowData', v.satirlar || []);
        });
    }

    function satirlar(t) {
        t.stopEditing();
        var l = [];
        t.forEachNode(function (n) { l.push(n.data); });
        return l;
    }

    $('#btnListele').on('click', listele);
    $('#btnKaydet').on('click', function () {
        Sgb.mt.islem('api/muhtanim/kbs/kaydet', { yil: yil(), satirlar: $.map(satirlar(tablo), function (s) {
            return { HESAPKOD: s.HESAPKOD, HESAPADI: s.HESAPADI, GOSTER: parseInt(s.GOSTER, 10) || 0, HESAPUZUNLUK: parseInt(s.HESAPUZUNLUK, 10) || 0,
                FONUZUNLUK: parseInt(s.FONUZUNLUK, 10) || 0, FINUZUNLUK: parseInt(s.FINUZUNLUK, 10) || 0 };
        }) }, 'Kaydedilecek, onaylıyor musunuz?');
    });

    // ---------------------------------------------------------------- detay hesap kodları

    var pencere = Sgb.mt.pencere('#dlgDetay');
    var detay = null;

    $('#btnDetay').on('click', function () {
        pencere.showModal();
        if (!detay) {
            detay = Sgb.grid('#grdDetay', {
                columnDefs: [{ field: 'HESAPKOD', headerName: 'Hesap Kodu', flex: 1, editable: true, cellClass: 'mt-girilir', sortable: false }],
                singleClickEdit: true,
                stopEditingWhenCellsLoseFocus: true
            });
        }
        Sgb.mt.oku('api/muhtanim/kbs/detay', { yil: yil() }, detay).done(function (v) {
            detay.setGridOption('rowData', v.satirlar || []);
        });
    });

    $('#btnDetayKaydet').on('click', function () {
        if (!detay) return;
        Sgb.mt.islem('api/muhtanim/kbs/detay-kaydet', { yil: yil(), satirlar: $.map(satirlar(detay), function (s) { return { HESAPKOD: s.HESAPKOD || '' }; }) }, 'Kaydedilecek, onaylıyor musunuz?');
    });

    listele();
});
