// Form 10: birimlerin hizmet maliyetinin tespitine ilişkin bilgi formu (eski ButceForm/HizmetMaliyetiTespiti.aspx).
// Eski tablo Ext GridPanel'di: Kod ve Açıklama hiç girilmez; tutarlar yalnızca degerGirilsinMi olan satırlarda girilir
// (diğerleri sarı, alt satırların toplamı olan üst satırlardır). Sütun başlıklarının üstünde yıllar (yıl-2 .. yıl+2) yazar.
// "Detaydan Al" taşra detayındaki tutarları ekrana getirir; kaydetmek için ayrıca Kaydet'e basılmalıdır.
(function ($) {
    'use strict';

    var TUTARLAR = [
        { alan: 'gercek', ad: 'Gerçekleşme', fark: -2 },
        { alan: 'haziran', ad: 'Haziran', fark: -1 },
        { alan: 'tahmin', ad: 'Tahmin', fark: 0 },
        { alan: 'tahmin1', ad: 'Tahmin', fark: 1 },
        { alan: 'tahmin2', ad: 'Tahmin', fark: 2 }
    ];

    function girilir(p) { return !!(p.data && p.data.degerGirilsinMi); }

    function doldur(ctx, v) {
        ctx.tablo.setGridOption('rowData', $.map(v.satirlar || [], function (s) {
            var r = { kod: s.kod, aciklama: s.aciklama, degerGirilsinMi: !!s.degerGirilsinMi };
            $.each(TUTARLAR, function (i, t) { r[t.alan] = s[t.alan] ? s[t.alan] : null; });
            return r;
        }));
    }

    EkForm.tanimla('hizmetMaliyeti', {
        ac: function (ctx) {
            $('<p class="form-aciklama"></p>')
                .text('Birimlerin hizmet maliyetinin tespitine ilişkin bilgi formu. Sarı satırlar alt satırların toplamıdır, girilemez. '
                    + '"Detaydan Al" ile gelen verileri kaydetmeyi unutmayınız.')
                .appendTo(ctx.$govde);
            var $t = $('<div class="ekform-grid" aria-label="Hizmet maliyeti"></div>').appendTo(ctx.$govde);
            var yil = ctx.secim.yil;
            ctx.tablo = EkForm.grid(ctx, $t[0], {
                columnDefs: [
                    { headerName: '', children: [EkForm.metinSutun('kod', 'Kod', { width: 90, girilir: false, pinned: 'left' })] },
                    { headerName: '', children: [EkForm.metinSutun('aciklama', 'Açıklama', { width: 300, girilir: false, pinned: 'left' })] }
                ].concat($.map(TUTARLAR, function (t) {
                    return { headerName: String(yil + t.fark), headerClass: 'orta',
                        children: [EkForm.sayiSutun(t.alan, t.ad, { width: 130, ondalik: true, girilir: girilir })] };
                })),
                rowData: []
            });
        },

        listele: function (ctx) {
            return EkForm.al('api/ekform/hizmetmaliyeti/liste', { yil: ctx.secim.yil, kur: ctx.secim.kur },
                'Hizmet maliyeti bilgileri okunamadı.').done(function (v) { doldur(ctx, v); });
        },

        dugmeler: function () {
            return [{
                ad: 'Detaydan Al', ikon: 'flash', ipucu: 'Taşra detayındaki hizmet maliyeti tutarlarını ekrana getirir',
                tikla: function (ctx) {
                    if (!ctx.secim.yil || !ctx.secim.kur) { Sgb.bildirim('Yıl ve birim seçiniz.', 'warning'); return null; }
                    return EkForm.onay('Detaydan veriler alınacak ve ekrana getirilecektir, daha sonra gelen verileri kaydetmelisiniz. Onaylıyor musunuz?', 'Evet')
                        .then(function () {
                            return EkForm.al('api/ekform/hizmetmaliyeti/detaydanal', { yil: ctx.secim.yil, kur: ctx.secim.kur },
                                'Detay bilgileri okunamadı.').done(function (v) {
                                doldur(ctx, v);
                                EkForm.degisti(ctx);   // gelen veriler henüz saklanmadı
                            });
                        });
                }
            }];
        },

        // Eski btnKaydet: onaydan sonra tüm satırlar gönderilir; süzme (değer girilen, açıklamalı, toplamı > 0) sunucuda yapılır.
        kaydet: function (ctx) {
            return EkForm.onay('Veriler kaydedilecek onaylıyor musunuz?', 'Kaydet').then(function () {
                var satirlar = $.map(EkForm.satirlar(ctx.tablo), function (r) {
                    return { kod: r.kod, aciklama: r.aciklama, degerGirilsinMi: r.degerGirilsinMi,
                        gercek: r.gercek, haziran: r.haziran, tahmin: r.tahmin, tahmin1: r.tahmin1, tahmin2: r.tahmin2 };
                });
                return EkForm.kaydet(ctx, 'api/ekform/hizmetmaliyeti/kaydet', { yil: ctx.secim.yil, kur: ctx.secim.kur, satirlar: satirlar });
            });
        },

        yazdir: function (ctx) {
            return EkForm.indir('api/ekform/hizmetmaliyeti/yazdir', { yil: ctx.secim.yil, kur: ctx.secim.kur });
        }
    });
})(jQuery);
