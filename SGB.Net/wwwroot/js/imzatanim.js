// İmza Tanımları (ButceMuhasebe/ImzaTanim, eski ImzaTanim.aspx): kullanıcının adlandırılmış imza tanımları.
// Solda tanımlar; satır seçilince belge türü, tanım adı ve imza yerleri sağda okunur. Belge değişince o belgenin boş imza yerleri gelir.
// Ad soyad, ünvan / rütbe, görev ünvanı ve tarih yaz tabloda girilir. Kaydet / Sil / Yeni Kayıt.
// Ödeme Emri / MİF formundan Sgb.pencere ile açılır; form pencere kapanınca imza tanım listesini (/api/imzatanim/liste) yeniden okur.
// Uçlar: /api/imzatanim/* (Istemci/Muhasebe/ImzaTanimUclari.cs).
$(function () {
    'use strict';

    if (!$('#grdImzaTanim').length) return;

    var seciliKod = 0;

    var liste = Sgb.grid('#grdImzaTanim', {
        columnDefs: [
            { headerName: '#', width: 60, sortable: false, cellClass: 'mt-sari', valueGetter: function (p) { return p.node ? p.node.rowIndex + 1 : ''; } },
            { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 150, tooltipField: 'ad' },
            { field: 'belgeTurAd', headerName: 'Belge Tür', width: 190 }
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { if (e.data) sec(e.data.kod); }
    });

    var girilir = { editable: true, cellClass: 'mt-girilir' };
    var detay = Sgb.grid('#grdImzaDetay', {
        columnDefs: [
            { field: 'deger', headerName: '#', width: 60, cellClass: 'mt-sari' },
            { field: 'imzaYeri', headerName: 'İmza Yeri', width: 230, cellClass: 'mt-sari' },
            // Eski sayfada gizli sütun; değeri okunur ve aynen kaydedilir.
            $.extend({ field: 'kullanilmiyor', headerName: 'Kullanılmıyor', width: 120, cellDataType: 'boolean', hide: true }, girilir),
            $.extend({ field: 'adSoyad', headerName: 'Ad Soyad', flex: 1, minWidth: 170 }, girilir),
            $.extend({ field: 'unvanRutbe', headerName: 'Ünvan/Rütbe', flex: 1, minWidth: 150 }, girilir),
            $.extend({ field: 'gorevUnvani', headerName: 'Görev Ünvanı', flex: 1, minWidth: 150 }, girilir),
            $.extend({ field: 'tarihYaz', headerName: 'Tarih Yaz', width: 100, cellDataType: 'boolean' }, girilir)
        ],
        defaultColDef: { sortable: false },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true
    });

    function silDurum() { $('#btnSil').prop('disabled', seciliKod <= 0); }

    function listele(secilecek) {
        return Sgb.mt.oku('api/imzatanim/liste', null, liste).done(function (v) {
            liste.setGridOption('rowData', v.satirlar || []);
            if (secilecek) {
                var n = liste.getRowNode(String(secilecek));
                if (n) { n.setSelected(true); liste.ensureNodeVisible(n); }
            }
        });
    }

    // Detay: kod verilmişse tanımın kendisi (belge türü ve adı da), değilse seçili belge türünün boş imza yerleri.
    function detayOku(kod, belge) {
        return Sgb.mt.oku('api/imzatanim/detay', { kod: kod || 0, belge: belge || 0 }, detay).done(function (v) {
            if (kod > 0) {
                $('#itBelge').val(v.belgeTur > 0 ? String(v.belgeTur) : '');
                $('#itAd').val(v.ad || '');
            }
            detay.setGridOption('rowData', v.satirlar || []);
        });
    }

    function sec(kod) {
        seciliKod = parseInt(kod, 10) || 0;
        silDurum();
        detayOku(seciliKod, 0);
    }

    // Yeni Kayıt (eski Temizle): seçim kalkar, form ve imza yerleri boşalır.
    function temizle() {
        seciliKod = 0;
        silDurum();
        liste.deselectAll();
        $('#itBelge').val('');
        $('#itAd').val('');
        detay.setGridOption('rowData', []);
        $('#itAd').trigger('focus');
    }

    // Belge değişince o belgenin imza yerleri boş gelir (eski ddlBelge_SelectedIndexChanged → ListeleDetay(0)).
    $('#itBelge').on('change', function () {
        var belge = parseInt($(this).val(), 10) || 0;
        if (belge <= 0) { detay.setGridOption('rowData', []); return; }
        detayOku(0, belge);
    });

    $('#btnKaydet').on('click', function () {
        detay.stopEditing();
        var belge = parseInt($('#itBelge').val(), 10) || 0;
        if (belge <= 0) { Sgb.bildirim('Lütfen işlem yapacağınız belgeyi seçin.', 'warning'); return; }
        var satirlar = [];
        detay.forEachNode(function (n) { satirlar.push(n.data); });
        Sgb.mt.islem('api/imzatanim/kaydet', {
            kod: seciliKod,
            ad: $.trim($('#itAd').val()),
            belge: belge,
            satirlar: satirlar
        }).done(function (v) {
            var kod = parseInt(v.anahtar, 10) || seciliKod;
            seciliKod = kod;
            silDurum();
            listele(kod);
        });
    });

    $('#btnSil').on('click', function () {
        if (seciliKod <= 0) { Sgb.bildirim('Lütfen silmek istediğiniz imza tanımını listeden seçin.', 'warning'); return; }
        Sgb.mt.islem('api/imzatanim/sil', { kod: seciliKod }, 'Seçilen imza bilgileri silinecektir!\nBu işlemi onaylıyor musunuz?').done(function () {
            temizle();
            listele();
        });
    });

    $('#btnYeniKayit').on('click', temizle);

    silDurum();
    listele();
});
