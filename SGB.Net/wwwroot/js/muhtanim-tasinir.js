// Taşınır Ekonomik Kod (ButceMuhasebe/TasinirEkonomikKod): yılın 150 ve 25 ile başlayan, yansıtması olan hesapları solda
// (kod ve açıklama süzgeçli); seçilen hesabın 830 ile başlayan yansıtma hesapları sağda. Uçlar: /api/muhtanim/tasinir/*.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return;

    var suzgec = { filter: 'agTextColumnFilter', floatingFilter: true, suppressHeaderMenuButton: true, suppressFloatingFilterButton: true, filterParams: { filterOptions: ['contains'], maxNumConditions: 1 } };

    var kodlar = Sgb.grid('#grdKod', {
        columnDefs: [
            { field: 'YKOD', headerName: 'Kullanılabilecek Hesap Kodları', flex: 1, minWidth: 180 }
        ]
    });

    var liste = Sgb.grid('#grdListe', {
        columnDefs: [
            $.extend({ field: 'HKOD', headerName: 'Kod', width: 140 }, suzgec),
            $.extend({ field: 'ACIKLAMA', headerName: 'Açıklama', flex: 1, minWidth: 220, tooltipField: 'ACIKLAMA' }, suzgec)
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        onRowClicked: function (e) { kodlariOku(e.data); }
    });

    function yil() { return parseInt($('#tsYil').val(), 10) || Sgb.mt.buYil; }

    function kodlariTemizle() {
        kodlar.setGridOption('rowData', []);
        $('#tsSecili').text('');
    }

    function listele() {
        kodlariTemizle();
        Sgb.mt.oku('api/muhtanim/tasinir/liste', { yil: yil() }, liste).done(function (v) {
            liste.setGridOption('rowData', v.satirlar || []);
        });
    }

    function kodlariOku(satir) {
        if (!satir) return;
        $('#tsSecili').text('(' + satir.HKOD + ')');
        Sgb.mt.oku('api/muhtanim/tasinir/kodlar', { yil: satir.YIL || yil(), hesapKod: satir.HKOD }, kodlar).done(function (v) {
            kodlar.setGridOption('rowData', v.satirlar || []);
        });
    }

    $('#btnAra').on('click', listele);
    $('#tsYil').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); listele(); } });
    $('#btnSuzTemizle').on('click', function () { liste.setFilterModel(null); });

    // Eski sayfa açılışta bu yılın listesini getiriyordu.
    listele();
});
