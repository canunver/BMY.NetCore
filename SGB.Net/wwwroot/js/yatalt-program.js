// Alt Proje Detay Programı (ButceHazirlikv2/AltProjeDetayProgramiForm): AltProjeDetay sekmesinde açılır. Alt projenin bu yıl /
// aşamadaki detay programı (yer, birimi, ödenek, yılı faaliyeti; saymanlık ve birim kodu gizli) düzenlenir; ödenek toplamı
// başlıkta. Oku, Kaydet (onay), Yazdır (Word, eski 2004-13), Satır Aç / Sil, Yapıştır (Excel satırları) ve Önceki Aşamadan Al
// (bir önceki aşamanın programı getirilir; kaydedilene kadar saklanmaz). Ölçüt sayfa adresinden: yil, senaryoKod, projeKodu, altProjeKodu.
$(function () {
    'use strict';

    if (!$('#grdProgram').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var adres = new URLSearchParams(location.search);
    var olcut = {
        yil: parseInt(adres.get('yil'), 10) || 0,
        senaryoKod: adres.get('senaryoKod') || '',
        projeKodu: parseInt(adres.get('projeKodu'), 10) || 0,
        altProjeKodu: parseInt(adres.get('altProjeKodu'), 10) || 0
    };
    // Yapıştırmada panodaki sütunların sırayla yazılacağı (görünen, girilebilir) sütunlar.
    var GIRILEN = ['yer', 'birimi', 'odenek', 'aciklama'];
    var kirli = false;
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş -> 0. Geçersizse null.
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function bosSatir() {
        return { yer: '', birimi: '', saymanlikKod: 0, birimKod: '', odenek: 0, aciklama: '' };
    }

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;

    function sor(metin, evet, is) {
        $('#soruMetin').text(metin);
        $('#btnSoruEvet').text(evet);
        soruIsi = is;
        dlgSoru.showModal();
    }

    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        if (soruIsi) soruIsi();
        soruIsi = null;
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- tablo

    function metinSutun(alan, baslik, genislik, buyuk) {
        return {
            field: alan, headerName: baslik, width: genislik, sortable: false, editable: true, cellClass: 'girilir', tooltipField: alan,
            cellEditor: buyuk ? 'agLargeTextCellEditor' : 'agTextCellEditor',
            cellEditorPopup: !!buyuk, cellEditorParams: buyuk ? { maxLength: 4000, rows: 6, cols: 50 } : undefined
        };
    }

    var grid = Sgb.grid('#grdProgram', {
        columnDefs: [
            { colId: 'sira', headerName: '#', width: 60, sortable: false, resizable: false, valueGetter: function (p) { return p.node.rowIndex + 1; } },
            metinSutun('yer', 'Yer', 200),
            metinSutun('birimi', 'Birimi', 200),
            { field: 'saymanlikKod', headerName: 'Saymanlık Kod', hide: true },
            { field: 'birimKod', headerName: 'Birim Kod', hide: true },
            {
                field: 'odenek', headerName: 'Ödenek', width: 150, sortable: false, editable: true, cellClass: ['sayi', 'girilir'], headerClass: 'sayi',
                cellEditor: 'agTextCellEditor',
                valueFormatter: function (p) { return p.value == null || p.value === '' ? '' : para.format(p.value); },
                valueParser: function (p) {
                    var n = sayiCoz(p.newValue);
                    return n === null ? p.oldValue : Math.round(n * 100) / 100;
                }
            },
            metinSutun('aciklama', 'Yılı Faaliyeti', 350, true)
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        overlayNoRowsTemplate: '<span>Satır yok. Satır Aç ile ekleyebilirsiniz.</span>',
        // Ödenek değişince toplam yenilenir (eski HucreDegisti / OdenekToplaYaz).
        onCellValueChanged: function (e) {
            if (e.newValue === e.oldValue) return;
            kirli = true;
            if (e.colDef.field === 'odenek') toplamHesapla();
        }
    });

    function satirlar() {
        var liste = [];
        grid.forEachNode(function (n) { liste.push(n.data); });
        return liste;
    }

    function toplamHesapla() {
        var t = 0;
        $.each(satirlar(), function (i, s) { t += Number(s.odenek) || 0; });
        $('#odenekToplam').text(para.format(t));
    }

    // Oku (eski Oku(false)); oncekiAsama: eski Oku(true). Kayıt yoksa eskisi gibi boş bir satır açılır.
    function oku(oncekiAsama) {
        var $d = $('#btnOku, #btnOncekiAsama').prop('disabled', true);
        grid.stopEditing(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/yatalt/program/liste', $.extend({}, olcut, oncekiAsama ? { oncekiAsama: 1 } : {}))
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', v.satirlar.length ? v.satirlar : [bosSatir()]);
                toplamHesapla();
                kirli = !!oncekiAsama;
                if (oncekiAsama)
                    mesaj(v.yil + ' / ' + v.senaryoKod + ' aşamasının verileri getirildi (' + v.satirlar.length + ' satır). Kaydetmeyi unutmayınız.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#btnOku').on('click', function () {
        if (kirli && !window.confirm('Kaydedilmemiş değişiklikler var. Yeniden okursanız kaybolacak. Devam edilsin mi?')) return;
        oku(false);
    });

    // Önceki Aşamadan Al (eski btnOncekindenAl_Click; onay sorulur).
    $('#btnOncekiAsama').on('click', function () {
        sor('Ekrandaki veriler silinip, bir önceki aşamanın verileri getirilecektir. Veriler geldikten sonra kaydetme işlemi yapmayı unutmayınız. İşleme devam edilsin mi?',
            'Devam', function () { oku(true); });
    });

    // Kaydet (eski btnKaydet_Click; onay sorulur): satırlar sırayla yazılır.
    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        sor('Kaydetmek İstiyor musunuz?', 'Kaydet', function () {
            var gonderilecek = $.map(satirlar(), function (s) {
                return {
                    yer: s.yer || '', birimi: s.birimi || '', saymanlikKod: parseInt(s.saymanlikKod, 10) || 0,
                    birimKod: s.birimKod || '', odenek: Number(s.odenek) || 0, aciklama: s.aciklama || ''
                };
            });
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder('api/yatalt/program/kaydet', $.extend({}, olcut, { satirlar: gonderilecek }))
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) kirli = false;
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // Yazdır (eski btnYazdir_Click → Rapor 2004-13), Word.
    $('#btnYazdir').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/yatalt/program/rapor', olcut).then(function () { $d.prop('disabled', false); });
    });

    // Satır Aç / Satır Sil (eski addRecord / deleteSelected); kayıt Kaydet ile yapılır.
    $('#btnSatirAc').on('click', function () {
        grid.stopEditing();
        var r = grid.applyTransaction({ add: [bosSatir()] });
        kirli = true;
        if (r && r.add && r.add.length) {
            grid.ensureIndexVisible(r.add[0].rowIndex, 'bottom');
            grid.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: 'yer' });
        }
    });

    $('#btnSatirSil').on('click', function () {
        grid.stopEditing();
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Silinecek satırları işaretleyiniz.', 'info'); return; }
        grid.applyTransaction({ remove: secili });
        grid.refreshCells({ columns: ['sira'], force: true });
        kirli = true;
        toplamHesapla();
    });

    // Yapıştır (eski EXTVeriYapistir): panodaki satırlar seçili (odaktaki) satırdan başlayarak sırayla, sütunlar Yer'den
    // başlayarak görünen sütunlara (Yer, Birimi, Ödenek, Yılı Faaliyeti) yazılır; tablonun sonu aşılırsa yeni satır açılır.
    var yapistirBas = -1;
    var dlgYapistir = document.getElementById('dlgYapistir');

    function yapistir(metin) {
        var gelen = String(metin || '').split('\n');
        while (gelen.length && /^\r?$/.test(gelen[gelen.length - 1])) gelen.pop();
        if (!gelen.length) { mesaj('Panoda yapıştırılacak veri yok.', 'warning'); return; }
        var dugumler = [];
        grid.forEachNode(function (n) { dugumler.push(n); });
        var eklenecek = [];
        for (var i = 0; i < gelen.length; i++) {
            var d = yapistirBas + i < dugumler.length ? dugumler[yapistirBas + i].data : null;
            if (!d) { d = bosSatir(); eklenecek.push(d); }
            var degerler = gelen[i].replace(/\r$/, '').split('\t');
            for (var j = 0; j < Math.min(GIRILEN.length, degerler.length); j++) {
                var alan = GIRILEN[j], deger = $.trim(degerler[j]);
                if (alan === 'odenek') {
                    var n = sayiCoz(deger);
                    d[alan] = n === null ? 0 : Math.round(n * 100) / 100;
                } else d[alan] = deger;
            }
        }
        if (eklenecek.length) grid.applyTransaction({ add: eklenecek });
        kirli = true;
        grid.refreshCells({ force: true });
        toplamHesapla();
        mesaj(gelen.length + ' satır yapıştırıldı; kaydetmeyi unutmayınız.', 'info');
    }

    $('#btnYapistir').on('click', function () {
        grid.stopEditing();
        var hucre = grid.getFocusedCell();
        var secili = grid.getSelectedNodes();
        if (hucre && hucre.rowIndex != null && hucre.rowIndex >= 0 && !hucre.rowPinned) yapistirBas = hucre.rowIndex;
        else if (secili.length) yapistirBas = secili[0].rowIndex;
        else { mesaj('Lütfen satır seçiniz!', 'warning'); return; }
        // Pano okunamazsa (tarayıcı izin vermedi) veri pencereye yapıştırılır.
        var pencere = function () { $('#txtYapistir').val(''); dlgYapistir.showModal(); $('#txtYapistir').trigger('focus'); };
        if (navigator.clipboard && navigator.clipboard.readText && window.isSecureContext) navigator.clipboard.readText().then(yapistir, pencere);
        else pencere();
    });

    $('#btnYapistirTamam').on('click', function () {
        dlgYapistir.close();
        yapistir($('#txtYapistir').val());
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    oku(false);
});
