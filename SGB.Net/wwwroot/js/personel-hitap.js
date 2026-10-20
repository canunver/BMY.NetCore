// HİTAP'tan Al penceresi (eski InsanKaynaklari/PersonelHITAP.aspx). Hizmet Belgesi sekmesinden Sgb.pencere ile açılır;
// aktarım bitince pencere kapanır ve sekme listeyi yeniden okur (eski parent.parent.KadroYenile).
(function ($) {
    'use strict';

    var $kok = $('#pbHitap');
    if (!$kok.length) return;
    var PERSONEL = parseInt($kok.data('personel-kod'), 10) || 0;
    var TUR = parseInt($kok.data('tur'), 10) || 1;
    var UC = 'api/personelbilgi/kadro/hitap/';
    var birimKod = 0;
    var agac = null;

    function cerceve() {
        try { return window.frameElement ? window.frameElement.closest('dialog') : null; } catch (e) { return null; }
    }
    function pencereKapat() { var d = cerceve(); if (d) d.close(); }

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // Eski Page_Load: kayıtlı HİTAP kullanıcı kodu / parolası, personel türleri.
    function bilgiOku() {
        Sgb.al(UC + 'bilgi').done(function (v) {
            $('#txtHITAPKul').val(v.kullanici || '');
            $('#txtHITAPPar').val(v.parola || '');
            var $c = $('#cmbPersonelTip').empty();
            $.each(v.personelTipleri || [], function (i, t) { $('<option></option>').val(t.kod).text(t.ad).appendTo($c); });
        }).fail(hataGoster);
    }

    // Birim seçimi (eski ddlBirim + trpBirim, OrgBirimDoldur).
    var dlg = document.getElementById('dlgBirim');
    $(dlg).on('click', '[data-kapat]', function () { dlg.close(); });
    $('#btnBirimSec, #txtBirim').on('click', function () {
        if (!agac) {
            agac = Sgb.agac('#agcBirim', {
                yukle: function (ust) {
                    return Sgb.al(UC + 'birimler', { ust: ust ? ust.kod : 0 }).then(function (v) {
                        if (v.hata) Sgb.bildirim(v.hata, 'danger');
                        return v.satirlar || [];
                    });
                },
                secilince: function (d) {
                    birimKod = d.kod;
                    $('#txtBirim').val(d.ad);
                    dlg.close();
                },
                seciliMi: function (d) { return d.kod === birimKod; },
                bosMetin: 'Birim bulunamadı.',
                hata: hataGoster
            });
        }
        dlg.showModal();
    });
    $('#btnBirimTemizle').on('click', function () { birimKod = 0; $('#txtBirim').val(''); });

    // Eski btnVeriAl_Click.
    $('#btnVeriAl').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'verial', {
            personelKod: PERSONEL,
            tur: TUR,
            kullanici: $('#txtHITAPKul').val(),
            parola: $('#txtHITAPPar').val(),
            personelTip: parseInt($('#cmbPersonelTip').val(), 10) || 0,
            organizasyonKodu: birimKod
        }).done(function (v) {
            if (v.basarili) {
                Sgb.bildirim(v.bilgi || 'Aktarım tamamlandı.', 'success');
                if (cerceve()) setTimeout(pencereKapat, 900);
            } else Sgb.bildirim(v.hata, 'danger');
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    });

    bilgiOku();
})(jQuery);
