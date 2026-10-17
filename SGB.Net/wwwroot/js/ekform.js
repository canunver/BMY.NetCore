// Ek Formlar (eski ButceHazirlikv2/EkFormAna.aspx). Eski sayfa soldaki listeden seçilen alt formu (ButceForm/*.aspx) bir çerçevede
// açıyor, üstteki Kaydet / Yazdır / Detaydan Al tuşları çerçevedeki gizli düğmelere tıklıyordu. Burada çerçeve yoktur: formlar
// aynı sayfada çizilir ve eski gizli düğmelerin yaptığı iş kuralı çağrılarını /api/ekform/{form}/... uçlarından yapar.
//
// Her form wwwroot/js/ekform/*.js içinde EkForm.tanimla(ad, modul) ile kaydolur. modul:
//   ac(ctx)          : formu ctx.$govde içine çizer (tablolar, alanlar). Ardından sayfa listele(ctx)'i çağırır.
//   listele(ctx)     : verileri okur (söz döndürür).          kaydet(ctx): saklar (söz döndürür; başarılıysa değişiklik işareti kalkar).
//   yazdir(ctx)      : raporu indirir (genellikle EkForm.indir ile).
//   dugmeler(ctx)    : ek tuşlar [{ ad, ikon, sinif, ipucu, tikla(ctx) }] (ör. Detaydan Al).
//   kapat(ctx)       : başka forma geçilirken (tablolar yok edilir).   kayitYok: true ise Kaydet tuşu gösterilmez.
// ctx: { $govde, secim: { yil, senaryo, kur, kurAd }, p: listedeki seçeneğin parametreleri, degisti: bool }
(function ($) {
    'use strict';

    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });
    var ondalik = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 0, maximumFractionDigits: 3 });
    var SAKLA = 'ekform.secim';

    // Eski EkFormAna.FormlariListele sırası; adlar eski kaynak dosyasından (BHMEFA...). yilEk: adın sonuna yazılacak yıl farkı.
    var FORMLAR = [
        { id: 1, no: '6', ad: 'İlk defa yapılması planlanan hizmetler', form: 'hizmetBilgisi', p: { ekyil: 0, tamam: 2 }, yilEk: 0 },
        { id: 2, no: '6', ad: 'İlk defa yapılması planlanan hizmetler', form: 'hizmetBilgisi', p: { ekyil: 1, tamam: 2 }, yilEk: 1 },
        { id: 3, no: '6', ad: 'İlk defa yapılması planlanan hizmetler', form: 'hizmetBilgisi', p: { ekyil: 2, tamam: 2 }, yilEk: 2 },
        { id: 4, no: '7', ad: 'Tamamlanması planlanan hizmetler', form: 'hizmetBilgisi', p: { ekyil: -1, tamam: 1 }, yilEk: -1 },
        { id: 5, no: '7', ad: 'Tamamlanması planlanan hizmetler', form: 'hizmetBilgisi', p: { ekyil: 0, tamam: 1 }, yilEk: 0 },
        { id: 6, no: '7', ad: 'Tamamlanması planlanan hizmetler', form: 'hizmetBilgisi', p: { ekyil: 1, tamam: 1 }, yilEk: 1 },
        { id: 7, no: '10', ad: 'Hizmet maliyeti tespit formu', form: 'hizmetMaliyeti', p: {} },
        { id: 8, no: '11', ad: 'Fiziksel değerler bilgi tablosu', form: 'fizikselDegerler', p: {} },
        { id: 9, no: '17', ad: 'Uluslararası kuruluşlar bilgi formu', form: 'uluslararasi', p: {} },
        { id: 10, no: '18', ad: 'Edinilecek taşıtlar', form: 'edinilecekTasit', p: {} },
        { id: 11, no: '19.1', ad: 'Kullanılan taşıtlara ilişkin bilgi formu', form: 'kullanilanTasit', p: {} },
        { id: 12, no: '19.3', ad: 'Hizmet alımı suretiyle kullanılan taşıtlar', form: 'hizmetAlimiTasit', p: { kullanilacak: 0 } },
        { id: 13, no: '19.2', ad: 'Hizmet alımı suretiyle kullanılacak taşıtlar', form: 'hizmetAlimiTasit', p: { kullanilacak: 1 } },
        { id: 14, no: '21', ad: 'Özel güvenlik hizmetine dair bilgi formu', form: 'ozelGuvenlik', p: {} },
        { id: 15, no: '22', ad: 'Temizlik hizmetine ilişkin bilgi formu', form: 'temizlik', p: {} },
        { id: 16, no: '23', ad: 'Döner sermaye gelir-gider cetveli', form: 'donerSermaye', p: { fon: false } },
        { id: 17, no: '24', ad: 'Fon gelir-gider cetveli', form: 'donerSermaye', p: { fon: true } },
        { id: 18, no: '25', ad: 'Tavanı aşan ilave ödenek teklifleri', form: 'tavanUstu', p: { detay: false } },
        { id: 19, no: '25.2', ad: 'Tavanı aşan ilave ödenek teklifleri (detay)', form: 'tavanUstu', p: { detay: true } },
        { id: 20, no: '28', ad: 'Hizmet alımı suretiyle çalıştırılan görevli bilgileri', form: 'hizmetGorevli', p: {} }
    ];

    var moduller = {};
    var tanim = null;
    var aktif = null;   // { tanim (FORMLAR satırı), modul, ctx }
    var mesgul = false;

    // ------------------------------------------------------------------ yardımcılar (formlar da kullanır)

    // "1.234.000" ya da "1234000,5" -> sayı. Boş -> null. Geçersizse undefined.
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '');
        if (s === '') return null;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        else if (/^-?\d{1,3}(\.\d{3})+$/.test(s)) s = s.replace(/\./g, '');
        var n = Number(s);
        return isFinite(n) ? n : undefined;
    }

    function para(p) { return p.value == null || p.value === '' ? '' : tamSayi.format(p.value); }
    function sayi(p) { return p.value == null || p.value === '' ? '' : ondalik.format(p.value); }

    // gg.aa.yyyy denetimi; boş geçerli. Geçerliyse sıfırlarla tamamlanmış metni, değilse undefined döndürür.
    function tarihCoz(metin) {
        var s = String(metin == null ? '' : metin).trim();
        if (s === '') return '';
        var m = /^(\d{1,2})[./-](\d{1,2})[./-](\d{4})$/.exec(s);
        if (!m) return undefined;
        var g = +m[1], a = +m[2], y = +m[3];
        var d = new Date(y, a - 1, g);
        if (d.getFullYear() !== y || d.getMonth() !== a - 1 || d.getDate() !== g) return undefined;
        return (g < 10 ? '0' : '') + g + '.' + (a < 10 ? '0' : '') + a + '.' + y;
    }

    function hucreSinif(girilirMi) {
        return function (p) {
            if (p.node && p.node.rowPinned) return 'kapali';
            var gir = typeof girilirMi === 'function' ? girilirMi(p) : girilirMi !== false;
            if (!gir) return 'kapali';
            return p.data && p.data.dg && p.data.dg[p.colDef.field] ? 'girilir degisti' : 'girilir';
        };
    }

    function birlestir(sinif, ek) { return function (p) { return (ek ? ek + ' ' : '') + sinif(p); }; }

    // Sütun kalıpları. o: { girilir (bool | fn(p)), width, flex, ... } AG Grid sütun özellikleri de verilebilir.
    function sayiSutun(field, baslik, o) {
        o = o || {};
        var gir = o.girilir === undefined ? true : o.girilir;
        var c = $.extend({
            field: field, headerName: baslik, width: 110, headerClass: 'sayi',
            valueFormatter: o.ondalik ? sayi : para,
            cellClass: birlestir(hucreSinif(gir), 'sayi'),
            editable: function (p) { return !p.node.rowPinned && (typeof gir === 'function' ? gir(p) : gir !== false); },
            cellEditor: 'agTextCellEditor',
            valueParser: function (p) { var n = sayiCoz(p.newValue); return n === undefined ? p.oldValue : n; }
        }, o);
        delete c.girilir; delete c.ondalik;
        return c;
    }

    function metinSutun(field, baslik, o) {
        o = o || {};
        var gir = o.girilir === undefined ? true : o.girilir;
        var c = $.extend({
            field: field, headerName: baslik, width: 180, tooltipField: field,
            cellClass: birlestir(hucreSinif(gir), 'metin'),
            editable: function (p) { return !p.node.rowPinned && (typeof gir === 'function' ? gir(p) : gir !== false); },
            cellEditor: o.uzun ? 'agLargeTextCellEditor' : 'agTextCellEditor',
            cellEditorPopup: !!o.uzun,
            cellEditorParams: o.uzun ? { maxLength: 4000, rows: 6, cols: 60 } : undefined,
            valueParser: function (p) { return p.newValue == null ? '' : String(p.newValue); }
        }, o);
        delete c.girilir; delete c.uzun;
        return c;
    }

    function tarihSutun(field, baslik, o) {
        return metinSutun(field, baslik, $.extend({
            width: 105, tooltipField: undefined,
            valueParser: function (p) {
                var t = tarihCoz(p.newValue);
                if (t === undefined) { Sgb.bildirim('Tarihi gg.aa.yyyy biçiminde yazınız.', 'warning'); return p.oldValue; }
                return t;
            }
        }, o || {}));
    }

    // Seçim listesinden değer seçilen sütun. secenekler: [{ kod, ad }] ya da onu döndüren fonksiyon.
    function secimSutun(field, baslik, secenekler, o) {
        function liste() { return (typeof secenekler === 'function' ? secenekler() : secenekler) || []; }
        function ad(kod) {
            var s = $.grep(liste(), function (x) { return String(x.kod) === String(kod); })[0];
            return s ? s.ad : (kod == null ? '' : String(kod));
        }
        return metinSutun(field, baslik, $.extend({
            width: 200,
            cellEditor: 'agSelectCellEditor',
            cellEditorParams: function () { return { values: $.map(liste(), function (x) { return x.kod; }) }; },
            valueFormatter: function (p) { return p.value == null || p.value === '' ? '' : ad(p.value); },
            tooltipValueGetter: function (p) { return p.value == null || p.value === '' ? '' : ad(p.value); },
            valueParser: function (p) { return p.newValue; },
            cellEditorPopup: false
        }, o || {}));
    }

    // Girilebilir tablo: tek tıkla düzenleme, Enter alta geçer; değişen hücre işaretlenir ve sayfa "değişti" sayılır.
    function grid(ctx, element, ozellikler) {
        var kendi = ozellikler.onCellValueChanged;
        var g = Sgb.grid(element, $.extend({
            suppressCellFocus: false,
            singleClickEdit: true,
            stopEditingWhenCellsLoseFocus: true,
            enterNavigatesVertically: true,
            enterNavigatesVerticallyAfterEdit: true,
            tooltipShowDelay: 500,
            // Başlıklar sütun genişliğine sığmazsa alt satıra geçer, başlık satırı metne göre yükselir.
            defaultColDef: { sortable: false, filter: false, resizable: true, wrapHeaderText: true, autoHeaderHeight: true },
            defaultColGroupDef: { wrapHeaderText: true, autoHeaderHeight: true },
            getRowStyle: undefined
        }, ozellikler, {
            onCellValueChanged: function (e) {
                if (e.newValue === e.oldValue || e.node.rowPinned) return;
                e.data.dg = e.data.dg || {};
                e.data.dg[e.colDef.field] = true;
                degisti(ctx);
                e.api.refreshCells({ rowNodes: [e.node], force: true });
                if (kendi) kendi(e);
            }
        }));
        ctx.tablolar = ctx.tablolar || [];
        ctx.tablolar.push(g);
        return g;
    }

    function satirlar(g) {
        var l = [];
        g.forEachNode(function (n) { if (!n.rowPinned) l.push(n.data); });
        return l;
    }

    // Kaydetten sonra değişti işaretleri temizlenir (düzenleme bitince gelen olay geç gelebilir, sonraya bırakılır).
    function isaretleriTemizle(ctx) {
        setTimeout(function () {
            $.each(ctx.tablolar || [], function (i, g) {
                if (g.isDestroyed && g.isDestroyed()) return;
                g.forEachNode(function (n) { if (n.data) n.data.dg = null; });
                g.refreshCells({ force: true });
            });
            ctx.degisti = false;
        }, 0);
    }

    function degisti(ctx) { ctx.degisti = true; }

    function duzenlemeBitir(ctx) {
        $.each(ctx.tablolar || [], function (i, g) { if (!(g.isDestroyed && g.isDestroyed())) g.stopEditing(); });
    }

    // Hata ayıklama: istek başarısızsa (403 / bağlantı) bildirim; { hata } döndüyse bildirim ve reddedilmiş söz.
    function sonuc(soz, varsayilan) {
        var d = $.Deferred();
        soz.done(function (v) {
            if (v && v.hata) { Sgb.bildirim(v.hata, 'danger'); d.reject(v); return; }
            d.resolve(v);
        }).fail(function (xhr) {
            if (xhr.status === 403) Sgb.bildirim('Bu işlem için yetkiniz yok.', 'danger');
            else if (xhr.status !== 401) Sgb.bildirim(varsayilan || 'İşlem yapılamadı.', 'danger');
            d.reject(xhr);
        });
        return d.promise();
    }

    function al(adres, parametreler, varsayilan) { return sonuc(Sgb.al(adres, parametreler), varsayilan || 'Bilgiler okunamadı.'); }

    // Kayıt: başarılıysa bildirim gösterilir ve değişiklik işaretleri temizlenir.
    function kaydetGonder(ctx, adres, veri) {
        return sonuc(Sgb.gonder(adres, veri), 'Kayıt yapılamadı.').done(function (v) {
            Sgb.bildirim(v.bilgi || 'Kayıt işlemi başarıyla gerçekleşti.', 'success');
            isaretleriTemizle(ctx);
        });
    }

    function indir(adres, parametreler) { return Sgb.indir(adres, parametreler); }

    // Onay penceresi; Tamam'a basılırsa söz tamamlanır.
    function onay(metin, dugme) {
        var d = $.Deferred();
        var dlg = document.getElementById('dlgOnay');
        $('#onayMetin').text(metin);
        $('#btnOnayEvet').text(dugme || 'Tamam').off('click').on('click', function () { dlg.close('evet'); });
        $(dlg).off('close.onay').one('close.onay', function () { if (dlg.returnValue === 'evet') d.resolve(); else d.reject(); });
        dlg.returnValue = '';
        dlg.showModal();
        return d.promise();
    }

    // Seçim bilgisinin okunur hâli (eski FormBaslikOl: bütçe yılı, birim).
    function secimMetni(s) { return 'Bütçe yılı: ' + s.yil + ' · Birim: ' + s.kur + (s.kurAd ? ' - ' + s.kurAd : ''); }

    // ------------------------------------------------------------------ sayfa

    function saklananOku() {
        try { return JSON.parse(localStorage.getItem(SAKLA) || '{}') || {}; } catch (e) { return {}; }
    }

    function sakla() {
        try {
            localStorage.setItem(SAKLA, JSON.stringify({
                yil: $('#ddlYil').val(), senaryo: $('#ddlSenaryo').val(), kur: $('#ddlBirim').val(), form: aktif ? aktif.tanim.id : null
            }));
        } catch (e) { /* tarayıcı saklamaya izin vermiyor */ }
    }

    function secim() {
        var $b = $('#ddlBirim option:selected');
        return {
            yil: parseInt($('#ddlYil').val(), 10) || 0,
            senaryo: $('#ddlSenaryo').val() || '',
            kur: $('#ddlBirim').val() || '',
            kurAd: $b.length ? ($b.data('ad') || '') : ''
        };
    }

    function formAdi(f, yil) {
        var ad = 'Form ' + f.no + ' - ' + f.ad;
        if (f.yilEk !== undefined && yil) ad += ' (' + (yil + f.yilEk) + ')';
        return ad;
    }

    function listeyiCiz() {
        var yil = secim().yil;
        var $l = $('#lstFormlar').empty();
        $.each(FORMLAR, function (i, f) {
            var $o = $('<button type="button" class="ekform-oge" role="option"></button>').attr('data-id', f.id)
                .attr('aria-selected', aktif && aktif.tanim.id === f.id ? 'true' : 'false')
                .toggleClass('secili', !!(aktif && aktif.tanim.id === f.id))
                .toggleClass('hazir-degil', !moduller[f.form]);
            $('<span class="form-no"></span>').text(f.no).appendTo($o);
            $('<span class="form-ad"></span>').text(formAdi(f, yil).replace(/^Form [^-]+ - /, '')).appendTo($o);
            $o.appendTo($l);
        });
    }

    function devamEdilsinMi() {
        if (!aktif || !aktif.ctx.degisti) return $.Deferred().resolve().promise();
        return onay('Kaydedilmemiş değişiklikler var. Kaydetmeden devam edilsin mi?', 'Devam et');
    }

    function dugmeleriAyarla() {
        var m = aktif && aktif.modul;
        $('#btnListele').prop('disabled', !m || mesgul);
        $('#btnKaydet').prop('hidden', !!(m && m.kayitYok)).prop('disabled', !m || !m.kaydet || mesgul);
        $('#btnYazdir').prop('hidden', !!(m && !m.yazdir)).prop('disabled', !m || !m.yazdir || mesgul);
        $('#ekDugmeler .btn').prop('disabled', mesgul);
    }

    function islem(fn) {
        if (mesgul || !aktif) return;
        duzenlemeBitir(aktif.ctx);
        var s = fn(aktif.ctx);
        if (!s || !s.then) return;
        var r = $.when(s);   // jQuery ve tarayıcı sözleri (Sgb.indir) aynı biçimde beklenir
        mesgul = true;
        dugmeleriAyarla();
        r.always(function () { mesgul = false; dugmeleriAyarla(); });
    }

    function formuKapat() {
        if (!aktif) return;
        if (aktif.modul.kapat) { try { aktif.modul.kapat(aktif.ctx); } catch (e) { /* yok */ } }
        $.each(aktif.ctx.tablolar || [], function (i, g) { try { g.destroy(); } catch (e) { /* yok */ } });
        aktif = null;
    }

    function formuAc(f) {
        formuKapat();
        var m = moduller[f.form];
        var $g = $('#formGovde').empty();
        var s = secim();
        $('#formBaslik').text(formAdi(f, s.yil));
        $('#formAltBaslik').text(secimMetni(s));
        $('#ekDugmeler').empty();
        if (!m) {
            $('<p class="text-muted bos-metin"></p>').text('Bu form henüz taşınmadı.').appendTo($g);
            aktif = null;
            dugmeleriAyarla();
            listeyiCiz();
            return;
        }
        var ctx = { $govde: $g, secim: s, p: $.extend({}, f.p), tanim: f, degisti: false, tablolar: [] };
        aktif = { tanim: f, modul: m, ctx: ctx };
        listeyiCiz();
        sakla();
        m.ac(ctx);
        var ek = typeof m.dugmeler === 'function' ? m.dugmeler(ctx) : (m.dugmeler || []);
        $.each(ek, function (i, d) {
            $('<button type="button" class="btn btn-sm"></button>').addClass(d.sinif || 'btn-default').attr('title', d.ipucu || '')
                .append($('<span class="glyphicon" aria-hidden="true"></span>').addClass('glyphicon-' + (d.ikon || 'flash')))
                .append(document.createTextNode(' ' + d.ad))
                .on('click', function () { islem(d.tikla); })
                .appendTo('#ekDugmeler');
        });
        dugmeleriAyarla();
        listele();
    }

    function listele() {
        islem(function (ctx) {
            if (!ctx.secim.yil || !ctx.secim.kur) { Sgb.bildirim('Yıl ve birim seçiniz.', 'warning'); return null; }
            var r = aktif.modul.listele(ctx);
            if (r && r.done) r.done(function () { ctx.degisti = false; });
            return r;
        });
    }

    function secimDegisti() {
        var onceki = aktif;
        devamEdilsinMi().done(function () {
            sakla();
            if (onceki) {
                if (onceki.ctx) onceki.ctx.degisti = false;
                formuAc(onceki.tanim);
            }
            else listeyiCiz();
        }).fail(function () {
            // Vazgeçildi: seçim eski değerine döner.
            var s = onceki.ctx.secim;
            $('#ddlYil').val(String(s.yil)); $('#ddlSenaryo').val(s.senaryo); $('#ddlBirim').val(s.kur);
        });
    }

    function doldur($s, liste, secili) {
        $s.empty();
        $.each(liste, function (i, x) { $('<option></option>').val(x.kod).text(x.ad).data('ad', x.adSade || x.ad).appendTo($s); });
        if (secili != null && $s.find('option').filter(function () { return this.value === String(secili); }).length) $s.val(String(secili));
    }

    function basla() {
        Sgb.al('api/ekform/tanim').done(function (t) {
            if (t.hata) { Sgb.bildirim(t.hata, 'danger'); return; }
            tanim = t;
            var sak = saklananOku();
            Sgb.yilKutusu('#ddlYil', t.yillar, sak.yil || t.yil);
            doldur($('#ddlSenaryo'), t.senaryolar || [], sak.senaryo || t.senaryo);
            doldur($('#ddlBirim'), $.map(t.birimler || [], function (b) { return { kod: b.kod, ad: b.kod + ' - ' + b.ad, adSade: b.ad }; }), sak.kur || t.kurum);
            var ilk = $.grep(FORMLAR, function (f) { return f.id === sak.form; })[0] || FORMLAR[0];
            formuAc(ilk);
        }).fail(function (xhr) {
            if (xhr.status === 403) Sgb.bildirim('Bu sayfayı kullanma yetkiniz yok.', 'danger');
            else if (xhr.status !== 401) Sgb.bildirim('Sayfa bilgileri okunamadı.', 'danger');
        });

        $('#lstFormlar').on('click', '.ekform-oge', function () {
            var id = parseInt($(this).attr('data-id'), 10);
            if (aktif && aktif.tanim.id === id) return;
            var f = $.grep(FORMLAR, function (x) { return x.id === id; })[0];
            devamEdilsinMi().done(function () { formuAc(f); });
        });
        $('#ddlYil, #ddlSenaryo, #ddlBirim').on('change', secimDegisti);
        $('#btnListele').on('click', function () {
            devamEdilsinMi().done(listele);
        });
        $('#btnKaydet').on('click', function () { islem(function (ctx) { return aktif.modul.kaydet(ctx); }); });
        $('#btnYazdir').on('click', function () { islem(function (ctx) { return aktif.modul.yazdir(ctx); }); });
        $(document).on('click', '[data-kapat]', function () { this.closest('dialog').close(); });
        window.addEventListener('beforeunload', function (e) {
            if (aktif && aktif.ctx.degisti) { e.preventDefault(); e.returnValue = ''; }
        });
    }

    window.EkForm = {
        tanimla: function (ad, modul) { moduller[ad] = modul; },
        basla: basla,
        sayiCoz: sayiCoz, tarihCoz: tarihCoz, para: para, sayi: sayi,
        sayiSutun: sayiSutun, metinSutun: metinSutun, tarihSutun: tarihSutun, secimSutun: secimSutun, hucreSinif: hucreSinif,
        grid: grid, satirlar: satirlar, degisti: degisti, isaretleriTemizle: isaretleriTemizle,
        al: al, kaydet: kaydetGonder, sonuc: sonuc, indir: indir, onay: onay, secimMetni: secimMetni,
        // Satır listesinin sonuna n boş satır ekler (eski tablolardaki boş giriş satırları).
        bosSatirEkle: function (liste, n, sablon) {
            for (var i = 0; i < n; i++) liste.push($.extend(true, {}, sablon || {}));
            return liste;
        }
    };

    $(function () { if ($('.ek-form').length) basla(); });
})(jQuery);
