// Form 21 Özel güvenlik hizmetine dair bilgi formu (eski ButceForm/OzelGuvenlik.aspx).
// Eski formda tek ihale (no 1) vardı: açıklama, tertip tablosu (5 satır: KurKod, FonKod, FinKod, EkoKod), üç yılın (yıl-2, yıl-1, yıl)
// hizmet alanı / görevli sayısı / işin başlama ve bitiş tarihi kutuları ve 10 kalemli maliyet tablosu (harcama, yıl sonu tahmini, teklif).
// Aynı ihale ayrıntısı Form 22 (temizlik.js) sekmelerinde de kullanıldığı için çizen kod EkFormIhale.ciz olarak dışarı verilir
// (bu dosya temizlik.js'den önce yüklenir).
(function ($) {
    'use strict';

    var GUVENLIK_MALIYET = ['A-GÜVENLİK GÖREVLİSİ ÜCRETLERİ', 'B-YOL BEDELİ', 'C-YEMEK BEDELİ', 'D-GİYECEK BEDELİ',
        'GÜVENLİK EKİPMANLARININ MALİYETİ', 'FİRMA KARI', 'SÖZLEŞME GİDERİ', 'KDV DAHİL TOPLAM', 'BÜTÇEDEN KARŞILANAN TUTAR',
        'DİĞER KAYNAKLARDAN KARŞILANAN TUTAR'];
    var TERTIP_SATIR = 5;
    var YILLIK = [
        { alan: 'hizmetAlani', ad: 'Hizmet Alanı', sayi: true },
        { alan: 'gorevliSayisi', ad: null, sayi: true },
        { alan: 'baslama', ad: 'İşin Başlama Tarihi' },
        { alan: 'bitis', ad: 'İşin Bitiş Tarihi' }
    ];
    var sayac = 0;

    // İhale ayrıntısını $kap içine çizer. o: { yil, maliyetAdlari, gorevliEtiket, degisti() }.
    // Döndürür: { doldur(detay | null), oku() -> sunucunun beklediği detay }.
    function ciz(ctx, $kap, o) {
        var no = ++sayac;
        function degisti() { EkForm.degisti(ctx); if (o.degisti) o.degisti(); }

        // İhale tanımı
        var $b1 = $('<section class="ekform-bolum mavi"><h3>İhale bilgileri</h3><div class="bolum-ic"></div></section>').appendTo($kap);
        var $al = $('<div class="ekform-alanlar"></div>').appendTo($b1.find('.bolum-ic'));
        var $g = $('<div class="form-group" style="grid-column: 1 / -1; max-width: 640px;"></div>').appendTo($al);
        $('<label></label>').attr('for', 'ihAciklama' + no).text('Açıklama').appendTo($g);
        var $aciklama = $('<input type="text" class="form-control" maxlength="4000">').attr('id', 'ihAciklama' + no).appendTo($g);
        $aciklama.on('input', degisti);

        // Ödeneğin karşılanacağı tertipler
        var $b2 = $('<section class="ekform-bolum"><h3>Ödeneğin karşılanacağı tertipler</h3><div class="bolum-ic"></div></section>').appendTo($kap);
        var $tt = $('<div class="ekform-grid" style="height: 200px; max-width: 560px;" aria-label="Tertipler"></div>').appendTo($b2.find('.bolum-ic'));
        var tertipTablo = EkForm.grid(ctx, $tt[0], {
            columnDefs: [
                EkForm.metinSutun('kurKod', 'KurKod', { width: 130 }),
                EkForm.metinSutun('fonKod', 'FonKod', { width: 130 }),
                EkForm.metinSutun('finKod', 'FinKod', { width: 130 }),
                EkForm.metinSutun('ekoKod', 'EkoKod', { width: 130 })
            ],
            rowData: [],
            onCellValueChanged: function () { if (o.degisti) o.degisti(); }
        });

        // Yıllara göre bilgiler (eski lblYil1..3: yıl-2, yıl-1, yıl)
        var $b3 = $('<section class="ekform-bolum yesil"><h3>Yıllara göre bilgiler</h3><div class="bolum-ic"></div></section>').appendTo($kap);
        var $tablo = $('<table class="table table-condensed" style="width: auto; margin: 0;"></table>').appendTo($b3.find('.bolum-ic'));
        var $bas = $('<tr><th scope="col"></th></tr>').appendTo($('<thead></thead>').appendTo($tablo));
        for (var y = 2; y >= 0; y--) $('<th scope="col" class="text-center"></th>').text(o.yil - y).appendTo($bas);
        var $govde = $('<tbody></tbody>').appendTo($tablo);
        var kutular = {};
        $.each(YILLIK, function (i, s) {
            var ad = s.ad || o.gorevliEtiket;
            var $tr = $('<tr></tr>').appendTo($govde);
            $('<th scope="row" style="vertical-align: middle; padding-right: 14px;"></th>').text(ad).appendTo($tr);
            kutular[s.alan] = [];
            for (var j = 0; j < 3; j++) {
                var $i = $('<input type="text" class="form-control input-sm" style="width: 130px;">')
                    .toggleClass('text-right', !!s.sayi).attr('aria-label', ad + ' ' + (o.yil - 2 + j)).on('input', degisti);
                $('<td></td>').append($i).appendTo($tr);
                kutular[s.alan].push($i);
            }
        });

        // Maliyet unsurları
        var $b4 = $('<section class="ekform-bolum mor"><h3>Maliyet unsurları</h3><div class="bolum-ic"></div></section>').appendTo($kap);
        var $mt = $('<div class="ekform-grid" style="height: 340px; max-width: 780px;" aria-label="Maliyet unsurları"></div>').appendTo($b4.find('.bolum-ic'));
        var maliyetTablo = EkForm.grid(ctx, $mt[0], {
            columnDefs: [
                EkForm.metinSutun('ad', '', { width: 300, girilir: false, pinned: 'left' }),
                EkForm.sayiSutun('harcama', 'HARCAMA', { width: 150 }),
                EkForm.sayiSutun('tahmin', 'YIL SONU HARCAMA TAHMİNİ', { width: 190, headerTooltip: 'YIL SONU HARCAMA TAHMİNİ' }),
                EkForm.sayiSutun('teklif', 'TEKLİF', { width: 150 })
            ],
            rowData: [],
            onCellValueChanged: function () { if (o.degisti) o.degisti(); }
        });

        function doldur(d) {
            d = d || {};
            $aciklama.val(d.aciklama || '');
            $.each(YILLIK, function (i, s) {
                for (var j = 0; j < 3; j++) kutular[s.alan][j].val(d[s.alan] && d[s.alan][j] != null ? d[s.alan][j] : '');
            });
            var tl = $.map(d.tertipler || [], function (t) { return { kurKod: t.kurKod || '', fonKod: t.fonKod || '', finKod: t.finKod || '', ekoKod: t.ekoKod || '' }; });
            while (tl.length < TERTIP_SATIR) tl.push({ kurKod: '', fonKod: '', finKod: '', ekoKod: '' });
            tertipTablo.setGridOption('rowData', tl);
            maliyetTablo.setGridOption('rowData', $.map(o.maliyetAdlari, function (ad, i) {
                var m = (d.maliyetler || [])[i] || {};
                return { ad: ad, harcama: m.harcama == null ? null : m.harcama, tahmin: m.tahmin == null ? null : m.tahmin, teklif: m.teklif == null ? null : m.teklif };
            }));
        }

        function oku() {
            var d = { aciklama: $aciklama.val() };
            $.each(YILLIK, function (i, s) { d[s.alan] = $.map(kutular[s.alan], function ($i) { return [$i.val()]; }); });
            d.tertipler = $.map(EkForm.satirlar(tertipTablo), function (r) {
                return { kurKod: r.kurKod || '', fonKod: r.fonKod || '', finKod: r.finKod || '', ekoKod: r.ekoKod || '' };
            });
            d.maliyetler = $.map(EkForm.satirlar(maliyetTablo), function (r) {
                return { harcama: r.harcama == null || r.harcama === '' ? null : r.harcama,
                    tahmin: r.tahmin == null || r.tahmin === '' ? null : r.tahmin,
                    teklif: r.teklif == null || r.teklif === '' ? null : r.teklif };
            });
            return d;
        }

        doldur(null);
        return { doldur: doldur, oku: oku, tablolar: [tertipTablo, maliyetTablo] };
    }

    window.EkFormIhale = { ciz: ciz };

    EkForm.tanimla('ozelGuvenlik', {
        ac: function (ctx) {
            ctx.ihale = ciz(ctx, ctx.$govde, {
                yil: ctx.secim.yil, maliyetAdlari: GUVENLIK_MALIYET, gorevliEtiket: 'Güvenlik Görevlisi Sayısı'
            });
        },

        listele: function (ctx) {
            return EkForm.al('api/ekform/ozelguvenlik/liste', { yil: ctx.secim.yil, kur: ctx.secim.kur },
                'Özel güvenlik bilgileri okunamadı.').done(function (v) {
                ctx.ihale.doldur(v.detay);
            });
        },

        kaydet: function (ctx) {
            return EkForm.kaydet(ctx, 'api/ekform/ozelguvenlik/kaydet', {
                yil: ctx.secim.yil, kur: ctx.secim.kur, detay: ctx.ihale.oku()
            });
        },

        yazdir: function (ctx) {
            return EkForm.indir('api/ekform/ozelguvenlik/yazdir', { yil: ctx.secim.yil, kur: ctx.secim.kur });
        }
    });
})(jQuery);
