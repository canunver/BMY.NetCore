// Tertip bilgisi (ButceHazirlikv2/MasrafPlaniTertipBilgi; eski MasrafPlaniTertipBilgi.aspx): tertibin onaylı masraf planı ay ay;
// seçilen birime (kurumsal yapı), dağıtılmayan (merkez) ve dağıtılan tutarlar; Detay Yaz (Tertip Detay / Birim Detay) Excel.
// Bütçe İşlemi formundaki satırdan ?yil=&tertip=&kurumsalYapiKod= ile açılır. Uçlar: /api/mprevize/tertipbilgi*.
$(function () {
    'use strict';

    if (!$('.mp-tertip-bilgi').length) return;

    var API = 'api/mprevize';
    var q = new URLSearchParams(window.location.search);

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }
    var kodlar = MPTavan.kodAlanlari(yilOku, hataGoster, API);

    function kistas() {
        return { yil: yilOku(), kur: $('#ddlBirim').val() || '', fon: kodlar.kod('fon'), fin: kodlar.kod('fin'), eko: kodlar.kod('eko'), peb: kodlar.kod('peb'), ky: kodlar.kod('ky') };
    }

    function tutarYaz(p) { return MPTavan.tutar(p.value); }
    var sayi = function (alan, ad) { return { field: alan, headerName: ad, flex: 1, minWidth: 130, type: 'rightAligned', valueFormatter: tutarYaz }; };
    var grid = Sgb.grid('#grdAylar', {
        columnDefs: [{ field: 'ay', headerName: 'Ay', width: 140 }, sayi('birim', 'Birim'), sayi('dagitilmayan', 'Dağıtılmayan'), sayi('dagitilan', 'Dağıtılan')],
        overlayNoRowsTemplate: '<span>Tertibi girip Listele\'ye basınız.</span>'
    });

    function toplamYenile(r) {
        if (!r.length) { grid.setGridOption('pinnedBottomRowData', []); return; }
        var t = { ay: 'Toplam', birim: 0, dagitilmayan: 0, dagitilan: 0 };
        $.each(r, function (i, d) { t.birim += d.birim || 0; t.dagitilmayan += d.dagitilmayan || 0; t.dagitilan += d.dagitilan || 0; });
        grid.setGridOption('pinnedBottomRowData', [t]);
    }

    function listele() {
        var k = kistas();
        if (k.yil <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(API + '/tertipbilgi', k)
            .done(function (v) {
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                toplamYenile(v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }
    $('#btnListele').on('click', listele);
    $('#frmTertip').on('submit', function (e) { e.preventDefault(); listele(); });

    $('.detay-yaz').on('click', function () {
        var k = kistas();
        if (k.yil <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        mesaj('Rapor hazırlanıyor...', 'info');
        Sgb.indir(API + '/tertipbilgi/detay', $.extend(k, { tur: $(this).attr('data-tur') })).then(function () { $d.prop('disabled', false); });
    });

    // Açılış: adres satırındaki tertip (kur-fon-fin-eko-peb) ve kurumsal yapı kodu yazılıp listelenir (eski Page_Load).
    var parca = (q.get('tertip') || '').split('-');
    var yil = parseInt(q.get('yil'), 10) || 0;
    Sgb.al(API + '/tertipbilgi/tanim', { yil: yil || undefined })
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            Sgb.yilKutusu('#ddlYil', t.yillar, t.yil);
            var $b = $('#ddlBirim').empty();
            $('<option></option>').val('').text('').appendTo($b);
            $.each(t.birimler || [], function (i, o) { $('<option></option>').val(o.kod).text(o.ad).appendTo($b); });
            if (parca[0]) {
                if (!$b.find('option').filter(function () { return this.value === parca[0]; }).length) $('<option></option>').val(parca[0]).text(parca[0]).appendTo($b);
                $b.val(parca[0]);
            }
            kodlar.ata('fon', parca[1] || '');
            kodlar.ata('fin', parca[2] || '');
            kodlar.ata('eko', parca[3] || '');
            kodlar.ata('peb', parca[4] || '');
            kodlar.ata('ky', q.get('kurumsalYapiKod') || '');
            if (q.get('tertip')) listele();
        })
        .fail(hataGoster);
    $('#ddlYil').on('change', function () { kodlar.adlariYenile(); });

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
    function tabloYay() { yuksekligeYay(document.getElementById('grdAylar'), 200); }
    $(window).on('resize', tabloYay);
    if (window.ResizeObserver) new ResizeObserver(tabloYay).observe(document.querySelector('.mp-tertip-bilgi .mp-ust'));
    tabloYay();
});
