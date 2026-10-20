// Tema Değiştirme (KYM/TemaDegistir): eski TemaDegistir.aspx.
//   Her tema, o temanın renkleriyle çizilmiş küçük bir ekran önizlemesiyle gösterilir (kart data-tema taşır,
//   renkler css/tema.css'teki değişkenlerden gelir). Karta tıklayınca sayfa hemen o temayla görünür;
//   Uygula seçimi /api/ortaksayfa/tema ile kaydeder ve tüm sayfalarda kullanılır. Vazgeç kayıtlı temaya döner.
//   Kaydedilmeden sayfadan çıkılırsa sonraki sayfa kayıtlı temayla açılır.
(function ($) {
    'use strict';

    var UC = 'api/ortaksayfa/tema';
    var ANAHTAR = 'sgb.menu';       // menu.js'in tarayıcıda sakladığı menü verisi (tema dahil)
    var kok = document.documentElement;
    var metin = {};
    try { metin = JSON.parse(document.getElementById('tdMetin').textContent); } catch (e) { }

    var kayitli = { tema: 'Klasik', ust: 'renkli' };
    var secili = { tema: 'Klasik', ust: 'renkli' };
    var $kartlar = $('#tdKartlar');

    function el(etiket, sinif, yazi) {
        var e = document.createElement(etiket);
        if (sinif) e.className = sinif;
        if (yazi !== undefined && yazi !== null) e.textContent = yazi;
        return e;
    }

    // Önizleme: üst şerit, sol menü, bölüm kutusu, tablo ve düğme. Yazı yerine çizgiler kullanılır.
    function onizleme() {
        var o = el('div', 'to');
        o.setAttribute('aria-hidden', 'true');

        var ust = el('div', 'to-ust');
        ust.appendChild(el('span', 'to-logo'));
        ust.appendChild(el('span', 'to-cizgi to-c-ust'));
        ust.appendChild(el('span', 'to-bosluk'));
        ust.appendChild(el('span', 'to-avatar'));
        o.appendChild(ust);

        var govde = el('div', 'to-govde');
        var yan = el('div', 'to-yan');
        for (var i = 0; i < 5; i++) {
            var s = el('div', 'to-yan-satir' + (i === 1 ? ' burada' : ''));
            s.appendChild(el('span', 'to-ikon'));
            s.appendChild(el('span', 'to-cizgi'));
            yan.appendChild(s);
        }
        govde.appendChild(yan);

        var ic = el('div', 'to-ic');
        var bolum = el('div', 'to-bolum');
        var bb = el('div', 'to-bolum-baslik');
        bb.appendChild(el('span', 'to-cizgi'));
        bolum.appendChild(bb);
        var alanlar = el('div', 'to-alanlar');
        for (var j = 0; j < 2; j++) {
            var a = el('div', 'to-alan');
            a.appendChild(el('span', 'to-cizgi to-etiket'));
            a.appendChild(el('span', 'to-girdi'));
            alanlar.appendChild(a);
        }
        bolum.appendChild(alanlar);
        var dugmeler = el('div', 'to-dugmeler');
        dugmeler.appendChild(el('span', 'to-dugme'));
        dugmeler.appendChild(el('span', 'to-dugme ikincil'));
        bolum.appendChild(dugmeler);
        ic.appendChild(bolum);

        var tablo = el('div', 'to-tablo');
        var bas = el('div', 'to-satir to-tablo-baslik');
        for (var k = 0; k < 3; k++) bas.appendChild(el('span'));
        tablo.appendChild(bas);
        for (var r = 0; r < 3; r++) {
            var satir = el('div', 'to-satir' + (r === 1 ? ' secili' : ''));
            satir.appendChild(el('span', 'to-hucre'));
            satir.appendChild(el('span', 'to-hucre salt-okunur'));
            satir.appendChild(el('span', 'to-hucre'));
            tablo.appendChild(satir);
        }
        ic.appendChild(tablo);

        var ikinci = el('div', 'to-bolum2');
        ikinci.appendChild(el('span', 'to-cizgi'));
        ic.appendChild(ikinci);

        govde.appendChild(ic);
        o.appendChild(govde);
        return o;
    }

    function kartlariCiz(veri) {
        $kartlar.empty();
        var kullanilan = $kartlar.data('kullanilan') || '';
        (veri.temalar || []).forEach(function (tema) {
            var kart = el('button', 'td-kart');
            kart.type = 'button';
            kart.setAttribute('role', 'radio');
            kart.setAttribute('data-tema', tema);
            kart.appendChild(onizleme());
            var alt = el('div', 'td-kart-alt');
            alt.appendChild(el('span', 'td-renk'));
            alt.appendChild(el('span', 'td-ad', (veri.temaAdlari && veri.temaAdlari[tema]) || tema));
            alt.appendChild(el('span', 'td-kullanilan label', kullanilan));
            kart.appendChild(alt);
            kart.addEventListener('click', function () { sec(tema, secili.ust); });
            $kartlar.append(kart);
        });
    }

    // Seçimi sayfaya uygular (kaydetmez).
    function sec(tema, ust) {
        secili = { tema: tema, ust: ust };
        kok.setAttribute('data-tema', tema);
        if (ust === 'sade') kok.setAttribute('data-ust', 'sade'); else kok.removeAttribute('data-ust');
        $kartlar.attr('data-ust', ust);
        $kartlar.children('.td-kart').each(function () {
            var bu = this.getAttribute('data-tema') === tema;
            this.classList.toggle('secili', bu);
            this.setAttribute('aria-checked', bu ? 'true' : 'false');
            this.classList.toggle('kayitli', this.getAttribute('data-tema') === kayitli.tema);
        });
        $('input[name="ustSerit"][value="' + ust + '"]').prop('checked', true);
        // Klasik temada üst şerit hep beyazdır
        $('input[name="ustSerit"]').prop('disabled', tema === 'Klasik');
        var degisti = tema !== kayitli.tema || ust !== kayitli.ust;
        $('#btnUygula, #btnVazgec').prop('disabled', !degisti);
    }

    // Saklanan menü verisi güncellenir; menülü sayfada menu.js de bilgilendirilir (kullanıcı menüsündeki düğmeler).
    function sakla(tema, ust) {
        if (window.SgbMenu && window.SgbMenu.temaUygula) {
            window.SgbMenu.temaUygula(tema, ust);
            return;
        }
        try {
            var veri = JSON.parse(localStorage.getItem(ANAHTAR) || 'null');
            if (veri) {
                veri.tema = tema;
                veri.temaUst = ust;
                localStorage.setItem(ANAHTAR, JSON.stringify(veri));
            }
        } catch (e) { /* tarayıcıda saklanamazsa sonraki girişte sunucudan okunur */ }
    }

    function uygula() {
        var $d = $('#btnUygula').prop('disabled', true);
        Sgb.gonder(UC, { tema: secili.tema, ust: secili.ust })
            .done(function (sonuc) {
                if (sonuc && sonuc.basarili) {
                    kayitli = { tema: secili.tema, ust: secili.ust };
                    sakla(kayitli.tema, kayitli.ust);
                    sec(kayitli.tema, kayitli.ust);
                    Sgb.bildirim(metin.kaydedildi || 'Tema kaydedildi.', 'success');
                } else {
                    $d.prop('disabled', false);
                    Sgb.bildirim((metin.kaydedilemedi || 'Tema kaydedilemedi.') + (sonuc && sonuc.hata ? ' ' + sonuc.hata : ''), 'danger');
                }
            })
            .fail(function () {
                $d.prop('disabled', false);
                Sgb.bildirim(metin.kaydedilemedi || 'Tema kaydedilemedi.', 'danger');
            });
    }

    $('input[name="ustSerit"]').on('change', function () { sec(secili.tema, this.value); });
    $('#btnUygula').on('click', uygula);
    $('#btnVazgec').on('click', function () { sec(kayitli.tema, kayitli.ust); });

    Sgb.al(UC).done(function (veri) {
        kayitli = { tema: veri.tema, ust: veri.ust || 'renkli' };
        kartlariCiz(veri);
        sec(kayitli.tema, kayitli.ust);
        // Başka bir tarayıcıda değiştirilmişse saklanan veri de sunucudakine uyar.
        sakla(kayitli.tema, kayitli.ust);
    }).fail(function () {
        Sgb.bildirim(metin.okunamadi || 'Tema okunamadı.', 'danger');
    });
})(jQuery);
