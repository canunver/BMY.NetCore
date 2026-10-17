// Bütçe İşlemi formu (ButceHazirlikv2/MasrafPlaniRevize; eski MasrafPlaniRevize.aspx): masraf planı revize / aktarma / ekleme
// belgesi. Tertipler AG Grid'de girilir (beyaz hücreler girilir, sarılar salt okunur; toplamlar alttaki satırda). Ekle/Düş Göster
// işaretliyse düşülen / eklenen sütunları, değilse 12 ay görünür. Kaydet, Onayla (yönetici), Yazdır, Yapıştır, Kopyala, Öd.Gönd.Üret,
// Raporlar, Yükle, İşlem Üret, Rakam Dağıt, Ekli Dosyalar, Ödenek Devri. Masraf planı onaylı değilse Raporlar, Yükle ve Birim /
// Tertip Prg. / İş Kod sütunları kapalıdır. Çerçevenin Belge Arama sekmesi belgeyi window.OgBelgeYukle(yil, no, birim) ile okutur.
// Uçlar: /api/mprevize/* (ButceHazirlik/MPRevizeUclari.cs).
$(function () {
    'use strict';

    if (!$('.mp-revize-form').length) return;

    var API = 'api/mprevize';
    var AYLAR = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
    var BOS_SATIR = 20;
    var durum = { mpOnayli: false, yonetici: false, bugun: '', odGonNo: '', odGonTur: 0, odGonYil: 0, ilgiliSiraNo: -1 };
    var sonId = 0;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function sonucPencere(baslik, metin) {
        $('#dlgSonucBaslik').text(baslik);
        $('#sonucMetin').text(metin);
        document.getElementById('dlgSonuc').showModal();
    }
    function sonucGoster(v) {
        if (v.hata) { if (String(v.hata).length > 200) sonucPencere('Hata', v.hata); else mesaj(v.hata, 'danger'); return false; }
        if (v.bilgi) {
            if (v.uyariVar) sonucPencere('Sonuç', v.bilgi);
            else mesaj(v.bilgi, 'success');
        }
        return true;
    }
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }
    function noOku() { return parseInt($.trim($('#txtSiraNo').val()), 10) || 0; }
    function turOku() { return parseInt($('#ddlIslemTur').val(), 10) || 0; }
    function ekDus() { return $('#chkEklenenDusulen').prop('checked'); }

    // Türkçe biçimli sayı (1.234,56) ya da düz sayı.
    function sayiCoz(v) {
        if (typeof v === 'number') return isFinite(v) ? v : null;
        var s = $.trim(String(v === undefined || v === null ? '' : v)).replace(/\s/g, '');
        if (s === '') return 0;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        else if ((s.match(/\./g) || []).length > 1) s = s.replace(/\./g, '');
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    // ---------------------------------------------------------------- tablo

    function salt(ek) { return function (p) { return p.node.rowPinned ? ek : ek.concat('salt'); }; }
    function girilir(p) { return !p.node.rowPinned; }
    function tutarYaz(p) { return MPTavan.tutar(p.value); }
    function sayiParse(p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : Math.round(n * 100) / 100; }
    function sayiSutun(ozellik) {
        return $.extend({ type: 'rightAligned', width: 105, cellClass: ['sayi'], valueFormatter: tutarYaz, editable: girilir, valueParser: sayiParse, sortable: false }, ozellik);
    }

    function bosSatir() {
        var s = { _id: ++sonId, tertip: '', kurYapiKod: '', tertipPrg: '', ioiIsKod: 0, ioiIsAd: '', eklenen: 0, dusulen: 0, altProjeNo: '', sektorAd: '', bolgeler: '', altProjeAd: '' };
        for (var i = 1; i <= 12; i++) s['ay' + i] = 0;
        return s;
    }
    function satirToplam(d) { var t = 0; for (var i = 1; i <= 12; i++) t += Number(d['ay' + i]) || 0; return Math.round(t * 100) / 100; }
    function isKodAd(d) { return d && d.ioiIsKod > 0 ? d.ioiIsKod + '-' + (d.ioiIsAd || '') : ''; }

    function mercekHucre(komut, baslik) {
        return function (p) {
            var $h = $('<span class="mp-mercekli"></span>');
            $('<span class="mp-mercek-deger"></span>').text(p.valueFormatted !== undefined && p.valueFormatted !== null ? p.valueFormatted : (p.value || '')).appendTo($h);
            if (!p.node.rowPinned) {
                $('<button type="button" class="btn btn-link btn-xs mp-mercek"></button>').attr('title', baslik)
                    .html('<span class="glyphicon glyphicon-search" aria-hidden="true"></span><span class="sr-only">' + baslik + '</span>')
                    .on('click', function (e) { e.stopPropagation(); komut(p.node); })
                    .appendTo($h);
            }
            return $h[0];
        };
    }

    function komutHucre(p) {
        if (p.node.rowPinned) return '';
        var $h = $('<span class="mp-komutlar"></span>');
        $('<button type="button" class="btn btn-link btn-xs" title="Tertip bilgisi"><span class="glyphicon glyphicon-question-sign" aria-hidden="true"></span><span class="sr-only">Tertip bilgisi</span></button>')
            .on('click', function () { tertipBilgiAc(p.node); }).appendTo($h);
        if (durum.mpOnayli) {
            $('<button type="button" class="btn btn-link btn-xs" title="İş seçimi"><span class="glyphicon glyphicon-pushpin" aria-hidden="true"></span><span class="sr-only">İş seçimi</span></button>')
                .on('click', function () { isAc(p.node); }).appendTo($h);
        }
        return $h[0];
    }

    var aySutunlari = [];
    for (var i = 0; i < 12; i++) aySutunlari.push(sayiSutun({ field: 'ay' + (i + 1), headerName: AYLAR[i], colId: 'ay' + (i + 1) }));

    var sutunlar = [
        {
            headerName: 'Bütçe Gideri', children: [
                {
                    field: 'tertip', headerName: 'Tertip', width: 260, pinned: 'left', editable: girilir, sortable: false, tooltipField: 'tertip',
                    cellRenderer: mercekHucre(function (n) { tertipPencereAc(n.rowIndex); }, 'Tertip seç'),
                    valueParser: function (p) { return $.trim(p.newValue || ''); }
                },
                {
                    field: 'kurYapiKod', colId: 'kurYapiKod', headerName: 'Birim', width: 130, editable: girilir, sortable: false, tooltipField: 'kurYapiKod',
                    cellRenderer: mercekHucre(function (n) { kurYapiAc(n); }, 'Birim seç'),
                    valueParser: function (p) { return $.trim(p.newValue || ''); }
                },
                { field: 'tertipPrg', colId: 'tertipPrg', headerName: 'Tertip Prg.', width: 120, sortable: false, cellClass: salt([]), tooltipField: 'tertipPrg' },
                { colId: 'ioiIsKodAd', headerName: 'İş Kod', width: 140, sortable: false, cellClass: salt([]), valueGetter: function (p) { return isKodAd(p.data); }, tooltipValueGetter: function (p) { return isKodAd(p.data); } },
                { colId: 'komut', headerName: '', width: 62, sortable: false, resizable: false, cellRenderer: komutHucre }
            ]
        },
        { headerName: 'Düşülen', children: [sayiSutun({ field: 'dusulen', colId: 'dusulen', headerName: '', width: 115 })] },
        { headerName: 'Eklenen', children: [sayiSutun({ field: 'eklenen', colId: 'eklenen', headerName: '', width: 115 })] },
        { headerName: 'Aylar', children: aySutunlari },
        {
            headerName: 'Toplam', children: [sayiSutun({
                colId: 'toplam', headerName: '', width: 120, editable: false, cellClass: salt(['sayi']),
                valueGetter: function (p) { return p.node.rowPinned ? p.data.toplam : satirToplam(p.data); }
            })]
        },
        {
            headerName: 'Yatırım Prj. Bilgi', children: [
                { field: 'altProjeNo', headerName: 'Proje', width: 100, sortable: false, cellClass: salt([]) },
                { field: 'sektorAd', headerName: 'Sektör', width: 130, sortable: false, cellClass: salt([]), tooltipField: 'sektorAd' },
                { field: 'bolgeler', headerName: 'Bölge', width: 110, sortable: false, cellClass: salt([]), tooltipField: 'bolgeler' },
                { field: 'altProjeAd', headerName: 'Alt Prj. Ad', width: 200, sortable: false, cellClass: salt([]), tooltipField: 'altProjeAd' }
            ]
        }
    ];

    var seciliSatir = 0;
    var grid = Sgb.grid('#grdTertipler', {
        columnDefs: sutunlar,
        getRowId: function (p) { return String(p.data._id); },
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        tooltipShowDelay: 400,
        onCellFocused: function (e) { if (e.rowIndex !== null && e.rowPinned == null) seciliSatir = e.rowIndex; },
        onCellValueChanged: function (e) {
            if (e.newValue === e.oldValue) return;
            if (e.colDef.field === 'tertip') pebBilgiYaz(e.node);
            toplamYenile();
            if (e.colDef.field !== 'tertip') e.api.refreshCells({ rowNodes: [e.node], columns: ['toplam'] });
        }
    });

    var toplamAlanlari = ['dusulen', 'eklenen', 'ay1', 'ay2', 'ay3', 'ay4', 'ay5', 'ay6', 'ay7', 'ay8', 'ay9', 'ay10', 'ay11', 'ay12'];

    function tumSatirlar() {
        var r = [];
        grid.forEachNode(function (n) { r.push(n.data); });
        return r;
    }
    function doluSatirlar() { return $.grep(tumSatirlar(), function (d) { return $.trim(d.tertip || '') !== ''; }); }

    function toplamYenile() {
        var t = { tertip: 'T O P L A M', toplam: 0 };
        $.each(toplamAlanlari, function (i, a) { t[a] = 0; });
        $.each(tumSatirlar(), function (j, d) {
            $.each(toplamAlanlari, function (i, a) { t[a] += Number(d[a]) || 0; });
            t.toplam += satirToplam(d);
        });
        grid.setGridOption('pinnedBottomRowData', [t]);
    }

    function satirlariYaz(liste) {
        var r = $.map(liste || [], function (s) { return $.extend(bosSatir(), s, { _id: ++sonId }); });
        for (var k = r.length; k < BOS_SATIR; k++) r.push(bosSatir());
        grid.setGridOption('rowData', r);
        seciliSatir = 0;
        toplamYenile();
    }

    // Satır sayısı yetmezse sona boş satır eklenir.
    function satirGaranti(adet) {
        var eksik = adet - grid.getDisplayedRowCount();
        if (eksik <= 0) return;
        var ekle = [];
        for (var k = 0; k < eksik; k++) ekle.push(bosSatir());
        grid.applyTransaction({ add: ekle });
    }

    function satirGuncelle(node, degisen) {
        var g = grid.getRowNode(node.id);
        if (!g) return;
        g.setData($.extend({}, g.data, degisen));
    }

    // Eski PEBBilgiYaz: tertip girilince alt proje no, sektör, bölgeler ve program tertibi.
    function pebBilgiYaz(node) {
        var tertip = $.trim(node.data.tertip || '');
        if (!tertip) { satirGuncelle(node, { tertipPrg: '', altProjeNo: '', sektorAd: '', bolgeler: '' }); return; }
        Sgb.al(API + '/pebbilgi', { yil: yilOku(), tertip: tertip })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                satirGuncelle(node, { altProjeNo: v.altProjeNo, sektorAd: v.sektorAd, bolgeler: v.bolgeler, tertipPrg: v.tertipPrg });
            })
            .fail(hataGoster);
    }

    // Eski KolonlariAyarla: Ekle/Düş Göster işaretliyse düşülen / eklenen, değilse aylar; masraf planı onaylı değilse Birim,
    // Tertip Prg. ve İş Kod sütunları kapalı.
    function sutunlariAyarla() {
        var ed = ekDus();
        grid.setColumnsVisible(['dusulen', 'eklenen'], ed);
        grid.setColumnsVisible(aySutunlari.map(function (s) { return s.colId; }).concat(['toplam']), !ed);
        grid.setColumnsVisible(['kurYapiKod', 'tertipPrg', 'ioiIsKodAd'], durum.mpOnayli);
        grid.refreshCells({ columns: ['komut'], force: true });
    }
    $('#chkEklenenDusulen').on('change', sutunlariAyarla);

    // ---------------------------------------------------------------- işlem türü, birim, dayanaklar

    function dayanaklariYaz(liste, secili) {
        var $k = $('#cblDayanaklar').empty();
        var sec = (secili || '').split(',');
        if (!(liste || []).length) { $('<span class="mp-soluk"></span>').text('Dayanak yok.').appendTo($k); return; }
        $.each(liste, function (i, d) {
            var $l = $('<label class="checkbox mp-dayanak"></label>').attr('title', d.aciklama || d.ad);
            $('<input type="checkbox" />').val(d.kod).prop('checked', sec.indexOf(String(d.kod)) >= 0).appendTo($l);
            $l.append(document.createTextNode(' ' + d.ad)).appendTo($k);
        });
    }
    function dayanaklarOku() {
        return $('#cblDayanaklar input:checked').map(function () { return this.value; }).get().join(',');
    }
    function dayanaklariSec(secili) {
        var sec = (secili || '').split(',');
        $('#cblDayanaklar input').each(function () { this.checked = sec.indexOf(this.value) >= 0; });
    }

    // Eski ddlIslemTur_Degisti / SayfaInit: dayanaklar, alt işlemler ve Ekle/Düş Göster.
    function turYukle(secAltIslem, secDayanak) {
        return Sgb.al(API + '/tur', { yil: yilOku(), tur: turOku() })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                dayanaklariYaz(v.dayanaklar, secDayanak);
                var $a = $('#ddlAltIslem').empty();
                $.each(v.altIslemler || [], function (i, a) { $('<option></option>').val(a.kod).text(a.ad ? a.kod + ' - ' + a.ad : '').appendTo($a); });
                $a.val(secAltIslem || '');
                $('#chkEklenenDusulen').prop('checked', !!v.ekDus);
                sutunlariAyarla();
            })
            .fail(hataGoster);
    }
    $('#ddlIslemTur').on('change', function () { turYukle('', ''); });

    function seceneklerYaz($s, liste, deger) {
        $s.empty();
        $.each(liste || [], function (i, o) { $('<option></option>').val(o.kod).text(o.ad).appendTo($s); });
        if (deger !== undefined && deger !== null) $s.val(String(deger));
    }

    // Eski Page_Load / ddlYil_Degisti.
    function tanimYukle(sonra) {
        return Sgb.al(API + '/tanim', { yil: yilOku() })
            .done(function (t) {
                if (t.hata) { mesaj(t.hata, 'danger'); return; }
                durum.mpOnayli = !!t.mpOnayli;
                durum.yonetici = !!t.yonetici;
                durum.bugun = t.bugun || Sgb.bugun();
                $('#ddlYil').val(t.yil);
                var birim = $('#ddlBirim').val();
                seceneklerYaz($('#ddlBirim'), t.birimler, birim);
                if (!$('#ddlBirim').val() && (t.birimler || []).length) $('#ddlBirim').val(String(t.birimler[0].kod));
                var tur = $('#ddlIslemTur').val() || t.islemTur;
                seceneklerYaz($('#ddlIslemTur'), t.islemTurleri, tur);
                if (!$('#ddlIslemTur').val()) $('#ddlIslemTur').val(String(t.islemTur));
                $('.yonetici').prop('hidden', !durum.yonetici);
                $('#grpRapor, #btnYukle').prop('hidden', !durum.mpOnayli);
                if (!$('#txtEvrakTarih').val()) $('#txtEvrakTarih').val(durum.bugun);
                if (sonra) sonra(); else turYukle($('#ddlAltIslem').val(), dayanaklarOku());
            })
            .fail(hataGoster);
    }
    $('#ddlYil').on('change', function () { tKodlar.adlariYenile(); tanimYukle(); });

    // ---------------------------------------------------------------- oku (eski btnListele_Click, BelgeAc)

    function formYaz(v) {
        var f = v.form;
        $('#ddlYil').val(f.yil);
        $('#txtSiraNo').val(f.no);
        if (v.birim) {
            if (!$('#ddlBirim option').filter(function () { return this.value === v.birim; }).length)
                $('<option></option>').val(v.birim).text(v.birim).appendTo('#ddlBirim');
            $('#ddlBirim').val(v.birim);
        }
        if (f.islemTur) $('#ddlIslemTur').val(String(f.islemTur));
        $('#txtEvrakTarih').val(f.evrakTarih || '');
        $('#txtEvrakNo').val(f.evrakNo || '');
        $('#txtEvrakIlgi').val(f.evrakIlgi || '');
        $('#txtAciklama').val(f.aciklama || '');
        $('#txtBumkoNo').val(f.bumkoNo || '');
        $('#cbProjelerArasi').prop('checked', !!f.projelerArasi);
        $('#cbSektorlerArasi').prop('checked', !!f.sektorlerArasi);
        $('#cbBolgelerArasi').prop('checked', !!f.bolgelerArasi);
        $('#txtOnayYaziNo').val(f.onayYaziNo || '');
        $('#dfOnayYazisiTarih').val(f.onayYaziTarih || '');
        $('#lblDurum').text(f.durumAd ? '(' + f.durumAd + ')' : '');
        durum.odGonNo = f.odGonNo || '';
        durum.odGonTur = f.odGonTur || 0;
        durum.odGonYil = f.odGonYil || 0;
        durum.ilgiliSiraNo = f.ilgiliSiraNo === undefined ? -1 : f.ilgiliSiraNo;
        turYukle(f.altIslem, f.dayanaklar).then(function () {
            // Okunan belgede aylar gösterilir (eski BelgeAc sonrası KolonlariAyarla).
            $('#chkEklenenDusulen').prop('checked', false);
            sutunlariAyarla();
        });
        satirlariYaz(v.satirlar);
        ekliFormDegistir();
    }

    function oku(yil, no) {
        if (yil) $('#ddlYil').val(yil);
        if (no) $('#txtSiraNo').val(no);
        if (yilOku() <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        if (noOku() <= 0) { mesaj('Belge numarası giriniz.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(API + '/oku', { yil: yilOku(), no: noOku(), birim: $('#ddlBirim').val() || '', tur: turOku() })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                formYaz(v);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }
    $('#frmBelge').on('submit', function (e) { e.preventDefault(); oku(); });
    $('#txtSiraNo').on('change', function () { ekliFormDegistir(); });

    // Belge Arama sekmesinden açılan belge (eski BelgeAc).
    window.OgBelgeYukle = function (yil, no, birim) {
        var y = parseInt(yil, 10) || 0;
        if (birim) {
            if (!$('#ddlBirim option').filter(function () { return this.value === birim; }).length)
                $('<option></option>').val(birim).text(birim).appendTo('#ddlBirim');
            $('#ddlBirim').val(birim);
        }
        if (y && y !== yilOku()) {
            $('#ddlYil').val(y);
            tanimYukle(function () { oku(y, no); });
        } else oku(y, no);
    };

    // ---------------------------------------------------------------- kaydet, onayla, rakam dağıt

    function istekYap() {
        grid.stopEditing();
        return {
            Yil: yilOku(), No: noOku(), Birim: $('#ddlBirim').val() || '', IslemTur: turOku(), AltIslem: $('#ddlAltIslem').val() || '',
            EvrakTarih: $('#txtEvrakTarih').val(), EvrakNo: $('#txtEvrakNo').val(), EvrakIlgi: $('#txtEvrakIlgi').val(), Aciklama: $('#txtAciklama').val(),
            BumkoNo: $('#txtBumkoNo').val(), OdGonNo: durum.odGonNo, OdGonTur: durum.odGonTur, OdGonYil: durum.odGonYil,
            ProjelerArasi: $('#cbProjelerArasi').prop('checked'), SektorlerArasi: $('#cbSektorlerArasi').prop('checked'), BolgelerArasi: $('#cbBolgelerArasi').prop('checked'),
            OnayYaziNo: $('#txtOnayYaziNo').val(), OnayYaziTarih: $('#dfOnayYazisiTarih').val(), IlgiliSiraNo: durum.ilgiliSiraNo,
            Dayanaklar: dayanaklarOku(), EkDus: ekDus(),
            Satirlar: $.map(doluSatirlar(), function (d) {
                var s = { tertip: $.trim(d.tertip), kurYapiKod: $.trim(d.kurYapiKod || ''), tertipPrg: d.tertipPrg || '', eklenen: Number(d.eklenen) || 0, dusulen: Number(d.dusulen) || 0, ioiIsKod: d.ioiIsKod || 0, ioiIsAd: d.ioiIsAd || '' };
                for (var a = 1; a <= 12; a++) s['ay' + a] = Number(d['ay' + a]) || 0;
                return s;
            })
        };
    }

    function rakamlariYaz(satirlar) {
        // Sunucu dağıtılan rakamları döndürür; ekran bilgileri (program tertibi, proje vb.) aynı sıradaki satırdan korunur.
        var eski = doluSatirlar();
        satirlariYaz($.map(satirlar || [], function (s, i) {
            var e = eski[i] && $.trim(eski[i].tertip) === s.tertip ? eski[i] : {};
            return $.extend({}, e, s, {
                tertipPrg: s.tertipPrg || e.tertipPrg || '', altProjeNo: s.altProjeNo || e.altProjeNo || '', sektorAd: s.sektorAd || e.sektorAd || '',
                bolgeler: s.bolgeler || e.bolgeler || '', altProjeAd: s.altProjeAd || e.altProjeAd || ''
            });
        }));
    }

    $('#btnKaydet').on('click', function () {
        var ist = istekYap();
        if (ist.Yil <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        if (!ist.Satirlar.length) { mesaj('Tertip giriniz.', 'warning'); return; }
        if (!window.confirm('Form kaydedilecek. Onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(API + '/kaydet', ist)
            .done(function (v) {
                if (v.satirlar) rakamlariYaz(v.satirlar);
                if (!sonucGoster(v)) return;
                if (v.no) { $('#txtSiraNo').val(v.no); ekliFormDegistir(); }
                if (ist.EkDus) { $('#chkEklenenDusulen').prop('checked', false); sutunlariAyarla(); }
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnRakamDagit').on('click', function () {
        var ist = istekYap();
        if (ist.Yil <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        if (!ist.Satirlar.length) { mesaj('Tertip giriniz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(API + '/dagit', ist)
            .done(function (v) {
                if (!sonucGoster(v)) return;
                rakamlariYaz(v.satirlar);
                $('#chkEklenenDusulen').prop('checked', false);
                sutunlariAyarla();
                mesaj('Rakamlar aylara dağıtıldı.', 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnOnayla').on('click', function () {
        if (noOku() <= 0) { mesaj('Önce formu kaydediniz ya da okuyunuz.', 'warning'); return; }
        if (!window.confirm('Onaylamak istiyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(API + '/onayla', { Yil: yilOku(), No: noOku() })
            .done(function (v) { if (sonucGoster(v)) $('#lblDurum').text('(Onaylı)'); })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnOdenekGondermeUret').on('click', function () {
        if (noOku() <= 0) { mesaj('Önce formu kaydediniz ya da okuyunuz.', 'warning'); return; }
        if (!window.confirm('Onaylı formdaki taşra birimleri için ödenek gönderme / tenkis belgeleri üretilecek. Onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        mesaj('Belgeler üretiliyor, lütfen bekleyiniz...', 'info');
        Sgb.gonder(API + '/odgonduret', { Yil: yilOku(), No: noOku(), Aciklama: $('#txtAciklama').val() })
            .done(sonucGoster)
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- yazdır, raporlar

    $('#btnYazdir').on('click', function () {
        if (noOku() <= 0) { mesaj('Önce formu kaydediniz ya da okuyunuz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        mesaj('Form hazırlanıyor...', 'info');
        Sgb.indir(API + '/yazdir', { yil: yilOku(), no: noOku() }).then(function () { $d.prop('disabled', false); });
    });

    var $mnuRapor = $('#mnuRapor');
    function raporMenu(ac) {
        $mnuRapor.prop('hidden', !ac);
        $('#btnRapor').attr('aria-expanded', ac ? 'true' : 'false');
    }
    $('#btnRapor').on('click', function (e) { e.stopPropagation(); raporMenu($mnuRapor.prop('hidden')); });
    $(document).on('click', function () { raporMenu(false); });
    $mnuRapor.on('click', '[data-rapor]', function () {
        raporMenu(false);
        var birim = $('#ddlBirim').val() || '';
        if (yilOku() <= 0 || !birim) { mesaj('Yıl ve birim seçiniz.', 'warning'); return; }
        mesaj('Rapor hazırlanıyor...', 'info');
        Sgb.indir(API + '/rapor', { yil: yilOku(), birim: birim, birimAd: $('#ddlBirim option:selected').text(), tur: $(this).attr('data-rapor') });
    });

    // ---------------------------------------------------------------- Yükle (eski btnDosyaYukle_Click)

    function dosyaGonder(adres, veri) {
        return $.ajax({
            url: Sgb.KOK + API + '/' + adres, method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        });
    }

    $('#btnYukle').on('click', function () {
        if (yilOku() <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        $('#fuDosya').val('').trigger('click');
    });
    $('#fuDosya').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('yil', yilOku());
        var $d = $('#btnYukle').prop('disabled', true);
        mesaj('Dosya yükleniyor...', 'info');
        dosyaGonder('yukle', veri)
            .done(function (v) {
                if (!sonucGoster(v)) return;
                satirlariYaz(v.satirlar);
                $('#chkEklenenDusulen').prop('checked', false);
                sutunlariAyarla();
                mesaj((v.satirlar || []).length + ' satır yüklendi.', 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); $('#fuDosya').val(''); });
    });

    // ---------------------------------------------------------------- İşlem Üret (eski wndIslemUret)

    var dlgIslemUret = document.getElementById('dlgIslemUret');
    $('#btnIslemUret').on('click', function () { $('#fuIslemYap').val(''); dlgIslemUret.showModal(); });
    $('#btnIslemYardim').on('click', function () {
        var $p = $('#pnlIslemIpucu'), ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false').toggleClass('active', ac);
    });

    function dosyaAdi(baslik) {
        var m = /filename\*=UTF-8''([^;]+)/i.exec(baslik || '');
        if (m) return decodeURIComponent(m[1]);
        m = /filename="?([^";]+)"?/i.exec(baslik || '');
        return m ? m[1] : 'Tenkis.xlsx';
    }

    $('.islem-uret').on('click', function () {
        var dosya = $('#fuIslemYap')[0].files[0];
        if (!dosya) { mesaj('İşlem yapılacak dosyayı seçiniz.', 'warning'); return; }
        if (yilOku() <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        var tur = $(this).attr('data-tur');
        if (!window.confirm($.trim($(this).text()) + ' işlemi dosyadaki tertipler için yapılacak. Onaylıyor musunuz?')) return;
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('yil', yilOku());
        veri.append('tur', tur);
        var $d = $('.islem-uret').prop('disabled', true);
        mesaj('İşlem yapılıyor, lütfen bekleyiniz...', 'info');
        // Tenkis sonucu Excel dosyasıdır, diğerleri JSON; ikisi de aynı uçtan döner.
        fetch(Sgb.KOK + API + '/islemuret', { method: 'POST', body: veri, credentials: 'same-origin', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') } })
            .then(function (c) {
                if (c.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return null; }
                if (!c.ok) { hataGoster({ status: c.status }); return null; }
                var tip = c.headers.get('Content-Type') || '';
                if (tip.indexOf('json') >= 0) return c.json().then(sonucGoster);
                var ad = dosyaAdi(c.headers.get('Content-Disposition'));
                return c.blob().then(function (b) {
                    var yerel = URL.createObjectURL(b);
                    var a = document.createElement('a');
                    a.href = yerel;
                    a.download = ad;
                    document.body.appendChild(a);
                    a.click();
                    a.remove();
                    setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
                });
            })
            .catch(function () { mesaj('İşlem yapılamadı.', 'danger'); })
            .finally(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Ödenek Devri (eski btnOdenekDevri_Click)

    $('#btnOdenekDevri').on('click', function () {
        if (yilOku() <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        if (doluSatirlar().length && !window.confirm('Tablodaki satırların yerine geçen yılın ödenek devri yazılacak. Onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.al(API + '/odenekdevri', { yil: yilOku() })
            .done(function (v) {
                if (!sonucGoster(v)) return;
                satirlariYaz(v.satirlar);
                if (v.altIslem) $('#ddlAltIslem').val(v.altIslem);
                $('#chkEklenenDusulen').prop('checked', true);
                sutunlariAyarla();
                mesaj((v.satirlar || []).length + ' satır alındı.', 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- satır, kopyala, yapıştır

    $('#btnSatirEkle').on('click', function () { grid.applyTransaction({ add: [bosSatir()] }); });
    $('#btnSatirSil').on('click', function () {
        var n = grid.getDisplayedRowAtIndex(seciliSatir);
        if (!n) { mesaj('Silinecek satırı seçiniz.', 'warning'); return; }
        grid.applyTransaction({ remove: [n.data] });
        toplamYenile();
    });

    // Görünen veri sütunları (komut sütunu hariç), eski EXTVeriYapistir / CopyStore'daki gibi.
    function veriSutunlari() {
        return $.grep(grid.getAllDisplayedColumns(), function (c) { return c.getColId() !== 'komut'; });
    }

    $('#btnKopyala').on('click', function () {
        var r = doluSatirlar();
        if (!r.length) { mesaj('Kopyalanacak satır yok.', 'warning'); return; }
        var sutun = veriSutunlari();
        var sayiYaz = function (v) { return (Number(v) || 0).toLocaleString('tr-TR', { maximumFractionDigits: 2, useGrouping: false }); };
        var baslik = sutun.map(function (c) {
            var d = c.getColDef(), ust = c.getParent() && c.getParent().getColGroupDef();
            return d.headerName || (ust ? ust.headerName : '');
        });
        var metin = [baslik.join('\t')].concat(r.map(function (d) {
            return sutun.map(function (c) {
                var id = c.getColId();
                if (id === 'toplam') return sayiYaz(satirToplam(d));
                if (id === 'ioiIsKodAd') return isKodAd(d);
                var v = d[c.getColDef().field];
                return typeof v === 'number' ? sayiYaz(v) : (v || '');
            }).join('\t');
        })).join('\r\n');
        var tamam = function () { mesaj(r.length + ' satır panoya kopyalandı.', 'success'); };
        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(metin).then(tamam, function () { mesaj('Panoya kopyalanamadı.', 'danger'); });
        } else {
            var $t = $('<textarea class="sr-only"></textarea>').val(metin).appendTo(document.body);
            $t[0].select();
            try { document.execCommand('copy'); tamam(); } catch (e) { mesaj('Panoya kopyalanamadı.', 'danger'); }
            $t.remove();
        }
    });

    var dlgYapistir = document.getElementById('dlgYapistir');
    $('#btnYapistir').on('click', function () {
        $('#txtYapistir').val('');
        dlgYapistir.showModal();
        $('#txtYapistir').trigger('focus');
    });
    // Panodaki satırlar seçili satırdan başlayarak, görünen sütun sırasıyla yazılır; salt okunur sütunlar atlanır.
    $('#btnYapistirUygula').on('click', function () {
        var satirlar = $.grep(($('#txtYapistir').val() || '').split(/\r?\n/), function (s) { return $.trim(s) !== ''; });
        if (!satirlar.length) { mesaj('Yapıştırılacak veri yok.', 'warning'); return; }
        grid.stopEditing();
        var sutun = veriSutunlari();
        var bas = seciliSatir || 0;
        satirGaranti(bas + satirlar.length);
        var tertipDegisen = [];
        $.each(satirlar, function (i, s) {
            var node = grid.getDisplayedRowAtIndex(bas + i);
            if (!node) return;
            var h = s.split('\t'), yeni = {};
            $.each(sutun, function (j, c) {
                if (j >= h.length) return false;
                var d = c.getColDef(), alan = d.field;
                if (!alan || !d.editable) return;
                if (alan === 'tertip' || alan === 'kurYapiKod') yeni[alan] = $.trim(h[j]);
                else { var n = sayiCoz(h[j]); if (n !== null) yeni[alan] = Math.round(n * 100) / 100; }
            });
            node.setData($.extend({}, node.data, yeni));
            if (yeni.tertip !== undefined) tertipDegisen.push(node);
        });
        dlgYapistir.close();
        toplamYenile();
        $.each(tertipDegisen, function (i, n) { pebBilgiYaz(n); });
        mesaj(satirlar.length + ' satır yapıştırıldı.', 'success');
    });

    // ---------------------------------------------------------------- tertip seçimi (eski ListeTertip.aspx)

    var dlgTertip = document.getElementById('dlgTertip');
    var hedefSatir = 0, tertipSayi = 30, tertipMpOnayli = null, tertipGrid = null;

    function tertipGridKur(mpOnayli, odenekBaslik) {
        if (tertipGrid && tertipMpOnayli === mpOnayli) return;
        tertipMpOnayli = mpOnayli;
        if (tertipGrid) tertipGrid.destroy();
        var sayi = function (alan, ad) { return { field: alan, headerName: ad, width: 125, type: 'rightAligned', valueFormatter: tutarYaz }; };
        var s = [
            { field: 'TERTIP', headerName: 'Tertip', width: 250, tooltipField: 'TERTIP' },
            sayi('KALAN', 'Tertip Kalan'), sayi('ODENEK', odenekBaslik || 'Tertip Ödeneği'), sayi('GONDERILEN', 'Tertipden Gönderilen')
        ];
        if (mpOnayli) s.push(sayi('MP', 'Masraf Planı'));
        s.push({ field: 'ACIKLAMA', headerName: 'Açıklama', width: 200, tooltipField: 'ACIKLAMA' },
            { field: 'FAALIYETACIKLAMA', headerName: 'Faaliyet', width: 200, tooltipField: 'FAALIYETACIKLAMA' });
        tertipGrid = Sgb.grid('#grdTertipSec', {
            columnDefs: s,
            rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
            overlayNoRowsTemplate: '<span>Kıstasları girip Listele\'ye basınız.</span>',
            onRowDoubleClicked: function (e) { tertipleriAktar([e.data]); }
        });
    }

    // Tertip seçimindeki kod kutuları (Kurumsal, Finans, Ekonomik, Alt Faaliyet): yazılınca adı, mercekle kod ağacı (#dlgKod).
    var tKodlar = MPTavan.kodAlanlari(yilOku, hataGoster, API);

    function tertipListele(ilk, hepsi) {
        if (!tertipGrid) tertipGridKur(false);
        tertipGrid.setGridOption('loading', true);
        Sgb.al(API + '/tertipler', {
            yil: yilOku(), kur: tKodlar.kod('tkur'), fin: tKodlar.kod('tfin'), eko: tKodlar.kod('teko'), peb: tKodlar.kod('tpeb'),
            hepsi: hepsi ? 1 : '', sayi: tertipSayi, ilk: ilk ? 1 : ''
        }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            tertipGridKur(!!v.mpOnayli, v.odenekBaslik);
            tertipSayi = v.sayi || tertipSayi;
            if (v.uyari) mesaj(v.uyari, 'warning');
            tertipGrid.setGridOption('overlayNoRowsTemplate', '<span>' + (ilk && !(v.satirlar || []).length ? 'Kıstasları girip Listele\'ye basınız.' : 'Tertip bulunamadı.') + '</span>');
            tertipGrid.setGridOption('rowData', v.satirlar || []);
        }).fail(hataGoster).always(function () { if (tertipGrid) tertipGrid.setGridOption('loading', false); });
    }

    // Eski TertipGosterPEB: kurumsal kıstas seçili birimden gelir; birim seçilmemişse boş açılır, kıstas pencerede girilir.
    function tertipPencereAc(satir) {
        var birim = $('#ddlBirim').val() || '';
        hedefSatir = satir || 0;
        tertipSayi = 30;
        if (birim || !tKodlar.kod('tkur')) tKodlar.ata('tkur', birim);
        tKodlar.ata('tpeb', '', '');
        if (tertipGrid) tertipGrid.setGridOption('rowData', []);
        dlgTertip.showModal();
        tertipListele(true, false);
    }

    // Eski ListeTertipAktar: seçilen tertipler hedef satırdan başlayarak yazılır, her biri için PEB bilgisi okunur.
    function tertipleriAktar(secili) {
        if (!secili.length) { mesaj('Tertip seçiniz.', 'warning'); return; }
        satirGaranti(hedefSatir + secili.length);
        $.each(secili, function (i, t) {
            var node = grid.getDisplayedRowAtIndex(hedefSatir + i);
            if (!node) return;
            node.setData($.extend({}, node.data, { tertip: t.TERTIP }));
            pebBilgiYaz(node);
        });
        dlgTertip.close();
    }

    $('#frmTertipAra').on('submit', function (e) { e.preventDefault(); tertipListele(false, false); });
    $('#btnTertipHepsi').on('click', function () { tertipListele(false, true); });
    $('#btnTertipSec').on('click', function () { tertipleriAktar(tertipGrid ? tertipGrid.getSelectedRows() : []); });

    // ---------------------------------------------------------------- kurumsal yapı (eski KodAgac.aspx?kodTur=17)

    var dlgKurYapi = document.getElementById('dlgKurYapi');
    var kurAgac = null, kurAgacYil = 0, kurNode = null;
    function kurYapiAc(node) {
        if (!$('#ddlBirim').val()) { mesaj('Lütfen ilk önce birim bilgisini giriniz!', 'warning'); return; }
        kurNode = node;
        dlgKurYapi.showModal();
        if (kurAgac) {
            if (kurAgacYil !== yilOku()) { kurAgacYil = yilOku(); kurAgac.yenile(); }
            return;
        }
        kurAgacYil = yilOku();
        kurAgac = Sgb.agac(document.getElementById('agacKurYapi'), {
            yukle: function (ust) {
                return Sgb.al(API + '/kodlar', { yil: kurAgacYil, kod: 17, ust: ust ? ust.kod : '' }).then(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                    return v.dugumler;
                });
            },
            secilince: function (d) {
                dlgKurYapi.close();
                if (kurNode) { satirGuncelle(kurNode, { kurYapiKod: d.kod }); }
            },
            seciliMi: function (d) { return !!kurNode && kurNode.data.kurYapiKod === d.kod; },
            ilkAcik: true,
            bosMetin: 'Kod bulunamadı.',
            hata: hataGoster
        });
    }

    // ---------------------------------------------------------------- tertip bilgisi (eski MasrafPlaniTertipBilgi.aspx)

    function tertipBilgiAc(node) {
        var tertip = $.trim(node.data.tertip || '');
        if (!tertip) { mesaj('Satırda tertip yok.', 'warning'); return; }
        Sgb.pencere('MasrafPlaniTertipBilgi?' + $.param({ yil: yilOku(), tertip: tertip, kurumsalYapiKod: node.data.kurYapiKod || '' }), 'Tertip Bilgisi');
    }

    // ---------------------------------------------------------------- iş seçimi (eski IOIProjeSecim.aspx)

    var dlgIs = document.getElementById('dlgIs');
    var isNode = null, isGrid = null;
    function isGridKur() {
        if (isGrid) return;
        var sayi = function (alan, ad) { return { field: alan, headerName: ad, width: 120, type: 'rightAligned', valueFormatter: tutarYaz }; };
        isGrid = Sgb.grid('#grdIs', {
            columnDefs: [
                { field: 'siraNo', headerName: 'İş Kodu', width: 90 },
                { field: 'ioiProjeAdi', headerName: 'İş Adı', width: 280, tooltipField: 'ioiProjeAdi' },
                { field: 'ilAdi', headerName: 'İl Adı', width: 120 },
                sayi('aktarilanTutar', 'MP Onaylı'), sayi('islemleEklenen', 'MP Yeni'), sayi('iOIHarc', 'Gönderilen Onaylı'), sayi('gonderilen', 'Gönderilen Yeni')
            ],
            rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
            overlayNoRowsTemplate: '<span>Kıstasları girip Listele\'ye basınız.</span>',
            onRowDoubleClicked: function (e) { isSecildi(e.data.siraNo, e.data.ioiProjeAdi); }
        });
    }
    function isSecildi(kod, ad) {
        if (isNode) satirGuncelle(isNode, { ioiIsKod: parseInt(kod, 10) || 0, ioiIsAd: kod ? (ad || '') : '' });
        dlgIs.close();
    }
    function isListele(ilk) {
        isGridKur();
        isGrid.setGridOption('loading', true);
        Sgb.al(API + '/ioi', {
            yil: yilOku(), ky: isNode ? isNode.data.kurYapiKod || '' : '', muhasebe: $.trim($('#txtIsMuhasebe').val()), birim: $.trim($('#txtIsBirim').val()),
            ad: $.trim($('#txtIsAd').val()), isTur: $('input[name=isTur]:checked').val(), ilk: ilk ? 1 : ''
        }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            if (ilk) {
                $('#txtIsMuhasebe').val(v.muhasebe || '');
                $('#txtIsBirim').val(v.birim || '');
            }
            $('#lblIsBirimAd').text([v.muhasebeAd, v.birimAd].filter(Boolean).join(' / '));
            if (!ilk) isGrid.setGridOption('overlayNoRowsTemplate', '<span>İş bulunamadı.</span>');
            isGrid.setGridOption('rowData', v.satirlar || []);
        }).fail(hataGoster).always(function () { isGrid.setGridOption('loading', false); });
    }
    function isAc(node) {
        isNode = node;
        $('#txtIsAd').val('');
        isGridKur();
        isGrid.setGridOption('rowData', []);
        dlgIs.showModal();
        isListele(true);
    }
    $('#frmIsAra').on('submit', function (e) { e.preventDefault(); isListele(false); });
    $('#btnIsTemizle').on('click', function () { isSecildi(0, ''); });
    $('#btnIsKopyala').on('click', function () {
        if (!isGrid) return;
        var r = [];
        isGrid.forEachNode(function (n) { r.push(n.data); });
        if (!r.length) { mesaj('Kopyalanacak satır yok.', 'warning'); return; }
        var metin = ['İş Kodu\tİş Adı\tİl Adı\tMP Onaylı\tMP Yeni\tGönderilen Onaylı\tGönderilen Yeni'].concat(r.map(function (d) {
            return [d.siraNo, d.ioiProjeAdi, d.ilAdi, d.aktarilanTutar, d.islemleEklenen, d.iOIHarc, d.gonderilen].map(function (x) {
                return typeof x === 'number' ? x.toLocaleString('tr-TR', { maximumFractionDigits: 2, useGrouping: false }) : (x || '');
            }).join('\t');
        })).join('\r\n');
        if (navigator.clipboard && window.isSecureContext)
            navigator.clipboard.writeText(metin).then(function () { mesaj(r.length + ' satır panoya kopyalandı.', 'success'); });
    });

    // ---------------------------------------------------------------- Ekli Dosyalar (eski DosyaEkranAc)

    var ekli = null;
    function ekliForm() { return noOku() > 0 ? 'BI_' + yilOku() + '_' + noOku() : '-1'; }
    function ekliFormDegistir() { if (ekli) ekli.formDegistir(ekliForm()); }
    $('#btnEkliDosyalar').on('click', function () {
        if (noOku() <= 0) { mesaj('Formu kaydediniz!', 'warning'); return; }
        if (!ekli) ekli = Sgb.ekliDosyalar('#ekliDosyalar', { modul: 'BHM', form: ekliForm() });
        else ekliFormDegistir();
        document.getElementById('dlgEkli').showModal();
    });

    // ---------------------------------------------------------------- açılış

    // Yıl açılışta bulunulan yıl (tanım okunamasa da boş kalmaz).
    if (!yilOku()) $('#ddlYil').val(new Date().getFullYear());
    satirlariYaz([]);
    tanimYukle();

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
    function tabloYay() { yuksekligeYay(document.getElementById('grdTertipler'), 200); }
    $(window).on('resize', tabloYay);
    if (window.ResizeObserver) new ResizeObserver(tabloYay).observe(document.querySelector('.mp-revize-form .mp-ust'));
    tabloYay();
});
