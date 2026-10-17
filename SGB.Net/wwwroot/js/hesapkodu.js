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
//       secilince: function (h) { },   // mercekten seçilince: { kod, ad, detay }
//       degisince: function (h) { }    // kod değişip okununca (seçim dahil): { kod, ad, detay, gecerli }
//   });
//   hk.deger() -> { kod, ad, detay, gecerli }   hk.ata(kod, ad)   hk.temizle()   hk.gecerli()
//
// Kullanım (işaretleme): <input type="text" class="form-control kisa" data-hesap-kodu data-yil="#txtYil" data-son-seviye data-ad
//   data-ad-hedef="#txtAd" data-denetle="false" />  -> sayfa açılınca kendiliğinden kurulur; nesnesi $(kutu).data('hesapKodu').
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
            Sgb.al(o.uc + 'ad', { yil: yilBul(o.yil), kod: kod }).done(function (v) {
                if (no !== istekNo) return;
                if (v.hata) { isaretle(false, v.hata); return; }
                if (v.kod && v.kod !== kod) { $in.val(v.kod); okunan = v.kod; }
                durum = { kod: v.kod || kod, ad: v.ad || '', detay: !!v.detay, gecerli: true };
                if (!v.ad) isaretle(false, 'Hesap planında yok');
                else if (o.sonSeviye && !v.detay) { adYaz(v.ad); isaretle(false, 'Son seviye değil: ' + v.ad); }
                else adYaz(v.ad);
                if (!sessiz) bildir();
            }).fail(function (xhr) {
                if (no !== istekNo || (xhr && xhr.status === 401)) return;
                isaretle(false, 'Hesap kodu okunamadı');
            });
        }

        $in.on('change', function () { oku(false); });
        $in.on('sgb:yazildi', function () { oku(true); });

        $sec.on('click', function () {
            if ($in.prop('readonly') || $in.prop('disabled')) return;
            Sgb.hesapPlaniSec({
                yil: yilBul(o.yil),
                sonKirilim: !!o.sonSeviye,
                uc: o.uc,
                secilince: function (h) {
                    if (o.sonSeviye && !h.detay) { Sgb.bildirim('Son seviye bir hesap seçiniz.', 'warning'); return; }
                    okunan = h.kod;
                    $in.val(h.kod);
                    durum = { kod: h.kod, ad: h.hesapAd || '', detay: !!h.detay, gecerli: true };
                    isaretle(true);
                    adYaz(h.hesapAd);
                    $in.trigger('change');   // sayfanın kendi dinleyicileri için; aynı kod yeniden okunmaz
                    if (o.secilince) o.secilince({ kod: h.kod, ad: h.hesapAd || '', detay: !!h.detay });
                    bildir();
                }
            });
        });

        var nesne = {
            deger: function () { return $.extend({}, durum); },
            gecerli: function () { return durum.gecerli; },
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
                uc: $i.attr('data-uc') || undefined
            });
        });
    };

    Sgb.hesapKodu = hesapKodu;
    $(function () { hesapKodu.hepsi(); });
})(window.Sgb, jQuery);
