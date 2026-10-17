// Eski Kurumsal Yapı Tanımlama (ButceHazirlikv2/TanimKurumsalYapi): üstte kurumsal yapının kodu, adı, muhasebe - birimi ve kurumsal kodu;
// altta liste. Listele formdaki değerleri ölçüt olarak kullanır (eski btnAra_Click), listeden tıklanan kayıt forma gelir (eski VeriGoster).
// Kaydet başarılıysa kaydedilen yapı listelenir. Muhasebe ve birim ortak bileşenle (muhbirim.js) yazılır ya da seçilir.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtBirimKurumsal', ad: 'lblBirimAd' }
    });

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'kod', headerName: 'Kod', width: 160 },
            { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 220, tooltipField: 'ad' },
            { field: 'muhasebe', headerName: 'Muhasebe', width: 110 },
            { field: 'birim', headerName: 'Birim', width: 90 },
            { field: 'kurKod', headerName: 'Kurumsal Kod', width: 150 }
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        overlayNoRowsTemplate: '<span>Ölçüleri yazıp Listele\'ye basınız.</span>',
        onRowClicked: function (e) { goster(e.data); }
    });

    function goster(o) {
        $('#txtKod').val(o.kod || '');
        $('#txtAd').val(o.ad || '');
        muhBirim.ata({ muhasebe: o.muhasebe || '', birim: o.birim || '', kurumsal: '', birimAd: '' });
        if (o.muhasebe) muhBirim.yenile(); // birimin adı okunsun
        $('#ddlKur').val(o.kurKod || '');
    }

    function formOku() {
        var mb = muhBirim.deger();
        return {
            kod: $.trim($('#txtKod').val()),
            ad: $.trim($('#txtAd').val()),
            muhasebe: mb.muhasebe || 0,
            birim: mb.birim || '',
            kurKod: $('#ddlKur').val() || ''
        };
    }

    function listele(o) {
        var $d = $('#btnAra').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/kurumsalyapi/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    Sgb.al('api/kurumsalyapi/tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            var $s = $('#ddlKur');
            $.each(v.kurumlar || [], function (i, k) { $('<option></option>').attr('value', k.kod).text(k.ad).appendTo($s); });
        })
        .fail(hataGoster);

    $('#frmKayit').on('submit', function (e) { e.preventDefault(); listele(formOku()); });

    $('#btnKaydet').on('click', function () {
        var o = formOku();
        if (!o.kod) { mesaj('Kodu yazınız.', 'warning'); $('#txtKod').trigger('focus'); return; }
        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/kurumsalyapi/kaydet', o)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                listele(o);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnTemizle').on('click', function () {
        goster({});
        grid.deselectAll();
        $('#txtKod').trigger('focus');
    });
});
