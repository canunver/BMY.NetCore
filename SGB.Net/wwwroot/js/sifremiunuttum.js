// Şifremi Unuttum sayfası (eski OrtakSayfa/SifremiUnuttum.aspx). Sayfa OrtakSayfa altında; uç adresleri ona göre görelidir.
(function () {
    'use strict';

    var jeton = document.querySelector('meta[name="istek-jetonu"]').content;
    var metin = {};
    try { metin = JSON.parse(document.getElementById('metinler').textContent) || {}; } catch (e) { }
    var form = document.getElementById('frmUnuttum');
    if (!form) return; // LDAP kullanılıyor: sayfada yalnızca uyarı var.

    var txtBilgi = document.getElementById('txtBilgi');
    var txtGuvenlik = document.getElementById('txtGuvenlik');
    var imgGuvenlik = document.getElementById('imgGuvenlik');
    var btnGonder = document.getElementById('btnGonder');
    var kutuHata = document.getElementById('hata');
    var kutuBilgi = document.getElementById('bilgi');

    function goster(kutu, yazi) {
        kutu.textContent = yazi || '';
        kutu.hidden = !yazi;
    }

    // Güvenlik kodu bir kez kullanılır; her gönderimden sonra yeni resim alınır.
    function guvenlikYenile() {
        txtGuvenlik.value = '';
        imgGuvenlik.src = '../api/giris/guvenlik-resmi?v=' + Date.now();
    }

    document.getElementById('btnGuvenlikYenile').addEventListener('click', function () {
        guvenlikYenile();
        txtGuvenlik.focus();
    });

    form.addEventListener('submit', function (olay) {
        olay.preventDefault();
        goster(kutuHata, '');
        goster(kutuBilgi, '');

        if (!txtBilgi.value.trim()) {
            goster(kutuHata, metin.bilgiGir || 'Lütfen T.C. Kimlik Numarası veya E-Posta Adresi bilgisini girin.');
            txtBilgi.focus();
            return;
        }
        if (!txtGuvenlik.value.trim()) {
            goster(kutuHata, metin.guvenlikKoduGir || 'Resimdeki güvenlik kodunu giriniz.');
            txtGuvenlik.focus();
            return;
        }

        btnGonder.disabled = true;
        fetch('../api/giris/sifremi-unuttum', {
            method: 'POST',
            credentials: 'same-origin',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': jeton },
            body: JSON.stringify({ bilgi: txtBilgi.value.trim(), guvenlikKodu: txtGuvenlik.value.trim() })
        })
            .then(function (yanit) {
                if (!yanit.ok) throw new Error('HTTP ' + yanit.status);
                return yanit.json();
            })
            .catch(function () {
                return { Basarili: false, Hata: metin.sunucuYok || 'Sunucuya ulaşılamadı. Lütfen tekrar deneyiniz.' };
            })
            .then(function (sonuc) {
                btnGonder.disabled = false;
                guvenlikYenile();
                if (!sonuc.Basarili) {
                    goster(kutuHata, sonuc.Hata);
                    txtGuvenlik.focus();
                    return;
                }
                goster(kutuBilgi, sonuc.Bilgi);
                txtBilgi.value = '';
                form.hidden = true;
            });
    });

    guvenlikYenile();
    txtBilgi.focus();
})();
