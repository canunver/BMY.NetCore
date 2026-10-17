// Masraf Planı Revize Talep (ButceHazirlikv2/MasrafPlaniRevizeTalep; eski MasrafPlaniRevizeTalep.aspx): seçilen saymanlık - birimin
// ve kodların tertiplerinde gönderilen, harcanan ödenek ve masraf planının 12 ayı AG Grid'de listelenir (toplamlar alttaki satırda).
// Liste Yazdır, KBS-SGB Fark, Kopyala, Merkeze Al; satırdaki saat tuşu tertibin aylık harcamalarını açar (eski wndDetay).
// Uçlar: /api/mprevizetalep/* (ButceHazirlik/MPRevizeTalepUclari.cs).
$(function () {
    'use strict';

    if (!$('.mp-revize-talep').length) return;

    var API = 'api/mprevizetalep';
    var AYLAR = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }
    var kodlar = MPTavan.kodAlanlari(yilOku, hataGoster, API);

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: yilOku,
        kurumsalGizle: true,
        degisince: function () { tabloBosalt(); }
    });

    function secimOku() {
        var mb = muhBirim.deger();
        return {
            yil: yilOku(), ilkHali: $('#chkIlkHali').prop('checked') ? 1 : 0,
            muhasebe: mb.muhasebe || '', birim: mb.birim,
            kur: kodlar.kod('kur'), fon: kodlar.kod('fon'), fin: kodlar.kod('fin'), eko: kodlar.kod('eko'), peb: kodlar.kod('peb')
        };
    }
    function secimTamamMi(o) {
        if (o.yil <= 0) { mesaj('Yıl giriniz.', 'warning'); return false; }
        if (!o.muhasebe || !o.birim) { mesaj('Saymanlık ve birim seçiniz.', 'warning'); return false; }
        return true;
    }

    // ---------------------------------------------------------------- tablo

    function tutarYaz(p) { return MPTavan.tutar(p.value); }
    function sayiSutun(ozellik) {
        return $.extend({ type: 'rightAligned', width: 105, cellClass: ['sayi'], valueFormatter: tutarYaz }, ozellik);
    }

    function aylikHucresi(p) {
        if (!p.data || p.node.rowPinned) return '';
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-default btn-xs';
        b.title = 'Aylık harcama durumu';
        b.innerHTML = '<span class="glyphicon glyphicon-time" aria-hidden="true"></span><span class="sr-only">Aylık harcama durumu</span>';
        b.addEventListener('click', function () { aylikAc(p.data.tertip); });
        return b;
    }

    var aySutunlari = [];
    for (var i = 0; i < 12; i++) aySutunlari.push(sayiSutun({ field: 'a' + (i + 1), headerName: AYLAR[i] }));

    var sutunlar = [
        {
            headerName: 'Tertip', children: [
                { field: 'tertip', headerName: 'Tertip', width: 230, pinned: 'left', tooltipField: 'tertip' },
                { field: 'tertipPrg', headerName: 'Program Tertibi', width: 140, tooltipField: 'tertipPrg' },
                { field: 'ekoAd', headerName: 'Eko Ad', width: 130, tooltipField: 'ekoAd' },
                { field: 'pebAd', headerName: 'Faal. Ad', width: 130, tooltipField: 'pebAd' }
            ]
        },
        {
            headerName: 'Ödenek', children: [
                sayiSutun({ field: 'gonderilen', headerName: 'Gönderilen' }),
                sayiSutun({ field: 'harcanan', headerName: 'Harcanan' }),
                sayiSutun({ field: 'harcanabilir', headerName: 'Harcanabilir' }),
                { headerName: '', width: 42, cellRenderer: aylikHucresi, sortable: false, resizable: false, cellClass: 'mp-aylik-tus' }
            ]
        },
        { headerName: 'Masraf Planı', children: aySutunlari.concat([sayiSutun({ field: 'toplam', headerName: 'Toplam' }), sayiSutun({ field: 'gonderilebilir', headerName: 'Gönderilebilir' })]) },
        {
            headerName: '', children: [
                { field: 'sektorAd', headerName: 'Sektör', width: 150, tooltipField: 'sektorAd' },
                { field: 'projeAd', headerName: 'Proje', width: 180, tooltipField: 'projeAd' }
            ]
        }
    ];

    var grid = Sgb.grid('#grdTertipler', {
        columnDefs: sutunlar,
        tooltipShowDelay: 400,
        overlayNoRowsTemplate: '<span>Yılı, saymanlık - birimi ve kodları seçip Listele\'ye basınız.</span>'
    });

    var toplamAlanlari = ['gonderilen', 'harcanan', 'harcanabilir', 'a1', 'a2', 'a3', 'a4', 'a5', 'a6', 'a7', 'a8', 'a9', 'a10', 'a11', 'a12', 'toplam', 'gonderilebilir'];

    function satirlar() {
        var r = [];
        grid.forEachNodeAfterFilterAndSort(function (n) { if (n.data) r.push(n.data); });
        return r;
    }

    function toplamYenile() {
        var r = satirlar();
        if (!r.length) { grid.setGridOption('pinnedBottomRowData', []); return; }
        var t = { tertip: 'T O P L A M' };
        $.each(toplamAlanlari, function (i, a) {
            t[a] = 0;
            $.each(r, function (j, d) { t[a] += Number(d[a]) || 0; });
        });
        grid.setGridOption('pinnedBottomRowData', [t]);
    }

    function tabloBosalt() {
        grid.setGridOption('rowData', []);
        toplamYenile();
    }

    // ---------------------------------------------------------------- Listele (eski btnListele_Click)

    function listele() {
        var o = secimOku();
        if (!secimTamamMi(o)) return;
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(API + '/liste', o)
            .done(function (v) {
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                if (v.hata) { tabloBosalt(); mesaj(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                toplamYenile();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }
    $('#btnListele').on('click', listele);
    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(); });
    $('#chkIlkHali').on('change', function () { if (satirlar().length) listele(); });

    // ---------------------------------------------------------------- dosyalar (eski btnRaporYazdir2_Click, btnRaporYazdir_Click, btnFazlaCek_Click)

    function indir(adres, $d, bilgi) {
        var o = secimOku();
        if (!secimTamamMi(o)) return;
        $d.prop('disabled', true);
        mesaj(bilgi || 'Rapor hazırlanıyor...', 'info');
        Sgb.indir(API + '/' + adres, o).then(function () { $d.prop('disabled', false); });
    }
    $('#btnYazdir2').on('click', function () { indir('yazdir', $(this)); });
    $('#btnYazdir').on('click', function () { indir('fark', $(this)); });
    $('#btnFazlaCek').on('click', function () { indir('merkezeal', $(this), 'Merkeze alma dosyası hazırlanıyor...'); });

    // Kopyala (eski CopyStore): tablo sekmeyle ayrılmış metin olarak panoya kopyalanır.
    $('#btnKopyala').on('click', function () {
        var r = satirlar();
        if (!r.length) { mesaj('Kopyalanacak satır yok.', 'warning'); return; }
        var metinAlanlari = ['tertip', 'tertipPrg', 'ekoAd', 'pebAd'];
        var basliklar = ['Tertip', 'Program Tertibi', 'Eko Ad', 'Faal. Ad', 'Gönderilen', 'Harcanan', 'Harcanabilir']
            .concat(AYLAR, ['Toplam', 'Gönderilebilir', 'Sektör', 'Proje']);
        var sayiYaz = function (v) { return (Number(v) || 0).toLocaleString('tr-TR', { maximumFractionDigits: 2, useGrouping: false }); };
        var metin = [basliklar.join('\t')].concat(r.map(function (d) {
            return metinAlanlari.map(function (a) { return d[a] || ''; })
                .concat(toplamAlanlari.map(function (a) { return sayiYaz(d[a]); }), [d.sektorAd || '', d.projeAd || '']).join('\t');
        })).join('\r\n');
        var tamam = function () { mesaj(r.length + ' satır panoya kopyalandı.', 'success'); };
        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(metin).then(tamam, function () { mesaj('Panoya kopyalanamadı.', 'danger'); });
        } else {
            var $t = $('<textarea class="sr-only"></textarea>').val(metin).appendTo(document.body);
            $t[0].select();
            try { document.execCommand('copy'); tamam(); } catch (e) { mesaj('Panoya kopyalanamadı.', 'danger'); }
            $t.remove();
        }
    });

    // ---------------------------------------------------------------- Aylık harcamalar (eski AylikGuncelle, wndDetay)

    var dlgAylik = document.getElementById('dlgAylik');
    var gridAylik = Sgb.grid('#grdAylik', {
        columnDefs: [
            { field: 'ay', headerName: 'Ay', width: 110, valueFormatter: function (p) { return p.node.rowPinned ? 'Toplam' : (AYLAR[p.value - 1] || p.value); } },
            sayiSutun({ field: 'tutar', headerName: 'Aylık Harcama', flex: 1 })
        ],
        overlayNoRowsTemplate: '<span>Harcama yok.</span>'
    });
    function aylikAc(tertip) {
        var o = secimOku();
        $('#aylikTertip').text(tertip);
        gridAylik.setGridOption('rowData', []);
        gridAylik.setGridOption('pinnedBottomRowData', []);
        dlgAylik.showModal();
        gridAylik.setGridOption('loading', true);
        Sgb.al(API + '/aylik', { yil: o.yil, muhasebe: o.muhasebe, birim: o.birim, tertip: tertip })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                gridAylik.setGridOption('rowData', v.satirlar || []);
                gridAylik.setGridOption('pinnedBottomRowData', [{ ay: 0, tutar: v.toplam || 0 }]);
            })
            .fail(hataGoster)
            .always(function () { gridAylik.setGridOption('loading', false); });
    }

    // Yıl değişince kod adları o yılın kodlarından okunur, liste boşaltılır.
    $('#ddlYil').on('change', function () { kodlar.adlariYenile(); tabloBosalt(); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al(API + '/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            $('#ddlYil').val(t.yil);
            if (t.muhasebe && t.birim) {
                muhBirim.ata({ muhasebe: t.muhasebe, birim: t.birim });
                muhBirim.yenile();
            }
        })
        .fail(hataGoster);

    // Tabloyu sayfa alanının kalan yüksekliğine yayar; satırlar tablonun içinde kayar.
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
    function tabloYay() { yuksekligeYay(document.getElementById('grdTertipler'), 200); }
    $(window).on('resize', tabloYay);
    if (window.ResizeObserver) new ResizeObserver(tabloYay).observe(document.querySelector('.mp-revize-talep .mp-ust'));
    tabloYay();
});
