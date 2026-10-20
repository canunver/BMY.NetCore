// Kullanıcılara e-posta Gönderme (KYM/ePostaAt, eski ePostaAt.aspx).
// Konu ve mesaj seçilen kullanıcı grubuna gönderilmek üzere kaydedilir; gönderimi sunucu arka planda yapar.
// "Önceki Gönderiler" (KYM/ePostaGonderi) yalnızca sistem yöneticisi / destek yöneticisine görünür.
$(function () {
    'use strict';

    var $btn = $('#btnTamam');

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    Sgb.al('api/eposta/tanim').done(function (t) {
        if (t.hata) Sgb.bildirim(t.hata, 'danger');
        if (t.adres) { $('#txtAdres').text(t.adres); $('#lblAdres').prop('hidden', false); }
        var $l = $('#lstKullaniciTipi').empty();
        $.each(t.gruplar || [], function (i, g) { $('<option>').val(g.kod).text(g.ad).appendTo($l); });
        $('#btnGonderiler').prop('hidden', !t.gonderiYetki);
    }).fail(hataGoster);

    $('#frmPosta').on('submit', function (e) {
        e.preventDefault();
        var konu = $.trim($('#txtKonu').val());
        var mesaj = $('#txtMesaj').val();
        var hatalar = [];
        if (!konu) hatalar.push('Lütfen konu alanını doldurunuz!');
        if (!$.trim(mesaj)) hatalar.push('Lütfen mesaj alanını doldurunuz!');
        if (hatalar.length) { Sgb.bildirim(hatalar.join('\n'), 'warning'); return; }
        if (!window.confirm($btn.data('onay'))) return;

        $btn.prop('disabled', true);
        Sgb.gonder('api/eposta/kaydet', {
            konu: konu,
            mesaj: mesaj,
            ktKod: parseInt($('#lstKullaniciTipi').val(), 10) || 0
        }).done(function (s) {
            if (!s.basarili) { Sgb.bildirim(s.hata, 'danger'); return; }
            Sgb.bildirim(s.bilgi || 'e-posta gönderilmek üzere kaydedildi.', 'success');
        }).fail(hataGoster).always(function () { $btn.prop('disabled', false); });
    });
});
