// Masraf Planı (ButceHazirlikv2/MasrafPlaniGenel; eski MasrafPlaniGenel.aspx "Masraf Planı Veri Girişi"): seçilen aşamada kodlara ve
// harcama birimine uyan tertiplerin tavanı, toplamı ve aylık dağılımı AG Grid'de listelenir (toplamlar alttaki satırda). Tavandan
// MP'ye, AHP'den MP'ye, MP Onayla / Onay Kaldır, Önc. Al, Panoya, raporlar (Rapor.xml 998, 5000-13), hızlı ödenek dosyaları,
// rapor imzaları ve sistem yöneticisine Kontrol Dönemi. Uçlar: /api/masrafplani/* (ButceHazirlik/MasrafPlaniUclari.cs).
$(function () {
    'use strict';

    if (!$('.mp-genel').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var API = 'api/masrafplani';
    var AYLAR = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
    var listelendi = false;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function sonucPencere(baslik, metin) {
        $('#dlgSonucBaslik').text(baslik);
        $('#sonucMetin').text(metin);
        document.getElementById('dlgSonuc').showModal();
    }
    function sonucGoster(v) {
        if (v.hata) { mesaj(v.hata, 'danger'); return false; }
        if (v.bilgi) {
            if (v.uyariVar) sonucPencere('Sonuç', v.bilgi);
            else mesaj(v.bilgi, 'success');
        }
        return true;
    }

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }
    var kodlar = MPTavan.kodAlanlari(yilOku, hataGoster, API);

    function secimOku() {
        return {
            yil: yilOku(), asama: parseInt($('#ddlMPAsama').val(), 10) || 0,
            kur: kodlar.kod('kur'), fon: kodlar.kod('fon'), fin: kodlar.kod('fin'), eko: kodlar.kod('eko'), peb: kodlar.kod('peb'),
            ky: kodlar.kod('ky')
        };
    }
    function yilVarMi(o) {
        if (o.yil > 0) return true;
        mesaj('Yıl giriniz.', 'warning');
        return false;
    }

    // ---------------------------------------------------------------- tablo

    function tutarYaz(p) { return MPTavan.tutar(p.value); }
    function sayiSutun(ozellik) {
        return $.extend({ type: 'rightAligned', width: 112, cellClass: ['sayi'], valueFormatter: tutarYaz }, ozellik);
    }

    var aySutunlari = [];
    for (var i = 0; i < 12; i++) aySutunlari.push(sayiSutun({ field: 'a' + (i + 1), headerName: (i + 1) + '- ' + AYLAR[i] }));

    var sutunlar = [
        {
            headerName: 'Tertip', children: [
                { field: 'tertip', headerName: 'Kodu', width: 280, pinned: 'left', tooltipField: 'tertip' },
                { field: 'ad', headerName: 'Adı', width: 200, tooltipField: 'ad' },
                { field: 'birim', headerName: 'Birimi', width: 200, tooltipField: 'birim' }
            ]
        },
        { headerName: '', children: [sayiSutun({ field: 'tavan', headerName: 'Tavan', width: 125 }), sayiSutun({ field: 'toplam', headerName: 'Toplam', width: 125 })] },
        { headerName: 'Aylar', children: aySutunlari }
    ];

    var grid = Sgb.grid('#grdMPTertipler', {
        columnDefs: sutunlar,
        tooltipShowDelay: 400,
        overlayNoRowsTemplate: '<span>Yılı, aşamayı ve kodları seçip Listele\'ye basınız.</span>'
    });

    var toplamAlanlari = ['tavan', 'toplam', 'a1', 'a2', 'a3', 'a4', 'a5', 'a6', 'a7', 'a8', 'a9', 'a10', 'a11', 'a12'];

    function satirlar() {
        var r = [];
        grid.forEachNodeAfterFilterAndSort(function (n) { if (n.data) r.push(n.data); });
        return r;
    }

    // Eski sütun başlıklarındaki toplamlar: tablonun altında sabit toplam satırı.
    function toplamYenile() {
        var r = satirlar();
        if (!r.length) { grid.setGridOption('pinnedBottomRowData', []); return; }
        var t = { tertip: 'T O P L A M', ad: '', birim: '' };
        $.each(toplamAlanlari, function (i, a) {
            t[a] = 0;
            $.each(r, function (j, d) { t[a] += Number(d[a]) || 0; });
        });
        grid.setGridOption('pinnedBottomRowData', [t]);
    }

    // ---------------------------------------------------------------- Listele (eski btnListele_Click)

    function listele(sessiz) {
        var o = secimOku();
        if (!yilVarMi(o)) return;
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(API + '/liste', o)
            .done(function (v) {
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                if (v.hata) {
                    grid.setGridOption('rowData', []);
                    toplamYenile();
                    if (!sessiz) mesaj(v.hata, 'danger');
                    return;
                }
                listelendi = true;
                grid.setGridOption('rowData', v.satirlar || []);
                toplamYenile();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }
    $('#btnListele').on('click', function () { listele(false); });
    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(false); });

    // ---------------------------------------------------------------- işlemler

    function islem(adres, veri, $d, soru, sonra) {
        if (soru && !window.confirm(soru)) return;
        if ($d) $d.prop('disabled', true);
        Sgb.gonder(API + '/' + adres, veri)
            .done(function (v) { if (sonucGoster(v) && sonra) sonra(v); })
            .fail(hataGoster)
            .always(function () { if ($d) $d.prop('disabled', false); });
    }

    function asamaAd() { return $('#ddlMPAsama option:selected').text(); }

    // Tavandan MP'ye / Tavandan MP'ye (Tertip) (eski TavandanMPYe).
    $('#btnTavandanMPye, #btnTavandanMPyeTertip').on('click', function () {
        var o = secimOku();
        if (!yilVarMi(o)) return;
        var tertip = $(this).attr('data-tertip') === '1';
        o.ay = parseInt($('#ddlAy').val(), 10);
        o.tertip = tertip;
        var soru = o.yil + ' yılı ' + asamaAd() + ' aşamasında ' + (tertip ? 'seçilen kodlara uyan tertiplerin' : 'bütün tertiplerin') +
            ' tavanları ' + $('#ddlAy option:selected').text() + ' ayına dağıtılacak. Onaylıyor musunuz?';
        islem('tavandanmp', o, $(this), soru, function () { if (listelendi) listele(true); });
    });

    // MP Onayla / MP Onay Kaldır (eski btnOnayla_Click, btnOnayKaldir_Click).
    $('#btnOnayla').on('click', function () {
        var o = secimOku();
        if (!yilVarMi(o)) return;
        o.ay = parseInt($('#ddlAy').val(), 10);
        islem('onayla', o, $(this), 'Masraf planı onaylanacak (' + asamaAd() + '). Onaylıyor musunuz?');
    });
    $('#btnOnayKaldir').on('click', function () {
        var o = secimOku();
        if (!yilVarMi(o)) return;
        islem('onaykaldir', o, $(this), 'Masraf planı onayı kaldırılacak (' + asamaAd() + '). Onaylıyor musunuz?');
    });

    // Önc. Al (eski btnAsamaKopyala_Click).
    $('#btnAsamaKopyala').on('click', function () {
        var o = secimOku();
        if (!yilVarMi(o)) return;
        islem('oncal', o, $(this), 'Bir önceki aşamanın masraf planı ' + asamaAd() + ' aşamasına kopyalanacak. Onaylıyor musunuz?',
            function () { if (listelendi) listele(true); });
    });

    // Panoya (eski CopyStore): tablo sekmeyle ayrılmış metin olarak panoya kopyalanır.
    $('#btnClipboard').on('click', function () {
        var r = satirlar();
        if (!r.length) { mesaj('Kopyalanacak satır yok.', 'warning'); return; }
        var basliklar = ['Kodu', 'Adı', 'Birimi', 'Tavan', 'Toplam'].concat(AYLAR.map(function (a, i) { return (i + 1) + '- ' + a; }));
        var sayiYaz = function (v) { return (Number(v) || 0).toLocaleString('tr-TR', { maximumFractionDigits: 2, useGrouping: false }); };
        var metin = [basliklar.join('\t')].concat(r.map(function (d) {
            return [d.tertip || '', d.ad || '', d.birim || ''].concat(toplamAlanlari.map(function (a) { return sayiYaz(d[a]); })).join('\t');
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

    // ---------------------------------------------------------------- AHP'den MP'ye (eski btnAHPdenMPye_Click)

    $('#btnAHPdenMPye').on('click', function () {
        if (yilOku() <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        $('#fuAHPdenMPye').val('').trigger('click');
    });
    $('#fuAHPdenMPye').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        var o = secimOku();
        if (!window.confirm(dosya.name + ' dosyasındaki tertiplerde ' + o.yil + ' yılı ' + asamaAd() +
            ' aşamasının masraf planı silinip AHP\'den yeniden oluşturulacak. Onaylıyor musunuz?')) { this.value = ''; return; }
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('yil', o.yil);
        veri.append('asama', o.asama);
        var $d = $('#btnAHPdenMPye').prop('disabled', true);
        mesaj('Dosya yükleniyor, lütfen bekleyiniz...', 'info');
        $.ajax({
            url: Sgb.KOK + API + '/ahpdenmp', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (sonucGoster(v) && listelendi) listele(true);
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else hataGoster(xhr);
        }).always(function () { $d.prop('disabled', false); $('#fuAHPdenMPye').val(''); });
    });

    // ---------------------------------------------------------------- raporlar (eski btnYazdir_Click, FormUret)

    function indir(adres, p, $d) {
        var o = secimOku();
        if (!yilVarMi(o)) return;
        $d.prop('disabled', true);
        mesaj('Rapor hazırlanıyor...', 'info');
        Sgb.indir(API + '/' + adres, $.extend(o, p)).then(function () { $d.prop('disabled', false); });
    }

    $('#btnYazdir').on('click', function () { indir('rapor', { rapor: $('#ddlRaporListe').val() }, $(this)); });
    $('#btnHizliOdGondUret, #btnHizliOdGondUretC').on('click', function () {
        indir('hizli', { cezali: $(this).attr('data-cezali') }, $(this));
    });

    // ---------------------------------------------------------------- İmzalar (eski MasrafPlaniGenelImza.aspx)

    var dlgImza = document.getElementById('dlgImza');
    $('#btnImza').on('click', function () {
        $('#frmImza input').val('');
        dlgImza.showModal();
        Sgb.al(API + '/imza')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#txtHazGorev').val(v.HazGorev || '');
                $('#txtHazKisi').val(v.HazKisi || '');
                $('#txtImzGorev').val(v.ImzGorev || '');
                $('#txtImzKisi').val(v.ImzKisi || '');
            })
            .fail(hataGoster);
    });
    $('#btnImzaKaydet').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(API + '/imza', {
            HazGorev: $('#txtHazGorev').val(), HazKisi: $('#txtHazKisi').val(),
            ImzGorev: $('#txtImzGorev').val(), ImzKisi: $('#txtImzKisi').val()
        }).done(function (v) { if (sonucGoster(v)) dlgImza.close(); })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });
    $('#frmImza').on('submit', function (e) { e.preventDefault(); $('#btnImzaKaydet').trigger('click'); });

    // ---------------------------------------------------------------- Kontrol Dönemi (eski MasrafPlaniKontrolDonem.aspx)

    var dlgKontrol = document.getElementById('dlgKontrol');
    $('#btnSariKontrol').on('click', function () {
        $('#dlgKontrol input[type=number], #kdTertipler').val('');
        $('#kdKontrolYap').prop('checked', false);
        dlgKontrol.showModal();
        Sgb.al(API + '/kontroldonem')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#kdYil').val(v.yil);
                $('#kdAy').val(v.ay);
                $('#kdHizYil').val(v.hizYil);
                $('#kdKontrolYap').prop('checked', !!v.kontrolYap);
                $('#kdTertipler').val(v.tertipler || '');
            })
            .fail(hataGoster);
    });
    $('#frmKontrol1').on('submit', function (e) {
        e.preventDefault();
        islem('kontroldonem', { Bolum: 1, Yil: $('#kdYil').val(), Ay: $('#kdAy').val(), HizYil: $('#kdHizYil').val() }, $(this).find('button'));
    });
    $('#frmKontrol2').on('submit', function (e) {
        e.preventDefault();
        islem('kontroldonem', { Bolum: 2, KontrolYap: $('#kdKontrolYap').prop('checked'), Tertipler: $('#kdTertipler').val() }, $(this).find('button'));
    });

    // Yıl değişince kod adları o yılın kodlarından okunur.
    $('#ddlYil').on('change', function () { kodlar.adlariYenile(); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al(API + '/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            $('#ddlYil').val(t.yil);
            var $a = $('#ddlMPAsama').empty();
            $.each(t.asamalar || [], function (i, a) { $('<option></option>').val(a.kod).text(a.ad).appendTo($a); });
            $a.val(String(t.asama));
            var $r = $('#ddlRaporListe').empty();
            $.each(t.raporlar || [], function (i, r) { $('<option></option>').val(r.kod).text(r.ad).appendTo($r); });
            kodlar.ata('ky', t.ky || '', t.kyAd || '');
            $('.yonetici').prop('hidden', !t.yonetici);
        })
        .fail(hataGoster);

    // Soru işareti: AHP'den MP'ye dosya formatı açıklamasını gösterir / gizler.
    $('#btnYardim').on('click', function () {
        var $p = $('#pnlIpucu');
        var ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false').toggleClass('active', ac);
    });

    // Tabloyu sayfa alanının (.sayfa-icerik) kalan yüksekliğine yayar; sayfada dikey kaydırma çıkmaz, satırlar tablonun içinde kayar.
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
    function tabloYay() { yuksekligeYay(document.getElementById('grdMPTertipler'), 200); }
    $(window).on('resize', tabloYay);
    if (window.ResizeObserver) new ResizeObserver(tabloYay).observe(document.querySelector('.mp-genel .mp-ust'));
    tabloYay();
});
