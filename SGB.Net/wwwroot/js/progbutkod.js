// Programlar / Alt Programlar / Faaliyetler / Alt Faaliyetler (ButceHazirlikv2/ProgButKod?tur=1..4): yılın program bütçe kodları.
// Bul yılın kodlarını arama metnine göre listeler (açılışta Alt Faaliyet dışında kendiliğinden); listeden tıklanan ya da Yeni ile açılan
// pencerede kod kaydedilir / silinir. Önceki Yıldan Al bir önceki yılın kodlarını seçilen yıla aktarır. SBB'den Al program bütçe
// sistemindeki kodlarla karşılaştırıp farklı olanları gösterir; işaretlenenler kaydedilir. Yazdır listeyi Excel olarak indirir.
$(function () {
    'use strict';

    var $kok = $('.prog-but-kod');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var tur = parseInt($kok.data('tur'), 10) || 0;
    var kodAdi = $kok.data('kod-adi');
    var PROGRAM_TURLERI = { 1: 'Program', 2: 'Yönetim Hizmet Gideri', 3: 'Transfer' };

    function mesaj(metin, t) { Sgb.bildirim(metin, t); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }

    $('#txtYil').val(new Date().getFullYear());

    // ---------------------------------------------------------------- liste

    var sutunlar = [{ field: 'kod', headerName: 'Kod', width: 100 }];
    if (tur === 3) sutunlar.push({ field: 'fonKod', headerName: 'Fon.', width: 100 });
    if (tur === 1) sutunlar.push({ field: 'tur', headerName: 'Tür', width: 180, valueFormatter: function (p) { return PROGRAM_TURLERI[p.value] || ''; } });
    sutunlar.push({ field: 'ad', headerName: 'Adı', flex: 1, minWidth: 260, tooltipField: 'ad' });

    var grid = Sgb.grid('#grdListe', {
        columnDefs: sutunlar,
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        overlayNoRowsTemplate: '<span>Yılı seçip Bul\'a basınız.</span>',
        onRowClicked: function (e) { girisAc(e.data); }
    });

    function listele() {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $('#btnBul').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/progbutkod/liste', { tur: tur, yil: yil, ara: $.trim($('#txtAra').val()) })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(); });

    // ---------------------------------------------------------------- kod girişi (eski GirisAc / wndKodGiris)

    var dlgKod = document.getElementById('dlgKod');
    var duzenlenen = null; // listeden açılan kayıt; Yeni ile açıldıysa null

    function girisAc(d) {
        duzenlenen = d || null;
        $('#txtKodYil').val(d && d.yil ? d.yil : yilOku());
        $('#txtKod').val(d ? d.kod : '');
        $('#txtKodAd').val(d ? d.ad : '');
        $('#ddlProgramTur').val(d ? String(d.tur || 0) : '0');
        $('#txtFonKod').val(d ? d.fonKod : '');
        $('#btnSil').prop('disabled', !d);
        dlgKod.showModal();
        $(d ? '#txtKodAd' : '#txtKod').trigger('focus');
    }

    function girisOku() {
        return {
            tur: tur,
            yil: parseInt($('#txtKodYil').val(), 10) || 0,
            kod: parseInt($('#txtKod').val(), 10) || 0,
            ad: $.trim($('#txtKodAd').val()),
            programTur: parseInt($('#ddlProgramTur').val(), 10) || 0,
            fonKod: $.trim($('#txtFonKod').val() || '')
        };
    }

    $('#btnYeni').on('click', function () { girisAc(null); });

    $('#btnKaydet').on('click', function () {
        var o = girisOku();
        if (!o.ad) { mesaj('Adı yazınız.', 'warning'); $('#txtKodAd').trigger('focus'); return; }
        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/progbutkod/kaydet', o)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                dlgKod.close();
                mesaj(s.bilgi, 'success');
                listele();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnSil').on('click', function () {
        if (!duzenlenen) return;
        if (!window.confirm('Seçili ' + kodAdi.toLocaleLowerCase('tr') + ' silinsin mi?')) return;
        var $d = $('#btnSil').prop('disabled', true);
        Sgb.gonder('api/progbutkod/sil', girisOku())
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); $d.prop('disabled', false); return; }
                dlgKod.close();
                mesaj(s.bilgi, 'success');
                listele();
            })
            .fail(function (xhr) { hataGoster(xhr); $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Önceki Yıldan Al (eski btnAktar_Click)

    $('#btnAktar').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl bilgisi boş bırakılamaz.', 'warning'); return; }
        if (!window.confirm((yil - 1) + ' yılının ' + kodAdi.toLocaleLowerCase('tr') + ' kodları ' + yil + ' yılına aktarılacak. Devam edilsin mi?')) return;
        var $d = $('#btnAktar').prop('disabled', true);
        Sgb.gonder('api/progbutkod/aktar', { tur: tur, yil: yil })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                listele();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- SBB'den Al (eski wndFark)

    var dlgFark = document.getElementById('dlgFark');
    var farkGrid = null;

    function farkGridKur() {
        if (farkGrid) return farkGrid;
        farkGrid = Sgb.grid('#grdFark', {
            columnDefs: [
                { field: 'kodPB', headerName: 'Kod (PB)', width: 100 },
                { field: 'adPB', headerName: 'Adı (PB)', flex: 1, minWidth: 220, tooltipField: 'adPB' },
                { field: 'kodSGB', headerName: 'Kod (SGB)', width: 100, valueFormatter: function (p) { return p.value ? p.value : ''; } },
                { field: 'adSGB', headerName: 'Adı (SGB)', flex: 1, minWidth: 220, tooltipField: 'adSGB' }
            ],
            rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
            overlayNoRowsTemplate: '<span>Fark yok.</span>'
        });
        return farkGrid;
    }

    $('#btnSBBAl').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $('#btnSBBAl').prop('disabled', true);
        Sgb.al('api/progbutkod/sbb-fark', { tur: tur, yil: yil })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                if (v.bilgi) { mesaj(v.bilgi, 'info'); return; }
                dlgFark.showModal();
                farkGridKur().setGridOption('rowData', v.farklar || []);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnFarkKaydet').on('click', function () {
        var secili = farkGrid ? farkGrid.getSelectedRows() : [];
        if (!secili.length) { mesaj('Kaydedilecek satırları işaretleyiniz.', 'warning'); return; }
        var $d = $('#btnFarkKaydet').prop('disabled', true);
        Sgb.gonder('api/progbutkod/sbb-kaydet', {
            tur: tur, yil: yilOku(),
            satirlar: $.map(secili, function (s) { return { kodPB: s.kodPB, adPB: s.adPB }; })
        })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                dlgFark.close();
                mesaj(s.bilgi, 'success');
                listele();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Yazdır (eski btnYazdir_Click)

    $('#btnYazdir').on('click', function () {
        Sgb.indir('api/progbutkod/rapor', { tur: tur, yil: yilOku(), ara: $.trim($('#txtAra').val()) });
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // Eski sayfa Alt Faaliyet dışında açılışta listeliyordu.
    if (tur < 4) listele();
});
