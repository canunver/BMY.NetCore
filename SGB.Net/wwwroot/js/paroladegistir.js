// Parola Değiştir (KYM/ParolaDegistir): eski ParolaDegistir.aspx.
//   Yeni Parola yazılırken yanındaki çubuk parolanın gücünü gösterir (eski pwdmeter.js'in yerine).
//   Parola Değiştir (ya da Enter): /api/paroladegistir/degistir. Başarılıysa oturum kapanır ve giriş sayfasına gidilir
//   (eskisi Cikis.aspx'e yönlendiriyordu).
(function ($) {
    'use strict';

    var UC = 'api/paroladegistir/';

    // 0-100 arası parola gücü: uzunluk, büyük/küçük harf, rakam ve simge çeşitliliği artırır;
    // yalnızca harf ya da yalnızca rakam, tekrar eden karakterler ve ardışık diziler (abc, 123) düşürür.
    function guc(p) {
        if (!p) return 0;
        var n = p.length, buyuk = 0, kucuk = 0, rakam = 0, simge = 0, orta = 0, tekrar = 0;
        var gorulen = {};
        for (var i = 0; i < n; i++) {
            var c = p.charAt(i);
            if (/[A-ZÇĞİÖŞÜ]/.test(c)) buyuk++;
            else if (/[a-zçğıöşü]/.test(c)) kucuk++;
            else if (/[0-9]/.test(c)) { rakam++; if (i > 0 && i < n - 1) orta++; }
            else { simge++; if (i > 0 && i < n - 1) orta++; }
            if (gorulen[c]) tekrar++;
            gorulen[c] = true;
        }
        var puan = n * 4;
        if (buyuk > 0 && buyuk < n) puan += (n - buyuk) * 2;
        if (kucuk > 0 && kucuk < n) puan += (n - kucuk) * 2;
        if (rakam > 0 && rakam < n) puan += rakam * 4;
        puan += simge * 6 + orta * 2;
        if ((buyuk + kucuk) === n || rakam === n) puan -= n;
        puan -= tekrar * 2;

        var kucukHarf = p.toLowerCase(), dizi = 0;
        for (var j = 0; j < kucukHarf.length - 2; j++) {
            var a = kucukHarf.charCodeAt(j), b = kucukHarf.charCodeAt(j + 1), d = kucukHarf.charCodeAt(j + 2);
            if ((b - a === 1 && d - b === 1) || (a - b === 1 && b - d === 1)) dizi++;
        }
        puan -= dizi * 3;

        var cesit = (buyuk > 0) + (kucuk > 0) + (rakam > 0) + (simge > 0) + (n >= 8);
        if (cesit > (n >= 8 ? 3 : 4)) puan += cesit * 2;
        return Math.max(0, Math.min(100, Math.round(puan)));
    }

    function gucGoster() {
        var puan = guc($('#txtYeniParola').val());
        var sinif = puan < 40 ? 'progress-bar-danger' : puan < 70 ? 'progress-bar-warning' : 'progress-bar-success';
        $('#pdGuc .progress-bar')
            .removeClass('progress-bar-danger progress-bar-warning progress-bar-success')
            .addClass(sinif)
            .css('width', puan + '%')
            .attr('aria-valuenow', puan)
            .text(puan + '%');
    }

    function degistir(e) {
        e.preventDefault();
        var $t = $('#btnParola').prop('disabled', true);
        Sgb.gonder(UC + 'degistir', {
            parola: $('#txtParola').val() || '',
            yeniParola: $('#txtYeniParola').val() || '',
            yeniParolaTekrar: $('#txtYeniParolaTekrar').val() || ''
        }).done(function (c) {
            if (!c || !c.basarili) {
                Sgb.bildirim(c && c.hata ? c.hata : 'Parola değiştirilemedi.', 'danger');
                $t.prop('disabled', false);
                return;
            }
            // Oturum kapandı: saklanan menü verisi silinir, giriş sayfasına gidilir.
            try { localStorage.removeItem('sgb.menu'); } catch (x) { }
            Sgb.bildirim(c.bilgi, 'success');
            setTimeout(function () { window.top.location.href = c.yonlen || '../Giris'; }, 1500);
        }).fail(function (xhr) {
            $t.prop('disabled', false);
            if (xhr && xhr.status === 401) return;
            Sgb.bildirim(xhr && xhr.status === 403 ? 'Parolanız bu ekrandan değiştirilemez.' : 'İşlem yapılamadı.', 'danger');
        });
    }

    $(function () {
        $('#txtYeniParola').on('input', gucGoster);
        $('#frmParola').on('submit', degistir);
    });
})(jQuery);
