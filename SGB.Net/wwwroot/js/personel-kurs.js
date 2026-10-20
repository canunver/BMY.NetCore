// Personel İşlemleri > Hizmet İçi Eğitim (InsanKaynaklari/PersonelKurs; eski PersonelKurs.aspx).
// Solda eğitim formu ve kayda ekli belgeler, sağda personelin eğitim kayıtları (bitirme tarihine göre yeniden eskiye).
// Listeden seçilen kayıt forma gelir; Kaydet / Sil onay sorar, Yeni Kayıt formu boşaltır.
$(function () {
    'use strict';

    var $sayfa = $('#pbKurs');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var KOK = 'api/personelbilgi/kurs/';
    var personelKod = parseInt($sayfa.data('personel-kod'), 10) || 0;
    var kod = 0; // eski hdnKod
    var mesaj = PbEgitim.mesaj, hataGoster = PbEgitim.hataGoster;

    function mesgul(evet) { $('#btnKaydet, #btnSil, #btnTemizle, #btnBelgeEkle').prop('disabled', evet); }

    var grid = PbEgitim.listeGrid('#grdListe', [
        { field: 'kursAdi', headerName: 'Adı', flex: 1, minWidth: 150, tooltipField: 'kursAdi' },
        { field: 'baslamaTarihi', headerName: 'Başlama Tarihi', width: 110, valueFormatter: PbEgitim.tarihHucre },
        { field: 'bitirmeTarihi', headerName: 'Bitirme Tarihi', width: 110, valueFormatter: PbEgitim.tarihHucre, sort: 'desc' } // eski SortInfo BITIRMETARIH DESC
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

    // Eski KursDoldur, OgrenimYeriDoldur, yıl listesi
    function secenekleriOku() {
        return Sgb.al(KOK + 'secenekler')
            .done(function (v) {
                PbEgitim.secenekDoldur('#cmbKurs', v.kurs);
                PbEgitim.secenekDoldur('#cmbOgrenimYeri', v.egitimYeri);
                Sgb.yilKutusu('#txtYil', [v.yilEnAz, v.yilEnCok]);
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
        $('#frmKurs')[0].reset();
        $('#frmKurs select').prop('selectedIndex', 0);
        $('#txtBaslamaTarihi, #txtBitirmeTarihi').removeAttr('min max');
        grid.deselectAll();
        belgeler.temizle();
    }

    function sayiYaz(x) { return (parseInt(x, 10) || 0) !== 0 ? String(x) : ''; }

    // Eski SatirSecildi
    function doldur(o) {
        kod = parseInt(o.kod, 10) || 0;
        PbEgitim.secimYap('#cmbKurs', o.kursKod);
        $('#txtAlinanYer').val(o.aldigiYer || '');
        $('#txtBaslamaTarihi').val(o.baslamaTarihi || '');
        $('#txtBitirmeTarihi').val(o.bitirmeTarihi || '');
        $('#txtBaslamaTarihi').attr('max', o.bitirmeTarihi || null);
        $('#txtBitirmeTarihi').attr('min', o.baslamaTarihi || null);
        $('#txtPuan').val(o.puan);
        $('#txtDerece').val(o.derece || '');
        $('#txtKursAdi').val(o.kursAdi || '');
        $('#txtDersSaati').val(o.dersSaati);
        PbEgitim.secimYap('#cmbOgrenimYeri', o.egitimYeri);
        $('#txtYil').val(sayiYaz(o.yil));
        PbEgitim.secimYap('#cmbDonem', o.donem);
        $('#txtGun').val(parseInt(o.gun, 10) || 0);
        $('#txtDosyaNo').val(o.dosyaNo || '');
        $('#txtAciklama').val(o.aciklama || '');
        belgeler.yenile();
    }

    function formOku() {
        // Eski: Adı boşsa ve eğitim tipi seçiliyse Adı alanına seçilen eğitim tipinin metni yazılır.
        var $kurs = $('#cmbKurs');
        if ($.trim($('#txtKursAdi').val()) === '' && (parseInt($kurs.val(), 10) || 0) !== 0)
            $('#txtKursAdi').val($kurs.find('option:selected').text());
        return {
            personelKod: personelKod,
            kod: String(kod || ''),
            kursKod: $kurs.val() || '0',
            kursAdi: $('#txtKursAdi').val(),
            baslamaTarihi: $('#txtBaslamaTarihi').val(),
            bitirmeTarihi: $('#txtBitirmeTarihi').val(),
            aldigiYer: $('#txtAlinanYer').val(),
            egitimYeri: $('#cmbOgrenimYeri').val() || '0',
            gun: $('#txtGun').val(),
            dersSaati: $('#txtDersSaati').val(),
            yil: $('#txtYil').val(),
            donem: $('#cmbDonem').val() || '',
            derece: $('#txtDerece').val(),
            puan: $('#txtPuan').val(),
            dosyaNo: $('#txtDosyaNo').val(),
            aciklama: $('#txtAciklama').val()
        };
    }

    // Eski btnKaydet_Click: başarıda kayıt kodu saklanır, liste yenilenir, form temizlenmez. Söz kayıt kodu ile çözülür.
    function kaydetIstek() {
        var d = $.Deferred();
        if (new Date($('#txtBaslamaTarihi').val()) > new Date($('#txtBitirmeTarihi').val())) {
            // Eski tarih alanları birbirine bağlıydı (daterange): bitirme, başlamadan önce seçilemiyordu.
            mesaj('Bitirme Tarihi, Başlama Tarihinden önce olamaz.', 'warning');
            return d.reject().promise();
        }
        mesgul(true);
        Sgb.gonder(KOK + 'kaydet', formOku())
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); d.reject(); return; }
                if (s.kod > 0) kod = s.kod;
                if (s.kursAdi && $.trim($('#txtKursAdi').val()) === '') $('#txtKursAdi').val(s.kursAdi);
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
        if (personelKod <= 0 || kod <= 0) { mesaj('Lütfen silmek istediğiniz kaydı seçiniz.', 'warning'); return; }
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

    $('#btnTemizle').on('click', function () { temizle(); $('#cmbKurs').trigger('focus'); });

    // Eski daterange bağı: başlama tarihi bitirmenin alt sınırı, bitirme başlamanın üst sınırı.
    $('#txtBaslamaTarihi').on('change', function () { $('#txtBitirmeTarihi').attr('min', this.value || null); });
    $('#txtBitirmeTarihi').on('change', function () { $('#txtBaslamaTarihi').attr('max', this.value || null); });

    PbEgitim.yardim();
    secenekleriOku().always(function () { temizle(); });
    listele(true);
});
