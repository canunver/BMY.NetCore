// Personel İzin Devir Bilgileri (eski InsanKaynaklari/PersonelIzinDevir.aspx; uçlar /api/izindevir/...).
// Eski sayfadaki gibi: yıl, birim, personel tipi ve arama ile Listele; Devreden ve Yıl içi hücreleri değiştirilir, Kaydet onay sorup
// yalnızca değişen satırları (eski dirtyRowsOnly) kaydeder; Devreden İzinleri Aktar bir önceki yıldan kalan günleri seçilen yıla aktarır.
$(function () {
    'use strict';

    if (!$('#izdAna').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var U = 'api/izindevir/';
    var birim = { kod: 0, ad: '' };
    var listelenenYil = 0;    // eski hdnYil: son listelenen yıl, Kaydet bunu kullanır
    var degisenler = {};      // satır kimliği -> true

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function satirKimlik(d) { return String(d.personelKod) + '-' + String(d.yil) + '-' + String(d.kod); }
    function yil() { return parseInt($('#txtYil').val(), 10) || 0; }

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    $('#btnYardim').on('click', function () {
        var $n = $('#izdYardim'), acik = !$n.prop('hidden');
        $n.prop('hidden', acik);
        $(this).attr('aria-expanded', !acik);
        gridYay();
    });

    // ---------------------------------------------------------------- onay penceresi

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

    // ---------------------------------------------------------------- personel tipi (eski ExtCokluSecim)

    function cokluSecim(el) {
        var $el = $(el).addClass('dropdown');
        var $dugme = $('<button type="button" class="btn btn-default btn-sm form-control izd-coklu-dugme" aria-haspopup="true" aria-expanded="false"></button>').appendTo($el);
        var $metin = $('<span class="izd-coklu-metin"></span>').appendTo($dugme);
        $('<span class="caret"></span>').appendTo($dugme);
        var $liste = $('<ul class="dropdown-menu izd-coklu-liste"></ul>').appendTo($el);
        var ogeler = [];

        function metinYaz() {
            var adlar = $.map(ogeler, function (o) { return o.secili ? o.ad : null; });
            $metin.text(adlar.length === 0 ? ($el.data('bos') || '') : adlar.length > 1 ? '[' + adlar.join(', ') + ']' : adlar[0]);
            $dugme.attr('title', adlar.join(', '));
        }
        function ciz() {
            $liste.empty();
            if (!ogeler.length) $('<li class="izd-coklu-bos">Seçenek yok</li>').appendTo($liste);
            $.each(ogeler, function (i, o) {
                var $l = $('<label></label>').appendTo($('<li></li>').appendTo($liste));
                $('<input type="checkbox" />').prop('checked', !!o.secili).on('change', function () {
                    o.secili = this.checked;
                    metinYaz();
                }).appendTo($l);
                $l.append(document.createTextNode(' ' + o.ad));
            });
            metinYaz();
        }
        $liste.on('click', function (e) { e.stopPropagation(); });
        $dugme.on('click', function (e) {
            e.stopPropagation();
            var acik = $el.hasClass('open');
            $el.toggleClass('open', !acik);
            $dugme.attr('aria-expanded', !acik);
        });
        $(document).on('click', function () { $el.removeClass('open'); $dugme.attr('aria-expanded', false); });
        ciz();
        return {
            doldur: function (liste) { ogeler = $.map(liste || [], function (o) { return { kod: o.kod, ad: o.ad, secili: false }; }); ciz(); },
            secilenler: function () { return $.map(ogeler, function (o) { return o.secili ? o.kod : null; }); }
        };
    }
    var tur = cokluSecim('#cokTur');

    // ---------------------------------------------------------------- yıl (eski ExtIzinDonem: yıl ve dönem adı)

    var donemZaman = null;
    $('#txtYil').on('input change', function () {
        clearTimeout(donemZaman);
        donemZaman = setTimeout(function () {
            var y = yil();
            if (!y) { $('#lblDonemAd').text(''); return; }
            Sgb.al('api/izintur/donem', { yil: y }).done(function (v) { if (yil() === y) $('#lblDonemAd').text(v.ad || ''); });
        }, 250);
    });

    // ---------------------------------------------------------------- birim seçimi (eski ExtOrgSecim)

    function birimYaz(kod, ad) {
        birim = { kod: kod || 0, ad: ad || '' };
        $('#txtBirimAd').val(birim.ad).attr('title', birim.ad);
    }

    var dlgBirim = document.getElementById('dlgBirim');
    var birimAgac = null, birimAra = '';
    function birimAgaciKur() {
        birimAgac = Sgb.agac('#agcBirim', {
            yukle: function (ust) {
                return Sgb.al(U + 'orgagac', ust ? { ust: ust.kod } : { ust: 0, ara: birimAra }).then(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                    return v.satirlar || [];
                });
            },
            secilince: function (d) { birimYaz(d.kod, d.birimAd || d.ad); dlgBirim.close(); },
            ilkAcik: true,
            bosMetin: 'Birim bulunamadı.',
            hata: hataGoster
        });
    }
    $('#btnBirimSec, #txtBirimAd').on('click', function () {
        if (!birimAgac) birimAgaciKur();
        dlgBirim.showModal();
        $('#txtBirimAra').trigger('focus');
    });
    function birimAramaYap() {
        var a = $.trim($('#txtBirimAra').val());
        if (a && a.length < 3) { mesaj('Aramak için en az 3 harf yazınız.', 'warning'); return; }
        birimAra = a;
        birimAgac.yenile();
    }
    $('#btnBirimAra').on('click', birimAramaYap);
    $('#txtBirimAra').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); birimAramaYap(); } });
    $('#btnBirimSil').on('click', function () { birimYaz(0, ''); });

    // ---------------------------------------------------------------- grid (eski grdListe)

    var sayiDuzenleyici = { min: 0, precision: 0, showStepperButtons: false };
    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { headerName: 'Sıra', width: 64, sortable: false, cellClass: 'hucre-salt', valueGetter: function (p) { return p.node.rowIndex + 1; } },
            { field: 'sicilNo', headerName: 'Sicil', width: 110, cellClass: 'hucre-salt' },
            { field: 'adSoyad', headerName: 'Personel Adı', width: 240, cellClass: 'hucre-salt' },
            { field: 'yilAd', headerName: 'Yıl', width: 120, cellClass: 'hucre-salt' },
            {
                field: 'devreden', headerName: 'Devreden İzin', width: 120, type: 'numericColumn', editable: true, cellClass: 'hucre-girilir',
                cellEditor: 'agNumberCellEditor', cellEditorParams: sayiDuzenleyici
            },
            {
                field: 'yilici', headerName: 'Yıl İçi İzin', width: 120, type: 'numericColumn', editable: true, cellClass: 'hucre-girilir',
                cellEditor: 'agNumberCellEditor', cellEditorParams: sayiDuzenleyici
            },
            { field: 'kullanilan', headerName: 'Kullanılan', width: 110, type: 'numericColumn', cellClass: 'hucre-salt' },
            { headerName: '', flex: 1, sortable: false, cellClass: 'hucre-salt' }
        ],
        getRowId: function (p) { return satirKimlik(p.data); },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        onCellValueChanged: function (e) {
            if (e.newValue === null || e.newValue === undefined || e.newValue === '') e.node.setDataValue(e.colDef.field, 0);
            degisenler[satirKimlik(e.data)] = true;
        },
        overlayNoRowsTemplate: '<span class="text-muted">Birim seçip Listele\'ye basınız.</span>'
    });

    // Grid sayfanın kalan yüksekliğini doldurur, kaydırma gridin içinde kalır (odenekgonderme.js yuksekligeYay).
    function gridYay() {
        var el = document.getElementById('grdListe');
        if (!el || el.offsetParent === null) return;
        var kap = el.closest('.sayfa-icerik') || document.body, cs = getComputedStyle(kap);
        var kaydirir = /(auto|scroll)/.test(cs.overflowY);
        var kr = kap.getBoundingClientRect(), kay = kaydirir ? kap.scrollTop : 0;
        var gorunen = kaydirir ? kap.clientHeight : window.innerHeight - kr.top;
        var ust = el.getBoundingClientRect().top - kr.top + kay;
        var son = 0;
        $(kap).children().each(function () {
            if (this.offsetParent === null && getComputedStyle(this).position !== 'fixed') return;
            son = Math.max(son, this.getBoundingClientRect().bottom - kr.top + kay + parseFloat(getComputedStyle(this).marginBottom || 0));
        });
        son += parseFloat(cs.paddingBottom || 0);
        var bosluk = Math.max(0, son - (ust + el.offsetHeight));
        el.style.height = Math.max(240, Math.floor(gorunen - ust - bosluk - 1)) + 'px';
    }
    $(window).on('resize', gridYay);

    function olcut() {
        return { yil: yil(), birim: birim.kod || 0, turler: tur.secilenler().join(','), ara: $.trim($('#txtAra').val()) };
    }

    function birimDenetle() {
        if (!birim.kod) { mesaj('Birim bilgisini boş bırakamazsınız', 'warning'); return false; }
        return true;
    }

    // Eski btnListele_Click
    function listele() {
        if (!birimDenetle()) return $.Deferred().reject().promise();
        var o = olcut();
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        return Sgb.al(U + 'liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                listelenenYil = v.yil || o.yil;
                degisenler = {};
                grid.setGridOption('overlayNoRowsTemplate', '<span class="text-muted">Kayıt bulunamadı.</span>');
                grid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); $d.prop('disabled', false); });
    }
    $('#btnListele').on('click', listele);
    $('#txtAra').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); listele(); } });
    $('#frmKriter').on('submit', function (e) { e.preventDefault(); });

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
        if (!listelenenYil) { mesaj('Yıl bilgisini boş bırakamazsınız', 'warning'); return; }
        var satirlar = degisenSatirlar();
        if (!satirlar.length) { mesaj('Değişen satır yok.', 'info'); return; }
        onay('Kayıt işlemi yapılacak. Onaylıyor musunuz?', function () {
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder(U + 'kaydet', { yil: listelenenYil, satirlar: satirlar })
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                    if (v.bos) { mesaj(v.bilgi, 'info'); return; }
                    listele().always(function () { mesaj(v.bilgi, 'success'); });
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // Eski btnAktar_Click
    $('#btnAktar').on('click', function () {
        if (!birimDenetle()) return;
        var o = olcut();
        onay((o.yil - 1) + ' yılından devreden izinler ' + o.yil + ' yılına aktarılacak. Onaylıyor musunuz?', function () {
            var $d = $('#btnAktar').prop('disabled', true);
            mesaj('Bir önceki yıldan devreden günler aktarılıyor.', 'info');
            Sgb.gonder(U + 'aktar', o)
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                    mesaj(v.bilgi, 'success');
                    listele();
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al(U + 'tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            tur.doldur(v.turler);
            Sgb.yilKutusu('#txtYil', [v.yilMin, v.yilMaks], v.yil);
            $('#lblDonemAd').text(v.donemAd || '');
        })
        .fail(hataGoster)
        .always(gridYay);
    gridYay();
});
