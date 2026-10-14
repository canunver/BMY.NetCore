// Bütçe finans girişi (ButceHazirlikv2/ButceFinansGiris): varsayılan kurumun gider (toplam, sermaye 06-07, diğer) ve gelir (toplam, öz gelir)
// rakamları okunur, finansman tutarı girilip saklanır. Haz. Yar. Hesapla eski sayfadaki gibi tarayıcıda hesaplar: finansman farkı
// (toplam gider - finansman - öz gelir) önce sermaye ödeneklerine kadar sermaye yardımına, kalanı cari yardıma yazılır.
// Yazdır Form 18'i (finansman cetveli) eski rapor koduyla indirir.
$(function () {
    'use strict';

    var $sayfa = $('.butce-finans');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });
    var acik = null; // okunmuş seçim: { yil, senaryo }

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur || 'info'); }

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }

    // Eski "#,##0" biçimi
    function yaz(sel, d) { $(sel).val(tamSayi.format(Math.round(d || 0))); }
    function sayiOku(metin) {
        var s = $.trim(metin || '').replace(/\./g, '').replace(/\s/g, '').replace(',', '.');
        if (s === '') return 0;
        var d = parseFloat(s);
        return isNaN(d) ? null : d;
    }
    function oku(sel) { return sayiOku($(sel).val()) || 0; }

    // ---------------------------------------------------------------- Yenile (eski TablolariDoldur)

    function listele() {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var senaryo = $('#ddlSenaryo').val() || '';
        var $d = $('#btnListele').prop('disabled', true);
        Sgb.al('api/butcefinans/bilgi', { yil: yil, senaryo: senaryo }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            acik = { yil: yil, senaryo: senaryo };
            yaz('#txtFinansmanTutar', v.finansman);
            yaz('#lblGiderToplamOdenek', v.giderToplam);
            yaz('#lblGiderSermayeOdenek', v.giderSermaye);
            yaz('#lblGiderDigerOdenek', v.giderDiger);
            yaz('#lblGelirToplam', v.gelirToplam);
            yaz('#lblGelirOzGelir', v.gelirOzGelir);
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    }

    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(); });
    $('#txtYil, #ddlSenaryo').on('change', function () {
        if (acik && (yilOku() !== acik.yil || ($('#ddlSenaryo').val() || '') !== acik.senaryo)) listele();
    });

    // Tutar alanları bırakılınca binlik ayraçla yazılır.
    $('#txtFinansmanTutar, #txtHazineSermaye, #txtHazineCari').on('blur', function () {
        var d = sayiOku(this.value);
        if (d !== null && $.trim(this.value) !== '') this.value = tamSayi.format(d);
    });

    // ---------------------------------------------------------------- Haz. Yar. Hesapla (eski Hesapla)

    $('#btnHesapla').on('click', function () {
        if (!acik) { mesaj('Önce rakamları okuyunuz (Yenile).', 'warning'); return; }
        var finansman = sayiOku($('#txtFinansmanTutar').val());
        if (finansman === null) { mesaj('Finansman tutarı sayı olmalıdır.', 'warning'); return; }
        var finansmanOzGelir = finansman + oku('#lblGelirOzGelir');
        var toplamGider = oku('#lblGiderToplamOdenek');
        var fark = toplamGider - finansmanOzGelir;
        yaz('#lblFinansmanOzGelir', finansmanOzGelir);
        yaz('#lblFinansmanToplamGider', toplamGider);
        yaz('#lblFinansmanFark', fark);

        var sermaye = oku('#lblGiderSermayeOdenek');
        if (fark > 0) {
            if (fark > sermaye) {
                yaz('#txtHazineSermaye', sermaye);
                yaz('#txtHazineCari', fark - sermaye);
            } else {
                yaz('#txtHazineSermaye', fark);
                yaz('#txtHazineCari', 0);
            }
        }
        yaz('#lblGelirToplam', oku('#lblGelirOzGelir') + oku('#txtHazineSermaye') + oku('#txtHazineCari'));
    });

    // ---------------------------------------------------------------- Kaydet (eski btnFinansKaydet_Click)

    $('#btnKaydet').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var finansman = sayiOku($('#txtFinansmanTutar').val());
        if (finansman === null) { mesaj('Finansman tutarı sayı olmalıdır.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/butcefinans/kaydet', { yil: yil, senaryo: $('#ddlSenaryo').val() || '', finansman: finansman }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            mesaj(v.bilgi || 'Kayıt işlemi başarıyla tamamlandı.', 'success');
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Yazdır (eski btnYazdir_Click: Form 18)

    $('#btnYazdir').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        fetch(Sgb.KOK + 'api/butcefinans/yazdir', {
            method: 'POST', credentials: 'same-origin', cache: 'no-store',
            headers: { 'Content-Type': 'application/json', RequestVerificationToken: Sgb.meta('istek-jetonu') },
            body: JSON.stringify({ yil: yil, senaryo: $('#ddlSenaryo').val() || '' })
        }).then(function (y) {
            if (y.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            var tur = y.headers.get('Content-Type') || '';
            if (!y.ok || tur.indexOf('json') >= 0 || tur.indexOf('text/html') >= 0) {
                if (tur.indexOf('json') >= 0) return y.json().then(function (v) { mesaj(v.hata || 'Rapor hazırlanamadı.', 'danger'); });
                mesaj(y.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Rapor hazırlanamadı.', 'danger');
                return;
            }
            var baslik = y.headers.get('Content-Disposition') || '';
            var ad = /filename\*=UTF-8''([^;]+)/i.exec(baslik) || /filename="?([^";]+)"?/i.exec(baslik);
            return y.blob().then(function (icerik) {
                var yerel = URL.createObjectURL(icerik);
                var a = document.createElement('a');
                a.href = yerel;
                a.download = ad ? decodeURIComponent(ad[1]) : 'Form18.xls';
                document.body.appendChild(a);
                a.click();
                a.remove();
                setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
            });
        }).catch(function () { mesaj('Rapor hazırlanamadı.', 'danger'); })
            .then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load: rakamlar hemen okunur)

    Sgb.al('api/butcefinans/tanim').done(function (t) {
        if (t.hata) { mesaj(t.hata, 'danger'); return; }
        $('#txtYil').val(t.yil);
        $('#lblKurum').text(t.kurumAd || '');
        var $s = $('#ddlSenaryo').empty();
        $.each(t.senaryolar || [], function (i, s) { $('<option></option>').val(s.kod).text(s.ad).appendTo($s); });
        $s.val(t.senaryo);
        if (!$s.val() && t.senaryolar && t.senaryolar.length) $s.val(t.senaryolar[0].kod);
        listele();
    }).fail(hataGoster);
});
