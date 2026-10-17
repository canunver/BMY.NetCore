// Ödenek Gönderme / Tenkis belge arama (ButceMuhasebe/OdenekGondermeTenkisSorgu; eski aynı adlı Ext.NET sayfası).
// Süzgeçlerle belgeler listelenir; seçili belgelerde durum işlemleri (muhasebata gönder, iptal, tahsis, icmal no ...),
// liste / belge / icmal çıktıları ve belge tarihçesi. Belge numarasına tıklanınca belge çerçevenin "Belge Düzenleme"
// sekmesinde açılır (window.parent.OgBelgeAc). Uçlar: /api/odenekgonderme/* (Muhasebe/OdenekGondermeUclari.cs).
$(function () {
    'use strict';

    var $kok = $('.og-liste');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var BT = parseInt($kok.attr('data-bt'), 10) || 0;
    var TALEP = $kok.attr('data-talep') || '';
    var UC = 'api/odenekgonderme/';
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var tanim = {};
    var sonKriter = null;   // son listelemenin kıstasları (yazdırma ve işlemler aynı yıl ile)

    function p(ek) { return $.extend({ bt: BT, talep: TALEP }, ek || {}); }
    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function para(v) { return v === null || v === undefined || v === '' ? '' : tutar2.format(Number(v) || 0); }
    function sayiMetin(s) { s = $.trim(s || ''); return s.indexOf(',') >= 0 ? s.replace(/\./g, '').replace(',', '.') : s; }

    // ---------------------------------------------------------------- açılır menüler (Bootstrap 3 görünümü, betiksiz)

    $kok.on('click', '.og-menu > .dropdown-toggle', function (e) {
        e.stopPropagation();
        var $g = $(this).parent();
        var acik = $g.hasClass('open');
        $kok.find('.og-menu.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false');
        if (!acik) $g.addClass('open').children('.dropdown-toggle').attr('aria-expanded', 'true');
    });
    $(document).on('click', function () { $kok.find('.og-menu.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false'); });
    $kok.on('click', '.dropdown-menu a', function (e) { e.preventDefault(); });

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;
    function sor(metin, evet, is) {
        $('#soruMetin').text(metin);
        $('#btnSoruEvet').text(evet || 'Evet');
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

    // ---------------------------------------------------------------- kıstaslar

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: function () { return parseInt($('#ddlYil').val(), 10) || 0; },
        kurumsalGizle: true,
        adGizle: true
    });

    // "13.01.31.00- Ad" -> "13.01.31.00"
    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }
    function alanKod(alan) { return kodAl($('.kod-alan[data-alan="' + alan + '"] input').val()); }

    function kriter() {
        var d = muhBirim.deger();
        return p({
            yil: $('#ddlYil').val(), durum: $('#ddlDurum').val(),
            muhasebe: d.muhasebe || '', birim: d.birim,
            icmalNo: $.trim($('#txtIcmalNoK').val()), icmalTarihi: $('#txtIcmalTarihi').val(),
            belgeNo1: $.trim($('#txtBelgeNo1').val()), belgeNo2: $.trim($('#txtBelgeNo2').val()),
            belgeTarihi1: $('#txtBelgeTarihi1').val(), belgeTarihi2: $('#txtBelgeTarihi2').val(),
            durumTarihi1: $('#txtDurumTarihi1').val(), durumTarihi2: $('#txtDurumTarihi2').val(),
            kur: alanKod('kur'), fon: alanKod('fon'), fin: alanKod('fin'), eko: alanKod('eko'), peb: alanKod('peb'),
            islemYapan: $.trim($('#txtIslemYapan').val()), aciklama: $.trim($('#txtAciklamaK').val()),
            tutarbe: sayiMetin($('#txtTutarBE').val()), tutarke: sayiMetin($('#txtTutarKE').val()),
            surecno: $.trim($('#txtSurecNo').val()), dortDuzeyDegisim: $('#chkDortDuzey').prop('checked') ? 'true' : 'false'
        });
    }

    function yil() { return parseInt((sonKriter || {}).yil || $('#ddlYil').val(), 10) || 0; }

    // Kod ağaçları (Ödenek Durum sayfalarıyla aynı alanlar ve pencere).
    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};
    var aktifAlan = null;
    function agacAc($alan) {
        var y = parseInt($('#ddlYil').val(), 10) || 0;
        if (y <= 0) { mesaj('Yıl seçiniz.', 'warning'); return; }
        var tur = $alan.attr('data-tur');
        var ad = $alan.find('label').text();
        aktifAlan = $alan;
        $('#dlgKodBaslik').text(ad + ' Seçimi');
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[tur];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', ad + ' ağacı').appendTo('#agacKutu'), yil: y };
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al(UC + 'kodlar', p({ yil: a.yil, tur: tur, ust: ust ? ust.kod : '' })).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    dlgKod.close();
                    if (aktifAlan) aktifAlan.find('input').val(d.ad).trigger('change');
                },
                seciliMi: function (d) { return !!aktifAlan && kodAl(aktifAlan.find('input').val()) === d.kod; },
                ilkAcik: true,
                bosMetin: 'Kod bulunamadı.',
                hata: hataGoster
            });
        } else {
            a.$ul.prop('hidden', false);
            if (a.yil !== y) { a.yil = y; a.agac.yenile(); }
        }
    }
    $('.kod-alan .kod-sec').on('click', function () { agacAc($(this).closest('.kod-alan')); });
    $('.kod-alan .kod-temizle').on('click', function () { $(this).closest('.kod-alan').find('input').val('').trigger('focus'); });
    $('.kod-alan input').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
    });

    $('#btnKriterTemizle').on('click', function () {
        $('#frmKriter').find('input[type="text"], input[type="date"]').val('');
        $('#chkDortDuzey').prop('checked', false);
        muhBirim.ata({});
        if (tanim.varsayilanDurum) $('#ddlDurum').val(String(tanim.varsayilanDurum));
    });

    // ---------------------------------------------------------------- liste

    function belgeAc(d) {
        if (window.parent && window.parent !== window && typeof window.parent.OgBelgeAc === 'function') {
            window.parent.OgBelgeAc(yil(), d.belgeNo);
            return;
        }
        window.open('OdenekGondermeTenkisMPExt?' + $.param({ belgetur: BT, talep: TALEP, yil: yil(), belgeNo: d.belgeNo }), '_blank');
    }

    // Açılan belgelerin bağlantısı ziyaret edilmiş renkte kalır (href="#" bağlantılarda tarayıcı bunu kendisi yapmaz).
    // Liste sayfa yenilense de aynı oturumda (sekme kapanana kadar) hatırlanır.
    var ACILAN_ANAHTAR = 'og-acilan-' + BT + '-' + (TALEP ? 't' : '');
    var acilanlar = {};
    try { acilanlar = JSON.parse(sessionStorage.getItem(ACILAN_ANAHTAR) || '{}') || {}; } catch (e) { acilanlar = {}; }

    function bagHucre(is, isaretle) {
        return function (x) {
            if (!x.data) return x.value;
            var $a = $('<a href="#"></a>').text(x.value || '').toggleClass('og-ziyaret', !!(isaretle && acilanlar[yil() + '-' + x.data.belgeNo]));
            return $a.on('click', function (e) {
                e.preventDefault(); e.stopPropagation();
                if (isaretle) {
                    acilanlar[yil() + '-' + x.data.belgeNo] = true; $a.addClass('og-ziyaret');
                    try { sessionStorage.setItem(ACILAN_ANAHTAR, JSON.stringify(acilanlar)); } catch (e2) { }
                }
                is(x.data);
            })[0];
        };
    }

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

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { colId: 'ac', headerName: '', width: 40, minWidth: 40, maxWidth: 40, sortable: false, resizable: false, cellClass: 'og-ac-hucre', cellRenderer: acHucre },
            { field: 'belgeNo', headerName: 'Belge No', minWidth: 100, flex: 130, cellRenderer: bagHucre(belgeAc, true) },
            { field: 'belgeTarih', headerName: 'Belge Tarihi', minWidth: 90, flex: 110, valueFormatter: function (x) { return Sgb.tarih(x.value); } },
            { field: 'muhasebe', headerName: 'Muhasebe', minWidth: 130, flex: 220, tooltipField: 'muhasebe' },
            { field: 'birim', headerName: 'Birim', minWidth: 130, flex: 220, tooltipField: 'birim' },
            { field: 'tutar', headerName: 'Tutar', minWidth: 110, flex: 140, type: 'rightAligned', valueFormatter: function (x) { return para(x.value); } },
            { field: 'icmalNo', headerName: 'İcmal No', minWidth: 70, flex: 90, type: 'rightAligned' },
            { field: 'icmalTarih', headerName: 'İcmal Tarihi', minWidth: 90, flex: 110, valueFormatter: function (x) { return Sgb.tarih(x.value); } },
            { field: 'muhasebatNo', headerName: 'Muhasebat No', minWidth: 90, flex: 110, hide: true },
            { field: 'muhasebatTarih', headerName: 'Muhasebat Tarihi', minWidth: 100, flex: 120, hide: true, valueFormatter: function (x) { return Sgb.tarih(x.value); } },
            { field: 'durum', headerName: 'Durum', minWidth: 130, flex: 200, cellRenderer: bagHucre(tarihceAc), tooltipValueGetter: function () { return 'Belge tarihçesi'; } },
            { field: 'islemTarih', headerName: 'Durum Tarihi', minWidth: 90, flex: 110, hide: true, valueFormatter: function (x) { return Sgb.tarih(x.value); } },
            { field: 'islemKisi', headerName: 'İşlemi Yapan', minWidth: 90, flex: 120, hide: true }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false, isRowSelectable: function (n) { return !(n.data && n.data._detay); } },
        getRowId: function (x) { return x.data._detay ? 'detay-' + x.data.belgeNo : x.data.belgeNo; },
        isFullWidthRow: function (x) { return !!(x.rowNode.data && x.rowNode.data._detay); },
        fullWidthCellRenderer: ayrintiHucre,
        getRowHeight: function (x) { return x.data && x.data._detay ? (x.data._yukseklik || 90) : undefined; },
        onSortChanged: function () { ayrintilariKapat(); },
        onPaginationChanged: function (e) { if (e.newPage) ayrintilariKapat(); },
        pagination: true,
        paginationPageSize: 50,
        paginationPageSizeSelector: [20, 50, 100, 200],
        overlayNoRowsTemplate: '<span>Kıstasları seçip Listele\'ye basınız.</span>'
    });

    function ozetYaz() {
        var adet = 0, toplam = 0;
        grid.forEachNode(function (n) { if (n.data._detay) return; adet++; toplam += Number(n.data.tutar) || 0; });
        $('#lblListeOzet').text(adet ? adet + ' belge, toplam ' + tutar2.format(toplam) : '');
    }

    function listele(sessiz) {
        var k = kriter();
        if (!(parseInt(k.yil, 10) > 0) || !(parseInt(k.durum, 10) > 0)) { mesaj('Sorgulama için yıl alanını boş bırakamazsınız.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        grid.deselectAll();
        acik = {};
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', k)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); grid.setGridOption('rowData', []); ozetYaz(); return; }
                sonKriter = k;
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
                grid.setGridOption('rowData', v.satirlar || []);
                ozetYaz();
                if (!sessiz && !(v.satirlar || []).length) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmKriter').on('submit', function (e) { e.preventDefault(); listele(false); });

    // ---------------------------------------------------------------- satır ayrıntısı (eski satır açıcı)
    // Satırdaki + düğmesi, satırın hemen altına tam genişlikte bir ayrıntı satırı ekler: açıklama, iptal nedeni ve tertipler.

    var acik = {};

    function acHucre(x) {
        if (!x.data || x.data._detay) return '';
        var a = !!acik[x.data.belgeNo];
        return $('<button type="button" class="btn btn-link btn-xs og-ac"></button>')
            .attr('title', a ? 'Ayrıntıyı kapat' : 'Ayrıntıyı göster').attr('aria-expanded', a ? 'true' : 'false')
            .text(a ? '\u2212' : '+')
            .on('click', function (e) { e.stopPropagation(); ayrintiDegistir(x.node); })[0];
    }

    function ayrintiDegistir(node) {
        var d = node.data, no = d.belgeNo;
        if (acik[no]) {
            grid.applyTransaction({ remove: [acik[no]] });
            delete acik[no];
        } else {
            acik[no] = { _detay: true, belgeNo: no, aciklama: d.aciklama || '', iptalNeden: d.iptalNeden || '', satirlar: null };
            grid.applyTransaction({ add: [acik[no]], addIndex: node.rowIndex + 1 });
            Sgb.al(UC + 'liste/detay', p({ yil: yil(), belgeNo: no }))
                .done(function (v) {
                    if (!acik[no]) return;
                    if (v.hata) { mesaj(v.hata, 'danger'); acik[no].satirlar = []; } else acik[no].satirlar = v.satirlar || [];
                    ayrintiYenile(no);
                })
                .fail(hataGoster);
        }
        grid.refreshCells({ rowNodes: [node], columns: ['ac'], force: true });
    }

    function ayrintiYenile(no) {
        var n = grid.getRowNode('detay-' + no);
        if (!n) return;
        var t = n.data.satirlar;
        n.data._yukseklik = 28 + (n.data.aciklama ? 24 : 0) + (n.data.iptalNeden ? 24 : 0) + 32 + 29 * Math.max(1, (t || []).length);
        grid.resetRowHeights();
        grid.redrawRows({ rowNodes: [grid.getRowNode('detay-' + no)] });
    }

    function ayrintilariKapat() {
        var l = $.map(acik, function (v) { return v; });
        acik = {};
        if (l.length) grid.applyTransaction({ remove: l });
        grid.refreshCells({ columns: ['ac'], force: true });
    }

    function ayrintiHucre(x) {
        var d = x.data, $k = $('<div class="og-ayrinti-satir"></div>');
        if (d.aciklama) $('<div></div>').append($('<b></b>').text('Açıklama: ')).append(document.createTextNode(d.aciklama)).appendTo($k);
        if (d.iptalNeden) $('<div></div>').append($('<b></b>').text('İptal: ')).append(document.createTextNode(d.iptalNeden)).appendTo($k);
        if (d.satirlar === null) { $('<div class="og-ipucu"></div>').text('Tertipler okunuyor...').appendTo($k); return $k[0]; }
        var $t = $('<table class="og-ayrinti-tablo"><thead><tr><th>Tertip</th><th class="og-sag">Tutar</th></tr></thead><tbody></tbody></table>');
        var $b = $t.find('tbody');
        if (!d.satirlar.length) $('<tr><td colspan="2" class="og-ipucu">Tertip yok.</td></tr>').appendTo($b);
        $.each(d.satirlar, function (i, r) {
            $('<tr></tr>').append($('<td></td>').text(r.tertip || '')).append($('<td class="og-sag"></td>').text(para(r.tutar))).appendTo($b);
        });
        $k.append($t);
        return $k[0];
    }

    // ---------------------------------------------------------------- tarihçe (eski BelgeTarihce.aspx)

    var dlgTarihce = document.getElementById('dlgTarihce');
    var tarihceGrid = null;
    function tarihceAc(d) {
        if (!tarihceGrid) {
            tarihceGrid = Sgb.grid('#grdTarihce', {
                columnDefs: [
                    { field: 'sira', headerName: 'Sıra', minWidth: 70, flex: 70, type: 'rightAligned' },
                    { field: 'islemTarih', headerName: 'İşlem Tarih', minWidth: 170, flex: 170 },
                    { field: 'islem', headerName: 'İşlem', minWidth: 220, flex: 220, tooltipField: 'islem' },
                    { field: 'islemYapan', headerName: 'İşlemi Yapan', minWidth: 130, flex: 130 },
                    { field: 'islemYapanAd', headerName: 'İşlem Yapan Ad', minWidth: 180, flex: 180 }
                ],
                overlayNoRowsTemplate: '<span>Tarihçe kaydı yok.</span>'
            });
        }
        $('#lblTarihceBelge').text('');
        tarihceGrid.setGridOption('rowData', []);
        dlgTarihce.showModal();
        tarihceGrid.setGridOption('loading', true);
        Sgb.al(UC + 'tarihce', p({ yil: yil(), belgeNo: d.belgeNo }))
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#lblTarihceBelge').text(v.belge || '');
                tarihceGrid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { tarihceGrid.setGridOption('loading', false); });
    }

    // ---------------------------------------------------------------- yazdırma

    $('.liste-yazdir').on('click', function () {
        var k = kriter();
        if (!(parseInt(k.yil, 10) > 0) || !(parseInt(k.durum, 10) > 0)) { mesaj('Sorgulama için yıl alanını boş bırakamazsınız.', 'warning'); return; }
        Sgb.indir(UC + 'liste/yazdir', $.extend(k, { tur: $(this).attr('data-tur') }));
    });

    function seciliBelgeler() { return $.map(grid.getSelectedRows(), function (r) { return r.belgeNo; }); }

    $('#mnBelgeYazdir').on('click', function () {
        var b = seciliBelgeler();
        if (!b.length) { mesaj('Listeden herhangi bir belge seçilmediği için işlem yapılamadı.', 'info'); return; }
        Sgb.indir(UC + 'belgeler-yazdir', p({ yil: yil(), belgeler: b.join(';') }));
    });

    $('#mnIcmalYazdir').on('click', function () {
        var no = $.trim($('#txtIcmalNo').val());
        if (!(parseInt(no, 10) > 0)) { mesaj('Yazdırılacak İcmal numarası belirtilmemiş.', 'warning'); return; }
        Sgb.indir(UC + 'icmal-yazdir', p({ yil: yil(), icmalNo: no }));
    });

    // ---------------------------------------------------------------- durum işlemleri (eski BelgeDurumDegisikligi)

    function durumGonder(islem, ek) {
        var b = seciliBelgeler();
        if (!b.length) { mesaj('Listeden herhangi bir belge seçilmediği için işlem yapılamadı.', 'info'); return; }
        Sgb.gonder(UC + 'durum', $.extend({ belgeTur: BT, talep: TALEP, yil: yil(), islem: islem, belgeler: b }, ek || {}))
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata || 'İşlem yapılamadı.', 'danger'); return; }
                if (v.icmalNo) $('#txtIcmalNo').val(v.icmalNo);
                mesaj(v.bilgi || 'İşlem tamamlandı.', 'success');
                if (islem !== 'ebutce') listele(true);
            })
            .fail(hataGoster);
    }

    $('.durum-islem').on('click', function () {
        var islem = $(this).attr('data-islem');
        if (!seciliBelgeler().length) { mesaj('Listeden herhangi bir belge seçilmediği için işlem yapılamadı.', 'info'); return; }
        sor('İşlem yapılacak. Onaylıyor musunuz?', 'Evet', function () {
            durumGonder(islem, islem === 'icmalno' ? { icmalNo: $.trim($('#txtIcmalNo').val()) } : null);
        });
    });

    var dlgIptal = document.getElementById('dlgIptal');
    $('#mnIptal').on('click', function () {
        if (!seciliBelgeler().length) { mesaj('Listeden herhangi bir belge seçilmediği için işlem yapılamadı.', 'info'); return; }
        $('#txtIptalGerekce').val('');
        dlgIptal.showModal();
    });
    $('#frmIptal').on('submit', function (e) {
        e.preventDefault();
        var gerekce = $.trim($('#txtIptalGerekce').val());
        dlgIptal.close();
        sor('İşlem yapılacak. Onaylıyor musunuz?', 'İptal Et', function () { durumGonder('iptal', { gerekce: gerekce }); });
    });

    // Birimlere Aç / Kapa (eski btnIslemAcKapa_Click; belge seçimi gerekmez).
    $('.ac-kapa').on('click', function () {
        var islem = $(this).attr('data-islem');
        sor(islem === 'kapa' ? 'Belge girişi birimlere kapatılacak. Onaylıyor musunuz?' : 'Belge girişi birimlere açılacak. Onaylıyor musunuz?', 'Evet', function () {
            Sgb.gonder(UC + 'durum', { belgeTur: BT, talep: TALEP, yil: parseInt($('#ddlYil').val(), 10) || 0, islem: islem })
                .done(function (v) { mesaj(v.basarili ? v.bilgi : v.hata, v.basarili ? 'success' : 'danger'); })
                .fail(hataGoster);
        });
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al(UC + 'tanim', p())
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            var $y = Sgb.yilKutusu('#ddlYil', t.yillar);
            $y.val(String(t.yil));
            var $d = $('#ddlDurum').empty();
            $.each(t.durumlar || [], function (i, d) { $('<option></option>').val(d.kod).text(d.ad).appendTo($d); });
            $d.val(String(t.varsayilanDurum));
            $('.kod-alan[data-alan="kur"] input').val(t.kurum || '');

            // Düğme görünürlükleri (eski Page_Load).
            var talep = !!TALEP;
            $('#mnYeniYap').prop('hidden', !t.yonetici);
            $('#grpDiger').prop('hidden', !t.yonetici);
            $('#mnTahsis, #mnMuhasebat').prop('hidden', talep);
            $('#grpIcmal').prop('hidden', talep);
            $('#mnSgb').prop('hidden', !t.sgbGoster);
            if (t.muhasebatYok) {
                $('#mnSgb, #mnMuhasebat, #mnEbutce, #mnDegisim, #mnMuhAldi').prop('hidden', true);
                $('#mnOnayla').prop('hidden', false);
            }
        })
        .fail(hataGoster);

    function listeYay() { yuksekligeYay(document.getElementById('grdListe'), 250); }
    $(window).on('resize', listeYay);
    listeYay();
});
