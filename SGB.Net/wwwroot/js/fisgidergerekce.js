// Gider Cetveli Açıklaması Girişi (KesinHesap/FisGiderGerekce): yıl ve gider tertibi seçilir; tertibin bütçe değerleri ve
// gerekçesi okunur (gerekçe yoksa şablondan üretilir, kırmızı görünür), kaydedilir; Word raporu tablolu / tablosuz alınır.
// Sistem yöneticisine Verileri Yükle (Excel) ve Gerekçeleri Üret tuşları da görünür.
$(function () {
    'use strict';

    if (!$('#txtGerekce').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var acik = null;     // Listele ile okunmuş { yil, tertip }: Kaydet / Şablon / Üret bunu kullanır
    var degisti = false; // gerekçede kaydedilmemiş değişiklik var mı?
    var sayi = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var $gerekce = $('#txtGerekce');

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function tertipOku() { return $('#ddlTertip').val() || ''; }

    function dugmeler() {
        var tertip = !!tertipOku();
        $('#btnListele').prop('disabled', !tertip);
        $('#btnKaydet, #btnSablon, #btnUret').prop('disabled', !acik);
        $gerekce.prop('disabled', !acik);
    }

    function sablonIsaretle(sablon) {
        $gerekce.toggleClass('kirmizi', !!sablon);
        $('#sablonNot').prop('hidden', !sablon);
    }

    function degerleriYaz(d, ipucu) {
        var $s = $('#degerSatir').empty();
        if (!d) {
            $('<tr><td colspan="6" class="bos-ipucu"></td></tr>').find('td').text(ipucu || 'Tertip seçiniz.').end().appendTo($s);
            return;
        }
        var $tr = $('<tr></tr>');
        $.each([d.odenek, d.harcama, d.iptal, d.odenekUstu, d.eklenen, d.dusulen], function (i, v) {
            $('<td></td>').text(sayi.format(v || 0)).appendTo($tr);
        });
        $tr.appendTo($s);
    }

    function temizle() {
        acik = null;
        degisti = false;
        $gerekce.val('');
        sablonIsaretle(false);
        degerleriYaz(null);
        dugmeler();
    }

    function vazgecilsinMi() {
        return !degisti || window.confirm('Kaydedilmemiş değişiklikler var. Devam edilirse kaybolacak. Devam edilsin mi?');
    }

    $gerekce.on('input', function () { degisti = true; });
    window.addEventListener('beforeunload', function (olay) {
        if (degisti) { olay.preventDefault(); olay.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış ve tertipler

    function tertipleriDoldur() {
        var yil = yilOku();
        var $ddl = $('#ddlTertip');
        if (yil < 2007) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $('#btnTertipYenile').prop('disabled', true);
        Sgb.al('api/fisgidergerekce/tertipler', { yil: yil })
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                $ddl.empty();
                $('<option value=""></option>').text('Tertip seçiniz').appendTo($ddl);
                $.each(veri.tertipler || [], function (i, t) { $('<option></option>').val(t).text(t).appendTo($ddl); });
                temizle();
                if (!(veri.tertipler || []).length) degerleriYaz(null, 'Bu yıl için gider tertibi yok.');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    }

    Sgb.al('api/fisgidergerekce/tanim')
        .done(function (veri) {
            if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
            $('#txtYil').attr({ min: veri.enAz, max: veri.enCok }).val(veri.yil);
            $('#btnVeriYukle, #btnUret').prop('hidden', !veri.yonetici);
            tertipleriDoldur();
        })
        .fail(hataGoster);

    $('#btnTertipYenile').on('click', function () { if (vazgecilsinMi()) tertipleriDoldur(); });
    $('#txtYil').on('change', function () { if (vazgecilsinMi()) tertipleriDoldur(); });

    // ---------------------------------------------------------------- listele (eski btnListele_Click)

    function listele() {
        var o = { yil: yilOku(), tertip: tertipOku() };
        if (o.yil < 2007 || !o.tertip) { temizle(); return; }
        var $d = $('#btnListele').prop('disabled', true);
        degerleriYaz(null, 'Okunuyor...');
        Sgb.al('api/fisgidergerekce/oku', o)
            .done(function (veri) {
                if (veri.hata) { temizle(); mesaj(veri.hata, 'danger'); return; }
                acik = o;
                $gerekce.val(veri.gerekce || '');
                degerleriYaz(veri.degerler, 'Tertibin bütçe değeri yok.');
                sablonIsaretle(veri.sablon);
                // Şablondan gelen metin kaydedilmemiştir.
                degisti = !!veri.sablon && !!veri.gerekce;
                dugmeler();
            })
            .fail(function (xhr) { temizle(); hataGoster(xhr); })
            .always(function () { dugmeler(); });
    }

    $('#btnListele').on('click', function () { if (vazgecilsinMi()) listele(); });

    var oncekiTertip = '';
    $('#ddlTertip').on('focus', function () { oncekiTertip = tertipOku(); })
        .on('change', function () {
            if (!vazgecilsinMi()) { $(this).val(acik ? acik.tertip : oncekiTertip); return; }
            oncekiTertip = tertipOku();
            listele();
        });

    // ---------------------------------------------------------------- şablon ve kaydet

    $('#btnSablon').on('click', function () {
        if (!acik) return;
        var $d = $(this).prop('disabled', true);
        Sgb.al('api/fisgidergerekce/sablon', acik)
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                $gerekce.val(veri.gerekce || '');
                sablonIsaretle(true);
                degisti = true;
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', !acik); });
    });

    $('#btnKaydet').on('click', function () {
        if (!acik) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/fisgidergerekce/kaydet', { yil: acik.yil, tertip: acik.tertip, gerekce: $gerekce.val() })
            .done(function (veri) {
                if (!veri.basarili) { mesaj(veri.hata, 'danger'); return; }
                degisti = false;
                sablonIsaretle(false);
                mesaj(veri.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', !acik); });
    });

    // ---------------------------------------------------------------- yazdır (Word; eski RTF şablonu)

    function yazdir(tablosuz, $d) {
        var yil = yilOku();
        if (yil < 2007) { mesaj('Yıl hatalı!', 'warning'); return; }
        $d.prop('disabled', true);
        Sgb.indir('api/fisgidergerekce/rapor', { yil: yil, tablosuz: tablosuz })
            .finally(function () { $d.prop('disabled', false); });
    }

    $('#btnYazdir').on('click', function () { yazdir(false, $(this)); });
    $('#btnYazdirTablosuz').on('click', function () { yazdir(true, $(this)); });

    // ---------------------------------------------------------------- yönetici: verileri yükle, gerekçeleri üret

    $('#btnVeriYukle').on('click', function () {
        if (yilOku() < 2007) { mesaj('Yıl hatalı!', 'warning'); return; }
        $('#dosyaYukle').val('').trigger('click');
    });

    $('#dosyaYukle').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        if (!window.confirm('İçinde 14. satırdan itibaren ve E Sütununda tertip (Prg-Kur-Fin-Eko), F KBÖ, G Eklenen, H Düşülen, ' +
            'N Gönderilen, P Harcama, S Kurum İçi Ekleme, T Yedek Ekleme, U Kurum İçi Düşme verileri olan dosyadaki tertiplere ait ' +
            'veriler, mevcut verilerle değiştirilecektir, onaylıyor musunuz?')) return;
        var veri = new FormData();
        veri.append('yil', yilOku());
        veri.append('dosya', dosya);
        var $d = $('#btnVeriYukle').prop('disabled', true);
        $.ajax({
            url: '../api/fisgidergerekce/yukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            mesaj(v.basarili ? v.bilgi : v.hata, v.basarili ? 'success' : 'danger');
            if (v.basarili && acik) listele();
        }).fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnUret').on('click', function () {
        if (!acik) return;
        if (!window.confirm('Ekranda seçili tertibin, programında ve seçili tertibin altındaki tüm tertipler için gerekçeler üretilecektir. Onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/fisgidergerekce/uret', { yil: acik.yil, tertip: acik.tertip })
            .done(function (veri) {
                mesaj(veri.basarili ? veri.bilgi : veri.hata, veri.basarili ? 'info' : 'danger');
                if (veri.basarili) { degisti = false; listele(); }
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', !acik); });
    });

    dugmeler();
});
