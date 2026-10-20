// Personel Atama penceresi (eski InsanKaynaklari/PersonelAtama.aspx). Hizmet Belgesi (Kadro) sekmesinden Sgb.pencere ile açılır:
// /InsanKaynaklari/PersonelAtama?menuYok=1&personelKod=<p>&kod=<hizmet kaydı kodu; yeni kayıt için 0>.
// Her satırda önceki (eski) kadro kaydı salt okunur, yanında yeni kayıt düzenlenir; eskisinden farklı yeni değerler kırmızı yazılır.
// Kaydet (onaylı) / Yeni Kayıt. Derece-kademe değişince gösterge hesaplanır. Birim seçimi sayfa içi pencerede ağaçtan yapılır.
// Uçlar: /api/personelbilgi/atama/* (InsanKaynaklari/PersonelAtamaUclari.cs).
$(function () {
    'use strict';

    var $sayfa = $('.pa');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var U = 'api/personelbilgi/atama/';
    var personelKod = parseInt($sayfa.data('personel-kod'), 10) || 0;
    var ilkKod = parseInt($sayfa.data('kod'), 10) || 0;

    // Açık kayıt (eski gizli alanlar hdnYeniKadroDurum, hdnYeniPKadroKod, hdnEskiKadroKod, hdnIliskiliKadroIslemNo).
    var acik = { kod: 0, eskiKod: 0, kadroDurum: 0, iliskiliKadroIslemNo: 0 };
    var eski = {};         // önceki kaydın ekranda görünen değerleri (renk karşılaştırması için)
    var eskiPersonelTip = 0;
    var birimler = { birim: { kod: 0, ad: '' }, gorevBirim: { kod: 0, ad: '' }, tayinBirim: { kod: 0, ad: '' } };
    var duzen = { kaydetKapali: true, onayDurum: false, ayrilabilir: true };
    var tanimHazir = false;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        if (xhr && xhr.status === 403) { mesaj('Bu işlem için yetkiniz yok.', 'danger'); return; }
        var ek = '';
        if (xhr && xhr.responseJSON && xhr.responseJSON.hata) ek = xhr.responseJSON.hata;
        mesaj('İşlem yapılamadı' + (xhr && xhr.status ? ' (HTTP ' + xhr.status + ')' : '') + (ek ? ': ' + ek : '.'), 'danger');
    }

    function secenekDoldur($s, liste, kodAlan, adAlan) {
        $s.empty();
        $.each(liste || [], function (i, x) {
            $('<option></option>').attr('value', x[kodAlan] == null ? '' : String(x[kodAlan])).text(x[adAlan] || '').appendTo($s);
        });
    }

    // Değeri listede olmayan kayıt (ör. pasif unvan) seçenek olarak eklenir; kayıttaki değer kaybolmasın.
    function secimYaz($s, deger, ad) {
        deger = deger == null ? '' : String(deger);
        if (deger !== '' && !$s.find('option').filter(function () { return this.value === deger; }).length)
            $('<option></option>').attr('value', deger).text(ad || deger).appendTo($s);
        $s.val(deger);
        if ($s.val() === null) $s.val($s.find('option').first().val());
    }

    function secimMetni($s) {
        var $o = $s.find('option:selected');
        return $o.length ? $o.text() : '';
    }

    function sayiYaz(n) { return n === null || n === undefined ? '' : String(n); }

    // ---------------------------------------------------------------- pencereler

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    var dlgSoru = document.getElementById('dlgSoru');
    $('#btnSoruEvet').on('click', function () { dlgSoru.close(); kaydet(); });

    // ---------------------------------------------------------------- değişen alanların rengi (eski ...Renderer işlevleri)

    // Yeni alan -> eski kayıttaki karşılığı. tur: 'deger' (değer karşılaştırılır), 'metin' (seçilen metin), 'birim', 'tip'.
    var KARSILASTIR = [
        { alan: '#cmbPersonelTip', tur: 'tip' },
        { alan: '#ddlBaslamaGerekcesi', tur: 'metin', eski: 'baslamaGerekcesi' },
        { alan: '#txtAtamaTarihi', eski: 'atamaTarihi' },
        { alan: '#txtAtamaNedeni', eski: 'atamaNedeni' },
        { alan: '#txtBirim', tur: 'birim', birim: 'birim', eski: 'birim' },
        { alan: '#txtGorevBirim', tur: 'birim', birim: 'gorevBirim', eski: 'gorevBirim' },
        { alan: '#txtGorevYeri', eski: 'gorevYeri' },
        { alan: '#ddlUnvan', tur: 'metin', eski: 'unvan' },
        { alan: '#ddlVekaletUnvani', tur: 'metin', eski: 'vekaletUnvani' },
        { alan: '#txtEvrakNo', eski: 'evrakNo' },
        { alan: '#txtYasalDayanak', eski: 'yasalDayanak' },
        { alan: '#txtOnayTarih', eski: 'onayTarih' },
        { alan: '#txtMeslekKodu', eski: 'meslekKodu' },
        { alan: '#txtAciklama', eski: 'aciklama' },
        { alan: '#txtKadroDerece', eski: 'kadroDerece' },
        { alan: '#txtKadroTarih', eski: 'kadroTarih' },
        { alan: '#txtKazanilmisHakDerece', eski: 'kazanilmisHakDerece' },
        { alan: '#txtKazanilmisHakKademe', eski: 'kazanilmisHakKademe' },
        { alan: '#txtKazanilmisHakTarih', eski: 'kazanilmisHakTarih' },
        { alan: '#txtKazanilmisEkGosterge', eski: 'kazanilmisEkGosterge' },
        { alan: '#txtEmekliyeEsasDerece', eski: 'emekliyeEsasDerece' },
        { alan: '#txtEmekliyeEsasKademe', eski: 'emekliyeEsasKademe' },
        { alan: '#txtEmekliyeEsasTarih', eski: 'emekliyeEsasTarih' },
        { alan: '#txtEmekliEkGosterge', eski: 'emekliyeEsasEkGosterge' },
        { alan: '#txtOdemeyeEsasDerece', eski: 'odemeyeEsasDerece' },
        { alan: '#txtOdemeyeEsasKademe', eski: 'odemeyeEsasKademe' },
        { alan: '#txtOdemeyeEsasTarih', eski: 'odemeyeEsasTarih' },
        { alan: '#txtOdemeyeEsasEkGosterge', eski: 'odemeyeEsasEkGosterge' },
        { alan: '#ddlAyrilmaGerekcesi', tur: 'metin', eski: 'ayrilmaGerekcesi' },
        { alan: '#txtAyrilmaNedeni', eski: 'ayrilmaNedeni' },
        { alan: '#txtAyrilmaTarihi', eski: 'ayrilmaTarihi' },
        { alan: '#txtHizmetGunu', eski: 'hizmetGunSayisi' }
    ];

    function renkle(k) {
        var $a = $(k.alan), ayni;
        if (k.tur === 'tip') ayni = String($a.val() || '') === String(eskiPersonelTip);
        else if (k.tur === 'metin') ayni = secimMetni($a) === (eski[k.eski] || '');
        else if (k.tur === 'birim') ayni = (birimler[k.birim].ad || '') === (eski[k.eski] || '');
        else ayni = String($a.val() == null ? '' : $a.val()) === String(eski[k.eski] == null ? '' : eski[k.eski]);
        $a.toggleClass('pa-farkli', !ayni);
    }
    function hepsiniRenkle() { $.each(KARSILASTIR, function (i, k) { renkle(k); }); }
    $.each(KARSILASTIR, function (i, k) {
        if (k.tur !== 'birim') $(k.alan).on('change input', function () { renkle(k); });
    });

    // ---------------------------------------------------------------- birim alanları

    var BIRIM_ALAN = { birim: '#txtBirim', gorevBirim: '#txtGorevBirim', tayinBirim: '#txtTayinBirim' };
    function birimYaz(ad, kod, metin) {
        birimler[ad] = { kod: kod || 0, ad: kod ? (metin || '') : '' };
        $(BIRIM_ALAN[ad]).val(birimler[ad].ad);
        $.each(KARSILASTIR, function (i, k) { if (k.birim === ad) renkle(k); });
    }

    var dlgBirim = document.getElementById('dlgBirim');
    var birimHedef = 'birim', birimAgac = null, agacTum = null, birimAra = '';
    function birimAgaciKur(tum) {
        agacTum = tum;
        birimAgac = Sgb.agac('#agcBirim', {
            yukle: function (ust) {
                var p = ust ? { ust: ust.kod } : { ust: 0, ara: birimAra };
                if (agacTum) p.tum = 1;
                return Sgb.al(U + 'orgagac', p).then(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                    return v.satirlar || [];
                });
            },
            secilince: function (d) { birimYaz(birimHedef, d.kod, d.birimAd || d.ad); dlgBirim.close(); },
            ilkAcik: true,
            bosMetin: 'Birim bulunamadı.',
            hata: hataGoster
        });
    }
    $sayfa.on('click', '.pa-birim-sec', function () {
        birimHedef = $(this).closest('.pa-birim').data('birim');
        // Kadro / görev birimi kullanıcının birim yetkisine göre (eski KullanicininBirimleri); gideceği birim tüm birimlerden.
        var tum = birimHedef === 'tayinBirim';
        if (!birimAgac || agacTum !== tum) { birimAra = ''; $('#txtBirimAra').val(''); birimAgaciKur(tum); }
        dlgBirim.showModal();
    });
    $sayfa.on('click', '.pa-birim-sil', function () { birimYaz($(this).closest('.pa-birim').data('birim'), 0, ''); });
    function birimAramaYap() {
        var a = $.trim($('#txtBirimAra').val());
        if (a && a.length < 3) { mesaj('Aramak için en az 3 harf yazınız.', 'warning'); return; }
        birimAra = a;
        if (birimAgac) birimAgac.yenile();
    }
    $('#btnBirimAra').on('click', birimAramaYap);
    $('#txtBirimAra').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); birimAramaYap(); } });

    // ---------------------------------------------------------------- gösterge (eski GostergeHesapla)

    var GOSTERGE = {
        1: { d: '#txtKazanilmisHakDerece', k: '#txtKazanilmisHakKademe', g: '#txtKazanilmisGosterge' },
        2: { d: '#txtEmekliyeEsasDerece', k: '#txtEmekliyeEsasKademe', g: '#txtEmekliGosterge' },
        3: { d: '#txtOdemeyeEsasDerece', k: '#txtOdemeyeEsasKademe', g: '#txtOdemeyeEsasGosterge' }
    };
    function gostergeHesapla(yer) {
        var a = GOSTERGE[yer];
        return Sgb.al(U + 'gosterge', { derece: $(a.d).val() || '', kademe: $(a.k).val() || '' }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            $(a.g).val(sayiYaz(v.gosterge));
        }).fail(hataGoster);
    }
    $sayfa.on('change', '[data-gosterge]', function () { gostergeHesapla($(this).data('gosterge')); });

    // ---------------------------------------------------------------- personel türü (eski cmbPersonelTipDegisti / BirimGoster)

    var eskiKurumDisi = false;
    function tipUygula(tip) {
        tip = tip || {};
        $('#txtHizmetGrubu').val(tip.hizmetGrubu || '');
        var kurumDisi = !!tip.kurumDisi;
        // Yeni kayıt: kurum içi türlerde birim seçimi, kurum dışı türlerde görev yeri yazılır.
        $('#satirBirim .pa-yeni .pa-birim').prop('hidden', kurumDisi);
        $('#eskiBirim').prop('hidden', eskiKurumDisi);
        $('#satirBirim').prop('hidden', kurumDisi && eskiKurumDisi);
        $('#satirGorevBirim').prop('hidden', kurumDisi);
        $('#satirGorevYeri').prop('hidden', !kurumDisi);
        $('#bolumAtanacagiKadro').prop('hidden', !tip.kadroIslemi);
    }
    $('#cmbPersonelTip').on('change', function () {
        Sgb.al(U + 'personeltip', { tur: parseInt($(this).val(), 10) || 0 }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            tipUygula(v.tip);
        }).fail(hataGoster);
    });

    // ---------------------------------------------------------------- salt okunurluk (eski GizleAc)

    function duzenUygula(d) {
        duzen = d || duzen;
        $('#btnKaydet').prop('disabled', !!duzen.kaydetKapali);
        $sayfa.find('[data-onay]').each(function () { kilitle($(this), !!duzen.onayDurum); });
        $sayfa.find('[data-ayrilis]').each(function () { kilitle($(this), !duzen.ayrilabilir); });
    }
    function kilitle($a, kilitli) {
        if ($a.is('select, button')) $a.prop('disabled', kilitli);
        else $a.prop('readonly', kilitli);
    }

    // ---------------------------------------------------------------- okuma (eski Listele)

    function oku(kod, eskiKod) {
        return Sgb.al(U + 'oku', { personelKod: personelKod, kod: kod || 0, eskiKod: eskiKod === undefined ? -1 : eskiKod }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            doldur(v);
        }).fail(hataGoster);
    }

    function doldur(v) {
        var y = v.yeni || {}, e = v.eski || {};
        acik = { kod: y.kod || 0, eskiKod: e.kod || 0, kadroDurum: y.kadroDurum || 0, iliskiliKadroIslemNo: y.iliskiliKadroIslemNo || 0 };
        eskiPersonelTip = e.personelTip || 0;
        eskiKurumDisi = !!v.eskiKurumDisi;

        $('#lblEskiBaslik').text('Eski Kadro / ' + (e.kadroDurumAd || ''));
        $('#lblYeniBaslik').text('Yeni Kadro / ' + (y.kadroDurumAd || ''));

        // Eski kadro (salt okunur)
        eski = {
            baslamaGerekcesi: e.hizmetSebebiAd || '',
            atamaTarihi: e.atamaTarihi, atamaNedeni: e.atamaNedeni,
            birim: e.birimAd || '', gorevBirim: e.gorevBirimAd || '', gorevYeri: e.gorevYeri,
            unvan: e.unvanAd || '', vekaletUnvani: e.vekilUnvanAd || '',
            evrakNo: e.evrakNo, yasalDayanak: e.yasalDayanak, onayTarih: e.onayTarih, meslekKodu: e.meslekKodu, aciklama: e.aciklama,
            kadroDerece: sayiYaz(e.kadroDerece), kadroTarih: e.kadroTarih,
            kazanilmisHakDerece: sayiYaz(e.kazanilmisHakDerece), kazanilmisHakKademe: sayiYaz(e.kazanilmisHakKademe),
            kazanilmisHakTarih: e.kazanilmisHakTarih, kazanilmisEkGosterge: sayiYaz(e.kazanilmisEkGosterge),
            emekliyeEsasDerece: sayiYaz(e.emekliyeEsasDerece), emekliyeEsasKademe: sayiYaz(e.emekliyeEsasKademe),
            emekliyeEsasTarih: e.emekliyeEsasTarih, emekliyeEsasEkGosterge: sayiYaz(e.emekliyeEsasEkGosterge),
            odemeyeEsasDerece: sayiYaz(e.odemeyeEsasDerece), odemeyeEsasKademe: sayiYaz(e.odemeyeEsasKademe),
            odemeyeEsasTarih: e.odemeyeEsasTarih, odemeyeEsasEkGosterge: sayiYaz(e.odemeyeEsasEkGosterge),
            ayrilmaGerekcesi: e.ayrilmaGerekcesiAd || '', ayrilmaNedeni: e.ayrilmaNedeni, ayrilmaTarihi: e.ayrilmaTarihi,
            hizmetGunSayisi: sayiYaz(e.hizmetGunSayisi)
        };
        $('#eskiPersonelTip').val(e.personelTipAd || '');
        $('#eskiHizmetGrubu').val(e.hizmetHesabi || '');
        $('#eskiBaslamaGerekcesi').val(eski.baslamaGerekcesi);
        $('#eskiAtamaTarihi').val(e.atamaTarihi || '');
        $('#eskiAtamaNedeni').val(e.atamaNedeni || '');
        $('#eskiBirim').val(eski.birim);
        $('#eskiGorevBirim').val(eski.gorevBirim);
        $('#eskiGorevYeri').val(e.gorevYeri || '');
        $('#eskiUnvan').val(eski.unvan);
        $('#eskiVekaletUnvani').val(eski.vekaletUnvani);
        $('#eskiEvrakNo').val(e.evrakNo || '');
        $('#eskiYasalDayanak').val(e.yasalDayanak || '');
        $('#eskiOnayTarih').val(e.onayTarih || '');
        $('#eskiMeslekKodu').val(e.meslekKodu || '');
        $('#eskiAciklama').val(e.aciklama || '');
        $('#eskiKadroDerece').val(sayiYaz(e.kadroDerece));
        $('#eskiKadroTarih').val(e.kadroTarih || '');
        $('#eskiKazanilmisHakDerece').val(sayiYaz(e.kazanilmisHakDerece));
        $('#eskiKazanilmisHakKademe').val(sayiYaz(e.kazanilmisHakKademe));
        $('#eskiKazanilmisHakTarih').val(e.kazanilmisHakTarih || '');
        $('#eskiKazanilmisGosterge').val(sayiYaz(e.kazanilmisGosterge));
        $('#eskiKazanilmisEkGosterge').val(sayiYaz(e.kazanilmisEkGosterge));
        $('#eskiEmekliyeEsasDerece').val(sayiYaz(e.emekliyeEsasDerece));
        $('#eskiEmekliyeEsasKademe').val(sayiYaz(e.emekliyeEsasKademe));
        $('#eskiEmekliyeEsasTarih').val(e.emekliyeEsasTarih || '');
        $('#eskiEmekliGosterge').val(sayiYaz(e.emekliyeEsasGosterge));
        $('#eskiEmekliEkGosterge').val(sayiYaz(e.emekliyeEsasEkGosterge));
        $('#eskiOdemeyeEsasDerece').val(sayiYaz(e.odemeyeEsasDerece));
        $('#eskiOdemeyeEsasKademe').val(sayiYaz(e.odemeyeEsasKademe));
        $('#eskiOdemeyeEsasTarih').val(e.odemeyeEsasTarih || '');
        $('#eskiOdemeyeEsasGosterge').val(sayiYaz(e.odemeyeEsasGosterge));
        $('#eskiOdemeyeEsasEkGosterge').val(sayiYaz(e.odemeyeEsasEkGosterge));
        $('#eskiAyrilmaGerekcesi').val(eski.ayrilmaGerekcesi);
        $('#eskiAyrilmaNedeni').val(e.ayrilmaNedeni || '');
        $('#eskiAyrilmaTarihi').val(e.ayrilmaTarihi || '');
        $('#eskiHizmetGunu').val(sayiYaz(e.hizmetGunSayisi));

        // Yeni kadro
        secimYaz($('#cmbPersonelTip'), y.personelTip || '', y.personelTipAd);
        secimYaz($('#ddlBaslamaGerekcesi'), y.baslamaGerekcesi, y.hizmetSebebiAd);
        $('#txtAtamaTarihi').val(y.atamaTarihi || '');
        $('#txtAtamaNedeni').val(y.atamaNedeni || '');
        birimYaz('birim', y.organizasyonKodu, y.birimAd);
        birimYaz('gorevBirim', y.gorevOrganizasyonKodu, y.gorevBirimAd);
        $('#txtGorevYeri').val(y.gorevYeri || '');
        secimYaz($('#ddlUnvan'), y.unvan > 0 ? y.unvan : '', y.unvanAd);
        secimYaz($('#ddlVekaletUnvani'), y.vekilUnvanKod > 0 ? y.vekilUnvanKod : '', y.vekilUnvanAd);
        $('#txtEvrakNo').val(y.evrakNo || '');
        $('#txtYasalDayanak').val(y.yasalDayanak || '');
        $('#txtOnayTarih').val(y.onayTarih || '');
        $('#txtMeslekKodu').val(y.meslekKodu || '');
        $('#txtAciklama').val(y.aciklama || '');
        $('#txtKadroDerece').val(sayiYaz(y.kadroDerece));
        $('#txtKadroTarih').val(y.kadroTarih || '');
        $('#txtKazanilmisHakDerece').val(sayiYaz(y.kazanilmisHakDerece));
        $('#txtKazanilmisHakKademe').val(sayiYaz(y.kazanilmisHakKademe));
        $('#txtKazanilmisHakTarih').val(y.kazanilmisHakTarih || '');
        $('#txtKazanilmisGosterge').val(sayiYaz(y.kazanilmisGosterge));
        $('#txtKazanilmisEkGosterge').val(sayiYaz(y.kazanilmisEkGosterge));
        $('#txtEmekliyeEsasDerece').val(sayiYaz(y.emekliyeEsasDerece));
        $('#txtEmekliyeEsasKademe').val(sayiYaz(y.emekliyeEsasKademe));
        $('#txtEmekliyeEsasTarih').val(y.emekliyeEsasTarih || '');
        $('#txtEmekliGosterge').val(sayiYaz(y.emekliyeEsasGosterge));
        $('#txtEmekliEkGosterge').val(sayiYaz(y.emekliyeEsasEkGosterge));
        $('#txtOdemeyeEsasDerece').val(sayiYaz(y.odemeyeEsasDerece));
        $('#txtOdemeyeEsasKademe').val(sayiYaz(y.odemeyeEsasKademe));
        $('#txtOdemeyeEsasTarih').val(y.odemeyeEsasTarih || '');
        $('#txtOdemeyeEsasGosterge').val(sayiYaz(y.odemeyeEsasGosterge));
        $('#txtOdemeyeEsasEkGosterge').val(sayiYaz(y.odemeyeEsasEkGosterge));
        secimYaz($('#ddlAyrilmaGerekcesi'), y.ayrilmaGerekcesi, y.ayrilmaGerekcesiAd);
        $('#txtAyrilmaNedeni').val(y.ayrilmaNedeni || '');
        $('#txtAyrilmaTarihi').val(y.ayrilmaTarihi || '');
        birimYaz('tayinBirim', y.tayinOlacagiBirim, y.tayinOlacagiBirimAd);
        $('#txtHizmetGunu').val(sayiYaz(y.hizmetGunSayisi));
        secimYaz($('#cmbAtanacagiKadro'), y.atanacagiKadro || '0-0', y.atanacagiKadro);

        tipUygula(v.tip);
        duzenUygula(v.duzen);
        hepsiniRenkle();
        // Eski Listele'de derece / kademe alanlarının change olayı göstergeleri yeniden hesaplatırdı.
        gostergeHesapla(1); gostergeHesapla(2); gostergeHesapla(3);
    }

    // ---------------------------------------------------------------- kaydet (eski btnKaydet_Click / VeriTopla)

    function deger(s) { var v = $(s).val(); return v == null ? '' : String(v); }

    function veriTopla() {
        return {
            kadroDurum: acik.kadroDurum,
            kod: acik.kod,
            eskiKod: acik.eskiKod,
            personelKod: personelKod,
            iliskiliKadroIslemNo: String(acik.iliskiliKadroIslemNo || 0),
            personelTip: deger('#cmbPersonelTip'),
            baslamaGerekcesi: deger('#ddlBaslamaGerekcesi'),
            atamaTarihi: deger('#txtAtamaTarihi'),
            atamaNedeni: deger('#txtAtamaNedeni'),
            organizasyonKodu: birimler.birim.kod || 0,
            gorevOrganizasyonKodu: birimler.gorevBirim.kod || 0,
            tayinOlacagiBirim: birimler.tayinBirim.kod || 0,
            gorevYeri: deger('#txtGorevYeri'),
            unvan: deger('#ddlUnvan'),
            vekilUnvanKod: deger('#ddlVekaletUnvani'),
            evrakNo: deger('#txtEvrakNo'),
            yasalDayanak: deger('#txtYasalDayanak'),
            onayTarih: deger('#txtOnayTarih'),
            meslekKodu: deger('#txtMeslekKodu'),
            aciklama: deger('#txtAciklama'),
            kadroDerece: deger('#txtKadroDerece'),
            kadroTarih: deger('#txtKadroTarih'),
            kazanilmisHakDerece: deger('#txtKazanilmisHakDerece'),
            kazanilmisHakKademe: deger('#txtKazanilmisHakKademe'),
            kazanilmisHakTarih: deger('#txtKazanilmisHakTarih'),
            kazanilmisGosterge: deger('#txtKazanilmisGosterge'),
            kazanilmisEkGosterge: deger('#txtKazanilmisEkGosterge'),
            odemeyeEsasDerece: deger('#txtOdemeyeEsasDerece'),
            odemeyeEsasKademe: deger('#txtOdemeyeEsasKademe'),
            odemeyeEsasTarih: deger('#txtOdemeyeEsasTarih'),
            odemeyeEsasGosterge: deger('#txtOdemeyeEsasGosterge'),
            odemeyeEsasEkGosterge: deger('#txtOdemeyeEsasEkGosterge'),
            emekliyeEsasDerece: deger('#txtEmekliyeEsasDerece'),
            emekliyeEsasKademe: deger('#txtEmekliyeEsasKademe'),
            emekliyeEsasTarih: deger('#txtEmekliyeEsasTarih'),
            emekliyeEsasGosterge: deger('#txtEmekliGosterge'),
            emekliyeEsasEkGosterge: deger('#txtEmekliEkGosterge'),
            ayrilmaGerekcesi: deger('#ddlAyrilmaGerekcesi'),
            ayrilmaNedeni: deger('#txtAyrilmaNedeni'),
            ayrilmaTarihi: deger('#txtAyrilmaTarihi'),
            hizmetGunSayisi: deger('#txtHizmetGunu'),
            atanacagiKadro: deger('#cmbAtanacagiKadro')
        };
    }

    function kaydet() {
        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder(U + 'kaydet', veriTopla()).done(function (v) {
            if (!v.basarili) { mesaj(v.hata || 'Kayıt yapılamadı.', 'danger'); return; }
            mesaj(v.bilgi || 'Kayıt işlemi başarıyla gerçekleşti.', 'success');
            try { if (window.parent && window.parent !== window && typeof window.parent.Degisti === 'function') window.parent.Degisti(); } catch (e) { /* üst sayfa başka kökenden */ }
            oku(v.kod, v.eskiKod);
        }).fail(hataGoster).always(function () { $d.prop('disabled', !!duzen.kaydetKapali); });
    }

    $('#btnKaydet').on('click', function () {
        if (!tanimHazir) return;
        dlgSoru.showModal();
    });
    $('#btnYeniKayit').on('click', function () { oku(0, -1); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    if (!personelKod) { mesaj('Personel bilgisi bulunamadı.', 'warning'); return; }
    Sgb.al(U + 'tanim', { personelKod: personelKod }).done(function (v) {
        if (v.hata) mesaj(v.hata, 'warning');
        secenekDoldur($('#ddlUnvan'), [{ UnvanKod: '', UnvanAd: '' }].concat(v.unvanlar || []), 'UnvanKod', 'UnvanAd');
        secenekDoldur($('#ddlVekaletUnvani'), [{ UnvanKod: '', UnvanAd: '' }].concat(v.unvanlar || []), 'UnvanKod', 'UnvanAd');
        secenekDoldur($('#ddlBaslamaGerekcesi'), v.baslamaSebepleri, 'kod', 'ad');
        secenekDoldur($('#ddlAyrilmaGerekcesi'), v.ayrilmaSebepleri, 'kod', 'ad');
        secenekDoldur($('#cmbPersonelTip'), [{ kod: '', ad: '' }].concat(v.personelTipleri || []), 'kod', 'ad');
        secenekDoldur($('#cmbAtanacagiKadro'), v.acikKadrolar, 'ID', 'Aciklama');
        tanimHazir = true;
        oku(ilkKod, -1);
    }).fail(hataGoster);
});
