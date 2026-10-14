// Faaliyet Detay Teklif Girişi - SGB (ButceHazirlikv2/TavanTertipCariSGB): faaliyet/proje ağaçtan seçilir, faaliyetin SGB görüşme (cari)
// tavanlarından biri seçilip listelenir; tavan tertiplere (kurumsal-fonksiyonel-finansman-ekonomik) üç yıllık görüşme sonrası tutar ve
// ek ödenek olarak dağıtılır. En üstteki TAVAN, FARK (tavan - toplam) ve TOPLAM satırları salt okunurdur; bir tutar değişince yeniden
// hesaplanır. Giriş yalnızca faaliyetin görüşmesi onaylıyken (5) ve tertip teklifleri onaylanmamışken açıktır. Raporlar eski kodla üretilir.
$(function () {
    'use strict';

    if (!$('#grdTertip').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TG = window.TavanGorusme, mesaj = TG.mesaj, hataGoster = TG.hataGoster;
    // TOPLAM ve FARK satırlarında hesaplanan alanlar (eski Oku).
    var SAYILAR = ['yil0', 'yil1', 'yil2', 'ekOdenek', 'ekOdenek1', 'ekOdenek2',
        'teklif0', 'teklif1', 'teklif2', 'ekOdenekTeklif', 'ekOdenek1Teklif', 'ekOdenek2Teklif'];
    var GIRILEN = ['yil0', 'yil1', 'yil2', 'ekOdenek', 'ekGerekce', 'ekOdenek1', 'ekOdenek2'];
    // Yapıştır yalnızca tutarları yazar (tablodaki sırayla).
    var YAPISTIRILAN = ['yil0', 'yil1', 'yil2', 'ekOdenek'];
    // Eski sayfada gizli olan sonraki yılların ek ödenek sütunları ("Sonraki yılların ek ödenekleri" ile gösterilir).
    var EK_YIL = ['ekOdenek1', 'ekOdenek2', 'ekOdenek1Teklif', 'ekOdenek2Teklif'];

    var acik = null;        // listelenmiş seçim { yil, kod ("kur-fon-fin-eko-faaliyet") }: Kaydet / Onay bunu kullanır
    var tavan = null;       // seçilen görüşme tavanı (TAVAN satırı)
    var durum = -1;         // tertip tekliflerinin onay durumu: 0/1 açık, 5 onaylı, -1 görüşme onaysız ya da okunamadı
    var gorusmeDurum = -1;  // faaliyetin SGB görüşme onay durumu (5 değilse giriş kapalı)
    var kirli = false;      // kaydedilmemiş değişiklik var mı
    var faal = '';          // ağaçtan seçilen faaliyet/proje kodu

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function onayli() { return durum > 1; }
    function girisAcik() { return !!acik && durum >= 0 && !onayli(); }
    function tertipSatiri(d) { return !!d && String(d.kod4 || '').length > 2; }
    function kacir(metin) { return $('<span></span>').text(metin == null ? '' : metin).html(); }

    // ---------------------------------------------------------------- tablo

    // Yalnızca tertip satırları (ekonomik kodu 2 haneden uzun), giriş açıkken girilir (eski BeforeEditPar).
    function girilebilir(p) {
        return girisAcik() && !p.node.rowPinned && GIRILEN.indexOf(p.colDef.field) >= 0 && tertipSatiri(p.data);
    }

    function hucreSinifi(p) {
        var s = [];
        if (p.colDef.type === 'sayi') s.push('sayi');
        if (p.colDef.field === 'ekGerekce' || p.colDef.field === 'ekGerekceTeklif') s.push('metin');
        if (p.colDef.colId === 'tertip') s.push('tertip');
        if (!p.node.rowPinned) {
            if (tertipSatiri(p.data)) s.push(girilebilir(p) ? 'girilir' : 'kapali');
        } else if (p.data.kod4 === '--' && typeof p.value === 'number' && p.value < 0) s.push('eksi');
        return s;
    }

    function tertipKod(d) { return [d.kod1, d.kod2, d.kod3, d.kod4].join('-'); }

    // Tertip: "kurumsal-fonksiyonel-finansman-ekonomik" kodu ve altında ad (eski ColorRendererKodAd).
    function tertipCiz(p) {
        if (!p.data) return '';
        if (p.node.rowPinned || !tertipSatiri(p.data)) return kacir(p.data.ad4);
        return '<span class="kod">' + kacir(tertipKod(p.data)) + '</span>' + kacir(p.data.ad4);
    }

    function tertipMetin(p) {
        if (!p.data) return '';
        if (p.node.rowPinned || !tertipSatiri(p.data)) return p.data.ad4;
        return tertipKod(p.data) + ' ' + (p.data.ad4 || '');
    }

    function sutunlar(yil) {
        var giris = TG.giris(girilebilir);
        var ekYil = $('#chkEkYillar').prop('checked');
        function s(alan, baslik, ek) { return TG.sayiSutunu(alan, baslik, hucreSinifi, ek); }
        function g(alan, baslik) { return s(alan, baslik, $.extend({ hide: !ekYil }, giris)); }
        return [
            { colId: 'tertip', headerName: 'Tertip', width: 280, pinned: 'left', sortable: false, cellRenderer: tertipCiz, valueGetter: tertipMetin,
                tooltipValueGetter: tertipMetin, cellClass: hucreSinifi },
            s('yil0', yil + ' Gör. Sonrası', giris),
            s('yil1', (yil + 1) + ' Gör. Sonrası', giris),
            s('yil2', (yil + 2) + ' Gör. Sonrası', giris),
            s('ekOdenek', yil + ' Ek Gör. Sonrası', $.extend({ width: 118 }, giris)),
            { field: 'ekGerekce', headerName: 'Ek Gerekçe', width: 240, sortable: false, tooltipField: 'ekGerekce', cellClass: hucreSinifi,
                editable: girilebilir, cellEditor: 'agLargeTextCellEditor', cellEditorPopup: true, cellEditorParams: { maxLength: 300, rows: 5, cols: 50 } },
            g('ekOdenek1', (yil + 1) + ' Ek Gör. Sonrası'),
            g('ekOdenek2', (yil + 2) + ' Ek Gör. Sonrası'),
            s('teklif0', yil + ' Teklif'),
            s('teklif1', (yil + 1) + ' Teklif'),
            s('teklif2', (yil + 2) + ' Teklif'),
            s('ekOdenekTeklif', yil + ' Ek Teklif'),
            { field: 'ekGerekceTeklif', headerName: 'Ek Gerekçe (Teklif)', width: 240, sortable: false, tooltipField: 'ekGerekceTeklif', cellClass: hucreSinifi },
            s('ekOdenek1Teklif', (yil + 1) + ' Ek Teklif', { hide: !ekYil }),
            s('ekOdenek2Teklif', (yil + 2) + ' Ek Teklif', { hide: !ekYil })
        ];
    }

    var grid = Sgb.grid('#grdTertip', {
        columnDefs: sutunlar(new Date().getFullYear()),
        // Hücreye tıklayınca düzenlenir; Tab / Enter ile sonraki hücreye geçilir.
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        tooltipShowDelay: 500,
        getRowId: function (p) { return p.data.id; },
        getRowHeight: function (p) { return !p.node.rowPinned && tertipSatiri(p.data) ? 40 : 30; },
        getRowClass: function (p) {
            if (p.node.rowPinned) return p.data.kod4 === '++' ? 'satir-tavan' : '';
            return tertipSatiri(p.data) ? '' : 'satir-toplam';
        },
        overlayNoRowsTemplate: '<span>Faaliyet/proje ve görüşme tavanı seçip Listele\'ye basınız.</span>',
        onCellValueChanged: function (e) {
            if (e.node.rowPinned || e.newValue === e.oldValue) return;
            kirli = true;
            toplamHesapla();
        }
    });

    // Üstteki satırlar (eski "T A V A N", "F A R K" ve sütun başlığındaki "T O P L A M"); TOPLAM bütün satırların toplamıdır.
    function toplamHesapla() {
        if (!tavan) { grid.setGridOption('pinnedTopRowData', []); return; }
        var t = { id: '00', kod4: '00', ad4: 'T O P L A M', ekGerekce: '', ekGerekceTeklif: '' };
        $.each(SAYILAR, function (i, a) { t[a] = 0; });
        grid.forEachNode(function (n) { $.each(SAYILAR, function (i, a) { t[a] += Number(n.data[a]) || 0; }); });
        var f = { id: '--', kod4: '--', ad4: 'F A R K', ekGerekce: '', ekGerekceTeklif: '' };
        $.each(SAYILAR, function (i, a) { f[a] = Math.round(((Number(tavan[a]) || 0) - t[a]) * 100) / 100; });
        grid.setGridOption('pinnedTopRowData', [$.extend({}, tavan, { id: '++', kod4: '++', ad4: 'T A V A N', ekGerekce: '', ekGerekceTeklif: '' }), f, t]);
    }

    // Tuşlar onay durumuna göre (eski TuslariAyarla): görüşme onaysız -> hiçbiri; 5 -> Onay Kaldır; diğer -> Kaydet, Onayla.
    function durumGoster() {
        var $e = $('#durumEtiket');
        var var_ = !!acik && durum >= 0;
        if (!acik) $e.prop('hidden', true);
        else if (!var_) $e.prop('hidden', false).removeClass('label-success').addClass('label-default')
            .text(gorusmeDurum !== 5 ? 'SGB görüşmesi onaylı değil (giriş kapalı)' : 'Onay durumu okunamadı');
        else $e.prop('hidden', false).toggleClass('label-success', onayli()).toggleClass('label-default', !onayli()).text(onayli() ? 'Onaylı' : 'Açık');
        $('#btnKaydet, #btnOnayla, #btnTekliftenAl, #btnYapistir').prop('hidden', !var_ || onayli());
        $('#btnKaydet, #btnTekliftenAl, #btnYapistir').prop('disabled', !grid.getDisplayedRowCount());
        $('#btnOnayKaldir').prop('hidden', !var_ || durum !== 5);
        $('#btnKopyala').prop('disabled', !acik);
        grid.refreshCells({ force: true });
    }

    function degisiklikVarsaSor(neden) {
        return !kirli || window.confirm('Kaydedilmemiş değişiklikler var. ' + (neden || 'Devam ederseniz') + ' kaybolacak. Devam edilsin mi?');
    }

    function listele() {
        var o = { yil: yilOku(), kod: $('#ddlEkoFaal').val() || '' };
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!o.kod) { mesaj('Lütfen seçim yapınız!', 'warning'); return; }
        if (!degisiklikVarsaSor()) return;
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/tavantertipsgb/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = o;
                tavan = v.tavan;
                durum = v.durum;
                gorusmeDurum = v.gorusmeDurum;
                kirli = false;
                $.each(v.satirlar, function (i, s) { s.id = 's' + i; });
                grid.setGridOption('columnDefs', sutunlar(o.yil));
                grid.setGridOption('overlayNoRowsTemplate', '<span>Tertip bulunamadı.</span>');
                grid.setGridOption('rowData', v.satirlar);
                toplamHesapla();
                durumGoster();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmSecim').on('submit', function (olay) { olay.preventDefault(); listele(); });

    // Seçim değişince eski liste kaydedilemez (Kaydet / Onay listelenen tavana yazar).
    function listeyiBosalt() {
        if (!acik) return;
        acik = null;
        tavan = null;
        durum = -1;
        gorusmeDurum = -1;
        kirli = false;
        grid.setGridOption('rowData', []);
        grid.setGridOption('pinnedTopRowData', []);
        durumGoster();
    }

    // Görüşme tavanı seçenekleri: değer tertip kodu "kur-fon-fin-eko-faaliyet" (eski cbEkoFaal TertipKod / kod4Aciklama).
    function ekoFaalDoldur(liste) {
        var $ddl = $('#ddlEkoFaal');
        $ddl.empty();
        $('<option></option>').val('').text(!faal ? 'Önce faaliyet/proje seçiniz' : liste && liste.length ? 'Seçiniz' : '(tavan yok)').appendTo($ddl);
        $.each(liste || [], function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($ddl); });
        if (liste && liste.length === 1) $ddl.val(liste[0].kod);
    }

    function ekoFaalYukle() {
        var yil = yilOku();
        ekoFaalDoldur([]);
        if (yil < 2000 || !faal) return;
        Sgb.al('api/tavantertipsgb/ekofaal', { yil: yil, faal: faal })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'warning'); return; }
                ekoFaalDoldur(v.kodlar);
            })
            .fail(hataGoster);
    }

    $('#ddlEkoFaal').on('change', function () {
        if (acik && !degisiklikVarsaSor('Seçimi değiştirirseniz')) { this.value = acik.kod; return; }
        listeyiBosalt();
    });

    // Faaliyetler yıla bağlıdır: yıl değişince seçim boşalır (eski YilDegisti).
    $('#txtYil').on('change', function () {
        if (acik && !degisiklikVarsaSor('Yılı değiştirirseniz')) { this.value = acik.yil; return; }
        listeyiBosalt();
        faal = '';
        $('#txtFaaliyet').val('');
        ekoFaalDoldur([]);
        agacYil = 0;
        grid.setGridOption('columnDefs', sutunlar(yilOku() || new Date().getFullYear()));
    });

    $('#chkEkYillar').on('change', function () { grid.setColumnsVisible(EK_YIL, this.checked); });

    // ---------------------------------------------------------------- faaliyet/proje ağacı (eski ddProg)

    var dlgFaaliyet = document.getElementById('dlgFaaliyet');
    var agac = null;
    var agacYil = 0;

    $('#btnFaaliyetSec').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        dlgFaaliyet.showModal();
        if (!agac) {
            // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
            agacYil = yil;
            agac = Sgb.agac('#agcFaaliyet', {
                yukle: function (ust) {
                    return Sgb.al('api/tavantertipsgb/faaliyetler', { yil: agacYil, ust: ust ? ust.kod : '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    dlgFaaliyet.close();
                    if (d.kod === faal) return;
                    if (acik && !degisiklikVarsaSor('Seçimi değiştirirseniz')) return;
                    listeyiBosalt();
                    faal = d.kod;
                    $('#txtFaaliyet').val(d.ad);
                    ekoFaalYukle();
                },
                seciliMi: function (d) { return d.kod === faal; },
                ilkAcik: true,
                bosMetin: 'Faaliyet/proje bulunamadı.',
                hata: hataGoster
            });
        } else if (agacYil !== yil) {
            agacYil = yil;
            agac.yenile();
        }
    });

    // Salt okunur kutuya tıklamak (veya Enter/boşluk) da ağacı açar.
    $('#txtFaaliyet').on('click', function () { $('#btnFaaliyetSec').trigger('click'); })
        .on('keydown', function (e) {
            if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); $('#btnFaaliyetSec').trigger('click'); }
        });

    // ---------------------------------------------------------------- işlemler

    var sor = TG.soruKur();

    function kaydet() {
        grid.stopEditing();
        var satirlar = [];
        grid.forEachNode(function (n) {
            var d = n.data;
            if (!tertipSatiri(d)) return;
            satirlar.push({ kod1: d.kod1, kod2: d.kod2, kod3: d.kod3, kod4: d.kod4, yil0: d.yil0, yil1: d.yil1, yil2: d.yil2,
                ekOdenek: d.ekOdenek, ekGerekce: d.ekGerekce || '', ekOdenek1: d.ekOdenek1, ekOdenek2: d.ekOdenek2 });
        });
        $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/tavantertipsgb/kaydet', { yil: acik.yil, kod: acik.kod, satirlar: satirlar })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (s.basarili) kirli = false;
            })
            .fail(hataGoster)
            .always(function () { durumGoster(); });
    }

    $('#btnKaydet').on('click', function () {
        if (!acik) return;
        grid.stopEditing();
        sor('Onay', 'Kaydetmek istiyor musunuz?', 'Kaydet', kaydet);
    });

    function onayKaydet(onayla) {
        Sgb.gonder('api/tavantertipsgb/onay', { yil: acik.yil, kod: acik.kod, onayla: onayla })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (s.basarili) { durum = s.durum; durumGoster(); }
            })
            .fail(hataGoster);
    }

    $('#btnOnayla').on('click', function () {
        if (!acik) return;
        if (kirli) { mesaj('Önce değişiklikleri kaydediniz.', 'warning'); return; }
        sor('Onay', 'Onayladıktan sonra faaliyet/proje için bütçe teklifi yapamazsınız. Devam etmek istiyor musunuz?', 'Onayla',
            function () { onayKaydet(true); });
    });

    $('#btnOnayKaldir').on('click', function () {
        if (!acik) return;
        sor('Onay', 'Faaliyet/proje için bütçe teklifi veri girişine açılacaktır. Devam etmek istiyor musunuz?', 'Onayı Kaldır',
            function () { onayKaydet(false); });
    });

    // Tekliften Al (eski TekliftenAl): girilmemiş (0) tutarlara teklif tutarları, boş gerekçeye teklif gerekçesi yazılır.
    $('#btnTekliftenAl').on('click', function () {
        if (!girisAcik()) return;
        var n = TG.tekliftenAl(grid, { yil0: 'teklif0', yil1: 'teklif1', yil2: 'teklif2', ekOdenek: 'ekOdenekTeklif',
            ekOdenek1: 'ekOdenek1Teklif', ekOdenek2: 'ekOdenek2Teklif' }, tertipSatiri);
        var gerekce = 0;
        grid.forEachNode(function (d) {
            if (!tertipSatiri(d.data) || d.data.ekGerekce || !d.data.ekGerekceTeklif) return;
            d.data.ekGerekce = String(d.data.ekGerekceTeklif).slice(0, 300);
            gerekce++;
        });
        if (!n.length && !gerekce) { mesaj('Tekliften alınacak değer yok (yalnızca boş tutarlar ve gerekçeler doldurulur).', 'info'); return; }
        if (gerekce) grid.refreshCells({ force: true });
        kirli = true;
        toplamHesapla();
        mesaj('Teklif değerleri alındı; kaydetmeyi unutmayınız.', 'info');
    });

    $('#btnKopyala').on('click', function () { TG.kopyala(grid); });

    TG.yapistirKur({
        grid: grid,
        alanlar: YAPISTIRILAN,
        satirUygun: tertipSatiri,
        acik: girisAcik,
        sonra: function () { kirli = true; toplamHesapla(); }
    });

    // Rapor ekrandaki seçimle alınır (eski btnRaporAl_Click: tavan seçilmemişse uyarır).
    var raporlariDoldur = TG.raporMenuKur('api/tavantertipsgb/rapor', function (rapor) {
        var o = { yil: yilOku(), kod: $('#ddlEkoFaal').val() || '', rapor: rapor };
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return null; }
        if (!o.kod) { mesaj('Lütfen seçim yapınız!', 'warning'); return null; }
        return o;
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış

    durumGoster();
    ekoFaalDoldur([]);
    Sgb.al('api/tavantertipsgb/tanim')
        .done(function (t) {
            if (t.yil) $('#txtYil').val(t.yil);
            raporlariDoldur(t.raporlar);
            grid.setGridOption('columnDefs', sutunlar(t.yil || new Date().getFullYear()));
        })
        .fail(hataGoster);
});
