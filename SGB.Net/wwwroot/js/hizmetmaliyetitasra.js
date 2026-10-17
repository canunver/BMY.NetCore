// Birimlerin Hizmet Maliyetinin Tespitine İlişkin Bilgi Formu, taşra (ButceForm/HizmetMaliyetiTespitiTasra): seçilen yıl ve
// muhasebe / birim için hizmet maliyeti tutarları. Kod hiç girilmez; tutarlar yalnızca degerGirilsinMi olan satırlarda girilir
// (diğerleri sarı başlık satırlarıdır); açıklama yalnızca "6" ile başlayan diğer gider satırlarında yazılır. Sütun başlıklarının
// üstünde yıllar (yıl-2 .. yıl+2) yazar. Yazdır seçili birimin Excel raporunu verir; sistem yöneticisi seçili birimin kurumsal kodunun toplu raporunu,
// formu girmeyenlerin listesini alır ve formu bütün birimler için veri girişine kapatır / açar (Onayla / Onay Kaldır).
$(function () {
    'use strict';

    if (!$('#grdListe').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TUTARLAR = [
        { alan: 'gercek', ad: 'Gerçekleşme', fark: -2 },
        { alan: 'haziran', ad: 'Haziran', fark: -1 },
        { alan: 'tahmin', ad: 'Tahmin', fark: 0 },
        { alan: 'tahmin1', ad: 'Tahmin', fark: 1 },
        { alan: 'tahmin2', ad: 'Tahmin', fark: 2 }
    ];

    var tanim = { yil: new Date().getFullYear() + 1, sistemYoneticisi: false, kurumlar: [] };
    var acik = null;          // listelenmiş form { yil, muhasebe, birim, kurumsal }: kaydet bunu kullanır
    var degisti = false;      // listelendikten sonra tabloda değişiklik yapıldı mı
    var sayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }

    // "1.234" ya da "1234" -> 1234 (tam sayı). Boş -> null. Geçersizse undefined (eski değer kalır).
    function sayiCoz(metin) {
        if (typeof metin === 'number') return Math.round(metin);
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return null;
        var n = Number(s);
        return isFinite(n) ? Math.round(n) : undefined;
    }

    // ---------------------------------------------------------------- tablo

    function digerSatir(d) { return !!d && d.degerGirilsinMi && (d.kod === '' || String(d.kod).charAt(0) === '6'); }
    function tutarGirilir(p) { return !!acik && !!p.data && p.data.degerGirilsinMi; }
    function aciklamaGirilir(p) { return !!acik && digerSatir(p.data); }

    function hucreSinif(girilir, ek) {
        return function (p) {
            var s = ek ? [ek] : [];
            if (p.data && !p.data.degerGirilsinMi) s.push('kapali');
            else if (girilir(p)) s.push('girilir');
            return s;
        };
    }

    function sutunlar(yil) {
        return [
            { field: 'kod', headerName: 'Kod', width: 80, pinned: 'left', cellClass: hucreSinif(function () { return false; }) },
            { field: 'aciklama', headerName: 'Açıklama', width: 420, pinned: 'left', tooltipField: 'aciklama',
                editable: aciklamaGirilir, cellClass: hucreSinif(aciklamaGirilir) }
        ].concat($.map(TUTARLAR, function (t) {
            return {
                headerName: String(yil + t.fark), headerClass: 'sayi', suppressStickyLabel: true,
                children: [{
                    field: t.alan, headerName: t.ad, width: 130, type: 'sayi', headerClass: 'sayi',
                    editable: tutarGirilir, cellClass: hucreSinif(tutarGirilir, 'sayi'), cellEditor: 'agTextCellEditor',
                    valueFormatter: function (p) { return p.value == null || p.value === '' || !p.data.degerGirilsinMi ? '' : sayi.format(p.value); },
                    valueParser: function (p) {
                        var n = sayiCoz(p.newValue);
                        if (n === undefined) { mesaj('Geçerli bir sayı yazınız.', 'warning'); return p.oldValue; }
                        return n;
                    }
                }]
            };
        }));
    }

    var BOS_IPUCU = '<span>Yılı ve muhasebe / birimi seçip Listele\'ye basınız.</span>';

    var grid = Sgb.grid('#grdListe', {
        columnDefs: sutunlar(tanim.yil),
        defaultColDef: { sortable: false, filter: false, resizable: true, wrapHeaderText: true, autoHeaderHeight: true },
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        tooltipShowDelay: 500,
        overlayNoRowsTemplate: BOS_IPUCU,
        onCellValueChanged: function (e) {
            if (e.newValue === e.oldValue) return;
            degisti = true;
        }
    });

    function satirlar() {
        var l = [];
        grid.forEachNode(function (n) { if (n.data) l.push(n.data); });
        return l;
    }

    function tuslariAyarla() {
        $('#btnKaydet, #btnSatirEkle').prop('disabled', !acik);
    }

    function listeyiBosalt() {
        acik = null;
        degisti = false;
        grid.setGridOption('rowData', []);
        grid.setGridOption('overlayNoRowsTemplate', BOS_IPUCU);
        tuslariAyarla();
    }

    function secimOku() {
        var d = muhBirim.deger();
        return { yil: yilOku(), muhasebe: d.muhasebe, birim: d.birim, kurumsal: d.kurumsal };
    }

    // Listele (eski btnListele_Click). sessiz: kayıttan sonra yenilerken açık formun seçimiyle okunur.
    function listele(sessiz) {
        var o = sessiz && acik ? $.extend({}, acik) : secimOku();
        if (o.yil <= 0 || o.muhasebe <= 0 || !o.birim) {
            mesaj('Yıl, Kurum veya Birim alanlarından biri veya bir kaçı boş bırakılmış. Lütfen ilgili alanları kontrol edip tekrar deneyin.', 'warning');
            return;
        }
        var $d = $('#btnListele').prop('disabled', true);
        grid.stopEditing(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/hizmetmaliyetitasra/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = o;
                degisti = false;
                grid.setGridOption('columnDefs', sutunlar(o.yil));
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                grid.setGridOption('rowData', $.map(v.satirlar || [], function (s) {
                    var r = { kod: s.kod, aciklama: s.aciklama, degerGirilsinMi: !!s.degerGirilsinMi };
                    $.each(TUTARLAR, function (i, t) { r[t.alan] = s[t.alan] ? s[t.alan] : null; });
                    return r;
                }));
                tuslariAyarla();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#btnListele').on('click', function () {
        if (degisti && !window.confirm('Kaydedilmemiş değişiklikler var. Yeniden listelerseniz kaybolacak. Devam edilsin mi?')) return;
        listele(false);
    });

    // Yıl ya da muhasebe / birim değişince listelenmiş form kullanılmaz (kayıt listelenen birime yazılır).
    function secimDegisiyor() {
        if (!acik) return true;
        if (degisti && !window.confirm('Kaydedilmemiş değişiklikler var. Seçimi değiştirirseniz kaybolacak. Devam edilsin mi?')) return false;
        listeyiBosalt();
        return true;
    }

    $('#txtYil').on('focus', function () { $(this).data('onceki', $(this).val()); });
    $('#txtYil').on('change', function () {
        if (!secimDegisiyor()) { $(this).val($(this).data('onceki')); return; }
        $(this).data('onceki', $(this).val());
        var yil = yilOku();
        if (yil > 0) grid.setGridOption('columnDefs', sutunlar(yil));
    });

    // Muhasebe - birim - kurumsal kod (ortak bileşen, muhbirim.js; eski txtMuhasebe / txtBirim ve ListeMuhasebeBirim penceresi).
    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: yilOku,
        oncesi: secimDegisiyor
    });

    // Diğer giderler için boş satır (eski tablonun sonundaki 20 boş satır yerine).
    $('#btnSatirEkle').on('click', function () {
        if (!acik) return;
        grid.stopEditing();
        var sonuc = grid.applyTransaction({ add: [{ kod: '', aciklama: '', degerGirilsinMi: true }] });
        degisti = true;
        if (sonuc && sonuc.add && sonuc.add.length) {
            grid.ensureNodeVisible(sonuc.add[0], 'bottom');
            grid.startEditingCell({ rowIndex: sonuc.add[0].rowIndex, colKey: 'aciklama' });
        }
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

    // ---------------------------------------------------------------- kaydet (eski btnKaydet_Click)

    $('#btnKaydet').on('click', function () {
        if (!acik) return;
        grid.stopEditing();
        sor('Onay', 'Veriler kaydedilecek, onaylıyor musunuz?', 'Kaydet', function () {
            var gonderilen = $.map(satirlar(), function (r) {
                if (!r.degerGirilsinMi) return null;
                return { kod: r.kod || '', aciklama: r.aciklama || '', degerGirilsinMi: true,
                    gercek: r.gercek, haziran: r.haziran, tahmin: r.tahmin, tahmin1: r.tahmin1, tahmin2: r.tahmin2 };
            });
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder('api/hizmetmaliyetitasra/kaydet', $.extend({}, acik, { satirlar: gonderilen }))
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) { degisti = false; listele(true); }
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', !acik); });
        });
    });

    // ---------------------------------------------------------------- raporlar (eski btnYazdir_Click, btnYazdirToplu_Click, btnGirmeyenler_Click)

    $('#btnYazdir').on('click', function () {
        var o = secimOku();
        if (o.yil <= 0) { mesaj('Yıl alanı boş bırakılamaz.', 'warning'); return; }
        if (o.muhasebe <= 0 || !o.birim) { mesaj('Muhasebe ve birim bilgisi boş bırakılamaz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/hizmetmaliyetitasra/yazdir', { yil: o.yil, muhasebe: o.muhasebe, birim: o.birim })
            .then(function () { $d.prop('disabled', false); });
    });

    $('#btnYazdirToplu').on('click', function () {
        var yil = yilOku();
        if (yil <= 0) { mesaj('Yıl alanı boş bırakılamaz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        // Kurumsal kod Muhasebe Kodu alanındaki birimin kurumsal kodudur; muhasebe boşsa sunucu bütün kurumun raporunu verir.
        var kurumsal = muhBirim.deger().muhasebe > 0 ? muhBirim.deger().kurumsal : '';
        Sgb.indir('api/hizmetmaliyetitasra/yazdir', { yil: yil, kurumsal: kurumsal, toplu: true })
            .then(function () { $d.prop('disabled', false); });
    });

    $('#btnGirmeyenler').on('click', function () {
        var yil = yilOku();
        if (yil <= 0) { mesaj('Yıl alanı boş bırakılamaz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/hizmetmaliyetitasra/girmeyenler', { yil: yil }).then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- onay (eski btnOnayla_Click / btnOnayKaldir_Click)

    function onay(onayla) {
        var yil = yilOku();
        if (yil <= 0) { mesaj('Yıl alanı boş bırakılamaz.', 'warning'); return; }
        var metin = onayla
            ? yil + ' yılı formu bütün birimler için veri girişine kapatılacak. Devam edilsin mi?'
            : yil + ' yılı formu yeniden veri girişine açılacak. Devam edilsin mi?';
        sor(onayla ? 'Onay' : 'Onay Kaldır', metin, onayla ? 'Onayla' : 'Onay Kaldır', function () {
            Sgb.gonder('api/hizmetmaliyetitasra/onay', { yil: yil, onayla: onayla })
                .done(function (s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); })
                .fail(hataGoster);
        });
    }

    $('#btnOnayla').on('click', function () { onay(true); });
    $('#btnOnayKaldir').on('click', function () { onay(false); });

    window.addEventListener('beforeunload', function (e) {
        if (degisti) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    tuslariAyarla();
    Sgb.al('api/hizmetmaliyetitasra/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            $('#txtYil').val(t.yil).data('onceki', String(t.yil));
            muhBirim.ata({ muhasebe: t.muhasebe, birim: t.birim, kurumsal: t.kurumsal, birimAd: t.birimAd });
            $('.yonetici').prop('hidden', !t.sistemYoneticisi);
            grid.setGridOption('columnDefs', sutunlar(t.yil));
        })
        .fail(hataGoster);
});
