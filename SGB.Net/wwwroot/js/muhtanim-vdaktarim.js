// Damga Vergisi / KDV Tevkifatı / KDV / Muhtasar Aktarım Tür Tanımlama (ButceMuhasebe/TanimVergiDairesiAktarimTur?tur=1..4).
// Solda liste, sağda form. Uçlar: /api/muhtanim/vdaktarim/*.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return;

    var tur = parseInt($('.muh-tanim').attr('data-tur'), 10) || 0;
    var sira = 0; // tanımsız (olmayan) satırların tekil kimliği

    function param(bul) {
        var p = { tur: tur, olmayanEkle: $('#vdOlmayan').prop('checked') ? 1 : 0 };
        if (bul) $.extend(p, { bul: 1 }, sayfa.form.oku());
        delete p.kod;
        return p;
    }

    var sayfa = Sgb.mt.tanim({
        tablo: '#grdListe',
        form: '#frmBilgi',
        uc: 'api/muhtanim/vdaktarim/',
        kolonlar: [
            { field: 'KOD', headerName: 'ID', width: 80 },
            { field: 'TURKOD', headerName: 'Kodu', width: 90 },
            { field: 'ADI', headerName: 'Adı', flex: 1, minWidth: 180, tooltipField: 'ADI' },
            { field: 'HESAPKOD', headerName: 'Hesap Kodu', width: 140 },
            { field: 'ORANI', headerName: 'Oranı', width: 90, type: 'rightAligned' }
        ],
        anahtar: '_id',
        // Satır kimliği: kayıtlıysa KOD, tanımsız (olmayan) satırda sıra (KOD = 0).
        hazirla: function (satirlar) {
            $.each(satirlar, function (i, s) { s._id = s.KOD > 0 ? 'k' + s.KOD : 'o' + (++sira); });
        },
        varsayilan: { kod: '0' },
        acilistaListele: true,
        listeParam: function () { return param(false); },
        satirdanForma: function (s) { return { kod: s.KOD, turKod: s.TURKOD, adi: s.ADI, hesapKod: s.HESAPKOD, orani: s.ORANI }; },
        istek: function (f) { return { tur: tur, kod: parseInt(f.kod, 10) || 0, turKod: f.turKod, adi: f.adi, hesapKod: f.hesapKod, orani: f.orani }; },
        silIstek: function (f) {
            var kod = parseInt(f.kod, 10) || 0;
            return kod > 0 ? { tur: tur, kod: kod } : null;
        }
    });

    $('#btnBul').on('click', function () { sayfa.listele(param(true)); });
    $('#vdOlmayan').on('change', function () { sayfa.listele(); });

});
