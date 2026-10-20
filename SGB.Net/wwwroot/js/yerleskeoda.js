// Yerleşke Oda Tanımı (KYM/TanimOda): solda yerleşke / bina / oda ağacı (Sgb.agac), sağda seçili yerin bilgileri.
// Eski sayfadaki gibi: ağaçta yere tıklanınca bilgileri gelir; Yeni Yer seçili yerin altına kayıt açar (eski sağ tık menüsü);
// Taşı seçili yeri başka yerin altına alır (eski sürükle-bırak); Yazdır bütün yerlerin Excel listesi; Dosya Yükle Excel'deki
// kod / adları formdaki yerin altına ekler. Yer türü Yerleşke ise il, Oda ise birim seçilir. Merkez Bankası kurulumunda
// maliyet merkezi, sıra no, birim kodu alanları ve Pasifleri de Listele tuşu görünür; oda kodu 50 haneye kadar olabilir.
$(function () {
    'use strict';

    if (!$('#agacYer').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var KOK_DUGUM = { kod: -1, ad: 'Yerleşke ve Oda Şeması', altVar: true, birimKod: '', kok: true };
    var tanim = { merkezBankasi: false, iller: [] };
    var secili = null;   // ağaçta seçili düğüm
    var kayit = null;    // formdaki kayıt (eski gizli alanlar: kod, ustKod, ustGorKod, ustTarif, saymanlikKod, orgKod)
    var pasifler = false;
    var agac = null;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function sayi(deger) { return parseInt(deger, 10) || 0; }
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- ağaç

    function yerleriOku(ust) {
        if (!ust) return $.Deferred().resolve([KOK_DUGUM]).promise();
        var p = { ust: ust.kod };
        if (pasifler) p.pasif = '1';
        return Sgb.al('api/yerleskeoda/agac', p).then(function (veri) {
            if (veri.hata) { mesaj(veri.hata, 'danger'); return []; }
            return veri.satirlar || [];
        });
    }

    function agacKur() {
        secili = null;
        dugmeleriAyarla();
        agac = Sgb.agac('#agacYer', {
            yukle: yerleriOku,
            ilkAcik: true,
            simge: function (d) { return d.kok ? 'glyphicon-tasks' : (d.altVar ? 'glyphicon-folder-close' : 'glyphicon-map-marker'); },
            secilince: yerSecildi,
            seciliMi: function (d) { return secili && secili.kod === d.kod; },
            hata: hataGoster
        });
    }

    function dugmeleriAyarla() {
        $('#btnYeni').prop('disabled', !secili);
        $('#btnTasi').prop('disabled', !secili || secili.kok);
    }

    function yerSecildi(d) {
        secili = d;
        dugmeleriAyarla();
        if (d.kok) { formGizle(); return; }
        Sgb.al('api/yerleskeoda/oku', { kod: d.kod }).done(function (veri) {
            if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
            if (secili === d) formaYaz(veri.kayit);
        }).fail(hataGoster);
    }

    // Kaydet / taşı sonrası: üst yerin altı yeniden okunur (kökteki yerlerin üstü -1 ya da 0).
    function ustuYenile(ustKod) {
        agac.altiniYenile(ustKod > 0 ? ustKod : -1);
    }

    // ---------------------------------------------------------------- form

    function formGizle() {
        kayit = null;
        $('#frmYer').prop('hidden', true);
        $('#ipucu').prop('hidden', false);
        $('#btnKaydet, #btnDosya').prop('disabled', true);
    }

    function alanGoster() {
        var tur = sayi($('#cmbYerTur').val());
        $('#grpIl').prop('hidden', tur !== 1);
        $('#grpBirim').prop('hidden', !(tur === 6 && !tanim.merkezBankasi));
    }
    $('#cmbYerTur').on('change', alanGoster);

    function ilSec(kod) {
        var $c = $('#cmbIl');
        if (kod && !$c.find('option').filter(function () { return this.value === kod; }).length)
            $('<option></option>').val(kod).text(kod).appendTo($c);
        $c.val(kod || '');
    }

    // Eski EkranaYaz.
    function formaYaz(k) {
        kayit = k;
        $('#txtUstAd').val(k.ustBilgi);
        $('#txtTarif').val(k.tarif);
        $('#txtGorKod').val(k.gorKod);
        $('#txtAd').val(k.ad);
        $('#cmbYerTur').val(String(k.yerTurKod || 0));
        $('#cmbDurum').val(String(k.durum === 3 ? 3 : 1));
        $('#txtBirimKodu').val(k.birimKod);
        $('#txtBirimAd').val(k.orgKod > 0 ? k.orgAd : '');
        ilSec(k.ilKod);
        $('#txtMaliyetMerkezi').val(k.maliyetMerkezi || '');
        $('#txtSiraNo').val(k.siraNo || '');
        alanGoster();
        $('#frmYer').prop('hidden', false);
        $('#ipucu').prop('hidden', true);
        $('#btnKaydet, #btnDosya').prop('disabled', false);
    }

    // Eski EkrandanAl (gizli alanlar kayıttan).
    function formOku() {
        return {
            kod: kayit.kod > 0 ? kayit.kod : 0,
            ustKod: kayit.ustKod,
            ustGorKod: kayit.ustGorKod,
            ustTarif: kayit.ustTarif,
            gorKod: $.trim($('#txtGorKod').val()),
            ad: $.trim($('#txtAd').val()),
            saymanlikKod: kayit.saymanlikKod,
            birimKod: $.trim($('#txtBirimKodu').val()),
            yerTurKod: sayi($('#cmbYerTur').val()),
            odaTurKod: kayit.odaTurKod,
            ilKod: $('#cmbIl').val() || '',
            orgKod: kayit.orgKod,
            durum: sayi($('#cmbDurum').val()),
            maliyetMerkezi: sayi($('#txtMaliyetMerkezi').val()),
            siraNo: sayi($('#txtSiraNo').val())
        };
    }

    $('#btnYeni').on('click', function () {
        if (!secili) return;
        Sgb.al('api/yerleskeoda/yeni', { ust: secili.kod, birimKod: secili.birimKod || '' }).done(function (veri) {
            if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
            if (veri.uyari) mesaj(veri.uyari, 'warning');
            formaYaz(veri.kayit);
            $('#txtGorKod').trigger('focus');
        }).fail(hataGoster);
    });

    $('#btnKaydet').on('click', function () {
        if (!kayit) return;
        var istek = formOku();
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/yerleskeoda/kaydet', istek).done(function (s) {
            if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
            mesaj(s.bilgi, 'success');
            kayit.kod = s.kod;
            kayit.tarif = istek.ustTarif + (istek.ustTarif ? ', ' : '') + istek.ad;
            $('#txtTarif').val(kayit.tarif);
            ustuYenile(istek.ustKod);
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Dosya Yükle (eski fuDosya_FileSelected)

    $('#btnDosya').on('click', function () { if (kayit) $('#dosyaSec').val('').trigger('click'); });

    $('#dosyaSec').on('change', function () {
        var dosya = this.files[0];
        if (!dosya || !kayit) return;
        var f = formOku();
        var veri = new FormData();
        $.each(['kod', 'ustKod', 'ustGorKod', 'ustTarif', 'gorKod', 'ad', 'yerTurKod'], function (i, ad) { veri.append(ad, String(f[ad] == null ? '' : f[ad])); });
        veri.append('dosya', dosya);
        var $d = $('#btnDosya').prop('disabled', true);
        var ustKod = f.kod > 0 ? f.kod : -1;
        $.ajax({
            url: Sgb.KOK + 'api/yerleskeoda/yukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (s) {
            mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
            agac.altiniYenile(ustKod);
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else hataGoster(xhr);
        }).always(function () {
            $d.prop('disabled', false);
            $('#dosyaSec').val('');
        });
    });

    // ---------------------------------------------------------------- ağaçtan seçim penceresi (birim / taşınacak yer)

    var dlgSec = document.getElementById('dlgSec');
    var secimDugum = null, secimTamam = null;

    function secimAc(baslik, yukle, tamam) {
        secimDugum = null;
        secimTamam = tamam;
        $('#dlgSecBaslik').text(baslik);
        $('#btnSecTamam').prop('disabled', true);
        Sgb.agac('#agacSec', {
            yukle: yukle,
            ilkAcik: true,
            secilince: function (d) { secimDugum = d; $('#btnSecTamam').prop('disabled', false); },
            hata: hataGoster
        });
        dlgSec.showModal();
    }

    $('#btnSecTamam').on('click', function () {
        if (!secimDugum || !secimTamam) return;
        dlgSec.close();
        secimTamam(secimDugum);
    });

    // Odanın birimi: eski ddlBirim (seçilince birim kodu alanına birimin muhasebe birimi yazılırdı).
    $('#btnBirimSec').on('click', function () {
        secimAc('Birim Seçimi', function (ust) {
            return Sgb.al('api/yerleskeoda/birimler', { ust: ust ? ust.kod : 0 }).then(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return []; }
                return veri.satirlar || [];
            });
        }, function (d) {
            kayit.orgKod = d.kod;
            kayit.orgAd = d.ad;
            $('#txtBirimAd').val(d.ad);
            $('#txtBirimKodu').val(d.muhasebeBirim || '');
        });
    });
    $('#btnBirimTemizle').on('click', function () {
        if (!kayit) return;
        kayit.orgKod = 0;
        kayit.orgAd = '';
        $('#txtBirimAd').val('');
    });

    // Taşı: eski sürükle-bırak; yeni üst yerin birim kodu da gönderilir.
    $('#btnTasi').on('click', function () {
        if (!secili || secili.kok) return;
        var tasinan = secili;
        secimAc('"' + tasinan.ad + '" nereye taşınsın?', yerleriOku, function (hedef) {
            if (hedef.kod === tasinan.kod) { mesaj('Bir yer kendi altına taşınamaz.', 'warning'); return; }
            Sgb.gonder('api/yerleskeoda/tasi', { kod: tasinan.kod, ustKod: hedef.kod, birimKod: hedef.birimKod || '' }).done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                formGizle();
                agacKur();
            }).fail(hataGoster);
        });
    });

    // ---------------------------------------------------------------- Yazdır, Pasifleri de Listele

    $('#btnYazdir').on('click', function () {
        var $d = $(this).prop('disabled', true);
        var p = tanim.merkezBankasi && secili && secili.birimKod ? { birimKod: secili.birimKod } : null;
        Sgb.indir('api/yerleskeoda/rapor', p).then(function () { $d.prop('disabled', false); });
    });

    $('#btnPasifler').on('click', function () {
        pasifler = !pasifler;
        $(this).attr('aria-pressed', pasifler ? 'true' : 'false').toggleClass('active', pasifler);
        formGizle();
        agacKur();
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/yerleskeoda/tanim').done(function (veri) {
        if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
        tanim = veri;
        var $il = $('#cmbIl').empty().append('<option value=""></option>');
        $.each(tanim.iller || [], function (i, il) { $('<option></option>').val(il.kod).text(il.ad).appendTo($il); });
        $('[data-mb], #btnPasifler').prop('hidden', !tanim.merkezBankasi);
        $('#txtGorKod').attr('maxlength', tanim.merkezBankasi ? 50 : 3);
        agacKur();
    }).fail(hataGoster);
});
