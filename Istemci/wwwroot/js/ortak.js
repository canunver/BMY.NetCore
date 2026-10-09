// Giriş sonrası sayfaların ortak JavaScript yardımcıları (jQuery gerektirir).
//   Sgb.al / Sgb.gonder : JSON uçlarına istek. Oturum düşmüşse (401) giriş sayfasına gider; POST'lara istek jetonu eklenir.
//   Sgb.mesaj           : sayfadaki uyarı kutusuna metin yazar (kalıcı uyarılar için).
//   Sgb.bildirim        : sağ üstte kendiliğinden kapanan bildirim (işlem sonuçları için; sayfa kaymaz).
//   Sgb.grid            : AG Grid tablosu oluşturur (Türkçe metinler, sayfanın temasıyla uyumlu görünüm; süzme kapalı).
//   Sgb.tarih           : yyyy-MM-dd -> gg.aa.yyyy
// Bütün sayfalar uygulama kökünün bir alt klasöründedir; adresler buna göre "../api/..." biçimindedir.
window.Sgb = (function ($) {
    'use strict';

    var KOK = '../';

    function meta(ad) {
        var m = document.querySelector('meta[name="' + ad + '"]');
        return m ? m.content : '';
    }

    function istek(yontem, adres, veri) {
        return $.ajax({
            url: KOK + adres,
            method: yontem,
            dataType: 'json',
            contentType: veri !== undefined ? 'application/json; charset=utf-8' : undefined,
            data: veri !== undefined ? JSON.stringify(veri) : undefined,
            headers: yontem === 'GET' ? {} : { RequestVerificationToken: meta('istek-jetonu') },
            cache: false
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = KOK + 'Giris';
        });
    }

    function al(adres, parametreler) {
        if (parametreler) {
            var sorgu = $.param(parametreler);
            if (sorgu) adres += (adres.indexOf('?') >= 0 ? '&' : '?') + sorgu;
        }
        return istek('GET', adres);
    }

    function gonder(adres, veri) {
        return istek('POST', adres, veri || {});
    }

    // tur: success | info | warning | danger. Metin boşsa kutu gizlenir.
    function mesaj(kutu, metin, tur) {
        var $k = $(kutu);
        $k.removeClass('alert-success alert-info alert-warning alert-danger')
            .addClass('alert-' + (tur || 'danger'))
            .text(metin || '')
            .prop('hidden', !metin);
    }

    // Sağ üstte açılan bildirim (sayfa kaymaz). tur: success | info | warning | danger.
    // Başarı ve bilgi 4 sn, uyarı ve hata 8 sn sonra kendiliğinden kapanır; fare üzerindeyken beklenir. Aynı metin üst üste açılmaz.
    function bildirim(metin, tur) {
        if (!metin) return;
        tur = tur || 'danger';
        var $alan = $('#sgbBildirimler');
        if (!$alan.length) $alan = $('<div id="sgbBildirimler" class="sgb-bildirimler" aria-live="polite"></div>').appendTo(document.body);
        $alan.children().filter(function () { return $(this).data('metin') === metin; }).remove();

        var $b = $('<div class="sgb-bildirim alert alert-' + tur + '"></div>')
            .attr('role', tur === 'danger' || tur === 'warning' ? 'alert' : 'status')
            .data('metin', metin);
        $('<button type="button" class="close" aria-label="Kapat"><span aria-hidden="true">&times;</span></button>').appendTo($b);
        $('<div class="sgb-bildirim-metin"></div>').text(metin).appendTo($b);
        $b.appendTo($alan);

        var sure = tur === 'danger' || tur === 'warning' ? 8000 : 4000;
        var zamanlayici;
        function kapat() {
            clearTimeout(zamanlayici);
            $b.addClass('kapaniyor');
            setTimeout(function () { $b.remove(); }, 200);
        }
        function kur() { zamanlayici = setTimeout(kapat, sure); }
        $b.on('click', '.close', kapat).on('mouseenter', function () { clearTimeout(zamanlayici); }).on('mouseleave', kur);
        kur();
    }

    function tarih(deger) {
        var p = /^(\d{4})-(\d{2})-(\d{2})/.exec(deger || '');
        return p ? p[3] + '.' + p[2] + '.' + p[1] : (deger || '');
    }

    function bugun() {
        var d = new Date();
        var iki = function (s) { return (s < 10 ? '0' : '') + s; };
        return d.getFullYear() + '-' + iki(d.getMonth() + 1) + '-' + iki(d.getDate());
    }

    // AG Grid'in ekranda görünen metinleri. Eksik olanlar İngilizce kalır.
    var GRID_METIN = {
        page: 'Sayfa', of: '/', to: '-', more: 'daha fazla', next: 'Sonraki', last: 'Son', first: 'İlk', previous: 'Önceki',
        loadingOoo: 'Yükleniyor...', noRowsToShow: 'Gösterilecek kayıt yok',
        filterOoo: 'Süz...', equals: 'Eşit', notEqual: 'Eşit değil', blank: 'Boş', notBlank: 'Boş değil',
        lessThan: 'Küçük', greaterThan: 'Büyük', lessThanOrEqual: 'Küçük veya eşit', greaterThanOrEqual: 'Büyük veya eşit',
        inRange: 'Aralıkta', inRangeStart: 'başlangıç', inRangeEnd: 'bitiş',
        contains: 'İçerir', notContains: 'İçermez', startsWith: 'İle başlar', endsWith: 'İle biter',
        before: 'Önce', after: 'Sonra', andCondition: 'VE', orCondition: 'VEYA',
        applyFilter: 'Uygula', resetFilter: 'Sıfırla', clearFilter: 'Temizle', cancelFilter: 'Vazgeç',
        selectAll: 'Tümünü seç', searchOoo: 'Ara...', pinColumn: 'Sabitle', autosizeThisColumn: 'Bu sütunu sığdır',
        autosizeAllColumns: 'Bütün sütunları sığdır', resetColumns: 'Sütunları sıfırla', copy: 'Kopyala',
        ariaFilterValue: 'Süzme değeri', ariaChecked: 'seçili', ariaUnchecked: 'seçili değil',
        pageSizeSelectorLabel: 'Sayfa boyutu:'
    };

    // ozellikler: AG Grid gridOptions. Varsayılanlar: sıralama ve sütun genişliği değiştirme açık, süzme kapalı.
    function grid(element, ozellikler) {
        var el = typeof element === 'string' ? document.querySelector(element) : element;
        el.classList.add('ag-theme-quartz', 'sgb-grid');
        var secenekler = $.extend(true, {
            localeText: GRID_METIN,
            defaultColDef: { sortable: true, filter: false, resizable: true },
            animateRows: false,
            suppressCellFocus: true
        }, ozellikler);
        return agGrid.createGrid(el, secenekler);
    }

    return { KOK: KOK, meta: meta, al: al, gonder: gonder, mesaj: mesaj, bildirim: bildirim, tarih: tarih, bugun: bugun, grid: grid };
})(jQuery);
