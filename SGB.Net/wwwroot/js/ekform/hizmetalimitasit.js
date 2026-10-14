// Form 19.2 / 19.3 - Hizmet alımı suretiyle kullanılacak (p.kullanilacak = 1) / kullanılan (p.kullanilacak = 0) taşıtlar
// (eski ButceForm/HizmetAlimiTasitlariDetay.aspx?Kullanilacak=..). Eski tablo FarPoint'ti: yılın taşıt sözlüğünün her satırı için
// Bütçe ve Döner Sermaye + Diğer altında şoförlü / şoförsüz (yakıt gider hariç) adet, süre ve aylık kira bedeli girilir.
// "Detaydan Al" (eski EkFormAna'daki tuş -> btnDetaydanAl "Sözleşmelerden Al") kiralık araç sözleşmelerinden toplanan bilgileri
// tabloya alır; saklamak için Kaydet'e basılır.
(function ($) {
    'use strict';

    var GRUPLAR = ['BÜTÇE', 'DÖNER SERMAYE+DİĞER'];
    var ALT = ['Şoförlü Yakıt Gider Hariç', 'Şoförsüz Yakıt Gider Hariç'];
    var SUTUN = [
        { ad: 'Adet' },
        { ad: '1 Aracın ort. kullanım süresi (ay)' },
        { ad: '1 Aracın aylık ort. kira bedeli', ondalik: true, width: 130 }
    ];

    function satirCevir(s) {
        var r = { siraNo: s.siraNo, tasit: s.tasit };
        for (var i = 0; i < 12; i++) r['d' + i] = s.degerler ? s.degerler[i] : null;
        return r;
    }

    function doldur(ctx, v) { ctx.tablo.setGridOption('rowData', $.map(v.satirlar || [], satirCevir)); }

    function parametreler(ctx) { return { yil: ctx.secim.yil, kur: ctx.secim.kur, kullanilacak: ctx.p.kullanilacak }; }

    EkForm.tanimla('hizmetAlimiTasit', {
        ac: function (ctx) {
            $('<p class="form-aciklama"></p>').text(ctx.p.kullanilacak === 1
                ? 'Hizmet Alımı Suretiyle Kullanılacak Taşıtlara İlişkin Bilgi Formu'
                : 'Hizmet Alımı Suretiyle Kullanılan Taşıtlara İlişkin Bilgi Formu').appendTo(ctx.$govde);
            var $t = $('<div class="ekform-grid" aria-label="Hizmet alımı taşıtları"></div>').appendTo(ctx.$govde);

            var gruplar = $.map(GRUPLAR, function (grup, gi) {
                return {
                    headerName: grup, headerClass: 'orta',
                    children: $.map(ALT, function (alt, ai) {
                        return {
                            headerName: alt, headerClass: 'orta',
                            children: $.map(SUTUN, function (s, si) {
                                var no = gi * 6 + ai * 3 + si;
                                return EkForm.sayiSutun('d' + no, s.ad, { width: s.width || 105, ondalik: !!s.ondalik, headerTooltip: alt + ' - ' + s.ad });
                            })
                        };
                    })
                };
            });
            ctx.tablo = EkForm.grid(ctx, $t[0], {
                columnDefs: [
                    { headerName: '', children: [{ headerName: '', children: [
                        EkForm.metinSutun('tasit', '(T) Cetveli Sıra no./Taşıtın cinsi/Diferansiyel', { width: 260, pinned: 'left', girilir: false })
                    ] }] }
                ].concat(gruplar),
                rowData: []
            });
        },

        dugmeler: function (ctx) {
            return [{
                ad: 'Detaydan Al', ikon: 'import', ipucu: 'Sözleşmelerden Al: kiralık araç sözleşmelerindeki bilgileri tabloya alır',
                tikla: function () {
                    return EkForm.al('api/ekform/hizmetalimitasit/sozlesmeler', parametreler(ctx), 'Sözleşme bilgileri okunamadı.').done(function (v) {
                        doldur(ctx, v);
                        EkForm.degisti(ctx);
                        Sgb.bildirim('Sözleşme bilgileri tabloya alındı; saklamak için Kaydet\'e basınız.', 'info');
                    });
                }
            }];
        },

        listele: function (ctx) {
            return EkForm.al('api/ekform/hizmetalimitasit/liste', parametreler(ctx), 'Hizmet alımı taşıt bilgileri okunamadı.')
                .done(function (v) { doldur(ctx, v); });
        },

        kaydet: function (ctx) {
            var satirlar = $.map(EkForm.satirlar(ctx.tablo), function (r) {
                var d = [];
                for (var i = 0; i < 12; i++) d.push(r['d' + i] == null || r['d' + i] === '' ? null : r['d' + i]);
                return { siraNo: r.siraNo, degerler: d };
            });
            return EkForm.kaydet(ctx, 'api/ekform/hizmetalimitasit/kaydet', $.extend(parametreler(ctx), { satirlar: satirlar }));
        },

        yazdir: function (ctx) {
            return EkForm.indir('api/ekform/hizmetalimitasit/yazdir', parametreler(ctx));
        }
    });
})(jQuery);
