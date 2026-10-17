// Ödeme Bilgileri (AlacakTakip/Odeme; eski aynı adlı Ext.NET sayfası). Takip İşlemleri ekranından
// Sgb.pencere('../AlacakTakip/Odeme?yil=2026&dosyaNo=12&paraBirimi=1', 'Ödeme Bilgileri') ile açılır.
//   Ödeme Satırı Aç, Ödeme Planı Hazırla, Kaydet (tablodaki taksitler), Yazdır (Excel), Ana Para Hesapla (pencere), Kapat.
//   Açılış Tahakkuku Oluştur ve satırdaki Tahakkuk Oluştur tuşu hesap kodu penceresini açar (eski wndHesapKodSecim); hesap kodları
//   elle yazılır ya da mercek tuşuyla hesap planından seçilir (Sgb.hesapPlaniSec, makbuzsecim.js).
//   Belge numarasına tıklanınca ödeme emri ButceMuhasebe/OdemeEmrimifFrame sayfasında pencere olarak açılır.
// Uçlar: /api/alacaktakip/odeme/* (AlacakTakip/OdemeUclari.cs). Tabloda beyaz hücreler girilebilir, sarı hücreler salt okunurdur.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/alacaktakip/odeme/';
    var sorgu = new URLSearchParams(window.location.search);
    var dosya = {
        yil: parseInt(sorgu.get('yil'), 10) || 0,
        dosyaNo: parseInt(sorgu.get('dosyaNo'), 10) || 0
    };
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var degisti = false; // tabloda kaydedilmemiş değişiklik var mı

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function tutar(v) { return v === null || v === undefined || v === '' ? '' : para.format(Number(v) || 0); }

    // "1.234,56" ya da "1234.56" -> sayı (boşsa 0, hatalıysa null)
    function sayiCoz(s) {
        if (typeof s === 'number') return s;
        s = $.trim(String(s || '')).replace(/\s/g, '');
        if (s === '') return 0;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = parseFloat(s);
        return isNaN(n) ? null : n;
    }

    // ---------------------------------------------------------------- pencereler, onay sorusu, kapat

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

    // Kapat (eski parentAutoLoadControl.hide()): çerçeve (Sgb.pencere) içindeyse pencere kapanır.
    function kapat() {
        try {
            var d = window.frameElement && window.frameElement.closest('dialog');
            if (d) { d.close(); return; }
        } catch (e) { /* başka kökenden açılmış */ }
        if (window.opener) window.close();
        else window.history.back();
    }
    $('#btnKapat').on('click', kapat);

    // Ödeme emri belgesi (eski OdemeEmriAc: ButceMuhasebe/OdemeEmrimifFrame.aspx yeni pencerede). Bu sayfa zaten bir çerçevede
    // olduğundan, pencere mümkünse en üstteki sayfada açılır (daha geniş yer kalır).
    function odemeEmriAc(yil, belgeNo) {
        if (!belgeNo) { mesaj('Tahakkuk belge numarası okunamadı.', 'warning'); return; }
        var ust = Sgb;
        try {
            if (window.top !== window && window.top.Sgb && window.top.Sgb.pencere) ust = window.top.Sgb;
        } catch (e) { /* başka kökenden açılmış */ }
        ust.pencere('../ButceMuhasebe/OdemeEmrimifFrame?' + $.param({ aramaYok: 1, yil: yil, belgeNo: belgeNo }), 'Ödeme Emri ' + yil + '/' + belgeNo);
    }

    function belgeBag(yil, belgeNo) {
        var a = document.createElement('a');
        a.href = '#';
        a.textContent = yil + '/' + belgeNo;
        a.title = 'Ödeme emri belgesini açar';
        a.addEventListener('click', function (e) { e.preventDefault(); odemeEmriAc(yil, belgeNo); });
        return a;
    }

    // ---------------------------------------------------------------- taksit tablosu

    function sayiSutun(alan, baslik, girilir, genislik) {
        return {
            field: alan, headerName: baslik, width: genislik || 110, type: 'rightAligned', cellDataType: 'number', editable: !!girilir,
            cellClass: [girilir ? 'atmo-girilir' : 'atmo-salt', 'ag-right-aligned-cell'],
            cellEditor: 'agNumberCellEditor', cellEditorParams: { precision: 2 },
            valueFormatter: function (p) { return tutar(p.value); }
        };
    }

    function tarihSutun(alan, baslik) {
        return {
            field: alan, headerName: baslik, width: 130, cellDataType: 'dateString', editable: true, cellClass: 'atmo-girilir',
            cellEditor: 'agDateStringCellEditor',
            valueFormatter: function (p) { return p.value ? Sgb.tarih(p.value) : ''; }
        };
    }

    function belgeSutun(alan, baslik) {
        return {
            field: alan, headerName: baslik, width: 120, sortable: false, cellClass: 'atmo-salt',
            cellRenderer: function (p) { return p.value ? belgeBag(p.data.ODEMEEMRIYIL, p.value) : ''; }
        };
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { headerName: '#', valueGetter: 'node.rowIndex + 1', width: 52, sortable: false, cellClass: 'atmo-salt' },
            {
                field: 'ODENDI', headerName: 'Ödendi', width: 78, sortable: false, cellClass: ['atmo-salt', 'atmo-orta'], cellDataType: false,
                cellRenderer: function (p) { return p.value ? '<span class="glyphicon glyphicon-ok" aria-label="Ödendi"></span>' : ''; }
            },
            sayiSutun('TUTAR', 'Tutar', true, 120),
            sayiSutun('TAKSITFAIZI', 'Taksit Günü Faizi', true, 110),
            sayiSutun('FAIZTUTAR', 'Faiz Tutarı', false, 110),
            sayiSutun('SATIRTUTAR', 'Satır Tutarı', false, 120),
            tarihSutun('ODEMETARIH', 'Son Ödeme Tarihi'),
            tarihSutun('GERCEKODEMETARIH', 'Ödeme Tarihi'),
            { field: 'MAKBUZNO', headerName: 'Makbuz No', width: 110, editable: true, cellClass: 'atmo-girilir', cellDataType: 'text',
                cellEditorParams: { maxLength: 50 } },
            {
                colId: 'tahakkuk', headerName: '', width: 52, sortable: false, resizable: false, cellClass: 'atmo-salt',
                headerTooltip: 'Tahakkuk Oluştur',
                cellRenderer: function (p) {
                    var b = document.createElement('button');
                    b.type = 'button';
                    b.className = 'btn btn-link btn-xs atmo-satir-tus';
                    b.title = 'Tahakkuk Oluştur';
                    b.innerHTML = '<span class="glyphicon glyphicon-edit" aria-hidden="true"></span><span class="sr-only">Tahakkuk Oluştur</span>';
                    b.addEventListener('click', function () { hesapKodSec(2, p.node); });
                    return b;
                }
            },
            belgeSutun('FAIZODEMEEMRINO', 'Faiz Tahakkuku'),
            belgeSutun('ODEMEEMRINO', 'Ödeme Tahakkuku')
        ],
        defaultColDef: { sortable: false, wrapHeaderText: true, autoHeaderHeight: true },
        getRowId: function (p) { return String(p.data.SIRANO); },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        overlayNoRowsTemplate: '<span>Taksit yok. Ödeme Planı Hazırla ya da Ödeme Satırı Aç ile ekleyiniz.</span>',
        onCellValueChanged: function (e) {
            if (e.newValue === e.oldValue) return;
            degisti = true;
            if (e.colDef.field === 'TUTAR' || e.colDef.field === 'TAKSITFAIZI') {
                var d = e.data;
                d.SATIRTUTAR = (Number(d.TUTAR) || 0) + (Number(d.FAIZTUTAR) || 0) + (Number(d.TAKSITFAIZI) || 0);
                e.api.refreshCells({ rowNodes: [e.node], columns: ['SATIRTUTAR'] });
            }
        }
    });

    function satirlar() {
        var r = [];
        grid.forEachNode(function (n) { r.push(n.data); });
        return r;
    }

    // ---------------------------------------------------------------- bilgi (eski Listele)

    var bilgi = {};

    function listele() {
        grid.setGridOption('loading', true);
        return Sgb.al(UC + 'bilgi', dosya)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); grid.setGridOption('rowData', []); return; }
                bilgi = v;
                $('#lblBorcMiktari').text(tutar(v.borcMiktari));
                $('#lblDosyaNo').text(v.dosyaNo);
                var $b = $('#lblOdemeEmriNo').empty();
                if (v.odemeEmriNo) $b.append(belgeBag(v.odemeEmriYil, v.odemeEmriNo));
                else $b.append($('<span class="atmo-soluk"></span>').text('Oluşturulmamış'));
                $.each(v.satirlar || [], function (i, s) {
                    // Boş tarih tabloya boş değer (null) olarak verilir
                    if (!s.ODEMETARIH) s.ODEMETARIH = null;
                    if (!s.GERCEKODEMETARIH) s.GERCEKODEMETARIH = null;
                });
                grid.setGridOption('rowData', v.satirlar || []);
                degisti = false;
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    // İşlem sonucu: başarılıysa bilgi, değilse hata; ardından (eskisi gibi) liste yeniden okunur.
    function sonucGoster(s, varsayilan) {
        if (s.basarili) mesaj(s.bilgi, 'success');
        else mesaj(s.hata || varsayilan, 'danger');
    }

    // ---------------------------------------------------------------- üst tuşlar

    $('#btnTaksit').on('click', function () {
        var $t = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'taksit-ekle', dosya)
            .done(function (s) { sonucGoster(s, 'Taksit eklenemedi.'); listele(); })
            .fail(hataGoster)
            .always(function () { $t.prop('disabled', false); });
    });

    $('#frmPlan').on('submit', function (e) {
        e.preventDefault();
        var $t = $('#btnOdemePlani').prop('disabled', true);
        Sgb.gonder(UC + 'plan', {
            yil: dosya.yil,
            dosyaNo: dosya.dosyaNo,
            taksitSayisi: parseInt($('#txtVade').val(), 10) || 0,
            periyod: parseInt($('#txtPeriyod').val(), 10) || 0,
            ilkOdemeTarihi: $('#txtIlkOdemeTarihi').val() || ''
        })
            .done(function (s) {
                sonucGoster(s, 'Ödeme planı hazırlanamadı.');
                if (s.basarili) $('#txtVade, #txtPeriyod, #txtIlkOdemeTarihi').val('');
                listele();
            })
            .fail(hataGoster)
            .always(function () { $t.prop('disabled', false); });
    });

    $('#btnYardim').on('click', function () {
        var $p = $('#pYardim'), ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false');
    });

    // ---------------------------------------------------------------- kaydet, yazdır

    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        var r = satirlar();
        if (!r.length) { mesaj('Herhangi bir taksit seçilmemiş!', 'warning'); return; }
        var toplam = 0;
        $.each(r, function (i, s) { toplam += Number(s.TUTAR) || 0; });
        if (bilgi.borcMiktari != null && Math.round(toplam * 100) > Math.round(Number(bilgi.borcMiktari) * 100)) {
            mesaj('Taksit tutarları toplamı borç miktarından büyük olamaz', 'warning');
            return;
        }
        sor('Taksit bilgileri kaydedilecektir. Onaylıyor musunuz?', function () {
            var $t = $('#btnKaydet').prop('disabled', true);
            grid.setGridOption('loading', true);
            Sgb.gonder(UC + 'kaydet', {
                yil: dosya.yil,
                dosyaNo: dosya.dosyaNo,
                taksitler: $.map(r, function (s) {
                    return {
                        SIRANO: s.SIRANO,
                        TUTAR: s.TUTAR == null ? 0 : Number(s.TUTAR),
                        TAKSITFAIZI: s.TAKSITFAIZI == null ? 0 : Number(s.TAKSITFAIZI),
                        FAIZTUTAR: s.FAIZTUTAR == null ? 0 : Number(s.FAIZTUTAR),
                        ODEMETARIH: s.ODEMETARIH || '',
                        GERCEKODEMETARIH: s.GERCEKODEMETARIH || '',
                        MAKBUZNO: s.MAKBUZNO || ''
                    };
                })
            })
                .done(function (s) {
                    sonucGoster(s, 'Kayıt işlemi yapılamadı.');
                    // Uyarıda (ör. toplam borcu aşıyor) girilenler kaybolmasın; eskisi gibi yalnızca kayıttan sonra yeniden okunur.
                    if (s.basarili) listele();
                })
                .fail(hataGoster)
                .always(function () { $t.prop('disabled', false); grid.setGridOption('loading', false); });
        });
    });

    $('#btnYazdir').on('click', function () {
        var $t = $(this).prop('disabled', true);
        Sgb.indir(UC + 'yazdir', dosya).then(function () { $t.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- tahakkuk (eski OdemeEmriHesapKodSec / OdemeEmriOlustur)

    var dlgHesapKod = document.getElementById('dlgHesapKod');
    var odemeEmriTur = 0;
    var seciliSatir = null;

    // tur 1: açılış tahakkuku (dosya), tur 2: satırdaki taksit için faiz / ödeme tahakkuku
    function hesapKodSec(tur, node) {
        if (tur === 1) {
            $('#pnlHesapNoOdeme').prop('hidden', true);
            $('#dlgHesapKodBaslik').text('Açılış Tahakkuku Hesap Kodları');
            $('#txtHesapNoBorc').val('140.01.01');
            $('#txtHesapNoAlacak').val('600.05.09.01.06');
            seciliSatir = null;
        } else {
            $('#pnlHesapNoOdeme').prop('hidden', false);
            $('#dlgHesapKodBaslik').text('Faiz/Ödeme Tahakkuku Hesap Kodları');
            $('#txtHesapNoBorc').val('140.01.02');
            $('#txtHesapNoAlacak').val('600.05.01.09.01');
            seciliSatir = node;
            if (degisti) mesaj('Tabloda kaydedilmemiş değişiklik var; tahakkuk kayıtlı taksit bilgilerine göre oluşturulur.', 'warning');
        }
        odemeEmriTur = tur;
        dlgHesapKod.showModal();
    }

    $('#btnOdemeEmri').on('click', function () { hesapKodSec(1); });

    $('#dlgHesapKod').on('click', '[data-hesap-sec]', function () {
        var $k = $($(this).data('hesap-sec'));
        Sgb.hesapPlaniSec({
            uc: UC + 'hesap/', // yetkisi Alacak Takip'ten gelen hesap planı uçları
            yil: bilgi.yil || dosya.yil,
            sonKirilim: true,
            secilince: function (h) { $k.val(h.kod).trigger('focus'); }
        });
    });

    $('#frmHesapKod').on('submit', function (e) {
        e.preventDefault();
        var istek = {
            yil: dosya.yil,
            dosyaNo: dosya.dosyaNo,
            siraNo: 0,
            borcHesap: $.trim($('#txtHesapNoBorc').val()),
            alacakHesap: $.trim($('#txtHesapNoAlacak').val()),
            odemeHesap: ''
        };
        var $t = $('#btnOdemeEmriOlustur').prop('disabled', true);
        var bitti = function () { $t.prop('disabled', false); };

        if (odemeEmriTur === 1) {
            Sgb.gonder(UC + 'acilis-tahakkuk', istek)
                .done(function (s) {
                    sonucGoster(s, 'Tahakkuk oluşturulamadı.');
                    if (s.basarili || s.belgeNo) { dlgHesapKod.close(); listele(); }
                })
                .fail(hataGoster)
                .always(bitti);
            return;
        }

        if (odemeEmriTur === 2 && seciliSatir) {
            istek.siraNo = seciliSatir.data.SIRANO;
            istek.odemeHesap = $.trim($('#txtHesapNoOdeme').val());
            if (!istek.odemeHesap) { mesaj('Ödeme Tahakkuku hesap kodu boş bırakılmaz', 'warning'); bitti(); $('#txtHesapNoOdeme').trigger('focus'); return; }
            var node = seciliSatir;
            Sgb.gonder(UC + 'taksit-tahakkuk', istek)
                .done(function (s) {
                    if (!s.basarili) {
                        mesaj((s.hata || 'Tahakkuk oluşturulamadı.') + (s.satirlar ? ' Satırlar: ' + s.satirlar.split('\n').join('; ') : ''), 'danger');
                        return;
                    }
                    mesaj(s.bilgi, 'success');
                    dlgHesapKod.close();
                    // Eski kod yalnızca satırın belge numaralarını güncelliyordu.
                    var d = $.extend({}, node.data, { ODEMEEMRIYIL: s.ODEMEEMRIYIL, FAIZODEMEEMRINO: s.FAIZODEMEEMRINO, ODEMEEMRINO: s.ODEMEEMRINO });
                    node.setData(d);
                })
                .fail(hataGoster)
                .always(bitti);
            return;
        }
        bitti();
    });

    // ---------------------------------------------------------------- ana paradan faiz hesapla (eski wndHesap)

    var dlgHesap = document.getElementById('dlgHesap');
    $('#btnAnaParaHesapla').on('click', function () {
        dlgHesap.showModal();
        $('#txtHesapOdemeTutari').trigger('focus');
    });

    $('#frmHesap').on('submit', function (e) {
        e.preventDefault();
        var t = $('#txtHesapOdemeTarihi').val();
        if (!t) { mesaj('Lütfen ödeme tarihini giriniz', 'warning'); $('#txtHesapOdemeTarihi').trigger('focus'); return; }
        var odeme = sayiCoz($('#txtHesapOdemeTutari').val());
        if (!odeme || odeme <= 0) { mesaj('Lütfen ödeme tutarını giriniz', 'warning'); $('#txtHesapOdemeTutari').trigger('focus'); return; }
        var $t = $('#btnHesapla').prop('disabled', true);
        $('#frmHesap .atmo-salt').val('');
        Sgb.gonder(UC + 'hesapla', { yil: dosya.yil, dosyaNo: dosya.dosyaNo, odemeTutari: odeme, odemeTarihi: t })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata || 'Hesaplama yapılamadı.', 'danger'); return; }
                $('#txtHesapAnaPara').val(s.anaPara);
                $('#txtHesapFaizTutari').val(s.faizTutari);
                $('#txtHesapFaizOrani').val(s.faizOrani);
                $('#txtTaksitTarihiOrani').val(s.taksitTarihiOrani);
                $('#txtTaksitTarihiFaizi').val(s.taksitTarihiFaizi);
            })
            .fail(hataGoster)
            .always(function () { $t.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açılış

    if (!dosya.yil || !dosya.dosyaNo) {
        mesaj('Yıl ve dosya numarası bilgisi eksik.', 'danger');
        $('.atmo button').not('#btnKapat').prop('disabled', true);
        return;
    }
    $('#lblDosyaNo').text(dosya.dosyaNo);
    listele();
});
