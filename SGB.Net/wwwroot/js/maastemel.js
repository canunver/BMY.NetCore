// Maaş Temel Bilgileri Tanımlama (HarcamaSurecleri/TanimPSLMaasTemelBilgi; eski TanimPSLMaasTemelBilgi.aspx).
// Sayfa açılınca bu yılın bilgileri okunur (eski Page_Load). Yıl değişince ya da Listele ile yeniden okunur.
// Tablo: 23 değişken x 12 ay; # ve Değişken sütunları solda sabit ve salt okunur, ay hücreleri girilir (beyaz).
// Kaydet tablodaki bütün satırları gönderir; iş kuralı yılın 12 ayını silip yeniden yazar.
$(function () {
    'use strict';

    if (!$('#grdMaasTemel').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var AYLAR = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
    var sayiBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 6, maximumFractionDigits: 6 }); // eski 0.000,000000/i
    var kirli = false;       // saklanmamış değişiklik var mı
    var okunanYil = null;    // tablodaki bilgilerin yılı

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }

    // Türkçe (1.234,56) ve noktalı (1234.56) yazımı kabul edilir.
    function sayiOku(deger) {
        if (deger == null) return 0;
        if (typeof deger === 'number') return deger;
        var s = $.trim(String(deger)).replace(/\s/g, '');
        if (s === '') return 0;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = parseFloat(s);
        return isNaN(n) ? null : Math.round(n * 1e6) / 1e6;
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
    function yay() { yuksekligeYay(document.getElementById('grdMaasTemel'), 200); }
    $(window).on('resize', yay);

    $('#btnYardim').on('click', function () {
        var $m = $('#mtYardim'), acik = $m.prop('hidden');
        $m.prop('hidden', !acik);
        $(this).attr('aria-expanded', acik ? 'true' : 'false');
        yay();
    });

    // ---------------------------------------------------------------- tablo

    var sutunlar = [
        { field: 'kod', headerName: '#', width: 37, pinned: 'left', cellClass: 'mt-salt mt-sag', sortable: false, resizable: false },
        { field: 'ad', headerName: 'Değişken', width: 250, pinned: 'left', cellClass: 'mt-salt', tooltipField: 'ad', sortable: false }
    ];
    $.each(AYLAR, function (i, ay) {
        sutunlar.push({
            field: 'a' + (i + 1), headerName: ay, flex: 1, minWidth: 84, editable: true, sortable: false,
            cellClass: 'mt-girilir mt-sag', headerClass: 'mt-sag-baslik',
            valueFormatter: function (p) { return p.value == null ? '' : sayiBicim.format(p.value); },
            valueParser: function (p) {
                var n = sayiOku(p.newValue);
                if (n === null) { mesaj('Geçerli bir sayı giriniz.', 'warning'); return p.oldValue; }
                return n;
            }
        });
    });

    var grid = Sgb.grid('#grdMaasTemel', {
        columnDefs: sutunlar,
        rowHeight: 24,
        headerHeight: 26,
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVerticallyAfterEdit: true,
        enterNavigatesVertically: true,
        onCellValueChanged: function (p) { if (p.oldValue !== p.newValue) kirli = true; },
        overlayNoRowsTemplate: '<span class="text-muted">Yıl seçip Listele\'ye basınız.</span>'
    });

    // ---------------------------------------------------------------- listele / kaydet

    var $yil = $('#txtYil');

    function listele(yil) {
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        return Sgb.al('api/maastemel/listele', yil ? { yil: yil } : null)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                Sgb.yilKutusu($yil, v.yillar, v.yil);
                okunanYil = v.yil;
                grid.setGridOption('rowData', v.satirlar || []);
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
    // Eski SpinnerField: Spin ve Change olaylarında listeliyordu. Yazarken her tuşta okumamak için kısa bekleme yapılır.
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
            var d = n.data, s = { kod: d.kod, ad: d.ad };
            for (var i = 1; i <= 12; i++) s['a' + i] = Number(d['a' + i]) || 0;
            satirlar.push(s);
        });
        if (!satirlar.length) { mesaj('Kaydedilecek satır yok.', 'warning'); return; }
        if (!window.confirm(y + ' yılının maaş temel bilgileri kaydedilsin mi?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/maastemel/kaydet', { yil: y, satirlar: satirlar })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
                kirli = false;   // eski: stoMaasTemelBilgi.CommitChanges()
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $(window).on('beforeunload', function () { if (kirli) return 'Kaydedilmemiş değişiklikler var.'; });

    yay();
    listele(new Date().getFullYear());   // eski Page_Load: bu yıl listelenir
});
