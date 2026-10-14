// Faaliyet Gerekçesi (ButceForm/HizmetGerekcesiv2): yılın ve birimin faaliyetleri listelenir; seçilen faaliyetin gerekçe,
// dayanak ve maliyet metinleri okunur, önceki yıldan alınır, kaydedilir; Word raporu tek ya da tüm faaliyetler için alınır.
// Birleştir (yalnızca sistem yöneticisi) kurumun boş gerekçelerini birimlerden alınanlarla doldurur.
$(function () {
    'use strict';

    if (!$('#agcFaaliyet').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var acik = null;    // Faal. Listele ile açılmış ölçüt { yil, birim }: bütün işlemler bunu kullanır
    var secili = null;  // seçili faaliyet düğümü { kod, ad }
    var agac = null;
    var degisti = false; // metinlerde kaydedilmemiş değişiklik var mı?
    var $metinler = $('#txtGerekce, #txtDayanak, #txtMaliyet');

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }

    function birimleriDoldur(liste) {
        var $ddl = $('#ddlBirim'), onceki = $ddl.val();
        $ddl.empty();
        $.each(liste || [], function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($ddl); });
        if (onceki) $ddl.val(onceki);
        if ($ddl.val() === null && $ddl.children().length) $ddl.prop('selectedIndex', 0);
    }

    function dugmeler() {
        var faal = !!secili;
        $('#btnOku, #btnOnceki, #btnKaydet, #btnYazdirTek').prop('disabled', !faal);
        $('#btnYazdirTum').prop('disabled', !acik);
        $metinler.prop('disabled', !faal);
    }

    function metinleriYaz(g) {
        $('#txtGerekce').val(g ? g.gerekce : '');
        $('#txtDayanak').val(g ? g.dayanak : '');
        $('#txtMaliyet').val(g ? g.maliyet : '');
    }

    function vazgecilsinMi() {
        return !degisti || window.confirm('Kaydedilmemiş değişiklikler var. Devam edilirse kaybolacak. Devam edilsin mi?');
    }

    $metinler.on('input', function () { degisti = true; });
    window.addEventListener('beforeunload', function (olay) {
        if (degisti) { olay.preventDefault(); olay.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/hizmetgerekce/tanim')
        .done(function (veri) {
            if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
            $('#txtYil').val(veri.yil);
            birimleriDoldur(veri.birimler);
            $('#btnBirlestir').prop('hidden', !veri.birlestirebilir);
        })
        .fail(hataGoster);

    // Birimler yıla bağlıdır: yıl değişince o yılın birimleri okunur.
    $('#txtYil').on('change', function () {
        var yil = yilOku();
        if (yil < 2000) return;
        Sgb.al('api/hizmetgerekce/birimler', { yil: yil })
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                birimleriDoldur(veri.birimler);
            })
            .fail(hataGoster);
    });

    // ---------------------------------------------------------------- faaliyetler

    function faaliyetleriYukle() {
        return Sgb.al('api/hizmetgerekce/faaliyetler', acik).then(function (veri) {
            if (veri.hata) { mesaj(veri.hata, 'warning'); return []; }
            return veri.dugumler;
        });
    }

    $('#frmFaaliyet').on('submit', function (olay) {
        olay.preventDefault();
        var o = { yil: yilOku(), birim: $('#ddlBirim').val() || '' };
        if (o.yil < 2000 || !o.birim) { mesaj('Yıl giriniz ve birim seçiniz', 'warning'); return; }
        if (!vazgecilsinMi()) return;
        acik = o;
        secili = null;
        degisti = false;
        metinleriYaz(null);
        $('#faaliyetBaslik').text('Faaliyet seçiniz');
        dugmeler();
        if (agac) { agac.yenile(); return; }
        agac = Sgb.agac('#agcFaaliyet', {
            duz: true,
            yukle: faaliyetleriYukle,
            bosMetin: 'Birime bağlı faaliyet yok.',
            seciliMi: function (d) { return secili && secili.kod === d.kod; },
            secilince: faaliyetSecildi,
            hata: hataGoster
        });
    });

    function faaliyetSecildi(d) {
        if (secili && secili.kod === d.kod) return;
        if (!vazgecilsinMi()) {
            // Seçim eski faaliyette kalır.
            $('#agcFaaliyet .sec').each(function () {
                var dd = $(this).closest('li').data('dugum');
                $(this).attr('aria-current', dd && secili && dd.kod === secili.kod ? 'true' : null);
            });
            return;
        }
        secili = d;
        $('#faaliyetBaslik').text(d.ad);
        oku(false);
    }

    // ---------------------------------------------------------------- metinler

    // onceki: Önceki Yıldan Al (bir önceki yılın metinleri; kaydedilene kadar seçili yıla yazılmaz).
    function oku(onceki) {
        if (!secili) return;
        var $d = $('#btnOku, #btnOnceki').prop('disabled', true);
        Sgb.al('api/hizmetgerekce/oku', { yil: acik.yil, birim: acik.birim, faal: secili.kod, onceki: onceki })
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                metinleriYaz(veri);
                degisti = onceki;
                dugmeler();
                if (onceki) mesaj((acik.yil - 1) + ' yılının metinleri alındı; saklamak için Kaydet\'e basınız.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', !secili); });
    }

    $('#btnOku').on('click', function () { if (vazgecilsinMi()) oku(false); });
    $('#btnOnceki').on('click', function () { if (vazgecilsinMi()) oku(true); });

    $('#btnKaydet').on('click', function () {
        if (!secili) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/hizmetgerekce/kaydet', {
            yil: acik.yil,
            birim: acik.birim,
            faalKod: parseInt(secili.kod, 10),
            gerekce: $('#txtGerekce').val(),
            dayanak: $('#txtDayanak').val(),
            maliyet: $('#txtMaliyet').val()
        })
            .done(function (veri) {
                if (!veri.basarili) { mesaj(veri.hata, 'danger'); return; }
                degisti = false;
                mesaj(veri.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', !secili); });
    });

    // ---------------------------------------------------------------- yazdır (Word)

    function yazdir(faal, $d) {
        if (!acik) return;
        $d.prop('disabled', true);
        Sgb.indir('api/hizmetgerekce/rapor', { yil: acik.yil, birim: acik.birim, faal: faal })
            .finally(function () { dugmeler(); });
    }

    $('#btnYazdirTek').on('click', function () { if (secili) yazdir(secili.kod, $(this)); });
    $('#btnYazdirTum').on('click', function () { yazdir(0, $(this)); });

    // ---------------------------------------------------------------- birleştir

    var dlgBirlestir = document.getElementById('dlgBirlestir');
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    function birlestirYili() { return acik ? acik.yil : yilOku(); }

    $('#btnBirlestir').on('click', function () {
        var yil = birlestirYili();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        $('#birlestirMetin').text(yil + ' yılında kurumun boş olan faaliyet gerekçeleri, birimlerden alınanlarla doldurulacak. Devam edilsin mi?');
        dlgBirlestir.showModal();
    });

    $('#btnBirlestirOnay').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/hizmetgerekce/birlestir', { yil: birlestirYili() })
            .done(function (veri) {
                dlgBirlestir.close();
                mesaj(veri.basarili ? veri.bilgi : veri.hata, veri.basarili ? 'success' : 'danger');
            })
            .fail(function (xhr) { dlgBirlestir.close(); hataGoster(xhr); })
            .always(function () { $d.prop('disabled', false); });
    });

    dugmeler();
});
