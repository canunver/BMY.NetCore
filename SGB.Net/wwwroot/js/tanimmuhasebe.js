// Muhasebe Bilgileri (ButceMuhasebe/TanimMuhasebe): solda arama + liste (AG Grid), sağda seçili muhasebenin bilgileri.
// Saymanlık taşıma ayrı bir pencerededir (<dialog>).
$(function () {
    'use strict';

    if (!$('#grdListe').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var M = JSON.parse($('#metinler').text() || '{}');
    var tanimlar = JSON.parse($('#tanimlar').text() || '{}');
    // Form alanları: istek/yanıt alan adı -> giriş kutusu
    var ALANLAR = {
        ad: '#txtAd', ilgili1: '#txtIlgili1', ilgili1Unvan: '#txtIlgili1Unvan', ilgili2: '#txtIlgili2', ilgili2Unvan: '#txtIlgili2Unvan',
        adres: '#txtAdres', telefon: '#txtTelefon', faks: '#txtFaks', vergiNo: '#txtVergiNo', ePosta: '#txtEPosta'
    };

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(M.IslemHata, 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnYeni, #btnListele').prop('disabled', evet); }

    // ---------------------------------------------------------------- liste

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'kod', headerName: M.MUHGNL075, width: 96, sort: 'asc' },
            { field: 'ad', headerName: M.MUHGNL076, flex: 1, minWidth: 240, tooltipField: 'ad' }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { doldur(e.data); }
    });

    function listele(secilecekKod) {
        grid.setGridOption('loading', true);
        return Sgb.al('api/muhasebe/saymanlik/liste', { ara: $('#aramaMetin').val() })
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                grid.setGridOption('rowData', veri.satirlar);
                if (secilecekKod) {
                    var n = grid.getRowNode(String(secilecekKod));
                    if (n) { n.setSelected(true); grid.ensureNodeVisible(n); }
                }
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    $('#frmArama').on('submit', function (e) {
        e.preventDefault();
        listele();
    });

    // ---------------------------------------------------------------- form

    function temizle() {
        $('#txtKod').val('');
        $.each(ALANLAR, function (ad, kutu) { $(kutu).val(''); });
        $('#btnSil').prop('disabled', true);
        grid.deselectAll();
    }

    function doldur(m) {
        $('#txtKod').val(m.kod);
        $.each(ALANLAR, function (ad, kutu) { $(kutu).val(m[ad] || ''); });
        $('#btnSil').prop('disabled', false);
    }

    $('#btnYeni').on('click', function () {
        temizle();
        $('#txtKod').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        var istek = { kod: $.trim($('#txtKod').val()) };
        $.each(ALANLAR, function (ad, kutu) { istek[ad] = $(kutu).val(); });
        mesgul(true);
        Sgb.gonder('api/muhasebe/saymanlik/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                $('#btnSil').prop('disabled', false);
                mesaj(s.bilgi, 'success');
                listele(s.kod);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnSil').on('click', function () {
        var kod = parseInt($('#txtKod').val(), 10) || 0;
        if (kod <= 0) { mesaj(M.MUHGNL183, 'warning'); return; }
        if (!window.confirm(M.MUHGNL162)) return;
        mesgul(true);
        Sgb.gonder('api/muhasebe/saymanlik/sil', { kod: kod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                temizle();
                mesaj(s.bilgi, 'success');
                listele();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // ---------------------------------------------------------------- saymanlık taşıma

    var pencere = document.getElementById('dlgTasi');

    $.each(tanimlar.islemler || [], function (i, islem) {
        var $etiket = $('<label></label>');
        $('<input type="radio" name="tasiIslem" />').val(islem.kod).appendTo($etiket);
        $etiket.append(document.createTextNode(' ' + islem.ad)).appendTo('#tasiIslemler');
    });

    function tasiSonuc(metin, tur) {
        $('#tasiSonuc').removeClass('alert-success alert-danger').addClass('alert-' + (tur || 'danger')).text(metin || '').prop('hidden', !metin);
    }

    $('#btnTasi').on('click', function () {
        tasiSonuc('');
        $('#tasiHazirDegil').prop('hidden', !!tanimlar.tasimaHazir);
        $('#btnTasiYap').prop('disabled', !tanimlar.tasimaHazir);
        // Seçili muhasebe taşınacak saymanlık olarak önerilir.
        if (!$('#tsSaymanlik').val()) $('#tsSaymanlik').val($('#txtKod').val());
        pencere.showModal();
    });

    $(pencere).on('click', '[data-kapat]', function () { pencere.close(); });

    $('#btnTasiYap').on('click', function () {
        var istek = {
            eskiSaymanlik: parseInt($('#tsSaymanlik').val(), 10) || 0,
            eskiBirim: $.trim($('#tsBirim').val()),
            yeniSaymanlik: parseInt($('#ynSaymanlik').val(), 10) || 0,
            yeniBirim: $.trim($('#ynBirim').val()),
            islem: parseInt($('input[name=tasiIslem]:checked').val(), 10)
        };
        if (!istek.eskiSaymanlik || !istek.eskiBirim || !istek.yeniSaymanlik || !istek.yeniBirim) { tasiSonuc(M.TasiEksik, 'danger'); return; }
        if (isNaN(istek.islem)) { tasiSonuc(M.TasiIslemSec, 'danger'); return; }
        if (!window.confirm(M.TasiOnay)) return;

        tasiSonuc('');
        $('#btnTasiYap').prop('disabled', true);
        Sgb.gonder('api/muhasebe/saymanlik/tasi', istek)
            .done(function (s) { tasiSonuc(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); })
            .fail(function (xhr) { if (!xhr || xhr.status !== 401) tasiSonuc(M.IslemHata, 'danger'); })
            .always(function () { $('#btnTasiYap').prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açılış

    temizle();
    listele();
});
