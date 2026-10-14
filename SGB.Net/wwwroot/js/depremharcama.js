// Deprem Harcamaları Bilgi Formu (ButceForm/DepremHarcamaForm): seçilen yıl / ay, muhasebe birimi ve il için ekonomik kodlara göre
// bütçe içi ve bütçe dışı (özel hesap, döner sermaye, fon, sosyal tesis, diğer) harcamalar. İlk satır genel toplam (üstte sabit),
// 2 haneli eko satırları toplamdır; yalnızca 4 haneli eko satırları girilir, toplamlar değişikliği hemen gösterir (eski KolonDegisti).
// Yazdır tuşları eski Excel raporlarını indirir. Sistem yöneticisi formu onaylar / onayını kaldırır, Onay İşlemleri penceresinde
// ayın bütün birimlerini toplu onaylar ve önceki aydan kopyalar, Aktar ile ayın harcamalarını e-bütçeye gönderir.
$(function () {
    'use strict';

    if (!$('#grdDetay').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var ONAYLI = 5;
    var SAYILAR = ['butceIci', 'hT1', 'hT2', 'hT3', 'hT4', 'hT5'];
    var acik = null;          // okunmuş ölçüt { yil, ay, muhasebe, birim, il }: Kaydet ve Onayla bunu kullanır
    var durum = 0;            // okunan formun onay durumu (5: onaylı)
    var kirli = false;        // kaydedilmemiş değişiklik var mı?
    var yonetici = false;
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 0, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function olcutOku() {
        return {
            yil: parseInt($('#txtYil').val(), 10) || 0,
            ay: parseInt($('#ddlAy').val(), 10) || 0,
            muhasebe: parseInt($.trim($('#txtMuhasebe').val()), 10) || 0,
            birim: $.trim($('#txtBirim').val()),
            il: parseInt($('#ddlIl').val(), 10) || 0
        };
    }

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function paraYaz(p) { return p.value == null || p.value === '' ? '' : para.format(p.value); }

    // ---------------------------------------------------------------- harcama tablosu

    function girilir(p) {
        return !!p.data && !p.node.rowPinned && (p.data.ekoKod || '').length > 2 && durum !== ONAYLI;
    }

    function hucreSinif(p) {
        var s = [];
        if (p.colDef.type === 'sayi') s.push('sayi');
        if (girilir(p)) s.push('girilir');
        return s;
    }

    function sayiSutun(alan, baslik) {
        return {
            field: alan, headerName: baslik, type: 'sayi', width: 120, sortable: false, headerClass: 'sayi',
            valueFormatter: paraYaz, editable: girilir, cellClass: hucreSinif, cellEditor: 'agTextCellEditor',
            valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; }
        };
    }

    function metinSutun(alan, baslik) {
        return {
            field: alan, headerName: baslik, width: 200, sortable: false, tooltipField: alan,
            editable: girilir, cellClass: hucreSinif, cellEditor: 'agTextCellEditor', cellEditorParams: { maxLength: 50 }
        };
    }

    var grid = Sgb.grid('#grdDetay', {
        columnDefs: [
            { field: 'ad', headerName: 'Harcama Türü', width: 300, pinned: 'left', sortable: false, tooltipField: 'ad' },
            sayiSutun('butceIci', 'Bütçe İçi'),
            metinSutun('butceIciAciklama', 'Bütçe İçi Açıklama'),
            sayiSutun('hT1', 'Özel Hesap'),
            sayiSutun('hT2', 'Döner Sermaye'),
            sayiSutun('hT3', 'Fon'),
            sayiSutun('hT4', 'Sosyal Tesis'),
            sayiSutun('hT5', 'Diğer'),
            metinSutun('butceDisiAciklama', 'Bütçe Dışı Açıklama')
        ],
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        getRowId: function (p) { return p.data.ekoKod; },
        getRowClass: function (p) { return p.data && !p.node.rowPinned && (p.data.ekoKod || '').length === 2 ? 'eko2' : ''; },
        overlayNoRowsTemplate: '<span>Yıl / ay, muhasebe birimi ve il seçip Oku\'ya basınız.</span>',
        // Eko 4 tutarı değişince eko 2 toplam satırı ve genel toplam farkla güncellenir (eski KolonDegisti).
        onCellValueChanged: function (e) {
            if (e.node.rowPinned || (e.data.ekoKod || '').length <= 2 || e.newValue === e.oldValue) return;
            kirliYap();
            var alan = e.colDef.field;
            if (SAYILAR.indexOf(alan) < 0) return;
            var fark = (Number(e.newValue) || 0) - (Number(e.oldValue) || 0);
            if (!fark) return;
            var ust = grid.getRowNode(e.data.ekoKod.substring(0, 2));
            if (ust) ust.setDataValue(alan, (Number(ust.data[alan]) || 0) + fark);
            var toplam = grid.getPinnedTopRow(0);
            if (toplam) toplam.setDataValue(alan, (Number(toplam.data[alan]) || 0) + fark);
        }
    });

    function kirliYap() {
        kirli = true;
        $('#btnKaydet').prop('disabled', durum === ONAYLI);
    }

    // Onay durumuna göre tuşlar (eski TuslariAyarla). d null: henüz okunmadı (eski sayfadaki gibi iki onay tuşu da görünür).
    function tuslariAyarla(d) {
        $('#lblDurum').prop('hidden', d !== ONAYLI);
        $('#btnKaydet').prop('disabled', d === null || d === ONAYLI || !kirli);
        if (yonetici) {
            $('#btnOnayla').prop('hidden', d === ONAYLI);
            $('#btnOnayKaldir').prop('hidden', d !== null && d !== ONAYLI);
        }
    }

    function listeyiBosalt() {
        acik = null;
        durum = 0;
        kirli = false;
        grid.setGridOption('rowData', []);
        grid.setGridOption('pinnedTopRowData', []);
        grid.setGridOption('overlayNoRowsTemplate', '<span>Yıl / ay, muhasebe birimi ve il seçip Oku\'ya basınız.</span>');
        tuslariAyarla(null);
    }

    function kaybolsunMu() {
        return !kirli || window.confirm('Kaydedilmemiş değişiklikler var. Devam ederseniz kaybolacak. Devam edilsin mi?');
    }

    // Oku (eski btnOku_Click).
    function oku() {
        var o = olcutOku();
        if (o.yil < 2000) { mesaj('Lütfen yıl giriniz!', 'warning'); return; }
        if (o.muhasebe <= 0) { mesaj('Lütfen muhasebe giriniz!', 'warning'); return; }
        if (o.birim.length !== 3) { mesaj('Lütfen birim giriniz!', 'warning'); return; }
        if (o.il <= 0) { mesaj('Lütfen harcamanın hangi il için yapıldığını seçiniz!', 'warning'); return; }
        if (!kaybolsunMu()) return;
        var $d = $('#btnOku').prop('disabled', true);
        grid.stopEditing(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/depremharcama/oku', o)
            .done(function (v) {
                if (v.hata) { listeyiBosalt(); mesaj(v.hata, 'danger'); return; }
                kirli = false;
                durum = v.durum || 0;
                acik = v.satirlar.length ? o : null;
                var satirlar = v.satirlar.slice();
                var toplam = satirlar.length && satirlar[0].ekoKod === 'T' ? satirlar.shift() : null;
                grid.setGridOption('pinnedTopRowData', toplam ? [toplam] : []);
                grid.setGridOption('rowData', satirlar);
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                grid.refreshCells({ force: true });
                tuslariAyarla(v.satirlar.length ? durum : null);
                if (v.bilgi) mesaj(v.bilgi, 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#btnOku').on('click', oku);

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

    // ---------------------------------------------------------------- kaydet / onayla / aktar

    // Kaydet (eski btnKaydet_Click): genel toplam dahil bütün satırlar gönderilir; iş kuralı rakamı olan eko 4 satırlarını yazar.
    $('#btnKaydet').on('click', function () {
        if (!acik) return;
        grid.stopEditing();
        if (!kirli) { mesaj('Kaydedilecek değişiklik yok.', 'info'); return; }
        sor('Onay', 'Kaydetmek İstiyor musunuz?', 'Kaydet', function () {
            var satirlar = [];
            var toplam = grid.getPinnedTopRow(0);
            if (toplam) satirlar.push(toplam.data);
            grid.forEachNode(function (n) { satirlar.push(n.data); });
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder('api/depremharcama/kaydet', $.extend({}, acik, { satirlar: satirlar }))
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (!s.basarili) { $d.prop('disabled', false); return; }
                    kirli = false;
                })
                .fail(function (xhr) { $d.prop('disabled', false); hataGoster(xhr); });
        });
    });

    // Onayla / Onay Kaldır (eski btnOnayla_Click / btnOnayKaldir_Click; yalnızca sistem yöneticisi).
    function onayla(d) {
        var o = acik || olcutOku();
        if (o.yil < 2000 || o.muhasebe <= 0 || !o.birim) { mesaj('Yıl, ay, muhasebe ve birim giriniz.', 'warning'); return; }
        var metin = d === ONAYLI
            ? 'Onayladıktan sonra verileri değiştiremezsiniz, devam etmek istiyor musunuz?'
            : o.muhasebe + '-' + o.birim + ' biriminin ' + o.yil + ' / ' + o.ay + '. ay formunun onayı kaldırılacak. Devam edilsin mi?';
        sor('Onay', metin, d === ONAYLI ? 'Onayla' : 'Onay Kaldır', function () {
            Sgb.gonder('api/depremharcama/onayla', { yil: o.yil, ay: o.ay, muhasebe: o.muhasebe, birim: o.birim, durum: d })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (!s.basarili) return;
                    if (acik) {
                        durum = s.durum;
                        grid.refreshCells({ force: true });
                    }
                    tuslariAyarla(s.durum);
                })
                .fail(hataGoster);
        });
    }

    $('#btnOnayla').on('click', function () { onayla(ONAYLI); });
    $('#btnOnayKaldir').on('click', function () { onayla(1); });

    // Aktar (eski btnAktar_Click): ayın (il seçiliyse yalnızca o ilin) harcamaları e-bütçeye gönderilir.
    $('#btnAktar').on('click', function () {
        var o = olcutOku();
        if (o.yil < 2000 || o.ay < 1) { mesaj('Yıl / ay hatalı!', 'warning'); return; }
        var ilAd = o.il > 0 ? $('#ddlIl option:selected').text() + ' ilinin' : 'bütün illerin';
        sor('Aktar', o.yil + ' yılı ' + $('#ddlAy option:selected').text() + ' ayı için ' + ilAd + ' deprem harcamaları e-bütçe sistemine aktarılacak. Devam edilsin mi?', 'Aktar', function () {
            var $d = $('#btnAktar').prop('disabled', true);
            mesaj('Aktarım yapılıyor, lütfen bekleyiniz.', 'info');
            Sgb.gonder('api/depremharcama/aktar', { yil: o.yil, ay: o.ay, il: o.il })
                .done(function (s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // ---------------------------------------------------------------- raporlar

    // Yazdır tuşları (eski btnYazdir_Click): 1 İl-Eko, 2 Eko, 3 Muh. Birim, 4 Liste (Kod alanındaki ekonomik kod başına göre).
    $('.rapor').on('click', function () {
        var o = olcutOku();
        if (o.yil < 2000 || o.ay < 1) { mesaj('Yıl / ay hatalı!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/depremharcama/rapor', $.extend({ tur: $(this).attr('data-tur'), kod: $.trim($('#txtKriter').val()) }, o))
            .then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- ölçüt değişikliği

    // Ölçüt değişince okunmuş liste kullanılmaz (Kaydet listelenen forma yazar). Kaydedilmemiş değişiklik varsa sorulur.
    function olcutDegisti() {
        if (!acik) return true;
        if (!kaybolsunMu()) return false;
        listeyiBosalt();
        return true;
    }

    function degerSakla() {
        $('#txtYil, #ddlAy, #ddlIl, #txtMuhasebe, #txtBirim').each(function () { $(this).data('deger', $(this).val()); });
    }

    $('#txtYil, #ddlAy, #ddlIl').on('change', function () {
        if (!olcutDegisti()) { $(this).val($(this).data('deger')); return; }
        $(this).data('deger', $(this).val());
    });

    // Muhasebe - birim - kurumsal kod (ortak bileşen, muhbirim.js): değişince ölçüt değişmiş sayılır (eski txtBirim_Changed).
    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: function () { return parseInt($('#txtYil').val(), 10) || 0; },
        oncesi: olcutDegisti,
        degisince: degerSakla
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- onay işlemleri (eski wndOnayIslemler penceresi)

    var dlgOnay = document.getElementById('dlgOnay');
    var onayDonem = null; // { yil, ay }

    var durumGrid = Sgb.grid('#grdDurumlar', {
        columnDefs: [
            { field: 'saymanlikKod', headerName: 'Say.', width: 90 },
            { field: 'birimKod', headerName: 'Brm', width: 80 },
            { field: 'birimAd', headerName: 'Birim Ad', width: 300, tooltipField: 'birimAd' },
            { field: 'tutar', headerName: 'Tutar', width: 130, type: 'sayi', valueFormatter: paraYaz, cellClass: 'sayi', headerClass: 'sayi' },
            { field: 'durum', headerName: 'Durum', width: 100,
                valueFormatter: function (p) { return p.value === ONAYLI ? 'Onaylı' : (p.value ? String(p.value) : ''); } }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        getRowId: function (p) { return p.data.saymanlikKod + '-' + p.data.birimKod; },
        overlayNoRowsTemplate: '<span>Birim bulunamadı.</span>'
    });

    function durumlariYukle() {
        durumGrid.setGridOption('loading', true);
        Sgb.al('api/depremharcama/durumlar', onayDonem)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                durumGrid.setGridOption('rowData', v.satirlar);
            })
            .fail(hataGoster)
            .always(function () { durumGrid.setGridOption('loading', false); });
    }

    $('#btnOnayIslemler').on('click', function () {
        var o = olcutOku();
        if (o.yil < 2000 || o.ay < 1) { mesaj('Yıl / ay hatalı!', 'warning'); return; }
        onayDonem = { yil: o.yil, ay: o.ay };
        $('#lblOnayDonem').text(o.yil + ' yılı ' + $('#ddlAy option:selected').text() + ' ayı');
        durumGrid.setGridOption('rowData', []);
        dlgOnay.showModal();
        durumlariYukle();
    });

    // Seçili birimleri onaylar (5) ya da onaylarını kaldırır (1) (eski btnSeciliOnayla_Click).
    function seciliOnayla(d) {
        var secili = durumGrid.getSelectedRows();
        if (!secili.length) { mesaj('Seçim yapmanız gerekmektedir!', 'warning'); return; }
        sor('Onay', secili.length + ' birimin ' + (d === ONAYLI ? 'formu onaylanacak' : 'formunun onayı kaldırılacak') + '. Devam edilsin mi?', d === ONAYLI ? 'Onayla' : 'Onay Kaldır', function () {
            Sgb.gonder('api/depremharcama/secili-onayla', {
                yil: onayDonem.yil, ay: onayDonem.ay, durum: d,
                birimler: $.map(secili, function (s) { return { saymanlikKod: s.saymanlikKod, birimKod: s.birimKod }; })
            })
                .done(function (s) {
                    mesaj(s.bilgi || s.hata, s.basarili ? 'success' : (s.bilgi ? 'warning' : 'danger'));
                    durumlariYukle();
                })
                .fail(hataGoster);
        });
    }

    $('#btnSeciliOnayla').on('click', function () { seciliOnayla(ONAYLI); });
    $('#btnSeciliOnayKaldir').on('click', function () { seciliOnayla(1); });

    // Önceki Aydan Kopyala (eski btnOncekiAydanKopyala_Click).
    $('#btnOncekiAydanKopyala').on('click', function () {
        if (!onayDonem) return;
        sor('Önceki Aydan Kopyala', onayDonem.yil + ' / ' + onayDonem.ay + '. ay için önceki ayın harcamaları kopyalanacak. Devam edilsin mi?', 'Kopyala', function () {
            Sgb.gonder('api/depremharcama/onceki-aydan-kopyala', onayDonem)
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) durumlariYukle();
                })
                .fail(hataGoster);
        });
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/depremharcama/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            $('#txtYil').attr({ min: t.ilkYil, max: t.sonYil }).val(t.yil);
            var $ay = $('#ddlAy').empty();
            $.each(t.aylar, function (i, a) { $('<option></option>').val(a.kod).text(a.ad).appendTo($ay); });
            $ay.val(t.ay);
            var $il = $('#ddlIl').empty();
            $('<option></option>').val(0).text('İl Seçiniz').appendTo($il);
            $.each(t.iller, function (i, il) { $('<option></option>').val(il.kod).text(il.ad).appendTo($il); });
            muhBirim.ata({ muhasebe: t.muhasebe, birim: t.birim, kurumsal: t.kurumsal, birimAd: t.birimAd });
            yonetici = !!t.sistemYoneticisi;
            $('.yonetici').prop('hidden', !yonetici);
            degerSakla();
            tuslariAyarla(null);
        })
        .fail(hataGoster);
});
