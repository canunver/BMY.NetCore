// Proje Gerekçe Girişi (ButceHazirlikv2/ProjeGerekceAnaForm): yıl, kurum ve sektör seçilip Oku'ya basılınca iki sekme dolar:
//   Tavan İçi Projeler      : alt projeler; yalnızca Gerekçe girilir (eski ProjeGerekceMevcut.aspx).
//   Yeni Tavan Dışı Projeler: birim ve sektörün yeni projeleri; satır açılıp silinir (eski ProjeGerekceYeni.aspx). Sektör boşsa salt okunur.
// Aşama seçimi, Ek / Özet raporları ve onay düğmeleri yalnızca sistem yöneticisine görünür. Raporlar sunucuda eski kodla üretilir.
$(function () {
    'use strict';

    if (!$('#grdMevcut').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var sistemYoneticisi = false;
    var acik = null;        // Oku ile açılmış ölçüt { yil, kur, sektor, senaryo }: Kaydet bunu kullanır
    var kapali = false;     // proje gerekçeleri veri girişine kapalı mı (sistem yöneticisi için hep false)
    var kirliMevcut = false, kirliYeni = false;
    var kurumKod = '';      // seçili kurumsal kod (alanda "kod- ad" yazar)
    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });
    var kurusLu = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }

    function secim() {
        return { yil: yilOku(), kur: kurumKod, sektor: $('#ddlSektor').val() || '', senaryo: sistemYoneticisi ? ($('#ddlSenaryo').val() || '') : '' };
    }

    function secenekler($ddl, liste, secili) {
        $ddl.empty();
        $.each(liste || [], function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($ddl); });
        if (secili !== undefined) $ddl.val(secili);
        if ($ddl.val() === null && $ddl.children().length) $ddl.prop('selectedIndex', 0);
    }

    // "1.234.000" ya da "1234000" -> 1234000. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function para(p) { return p.value == null || p.value === '' ? '' : tamSayi.format(p.value); }
    function paraKurus(p) { return p.value == null || p.value === '' ? '' : kurusLu.format(p.value); }
    function durumSinifi(girilebilir) { return function (p) { return girilebilir && girilebilir(p) ? 'girilir' : 'kapali'; }; }

    // Sayı sütunu (sağa yaslı). girilebilir verilirse hücre düzenlenir; sayı "1.234.000" biçiminde de yazılabilir.
    function sayiSutun(alan, baslik, girilebilir, ek) {
        var durum = durumSinifi(girilebilir);
        var s = { field: alan, headerName: baslik, width: 120, headerClass: 'sayi', valueFormatter: para,
            cellClass: function (p) { return ['sayi', durum(p)]; } };
        if (girilebilir) $.extend(s, { editable: girilebilir, cellEditor: 'agTextCellEditor',
            valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; } });
        return $.extend(s, ek || {});
    }

    function gerekceSutun(enCok, girilebilir) {
        return {
            field: 'gerekce', headerName: 'Gerekçe', flex: 1, minWidth: 300, wrapText: true, autoHeight: true, tooltipField: 'gerekce',
            editable: girilebilir, cellEditor: 'agLargeTextCellEditor', cellEditorPopup: true,
            cellEditorParams: { maxLength: enCok, rows: 6, cols: 60 }, cellClass: durumSinifi(girilebilir)
        };
    }

    var duzenleme = {
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true
    };

    // ---------------------------------------------------------------- Tavan İçi Projeler

    function mevcutGirilebilir() { return !!acik && !kapali; }

    function mevcutSutunlar(yil) {
        return [
            { field: 'birimKod', headerName: 'Birim', width: 110, pinned: 'left', cellClass: 'kapali' },
            { field: 'ad', headerName: 'Proje Adı', width: 320, pinned: 'left', wrapText: true, autoHeight: true, tooltipField: 'ad', cellClass: 'kapali' },
            sayiSutun('baslangicYil', 'Baş. Yılı', null, { width: 90, valueFormatter: null }),
            sayiSutun('bitisYil', 'Bitiş Yılı', null, { width: 90, valueFormatter: null }),
            sayiSutun('projeTutari', 'Proje Tutarı', null, { valueFormatter: paraKurus }),
            sayiSutun('yiliTeklifi', yil + ' Yılı Teklifi', null, { valueFormatter: paraKurus }),
            sayiSutun('yiliTeklifiEk', yil + ' Ek Talebi', null, { valueFormatter: paraKurus }),
            gerekceSutun(300, mevcutGirilebilir)
        ];
    }

    var grdMevcut = Sgb.grid('#grdMevcut', $.extend({}, duzenleme, {
        columnDefs: mevcutSutunlar(new Date().getFullYear()),
        getRowId: function (p) { return String(p.data.kod); },
        overlayNoRowsTemplate: '<span>Yıl, kurum ve sektörü seçip Oku\'ya basınız.</span>',
        onCellValueChanged: function (e) { if (e.newValue !== e.oldValue) kirliMevcut = true; }
    }));

    // ---------------------------------------------------------------- Yeni Tavan Dışı Projeler

    function yeniGirilebilir() { return !!acik && !!acik.sektor && !kapali; }

    function digerYillar(p) {
        var d = p.data;
        if (!d) return null;
        return (Number(d.projeTutari) || 0) - ((Number(d.yiliTeklifi0) || 0) + (Number(d.yiliTeklifi1) || 0) + (Number(d.yiliTeklifi2) || 0));
    }

    function yeniSutunlar(yil) {
        return [
            { colId: 'sira', headerName: 'Sıra', width: 80, sortable: false, headerClass: 'sayi', cellClass: 'sayi', valueGetter: function (p) { return p.node.rowIndex + 1; } },
            { field: 'ad', headerName: 'Proje Adı', width: 260, wrapText: true, autoHeight: true, tooltipField: 'ad',
                editable: yeniGirilebilir, cellEditor: 'agTextCellEditor', cellClass: durumSinifi(yeniGirilebilir) },
            sayiSutun('bitisYil', 'Bitiş Yılı', yeniGirilebilir, { width: 90, valueFormatter: null }),
            sayiSutun('projeTutari', 'Proje Tutarı', yeniGirilebilir),
            sayiSutun('yiliTeklifi0', yil + ' Yılı Teklifi', yeniGirilebilir),
            sayiSutun('yiliTeklifi1', (yil + 1) + ' Yılı Teklifi', yeniGirilebilir),
            sayiSutun('yiliTeklifi2', (yil + 2) + ' Yılı Teklifi', yeniGirilebilir),
            sayiSutun('digerYillar', 'Diğer Yıllar', null, { sortable: false, valueGetter: digerYillar }),
            gerekceSutun(1000, yeniGirilebilir)
        ];
    }

    function bosProje() { return { ad: '', bitisYil: 0, projeTutari: 0, yiliTeklifi0: 0, yiliTeklifi1: 0, yiliTeklifi2: 0, gerekce: '' }; }

    var grdYeni = Sgb.grid('#grdYeni', $.extend({}, duzenleme, {
        columnDefs: yeniSutunlar(new Date().getFullYear()),
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        overlayNoRowsTemplate: '<span>Yıl, kurum ve sektörü seçip Oku\'ya basınız.</span>',
        onCellValueChanged: function (e) {
            if (e.newValue === e.oldValue) return;
            kirliYeni = true;
            e.api.refreshCells({ rowNodes: [e.node], columns: ['digerYillar'] });
        }
    }));

    function yeniSiraYenile() { grdYeni.refreshCells({ columns: ['sira'], force: true }); }

    // ---------------------------------------------------------------- düğmeler ve durum

    function dugmeler() {
        var var_ = !!acik;
        $('#btnMevcutKaydet').prop('disabled', !var_ || kapali || !grdMevcut.getDisplayedRowCount());
        $('#btnMevcutKopyala').prop('disabled', !var_ || !grdMevcut.getDisplayedRowCount());
        $('#btnSatirAc, #btnSatirSil, #btnYeniKaydet').prop('disabled', !yeniGirilebilir());
        $('#btnYeniKopyala').prop('disabled', !var_ || !grdYeni.getDisplayedRowCount());
        $('#kapaliEtiket').prop('hidden', !var_ || !kapali);
        var not = '';
        if (var_ && !acik.sektor) not = 'Yeni proje girmek için sektör seçip yeniden okuyunuz.';
        else if (var_ && !kapali) not = 'Kaydedince birim ve sektörün yeni projeleri bu tablodakilerle değiştirilir.';
        $('#yeniNot').text(not);
    }

    function degisiklikVarsaSor(neden) {
        return !(kirliMevcut || kirliYeni) || window.confirm('Kaydedilmemiş değişiklikler var. ' + neden + ' kaybolacak. Devam edilsin mi?');
    }

    function bosalt() {
        if (!acik) return;
        acik = null;
        kapali = false;
        kirliMevcut = kirliYeni = false;
        grdMevcut.setGridOption('rowData', []);
        grdYeni.setGridOption('rowData', []);
        dugmeler();
        grdMevcut.refreshCells({ force: true });
        grdYeni.refreshCells({ force: true });
    }

    // ---------------------------------------------------------------- Oku (eski AdresYaz: iki çerçeve yeniden yüklenirdi)

    function oku() {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!degisiklikVarsaSor('Yeniden okunursa değişiklikler')) return;
        var $d = $('#btnOku').prop('disabled', true);
        grdMevcut.setGridOption('loading', true);
        grdYeni.setGridOption('loading', true);
        $.when(Sgb.al('api/projegerekce/mevcut', o), Sgb.al('api/projegerekce/yeni', o))
            .done(function (r1, r2) {
                var m = r1[0], y = r2[0];
                if (m.hata || y.hata) { mesaj(m.hata || y.hata, 'danger'); return; }
                acik = o;
                kapali = !!(m.kapali || y.kapali);
                kirliMevcut = kirliYeni = false;
                grdMevcut.setGridOption('columnDefs', mevcutSutunlar(o.yil));
                grdMevcut.setGridOption('overlayNoRowsTemplate', '<span>Tavan içi proje bulunamadı.</span>');
                grdMevcut.setGridOption('rowData', m.satirlar);
                grdYeni.setGridOption('columnDefs', yeniSutunlar(o.yil));
                grdYeni.setGridOption('overlayNoRowsTemplate', '<span>Yeni proje yok.</span>');
                var yeniler = y.satirlar || [];
                // Eski sayfa gibi: hiç yeni proje yoksa ve girilebiliyorsa boş bir satır açılır.
                if (!yeniler.length && yeniGirilebilir()) yeniler = [bosProje()];
                grdYeni.setGridOption('rowData', yeniler);
                dugmeler();
            })
            .fail(hataGoster)
            .always(function () {
                $d.prop('disabled', false);
                grdMevcut.setGridOption('loading', false);
                grdYeni.setGridOption('loading', false);
            });
    }

    $('#frmSecim').on('submit', function (olay) { olay.preventDefault(); oku(); });

    // Seçim değişince açık liste kaydedilemez (Kaydet okunan ölçüte yazar).
    function secimDegisti(geriAl) {
        if (acik && !degisiklikVarsaSor('Seçimi değiştirirseniz değişiklikler')) { geriAl(); return false; }
        bosalt();
        return true;
    }

    $('#ddlSektor').on('change', function () { var s = this; secimDegisti(function () { $(s).val(acik.sektor); }); });
    $('#ddlSenaryo').on('change', function () { var s = this; secimDegisti(function () { $(s).val(acik.senaryo); }); });

    // Sektörler ve kurum ağacı yıla bağlıdır.
    $('#txtYil').on('change', function () {
        var s = this;
        if (!secimDegisti(function () { s.value = acik.yil; })) return;
        var yil = yilOku();
        if (yil < 2000) return;
        var sektor = $('#ddlSektor').val();
        Sgb.al('api/projegerekce/sektorler', { yil: yil })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                secenekler($('#ddlSektor'), v.sektorler, sektor);
            })
            .fail(hataGoster);
        if (agac && agacYil !== yil) { agacYil = yil; agac.yenile(); }
    });

    // ---------------------------------------------------------------- kurum ağacı (eski açılır ağaç)

    var dlgKurum = document.getElementById('dlgKurum');
    var agac = null, agacYil = 0;

    function kurumSec(kod, ad) {
        var eski = kurumKod, eskiAd = $('#txtKurum').val();
        kurumKod = kod;
        $('#txtKurum').val(ad);
        secimDegisti(function () { kurumKod = eski; $('#txtKurum').val(eskiAd); });
    }

    $('#btnKurumSec').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        dlgKurum.showModal();
        if (!agac) {
            // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
            agacYil = yil;
            agac = Sgb.agac('#agcKurum', {
                yukle: function (ust) {
                    return Sgb.al('api/projegerekce/kurumlar', { yil: agacYil, ust: ust ? ust.kod : '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    dlgKurum.close();
                    if (d.kod !== kurumKod) kurumSec(d.kod, d.ad);
                },
                seciliMi: function (d) { return d.kod === kurumKod; },
                ilkAcik: true,
                bosMetin: 'Kod bulunamadı.',
                hata: hataGoster
            });
        } else if (agacYil !== yil) {
            agacYil = yil;
            agac.yenile();
        }
    });

    $('#btnKurumTemizle').on('click', function () { if (kurumKod) kurumSec('', ''); });

    // ---------------------------------------------------------------- sekmeler

    $('.proje-gerekce').on('click', '[role="tab"]', function () {
        var $s = $(this);
        $('.proje-gerekce [role="tab"]').each(function () {
            var secili = this === $s[0];
            $(this).attr('aria-selected', String(secili));
            $('#' + $(this).attr('aria-controls')).prop('hidden', !secili);
        });
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

    // ---------------------------------------------------------------- kaydet

    function sonucGoster(s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); }

    $('#btnMevcutKaydet').on('click', function () {
        if (!acik) return;
        grdMevcut.stopEditing();
        sor('Onay', 'Proje gerekçeleri kaydedilsin mi?', 'Kaydet', function () {
            var satirlar = [];
            grdMevcut.forEachNode(function (n) { satirlar.push({ kod: n.data.kod, gerekce: n.data.gerekce || '' }); });
            var $d = $('#btnMevcutKaydet').prop('disabled', true);
            Sgb.gonder('api/projegerekce/mevcut-kaydet', $.extend({ satirlar: satirlar }, acik))
                .done(function (s) { sonucGoster(s); if (s.basarili) kirliMevcut = false; })
                .fail(hataGoster)
                .always(dugmeler);
        });
    });

    $('#btnYeniKaydet').on('click', function () {
        if (!yeniGirilebilir()) return;
        grdYeni.stopEditing();
        sor('Onay', 'Yeni projeler kaydedilsin mi? Birim ve sektörün kayıtlı yeni projeleri bu tablodakilerle değiştirilecek.', 'Kaydet', function () {
            var satirlar = [];
            grdYeni.forEachNode(function (n) {
                var d = n.data;
                satirlar.push({
                    ad: d.ad || '', bitisYil: Number(d.bitisYil) || 0, projeTutari: Number(d.projeTutari) || 0,
                    yiliTeklifi0: Number(d.yiliTeklifi0) || 0, yiliTeklifi1: Number(d.yiliTeklifi1) || 0, yiliTeklifi2: Number(d.yiliTeklifi2) || 0,
                    gerekce: d.gerekce || ''
                });
            });
            var $d = $('#btnYeniKaydet').prop('disabled', true);
            Sgb.gonder('api/projegerekce/yeni-kaydet', { yil: acik.yil, kur: acik.kur, sektor: acik.sektor, satirlar: satirlar })
                .done(function (s) { sonucGoster(s); if (s.basarili) kirliYeni = false; })
                .fail(hataGoster)
                .always(dugmeler);
        });
    });

    // Satır Aç / Satır Sil (eski addRecord / deleteSelected)
    $('#btnSatirAc').on('click', function () {
        if (!yeniGirilebilir()) return;
        grdYeni.stopEditing();
        var r = grdYeni.applyTransaction({ add: [bosProje()] });
        kirliYeni = true;
        yeniSiraYenile();
        dugmeler();
        if (r && r.add && r.add[0]) {
            grdYeni.ensureIndexVisible(r.add[0].rowIndex);
            grdYeni.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: 'ad' });
        }
    });

    $('#btnSatirSil').on('click', function () {
        if (!yeniGirilebilir()) return;
        grdYeni.stopEditing();
        var secili = grdYeni.getSelectedRows();
        if (!secili.length) { mesaj('Silinecek satırları işaretleyiniz.', 'warning'); return; }
        grdYeni.applyTransaction({ remove: secili });
        kirliYeni = true;
        yeniSiraYenile();
        dugmeler();
    });

    // ---------------------------------------------------------------- kopyala (eski CopyStore)

    // Tablo (başlıklarla) sekmeyle ayrılmış metin olarak panoya; Excel'e yapıştırılabilir.
    function kopyala(grid) {
        grid.stopEditing();
        var sutun = grid.getAllDisplayedColumns().filter(function (c) { return c.getColDef().headerName; });
        var satirlar = [sutun.map(function (c) { return c.getColDef().headerName; }).join('\t')];
        grid.forEachNodeAfterFilterAndSort(function (n) {
            satirlar.push(sutun.map(function (c) {
                var v = grid.getCellValue({ rowNode: n, colKey: c, useFormatter: true });
                return String(v == null ? '' : v).replace(/[\t\r\n]+/g, ' ');
            }).join('\t'));
        });
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
    }

    $('#btnMevcutKopyala').on('click', function () { kopyala(grdMevcut); });
    $('#btnYeniKopyala').on('click', function () { kopyala(grdYeni); });

    // ---------------------------------------------------------------- raporlar ve onay

    $('.proje-arac').on('click', '[data-rapor]', function () {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/projegerekce/rapor', $.extend({ tur: $d.attr('data-rapor') }, o))
            .then(function () { $d.prop('disabled', false); });
    });

    function onayKaydet(onayla) {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        Sgb.gonder('api/projegerekce/onay', { yil: yil, onayla: onayla })
            .done(sonucGoster)
            .fail(hataGoster);
    }

    $('#btnOnayla').on('click', function () {
        sor('Onay', yilOku() + ' yılı proje gerekçeleri onaylanacak ve birimlerin veri girişine kapanacak. Devam edilsin mi?', 'Onayla',
            function () { onayKaydet(true); });
    });

    $('#btnOnayKaldir').on('click', function () {
        sor('Onay', yilOku() + ' yılı proje gerekçelerinin onayı kaldırılacak ve veri girişine açılacak. Devam edilsin mi?', 'Onayı Kaldır',
            function () { onayKaydet(false); });
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirliMevcut || kirliYeni) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/projegerekce/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            sistemYoneticisi = !!t.sistemYoneticisi;
            $('#txtYil').val(t.yil);
            secenekler($('#ddlSektor'), t.sektorler);
            if (sistemYoneticisi) secenekler($('#ddlSenaryo'), t.senaryolar, t.senaryo);
            $('#alanSenaryo').prop('hidden', !sistemYoneticisi);
            $('[data-yonetici]').prop('hidden', !sistemYoneticisi);
            grdMevcut.setGridOption('columnDefs', mevcutSutunlar(t.yil));
            grdYeni.setGridOption('columnDefs', yeniSutunlar(t.yil));
        })
        .fail(hataGoster);

    dugmeler();
});
