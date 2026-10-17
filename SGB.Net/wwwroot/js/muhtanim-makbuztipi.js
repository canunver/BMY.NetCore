// Makbuz Tipi Bilgileri (ButceMuhasebe/TanimMakbuzTipiYeni): solda liste, sağda form (Durum: Aktif / Pasif).
// Uçlar: /api/muhtanim/makbuztipi/*.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return;

    var kolonlar = [
        { field: 'aciklama', headerName: 'Açıklama', flex: 1, minWidth: 200, tooltipField: 'aciklama' },
        { field: 'hesapKod', headerName: 'Hesap Kodu', width: 130 },
        { field: 'krediHesapKod', headerName: 'Kart Ödeme Hesap Kod', width: 150 },
        { field: 'kdv', headerName: 'KDV(%)', width: 80, type: 'rightAligned' },
        { field: 'kdvHesapKod', headerName: 'KDV Hesap Kodu', width: 130 },
        { field: 'kasaHesapKod', headerName: 'Kasa Hesap Kodu', width: 130 },
        { field: 'yansitmaHesapKod', headerName: 'Yansıtma Hesap Kodu', width: 150 }
    ];
    kolonlar.push({ field: 'durumAd', headerName: 'Durum', width: 80 });

    var sayfa = Sgb.mt.tanim({
        tablo: '#grdListe',
        form: '#frmBilgi',
        uc: 'api/muhtanim/makbuztipi/',
        kolonlar: kolonlar,
        anahtar: 'kod',
        varsayilan: { durum: '1' },
        acilistaListele: true,
        istek: function (f) {
            f.kod = parseInt(f.kod, 10) || 0;
            f.durum = parseInt(f.durum, 10) || 1;
            return f;
        },
        silIstek: function (f) {
            var kod = parseInt(f.kod, 10) || 0;
            return kod > 0 ? { kod: kod } : null;
        }
    });

    // Bul: formdaki değerler kıstastır (KDV boşsa aranmaz).
    $('#btnBul').on('click', function () {
        var f = sayfa.form.oku();
        delete f.kod;
        sayfa.listele(f);
    });
});
