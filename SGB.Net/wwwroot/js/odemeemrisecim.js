// Sgb.oem: Ödeme Emri / MİF formunun (eski ButceMuhasebe/OdemeEmriMIF2.aspx + ModulScripts/OdemeEmri2.js) açtığı küçük seçim ve
// gösterim pencereleri. Pencereleri bu dosya kurar (ilk açılışta, sonra aynı pencere kullanılır); sayfanın cshtml'ine dialog konmaz.
// Uçlar: /api/oemsecim/* (Muhasebe/OdemeEmriSecimUclari.cs). Görünüm: css/odemeemrisecim.css (+ tanim.css pencere çerçevesi).
// Gerekenler: jQuery, ortak.js, AG Grid; mahsup ve fatura pencerelerinde ayrıca muhbirim.js (+ muhbirim.css), mahsupta hesapkodu.js
// (+ hesapkodu.css) ve makbuzsecim.js (hesap planı penceresi). Bunlar yüklenmemişse alanlar düz metin kutusu olur.
//
// İşlev (eski sayfa)                                   geri çağrı (eski üst sayfa işlevi) ve alanları
// Sgb.oem.mahsupBelgeSec(o)  (ListeMahsupBelge.aspx)   o.secilince(mahsup)  (eski parent.MahsupSecKapatBasildi(mahsup))
//     o: { yil, ilgiliAd, ilgiliNo, muhasebe, birim, hesapKod }
//     mahsup: { belgeNo, tutar (BORÇ - ALACAK, sayı), altHesapKod ('ilgiliNo - ilgiliAd'), satirAciklama,
//               doviz, dovizKurTur, dovizBorc, dovizAlacak, dovizKur }  (döviz alanları belgenin 'kod*kurTur*borç*alacak*kur'
//               bilgisinden; olmayanlar '')
// Sgb.oem.surecFaturaSec(o)  (ListeSurecFatura.aspx)   o.secilince(fatura)  (eski parent.SurecFaturaSecKapatBasildi(fatura))
//     o: { yil, ilgiliAd, ilgiliNo, muhasebe, birim }   çoklu seçim
//     fatura: { faturaNo (seçilenler ';' ile), faturaTarih (ilk seçilenin, gg.aa.yyyy), surecYilNo (ilk seçilenin, 'yıl/no'),
//               tutarKDVli (seçilenlerin KDV'li toplamı, metin; ondalık ayıracı nokta) }
// Sgb.oem.isSec(o)           (ListeIs.aspx)            o.secilince(kod, adi)  (eski parent.IsBilgisiYaz(satir, {kod, adi}))
//     o: { yil, muh, muhBir }
// Sgb.oem.kesintiHesapla(o)  (KesintiHesapla2.aspx + ModulScripts/KesintiHesapla2.js)
//                                                      o.secilince(tutar, satir)  (eski parent.KesintiYaz(vergiTutari, satir))
//     o: { borc, avans, satir }   tutar: hesaplanan kesinti (sayı, eski gibi biçimsiz); satir: o.satir aynen
// Sgb.oem.taahhutSec(o)      (TaahhutKartiListe.aspx)  o.secilince(kod)  (eski parent.TaahhutEkraniBilgiYaz(kod); form kartın
//     o: { yil, vergiNo }                               bilgisini kendi ucuyla okur)
// Sgb.oem.kapatmaListesi(o)  (ListeKapatma.aspx)       gösterim; o: { yil, belgeNo }
// Sgb.oem.tarihce(o)         (BelgeTarihce.aspx)       gösterim; o: { yil, belgeNo }
// Sgb.oem.altHesapMuavin(o)  (MuavinDefteriAltHesapBazindaRapor.aspx)
//                                                      gösterim; o: { yil, hesapKod, altHesapKod, projeNo }. Belge numarasına
//     tıklanınca ödeme emri ButceMuhasebe/OdemeEmriMIFFrame sayfasında pencere olarak açılır. Aynı liste Razor sayfası
//     ButceMuhasebe/MuavinDefteriAltHesapBazindaRapor'da da (#oemMuavinSayfa; ?yil=&hesapKod=&altHesapKod=&projeNo=) bu dosyayla çizilir.
// Seçim pencerelerinde secilince false döndürürse pencere açık kalır.
(function (Sgb, $) {
    'use strict';

    var UC = 'api/oemsecim/';
    var para2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function tutar(x) { return x.value === null || x.value === undefined || x.value === '' ? '' : para2.format(Number(x.value) || 0); }
    function bos(v) { return v === undefined || v === null ? '' : String(v); }
    function buYil() { return new Date().getFullYear(); }

    function hataGoster(xhr, metin) {
        if (xhr && xhr.status === 401) return;
        Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : (metin || 'Bilgiler okunamadı.'), 'danger');
    }

    // Pencere çerçevesi (tanim.css .tanim-pencere). alt: Kapat'ın soluna konacak düğmeler.
    var sayac = 0;
    function pencereKur(sinif, baslik, govde, alt) {
        var kimlik = 'oemPencere' + (++sayac);
        var $p = $('<dialog class="tanim-pencere oem-pencere"></dialog>').addClass(sinif).attr('aria-labelledby', kimlik)
            .append($('<div class="pencere-baslik"></div>')
                .append($('<h2></h2>').attr('id', kimlik).text(baslik))
                .append('<button type="button" class="close" data-kapat aria-label="Kapat"><span aria-hidden="true">&times;</span></button>'))
            .append($('<div class="pencere-govde"></div>').append(govde))
            .append($('<div class="pencere-alt"></div>').append(alt || '').append('<button type="button" class="btn btn-default btn-sm" data-kapat>Kapat</button>'))
            .appendTo(document.body);
        $p.on('click', '[data-kapat]', function (e) { e.stopPropagation(); $p[0].close(); });
        return $p;
    }

    function baslikYaz($p, metin) { $p.find('.pencere-baslik h2').text(metin); }

    // Seçim sonucu üst forma verilir; secilince false dönerse pencere açık kalır.
    function sonucVer($p, islev, argumanlar) {
        var r = islev ? islev.apply(null, argumanlar) : undefined;
        if (r !== false) $p[0].close();
    }

    // Etiket solda, alan sağda bir satır
    function satir(etiket, alan, kimlik) {
        var $s = $('<div class="oem-satir"></div>');
        $('<label class="control-label"></label>').attr('for', kimlik || null).text(etiket).appendTo($s);
        $('<div class="oem-alan"></div>').append(alan).appendTo($s);
        return $s;
    }

    function metinKutu(sinif, kimlik, ek) {
        return $('<input type="text" class="form-control input-sm" autocomplete="off" />').addClass(sinif).attr($.extend({ id: kimlik }, ek || {}));
    }

    function yilKutu(kimlik) {
        return $('<input type="number" class="form-control input-sm sgb-yil oem-yil" step="1" />').attr({ id: kimlik, min: 2006, max: buYil() });
    }

    function onay(sinif, kimlik, metin, isaretli) {
        return $('<div class="checkbox oem-onay"></div>').append($('<label></label>').attr('for', kimlik)
            .append($('<input type="checkbox" />').addClass(sinif).attr('id', kimlik).prop('checked', !!isaretli))
            .append(document.createTextNode(' ' + metin)));
    }

    // Muhasebe - birim alanı: Sgb.muhasebeBirim varsa o, yoksa iki düz kutu. Dönen: { deger(), ata(muh, birim) }
    function muhBirimKur($yer, kimlikOnEk, yilIslev) {
        if (Sgb.muhasebeBirim) {
            var mb = Sgb.muhasebeBirim($yer, {
                kimlik: { muhasebe: kimlikOnEk + 'Muh', birim: kimlikOnEk + 'Birim' },
                yil: yilIslev, kurumsalGizle: true
            });
            return {
                deger: function () { var d = mb.deger(); return { muhasebe: d.muhasebe || '', birim: d.birim }; },
                ata: function (muh, birim) {
                    mb.ata({ muhasebe: parseInt(muh, 10) || '', birim: bos(birim) });
                    if (parseInt(muh, 10) > 0) mb.yenile();
                }
            };
        }
        var $m = metinKutu('oem-muh', kimlikOnEk + 'Muh', { placeholder: 'Muh.', 'aria-label': 'Muhasebe kodu' });
        var $b = metinKutu('oem-birim', kimlikOnEk + 'Birim', { placeholder: 'Brm', maxlength: 3, 'aria-label': 'Birim kodu' });
        $yer.addClass('oem-muh-birim').append($m, $b);
        return {
            deger: function () { return { muhasebe: $.trim($m.val()), birim: $.trim($b.val()) }; },
            ata: function (muh, birim) { $m.val(bos(muh)); $b.val(bos(birim)); }
        };
    }

    function gridYukle(grid, istek, yaz, bosMetin) {
        grid.setGridOption('loading', true);
        return istek.done(function (v) {
            if (v.hata) { Sgb.bildirim(v.hata, 'warning'); return; }
            grid.setGridOption('overlayNoRowsTemplate', '<span>' + (bosMetin || 'Kayıt bulunamadı.') + '</span>');
            yaz(v);
        }).fail(function (xhr) { hataGoster(xhr); })
            .always(function () { grid.setGridOption('loading', false); });
    }

    // ================================================================ 1. mahsup belgesi seçimi (eski ListeMahsupBelge.aspx)

    var mahsup = null; // { $p, grid, mb, hk, o }

    function mahsupKur() {
        var $form = $('<form class="oem-kriter" novalidate></form>');
        var $sol = $('<div class="oem-kolon"></div>'), $sag = $('<div class="oem-kolon"></div>');
        var $muhYer = $('<div class="oem-mb"></div>');
        var $hesap = metinKutu('oem-hesap kisa', 'oemMhHesap');
        var $goster = $('<select class="form-control input-sm oem-goster" id="oemMhGoster"></select>')
            .append('<option value="0">Hepsi</option><option value="1">Borç</option><option value="2">Alacak</option>');
        $sol.append(satir('Yıl', yilKutu('oemMhYil'), 'oemMhYil'))
            .append(satir('Muh. / Birim', $muhYer, 'oemMhMuh'))
            .append(satir('Hesap Kodu', $hesap, 'oemMhHesap'))
            .append(satir('Göster', $goster, 'oemMhGoster'));
        $sag.append(satir('Adı Soyadı/Ünvanı', metinKutu('oem-ilgili-ad', 'oemMhIlgiliAd'), 'oemMhIlgiliAd'))
            .append(satir('T.C./Vergi No', metinKutu('oem-ilgili-no kisa', 'oemMhIlgiliNo'), 'oemMhIlgiliNo'))
            .append(satir('', onay('oem-yevmiye', 'oemMhYevmiye', 'Sadece yevmiyeli belgeleri listele', true)))
            .append(satir('', onay('oem-kapatilmis', 'oemMhKapatilmis', 'Kapatılmış belgeleri gösterme', true)));
        $form.append($('<div class="oem-bolum oem-bolum-kriter"></div>').append($('<div class="oem-kolonlar"></div>').append($sol, $sag))
            .append('<div class="oem-tuslar"><button type="submit" class="btn btn-primary btn-sm"><span class="glyphicon glyphicon-search" aria-hidden="true"></span> Listele</button></div>'));
        var $govde = $('<div></div>').append($form)
            .append($('<div class="oem-bolum oem-bolum-liste"></div>')
                .append('<div class="oem-grid oem-mahsup-grid" aria-label="Mahsup edilecek belgeler"></div>'));
        var $sec = $('<button type="button" class="btn btn-primary btn-sm oem-sec"><span class="glyphicon glyphicon-ok" aria-hidden="true"></span> Seç ve Kapat</button>');
        mahsup = { $p: pencereKur('oem-mahsup', 'Mahsup Belgesi Seçimi', $govde, $sec), o: {} };
        var $p = mahsup.$p;

        var yil = function () { return parseInt($p.find('.oem-yil').val(), 10) || buYil(); };
        mahsup.mb = muhBirimKur($muhYer, 'oemMh', yil);
        mahsup.hk = Sgb.hesapKodu ? Sgb.hesapKodu($hesap[0], { yil: yil, sonSeviye: true, uc: UC + 'hesap/' }) : null;

        mahsup.grid = Sgb.grid($p.find('.oem-mahsup-grid')[0], {
            columnDefs: [
                { field: 'MUHASEBE', headerName: 'Muhasebe', width: 80, hide: true },
                { field: 'BIRIM', headerName: 'Birim', width: 70, hide: true },
                { field: 'YEVMIYENO', headerName: 'Yev.No', width: 80, type: 'rightAligned' },
                { field: 'YEVMIYETARIH', headerName: 'Yev.Tarih', width: 100 },
                { field: 'BELGENO', headerName: 'Belge No', width: 100 },
                { field: 'BELGETARIH', headerName: 'Belge Tarih', width: 100, hide: true },
                { field: 'ILGILI', headerName: 'İlgili', flex: 1, minWidth: 170, tooltipField: 'ILGILI' },
                { field: 'HESAPKOD', headerName: 'Hesap Kodu', width: 150 },
                { field: 'BORC', headerName: 'Borç', width: 110, type: 'rightAligned', valueFormatter: tutar },
                { field: 'ALACAK', headerName: 'Alacak', width: 110, type: 'rightAligned', valueFormatter: tutar }
            ],
            rowSelection: { mode: 'singleRow', checkboxes: true, enableClickSelection: true },
            overlayNoRowsTemplate: '<span>Kıstasları girip Listele\'ye basınız.</span>',
            onRowDoubleClicked: function (e) { mahsupSec(e.data); }
        });

        $form.on('submit', function (e) { e.preventDefault(); mahsupListele(); });
        $sec.on('click', function () {
            var s = mahsup.grid.getSelectedRows();
            if (!s.length) { Sgb.bildirim('Listeden bir belge seçiniz.', 'warning'); return; }
            mahsupSec(s[0]);
        });
    }

    function mahsupHesap() {
        var $h = mahsup.$p.find('.oem-hesap');
        // Eski: kutu boşsa formdan gelen hesap kodu kullanılır.
        return $.trim($h.val()) || bos(mahsup.o.hesapKod);
    }

    function mahsupListele() {
        var $p = mahsup.$p;
        var mb = mahsup.mb.deger();
        gridYukle(mahsup.grid, Sgb.al(UC + 'mahsup', {
            yil: parseInt($p.find('.oem-yil').val(), 10) || 0,
            muhasebe: mb.muhasebe, birim: mb.birim,
            hesapKod: mahsupHesap(),
            ilgiliNo: $.trim($p.find('.oem-ilgili-no').val()),
            ilgiliAd: $.trim($p.find('.oem-ilgili-ad').val()),
            yevmiye: $p.find('.oem-yevmiye').prop('checked') ? 1 : 0,
            kapatilmis: $p.find('.oem-kapatilmis').prop('checked') ? 1 : 0,
            goster: $p.find('.oem-goster').val()
        }), function (v) { mahsup.grid.setGridOption('rowData', v.satirlar || []); });
    }

    // Eski SeciliSatirBilgisiAyarla + SeciliBilgileriYaz
    function mahsupSec(d) {
        var k = bos(d.DOVIZ).split('*');
        var m = {
            belgeNo: d.BELGENO,
            tutar: (Number(d.BORC) || 0) - (Number(d.ALACAK) || 0),
            altHesapKod: bos(d.ILGILIKOD) + ' - ' + bos(d.ILGILI),
            satirAciklama: bos(d.SATIRACIKLAMA),
            doviz: k.length > 0 ? k[0] : '',
            dovizKurTur: k.length > 1 ? k[1] : '',
            dovizBorc: k.length > 2 ? k[2] : '',
            dovizAlacak: k.length > 3 ? k[3] : '',
            dovizKur: k.length > 4 ? k[4] : ''
        };
        sonucVer(mahsup.$p, mahsup.o.secilince, [m]);
    }

    function mahsupBelgeSec(o) {
        o = o || {};
        if (!mahsup) mahsupKur();
        mahsup.o = o;
        var $p = mahsup.$p;
        $p.find('.oem-yil').val(parseInt(o.yil, 10) || buYil());
        mahsup.mb.ata(o.muhasebe, o.birim);
        if (mahsup.hk) mahsup.hk.ata(bos(o.hesapKod));
        else $p.find('.oem-hesap').val(bos(o.hesapKod));
        $p.find('.oem-ilgili-no').val(bos(o.ilgiliNo));
        $p.find('.oem-ilgili-ad').val(bos(o.ilgiliAd));
        $p.find('.oem-goster').val('0');
        mahsup.grid.setGridOption('rowData', []);
        mahsup.grid.setGridOption('overlayNoRowsTemplate', '<span>Kıstasları girip Listele\'ye basınız.</span>');
        $p[0].showModal();
        // Eski: yıl, muhasebe, birim, ilgili no ve hesap kodu geldiyse açılışta listelenir.
        if ((parseInt(o.yil, 10) || 0) > 0 && bos(o.muhasebe) !== '' && bos(o.birim) !== '' && bos(o.ilgiliNo) !== '' && bos(o.hesapKod) !== '')
            mahsupListele();
    }

    // ================================================================ 2. süreç faturası seçimi (eski ListeSurecFatura.aspx)

    var fatura = null; // { $p, grid, mb, o }

    function faturaKur() {
        var $form = $('<form class="oem-kriter" novalidate></form>');
        var $sol = $('<div class="oem-kolon"></div>'), $sag = $('<div class="oem-kolon"></div>');
        var $muhYer = $('<div class="oem-mb"></div>');
        $sol.append(satir('Yıl', yilKutu('oemFtYil'), 'oemFtYil'))
            .append(satir('Muh. / Birim', $muhYer, 'oemFtMuh'));
        $sag.append(satir('Adı Soyadı/Ünvanı', metinKutu('oem-ilgili-ad', 'oemFtIlgiliAd'), 'oemFtIlgiliAd'))
            .append(satir('T.C./Vergi No', metinKutu('oem-ilgili-no kisa', 'oemFtIlgiliNo'), 'oemFtIlgiliNo'))
            .append(satir('', onay('oem-islendi', 'oemFtIslendi', 'Önceden işlem yapılanları gösterme', true)));
        $form.append($('<div class="oem-bolum oem-bolum-kriter"></div>').append($('<div class="oem-kolonlar"></div>').append($sol, $sag))
            .append('<div class="oem-tuslar"><button type="submit" class="btn btn-primary btn-sm"><span class="glyphicon glyphicon-search" aria-hidden="true"></span> Listele</button></div>'));
        var $govde = $('<div></div>').append($form)
            .append($('<div class="oem-bolum oem-bolum-liste"></div>')
                .append('<div class="oem-grid oem-fatura-grid" aria-label="Süreç faturaları"></div>'));
        var $sec = $('<button type="button" class="btn btn-primary btn-sm oem-sec"><span class="glyphicon glyphicon-ok" aria-hidden="true"></span> Seç ve Kapat</button>');
        fatura = { $p: pencereKur('oem-fatura', 'Fatura Seçimi', $govde, $sec), o: {} };
        var $p = fatura.$p;
        fatura.mb = muhBirimKur($muhYer, 'oemFt', function () { return parseInt($p.find('.oem-yil').val(), 10) || buYil(); });

        fatura.grid = Sgb.grid($p.find('.oem-fatura-grid')[0], {
            columnDefs: [
                { field: 'MUHASEBE', headerName: 'Muhasebe', width: 80, hide: true },
                { field: 'BIRIM', headerName: 'Birim', width: 70, hide: true },
                { field: 'SURECYILNO', headerName: 'Süreç No', width: 130 },
                { field: 'FATURANO', headerName: 'Fatura No', flex: 1, minWidth: 120 },
                { field: 'FATURATARIH', headerName: 'Fatura Tarih', width: 110 },
                { field: 'FATURATUTAR', headerName: 'Fatura Tutarı', width: 130, type: 'rightAligned', valueFormatter: tutar },
                { field: 'FATURATUTARKDVLI', headerName: 'Fatura Tutarı KDVli', width: 150, type: 'rightAligned', valueFormatter: tutar }
            ],
            rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
            overlayNoRowsTemplate: '<span>İlgilinin adını ya da T.C./Vergi numarasını yazıp Listele\'ye basınız.</span>'
        });

        $form.on('submit', function (e) { e.preventDefault(); faturaListele(); });
        $sec.on('click', faturaSec);
    }

    function faturaListele() {
        var $p = fatura.$p;
        var no = $.trim($p.find('.oem-ilgili-no').val()), ad = $.trim($p.find('.oem-ilgili-ad').val());
        if (!no && !ad) { Sgb.bildirim('İlgilinin adı veya TCKN/VKN bilgilerini doldurun', 'warning'); return; }
        var mb = fatura.mb.deger();
        gridYukle(fatura.grid, Sgb.al(UC + 'fatura', {
            yil: parseInt($p.find('.oem-yil').val(), 10) || 0,
            muhasebe: mb.muhasebe, birim: mb.birim, ilgiliNo: no, ilgiliAd: ad,
            islendi: $p.find('.oem-islendi').prop('checked') ? 1 : 0
        }), function (v) { fatura.grid.setGridOption('rowData', v.satirlar || []); });
    }

    // Eski SeciliBilgileriYaz: seçilen faturalar birleştirilir; tarih ve süreç ilk seçilenden.
    function faturaSec() {
        var secili = [];
        fatura.grid.forEachNodeAfterFilterAndSort(function (n) { if (n.isSelected()) secili.push(n.data); });
        if (!secili.length) { Sgb.bildirim('Listeden en az bir fatura seçiniz.', 'warning'); return; }
        var f = { faturaNo: '', faturaTarih: '', surecYilNo: '', tutarKDVli: 0.0 };
        $.each(secili, function (i, r) {
            if (f.faturaNo === '') {
                f.faturaTarih = bos(r.FATURATARIH);
                f.surecYilNo = bos(r.SURECYILNO);
            }
            if (f.faturaNo !== '') f.faturaNo += ';';
            f.faturaNo += bos(r.FATURANO);
            f.tutarKDVli += parseFloat(r.FATURATUTARKDVLI) || 0;
        });
        f.tutarKDVli = f.tutarKDVli + '';
        sonucVer(fatura.$p, fatura.o.secilince, [f]);
    }

    function surecFaturaSec(o) {
        o = o || {};
        if (!fatura) faturaKur();
        fatura.o = o;
        var $p = fatura.$p;
        $p.find('.oem-yil').val(parseInt(o.yil, 10) || buYil());
        fatura.mb.ata(o.muhasebe, o.birim);
        $p.find('.oem-ilgili-no').val(bos(o.ilgiliNo));
        $p.find('.oem-ilgili-ad').val(bos(o.ilgiliAd));
        fatura.grid.setGridOption('rowData', []);
        fatura.grid.setGridOption('overlayNoRowsTemplate', '<span>İlgilinin adını ya da T.C./Vergi numarasını yazıp Listele\'ye basınız.</span>');
        $p[0].showModal();
        // Eski: ilgili no geldiyse açılışta listelenir.
        if (bos(o.ilgiliNo) !== '') faturaListele();
    }

    // ================================================================ 3. iş seçimi (eski ListeIs.aspx)

    var isPen = null; // { $p, grid, o }

    function isSec(o) {
        o = o || {};
        if (!isPen) {
            var $govde = $('<div></div>')
                .append('<p class="help-block small oem-yardim">Seçmek için işin adına tıklayınız.</p>')
                .append('<div class="oem-grid oem-is-grid" aria-label="İş listesi"></div>');
            isPen = { $p: pencereKur('oem-is', 'İş Listesi', $govde), o: o };
            isPen.grid = Sgb.grid(isPen.$p.find('.oem-is-grid')[0], {
                columnDefs: [
                    { headerName: '#', width: 60, sortable: false, type: 'rightAligned', valueGetter: function (p) { return p.node.rowIndex + 1; } },
                    { field: 'adi', headerName: 'Adı', flex: 1, minWidth: 200, tooltipField: 'adi' }
                ],
                onRowClicked: function (e) {
                    if (e.data.kod === '' || e.data.kod === null || e.data.kod === undefined) {
                        Sgb.bildirim('Lütfen İş Listesinden seçim yapın.', 'warning');
                        return;
                    }
                    sonucVer(isPen.$p, isPen.o.secilince, [e.data.kod, e.data.adi]);
                }
            });
        }
        isPen.o = o;
        isPen.grid.setGridOption('rowData', []);
        isPen.$p[0].showModal();
        gridYukle(isPen.grid, Sgb.al(UC + 'is', { yil: parseInt(o.yil, 10) || 0, muh: bos(o.muh), muhBir: bos(o.muhBir) }),
            function (v) { isPen.grid.setGridOption('rowData', v.satirlar || []); }, 'Tanımlı iş bulunamadı.');
    }

    // ================================================================ 4. kesinti hesabı (eski KesintiHesapla2.aspx + KesintiHesapla2.js)

    var kesinti = null; // { $p, o, kdvOkundu }

    // Eski sayfadaki sıra: KDV listesinin ilk üçü, 10, dördüncüsü, 20, beşincisi (listede olmayan ya da -1 olan gizlenir).
    var KDV_SIRA = [0, 1, 2, '10', 3, '20', 4];
    var VERGI_BINDE = [{ d: '0', e: '0' }, { d: '5.69', e: '5,69' }, { d: '7.59', e: '7,59' }, { d: '9.48', e: '9,48' }, { d: '15.17', e: '15,17' }, { d: '18.96', e: '18,96' }];

    function radyo(ad, deger, etiket, kimlik, isaretli) {
        return $('<div class="radio"></div>').append($('<label></label>').attr('for', kimlik)
            .append($('<input type="radio" />').attr({ name: ad, id: kimlik, value: deger }).prop('checked', !!isaretli))
            .append(document.createTextNode(' ' + etiket)));
    }

    function kesintiKur() {
        var $ust = $('<div class="oem-bolum oem-bolum-kriter oem-kesinti-tutar"></div>')
            .append(satir('Borç Toplamı', metinKutu('oem-borc', 'oemKsBorc'), 'oemKsBorc'))
            .append(satir('Avans Toplamı', metinKutu('oem-avans', 'oemKsAvans'), 'oemKsAvans'));

        var $kdv = $('<fieldset class="oem-kesinti-kolon oem-kdv"><legend>KDV Yüzde</legend></fieldset>');
        $.each(KDV_SIRA, function (i, s) {
            radyo('oemKdv', '', '', 'oemKsKdv' + i, false).attr('data-sira', s).appendTo($kdv);
        });

        var $vergi = $('<fieldset class="oem-kesinti-kolon"><legend>Vergi Kesintisi Binde</legend></fieldset>');
        $.each(VERGI_BINDE, function (i, v) { radyo('oemVergi', v.d, v.e, 'oemKsVergi' + i, i === 0).appendTo($vergi); });
        radyo('oemVergi', 'damga', 'Girilen', 'oemKsVergiGir', false).appendTo($vergi);
        $vergi.append($('<input type="number" step="any" class="form-control input-sm oem-girilen oem-damga" aria-label="Vergi kesintisi binde (girilen)" />').attr('data-radyo', 'oemKsVergiGir'));

        var $tevkifat = $('<fieldset class="oem-kesinti-kolon"><legend>KDV Tevkifatı</legend></fieldset>');
        radyo('oemVergi', 'tevkifat', 'Girilen', 'oemKsTevkifat', false).appendTo($tevkifat);
        $tevkifat.append($('<input type="text" class="form-control input-sm oem-girilen oem-tevkifat" autocomplete="off" aria-label="KDV tevkifatı (girilen)" />').attr('data-radyo', 'oemKsTevkifat'))
            .append('<p class="help-block small">Örnek: 2/3</p>');

        var $gelir = $('<fieldset class="oem-kesinti-kolon"><legend>Gelir Vergisi Tevkifatı %</legend></fieldset>');
        radyo('oemVergi', 'gelir', 'Girilen', 'oemKsGelir', false).appendTo($gelir);
        $gelir.append($('<input type="number" step="0.001" class="form-control input-sm oem-girilen oem-gelir" aria-label="Gelir vergisi tevkifatı yüzde (girilen)" />').attr('data-radyo', 'oemKsGelir'))
            .append('<p class="help-block small">Örnek: 10</p>');

        var $govde = $('<div></div>').append($ust)
            .append($('<div class="oem-bolum oem-bolum-liste oem-kesinti-oranlar"></div>').append($kdv, $vergi, $tevkifat, $gelir));
        var $hesapla = $('<button type="button" class="btn btn-primary btn-sm oem-hesapla"><span class="glyphicon glyphicon-ok" aria-hidden="true"></span> Hesapla ve Yaz</button>');
        kesinti = { $p: pencereKur('oem-kesinti', 'Kesinti Hesapla', $govde, $hesapla), o: {}, kdvOkundu: false };

        // Eski Isaretle: girilen kutusuna yazılınca kendi seçeneği işaretlenir.
        kesinti.$p.on('input keydown', '.oem-girilen', function () { $('#' + $(this).attr('data-radyo')).prop('checked', true); });
        $hesapla.on('click', kesintiYaz);
        kdvYaz([0, 1, 8, 18, 26]);
    }

    function kdvYaz(oranlar) {
        var ilk = true;
        kesinti.$p.find('.oem-kdv .radio').each(function () {
            var $r = $(this), s = $r.attr('data-sira');
            var oran = s === '10' || s === '20' ? parseInt(s, 10) : (parseInt(s, 10) < oranlar.length ? oranlar[parseInt(s, 10)] : -1);
            var gizli = oran === -1 || oran === undefined || oran === null || isNaN(oran);
            $r.prop('hidden', gizli);
            var $i = $r.find('input').val(gizli ? '' : String(oran)).prop('checked', false);
            $r.find('label').contents().filter(function () { return this.nodeType === 3; }).remove();
            $r.find('label').append(document.createTextNode(' ' + (gizli ? '' : String(oran))));
            if (!gizli && ilk) { $i.prop('checked', true); ilk = false; }
        });
    }

    // Metin tutarı sayıya çevirir (eski: ilk üç nokta silinir, ilk üç virgül noktaya çevrilir).
    function tutarOku(s) {
        s = bos(s);
        for (var j = 0; j < 3; j++) s = s.replace(/\./, '');
        for (var k = 0; k < 3; k++) s = s.replace(/,/, '.');
        return s;
    }

    function tutarMetin(v) {
        // Eski: sorgu dizgisindeki tutarın noktası virgüle çevrilip kutuya yazılırdı.
        if (v === undefined || v === null || v === '') return '';
        return String(v).replace('.', ',');
    }

    // Eski KesintiYaz (ModulScripts/KesintiHesapla2.js) aynen
    function kesintiYaz() {
        var $p = kesinti.$p;
        var netTutar = 0, vergiTutari = 0;
        var borc = tutarOku($p.find('.oem-borc').val());
        var avans = tutarOku($p.find('.oem-avans').val());

        if (!(borc > 0)) {
            Sgb.bildirim('Borç toplamında hesaplanacak bilgi olmadığı için hesaplama yapılamadı.', 'warning');
            return;
        }
        borc = Number(borc);
        avans = avans > 0 ? Number(avans) : 0;

        var kdv = Number($p.find('input[name=oemKdv]:checked').val()) || 0;
        var $v = $p.find('input[name=oemVergi]:checked');
        var secim = $v.val();
        var vergi = Number(secim) || 0;
        var tevkifat = '';
        var gelir = 0;
        if (secim === 'damga') {
            vergi = Number($p.find('.oem-damga').val()) || 0;
        } else if (secim === 'tevkifat') {
            vergi = 0;
            tevkifat = $.trim($p.find('.oem-tevkifat').val());
        } else if (secim === 'gelir') {
            gelir = Number($p.find('.oem-gelir').val()) || 0;
            vergi = gelir;
        }

        if (kdv === 0) netTutar = borc - avans;
        else netTutar = (borc - avans) / (1 + (kdv / 100));

        if (tevkifat !== '') {
            var yer = tevkifat.indexOf('/');
            var r1 = tevkifat.substr(0, yer);
            var r2 = tevkifat.substr(yer + 1, tevkifat.length - yer - 1);
            netTutar = (borc - avans) - netTutar;
            vergiTutari = (netTutar * Number(r1)) / Number(r2);
        } else {
            var oran = gelir > 0 ? 100 : 1000;
            if (vergi > 0) {
                vergiTutari = netTutar * (vergi / oran);
                var vergiArtik = Math.round((vergiTutari - Math.floor(vergiTutari)) * 100);
                vergiTutari = Math.floor(vergiTutari);
                vergiTutari = vergiTutari + (vergiArtik / 100);
            } else {
                vergiTutari = (borc - avans) * (kdv / 100);
            }
        }

        if (!isFinite(vergiTutari)) { Sgb.bildirim('Kesinti hesaplanamadı; girilen oranı denetleyiniz.', 'warning'); return; }
        sonucVer($p, kesinti.o.secilince, [vergiTutari, kesinti.o.satir]);
    }

    function kesintiHesapla(o) {
        o = o || {};
        if (!kesinti) kesintiKur();
        kesinti.o = o;
        var $p = kesinti.$p;
        $p.find('.oem-borc').val(tutarMetin(o.borc));
        $p.find('.oem-avans').val(tutarMetin(o.avans));
        $p.find('.oem-girilen').val('');
        $p.find('input[name=oemVergi]').first().prop('checked', true);
        $p[0].showModal();
        if (!kesinti.kdvOkundu) {
            Sgb.al(UC + 'kdv').done(function (v) {
                if (v.hata || !v.oranlar) return;
                kesinti.kdvOkundu = true;
                kdvYaz(v.oranlar);
            }).fail(function (xhr) { hataGoster(xhr, 'KDV oranları okunamadı.'); });
        } else {
            $p.find('.oem-kdv .radio:not([hidden]) input').first().prop('checked', true);
        }
    }

    // ================================================================ 5. taahhüt kartı seçimi (eski TaahhutKartiListe.aspx)

    var taahhut = null; // { $p, grid, o }

    function taahhutSec(o) {
        o = o || {};
        if (!taahhut) {
            var $govde = $('<div></div>')
                .append('<p class="help-block small oem-yardim">Seçmek için proje sıra numarasına tıklayınız ya da satıra çift tıklayınız.</p>')
                .append('<div class="oem-grid oem-taahhut-grid" aria-label="Sözleşme ve taahhüt kartları"></div>');
            taahhut = { $p: pencereKur('oem-taahhut', 'Sözleşme ve Taahhüt Kartı', $govde), o: o };
            var sec = function (d) { sonucVer(taahhut.$p, taahhut.o.secilince, [d.KOD]); };
            taahhut.grid = Sgb.grid(taahhut.$p.find('.oem-taahhut-grid')[0], {
                columnDefs: [
                    { headerName: '#', width: 55, sortable: false, type: 'rightAligned', valueGetter: function (p) { return p.node.rowIndex + 1; } },
                    { field: 'DURUMADI', headerName: 'Durumu', width: 90 },
                    { field: 'YIL', headerName: 'Yıl', width: 65 },
                    {
                        field: 'PROJESIRANO', headerName: 'Proje Sıra No.', width: 120,
                        cellRenderer: function (p) {
                            return $('<button type="button" class="btn btn-link btn-xs oem-bag"></button>').text(bos(p.value) || '(seç)')
                                .on('click', function () { sec(p.data); })[0];
                        }
                    },
                    { field: 'MUHASEBEAD', headerName: 'Muhasebe', width: 140, hide: true },
                    { field: 'BIRIMAD', headerName: 'Birim', width: 180, tooltipField: 'BIRIMAD' },
                    { field: 'KONU', headerName: 'Konu', flex: 1, minWidth: 200, tooltipField: 'KONU' },
                    { field: 'MUTEAHHITKIMLIKNO', headerName: 'Müteahhid Kimlik/Vergi No', width: 130 },
                    { field: 'MUTEAHHITUNVAN', headerName: 'Müteahhid Ad, Soyad/Ünvan', width: 200, tooltipField: 'MUTEAHHITUNVAN' },
                    { field: 'SOZLESMETARIHI', headerName: 'Sözleşme Tarihi', width: 115, valueFormatter: function (x) { return Sgb.tarih(x.value); } },
                    { field: 'IHALEBEDELI', headerName: 'İhale Bedeli', width: 120, type: 'rightAligned', valueFormatter: tutar },
                    { field: 'ODEMEEMRIYILNO', headerName: 'Tahakkuk Belge No', width: 130 }
                ],
                onRowDoubleClicked: function (e) { sec(e.data); }
            });
        }
        taahhut.o = o;
        taahhut.grid.setGridOption('rowData', []);
        taahhut.$p[0].showModal();
        gridYukle(taahhut.grid, Sgb.al(UC + 'taahhut', { yil: parseInt(o.yil, 10) || 0, vergiNo: bos(o.vergiNo) }),
            function (v) { taahhut.grid.setGridOption('rowData', v.satirlar || []); }, 'Bu ilgiliye ait devam eden taahhüt kartı bulunamadı.');
    }

    // ================================================================ 6. kapatma listesi (eski ListeKapatma.aspx)

    var kapatma = null; // { $p, kapatilan, kapatan }

    function kapatmaSutunlari(kapatan) {
        var s = [
            { headerName: '#', width: 55, sortable: false, type: 'rightAligned', valueGetter: function (p) { return p.node.rowIndex + 1; } },
            { field: 'YIL', headerName: 'Yıl', width: 65 }
        ];
        if (kapatan) s.push({ field: 'BELGENO', headerName: 'BelgeNo', width: 95 });
        s.push({ field: 'KYIL', headerName: 'K. Yıl', width: 70 });
        if (!kapatan) s.push({ field: 'KBELGENO', headerName: 'K. BelgeNo', width: 95 });
        return s.concat([
            { field: 'KHESAPKOD', headerName: 'K. HesapKod', width: 120 },
            { field: 'KILGILIVNO', headerName: 'K. İlgili', width: 110 },
            { field: 'KTERTIP', headerName: 'K. Tertip', flex: 1, minWidth: 150, tooltipField: 'KTERTIP' },
            { field: 'KTUTAR', headerName: 'K. Tutar', width: 115, type: 'rightAligned', valueFormatter: tutar },
            { field: 'SIRANO', headerName: 'Sıra No', width: 75, type: 'rightAligned' },
            { field: 'BORCALACAK', headerName: 'Borç Alacak', width: 95 },
            { field: 'KBIRIMKOD', headerName: 'K. Birim', width: 85 }
        ]);
    }

    function kapatmaListesi(o) {
        o = o || {};
        if (!bos(o.belgeNo)) return;   // eski: belge no yoksa açılmaz
        if (!kapatma) {
            var $govde = $('<div></div>')
                .append($('<section class="oem-bolum oem-bolum-kriter"></section>').append('<h3 class="oem-bolum-baslik">Kapatılan</h3>')
                    .append('<div class="oem-grid oem-kapatma-grid oem-kapatilan" aria-label="Kapatılan"></div>'))
                .append($('<section class="oem-bolum oem-bolum-liste"></section>').append('<h3 class="oem-bolum-baslik">Kapatan</h3>')
                    .append('<div class="oem-grid oem-kapatma-grid oem-kapatan" aria-label="Kapatan"></div>'));
            kapatma = { $p: pencereKur('oem-kapatma', 'Kapatma Listesi', $govde) };
            kapatma.kapatilan = Sgb.grid(kapatma.$p.find('.oem-kapatilan')[0], { columnDefs: kapatmaSutunlari(false), overlayNoRowsTemplate: '<span>Kayıt yok.</span>' });
            kapatma.kapatan = Sgb.grid(kapatma.$p.find('.oem-kapatan')[0], { columnDefs: kapatmaSutunlari(true), overlayNoRowsTemplate: '<span>Kayıt yok.</span>' });
        }
        baslikYaz(kapatma.$p, 'Kapatma Listesi: ' + bos(o.yil) + ' / ' + bos(o.belgeNo));
        kapatma.kapatilan.setGridOption('rowData', []);
        kapatma.kapatan.setGridOption('rowData', []);
        kapatma.$p[0].showModal();
        kapatma.kapatan.setGridOption('loading', true);
        gridYukle(kapatma.kapatilan, Sgb.al(UC + 'kapatma', { yil: parseInt(o.yil, 10) || 0, belgeNo: bos(o.belgeNo) }), function (v) {
            kapatma.kapatilan.setGridOption('rowData', v.kapatilan || []);
            kapatma.kapatan.setGridOption('rowData', v.kapatan || []);
        }, 'Kayıt yok.').always(function () { kapatma.kapatan.setGridOption('loading', false); });
    }

    // ================================================================ 7. belge tarihçesi (eski BelgeTarihce.aspx)

    var tarihcePen = null; // { $p, grid }

    function tarihce(o) {
        o = o || {};
        if (!tarihcePen) {
            var $govde = $('<div></div>')
                .append('<p class="oem-belge"><strong>Belge No:</strong> <span class="oem-belge-no"></span></p>')
                .append('<div class="oem-grid oem-tarihce-grid" aria-label="Belge tarihçesi"></div>');
            tarihcePen = { $p: pencereKur('oem-tarihce', 'Tarihçe', $govde) };
            tarihcePen.grid = Sgb.grid(tarihcePen.$p.find('.oem-tarihce-grid')[0], {
                columnDefs: [
                    { field: 'sira', headerName: 'Sıra', minWidth: 70, flex: 70, type: 'rightAligned' },
                    { field: 'islemTarih', headerName: 'İşlem Tarih', minWidth: 170, flex: 170 },
                    { field: 'islem', headerName: 'İşlem', minWidth: 220, flex: 220, tooltipField: 'islem' },
                    { field: 'islemYapan', headerName: 'İşlemi Yapan', minWidth: 130, flex: 130 },
                    { field: 'islemYapanAd', headerName: 'İşlem Yapan Ad', minWidth: 180, flex: 180 }
                ],
                overlayNoRowsTemplate: '<span>Tarihçe kaydı yok.</span>'
            });
        }
        var $p = tarihcePen.$p;
        $p.find('.oem-belge-no').text('');
        tarihcePen.grid.setGridOption('rowData', []);
        $p[0].showModal();
        if (!(parseInt(o.yil, 10) > 0) || !bos(o.belgeNo)) { Sgb.bildirim('Yıl ve/ya Belge No bilgisi alınamadı.', 'warning'); return; }
        gridYukle(tarihcePen.grid, Sgb.al(UC + 'tarihce', { yil: o.yil, belgeNo: o.belgeNo }), function (v) {
            $p.find('.oem-belge-no').text(v.belge || '');
            tarihcePen.grid.setGridOption('rowData', v.satirlar || []);
        }, 'Tarihçe kaydı yok.');
    }

    // ================================================================ 8. alt hesap bazında muavin defteri (eski MuavinDefteriAltHesapBazindaRapor.aspx)

    // Eski OdemeEmriAc: ödeme emri belgesi ayrı pencerede. Mümkünse en üstteki sayfanın penceresi kullanılır.
    function odemeEmriAc(yil, belgeNo) {
        if (!bos(belgeNo)) return;
        var ust = Sgb;
        try {
            if (window.top !== window && window.top.Sgb && window.top.Sgb.pencere) ust = window.top.Sgb;
        } catch (e) { /* başka kökenden açılmış */ }
        ust.pencere('../ButceMuhasebe/OdemeEmriMIFFrame?' + $.param({ aramaYok: 1, yil: yil, belgeNo: belgeNo }), 'Ödeme Emri ' + yil + '/' + belgeNo);
    }

    function muavinGrid(el) {
        return Sgb.grid(el, {
            columnDefs: [
                { field: 'YEVMIYETARIH', headerName: 'Belge Tarihi', width: 105, valueFormatter: function (x) { return Sgb.tarih(x.value); } },
                {
                    field: 'ODEMEEMRIKOD', headerName: 'Belge No', width: 100,
                    cellRenderer: function (p) {
                        if (!bos(p.value)) return '';
                        return $('<button type="button" class="btn btn-link btn-xs oem-bag"></button>').text(p.value).attr('title', 'Ödeme emrini aç')
                            .on('click', function () { odemeEmriAc(p.data.ODEMEEMRIYIL, p.data.ODEMEEMRIKOD); })[0];
                    }
                },
                { field: 'KODLAR', headerName: 'Tertip', width: 190, tooltipField: 'KODLAR' },
                { field: 'ACIKLAMA', headerName: 'Açıklama', flex: 1, minWidth: 160, tooltipField: 'ACIKLAMA' },
                { field: 'BORC', headerName: 'Borç', width: 115, type: 'rightAligned', valueFormatter: tutar },
                { field: 'ALACAK', headerName: 'Alacak', width: 115, type: 'rightAligned', valueFormatter: tutar },
                { field: 'BAKIYE', headerName: 'Bakiye', width: 140, type: 'rightAligned', sortable: false }
            ],
            overlayNoRowsTemplate: '<span>Hareket bulunamadı.</span>'
        });
    }

    function muavinOku(grid, o, $baslik) {
        grid.setGridOption('rowData', []);
        return gridYukle(grid, Sgb.al(UC + 'muavin', {
            yil: parseInt(o.yil, 10) || 0, hesapKod: bos(o.hesapKod), altHesapKod: bos(o.altHesapKod), projeNo: bos(o.projeNo)
        }), function (v) {
            if ($baslik) $baslik.text((v.hesapKod || '') + (v.altHesapKod ? ' / ' + v.altHesapKod : '') + ((parseInt(o.yil, 10) || 0) > 0 ? ' (' + o.yil + ')' : ''));
            grid.setGridOption('rowData', v.satirlar || []);
        }, 'Hareket bulunamadı.');
    }

    var muavin = null; // { $p, grid }

    function altHesapMuavin(o) {
        o = o || {};
        if (!muavin) {
            var $govde = $('<div></div>')
                .append('<p class="oem-belge"><strong>Hesap:</strong> <span class="oem-muavin-hesap"></span></p>')
                .append('<div class="oem-grid oem-muavin-grid" aria-label="Muavin defteri"></div>');
            muavin = { $p: pencereKur('oem-muavin', 'Muavin Defteri Listesi', $govde) };
            muavin.grid = muavinGrid(muavin.$p.find('.oem-muavin-grid')[0]);
        }
        muavin.$p.find('.oem-muavin-hesap').text('');
        muavin.$p[0].showModal();
        muavinOku(muavin.grid, o, muavin.$p.find('.oem-muavin-hesap'));
    }

    // Razor sayfası (ButceMuhasebe/MuavinDefteriAltHesapBazindaRapor): eski adres alacaktakip.js'den pencere olarak açılır.
    $(function () {
        var $s = $('#oemMuavinSayfa');
        if (!$s.length) return;
        var q = new URLSearchParams(window.location.search);
        var grid = muavinGrid($s.find('.oem-muavin-grid')[0]);
        muavinOku(grid, { yil: q.get('yil'), hesapKod: q.get('hesapKod'), altHesapKod: q.get('altHesapKod'), projeNo: q.get('projeNo') },
            $s.find('.oem-muavin-hesap'));
        $s.find('.oem-sayfa-kapat').on('click', function () {
            // Sgb.pencere içinde açıldıysa üstteki pencere kapatılır.
            try {
                var d = window.frameElement && window.frameElement.closest('dialog');
                if (d) { d.close(); return; }
            } catch (e) { /* başka köken */ }
            window.history.back();
        });
    });

    Sgb.oem = {
        mahsupBelgeSec: mahsupBelgeSec,
        surecFaturaSec: surecFaturaSec,
        isSec: isSec,
        kesintiHesapla: kesintiHesapla,
        taahhutSec: taahhutSec,
        kapatmaListesi: kapatmaListesi,
        tarihce: tarihce,
        altHesapMuavin: altHesapMuavin
    };
})(window.Sgb, jQuery);
