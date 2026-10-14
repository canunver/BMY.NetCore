// Form 28: hizmet alımı suretiyle çalıştırılan görevli sayısına ilişkin bilgi formu (eski ButceForm/HizmetGorevli.aspx).
// Eski tablo FarPoint'ti: sabit 15 satır (sıra no kilitli); (yıl-2) ve (yıl-1) için özel temizlik, özel güvenlik, şoförlü kiralanan
// taşıt için şoför ve diğer görevlilerin adet, süre ve maliyeti.
(function ($) {
    'use strict';

    var GOREVLER = ['Özel Temizlik Görevlisi', 'Özel Güvenlik Görevlisi', 'Şoförlü Kiralanan Taşıt için Şoför', 'Diğer Görevliler'];
    var ALANLAR = ['Adet', 'Süre', 'Maliyet'];
    var SATIR = 15;
    var DEGER = 24;

    function bosSatir(no) {
        var r = { siraNo: no };
        for (var i = 0; i < DEGER; i++) r['d' + i] = null;
        return r;
    }

    function yilSutunlari(yil, ilk) {
        return {
            headerName: String(yil), headerClass: 'orta',
            children: $.map(GOREVLER, function (g, gi) {
                return {
                    headerName: g, headerClass: 'orta', headerTooltip: g,
                    children: $.map(ALANLAR, function (a, ai) {
                        var i = ilk + gi * 3 + ai;
                        return EkForm.sayiSutun('d' + i, a, { width: ai === 2 ? 105 : 72, headerTooltip: g + ' - ' + a });
                    })
                };
            })
        };
    }

    EkForm.tanimla('hizmetGorevli', {
        ac: function (ctx) {
            var yil = ctx.secim.yil;
            $('<p class="form-aciklama"></p>').text('Hizmet alımı suretiyle çalıştırılan görevli sayısına ilişkin bilgi formu').appendTo(ctx.$govde);
            var $t = $('<div class="ekform-grid" aria-label="Görevliler"></div>').appendTo(ctx.$govde);
            ctx.tablo = EkForm.grid(ctx, $t[0], {
                columnDefs: [
                    { headerName: '', children: [EkForm.metinSutun('siraNo', 'Sıra No', { girilir: false, width: 70, pinned: 'left' })] },
                    yilSutunlari(yil - 2, 0),
                    yilSutunlari(yil - 1, 12)
                ],
                rowData: []
            });
        },

        listele: function (ctx) {
            return EkForm.al('api/ekform/hizmetgorevli/liste', { yil: ctx.secim.yil, kur: ctx.secim.kur },
                'Görevli bilgileri okunamadı.').done(function (v) {
                var liste = [];
                for (var n = 1; n <= SATIR; n++) liste.push(bosSatir(n));
                $.each(v.satirlar || [], function (i, s) {
                    var r = liste[s.siraNo - 1];
                    if (!r) return;
                    for (var j = 0; j < DEGER; j++) r['d' + j] = s.degerler ? s.degerler[j] : null;
                });
                ctx.tablo.setGridOption('rowData', liste);
            });
        },

        kaydet: function (ctx) {
            var satirlar = $.map(EkForm.satirlar(ctx.tablo), function (r) {
                var d = [];
                for (var i = 0; i < DEGER; i++) d.push(r['d' + i] == null ? null : r['d' + i]);
                return { siraNo: r.siraNo, degerler: d };
            });
            return EkForm.kaydet(ctx, 'api/ekform/hizmetgorevli/kaydet', { yil: ctx.secim.yil, kur: ctx.secim.kur, satirlar: satirlar });
        },

        yazdir: function (ctx) {
            return EkForm.indir('api/ekform/hizmetgorevli/yazdir', { yil: ctx.secim.yil, kur: ctx.secim.kur });
        }
    });
})(jQuery);
