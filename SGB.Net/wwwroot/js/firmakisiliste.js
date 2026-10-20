// Firma / Kişi Listesi, HarcamaSurecleri/ListeFirmaKisi (eski aynı adlı Ext1.NET sayfası). Menüden açılır ya da başka bir ekranın
// içinde pencere olarak (Sgb.pencere) açılıp seçilen firmayı o ekrana verir (eski parent.FirmaBilgisiGoster / KazananFirmaBilgiGoster).
// Adres parametreleri: muhasebe, per, yeni, seckapat, firmaSayisi / surecNo / yil (eski sayfadakiyle aynı).
// Uçlar: /api/firmakisi/* (HarcamaSurecleri/FirmaKisiListeUclari.cs).
$(function () {
    'use strict';

    var $kok = $('.firma-kisi-liste');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/firmakisi/';
    var q = new URLSearchParams(window.location.search);
    var TANIM_VAR = String($kok.data('tanim')) === '1';
    var per = q.get('per') || '';
    var yeniFirma = q.get('yeni') || '';
    var secKapat = q.get('seckapat') || '';
    var kazananMod = (parseInt(q.get('firmaSayisi'), 10) || 0) > 1;
    var listelendi = false;

    // Başka bir ekranın penceresinde mi açıldı? Öyleyse seçilen firma o ekrana verilir.
    var ust = null;
    try { if (window.parent && window.parent !== window) ust = window.parent; } catch (e) { ust = null; }
    function ustFonk(ad) {
        try { return ust && typeof ust[ad] === 'function' ? ust[ad] : null; } catch (e) { return null; }
    }
    function pencereKapat() {
        try {
            var d = window.frameElement && window.frameElement.closest('dialog');
            if (d) d.close();
        } catch (e) { /* başka kökenden açıldıysa kapatılamaz */ }
    }
    var secimVar = !!(kazananMod ? ustFonk('KazananFirmaBilgiGoster') : ustFonk('FirmaBilgisiGoster'));

    $('#txtMuhasebe').val(parseInt($kok.data('muhasebe'), 10) || '');

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });

    if (ust && window.frameElement) $('#btnKapat').prop('hidden', false).on('click', pencereKapat);
    if (secimVar) $('#fkIpucu').prop('hidden', false);

    // Eski VeriGoster / FirmaNoGonder: seçilen firma pencereyi açan ekrana verilir.
    function sec(r) {
        var f = ustFonk(kazananMod ? 'KazananFirmaBilgiGoster' : 'FirmaBilgisiGoster');
        if (!r || !f) return;
        if (kazananMod) {
            f(String(r.kod), r.ad);
            pencereKapat();
            return;
        }
        f({
            kod: r.kod, ad: r.ad, vergiNo: r.vergiNo, vergiDairesi: r.vergiDairesi, banka: r.banka, hesapNo: r.hesapNo,
            muhasebe: r.muhasebe, birim: r.birim, firmaYetkilisi: r.firmaYetkilisi, eMail: r.eMail, telefon: r.telefon,
            faks: r.faks, adres: r.adres, uyruk: r.uyruk
        });
        if (!secKapat) pencereKapat();
    }

    // Eski sütun: "Ad (vergi no)" ve kara listedekilerde kırmızı "(Kara Listede)".
    function adHucre(p) {
        var r = p.data || {};
        var $s = $('<span></span>');
        var metin = (r.ad || '') + (r.vergiNo ? ' (' + r.vergiNo + ')' : '');
        if (secimVar) $('<a href="#" class="fk-sec"></a>').text(metin).on('click', function (e) { e.preventDefault(); sec(r); }).appendTo($s);
        else $s.text(metin);
        if (r.karaListe === 1) $('<span class="fk-kara"></span>').text(' (Kara Listede)').appendTo($s);
        return $s[0];
    }

    var sutunlar = kazananMod
        ? [{ field: 'ad', headerName: 'Adı Soyadı/Ünvanı', flex: 1, minWidth: 220, cellRenderer: adHucre }]
        : [
            { field: 'ad', headerName: 'Adı Soyadı/Ünvanı', flex: 2, minWidth: 240, cellRenderer: adHucre, tooltipField: 'ad' },
            { field: 'vergiDairesi', headerName: 'Vergi Dairesi', width: 140 },
            { field: 'banka', headerName: 'Banka', width: 140 },
            { field: 'hesapNo', headerName: 'Hesap No / IBAN', width: 200 },
            { field: 'firmaYetkilisi', headerName: 'Yetkili', width: 140 },
            { field: 'telefon', headerName: 'Telefon', width: 120 },
            { field: 'eMail', headerName: 'E-posta', width: 160 },
            { field: 'adres', headerName: 'Adres', flex: 1, minWidth: 200, tooltipField: 'adres' }
        ];

    var grid = Sgb.grid('#grdListe', {
        columnDefs: sutunlar,
        overlayNoRowsTemplate: kazananMod ? '<span>Kazanan firma bulunamadı.</span>'
            : '<span>Ad Soyad/Ünvan ya da TC Kimlik/Vergi No yazıp Listele\'ye basınız.</span>',
        onRowDoubleClicked: function (p) { sec(p.data); }
    });

    // Eski btnBul_Click
    function listele(sessiz) {
        var muhasebe = $.trim($('#txtMuhasebe').val());
        var ad = $.trim($('#txtAd').val()), vNo = $.trim($('#txtVNo').val());
        if (!muhasebe) { if (!sessiz) mesaj('Firma bilgisinin kayıtlı olduğu Muhasebe kodu bilgisi verilmemiş. Listeleme yapılamıyor.', 'warning'); return; }
        if (!ad && !vNo) { if (!sessiz) mesaj('Lütfen Ad Soyad/Ünvan veya TC Kimlik/Vergi No alanlarında birine kriter yazın.', 'warning'); return; }
        var $d = $('#btnBul').prop('disabled', true);
        grid.setGridOption('loading', true);
        var p = { muhasebe: muhasebe, ad: ad, vergiNo: vNo };
        if (per) p.per = per;
        Sgb.al(UC + 'liste', p)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'warning'); return; }
                listelendi = true;
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun firma/kişi bilgisi bulunamadı.</span>');
                grid.setGridOption('rowData', v.satirlar || []);
                if (v.bilgi && !sessiz) mesaj(v.bilgi, 'info');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); $d.prop('disabled', false); });
    }
    $('#btnBul').on('click', function () { listele(false); });
    $('#frmAra input').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); listele(false); } });

    // Eski FirmaKisiAc / Hazir: Firma Tanımlama (TanimFirmaKisi) pencerede açılır; kapanınca liste yenilenir.
    function tanimAc(yeni) {
        if (!TANIM_VAR) { mesaj('Firma / Kişi Tanımlama ekranı henüz Core\'a taşınmadı.', 'warning'); return; }
        var p = { muhasebe: $.trim($('#txtMuhasebe').val()) };
        if (yeni) p.yeniFirma = yeni;
        Sgb.pencere('../HarcamaSurecleri/TanimFirmaKisi.aspx?' + $.param(p), 'Firma / Kişi Tanımlama', function () {
            if (listelendi) listele(true);
        });
    }
    $('#btnYeni').on('click', function () { tanimAc(''); });

    // Eski FirmaDuzenleme: Firma Birleştirme (FirmaBilgisiDuzenleme) pencerede açılır.
    $('#btnDuzenle').on('click', function () {
        Sgb.pencere('../ButceMuhasebe/FirmaBilgisiDuzenleme.aspx?' + $.param({ muhasebe: $.trim($('#txtMuhasebe').val()) }), 'Firma Birleştirme', function () {
            if (listelendi) listele(true);
        });
    });

    // Tablo sayfanın kalan yüksekliğini doldurur.
    function yay() {
        var el = document.getElementById('grdListe');
        if (!el || el.offsetParent === null) return;
        var ustK = el.getBoundingClientRect().top;
        el.style.height = Math.max(220, Math.floor(window.innerHeight - ustK - 30)) + 'px';
    }
    $(window).on('resize', yay);

    // Eski KazananFirmaYukle: ihalede birden çok kazanan firma varsa arama gizlenir, yalnızca kazananlar listelenir.
    if (kazananMod) {
        $('.fk-sorgu, .fk-kriter').prop('hidden', true);
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'kazanan', { yil: q.get('yil') || '', surecNo: q.get('surecNo') || '' })
            .done(function (v) {
                if (v.hata) {
                    // Eski kodda olduğu gibi okunamazsa arama gösterilir.
                    kazananMod = false;
                    $('.fk-sorgu, .fk-kriter').prop('hidden', false);
                    mesaj(v.hata, 'warning');
                    yay();
                    return;
                }
                grid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    } else {
        $('#txtAd').trigger('focus');
        if (yeniFirma) tanimAc(yeniFirma);
    }
    yay();
});
