// Durum Tanımlama (AlacakTakip/TanimDurum): Alacak Takip dosya durumları. Açılışta ve Bul ile listelenir; tıklanan durum forma
// gelir (eski VeriGoster). Kaydet eski sayfadaki gibi yalnızca alanları denetler: sunucuda durum kaydeden işlem yok.
$(function () {
    'use strict';

    if (!$('#grdDurum').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    var grid = Sgb.grid('#grdDurum', {
        columnDefs: [
            { field: 'ad', headerName: 'Durum Tanımı', flex: 1, minWidth: 220, tooltipField: 'ad' }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { veriGoster(e.data); }
    });

    function veriGoster(o) {
        $('#hdnKod').val(o ? o.kod : '');
        $('#txtAd').val(o ? o.ad : '');
        if (!o) grid.deselectAll();
    }

    function ara() {
        $('#btnAra').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/alacaktakip/durum/liste')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); grid.setGridOption('rowData', []); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                if (v.bilgi) mesaj(v.bilgi, 'info');
            })
            .fail(hataGoster)
            .always(function () { $('#btnAra').prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmDurum').on('submit', function (e) { e.preventDefault(); ara(); });

    $('#btnTemizle').on('click', function () { veriGoster(null); $('#txtAd').trigger('focus'); });

    // eski btnKaydet_Click: yalnızca denetim
    $('#btnKaydet').on('click', function () {
        var hata = [];
        if (!$.trim($('#hdnKod').val())) hata.push('Durum Kodu boş bırakılmış.');
        if (!$.trim($('#txtAd').val())) hata.push('Durum Tanımı boş bırakılmış.');
        if (hata.length) { mesaj(hata.join(' ') + ' Kayıt işlemi yapılamadı.', 'warning'); return; }
        mesaj('Durum tanımları değiştirilemez; Kaydet yalnızca alanları denetler.', 'info');
    });

    ara(); // eski sayfa açılışta listeliyordu
});
