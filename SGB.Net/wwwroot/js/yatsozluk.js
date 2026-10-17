// Proje Türü (ButceHazirlikv2/ProjeTurTanimla) ve Döküman Türü (ButceHazirlikv2/DokumanTurTanimla) sayfaları: solda türler (AG Grid),
// sağda seçilenin kodu ve adı (eski çerçevedeki ...Detay sayfası). Sayfanın türü .yat-sozluk öğesinin data-tur özniteliğindedir.
// Kod elle yazılır; listede olan bir kodla kaydetmek o kaydı günceller. Açılışta listenin ilk kaydı seçilir (eski setTimeout(furl)).
$(function () {
    'use strict';

    var $kok = $('.yat-sozluk');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var tur = $kok.data('tur');
    var tekil = $kok.data('tekil');
    var adres = 'api/yatsozluk/' + tur + '/';
    var tumu = [];
    var seciliKod = 0;

    function mesaj(metin, t) { Sgb.bildirim(metin, t); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) {
        $('#btnKaydet, #btnYeni, #btnListele').prop('disabled', evet);
        $('#btnSil').prop('disabled', evet || !seciliKod);
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'kod', headerName: 'Kod', width: 90, sort: 'asc', cellClass: 'sayi', headerClass: 'sayi' },
            { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 200, tooltipField: 'ad' }
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        getRowId: function (p) { return String(p.data.kod); },
        overlayNoRowsTemplate: '<span>Tanımlı kayıt yok.</span>',
        onRowClicked: function (e) { doldur(e.data); }
    });

    function suz() {
        var ara = $.trim($('#txtAra').val()).toLocaleLowerCase('tr');
        grid.setGridOption('rowData', !ara ? tumu : $.grep(tumu, function (o) {
            return String(o.kod).indexOf(ara) >= 0 || (o.ad || '').toLocaleLowerCase('tr').indexOf(ara) >= 0;
        }));
    }

    function sec(kod) {
        var n = kod ? grid.getRowNode(String(kod)) : null;
        if (!n) return false;
        n.setSelected(true, true);
        grid.ensureNodeVisible(n);
        doldur(n.data);
        return true;
    }

    function listele(secilecekKod) {
        grid.setGridOption('loading', true);
        return Sgb.al(adres + 'liste')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                tumu = v.satirlar || [];
                suz();
                if (!sec(secilecekKod)) {
                    var ilk = grid.getDisplayedRowAtIndex(0);
                    if (ilk && secilecekKod === undefined) sec(ilk.data.kod);
                }
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    function doldur(o) {
        seciliKod = o.kod;
        $('#txtKod').val(o.kod);
        $('#txtAd').val(o.ad || '');
        $('#btnSil').prop('disabled', false);
    }

    function temizle() {
        seciliKod = 0;
        $('#txtKod, #txtAd').val('');
        $('#btnSil').prop('disabled', true);
        grid.deselectAll();
    }

    $('#frmArama').on('submit', function (e) { e.preventDefault(); listele(seciliKod || null); });
    $('#txtAra').on('input', suz);

    $('#btnYeni').on('click', function () {
        temizle();
        $('#txtKod').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        var istek = { kod: parseInt($('#txtKod').val(), 10) || 0, ad: $.trim($('#txtAd').val()) };
        if (istek.kod <= 0) { mesaj('Kodu yazınız.', 'warning'); $('#txtKod').trigger('focus'); return; }
        if (!istek.ad) { mesaj('Adı yazınız.', 'warning'); $('#txtAd').trigger('focus'); return; }
        mesgul(true);
        Sgb.gonder(adres + 'kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                listele(istek.kod);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnSil').on('click', function () {
        if (!seciliKod) return;
        if (!window.confirm('Seçili ' + tekil + ' silinsin mi?')) return;
        mesgul(true);
        Sgb.gonder(adres + 'sil', { kod: seciliKod, ad: $.trim($('#txtAd').val()) })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                temizle();
                listele(null);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // Önceki / Sonraki (eski Onceki(fpL) / Sonraki(fpL)): listede görünen sıraya göre.
    function komsu(yon) {
        var adet = grid.getDisplayedRowCount();
        if (!adet) return;
        var secili = grid.getSelectedNodes()[0];
        var i = secili && secili.displayed ? secili.rowIndex + yon : 0;
        if (i < 0 || i >= adet) return;
        sec(grid.getDisplayedRowAtIndex(i).data.kod);
    }

    $('#btnOnceki').on('click', function () { komsu(-1); });
    $('#btnSonraki').on('click', function () { komsu(1); });

    listele();
});
