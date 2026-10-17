// Kapalı Birim Bilgileri (ButceMuhasebe/KapaliMuhasebeBirimForm): kapalı muhasebe birimlerinin listesi, birim kapatma / açma
// ve kapatılan birimin personelini başka birime taşıma. Uçlar: /api/muhtanim/kapalibirim/*.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return;

    var form = Sgb.mt.form('#frmBilgi');
    var tablo = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'muhasebe', headerName: 'Muhasebe Kodu', width: 120 },
            { field: 'birimKod', headerName: 'Birim Kodu', width: 100 },
            { field: 'kurum', headerName: 'Kurumsal Kod', width: 140 },
            { field: 'MuhasebeBirimAd', headerName: 'Birim Ad', flex: 1, minWidth: 200, tooltipField: 'MuhasebeBirimAd' }
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        onRowClicked: function (e) { form.yaz(e.data); }
    });

    function listele() {
        Sgb.mt.oku('api/muhtanim/kapalibirim/liste', form.oku(), tablo).done(function (v) {
            tablo.setGridOption('rowData', v.satirlar || []);
        });
    }

    function acKapat(kapat) {
        var f = form.oku();
        if (!f.muhasebe || !f.birimKod) { Sgb.bildirim('Muhasebe ve birim kodunu giriniz.', 'warning'); return; }
        f.kapat = kapat;
        Sgb.mt.islem('api/muhtanim/kapalibirim/ackapat', f,
            f.muhasebe + '-' + f.birimKod + ' birimi ' + (kapat ? 'kapatılacak' : 'açılacak') + '. Emin misiniz?').done(listele);
    }

    $('#btnListele').on('click', listele);
    $('#btnKapat').on('click', function () { acKapat(true); });
    $('#btnAc').on('click', function () { acKapat(false); });
    $('#btnTemizle').on('click', function () { form.temizle(); tablo.deselectAll(); });

    var pencere = Sgb.mt.pencere('#dlgTasi');
    $('#btnVeriAktar').on('click', function () { pencere.showModal(); });
    $('#btnPersonelTasi').on('click', function () {
        var f = form.oku();
        var istek = { muhasebe: f.muhasebe, birimKod: f.birimKod, yeniMuhasebe: $.trim($('#kbYeniSay').val()), yeniBirim: $.trim($('#kbYeniBirim').val()) };
        if (!istek.muhasebe || !istek.birimKod) { Sgb.bildirim('Taşınacak muhasebe ve birim kodunu formda giriniz.', 'warning'); return; }
        if (!istek.yeniMuhasebe || !istek.yeniBirim) { Sgb.bildirim('Aktarılacağı saymanlık ve birimi giriniz.', 'warning'); return; }
        Sgb.mt.islem('api/muhtanim/kapalibirim/personel-tasi', istek,
            istek.muhasebe + '-' + istek.birimKod + ' birimindeki personel ' + istek.yeniMuhasebe + '-' + istek.yeniBirim + ' birimine taşınacak. Emin misiniz?')
            .done(function () { pencere.close(); });
    });
});
