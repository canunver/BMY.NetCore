// Form 19.1 - Kullanılan taşıtlara ilişkin bilgi formu (eski ButceForm/KullanilanTasitlar.aspx).
// Merkez / Taşra / Toplam seçilip Listele'ye basılır; taşıt cetveli, kaynak ve yaş açılır listeden seçilir.
// Eski sayfada olduğu gibi Toplam seçiliyken boş satır açılamaz ve kaydedilemez; seçim değiştirilip listelenmeden kaydedilemez.
(function ($) {
    'use strict';

    var SECENEKLER = [{ kod: 1, ad: 'Merkez' }, { kod: 2, ad: 'Taşra' }, { kod: 3, ad: 'Toplam' }];

    function bosSatir() { return { siraNo: '', kaynakKod: null, yasKod: null, adet: null }; }

    function secili(ctx) { return parseInt(ctx.$merkezTasra.val(), 10) || 1; }

    // Toplam listelenmişse tablo yalnızca okunur (eski sayfada Toplam'da Kaydet kapalıydı; girilen değer saklanamazdı).
    function girilir(ctx) { return function () { return ctx.listelenen !== 3; }; }

    function dugmeAyarla(ctx) { ctx.$satirAc.prop('disabled', secili(ctx) === 3 || ctx.listelenen === 3); }

    EkForm.tanimla('kullanilanTasit', {
        ac: function (ctx) {
            ctx.sozluk = [];
            ctx.kaynaklar = [];
            ctx.yaslar = [];
            ctx.listelenen = 0;

            $('<p class="form-aciklama"></p>')
                .text('Seçim değiştirildiğinde Listele\'ye basınız; kayıt listelenen Merkez / Taşra için yapılır.').appendTo(ctx.$govde);
            var $ust = $('<div class="ekform-alanlar"></div>').appendTo(ctx.$govde);
            var $g = $('<div class="form-group"></div>').appendTo($ust);
            $('<label for="ekfKullanilanMerkezTasra">Merkez / Taşra</label>').appendTo($g);
            ctx.$merkezTasra = $('<select id="ekfKullanilanMerkezTasra" class="form-control"></select>').appendTo($g);
            $.each(SECENEKLER, function (i, s) { $('<option></option>').val(s.kod).text(s.ad).appendTo(ctx.$merkezTasra); });
            var $d = $('<div class="form-group"></div>').appendTo($ust);
            $('<label>&nbsp;</label>').appendTo($d);
            ctx.$satirAc = $('<button type="button" class="btn btn-default btn-sm"></button>')
                .append('<span class="glyphicon glyphicon-plus" aria-hidden="true"></span> Boş Satır Aç')
                .on('click', function () {
                    var r = ctx.tablo.applyTransaction({ add: [bosSatir()] });
                    EkForm.degisti(ctx);
                    if (r && r.add && r.add[0]) ctx.tablo.ensureNodeVisible(r.add[0]);
                })
                .appendTo($d);
            ctx.$merkezTasra.on('change', function () { dugmeAyarla(ctx); });
            $ust.css('margin-bottom', '10px');

            var $t = $('<div class="ekform-grid" aria-label="Kullanılan taşıtlar"></div>').appendTo(ctx.$govde);
            var g = girilir(ctx);
            ctx.tablo = EkForm.grid(ctx, $t[0], {
                columnDefs: [
                    EkForm.secimSutun('siraNo', 'T Cetveli Sıra No', function () { return ctx.sozluk; }, { width: 300, girilir: g }),
                    EkForm.secimSutun('kaynakKod', 'Kaynak', function () { return ctx.kaynaklar; }, { width: 220, girilir: g }),
                    EkForm.secimSutun('yasKod', 'Yaş', function () { return ctx.yaslar; }, { width: 150, girilir: g }),
                    EkForm.sayiSutun('adet', 'Mevcut Taşıt Sayısı', { width: 150, girilir: g })
                ],
                rowData: []
            });
            dugmeAyarla(ctx);
        },

        listele: function (ctx) {
            var mt = secili(ctx);
            return EkForm.al('api/ekform/kullanilantasit/liste', { yil: ctx.secim.yil, kur: ctx.secim.kur, merkezTasra: mt },
                'Kullanılan taşıtlar okunamadı.').done(function (v) {
                ctx.sozluk = v.sozluk || [];
                ctx.kaynaklar = v.kaynaklar || [];
                ctx.yaslar = v.yaslar || [];
                ctx.listelenen = v.listelenen || mt;
                ctx.tablo.setGridOption('rowData', v.satirlar || []);
                dugmeAyarla(ctx);
            }).fail(function () {
                // Eski sayfa okuma hatasında tabloyu boşaltıyordu.
                ctx.listelenen = 0;
                ctx.tablo.setGridOption('rowData', []);
                dugmeAyarla(ctx);
            });
        },

        kaydet: function (ctx) {
            var mt = secili(ctx);
            if (mt === 3) { Sgb.bildirim('Lütfen kaydetmek için merkez veya taşra seçiniz', 'warning'); return null; }
            if (mt !== ctx.listelenen) { Sgb.bildirim('Lütfen listeleme yaptıktan sonra kaydediniz!', 'warning'); return null; }
            return EkForm.onay('Kaydetmek İstiyor musunuz?', 'Kaydet').then(function () {
                var satirlar = $.map(EkForm.satirlar(ctx.tablo), function (r) {
                    return {
                        siraNo: r.siraNo || '',
                        kaynakKod: r.kaynakKod === '' || r.kaynakKod == null ? null : parseInt(r.kaynakKod, 10),
                        yasKod: r.yasKod === '' || r.yasKod == null ? null : parseInt(r.yasKod, 10),
                        adet: r.adet == null || r.adet === '' ? null : r.adet
                    };
                });
                return EkForm.kaydet(ctx, 'api/ekform/kullanilantasit/kaydet', {
                    yil: ctx.secim.yil, kur: ctx.secim.kur, merkezTasra: mt, listelenen: ctx.listelenen, satirlar: satirlar
                });
            });
        },

        yazdir: function (ctx) {
            return EkForm.indir('api/ekform/kullanilantasit/yazdir', { yil: ctx.secim.yil, kur: ctx.secim.kur });
        }
    });
})(jQuery);
