// Organizasyon Şeması (Organizasyon/ORGAna): birim ağacı (Sgb.agac), seçili birimin bilgileri ve Görevler sekmesi.
// Birim özelliklerine (harcama, bütçe, evrak ...) göre formun ilgili bölümleri görünür; yalnızca görünen bölümler kaydedilir.
$(function () {
    'use strict';

    if (!$('#agacBirim').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var sablon = new URLSearchParams(window.location.search).get('sablon') === '1' ? '1' : '';
    var tanim = { yazabilir: false, gorevAtayabilir: false, birimTurleri: [], unvanlar: [], ulkeler: [] };
    var birim = null;          // formda açık birim: { kod (yeni birimde 0), anaBirimKod, ad }
    var merkez = { kod: 0, ad: '' };
    var harcamaKodlari = [];   // [{ kurKod, kurAd, muhasebeBirimKod, muhasebeBirimAd, fonKod, fonAd }]
    var kisiler = {};          // yonetici / ust1 / ust2 -> { tc, ad }: unvana atanmış kişi
    var agac = null;

    var TIPLER = {
        harcama: ['glyphicon-credit-card', 'Harcama Birimi'], butce: ['glyphicon-stats', 'Bütçe Birimi'], tasinir: ['glyphicon-briefcase', 'Taşınır Birimi'],
        personel: ['glyphicon-user', 'Personel Birimi'], evrak: ['glyphicon-file', 'Evrak Birimi'], cografya: ['glyphicon-map-marker', 'Coğrafi Birim'],
        grup: ['glyphicon-folder-open', 'Grup']
    };
    var YONETICILER = { yonetici: ['#ddlYonetici', '#kisiYonetici'], ust1: ['#ddlUstYonetici1', '#kisiUstYonetici1'], ust2: ['#ddlUstYonetici2', '#kisiUstYonetici2'] };
    var METIN_ALANLARI = {
        ad: '#txtAd', entegreKod: '#txtEntegreKod', kisaAd: '#txtKisaAd', kurumKod: '#txtKurumKod', turKod: '#ddlBirimTur', muhasebe: '#txtMuhasebe',
        merkezKurKod: '#txtMerkezKur', tasraKurKod: '#txtTasraKur', yurtDisiKurKod: '#txtYurtDisiKur', faaliyetKodlari: '#txtFaaliyet',
        ambarKurKod: '#txtAmbarKur', ambarBirimKod: '#txtAmbarBirim', ambarKod: '#txtAmbarKod', vergiNo: '#txtVergiNo', vergiDairesi: '#txtVergiDairesi',
        evrakBirim: '#txtEvrakBirim', antet: '#txtAntet', adres: '#txtAdres', telefon: '#txtTelefon', faks: '#txtFaks', eposta: '#txtEposta', url: '#txtUrl'
    };
    var OZELLIKLER = {
        yonetici: '#chkYonetici', kurum: '#chkKurum', harcama: '#chkHarcama', butce: '#chkButce', tasinir: '#chkTasinir', ambar: '#chkAmbar',
        personel: '#chkPersonel', evrak: '#chkEvrak', iletisim: '#chkIletisim', cizimCikisNoktasi: '#chkCizimCikis', cizimdeGorunme: '#chkCizimdeGorunme'
    };

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function secili(kutu) { return $(kutu).prop('checked'); }
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- formun görünen bölümleri

    function alanGoster() {
        var cografya = $('#ddlCografya').val();
        var kosullar = {
            kurum: secili('#chkKurum'), yonetici: secili('#chkYonetici'), butce: secili('#chkButce'), ambar: secili('#chkAmbar'),
            harcama: secili('#chkHarcama'), evrak: secili('#chkEvrak'), iletisim: secili('#chkIletisim'), cografya: !!cografya,
            muhasebe: secili('#chkHarcama') || secili('#chkTasinir') || secili('#chkAmbar'),
            harcamaKod: secili('#chkHarcama') || secili('#chkTasinir')
        };
        $('#frmBirim [data-kosul]').each(function () { $(this).prop('hidden', !kosullar[$(this).attr('data-kosul')]); });
        $('#frmBirim [data-cografya]').each(function () { $(this).prop('hidden', $(this).attr('data-cografya').split(' ').indexOf(cografya) < 0); });
        // Üst yöneticiler yalnızca bir üst birimi olan birimlerde tanımlanır.
        $('#frmBirim [data-yonetici^="ust"]').prop('hidden', !birim || !(birim.anaBirimKod > 0));
    }

    // Ambar, harcama ya da taşınır birimiyle birlikte seçilemez.
    $('#chkHarcama, #chkTasinir').on('change', function () { if (this.checked) $('#chkAmbar').prop('checked', false); });
    $('#chkAmbar').on('change', function () { if (this.checked) $('#chkHarcama, #chkTasinir').prop('checked', false); });
    $('#frmBirim .tanim-secenekler input, #ddlCografya').on('change', alanGoster);

    // ---------------------------------------------------------------- yöneticiler

    function kisiCiz(hangi) {
        var $kutu = $(YONETICILER[hangi][1]).empty();
        var k = kisiler[hangi];
        if (!k || !k.tc) return;
        $('<span></span>').text(k.tc + (k.ad ? ' (' + k.ad + ')' : '')).appendTo($kutu);
        if (tanim.yazabilir)
            $('<button type="button" class="btn btn-default btn-xs">&times;</button>').attr('aria-label', 'Kişiyi kaldır').attr('title', 'Kişiyi kaldır')
                .css('margin-left', '6px').on('click', function () { kisiler[hangi] = null; kisiCiz(hangi); }).appendTo($kutu);
    }

    function unvanSec($ddl, kod, ad) {
        // Kayıtlı unvan listede yoksa (liste okunamadıysa) seçenek olarak eklenir ki değer kaybolmasın.
        if (kod > 0 && !$ddl.find('option').filter(function () { return this.value === String(kod); }).length)
            $('<option></option>').val(kod).text(ad || kod).appendTo($ddl);
        $ddl.val(String(kod || 0));
    }

    // Unvan değişince o unvana atanmış kişi kalkar (kişi seçimi Personel ekranlarıyla birlikte taşınacak).
    $.each(YONETICILER, function (hangi, alan) {
        $(alan[0]).on('change', function () { kisiler[hangi] = null; kisiCiz(hangi); });
    });

    // ---------------------------------------------------------------- muhasebe kodu ve kod aramaları

    $('#txtMuhasebe').on('change', function () {
        var kod = $.trim(this.value);
        $('#lblMuhasebeAd').text('');
        if (!kod) return;
        Sgb.al('api/orgbirim/muhasebe', { kod: kod }).done(function (s) {
            if ($.trim($('#txtMuhasebe').val()) === kod) $('#lblMuhasebeAd').text(s.ad || 'Bu kodla muhasebe bulunamadı.');
        });
    });

    var kodAdlari = {}; // "tur:kod" -> ad (eklenen harcama kodunun adını yazmak için)
    var aramaZamanlayici;

    function onerileriDoldur(veri, onEk) {
        var $liste = $('#dlKodlar').empty();
        if (!veri || veri.hata) return;
        $.each(veri.satirlar, function (i, s) {
            kodAdlari[onEk + s.kod] = s.ad || '';
            $('<option></option>').val(s.kod).text(s.ad || '').appendTo($liste);
        });
    }

    // Kurumsal / fonksiyonel kod: yazılan değerle başlayan kodlar önerilir.
    $('#frmBirim').on('focus input', '.kod-ara', function () {
        var $alan = $(this).attr('list', 'dlKodlar'), tur = $alan.attr('data-tur');
        clearTimeout(aramaZamanlayici);
        aramaZamanlayici = setTimeout(function () {
            Sgb.al('api/orgbirim/kodlar', { tur: tur, q: $.trim($alan.val()) }).done(function (veri) { onerileriDoldur(veri, tur + ':'); });
        }, 250);
    });

    // Harcama birimi: muhasebenin (ve yazıldıysa kurumsal kodun) birimleri, ada ya da koda göre.
    function birimAramasi(alan, kurumAlani) {
        $(alan).on('focus input', function () {
            var $alan = $(this).attr('list', 'dlKodlar');
            clearTimeout(aramaZamanlayici);
            aramaZamanlayici = setTimeout(function () {
                var muhasebe = $.trim($('#txtMuhasebe').val());
                if (!muhasebe) { $('#dlKodlar').empty(); return; }
                Sgb.al('api/orgbirim/muhasebe-birim', { muhasebe: muhasebe, kurum: $.trim($(kurumAlani).val()), q: $.trim($alan.val()) })
                    .done(function (veri) { onerileriDoldur(veri, 'mb:'); });
            }, 250);
        });
    }
    birimAramasi('#txtHkBirim', '#txtHkKur');
    birimAramasi('#txtAmbarBirim', '#txtAmbarKur');

    // ---------------------------------------------------------------- harcama kodları

    function kodYazi(kod, ad) { return kod ? kod + (ad ? ' (' + ad + ')' : '') : ''; }

    function harcamaKodlariCiz() {
        var $liste = $('#lstHarcamaKod').empty();
        if (!harcamaKodlari.length) {
            $('<li></li>').append($('<span class="ad bos"></span>').text('Harcama kodu eklenmedi.')).appendTo($liste);
            return;
        }
        $.each(harcamaKodlari, function (i, h) {
            var $li = $('<li></li>').appendTo($liste);
            var $ad = $('<span class="ad"></span>').appendTo($li);
            $('<span></span>').text(kodYazi(h.kurKod, h.kurAd)).appendTo($ad);
            $('<span></span>').text(kodYazi(h.muhasebeBirimKod, h.muhasebeBirimAd)).appendTo($ad);
            $('<span></span>').text(kodYazi(h.fonKod, h.fonAd)).appendTo($ad);
            if (tanim.yazabilir)
                $('<button type="button" class="btn btn-danger btn-xs"><span class="glyphicon glyphicon-trash" aria-hidden="true"></span></button>')
                    .attr('aria-label', 'Sil: ' + kodYazi(h.kurKod, h.kurAd) + ' ' + kodYazi(h.muhasebeBirimKod, h.muhasebeBirimAd)).attr('title', 'Sil')
                    .on('click', function () { harcamaKodlari.splice(i, 1); harcamaKodlariCiz(); }).appendTo($li);
        });
    }

    $('#btnHkEkle').on('click', function () {
        var h = { kurKod: $.trim($('#txtHkKur').val()), muhasebeBirimKod: $.trim($('#txtHkBirim').val()), fonKod: $.trim($('#txtHkFon').val()) };
        if (!h.kurKod && !h.muhasebeBirimKod && !h.fonKod) { mesaj('Kurumsal kod, harcama birimi ya da fonksiyonel kod yazınız.', 'warning'); return; }
        var var_ = harcamaKodlari.some(function (x) { return x.kurKod === h.kurKod && x.muhasebeBirimKod === h.muhasebeBirimKod && x.fonKod === h.fonKod; });
        if (var_) { mesaj('Bu harcama kodu zaten eklenmiş.', 'warning'); return; }
        h.kurAd = kodAdlari['1:' + h.kurKod] || '';
        h.muhasebeBirimAd = kodAdlari['mb:' + h.muhasebeBirimKod] || '';
        h.fonAd = kodAdlari['2:' + h.fonKod] || '';
        harcamaKodlari.push(h);
        harcamaKodlariCiz();
        $('#txtHkKur, #txtHkBirim, #txtHkFon').val('');
    });

    // ---------------------------------------------------------------- coğrafya (ülke > bölge / il > ilçe)

    function secenekDoldur($ddl, satirlar) {
        $ddl.empty().append($('<option value=""></option>').text('Lütfen Seçiniz.'));
        $.each(satirlar || [], function (i, s) { $('<option></option>').val(s.kod).text(s.ad).appendTo($ddl); });
    }

    function cografyaYukle($ddl, ust, seviye) {
        if (!ust) { secenekDoldur($ddl, []); return $.Deferred().resolve().promise(); }
        return Sgb.al('api/orgbirim/cografya', { ust: ust, seviye: seviye }).done(function (veri) { secenekDoldur($ddl, veri.satirlar); });
    }

    function ulkeDegisti() {
        var ulke = $('#ddlUlke').val();
        secenekDoldur($('#ddlIlce'), []);
        return $.when(cografyaYukle($('#ddlBolge'), ulke, 2), cografyaYukle($('#ddlIl'), ulke, 3));
    }
    $('#ddlUlke').on('change', ulkeDegisti);
    $('#ddlIl').on('change', function () { cografyaYukle($('#ddlIlce'), $(this).val(), 4); });

    function varsayilanUlke() {
        var tr = $.grep(tanim.ulkeler, function (u) { return (u.ad || '').toLocaleLowerCase('tr') === 'türkiye'; })[0];
        return tr ? tr.kod : (tanim.ulkeler[0] || {}).kod || '';
    }

    function cografyaDoldur(b) {
        $('#ddlUlke').val(b.ulke || varsayilanUlke());
        return ulkeDegisti().then(function () {
            $('#ddlBolge').val(b.bolge || '');
            $('#ddlIl').val(b.il || '');
            return cografyaYukle($('#ddlIlce'), b.il || '', 4);
        }).then(function () { $('#ddlIlce').val(b.ilce || ''); });
    }

    // ---------------------------------------------------------------- form

    function yazmaDurumu() {
        var acik = !!birim && tanim.yazabilir;
        $('#alanlar').prop('disabled', !acik);
        $('#btnKaydet').prop('disabled', !acik);
        $('#btnSil, #btnBirimKapat').prop('disabled', !acik || !birim.kod);
        $('#sekmeGorev').prop('disabled', !(birim && birim.kod > 0 && tanim.gorevAtayabilir));
    }

    function formDoldur(b) {
        $.each(METIN_ALANLARI, function (ad, kutu) { $(kutu).val(b[ad] || ''); });
        // Kayıtlı birim türü listede yoksa seçenek olarak eklenir.
        if (b.turKod && $('#ddlBirimTur').val() !== b.turKod) {
            $('<option></option>').val(b.turKod).text(b.turKod + (b.turAd ? ' - ' + b.turAd : '')).appendTo('#ddlBirimTur');
            $('#ddlBirimTur').val(b.turKod);
        }
        $.each(OZELLIKLER, function (ad, kutu) { $(kutu).prop('checked', !!b[ad]); });
        $('#ddlCografya').val(b.cografya || '');
        $('#lblMuhasebeAd').text(b.muhasebeAd || '');

        merkez = { kod: b.merkezKod || 0, ad: b.merkezAd || '' };
        $('#txtMerkezAd').val(merkez.ad);

        unvanSec($('#ddlYonetici'), b.yoneticiKod, b.yoneticiAd);
        unvanSec($('#ddlUstYonetici1'), b.ustYoneticiKod1, b.ustYoneticiAd1);
        unvanSec($('#ddlUstYonetici2'), b.ustYoneticiKod2, b.ustYoneticiAd2);
        kisiler = {
            yonetici: b.yoneticiTc ? { tc: b.yoneticiTc, ad: b.yoneticiTcAd } : null,
            ust1: b.ustYoneticiTc1 ? { tc: b.ustYoneticiTc1, ad: b.ustYoneticiTcAd1 } : null,
            ust2: b.ustYoneticiTc2 ? { tc: b.ustYoneticiTc2, ad: b.ustYoneticiTcAd2 } : null
        };
        $.each(YONETICILER, function (hangi) { kisiCiz(hangi); });

        harcamaKodlari = (b.harcamaKodlari || []).slice();
        harcamaKodlariCiz();
        $('#txtHkKur, #txtHkBirim, #txtHkFon').val('');

        alanGoster();
        yazmaDurumu();
        return cografyaDoldur(b);
    }

    function formOku() {
        var istek = { kod: birim.kod, anaBirimKod: birim.anaBirimKod, sablon: sablon === '1', merkezKod: merkez.kod };
        $.each(METIN_ALANLARI, function (ad, kutu) { istek[ad] = $.trim($(kutu).val() || ''); });
        $.each(OZELLIKLER, function (ad, kutu) { istek[ad] = secili(kutu); });
        istek.cografya = $('#ddlCografya').val();
        istek.yoneticiKod = parseInt($('#ddlYonetici').val(), 10) || 0;
        istek.ustYoneticiKod1 = parseInt($('#ddlUstYonetici1').val(), 10) || 0;
        istek.ustYoneticiKod2 = parseInt($('#ddlUstYonetici2').val(), 10) || 0;
        istek.yoneticiTc = kisiler.yonetici ? kisiler.yonetici.tc : '';
        istek.ustYoneticiTc1 = kisiler.ust1 ? kisiler.ust1.tc : '';
        istek.ustYoneticiTc2 = kisiler.ust2 ? kisiler.ust2.tc : '';
        istek.harcamaKodlari = harcamaKodlari;
        istek.ulke = $('#ddlUlke').val() || '';
        istek.il = $('#ddlIl').val() || '';
        istek.ilce = $('#ddlIlce').val() || '';
        // İl ve ilçede bölge, ilin kodundan gelir (ülke.bölge.il).
        istek.bolge = istek.cografya === 'bolge' ? ($('#ddlBolge').val() || '') : istek.il.split('.').slice(0, 2).join('.');
        return istek;
    }

    function baslikYaz() {
        $('#birimBaslik').text(!birim ? 'Soldan bir birim seçiniz.' : birim.kod ? birim.ad : 'Yeni birim' + (birim.ustAd ? ' (üst birim: ' + birim.ustAd + ')' : ''));
    }

    function birimAc(kod) {
        return Sgb.al('api/orgbirim/oku', { kod: kod, sablon: sablon })
            .done(function (b) {
                if (b.hata) { mesaj(b.hata, 'danger'); return; }
                birim = { kod: b.kod, anaBirimKod: b.anaBirimKod, ad: b.ad };
                baslikYaz();
                formDoldur(b);
                if ($('#sekmeGorev').attr('aria-selected') === 'true') gorevleriYukle();
            })
            .fail(hataGoster);
    }

    function formuKapat() {
        birim = null;
        baslikYaz();
        formDoldur({});
        sekmeSec('Bilgi');
    }

    // ---------------------------------------------------------------- sekmeler

    function gorevleriYukle() {
        if (!birim || !birim.kod) return;
        var adres = 'GorevAta?menuYok=1&orgKod=' + birim.kod + '&orgAd=' + encodeURIComponent(birim.ad);
        var cerceve = document.getElementById('cerceveGorev');
        if (cerceve.getAttribute('src') !== adres) cerceve.setAttribute('src', adres);
    }

    function sekmeSec(ad) {
        var gorev = ad === 'Gorev';
        $('#sekmeBilgi').attr('aria-selected', String(!gorev));
        $('#sekmeGorev').attr('aria-selected', String(gorev));
        $('#pnlBilgi').prop('hidden', gorev);
        $('#pnlGorev').prop('hidden', !gorev);
        if (gorev) gorevleriYukle();
    }
    $('#sekmeBilgi').on('click', function () { sekmeSec('Bilgi'); });
    $('#sekmeGorev').on('click', function () { sekmeSec('Gorev'); });

    // ---------------------------------------------------------------- işlemler

    $('#btnYeni').on('click', function () {
        // Yeni birim, seçili birimin altına (seçili yoksa en üste) eklenir.
        birim = { kod: 0, anaBirimKod: birim && birim.kod ? birim.kod : 0, ad: '', ustAd: birim && birim.kod ? birim.ad : '' };
        $('#agacBirim .sec').removeAttr('aria-current');
        baslikYaz();
        sekmeSec('Bilgi');
        formDoldur({});
        $('#txtAd').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        if (!birim) return;
        var istek = formOku();
        if (!istek.ad) { mesaj('Birimin adını yazınız.', 'warning'); $('#txtAd').trigger('focus'); return; }
        if (istek.cografya === 'il' && !istek.il && sablon !== '1') { mesaj('İl bilgisi boş bırakılamaz.', 'warning'); $('#ddlIl').trigger('focus'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/orgbirim/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                birim = { kod: s.kod, anaBirimKod: s.anaBirimKod, ad: istek.ad };
                // Ağaçta yalnızca birimin bulunduğu dal yeniden okunur.
                agac.altiniYenile(s.anaBirimKod || null);
                birimAc(s.kod);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); yazmaDurumu(); });
    });

    function birimIslemi(adres, soru) {
        if (!birim || !birim.kod) return;
        if (!window.confirm('"' + birim.ad + '" ' + soru)) return;
        var ust = birim.anaBirimKod;
        Sgb.gonder(adres, { kod: birim.kod, sablon: sablon === '1' })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                formuKapat();
                agac.altiniYenile(ust || null);
            })
            .fail(hataGoster);
    }
    $('#btnSil').on('click', function () { birimIslemi('api/orgbirim/sil', 'birimi silinecek. Onaylıyor musunuz?'); });
    $('#btnBirimKapat').on('click', function () { birimIslemi('api/orgbirim/kapat', 'birimi kapatılacak. Onaylıyor musunuz?'); });

    // ---------------------------------------------------------------- raporlar

    function raporIndir($dugme, adres, parametreler) {
        $dugme.prop('disabled', true);
        Sgb.indir(adres, parametreler).then(function () { $dugme.prop('disabled', false); });
    }
    // Yazdır: seçili birim ve altındaki birimler; birim seçili değilse bütün şema.
    $('#btnYazdir').on('click', function () {
        raporIndir($(this), 'api/orgbirim/rapor/birimler', birim && birim.kod ? { kod: birim.kod } : null);
    });
    $('#btnGorevYaz').on('click', function () { raporIndir($(this), 'api/orgbirim/rapor/gorevler'); });

    // ---------------------------------------------------------------- birim seçme penceresi (merkez birimi, birim açma)

    var birimSecimi = null; // { secilen, bitince }

    function birimSecAc(baslik, kapaliDahil, bitince) {
        birimSecimi = { secilen: null, bitince: bitince };
        $('#dlgBirimSecBaslik').text(baslik);
        $('#btnBirimSecTamam').prop('disabled', true);
        Sgb.agac('#agacBirimSec', {
            yukle: function (ust) { return agacYukle(ust, kapaliDahil); },
            secilince: function (d) { birimSecimi.secilen = d; $('#btnBirimSecTamam').prop('disabled', false); },
            ilkAcik: true,
            bosMetin: 'Birim bulunamadı.',
            hata: hataGoster
        });
        document.getElementById('dlgBirimSec').showModal();
    }

    $('#btnBirimSecTamam').on('click', function () {
        if (!birimSecimi || !birimSecimi.secilen) return;
        var s = birimSecimi;
        birimSecimi = null;
        document.getElementById('dlgBirimSec').close();
        s.bitince(s.secilen);
    });

    $('#btnMerkezSec').on('click', function () {
        birimSecAc('Merkez Birimi', false, function (d) { merkez = { kod: d.kod, ad: d.ad }; $('#txtMerkezAd').val(d.ad); });
    });
    $('#btnMerkezSil').on('click', function () { merkez = { kod: 0, ad: '' }; $('#txtMerkezAd').val(''); });

    // Birim Aç: kapatılmış birimler de listelenir; seçilen birim yeniden açılır.
    $('#btnBirimAc').on('click', function () {
        birimSecAc('Açılacak Birim', true, function (d) {
            Sgb.gonder('api/orgbirim/ac', { kod: d.kod, sablon: sablon === '1' })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) agac.yenile();
                })
                .fail(hataGoster);
        });
    });

    // ---------------------------------------------------------------- ağaç

    function agacYukle(ust, kapaliDahil) {
        var sorgu = { ust: ust ? ust.kod : 0 };
        if (sablon) sorgu.sablon = sablon;
        if (kapaliDahil) sorgu.kapaliDahil = '1';
        return Sgb.al('api/orgbirim/agac', sorgu).then(function (veri) {
            if (veri.hata) { mesaj(veri.hata, 'danger'); return []; }
            return veri.satirlar;
        });
    }

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/orgbirim/tanimlar')
        .done(function (t) {
            tanim = t;
            $('.organa .yazma').prop('hidden', !t.yazabilir);
            $('#btnGorevYaz').prop('hidden', !t.gorevAtayabilir); // yalnızca sistem yöneticisi
            $('#formUyari').text(t.uyari || '').prop('hidden', !t.uyari);
            $.each(t.birimTurleri, function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo('#ddlBirimTur'); });
            $.each(t.unvanlar, function (i, u) { $('<option></option>').val(u.kod).text(u.ad).appendTo('#ddlYonetici, #ddlUstYonetici1, #ddlUstYonetici2'); });
            $('#ddlUlke').empty();
            $.each(t.ulkeler, function (i, u) { $('<option></option>').val(u.kod).text(u.ad).appendTo('#ddlUlke'); });
            formDoldur({});

            agac = Sgb.agac('#agacBirim', {
                yukle: function (ust) { return agacYukle(ust, false); },
                secilince: function (d) { birimAc(d.kod); },
                seciliMi: function (d) { return !!birim && birim.kod === d.kod; },
                simge: function (d) { return (TIPLER[d.tip] || TIPLER.grup)[0]; },
                ipucu: function (d) { return (TIPLER[d.tip] || TIPLER.grup)[1]; },
                ilkAcik: true,
                bosMetin: 'Birim bulunamadı.',
                hata: hataGoster
            });
        })
        .fail(hataGoster);
});
