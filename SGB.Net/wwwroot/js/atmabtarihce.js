// AB Dosya Tarihçesi (AlacakTakip/AlacakTakipABTarihce): AB Takip İşlemleri ekranında dosyanın Tarihçe sekmesinde çerçeve olarak
// açılır, dosyada yapılan işlemleri listeler (eski AlacakTakipABTarihce.aspx). Adres: ?dosyaAnahtar=&iPARDDonemi=&menuYok=1.
// Uç: /api/alacaktakip/ab/tarihce. Yenile tuşu (eski btnYenile) listeyi yeniden okur.
$(function () {
    'use strict';

    if (!$('#grdTarihce').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var p = new URLSearchParams(window.location.search);
    var dosyaAnahtar = parseInt(p.get('dosyaAnahtar'), 10) || -1;
    var iPARDDonemi = parseInt(p.get('iPARDDonemi'), 10) || 1;

    // "yyyy-MM-dd HH:mm" -> "gg.aa.yyyy SS:dd"
    function zamanYaz(e) {
        var d = e.value || '';
        return Sgb.tarih(d.substring(0, 10)) + (d.length > 10 ? d.substring(10) : '');
    }

    var grid = Sgb.grid('#grdTarihce', {
        columnDefs: [
            { headerName: '#', width: 60, sortable: false, type: 'rightAligned', valueGetter: function (e) { return e.node.rowIndex + 1; } },
            { field: 'zamanDT', headerName: 'Tarih/Saat', width: 140, valueFormatter: zamanYaz },
            { field: 'Kim', headerName: 'İşlemi Yapan', width: 200, tooltipField: 'Kim' },
            { field: 'islemAd', headerName: 'İşlem', width: 200, tooltipField: 'islemAd' },
            {
                field: 'metin', headerName: 'Alan Değerleri', flex: 1, minWidth: 240,
                wrapText: true, autoHeight: true, cellStyle: { 'white-space': 'pre-wrap', 'line-height': '18px', 'padding-top': '5px', 'padding-bottom': '5px' }
            }
        ],
        overlayNoRowsTemplate: '<span>Kayıt bulunamadı.</span>'
    });

    function yenile() {
        if (dosyaAnahtar <= 0) { grid.setGridOption('rowData', []); return; }
        grid.setGridOption('loading', true);
        Sgb.al('api/alacaktakip/ab/tarihce', { iPARDDonemi: iPARDDonemi, dosyaAnahtar: dosyaAnahtar })
            .done(function (v) {
                if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(function (xhr) {
                if (xhr && xhr.status === 401) return;
                Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Tarihçe okunamadı.', 'danger');
            })
            .always(function () { grid.setGridOption('loading', false); });
    }

    $('#btnYenile').on('click', yenile);
    yenile();
});
