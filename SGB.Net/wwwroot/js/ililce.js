// İl ve İlçe Tanımı (ButceHazirlikv2/IlIlceTanimla; eski IlIlceTanimla.aspx + IlTanimlaDetay.aspx + IlceTanimlaDetay.aspx).
// Solda iller (Sgb.agac; ilçeler il açılınca okunur), sağda seçilen il ya da ilçenin formu. Açılışta ilk il açılır ve seçilir.
// Önceki / Sonraki listede görünen sıraya göre bir önceki / sonraki kaydı seçer. Kod girilir: aynı kodla kaydetmek günceller.
$(function () {
    'use strict';

    if (!$('#agcIlIlce').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    // Formdaki kayıt: { tur: 'il' | 'ilce', ilKod, ilAd, kod (0: yeni), ... }
    var secili = { tur: 'il', kod: 0 };
    var ilkSecildi = false;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) {
        $('#btnYeniIl, #btnYeniIlce, #btnKaydet').prop('disabled', evet);
        $('#btnSil').prop('disabled', evet || !secili.kod);
    }

    // ---------------------------------------------------------------- ağaç

    var agac = Sgb.agac(document.getElementById('agcIlIlce'), {
        ilkAcik: true,
        bosMetin: 'Tanımlı il yok.',
        yukle: function (ust) {
            if (!ust) {
                return Sgb.al('api/ililce/iller').then(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                    return $.map(v.iller || [], function (il) {
                        return { tur: 'il', kod: il.kod, ad: il.kod + '-' + il.ad, adSade: il.ad, duzeyKod: il.duzeyKod, altVar: true };
                    });
                });
            }
            return Sgb.al('api/ililce/ilceler', { il: ust.kod }).then(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                return $.map(v.ilceler || [], function (c) {
                    return { tur: 'ilce', kod: 'c' + c.kod, ilceKod: c.kod, ad: c.kod + '-' + c.ad, adSade: c.ad, ilKod: ust.kod, ilAd: ust.ad, altVar: false };
                });
            }).done(function () {
                // Eski sayfa açılışta listenin ilk satırını (ilk ili) gösterirdi.
                if (!ilkSecildi) { ilkSecildi = true; $('#agcIlIlce > li:first-child > .satir > .sec').trigger('click'); }
            });
        },
        seciliMi: function (d) {
            return d.tur === secili.tur && (d.tur === 'il' ? d.kod === secili.kod : d.ilceKod === secili.kod && d.ilKod === secili.ilKod);
        },
        secilince: goster,
        hata: hataGoster
    });

    // ---------------------------------------------------------------- form

    function formKur(tur) {
        var ilce = tur === 'ilce';
        $('#formBaslik').text(ilce ? 'İlçe Tanımlama' : 'İl Tanımlama');
        $('#kutuForm').toggleClass('ilce-formu', ilce);
        $('#grpIl').prop('hidden', !ilce);
        $('#grpDuzey').prop('hidden', ilce);
    }

    function goster(d) {
        if (d.tur === 'il') {
            secili = { tur: 'il', kod: d.kod, ilKod: d.kod, ilAd: d.ad };
            $('#txtKod').val(d.kod);
            $('#txtAd').val(d.adSade || '');
            $('#txtDuzeyKod').val(d.duzeyKod || '');
            $('#txtIl').val('');
        } else {
            secili = { tur: 'ilce', kod: d.ilceKod, ilKod: d.ilKod, ilAd: d.ilAd };
            $('#txtKod').val(d.ilceKod);
            $('#txtAd').val(d.adSade || '');
            $('#txtDuzeyKod').val('');
            $('#txtIl').val(d.ilAd);
        }
        formKur(secili.tur);
        $('#btnSil').prop('disabled', false);
    }

    function yeni(tur, ilKod, ilAd) {
        secili = { tur: tur, kod: 0, ilKod: ilKod || 0, ilAd: ilAd || '' };
        $('#txtKod, #txtAd, #txtDuzeyKod').val('');
        $('#txtIl').val(secili.ilAd);
        formKur(tur);
        $('#btnSil').prop('disabled', true);
        $('#agcIlIlce .sec').removeAttr('aria-current');
        $('#txtKod').trigger('focus');
    }

    $('#btnYeniIl').on('click', function () { yeni('il'); });

    // Eski YeniIlce: seçili il (ya da seçili ilçenin ili) yoksa uyarır.
    $('#btnYeniIlce').on('click', function () {
        if (!secili.ilKod) { mesaj('Yeni ilçe kaydedebilmek için lütfen il oluşturunuz', 'warning'); return; }
        yeni('ilce', secili.ilKod, secili.ilAd);
    });

    // Kaydedilen / silinen kaydın yeri ağaçta yenilenir: il için bütün liste, ilçe için ilin altı.
    function agacYenile() {
        if (secili.tur === 'ilce') agac.altiniYenile(secili.ilKod);
        else agac.yenile();
    }

    $('#btnKaydet').on('click', function () {
        var kod = parseInt($('#txtKod').val(), 10) || 0;
        var ad = $.trim($('#txtAd').val());
        if (kod <= 0) { mesaj('Kodu yazınız.', 'warning'); $('#txtKod').trigger('focus'); return; }
        if (!ad) { mesaj('Adı yazınız.', 'warning'); $('#txtAd').trigger('focus'); return; }
        var ilce = secili.tur === 'ilce';
        var istek = ilce ? { ilKod: secili.ilKod, kod: kod, ad: ad } : { kod: kod, ad: ad, duzeyKod: $.trim($('#txtDuzeyKod').val()) };
        mesgul(true);
        Sgb.gonder(ilce ? 'api/ililce/ilce-kaydet' : 'api/ililce/il-kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                secili.kod = kod;
                if (!ilce) { secili.ilKod = kod; secili.ilAd = kod + '-' + ad; }
                mesaj(s.bilgi, 'success');
                agacYenile();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnSil').on('click', function () {
        if (!secili.kod) return;
        var ilce = secili.tur === 'ilce';
        if (!window.confirm(ilce ? 'Seçili ilçe silinsin mi?' : 'Seçili il silinsin mi?')) return;
        var istek = ilce ? { ilKod: secili.ilKod, kod: secili.kod, ad: $.trim($('#txtAd').val()) }
            : { kod: secili.kod, ad: $.trim($('#txtAd').val()), duzeyKod: $.trim($('#txtDuzeyKod').val()) };
        mesgul(true);
        Sgb.gonder(ilce ? 'api/ililce/ilce-sil' : 'api/ililce/il-sil', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                var ilKod = secili.ilKod, ilAd = secili.ilAd;
                if (ilce) { yeni('ilce', ilKod, ilAd); agac.altiniYenile(ilKod); }
                else { yeni('il'); agac.yenile(); }
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // ---------------------------------------------------------------- Önceki / Sonraki (eski Onceki(fpL) / Sonraki(fpL))

    function komsu(yon) {
        var $hepsi = $('#agcIlIlce .sec:visible');
        if (!$hepsi.length) return;
        var i = $hepsi.index($hepsi.filter('[aria-current="true"]'));
        var j = i < 0 ? 0 : i + yon;
        if (j < 0 || j >= $hepsi.length) return;
        var $s = $hepsi.eq(j);
        $s.trigger('click');
        $s[0].scrollIntoView({ block: 'nearest' });
    }

    $('#btnOnceki').on('click', function () { komsu(-1); });
    $('#btnSonraki').on('click', function () { komsu(1); });
    $('#btnYenile').on('click', function () { agac.yenile(); });

    formKur('il');
});
