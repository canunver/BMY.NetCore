// Avans Sorgulama, ButceMuhasebe/AvansSorgulaForm (eski aynı adlı Ext.NET sayfası): avans hesaplarının kişi / hesap (/ tertip)
// bazında borç - alacak toplamları. Satır başındaki + ile o satırın belge hareketleri açılır (eski RowExpander / BeforeExpand).
// Kopyala listeyi sekme ayrımlı metin olarak panoya alır (eski CopyStore). Uçlar: /api/avans/sorgu, /api/avans/sorgu/detay
// (Muhasebe/AvansUclari.cs). Birim seçimi Sgb.muhasebeBirim.
$(function () {
    'use strict';

    if (!$('.avans-sorgu').length) return;

    var UC = 'api/avans/';
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var sonKriter = null;   // son listelemenin kıstasları (ayrıntı aynı kıstaslarla okunur)
    var acik = {};

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function paraMetin(v) { return v === null || v === undefined || v === '' ? '' : tutar2.format(Number(v) || 0); }
    function para(x) { return x.data && x.data._detay ? '' : paraMetin(x.value); }
    function tarihMetin(v) { if (!v) return ''; var p = String(v).split('-'); return p.length === 3 ? p[2] + '.' + p[1] + '.' + p[0] : v; }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim' },
        yil: function () { return $('#txtYil').val(); }
    });

    function anahtar(d) { return d.sira + '|' + d.tCVno + '|' + d.hesapNo + '|' + d.tertip; }

    function acHucre(x) {
        if (!x.data || x.data._detay) return '';
        var a = !!acik[anahtar(x.data)];
        return $('<button type="button" class="btn btn-link btn-xs avans-ac"></button>')
            .attr('title', a ? 'Hareketleri kapat' : 'Belge hareketlerini göster').attr('aria-expanded', a ? 'true' : 'false')
            .text(a ? '−' : '+')
            .on('click', function (e) { e.stopPropagation(); ayrintiDegistir(x.node); })[0];
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { colId: 'ac', headerName: '', width: 40, minWidth: 40, maxWidth: 40, sortable: false, resizable: false, cellRenderer: acHucre },
            { field: 'tCVno', headerName: 'TC Vno', minWidth: 110, flex: 1 },
            { field: 'adSoyadUnvan', headerName: 'İlgili', minWidth: 200, flex: 4 },
            { field: 'hesapNo', headerName: 'Hesap No', minWidth: 110, flex: 1 },
            { field: 'hesapAdi', headerName: 'Hesap Adı', minWidth: 160, flex: 3 },
            { field: 'tertip', headerName: 'Tertip', minWidth: 150, flex: 2 },
            { field: 'sonTarih', headerName: 'Son Tarih', minWidth: 100, flex: 1, valueFormatter: function (x) { return tarihMetin(x.value); } },
            { field: 'borc', headerName: 'Borç', minWidth: 120, flex: 1, type: 'rightAligned', valueFormatter: para },
            { field: 'alacak', headerName: 'Alacak', minWidth: 120, flex: 1, type: 'rightAligned', valueFormatter: para }
        ],
        getRowId: function (x) { return x.data._detay ? 'detay-' + x.data._anahtar : anahtar(x.data); },
        isFullWidthRow: function (x) { return !!(x.rowNode.data && x.rowNode.data._detay); },
        fullWidthCellRenderer: ayrintiHucre,
        getRowHeight: function (x) { return x.data && x.data._detay ? (x.data._yukseklik || 90) : undefined; },
        onSortChanged: function () { ayrintilariKapat(); },
        overlayNoRowsTemplate: '<span>Kıstasları girip Listele\'ye basınız.</span>'
    });

    function ayrintiDegistir(node) {
        var d = node.data, a = anahtar(d);
        if (acik[a]) {
            grid.applyTransaction({ remove: [acik[a]] });
            delete acik[a];
        } else if (sonKriter) {
            acik[a] = { _detay: true, _anahtar: a, satirlar: null };
            grid.applyTransaction({ add: [acik[a]], addIndex: node.rowIndex + 1 });
            Sgb.al(UC + 'sorgu/detay', $.extend({}, sonKriter, { hesapNo: d.hesapNo, tcVno: d.tCVno, tertip: d.tertip }))
                .done(function (v) {
                    if (!acik[a]) return;
                    if (v.hata) { mesaj(v.hata, 'danger'); acik[a].satirlar = []; } else acik[a].satirlar = v.satirlar || [];
                    ayrintiYenile(a);
                })
                .fail(hataGoster);
        }
        grid.refreshCells({ rowNodes: [node], columns: ['ac'], force: true });
    }

    function ayrintiYenile(a) {
        var n = grid.getRowNode('detay-' + a);
        if (!n) return;
        n.data._yukseklik = Math.min(320, 24 + 30 + 29 * Math.max(1, (n.data.satirlar || []).length));
        grid.resetRowHeights();
        grid.redrawRows({ rowNodes: [n] });
    }

    function ayrintilariKapat() {
        var l = $.map(acik, function (v) { return v; });
        acik = {};
        if (l.length) grid.applyTransaction({ remove: l });
        grid.refreshCells({ columns: ['ac'], force: true });
    }

    // Eski alt tablo: BelgeNo (sira), Tarih, Açıklama (adSoyadUnvan), Tertip, Borç, Alacak.
    function ayrintiHucre(x) {
        var d = x.data, $k = $('<div class="avans-ayrinti-satir"></div>');
        if (d.satirlar === null) { $('<div class="avans-not"></div>').text('Hareketler okunuyor...').appendTo($k); return $k[0]; }
        var $t = $('<table class="avans-ayrinti-tablo"><thead><tr><th>Belge No</th><th>Tarih</th><th>Açıklama</th><th>Tertip</th><th class="avans-sag">Borç</th><th class="avans-sag">Alacak</th></tr></thead><tbody></tbody></table>');
        var $b = $t.find('tbody');
        if (!d.satirlar.length) $('<tr><td colspan="6" class="avans-not">Hareket yok.</td></tr>').appendTo($b);
        $.each(d.satirlar, function (i, r) {
            $('<tr></tr>')
                .append($('<td class="avans-sag"></td>').text(r.sira || ''))
                .append($('<td></td>').text(tarihMetin(r.sonTarih)))
                .append($('<td></td>').text(r.adSoyadUnvan || ''))
                .append($('<td class="avans-kod"></td>').text(r.tertip || ''))
                .append($('<td class="avans-sag"></td>').text(paraMetin(r.borc)))
                .append($('<td class="avans-sag"></td>').text(paraMetin(r.alacak)))
                .appendTo($b);
        });
        $k.append($t);
        return $k[0];
    }

    function kriter() {
        var mb = muhBirim.deger();
        return {
            yil: parseInt($('#txtYil').val(), 10) || 0,
            durum: $('#ddlDurum').val(),
            muhasebe: mb.muhasebe || '',
            birim: mb.birim,
            hesapKod: $.trim($('#txtHesapKod').val()),
            tcVno: $.trim($('#txtTcVno').val()),
            tertipli: $('#chkTertipli').prop('checked') ? 1 : 0,
            altHesaptan: $('#chkAltHesaptan').prop('checked') ? 1 : 0,
            farklilar: $('#chkFarklilar').prop('checked') ? 1 : 0
        };
    }

    function ozetYaz(satirlar) {
        if (!satirlar || !satirlar.length) { $('#lblOzet').text(''); return; }
        var b = 0, a = 0;
        $.each(satirlar, function (i, r) { b += Number(r.borc) || 0; a += Number(r.alacak) || 0; });
        $('#lblOzet').text(satirlar.length + ' kayıt. Borç ' + tutar2.format(b) + ', Alacak ' + tutar2.format(a));
    }

    function listele() {
        var k = kriter();
        if (!k.yil || !k.durum) { mesaj('Anahtar alanları girmelisiniz.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        acik = {};
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'sorgu', k)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                sonKriter = k;
                grid.setGridOption('rowData', v.satirlar || []);
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
                ozetYaz(v.satirlar);
                if (v.bilgi) mesaj(v.bilgi, 'info');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); $d.prop('disabled', false); });
    }

    $('#btnListele').on('click', listele);
    $('#frmKriter').on('keydown', 'input', function (e) {
        if (e.key === 'Enter' && !$(this).closest('#muhBirim').length) { e.preventDefault(); listele(); }
    });

    // Kopyala (eski CopyStore): görünen sırayla bütün satırlar, sekme ayrımlı; Excel'e yapıştırılabilir.
    $('#btnKopyala').on('click', function () {
        var basliklar = ['TC Vno', 'İlgili', 'Hesap No', 'Hesap Adı', 'Tertip', 'Son Tarih', 'Borç', 'Alacak'];
        var satirlar = [basliklar.join('\t')];
        grid.forEachNodeAfterFilterAndSort(function (n) {
            var r = n.data;
            if (!r || r._detay) return;
            satirlar.push([r.tCVno, r.adSoyadUnvan, r.hesapNo, r.hesapAdi, r.tertip, tarihMetin(r.sonTarih), paraMetin(r.borc), paraMetin(r.alacak)]
                .map(function (h) { return String(h === null || h === undefined ? '' : h).replace(/[\t\r\n]+/g, ' '); }).join('\t'));
        });
        if (satirlar.length < 2) { mesaj('Kopyalanacak kayıt yok.', 'warning'); return; }
        var metin = satirlar.join('\r\n');
        if (navigator.clipboard && navigator.clipboard.writeText) {
            navigator.clipboard.writeText(metin).then(function () { mesaj((satirlar.length - 1) + ' satır panoya kopyalandı.', 'success'); },
                function () { eskiKopyala(metin, satirlar.length - 1); });
        } else eskiKopyala(metin, satirlar.length - 1);
    });

    function eskiKopyala(metin, adet) {
        var $t = $('<textarea class="avans-gizli-metin" readonly></textarea>').val(metin).appendTo('body');
        $t[0].select();
        var oldu = false;
        try { oldu = document.execCommand('copy'); } catch (e) { oldu = false; }
        $t.remove();
        mesaj(oldu ? adet + ' satır panoya kopyalandı.' : 'Panoya kopyalanamadı.', oldu ? 'success' : 'danger');
    }

    Sgb.al(UC + 'tanim')
        .done(function (t) { Sgb.yilKutusu('#txtYil', t.yillar, t.yil); })
        .fail(hataGoster);

    function yay() {
        var el = document.getElementById('grdListe');
        if (!el || el.offsetParent === null) return;
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(260, Math.floor(window.innerHeight - ust - 24)) + 'px';
    }
    $(window).on('resize', yay);
    yay();
});
