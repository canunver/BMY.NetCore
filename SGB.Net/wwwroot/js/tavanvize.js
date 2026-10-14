// Yatırım Projeleri Vize Tavanı Girişi (ButceHazirlikv2/TavanYatirimVize): birim, ekonomik kod ve finansman (birinci düzey) ile isteğe
// bağlı proje seçilir; faaliyet/projelerin üç yıllık vize tavanları girilir. Projeler proje grubuna (kod5) göre ara toplamlıdır, en
// üstteki TOPLAM satırı salt okunurdur. Onaylanmış (durum 5/6) tavanlar değiştirilemez. Toplu Kapatma yılın bütün vize tavanlarını
// girişe kapatır. Yazdır / Detay Yaz ve Rapor Al sunucuda eski kodla üretilir.
$(function () {
    'use strict';

    var $sayfa = $('.tavan-vize');
    if (!$('#grdTavan').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TG = window.TavanGorusme, mesaj = TG.mesaj, hataGoster = TG.hataGoster;
    var SAYILAR = ['harcama_2', 'odenek_1', 'teklif0', 'yil0', 'teklif1', 'yil1', 'teklif2', 'yil2', 'sgb0', 'sgb1', 'sgb2'];
    var GIRILEN = ['yil0', 'yil1', 'yil2'];
    var sistemYoneticisi = false;
    var acik = null;     // listelenmiş ölçüt { yil, birim, eko, fin, proje }: Kaydet / Onay bunu kullanır
    var durum = -1;      // onay durumu: 0/1 açık, 5 onaylı, 6 girişe kapalı, -1 okunamadı
    var kirli = false;   // kaydedilmemiş değişiklik var mı

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function onayli() { return durum > 1; }
    function girisAcik() { return !!acik && durum >= 0 && !onayli(); }
    function projeSatiri(d) { return !!d && !d.ara && String(d.kod4 || '').length > 2; }
    function kacir(metin) { return $('<span></span>').text(metin == null ? '' : metin).html(); }

    function secenekDoldur($ddl, liste, bos) {
        var onceki = $ddl.val();
        $ddl.empty();
        if (bos !== null) $('<option></option>').val('').text(bos).appendTo($ddl);
        $.each(liste || [], function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($ddl); });
        if (onceki) $ddl.val(onceki);
        if ($ddl.val() === null) $ddl.prop('selectedIndex', 0);
    }

    function kodlariDoldur(k) {
        k = k || {};
        secenekDoldur($('#ddlBirim'), k.birimler, 'Birim seçiniz');
        secenekDoldur($('#ddlEko'), k.ekolar, 'Ekonomik kod seçiniz');
        secenekDoldur($('#ddlFin'), k.finler, 'Finansman seçiniz');
    }

    function secim() {
        return { yil: yilOku(), birim: $('#ddlBirim').val() || '', eko: $('#ddlEko').val() || '', fin: $('#ddlFin').val() || '',
            proje: parseInt($('#ddlProje').val(), 10) || -1 };
    }

    // ---------------------------------------------------------------- tablo

    function girilebilir(p) {
        return girisAcik() && !p.node.rowPinned && GIRILEN.indexOf(p.colDef.field) >= 0 && projeSatiri(p.data);
    }

    function hucreSinifi(p) {
        var s = [];
        if (p.colDef.type === 'sayi') s.push('sayi');
        if (p.colDef.type === 'orta') s.push('orta');
        if (p.colDef.colId === 'proje') s.push('tertip');
        if (!p.node.rowPinned && projeSatiri(p.data)) s.push(girilebilir(p) ? 'girilir' : 'kapali');
        return s;
    }

    // Faaliyet/proje: proje satırında kod ve altında ad; ara toplamda proje kodu ve adı (eski ColorRendererKodAd).
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
            s('yil0', yil + ' Vize Tavanı', giris),
            s('teklif1', (yil + 1) + ' Teklif'),
            s('yil1', (yil + 1) + ' Vize Tavanı', giris),
            s('teklif2', (yil + 2) + ' Teklif'),
            s('yil2', (yil + 2) + ' Vize Tavanı', giris),
            s('sgb0', yil + ' Kontrol SGB', { width: 115 }),
            s('sgb1', (yil + 1) + ' Kontrol SGB', { width: 115 }),
            s('sgb2', (yil + 2) + ' Kontrol SGB', { width: 115 }),
            { field: 'projeBitisYili', headerName: 'Bitiş Yılı', type: 'orta', width: 80, sortable: false, cellClass: hucreSinifi,
                valueFormatter: function (p) { return p.value ? String(p.value) : ''; } },
            { field: 'projeDevYeni', headerName: 'Devam / Yeni', type: 'orta', width: 90, sortable: false, cellClass: hucreSinifi }
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
        getRowClass: function (p) { return !p.node.rowPinned && p.data.ara ? 'satir-toplam' : ''; },
        overlayNoRowsTemplate: '<span>Birim, ekonomik kod ve finansman seçip Listele\'ye basınız.</span>',
        onCellValueChanged: function (e) {
            if (e.node.rowPinned || e.newValue === e.oldValue) return;
            kirli = true;
            toplamHesapla();
        }
    });

    function toplamHesapla() {
        if (!acik) { grid.setGridOption('pinnedTopRowData', []); return; }
        grid.setGridOption('pinnedTopRowData', [$.extend(TG.toplamlariGuncelle(grid, SAYILAR), { kod4: '00', ad4: 'T O P L A M' })]);
    }

    var DURUM_AD = { 0: 'Açık', 1: 'Açık', 5: 'Onaylı', 6: 'Onaylı (Girişe Kapalı)' };

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

    function listele(o) {
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!o.birim || !o.eko || !o.fin) { mesaj('Lütfen seçim yapınız!', 'warning'); return; }
        if (!degisiklikVarsaSor()) return;
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/tavanvize/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = o;
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

    $('#frmSecim').on('submit', function (olay) { olay.preventDefault(); listele(secim()); });

    // Seçim değişince eski liste kaydedilemez (Kaydet / Onay listelenen seçime yazar).
    function listeyiBosalt() {
        if (!acik) return;
        acik = null;
        durum = -1;
        kirli = false;
        grid.setGridOption('rowData', []);
        grid.setGridOption('pinnedTopRowData', []);
        durumGoster();
    }

    function acikSecimiGeriAl() {
        $('#txtYil').val(acik.yil);
        $('#ddlBirim').val(acik.birim);
        $('#ddlEko').val(acik.eko);
        $('#ddlFin').val(acik.fin);
        $('#ddlProje').val(String(acik.proje));
    }

    // Birimin projeleri (eski ddProg_Changed), başta "Hepsi".
    function projeleriYukle() {
        var yil = yilOku(), birim = $('#ddlBirim').val() || '';
        secenekDoldur($('#ddlProje'), [{ kod: -1, ad: 'Hepsi' }], null);
        if (yil < 2000 || !birim) return;
        Sgb.al('api/tavanvize/projeler', { yil: yil, birim: birim })
            .done(function (v) {
                if (v.hata) mesaj(v.hata, 'danger');
                if (v.projeler) secenekDoldur($('#ddlProje'), v.projeler, null);
            })
            .fail(hataGoster);
    }

    $('#ddlBirim').on('change', function () {
        if (acik && !degisiklikVarsaSor('Seçimi değiştirirseniz')) { acikSecimiGeriAl(); return; }
        listeyiBosalt();
        $('#ddlProje').val('-1');
        projeleriYukle();
    });

    $('#ddlEko, #ddlFin, #ddlProje').on('change', function () {
        if (acik && !degisiklikVarsaSor('Seçimi değiştirirseniz')) { acikSecimiGeriAl(); return; }
        listeyiBosalt();
    });

    // Kodlar yıla bağlıdır: yıl değişince yeniden okunur (eski YilDegisti).
    $('#txtYil').on('change', function () {
        if (acik && !degisiklikVarsaSor('Yılı değiştirirseniz')) { this.value = acik.yil; return; }
        listeyiBosalt();
        var yil = yilOku();
        grid.setGridOption('columnDefs', sutunlar(yil || new Date().getFullYear()));
        if (yil < 2000) return;
        Sgb.al('api/tavanvize/kodlar', { yil: yil })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                kodlariDoldur(v.kodlar);
                projeleriYukle();
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
            satirlar.push({ kod1: d.kod1, kod2: d.kod2, kod3: d.kod3, kod4: d.kod4, yil0: d.yil0, yil1: d.yil1, yil2: d.yil2 });
        });
        $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/tavanvize/kaydet', { yil: acik.yil, birim: acik.birim, eko: acik.eko, fin: acik.fin, satirlar: satirlar })
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
            Sgb.gonder('api/tavanvize/onay', { yil: acik.yil, birim: acik.birim, eko: acik.eko, fin: acik.fin, islem: islem })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) { durum = s.durum; durumGoster(); }
                })
                .fail(hataGoster);
        });
    });

    // Toplu Kapatma (eski btnTopluKapat_Click): yılın kapalı olmayan bütün vize tavanları girişe kapatılır.
    $('#btnTopluKapat').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $b = $(this);
        sor('Onay', yil + ' yılının bütün vize tavanları girişe kapatılacaktır. Giriş işlemi tamamlandı yapılınca, faaliyet/proje bütçe rakamlarını değiştiremezsiniz. Devam etmek istiyor musunuz?', 'Toplu Kapat', function () {
            $b.prop('disabled', true);
            Sgb.gonder('api/tavanvize/toplu-kapat', { yil: yil })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    // Listelenen tavanın durumu değişmiş olabilir: kaydedilmemiş değişiklik yoksa yeniden okunur.
                    if (acik && acik.yil === yil && !kirli) listele(acik);
                })
                .fail(hataGoster)
                .always(function () { $b.prop('disabled', false); });
        });
    });

    $('#btnTekliftenAl').on('click', function () {
        if (!girisAcik()) return;
        var n = TG.tekliftenAl(grid, { yil0: 'teklif0', yil1: 'teklif1', yil2: 'teklif2' }, projeSatiri);
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

    // Yazdır / Detay Yaz: ekrandaki seçimle (eski sayfadaki gibi; seçilmeyen kodlar süzülmez).
    function yazdir(adres, $b) {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        $b.prop('disabled', true);
        Sgb.indir(adres, { yil: o.yil, birim: o.birim, eko: o.eko, fin: o.fin }).then(function () { $b.prop('disabled', false); });
    }

    $('#btnYazdir').on('click', function () { yazdir('api/tavanvize/yazdir', $(this)); });
    $('#btnYazdirDetay').on('click', function () { yazdir('api/tavanvize/yazdir-detay', $(this)); });

    var raporlariDoldur = TG.raporMenuKur('api/tavanvize/rapor', function (rapor) {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return null; }
        return { yil: o.yil, birim: o.birim, eko: o.eko, fin: o.fin, rapor: rapor };
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış

    durumGoster();
    kodlariDoldur(null);
    secenekDoldur($('#ddlProje'), [{ kod: -1, ad: 'Hepsi' }], null);
    Sgb.al('api/tavanvize/tanim')
        .done(function (t) {
            sistemYoneticisi = !!t.sistemYoneticisi;
            if (t.yil) $('#txtYil').val(t.yil);
            raporlariDoldur(t.raporlar);
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            kodlariDoldur(t.kodlar);
            grid.setGridOption('columnDefs', sutunlar(t.yil));
        })
        .fail(hataGoster);
});
