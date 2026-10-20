// Geçici / Sürekli Görev Gündelik Tanım Formu (HarcamaSurecleri/TanimPSLSurekliGeciciGorevGundelik; eski aynı adlı .aspx).
// Üstte form, altta tanımlar (AG Grid, salt okunur). Eski sayfadaki gibi açılışta liste okunmaz; Bul ile listelenir.
// Tür seçimi alanları gösterir/gizler (eski PanelGosterGizle): 1 Sürekli-Geçici: derece + ek gösterge, 2 Denetim: hiçbiri,
// 3 Yurtdışı: ülke + derece + ek gösterge. Listede koda tıklanınca satır forma gelir (eski VeriGoster).
// Kaydet ve Sil sonrası yıl + türe göre liste yenilenir, başarılıysa form temizlenir (eski Arama + Temizle).
$(function () {
    'use strict';

    if (!$('#grdGundelik').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var API = 'api/gundeliktanim';
    var degerBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('.gt-arac .btn').not('#btnYardim').prop('disabled', evet); }
    function tamSayi(sec) { return parseInt($(sec).val(), 10) || 0; }

    // Türkçe (1.234,56) ve noktalı (1234.56) yazımı kabul edilir; boş ya da hatalı -> 0 (eski ConvertToDouble).
    function sayiOku(deger) {
        var s = $.trim(String(deger == null ? '' : deger)).replace(/\s/g, '');
        if (s === '') return 0;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = parseFloat(s);
        return isNaN(n) ? 0 : n;
    }

    // ---------------------------------------------------------------- yükseklik: liste sayfanın kalan yüksekliğini doldurur

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
    function yay() { yuksekligeYay(document.getElementById('grdGundelik'), 240); }
    $(window).on('resize', yay);

    $('#btnYardim').on('click', function () {
        var $m = $('#gtYardim'), acik = $m.prop('hidden');
        $m.prop('hidden', !acik);
        $(this).attr('aria-expanded', acik ? 'true' : 'false').toggleClass('active', acik);
        yay();
    });

    // ---------------------------------------------------------------- liste

    var grid = Sgb.grid('#grdGundelik', {
        columnDefs: [
            { field: 'yil', headerName: 'Yıl', width: 75, cellClass: 'text-center' },
            {
                field: 'kod', headerName: 'Kod', width: 80, cellClass: 'text-center',
                cellRenderer: function (p) {
                    var a = document.createElement('a');
                    a.href = '#';
                    a.textContent = p.value == null ? '' : p.value;
                    a.addEventListener('click', function (e) { e.preventDefault(); doldur(p.data); });
                    return a;
                }
            },
            {
                field: 'deger', headerName: 'Değer', width: 110, cellClass: 'text-right', headerClass: 'gt-sag-baslik',
                valueFormatter: function (p) { return p.value == null ? '' : degerBicim.format(p.value); }
            },
            { field: 'paraBirimi', headerName: 'Para Birimi', width: 100, cellClass: 'text-center' },
            { field: 'ulke', headerName: 'Ülke', width: 140, tooltipField: 'ulke' },
            { field: 'ayKadroDerece', headerName: 'Aylık Kadro Derece', width: 140, cellClass: 'text-center' },
            { field: 'ekGosterge', headerName: 'Ek Gösterge', width: 120, cellClass: 'text-center' },
            { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 220, tooltipField: 'ad' },
            { field: 'turAd', headerName: 'Tür', width: 180 }
        ],
        rowSelection: 'single',
        onRowDoubleClicked: function (e) { doldur(e.data); },
        overlayNoRowsTemplate: '<span class="text-muted">Tür ve yılı seçip Bul\'a basınız.</span>'
    });

    // ---------------------------------------------------------------- form

    var $tur = $('#cmbTur'), $yil = $('#txtYil');

    // Eski PanelGosterGizle
    function panelGoster() {
        var t = $tur.val();
        $('#pnlUlkeler').prop('hidden', t !== '3');
        $('#pnlAylikKadroDerece, #pnlEkGosterge').prop('hidden', t === '2');
        yay();
    }
    $tur.on('change', panelGoster);

    function seciliYil() { return tamSayi('#txtYil'); }

    // Eski Temizle: kod, değer, ad, derece ve ek gösterge boşalır; tür, yıl, ülke ve para birimi kalır.
    function temizle() {
        $('#txtKod, #txtGundelik, #txtAd, #txtAylikKadroDereceBaslangic, #txtAylikKadroDereceBitis, #txtEkGostergeBaslangic, #txtEkGostergeBitis').val('');
        grid.deselectAll();
    }

    // Eski VeriGoster: satırın bilgileri forma gelir.
    function doldur(o) {
        if (!o) return;
        if (o.yil) $yil.val(String(o.yil));
        $('#txtKod').val(o.kod || '');
        $('#txtGundelik').val(o.deger == null ? '' : degerBicim.format(o.deger));
        $('#txtAd').val(o.ad || '');
        $('#cmbUlke').val(String(o.ulkeKod || 0));
        if ($('#cmbUlke').val() === null) $('#cmbUlke').val('0');
        if (o.paraBirimiKod) $('#cmbParaBirimi').val(String(o.paraBirimiKod));
        $('#txtAylikKadroDereceBaslangic').val(o.ayKadroDereceBaslangic);
        $('#txtAylikKadroDereceBitis').val(o.ayKadroDereceBitis);
        $('#txtEkGostergeBaslangic').val(o.ekGostergeBaslangic);
        $('#txtEkGostergeBitis').val(o.ekGostergeBitis);
    }

    function formOku() {
        return {
            yil: seciliYil(),
            tur: parseInt($tur.val(), 10) || 0,
            kod: $.trim($('#txtKod').val()),
            ad: $.trim($('#txtAd').val()),
            ulkeKod: tamSayi('#cmbUlke'),
            ayKadroDereceBaslangic: tamSayi('#txtAylikKadroDereceBaslangic'),
            ayKadroDereceBitis: tamSayi('#txtAylikKadroDereceBitis'),
            ekGostergeBaslangic: tamSayi('#txtEkGostergeBaslangic'),
            ekGostergeBitis: tamSayi('#txtEkGostergeBitis'),
            deger: sayiOku($('#txtGundelik').val()),
            paraBirimiKod: tamSayi('#cmbParaBirimi')
        };
    }

    function secenekDoldur($c, liste, metin, bosMetin) {
        $c.empty();
        if (bosMetin) $('<option></option>').val('0').text(bosMetin).appendTo($c);
        $.each(liste || [], function (i, o) { $('<option></option>').val(String(o.kod)).text(metin(o)).appendTo($c); });
    }

    // ---------------------------------------------------------------- listeleme

    function listeyiYaz(v, sessiz) {
        if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
        grid.setGridOption('rowData', v.satirlar || []);
        if (!sessiz && v.bilgi) mesaj(v.bilgi, 'info');
    }

    // kriter: { yil, tur, kod, ad }. sessiz: kayıt yoksa bilgi mesajı gösterilmez (Kaydet/Sil sonrası yenilemede başarı mesajı kalsın diye).
    function listele(kriter, sessiz) {
        grid.setGridOption('loading', true);
        return Sgb.gonder(API + '/bul', kriter)
            .done(function (v) { listeyiYaz(v, sessiz); })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    // Eski Arama: yıl (boşsa bu yıl) ve türe göre.
    function arama() {
        return listele({ yil: seciliYil() || new Date().getFullYear(), tur: parseInt($tur.val(), 10) || 1, kod: '', ad: '' }, true);
    }

    // Eski btnAra_Click (KriterTopla): yıl, tür, kod, ad.
    $('#btnBul').on('click', function () {
        var k = formOku();
        mesgul(true);
        listele({ yil: k.yil, tur: k.tur, kod: k.kod, ad: k.ad }, false).always(function () { mesgul(false); });
    });

    $('#btnKaydet').on('click', function () {
        var k = formOku(), hata = [];
        if (k.yil <= 0) hata.push('Yıl boş bırakılamaz.');
        if (k.kod === '') hata.push('Gündelik Tanım Kodu boş bırakılamaz.');
        if (hata.length) { mesaj(hata.join(' ') + ' Kayıt işlemi yapılamadı.', 'warning'); return; }
        if (!window.confirm('Kayıt işlemi yapılacak. Onaylıyor musunuz?')) return;
        mesgul(true);
        Sgb.gonder(API + '/kaydet', k)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); arama(); return; }
                temizle();
                mesaj(s.bilgi, 'success');
                arama();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnSil').on('click', function () {
        var k = formOku();
        if (k.yil <= 0 || k.kod === '') { mesaj('Lütfen silmek istediğiniz bilgiyi seçin.', 'warning'); return; }
        if (!window.confirm(k.yil + ' yılı ' + k.kod + ' kodlu gündelik tanımı silinecek. Onaylıyor musunuz?')) return;
        mesgul(true);
        Sgb.gonder(API + '/sil', { yil: k.yil, kod: k.kod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); arama(); return; }
                temizle();
                mesaj(s.bilgi, 'success');
                arama();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // Eski btnKopyala_Click: seçili türün önceki yıldaki tanımları seçili yıla kopyalanır.
    $('#btnKopyala').on('click', function () {
        var y = seciliYil(), $t = $tur.find('option:selected').text();
        if (y <= 0) { mesaj('Yıl boş bırakılamaz.', 'warning'); return; }
        if (!window.confirm((y - 1) + ' yılının "' + $t + '" gündelik tanımları ' + y + ' yılına kopyalanacak; ' + y +
            ' yılının bu türdeki tanımları silinip yeniden yazılacak. Onaylıyor musunuz?')) return;
        mesgul(true);
        Sgb.gonder(API + '/kopyala', { yil: y, tur: parseInt($tur.val(), 10) || 0 })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                temizle();
                mesaj(s.bilgi, 'success');
                arama();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnTemizle').on('click', function () {
        temizle();
        $('#txtKod').trigger('focus');
    });

    // ---------------------------------------------------------------- Dosya Seç (eski dosyaSec + btnDosyaYukle_Click)

    $('#btnDosyaSec').on('click', function () {
        if (seciliYil() <= 0) { mesaj('Yıl boş bırakılamaz.', 'warning'); return; }
        $('#fuDosya').val('').trigger('click');
    });
    $('#fuDosya').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        var ad = dosya.name.toLowerCase();
        if (!/\.xlsx?$/.test(ad)) { mesaj('Sadece Excel dosyası yüklenebilir.', 'warning'); this.value = ''; return; }
        var y = seciliYil();
        if (!window.confirm('Seçilen dosyadaki yurtdışı gündelikleri ' + y + ' yılına yüklenecek:\n\n' + dosya.name + '\n\nDevam edilsin mi?')) { this.value = ''; return; }
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('yil', y);
        mesgul(true);
        mesaj('Dosya yükleniyor, lütfen bekleyiniz...', 'info');
        $.ajax({
            url: Sgb.KOK + API + '/dosyayukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (s) {
            if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
            mesaj(s.bilgi, 'success');
            // Yüklenen tanımlar yurtdışı türündedir; liste o türle yenilenir.
            $tur.val('3');
            panelGoster();
            arama();
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else hataGoster(xhr);
        }).always(function () { mesgul(false); $('#fuDosya').val(''); });
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load: para birimi, ülke ve yıl doldurulur)

    panelGoster();
    yay();
    Sgb.al(API + '/baslangic')
        .done(function (v) {
            secenekDoldur($('#cmbParaBirimi'), v.paraBirimleri, function (o) { return o.kisaltma; });
            secenekDoldur($('#cmbUlke'), v.ulkeler, function (o) { return o.ad; }, '<-Seçiniz->');
            Sgb.yilKutusu($yil, v.yillar, v.yil);
        })
        .fail(hataGoster);
});
