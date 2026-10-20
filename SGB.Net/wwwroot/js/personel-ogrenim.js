// Personel İşlemleri > Öğrenim Durumu (InsanKaynaklari/PersonelOgrenim; eski PersonelOgrenim.aspx).
// Solda öğrenim formu ve kayda ekli belgeler, sağda personelin öğrenim kayıtları. Listeden seçilen kayıt forma gelir;
// Kaydet / Sil onay sorar, Yeni Kayıt formu boşaltır. Eğitim seviyesinin sözlük kayıt kodu üniversite / fakülte / bölüm
// alanlarını gösterir ya da gizler (eski OgrenimKodDegisti).
$(function () {
    'use strict';

    var $sayfa = $('#pbOgrenim');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var KOK = 'api/personelbilgi/ogrenim/';
    var personelKod = parseInt($sayfa.data('personel-kod'), 10) || 0;
    var kod = 0; // eski hdnKod
    var mesaj = PbEgitim.mesaj, hataGoster = PbEgitim.hataGoster;

    // Eski OgrenimKodDegisti: bu kayıt kodlu eğitim seviyelerinde üniversite / fakülte / bölüm görünür.
    var UNIVERSITELI = ['08.00007', '08.00008', '08.00009', '17.00002', '17.00003', '17.00004', '17.00005', '17.00008', '17.00009'];

    function mesgul(evet) { $('#btnKaydet, #btnSil, #btnTemizle, #btnBelgeEkle').prop('disabled', evet); }

    var grid = PbEgitim.listeGrid('#grdListe', [
        { field: 'okulAdi', headerName: 'Okul Adı', flex: 1, minWidth: 150, tooltipField: 'okulAdi' },
        { field: 'baslangicTarihi', headerName: 'Mezun Tarihi', width: 105, valueFormatter: PbEgitim.tarihHucre },
        { field: 'memuriyeteBasladigiOkul', headerName: 'Memuriyete Başladığı Okul', width: 95, valueFormatter: PbEgitim.isaretHucre, cellStyle: { textAlign: 'center' }, headerTooltip: 'Memuriyete Başladığı Okul' },
        { field: 'gecerli', headerName: 'Son Okulu', width: 85, valueFormatter: PbEgitim.isaretHucre, cellStyle: { textAlign: 'center' } }
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

    function universiteGoster() {
        var deger = $('#cmbOgrenim').val() || '';
        var kayitKod = deger.split(':')[1];
        $('.uni-alan').prop('hidden', $.inArray(kayitKod, UNIVERSITELI) < 0);
    }

    function denklikGoster() { $('#grpDenklik').prop('hidden', !$('#chkDenklik').prop('checked')); }

    // Eski OgrenimDoldur, UniversiteDoldur, FakulteDoldur, BolumDoldur, OgrenimYeriDoldur, OgrenimBelgeTurDoldur
    function secenekleriOku() {
        return Sgb.al(KOK + 'secenekler')
            .done(function (v) {
                PbEgitim.secenekDoldur('#cmbOgrenim', v.ogrenim);
                PbEgitim.secenekDoldur('#cmbUniversite', v.universite);
                PbEgitim.secenekDoldur('#cmbFakulte', v.fakulte);
                PbEgitim.secenekDoldur('#cmbBolum', v.bolum);
                PbEgitim.secenekDoldur('#cmbOgrenimYeri', v.ogrenimYeri);
                PbEgitim.secenekDoldur('#cmbBelgeTur', v.belgeTur);
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
        $('#frmOgrenim')[0].reset();
        $('#frmOgrenim select').prop('selectedIndex', 0);
        denklikGoster();
        grid.deselectAll();
        belgeler.temizle();
    }

    // Eski SatirSecildi
    function doldur(o) {
        if ((parseInt(o.ogrenimKod, 10) || 0) > 0) {
            PbEgitim.secimYap('#cmbOgrenim', o.ogrenimSecim);
            universiteGoster();
        } else {
            PbEgitim.secimYap('#cmbOgrenim', '');
        }
        kod = parseInt(o.kod, 10) || 0;
        PbEgitim.secimYap('#cmbUniversite', o.universiteKod);
        PbEgitim.secimYap('#cmbFakulte', o.fakulteKod);
        PbEgitim.secimYap('#cmbBolum', o.bolumKod);
        $('#txtOkulAdi').val(o.okulAdi || '');
        $('#txtBaslamaTarihi').val(o.baslangicTarihi || '');
        PbEgitim.secimYap('#cmbBelgeTur', o.belgeTur);
        $('#txtBitirmeTarihi').val(o.belgeTarihi || '');
        $('#txtBelgeNo').val(o.belgeNo || '');
        $('#txtDerecesi').val(o.derece);
        $('#chkGecerli').prop('checked', PbEgitim.evetMi(o.gecerli));
        PbEgitim.secimYap('#cmbOgrenimYeri', o.ogrenimYeri);
        $('#chkDenklik').prop('checked', PbEgitim.evetMi(o.denklik));
        if (PbEgitim.evetMi(o.denklik)) {
            $('#txtDenklikTarihi').val(o.denklikTarihi || '');
            $('#txtDenklikBolum').val(o.denkBolum || '');
            $('#txtDenklikOkul').val(o.denkOkul || '');
        } else {
            $('#txtDenklikTarihi, #txtDenklikBolum, #txtDenklikOkul').val('');
        }
        denklikGoster();
        $('#chkHazirlik').prop('checked', PbEgitim.evetMi(o.hazirlikVarMi));
        $('#txtKurumIbrazTarihi').val(o.kurumaIbrazTarihi || '');
        $('#chkMemuriyeteBaslama').prop('checked', PbEgitim.evetMi(o.memuriyeteBasladigiOkul));
        $('#txtOgrenimSuresi').val(o.ogrenimSuresi);
        belgeler.yenile();
    }

    function formOku() {
        return {
            personelKod: personelKod,
            kod: String(kod || ''),
            ogrenimSecim: $('#cmbOgrenim').val() || '',
            universiteKod: $('#cmbUniversite').val() || '0',
            fakulteKod: $('#cmbFakulte').val() || '0',
            bolumKod: $('#cmbBolum').val() || '0',
            okulAdi: $('#txtOkulAdi').val(),
            memuriyeteBasladigiOkul: $('#chkMemuriyeteBaslama').prop('checked'),
            gecerli: $('#chkGecerli').prop('checked'),
            ogrenimYeri: $('#cmbOgrenimYeri').val() || '0',
            ogrenimSuresi: $('#txtOgrenimSuresi').val(),
            baslangicTarihi: $('#txtBaslamaTarihi').val(),
            belgeTur: $('#cmbBelgeTur').val() || '0',
            belgeNo: $('#txtBelgeNo').val(),
            derece: $('#txtDerecesi').val(),
            belgeTarihi: $('#txtBitirmeTarihi').val(),
            denklik: $('#chkDenklik').prop('checked'),
            denkBolum: $('#txtDenklikBolum').val(),
            denkOkul: $('#txtDenklikOkul').val(),
            denklikTarihi: $('#txtDenklikTarihi').val(),
            hazirlikVarMi: $('#chkHazirlik').prop('checked'),
            kurumaIbrazTarihi: $('#txtKurumIbrazTarihi').val()
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

    $('#cmbOgrenim').on('change', universiteGoster);
    $('#chkDenklik').on('change', denklikGoster);
    $('#txtDerecesi').on('input', function () { this.value = this.value.replace(/[^0-9]/g, ''); });

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

    $('#btnTemizle').on('click', function () { temizle(); $('#cmbOgrenim').trigger('focus'); });

    PbEgitim.yardim();
    secenekleriOku().always(function () { temizle(); });
    listele(true);
});
