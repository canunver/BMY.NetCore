// Gösterge Tanımları (PerformansButce/TanimSPGosterge): solda göstergeler (AG Grid), sağda seçili göstergenin bilgileri.
// Stratejik Plan'ın Hedef Gösterge penceresinden de (çerçevede) açılır; kapanınca oradaki gösterge listesi yenilenir.
$(function () {
    'use strict';

    if (!$('#grdGosterge').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TUR = { 1: 'Gerç.*100/Hedef', 2: '(Gerç.-Baş.)*100/(Hedef-Baş.)' };
    var YON = { 1: 'Artan', 2: 'Azalan' };
    var tumu = [];

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj('İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnYeni, #btnSil, #btnListele').prop('disabled', evet); if (!evet) $('#btnSil').prop('disabled', !$('#txtKod').val()); }

    // ---------------------------------------------------------------- liste

    var grid = Sgb.grid('#grdGosterge', {
        columnDefs: [
            { field: 'kod', headerName: 'Kod', width: 80 },
            { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 220, tooltipField: 'ad', sort: 'asc' },
            { field: 'olcuBirimAd', headerName: 'Ölçü Birimi', width: 130 },
            { field: 'ilerlemeYonu', headerName: 'Yön', width: 90, valueFormatter: function (p) { return YON[p.value] || ''; } }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { doldur(e.data); }
    });

    function suz() {
        var ara = $.trim($('#txtAra').val()).toLocaleLowerCase('tr');
        grid.setGridOption('rowData', !ara ? tumu : $.grep(tumu, function (g) { return (g.ad || '').toLocaleLowerCase('tr').indexOf(ara) >= 0; }));
    }

    function listele(secilecekKod) {
        grid.setGridOption('loading', true);
        return Sgb.al('api/spgosterge/liste')
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

    // ---------------------------------------------------------------- form

    function temizle() {
        $('#txtKod, #txtAd').val('');
        $('#ddlOlcuBirim').val('');
        $('#ddlTur').val('1');
        $('#ddlIlerlemeYonu').val('1');
        $('#txtOndalik').val('2');
        $('#btnSil').prop('disabled', true);
        grid.deselectAll();
    }

    function doldur(g) {
        $('#txtKod').val(g.kod);
        $('#txtAd').val(g.ad || '');
        $('#ddlOlcuBirim').val(g.olcuBirim ? String(g.olcuBirim) : '');
        $('#ddlTur').val(TUR[g.tur] ? String(g.tur) : '1');
        $('#ddlIlerlemeYonu').val(YON[g.ilerlemeYonu] ? String(g.ilerlemeYonu) : '1');
        $('#txtOndalik').val(g.ondalikSayisi);
        $('#btnSil').prop('disabled', false);
    }

    $('#btnYeni').on('click', function () {
        temizle();
        $('#txtAd').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        var istek = {
            kod: parseInt($('#txtKod').val(), 10) || 0,
            ad: $.trim($('#txtAd').val()),
            olcuBirim: parseInt($('#ddlOlcuBirim').val(), 10) || 0,
            tur: parseInt($('#ddlTur').val(), 10) || 1,
            ilerlemeYonu: parseInt($('#ddlIlerlemeYonu').val(), 10) || 1,
            ondalikSayisi: parseInt($('#txtOndalik').val(), 10) || 0
        };
        if (!istek.ad) { mesaj('Göstergenin adını yazınız.', 'warning'); $('#txtAd').trigger('focus'); return; }
        if (!istek.olcuBirim) { mesaj('Ölçü birimini seçiniz.', 'warning'); $('#ddlOlcuBirim').trigger('focus'); return; }
        mesgul(true);
        Sgb.gonder('api/spgosterge/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                $('#txtKod').val(s.kod || '');
                mesaj(s.bilgi, 'success');
                listele(s.kod);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnSil').on('click', function () {
        var kod = parseInt($('#txtKod').val(), 10) || 0;
        if (!kod) return;
        if (!window.confirm('Seçili gösterge silinsin mi?')) return;
        mesgul(true);
        Sgb.gonder('api/spgosterge/sil', { kod: kod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                temizle();
                mesaj(s.bilgi, 'success');
                listele();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/spgosterge/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            var $o = $('#ddlOlcuBirim').empty().append('<option value=""></option>');
            $.each(t.olculer || [], function (i, o) { $('<option></option>').val(o.kod).text(o.ad).appendTo($o); });
            temizle();
            listele();
        })
        .fail(hataGoster);
});
