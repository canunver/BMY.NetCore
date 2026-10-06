// Açılış sayfası (OrtakSayfa/GirisBilgi): bilgi ekranlarını sunucudan alır ve yerleştirir.
// Her bilgi ekranı kendi çerçevesinde (iframe) açılır; biri hata verirse diğerlerini etkilemez.
(function () {
    'use strict';

    var jeton = document.querySelector('meta[name="istek-jetonu"]').content;
    var kutuHata = document.getElementById('hata');
    var kutuUyari = document.getElementById('uyari');

    function goster(kutu, metin) {
        kutu.textContent = metin || '';
        kutu.hidden = !metin;
    }

    function ekranOlustur(ekran) {
        var panel = document.createElement('div');
        panel.className = 'panel panel-default bilgi-ekran';
        panel.id = ekran.id;

        var baslik = document.createElement('div');
        baslik.className = 'panel-heading';
        baslik.textContent = ekran.baslik;

        var govde = document.createElement('div');
        govde.className = 'panel-body';

        var cerceve = document.createElement('iframe');
        cerceve.title = ekran.baslik;
        cerceve.style.height = ekran.yukseklik + 'px';
        cerceve.src = ekran.adres;
        cerceve.setAttribute('loading', 'lazy');

        govde.appendChild(cerceve);
        panel.appendChild(baslik);
        panel.appendChild(govde);
        return panel;
    }

    function yerlestir(veri) {
        document.getElementById('lblKullanici').textContent = veri.adSoyad + ' (' + veri.kullaniciKodu + ')';
        goster(kutuUyari, veri.uyari);

        (veri.ekranlar || []).forEach(function (ekran) {
            var sutun = document.querySelector('#ekranlar [data-sutun="' + ekran.sutun + '"]');
            if (sutun) sutun.appendChild(ekranOlustur(ekran));
        });
    }

    fetch('../api/ortaksayfa/giris-bilgi', { credentials: 'same-origin' })
        .then(function (yanit) {
            if (yanit.status === 401) {
                window.location.href = '../Giris';
                return null;
            }
            if (!yanit.ok) throw new Error('HTTP ' + yanit.status);
            return yanit.json();
        })
        .then(function (veri) {
            if (veri) yerlestir(veri);
        })
        .catch(function () {
            goster(kutuHata, 'Açılış bilgileri alınamadı. Lütfen sayfayı yenileyiniz.');
        });

    document.getElementById('btnCikis').addEventListener('click', function () {
        fetch('../api/giris/cikis', {
            method: 'POST',
            credentials: 'same-origin',
            headers: { 'RequestVerificationToken': jeton }
        }).then(function () {
            window.location.href = '../Giris';
        });
    });
})();
