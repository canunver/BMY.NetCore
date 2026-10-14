// Şifre Üret (eski YazilimGuncelle/SifreUret.aspx). Uç: /api/guncelleme/sifre (yon 1: şifreli yap, 2: şifre çöz)
(function ($) {
    'use strict';

    $('.islem').on('click', function () {
        var $d = $('.islem').prop('disabled', true);
        Sgb.gonder('api/guncelleme/sifre', { veri: $('#txtVeri').val(), yon: parseInt($(this).data('yon'), 10), xml: $('#chkxml').prop('checked') })
            .done(function (s) {
                if (!s.basarili) { Sgb.bildirim(s.hata, 'danger'); return; }
                $('#txtSonuc').val(s.sonuc || '');
            })
            .fail(function (x) { if (x.status !== 401) Sgb.bildirim(x.status === 403 ? 'Yetkiniz yok!' : 'İşlem yapılamadı.', 'danger'); })
            .always(function () { $d.prop('disabled', false); });
    });
})(jQuery);
