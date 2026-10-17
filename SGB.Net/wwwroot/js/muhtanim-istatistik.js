// İstatistik Kod Girişi (ButceMuhasebe/IstatistikKodGirisForm): istatistik kodları (ad, ödeme emri / MİF zorunluluğu, ek alan adları)
// ve seçili kodun seçim değerleri (değer kodu, açıklama, ek alanlar). Uçlar: /api/muhtanim/istatistik/*.
$(function () {
    'use strict';

    if (!$('#grdDeger').length) return;

    var kodlar = [];
    var girilir = { editable: true, cellClass: 'mt-girilir' };
    var tablo = Sgb.grid('#grdDeger', {
        columnDefs: [
            $.extend({ field: 'degerKod', headerName: 'Değer Kod', width: 120, type: 'rightAligned', cellEditor: 'agNumberCellEditor', cellEditorParams: { precision: 0 } }, girilir),
            $.extend({ field: 'aciklama', headerName: 'Açıklama', flex: 2, minWidth: 260 }, girilir),
            $.extend({ field: 'ekAlan1', headerName: 'Ek Alan1', flex: 1, minWidth: 160, hide: true }, girilir),
            $.extend({ field: 'ekAlan2', headerName: 'Ek Alan2', flex: 1, minWidth: 160, hide: true }, girilir),
            $.extend({ field: 'ekAlan3', headerName: 'Ek Alan3', flex: 1, minWidth: 160, hide: true }, girilir)
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true
    });
    tablo.setGridOption('overlayNoRowsTemplate', '<span class="ag-overlay-no-rows-center">İstatistik kodunu seçip Oku\'ya basın.</span>');

    function seciliKod() { return parseInt($('#isKod').val(), 10) || 0; }

    function kodBul(kod) {
        var k = null;
        $.each(kodlar, function (i, x) { if (x.kod === kod) { k = x; return false; } });
        return k;
    }

    function kodlariOku(secilecek) {
        return Sgb.mt.oku('api/muhtanim/istatistik/kodlar').done(function (v) {
            kodlar = v.kodlar || [];
            var $s = $('#isKod').empty();
            $.each(kodlar, function (i, k) { $('<option></option>').val(k.kod).text(k.kod + ' - ' + k.ad).appendTo($s); });
            if (secilecek) $s.val(String(secilecek));
        });
    }

    // Ek alan sütunları: başlık kodun ek alan adından; adı boşsa sütun gizlenir (eski KolonAyarla).
    function kolonAyarla(k) {
        var tanimlar = tablo.getColumnDefs();
        $.each(tanimlar, function (i, c) {
            var m = /^ekAlan([123])$/.exec(c.field || '');
            if (!m) return;
            var ad = k ? k['ekAlan' + m[1] + 'Ad'] : '';
            c.headerName = ad || ('Ek Alan' + m[1]);
            c.hide = !ad;
        });
        tablo.setGridOption('columnDefs', tanimlar);
    }

    function oku() {
        var kod = seciliKod();
        if (!kod) { Sgb.bildirim('Lütfen istatistik kodunu seçin.', 'warning'); return; }
        Sgb.mt.oku('api/muhtanim/istatistik/oku', { kod: kod }, tablo).done(function (v) {
            kolonAyarla(v.kod);
            tablo.setGridOption('rowData', v.degerler || []);
        });
    }

    $('#btnOku').on('click', oku);
    $('#isKod').on('change', function () { tablo.setGridOption('rowData', []); kolonAyarla(kodBul(seciliKod())); });

    $('#btnSatirEkle').on('click', function () {
        tablo.applyTransaction({ add: [{ degerKod: null, aciklama: '', ekAlan1: '', ekAlan2: '', ekAlan3: '' }] });
    });
    $('#btnSatirSil').on('click', function () {
        var s = tablo.getSelectedRows();
        if (!s.length) { Sgb.bildirim('Silinecek satırı seçin.', 'warning'); return; }
        tablo.applyTransaction({ remove: s });
    });

    $('#btnKaydet').on('click', function () {
        var kod = seciliKod();
        if (!kod) { Sgb.bildirim('Lütfen istatistik kodunu seçin.', 'warning'); return; }
        tablo.stopEditing();
        var degerler = [];
        tablo.forEachNode(function (n) {
            var d = n.data;
            degerler.push({ degerKod: parseInt(d.degerKod, 10) || 0, aciklama: d.aciklama || '', ekAlan1: d.ekAlan1 || '', ekAlan2: d.ekAlan2 || '', ekAlan3: d.ekAlan3 || '' });
        });
        Sgb.mt.islem('api/muhtanim/istatistik/kaydet', { kod: kod, degerler: degerler }, 'Kaydetmek istiyor musunuz?');
    });

    // ---------------------------------------------------------------- istatistik kodu tanımı

    var pencere = Sgb.mt.pencere('#dlgKod');
    var kodForm = Sgb.mt.form('#frmKod');

    $('#btnKodEkle').on('click', function () {
        kodForm.temizle();
        pencere.showModal();
    });
    $('#btnKodDuzenle').on('click', function () {
        var k = kodBul(seciliKod());
        if (!k) { Sgb.bildirim('Lütfen istatistik kodunu seçin.', 'warning'); return; }
        kodForm.temizle();
        kodForm.yaz(k);
        pencere.showModal();
    });
    $('#btnKodKaydet').on('click', function () {
        var f = kodForm.oku();
        f.kod = parseInt(f.kod, 10) || 0;
        Sgb.mt.islem('api/muhtanim/istatistik/kod-kaydet', f).done(function (v) {
            var kod = parseInt(v.anahtar, 10) || f.kod;
            $('#kdKod').val(kod || '');
            kodlariOku(kod).done(function () { kolonAyarla(kodBul(kod)); });
        });
    });

    kodlariOku().done(function () { kolonAyarla(kodBul(seciliKod())); });
});
