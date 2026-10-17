// Ölçü Birimleri (PerformansButce/TanimSPOlcuBirim): solda ölçü birimleri (AG Grid), sağda seçilenin adı.
// Gösterge Tanımları sayfasından da (çerçevede) açılır; kapanınca oradaki ölçü birimi listesi yenilenir.
$(function () {
    'use strict';

    if (!$('#grdOlcu').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var tumu = [];

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj('İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnYeni, #btnSil, #btnListele').prop('disabled', evet); if (!evet) $('#btnSil').prop('disabled', !$('#txtKod').val()); }

    var grid = Sgb.grid('#grdOlcu', {
        columnDefs: [
            { field: 'kod', headerName: 'Kod', width: 80 },
            { field: 'ad', headerName: 'Ölçü Birimi', flex: 1, minWidth: 200, tooltipField: 'ad', sort: 'asc' }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { doldur(e.data); }
    });

    function suz() {
        var ara = $.trim($('#txtAra').val()).toLocaleLowerCase('tr');
        grid.setGridOption('rowData', !ara ? tumu : $.grep(tumu, function (o) { return (o.ad || '').toLocaleLowerCase('tr').indexOf(ara) >= 0; }));
    }

    function listele(secilecekKod) {
        grid.setGridOption('loading', true);
        return Sgb.al('api/spgosterge/olcu-liste')
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                tumu = veri.satirlar || [];
                suz();
                if (secilecekKod) {
                    var n = grid.getRowNode(String(secilecekKod));
                    if (n) { n.setSelected(true); grid.ensureNodeVisible(n); }
                }
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    $('#frmArama').on('submit', function (e) {
        e.preventDefault();
        listele();
    });
    $('#txtAra').on('input', suz);

    function temizle() {
        $('#txtKod, #txtAd').val('');
        $('#btnSil').prop('disabled', true);
        grid.deselectAll();
    }

    function doldur(o) {
        $('#txtKod').val(o.kod);
        $('#txtAd').val(o.ad || '');
        $('#btnSil').prop('disabled', false);
    }

    $('#btnYeni').on('click', function () {
        temizle();
        $('#txtAd').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        var istek = { kod: parseInt($('#txtKod').val(), 10) || 0, ad: $.trim($('#txtAd').val()) };
        if (!istek.ad) { mesaj('Ölçü biriminin adını yazınız.', 'warning'); $('#txtAd').trigger('focus'); return; }
        mesgul(true);
        Sgb.gonder('api/spgosterge/olcu-kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                if (s.kod) $('#txtKod').val(s.kod);
                mesaj(s.bilgi, 'success');
                listele(s.kod);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnSil').on('click', function () {
        var kod = parseInt($('#txtKod').val(), 10) || 0;
        if (!kod) return;
        if (!window.confirm('Seçili ölçü birimi silinsin mi?')) return;
        mesgul(true);
        Sgb.gonder('api/spgosterge/olcu-sil', { kod: kod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                temizle();
                mesaj(s.bilgi, 'success');
                listele();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    temizle();
    listele();
});
