// Birim Bütçe Hazırlığı çerçevesi (ButceHazirlikv2/BirimButceHazirlikFormAna): sekme geçişi ve yükseklik odenekgondermecerceve.js'ten.
// Sekmelerdeki sayfalar (birimbutcehazirlik.js) gönderme / iade öncesinde öbür sekmenin kaydedilmemiş değişikliğini ve "teklif yoktur"
// seçimini BbhDiger ile sorar (eski DigerSayfaKirli, YeniYokBul, GonderTusAyarla). Öbür sekme hiç açılmadıysa null döner.
(function () {
    'use strict';

    window.BbhDiger = function (yeni) {
        var hedef = document.querySelector('iframe.og-alt-cerceve[data-yeni="' + (yeni ? '0' : '1') + '"]');
        if (!hedef || !hedef.getAttribute('src')) return null;
        try {
            var w = hedef.contentWindow;
            return w && w.BbhSayfa ? w.BbhSayfa : null;
        } catch (e) {
            return null;
        }
    };
})();
