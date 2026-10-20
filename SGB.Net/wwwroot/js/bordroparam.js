// Personel Bordro Parametreleri (HarcamaSurecleri/PersonelBordroParamForm; eski PersonelBordroParamForm.aspx).
// Sayfa açılınca parametreler okunur (eski Page_Load). Tablo düzenlenebilir (AG Grid): Etiket, Sıra ve Tip her satırda,
// Değişken Ad yalnızca Ekle ile açılan yeni satırda yazılır. Kaydet bütün satırları gönderir (yeni satırlar eklenir,
// kayıtlılar güncellenir). Sil yalnızca kaydedilmemiş satırları kaldırır; iş kuralında kayıtlı değişken silme yoktur.
$(function () {
    'use strict';

    if (!$('#grdBordroParam').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TIPLER = ['Para', 'Yazı'];   // eski ComboBox seçenekleri
    var kirli = false;               // saklanmamış değişiklik var mı

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function yeniMi(p) { return !!(p.data && p.data.yeni === 1); }

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
    function yay() { yuksekligeYay(document.getElementById('grdBordroParam'), 260); }
    $(window).on('resize', yay);

    $('#btnYardim').on('click', function () {
        var $m = $('#bpYardim'), acik = $m.prop('hidden');
        $m.prop('hidden', !acik);
        $(this).attr('aria-expanded', acik ? 'true' : 'false');
        yay();
    });

    // ---------------------------------------------------------------- tablo

    function yaziSutun(alan, baslik, ek) {
        return $.extend({
            field: alan, headerName: baslik, editable: true, sortable: false,
            cellClass: 'bp-girilir', tooltipField: alan,
            valueParser: function (p) { return p.newValue == null ? '' : String(p.newValue); }
        }, ek || {});
    }

    var grid = Sgb.grid('#grdBordroParam', {
        columnDefs: [
            yaziSutun('degiskenAd', 'Değişken Ad', {
                width: 200, editable: yeniMi,
                cellClass: function (p) { return yeniMi(p) ? 'bp-girilir' : 'bp-salt'; }
            }),
            yaziSutun('aciklama', 'Değişken Etiket', { flex: 1, minWidth: 300 }),
            yaziSutun('sira', 'Sıra', { width: 90 }),
            yaziSutun('veriTipiStr', 'Tip', {
                width: 110, tooltipField: null,
                cellEditor: 'agSelectCellEditor', cellEditorParams: { values: TIPLER }
            })
        ],
        rowSelection: 'multiple',
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        rowClassRules: { 'bp-yeni-satir': yeniMi },
        onCellValueChanged: function () { kirli = true; },
        overlayNoRowsTemplate: '<span class="text-muted">Bordro parametresi yok. Ekle ile yeni değişken açabilirsiniz.</span>'
    });

    // ---------------------------------------------------------------- oku / kaydet

    function oku() {
        var $d = $('#btnOku').prop('disabled', true);
        grid.setGridOption('loading', true);
        return Sgb.al('api/bordroparam/oku')
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                kirli = false;
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#btnOku').on('click', function () {
        grid.stopEditing();
        if (kirli && !window.confirm('Kaydedilmemiş değişiklikler kaybolacak. Yeniden okunsun mu?')) return;
        oku();
    });

    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        var satirlar = [];
        grid.forEachNode(function (n) {
            var d = n.data;
            satirlar.push({
                degiskenAd: $.trim(d.degiskenAd || ''), aciklama: d.aciklama || '', sira: d.sira || '', yeni: d.yeni === 1 ? 1 : 0,
                veriTipiStr: d.veriTipiStr || '', yontem: d.yontem || 0, liste: d.liste || ''
            });
        });
        if (!satirlar.length) { mesaj('Kaydedilecek satır yok.', 'warning'); return; }
        if (!window.confirm('Kaydetmek İstiyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/bordroparam/kaydet', satirlar)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
                oku();   // eski Kaydet de başarıdan sonra yeniden okuyordu
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- ekle / sil

    $('#btnEkle').on('click', function () {
        grid.stopEditing();
        var r = grid.applyTransaction({ add: [{ degiskenAd: '', aciklama: '', sira: '', yeni: 1, veriTipiStr: 'Para', yontem: 0, liste: '' }] });
        kirli = true;
        if (r.add.length) {
            grid.ensureIndexVisible(r.add[0].rowIndex);
            grid.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: 'degiskenAd' });
        }
    });

    $('#btnSil').on('click', function () {
        grid.stopEditing();
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Silinecek satırları seçiniz.', 'warning'); return; }
        var yeniler = secili.filter(function (d) { return d.yeni === 1; });
        if (yeniler.length < secili.length)
            mesaj('Kayıtlı değişkenler silinemez; yalnızca kaydedilmemiş yeni satırlar kaldırıldı.', 'warning');
        if (yeniler.length) grid.applyTransaction({ remove: yeniler });
    });

    yay();
    oku();
});
