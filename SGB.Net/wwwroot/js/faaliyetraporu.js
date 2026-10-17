// Faaliyet Raporu (ButceHazirlikv2/FaaliyetRaporuForm; eski FaaliyetRaporuForm.aspx).
// Açılışta bu yılın metinleri okunur (eski Page_Load). Bölüm 4, 4.1, 4.2 ve Tablo 1..15 notları ortak HTML editöründe
// (editor.js) yazılır; tablo notlarının başlığı yıla göredir. Kaydet yalnızca sistem yöneticisine görünür, diğerlerine editörler
// salt okunurdur. Raporlar menüsü: gruplama (grupsuz / sektör / birim), sistem yöneticisine "PDF olarak" seçimi ve raporlar
// (eski rapor kodu; sistem yöneticisi olmayana hep PDF).
$(function () {
    'use strict';

    if (!$('#bolumler').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var SAYI = 18;           // 3 bölüm + 15 tablo notu (sunucudaki FaaliyetRaporuBilgi sırası)
    var editorler = [];
    var okunanYil = null;
    var degisti = false;
    var yonetici = false;
    var yukleniyor = false;  // editörlere metin yazılırken "değişti" sayılmaz

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) {
        $('#btnOku, #btnRapor').prop('disabled', evet);
        $('#btnKaydet').prop('disabled', evet || okunanYil === null);
    }

    // ---------------------------------------------------------------- editörler

    // Bölümler büyük, tablo notları küçük editör (eski 500 / 200 piksel).
    for (var i = 0; i < SAYI; i++) {
        var bolum = i < 3;
        var kimlik = 'txtMetin' + i;
        var $kutu = $('<div class="metin-kutu"></div>').toggleClass('metin-bolum', bolum).toggleClass('metin-not', !bolum)
            .appendTo(bolum ? '#bolumler' : '#notlar');
        $('<label></label>').attr('for', kimlik).attr('id', 'lblMetin' + i).text(bolum ? 'Bölüm ' + (i + 1) : 'Tablo ' + (i - 2) + ' Notu').appendTo($kutu);
        $('<textarea></textarea>').attr('id', kimlik).appendTo($kutu);
        var ed = Sgb.editor('#' + kimlik, { yukseklik: bolum ? 420 : 200 });
        ed.jodit.events.on('change', function () { if (!yukleniyor) degisti = true; });
        editorler.push(ed);
    }

    function saltOkunur(evet) {
        $.each(editorler, function (i, ed) { ed.jodit.setReadOnly(evet); });
    }

    function vazgecilsinMi() {
        return !degisti || window.confirm('Yapmış olduğunuz değişiklikler varsa, kaybolacaktır. Devam etmek istiyor musunuz?');
    }

    $(window).on('beforeunload', function (e) {
        if (!degisti || !yonetici) return;
        e.preventDefault();
        return '';
    });

    // ---------------------------------------------------------------- Oku / Kaydet

    function oku() {
        var yil = parseInt($('#txtYil').val(), 10) || 0;
        if (yil < 1000) { mesaj('Lütfen yıl giriniz.', 'warning'); $('#txtYil').trigger('focus'); return; }
        mesgul(true);
        Sgb.al('api/faaliyetraporu/oku', { yil: yil })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                yukleniyor = true;
                $.each(v.metinler || [], function (i, m) {
                    $('#lblMetin' + m.sira).text(m.baslik);
                    if (editorler[m.sira]) editorler[m.sira].html(m.metin || '');
                });
                // Editör "change" olayını gecikmeli de verebilir; okunan metin değişiklik sayılmasın.
                setTimeout(function () { yukleniyor = false; degisti = false; }, 300);
                degisti = false;
                okunanYil = v.yil;
                $('#lblOkunan').text(okunanYil + ' yılı');
            })
            .fail(function (xhr) { yukleniyor = false; hataGoster(xhr); })
            .always(function () { mesgul(false); });
    }

    $('#frmSecim').on('submit', function (e) {
        e.preventDefault();
        if (vazgecilsinMi()) oku();
    });

    $('#btnKaydet').on('click', function () {
        if (okunanYil === null) return;
        var yil = parseInt($('#txtYil').val(), 10) || 0;
        if (yil !== okunanYil) {
            mesaj('Metinler ' + okunanYil + ' yılı için okundu; yıl değiştiyse önce Oku\'ya basınız.', 'warning');
            return;
        }
        if (!window.confirm('Kaydetmek istiyor musunuz?')) return;
        var istek = { yil: okunanYil, metinler: $.map(editorler, function (ed) { return [ed.html()]; }) };
        mesgul(true);
        Sgb.gonder('api/faaliyetraporu/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                degisti = false;
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // ---------------------------------------------------------------- Raporlar menüsü

    // Bootstrap betiği yüklenmediğinden açılır menü .open ile açılır.
    var $grup = $('#grpRapor');
    function menuKapat() { $grup.removeClass('open'); $('#btnRapor').attr('aria-expanded', 'false'); }
    $('#btnRapor').on('click', function (e) {
        e.stopPropagation();
        var ac = !$grup.hasClass('open');
        $grup.toggleClass('open', ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false');
        if (ac) $('#lstRapor button').first().trigger('focus');
    });
    $grup.find('.dropdown-menu').on('click', function (e) { e.stopPropagation(); });
    $(document).on('click', menuKapat);
    $(document).on('keydown', function (e) {
        if (e.key === 'Escape' && $grup.hasClass('open')) { menuKapat(); $('#btnRapor').trigger('focus'); }
    });

    $('#lstRapor').on('click', 'button', function () {
        var yil = parseInt($('#txtYil').val(), 10) || 0;
        if (yil < 1000) { mesaj('Lütfen yıl giriniz.', 'warning'); return; }
        menuKapat();
        var $d = $('#btnRapor').prop('disabled', true);
        Sgb.indir('api/faaliyetraporu/rapor', {
            rapor: $(this).attr('data-rapor'),
            yil: yil,
            grup: $grup.find('input[name=grup]:checked').val() || 0,
            yikob: 0,
            pdf: yonetici && !$('#chkPdf').prop('checked') ? 0 : 1
        }).then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açılış

    mesgul(true);
    Sgb.al('api/faaliyetraporu/tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); mesgul(false); return; }
            yonetici = !!v.sistemYoneticisi;
            $('#btnKaydet').prop('hidden', !yonetici);
            $('.rapor-pdf').prop('hidden', !yonetici);
            saltOkunur(!yonetici);
            var $l = $('#lstRapor').empty();
            $.each(v.raporlar || [], function (i, r) {
                $('<button type="button" role="menuitem"></button>').attr('data-rapor', r.kod).text(r.ad).appendTo($l);
            });
            $('#txtYil').val(v.yil);
            oku();
        })
        .fail(function (xhr) { hataGoster(xhr); mesgul(false); });
});
