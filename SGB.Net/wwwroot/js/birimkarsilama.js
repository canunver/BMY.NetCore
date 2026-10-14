// Proje Teklifi Talep Karşılama (ButceHazirlikv2/BirimButceKarsilamaForm): seçilen taşra tertibi (eko 2 düzeyinde) için illerin
// iki yıl önceki harcaması, önceki yılın masraf planı, yılın talebi ve karşılananı. İl satırı açılınca ilin saymanlık / birimleri
// altına gelir (eski satır açıcı). Karşılanan sütununa yazılan tutar → tuşuyla harcamaya ya da talebe göre dağıtılır: il satırında
// ilin bütün birimlerine ve tertiplerine, birim satırında birimin tertiplerine. Birim satırındaki Detay tuşu birimin tertiplerini
// açar; orada eko 4 tertiplerinin karşılananı girilip kaydedilir, eko 2 toplam satırı değişikliği hemen gösterir.
$(function () {
    'use strict';

    if (!$('#grdIller').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var SAYILAR = ['harcama', 'planlanan', 'talepEdilen', 'karsilanan'];
    var acik = null;          // okunmuş ölçüt { yil, tertip, bosGizle }: dağıtma ve detay bunu kullanır
    var iller = [];           // il satırları
    var birimler = {};        // il -> birim satırları (açılınca okunur)
    var acikIller = {};       // il -> true: altı açık iller
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }

    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }

    function alanKod(alan) { return kodAl($('.kod-alan[data-alan="' + alan + '"] input').val()); }

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function paraYaz(p) { return p.value == null || p.value === '' ? '' : para.format(p.value); }

    function sayiSutun(alan, baslik, ek) {
        return $.extend({ field: alan, headerName: baslik, type: 'sayi', width: 130, sortable: false, valueFormatter: paraYaz, headerClass: 'sayi' }, ek || {});
    }

    var giris = {
        cellEditor: 'agTextCellEditor',
        valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; }
    };

    function toplamSatiri(satirlar, ek) {
        var t = $.extend({}, ek);
        $.each(SAYILAR, function (i, a) { t[a] = 0; });
        $.each(satirlar, function (i, s) { $.each(SAYILAR, function (j, a) { t[a] += Number(s[a]) || 0; }); });
        return t;
    }

    // ---------------------------------------------------------------- iller tablosu

    function ilMi(p) { return !!p.data && p.data.tip === 'il'; }
    function birimMi(p) { return !!p.data && p.data.tip === 'birim'; }

    function sutunlar(yil) {
        return [
            { colId: 'ac', headerName: '', width: 44, pinned: 'left', sortable: false, resizable: false,
                cellRenderer: function (p) {
                    if (!ilMi(p)) return '';
                    var a = !!acikIller[p.data.il];
                    return '<button type="button" class="btn btn-link btn-xs satir-ac" data-is="ac" aria-expanded="' + a + '" title="' + (a ? 'Birimleri kapat' : 'Birimleri göster') + '">'
                        + '<span class="glyphicon glyphicon-' + (a ? 'minus' : 'plus') + '" aria-hidden="true"></span></button>';
                } },
            { colId: 'kod', headerName: 'İl / Birim', width: 100, pinned: 'left', sortable: false,
                valueGetter: function (p) {
                    if (!p.data || p.node.rowPinned) return '';
                    return p.data.tip === 'il' ? ('0' + p.data.il).slice(-2) : p.data.saymanlik + '-' + p.data.birim;
                } },
            { field: 'ad', headerName: 'Ad', width: 300, pinned: 'left', sortable: false, tooltipField: 'ad' },
            sayiSutun('harcama', (yil - 2) + ' Harcama'),
            sayiSutun('planlanan', (yil - 1) + ' Masraf Planı'),
            sayiSutun('talepEdilen', yil + ' Talep'),
            sayiSutun('karsilanan', yil + ' Karşılanan', $.extend({ editable: function (p) { return !p.node.rowPinned; } }, giris)),
            { colId: 'islem', headerName: '', width: 84, sortable: false, resizable: false,
                cellRenderer: function (p) {
                    if (p.node.rowPinned || !p.data) return '';
                    var h = '<button type="button" class="btn btn-default btn-xs" data-is="oranla" title="'
                        + (ilMi(p) ? 'Girilen tutarı ilin bütün alt birimlerine ve tertiplerine dağıtmak için basınız' : 'Girilen tutarı birimin tertiplerine dağıtmak için basınız')
                        + '"><span class="glyphicon glyphicon-arrow-right" aria-hidden="true"></span><span class="sr-only">Dağıt</span></button>';
                    if (birimMi(p))
                        h += ' <button type="button" class="btn btn-default btn-xs" data-is="detay" title="Birimin tertiplerini göster"><span class="glyphicon glyphicon-folder-open" aria-hidden="true"></span><span class="sr-only">Detay</span></button>';
                    return h;
                } }
        ];
    }

    var grid = Sgb.grid('#grdIller', {
        columnDefs: sutunlar(new Date().getFullYear()),
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        getRowId: function (p) { return p.data.id; },
        getRowClass: function (p) { return birimMi(p) ? 'alt-satir' : (ilMi(p) ? 'il-satir' : ''); },
        overlayNoRowsTemplate: '<span>Kodları seçip tertip listesini yükleyiniz, tertibi seçip Oku\'ya basınız.</span>',
        onCellClicked: function (e) {
            var $b = $(e.event && e.event.target).closest('[data-is]');
            if (!$b.length || !e.data) return;
            var is = $b.attr('data-is');
            if (is === 'ac') ilAcKapat(e.data);
            else if (is === 'oranla') oranla(e.data);
            else if (is === 'detay') detayAc(e.data);
        }
    });

    // Görünen satırlar: iller ve açık illerin altında birimleri. Girilen karşılananlar satır nesnelerinde kalır.
    function tabloyuDoldur() {
        var satirlar = [];
        $.each(iller, function (i, il) {
            satirlar.push(il);
            if (acikIller[il.il] && birimler[il.il]) Array.prototype.push.apply(satirlar, birimler[il.il]);
        });
        grid.setGridOption('rowData', satirlar);
        grid.setGridOption('pinnedTopRowData', iller.length ? [toplamSatiri(iller, { id: 'toplam', ad: 'T O P L A M' })] : []);
        grid.refreshCells({ columns: ['ac'], force: true });
    }

    function birimleriOku(il) {
        return Sgb.al('api/birimkarsilama/birimler', { yil: acik.yil, tertip: acik.tertip, il: il, bosGizle: acik.bosGizle })
            .then(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return false; }
                birimler[il] = $.map(v.satirlar, function (s) { return $.extend(s, { tip: 'birim', id: 'b' + s.saymanlik + '-' + s.birim }); });
                return true;
            }, function (xhr) { hataGoster(xhr); return false; });
    }

    function ilAcKapat(satir) {
        var il = satir.il;
        if (acikIller[il]) { delete acikIller[il]; tabloyuDoldur(); return; }
        if (birimler[il]) { acikIller[il] = true; tabloyuDoldur(); return; }
        grid.setGridOption('loading', true);
        birimleriOku(il).then(function (tamam) {
            grid.setGridOption('loading', false);
            if (tamam) { acikIller[il] = true; tabloyuDoldur(); }
        });
    }

    function listeyiBosalt() {
        acik = null;
        iller = [];
        birimler = {};
        acikIller = {};
        tabloyuDoldur();
    }

    // Oku (eski btnOku_Click). yenile: dağıtma / kayıttan sonra; açık illerin birimleri de yeniden okunur.
    function oku(yenile) {
        var o = yenile && acik ? acik : { yil: yilOku(), tertip: $('#ddlTertip').val() || '', bosGizle: $('#chkBosGizle').prop('checked') };
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!o.tertip) { mesaj('Lütfen tertip listesinden tertip seçiniz, tertip listesi boş ise yanındaki yenile tuşuna basınız.', 'warning'); return; }
        var acilacak = yenile ? Object.keys(acikIller) : [];
        var $d = $('#btnOku').prop('disabled', true);
        grid.stopEditing(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/birimkarsilama/iller', o)
            .then(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = o;
                iller = $.map(v.satirlar, function (s) { return $.extend(s, { tip: 'il', id: 'i' + s.il }); });
                birimler = {};
                acikIller = {};
                grid.setGridOption('columnDefs', sutunlar(o.yil));
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                return $.when.apply($, $.map(acilacak, function (il) {
                    return birimleriOku(il).then(function (tamam) { if (tamam) acikIller[il] = true; });
                })).then(tabloyuDoldur);
            }, hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#btnOku').on('click', function () { oku(false); });

    // Ölçüt değişince eski liste kullanılmaz (dağıtma listelenen tertibe yazar).
    $('#ddlTertip, #chkBosGizle').on('change', function () { if (acik) listeyiBosalt(); });

    // Tertip listesi (eski btnTertipYukle_Click): seçilen kodlara uyan eko 2 tertipleri.
    $('#btnTertipYukle').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        var secili = $('#ddlTertip').val();
        Sgb.al('api/birimkarsilama/tertipler', { yil: yil, kur: alanKod('kur'), fon: alanKod('fon'), fin: alanKod('fin'), eko: alanKod('eko'), peb: alanKod('peb') })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var $ddl = $('#ddlTertip').empty();
                $('<option></option>').val('').text(v.satirlar.length ? 'Tertip seçiniz (' + v.satirlar.length + ')' : 'Seçilen kodlara uyan tertip yok').appendTo($ddl);
                $.each(v.satirlar, function (i, t) { $('<option></option>').val(t.tertip).text(t.ad).attr('title', t.ad).appendTo($ddl); });
                if (secili && $ddl.find('option').filter(function () { return this.value === secili; }).length) $ddl.val(secili);
                else if (v.satirlar.length === 1) $ddl.prop('selectedIndex', 1);
                if ($ddl.val() !== secili) $ddl.trigger('change');
                if (!v.satirlar.length) mesaj('Seçilen kodlara uyan tertip bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // Yıl değişince tertip listesi ve tablo boşaltılır; kod ağaçları bir sonraki açılışta yeni yıla göre okunur.
    $('#txtYil').on('change', function () {
        $('#ddlTertip').empty().append($('<option></option>').val('').text('Kodları seçip yenile tuşuna basınız'));
        listeyiBosalt();
    });

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;

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

    // ---------------------------------------------------------------- dağıtma (eski DetayOranla)

    function oranla(satir) {
        if (!acik) return;
        grid.stopEditing();
        var il = satir.tip === 'il';
        var yontem = $('#ddlDagit').val();
        var ad = (il ? ('0' + satir.il).slice(-2) : satir.saymanlik + '-' + satir.birim) + ' ' + (satir.ad || '');
        var metin = ad + ' için karşılanan ' + para.format(Number(satir.karsilanan) || 0) + ' TL, '
            + (yontem === '2' ? 'talebe' : 'harcamaya') + ' göre ' + (il ? 'ilin bütün birimlerine ve tertiplerine' : 'birimin tertiplerine')
            + ' dağıtılacak. Var olan karşılananların yerine yazılır. Devam edilsin mi?';
        sor('Karşılananı Dağıt', metin, 'Dağıt', function () {
            Sgb.gonder('api/birimkarsilama/oranla', {
                yil: acik.yil, tertip: acik.tertip,
                il: il ? satir.il : -1, saymanlik: il ? -1 : satir.saymanlik, birim: il ? '' : satir.birim,
                karsilanan: Number(satir.karsilanan) || 0, yontem: parseInt(yontem, 10) || 1
            })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) oku(true);
                })
                .fail(hataGoster);
        });
    }

    // ---------------------------------------------------------------- birimin tertipleri (eski Detay penceresi)

    var dlgDetay = document.getElementById('dlgDetay');
    var detay = null;         // { yil, saymanlik, birim }
    var degisen = {};         // tertipTam -> true

    function detayMi(p) { return !!p.data && !p.node.rowPinned && !!p.data.tertipTam; }
    function detayGirilir(p) { return detayMi(p) && p.colDef.field === 'karsilanan'; }

    function detayHucre(p) {
        var s = [];
        if (p.colDef.type === 'sayi') s.push('sayi');
        if (p.node.rowPinned) return s;
        if (!detayMi(p)) s.push('eko2');
        else s.push(detayGirilir(p) ? 'girilir' : 'kapali');
        return s;
    }

    function detaySutunlar(yil) {
        function sayi(alan, baslik, ek) { return sayiSutun(alan, baslik, $.extend({ width: 120, cellClass: detayHucre }, ek || {})); }
        return [
            { field: 'tertip', headerName: 'Tertip', width: 250, sortable: false, cellClass: detayHucre, tooltipField: 'tertip' },
            { field: 'ekoad', headerName: 'Ad', width: 240, sortable: false, cellClass: detayHucre, tooltipField: 'ekoad' },
            sayi('harcama', (yil - 2) + ' Harcama'),
            sayi('planlanan', (yil - 1) + ' Masraf Planı'),
            sayi('talepEdilen', yil + ' Talep'),
            sayi('karsilanan', yil + ' Karşılanan', $.extend({ editable: detayGirilir }, giris))
        ];
    }

    function detayToplam() {
        var ust = [];
        detayGrid.forEachNode(function (n) { if (!n.data.tertipTam) ust.push(n.data); });
        detayGrid.setGridOption('pinnedTopRowData', ust.length ? [toplamSatiri(ust, { tertip: 'T O P L A M' })] : []);
    }

    var detayGrid = Sgb.grid('#grdDetay', {
        columnDefs: detaySutunlar(new Date().getFullYear()),
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        getRowId: function (p) { return p.data.tertip; },
        overlayNoRowsTemplate: '<span>Tertip bulunamadı.</span>',
        // Eko 4 karşılananı değişince üstündeki eko 2 toplam satırı farkla güncellenir (eski KolonDegisti).
        onCellValueChanged: function (e) {
            if (e.node.rowPinned || !e.data.tertipTam || e.colDef.field !== 'karsilanan' || e.newValue === e.oldValue) return;
            degisen[e.data.tertipTam] = true;
            var fark = (Number(e.newValue) || 0) - (Number(e.oldValue) || 0);
            detayGrid.forEachNode(function (n) {
                if (!n.data.tertipTam && n.data.tertipEko2 === e.data.tertipEko2) n.setDataValue('karsilanan', (Number(n.data.karsilanan) || 0) + fark);
            });
            detayToplam();
            $('#btnKaydet').prop('disabled', false);
        }
    });

    function detayKirli() { return Object.keys(degisen).length > 0; }

    function detayAc(satir) {
        if (!acik) return;
        grid.stopEditing();
        detay = { yil: acik.yil, saymanlik: satir.saymanlik, birim: satir.birim };
        degisen = {};
        $('#btnKaydet').prop('disabled', true);
        $('#dtyYil').text(acik.yil);
        $('#dtySaymanlik').text(satir.saymanlik);
        $('#dtyBirim').text(satir.birim + (satir.ad ? ' - ' + satir.ad : ''));
        detayGrid.setGridOption('columnDefs', detaySutunlar(acik.yil));
        detayGrid.setGridOption('rowData', []);
        detayGrid.setGridOption('pinnedTopRowData', []);
        detayGrid.setGridOption('loading', true);
        dlgDetay.showModal();
        Sgb.al('api/birimkarsilama/detay', { yil: acik.yil, tertip: acik.tertip, saymanlik: satir.saymanlik, birim: satir.birim, bosGizle: acik.bosGizle })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                detayGrid.setGridOption('rowData', v.satirlar);
                detayToplam();
            })
            .fail(hataGoster)
            .always(function () { detayGrid.setGridOption('loading', false); });
    }

    function detayKapatilabilir() {
        return !detayKirli() || window.confirm('Kaydedilmemiş değişiklikler var. Kapatırsanız kaybolacak. Devam edilsin mi?');
    }

    $('dialog').on('click', '[data-kapat]', function () {
        var d = $(this).closest('dialog')[0];
        if (d === dlgDetay) { detayGrid.stopEditing(); if (!detayKapatilabilir()) return; degisen = {}; }
        d.close();
    });
    dlgDetay.addEventListener('cancel', function (e) {
        detayGrid.stopEditing();
        if (!detayKapatilabilir()) e.preventDefault();
        else degisen = {};
    });

    // Kaydet (eski btnKaydet_Click): yalnızca değişen eko 4 satırları gönderilir.
    $('#btnKaydet').on('click', function () {
        if (!detay) return;
        detayGrid.stopEditing();
        if (!detayKirli()) { mesaj('Kaydedilecek değişiklik yok.', 'info'); return; }
        sor('Onay', 'Kaydetmek istiyor musunuz?', 'Kaydet', function () {
            var satirlar = [];
            detayGrid.forEachNode(function (n) {
                if (n.data.tertipTam && degisen[n.data.tertipTam]) satirlar.push({ tertipTam: n.data.tertipTam, karsilanan: Number(n.data.karsilanan) || 0 });
            });
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder('api/birimkarsilama/kaydet', { yil: detay.yil, saymanlik: detay.saymanlik, birim: detay.birim, satirlar: satirlar })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (!s.basarili) { $d.prop('disabled', false); return; }
                    degisen = {};
                    oku(true); // birimin ve ilin karşılananı değişti
                })
                .fail(function (xhr) { $d.prop('disabled', false); hataGoster(xhr); });
        });
    });

    window.addEventListener('beforeunload', function (e) {
        if (dlgDetay.open && detayKirli()) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- kod ağaçları (eski açılır ağaçlar)

    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};  // tür -> { $ul, agac, yil }
    var aktifAlan = null;

    function agacAc($alan) {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var tur = $alan.attr('data-tur');
        aktifAlan = $alan;
        $('#dlgKodBaslik').text($alan.find('label').text() + ' Kod Seçimi');
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[tur];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', $alan.find('label').text() + ' kod ağacı').appendTo('#agacKutu'), yil: yil };
            // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al('api/birimkarsilama/kodlar', { yil: a.yil, tur: tur, ust: ust ? ust.kod : '' }).then(function (v) {
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

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/birimkarsilama/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            $('#txtYil').val(t.yil);
            grid.setGridOption('columnDefs', sutunlar(t.yil));
        })
        .fail(hataGoster);
});
