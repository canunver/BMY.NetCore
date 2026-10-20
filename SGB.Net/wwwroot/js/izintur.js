// İzin Türleri (InsanKaynaklari/TanimIzinTur): solda izin türleri (AG Grid), sağda seçilenin bilgileri.
// Eski sayfadaki gibi: listeden seçilen kayıt forma gelir; Kaydet ve Sil onay sorar, sonra liste yeniden okunur;
// Bul formdaki değerlere (ad başı, süre, görülebilirlik, sıra) uyanları listeler; Yeni Kayıt formu boşaltıp listeyi yeniden okur.
$(function () {
    'use strict';

    if (!$('#grdIzinTur').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var tumu = [];

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    var kaydetKapali = false;
    function mesgul(evet) {
        $('#btnSil, #btnBul, #btnTemizle, #btnListele').prop('disabled', evet);
        $('#btnKaydet').prop('disabled', evet || kaydetKapali);
    }
    function sayi(sec) { return parseInt($(sec).val(), 10) || 0; }

    var grid = Sgb.grid('#grdIzinTur', {
        columnDefs: [
            { field: 'sira', headerName: 'Sıra No', width: 80, cellClass: 'text-center' },
            { field: 'ad', headerName: 'İzin Tür', flex: 1, minWidth: 180, tooltipField: 'ad' },
            { field: 'gorulebilirlikAd', headerName: 'Görülebilirlik', width: 130 },
            { field: 'sure', headerName: 'Süre', width: 80, cellClass: 'text-right' }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod) + ':' + p.data.sira; },
        onRowClicked: function (e) { doldur(e.data); },
        overlayNoRowsTemplate: '<span class="text-muted">İzin türü yok</span>'
    });

    function suz() {
        var ara = $.trim($('#txtAra').val()).toLocaleLowerCase('tr');
        grid.setGridOption('rowData', !ara ? tumu : $.grep(tumu, function (o) {
            return (String(o.sira) + ' ' + (o.ad || '') + ' ' + (o.gorulebilirlikAd || '')).toLocaleLowerCase('tr').indexOf(ara) >= 0;
        }));
    }

    function secenekleriDoldur(liste) {
        var $c = $('#cmbGorulebilirlik');
        if ($c.find('option').length || !liste) return;
        $('<option></option>').val('').text('').appendTo($c);
        $.each(liste, function (i, g) { $('<option></option>').val(g.kod).text(g.ad).appendTo($c); });
    }

    function sonucIsle(veri) {
        secenekleriDoldur(veri.gorulebilirlikler);
        if (veri.kaydetKapali) { kaydetKapali = true; $('#btnKaydet').prop('disabled', true); }
        if (veri.hata) { mesaj(veri.hata, 'danger'); }
        tumu = veri.satirlar || [];
        suz();
    }

    // ilk: sayfa açılışındaki okuma; eski sayfadaki gibi liste açılışta gelir ama "Yükleniyor..." gösterilmez.
    function listele(ilk) {
        if (!ilk) grid.setGridOption('loading', true);
        return Sgb.al('api/izintur/liste')
            .done(sonucIsle)
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    function formOku() {
        return {
            kod: parseInt($('#frmIzinTur').data('kod'), 10) || 0,
            ad: $('#txtAd').val(),
            sure: sayi('#txtSure'),
            gorulebilirlik: sayi('#cmbGorulebilirlik'),
            sira: sayi('#txtSira')
        };
    }

    // Eski Temizle
    function temizle() {
        $('#frmIzinTur')[0].reset();
        $('#frmIzinTur').data('kod', 0);
        $('#cmbGorulebilirlik').val('');
        grid.deselectAll();
    }

    // Eski SatirSecildi
    function doldur(o) {
        $('#frmIzinTur').data('kod', o.kod);
        $('#txtAd').val(o.ad || '');
        $('#txtSure').val(o.sure);
        $('#cmbGorulebilirlik').val(String(o.gorulebilirlik));
        if ($('#cmbGorulebilirlik').val() === null) $('#cmbGorulebilirlik').val('');
        $('#txtSira').val(o.sira);
    }

    $('#frmArama').on('submit', function (e) {
        e.preventDefault();
        listele(false);
    });
    $('#txtAra').on('input', suz);

    // Eski btnTemizle_Click: liste yeniden okunur, form boşalır.
    $('#btnTemizle').on('click', function () {
        temizle();
        listele(false);
        $('#txtAd').trigger('focus');
    });

    // Eski btnAra_Click: formdaki değerler ölçüt olur.
    $('#btnBul').on('click', function () {
        var k = formOku();
        mesgul(true);
        grid.setGridOption('loading', true);
        Sgb.gonder('api/izintur/bul', k)
            .done(sonucIsle)
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); mesgul(false); });
    });

    $('#btnKaydet').on('click', function () {
        var istek = formOku();
        if ($.trim(istek.ad) === '') { mesaj('İzin Türü Adı boş bırakılamaz.', 'warning'); $('#txtAd').trigger('focus'); return; }
        if (!window.confirm('Kayıt işlemi yapılacak. Onaylıyor musunuz?')) return;
        mesgul(true);
        Sgb.gonder('api/izintur/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                temizle();
                listele(false);
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnSil').on('click', function () {
        var kod = parseInt($('#frmIzinTur').data('kod'), 10) || 0;
        if (kod <= 0) { mesaj('Lütfen silmek istediğiniz izin türünü seçin.', 'warning'); return; }
        if (!window.confirm('Kayıt silinecek. Onaylıyor musunuz?')) return;
        mesgul(true);
        Sgb.gonder('api/izintur/sil', { kod: kod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                temizle();
                listele(false);
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    listele(true);
});
