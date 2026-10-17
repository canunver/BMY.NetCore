// Sgb.muhasebeBirim: muhasebe - birim - kurumsal kod seçim bileşeni (etiketsiz). Üç alan tek alan gibi bitişik, hemen yanında
// mercek tuşu, onun yanında seçilen birimin adı. Muhasebe ya da birim elle yazılınca adı ve kurumsal kodu okunur (eski
// txtBirim_Changed); mercek kullanıcının yetkili olduğu saymanlık / birimleri Sgb.agac penceresinde açar (eski ListeMuhasebeBirim.aspx).
// Uçlar: /api/muhbirim/bilgi, /api/muhbirim/agac (OrtakSayfa/MuhasebeBirimUclari.cs). Görünüm: css/muhbirim.css.
//
// Kullanım:
//   var mb = Sgb.muhasebeBirim('#muhBirim', {
//       kimlik:    { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },  // isteğe bağlı
//       yil:       function () { return 2026; },        // kurumsal kod yapısının yılı (isteğe bağlı; yoksa bu yıl)
//       oncesi:    function () { return true; },        // değişiklikten önce sorulur; false dönerse eski değer geri yazılır
//       degisince: function (deger) { },                // birim değişip adı okunduktan sonra
//       kurumsalGizle: true,                            // kurumsal kod alanı gösterilmez (değeri yine okunur)
//       adGizle: true                                   // birim adı yazılmaz; ad, alanların üzerine gelince ipucu olarak görünür
//   });
//   mb.deger()  -> { muhasebe: 1, birim: '000', kurumsal: '13.99.01.01', birimAd: '...' }
//   mb.ata({ muhasebe, birim, kurumsal, birimAd })     // olay çıkarmadan yazar (ör. açılışta kullanıcının birimi)
(function (Sgb, $) {
    'use strict';

    var sayac = 0;

    function muhasebeBirim(element, ozellikler) {
        var o = ozellikler || {};
        var k = o.kimlik || {};
        var no = ++sayac;
        var $kok = $(element).addClass('muh-birim').empty();

        function girdi(sinif, kimlik, etiket, ek) {
            return $('<input type="text" class="form-control input-sm" autocomplete="off" />').addClass(sinif)
                .attr($.extend({ id: kimlik || null, title: etiket, 'aria-label': etiket }, ek || {}));
        }

        var $muh = girdi('mb-muhasebe', k.muhasebe, 'Muhasebe kodu', { placeholder: 'Muh.' });
        var $brm = girdi('mb-birim', k.birim, 'Birim kodu', { placeholder: 'Brm', maxlength: 3 });
        var $kur = girdi('mb-kurumsal', k.kurumsal, 'Kurumsal kod', { placeholder: 'Kur', readonly: true, tabindex: -1 });
        var $sec = $('<button type="button" class="btn btn-default btn-sm mb-sec" title="Muhasebe birimi seç">'
            + '<span class="glyphicon glyphicon-search" aria-hidden="true"></span><span class="sr-only">Muhasebe birimi seç</span></button>');
        var $ad = $('<span class="mb-ad"></span>').attr('id', k.ad || null);
        $('<div class="mb-bitisik"></div>')
            .append($muh, '<span class="mb-ayrac" aria-hidden="true">-</span>', $brm, '<span class="mb-ayrac" aria-hidden="true">-</span>', $kur, $sec)
            .appendTo($kok);
        $kok.append($ad);
        if (o.kurumsalGizle) { $kur.prop('hidden', true).prev('.mb-ayrac').prop('hidden', true); }
        if (o.adGizle) $ad.prop('hidden', true);

        var kayitli = { muhasebe: '', birim: '' }; // son kabul edilen değer: oncesi() false dönerse geri yazılır

        function yil() { return o.yil ? (parseInt(o.yil(), 10) || 0) : 0; }
        function oncesi() { return !o.oncesi || o.oncesi() !== false; }

        function deger() {
            return {
                muhasebe: parseInt($.trim($muh.val()), 10) || 0,
                birim: $.trim($brm.val()),
                kurumsal: $kur.val(),
                birimAd: $ad.text()
            };
        }

        function adYaz(ad) {
            $ad.text(ad || '').attr('title', ad || '');
            if (o.adGizle) $muh.add($brm).attr('title', ad || null);
        }

        function ata(v) {
            v = v || {};
            $muh.val(v.muhasebe ? String(v.muhasebe) : '');
            $brm.val(v.birim || '');
            $kur.val(v.kurumsal || '');
            adYaz(v.birimAd);
            kayitli = { muhasebe: $muh.val(), birim: $brm.val() };
        }

        function bildir() { if (o.degisince) o.degisince(deger()); }

        function bilgiOku() {
            var d = deger();
            if (d.muhasebe <= 0) { $kur.val(''); adYaz(''); bildir(); return; }
            Sgb.al('api/muhbirim/bilgi', { muhasebe: d.muhasebe, birim: d.birim, yil: yil() })
                .done(function (v) {
                    if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
                    $kur.val(v.kurumsal || '');
                    adYaz(v.birimAd);
                    bildir();
                })
                .fail(function (xhr) { if (!xhr || xhr.status !== 401) Sgb.bildirim('Birim bilgisi okunamadı.', 'danger'); });
        }

        $muh.add($brm).on('change', function () {
            if (!oncesi()) { $muh.val(kayitli.muhasebe); $brm.val(kayitli.birim); return; }
            kayitli = { muhasebe: $muh.val(), birim: $brm.val() };
            bilgiOku();
        });

        // ------------------------------------------------------------ seçim penceresi

        var $pencere = $('<dialog class="tanim-pencere muh-birim-pencere"></dialog>').attr('aria-labelledby', 'mbBaslik' + no)
            .append('<div class="pencere-baslik"><h2 id="mbBaslik' + no + '">Muhasebe Birimi Seçimi</h2>'
                + '<button type="button" class="close" data-kapat aria-label="Kapat"><span aria-hidden="true">&times;</span></button></div>')
            .append($('<div class="pencere-govde"></div>').append('<ul aria-label="Saymanlıklar ve birimleri"></ul>'))
            .append('<div class="pencere-alt"><button type="button" class="btn btn-default btn-sm" data-kapat>Kapat</button></div>')
            .appendTo(document.body);
        var pencere = $pencere[0];
        $pencere.on('click', '[data-kapat]', function (e) { e.stopPropagation(); pencere.close(); });

        var agac = null;
        var agacYil = 0;

        $sec.on('click', function () {
            pencere.showModal();
            if (agac) {
                if (agacYil !== yil()) { agacYil = yil(); agac.yenile(); }
                return;
            }
            agacYil = yil();
            agac = Sgb.agac($pencere.find('ul')[0], {
                yukle: function (ust) {
                    return Sgb.al('api/muhbirim/agac', { ust: ust ? ust.kod : '', yil: agacYil }).then(function (v) {
                        if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                // Birim seçilince saymanlık, birim, kurumsal kod ve ad yazılır; saymanlık seçilince yalnızca saymanlık.
                secilince: function (d) {
                    var simdi = deger();
                    if (d.birim && d.muhasebe === simdi.muhasebe && d.birim === simdi.birim) { pencere.close(); return; }
                    if (!oncesi()) return;
                    pencere.close();
                    if (d.birim) {
                        ata({ muhasebe: d.muhasebe, birim: d.birim, kurumsal: d.kurumsal, birimAd: d.birimAd });
                        bildir();
                    } else {
                        $muh.val(d.kod);
                        $brm.val('');
                        kayitli = { muhasebe: $muh.val(), birim: '' };
                        bilgiOku();
                    }
                },
                seciliMi: function (d) {
                    var simdi = deger();
                    return !!d.birim && d.muhasebe === simdi.muhasebe && d.birim === simdi.birim;
                },
                ilkAcik: true,
                bosMetin: 'Yetkili olduğunuz muhasebe birimi bulunamadı.',
                hata: function (xhr) { if (!xhr || xhr.status !== 401) Sgb.bildirim('Muhasebe birimleri okunamadı.', 'danger'); }
            });
        });

        return { deger: deger, ata: ata, yenile: bilgiOku };
    }

    Sgb.muhasebeBirim = muhasebeBirim;
})(window.Sgb, jQuery);
