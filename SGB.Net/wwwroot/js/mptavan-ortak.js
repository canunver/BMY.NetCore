// Masraf Planı Tavan ekranlarının ortak yardımcıları (MPTavanAna, MPTavanGiris).
//   MPTavan.kodAlanlari(yilOku, hataGoster[, api]): sayfadaki .kod-alan kutularını kurar. api: kodlar / kodad uçlarının kökü
//   (varsayılan 'api/mptavan'; Masraf Planı 'api/masrafplani', Masraf Planı Dağıtım 'api/mpdagitim' kullanır). Kutuda data-agac kod ağacının türü (eski
//   KodAgac.aspx?kodTur=: 1, 2, 3, 4, 14, 17), data-ad adının türüdür (eski KodAdGetir: 1, 2, 3, 4, 58, 53). Kod yazılıp çıkılınca
//   adı yanına yazılır; mercekle kod ağacı (#dlgKod) açılır. Dönen nesne: { kod(alan), ata(alan, kod, ad), adlariYenile() }.
//   MPTavan.tutar(deger): Türkçe binlik ayraçlı tutar.
window.MPTavan = (function ($) {
    'use strict';

    var tutarBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 0, maximumFractionDigits: 2 });
    var yuzdeBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function tutar(v) { return v === null || v === undefined || v === '' || !isFinite(v) ? '' : tutarBicim.format(v); }
    function yuzde(v) { return v === null || v === undefined || !isFinite(v) ? '' : yuzdeBicim.format(v); }

    // "13.01.31.00- Ad" -> "13.01.31.00"
    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('- ');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }

    function kodAlanlari(yilOku, hataGoster, api) {
        api = api || 'api/mptavan';
        var dlgKod = document.getElementById('dlgKod');
        var agaclar = {};  // ağaç türü -> { $ul, agac, yil }
        var aktifAlan = null;

        function $alan(alan) { return $('.kod-alan[data-alan="' + alan + '"]'); }
        function kod(alan) { return kodAl($alan(alan).find('input').val()); }

        function adYaz($a, ad) { $a.find('.kod-ad').text(ad || '').attr('title', ad || null); }

        // Eski KodAdGetir (alanın Blur'u): yazılan kodun adı.
        function adGetir($a) {
            var k = kodAl($a.find('input').val());
            if (!k) { adYaz($a, ''); return; }
            var yil = yilOku();
            Sgb.al(api + '/kodad', { yil: yil, tur: $a.attr('data-ad'), kod: k })
                .done(function (v) { adYaz($a, v.hata ? '' : v.ad); })
                .fail(function () { adYaz($a, ''); });
        }

        function ata(alan, k, ad) {
            var $a = $alan(alan);
            $a.find('input').val(k || '');
            if (ad !== undefined) adYaz($a, ad);
            else adGetir($a);
        }

        function adlariYenile() { $('.kod-alan').each(function () { adGetir($(this)); }); }

        function agacAc($a) {
            var yil = yilOku();
            if (yil <= 0) { Sgb.bildirim('Yıl seçiniz.', 'warning'); return; }
            var tur = $a.attr('data-agac');
            var baslik = $a.find('label').first().text();
            aktifAlan = $a;
            $('#dlgKodBaslik').text(baslik + ' Kod Seçimi');
            $('#agacKutu > ul').prop('hidden', true);
            var a = agaclar[tur];
            dlgKod.showModal();
            if (!a) {
                a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', baslik + ' ağacı').appendTo('#agacKutu'), yil: yil };
                a.agac = Sgb.agac(a.$ul[0], {
                    yukle: function (ust) {
                        return Sgb.al(api + '/kodlar', { yil: a.yil, kod: tur, ust: ust ? ust.kod : '' }).then(function (v) {
                            if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return []; }
                            return v.dugumler;
                        });
                    },
                    secilince: function (d) {
                        dlgKod.close();
                        if (!aktifAlan) return;
                        // Düğüm adı "kod- Ad" biçimindedir.
                        var ad = d.ad.indexOf('- ') > 0 ? d.ad.substring(d.ad.indexOf('- ') + 2) : '';
                        aktifAlan.find('input').val(d.kod).trigger('mpkod');
                        adYaz(aktifAlan, ad);
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
        $('.kod-alan .kod-temizle').on('click', function () {
            var $a = $(this).closest('.kod-alan');
            $a.find('input').val('').trigger('focus');
            adYaz($a, '');
        });
        $('.kod-alan input')
            .on('keydown', function (e) {
                if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
            })
            .on('change', function () { adGetir($(this).closest('.kod-alan')); });

        $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

        return { kod: kod, ata: ata, adlariYenile: adlariYenile };
    }

    return { tutar: tutar, yuzde: yuzde, kodAl: kodAl, kodAlanlari: kodAlanlari };
})(jQuery);
