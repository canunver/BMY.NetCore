// Hizmet Sebepleri (InsanKaynaklari/HizmetSebebiForm): solda arama + liste (AG Grid), sağda seçilen sebebin bilgileri.
// Eski sayfadaki gibi: Listele kriterlere göre okur, satıra tıklanınca form dolar, Kaydet onay sorup kaydeder,
// Sil işaretli satırları siler. Liste açılışta boş gelir (eski sayfada da Listele'ye basılınca dolardı).
$(function () {
    'use strict';

    if (!$('#grdSebepler').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnListele, #btnSil, #btnKaydet, #btnTemizle').prop('disabled', evet); }

    var grid = Sgb.grid('#grdSebepler', {
        columnDefs: [
            { field: 'kod', headerName: '#', width: 80 },
            { field: 'tipAd', headerName: 'Tip', width: 170, tooltipField: 'tipAd' },
            { field: 'sebepAdi', headerName: 'Sebep Adı', flex: 1, minWidth: 160, tooltipField: 'sebepAdi' }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { doldur(e.data); },
        overlayNoRowsTemplate: '<span class="text-muted">Kriterleri seçip Listele tuşuna basınız.</span>'
    });

    // Tip seçenekleri (eski stoTip): aramada "-" tümü demektir.
    Sgb.al('api/hizmetsebebi/tipler')
        .done(function (veri) {
            var $ara = $('#cmbAraTip').empty(), $tip = $('#cmbTip').empty();
            $.each(veri.tipler || [], function (i, t) {
                $('<option></option>').val(t.kod).text(t.kod === 0 ? '(Tümü)' : t.ad).appendTo($ara);
                $('<option></option>').val(t.kod).text(t.ad).appendTo($tip);
            });
            $ara.val('0');
            $tip.val('0');
        })
        .fail(hataGoster);

    var sonKriter = null;

    function listele(kriter, secilecekKod) {
        sonKriter = kriter;
        grid.setGridOption('loading', true);
        return Sgb.al('api/hizmetsebebi/liste', kriter)
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                grid.setGridOption('rowData', veri.satirlar || []);
                if (!(veri.satirlar || []).length) grid.setGridOption('overlayNoRowsTemplate', '<span class="text-muted">Verilen kriterlere uygun kayıt bulunamadı.</span>');
                var n = secilecekKod ? grid.getRowNode(String(secilecekKod)) : null;
                if (n) grid.ensureNodeVisible(n);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    function kriterTopla() {
        var k = {};
        var ad = $.trim($('#txtAraAd').val());
        var tip = parseInt($('#cmbAraTip').val(), 10) || 0;
        if (ad) k.sebepAdi = ad;
        if (tip) k.tip = tip;
        return k;
    }

    $('#frmArama').on('submit', function (e) {
        e.preventDefault();
        listele(kriterTopla());
    });

    function doldur(o) {
        $('#txtKod').val(o.kod);
        $('#txtSebepAdi').val(o.sebepAdi || '');
        $('#cmbTip').val(String(o.tip));
        if ($('#cmbTip').val() === null) $('#cmbTip').val('0');
    }

    $('#btnTemizle').on('click', function () {
        $('#frmSebep')[0].reset();
        $('#cmbTip').val('0');
        $('#txtKod').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        var istek = {
            kod: parseInt($('#txtKod').val(), 10) || 0,
            sebepAdi: $.trim($('#txtSebepAdi').val()),
            tip: parseInt($('#cmbTip').val(), 10) || 0
        };
        if (!window.confirm('Kaydetmek istiyor musunuz?')) return;
        mesgul(true);
        Sgb.gonder('api/hizmetsebebi/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                if (sonKriter) listele(sonKriter, s.kod);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnSil').on('click', function () {
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Silinecek satırları işaretleyiniz.', 'warning'); return; }
        if (!window.confirm(secili.length + ' hizmet sebebi silinecek. Emin misiniz?')) return;
        mesgul(true);
        Sgb.gonder('api/hizmetsebebi/sil', { kodlar: $.map(secili, function (o) { return o.kod; }) })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                if (sonKriter) listele(sonKriter);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });
});
