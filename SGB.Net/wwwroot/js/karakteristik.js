// Karakteristikler (ButceHazirlikv2/KarakteristikForm): yatırım projelerinde seçilen karakteristik değerlerinin tanımları.
// Listele ile okunur; tabloda hücreye tıklanıp yazılır, Satır Ekle / Satır Sil ile satır eklenir ya da işaretliler silinir.
// Yapıştır panodaki satırları (Excel) seçili satırdan başlayarak yazar ve gerekirse satır açar. Kaydet bütün listeyi tablodakilerle
// değiştirir (eski sayfa gibi onay sorar).
$(function () {
    'use strict';

    if (!$('#grdKarakteristik').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var ALANLAR = ['sira', 'deger', 'degerKisa', 'ekTurKod'];
    var EK_TURLER = [
        { kod: 0, ad: 'Ek Girilmesin' },
        { kod: 1, ad: 'Arkaya Ek (Zorunlu)' },
        { kod: 2, ad: 'Öne Ek (Zorunlu)' },
        { kod: 3, ad: 'Arkaya Ek (Zorunlu Değil)' },
        { kod: 4, ad: 'Öne Ek (Zorunlu Değil)' }
    ];
    var kirli = false;
    var siraNo = 0;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function ekTurAd(kod) {
        var k = parseInt(kod, 10) || 0;
        for (var i = 0; i < EK_TURLER.length; i++) if (EK_TURLER[i].kod === k) return EK_TURLER[i].ad;
        return '';
    }

    function bosSatir() { return { id: 'k' + (++siraNo), sira: '', deger: '', degerKisa: '', ekTurKod: 0 }; }

    function kirliYap(evet) {
        kirli = evet;
        $('#kirliEtiket').prop('hidden', !evet);
    }

    function metinSutun(alan, baslik, genislik, esnek) {
        return { field: alan, headerName: baslik, width: genislik, flex: esnek ? 1 : undefined, minWidth: esnek ? 200 : undefined,
            sortable: false, editable: true, tooltipField: alan, cellEditor: 'agTextCellEditor', cellClass: 'girilir',
            valueParser: function (p) { return p.newValue == null ? '' : String(p.newValue); } };
    }

    var grid = Sgb.grid('#grdKarakteristik', {
        columnDefs: [
            metinSutun('sira', 'Sıra', 100),
            metinSutun('deger', 'Karakteristik', 400, true),
            metinSutun('degerKisa', 'Karakteristik (Kısa)', 220),
            { field: 'ekTurKod', headerName: 'Ek Değer Girişi', width: 220, sortable: false, editable: true, cellClass: 'girilir',
                cellEditor: 'agSelectCellEditor',
                cellEditorParams: { values: $.map(EK_TURLER, function (t) { return t.kod; }) },
                valueFormatter: function (p) { return ekTurAd(p.value); },
                valueParser: function (p) { return parseInt(p.newValue, 10) || 0; } }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        getRowId: function (p) { return p.data.id; },
        overlayNoRowsTemplate: '<span>Listele\'ye basınız; yeni tanım için Satır Ekle\'ye basınız.</span>',
        onCellValueChanged: function (e) { if (e.newValue !== e.oldValue) kirliYap(true); }
    });

    function satirlar() {
        var liste = [];
        grid.forEachNodeAfterFilterAndSort(function (n) { liste.push(n.data); });
        return liste;
    }

    // ---------------------------------------------------------------- Listele (eski btnOku_Click)

    function oku() {
        if (kirli && !window.confirm('Kaydedilmemiş değişiklikler var. Yeniden listelenirse değişiklikler kaybolacak. Devam edilsin mi?')) return;
        var $d = $('#btnOku').prop('disabled', true);
        grid.stopEditing(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/karakteristik/liste')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', $.map(v.satirlar || [], function (s) { return $.extend(s, { id: 'k' + (++siraNo) }); }));
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı. Yeni tanım için Satır Ekle\'ye basınız.</span>');
                kirliYap(false);
                if (!(v.satirlar || []).length) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#btnOku').on('click', oku);

    // ---------------------------------------------------------------- Satır Ekle / Satır Sil (eski addRecord / deleteSelected)

    $('#btnSatirAc').on('click', function () {
        grid.stopEditing();
        var r = grid.applyTransaction({ add: [bosSatir()] });
        kirliYap(true);
        if (r && r.add && r.add[0]) {
            grid.ensureIndexVisible(r.add[0].rowIndex);
            grid.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: 'sira' });
        }
    });

    $('#btnSatirSil').on('click', function () {
        grid.stopEditing();
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Silinecek satırları işaretleyiniz.', 'warning'); return; }
        grid.applyTransaction({ remove: secili });
        kirliYap(true);
    });

    // ---------------------------------------------------------------- Yapıştır (eski Yapistir)

    var yapistirBas = -1;
    var dlgYapistir = document.getElementById('dlgYapistir');

    function yapistir(metin) {
        var gelen = String(metin || '').split('\n');
        while (gelen.length && /^\r?$/.test(gelen[gelen.length - 1])) gelen.pop();
        if (!gelen.length) { mesaj('Panoda yapıştırılacak veri yok.', 'warning'); return; }
        var eksik = yapistirBas + gelen.length - grid.getDisplayedRowCount();
        if (eksik > 0) {
            var yeni = [];
            for (var k = 0; k < eksik; k++) yeni.push(bosSatir());
            grid.applyTransaction({ add: yeni });
        }
        var liste = satirlar();
        for (var i = 0; i < gelen.length && yapistirBas + i < liste.length; i++) {
            var d = liste[yapistirBas + i];
            var degerler = gelen[i].replace(/\r$/, '').split('\t');
            for (var j = 0; j < Math.min(ALANLAR.length, degerler.length); j++) {
                d[ALANLAR[j]] = ALANLAR[j] === 'ekTurKod' ? (parseInt(degerler[j], 10) || 0) : degerler[j];
            }
        }
        grid.refreshCells({ force: true });
        kirliYap(true);
        mesaj(gelen.length + ' satır yapıştırıldı; kaydetmeyi unutmayınız.', 'info');
    }

    $('#btnYapistir').on('click', function () {
        grid.stopEditing();
        var hucre = grid.getFocusedCell();
        var bas = hucre && !hucre.rowPinned && hucre.rowIndex != null ? hucre.rowIndex : -1;
        if (bas < 0) {
            var secili = grid.getSelectedNodes();
            if (secili.length) bas = secili[0].rowIndex;
        }
        if ((bas == null || bas < 0) && !grid.getDisplayedRowCount()) bas = 0;
        if (bas == null || bas < 0) { mesaj('Lütfen satır seçiniz!', 'warning'); return; }
        yapistirBas = bas;
        var pencere = function () { $('#txtYapistir').val(''); dlgYapistir.showModal(); $('#txtYapistir').trigger('focus'); };
        if (navigator.clipboard && navigator.clipboard.readText && window.isSecureContext) navigator.clipboard.readText().then(yapistir, pencere);
        else pencere();
    });

    $('#btnYapistirTamam').on('click', function () {
        dlgYapistir.close();
        yapistir($('#txtYapistir').val());
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- Kaydet (eski btnKaydet_Click)

    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        var liste = $.map(satirlar(), function (s) {
            return { sira: $.trim(s.sira || ''), deger: $.trim(s.deger || ''), degerKisa: $.trim(s.degerKisa || ''), ekTurKod: parseInt(s.ekTurKod, 10) || 0 };
        });
        if (!window.confirm('Kaydetmek istiyor musunuz?')) return;
        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/karakteristik/kaydet', { satirlar: liste })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                kirliYap(false);
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    oku();
});
