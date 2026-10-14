// Faaliyet/Proje Detay Teklif Girişi (ButceHazirlikv2/TavanTertip): faaliyet/proje ağaçtan seçilir, faaliyetin onaylı ekonomik kod +
// sektör + birim tavanlarından biri seçilip listelenir; tavan tertiplere (kurumsal-fonksiyonel-finansman-ekonomik) üç yıllık teklif ve
// ek ödenek olarak dağıtılır. En üstteki TAVAN, FARK (tavan - toplam) ve TOPLAM satırları salt okunurdur; bir tutar değişince
// yeniden hesaplanır. Onaylanmış (durum > 1) teklifler değiştirilemez. Raporlar sunucuda eski kodla üretilir.
$(function () {
    'use strict';

    if (!$('#grdTertip').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    // TOPLAM ve FARK satırlarında hesaplanan alanlar (eski Oku).
    var SAYILAR = ['odenek_2', 'harcama_2', 'odenek_1', 'harcama_1', 'talep_0', 'yil0', 'yil1', 'yil2', 'ekOdenek', 'ekOdenek1', 'ekOdenek2'];
    // Girilebilen sütunlar, tablodaki sırasıyla (Yapıştır bu sırayla yazar).
    var GIRILEN = ['yil0', 'yil1', 'yil2', 'ekOdenek', 'ekGerekce', 'ekOdenek1', 'ekOdenek2'];
    // Sade görünümde gizlenen sütunlar (eski cbKolonSec).
    var SADE_GIZLI = ['odenek_2', 'harcama_2', 'odenek_1', 'harcama_1', 'talep_0', 'yil0Oran', 'yil1Oran', 'yil2Oran'];
    var acik = null;     // listelenmiş seçim { yil, kod ("eko@sektör@birim@faaliyet") }: Kaydet / Onay bunu kullanır
    var tavan = null;    // faaliyetin ekonomik kod + sektör + birim tavanı (TAVAN satırı)
    var durum = -1;      // tertip tekliflerinin onay durumu: 0/1 açık, 5 onaylı, -1 okunamadı
    var kirli = false;   // kaydedilmemiş değişiklik var mı
    var faal = '';       // ağaçtan seçilen faaliyet/proje kodu
    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });
    var yuzde = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function onayli() { return durum > 1; }
    function girisAcik() { return durum >= 0 && !onayli(); }
    function faaliyetSatiri(d) { return String(d && d.kod4 || '').length > 2; }

    // "1.234.000" ya da "1234000" -> 1234000. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function kacir(metin) { return $('<span></span>').text(metin == null ? '' : metin).html(); }

    // ---------------------------------------------------------------- tablo

    // Yalnızca tertip satırları (ekonomik kodu 2 haneden uzun), onaysızken girilir (eski BeforeEditPar).
    function girilebilir(p) {
        return girisAcik() && !p.node.rowPinned && GIRILEN.indexOf(p.colDef.field) >= 0 && faaliyetSatiri(p.data);
    }

    function hucreSinifi(p) {
        var s = [];
        if (p.colDef.type === 'sayi') s.push('sayi');
        if (p.colDef.field === 'ekGerekce') s.push('metin');
        if (p.colDef.colId === 'tertip') s.push('tertip');
        if (!p.node.rowPinned) {
            if (faaliyetSatiri(p.data)) {
                if (girilebilir(p)) s.push('girilir');
                else s.push('kapali', p.data.kod4 === '06.1.4.01' ? 'mavi' : '');
            }
        } else if (p.data.kod4 === '--' && typeof p.value === 'number' && p.value < 0) s.push('eksi');
        return s;
    }

    function para(p) { return p.value == null || p.value === '' ? '' : tamSayi.format(p.value); }

    // Artış yüzdesi: üç yılın teklifi de bir önceki yılın K.B.Ö'ne göre (eski ColorRendererOran).
    function artis(alan) {
        return function (p) {
            if (!p.data || p.data.kod4 === '--') return null; // FARK satırında artış anlamsız
            var b = Number(p.data.odenek_1) || 0;
            return b ? ((Number(p.data[alan]) || 0) - b) * 100 / b : null;
        };
    }

    // Tertip: tertip satırında "kurumsal-fonksiyonel-finansman-ekonomik" kodu ve altında ekonomik kod adı, diğerlerinde yalnızca ad.
    function tertipCiz(p) {
        if (!p.data) return '';
        if (p.node.rowPinned || !faaliyetSatiri(p.data)) return kacir(p.data.ad4);
        return '<span class="kod">' + kacir([p.data.kod1, p.data.kod2, p.data.kod3, p.data.kod4].join('-')) + '</span>' + kacir(p.data.ad4);
    }

    function tertipMetin(p) {
        if (!p.data) return '';
        if (p.node.rowPinned || !faaliyetSatiri(p.data)) return p.data.ad4;
        return [p.data.kod1, p.data.kod2, p.data.kod3, p.data.kod4].join('-') + ' ' + (p.data.ad4 || '');
    }

    function sutunlar(yil) {
        function para2(alan, baslik, ek) {
            return $.extend({ field: alan, headerName: baslik, type: 'sayi', width: 108, valueFormatter: para, cellClass: hucreSinifi, headerClass: 'sayi' }, ek || {});
        }
        function oran(alan) {
            return { colId: alan + 'Oran', headerName: 'Artış %', type: 'sayi', width: 76, sortable: false, headerClass: 'sayi', cellClass: hucreSinifi,
                valueGetter: artis(alan), valueFormatter: function (p) { return p.value == null ? '' : yuzde.format(p.value); } };
        }
        var giris = { editable: girilebilir, cellEditor: 'agTextCellEditor', valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; } };
        var sade = $('#chkSade').prop('checked');
        var liste = [
            { colId: 'tertip', headerName: 'Tertip', width: 280, pinned: 'left', sortable: false, cellRenderer: tertipCiz, valueGetter: tertipMetin,
                tooltipValueGetter: tertipMetin, cellClass: hucreSinifi },
            para2('odenek_2', (yil - 2) + ' K.B.Ö'),
            para2('harcama_2', (yil - 2) + ' Harcama'),
            para2('odenek_1', (yil - 1) + ' K.B.Ö'),
            para2('harcama_1', (yil - 1) + ' Harcama'),
            para2('talep_0', yil + ' Taşra Teklifi'),
            para2('yil0', yil + ' Teklif', giris),
            oran('yil0'),
            para2('yil1', (yil + 1) + ' Teklif', giris),
            oran('yil1'),
            para2('yil2', (yil + 2) + ' Teklif', giris),
            oran('yil2'),
            para2('ekOdenek', yil + ' Ek Ödenek', giris),
            { field: 'ekGerekce', headerName: yil + ' Ek Gerekçe', width: 200, sortable: false, tooltipField: 'ekGerekce', cellClass: hucreSinifi,
                editable: girilebilir, cellEditor: 'agLargeTextCellEditor', cellEditorPopup: true, cellEditorParams: { maxLength: 300, rows: 5, cols: 50 } },
            para2('ekOdenek1', (yil + 1) + ' Ek Ödenek', giris),
            para2('ekOdenek2', (yil + 2) + ' Ek Ödenek', giris)
        ];
        $.each(liste, function (i, c) { if (SADE_GIZLI.indexOf(c.colId || c.field) >= 0) c.hide = sade; });
        return liste;
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
        getRowHeight: function (p) { return !p.node.rowPinned && faaliyetSatiri(p.data) ? 40 : 30; },
        getRowClass: function (p) {
            if (p.node.rowPinned) return p.data.kod4 === '++' ? 'satir-tavan' : '';
            return faaliyetSatiri(p.data) ? '' : 'satir-toplam';
        },
        overlayNoRowsTemplate: '<span>Faaliyet/proje ve tavan seçip Listele\'ye basınız.</span>',
        onCellValueChanged: function (e) {
            if (e.node.rowPinned || e.newValue === e.oldValue) return;
            kirli = true;
            toplamHesapla();
        }
    });

    // Üstteki satırlar (eski "T A V A N" ++, "F A R K" --, "T O P L A M" 00); TOPLAM bütün satırların toplamıdır (eski Oku).
    function toplamHesapla() {
        if (!tavan) { grid.setGridOption('pinnedTopRowData', []); return; }
        var t = { id: '00', kod4: '00', ad4: 'T O P L A M' };
        $.each(SAYILAR, function (i, a) { t[a] = 0; });
        grid.forEachNode(function (n) { $.each(SAYILAR, function (i, a) { t[a] += Number(n.data[a]) || 0; }); });
        var f = { id: '--', kod4: '--', ad4: 'F A R K' };
        $.each(SAYILAR, function (i, a) { f[a] = Math.round(((Number(tavan[a]) || 0) - t[a]) * 100) / 100; });
        grid.setGridOption('pinnedTopRowData', [$.extend({}, tavan, { id: '++', kod4: '++', ad4: 'T A V A N', ekGerekce: '' }), f, t]);
    }

    // Tuşlar onay durumuna göre (eski TuslariAyarla): okunamadı -> hiçbiri; 5 -> Onay Kaldır; diğer -> Kaydet, Onayla.
    function durumGoster() {
        var $e = $('#durumEtiket');
        var var_ = !!acik && durum >= 0;
        if (!var_) $e.prop('hidden', true);
        else $e.prop('hidden', false).toggleClass('label-success', onayli()).toggleClass('label-default', !onayli()).text(onayli() ? 'Onaylı' : 'Açık');
        $('#btnKaydet, #btnOnayla, #btnOranla, #btnYapistir').prop('hidden', !var_ || onayli());
        $('#btnKaydet, #btnOranla, #btnYapistir').prop('disabled', !grid.getDisplayedRowCount());
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
        Sgb.al('api/tavantertip/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = o;
                tavan = v.tavan;
                durum = v.durum;
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
        kirli = false;
        grid.setGridOption('rowData', []);
        grid.setGridOption('pinnedTopRowData', []);
        durumGoster();
    }

    // Eko. Kod / Sektör / Birim seçenekleri: değer "eko@sektör@birim@faaliyet" (eski cbEkoFaal tamKod).
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
        Sgb.al('api/tavantertip/ekofaal', { yil: yil, faal: faal })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                ekoFaalDoldur(v.kodlar);
            })
            .fail(hataGoster);
    }

    $('#ddlEkoFaal').on('change', function () {
        if (acik && !degisiklikVarsaSor('Seçimi değiştirirseniz')) { this.value = acik.kod; return; }
        listeyiBosalt();
    });

    // Faaliyetler yıla bağlıdır: yıl değişince seçim boşalır.
    $('#txtYil').on('change', function () {
        if (acik && !degisiklikVarsaSor('Yılı değiştirirseniz')) { this.value = acik.yil; return; }
        listeyiBosalt();
        faal = '';
        $('#txtFaaliyet').val('');
        ekoFaalDoldur([]);
        agacYil = 0;
        grid.setGridOption('columnDefs', sutunlar(yilOku() || new Date().getFullYear()));
    });

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
                    return Sgb.al('api/tavantertip/faaliyetler', { yil: agacYil, ust: ust ? ust.kod : '' }).then(function (v) {
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
            if (!faaliyetSatiri(d)) return;
            satirlar.push({ kod1: d.kod1, kod2: d.kod2, kod3: d.kod3, kod4: d.kod4, yil0: d.yil0, yil1: d.yil1, yil2: d.yil2,
                ekOdenek: d.ekOdenek, ekGerekce: d.ekGerekce || '', ekOdenek1: d.ekOdenek1, ekOdenek2: d.ekOdenek2 });
        });
        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/tavantertip/kaydet', { yil: acik.yil, kod: acik.kod, satirlar: satirlar })
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
        Sgb.gonder('api/tavantertip/onay', { yil: acik.yil, kod: acik.kod, onayla: onayla })
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

    // Teklif Üret (eski Oranla / OranIslem): her yıl için TAVAN tutarı, satırlara önceki yılın K.B.Ö'leri oranında dağıtılır ve binliğe
    // yuvarlanır; yuvarlama farkı kalan satırlarda kapanır (dağıtılacak kalanı tam karşılayan satıra kalanın hepsi yazılır).
    function oranIslem(kaynakAlan, kaynakToplam, hedefAlan, hedefToplam) {
        var dugumler = [];
        grid.forEachNode(function (n) { dugumler.push(n); });
        for (var i = 0; i < dugumler.length; i++) {
            if (hedefToplam === 0 || kaynakToplam === 0) break;
            var d = dugumler[i].data;
            var eski = Number(d[kaynakAlan]) || 0;
            var yeni = eski === kaynakToplam ? hedefToplam : Math.round(((eski / kaynakToplam) * hedefToplam) / 1000) * 1000;
            d[hedefAlan] = yeni;
            kaynakToplam -= eski;
            hedefToplam -= yeni;
        }
        return hedefToplam;
    }

    $('#btnOranla').on('click', function () {
        if (!acik || !tavan || !girisAcik()) return;
        grid.stopEditing();
        var kbo = Number(tavan.odenek_1) || 0;
        if (!kbo) { mesaj('Tavanın önceki yıl K.B.Ö tutarı sıfır; oranlanamaz.', 'warning'); return; }
        for (var i = 0; i < 3; i++) oranIslem('odenek_1', kbo, 'yil' + i, Number(tavan['yil' + i]) || 0);
        kirli = true;
        grid.refreshCells({ force: true });
        toplamHesapla();
        mesaj('Teklifler önceki yılın K.B.Ö oranında üretildi; kaydetmeyi unutmayınız.', 'info');
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

    // Yapıştır (eski EXTVeriYapistir): panodaki satırlar seçili satırdan başlayarak sırayla, sütunlar Teklif sütunundan başlayarak
    // girilebilir sütunlara yazılır. Tertip satırı olmayan satırlara yazılmaz (panodaki o satır atlanır).
    var yapistirBas = -1;
    var dlgYapistir = document.getElementById('dlgYapistir');

    function yapistir(metin) {
        var satirlar = String(metin || '').split('\n');
        while (satirlar.length && /^\r?$/.test(satirlar[satirlar.length - 1])) satirlar.pop();
        if (!satirlar.length) { mesaj('Panoda yapıştırılacak veri yok.', 'warning'); return; }
        var dugumler = [];
        grid.forEachNodeAfterFilterAndSort(function (n) { dugumler.push(n); });
        var yazilan = 0;
        for (var i = 0; i < satirlar.length && yapistirBas + i < dugumler.length; i++) {
            var d = dugumler[yapistirBas + i].data;
            if (!faaliyetSatiri(d)) continue;
            var degerler = satirlar[i].replace(/\r$/, '').split('\t');
            for (var j = 0; j < Math.min(GIRILEN.length, degerler.length); j++) {
                var alan = GIRILEN[j], deger = degerler[j];
                if (alan === 'ekGerekce') d[alan] = String(deger).slice(0, 300);
                else {
                    var n = parseFloat(String(deger).replace(/\./g, '').replace(/,/g, '.'));
                    d[alan] = isFinite(n) ? n : 0;
                }
            }
            yazilan++;
        }
        if (!yazilan) return;
        kirli = true;
        grid.refreshCells({ force: true });
        toplamHesapla();
        mesaj(yazilan + ' satır yapıştırıldı; kaydetmeyi unutmayınız.', 'info');
    }

    $('#btnYapistir').on('click', function () {
        if (!acik || !girisAcik()) return;
        grid.stopEditing();
        var hucre = grid.getFocusedCell();
        if (!hucre || hucre.rowPinned || hucre.rowIndex == null || hucre.rowIndex < 0) { mesaj('Lütfen satır seçiniz!', 'warning'); return; }
        yapistirBas = hucre.rowIndex;
        // Pano okunamazsa (tarayıcı izin vermedi) veri pencereye yapıştırılır.
        var pencere = function () { $('#txtYapistir').val(''); dlgYapistir.showModal(); $('#txtYapistir').trigger('focus'); };
        if (navigator.clipboard && navigator.clipboard.readText && window.isSecureContext) navigator.clipboard.readText().then(yapistir, pencere);
        else pencere();
    });

    $('#btnYapistirTamam').on('click', function () {
        dlgYapistir.close();
        yapistir($('#txtYapistir').val());
    });

    // Sade görünüm (eski cbKolonSec): önceki yıllar, harcama, taşra teklifi ve artış sütunları gizlenir.
    $('#chkSade').on('change', function () { grid.setColumnsVisible(SADE_GIZLI, !this.checked); });

    // ---------------------------------------------------------------- Rapor Al menüsü

    var $raporGrup = $('.rapor-menu');
    function menuKapat() { $raporGrup.removeClass('open'); $('#btnRapor').attr('aria-expanded', 'false'); }

    function raporlariDoldur(liste) {
        var $l = $('#lstRapor').empty();
        $.each(liste || [], function (i, r) {
            $('<li></li>').append($('<button type="button"></button>').attr('data-rapor', r.kod).text(r.ad)).appendTo($l);
        });
    }

    $('#btnRapor').on('click', function (e) {
        e.stopPropagation();
        var ac = !$raporGrup.hasClass('open');
        $raporGrup.toggleClass('open', ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false');
        if (ac) $('#lstRapor button').first().trigger('focus');
    });
    $(document).on('click', menuKapat);
    $(document).on('keydown', function (e) { if (e.key === 'Escape' && $raporGrup.hasClass('open')) { menuKapat(); $('#btnRapor').trigger('focus'); } });

    // Rapor ekrandaki seçimle alınır (eski RaporAl: tavan seçilmemişse uyarır).
    $('#lstRapor').on('click', 'button', function () {
        menuKapat();
        var o = { yil: yilOku(), kod: $('#ddlEkoFaal').val() || '', rapor: $(this).attr('data-rapor') };
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!o.kod) { mesaj('Lütfen seçim yapınız!', 'warning'); return; }
        var $d = $('#btnRapor').prop('disabled', true);
        Sgb.indir('api/tavantertip/rapor', o).then(function () { $d.prop('disabled', false); });
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış

    durumGoster();
    ekoFaalDoldur([]);
    Sgb.al('api/tavantertip/tanim')
        .done(function (t) {
            if (t.yil) $('#txtYil').val(t.yil);
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            raporlariDoldur(t.raporlar);
            grid.setGridOption('columnDefs', sutunlar(t.yil));
        })
        .fail(hataGoster);
});
