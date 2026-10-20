// Ödeme Emri / MİF belge arama (ButceMuhasebe/OdemeEmriMIFSorgu; eski aynı adlı Ext.NET sayfası, Ödeme Emri / MİF çerçevesinin
// "Belge Arama" sekmesi). Sorgu alanlarıyla belgeler listelenir; işaretlenen belgelerde Liste İşlemleri (onayla, onay kaldır,
// yazdır (zip), iptal), Yevmiye İşlemleri (yevmiye no ver / kaldır), Çek İşlemleri (çek no güncelle / kaldır, çek listesi yazdır)
// ve İstatistik İşlemleri (istatistik kodu ata). Liste Yazdır Excel'e döker. Satırdaki + ile belgenin hesap / tertip satırları,
// Durum'a tıklanınca tarihçe açılır. Belge numarasına tıklanınca ya da satıra çift tıklanınca belge çerçevenin "Belge Düzenleme"
// sekmesinde açılır (window.parent.OemBelgeAc); çerçeve yoksa Ödeme Emri / MİF çerçevesi pencerede açılır.
// Uçlar: /api/odemeemrimif/liste... (Muhasebe/OdemeEmriMIFSorguUclari.cs); hesap ve alt hesap uçları /api/odemeemrimif/hesap/*,
// althesap/ara, firma/ara (Muhasebe/OdemeEmriMIFUclari.cs). Ortak bileşenler: Sgb.muhasebeBirim, Sgb.hesapKodu, Sgb.firmaSec, Sgb.agac.
$(function () {
    'use strict';

    var $kok = $('.oem-sorgu');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/odemeemrimif/';
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var tanim = {};
    var sonKriter = null;   // son listelemenin kıstasları (işlemler ve yazdırma aynı yılla)
    var SAKLA = 'odemeemrimifliste.';

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function para(v) { return v === null || v === undefined || v === '' ? '' : tutar2.format(Number(v) || 0); }
    function sakliOku(ad) { try { return localStorage.getItem(SAKLA + ad); } catch (e) { return null; } }
    function sakliYaz(ad, deger) { try { localStorage.setItem(SAKLA + ad, deger); } catch (e) { /* özel pencere */ } }

    // ---------------------------------------------------------------- açılır menüler

    $kok.on('click', '.oms-menu > .dropdown-toggle', function (e) {
        e.stopPropagation();
        var $g = $(this).parent();
        var acik = $g.hasClass('open');
        $kok.find('.oms-menu.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false');
        if (!acik) $g.addClass('open').children('.dropdown-toggle').attr('aria-expanded', 'true');
    });
    $kok.on('click', '#mnSutunlar .dropdown-menu', function (e) { e.stopPropagation(); });
    $kok.on('click', '.dropdown-menu a', function (e) { e.preventDefault(); });
    $(document).on('click', function () { $kok.find('.oms-menu.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false'); });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- onay sorusu (eski Confirmation)

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;
    function sor(metin, is) {
        $('#soruMetin').text(metin || 'İşlem yapılacak. Onaylıyor musunuz?');
        soruIsi = is;
        dlgSoru.showModal();
    }
    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        var is = soruIsi;
        soruIsi = null;
        if (is) is();
    });

    // ---------------------------------------------------------------- sorgu alanları

    $('#btnYardim').on('click', function () {
        var $p = $('#pnlYardim');
        var ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false');
        listeYay();
    });

    function kriterGoster(goster) {
        $kok.toggleClass('oms-kriter-gizli', !goster);
        $('#pnlKriter').prop('hidden', !goster);
        $('#btnKriterGoster').prop('hidden', goster);
        sakliYaz('kriterGizli', goster ? '0' : '1');
        listeYay();
    }
    $('#btnKriterGizle').on('click', function () { kriterGoster(false); });
    $('#btnKriterGoster').on('click', function () { kriterGoster(true); });

    function yilKutu() { return parseInt($('#txtYil').val(), 10) || 0; }
    function kodYil() { return yilKutu() || new Date().getFullYear(); }
    // İşlemlerde kullanılan yıl: son listelemenin yılı (eski pgFiltre yılı).
    function yil() { return parseInt((sonKriter || {}).yil, 10) || yilKutu(); }

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: kodYil,
        kurumsalGizle: true,
        adGizle: true
    });

    var hesap = Sgb.hesapKodu('#txtHesapNo', {
        yil: kodYil,
        ad: true,
        denetle: true,
        uc: UC + 'hesap/'
    });

    function hesapNo() {
        var h = hesap ? hesap.deger() : null;
        return h && h.kod ? h.kod : $.trim($('#txtHesapNo').val());
    }
    function hesapHazir(is) {
        var h = hesap && hesap.hazir ? hesap.hazir() : null;
        if (h && h.always) h.always(is); else is();
    }

    // Alt hesap (eski StoreAltHesapPlan: en az üç karakterle "vergi no - ad" önerileri) ve kişi / firma araması.
    var altZaman = null, altSon = '';
    $('#txtAltHesapNo').on('input', function () {
        var m = $.trim(this.value);
        clearTimeout(altZaman);
        if (m.length < 3 || m === altSon || m.indexOf(' - ') > 0) return;
        altZaman = setTimeout(function () {
            altSon = m;
            Sgb.al(UC + 'althesap/ara', { muhasebe: muhBirim.deger().muhasebe || 0, metin: m })
                .done(function (v) {
                    var $l = $('#lstAltHesap').empty();
                    if (v.hata) return;
                    $.each(v.satirlar || [], function (i, s) { $('<option></option>').attr('value', s.kod).appendTo($l); });
                });
        }, 300);
    });
    $('#btnAltHesapSec').on('click', function () {
        Sgb.firmaSec({
            muhasebe: muhBirim.deger().muhasebe || 0,
            ara: $.trim($('#txtAltHesapNo').val()).split(' - ')[0],
            adres: UC + 'firma/ara',
            secilince: function (f) { $('#txtAltHesapNo').val(f.vergiNo + ' - ' + f.ad); }
        });
    });

    // "13.01.31.00- Ad" -> "13.01.31.00"
    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }
    function alanKod(alan) { return kodAl($('.kod-alan[data-alan="' + alan + '"] input').val()); }

    function fisTur() {
        return $.map($('#fisTurler input:checked'), function (c) { return c.value; }).join(',');
    }

    // Eski pgFiltre alan adlarıyla kıstaslar.
    function kriter() {
        var d = muhBirim.deger();
        return {
            yil: yilKutu(), durum: $('#ddlDurum').val() || '', belgeTur: $('#ddlBelgeTur').val() || '',
            muhasebe: d.muhasebe || '', birim: d.birim || '',
            yevmiyeNo1: $.trim($('#txtYevmiyeNo1').val()), yevmiyeNo2: $.trim($('#txtYevmiyeNo2').val()),
            belgeNo1: $.trim($('#txtBelgeNo1').val()), belgeNo2: $.trim($('#txtBelgeNo2').val()),
            belgeTarihi1: $('#txtBelgeTarih1').val(), belgeTarihi2: $('#txtBelgeTarih2').val(),
            hesapNo: hesapNo(), althesapNo: $.trim($('#txtAltHesapNo').val()),
            ilgili: $.trim($('#txtIlgiliAd').val()), ilgiliNo: $.trim($('#txtIlgiliNo').val()),
            kur: alanKod('kur'), fon: alanKod('fon'), fin: alanKod('fin'), eko: alanKod('eko'), peb: alanKod('peb'),
            yevmiyeTarihi1: $('#txtYevmiyeTarih1').val(), yevmiyeTarihi2: $('#txtYevmiyeTarih2').val(),
            durumTarihi1: $('#txtDurumTarih1').val(), durumTarihi2: $('#txtDurumTarih2').val(),
            borc: $.trim($('#txtBorc').val()), alacak: $.trim($('#txtAlacak').val()),
            fisTur: fisTur(),
            islemYapan: $.trim($('#txtIslemYapan').val()), aciklama: $.trim($('#txtAciklama').val()),
            cekNoSorgu: $.trim($('#txtCekNoSorgu').val()),
            OdEsTur: $.trim($('#txtOdEsBeTur').val()), OdEsNo: $.trim($('#txtOdEsBeNo').val()), OdEsTarihi: $('#txtOdEsTarih').val(),
            yatirimProjeNo: $.trim($('#txtYatirimProjeNo').val()),
            surecNo: $.trim($('#txtSurecNo').val()), ihaleMadde: $.trim($('#txtIhaleMadde').val()),
            ist1: tanim.ist1 ? ($('#ddlIst1').val() || '') : '', ist2: tanim.ist2 ? ($('#ddlIst2').val() || '') : ''
        };
    }

    function kriterGecerli(k) {
        if (k.yil > 0 && parseInt(k.durum, 10) > 0) return true;
        mesaj('Sorgulama için yıl alanını boş bırakamazsınız.', 'warning');
        return false;
    }

    // Kod ağaçları (eski DropDownField + TreePanel, nodeLoadPar: 1 kurumsal, 2 fonksiyonel, 3 finans, 4 ekonomik, 14 faaliyet).
    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};
    var aktifAlan = null;
    function agacAc($alan) {
        var y = kodYil();
        var tur = $alan.attr('data-tur');
        var ad = $alan.find('label').text();
        aktifAlan = $alan;
        $('#dlgKodBaslik').text(ad + ' Seçimi');
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[tur];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', ad + ' ağacı').appendTo('#agacKutu'), yil: y };
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al(UC + 'kodlar', { yil: a.yil, tur: tur, ust: ust ? ust.kod : '' }).then(function (v) {
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
            if (a.yil !== y) { a.yil = y; a.agac.yenile(); }
        }
    }
    $('.kod-alan .kod-sec').on('click', function () { agacAc($(this).closest('.kod-alan')); });
    $('.kod-alan .kod-temizle').on('click', function () { $(this).closest('.kod-alan').find('input').val('').trigger('focus'); });
    $('.kod-alan input').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
    });

    function varsayilanlar() {
        $('#ddlDurum').val(String(tanim.varsayilanDurum != null ? tanim.varsayilanDurum : 1));
        $('#ddlBelgeTur').val(String(tanim.varsayilanBelgeTur || 0));
        $('#fisTurler input').prop('checked', false);
        $('#ddlIst1, #ddlIst2').each(function () { this.selectedIndex = 0; });
    }

    $('#btnKriterTemizle').on('click', function () {
        $('#frmKriter').find('input[type="text"], input[type="date"]').not('#txtHesapNo').val('');
        if (hesap) hesap.temizle(); else $('#txtHesapNo').val('');
        muhBirim.ata({});
        $('#txtYil').val(String(tanim.yil || new Date().getFullYear()));
        varsayilanlar();
    });

    // ---------------------------------------------------------------- sonuç tablosu (eski grdListeMIF)

    // Belge: çerçevenin "Belge Düzenleme" sekmesinde açılır (eski BelgeGirisAc).
    function belgeAc(d) {
        var y = d.yil || yil();
        if (window.parent && window.parent !== window && typeof window.parent.OemBelgeAc === 'function') {
            window.parent.OemBelgeAc(y, d.belgeNo);
            return;
        }
        Sgb.pencere('../ButceMuhasebe/OdemeEmriMIFFrame?' + $.param({ aramaYok: 1, yil: y, belgeNo: d.belgeNo }), 'Ödeme Emri / MİF');
    }

    function bagHucre(is, ipucu) {
        return function (x) {
            if (!x.data || x.data._detay || x.data._toplam || x.value === null || x.value === undefined || x.value === '') return x.value || '';
            return $('<a href="#"></a>').text(x.value).attr('title', ipucu).on('click', function (e) {
                e.preventDefault(); e.stopPropagation();
                is(x.data);
            })[0];
        };
    }

    function tarihBicim(x) { return Sgb.tarih(x.value); }
    function paraBicim(x) { return para(x.value); }
    function sifirsiz(x) { return x.value === 0 || x.value === '0' ? '' : (x.value || ''); }

    // Sıra no (eski RowNumberer): görünen sıradaki yeri; açık ayrıntı satırları sayılmaz.
    function siraNo(x) {
        if (!x.data || x.data._detay || x.data._toplam || x.node.rowIndex === null) return '';
        var once = 0;
        $.each(acik, function (no) {
            var n = grid.getRowNode('detay-' + no);
            if (n && n.rowIndex !== null && n.rowIndex < x.node.rowIndex) once++;
        });
        return x.node.rowIndex - once + 1;
    }

    // Sütunlar (eski ColumnModel; gizli: açılışta gizli). sabit: Sütunlar menüsünde yok.
    var SUTUNLAR = [
        { colId: 'ac', headerName: '', width: 32, minWidth: 32, maxWidth: 32, headerClass: 'oms-dar',  sortable: false, resizable: false, suppressMovable: true, cellClass: 'oms-ac-hucre', cellRenderer: acHucre, sabit: true },
        { colId: 'sira', headerName: '#', width: 52, minWidth: 44, maxWidth: 80, sortable: false, suppressMovable: true, type: 'rightAligned', cellClass: 'oms-sira', valueGetter: siraNo, sabit: true },
        { field: 'belgeNo', headerName: 'Belge No', minWidth: 110, flex: 110, cellRenderer: bagHucre(belgeAc, 'Belgeyi aç'), sabit: true },
        { field: 'yevmiyeNo', headerName: 'Yevmiye No', minWidth: 80, flex: 80, type: 'rightAligned', valueFormatter: sifirsiz },
        { field: 'belgeTur', headerName: 'Belge Türü', minWidth: 60, flex: 60, hide: true },
        { field: 'belgeTarih', headerName: 'Belge Tarihi', minWidth: 90, flex: 90, valueFormatter: tarihBicim },
        { field: 'muhasebe', headerName: 'Muhasebe Birim Kodu', minWidth: 130, flex: 170, tooltipField: 'muhasebe' },
        { field: 'birim', headerName: 'Birim Kodu', minWidth: 130, flex: 170, tooltipField: 'birim' },
        { field: 'borc', headerName: 'Borç', minWidth: 110, flex: 110, type: 'rightAligned', valueFormatter: paraBicim },
        { field: 'alacak', headerName: 'Alacak', minWidth: 110, flex: 110, type: 'rightAligned', valueFormatter: paraBicim },
        { field: 'aciklama', headerName: 'Açıklama', minWidth: 140, flex: 200, tooltipField: 'aciklama' },
        { field: 'ilgili', headerName: 'İlgili', minWidth: 120, flex: 150, tooltipField: 'ilgili' },
        { field: 'yevmiyeTarih', headerName: 'Yevmiye Tarihi', minWidth: 90, flex: 90, valueFormatter: tarihBicim },
        { field: 'durum', headerName: 'Durum', minWidth: 90, flex: 90, cellRenderer: bagHucre(tarihceAc, 'Belge tarihçesi') },
        { field: 'durumTarih', headerName: 'En Son İşlem Tarihi', minWidth: 100, flex: 100, hide: true, valueFormatter: tarihBicim },
        { field: 'islemYapan', headerName: 'En Son İşlem Yapan', minWidth: 100, flex: 110, hide: true },
        { field: 'OEBNo', headerName: 'Ö.Esas Belge No', minWidth: 100, flex: 100, hide: true },
        { field: 'OEBTarih', headerName: 'Ö.Esas Belge Tarihi', minWidth: 100, flex: 100, hide: true, valueFormatter: tarihBicim },
        { field: 'cekNo', headerName: 'Çek/Gönderme No', minWidth: 100, flex: 100, hide: true },
        { field: 'cekTarih', headerName: 'Çek/Gönderme Tarihi', minWidth: 100, flex: 100, hide: true, valueFormatter: tarihBicim },
        { field: 'surecNo', headerName: 'Süreç No', minWidth: 90, flex: 90, hide: true },
        { field: 'ihaleMadde', headerName: 'İhale Madde', minWidth: 90, flex: 90, hide: true },
        { field: 'ist1Ad', headerName: 'İst Kod 1', minWidth: 90, flex: 100, hide: true, tooltipField: 'ist1Ad' },
        { field: 'ist2Ad', headerName: 'İst Kod 2', minWidth: 90, flex: 100, hide: true, tooltipField: 'ist2Ad' },
        { field: 'ekliDosya', headerName: 'Ekli Dosya', minWidth: 70, flex: 70, hide: true },
        { field: 'kimlikno', headerName: 'Kimlik No', minWidth: 100, flex: 100, hide: true }
    ];

    var grid = Sgb.grid('#grdListe', {
        columnDefs: $.map(SUTUNLAR, function (s) {
            var c = $.extend({}, s);
            delete c.sabit;
            c.colId = s.colId || s.field;
            return c;
        }),
        defaultColDef: { wrapHeaderText: true, autoHeaderHeight: true },
        selectionColumnDef: { width: 32, minWidth: 32, maxWidth: 32, resizable: false, suppressHeaderMenuButton: true, cellClass: 'oms-dar', headerClass: 'oms-dar' },
        getRowClass: function (x) { return x.data && x.data._toplam ? 'oms-toplam' : ''; },
        onModelUpdated: function () { grid && grid.refreshCells({ columns: ['sira'], force: true }); },
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false, isRowSelectable: function (n) { return !(n.data && n.data._detay); } },
        getRowId: function (x) { if (x.data._toplam) return 'toplam'; return x.data._detay ? 'detay-' + x.data.belgeNo : x.data.belgeNo; },
        isFullWidthRow: function (x) { return !!(x.rowNode.data && x.rowNode.data._detay); },
        fullWidthCellRenderer: ayrintiHucre,
        getRowHeight: function (x) { return x.data && x.data._detay ? (x.data._yukseklik || 90) : undefined; },
        onRowDoubleClicked: function (e) { if (e.data && !e.data._detay && !e.data._toplam) belgeAc(e.data); },
        onSortChanged: function () { ayrintilariKapat(); },
        onPaginationChanged: function (e) { if (e.newPage) ayrintilariKapat(); },
        onSelectionChanged: function () { ozetYaz(); },
        pagination: true,
        paginationPageSize: 250,
        paginationPageSizeSelector: [50, 100, 250, 500, 1000, 2500, 5000],
        overlayNoRowsTemplate: '<span>Kıstasları girip Listele\'ye basınız.</span>'
    });

    // Sütun seçimi (eski sütun başlığı menüsündeki Columns listesi).
    function sutunListesiYenile() {
        var $u = $('#mnSutunlar .oms-sutun-liste').empty();
        $.each(SUTUNLAR, function (i, s) {
            if (s.sabit) return;
            var kol = grid.getColumn(s.field);
            var $c = $('<input type="checkbox" />').prop('checked', !!(kol && kol.isVisible())).on('change', function () {
                grid.setColumnsVisible([s.field], this.checked);
            });
            $('<li></li>').append($('<label class="oms-sutun"></label>').append($c, document.createTextNode(' ' + (kol ? kol.getColDef().headerName : s.headerName)))).appendTo($u);
        });
    }

    // Eski GroupingSummary: borç / alacak genel toplamı tablonun en altında sabit satır; araç çubuğunda belge sayısı.
    function ozetYaz() {
        var adet = 0, borc = 0, alacak = 0;
        grid.forEachNode(function (n) {
            if (!n.data || n.data._detay) return;
            adet++; borc += Number(n.data.borc) || 0; alacak += Number(n.data.alacak) || 0;
        });
        grid.setGridOption('pinnedBottomRowData', adet ? [{ _toplam: true, belgeNo: 'Toplam', borc: borc, alacak: alacak }] : []);
        if (!adet) { $('#lblOzet').text(''); return; }
        var secili = seciliBelgeler().length;
        $('#lblOzet').text(adet + ' belge' + (secili ? ' (' + secili + ' seçili)' : ''));
    }

    function listele(sessiz) {
        hesapHazir(function () {
            var k = kriter();
            if (!kriterGecerli(k)) return;
            var $d = $('#btnListe').prop('disabled', true);
            grid.deselectAll();
            acik = {};
            grid.setGridOption('loading', true);
            Sgb.al(UC + 'liste', k)
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); grid.setGridOption('rowData', []); ozetYaz(); return; }
                    sonKriter = k;
                    var satirlar = v.satirlar || [];
                    grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
                    grid.setGridOption('rowData', satirlar);
                    ozetYaz();
                    if (!sessiz && !satirlar.length) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
        });
    }

    $('#frmKriter').on('submit', function (e) { e.preventDefault(); listele(false); });
    $('#frmKriter').on('keydown', 'input[type="text"], input[type="date"], input[type="number"]', function (e) {
        if (e.key === 'Enter' && !$(this).closest('.kod-alan').length && !$(this).closest('.hk-bilesen').length) { e.preventDefault(); listele(false); }
    });

    // ---------------------------------------------------------------- satır ayrıntısı (eski RowExpander / BeforeExpand)

    var acik = {};

    function acHucre(x) {
        if (!x.data || x.data._detay || x.data._toplam) return '';
        var a = !!acik[x.data.belgeNo];
        return $('<button type="button" class="btn btn-link btn-xs oms-ac"></button>')
            .attr('title', a ? 'Satırları kapat' : 'Belge satırlarını göster').attr('aria-expanded', a ? 'true' : 'false')
            .text(a ? '−' : '+')
            .on('click', function (e) { e.stopPropagation(); ayrintiDegistir(x.node); })[0];
    }

    function ayrintiDegistir(node) {
        var d = node.data, no = d.belgeNo;
        if (acik[no]) {
            grid.applyTransaction({ remove: [acik[no]] });
            delete acik[no];
        } else {
            acik[no] = { _detay: true, belgeNo: no, satirlar: null };
            grid.applyTransaction({ add: [acik[no]], addIndex: node.rowIndex + 1 });
            Sgb.al(UC + 'liste/detay', { yil: d.yil || yil(), belgeNo: no })
                .done(function (v) {
                    if (!acik[no]) return;
                    if (v.hata) { mesaj(v.hata, 'danger'); acik[no].satirlar = []; } else acik[no].satirlar = v.satirlar || [];
                    ayrintiYenile(no);
                })
                .fail(hataGoster);
        }
        grid.refreshCells({ rowNodes: [node], columns: ['ac'], force: true });
    }

    function ayrintiYenile(no) {
        var n = grid.getRowNode('detay-' + no);
        if (!n) return;
        n.data._yukseklik = Math.min(320, 24 + 30 + 29 * Math.max(1, (n.data.satirlar || []).length));
        grid.resetRowHeights();
        grid.redrawRows({ rowNodes: [grid.getRowNode('detay-' + no)] });
    }

    function ayrintilariKapat() {
        var l = $.map(acik, function (v) { return v; });
        acik = {};
        if (l.length) grid.applyTransaction({ remove: l });
        grid.refreshCells({ columns: ['ac'], force: true });
    }

    function ayrintiHucre(x) {
        var d = x.data, $k = $('<div class="oms-ayrinti-satir"></div>');
        if (d.satirlar === null) { $('<div class="oms-not"></div>').text('Belge satırları okunuyor...').appendTo($k); return $k[0]; }
        var $t = $('<table class="oms-ayrinti-tablo"><thead><tr><th>Hesap Kodu</th><th>Tertip</th><th class="oms-sag">Borç</th><th class="oms-sag">Alacak</th></tr></thead><tbody></tbody></table>');
        var $b = $t.find('tbody');
        if (!d.satirlar.length) $('<tr><td colspan="4" class="oms-not">Satır yok.</td></tr>').appendTo($b);
        $.each(d.satirlar, function (i, r) {
            $('<tr></tr>')
                .append($('<td class="oms-kod"></td>').text(r.HesapKod || ''))
                .append($('<td class="oms-kod"></td>').text(r.Tertip || ''))
                .append($('<td class="oms-sag"></td>').text(para(r.Borc)))
                .append($('<td class="oms-sag"></td>').text(para(r.Alacak)))
                .appendTo($b);
        });
        $k.append($t);
        return $k[0];
    }

    // ---------------------------------------------------------------- tarihçe (eski TarihceGoster -> BelgeTarihce.aspx)

    var dlgTarihce = document.getElementById('dlgTarihce');
    var tarihceGrid = null;
    function tarihceAc(d) {
        if (!tarihceGrid) {
            tarihceGrid = Sgb.grid('#grdTarihce', {
                columnDefs: [
                    { field: 'sira', headerName: 'Sıra', minWidth: 60, flex: 60, type: 'rightAligned' },
                    { field: 'islemTarih', headerName: 'İşlem Tarih', minWidth: 160, flex: 160 },
                    { field: 'islem', headerName: 'İşlem', minWidth: 200, flex: 220, tooltipField: 'islem' },
                    { field: 'islemYapan', headerName: 'İşlemi Yapan', minWidth: 120, flex: 120 },
                    { field: 'islemYapanAd', headerName: 'İşlem Yapan Ad', minWidth: 160, flex: 170 }
                ],
                overlayNoRowsTemplate: '<span>Tarihçe kaydı yok.</span>'
            });
        }
        $('#lblTarihceBelge').text('');
        tarihceGrid.setGridOption('rowData', []);
        dlgTarihce.showModal();
        tarihceGrid.setGridOption('loading', true);
        Sgb.al(UC + 'liste/tarihce', { yil: d.yil || yil(), belgeNo: d.belgeNo })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#lblTarihceBelge').text(v.belge || '');
                tarihceGrid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { tarihceGrid.setGridOption('loading', false); });
    }

    // ---------------------------------------------------------------- yazdırma

    // Liste Yazdır (eski btnListeYazdir_Click: o anki sorgu alanlarıyla).
    $('#btnListeYazdir').on('click', function () {
        var $d = $(this);
        hesapHazir(function () {
            var k = kriter();
            if (!kriterGecerli(k)) return;
            $d.prop('disabled', true);
            Sgb.indir(UC + 'liste/yazdir', k).then(function () { $d.prop('disabled', false); });
        });
    });

    function seciliBelgeler() {
        return $.map(grid.getSelectedRows(), function (r) { return r._detay ? null : r.belgeNo; });
    }
    function seciliVarMi() {
        if (seciliBelgeler().length) return true;
        mesaj('Listeden herhangi bir belge seçilmediği için işlem yapılamadı.', 'info');
        return false;
    }
    function seciliMetin() {
        var n = seciliBelgeler().length;
        return n ? n + ' belge seçili.' : 'Listeden belge seçilmedi; numara verme / kaldırma için önce belgeleri işaretleyiniz.';
    }

    // Liste İşlemleri / Yazdır (eski btnBelgeYazdir_Click: seçili belgelerin çıktıları zip).
    $('#mnBelgeYazdir').on('click', function () {
        if (!seciliVarMi()) return;
        Sgb.indir(UC + 'belgeler-yazdir', { yil: yil(), belgeler: seciliBelgeler().join(';') });
    });

    // ---------------------------------------------------------------- toplu durum işlemleri (eski BelgeDurumDegisikligi)

    function durumGonder(islem, ek, sonra) {
        var b = seciliBelgeler();
        Sgb.gonder(UC + 'liste/durum', $.extend({ yil: yil(), islem: islem, belgeler: b }, ek || {}))
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata || 'İşlem yapılamadı.', 'danger'); return; }
                if (sonra) sonra();
                listele(true);
                mesaj(v.bilgi || 'İşlem tamamlandı.', 'success');
                if (v.uyari) mesaj(v.uyari, 'warning');
            })
            .fail(hataGoster);
    }

    $('.durum-islem').on('click', function () {
        var islem = $(this).attr('data-islem');
        if (!seciliVarMi()) return;
        sor(null, function () { durumGonder(islem); });
    });

    // Yevmiye İşlemleri (eski menuYevmiye).
    var dlgYevmiye = document.getElementById('dlgYevmiye');
    $('#btnYevmiye').on('click', function () {
        $('#lblYevmiyeSecili').text(seciliMetin());
        dlgYevmiye.showModal();
        $('#txtIslemYevmiyeNo').trigger('focus');
    });
    $('#chkIslemYevmiye').on('change', function () { $('#grpIslemYevmiyeTarihi').prop('hidden', this.checked); });
    $('#frmYevmiye').on('submit', function (e) {
        e.preventDefault();
        if (!seciliVarMi()) return;
        var ayni = $('#chkIslemYevmiye').prop('checked');
        var tarih = $('#txtIslemYevmiyeTarihi').val();
        if (!ayni && !tarih) { mesaj('Yevmiye tarihi hatalı. Lütfen geçerli bir tarih giriniz.', 'warning'); return; }
        var ek = { yevmiyeNo: $.trim($('#txtIslemYevmiyeNo').val()), belgeTarihi: ayni, yevmiyeTarih: tarih };
        dlgYevmiye.close();
        sor(null, function () { durumGonder('yevver', ek); });
    });
    $('#btnYevmiyeNoKaldir').on('click', function () {
        if (!seciliVarMi()) return;
        dlgYevmiye.close();
        sor(null, function () { durumGonder('yevkaldir'); });
    });

    // Çek İşlemleri (eski menuCek).
    var dlgCek = document.getElementById('dlgCek');
    $('#btnCek').on('click', function () {
        $('#lblCekSecili').text(seciliMetin());
        dlgCek.showModal();
        $('#txtIslemCekNo').trigger('focus');
    });
    $('#frmCek').on('submit', function (e) {
        e.preventDefault();
        if (!seciliVarMi()) return;
        var cekNo = $.trim($('#txtIslemCekNo').val());
        dlgCek.close();
        sor(null, function () { durumGonder('cekver', { cekNo: cekNo }); });
    });
    $('#btnCekNoKaldir').on('click', function () {
        if (!seciliVarMi()) return;
        dlgCek.close();
        sor(null, function () { durumGonder('cekkaldir', null, function () { $('#txtIslemCekNo').val(''); }); });
    });
    // Çek listesi (eski btnCekNoYazdir_Click: sorgu alanları + çek no; Sorgula("103")).
    $('#btnCekNoYazdir').on('click', function () {
        var $d = $(this);
        hesapHazir(function () {
            var k = kriter();
            if (!kriterGecerli(k)) return;
            $d.prop('disabled', true);
            Sgb.indir(UC + 'cek-yazdir', $.extend(k, { cekNo: $.trim($('#txtIslemCekNo').val()) })).then(function () { $d.prop('disabled', false); });
        });
    });

    // İstatistik İşlemleri (eski wndIst / btnIstAta_Click).
    var dlgIst = document.getElementById('dlgIst');
    $('#btnIstatistik').on('click', function () {
        $('#lblIstSecili').text(seciliMetin());
        dlgIst.showModal();
    });
    $('#frmIst').on('submit', function (e) {
        e.preventDefault();
        if (!seciliVarMi()) return;
        var ek = { ist1: tanim.ist1 ? ($('#ddlIstAta1').val() || '') : '', ist2: tanim.ist2 ? ($('#ddlIstAta2').val() || '') : '' };
        dlgIst.close();
        sor(null, function () { durumGonder('istata', ek); });
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    function doldur(sec, liste, bosEkle) {
        var $s = $(sec).empty();
        var sifirVar = false;
        $.each(liste || [], function (i, d) { if (String(d.kod) === '0') sifirVar = true; });
        if (bosEkle && !sifirVar) $('<option></option>').val('').text('').appendTo($s);
        $.each(liste || [], function (i, d) { $('<option></option>').val(d.kod).text(d.ad).appendTo($s); });
    }

    function istatistikKur(no, ist) {
        if (!ist) return;
        $('#grpIst' + no + ', #grpIstAta' + no).prop('hidden', false);
        $('#lblIst' + no + ', #lblIstAta' + no).text(ist.ad || ('İst Kod ' + no));
        doldur('#ddlIst' + no, ist.degerler, true);
        doldur('#ddlIstAta' + no, ist.degerler, true);
        var defs = grid.getColumnDefs();
        $.each(defs, function (i, c) { if (c.colId === 'ist' + no + 'Ad' || c.field === 'ist' + no + 'Ad') c.headerName = ist.ad || c.headerName; });
        grid.setGridOption('columnDefs', defs);
    }

    Sgb.al(UC + 'liste/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            Sgb.yilKutusu('#txtYil', t.yillar, t.yil);
            doldur('#ddlDurum', t.durumlar);
            doldur('#ddlBelgeTur', t.belgeTurleri);
            var $f = $('#fisTurler').empty();
            $.each(t.fisTurleri || [], function (i, d) {
                $('<label class="oms-secenek"></label>')
                    .append($('<input type="checkbox" />').val(d.kod), document.createTextNode(' ' + d.ad))
                    .appendTo($f);
            });
            istatistikKur(1, t.ist1);
            istatistikKur(2, t.ist2);
            $('#btnIstatistik').prop('hidden', !(t.ist1 || t.ist2));
            varsayilanlar();
            sutunListesiYenile();
        })
        .fail(hataGoster);

    if (sakliOku('kriterGizli') === '1') kriterGoster(false);
    sutunListesiYenile();

    // Öğeyi sayfa alanının (.sayfa-icerik) kalan yüksekliğine yayar; dar ekranda tablo kriterlerin altında en az 320px'tir.
    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var kap = el.closest('.sayfa-icerik') || document.body, cs = getComputedStyle(kap);
        var kaydirir = /(auto|scroll)/.test(cs.overflowY);
        var kr = kap.getBoundingClientRect(), kay = kaydirir ? kap.scrollTop : 0;
        var gorunen = kaydirir ? kap.clientHeight : window.innerHeight - kr.top;
        var ust = el.getBoundingClientRect().top - kr.top + kay;
        var son = 0;
        $(kap).find('.oem-sorgu .oms-liste').each(function () {
            son = Math.max(son, this.getBoundingClientRect().bottom - kr.top + kay + parseFloat(getComputedStyle(this).marginBottom || 0));
        });
        son += parseFloat(cs.paddingBottom || 0);
        var bosluk = Math.max(0, son - (ust + el.offsetHeight));
        el.style.height = Math.max(enAz, Math.floor(gorunen - ust - bosluk - 1)) + 'px';
    }
    // Geniş ekranda tablo CSS ile (esnek düzen) kalan yüksekliği doldurur; dar ekranda kalan yüksekliğe yayılır (en az 320px).
    function listeYay() {
        var el = document.getElementById('grdListe');
        if (!el) return;
        if (window.matchMedia('(min-width: 992px)').matches) { el.style.height = ''; return; }
        yuksekligeYay(el, 320);
    }
    $(window).on('resize', listeYay);
    listeYay();
});
