// Saklı Raporlar (AlacakTakip/SakliRaporlar): her rapor grubu bir sekme; sekmede ATMAB modülünün o gruba ekli dosyaları
// (Duyuru/EkliDosyalarForm, menüsüz) çerçevede gösterilir (eski SakliRaporlar.aspx, Ext.NET TabPanel + IFrame AutoLoad).
// Çerçeve, sekme ilk gösterildiğinde yüklenir (data-src -> src). Sekmeler sayfanın sunucuda yazdığı listedir (SakliRaporGruplari).
$(function () {
    'use strict';

    var $kok = $('.sakli-raporlar');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var $sekmeler = $kok.find('.sakli-sekmeler a[role="tab"]');

    function yukle($panel) {
        var $f = $panel.find('iframe');
        if ($f.length && !$f.attr('src')) $f.attr('src', $f.attr('data-src'));
    }

    function sec($a, odakla) {
        $sekmeler.each(function () {
            var $s = $(this);
            var secili = this === $a[0];
            $s.attr({ 'aria-selected': secili ? 'true' : 'false', tabindex: secili ? '0' : '-1' });
            $s.parent().toggleClass('active', secili);
            $('#' + $s.attr('aria-controls')).prop('hidden', !secili);
        });
        yukle($('#' + $a.attr('aria-controls')));
        if (odakla) $a.trigger('focus');
    }

    $sekmeler.on('click', function (e) {
        e.preventDefault();
        sec($(this), false);
    });

    // Klavye: sol / sağ ok, Home / End ile sekmeler arasında gezinme.
    $sekmeler.on('keydown', function (e) {
        var i = $sekmeler.index(this);
        var n = $sekmeler.length;
        var j = -1;
        if (e.key === 'ArrowRight') j = (i + 1) % n;
        else if (e.key === 'ArrowLeft') j = (i - 1 + n) % n;
        else if (e.key === 'Home') j = 0;
        else if (e.key === 'End') j = n - 1;
        if (j < 0) return;
        e.preventDefault();
        sec($sekmeler.eq(j), true);
    });

    // İlk açılışta: adresteki #sek_KOD sekmesi, yoksa ilk sekme.
    var $ilk = $sekmeler.filter(function () { return '#' + $(this).attr('aria-controls') === window.location.hash; });
    sec($ilk.length ? $ilk.first() : $sekmeler.first(), false);
});
