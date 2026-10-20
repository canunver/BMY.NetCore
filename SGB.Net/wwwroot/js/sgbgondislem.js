// Bütçe İşlemleri Düzeltme (ButceMuhasebe/SGBGondIslemDuzeltme; eski aynı adlı Ext.NET sayfası: Gönderme/Tenkis - Bütçe İşlemi
// listeleme/düzeltme). İşlem türüne göre çift işlemli belgeler ya da tenkis / gönderme belgesi ile masraf planı bütçe işlemi
// arasındaki tutarsızlıklar listelenir. Satırdaki tuş o satırı düzeltir (çift işlemlide "Bağı Kopart", diğerlerinde
// "B.İ. Üret/Yaz"); "İşlem Yap" seçili satırları sırayla düzeltir ve ilerlemeyi yazar. Uçlar: /api/sgbgondislem/*.
$(function () {
    'use strict';

    var $kok = $('.sgb-gond-islem');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/sgbgondislem/';
    var IPUCU = '<span>Kıstasları seçip Listele\'ye basınız.</span>';
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var sonKriter = null;
    var calisiyor = false;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function para(v) { return v === null || v === undefined || v === '' ? '' : tutar2.format(Number(v) || 0); }
    function hataYaz(metin) { Sgb.mesaj('#kutuHata', metin || '', 'danger'); listeYay(); }

    // ---------------------------------------------------------------- onay sorusu

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

    // ---------------------------------------------------------------- tablo

    // Öğeyi sayfa alanının (.sayfa-icerik) kalan yüksekliğine yayar; sayfada dikey kaydırma çıkmaz, kaydırma öğenin içinde kalır.
    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var kap = el.closest('.sayfa-icerik') || document.body, cs = getComputedStyle(kap);
        var kaydirir = /(auto|scroll)/.test(cs.overflowY);
        var kr = kap.getBoundingClientRect(), kay = kaydirir ? kap.scrollTop : 0;
        var gorunen = kaydirir ? kap.clientHeight : window.innerHeight - kr.top;
        var ust = el.getBoundingClientRect().top - kr.top + kay;
        var son = 0;
        $(kap).find('.og-liste .bolum-liste').each(function () {
            son = Math.max(son, this.getBoundingClientRect().bottom - kr.top + kay + parseFloat(getComputedStyle(this).marginBottom || 0));
        });
        son += parseFloat(cs.paddingBottom || 0);
        var bosluk = Math.max(0, son - (ust + el.offsetHeight));
        el.style.height = Math.max(enAz, Math.floor(gorunen - ust - bosluk - 1)) + 'px';
    }

    // Eski BelgeDurumBul / BelgeTurBul.
    function durumAd(x) { return x.value === 5 ? 'Onaylı' : x.value === 9 ? 'İptal' : (x.value || x.value === 0 ? String(x.value) : ''); }
    function turAd(x) { return x.value === 3 ? 'Gönd.' : x.value === 4 ? 'Tenkis' : (x.value || x.value === 0 ? String(x.value) : ''); }
    function sayi(x) { return x.value ? String(x.value) : ''; }

    // Satır tuşu (eski komutAyarla: çift işlemlide yalnız Bağı Kopart, diğerlerinde yalnız B.İ. Üret/Yaz).
    function islemHucre(x) {
        if (!x.data) return '';
        var kopar = x.data.tur === 0;
        return $('<button type="button" class="btn btn-default btn-xs sgb-satir-islem"></button>')
            .append($('<span aria-hidden="true"></span>').addClass('glyphicon ' + (kopar ? 'glyphicon-scissors' : 'glyphicon-share')))
            .append(document.createTextNode(kopar ? ' Bağı Kopart' : ' B.İ. Üret/Yaz'))
            .on('click', function (e) {
                e.stopPropagation();
                if (calisiyor) return;
                var d = x.data;
                sor((kopar ? 'Bağı koparılacak' : 'Bütçe işlemi üretilecek / yazılacak') + ' (' + (d.belgeNo || '') + '). Onaylıyor musunuz?', function () {
                    calisiyor = true;
                    hataYaz('');
                    islemGonder(d)
                        .done(function (v) {
                            if (!v.basarili) { mesaj(v.hata || 'İşlem yapılamadı.', 'danger'); return; }
                            mesaj(v.bilgi, 'success');
                            listele(true);
                        })
                        .fail(hataGoster)
                        .always(function () { calisiyor = false; });
                });
            })[0];
    }

    var grid = Sgb.grid('#grdFark', {
        columnDefs: [
            {
                headerName: 'Gönderme / Tenkis', children: [
                    { field: 'yil', headerName: 'Yıl', minWidth: 60, flex: 60, valueFormatter: sayi },
                    { field: 'belgeTur', headerName: 'Tür', minWidth: 65, flex: 65, valueFormatter: turAd },
                    { field: 'belgeNo', headerName: 'Belge No', minWidth: 110, flex: 120 },
                    { field: 'muhasebeKod', headerName: 'Say.', minWidth: 65, flex: 70, type: 'rightAligned', valueFormatter: sayi },
                    { field: 'birimKod', headerName: 'Birim', minWidth: 60, flex: 60 },
                    { field: 'gondDurum', headerName: 'Durum', minWidth: 70, flex: 70, valueFormatter: durumAd },
                    { field: 'gondToplam', headerName: 'Toplam', minWidth: 100, flex: 110, type: 'rightAligned', valueFormatter: function (x) { return para(x.value); } }
                ]
            },
            {
                headerName: 'Bütçe İşlemi', children: [
                    { field: 'islYil', headerName: 'Yıl', minWidth: 60, flex: 60, valueFormatter: sayi },
                    { field: 'islBelgeTur', headerName: 'Tür', minWidth: 65, flex: 65, valueFormatter: turAd },
                    { field: 'islBelgeNo', headerName: 'Belge No', minWidth: 110, flex: 120 },
                    { field: 'islDurum', headerName: 'Durum', minWidth: 70, flex: 70, valueFormatter: durumAd },
                    { field: 'revizeYil', headerName: 'Revize Yıl', minWidth: 70, flex: 70, hide: true },
                    { field: 'revizeNo', headerName: 'Revize No', minWidth: 75, flex: 75, type: 'rightAligned', valueFormatter: sayi },
                    { field: 'islToplam', headerName: 'Toplam', minWidth: 100, flex: 110, type: 'rightAligned', valueFormatter: function (x) { return para(x.value); } }
                ]
            },
            { field: 'fark', headerName: 'Fark', minWidth: 100, flex: 110, type: 'rightAligned', valueFormatter: function (x) { return para(x.value); } },
            { colId: 'islem', headerName: 'İşlem', minWidth: 125, flex: 125, sortable: false, cellRenderer: islemHucre }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        overlayNoRowsTemplate: IPUCU
    });

    // ---------------------------------------------------------------- liste (eski Listele)

    function kriter() {
        return {
            tur: parseInt($('#ddlSorguTur').val(), 10) || 0,
            yil: parseInt($('#ddlYil').val(), 10) || 0,
            muhasebe: parseInt($.trim($('#txtSaymanlik').val()), 10) || 0,
            eko: $.trim($('#txtEko').val()),
            peb: $.trim($('#txtPeb').val())
        };
    }

    function listele(sessiz) {
        var k = sessiz && sonKriter ? sonKriter : kriter();
        if (!(k.yil > 0)) { mesaj('Yıl giriniz.', 'warning'); return; }
        var $d = $('#btnListele, #btnIslemYap').prop('disabled', true);
        grid.deselectAll();
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', k)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                sonKriter = k;
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
                grid.setGridOption('rowData', v.satirlar || []);
                var n = (v.satirlar || []).length;
                $('#lblOzet').text(n ? n + ' satır' + (n >= 500 ? ' (en çok 500 satır gösterilir)' : '') : '');
                if (!sessiz && !n) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', calisiyor); grid.setGridOption('loading', false); });
    }

    $('#frmKriter').on('submit', function (e) { e.preventDefault(); if (!calisiyor) { hataYaz(''); listele(false); } });

    // ---------------------------------------------------------------- işlemler

    function islemGonder(d) {
        return Sgb.gonder(UC + 'islem', { tur: d.tur, yil: d.yil, belgeTur: d.belgeTur, belgeNo: d.belgeNo, revizeYil: d.revizeYil, revizeNo: d.revizeNo });
    }

    // Seçili satırlar sırayla işlenir; ilerleme "yapılan/toplam hata:n" olarak yazılır (eski IslemDurumBul).
    $('#btnIslemYap').on('click', function () {
        if (calisiyor) return;
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('İşlem yapılacak kayıtları seçiniz.', 'warning'); return; }
        sor('Seçili kayıtlar üzerinde işlem yapılacaktır. Onaylıyor musunuz?', function () {
            calisiyor = true;
            hataYaz('');
            var $d = $('#btnListele, #btnIslemYap').prop('disabled', true);
            var toplam = secili.length, yapilan = 0, hata = 0, hatalar = [];
            function durumYaz() { $('#lblDurum').text(yapilan + '/' + toplam + ' hata:' + hata); }
            function bitir() {
                calisiyor = false;
                $d.prop('disabled', false);
                $('#lblDurum').text('');
                if (hatalar.length) hataYaz(hatalar.join('\n'));
                mesaj(toplam + ' kayıt işlendi' + (hata ? ', ' + hata + ' hata' : '') + '.', hata ? 'warning' : 'success');
                listele(true);
            }
            function sonraki() {
                if (yapilan >= toplam) { bitir(); return; }
                var d = secili[yapilan];
                durumYaz();
                islemGonder(d)
                    .done(function (v) {
                        if (!v.basarili) { hata++; hatalar.push((d.belgeNo || '') + ' / ' + (d.revizeNo || '') + ': ' + (v.hata || 'İşlem yapılamadı.')); }
                    })
                    .fail(function (xhr) {
                        hata++;
                        hatalar.push((d.belgeNo || '') + ' / ' + (d.revizeNo || '') + ': ' + (xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.'));
                    })
                    .always(function () { yapilan++; durumYaz(); sonraki(); });
            }
            sonraki();
        });
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al(UC + 'tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            Sgb.yilKutusu('#ddlYil', t.yillar, t.yil);
        })
        .fail(hataGoster);

    function listeYay() { yuksekligeYay(document.getElementById('grdFark'), 250); }
    $(window).on('resize', listeYay);
    listeYay();
});
