// Personel İşlemleri "Kart" sekmesi (InsanKaynaklari/PersonelKart; eski PersonelKart.aspx).
// Sağda personelin kartları listelenir; satıra tıklanınca kart forma okunur (eski SatirSecildi). Kaydet yeni kartı ya da seçili kartı
// kaydeder, Sil seçili kartı siler, Yeni Kayıt formu boşaltır (eski btnTemizle_Click).
// Zimmet eden / iade alan personel ortak personel seçimiyle (Sgb.personelSec, js/personelsec.js) seçilir (eski PersonelSorguAc + BilgiListeleAc).
$(function () {
    'use strict';

    var $sayfa = $('#pktSayfa');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var personelKod = parseInt($sayfa.data('personel-kod'), 10) || 0;
    var seciliKod = 0, zimmetEden = 0, iadeAlan = 0;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnSil, #btnTemizle').prop('disabled', evet); }

    function onayla(metin) {
        var d = document.getElementById('dlgOnay');
        $('#dlgOnayMetin').text(metin);
        return new Promise(function (coz) {
            function kapandi() {
                d.removeEventListener('close', kapandi);
                coz(d.returnValue === 'evet');
            }
            d.returnValue = '';
            d.addEventListener('close', kapandi);
            d.showModal();
        });
    }

    function secenekDoldur($ddl, liste) {
        $ddl.empty();
        $.each(liste || [], function (i, s) { $('<option></option>').val(String(s.KOD)).text(s.AD).appendTo($ddl); });
    }

    function seciliYap($ddl, deger) {
        $ddl.val(String(deger == null ? 0 : deger));
        if ($ddl.val() === null) $ddl.prop('selectedIndex', 0);
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'ZIMMETTAR', headerName: 'Zimmet Tarihi', width: 115, cellClass: 'hucre-salt', valueFormatter: function (p) { return Sgb.tarih(p.value); } },
            { field: 'KARTTIPI', headerName: 'Kart Tipi', width: 150, cellClass: 'hucre-salt', tooltipField: 'KARTTIPI' },
            { field: 'KARTNO', headerName: 'Kart No', width: 120, cellClass: 'hucre-salt' },
            { field: 'ADSOYAD', headerName: 'Ad Soyad', flex: 1, minWidth: 160, cellClass: 'hucre-salt', tooltipField: 'ADSOYAD' },
            { field: 'AKTIF', headerName: 'Aktif Mi?', width: 90, cellClass: 'hucre-salt' }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.KOD); },
        onRowClicked: function (e) { oku(e.data.KOD); },
        pagination: true,
        paginationPageSize: 25,
        paginationPageSizeSelector: false,
        overlayNoRowsTemplate: '<span class="text-muted">Kart yok</span>'
    });

    // Eski Listele; ilk: açılıştaki okuma ("Yükleniyor..." gösterilmez).
    function listele(ilk) {
        if (personelKod <= 0) return;
        if (!ilk) grid.setGridOption('loading', true);
        Sgb.al('api/personelbilgi/kart/liste', { personelKod: personelKod })
            .done(function (veri) {
                grid.setGridOption('rowData', veri.satirlar || []);
                if (veri.hata) mesaj(veri.hata, 'danger');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    // Eski Temizle
    function temizle() {
        seciliKod = 0;
        zimmetEden = 0;
        iadeAlan = 0;
        $('#frmKart')[0].reset();
        $('#ddlTip, #ddlYakinlik').prop('selectedIndex', 0);
        grid.deselectAll();
    }

    // Eski SatirSecildi
    function oku(kod) {
        temizle();
        kod = parseInt(kod, 10) || 0;
        if (kod <= 0) return;
        var n = grid.getRowNode(String(kod));
        if (n) n.setSelected(true);
        Sgb.al('api/personelbilgi/kart/oku', { personelKod: personelKod, kod: kod })
            .done(function (k) {
                if (k.hata) { mesaj(k.hata, 'danger'); return; }
                seciliKod = k.kod;
                $('#txtKartNo').val(k.kartNo || '');
                $('#txtAdSoyad').val(k.adiSoyadi || '');
                $('#txtPlaka').val(k.plaka || '');
                $('#txtCihazNo').val(k.tagCihazNo || '');
                $('#txtZimmetTarihi').val(k.zimmetTarihi || '');
                $('#txtIadeTarihi').val(k.iadeTarihi || '');
                $('#txtAciklama').val(k.aciklama || '');
                $('#txtKayipTarihi').val(k.kayipTarihi || '');
                seciliYap($('#ddlTip'), k.kartTipi);
                seciliYap($('#ddlYakinlik'), k.yakinligi);
                zimmetEden = k.zimmetEdenPersonel || 0;
                $('#txtZimmetEden').val(k.zimmetEdenPersonelAd || '');
                iadeAlan = k.iadeAlanPersonel || 0;
                $('#txtIadeAlan').val(k.iadeAlanPersonelAd || '');
            })
            .fail(hataGoster);
    }

    function formOku() {
        return {
            kod: seciliKod,
            personelKod: personelKod,
            kartTipi: parseInt($('#ddlTip').val(), 10) || 0,
            kartNo: $('#txtKartNo').val(),
            yakinligi: parseInt($('#ddlYakinlik').val(), 10) || 0,
            adiSoyadi: $('#txtAdSoyad').val(),
            plaka: $('#txtPlaka').val(),
            tagCihazNo: $('#txtCihazNo').val(),
            zimmetTarihi: $('#txtZimmetTarihi').val() || '',
            iadeTarihi: $('#txtIadeTarihi').val() || '',
            aciklama: $('#txtAciklama').val(),
            kayipTarihi: $('#txtKayipTarihi').val() || '',
            zimmetEdenPersonel: zimmetEden,
            iadeAlanPersonel: iadeAlan
        };
    }

    // Eski PersonelSorguAc(tip) + BilgiListeleAc: 1 = zimmet eden, 2 = iade alan
    $sayfa.on('click', '[data-sorgu]', function () {
        var tip = parseInt($(this).data('sorgu'), 10) || 0;
        Sgb.personelSec({
            baslik: tip === 1 ? 'Zimmet Eden Personel' : 'İade Alan Personel',
            secilince: function (satirlar) {
                var v = satirlar[0], ad = v.adSoyad || ((v.AD || '') + ' ' + (v.SOYAD || ''));
                if (tip === 1) { zimmetEden = parseInt(v.kod, 10) || 0; $('#txtZimmetEden').val(ad); }
                else if (tip === 2) { iadeAlan = parseInt(v.kod, 10) || 0; $('#txtIadeAlan').val(ad); }
            }
        });
    });

    $('#btnTemizle').on('click', function () {
        temizle();
        $('#ddlTip').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        if (personelKod <= 0) { mesaj('Personel Bilgisi boş bırakılamaz.', 'warning'); return; }
        var istek = formOku();
        onayla('Kayıt işlemi yapılacak. Onaylıyor musunuz?').then(function (evet) {
            if (!evet) return;
            mesgul(true);
            Sgb.gonder('api/personelbilgi/kart/kaydet', istek)
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    mesaj(s.bilgi, 'success');
                    temizle();
                    listele(false);
                })
                .fail(hataGoster)
                .always(function () { mesgul(false); });
        });
    });

    $('#btnSil').on('click', function () {
        if (seciliKod <= 0) { mesaj('Silmek istediğiniz kaydı seçiniz.', 'warning'); return; }
        onayla('Kayıt silinecek. Onaylıyor musunuz?').then(function (evet) {
            if (!evet) return;
            mesgul(true);
            Sgb.gonder('api/personelbilgi/kart/sil', { kod: seciliKod, personelKod: personelKod })
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    mesaj(s.bilgi, 'success');
                    temizle();
                    listele(false);
                })
                .fail(hataGoster)
                .always(function () { mesgul(false); });
        });
    });

    Sgb.al('api/personelbilgi/kart/secenekler')
        .done(function (veri) {
            secenekDoldur($('#ddlTip'), veri.kartTipleri);
            secenekDoldur($('#ddlYakinlik'), veri.yakinliklar);
        })
        .fail(hataGoster)
        .always(function () { temizle(); });
    listele(true);
});
