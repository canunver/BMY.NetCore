// Birim Görüşme Ekranı (Yatırım / Cari) (ButceHazirlikv2/TavanYatirimProje?tur=2 / ?tur=1): birim ve onun onaylı ekonomik kod + sektör
// tavanı (tur 2: yatırım, tur 1: cari) seçilir; tavanın faaliyet/projelere üç yıllık görüşme tutarları ve ek ödenekleri girilir.
// Projeler proje grubuna (kod5) göre ara toplamlıdır. En üstteki TAVAN, FARK (tavan - toplam) ve TOPLAM satırları salt okunurdur;
// bir tutar değişince toplamlar yeniden hesaplanır. Onaylanmış (durum 5/6) tutarlar değiştirilemez. Raporlar sunucuda eski kodla üretilir.
$(function () {
    'use strict';

    var $sayfa = $('.tavan-yatirim');
    if (!$('#grdTavan').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TG = window.TavanGorusme, mesaj = TG.mesaj, hataGoster = TG.hataGoster;
    var TUR = parseInt($sayfa.attr('data-tur'), 10) === 2 ? 2 : 1;
    var SAYILAR = ['harcama_2', 'odenek_1', 'teklif0', 'ekOdenekTeklif', 'yil0', 'ekOdenek', 'talep_0', 'teklif1', 'ekOdenek1Teklif', 'yil1', 'ekOdenek1',
        'teklif2', 'ekOdenek2Teklif', 'yil2', 'ekOdenek2', 'sgb0', 'sgb1', 'sgb2', 'ekSgb', 'ekSgb1', 'ekSgb2'];
    var GIRILEN = ['yil0', 'ekOdenek', 'yil1', 'ekOdenek1', 'yil2', 'ekOdenek2'];
    var sistemYoneticisi = false;
    var acik = null;     // listelenmiş ölçüt { tur, yil, eko, sektor, birim }: Kaydet / Onay bunu kullanır
    var tavan = null;    // listelenen ekonomik kod + sektör + birim tavanı (TAVAN satırı)
    var durum = -1;      // onay durumu: 0/1 açık, 5 onaylı (faaliyet girişine açık), 6 onaylı (kapalı), -1 okunamadı
    var kirli = false;   // kaydedilmemiş değişiklik var mı

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function onayli() { return durum > 1; }
    function girisAcik() { return !!acik && durum >= 0 && !onayli(); }
    function projeSatiri(d) { return !!d && !d.ara && String(d.kod4 || '').length > 2; }
    function kacir(metin) { return $('<span></span>').text(metin == null ? '' : metin).html(); }

    function birimleriDoldur(liste) {
        var $ddl = $('#ddlBirim'), onceki = $ddl.val();
        $ddl.empty();
        $('<option></option>').val('').text('Birim seçiniz').appendTo($ddl);
        $.each(liste || [], function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($ddl); });
        if (onceki) $ddl.val(onceki);
        if ($ddl.val() === null) $ddl.val('');
    }

    // Eko. Kod / Sektör seçenekleri: değer "eko@sektör@birim" (eski tamKod).
    function ekoSektorDoldur(liste) {
        var $ddl = $('#ddlEkoSektor');
        $ddl.empty();
        $('<option></option>').val('').text(liste && liste.length ? 'Seçiniz' : '(tavan yok)').appendTo($ddl);
        $.each(liste || [], function (i, k) { $('<option></option>').val([k.eko, k.sektor, k.birim].join('@')).text(k.ad).appendTo($ddl); });
    }

    function secim() {
        var p = ($('#ddlEkoSektor').val() || '').split('@');
        return { tur: TUR, yil: yilOku(), eko: p[0] || '', sektor: p[1] || '', birim: p.length > 2 ? p[2] : ($('#ddlBirim').val() || '') };
    }

    // ---------------------------------------------------------------- tablo

    // Yalnızca faaliyet/proje satırları, onaysızken girilir (eski BeforeEdit).
    function girilebilir(p) {
        return girisAcik() && !p.node.rowPinned && GIRILEN.indexOf(p.colDef.field) >= 0 && projeSatiri(p.data);
    }

    function hucreSinifi(p) {
        var s = [];
        if (p.colDef.type === 'sayi') s.push('sayi');
        if (p.colDef.type === 'orta') s.push('orta');
        if (p.colDef.colId === 'proje') s.push('tertip');
        if (!p.node.rowPinned) {
            if (projeSatiri(p.data)) s.push(girilebilir(p) ? 'girilir' : 'kapali');
        } else if (p.data.kod4 === '--' && typeof p.value === 'number' && p.value < 0) s.push('eksi');
        return s;
    }

    // Faaliyet/proje: proje satırında kod ve altında ad; ara toplamda proje grubu kodu ve adı (eski ColorRendererKodAd).
    function projeCiz(p) {
        if (!p.data) return '';
        if (p.node.rowPinned) return kacir(p.data.ad4);
        if (p.data.ara) return '<span class="kod">' + kacir(p.data.kod5) + '</span>' + kacir(p.data.ad5);
        return '<span class="kod">' + kacir(p.data.kod4) + '</span>' + kacir(p.data.ad4);
    }

    function projeMetin(p) {
        if (!p.data) return '';
        if (p.node.rowPinned) return p.data.ad4;
        return p.data.ara ? (p.data.kod5 || '') + ' ' + (p.data.ad5 || '') : (p.data.kod4 || '') + ' ' + (p.data.ad4 || '');
    }

    function sutunlar(yil) {
        var giris = TG.giris(girilebilir);
        function s(alan, baslik, ek) { return TG.sayiSutunu(alan, baslik, hucreSinifi, ek); }
        return [
            { colId: 'proje', headerName: 'Proje Adı / Faaliyet', width: 280, pinned: 'left', sortable: false, cellRenderer: projeCiz, valueGetter: projeMetin,
                tooltipValueGetter: projeMetin, cellClass: hucreSinifi },
            s('harcama_2', (yil - 2) + ' Harcama'),
            s('odenek_1', (yil - 1) + ' K.B.Ö'),
            s('teklif0', yil + ' Teklif'),
            s('ekOdenekTeklif', yil + ' Ek Teklif'),
            s('yil0', yil + ' Görüşme', giris),
            s('ekOdenek', yil + ' Ek Görüşme', giris),
            s('talep_0', yil + ' Taşra Teklifi'),
            s('teklif1', (yil + 1) + ' Teklif'),
            s('ekOdenek1Teklif', (yil + 1) + ' Ek Teklif'),
            s('yil1', (yil + 1) + ' Görüşme', giris),
            s('ekOdenek1', (yil + 1) + ' Ek Görüşme', giris),
            s('teklif2', (yil + 2) + ' Teklif'),
            s('ekOdenek2Teklif', (yil + 2) + ' Ek Teklif'),
            s('yil2', (yil + 2) + ' Görüşme', giris),
            s('ekOdenek2', (yil + 2) + ' Ek Görüşme', giris),
            { field: 'projeBitisYili', headerName: 'Bitiş Yılı', type: 'orta', width: 80, sortable: false, cellClass: hucreSinifi,
                valueFormatter: function (p) { return p.value ? String(p.value) : ''; } },
            { field: 'projeDevYeni', headerName: 'Devam / Yeni', type: 'orta', width: 90, sortable: false, cellClass: hucreSinifi },
            s('sgb0', yil + ' Kontrol SGB', { width: 115 }),
            s('sgb1', (yil + 1) + ' Kontrol SGB', { width: 115 }),
            s('sgb2', (yil + 2) + ' Kontrol SGB', { width: 115 }),
            s('ekSgb', yil + ' Kontrol SGB Ek', { width: 120 }),
            s('ekSgb1', (yil + 1) + ' Kontrol SGB Ek', { width: 120 }),
            s('ekSgb2', (yil + 2) + ' Kontrol SGB Ek', { width: 120 })
        ];
    }

    var grid = Sgb.grid('#grdTavan', {
        columnDefs: sutunlar(new Date().getFullYear()),
        // Hücreye tıklayınca düzenlenir; Tab / Enter ile sonraki hücreye geçilir.
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        tooltipShowDelay: 500,
        getRowId: function (p) { return p.data.id; },
        getRowHeight: function (p) { return p.node.rowPinned ? 30 : 40; },
        getRowClass: function (p) {
            if (p.node.rowPinned) return p.data.kod4 === '++' ? 'satir-tavan' : '';
            return p.data.ara ? 'satir-toplam' : '';
        },
        overlayNoRowsTemplate: '<span>Birim ve ekonomik kod / sektör seçip Listele\'ye basınız.</span>',
        onCellValueChanged: function (e) {
            if (e.node.rowPinned || e.newValue === e.oldValue) return;
            kirli = true;
            toplamHesapla();
        }
    });

    // Üstteki satırlar (eski "T A V A N", "F A R K" ve sütun başlığındaki "T O P L A M"); ara toplamlar da yenilenir.
    function toplamHesapla() {
        if (!tavan) { grid.setGridOption('pinnedTopRowData', []); return; }
        var t = $.extend(TG.toplamlariGuncelle(grid, SAYILAR), { kod4: '00', ad4: 'T O P L A M' });
        var f = { id: '--', kod4: '--', ad4: 'F A R K' };
        $.each(SAYILAR, function (i, a) { f[a] = Math.round(((Number(tavan[a]) || 0) - t[a]) * 100) / 100; });
        grid.setGridOption('pinnedTopRowData', [$.extend({}, tavan, { id: '++', kod4: '++', ad4: 'T A V A N', projeBitisYili: null, projeDevYeni: '' }), f, t]);
    }

    var DURUM_AD = { 0: 'Açık', 1: 'Açık', 5: 'Onaylı (Faaliyet Girişine Açık)', 6: 'Onaylı (Faaliyet Girişine Kapalı)' };

    // Tuşlar onay durumuna göre (eski TuslariAyarla): açık -> Kaydet, Onayla; 5 -> Kapat, Onay Kaldır; 6 -> Girişe Aç (sistem yöneticisi).
    function durumGoster() {
        var $e = $('#durumEtiket');
        var var_ = !!acik && durum >= 0;
        if (!var_) $e.prop('hidden', true);
        else $e.prop('hidden', false).toggleClass('label-success', onayli()).toggleClass('label-default', !onayli()).text(DURUM_AD[durum] || ('Durum ' + durum));
        $('#btnKaydet, #btnOnayla, #btnTekliftenAl, #btnYapistir').prop('hidden', !var_ || onayli());
        $('#btnKaydet, #btnTekliftenAl, #btnYapistir').prop('disabled', !grid.getDisplayedRowCount());
        $('#btnKapat, #btnOnayKaldir').prop('hidden', !var_ || durum !== 5);
        $('#btnGirisAc').prop('hidden', !var_ || durum !== 6 || !sistemYoneticisi);
        $('#btnKopyala').prop('disabled', !acik);
        grid.refreshCells({ force: true });
    }

    function degisiklikVarsaSor(neden) {
        return !kirli || window.confirm('Kaydedilmemiş değişiklikler var. ' + (neden || 'Devam ederseniz') + ' kaybolacak. Devam edilsin mi?');
    }

    function listele() {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!o.eko || !o.sektor || !o.birim) { mesaj('Lütfen seçim yapınız!', 'warning'); return; }
        if (!degisiklikVarsaSor()) return;
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/tavanyatirim/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = o;
                tavan = v.tavan;
                durum = v.durum;
                kirli = false;
                grid.setGridOption('columnDefs', sutunlar(o.yil));
                grid.setGridOption('overlayNoRowsTemplate', '<span>Faaliyet / proje bulunamadı.</span>');
                grid.setGridOption('rowData', TG.araToplamla(v.satirlar, SAYILAR));
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
        kirli = false;
        grid.setGridOption('rowData', []);
        grid.setGridOption('pinnedTopRowData', []);
        durumGoster();
    }

    function ekoSektorYukle() {
        var yil = yilOku(), birim = $('#ddlBirim').val() || '';
        ekoSektorDoldur([]);
        if (yil < 2000 || !birim) return;
        Sgb.al('api/tavanyatirim/ekosektor', { tur: TUR, yil: yil, birim: birim })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                ekoSektorDoldur(v.kodlar);
            })
            .fail(hataGoster);
    }

    function acikSecimiGeriAl() {
        $('#txtYil').val(acik.yil);
        $('#ddlBirim').val(acik.birim);
        $('#ddlEkoSektor').val([acik.eko, acik.sektor, acik.birim].join('@'));
    }

    $('#ddlBirim').on('change', function () {
        if (acik && !degisiklikVarsaSor('Seçimi değiştirirseniz')) { acikSecimiGeriAl(); return; }
        listeyiBosalt();
        ekoSektorYukle();
    });

    $('#ddlEkoSektor').on('change', function () {
        if (acik && !degisiklikVarsaSor('Seçimi değiştirirseniz')) { acikSecimiGeriAl(); return; }
        listeyiBosalt();
    });

    // Birimler yıla bağlıdır: yıl değişince yeniden okunur (eski YilDegisti), eko-sektör boşalır.
    $('#txtYil').on('change', function () {
        if (acik && !degisiklikVarsaSor('Yılı değiştirirseniz')) { this.value = acik.yil; return; }
        listeyiBosalt();
        var yil = yilOku();
        grid.setGridOption('columnDefs', sutunlar(yil || new Date().getFullYear()));
        ekoSektorDoldur([]);
        if (yil < 2000) return;
        Sgb.al('api/tavanyatirim/birimler', { tur: TUR, yil: yil })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                birimleriDoldur(v.birimler);
                ekoSektorYukle();
            })
            .fail(hataGoster);
    });

    // ---------------------------------------------------------------- işlemler

    var sor = TG.soruKur();

    function kaydet() {
        grid.stopEditing();
        var satirlar = [];
        grid.forEachNode(function (n) {
            var d = n.data;
            if (!projeSatiri(d)) return;
            satirlar.push({ kod1: d.kod1, kod2: d.kod2, kod3: d.kod3, kod4: d.kod4, kod6: d.kod6,
                yil0: d.yil0, yil1: d.yil1, yil2: d.yil2, ekOdenek: d.ekOdenek, ekOdenek1: d.ekOdenek1, ekOdenek2: d.ekOdenek2 });
        });
        $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/tavanyatirim/kaydet', { tur: TUR, yil: acik.yil, eko: acik.eko, sektor: acik.sektor, birim: acik.birim, satirlar: satirlar })
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

    var ONAY_SORU = {
        onayla: ['Onayladıktan sonra faaliyet/proje için bütçe teklifi yapamazsınız. Devam etmek istiyor musunuz?', 'Onayla'],
        kapat: ['Giriş işlemi tamamlandı yapılınca, faaliyet/proje bütçe rakamlarını değiştiremezsiniz. Devam etmek istiyor musunuz?', 'Girişe Kapat'],
        kaldir: ['Faaliyet/proje için bütçe teklifi veri girişine açılacaktır. Devam etmek istiyor musunuz?', 'Onayı Kaldır'],
        ac: ['Faaliyet girişi veri girişine açılacaktır. Devam etmek istiyor musunuz?', 'Girişe Aç']
    };

    $sayfa.on('click', '[data-islem]', function () {
        if (!acik) return;
        var islem = $(this).attr('data-islem');
        if (islem === 'onayla' && kirli) { mesaj('Önce değişiklikleri kaydediniz.', 'warning'); return; }
        sor('Onay', ONAY_SORU[islem][0], ONAY_SORU[islem][1], function () {
            Sgb.gonder('api/tavanyatirim/onay', { tur: TUR, yil: acik.yil, eko: acik.eko, sektor: acik.sektor, birim: acik.birim, islem: islem })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) { durum = s.durum; durumGoster(); }
                })
                .fail(hataGoster);
        });
    });

    $('#btnTekliftenAl').on('click', function () {
        if (!girisAcik()) return;
        var n = TG.tekliftenAl(grid, { yil0: 'teklif0', yil1: 'teklif1', yil2: 'teklif2', ekOdenek: 'ekOdenekTeklif', ekOdenek1: 'ekOdenek1Teklif', ekOdenek2: 'ekOdenek2Teklif' }, projeSatiri);
        if (!n.length) { mesaj('Tekliften alınacak tutar yok (yalnızca boş tutarlar doldurulur).', 'info'); return; }
        kirli = true;
        toplamHesapla();
        mesaj(n.length + ' satıra teklif tutarları alındı; kaydetmeyi unutmayınız.', 'info');
    });

    $('#btnKopyala').on('click', function () { TG.kopyala(grid); });

    TG.yapistirKur({
        grid: grid,
        alanlar: GIRILEN,
        satirUygun: projeSatiri,
        acik: girisAcik,
        sonra: function () { kirli = true; toplamHesapla(); }
    });

    // Rapor ekrandaki seçimle alınır (eski btnRaporAl_Click: tavan seçilmemişse uyarır; rapor birim için alınır).
    var raporlariDoldur = TG.raporMenuKur('api/tavanyatirim/rapor', function (rapor) {
        var o = $.extend(secim(), { rapor: rapor });
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return null; }
        if (!o.eko || !o.sektor || !o.birim) { mesaj('Lütfen seçim yapınız!', 'warning'); return null; }
        return o;
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış

    durumGoster();
    ekoSektorDoldur([]);
    Sgb.al('api/tavanyatirim/tanim', { tur: TUR })
        .done(function (t) {
            sistemYoneticisi = !!t.sistemYoneticisi;
            if (t.yil) $('#txtYil').val(t.yil);
            raporlariDoldur(t.raporlar);
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            birimleriDoldur(t.birimler);
            grid.setGridOption('columnDefs', sutunlar(t.yil));
        })
        .fail(hataGoster);
});
