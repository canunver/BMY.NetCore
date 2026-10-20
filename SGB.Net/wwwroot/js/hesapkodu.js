// Sgb.hesapKodu: hesap kodu giriş bileşeni. Kutunun hemen yanına hesap planı merceği (Sgb.hesapPlaniSec, js/makbuzsecim.js),
// isteğe bağlı olarak onun yanına hesabın adı gelir. Elle yazılan kod sunucuda okunur: yılın kod yapısına göre noktalanır, adı
// yazılır; hesap planında yoksa ya da (sonSeviye istenmişse) son seviye değilse kutu kırmızı çerçeveyle işaretlenir.
// Uçlar: <uc>agac, <uc>ara, <uc>ad (varsayılan /api/muhtanim/hesap/*; /api/makbuz/hesap/* de aynı uçlara sahip). Görünüm: css/hesapkodu.css.
//
// Kullanım (JS):
//   var hk = Sgb.hesapKodu('#txtHesap', {
//       yil:       '#txtYil',          // sayı, seçici (kutunun değeri) ya da işlev; verilmezse bu yıl
//       sonSeviye: true,               // yalnız son seviye (detay) hesap kabul edilir; mercek de yalnız onları arar
//       ad:        true,               // hesabın adı merceğin yanında gösterilir
//       adHedef:   '#txtHesapAd',      // ad ayrıca (ya da yalnızca) bu kutuya yazılır
//       denetle:   true,               // elle yazılan kod sunucuda okunsun (false: yalnız mercek; ör. yeni hesap tanımlanan kutu)
//       uc:        'api/muhtanim/hesap/',
//       onEk:      '830',              // yalnız bu hesabın altındakiler kabul edilir; mercekteki ağaç da buradan başlar
//       seviye:    3,                  // yalnız bu düzeydeki hesaplar (830.01.01 -> 3) kabul edilir
//       secilince: function (h) { },   // mercekten seçilince: { kod, ad, detay }
//       degisince: function (h) { }    // kod değişip okununca (seçim dahil): { kod, ad, detay, gecerli }
//   });
//   hk.deger() -> { kod, ad, detay, gecerli }   hk.ata(kod, ad)   hk.temizle()   hk.gecerli()   hk.hazir().done(function (h) { })
//
// Kullanım (işaretleme): <input type="text" class="form-control kisa" data-hesap-kodu data-yil="#txtYil" data-son-seviye data-ad
//   data-ad-hedef="#txtAd" data-denetle="false" data-on-ek="830" data-seviye="3" />  -> sayfa açılınca kendiliğinden kurulur; nesnesi $(kutu).data('hesapKodu').
// Kutunun değeri koddan yazılırsa (Sgb.mt.form gibi) kutuda 'sgb:yazildi' olayı çıkarılmalı; bileşen adı yeniden okur.
(function (Sgb, $) {
    'use strict';

    function yilBul(y) {
        var n;
        if (typeof y === 'function') n = parseInt(y(), 10);
        else if (typeof y === 'number') n = y;
        else if (y) n = parseInt($(y).val(), 10);
        return n > 2000 ? n : new Date().getFullYear();
    }

    function hesapKodu(element, ozellikler) {
        var $in = $(element).first();
        if (!$in.length) return null;
        if ($in.data('hesapKodu')) return $in.data('hesapKodu');

        var o = $.extend({ ad: false, denetle: true, sonSeviye: false, uc: 'api/muhtanim/hesap/' }, ozellikler || {});
        var durum = { kod: '', ad: '', detay: false, gecerli: true };
        var okunan = null;   // son okunan kod (aynı kod yeniden okunmasın)
        var istekNo = 0;
        var bekleyen = null;  // süren ad okuması (hazir() bunu bekletir)

        var $kap = $('<span class="hk-bilesen"></span>');
        $in.addClass('hk-kod').attr('autocomplete', 'off').after($kap);
        var $sec = $('<button type="button" class="btn btn-default btn-sm hk-sec"></button>')
            .attr('title', o.sonSeviye ? 'Hesap planından seç (son seviye)' : 'Hesap planından seç')
            .append('<span class="glyphicon glyphicon-search" aria-hidden="true"></span><span class="sr-only">Hesap planından seç</span>');
        var $ad = o.ad ? $('<span class="hk-ad"></span>') : $();
        $kap.append($in, $sec, $ad);

        function adYaz(ad) {
            $ad.text(ad || '').attr('title', ad || '');
            if (o.adHedef) $(o.adHedef).val(ad || '');
        }

        function isaretle(gecerli, neden) {
            durum.gecerli = gecerli;
            $in.toggleClass('hk-gecersiz', !gecerli).attr('aria-invalid', gecerli ? null : 'true').attr('title', gecerli ? null : neden);
            $ad.toggleClass('hk-gecersiz', !gecerli);
            if (!gecerli) $ad.text(neden).attr('title', neden);
        }

        // onEk / seviye kısıtı; uymuyorsa nedeni döner
        function kisitHata(kod) {
            var k = String(kod || '');
            if (o.onEk && k.indexOf(o.onEk + '.') !== 0) return o.onEk + ' ile başlayan bir hesap olmalı';
            if (o.seviye && k.split('.').length !== o.seviye) return o.seviye + '. düzeyde bir hesap olmalı' + (o.onEk ? ' (ör. ' + o.onEk + '.01.01)' : '');
            return '';
        }

        function bildir() {
            if (o.degisince) o.degisince($.extend({}, durum));
        }

        function oku(sessiz) {
            var kod = $.trim($in.val());
            if (kod === okunan) return;
            okunan = kod;
            durum = { kod: kod, ad: '', detay: false, gecerli: true };
            isaretle(true);
            adYaz('');
            if (!kod) { if (!sessiz) bildir(); return; }
            if (!o.denetle) { if (!sessiz) bildir(); return; }
            var no = ++istekNo;
            bekleyen = Sgb.al(o.uc + 'ad', { yil: yilBul(o.yil), kod: kod });
            bekleyen.done(function (v) {
                if (no !== istekNo) return;
                if (v.hata) { isaretle(false, v.hata); return; }
                if (v.kod && v.kod !== kod) { $in.val(v.kod); okunan = v.kod; }
                durum = { kod: v.kod || kod, ad: v.ad || '', detay: !!v.detay, gecerli: true };
                var kh = kisitHata(v.kod || kod);
                if (!v.ad) isaretle(false, 'Hesap planında yok');
                else if (kh) { adYaz(v.ad); isaretle(false, kh); }
                else if (o.sonSeviye && !v.detay) { adYaz(v.ad); isaretle(false, 'Son seviye değil: ' + v.ad); }
                else adYaz(v.ad);
                if (!sessiz) bildir();
            }).fail(function (xhr) {
                if (no !== istekNo || (xhr && xhr.status === 401)) return;
                isaretle(false, 'Hesap kodu okunamadı');
            });
        }

        // Mercekten ya da öneri listesinden seçilen hesap: { kod, hesapAd, detay }
        function secildi(h) {
            if (o.sonSeviye && !h.detay) { Sgb.bildirim('Son seviye bir hesap seçiniz.', 'warning'); return false; }
            var kh = kisitHata(h.kod);
            if (kh) { Sgb.bildirim('Seçilen hesap ' + kh + '.', 'warning'); return false; }
            okunan = h.kod;
            $in.val(h.kod);
            durum = { kod: h.kod, ad: h.hesapAd || '', detay: !!h.detay, gecerli: true };
            isaretle(true);
            adYaz(h.hesapAd);
            $in.trigger('change');   // sayfanın kendi dinleyicileri için; aynı kod yeniden okunmaz
            if (o.secilince) o.secilince({ kod: h.kod, ad: h.hesapAd || '', detay: !!h.detay });
            bildir();
            return true;
        }

        // Yazdıkça arama (eski ComboBox MinChars=2): kodla başlayan ya da adında yazılan geçen hesaplar kutunun altında listelenir;
        // ok tuşlarıyla gezilir, Enter / tıklama ile seçilir, Esc listeyi kapatır.
        var $liste = $('<div class="hk-oneri-liste" role="listbox" hidden></div>').appendTo(document.body);
        var oneri = { secenekler: [], isaret: -1, istek: 0, zaman: null, acik: false };

        function oneriKapat() {
            oneri.istek++;
            clearTimeout(oneri.zaman);
            $liste.prop('hidden', true).empty();
            oneri.secenekler = []; oneri.isaret = -1; oneri.acik = false;
            $in.attr('aria-expanded', 'false');
        }

        function oneriIsaretle(i) {
            oneri.isaret = i;
            var $s = $liste.children().removeClass('secili');
            if (i >= 0) $s.eq(i).addClass('secili')[0].scrollIntoView({ block: 'nearest' });
        }

        function oneriAra() {
            var metin = $.trim($in.val());
            if (metin.length < 2 || $in.prop('readonly') || $in.prop('disabled')) { oneriKapat(); return; }
            var no = ++oneri.istek;
            Sgb.al(o.uc + 'ara', { yil: yilBul(o.yil), metin: metin, sonKirilim: o.sonSeviye ? '1' : '0' }).done(function (v) {
                if (no !== oneri.istek || !$in.is(':focus')) return;
                var liste = $.grep((v && v.satirlar) || [], function (h) { return !kisitHata(h.kod); }).slice(0, 50);
                oneri.secenekler = liste;
                $liste.empty();
                if (!liste.length) { oneriKapat(); return; }
                $.each(liste, function (i, h) {
                    $('<div class="hk-oneri-satir" role="option"></div>').text(h.ad).attr('title', h.ad).appendTo($liste);
                });
                var r = $in[0].getBoundingClientRect();
                var alt = window.innerHeight - r.bottom;
                $liste.css({ left: r.left, minWidth: Math.max(r.width, 360), top: alt > 220 ? r.bottom : '', bottom: alt > 220 ? '' : window.innerHeight - r.top })
                    .prop('hidden', false);
                oneri.acik = true;
                $in.attr('aria-expanded', 'true');
                oneriIsaretle(-1);
            });
        }

        function oneriSec(i) {
            var h = oneri.secenekler[i];
            if (!h) return;
            oneriKapat();
            secildi(h);
        }

        $in.attr({ role: 'combobox', 'aria-autocomplete': 'list', 'aria-expanded': 'false' });
        $in.on('input', function () {
            clearTimeout(oneri.zaman);
            oneri.zaman = setTimeout(oneriAra, 300);
        });
        $in.on('keydown', function (e) {
            if (!oneri.acik) return;
            var n = oneri.secenekler.length;
            if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
                e.preventDefault();
                oneriIsaretle(e.key === 'ArrowDown' ? (oneri.isaret + 1) % n : (oneri.isaret - 1 + n) % n);
            } else if (e.key === 'Enter') {
                if (oneri.isaret >= 0) { e.preventDefault(); e.stopPropagation(); oneriSec(oneri.isaret); }
                else oneriKapat();
            } else if (e.key === 'Escape') {
                e.preventDefault(); e.stopPropagation(); oneriKapat();
            }
        });
        $in.on('blur', oneriKapat);
        $liste.on('mousedown', '.hk-oneri-satir', function (e) { e.preventDefault(); oneriSec($(this).index()); });
        // Sayfa kayınca liste kutudan kopmasın (listenin kendi kaydırması hariç)
        window.addEventListener('scroll', function (e) { if (oneri.acik && e.target !== $liste[0]) oneriKapat(); }, true);

        $in.on('change', function () { oku(false); });
        $in.on('sgb:yazildi', function () { oku(true); });

        $sec.on('click', function () {
            if ($in.prop('readonly') || $in.prop('disabled')) return;
            oneriKapat();
            Sgb.hesapPlaniSec({
                yil: yilBul(o.yil),
                sonKirilim: !!o.sonSeviye,
                uc: o.uc,
                kok: o.onEk || '',
                secilince: secildi
            });
        });

        var nesne = {
            deger: function () { return $.extend({}, durum); },
            gecerli: function () { return durum.gecerli; },
            // Kutudaki kodun okunmasını bekler (ör. kod yazılıp hemen Ekle'ye basılınca): önce değişikliği işler.
            hazir: function () {
                if ($.trim($in.val()) !== okunan) oku(false);
                var d = $.Deferred();
                (bekleyen || $.Deferred().resolve()).always(function () { d.resolve($.extend({}, durum)); });
                return d.promise();
            },
            ata: function (kod, ad) {
                $in.val(kod || '');
                if (ad === undefined) { oku(true); return; }
                okunan = $.trim(kod || '');
                durum = { kod: okunan, ad: ad || '', detay: false, gecerli: true };
                isaretle(true);
                adYaz(ad);
            },
            temizle: function () { nesne.ata('', ''); },
            yenile: function () { okunan = null; oku(true); }
        };
        $in.data('hesapKodu', nesne);
        if ($.trim($in.val())) oku(true);
        return nesne;
    }

    // İşaretlemeyle kurulum: [data-hesap-kodu] kutuları.
    hesapKodu.hepsi = function (kok) {
        $(kok || document).find('input[data-hesap-kodu]').each(function () {
            var $i = $(this);
            if ($i.data('hesapKodu')) return;
            var d = $i.attr('data-denetle');
            hesapKodu(this, {
                yil: $i.attr('data-yil') || undefined,
                sonSeviye: $i.is('[data-son-seviye]'),
                ad: $i.is('[data-ad]'),
                adHedef: $i.attr('data-ad-hedef') || undefined,
                denetle: d === undefined ? true : d !== 'false',
                uc: $i.attr('data-uc') || undefined,
                onEk: $i.attr('data-on-ek') || undefined,
                seviye: parseInt($i.attr('data-seviye'), 10) || undefined
            });
        });
    };

    Sgb.hesapKodu = hesapKodu;
    $(function () { hesapKodu.hepsi(); });
})(window.Sgb, jQuery);
