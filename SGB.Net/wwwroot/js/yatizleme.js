// Yatırım Programı İzleme ve Değerlendirme (ButceHazirlikv2/YATIzlemeDegerlendirme): ProjeGiris islemKod=1 ekranında
// proje ya da alt proje seçilince sağdaki çerçevede açılır. Adres parametreleri eski sayfanınkiyle aynıdır:
//   proje     : ?menuYok=1&yil=..&kod=<projeKod>&senaryoKod=..
//   alt proje : ?menuYok=1&yil=..&apKod=..&pKod=..&senaryoKod=..
// (eski Page_Load: apKod yoksa proje kodu "kod"dan, varsa "pKod"dan okunur; senaryo her zaman 06.00'dır.)
// Seçilen aşamanın Pr. ve Gerç. Durumu metni, ödenek tablosu ve Tamamlama Raporu okunur / saklanır; aşama onaylanır;
// sistem yöneticisi Genel Değerlendirme penceresinde aşamanın bütün projeler için genel metinlerini girer.
$(function () {
    'use strict';

    if (!$('#txtGerekce').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var adres = new URLSearchParams(location.search);
    function sayi(ad) { return parseInt(adres.get(ad), 10) || 0; }
    var YIL = sayi('yil');
    var AP_KOD = sayi('apKod');
    var P_KOD = AP_KOD === 0 ? sayi('kod') : sayi('pKod');
    var projeVar = YIL >= 2000 && (P_KOD > 0 || AP_KOD > 0);

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    // Uç { hata } döndürdüyse gösterir ve true döner.
    function hataVar(veri) {
        if (veri && veri.hata) { mesaj(veri.hata, 'danger'); return true; }
        return false;
    }

    function asama() { return parseInt($('#ddlAsama').val(), 10) || 1; }
    function kimlik() { return { yil: YIL, pKod: P_KOD, apKod: AP_KOD, asama: asama() }; }

    // İşlem sürerken tuşu kapatır; bitince açar.
    function bekle($tus, soz) {
        $tus.prop('disabled', true);
        return soz.always(function () { $tus.prop('disabled', false); });
    }

    // ---------------------------------------------------------------- sekmeler (sayfadaki ve penceredeki)

    $('.izleme-sekmeler').on('click', 'a', function (olay) {
        olay.preventDefault();
        var $a = $(this), $ul = $a.closest('ul');
        $ul.find('li').removeClass('active').find('a').attr('aria-selected', 'false');
        $a.attr('aria-selected', 'true').parent().addClass('active');
        $ul.find('a').each(function () { $($(this).attr('href')).prop('hidden', this !== $a[0]); });
    });

    // ---------------------------------------------------------------- editör ve form

    var edGerekce = Sgb.editor('#txtGerekce', { yukseklik: 380 });

    function tusAyarla(t) {
        // Eski TuslariAyarla: onaylıysa Kaydet ve Onayla gizli; Onay Kaldır yalnızca yöneticiye.
        t = t || {};
        $('#btnKaydet').prop('hidden', !t.kaydet);
        $('#btnOnayla').prop('hidden', !t.onayla);
        $('#btnOnayKaldir').prop('hidden', !t.onayKaldir);
        $('#onayEtiket').prop('hidden', !t.onayli);
    }

    function odenekYaz(o) {
        o = o || {};
        $('#odnProgram').text(o.program || '-');
        $('#odnRevize').text(o.revize || '-');
        $('#odnHarcanan').text(o.harcanan || '-');
        $('#odnOran').text(o.oran || '-');
    }

    function doldur(veri) {
        if (veri.asama) $('#ddlAsama').val(String(veri.asama));
        var m = veri.metinler || {};
        edGerekce.html(m.gerekce || '');
        $('#frmTamam [data-alan]').each(function () { $(this).val(m[$(this).attr('data-alan')] || ''); });
        odenekYaz(veri.odenek);
        tusAyarla(veri.tuslar);
    }

    // Açılışta aşama verilmez: kullanıcının en son okuduğu aşama gelir (eski AsamaGetir).
    function oku(asamaIle) {
        var p = { yil: YIL, pKod: P_KOD, apKod: AP_KOD };
        if (asamaIle) p.asama = asama();
        return Sgb.al('api/yatizleme/oku', p).done(function (veri) {
            if (hataVar(veri)) return;
            doldur(veri);
        }).fail(hataGoster);
    }

    // Eski btnOku_Click: önce aşama saklanır (AsamaSakla), sonra okunur.
    $('#btnOku').on('click', function () {
        var $t = $(this);
        bekle($t, Sgb.gonder('api/yatizleme/asama', kimlik()).then(function () { return oku(true); }, function () { return oku(true); }));
    });

    // Eski cmbAsama_Changed: yalnızca tuşlar yeni aşamanın onay durumuna göre ayarlanır (metinler Oku ile gelir).
    $('#ddlAsama').on('change', function () {
        Sgb.al('api/yatizleme/tuslar', kimlik()).done(function (veri) {
            if (hataVar(veri)) return;
            tusAyarla(veri.tuslar);
        }).fail(hataGoster);
    });

    function kaydet($tus, uc, veri) {
        bekle($tus, Sgb.gonder(uc, $.extend(kimlik(), veri)).done(function (s) {
            if (hataVar(s)) return;
            mesaj(s.bilgi || 'Kayıt tamamlandı', 'success');
        }).fail(hataGoster));
    }

    $('#btnKaydet').on('click', function () {
        kaydet($(this), 'api/yatizleme/gerekce', { gerekce: edGerekce.html() });
    });

    $('#btnKaydetTamam').on('click', function () {
        var veri = {};
        $('#frmTamam [data-alan]').each(function () { veri[$(this).attr('data-alan')] = $(this).val(); });
        kaydet($(this), 'api/yatizleme/tamamlama', veri);
    });

    // ---------------------------------------------------------------- onay

    function onay($tus, kaldir) {
        bekle($tus, Sgb.gonder('api/yatizleme/onay', $.extend(kimlik(), { kaldir: kaldir })).done(function (s) {
            if (hataVar(s)) return;
            tusAyarla(s.tuslar);
            mesaj(s.bilgi, 'success');
        }).fail(hataGoster));
    }
    $('#btnOnayla').on('click', function () { onay($(this), false); });
    $('#btnOnayKaldir').on('click', function () { onay($(this), true); });

    // ---------------------------------------------------------------- raporlar (Rapor.xml 2004-14, 2004-16)

    function rapor($tus, n) {
        $tus.prop('disabled', true);
        Sgb.indir('api/yatizleme/rapor', $.extend({ n: n }, kimlik())).then(function () { $tus.prop('disabled', false); });
    }
    $('[data-rapor="14"], [data-rapor="16"]').on('click', function () { rapor($(this), $(this).attr('data-rapor')); });

    // ---------------------------------------------------------------- Genel Değerlendirme (yalnızca sistem yöneticisi)

    var dlgGenel = document.getElementById('dlgGenel');
    var genelEditorler = null; // { alan: editör }, pencere ilk açılışta kurulur

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    if (dlgGenel) {
        var $menu = $('#dlgGenel .rapor-menu');
        function menuKapat() { $menu.removeClass('open'); $('#btnGenelYazdir').attr('aria-expanded', 'false'); }

        // Eski BeforeShow: pencere her açılışta seçili aşamanın metinlerini okur.
        $('#btnGenelGoster').on('click', function () {
            if (!genelEditorler) {
                genelEditorler = {};
                $('#dlgGenel textarea[data-alan]').each(function () {
                    genelEditorler[$(this).attr('data-alan')] = Sgb.editor(this, { yukseklik: 360 });
                });
            }
            $('#genelAsama').text('— ' + $('#ddlAsama option:selected').text());
            $.each(genelEditorler, function (k, ed) { ed.html(''); });
            dlgGenel.showModal();
            Sgb.al('api/yatizleme/genel', { yil: YIL, asama: asama() }).done(function (veri) {
                if (hataVar(veri)) return;
                var m = veri.metinler || {};
                $.each(genelEditorler, function (k, ed) { ed.html(m[k] || ''); });
            }).fail(hataGoster);
        });

        $('#btnGenelKaydet').on('click', function () {
            var veri = { yil: YIL, asama: asama() };
            $.each(genelEditorler || {}, function (k, ed) { veri[k] = ed.html(); });
            bekle($(this), Sgb.gonder('api/yatizleme/genel', veri).done(function (s) {
                if (hataVar(s)) return;
                mesaj(s.bilgi || 'Kayıt tamamlandı', 'success');
            }).fail(hataGoster));
        });

        // Yazdır menüsü (Bootstrap betiği yüklenmediğinden .open ile açılır): 2004-15 Genel Değerlendirme, 2004-17 Sektörlere Göre.
        $('#btnGenelYazdir').on('click', function (e) {
            e.stopPropagation();
            var ac = !$menu.hasClass('open');
            $menu.toggleClass('open', ac);
            $(this).attr('aria-expanded', ac ? 'true' : 'false');
            if (ac) $menu.find('.dropdown-menu button').first().trigger('focus');
        });
        $(dlgGenel).on('click', menuKapat);
        $(dlgGenel).on('keydown', function (e) {
            if (e.key === 'Escape' && $menu.hasClass('open')) { e.preventDefault(); menuKapat(); $('#btnGenelYazdir').trigger('focus'); }
        });
        $menu.on('click', '.dropdown-menu button', function () {
            menuKapat();
            var $t = $('#btnGenelYazdir').prop('disabled', true);
            Sgb.indir('api/yatizleme/rapor', { n: $(this).attr('data-rapor'), yil: YIL, asama: asama() })
                .then(function () { $t.prop('disabled', false); });
        });
    }

    // ---------------------------------------------------------------- açılış

    if (!projeVar) {
        $('#btnOku, #btnKaydet, #btnKaydetTamam, [data-rapor="14"], [data-rapor="16"]').prop('disabled', true);
        mesaj('Proje ya da alt proje seçilmemiş.', 'warning');
        return;
    }
    oku(false);
});
