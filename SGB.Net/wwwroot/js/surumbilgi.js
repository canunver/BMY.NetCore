// Sistem Dosyaları Sürüm Bilgisi (OrtakSayfa/SurumBilgi): uygulama klasöründeki dll dosyaları.
// Uç: /api/surumbilgi/liste?sgb= (OrtakSayfa/SurumBilgiUclari.cs). Varsayılan sıra dosya adına göre (eski sayfa gibi).
$(function () {
    'use strict';

    var UC = 'api/surumbilgi/';
    var $sayfa = $('.surum-bilgi');
    if (!$sayfa.length) return;

    function hataYaz(metin) { $('#hata').text(metin || '').prop('hidden', !metin); }
    function istekHatasi(xhr) {
        if (xhr && xhr.status === 401) return;
        hataYaz(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Dosya listesi okunamadı.');
    }

    // Tabloyu sayfanın kalan yüksekliğine yayar (kaydırma tablonun içinde kalır).
    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(enAz, Math.floor(window.innerHeight - ust - 24)) + 'px';
    }

    function buyuklukYaz(b) {
        if (b == null) return '';
        if (b < 1024) return b + ' B';
        if (b < 1024 * 1024) return (b / 1024).toLocaleString('tr-TR', { maximumFractionDigits: 1 }) + ' KB';
        return (b / 1024 / 1024).toLocaleString('tr-TR', { maximumFractionDigits: 1 }) + ' MB';
    }

    var metinKiyas = function (a, b) { return (a || '').localeCompare(b || '', 'tr', { sensitivity: 'base', numeric: true }); };

    var tabloEl = document.getElementById('grdDosya');
    var tablo = Sgb.grid(tabloEl, {
        columnDefs: [
            { field: 'ad', headerName: 'Dosya Adı', flex: 1, minWidth: 240, tooltipField: 'ad', sort: 'asc', comparator: metinKiyas },
            {
                field: 'tarih', headerName: 'Dosya Tarihi', width: 150, sortingOrder: ['desc', 'asc'],
                valueFormatter: function (p) { return p.data ? p.data.tarihStr : ''; }
            },
            { field: 'surum', headerName: 'Sürüm', width: 150, comparator: metinKiyas },
            { field: 'urunSurum', headerName: 'Ürün Sürümü', width: 150, comparator: metinKiyas },
            {
                field: 'buyukluk', headerName: 'Büyüklük', width: 110, type: 'rightAligned', sortingOrder: ['desc', 'asc'],
                valueFormatter: function (p) { return p.data ? buyuklukYaz(p.data.buyukluk) : ''; }
            },
            { field: 'firma', headerName: 'Üretici Firma', width: 220, tooltipField: 'firma', comparator: metinKiyas }
        ],
        getRowId: function (p) { return p.data.ad; },
        tooltipShowDelay: 400
    });

    var yay = function () { yuksekligeYay(tabloEl, 200); };
    $(window).on('resize', yay);

    function listele() {
        var $b = $('#btnYenile').prop('disabled', true);
        hataYaz('');
        tablo.setGridOption('loading', true);
        return Sgb.al(UC + 'liste?sgb=' + ($('#chkSGBdll').prop('checked') ? 'true' : 'false'))
            .done(function (v) {
                var s = v.satirlar || [];
                tablo.setGridOption('rowData', s);
                $('#lblAdet').text(s.length ? s.length + ' dosya' : '');
                $('#lblKlasor').text(v.klasor ? 'Klasör: ' + v.klasor : '');
                yay();
            })
            .fail(istekHatasi)
            .always(function () { tablo.setGridOption('loading', false); $b.prop('disabled', false); });
    }

    $('#btnYenile').on('click', listele);
    $('#chkSGBdll').on('change', listele);
    yay();
    // Eski sayfa da açılışta listeliyordu.
    listele();
});
