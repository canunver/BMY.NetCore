// Hizmet Ayarları penceresi (eski InsanKaynaklari/PersonelHizmetAyari.aspx): KHA / EKEA terfi ayı ve açılış tarihleri.
(function ($) {
    'use strict';

    var $kok = $('#pbHizmetAyari');
    if (!$kok.length) return;
    var PERSONEL = parseInt($kok.data('personel-kod'), 10) || 0;
    var UC = 'api/personelbilgi/kadro/hizmetayari';

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // Eski Page_Load: Genel.AyDoldur + Listele.
    function oku() {
        Sgb.al(UC, { personelKod: PERSONEL }).done(function (v) {
            $('#ddlKTAyi, #ddlETAyi').each(function () {
                var $c = $(this).empty();
                $.each(v.aylar || [], function (i, a) { $('<option></option>').val(a.KOD).text(a.AD).appendTo($c); });
            });
            if (v.hata) Sgb.bildirim(v.hata, 'danger');
            if (v.ayar) {
                $('#ddlKTAyi').val(String(v.ayar.kTerfiAyi));
                $('#ddlETAyi').val(String(v.ayar.eTerfiAyi));
                $('#txtKAcilisTarihi').val(v.ayar.kAcilisTarihi || '');
                $('#txtEAcilisTarihi').val(v.ayar.eAcilisTarihi || '');
            }
        }).fail(hataGoster);
    }

    // Eski btnKaydet_Click (onay sorusuyla).
    var dlg = document.getElementById('dlgOnay');
    $(dlg).on('click', '[data-kapat]', function () { dlg.close(); });
    $('#btnKaydet').on('click', function () { dlg.showModal(); });
    $('#btnOnayEvet').on('click', function () {
        dlg.close();
        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder(UC + '/kaydet', {
            personelKod: PERSONEL,
            kTerfiAyi: parseInt($('#ddlKTAyi').val(), 10) || 0,
            eTerfiAyi: parseInt($('#ddlETAyi').val(), 10) || 0,
            kAcilisTarihi: $('#txtKAcilisTarihi').val() || '',
            eAcilisTarihi: $('#txtEAcilisTarihi').val() || ''
        }).done(function (v) {
            Sgb.bildirim(v.basarili ? v.bilgi : v.hata, v.basarili ? 'success' : 'danger');
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    });

    oku();
})(jQuery);
