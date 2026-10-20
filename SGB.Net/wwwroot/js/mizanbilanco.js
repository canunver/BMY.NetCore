// Muhasebe Raporları (ButceMuhasebe/MizanBilanco; eski MizanBilanco.aspx + ModulScripts/MizanBilanco.js). Solda rapor listesi,
// sağda seçilen raporun kuralları: her raporda yalnızca eski RaporBilgi tablosunda görünen alanlar açılır (eski
// KontrolleriGosterGizle). Yazdır raporu indirir (eski btnListele_Click), Temizle alanları ilk değerlerine döndürür.
// Uçlar: /api/muhrapor/* (Muhasebe/MizanBilancoUclari.cs). Ortak bileşenler: Sgb.muhasebeBirim, Sgb.hesapKodu, Sgb.firmaSec,
// Sgb.pencere (Açıklama Listesi: TanimAciklama?tip=2).
$(function () {
    'use strict';

    var $kok = $('.mizan-bilanco');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/muhrapor/';
    var SAKLA = 'mizanbilanco.';
    var AYLAR = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
    var tanim = null;
    var secili = null; // seçili rapor { RAPORKODU, RAPORADI, gizli }

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function sakliOku(ad) { try { return localStorage.getItem(SAKLA + ad); } catch (e) { return null; } }
    function sakliYaz(ad, deger) { try { localStorage.setItem(SAKLA + ad, deger); } catch (e) { /* özel pencere */ } }
    function iki(n) { return (n < 10 ? '0' : '') + n; }

    $('#btnYardim').on('click', function () {
        var $p = $('#pnlYardim');
        var ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false');
    });

    // ---------------------------------------------------------------- alanlar

    function yil() { return parseInt($('#txtYil').val(), 10) || new Date().getFullYear(); }

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: yil,
        kurumsalGizle: true
    });

    var hesap1 = Sgb.hesapKodu('#txtHesap1', { yil: yil, ad: true, uc: UC + 'hesap/' });
    var hesap2 = Sgb.hesapKodu('#txtHesap2', { yil: yil, ad: true, uc: UC + 'hesap/' });

    // Eski YilDegistir: yıl değişince tarih aralığı o yılın başı / sonu olur.
    function tarihleriYilaGore() {
        var y = yil();
        $('#txtBasTarih').val(y + '-01-01');
        $('#txtBitTarih').val(y + '-12-31');
    }
    $('#txtYil').on('change', tarihleriYilaGore);

    // Eski chkHesapAdiniSec Check: işaretliyse bitiş hesap kodu gizlenir.
    $('#chkHesapAdiniSec').on('change', function () {
        if (secili && !secili.gizli.chkHesapAdiniSec) $('#grpBitisHesap').prop('hidden', this.checked);
    });

    // Fiş türü (eski MultiCombo mcFisTur): çoklu seçim kutuları.
    function fisTurYaz(degerler) {
        var s = String(degerler || '').split(',').map(function (x) { return $.trim(x); });
        $('#lstFisTur input').each(function () { this.checked = s.indexOf(this.value) >= 0; });
    }
    function fisTurOku() {
        return $('#lstFisTur input:checked').map(function () { return this.value; }).get().join(',');
    }

    // ---------------------------------------------------------------- görünürlük (eski KontrolleriGosterGizle)

    function gorunurlukUygula() {
        if (!secili) return;
        var g = secili.gizli || {};
        var kod = secili.RAPORKODU;
        var ek = {
            // eski: rapor 4'te hesap adı seçimi işaretli değilse bitiş hesap kodu görünür
            cmpBitisHesapKodu: kod === '4' ? $('#chkHesapAdiniSec').prop('checked') : g.cmpBitisHesapKodu,
            // eski: muhasebede döviz yoksa Döviz Tür hep gizli
            ddlListeTur: tanim && !tanim.dovizVar ? true : g.ddlListeTur,
            // eski: Stopaj Oran yalnızca 55 ve 61'de
            sfStopajOran: !(kod === '55' || kod === '61')
        };
        $kok.find('[data-alan]').each(function () {
            var adlar = $(this).attr('data-alan').split(' ');
            var gizli = true;
            $.each(adlar, function (i, ad) {
                var h = ek.hasOwnProperty(ad) ? ek[ad] : g[ad];
                if (h === false) gizli = false;
            });
            $(this).prop('hidden', gizli);
        });
    }

    function raporSec(r, ilkAcilis) {
        secili = r;
        $('#lstRapor li').each(function () {
            var s = $(this).attr('data-kod') === r.RAPORKODU;
            $(this).toggleClass('secili', s).attr('aria-selected', s ? 'true' : 'false');
        });
        $('#lblRaporBaslik').text(r.RAPORADI + ' Raporu - Rapor Kuralları');
        // eski: 44 Mizan (Geçici) fiş türü "0, 1", 45 Mizan (Kesin) "0, 1, 2, 3"
        if (r.RAPORKODU === '44') fisTurYaz('0,1');
        else if (r.RAPORKODU === '45') fisTurYaz('0,1,2,3');
        gorunurlukUygula();
        if (!ilkAcilis) sakliYaz('rapor', r.RAPORKODU);
    }

    function listeCiz() {
        var $l = $('#lstRapor').empty();
        $.each(tanim.raporlar, function (i, r) {
            $('<li role="option" tabindex="0"></li>').attr('data-kod', r.RAPORKODU)
                .append($('<span class="mzb-no"></span>').text(i + 1))
                .append($('<span class="mzb-ad"></span>').text(r.RAPORADI))
                .data('rapor', r).appendTo($l);
        });
    }

    $('#lstRapor').on('click', 'li', function () { raporSec($(this).data('rapor')); });
    $('#lstRapor').on('keydown', 'li', function (e) {
        if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); raporSec($(this).data('rapor')); }
        else if (e.key === 'ArrowDown') { e.preventDefault(); $(this).nextAll('li:visible').first().trigger('focus'); }
        else if (e.key === 'ArrowUp') { e.preventDefault(); $(this).prevAll('li:visible').first().trigger('focus'); }
    });
    $('#txtRaporAra').on('input', function () {
        var a = $.trim($(this).val()).toLocaleLowerCase('tr-TR');
        $('#lstRapor li').each(function () {
            $(this).prop('hidden', a !== '' && $(this).text().toLocaleLowerCase('tr-TR').indexOf(a) < 0);
        });
    });

    // ---------------------------------------------------------------- Temizle (eski Temizle)

    function temizle() {
        var y = new Date().getFullYear();
        muhBirim.ata({ muhasebe: muhBirim.deger().muhasebe || (tanim ? tanim.muhasebe : ''), birim: '' });
        $('#txtYil').val(String(y));
        $('#txtBasTarih').val(y + '-01-01');
        $('#txtBitTarih').val(y + '-12-31');
        hesap1.ata('100');
        hesap2.ata('999');
        $('#ddlSeviye').val('1');
        $('#ddlTur').val('3');
        fisTurYaz('0,1');
        $('#ddlListeTur').val('1');
    }
    $('#btnTemizle').on('click', function () { temizle(); gorunurlukUygula(); });

    // ---------------------------------------------------------------- seçim pencereleri

    // Eski FirmaBilgisiAc / FirmaBilgisiGoster: alt hesap "vergiNo - ad"
    $('#btnAltHesapSec').on('click', function () {
        Sgb.firmaSec({
            muhasebe: muhBirim.deger().muhasebe || 0,
            adres: UC + 'firma/ara',
            ara: $.trim(($('#txtAltHesap').val() || '').split('-')[0]),
            secilince: function (f) { $('#txtAltHesap').val(f.vergiNo + ' - ' + f.ad); }
        });
    });

    // Eski TanimAciklamaAc / AciklamaBilgiYaz
    window.AciklamaBilgiYaz = function (aciklama) { $('#txtAciklama').val(aciklama); };
    $('#btnAciklama').on('click', function () {
        var d = muhBirim.deger();
        Sgb.pencere('TanimAciklama?' + $.param({ muhasebe: d.muhasebe || '', birim: d.birim || '', tip: 2 }), 'Açıklama Listesi');
    });

    // ---------------------------------------------------------------- Yazdır (eski btnListele_Click)

    function hesapDegeri(id) { return $.trim(($('#' + id).val() || '').split('-')[0]); }

    $('#btnYazdir').on('click', function () {
        if (!secili) { mesaj('Raporu seçiniz.', 'warning'); return; }
        var d = muhBirim.deger();
        var p = {
            raporTur: secili.RAPORKODU,
            yil: String(yil()),
            ay: $('#ddlAy').val() || '',
            muhasebe: d.muhasebe ? String(d.muhasebe) : $.trim($('#txtMuhasebe').val()),
            birim: d.birim ? String(d.birim) : $.trim($('#txtBirim').val()),
            kurKod: $.trim($('#txtKurKod').val()),
            fonKod: $.trim($('#txtFonKod').val()),
            basTarih: $('#txtBasTarih').val(),
            bitTarih: $('#txtBitTarih').val(),
            basHesap: hesapDegeri('txtHesap1'),
            bitHesap: hesapDegeri('txtHesap2'),
            altHesap: $.trim($('#txtAltHesap').val()),
            tur: $('#ddlTur').val() || '',
            seviye: $('#ddlSeviye').val() || '',
            fisTur: fisTurOku(),
            listeTur: $('#ddlListeTur').val() || '',
            sayistay: $('#ddlSayistay').val() || '',
            raporSira: $('#ddlRaporSira').val() || '',
            hesapAdiAyni: $('#chkHesapAdiniSec').prop('checked') ? 1 : 0,
            tertipGosterme: $('#chkTertipGosterme').prop('checked') ? 1 : 0,
            tertipGostermeHesaplar: $.trim($('#txtTertipGosterme').val()),
            bakiyeSifir: $('#chkBakiyeSifir').prop('checked') ? 1 : 0,
            programButceTertip: $('#chkProgramButceTertip').prop('checked') ? 1 : 0,
            aciklama: $('#txtAciklama').val() || '',
            kesintiRaporTur: $('#ddlFirmaRaporTur').val() || '',
            stopajOran: $('#txtStopajOran').val() || '0',
            imzaAdi: $.trim($('#txtImzaAdi').val()),
            imzaBirim: $.trim($('#txtImzaBirim').val()),
            imzaUnvan: $.trim($('#txtImzaUnvan').val())
        };
        var $b = $(this).prop('disabled', true);
        var $s = $b.find('.glyphicon').removeClass('glyphicon-print').addClass('glyphicon-hourglass');
        Promise.resolve(Sgb.indir(UC + 'yazdir', p)).finally(function () {
            $b.prop('disabled', false);
            $s.removeClass('glyphicon-hourglass').addClass('glyphicon-print');
        });
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    var $ay = $('#ddlAy');
    $.each(AYLAR, function (i, a) { $('<option></option>').val(String(i + 1)).text(a).appendTo($ay); });
    $ay.val(String(new Date().getMonth() + 1));

    Sgb.al(UC + 'tanim').done(function (t) {
        if (t.hata) { mesaj(t.hata, 'danger'); return; }
        tanim = t;

        var yillar = [];
        for (var y = t.yilIlk; y <= t.yilSon; y++) yillar.push(y);
        Sgb.yilKutusu('#txtYil', yillar, new Date().getFullYear());

        var $f = $('#lstFisTur').empty();
        $.each(t.fisTurleri, function (i, f) {
            $('<label class="mzb-secenek"></label>')
                .append($('<input type="checkbox" />').val(String(f.kod)))
                .append(document.createTextNode(' ' + f.ad)).appendTo($f);
        });

        $('#ddlListeTur').empty()
            .append($('<option value="1"></option>').text(t.paraBirimi || 'TL'))
            .append('<option value="2">Döviz</option>');

        muhBirim.ata({ muhasebe: t.muhasebe || '', birim: '' });
        temizle();
        $('#ddlSeviye').val(String(t.seviye || 1));
        $('#txtTertipGosterme').val(t.tertipGosterme || '');
        $('#txtImzaAdi').val(t.imzaAdi || '');
        $('#txtImzaBirim').val(t.imzaBirim || '');
        $('#txtImzaUnvan').val(t.imzaUnvan || '');

        listeCiz();
        // eski: ilk açılışta Mizan (1) seçili; son seçilen rapor tarayıcıda hatırlanır
        var son = sakliOku('rapor');
        var r = null;
        $.each(t.raporlar, function (i, x) { if (x.RAPORKODU === son) r = x; });
        raporSec(r || t.raporlar[0], true);
    }).fail(hataGoster);
});
