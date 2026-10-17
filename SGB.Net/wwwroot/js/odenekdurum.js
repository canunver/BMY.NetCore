// Ödenek Durum Bilgisi (ButceMuhasebe/OdenekDurumTertip) ve Ödenek Durum Bilgisi (Birim) (ButceMuhasebe/OdenekDurumHarcamaBirim):
// seçilen yıl ve tertip (birim sayfasında muhasebe / harcama birimi de) için ödenek, ödenek gönderme / tenkis, harcama ve
// (tertip sayfasında) AHP bilgileri. Değerler data-deger alanlarına yazılır; birim sayfası birimlere göre listeyi AG Grid'de gösterir.
// Yazdır Excel raporu (MUH-OdenekDurum.xlt) indirir. Uçlar: /api/odenekdurum/* (Muhasebe/OdenekDurumUclari.cs).
// Adres parametreleri eski sayfadaki gibi okunur: yil, kur, fon, fin, eko, peb, tertip (kur-fon-fin-eko-peb); birimde muhasebe, birim
// (muhasebe ve bir kod verilirse açılışta listelenir).
$(function () {
    'use strict';

    var $kok = $('.odenek-durum');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var BIRIM = $kok.attr('data-sayfa') === 'birim';
    var UC = BIRIM ? 'api/odenekdurum/birim' : 'api/odenekdurum/tertip';
    var tamSayilar = {};
    var tutar0 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 0, maximumFractionDigits: 0 });
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }

    // "13.01.31.00- Ad" -> "13.01.31.00"
    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }

    function alanKod(alan) { return kodAl($('.kod-alan[data-alan="' + alan + '"] input').val()); }

    var muhBirim = BIRIM ? Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal' },
        yil: yilOku
    }) : null;

    function secimOku() {
        var o = { yil: yilOku(), kur: alanKod('kur'), fon: alanKod('fon'), fin: alanKod('fin'), eko: alanKod('eko'), peb: alanKod('peb') };
        if (BIRIM) {
            var d = muhBirim.deger();
            o.muhasebe = d.muhasebe;
            o.birim = d.birim;
        }
        return o;
    }

    // ---------------------------------------------------------------- birimlere göre liste (yalnızca birim sayfası)

    var grid = null;
    var BOS_IPUCU = '<span>Muhasebe / birimi seçip Listele\'ye basınız.</span>';
    if (BIRIM) {
        var para = function (p) { return p.value == null ? '' : tutar2.format(p.value); };
        var sayiSutun = function (alan, ad) {
            return { field: alan, headerName: ad, width: 140, type: 'rightAligned', valueFormatter: para };
        };
        grid = Sgb.grid('#grdBirim', {
            columnDefs: [
                { field: 'muhasebe', headerName: 'Muhasebe', width: 100 },
                { field: 'birim', headerName: 'Birim', width: 80 },
                { field: 'tertip', headerName: 'Tertip', width: 260, tooltipField: 'tertip' },
                sayiSutun('odenekGonderme', 'Top. Öde. Gön.'),
                sayiSutun('yevmiyeli', 'Yevmiyeli'),
                sayiSutun('harcama', 'Top. Harcama'),
                sayiSutun('avans', 'Avans'),
                sayiSutun('kalan', 'Kalan')
            ],
            overlayNoRowsTemplate: BOS_IPUCU
        });
    }

    // ---------------------------------------------------------------- listele (eski btnListele_Click)

    function degerleriYaz(d) {
        $kok.find('[data-deger]').each(function () {
            var ad = $(this).attr('data-deger');
            var v = d && Object.prototype.hasOwnProperty.call(d, ad) ? d[ad] : null;
            $(this).text(v == null ? '' : (tamSayilar[ad] ? tutar0 : tutar2).format(v))
                .toggleClass('eksi', v != null && v < 0);
        });
    }

    function listele() {
        var o = secimOku();
        if (o.yil <= 0) { mesaj('Yıl seçiniz.', 'warning'); return; }
        if (BIRIM && o.muhasebe <= 0) { mesaj('Lütfen Muhasebe kodunu boş bırakmayınız', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        if (grid) grid.setGridOption('loading', true);
        Sgb.al(UC, o)
            .done(function (v) {
                if (v.hata) {
                    mesaj(v.hata, 'danger');
                    if (grid) grid.setGridOption('rowData', []);
                    return;
                }
                degerleriYaz(v.degerler);
                $('#odIpucu').prop('hidden', true);
                $('#odSonuc').prop('hidden', false);
                if (grid) {
                    grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                    grid.setGridOption('rowData', v.satirlar || []);
                }
            })
            .fail(hataGoster)
            .always(function () {
                $d.prop('disabled', false);
                if (grid) grid.setGridOption('loading', false);
            });
    }

    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(); });

    // ---------------------------------------------------------------- yazdır (eski btnYazdir_Click)

    $('#btnYazdir').on('click', function () {
        var o = secimOku();
        if (o.yil <= 0) { mesaj('Yıl seçiniz.', 'warning'); return; }
        if (BIRIM && o.muhasebe <= 0) { mesaj('Lütfen Muhasebe kodunu boş bırakmayınız', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir(UC + '/yazdir', o).then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- kod ağaçları (eski açılır ağaçlar, nodeLoadPar)

    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};  // tür -> { $ul, agac, yil }
    var aktifAlan = null;

    function agacAc($alan) {
        var yil = yilOku();
        if (yil <= 0) { mesaj('Yıl seçiniz.', 'warning'); return; }
        var tur = $alan.attr('data-tur');
        var ad = $alan.find('label').text();
        aktifAlan = $alan;
        $('#dlgKodBaslik').text(ad + ' Seçimi');
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[tur];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', ad + ' ağacı').appendTo('#agacKutu'), yil: yil };
            // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al('api/odenekdurum/kodlar', { yil: a.yil, tur: tur, ust: ust ? ust.kod : '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    dlgKod.close();
                    if (aktifAlan) aktifAlan.find('input').val(d.ad).trigger('change');
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
    $('.kod-alan .kod-temizle').on('click', function () { $(this).closest('.kod-alan').find('input').val('').trigger('focus'); });
    // Alana kod yazılabilir; Alt+Aşağı ağacı açar.
    $('.kod-alan input').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    var p = new URLSearchParams(window.location.search);
    function parametre(ad) { return $.trim(p.get(ad) || ''); }

    Sgb.al('api/odenekdurum/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            $.each(t.tamSayilar || [], function (i, ad) { tamSayilar[ad] = true; });

            var $y = Sgb.yilKutusu('#ddlYil', t.yillar);
            var yil = parseInt(parametre('yil'), 10) || t.yil;
            Sgb.yilKutusu($y, t.yillar, yil);

            var kodlar = { kur: parametre('kur'), fon: parametre('fon'), fin: parametre('fin'), eko: parametre('eko'), peb: parametre('peb') };
            var tertip = parametre('tertip');
            if (tertip) {
                var k = tertip.split('-');
                $.each(['kur', 'fon', 'fin', 'eko', 'peb'], function (i, ad) { if (k.length > i && k[i] !== '') kodlar[ad] = k[i]; });
            }
            if (!kodlar.kur) kodlar.kur = t.kurum || '';
            $.each(kodlar, function (ad, deger) { $('.kod-alan[data-alan="' + ad + '"] input').val(deger); });

            if (BIRIM) {
                var muhasebe = parseInt(parametre('muhasebe'), 10) || 0;
                var birim = parametre('birim');
                if (muhasebe > 0) {
                    muhBirim.ata({ muhasebe: muhasebe, birim: birim });
                    muhBirim.yenile();
                    if (kodlar.kur || kodlar.fon || kodlar.fin || kodlar.eko || kodlar.peb) listele();
                }
            }
        })
        .fail(hataGoster);
});
