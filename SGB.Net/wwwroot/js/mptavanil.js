// İl / İlçe Ödenek Dağıtım (ButceHazirlikv2/MPTavanIlAna; eski MPTavanIlAna.aspx): saymanlık / birimin kurumsal yapısına ait tertiplerin
// tavanı ve birimlere dağıtılan tutarı AG Grid'de listelenir. İl izni açık tertipte tutara tıklanınca Tavan Girişi (MPTavanGiris, DG=I)
// pencerede açılır; pencere kapanınca liste yenilenir. Tavan Yazdır / Detay Yazdır Excel raporları.
// Uçlar: /api/mptavanil/* (ButceHazirlik/MPTavanIlUclari.cs); kod ağacı ve kod adı /api/mptavan/kodlar, kodad.
$(function () {
    'use strict';

    var $kok = $('.mp-tavan-il');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var son = null; // son listelemenin kurumsal yapı kodu ve saymanlık-birimi (tavan girişine gider)

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }
    var kodlar = MPTavan.kodAlanlari(yilOku, hataGoster);

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', ad: 'lblBirimAd' },
        yil: yilOku,
        kurumsalGizle: true
    });

    function secimOku() {
        var mb = muhBirim.deger();
        return { yil: yilOku(), muhasebe: mb.muhasebe > 0 ? mb.muhasebe : '', birim: mb.birim, eko: kodlar.kod('eko'), peb: kodlar.kod('peb') };
    }

    // ---------------------------------------------------------------- tablo

    // Eski TavanDetayRenderer: il izni açıksa tutar bağlantıdır, il tavan girişini açar.
    function bagHucre(p) {
        if (!p.data.ilIzni) return MPTavan.tutar(p.value);
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-link mp-bag';
        b.textContent = MPTavan.tutar(p.value);
        b.title = 'Birimlere dağıtımı aç';
        b.addEventListener('click', function () { tavanGirisAc(p.data.tertip); });
        return b;
    }

    // Eski IzinRenderer: il girişi açık / kapalı simgesi (burada yalnızca gösterilir).
    function izinHucre(p) {
        var s = document.createElement('span');
        s.className = 'mp-izin ' + (p.value ? 'izin-var' : 'izin-yok');
        s.title = p.value ? 'Bu tertibin birim dağıtımını il yapabilir.' : 'Bu tertibin birim dağıtımı il girişine kapalıdır.';
        s.innerHTML = '<span class="glyphicon ' + (p.value ? 'glyphicon-ok-sign' : 'glyphicon-remove-sign') + '" aria-hidden="true"></span>';
        $('<span class="sr-only"></span>').text(p.value ? 'İl girişi açık' : 'İl girişi kapalı').appendTo(s);
        return s;
    }

    var sutunlar = [
        {
            field: 'tertip', headerName: 'Tertip', width: 420, cellClass: ['mp-tertip'],
            tooltipValueGetter: function (p) { return p.data ? (p.data.tertip || '') + (p.data.tertipAdi ? ' ' + p.data.tertipAdi : '') : ''; },
            cellRenderer: function (p) {
                var e = document.createElement('div');
                e.className = 'mp-iki-satir';
                e.appendChild(document.createTextNode(p.data.tertip || ''));
                if (p.data.tertipAdi) {
                    var k = document.createElement('small');
                    k.textContent = p.data.tertipAdi;
                    e.appendChild(k);
                }
                return e;
            }
        },
        { colId: 'tavan', field: 'tavan', headerName: 'Tavan', type: 'rightAligned', width: 160, cellClass: ['sayi'], cellRenderer: bagHucre },
        { colId: 'detay', field: 'detay', headerName: 'Birimler', type: 'rightAligned', width: 160, cellClass: ['sayi'], cellRenderer: bagHucre },
        { field: 'ilIzni', headerName: 'İl İzni', width: 80, cellClass: ['orta'], cellRenderer: izinHucre, sortable: false }
    ];

    var grid = Sgb.grid('#grdMPTavanIl', {
        columnDefs: sutunlar,
        rowHeight: 42,
        tooltipShowDelay: 400,
        overlayNoRowsTemplate: '<span>Yılı, saymanlık / birimi seçip Listele\'ye basınız.</span>'
    });

    // Eski SetColumnHeader: toplamlar sütun başlıklarında.
    function basliklariYaz(tavan, detay) {
        sutunlar[1].headerName = tavan === null ? 'Tavan' : 'Tavan: ' + MPTavan.tutar(Math.round(tavan));
        sutunlar[2].headerName = detay === null ? 'Birimler' : 'Birimler: ' + MPTavan.tutar(Math.round(detay));
        grid.setGridOption('columnDefs', sutunlar);
    }

    // ---------------------------------------------------------------- Listele (eski btnListele_Click)

    function listele(sessiz) {
        var o = secimOku();
        if (o.yil <= 0) { mesaj('Yıl alanı boş boş bırakılamaz!', 'warning'); return; }
        if (!o.muhasebe || !o.birim) { mesaj('Saymanlık ve birim seçiniz.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/mptavanil/liste', o)
            .done(function (v) {
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                if (v.hata) {
                    son = null;
                    grid.setGridOption('rowData', []);
                    basliklariYaz(null, null);
                    if (!sessiz) mesaj(v.hata, 'danger');
                    return;
                }
                son = { yil: o.yil, kykod: v.kykod, saybir: v.saybir };
                grid.setGridOption('rowData', v.satirlar || []);
                basliklariYaz(v.tavanToplam, v.detayToplam);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }
    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(false); });

    // ---------------------------------------------------------------- İl tavan girişi (eski AdresAc2 -> MPTavanGiris.aspx?DG=I)

    function tavanGirisAc(tertip) {
        if (!son) return;
        var adres = 'MPTavanGiris?' + $.param({ yil: son.yil, tertip: tertip, DG: 'I', kykod: son.kykod, saybir: son.saybir });
        Sgb.pencere(adres, 'İl Tavan Girişi', function () { listele(true); });
    }

    // ---------------------------------------------------------------- Tavan / Detay Yazdır (eski btnYazdir_Click, tur 1 / 2)

    $('#btnYazdirTavan, #btnYazdir').on('click', function () {
        var o = secimOku();
        if (o.yil <= 0) { mesaj('Yıl alanı boş boş bırakılamaz!', 'warning'); return; }
        if (!o.muhasebe || !o.birim) { mesaj('Saymanlık ve birim seçiniz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        o.tur = $d.attr('data-tur');
        Sgb.indir('api/mptavanil/rapor', o).then(function () { $d.prop('disabled', false); }, function () { $d.prop('disabled', false); });
    });

    $('#btnYardim').on('click', function () {
        var $p = $('#pnlIpucu'), ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).toggleClass('active', ac).attr('aria-expanded', ac ? 'true' : 'false');
    });

    // Yıl değişince kod adları o yılın kodlarından okunur.
    $('#ddlYil').on('change', function () { kodlar.adlariYenile(); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al('api/mptavanil/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            Sgb.yilKutusu('#ddlYil', t.yillar, t.yil);
            if (t.muhasebe) muhBirim.ata({ muhasebe: t.muhasebe, birim: t.birim, kurumsal: t.kurumsal, birimAd: t.birimAd });
        })
        .fail(hataGoster);

    // Tabloyu sayfa alanının (.sayfa-icerik) kalan yüksekliğine yayar; satırlar tablonun içinde kayar.
    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var kap = el.closest('.sayfa-icerik') || document.body, cs = getComputedStyle(kap);
        var kaydirir = /(auto|scroll)/.test(cs.overflowY);
        var kr = kap.getBoundingClientRect(), kay = kaydirir ? kap.scrollTop : 0;
        var gorunen = kaydirir ? kap.clientHeight : window.innerHeight - kr.top;
        var ust = el.getBoundingClientRect().top - kr.top + kay;
        var sonY = 0;
        $(kap).children().each(function () {
            if (this.offsetParent === null && getComputedStyle(this).position !== 'fixed') return;
            sonY = Math.max(sonY, this.getBoundingClientRect().bottom - kr.top + kay + parseFloat(getComputedStyle(this).marginBottom || 0));
        });
        sonY += parseFloat(cs.paddingBottom || 0);
        var bosluk = Math.max(0, sonY - (ust + el.offsetHeight));
        el.style.height = Math.max(enAz, Math.floor(gorunen - ust - bosluk - 1)) + 'px';
    }
    function tabloYay() { yuksekligeYay(document.getElementById('grdMPTavanIl'), 200); }
    $(window).on('resize', tabloYay);
    if (window.ResizeObserver) new ResizeObserver(tabloYay).observe(document.querySelector('.mp-tavan-il .mp-ust'));
    tabloYay();
});
