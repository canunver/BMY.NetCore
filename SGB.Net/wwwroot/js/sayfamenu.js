// Sayfa menüsü (OrtakSayfa/SayfaMenu?menuID=...): bir menü başlığının bütün seçenekleri, menu.xml'deki kolonlara göre yan yana.
// Yeni sisteme taşınmış sayfalar bağlantıdır (yeşil işaret); henüz taşınmamış olanlar soluk yazılır ve "kalan" olarak sayılır.
(function () {
    'use strict';

    var parametre = new URLSearchParams(window.location.search);
    var menuID = parametre.get('menuID') || '';
    var $kolonlar = $('#kolonlar');
    var suzgec = '';

    function oku() {
        Sgb.mesaj('#hata', '');
        Sgb.al('api/ortaksayfa/sayfa-menu', { menuID: menuID, tumu: $('#tumu').prop('checked') ? '1' : '' })
            .done(function (s) {
                if (s.hata) { Sgb.mesaj('#hata', s.hata); return; }
                $('#baslik').text(s.baslik);
                document.title = s.baslik;
                $('#tumuAlan').prop('hidden', !s.yonetici);
                ciz(s.kolonlar || []);
            })
            .fail(function (xhr) {
                if (xhr.status === 403) Sgb.mesaj('#hata', 'Bu menüyü görme yetkiniz yok.', 'warning');
                else if (xhr.status !== 401) Sgb.mesaj('#hata', 'Menü okunamadı.');
            });
    }

    function ciz(kolonlar) {
        $kolonlar.empty();
        var tasinan = 0, kalan = 0;
        $.each(kolonlar, function (i, kolon) {
            var $k = $('<div class="sayfa-menu-kolon"></div>').appendTo($kolonlar);
            var $liste = null;
            $.each(kolon, function (j, s) {
                if (s.t === 'b') {
                    $('<div class="sm-baslik"></div>').addClass('sm-duzey-' + Math.min(s.duzey, 3))
                        .toggleClass('sm-gizli', s.gizli).text(s.ad).appendTo($k);
                    $liste = null;
                    return;
                }
                if (!$liste) $liste = $('<ul class="sm-liste"></ul>').appendTo($k);
                if (s.tasindi) tasinan++; else kalan++;
                var $li = $('<li></li>').addClass(s.tasindi ? 'sm-tasindi' : 'sm-kalan').toggleClass('sm-gizli', s.gizli)
                    .addClass(s.durum ? 'sm-durum-' + s.durum.toLowerCase() : '')
                    .attr('title', s.url + (s.gizli ? ' (menüde gizli)' : '')).appendTo($liste);
                $('<span class="glyphicon" aria-hidden="true"></span>')
                    .addClass(s.tasindi ? 'glyphicon-ok-sign' : 'glyphicon-time').appendTo($li);
                if (s.tasindi) {
                    var $a = $('<a></a>').attr('href', s.url).text(s.ad).appendTo($li);
                    if (s.yeni) $a.on('click', function (e) { e.preventDefault(); Sgb.pencere(s.url, s.ad); });
                    // Yeni (kırmızı) ekran açıldı: incelendi sayılır, Eski yapılır (bkz. menu.js).
                    if (s.durum === 'Yeni') $a.on('click', function () {
                        if (window.SgbMenu) window.SgbMenu.yeniIncelendi(s.url);
                        s.durum = 'Eski';
                        $li.removeClass('sm-durum-yeni').addClass('sm-durum-eski');
                    });
                } else {
                    $('<span class="sm-ad"></span>').text(s.ad).appendTo($li);
                    $('<span class="sr-only"></span>').text(' (henüz taşınmadı)').appendTo($li);
                }
            });
        });
        if (!kolonlar.length) $('<p class="text-muted"></p>').text('Bu menüde gösterilecek seçenek yok.').appendTo($kolonlar);
        $('#sayiTumu').text(tasinan + kalan);
        $('#sayiTasinan').text(tasinan);
        $('#sayiKalan').text(kalan);
        suz();
    }

    // Süzme (tümü / taşınan / kalan) ve arama: uymayan seçenekler, seçeneği kalmayan başlıklar ve kolonlar gizlenir.
    function suz() {
        var aranan = ($('#ara').val() || '').toLocaleLowerCase('tr');
        $kolonlar.find('li').each(function () {
            var $li = $(this);
            var uygun = (!suzgec || $li.hasClass('sm-' + suzgec)) &&
                (!aranan || $li.text().toLocaleLowerCase('tr').indexOf(aranan) >= 0);
            $li.prop('hidden', !uygun);
        });
        $kolonlar.find('.sm-liste').each(function () {
            $(this).prop('hidden', !$(this).children('li:not([hidden])').length);
        });
        $kolonlar.find('.sayfa-menu-kolon').each(function () {
            var $k = $(this);
            // Başlık, kendinden sonra (aynı ya da daha üst düzeyde başka başlığa kadar) görünür bir liste varsa görünür.
            var $ogeler = $k.children();
            $ogeler.filter('.sm-baslik').each(function () {
                var duzey = basDuzey(this);
                var gorunur = false;
                $(this).nextAll().each(function () {
                    if ($(this).hasClass('sm-baslik') && basDuzey(this) <= duzey) return false;
                    if ($(this).hasClass('sm-liste') && !this.hidden) { gorunur = true; return false; }
                });
                this.hidden = !gorunur;
            });
            $k.prop('hidden', !$k.children('.sm-liste:not([hidden])').length);
        });
    }

    function basDuzey(el) {
        var m = /sm-duzey-(\d)/.exec(el.className);
        return m ? +m[1] : 1;
    }

    $('[data-suz]').on('click', function () {
        suzgec = $(this).data('suz') || '';
        $('[data-suz]').removeClass('active').attr('aria-pressed', 'false');
        $(this).addClass('active').attr('aria-pressed', 'true');
        suz();
    });
    $('#ara').on('input', suz);
    $('#tumu').on('change', oku);

    if (!menuID) Sgb.mesaj('#hata', 'Menü belirtilmedi (menuID).');
    else oku();
})();
