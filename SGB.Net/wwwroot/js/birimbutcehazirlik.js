// Birim Bütçe Teklifi (ButceHazirlikv2/BirimButceHazirlikForm): seçilen saymanlık / birimin taşra tertipleri; eko 2 satırları (ana)
// toplamdır, altlarındaki eko 4 satırlarına (detay) yılın bütçe teklifi ve gerekçesi girilir. İlk satır genel toplam (üstte sabit);
// teklif değişince ana satır ve genel toplam hemen güncellenir (eski KolonDegisti). yeni=0 sekmesi önceki yıllarda yürütülen
// faaliyet/projeleri (Birim Projeleri listesiyle süzülür), yeni=1 sekmesi diğerlerini (Bakanlığın Tüm Projeleri ağacıyla) gösterir.
// İle Gönder / Gönder / İade tuşları birim türüne ve onay durumuna göre görünür. Birim Bütçe Hazırlığı çerçevesinde açıldığında
// öbür sekmenin kaydedilmemiş değişikliği ve "teklif yoktur" seçimi BbhDiger ile sorulur (birimbutcehazirlikcerceve.js).
$(function () {
    'use strict';

    var $kok = $('.birim-butce-hazirlik');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var ONAYLI = 5;
    var TOPLAM = 'T o p l a m';
    var yeni = $kok.attr('data-yeni') === '1';
    var acik = null;          // listelenmiş ölçüt { yil, muhasebe, birim }: Kaydet ve gönderme bunu kullanır
    var durum = 0;            // birimin onay durumu (5: onaylı)
    var degisen = {};         // tertip -> true: kaydedilmemiş detay satırları
    var tarihce = [];
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 0, maximumFractionDigits: 2 });
    var oran = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function kirliMi() { for (var k in degisen) if (degisen.hasOwnProperty(k)) return true; return false; }

    // Çerçevedeki öbür sekme (yoksa ya da açılmadıysa null).
    function diger() {
        try {
            if (window.parent && window.parent !== window && typeof window.parent.BbhDiger === 'function') return window.parent.BbhDiger(yeni);
        } catch (e) { /* farklı kökten açıldıysa öbür sekme yok sayılır */ }
        return null;
    }

    // "Diğer Proje/Faaliyetlerden Bütçe Teklifi Yoktur" işaretli mi? (eski YeniYokBul: Mevcut sekmesi Diğer sekmesine sorar)
    function yeniYokMu() {
        if (yeni) return $('#chkYeniYok').prop('checked');
        var d = diger();
        return d ? !!d.yeniYokMu() : false;
    }

    function olcutOku() {
        var m = muhBirim ? muhBirim.deger() : { muhasebe: 0, birim: '' };
        return { yil: yilOku(), muhasebe: m.muhasebe, birim: m.birim };
    }

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function paraYaz(p) { return p.value == null || p.value === '' ? '' : para.format(p.value); }

    // ---------------------------------------------------------------- teklif tablosu

    function anaMi(p) { return !p.data || !p.data.detayMi; }
    function girilir(p) { return !!p.data && !p.node.rowPinned && !!p.data.detayMi && durum !== ONAYLI; }

    function hucreSinif(p) {
        var s = [];
        if (p.colDef.type === 'sayi') s.push('sayi');
        if (!anaMi(p) && !p.node.rowPinned) s.push(p.colDef.editable && girilir(p) ? 'girilir' : 'kapali');
        return s;
    }

    // Artış oranı (eski renderArtis): tur 1 masraf planına göre (yalnızca ana satırlarda), tur 2 harcamaya göre.
    function artis(tur) {
        return function (p) {
            if (!p.data) return null;
            var harc = tur === 1 ? Number(p.data.planlanan) || 0 : Number(p.data.harcama) || 0;
            if (harc <= 0 || (tur === 1 && p.data.detayMi)) return null;
            return ((Number(p.data.talepEdilen) || 0) - harc) * 100 / harc;
        };
    }

    function sayiSutun(alan, baslik, ek) {
        return $.extend({
            field: alan, colId: alan, headerName: baslik, type: 'sayi', width: 130, sortable: false, headerClass: 'sayi',
            valueFormatter: paraYaz, cellClass: hucreSinif
        }, ek || {});
    }

    function sutunlar(yil) {
        return [
            { field: 'tertip', headerName: 'Tertip', width: 300, pinned: 'left', sortable: false, tooltipField: 'tertip', cellClass: hucreSinif },
            { field: 'pebad', headerName: 'Faaliyet/Proje Adı', width: 200, sortable: false, tooltipField: 'pebad', cellClass: hucreSinif },
            { field: 'ekoad', headerName: 'Ekonomik Ad', width: 220, sortable: false, tooltipField: 'ekoad', cellClass: hucreSinif },
            sayiSutun('harcama', (yil - 2) + ' Harcama', { hide: yeni }),
            sayiSutun('planlanan', (yil - 1) + ' Masraf Planı', { hide: yeni }),
            sayiSutun('talepEdilen', yil + ' Bütçe Teklifi', {
                editable: girilir, cellEditor: 'agTextCellEditor',
                valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; }
            }),
            sayiSutun('artisOranH', 'Hrc. Artış %', {
                field: undefined, valueGetter: artis(2), width: 110, hide: yeni, headerTooltip: 'Harcamaya Göre Artış Oranı',
                valueFormatter: function (p) { return p.value == null ? '' : oran.format(p.value); }
            }),
            sayiSutun('artisOranM', 'MP Artış %', {
                field: undefined, valueGetter: artis(1), width: 110, hide: yeni, headerTooltip: 'Masraf Planına Göre Artış Oranı',
                valueFormatter: function (p) { return p.value == null ? '' : oran.format(p.value); }
            }),
            {
                field: 'aciklama', headerName: 'Gerekçe', width: 300, flex: 1, minWidth: 200, sortable: false, tooltipField: 'aciklama',
                editable: girilir, cellClass: hucreSinif, cellEditor: 'agLargeTextCellEditor', cellEditorPopup: true,
                cellEditorParams: { maxLength: 4000, rows: 6, cols: 60 }
            }
        ];
    }

    var BOS_METIN = '<span>Saymanlık / birim seçip Listele\'ye basınız.</span>';

    var grid = Sgb.grid('#grdTeklif', {
        columnDefs: sutunlar(new Date().getFullYear() + 1),
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        getRowId: function (p) { return p.data.tertip; },
        getRowClass: function (p) { return p.data && !p.node.rowPinned && !p.data.detayMi ? 'ana' : ''; },
        overlayNoRowsTemplate: BOS_METIN,
        // Detay satırının teklifi değişince ana (eko 2) satırı ve genel toplam farkla güncellenir (eski KolonDegisti / ToplamSatirBul).
        onCellValueChanged: function (e) {
            if (e.node.rowPinned || !e.data.detayMi || e.newValue === e.oldValue) return;
            degisen[e.data.tertip] = true;
            kaydetAyarla();
            if (e.colDef.field !== 'talepEdilen') return;
            var fark = (Number(e.newValue) || 0) - (Number(e.oldValue) || 0);
            if (!fark) return;
            var t = e.data.tertip;
            var ana = grid.getRowNode(t.substring(0, 28) + t.substring(33));
            if (ana) ana.setDataValue('talepEdilen', (Number(ana.data.talepEdilen) || 0) + fark);
            var toplam = grid.getPinnedTopRow(0);
            if (toplam) toplam.setDataValue('talepEdilen', (Number(toplam.data.talepEdilen) || 0) + fark);
        }
    });

    function kaydetAyarla() {
        $('#btnKaydet').prop('disabled', !acik || durum === ONAYLI || !kirliMi());
    }

    function listeyiBosalt() {
        acik = null;
        degisen = {};
        grid.setGridOption('rowData', []);
        grid.setGridOption('pinnedTopRowData', []);
        grid.setGridOption('overlayNoRowsTemplate', BOS_METIN);
        kaydetAyarla();
    }

    function kaybolsunMu() {
        return !kirliMi() || window.confirm('Kaydedilmemiş değişiklikler var. Devam ederseniz kaybolacak. Devam edilsin mi?');
    }

    // ---------------------------------------------------------------- onay durumu ve tuşlar (eski BirimTuslariAyarla)

    function durumUygula(b) {
        if (!b) return;
        durum = b.durum || 0;
        tarihce = b.tarihce || [];
        $('#btnTarihce').text(b.durumAd || '-');
        $('#lblOnayli').prop('hidden', durum !== ONAYLI);
        var t = b.tuslar || {};
        $('#btnKaydet').prop('hidden', !t.kaydet);
        $('.rapor[data-grup="0"], .rapor[data-grup="1"]').prop('hidden', !t.yazdir);
        $('#btnIleGonder').prop('hidden', !t.ileGonder).prop('disabled', !!t.ileGonderPasif);
        $('#btnMerkezeGonder').prop('hidden', !t.merkezeGonder).prop('disabled', !!t.merkezePasif);
        $('#btnIleIade').prop('hidden', !t.ileIade);
        $('#btnIlceyeIade').prop('hidden', !t.ilceyeIade);
        $('#btnDurum').prop('hidden', !t.durum);
        if (yeni) $('#satirYeniYok').prop('hidden', !t.yeniYok);
        kaydetAyarla();
        grid.refreshCells({ force: true });
    }

    function durumOku() {
        var o = olcutOku();
        return Sgb.al('api/birimbutcehazirlik/durum', $.extend({ yeni: yeni ? 1 : 0, yeniYok: yeniYokMu() }, o))
            .done(function (b) {
                if (b.hata) { mesaj(b.hata, 'danger'); return; }
                durumUygula(b);
            })
            .fail(hataGoster);
    }

    // Birim Projeleri listesi (eski FillddlFaaliyetKod; yalnızca Mevcut sekmesinde).
    function faaliyetleriOku() {
        if (yeni) return;
        var o = olcutOku();
        var $f = $('#ddlFaaliyet').empty();
        $('<option></option>').val('').text('Tümü').appendTo($f);
        if (o.muhasebe <= 0 || !o.birim) return;
        Sgb.al('api/birimbutcehazirlik/faaliyetler', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $.each(v.satirlar, function (i, s) {
                    if (!s.kod4) return;
                    $('<option></option>').val(s.kod4).text(s.kodad4 || s.kod4).appendTo($f);
                });
            })
            .fail(hataGoster);
    }

    // Yıl / saymanlık / birim değişti (eski FaaliyetDoldur): listelenmiş tablo bırakılır, projeler ve durum yeniden okunur.
    function birimDegisti() {
        listeyiBosalt();
        grid.setGridOption('columnDefs', sutunlar(yilOku()));
        faaliyetleriOku();
        durumOku();
    }

    // ---------------------------------------------------------------- listele

    function listele() {
        var o = olcutOku();
        if (o.yil < 2000) { mesaj('Lütfen Yıl\'ı seçin.', 'warning'); return; }
        if (o.muhasebe <= 0 || !o.birim) { mesaj('Lütfen saymanlık ve birim kodunu giriniz!', 'warning'); return; }
        if (!kaybolsunMu()) return;
        var p = $.extend({ yeni: yeni ? 1 : 0, yeniYok: yeniYokMu(), eko: $('#txtEko').val() }, o);
        p.faaliyet = yeni ? $('#txtProg').val() : $('#ddlFaaliyet').val();
        var $d = $('#btnListele').prop('disabled', true);
        grid.stopEditing(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/birimbutcehazirlik/oku', p)
            .done(function (v) {
                if (v.hata) { listeyiBosalt(); mesaj(v.hata, 'warning'); return; }
                degisen = {};
                acik = o;
                grid.setGridOption('columnDefs', sutunlar(o.yil));
                grid.setGridOption('pinnedTopRowData', [v.toplam]);
                grid.setGridOption('rowData', v.satirlar);
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                durumUygula(v.durumBilgi);
                if (v.uyari) mesaj(v.uyari, 'warning');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#btnListele').on('click', listele);

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

    // ---------------------------------------------------------------- kaydet / gönder

    // Kaydet (eski btnSave_Click): yalnızca değişen detay satırları gönderilir.
    $('#btnKaydet').on('click', function () {
        if (!acik) { mesaj('Önce Listele\'ye basınız.', 'warning'); return; }
        grid.stopEditing();
        if (!kirliMi()) { mesaj('Bütçe teklifi girişi yapmadan kaydedemezsiniz!', 'warning'); return; }
        if (yeni && $('#chkYeniYok').prop('checked')) { mesaj('Diğer faaliyet/projelerden teklifim yoktur işaretli olmasına rağmen, kaydetmeye çalışıyorsunuz!', 'danger'); return; }
        sor('Onay', 'Kaydetmek istiyor musunuz?', 'Kaydet', function () {
            var satirlar = [];
            grid.forEachNode(function (n) {
                if (n.data.detayMi && degisen[n.data.tertip])
                    satirlar.push({ tertip: n.data.tertip, talepEdilen: Number(n.data.talepEdilen) || 0, aciklama: n.data.aciklama || '' });
            });
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder('api/birimbutcehazirlik/kaydet', $.extend({ yeni: yeni, yeniYok: yeniYokMu(), satirlar: satirlar }, acik))
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (!s.basarili) { kaydetAyarla(); return; }
                    degisen = {};
                    durumUygula(s.durumBilgi);
                    digerDurumYenile();
                })
                .fail(function (xhr) { kaydetAyarla(); hataGoster(xhr); });
        });
    });

    function digerDurumYenile() {
        var d = diger();
        if (d) d.durumYenile();
    }

    // İle Gönder (1), İlçeye İade Et (2), Gönder (3), İle İade Et (4) (eski btnOnay_Click).
    $('.onay').on('click', function () {
        var tur = parseInt($(this).attr('data-tur'), 10);
        var o = olcutOku();
        if (o.yil < 2000 || o.muhasebe <= 0 || !o.birim) { mesaj('Lütfen saymanlık ve birim kodunu giriniz!', 'warning'); return; }
        grid.stopEditing();
        if (kirliMi()) { mesaj('Teklifinizi gönderebilmeniz için yapılan değişiklikleri lütfen kaydediniz.', 'danger'); return; }
        var d = diger();
        if (d && d.kirliMi()) { mesaj('Teklifinizi gönderebilmeniz için diğer sayfada yapılan değişiklikleri lütfen kaydediniz.', 'danger'); return; }
        var $t = $(this);
        function gonder() {
            $t.prop('disabled', true);
            Sgb.gonder('api/birimbutcehazirlik/onay', $.extend({ yeni: yeni, yeniYok: yeniYokMu(), tur: tur }, o))
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) { durumUygula(s.durumBilgi); digerDurumYenile(); }
                    else durumOku();
                })
                .fail(function (xhr) { hataGoster(xhr); durumOku(); });
        }
        if (tur === 1) sor('Onay', 'Teklifiniz gönderildikten sonra değişiklik yapamazsınız. Devam etmek istiyor musunuz?', 'İle Gönder', gonder);
        else if (tur === 3) sor('Onay', 'Merkeze Gönder', 'Gönder', gonder);
        else gonder();
    });

    // "Teklif yoktur" değişince iki sekmenin gönder tuşları da yeniden ayarlanır (eski chkYeniFaalTeklifYok_Changed / GonderTusAyarla).
    $('#chkYeniYok').on('change', function () {
        durumOku();
        digerDurumYenile();
    });

    // ---------------------------------------------------------------- raporlar

    // Yazdır (0), Rapor Yazdır (1), Özet Yazdır (2) (eski btnYazdir_Click): eski Excel raporları.
    $('.rapor').on('click', function () {
        var o = olcutOku();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/birimbutcehazirlik/rapor', $.extend({ grup: $(this).attr('data-grup') }, o))
            .then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- tarihçe (eski wndTarihce)

    var dlgTarihce = document.getElementById('dlgTarihce');
    var tarihceGrid = Sgb.grid('#grdTarihce', {
        columnDefs: [
            { colId: 'no', headerName: '#', width: 60, sortable: false, valueGetter: function (p) { return p.node.rowIndex + 1; } },
            { field: 'aciklama', headerName: 'İşlem', width: 160, flex: 1 },
            { field: 'ISLEMIYAPAN', headerName: 'Kullanıcı', width: 160 },
            { field: 'ISLEMTARIHSTR', headerName: 'Tarih', width: 180 }
        ],
        overlayNoRowsTemplate: '<span>İşlem bulunamadı.</span>'
    });

    $('#btnTarihce').on('click', function () {
        tarihceGrid.setGridOption('rowData', tarihce);
        dlgTarihce.showModal();
    });

    // ---------------------------------------------------------------- birimlerin onay durumu (eski wndDurum; sistem yöneticisi)

    var dlgDurum = document.getElementById('dlgDurum');
    var durumYil = 0;

    function durumSinif(p) {
        var s = p.data && p.data.BIRIMKOD === '250' ? ['kapali'] : ['ana-hucre'];
        if (p.colDef.type === 'sayi') s.push('sayi');
        return s;
    }

    var durumGrid = Sgb.grid('#grdDurum', {
        columnDefs: [
            { field: 'MUHASEBEKOD', headerName: 'Say.', width: 100, cellClass: durumSinif },
            { field: 'BIRIMKOD', headerName: 'Birim', width: 80, cellClass: durumSinif },
            { field: 'MUHASEBEBIRIMAD', headerName: 'Ad', width: 260, flex: 1, tooltipField: 'MUHASEBEBIRIMAD', cellClass: durumSinif },
            { field: 'GIRILEN', headerName: 'Toplam', width: 140, type: 'sayi', headerClass: 'sayi', valueFormatter: paraYaz, cellClass: durumSinif },
            { field: 'DURUMAD', headerName: 'Durum', width: 100, cellClass: durumSinif }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        getRowId: function (p) { return p.data.MUHASEBEKOD + '-' + p.data.BIRIMKOD; },
        overlayNoRowsTemplate: '<span>Birim bulunamadı.</span>'
    });

    function durumlariYukle() {
        durumGrid.setGridOption('loading', true);
        Sgb.al('api/birimbutcehazirlik/onaydurum', { yil: durumYil })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                durumGrid.setGridOption('rowData', v.satirlar);
            })
            .fail(hataGoster)
            .always(function () { durumGrid.setGridOption('loading', false); });
    }

    $('#btnDurum').on('click', function () {
        durumYil = yilOku();
        if (durumYil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        $('#dlgDurumBaslik').text(durumYil + ' Yılı Birimlerin Onay Durumu');
        durumGrid.setGridOption('rowData', []);
        dlgDurum.showModal();
        durumlariYukle();
    });

    // Kopyala (eski CopyStore): liste sekmeyle ayrılmış metin olarak panoya kopyalanır (Excel'e yapıştırılabilir).
    $('#btnKopyala').on('click', function () {
        var satirlar = ['Say.\tBirim\tAd\tToplam\tDurum'];
        durumGrid.forEachNodeAfterFilterAndSort(function (n) {
            var d = n.data;
            satirlar.push([d.MUHASEBEKOD, d.BIRIMKOD, d.MUHASEBEBIRIMAD, String(d.GIRILEN).replace('.', ','), d.DURUMAD].join('\t'));
        });
        if (!navigator.clipboard) { mesaj('Tarayıcı panoya kopyalamaya izin vermiyor.', 'warning'); return; }
        navigator.clipboard.writeText(satirlar.join('\r\n'))
            .then(function () { mesaj((satirlar.length - 1) + ' satır panoya kopyalandı.', 'success'); },
                function () { mesaj('Panoya kopyalanamadı.', 'danger'); });
    });

    // Onayla (5: kurum onayı) / Onay Kaldır (1: il onayı kaldırılır) (eski btnSeciliOnayla_Click).
    function seciliOnayla(kod) {
        var secili = durumGrid.getSelectedRows();
        if (!secili.length) { mesaj('Seçim yapmanız gerekmektedir!', 'warning'); return; }
        Sgb.gonder('api/birimbutcehazirlik/secili-onayla', {
            yil: durumYil, onayKod: kod,
            birimler: $.map(secili, function (s) { return { MUHASEBEKOD: s.MUHASEBEKOD, BIRIMKOD: s.BIRIMKOD }; })
        })
            .done(function (s) {
                mesaj(s.bilgi || s.hata, s.basarili ? 'success' : (s.bilgi ? 'warning' : 'danger'));
                durumlariYukle();
                durumOku();
            })
            .fail(hataGoster);
    }

    $('#btnSeciliOnayla').on('click', function () { seciliOnayla(ONAYLI); });
    $('#btnSeciliOnayKaldir').on('click', function () { seciliOnayla(1); });

    // ---------------------------------------------------------------- kod ağaçları (eski açılır ağaçlar: Ekonomik, Bakanlığın Tüm Projeleri)

    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }

    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};  // tür -> { $ul, agac, yil }
    var aktifAlan = null;

    function agacAc($alan) {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var tur = $alan.attr('data-tur');
        var ad = $.trim($alan.find('label').text()).replace(/:$/, '');
        aktifAlan = $alan;
        $('#dlgKodBaslik').text(ad);
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[tur];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', ad + ' kod ağacı').appendTo('#agacKutu'), yil: yil };
            // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al('api/birimbutcehazirlik/kodlar', { yil: a.yil, tur: tur, ust: ust ? ust.kod : '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    dlgKod.close();
                    if (aktifAlan) aktifAlan.find('input').val(d.ad).trigger('change');
                },
                seciliMi: function (d) { return !!aktifAlan && kodAl(aktifAlan.find('input').val()) === d.kod; },
                ilkAcik: true,
                bosMetin: 'Kod bulunamadı.',
                hata: hataGoster
            });
        } else {
            a.$ul.prop('hidden', false);
            if (a.yil !== yil) { a.yil = yil; a.agac.yenile(); }
        }
    }

    $('.kod-alan .kod-sec').on('click', function () { agacAc($(this).closest('.kod-alan')); });
    $('.kod-alan .kod-temizle').on('click', function () { $(this).closest('.kod-alan').find('input').val('').trigger('focus'); });
    // Alana kod yazılabilir; Alt+Aşağı ağacı açar.
    $('.kod-alan input').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
    });

    // ---------------------------------------------------------------- ölçüt değişikliği

    $('#txtYil').on('change', function () {
        if (acik && !kaybolsunMu()) { $(this).val($(this).data('deger')); return; }
        $(this).data('deger', $(this).val());
        birimDegisti();
    });

    // Saymanlık - birim (ortak bileşen, muhbirim.js; kurumsal kod bu sayfada gösterilmez).
    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: yilOku,
        kurumsalGizle: true,
        oncesi: function () { return !acik || kaybolsunMu(); },
        degisince: birimDegisti
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirliMi()) { e.preventDefault(); e.returnValue = ''; }
    });

    // Çerçevedeki öbür sekme bunları çağırır (eski DigerSayfaKirli, YeniFaalTeklifYokBul, GonderTusAyarla).
    window.BbhSayfa = {
        kirliMi: function () { grid.stopEditing(); return kirliMi(); },
        yeniYokMu: function () { return yeni && $('#chkYeniYok').prop('checked'); },
        durumYenile: function () { durumOku(); }
    };

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/birimbutcehazirlik/tanim', { yeni: yeni ? 1 : 0 })
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            $('#txtYil').val(t.yil).data('deger', String(t.yil));
            $('#lblBilgi').text(t.bilgi || '');
            if (t.yeniYokMetni) $('#lblYeniYok').text(t.yeniYokMetni);
            muhBirim.ata({ muhasebe: t.muhasebe, birim: t.birim, kurumsal: t.kurumsal, birimAd: t.birimAd });
            birimDegisti();
        })
        .fail(hataGoster);
});
