// Yıl, Unvan ve Ülke Kriterine Göre Gündelik Tanımlama (HarcamaSurecleri/TanimGundelik; eski aynı adlı .aspx).
// Üstte form, altta gündelik listesi (AG Grid, salt okunur). Eski sayfadaki gibi açılışta seçili yılın gündelikleri listelenir.
// Listede bir satır seçilince bilgileri forma gelir (eski SatirSecildi); Kaydet seçili kodla gönderilir, Sil seçili kaydı siler.
$(function () {
    'use strict';

    if (!$('#grdGundelik').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var API = 'api/unvangundelik';
    var tutarBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var seciliKod = 0; // eski hdnKod

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('.ug-arac .btn').not('#btnYardim').prop('disabled', evet); }
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
        var $m = $('#ugYardim'), acik = $m.prop('hidden');
        $m.prop('hidden', !acik);
        $(this).attr('aria-expanded', acik ? 'true' : 'false').toggleClass('active', acik);
        yay();
    });

    // ---------------------------------------------------------------- liste

    var grid = Sgb.grid('#grdGundelik', {
        columnDefs: [
            { field: 'yil', headerName: 'Yıl', width: 80, cellClass: 'text-center' },
            { field: 'unvanAd', headerName: 'Unvan', flex: 1, minWidth: 220, tooltipField: 'unvanAd' },
            { field: 'ulkeAd', headerName: 'Ülke', width: 250, tooltipField: 'ulkeAd' },
            { field: 'turAd', headerName: 'Tür', width: 160 },
            {
                field: 'gundelikTutar', headerName: 'Tutar', width: 140, cellClass: 'text-right', headerClass: 'ug-sag-baslik',
                valueFormatter: function (p) { return p.value == null ? '' : tutarBicim.format(p.value); }
            },
            { field: 'paraBirimAd', headerName: 'Para Birimi', width: 100, cellClass: 'text-center' }
        ],
        rowSelection: 'single',
        onRowClicked: function (e) { e.node.setSelected(true); doldur(e.data); },
        overlayNoRowsTemplate: '<span class="text-muted">Kayıt yok. Yılı seçip Ara\'ya basınız.</span>'
    });

    // ---------------------------------------------------------------- form

    var $yil = $('#txtYil');
    function seciliYil() { return tamSayi('#txtYil'); }

    // Eski FormTemizle: seçili kod, unvan, ülke, tutar ve tür boşalır; yıl ve para birimi kalır.
    function temizle() {
        seciliKod = 0;
        $('#cmbUnvan, #cmbUlke, #cmbTur').val('');
        $('#txtTutar').val('');
        grid.deselectAll();
    }

    // Eski SatirSecildi
    function doldur(o) {
        if (!o) return;
        seciliKod = o.kod || 0;
        if (o.yil) $yil.val(String(o.yil));
        $('#cmbUnvan').val(String(o.unvanKod == null ? '' : o.unvanKod));
        $('#cmbUlke').val(o.ulkeKod || '');
        $('#cmbTur').val(o.turKod ? String(o.turKod) : '');
        $('#txtTutar').val(o.gundelikTutar == null ? '' : tutarBicim.format(o.gundelikTutar));
        $('#cmbParaBirimi').val(o.paraBirimKod ? String(o.paraBirimKod) : '');
        $('#cmbUnvan, #cmbUlke, #cmbTur, #cmbParaBirimi').each(function () { if ($(this).val() === null) $(this).val(''); });
    }

    // Eski KriterTopla: seçilmeyen alan 0 / boş gider (unvan seçilmezse 0 = Diğer).
    function formOku() {
        return {
            kod: seciliKod,
            yil: seciliYil(),
            unvanKod: tamSayi('#cmbUnvan'),
            ulkeKod: $('#cmbUlke').val() || '',
            tur: tamSayi('#cmbTur'),
            gundelikTutar: sayiOku($('#txtTutar').val()),
            paraBirimKod: tamSayi('#cmbParaBirimi')
        };
    }

    function secenekDoldur($c, liste, metin) {
        $c.empty();
        $('<option></option>').val('').text('').appendTo($c);
        $.each(liste || [], function (i, o) { $('<option></option>').val(String(o.kod)).text(metin(o)).appendTo($c); });
    }

    // ---------------------------------------------------------------- listeleme

    function listeyiYaz(v) {
        if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
        grid.setGridOption('rowData', v.satirlar || []);
        if (v.bilgi) mesaj(v.bilgi, 'info');
    }

    function oku(uc, veri) {
        grid.setGridOption('loading', true);
        return Sgb.gonder(API + '/' + uc, veri)
            .done(listeyiYaz)
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    // Eski Listele(): yalnız yıla göre (yıl boşsa bu yıl).
    function listele() { return oku('listele', { yil: seciliYil() || new Date().getFullYear() }); }

    $('#btnAra').on('click', function () {
        mesgul(true);
        oku('bul', formOku()).always(function () { mesgul(false); });
    });

    $('#btnKaydet').on('click', function () {
        var k = formOku();
        mesgul(true);
        Sgb.gonder(API + '/kaydet', k)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                listele().done(function () {
                    // Eski sayfada form temizlenmez; kaydedilen satır seçili kalır.
                    seciliKod = s.kod || seciliKod;
                    grid.forEachNode(function (n) { if (n.data && n.data.kod === seciliKod) n.setSelected(true); });
                });
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnSil').on('click', function () {
        if (seciliKod <= 0) { mesaj('Lütfen silmek istediğiniz kaydı listeden seçiniz.', 'warning'); return; }
        if (!window.confirm('Gündelik bilgisini silmek istediğinize emin misiniz?')) return;
        mesgul(true);
        Sgb.gonder(API + '/sil', { kod: seciliKod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                // Eski: FormTemizle + Ara (KriterTopla temizlenmiş formdan: yıl ve para birimi süzmez, liste yıla göre gelir)
                temizle();
                mesaj(s.bilgi, 'success');
                listele();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnTemizle').on('click', function () {
        temizle();
        $('#cmbUnvan').trigger('focus');
    });

    $('#btnKopyala').on('click', function () {
        var y = seciliYil();
        if (y <= 0) { mesaj('Yıl boş bırakılamaz.', 'warning'); return; }
        if (!window.confirm(y + ' yılının gündelik bilgileri ' + (y + 1) + ' yılına kopyalanacaktır. Onaylıyor musunuz?')) return;
        mesgul(true);
        Sgb.gonder(API + '/kopyala', { yil: y })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                var max = parseInt($yil.attr('max'), 10) || 0;
                if (s.yil > max) $yil.attr('max', s.yil);
                $yil.val(String(s.yil));
                temizle();
                mesaj(s.bilgi, 'success');
                listele();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load: unvan, tür, ülke, para birimi doldurulur, liste okunur)

    yay();
    Sgb.al(API + '/baslangic')
        .done(function (v) {
            secenekDoldur($('#cmbUnvan'), v.unvanlar, function (o) { return o.ad; });
            secenekDoldur($('#cmbUlke'), v.ulkeler, function (o) { return o.ad; });
            secenekDoldur($('#cmbTur'), v.turler, function (o) { return o.ad; });
            secenekDoldur($('#cmbParaBirimi'), v.paraBirimleri, function (o) { return o.kisaltma; });
            Sgb.yilKutusu($yil, v.yillar, v.yil);
            listele();
        })
        .fail(hataGoster);
});
