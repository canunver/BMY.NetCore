// Bütçe Çalışmalarında Görevli Personel Listesi (ButceHazirlikv2/ButceCalismasiPersoneliForm): seçilen yıl ve faaliyet için bütçe
// çalışmalarında görevli personel. Listele ile okunur; tabloda hücreye tıklanıp yazılır, Satır Aç / Satır Sil ile satır eklenir ya da
// işaretliler silinir. Kopyala tablodaki satırları panoya (sekmeyle ayrılmış, eski CopyStore gibi), Yapıştır panodaki satırları seçili
// satırdan başlayarak yazar ve gerekirse satır açar (eski EXTVeriYapistir). Kaydet faaliyetin listesini tablodakilerle değiştirir.
// Yazdır Excel raporu indirir; Önc. Yıldan Kopyala yalnızca sistem yöneticisine görünür.
$(function () {
    'use strict';

    if (!$('#grdPersonel').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    // Girilen alanlar (eski store sırası); Kopyala / Yapıştır bu sırayı kullanır.
    var ALANLAR = ['ad', 'soyad', 'unvan', 'birim', 'isTel', 'cepTel', 'faks', 'ePosta'];
    var kirli = false;        // kaydedilmemiş değişiklik var mı
    var siraNo = 0;           // satır kimliği üretmek için

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function sonucGoster(s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }

    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }

    function secim() { return { yil: yilOku(), peb: $.trim($('#txtKod14').val()) }; }

    function bosSatir() {
        var s = { id: 'y' + (++siraNo) };
        $.each(ALANLAR, function (i, a) { s[a] = ''; });
        return s;
    }

    function kirliYap(evet) {
        kirli = evet;
        $('#kirliEtiket').prop('hidden', !evet);
    }

    function degisiklikVarsaSor(neden) {
        return !kirli || window.confirm('Kaydedilmemiş değişiklikler var. ' + neden + ' kaybolacak. Devam edilsin mi?');
    }

    // ---------------------------------------------------------------- tablo

    function metinSutun(alan, baslik, genislik) {
        return { field: alan, headerName: baslik, width: genislik, sortable: false, editable: true, tooltipField: alan,
            cellEditor: 'agTextCellEditor', cellClass: 'girilir',
            valueParser: function (p) { return p.newValue == null ? '' : String(p.newValue); } };
    }

    var grid = Sgb.grid('#grdPersonel', {
        columnDefs: [
            { colId: 'sira', headerName: '#', width: 60, sortable: false, resizable: false, cellClass: 'sayi', headerClass: 'sayi',
                valueGetter: function (p) { return p.node.rowIndex == null ? '' : p.node.rowIndex + 1; } },
            metinSutun('ad', 'Ad', 130),
            metinSutun('soyad', 'Soyad', 130),
            metinSutun('unvan', 'Unvan', 200),
            metinSutun('birim', 'Birimi (Daire Başkanlığı)', 220),
            metinSutun('isTel', 'İş Tel', 120),
            metinSutun('cepTel', 'Cep Tel', 120),
            metinSutun('faks', 'Faks', 120),
            metinSutun('ePosta', 'EPosta', 220)
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        getRowId: function (p) { return p.data.id; },
        overlayNoRowsTemplate: '<span>Yılı ve faaliyeti seçip Listele\'ye basınız; yeni liste için Satır Aç\'a basınız.</span>',
        onCellValueChanged: function (e) { if (e.newValue !== e.oldValue) kirliYap(true); }
    });

    function siraYenile() { grid.refreshCells({ columns: ['sira'], force: true }); }

    function satirlar() {
        var liste = [];
        grid.forEachNodeAfterFilterAndSort(function (n) { liste.push(n.data); });
        return liste;
    }

    // ---------------------------------------------------------------- Listele (eski btnOku_Click)

    function oku() {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!degisiklikVarsaSor('Yeniden listelenirse değişiklikler')) return;
        var $d = $('#btnOku').prop('disabled', true);
        grid.stopEditing(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/butcepersonel/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', $.map(v.satirlar, function (s) { return $.extend(s, { id: 'y' + (++siraNo) }); }));
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı. Yeni liste için Satır Aç\'a basınız.</span>');
                kirliYap(false);
                if (!v.satirlar.length) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmSecim').on('submit', function (e) { e.preventDefault(); oku(); });

    // ---------------------------------------------------------------- Satır Aç / Satır Sil (eski addRecord / deleteSelected)

    $('#btnSatirAc').on('click', function () {
        grid.stopEditing();
        var r = grid.applyTransaction({ add: [bosSatir()] });
        kirliYap(true);
        siraYenile();
        if (r && r.add && r.add[0]) {
            grid.ensureIndexVisible(r.add[0].rowIndex);
            grid.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: 'ad' });
        }
    });

    $('#btnSatirSil').on('click', function () {
        grid.stopEditing();
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Silinecek satırları işaretleyiniz.', 'warning'); return; }
        grid.applyTransaction({ remove: secili });
        kirliYap(true);
        siraYenile();
    });

    // ---------------------------------------------------------------- Kopyala / Yapıştır (eski CopyStore / EXTVeriYapistir)

    // Eski RemoveEnterTab / AddEnterTab: hücre içindeki sekme ve satır sonları kaçışlı yazılır, yapıştırırken geri çevrilir.
    function kacisla(d) { return String(d == null ? '' : d).replace(/\t/g, '\\t').replace(/\r/g, '\\r').replace(/\n/g, '\\n'); }
    function kacisCoz(d) { return String(d == null ? '' : d).replace(/\\t/g, '\t').replace(/\\r/g, '\r').replace(/\\n/g, '\n'); }

    $('#btnKopyala').on('click', function () {
        grid.stopEditing();
        var liste = satirlar();
        if (!liste.length) { mesaj('Kopyalanacak satır yok.', 'warning'); return; }
        var metin = $.map(liste, function (s) { return $.map(ALANLAR, function (a) { return kacisla(s[a]); }).join('\t') + '\r\n'; }).join('');
        var tamam = function () { mesaj(liste.length + ' satır panoya kopyalandı.', 'success'); };
        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(metin).then(tamam, function () { mesaj('Panoya kopyalanamadı.', 'danger'); });
        } else {
            var $t = $('<textarea readonly></textarea>').val(metin).css({ position: 'fixed', left: '-9999px' }).appendTo(document.body);
            $t[0].select();
            try { document.execCommand('copy'); tamam(); } catch (e) { mesaj('Panoya kopyalanamadı.', 'danger'); }
            $t.remove();
        }
    });

    var yapistirBas = -1;
    var dlgYapistir = document.getElementById('dlgYapistir');

    // Panodaki satırlar seçili satırdan başlayarak sırayla yazılır; tablo yetmezse sona satır açılır (eski satirAc = true).
    function yapistir(metin) {
        var gelen = String(metin || '').split('\n');
        while (gelen.length && /^\r?$/.test(gelen[gelen.length - 1])) gelen.pop();
        if (!gelen.length) { mesaj('Panoda yapıştırılacak veri yok.', 'warning'); return; }
        var eksik = yapistirBas + gelen.length - grid.getDisplayedRowCount();
        if (eksik > 0) {
            var yeni = [];
            for (var k = 0; k < eksik; k++) yeni.push(bosSatir());
            grid.applyTransaction({ add: yeni });
        }
        var liste = satirlar();
        for (var i = 0; i < gelen.length && yapistirBas + i < liste.length; i++) {
            var d = liste[yapistirBas + i];
            var degerler = gelen[i].replace(/\r$/, '').split('\t');
            for (var j = 0; j < Math.min(ALANLAR.length, degerler.length); j++) d[ALANLAR[j]] = kacisCoz(degerler[j]);
        }
        grid.refreshCells({ force: true });
        kirliYap(true);
        mesaj(gelen.length + ' satır yapıştırıldı; kaydetmeyi unutmayınız.', 'info');
    }

    $('#btnYapistir').on('click', function () {
        grid.stopEditing();
        var hucre = grid.getFocusedCell();
        var bas = hucre && !hucre.rowPinned && hucre.rowIndex != null ? hucre.rowIndex : -1;
        if (bas < 0) {
            var secili = grid.getSelectedNodes();
            if (secili.length) bas = secili[0].rowIndex;
        }
        // Tablo boşsa ilk satırdan başlanır (satırlar açılır).
        if ((bas == null || bas < 0) && !grid.getDisplayedRowCount()) bas = 0;
        if (bas == null || bas < 0) { mesaj('Lütfen satır seçiniz!', 'warning'); return; }
        yapistirBas = bas;
        // Pano okunamazsa (tarayıcı izin vermedi) veri pencereye yapıştırılır.
        var pencere = function () { $('#txtYapistir').val(''); dlgYapistir.showModal(); $('#txtYapistir').trigger('focus'); };
        if (navigator.clipboard && navigator.clipboard.readText && window.isSecureContext) navigator.clipboard.readText().then(yapistir, pencere);
        else pencere();
    });

    $('#btnYapistirTamam').on('click', function () {
        dlgYapistir.close();
        yapistir($('#txtYapistir').val());
    });

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;

    function sor(baslik, metin, evet, is) {
        $('#dlgSoruBaslik').text(baslik);
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

    // ---------------------------------------------------------------- Kaydet (eski btnKaydet_Click)

    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!o.peb) { mesaj('Kaydetmek için en alt faaliyeti seçiniz.', 'warning'); return; }
        var liste = $.map(satirlar(), function (s) {
            var r = {};
            $.each(ALANLAR, function (i, a) { r[a] = s[a] == null ? '' : String(s[a]); });
            return r;
        });
        sor('Onay', o.yil + ' yılı ' + o.peb + ' faaliyetinin personel listesi tablodaki ' + liste.length
            + ' satırla değiştirilecek. Kaydetmek istiyor musunuz?', 'Kaydet', function () {
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder('api/butcepersonel/kaydet', { yil: o.yil, peb: o.peb, satirlar: liste })
                .done(function (s) {
                    sonucGoster(s);
                    if (s.basarili) kirliYap(false);
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // ---------------------------------------------------------------- Yazdır (eski btnYazdir_Click)

    $('#btnYazdir').on('click', function () {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/butcepersonel/rapor', o).then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Önc. Yıldan Kopyala (eski btnKopyala_Click; sistem yöneticisi)

    $('#btnKopyalaYil').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        sor('Uyarı', 'Eğer bulunulan yıldan sonraki bir yıl seçilmişse, o yılın verileri silinir ve önceki yıldan veriler kopyalanır. '
            + 'Devam ederseniz ve gelecek yıl seçmişseniz girilen verileriniz silinecek ve önceki yıldan kopyalanacaktır. '
            + yil + ' yılı için devam etmek istiyor musunuz?', 'Kopyala', function () {
            var $d = $('#btnKopyalaYil').prop('disabled', true);
            Sgb.gonder('api/butcepersonel/kopyala', { yil: yil })
                .done(sonucGoster)
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- faaliyet kod ağacı (eski açılır ağaç)

    var dlgKod = document.getElementById('dlgKod');
    var agac = null;
    var agacYil = 0;
    var $faaliyet = $('#txtKod14');

    function agacAc() {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        dlgKod.showModal();
        if (!agac) {
            agacYil = yil;
            // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
            agac = Sgb.agac($('<ul></ul>').attr('aria-label', 'Faaliyet kod ağacı').appendTo('#agacKutu')[0], {
                yukle: function (ust) {
                    return Sgb.al('api/butcepersonel/kodlar', { yil: agacYil, tur: '14', ust: ust ? ust.kod : '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    dlgKod.close();
                    $faaliyet.val(d.ad).trigger('change');
                },
                seciliMi: function (d) { return kodAl($faaliyet.val()) === d.kod; },
                ilkAcik: true,
                bosMetin: 'Kod bulunamadı.',
                hata: hataGoster
            });
        } else if (agacYil !== yil) {
            agacYil = yil;
            agac.yenile();
        }
    }

    $('.kod-alan .kod-sec').on('click', agacAc);
    $('.kod-alan .kod-temizle').on('click', function () { $faaliyet.val('').trigger('focus'); });
    // Alana kod yazılabilir; Alt+Aşağı ağacı açar.
    $faaliyet.on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc(); }
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/butcepersonel/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            $('#txtYil').val(t.yil);
            $('#btnKopyalaYil').prop('hidden', !t.yonetici);
        })
        .fail(hataGoster);
});
