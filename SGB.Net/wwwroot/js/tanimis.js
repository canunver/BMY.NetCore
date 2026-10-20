// İş Tanımları (ButceMuhasebe/TanimIs.aspx): ödeme emri satırlarında seçilen işler burada tanımlanır.
// Solda kıstaslarla iş listesi (Listele: bitiş tarihi bugünden sonra olanlar; Yazdır: işlere göre ödenek bloke durumu, Excel).
// Listeden seçilen iş sağda açılır; Kaydet / Yeni / Sil. Altında dört sekme (eski TanimIsButceFisleri, TanimIsMuhasebeFisleri,
// TanimIsOdenekDurum, TanimIsTaahhutler): Ödenek Bloke (tertip ekle, tutar gir, kaydet), Muhasebe Fişleri, Ödenek Durum, Taahhütler.
// Uçlar: /api/tanimis/* (Muhasebe/TanimIsUclari.cs). Tertip seçimi: Sgb.tertipSec; belge tarihçesi: Sgb.oem.tarihce.
$(function () {
    'use strict';

    if (!$('.tis').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var U = 'api/tanimis/';
    var tanim = { yil: new Date().getFullYear(), birimKod: 0, birimAd: '', fisYetki: false };
    var acik = { kod: 0, orgBirimKod: 0, muhasebeKod: 0, muhasebeBirimKod: '' }; // sağda açık iş (kod 0: yeni kayıt)
    var aktifSekme = 'bloke';
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    // Sunucu hatasında durum kodu ve sunucunun kısa açıklaması da yazılır (sorunun nerede olduğu anlaşılsın).
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        if (xhr && xhr.status === 403) { mesaj('Bu işlem için yetkiniz yok.', 'danger'); return; }
        var ek = '';
        if (xhr && xhr.responseJSON && xhr.responseJSON.hata) ek = xhr.responseJSON.hata;
        else if (xhr && xhr.responseText) ek = (new DOMParser().parseFromString(xhr.responseText, 'text/html').body.textContent || '').replace(/\s+/g, ' ').trim().substring(0, 300);
        mesaj('İşlem yapılamadı' + (xhr && xhr.status ? ' (HTTP ' + xhr.status + ')' : '') + (ek ? ': ' + ek : '.'), 'danger');
    }
    function tutar(p) { return p.value === null || p.value === undefined || p.value === '' ? '' : para.format(p.value); }
    function tarih(p) { return p.value ? Sgb.tarih(p.value) : ''; }

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş -> 0. Geçersizse null.
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function secenekEkle($s, liste) {
        $.each(liste || [], function (i, x) { $s.append($('<option></option>').attr('value', x.kod).text(x.ad)); });
    }

    function sorguYil() { return parseInt($('#txtSorguYil').val(), 10) || tanim.yil; }
    function sekmeYil() { return parseInt($('#txtYil').val(), 10) || tanim.yil; }

    // ---------------------------------------------------------------- pencereler, onay sorusu

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;
    function sor(metin, evet, is) {
        $('#soruMetin').text(metin);
        $('#btnSoruEvet').text(evet);
        soruIsi = is;
        dlgSoru.showModal();
    }
    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        var is = soruIsi;
        soruIsi = null;
        if (is) is();
    });

    $('.tis-yardim-dugme').on('click', function () {
        var $y = $($(this).data('yardim'));
        $y.prop('hidden', !$y.prop('hidden'));
    });

    // ---------------------------------------------------------------- iş listesi (sol)

    var isGrid = Sgb.grid('#grdIs', {
        columnDefs: [
            { field: 'kod', headerName: '#', width: 70, type: 'rightAligned' },
            { field: 'adi', headerName: 'Adı', flex: 1, minWidth: 160, tooltipField: 'adi' }
        ],
        defaultColDef: { sortable: true, resizable: true },
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        overlayNoRowsTemplate: '<span>Kıstasları seçip Listele\'ye basınız.</span>',
        onRowClicked: function (e) { if (e.data) isOku(e.data.kod); }
    });

    function kriter() {
        return {
            yil: sorguYil(),
            eko: $.trim($('#txtSorguEko').val()),
            tip: parseInt($('#ddlSorguTip').val(), 10) || 0,
            durum: parseInt($('#ddlSorguDurum').val(), 10) || 0
        };
    }

    // secilecek: liste gelince seçili gösterilecek iş kodu.
    function listele(secilecek) {
        var $d = $('#btnListele').prop('disabled', true);
        isGrid.setGridOption('loading', true);
        Sgb.al(U + 'liste', kriter())
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var s = v.satirlar || [];
                isGrid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun iş bulunamadı.</span>');
                isGrid.setGridOption('rowData', s);
                $('#lblIsSayi').text(s.length ? '(' + s.length + ')' : '');
                if (secilecek) isGrid.forEachNode(function (n) {
                    if (n.data && n.data.kod === secilecek) { n.setSelected(true); isGrid.ensureNodeVisible(n); }
                });
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); isGrid.setGridOption('loading', false); });
    }

    $('#btnListele').on('click', function () { listele(acik.kod); });
    $('#frmSorgu').on('keydown', 'input', function (e) { if (e.key === 'Enter') { e.preventDefault(); listele(acik.kod); } });

    $('#btnSorguTemizle').on('click', function () {
        $('#txtSorguYil').val(tanim.yil);
        $('#ddlSorguTip, #ddlSorguDurum').val('');
        $('#txtSorguEko').val('');
        isGrid.setGridOption('overlayNoRowsTemplate', '<span>Kıstasları seçip Listele\'ye basınız.</span>');
        isGrid.setGridOption('rowData', []);
        $('#lblIsSayi').text('');
    });

    // Eski btnYazdir: işlere göre ödenek bloke durumu (MUH-BlokeOdenek.xlt).
    $('#btnYazdir').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.indir(U + 'liste/yazdir', kriter()).finally(function () { $d.prop('disabled', false); });
    });

    // Ekonomik kod ağacı (eski KodAgac.aspx?kodTur=4).
    var dlgEko = document.getElementById('dlgEko');
    var ekoAgac = null, ekoYil = 0;
    $('#btnEkoSec').on('click', function () {
        var y = sorguYil();
        if (!ekoAgac) {
            ekoYil = y;
            ekoAgac = Sgb.agac('#agcEko', {
                yukle: function (ust) {
                    return Sgb.al('api/tertipsec/kodlar', { yil: ekoYil, tur: 4, ust: ust ? ust.kod : '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler || [];
                    });
                },
                secilince: function (d) { $('#txtSorguEko').val(d.kod); dlgEko.close(); },
                bosMetin: 'Ekonomik kod bulunamadı.',
                hata: hataGoster
            });
        } else if (ekoYil !== y) {
            ekoYil = y;
            ekoAgac.yenile();
        }
        dlgEko.showModal();
    });
    $('#btnEkoTemizle').on('click', function () { $('#txtSorguEko').val(''); dlgEko.close(); });

    // ---------------------------------------------------------------- iş bilgileri (sağ)

    function birimYaz(kod, ad) {
        acik.orgBirimKod = kod || 0;
        $('#txtBirimAd').val(kod ? ad : '');
    }

    function formTemizle() {
        acik = { kod: 0, orgBirimKod: 0, muhasebeKod: 0, muhasebeBirimKod: '' };
        $('#lblIsKod').text('(Yeni Kayıt)');
        birimYaz(tanim.birimKod, tanim.birimAd);
        $('#txtAdi, #txtBaslangic, #txtBitis, #txtSorumlu, #txtAmac, #txtHedef, #txtGorev').val('');
        $('#ddlTip, #ddlDurum').val('0');
        $('#lblSorumluAd').text('');
        $('#btnSil').prop('disabled', true);
        isGrid.deselectAll();
        sekmeleriBosalt();
    }

    function isOku(kod) {
        if (!kod) return;
        Sgb.al(U + 'oku', { kod: kod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = { kod: v.kod, orgBirimKod: v.orgBirimKod, muhasebeKod: v.muhasebeKod, muhasebeBirimKod: v.muhasebeBirimKod };
                $('#lblIsKod').text('(#' + v.kod + (v.muhasebeKod || v.muhasebeBirimKod ? ', muhasebe birimi: ' + v.muhasebeKod + '-' + v.muhasebeBirimKod : ', birimin muhasebe birimi tanımlı değil') + ')');
                birimYaz(v.orgBirimKod, v.orgBirimAd);
                $('#txtAdi').val(v.adi);
                $('#ddlTip').val(String(v.tip || 0));
                $('#txtBaslangic').val(v.baslangicTarihi);
                $('#txtBitis').val(v.bitisTarihi);
                $('#txtSorumlu').val(v.sorumluKod);
                $('#lblSorumluAd').text(v.sorumluAd);
                $('#txtAmac').val(v.amac);
                $('#txtHedef').val(v.hedef);
                $('#txtGorev').val(v.gorevTanimi);
                $('#ddlDurum').val(String(v.durum || 0));
                $('#btnSil').prop('disabled', false);
                sekmeYukle();
            })
            .fail(hataGoster);
    }

    $('#btnYeni').on('click', function () { formTemizle(); $('#txtAdi').trigger('focus'); });

    $('#btnKaydet').on('click', function () {
        var o = {
            Kod: acik.kod,
            OrgBirimKod: acik.orgBirimKod,
            Adi: $.trim($('#txtAdi').val()),
            Tip: parseInt($('#ddlTip').val(), 10) || 0,
            BaslangicTarihi: $('#txtBaslangic').val(),
            BitisTarihi: $('#txtBitis').val(),
            Amac: $('#txtAmac').val(),
            Hedef: $('#txtHedef').val(),
            SorumluKod: $.trim($('#txtSorumlu').val()),
            GorevTanimi: $('#txtGorev').val(),
            Durum: parseInt($('#ddlDurum').val(), 10) || 0
        };
        if (!o.OrgBirimKod) { mesaj('Birimi seçiniz.', 'warning'); return; }
        if (!o.Adi) { mesaj('İşin adını yazınız.', 'warning'); $('#txtAdi').trigger('focus'); return; }
        if (o.BaslangicTarihi && o.BitisTarihi && o.BitisTarihi < o.BaslangicTarihi) {
            mesaj('Bitiş tarihi başlangıç tarihinden önce olamaz.', 'warning');
            return;
        }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(U + 'kaydet', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
                tanim.birimKod = o.OrgBirimKod;
                tanim.birimAd = $('#txtBirimAd').val();
                if (v.kod > 0) { isOku(v.kod); listele(v.kod); }
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnSil').on('click', function () {
        if (!acik.kod) return;
        var kod = acik.kod;
        sor('"' + $('#txtAdi').val() + '" işi silinsin mi?', 'Sil', function () {
            Sgb.gonder(U + 'sil', { Kod: kod })
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    mesaj(v.bilgi, 'success');
                    formTemizle();
                    listele();
                })
                .fail(hataGoster);
        });
    });

    // Birim seçimi (eski organizasyon ağacı): yetkili birimlerden başlar; adda arama düz liste verir.
    var dlgBirim = document.getElementById('dlgBirim');
    var birimAgac = null, birimAra = '';
    function birimAgaciKur() {
        birimAgac = Sgb.agac('#agcBirim', {
            yukle: function (ust) {
                return Sgb.al(U + 'orgagac', ust ? { ust: ust.kod } : { ust: 0, ara: birimAra }).then(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                    return v.satirlar || [];
                });
            },
            secilince: function (d) { birimYaz(d.kod, d.birimAd || d.ad); dlgBirim.close(); },
            ilkAcik: true,
            bosMetin: 'Birim bulunamadı.',
            hata: hataGoster
        });
    }
    $('#btnBirimSec').on('click', function () {
        if (!birimAgac) birimAgaciKur();
        dlgBirim.showModal();
    });
    function birimAramaYap() {
        var a = $.trim($('#txtBirimAra').val());
        if (a && a.length < 3) { mesaj('Aramak için en az 3 harf yazınız.', 'warning'); return; }
        birimAra = a;
        birimAgac.yenile();
    }
    $('#btnBirimAra').on('click', birimAramaYap);
    $('#txtBirimAra').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); birimAramaYap(); } });
    $('#btnBirimSil').on('click', function () { birimYaz(0, ''); });

    // Sorumlu seçimi (eski ListeKullanici / personel listesi).
    var dlgSorumlu = document.getElementById('dlgSorumlu');
    var sorumluGrid = Sgb.grid('#grdSorumlu', {
        columnDefs: [
            { field: 'tc', headerName: 'T.C. Kimlik No', width: 120 },
            { field: 'ad', headerName: 'Ad', width: 130 },
            { field: 'soyad', headerName: 'Soyad', width: 130 },
            { field: 'birim', headerName: 'Birim', flex: 1, minWidth: 160, tooltipField: 'birim' }
        ],
        defaultColDef: { sortable: true, resizable: true },
        overlayNoRowsTemplate: '<span>Ad, soyad ya da T.C. kimlik no yazıp Ara\'ya basınız. Seçmek için satıra tıklayınız.</span>',
        onRowClicked: function (e) {
            if (!e.data) return;
            $('#txtSorumlu').val(e.data.tc);
            $('#lblSorumluAd').text($.trim(e.data.ad + ' ' + e.data.soyad));
            dlgSorumlu.close();
        }
    });
    $('#btnSorumluSec').on('click', function () { dlgSorumlu.showModal(); $('#txtSorAd').trigger('focus'); });
    $('#frmSorumlu').on('submit', function () {
        var o = { ad: $.trim($('#txtSorAd').val()), soyad: $.trim($('#txtSorSoyad').val()), tc: $.trim($('#txtSorTc').val()) };
        if (!o.ad && !o.soyad && !o.tc) { mesaj('Ad, soyad ya da T.C. kimlik nodan en az birini yazınız.', 'warning'); return false; }
        sorumluGrid.setGridOption('loading', true);
        Sgb.al(U + 'sorumlu', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                sorumluGrid.setGridOption('overlayNoRowsTemplate', '<span>Kişi bulunamadı.</span>');
                sorumluGrid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { sorumluGrid.setGridOption('loading', false); });
        return false;
    });
    $('#txtSorumlu').on('input', function () { $('#lblSorumluAd').text(''); });

    // ---------------------------------------------------------------- sekmeler

    var BOS_SEKME = '<span>Listeden bir iş seçiniz.</span>';

    function silHucre() {
        return '<button type="button" class="btn btn-link btn-xs tis-sil" title="Satırı sil" aria-label="Satırı sil">'
            + '<span class="glyphicon glyphicon-remove" aria-hidden="true"></span></button>';
    }

    function toplamYaz(grid, alanlar, etiketAlan) {
        var t = {};
        t[etiketAlan] = 'Toplam';
        $.each(alanlar, function (i, a) { t[a] = 0; });
        var sayi = 0;
        grid.forEachNode(function (n) {
            sayi++;
            $.each(alanlar, function (i, a) { t[a] += Number(n.data[a]) || 0; });
        });
        grid.setGridOption('pinnedBottomRowData', sayi ? [t] : []);
    }

    // Ödenek Bloke (eski TanimIsButceFisleri): tertip ve bloke tutarı; tutar girilir (beyaz), tertip salt okunur (sarı).
    var blokeGrid = Sgb.grid('#grdBloke', {
        columnDefs: [
            { colId: 'sil', headerName: '', width: 40, sortable: false, resizable: false, cellRenderer: silHucre },
            { field: 'tertip', headerName: 'Tertip', flex: 1, minWidth: 260, cellClass: 'tis-sari', tooltipField: 'tertip' },
            {
                field: 'blokeTutar', headerName: 'Bloke Tutarı', width: 150, type: 'rightAligned', editable: true,
                cellClass: ['tis-girilir', 'ag-right-aligned-cell'], valueFormatter: tutar,
                valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : Math.round(n * 100) / 100; }
            }
        ],
        defaultColDef: { sortable: false, resizable: true },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        overlayNoRowsTemplate: BOS_SEKME,
        onCellClicked: function (e) {
            if (e.colDef.colId === 'sil' && e.data) { blokeGrid.applyTransaction({ remove: [e.data] }); blokeToplam(); }
        },
        onCellValueChanged: function () { blokeToplam(); }
    });

    function blokeToplam() {
        var t = 0, s = 0;
        blokeGrid.forEachNode(function (n) { s++; t += Number(n.data.blokeTutar) || 0; });
        $('#lblBlokeToplam').text(s ? s + ' tertip, toplam ' + para.format(t) : '');
    }

    $('#btnTertipEkle').on('click', function () {
        if (!acik.kod) { mesaj('Önce işi seçiniz.', 'warning'); return; }
        Sgb.tertipSec({
            tur: 'muh', yil: sekmeYil(), muhasebe: acik.muhasebeKod, birim: acik.muhasebeBirimKod, onay: true, coklu: true,
            secilince: function (satirlar) {
                var varOlan = {};
                blokeGrid.forEachNode(function (n) { varOlan[n.data.tertip] = true; });
                var ekle = [];
                $.each(satirlar, function (i, t) {
                    if (!t.TERTIP || varOlan[t.TERTIP]) return;
                    varOlan[t.TERTIP] = true;
                    ekle.push({ tertip: t.TERTIP, blokeTutar: 0 });
                });
                if (ekle.length) {
                    blokeGrid.setGridOption('overlayNoRowsTemplate', BOS_SEKME);
                    blokeGrid.applyTransaction({ add: ekle });
                    blokeToplam();
                }
            }
        });
    });

    $('#btnBlokeKaydet').on('click', function () {
        if (!acik.kod) { mesaj('Önce işi seçiniz.', 'warning'); return; }
        blokeGrid.stopEditing();
        var satirlar = [];
        blokeGrid.forEachNode(function (n) { satirlar.push({ tertip: n.data.tertip, blokeTutar: Number(n.data.blokeTutar) || 0 }); });
        var $d = $(this);
        function kaydet() {
            $d.prop('disabled', true);
            Sgb.gonder(U + 'odenek/kaydet', { Yil: sekmeYil(), IsKod: acik.kod, Satirlar: satirlar })
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    mesaj('Ödenek blokeleri kaydedildi.', 'success');
                    sekmeYukle(true);
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        }
        // Boş liste kaydedilirse işin o yıldaki bütün blokeleri silinir (eski davranış); önce sorulur.
        if (!satirlar.length) sor('Listede tertip yok. İşin ' + sekmeYil() + ' yılındaki bütün ödenek blokeleri silinsin mi?', 'Sil', kaydet);
        else kaydet();
    });

    // Muhasebe Fişleri (eski TanimIsMuhasebeFisleri): işe bağlı belgeler. Belge No belgeyi, Durum tarihçeyi açar.
    function bagHucre(p) {
        if (p.node.rowPinned || p.value === null || p.value === undefined || p.value === '') return p.value || '';
        return $('<a href="#"></a>').text(p.value)[0];
    }
    function sutun(alan, baslik, genislik, ek) {
        return $.extend({ field: alan, headerName: baslik, width: genislik, tooltipField: alan }, ek || {});
    }
    var fisGrid = Sgb.grid('#grdFis', {
        columnDefs: [
            {
                colId: 'satir', headerName: '', width: 40, sortable: false, resizable: false,
                cellRenderer: function (p) {
                    return p.node.rowPinned ? '' : '<button type="button" class="btn btn-link btn-xs" title="Belge satırları" aria-label="Belge satırları">'
                        + '<span class="glyphicon glyphicon-list" aria-hidden="true"></span></button>';
                }
            },
            sutun('belgeNo', 'Belge No', 110, { cellRenderer: bagHucre }),
            sutun('yevmiyeNo', 'Yevmiye No', 90),
            sutun('belgeTur', 'Tür', 56),
            sutun('belgeTarih', 'Belge Tarihi', 96, { valueFormatter: tarih }),
            sutun('muhasebe', 'Muhasebe', 180),
            sutun('birim', 'Birim', 180),
            sutun('borc', 'Borç', 120, { type: 'rightAligned', valueFormatter: tutar, tooltipField: null }),
            sutun('alacak', 'Alacak', 120, { type: 'rightAligned', valueFormatter: tutar, tooltipField: null }),
            sutun('aciklama', 'Açıklama', 240),
            sutun('ilgili', 'İlgili', 160),
            sutun('yevmiyeTarih', 'Yevmiye Tarihi', 100, { valueFormatter: tarih }),
            sutun('durum', 'Durum', 130, { cellRenderer: bagHucre }),
            sutun('durumTarih', 'Durum Tarihi', 96, { valueFormatter: tarih }),
            sutun('islemYapan', 'İşlem Yapan', 130),
            sutun('OEBNo', 'ÖEB No', 90),
            sutun('OEBTarih', 'ÖEB Tarihi', 96, { valueFormatter: tarih }),
            sutun('cekNo', 'Çek No', 90),
            sutun('cekTarih', 'Çek Tarihi', 96, { valueFormatter: tarih }),
            sutun('surecNo', 'Süreç No', 90),
            sutun('ihaleMadde', 'İhale Madde', 100),
            sutun('ist1Ad', 'İstatistik Kod 1', 140),
            sutun('ist2Ad', 'İstatistik Kod 2', 140),
            sutun('ekliDosya', 'Ekli Dosya', 80)
        ],
        defaultColDef: { sortable: true, resizable: true },
        overlayNoRowsTemplate: BOS_SEKME,
        onCellClicked: function (e) {
            if (!e.data || e.node.rowPinned) return;
            if (e.colDef.colId === 'satir') fisDetay(e.data);
            else if (e.colDef.field === 'belgeNo') {
                if (e.event) e.event.preventDefault();
                Sgb.pencere('../ButceMuhasebe/OdemeEmriMIFFrame?' + $.param({ aramaYok: 1, yil: e.data.yil, belgeNo: e.data.belgeNo }),
                    'Belge ' + e.data.belgeNo);
            } else if (e.colDef.field === 'durum') {
                if (e.event) e.event.preventDefault();
                Sgb.oem.tarihce({ yil: e.data.yil, belgeNo: e.data.belgeNo });
            }
        }
    });

    var dlgFisDetay = document.getElementById('dlgFisDetay');
    var fisDetayGrid = Sgb.grid('#grdFisDetay', {
        columnDefs: [
            sutun('hesapKod', 'Hesap Kodu', 140),
            sutun('tertip', 'Tertip', 260, { flex: 1 }),
            sutun('borc', 'Borç', 120, { type: 'rightAligned', valueFormatter: tutar, tooltipField: null }),
            sutun('alacak', 'Alacak', 120, { type: 'rightAligned', valueFormatter: tutar, tooltipField: null })
        ],
        defaultColDef: { sortable: true, resizable: true },
        overlayNoRowsTemplate: '<span>Belgede satır yok.</span>'
    });
    function fisDetay(r) {
        $('#dlgFisDetayBaslik').text('Belge Satırları: ' + r.belgeNo);
        fisDetayGrid.setGridOption('rowData', []);
        dlgFisDetay.showModal();
        fisDetayGrid.setGridOption('loading', true);
        Sgb.al(U + 'fisler/detay', { yil: r.yil, belgeNo: r.belgeNo })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                fisDetayGrid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { fisDetayGrid.setGridOption('loading', false); });
    }

    // Ödenek Durum (eski TanimIsOdenekDurum).
    var durumGrid = Sgb.grid('#grdDurum', {
        columnDefs: [
            sutun('tertip', 'Tertip', 300, { flex: 1 }),
            sutun('blokeTutar', 'Bloke Tutarı', 150, { type: 'rightAligned', valueFormatter: tutar, tooltipField: null })
        ],
        defaultColDef: { sortable: true, resizable: true },
        overlayNoRowsTemplate: BOS_SEKME
    });

    // Taahhütler (eski TanimIsTaahhutler).
    var taahhutGrid = Sgb.grid('#grdTaahhut', {
        columnDefs: [
            sutun('kod', 'Kart No', 90, { type: 'rightAligned' }),
            sutun('muteahhit', 'Müteahhit', 260, { flex: 1 }),
            sutun('sozlesmeTarih', 'Sözleşme Tarihi', 110, { valueFormatter: tarih }),
            sutun('sozlesmeTutar', 'Sözleşme Tutarı', 140, { type: 'rightAligned', valueFormatter: tutar, tooltipField: null }),
            sutun('kalanBakiye', 'Kalan Bakiye', 130, { type: 'rightAligned', valueFormatter: tutar, tooltipField: null })
        ],
        defaultColDef: { sortable: true, resizable: true },
        overlayNoRowsTemplate: BOS_SEKME
    });

    var SEKMELER = {
        bloke: { grid: blokeGrid, uc: 'odenek', bos: 'Bu yıl için bloke yok. Tertip Ekle ile ekleyiniz.' },
        fis: { grid: fisGrid, uc: 'fisler', bos: 'Bu işe bağlı belge bulunamadı.', toplam: ['borc', 'alacak'], etiket: 'muhasebe' },
        durum: { grid: durumGrid, uc: 'odenekdurum', bos: 'Ödenek durumu bulunamadı.' },
        taahhut: { grid: taahhutGrid, uc: 'taahhut', bos: 'Bu işe bağlı taahhüt bulunamadı.' }
    };

    function sekmeleriBosalt() {
        $.each(SEKMELER, function (ad, s) {
            s.grid.setGridOption('overlayNoRowsTemplate', BOS_SEKME);
            s.grid.setGridOption('pinnedBottomRowData', []);
            s.grid.setGridOption('rowData', []);
            s.yuklu = null;
        });
        blokeToplam();
    }

    // Açık sekmeyi açık iş ve sekme yılı için okur (aynısı okunmuşsa yeniden okumaz; zorla: Listele).
    function sekmeYukle(zorla) {
        var s = SEKMELER[aktifSekme];
        if (!acik.kod) return;
        var anahtar = acik.kod + '|' + sekmeYil();
        if (!zorla && s.yuklu === anahtar) return;
        s.yuklu = anahtar;
        s.grid.setGridOption('loading', true);
        Sgb.al(U + s.uc, { yil: sekmeYil(), isKod: acik.kod, muh: acik.muhasebeKod || 0, muhBir: acik.muhasebeBirimKod })
            .done(function (v) {
                if (v.hata) { s.yuklu = null; mesaj(v.hata, 'danger'); return; }
                s.grid.setGridOption('overlayNoRowsTemplate', '<span>' + s.bos + '</span>');
                s.grid.setGridOption('rowData', v.satirlar || []);
                if (s.toplam) toplamYaz(s.grid, s.toplam, s.etiket);
                if (s === SEKMELER.bloke) blokeToplam();
            })
            .fail(function (xhr) { s.yuklu = null; hataGoster(xhr); })
            .always(function () { s.grid.setGridOption('loading', false); });
    }

    $('.tis-sekmeler').on('click', 'a[data-sekme]', function (e) {
        e.preventDefault();
        aktifSekme = $(this).data('sekme');
        $('.tis-sekmeler li').removeClass('active');
        $(this).parent().addClass('active');
        $('.tis-sekme').each(function () { $(this).prop('hidden', $(this).data('sekme') !== aktifSekme); });
        sekmeYukle();
    });
    $('#btnSekmeListele').on('click', function () {
        if (!acik.kod) { mesaj('Önce listeden bir iş seçiniz.', 'warning'); return; }
        sekmeYukle(true);
    });
    $('#txtYil').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); sekmeYukle(true); } });

    // ---------------------------------------------------------------- açılış

    Sgb.al(U + 'tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            tanim = v;
            Sgb.yilKutusu('#txtSorguYil', v.yillar, v.yil);
            Sgb.yilKutusu('#txtYil', v.yillar, v.yil);
            secenekEkle($('#ddlSorguTip'), v.tipler);
            secenekEkle($('#ddlTip'), v.tipler);
            secenekEkle($('#ddlSorguDurum'), v.durumlar);
            secenekEkle($('#ddlDurum'), v.durumlar);
            // Ödeme emri görme yetkisi olmayana Muhasebe Fişleri sekmesi gösterilmez.
            $('.tis-sekmeler a[data-sekme="fis"]').parent().prop('hidden', !v.fisYetki);
            formTemizle();
        })
        .fail(hataGoster);
});
