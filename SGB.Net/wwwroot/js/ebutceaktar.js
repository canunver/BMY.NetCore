// e-bütçe Aktarımları, ButceMuhasebe/Aktar (eski Aktar.aspx): harcama gerçekleşmelerini (yıl, ay) e-bütçeye aktarır ya da ödenek
// servisini (yıl) çalıştırır; e-bütçe kullanıcı adı / parolası saklanır. Kayıtlı parola tarayıcıya gelmez; parola alanı boş
// bırakılıp Sakla'ya basılırsa yalnızca kullanıcı adı güncellenir. Uçlar: /api/ebutceaktar/* (Muhasebe/EbutceAktarUclari.cs).
$(function () {
    'use strict';

    if (!$('.ebutce-aktar').length) return;

    var UC = 'api/ebutceaktar/';

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function islem() { return $('input[name="islem"]:checked').val(); }
    function sonucYaz(metin, tur) {
        $('#pnlSonuc').removeClass('alert-success alert-danger alert-info').addClass('alert-' + tur).text(metin || '').prop('hidden', !metin);
    }

    // Eski AlanlariAyarla: ay yalnızca harcama aktarımında.
    function alanlariAyarla() { $('#grpAy').prop('hidden', islem() !== 'harcama'); }
    $('input[name="islem"]').on('change', alanlariAyarla);

    $('#btnAktar').on('click', function () {
        var $d = $(this).prop('disabled', true).addClass('avans-bekle');
        sonucYaz('Aktarım yapılıyor, lütfen bekleyiniz...', 'info');
        Sgb.gonder(UC + 'aktar', { Yil: parseInt($('#txtYil').val(), 10) || 0, Ay: parseInt($('#ddlAy').val(), 10) || 0, Islem: islem() })
            .done(function (v) {
                if (v.sonuc !== undefined) sonucYaz('Sonuç: ' + (v.sonuc || (v.basarili ? 'İşlem tamamlandı.' : 'İşlem yapılamadı.')), v.basarili ? 'success' : 'danger');
                else if (v.basarili) sonucYaz(v.bilgi, 'success');
                else sonucYaz(v.hata || 'Aktarım yapılamadı.', 'danger');
            })
            .fail(function (x) { sonucYaz('', 'info'); hataGoster(x); })
            .always(function () { $d.prop('disabled', false).removeClass('avans-bekle'); });
    });

    $('#btnSakla').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'parola', { Kullanici: $.trim($('#txtKullanici').val()), Parola: $('#txtParola').val() })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata || 'Saklanamadı.', 'danger'); return; }
                mesaj(v.bilgi, 'success');
                if ($('#txtParola').val()) $('#txtParola').val('').attr('placeholder', 'Kayıtlı parola var');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    Sgb.al(UC + 'tanim')
        .done(function (t) {
            Sgb.yilKutusu('#txtYil', t.yillar, t.yil);
            var $a = $('#ddlAy').empty();
            $.each(t.aylar || [], function (i, a) { $('<option></option>').val(a.kod).text(a.ad).appendTo($a); });
            $a.val(String(t.ay));
            $('#txtKullanici').val(t.ebutceKullanici || '');
            $('#txtParola').attr('placeholder', t.parolaVar ? 'Kayıtlı parola var' : 'Parola');
        })
        .fail(hataGoster);

    alanlariAyarla();
});
