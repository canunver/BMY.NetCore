// Dosya Hesap Hareket Formu (AlacakTakip/HesapHareket; eski HesapHareket.aspx). Takip İşlemleri ekranından
// Sgb.pencere('../AlacakTakip/HesapHareket?yil=2026&dosyaNo=12', 'Hesap Hareket') ile açılır.
//   Üst tablo : seçilen işlemin hesap satırları; yalnızca Tutar girilir (beyaz hücre).
//   Alt tablo : dosyanın kayıtlı hareketleri; tıklanan kaydın satırları üstte açılır ve Sil etkinleşir (eski SatirSecildi).
//   Kaydet    : onay sorulur; toplam ve tasarımdaki formül satırları sunucuda hesaplanır (eski btnKaydet_Click).
//   Yeni Kayıt: İşlem Seçimi penceresi; seçilen tasarımın formülsüz satırları üstte açılır (eski btnSec_Click).
//   Kapat     : çerçeve içindeyse pencereyi kapatır.
// Uçlar: /api/alacaktakip/hesaphareket/* (Istemci/AlacakTakip/HesapHareketUclari.cs).
$(function () {
    'use strict';

    if (!$('#grdSatirlar').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/alacaktakip/hesaphareket/';
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    var sorgu = new URLSearchParams(window.location.search);
    var yil = parseInt(sorgu.get('yil'), 10) || 0;
    var dosyaNoMetin = $.trim(sorgu.get('dosyaNo') || '');
    var dosyaNo = dosyaNoMetin === '-1' ? 0 : (parseInt(dosyaNoMetin, 10) || 0);

    // Eski gizli alanlar: hdnKod (seçili kaydın kodu), hdnTasarimKod (seçili işlem)
    var durum = { kod: 0, tasarimKod: 0 };

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function mesgul(evet) {
        $('#btnKaydet').prop('disabled', evet || !dosyaNo);
        $('#btnYeni').prop('disabled', evet);
        $('#btnSil').prop('disabled', evet || !durum.kod);
    }
    function siraNo(p) { return p.node && p.node.rowIndex != null ? p.node.rowIndex + 1 : ''; }
    function paraYaz(p) { return p.value == null || p.value === '' ? '' : para.format(p.value); }

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;
    function sor(metin, is) {
        $('#soruMetin').text(metin);
        soruIsi = is;
        dlgSoru.showModal();
    }
    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        var is = soruIsi;
        soruIsi = null;
        if (is) is();
    });
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- üst tablo: hesap satırları (eski gpData)

    var IPUCU_SATIR = '<span>Yeni Kayıt ile işlem seçiniz ya da alttaki listeden bir kayda tıklayınız.</span>';

    var gridSatir = Sgb.grid('#grdSatirlar', {
        columnDefs: [
            { headerName: 'Sıra No', width: 80, sortable: false, valueGetter: siraNo },
            { field: 'ADI', headerName: 'Açıklama', flex: 1, minWidth: 200, sortable: false, tooltipField: 'ADI', cellClass: 'hh-salt' },
            {
                field: 'TUTAR', headerName: 'Tutar', width: 150, sortable: false, type: 'rightAligned',
                editable: true, cellEditor: 'agNumberCellEditor', cellEditorParams: { precision: 2 },
                cellClass: 'hh-girilir', headerTooltip: 'Tutar girilebilir',
                valueParser: function (p) {
                    if (typeof p.newValue === 'number') return p.newValue;
                    var m = $.trim(String(p.newValue == null ? '' : p.newValue));
                    if (m === '') return null;
                    if (m.indexOf(',') >= 0) m = m.replace(/\./g, '').replace(',', '.');
                    var s = parseFloat(m);
                    return isNaN(s) ? p.oldValue : s;
                },
                valueFormatter: paraYaz
            },
            { field: 'BORCALACAK', hide: true },
            { field: 'HESAPKODU', hide: true }
        ],
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        suppressCellFocus: false,
        overlayNoRowsTemplate: IPUCU_SATIR
    });

    function satirlariYaz(satirlar) {
        gridSatir.setGridOption('rowData', satirlar || []);
    }

    function satirlariTopla() {
        gridSatir.stopEditing();
        var satirlar = [];
        gridSatir.forEachNode(function (n) {
            var d = n.data;
            var t = d.TUTAR;
            satirlar.push({
                KOD: parseInt(d.KOD, 10) || 0,
                ANAKOD: parseInt(d.ANAKOD, 10) || 0,
                SIRANO: d.SIRANO == null || d.SIRANO === '' ? -1 : (parseInt(d.SIRANO, 10) || 0),
                HESAPKODU: d.HESAPKODU || '',
                ADI: d.ADI || '',
                TUTAR: t == null || t === '' || isNaN(Number(t)) ? null : Number(t),
                BORCALACAK: d.BORCALACAK == null || d.BORCALACAK === '' ? null : parseInt(d.BORCALACAK, 10)
            });
        });
        return satirlar;
    }

    // ---------------------------------------------------------------- alt tablo: kayıtlı hareketler (eski grdListe)

    var gridListe = Sgb.grid('#grdListe', {
        columnDefs: [
            { headerName: 'Sıra No', width: 80, sortable: false, valueGetter: siraNo },
            { field: 'TARIH', headerName: 'Tarih', width: 100, valueFormatter: function (p) { return Sgb.tarih(p.value); } },
            { field: 'TASARIMADI', headerName: 'İşlem Adı', flex: 1, minWidth: 160, tooltipField: 'TASARIMADI' },
            { field: 'TOPLAMTUTAR', headerName: 'Tutar', width: 120, type: 'rightAligned', valueFormatter: paraYaz },
            { field: 'ODEMEEMRINO', headerName: 'Ödeme Emri No', width: 120 },
            { field: 'ISLEMYAPANAD', headerName: 'İşlem Yapan', width: 160, tooltipField: 'ISLEMYAPANAD' },
            { field: 'ISLEMTARIHI', headerName: 'İşlem Tarihi', width: 110, valueFormatter: function (p) { return Sgb.tarih(p.value); } }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.KOD); },
        pagination: true,
        paginationPageSize: 150,
        paginationPageSizeSelector: false,
        overlayNoRowsTemplate: dosyaNo ? '<span>Bu dosya için kayıtlı hesap hareketi yok.</span>' : '<span>Dosya seçilmeden açıldı.</span>',
        onRowClicked: function (e) { if (e.data) satirSecildi(e.data); }
    });

    // Eski ListeleAna
    function listeleAna() {
        if (!dosyaNo) { gridListe.setGridOption('rowData', []); return; }
        gridListe.setGridOption('loading', true);
        Sgb.al(UC + 'liste', { yil: yil, dosyaNo: dosyaNo })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); gridListe.setGridOption('rowData', []); return; }
                gridListe.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { gridListe.setGridOption('loading', false); });
    }

    // Eski SatirSecildi: hdnKod / hdnTasarimKod dolar, satırlar üstte açılır, Sil etkinleşir
    function satirSecildi(d) {
        durum.kod = parseInt(d.KOD, 10) || 0;
        durum.tasarimKod = parseInt(d.TASARIMKODU, 10) || 0;
        $('#lblIslem').text(d.TASARIMADI ? '— ' + d.TASARIMADI : '');
        $('#btnSil').prop('disabled', !durum.kod);
        gridSatir.setGridOption('loading', true);
        Sgb.al(UC + 'satirlar', { kod: durum.kod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); satirlariYaz([]); return; }
                satirlariYaz(v.satirlar);
            })
            .fail(hataGoster)
            .always(function () { gridSatir.setGridOption('loading', false); });
    }

    // Eski Temizle: gizli alanlar boşalır, liste yenilenir, üst tablo boşalır
    function temizle() {
        durum.kod = 0;
        durum.tasarimKod = 0;
        $('#lblIslem').text('');
        $('#btnSil').prop('disabled', true);
        gridListe.deselectAll();
        satirlariYaz([]);
        listeleAna();
    }

    // ---------------------------------------------------------------- Yeni Kayıt: İşlem Seçimi (eski wndHesap)

    var dlgIslem = document.getElementById('dlgIslem');

    // Eski TasarimDoldur: aktif tasarımlar
    function tasarimDoldur() {
        Sgb.al(UC + 'tasarimlar')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var $s = $('#ddlTasarim').empty();
                $('<option value=""></option>').text('Seçiniz').appendTo($s);
                $.each(v.satirlar || [], function (i, t) { $('<option></option>').val(t.KOD).text(t.ADI).appendTo($s); });
            })
            .fail(hataGoster);
    }

    $('#btnYeni').on('click', function () { dlgIslem.showModal(); $('#ddlTasarim').trigger('focus'); });

    // Eski btnSec_Click + ListeleTasarim(false)
    $('#frmIslem').on('submit', function (e) {
        e.preventDefault();
        var tasarimKod = parseInt($('#ddlTasarim').val(), 10) || 0;
        if (!tasarimKod) { mesaj('Lütfen ilk önce, İşlem seçimi yapınız', 'info'); return; }
        var ad = $('#ddlTasarim option:selected').text();
        $('#btnSec').prop('disabled', true);
        gridSatir.setGridOption('loading', true);
        Sgb.al(UC + 'tasarim', { tasarimKod: tasarimKod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                durum.kod = 0;
                durum.tasarimKod = tasarimKod;
                gridListe.deselectAll();
                $('#btnSil').prop('disabled', true);
                $('#lblIslem').text('— ' + ad);
                satirlariYaz(v.satirlar);
                if (v.bilgi) mesaj(v.bilgi, 'info');
                dlgIslem.close();
            })
            .fail(hataGoster)
            .always(function () {
                $('#btnSec').prop('disabled', false);
                gridSatir.setGridOption('loading', false);
            });
    });

    // ---------------------------------------------------------------- Kaydet / Sil

    $('#btnKaydet').on('click', function () {
        if (!dosyaNo) { mesaj('Lütfen hesap hareketi girilecek dosyayı seçiniz.', 'warning'); return; }
        var satirlar = satirlariTopla();
        if (!satirlar.length) { mesaj('Kaydedilecek satır yok. Yeni Kayıt ile işlem seçiniz.', 'warning'); return; }
        sor('Kayıt işlemini yapmak istediğinize emin misiniz?', function () {
            var istek = { yil: yil, dosyaNo: dosyaNo, kod: durum.kod, tasarimKod: durum.tasarimKod, satirlar: satirlar };
            mesgul(true);
            Sgb.gonder(UC + 'kaydet', istek)
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    temizle();
                    mesaj(s.bilgi, 'success');
                })
                .fail(hataGoster)
                .always(function () { mesgul(false); });
        });
    });

    $('#btnSil').on('click', function () {
        if (!durum.kod) { mesaj('Silinecek kayıt seçiniz', 'warning'); return; }
        sor('Kaydı silmek istediğinize emin misiniz?', function () {
            mesgul(true);
            Sgb.gonder(UC + 'sil', { kod: durum.kod })
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    temizle();
                    mesaj(s.bilgi, 'success');
                })
                .fail(hataGoster)
                .always(function () { mesgul(false); });
        });
    });

    // ---------------------------------------------------------------- Kapat (eski hidePopWin)

    $('#btnKapat').on('click', function () {
        try {
            var d = window.frameElement && window.frameElement.closest('dialog');
            if (d) { d.close(); return; }
        } catch (e) { /* başka kökenden açılmış */ }
        if (window.opener) window.close();
        else window.history.back();
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    $('#lblDosya').text(dosyaNo ? 'Dosya: ' + (yil ? yil + ' / ' : '') + ('0000' + dosyaNo).slice(-Math.max(4, String(dosyaNo).length)) : '');
    if (!dosyaNo) $('#btnKaydet').prop('disabled', true);
    tasarimDoldur();
    listeleAna();
});
