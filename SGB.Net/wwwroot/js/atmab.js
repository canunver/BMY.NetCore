// AB (IPARD) Takip İşlemleri (AlacakTakip/AlacakTakipListeAB): solda sorgu ve dosya listesi, sağda seçilen dosyanın sekmeleri ve
// borç hareketleri. Listele / Listele (Bek.) (eski btnListele_Click / btnListeleBek_Click), Liste Yazdır ve Sakla (btnYazdir_Click /
// btnSakla_Click), Mut. Raporları, Dönem Faizi (wndHesaplat), Yeni Dosya (YeniBorclu), Yeni İşlem (VeriGirisAc), Raporlar
// (btnRaporlar_Click), İşl. Tarih / Onay Tarih (IslemTarihDegisti), İmzalar (wndImzalar). Satır seçilince dosya okunur (SatirSecildi /
// EkranaKayitYaz). Hareket süzgeçleri (eski chkGoster*) ve getRowClass (yeni satır sarı, iptal satırı üstü çizili) buradadır.
// İşlem pencereleri atmabpencere.js'tedir (AtmAB.islemAc, AtmAB.odemeAc, AtmAB.imzaAc, AtmAB.donemFaiziAc).
// Uçlar: /api/alacaktakip/ab/* (Istemci/AlacakTakip/AlacakTakipListeABUclari.cs, AlacakTakipABIslemUclari.cs).
window.AtmAB = (function ($) {
    'use strict';

    var U = 'api/alacaktakip/ab/';
    var tutarBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var kurBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 4, maximumFractionDigits: 4, useGrouping: false });

    // JSON'da sayı olarak gönderilen alanlar (istek sınıflarında int)
    var SAYI_ALANLARI = ['ipardDonemi', 'iPARDDonemi', 'dosyaAnahtar', 'sira', 'siraNo', 'ilgiliIslemSiraNo', 'tur', 'borcTuru', 'borcluKod',
        'surecAsamasi', 'taksitSayisi', 'tecilFaizNo', 'islemTur'];

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function tutarYaz(n) { return n == null || n === '' || isNaN(n) ? '' : tutarBicim.format(n); }
    function kurYaz(n) { return n == null || n === '' || isNaN(n) ? '' : kurBicim.format(n); }
    function tutarOku(s) {
        s = $.trim(s == null ? '' : String(s));
        if (!s) return 0;
        // Türkçe biçim: binlik nokta, ondalık virgül
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = parseFloat(s.replace(/[^0-9.\-]/g, ''));
        return isNaN(n) ? NaN : n;
    }
    // Kuruş yuvarlaması (eski OrtakFonksiyonlar.KurusYap / Yuvarla(x, 2))
    function kurus(n) { return Math.round((n + (n >= 0 ? 1e-9 : -1e-9)) * 100) / 100; }

    // ---------------------------------------------------------------- alan doldurma / toplama (data-* ile)

    function alanYaz(el, v) {
        var $e = $(el), b = $e.data('bicim');
        if (el.type === 'checkbox') { el.checked = !!v; return; }
        if (v == null) v = '';
        if (b === 'tutar') $e.val(v === '' ? '' : tutarYaz(v));
        else if (b === 'kur') $e.val(v === '' ? '' : kurYaz(v));
        else if (b === 'tarih' && el.type !== 'date') $e.val(Sgb.tarih(v));
        else $e.val(String(v));
    }

    // kok içindeki [attr] alanlarına veri[ad] yazılır; veride olmayan alan boşaltılır (bos: true ise)
    function doldur(kok, veri, attr, bos) {
        veri = veri || {};
        $(kok).find('[' + attr + ']').each(function () {
            var ad = $(this).attr(attr);
            if (Object.prototype.hasOwnProperty.call(veri, ad)) alanYaz(this, veri[ad]);
            else if (bos) alanYaz(this, this.type === 'checkbox' ? false : '');
        });
    }

    function topla(kok, attr) {
        var o = {};
        $(kok).find('[' + attr + ']').each(function () {
            if (this.hasAttribute('data-salt')) return;
            var ad = $(this).attr(attr), b = $(this).data('bicim'), v;
            if (this.type === 'checkbox') v = this.checked;
            else if (b === 'tutar' || b === 'kur') { v = tutarOku($(this).val()); if (isNaN(v)) v = 0; }
            else v = $(this).val() == null ? '' : $.trim(String($(this).val()));
            if (SAYI_ALANLARI.indexOf(ad) >= 0 && typeof v !== 'boolean') v = parseInt(v, 10) || 0;
            o[ad] = v;
        });
        return o;
    }

    // Girilen tutar alanı Türkçe biçime çevrilir
    $(document).on('change', 'input.ab-tutar:not([readonly])', function () {
        var n = tutarOku($(this).val());
        if (!isNaN(n) && $.trim($(this).val()) !== '') $(this).val(tutarYaz(n));
    });

    // ---------------------------------------------------------------- çoklu seçim (eski csDurum / csBorcTuru / cmbBorcunKodu)

    function coklu(element, o) {
        o = o || {};
        var $k = $(element).addClass('ab-coklu').empty();
        var bos = $k.data('bos') || '';
        var $tus = $('<button type="button" class="form-control input-sm ab-coklu-tus" aria-haspopup="true" aria-expanded="false"></button>')
            .attr('aria-label', $k.attr('aria-label') || null).appendTo($k);
        var $panel = $('<div class="ab-coklu-panel" role="listbox" aria-multiselectable="true"></div>').appendTo($k);
        var secenekler = [];

        function ozet() {
            var adlar = [];
            $panel.find('input:checked').each(function () { adlar.push($(this).data('ad')); });
            $tus.text(adlar.length ? adlar.join(', ') : bos).attr('title', adlar.join(', '));
        }
        function ata(liste) {
            secenekler = liste || [];
            $panel.empty();
            $.each(secenekler, function (i, s) {
                $('<label class="ab-coklu-satir"></label>')
                    .append($('<input type="checkbox" />').val(String(s.kod)).data('ad', s.ad))
                    .append(document.createTextNode(' ' + s.ad))
                    .appendTo($panel);
            });
            ozet();
        }
        function deger() {
            return $panel.find('input:checked').map(function () { return this.value; }).get().join(',');
        }
        function sec(degerler) {
            var l = String(degerler || '').split(/[,;]/).map(function (x) { return $.trim(x); });
            $panel.find('input').each(function () { this.checked = l.indexOf(this.value) >= 0; });
            ozet();
        }
        function etkin(evet) {
            $tus.prop('disabled', !evet);
            if (!evet) kapat();
        }
        function kapat() { $k.removeClass('open'); $tus.attr('aria-expanded', 'false'); }

        $tus.on('click', function (e) {
            e.stopPropagation();
            var acik = $k.hasClass('open');
            $('.ab-coklu.open').removeClass('open').children('.ab-coklu-tus').attr('aria-expanded', 'false');
            if (!acik) { $k.addClass('open'); $tus.attr('aria-expanded', 'true'); }
        });
        $panel.on('click', function (e) { e.stopPropagation(); });
        $panel.on('change', 'input', function () { ozet(); if (o.degisince) o.degisince(deger()); });
        $(document).on('click', kapat);

        return { ata: ata, deger: deger, sec: sec, etkin: etkin };
    }

    // ---------------------------------------------------------------- tablo yardımcıları

    function tarihSutun(field, baslik, gen) {
        return { field: field, headerName: baslik, width: gen || 95, valueFormatter: function (p) { return Sgb.tarih(p.value); } };
    }
    function tutarSutun(field, baslik, gen) {
        return { field: field, headerName: baslik, width: gen || 115, type: 'rightAligned', valueFormatter: function (p) { return tutarYaz(p.value); } };
    }
    // eski getRowClass: kayitDurum 1 (Yeni) sarı, 9 (İptal) üstü çizili
    function satirSinif(p) {
        if (!p.data) return null;
        if (p.data.kayitDurum === 1) return 'ab-yeni-satir';
        if (p.data.kayitDurum === 9) return 'ab-iptal-satir';
        return null;
    }
    function acSutun() {
        return {
            colId: 'ac', headerName: '', width: 54, sortable: false, resizable: false,
            cellRenderer: function () {
                var b = document.createElement('button');
                b.type = 'button';
                b.className = 'btn btn-default btn-xs ab-ac';
                b.textContent = 'Aç';
                return b;
            }
        };
    }
    // Taksit Bilgileri tablosu (sekmede ve Taksit Girişi penceresinde)
    function taksitKolonlari() {
        return [
            { field: 'siraNo', headerName: '#', width: 55, type: 'rightAligned' },
            tarihSutun('duzeltimTarihi', 'Tarih'),
            { field: 'taksitGun', headerName: 'Gün', width: 65, type: 'rightAligned' },
            tutarSutun('miktarToplam', 'Ana Para'),
            tutarSutun('gecikmeZammi', 'Gec. Zam.', 100),
            tutarSutun('tecilFaizi', 'Tec. Faiz', 100),
            tutarSutun('toplamToplam', 'Aylık Ödeme'),
            tutarSutun('toplamAB', 'AB (%75)'),
            tutarSutun('toplamUlusal', 'TC (%25)')
        ];
    }

    // ---------------------------------------------------------------- açılır menüler (Bootstrap JS yüklenmiyor)

    $(document).on('click', '.atmab .dropdown-toggle, .ab-pencere .dropdown-toggle', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (this.disabled) return;
        var $g = $(this).closest('.btn-group');
        var acik = $g.hasClass('open');
        $('.btn-group.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false');
        if (!acik) $g.addClass('open').children('.dropdown-toggle').attr('aria-expanded', 'true');
    });
    $(document).on('click', function () {
        $('.btn-group.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false');
    });

    // Alt sekmeler (TL / EUR, Taksit Bilgileri / Taksitlendirme İşlemleri): kardeş .ab-alt-sekme'ler arasında geçiş
    $(document).on('click', '.ab-alt-sekme-tus', function () {
        var ad = $(this).data('alt-sekme');
        var $b = $(this).closest('.ab-alt-sekme-basliklar');
        $b.children('.ab-alt-sekme-tus').each(function () {
            var evet = $(this).data('alt-sekme') === ad;
            $(this).toggleClass('active', evet).attr('aria-selected', evet ? 'true' : 'false');
        });
        $b.parent().children('.ab-alt-sekme').each(function () { this.hidden = $(this).data('alt-sekme') !== ad; });
    });

    return {
        U: U,
        mesaj: mesaj,
        hataGoster: hataGoster,
        tutarYaz: tutarYaz,
        kurYaz: kurYaz,
        tutarOku: tutarOku,
        kurus: kurus,
        alanYaz: alanYaz,
        doldur: doldur,
        topla: topla,
        coklu: coklu,
        tarihSutun: tarihSutun,
        tutarSutun: tutarSutun,
        satirSinif: satirSinif,
        acSutun: acSutun,
        taksitKolonlari: taksitKolonlari,
        tanim: null,
        sayfa: null
    };
})(jQuery);

$(function () {
    'use strict';

    if (!$('#grdListe').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var A = window.AtmAB;
    var U = A.U;
    var mesaj = A.mesaj, hataGoster = A.hataGoster;

    var aktif = { iPARDDonemi: 0, dosyaAnahtar: 0 }; // seçili dosya (eski lblhdniPARDDonemi / hdndosyaAnahtar)
    var dosya = null;                                // son okunan dosya bilgileri

    function tarihler() {
        return { islemTarihi: $('#txtIslemTarih').val() || '', onayTarihi: $('#txtOnayTarih').val() || '' };
    }

    // ---------------------------------------------------------------- sorgu

    var muhBirim = Sgb.muhasebeBirim('#sMuhBirim', { kurumsalGizle: true, adGizle: true });
    var durumSec = A.coklu('#sDurum');
    var borcTuruSec = A.coklu('#sBorcTuru');

    function kriter() {
        var mb = muhBirim.deger();
        return $.extend({
            ipard: parseInt($('#sIpard').val(), 10) || 0,
            durum: durumSec.deger(),
            muhasebe: mb.muhasebe ? String(mb.muhasebe) : '',
            birim: mb.birim || '',
            aramaMetni: $.trim($('#sArama').val()),
            borcTuru: borcTuruSec.deger(),
            borcDurumu: parseInt($('#sBorcDurumu').val(), 10) || 0,
            avans: $('#sAvans').prop('checked'),
            tarihTur: $('#sTarihTur').val() || '',
            islTarih: $('#sIslTarih').val() || '',
            islTarihSon: $('#sIslTarihSon').val() || ''
        }, tarihler());
    }

    // ---------------------------------------------------------------- dosya listesi (eski grdListe)

    var grdListe = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'defterSirasi', headerName: '#', width: 70 },
            { field: 'HrcBirimAd', headerName: 'İl Koordinatörlüğü', width: 150, tooltipField: 'HrcBirimAd' },
            { field: 'borcluKodAd', headerName: 'Borçlu', flex: 1, minWidth: 180, tooltipField: 'borcluKodAd' },
            { field: 'geriAlimSurecAsamasiAd', headerName: 'Süreç Aşaması', width: 150, tooltipField: 'geriAlimSurecAsamasiAd' },
            { field: 'borcTuruAd', headerName: 'Borcun Kodu', width: 140, tooltipField: 'borcTuruAd' },
            A.tutarSutun('kalanToplamToplam', 'Kalan Toplam'),
            A.tutarSutun('kalanMiktarToplam', 'Kalan Ana Para'),
            A.tutarSutun('geriAlimMiktarToplam', 'Ana Para'),
            A.tutarSutun('islemYapilanMiktarToplam', 'İşlem Yapılan')
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        getRowId: function (p) { return p.data.iPARDDonemi + '-' + p.data.dosyaAnahtar; },
        overlayNoRowsTemplate: '<span>Sorgu alanlarını doldurup Listele\'ye basınız.</span>',
        pagination: true,
        paginationPageSize: 250,
        paginationPageSizeSelector: false,
        onSelectionChanged: function () {
            var s = grdListe.getSelectedRows()[0];
            if (s && (s.iPARDDonemi !== aktif.iPARDDonemi || s.dosyaAnahtar !== aktif.dosyaAnahtar)) dosyaAc(s.iPARDDonemi, s.dosyaAnahtar);
        }
    });

    function listele(bekleyen) {
        var $t = $('#btnListele, #btnListeleBek').prop('disabled', true);
        grdListe.setGridOption('loading', true);
        Sgb.al(U + 'liste', $.extend(kriter(), { bekleyen: !!bekleyen }))
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var s = v.satirlar || [];
                grdListe.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun bilgi bulunamadı.</span>');
                grdListe.setGridOption('rowData', s);
                $('#lblSayi').text(s.length ? '(' + s.length + ')' : '');
                if (v.bilgi) mesaj(v.bilgi, 'info');
            })
            .fail(hataGoster)
            .always(function () { $t.prop('disabled', false); grdListe.setGridOption('loading', false); });
    }

    $('#btnListele').on('click', function () { listele(false); });
    $('#btnListeleBek').on('click', function () { listele(true); });
    $('#frmSorgu').on('submit', function (e) { e.preventDefault(); listele(false); });

    // ---------------------------------------------------------------- sekmeler

    $('.ab-sekme-basliklar').on('click', '.ab-sekme-tus', function () {
        var ad = $(this).data('sekme');
        $('.ab-sekme-tus').each(function () {
            var evet = $(this).data('sekme') === ad;
            $(this).toggleClass('active', evet).attr('aria-selected', evet ? 'true' : 'false');
        });
        $('.ab-sekme').each(function () {
            var evet = $(this).data('sekme') === ad;
            this.hidden = !evet;
            $(this).toggleClass('active', evet);
        });
        cerceveYukle();
    });

    // Dosyalar / Tarihçe çerçeveleri yalnızca sekme açıkken ve dosya seçiliyken yüklenir (eski pnlDosyalar / pnlTarihce)
    function cerceveAdres(ad) {
        if (ad === 'dosyalar')
            return '../Duyuru/EkliDosyalarForm?' + $.param({ mdl: 'ATMAB', frm: aktif.dosyaAnahtar, menuYok: 1 });
        return '../AlacakTakip/AlacakTakipABTarihce?' + $.param({ dosyaAnahtar: aktif.dosyaAnahtar, iPARDDonemi: aktif.iPARDDonemi, menuYok: 1 });
    }
    function cerceveYukle() {
        var $s = $('.ab-cerceve-sekme.active');
        if (!$s.length) return;
        var ad = $s.data('sekme');
        var $f = $s.find('iframe'), $b = $s.find('.ab-bos');
        if (!(aktif.dosyaAnahtar > 0)) { $f.prop('hidden', true).attr('src', 'about:blank').removeData('yuklu'); $b.prop('hidden', false); return; }
        var adres = cerceveAdres(ad);
        $b.prop('hidden', true);
        $f.prop('hidden', false);
        if ($f.data('yuklu') !== adres) { $f.attr('src', adres).data('yuklu', adres); }
    }
    function cerceveleriBayatla() { $('.ab-cerceve-sekme iframe').removeData('yuklu'); }

    // ---------------------------------------------------------------- parça tabloları (eski grdBorcluIslemler, ... , grdIslemler)

    function parcaGrid(sec, kolonlar, tabloKod) {
        var g = Sgb.grid(sec, {
            columnDefs: [A.acSutun()].concat(kolonlar),
            getRowClass: A.satirSinif,
            overlayNoRowsTemplate: '<span>Kayıt yok.</span>',
            onCellClicked: function (e) {
                if (!e.event || !$(e.event.target).closest('.ab-ac').length) return;
                A.islemAc(tabloKod != null ? tabloKod : e.data.tabloKod, e.data.sira);
            }
        });
        return g;
    }
    var ortakSon = [
        { field: 'kaydedenAdSoyad', headerName: 'Kaydeden', width: 140 },
        { field: 'onaylayanAdSoyad', headerName: 'Onaylayan', width: 140 },
        { field: 'kayitDurumAd', headerName: 'Durum', width: 95 },
        { field: 'gerekce', headerName: 'Gerekçe', flex: 1, minWidth: 140, tooltipField: 'gerekce' }
    ];
    var grdBorclu = parcaGrid('#grdBorcluIslem', [
        A.tarihSutun('basTarih', 'Tarih'),
        { field: 'defterSirasi', headerName: 'Sıra', width: 70 },
        { field: 'kaydedenAdSoyad', headerName: 'Kaydeden', width: 140 },
        { field: 'onaylayanAdSoyad', headerName: 'Onaylayan', width: 140 },
        { field: 'borcluKodAd', headerName: 'Borçlu', width: 180, tooltipField: 'borcluKodAd' },
        { field: 'birimAd', headerName: 'Birim', width: 140 }
    ].concat(ortakSon.slice(2)), 1);
    var grdOnBorc = parcaGrid('#grdOnBorcIslem', [
        A.tarihSutun('basTarih', 'Tarih'),
        { field: 'borcTuruAd', headerName: 'Borç Türü', width: 140 },
        A.tarihSutun('onBorcTarihi', 'Ön B.Tarih'),
        A.tutarSutun('onBorcToplam', 'Toplam Borç')
    ].concat(ortakSon), 2);
    var grdGak = parcaGrid('#grdGakIslem', [
        A.tarihSutun('basTarih', 'Tarih'),
        A.tarihSutun('geriAlimKarariTarihi', 'Geri A. K. T.'),
        A.tarihSutun('temerrutTarihi', 'Temerrüt T.'),
        A.tutarSutun('gakToplam', 'G.A. Toplam Borç', 130)
    ].concat(ortakSon), 3);
    var grdDurum = parcaGrid('#grdDurumIslem', [
        A.tarihSutun('basTarih', 'Tarih'),
        { field: 'surecAsamasiAd', headerName: 'Aşama', width: 200, tooltipField: 'surecAsamasiAd' }
    ].concat(ortakSon), 4);
    var grdTaksitIslem = parcaGrid('#grdTaksitIslem', [
        A.tarihSutun('basTarih', 'Tarih'),
        A.tarihSutun('taksitTalepTarihi', 'Talep T.'),
        { field: 'taksitSayisi', headerName: 'Taksit S.', width: 85, type: 'rightAligned' }
    ].concat(ortakSon), 5);
    // İşlem Listesi ve Onaylama (eski grdIslemler: Aç → IslemAcOnayla, tabloKod satırdan)
    var grdIslemler = parcaGrid('#grdIslemler', [
        A.tarihSutun('kayitTarihi', 'Tarih'),
        { field: 'tabloKodAd', headerName: 'Tür', width: 140 },
        { field: 'gerekce', headerName: 'Gerekçe', flex: 1, minWidth: 160, tooltipField: 'gerekce' },
        { field: 'kaydedenAdSoyad', headerName: 'Kaydeden', width: 150 },
        { field: 'kayitDurumAd', headerName: 'Durum', width: 95 }
    ], null);

    var grdTaksitler = Sgb.grid('#grdTaksitler', {
        columnDefs: A.taksitKolonlari(),
        getRowClass: A.satirSinif,
        overlayNoRowsTemplate: '<span>Taksit yok.</span>'
    });

    // ---------------------------------------------------------------- borç hareketleri (eski grdBorcHareket)

    var GOSTER_VARSAYILAN = { faiz: false, faizIslem: false, odeme: true, silme: true, duzeltme: true };
    function gosterOku(ad) {
        try {
            var v = window.localStorage.getItem('atmab.goster.' + ad);
            if (v === '1') return true;
            if (v === '0') return false;
        } catch (e) { /* saklama kapalı */ }
        return GOSTER_VARSAYILAN[ad];
    }
    function gosterYaz(ad, evet) {
        try { window.localStorage.setItem('atmab.goster.' + ad, evet ? '1' : '0'); } catch (e) { /* saklama kapalı */ }
    }
    $('[data-goster]').each(function () { this.checked = gosterOku($(this).data('goster')); });

    function odemeTuruMu(t) { return t === 51 || t === 54 || t === 52 || t === 59 || t === 53; }
    // eski TurVarMi
    function turVarMi(t, g) {
        if (t === 1 && g.faiz) return true;
        if (t === 2 && g.faizIslem) return true;
        if ((odemeTuruMu(t) || t === 62) && g.odeme) return true;
        if ((t === 61 || t === 52) && g.taksit) return true;
        if ((t === 151 || t === 152) && g.silme) return true;
        if (t === 102 && g.duzeltme) return true;
        return false;
    }
    function suzgec() {
        var g = { taksit: false };
        $('[data-goster]').each(function () { g[$(this).data('goster')] = this.checked; });
        return g;
    }

    function komutSutun() {
        return {
            colId: 'komut', headerName: '', width: 190, sortable: false, resizable: false,
            cellRenderer: function () {
                var s = document.createElement('span');
                s.className = 'ab-komutlar';
                [['1', 'Aç', 'Aç'], ['2', 'Sil', 'Sil'], ['3', 'Akt.', 'Ödenen tutarı aktar'], ['4', 'İade', 'İade et']].forEach(function (k) {
                    var b = document.createElement('button');
                    b.type = 'button';
                    b.className = 'btn btn-default btn-xs' + (k[0] === '2' ? ' ab-sil' : '');
                    b.setAttribute('data-komut', k[0]);
                    b.title = k[2];
                    b.textContent = k[1];
                    s.appendChild(b);
                });
                return s;
            }
        };
    }
    function grup(baslik, cocuk) { return { headerName: baslik, children: [cocuk] }; }
    function toplamBaslik(d, ad, kur) {
        var v = d ? d[ad] : 0;
        return kur ? A.kurYaz(v || 0) : A.tutarYaz(v || 0);
    }
    // Sütun gruplarının altında dosyanın işlem toplamları yazılır (eski GridBaslikToplamYaz)
    function hareketKolonlari(d) {
        function t(field, ad, kur) {
            var s = A.tutarSutun(field, toplamBaslik(d, ad, kur), kur ? 90 : 110);
            if (kur) s.valueFormatter = function (p) { return A.kurYaz(p.value); };
            return s;
        }
        return [
            grup('', komutSutun()),
            grup('Tarih', A.tarihSutun('tarih', '', 90)),
            grup('İşlem Tür', { field: 'turAd', headerName: '', width: 120, tooltipField: 'turAd' }),
            grup('Karar', { field: 'karar', headerName: '', width: 110, tooltipField: 'karar' }),
            grup('Tutar (AB)', t('miktarAB', 'islemYapilanMiktarAB')),
            grup('Tutar (Ulusal)', t('miktarUlusal', 'islemYapilanMiktarUlusal')),
            grup('Faiz (AB)', t('faizAB', 'islemYapilanFaizMiktarAB')),
            grup('Faiz (Ulusal)', t('faizUlusal', 'islemYapilanFaizMiktarUlusal')),
            grup('Tutar Toplam', t('toplamToplam', 'islemYapilanToplamToplam')),
            grup('Kur', t('dovizKuru', 'geriAlimDovizKuru', true)),
            grup('Tutar € (AB)', t('miktarABEUR', 'islemYapilanMiktarABEUR')),
            grup('Tutar € (Ulusal)', t('miktarUlusalEUR', 'islemYapilanMiktarUlusalEUR')),
            grup('Faiz € (AB)', t('faizABEUR', 'islemYapilanFaizMiktarABEUR')),
            grup('Faiz € (Ulusal)', t('faizUlusalEUR', 'islemYapilanFaizMiktarUlusalEUR')),
            grup('Tutar € Toplam', t('toplamToplamEUR', 'islemYapilanToplamToplamEUR')),
            grup('Durum', { field: 'kayitDurumAd', headerName: '', width: 85 }),
            grup('K. Tarih', A.tarihSutun('kayitTarihi', '', 90)),
            grup('Kaydeden', { field: 'kaydedenAdSoyad', headerName: '', width: 140 }),
            grup('O. Tarih', A.tarihSutun('onayTarihi', '', 90)),
            grup('Onaylayan', { field: 'onaylayanAdSoyad', headerName: '', width: 140 })
        ];
    }

    var grdHareket = Sgb.grid('#grdHareket', {
        columnDefs: hareketKolonlari(null),
        getRowClass: A.satirSinif,
        getRowId: function (p) { return String(p.data.siraNo) + '-' + p.data.tur; },
        overlayNoRowsTemplate: '<span>Hareket yok.</span>',
        onCellClicked: function (e) {
            var b = e.event && $(e.event.target).closest('[data-komut]');
            if (!b || !b.length) return;
            var k = String(b.data('komut')), s = e.data;
            if (k === '2') hareketSil(s);
            else if (k === '3') A.odemeAc(62, 0, s.siraNo);
            else if (k === '4') A.odemeAc(53, 0, s.siraNo);
            else A.odemeAc(-1, s.siraNo, 0);
        },
        onRowDoubleClicked: function (e) {
            if (e.event && $(e.event.target).closest('[data-komut]').length) return;
            A.odemeAc(-1, e.data.siraNo, 0);
        }
    });

    function hareketleriYaz() {
        var h = dosya ? dosya.hareketler || [] : [];
        var g = suzgec();
        grdHareket.setGridOption('rowData', h.filter(function (x) { return turVarMi(x.tur, g); }));
        grdTaksitler.setGridOption('rowData', h.filter(function (x) { return turVarMi(x.tur, { taksit: true }); }));
    }

    $('[data-goster]').on('change', function () {
        gosterYaz($(this).data('goster'), this.checked);
        hareketleriYaz();
    });

    // eski OdemeSil
    function hareketSil(s) {
        if (!window.confirm('Seçilen satır silinecektir. Onaylıyor musunuz?')) return;
        Sgb.gonder(U + 'odeme/sil', { iPARDDonemi: aktif.iPARDDonemi, dosyaAnahtar: aktif.dosyaAnahtar, siraNo: s.siraNo })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
                yenile();
            })
            .fail(hataGoster);
    }

    // ---------------------------------------------------------------- dosya (eski SatirSecildi / EkranaKayitYaz)

    function tuslar() {
        var var_ = aktif.dosyaAnahtar > 0;
        $('#btnYeniIslem, #btnRaporlar, #btnTaksitYazdir').prop('disabled', !var_);
        $('[data-yeni-parca]').prop('disabled', !var_);
    }

    function temizle() {
        dosya = null;
        A.doldur('.bolum-dosya', {}, 'data-d', true);
        $('.bolum-dosya .ab-fazla-odeme').prop('hidden', true);
        $('#lblDosya').text('');
        $.each([grdBorclu, grdOnBorc, grdGak, grdDurum, grdTaksitIslem, grdIslemler], function (i, g) { g.setGridOption('rowData', []); });
        grdHareket.setGridOption('columnDefs', hareketKolonlari(null));
        hareketleriYaz();
        tuslar();
        cerceveleriBayatla();
        cerceveYukle();
    }

    function dosyaYaz(d) {
        dosya = d;
        aktif = { iPARDDonemi: d.iPARDDonemi, dosyaAnahtar: d.dosyaAnahtar };
        A.doldur('.bolum-dosya', d, 'data-d', true);
        $('.bolum-dosya .ab-fazla-odeme').prop('hidden', !d.fazlaOdeme);
        var $lbl = $('#lblDosya').text('(' + (d.iPARDDonemiAd || '') + (d.borcluKodAd ? ' · ' + d.borcluKodAd : '') + ')');
        // Süreç durumu başlıkta ayrı renkle (eski alt satırdaki Süreç Durum alanı)
        if (d.geriAlimSurecAsamasiAd) $('<span class="ab-surec-baslik">').append('Süreç Durum: ', $('<b>').text(d.geriAlimSurecAsamasiAd)).appendTo($lbl);
        grdBorclu.setGridOption('rowData', d.borcluIslemler || []);
        grdOnBorc.setGridOption('rowData', d.onBorcIslemler || []);
        grdGak.setGridOption('rowData', d.gakIslemler || []);
        grdDurum.setGridOption('rowData', d.durumIslemler || []);
        grdTaksitIslem.setGridOption('rowData', d.taksitIslemler || []);
        grdIslemler.setGridOption('rowData', d.islemler || []);
        grdHareket.setGridOption('columnDefs', hareketKolonlari(d));
        hareketleriYaz();
        tuslar();
        cerceveleriBayatla();
        cerceveYukle();
    }

    function dosyaAc(ipard, anahtar) {
        if (!(ipard > 0 && anahtar > 0)) { aktif = { iPARDDonemi: ipard || 0, dosyaAnahtar: anahtar || 0 }; temizle(); return $.Deferred().resolve().promise(); }
        aktif = { iPARDDonemi: ipard, dosyaAnahtar: anahtar };
        grdHareket.setGridOption('loading', true);
        return Sgb.al(U + 'dosya', $.extend({ iPARDDonemi: ipard, dosyaAnahtar: anahtar }, tarihler()))
            .done(function (d) {
                if (d.hata) { mesaj(d.hata, 'danger'); return; }
                dosyaYaz(d);
            })
            .fail(hataGoster)
            .always(function () { grdHareket.setGridOption('loading', false); });
    }

    // eski SayfaYenile: pencereler kapanınca / kayıttan sonra dosya yeniden okunur
    function yenile() { return dosyaAc(aktif.iPARDDonemi, aktif.dosyaAnahtar); }

    $('#txtIslemTarih, #txtOnayTarih').on('change', function () { if (aktif.dosyaAnahtar > 0) yenile(); });

    // ---------------------------------------------------------------- araç çubuğu

    // eski YeniBorclu: anahtarlar sıfırlanır, Borç Bilgisi Girişi yeni dosya için açılır
    $('#btnYeniDosya').on('click', function () {
        grdListe.deselectAll();
        aktif = { iPARDDonemi: 0, dosyaAnahtar: 0 };
        temizle();
        A.islemAc(1, 0);
    });

    // eski VeriGirisAc (Yeni İşlem menüsü)
    $('.ab-arac').on('click', '[data-yeni-islem]', function (e) {
        e.preventDefault();
        A.odemeAc(parseInt($(this).data('yeni-islem'), 10), 0, 0);
    });

    // Sekmelerdeki "Yeni ... Değişikliği" tuşları (eski IslemEkranAc(tabloKod, 0))
    $('[data-yeni-parca]').on('click', function () { A.islemAc(parseInt($(this).data('yeni-parca'), 10), 0); });

    // Liste Yazdır / Mut. Raporları (eski btnYazdir_Click)
    $('.ab-arac').on('click', '[data-yazdir]', function (e) {
        e.preventDefault();
        Sgb.indir(U + 'yazdir', $.extend({ no: parseInt($(this).data('yazdir'), 10) }, kriter()));
    });

    // Sakla (eski btnSakla_Click): rapor üretilir ve Saklı Raporlar'a konur
    $('.ab-arac').on('click', '[data-sakla]', function (e) {
        e.preventDefault();
        var $a = $(this);
        if ($a.hasClass('ab-mesgul')) return;
        $a.addClass('ab-mesgul');
        mesaj('Rapor hazırlanıyor...', 'info');
        Sgb.gonder(U + 'sakla', $.extend({ no: parseInt($a.data('sakla'), 10) }, kriter()))
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $a.removeClass('ab-mesgul'); });
    });

    // Dosya raporları (eski btnRaporlar_Click)
    $('.ab-arac').on('click', '[data-rapor]', function (e) {
        e.preventDefault();
        if (!(aktif.dosyaAnahtar > 0)) return;
        var $a = $(this);
        var p = $.extend({ no: $a.data('rapor'), tur: $a.data('tur') || 1, iPARDDonemi: aktif.iPARDDonemi, dosyaAnahtar: aktif.dosyaAnahtar }, tarihler());
        if ($a.data('pb')) p.pb = $a.data('pb');
        Sgb.indir(U + 'rapor', p);
    });

    // Taksitlendirme sekmesindeki Yazdır: kayıtlı taksitler (eski btnTaksitYazdir_Click, Kayitli=1)
    $('#btnTaksitYazdir').on('click', function () {
        if (!(aktif.dosyaAnahtar > 0)) return;
        Sgb.indir(U + 'rapor/taksit', $.extend({ kayitli: 1, iPARDDonemi: aktif.iPARDDonemi, dosyaAnahtar: aktif.dosyaAnahtar }, tarihler()));
    });

    $('#btnImzalar').on('click', function () { A.imzaAc(); });
    $('#btnDonemFaizi').on('click', function () { A.donemFaiziAc(); });

    // ---------------------------------------------------------------- pencerelerin kullandığı sayfa durumu

    A.sayfa = {
        anahtar: function () { return { iPARDDonemi: aktif.iPARDDonemi, dosyaAnahtar: aktif.dosyaAnahtar }; },
        dosyaAyarla: function (ipard, anahtar) { aktif = { iPARDDonemi: ipard || 0, dosyaAnahtar: anahtar || 0 }; },
        tarihler: tarihler,
        dosya: function () { return dosya; },
        yenile: yenile,
        ipard: function () { return parseInt($('#sIpard').val(), 10) || 0; },
        muhasebe: function () { return muhBirim.deger().muhasebe || 0; }
    };

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    function secenekYaz(sel, liste) {
        var $s = $(sel).empty();
        $.each(liste || [], function (i, x) { $('<option></option>').attr('value', x.kod).text(x.ad).appendTo($s); });
    }

    temizle();
    Sgb.al(U + 'tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            A.tanim = v;
            secenekYaz('#sIpard', v.ipardlar);
            $('#sIpard').val('1');
            durumSec.ata(v.durumlar);
            borcTuruSec.ata(v.borcTurleri);
            muhBirim.ata({ muhasebe: v.muhasebe, birim: v.birim });
            if (v.muhasebe) muhBirim.yenile();
            $(document).trigger('atmab:tanim', [v]);
        })
        .fail(hataGoster);
});
