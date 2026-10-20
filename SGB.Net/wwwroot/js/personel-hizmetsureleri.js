// Hizmet Süreleri penceresi (eski InsanKaynaklari/PersonelHizmetSureleri.aspx): hesaplanan hizmet süreleri (salt okunur).
(function ($) {
    'use strict';

    var $kok = $('#pbHizmetSureleri');
    if (!$kok.length) return;
    var PERSONEL = parseInt($kok.data('personel-kod'), 10) || 0;

    Sgb.al('api/personelbilgi/kadro/hizmetsureleri', { personelKod: PERSONEL }).done(function (v) {
        if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
        $('#txtToplamHizmet').val(v.toplamHizmet || '');
        $('#txtKAH').val(v.kah || '');
        $('#txtKHA').val(v.kha || '');
        $('#txtEKEA').val(v.ekea || '');
        $('#txtKurum').val(v.kurum || '');
        $('#txtIzin').val(v.izin || '');
    }).fail(function (xhr) {
        if (xhr && xhr.status === 401) return;
        Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Hizmet süreleri okunamadı.', 'danger');
    });
})(jQuery);
