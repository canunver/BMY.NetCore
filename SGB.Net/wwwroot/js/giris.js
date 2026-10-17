// Giriş sayfası: bütün sunucu çağrıları buradan (fetch + JSON) yapılır.
// Adresler görelidir; uygulama IIS'te bir alt klasörde (sanal dizin) çalışsa da doğru yere gider.
(function () {
    'use strict';

    var jeton = document.querySelector('meta[name="istek-jetonu"]').content;
    // Sayfanın dilindeki metinler sunucudan gelir.
    var metin = {};
    try { metin = JSON.parse(document.getElementById('metinler').textContent) || {}; } catch (e) { }
    var frmParola = document.getElementById('frmParola');
    var frmKod = document.getElementById('frmKod');
    var txtKullanici = document.getElementById('txtKullanici');
    var txtParola = document.getElementById('txtParola');
    var txtKod = document.getElementById('txtKod');
    var chkHatirla = document.getElementById('chkHatirla');
    var kutuHata = document.getElementById('hata');
    var kutuBilgi = document.getElementById('bilgi');
    var buyukHarf = document.getElementById('buyukHarf');

    function goster(kutu, metin) {
        kutu.textContent = metin || '';
        kutu.hidden = !metin;
    }

    function mesajlariTemizle() {
        goster(kutuHata, '');
        goster(kutuBilgi, '');
    }

    function mesgul(form, durum) {
        var dugmeler = form.querySelectorAll('button');
        for (var i = 0; i < dugmeler.length; i++) dugmeler[i].disabled = durum;
    }

    // Sunucudaki hata metinleri <br> içerebiliyor; düz metne çevrilir.
    function duzMetin(html) {
        return (html || '').replace(/<br\s*\/?>/gi, '\n').replace(/<[^>]*>/g, '').trim();
    }

    function istek() {
        return {
            kullaniciKodu: txtKullanici.value.trim(),
            parola: txtParola.value,
            ikinciKod: txtKod.value.trim(),
            hatirla: chkHatirla.checked,
            donusAdresi: new URLSearchParams(window.location.search).get('ReturnUrl')
        };
    }

    function gonder(adres, form) {
        mesajlariTemizle();
        mesgul(form, true);
        return fetch(adres, {
            method: 'POST',
            credentials: 'same-origin',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': jeton },
            body: JSON.stringify(istek())
        })
            .then(function (yanit) {
                if (!yanit.ok) throw new Error('HTTP ' + yanit.status);
                return yanit.json();
            })
            .catch(function () {
                return { Basarili: false, Hata: metin.sunucuYok || 'Sunucuya ulaşılamadı. Lütfen tekrar deneyiniz.' };
            })
            .then(function (sonuc) {
                mesgul(form, false);
                return sonuc;
            });
    }

    function sonucuIsle(sonuc) {
        if (!sonuc.Basarili) {
            goster(kutuHata, duzMetin(sonuc.Hata));
            return;
        }
        if (sonuc.Yonlen) {
            window.location.href = sonuc.Yonlen;
            return;
        }
        if (sonuc.IkinciFaktorGerekli) {
            frmParola.hidden = true;
            frmKod.hidden = false;
            goster(kutuBilgi, sonuc.Bilgi);
            txtKod.value = '';
            txtKod.focus();
        }
    }

    frmParola.addEventListener('submit', function (olay) {
        olay.preventDefault();
        if (!txtKullanici.value.trim() || !txtParola.value) {
            goster(kutuHata, metin.kodParolaGir || 'Kullanıcı kodu ve parola giriniz.');
            return;
        }
        gonder('api/giris/parola', frmParola).then(sonucuIsle);
    });

    frmKod.addEventListener('submit', function (olay) {
        olay.preventDefault();
        if (!txtKod.value.trim()) {
            goster(kutuHata, metin.kodGir || 'Doğrulama kodunu giriniz.');
            return;
        }
        gonder('api/giris/tamamla', frmKod).then(sonucuIsle);
    });

    document.getElementById('btnKodYenile').addEventListener('click', function () {
        gonder('api/giris/kod-yenile', frmKod).then(sonucuIsle);
    });

    document.getElementById('btnGeri').addEventListener('click', function () {
        mesajlariTemizle();
        frmKod.hidden = true;
        frmParola.hidden = false;
        txtParola.value = '';
        txtParola.focus();
    });

    txtParola.addEventListener('keyup', function (olay) {
        if (olay.getModifierState) buyukHarf.hidden = !olay.getModifierState('CapsLock');
    });

    // Giriş sayfası açıldıysa önceki kullanıcıdan kalan menü verisi (menü vb.) silinir.
    try { localStorage.removeItem('sgb.menu'); } catch (e) { }

    (txtKullanici.value ? txtParola : txtKullanici).focus();
})();
