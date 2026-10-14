// Veritabanı Index İşlemleri (eski Evrak/EvrakVeritabaniIndex.aspx). Uçlar: /api/vtindex/baglantilar, liste, islem, olustur
// Bağlantı satırı ya da index tipi seçilince liste okunur. Index Oluştur sekmesi yalnızca evrak veri tabanında açılır; seçilen
// indexler teker teker oluşturulur ve ilerleme pencerede gösterilir.
(function ($) {
    'use strict';

    var sayi3 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 3, maximumFractionDigits: 3 });
    var sayi2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var olusturAcik = false;
    var okundu = false;

    function hata(x) {
        if (x.status !== 401) Sgb.bildirim(x.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    var gridListe = Sgb.grid('#grdIndexListe', {
        columnDefs: [
            { headerName: '', valueGetter: function (p) { return p.node.rowIndex + 1; }, width: 60, sortable: false },
            { field: 'tabloAd', headerName: 'Tablo Ad', width: 220 },
            { field: 'indexAd', headerName: 'Index Ad', flex: 1, minWidth: 220 },
            { field: 'indexTip', headerName: 'Index Tip', width: 130, cellClass: 'text-center' },
            { field: 'indexSize', headerName: 'Index Boyutu (MB)', width: 150, type: 'rightAligned', valueFormatter: function (p) { return sayi3.format(p.value || 0); } },
            { field: 'fragmentation', headerName: 'Fragmentation (%)', width: 150, type: 'rightAligned', valueFormatter: function (p) { return sayi2.format(p.value || 0); } }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
        getRowId: function (p) { return p.data.tabloAd + '|' + p.data.indexAd; },
        pagination: true,
        paginationPageSize: 25,
        paginationPageSizeSelector: [25, 100, 250, 500, 1000],
        overlayNoRowsTemplate: '<span>Listelemek için bağlantı satırını seçiniz.</span>'
    });

    var gridOlustur = Sgb.grid('#grdIndexOlustur', {
        columnDefs: [
            { headerName: '', valueGetter: function (p) { return p.node.rowIndex + 1; }, width: 60, sortable: false },
            { field: 'tabloAd', headerName: 'Tablo Ad', width: 200 },
            { field: 'indexAd', headerName: 'Index Ad', flex: 1, minWidth: 220 },
            { field: 'indexTip', headerName: 'Index Tip', width: 110, cellClass: 'text-center' },
            { field: 'kolon', headerName: 'Kolon', width: 200 },
            { field: 'include', headerName: 'Include', width: 200 },
            { field: 'durum', headerName: 'Index Var Mı?', width: 120, cellClass: 'text-center',
              cellRenderer: function (p) { return p.value === 1 ? '<span class="glyphicon glyphicon-ok durum-var" aria-label="Var"></span>' : ''; } }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
        getRowId: function (p) { return p.data.indexAd; },
        overlayNoRowsTemplate: '<span>Evrak veri tabanı Nonclustered listelendiğinde tanımlı indexler burada görünür.</span>'
    });

    // ---------------------------------------------------------------- sekmeler

    function sekmeAc(ad) {
        if (ad === 'olustur' && !olusturAcik) return;
        $('.vt-index .nav-tabs li').each(function () {
            var $a = $(this).find('a');
            var secili = $a.data('sekme') === ad;
            $(this).toggleClass('active', secili);
            $a.attr('aria-selected', secili ? 'true' : 'false');
        });
        $('.vt-index .sekme').each(function () { $(this).prop('hidden', $(this).data('sekme') !== ad); });
    }

    $('.vt-index .nav-tabs a').on('click', function (e) { e.preventDefault(); sekmeAc($(this).data('sekme')); });

    function olusturDurumu(acik) {
        olusturAcik = acik;
        $('#sekmeOlustur').toggleClass('disabled', !acik);
        $('#btnIndexOlustur').prop('disabled', !acik);
        if (!acik) sekmeAc('liste');
    }

    // ---------------------------------------------------------------- listeleme

    function listeyiBosalt() {
        okundu = false;
        gridListe.setGridOption('rowData', []);
        gridOlustur.setGridOption('rowData', []);
        $('.islem').prop('disabled', true);
        $('#lblVeritabani').text('');
        olusturDurumu(false);
    }

    function listele() {
        var baglanti = $('#ddlBaglantiSatiri').val();
        if (!baglanti) { listeyiBosalt(); return $.Deferred().resolve().promise(); }
        var tip = $('#ddlIndexTip').val();
        gridListe.setGridOption('loading', true);
        return Sgb.al('api/vtindex/liste', { baglanti: baglanti, tip: tip }).done(function (v) {
            if (!v.basarili) {
                listeyiBosalt();
                Sgb.bildirim(v.hata, 'warning');
                return;
            }
            okundu = true;
            $('#lblVeritabani').text(v.veritabani || '');
            gridListe.setGridOption('rowData', v.satirlar || []);
            gridListe.deselectAll();
            $('.islem').prop('disabled', false);
            // Drop yalnızca Nonclustered listelenince açık (eski btnDrop).
            $('#btnDrop').prop('disabled', tip !== 'Nonclustered');
            if (v.yeniler) gridOlustur.setGridOption('rowData', v.yeniler);
            olusturDurumu(!!v.olusturAcik);
        }).fail(function (x) { listeyiBosalt(); hata(x); })
          .always(function () { gridListe.setGridOption('loading', false); });
    }

    $('#ddlBaglantiSatiri').on('change', listele);
    $('#ddlIndexTip').on('change', function () { if ($('#ddlBaglantiSatiri').val()) listele(); });

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
        if (soruIsi) soruIsi();
        soruIsi = null;
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- rebuild / reorganize / drop

    $('.islem').on('click', function () {
        if (!okundu) return;
        var islem = $(this).data('islem');
        var ad = $(this).data('ad');
        var secilen = gridListe.getSelectedRows();
        if (!secilen.length) { Sgb.bildirim('Lütfen işlem yapmak istediğiniz indexleri seçiniz.', 'warning'); return; }
        sor('Seçilen ' + secilen.length + ' index ' + ad + ' edilecektir. Bu işlemi onaylıyor musunuz?', ad, function () {
            var istek = {
                baglanti: $('#ddlBaglantiSatiri').val(),
                islem: islem,
                indexler: $.map(secilen, function (s) { return { tabloAd: s.tabloAd, indexAd: s.indexAd }; })
            };
            var $d = $('.islem').prop('disabled', true);
            gridListe.setGridOption('loading', true);
            Sgb.gonder('api/vtindex/islem', istek).done(function (v) {
                if (!v.basarili) { Sgb.bildirim(v.hata, 'warning'); return; }
                Sgb.bildirim(v.bilgi, 'info');
                listele();
            }).fail(hata).always(function () {
                gridListe.setGridOption('loading', false);
                $d.prop('disabled', false);
                $('#btnDrop').prop('disabled', $('#ddlIndexTip').val() !== 'Nonclustered');
            });
        });
    });

    // ---------------------------------------------------------------- index oluştur

    var dlgIlerleme = document.getElementById('dlgIlerleme');
    // İşlem sürerken pencere Esc ile kapanmasın.
    dlgIlerleme.addEventListener('cancel', function (e) { e.preventDefault(); });

    function ilerleme(sira, toplam, ad) {
        var yuzde = toplam ? Math.round(sira / toplam * 100) : 0;
        $('#prgIlerleme').css('width', yuzde + '%').attr('aria-valuenow', yuzde).text(sira + '/' + toplam);
        $('#lblIlerleme').text(ad || '');
    }

    $('#btnIndexOlustur').on('click', function () {
        if (!olusturAcik) return;
        var secilen = gridOlustur.getSelectedRows();
        if (!secilen.length) { Sgb.bildirim('Lütfen işlem yapmak istediğiniz indexleri seçiniz.', 'warning'); return; }
        var liste = $.grep(secilen, function (s) { return s.durum !== 1; });
        if (!liste.length) { Sgb.bildirim('Lütfen listeden daha önce oluşturulmamış indexleri seçiniz.', 'warning'); return; }

        sor('Seçilen ' + liste.length + ' index veri tabanında oluşturulacaktır. Bu işlemi onaylıyor musunuz?', 'Oluştur', function () {
            var baglanti = $('#ddlBaglantiSatiri').val();
            var $d = $('#btnIndexOlustur').prop('disabled', true);
            ilerleme(0, liste.length, '');
            dlgIlerleme.showModal();

            function sonraki(i) {
                if (i >= liste.length) {
                    dlgIlerleme.close();
                    $d.prop('disabled', false);
                    $('#ddlIndexTip').val('Nonclustered');
                    listele();
                    Sgb.bildirim('Index oluşturma işlemi tamamlandı.', 'info');
                    return;
                }
                ilerleme(i + 1, liste.length, liste[i].indexAd);
                Sgb.gonder('api/vtindex/olustur', { baglanti: baglanti, indexAd: liste[i].indexAd }).done(function (v) {
                    if (!v.basarili) { bitir(v.hata); return; }
                    sonraki(i + 1);
                }).fail(function (x) { bitir(x.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Index oluşturulamadı.'); });
            }

            function bitir(metin) {
                dlgIlerleme.close();
                $d.prop('disabled', false);
                Sgb.bildirim(metin, 'danger');
                listele();
            }

            sonraki(0);
        });
    });

    // ---------------------------------------------------------------- açılış

    // Bağlantı satırları okunur; adreste ?veritabani= varsa o veri tabanı seçilip listelenir (eski setValueAndFireSelect).
    Sgb.al('api/vtindex/baglantilar').done(function (v) {
        var $s = $('#ddlBaglantiSatiri');
        var istenen = (new URLSearchParams(window.location.search).get('veritabani') || '').toLowerCase();
        var secilecek = '';
        $.each(v.satirlar || [], function (i, b) {
            $('<option></option>').val(b.baglanti).text(b.veritabani).attr('title', b.baglanti).appendTo($s);
            if (istenen && b.veritabani.toLowerCase() === istenen) secilecek = b.baglanti;
        });
        if (v.hata) Sgb.bildirim(v.hata, 'warning');
        if (secilecek) { $s.val(secilecek); listele(); }
    }).fail(hata);
})(jQuery);
