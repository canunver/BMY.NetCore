// Ödenek Gönderme/Tenkis Bilgisi (ButceMuhasebe/OdenekGondermeTenkisEkonomik; eski OdenekGondermeTenkisEkonomik.aspx): seçilen yıl,
// muhasebe / harcama birimi ve ekonomik kod için tertip başına onaysız / onaylı ödenek gönderme-tenkis, harcama ve kullanılabilir tutar.
// Ödenek Gönderme / Tenkis formundaki "Önceden gönderilen ödenekler" tuşuyla ?muhasebe=&birim= ile açılır; muhasebe verilmişse
// açılışta listelenir. Uçlar: /api/ogtekonomik/* (Muhasebe/OdenekGondermeEkonomikUclari.cs).
$(function () {
    'use strict';

    if (!$('.ogt-ekonomik').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var API = 'api/ogtekonomik';
    var q = new URLSearchParams(window.location.search);
    var tutar = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 0, maximumFractionDigits: 0 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal' },
        yil: yilOku,
        kurumsalGizle: true
    });

    function sayi(alan, ad) {
        return {
            field: alan, headerName: ad, flex: 1, minWidth: 110, type: 'rightAligned',
            valueFormatter: function (p) { return p.value == null ? '' : tutar.format(p.value); }
        };
    }
    var BOS = '<span>Yılı, muhasebe / harcama birimini seçip Listele\'ye basınız.</span>';
    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'tertip', headerName: 'Tertip', width: 260, minWidth: 200, tooltipField: 'tertip' },
            sayi('onaysiz', 'Onaysız'),
            sayi('onayli', 'Onaylı'),
            sayi('toplam', 'Toplam'),
            sayi('harcama', 'Harcama'),
            sayi('kalan', 'Kullanılabilir')
        ],
        overlayNoRowsTemplate: BOS,
        getRowStyle: function (p) { return p.node.rowPinned ? { fontWeight: 700 } : null; }
    });

    function toplamYaz(r) {
        if (!r.length) { grid.setGridOption('pinnedBottomRowData', []); return; }
        var t = { tertip: 'TOPLAM', onaysiz: 0, onayli: 0, toplam: 0, harcama: 0, kalan: 0 };
        $.each(r, function (i, d) {
            t.onaysiz += d.onaysiz || 0; t.onayli += d.onayli || 0; t.toplam += d.toplam || 0;
            t.harcama += d.harcama || 0; t.kalan += d.kalan || 0;
        });
        grid.setGridOption('pinnedBottomRowData', [t]);
    }

    function listele() {
        var d = muhBirim.deger();
        if (yilOku() <= 0) { mesaj('Sorgulama için yıl alanını boş bırakamazsınız.', 'warning'); return; }
        if (!(d.muhasebe > 0)) { mesaj('Sorgulama için Muhasebe alanını boş bırakamazsınız.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(API + '/liste', { yil: yilOku(), muhasebe: d.muhasebe, birim: d.birim, eko: $.trim($('#txtEko').val()) })
            .done(function (v) {
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                if (v.hata) { mesaj(v.hata, 'danger'); grid.setGridOption('rowData', []); toplamYaz([]); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                toplamYaz(v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }
    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(); });

    // Açılış (eski Page_Load): yıl bu yıl, muhasebe / birim adres satırından.
    Sgb.al(API + '/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            Sgb.yilKutusu('#ddlYil', t.yillar, t.yil);
            var muh = parseInt(q.get('muhasebe'), 10) || 0;
            if (muh > 0) {
                muhBirim.ata({ muhasebe: muh, birim: q.get('birim') || '' });
                muhBirim.yenile();
                listele();
            }
        })
        .fail(hataGoster);
    if (!yilOku()) $('#ddlYil').val(new Date().getFullYear());

    function yuksekligeYay(el, enAz) {
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
        el.style.height = Math.max(enAz, Math.floor(gorunen - ust - bosluk - 1)) + 'px';
    }
    function tabloYay() { yuksekligeYay(document.getElementById('grdListe'), 200); }
    $(window).on('resize', tabloYay);
    tabloYay();
});
