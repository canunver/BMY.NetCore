// Tavan üstü ödenek talebi (eski ButceHazirlikv2/TavanUstuOdenek.aspx). Bütçe teklifi ekranındaki "Tavan Üstü" tuşuyla, işaretli
// tertipler için pencere (çerçeve) içinde açılır: her tertibin üç yıllık ödeneği yanında ek ödenek talebi ve gerekçe girilir.
// Tavanına ulaşmamış tasnif satırları varsa üstte uyarı gösterilir.
(function ($) {
    'use strict';

    var $sayfa = $('.tavan-ustu');
    if (!$sayfa.length) return;

    var YIL = parseInt($sayfa.attr('data-yil'), 10) || 0;
    var SENARYO = $sayfa.attr('data-senaryo') || '';
    var TERTIPLER = $sayfa.attr('data-tertipler') || '';
    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 3 });

    // "1.234.000" ya da "1234000,5" -> sayı. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function para(p) { return p.value == null || p.value === '' ? '' : tamSayi.format(p.value); }

    function girilir(p) { return p.data && p.data.dg && p.data.dg[p.colDef.field] ? 'girilir degisti' : 'girilir'; }

    function tertipCiz(p) {
        var $d = $('<div></div>');
        $('<span></span>').text(p.data.tertipStr || '').appendTo($d);
        $('<small></small>').text(p.data.tertipAd || '').appendTo($d);
        return $d[0];
    }

    function yilGrubu(i) {
        return {
            headerName: String(YIL + i),
            children: [
                { field: 'odenek' + i, headerName: 'Ödenek', type: 'sayi', width: 110, valueFormatter: para, cellClass: 'sayi kapali', headerClass: 'sayi' },
                { field: 'ekOdenek' + i, headerName: 'Ek Ödenek', type: 'sayi', width: 110, valueFormatter: para, cellClass: function (p) { return 'sayi ' + girilir(p); },
                    headerClass: 'sayi', editable: true, cellEditor: 'agTextCellEditor',
                    valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; } }
            ]
        };
    }

    var grid = Sgb.grid('#grdTavanUstu', {
        columnDefs: [
            { field: 'tertipStr', headerName: 'Tertip', width: 240, pinned: 'left', cellRenderer: tertipCiz, cellClass: 'tertip kapali',
                tooltipValueGetter: function (p) { return (p.data.tertipStr || '') + '\n' + (p.data.tertipAd || ''); } },
            yilGrubu(0), yilGrubu(1), yilGrubu(2),
            { field: 'gerekce', headerName: 'Gerekçe', flex: 1, minWidth: 260, editable: true, cellEditor: 'agLargeTextCellEditor', cellEditorPopup: true,
                cellEditorParams: { maxLength: 4000, rows: 6, cols: 60 }, cellClass: function (p) { return 'metin ' + girilir(p); }, tooltipField: 'gerekce' }
        ],
        columnTypes: { sayi: {} },
        rowHeight: 48,
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        tooltipShowDelay: 500,
        getRowId: function (p) { return p.data.id; },
        onCellValueChanged: function (e) {
            if (e.newValue === e.oldValue) return;
            e.data.dg = e.data.dg || {};
            e.data.dg[e.colDef.field] = true;
            e.api.refreshCells({ rowNodes: [e.node], columns: [e.column], force: true });
        },
        overlayNoRowsTemplate: '<span>Tavan üstü talep girilecek tertip bulunamadı.</span>'
    });

    function yukle() {
        $('#lblAciklama').text(YIL ? YIL + ' yılı, ' + SENARYO + ' aşaması' : '');
        if (!YIL || !SENARYO) { Sgb.bildirim('Yıl ve aşama boş olamaz!', 'warning'); return; }
        if (!TERTIPLER) { Sgb.bildirim('Tavan üstü değer gireceğiniz tertipleri seçiniz!', 'warning'); return; }
        grid.setGridOption('loading', true);
        Sgb.al('api/tavanustu/liste', { yil: YIL, senaryo: SENARYO, tertipler: TERTIPLER }).done(function (v) {
            if (v.hata) { Sgb.bildirim(v.hata, 'danger'); grid.setGridOption('rowData', []); return; }
            grid.setGridOption('rowData', $.map(v.satirlar || [], function (s, i) { return $.extend({ id: 's' + i }, s); }));
            $('#btnKaydet').prop('disabled', !(v.satirlar || []).length);
            var $l = $('#lstUyari').empty();
            $.each(v.uyarilar || [], function (i, u) { $('<li></li>').text(u).appendTo($l); });
            $('#pnlUyari').prop('hidden', !(v.uyarilar || []).length);
        }).fail(function (xhr) {
            if (xhr.status === 403) Sgb.bildirim('Bu işlem için yetkiniz yok.', 'danger');
            else if (xhr.status !== 401) Sgb.bildirim('Tavan üstü talepler okunamadı.', 'danger');
        }).always(function () { grid.setGridOption('loading', false); });
    }

    // Kaydet (eski btnKaydet_Click): tablodaki bütün satırlar gönderilir.
    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        var talepler = [];
        grid.forEachNode(function (n) {
            var d = n.data;
            talepler.push({ tertipStr: d.tertipStr, ekOdenek0: d.ekOdenek0 || 0, ekOdenek1: d.ekOdenek1 || 0, ekOdenek2: d.ekOdenek2 || 0, gerekce: d.gerekce || '' });
        });
        if (!talepler.length) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/tavanustu/kaydet', { yil: YIL, senaryo: SENARYO, talepler: talepler }).done(function (s) {
            if (s.hata) { Sgb.bildirim(s.hata, 'danger'); return; }
            // Düzenleme bitince gelen değişiklik olayı geç gelebilir; işaretler ondan sonra temizlenir.
            setTimeout(function () {
                grid.forEachNode(function (n) { n.data.dg = null; });
                grid.refreshCells({ force: true });
            }, 0);
            Sgb.bildirim(s.bilgi || 'Kayıt işlemi başarıyla gerçekleşti.', 'success');
        }).fail(function (xhr) {
            if (xhr.status === 403) Sgb.bildirim('Bu işlem için yetkiniz yok.', 'danger');
            else if (xhr.status !== 401) Sgb.bildirim('Kayıt yapılamadı.', 'danger');
        }).always(function () { $d.prop('disabled', false); });
    });

    yukle();
})(jQuery);
