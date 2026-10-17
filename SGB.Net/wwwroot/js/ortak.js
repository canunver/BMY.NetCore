// Giriş sonrası sayfaların ortak JavaScript yardımcıları (jQuery gerektirir).
//   Sgb.al / Sgb.gonder : JSON uçlarına istek. Oturum düşmüşse (401) giriş sayfasına gider; POST'lara istek jetonu eklenir.
//   Sgb.mesaj           : sayfadaki uyarı kutusuna metin yazar (kalıcı uyarılar için).
//   Sgb.bildirim        : sağ üstte kendiliğinden kapanan bildirim (işlem sonuçları için; sayfa kaymaz).
//   Sgb.grid            : AG Grid tablosu oluşturur (Türkçe metinler, sayfanın temasıyla uyumlu görünüm; süzme kapalı).
//   Sgb.csvIndir        : tablodaki satırları CSV dosyası olarak indirir (Excel'e aktarma).
//   Sgb.pencere         : başka bir sayfayı pencere (çerçeve) olarak açar.
//   Sgb.agac            : ağaç ya da düz seçim listesi (tek seçimli ya da onay kutulu; düzey düzey okunur).
//   Sgb.indir           : sunucunun ürettiği dosyayı (Word / Excel raporu) indirir; hata dönerse bildirim gösterir.
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
        if (!$alan.length) $alan = $('<div id="sgbBildirimler" class="sgb-bildirimler" aria-live="polite"></div>');
        // Açık bir pencere (<dialog>) varsa bildirim onun içine konur; yoksa pencerenin arkasında kalır ve görünmez.
        var $pencere = $('dialog[open]').last();
        $alan.appendTo($pencere.length ? $pencere : document.body);
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
        // Veri verilmeden kurulan tablo açılışta "Yükleniyor..." göstermesin; yükleme göstergesi yalnızca istekte (loading: true) açılır.
        if (secenekler.rowData === undefined) secenekler.rowData = [];
        if (secenekler.loading === undefined) secenekler.loading = false;
        return agGrid.createGrid(el, secenekler);
    }

    // Tablodaki satırları (görünen sıra ve sütunlarla) Excel'in açtığı CSV dosyası olarak indirir.
    // hucre(sutunKimligi, deger): isteğe bağlı; hücre metnini değiştirmek için (ör. true/false yerine Aktif/Pasif).
    function csvIndir(tablo, dosyaAd, hucre) {
        var csv = tablo.getDataAsCsv({
            columnSeparator: ';',
            processCellCallback: function (p) {
                var d = hucre ? hucre(p.column.getColId(), p.value) : p.value;
                return d === undefined || d === null ? '' : d;
            }
        });
        // BOM: Excel dosyayı UTF-8 olarak açsın (Türkçe harfler).
        var adres = URL.createObjectURL(new Blob(['﻿' + csv], { type: 'text/csv;charset=utf-8' }));
        var a = document.createElement('a');
        a.href = adres;
        a.download = dosyaAd + '_' + bugun().replace(/-/g, '') + '.csv';
        document.body.appendChild(a);
        a.click();
        a.remove();
        setTimeout(function () { URL.revokeObjectURL(adres); }, 1000);
    }

    // Başka bir sayfayı bu sayfanın üstünde pencere olarak açar (eski showPopWin). Sayfa çerçevede menüsüz açılır (?menuYok=1).
    // kapaninca: isteğe bağlı; pencere kapatıldığında çağrılır.
    function pencere(adres, baslik, kapaninca) {
        var $p = $('<dialog class="sgb-pencere"></dialog>');
        var $ust = $('<div class="sgb-pencere-baslik"></div>').appendTo($p);
        $('<h2></h2>').text(baslik || '').appendTo($ust);
        $('<button type="button" class="close" aria-label="Kapat"><span aria-hidden="true">&times;</span></button>')
            .on('click', function () { $p[0].close(); }).appendTo($ust);
        $('<iframe></iframe>').attr('title', baslik || '')
            .attr('src', adres + (adres.indexOf('?') >= 0 ? '&' : '?') + 'menuYok=1').appendTo($p);
        $p.on('close', function () {
            $p.remove();
            if (kapaninca) kapaninca();
        }).appendTo(document.body);
        $p[0].showModal();
    }

    // Ağaç ya da düz seçim listesi. element: bir <ul>. Düğümler düzey düzey okunur (alt düğümler dal açılınca istenir).
    //   yukle(ust)         : ust düğümünün (kökte null) alt düğümlerini veren söz (promise). Düğüm: { kod, ad, altVar, ... }
    //   secilince(dugum)   : tek seçimli kullanım; ada tıklanınca çağrılır, seçili düğüm vurgulanır.
    //   isaretli(dugum)    : verilirse düğümler onay kutuludur (çoklu seçim); kutunun ilk durumunu döndürür.
    //   isaretlenince(dugum, evet) : onay kutusu değişince çağrılır.
    //   ilkAcik            : kökteki ilk dal açık gelsin mi?   duz: alt düğümü olmayan düz liste (açma sütunu çizilmez).
    //   simge(dugum)       : düğüm adının başına konacak glyphicon sınıfı (ör. 'glyphicon-user'); ipucu(dugum): üzerine gelince çıkan metin.
    //   seciliMi(dugum)    : tek seçimli kullanımda, yeniden çizilen düğümün seçili (vurgulu) gelip gelmeyeceği.
    //   bosMetin           : kökte hiç düğüm yoksa yazılacak metin.   hata(xhr): okuma başarısız olursa çağrılır.
    // Dönen nesne: yenile() ağacı baştan okur; isaretleriGuncelle() kutuları isaretli() ile yeniden eşler; temizle() boşaltır;
    // altiniYenile(kod) yalnızca o düğümün altını yeniden okur ve açar (düğüm çizili değilse ya da kod boşsa ağacın tamamını).
    function agac(element, ozellikler) {
        var o = ozellikler || {};
        var $kok = $(element).addClass('sgb-agac');
        var surum = 0; // yenile() sonrası gelen eski yanıtlar atılır

        function dugumCiz(d) {
            var $li = $('<li></li>').data('dugum', d);
            var $satir = $('<div class="satir"></div>').appendTo($li);
            if (!o.duz) {
                // Alt düğümü olmayanın yanında açma düğmesi gösterilmez.
                if (!d.altVar) $('<span class="ac-yok"></span>').appendTo($satir);
                else $('<button type="button" class="ac" aria-expanded="false">&#9656;</button>').attr('aria-label', 'Aç: ' + d.ad).on('click', function () {
                    var $d = $(this), $alt = $li.children('ul');
                    if ($d.attr('aria-expanded') === 'true') { $alt.prop('hidden', true); $d.attr('aria-expanded', 'false').html('&#9656;'); return; }
                    $d.attr('aria-expanded', 'true').html('&#9662;');
                    if ($alt.length) { $alt.prop('hidden', false); return; }
                    doldur($('<ul></ul>').appendTo($li), d, $d);
                }).appendTo($satir);
            }
            if (o.isaretli) {
                var kimlik = 'agc' + Math.random().toString(36).slice(2);
                $('<input type="checkbox" />').attr('id', kimlik).data('dugum', d).prop('checked', !!o.isaretli(d))
                    .on('change', function () { if (o.isaretlenince) o.isaretlenince(d, this.checked); }).appendTo($satir);
                $('<label></label>').attr('for', kimlik).text(d.ad).appendTo($satir);
            } else {
                var $sec = $('<button type="button" class="sec"></button>').text(d.ad).attr('aria-current', o.seciliMi && o.seciliMi(d) ? 'true' : null);
                if (o.simge && o.simge(d)) $('<span class="glyphicon simge" aria-hidden="true"></span>').addClass(o.simge(d)).prependTo($sec);
                if (o.ipucu) $sec.attr('title', o.ipucu(d) || null);
                $sec.on('click', function () {
                    $kok.find('.sec').removeAttr('aria-current');
                    $(this).attr('aria-current', 'true');
                    if (o.secilince) o.secilince(d);
                }).appendTo($satir);
            }
            return $li;
        }

        // $ac: dalı açan düğme (kökte yok). Dalın altı boş çıkarsa düğme kaldırılır.
        function doldur($hedef, ust, $ac) {
            var benim = surum;
            return o.yukle(ust).done(function (dugumler) {
                if (benim !== surum) return;
                dugumler = dugumler || [];
                if (!dugumler.length) {
                    if ($ac) { $ac.replaceWith('<span class="ac-yok"></span>'); $hedef.remove(); }
                    else if (o.bosMetin) $('<li></li>').append($('<span class="bos"></span>').text(o.bosMetin)).appendTo($hedef);
                    return;
                }
                $.each(dugumler, function (i, d) { dugumCiz(d).appendTo($hedef); });
                if (!$ac && o.ilkAcik) $hedef.children('li').first().find('> .satir > .ac').trigger('click');
            }).fail(function (xhr) { if (o.hata) o.hata(xhr); });
        }

        function temizle() { surum++; $kok.empty(); }
        function yenile() { temizle(); return doldur($kok, null, null); }
        function isaretleriGuncelle() {
            $kok.find('input[type=checkbox]').each(function () { this.checked = !!o.isaretli($(this).data('dugum')); });
        }

        function altiniYenile(kod) {
            var $li = kod ? $kok.find('li').filter(function () { var d = $(this).data('dugum'); return d && d.kod === kod; }).first() : $();
            if (!$li.length) return yenile();
            // Düğüm, altı var sayılarak yeniden çizilir ve açılır; altı boş çıkarsa açma düğmesi kendiliğinden kalkar.
            var $yeni = dugumCiz($.extend($li.data('dugum'), { altVar: true }));
            $li.replaceWith($yeni);
            $yeni.find('> .satir > .ac').trigger('click');
        }

        yenile();
        return { yenile: yenile, temizle: temizle, isaretleriGuncelle: isaretleriGuncelle, altiniYenile: altiniYenile };
    }

    // Sunucunun ürettiği dosyayı (rapor: Word, Excel ...) indirir. Sunucu dosya yerine { hata } döndürürse bildirim olarak
    // gösterir. Dönen söz, indirme başladığında (ya da hata gösterildiğinde) tamamlanır.
    function indir(adres, parametreler) {
        var tam = KOK + adres;
        if (parametreler) {
            var sorgu = $.param(parametreler);
            if (sorgu) tam += (tam.indexOf('?') >= 0 ? '&' : '?') + sorgu;
        }
        return fetch(tam, { credentials: 'same-origin', cache: 'no-store' }).then(function (yanit) {
            if (yanit.status === 401) { window.top.location.href = KOK + 'Giris'; return; }
            if (!yanit.ok) { bildirim(yanit.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Dosya hazırlanamadı.', 'danger'); return; }
            var tur = yanit.headers.get('Content-Type') || '';
            if (tur.indexOf('json') >= 0)
                return yanit.json().then(function (veri) { bildirim(veri.hata || 'Dosya hazırlanamadı.', 'danger'); });
            // Dosya yerine sayfa geldiyse (ör. oturum düşmüş ve giriş sayfasına yönlenmiş) indirilmez.
            if (tur.indexOf('text/html') >= 0) { bildirim('Dosya hazırlanamadı.', 'danger'); return; }

            // Dosya adı Content-Disposition başlığından alınır (filename*=UTF-8''... ya da filename=...).
            var baslik = yanit.headers.get('Content-Disposition') || '';
            var ad = /filename\*=UTF-8''([^;]+)/i.exec(baslik) || /filename="?([^";]+)"?/i.exec(baslik);
            return yanit.blob().then(function (icerik) {
                var yerel = URL.createObjectURL(icerik);
                var a = document.createElement('a');
                a.href = yerel;
                a.download = ad ? decodeURIComponent(ad[1]) : 'rapor';
                document.body.appendChild(a);
                a.click();
                a.remove();
                setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
            });
        }).catch(function () { bildirim('Dosya hazırlanamadı.', 'danger'); });
    }

    // Yıl kutusu: tüm sayfalarda yıl tek tip seçilir, <input type="number" class="sgb-yil">; yukarı / aşağı okla ya da yazarak
    // değişir. Alt ve üst sınır sunucunun verdiği yıllardan (ve seçili yıldan) alınır, seçili yıl kutuya yazılır.
    function yilKutusu(kutu, yillar, secili) {
        var $k = $(kutu);
        var y = $.map((yillar || []).concat(secili != null && secili !== '' ? [secili] : []), function (x) {
            x = parseInt(x, 10);
            return x > 0 ? x : null;
        });
        if (y.length) $k.attr({ min: Math.min.apply(null, y), max: Math.max.apply(null, y) });
        if (secili != null && secili !== '') $k.val(String(secili));
        return $k;
    }

    // Grafik (pasta, çizgi, sütun): SVG olarak çizilir, ek kütüphane gerekmez. element: grafiğin konacağı kutu.
    //   veri: { baslik, tur: 'pasta' | 'cizgi' | 'sutun', etiketler: [...], seriler: [{ ad, renk, degerler: [...] }], renkler: [...] }
    //   Pastada tek seri çizilir, dilim renkleri veri.renkler'den gelir. Veri /api/grafik ucundan ya da sayfanın kendisinden gelebilir.
    var GRAFIK_RENK = ['#3d8fc4', '#4c9a4c', '#d9a300', '#cf4b47', '#7b62b3', '#2aa198', '#c7672a', '#8a8f98'];
    var grafikSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 2 });

    function grafik(element, veri) {
        var NS = 'http://www.w3.org/2000/svg';
        function el(ad, oz, ust) {
            var e = document.createElementNS(NS, ad);
            for (var k in oz) e.setAttribute(k, oz[k]);
            if (ust) ust.appendChild(e);
            return e;
        }
        function ipucu(e, metin) { el('title', {}, e).textContent = metin; return e; }

        var $kutu = $(element).empty();
        var $f = $('<figure class="sgb-grafik"></figure>').appendTo($kutu);
        if (veri.baslik) $('<figcaption></figcaption>').text(veri.baslik).appendTo($f);

        var seriler = veri.seriler || [], etiketler = veri.etiketler || [];
        var pasta = veri.tur === 'pasta';
        var toplam = 0, enBuyuk = 0;
        $.each(seriler, function (i, s) { $.each(s.degerler, function (j, d) { toplam += d; enBuyuk = Math.max(enBuyuk, d); }); });
        if (!etiketler.length || !seriler.length || (pasta && toplam <= 0)) {
            $('<p class="sgb-grafik-bos"></p>').text('Gösterilecek veri yok.').appendTo($f);
            return;
        }

        var G = 640, Y = 340;
        var svg = el('svg', { viewBox: '0 0 ' + G + ' ' + Y, role: 'img', 'aria-label': veri.baslik || 'Grafik' });
        var aciklama = []; // { ad, renk }

        if (pasta) {
            var mx = G / 2, my = Y / 2, r = Y / 2 - 14, aci = -Math.PI / 2;
            var degerler = seriler[0].degerler;
            $.each(etiketler, function (i, ad) {
                var d = degerler[i] || 0, renk = (veri.renkler && veri.renkler[i]) || GRAFIK_RENK[i % GRAFIK_RENK.length];
                var metin = ad + ': ' + grafikSayi.format(d) + ' (%' + grafikSayi.format(Math.round(d * 1000 / toplam) / 10) + ')';
                aciklama.push({ ad: metin, renk: renk });
                if (d <= 0) return;
                var dilim;
                if (d >= toplam) dilim = el('circle', { cx: mx, cy: my, r: r, fill: renk }, svg);
                else {
                    var son = aci + d / toplam * 2 * Math.PI;
                    dilim = el('path', {
                        d: 'M' + mx + ' ' + my + ' L' + (mx + r * Math.cos(aci)).toFixed(2) + ' ' + (my + r * Math.sin(aci)).toFixed(2) +
                            ' A' + r + ' ' + r + ' 0 ' + (son - aci > Math.PI ? 1 : 0) + ' 1 ' + (mx + r * Math.cos(son)).toFixed(2) + ' ' + (my + r * Math.sin(son)).toFixed(2) + ' Z',
                        fill: renk, 'class': 'dilim'
                    }, svg);
                    // Dilimin ortasına değeri yazılır (dar dilimlerde yazılmaz).
                    if (d / toplam > 0.06) {
                        var orta = (aci + son) / 2;
                        el('text', { x: (mx + r * 0.62 * Math.cos(orta)).toFixed(1), y: (my + r * 0.62 * Math.sin(orta)).toFixed(1), 'class': 'deger', 'text-anchor': 'middle', 'dominant-baseline': 'middle' }, svg)
                            .textContent = grafikSayi.format(d);
                    }
                    aci = son;
                }
                ipucu(dilim, metin);
            });
        } else {
            var sol = 46, sag = 14, ust = 14, alt = 30, ic = G - sol - sag, yuk = Y - ust - alt;
            // Dikey eksen: 1, 2, 5 katlarıyla en çok altı aralık olacak en küçük adım
            var adim = Math.pow(10, Math.floor(Math.log(Math.max(enBuyuk, 1)) / Math.LN10) - 1);
            $.each([1, 2, 5, 10, 20, 50, 100], function (i, kat) {
                if ((enBuyuk || 1) / (adim * kat) <= 6) { adim *= kat; return false; }
            });
            var tavan = Math.max(adim, Math.ceil(enBuyuk / adim) * adim);
            function yKonum(d) { return ust + yuk - d / tavan * yuk; }
            for (var t = 0; t <= tavan + adim / 1000; t += adim) {
                el('line', { x1: sol, x2: G - sag, y1: yKonum(t), y2: yKonum(t), 'class': t === 0 ? 'eksen' : 'izgara' }, svg);
                el('text', { x: sol - 6, y: yKonum(t), 'text-anchor': 'end', 'dominant-baseline': 'middle' }, svg).textContent = grafikSayi.format(t);
            }
            var dilimG = ic / etiketler.length;
            $.each(etiketler, function (i, ad) {
                el('text', { x: sol + dilimG * (i + 0.5), y: Y - alt + 18, 'text-anchor': 'middle' }, svg).textContent = ad;
            });
            $.each(seriler, function (si, s) {
                var renk = s.renk || GRAFIK_RENK[si % GRAFIK_RENK.length];
                aciklama.push({ ad: s.ad, renk: renk });
                if (veri.tur === 'sutun') {
                    var sutunG = Math.min(46, dilimG * 0.8 / seriler.length);
                    $.each(s.degerler, function (i, d) {
                        var x = sol + dilimG * (i + 0.5) - sutunG * seriler.length / 2 + sutunG * si;
                        ipucu(el('rect', { x: x + 1, y: yKonum(d), width: sutunG - 2, height: Math.max(0, ust + yuk - yKonum(d)), fill: renk }, svg),
                            s.ad + ', ' + etiketler[i] + ': ' + grafikSayi.format(d));
                    });
                } else {
                    var noktalar = $.map(s.degerler, function (d, i) { return (sol + dilimG * (i + 0.5)).toFixed(1) + ',' + yKonum(d).toFixed(1); });
                    if (noktalar.length > 1) el('polyline', { points: noktalar.join(' '), fill: 'none', stroke: renk, 'class': 'cizgi' }, svg);
                    $.each(s.degerler, function (i, d) {
                        ipucu(el('circle', { cx: sol + dilimG * (i + 0.5), cy: yKonum(d), r: 5, fill: renk, 'class': 'nokta' }, svg),
                            s.ad + ', ' + etiketler[i] + ': ' + grafikSayi.format(d));
                    });
                }
            });
        }

        $f.append(svg);
        var $a = $('<ul class="sgb-grafik-aciklama"></ul>').appendTo($f);
        $.each(aciklama, function (i, a) { $('<li></li>').append($('<i aria-hidden="true"></i>').css('background', a.renk)).append(document.createTextNode(a.ad)).appendTo($a); });
    }

    return { KOK: KOK, meta: meta, al: al, gonder: gonder, mesaj: mesaj, bildirim: bildirim, tarih: tarih, bugun: bugun, grid: grid, csvIndir: csvIndir, pencere: pencere, agac: agac, indir: indir, grafik: grafik, yilKutusu: yilKutusu };
})(jQuery);
