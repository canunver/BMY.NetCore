// Form 22 Temizlik hizmetine ilişkin bilgi formu (eski ButceForm/TemizlikHizmeti.aspx).
// Eski form birimin her ihalesi için bir sekme ("n. İhale") ve en alt düzey birimde (noktasız 8 hane) "Yeni İhale" sekmesi açıyor,
// her sekmede çerçeve içinde TemizlikHizmetiIhaleDetay.aspx'i yüklüyordu. Burada sekmeler aynı sayfadadır; her sekmenin içeriği
// özel güvenlik formunun ihale ayrıntısıyla aynıdır (EkFormIhale.ciz, ozelguvenlik.js).
// Eski Kaydet yalnızca açık sekmeyi saklıyordu; burada değişen sekmelerin hepsi (değişen yoksa açık sekme) saklanır, ardından
// sekmeler yeniden okunur (yeni ihale numarasını alır).
(function ($) {
    'use strict';

    var TEMIZLIK_MALIYET = ['A-TEMİZLİK GÖREVLİSİ ÜCRETLERİ', 'B-YOL BEDELİ', 'C-YEMEK BEDELİ', 'D-GİYECEK BEDELİ',
        'TEMİZLİK MALZEMELERİNİN MALİYETİ', 'FİRMA KARI', 'SÖZLEŞME GİDERİ', 'KDV DAHİL TOPLAM', 'BÜTÇEDEN KARŞILANAN TUTAR',
        'DİĞER KAYNAKLARDAN KARŞILANAN TUTAR'];

    function sekmeleriKaldir(ctx) {
        $.each(ctx.sekmeler || [], function (i, s) {
            $.each(s.ihale.tablolar, function (j, g) { try { g.destroy(); } catch (e) { /* yok */ } });
        });
        ctx.tablolar = $.grep(ctx.tablolar || [], function (g) { return !(g.isDestroyed && g.isDestroyed()); });
        ctx.sekmeler = [];
        ctx.$sekmeler.empty();
    }

    function sekmeSec(ctx, i) {
        ctx.aktifSekme = i;
        ctx.$sekmeler.find('.nav-tabs > li').each(function (j) {
            $(this).toggleClass('active', j === i).children('a').attr('aria-selected', j === i ? 'true' : 'false');
        });
        ctx.$sekmeler.find('.temizlik-sekme').each(function (j) { $(this).prop('hidden', j !== i); });
    }

    function basligiYaz(s) {
        s.$a.text(s.baslik + (s.degisti ? ' •' : ''));
    }

    function sekmeleriCiz(ctx, ihaleler, secilecek) {
        sekmeleriKaldir(ctx);
        if (!ihaleler.length) {
            $('<p class="text-muted bos-metin"></p>')
                .text('Bu birim için temizlik ihalesi yok. Yeni ihale girmek için en alt düzeydeki bir birimi seçiniz.')
                .appendTo(ctx.$sekmeler);
            return;
        }
        var $ul = $('<ul class="nav nav-tabs" role="tablist" style="margin-bottom: 10px;"></ul>').appendTo(ctx.$sekmeler);
        $.each(ihaleler, function (i, ih) {
            var s = { baslik: ih.baslik, kurKod: ih.kurKod, ihaleNo: ih.ihaleNo, degisti: false };
            var $li = $('<li role="presentation"></li>').appendTo($ul);
            s.$a = $('<a href="#" role="tab"></a>').attr('title', ih.kurKod + (ih.ihaleNo ? ' / ihale ' + ih.ihaleNo : '')).appendTo($li)
                .on('click', function (e) { e.preventDefault(); sekmeSec(ctx, i); });
            basligiYaz(s);
            var $p = $('<div class="temizlik-sekme" role="tabpanel"></div>').appendTo(ctx.$sekmeler);
            s.ihale = EkFormIhale.ciz(ctx, $p, {
                yil: ctx.secim.yil, maliyetAdlari: TEMIZLIK_MALIYET, gorevliEtiket: 'Temizlik Görevlisi Sayısı',
                degisti: function () { if (!s.degisti) { s.degisti = true; basligiYaz(s); } }
            });
            s.ihale.doldur(ih.detay);
            ctx.sekmeler.push(s);
        });
        var sec = 0;
        if (secilecek) {
            $.each(ctx.sekmeler, function (i, s) {
                if (s.kurKod === secilecek.kurKod && s.ihaleNo === secilecek.ihaleNo) sec = i;
            });
            if (!secilecek.ihaleNo && secilecek.sira < ctx.sekmeler.length) sec = secilecek.sira;   // yeni ihale saklandıysa aynı sırada
        }
        sekmeSec(ctx, sec);
    }

    function oku(ctx, secilecek) {
        return EkForm.al('api/ekform/temizlik/liste', { yil: ctx.secim.yil, kur: ctx.secim.kur },
            'Temizlik hizmeti bilgileri okunamadı.').done(function (v) {
            sekmeleriCiz(ctx, v.ihaleler || [], secilecek);
            ctx.degisti = false;
        });
    }

    EkForm.tanimla('temizlik', {
        ac: function (ctx) {
            ctx.sekmeler = [];
            ctx.$sekmeler = $('<div class="temizlik-sekmeler"></div>').appendTo(ctx.$govde);
        },

        listele: function (ctx) { return oku(ctx, null); },

        kaydet: function (ctx) {
            if (!ctx.sekmeler.length) { Sgb.bildirim('Saklanacak ihale yok.', 'warning'); return null; }
            var secili = ctx.sekmeler[ctx.aktifSekme] || ctx.sekmeler[0];
            var saklanacak = $.grep(ctx.sekmeler, function (s) { return s.degisti; });
            if (!saklanacak.length) saklanacak = [secili];
            var ihaleler = $.map(saklanacak, function (s) { return { kurKod: s.kurKod, ihaleNo: s.ihaleNo, detay: s.ihale.oku() }; });
            var secilecek = { kurKod: secili.kurKod, ihaleNo: secili.ihaleNo, sira: ctx.aktifSekme };
            return EkForm.kaydet(ctx, 'api/ekform/temizlik/kaydet', { yil: ctx.secim.yil, ihaleler: ihaleler })
                .then(function () { return oku(ctx, secilecek); });
        },

        yazdir: function (ctx) {
            return EkForm.indir('api/ekform/temizlik/yazdir', { yil: ctx.secim.yil, kur: ctx.secim.kur });
        },

        kapat: function (ctx) { sekmeleriKaldir(ctx); }
    });
})(jQuery);
