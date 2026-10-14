// Birim Görüşme Ekranı (ButceHazirlikv2/GorusmeCari): faaliyet/proje ve cari ekonomik kod ağaçtan seçilir, kurumun bu koda uyan
// tertipleri (ekonomik kod 2. düzeyde) listelenir ve SGB görüşme tutarları girilir. En üstteki TOPLAM satırı salt okunurdur.
// Adresle açılışta (?yil=&tertip=, başka sayfadan) seçim bölümü gizlenir, verilen tertip okunur ve yalnızca Kaydet kalır.
// Onay işlemleri seçili faaliyet/projenin kaydına yazılır; onaylı faaliyetin tutarları değiştirilemez. Raporlar eski kodla üretilir.
$(function () {
    'use strict';

    if (!$('#grdTertip').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TG = window.TavanGorusme, mesaj = TG.mesaj, hataGoster = TG.hataGoster;
    var SAYILAR = ['harcama_2', 'odenek_1', 'talep_0', 'teklif0', 'ekOdenekTeklif', 'yil0', 'ekOdenek', 'teklif1', 'yil1', 'teklif2', 'yil2', 'sgb0', 'sgb1', 'sgb2', 'ekSgb'];
    var GIRILEN = ['yil0', 'ekOdenek', 'yil1', 'yil2'];

    var adres = new URLSearchParams(window.location.search);
    var adresYil = parseInt(adres.get('yil'), 10) || 0;
    var adresTertip = (adres.get('tertip') || '').trim();
    var adresle = adresYil > 0 && adresTertip !== '';

    var faal = { kod: '', ad: '' };   // seçili faaliyet/proje
    var eko = { kod: '', ad: '' };    // seçili cari ekonomik kod
    var acik = null;     // listelenmiş ölçüt { yil, faal, eko } ya da { yil, tertip }: Kaydet / Onay bunu kullanır
    var eko2 = true;     // ekonomik kod 2. düzeyde mi (adresle açılışta tam kod)
    var durum = -1;      // faaliyetin görüşme onay durumu: 0/1 açık, 5 onaylı, 6 kapalı, -1 okunamadı
    var degisen = {};    // kaydedilmemiş satırlar (id -> true); eski sayfa yalnızca değişen satırları gönderiyordu

    function kirli() { return !$.isEmptyObject(degisen); }
    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function onayli() { return durum > 1; }
    function girisAcik() { return !!acik && durum >= 0 && !onayli(); }
    function tertipSatiri(d) { return !!d && String(d.kod4 || '').length > 2; }
    function kacir(metin) { return $('<span></span>').text(metin == null ? '' : metin).html(); }

    // ---------------------------------------------------------------- tablo

    function girilebilir(p) {
        return girisAcik() && !p.node.rowPinned && GIRILEN.indexOf(p.colDef.field) >= 0 && tertipSatiri(p.data);
    }

    function hucreSinifi(p) {
        var s = [];
        if (p.colDef.type === 'sayi') s.push('sayi');
        if (p.colDef.field === 'ekGerekceTeklif') s.push('metin');
        if (p.colDef.colId === 'tertip') s.push('tertip');
        if (!p.node.rowPinned && tertipSatiri(p.data)) s.push(girilebilir(p) ? 'girilir' : 'kapali');
        return s;
    }

    function tertipKod(d) { return [d.kod1, d.kod2, d.kod3, d.kod4, d.kod5].join('-'); }

    // Tertip: "kurumsal-fonksiyonel-finansman-ekonomik-faaliyet" kodu ve altında ad (eski ColorRendererKodAd).
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
        function s(alan, baslik, ek) { return TG.sayiSutunu(alan, baslik, hucreSinifi, ek); }
        return [
            { colId: 'tertip', headerName: 'Tertip', width: 300, pinned: 'left', sortable: false, cellRenderer: tertipCiz, valueGetter: tertipMetin,
                tooltipValueGetter: tertipMetin, cellClass: hucreSinifi },
            s('harcama_2', (yil - 2) + ' Harcama'),
            s('odenek_1', (yil - 1) + ' K.B.Ö', { hide: !eko2 }),
            s('talep_0', yil + ' Taşra Teklifi'),
            s('teklif0', yil + ' Teklif'),
            s('ekOdenekTeklif', yil + ' Ek Teklif'),
            { field: 'ekGerekceTeklif', headerName: 'Açıklama', width: 260, sortable: false, tooltipField: 'ekGerekceTeklif', cellClass: hucreSinifi },
            s('yil0', yil + ' SGB Gör.', giris),
            s('ekOdenek', yil + ' Ek SGB Gör.', giris),
            s('teklif1', (yil + 1) + ' Teklif'),
            s('yil1', (yil + 1) + ' SGB Gör.', giris),
            s('teklif2', (yil + 2) + ' Teklif'),
            s('yil2', (yil + 2) + ' SGB Gör.', giris),
            s('sgb0', yil + ' Kontrol SGB Gör.', { width: 120 }),
            s('sgb1', (yil + 1) + ' Kontrol SGB Gör.', { width: 120 }),
            s('sgb2', (yil + 2) + ' Kontrol SGB Gör.', { width: 120 }),
            s('ekSgb', yil + ' Kontrol Ek SGB Gör.', { width: 130 })
        ];
    }

    var grid = Sgb.grid('#grdTertip', {
        columnDefs: sutunlar(adresYil || new Date().getFullYear()),
        // Hücreye tıklayınca düzenlenir; Tab / Enter ile sonraki hücreye geçilir.
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        tooltipShowDelay: 500,
        getRowId: function (p) { return p.data.id; },
        getRowHeight: function (p) { return !p.node.rowPinned && tertipSatiri(p.data) ? 40 : 30; },
        getRowClass: function (p) { return p.node.rowPinned || tertipSatiri(p.data) ? '' : 'satir-toplam'; },
        overlayNoRowsTemplate: '<span>Faaliyet/proje ve cari ekonomik kod seçip Listele\'ye basınız.</span>',
        onCellValueChanged: function (e) {
            if (e.node.rowPinned || e.newValue === e.oldValue) return;
            degisen[e.data.id] = true;
            toplamHesapla();
        }
    });

    function toplamHesapla() {
        if (!acik) { grid.setGridOption('pinnedTopRowData', []); return; }
        var t = { id: '00', kod4: '00', ad4: 'T O P L A M' };
        $.each(SAYILAR, function (i, a) { t[a] = 0; });
        grid.forEachNode(function (n) { $.each(SAYILAR, function (i, a) { t[a] += Number(n.data[a]) || 0; }); });
        grid.setGridOption('pinnedTopRowData', [t]);
    }

    var DURUM_AD = { 0: 'Açık', 1: 'Açık', 5: 'Onaylı (SGB Detay Girişi Açık)', 6: 'Kapalı' };

    // Tuşlar: Kaydet açıkken; Onayla açıkken, Kapat kapalı değilken, Onay Kaldır onaylıyken (faaliyet seçilmişse).
    function durumGoster() {
        var var_ = !!acik && durum >= 0;
        var onayVar = var_ && !adresle && !!acik.faal;
        var $e = $('#durumEtiket');
        if (!onayVar) $e.prop('hidden', true);
        else $e.prop('hidden', false).toggleClass('label-success', onayli()).toggleClass('label-default', !onayli()).text(DURUM_AD[durum] || ('Durum ' + durum));
        $('#btnKaydet, #btnTekliftenAl, #btnYapistir').prop('hidden', !var_ || onayli());
        $('#btnKaydet, #btnTekliftenAl, #btnYapistir').prop('disabled', !grid.getDisplayedRowCount());
        $('#btnOnayla').prop('hidden', !onayVar || onayli());
        $('#btnKapat').prop('hidden', !onayVar || durum === 6);
        $('#btnOnayKaldir').prop('hidden', !onayVar || !onayli());
        $('#btnKopyala').prop('disabled', !acik);
        grid.refreshCells({ force: true });
    }

    function degisiklikVarsaSor(neden) {
        return !kirli() || window.confirm('Kaydedilmemiş değişiklikler var. ' + (neden || 'Devam ederseniz') + ' kaybolacak. Devam edilsin mi?');
    }

    function listele(o) {
        if (!degisiklikVarsaSor()) return;
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/gorusmecari/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = o;
                eko2 = v.eko2 !== false;
                durum = v.durum;
                degisen = {};
                grid.setGridOption('columnDefs', sutunlar(o.yil));
                grid.setGridOption('overlayNoRowsTemplate', '<span>Tertip bulunamadı.</span>');
                grid.setGridOption('rowData', v.satirlar);
                toplamHesapla();
                durumGoster();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmSecim').on('submit', function (olay) {
        olay.preventDefault();
        var o = { yil: yilOku(), faal: faal.kod, eko: eko.kod };
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!o.eko) { mesaj('Lütfen ekonomik kod olarak cari ekonomik kod seçiniz!', 'warning'); return; }
        listele(o);
    });

    // Seçim değişince eski liste kaydedilemez (Kaydet / Onay listelenen seçime yazar).
    function listeyiBosalt() {
        if (!acik) return;
        acik = null;
        durum = -1;
        degisen = {};
        grid.setGridOption('rowData', []);
        grid.setGridOption('pinnedTopRowData', []);
        durumGoster();
    }

    $('#txtYil').on('change', function () {
        if (acik && !degisiklikVarsaSor('Yılı değiştirirseniz')) { this.value = acik.yil; return; }
        listeyiBosalt();
        faal = { kod: '', ad: '' };
        eko = { kod: '', ad: '' };
        $('#txtFaaliyet, #txtEko').val('');
        agaclar.forEach(function (a) { a.yilDegisti(); });
        grid.setGridOption('columnDefs', sutunlar(yilOku() || new Date().getFullYear()));
    });

    // ---------------------------------------------------------------- ağaçlar (eski ddProg / ddEko)

    // Seçim kutusu + ağaç penceresi. secim: { kod, ad } nesnesini döndüren / atayan işlevler.
    function agacKur(o) {
        var dlg = document.getElementById(o.pencere);
        var agac = null, agacYil = 0;

        $(o.dugme).on('click', function () {
            var yil = yilOku();
            if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
            dlg.showModal();
            if (!agac) {
                // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
                agacYil = yil;
                agac = Sgb.agac(o.liste, {
                    yukle: function (ust) {
                        return Sgb.al(o.adres, { yil: agacYil, ust: ust ? ust.kod : '' }).then(function (v) {
                            if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                            return v.dugumler;
                        });
                    },
                    secilince: function (d) {
                        dlg.close();
                        if (d.kod === o.al().kod) return;
                        if (acik && !degisiklikVarsaSor('Seçimi değiştirirseniz')) return;
                        listeyiBosalt();
                        o.ata({ kod: d.kod, ad: d.ad });
                        $(o.kutu).val(d.ad);
                    },
                    seciliMi: function (d) { return d.kod === o.al().kod; },
                    ilkAcik: true,
                    bosMetin: 'Kod bulunamadı.',
                    hata: hataGoster
                });
            } else if (agacYil !== yil) {
                agacYil = yil;
                agac.yenile();
            }
        });

        // Salt okunur kutuya tıklamak (veya Enter/boşluk) da ağacı açar.
        $(o.kutu).on('click', function () { $(o.dugme).trigger('click'); })
            .on('keydown', function (e) {
                if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); $(o.dugme).trigger('click'); }
            });

        return { yilDegisti: function () { agacYil = 0; } };
    }

    var agaclar = [
        agacKur({ dugme: '#btnFaaliyetSec', kutu: '#txtFaaliyet', pencere: 'dlgFaaliyet', liste: '#agcFaaliyet', adres: 'api/gorusmecari/faaliyetler',
            al: function () { return faal; }, ata: function (d) { faal = d; } }),
        agacKur({ dugme: '#btnEkoSec', kutu: '#txtEko', pencere: 'dlgEko', liste: '#agcEko', adres: 'api/gorusmecari/ekonomik',
            al: function () { return eko; }, ata: function (d) { eko = d; } })
    ];

    // ---------------------------------------------------------------- işlemler

    var sor = TG.soruKur();

    function kaydet() {
        grid.stopEditing();
        var satirlar = [];
        grid.forEachNode(function (n) {
            var d = n.data;
            if (!degisen[d.id] || !tertipSatiri(d)) return;
            satirlar.push({ kod1: d.kod1, kod2: d.kod2, kod3: d.kod3, kod4: d.kod4, kod5: d.kod5, kod6: d.kod6,
                yil0: d.yil0, yil1: d.yil1, yil2: d.yil2, ekOdenek: d.ekOdenek });
        });
        if (!satirlar.length) { mesaj('Kaydedilecek değişiklik yok.', 'info'); return; }
        $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/gorusmecari/kaydet', { yil: acik.yil, faal: acik.faal || '', satirlar: satirlar })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (s.basarili) degisen = {};
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
        onayla: ['Onayladıktan sonra faaliyet için SGB aşaması detay girişi açılacaktır. Devam etmek istiyor musunuz?', 'Onayla'],
        kapat: ['Faaliyet için SGB aşaması detay girişi veri girişine kapatılacaktır. Devam etmek istiyor musunuz?', 'Kapat'],
        kaldir: ['Faaliyet için SGB aşaması detay girişi veri girişine kapatılacak, tavan girişi açılacaktır. Devam etmek istiyor musunuz?', 'Onayı Kaldır']
    };

    $('.tavan-gorusme').on('click', '[data-islem]', function () {
        if (!acik || !acik.faal) return;
        var islem = $(this).attr('data-islem');
        if (islem === 'onayla' && kirli()) { mesaj('Önce değişiklikleri kaydediniz.', 'warning'); return; }
        sor('Onay', ONAY_SORU[islem][0], ONAY_SORU[islem][1], function () {
            Sgb.gonder('api/gorusmecari/onay', { yil: acik.yil, faal: acik.faal, islem: islem })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) { durum = s.durum; durumGoster(); }
                })
                .fail(hataGoster);
        });
    });

    $('#btnTekliftenAl').on('click', function () {
        if (!girisAcik()) return;
        var n = TG.tekliftenAl(grid, { yil0: 'teklif0', yil1: 'teklif1', yil2: 'teklif2', ekOdenek: 'ekOdenekTeklif' }, tertipSatiri);
        if (!n.length) { mesaj('Tekliften alınacak tutar yok (yalnızca boş tutarlar doldurulur).', 'info'); return; }
        $.each(n, function (i, d) { degisen[d.data.id] = true; });
        toplamHesapla();
        mesaj(n.length + ' satıra teklif tutarları alındı; kaydetmeyi unutmayınız.', 'info');
    });

    $('#btnKopyala').on('click', function () { TG.kopyala(grid); });

    TG.yapistirKur({
        grid: grid,
        alanlar: GIRILEN,
        satirUygun: tertipSatiri,
        acik: girisAcik,
        sonra: function (dugumler) {
            $.each(dugumler, function (i, n) { degisen[n.data.id] = true; });
            toplamHesapla();
        }
    });

    // Rapor ekrandaki seçimle alınır (eski btnRaporAl_Click: ekonomik kod ve faaliyet/proje).
    var raporlariDoldur = TG.raporMenuKur('api/gorusmecari/rapor', function (rapor) {
        var o = { yil: yilOku(), faal: faal.kod, eko: eko.kod, rapor: rapor };
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return null; }
        return o;
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli()) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış

    durumGoster();
    if (adresle) {
        // Başka sayfadan verilen tertip (eski ?yil=&tertip=): seçim, rapor ve onay yok.
        $('#bolSecim, .rapor-menu').prop('hidden', true);
        $('.tavan-gorusme').addClass('adresle');
        $('#txtYil').val(adresYil);
        listele({ yil: adresYil, tertip: adresTertip });
        return;
    }
    Sgb.al('api/gorusmecari/tanim')
        .done(function (t) {
            if (t.yil) $('#txtYil').val(t.yil);
            raporlariDoldur(t.raporlar);
            grid.setGridOption('columnDefs', sutunlar(t.yil || new Date().getFullYear()));
        })
        .fail(hataGoster);
});
