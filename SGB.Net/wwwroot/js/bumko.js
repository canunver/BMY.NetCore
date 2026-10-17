// BÜMKO Aktarım (ButceHazirlikv2/BUMKOVeriGonder): bütçe hazırlık verisini SBB program bütçe sistemine gönderir.
// Program Bütçe (ekonomik kod düzeyinde ödenek, YSHT, tavan üstü), Bütçe Fiş/Ek Ödenek Gerekçeleri ya da Faaliyet Gerekçesi seçilip
// Aktar'a basılır; sonuç ve ayrıntılı hata listesi sayfada gösterilir. Ödenek Kontrol SBB ile SGB arasındaki farkları Excel'e yazar.
// e-Bütçe kullanıcı kodu ve şifresi her işlemde istekle gönderilir, saklanmaz.
$(function () {
    'use strict';

    if (!$('.bumko-aktarim').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    function mesaj(metin, t) { Sgb.bildirim(metin, t); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function sonucYaz(metin, ayrinti, tur) {
        var $s = $('#sonuc').removeClass('alert-success alert-danger alert-info').addClass('alert-' + tur).empty();
        $('<strong></strong>').text(metin).appendTo($s);
        if (ayrinti) $('<div></div>').text(ayrinti).appendTo($s);
        $s.prop('hidden', false);
    }

    function istekOl() {
        var secim = $('input[name="secim"]:checked').val();
        return {
            yil: parseInt($('#txtYil').val(), 10) || 0,
            senaryo: $('#ddlSenaryo').val() || '',
            asama: parseInt($('#ddlAsama').val(), 10) || 0,
            kullanici: $.trim($('#txtKullanici').val()),
            parola: $('#txtParola').val(),
            secim: secim,
            eko: secim === 'gerekce' ? $('#ddlEkoGerekce').val() : $('#ddlEko').val()
        };
    }

    function denetle(o) {
        if (o.yil < 2003) { mesaj('Yıl bilgisi hatalı!', 'warning'); $('#txtYil').trigger('focus'); return false; }
        if (!o.kullanici) { mesaj('e-Bütçe kullanıcı adını yazınız.', 'warning'); $('#txtKullanici').trigger('focus'); return false; }
        if (!o.parola) { mesaj('e-Bütçe kullanıcı şifresini yazınız.', 'warning'); $('#txtParola').trigger('focus'); return false; }
        return true;
    }

    function mesgul(evet) { $('#btnAktar, #btnOdenekKontrol').prop('disabled', evet); }

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/bumko/tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            $('#txtYil').val(v.yil);
            var $s = $('#ddlSenaryo').empty();
            $.each(v.senaryolar || [], function (i, s) { $('<option></option>').val(s.kod).text(s.ad).appendTo($s); });
            $s.val(v.senaryo);
            $('#lblKurum').text(v.kurum + ' Kurumu Bütçe Hazırlık Veri Aktarma Ekranı');
        })
        .fail(hataGoster);

    // Seçilmeyen satırın ekonomik kodu kapalı görünür.
    function secimGuncelle() {
        var secim = $('input[name="secim"]:checked').val();
        $('#ddlEko').prop('disabled', secim !== 'odenek');
        $('#ddlEkoGerekce').prop('disabled', secim !== 'gerekce');
    }
    $('input[name="secim"]').on('change', secimGuncelle);
    secimGuncelle();

    // ---------------------------------------------------------------- Aktar (eski btnAktar_Click)

    $('#frmAktar').on('submit', function (e) {
        e.preventDefault();
        var o = istekOl();
        if (!denetle(o)) return;
        var ad = $('input[name="secim"]:checked').parent().text().trim();
        if (!window.confirm(o.yil + ' yılı "' + ad + '"' + (o.secim !== 'faaliyet' ? ' (' + o.eko + ')' : '') + ' SBB\'ye aktarılacak. Devam edilsin mi?')) return;
        mesgul(true);
        sonucYaz('Aktarılıyor, lütfen bekleyiniz...', '', 'info');
        Sgb.gonder('api/bumko/aktar', o)
            .done(function (s) {
                if (s.basarili) { sonucYaz(s.bilgi, s.ayrinti, 'success'); mesaj(s.bilgi, 'success'); }
                else { sonucYaz(s.hata, s.ayrinti, 'danger'); mesaj(s.hata, 'danger'); }
            })
            .fail(function (xhr) { $('#sonuc').prop('hidden', true); hataGoster(xhr); })
            .always(function () { mesgul(false); });
    });

    // ---------------------------------------------------------------- Ödenek Kontrol (eski btnOdenekKontrol_Click)
    // Şifre adres satırına yazılmasın diye istek POST ile gönderilir; dönen dosya indirilir.

    $('#btnOdenekKontrol').on('click', function () {
        var o = istekOl();
        if (!denetle(o)) return;
        mesgul(true);
        fetch(Sgb.KOK + 'api/bumko/odenek-kontrol', {
            method: 'POST',
            credentials: 'same-origin',
            cache: 'no-store',
            headers: { 'Content-Type': 'application/json; charset=utf-8', RequestVerificationToken: Sgb.meta('istek-jetonu') },
            body: JSON.stringify(o)
        }).then(function (yanit) {
            if (yanit.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            if (!yanit.ok) { mesaj(yanit.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Rapor hazırlanamadı.', 'danger'); return; }
            var tur = yanit.headers.get('Content-Type') || '';
            if (tur.indexOf('json') >= 0)
                return yanit.json().then(function (v) { mesaj(v.hata || 'Rapor hazırlanamadı.', 'danger'); });
            if (tur.indexOf('text/html') >= 0) { mesaj('Rapor hazırlanamadı.', 'danger'); return; }
            var baslik = yanit.headers.get('Content-Disposition') || '';
            var ad = /filename\*=UTF-8''([^;]+)/i.exec(baslik) || /filename="?([^";]+)"?/i.exec(baslik);
            return yanit.blob().then(function (icerik) {
                var yerel = URL.createObjectURL(icerik);
                var a = document.createElement('a');
                a.href = yerel;
                a.download = ad ? decodeURIComponent(ad[1]) : 'SBBSGBOdenekKontrol.xlsx';
                document.body.appendChild(a);
                a.click();
                a.remove();
                setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
            });
        }).catch(function () {
            mesaj('Rapor hazırlanamadı.', 'danger');
        }).finally(function () { mesgul(false); });
    });
});
