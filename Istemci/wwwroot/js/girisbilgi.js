// Açılış sayfası (OrtakSayfa/GirisBilgi): bilgi ekranlarını sunucudan alır ve yerleştirir.
// Her bilgi ekranı kendi çerçevesinde (iframe) açılır; biri hata verirse diğerlerini etkilemez.
// Üst başlık, menü ve çıkış menu.js tarafından çizilir.
(function () {
    'use strict';

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

    // Duyuru ekranı "pencerede göster" işaretli duyuru bulursa haber verir; bunlar oturumda bir kez pencerede gösterilir
    // (eski Duyuru.aspx'teki showPopWinTop).
    function duyuruPenceresiAc(baslik, kapat) {
        var anahtar = 'sgb.duyuruPenceresi';
        try {
            var kullanici = (document.querySelector('meta[name="kullanici"]') || {}).content || '';
            if (sessionStorage.getItem(anahtar) === kullanici) return;
            sessionStorage.setItem(anahtar, kullanici);
        } catch (e) { /* depo kapalıysa her açılışta gösterilir */ }
        if (document.getElementById('duyuruPenceresi')) return;

        var perde = document.createElement('div');
        perde.className = 'modal-backdrop fade in';

        var pencere = document.createElement('div');
        pencere.id = 'duyuruPenceresi';
        pencere.className = 'modal fade in duyuru-pencere';
        pencere.setAttribute('role', 'dialog');
        pencere.setAttribute('aria-modal', 'true');
        pencere.setAttribute('aria-labelledby', 'duyuruPenceresiBaslik');
        pencere.style.display = 'block';
        pencere.innerHTML =
            '<div class="modal-dialog modal-lg"><div class="modal-content">' +
            '<div class="modal-header"><button type="button" class="close" data-kapat aria-label=""><span aria-hidden="true">&times;</span></button>' +
            '<h4 class="modal-title" id="duyuruPenceresiBaslik"></h4></div>' +
            '<div class="modal-body"><iframe src="../Duyuru/Duyuru?penceremi=1&menuYok=1"></iframe></div>' +
            '<div class="modal-footer"><button type="button" class="btn btn-default" data-kapat></button></div>' +
            '</div></div>';
        pencere.querySelector('.modal-title').textContent = baslik || '';
        pencere.querySelector('.modal-footer .btn').textContent = kapat || '×';
        pencere.querySelector('.close').setAttribute('aria-label', kapat || '');
        pencere.querySelector('iframe').title = baslik || '';

        function kapa() {
            document.removeEventListener('keydown', tus);
            pencere.remove();
            perde.remove();
            document.body.classList.remove('modal-open');
        }
        function tus(e) { if (e.key === 'Escape') kapa(); }
        pencere.addEventListener('click', function (e) {
            if (e.target === pencere || e.target.closest('[data-kapat]')) kapa();
        });
        document.addEventListener('keydown', tus);

        document.body.classList.add('modal-open');
        document.body.appendChild(perde);
        document.body.appendChild(pencere);
        pencere.querySelector('.modal-footer .btn').focus();
    }

    window.addEventListener('message', function (e) {
        if (e.origin !== window.location.origin || !e.data || e.data.tur !== 'duyuruPenceresi') return;
        duyuruPenceresiAc(e.data.baslik, e.data.kapat);
    });

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
            if (!veri) return;
            goster(kutuUyari, veri.uyari);
            (veri.ekranlar || []).forEach(function (ekran) {
                var sutun = document.querySelector('#ekranlar [data-sutun="' + ekran.sutun + '"]');
                if (sutun) sutun.appendChild(ekranOlustur(ekran));
            });
        })
        .catch(function () {
            goster(kutuHata, 'Açılış bilgileri alınamadı. Lütfen sayfayı yenileyiniz.');
        });
})();
