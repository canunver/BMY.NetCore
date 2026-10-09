// Duyuru ekranı (Duyuru/Duyuru): duyuruları önem işaretiyle listeler.
// Bilgi ekranı olarak açıldığında liste sığmıyorsa yavaşça kayar, fare üzerindeyken durur (eski Duyuru.aspx).
// "Pencerede göster" işaretli duyuru varsa açılış sayfasına haber verir; açılış sayfası bunları bir pencerede gösterir.
$(function () {
    'use strict';

    var pencereKipi = /[?&]penceremi=/.test(window.location.search);
    var $kutu = $('#duyuruKutu');
    var $liste = $('#duyurular');

    function satir(d) {
        var $li = $('<li class="duyuru"></li>').addClass('onem-' + (d.onem >= 1 && d.onem <= 3 ? d.onem : 0));
        var $ust = $('<div class="duyuru-ust"></div>');
        $('<span class="duyuru-bayrak" aria-hidden="true"></span>').appendTo($ust);
        $('<time class="duyuru-tarih"></time>').text(d.tarih).appendTo($ust);
        if (d.onemAd) $('<span class="duyuru-onem"></span>').text(d.onemAd).appendTo($ust);
        $li.append($ust);
        if (d.konu) $('<div class="duyuru-konu"></div>').text(d.konu).appendTo($li);
        // Metin yönetici tarafından HTML olarak girilir (Duyuru Girişi); biçimiyle gösterilir.
        $('<div class="duyuru-metin"></div>').html(d.metin).appendTo($li);
        return $li;
    }

    // Eski kaydırma: sona gelince 2 sn bekler, başa döner.
    function kaydirmayaBasla() {
        var kutu = $kutu[0];
        var dur = false;
        $kutu.on('mouseenter focusin', function () { dur = true; })
            .on('mouseleave focusout', function () { dur = false; });
        function adim() {
            var bekle = 60;
            if (!dur) {
                var once = kutu.scrollTop;
                kutu.scrollTop = once + 1;
                if (kutu.scrollTop === once) {
                    bekle = 2000;
                    setTimeout(function () { if (!dur) kutu.scrollTop = 0; }, 1500);
                }
            }
            setTimeout(adim, bekle);
        }
        setTimeout(adim, 2000);
    }

    Sgb.al('api/duyuru/goster', pencereKipi ? { penceremi: 1 } : null)
        .done(function (veri) {
            if (veri.hata) { Sgb.mesaj('#hata', veri.hata); return; }
            if (!veri.duyurular.length) {
                $('<li class="duyuru-bos"></li>').text(veri.bos).appendTo($liste);
                return;
            }
            $.each(veri.duyurular, function (i, d) { $liste.append(satir(d)); });

            if (!pencereKipi) {
                if (!window.matchMedia('(prefers-reduced-motion: reduce)').matches
                    && $kutu[0].scrollHeight > $kutu[0].clientHeight + 4)
                    kaydirmayaBasla();
                if (veri.penceredeVar && window.parent !== window)
                    window.parent.postMessage({ tur: 'duyuruPenceresi', baslik: veri.pencereBaslik, kapat: veri.kapat }, window.location.origin);
            }
        })
        .fail(function (xhr) {
            if (xhr.status !== 401) Sgb.mesaj('#hata', 'Duyurular alınamadı.');
        });
});
