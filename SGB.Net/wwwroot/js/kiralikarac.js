// Kiralık Araç Bilgileri (ButceForm/KiralikAraclarForm): birimlerin araç kiralama sözleşmeleri ve sözleşmelerdeki araçlar.
// Solda yıl ve muhasebe / birim seçilip sözleşmeler listelenir; seçilen sözleşme sağda açılır, kaydedilir ya da silinir.
// Kayıtlı sözleşmeye araç penceresinden araç eklenir / düzeltilir / silinir; pencere kapanınca sözleşme yeniden okunur
// (bedeller araçlardan hesaplanır). Yazdır birimin kiralık araç listesini, Tablo Yazdır hizmet alımı taşıt tablosunu verir (Excel).
// Uçlar: /api/kiralikarac/* (ButceForm/KiralikAracUclari.cs).
$(function () {
    'use strict';

    if (!$('#grdSozlesme').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var U = 'api/kiralikarac/';
    var tanim = { yil: new Date().getFullYear(), tipler: [], yakitTurleri: [], sureBirimleri: [] };
    var acik = null;      // sağda açık sözleşme { yil, sozlesmeKod, saymanlikNo, birimKod }; yeni kayıtta yil = 0
    var aracNo = 0;       // pencerede açık aracın sıra numarası (0: yeni)
    var cinsOnbellek = {};
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function muhasebeOku() { return $.trim($('#txtMuhasebe').val()); }
    function birimOku() { return $.trim($('#txtBirim').val()); }

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş -> 0. Geçersizse null.
    function sayiCoz(metin) {
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function secenekDoldur($s, liste, bosEkle) {
        $s.empty();
        if (bosEkle) $s.append($('<option value="0"></option>'));
        $.each(liste || [], function (i, x) { $s.append($('<option></option>').attr('value', x.kod).text(x.ad)); });
    }

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;

    function sor(baslik, metin, evet, is) {
        $('#dlgSoruBaslik').text(baslik);
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

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- sözleşme listesi (eski grdKASozListe)

    var BOS_LISTE = '<span>Yılı ve muhasebe / birimi seçip Listele\'ye basınız.</span>';

    var listeGrid = Sgb.grid('#grdSozlesme', {
        columnDefs: [
            { field: 'sozlesmeKod', headerName: '#', width: 64 },
            { field: 'imzaTarihi', headerName: 'Söz. Tar', width: 96 },
            { field: 'birimAd', headerName: 'Birim', flex: 1, minWidth: 140, tooltipField: 'birimAd' },
            {
                field: 'sozlesmeBedeliTop', headerName: 'Tutar', width: 110, headerClass: 'sayi', cellClass: 'sayi',
                valueFormatter: function (p) { return p.value == null ? '' : tamSayi.format(p.value); }
            },
            { field: 'firmaAd', headerName: 'Firma', width: 160, tooltipField: 'firmaAd' }
        ],
        defaultColDef: { sortable: true, filter: false, resizable: true },
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        overlayNoRowsTemplate: BOS_LISTE,
        onRowClicked: function (e) { if (e.data) sozlesmeOku(e.data.yil, e.data.sozlesmeKod); }
    });

    // secilecek: { yil, sozlesmeKod } verilirse liste gelince o satır seçili gösterilir.
    function listele(secilecek) {
        var o = { yil: yilOku(), muhasebe: muhasebeOku(), birim: birimOku() };
        if (o.yil <= 0) { mesaj('Yılı giriniz.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        listeGrid.setGridOption('loading', true);
        Sgb.al(U + 'liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                listeGrid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
                listeGrid.setGridOption('rowData', v.satirlar || []);
                if (secilecek) listeGrid.forEachNode(function (n) {
                    if (n.data && n.data.yil === secilecek.yil && n.data.sozlesmeKod === secilecek.sozlesmeKod) {
                        n.setSelected(true);
                        listeGrid.ensureNodeVisible(n);
                    }
                });
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); listeGrid.setGridOption('loading', false); });
    }

    $('#btnListele').on('click', function () { listele(); });

    // Kayıttan sonra sol taraf kaydedilen sözleşmenin yılına ve muhasebe / birimine geçer, liste ona göre yenilenir.
    function solaKayitliyiGetir(yil, sozlesmeKod) {
        if (yil > 0) $('#txtYil').val(yil);
        if (String(acik.saymanlikNo) !== muhasebeOku() || acik.birimKod !== birimOku()) {
            muhBirim.ata({ muhasebe: acik.saymanlikNo, birim: acik.birimKod, birimAd: $('#lblSozBirimAd').text() });
            muhBirim.yenile();
        }
        listele({ yil: yil, sozlesmeKod: sozlesmeKod });
    }

    // Muhasebe - birim (ortak bileşen, muhbirim.js; eski txtBirim_Changed ve ListeMuhasebeBirim penceresi).
    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: yilOku,
        kurumsalGizle: true
    });

    // ---------------------------------------------------------------- sözleşme formu

    function aracSutun(alan, baslik, genislik, sinif) {
        return { field: alan, headerName: baslik, width: genislik, headerClass: sinif || '', cellClass: sinif || '' };
    }

    function paraSutun(alan, baslik, genislik) {
        return {
            field: alan, headerName: baslik, width: genislik, headerClass: 'sayi', cellClass: 'sayi',
            valueFormatter: function (p) { return p.value == null ? '' : para.format(p.value); }
        };
    }

    var aracGrid = Sgb.grid('#grdArac', {
        columnDefs: [
            aracSutun('aracSiraNo', '#', 50, 'orta'),
            {
                colId: 'ac', headerName: '', width: 64, sortable: false, resizable: false, cellClass: 'orta',
                cellRenderer: function () {
                    return '<button type="button" class="btn btn-default btn-xs" title="Aracı aç"><span class="glyphicon glyphicon-pencil" aria-hidden="true"></span> Aç</button>';
                }
            },
            { field: 'tasitCinsiAdTKOD', headerName: 'Taşıt Cinsi', width: 220, tooltipField: 'tasitCinsiAdTKOD' },
            aracSutun('enBuyukYas', 'Yaş', 56, 'orta'),
            aracSutun('koltukSayisi', 'Koltuk Sayısı', 76, 'orta'),
            aracSutun('yakitTuruAd', 'Yakıt Türü', 100, 'orta'),
            aracSutun('soforDahilAd', 'Şoför Dahil', 70, 'orta'),
            aracSutun('yakitDahilAd', 'Yakıt Dahil', 70, 'orta'),
            aracSutun('adet', 'Adet', 60, 'orta'),
            aracSutun('sureAd', 'Süre', 150, 'orta'),
            paraSutun('birimFiyat', 'Birim Fiyat', 110),
            paraSutun('Tutar', 'Tutar', 120),
            { field: 'FaaliyetAd', headerName: 'Kaynak', width: 120 }
        ],
        defaultColDef: { sortable: true, filter: false, resizable: true, wrapHeaderText: true, autoHeaderHeight: true },
        overlayNoRowsTemplate: '<span>Sözleşme seçilmedi.</span>',
        onCellClicked: function (e) {
            if (e.colDef.colId === 'ac' && e.data) aracAc(e.data.aracSiraNo);
        },
        onRowDoubleClicked: function (e) { if (e.data) aracAc(e.data.aracSiraNo); }
    });

    function bedelYaz(v) {
        $('#txtBedelButce').val(v ? tamSayi.format(v.sozlesmeBedeliButce || 0) : '');
        $('#txtBedelDoner').val(v ? tamSayi.format(v.sozlesmeBedeliDoner || 0) : '');
        $('#txtBedelToplam').val(v ? tamSayi.format(v.sozlesmeBedeliTop || 0) : '');
        $('#lblToplam').text(v && v.ToplamTutarBul ? 'Toplam tutar: ' + tamSayi.format(v.ToplamTutarBul) : '');
    }

    function aracListeYaz(v) {
        aracGrid.setGridOption('overlayNoRowsTemplate', acik && acik.yil
            ? '<span>Araç yok. Yeni Araç Ekle ile ekleyiniz.</span>'
            : (acik ? '<span>Araç eklemek için önce sözleşmeyi kaydediniz.</span>' : '<span>Sözleşme seçilmedi.</span>'));
        aracGrid.setGridOption('rowData', v ? (v.kiralikArac || []) : []);
        bedelYaz(v);
    }

    function tuslariAyarla() {
        var kayitli = !!acik && acik.yil > 0;
        $('#frmSozlesme').prop('disabled', !acik);
        $('#btnKaydet').prop('disabled', !acik);
        $('#btnAracEkle, #btnSozlesmeSil').prop('hidden', !kayitli);
    }

    // Eski FormTemizle: yeni kayıt için alanlar boşaltılır.
    function formTemizle() {
        $('#lblSozKod').text(acik ? 'Yeni Kayıt' : '');
        $('#lblSozBirimAd').text('');
        $('#dtImzaTarihi, #txtSozlesmeNo, #dtIsBaslangicTarihi, #dtIsBitisTarihi, #txtFirmaAd, #txtSurecYil, #txtSurecNo').val('');
        $('#txtFirmaAd').data('kod', 0);
        $('#cbIlcelerDahil').prop('checked', false);
        $('#grpIlceler').prop('hidden', true);
        aracListeYaz(null);
    }

    // Eski OkuSozlesme. sadeceListe: araç penceresi kapanınca yalnızca araçlar ve bedeller yenilenir.
    function sozlesmeOku(yil, sozlesmeKod, sadeceListe) {
        if (!yil || !sozlesmeKod) return;
        Sgb.al(U + 'oku', { yil: yil, sozlesmeKod: sozlesmeKod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                if (!sadeceListe) {
                    acik = { yil: v.yil, sozlesmeKod: v.sozlesmeKod, saymanlikNo: v.saymanlikNo, birimKod: v.birimKod };
                    $('#lblSozKod').text(v.kodYazi);
                    $('#lblSozBirimAd').text(v.birimAd);
                    $('#dtImzaTarihi').val(v.imzaTarihi);
                    $('#txtSozlesmeNo').val(v.sozlesmeNo);
                    $('#dtIsBaslangicTarihi').val(v.isBaslangicTarihi);
                    $('#dtIsBitisTarihi').val(v.isBitisTarihi);
                    $('#txtFirmaAd').val(v.firmaAd).data('kod', v.firmaKod || 0);
                    $('#grpIlceler').prop('hidden', !v.ilceSorulur);
                    $('#cbIlcelerDahil').prop('checked', !!v.ilceSorulur && !!v.ilcelerDahil);
                    $('#txtSurecYil').val(v.surecYil ? v.surecYil : '');
                    $('#txtSurecNo').val(v.surecNo);
                    tuslariAyarla();
                }
                aracListeYaz(v);
            })
            .fail(hataGoster);
    }

    // Eski btnYeni_Click: sol taraftaki muhasebe / birim için boş sözleşme.
    $('#btnYeni').on('click', function () {
        var m = muhasebeOku(), b = birimOku();
        if (!m || !b) { mesaj('Lütfen sol taraftan saymanlık birim seçiniz!', 'warning'); return; }
        Sgb.al(U + 'birim', { muhasebe: m, birim: b })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                listeGrid.deselectAll();
                acik = { yil: 0, sozlesmeKod: 0, saymanlikNo: parseInt(m, 10) || 0, birimKod: b };
                formTemizle();
                $('#lblSozBirimAd').text(v.birimAd);
                $('#grpIlceler').prop('hidden', !v.ilceSorulur);
                tuslariAyarla();
                $('#dtImzaTarihi').trigger('focus');
            })
            .fail(hataGoster);
    });

    // Eski FirmaGoster / FirmaBilgisiGoster (ListeFirmaKisi penceresi).
    $('#btnFirmaSec').on('click', function () {
        Sgb.firmaSec({
            adres: U + 'firma/ara',
            muhasebe: acik ? acik.saymanlikNo : muhasebeOku(),
            secilince: function (f) { $('#txtFirmaAd').val(f.ad).data('kod', f.kod || 0); }
        });
    });

    // Eski btnKaydet_Click.
    $('#btnKaydet').on('click', function () {
        if (!acik) return;
        sor('Onay', 'Kaydetmek İstiyor musunuz?', 'Kaydet', function () {
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder(U + 'kaydet', {
                yil: acik.yil, sozlesmeKod: acik.sozlesmeKod, saymanlikNo: acik.saymanlikNo, birimKod: acik.birimKod,
                imzaTarihi: $('#dtImzaTarihi').val(), sozlesmeNo: $.trim($('#txtSozlesmeNo').val()),
                isBaslangicTarihi: $('#dtIsBaslangicTarihi').val(), isBitisTarihi: $('#dtIsBitisTarihi').val(),
                firmaKod: parseInt($('#txtFirmaAd').data('kod'), 10) || 0,
                ilcelerDahil: !$('#grpIlceler').prop('hidden') && $('#cbIlcelerDahil').prop('checked'),
                surecYil: parseInt($('#txtSurecYil').val(), 10) || 0, surecNo: $.trim($('#txtSurecNo').val())
            })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (!s.basarili) return;
                    var yil = s.yil || acik.yil, kod = s.sozlesmeKod || acik.sozlesmeKod;
                    acik.yil = yil;
                    acik.sozlesmeKod = kod;
                    solaKayitliyiGetir(yil, kod);
                    sozlesmeOku(yil, kod);
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', !acik); });
        });
    });

    // Eski btnSozlesmeSil_Click.
    $('#btnSozlesmeSil').on('click', function () {
        if (!acik || !acik.yil) return;
        sor('Onay', 'Ekranda bilgileri gösterilen sözleşme silinecektir. Onaylıyor musunuz?', 'Sil', function () {
            Sgb.gonder(U + 'sil', { yil: acik.yil, sozlesmeKod: acik.sozlesmeKod })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (!s.basarili) return;
                    // Eski FormTemizle: aynı birim için yeni kayıt olarak kalır.
                    acik = { yil: 0, sozlesmeKod: 0, saymanlikNo: acik.saymanlikNo, birimKod: acik.birimKod };
                    var ad = $('#lblSozBirimAd').text(), ilceGizli = $('#grpIlceler').prop('hidden');
                    formTemizle();
                    $('#lblSozBirimAd').text(ad);
                    $('#grpIlceler').prop('hidden', ilceGizli);
                    tuslariAyarla();
                    listele();
                })
                .fail(hataGoster);
        });
    });

    // ---------------------------------------------------------------- araç penceresi (eski wndAracGiris)

    var dlgArac = document.getElementById('dlgArac');

    // Eski cmbTipi_Selected / CinsleriDoldur. Dönen söz, liste dolunca tamamlanır.
    function cinsleriDoldur(tip) {
        var $c = $('#cmbCinsi');
        var d = $.Deferred();
        if (!(tip > 0)) { secenekDoldur($c, [], true); return d.resolve().promise(); }
        if (cinsOnbellek[tip]) { secenekDoldur($c, cinsOnbellek[tip], true); return d.resolve().promise(); }
        $c.empty();
        Sgb.al(U + 'cinsler', { tip: tip })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); d.resolve(); return; }
                cinsOnbellek[tip] = v.cinsler || [];
                secenekDoldur($c, cinsOnbellek[tip], true);
                d.resolve();
            })
            .fail(function (x) { hataGoster(x); d.resolve(); });
        return d.promise();
    }

    $('#cmbTipi').on('change', function () { cinsleriDoldur(parseInt($(this).val(), 10) || 0); });

    // Eski txtTutar_Hesapla: tutar = birim fiyat * süre * adet.
    function tutarYaz() {
        var f = sayiCoz($('#txtBirimFiyat').val()) || 0;
        var t = f * (parseInt($('#sfSure').val(), 10) || 0) * (parseInt($('#sfAdet').val(), 10) || 0);
        $('#txtTutar').val(para.format(t));
    }

    $('#sfAdet, #sfSure, #txtBirimFiyat').on('input change', tutarYaz);
    $('#txtBirimFiyat').on('change', function () {
        var n = sayiCoz($(this).val());
        if (n === null) { mesaj('Geçerli bir sayı yazınız.', 'warning'); return; }
        $(this).val(para.format(n));
    });

    // Eski AracTemizle: varsayılan değerler.
    function aracTemizle() {
        aracNo = 0;
        $('#cmbTipi').val('0');
        secenekDoldur($('#cmbCinsi'), [], true);
        $('#sfEnBuyukYas').val(3);
        $('#sfKoltukSayisi').val(5);
        $('#cmbYakitTuru').prop('selectedIndex', tanim.yakitTurleri.length > 1 ? 1 : 0);
        $('#sfAdet').val(1);
        $('#sfSure').val(1);
        $('#cmbSureBirimi').prop('selectedIndex', tanim.sureBirimleri.length > 1 ? 1 : 0);
        $('#txtBirimFiyat').val('');
        $('#txtTutar').val('');
        $('#cbSoforDahil').prop('checked', true);
        $('#cbYakitDahil').prop('checked', false);
        $('#rdKaynakButce').prop('checked', true);
        $('#btnAracSil').prop('hidden', true);
    }

    function aracYaz(a) {
        aracNo = a.aracSiraNo;
        $('#cmbTipi').val(String(a.tasitTipKodu || 0));
        cinsleriDoldur(a.tasitTipKodu).then(function () { $('#cmbCinsi').val(String(a.tasitCinsi || 0)); });
        $('#sfEnBuyukYas').val(a.enBuyukYas);
        $('#sfKoltukSayisi').val(a.koltukSayisi);
        $('#cmbYakitTuru').val(String(a.yakitTuru));
        $('#cbSoforDahil').prop('checked', !!a.soforDahil);
        $('#cbYakitDahil').prop('checked', !!a.yakitDahil);
        $('#sfAdet').val(a.adet);
        $('#sfSure').val(a.sure);
        $('#cmbSureBirimi').val(String(a.sureBirimi));
        $('#txtBirimFiyat').val(para.format(a.birimFiyat || 0));
        $('#txtTutar').val(para.format(a.Tutar || 0));
        $('#rdKaynakButce').prop('checked', !a.dsMi);
        $('#rdKaynakDS').prop('checked', !!a.dsMi);
        $('#btnAracSil').prop('hidden', false);
    }

    function aracOku(no) {
        return Sgb.al(U + 'arac/oku', { yil: acik.yil, sozlesmeKod: acik.sozlesmeKod, aracSiraNo: no })
            .then(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return false; }
                aracYaz(v);
                return true;
            }, function (x) { hataGoster(x); return false; });
    }

    // Eski grdKiralikArac_Command.
    function aracAc(no) {
        if (!acik || !acik.yil) return;
        aracOku(no).then(function (ok) { if (ok && !dlgArac.open) dlgArac.showModal(); });
    }

    // Eski btnAracEkle_Click.
    $('#btnAracEkle').on('click', function () {
        if (!acik || !acik.yil) return;
        aracTemizle();
        dlgArac.showModal();
        $('#cmbTipi').trigger('focus');
    });

    // Eski btnAracKaydet_Click.
    $('#btnAracKaydet').on('click', function () {
        if (!acik || !acik.yil) return;
        var fiyat = sayiCoz($('#txtBirimFiyat').val());
        if (fiyat === null) { mesaj('Birim fiyat geçerli bir sayı olmalıdır.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(U + 'arac/kaydet', {
            yil: acik.yil, sozlesmeKod: acik.sozlesmeKod, aracSiraNo: aracNo,
            tasitCinsi: parseInt($('#cmbCinsi').val(), 10) || 0,
            enBuyukYas: parseInt($('#sfEnBuyukYas').val(), 10) || 0,
            koltukSayisi: parseInt($('#sfKoltukSayisi').val(), 10) || 0,
            yakitTuru: parseInt($('#cmbYakitTuru').val(), 10) || 0,
            soforDahil: $('#cbSoforDahil').prop('checked'), yakitDahil: $('#cbYakitDahil').prop('checked'),
            adet: parseInt($('#sfAdet').val(), 10) || 0, sure: parseInt($('#sfSure').val(), 10) || 0,
            sureBirimi: parseInt($('#cmbSureBirimi').val(), 10) || 0,
            birimFiyat: fiyat, dsMi: $('#rdKaynakDS').prop('checked')
        })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                if (s.aracSiraNo) aracOku(s.aracSiraNo);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // Eski btnAracSil_Click.
    $('#btnAracSil').on('click', function () {
        if (!acik || !acik.yil || !aracNo) return;
        sor('Onay', 'Ekranda bilgileri gösterilen araç silinecektir. Onaylıyor musunuz?', 'Sil', function () {
            Sgb.gonder(U + 'arac/sil', { yil: acik.yil, sozlesmeKod: acik.sozlesmeKod, aracSiraNo: aracNo })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) aracTemizle();
                })
                .fail(hataGoster);
        });
    });

    // Eski wndAracGiris_BeforeClose: araçlar ve bedeller yenilenir.
    dlgArac.addEventListener('close', function () {
        if (acik && acik.yil) sozlesmeOku(acik.yil, acik.sozlesmeKod, true);
    });

    // ---------------------------------------------------------------- raporlar (eski btnYazdir_Click, btnYazdirOzet_Click)

    function yazdir(tur, $d) {
        var yil = yilOku();
        if (yil <= 0) { mesaj('Yılı giriniz.', 'warning'); return; }
        $d.prop('disabled', true);
        Sgb.indir(U + 'rapor', { yil: yil, muhasebe: muhasebeOku(), birim: birimOku(), tur: tur })
            .then(function () { $d.prop('disabled', false); });
    }

    $('#btnYazdir').on('click', function () { yazdir(1, $(this)); });
    $('#btnYazdirOzet').on('click', function () { yazdir(2, $(this)); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    tuslariAyarla();
    Sgb.al(U + 'tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            Sgb.yilKutusu('#txtYil', [2010, t.yil + 1], t.yil);
            secenekDoldur($('#cmbTipi'), t.tipler, true);
            secenekDoldur($('#cmbYakitTuru'), t.yakitTurleri, false);
            secenekDoldur($('#cmbSureBirimi'), t.sureBirimleri, false);
            muhBirim.ata({ muhasebe: t.muhasebe, birim: t.birim, kurumsal: t.kurumsal, birimAd: t.birimAd });
            if (t.muhasebe && t.birim) listele();
        })
        .fail(hataGoster);
});
