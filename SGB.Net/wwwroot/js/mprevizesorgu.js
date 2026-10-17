// Bütçe İşlemi belge arama (ButceHazirlikv2/MasrafPlaniRevizeSorgu; eski MasrafPlaniRevizeSorgu.aspx): kıstaslara uyan formlar
// AG Grid'de listelenir; belge no'ya tıklanınca belge çerçevenin Form sekmesinde açılır (parent.OgBelgeAc). Yöneticiye seçili
// belgeler için İptal Et / Onayla / Yeni Yap; Raporlar: Tertip durum, İşlem Tertip Toplam, İşlem Detay Liste.
// Uçlar: /api/mprevize/sorgu/* (ButceHazirlik/MPRevizeUclari.cs).
$(function () {
    'use strict';

    if (!$('.mp-revize-sorgu').length) return;

    var API = 'api/mprevize';

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }
    var kodlar = MPTavan.kodAlanlari(yilOku, hataGoster, API);

    function kistas() {
        return {
            yil: yilOku(), birim: $('#ddlBirim').val() || '', durum: $('#ddlDurum').val() || '', tur: $('#ddlIslemTur').val() || '',
            no1: $.trim($('#txtBelgeNo1').val()), no2: $.trim($('#txtBelgeNo2').val()), islemYapan: $.trim($('#txtIslemYapan').val()),
            tarih1: $('#txtBelgeTarih1').val(), tarih2: $('#txtBelgeTarih2').val(), durumTarih1: $('#txtDurumTarih1').val(), durumTarih2: $('#txtDurumTarih2').val(),
            fon: kodlar.kod('fon'), fin: kodlar.kod('fin'), eko: kodlar.kod('eko'), peb: kodlar.kod('peb'), ky: kodlar.kod('ky')
        };
    }
    function kistasTamamMi(k) {
        if (k.yil < 2012) { mesaj('Sorgulama için yıl alanını boş bırakamazsınız.', 'warning'); return false; }
        if (!k.birim) { mesaj('Sorgulama için Birim alanını boş bırakamazsınız.', 'warning'); return false; }
        return true;
    }

    // ---------------------------------------------------------------- tablo

    var listeYil = 0, listeBirim = '';

    // Eski BelgeGirisAc: belge çerçevenin Form sekmesinde açılır.
    function belgeAc(no) {
        if (window.parent && window.parent !== window && typeof window.parent.OgBelgeAc === 'function') {
            window.parent.OgBelgeAc(listeYil, no, listeBirim);
            return;
        }
        window.open('MasrafPlaniButceIslemFrame', '_blank');
    }

    function noHucre(p) {
        if (!p.data) return '';
        var a = document.createElement('a');
        a.href = '#';
        a.textContent = p.value;
        a.className = 'mp-belge-ac';
        a.title = 'Belgeyi Form sekmesinde aç';
        a.addEventListener('click', function (e) { e.preventDefault(); belgeAc(p.value); });
        return a;
    }

    var grid = Sgb.grid('#grdBelgeler', {
        columnDefs: [
            { field: 'revizeNo', headerName: 'No', width: 56, minWidth: 48, cellRenderer: noHucre },
            { field: 'odenekToplam', headerName: 'Miktar', width: 92, minWidth: 80, type: 'rightAligned', valueFormatter: function (p) { return MPTavan.tutar(p.value); } },
            { field: 'durumAd', headerName: 'Durum', width: 85 },
            { field: 'evrakTarih', headerName: 'Evrak Tar.', width: 95, valueFormatter: function (p) { return Sgb.tarih(p.value); } },
            { field: 'kaydedenKullaniciKodu', headerName: 'Kaydeden', width: 120 },
            { field: 'onayTarih', headerName: 'Onay Tar.', width: 95, valueFormatter: function (p) { return Sgb.tarih(p.value); } },
            { field: 'onaylayanKullaniciKodu', headerName: 'Onaylayan', width: 120 },
            { field: 'formTipAd', headerName: 'İşlem', width: 160, tooltipField: 'formTipAd' },
            { field: 'aciklama', headerName: 'Açıklama', width: 260, tooltipField: 'aciklama' },
            { field: 'BUMKONo', headerName: 'BÜMKO No', width: 100 }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        selectionColumnDef: { width: 44, minWidth: 44, maxWidth: 44, resizable: false },
        tooltipShowDelay: 400,
        overlayNoRowsTemplate: '<span>Kıstasları girip Listele\'ye basınız.</span>'
    });

    function listele() {
        var k = kistas();
        if (!kistasTamamMi(k)) return;
        var $d = $('#btnListe').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(API + '/sorgu/liste', k)
            .done(function (v) {
                grid.setGridOption('overlayNoRowsTemplate', '<span>Belge bulunamadı.</span>');
                if (v.hata) { grid.setGridOption('rowData', []); mesaj(v.hata, 'danger'); return; }
                listeYil = v.yil;
                listeBirim = k.birim;
                grid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }
    $('#btnListe').on('click', listele);
    $('#frmSorgu').on('submit', function (e) { e.preventDefault(); listele(); });

    // ---------------------------------------------------------------- durum değiştir (eski btnDurumDegistir_Click)

    $('.durum-degistir').on('click', function () {
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Belge seçiniz.', 'warning'); return; }
        var ad = $.trim($(this).text());
        if (!window.confirm(secili.length + ' belge için "' + ad + '" işlemi yapılacak. Onaylıyor musunuz?')) return;
        var $d = $('.durum-degistir').prop('disabled', true);
        Sgb.gonder(API + '/sorgu/durum', { Yil: listeYil || yilOku(), Durum: parseInt($(this).attr('data-durum'), 10), Nolar: secili.map(function (s) { return s.revizeNo; }) })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi || 'İşlem tamamlandı.', 'success');
                listele();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- raporlar (eski btnRapor_Click)

    var $mnu = $('#mnuRapor');
    function menu(ac) { $mnu.prop('hidden', !ac); $('#btnRapor').attr('aria-expanded', ac ? 'true' : 'false'); }
    $('#btnRapor').on('click', function (e) { e.stopPropagation(); menu($mnu.prop('hidden')); });
    $(document).on('click', function () { menu(false); });
    $mnu.on('click', '[data-rapor]', function () {
        menu(false);
        var k = kistas();
        if (!kistasTamamMi(k)) return;
        mesaj('Rapor hazırlanıyor...', 'info');
        Sgb.indir(API + '/sorgu/rapor', $.extend(k, { rapor: $(this).attr('data-rapor') }));
    });

    // ---------------------------------------------------------------- açılış

    function tanimYukle() {
        Sgb.al(API + '/sorgu/tanim', { yil: yilOku() })
            .done(function (t) {
                if (t.hata) { mesaj(t.hata, 'danger'); return; }
                $('#ddlYil').val(t.yil);
                var doldur = function ($s, liste) {
                    var eski = $s.val();
                    $s.empty();
                    $.each(liste || [], function (i, o) { $('<option></option>').val(o.kod).text(o.ad).appendTo($s); });
                    if (eski && $s.find('option').filter(function () { return this.value === eski; }).length) $s.val(eski);
                };
                doldur($('#ddlDurum'), t.durumlar);
                doldur($('#ddlIslemTur'), t.islemTurleri);
                doldur($('#ddlBirim'), t.birimler);
                $('.yonetici').prop('hidden', !t.yonetici);
            })
            .fail(hataGoster);
    }
    $('#ddlYil').on('change', function () { kodlar.adlariYenile(); tanimYukle(); });
    if (!yilOku()) $('#ddlYil').val(new Date().getFullYear());
    tanimYukle();

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
    function tabloYay() { yuksekligeYay(document.getElementById('grdBelgeler'), 200); }
    $(window).on('resize', tabloYay);
    if (window.ResizeObserver) new ResizeObserver(tabloYay).observe(document.querySelector('.mp-revize-sorgu .mp-ust'));
    tabloYay();
});
