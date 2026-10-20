// Personel İşlemleri > Ceza/Ödül (InsanKaynaklari/PersonelCezaOdul; eski PersonelCezaOdul.aspx).
// Solda ceza/ödül formu, sağda personelin ceza/ödül kayıtları. Listeden seçilen kayıt sunucudan yeniden okunup forma gelir
// (eski SatirSecildi); Kaydet / Sil onay sorar, işlem sonrası form temizlenir; Yeni Kayıt formu boşaltır.
$(function () {
    'use strict';

    var $sayfa = $('#pbCezaOdul');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var KOK = 'api/personelbilgi/cezaodul/';
    var personelKod = parseInt($sayfa.data('personel-kod'), 10) || 0;
    var kod = 0; // eski hdnKod
    var mesaj = PbEgitim.mesaj, hataGoster = PbEgitim.hataGoster;

    function mesgul(evet) { $('#btnKaydet, #btnSil, #btnTemizle').prop('disabled', evet); }

    var grid = PbEgitim.listeGrid('#grdListe', [
        { field: 'cezaOdulTipiAdi', headerName: 'Ceza/Ödül Tipi', flex: 1, minWidth: 150, tooltipField: 'cezaOdulTipiAdi' },
        { field: 'tarih', headerName: 'Tarih', width: 105, valueFormatter: PbEgitim.tarihHucre }
    ], {
        pagination: true,
        paginationPageSize: 25,
        paginationPageSizeSelector: false,
        onRowClicked: function (e) { sec(e.data); }
    });

    // Eski CezaOdulTipiDoldur
    function secenekleriOku() {
        return Sgb.al(KOK + 'secenekler')
            .done(function (v) {
                PbEgitim.secenekDoldur('#cmbTip', v.cezaOdulTipi);
                if (v.hata) mesaj(v.hata, 'danger');
            })
            .fail(hataGoster);
    }

    // Eski Listele; ilk: açılışta "Yükleniyor..." gösterilmez.
    function listele(ilk) {
        if (personelKod === 0) return $.Deferred().resolve().promise();
        if (!ilk) grid.setGridOption('loading', true);
        return Sgb.al(KOK + 'liste', { personelKod: personelKod })
            .done(function (v) {
                grid.setGridOption('rowData', v.satirlar || []);
                if (v.hata) mesaj(v.hata, 'danger');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    // Eski Temizle
    function temizle() {
        kod = 0;
        $('#frmCezaOdul')[0].reset();
        $('#frmCezaOdul select').prop('selectedIndex', 0);
        grid.deselectAll();
    }

    // Eski SatirSecildi: seçilen kayıt kodu ile kayıt yeniden okunur.
    function sec(satir) {
        kod = parseInt(satir.kod, 10) || 0;
        if (kod === 0) return;
        Sgb.al(KOK + 'oku', { personelKod: personelKod, kod: kod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                if (v.kayit) doldur(v.kayit);
            })
            .fail(hataGoster);
    }

    function doldur(p) {
        $('#txtAlanUnvan').val(p.alanUnvani || '');
        $('#txtBelgeCinsi').val(p.belgeCinsi || '');
        $('#txtBelgeNo').val(p.belgeNo || '');
        $('#txtBelgeTarihi').val(p.belgeTarihi || '');
        $('#txtCinsi').val(p.cinsi || '');
        $('#txtDayanak').val(p.dayanak || '');
        $('#txtSebebi').val(p.sebebi || '');
        $('#txtTarih').val(p.tarih || '');
        $('#txtVerenAd').val(p.verenAdi || '');
        $('#txtVerenKurum').val(p.verenKurum || '');
        $('#txtVerenUnvan').val(p.verenUnvani || '');
        PbEgitim.secimYap('#cmbTip', p.cezaOdulTipi);
    }

    function formOku() {
        return {
            personelKod: personelKod,
            kod: String(kod || ''),
            cezaOdulTipi: $('#cmbTip').val() || '0',
            tarih: $('#txtTarih').val(),
            verenKurum: $('#txtVerenKurum').val(),
            verenAdi: $('#txtVerenAd').val(),
            verenUnvani: $('#txtVerenUnvan').val(),
            alanUnvani: $('#txtAlanUnvan').val(),
            cinsi: $('#txtCinsi').val(),
            sebebi: $('#txtSebebi').val(),
            dayanak: $('#txtDayanak').val(),
            belgeNo: $('#txtBelgeNo').val(),
            belgeTarihi: $('#txtBelgeTarihi').val(),
            belgeCinsi: $('#txtBelgeCinsi').val()
        };
    }

    // Eski btnKaydet_Click: başarıda form temizlenir, liste yenilenir.
    $('#btnKaydet').on('click', function () {
        PbEgitim.onay('Kayıt işlemi yapılacak. Onaylıyor musunuz?').then(function () {
            mesgul(true);
            Sgb.gonder(KOK + 'kaydet', formOku())
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    temizle();
                    listele(false);
                    mesaj(s.bilgi, 'success');
                })
                .fail(hataGoster)
                .always(function () { mesgul(false); });
        }, function () { });
    });

    // Eski btnSil_Click
    $('#btnSil').on('click', function () {
        if (kod === 0) { mesaj('Lütfen silmek istediğiniz Birey bilgisini seçin.', 'warning'); return; }
        PbEgitim.onay('Kayıt silinecek. Onaylıyor musunuz?').then(function () {
            mesgul(true);
            Sgb.gonder(KOK + 'sil', { personelKod: personelKod, kod: String(kod) })
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    temizle();
                    listele(false);
                    mesaj(s.bilgi, 'success');
                })
                .fail(hataGoster)
                .always(function () { mesgul(false); });
        }, function () { });
    });

    $('#btnTemizle').on('click', function () { temizle(); $('#cmbTip').trigger('focus'); });

    secenekleriOku().always(function () { temizle(); });
    listele(true);
});
