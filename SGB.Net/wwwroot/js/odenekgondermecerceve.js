// Ödenek Gönderme / Tenkis ana çerçevesi (ButceMuhasebe/OdenekGondermeTenkisFrame; İcmal çerçevesi OdenekGondermeIcmalFrame de kullanır): "Belge Düzenleme" ve "Belge Arama"
// sekmeleri. Belge Arama sekmesi ilk açılışta yüklenir. Listeden bir belge açılınca (OgBelgeAc) Belge Düzenleme sekmesine
// geçilir ve belge oradaki formda okunur (eski çerçevedeki sekme geçişi).
$(function () {
    'use strict';

    var $kok = $('.og-cerceve');
    if (!$kok.length) return;

    // Öğeyi sayfa alanının (.sayfa-icerik) kalan yüksekliğine yayar; sayfada dikey kaydırma çıkmaz, kaydırma öğenin içinde kalır.
    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var kap = el.closest('.sayfa-icerik') || document.body, cs = getComputedStyle(kap);
        var kaydirir = /(auto|scroll)/.test(cs.overflowY);
        var kr = kap.getBoundingClientRect(), kay = kaydirir ? kap.scrollTop : 0;
        var gorunen = kaydirir ? kap.clientHeight : window.innerHeight - kr.top;
        var ust = el.getBoundingClientRect().top - kr.top + kay;
        var son = 0;
        $(kap).children().each(function () {
            if (this.offsetParent === null && getComputedStyle(this).position !== 'fixed') return;
            son = Math.max(son, this.getBoundingClientRect().bottom - kr.top + kay + parseFloat(getComputedStyle(this).marginBottom || 0));
        });
        son += parseFloat(cs.paddingBottom || 0);
        var bosluk = Math.max(0, son - (ust + el.offsetHeight));
        el.style.height = Math.max(enAz, Math.floor(gorunen - ust - bosluk - 1)) + 'px';
    }

    function cerceveYay() {
        $kok.find('iframe.og-alt-cerceve').each(function () { yuksekligeYay(this, 300); });
    }

    function sekmeAc(ad) {
        $kok.find('.og-sekmeler a').each(function () {
            var secili = $(this).attr('data-sekme') === ad;
            $(this).attr('aria-selected', secili ? 'true' : 'false').parent().toggleClass('active', secili);
        });
        $kok.find('iframe.og-alt-cerceve').each(function () {
            var secili = $(this).attr('data-sekme') === ad;
            if (secili && !this.getAttribute('src') && this.getAttribute('data-src')) this.setAttribute('src', this.getAttribute('data-src'));
            $(this).prop('hidden', !secili);
        });
        cerceveYay();
    }

    $kok.on('click', '.og-sekmeler a', function (e) {
        e.preventDefault();
        sekmeAc($(this).attr('data-sekme'));
    });

    // Liste sayfası (aynı kökten) çağırır; bağımsız değişkenler formdaki OgBelgeYukle'ye olduğu gibi geçer
    // (Ödenek Gönderme: yil, belgeNo; İcmal: yil, icmalNo, belgeTur).
    window.OgBelgeAc = function () {
        sekmeAc('form');
        var form = document.getElementById('cerceveForm');
        var w = form && form.contentWindow;
        if (w && typeof w.OgBelgeYukle === 'function') w.OgBelgeYukle.apply(w, arguments);
    };

    $(window).on('resize', cerceveYay);
    cerceveYay();
});
