// Kurum Değerlendirme (PerformansButce/SPIzlemeKurum): kurumun stratejik planındaki hedeflerin plan yıllarına göre
// başarım yüzdeleri; satır açılınca hedefin göstergelerinin başarımları; hedef açıklaması; iki Excel raporu.
$(function () {
    'use strict';

    if (!$('#tblHedef').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var acik = null;          // listelenmiş ölçüt: { kurum, yil, donem } (göstergeler ve açıklama bununla okunur / yazılır)
    var yilSay = 0;           // tabloda gösterilen plan yılı sayısı
    var aciklamaSatir = null; // açıklaması düzenlenen hedefin satırı
    var sayiBicimi = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // Arama bölmesindeki seçim. Boş bırakılan başarım sınırı gönderilmez (sınır yok).
    function olcut() {
        var o = { kurum: $('#ddlKurum').val() || '', yil: parseInt($('#txtYil').val(), 10) || 0, donem: parseInt($('#ddlDonem').val(), 10) || 0 };
        var alt = parseInt($('#txtAlt').val(), 10), ust = parseInt($('#txtUst').val(), 10);
        if (!isNaN(alt) && alt >= 0) o.alt = alt;
        if (!isNaN(ust) && ust >= 0) o.ust = ust;
        return o;
    }

    function olcutGecerli(o) {
        if (o.kurum && o.yil && o.donem) return true;
        mesaj('Kurum, yıl ve dönem seçiniz.', 'warning');
        return false;
    }

    // ---------------------------------------------------------------- hedefler

    function hedefSatiri(h) {
        var $tr = $('<tr class="hedef"></tr>').addClass('not' + h.basariNotu).data('hedef', h);
        $('<td class="ac"></td>').append($('<button type="button" class="btn btn-link btn-xs" data-ac aria-expanded="false"></button>')
            .attr('aria-label', 'Göstergeler: ' + h.hedefKisa).append('<span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>')).appendTo($tr);
        $('<td class="kisa"></td>').text(h.hedefKisa).appendTo($tr);
        $('<td class="ad"></td>').text(h.hedefAd).appendTo($tr);
        $('<td class="sayi"></td>').text(h.gostergeSay).appendTo($tr);
        for (var i = 0; i < yilSay; i++) $('<td class="sayi"></td>').text(sayiBicimi.format(h.oranlar[i])).appendTo($tr);
        $('<td class="aciklama"></td>')
            .append($('<button type="button" class="btn btn-default btn-xs" data-aciklama title="Açıklamayı düzenle"></button>')
                .attr('aria-label', 'Açıklamayı düzenle: ' + h.hedefKisa).append('<span class="glyphicon glyphicon-pencil" aria-hidden="true"></span>'))
            .append($('<span class="metin"></span>').text(h.aciklama))
            .appendTo($tr);
        return $tr;
    }

    function listeyiCiz(veri) {
        yilSay = veri.yilSay;
        var $bas = $('<tr></tr>').appendTo($('#tblHedef > thead').empty());
        $('<th scope="col"><span class="sr-only">Göstergeler</span></th>').appendTo($bas);
        $('<th scope="col">Hedef</th><th scope="col">Hedef Adı</th><th scope="col" class="sayi">Gösterge</th>').appendTo($bas);
        for (var i = 0; i < yilSay; i++) $('<th scope="col" class="sayi"></th>').text((veri.basYil + i) + ' Başarım (%)').appendTo($bas);
        $('<th scope="col">Açıklama</th>').appendTo($bas);

        var $govde = $('#tblHedef > tbody').empty();
        $.each(veri.satirlar, function (i, h) { hedefSatiri(h).appendTo($govde); });

        var bos = !veri.satirlar.length;
        $('#listeBos').text('Ölçütlere uyan hedef bulunamadı.').prop('hidden', !bos);
        $('#tblHedef').prop('hidden', bos);
        $('#lstOzet').prop('hidden', bos).find('b').each(function (i) { $(this).text(veri.basariSay[i]); });
        $('#listeBaslik').text('Gerçekleşmeler: ' + acik.yil + ' ' + $('#ddlDonem option:selected').text());
    }

    function listele() {
        var o = olcut();
        if (!olcutGecerli(o)) return;
        var $d = $('#btnListele').prop('disabled', true);
        Sgb.al('api/strkurum/liste', o)
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                acik = { kurum: o.kurum, yil: o.yil, donem: o.donem };
                listeyiCiz(veri);
                grafikleriBosalt();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    }

    $('#frmArama').on('submit', function (olay) { olay.preventDefault(); listele(); });

    // ---------------------------------------------------------------- sekmeler ve grafikler

    // Grafik sekmeleri listelenmiş kurum ve yıl için, sekme ilk açıldığında okunur (eski GrafikGoster ile aynı param).
    function grafikYukle($sekme) {
        var $p = $('#' + $sekme.attr('aria-controls'));
        if ($p.data('okundu')) return;
        if (!acik) { $('<p class="tanim-not duz"></p>').text('Önce listeleyiniz.').appendTo($p.empty()); return; }
        $p.data('okundu', true);
        Sgb.al('api/grafik', { param: '1-' + acik.yil + '-' + acik.kurum + '-t-s-d-' + $sekme.attr('data-grafik') })
            .done(function (veri) {
                if (veri.hata) $('<p class="tanim-not duz"></p>').text(veri.hata).appendTo($p.empty());
                else Sgb.grafik($p, veri);
            })
            .fail(function (xhr) { $p.removeData('okundu'); hataGoster(xhr); });
    }

    // Yeni listelemede eski grafikler atılır; açık olan grafik sekmesi yeniden okunur.
    function grafikleriBosalt() {
        $('.grafik-sekme').empty().removeData('okundu');
        var $secili = $('.kurum-izleme [role="tab"][aria-selected="true"]');
        if ($secili.is('[data-grafik]')) grafikYukle($secili);
    }

    $('.kurum-izleme .sekmeler').on('click', '[role="tab"]', function () {
        var $sekme = $(this);
        $sekme.siblings().addBack().each(function () {
            var secili = this === $sekme[0];
            $(this).attr('aria-selected', String(secili));
            $('#' + $(this).attr('aria-controls')).prop('hidden', !secili);
        });
        if ($sekme.is('[data-grafik]')) grafikYukle($sekme);
    });

    // ---------------------------------------------------------------- göstergeler (satır açma)

    function gostergeTablosu(veri) {
        if (!veri.satirlar.length) return $('<p class="tanim-not duz">Bu hedefin göstergesi yok.</p>');
        var $t = $('<table class="table table-condensed gosterge"><thead><tr></tr></thead><tbody></tbody></table>');
        var $bas = $t.find('thead tr');
        $('<th scope="col">Gösterge</th><th scope="col">Gösterge Adı</th>').appendTo($bas);
        $('<th scope="col" class="sayi"></th>').text((veri.basYil - 1) + ' (Başlangıç)').appendTo($bas);
        for (var i = 0; i < yilSay; i++) $('<th scope="col" class="sayi"></th>').text((veri.basYil + i) + ' Başarım (%)').appendTo($bas);
        $.each(veri.satirlar, function (j, g) {
            var $tr = $('<tr></tr>').addClass('not' + g.basariNotu).appendTo($t.find('tbody'));
            $('<td class="kisa"></td>').text(g.gorKod).appendTo($tr);
            $('<td></td>').text(g.ad).appendTo($tr);
            $('<td class="sayi"></td>').text(sayiBicimi.format(g.baslangic)).appendTo($tr);
            for (var i = 0; i < yilSay; i++) $('<td class="sayi"></td>').text(sayiBicimi.format(g.oranlar[i])).appendTo($tr);
        });
        return $t;
    }

    $('#tblHedef').on('click', '[data-ac]', function () {
        var $d = $(this), $tr = $d.closest('tr'), $alt = $tr.next('.alt');
        var ac = $d.attr('aria-expanded') !== 'true';
        $d.attr('aria-expanded', String(ac)).find('.glyphicon').toggleClass('glyphicon-chevron-right', !ac).toggleClass('glyphicon-chevron-down', ac);
        if ($alt.length) { $alt.prop('hidden', !ac); return; }
        if (!ac || !acik) return;

        var $hucre = $('<td></td>').attr('colspan', $tr.children().length).text('Okunuyor...');
        $alt = $('<tr class="alt"></tr>').append($hucre).insertAfter($tr);
        Sgb.al('api/strkurum/gostergeler', { kurum: acik.kurum, yil: acik.yil, donem: acik.donem, hedef: $tr.data('hedef').hedefKod })
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); $alt.remove(); return; }
                $hucre.empty().append(gostergeTablosu(veri));
            })
            .fail(function (xhr) { $alt.remove(); hataGoster(xhr); });
    });

    // ---------------------------------------------------------------- hedef açıklaması

    var dlgAciklama = document.getElementById('dlgAciklama');
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    $('#tblHedef').on('click', '[data-aciklama]', function () {
        aciklamaSatir = $(this).closest('tr');
        var h = aciklamaSatir.data('hedef');
        $('#aciklamaHedef').text(h.hedefKisa + ' ' + h.hedefAd);
        $('#txtAciklama').val(h.aciklama);
        dlgAciklama.showModal();
    });

    $('#btnAciklamaKaydet').on('click', function () {
        if (!aciklamaSatir || !acik) return;
        var h = aciklamaSatir.data('hedef'), $satir = aciklamaSatir;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/strkurum/aciklama', { kurum: acik.kurum, yil: acik.yil, hedef: h.hedefKod, metin: $('#txtAciklama').val() })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (!s.basarili) return;
                h.aciklama = s.aciklama;
                $satir.find('.aciklama .metin').text(s.aciklama);
                dlgAciklama.close();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- raporlar (Excel)

    $('.kurum-izleme').on('click', '[data-rapor]', function () {
        var o = olcut();
        if (!olcutGecerli(o)) return;
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/strkurum/rapor-' + $d.attr('data-rapor'), o).then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/strkurum/tanim')
        .done(function (t) {
            $.each(t.kurumlar, function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo('#ddlKurum'); });
            $.each(t.donemler, function (i, d) { $('<option></option>').val(d.kod).text(d.ad).appendTo('#ddlDonem'); });

            // Başka sayfadan yıl, kurum ve dönemle çağrılmışsa (eski sayfadaki gibi) liste hemen gelir.
            var sorgu = new URLSearchParams(window.location.search);
            var yil = parseInt(sorgu.get('yil'), 10) || 0, kurum = sorgu.get('kur') || '', donem = parseInt(sorgu.get('donem'), 10) || 0;
            $('#ddlKurum').val(kurum || t.kurumKod);
            if (!$('#ddlKurum').val() && t.kurumlar.length) $('#ddlKurum').val(t.kurumlar[0].kod);
            $('#txtYil').val(yil || t.yil);
            $('#ddlDonem').val(String(donem || t.donem));
            if (!$('#ddlDonem').val()) $('#ddlDonem').val(String(t.donem));
            if (yil && kurum && donem) listele();
        })
        .fail(hataGoster);
});
