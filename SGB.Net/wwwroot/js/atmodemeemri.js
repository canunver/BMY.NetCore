// Toplu Ödeme Emri (AlacakTakip/AlacakTakipOdemeEmri; eski aynı adlı Ext.NET sayfası). Takip İşlemleri ekranından
// Sgb.pencere('../AlacakTakip/AlacakTakipOdemeEmri', 'Toplu Ödeme Emri') ile açılır.
// Ödeme emri bağlanmamış hesap hareketleri listelenir; onay kutusuyla seçilenlerden
//   Ödeme Emri Oluştur : tek ödeme emri belgesi oluşturur (açıklama kullanıcı için saklanır, sonraki açılışta gelir),
//   Ödeme Emri Yazdır  : oluşturulacak belgenin satırlarını kaydetmeden Excel'e yazar.
// Uçlar: /api/alacaktakip/odemeemri/* (AlacakTakip/OdemeEmriUclari.cs).
$(function () {
    'use strict';

    if (!$('#grdListe').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/alacaktakip/odemeemri/';
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // ---------------------------------------------------------------- pencereler, kapat

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;
    function sor(metin, is) {
        $('#soruMetin').text(metin);
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

    // Kapat (eski hidePopWin): çerçeve (Sgb.pencere) içindeyse pencere kapanır.
    function kapat() {
        try {
            var d = window.frameElement && window.frameElement.closest('dialog');
            if (d) { d.close(); return; }
        } catch (e) { /* başka kökenden açılmış */ }
        if (window.opener) window.close();
        else window.history.back();
    }
    $('#btnKapat, #btnSonucKapat').on('click', kapat);

    // Oluşturulan ödeme emri (eski bağlantı: ButceMuhasebe/OdemeEmrimifFrame.aspx). Mümkünse en üstteki sayfada pencere açılır.
    function odemeEmriAc(yil, belgeNo) {
        var ust = Sgb;
        try {
            if (window.top !== window && window.top.Sgb && window.top.Sgb.pencere) ust = window.top.Sgb;
        } catch (e) { /* başka kökenden açılmış */ }
        ust.pencere('../ButceMuhasebe/OdemeEmrimifFrame?' + $.param({ aramaYok: 1, yil: yil, belgeNo: belgeNo }), 'Ödeme Emri ' + yil + '/' + belgeNo);
    }

    // ---------------------------------------------------------------- liste

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'TASARIMAD', headerName: 'İşlem Adı', width: 170, tooltipField: 'TASARIMAD' },
            { field: 'DOSYANO', headerName: 'Dosya No', width: 100, cellClass: 'atmo-orta' },
            { field: 'BORCLUAD', headerName: 'Borçlu Adı', flex: 1, minWidth: 190, tooltipField: 'BORCLUAD' },
            { field: 'BIRIMKODU', headerName: 'Birimi', width: 130 },
            { field: 'TOPLAMTUTAR', headerName: 'Tutar', width: 120, type: 'rightAligned',
                valueFormatter: function (p) { return p.value == null || p.value === '' ? '' : para.format(Number(p.value) || 0); } },
            { field: 'ISLEMTARIHI', headerName: 'İşlem Tarihi', width: 110,
                valueFormatter: function (p) { return p.value ? Sgb.tarih(p.value) : ''; } }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
        getRowId: function (p) { return String(p.data.KOD); },
        pagination: true,
        paginationPageSize: 150,
        paginationPageSizeSelector: [50, 150, 500],
        overlayNoRowsTemplate: '<span>Ödeme emri oluşturulacak hesap hareketi yok.</span>',
        onSelectionChanged: function () { seciliYaz(); }
    });

    function secilenKodlar() {
        return $.map(grid.getSelectedRows(), function (r) { return r.KOD; });
    }

    function seciliYaz() {
        var n = grid.getSelectedRows().length;
        $('#lblSecili').text(n ? n + ' kayıt seçili' : '');
    }

    function listele(aciklamaYaz) {
        grid.setGridOption('loading', true);
        return Sgb.al(UC + 'liste')
            .done(function (v) {
                if (aciklamaYaz && v.aciklama != null) $('#txtAciklama').val(v.aciklama);
                if (v.hata) mesaj(v.hata, 'danger');
                grid.setGridOption('rowData', v.satirlar || []);
                if (v.bilgi) mesaj(v.bilgi, 'info');
                seciliYaz();
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    // ---------------------------------------------------------------- oluştur, yazdır

    $('#btnOlustur').on('click', function () {
        var kodlar = secilenKodlar();
        if (!kodlar.length) { mesaj('Lütfen kayıt seçiniz.', 'warning'); return; }
        sor('Ödeme emri oluşturulacak. Onaylıyor musunuz?', function () {
            var $t = $('#btnOlustur, #btnYazdir').prop('disabled', true);
            grid.setGridOption('loading', true);
            Sgb.gonder(UC + 'olustur', { kodlar: kodlar, aciklama: $.trim($('#txtAciklama').val()) })
                .done(function (s) {
                    if (!s.basarili && !s.belgeNo) { mesaj(s.hata || 'Ödeme emri oluşturulamadı.', 'danger'); return; }
                    // Eski BilgiYaz: belge numarası bağlantısıyla bilgi
                    var $m = $('#sonucMetin').empty();
                    $m.append(document.createTextNode(s.basarili ? 'Ödeme emri oluşturuldu. Belge, ' : (s.hata + ' Belge: ')));
                    var a = $('<a href="#"></a>').text(s.yil + '/' + s.belgeNo).on('click', function (e) { e.preventDefault(); odemeEmriAc(s.yil, s.belgeNo); });
                    $m.append(a);
                    if (s.basarili) $m.append(document.createTextNode(' numarası verilerek başarıyla kayıt edildi.'));
                    document.getElementById('dlgSonuc').showModal();
                    listele(false);
                })
                .fail(hataGoster)
                .always(function () { $t.prop('disabled', false); grid.setGridOption('loading', false); });
        });
    });

    $('#btnYazdir').on('click', function () {
        var kodlar = secilenKodlar();
        if (!kodlar.length) { mesaj('Lütfen kayıt seçiniz.', 'warning'); return; }
        var $t = $(this).prop('disabled', true);
        Sgb.indir(UC + 'yazdir', { kodlar: kodlar.join(',') })
            .then(function () { $t.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    listele(true);
});
