// Sendika Bilgisi (InsanKaynaklari/SendikaBilgiForm): solda sendika listesi, sağda seçilen sendika ve üye personeli.
// Eski sayfadaki gibi: Yeni Sendika formu boşaltır, Kaydet onay sorar; Personel Ekle ortak personel seçimini (Sgb.personelSec, çoklu)
// açar ve seçilenleri "Kes. Oranı" ile ekler; Değer Ata listede seçili personele oranı verir; Seç. Per. Sil seçili personeli çıkarır.
// Uçlar: /api/sendika/* (InsanKaynaklari/SendikaBilgiUclari.cs).
$(function () {
    'use strict';

    if (!$('#grdSendika').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/sendika/';

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnYeni, #btnKaydet, #btnPersonelEkle, #btnDegerAta, #btnPersonelSil').prop('disabled', evet); }

    var grdSendika = Sgb.grid('#grdSendika', {
        columnDefs: [
            { field: 'sendikaNo', headerName: 'No', width: 70, type: 'numericColumn' },
            { field: 'sendikaAd', headerName: 'Sendika Ad', flex: 1, minWidth: 160, tooltipField: 'sendikaAd' }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.sendikaNo); },
        onRowClicked: function (e) { if (e.data) oku(e.data.sendikaNo); },
        overlayNoRowsTemplate: '<span class="text-muted">Sendika yok</span>'
    });

    var grdPersonel = Sgb.grid('#grdPersonel', {
        columnDefs: [
            { field: 'kod', headerName: 'PKod', width: 80, type: 'numericColumn' },
            { field: 'tcKimlikNo', headerName: 'TC', width: 115 },
            { field: 'sicilNo', headerName: 'Sicil', width: 90 },
            { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 110 },
            { field: 'soyad', headerName: 'Soyad', flex: 1, minWidth: 110 },
            {
                field: 'kesintiOrani', headerName: 'Kes. Oranı', width: 100, type: 'numericColumn',
                valueFormatter: function (p) { return p.value == null ? '' : Number(p.value).toLocaleString('tr-TR', { maximumFractionDigits: 4 }); }
            }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        overlayNoRowsTemplate: '<span class="text-muted">Sendika seçiniz ya da personel ekleyiniz.</span>'
    });

    function sendikaNo() { return parseInt($('#txtSendikaNo').val(), 10) || 0; }
    function oran() { return parseFloat(String($('#txtKesintiOrani').val()).replace(',', '.')) || 0; }

    // Eski Listele; ilk: açılışta "Yükleniyor..." gösterilmez
    function listele(ilk, secilecek) {
        if (!ilk) grdSendika.setGridOption('loading', true);
        return Sgb.al(UC + 'liste')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grdSendika.setGridOption('rowData', v.satirlar || []);
                if (secilecek) {
                    var n = grdSendika.getRowNode(String(secilecek));
                    if (n) { n.setSelected(true); grdSendika.ensureNodeVisible(n); }
                }
            })
            .fail(hataGoster)
            .always(function () { grdSendika.setGridOption('loading', false); });
    }

    // Eski YeniKayit
    function temizle() {
        $('#frmSendika')[0].reset();
        $('#txtTopluGorusmePuani').val('0');
        $('#txtKesintiOrani').val('');
        grdPersonel.setGridOption('rowData', []);
        grdSendika.deselectAll();
    }

    // Eski SendikaSecildiIc
    function oku(no) {
        if (!no) return $.Deferred().resolve().promise();
        grdPersonel.setGridOption('loading', true);
        return Sgb.al(UC + 'oku', { sendikaNo: no })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'warning'); return; }
                var b = v.bilgi || {};
                $('#txtSendikaNo').val(b.sendikaNo);
                $('#txtSendikaAd').val(b.sendikaAd || '');
                $('#txtAdres').val(b.adres || '');
                $('#txtIban').val(b.iBAN || '');
                $('#txtHesapNo').val(b.hesapNo || '');
                $('#txtTopluGorusmePuani').val(b.topluGorusmePuani == null ? '0' : b.topluGorusmePuani);
                $('#txtKesintiOrani').val(b.kesintiOrani == null ? '' : b.kesintiOrani);
                grdPersonel.setGridOption('rowData', v.personel || []);
            })
            .fail(hataGoster)
            .always(function () { grdPersonel.setGridOption('loading', false); });
    }

    function seciliPersonel() {
        var s = grdPersonel.getSelectedRows();
        return s.length ? s[0] : null;
    }

    $('#btnYeni').on('click', function () { temizle(); $('#txtSendikaAd').trigger('focus'); });

    $('#btnKaydet').on('click', function () {
        if ($.trim($('#txtSendikaAd').val()) === '') { mesaj('Sendika adı boş bırakılamaz.', 'warning'); return; }
        if (!window.confirm('Kaydetmek İstiyor musunuz?')) return;
        var istek = {
            sendikaNo: sendikaNo(),
            sendikaAd: $('#txtSendikaAd').val(),
            adres: $('#txtAdres').val(),
            iBAN: $('#txtIban').val(),
            hesapNo: $('#txtHesapNo').val(),
            topluGorusmePuani: $.trim($('#txtTopluGorusmePuani').val()),
            kesintiOrani: oran()
        };
        mesgul(true);
        Sgb.gonder(UC + 'kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                if (s.sendikaNo) $('#txtSendikaNo').val(s.sendikaNo);
                listele(false, s.sendikaNo);
                oku(s.sendikaNo);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // Eski btnKisiEkleClick + PersonelleriEkle (ListePersonelSec?cokluSec=1&maas=1)
    $('#btnPersonelEkle').on('click', function () {
        var no = sendikaNo();
        if (!no) { mesaj('Önce bir sendika seçiniz ya da kaydediniz.', 'warning'); return; }
        Sgb.personelSec({
            baslik: 'Sendikaya Personel Ekle',
            coklu: true,
            secilince: function (satirlar) {
                var kodlar = $.map(satirlar, function (r) { return r.kod; });
                mesgul(true);
                Sgb.gonder(UC + 'personel-ekle', { sendikaNo: no, kodlar: kodlar, kesintiOrani: oran() })
                    .done(function (s) {
                        if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                        mesaj(kodlar.length + ' personel sendikaya eklendi.', 'success');
                        oku(no);
                    })
                    .fail(hataGoster)
                    .always(function () { mesgul(false); });
            }
        });
    });

    // Eski btnKisiDegerAtaClick
    $('#btnDegerAta').on('click', function () {
        var p = seciliPersonel();
        if (!p) { mesaj('Lütfen listeden personel seçiniz.', 'warning'); return; }
        var no = sendikaNo();
        mesgul(true);
        Sgb.gonder(UC + 'oran-ata', { pkod: p.kod, kesintiOrani: oran() })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                oku(no);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // Eski PersonelCikart
    $('#btnPersonelSil').on('click', function () {
        var p = seciliPersonel();
        if (!p) { mesaj('Lütfen listeden personel seçiniz.', 'warning'); return; }
        if (!window.confirm('Seçili kişi personel listesinden çıkartılacaktır, onaylıyor musunuz?')) return;
        var no = sendikaNo();
        mesgul(true);
        Sgb.gonder(UC + 'personel-cikar', { pkod: p.kod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                oku(no);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    temizle();
    listele(true);
});
