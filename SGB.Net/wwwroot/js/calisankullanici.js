// Sistemi Kullanan Kişilerin Listesi (KYM/CalisanKullaniciListe) ve Sistem Kullanım Grafiği (KYM/CalisanKullaniciGrafik).
// Uçlar: /api/calisankullanici/liste ve /grafik (KYM/CalisanKullaniciUclari.cs). Kaynak KULLANICI_BILGI (+ arşivi) tablosudur.
$(function () {
    'use strict';

    var UC = 'api/calisankullanici/';

    function hataYaz(metin) { $('#hata').text(metin || '').prop('hidden', !metin); }
    function istekHatasi(xhr) {
        if (xhr && xhr.status === 401) return;
        hataYaz(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Bilgiler okunamadı.');
    }

    // Tabloyu sayfanın kalan yüksekliğine yayar (kaydırma tablonun içinde kalır).
    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(enAz, Math.floor(window.innerHeight - ust - 24)) + 'px';
    }

    // ---------------- Liste ----------------
    var $liste = $('.ck-liste');
    if ($liste.length) {
        var d = $liste[0].dataset;
        var tabloEl = document.getElementById('grdListe');
        var tablo = Sgb.grid(tabloEl, {
            columnDefs: [
                { field: 'no', headerName: d.no, width: 70, type: 'rightAligned' },
                { field: 'ad', headerName: d.ad, width: 260, tooltipField: 'ad' },
                { field: 'islemTarih', headerName: d.tarih, width: 170 },
                { field: 'islemYer', headerName: d.yer, width: 140 },
                { field: 'islemSayfa', headerName: d.sayfa, flex: 1, minWidth: 220, tooltipField: 'islemSayfa' }
            ],
            tooltipShowDelay: 400,
            overlayNoRowsTemplate: '<span class="text-muted">Bu sürede işlem yapan kullanıcı yok.</span>'
        });
        var yay = function () { yuksekligeYay(tabloEl, 200); };
        $(window).on('resize', yay);
        yay();

        var listele = function () {
            var sure = parseInt($('#txtSure').val(), 10);
            if (!(sure > 0)) { sure = 5; $('#txtSure').val(5); }
            var $b = $('#btnListele').prop('disabled', true);
            hataYaz('');
            tablo.setGridOption('loading', true);
            Sgb.al(UC + 'liste', { sayfa: $('#txtSayfa').val() || '', sure: sure })
                .done(function (v) {
                    if (v.hata) { hataYaz(v.hata); return; }
                    tablo.setGridOption('rowData', v.satirlar || []);
                    $('#lblAdet').text((v.adet || 0) + ' ' + d.adet);
                })
                .fail(istekHatasi)
                .always(function () { tablo.setGridOption('loading', false); $b.prop('disabled', false); });
        };
        $('#frmKriter').on('submit', listele);
        // Eski sayfa da açılışta listeliyordu.
        listele();
    }

    // ---------------- Grafik ----------------
    var $grafikSayfa = $('.ck-grafik-sayfa');
    if (!$grafikSayfa.length) return;

    var sayi = new Intl.NumberFormat('tr-TR');
    function iki(n) { return (n < 10 ? '0' : '') + n; }
    function isoGun(t) { return t.getFullYear() + '-' + iki(t.getMonth() + 1) + '-' + iki(t.getDate()); }
    function gunMetin(t) { return iki(t.getDate()) + '.' + iki(t.getMonth() + 1) + '.' + t.getFullYear(); }
    function saatMetin(t) { return iki(t.getHours()) + ':' + iki(t.getMinutes()); }
    function zamanSec(z) { $('input[name=zaman][value=' + z + ']').prop('checked', true); }

    // Hızlı aralıklar: Bu ay (eski sayfanın açılış değeri), Son 1 yıl, Son 5 yıl
    function aralikVer(ay) {
        var bugun = new Date(), bas;
        if (ay === 0) bas = new Date(bugun.getFullYear(), bugun.getMonth(), 1);
        else bas = new Date(bugun.getFullYear(), bugun.getMonth() - ay, bugun.getDate() + 1);
        $('#txtTarih1').val(isoGun(bas));
        $('#txtTarih2').val(isoGun(bugun));
    }
    $('.ck-hizli button').on('click', function () {
        aralikVer(parseInt(this.dataset.ay, 10));
        zamanSec(this.dataset.zaman);
        ciz();
    });

    // Bir noktanın açıklaması, zaman dilimine göre
    function noktaMetin(t, z) {
        if (z <= 2) return gunMetin(t) + ' ' + saatMetin(t);
        if (z === 3) return gunMetin(t);
        if (z === 4) return gunMetin(t) + ' başlayan hafta';
        return gunMetin(t) + ' başlayan 4 hafta';
    }

    function svgEl(ad, oz, ust) {
        var e = document.createElementNS('http://www.w3.org/2000/svg', ad);
        for (var k in oz) e.setAttribute(k, oz[k]);
        if (ust) ust.appendChild(e);
        return e;
    }

    // Zaman eksenli çizgi grafik: noktalar zamana göre yerleşir (boş dönemler boşluk olarak görünür), eksen etiketleri seyreltilir.
    function grafikCiz(kutu, noktalar, z, bas, son) {
        var $k = $(kutu).empty();
        var $f = $('<figure class="sgb-grafik ck-figur"></figure>').appendTo($k);
        $('<figcaption></figcaption>').text($grafikSayfa.data('baslik')).appendTo($f);
        if (!noktalar.length) { $('<p class="sgb-grafik-bos"></p>').text('Gösterilecek veri yok.').appendTo($f); return; }

        var G = 960, Y = 380, sol = 64, sag = 16, ust = 14, alt = 34, ic = G - sol - sag, yuk = Y - ust - alt;
        var veri = $.map(noktalar, function (n) { return { t: new Date(n.t), adet: n.adet }; });
        var t0 = Math.min(bas.getTime(), veri[0].t.getTime());
        var t1 = Math.max(son.getTime(), veri[veri.length - 1].t.getTime());
        if (t1 <= t0) t1 = t0 + 3600000;
        var enBuyuk = 0;
        $.each(veri, function (i, n) { enBuyuk = Math.max(enBuyuk, n.adet); });

        var adim = Math.pow(10, Math.floor(Math.log(Math.max(enBuyuk, 1)) / Math.LN10) - 1);
        $.each([1, 2, 5, 10, 20, 50, 100], function (i, kat) {
            if ((enBuyuk || 1) / (adim * kat) <= 6) { adim *= kat; return false; }
        });
        adim = Math.max(1, adim);
        var tavan = Math.max(adim, Math.ceil(enBuyuk / adim) * adim);
        function xK(t) { return sol + (t - t0) / (t1 - t0) * ic; }
        function yK(d) { return ust + yuk - d / tavan * yuk; }

        var svg = svgEl('svg', { viewBox: '0 0 ' + G + ' ' + Y, role: 'img', 'aria-label': $grafikSayfa.data('baslik') });
        for (var v = 0; v <= tavan + adim / 1000; v += adim) {
            svgEl('line', { x1: sol, x2: G - sag, y1: yK(v), y2: yK(v), 'class': v === 0 ? 'eksen' : 'izgara' }, svg);
            svgEl('text', { x: sol - 6, y: yK(v), 'text-anchor': 'end', 'dominant-baseline': 'middle' }, svg).textContent = sayi.format(v);
        }

        // Yatay eksen: en çok 8 etiket; aralık kısaysa saat, uzunsa ay.yıl yazılır.
        var aralikGun = (t1 - t0) / 86400000;
        var etiketSay = 8;
        for (var i = 0; i <= etiketSay; i++) {
            var t = new Date(t0 + (t1 - t0) * i / etiketSay);
            var metin = aralikGun <= 3 ? iki(t.getDate()) + '.' + iki(t.getMonth() + 1) + ' ' + saatMetin(t)
                : aralikGun <= 400 ? gunMetin(t) : iki(t.getMonth() + 1) + '.' + t.getFullYear();
            svgEl('line', { x1: xK(t.getTime()), x2: xK(t.getTime()), y1: ust + yuk, y2: ust + yuk + 4, 'class': 'eksen' }, svg);
            svgEl('text', { x: xK(t.getTime()), y: Y - alt + 20, 'text-anchor': i === 0 ? 'start' : i === etiketSay ? 'end' : 'middle' }, svg).textContent = metin;
        }

        var nokta = $.map(veri, function (n) { return xK(n.t.getTime()).toFixed(1) + ',' + yK(n.adet).toFixed(1); });
        var renk = 'var(--tema-ana, #3d8fc4)'; // stil olarak verilir (SVG özniteliği CSS değişkeni almaz)
        if (veri.length > 1) svgEl('polyline', { points: nokta.join(' '), fill: 'none', style: 'stroke:' + renk, 'class': 'cizgi ck-cizgi' }, svg);
        if (veri.length <= 60 || veri.length === 1)
            $.each(veri, function (j, n) { svgEl('circle', { cx: xK(n.t.getTime()), cy: yK(n.adet), r: 4, style: 'fill:' + renk, 'class': 'nokta' }, svg); });

        // Fareyle en yakın noktanın değeri gösterilir (çok noktada da okunur).
        var isaret = svgEl('circle', { r: 5, style: 'fill:' + renk, 'class': 'nokta', visibility: 'hidden' }, svg);
        var $ipucu = $('<div class="ck-ipucu" hidden></div>');
        var alan = svgEl('rect', { x: sol, y: ust, width: ic, height: yuk, fill: 'transparent' }, svg);
        $(alan).on('mousemove', function (e) {
            var r = svg.getBoundingClientRect();
            var x = (e.clientX - r.left) / r.width * G;
            var t = t0 + (x - sol) / ic * (t1 - t0);
            var lo = 0, hi = veri.length - 1;
            while (hi - lo > 1) { var m = (lo + hi) >> 1; if (veri[m].t.getTime() < t) lo = m; else hi = m; }
            var n = Math.abs(veri[lo].t.getTime() - t) <= Math.abs(veri[hi].t.getTime() - t) ? veri[lo] : veri[hi];
            isaret.setAttribute('cx', xK(n.t.getTime()));
            isaret.setAttribute('cy', yK(n.adet));
            isaret.setAttribute('visibility', 'visible');
            var kr = $k[0].getBoundingClientRect();
            $ipucu.text(noktaMetin(n.t, z) + ': ' + sayi.format(n.adet)).prop('hidden', false)
                .css({ left: (r.left - kr.left + xK(n.t.getTime()) / G * r.width) + 'px', top: (r.top - kr.top + yK(n.adet) / Y * r.height - 34) + 'px' });
        }).on('mouseleave', function () {
            isaret.setAttribute('visibility', 'hidden');
            $ipucu.prop('hidden', true);
        });

        $f.append(svg);
        $k.append($ipucu);
    }

    function ciz() {
        var t1 = $('#txtTarih1').val(), t2 = $('#txtTarih2').val();
        if (!t1 || !t2) { hataYaz('Tarih aralığını giriniz.'); return; }
        if (t1 > t2) { hataYaz('Başlangıç tarihi bitiş tarihinden sonra olamaz.'); return; }
        var z = parseInt($('input[name=zaman]:checked').val(), 10) || 3;
        var $b = $('#btnCiz').prop('disabled', true);
        hataYaz('');
        $('#lblToplam').text('Okunuyor...');
        Sgb.al(UC + 'grafik', { sayfa: $('#txtSayfa').val() || '', tarih1: t1, tarih2: t2, zaman: z })
            .done(function (v) {
                if (v.hata) { hataYaz(v.hata); $('#lblToplam').text(''); return; }
                $('#lblToplam').text('Toplam görüntülenme: ' + sayi.format(v.toplam || 0));
                var son = new Date(t2 + 'T00:00'); son.setDate(son.getDate() + 1);
                grafikCiz('#grafik', v.noktalar || [], z, new Date(t1 + 'T00:00'), son);
            })
            .fail(function (xhr) { $('#lblToplam').text(''); istekHatasi(xhr); })
            .always(function () { $b.prop('disabled', false); });
    }
    $('#frmKriter').on('submit', ciz);

    // Eski sayfadaki gibi açılışta bu ayın grafiği çizilir.
    aralikVer(0);
    ciz();
});
