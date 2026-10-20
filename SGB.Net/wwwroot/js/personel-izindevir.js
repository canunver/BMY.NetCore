// Personel İşlemleri "İzin Devir" sekmesi (eski InsanKaynaklari/PersonelIzinDevirTab.aspx; uçlar /api/personelbilgi/izindevir/...).
// Eski sayfadaki gibi: açılışta personelin yıllara göre izin devirleri gelir; Devreden ve Yıl içi hücreleri değiştirilir,
// Kaydet onay sorup yalnızca değişen satırları (eski dirtyRowsOnly) kaydeder; satırdaki sil düğmesi devri onayla siler.
$(function () {
    'use strict';

    var $kok = $('#pbIzinDevir');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var API = 'api/personelbilgi/izindevir/';
    var personelKod = parseInt($kok.data('personelKod'), 10) || 0;
    var degisenler = {}; // satır kimliği -> true

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function satirKimlik(d) { return String(d.kod) + '-' + String(d.yil); }

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });
    var onayIslem = null;
    function onay(metin, evet) {
        $('#dlgOnayMetin').text(metin);
        onayIslem = evet;
        $('#dlgOnay')[0].showModal();
        $('#btnOnayEvet').trigger('focus');
    }
    $('#btnOnayEvet').on('click', function () {
        var f = onayIslem;
        onayIslem = null;
        $('#dlgOnay')[0].close();
        if (f) f();
    });

    function silDugmesi(d) {
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'grid-komut tehlike';
        b.title = 'Devir Sil.';
        b.setAttribute('aria-label', 'Devir sil: ' + (d.yilAd || ''));
        b.innerHTML = '<span class="glyphicon glyphicon-trash" aria-hidden="true"></span>';
        b.addEventListener('click', function (e) { e.stopPropagation(); devirSil(d); });
        return b;
    }

    var sayiDuzenleyici = { min: 0, precision: 0, showStepperButtons: false };
    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { headerName: '', width: 46, sortable: false, resizable: false, cellClass: 'hucre-salt', cellRenderer: function (p) { return silDugmesi(p.data); } },
            { field: 'yilAd', headerName: 'Yıl', width: 160, cellClass: 'hucre-salt' },
            {
                field: 'devreden', headerName: 'Devreden', width: 120, type: 'numericColumn', editable: true, cellClass: 'hucre-girilir',
                cellEditor: 'agNumberCellEditor', cellEditorParams: sayiDuzenleyici
            },
            {
                field: 'yilici', headerName: 'Yıl içi', width: 120, type: 'numericColumn', editable: true, cellClass: 'hucre-girilir',
                cellEditor: 'agNumberCellEditor', cellEditorParams: sayiDuzenleyici
            },
            { headerName: '', flex: 1, sortable: false, cellClass: 'hucre-salt' }
        ],
        getRowId: function (p) { return satirKimlik(p.data); },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        onCellValueChanged: function (e) {
            if (e.newValue === null || e.newValue === undefined || e.newValue === '') e.node.setDataValue(e.colDef.field, 0);
            degisenler[satirKimlik(e.data)] = true;
        },
        overlayNoRowsTemplate: '<span class="text-muted">İzin devir bilgisi yok</span>'
    });

    // Eski Listele
    function listele(ilk) {
        if (!ilk) grid.setGridOption('loading', true);
        return Sgb.al(API + 'liste', { personelKod: personelKod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                degisenler = {};
                grid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    function degisenSatirlar() {
        var satirlar = [];
        grid.forEachNode(function (n) {
            if (!degisenler[satirKimlik(n.data)]) return;
            satirlar.push({
                kod: n.data.kod,
                personelKod: n.data.personelKod,
                yil: n.data.yil,
                devreden: parseInt(n.data.devreden, 10) || 0,
                yilici: parseInt(n.data.yilici, 10) || 0
            });
        });
        return satirlar;
    }

    // Eski btnKaydet_Click
    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        onay('Kayıt işlemi yapılacak. Onaylıyor musunuz?', function () {
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder(API + 'kaydet', { personelKod: personelKod, satirlar: degisenSatirlar() })
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                    mesaj(v.bilgi, 'success');
                    listele();
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // Eski DevirSil
    function devirSil(d) {
        onay('Devir bilgisi silinecek. Onaylıyor musunuz?', function () {
            Sgb.gonder(API + 'sil', { personelKod: personelKod, kod: d.kod, yil: d.yil })
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                    mesaj(v.bilgi, 'success');
                    listele();
                })
                .fail(hataGoster);
        });
    }

    if (personelKod > 0) listele(true);
});
