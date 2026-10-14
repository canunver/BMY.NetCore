// Form 11: fiziksel değerler bilgi tablosu (eski ButceForm/FizikselDegerler.aspx).
// Eski tablo FarPoint'ti: Kod ve Değerler (açıklama) girilmez; değer girilmeyen (degerGirilsinMi = false) satırlar tümüyle
// kilitli ve sarıdır. Kod ve açıklama koda göre girintili yazılır. Değerler tam sayıdır (yıl-3 .. yıl).
(function ($) {
    'use strict';

    var FARKLAR = [-3, -2, -1, 0];

    function girilir(p) { return !!(p.data && p.data.degerGirilsinMi); }
    function girinti(p) { return p.data && p.data.girinti ? { paddingLeft: (p.data.girinti + 6) + 'px' } : null; }

    // Eski hücreler tam sayı biçimindeydi; ondalıklı ya da geçersiz giriş kabul edilmez.
    function tamSayiParser(p) {
        var n = EkForm.sayiCoz(p.newValue);
        if (n === undefined) return p.oldValue;
        if (n !== null && Math.floor(n) !== n) { Sgb.bildirim('Değerleri tam sayı olarak yazınız.', 'warning'); return p.oldValue; }
        return n;
    }

    EkForm.tanimla('fizikselDegerler', {
        ac: function (ctx) {
            $('<p class="form-aciklama"></p>').text('Birimin fiziksel değerleri (adet, kişi, m² vb.). Sarı satırlara değer girilmez.').appendTo(ctx.$govde);
            var $t = $('<div class="ekform-grid" aria-label="Fiziksel değerler"></div>').appendTo(ctx.$govde);
            var yil = ctx.secim.yil;
            ctx.tablo = EkForm.grid(ctx, $t[0], {
                columnDefs: [
                    EkForm.metinSutun('kod', 'Kod', { width: 110, girilir: false, pinned: 'left', cellStyle: girinti }),
                    EkForm.metinSutun('aciklama', 'Değerler', { width: 380, girilir: false, cellStyle: girinti })
                ].concat($.map(FARKLAR, function (f, i) {
                    return EkForm.sayiSutun('d' + i, String(yil + f), { width: 110, girilir: girilir, valueParser: tamSayiParser });
                })),
                rowData: []
            });
        },

        listele: function (ctx) {
            return EkForm.al('api/ekform/fizikseldegerler/liste', { yil: ctx.secim.yil, kur: ctx.secim.kur },
                'Fiziksel değerler okunamadı.').done(function (v) {
                ctx.tablo.setGridOption('rowData', $.map(v.satirlar || [], function (s) {
                    var r = { kod: s.kod, aciklama: s.aciklama, degerGirilsinMi: !!s.degerGirilsinMi, girinti: s.girinti };
                    $.each(FARKLAR, function (i) { r['d' + i] = s.degerler ? s.degerler[i] : null; });
                    return r;
                }));
            });
        },

        // Eski btnKaydet: yalnızca kilitli olmayan (değer girilen) satırlar gönderilir.
        kaydet: function (ctx) {
            var satirlar = $.map($.grep(EkForm.satirlar(ctx.tablo), function (r) { return r.degerGirilsinMi; }), function (r) {
                return { kod: r.kod, aciklama: r.aciklama, degerler: FARKLAR.map(function (f, i) { return r['d' + i] == null ? null : r['d' + i]; }) };
            });
            return EkForm.kaydet(ctx, 'api/ekform/fizikseldegerler/kaydet', { yil: ctx.secim.yil, kur: ctx.secim.kur, satirlar: satirlar });
        },

        yazdir: function (ctx) {
            return EkForm.indir('api/ekform/fizikseldegerler/yazdir', { yil: ctx.secim.yil, kur: ctx.secim.kur });
        }
    });
})(jQuery);
