// Teminat Mektubu Nevi Tanımlama (HarcamaSurecleri/TanimTeminatNevi): solda liste, sağda form. Uçlar: /api/muhtanim/teminatnevi/*.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return;

    var sayfa = Sgb.mt.tanim({
        tablo: '#grdListe',
        form: '#frmBilgi',
        uc: 'api/muhtanim/teminatnevi/',
        kolonlar: [
            { field: 'KOD', headerName: 'Kod', width: 90, sort: 'asc' },
            { field: 'ADI', headerName: 'Adı', flex: 1, minWidth: 200, tooltipField: 'ADI' },
            { field: 'HESAPKODU', headerName: 'Hesap Kodu', width: 140 }
        ],
        anahtar: 'KOD',
        acilistaListele: true,
        satirdanForma: function (s) { return { kod: s.KOD, adi: s.ADI, hesapKodu: s.HESAPKODU }; },
        silIstek: function (f) { return f.kod ? { kod: f.kod } : null; }
    });

    $('#btnBul').on('click', function () { sayfa.listele(sayfa.form.oku()); });
});
