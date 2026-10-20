// SGB - KBS Fark (Faal.) (ButceMuhasebe/SGBKBSDuzeltmePEB; eski aynı adlı Ext.NET sayfası).
// "Listele Harcama Fark" SGB ile KBS harcamaları arasındaki farkları, "Listele Gönderme Harcama Fark" gönderilen ödenek ile KBS
// harcaması arasındaki farkları faaliyet kırılımında listeler. Kaydet, seçili satırlar için ters kayıt yapar (harcama farkında
// ödeme emri / MİF, gönderme farkında tenkis belgesi); işlem son listelemenin türüne göre yapılır. Kopyala listeyi panoya alır.
// Uçlar: /api/sgbkbsfark/* (Muhasebe/SgbKbsFarkUclari.cs).
$(function () {
    'use strict';

    var $kok = $('.sgb-kbs-fark');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/sgbkbsfark/';
    var IPUCU = '<span>Kıstasları seçip listeleme tuşlarından birine basınız.</span>';
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var sonKriter = null; // son listelemenin kıstasları (Kaydet aynı yıl ve tür ile yapılır)

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function para(v) { return v === null || v === undefined || v === '' ? '' : tutar2.format(Number(v) || 0); }
    function sayiMetin(s) { s = $.trim(s || ''); return s.indexOf(',') >= 0 ? s.replace(/\./g, '').replace(',', '.') : s; }

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;
    function sor(metin, is) {
        $('#soruMetin').text(metin);
        soruIsi = is;
        dlgSoru.showModal();
    }
    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        var is = soruIsi;
        soruIsi = null;
        if (is) is();
    });
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- kıstaslar

    function yil() { return parseInt($('#ddlYil').val(), 10) || 0; }

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: yil,
        kurumsalGizle: true,
        adGizle: true
    });

    // "13.01.31.00- Ad" -> "13.01.31.00"
    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }
    function alanKod(alan) { return kodAl($('.kod-alan[data-alan="' + alan + '"] input').val()); }

    function kriter(mode) {
        return {
            mode: mode,
            yil: yil(),
            ay: parseInt($('#ddlAy').val(), 10) || 0,
            // Saymanlık alanına birden çok kod (1,5) ya da aralık (1-9) yazılabilir (eski serbest metin alanı gibi).
            muhasebe: $.trim($('#txtMuhasebe').val()),
            birim: muhBirim.deger().birim,
            kur: alanKod('kur'), fon: alanKod('fon'), fin: alanKod('fin'), eko: alanKod('eko'), peb: alanKod('peb'),
            durum: parseInt($('#ddlHarcamaDurum').val(), 10) || 0,
            minFark: sayiMetin($('#txtEnAzFark').val()) || '0',
            farkTur: parseInt($('#ddlFarkTur').val(), 10) || 0
        };
    }

    // Kod ağaçları (Ödenek Durum sayfalarıyla aynı alanlar ve pencere).
    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};
    var aktifAlan = null;
    function agacAc($alan) {
        var y = yil();
        if (y <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        var tur = $alan.attr('data-tur');
        var ad = $alan.find('label').text();
        aktifAlan = $alan;
        $('#dlgKodBaslik').text(ad + ' Seçimi');
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[tur];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', ad + ' ağacı').appendTo('#agacKutu'), yil: y };
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al(UC + 'kodlar', { yil: a.yil, tur: tur, ust: ust ? ust.kod : '' }).then(function (v) {
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
            if (a.yil !== y) { a.yil = y; a.agac.yenile(); }
        }
    }
    $('.kod-alan .kod-sec').on('click', function () { agacAc($(this).closest('.kod-alan')); });
    $('.kod-alan .kod-temizle').on('click', function () { $(this).closest('.kod-alan').find('input').val('').trigger('focus'); });
    $('.kod-alan input').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
    });

    // ---------------------------------------------------------------- liste

    // Öğeyi sayfa alanının (.sayfa-icerik) kalan yüksekliğine yayar; sayfada dikey kaydırma çıkmaz, kaydırma öğenin içinde kalır.
    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var kap = el.closest('.sayfa-icerik') || document.body, cs = getComputedStyle(kap);
        var kaydirir = /(auto|scroll)/.test(cs.overflowY);
        var kr = kap.getBoundingClientRect(), kay = kaydirir ? kap.scrollTop : 0;
        var gorunen = kaydirir ? kap.clientHeight : window.innerHeight - kr.top;
        var ust = el.getBoundingClientRect().top - kr.top + kay;
        var son = 0;
        $(kap).find('.og-liste .bolum-liste').each(function () {
            son = Math.max(son, this.getBoundingClientRect().bottom - kr.top + kay + parseFloat(getComputedStyle(this).marginBottom || 0));
        });
        son += parseFloat(cs.paddingBottom || 0);
        var bosluk = Math.max(0, son - (ust + el.offsetHeight));
        el.style.height = Math.max(enAz, Math.floor(gorunen - ust - bosluk - 1)) + 'px';
    }

    function kolonlar(mode) {
        return [
            { field: 'SayKod', headerName: 'Say.', minWidth: 60, flex: 60, type: 'rightAligned' },
            { field: 'BirimKod', headerName: 'Birim', minWidth: 60, flex: 60 },
            { field: 'Tertip', headerName: 'Tertip', minWidth: 220, flex: 300, tooltipField: 'Tertip' },
            { field: 'SGBHarcama', headerName: mode === 2 ? 'Gönderilen' : 'SGB Harcama', minWidth: 110, flex: 120, type: 'rightAligned', valueFormatter: function (x) { return para(x.value); } },
            { field: 'KBSHarcama', headerName: 'KBS Harcama', minWidth: 110, flex: 120, type: 'rightAligned', valueFormatter: function (x) { return para(x.value); } },
            { field: 'fark', headerName: 'Fark', minWidth: 110, flex: 120, type: 'rightAligned', valueFormatter: function (x) { return para(x.value); } }
        ];
    }

    var grid = Sgb.grid('#grdFark', {
        columnDefs: kolonlar(1),
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        overlayNoRowsTemplate: IPUCU
    });

    function ozetYaz() {
        var adet = 0, toplam = 0;
        grid.forEachNode(function (n) { adet++; toplam += Number(n.data.fark) || 0; });
        $('#lblOzet').text(adet ? adet + ' satır, fark toplamı ' + tutar2.format(toplam) : '');
    }

    function hataYaz(metin) { Sgb.mesaj('#kutuHata', metin || '', 'danger'); listeYay(); }

    function listele(k, sessiz) {
        if (!(k.yil > 0)) { mesaj('Yıl giriniz.', 'warning'); return; }
        if (!(k.ay >= 1 && k.ay <= 12)) { mesaj('Ay 1 ile 12 arasında olmalıdır.', 'warning'); return; }
        var $d = $kok.find('.listele, #btnKaydet').prop('disabled', true);
        grid.deselectAll();
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', k)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                sonKriter = k;
                grid.setGridOption('columnDefs', kolonlar(v.mode));
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
                grid.setGridOption('rowData', v.satirlar || []);
                ozetYaz();
                if (!sessiz && !(v.satirlar || []).length) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $kok.find('.listele').on('click', function () {
        hataYaz('');
        listele(kriter(parseInt($(this).attr('data-mode'), 10) || 1), false);
    });
    $('#frmKriter').on('submit', function (e) { e.preventDefault(); });

    // ---------------------------------------------------------------- kopyala (eski CopyStore)

    $('#btnKopyala').on('click', function () {
        var satirlar = [];
        grid.forEachNodeAfterFilterAndSort(function (n) { satirlar.push(n.data); });
        if (!satirlar.length) { mesaj('Kopyalanacak satır yok.', 'info'); return; }
        var sayiYaz = function (v) { return (Number(v) || 0).toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2, useGrouping: false }); };
        var kol = kolonlar(sonKriter ? sonKriter.mode : 1);
        var metin = [$.map(kol, function (c) { return c.headerName; }).join('\t')].concat($.map(satirlar, function (d) {
            return [[d.SayKod, d.BirimKod || '', d.Tertip || '', sayiYaz(d.SGBHarcama), sayiYaz(d.KBSHarcama), sayiYaz(d.fark)].join('\t')];
        })).join('\r\n');
        var tamam = function () { mesaj(satirlar.length + ' satır panoya kopyalandı.', 'success'); };
        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(metin).then(tamam, function () { mesaj('Panoya kopyalanamadı.', 'danger'); });
        } else {
            var $t = $('<textarea class="sr-only"></textarea>').val(metin).appendTo(document.body);
            $t[0].select();
            try { document.execCommand('copy'); tamam(); } catch (e) { mesaj('Panoya kopyalanamadı.', 'danger'); }
            $t.remove();
        }
    });

    // ---------------------------------------------------------------- kaydet (eski btnKaydet_Click)

    $('#btnKaydet').on('click', function () {
        if (!sonKriter) { mesaj('Hazır değil', 'warning'); return; }
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('ters kayıt yapılacak işlemleri seçiniz', 'warning'); return; }
        sor('Seçili kayıtlar için ters kayıt işlemi yapılacaktır. Onaylıyor musunuz?', function () {
            var k = sonKriter;
            var $d = $kok.find('.listele, #btnKaydet').prop('disabled', true);
            hataYaz('');
            Sgb.gonder(UC + 'kaydet', {
                Yil: k.yil, Mode: k.mode, Ihale: $('#ddlIhale').val() || '',
                Satirlar: $.map(secili, function (r) {
                    return { SayKod: r.SayKod, BirimKod: r.BirimKod, Tertip: r.Tertip, SGBHarcama: r.SGBHarcama, KBSHarcama: r.KBSHarcama };
                })
            })
                .done(function (v) {
                    if (v.hata) hataYaz(v.hata);
                    if (v.yapilan > 0) mesaj(v.bilgi, v.hata ? 'warning' : 'success');
                    else if (!v.hata) mesaj('Ters kayıt yapılacak fark bulunamadı.', 'info');
                    listele(k, true);
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al(UC + 'tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            Sgb.yilKutusu('#ddlYil', t.yillar, t.yil);
            $('#ddlAy').val(String(t.ay));
        })
        .fail(hataGoster);

    function listeYay() { yuksekligeYay(document.getElementById('grdFark'), 250); }
    $(window).on('resize', listeYay);
    listeYay();
});
