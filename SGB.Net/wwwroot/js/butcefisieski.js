// Bütçe gider fişi, e-Bütçe biçimi (ButceHazirlikv2/ButceFisiGirisEski): arama kriterlerine uyan tertipler İlk Tertip / Önceki / Sonraki
// ile tek tek getirilir; tertibin adları, geçmiş yıl rakamları, 3 yıllık teklifi ve gerekçesi gösterilir. Kaydet gerekçeyi, farklılaşma
// sebeplerini ve teklifi saklar; Yazdır kriterlere uyan bütün tertiplerin fişlerini eski koddaki Word (.doc) dosyası olarak indirir.
// Arama kriterleri tarayıcıda saklanır.
$(function () {
    'use strict';

    var $sayfa = $('.butce-fisi-eski');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var SAKLA = 'butcefisieski.';
    var acik = null;      // gösterilen tertip: { yil, senaryo, kur, fon, fin, eko, tertip }
    var degisti = false;  // gösterilen tertipte kaydedilmemiş değişiklik var mı?
    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur || 'info'); }

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function saklananOku(ad, varsayilan) {
        try { var d = window.localStorage.getItem(SAKLA + ad); return d === null ? varsayilan : d; } catch (e) { return varsayilan; }
    }
    function sakla(ad, deger) {
        try { window.localStorage.setItem(SAKLA + ad, deger); } catch (e) { /* tarayıcı izin vermiyor */ }
    }

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }

    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }

    function alanMetin(alan) { return $.trim($('.kod-alan[data-alan="' + alan + '"] input').val()); }

    // Eski sayfadaki "#,###" biçimi: sıfır boş görünür.
    function sayiYaz(d) { return d ? tamSayi.format(d) : ''; }
    function sayiOku(metin) {
        var s = $.trim(metin || '').replace(/\./g, '').replace(/\s/g, '').replace(',', '.');
        if (s === '') return 0;
        var d = parseFloat(s);
        return isNaN(d) ? null : d;
    }

    var editor = Sgb.editor('#txtGerekce', { yukseklik: 320 });
    editor.jodit.events.on('change', function () { if (acik) degisti = true; });

    // ---------------------------------------------------------------- yıl başlıkları (eski ddlYil_SelectedIndexChanged)

    function yilBasliklari() {
        var yil = yilOku();
        $('#lblOdenek1').text(yil);
        $('#lblOdenek2').text(yil + 1);
        $('#lblOdenek3').text(yil + 2);
        $('#lblYil2').text(yil - 2);
        $('#lblYil1').text(yil - 1);
    }

    function bosalt() {
        acik = null;
        degisti = false;
        $('.bilgi-deger').text('');
        $('#txtOdenek1, #txtOdenek2, #txtOdenek3, #txtSebep1, #txtSebep2').val('');
        editor.html('');
    }

    function degisiklikVarsaSor() {
        return !degisti || window.confirm('Gösterilen tertipte kaydedilmemiş değişiklikler var. Devam ederseniz kaybolacak. Devam edilsin mi?');
    }

    $('#ddlYil, #ddlSenaryo').on('change', function () {
        var eski = this.id === 'ddlYil' ? (acik ? acik.yil : null) : (acik ? acik.senaryo : null);
        if (!degisiklikVarsaSor()) { if (eski !== null) $(this).val(eski); return; }
        sakla(this.id === 'ddlYil' ? 'yil' : 'senaryo', this.value);
        yilBasliklari();
        bosalt();
    });

    // ---------------------------------------------------------------- İlk Tertip / Önceki / Sonraki (eski OncekiSonrakiTertipAra)

    function kriterleriSakla() {
        $('.kod-alan').each(function () { sakla('kod.' + $(this).attr('data-alan'), $(this).find('input').val()); });
    }

    function tertipGetir(yon, $d) {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl seçiniz.', 'warning'); return; }
        if (!degisiklikVarsaSor()) return;
        kriterleriSakla();
        var senaryo = $('#ddlSenaryo').val() || '';
        var p = {
            yil: yil, senaryo: senaryo, yon: yon,
            kur: acik ? acik.kur : '', fon: acik ? acik.fon : '', fin: acik ? acik.fin : '', eko: acik ? acik.eko : '',
            araKur: alanMetin('kur'), araFon: alanMetin('fon'), araFin: alanMetin('fin'), araEko: alanMetin('eko')
        };
        $('.tertip-git').prop('disabled', true);
        Sgb.al('api/butcefisieski/bul', p).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'warning'); return; }
            goster(v, yil, senaryo);
        }).fail(hataGoster).always(function () { $('.tertip-git').prop('disabled', false); });
    }

    function goster(v, yil, senaryo) {
        acik = { yil: yil, senaryo: senaryo, kur: v.kur, fon: v.fon, fin: v.fin, eko: v.eko, tertip: v.tertip };
        $('#dfTertip').text(v.tertip).attr('title', v.tertip);
        $('#dfBirimAd').text(v.birimAd || '').attr('title', v.birimAd || '');
        $('#dfEkoAd').text(v.ekoAd || '').attr('title', v.ekoAd || '');
        $('#dfKBO2').text(sayiYaz(v.kbo2));
        $('#dfHarcama2').text(sayiYaz(v.harcama2));
        $('#dfKBO1').text(sayiYaz(v.kbo1));
        $('#dfHarcama1').text(sayiYaz(v.harcama1));
        $('#dfYSTahmin').text(sayiYaz(v.ysTahmin));
        var o = v.odenek || [];
        $('#txtOdenek1').val(sayiYaz(o[0]));
        $('#txtOdenek2').val(sayiYaz(o[1]));
        $('#txtOdenek3').val(sayiYaz(o[2]));
        editor.html(v.gerekce || '');
        $('#txtSebep1').val(v.sebep1 || '');
        $('#txtSebep2').val(v.sebep2 || '');
        degisti = false;
        if (!v.okundu) mesaj('Okuma esnasında bir hata oldu!', 'danger');
    }

    $('.tertip-git').on('click', function () { tertipGetir(parseInt($(this).attr('data-yon'), 10) || 0, $(this)); });
    $('#frmSecim').on('submit', function (e) { e.preventDefault(); tertipGetir(0); });

    $('#txtOdenek1, #txtOdenek2, #txtOdenek3, #txtSebep1, #txtSebep2').on('input', function () { if (acik) degisti = true; });
    // Tutar alanı bırakılınca binlik ayraçla yazılır.
    $('#txtOdenek1, #txtOdenek2, #txtOdenek3').on('blur', function () {
        var d = sayiOku(this.value);
        if (d !== null) this.value = sayiYaz(d);
    });

    // ---------------------------------------------------------------- Kaydet (eski btnKaydet_Click)

    $('#btnKaydet').on('click', function () {
        if (!acik) { mesaj('Önce İlk Tertip, Önceki ya da Sonraki ile bir tertip getiriniz.', 'warning'); return; }
        var o1 = sayiOku($('#txtOdenek1').val()), o2 = sayiOku($('#txtOdenek2').val()), o3 = sayiOku($('#txtOdenek3').val());
        if (o1 === null || o2 === null || o3 === null) { mesaj('Teklif rakamları sayı olmalıdır.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/butcefisieski/kaydet', {
            yil: acik.yil, senaryo: acik.senaryo, kur: acik.kur, fon: acik.fon, fin: acik.fin, eko: acik.eko,
            gerekce: editor.html(), sebep1: $('#txtSebep1').val() || '', sebep2: $('#txtSebep2').val() || '',
            odenek1: o1, odenek2: o2, odenek3: o3
        }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            degisti = false;
            mesaj(v.bilgi || 'Bilgiler başarıyla saklandı.', 'success');
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Yazdır (eski btnTertipYazdir_Click: Word dosyası)

    $('#btnYazdir').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl seçiniz.', 'warning'); return; }
        if (kodAl(alanMetin('kur')) === '' && kodAl(alanMetin('fon')) === '') {
            mesaj('Toplu bütçe fişi yazdırma işleminda Kurumsal ve Fonksiyonel kodlarını tam olarak yazmanız gerekmektedir.', 'warning');
            return;
        }
        kriterleriSakla();
        var $d = $(this).prop('disabled', true);
        fetch(Sgb.KOK + 'api/butcefisieski/yazdir', {
            method: 'POST', credentials: 'same-origin', cache: 'no-store',
            headers: { 'Content-Type': 'application/json', RequestVerificationToken: Sgb.meta('istek-jetonu') },
            body: JSON.stringify({
                yil: yil, senaryo: $('#ddlSenaryo').val() || '', kur: acik ? acik.kur : '',
                araKur: alanMetin('kur'), araFon: alanMetin('fon'), araFin: alanMetin('fin'), araEko: alanMetin('eko')
            })
        }).then(function (y) {
            if (y.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            var tur = y.headers.get('Content-Type') || '';
            if (!y.ok || tur.indexOf('json') >= 0 || tur.indexOf('text/html') >= 0) {
                if (tur.indexOf('json') >= 0) return y.json().then(function (v) { mesaj(v.hata || 'Bütçe fişi hazırlanamadı.', 'danger'); });
                mesaj(y.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Bütçe fişi hazırlanamadı.', 'danger');
                return;
            }
            var baslik = y.headers.get('Content-Disposition') || '';
            var ad = /filename\*=UTF-8''([^;]+)/i.exec(baslik) || /filename="?([^";]+)"?/i.exec(baslik);
            return y.blob().then(function (icerik) {
                var yerel = URL.createObjectURL(icerik);
                var a = document.createElement('a');
                a.href = yerel;
                a.download = ad ? decodeURIComponent(ad[1]) : 'ButceFisi.doc';
                document.body.appendChild(a);
                a.click();
                a.remove();
                setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
            });
        }).catch(function () { mesaj('Bütçe fişi hazırlanamadı.', 'danger'); })
            .then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- kod ağaçları

    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};  // tür -> { $ul, agac, yil }
    var aktifAlan = null;

    function agacAc($alan) {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl seçiniz.', 'warning'); return; }
        var tur = $alan.attr('data-tur');
        aktifAlan = $alan;
        $('#dlgKodBaslik').text($alan.find('label').text() + ' Kod Seçimi');
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[tur];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', $alan.find('label').text() + ' kod ağacı').appendTo('#agacKutu'), yil: yil };
            // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al('api/butcefisieski/kodlar', { yil: a.yil, tur: tur, ust: ust ? ust.kod : '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    dlgKod.close();
                    if (aktifAlan) aktifAlan.find('input').val(d.ad).trigger('change');
                },
                seciliMi: function (d) { return !!aktifAlan && kodAl(aktifAlan.find('input').val()) === d.kod; },
                ilkAcik: true,
                bosMetin: 'Kod bulunamadı.',
                hata: hataGoster
            });
        } else {
            a.$ul.prop('hidden', false);
            if (a.yil !== yil) { a.yil = yil; a.agac.yenile(); }
        }
    }

    $('.kod-alan .kod-sec').on('click', function () { agacAc($(this).closest('.kod-alan')); });
    $('.kod-alan .kod-temizle').on('click', function () { $(this).closest('.kod-alan').find('input').val('').trigger('focus'); });
    // Alana kod yazılabilir; Alt+Aşağı ağacı açar.
    $('.kod-alan input').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    window.addEventListener('beforeunload', function (e) {
        if (degisti) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al('api/butcefisieski/tanim').done(function (t) {
        if (t.hata) { mesaj(t.hata, 'danger'); return; }
        var $y = Sgb.yilKutusu('#ddlYil', t.yillar);
        $y.val(saklananOku('yil', String(t.yil)));
        if (!$y.val()) $y.val(String(t.yil));

        var $s = $('#ddlSenaryo').empty();
        $.each(t.senaryolar || [], function (i, s) { $('<option></option>').val(s.kod).text(s.ad).appendTo($s); });
        $s.val(saklananOku('senaryo', t.senaryo));
        if (!$s.val()) $s.val(t.senaryo);
        if (!$s.val() && t.senaryolar && t.senaryolar.length) $s.val(t.senaryolar[0].kod);

        $('.kod-alan').each(function () { $(this).find('input').val(saklananOku('kod.' + $(this).attr('data-alan'), '')); });
        yilBasliklari();
    }).fail(hataGoster);
});
