// Sayfa yardımı (eski KYM/YardimSayfa.aspx). Üst şeritteki Yardım düğmesiyle pencerede açılır.
//   Okuyan: metin salt okunur gösterilir (bağlantılar yeni sekmede açılır).
//   Yazabilen (sistem yöneticisi; DBP sayfalarında DBP proje yöneticisi): HTML editörü (Sgb.editor), Kaydet, Düzenle
//   (salt okunur / düzenleme arasında geçiş) ve Dosyalar (ortak ekli dosyalar bileşeni, modül YRD, form = sayfa kodu).
// Uçlar: GET /api/yardim/oku?sayfaKod=, POST /api/yardim/kaydet.
$(function () {
    'use strict';

    var $kok = $('.yardim-sayfa');
    var sayfaKod = $kok.data('sayfa-kod') || '';
    var yazabilir = String($kok.data('yazabilir')) === '1';
    var BOS = 'Bu sayfa için yardım bilgisi girilmemiş.';

    function bildirim(metin, tur) { Sgb.bildirim(metin, tur); }

    if (!sayfaKod) {
        bildirim('Sayfa belirlenemedi.', 'danger');
        return;
    }

    // Kayıtlı HTML'den betik çalıştırabilecek parçalar atılır; bağlantılar yeni sekmede açılır (sayfa pencerede durur).
    function temizHtml(html) {
        var belge = new DOMParser().parseFromString('<div>' + (html || '') + '</div>', 'text/html');
        var kap = belge.body.firstChild;
        $(kap).find('script, iframe, object, embed, form, base, meta, link').remove();
        $(kap).find('*').each(function () {
            var el = this;
            $.each($.makeArray(el.attributes), function (i, a) {
                var ad = a.name.toLowerCase();
                if (ad.indexOf('on') === 0) el.removeAttribute(a.name);
                else if ((ad === 'href' || ad === 'src') && /^\s*javascript:/i.test(a.value)) el.removeAttribute(a.name);
            });
            if (el.tagName === 'A' && el.getAttribute('href')) {
                el.setAttribute('target', '_blank');
                el.setAttribute('rel', 'noopener');
            }
        });
        return kap.innerHTML;
    }

    // ------------------------------------------------------------------ salt okunur

    if (!yazabilir) {
        var $metin = $('#yardimMetin');
        Sgb.al('api/yardim/oku', { sayfaKod: sayfaKod }).done(function (v) {
            if (!v.basarili) { bildirim(v.hata, 'danger'); return; }
            if (v.metin && v.metin.replace(/<[^>]*>|&nbsp;|\s/g, '') !== '' || /<(img|table)/i.test(v.metin || ''))
                $metin.html(temizHtml(v.metin));
            else
                $('<p class="yardim-bos"></p>').text(BOS).appendTo($metin.empty());
        });
        return;
    }

    // ------------------------------------------------------------------ yazabilen

    var yukseklik = Math.max(300, window.innerHeight - $('.yardim-araclar').outerHeight(true) - 24);
    var editor = Sgb.editor('#txtEditor', { yukseklik: yukseklik, yerTutucu: BOS });
    var dosyalar = null;
    var okunan = '';

    function degisti() { return editor.html() !== okunan; }

    Sgb.al('api/yardim/oku', { sayfaKod: sayfaKod }).done(function (v) {
        if (!v.basarili) { bildirim(v.hata, 'danger'); return; }
        editor.html(v.metin || '');
        okunan = editor.html();
    });

    $('#btnKaydet').on('click', function () {
        var $d = $(this).prop('disabled', true);
        var metin = editor.html();
        Sgb.gonder('api/yardim/kaydet', { sayfaKod: sayfaKod, metin: metin }).done(function (s) {
            if (!s.basarili) { bildirim(s.hata, 'danger'); return; }
            okunan = metin;
            bildirim(s.bilgi, 'success');
        }).always(function () { $d.prop('disabled', false); });
    });

    // Eski Düzenle: editörü salt okunur / düzenlenebilir yapar.
    $('#btnDuzenle').on('click', function () {
        var duzenlemede = $(this).attr('aria-pressed') === 'true';
        editor.jodit.setReadOnly(duzenlemede);
        $(this).attr('aria-pressed', duzenlemede ? 'false' : 'true').toggleClass('active', !duzenlemede);
    }).addClass('active');

    // Dosyalar kipsiz, taşınabilir bir pencerede (eski EkliDosyalarForm penceresi): açıkken metin de kullanılabilir.
    // Dosyaya bağlantı vermek için satırdaki bağlantı düğmesiyle adres kopyalanır, editörün bağlantı düğmesiyle eklenir.
    // Bileşen ilk açılışta, pencere görünürken kurulur (tablo gizli bir alanda kurulursa boyutu yanlış hesaplanıyor).
    var pencere = document.getElementById('yardimDosyalar');
    $('#btnDosyalar').on('click', function () {
        if (pencere.open) { pencere.close(); return; }
        pencere.show();
        $(this).attr('aria-expanded', 'true');
        if (!dosyalar) dosyalar = Sgb.ekliDosyalar('#dosyalar', { modul: 'YRD', form: sayfaKod });
        else dosyalar.yenile();
    });
    $('#btnDosyalarKapat').on('click', function () { pencere.close(); });
    $(pencere).on('close', function () { $('#btnDosyalar').attr('aria-expanded', 'false'); });
    $(pencere).on('keydown', function (e) { if (e.key === 'Escape') pencere.close(); });

    // Başlığından tutulup taşınır; ekranın dışına çıkamaz.
    $(pencere).find('.sgb-pencere-baslik').on('pointerdown', function (e) {
        if (e.button !== 0 || $(e.target).closest('button').length) return;
        e.preventDefault();
        var baslik = this;
        var r = pencere.getBoundingClientRect();
        var dx = e.clientX - r.left, dy = e.clientY - r.top;
        baslik.setPointerCapture(e.pointerId);
        function tasi(o) {
            var x = Math.min(Math.max(0, o.clientX - dx), window.innerWidth - r.width);
            var y = Math.min(Math.max(0, o.clientY - dy), window.innerHeight - 40);
            pencere.style.left = x + 'px';
            pencere.style.top = y + 'px';
            pencere.style.right = 'auto';
            pencere.style.bottom = 'auto';
        }
        function birak() {
            baslik.removeEventListener('pointermove', tasi);
            baslik.removeEventListener('pointerup', birak);
            baslik.removeEventListener('pointercancel', birak);
        }
        baslik.addEventListener('pointermove', tasi);
        baslik.addEventListener('pointerup', birak);
        baslik.addEventListener('pointercancel', birak);
    });

    // Kaydedilmemiş değişiklik varken pencere kapatılırsa (sekme kapatma / yenileme) uyar.
    window.addEventListener('beforeunload', function (e) {
        if (degisti()) { e.preventDefault(); e.returnValue = ''; }
    });
});
