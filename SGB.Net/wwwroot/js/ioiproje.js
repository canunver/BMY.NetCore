// İl Özel İdaresi Projeleri (ButceHazirlikv2/IOIProjeListeForm): İÖİ / YİKOB projeleri kriterlere ve yıla göre listelenir.
// Satırdaki Bilgi tuşu proje tanım penceresini (eski wndTanim; eski IOIProjeGirisForm'un yerine), Tahsis ve Harc. tuşları
// tahsis / harcama giriş pencerelerini açar. Tanım penceresinde birimin proje girişi onaylanır (Onay Kaldır sistem yöneticisi).
// Onay Durum (sistem yöneticisi) birimlerin onay durumunu gösterir. Yazdır / Tertip / Detaylı listeleri Excel olarak verir.
// Uçlar: /api/ioiproje/* (ButceHazirlik/IOIProjeUclari.cs).
$(function () {
    'use strict';

    if (!$('#grdIOIProje').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var U = 'api/ioiproje/';
    var tanim = { yil: new Date().getFullYear(), iller: [], yonetici: false };
    var acikSira = 0;        // tanım / tahsis / harcama penceresinde açık proje (0: yeni)
    var birimOnayli = false; // tanım penceresindeki birimin onay durumu
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function tamSayi(sec) { return parseInt($(sec).val(), 10) || 0; }

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş -> 0. Geçersizse null.
    function sayiCoz(metin) {
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function paraYaz(p) { return p.value == null || p.value === '' ? '' : para.format(p.value); }

    function ilDoldur($s, bosAd) {
        $s.empty().append($('<option value="0"></option>').text(bosAd));
        $.each(tanim.iller || [], function (i, x) { $s.append($('<option></option>').attr('value', x.kod).text(x.ad)); });
    }

    // ---------------------------------------------------------------- onay sorusu

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

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- proje listesi (eski grdIOIProje)

    function tutarSutun(alan, baslik) {
        return { field: alan, headerName: baslik, width: 120, type: 'rightAligned', valueFormatter: paraYaz };
    }

    var listeGrid = Sgb.grid('#grdIOIProje', {
        columnDefs: [
            {
                colId: 'komut', headerName: 'Proje Detayı', width: 190, pinned: 'left', sortable: false, resizable: false,
                cellRenderer: function () {
                    return '<span class="ioi-komut">'
                        + '<button type="button" class="btn btn-default btn-xs" data-komut="Ac" title="Bu projeyi görmek / düzenlemek / silmek için tıklayınız."><span class="glyphicon glyphicon-pencil" aria-hidden="true"></span> Bilgi</button>'
                        + '<button type="button" class="btn btn-default btn-xs" data-komut="Akt" title="Aktarılan tutarları görmek / düzenlemek için tıklayınız."><span class="glyphicon glyphicon-plus-sign" aria-hidden="true"></span> Tahsis</button>'
                        + '<button type="button" class="btn btn-default btn-xs" data-komut="Harc" title="Harcanan tutarları görmek / düzenlemek için tıklayınız."><span class="glyphicon glyphicon-minus-sign" aria-hidden="true"></span> Harc.</button>'
                        + '</span>';
                }
            },
            { field: 'siraNo', headerName: '#', width: 64, type: 'rightAligned' },
            { field: 'muhasebeBirimAd', headerName: 'Birim Ad', width: 200, tooltipField: 'muhasebeBirimAd' },
            { field: 'DPTProjeNo', headerName: 'Yat. Proje No', width: 110 },
            { field: 'projeAdi', headerName: 'Yat. Proje Adı', width: 180, tooltipField: 'projeAdi' },
            { field: 'altProjeAdi', headerName: 'Yat. Alt Proje Adı', width: 180, tooltipField: 'altProjeAdi' },
            { field: 'ioiProjeAdi', headerName: 'İl Öz. İd. Proje Adı', width: 200, tooltipField: 'ioiProjeAdi' },
            { field: 'baslaYil', headerName: 'Baş. Yılı', width: 76, type: 'rightAligned' },
            { field: 'bitisYil', headerName: 'Bitiş Yılı', width: 76, type: 'rightAligned' },
            tutarSutun('projeTutari', 'İl Öz.İd.Pr. Tutarı'),
            tutarSutun('aktarilanTutar', 'Girilen Tahsis'),
            tutarSutun('islemleEklenen', 'İşlemle Eklenen'),
            tutarSutun('toplamButce', 'Bütçe'),
            tutarSutun('iOIHarc', 'Gir. Harcama'),
            tutarSutun('gonderilen', 'Gönderilen'),
            tutarSutun('toplamHarc', 'Toplam'),
            { field: 'ilAdi', headerName: 'İl', width: 130 },
            { field: 'aciklama', headerName: 'Açıklama', width: 200, tooltipField: 'aciklama' }
        ],
        defaultColDef: { sortable: true, filter: false, resizable: true, wrapHeaderText: true, autoHeaderHeight: true },
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        overlayNoRowsTemplate: '<span>Kriterleri seçip Listele\'ye basınız.</span>',
        onCellClicked: function (e) {
            if (e.colDef.colId !== 'komut' || !e.data) return;
            var $t = $(e.event && e.event.target).closest('[data-komut]');
            if (!$t.length) return;
            var komut = $t.attr('data-komut');
            if (komut === 'Ac') tanimAc(e.data.siraNo);
            else if (komut === 'Akt') tahsisAc(e.data);
            else if (komut === 'Harc') harcamaAc(e.data);
        },
        onRowDoubleClicked: function (e) { if (e.data) tanimAc(e.data.siraNo); }
    });

    function kriter() {
        return {
            tur: parseInt($('input[name=fltTur]:checked').val(), 10) || 0,
            ad: $.trim($('#fltAd').val()),
            basYil: tamSayi('#fltBasYil'),
            bitYil: tamSayi('#fltBitYil'),
            altOdenek: tamSayi('#fltOdenek'),
            ustOdenek: tamSayi('#fltUstOdenek'),
            il: tamSayi('#fltIl'),
            yil: tamSayi('#txtYil')
        };
    }

    // Eski btnOku_Click / Listele.
    function listele() {
        var k = kriter();
        if (k.yil <= 0) { mesaj('Yılı giriniz.', 'warning'); return; }
        var $d = $('#btnOku').prop('disabled', true);
        listeGrid.setGridOption('loading', true);
        Sgb.al(U + 'liste', k)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                listeGrid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun proje bulunamadı.</span>');
                listeGrid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); listeGrid.setGridOption('loading', false); });
    }

    $('#btnOku').on('click', listele);
    $('#frmKriter').on('keydown', 'input', function (e) { if (e.key === 'Enter') { e.preventDefault(); listele(); } });

    // ---------------------------------------------------------------- raporlar (eski btnYazdir / btnYazdirTertip / btnYazdirDetayli)

    function yazdir(tur, $d) {
        var k = kriter();
        if (k.yil <= 0) { mesaj('Yılı giriniz.', 'warning'); return; }
        var p = { tur: tur, isTur: k.tur, ad: k.ad, basYil: k.basYil, bitYil: k.bitYil, altOdenek: k.altOdenek, ustOdenek: k.ustOdenek, il: k.il, yil: k.yil };
        $d.prop('disabled', true);
        Sgb.indir(U + 'rapor', p).then(function () { $d.prop('disabled', false); });
    }

    $('#btnYazdir').on('click', function () { yazdir(1, $(this)); });
    $('#btnYazdirDetayli').on('click', function () { yazdir(2, $(this)); });
    $('#btnYazdirTertip').on('click', function () { yazdir(3, $(this)); });

    // ---------------------------------------------------------------- proje tanım penceresi (eski wndTanim)

    var dlgTanim = document.getElementById('dlgTanim');

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: function () { return tamSayi('#txtYil') || tanim.yil; },
        kurumsalGizle: true,
        degisince: function (d) { birimDurumOku(d.muhasebe, d.birim); }
    });

    var altGrid = Sgb.grid('#grdAltFaaliyet', {
        columnDefs: [
            { field: 'yil', headerName: 'Yıl', width: 64 },
            { field: 'pebKod', headerName: 'Kod', width: 110 },
            { field: 'pebAd', headerName: 'Ad', flex: 1, minWidth: 140, tooltipField: 'pebAd' },
            { field: 'projeAd', headerName: 'Proje', width: 150, tooltipField: 'projeAd' },
            { field: 'altProjeAd', headerName: 'Alt Proje', width: 150, tooltipField: 'altProjeAd' }
        ],
        overlayNoRowsTemplate: '<span>Bağlı alt faaliyet yok.</span>'
    });

    // Eski TuslariAyarla: birim seçiliyse durum yazılır; onaysızsa Onayla, onaylıysa (yöneticiye) Onay Kaldır görünür.
    function durumYaz(b) {
        b = b || {};
        birimOnayli = !!b.onayli;
        var birimVar = !!b.birimAd;
        $('#lblDurum').text(b.durum || '');
        $('#btnOnay').prop('hidden', !birimVar || birimOnayli);
        $('#btnOnayKaldir').prop('hidden', !(birimVar && birimOnayli && tanim.yonetici));
    }

    function birimDurumOku(m, b) {
        if (!m || !b) { durumYaz(null); return; }
        Sgb.al(U + 'birim', { muhasebe: m, birim: b })
            .done(function (v) { if (v.hata) { mesaj(v.hata, 'danger'); return; } durumYaz(v); })
            .fail(hataGoster);
    }

    function siraYaz() {
        $('#lblSiraNo').text(acikSira ? '#' + acikSira : 'Yeni Kayıt');
        $('#btnSil').prop('hidden', !acikSira);
    }

    // Eski Goster.
    function tanimYaz(p, pebKodlar, birim) {
        acikSira = p ? p.siraNo : 0;
        p = p || {};
        $('#radIOI').prop('checked', p.isTur === 1);
        $('#radYIKOB').prop('checked', p.isTur === 2);
        muhBirim.ata({ muhasebe: p.muhasebeKod ? String(p.muhasebeKod) : '', birim: p.birimKod || '', kurumsal: '', birimAd: birim ? birim.birimAd : '' });
        $('#txtProjeAdi').val(p.ioiProjeAdi || '');
        $('#txtBaslaYil').val(p.baslaYil ? p.baslaYil : '');
        $('#txtBitisYil').val(p.bitisYil ? p.bitisYil : '');
        $('#txtToplamMaliyet').val(p.projeTutari ? para.format(p.projeTutari) : '');
        $('#ddlIl').val(String(p.ilKod || 0));
        $('#txtAciklama').val(p.aciklama || '');
        altGrid.setGridOption('rowData', pebKodlar || []);
        durumYaz(birim);
        siraYaz();
    }

    // Eski wndTanim_BeforeShow: sıra 0 ise boş form.
    function tanimAc(sira) {
        if (!sira) {
            tanimYaz(null, [], null);
            if (!dlgTanim.open) dlgTanim.showModal();
            $('#txtProjeAdi').trigger('focus');
            return;
        }
        Sgb.al(U + 'oku', { siraNo: sira })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                tanimYaz(v.proje, v.pebKodlar, v.birim);
                if (!dlgTanim.open) dlgTanim.showModal();
            })
            .fail(hataGoster);
    }

    $('#btnYeni').on('click', function () { tanimAc(0); });

    $('#txtToplamMaliyet').on('change', function () {
        var n = sayiCoz($(this).val());
        if (n === null) { mesaj('Geçerli bir sayı yazınız.', 'warning'); return; }
        $(this).val($.trim($(this).val()) === '' ? '' : para.format(n));
    });

    // Eski btnTanimKaydet_Click.
    $('#btnTanimKaydet').on('click', function () {
        var tutar = sayiCoz($('#txtToplamMaliyet').val());
        if (tutar === null) { mesaj('Toplam maliyet geçerli bir sayı olmalıdır.', 'warning'); return; }
        var mb = muhBirim.deger();
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(U + 'kaydet', {
            siraNo: acikSira,
            isTur: $('#radYIKOB').prop('checked') ? 2 : 1,
            muhasebeKod: parseInt(mb.muhasebe, 10) || 0,
            birimKod: mb.birim || '',
            ioiProjeAdi: $.trim($('#txtProjeAdi').val()),
            baslaYil: tamSayi('#txtBaslaYil'),
            bitisYil: tamSayi('#txtBitisYil'),
            projeTutari: tutar,
            ilKod: tamSayi('#ddlIl'),
            aciklama: $.trim($('#txtAciklama').val())
        })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (!s.basarili) return;
                if (s.siraNo) acikSira = s.siraNo;
                siraYaz();
                listele();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // Eski btnTanimSil_Click: pencere açık kalır, yeniden kaydedilirse yeni proje olur.
    $('#btnSil').on('click', function () {
        if (!acikSira) return;
        sor('Proje kaydını silmek istiyor musunuz?', 'Sil', function () {
            Sgb.gonder(U + 'sil', { siraNo: acikSira })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (!s.basarili) return;
                    acikSira = 0;
                    siraYaz();
                    listele();
                })
                .fail(hataGoster);
        });
    });

    // Eski btnOnay_Click / btnOnayKaldir_Click.
    function onayDegistir(onayla) {
        var mb = muhBirim.deger();
        Sgb.gonder(U + 'onay', { muhasebeKod: parseInt(mb.muhasebe, 10) || 0, birimKod: mb.birim || '', onayla: onayla })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (s.basarili) durumYaz(s.birim);
            })
            .fail(hataGoster);
    }

    $('#btnOnay').on('click', function () {
        sor('Onay işleminden sonra proje bilgileri değiştirilemeyecektir. Onaylama işlemine devam etmek istiyor musunuz?', 'Onayla', function () { onayDegistir(true); });
    });
    $('#btnOnayKaldir').on('click', function () {
        sor('Onay kaldırma işlemine devam etmek istiyor musunuz?', 'Onay Kaldır', function () { onayDegistir(false); });
    });

    // ---------------------------------------------------------------- tahsis ve harcama pencereleri (eski wndAkt, wndHarc)

    function silSutun() {
        return {
            colId: 'sil', headerName: '', width: 44, sortable: false, resizable: false, editable: false,
            cellRenderer: function () {
                return '<button type="button" class="btn btn-default btn-xs" title="Satırı sil (kaydedince silinir)"><span class="glyphicon glyphicon-trash" aria-hidden="true"></span><span class="sr-only">Sil</span></button>';
            }
        };
    }

    // Satırı boşaltır: iş kuralı tutarı 0, açıklaması boş satırı kaydederken siler (yeni satır hiç eklenmez).
    function satirBosalt(grid, e, tutarAlan) {
        if (e.colDef.colId !== 'sil' || !e.data) return;
        e.node.setDataValue(tutarAlan, 0);
        e.node.setDataValue('aciklama', '');
    }

    function tarihSutun(alan, baslik) {
        return {
            field: alan, headerName: baslik, width: 140, editable: true, cellEditor: 'agDateStringCellEditor',
            valueFormatter: function (p) { return Sgb.tarih ? Sgb.tarih(p.value) : (p.value || ''); }
        };
    }

    function tutarGirisSutun(alan) {
        return {
            field: alan, headerName: 'Tutarı', width: 150, editable: true, type: 'rightAligned', cellEditor: 'agNumberCellEditor',
            valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; },
            valueFormatter: paraYaz
        };
    }

    var girisOrtak = {
        defaultColDef: { sortable: false, filter: false, resizable: true },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        suppressCellFocus: false
    };

    var tahsisGrid = Sgb.grid('#grdTahsis', $.extend({}, girisOrtak, {
        columnDefs: [
            {
                field: 'turStr', headerName: 'Tür', width: 110, editable: true, cellEditor: 'agSelectCellEditor',
                cellEditorParams: { values: ['Tahsis', 'Eklenen', 'Düşülen'] }
            },
            tarihSutun('aktarmaTarihiDT', 'Akt. Tarihi'),
            tutarGirisSutun('aktarilanTutar'),
            { field: 'aciklama', headerName: 'Açıklama', flex: 1, minWidth: 160, editable: true },
            silSutun()
        ],
        overlayNoRowsTemplate: '<span>Tahsis yok. Satır Ekle ile ekleyiniz.</span>',
        onCellClicked: function (e) { satirBosalt(tahsisGrid, e, 'aktarilanTutar'); }
    }));

    var harcamaGrid = Sgb.grid('#grdHarcama', $.extend({}, girisOrtak, {
        columnDefs: [
            tarihSutun('harcamaTarihiDT', 'Harc. Tarihi'),
            tutarGirisSutun('harcamaTutari'),
            { field: 'aciklama', headerName: 'Açıklama', flex: 1, minWidth: 160, editable: true },
            silSutun()
        ],
        overlayNoRowsTemplate: '<span>Harcama yok. Satır Ekle ile ekleyiniz.</span>',
        onCellClicked: function (e) { satirBosalt(harcamaGrid, e, 'harcamaTutari'); }
    }));

    function satirlar(grid) {
        grid.stopEditing();
        var l = [];
        grid.forEachNode(function (n) { if (n.data) l.push($.extend({}, n.data)); });
        return l;
    }

    function pencereProje(dlg, p) {
        $(dlg).find('.ioi-pencere-proje').text(p ? '#' + p.siraNo + ' ' + (p.ioiProjeAdi || '') : '');
    }

    var dlgTahsis = document.getElementById('dlgTahsis');
    var dlgHarcama = document.getElementById('dlgHarcama');

    function tahsisOku() {
        return Sgb.al(U + 'tahsis', { siraNo: acikSira })
            .then(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return false; }
                tahsisGrid.setGridOption('rowData', v.satirlar || []);
                return true;
            }, function (x) { hataGoster(x); return false; });
    }

    function harcamaOku() {
        return Sgb.al(U + 'harcama', { siraNo: acikSira })
            .then(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return false; }
                harcamaGrid.setGridOption('rowData', v.satirlar || []);
                return true;
            }, function (x) { hataGoster(x); return false; });
    }

    function tahsisAc(p) {
        acikSira = p.siraNo;
        pencereProje(dlgTahsis, p);
        tahsisOku().then(function (ok) { if (ok && !dlgTahsis.open) dlgTahsis.showModal(); });
    }

    function harcamaAc(p) {
        acikSira = p.siraNo;
        pencereProje(dlgHarcama, p);
        harcamaOku().then(function (ok) { if (ok && !dlgHarcama.open) dlgHarcama.showModal(); });
    }

    // Eski "Satır Ekle": stoAktarim.addRecord({ aktarmaNo: 0, aktarilanTutar: 0 }).
    $('#btnTahsisSatir').on('click', function () {
        var r = tahsisGrid.applyTransaction({ add: [{ turStr: 'Tahsis', aktarmaNo: 0, aktarmaTarihiDT: Sgb.bugun ? Sgb.bugun() : '', aktarilanTutar: 0, aciklama: '' }] });
        if (r && r.add && r.add[0]) tahsisGrid.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: 'aktarilanTutar' });
    });
    $('#btnHarcamaSatir').on('click', function () {
        var r = harcamaGrid.applyTransaction({ add: [{ harcamaNo: 0, harcamaTarihiDT: Sgb.bugun ? Sgb.bugun() : '', harcamaTutari: 0, aciklama: '' }] });
        if (r && r.add && r.add[0]) harcamaGrid.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: 'harcamaTutari' });
    });

    // Eski btnAktarimKaydet_Click / btnHarcamaKaydet_Click: kayıttan sonra yeniden okunur, liste tazelenir.
    $('#btnTahsisKaydet').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(U + 'tahsis/kaydet', { siraNo: acikSira, satirlar: satirlar(tahsisGrid) })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (s.basarili) { tahsisOku(); listele(); }
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });
    $('#btnHarcamaKaydet').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(U + 'harcama/kaydet', { siraNo: acikSira, satirlar: satirlar(harcamaGrid) })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (s.basarili) { harcamaOku(); listele(); }
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- onay durumu (eski TavanGenelDurumBirim.aspx?onayKod=21)

    var dlgOnayDurum = document.getElementById('dlgOnayDurum');

    var onayGrid = Sgb.grid('#grdOnayDurum', {
        columnDefs: [
            { field: 'muhKod', headerName: 'Muh. Kod', width: 90, type: 'rightAligned' },
            { field: 'birimKod', headerName: 'Birim Kod', width: 80 },
            { field: 'birimAd', headerName: 'Birim Ad', flex: 1, minWidth: 200, tooltipField: 'birimAd' },
            { field: 'projeSay', headerName: 'Proje Sayısı', width: 90, type: 'rightAligned' },
            { field: 'durum', headerName: 'Durum', width: 90 }
        ],
        defaultColDef: { sortable: true, filter: false, resizable: true, wrapHeaderText: true, autoHeaderHeight: true },
        overlayNoRowsTemplate: '<span>Kayıt yok.</span>'
    });

    function onayDurumOku() {
        onayGrid.setGridOption('loading', true);
        return Sgb.al(U + 'onaydurum')
            .then(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return false; }
                onayGrid.setGridOption('rowData', v.satirlar || []);
                return true;
            }, function (x) { hataGoster(x); return false; })
            .always(function () { onayGrid.setGridOption('loading', false); });
    }

    $('#btnOnaylilar').on('click', function () {
        onayGrid.setGridOption('rowData', []);
        dlgOnayDurum.showModal();
        onayDurumOku();
    });

    $('#btnHepOnayla').on('click', function () {
        sor('Onaylı olmayan bütün birimler onaylanacaktır. Devam etmek istiyor musunuz?', 'Hepsini Onayla', function () {
            var $d = $('#btnHepOnayla').prop('disabled', true);
            Sgb.gonder(U + 'onaydurum/hepsi', {})
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    onayDurumOku();
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // ---------------------------------------------------------------- tablo yüksekliği

    // Tabloyu sayfa alanının (.sayfa-icerik) kalan yüksekliğine yayar; satırlar tablonun içinde kayar (mptavan.js'deki gibi).
    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var kap = el.closest('.sayfa-icerik') || document.body, cs = getComputedStyle(kap);
        var kaydirir = /(auto|scroll)/.test(cs.overflowY);
        var kr = kap.getBoundingClientRect(), kay = kaydirir ? kap.scrollTop : 0;
        var gorunen = kaydirir ? kap.clientHeight : window.innerHeight - kr.top;
        var ust = el.getBoundingClientRect().top - kr.top + kay;
        var son = 0;
        $(kap).children().each(function () {
            if (this.offsetParent === null && getComputedStyle(this).position !== 'fixed') return;
            son = Math.max(son, this.getBoundingClientRect().bottom - kr.top + kay + parseFloat(getComputedStyle(this).marginBottom || 0));
        });
        son += parseFloat(cs.paddingBottom || 0);
        var bosluk = Math.max(0, son - (ust + el.offsetHeight));
        el.style.height = Math.max(enAz, Math.floor(gorunen - ust - bosluk - 1)) + 'px';
    }
    function tabloYay() { yuksekligeYay(document.getElementById('grdIOIProje'), 220); }
    $(window).on('resize', tabloYay);
    if (window.ResizeObserver) new ResizeObserver(tabloYay).observe(document.querySelector('.ioi-proje .ioi-ust'));
    tabloYay();

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al(U + 'tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            Sgb.yilKutusu('#txtYil', [2010, t.yil + 1], t.yil);
            ilDoldur($('#fltIl'), 'Hepsi');
            ilDoldur($('#ddlIl'), 'İl Seçiniz');
            $('#btnOnaylilar').prop('hidden', !t.yonetici);
            if (t.listele) listele();
        })
        .fail(hataGoster);
});
