// Alt Proje Bütçe Kaynağı Giriş (ButceHazirlikv2/AltProjeButceKaynakGiris; eski AltProjeButceKaynakGiris.aspx).
// AltProjeDetay'ın kaynak sekmesinde çerçeve olarak açılır: ?menuYok=1&yil=&senaryoKod=&altProjeKodu=&kaynakNo=
// Kaynak bilgileri ve tablolar /api/yatkaynak/oku ile gelir; Kaydet hepsini birlikte gönderir (eski btnKaydet_Click).
// Hangi sekmenin, hangi sütunun girilebildiği kaynak türüne göre değişir (eski DivYonet / GirisAcKapat); teklifte yalnızca
// aktif yılın satırı, harcamada yıl-1 satırının 3. sütundan sonrası ve yıl-2 satırının ilk sütunları girilir (eski BeforeEditPar).
// Girilemeyen hücreler açık sarıdır. Kaydedince / silince ana sayfaya (window.parent.YatProje) bildirilir.
$(function () {
    'use strict';

    if (!$('#grdTeklif').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TUR = { BUTCE: 1, KREDIBUTCEICI: 2, KREDIBUTCEDISI: 3, HIBE: 4, DONERSERMAYE: 5, YENIPROJE: 6 };

    var adres = new URLSearchParams(location.search);
    var yil = parseInt(adres.get('yil'), 10) || 0;
    var senaryoKod = adres.get('senaryoKod') || '';
    var altProjeKodu = parseInt(adres.get('altProjeKodu'), 10) || 0;
    var kaynakNo = parseInt(adres.get('kaynakNo'), 10) || 0;

    var yonetici = false;
    var kaynakTur = 0;          // okunan kaynağın türü (0: okunmadı)
    var odenekleriGir = false;  // ödenek / harcama sütunları girilir mi (eski DivYonet: hibe, bütçe dışı kredi, döner sermaye)
    var silindi = false;

    var tamsayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
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
        var is = soruIsi;
        soruIsi = null;
        if (is) is();
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- sekmeler

    function sekmeAc(hedef) {
        var $a = $('.kaynak-sekmeler a[href="' + hedef + '"]');
        $('.kaynak-sekmeler li').removeClass('active').find('a').attr('aria-selected', 'false');
        $a.attr('aria-selected', 'true').parent().addClass('active');
        $('.kaynak-panel').prop('hidden', true);
        $(hedef).prop('hidden', false);
    }

    $('.kaynak-sekmeler').on('click', 'a', function (olay) {
        olay.preventDefault();
        sekmeAc($(this).attr('href'));
    });

    function sekmeGoster(hedef, goster) {
        $('.kaynak-sekmeler a[href="' + hedef + '"]').parent().prop('hidden', !goster);
    }

    // ---------------------------------------------------------------- tablo yardımcıları

    // Sütun tanımları: girilir(p) hücrenin girilip girilemeyeceğini söyler; sari true ise girilemeyen hücre açık sarı gösterilir.
    function hucreSinif(sari, girilir, sayi) {
        return function (p) {
            var s = [];
            if (sayi) s.push('sayi');
            var g = girilir(p);
            if (g) s.push('girilir');
            else if (sari) s.push('kapali');
            return s;
        };
    }

    function sayiSutun(alan, baslik, girilir, sari, ozellik) {
        return $.extend({
            field: alan, colId: alan, headerName: baslik, width: 110, minWidth: 80, sortable: false, headerClass: 'sayi',
            editable: girilir, cellClass: hucreSinif(sari, girilir, true), cellEditor: 'agTextCellEditor',
            valueFormatter: function (p) { return p.value == null || p.value === '' ? '' : tamsayi.format(p.value); },
            valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; }
        }, ozellik || {});
    }

    function yilSutun(alan, baslik, girilir, sari, ozellik) {
        return $.extend({
            field: alan, colId: alan, headerName: baslik, width: 80, minWidth: 70, sortable: false, headerClass: 'sayi',
            editable: girilir, cellClass: hucreSinif(sari, girilir, true), cellEditor: 'agTextCellEditor',
            valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : Math.round(n); }
        }, ozellik || {});
    }

    function metinSutun(alan, baslik, girilir, sari, ozellik) {
        return $.extend({
            field: alan, colId: alan, headerName: baslik, width: 200, sortable: false, tooltipField: alan,
            editable: girilir, cellClass: hucreSinif(sari, girilir, false), cellEditor: 'agTextCellEditor'
        }, ozellik || {});
    }

    function hic() { return false; }
    function hep() { return true; }

    var GRID_ORTAK = {
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        tooltipShowDelay: 400
    };
    var SECIMLI = { rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false } };

    function satirlar(grid) {
        var liste = [];
        grid.forEachNode(function (n) { liste.push(n.data); });
        return liste;
    }

    // Excel'den kopyalanan değerleri (sekme / satır ayraçlı) odaklı hücreden başlayarak girilebilen hücrelere yazar
    // (eski EXTVeriYapistir). Satır eklenmez; girilemeyen hücreler atlanır.
    function yapistirKur(grid, element) {
        element.addEventListener('paste', function (olay) {
            if (grid.getEditingCells().length) return; // düzenleyicideki yapıştırma olağan çalışır
            var odak = grid.getFocusedCell();
            if (!odak) return;
            var metin = (olay.clipboardData || window.clipboardData).getData('text');
            if (!metin) return;
            olay.preventDefault();
            var satirlarMetin = metin.replace(/\r/g, '').split('\n');
            if (satirlarMetin.length && satirlarMetin[satirlarMetin.length - 1] === '') satirlarMetin.pop();
            var sutunlar = grid.getAllDisplayedColumns().filter(function (s) { return !!s.getColDef().field; });
            var ilkSutun = sutunlar.findIndex(function (s) { return s.getColId() === odak.column.getColId(); });
            if (ilkSutun < 0) return;
            var sayac = 0;
            $.each(satirlarMetin, function (i, satir) {
                var dugum = grid.getDisplayedRowAtIndex(odak.rowIndex + i);
                if (!dugum) return false;
                $.each(satir.split('\t'), function (j, deger) {
                    var sutun = sutunlar[ilkSutun + j];
                    if (!sutun) return false;
                    var tanim = sutun.getColDef();
                    var p = { node: dugum, data: dugum.data, colDef: tanim, column: sutun, api: grid };
                    var girilir = typeof tanim.editable === 'function' ? tanim.editable(p) : !!tanim.editable;
                    if (!girilir) return;
                    var eski = dugum.data[tanim.field];
                    var yeni = tanim.valueParser ? tanim.valueParser($.extend(p, { newValue: deger, oldValue: eski })) : deger;
                    dugum.setDataValue(sutun.getColId(), yeni);
                    sayac++;
                });
            });
            if (sayac) mesaj(sayac + ' hücre yapıştırıldı.', 'info');
        });
    }

    function gridKur(secici, ozellikler) {
        var el = document.querySelector(secici);
        var grid = Sgb.grid(el, $.extend({}, GRID_ORTAK, ozellikler));
        yapistirKur(grid, el);
        return grid;
    }

    // Teklif ve Harcama: yıl 5 rakamlık, ödenek sütunları 15 haneli tutara (binlik ayraçlarıyla) göre ve hepsi aynı genişlikte.
    var ODENEK_GENISLIK = 90;
    var ODENEK = { width: ODENEK_GENISLIK, minWidth: ODENEK_GENISLIK };
    var YIL_GENISLIK = { pinned: 'left', width: 64, minWidth: 64 };

    // ---------------------------------------------------------------- Teklif (eski grdTeklif)

    // Teklifte yalnızca aktif yılın satırı girilir; ödenek sütunları kaynak türüne göre açılır, sonraki yıllar hep açıktır.
    function teklifGirilir(sutunAcik) {
        return function (p) { return !!p.data && sutunAcik() && p.data.butceYil === yil && !silindi; };
    }
    var odenekAcik = function () { return odenekleriGir; };

    var grdTeklif = gridKur('#grdTeklif', {
        columnDefs: [
            yilSutun('butceYil', 'Yıl', hic, true, YIL_GENISLIK),
            { headerName: 'Proje Tutarı', children: [
                sayiSutun('projeTutarDis', 'Dış', hic, true, ODENEK),
                sayiSutun('projeTutarToplam', 'Toplam', hic, true, ODENEK)] },
            { headerName: 'Yılı Teklifi', children: [
                sayiSutun('odenek0Dis', 'Kredi', teklifGirilir(odenekAcik), true, ODENEK),
                sayiSutun('odenek0Toplam', 'Toplam', teklifGirilir(odenekAcik), true, ODENEK)] },
            { headerName: 'Yılı+1 Teklifi', children: [
                sayiSutun('odenek1Dis', 'Kredi', teklifGirilir(odenekAcik), true, ODENEK),
                sayiSutun('odenek1Toplam', 'Toplam', teklifGirilir(odenekAcik), true, ODENEK)] },
            { headerName: 'Yılı+2 Teklifi', children: [
                sayiSutun('odenek2Dis', 'Kredi', teklifGirilir(odenekAcik), true, ODENEK),
                sayiSutun('odenek2Toplam', 'Toplam', teklifGirilir(odenekAcik), true, ODENEK)] },
            { headerName: 'Sonraki Yıllar', children: [
                sayiSutun('odenekSDis', 'Kredi', teklifGirilir(hep), true, ODENEK),
                sayiSutun('odenekSToplam', 'Toplam', teklifGirilir(hep), true, ODENEK)] }
        ]
    });

    // ---------------------------------------------------------------- Harcama (eski grdHarcama)

    // Eski BeforeEditPar sütun sırasıyla (0: yıl): yıl-1 satırında 3'ten sonraki, yıl-2 satırında 3 ve öncesi sütunlar.
    function harcamaGirilir(sira, sutunAcik) {
        return function (p) {
            if (!p.data || silindi || !sutunAcik()) return false;
            if (p.data.butceYil === yil - 1 && sira > 3) return true;
            if (p.data.butceYil === yil - 2 && sira <= 3) return true;
            return false;
        };
    }

    var grdHarcama = gridKur('#grdHarcama', {
        columnDefs: [
            yilSutun('butceYil', 'Yıl', hic, true, YIL_GENISLIK),
            { headerName: 'Kümülatif Harcama (Re.Ga.)', children: [
                sayiSutun('kumulatifHarcamaDis', 'Kredi', harcamaGirilir(1, hep), true, ODENEK),
                sayiSutun('kumulatifHarcamaToplam', 'Toplam', harcamaGirilir(2, hep), true, ODENEK)] },
            { headerName: 'Haziran Sonu Harcama', children: [
                sayiSutun('harcama6ayDis', 'Kredi', harcamaGirilir(3, odenekAcik), true, ODENEK),
                sayiSutun('harcama6ayToplam', 'Toplam', harcamaGirilir(4, odenekAcik), true, ODENEK)] },
            { headerName: 'Yıl Sonu Harcama', children: [
                sayiSutun('harcamaYilSonuDis', 'Kredi', harcamaGirilir(5, odenekAcik), true, ODENEK),
                sayiSutun('harcamaYilSonuToplam', 'Toplam', harcamaGirilir(6, odenekAcik), true, ODENEK)] }
        ]
    });

    // ---------------------------------------------------------------- Alt Faaliyetler (eski grdAltFaaliyet)

    function acikMi() { return !silindi; }

    var grdAltFaaliyet = gridKur('#grdAltFaaliyet', $.extend({
        columnDefs: [
            metinSutun('pebKod', 'Alt Faaliyet Kodu', acikMi, true, { width: 200 }),
            metinSutun('pebAd', 'Alt Faaliyet Adı', hic, true, { flex: 1, minWidth: 240 })
        ]
    }, SECIMLI));

    // ---------------------------------------------------------------- Kredi/Hibe Planı (eski grdKrediPlan; başlıkta toplamlar)

    var KREDI_ALANLAR = ['anlasmaTutari', 'anlasmaTutariIc', 'kullanilanTutar', 'kullanilanTutarIc', 'kullanilacakTutar', 'kullanilacakTutarIc'];
    var krediToplam = {};

    function toplamBaslik(toplamlar, alan) {
        return function () { return tamsayi.format(toplamlar[alan] || 0); };
    }

    function krediGrup(baslik, alan) {
        return { headerName: baslik, headerClass: 'sayi', children: [
            sayiSutun(alan, '', acikMi, false, { width: 120, headerValueGetter: toplamBaslik(krediToplam, alan), headerTooltip: baslik + ' toplamı' })] };
    }

    var grdKrediPlan = gridKur('#grdKrediPlan', $.extend({
        columnDefs: [
            { headerName: 'Yıl', children: [yilSutun('yil', 'Toplam', acikMi, false, { width: 90 })] },
            krediGrup('Anlaşma Tutarı Kredi', 'anlasmaTutari'),
            krediGrup('Anlaşma Tutarı İç Kaynak', 'anlasmaTutariIc'),
            krediGrup('Kullanılan Tutar Kredi', 'kullanilanTutar'),
            krediGrup('Kullanılan Tutar İç Kaynak', 'kullanilanTutarIc'),
            krediGrup('Kullanılacak Tutar Kredi', 'kullanilacakTutar'),
            krediGrup('Kullanılacak Tutar İç Kaynak', 'kullanilacakTutarIc')
        ],
        groupHeaderHeight: 44,
        onCellValueChanged: function (e) { toplamlariHesapla(e.api, KREDI_ALANLAR, krediToplam); },
        onRowDataUpdated: function (e) { toplamlariHesapla(e.api, KREDI_ALANLAR, krediToplam); }
    }, SECIMLI));

    // Başlıktaki toplamlar (eski ToplamlariYaz / grdKrediPlan_AfterEdit; burada her değişiklikte yenilenir).
    function toplamlariHesapla(grid, alanlar, toplamlar) {
        $.each(alanlar, function (i, a) { toplamlar[a] = 0; });
        grid.forEachNode(function (n) {
            $.each(alanlar, function (i, a) { toplamlar[a] += Number(n.data[a]) || 0; });
        });
        grid.refreshHeader();
    }

    // ---------------------------------------------------------------- Kredi Geri Ödeme (eski grdKrediOdeme)

    var grdKrediOdeme = gridKur('#grdKrediOdeme', $.extend({
        columnDefs: [
            yilSutun('yil', 'Yıl', acikMi, false, { width: 90 }),
            sayiSutun('anaParaGeriOdeme', 'Ana Para Geri Ödeme', acikMi, false, { width: 150 }),
            sayiSutun('odenecekFaiz', 'Ödenecek Faiz', acikMi, false, { width: 130 }),
            metinSutun('projeninSonDurumu', 'Projenin Son Durumu', acikMi, false, { width: 360 }),
            metinSutun('darBogazCozum', 'Dar Boğaz Çözüm', acikMi, false, { width: 360 })
        ]
    }, SECIMLI));

    // ---------------------------------------------------------------- Proje Teklif Rakamları (eski grdYeniTeklif; başlıkta toplamlar)

    var YENI_ALANLAR = ['projeTutariDis', 'projeTutariIc', 'disKredi0', 'icKaynak0', 'disKredi1', 'icKaynak1', 'disKredi2', 'icKaynak2'];
    var yeniToplam = {};

    function yeniCift(baslik, disAlan, icAlan) {
        function alt(ad, alan) {
            return { headerName: ad, headerClass: 'sayi', children: [
                sayiSutun(alan, '', acikMi, false, { width: 115, headerValueGetter: toplamBaslik(yeniToplam, alan), headerTooltip: baslik + ' ' + ad + ' toplamı' })] };
        }
        return { headerName: baslik, headerClass: 'orta', children: [alt('Dış Kredi', disAlan), alt('İç Kaynak', icAlan)] };
    }

    function yeniTeklifSutunlari() {
        return [
            { headerName: 'Ekonomik', headerClass: 'orta', children: [
                { headerName: 'Kod', children: [metinSutun('ekoKod', '', hic, false, { width: 110, pinned: 'left' })] },
                { headerName: 'Ad', children: [metinSutun('ekoAd', '', hic, false, { width: 280 })] }] },
            yeniCift('Proje Tutarı', 'projeTutariDis', 'projeTutariIc'),
            yeniCift(String(yil), 'disKredi0', 'icKaynak0'),
            yeniCift(String(yil + 1), 'disKredi1', 'icKaynak1'),
            yeniCift(String(yil + 2), 'disKredi2', 'icKaynak2')
        ];
    }

    var grdYeniTeklif = gridKur('#grdYeniTeklif', $.extend({
        columnDefs: yeniTeklifSutunlari(),
        onCellValueChanged: function (e) { toplamlariHesapla(e.api, YENI_ALANLAR, yeniToplam); },
        onRowDataUpdated: function (e) { toplamlariHesapla(e.api, YENI_ALANLAR, yeniToplam); }
    }, SECIMLI));

    var GRIDLER = { altFaaliyet: grdAltFaaliyet, krediPlan: grdKrediPlan, krediOdeme: grdKrediOdeme, yeniTeklif: grdYeniTeklif };

    // Toplam sütunlarının başlıkları satır ekleme / silmede de yenilensin.
    function toplamYenile(ad) {
        if (ad === 'krediPlan') toplamlariHesapla(grdKrediPlan, KREDI_ALANLAR, krediToplam);
        else if (ad === 'yeniTeklif') toplamlariHesapla(grdYeniTeklif, YENI_ALANLAR, yeniToplam);
    }

    // Satır Aç (eski addRecord / insertRecord): kredi planında satır başa, eskisi gibi bu yılla eklenir.
    var BOS_SATIR = {
        altFaaliyet: function () { return { pebKod: '', pebAd: '' }; },
        krediPlan: function () { return { yil: new Date().getFullYear(), anlasmaTutari: 0, anlasmaTutariIc: 0, kullanilanTutar: 0, kullanilanTutarIc: 0, kullanilacakTutar: 0, kullanilacakTutarIc: 0 }; },
        krediOdeme: function () { return { yil: new Date().getFullYear(), anaParaGeriOdeme: 0, odenecekFaiz: 0, projeninSonDurumu: '', darBogazCozum: '' }; }
    };
    var ILK_SUTUN = { altFaaliyet: 'pebKod', krediPlan: 'yil', krediOdeme: 'yil' };

    $('[data-satir-ac]').on('click', function () {
        if (!kaynakTur || silindi) return;
        var ad = $(this).attr('data-satir-ac');
        var grid = GRIDLER[ad];
        grid.stopEditing();
        var islem = { add: [BOS_SATIR[ad]()] };
        if (ad === 'krediPlan') islem.addIndex = 0;
        var r = grid.applyTransaction(islem);
        toplamYenile(ad);
        if (r && r.add && r.add.length) {
            grid.ensureIndexVisible(r.add[0].rowIndex);
            grid.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: ILK_SUTUN[ad] });
        }
    });

    // Seçilileri sil (eski grdYeniTeklif.deleteSelected); kayıt Kaydet ile yapılır.
    $('[data-satir-sil]').on('click', function () {
        if (!kaynakTur || silindi) return;
        var ad = $(this).attr('data-satir-sil');
        var grid = GRIDLER[ad];
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Lütfen silinecek satırları seçiniz!', 'warning'); return; }
        grid.stopEditing(true);
        grid.applyTransaction({ remove: secili });
        toplamYenile(ad);
    });

    // ---------------------------------------------------------------- ekonomik kod seçimi (eski ddEko + SatirEkle)

    var dlgEko = document.getElementById('dlgEko');
    var agacEko = null;
    var seciliEko = null; // { kod, ad }

    // Ağaç düğüm adı "kod- ad" biçimindedir; satıra yalnızca ad yazılır.
    function ekoAdAyir(d) {
        var ad = d.ad || '';
        var on = d.kod + '-';
        if (ad.indexOf(on) === 0) ad = ad.substring(on.length);
        return $.trim(ad);
    }

    $('#btnEkoSec').on('click', function () {
        dlgEko.showModal();
        if (agacEko) return;
        agacEko = Sgb.agac(document.getElementById('agacEko'), {
            yukle: function (ust) {
                return Sgb.al('api/yatkaynak/ekokodlar', { yil: yil, ust: ust ? ust.kod : '' }).then(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                    return v.dugumler;
                });
            },
            secilince: function (d) {
                seciliEko = { kod: d.kod, ad: ekoAdAyir(d) };
                $('#txtEko').val(d.ad);
                dlgEko.close();
            },
            seciliMi: function (d) { return !!seciliEko && seciliEko.kod === d.kod; },
            ilkAcik: true,
            bosMetin: 'Kod bulunamadı.',
            hata: hataGoster
        });
    });

    // Yalnızca son kırılım (noktasız 6 hane) eklenebilir (eski SatirEkle).
    $('#btnEkoEkle').on('click', function () {
        if (!kaynakTur || silindi) return;
        if (!seciliEko) { mesaj('Lütfen ekonomik kod seçiniz!', 'warning'); return; }
        if (seciliEko.kod.replace(/\./g, '').length !== 6) { mesaj('Lütfen ekonomik kodun son kırılımını seçiniz!', 'warning'); return; }
        grdYeniTeklif.stopEditing();
        var r = grdYeniTeklif.applyTransaction({ add: [{
            ekoKod: seciliEko.kod, ekoAd: seciliEko.ad, projeTutariDis: 0, projeTutariIc: 0,
            disKredi0: 0, icKaynak0: 0, disKredi1: 0, icKaynak1: 0, disKredi2: 0, icKaynak2: 0
        }] });
        toplamYenile('yeniTeklif');
        if (r && r.add && r.add.length) grdYeniTeklif.ensureIndexVisible(r.add[0].rowIndex, 'bottom');
    });

    // ---------------------------------------------------------------- ekran düzeni (eski DivYonet)

    function ekranAyarla() {
        var t = kaynakTur;
        odenekleriGir = t === TUR.HIBE || t === TUR.KREDIBUTCEDISI || t === TUR.DONERSERMAYE;
        var yeniProje = t === TUR.YENIPROJE;

        sekmeGoster('#sekTeklif', !yeniProje);
        sekmeGoster('#sekHarcama', !yeniProje);
        sekmeGoster('#sekAltFaaliyet', !yeniProje && yonetici && (t === TUR.BUTCE || t === TUR.KREDIBUTCEICI));
        sekmeGoster('#sekKrediOdeme', !yeniProje && !(t === TUR.BUTCE || t === TUR.HIBE || t === TUR.DONERSERMAYE));
        sekmeGoster('#sekKrediPlan', yeniProje || t === TUR.HIBE || t === TUR.KREDIBUTCEDISI || t === TUR.KREDIBUTCEICI);
        sekmeGoster('#sekYeniTeklif', yeniProje);
        sekmeAc(yeniProje ? '#sekYeniTeklif' : '#sekTeklif');

        $('#divKrediBilgi').prop('hidden', t === TUR.BUTCE || t === TUR.DONERSERMAYE || yeniProje);

        $.each([grdTeklif, grdHarcama], function (i, g) { g.refreshCells({ force: true }); });
    }

    function tuslariAyarla() {
        var acik = kaynakTur > 0 && !silindi;
        $('#btnKaydet, #btnSil').prop('disabled', !acik);
        $('#btnTasi').prop('hidden', !yonetici).prop('disabled', !acik);
        $('#frmKaynak').find('input, select').prop('disabled', !acik);
        $('[data-satir-ac], [data-satir-sil], #btnEkoSec, #btnEkoEkle, #btnYazdir, #btnYazdirIzleme').prop('disabled', !acik);
    }

    // ---------------------------------------------------------------- okuma (eski ButceKaynakListele)

    function paraBirimleriDoldur(liste, secili) {
        var $s = $('#ddlParaBirimi').empty();
        $.each(liste || [], function (i, p) { $('<option></option>').val(p.kod).text(p.kisaltma).appendTo($s); });
        if (secili && !$s.find('option').filter(function () { return Number(this.value) === secili; }).length)
            $('<option></option>').val(secili).text(String(secili)).appendTo($s);
        $s.val(secili ? String(secili) : ($s.find('option').first().val() || ''));
    }

    function formDoldur(k) {
        $('#txtKaynakAd').val(k.kaynakAd);
        $('#lblKaynakTur').text(k.turAd);
        $('#lblTurEtiket').text(k.turAd).prop('hidden', !k.turAd);
        $('#ddlToplamDahil').val(String(k.toplamaDahil || 1));
        $('#chkDPTRapor').prop('checked', !!k.dptRapor);
        $('#chkYIKOB').prop('checked', !!k.yikob);
        $('#txtKrediyiVeren').val(k.krediyiVeren);
        $('#txtBaslamaTarihi').val(k.baslamaTarih);
        $('#txtYururlukTarihi').val(k.yururlukTarih);
        $('#txtTur').val(k.tur);
        $('#txtTutar').val(k.tutar);
        $('#txtVade').val(k.vade);
        $('#txtOdemesizDonem').val(k.odemesizDonem);
        $('#txtFaizOran').val(k.faizOran);
        $('#txtIcParaKrediOran').val(k.icParaKrediOran);
        $('#txtDipnot').val(k.dipnot);
    }

    function oku() {
        if (kaynakNo <= 0) { mesaj('Bütçe Kaynak No boş geliyor.', 'danger'); return; }
        var tumu = [grdTeklif, grdHarcama, grdAltFaaliyet, grdKrediPlan, grdKrediOdeme, grdYeniTeklif];
        $.each(tumu, function (i, g) { g.setGridOption('loading', true); });
        Sgb.al('api/yatkaynak/oku', { yil: yil, senaryoKod: senaryoKod, kaynakNo: kaynakNo })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                yonetici = !!v.yonetici;
                kaynakTur = v.kaynak.kaynakTur;
                paraBirimleriDoldur(v.paraBirimleri, v.kaynak.pbKod);
                formDoldur(v.kaynak);
                grdTeklif.setGridOption('rowData', v.teklif);
                grdHarcama.setGridOption('rowData', v.harcama);
                grdAltFaaliyet.setGridOption('rowData', v.altFaaliyet);
                grdKrediPlan.setGridOption('rowData', v.krediPlan);
                grdKrediOdeme.setGridOption('rowData', v.krediOdeme);
                grdYeniTeklif.setGridOption('rowData', v.yeniTeklif);
                ekranAyarla();
                tuslariAyarla();
            })
            .fail(hataGoster)
            .always(function () { $.each(tumu, function (i, g) { g.setGridOption('loading', false); }); });
    }

    // ---------------------------------------------------------------- Kaydet (eski btnKaydet_Click)

    function tumunuDurdur() {
        $.each([grdTeklif, grdHarcama, grdAltFaaliyet, grdKrediPlan, grdKrediOdeme, grdYeniTeklif], function (i, g) { g.stopEditing(); });
    }

    // Teklif / harcama satırları sayı alanlarıyla gönderilir (sunucu tablonun kendi alanlarını kullanır).
    function sayisalSatirlar(grid) {
        return satirlar(grid).map(function (s) {
            var d = {};
            $.each(s, function (a, deger) { d[a] = Number(deger) || 0; });
            return d;
        });
    }

    function sayilar(liste, alanlar) {
        return liste.map(function (s) {
            var d = $.extend({}, s);
            $.each(alanlar, function (i, a) { d[a] = Number(d[a]) || 0; });
            return d;
        });
    }

    $('#btnKaydet').on('click', function () {
        if (!kaynakTur || silindi) return;
        tumunuDurdur();
        var kaynakAd = $('#txtKaynakAd').val();
        var veri = {
            yil: yil, senaryoKod: senaryoKod, altProjeKodu: altProjeKodu, kaynakNo: kaynakNo, kaynakTur: kaynakTur,
            kaynakAd: kaynakAd,
            toplamaDahil: parseInt($('#ddlToplamDahil').val(), 10) || 1,
            dptRapor: $('#chkDPTRapor').prop('checked'),
            yikob: $('#chkYIKOB').prop('checked'),
            pbKod: parseInt($('#ddlParaBirimi').val(), 10) || 1,
            dipnot: $('#txtDipnot').val(),
            krediyiVeren: $('#txtKrediyiVeren').val(),
            baslamaTarih: $('#txtBaslamaTarihi').val(),
            yururlukTarih: $('#txtYururlukTarihi').val(),
            tur: $('#txtTur').val(),
            tutar: $('#txtTutar').val(),
            vade: $('#txtVade').val(),
            odemesizDonem: $('#txtOdemesizDonem').val(),
            faizOran: $('#txtFaizOran').val(),
            icParaKrediOran: $('#txtIcParaKrediOran').val(),
            teklif: sayisalSatirlar(grdTeklif),
            harcama: sayisalSatirlar(grdHarcama),
            yeniTeklif: sayilar(satirlar(grdYeniTeklif), YENI_ALANLAR),
            altFaaliyet: satirlar(grdAltFaaliyet),
            krediDetay: sayilar(satirlar(grdKrediPlan), ['yil'].concat(KREDI_ALANLAR)),
            krediOdeme: sayilar(satirlar(grdKrediOdeme), ['yil', 'anaParaGeriOdeme', 'odenecekFaiz'])
        };
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/yatkaynak/kaydet', veri)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                // Ana sayfadaki sekme başlığı (eski KaydetSonrasi -> parent.ButceKaynakTabBaslik).
                try {
                    if (window.parent && window.parent !== window && window.parent.YatProje && window.parent.YatProje.kaynakAdDegisti)
                        window.parent.YatProje.kaynakAdDegisti(kaynakNo, s.kaynakAd !== undefined ? s.kaynakAd : kaynakAd);
                } catch (e) { /* ana sayfa erişilemezse yok sayılır */ }
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', !kaynakTur || silindi); });
    });

    // ---------------------------------------------------------------- Sil (eski btnSil_Click)

    $('#btnSil').on('click', function () {
        if (!kaynakTur || silindi) return;
        sor('Bu kaynağı ve kaynağa girilmiş bütün bilgileri silmek istiyor musunuz?', 'Sil', function () {
            var $d = $('#btnSil').prop('disabled', true);
            Sgb.gonder('api/yatkaynak/sil', { yil: yil, senaryoKod: senaryoKod, altProjeKodu: altProjeKodu, kaynakNo: kaynakNo })
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    silindi = true;
                    mesaj(s.bilgi, 'success');
                    tumunuDurdur();
                    $.each([grdTeklif, grdHarcama, grdAltFaaliyet, grdKrediPlan, grdKrediOdeme, grdYeniTeklif], function (i, g) { g.refreshCells({ force: true }); });
                    // Eski SilSonrasi -> parent.ButceKaynakTabSil
                    try {
                        if (window.parent && window.parent !== window && window.parent.YatProje && window.parent.YatProje.kaynakSilindi)
                            window.parent.YatProje.kaynakSilindi(kaynakNo);
                    } catch (e) { /* ana sayfa erişilemezse yok sayılır */ }
                })
                .fail(hataGoster)
                .always(function () { tuslariAyarla(); });
        });
    });

    // ---------------------------------------------------------------- Taşı (eski wndAltProjeler / btnTasiKomut_Click)

    var dlgTasi = document.getElementById('dlgTasi');
    var grdAltProjeler = null;

    $('#btnTasi').on('click', function () {
        if (!yonetici || !kaynakTur || silindi) return;
        dlgTasi.showModal();
        if (grdAltProjeler) return;
        grdAltProjeler = Sgb.grid('#grdAltProjeler', {
            columnDefs: [
                { field: 'kod', headerName: 'Kod', width: 80, cellClass: 'sayi', headerClass: 'sayi' },
                { field: 'projeAd', headerName: 'Proje', flex: 2, minWidth: 180, tooltipField: 'projeAd' },
                { field: 'ad', headerName: 'Ad', flex: 2, minWidth: 180, tooltipField: 'ad' },
                { field: 'projeAciklama', headerName: 'DPT Proje No', width: 130 }
            ],
            rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
            onRowDoubleClicked: function () { $('#btnTasiKomut').trigger('click'); },
            overlayNoRowsTemplate: '<span>Alt proje bulunamadı.</span>'
        });
        grdAltProjeler.setGridOption('loading', true);
        Sgb.al('api/yatkaynak/altprojeler', { yil: yil, senaryoKod: senaryoKod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grdAltProjeler.setGridOption('rowData', v.satirlar);
            })
            .fail(hataGoster)
            .always(function () { grdAltProjeler.setGridOption('loading', false); });
    });

    $('#btnTasiKomut').on('click', function () {
        var secili = grdAltProjeler ? grdAltProjeler.getSelectedRows() : [];
        if (secili.length !== 1) { mesaj('Lütfen taşınacak alt projeyi seçiniz.', 'warning'); return; }
        var hedef = secili[0];
        sor('Bu kaynağı "' + hedef.ad + '" alt projesine taşımak istiyor musunuz?', 'Taşı', function () {
            var $d = $('#btnTasiKomut').prop('disabled', true);
            Sgb.gonder('api/yatkaynak/tasi', { yil: yil, senaryoKod: senaryoKod, altProjeKodu: altProjeKodu, kaynakNo: kaynakNo, hedefAltProje: hedef.kod })
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    dlgTasi.close();
                    mesaj(s.bilgi, 'success');
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // ---------------------------------------------------------------- raporlar (eski btnYazdir_Click / btnYazdirIzleme_Click)

    function raporAl($d, adres) {
        if (!kaynakTur) return;
        $d.prop('disabled', true);
        Sgb.indir(adres, { yil: yil, senaryoKod: senaryoKod, kaynakNo: kaynakNo })
            .then(function () { $d.prop('disabled', false); });
    }

    $('#btnYazdir').on('click', function () { raporAl($(this), 'api/yatkaynak/yazdir'); });
    $('#btnYazdirIzleme').on('click', function () { raporAl($(this), 'api/yatkaynak/izleme'); });

    // ---------------------------------------------------------------- açılış

    tuslariAyarla();
    oku();
});
