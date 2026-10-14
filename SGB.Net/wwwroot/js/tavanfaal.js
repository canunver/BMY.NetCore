// Koordinasyon Tavan Girişi ve Kontrolü (ButceHazirlikv2/TavanEkoFaaliyet): birim ve onun ekonomik kod + sektör tavanı seçilir,
// tavan faaliyet/projelere üç yıllık olarak dağıtılır. En üstteki TAVAN, FARK (tavan - toplam) ve TOPLAM satırları salt okunurdur;
// bir tutar değişince TOPLAM ve FARK yeniden hesaplanır. Onaylanmış (durum 5/6) tavanlar değiştirilemez.
// Toplu Onay penceresi yalnızca sistem yöneticisine görünür. Raporlar sunucuda eski kodla üretilir.
$(function () {
    'use strict';

    if (!$('#grdTavan').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    // TOPLAM satırında toplanan alanlar; FARK satırında teklifler (Faal. Kontrol) yoktur (eski sayfadaki gibi).
    var SAYILAR = ['odenek_2', 'harcama_2', 'odenek_1', 'yil0', 'yil1', 'yil2', 'teklif0', 'teklif1', 'teklif2', 'ekOdenek', 'ekOdenek1', 'ekOdenek2'];
    var FARKLAR = ['odenek_2', 'harcama_2', 'odenek_1', 'yil0', 'yil1', 'yil2', 'ekOdenek', 'ekOdenek1', 'ekOdenek2'];
    var GIRILEN = ['yil0', 'yil1', 'yil2'];
    var sistemYoneticisi = false;
    var acik = null;     // listelenmiş ölçüt { yil, eko, sektor, birim }: Kaydet / Onay bunu kullanır
    var tavan = null;    // listelenen ekonomik kod + sektör + birim tavanı (TAVAN satırı)
    var durum = -1;      // faaliyet tavanlarının onay durumu: 0/1 açık, 5 onaylı (detay açık), 6 onaylı (detay kapalı), -1 okunamadı
    var kirli = false;   // kaydedilmemiş değişiklik var mı
    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });
    var yuzde = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }

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

    // Ekrandaki seçim { eko, sektor, birim }; eko-sektör seçilmemişse eko ve sektör boştur.
    function secim() {
        var p = ($('#ddlEkoSektor').val() || '').split('@');
        return { yil: yilOku(), eko: p[0] || '', sektor: p[1] || '', birim: p.length > 2 ? p[2] : ($('#ddlBirim').val() || '') };
    }

    function onayli() { return durum > 1; }
    function girisAcik() { return durum >= 0 && !onayli(); }

    // "1.234.000" ya da "1234000" -> 1234000. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    // ---------------------------------------------------------------- tablo

    // Yalnızca faaliyet/proje satırlarının (kod 2 haneden uzun) tavanları, onaysızken girilir (eski BeforeEdit).
    function girilebilir(p) {
        return girisAcik() && !p.node.rowPinned && GIRILEN.indexOf(p.colDef.field) >= 0 && String(p.data.kod4 || '').length > 2;
    }

    function hucreSinifi(p) {
        var s = [];
        if (p.colDef.type === 'sayi') s.push('sayi');
        if (!p.node.rowPinned) s.push(girilebilir(p) ? 'girilir' : 'kapali');
        else if (p.data.kod4 === '--' && typeof p.value === 'number' && p.value < 0) s.push('eksi');
        return s;
    }

    function para(p) { return p.value == null || p.value === '' ? '' : tamSayi.format(p.value); }

    // Artış yüzdesi: tavanın bir önceki yıla göre artışı (yil0: önceki yılın K.B.Ö'ne, yil1: yil0'a, yil2: yil1'e göre).
    function artis(alan, taban) {
        return function (p) {
            if (!p.data || p.data.kod4 === '--') return null; // FARK satırında artış anlamsız
            var b = p.data[taban];
            return b ? (p.data[alan] - b) * 100 / b : null;
        };
    }

    function sutunlar(yil) {
        function para2(alan, baslik, ek) {
            return $.extend({ field: alan, headerName: baslik, type: 'sayi', width: 108, valueFormatter: para, cellClass: hucreSinifi, headerClass: 'sayi' }, ek || {});
        }
        function oran(alan, taban) {
            return { colId: alan + 'Oran', headerName: 'Artış %', type: 'sayi', width: 76, sortable: false, headerClass: 'sayi', cellClass: hucreSinifi,
                valueGetter: artis(alan, taban), valueFormatter: function (p) { return p.value == null ? '' : yuzde.format(p.value); } };
        }
        var giris = { editable: girilebilir, cellEditor: 'agTextCellEditor', valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; } };
        return [
            { field: 'kod4', headerName: 'Faal. Kodu', width: 90, pinned: 'left', cellClass: hucreSinifi },
            { field: 'ad4', headerName: 'Faaliyet/Proje Adı', width: 260, pinned: 'left', tooltipField: 'ad4', cellClass: hucreSinifi },
            para2('odenek_2', (yil - 2) + ' K.B.Ö'),
            para2('harcama_2', (yil - 2) + ' Harcama'),
            para2('odenek_1', (yil - 1) + ' K.B.Ö'),
            para2('yil0', yil + ' Tavan', giris),
            oran('yil0', 'odenek_1'),
            para2('yil1', (yil + 1) + ' Tavan', giris),
            oran('yil1', 'yil0'),
            para2('yil2', (yil + 2) + ' Tavan', giris),
            oran('yil2', 'yil1'),
            para2('teklif0', yil + ' Faal. Kontrol'),
            para2('teklif1', (yil + 1) + ' Faal. Kontrol'),
            para2('teklif2', (yil + 2) + ' Faal. Kontrol'),
            para2('ekOdenek', yil + ' Faal. Ek Kontrol', { width: 120 }),
            para2('ekOdenek1', (yil + 1) + ' Faal. Ek Kontrol', { width: 120 }),
            para2('ekOdenek2', (yil + 2) + ' Faal. Ek Kontrol', { width: 120 })
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
        getRowId: function (p) { return p.data.id; },
        getRowClass: function (p) { return p.node.rowPinned && p.data.kod4 === '++' ? 'satir-tavan' : ''; },
        overlayNoRowsTemplate: '<span>Birim ve ekonomik kod / sektör seçip Listele\'ye basınız.</span>',
        onCellValueChanged: function (e) {
            if (e.node.rowPinned || e.newValue === e.oldValue) return;
            kirli = true;
            toplamHesapla();
        }
    });

    // Üstteki satırlar (eski "T A V A N" ++, "F A R K" --, "T O P L A M" 00).
    function toplamHesapla() {
        if (!tavan) { grid.setGridOption('pinnedTopRowData', []); return; }
        var t = { id: '00', kod4: '00', ad4: 'T O P L A M' };
        $.each(SAYILAR, function (i, a) { t[a] = 0; });
        grid.forEachNode(function (n) { $.each(SAYILAR, function (i, a) { t[a] += Number(n.data[a]) || 0; }); });
        var f = { id: '--', kod4: '--', ad4: 'F A R K' };
        $.each(FARKLAR, function (i, a) { f[a] = Math.round(((Number(tavan[a]) || 0) - t[a]) * 100) / 100; });
        grid.setGridOption('pinnedTopRowData', [$.extend({}, tavan, { id: '++', kod4: '++', ad4: 'T A V A N' }), f, t]);
    }

    var DURUM_AD = { 0: 'Açık', 1: 'Açık', 5: 'Onaylı (Faaliyet Girişine Açık)', 6: 'Onaylı (Faaliyet Girişine Kapalı)' };

    // Tuşlar onay durumuna göre (eski TuslariAyarla): açık -> Kaydet, Onayla; 5 -> Kapat, Onay Kaldır; 6 -> Girişe Aç (sistem yöneticisi).
    function durumGoster() {
        var $e = $('#durumEtiket');
        var var_ = !!acik && durum >= 0;
        if (!var_) $e.prop('hidden', true);
        else $e.prop('hidden', false).toggleClass('label-success', onayli()).toggleClass('label-default', !onayli()).text(DURUM_AD[durum] || ('Durum ' + durum));
        $('#btnKaydet, #btnOnayla').prop('hidden', !var_ || onayli());
        $('#btnKaydet').prop('disabled', !grid.getDisplayedRowCount());
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
        Sgb.al('api/tavanfaal/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = o;
                tavan = v.tavan;
                durum = v.durum;
                kirli = false;
                grid.setGridOption('columnDefs', sutunlar(o.yil));
                grid.setGridOption('overlayNoRowsTemplate', '<span>Faaliyet / proje bulunamadı.</span>');
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
        kirli = false;
        grid.setGridOption('rowData', []);
        grid.setGridOption('pinnedTopRowData', []);
        durumGoster();
    }

    function ekoSektorYukle() {
        var yil = yilOku(), birim = $('#ddlBirim').val() || '';
        ekoSektorDoldur([]);
        if (yil < 2000 || !birim) return;
        Sgb.al('api/tavanfaal/ekosektor', { yil: yil, birim: birim })
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
        Sgb.al('api/tavanfaal/birimler', { yil: yil })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                birimleriDoldur(v.birimler);
                ekoSektorYukle();
            })
            .fail(hataGoster);
    });

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    function sor(baslik, metin, evet, is) {
        $('#dlgSoruBaslik').text(baslik);
        $('#soruMetin').text(metin);
        $('#btnSoruEvet').text(evet);
        soruIsi = is;
        dlgSoru.showModal();
    }

    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        if (soruIsi) soruIsi();
        soruIsi = null;
    });

    // ---------------------------------------------------------------- işlemler

    function kaydet() {
        grid.stopEditing();
        var satirlar = [];
        grid.forEachNode(function (n) {
            var d = n.data;
            if (String(d.kod4 || '').length <= 2) return;
            // Ek ödenekler değişmez ama geri gönderilir: kayıt onları da yazar.
            satirlar.push({ kod1: d.kod1, kod2: d.kod2, kod3: d.kod3, kod4: d.kod4, kod5: d.kod5, kod6: d.kod6,
                yil0: d.yil0, yil1: d.yil1, yil2: d.yil2, ekOdenek: d.ekOdenek, ekOdenek1: d.ekOdenek1, ekOdenek2: d.ekOdenek2 });
        });
        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/tavanfaal/kaydet', { yil: acik.yil, eko: acik.eko, sektor: acik.sektor, birim: acik.birim, satirlar: satirlar })
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
        onayla: ['Onayladıktan sonra faaliyet/proje tavanlarını değiştiremezsiniz. Devam etmek istiyor musunuz?', 'Onayla'],
        kapat: ['Giriş işlemi tamamlandı yapılınca, faaliyet/proje bütçe rakamlarını değiştiremezsiniz. Devam etmek istiyor musunuz?', 'Girişe Kapat'],
        kaldir: ['Tavan girişi veri girişine açılacaktır. Devam etmek istiyor musunuz?', 'Onayı Kaldır'],
        ac: ['Faaliyet girişi veri girişine açılacaktır. Devam etmek istiyor musunuz?', 'Girişe Aç']
    };

    $('.tavan-faal').on('click', '[data-islem]', function () {
        if (!acik) return;
        var islem = $(this).attr('data-islem');
        if (islem === 'onayla' && kirli) { mesaj('Önce değişiklikleri kaydediniz.', 'warning'); return; }
        sor('Onay', ONAY_SORU[islem][0], ONAY_SORU[islem][1], function () {
            Sgb.gonder('api/tavanfaal/onay', { yil: acik.yil, eko: acik.eko, sektor: acik.sektor, birim: acik.birim, islem: islem })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) { durum = s.durum; durumGoster(); }
                })
                .fail(hataGoster);
        });
    });

    // Kopyala: tablo (başlıklar ve TAVAN / FARK / TOPLAM satırları dahil) sekmeyle ayrılmış metin olarak panoya.
    $('#btnKopyala').on('click', function () {
        grid.stopEditing();
        var sutun = grid.getAllDisplayedColumns();
        var satirlar = [sutun.map(function (c) { return c.getColDef().headerName; }).join('\t')];
        function ekle(n) {
            satirlar.push(sutun.map(function (c) {
                var v = grid.getCellValue({ rowNode: n, colKey: c, useFormatter: true });
                return String(v == null ? '' : v).replace(/[\t\r\n]+/g, ' ');
            }).join('\t'));
        }
        for (var i = 0; i < grid.getPinnedTopRowCount(); i++) ekle(grid.getPinnedTopRow(i));
        grid.forEachNodeAfterFilterAndSort(ekle);
        var metin = satirlar.join('\r\n');
        var tamam = function () { mesaj('Tablo panoya kopyalandı.', 'success'); };
        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(metin).then(tamam, function () { mesaj('Panoya kopyalanamadı.', 'danger'); });
        } else {
            var $t = $('<textarea readonly></textarea>').val(metin).css({ position: 'fixed', left: '-9999px' }).appendTo(document.body);
            $t[0].select();
            try { document.execCommand('copy'); tamam(); } catch (e) { mesaj('Panoya kopyalanamadı.', 'danger'); }
            $t.remove();
        }
    });

    // Yazdır: ekrandaki seçim (eski sayfadaki gibi; ekonomik kod / sektör seçilmemişse birimin bütün tavanları).
    $('#btnYazdir').on('click', function () {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/tavanfaal/rapor', o).then(function () { $d.prop('disabled', false); });
    });

    $('#btnImzali').on('click', function () {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!o.eko || !o.sektor || !o.birim) { mesaj('Lütfen seçim yapınız!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/tavanfaal/rapor-imzali', o).then(function () { $d.prop('disabled', false); });
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- toplu onay (sistem yöneticisi)

    var dlgOnay = document.getElementById('dlgOnay');
    var onayYil = 0;
    var onayGrid = null;

    function kilitDurumu() { return $('#chkDetayiKapat').prop('checked') ? 6 : 5; }

    function onayGridOlustur() {
        onayGrid = Sgb.grid('#grdOnay', {
            columnDefs: [
                { field: 'kod', headerName: 'Kod', width: 130 },
                { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 260, tooltipField: 'ad' },
                { field: 'durumAd', headerName: 'Durum', width: 170 },
                { colId: 'komut', headerName: 'Komutlar', width: 96, sortable: false, cellRenderer: function () {
                    return '<button type="button" class="btn btn-link btn-xs komut" data-durum="kilit" title="Onayla" aria-label="Onayla"><span class="glyphicon glyphicon-lock" aria-hidden="true"></span></button>' +
                        '<button type="button" class="btn btn-link btn-xs komut" data-durum="1" title="Aç" aria-label="Aç"><span class="glyphicon glyphicon-repeat" aria-hidden="true"></span></button>';
                } }
            ],
            getRowId: function (p) { return p.data.kod; },
            overlayNoRowsTemplate: '<span>Tavan bulunamadı.</span>'
        });
    }

    function onaylariYukle() {
        onayGrid.setGridOption('loading', true);
        Sgb.al('api/tavanfaal/onaylar', { yil: onayYil })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                onayGrid.setGridOption('rowData', v.onaylar);
            })
            .fail(hataGoster)
            .always(function () { onayGrid.setGridOption('loading', false); });
    }

    function topluKaydet(kod, yeniDurum, $d) {
        $d.prop('disabled', true);
        Sgb.gonder('api/tavanfaal/onay-toplu', { yil: onayYil, kod: kod, durum: yeniDurum })
            .done(function (s) {
                if (s.onaylar) onayGrid.setGridOption('rowData', s.onaylar);
                if (!s.basarili) mesaj(s.hata, 'danger');
                // Listelenen tavanın durumu değişmiş olabilir.
                if (acik && acik.yil === onayYil && (!kod || kod === [acik.eko, acik.sektor, acik.birim].join('-'))) {
                    var r = onayGrid.getRowNode([acik.eko, acik.sektor, acik.birim].join('-'));
                    if (r && typeof r.data.durum === 'number') { durum = r.data.durum; durumGoster(); }
                }
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    }

    $('#btnTopluOnay').on('click', function () {
        onayYil = yilOku();
        if (onayYil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        $('#dlgOnayBaslik').text('Onay durumu (' + onayYil + ')');
        dlgOnay.showModal();
        if (!onayGrid) onayGridOlustur();
        onayGrid.setGridOption('rowData', []);
        onaylariYukle();
    });

    $('#grdOnay').on('click', '[data-durum]', function () {
        var node = onayGrid.getRowNode($(this).closest('.ag-row').attr('row-id'));
        if (!node) return;
        var d = $(this).attr('data-durum');
        topluKaydet(node.data.kod, d === 'kilit' ? kilitDurumu() : 1, $(this));
    });

    $('#btnHepsiniOnayla').on('click', function () {
        var $b = $(this);
        sor('Onay', 'Bütün tavanların faaliyet girişi ' + (kilitDurumu() === 6 ? 'onaylanıp detay girişine kapatılacaktır' : 'onaylanacaktır') + '. Devam etmek istiyor musunuz?', 'Hepsini Onayla',
            function () { topluKaydet('', kilitDurumu(), $b); });
    });

    $('#btnHepsiniAc').on('click', function () {
        var $b = $(this);
        sor('Onay', 'Bütün tavanların onayı kaldırılıp veri girişine açılacaktır. Devam etmek istiyor musunuz?', 'Hepsini Aç',
            function () { topluKaydet('', 1, $b); });
    });

    // ---------------------------------------------------------------- açılış

    durumGoster();
    ekoSektorDoldur([]);
    Sgb.al('api/tavanfaal/tanim')
        .done(function (t) {
            sistemYoneticisi = !!t.sistemYoneticisi;
            $('#btnTopluOnay').prop('hidden', !sistemYoneticisi);
            if (t.yil) $('#txtYil').val(t.yil);
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            birimleriDoldur(t.birimler);
            grid.setGridOption('columnDefs', sutunlar(t.yil));
        })
        .fail(hataGoster);
});
