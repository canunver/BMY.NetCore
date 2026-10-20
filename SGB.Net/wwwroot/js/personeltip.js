// Personel Tipleri (InsanKaynaklari/PersonelTipForm): solda personel tipleri (AG Grid), sağda seçilenin bilgileri.
// Eski sayfadaki gibi: listeden seçilen tip okunur, Oku yazılan kodu okur, Kaydet onay sorup kaydeder.
$(function () {
    'use strict';

    if (!$('#grdPersonelTip').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var tumu = [];
    var SECIMLER = { chkKadrolu: 'kadrolu', chkKurumDisi: 'kurumDisi', chkGoster: 'goster', chkAktif: 'aktif', chkTerfiyeDahil: 'terfiyeDahil', chkMaasaDahil: 'maasaDahil', chkHitapMi: 'hitapMi', chkPesinMi: 'pesinMi' };
    var SAYILAR = { txtKod: 'kod', txtSira: 'sira', txtMaasDonemGun: 'maasDonemGun', txtMaasOdemeGun: 'maasOdemeGun', txtH1: 'H1', txtH2: 'H2', txtH3: 'H3', txtH4: 'H4', txtH5: 'H5', txtH6: 'H6', txtH7: 'H7' };
    var METINLER = { txtAd: 'ad', txtGrupAd: 'grupAd', txtMaasGrupAd: 'maasGrupAd' };

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnYeni, #btnOku, #btnKaydet, #btnListele').prop('disabled', evet); }
    function evet(p) { return p.value === 1 ? 'Evet' : ''; }

    var grid = Sgb.grid('#grdPersonelTip', {
        columnDefs: [
            { field: 'grupAd', headerName: 'Grup', width: 120, tooltipField: 'grupAd' },
            { field: 'kod', headerName: 'Kod', width: 70 },
            { field: 'sira', headerName: 'Sıra', width: 70 },
            { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 180, tooltipField: 'ad' },
            { field: 'kurumDisi', headerName: 'Kurum Dışı', width: 95, valueFormatter: evet },
            { field: 'kadrolu', headerName: 'Kadrolu', width: 85, valueFormatter: evet },
            { field: 'goster', headerName: 'Göster', width: 80, valueFormatter: evet },
            { field: 'aktif', headerName: 'Aktif', width: 75, valueFormatter: evet }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { doldur(e.data); },
        overlayNoRowsTemplate: '<span class="text-muted">Personel tipi yok</span>'
    });

    function suz() {
        var ara = $.trim($('#txtAra').val()).toLocaleLowerCase('tr');
        grid.setGridOption('rowData', !ara ? tumu : $.grep(tumu, function (o) {
            return ((o.grupAd || '') + ' ' + o.kod + ' ' + (o.ad || '')).toLocaleLowerCase('tr').indexOf(ara) >= 0;
        }));
    }

    function sec(kod) {
        var n = kod ? grid.getRowNode(String(kod)) : null;
        if (n) { n.setSelected(true); grid.ensureNodeVisible(n); } else grid.deselectAll();
    }

    // ilk: sayfa açılışındaki okuma; eski sayfadaki gibi liste açılışta gelir ama "Yükleniyor..." gösterilmez.
    function listele(secilecekKod, ilk) {
        if (!ilk) grid.setGridOption('loading', true);
        return Sgb.al('api/personeltip/liste')
            .done(function (veri) {
                if (veri.hizmetGruplari && !$('#cmbHitap option').length) {
                    var $c = $('#cmbHitap');
                    $.each(veri.hizmetGruplari, function (i, g) { $('<option></option>').val(g.kod).text(g.ad).appendTo($c); });
                    $c.val('');
                }
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                tumu = veri.satirlar || [];
                suz();
                if (secilecekKod) sec(secilecekKod);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    $('#frmArama').on('submit', function (e) {
        e.preventDefault();
        listele(parseInt($('#txtKod').val(), 10) || 0);
    });
    $('#txtAra').on('input', suz);

    function temizle() {
        $('#frmPersonelTip')[0].reset();
        $('#cmbHitap').val('');
        grid.deselectAll();
    }

    function doldur(o) {
        $.each(SAYILAR, function (id, alan) { $('#' + id).val(o[alan]); });
        $.each(METINLER, function (id, alan) { $('#' + id).val(o[alan] || ''); });
        $.each(SECIMLER, function (id, alan) { $('#' + id).prop('checked', o[alan] === 1); });
        $('#cmbHitap').val(String(o.HITAP));
        if ($('#cmbHitap').val() === null) $('#cmbHitap').val('');
    }

    function topla() {
        var o = {};
        $.each(SAYILAR, function (id, alan) { o[alan] = parseInt($('#' + id).val(), 10) || 0; });
        $.each(METINLER, function (id, alan) { o[alan] = $('#' + id).val(); });
        $.each(SECIMLER, function (id, alan) { o[alan] = $('#' + id).prop('checked') ? 1 : 0; });
        o.HITAP = parseInt($('#cmbHitap').val(), 10) || 0;
        return o;
    }

    $('#btnYeni').on('click', function () {
        temizle();
        $('#txtKod').trigger('focus');
    });

    $('#btnOku').on('click', function () {
        var kod = parseInt($('#txtKod').val(), 10) || 0;
        if (kod <= 0) { mesaj('Okunacak personel tipinin kodunu yazınız.', 'warning'); $('#txtKod').trigger('focus'); return; }
        mesgul(true);
        Sgb.al('api/personeltip/oku', { kod: kod })
            .done(function (s) {
                if (s.hata) { mesaj(s.hata, 'danger'); return; }
                if (!s.kayit) { mesaj(s.bilgi, 'info'); return; }
                doldur(s.kayit);
                sec(s.kayit.kod);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnKaydet').on('click', function () {
        var istek = topla();
        if (istek.kod <= 0) { mesaj('Personel tipinin kodunu yazınız.', 'warning'); $('#txtKod').trigger('focus'); return; }
        if (!window.confirm('Kaydetmek istiyor musunuz?')) return;
        mesgul(true);
        Sgb.gonder('api/personeltip/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                listele(s.kod);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    listele(0, true);
});
