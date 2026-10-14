// Bütçe teklifi tertip detayı (eski ButceHazirlikv2/ButceTeklifHazirlaDetay.aspx). Bütçe teklifi ekranında satırdaki detay
// simgesiyle pencere (çerçeve) içinde açılır: kesin hesap / taahhüt / taşınır bilgileri gösterilir, 3 yıllık teklif girilip saklanır.
// Önceki / Sonraki (eski pencerenin araç çubuğu) yalnızca açan sayfa listede gezinmeyi sağlıyorsa görünür.
(function ($) {
    'use strict';

    var $sayfa = $('.tertip-detay');
    if (!$sayfa.length) return;

    var YIL = parseInt($sayfa.attr('data-yil'), 10) || 0;
    var TAHMIN = parseInt($sayfa.attr('data-tahmin'), 10) || 0;
    var SENARYO = $sayfa.attr('data-senaryo') || '';
    var TERTIP = $sayfa.attr('data-tertip') || '';
    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 2 });

    // "1.234.000" ya da "1234000,50" -> sayı. Boş -> null, geçersiz -> NaN.
    function sayiCoz(metin) {
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return null;
        var n = Number(s);
        return isFinite(n) ? n : NaN;
    }

    function sayiYaz(n) { return n == null || isNaN(n) ? '' : tamSayi.format(n); }

    // ---------------------------------------------------------------- sekmeler
    $sayfa.on('click', '[role="tab"]', function () {
        var $s = $(this);
        $sayfa.find('[role="tab"]').each(function () {
            var secili = this === $s[0];
            $(this).attr('aria-selected', String(secili));
            $('#' + $(this).attr('aria-controls')).prop('hidden', !secili);
        });
    });

    // ---------------------------------------------------------------- bilgiler
    function tabloDoldur(v) {
        $sayfa.find('[data-alan]').each(function () { $(this).text(v[$(this).attr('data-alan')] || ''); });
        $sayfa.find('tbody[data-satirlar]').each(function () {
            var $tb = $(this).empty();
            var alanlar = $tb.attr('data-alanlar').split(',');
            $.each(v[$tb.attr('data-satirlar')] || [], function (i, s) {
                var $tr = $('<tr></tr>').appendTo($tb);
                $('<th scope="row" class="yil"></th>').text(s.yil).appendTo($tr);
                $.each(alanlar, function (j, a) { $('<td class="sayi"></td>').text(s[a] == null ? '' : s[a]).appendTo($tr); });
            });
        });
        // Taşınır sekmesi ekonomik kodla ilişkili taşınır hesabı varsa görünür (eski TNSTab1.TABParcalar[2].Gizli).
        var $tasinir = $('#sekmeTasinir').prop('hidden', !v.tasinirVar);
        if (!v.tasinirVar && $tasinir.attr('aria-selected') === 'true') $('#sekmeKesin').trigger('click');
        $.each(v.teklif || [], function (i, t) {
            $('#lblTeklifYil' + (i + 1)).text(t.yil);
            $('#txtYilDeger' + (i + 1)).val(sayiYaz(t.deger));
        });
    }

    function yukle() {
        $('#lblTertip').text(TERTIP);
        for (var i = 1; i <= 3; i++) $('#lblTeklifYil' + i).text(YIL + i - 1);
        if (!YIL || !SENARYO || !TERTIP) {
            Sgb.bildirim('Tertip, yıl veya aşama kodu boş olamaz!', 'warning');
            return;
        }
        Sgb.al('api/tertipdetay/bilgi', {
            yil: YIL, senaryo: SENARYO, tertip: TERTIP, tahmin: TAHMIN,
            muhasebekod: $sayfa.attr('data-muhasebekod') || '', birimkod: $sayfa.attr('data-birimkod') || ''
        }).done(function (v) {
            if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
            tabloDoldur(v);
        }).fail(function (xhr) {
            if (xhr.status === 403) Sgb.bildirim('Bu işlem için yetkiniz yok.', 'danger');
            else if (xhr.status !== 401) Sgb.bildirim('Bilgiler okunamadı.', 'danger');
        });
    }

    $('#frmTeklif').on('blur', 'input.sayi', function () {
        var n = sayiCoz(this.value);
        if (n != null && !isNaN(n)) this.value = sayiYaz(n);
    }).on('submit', function (e) { e.preventDefault(); kaydet(); });

    // ---------------------------------------------------------------- Kaydet (eski btnKaydet_Click)
    function kaydet() {
        var degerler = [], hatalar = [];
        for (var i = 1; i <= 3; i++) {
            var n = sayiCoz($('#txtYilDeger' + i).val());
            if (n == null) hatalar.push(i + '. Yıl boş bırakılamaz');
            else if (isNaN(n)) hatalar.push(i + '. Yıl sayı olmalı');
            degerler.push(n == null || isNaN(n) ? null : n);
        }
        if (!YIL || !SENARYO || !TERTIP) hatalar.push('Tertip, yıl veya aşama kodu boş olamaz!');
        if (hatalar.length) { Sgb.bildirim(hatalar.join(' '), 'warning'); return; }

        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/tertipdetay/kaydet', { yil: YIL, senaryo: SENARYO, tertip: TERTIP, tahmin: TAHMIN, degerler: degerler })
            .done(function (s) {
                if (s.hata) { Sgb.bildirim(s.hata, 'danger'); return; }
                Sgb.bildirim(s.bilgi || 'Bilgiler başarıyla saklandı.', 'success');
            })
            .fail(function (xhr) {
                if (xhr.status === 403) Sgb.bildirim('Bu işlem için yetkiniz yok.', 'danger');
                else if (xhr.status !== 401) Sgb.bildirim('Bilgiler saklanamadı.', 'danger');
            })
            .always(function () { $d.prop('disabled', false); });
    }
    $('#btnKaydet').on('click', kaydet);

    // ---------------------------------------------------------------- Önceki / Sonraki (eski FPIlerle)
    var ust = null;
    try {
        if (window.parent !== window && window.frameElement && typeof window.parent.butceRakamDetayIlerle === 'function') ust = window.parent;
    } catch (e) { ust = null; }
    if (ust) {
        $sayfa.find('.gezinme').prop('hidden', false);
        $('#btnOnceki').on('click', function () { ust.butceRakamDetayIlerle(window.frameElement, TERTIP, -1); });
        $('#btnSonraki').on('click', function () { ust.butceRakamDetayIlerle(window.frameElement, TERTIP, 1); });
    }

    yukle();
})(jQuery);
