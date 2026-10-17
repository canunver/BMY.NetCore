// Ödenek Gönderme / Tenkis İcmali - belge arama (ButceMuhasebe/OdenekGondermeIcmalSorgu; eski aynı adlı Ext.NET sayfası).
// Sorgu alanlarıyla icmaller listelenir; seçili icmaller onaylanır ya da yeni yapılır. İcmal numarasına tıklanınca icmal
// çerçevenin "Belge Düzenleme" sekmesinde açılır (window.parent.OgBelgeAc). Uçlar: /api/odenekgondermeicmal/*.
$(function () {
    'use strict';

    var $kok = $('.og-icmal-liste');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/odenekgondermeicmal/';
    var tanim = {};
    var sonKriter = null;   // son listelemenin kıstasları (işlemler aynı yıl ve belge türü ile)

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

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

    function kriter() {
        return {
            yil: $('#ddlYil').val(), belgeTur: $('#ddlBelgeTur').val(), durum: $('#ddlDurum').val(),
            icmalBas: $.trim($('#txtIcmalBas').val()), icmalBit: $.trim($('#txtIcmalBit').val()),
            kurKod: $.trim($('#txtKurKod').val()), islemYapan: $.trim($('#txtIslemYapan').val()),
            basTarih: $('#txtBasTarih').val(), bitTarih: $('#txtBitTarih').val()
        };
    }

    $('#btnKriterTemizle').on('click', function () {
        $('#frmKriter').find('input[type="text"], input[type="number"], input[type="date"]').val('');
        $('#ddlDurum').val('1');
    });

    // Kurumsal kod ağacı (eski açılır ağaç; ortak Sgb.agac).
    var dlgKod = document.getElementById('dlgKod');
    var agac = null, agacYil = 0;
    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }
    $('#btnKurSec').on('click', function () {
        var y = parseInt($('#ddlYil').val(), 10) || 0;
        if (y <= 0) { mesaj('Yıl seçiniz.', 'warning'); return; }
        dlgKod.showModal();
        if (!agac) {
            agacYil = y;
            agac = Sgb.agac($('#agacKutu > ul')[0], {
                yukle: function (ust) {
                    return Sgb.al(UC + 'kodlar', { yil: agacYil, ust: ust ? ust.kod : '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    dlgKod.close();
                    $('#txtKurKod').val(d.kod);
                },
                seciliMi: function (d) { return kodAl($('#txtKurKod').val()) === d.kod; },
                ilkAcik: true,
                bosMetin: 'Kod bulunamadı.',
                hata: hataGoster
            });
        } else if (agacYil !== y) {
            agacYil = y;
            agac.yenile();
        }
    });
    $('#btnKodTemizle').on('click', function () { $('#txtKurKod').val(''); dlgKod.close(); });

    // ---------------------------------------------------------------- liste

    function belgeAc(d) {
        var yil = parseInt((sonKriter || {}).yil, 10) || 0;
        if (window.parent && window.parent !== window && typeof window.parent.OgBelgeAc === 'function') {
            window.parent.OgBelgeAc(yil, d.icmalNo, d.belgeTur);
            return;
        }
        mesaj('İcmali açmak için Belge Düzenleme sekmesini kullanınız.', 'info');
    }

    function bagHucre(x) {
        if (!x.data) return x.value;
        return $('<a href="#"></a>').text(x.value).on('click', function (e) {
            e.preventDefault(); e.stopPropagation();
            belgeAc(x.data);
        })[0];
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
            { colId: 'sira', headerName: '#', width: 60, minWidth: 50, maxWidth: 70, sortable: false, type: 'rightAligned', valueGetter: function (x) { return x.node.rowIndex === null ? '' : x.node.rowIndex + 1; } },
            { field: 'icmalNo', headerName: 'İcmal No', minWidth: 80, flex: 80, type: 'rightAligned', cellRenderer: bagHucre },
            { field: 'belgeTurAd', headerName: 'Belge Tür', minWidth: 120, flex: 120 },
            { field: 'kisiler', headerName: 'İlgili/İlgililer', minWidth: 200, flex: 260, tooltipField: 'kisiler' },
            { field: 'kurKod', headerName: 'Kur Kod', minWidth: 90, flex: 90 },
            { field: 'durum', headerName: 'Durum', minWidth: 80, flex: 80 },
            { field: 'dtKayitTarih', headerName: 'Tarih', minWidth: 100, flex: 110, valueFormatter: function (x) { return Sgb.tarih(x.value); } }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        getRowId: function (x) { return String(x.data.icmalNo); },
        pagination: true,
        paginationPageSize: 50,
        paginationPageSizeSelector: [20, 50, 100, 200],
        overlayNoRowsTemplate: '<span>Sorgu alanlarını seçip Listele\'ye basınız.</span>'
    });

    function listele(sessiz) {
        var k = kriter();
        if (!(parseInt(k.yil, 10) > 0)) { mesaj('Sorgulama için yıl alanını boş bırakamazsınız.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        grid.deselectAll();
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', k)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); grid.setGridOption('rowData', []); $('#lblListeOzet').text(''); return; }
                sonKriter = k;
                var s = v.satirlar || [];
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
                grid.setGridOption('rowData', s);
                $('#lblListeOzet').text(s.length ? s.length + ' icmal' : '');
                if (!sessiz && !s.length) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmKriter').on('submit', function (e) { e.preventDefault(); listele(false); });

    // ---------------------------------------------------------------- Onayla / Yeni Yap (eski BelgeDurumDegisikligi)

    function durumAta(durum) {
        var s = grid.getSelectedRows();
        if (!s.length || !sonKriter) { mesaj('Listeden herhangi bir belge seçilmediği için işlem yapılamadı.', 'info'); return; }
        sor(durum === 5 ? 'Seçili icmaller onaylanacak. Onaylıyor musunuz?' : 'Seçili icmaller yeni yapılacak. Onaylıyor musunuz?', 'Evet', function () {
            Sgb.gonder(UC + 'durum', {
                yil: parseInt(sonKriter.yil, 10) || 0, belgeTur: parseInt(sonKriter.belgeTur, 10) || 0, durum: durum,
                icmalNolar: $.map(s, function (r) { return r.icmalNo; })
            })
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata || 'İşlem yapılamadı.', 'danger'); return; }
                    mesaj(v.bilgi || 'İşlem tamamlandı.', 'success');
                    listele(true);
                })
                .fail(hataGoster);
        });
    }
    $('#btnOnayla').on('click', function () { durumAta(5); });
    $('#btnYeniYap').on('click', function () { durumAta(1); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al(UC + 'tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            var $y = Sgb.yilKutusu('#ddlYil', t.yillar);
            $y.val(String(t.yil));
            var $b = $('#ddlBelgeTur').empty();
            $.each(t.belgeTurleri || [], function (i, b) { $('<option></option>').val(b.kod).text(b.ad).appendTo($b); });
            var $d = $('#ddlDurum').empty();
            $.each(t.durumlar || [], function (i, d) { $('<option></option>').val(d.kod).text(d.ad).appendTo($d); });
            $d.val('1');
        })
        .fail(hataGoster);

    function listeYay() { yuksekligeYay(document.getElementById('grdListe'), 250); }
    $(window).on('resize', listeYay);
    listeYay();
});
