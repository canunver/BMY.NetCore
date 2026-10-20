// Personel İşlemleri > İletişim/Adres sekmesi (InsanKaynaklari/PersonelAdres; eski PersonelAdres.aspx).
// Açılışta personelin iletişim bilgileri okunur; Kaydet onay sorar; "Adres Bilgisini Getir" KPS'deki ikametgah adresini
// İkamet Adresi alanına yazar (kaydetmez).
$(function () {
    'use strict';

    var $kok = $('#pbAdres');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var personelKod = parseInt($kok.data('personel-kod'), 10) || 0;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $kok.find('.pb-arac button').prop('disabled', evet); }

    function onayla(metin) {
        var d = $.Deferred();
        var dlg = document.getElementById('dlgOnay');
        $('#dlgOnayMetin').text(metin);
        $(dlg).off('click.onay').on('click.onay', '[data-sonuc]', function () {
            var evet = $(this).data('sonuc') === 'evet';
            dlg.close();
            if (evet) d.resolve(); else d.reject();
        });
        $(dlg).off('cancel.onay').on('cancel.onay', function () { d.reject(); });
        dlg.showModal();
        return d.promise();
    }

    // Eski Listele (önce Temizle)
    function oku() {
        return Sgb.al('api/personelbilgi/adres/oku', { personelKod: personelKod })
            .done(function (v) {
                // Eski Page_Load: Manas kullanılıyorsa Tarabya ve Görüntülü telefon alanları gizlenir.
                $('#grpTarabyaTel, #grpGoruntuluTel').prop('hidden', !!v.manas);
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#frmAdres')[0].reset();
                var b = v.bilgi;
                if (!b) return;
                $('#txtAdres').val(b.adres);
                $('#txtIsTel').val(b.isTelefon);
                $('#txtEvTel').val(b.evTelefon);
                $('#txtCepTel').val(b.cepTelefon);
                $('#txtFaks').val(b.faks);
                $('#txtMobilImzaTel').val(b.mobilImzaTelefon);
                $('#txtePosta').val(b.eposta);
                $('#txtePosta2').val(b.eposta2);
                $('#txtCep2').val(b.cepTelefon2);
                $('#txtTarabyaTel').val(b.tarabyaTel);
                $('#txtLojmanTel').val(b.lojmanTel);
                $('#txtTelsizTel').val(b.telsiz);
                $('#txtGoruntuluTel').val(b.goruntulu);
                $('#chkRehber').prop('checked', !!b.rehberdeGozuksun);
                $('#txtURL').val(b.url);
            })
            .fail(hataGoster);
    }

    $('#btnKaydet').on('click', function () {
        // Eski: form geçerli değilse (e-posta / web adresi biçimi) uyarı verilir.
        if (!$('#frmAdres')[0].checkValidity()) {
            mesaj('Lütfen, ikamet adresi, e-posta ve web adresi bilgilerinizi kontrol edin.', 'warning');
            return;
        }
        onayla('Kayıt işlemi yapılacak. Onaylıyor musunuz?').done(function () {
            if (!personelKod) { mesaj('Personel Bilgisi boş bırakılamaz.', 'warning'); return; }
            mesgul(true);
            Sgb.gonder('api/personelbilgi/adres/kaydet', {
                personelKod: personelKod,
                adres: $('#txtAdres').val(),
                isTelefon: $('#txtIsTel').val(),
                evTelefon: $('#txtEvTel').val(),
                cepTelefon: $('#txtCepTel').val(),
                faks: $('#txtFaks').val(),
                eposta: $('#txtePosta').val(),
                eposta2: $('#txtePosta2').val(),
                url: $('#txtURL').val(),
                cepTelefon2: $('#txtCep2').val(),
                tarabyaTel: $('#txtTarabyaTel').val(),
                lojmanTel: $('#txtLojmanTel').val(),
                telsiz: $('#txtTelsizTel').val(),
                goruntulu: $('#txtGoruntuluTel').val(),
                rehberdeGozuksun: $('#chkRehber').prop('checked')
            }).done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                oku();
                if (window.parent && window.parent.PersonelBilgi) window.parent.PersonelBilgi.ozetYenile();
            }).fail(hataGoster)
                .always(function () { mesgul(false); });
        });
    });

    // Eski btnAdres_Click
    $('#btnAdres').on('click', function () {
        mesgul(true);
        Sgb.al('api/personelbilgi/adres/getir', { personelKod: personelKod })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                $('#txtAdres').val(v.adres || '');
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    oku();
});
