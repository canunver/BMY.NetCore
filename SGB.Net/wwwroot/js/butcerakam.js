// Bütçe teklifi / yıl sonu tahmini girişi (ButceHazirlikv2/ButceRakamDuzenleYeni): sorgu alanlarındaki kodlara göre tertipler
// listelenir, tutarlar (teklifte üç yıl, yıl sonu tahmininde bir yıl) ve açıklama girilip değişen satırlar kaydedilir. Sayfa menüdeki
// seçeneğe göre çalışır (odenekTur 1 gider, 3 gider eko 2, 4 gider eko 4, 2 gelir; tahmin 0 teklif, 1 yıl sonu tahmini, 2 tavan üstü):
// hangi kod alanlarının görüneceği, sütunlar ve yazılan tablo sunucudan gelen tanıma göre ayarlanır. En üstte TOPLAM ve işaretli
// satırların SEÇİLİ TOPLAM'ı vardır. Raporlar sunucuda eski kodla üretilir.
$(function () {
    'use strict';

    var $sayfa = $('.butce-rakam');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TUR = parseInt($sayfa.attr('data-odenek-tur'), 10) || 1;
    var TAHMIN = parseInt($sayfa.attr('data-tahmin'), 10) || 0;
    var SAKLA = 'butcerakam.'; // tarayıcıda tutulan görünüm seçimleri (Tertip Gösterim, Ek Sütunlar)

    var tanim = null;    // sayfanın açılış bilgileri (api/butcerakam/tanim)
    var acik = null;     // listelenmiş seçim ve sütun bilgileri: Kaydet / İndir / detay bunu kullanır
    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });
    var ondalik = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function saklananOku(ad, varsayilan) {
        try { var d = window.localStorage.getItem(SAKLA + ad); return d === null ? varsayilan : d; } catch (e) { return varsayilan; }
    }
    function sakla(ad, deger) {
        try { window.localStorage.setItem(SAKLA + ad, deger); } catch (e) { /* tarayıcı izin vermiyor */ }
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function ayOku() {
        var ay = parseInt($('#txtAy').val(), 10) || 0;
        return ay >= 1 && ay <= 12 ? ay : new Date().getMonth() + 1;
    }

    // "1.234.000" ya da "1234000" -> 1234000. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    // Katsayı: "1,0500" ya da "1.05" -> 1.05.
    function katsayiCoz(metin) {
        var s = String(metin == null ? '' : metin).trim();
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = Number(s);
        return s !== '' && isFinite(n) ? n : null;
    }

    // Kod alanı "kod- ad" ya da yazılmış kod(lar): tireden önceki kısım (eski ddDegerAl / TiredenKodBul).
    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }

    function alanKod(alan) { return kodAl($('.kod-alan[data-alan="' + alan + '"] input').val()); }

    // Ekrandaki seçim (Listele, Rapor Üret ve Rapor Al bunu kullanır).
    function secim() {
        return {
            odenekTur: TUR, tahmin: TAHMIN, yil: yilOku(), ay: ayOku(), senaryo: $('#ddlSenaryo').val() || '',
            kur: alanKod('kur'), fon: alanKod('fon'), fin: alanKod('fin'), eko: alanKod('eko'), peb: alanKod('peb')
        };
    }

    // ---------------------------------------------------------------- tablo

    function degerAlanlari() {
        var a = [];
        for (var i = 0; i < (acik ? acik.veriGirisSay : 0); i++) a.push('d' + i);
        return a;
    }

    // Girilebilen sütunlar, tablodaki sırasıyla (Yapıştır bu sırayla yazar).
    function girilenAlanlar() {
        var a = degerAlanlari();
        if (acik && acik.aciklamaVar) a.push('aciklama');
        return a;
    }

    function girilebilir(p) { return !!acik && !p.node.rowPinned && girilenAlanlar().indexOf(p.colDef.field) >= 0; }

    function hucreSinifi(p) {
        var s = [];
        if (p.colDef.type === 'sayi') s.push('sayi');
        if (p.colDef.field === 'aciklama') s.push('metin');
        if (!p.node.rowPinned) {
            if (girilebilir(p)) s.push('girilir');
            if (p.data && p.data.dg && p.data.dg[p.colDef.field]) s.push('degisti');
        }
        return s;
    }

    function para(p) { return p.value == null || p.value === '' ? '' : tamSayi.format(p.value); }
    function oranBicim(p) {
        if (p.value === undefined || p.value === '') return '';
        if (p.value === null) return p.node.rowPinned && p.data.id === 'secili' ? '' : '-';
        return '%' + tamSayi.format(Math.round(p.value * 100)); // eski "%##0"
    }

    function kodAdOku() { return $('#ddlKodAd').val() || '3'; }

    function detayCiz(p) {
        if (!p.data || p.node.rowPinned) return '';
        var giris = acik && acik.detayGiris;
        return '<button type="button" class="btn btn-link btn-xs detay" title="' + (giris ? 'Tutar girişi (alt kırılımlar)' : 'Tertip bilgileri') + '">' +
            '<span class="glyphicon ' + (giris ? 'glyphicon-pencil' : 'glyphicon-list-alt') + '" aria-hidden="true"></span>' +
            '<span class="sr-only">Detay</span></button>';
    }

    function sutunlar() {
        var kodAd = kodAdOku();
        var giris = { editable: girilebilir, cellEditor: 'agTextCellEditor', valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; } };
        var liste = [
            { field: 'tertipGor', headerName: acik ? acik.kodBaslik : 'Tertip', width: 230, pinned: 'left', hide: kodAd === '1',
                tooltipField: 'tertipGor', cellClass: hucreSinifi },
            { field: 'tertipAd', headerName: kodAd === '1' && acik ? acik.kodBaslik : 'Tertip Adı', width: 300, pinned: 'left', hide: kodAd === '2',
                tooltipField: 'tertipAd', cellClass: hucreSinifi },
            { colId: 'detay', headerName: '', width: 44, pinned: 'left', sortable: false, resizable: false, cellRenderer: detayCiz, cellClass: 'komut' }
        ];
        var veriYil = acik ? acik.veriYil : (yilOku() || new Date().getFullYear()) - (TAHMIN === 1 ? 1 : 0);
        var say = acik ? acik.veriGirisSay : (TAHMIN === 1 ? 1 : 3);
        for (var i = 0; i < say; i++) {
            liste.push($.extend({ field: 'd' + i, headerName: String(veriYil + i), type: 'sayi', width: 130, valueFormatter: para,
                cellClass: hucreSinifi, headerClass: 'sayi' }, giris));
        }
        liste.push({ field: 'aciklama', headerName: 'Açıklama', width: 260, sortable: false, tooltipField: 'aciklama', cellClass: hucreSinifi,
            hide: acik ? !acik.aciklamaVar : !(tanim && tanim.aciklamaVar),
            editable: girilebilir, cellEditor: 'agLargeTextCellEditor', cellEditorPopup: true, cellEditorParams: { maxLength: 500, rows: 5, cols: 50 } });

        // Ek sütunlar iki başlık satırlıdır; üst başlığı boş olan sütun soldaki grubun altına girer (eski ExtGrideEkle).
        var grup = null;
        $.each(acik ? acik.ekSutunlar : [], function (j, s) {
            var sutun = { colId: 'e' + j, headerName: s.alt, type: 'sayi', width: 110, headerClass: 'sayi', cellClass: ['sayi', 'ek'], sortable: false,
                valueGetter: (function (k) { return function (p) { return p.data && p.data.ek ? p.data.ek[k] : ''; }; })(j),
                valueFormatter: s.oran ? oranBicim : para };
            if (!grup || (s.ust && s.ust !== grup.headerName)) {
                grup = { headerName: s.ust || '', headerClass: 'ek-grup', children: [] };
                liste.push(grup);
            }
            grup.children.push(sutun);
        });
        return liste;
    }

    var grid = Sgb.grid('#grdRakam', {
        columnDefs: sutunlar(),
        columnTypes: { sayi: {} },
        // Hücreye tıklayınca düzenlenir; Tab / Enter ile sonraki hücreye geçilir.
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        tooltipShowDelay: 500,
        rowSelection: { mode: 'multiRow', checkboxes: function (p) { return !p.node.rowPinned; }, headerCheckbox: true, enableClickSelection: false },
        selectionColumnDef: { pinned: 'left', width: 44, resizable: false, sortable: false },
        getRowId: function (p) { return p.data.id; },
        getRowClass: function (p) { return p.node.rowPinned ? (p.data.id === 'secili' ? 'satir-secili' : 'satir-toplam') : ''; },
        overlayNoRowsTemplate: '<span>Seçim yapıp Listele\'ye basınız.</span>',
        onCellValueChanged: function (e) {
            if (e.node.rowPinned || e.newValue === e.oldValue) return;
            e.data.dg = e.data.dg || {};
            e.data.dg[e.colDef.field] = true;
            degisti();
            e.api.refreshCells({ rowNodes: [e.node], force: true });
        },
        onSelectionChanged: function () { toplamHesapla(); },
        onCellClicked: function (e) {
            if (e.column.getColId() === 'detay' && !e.node.rowPinned && $(e.event.target).closest('.detay').length) detayAc(e.data);
        }
    });

    function kirliMi() {
        var var_ = false;
        grid.forEachNode(function (n) { if (n.data.dg) var_ = true; });
        return var_;
    }

    function degisti() {
        toplamHesapla();
        $('#btnKaydet').prop('disabled', !kirliMi());
    }

    // TOPLAM (bütün satırlar) ve SEÇİLİ TOPLAM (işaretli satırlar) satırları (eski başlıktaki toplamlar). Ek sütunların toplamı
    // sunucuda hesaplanır; oran sütunlarının seçili toplamı yoktur.
    function toplamHesapla() {
        if (!acik) { grid.setGridOption('pinnedTopRowData', []); return; }
        var kodAd = kodAdOku();
        function satir(id, ad) {
            var s = { id: id, tertipGor: kodAd === '2' ? ad : '', tertipAd: kodAd === '2' ? '' : ad, aciklama: '', ek: [] };
            $.each(degerAlanlari(), function (i, a) { s[a] = 0; });
            return s;
        }
        var t = satir('toplam', 'T O P L A M');
        var s = satir('secili', 'S E Ç İ L İ \u00a0\u00a0 T O P L A M');
        $.each(acik.ekSutunlar, function (j, k) { s.ek[j] = k.oran ? null : 0; });
        var alanlar = degerAlanlari();
        grid.forEachNode(function (n) {
            var secili = n.isSelected();
            $.each(alanlar, function (i, a) {
                var d = Number(n.data[a]) || 0;
                t[a] += d;
                if (secili) s[a] += d;
            });
            if (secili) $.each(acik.ekSutunlar, function (j, k) { if (!k.oran) s.ek[j] += Number(n.data.ek[j]) || 0; });
        });
        t.ek = acik.ekToplam.slice();
        grid.setGridOption('pinnedTopRowData', [t, s]);
    }

    function tuslariAyarla() {
        var var_ = !!acik && grid.getDisplayedRowCount() > 0;
        $('#btnYapistir, #btnIndir, #btnKatsayi').prop('disabled', !var_);
        $('#btnKaydet').prop('disabled', !var_ || !kirliMi());
    }

    function degisiklikVarsaSor(neden) {
        return !kirliMi() || window.confirm('Kaydedilmemiş değişiklikler var. ' + (neden || 'Devam ederseniz') + ' kaybolacak. Devam edilsin mi?');
    }

    function listele() {
        var o = secim();
        o.ekBilgi = parseInt($('#ddlEkBilgi').val(), 10);
        if (isNaN(o.ekBilgi)) o.ekBilgi = -1;
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!o.senaryo) { mesaj('Aşama seçiniz!', 'warning'); return; }
        if (!degisiklikVarsaSor()) return;
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/butcerakam/liste', o)
            .done(function (v) {
                if (v.hata) {
                    mesaj(v.hata, 'warning');
                    listeyiBosalt();
                    return;
                }
                acik = $.extend(o, {
                    veriYil: v.veriYil, veriGirisSay: v.veriGirisSay, aciklamaVar: v.aciklamaVar, detayGiris: v.detayGiris,
                    kodBaslik: v.kodBaslik, ekSutunlar: v.ekSutunlar || [], ekToplam: v.ekToplam || []
                });
                var satirlar = $.map(v.satirlar, function (s, i) {
                    var r = { id: 'r' + i, tertip: s.tertip, tertipGor: s.tertipGor, tertipAd: s.tertipAd, aciklama: s.aciklama || '', ek: s.ek || [] };
                    $.each(s.degerler, function (j, d) { r['d' + j] = d == null ? 0 : d; });
                    return r;
                });
                $('#lblAltBaslik').text(v.baslik);
                grid.setGridOption('columnDefs', sutunlar());
                grid.setGridOption('overlayNoRowsTemplate', '<span>Tertip bulunamadı.</span>');
                grid.setGridOption('rowData', satirlar);
                toplamHesapla();
                tuslariAyarla();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    function listeyiBosalt() {
        acik = null;
        $('#lblAltBaslik').text('');
        grid.setGridOption('rowData', []);
        grid.setGridOption('pinnedTopRowData', []);
        grid.setGridOption('columnDefs', sutunlar());
        tuslariAyarla();
    }

    $('#frmSecim').on('submit', function (olay) { olay.preventDefault(); listele(); });

    // Tertip Gösterim (eski ListeleKodAd): 1 isimler, 2 kodlar, 3 kod ve isim. Yalnızca sütunları değiştirir.
    $('#ddlKodAd').on('change', function () {
        sakla('kodAd', this.value);
        grid.setGridOption('columnDefs', sutunlar());
        toplamHesapla();
    });

    // Ek Sütunlar (eski ListeleEkBilgi): sunucuda hesaplandığından yeniden listelenir.
    $('#ddlEkBilgi').on('change', function () {
        sakla('ekBilgi', this.value);
        if (acik) listele();
    });

    // Yıl değişince kod ağaçları yeni yıla göre okunur; sütun başlıkları da yeni yıla göre olsun diye liste boşaltılır.
    $('#txtYil').on('change', function () {
        if (acik && !degisiklikVarsaSor('Yılı değiştirirseniz')) { this.value = acik.yil; return; }
        if (acik) listeyiBosalt();
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
                    return Sgb.al('api/butcerakam/kodlar', { odenekTur: TUR, tahmin: TAHMIN, yil: a.yil, tur: tur, ust: ust ? ust.kod : '' }).then(function (v) {
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
    // Alana kod yazılabilir (birden çok kod virgülle); Alt+Aşağı ağacı açar.
    $('.kod-alan input').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
    });

    // Sektör Faaliyetlerini Bul (eski btnSektorFaalGetir): sektörde kullanılan faaliyetler Faaliyet alanına yazılır.
    $('#btnSektorFaal').on('click', function () {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.al('api/butcerakam/sektorfaaliyet', { odenekTur: TUR, tahmin: TAHMIN, yil: o.yil, senaryo: o.senaryo, sektor: alanKod('sektor') })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                if (!v.kodlar) { mesaj('Sektörde kullanılan faaliyet bulunamadı.', 'info'); return; }
                $('.kod-alan[data-alan="peb"] input').val(v.kodlar);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- pencereler

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

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

    // Eski sayfadaki ayrı sayfalar (henüz taşınmadıysa "taşınmadı" sayfası açılır).
    function eskiSayfa(adres) { return '../ButceHazirlikv2/' + adres; }

    // Satırdaki simge (eski DetayAc): eko 2 / eko 4 girişinde alt kırılım tutar girişi, diğerlerinde tertip bilgileri.
    function detayAdres(d) {
        var ortak = 'yil=' + encodeURIComponent(acik.yil) + '&tahmin=' + TAHMIN + '&tertip=' + encodeURIComponent(d.tertip);
        if (acik.detayGiris)
            return eskiSayfa('ButceRakamDuzenle?odenekTur=1&senaryoKod=' + encodeURIComponent(acik.senaryo) + '&' + ortak);
        return eskiSayfa('ButceTeklifHazirlaDetay?senaryo=' + encodeURIComponent(acik.senaryo) + '&' + ortak);
    }
    function detayBaslik(d) { return 'Tertip Bilgileri: ' + (d.tertipGor || d.tertip); }
    function detayAc(d) {
        if (!acik || !d) return;
        Sgb.pencere(detayAdres(d), detayBaslik(d));
    }

    // Detay penceresindeki Önceki / Sonraki (eski FPIlerle): listede (sıralı / süzülmüş haliyle) bir önceki ya da sonraki
    // tertip aynı pencerede açılır; baştan önceki sona, sondan sonraki başa döner.
    window.butceRakamDetayIlerle = function (cerceve, tertip, yon) {
        if (!acik || !cerceve) return;
        var dugumler = [];
        grid.forEachNodeAfterFilterAndSort(function (n) { if (n.data && n.data.tertip) dugumler.push(n); });
        if (!dugumler.length) return;
        var i = -1;
        $.each(dugumler, function (j, n) { if (n.data.tertip === tertip) { i = j; return false; } });
        i = i < 0 ? 0 : (i + yon + dugumler.length) % dugumler.length;
        var d = dugumler[i].data;
        grid.ensureNodeVisible(dugumler[i]);
        var adres = detayAdres(d);
        cerceve.src = adres + (adres.indexOf('?') >= 0 ? '&' : '?') + 'menuYok=1';
        $(cerceve).attr('title', detayBaslik(d)).closest('.sgb-pencere').find('.sgb-pencere-baslik h2').text(detayBaslik(d));
    };

    // ---------------------------------------------------------------- işlemler

    // Kaydet (eski Sakla): yalnızca değişen satırlar gönderilir.
    $('#btnKaydet').on('click', function () {
        if (!acik) return;
        grid.stopEditing();
        var satirlar = [];
        grid.forEachNode(function (n) {
            var d = n.data;
            if (!d.dg) return;
            satirlar.push({ tertip: d.tertip, odenek0: Number(d.d0) || 0, odenek1: Number(d.d1) || 0, odenek2: Number(d.d2) || 0, aciklama: d.aciklama || '' });
        });
        if (!satirlar.length) { mesaj('Kayıtlarda değişiklik yapılmamış!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/butcerakam/kaydet', { odenekTur: TUR, tahmin: TAHMIN, yil: acik.yil, senaryo: acik.senaryo, satirlar: satirlar })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (s.basarili) {
                    grid.forEachNode(function (n) { delete n.data.dg; });
                    grid.refreshCells({ force: true });
                }
            })
            .fail(hataGoster)
            .always(function () { tuslariAyarla(); });
    });

    // Katsayı (eski Katsayı menüsü): altı grup için iki katsayı; Uygula seçili satırın katsayılarıyla bütün tertiplerde sonraki iki
    // yılı bu yılın tutarından üretir, Kaydet katsayıları saklar.
    var dlgKatsayi = document.getElementById('dlgKatsayi');

    function katsayilariDoldur(liste) {
        var $t = $('#tblKatsayi').empty();
        $.each(liste || [], function (i, k) {
            var id = 'rdKatsayi' + i;
            $('<tr></tr>')
                .append($('<td></td>').append($('<input type="radio" name="katsayi" />').attr('id', id).val(i).prop('checked', i === 0)))
                .append($('<td></td>').append($('<label></label>').attr('for', id).text(k.ad)))
                .append($('<td></td>').append($('<input type="text" class="form-control input-sm bir" inputmode="decimal" />').val(k.bir).attr('aria-label', k.ad + ' 1. katsayı')))
                .append($('<td></td>').append($('<input type="text" class="form-control input-sm iki" inputmode="decimal" />').val(k.iki).attr('aria-label', k.ad + ' 2. katsayı')))
                .appendTo($t);
        });
    }

    $('#btnKatsayi').on('click', function () {
        if (!acik) return;
        grid.stopEditing();
        $('#dlgKatsayi .yil0').text(acik.veriYil);
        $('#dlgKatsayi .yil1').text(acik.veriYil + 1);
        $('#dlgKatsayi .yil2').text(acik.veriYil + 2);
        dlgKatsayi.showModal();
    });

    $('#btnKatsayiUygula').on('click', function () {
        var $s = $('#tblKatsayi input[name="katsayi"]:checked').closest('tr');
        if (!$s.length) { mesaj('Katsayı satırı seçiniz.', 'warning'); return; }
        var k1 = katsayiCoz($s.find('.bir').val()), k2 = katsayiCoz($s.find('.iki').val());
        if (k1 === null || k2 === null) { mesaj('Katsayılar hatalı.', 'warning'); return; }
        if (!acik || acik.veriGirisSay < 3) return;
        grid.forEachNode(function (n) {
            var d = n.data;
            var bu = Number(d.d0) || 0;
            d.d1 = Math.round(bu * k1);
            d.d2 = Math.round(bu * k2);
            d.dg = d.dg || {};
            d.dg.d1 = d.dg.d2 = true;
        });
        dlgKatsayi.close();
        grid.refreshCells({ force: true });
        degisti();
        mesaj('Katsayılar uygulandı; kaydetmeyi unutmayınız.', 'info');
    });

    $('#btnKatsayiKaydet').on('click', function () {
        var katsayilar = [];
        var hatali = false;
        $('#tblKatsayi tr').each(function () {
            var b = $.trim($(this).find('.bir').val()), i = $.trim($(this).find('.iki').val());
            if (katsayiCoz(b) === null || katsayiCoz(i) === null) hatali = true;
            katsayilar.push([b, i]);
        });
        if (hatali) { mesaj('Katsayılar hatalı.', 'warning'); return; }
        if (!window.confirm('Katsayılar saklanacak. Onaylıyor musunuz?')) return;
        Sgb.gonder('api/butcerakam/katsayi?odenekTur=' + TUR + '&tahmin=' + TAHMIN, { katsayilar: katsayilar })
            .done(function (s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); })
            .fail(hataGoster);
    });

    // Yapıştır (eski EXTVeriYapistir): panodaki satırlar seçili satırdan başlayarak sırayla, sütunlar ilk tutar sütunundan başlayarak
    // girilebilir sütunlara (tutarlar, açıklama) yazılır.
    var yapistirBas = -1;
    var dlgYapistir = document.getElementById('dlgYapistir');

    function yapistir(metin) {
        var satirlar = String(metin || '').split('\n');
        while (satirlar.length && /^\r?$/.test(satirlar[satirlar.length - 1])) satirlar.pop();
        if (!satirlar.length) { mesaj('Panoda yapıştırılacak veri yok.', 'warning'); return; }
        var dugumler = [];
        grid.forEachNodeAfterFilterAndSort(function (n) { dugumler.push(n); });
        var alanlar = girilenAlanlar();
        var yazilan = 0;
        for (var i = 0; i < satirlar.length && yapistirBas + i < dugumler.length; i++) {
            var d = dugumler[yapistirBas + i].data;
            var degerler = satirlar[i].replace(/\r$/, '').split('\t');
            d.dg = d.dg || {};
            for (var j = 0; j < Math.min(alanlar.length, degerler.length); j++) {
                var alan = alanlar[j];
                if (alan === 'aciklama') d[alan] = String(degerler[j]).slice(0, 500);
                else {
                    var n = parseFloat(String(degerler[j]).replace(/\./g, '').replace(/,/g, '.'));
                    d[alan] = isFinite(n) ? n : 0;
                }
                d.dg[alan] = true;
            }
            yazilan++;
        }
        if (!yazilan) return;
        grid.refreshCells({ force: true });
        degisti();
        mesaj(yazilan + ' satır yapıştırıldı; kaydetmeyi unutmayınız.', 'info');
    }

    $('#btnYapistir').on('click', function () {
        if (!acik) return;
        grid.stopEditing();
        var hucre = grid.getFocusedCell();
        var bas = hucre && !hucre.rowPinned && hucre.rowIndex != null ? hucre.rowIndex : -1;
        if (bas < 0) {
            var secili = grid.getSelectedNodes();
            if (secili.length) bas = secili[0].rowIndex;
        }
        if (bas == null || bas < 0) { mesaj('Lütfen satır seçiniz!', 'warning'); return; }
        yapistirBas = bas;
        // Pano okunamazsa (tarayıcı izin vermedi) veri pencereye yapıştırılır.
        var pencere = function () { $('#txtYapistir').val(''); dlgYapistir.showModal(); $('#txtYapistir').trigger('focus'); };
        if (navigator.clipboard && navigator.clipboard.readText && window.isSecureContext) navigator.clipboard.readText().then(yapistir, pencere);
        else pencere();
    });

    $('#btnYapistirTamam').on('click', function () {
        dlgYapistir.close();
        yapistir($('#txtYapistir').val());
    });

    // İndir (eski Kopyala -> Indir): listelenen tablo sunucuda Excel'e yazılır.
    $('#btnIndir').on('click', function () {
        if (!acik) return;
        var $d = $(this).prop('disabled', true);
        var o = { odenekTur: TUR, tahmin: TAHMIN, yil: acik.yil, ay: acik.ay, senaryo: acik.senaryo, kur: acik.kur, fon: acik.fon, fin: acik.fin,
            eko: acik.eko, peb: acik.peb, ekBilgi: acik.ekBilgi };
        Sgb.indir('api/butcerakam/indir', o).then(function () { $d.prop('disabled', false); });
    });

    // Tavan Üstü (eski EkOdenekSayfaAc): işaretli tertipler için tavan üstü ödenek girişi.
    $('#btnTavanUstu').on('click', function () {
        var tertipler = $.map(grid.getSelectedRows(), function (d) { return d.tertip; });
        if (!tertipler.length) { mesaj('Tavan üstü değer gireceğiniz tertipleri seçiniz!', 'warning'); return; }
        Sgb.pencere(eskiSayfa('TavanUstuOdenek?yil=' + yilOku() + '&senaryo=' + encodeURIComponent($('#ddlSenaryo').val() || '') +
            '&tertipler=' + encodeURIComponent(tertipler.join(','))), 'Tavan Üstü Ödenek');
    });

    // Dosya Yükle (yalnızca sistem yöneticisi; eski FileUpload_Click).
    $('#btnDosyaYukle').on('click', function () { $('#dosyaYukle').val('').trigger('click'); });
    $('#dosyaYukle').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        var o = secim();
        if (o.yil < 2000 || !o.senaryo) { mesaj('Yıl ve aşama seçiniz.', 'warning'); return; }
        if (!window.confirm('Dosya veri tabanına kaydedilmek üzere gönderilecektir. Onaylıyor musunuz?\n\n' +
            'Dosyada başlık satırı olmamalı; sütunlar sırayla: tertip, ' + o.yil + ', ' + (o.yil + 1) + ', ' + (o.yil + 2) + ' tutarları.')) return;
        var veri = new FormData();
        veri.append('odenekTur', TUR);
        veri.append('tahmin', TAHMIN);
        veri.append('yil', o.yil);
        veri.append('senaryo', o.senaryo);
        veri.append('dosya', dosya);
        var $d = $('#btnDosyaYukle').prop('disabled', true);
        $.ajax({
            url: Sgb.KOK + 'api/butcerakam/yukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (s) {
            mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
            if (s.basarili && acik && !kirliMi()) listele();
        }).fail(function (xhr) {
            if (xhr.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            hataGoster(xhr);
        }).always(function () { $d.prop('disabled', false); });
    });

    $('#btnTertipler').on('click', function () {
        window.open(eskiSayfa('TertipTanimFormu?yil=' + yilOku() + '&Tur=' + TUR), '_blank');
    });
    $('#btnTertipDonusum').on('click', function () {
        window.open(eskiSayfa('TertipDonusumFormu?yil=' + yilOku()), '_blank');
    });

    // ---------------------------------------------------------------- Rapor Üret menüsü (eski Rapor.xml raporları)

    var $raporGrup = $('.rapor-menu');
    function menuKapat() { $raporGrup.removeClass('open'); $('#btnRapor').attr('aria-expanded', 'false'); }

    function raporlariDoldur(liste) {
        var $l = $('#lstRapor').empty();
        $.each(liste || [], function (i, r) {
            $('<li></li>').append($('<button type="button"></button>').attr('data-rapor', r.kod).text(r.ad)).appendTo($l);
        });
        $('#btnRapor').prop('disabled', !(liste && liste.length));
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

    $('#lstRapor').on('click', 'button', function () {
        menuKapat();
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var p = { odenekTur: TUR, tahmin: TAHMIN, yil: o.yil, senaryo: o.senaryo, kur: o.kur, fon: o.fon, fin: o.fin, eko: o.eko, peb: o.peb,
            rapor: $(this).attr('data-rapor') };
        var $d = $('#btnRapor').prop('disabled', true);
        Sgb.indir('api/butcerakam/rapor', p).then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Rapor Al penceresi (eski gelişmiş rapor)

    var dlgGelismis = document.getElementById('dlgGelismis');
    var ORANLAR = ['-', 'Oran Baş', 'Oran Son', 'Fark Baş', 'Fark Son'];

    function degerAdlari(yil) {
        var a = [];
        for (var g = 3; g >= 1; g--) a.push((yil - g) + ' Ödenek', (yil - g) + ' Ödenek Son', (yil - g) + ' Gerçekleşme');
        a.push((yil - 1) + ' Tahmin', yil + ' Ödenek Teklifi', (yil + 1) + ' Ödenek Tahmini', (yil + 2) + ' Ödenek Tahmini');
        return a;
    }

    function secenekler(adlar, secili) {
        return $.map(adlar, function (ad, i) { return $('<option></option>').val(i).text(ad).prop('selected', i === secili)[0]; });
    }

    function gelismisDoldur(g) {
        g = g || {};
        function bayrak(ad, varsayilan) { var v = g[ad]; if (v === undefined) v = g[ad.charAt(0).toLowerCase() + ad.slice(1)]; if (v === undefined) v = g[ad.replace(/^OE/, 'oe')]; return v === undefined ? varsayilan : !!v; }
        $('#chkAraToplam').prop('checked', bayrak('araToplam', false));
        $('#chkAciklama').prop('checked', bayrak('aciklamaOlsun', false));
        $('#chkUcgen').prop('checked', bayrak('ucgen', true));
        $('#chkOEYeni').prop('checked', bayrak('OEYeni', true));
        $('#chkOEOnayli').prop('checked', bayrak('OEOnayli', true));
        $('#chkOEYevmiyeli').prop('checked', bayrak('OEYevmiyeli', true));

        var $a = $('#tblAlanlar').empty();
        $.each(g.alanlar || [], function (i, a) {
            $('<tr></tr>').attr('data-kod', a.alanKod).attr('data-ad', a.alanAd)
                .append($('<td></td>').text(a.alanAd))
                .append($('<td></td>').append($('<select class="form-control input-sm kirilim"></select>').attr('aria-label', a.alanAd + ' kırılım')
                    .append(secenekler(['0', '1', '2', '3', '4'], a.kirSay))))
                .append($('<td></td>').append($('<select class="form-control input-sm sira"></select>').attr('aria-label', a.alanAd + ' sıra')
                    .append(secenekler(['0', '1', '2', '3', '4'], a.sira))))
                .appendTo($a);
        });

        var $d = $('#tblDegerler').empty();
        var secim_ = {};
        $.each(g.degerler || [], function (i, d) { secim_[d.alanKod] = d; });
        for (var i = 0; i <= 12; i++) {
            var d = secim_[i] || {};
            var id = 'chkDeger' + i;
            $('<tr></tr>').attr('data-kod', i)
                .append($('<td></td>').append($('<input type="checkbox" class="deger" />').attr('id', id).prop('checked', !!d.secili)))
                .append($('<td></td>').append($('<label class="deger-ad"></label>').attr('for', id)))
                .append($('<td></td>').append($('<select class="form-control input-sm oran"></select>').attr('aria-label', 'Oran / fark')
                    .append(secenekler(ORANLAR, d.yuzde || 0))))
                .appendTo($d);
        }
    }

    $('#chkDegerHepsi').on('change', function () { $('#tblDegerler .deger').prop('checked', this.checked); });

    $('#btnGelismis').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var adlar = degerAdlari(yil);
        $('#tblDegerler tr').each(function () { $(this).find('.deger-ad').text(adlar[parseInt($(this).attr('data-kod'), 10)]); });
        $('#chkDegerHepsi').prop('checked', $('#tblDegerler .deger').length === $('#tblDegerler .deger:checked').length);
        dlgGelismis.showModal();
    });

    $('#btnGelismisAl').on('click', function () {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var adlar = degerAdlari(o.yil);
        var p = {
            araToplam: $('#chkAraToplam').prop('checked'), aciklamaOlsun: $('#chkAciklama').prop('checked'), ucgen: $('#chkUcgen').prop('checked'),
            OEYeni: $('#chkOEYeni').prop('checked'), OEOnayli: $('#chkOEOnayli').prop('checked'), OEYevmiyeli: $('#chkOEYevmiyeli').prop('checked'),
            alanlar: [], degerler: []
        };
        $('#tblAlanlar tr').each(function () {
            p.alanlar.push({ alanKod: parseInt($(this).attr('data-kod'), 10), alanAd: $(this).attr('data-ad'),
                kirSay: parseInt($(this).find('.kirilim').val(), 10) || 0, sira: parseInt($(this).find('.sira').val(), 10) || 0, yuzde: 0 });
        });
        $('#tblDegerler tr').each(function () {
            if (!$(this).find('.deger').prop('checked')) return;
            var kod = parseInt($(this).attr('data-kod'), 10);
            p.degerler.push({ alanKod: kod, alanAd: adlar[kod], kirSay: 0, sira: 0, yuzde: parseInt($(this).find('.oran').val(), 10) || 0 });
        });
        if (!$.grep(p.alanlar, function (a) { return a.kirSay > 0; }).length) { mesaj('En az bir alanın kırılımını seçiniz.', 'warning'); return; }
        if (!p.degerler.length) { mesaj('En az bir değer seçiniz.', 'warning'); return; }
        // Bir sonraki açılışta aynı seçimler gelsin (sunucu da saklar).
        tanim.gelismis = { araToplam: p.araToplam, aciklamaOlsun: p.aciklamaOlsun, ucgen: p.ucgen, OEYeni: p.OEYeni, OEOnayli: p.OEOnayli,
            OEYevmiyeli: p.OEYevmiyeli, alanlar: p.alanlar,
            degerler: $.map(p.degerler, function (d) { return { alanKod: d.alanKod, secili: true, yuzde: d.yuzde }; }) };
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/butcerakam/gelismisrapor', $.extend({ odenekTur: TUR, tahmin: TAHMIN, yil: o.yil, ay: o.ay, senaryo: o.senaryo, kur: o.kur,
            fon: o.fon, fin: o.fin, eko: o.eko, peb: o.peb }, { p: JSON.stringify(p) })).then(function () { $d.prop('disabled', false); });
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirliMi()) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış

    tuslariAyarla();
    $('#ddlKodAd').val(saklananOku('kodAd', '3'));
    if (!$('#ddlKodAd').val()) $('#ddlKodAd').val('3');
    if (TAHMIN === 1) $('#btnKatsayi').prop('hidden', true); // yalnızca bir yıl girilir

    Sgb.al('api/butcerakam/tanim', { odenekTur: TUR, tahmin: TAHMIN })
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            $('#lblBaslik').text(t.baslik);
            $('#txtYil').val(t.yil);
            $('#txtAy').val(t.ay);
            var $s = $('#ddlSenaryo').empty();
            $.each(t.senaryolar || [], function (i, s) { $('<option></option>').val(s.kod).text(s.ad).appendTo($s); });
            $s.val(t.senaryo);
            if (!$s.val() && t.senaryolar && t.senaryolar.length) $s.val(t.senaryolar[0].kod);

            // Kod alanları (eski Page_Load): gelirde fonksiyonel / finansman yok, gelir kurumunda kurumsal da yok; eko 4'te faaliyet yok.
            $('.kod-alan[data-alan="kur"]').prop('hidden', !!t.gizli.kurumsal).find('input').val(t.kurum || '');
            $('.kod-alan[data-alan="fon"]').prop('hidden', !!t.gizli.fonksiyonel);
            $('.kod-alan[data-alan="fin"]').prop('hidden', !!t.gizli.finansman);
            $('.kod-alan[data-alan="peb"]').prop('hidden', !!t.gizli.faaliyet);
            $('.kod-alan[data-alan="eko"]').attr('data-tur', t.ekoKodTur);
            $('.kod-alan[data-alan="sektor"], #btnSektorFaal').prop('hidden', !t.sektorVar);
            $('#btnDosyaYukle, #btnTertipler').prop('hidden', !t.sistemYoneticisi);

            var $e = $('#ddlEkBilgi').empty();
            $('<option></option>').val(-1).text('Ek Sütunsuz').appendTo($e);
            $.each(t.ekKolonlar || [], function (i, k) { $('<option></option>').val(k.no).text(k.ad).appendTo($e); });
            $e.val(saklananOku('ekBilgi', '0'));
            if (!$e.val()) $e.val('0');

            raporlariDoldur(t.raporlar);
            katsayilariDoldur(t.katsayilar);
            gelismisDoldur(t.gelismis);
            grid.setGridOption('columnDefs', sutunlar());
            listele(); // eski sayfa açılışta kullanıcının son yıl / aşamasıyla listeliyordu
        })
        .fail(hataGoster);
});
