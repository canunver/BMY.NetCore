// Ana Harcama Grupları Tavan Girişi (ButceHazirlikv2/TavanEkoSektorBirim): seçilen ekonomik kod ve sektör için birimlerin üç yıllık
// tavanları. En üstteki TOPLAM satırı salt okunurdur ve alttaki birim satırlarının toplamıdır; bir tavan değişince yeniden hesaplanır.
// Onaylanmış tavanlar değiştirilemez. Raporlar sunucuda eski kodla üretilir.
$(function () {
    'use strict';

    if (!$('#grdTavan').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var SAYILAR = ['odenek_2', 'harcama_2', 'odenek_1', 'odenek_1_1', 'odenek_1_2', 'yil0', 'yil1', 'yil2'];
    var GIRILEN = ['yil0', 'yil1', 'yil2'];
    var sistemYoneticisi = false;
    var acik = null;     // listelenmiş ölçüt { yil, eko, sektor, yatirim }: Kaydet / Onay bunu kullanır
    var durum = -1;      // tavanların onay durumu: 5 onaylı, 1 onaysız, -1 kayıt yok
    var kirli = false;   // kaydedilmemiş değişiklik var mı
    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });
    var yuzde = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function secenekler($ddl, liste, secili) {
        $ddl.empty();
        $.each(liste, function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($ddl); });
        if (secili) $ddl.val(secili);
        if ($ddl.val() === null && $ddl.children().length) $ddl.prop('selectedIndex', 0);
    }

    function onayli() { return durum > 1; }

    // "1.234.000" ya da "1234000" -> 1234000. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    // ---------------------------------------------------------------- tablo

    function girilebilir(p) { return !onayli() && !p.node.rowPinned && GIRILEN.indexOf(p.colDef.field) >= 0; }

    function hucreSinifi(p) {
        var s = [];
        if (p.colDef.type === 'sayi') s.push('sayi');
        if (!p.node.rowPinned) s.push(girilebilir(p) ? 'girilir' : 'kapali');
        return s;
    }

    function para(p) { return p.value == null || p.value === '' ? '' : tamSayi.format(p.value); }

    // Artış yüzdesi: tavanın bir önceki yıla göre artışı (yil0: önceki yılın K.B.Ö'ne, yil1: yil0'a, yil2: yil1'e göre).
    function artis(alan, taban) {
        return function (p) {
            var b = p.data ? p.data[taban] : 0;
            return b ? (p.data[alan] - b) * 100 / b : null;
        };
    }

    function sutunlar(yil) {
        function para2(alan, baslik, ek) {
            return $.extend({ field: alan, headerName: baslik, type: 'sayi', width: 108, valueFormatter: para, cellClass: hucreSinifi, headerClass: 'sayi' }, ek || {});
        }
        function oran(alan, taban) {
            return { colId: alan + 'Oran', headerName: 'Artış %', type: 'sayi', width: 76, sortable: false, headerClass: 'sayi', cellClass: hucreSinifi,
                valueGetter: artis(alan, taban), valueFormatter: function (p) { return p.value == null ? '' : yuzde.format(p.value); } };
        }
        var giris = { editable: girilebilir, cellEditor: 'agTextCellEditor', valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; } };
        return [
            { field: 'kod3', headerName: 'Birim', width: 70, pinned: 'left', cellClass: hucreSinifi },
            { field: 'ad3', headerName: 'Birim Adı', width: 220, pinned: 'left', tooltipField: 'ad3', cellClass: hucreSinifi },
            para2('odenek_2', (yil - 2) + ' K.B.Ö'),
            para2('harcama_2', (yil - 2) + ' Harcama'),
            para2('odenek_1', (yil - 1) + ' K.B.Ö'),
            para2('odenek_1_1', yil + ' OVMP'),
            para2('odenek_1_2', (yil + 1) + ' OVMP'),
            para2('yil0', yil + ' Tavan', giris),
            oran('yil0', 'odenek_1'),
            para2('yil1', (yil + 1) + ' Tavan', giris),
            oran('yil1', 'yil0'),
            para2('yil2', (yil + 2) + ' Tavan', giris),
            oran('yil2', 'yil1')
        ];
    }

    var grid = Sgb.grid('#grdTavan', {
        columnDefs: sutunlar(new Date().getFullYear()),
        // Hücreye tıklayınca düzenlenir; Tab / Enter ile sonraki hücreye geçilir.
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        getRowId: function (p) { return p.data.kod3; },
        overlayNoRowsTemplate: '<span>Ekonomik kodu ve sektörü seçip Listele\'ye basınız.</span>',
        onCellValueChanged: function (e) {
            if (e.node.rowPinned || e.newValue === e.oldValue) return;
            kirli = true;
            toplamHesapla();
        }
    });

    // Üstteki TOPLAM satırı (eski "T O P L A M", kod 00): birim satırlarının toplamı.
    function toplamHesapla() {
        var t = { kod3: '00', ad3: 'T O P L A M' };
        $.each(SAYILAR, function (i, a) { t[a] = 0; });
        grid.forEachNode(function (n) { $.each(SAYILAR, function (i, a) { t[a] += Number(n.data[a]) || 0; }); });
        grid.setGridOption('pinnedTopRowData', grid.getDisplayedRowCount() ? [t] : []);
    }

    function durumGoster() {
        var $e = $('#durumEtiket');
        if (!acik || durum < 0) $e.prop('hidden', true);
        else $e.prop('hidden', false).toggleClass('label-success', onayli()).toggleClass('label-default', !onayli()).text(onayli() ? 'Onaylı' : 'Onaysız');
        var var_ = !!acik && grid.getDisplayedRowCount() > 0;
        $('#btnKaydet, #btnOvmp').prop('disabled', !var_ || onayli());
        $('#btnKopyala').prop('disabled', !var_);
        // Onay tuşları yalnızca sistem yöneticisine ve kayıtlı tavan varken görünür.
        $('#btnOnayla').prop('hidden', !sistemYoneticisi || !acik || durum < 0 || onayli());
        $('#btnOnayKaldir').prop('hidden', !sistemYoneticisi || !acik || !onayli());
        grid.refreshCells({ force: true });
    }

    function degisiklikVarsaSor() {
        return !kirli || window.confirm('Kaydedilmemiş değişiklikler var. Devam ederseniz kaybolacak. Devam edilsin mi?');
    }

    function listele() {
        var o = { yil: parseInt($('#txtYil').val(), 10) || 0, eko: $('#ddlEko').val() || '', sektor: $('#ddlSektor').val() || '', yatirim: $('#chkYatirim').prop('checked') };
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!o.eko || !o.sektor) { mesaj('Lütfen ekonomik kod ve sektör seçiniz!', 'warning'); return; }
        if (!degisiklikVarsaSor()) return;
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/tavaneko/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = o;
                durum = v.durum;
                kirli = false;
                grid.setGridOption('columnDefs', sutunlar(o.yil));
                grid.setGridOption('overlayNoRowsTemplate', '<span>Birim bulunamadı.</span>');
                grid.setGridOption('rowData', v.satirlar);
                toplamHesapla();
                durumGoster();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmSecim').on('submit', function (olay) { olay.preventDefault(); listele(); });

    // Seçim değişince eski liste kaydedilemez (Kaydet / Onay listelenen ekonomik kod ve sektöre yazar).
    function listeyiBosalt() {
        if (!acik) return;
        acik = null;
        durum = -1;
        kirli = false;
        grid.setGridOption('rowData', []);
        grid.setGridOption('pinnedTopRowData', []);
        durumGoster();
    }

    $('#ddlEko, #ddlSektor, #chkYatirim').on('change', function () {
        if (acik && kirli && !window.confirm('Kaydedilmemiş değişiklikler var. Seçimi değiştirirseniz kaybolacak. Devam edilsin mi?')) {
            if (this.type === 'checkbox') this.checked = acik.yatirim;
            else $(this).val(this.id === 'ddlEko' ? acik.eko : acik.sektor);
            return;
        }
        listeyiBosalt();
    });

    // Kodlar yıla bağlıdır: yıl değişince yeniden okunur, seçili olanlar varsa korunur.
    $('#txtYil').on('change', function () {
        var yil = parseInt(this.value, 10) || 0;
        if (acik && kirli && !window.confirm('Kaydedilmemiş değişiklikler var. Yılı değiştirirseniz kaybolacak. Devam edilsin mi?')) {
            this.value = acik.yil;
            return;
        }
        listeyiBosalt();
        if (yil < 2000) return;
        var eko = $('#ddlEko').val(), sektor = $('#ddlSektor').val();
        Sgb.al('api/tavaneko/kodlar', { yil: yil })
            .done(function (k) {
                if (k.hata) { mesaj(k.hata, 'danger'); return; }
                secenekler($('#ddlEko'), k.ekoKodlar, eko);
                secenekler($('#ddlSektor'), k.sektorler, sektor);
            })
            .fail(hataGoster);
    });

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

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

    // ---------------------------------------------------------------- işlemler

    function kaydet() {
        grid.stopEditing();
        var satirlar = [];
        grid.forEachNode(function (n) {
            satirlar.push({ kod1: n.data.kod1, kod2: n.data.kod2, kod3: n.data.kod3, yil0: n.data.yil0, yil1: n.data.yil1, yil2: n.data.yil2 });
        });
        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/tavaneko/kaydet', { yil: acik.yil, eko: acik.eko, sektor: acik.sektor, satirlar: satirlar })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (!s.basarili) return;
                kirli = false;
                if (durum < 0) durum = 1; // ilk kayıttan sonra onaylanabilir
            })
            .fail(hataGoster)
            .always(function () { durumGoster(); });
    }

    $('#btnKaydet').on('click', function () {
        if (!acik) return;
        grid.stopEditing();
        sor('Onay', 'Kaydetmek istiyor musunuz?', 'Kaydet', kaydet);
    });

    function onayKaydet(onayla) {
        Sgb.gonder('api/tavaneko/onay', { yil: acik.yil, eko: acik.eko, sektor: acik.sektor, onayla: onayla })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (s.basarili) { durum = s.durum; durumGoster(); }
            })
            .fail(hataGoster);
    }

    $('#btnOnayla').on('click', function () {
        if (!acik) return;
        if (kirli) { mesaj('Önce değişiklikleri kaydediniz.', 'warning'); return; }
        sor('Onay', 'Onayladıktan sonra birim tavanlarını, ekonomik kod ve sektör için değiştiremezsiniz. Devam etmek istiyor musunuz?', 'Onayla',
            function () { onayKaydet(true); });
    });

    $('#btnOnayKaldir').on('click', function () {
        if (!acik) return;
        sor('Onay', 'Tavan girişi veri girişine açılacaktır. Devam etmek istiyor musunuz?', 'Onayı Kaldır', function () { onayKaydet(false); });
    });

    // OVMP'den Al: boş (sıfır) tavanlar OVMP tutarlarıyla doldurulur (bine aşağı yuvarlanır). yil1 ve yil2 ikinci OVMP yılından alınır.
    $('#btnOvmp').on('click', function () {
        if (!acik || onayli()) return;
        grid.stopEditing();
        var say = 0;
        grid.forEachNode(function (n) {
            var d = n.data;
            if (!d.yil0) { n.setDataValue('yil0', Math.floor(d.odenek_1_1 / 1000) * 1000); say++; }
            if (!d.yil1) { n.setDataValue('yil1', Math.floor(d.odenek_1_2 / 1000) * 1000); say++; }
            if (!d.yil2) { n.setDataValue('yil2', Math.floor(d.odenek_1_2 / 1000) * 1000); say++; }
        });
        toplamHesapla();
        mesaj(say ? 'Boş tavanlar OVMP tutarlarıyla dolduruldu. Kaydetmeyi unutmayınız.' : 'Boş tavan yok.', 'info');
    });

    $('#btnTavanUret').on('click', function () {
        var yil = parseInt($('#txtYil').val(), 10) || 0;
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        sor('Tavan Üret', 'Var olan tavanlar silinecek ve OVMP\'den üretilen rakamlar tavan olarak kaydedilecektir. Onaylıyor musunuz?', 'Tavan Üret', function () {
            var $d = $('#btnTavanUret').prop('disabled', true);
            Sgb.gonder('api/tavaneko/uret', { yil: yil, yatirimAsamasi: $('#chkYatirim').prop('checked') })
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili && acik) { kirli = false; listele(); }
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // Kopyala: tablo (başlıklar ve toplam satırı dahil) sekmeyle ayrılmış metin olarak panoya; Excel'e yapıştırılabilir.
    $('#btnKopyala').on('click', function () {
        grid.stopEditing();
        var sutun = grid.getAllDisplayedColumns();
        var satirlar = [sutun.map(function (c) { return c.getColDef().headerName; }).join('\t')];
        function ekle(n) {
            satirlar.push(sutun.map(function (c) {
                var v = grid.getCellValue({ rowNode: n, colKey: c, useFormatter: true });
                return String(v == null ? '' : v).replace(/[\t\r\n]+/g, ' ');
            }).join('\t'));
        }
        var ust = grid.getPinnedTopRow(0);
        if (ust) ekle(ust);
        grid.forEachNodeAfterFilterAndSort(ekle);
        var metin = satirlar.join('\r\n');
        var tamam = function () { mesaj('Tablo panoya kopyalandı.', 'success'); };
        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(metin).then(tamam, function () { mesaj('Panoya kopyalanamadı.', 'danger'); });
        } else {
            var $t = $('<textarea readonly></textarea>').val(metin).css({ position: 'fixed', left: '-9999px' }).appendTo(document.body);
            $t[0].select();
            try { document.execCommand('copy'); tamam(); } catch (e) { mesaj('Panoya kopyalanamadı.', 'danger'); }
            $t.remove();
        }
    });

    $('.tavan-arac').on('click', '[data-rapor]', function () {
        var yil = parseInt($('#txtYil').val(), 10) || 0;
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/tavaneko/' + $d.attr('data-rapor'), { yil: yil, yatirim: $('#chkYatirim').prop('checked') })
            .then(function () { $d.prop('disabled', false); });
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/tavaneko/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            sistemYoneticisi = t.sistemYoneticisi;
            $('#txtYil').val(t.yil);
            secenekler($('#ddlEko'), t.kodlar.ekoKodlar);
            secenekler($('#ddlSektor'), t.kodlar.sektorler);
            grid.setGridOption('columnDefs', sutunlar(t.yil));
        })
        .fail(hataGoster);
});
