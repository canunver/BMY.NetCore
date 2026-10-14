// Yolluk Planlama Formu (ButceHazirlikv2/YollukPlanlamaForm): seçilen faaliyetin yolluk tertiplerine (06.9.2.01 / 03.3.1 yurt içi,
// 06.9.2.03 / 03.3.3 yurt dışı) eğitim / toplantı planları girilir. Faaliyet seçilince tertipler ödenekleriyle listelenir; Oku
// tertibin planını getirir. Muhtelif olmayan satırda süre ve kişi sayısı girilince maliyet yolluk değerlerinden hesaplanır
// (eski MaliyetHesapla), muhtelif satırda maliyet elle girilir. Maliyet toplamı tertibin ödeneğine eşit olmalıdır (sunucu denetler).
// Yolluk değerlerini saklama ve birimlerin onay durumu (Onaylama) yalnızca sistem yöneticisinindir; onaylı birim kaydedemez.
$(function () {
    'use strict';

    if (!$('#grdYolluk').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var ONAYLI = 5;
    var sistemYoneticisi = false;
    var acik = null;     // okunmuş ölçüt { yil, senaryo, tertip }: satır ekleme ve kayıt bunu kullanır
    var durum = -1;      // faaliyetin onay durumu: 5 onaylı, 0 / 1 açık, -1 faaliyet seçilmemiş
    var kirli = false;   // kaydedilmemiş değişiklik var mı
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var tamsayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function senaryoOku() { return $('#ddlSenaryo').val() || ''; }
    function pebOku() { return $.trim($('#txtPeb').val()); }
    function onayli() { return durum === ONAYLI; }

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

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

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- yolluk değerleri (eski wndDegerler)

    var dlgDegerler = document.getElementById('dlgDegerler');
    var DEGER_ALAN = { yiYolculuk: '#txtYIYolculuk', yiGundelik: '#txtYIGundelik', ydYolculuk: '#txtYDYolculuk', ydGundelik: '#txtYDGundelik' };

    function degerAl(ad) { return sayiCoz($(DEGER_ALAN[ad]).val()) || 0; }

    // Yılın değerleri (eski RakamlariOku). Değerler pencerede değiştirilirse hesap onlarla yapılır (eskisi gibi).
    function degerleriOku() {
        var yil = yilOku();
        if (yil < 2000) return;
        Sgb.al('api/yolluk/degerler', { yil: yil })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $.each(DEGER_ALAN, function (ad, alan) { $(alan).val(para.format(Number(v[ad]) || 0)); });
            })
            .fail(hataGoster);
    }

    $('#btnDegerler').on('click', function () { dlgDegerler.showModal(); });

    $('.sayi-giris').on('change', function () {
        var n = sayiCoz($(this).val());
        $(this).val(para.format(n === null ? 0 : n));
    });

    // Kaydet (eski btnDegerKaydet_Click): yalnızca sistem yöneticisi.
    $('#btnDegerKaydet').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        sor('Onay', yil + ' yılı yolluk değerleri kaydedilsin mi?', 'Kaydet', function () {
            var $d = $('#btnDegerKaydet').prop('disabled', true);
            Sgb.gonder('api/yolluk/deger-kaydet', {
                yil: yil, yiYolculuk: degerAl('yiYolculuk'), yiGundelik: degerAl('yiGundelik'), ydYolculuk: degerAl('ydYolculuk'), ydGundelik: degerAl('ydGundelik')
            })
                .done(function (s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // Maliyet (eski MaliyetHesapla / OrtalamaMaliyet): ((süre + 1) x gündelik + yolculuk) x kişi; yurt içi / dışı tertibin eko kodundan.
    function maliyetHesapla(gun, kisi) {
        var tertip = $('#ddlTertip').val() || '';
        if (!tertip) return 0;
        var eko = (tertip.split('-')[3] || '').replace(/\./g, '');
        if (!eko) return 0;
        var g = 0, y = 0;
        function bas(onekler) { return onekler.some(function (o) { return eko.indexOf(o) === 0; }); }
        if (bas(['069201', '0331', '06092001', '030310'])) { g = degerAl('yiGundelik'); y = degerAl('yiYolculuk'); }
        else if (bas(['069203', '0333', '06092003', '030330'])) { g = degerAl('ydGundelik'); y = degerAl('ydYolculuk'); }
        return (((Number(gun) || 0) + 1) * g + y) * (Number(kisi) || 0);
    }

    // ---------------------------------------------------------------- yolluk planı tablosu

    // Muhtelif satırda süre, kişi, ay ve yer girilmez, maliyet girilir; değilse tersi (eski BeforeEditPar).
    // Onaylı birimin planı değiştirilemez.
    function girilir(alan, veri) {
        if (!veri || onayli()) return false;
        if (alan === 'suresi' || alan === 'kisiSayisi' || alan === 'yapilacagiAy' || alan === 'yeri') return !veri.muhtelifMi;
        if (alan === 'maliyet') return !!veri.muhtelifMi;
        return true;
    }

    function hucreSinif(p) {
        var s = [];
        if (p.colDef.type === 'sayi') s.push('sayi');
        s.push(girilir(p.colDef.field, p.data) ? 'girilir' : 'kapali');
        return s;
    }

    function duzenlenir(p) { return girilir(p.colDef.field, p.data); }

    function metinSutun(alan, baslik, genislik) {
        return { field: alan, headerName: baslik, width: genislik, sortable: false, editable: duzenlenir, cellClass: hucreSinif, tooltipField: alan, cellEditor: 'agTextCellEditor' };
    }

    function sayiSutun(alan, baslik, genislik, kesirli) {
        return {
            field: alan, headerName: baslik, width: genislik, type: 'sayi', headerClass: 'sayi', sortable: false,
            editable: duzenlenir, cellClass: hucreSinif, cellEditor: 'agTextCellEditor',
            valueFormatter: function (p) { return p.value == null || p.value === '' ? '' : (kesirli ? para : tamsayi).format(p.value); },
            valueParser: function (p) {
                var n = sayiCoz(p.newValue);
                if (n === null) return p.oldValue;
                return kesirli ? Math.round(n * 100) / 100 : Math.max(0, Math.round(n));
            }
        };
    }

    var grid = Sgb.grid('#grdYolluk', {
        columnTypes: { sayi: {} },
        columnDefs: [
            { colId: 'sira', headerName: '#', width: 60, sortable: false, resizable: false, valueGetter: function (p) { return p.node.rowIndex + 1; } },
            metinSutun('dairesi', 'Düzenleyecek Daire', 220),
            metinSutun('konusu', 'Eğitim / Toplantının Konusu', 260),
            { field: 'muhtelifMi', headerName: 'Muhtelif', width: 90, sortable: false, editable: duzenlenir, cellClass: hucreSinif,
                cellRenderer: 'agCheckboxCellRenderer', cellEditor: 'agCheckboxCellEditor' },
            sayiSutun('suresi', 'Süresi (Gün)', 100, false),
            sayiSutun('kisiSayisi', 'Kişi Sayısı', 100, false),
            metinSutun('yapilacagiAy', 'Yapılacağı Ay', 150),
            metinSutun('yeri', 'Yeri', 160),
            sayiSutun('maliyet', 'Maliyeti', 150, true)
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        overlayNoRowsTemplate: '<span>Yıl, aşama ve faaliyeti seçip tertibi seçiniz, Oku\'ya basınız.</span>',
        // Süre / kişi değişince maliyet hesaplanır, maliyet değişince toplam yenilenir (eski AfterEdit); muhtelif değişince renkler.
        onCellValueChanged: function (e) {
            if (e.newValue === e.oldValue) return;
            kirli = true;
            var alan = e.colDef.field;
            if (alan === 'suresi' || alan === 'kisiSayisi')
                e.node.setDataValue('maliyet', Math.round(maliyetHesapla(e.data.suresi, e.data.kisiSayisi) * 100) / 100);
            else if (alan === 'muhtelifMi')
                grid.refreshCells({ rowNodes: [e.node], force: true });
            toplamHesapla();
        }
    });

    function satirlar() {
        var liste = [];
        grid.forEachNode(function (n) { liste.push(n.data); });
        return liste;
    }

    // Maliyet toplamı (eski ToplamHesapla).
    function toplamHesapla() {
        var t = 0;
        $.each(satirlar(), function (i, s) { t += Number(s.maliyet) || 0; });
        $('#txtToplam').val(para.format(t));
    }

    function tuslariAyarla() {
        var tertip = $('#ddlTertip').val() || '';
        $('#btnKaydet').prop('disabled', !acik || !acik.tertip || acik.tertip !== tertip || onayli());
        $('#btnSatirEkle, #btnSatirSil').prop('disabled', !acik || onayli());
    }

    function durumGoster() {
        var $e = $('#durumEtiket');
        if (durum < 0) $e.prop('hidden', true);
        else $e.prop('hidden', false).toggleClass('label-success', onayli()).toggleClass('label-default', !onayli())
            .text(onayli() ? 'Onaylı' : 'Açık').attr('title', onayli() ? 'Birimin yolluk planı onaylanmış, değiştirilemez.' : 'Birimin yolluk planı değiştirilebilir.');
        tuslariAyarla();
        grid.refreshCells({ force: true });
    }

    function listeyiBosalt() {
        acik = null;
        kirli = false;
        grid.setGridOption('rowData', []);
        grid.setGridOption('overlayNoRowsTemplate', '<span>Yıl, aşama ve faaliyeti seçip tertibi seçiniz, Oku\'ya basınız.</span>');
        toplamHesapla();
        tuslariAyarla();
    }

    // Oku (eski btnOku_Click). Tertip boşsa eskisi gibi tertip süzülmeden okunur (bu durumda kaydedilemez).
    function oku() {
        var o = { yil: yilOku(), senaryo: senaryoOku(), tertip: $('#ddlTertip').val() || '' };
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!o.senaryo) { mesaj('Lütfen aşama seçiniz!', 'warning'); return; }
        var $d = $('#btnOku').prop('disabled', true);
        grid.stopEditing(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/yolluk/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = o;
                kirli = false;
                grid.setGridOption('rowData', v.satirlar);
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt yok. Ekle tuşuyla satır ekleyebilirsiniz.</span>');
                toplamHesapla();
                tuslariAyarla();
                if (!o.tertip) mesaj('Tertip seçilmeden bütün tertipler okundu; kaydetmek için tertip seçip yeniden okuyunuz.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#btnOku').on('click', function () {
        if (kirli && !window.confirm('Kaydedilmemiş değişiklikler var. Yeniden okursanız kaybolacak. Devam edilsin mi?')) return;
        oku();
    });

    // Satır ekle / sil (eski addRecord / deleteSelected); kayıt Kaydet ile yapılır.
    $('#btnSatirEkle').on('click', function () {
        if (!acik) return;
        grid.stopEditing();
        var r = grid.applyTransaction({ add: [{ dairesi: '', konusu: '', muhtelifMi: false, suresi: 0, kisiSayisi: 0, yapilacagiAy: '', yeri: '', maliyet: 0 }] });
        kirli = true;
        if (r && r.add && r.add.length) {
            grid.ensureIndexVisible(r.add[0].rowIndex, 'bottom');
            grid.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: 'dairesi' });
        }
    });

    $('#btnSatirSil').on('click', function () {
        grid.stopEditing();
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Silinecek satırları işaretleyiniz.', 'info'); return; }
        grid.applyTransaction({ remove: secili });
        grid.refreshCells({ columns: ['sira'], force: true });
        kirli = true;
        toplamHesapla();
    });

    // Kaydet (eski btnKaydet_Click): tertibin planı silinip tablodaki satırlar sırayla yazılır.
    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        var tertip = $('#ddlTertip').val() || '';
        if (!tertip) { mesaj('Lütfen tertip seçiniz!', 'warning'); return; }
        if (!acik || acik.tertip !== tertip) { mesaj('Önce seçilen tertibi okuyunuz.', 'warning'); return; }
        sor('Onay', tertip + ' tertibinin yolluk planı kaydedilsin mi? Var olan planın yerine yazılır.', 'Kaydet', function () {
            var gonderilecek = $.map(satirlar(), function (s) {
                return {
                    dairesi: s.dairesi || '', konusu: s.konusu || '', muhtelifMi: !!s.muhtelifMi,
                    suresi: Number(s.suresi) || 0, kisiSayisi: Number(s.kisiSayisi) || 0,
                    yapilacagiAy: s.yapilacagiAy || '', yeri: s.yeri || '', maliyet: Number(s.maliyet) || 0
                };
            });
            $('#btnKaydet').prop('disabled', true);
            Sgb.gonder('api/yolluk/kaydet', { yil: acik.yil, senaryo: acik.senaryo, tertip: acik.tertip, satirlar: gonderilecek })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) { kirli = false; oku(); }
                })
                .fail(hataGoster)
                .always(tuslariAyarla);
        });
    });

    // Yazdır (eski btnYazdir_Click → Rapor 2003-14): faaliyetin yolluk planı, Excel.
    $('#btnYazdir').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!senaryoOku()) { mesaj('Lütfen aşama seçiniz!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/yolluk/rapor', { yil: yil, senaryo: senaryoOku(), peb: pebOku() })
            .then(function () { $d.prop('disabled', false); });
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- faaliyet ve tertipler (eski pebDegisti)

    var faaliyetSurum = 0;

    // Faaliyetin adı, yolluk tertipleri ve onay durumu. tertipKorunsun: seçili tertip listede varsa seçili kalır.
    function faaliyetOku(tertipKorunsun) {
        var yil = yilOku();
        var surum = ++faaliyetSurum;
        var secili = tertipKorunsun ? $('#ddlTertip').val() : '';
        if (yil < 2000) return;
        Sgb.al('api/yolluk/faaliyet', { yil: yil, senaryo: senaryoOku(), peb: pebOku() })
            .done(function (v) {
                if (surum !== faaliyetSurum) return; // arada başka seçim yapıldı
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#pebAd').text(v.ad ? '(' + v.ad + ')' : '');
                var $ddl = $('#ddlTertip').empty();
                var metin = !pebOku() ? 'Faaliyet seçiniz' : (v.tertipler.length ? 'Tertip seçiniz (' + v.tertipler.length + ')' : 'Faaliyetin yolluk tertibi yok');
                $('<option></option>').val('').text(metin).appendTo($ddl);
                $.each(v.tertipler, function (i, t) { $('<option></option>').val(t.kod).text(t.ad).attr('title', t.ad).appendTo($ddl); });
                if (secili && $ddl.find('option').filter(function () { return this.value === secili; }).length) $ddl.val(secili);
                $ddl.data('onceki', $ddl.val());
                durum = v.durum;
                durumGoster();
            })
            .fail(hataGoster);
    }

    // Ölçüt değişince okunmuş liste kullanılmaz; kaydedilmemiş değişiklik varsa sorulur, vazgeçilirse eski değer geri gelir.
    function olcutDegisti($alan, is) {
        if (kirli && !window.confirm('Kaydedilmemiş değişiklikler var. Seçimi değiştirirseniz kaybolacak. Devam edilsin mi?')) {
            $alan.val($alan.data('onceki'));
            return;
        }
        $alan.data('onceki', $alan.val());
        listeyiBosalt();
        if (is) is();
    }

    $('#txtYil').on('change', function () {
        olcutDegisti($(this), function () { degerleriOku(); faaliyetOku(false); agacYenile(); });
    });
    $('#ddlSenaryo').on('change', function () { olcutDegisti($(this), function () { faaliyetOku(false); }); });
    $('#txtPeb').on('change', function () { olcutDegisti($(this), function () { faaliyetOku(false); }); });
    $('#ddlTertip').on('change', function () { olcutDegisti($(this)); });

    $('#btnPebTemizle').on('click', function () {
        if (pebOku()) $('#txtPeb').val('').trigger('change');
        $('#txtPeb').trigger('focus');
    });

    // ---------------------------------------------------------------- faaliyet kod ağacı (eski KodAgac.aspx?kodTur=14)

    var dlgKod = document.getElementById('dlgKod');
    var agac = null;
    var agacYil = 0;

    function agacYenile() {
        if (agac && agacYil !== yilOku()) { agacYil = yilOku(); agac.yenile(); }
    }

    function agacAc() {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        dlgKod.showModal();
        if (agac) { agacYenile(); return; }
        agacYil = yil; // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
        var $ul = $('<ul></ul>').attr('aria-label', 'Faaliyet kod ağacı').appendTo('#agacKutu');
        agac = Sgb.agac($ul[0], {
            yukle: function (ust) {
                return Sgb.al('api/yolluk/kodlar', { yil: agacYil, ust: ust ? ust.kod : '' }).then(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                    return v.dugumler;
                });
            },
            secilince: function (d) {
                dlgKod.close();
                if (d.kod !== pebOku()) $('#txtPeb').val(d.kod).trigger('change');
            },
            seciliMi: function (d) { return pebOku() === d.kod; },
            ilkAcik: true,
            bosMetin: 'Kod bulunamadı.',
            hata: hataGoster
        });
    }

    $('#btnPebSec').on('click', agacAc);
    // Alana kod yazılabilir; Alt+Aşağı ağacı açar.
    $('#txtPeb').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc(); }
    });

    // ---------------------------------------------------------------- birimlerin onay durumu (eski wndOnay; sistem yöneticisi)

    var dlgOnay = document.getElementById('dlgOnay');
    var onayOlcut = null; // pencere açılırken alınan { yil, senaryo }

    var onayGrid = Sgb.grid('#grdOnay', {
        columnDefs: [
            { field: 'kod', headerName: 'Kod', width: 80 },
            { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 200, tooltipField: 'ad' },
            { colId: 'durum', headerName: 'Durum', width: 90, valueGetter: function (p) { return p.data && p.data.onayli ? 'Onaylı' : 'Açık'; } },
            { colId: 'islem', headerName: '', width: 90, sortable: false, resizable: false,
                cellRenderer: function (p) {
                    if (!p.data) return '';
                    return '<button type="button" class="btn btn-default btn-xs" data-durum="5" title="Onayla (kilitle)"' + (p.data.onayli ? ' disabled' : '') + '>'
                        + '<span class="glyphicon glyphicon-lock" aria-hidden="true"></span><span class="sr-only">Onayla</span></button> '
                        + '<button type="button" class="btn btn-default btn-xs" data-durum="1" title="Onayı kaldır (aç)"' + (p.data.onayli ? '' : ' disabled') + '>'
                        + '<span class="glyphicon glyphicon-folder-open" aria-hidden="true"></span><span class="sr-only">Onayı kaldır</span></button>';
                } }
        ],
        getRowId: function (p) { return p.data.kod; },
        getRowClass: function (p) { return p.data && p.data.onayli ? 'onayli' : ''; },
        overlayNoRowsTemplate: '<span>Birim bulunamadı.</span>',
        // Tek birim (eski DurumDegistirBirim): kilit / aç simgesi.
        onCellClicked: function (e) {
            var $b = $(e.event && e.event.target).closest('[data-durum]');
            if (!$b.length || $b.prop('disabled') || !e.data) return;
            onayGonder(e.data.kod, parseInt($b.attr('data-durum'), 10));
        }
    });

    function onaylariOku() {
        onayGrid.setGridOption('loading', true);
        Sgb.al('api/yolluk/onaylar', onayOlcut)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                onayGrid.setGridOption('rowData', v.satirlar);
            })
            .fail(hataGoster)
            .always(function () { onayGrid.setGridOption('loading', false); });
    }

    function onayGonder(kod, yeniDurum) {
        Sgb.gonder('api/yolluk/onay', { yil: onayOlcut.yil, senaryo: onayOlcut.senaryo, kod: kod, durum: yeniDurum })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                onaylariOku();
                faaliyetOku(true); // seçili faaliyetin kilidi değişmiş olabilir
            })
            .fail(hataGoster);
    }

    // Hepsini Onayla / Hepsini Aç (eski btnHepsiniOnayla_Click).
    $('#dlgOnay').on('click', '[data-hepsi]', function () {
        var yeniDurum = parseInt($(this).attr('data-hepsi'), 10);
        sor('Onay', onayOlcut.yil + ' yılı ' + onayOlcut.senaryo + ' aşamasında bütün birimlerin yolluk planları '
            + (yeniDurum === ONAYLI ? 'onaylanacak (kilitlenecek)' : 'açılacak') + '. Devam edilsin mi?',
            yeniDurum === ONAYLI ? 'Hepsini Onayla' : 'Hepsini Aç', function () { onayGonder('', yeniDurum); });
    });

    $('#btnOnaylama').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!senaryoOku()) { mesaj('Lütfen aşama seçiniz!', 'warning'); return; }
        onayOlcut = { yil: yil, senaryo: senaryoOku() };
        $('#dlgOnayBaslik').text('Birimlerin Onay Durumu (' + yil + ' / ' + onayOlcut.senaryo + ')');
        onayGrid.setGridOption('rowData', []);
        dlgOnay.showModal();
        onaylariOku();
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    tuslariAyarla();
    Sgb.al('api/yolluk/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            sistemYoneticisi = !!t.sistemYoneticisi;
            $('#btnOnaylama, #btnDegerKaydet').prop('hidden', !sistemYoneticisi);
            $('#txtYil').val(t.yil).data('onceki', String(t.yil));
            var $ddl = $('#ddlSenaryo').empty();
            $.each(t.senaryolar, function (i, s) { $('<option></option>').val(s.kod).text(s.ad).appendTo($ddl); });
            $ddl.data('onceki', $ddl.val());
            $('#txtPeb').data('onceki', '');
            degerleriOku();
            faaliyetOku(false);
        })
        .fail(hataGoster);
});
