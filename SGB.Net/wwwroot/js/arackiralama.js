// Kiralık Taşıt Planlama Formu (ButceForm/AracKiralamaPlaniForm): seçilen yıl ve muhasebe / birim için kiralanan taşıtlar.
// Her satırda bütçe kaynağı (genel bütçe / döner sermaye), taşıt kodu ve önceki yıl, bu yıl, sonraki yıl için adet, ortalama
// kullanım süresi (ay) ve aylık kira bedeli girilir; maliyet sütunları adet * süre * bedel olarak hesaplanır. Oku ile okunan form
// kaydedilir, onaylanır (onaylı form değiştirilemez); onayı yalnızca sistem yöneticisi kaldırır. Yazdır seçili birimin, Kurum
// Yazdır ve Detay Yazdır (sistem yöneticisi) bütün birimlerin Excel raporunu verir; Onay İşlemleri birimlerin durumunu listeler.
$(function () {
    'use strict';

    if (!$('#grdPlan').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var DURUM_AD = { 0: 'Yok', 1: 'Yeni', 5: 'Onaylı' };
    var simdi = new Date().getFullYear();
    var tanim = { kaynaklar: [], tasitlar: [], sistemYoneticisi: false, yil: simdi, ilkYil: 2023, sonYil: simdi + 1 };
    var acik = null;          // okunmuş form { yil, muhasebe, birim, durum }: kaydet / onayla bunu kullanır
    var degisti = false;      // okunduktan sonra tabloda değişiklik yapıldı mı
    var sayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 2 });
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function muhasebeOku() { return $.trim($('#txtMuhasebe').val()); }
    function birimOku() { return $.trim($('#txtBirim').val()); }

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function adBul(liste, kod) {
        var s = $.grep(liste, function (x) { return String(x.kod) === String(kod); })[0];
        return s ? s.ad : (kod == null || kod === '' || kod === 0 ? '' : String(kod));
    }

    // ---------------------------------------------------------------- plan tablosu

    function girilir(p) { return !!acik && acik.durum !== 5 && !p.node.rowPinned; }

    function hucreSinif(ek) {
        return function (p) {
            var s = ek ? [ek] : [];
            if (p.colDef.type === 'sayi') s.push('sayi');
            if (p.colDef.editable && girilir(p)) s.push('girilir');
            return s;
        };
    }

    // tam: süre gibi tam sayı alanlar; para: kira bedeli (iki ondalık).
    function sayiSutun(alan, baslik, o) {
        o = o || {};
        return {
            field: alan, headerName: baslik, type: 'sayi', width: o.para ? 120 : 96, headerClass: 'sayi',
            editable: girilir, cellClass: hucreSinif(), cellEditor: 'agTextCellEditor',
            valueFormatter: function (p) {
                if (p.value == null || p.value === '') return '';
                return o.para ? para.format(p.value) : sayi.format(p.value);
            },
            valueParser: function (p) {
                var n = sayiCoz(p.newValue);
                if (n === null) { mesaj('Geçerli bir sayı yazınız.', 'warning'); return p.oldValue; }
                return o.tam ? Math.round(n) : n;
            }
        };
    }

    function maliyet(onek, sinif, baslik) {
        return {
            colId: onek + 'Maliyet', headerName: baslik, type: 'sayi', width: 130, headerClass: 'sayi', cellClass: hucreSinif(sinif),
            valueGetter: function (p) {
                if (!p.data) return null;
                if (p.node.rowPinned) return p.data[onek + 'Maliyet'];
                return (Number(p.data[onek + 'Kiralanan']) || 0) * (Number(p.data[onek + 'KiralananSure']) || 0) * (Number(p.data[onek + 'Fiyat']) || 0);
            },
            valueFormatter: function (p) { return p.value == null ? '' : para.format(p.value); }
        };
    }

    function secimSutun(alan, baslik, liste, genislik) {
        return {
            field: alan, headerName: baslik, width: genislik, editable: girilir, cellClass: hucreSinif(),
            cellEditor: 'agSelectCellEditor',
            cellEditorParams: function () { return { values: $.map(liste(), function (x) { return x.kod; }) }; },
            valueFormatter: function (p) { return p.node.rowPinned ? (p.value || '') : adBul(liste(), p.value); },
            tooltipValueGetter: function (p) { return p.node.rowPinned ? '' : adBul(liste(), p.value); },
            valueParser: function (p) { return p.newValue; }
        };
    }

    // Eski BaslikAyarla: sütun başlıklarında yıllar.
    function sutunlar(yil) {
        return [
            secimSutun('butceKaynagi', 'Bütçe Kaynağı', function () { return tanim.kaynaklar; }, 140),
            secimSutun('tasitKodu', 'Taşıt Kodu', function () { return tanim.tasitlar; }, 360),
            { headerName: (yil - 1) + ' Yılı', headerClass: 'yil-onc', children: [
                sayiSutun('oncYilKiralanan', 'Kiralanan (adet)'),
                sayiSutun('oncYilKiralananSure', '1 Aracın Ort. Kullanım Süresi (Ay)', { tam: true }),
                sayiSutun('oncYilFiyat', '1 Aracın Aylık Ortalama Kira Bedeli', { para: true }),
                maliyet('oncYil', 'maliyet-onc', (yil - 1) + ' Yılı Maliyeti (TL)')
            ] },
            { headerName: yil + ' Yılı', headerClass: 'yil-bu', children: [
                sayiSutun('buYilKiralanan', 'Kiralanan (adet)'),
                sayiSutun('buYilKiralananSure', '1 Aracın Ort. Kullanım Süresi (Ay)', { tam: true }),
                sayiSutun('buYilFiyat', '1 Aracın Aylık Ortalama Kira Bedeli (KDV Dahil)', { para: true }),
                maliyet('buYil', 'maliyet-bu', yil + ' Yılı Maliyeti')
            ] },
            { headerName: (yil + 1) + ' Yılı', headerClass: 'yil-gel', children: [
                sayiSutun('gelecekYilKiralanacak', 'Kiralanacak (adet)'),
                sayiSutun('gelecekYilKiralanacakSure', '1 Aracın Ort. Kullanım Süresi (Ay)', { tam: true })
            ] }
        ];
    }

    var BOS_IPUCU = '<span>Yılı ve muhasebe / birimi seçip Oku\'ya basınız.</span>';

    var grid = Sgb.grid('#grdPlan', {
        columnDefs: sutunlar(tanim.yil),
        defaultColDef: { sortable: false, filter: false, resizable: true, wrapHeaderText: true, autoHeaderHeight: true },
        defaultColGroupDef: { wrapHeaderText: true, autoHeaderHeight: true },
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        tooltipShowDelay: 500,
        rowSelection: { mode: 'multiRow', checkboxes: function (p) { return !p.node.rowPinned; }, headerCheckbox: true, enableClickSelection: false },
        overlayNoRowsTemplate: BOS_IPUCU,
        onCellValueChanged: function (e) {
            if (e.node.rowPinned || e.newValue === e.oldValue) return;
            degisti = true;
            grid.refreshCells({ rowNodes: [e.node], force: true });
            toplamYaz();
        }
    });

    function satirlar() {
        var l = [];
        grid.forEachNode(function (n) { if (!n.rowPinned && n.data) l.push(n.data); });
        return l;
    }

    // Alttaki toplam satırı: adetler ve maliyetler.
    function toplamYaz() {
        var l = satirlar();
        if (!l.length) { grid.setGridOption('pinnedBottomRowData', []); return; }
        var t = { butceKaynagi: 'T O P L A M', oncYilKiralanan: 0, buYilKiralanan: 0, gelecekYilKiralanacak: 0, oncYilMaliyet: 0, buYilMaliyet: 0 };
        $.each(l, function (i, s) {
            t.oncYilKiralanan += Number(s.oncYilKiralanan) || 0;
            t.buYilKiralanan += Number(s.buYilKiralanan) || 0;
            t.gelecekYilKiralanacak += Number(s.gelecekYilKiralanacak) || 0;
            t.oncYilMaliyet += (Number(s.oncYilKiralanan) || 0) * (Number(s.oncYilKiralananSure) || 0) * (Number(s.oncYilFiyat) || 0);
            t.buYilMaliyet += (Number(s.buYilKiralanan) || 0) * (Number(s.buYilKiralananSure) || 0) * (Number(s.buYilFiyat) || 0);
        });
        grid.setGridOption('pinnedBottomRowData', [t]);
    }

    // Eski TuslariAyarla: Onayla yeni (1) formda, Onay Kaldır onaylı (5) formda ve yalnızca sistem yöneticisine; onaylı form kaydedilmez.
    function tuslariAyarla() {
        var durum = acik ? acik.durum : 0;
        var yazilir = !!acik && durum !== 5;
        $('#btnKaydet').prop('hidden', durum === 5).prop('disabled', !yazilir);
        $('#btnSatirEkle, #btnSatirSil').prop('disabled', !yazilir);
        $('#btnOnayla').prop('hidden', durum !== 1);
        $('#btnOnayKaldir').prop('hidden', !(durum === 5 && tanim.sistemYoneticisi));
        var $d = $('#lblDurum');
        if (!acik) { $d.prop('hidden', true); return; }
        $d.prop('hidden', false).text(DURUM_AD[durum] || String(durum))
            .removeClass('label-default label-info label-success').addClass(durum === 5 ? 'label-success' : (durum === 1 ? 'label-info' : 'label-default'));
    }

    function kayitBilgiYaz(v) {
        var p = [];
        if (v && v.kaydeden) p.push('Kaydeden: ' + v.kaydeden + (v.kayitTarihi ? ' (' + v.kayitTarihi + ')' : ''));
        if (v && v.onaylayan) p.push('Onaylayan: ' + v.onaylayan + (v.onayTarihi ? ' (' + v.onayTarihi + ')' : ''));
        $('#lblKayit').text(p.join(' · '));
    }

    function listeyiBosalt() {
        acik = null;
        degisti = false;
        grid.setGridOption('rowData', []);
        grid.setGridOption('pinnedBottomRowData', []);
        grid.setGridOption('overlayNoRowsTemplate', BOS_IPUCU);
        kayitBilgiYaz(null);
        tuslariAyarla();
    }

    // Oku (eski btnOku_Click). sessiz: kayıt / onaydan sonra yenilerken "bulunamadı" bildirimi gösterilmez.
    function oku(sessiz) {
        var o = sessiz && acik ? { yil: acik.yil, muhasebe: acik.muhasebe, birim: acik.birim } : { yil: yilOku(), muhasebe: muhasebeOku(), birim: birimOku() };
        if (o.yil < tanim.ilkYil || o.yil > tanim.sonYil) { mesaj('Yıl ' + tanim.ilkYil + ' ile ' + tanim.sonYil + ' arasında olmalıdır.', 'warning'); return; }
        if (!o.muhasebe || !o.birim) { mesaj('Lütfen muhasebe ve birim giriniz!', 'warning'); return; }
        var $d = $('#btnOku').prop('disabled', true);
        grid.stopEditing(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/arackiralama/oku', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = { yil: o.yil, muhasebe: o.muhasebe, birim: o.birim, durum: v.durum || 0 };
                degisti = false;
                grid.setGridOption('columnDefs', sutunlar(o.yil));
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt yok. Satır Ekle ile satır ekleyip Kaydet\'e basınız.</span>');
                grid.setGridOption('rowData', v.satirlar || []);
                toplamYaz();
                kayitBilgiYaz(v);
                tuslariAyarla();
                if (!v.bulundu && !sessiz) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#btnOku').on('click', function () {
        if (degisti && !window.confirm('Kaydedilmemiş değişiklikler var. Yeniden okursanız kaybolacak. Devam edilsin mi?')) return;
        oku(false);
    });

    // Yıl ya da muhasebe / birim değişince okunmuş form kullanılmaz (kayıt okunan birime yazılır).
    function secimDegisiyor() {
        if (!acik) return true;
        if (degisti && !window.confirm('Kaydedilmemiş değişiklikler var. Seçimi değiştirirseniz kaybolacak. Devam edilsin mi?')) return false;
        listeyiBosalt();
        return true;
    }

    $('#txtYil').on('focus', function () { $(this).data('onceki', $(this).val()); });
    $('#txtYil').on('change', function () {
        if (!secimDegisiyor()) { $(this).val($(this).data('onceki')); return; }
        $(this).data('onceki', $(this).val());
        var yil = yilOku();
        if (yil >= tanim.ilkYil && yil <= tanim.sonYil) grid.setGridOption('columnDefs', sutunlar(yil));
    });

    // Muhasebe - birim - kurumsal kod (ortak bileşen, muhbirim.js; eski txtBirim_Changed ve ListeMuhasebeBirim penceresi).
    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: yilOku,
        oncesi: secimDegisiyor
    });

    // ---------------------------------------------------------------- satır ekle / sil

    $('#btnSatirEkle').on('click', function () {
        if (!acik || acik.durum === 5) return;
        grid.stopEditing();
        var yeni = {
            tasitTipi: 0, butceKaynagi: tanim.kaynaklar.length ? tanim.kaynaklar[0].kod : 0, tasitKodu: '',
            oncYilKiralanan: 0, oncYilKiralananSure: 0, oncYilFiyat: 0,
            buYilKiralanan: 0, buYilKiralananSure: 0, buYilFiyat: 0,
            gelecekYilKiralanacak: 0, gelecekYilKiralanacakSure: 0, gelecekYilFiyat: 0
        };
        var sonuc = grid.applyTransaction({ add: [yeni] });
        degisti = true;
        toplamYaz();
        if (sonuc && sonuc.add && sonuc.add.length) {
            grid.ensureNodeVisible(sonuc.add[0], 'bottom');
            grid.startEditingCell({ rowIndex: sonuc.add[0].rowIndex, colKey: 'tasitKodu' });
        }
    });

    $('#btnSatirSil').on('click', function () {
        if (!acik || acik.durum === 5) return;
        grid.stopEditing();
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Silinecek satırları işaretleyiniz.', 'info'); return; }
        grid.applyTransaction({ remove: secili });
        degisti = true;
        toplamYaz();
        mesaj(secili.length + ' satır silindi; kalıcı olması için Kaydet\'e basınız.', 'info');
    });

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
        if (soruIsi) soruIsi();
        soruIsi = null;
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- kaydet / onayla (eski btnKaydet_Click, btnOnayla_Click, btnOnayKaldir_Click)

    $('#btnKaydet').on('click', function () {
        if (!acik || acik.durum === 5) return;
        grid.stopEditing();
        sor('Onay', 'Kaydetmek İstiyor musunuz?', 'Kaydet', function () {
            var gonderilen = $.map(satirlar(), function (s) {
                return {
                    tasitTipi: s.tasitTipi || 0, tasitKodu: s.tasitKodu || '', butceKaynagi: parseInt(s.butceKaynagi, 10) || 0,
                    oncYilKiralanan: Number(s.oncYilKiralanan) || 0, oncYilKiralananSure: Math.round(Number(s.oncYilKiralananSure) || 0), oncYilFiyat: Number(s.oncYilFiyat) || 0,
                    buYilKiralanan: Number(s.buYilKiralanan) || 0, buYilKiralananSure: Math.round(Number(s.buYilKiralananSure) || 0), buYilFiyat: Number(s.buYilFiyat) || 0,
                    gelecekYilKiralanacak: Number(s.gelecekYilKiralanacak) || 0, gelecekYilKiralanacakSure: Math.round(Number(s.gelecekYilKiralanacakSure) || 0),
                    gelecekYilFiyat: Number(s.gelecekYilFiyat) || 0
                };
            });
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder('api/arackiralama/kaydet', { yil: acik.yil, muhasebe: parseInt(acik.muhasebe, 10) || 0, birim: acik.birim, durum: acik.durum, satirlar: gonderilen })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) { degisti = false; oku(true); }
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', !acik || acik.durum === 5); });
        });
    });

    function onayla(durum) {
        if (!acik) return;
        grid.stopEditing();
        var metin = durum === 5
            ? 'Onayladıktan sonra verileri değiştiremezsiniz, devam etmek istiyor musunuz?' + (degisti ? ' (Kaydedilmemiş değişiklikler onaya girmez.)' : '')
            : 'Formun onayı kaldırılacak. Devam edilsin mi?';
        sor(durum === 5 ? 'Onay' : 'Onay Kaldır', metin, durum === 5 ? 'Onayla' : 'Onay Kaldır', function () {
            Sgb.gonder('api/arackiralama/onayla', { yil: acik.yil, muhasebe: parseInt(acik.muhasebe, 10) || 0, birim: acik.birim, durum: durum })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) { degisti = false; oku(true); }
                })
                .fail(hataGoster);
        });
    }

    $('#btnOnayla').on('click', function () { onayla(5); });
    $('#btnOnayKaldir').on('click', function () { onayla(1); });

    // ---------------------------------------------------------------- raporlar (eski btnYazdir_Click; Tur 1 / 2 / 3)

    function yazdir(tur, $d) {
        var yil = yilOku();
        if (yil < tanim.ilkYil) { mesaj('Yıl hatalı!', 'warning'); return; }
        var p = { yil: yil, tur: tur };
        if (tur === 1) {
            if (!muhasebeOku() || !birimOku()) { mesaj('Lütfen muhasebe ve birim giriniz!', 'warning'); return; }
            p.muhasebe = parseInt(muhasebeOku(), 10) || 0;
            p.birim = birimOku();
        }
        $d.prop('disabled', true);
        Sgb.indir('api/arackiralama/rapor', p).then(function () { $d.prop('disabled', false); });
    }

    $('#btnYazdir').on('click', function () { yazdir(1, $(this)); });
    $('#btnYazdirKurum').on('click', function () { yazdir(2, $(this)); });
    $('#btnYazdirDetay').on('click', function () { yazdir(3, $(this)); });

    // ---------------------------------------------------------------- onay işlemleri (eski wndOnayIslemler; sistem yöneticisi)

    var dlgOnayIslem = document.getElementById('dlgOnayIslem');

    function durumSayi(alan, baslik) {
        return { field: alan, headerName: baslik, width: 120, type: 'sayi', headerClass: 'sayi', cellClass: 'sayi',
            valueFormatter: function (p) { return p.value == null ? '' : sayi.format(Math.round(p.value)); } };
    }

    var durumGrid = Sgb.grid('#grdDurum', {
        columnDefs: [
            { field: 'saymanlik', headerName: 'Say.', width: 80 },
            { field: 'birim', headerName: 'Brm', width: 70 },
            { field: 'birimAd', headerName: 'Birim Ad', width: 300, tooltipField: 'birimAd' },
            { field: 'durum', headerName: 'Durum', width: 80, valueFormatter: function (p) { return p.value === 1 ? 'Yeni' : (p.value === 5 ? 'Onaylı' : 'Yok'); } },
            durumSayi('oncYilMiktar', 'Önc. Yıl (Araç*Ay)'),
            durumSayi('oncYilTutar', 'Önc. Yıl Maliyet'),
            durumSayi('buYilMiktar', 'Yılı (Araç*Ay)'),
            durumSayi('buYilTutar', 'Yılı Maliyet'),
            durumSayi('gelecekYilMiktar', 'Sonr. Yıl (Araç*Ay)')
        ],
        defaultColDef: { sortable: true, filter: false, resizable: true, wrapHeaderText: true, autoHeaderHeight: true },
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
        overlayNoRowsTemplate: '<span>Birim bulunamadı.</span>'
    });

    // Eski DurumlariYukle: pencere açılınca seçili yılın bütün birimleri.
    var durumYil = 0;
    function durumlariOku() {
        var yil = yilOku();
        if (yil < tanim.ilkYil) { mesaj('Yıl hatalı!', 'warning'); return; }
        durumYil = yil;
        $('#dlgOnayIslemBaslik').text('Onaylı listesi (' + yil + ')');
        durumGrid.setGridOption('rowData', []);
        durumGrid.setGridOption('loading', true);
        Sgb.al('api/arackiralama/durumlar', { yil: yil })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                durumGrid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { durumGrid.setGridOption('loading', false); });
    }

    $('#btnOnayIslemler').on('click', function () {
        dlgOnayIslem.showModal();
        durumlariOku();
    });
    $('#btnDurumYenile').on('click', durumlariOku);

    // Eski btnSeciliOnayla_Click (onayKod 5 / 1).
    function seciliOnayla(durum) {
        var secili = durumGrid.getSelectedRows();
        if (!secili.length) { mesaj('Seçim yapmanız gerekmektedir!', 'warning'); return; }
        var yil = durumYil;
        sor(durum === 5 ? 'Onay' : 'Onay Kaldır', 'Seçilen ' + secili.length + ' birimin ' + yil + ' yılı formu ' + (durum === 5 ? 'onaylanacak' : 'onayı kaldırılacak') + '. Devam edilsin mi?',
            durum === 5 ? 'Onayla' : 'Onay Kaldır', function () {
                Sgb.gonder('api/arackiralama/seciliOnayla', {
                    yil: yil, durum: durum,
                    birimler: $.map(secili, function (s) { return { saymanlik: s.saymanlik, birim: s.birim }; })
                })
                    .done(function (s) {
                        mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'warning');
                        durumlariOku();
                        if (acik && acik.yil === yil && !degisti) oku(true);
                    })
                    .fail(hataGoster);
            });
    }

    $('#btnSeciliOnayla').on('click', function () { seciliOnayla(5); });
    $('#btnSeciliOnayKaldir').on('click', function () { seciliOnayla(1); });

    window.addEventListener('beforeunload', function (e) {
        if (degisti) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    tuslariAyarla();
    Sgb.al('api/arackiralama/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            $('#txtYil').val(t.yil).attr({ min: t.ilkYil, max: t.sonYil });
            muhBirim.ata({ muhasebe: t.muhasebe, birim: t.birim, kurumsal: t.kurumsal, birimAd: t.birimAd });
            $('.yonetici').prop('hidden', !t.sistemYoneticisi);
            grid.setGridOption('columnDefs', sutunlar(t.yil));
            tuslariAyarla();
        })
        .fail(hataGoster);
});
