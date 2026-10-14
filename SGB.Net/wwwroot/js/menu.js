// Sayfaların ortak çerçevesi: üst şerit (yazılım ikonu, bulunulan sayfa, kullanıcı), sol menü ve alt bilgi.
//
// Menü verisi (menü, kullanıcı, tema, kurum) açılış sayfasında sunucudan okunur ve tarayıcıda saklanır
// (localStorage, sekmeler arasında ortak). Diğer sayfalar sunucuya sormadan saklanan veriden çizer.
// Saklanan veri yalnızca görüntü içindir; bir sayfaya erişim yetkisi her istekte sunucuda denetlenir.
//
// Sol menünün üç görünümü vardır (html[data-menu]):
//   acik  : geniş ve sabit; içerik yana kayar. Menüden bir ekran seçilince ikon görünümüne döner.
//   ikon  : dar şerit, yalnızca ikonlar. Fare üzerine gelince içeriğin üstüne doğru açılır, ayrılınca daralır.
//   gizli : hiç yer kaplamaz; üst şeritteki menü düğmesiyle geri gelir.
(function () {
    'use strict';

    var ANAHTAR = 'sgb.menu';
    var KIP_ANAHTARI = 'sgb.menuKipi';
    var GRUP_ANAHTARI = 'sgb.aktifGrup';
    var KOK = '../';    // bütün sayfalar uygulama kökünün bir alt klasöründedir (OrtakSayfa, TasinirMal ...)

    var kokEl = document.documentElement;
    var ust = document.getElementById('menuUst');
    var yan = document.getElementById('menuYan');
    var alt = document.getElementById('menuAlt');
    if (!ust || !yan) return;

    function meta(ad) {
        var m = document.querySelector('meta[name="' + ad + '"]');
        return m ? m.content : '';
    }

    var jeton = meta('istek-jetonu');
    var kullanici = meta('kullanici');
    var darEkran = window.matchMedia('(max-width: 767px)');

    function el(etiket, sinif, metin) {
        var e = document.createElement(etiket);
        if (sinif) e.className = sinif;
        if (metin !== undefined && metin !== null) e.textContent = metin;
        return e;
    }

    function svg(yolVerisi, sinif) {
        var s = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
        s.setAttribute('viewBox', '0 0 24 24');
        s.setAttribute('aria-hidden', 'true');
        if (sinif) s.setAttribute('class', sinif);
        var p = document.createElementNS('http://www.w3.org/2000/svg', 'path');
        p.setAttribute('d', yolVerisi);
        s.appendChild(p);
        return s;
    }

    var CIZIM = {
        menu: 'M4 7h16M4 12h16M4 17h16',
        ara: 'M11 4a7 7 0 1 0 0 14a7 7 0 0 0 0-14zM21 21l-4.3-4.3',
        ok: 'M9 6l6 6-6 6',
        sabitle: 'M9 4h6l-1 6 3 3H7l3-3-1-6zM12 13v7'
    };

    // ---------------------------------------------------------------- saklama

    function depoOku(anahtar) {
        try { return localStorage.getItem(anahtar); } catch (e) { return null; }
    }

    function depoYaz(anahtar, deger) {
        try { localStorage.setItem(anahtar, deger); } catch (e) { /* depo kapalıysa tercih hatırlanmaz */ }
    }

    function oku() {
        try {
            var veri = JSON.parse(depoOku(ANAHTAR) || 'null');
            // Başka bir kullanıcıdan kalan veri kullanılmaz.
            return veri && veri.kullaniciKodu === kullanici ? veri : null;
        } catch (e) {
            return null;
        }
    }

    function sunucudanAl() {
        return fetch(KOK + 'api/ortaksayfa/menu', { credentials: 'same-origin' })
            .then(function (yanit) {
                if (yanit.status === 401) {
                    window.location.href = KOK + 'Giris';
                    return null;
                }
                if (!yanit.ok) throw new Error('HTTP ' + yanit.status);
                return yanit.json();
            })
            .then(function (veri) {
                if (veri) depoYaz(ANAHTAR, JSON.stringify(veri));
                return veri;
            });
    }

    // ---------------------------------------------------------------- yardımcılar

    // Adresi karşılaştırma için sadeleştirir: /modul/sayfa (küçük harf, .aspx'siz)
    function yol(adres) {
        try {
            return new URL(adres, window.location.href).pathname.toLowerCase().replace(/\.aspx$/, '');
        } catch (e) {
            return '';
        }
    }

    var buSayfa = yol(window.location.href);

    function baglantilar(menu) {
        var liste = [];
        (menu.kolonlar || []).forEach(function (kolon) {
            kolon.forEach(function (satir) { if (satir.t === 'a') liste.push(satir); });
        });
        return liste;
    }

    function sayfayiIceriyor(menu) {
        if (menu.url && yol(menu.url) === buSayfa) return true;
        return baglantilar(menu).some(function (satir) { return yol(satir.url) === buSayfa; });
    }

    // Açık sayfanın menüdeki yeri. Menüde yoksa null döner (başlık olarak sayfanın kendi adı kullanılır).
    function sayfaKonumu(gruplar) {
        for (var g = 0; g < gruplar.length; g++) {
            var menuler = gruplar[g].menuler;
            for (var m = 0; m < menuler.length; m++) {
                if (menuler[m].url && yol(menuler[m].url) === buSayfa)
                    return { yer: gruplar[g].ad, ad: menuler[m].ad };
                var satirlar = baglantilar(menuler[m]);
                for (var i = 0; i < satirlar.length; i++)
                    if (yol(satirlar[i].url) === buSayfa)
                        return { yer: gruplar[g].ad + ' › ' + menuler[m].ad, ad: satirlar[i].ad };
            }
        }
        return null;
    }

    // Aramada karşılaştırma için: küçük harf; bölünmez boşluk (&nbsp;, menü adlarında sık geçer) ve sekme gibi
    // bütün boşluk karakterleri düz boşluk olur. Uzunluk değişmez, böylece bulunan yer adın içinde işaretlenebilir.
    function kucuk(metin) {
        return (metin || '').toLocaleLowerCase('tr').replace(/\s/g, ' ');
    }

    // İkonu olmayan menü için iki harf: tek kelimeyse ilk iki harfi, birden çok kelimeyse ilk iki kelimenin baş harfleri.
    function kisaAd(ad) {
        var kelimeler = (ad || '').split(/[\s\/\-.()]+/).filter(function (k) { return /^[0-9A-Za-zÀ-ɏЀ-ӿ]/.test(k); });
        if (kelimeler.length === 0) return '?';
        if (kelimeler.length === 1)
            return kelimeler[0].charAt(0).toLocaleUpperCase('tr') + kelimeler[0].charAt(1).toLocaleLowerCase('tr');
        return (kelimeler[0].charAt(0) + kelimeler[1].charAt(0)).toLocaleUpperCase('tr');
    }

    // Menü ikonu: menu.xml'de Ikon niteliği varsa wwwroot/img/menu altındaki dosya, yoksa iki harf.
    function ikonOlustur(menu) {
        var kutu = el('span', 'y-ikon');
        if (menu.ikon) {
            var resim = el('img');
            resim.src = KOK + 'img/menu/' + menu.ikon;
            resim.alt = '';
            resim.addEventListener('error', function () { kutu.textContent = kisaAd(menu.ad); });
            kutu.appendChild(resim);
        } else {
            kutu.textContent = kisaAd(menu.ad);
        }
        return kutu;
    }

    // ---------------------------------------------------------------- menü görünümü (kip)

    function kip() {
        return kokEl.getAttribute('data-menu') || 'ikon';
    }

    var sabitleDugme = null;
    var aramaKutusu = null;
    var fareIcinde = false;
    var zamanlayici = null;

    function bekle(islev, sure) {
        clearTimeout(zamanlayici);
        zamanlayici = setTimeout(islev, sure);
    }

    function genislet() {
        clearTimeout(zamanlayici);
        yan.classList.add('genis');
    }

    function daralt() {
        clearTimeout(zamanlayici);
        if (kip() === 'acik' && !darEkran.matches) return;
        if (aramaKutusu && document.activeElement === aramaKutusu) return;
        acilirKapat();
        yan.classList.remove('genis');
    }

    function kipYaz(yeni) {
        kokEl.setAttribute('data-menu', yeni);
        depoYaz(KIP_ANAHTARI, yeni);
        if (yeni === 'acik') yan.classList.add('genis');
        if (yeni === 'gizli') yan.classList.remove('genis');
        if (sabitleDugme) {
            sabitleDugme.classList.toggle('secili', yeni === 'acik');
            sabitleDugme.setAttribute('aria-pressed', yeni === 'acik' ? 'true' : 'false');
        }
    }

    // Fare menünün üzerine gelince açılır, ayrılınca daralır.
    yan.addEventListener('mouseenter', function () {
        fareIcinde = true;
        if (kip() === 'ikon') bekle(genislet, 90);
    });
    yan.addEventListener('mouseleave', function () {
        fareIcinde = false;
        bekle(daralt, 280);
    });
    // Klavyeyle gezinirken de açılır.
    yan.addEventListener('focusin', genislet);
    yan.addEventListener('focusout', function () {
        bekle(function () { if (!yan.contains(document.activeElement) && !fareIcinde) daralt(); }, 150);
    });

    // Menüden bir ekran seçildi: sabit açık görünüm ikon görünümüne döner (gidilen sayfa dar menüyle açılır).
    function ekranSecildi() {
        if (kip() === 'acik') depoYaz(KIP_ANAHTARI, 'ikon');
    }

    // ---------------------------------------------------------------- sol menü

    function satirCiz(kap, satir) {
        if (satir.t === 'b') {
            kap.appendChild(el('div', 'y-bolum', satir.ad));
        } else if (satir.t === 'c') {
            kap.appendChild(el('hr', 'y-ayrac'));
        } else {
            var a = el('a', 'y-baglanti', satir.ad);
            a.href = satir.url;
            if (satir.yeni) {
                a.target = '_blank';
                a.rel = 'noopener';
            }
            if (yol(satir.url) === buSayfa) a.classList.add('burada');
            a.addEventListener('click', ekranSecildi);
            kap.appendChild(a);
        }
    }

    // Menünün paneli: sol menünün sağında, tıklanan başlığın hizasında açılır; kolonlar yan yana dizilir.
    var acikPanel = null;

    function acilirAc(panel, dugme) {
        acilirKapat();
        panel.hidden = false;
        var hiza = dugme.getBoundingClientRect().top;
        var yukseklik = panel.offsetHeight;
        var sinir = window.innerHeight - 12;
        var tavan = ust.getBoundingClientRect().bottom + 8;     // üst şeridin altında kalır
        panel.style.top = Math.max(tavan, Math.min(hiza, sinir - yukseklik)) + 'px';
        dugme.setAttribute('aria-expanded', 'true');
        acikPanel = { panel: panel, dugme: dugme };
    }

    function acilirKapat() {
        if (!acikPanel) return;
        acikPanel.panel.hidden = true;
        acikPanel.dugme.setAttribute('aria-expanded', 'false');
        acikPanel = null;
    }

    // Bütün modüller (menü grupları) alt alta: modülün adı renkli bir başlık, altında birinci seviye menüler.
    // Aynı anda tek modül açıktır: açık sayfanın modülü, yoksa en son açılan, o da yoksa ilki.
    // Başka bir modülün başlığına basınca o açılır, diğeri kapanır.
    function menuAgaciCiz(kap, gruplar) {
        kap.textContent = '';
        acilirKapat();
        var eskiler = yan.querySelectorAll('.y-acilir');
        for (var i = 0; i < eskiler.length; i++) eskiler[i].parentNode.removeChild(eskiler[i]);

        if (gruplar.length === 1) {
            gruplar[0].menuler.forEach(function (menu) { menuCiz(kap, menu); });
            return;
        }

        var aktif = gruplar.filter(function (g) { return g.menuler.some(sayfayiIceriyor); })[0]
            || gruplar.filter(function (g) { return g.id === depoOku(GRUP_ANAHTARI); })[0]
            || gruplar[0];
        var bolumler = [];

        gruplar.forEach(function (grup, sira) {
            var acik = grup === aktif;
            // Her modülün kendi rengi vardır (4 renk sırayla tekrarlanır): başlık ve altındaki menü ikonları.
            var renk = ' y-renk-' + (sira % 4);
            var baslik = el('button', 'y-grup-baslik' + renk);
            baslik.type = 'button';
            baslik.title = grup.ad;
            baslik.setAttribute('aria-expanded', acik ? 'true' : 'false');
            baslik.appendChild(el('span', null, grup.ad));
            baslik.appendChild(svg(CIZIM.ok, 'y-grup-ok'));
            var icerik = el('div', 'y-grup-icerik' + renk);
            icerik.hidden = !acik;
            grup.menuler.forEach(function (menu) { menuCiz(icerik, menu); });
            bolumler.push({ baslik: baslik, icerik: icerik });

            baslik.addEventListener('click', function () {
                acilirKapat();
                var ac = icerik.hidden;
                bolumler.forEach(function (b) {
                    var bu = b.icerik === icerik && ac;
                    b.icerik.hidden = !bu;
                    b.baslik.setAttribute('aria-expanded', bu ? 'true' : 'false');
                });
                if (ac) depoYaz(GRUP_ANAHTARI, grup.id);
            });

            kap.appendChild(baslik);
            kap.appendChild(icerik);
        });
    }

    function menuCiz(kap, menu) {
        var burada = sayfayiIceriyor(menu);

        if (menu.url) {
            var dogrudan = el('a', 'y-menu' + (burada ? ' burada' : ''));
            dogrudan.href = menu.url;
            dogrudan.title = menu.ad;
            dogrudan.appendChild(ikonOlustur(menu));
            dogrudan.appendChild(el('span', 'y-ad', menu.ad));
            dogrudan.addEventListener('click', ekranSecildi);
            kap.appendChild(dogrudan);
            return;
        }

        var satirlar = baglantilar(menu);
        var dugme = el('button', 'y-menu' + (burada ? ' burada' : ''));
        dugme.type = 'button';
        dugme.title = menu.ad;
        dugme.setAttribute('aria-expanded', 'false');
        dugme.appendChild(ikonOlustur(menu));
        dugme.appendChild(el('span', 'y-ad', menu.ad));
        dugme.appendChild(el('span', 'y-sayi', satirlar.length));
        dugme.appendChild(svg(CIZIM.ok, 'y-ok'));

        // Seçenekler sağda açılan panelde (kolonlar yan yana, eski menüdeki gibi) gösterilir;
        // dar ekranda (telefon) panel sığmadığından menünün içinde alt alta açılır.
        var kolonlu = (menu.kolonlar || []).length > 1;
        var liste = el('div', 'y-liste');
        liste.hidden = !burada || !darEkran.matches;
        (menu.kolonlar || []).forEach(function (kolon) {
            kolon.forEach(function (satir) { satirCiz(liste, satir); });
        });

        var acilir = el('div', 'y-acilir');
        acilir.hidden = true;
        acilir.setAttribute('role', 'group');
        acilir.setAttribute('aria-label', menu.ad);
        acilir.appendChild(el('div', 'y-acilir-baslik', menu.ad));
        var kolonlar = el('div', 'y-kolonlar');
        menu.kolonlar.forEach(function (kolon) {
            var k = el('div', 'y-kolon');
            kolon.forEach(function (satir, sira) {
                if (sira === 0 && satir.t === 'b') k.appendChild(el('div', 'y-kolon-baslik', satir.ad));
                else satirCiz(k, satir);
            });
            kolonlar.appendChild(k);
        });
        acilir.appendChild(kolonlar);
        yan.appendChild(acilir);

        dugme.addEventListener('click', function (olay) {
            // Dar görünümde (ör. dokunmatik ekran) ikona basmak önce menüyü açar.
            var dardi = !yan.classList.contains('genis');
            if (dardi) genislet();
            if (!darEkran.matches) {
                liste.hidden = true;
                if (acikPanel && acikPanel.panel === acilir) acilirKapat();
                else {
                    acilirAc(acilir, dugme);
                    // Klavyeyle açıldıysa ilk seçeneğe geçilir.
                    if (olay.detail === 0) {
                        var ilk = acilir.querySelector('a');
                        if (ilk) ilk.focus();
                    }
                }
                return;
            }
            var ac = dardi ? true : liste.hidden;
            liste.hidden = !ac;
            dugme.setAttribute('aria-expanded', ac ? 'true' : 'false');
        });

        kap.appendChild(dugme);
        kap.appendChild(liste);
    }

    function aramaKur(veri, girdi, agac, sonuclar, metin) {
        // Bütün gruplardaki ekranlar tek listede aranır.
        var hepsi = [];
        (veri.gruplar || []).forEach(function (grup) {
            grup.menuler.forEach(function (menu) {
                if (menu.url) hepsi.push({ ad: menu.ad, url: menu.url, yer: grup.ad });
                baglantilar(menu).forEach(function (satir) {
                    hepsi.push({ ad: satir.ad, url: satir.url, yeni: satir.yeni, yer: grup.ad + ' › ' + menu.ad });
                });
            });
        });

        var secili = 0;

        function sec(sira) {
            var ogeler = sonuclar.querySelectorAll('.y-sonuc');
            if (!ogeler.length) return;
            secili = (sira + ogeler.length) % ogeler.length;
            for (var i = 0; i < ogeler.length; i++) ogeler[i].classList.toggle('secili', i === secili);
            ogeler[secili].scrollIntoView({ block: 'nearest' });
        }

        function ara() {
            var aranan = kucuk(girdi.value.trim()).replace(/ +/g, ' ');
            sonuclar.textContent = '';
            if (aranan.length < 2) {
                sonuclar.hidden = true;
                agac.hidden = false;
                return;
            }
            agac.hidden = true;
            sonuclar.hidden = false;

            var bulunan = hepsi.filter(function (x) { return kucuk(x.ad).indexOf(aranan) >= 0; }).slice(0, 40);
            if (!bulunan.length) {
                sonuclar.appendChild(el('div', 'y-bos', metin.sonucYok || 'Eşleşen ekran yok.'));
                return;
            }
            bulunan.forEach(function (x) {
                var a = el('a', 'y-sonuc');
                a.href = x.url;
                if (x.yeni) {
                    a.target = '_blank';
                    a.rel = 'noopener';
                }
                var konum = kucuk(x.ad).indexOf(aranan);
                a.appendChild(document.createTextNode(x.ad.slice(0, konum)));
                a.appendChild(el('mark', null, x.ad.slice(konum, konum + aranan.length)));
                a.appendChild(document.createTextNode(x.ad.slice(konum + aranan.length)));
                a.appendChild(el('small', null, x.yer));
                a.addEventListener('click', ekranSecildi);
                sonuclar.appendChild(a);
            });
            sec(0);
        }

        girdi.addEventListener('input', ara);
        girdi.addEventListener('keydown', function (olay) {
            if (olay.key === 'ArrowDown') { olay.preventDefault(); sec(secili + 1); }
            else if (olay.key === 'ArrowUp') { olay.preventDefault(); sec(secili - 1); }
            else if (olay.key === 'Enter') {
                var hedef = sonuclar.querySelectorAll('.y-sonuc')[secili];
                if (hedef && !sonuclar.hidden) { olay.preventDefault(); hedef.click(); }
            } else if (olay.key === 'Escape') {
                girdi.value = '';
                ara();
                girdi.blur();
                if (!fareIcinde) daralt();
            }
        });
    }

    // ---------------------------------------------------------------- kullanıcı işlemleri

    function gonder(adres, govde) {
        var secenek = {
            method: 'POST',
            credentials: 'same-origin',
            headers: { 'RequestVerificationToken': jeton }
        };
        if (govde) {
            secenek.headers['Content-Type'] = 'application/json';
            secenek.body = JSON.stringify(govde);
        }
        return fetch(KOK + adres, secenek);
    }

    function temaUygula(veri, tema) {
        kokEl.setAttribute('data-tema', tema);
        veri.tema = tema;
        depoYaz(ANAHTAR, JSON.stringify(veri));
        var dugmeler = ust.querySelectorAll('.k-tema');
        for (var i = 0; i < dugmeler.length; i++)
            dugmeler[i].classList.toggle('secili', dugmeler[i].getAttribute('data-tema') === tema);
    }

    function temaDegistir(veri, tema) {
        var onceki = veri.tema;
        temaUygula(veri, tema);
        gonder('api/ortaksayfa/tema', { tema: tema })
            .then(function (yanit) { return yanit.ok ? yanit.json() : { basarili: false }; })
            .catch(function () { return { basarili: false }; })
            .then(function (sonuc) {
                // Kaydedilemediyse eski temaya dönülür.
                if (!sonuc.basarili) temaUygula(veri, onceki);
            });
    }

    function dilDegistir(dil) {
        gonder('api/ortaksayfa/dil', { dil: dil })
            .then(function (yanit) {
                if (!yanit.ok) return;
                // Menü ve sayfa yeni dilde yeniden okunur.
                try { localStorage.removeItem(ANAHTAR); } catch (e) { }
                window.location.reload();
            })
            .catch(function () { });
    }

    function cikis() {
        gonder('api/giris/cikis')
            .catch(function () { })
            .then(function () {
                try { localStorage.removeItem(ANAHTAR); } catch (e) { }
                window.location.href = KOK + 'Giris';
            });
    }

    // ---------------------------------------------------------------- çizim

    function ciz(veri) {
        var metin = veri.metin || {};
        kokEl.setAttribute('data-tema', veri.tema || 'Theme1');

        var gruplar = veri.gruplar || [];

        // ---------- sol menü
        acilirKapat();
        yan.textContent = '';
        var panel = el('div', 'y-panel');
        var ic = el('div', 'y-ic');

        var yanUst = el('div', 'y-ust');
        var araKutu = el('div', 'y-ara');
        araKutu.appendChild(svg(CIZIM.ara));
        var girdi = el('input');
        girdi.type = 'search';
        girdi.placeholder = (metin.ekranAra || 'Ekran ara') + '  (Ctrl+K)';
        girdi.setAttribute('aria-label', metin.ekranAra || 'Ekran ara');
        girdi.autocomplete = 'off';
        araKutu.appendChild(girdi);
        aramaKutusu = girdi;
        yanUst.appendChild(araKutu);

        sabitleDugme = el('button', 'k-dugme-kare y-sabitle');
        sabitleDugme.type = 'button';
        sabitleDugme.title = metin.menuSabitle || 'Menüyü açık tut';
        sabitleDugme.setAttribute('aria-label', sabitleDugme.title);
        sabitleDugme.appendChild(svg(CIZIM.sabitle));
        sabitleDugme.addEventListener('click', function () {
            if (kip() === 'acik') {
                kipYaz('ikon');
                genislet();     // fare hâlâ menünün üzerinde; ayrılınca daralır
            } else {
                kipYaz('acik');
            }
        });
        yanUst.appendChild(sabitleDugme);
        ic.appendChild(yanUst);

        var govde = el('nav', 'y-govde');
        govde.setAttribute('aria-label', metin.menu || 'Menü');
        var agac = el('div', 'y-agac');
        var sonuclar = el('div', 'y-sonuclar');
        sonuclar.hidden = true;
        if (veri.menuHatasi) agac.appendChild(el('div', 'y-bos', veri.menuHatasi));
        else menuAgaciCiz(agac, gruplar);

        govde.appendChild(agac);
        govde.appendChild(sonuclar);
        ic.appendChild(govde);

        panel.appendChild(ic);
        yan.appendChild(panel);
        aramaKur(veri, girdi, agac, sonuclar, metin);
        kipYaz(kip());

        // ---------- üst şerit
        ust.textContent = '';

        var menuDugme = el('button', 'k-dugme-kare');
        menuDugme.type = 'button';
        menuDugme.title = metin.menuGosterGizle || 'Menüyü göster / gizle';
        menuDugme.setAttribute('aria-label', menuDugme.title);
        menuDugme.appendChild(svg(CIZIM.menu));
        menuDugme.addEventListener('click', function (olay) {
            olay.stopPropagation();
            if (darEkran.matches) {
                // Dar ekranda menü içeriğin üstünde açılıp kapanır.
                if (yan.classList.contains('genis')) yan.classList.remove('genis'); else genislet();
            } else {
                kipYaz(kip() === 'gizli' ? 'ikon' : 'gizli');
            }
        });
        ust.appendChild(menuDugme);

        var marka = el('a', 'k-marka');
        marka.href = KOK + 'OrtakSayfa/GirisBilgi';
        marka.title = veri.kurumAdi || veri.yazilimAd;
        if (veri.logo) {
            var logo = el('img');
            logo.src = KOK + veri.logo;
            logo.alt = '';
            marka.appendChild(logo);
        }
        marka.appendChild(el('span', 'k-yazilim', veri.yazilimAd));
        ust.appendChild(marka);

        // Bulunulan sayfa: menüdeki yeri (grup › menü) ve sayfanın adı.
        var konum = sayfaKonumu(gruplar);
        var sayfa = el('div', 'k-sayfa');
        if (konum && konum.yer) sayfa.appendChild(el('div', 'k-sayfa-yer', konum.yer));
        sayfa.appendChild(el('div', 'k-sayfa-ad', (konum && konum.ad) || document.title));
        ust.appendChild(sayfa);
        ust.appendChild(el('div', 'k-bosluk'));

        var kul = el('div', 'k-kullanici');
        var kulDugme = el('button', 'k-kullanici-dugme');
        kulDugme.type = 'button';
        kulDugme.setAttribute('aria-expanded', 'false');
        var adSoyad = veri.adSoyad || veri.kullaniciKodu || '';
        kulDugme.appendChild(el('span', 'k-avatar', kisaAd(adSoyad).toLocaleUpperCase('tr')));
        kulDugme.appendChild(el('span', 'k-kullanici-ad', adSoyad));
        kulDugme.addEventListener('click', function (olay) {
            olay.stopPropagation();
            var acik = kul.classList.toggle('acik');
            kulDugme.setAttribute('aria-expanded', acik ? 'true' : 'false');
        });
        kul.appendChild(kulDugme);

        var kulPanel = el('div', 'k-kullanici-panel');
        kulPanel.addEventListener('click', function (olay) { olay.stopPropagation(); });

        var temaBolum = el('div', 'k-panel-bolum');
        temaBolum.appendChild(el('div', 'k-panel-baslik', metin.renkTemasi || 'Renk teması'));
        var temalar = el('div', 'k-temalar');
        (veri.temalar || []).forEach(function (tema, sira) {
            var t = el('button', 'k-tema');
            t.type = 'button';
            t.setAttribute('data-tema', tema);
            t.title = (metin.tema || 'Tema') + ' ' + (sira + 1);
            t.setAttribute('aria-label', t.title);
            if (tema === veri.tema) t.classList.add('secili');
            t.addEventListener('click', function () { temaDegistir(veri, tema); });
            temalar.appendChild(t);
        });
        temaBolum.appendChild(temalar);
        kulPanel.appendChild(temaBolum);

        if ((veri.diller || []).length > 1) {
            var dilBolum = el('div', 'k-panel-bolum');
            dilBolum.appendChild(el('div', 'k-panel-baslik', metin.dil || 'Dil'));
            var diller = el('div', 'k-diller');
            veri.diller.forEach(function (d) {
                var b = el('button', 'k-dil', d.ad);
                b.type = 'button';
                b.lang = d.kod;
                if (d.kod === veri.dil) {
                    b.classList.add('secili');
                    b.setAttribute('aria-pressed', 'true');
                } else {
                    b.addEventListener('click', function () { dilDegistir(d.kod); });
                }
                diller.appendChild(b);
            });
            dilBolum.appendChild(diller);
            kulPanel.appendChild(dilBolum);
        }

        var sifre = el('a', 'k-panel-satir', metin.sifreDegistir || 'Şifre Değiştir');
        sifre.href = KOK + 'KYM/ParolaDegistir';
        kulPanel.appendChild(sifre);

        var cikisDugme = el('button', 'k-panel-satir', metin.cikis || 'Çıkış');
        cikisDugme.type = 'button';
        cikisDugme.addEventListener('click', cikis);
        kulPanel.appendChild(cikisDugme);

        kul.appendChild(kulPanel);
        ust.appendChild(kul);

        // ---------- alt bilgi
        if (alt) {
            alt.textContent = '';
            alt.appendChild(el('span', null, veri.yazilimAd));
            if (veri.surum) alt.appendChild(el('span', null, (metin.surum || 'Sürüm') + ' ' + veri.surum));
            if (veri.kurumAdi) alt.appendChild(el('span', null, veri.kurumAdi));
        }
    }

    // Dışarı tıklamak açık panelleri kapatır ve menüyü daraltır.
    document.addEventListener('click', function (olay) {
        var kul = ust.querySelector('.k-kullanici.acik');
        if (kul) {
            kul.classList.remove('acik');
            kul.querySelector('[aria-expanded]').setAttribute('aria-expanded', 'false');
        }
        if (!yan.contains(olay.target)) {
            acilirKapat();
            daralt();
        }
    });

    document.addEventListener('keydown', function (olay) {
        if (olay.key === 'Escape' && acikPanel) {
            var dugme = acikPanel.dugme;
            acilirKapat();
            dugme.focus();
        }
        // Ctrl+K: menüyü açar ve ekran aramayı odaklar.
        if ((olay.ctrlKey || olay.metaKey) && (olay.key === 'k' || olay.key === 'K') && aramaKutusu) {
            olay.preventDefault();
            genislet();
            aramaKutusu.focus();
            aramaKutusu.select();
        }
    });

    // ---------------------------------------------------------------- başlangıç

    var yenile = document.body.getAttribute('data-menu-yenile') === '1';
    var saklanan = oku();
    if (saklanan) ciz(saklanan);

    // Açılış sayfasında her zaman, diğer sayfalarda yalnızca saklanan veri yoksa sunucudan okunur.
    if (yenile || !saklanan) {
        sunucudanAl()
            .then(function (veri) { if (veri) ciz(veri); })
            .catch(function () {
                if (!saklanan) ust.appendChild(el('div', 'alert alert-danger', 'Menü yüklenemedi. Lütfen sayfayı yenileyiniz.'));
            });
    }
})();
