// Proje / alt proje bilgi sayfaları (ProjeDetay ve AltProjeDetay sekmelerinde çerçevede açılır):
//   AltProjeAmacGiris (Amaç ve Kapsam), AltProjeIlgiliGiris (İlgililer), AltProjeGerekceGiris (Gerekçe ve Hedefler).
// Alanlar sayfada data-alan ile işaretlidir; data-html olanlar HTML editörüdür (Sgb.editor). Kaydet ve Önceki Yıldan Al
// (bir önceki yılın değerleri getirilir; Kaydet'e basılana kadar saklanmaz). Adres: ?projeKodu&altProjeKodu&yil&senaryoKod.
$(function () {
    'use strict';

    var $kok = $('.yat-bilgi');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var sayfa = $kok.data('sayfa');
    var p = new URLSearchParams(location.search);
    var olcut = {
        sayfa: sayfa,
        yil: parseInt(p.get('yil'), 10) || 0,
        senaryo: p.get('senaryoKod') || '',
        projeKodu: parseInt(p.get('projeKodu'), 10) || 0,
        altProjeKodu: parseInt(p.get('altProjeKodu'), 10) || 0
    };

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // Yıla göre etiketler (eski Page_Load: "{0} YILI PLANI" ...).
    $kok.find('[data-etiket]').each(function () {
        var metin = $(this).attr('data-etiket').replace(/\{yil([+-]\d+)?\}/g, function (m, fark) {
            return olcut.yil ? String(olcut.yil + (parseInt(fark, 10) || 0)) : '';
        });
        $(this).text(metin.trim());
    });

    var editorler = {};
    $kok.find('[data-alan][data-html]').each(function () {
        editorler[$(this).data('alan')] = Sgb.editor(this, { yukseklik: parseInt($(this).data('yukseklik'), 10) || 220 });
    });

    function yaz(alanlar) {
        $kok.find('[data-alan]').each(function () {
            var ad = $(this).data('alan'), deger = (alanlar && alanlar[ad]) || '';
            if (editorler[ad]) editorler[ad].html(deger);
            else $(this).val(deger);
        });
    }

    function oku() {
        var alanlar = {};
        $kok.find('[data-alan]').each(function () {
            var ad = $(this).data('alan');
            alanlar[ad] = editorler[ad] ? editorler[ad].html() : $(this).val();
        });
        return alanlar;
    }

    function getir(oncekiYil) {
        var $d = $('#btnKaydet, #btnOncekiYil').prop('disabled', true);
        return Sgb.al('api/yatprojegiris/bilgi', $.extend({ oncekiYil: !!oncekiYil }, olcut)).done(function (s) {
            if (s.hata) { mesaj(s.hata, 'danger'); return; }
            yaz(s.alanlar);
            if (oncekiYil) mesaj((olcut.yil - 1) + ' yılının bilgileri getirildi; saklamak için Kaydet\'e basınız.', 'info');
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    }

    $('#btnKaydet').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/yatprojegiris/bilgi', $.extend({ alanlar: oku() }, olcut)).done(function (s) {
            if (s.hata) { mesaj(s.hata, 'danger'); return; }
            mesaj(s.bilgi || 'Kayıt işlemi başarılı.', 'success');
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    });

    $('#btnOncekiYil').on('click', function () { getir(true); });

    if (olcut.yil < 2000 || (olcut.projeKodu <= 0 && olcut.altProjeKodu <= 0)) {
        $('#btnKaydet, #btnOncekiYil').prop('disabled', true);
        mesaj('Herhangi bir Proje seçilmemiş!', 'warning');
        return;
    }
    getir(false);
});
