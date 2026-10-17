// Yatırım Projeleri Geçmiş Veri Girişi (ButceHazirlikv2/YatTopluVeriGirisForm; eski YatTopluVeriGirisForm.aspx).
// Listele seçime uyan bütçe kaynaklarını getirir; rakam sütunları girilir. Kaydet tablodaki bütün satırları, listelenen yıl ve
// aşamaya kaydeder (eski getRowsValues). Kopyala bütün satırları panoya yazar (eski CopyStore); Yapıştır panodaki değerleri seçili
// satırdan başlayarak Proje Tutarı (Dış) sütunundan itibaren yazar (eski EXTVeriYapistir(..., 5, ...)). Ctrl+V de çalışır.
$(function () {
    'use strict';

    if (!$('#grdToplu').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var BILGI = ['dptProjeNo', 'dptProjeAd', 'altProjeAdi', 'kaynakAd', 'kaynakNo'];
    var RAKAM = ['projeToplam_Dis', 'projeToplam_Toplam', 'odenek_Dis', 'odenek_Toplam', 'revizeOdenek_Dis', 'revizeOdenek_Toplam',
        'harcama_Dis', 'harcama_Toplam', 'kumulatifHarcama_Dis', 'kumulatifHarcama_Toplam'];

    var okunan = null; // { yil, senaryo, asamaAd }: kayıt listelenen yıl ve aşamaya yapılır
    var tamsayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) {
        $('#btnOku, #btnYapistir').prop('disabled', evet);
        $('#btnKaydet').prop('disabled', evet || !okunan);
    }

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    // ---------------------------------------------------------------- tablo

    function bilgiSutun(alan, baslik, genislik, ozellik) {
        return $.extend({ field: alan, headerName: baslik, width: genislik, tooltipField: alan, cellClass: 'kapali', pinned: 'left' }, ozellik || {});
    }

    function rakamSutun(alan, baslik) {
        return {
            field: alan, headerName: baslik, width: 105, minWidth: 80, sortable: false, headerClass: 'sayi', editable: true,
            cellEditor: 'agTextCellEditor',
            cellClass: function (p) { return ['sayi', 'girilir'].concat(p.data && p.data._degisen && p.data._degisen[alan] ? ['degisti'] : []); },
            valueFormatter: function (p) { return p.value == null || p.value === '' ? '' : tamsayi.format(p.value); },
            valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; }
        };
    }

    var grid = Sgb.grid('#grdToplu', {
        columnDefs: [
            bilgiSutun('dptProjeNo', 'DPT Proje No', 90),
            bilgiSutun('dptProjeAd', 'DPT Proje Adı', 230),
            bilgiSutun('altProjeAdi', 'Alt Proje Adı', 140, { pinned: null }),
            bilgiSutun('kaynakAd', 'Kaynak Adı', 160, { pinned: null }),
            bilgiSutun('kaynakNo', 'No', 60, { pinned: null, headerClass: 'sayi', cellClass: ['kapali', 'sayi'] }),
            { headerName: 'Proje Tutarı', children: [rakamSutun('projeToplam_Dis', 'Dış'), rakamSutun('projeToplam_Toplam', 'Toplam')] },
            { headerName: 'Ödenek', children: [rakamSutun('odenek_Dis', 'Dış'), rakamSutun('odenek_Toplam', 'Toplam')] },
            { headerName: 'Revize Ödenek', children: [rakamSutun('revizeOdenek_Dis', 'Dış'), rakamSutun('revizeOdenek_Toplam', 'Toplam')] },
            { headerName: 'Harcama', children: [rakamSutun('harcama_Dis', 'Dış'), rakamSutun('harcama_Toplam', 'Toplam')] },
            { headerName: 'Kümülatif Harcama (Res.Gaz.)', children: [rakamSutun('kumulatifHarcama_Dis', 'Dış'), rakamSutun('kumulatifHarcama_Toplam', 'Toplam')] }
        ],
        overlayNoRowsTemplate: '<span class="ag-overlay-no-rows-center">Yıl, aşama ve sektörü seçip Listele\'ye basınız.</span>',
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        onCellValueChanged: function (e) {
            if (e.oldValue === e.newValue) return;
            e.data._degisen = e.data._degisen || {};
            e.data._degisen[e.colDef.field] = true;
            e.api.refreshCells({ rowNodes: [e.node], columns: [e.colDef.field], force: true });
        }
    });

    function satirlar() {
        var liste = [];
        grid.forEachNode(function (n) { liste.push(n.data); });
        return liste;
    }

    // ---------------------------------------------------------------- açılış

    function seceneklerDoldur($s, liste, secili) {
        $s.empty();
        $.each(liste || [], function (i, o) { $('<option></option>').val(o.kod).text(o.ad).appendTo($s); });
        if (secili !== undefined && secili !== null) $s.val(secili);
        if ($s.val() === null && $s.children().length) $s.prop('selectedIndex', 0);
    }

    Sgb.al('api/yattoplu/tanim').done(function (v) {
        if (v.hata) { mesaj(v.hata, 'danger'); return; }
        $('#txtYil').val(v.yil);
        seceneklerDoldur($('#ddlAsama'), v.asamalar, v.senaryo);
        seceneklerDoldur($('#ddlSektor'), v.sektorler, '');
    }).fail(hataGoster);

    // ---------------------------------------------------------------- Listele / Kaydet

    function oku() {
        var yil = parseInt($('#txtYil').val(), 10) || 0;
        if (yil < 2000) { mesaj('Yılı yazınız.', 'warning'); $('#txtYil').trigger('focus'); return; }
        var o = { yil: yil, senaryo: $('#ddlAsama').val() || '', sektor: $('#ddlSektor').val() || '', projeAd: $.trim($('#txtProjeAd').val()) };
        mesgul(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/yattoplu/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var liste = v.satirlar || [];
                grid.setGridOption('rowData', liste);
                okunan = { yil: o.yil, senaryo: o.senaryo, asamaAd: $('#ddlAsama option:selected').text() };
                $('#lblOkunan').text(okunan.yil + ' yılı, ' + okunan.asamaAd + ' aşaması: ' + liste.length + ' kaynak');
                if (!liste.length) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); mesgul(false); });
    }

    $('#btnOku').on('click', oku);
    $('#frmSecim').on('submit', function (e) { e.preventDefault(); oku(); });

    $('#btnKaydet').on('click', function () {
        if (!okunan) return;
        grid.stopEditing();
        var liste = satirlar();
        if (!liste.length) { mesaj('Kaydedilecek satır yok.', 'warning'); return; }
        if (!window.confirm('Kaydetmek istiyor musunuz?')) return;
        var istek = {
            yil: okunan.yil, senaryo: okunan.senaryo,
            satirlar: $.map(liste, function (d) {
                var s = { kaynakNo: d.kaynakNo };
                $.each(RAKAM, function (i, alan) { s[alan] = Number(d[alan]) || 0; });
                return s;
            })
        };
        mesgul(true);
        Sgb.gonder('api/yattoplu/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                $.each(liste, function (i, d) { delete d._degisen; });
                grid.refreshCells({ force: true });
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // ---------------------------------------------------------------- Kopyala / Yapıştır

    // Eski CopyStore: bütün satırlar, bütün alanlar sekme ayraçlı; rakamlar binlik ayraçlı.
    $('#btnKopyala').on('click', function () {
        var liste = satirlar();
        if (!liste.length) { mesaj('Kopyalanacak satır yok.', 'warning'); return; }
        var metin = $.map(liste, function (d) {
            return $.map(BILGI, function (a) { return d[a] == null ? '' : String(d[a]).replace(/[\t\r\n]+/g, ' '); })
                .concat($.map(RAKAM, function (a) { return tamsayi.format(Number(d[a]) || 0); })).join('\t');
        }).join('\r\n') + '\r\n';
        if (!navigator.clipboard) { mesaj('Tarayıcı panoya yazmaya izin vermiyor.', 'warning'); return; }
        navigator.clipboard.writeText(metin)
            .then(function () { mesaj(liste.length + ' satır panoya kopyalandı.', 'info'); })
            .catch(function () { mesaj('Tarayıcı panoya yazmaya izin vermiyor.', 'warning'); });
    });

    // Değerleri (satırlar \n, sütunlar \t) verilen satırdan ve rakam sütunundan başlayarak yazar. Satır eklenmez.
    function yapistir(metin, satirSira, ilkRakam) {
        var satirlarMetin = String(metin || '').replace(/\r/g, '').split('\n');
        if (satirlarMetin.length && satirlarMetin[satirlarMetin.length - 1] === '') satirlarMetin.pop();
        var sayac = 0;
        $.each(satirlarMetin, function (i, satir) {
            var dugum = grid.getDisplayedRowAtIndex(satirSira + i);
            if (!dugum) return false;
            $.each(satir.split('\t'), function (j, deger) {
                var alan = RAKAM[ilkRakam + j];
                if (!alan) return false;
                var n = sayiCoz(deger);
                if (n === null) return;
                dugum.setDataValue(alan, n);
                sayac++;
            });
        });
        if (sayac) mesaj(sayac + ' hücre yapıştırıldı.', 'info');
    }

    $('#btnYapistir').on('click', function () {
        var secilen = grid.getSelectedNodes();
        if (!secilen.length) { mesaj('Lütfen satır seçiniz!', 'warning'); return; }
        if (!navigator.clipboard || !navigator.clipboard.readText) {
            mesaj('Tarayıcı panodan okumaya izin vermiyor; bir hücre seçip Ctrl+V ile yapıştırınız.', 'warning');
            return;
        }
        navigator.clipboard.readText()
            .then(function (metin) { yapistir(metin, secilen[0].rowIndex, 0); })
            .catch(function () { mesaj('Tarayıcı panodan okumaya izin vermiyor; bir hücre seçip Ctrl+V ile yapıştırınız.', 'warning'); });
    });

    // Ctrl+V: odaklı rakam hücresinden başlar (bilgi sütunundaysa Proje Tutarı (Dış) sütunundan).
    document.getElementById('grdToplu').addEventListener('paste', function (olay) {
        if (grid.getEditingCells().length) return; // düzenleyicideki yapıştırma olağan çalışır
        var odak = grid.getFocusedCell();
        if (!odak) return;
        var metin = (olay.clipboardData || window.clipboardData).getData('text');
        if (!metin) return;
        olay.preventDefault();
        var sira = RAKAM.indexOf(odak.column.getColId());
        yapistir(metin, odak.rowIndex, sira < 0 ? 0 : sira);
    });
});
