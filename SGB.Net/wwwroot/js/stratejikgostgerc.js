// İzleme Tablosu (PerformansButce/StratejikGostGerc): kurumun stratejik planındaki bir hedefin göstergelerinin
// altı aylık izleme (dönem 3) ya da yıl sonu değerlendirme (dönem 4) gerçekleşmeleri; izleme / değerlendirme raporları.
$(function () {
    'use strict';

    if (!$('#lstHedef').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var DEGERLENDIRME = 4;   // yıl sonu değerlendirme dönemi
    var hedef = null;        // seçili hedef: { kod, kisa, ad }
    var acik = null;         // tabloda açık olan gerçekleşmelerin ölçütü: { kurumKod, donemBYil, hedef, yil, donem }
    var hedefListe = null;
    var sayiBicimi = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function sayi(deger) {
        var s = parseFloat(String(deger === null || deger === undefined ? '' : deger).replace(',', '.'));
        return isNaN(s) ? 0 : s;
    }

    // Arama bölmesindeki seçim
    function olcut() {
        return {
            kurum: $('#ddlKurum').val() || '', donemBYil: parseInt($('#ddlPlan').val(), 10) || 0,
            yil: parseInt($('#txtYil').val(), 10) || 0, donem: parseInt($('#ddlDonem').val(), 10) || 0
        };
    }

    // ---------------------------------------------------------------- gerçekleşmeler

    // Performans yüzdesi sunucudaki kuralla aynıdır (StrHedefGosterge.Oranla).
    function oranla(g, gerceklesme) {
        var bolen = g.oranABCB ? g.hedeflenenBirikimliDeger - g.donemBaslangic : g.hedeflenenBirikimliDeger;
        var bolunen = g.oranABCB ? gerceklesme - g.donemBaslangic : gerceklesme;
        return bolen !== 0 ? bolunen * 100 / bolen : 0;
    }

    function metinKutusu(deger, alan, etiket) {
        return $('<td></td>').append($('<textarea class="form-control" rows="2"></textarea>').attr('data-alan', alan).attr('aria-label', etiket).val(deger || ''));
    }

    function gerceklesmeleriCiz(veri) {
        var degerlendirme = acik.donem === DEGERLENDIRME;
        // Yıl sonu değerlendirmesinde dört ölçüt (ilgililik, etkililik, etkinlik, sürdürülebilirlik) ayrı sütunlardır;
        // tablo ekrana sığmadığında sağa sola kaydırılır (ilk sütun yerinde kalır).
        var basliklar = ['Gösterge', 'Dönem Başlangıcı', 'Önc. Yıl Gerçekleşme', 'Hedeflenen Değer', degerlendirme ? 'Gerçekleşme (Yıl Sonu)' : 'Gerçekleşme (Altı Ay)', 'Performans (%)']
            .concat(degerlendirme ? ['İlgililik', 'Etkililik', 'Etkinlik', 'Sürdürülebilirlik'] : ['Açıklama']);
        var $bas = $('<tr></tr>').appendTo($('#tblGerc thead').empty());
        $.each(basliklar, function (i, b) { $('<th scope="col"></th>').text(b).toggleClass('sayi', i >= 1 && i <= 5).appendTo($bas); });

        var $govde = $('#tblGerc tbody').empty();
        $.each(veri.satirlar, function (i, g) {
            var $tr = $('<tr></tr>').data('gosterge', g).appendTo($govde);
            $('<td class="hesap gosterge-ad"></td>').text(g.gostergeKodAd).appendTo($tr);
            $('<td class="hesap sayi"></td>').text(sayiBicimi.format(g.donemBaslangic)).appendTo($tr);
            $('<td class="hesap sayi"></td>').text(sayiBicimi.format(g.oncYilGerceklesme)).appendTo($tr);
            $('<td class="hesap sayi"></td>').text(sayiBicimi.format(g.hedeflenenBirikimliDeger)).appendTo($tr);
            $('<td></td>').append($('<input type="number" class="form-control" step="any" data-alan="birikimliGerceklesme" />')
                .attr('aria-label', 'Gerçekleşme: ' + g.gostergeKodAd).val(g.birikimliGerceklesme)).appendTo($tr);
            $('<td class="hesap sayi performans"></td>').text(sayiBicimi.format(g.birikimliPerformans)).appendTo($tr);
            if (degerlendirme) {
                metinKutusu(g.ilgililik, 'ilgililik', 'İlgililik: ' + g.gostergeKodAd).appendTo($tr);
                metinKutusu(g.etkililik, 'etkililik', 'Etkililik: ' + g.gostergeKodAd).appendTo($tr);
                metinKutusu(g.etkinlik, 'etkinlik', 'Etkinlik: ' + g.gostergeKodAd).appendTo($tr);
                metinKutusu(g.surdurulebilirlik, 'surdurulebilirlik', 'Sürdürülebilirlik: ' + g.gostergeKodAd).appendTo($tr);
            } else
                metinKutusu(g.aciklama, 'aciklama', 'Açıklama').appendTo($tr);
        });

        $('#txtDegerlendirmeler').val(veri.degerlendirmeler || '');
        $('#txtSapmaNedeni').val(veri.sapmaNedeni || '');
        $('#txtAlinacakOnlem').val(veri.alinacakOnlem || '');
        $('#gercBilgi [data-degerlendirme]').prop('hidden', !degerlendirme);

        var bos = !veri.satirlar.length;
        $('#gercBos').text('Bu hedefin göstergesi yok.').prop('hidden', !bos);
        $('#tblGerc').prop('hidden', bos);
        $('#gercBilgi').prop('hidden', false);
        $('#btnKaydet').prop('disabled', bos);
    }

    // Gerçekleşme değişince performans yüzdesi yeniden hesaplanır.
    $('#tblGerc').on('input', '[data-alan="birikimliGerceklesme"]', function () {
        var $tr = $(this).closest('tr');
        $tr.find('.performans').text(sayiBicimi.format(oranla($tr.data('gosterge'), sayi(this.value))));
    });

    function tabloyuKapat(metin) {
        acik = null;
        $('#tblGerc, #gercBilgi').prop('hidden', true);
        $('#gercBos').text(metin).prop('hidden', false);
        $('#btnKaydet').prop('disabled', true);
    }

    function gerceklesmeleriYukle() {
        if (!hedef) return;
        var o = olcut();
        if (!o.yil || !o.donem) { mesaj('Gerçekleşme yılını ve dönemini seçiniz.', 'warning'); return; }
        var istek = { kurum: o.kurum, donemBYil: o.donemBYil, hedef: hedef.kod, yil: o.yil, donem: o.donem };
        Sgb.al('api/strizleme/gerceklesme', istek)
            .done(function (veri) {
                if (!hedef || hedef.kod !== istek.hedef) return; // bu arada başka hedef seçildi
                if (veri.hata) { mesaj(veri.hata, 'danger'); tabloyuKapat('Gerçekleşmeler okunamadı.'); return; }
                acik = { kurumKod: istek.kurum, donemBYil: istek.donemBYil, hedef: istek.hedef, yil: istek.yil, donem: istek.donem };
                gerceklesmeleriCiz(veri);
            })
            .fail(hataGoster);
    }

    function hedefSec(h) {
        hedef = h;
        $('#hedefBaslik').text('Gerçekleşmeler: ' + (h.kisa ? h.kisa + ' ' : '') + h.ad);
        $('[data-hedefli]').prop('disabled', false);
        gerceklesmeleriYukle();
    }

    function hedefiBirak() {
        hedef = null;
        $('#hedefBaslik').text('Gerçekleşmeler');
        $('[data-hedefli]').prop('disabled', true);
        tabloyuKapat('Soldan bir hedef seçiniz.');
    }

    // Yıl ya da dönem değişince açık hedefin gerçekleşmeleri yeni ölçütle okunur.
    $('#txtYil, #ddlDonem').on('change', gerceklesmeleriYukle);

    $('#btnKaydet').on('click', function () {
        if (!acik) return;
        if (!window.confirm('Kaydetmek istiyor musunuz?')) return;
        var istek = $.extend({}, acik, {
            satirlar: $('#tblGerc tbody tr').map(function () {
                var $tr = $(this), s = { gostergeKod: $tr.data('gosterge').gostergeKod };
                $tr.find('[data-alan]').each(function () {
                    var alan = $(this).attr('data-alan');
                    s[alan] = alan === 'birikimliGerceklesme' ? sayi(this.value) : $.trim(this.value);
                });
                return s;
            }).get(),
            degerlendirmeler: $.trim($('#txtDegerlendirmeler').val()),
            sapmaNedeni: $.trim($('#txtSapmaNedeni').val()),
            alinacakOnlem: $.trim($('#txtAlinacakOnlem').val())
        });
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/strizleme/kaydet', istek)
            .done(function (s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- raporlar

    function raporIndir($dugme, adres, parametreler) {
        $dugme.prop('disabled', true);
        Sgb.indir(adres, parametreler).then(function () { $dugme.prop('disabled', false); });
    }

    // Hedef izleme / değerlendirme raporları (Word). Yazdır tuşları seçili hedefi, Rapor tuşları bütün hedefleri verir.
    $('.izleme').on('click', '[data-rapor]', function () {
        var o = olcut(), $d = $(this), hedefli = $d.is('[data-hedefli]');
        if (!o.kurum || !o.donemBYil) { mesaj('Kurum ve plan dönemi seçiniz.', 'warning'); return; }
        if (hedefli && !hedef) { mesaj('Önce hedef seçiniz.', 'warning'); return; }
        raporIndir($d, 'api/strizleme/rapor', { tur: $d.attr('data-rapor'), kurum: o.kurum, donemBYil: o.donemBYil, hedef: hedef ? hedef.kod : 0, yil: o.yil, donem: o.donem });
    });

    // Kurumun hedef / gösterge başarımları (Excel).
    $('.izleme').on('click', '[data-kurum-rapor]', function () {
        var o = olcut();
        if (!o.kurum) { mesaj('Kurum seçiniz.', 'warning'); return; }
        raporIndir($(this), 'api/strizleme/rapor-' + $(this).attr('data-kurum-rapor'), { kurum: o.kurum, yil: o.yil, donem: o.donem });
    });

    // Grafikler (eski GrafikGoster penceresi): param eski sayfadakiyle aynıdır.
    $('.izleme').on('click', '[data-grafik]', function () {
        var o = olcut();
        if (!o.kurum || !o.yil) { mesaj('Kurum ve yıl seçiniz.', 'warning'); return; }
        Sgb.pencere('../OrtakSayfa/GrafikGoster?param=' + encodeURIComponent('1-' + o.yil + '-' + o.kurum + '-t-s-d-' + $(this).attr('data-grafik')), $.trim($(this).text()));
    });

    // ---------------------------------------------------------------- arama bölmesi

    function hedefleriListele(uyar) {
        hedefiBirak();
        hedefListe.yenile().done(function () {
            if (uyar && !$('#lstHedef .sec').length) mesaj('İlgili döneme ait hedef bulunamamıştır!', 'info');
        });
    }

    function planlariYukle(secili) {
        var $ddl = $('#ddlPlan').empty();
        var kurum = $('#ddlKurum').val();
        if (!kurum) return $.Deferred().resolve().promise();
        // Kurumun kayıtlı planları Stratejik Plan sayfasıyla aynı uçtan okunur.
        return Sgb.al('api/strplan/donemler', { kurum: kurum })
            .done(function (veri) {
                $.each(veri.satirlar, function (i, d) { $('<option></option>').val(d.yil).text(d.ad).appendTo($ddl); });
                if (secili) $ddl.val(String(secili));
                if (!$ddl.val() && veri.satirlar.length) $ddl.val(String(veri.satirlar[0].yil));
            })
            .fail(hataGoster);
    }

    $('#ddlKurum').on('change', function () { planlariYukle(0).done(function () { hedefleriListele(false); }); });
    $('#ddlPlan').on('change', function () { hedefleriListele(false); });
    $('#btnListele').on('click', function () { hedefleriListele(true); });
    $('#frmArama').on('submit', function (olay) { olay.preventDefault(); hedefleriListele(true); });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/strizleme/tanim')
        .done(function (t) {
            $.each(t.kurumlar, function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo('#ddlKurum'); });
            if (t.kurumKod) $('#ddlKurum').val(t.kurumKod);
            $.each(t.donemler, function (i, d) { $('<option></option>').val(d.kod).text(d.ad).appendTo('#ddlDonem'); });
            $('#ddlDonem').val(String(t.donem));
            $('#txtYil').val(t.sorguYil);
            $('#btnRaporV1').prop('hidden', !!t.raporV1Gizli);

            hedefListe = Sgb.agac('#lstHedef', {
                duz: true,
                yukle: function () {
                    var o = olcut();
                    if (!o.kurum || !o.donemBYil) return $.Deferred().resolve([]).promise();
                    return Sgb.al('api/strizleme/hedefler', { kurum: o.kurum, donemBYil: o.donemBYil }).then(function (veri) {
                        if (veri.hata) { mesaj(veri.hata, 'danger'); return []; }
                        return $.map(veri.satirlar, function (h) { return { kod: h.kod, kisa: h.kisa, ad: (h.kisa ? h.kisa + ' - ' : '') + h.ad, hedefAd: h.ad }; });
                    });
                },
                secilince: function (d) { hedefSec({ kod: d.kod, kisa: d.kisa, ad: d.hedefAd }); },
                bosMetin: 'Hedef yok.',
                hata: hataGoster
            });
            planlariYukle(t.donemBYil).done(function () { hedefleriListele(false); });
        })
        .fail(hataGoster);
});
