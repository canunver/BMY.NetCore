// Personel İşlemleri > Yabancı Dil (InsanKaynaklari/PersonelDil; eski PersonelDil.aspx).
// Solda dil formu ve kayda ekli belgeler, sağda personelin dil kayıtları (belge tarihine göre yeniden eskiye).
// Listeden seçilen kayıt forma gelir; Kaydet / Sil onay sorar, Yeni Kayıt formu boşaltır.
$(function () {
    'use strict';

    var $sayfa = $('#pbDil');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var KOK = 'api/personelbilgi/dil/';
    var personelKod = parseInt($sayfa.data('personel-kod'), 10) || 0;
    var kod = 0; // eski hdnKod
    var mesaj = PbEgitim.mesaj, hataGoster = PbEgitim.hataGoster;

    function mesgul(evet) { $('#btnKaydet, #btnSil, #btnTemizle, #btnBelgeEkle').prop('disabled', evet); }

    var grid = PbEgitim.listeGrid('#grdListe', [
        { field: 'dilAd', headerName: 'Dil', flex: 1, minWidth: 110, tooltipField: 'dilAd' },
        { field: 'belgeTurAd', headerName: 'Belge Türü', width: 100, tooltipField: 'belgeTurAd' },
        { field: 'puan', headerName: 'Puan', width: 65, type: 'numericColumn', comparator: function (a, b, na, nb) { return (na.data.puanSayi || 0) - (nb.data.puanSayi || 0); } },
        { field: 'tarih', headerName: 'Tarih', width: 95, valueFormatter: PbEgitim.tarihHucre },
        { field: 'bordroyaEsas', headerName: 'Bord. Es.', width: 78, valueFormatter: PbEgitim.isaretHucre, cellStyle: { textAlign: 'center' }, headerTooltip: 'Bordroya esas' },
        { field: 'kurumFaydalaniyor', headerName: 'Kur. Fay.', width: 78, valueFormatter: PbEgitim.isaretHucre, cellStyle: { textAlign: 'center' }, headerTooltip: 'Kurum faydalanıyor' },
        { field: 'belgeTarihi', hide: true, sort: 'desc' } // eski SortInfo BELGETARIHI DESC
    ], {
        pagination: true,
        paginationPageSize: 25,
        paginationPageSizeSelector: false,
        onRowClicked: function (e) { doldur(e.data); }
    });

    var belgeler = PbEgitim.belgeler({
        kok: KOK, grid: '#grdBelgeler', dugme: '#btnBelgeEkle', dosya: '#fuBelge', personelKod: personelKod,
        kodAl: function () { return kod; },
        kaydetOnce: function () { return kaydetIstek(); }
    });

    // Eski DilDoldur, DilBelgeTurDoldur, Genel.AyDoldur, Genel.YilDoldur
    function secenekleriOku() {
        return Sgb.al(KOK + 'secenekler')
            .done(function (v) {
                PbEgitim.secenekDoldur('#cmbDil', v.dil);
                PbEgitim.secenekDoldur('#cmbBelgeTuru', v.belgeTur);
                PbEgitim.secenekDoldur('#cmbDonemAy', v.aylar);
                Sgb.yilKutusu('#txtDonemYil', [v.yilEnAz, v.yilEnCok]);
                if (v.hata) mesaj(v.hata, 'danger');
            })
            .fail(hataGoster);
    }

    // Eski Listele; ilk: açılışta "Yükleniyor..." gösterilmez.
    function listele(ilk) {
        if (personelKod <= 0) return $.Deferred().resolve().promise();
        if (!ilk) grid.setGridOption('loading', true);
        return Sgb.al(KOK + 'liste', { personelKod: personelKod })
            .done(function (v) {
                grid.setGridOption('rowData', v.satirlar || []);
                if (v.hata) mesaj(v.hata, 'danger');
                isaretle();
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    function isaretle() {
        grid.deselectAll();
        if (kod > 0) {
            var n = grid.getRowNode(String(kod));
            if (n) n.setSelected(true);
        }
    }

    // Eski Temizle
    function temizle() {
        kod = 0;
        $('#frmDil')[0].reset();
        $('#frmDil select').prop('selectedIndex', 0);
        grid.deselectAll();
        belgeler.temizle();
    }

    // Eski SatirSecildi
    function doldur(o) {
        kod = parseInt(o.kod, 10) || 0;
        PbEgitim.secimYap('#cmbDil', o.dilKod);
        $('#txtPuan').val(o.puan);
        $('#txtTarih').val(o.tarih || '');
        PbEgitim.secimYap('#cmbDuzey', o.duzey);
        PbEgitim.secimYap('#cmbDonemAy', o.donemAy);
        $('#txtDonemYil').val((parseInt(o.donemYil, 10) || 0) > 0 ? o.donemYil : '');
        PbEgitim.secimYap('#cmbBelgeTuru', o.belgeTur);
        $('#txtBelgeTarihi').val(o.belgeTarihi || '');
        $('#txtBelgeNo').val(o.belgeNo || '');
        $('#txtDilSeviyesi').val(o.dilSeviyesi || '');
        $('#chkBordroyaEsas').prop('checked', PbEgitim.evetMi(o.bordroyaEsas));
        $('#chkKurumFaydalaniyor').prop('checked', PbEgitim.evetMi(o.kurumFaydalaniyor));
        belgeler.yenile();
    }

    function formOku() {
        return {
            personelKod: personelKod,
            kod: String(kod || ''),
            dilKod: $('#cmbDil').val() || '0',
            puan: $('#txtPuan').val(),
            tarih: $('#txtTarih').val(),
            duzey: $('#cmbDuzey').val() || '',
            donemAy: $('#cmbDonemAy').val() || '0',
            donemYil: $('#txtDonemYil').val(),
            belgeTur: $('#cmbBelgeTuru').val() || '0',
            belgeTarihi: $('#txtBelgeTarihi').val(),
            belgeNo: $('#txtBelgeNo').val(),
            dilSeviyesi: $('#txtDilSeviyesi').val(),
            bordroyaEsas: $('#chkBordroyaEsas').prop('checked'),
            kurumFaydalaniyor: $('#chkKurumFaydalaniyor').prop('checked')
        };
    }

    // Eski btnKaydet_Click: başarıda kayıt kodu saklanır, liste yenilenir, form temizlenmez. Söz kayıt kodu ile çözülür.
    function kaydetIstek() {
        var d = $.Deferred();
        mesgul(true);
        Sgb.gonder(KOK + 'kaydet', formOku())
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); d.reject(); return; }
                if (s.kod > 0) kod = s.kod;
                listele(false);
                mesaj(s.bilgi, 'success');
                d.resolve(kod);
            })
            .fail(function (xhr) { hataGoster(xhr); d.reject(); })
            .always(function () { mesgul(false); });
        return d.promise();
    }

    $('#btnKaydet').on('click', function () {
        PbEgitim.onay('Kayıt işlemi yapılacak. Onaylıyor musunuz?').then(kaydetIstek, function () { });
    });

    // Eski btnSil_Click
    $('#btnSil').on('click', function () {
        if (personelKod <= 0 || kod <= 0) { mesaj('Lütfen silmek istediğiniz Dil bilgisini seçin.', 'warning'); return; }
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

    $('#btnTemizle').on('click', function () { temizle(); $('#cmbDil').trigger('focus'); });

    PbEgitim.yardim();
    secenekleriOku().always(function () { temizle(); });
    listele(true);
});
