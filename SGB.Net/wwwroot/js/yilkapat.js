// Muhasebe Kayıt İşlemi Tarih Sınırlaması, ButceMuhasebe/YilKapat (eski YilKapat.aspx): verilen tarihten öncesine ait
// Muhasebe Kayıt Belgesi girişini engeller. Uçlar: /api/yilkapat/* (Muhasebe/YilKapatUclari.cs).
$(function () {
    'use strict';

    if (!$('.yil-kapat').length) return;

    var UC = 'api/yilkapat/';

    function sonucYaz(metin, tur) {
        $('#pnlSonuc').removeClass('alert-success alert-danger alert-info').addClass('alert-' + tur).text(metin || '').prop('hidden', !metin);
    }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        sonucYaz(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // Eski lblBos: "<tarih> öncesine ait Muhasebe Kayıt Belgesinin girişi engellenmiştir."
    function durumYaz(d) {
        $('#txtTarih').val(d && d.tarih ? d.tarih : '');
        var $p = $('#pnlDurum');
        if (d && d.tarih) $p.removeClass('alert-info').addClass('alert-warning').text((d.tarihMetin || Sgb.tarih(d.tarih)) + ' öncesine ait Muhasebe Kayıt Belgesinin girişi engellenmiştir.');
        else $p.removeClass('alert-warning').addClass('alert-info').text('Muhasebe Kayıt Belgesi girişi için tarih sınırlaması yok.');
        $p.prop('hidden', false);
    }

    $('#btnKaydet').on('click', function () {
        var $d = $(this).prop('disabled', true);
        sonucYaz('', 'info');
        Sgb.gonder(UC + 'kaydet', { Tarih: $('#txtTarih').val() || '' })
            .done(function (v) {
                if (!v.basarili) { sonucYaz(v.hata || 'Kayıt yapılamadı.', 'danger'); return; }
                durumYaz(v.durum);
                sonucYaz(v.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    Sgb.al(UC + 'tanim').done(durumYaz).fail(hataGoster);
});
