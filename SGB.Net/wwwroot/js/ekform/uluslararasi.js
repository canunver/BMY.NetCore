// Form 17: uluslararası kuruluşlar bilgi formu (eski ButceForm/UluslararasiKuruluslar.aspx).
// Üstte 07.2.2 ve 05.6.2 ekonomik kodlu tertiplerin toplamları (salt okunur), altında kuruluşlar tablosu (eski FarPoint,
// satırların altında 20 boş giriş satırı). Sıra numarası boş olan satırlar saklanmaz; sayılar tam sayı olarak saklanır.
(function ($) {
    'use strict';

    var BOS_SATIR = 20;
    var SAYILAR = ['siraNo', 'toplantiSayisi', 'toplantiKatsayisi', 'oncekiyilDoviz', 'oncekiyilAidat', 'hazirlikYilDoviz', 'hazirlikYilAidat',
        'ertesiYilDoviz', 'ertesiYilAidat', 'ertesiYil2Doviz', 'ertesiYil2Aidat'];
    var METINLER = ['kurulusAd', 'yasalDayanak', 'dovizCinsi', 'aciklama'];

    function bosSatir() {
        var r = {};
        $.each(SAYILAR, function (i, a) { r[a] = null; });
        $.each(METINLER, function (i, a) { r[a] = ''; });
        return r;
    }

    function toplamSutun(field, baslik) { return EkForm.sayiSutun(field, baslik, { width: 130, girilir: false }); }

    function aidatCifti(yil, doviz, tl) {
        return { headerName: String(yil), headerClass: 'orta', children: [
            EkForm.sayiSutun(doviz, 'Miktar Döviz', { width: 110 }),
            EkForm.sayiSutun(tl, 'Miktar TL', { width: 110 })
        ] };
    }

    EkForm.tanimla('uluslararasi', {
        ac: function (ctx) {
            var yil = ctx.secim.yil;

            var $b1 = $('<section class="ekform-bolum mavi"></section>').appendTo(ctx.$govde);
            $('<h3></h3>').text('07.2.2 ve 05.6.2 Ekonomik Kodlu Tertiplerin Toplamı').appendTo($b1);
            var $t1 = $('<div class="ekform-grid otomatik" aria-label="Tertip toplamları"></div>').appendTo($('<div class="bolum-ic"></div>').appendTo($b1));
            ctx.toplamTablo = EkForm.grid(ctx, $t1[0], {
                columnDefs: [
                    { headerName: String(yil - 2), headerClass: 'orta', children: [toplamSutun('oncYilOdenek', 'Baş.Ödeneği'), toplamSutun('oncYilHarcama', 'Harcama')] },
                    { headerName: String(yil - 1), headerClass: 'orta', children: [toplamSutun('yilOdenek', 'Baş.Ödeneği'), toplamSutun('yilHazHarcama', 'Haziran Harcaması'),
                        toplamSutun('yilSHTah', 'Yıl Sonu Harcama Tahmini')] },
                    { headerName: String(yil), headerClass: 'orta', children: [toplamSutun('yil1Teklif', 'Teklif')] },
                    { headerName: String(yil + 1), headerClass: 'orta', children: [toplamSutun('yil2Teklif', 'Teklif')] },
                    { headerName: String(yil + 2), headerClass: 'orta', children: [toplamSutun('yil3Teklif', 'Teklif')] }
                ],
                domLayout: 'autoHeight',
                rowData: []
            });

            var $b2 = $('<section class="ekform-bolum yesil"></section>').appendTo(ctx.$govde);
            $('<h3></h3>').text('Uluslararası kuruluşlar').appendTo($b2);
            var $t2 = $('<div class="ekform-grid" aria-label="Uluslararası kuruluşlar"></div>').appendTo($('<div class="bolum-ic"></div>').appendTo($b2));
            ctx.tablo = EkForm.grid(ctx, $t2[0], {
                columnDefs: [
                    { headerName: '', children: [EkForm.sayiSutun('siraNo', 'Sıra no', { width: 80, pinned: 'left' })] },
                    { headerName: '', children: [EkForm.metinSutun('kurulusAd', 'Uluslararası Kuruluşun Adı', { width: 220, pinned: 'left', uzun: true })] },
                    { headerName: '', children: [EkForm.metinSutun('yasalDayanak', 'Kuruluşa Üyeliğin Yasal Dayanağı (Kanun, Karar, Anlaşma, Protokol vb.)',
                        { width: 220, uzun: true, headerTooltip: 'Kuruluşa Üyeliğin Yasal Dayanağı (Kanun, Karar, Anlaşma, Protokol vb.)' })] },
                    { headerName: String(yil - 2), headerClass: 'orta', children: [
                        EkForm.sayiSutun('toplantiSayisi', 'Toplantı Sayısı', { width: 100 }),
                        EkForm.sayiSutun('toplantiKatsayisi', 'Toplantıya Katılan Sayısı', { width: 120, headerTooltip: 'Toplantıya Katılan Sayısı' })
                    ] },
                    { headerName: 'Yıllık Aidat veya Katkı Payı', headerClass: 'orta', children: [
                        { headerName: '', children: [EkForm.metinSutun('dovizCinsi', 'Döviz Cinsi', { width: 100 })] },
                        aidatCifti(yil - 1, 'oncekiyilDoviz', 'oncekiyilAidat'),
                        aidatCifti(yil, 'hazirlikYilDoviz', 'hazirlikYilAidat'),
                        aidatCifti(yil + 1, 'ertesiYilDoviz', 'ertesiYilAidat'),
                        aidatCifti(yil + 2, 'ertesiYil2Doviz', 'ertesiYil2Aidat')
                    ] },
                    { headerName: '', children: [EkForm.metinSutun('aciklama', 'Açıklama', { width: 220, uzun: true })] }
                ],
                rowData: []
            });
        },

        listele: function (ctx) {
            return EkForm.al('api/ekform/uluslararasi/liste', { yil: ctx.secim.yil, kur: ctx.secim.kur, senaryo: ctx.secim.senaryo },
                'Uluslararası kuruluş bilgileri okunamadı.').done(function (v) {
                ctx.toplamTablo.setGridOption('rowData', v.toplamlar ? [v.toplamlar] : []);
                var liste = $.map(v.satirlar || [], function (s) {
                    var r = bosSatir();
                    $.each(METINLER, function (i, a) { r[a] = s[a] || ''; });
                    // Eski tabloda sıfırlar da yazılırdı; boş hücre sıfır sayıldığı için sıfır boş gösterilir.
                    $.each(SAYILAR, function (i, a) { r[a] = s[a] ? s[a] : null; });
                    return r;
                });
                for (var i = 0; i < BOS_SATIR; i++) liste.push(bosSatir());
                ctx.tablo.setGridOption('rowData', liste);
                if (v.uyari) Sgb.bildirim(v.uyari, 'warning');
            });
        },

        kaydet: function (ctx) {
            var satirlar = $.map(EkForm.satirlar(ctx.tablo), function (r) {
                var s = {};
                $.each(SAYILAR, function (i, a) { s[a] = r[a] == null ? null : r[a]; });
                $.each(METINLER, function (i, a) { s[a] = r[a] || ''; });
                return s;
            });
            return EkForm.kaydet(ctx, 'api/ekform/uluslararasi/kaydet', { yil: ctx.secim.yil, kur: ctx.secim.kur, satirlar: satirlar });
        },

        yazdir: function (ctx) {
            return EkForm.indir('api/ekform/uluslararasi/yazdir', { yil: ctx.secim.yil, kur: ctx.secim.kur, senaryo: ctx.secim.senaryo });
        }
    });
})(jQuery);
