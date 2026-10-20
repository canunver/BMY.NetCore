// Müdürlük Bilgileri (ButceMuhasebe/TanimMudurluk; eski aynı adlı Ext.NET sayfası): solda liste, sağda form.
// Fatura Hazırlama / Fatura Giriş'teki müdürlük kodları buradan tanımlanır. Uçlar: /api/muhtanim/mudurluk/*.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return;

    var sayfa = Sgb.mt.tanim({
        tablo: '#grdListe',
        form: '#frmBilgi',
        uc: 'api/muhtanim/mudurluk/',
        kolonlar: [
            { field: 'kod', headerName: 'Kod', width: 90, sort: 'asc' },
            { field: 'ad', headerName: 'Adı', flex: 1, minWidth: 200, tooltipField: 'ad' },
            { field: 'aktif', headerName: 'Aktif', width: 90, valueFormatter: function (p) { return p.value > 0 ? 'Evet' : 'Hayır'; } }
        ],
        anahtar: 'kod',
        acilistaListele: true,
        varsayilan: { aktif: true },
        satirdanForma: function (s) { return { kod: s.kod, ad: s.ad, aktif: s.aktif > 0 }; },
        silIstek: function (f) { return f.kod ? { kod: f.kod } : null; }
    });

    $('#btnBul').on('click', function () { sayfa.listele({}); });
});
