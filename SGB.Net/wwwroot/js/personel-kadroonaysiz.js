// Bekleyen İşlemler penceresi (eski InsanKaynaklari/PersonelKadroOnaysiz.aspx): Onaysızlar ve Tayin Bekleyenler, Birime Al.
// Personel İşlemleri ana sayfasındaki "Bekleyen İşl." düğmesi açar: PersonelKadroOnaysiz?menuYok=1&orgKod=<birim>.
(function ($) {
    'use strict';

    var $kok = $('#pbOnaysiz');
    if (!$kok.length) return;
    var ORG = parseInt($kok.data('org-kod'), 10) || 0;
    var UC = 'api/personelbilgi/kadroonaysiz/';
    var grdOnaysiz, grdTayin;

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function sutunlar(tayin) {
        var s = [
            { colId: 'no', headerName: '#', width: 56, sortable: false, valueGetter: function (p) { return p.node.rowIndex + 1; }, cellClass: 'hucre-salt' },
            { field: 'kadroDurumAd', headerName: 'Kayıt Durum', width: 130, cellClass: 'hucre-salt' },
            { field: 'unvanAd', headerName: 'Unvanı', width: 200, cellClass: 'hucre-salt', tooltipField: 'unvanAd' },
            { field: 'personelTCKimlikNo', headerName: 'Kimlik No', width: 120, cellClass: 'hucre-salt' },
            { field: 'personelAdSoyad', headerName: 'Ad Soyad', width: 200, cellClass: 'hucre-salt' },
            { field: 'birimAd', headerName: tayin ? 'Gönderen Birim' : 'Birimi', width: 300, flex: tayin ? 0 : 1, cellClass: 'hucre-salt', tooltipField: 'birimAd' }
        ];
        if (tayin) s.push({ field: 'tayinOlacagiBirimAd', headerName: 'Alacak Birim', width: 300, flex: 1, cellClass: 'hucre-salt', tooltipField: 'tayinOlacagiBirimAd' });
        return s;
    }

    function gridKur() {
        grdOnaysiz = Sgb.grid('#grdOnaysizlar', {
            columnDefs: sutunlar(false),
            rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
            tooltipShowDelay: 400
        });
        grdTayin = Sgb.grid('#grdTayinBekleyenler', {
            columnDefs: sutunlar(true),
            rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
            tooltipShowDelay: 400
        });
    }

    // Eski Listele.
    function listele() {
        grdOnaysiz.setGridOption('loading', true);
        grdTayin.setGridOption('loading', true);
        Sgb.al(UC + 'liste', { orgKod: ORG }).done(function (v) {
            if (v.hata) Sgb.bildirim(v.hata, 'danger');
            grdOnaysiz.setGridOption('rowData', v.onaysizlar || []);
            grdTayin.setGridOption('rowData', v.tayinBekleyenler || []);
            $('#sayOnaysiz').text((v.onaysizlar || []).length);
            $('#sayTayin').text((v.tayinBekleyenler || []).length);
        }).fail(hataGoster).always(function () {
            grdOnaysiz.setGridOption('loading', false);
            grdTayin.setGridOption('loading', false);
        });
    }

    // Sekmeler (eski TabPanel).
    $('.pbk-sekme').on('click', function () {
        var $s = $(this);
        $('.pbk-sekme').attr('aria-selected', 'false');
        $s.attr('aria-selected', 'true');
        $('.pbk-panel').prop('hidden', true);
        $('#' + $s.attr('aria-controls')).prop('hidden', false);
    });

    // Eski btnBirimeAl_Click.
    var dlg = document.getElementById('dlgOnay');
    $(dlg).on('click', '[data-kapat]', function () { dlg.close(); });
    $('#btnBirimeAl').on('click', function () {
        if (!grdTayin.getSelectedRows().length) { Sgb.bildirim('Lütfen Satır Seçiniz!', 'warning'); return; }
        dlg.showModal();
    });
    $('#btnOnayEvet').on('click', function () {
        dlg.close();
        var s = grdTayin.getSelectedRows()[0];
        if (!s) return;
        var $d = $('#btnBirimeAl').prop('disabled', true);
        Sgb.gonder(UC + 'birimeal', {
            kod: s.kod, personelKod: s.personelKod, personelTCKimlikNo: s.personelTCKimlikNo || '', tarih: $('#dfAlisTarih').val() || ''
        }).done(function (v) {
            if (v.basarili) { Sgb.bildirim(v.bilgi, 'success'); listele(); }
            else {
                Sgb.bildirim(v.hata, 'danger');
                if (v.listele) listele();
            }
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    });

    gridKur();
    listele();
})(jQuery);
