// Borcu Yoktur Belgesi / Vergi Borcu Sorgulama Girişi (eski HarcamaSurecleri/BorcuYokBelgesi.aspx + ModulScripts/BorcuYokBelgesi.js).
// Uçlar: /api/borcuyok/* (HarcamaSurecleri/BorcuYokBelgesiUclari.cs); muhasebe adı ve listesi ortak /api/muhbirim/bilgi, /agac.
//
// Adres: BorcuYokBelgesi?muhasebe=&birim=&belgeYil=&belgeNo=&vNo=   (Ödeme Emri formundan Sgb.pencere ile)
//        BorcuYokBelgesi?kod=&tip=1&muhasebe=  dilekçenin belgesini yazdırır;  tip=2  dilekçeyi GİB'den sorgular (eski Belge Arama)
// Gönder başarılı olunca, sayfa bir pencerede açıldıysa ve üst sayfada window.borcuYokGonderildi işlevi varsa
// { vergiNo, cevapId, durum, muhasebe, birim, belgeYil, belgeNo } ile çağrılır.
(function ($) {
    'use strict';

    var adres = new URLSearchParams(window.location.search);
    var istek = {
        muhasebe: adres.get('muhasebe') || '',
        birim: adres.get('birim') || '',
        belgeYil: parseInt(adres.get('belgeYil'), 10) || 0,
        belgeNo: adres.get('belgeNo') || '',
        vNo: adres.get('vNo') || '',
        kod: parseInt(adres.get('kod'), 10) || 0,
        tip: parseInt(adres.get('tip'), 10) || 0
    };

    var oncekiKod = 0; // eski hdnOncekiDilekceNo
    var soruEvet = null;

    // ------------------------------------------------------------ ortak

    function hataGoster(xhr, varsayilan) {
        if (xhr && xhr.status === 401) return;
        Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : varsayilan, 'danger');
    }

    function mesgul($tus, evet) {
        $tus.prop('disabled', !!evet).toggleClass('yukleniyor', !!evet);
    }

    function sor(metin, evet) {
        $('#soruMetin').text(metin);
        soruEvet = evet;
        $('#dlgSoru')[0].showModal();
    }

    $(document).on('click', 'dialog [data-kapat]', function () { $(this).closest('dialog')[0].close(); });
    $('#btnSoruEvet').on('click', function () {
        var f = soruEvet;
        soruEvet = null;
        $('#dlgSoru')[0].close();
        if (f) f();
    });

    $('#btnYardim').on('click', function () {
        var $p = $('#pnlIpucu');
        var ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).toggleClass('active', ac).attr('aria-expanded', ac ? 'true' : 'false');
    });

    // ------------------------------------------------------------ muhasebe (eski txtMuhasebe + MuhasebeGoster(1) + kodAdGetir('11'))

    function muhasebeAdOku() {
        var m = parseInt($.trim($('#txtMuhasebe').val()), 10) || 0;
        if (m <= 0) { $('#lblMuhasebeAd').text(''); return; }
        Sgb.al('api/muhbirim/bilgi', { muhasebe: m }).done(function (v) {
            if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
            $('#lblMuhasebeAd').text(v.birimAd || '');
        }).fail(function (xhr) { hataGoster(xhr, 'Muhasebe adı okunamadı.'); });
    }

    $('#txtMuhasebe').on('change', muhasebeAdOku);

    var agac = null;
    $('#btnMuhasebeSec').on('click', function () {
        $('#dlgMuhasebe')[0].showModal();
        if (agac) return;
        agac = Sgb.agac('#agcMuhasebe', {
            duz: true,
            yukle: function () {
                return Sgb.al('api/muhbirim/agac', { ust: '' }).then(function (v) {
                    if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return []; }
                    return $.map(v.dugumler || [], function (d) { return $.extend({}, d, { altVar: false }); });
                });
            },
            secilince: function (d) {
                $('#dlgMuhasebe')[0].close();
                $('#txtMuhasebe').val(d.kod);
                muhasebeAdOku();
            },
            seciliMi: function (d) { return d.kod === $.trim($('#txtMuhasebe').val()); },
            bosMetin: 'Yetkili olduğunuz muhasebe birimi bulunamadı.',
            hata: function (xhr) { hataGoster(xhr, 'Muhasebe birimleri okunamadı.'); }
        });
    });

    // ------------------------------------------------------------ kimlik (eski imgKimlik_Click)

    function kimlikYaz(k) {
        k = k || {};
        $('#lblVD').text(k.vergiDaire || '');
        $('#lblAd').text(k.unvan || '');
        $('#lblAdres').text(k.adres || '');
    }

    $('#btnKimlik').on('click', function () {
        var $t = $(this);
        var no = $.trim($('#txtVergiNo').val());
        if (no.length !== 10 && no.length !== 11) {
            Sgb.bildirim('Lütfen 11 rakamlı Tc Kimlik veya 10 rakamlı Vergi Kimlik numarası giriniz.', 'warning');
            return;
        }
        mesgul($t, true);
        Sgb.al('api/borcuyok/kimlik', { vergiNo: no }).done(function (v) {
            kimlikYaz(v);
            if (!v.basarili && v.hata) Sgb.bildirim(v.hata, 'danger');
        }).fail(function (xhr) { hataGoster(xhr, 'Kimlik bilgisi okunamadı.'); })
            .always(function () { mesgul($t, false); });
    });

    $('#txtVergiNo').on('keydown', function (e) {
        if (e.key === 'Enter') { e.preventDefault(); $('#btnKimlik').trigger('click'); }
    });

    // ------------------------------------------------------------ gönder / temizle

    // Eski FormTemizle (muhasebe korunur).
    function formTemizle() {
        $('#ddlNeden').val('0');
        $('#txtVergiNo').val('');
        $('#txtIlgiliMakam').val('');
        kimlikYaz(null);
    }

    $('#btnTemizle').on('click', formTemizle);
    $('#frmGiris').on('submit', function (e) { e.preventDefault(); });

    $('#btnKaydet').on('click', function () {
        var $t = $(this);
        var veri = {
            Muhasebe: parseInt($.trim($('#txtMuhasebe').val()), 10) || 0,
            VergiNo: $.trim($('#txtVergiNo').val()),
            TalepNedeni: parseInt($('#ddlNeden').val(), 10) || 0,
            IlgiliMakam: $.trim($('#txtIlgiliMakam').val()),
            Birim: istek.birim,
            BelgeYil: istek.belgeYil,
            BelgeNo: istek.belgeNo
        };
        mesgul($t, true);
        Sgb.gonder('api/borcuyok/kaydet', veri).done(function (v) {
            if (v.kimlik) kimlikYaz(v.kimlik);
            else if (v.unvan !== undefined) kimlikYaz(v);
            if (!v.basarili) { Sgb.bildirim(v.hata || 'Kayıt işlemi yapılamadı.', 'danger'); return; }
            Sgb.bildirim(v.bilgi, 'success');
            if (v.temizle) formTemizle();
            try {
                if (window.parent !== window && typeof window.parent.borcuYokGonderildi === 'function')
                    window.parent.borcuYokGonderildi({
                        vergiNo: veri.VergiNo, cevapId: v.cevapId || '', durum: v.durum, muhasebe: veri.Muhasebe,
                        birim: istek.birim, belgeYil: istek.belgeYil, belgeNo: istek.belgeNo
                    });
            } catch (ex) { /* üst sayfa başka kökten ise yok sayılır */ }
        }).fail(function (xhr) { hataGoster(xhr, 'Kayıt işlemi yapılamadı.'); })
            .always(function () { mesgul($t, false); });
    });

    // ------------------------------------------------------------ yazdır / sorgula (eski BelgeYazdir, BelgeSorgula)

    function belgeYazdir(kod) {
        if (!kod) return;
        Sgb.indir('api/borcuyok/yazdir', { kod: kod });
    }

    function belgeSorgula(kod, muhasebe) {
        Sgb.gonder('api/borcuyok/sorgula', { Kod: kod, Muhasebe: parseInt(muhasebe, 10) || 0 }).done(function (v) {
            if (!v.basarili) { Sgb.bildirim(v.hata || 'Dilekçe sorgulanamadı.', 'danger'); return; }
            Sgb.bildirim(v.bilgi, 'success');
        }).fail(function (xhr) { hataGoster(xhr, 'Dilekçe sorgulanamadı.'); });
    }

    $('#btnOncekiYazdir').on('click', function () { belgeYazdir(oncekiKod); });

    // ------------------------------------------------------------ taahhütname (eski TanimTaahhutname.aspx?tur=1)

    $('#btnTaahhutKabul').on('click', function () {
        var $t = $(this);
        mesgul($t, true);
        Sgb.gonder('api/borcuyok/taahhut').done(function (v) {
            if (!v.basarili) { Sgb.bildirim(v.hata || 'Taahhütname kaydedilemedi.', 'danger'); return; }
            Sgb.bildirim(v.bilgi, 'success');
            yukle();
        }).fail(function (xhr) { hataGoster(xhr, 'Taahhütname kaydedilemedi.'); })
            .always(function () { mesgul($t, false); });
    });

    $('#btnTaahhutRet').on('click', function () {
        // Pencerede açıldıysa pencere kapanır, değilse giriş sayfasına dönülür (eski GirisBilgi.aspx).
        try {
            var cerceve = window.frameElement;
            var dlg = cerceve ? $(cerceve).closest('dialog')[0] : null;
            if (dlg) { dlg.close(); return; }
        } catch (ex) { /* yok sayılır */ }
        window.location.href = '../';
    });

    // ------------------------------------------------------------ açılış (eski Page_Load)

    function yukle() {
        Sgb.al('api/borcuyok/tanim', { muhasebe: istek.muhasebe, vNo: istek.vNo }).done(function (v) {
            if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
            $('#pnlTaahhut').prop('hidden', !!v.taahhut);
            $('#pnlForm').prop('hidden', !v.taahhut);
            if (!v.taahhut) return;

            var $n = $('#ddlNeden').empty();
            $.each(v.nedenler || [], function (i, n) { $('<option></option>').val(String(n.KOD)).text(n.AD).appendTo($n); });
            $n.val('0');
            $('#txtMuhasebe').val(v.muhasebe || '');
            $('#txtVergiNo').val(istek.vNo);
            $('#txtIlgiliMakam').val(v.ilgiliMakam || '');
            kimlikYaz(null);
            muhasebeAdOku();

            if (v.onceki && v.onceki.kod > 0) {
                oncekiKod = v.onceki.kod;
                $('#btnOncekiYazdir').prop('hidden', false);
                sor(v.onceki.metin, function () { belgeYazdir(oncekiKod); });
            }

            if (istek.kod > 0 && istek.tip === 1) belgeYazdir(istek.kod);
            else if (istek.kod > 0 && istek.tip === 2) belgeSorgula(istek.kod, istek.muhasebe);
        }).fail(function (xhr) { hataGoster(xhr, 'Sayfa bilgileri okunamadı.'); });
    }

    yukle();
})(jQuery);
