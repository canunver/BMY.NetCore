// Yatırım Projeleri Deflatörü (ButceHazirlikv2/DeflatorForm; eski DeflatorForm.aspx).
// Listele yılın ve senaryonun sektör deflatörlerini getirir; Deflatör sütunu girilir (4 ondalık). Kaydet tablodaki bütün satırları
// listelenen yıl ve senaryoya yazar (eski getRowsValues; sıfır olanlar saklanmaz). Kaydı yalnızca sistem yöneticisi yapabilir.
$(function () {
    'use strict';

    if (!$('#grdDeflator').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var okunan = null; // { yil, senaryo, senaryoAd }
    var yazabilir = false;
    var degisti = false;
    var ondalik = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 4, maximumFractionDigits: 4 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) {
        $('#btnOku').prop('disabled', evet);
        $('#btnKaydet').prop('disabled', evet || !okunan || !yazabilir);
    }

    // "1,2345" ya da "1.234,5" -> sayı. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '');
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? Math.round(n * 10000) / 10000 : null;
    }

    var grid = Sgb.grid('#grdDeflator', {
        columnDefs: [
            { field: 'sektorKod', headerName: 'Sektor Kod', width: 110, cellClass: 'kapali' },
            { field: 'sektorAd', headerName: 'Sektor Ad', flex: 1, minWidth: 260, tooltipField: 'sektorAd', cellClass: 'kapali' },
            {
                field: 'deflator', headerName: 'Deflatör', width: 130, headerClass: 'sayi', sortable: false,
                editable: function () { return yazabilir; },
                cellEditor: 'agTextCellEditor',
                cellClass: function (p) { return ['sayi', yazabilir ? 'girilir' : 'kapali'].concat(p.data && p.data._degisti ? ['degisti'] : []); },
                valueFormatter: function (p) { return p.value == null || p.value === '' ? '' : ondalik.format(p.value); },
                valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; }
            }
        ],
        overlayNoRowsTemplate: '<span class="ag-overlay-no-rows-center">Yıl ve senaryoyu seçip Listele\'ye basınız.</span>',
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        onCellValueChanged: function (e) {
            if (e.oldValue === e.newValue) return;
            e.data._degisti = true;
            degisti = true;
            e.api.refreshCells({ rowNodes: [e.node], force: true });
        }
    });

    function satirlar() {
        var liste = [];
        grid.forEachNode(function (n) { liste.push(n.data); });
        return liste;
    }

    $(window).on('beforeunload', function (e) {
        if (!degisti) return;
        e.preventDefault();
        return '';
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/deflator/tanim').done(function (v) {
        if (v.hata) { mesaj(v.hata, 'danger'); return; }
        yazabilir = !!v.yazabilir;
        $('#txtYil').val(v.yil);
        var $s = $('#ddlSenaryo').empty();
        $.each(v.asamalar || [], function (i, o) { $('<option></option>').val(o.kod).text(o.ad).appendTo($s); });
        if ($s.children().length) $s.prop('selectedIndex', 0);
        if (!yazabilir) $('#btnKaydet').attr('title', 'Deflatörleri yalnızca sistem yöneticisi kaydedebilir.');
    }).fail(hataGoster);

    // ---------------------------------------------------------------- Listele / Kaydet

    function oku() {
        var yil = parseInt($('#txtYil').val(), 10) || 0;
        if (yil < 1000) { mesaj('Yılı yazınız.', 'warning'); $('#txtYil').trigger('focus'); return; }
        var senaryo = $('#ddlSenaryo').val() || '';
        if (!senaryo) { mesaj('Senaryo seçiniz.', 'warning'); return; }
        if (degisti && !window.confirm('Kaydedilmemiş değişiklikler kaybolacak. Devam etmek istiyor musunuz?')) return;
        mesgul(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/deflator/liste', { yil: yil, senaryo: senaryo })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var liste = v.satirlar || [];
                grid.setGridOption('rowData', liste);
                degisti = false;
                okunan = { yil: yil, senaryo: senaryo, senaryoAd: $('#ddlSenaryo option:selected').text() };
                $('#lblOkunan').text(okunan.yil + ' yılı, ' + okunan.senaryoAd + ': ' + liste.length + ' sektör');
                if (!liste.length) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); mesgul(false); });
    }

    $('#btnOku').on('click', oku);
    $('#frmSecim').on('submit', function (e) { e.preventDefault(); oku(); });

    $('#btnKaydet').on('click', function () {
        if (!okunan) return;
        grid.stopEditing();
        var liste = satirlar();
        if (!liste.length) { mesaj('Kaydedilecek satır yok.', 'warning'); return; }
        if (!window.confirm('Kaydetmek İstiyor musunuz?')) return;
        var istek = {
            yil: okunan.yil, senaryo: okunan.senaryo,
            satirlar: $.map(liste, function (d) { return { sektorKod: d.sektorKod, deflator: Number(d.deflator) || 0 }; })
        };
        mesgul(true);
        Sgb.gonder('api/deflator/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                $.each(liste, function (i, d) { delete d._degisti; });
                degisti = false;
                grid.refreshCells({ force: true });
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });
});
