// Kalkınma Bölgeleri (ButceHazirlikv2/KalkinmaBolgeleriTanimForm): üstte bölge kodu ve adı, altta bölgedeki iller.
// Bölge kodu tanımlı bölgelerden seçilebilir ya da yeni kod yazılabilir (eski ComboBox ForceSelection=false); Oku bölgenin adını ve
// illerini getirir. İller tabloda Satır Ekle ile açılıp listeden seçilir. Kaydet bölgeyi ve illerini yazar (eski sayfa gibi onay sorar).
$(function () {
    'use strict';

    if (!$('#grdIller').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var iller = [];      // { kod, ad } bütün iller
    var kirli = false;
    var siraNo = 0;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function ilAd(kod) {
        for (var i = 0; i < iller.length; i++) if (iller[i].kod === kod) return iller[i].kod + '-' + iller[i].ad;
        return kod || '';
    }

    function kirliYap(evet) {
        kirli = evet;
        $('#kirliEtiket').prop('hidden', !evet);
    }

    var grid = Sgb.grid('#grdIller', {
        columnDefs: [
            { field: 'kod', headerName: 'İl', flex: 1, minWidth: 220, editable: true, cellClass: 'girilir', sortable: false,
                cellEditor: 'agSelectCellEditor',
                cellEditorParams: function () { return { values: $.map(iller, function (x) { return x.kod; }) }; },
                valueFormatter: function (p) { return ilAd(p.value); },
                tooltipValueGetter: function (p) { return ilAd(p.value); },
                valueParser: function (p) { return p.newValue; } }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        getRowId: function (p) { return p.data.id; },
        overlayNoRowsTemplate: '<span>Bölge kodunu yazıp Oku\'ya basınız; yeni bölge için illeri Satır Ekle ile ekleyiniz.</span>',
        onCellValueChanged: function (e) { if (e.newValue !== e.oldValue) kirliYap(true); }
    });

    function bolgeleriYaz(bolgeler) {
        var $l = $('#lstBolgeler').empty();
        $.each(bolgeler || [], function (i, b) { $('<option></option>').attr('value', b.kod).text(b.ad).appendTo($l); });
    }

    function satirlar() {
        var liste = [];
        grid.forEachNodeAfterFilterAndSort(function (n) { liste.push(n.data); });
        return liste;
    }

    function degisiklikVarsaSor() {
        return !kirli || window.confirm('Kaydedilmemiş değişiklikler var. Devam edilirse değişiklikler kaybolacak. Devam edilsin mi?');
    }

    // ---------------------------------------------------------------- açılış: iller ve tanımlı bölgeler

    Sgb.al('api/kalkinmabolge/tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            iller = v.iller || [];
            bolgeleriYaz(v.bolgeler);
            grid.refreshCells({ force: true });
        })
        .fail(hataGoster);

    // ---------------------------------------------------------------- Oku (eski btnOku_Click)

    $('#frmBolge').on('submit', function (e) {
        e.preventDefault();
        var kod = $.trim($('#txtBolgeKod').val());
        if (!kod) { mesaj('Bölge kodunu yazınız.', 'warning'); $('#txtBolgeKod').trigger('focus'); return; }
        if (!degisiklikVarsaSor()) return;
        var $d = $('#btnOku').prop('disabled', true);
        grid.stopEditing(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/kalkinmabolge/oku', { kod: kod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                if (v.bulunamadi) {
                    grid.setGridOption('rowData', []);
                    kirliYap(false);
                    mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
                    return;
                }
                $('#txtBolgeKod').val(v.bolgeKod);
                $('#txtBolgeAd').val(v.bolgeAd);
                grid.setGridOption('rowData', $.map(v.iller || [], function (il) { return { id: 'i' + (++siraNo), kod: il.kod }; }));
                kirliYap(false);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    });

    // Listeden bilinen bir bölge seçilince okunur.
    $('#txtBolgeKod').on('change', function () {
        var kod = $.trim($(this).val());
        if (kod && $('#lstBolgeler option').filter(function () { return this.value === kod; }).length) $('#frmBolge').trigger('submit');
    });

    $('#btnYeni').on('click', function () {
        if (!degisiklikVarsaSor()) return;
        $('#txtBolgeKod, #txtBolgeAd').val('');
        grid.setGridOption('rowData', []);
        kirliYap(false);
        $('#txtBolgeKod').trigger('focus');
    });

    $('#txtBolgeAd').on('input', function () { kirliYap(true); });

    // ---------------------------------------------------------------- Satır Ekle / Satır Sil

    $('#btnSatirAc').on('click', function () {
        grid.stopEditing();
        var r = grid.applyTransaction({ add: [{ id: 'i' + (++siraNo), kod: '' }] });
        kirliYap(true);
        if (r && r.add && r.add[0]) {
            grid.ensureIndexVisible(r.add[0].rowIndex);
            grid.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: 'kod' });
        }
    });

    $('#btnSatirSil').on('click', function () {
        grid.stopEditing();
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Silinecek satırları işaretleyiniz.', 'warning'); return; }
        grid.applyTransaction({ remove: secili });
        kirliYap(true);
    });

    // ---------------------------------------------------------------- Kaydet (eski btnKaydet_Click)

    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        var istek = {
            bolgeKod: $.trim($('#txtBolgeKod').val()),
            bolgeAd: $.trim($('#txtBolgeAd').val()),
            iller: $.map(satirlar(), function (s) { return s.kod ? s.kod : null; })
        };
        if (!istek.bolgeKod) { mesaj('Bölge kodu boş olamaz!', 'warning'); $('#txtBolgeKod').trigger('focus'); return; }
        if (!istek.bolgeAd) { mesaj('Bölge Adı boş olamaz!', 'warning'); $('#txtBolgeAd').trigger('focus'); return; }
        if (!istek.iller.length) { mesaj('Bölge illeri boş olamaz!', 'warning'); return; }
        if (!window.confirm('Kaydetmek istiyor musunuz?')) return;
        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/kalkinmabolge/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                kirliYap(false);
                if (s.bolgeler) bolgeleriYaz(s.bolgeler);
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });
});
