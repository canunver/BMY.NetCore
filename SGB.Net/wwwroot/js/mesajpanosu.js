// Sistem Mesajları bilgi ekranı (KYM/MesajPanosuForm, eski MesajPanosuForm.aspx).
// Açılış sayfasında kendi çerçevesinde açılır; kullanıcıya yazılmış mesajları tarih (gün.ay) ve metinle listeler. Salt okunurdur.
$(function () {
    'use strict';

    var el = document.getElementById('grdKullaniciMesaj');
    var tablo = Sgb.grid(el, {
        columnDefs: [
            { field: 'tarih', headerName: el.dataset.tarih, width: 70, minWidth: 60, tooltipField: 'tarihTam' },
            { field: 'mesaj', headerName: el.dataset.mesaj, flex: 1, minWidth: 150, wrapText: true, autoHeight: true, cellClass: 'mesaj-hucre' }
        ],
        tooltipShowDelay: 400,
        overlayNoRowsTemplate: '<span class="text-muted">Mesaj yok</span>',
        getRowId: function (p) { return String(p.data.mesajNo); }
    });

    tablo.setGridOption('loading', true);
    Sgb.al('api/mesajpanosu/liste')
        .done(function (veri) {
            if (veri.hata) Sgb.mesaj('#hata', veri.hata);
            tablo.setGridOption('rowData', veri.mesajlar || []);
        })
        .fail(function (xhr) {
            if (xhr.status !== 401) Sgb.mesaj('#hata', 'Mesajlar okunamadı.');
        })
        .always(function () { tablo.setGridOption('loading', false); });
});
