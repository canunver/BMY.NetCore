// Form 6 ve Form 7: ekonomik sınıflandırma düzeyinde hizmetlere ilişkin bilgi tablosu (eski ButceForm/HizmetBilgisiEko.aspx).
// p.tamam: 2 = ilk defa yapılması planlanan hizmetler (Form 6), 1 = tamamlanması planlanan hizmetler (Form 7); p.ekyil: yıl farkı.
// Eski tablo FarPoint'ti: satırlar ve altında 20 boş giriş satırı; 06 (Sermaye Giderleri) ve 09 (Yedek Ödenekler) girilemez.
(function ($) {
    'use strict';

    var EKO = ['Personel Giderleri', 'Sos.Güv.Kur. Dev.Primi Gid.', 'Mal Ve Hizmet Alım Gid.', 'Faiz Giderleri', 'Cari Transferler',
        'Sermaye Giderleri', 'Sermaye Transferleri', 'Borç Verme', 'Yedek Ödenekler'];
    var KAPALI = { 6: true, 9: true };
    var BOS_SATIR = 20;

    function bosSatir() { return { baslangic: '', bitis: '', hizmetAdi: '', aciklama: '', e1: null, e2: null, e3: null, e4: null, e5: null, e6: null, e7: null, e8: null, e9: null }; }

    EkForm.tanimla('hizmetBilgisi', {
        ac: function (ctx) {
            var yil = ctx.secim.yil + ctx.p.ekyil;
            var baslik = ctx.p.tamam === 2
                ? yil + ' yılında ilk defa yapılması planlanan hizmetlerin gerektirdiği ödenek teklifleri'
                : yil + ' yılında tamamlanması planlanan hizmetlerin gerektirdiği ödenek teklifleri';
            $('<p class="form-aciklama"></p>').text(baslik).appendTo(ctx.$govde);
            var $t = $('<div class="ekform-grid" aria-label="Hizmetler"></div>').appendTo(ctx.$govde);

            var ekoSutunlari = $.map(EKO, function (ad, i) {
                var no = i + 1;
                return { headerName: (no < 10 ? '0' : '') + no, headerClass: 'orta',
                    children: [EkForm.sayiSutun('e' + no, ad, { width: 115, girilir: !KAPALI[no], headerTooltip: ad })] };
            });
            ctx.tablo = EkForm.grid(ctx, $t[0], {
                columnDefs: [
                    { headerName: '', children: [EkForm.tarihSutun('baslangic', 'Başlama Tarihi', { pinned: 'left' })] },
                    { headerName: '', children: [EkForm.tarihSutun('bitis', 'Bitiş Tarihi', { pinned: 'left' })] },
                    { headerName: '', children: [EkForm.metinSutun('hizmetAdi', 'Hizmet Adı', { width: 200, pinned: 'left', uzun: true })] },
                    { headerName: '', children: [EkForm.metinSutun('aciklama', 'Açıklama', { width: 200, uzun: true })] }
                ].concat(ekoSutunlari),
                headerHeight: 46,
                groupHeaderHeight: 26,
                rowData: []
            });
        },

        listele: function (ctx) {
            return EkForm.al('api/ekform/hizmetbilgisi/liste', { yil: ctx.secim.yil, kur: ctx.secim.kur, tamam: ctx.p.tamam, ekyil: ctx.p.ekyil },
                'Hizmet bilgileri okunamadı.').done(function (v) {
                var liste = $.map(v.satirlar || [], function (s) {
                    var r = { baslangic: s.baslangic, bitis: s.bitis, hizmetAdi: s.hizmetAdi, aciklama: s.aciklama };
                    for (var i = 1; i <= 9; i++) r['e' + i] = s.miktarlar ? s.miktarlar[i - 1] : null;
                    return r;
                });
                for (var i = 0; i < BOS_SATIR; i++) liste.push(bosSatir());
                ctx.tablo.setGridOption('rowData', liste);
            });
        },

        kaydet: function (ctx) {
            var satirlar = $.map(EkForm.satirlar(ctx.tablo), function (r) {
                var m = [];
                for (var i = 1; i <= 9; i++) m.push(r['e' + i] == null ? null : r['e' + i]);
                return { baslangic: r.baslangic, bitis: r.bitis, hizmetAdi: r.hizmetAdi, aciklama: r.aciklama, miktarlar: m };
            });
            return EkForm.kaydet(ctx, 'api/ekform/hizmetbilgisi/kaydet', {
                yil: ctx.secim.yil, kur: ctx.secim.kur, tamam: ctx.p.tamam, ekYil: ctx.p.ekyil, satirlar: satirlar
            });
        },

        yazdir: function (ctx) {
            return EkForm.indir('api/ekform/hizmetbilgisi/yazdir', { yil: ctx.secim.yil, kur: ctx.secim.kur, tamam: ctx.p.tamam });
        }
    });
})(jQuery);
