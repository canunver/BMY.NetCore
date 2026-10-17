// Faaliyet Tertip Kısıtlama (ButceMuhasebe/BagliEkoPebAnaForm): solda ilişki kurulmuş tertip ve faaliyetler (AG Grid, satırda Sil),
// sağda yeni tertip - faaliyet ilişkisi. Uçlar: /api/bagliekopeb/liste, /kaydet, /sil.
$(function () {
    'use strict';

    if (!$('#grdBagliEkoPeb').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var M = JSON.parse($('#metinler').text() || '{}');

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(M.IslemHata, 'danger');
    }
    function anahtar(d) { return d.tertip + '\u0001' + d.peb; }

    // ---------------------------------------------------------------- liste

    function silDugmesi(p) {
        var $d = $('<button type="button" class="btn btn-link btn-xs text-danger"></button>')
            .attr('title', M.MUHGNL002).attr('aria-label', M.MUHGNL002 + ': ' + p.data.tertip + ' / ' + p.data.peb)
            .append('<span class="glyphicon glyphicon-trash" aria-hidden="true"></span>')
            .on('click', function () { sil(p.data); });
        return $d[0];
    }

    // Tertip hücresi: değer seçilip kopyalanabilir; yanındaki düğme değeri panoya kopyalar.
    function panoyaKopyala(metin) {
        if (navigator.clipboard && window.isSecureContext)
            return navigator.clipboard.writeText(metin).then(function () { mesaj(M.Kopyalandi, 'success'); });
        var $t = $('<textarea readonly></textarea>').val(metin).css({ position: 'fixed', top: '-1000px' }).appendTo(document.body);
        $t[0].select();
        try { document.execCommand('copy'); mesaj(M.Kopyalandi, 'success'); } catch (e) { /* tarayıcı izin vermedi */ }
        $t.remove();
    }

    function tertipHucresi(p) {
        var $h = $('<span class="kopyalanir"></span>');
        $('<span></span>').text(p.value || '').appendTo($h);
        if (p.value)
            $('<button type="button" class="btn btn-link btn-xs"></button>')
                .attr('title', M.Kopyala).attr('aria-label', M.Kopyala + ': ' + p.value)
                .append('<span class="glyphicon glyphicon-copy" aria-hidden="true"></span>')
                .on('click', function () { panoyaKopyala(p.value); })
                .appendTo($h);
        return $h[0];
    }

    var grid = Sgb.grid('#grdBagliEkoPeb', {
        enableCellTextSelection: true,
        ensureDomOrder: true,
        columnDefs: [
            { colId: 'sil', headerName: M.MUHGNL002, width: 64, sortable: false, resizable: false, cellRenderer: silDugmesi },
            { field: 'tertip', headerName: M.BagliEkoPebAna013, width: 280, sort: 'asc', cellRenderer: tertipHucresi },
            { field: 'peb', headerName: M.BagliEkoPebAna014, flex: 1, minWidth: 160 }
        ],
        getRowId: function (p) { return anahtar(p.data); },
        overlayNoRowsTemplate: '<span>' + $('<div>').text(M.BagliEkoPebBos).html() + '</span>'
    });

    function oku() {
        grid.setGridOption('loading', true);
        return Sgb.al('api/bagliekopeb/liste')
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                grid.setGridOption('rowData', veri.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    $('#btnOku').on('click', oku);
    $('#frmSuz').on('submit', function (e) { e.preventDefault(); });
    $('#txtSuz').on('input', function () { grid.setGridOption('quickFilterText', this.value); });

    // ---------------------------------------------------------------- kaydet / sil

    $('#btnKaydet').on('click', function () {
        var istek = { tertip: $.trim($('#txtTertip').val()), peb: $.trim($('#txtPeb').val()) };
        if (!istek.tertip || !istek.peb) { mesaj(M.BagliEkoPebAna003, 'warning'); return; }
        if (!window.confirm(M.BagliEkoPebKaydetOnay)) return;

        $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/bagliekopeb/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                // Eski sayfadaki gibi yeni satır listenin sonuna eklenir (listede zaten varsa eklenmez).
                var satir = { tertip: s.tertip, peb: s.peb };
                if (!grid.getRowNode(anahtar(satir))) grid.applyTransaction({ add: [satir] });
                var n = grid.getRowNode(anahtar(satir));
                if (n) grid.ensureNodeVisible(n);
                $('#txtTertip, #txtPeb').val('');
                $('#txtTertip').trigger('focus');
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $('#btnKaydet').prop('disabled', false); });
    });

    $('#frmBagliEkoPeb').on('submit', function (e) {
        e.preventDefault();
        $('#btnKaydet').trigger('click');
    });

    function sil(d) {
        if (!window.confirm(M.BagliEkoPebSilOnay)) return;
        Sgb.gonder('api/bagliekopeb/sil', { tertip: d.tertip, peb: d.peb })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                grid.applyTransaction({ remove: [d] });
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster);
    }

    // ---------------------------------------------------------------- açılış (eski sayfa da açılışta okurdu)

    oku();
});
