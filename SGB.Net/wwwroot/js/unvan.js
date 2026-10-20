// Unvan Bilgileri (InsanKaynaklari/TanimUnvan): solda unvan listesi (AG Grid), sağda seçilenin bilgileri.
// Eski sayfadaki gibi: liste açılışta gelir, listeden seçilen unvan forma gelir, Kaydet / Sil onay sorar,
// Bul formdaki Unvan Kodu, Unvan Adı, Sıra No'ya göre listeler, Temizle formu boşaltıp tüm listeyi okur.
$(function () {
    'use strict';

    if (!$('#grdUnvan').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnSil, #btnBul, #btnTemizle').prop('disabled', evet); }

    var grid = Sgb.grid('#grdUnvan', {
        columnDefs: [
            { field: 'kod', headerName: 'Unvan Kodu', width: 105, type: 'numericColumn' },
            { field: 'ad', headerName: 'Unvan Adı', flex: 1, minWidth: 180, tooltipField: 'ad' },
            { field: 'sinifAdKisa', headerName: 'Sınıf Kısa Ad', width: 115 },
            { field: 'siraNo', headerName: 'Sıra No', width: 85, type: 'numericColumn' },
            { field: 'sinifAd', headerName: 'Sınıf', width: 170, tooltipField: 'sinifAd' },
            { field: 'yoneticiAd', headerName: 'Yönetici', width: 85 },
            { field: 'aktifAd', headerName: 'Aktif', width: 70 },
            { field: 'hitapKod', headerName: 'Hitap', width: 75, type: 'numericColumn' }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { doldur(e.data); },
        overlayNoRowsTemplate: '<span class="text-muted">Unvan yok</span>'
    });

    // Eski SınıfDoldur
    function siniflariOku() {
        return Sgb.al('api/unvan/siniflar')
            .done(function (veri) {
                var $c = $('#cmbSinif').empty();
                $.each(veri.siniflar || [], function (i, s) { $('<option></option>').val(s.KOD).text(s.AD).appendTo($c); });
                if (veri.hata) mesaj(veri.hata, 'danger');
            })
            .fail(hataGoster);
    }

    // kriter verilmezse tüm liste (eski Listele(new Unvan())); ilk: açılışta "Yükleniyor..." gösterilmez.
    function listele(kriter, ilk) {
        if (!ilk) grid.setGridOption('loading', true);
        return Sgb.al('api/unvan/liste', kriter || {})
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                grid.setGridOption('rowData', veri.satirlar || []);
                if (veri.bilgi) mesaj(veri.bilgi, 'info');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    function temizle() {
        $('#frmUnvan')[0].reset();
        $('#cmbSinif').val('0');
        grid.deselectAll();
    }

    // Eski SatirSecildi
    function doldur(o) {
        $('#txtKod').val(o.kod);
        $('#txtAd').val(o.ad || '');
        $('#txtSinifAdKisa').val(o.sinifAdKisa || '');
        $('#txtSiraNo').val(o.siraNo);
        var $c = $('#cmbSinif');
        $c.val(String(o.sinifKod || 0));
        if ($c.val() === null) $c.val('0');
        $('#chkYonetici').prop('checked', (parseInt(o.yonetici, 10) || 0) > 0);
        $('#chkAktif').prop('checked', o.aktif === true);
        $('#txtHitapKod').val(o.hitapKod);
    }

    function formOku() {
        return {
            kod: $.trim($('#txtKod').val()),
            ad: $('#txtAd').val(),
            sinifAdKisa: $('#txtSinifAdKisa').val(),
            sinifKod: parseInt($('#cmbSinif').val(), 10) || 0,
            siraNo: $.trim($('#txtSiraNo').val()),
            yonetici: $('#chkYonetici').prop('checked'),
            aktif: $('#chkAktif').prop('checked'),
            hitapKod: $.trim($('#txtHitapKod').val())
        };
    }

    $('#btnBul').on('click', function () {
        var f = formOku();
        listele({ kod: f.kod, ad: $.trim(f.ad), sinifAdKisa: $.trim(f.sinifAdKisa), siraNo: f.siraNo }, false);
    });

    $('#btnTemizle').on('click', function () {
        temizle();
        listele(null, false);
        $('#txtKod').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        var istek = formOku();
        var eksik = [];
        if ((parseInt(istek.siraNo, 10) || 0) === 0) eksik.push('Sıra No boş bırakılamaz');
        if (istek.kod === '') eksik.push('Unvan Kod bilgisi boş bırakılamaz');
        if ($.trim(istek.ad) === '') eksik.push('Unvan bilgisi boş bırakılamaz');
        if (eksik.length) { mesaj(eksik.join('\n'), 'warning'); return; }
        if (!window.confirm('Kaydetmek istiyor musunuz?')) return;
        mesgul(true);
        Sgb.gonder('api/unvan/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                temizle();
                listele(null, false);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnSil').on('click', function () {
        var kod = $.trim($('#txtKod').val());
        if ((parseInt(kod, 10) || 0) <= 0) { mesaj('Lütfen silmek istediğiniz unvanı seçin.', 'warning'); return; }
        if (!window.confirm(kod + ' kodlu unvanı silmek istiyor musunuz?')) return;
        mesgul(true);
        Sgb.gonder('api/unvan/sil', { kod: kod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                temizle();
                listele(null, false);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    siniflariOku().always(function () { temizle(); });
    listele(null, true);
});
