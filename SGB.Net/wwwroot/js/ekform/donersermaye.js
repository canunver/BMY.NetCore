// Form 23: döner sermaye gelir-gider cetveli (eski ButceForm/DonerSermaye.aspx; döner sermaye no 1..6 ve adı) ve
// Form 24: fon gelir-gider cetveli (eski ButceForm/FonGelirGider.aspx; döner sermaye no 0, fon adı). p.fon: true ise Form 24.
// Eski tablo FarPoint'ti: kod ve açıklama sütunları ile değer girilemeyen (üst) satırlar kilitliydi; fon formunda kod ve açıklama
// kodun uzunluğuna göre içeri kaydırılıyordu. Döner sermaye no değişince liste yeniden okunur (eski ddlDSNo_SelectedIndexChanged).
(function ($) {
    'use strict';

    var DS_NOLARI = [1, 2, 3, 4, 5, 6];

    // Eski fon formu: kodun noktasız uzunluğu x 6 piksel girinti.
    function girinti(p) {
        var kod = p.data && p.data.kod ? String(p.data.kod).replace(/\./g, '') : '';
        return kod.length ? { paddingLeft: (11 + kod.length * 6) + 'px' } : null;
    }

    function no(ctx) { return ctx.p.fon ? 0 : (parseInt(ctx.$no.val(), 10) || 1); }

    var modul = {
        ac: function (ctx) {
            var yil = ctx.secim.yil;
            var fon = !!ctx.p.fon;
            ctx.yil = yil;

            // Döner sermaye / fon bilgisi
            var $b = $('<section class="ekform-bolum mavi"></section>').appendTo(ctx.$govde);
            $('<h3></h3>').text(fon ? 'Fon bilgisi' : 'Döner sermaye bilgisi').appendTo($b);
            var $a = $('<div class="ekform-alanlar"></div>').appendTo($('<div class="bolum-ic"></div>').appendTo($b));
            if (!fon) {
                var $g1 = $('<div class="form-group"></div>').appendTo($a);
                $('<label for="ekDSNo">Döner Sermaye No</label>').appendTo($g1);
                ctx.$no = $('<select id="ekDSNo" class="form-control"></select>').appendTo($g1);
                $.each(DS_NOLARI, function (i, n) { $('<option></option>').val(String(n)).text(String(n)).appendTo(ctx.$no); });
                ctx.onceNo = '1';
                ctx.$no.on('change', function () {
                    var yeni = ctx.$no.val();
                    var devam = ctx.degisti ? EkForm.onay('Kaydedilmemiş değişiklikler var. Kaydetmeden devam edilsin mi?', 'Devam et')
                        : $.Deferred().resolve().promise();
                    devam.done(function () {
                        ctx.onceNo = yeni;
                        modul.listele(ctx).done(function () { ctx.degisti = false; });
                    }).fail(function () { ctx.$no.val(ctx.onceNo); });
                });
            }
            var $g2 = $('<div class="form-group"></div>').css('grid-column', 'span 2').appendTo($a);
            $('<label for="ekDSAd"></label>').text(fon ? 'Fon Adı' : 'Döner Sermaye Adı').appendTo($g2);
            ctx.$ad = $('<input id="ekDSAd" type="text" class="form-control" maxlength="255" required>').appendTo($g2)
                .on('input', function () { EkForm.degisti(ctx); });

            $('<p class="form-aciklama"></p>').text(fon ? 'Fon gelir-gider cetveli' : 'Döner sermaye gelir-gider cetveli').appendTo(ctx.$govde);
            var $t = $('<div class="ekform-grid" aria-label="Gelir-gider cetveli"></div>').appendTo(ctx.$govde);

            var girilir = function (p) { return !!(p.data && p.data.girilir); };
            var kodO = { girilir: false, width: fon ? 80 : 60, pinned: 'left' };
            var acikO = { girilir: false, width: fon ? 350 : 300, pinned: 'left' };
            if (fon) { kodO.cellStyle = girinti; acikO.cellStyle = girinti; }

            ctx.tablo = EkForm.grid(ctx, $t[0], {
                columnDefs: [
                    { headerName: 'Açıklamalar', headerClass: 'orta', children: [
                        EkForm.metinSutun('kod', 'Kod', kodO),
                        EkForm.metinSutun('aciklama', 'Açıklama', acikO)
                    ] },
                    { headerName: String(yil - 2), headerClass: 'orta', children: [
                        EkForm.sayiSutun('d0', 'Gerçekleşme', { girilir: girilir, width: 120 })
                    ] },
                    { headerName: String(yil - 1), headerClass: 'orta', children: [
                        EkForm.sayiSutun('d1', 'Ödenek', { girilir: girilir, width: 120 }),
                        // Eski başlık: "Harcama (30.06." + bütçe yılı + ")".
                        EkForm.sayiSutun('d2', 'Harcama (30.06.' + yil + ')', { girilir: girilir, width: 120 })
                    ] },
                    { headerName: 'Teklif', headerClass: 'orta', children: [
                        EkForm.sayiSutun('d3', String(yil), { girilir: girilir, width: 120 }),
                        EkForm.sayiSutun('d4', String(yil + 1), { girilir: girilir, width: 120 }),
                        EkForm.sayiSutun('d5', String(yil + 2), { girilir: girilir, width: 120 })
                    ] }
                ],
                headerHeight: 40,
                rowData: []
            });
        },

        listele: function (ctx) {
            return EkForm.al('api/ekform/donersermaye/liste', { yil: ctx.secim.yil, kur: ctx.secim.kur, no: no(ctx) },
                'Döner sermaye bilgileri okunamadı.').done(function (v) {
                ctx.$ad.val(v.ad || '');
                var liste = $.map(v.satirlar || [], function (s) {
                    var r = { kod: s.kod, aciklama: s.aciklama, girilir: s.girilir };
                    for (var i = 0; i < 6; i++) r['d' + i] = s.degerler ? s.degerler[i] : null;
                    return r;
                });
                ctx.tablo.setGridOption('rowData', liste);
            });
        },

        // Eski btnKaydet_Click: yalnız kilitli olmayan satırlar gönderilir.
        kaydet: function (ctx) {
            var satirlar = $.map($.grep(EkForm.satirlar(ctx.tablo), function (r) { return r.girilir; }), function (r) {
                var d = [];
                for (var i = 0; i < 6; i++) d.push(r['d' + i] == null ? null : r['d' + i]);
                return { kod: r.kod, aciklama: r.aciklama, degerler: d };
            });
            return EkForm.kaydet(ctx, 'api/ekform/donersermaye/kaydet', {
                yil: ctx.secim.yil, kur: ctx.secim.kur, no: no(ctx), ad: ctx.$ad.val(), satirlar: satirlar
            });
        },

        yazdir: function (ctx) {
            return EkForm.indir('api/ekform/donersermaye/yazdir', { yil: ctx.secim.yil, kur: ctx.secim.kur, no: no(ctx) });
        }
    };

    EkForm.tanimla('donerSermaye', modul);
})(jQuery);
