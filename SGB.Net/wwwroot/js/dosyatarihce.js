// Dosya Tarihçesi (AlacakTakip/DosyaTarihce): Takip İşlemleri listesinde Durum'a tıklanınca pencerede açılır, dosyanın durum
// geçmişini listeler (eski DosyaTarihce.aspx). Adres: ?yil=&dosyaNo=. Uç: /api/alacaktakip/takip/tarihce.
$(function () {
    'use strict';

    if (!$('#grdTarihce').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var p = new URLSearchParams(window.location.search);
    var yil = parseInt(p.get('yil'), 10) || 0;
    var dosyaNo = parseInt(p.get('dosyaNo'), 10) || 0;

    var grid = Sgb.grid('#grdTarihce', {
        columnDefs: [
            { field: 'siraNo', headerName: 'Sıra', width: 80, type: 'rightAligned' },
            { field: 'durum', headerName: 'Durum', width: 200, tooltipField: 'durum' },
            { field: 'tarih', headerName: 'Tarih', width: 120, valueFormatter: function (e) { return Sgb.tarih(e.value); } },
            { field: 'kullaniciKodu', headerName: 'İşlem Yapan', width: 150 },
            { field: 'aciklama', headerName: 'Açıklama', flex: 1, minWidth: 220, tooltipField: 'aciklama' }
        ],
        overlayNoRowsTemplate: '<span>Kayıt bulunamadı.</span>'
    });

    if (!yil || !dosyaNo) { Sgb.bildirim('Dosya bilgisi eksik.', 'warning'); return; }

    grid.setGridOption('loading', true);
    Sgb.al('api/alacaktakip/takip/tarihce', { yil: yil, dosyaNo: dosyaNo })
        .done(function (v) {
            if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
            grid.setGridOption('rowData', v.satirlar || []);
        })
        .fail(function (xhr) {
            if (xhr && xhr.status === 401) return;
            Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Tarihçe okunamadı.', 'danger');
        })
        .always(function () { grid.setGridOption('loading', false); });
});
