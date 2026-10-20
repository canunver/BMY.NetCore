// Bordro Tanım Ekranı (HarcamaSurecleri/TanimBordroTanim; eski TanimBordroTanim.aspx).
// Bordro türü (personel tipi) ve işlem türü seçilir, Oku ile değişkenler, formüller, ortak fonksiyonlar ve ortak şablonlar gelir.
// Değişkenler ve Formüller düzenlenebilir tablolardır (AG Grid); Ekle / Sil / Hepsini Seç / Kopyala / Yapıştır eski sayfadaki gibi
// (Ctrl+A / Ctrl+C / Ctrl+V da çalışır). Sakla yapıyı ve ortak fonksiyonları saklar. Kodu İndir, Şablon Üret, Çalıştır dosya üretir.
$(function () {
    'use strict';

    if (!$('#grdDegiskenler').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var girisHarita = {};   // degiskenAd -> { veriTipiStr, aciklama }
    var tipAdlari = [];     // Veri Tipi seçenekleri
    var okunan = null;      // ekranda gösterilen bordro (eski hdnBordroTur / hdnBordroIslem)
    var kirli = false;      // tablolarda saklanmamış değişiklik var mı

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function sayi(v) { var n = parseInt(v, 10); return isNaN(n) ? 0 : n; }
    function secim() { return { bordroTur: sayi($('#cmbBordroTur').val()), islemTur: sayi($('#cmbIslemTur').val()) }; }

    // ---------------------------------------------------------------- yükseklik: tablolar sayfanın kalan yüksekliğini doldurur

    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var kap = el.closest('.sayfa-icerik') || document.body, cs = getComputedStyle(kap);
        var kaydirir = /(auto|scroll)/.test(cs.overflowY);
        var kr = kap.getBoundingClientRect(), kay = kaydirir ? kap.scrollTop : 0;
        var gorunen = kaydirir ? kap.clientHeight : window.innerHeight - kr.top;
        var ust = el.getBoundingClientRect().top - kr.top + kay;
        var son = 0;
        $(kap).children().each(function () {
            if (this.offsetParent === null && getComputedStyle(this).position !== 'fixed') return;
            son = Math.max(son, this.getBoundingClientRect().bottom - kr.top + kay + parseFloat(getComputedStyle(this).marginBottom || 0));
        });
        son += parseFloat(cs.paddingBottom || 0);
        var bosluk = Math.max(0, son - (ust + el.offsetHeight));
        el.style.height = Math.max(enAz, Math.floor(gorunen - ust - bosluk - 1)) + 'px';
    }
    function yay() {
        $('.bt-panel:not([hidden])').find('.bt-grid, .bt-kod').each(function () { yuksekligeYay(this, 260); });
    }
    $(window).on('resize', yay);

    // ---------------------------------------------------------------- sekmeler

    var $sekmeler = $('.bt-sekmeler a[role="tab"]');
    function sekmeSec($a, odakla) {
        $sekmeler.each(function () {
            var $s = $(this), secili = this === $a[0];
            $s.attr({ 'aria-selected': secili ? 'true' : 'false', tabindex: secili ? '0' : '-1' });
            $s.parent().toggleClass('active', secili);
            $('#' + $s.attr('aria-controls')).prop('hidden', !secili);
        });
        if (odakla) $a.trigger('focus');
        yay();
    }
    $sekmeler.on('click', function (e) { e.preventDefault(); sekmeSec($(this), false); });
    $sekmeler.on('keydown', function (e) {
        var i = $sekmeler.index(this), n = $sekmeler.length, j = -1;
        if (e.key === 'ArrowRight') j = (i + 1) % n;
        else if (e.key === 'ArrowLeft') j = (i - 1 + n) % n;
        else if (e.key === 'Home') j = 0;
        else if (e.key === 'End') j = n - 1;
        if (j < 0) return;
        e.preventDefault();
        sekmeSec($sekmeler.eq(j), true);
    });

    // ---------------------------------------------------------------- tablolar

    // Veri sütunu düzenleyicisi (eski düzenlenebilir SelectBox): yazarak süzülen liste, istenirse serbest metin.
    function GirisDuzenleyici() { }
    GirisDuzenleyici.prototype.init = function (p) {
        this.girdi = document.createElement('input');
        this.girdi.type = 'text';
        this.girdi.className = 'ag-input-field-input ag-text-field-input bt-giris-girdi';
        this.girdi.setAttribute('list', 'lstGiris');
        this.girdi.value = p.eventKey && p.eventKey.length === 1 ? p.eventKey : (p.value || '');
    };
    GirisDuzenleyici.prototype.getGui = function () { return this.girdi; };
    GirisDuzenleyici.prototype.afterGuiAttached = function () { this.girdi.focus(); if (this.girdi.value.length > 1) this.girdi.select(); };
    GirisDuzenleyici.prototype.getValue = function () { return this.girdi.value; };

    function metinSutun(alan, baslik, genislik, enUzun, ek) {
        return $.extend({
            field: alan, headerName: baslik, width: genislik, editable: true, sortable: false, tooltipField: alan,
            cellEditor: 'agTextCellEditor', cellEditorParams: { maxLength: enUzun || 640 }
        }, ek || {});
    }
    function sayiSutun(alan, baslik, genislik, ek) {
        return $.extend({
            field: alan, headerName: baslik, width: genislik, editable: true, sortable: false, type: 'rightAligned',
            cellEditor: 'agNumberCellEditor', cellEditorParams: { precision: 0 },
            valueParser: function (p) { var n = parseInt(p.newValue, 10); return isNaN(n) ? 0 : n; }
        }, ek || {});
    }
    var noSutun = { colId: 'no', headerName: '', width: 50, sortable: false, resizable: false, cellClass: 'bt-no', valueGetter: function (p) { return p.node.rowIndex + 1; } };
    var tipSutun = {
        field: 'tipStr', headerName: 'Veri Tipi', width: 90, editable: true, sortable: false,
        cellEditor: 'agSelectCellEditor', cellEditorParams: function () { return { values: tipAdlari }; }
    };

    function FormulDugmesi() { }
    FormulDugmesi.prototype.init = function (p) {
        this.e = document.createElement('button');
        this.e.type = 'button';
        this.e.className = 'btn btn-link btn-xs bt-hucre-dugme';
        this.e.title = 'Formülü geniş pencerede yaz';
        this.e.setAttribute('aria-label', 'Formülü geniş pencerede yaz');
        this.e.innerHTML = '<span class="glyphicon glyphicon-align-left" aria-hidden="true"></span>';
        this.e.addEventListener('click', function () { formulPenceresi(p.node); });
    };
    FormulDugmesi.prototype.getGui = function () { return this.e; };
    FormulDugmesi.prototype.refresh = function () { return false; };

    var ortakSecenek = {
        rowSelection: 'multiple',
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enableCellTextSelection: false,
        tooltipShowDelay: 400,
        overlayNoRowsTemplate: '<span class="text-muted">Bordro türü ve işlem türünü seçip Oku düğmesine basınız.</span>',
        onCellValueChanged: function (e) { kirli = true; if (e.colDef.field === 'girisStr') degisti(e); }
    };

    var tablolar = {
        degiskenler: {
            alanlar: ['girisStr', 'ad', 'siraNo', 'tipStr', 'noktaSay', 'borcHesap', 'alacakHesap', 'kisaEtiket', 'uzunEtiket', 'genislik', 'ilkDeger'],
            bos: function () { return { girisStr: '', ad: '', siraNo: '', tipStr: '', noktaSay: 0, borcHesap: '', alacakHesap: '', kisaEtiket: '', uzunEtiket: '', genislik: 50, ilkDeger: '' }; },
            grid: Sgb.grid('#grdDegiskenler', $.extend({}, ortakSecenek, {
                columnDefs: [
                    noSutun,
                    { field: 'girisStr', headerName: 'Veri', width: 160, editable: true, sortable: false, cellEditor: GirisDuzenleyici,
                        tooltipValueGetter: function (p) { var g = girisHarita[p.value]; return g ? g.aciklama : p.value; } },
                    metinSutun('ad', 'Değişken Ad', 150, 80),
                    metinSutun('siraNo', 'Sıra', 60, 10),
                    tipSutun,
                    sayiSutun('noktaSay', 'Ondalık', 70, { headerTooltip: 'Virgülden sonraki basamak sayısı' }),
                    metinSutun('borcHesap', 'Borç Hesabı', 110, 640),
                    metinSutun('alacakHesap', 'Alacak Hesabı', 110, 640),
                    metinSutun('kisaEtiket', 'Etiket (Kısa)', 110, 15),
                    metinSutun('uzunEtiket', 'Etiket (Uzun)', 200, 100),
                    sayiSutun('genislik', 'Genişlik', 70),
                    metinSutun('ilkDeger', 'İlk Değer', 120, 640, { type: 'rightAligned' })
                ]
            }))
        },
        formuller: {
            alanlar: ['ad', 'siraNo', 'tipStr', 'noktaSay', 'borcHesap', 'alacakHesap', 'kisaEtiket', 'uzunEtiket', 'genislik', 'formula'],
            bos: function () { return { ad: '', siraNo: '', tipStr: '', noktaSay: 0, formula: '', borcHesap: '', alacakHesap: '', kisaEtiket: '', uzunEtiket: '', genislik: 0 }; },
            grid: Sgb.grid('#grdFormuller', $.extend({}, ortakSecenek, {
                columnDefs: [
                    noSutun,
                    metinSutun('ad', 'Değişken Ad', 150, 80),
                    metinSutun('siraNo', 'Sıra', 60, 10),
                    tipSutun,
                    sayiSutun('noktaSay', 'Ondalık', 70, { headerTooltip: 'Virgülden sonraki basamak sayısı' }),
                    metinSutun('borcHesap', 'Borç Hesabı', 110, 640),
                    metinSutun('alacakHesap', 'Alacak Hesabı', 110, 640),
                    metinSutun('kisaEtiket', 'Etiket (Kısa)', 110, 15),
                    metinSutun('uzunEtiket', 'Etiket (Uzun)', 200, 100),
                    sayiSutun('genislik', 'Genişlik', 70),
                    { field: 'formula', headerName: 'Formül', flex: 1, minWidth: 260, editable: true, sortable: false, tooltipField: 'formula',
                        cellEditor: 'agLargeTextCellEditor', cellEditorPopup: true, cellEditorParams: { maxLength: 100000, rows: 8, cols: 80 } },
                    { colId: 'ac', headerName: '', width: 44, sortable: false, resizable: false, cellRenderer: FormulDugmesi, cellClass: 'bt-dugme-hucre' }
                ]
            }))
        }
    };

    function satirlar(t) {
        var l = [];
        t.grid.forEachNodeAfterFilterAndSort(function (n) { l.push(n.data); });
        return l;
    }

    // Sakla için satırlar: sayı alanları boşsa 0 (eski Ext Int alanları gibi).
    function gonderilecek(t) {
        return $.map(satirlar(t), function (d) { return $.extend({}, d, { noktaSay: sayi(d.noktaSay), genislik: sayi(d.genislik) }); });
    }

    // Eski Degisti: Veri seçilince (VERİ GİRİŞ değilse) değişken adı, uzun etiket ve veri tipi genel değişkenden gelir.
    function degisti(e) {
        var v = e.newValue;
        if (v === 'VERİ GİRİŞ') return;
        e.node.setDataValue('ad', v);
        var g = girisHarita[v];
        if (g) {
            e.node.setDataValue('uzunEtiket', g.aciklama);
            e.node.setDataValue('tipStr', g.veriTipiStr);
        }
    }

    // ---------------------------------------------------------------- Ekle / Sil / Hepsini Seç / Kopyala / Yapıştır

    function ekle(t) {
        t.grid.stopEditing();
        var r = t.grid.applyTransaction({ add: [t.bos()] });
        kirli = true;
        if (r.add.length) {
            t.grid.ensureIndexVisible(r.add[0].rowIndex);
            t.grid.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: t.alanlar[0] });
        }
    }

    function sil(t) {
        t.grid.stopEditing();
        var secili = t.grid.getSelectedRows();
        if (!secili.length) { mesaj('Silinecek satırları seçiniz.', 'warning'); return; }
        t.grid.applyTransaction({ remove: secili });
        t.grid.refreshCells({ columns: ['no'], force: true });
        kirli = true;
    }

    // Eski RemoveEnterTab / AddEnterTab: hücre içindeki sekme ve satır sonları kaçışlı yazılır, yapıştırırken geri çevrilir.
    function kacisla(d) { return String(d == null ? '' : d).replace(/\t/g, '\\t').replace(/\r/g, '\\r').replace(/\n/g, '\\n'); }
    function kacisCoz(d) { return String(d == null ? '' : d).replace(/\\t/g, '\t').replace(/\\r/g, '\r').replace(/\\n/g, '\n'); }

    // Eski CopyGrid: seçili satırlar (seçili yoksa tümü), düzenlenebilir sütunlar sırasıyla, sekmeyle ayrılmış.
    function kopyaMetni(t) {
        t.grid.stopEditing();
        var liste = t.grid.getSelectedNodes();
        liste.sort(function (a, b) { return a.rowIndex - b.rowIndex; });
        liste = liste.length ? $.map(liste, function (n) { return n.data; }) : satirlar(t);
        return {
            say: liste.length,
            metin: $.map(liste, function (s) { return $.map(t.alanlar, function (a) { return kacisla(s[a]); }).join('\t') + '\r\n'; }).join('')
        };
    }

    function kopyala(t) {
        var k = kopyaMetni(t);
        if (!k.say) { mesaj('Kopyalanacak satır yok.', 'warning'); return; }
        var tamam = function () { mesaj(k.say + ' satır panoya kopyalandı.', 'success'); };
        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(k.metin).then(tamam, function () { mesaj('Panoya kopyalanamadı.', 'danger'); });
        } else {
            var $t = $('<textarea readonly></textarea>').val(k.metin).css({ position: 'fixed', left: '-9999px' }).appendTo(document.body);
            $t[0].select();
            try { document.execCommand('copy'); tamam(); } catch (e) { mesaj('Panoya kopyalanamadı.', 'danger'); }
            $t.remove();
        }
    }

    // Eski EXTVeriYapistir: seçili satırdan (yoksa ilk satırdan) başlayarak ilk düzenlenebilir sütundan itibaren yazılır;
    // tablo yetmezse sona satır açılır. Sayı sütunlarında binlik noktası atılır, virgül ondalık sayılır.
    var SAYI_ALANLAR = { noktaSay: true, genislik: true };
    function yapistir(t, bas, metin) {
        var gelen = String(metin || '').split('\n');
        while (gelen.length && /^\r?$/.test(gelen[gelen.length - 1])) gelen.pop();
        if (!gelen.length) { mesaj('Panoda yapıştırılacak veri yok.', 'warning'); return; }
        var eksik = bas + gelen.length - t.grid.getDisplayedRowCount();
        if (eksik > 0) {
            var yeni = [];
            for (var k = 0; k < eksik; k++) yeni.push(t.bos());
            t.grid.applyTransaction({ add: yeni });
        }
        var liste = satirlar(t);
        for (var i = 0; i < gelen.length && bas + i < liste.length; i++) {
            var d = liste[bas + i];
            var degerler = gelen[i].replace(/\r$/, '').split('\t');
            for (var j = 0; j < Math.min(t.alanlar.length, degerler.length); j++) {
                var a = t.alanlar[j], v = kacisCoz(degerler[j]);
                if (SAYI_ALANLAR[a]) { v = parseInt(v.replace(/\./g, '').replace(/,/g, '.'), 10); if (isNaN(v)) v = 0; }
                d[a] = v;
            }
        }
        t.grid.refreshCells({ force: true });
        kirli = true;
        mesaj(gelen.length + ' satır yapıştırıldı; saklamayı unutmayınız.', 'info');
    }

    function yapistirBaslangic(t) {
        var secili = t.grid.getSelectedNodes();
        if (secili.length) return Math.min.apply(null, $.map(secili, function (n) { return n.rowIndex; }));
        var h = t.grid.getFocusedCell();
        return h && h.rowIndex != null && h.rowIndex >= 0 ? h.rowIndex : 0;
    }

    var dlgYapistir = document.getElementById('dlgYapistir');
    var yapistirIsi = null;
    function yapistirDugme(t) {
        t.grid.stopEditing();
        var bas = yapistirBaslangic(t);
        var pencere = function () {
            yapistirIsi = function (m) { yapistir(t, bas, m); };
            $('#yapistirNot').text("Excel'den kopyaladığınız satırları buraya yapıştırınız (Ctrl+V). Sütun sırası tablodaki gibidir.");
            $('#txtYapistir').val('');
            dlgYapistir.showModal();
            $('#txtYapistir').trigger('focus');
        };
        if (navigator.clipboard && navigator.clipboard.readText && window.isSecureContext)
            navigator.clipboard.readText().then(function (m) { yapistir(t, bas, m); }, pencere);
        else pencere();
    }
    $('#btnYapistirTamam').on('click', function () {
        dlgYapistir.close();
        if (yapistirIsi) yapistirIsi($('#txtYapistir').val());
        yapistirIsi = null;
    });

    $('.bt-grid-arac[data-grid]').on('click', 'button[data-is]', function () {
        var t = tablolar[$(this).closest('.bt-grid-arac').data('grid')];
        var is = $(this).data('is');
        if (is === 'ekle') ekle(t);
        else if (is === 'sil') sil(t);
        else if (is === 'hepsi') { t.grid.stopEditing(); t.grid.selectAll(); }
        else if (is === 'kopyala') kopyala(t);
        else if (is === 'yapistir') yapistirDugme(t);
    });

    // Klavye (eski KeyMap): düzenleme yokken Ctrl+A tümünü seçer; Ctrl+C / Ctrl+V tarayıcının kopyala / yapıştır olaylarıyla çalışır.
    $.each(tablolar, function (ad, t) {
        var el = document.getElementById(ad === 'degiskenler' ? 'grdDegiskenler' : 'grdFormuller');
        function duzenleniyor() { return t.grid.getEditingCells().length > 0; }
        el.addEventListener('keydown', function (e) {
            if ((e.ctrlKey || e.metaKey) && (e.key === 'a' || e.key === 'A') && !duzenleniyor()) {
                e.preventDefault();
                t.grid.selectAll();
            }
        });
        el.addEventListener('copy', function (e) {
            if (duzenleniyor()) return;
            var k = kopyaMetni(t);
            if (!k.say) return;
            e.preventDefault();
            e.clipboardData.setData('text/plain', k.metin);
            mesaj(k.say + ' satır panoya kopyalandı.', 'success');
        });
        el.addEventListener('paste', function (e) {
            if (duzenleniyor()) return;
            e.preventDefault();
            yapistir(t, yapistirBaslangic(t), e.clipboardData.getData('text/plain'));
        });
    });

    // ---------------------------------------------------------------- formül penceresi (eski wndFormulGir)

    var dlgFormul = document.getElementById('dlgFormul');
    var aktifSatir = null;
    function formulPenceresi(node) {
        tablolar.formuller.grid.stopEditing();
        aktifSatir = node;
        $('#dlgFormulBaslik').text('Formül' + (node.data.ad ? ': ' + node.data.ad : ''));
        $('#txtFormulGir').val(node.data.formula || '');
        dlgFormul.showModal();
        $('#txtFormulGir').trigger('focus');
    }
    $('#btnYazKapat').on('click', function () {
        dlgFormul.close();
        if (aktifSatir) aktifSatir.setDataValue('formula', $('#txtFormulGir').val());
        aktifSatir = null;
    });
    // Formül penceresinde Tab karakteri yazılabilsin
    $('#txtOrtakFonksiyonlar, #txtFormulGir').on('keydown', function (e) {
        if (e.key !== 'Tab' || e.shiftKey || e.ctrlKey || e.altKey) return;
        e.preventDefault();
        var el = this, b = el.selectionStart, s = el.selectionEnd;
        el.value = el.value.substring(0, b) + '\t' + el.value.substring(s);
        el.selectionStart = el.selectionEnd = b + 1;
    });

    // ---------------------------------------------------------------- ortak şablonlar

    function evet(p) { return p.value ? '✓' : ''; }
    function SablonKomut() { }
    SablonKomut.prototype.init = function (p) {
        var $e = $('<span class="bt-komutlar"></span>');
        $('<button type="button" class="btn btn-link btn-xs" title="Şablon dosyasını indir" aria-label="Şablon dosyasını indir"><span class="glyphicon glyphicon-download-alt" aria-hidden="true"></span></button>')
            .on('click', function () { Sgb.indir('api/bordrotanim/sablonal', { bordroTur: p.data.bordroTur, islemTur: p.data.islemTur, sablonKod: p.data.sablonKod }); })
            .appendTo($e);
        $('<button type="button" class="btn btn-link btn-xs" title="Düzenle" aria-label="Düzenle"><span class="glyphicon glyphicon-pencil" aria-hidden="true"></span></button>')
            .on('click', function () { sablonEkranAc(p.data); })
            .appendTo($e);
        this.e = $e[0];
    };
    SablonKomut.prototype.getGui = function () { return this.e; };
    SablonKomut.prototype.refresh = function () { return false; };

    var grdSablon = Sgb.grid('#grdSablonlar', {
        columnDefs: [
            { colId: 'no', headerName: '#', width: 50, sortable: false, valueGetter: function (p) { return p.node.rowIndex + 1; } },
            { field: 'siraNo', headerName: 'Sıra', width: 70 },
            { colId: 'komut', headerName: 'Komutlar', width: 90, sortable: false, cellRenderer: SablonKomut },
            { field: 'sablonAd', headerName: 'Şablon Ad', flex: 1, minWidth: 200, tooltipField: 'sablonAd' },
            { field: 'gecerliKodlar', headerName: 'Geçerli Kodlar', width: 160, tooltipField: 'gecerliKodlar' },
            { field: 'sayfa', headerName: 'Bordroda', width: 95, cellClass: 'bt-orta', valueFormatter: evet },
            { field: 'ortakRapor', headerName: 'Ortak Raporda', width: 115, cellClass: 'bt-orta', valueFormatter: evet },
            { field: 'tekKisilik', headerName: 'Kişide', width: 80, cellClass: 'bt-orta', valueFormatter: evet }
        ],
        rowSelection: 'single',
        onRowDoubleClicked: function (e) { sablonEkranAc(e.data); },
        overlayNoRowsTemplate: '<span class="text-muted">Ortak şablon yok</span>'
    });

    var dlgSablon = document.getElementById('dlgSablon');
    var sablonYeni = 1;

    // Eski SablonEkranAc: kayıt verilirse düzenleme (anahtar salt okunur, Sil ve Şablon Yükle görünür), verilmezse yeni şablon.
    function sablonDurum(yeni) {
        sablonYeni = yeni ? 1 : 0;
        $('#btnSablonSil, #lblSablonYukle').prop('hidden', yeni);
        $('#txtSabPTip, #txtSabIslem, #txtSabKod').prop('readOnly', !yeni);
    }
    function sablonEkranAc(d) {
        if (d) {
            $('#txtSabPTip').val(d.bordroTur);
            $('#txtSabIslem').val(d.islemTur);
            $('#txtSabKod').val(d.sablonKod);
            $('#txtSablonAd').val(d.sablonAd || '');
            $('#txtGecerliKodlar').val(d.gecerliKodlar || '');
            $('#txtSiralamaKod').val(d.siraNo || '');
            $('#chkBordroda').prop('checked', !!d.sayfa);
            $('#chkOrtakda').prop('checked', !!d.ortakRapor);
            $('#chkKiside').prop('checked', !!d.tekKisilik);
        } else {
            $('#frmSablon')[0].reset();
        }
        sablonDurum(!d);
        dlgSablon.showModal();
    }
    function sablonAnahtar() {
        return { bordroTur: $('#txtSabPTip').val(), islemTur: $('#txtSabIslem').val(), sablonKod: $('#txtSabKod').val() };
    }
    function sablonMesgul(evet) { $('#btnSablonKaydet, #btnSablonSil, #fuSablonYukle').prop('disabled', evet); }

    $('#btnYeniSablon').on('click', function () { sablonEkranAc(null); });

    $('#btnSablonKaydet').on('click', function () {
        var istek = $.extend(sablonAnahtar(), {
            sablonAd: $('#txtSablonAd').val(), gecerliKodlar: $('#txtGecerliKodlar').val(), siraNo: $('#txtSiralamaKod').val(),
            sayfa: $('#chkBordroda').prop('checked'), ortakRapor: $('#chkOrtakda').prop('checked'), tekKisilik: $('#chkKiside').prop('checked'),
            yeni: sablonYeni
        });
        sablonMesgul(true);
        Sgb.gonder('api/bordrotanim/sablonkaydet', istek)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                sablonDurum(false);
                grdSablon.setGridOption('rowData', v.sablonlar || []);
                mesaj('Şablon bilgisi kaydedildi.', 'success');
            })
            .fail(hataGoster)
            .always(function () { sablonMesgul(false); });
    });

    $('#btnSablonSil').on('click', function () {
        if (!window.confirm('Şablon bilgisi ve dosyası sunucudan silinecektir, onaylıyor musunuz?')) return;
        sablonMesgul(true);
        Sgb.gonder('api/bordrotanim/sablonsil', sablonAnahtar())
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                sablonDurum(true);
                grdSablon.setGridOption('rowData', v.sablonlar || []);
                mesaj('Şablon silindi.', 'success');
            })
            .fail(hataGoster)
            .always(function () { sablonMesgul(false); });
    });

    // Dosya seçilince hemen yüklenir (eski FileSelected).
    $('#fuSablonYukle').on('change', function () {
        var dosya = this.files && this.files[0];
        if (!dosya) return;
        var k = sablonAnahtar();
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('bordroTur', k.bordroTur);
        veri.append('islemTur', k.islemTur);
        veri.append('sablonKod', k.sablonKod);
        var girdi = this;
        sablonMesgul(true);
        $.ajax({
            url: Sgb.KOK + 'api/bordrotanim/sablonyukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
            mesaj(v.bilgi, 'success');
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else hataGoster(xhr);
        }).always(function () { sablonMesgul(false); girdi.value = ''; });
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- dosya isteği (POST, yanıt dosya ya da { hata })

    function dosyaIste(adres, govde) {
        var json = !(govde instanceof FormData);
        var basliklar = { RequestVerificationToken: Sgb.meta('istek-jetonu') };
        if (json) basliklar['Content-Type'] = 'application/json; charset=utf-8';
        return fetch(Sgb.KOK + adres, { method: 'POST', credentials: 'same-origin', cache: 'no-store', headers: basliklar, body: json ? JSON.stringify(govde) : govde })
            .then(function (y) {
                if (y.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
                if (!y.ok) { mesaj(y.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Dosya hazırlanamadı.', 'danger'); return; }
                var tur = y.headers.get('Content-Type') || '';
                if (tur.indexOf('json') >= 0) return y.json().then(function (v) { mesaj(v.hata || 'Dosya hazırlanamadı.', 'danger'); });
                if (tur.indexOf('text/html') >= 0) { mesaj('Dosya hazırlanamadı.', 'danger'); return; }
                var b = y.headers.get('Content-Disposition') || '';
                var ad = /filename\*=UTF-8''([^;]+)/i.exec(b) || /filename="?([^";]+)"?/i.exec(b);
                return y.blob().then(function (icerik) {
                    var yerel = URL.createObjectURL(icerik);
                    var a = document.createElement('a');
                    a.href = yerel;
                    a.download = ad ? decodeURIComponent(ad[1]) : 'dosya';
                    document.body.appendChild(a);
                    a.click();
                    a.remove();
                    setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
                });
            })
            .catch(function () { mesaj('Dosya hazırlanamadı.', 'danger'); });
    }

    // ---------------------------------------------------------------- oku / sakla

    function mesgul(evet) { $('#btnOku, #btnSakla, #btnKodIndir, #btnSablonUret, #btnCalistir').prop('disabled', evet); }

    function goster(v) {
        okunan = { bordroTur: v.bordroTur, islemTur: v.islemTur };
        $('#txtAd').val(v.ad || '');
        $('#txtKaynak').val(v.kaynak || '');
        tablolar.degiskenler.grid.setGridOption('rowData', v.degiskenler || []);
        tablolar.formuller.grid.setGridOption('rowData', v.formuller || []);
        grdSablon.setGridOption('rowData', v.sablonlar || []);
        $('#txtOrtakFonksiyonlar').val(v.ortakFonksiyonlar || '');
        kirli = false;
    }

    // ilk: sayfa açılışındaki okuma (eski Page_Load); "Yükleniyor..." ve hata gösterilmez.
    function oku(ilk) {
        var s = secim();
        if (!ilk) {
            mesgul(true);
            tablolar.degiskenler.grid.setGridOption('loading', true);
            tablolar.formuller.grid.setGridOption('loading', true);
        }
        return Sgb.al('api/bordrotanim/oku', s)
            .done(function (v) {
                if (!v.basarili) { if (!ilk) mesaj(v.hata, 'danger'); return; }
                goster(v);
            })
            .fail(function (xhr) { if (!ilk) hataGoster(xhr); })
            .always(function () {
                mesgul(false);
                tablolar.degiskenler.grid.setGridOption('loading', false);
                tablolar.formuller.grid.setGridOption('loading', false);
            });
    }

    $('#btnOku').on('click', function () {
        $.each(tablolar, function (a, t) { t.grid.stopEditing(); });
        if (kirli && !window.confirm('Bordroda değişiklikler yaptınız, yaptığınız değişiklikler kaybolacak emin misiniz??')) return;
        oku(false);
    });

    $('#btnSakla').on('click', function () {
        $.each(tablolar, function (a, t) { t.grid.stopEditing(); });
        if (!window.confirm('Bordro formülleri saklanacak ve var olan formüller kaybolacak, emin misiniz?')) return;
        var s = secim();
        var istek = {
            bordroTur: s.bordroTur, islemTur: s.islemTur,
            okunanBordroTur: okunan ? okunan.bordroTur : -1, okunanIslemTur: okunan ? okunan.islemTur : -1,
            ad: $('#txtAd').val(), kaynak: $('#txtKaynak').val(),
            degiskenler: gonderilecek(tablolar.degiskenler), formuller: gonderilecek(tablolar.formuller),
            ortakFonksiyonlar: $('#txtOrtakFonksiyonlar').val()
        };
        mesgul(true);
        Sgb.gonder('api/bordrotanim/sakla', istek)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                kirli = false;
                mesaj(v.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnKodIndir').on('click', function () {
        var s = secim();
        var $d = $(this).prop('disabled', true);
        dosyaIste('api/bordrotanim/kodindir', { bordroTur: s.bordroTur, islemTur: s.islemTur, ortakFonksiyonlar: $('#txtOrtakFonksiyonlar').val() })
            .then(function () { $d.prop('disabled', false); });
    });

    $('#btnSablonUret').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/bordrotanim/sablonuret', secim()).then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- çalıştır (eski wndCalistir / btnCalHesapla_Click)

    var dlgCalistir = document.getElementById('dlgCalistir');
    $('#btnCalistir').on('click', function () { dlgCalistir.showModal(); });

    $('#btnHesapla').on('click', function () {
        if (!$('#txtDonemTarih').val()) { mesaj('Dönem alanı boş bırakılamaz!', 'warning'); $('#txtDonemTarih').trigger('focus'); return; }
        if (!$('#txtCalTarih').val()) { mesaj('Hazırlama Tarihi alanı boş bırakılamaz!', 'warning'); $('#txtCalTarih').trigger('focus'); return; }
        var girdi = document.getElementById('fuCalDosya');
        var dosya = girdi.files && girdi.files[0];
        if (!dosya) { mesaj('Dosya kodu hatalı veya dosya alınamadı!', 'warning'); return; }
        var s = secim();
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('bordroTur', s.bordroTur);
        veri.append('islemTur', s.islemTur);
        veri.append('donem', $('#txtDonemTarih').val());
        veri.append('tarih', $('#txtCalTarih').val());
        veri.append('isGunu', $('#chkIsgunu').prop('checked') ? '1' : '0');
        var $d = $(this).prop('disabled', true);
        dosyaIste('api/bordrotanim/hesapla', veri).then(function () { $d.prop('disabled', false); girdi.value = ''; });
    });

    // ---------------------------------------------------------------- açılış (eski StorelariDoldur + BordroYapiOku)

    var sorgu = new URLSearchParams(window.location.search);
    var ilkParam = {};
    if (sayi(sorgu.get('bordroIslemTur')) > 0) ilkParam.bordroIslemTur = sayi(sorgu.get('bordroIslemTur'));
    if (sayi(sorgu.get('bordroYapiTur')) > 0) ilkParam.bordroYapiTur = sayi(sorgu.get('bordroYapiTur'));

    Sgb.al('api/bordrotanim/baslangic', ilkParam)
        .done(function (v) {
            var $b = $('#cmbBordroTur').empty();
            $.each(v.bordroTurler || [], function (i, o) { $('<option></option>').val(o.kod).text(o.kodAd).appendTo($b); });
            var $i = $('#cmbIslemTur').empty();
            $.each(v.islemTurler || [], function (i, o) { $('<option></option>').val(o.kod).text(o.ad).appendTo($i); });
            $b.val(String(v.bordroTur));
            $i.val(String(v.islemTur));
            tipAdlari = $.map(v.tipler || [], function (t) { return t.ad; });
            var $l = $('#lstGiris').empty();
            $.each(v.girisler || [], function (i, g) {
                girisHarita[g.degiskenAd] = g;
                $('<option></option>').val(g.degiskenAd).text(g.aciklama).appendTo($l);
            });
            oku(true);
        })
        .fail(hataGoster);

    yay();
});
