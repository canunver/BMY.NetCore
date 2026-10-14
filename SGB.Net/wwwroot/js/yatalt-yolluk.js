// Yolluk Planı (ButceHazirlikv2/AltProjeYollukPlan): ProjeDetay / AltProjeDetay sekmesinde açılır. Projenin (alt projenin) bağlı
// faaliyetlerinin yolluk planları salt okunur listelenir; Yazdır aynı planı Yolluk Planlama raporu (Excel) olarak verir.
// Ölçüt sayfa adresinden gelir: yil, senaryoKod, projeKodu, altProjeKodu.
$(function () {
    'use strict';

    if (!$('#grdYolluk').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var adres = new URLSearchParams(location.search);
    var olcut = {
        yil: adres.get('yil') || '',
        senaryoKod: adres.get('senaryoKod') || '',
        projeKodu: adres.get('projeKodu') || '0',
        altProjeKodu: adres.get('altProjeKodu') || '0'
    };
    var tamsayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function sayiSutun(alan, baslik, genislik) {
        return {
            field: alan, headerName: baslik, width: genislik, cellClass: 'sayi', headerClass: 'sayi',
            valueFormatter: function (p) { return p.value == null || p.value === '' ? '' : tamsayi.format(p.value); }
        };
    }

    // Sütunlar eski kaynak metinleriyle (YollukPlanlama020..031).
    var grid = Sgb.grid('#grdYolluk', {
        columnDefs: [
            { field: 'dairesi', headerName: 'Dairesi', width: 250, tooltipField: 'dairesi' },
            { field: 'konusu', headerName: 'Konusu', width: 250, tooltipField: 'konusu' },
            { field: 'muhtelifMi', headerName: 'Muhtelif mi?', width: 100, cellRenderer: 'agCheckboxCellRenderer', cellRendererParams: { disabled: true } },
            sayiSutun('suresi', 'Süresi (Yol Hariç)', 120),
            sayiSutun('kisiSayisi', 'Kişi Sayısı', 100),
            { field: 'yapilacagiAy', headerName: 'Yapılacağı Ay', width: 200 },
            { field: 'yeri', headerName: 'Yeri', width: 200, tooltipField: 'yeri' },
            { field: 'tertip', headerName: 'Tertip', width: 300, tooltipField: 'tertip' },
            sayiSutun('maliyet', 'Maliyet', 130)
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        overlayNoRowsTemplate: '<span>Yolluk planı yok.</span>'
    });

    // Liste (eski Listele).
    function oku() {
        grid.setGridOption('loading', true);
        Sgb.al('api/yatalt/yolluk/liste', olcut)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', v.satirlar);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    // Yazdır (eski btnYazdir_Click → Rapor 2003-14), Excel.
    $('#btnYazdir').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/yatalt/yolluk/rapor', olcut).then(function () { $d.prop('disabled', false); });
    });

    oku();
});
