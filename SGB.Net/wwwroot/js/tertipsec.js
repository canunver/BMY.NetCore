// Sgb.tertipSec: ortak tertip seçimi penceresi (eski ButceHazirlikv2/ListeTertip.aspx ve ButceMuhasebe/ListeTertip.aspx).
// Pencere sayfaya ilk kullanımda JS ile eklenir; sayfanın cshtml'ine bir şey konmaz. Uçlar: /api/tertipsec/* (OrtakSayfa/TertipSecimUclari.cs).
// Görünüm: css/tertipsec.css (+ tanim.css pencere görünümü). AG Grid ve ortak.js gerekir.
//
// Kullanım:
//   Sgb.tertipSec({
//       tur:       'muh',          // 'muh' (varsayılan): ButceMuhasebe/ListeTertip, ödenek - harcanan - bloke; ekonomik kod iki düzeyliyse
//                                  //   satırın "Alt kodlar" tuşuyla alt düzeyden seçilir.
//                                  // 'bhm': ButceHazirlikv2/ListeTertip, ödenek - gönderilen (muhasebe - birim verilirse birim sütunlarıyla).
//       yil:       2026,           // verilmezse bu yıl
//       muhasebe:  1, birim: '000',// eski muhasebe, birim (muh: kurumsal kod bunlardan bulunur; bhm: birim sütunları)
//       belgeTur:  1,              // eski belgeTur (1 varsayılan; 3 ödenek gönderme, 4 tenkis)
//       hesapKod:  '630.03.01',    // eski hesapKod: ekonomik kod hesap kodundan bulunur (muh)
//       onay:      false,          // eski onay=1 (onay / talimat formu)
//       surecNo:   '', surecYil: 0,// eski surecNo / surecYil: ekonomik kod ve (yıllara sari ise) yıl süreçten
//       kur: '', fin: '', eko: '', peb: '',   // açılıştaki kıstaslar (boş kalanlara son girilen kurumsal / ekonomik yazılır)
//       finGoster: false,          // Finans kıstas kutusu gösterilsin mi
//       coklu:     true,           // birden çok satır seçilebilir mi (seçilenler sırayla aktarılır)
//       secKapat:  false,          // eski secKapat=1: alt koddan Seç'e basınca tek tertip aktarılıp pencere kapanır
//       baslik:    'Tertip Seçimi',
//       secilince: function (satirlar, yil) { }   // false dönerse pencere açık kalır
//   });
// Aktarılan satır: { TERTIP, TERTIPPRG, KALAN, ODENEK, ACIKLAMA, FAALIYETACIKLAMA, MP, ... }
//   bhm: GONDERILEN, BIRIMKALAN, BIRIMGONDERILEN, BIRIMKULLANILAN; muh: HARCANAN, BLOKE.
(function (Sgb, $) {
    'use strict';

    var UC = 'api/tertipsec/';
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var KODLAR = [
        { alan: 'kur', etiket: 'Kurumsal', agac: 1 },
        { alan: 'fin', etiket: 'Finans', agac: 3 },
        { alan: 'eko', etiket: 'Ekonomik', agac: 4 },
        { alan: 'peb', etiket: 'Faaliyet', agac: 14 }
    ];

    var o = {};             // açık pencerenin seçenekleri
    var durum = {};         // son listenin biçimi: yil, anaGoster, mpOnayli, odenekBaslik
    var sayi = 30;          // eski hdnTertipSay
    var $p = null, pencere = null, $agacP = null, agacPencere = null;
    var anaGrid = null, altGrid = null, secilenGrid = null;
    var gridAnahtar = '';
    var altSatir = null, pebler = [];
    var agaclar = {}, aktifAlan = null;

    function tutar(x) { return x.value === null || x.value === undefined || x.value === '' ? '' : para.format(x.value); }
    function hata(xhr, metin) { if (!xhr || xhr.status !== 401) Sgb.bildirim(metin || 'Tertipler okunamadı.', 'danger'); }
    function oturumOku(ad) { try { return sessionStorage.getItem('tertipsec-' + ad) || ''; } catch (e) { return ''; } }
    function oturumYaz(ad, deger) { try { sessionStorage.setItem('tertipsec-' + ad, deger || ''); } catch (e) { /* özel pencere */ } }

    function $kod(alan) { return $p.find('.ts-kod[data-alan="' + alan + '"]'); }
    function kod(alan) { return $.trim($kod(alan).find('input').val()); }
    function yil() { return durum.yil || o.yil || new Date().getFullYear(); }

    // Ekonomik kod kırmızı ve kalın (eski TERTIPGOSTER).
    function tertipHucre(x) {
        var $h = $('<span></span>');
        $.each(String(x.value || '').split('-'), function (i, parca) {
            if (i > 0) $h.append(document.createTextNode('-'));
            if (i === 3) $('<strong class="ts-eko"></strong>').text(parca).appendTo($h);
            else $h.append(document.createTextNode(parca));
        });
        return $h[0];
    }

    // ------------------------------------------------------------------ pencere

    function kur() {
        if ($p) return;
        $p = $('<dialog class="tanim-pencere tertip-sec" aria-labelledby="tsBaslik"></dialog>').appendTo(document.body);
        $p.append('<div class="pencere-baslik"><h2 id="tsBaslik">Tertip Seçimi</h2>'
            + '<button type="button" class="close" data-kapat aria-label="Kapat"><span aria-hidden="true">&times;</span></button></div>');
        var $govde = $('<div class="pencere-govde"></div>').appendTo($p);
        var $form = $('<form class="ts-kriter" novalidate></form>').appendTo($govde);
        var $alanlar = $('<div class="ts-alanlar"></div>').appendTo($form);
        $.each(KODLAR, function (i, k) {
            var kimlik = 'tsKod_' + k.alan;
            $('<div class="ts-kod"></div>').attr({ 'data-alan': k.alan, 'data-agac': k.agac })
                .append($('<label class="control-label"></label>').attr('for', kimlik).text(k.etiket))
                .append($('<div class="input-group input-group-sm"></div>')
                    .append($('<input type="text" class="form-control" autocomplete="off" />').attr({
                        id: kimlik, title: k.etiket + ' kodu; virgülle birden çok kod yazılabilir (Alt+Aşağı: kod ağacı)'
                    }))
                    .append($('<span class="input-group-btn"></span>').append(
                        $('<button type="button" class="btn btn-default ts-agac-ac"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></button>')
                            .attr('title', 'Kod ağacından seç').append($('<span class="sr-only"></span>').text(k.etiket + ' seç')))))
                .append('<span class="ts-kod-ad" aria-live="polite"></span>')
                .appendTo($alanlar);
        });
        $form.append('<div class="ts-tuslar">'
            + '<button type="submit" class="btn btn-primary btn-sm"><span class="glyphicon glyphicon-list" aria-hidden="true"></span> Listele</button>'
            + '<button type="button" class="btn btn-default btn-sm ts-hepsi" title="Sayı sınırı olmadan, ödeneği olmayanlar dahil bütün tertipler">Aradığım tertip listede yok</button>'
            + '</div>');
        $govde.append('<p class="help-block small ts-yardim">En az iki alana değer giriniz. Birden fazla kod virgülle ayrılabilir.</p>');
        $govde.append('<div class="ts-grid ts-ana" aria-label="Tertipler"></div>');
        $govde.append('<div class="ts-alt" hidden>'
            + '<section><h3 class="ts-alt-baslik">Alt kodlar: <span class="ts-alt-tertip"></span></h3><div class="ts-grid ts-alt-grid" aria-label="Ekonomik alt kodlar"></div></section>'
            + '<section class="ts-secilen-kutu"><h3 class="ts-alt-baslik">Seçilen Tertipler</h3><div class="ts-grid ts-secilen-grid" aria-label="Seçilen tertipler"></div></section>'
            + '</div>');
        $p.append('<div class="pencere-alt"><button type="button" class="btn btn-default btn-sm" data-kapat>Vazgeç</button>'
            + '<button type="button" class="btn btn-primary btn-sm ts-aktar"><span class="glyphicon glyphicon-ok" aria-hidden="true"></span> Seç ve Kapat</button></div>');
        pencere = $p[0];

        $p.on('click', '[data-kapat]', function () { pencere.close(); });
        $form.on('submit', function (e) { e.preventDefault(); listele(false, false); });
        $p.on('click', '.ts-hepsi', function () { listele(false, true); });
        $p.on('click', '.ts-aktar', secilenleriAktar);
        $p.on('click', '.ts-agac-ac', function () { agacAc($(this).closest('.ts-kod')); });
        $p.on('keydown', '.ts-kod input', function (e) {
            if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.ts-kod')); }
        });
        $p.on('change', '.ts-kod input', function () { adGetir($(this).closest('.ts-kod')); });
    }

    // ------------------------------------------------------------------ kıstas kutuları: kod adı ve kod ağacı (eski KodAdGetir, KodAgac.aspx)

    function adYaz($a, ad) { $a.find('.ts-kod-ad').text(ad || '').attr('title', ad || null); }

    function adGetir($a) {
        var k = $.trim($a.find('input').val());
        if (!k || /[,;]/.test(k)) { adYaz($a, ''); return; }
        Sgb.al(UC + 'kodad', { yil: yil(), tur: $a.attr('data-alan'), kod: k })
            .done(function (v) { adYaz($a, v.hata ? '' : v.ad); })
            .fail(function () { adYaz($a, ''); });
    }

    function agacAc($a) {
        if (!$agacP) {
            $agacP = $('<dialog class="tanim-pencere tertip-sec-agac" aria-labelledby="tsAgacBaslik"></dialog>')
                .append('<div class="pencere-baslik"><h2 id="tsAgacBaslik">Kod Seçimi</h2>'
                    + '<button type="button" class="close" data-kapat aria-label="Kapat"><span aria-hidden="true">&times;</span></button></div>')
                .append('<div class="pencere-govde"></div>')
                .append('<div class="pencere-alt"><button type="button" class="btn btn-default btn-sm" data-kapat>Kapat</button></div>')
                .appendTo(document.body);
            agacPencere = $agacP[0];
            $agacP.on('click', '[data-kapat]', function () { agacPencere.close(); });
        }
        var tur = $a.attr('data-agac');
        var baslik = $a.find('label').text();
        aktifAlan = $a;
        $agacP.find('h2').text(baslik + ' Kod Seçimi');
        $agacP.find('.pencere-govde > ul').prop('hidden', true);
        agacPencere.showModal();
        var a = agaclar[tur];
        if (a) {
            a.$ul.prop('hidden', false);
            if (a.yil !== yil()) { a.yil = yil(); a.agac.yenile(); }
            return;
        }
        a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', baslik + ' ağacı').appendTo($agacP.find('.pencere-govde')), yil: yil() };
        a.agac = Sgb.agac(a.$ul[0], {
            yukle: function (ust) {
                return Sgb.al(UC + 'kodlar', { yil: a.yil, tur: tur, ust: ust ? ust.kod : '' }).then(function (v) {
                    if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return []; }
                    return v.dugumler;
                });
            },
            secilince: function (d) {
                agacPencere.close();
                if (!aktifAlan) return;
                // Düğüm adı "kod- Ad" biçimindedir.
                var i = d.ad.indexOf('- ');
                aktifAlan.find('input').val(d.kod);
                adYaz(aktifAlan, i > 0 ? d.ad.substring(i + 2) : '');
            },
            seciliMi: function (d) { return !!aktifAlan && $.trim(aktifAlan.find('input').val()) === d.kod; },
            ilkAcik: true,
            bosMetin: 'Kod bulunamadı.',
            hata: function (xhr) { hata(xhr, 'Kodlar okunamadı.'); }
        });
    }

    // ------------------------------------------------------------------ tertip listesi

    function anaSutunlar() {
        var s = function (alan, ad) { return { field: alan, headerName: ad, width: 125, type: 'rightAligned', valueFormatter: tutar }; };
        var d = [{ field: 'TERTIP', headerName: 'Tertip', width: 250, cellRenderer: tertipHucre, tooltipField: 'TERTIP' }];
        if (o.tur === 'bhm') {
            var birimli = (parseInt(o.muhasebe, 10) || 0) > 0;
            d.push(s('KALAN', 'Tertip Kalan'));
            if (birimli) d.push(s('BIRIMKALAN', 'Birim Kalan'));
            d.push(s('ODENEK', durum.odenekBaslik || 'Tertip Ödeneği'), s('GONDERILEN', 'Tertipten Gönderilen'));
            if (birimli) d.push(s('BIRIMGONDERILEN', 'Birime Gönderilen'), s('BIRIMKULLANILAN', 'Birimde Kullanılan'));
        } else {
            d.push(s('KALAN', 'Kalan'), s('ODENEK', 'Ödenek'), s('HARCANAN', 'Harcanan'), s('BLOKE', 'Bloke'));
        }
        if (durum.mpOnayli) d.push(s('MP', 'Masraf Planı'));
        d.push({ field: 'ACIKLAMA', headerName: 'Açıklama', width: 220, tooltipField: 'ACIKLAMA' },
            { field: 'FAALIYETACIKLAMA', headerName: 'Faaliyet', width: 200, tooltipField: 'FAALIYETACIKLAMA' },
            { field: 'TERTIPPRG', headerName: 'Program Tertibi', width: 220, tooltipField: 'TERTIPPRG' });
        if (!durum.anaGoster) {
            d.unshift({
                headerName: '', width: 92, sortable: false, pinned: 'left',
                cellRenderer: function (x) {
                    return $('<button type="button" class="btn btn-default btn-xs">Alt kodlar</button>')
                        .attr('title', 'Ekonomik kodun alt düzeylerini göster')
                        .on('click', function () { altKodlariAc(x.data); })[0];
                }
            });
        }
        return d;
    }

    // Sütunlar ve seçim biçimi son listeye göre; değişmişse tablo yeniden kurulur.
    function anaGridKur() {
        var anahtar = [o.tur, o.muhasebe > 0, o.coklu !== false, durum.anaGoster, durum.mpOnayli, durum.odenekBaslik].join('|');
        if (anaGrid && anahtar === gridAnahtar) return;
        gridAnahtar = anahtar;
        if (anaGrid) anaGrid.destroy();
        var secim;
        if (durum.anaGoster)
            secim = o.coklu === false ? { mode: 'singleRow', checkboxes: true, enableClickSelection: true }
                : { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true };
        anaGrid = Sgb.grid($p.find('.ts-ana')[0], {
            columnDefs: anaSutunlar(),
            rowSelection: secim,
            overlayNoRowsTemplate: '<span>Kıstasları girip Listele\'ye basınız.</span>',
            onRowDoubleClicked: function (e) { if (durum.anaGoster) aktar([e.data]); }
        });
    }

    function altPaneliGoster(goster) {
        $p.find('.ts-alt').prop('hidden', !goster);
        $p.toggleClass('ts-alt-acik', goster);
        $p.find('.ts-secilen-kutu').prop('hidden', !!o.secKapat);
        $p.find('.ts-aktar').prop('hidden', !goster ? false : !!o.secKapat);
    }

    function listele(ilk, hepsi) {
        if (anaGrid) anaGrid.setGridOption('loading', true);
        var istek = {
            tur: o.tur, yil: o.yil || '', muhasebe: o.muhasebe || '', birim: o.birim || '', belgeTur: o.belgeTur || 1,
            hesapKod: o.hesapKod || '', onay: o.onay ? 1 : '', surecNo: o.surecNo || '', surecYil: o.surecYil || '',
            kur: kod('kur'), fin: kod('fin'), eko: kod('eko'), peb: kod('peb'),
            hepsi: hepsi ? 1 : '', sayi: sayi, ilk: ilk ? 1 : ''
        };
        if (ilk) { istek.oturumKur = oturumOku('kur'); istek.oturumEko = oturumOku('eko'); }
        return Sgb.al(UC + 'liste', istek).done(function (v) {
            if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
            durum = { yil: v.yil, anaGoster: v.anaGoster !== false, mpOnayli: !!v.mpOnayli, odenekBaslik: v.odenekBaslik };
            anaGridKur();
            altPaneliGoster(!durum.anaGoster);
            if (ilk) {
                $kod('kur').find('input').val(v.kur || '');
                $kod('eko').find('input').val(v.eko || '');
                $p.find('.ts-kod').each(function () { adGetir($(this)); });
            }
            oturumYaz('kur', kod('kur'));
            oturumYaz('eko', kod('eko'));
            sayi = v.sayi || sayi;
            if (v.uyari) Sgb.bildirim(v.uyari, 'warning');
            anaGrid.setGridOption('overlayNoRowsTemplate', '<span>' + (ilk && !(v.satirlar || []).length ? 'Kıstasları girip Listele\'ye basınız.' : 'Tertip bulunamadı.') + '</span>');
            anaGrid.setGridOption('rowData', v.satirlar || []);
        }).fail(function (xhr) { hata(xhr); }).always(function () { if (anaGrid) anaGrid.setGridOption('loading', false); });
    }

    // ------------------------------------------------------------------ ekonomik kodun alt düzeyleri (eski BeforeExpand, DetayTertipTusu)

    function altGridKur() {
        if (altGrid) altGrid.destroy();
        var s = [
            {
                headerName: 'Seç', width: 70, sortable: false,
                cellRenderer: function (x) {
                    return $('<button type="button" class="btn btn-default btn-xs"><span class="glyphicon glyphicon-ok" aria-hidden="true"></span> Seç</button>')
                        .on('click', function () { altKodSec(x.data); })[0];
                }
            },
            { field: 'TERTIP', headerName: 'Tertip', width: 240, cellRenderer: tertipHucre, tooltipField: 'TERTIP' }
        ];
        if (pebler.length) {
            s.push({
                field: 'PEB', headerName: 'Faaliyet', width: 200, editable: true, singleClickEdit: true, cellClass: 'ts-girilir',
                cellEditor: 'agSelectCellEditor', cellEditorParams: { values: $.map(pebler, function (x) { return x.kod; }) },
                valueFormatter: function (x) {
                    var b = $.grep(pebler, function (y) { return y.kod === x.value; });
                    return b.length ? b[0].kod + ' - ' + (b[0].ad || '') : (x.value || '');
                }
            });
        }
        s.push({ field: 'ACIKLAMA', headerName: 'Açıklama', flex: 1, minWidth: 200, tooltipField: 'ACIKLAMA' });
        altGrid = Sgb.grid($p.find('.ts-alt-grid')[0], { columnDefs: s, stopEditingWhenCellsLoseFocus: true, overlayNoRowsTemplate: '<span>Alt kod bulunamadı.</span>' });
    }

    function secilenGridKur() {
        if (secilenGrid || o.secKapat) return;
        var s = function (alan, ad) { return { field: alan, headerName: ad, width: 120, type: 'rightAligned', valueFormatter: tutar }; };
        secilenGrid = Sgb.grid($p.find('.ts-secilen-grid')[0], {
            columnDefs: [
                {
                    headerName: '', width: 50, sortable: false,
                    cellRenderer: function (x) {
                        return $('<button type="button" class="btn btn-link btn-xs" title="Listeden çıkar"><span class="glyphicon glyphicon-remove" aria-hidden="true"></span><span class="sr-only">Listeden çıkar</span></button>')
                            .on('click', function () { secilenGrid.applyTransaction({ remove: [x.data] }); })[0];
                    }
                },
                { field: 'TERTIP', headerName: 'Tertip', width: 240, cellRenderer: tertipHucre },
                s('KALAN', 'Kalan'), s('ODENEK', 'Ödenek'), s('HARCANAN', 'Harcanan'),
                { field: 'ACIKLAMA', headerName: 'Açıklama', width: 200, tooltipField: 'ACIKLAMA' }
            ],
            overlayNoRowsTemplate: '<span>Alt koddan Seç ile tertip ekleyiniz.</span>'
        });
    }

    function altKodlariAc(ana) {
        altSatir = ana;
        $p.find('.ts-alt-tertip').text(ana.TERTIP);
        secilenGridKur();
        Sgb.al(UC + 'altkod', { yil: yil(), tertip: ana.TERTIP, eko: kod('eko') })
            .done(function (v) {
                if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
                pebler = v.pebler || [];
                altGridKur();
                altGrid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(function (xhr) { hata(xhr, 'Alt kodlar okunamadı.'); });
    }

    // Seçilen alt kodun tutarları ana satırdan gelir (eski DetayTertipTusu). secKapat: hemen aktarılır (eski DetayTertipTusu2).
    function altKodSec(alt) {
        if (!altSatir) return;
        if (altGrid) altGrid.stopEditing();
        var ana = altSatir;
        var kayit = {
            TERTIP: alt.TERTIP + (alt.PEB ? '-' + alt.PEB : ''), TERTIPPRG: alt.TERTIPPRG || '',
            KALAN: ana.KALAN, ODENEK: ana.ODENEK, HARCANAN: ana.HARCANAN, BLOKE: ana.BLOKE, MP: ana.MP,
            ACIKLAMA: ana.ACIKLAMA, FAALIYETACIKLAMA: ana.FAALIYETACIKLAMA
        };
        var bitti = function () {
            if (o.secKapat) aktar([kayit]);
            else secilenGrid.applyTransaction({ add: [kayit] });
        };
        if (!alt.PEB && kayit.TERTIPPRG) { bitti(); return; }
        Sgb.al(UC + 'prg', { yil: yil(), tertip: kayit.TERTIP })
            .done(function (v) { kayit.TERTIPPRG = v.prg || ''; bitti(); })
            .fail(function (xhr) { hata(xhr, 'Program tertibi okunamadı.'); });
    }

    // ------------------------------------------------------------------ aktarma (eski SecKapatBasildi / SecKapatBasildiDetay)

    function aktar(satirlar) {
        if (!satirlar.length) { Sgb.bildirim('Tertip seçiniz.', 'warning'); return; }
        var sonuc = o.secilince ? o.secilince($.map(satirlar, function (x) { return $.extend({}, x); }), yil()) : undefined;
        if (sonuc !== false) pencere.close();
    }

    function secilenleriAktar() {
        var secili = [];
        if (durum.anaGoster) secili = anaGrid ? anaGrid.getSelectedRows() : [];
        else if (secilenGrid) secilenGrid.forEachNode(function (n) { secili.push(n.data); });
        aktar(secili);
    }

    // ------------------------------------------------------------------ açma

    function tertipSec(secenek) {
        kur();
        o = $.extend({ tur: 'muh', belgeTur: 1, coklu: true }, secenek || {});
        o.tur = o.tur === 'bhm' ? 'bhm' : 'muh';
        durum = { yil: parseInt(o.yil, 10) || 0 };
        sayi = 30;
        altSatir = null;
        $p.find('#tsBaslik').text(o.baslik || 'Tertip Seçimi');
        $kod('fin').prop('hidden', !o.finGoster);
        $.each(KODLAR, function (i, k) { $kod(k.alan).find('input').val(o[k.alan] || ''); adYaz($kod(k.alan), ''); });
        $p.find('.ts-alt-tertip').text('');
        if (anaGrid) anaGrid.setGridOption('rowData', []);
        if (altGrid) altGrid.setGridOption('rowData', []);
        if (secilenGrid) secilenGrid.setGridOption('rowData', []);
        altPaneliGoster(false);
        pencere.showModal();
        listele(true, false);
    }

    Sgb.tertipSec = tertipSec;
})(window.Sgb, jQuery);
