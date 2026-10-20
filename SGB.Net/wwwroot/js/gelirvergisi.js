// Gelir Vergisi Bilgileri Tanımlama (HarcamaSurecleri/TanimPSLGelirVergisi; eski TanimPSLGelirVergisi.aspx).
// Sayfa açılınca bu yılın dilimleri okunur (eski Page_Load). Yıl değişince ya da Listele ile yeniden okunur.
// Tablo: Yüzde ve Miktar girilir (beyaz); okunan satırların altında her zaman 5 boş satır bulunur (eski ekleSatirSayisi).
// Kaydet yüzdesi 0'dan büyük satırları ekrandaki sırayla gönderir; iş kuralı yılın dilimlerini silip yeniden yazar.
$(function () {
    'use strict';

    if (!$('#grdGelirVergisi').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var BOS_SATIR = 5;
    var miktarBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var kirli = false;       // saklanmamış değişiklik var mı
    var okunanYil = null;    // tablodaki bilgilerin yılı

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }

    // Türkçe (1.234,56) ve noktalı (1234.56) yazımı kabul edilir. Boş -> null, hatalı -> NaN.
    function sayiOku(deger) {
        if (deger == null) return null;
        if (typeof deger === 'number') return deger;
        var s = $.trim(String(deger)).replace(/\s/g, '');
        if (s === '') return null;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = parseFloat(s);
        return isNaN(n) ? NaN : Math.round(n * 100) / 100;
    }

    // ---------------------------------------------------------------- yükseklik: tablo sayfanın kalan yüksekliğini doldurur

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
    function yay() { yuksekligeYay(document.getElementById('grdGelirVergisi'), 260); }
    $(window).on('resize', yay);

    $('#btnYardim').on('click', function () {
        var $m = $('#gvYardim'), acik = $m.prop('hidden');
        $m.prop('hidden', !acik);
        $(this).attr('aria-expanded', acik ? 'true' : 'false');
        yay();
    });

    // ---------------------------------------------------------------- tablo

    var grid = Sgb.grid('#grdGelirVergisi', {
        columnDefs: [
            {
                headerName: '#', width: 52, pinned: 'left', cellClass: 'gv-salt gv-sag', sortable: false, resizable: false,
                valueGetter: function (p) { return p.node.rowIndex + 1; }
            },
            {
                field: 'yuzde', headerName: 'Yüzde', width: 110, editable: true, sortable: false,
                cellClass: 'gv-girilir gv-sag', headerClass: 'gv-sag-baslik',
                valueParser: function (p) {
                    var n = sayiOku(p.newValue);
                    if (n === null) return null;
                    if (isNaN(n) || n < 0 || n !== Math.floor(n)) { mesaj('Yüzde 0 ya da daha büyük bir tam sayı olmalıdır.', 'warning'); return p.oldValue; }
                    return n;
                }
            },
            {
                field: 'miktar', headerName: 'Miktar', width: 170, editable: true, sortable: false,
                cellClass: 'gv-girilir gv-sag', headerClass: 'gv-sag-baslik',
                valueFormatter: function (p) { return p.value == null ? '' : miktarBicim.format(p.value); },
                valueParser: function (p) {
                    var n = sayiOku(p.newValue);
                    if (n === null) return null;
                    if (isNaN(n) || n < 0) { mesaj('Geçerli bir miktar giriniz.', 'warning'); return p.oldValue; }
                    return n;
                }
            }
        ],
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVerticallyAfterEdit: true,
        enterNavigatesVertically: true,
        onCellValueChanged: function (p) {
            if (p.oldValue !== p.newValue) kirli = true;
            bosSatirTamamla();
        },
        overlayNoRowsTemplate: '<span class="text-muted">Yıl seçip Listele\'ye basınız.</span>'
    });

    function satirBos(d) { return !d || (d.yuzde == null || d.yuzde === 0) && (d.miktar == null || d.miktar === 0); }

    // Sondaki boş satır sayısı BOS_SATIR'dan azsa eksik kadar boş satır eklenir.
    function bosSatirTamamla() {
        var sayi = grid.getDisplayedRowCount(), bos = 0;
        for (var i = sayi - 1; i >= 0; i--) {
            if (!satirBos(grid.getDisplayedRowAtIndex(i).data)) break;
            bos++;
        }
        if (bos >= BOS_SATIR) return;
        var ek = [];
        for (var j = bos; j < BOS_SATIR; j++) ek.push({ yuzde: null, miktar: null });
        grid.applyTransaction({ add: ek });
    }

    // ---------------------------------------------------------------- listele / kaydet

    var $yil = $('#txtYil');

    function listele(yil) {
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        return Sgb.al('api/gelirvergisi/listele', yil ? { yil: yil } : null)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                Sgb.yilKutusu($yil, v.yillar, v.yil);
                okunanYil = v.yil;
                var satirlar = (v.satirlar || []).map(function (s) { return { yuzde: s.yuzde, miktar: s.miktar }; });
                for (var i = 0; i < BOS_SATIR; i++) satirlar.push({ yuzde: null, miktar: null });
                grid.setGridOption('rowData', satirlar);
                kirli = false;
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    function seciliYil() {
        var y = parseInt($yil.val(), 10);
        return y > 0 ? y : 0;
    }

    function yildanOku() {
        grid.stopEditing();
        var y = seciliYil();
        if (!y) { mesaj('Yıl giriniz.', 'warning'); return; }
        if (kirli && !window.confirm('Kaydedilmemiş değişiklikler kaybolacak. Yeniden okunsun mu?')) {
            if (okunanYil) $yil.val(String(okunanYil));
            return;
        }
        listele(y);
    }

    $('#btnListele').on('click', yildanOku);
    // Yazarken her tuşta okumamak için kısa bekleme yapılır.
    var bekle = null;
    $yil.on('input change', function () {
        clearTimeout(bekle);
        bekle = setTimeout(function () { if (seciliYil() >= 1000 && seciliYil() !== okunanYil) yildanOku(); }, 400);
    }).on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); clearTimeout(bekle); yildanOku(); } });

    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        var y = okunanYil;
        if (!y) { mesaj('Önce yılı listeleyiniz.', 'warning'); return; }
        if (seciliYil() !== y) { mesaj('Yıl değişti; önce Listele\'ye basınız.', 'warning'); return; }
        var satirlar = [];
        grid.forEachNode(function (n) {
            var d = n.data;
            if (d && Number(d.yuzde) > 0) satirlar.push({ yuzde: Number(d.yuzde), miktar: Number(d.miktar) || 0 });
        });
        if (!satirlar.length) { mesaj('Kaydedilecek dilim yok; en az bir satıra yüzde giriniz.', 'warning'); return; }
        if (!window.confirm(y + ' yılının gelir vergisi dilimleri kaydedilsin mi?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/gelirvergisi/kaydet', { yil: y, satirlar: satirlar })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
                kirli = false;
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $(window).on('beforeunload', function () { if (kirli) return 'Kaydedilmemiş değişiklikler var.'; });

    yay();
    listele(new Date().getFullYear());   // eski Page_Load: bu yıl listelenir
});
