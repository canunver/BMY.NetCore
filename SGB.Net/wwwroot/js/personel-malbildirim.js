// Personel İşlemleri "Mal Bildirimi" sekmesi (InsanKaynaklari/PersonelMalBildirimi; eski PersonelMalBildirimi.aspx).
// Sağda personelin mal bildirimleri listelenir; satıra tıklanınca bildirimin tarihi, açıklaması ve dokuz tablosu okunur.
// Tablolara "Satır Ekle" ile satır eklenir, hücreler doğrudan düzenlenir. Kaydet bildirimi bütün tablolarıyla kaydeder
// (yeni kayıt ya da seçili bildirimin güncellenmesi), Sil seçili bildirimi siler, Yeni Kayıt formu boşaltır.
$(function () {
    'use strict';

    var $sayfa = $('#pmbSayfa');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var personelKod = parseInt($sayfa.data('personel-kod'), 10) || 0;
    var tcKimlikNo = String($sayfa.data('tc-kimlik-no') || '');
    var seciliKod = 0;

    var sayiBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnSil, #btnYeni').prop('disabled', evet); }

    // Sayfa içi onay penceresi; söz (promise) evet ise çözülür.
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

    // ------------------------------------------------------------------ sütun tanımları

    function metin(alan, baslik, genislik, uzun) {
        var s = { field: alan, headerName: baslik, width: genislik, editable: true, cellDataType: 'text', tooltipField: alan };
        if (uzun) { s.cellEditor = 'agLargeTextCellEditor'; s.cellEditorPopup = true; s.cellEditorParams = { maxLength: 1000 }; }
        return s;
    }
    function tamsayi(alan, baslik, genislik) {
        return {
            field: alan, headerName: baslik, width: genislik, editable: true, cellDataType: 'number', type: 'rightAligned',
            cellEditor: 'agNumberCellEditor', cellEditorParams: { precision: 0 }
        };
    }
    function tutar(alan, baslik, genislik) {
        return {
            field: alan, headerName: baslik, width: genislik, editable: true, cellDataType: 'number', type: 'rightAligned',
            cellEditor: 'agNumberCellEditor', cellEditorParams: { precision: 2 },
            valueFormatter: function (p) { return p.value === null || p.value === undefined || p.value === '' ? '' : sayiBicim.format(p.value); }
        };
    }
    function tarih(alan, baslik, genislik) {
        return {
            field: alan, headerName: baslik, width: genislik || 125, editable: true, cellDataType: 'dateString',
            cellEditor: 'agDateStringCellEditor',
            valueFormatter: function (p) { return p.value ? Sgb.tarih(p.value) : ''; }
        };
    }

    // Her tablonun: liste alanı (JSON), sütunları, sayı / tarih alanları (kaydetmeden önce düzeltilir).
    var TABLOLAR = {
        Kimlik: {
            liste: 'kimlikListesi',
            sutunlar: [tamsayi('siraNo', 'Sıra No', 80), metin('adiSoyadi', 'Adı Soyadı', 220), tarih('dogumTarihi', 'Doğum Tarihi'),
                metin('dogumYeri', 'Doğum Yeri', 150), metin('yakinligi', 'Yakınlığı', 140), metin('tcKimlikNo', 'T.C. Kimlik No', 150)],
            sayilar: ['siraNo'], tarihler: ['dogumTarihi']
        },
        Tasinmaz: {
            liste: 'tasinmazListesi',
            sutunlar: [tamsayi('siraNo', 'Sıra No', 80), tutar('degeri', 'Değeri', 130), metin('tasinmazCinsi', 'Taşınmazın Cinsi', 170),
                metin('adres', 'Adresi (Mah., Ada, Parsel)', 300, true), tutar('hisseMiktari', 'Hisse Miktarı', 120),
                tarih('edinmeTarihi', 'Edinme Tarihi'), metin('malikTCKN', 'Malikin T.C. Kimlik No', 170)],
            sayilar: ['siraNo', 'degeri', 'hisseMiktari'], tarihler: ['edinmeTarihi']
        },
        Kooperatif: {
            liste: 'kooperatifListesi',
            sutunlar: [tamsayi('siraNo', 'Sıra No', 80), metin('kooperatifAdiYeri', 'Kooperatifin Adı ve Yeri', 300, true),
                tutar('hisseDegeri', 'Hisse Değeri', 130), tarih('uyelikTarihi', 'Üyelik Tarihi'), metin('tcKimlikNo', 'Hissedar T.C. Kimlik No', 190)],
            sayilar: ['siraNo', 'hisseDegeri'], tarihler: ['uyelikTarihi']
        },
        Tasit: {
            liste: 'tasitListesi',
            sutunlar: [tamsayi('siraNo', 'Sıra No', 80), metin('plakaNo', 'Plaka No', 110), metin('tasitinCinsi', 'Taşıtın Cinsi', 150),
                metin('tasitinMarkasi', 'Taşıtın Markası', 150), tutar('edinmeDegeri', 'Edinme Değeri', 130), tamsayi('modelYili', 'Model Yılı', 100),
                tarih('edinmeTarihi', 'Edinme Tarihi'), metin('tcKimlikNo', 'Sahibinin T.C. Kimlik No', 190)],
            sayilar: ['siraNo', 'edinmeDegeri', 'modelYili'], tarihler: ['edinmeTarihi']
        },
        DigerTasinir: {
            liste: 'digerTasinirListesi',
            sutunlar: [tamsayi('siraNo', 'Sıra No', 80), metin('tasinirMalCinsi', 'Taşınır Malın Cinsi', 280, true),
                tutar('edinmeDegeri', 'Edinme Değeri', 140), tamsayi('modelYili', 'Model Yılı', 100), tarih('edinmeTarihi', 'Edinme Tarihi'),
                metin('tcKimlikNo', 'Sahibinin T.C. Kimlik No', 200)],
            sayilar: ['siraNo', 'edinmeDegeri', 'modelYili'], tarihler: ['edinmeTarihi']
        },
        BankaMenkul: {
            liste: 'bankaMenkulListesi',
            sutunlar: [tamsayi('siraNo', 'Sıra No', 80), metin('nitelik', 'Para veya Menkul Değerin Niteliği', 300),
                tutar('miktari', 'Miktarı', 140), metin('cinsi', 'Cinsi', 120), metin('tcKimlikNo', 'Sahibinin T.C. Kimlik No', 200)],
            sayilar: ['siraNo', 'miktari'], tarihler: []
        },
        AltinMucevher: {
            liste: 'altinMucevherListesi',
            sutunlar: [tamsayi('siraNo', 'Sıra No', 80), metin('cinsi', 'Cinsi', 250), tutar('degeri', 'Değeri', 140),
                tutar('miktari', 'Miktarı', 120), metin('tcKimlikNo', 'Sahibinin T.C. Kimlik No', 200)],
            sayilar: ['siraNo', 'degeri', 'miktari'], tarihler: []
        },
        BorcAlacak: {
            liste: 'borcAlacakListesi',
            sutunlar: [tamsayi('siraNo', 'Sıra No', 80), metin('borcluAdi', 'Borçlunun Adı Soyadı', 250),
                metin('alacakliAdi', 'Alacaklının Adı Soyadı', 250), tutar('tutar', 'Borç / Alacak Tutarı', 170)],
            sayilar: ['siraNo', 'tutar'], tarihler: []
        },
        DigerServet: {
            liste: 'digerServetListesi',
            sutunlar: [tamsayi('siraNo', 'Sıra No', 80), metin('servetUnsuru', 'Hak veya Beyanı Gerekli Görülen Diğer Servet Unsurları', 400, true),
                metin('edinmeSekli', 'Edinme Şekli', 190), metin('tcKimlikNo', 'Sahibinin T.C. Kimlik No', 200)],
            sayilar: ['siraNo'], tarihler: []
        }
    };

    $.each(TABLOLAR, function (ad, t) {
        t.grid = Sgb.grid('#grd' + ad, {
            columnDefs: t.sutunlar,
            suppressCellFocus: false,
            singleClickEdit: true,
            stopEditingWhenCellsLoseFocus: true,
            overlayNoRowsTemplate: '<span class="text-muted">Satır yok. "Satır Ekle" ile ekleyin.</span>'
        });
    });

    // ------------------------------------------------------------------ mal bildirimleri listesi

    var listeGrid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'tarih', headerName: 'Tarih', width: 95, cellClass: 'hucre-salt', valueFormatter: function (p) { return Sgb.tarih(p.value); } },
            { field: 'aciklama', headerName: 'Açıklama', flex: 1, minWidth: 140, cellClass: 'hucre-salt', tooltipField: 'aciklama' }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { oku(e.data.kod); },
        overlayNoRowsTemplate: '<span class="text-muted">Mal bildirimi yok</span>'
    });

    // ilk: açılıştaki okuma; tablo "Yükleniyor..." göstermez.
    function listele(ilk) {
        if (personelKod <= 0) return $.Deferred().resolve().promise();
        if (!ilk) listeGrid.setGridOption('loading', true);
        return Sgb.al('api/personelbilgi/malbildirim/liste', { personelKod: personelKod })
            .done(function (veri) {
                listeGrid.setGridOption('rowData', veri.satirlar || []);
                if (veri.hata) mesaj(veri.hata, 'danger');
                if (seciliKod > 0) {
                    var n = listeGrid.getRowNode(String(seciliKod));
                    if (n) n.setSelected(true);
                }
            })
            .fail(hataGoster)
            .always(function () { listeGrid.setGridOption('loading', false); });
    }

    function tablolariDoldur(veri) {
        $.each(TABLOLAR, function (ad, t) {
            var satirlar = $.map(veri ? (veri[t.liste] || []) : [], function (s) {
                var k = $.extend({}, s);
                $.each(t.tarihler, function (i, a) { if (!k[a]) k[a] = null; });
                return k;
            });
            t.grid.setGridOption('rowData', satirlar);
        });
    }

    function temizle() {
        seciliKod = 0;
        $('#txtTarih').val('');
        $('#txtAciklama').val('');
        tablolariDoldur(null);
        listeGrid.deselectAll();
    }

    // Eski malBildirimiOku / OkuMalBildirimi
    function oku(kod) {
        mesgul(true);
        Sgb.al('api/personelbilgi/malbildirim/oku', { kod: parseInt(kod, 10) || 0 })
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                seciliKod = veri.kod;
                $('#txtTarih').val(veri.tarih || '');
                $('#txtAciklama').val(veri.aciklama || '');
                tablolariDoldur(veri);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    }

    // Eski gridDataAl: düzenlenmekte olan hücre bitirilir, boş sayı 0'a, boş tarih ''ye çevrilir.
    function tabloVerisi(t) {
        t.grid.stopEditing();
        var liste = [];
        t.grid.forEachNode(function (n) {
            var k = $.extend({}, n.data);
            $.each(t.sayilar, function (i, a) {
                var s = parseFloat(k[a]);
                k[a] = isNaN(s) ? 0 : (a === 'siraNo' || a === 'modelYili' ? Math.round(s) : s);
            });
            $.each(t.tarihler, function (i, a) { k[a] = k[a] || ''; });
            liste.push(k);
        });
        return liste;
    }

    // ------------------------------------------------------------------ düğmeler

    $sayfa.on('click', '[data-ekle]', function () {
        var t = TABLOLAR[$(this).data('ekle')];
        var yeni = {};
        $.each(t.sutunlar, function (i, s) { yeni[s.field] = s.cellDataType === 'text' ? '' : null; });
        yeni.siraNo = t.grid.getDisplayedRowCount() + 1;
        var sonuc = t.grid.applyTransaction({ add: [yeni] });
        if (sonuc && sonuc.add && sonuc.add.length) {
            var i = sonuc.add[0].rowIndex;
            t.grid.ensureIndexVisible(i);
            t.grid.setFocusedCell(i, t.sutunlar[1].field);
        }
    });

    $sayfa.on('click', '[data-cikar]', function () {
        var t = TABLOLAR[$(this).data('cikar')];
        t.grid.stopEditing();
        var h = t.grid.getFocusedCell();
        var n = h ? t.grid.getDisplayedRowAtIndex(h.rowIndex) : null;
        if (!n) { mesaj('Lütfen silmek istediğiniz satırı seçiniz.', 'warning'); return; }
        t.grid.applyTransaction({ remove: [n.data] });
    });

    $('#btnYeni').on('click', function () {
        temizle();
        $('#txtTarih').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        var istek = {
            kod: seciliKod,
            personelKod: personelKod,
            tcKimlikNo: tcKimlikNo,
            tarih: $('#txtTarih').val(),
            aciklama: $('#txtAciklama').val()
        };
        $.each(TABLOLAR, function (ad, t) { istek[t.liste] = tabloVerisi(t); });

        var eksik = [];
        if (personelKod <= 0) eksik.push('Personel Bilgisi boş bırakılamaz.');
        if (!istek.tarih) eksik.push('Mal Bildirim Tarihi boş bırakılamaz.');
        if ($.trim(istek.aciklama) === '') eksik.push('Açıklama alanı boş bırakılamaz.');
        if (eksik.length) { mesaj(eksik.join('\n'), 'warning'); return; }

        onayla('Kayıt işlemi yapılacak. Onaylıyor musunuz?').then(function (evet) {
            if (!evet) return;
            mesgul(true);
            Sgb.gonder('api/personelbilgi/malbildirim/kaydet', istek)
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    mesaj(s.bilgi, 'success');
                    temizle();
                    listele();
                })
                .fail(hataGoster)
                .always(function () { mesgul(false); });
        });
    });

    $('#btnSil').on('click', function () {
        if (seciliKod <= 0) { mesaj('Lütfen silmek istediğiniz kaydı seçiniz.', 'warning'); return; }
        onayla('Kayıt silinecek. Onaylıyor musunuz?').then(function (evet) {
            if (!evet) return;
            mesgul(true);
            Sgb.gonder('api/personelbilgi/malbildirim/sil', { kod: seciliKod, personelKod: personelKod, tcKimlikNo: tcKimlikNo })
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    mesaj(s.bilgi, 'success');
                    temizle();
                    listele();
                })
                .fail(hataGoster)
                .always(function () { mesgul(false); });
        });
    });

    temizle();
    listele(true);
});
