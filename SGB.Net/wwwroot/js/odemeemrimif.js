// Ödeme Emri / MİF belgesi (ButceMuhasebe/OdemeEmriMIF2; eski aynı adlı Ext.NET sayfası + ModulScripts/OdemeEmri2.js). Ödeme Emri /
// MİF çerçevesinin "Belge Düzenleme" sekmesidir: OdemeEmriMIF2?yil=&belgeNo=&aramaYok=&baslikSayfasi= . Uçlar: /api/odemeemrimif/*
// (Muhasebe/OdemeEmriMIFUclari.cs). Seçim pencereleri Sgb.oem (js/odemeemrisecim.js), tertip Sgb.tertipSec, hesap planı ve kişi / firma
// Sgb.hesapPlaniSec / Sgb.firmaSec (js/makbuzsecim.js), muhasebe - birim Sgb.muhasebeBirim. Çerçeve, Belge Arama ya da Muavin Defteri'nden
// bir belge açılınca window.OemBelgeYukle(yil, belgeNo) çağrılır.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/odemeemrimif/';
    var HESAP_UC = UC + 'hesap/';
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var adres = new URLSearchParams(window.location.search);
    var sayfa = {
        yil: parseInt(adres.get('yil'), 10) || 0,
        belgeNo: $.trim(adres.get('belgeNo') || ''),
        aramaYok: adres.get('aramaYok') === '1',
        baslik: adres.get('baslikSayfasi') === '1'
    };
    var tanim = null;               // /tanim yanıtı
    var belge = { durum: 0, belgeNo: '' };
    var oybs = { secili: '', kilitli: false };
    var satirNo = 0;
    var mukerrerOnay = false;       // eski hdnMukerrerBelgeKontrol
    var KUR_TURLERI = [['', ''], ['ALIS', 'Alış'], ['SATIS', 'Satış'], ['EALIS', 'Efektif Alış'], ['ESATIS', 'Efektif Satış'], ['SALIS', 'Serbest Alış'], ['SSATIS', 'Serbest Satış']];

    // ---------------------------------------------------------------- ortak yardımcılar

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function paraYaz(v) { return para.format(Number(v) || 0); }
    function sayiCoz(s) {
        if (typeof s === 'number') return s;
        s = $.trim(String(s == null ? '' : s)).replace(/\s/g, '');
        if (s === '') return null;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = parseFloat(s);
        return isNaN(n) ? null : n;
    }
    function yuvarla(n) { return Math.round((Number(n) || 0) * 100) / 100; }
    function yil() { return parseInt($('#txtYil').val(), 10) || new Date().getFullYear(); }
    function belgeNo() { return $.trim($('#txtBelgeNo').val()); }
    function kodAyir(s) { return $.trim(String(s || '').split('-')[0]); }
    function yukleniyor($t, evet) { $t.prop('disabled', !!evet).toggleClass('oem-bekle', !!evet); }

    // Bir işlemin JSON yanıtını işler: hata / başarı iletisi; başarıda bitince(v) çağrılır.
    function sonucIsle(v, bitince) {
        if (!v) return;
        if (v.hata && v.basarili !== true) { mesaj(v.hata, 'danger'); return; }
        if (v.basarili === false) { mesaj(v.hata || 'İşlem yapılamadı.', 'danger'); return; }
        if (v.uyari) mesaj(v.uyari, 'warning');
        if (v.bilgi) mesaj(v.bilgi, 'success');
        if (bitince) bitince(v);
    }

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null, soruHayir = null;
    function sor(metin, is, o) {
        o = o || {};
        $('#soruMetin').text(metin);
        $('#soruUyari').text(o.uyari || '').prop('hidden', !o.uyari);
        var $b = $('#soruBelgeler').empty().prop('hidden', !(o.belgeler && o.belgeler.length));
        $.each(o.belgeler || [], function (i, no) {
            $('<a href="#"></a>').text(no).on('click', function (e) {
                e.preventDefault();
                Sgb.pencere('../ButceMuhasebe/OdemeEmriMIFFrame?' + $.param({ aramaYok: 1, yil: o.yil || yil(), belgeNo: no }), 'Ödeme Emri / MİF ' + no);
            }).appendTo($b);
        });
        $('#dlgSoruBaslik').text(o.baslik || 'Onay');
        $('#btnSoruEvet').text(o.evet || 'Evet');
        $('#btnSoruHayir').text(o.hayir || 'Vazgeç');
        soruIsi = is;
        soruHayir = o.hayirIse || null;
        dlgSoru.showModal();
    }
    $('#btnSoruEvet').on('click', function () {
        var is = soruIsi;
        soruIsi = null; soruHayir = null;
        dlgSoru.close();
        if (is) is();
    });
    $(dlgSoru).on('close', function () {
        var h = soruHayir;
        soruHayir = null; soruIsi = null;
        if (h) h();
    });
    $(document).on('click', 'dialog [data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // Açılır menüler (Bootstrap JS yüklenmiyor)
    $('.oem-arac').on('click', '.dropdown-toggle', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var $g = $(this).parent(), acik = $g.hasClass('open');
        $('.oem-arac .btn-group.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false');
        if (!acik) $g.addClass('open').children('.dropdown-toggle').attr('aria-expanded', 'true');
        if (!acik && $g.is('#grpYazdirTercih')) imzaListesiOku();
    });
    $(document).on('click', function (e) {
        if ($(e.target).closest('.oem-imza-satir').length) return;
        $('.oem-arac .btn-group.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false');
        $('#mnuBakiye').prop('hidden', true);
    });

    $('#btnYardim').on('click', function () {
        var $y = $('#oemYardim'), ac = $y.prop('hidden');
        $y.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false');
        gridYay();
    });

    // ---------------------------------------------------------------- sekmeler

    $('.oem-sekmeler').on('click', 'a', function (e) {
        e.preventDefault();
        var ad = $(this).attr('data-sekme');
        $('.oem-sekmeler li').each(function () { $(this).toggleClass('active', $(this).find('a').attr('data-sekme') === ad); });
        $('.oem-sekme').each(function () { $(this).prop('hidden', $(this).attr('data-sekme') !== ad); });
        if (ad === 'oybs' && oybsGrid) setTimeout(function () { oybsGrid.sizeColumnsToFit(); }, 0);
        if (ad === 'dokuman' && dokumanGrid) setTimeout(function () { dokumanGrid.sizeColumnsToFit(); }, 0);
    });

    // ---------------------------------------------------------------- muhasebe - birim

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: yil,
        degisince: function () {
            detayBirimleriOku();
            // eski DetayBirimDegistir: üst birim değişince detay birimler temizlensin mi?
            var var_ = false;
            grid.forEachNode(function (n) { if (n.data.DETAYBIRIM) var_ = true; });
            if (var_) sor('Üst birim değiştirildiği için Detay birimler temizlensin mi?', function () {
                grid.forEachNode(function (n) { n.data.DETAYBIRIM = ''; });
                grid.refreshCells({ force: true });
            }, { baslik: 'Uyarı' });
        }
    });

    var detayBirimler = [];
    function detayBirimleriOku() {
        if (!tanim || !tanim.detayBirim) return;
        var m = muhBirim.deger().muhasebe;
        if (!m) { detayBirimler = []; return; }
        Sgb.al(UC + 'detay-birimler', { muhasebe: m }).done(function (v) { detayBirimler = v.birimler || []; });
    }

    // ---------------------------------------------------------------- hesap satırları

    function bosSatir() {
        return {
            _id: ++satirNo, SIRANO: 0, KESINTI: 0, HESAPKOD: '', HESAPAD: '', TERTIP: '', TERTIPPRG: '', BORC: 0, ALACAK: 0, ALTHESAPKOD: '',
            SATIRACIKLAMA: '', DOVIZ: '', DOVIZKURTUR: '', DOVIZBORC: 0, DOVIZALACAK: 0, DOVIZKUR: 0, MAHSUPBELGENO: '', DETAYBIRIM: '', ISKOD: 0, ISAD: ''
        };
    }

    function hucreTus(simge, baslik, ne) {
        return $('<button type="button" class="btn btn-link btn-xs oem-mercek"></button>')
            .attr('title', baslik).append($('<span class="glyphicon" aria-hidden="true"></span>').addClass(simge))
            .append($('<span class="sr-only"></span>').text(baslik))
            .on('click', function (e) { e.stopPropagation(); grid.stopEditing(); ne(); });
    }

    // Hücre metni + tuşlar (eski ImageCommand'lar)
    function tusluHucre(tuslar) {
        return function (p) {
            var $h = $('<span class="oem-hucre-sec"></span>');
            $('<span class="oem-hucre-metin"></span>').text(p.valueFormatted != null ? p.valueFormatted : (p.value == null ? '' : p.value)).appendTo($h);
            $.each(tuslar, function (i, t) { $h.append(hucreTus(t[0], t[1], function () { t[2](p.node); })); });
            return $h[0];
        };
    }

    // Yazdıkça arama düzenleyicisi (eski ComboBox MinChars=2): en az iki karakter yazılınca eşleşen kayıtlar hücrenin altında listelenir;
    // ok tuşlarıyla gezilir, Enter / tıklama ile seçilir, Esc listeyi kapatır. ara(metin) -> jQuery promise [{deger, metin}].
    var oneriAcik = false;
    function OneriDuzenleyici() { }
    OneriDuzenleyici.prototype.init = function (p) {
        var self = this;
        self.p = p;
        self.$in = $('<input type="text" class="ag-input-field-input oem-oneri-girdi" autocomplete="off" />');
        self.$in.val(p.eventKey && p.eventKey.length === 1 ? p.eventKey : (p.value == null ? '' : p.value));
        self.$liste = $('<div class="oem-oneri-liste" role="listbox" hidden></div>').appendTo(document.body);
        self.secenekler = []; self.isaret = -1; self.istek = 0; self.zaman = null;
        self.$in.on('input', function () {
            clearTimeout(self.zaman);
            self.zaman = setTimeout(function () { self.ara(); }, 300);
        });
        self.$in.on('keydown', function (e) {
            if (!oneriAcik) return;
            if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
                e.preventDefault(); e.stopPropagation();
                var n = self.secenekler.length;
                self.isaretle(e.key === 'ArrowDown' ? (self.isaret + 1) % n : (self.isaret - 1 + n) % n);
            } else if (e.key === 'Enter') {
                if (self.isaret >= 0) { e.preventDefault(); e.stopPropagation(); self.sec(self.isaret); }
                else self.kapat();
            } else if (e.key === 'Escape') {
                e.preventDefault(); e.stopPropagation(); self.kapat();
            }
        });
        self.$in.on('blur', function () { self.kapat(); });
        self.$liste.on('mousedown', '.oem-oneri-satir', function (e) { e.preventDefault(); self.sec($(this).index()); });
        if (p.eventKey && p.eventKey.length === 1) self.zaman = setTimeout(function () { self.ara(); }, 300);
    };
    OneriDuzenleyici.prototype.ara = function () {
        var self = this, metin = $.trim(self.$in.val());
        if (metin.length < 2) { self.kapat(); return; }
        var no = ++self.istek;
        self.p.ara(metin).done(function (liste) {
            if (no !== self.istek || !self.$in.closest('body').length) return;
            self.secenekler = liste || [];
            self.$liste.empty();
            if (!self.secenekler.length) { self.kapat(); return; }
            $.each(self.secenekler, function (i, s) {
                $('<div class="oem-oneri-satir" role="option"></div>').text(s.metin).attr('title', s.metin).appendTo(self.$liste);
            });
            var r = self.$in[0].getBoundingClientRect();
            var alt = window.innerHeight - r.bottom;
            self.$liste.css({ left: r.left, minWidth: Math.max(r.width, 360), top: alt > 220 ? r.bottom : '', bottom: alt > 220 ? '' : window.innerHeight - r.top })
                .prop('hidden', false);
            oneriAcik = true;
            self.isaretle(-1);
        });
    };
    OneriDuzenleyici.prototype.isaretle = function (i) {
        this.isaret = i;
        var $s = this.$liste.children().removeClass('secili');
        if (i >= 0) { var $x = $s.eq(i).addClass('secili'); $x[0].scrollIntoView({ block: 'nearest' }); }
    };
    OneriDuzenleyici.prototype.sec = function (i) {
        var s = this.secenekler[i];
        if (!s) return;
        this.$in.val(s.deger);
        this.kapat();
        this.p.stopEditing();
    };
    OneriDuzenleyici.prototype.kapat = function () {
        this.istek++;
        this.$liste.prop('hidden', true).empty();
        this.secenekler = []; this.isaret = -1;
        oneriAcik = false;
    };
    OneriDuzenleyici.prototype.getGui = function () { return this.$in[0]; };
    OneriDuzenleyici.prototype.afterGuiAttached = function () {
        var el = this.$in[0];
        el.focus();
        if (!(this.p.eventKey && this.p.eventKey.length === 1)) el.select();
        else el.setSelectionRange(el.value.length, el.value.length);
    };
    OneriDuzenleyici.prototype.getValue = function () { return this.$in.val(); };
    OneriDuzenleyici.prototype.destroy = function () { clearTimeout(this.zaman); this.kapat(); this.$liste.remove(); };
    // Liste açıkken Enter / oklar tabloya gitmez, listeye gider.
    function oneriTusu(p) {
        return oneriAcik && p.editing && (p.event.key === 'Enter' || p.event.key === 'ArrowDown' || p.event.key === 'ArrowUp' || p.event.key === 'Escape');
    }

    function hesapOneri(metin) {
        return Sgb.al(HESAP_UC + 'ara', { yil: yil(), metin: metin, sonKirilim: '1' }).then(function (v) {
            return $.map(v.satirlar || [], function (h) { return { deger: h.kod + '-' + (h.hesapAd || ''), metin: h.ad }; }).slice(0, 50);
        }, function () { return $.Deferred().resolve([]).promise(); });
    }
    function altHesapOneri(metin) {
        return Sgb.al(UC + 'althesap/ara', { muhasebe: muhBirim.deger().muhasebe || 0, metin: metin }).then(function (v) {
            return $.map(v.satirlar || [], function (f) { return { deger: f.vergiNo + '-' + f.ad, metin: f.vergiNo + ' - ' + f.ad }; }).slice(0, 50);
        }, function () { return $.Deferred().resolve([]).promise(); });
    }

    function paraSutun(alan, ad, ek) {
        return $.extend({
            field: alan, headerName: ad, width: 115, type: 'rightAligned', editable: true, sortable: false,
            cellClass: ['oem-girilir', 'ag-right-aligned-cell'],
            valueFormatter: function (p) { return p.node && p.node.rowPinned ? paraYaz(p.value || 0) : (p.value ? paraYaz(p.value) : ''); },
            valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? 0 : yuvarla(n); }
        }, ek || {});
    }

    // eski KodNoktaKoy: koda yapıdaki noktaları yerleştirir (ör. yapı 99.99.99.99 → 13.99.01.01)
    function kodNoktala(yapi, kod) {
        if (!yapi) return kod;
        var r = '', k = 0;
        for (var i = 0; i < yapi.length && k < kod.length; i++) r += yapi[i] === '.' ? '.' : kod[k++];
        return r.replace(/\.+$/, '') + kod.substring(k);
    }

    function dovizAd(kisaltma) {
        var d = $.grep((tanim && tanim.dovizler) || [], function (x) { return x.kisaltma === kisaltma; })[0];
        return d ? d.adi : (kisaltma || '');
    }
    function kurTurAd(kod) {
        var k = $.grep(KUR_TURLERI, function (x) { return x[0] === kod; })[0];
        return k ? k[1] : (kod || '');
    }

    // Hesap Kodu / Alt Hesap Kodu sütun başlığındaki menü (üç çizgi) süzmesi (yalnız tarayıcıda, satırlar silinmez, gizlenir).
    // Rakam ve nokta yazılırsa kodu o yazıyla başlayanlar; "100-103" yazılırsa kodu 100 ile 103 arasında başlayanlar;
    // başka bir şey yazılırsa (ör. vakıf) içinde o yazı geçenler kalır. Toplam satırı süzülmez.
    function kodSuz(p) {
        var deger = p.value || '', f = $.trim(p.filterText || '');
        if (!f) return true;
        var kod = $.trim(deger.split('-')[0]).replace(/\./g, '');
        var ar = /^([\d.]+)\s*-\s*([\d.]+)$/.exec(f);
        if (ar) {
            var bas = ar[1].replace(/\./g, ''), son = ar[2].replace(/\./g, '');
            return kod.substring(0, bas.length) >= bas && kod.substring(0, son.length) <= son;
        }
        if (/^[\d.]+$/.test(f)) return kod.indexOf(f.replace(/\./g, '')) === 0;
        return deger.indexOf(f) >= 0;
    }
    function kodSuzmeAyari(ipucu) {
        return {
            filterOptions: ['contains'], maxNumConditions: 1, debounceMs: 250, filterPlaceholder: ipucu,
            textFormatter: function (x) { return x == null ? null : String(x).toLocaleLowerCase('tr-TR'); },
            textMatcher: kodSuz
        };
    }

    var sutunlar = [
        { headerName: '#', valueGetter: function (p) { return p.node.rowPinned ? '' : p.node.rowIndex + 1; }, width: 48, sortable: false, cellClass: 'oem-sari', pinned: 'left' },
        {
            field: 'KESINTI', headerName: 'Kes.', headerTooltip: 'Kesinti', width: 44, sortable: false, editable: true, cellClass: 'oem-girilir',
            cellDataType: false, cellRenderer: function (p) {
                var $k = $('<input type="checkbox" aria-label="Kesinti" />').prop('checked', !!Number(p.value) || p.value === true);
                $k.on('change', function () { p.node.setDataValue('KESINTI', this.checked ? 1 : 0); });
                return $k[0];
            }, cellEditor: null, suppressKeyboardEvent: function () { return false; }
        },
        {
            field: 'HESAPKOD', headerName: 'Hesap Kodu', width: 290, editable: true, sortable: false, cellClass: 'oem-girilir',
            cellRenderer: tusluHucre([['glyphicon-search', 'Hesap kodu seç', hesapPlaniAc]]),
            cellEditor: OneriDuzenleyici, cellEditorParams: { ara: hesapOneri }, suppressKeyboardEvent: oneriTusu,
            filter: 'agTextColumnFilter', filterParams: kodSuzmeAyari('Süz: 103 ya da 100-103')
        },
        {
            field: 'DETAYBIRIM', colId: 'DETAYBIRIM', headerName: 'Birim', width: 160, editable: true, sortable: false, cellClass: 'oem-girilir', hide: true,
            cellEditor: 'agSelectCellEditor',
            cellEditorParams: function () { return { values: $.map(detayBirimler, function (b) { return b.kod; }) }; },
            valueFormatter: function (p) {
                // Değer: 3 haneli birim + "." + kurumsal kod (birim seçiminden, ör. 005.12345678) ya da kayıtlı belgede yalnız birim.
                // Kurumsal kod değerde yoksa satırın tertibinden (ilk parça, KURKOD) alınır; kurum kod yapısına göre noktalı gösterilir.
                var deger = String(p.value || ''), nokta = deger.indexOf('.');
                var birim = nokta >= 0 ? deger.substring(0, nokta) : deger;
                if (!birim) return '';
                var kur = nokta >= 0 ? deger.substring(nokta + 1).replace(/\./g, '').trim() : '';
                if (!kur && p.data) kur = String(p.data.TERTIP || '').split('-')[0].replace(/\./g, '').trim();
                var b = $.grep(detayBirimler, function (x) { return x.kod === deger; })[0] ||
                    $.grep(detayBirimler, function (x) { return x.kod.split('.')[0] === birim; })[0];
                return birim + (kur ? '.' + kodNoktala((tanim && tanim.kurumsalYapi) || '', kur) : '') + (b && b.ad ? ' - ' + b.ad : '');
            }
        },
        {
            field: 'TERTIP', colId: 'TERTIP', headerName: 'Bütçe Tertibi', width: 270, editable: true, sortable: false, cellClass: 'oem-girilir',
            cellRenderer: tusluHucre([['glyphicon-search', 'Tertip seç', tertipAc]]), cellEditor: 'agTextCellEditor'
        },
        { field: 'SATIRACIKLAMA', colId: 'SATIRACIKLAMA', headerName: 'Açıklama', width: 140, editable: true, sortable: false, cellClass: 'oem-girilir', hide: true },
        {
            field: 'ALTHESAPKOD', colId: 'ALTHESAPKOD', headerName: 'Alt Hesap Kodu', width: 210, editable: true, sortable: false, cellClass: 'oem-girilir',
            cellRenderer: tusluHucre([['glyphicon-search', 'Firma seç', firmaAc]]),
            cellEditor: OneriDuzenleyici, cellEditorParams: { ara: altHesapOneri }, suppressKeyboardEvent: oneriTusu,
            filter: 'agTextColumnFilter', filterParams: kodSuzmeAyari('Süz: vergi no ya da ad')
        },
        paraSutun('BORC', 'Borç', { contextMenu: true }),
        paraSutun('ALACAK', 'Alacak', { contextMenu: true }),
        {
            colId: 'KESINTIHESAP', headerName: '', width: 40, sortable: false, cellClass: 'oem-sari',
            cellRenderer: function (p) {
                return $('<span class="oem-hucre-sec"></span>').append(hucreTus('glyphicon-th', 'Kesinti Hesapla', function () { kesintiAc(p.node); }))[0];
            }
        },
        {
            field: 'DOVIZ', colId: 'DOVIZ', headerName: 'Döviz Adı', width: 100, editable: true, sortable: false, cellClass: 'oem-girilir',
            cellEditor: 'agSelectCellEditor',
            cellEditorParams: function () { return { values: [''].concat($.map((tanim && tanim.dovizler) || [], function (d) { return d.kisaltma; })) }; },
            valueFormatter: function (p) { return dovizAd(p.value); }
        },
        {
            field: 'DOVIZKURTUR', colId: 'DOVIZKURTUR', headerName: 'Kur Türü', width: 100, editable: true, sortable: false, cellClass: 'oem-girilir',
            cellEditor: 'agSelectCellEditor', cellEditorParams: { values: $.map(KUR_TURLERI, function (k) { return k[0]; }) },
            valueFormatter: function (p) { return kurTurAd(p.value); }
        },
        paraSutun('DOVIZBORC', 'Döviz Borç', { colId: 'DOVIZBORC', width: 105 }),
        paraSutun('DOVIZALACAK', 'Döviz Alacak', { colId: 'DOVIZALACAK', width: 105 }),
        {
            field: 'MAHSUPBELGENO', colId: 'MAHSUPBELGENO', headerName: 'Mahsup Belge No', width: 150, editable: true, sortable: false, cellClass: 'oem-girilir',
            cellRenderer: tusluHucre([['glyphicon-search', 'Mahsup belgesi seç', function (n) { mahsupAc(n); }]]), cellEditor: 'agTextCellEditor'
        },
        {
            field: 'TERTIPPRG', colId: 'TERTIPPRG', headerName: 'Prg. Tertibi', width: 240, sortable: false, cellClass: 'oem-sari', hide: true,
            cellRenderer: tusluHucre([['glyphicon-search', 'Tertip seç', tertipAc]])
        },
        {
            field: 'ISAD', colId: 'ISAD', headerName: 'İş Adı', width: 190, sortable: false, cellClass: 'oem-sari',
            cellRenderer: tusluHucre([['glyphicon-remove', 'Temizle', isSil], ['glyphicon-search', 'Seç', isSec]])
        }
    ];

    // Toplam satırı (başlığın altında sabit, eski alt toplam bilgisi): düzenlenmez, hücre tuşları çizilmez.
    $.each(sutunlar, function (i, c) {
        if (c.editable === true) c.editable = function (p) { return !p.node.rowPinned; };
        var cizici = c.cellRenderer;
        if (cizici) c.cellRenderer = function (p) {
            if (!p.node.rowPinned) return cizici(p);
            return p.valueFormatted != null ? p.valueFormatted : (p.value == null ? '' : String(p.value));
        };
    });

    function toplamSatiri() {
        var fark = yuvarla(toplam.borc - toplam.alacak);
        return {
            _id: 'toplam', HESAPKOD: 'Toplam', ALTHESAPKOD: fark ? 'Fark: ' + paraYaz(fark) : '',
            BORC: toplam.borc, ALACAK: toplam.alacak, DOVIZBORC: toplam.dovizBorc, DOVIZALACAK: toplam.dovizAlacak
        };
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: sutunlar,
        pinnedTopRowData: [{ _id: 'toplam', HESAPKOD: 'Toplam', BORC: 0, ALACAK: 0, DOVIZBORC: 0, DOVIZALACAK: 0 }],
        getRowClass: function (p) { return p.node.rowPinned ? 'oem-toplam-satir' : ''; },
        rowData: [],
        rowHeight: 26,
        headerHeight: 30,
        getRowId: function (p) { return String(p.data._id); },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        suppressCellFocus: false,
        preventDefaultOnContextMenu: true,
        rowSelection: { mode: 'multiRow', checkboxes: false, headerCheckbox: false, enableClickSelection: true },
        onSelectionChanged: function () { $('#btnSatirSil').prop('disabled', grid.getSelectedNodes().length === 0); },
        onCellFocused: function (e) { if (e.rowIndex != null && !e.rowPinned) aktifSatir = grid.getDisplayedRowAtIndex(e.rowIndex); },
        onCellContextMenu: function (e) {
            if (!e.colDef.contextMenu || !e.event || e.node.rowPinned) return;
            aktifSatir = e.node;
            var ev = e.event;
            $('#mnuBakiye').css({ left: ev.clientX + 'px', top: ev.clientY + 'px' }).prop('hidden', false);
        },
        onCellValueChanged: function (e) {
            if (e.newValue === e.oldValue) return;
            var f = e.colDef.field;
            if (f === 'DOVIZKURTUR' || f === 'DOVIZ') dovizKurBul(e.node, f);
            if (f === 'DOVIZBORC' || f === 'DOVIZALACAK') dovizDonustur(e.node, f);
            if (f === 'BORC' || f === 'ALACAK' || f === 'DOVIZBORC' || f === 'DOVIZALACAK') toplamYaz();
            else if (f === 'HESAPKOD') hesapKodDegisti(e.node);
            else if (f === 'ALTHESAPKOD') altHesapDegisti(e.node);
            if (f === 'TERTIP') grid.refreshCells({ rowNodes: [e.node], columns: ['DETAYBIRIM'], force: true });
        }
    });
    var aktifSatir = null;

    function satirlar() {
        var r = [];
        grid.forEachNode(function (n) { r.push(n.data); });
        return r;
    }

    // Sunucuya giden satırlar (eski SatirDegerler): eski alan adlarıyla, yalnız hesap kodu dolu olanlar sunucuda işlenir.
    function satirDegerler() {
        var i = 0;
        return $.map(satirlar(), function (s) {
            i++;
            var d = $.extend({}, s);
            delete d._id;
            d.SIRANO = i;
            d.KESINTI = Number(d.KESINTI) ? 1 : 0;
            return d;
        });
    }

    function satirlariYaz(liste) {
        satirNo = 0;
        var r = $.map(liste || [], function (s) { return $.extend(bosSatir(), s); });
        if (!r.length) for (var i = 0; i < ((tanim && tanim.satirSayisi) || 20); i++) r.push(bosSatir());
        grid.setGridOption('rowData', r);
        toplamYaz();
    }

    // eski BorcAlacakToplaYaz
    var toplam = { borc: 0, alacak: 0, dovizBorc: 0, dovizAlacak: 0 };
    function toplamYaz() {
        toplam = { borc: 0, alacak: 0, dovizBorc: 0, dovizAlacak: 0 };
        grid.forEachNode(function (n) {
            toplam.borc += Number(n.data.BORC) || 0;
            toplam.alacak += Number(n.data.ALACAK) || 0;
            toplam.dovizBorc += Number(n.data.DOVIZBORC) || 0;
            toplam.dovizAlacak += Number(n.data.DOVIZALACAK) || 0;
        });
        $.each(toplam, function (k, v) { toplam[k] = yuvarla(v); });
        grid.setGridOption('pinnedTopRowData', [toplamSatiri()]);
    }

    function yenile(node) { grid.refreshCells({ rowNodes: [node], force: true }); }

    // eski BakiyeKapat: kasa / avans hesabı yazılınca bakiye o satıra kapatılır
    function bakiyeKapat(node, kapatmaHesapKod) {
        var toplamHesap = (tanim.kasaHesabi || '') + (tanim.avansHesabi || '');
        var k = '|' + String(kapatmaHesapKod || '').replace(/\./g, '').substring(0, 3) + '|';
        if (toplamHesap.indexOf(k) === -1) return;
        bakiyeYaz(node);
    }
    function bakiyeYaz(node) {
        node.data.BORC = 0; node.data.ALACAK = 0;
        toplamYaz();
        var kalan = yuvarla(toplam.borc - toplam.alacak);
        if (kalan > 0) node.data.ALACAK = kalan; else node.data.BORC = -kalan;
        yenile(node);
        toplamYaz();
    }

    // Borç / alacak sağ tık menüsü (eski mnuBakiye)
    $('#mnuBakiye').on('click', 'a', function (e) {
        e.preventDefault();
        $('#mnuBakiye').prop('hidden', true);
        var n = aktifSatir;
        if (!n) return;
        var islem = $(this).attr('data-islem');
        if (islem === 'bakiye') bakiyeYaz(n);
        else if (islem === 'yer') {
            var b = n.data.BORC; n.data.BORC = n.data.ALACAK; n.data.ALACAK = b;
            yenile(n); toplamYaz();
        } else if (islem === 'doviz') {
            var kur = Number(n.data.DOVIZKUR) || 0;
            if (!kur) { mesaj('Kur bilgisi okunamadı. Döviz Adı ve Kur Türü alanlarını doldurduktan sonra bu işlemi yapın', 'warning'); return; }
            n.data.DOVIZBORC = (Number(n.data.BORC) || 0) / kur;
            n.data.DOVIZALACAK = (Number(n.data.ALACAK) || 0) / kur;
            yenile(n); toplamYaz();
        }
    });

    // eski DovizKurBul / DovizDonustur
    function dovizKurBul(node, alan) {
        var d = node.data;
        if (!d.DOVIZ || !d.DOVIZKURTUR) return $.Deferred().resolve().promise();
        return Sgb.al(UC + 'doviz-kur', { doviz: d.DOVIZ, kurTur: d.DOVIZKURTUR, tarih: $('#txtBelgeTarih').val() }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            if (Number(v.kur) > 0) {
                d.DOVIZKUR = Number(v.kur);
                dovizDonustur(node, alan || 'DOVIZ');
                toplamYaz();
            }
        }).fail(hataGoster);
    }
    function dovizDonustur(node, alan) {
        var d = node.data, kur = Number(d.DOVIZKUR) || 0;
        if (!kur) { dovizKurBul(node, alan); return; }
        if (alan === 'DOVIZ' || alan === 'DOVIZKURTUR') {
            d.BORC = yuvarla((Number(d.DOVIZBORC) || 0) * kur);
            d.ALACAK = yuvarla((Number(d.DOVIZALACAK) || 0) * kur);
        } else if (alan === 'DOVIZBORC') d.BORC = yuvarla((Number(d.DOVIZBORC) || 0) * kur);
        else if (alan === 'DOVIZALACAK') d.ALACAK = yuvarla((Number(d.DOVIZALACAK) || 0) * kur);
        yenile(node);
    }

    // Belge tarihi değişince dolu satırların kuru yeniden okunur (eski TarihDegisti)
    $('#txtBelgeTarih').on('change', function () {
        grid.forEachNode(function (n) {
            var d = n.data;
            if (d.BORC > 0 || d.DOVIZBORC > 0 || d.ALACAK > 0 || d.DOVIZALACAK > 0) dovizKurBul(n, 'DOVIZ');
        });
    });

    // Elle yazılan hesap kodu: "kod - ad" biçimine getirilir, kasa / avans hesabıysa bakiye kapatılır, döviz hesabıysa döviz yazılır.
    function hesapKodDegisti(node) {
        var kod = kodAyir(node.data.HESAPKOD).replace(/\s/g, '');
        if (!kod) { node.data.HESAPAD = ''; yenile(node); return; }
        Sgb.al(HESAP_UC + 'ad', { yil: yil(), kod: kod }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            if (!v.ad) { mesaj(kod + ' kodlu hesap bulunamadı.', 'warning'); node.data.HESAPKOD = kod; yenile(node); return; }
            node.data.HESAPKOD = v.kod + '-' + v.ad;
            node.data.HESAPAD = v.ad;
            var duz = String(v.kod).replace(/\./g, '');
            if ((tanim.dovizHesabi || '').indexOf('|' + duz + '|') !== -1 && v.dovizKurKod) {
                node.data.DOVIZ = v.dovizKurKod;
                node.data.DOVIZKURTUR = v.dovizKurTurSatis || '';
            }
            yenile(node);
            bakiyeKapat(node, v.kod);
        }).fail(hataGoster);
    }

    function altHesapDegisti(node) {
        var deger = $.trim(node.data.ALTHESAPKOD || '');
        if (!deger || deger.indexOf('-') > 0) return;
        Sgb.al(UC + 'althesap/ara', { muhasebe: muhBirim.deger().muhasebe, metin: deger }).done(function (v) {
            var s = (v.satirlar || []).filter(function (x) { return x.vergiNo === deger; })[0];
            if (s) { node.data.ALTHESAPKOD = s.vergiNo + ' - ' + s.ad; yenile(node); }
        });
    }

    function hesapPlaniAc(node) {
        Sgb.hesapPlaniSec({
            yil: yil(), sonKirilim: true, uc: HESAP_UC,
            secilince: function (h) {
                node.data.HESAPKOD = h.kod + '-' + (h.hesapAd || '');
                node.data.HESAPAD = h.hesapAd || '';
                yenile(node);
                hesapKodDegisti(node);
            }
        });
    }

    // eski FirmaBilgisiAc('grd') / FirmaBilgisiGoster
    function firmaAc(node) {
        Sgb.firmaSec({
            muhasebe: muhBirim.deger().muhasebe, adres: UC + 'firma/ara',
            secilince: function (f) { node.data.ALTHESAPKOD = f.vergiNo + '-' + f.ad; yenile(node); }
        });
    }

    // eski TertipGosterX / ListeTertipAktar: seçilen tertipler bu satırdan başlayarak aşağı yazılır
    function tertipAc(node) {
        var d = node.data, mb = muhBirim.deger();
        var hesapKod = kodAyir(d.HESAPKOD);
        if (!hesapKod) { mesaj('Lütfen ilk önce hesap kodunu seçin.', 'warning'); return; }
        var birim = mb.birim;
        if (d.DETAYBIRIM) { var db = d.DETAYBIRIM.split('.'); if (db.length && db[0] !== '000') birim = db[0]; }
        Sgb.tertipSec({
            tur: 'muh', yil: yil(), muhasebe: mb.muhasebe, birim: birim, hesapKod: hesapKod, secKapat: true, coklu: true,
            secilince: function (liste) {
                var idx = node.rowIndex;
                $.each(liste, function (i, t) {
                    var n = grid.getDisplayedRowAtIndex(idx + i);
                    if (!n) return false;
                    n.data.TERTIP = t.TERTIP || '';
                    n.data.TERTIPPRG = t.TERTIPPRG || '';
                    yenile(n);
                });
            }
        });
    }

    function isSec(node) {
        var mb = muhBirim.deger();
        Sgb.oem.isSec({
            yil: yil(), muh: mb.muhasebe, muhBir: mb.birim,
            secilince: function (kod, adi) { node.data.ISKOD = kod; node.data.ISAD = adi; yenile(node); }
        });
    }
    function isSil(node) { node.data.ISKOD = 0; node.data.ISAD = ''; yenile(node); }

    // eski KomutCalistir 'KesintiSec': 320 / 16 hesaplarında alt hesap bazında muavin, diğerlerinde kesinti hesabı
    function kesintiAc(node) {
        var hesapKod = String(node.data.HESAPKOD || '');
        if (hesapKod.indexOf('320') === 0 || hesapKod.indexOf('16') === 0) {
            Sgb.oem.altHesapMuavin({ yil: yil(), hesapKod: hesapKod, altHesapKod: node.data.ALTHESAPKOD || '', projeNo: $('#txtProjeNo').val() });
            return;
        }
        var borc = 0, avans = 0;
        grid.forEachNode(function (n) {
            if (n.data.BORC > 0) borc += Number(n.data.BORC);
            if (String(n.data.HESAPKOD || '').substring(0, 2) === '16') avans += Number(n.data.ALACAK) || 0;
        });
        Sgb.oem.kesintiHesapla({
            borc: borc, avans: avans, satir: node,
            secilince: function (tutar, n) { n.data.ALACAK = yuvarla(tutar); yenile(n); toplamYaz(); }
        });
    }

    // eski MahsupBelgeEKraniAc / MahsupSecKapatBasildi
    function mahsupAc(node) {
        var mb = muhBirim.deger();
        var hesapKod = '', altKod = '', altAd = '';
        if (node) {
            hesapKod = kodAyir(node.data.HESAPKOD);
            var k = String(node.data.ALTHESAPKOD || '').split('-');
            altKod = $.trim(k[0]);
            if (k.length > 1) altAd = $.trim(k.slice(1).join('-'));
        }
        Sgb.oem.mahsupBelgeSec({
            yil: yil(), ilgiliAd: altAd, ilgiliNo: altKod, muhasebe: mb.muhasebe, birim: mb.birim, hesapKod: hesapKod,
            secilince: function (m) {
                if (!node) { $('#txtIlgiliBelgeNo').val(m.belgeNo); return; }
                var d = node.data;
                d.MAHSUPBELGENO = m.belgeNo;
                d.ALTHESAPKOD = m.altHesapKod;
                d.DOVIZKUR = Number(sayiCoz(m.dovizKur)) || 0;
                d.DOVIZKURTUR = m.dovizKurTur || '';
                d.DOVIZ = m.doviz || '';
                d.SATIRACIKLAMA = m.satirAciklama || '';
                if (m.doviz) {
                    if (!m.dovizKurTur) { yenile(node); return; }
                    Sgb.al(UC + 'doviz-kur', { doviz: m.doviz, kurTur: m.dovizKurTur, tarih: $('#txtBelgeTarih').val() }).done(function (v) {
                        var kur = Number(v.kur) || 0;
                        if (kur > 0) {
                            d.DOVIZKUR = kur;
                            var da = Number(sayiCoz(m.dovizAlacak)) || 0, dbo = Number(sayiCoz(m.dovizBorc)) || 0;
                            var tutar = yuvarla((da > 0 ? da : dbo) * kur);
                            if (da > 0) { d.BORC = tutar; d.DOVIZBORC = da; d.ALACAK = 0; d.DOVIZALACAK = 0; }   // mahsup edilen alacak kaydı borca
                            else { d.ALACAK = tutar; d.DOVIZALACAK = dbo; d.BORC = 0; d.DOVIZBORC = 0; }
                        }
                        yenile(node); toplamYaz();
                    }).fail(hataGoster);
                } else {
                    var t = Number(m.tutar) || 0;
                    if (t < 0) { d.BORC = -t; d.ALACAK = 0; } else { d.ALACAK = t; d.BORC = 0; }
                    yenile(node); toplamYaz();
                }
            }
        });
    }
    $('#btnMahsupSec').on('click', function () { mahsupAc(null); });

    // ---------------------------------------------------------------- satır işlemleri

    function seciliDugumler() {
        var s = grid.getSelectedNodes().slice();
        s.sort(function (a, b) { return a.rowIndex - b.rowIndex; });
        return s;
    }
    function seciliIlk() { var s = seciliDugumler(); return s.length ? s[0] : aktifSatir; }

    $('#btnSatirAc').on('click', function () {
        var ek = [];
        for (var i = 0; i < 20; i++) ek.push(bosSatir());
        grid.applyTransaction({ add: ek });
    });
    $('#btnSatirAcAraya').on('click', function () {
        var n = seciliIlk();
        grid.applyTransaction({ add: [bosSatir()], addIndex: n ? n.rowIndex : 0 });
    });
    function satirSil() {
        var s = grid.getSelectedRows();
        if (!s.length) return;
        grid.applyTransaction({ remove: s });
        $('#btnSatirSil').prop('disabled', true);
        toplamYaz();
    }
    $('#btnSatirSil').on('click', satirSil);

    // Panoya / panodan: tablodaki görünen girilebilir sütunlar sırasıyla, sekmeyle ayrılmış (eski CopyGrid / EXTVeriYapistir).
    function yapistirSutunlari() {
        // Sıra eski sayfanın sütun sırasıdır; ekrandaki sütun yerleri değişse de Excel'den yapıştırma sırası değişmez.
        var sira = ['KESINTI', 'HESAPKOD', 'DETAYBIRIM', 'TERTIP', 'SATIRACIKLAMA', 'ALTHESAPKOD', 'BORC', 'ALACAK', 'DOVIZ', 'DOVIZKURTUR', 'DOVIZBORC', 'DOVIZALACAK', 'MAHSUPBELGENO'];
        return $.map(sira, function (f) {
            var c = grid.getColumn(f);
            return c && c.isVisible() && c.getColDef().editable ? f : null;
        });
    }
    function kopyala() {
        var alanlar = yapistirSutunlari();
        var metin = $.map(seciliDugumler(), function (n) {
            return $.map(alanlar, function (a) {
                var v = n.data[a];
                if (a === 'BORC' || a === 'ALACAK' || a === 'DOVIZBORC' || a === 'DOVIZALACAK') v = String(Number(v) || 0).replace('.', ',');
                return String(v == null ? '' : v).replace(/[\t\r\n]/g, ' ');
            }).join('\t');
        }).join('\n');
        if (!metin) { mesaj('Kopyalamak için satır seçiniz.', 'warning'); return false; }
        if (navigator.clipboard && navigator.clipboard.writeText) navigator.clipboard.writeText(metin).catch(function () { });
        return true;
    }
    function yapistir(metin) {
        var bas = seciliIlk();
        if (!bas) { mesaj('Yapıştırmak için başlangıç satırını seçiniz.', 'warning'); return; }
        var alanlar = yapistirSutunlari();
        var satirlarP = String(metin || '').replace(/\r/g, '').split('\n');
        while (satirlarP.length && satirlarP[satirlarP.length - 1] === '') satirlarP.pop();
        if (!satirlarP.length) return;
        var idx = bas.rowIndex, eksik = idx + satirlarP.length - grid.getDisplayedRowCount();
        if (eksik > 0) { var ek = []; for (var i = 0; i < eksik; i++) ek.push(bosSatir()); grid.applyTransaction({ add: ek }); }
        $.each(satirlarP, function (i, s) {
            var n = grid.getDisplayedRowAtIndex(idx + i);
            if (!n) return false;
            var hucreler = s.split('\t');
            for (var j = 0; j < Math.min(alanlar.length, hucreler.length); j++) {
                var a = alanlar[j], v = hucreler[j];
                if (a === 'BORC' || a === 'ALACAK' || a === 'DOVIZBORC' || a === 'DOVIZALACAK') v = yuvarla(sayiCoz(String(v).replace(/\./g, '').replace(',', '.')) || 0);
                else if (a === 'KESINTI') v = Number(v) ? 1 : 0;
                n.data[a] = v;
            }
        });
        grid.refreshCells({ force: true });
        toplamYaz();
    }
    function panodanYapistir() {
        if (navigator.clipboard && navigator.clipboard.readText) {
            navigator.clipboard.readText().then(yapistir).catch(function () { yapistirPenceresi(); });
        } else yapistirPenceresi();
    }
    function yapistirPenceresi() {
        $('#txtYapistir').val('');
        document.getElementById('dlgYapistir').showModal();
        $('#txtYapistir').trigger('focus');
    }
    $('#btnYapistirAktar').on('click', function () {
        document.getElementById('dlgYapistir').close();
        yapistir($('#txtYapistir').val());
    });
    $('#btnSatirKopyala').on('click', kopyala);
    $('#btnSatirKes').on('click', function () { if (kopyala()) satirSil(); });
    $('#btnSatirYapistir, #btnVeriYapistir').on('click', panodanYapistir);
    $('#grdListe').on('keydown', function (e) {
        if (!(e.ctrlKey || e.metaKey) || $(e.target).is('input, textarea, select')) return;
        var k = (e.key || '').toLowerCase();
        if (k === 'c') { e.preventDefault(); kopyala(); }
        else if (k === 'x') { e.preventDefault(); if (kopyala()) satirSil(); }
        else if (k === 'v') { e.preventDefault(); panodanYapistir(); }
    });

    // ---------------------------------------------------------------- belge formu

    function tarihAl(id) { return $(id).val() || ''; }

    function formTemizle(satirSil_) {
        belge = { durum: 0, belgeNo: '' };
        durumYaz(0, '');
        $('#txtBelgeTarih').val(Sgb.bugun());
        $('#txtBelgeNo').val('');
        $('#txtIlgiliAd, #txtIlgiliNo, #txtIlgiliVD, #txtIlgiliBankaAd, #txtIlgiliBankaNo, #txtAciklama').val('');
        $('#ddlIst1, #ddlIst2').val('0');
        $('#txtOEBTur, #txtOEBNo, #txtOEBTarih, #txtOEBTutar, #txtIOETarih, #txtIOEYevNo, #txtIOEKim, #txtAVKTarih, #txtAVKYevNo, #txtAVKTutar').val('');
        $('#txtYevTarih, #txtYevNo, #txtYEVVerNo, #txtYEVCekNo, #txtYEVCekTarih, #txtProjeNo, #txtIlgiliBelgeNo, #txtSurecNo').val('');
        $('#ddlIhaleKod, #ddlAlimTur').val('');
        $('#ddlFisTur').prop('selectedIndex', 0);
        $('#txtIslemYevNo, #txtYevAciklama').val('');
        if (satirSil_) satirlariYaz([]);
        dokumanYaz([]);
        oybs = { secili: '', kilitli: false };
        if (oybsGrid) { oybsGrid.setGridOption('rowData', []); oybsKilit(false); }
        mukerrerOnay = false;
    }

    function durumYaz(durum, durumAd) {
        var metin = durumAd ? durumAd + ' Belge' : 'Belge';
        $('#lnkDurum').text(metin).toggleClass('oem-durum-var', !!durumAd);
    }

    function belgeYaz(b) {
        formTemizle(false);
        belge = { durum: b.durum, belgeNo: b.belgeNo, belgeTur: b.belgeTur };
        $('#txtYil').val(b.yil);
        durumYaz(b.durum, b.durumAd);
        $('#txtBelgeTarih').val(b.belgeTarih || '');
        $('#txtBelgeNo').val(b.belgeNo);
        muhBirim.ata({ muhasebe: b.muhasebe, birim: b.birim, kurumsal: b.kurumsal, birimAd: b.birimAd });
        detayBirimleriOku();
        $('#txtIlgiliAd').val(b.ilgiliAd); $('#txtIlgiliNo').val(b.ilgiliNo); $('#txtIlgiliVD').val(b.ilgiliVD);
        $('#txtIlgiliBankaAd').val(b.ilgiliBankaAd); $('#txtIlgiliBankaNo').val(b.ilgiliBankaNo); $('#txtAciklama').val(b.aciklama);
        $('#ddlIst1').val(String(b.ist1 || 0)); $('#ddlIst2').val(String(b.ist2 || 0));
        $('#txtOEBTur').val(b.OEBTur); $('#txtOEBNo').val(b.OEBNo); $('#txtOEBTarih').val(b.OEBTarih);
        $('#txtOEBTutar').val(b.OEBTutar ? paraYaz(b.OEBTutar) : '');
        $('#txtIOETarih').val(b.IOETarih); $('#txtIOEYevNo').val(b.IOEYevNo); $('#txtIOEKim').val(b.IOEKim);
        $('#txtAVKTarih').val(b.AVKTarih); $('#txtAVKYevNo').val(b.AVKYevNo);
        $('#txtAVKTutar').val(b.AVKTutar ? paraYaz(b.AVKTutar) : '');
        $('#txtYevTarih').val(b.YEVYevTarih); $('#txtYevNo').val(b.YEVYevNo);
        $('#txtYEVVerNo').val(b.YEVVerNo); $('#txtYEVCekNo').val(b.YEVCekNo); $('#txtYEVCekTarih').val(b.YEVCekTarih);
        $('#txtProjeNo').val(b.projeNo);
        $('#ddlIhaleKod').val(b.ihaleKod || ''); $('#ddlAlimTur').val(b.alimTur || '');
        $('#ddlFisTur').val(String(b.fisTur));
        $('#txtIlgiliBelgeNo').val(b.ilgiliBelgeNo); $('#txtSurecNo').val(b.surecNo);
        $('#txtIslemYevNo').val(b.YEVYevNo); $('#txtYevAciklama').val(b.yevmiyeAciklama);
        satirlariYaz(b.satirlar);
        dokumanOku();
        if (tanim.oybs) oybsOku(b.durum);
    }

    function belgeOku(y, no, yevNo) {
        var $t = $('#btnListele, #btnListeleYev');
        yukleniyor($t, true);
        grid.setGridOption('loading', true);
        return Sgb.al(UC + 'belge', { yil: y, belgeNo: no || '', yevmiyeNo: yevNo || '', baslik: sayfa.baslik ? '1' : '' })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                belgeYaz(v.belge);
            })
            .fail(hataGoster)
            .always(function () { yukleniyor($t, false); grid.setGridOption('loading', false); });
    }

    window.OemBelgeYukle = function (y, no) {
        $('#txtYil').val(y);
        belgeOku(parseInt(y, 10) || yil(), no);
    };

    $('#btnListele').on('click', function () { belgeOku(yil(), belgeNo()); });
    $('#txtBelgeNo').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); belgeOku(yil(), belgeNo()); } });
    $('#btnListeleYev').on('click', function () { belgeOku(yil(), '', $.trim($('#txtYevNo').val())); });
    $('#txtYevNo').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); belgeOku(yil(), '', $.trim($(this).val())); } });

    $('#btnTemizle').on('click', function () { formTemizle(true); });

    $('#lnkDurum').on('click', function (e) {
        e.preventDefault();
        if (tanim && tanim.tarim) return; // eski: Tarım'da durum yazısı bağlantı değil, ayrı tarihçe tuşu var
        tarihceAc();
    });
    function tarihceAc() {
        if (!belgeNo()) return;
        Sgb.oem.tarihce({ yil: yil(), belgeNo: belgeNo() });
    }
    $('#btnTarihce').on('click', tarihceAc);

    // Kayıt isteği (eski btnKaydet_Click toplama alanları)
    function kayitIstegi() {
        var mb = muhBirim.deger();
        return {
            Yil: yil(), BelgeTarih: tarihAl('#txtBelgeTarih'), BelgeNo: belgeNo(),
            Muhasebe: mb.muhasebe, Birim: mb.birim, Kurumsal: mb.kurumsal, Baslik: sayfa.baslik,
            IlgiliAd: $('#txtIlgiliAd').val(), IlgiliNo: $('#txtIlgiliNo').val(), IlgiliVD: $('#txtIlgiliVD').val(),
            IlgiliBankaAd: $('#txtIlgiliBankaAd').val(), IlgiliBankaNo: $('#txtIlgiliBankaNo').val(), Aciklama: $('#txtAciklama').val(),
            Ist1: parseInt($('#ddlIst1').val(), 10) || 0, Ist2: parseInt($('#ddlIst2').val(), 10) || 0,
            OEBTur: $('#txtOEBTur').val(), OEBNo: $('#txtOEBNo').val(), OEBTarih: tarihAl('#txtOEBTarih'), OEBTutar: $('#txtOEBTutar').val(),
            IOETarih: tarihAl('#txtIOETarih'), IOEYevNo: $('#txtIOEYevNo').val(), IOEKim: $('#txtIOEKim').val(),
            AVKTarih: tarihAl('#txtAVKTarih'), AVKYevNo: $('#txtAVKYevNo').val(), AVKTutar: $('#txtAVKTutar').val(),
            YEVVerNo: $('#txtYEVVerNo').val(), YEVCekNo: $('#txtYEVCekNo').val(), YEVCekTarih: tarihAl('#txtYEVCekTarih'),
            ProjeNo: $('#txtProjeNo').val(), IhaleKod: $('#ddlIhaleKod').val(), AlimTur: $('#ddlAlimTur').val(),
            FisTur: parseInt($('#ddlFisTur').val(), 10) || 0, IlgiliBelgeNo: $('#txtIlgiliBelgeNo').val(), SurecNo: $('#txtSurecNo').val(),
            MukerrerKontrol: !mukerrerOnay, Oybs: oybs.secili,
            Satirlar: sayfa.baslik ? [] : satirDegerler()
        };
    }

    function kaydet() {
        grid.stopEditing();
        var $t = $('#btnKaydet');
        yukleniyor($t, true);
        Sgb.gonder(UC + 'kaydet', kayitIstegi()).done(function (v) {
            mukerrerOnay = false;
            if (v.mukerrer) {
                sor(v.hata, function () { mukerrerOnay = true; kaydet(); },
                    { baslik: 'Mükerrer Belge Uyarısı', belgeler: v.belgeler, yil: v.yil, evet: 'Evet, kaydet', hayir: 'Hayır' });
                return;
            }
            sonucIsle(v, function () {
                if (v.belgeNo) { $('#txtBelgeNo').val(v.belgeNo); belgeOku(yil(), v.belgeNo); }
            });
        }).fail(hataGoster).always(function () { yukleniyor($t, false); });
    }
    $('#btnKaydet').on('click', function () {
        // eski OdemeEmriKayitTarihKontrol: yeni belgede tarih bugünden eskiyse uyarı
        var uyari = '';
        var t = tarihAl('#txtBelgeTarih');
        if (!belgeNo() && t && t < Sgb.bugun()) uyari = 'Uyarı: Yeni bir belge oluşturmak istiyorsunuz. Fakat tarih bugünden eski. Devam edilsin mi?';
        sor('Kayıt işlemi yapılacak. Onaylıyor musunuz?', kaydet, { uyari: uyari });
    });

    // ---------------------------------------------------------------- durum işlemleri

    function durumDegistir(islem, ek, bitince) {
        if (!belgeNo()) { mesaj('İşlem için önce belgeyi listeleyiniz.', 'warning'); return; }
        var istek = $.extend({ Yil: yil(), BelgeNo: belgeNo(), Islem: islem, SurecNo: $('#txtSurecNo').val(), Oybs: oybs.secili }, ek || {});
        Sgb.gonder(UC + 'durum', istek).done(function (v) {
            sonucIsle(v, function () {
                if (bitince) bitince();
                belgeOku(yil(), belgeNo());
            });
        }).fail(hataGoster);
    }
    $('#btnOnayla').on('click', function () { durumDegistir('onayla'); });
    $('#btnOnayKaldir').on('click', function () { durumDegistir('onaykaldir'); });
    $('#btnIptal').on('click', function () { sor('İşlem yapılacak. Onaylıyor musunuz?', function () { durumDegistir('iptal'); }, { baslik: 'İşlem Onayı' }); });

    var dlgYevmiye = document.getElementById('dlgYevmiye');
    $('#btnYevmiye').on('click', function () {
        if (!belgeNo()) { mesaj('İşlem için önce belgeyi listeleyiniz.', 'warning'); return; }
        dlgYevmiye.showModal();
    });
    $('#chkYevBelgeTarihi').on('change', function () { $('#grpYevTarih').prop('hidden', this.checked); });
    $('#btnYevmiyeVer').on('click', function () {
        sor('İşlem yapılacak. Onaylıyor musunuz?', function () {
            durumDegistir('yevver', {
                YevmiyeNo: $('#txtIslemYevNo').val(), YevmiyeAciklama: $('#txtYevAciklama').val(),
                BelgeTarihi: $('#chkYevBelgeTarihi').prop('checked'), YevmiyeTarih: $('#txtIslemYevTarih').val()
            }, function () { dlgYevmiye.close(); });
        }, { baslik: 'İşlem Onayı' });
    });
    $('#btnYevmiyeNoKaldir').on('click', function () {
        sor('İşlem yapılacak. Onaylıyor musunuz?', function () { durumDegistir('yevkaldir', {}, function () { dlgYevmiye.close(); }); }, { baslik: 'İşlem Onayı' });
    });

    // ---------------------------------------------------------------- yazdırma, EBYS, raporlar

    function prg() { return $('#chkPrg').prop('checked') ? '1' : ''; }
    function yazdir(tur) {
        if (!belgeNo()) { mesaj('Yazdırmak için önce belgeyi kaydediniz ya da listeleyiniz.', 'warning'); return; }
        var $t = $('#btnYazdir, #grpYazdirTercih > .btn');
        yukleniyor($t, true);
        Sgb.indir(UC + 'yazdir', { yil: yil(), belgeNo: belgeNo(), tur: tur, prg: prg() }).then(function () { yukleniyor($t, false); });
    }
    $('#btnYazdir').on('click', function () { yazdir(1); });
    $('.oem-yazdir-menu').on('click', '[data-yazdir]', function (e) { e.preventDefault(); yazdir(parseInt($(this).attr('data-yazdir'), 10)); });

    // İmza tanımıyla yazdır (eski ddlImzaTanimYazdir): seçilen tanım geçerli yapılır, belge o tanımın belge türüyle yazdırılır.
    var imzalar = [];
    function imzaListesiOku() {
        return Sgb.al('api/imzatanim/liste').done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            imzalar = v.satirlar || [];
            var $s = $('#ddlImza').empty().append('<option value="">İmza Seçiniz...</option>');
            $.each(imzalar, function (i, x) { $('<option></option>').val(x.kod).text(x.ad + (x.belgeTurAd ? ' (' + x.belgeTurAd + ')' : '')).appendTo($s); });
            var secili = v.secili || (tanim && tanim.imzaKod) || 0;
            if (secili) $s.val(String(secili));
            $('#btnImzaTanim').prop('hidden', !v.yonetici);
        });
    }
    $('#ddlImza').on('change', function () {
        var kod = parseInt($(this).val(), 10) || 0;
        if (!kod) return;
        var x = $.grep(imzalar, function (i) { return i.kod === kod; })[0];
        Sgb.gonder('api/imzatanim/gecerli', { kod: kod }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            $('.oem-arac .btn-group.open').removeClass('open');
            var bt = x ? x.belgeTur : 0;
            yazdir(bt === 1 ? 2 : (bt === 2 ? 3 : 1));
        }).fail(hataGoster);
    });
    $('#btnImzaTanim').on('click', function (e) {
        e.stopPropagation();
        $('.oem-arac .btn-group.open').removeClass('open');
        Sgb.pencere('ImzaTanim', 'İmza Tanımları', imzaListesiOku);
    });

    $('#btnKapatmaListesi').on('click', function () {
        if (!belgeNo()) { mesaj('Kapatma listesi için önce belgeyi listeleyiniz ya da belge numarasını yazınız.', 'warning'); return; }
        Sgb.oem.kapatmaListesi({ yil: yil(), belgeNo: belgeNo() });
    });

    // Rapor Al (eski ButceMuhasebeGenel.HizliRaporAl): 1 KBS-SGB Fark (Excel), 2 Seyyar Görev kullanımı penceresi
    var seyyarGrid = null;
    $('#btnRaporAl').on('click', function () {
        var kod = $('#ddlRapor').val(), mb = muhBirim.deger();
        if (kod === '2') { seyyarAc(mb); return; }
        var $t = $(this);
        yukleniyor($t, true);
        Sgb.indir(UC + 'hizli-rapor', { kod: kod, yil: yil(), muhasebe: mb.muhasebe || -1, birim: mb.birim }).then(function () { yukleniyor($t, false); });
    });
    function seyyarAc(mb) {
        var dlg = document.getElementById('dlgSeyyar');
        if (!seyyarGrid) {
            seyyarGrid = Sgb.grid('#grdSeyyar', {
                columnDefs: [
                    { field: 'ad', headerName: 'İst. Şekli', flex: 1, cellClass: 'oem-sari' },
                    { field: 'vizeEdilen', headerName: 'Vize Edilen', width: 110, type: 'rightAligned', cellClass: 'oem-sari' },
                    { field: 'kullanilan', headerName: 'Kullanılan', width: 110, type: 'rightAligned', cellClass: 'oem-sari' },
                    { field: 'kalan', headerName: 'Kalan', width: 100, type: 'rightAligned', cellClass: function (p) { return p.value < 0 ? 'oem-eksi' : 'oem-sari'; } },
                    { field: 'tutar', headerName: 'Tutar', width: 130, type: 'rightAligned', cellClass: 'oem-sari', valueFormatter: function (p) { return paraYaz(p.value); } }
                ],
                rowHeight: 26, headerHeight: 30
            });
        }
        dlg.showModal();
        seyyarGrid.setGridOption('loading', true);
        Sgb.al(UC + 'seyyar', { yil: yil(), muhasebe: mb.muhasebe, birim: mb.birim }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            seyyarGrid.setGridOption('rowData', v.satirlar || []);
            $('#lblSeyGonderilen').text(paraYaz(v.gonderilen));
            $('#lblSeyHarcanan').text(paraYaz(v.harcanan));
            $('#lblSeyKalan').text(paraYaz(v.kalan));
        }).fail(hataGoster).always(function () { seyyarGrid.setGridOption('loading', false); });
    }

    // ---------------------------------------------------------------- ilgili, süreç, proje, taahhüt

    $('#btnIlgiliSec').on('click', function () {
        Sgb.firmaSec({
            muhasebe: muhBirim.deger().muhasebe, adres: UC + 'firma/ara', ara: $.trim($('#txtIlgiliNo').val()),
            secilince: function (f) {
                $('#txtIlgiliAd').val(f.ad || '');
                $('#txtIlgiliNo').val(f.vergiNo || '');
                // firma/ara satırının vergi dairesi ve banka bilgileri (eski FirmaBilgisiGoster)
                Sgb.al(UC + 'firma/ara', { muhasebe: muhBirim.deger().muhasebe, vergiNo: f.vergiNo }).done(function (v) {
                    var s = (v.satirlar || []).filter(function (x) { return x.vergiNo === f.vergiNo; })[0];
                    if (!s) return;
                    $('#txtIlgiliVD').val(s.vergiDairesi || '');
                    $('#txtIlgiliBankaAd').val(s.bankaAd || '');
                    $('#txtIlgiliBankaNo').val(s.bankaHesapNo || '');
                });
            }
        });
    });

    // eski VergiBorcuAc: Borcu Yoktur Belgesi
    $('#btnVergiBorcu').on('click', function () {
        var vNo = $.trim($('#txtIlgiliNo').val());
        if (!vNo) { mesaj('Lütfen Vergi No veya TC Kimlik No Giriniz!', 'warning'); return; }
        var mb = muhBirim.deger();
        Sgb.pencere('../HarcamaSurecleri/BorcuYokBelgesi?' + $.param({ muhasebe: mb.muhasebe || '', birim: mb.birim, belgeYil: yil(), belgeNo: belgeNo(), vNo: vNo }), 'Borcu Yoktur Belgesi');
    });

    // eski TanimAciklamaAc / AciklamaBilgiYaz
    window.AciklamaBilgiYaz = function (aciklama) { $('#txtAciklama').val(aciklama); };
    $('#btnAciklamaListesi').on('click', function () {
        var mb = muhBirim.deger();
        Sgb.pencere('TanimAciklama?' + $.param({ muhasebe: mb.muhasebe || '', birim: mb.birim, tip: 1 }), 'Açıklama Listesi');
    });

    // Süreç Bağı Kur / süreci aç
    $('#btnSurecBagKur').on('click', function () {
        var mb = muhBirim.deger();
        Sgb.gonder(UC + 'surec-bag', { Yil: yil(), BelgeNo: belgeNo(), SurecNo: $('#txtSurecNo').val(), Muhasebe: mb.muhasebe, Birim: mb.birim })
            .done(function (v) {
                if (v.basarili === false || v.hata) { mesaj(v.hata || 'İşlem yapılamadı.', 'danger'); return; }
                if (v.ihaleKod !== undefined) $('#ddlIhaleKod').val(v.ihaleKod || '');
                if (v.alimTur !== undefined) $('#ddlAlimTur').val(v.alimTur || '');
                mesaj(v.bilgi || 'Süreç ilişkisi kuruldu', 'success');
            }).fail(hataGoster);
    });
    $('#btnSurecAc').on('click', function () {
        var s = $.trim($('#txtSurecNo').val()).split('/');
        if (s.length !== 2) { mesaj('Süreç no bulunamadı!', 'warning'); return; }
        Sgb.pencere('../GelirSurecleri/SurecFormAna.aspx?' + $.param({ tur: 1, yil: $.trim(s[0]), surecNo: $.trim(s[1]) }), 'Süreç');
    });

    // eski SurecFaturaEkraniAc / SurecFaturaSecKapatBasildi
    $('#btnFaturaSec').on('click', function () {
        var mb = muhBirim.deger();
        Sgb.oem.surecFaturaSec({
            yil: yil(), ilgiliAd: $('#txtIlgiliAd').val(), ilgiliNo: $('#txtIlgiliNo').val(), muhasebe: mb.muhasebe, birim: mb.birim,
            secilince: function (f) {
                $('#txtOEBTur').val('Fatura');
                $('#txtOEBNo').val(f.faturaNo);
                var t = /^(\d{2})\.(\d{2})\.(\d{4})/.exec(f.faturaTarih || '');
                $('#txtOEBTarih').val(t ? t[3] + '-' + t[2] + '-' + t[1] : '');
                $('#txtSurecNo').val(f.surecYilNo);
                $('#txtOEBTutar').val(String(f.tutarKDVli || '').replace('.', ','));
            }
        });
    });

    // İlgili belge bağı (yalnız sistem yöneticisi; eski btnIlgiliBelgeYaz)
    $('#btnIlgiliBelgeYaz').on('click', function () {
        Sgb.gonder(UC + 'ilgili-belge', { Yil: yil(), BelgeNo: belgeNo(), IlgiliBelgeNo: $('#txtIlgiliBelgeNo').val() })
            .done(function (v) { sonucIsle(v); }).fail(hataGoster);
    });

    // eski TaahhutEkraniAc / TaahhutEkraniBilgiYaz / Ekrana921ve920Yaz
    $('#btnTaahhutSec').on('click', function () {
        var vergiNo = $.trim($('#txtIlgiliNo').val());
        if (!vergiNo) { mesaj('Önce ilgilinin T.C./Vergi No bilgisini yazınız.', 'warning'); return; }
        Sgb.oem.taahhutSec({
            yil: yil(), vergiNo: vergiNo,
            secilince: function (kod) {
                Sgb.al(UC + 'taahhut', { kod: kod }).done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    if (v.projeNo) $('#txtProjeNo').val(v.projeNo);
                    satir921ve920Yaz(v);
                }).fail(hataGoster);
            }
        });
    });
    function satir921ve920Yaz(bilgi) {
        var borc = 0, sil = [];
        grid.forEachNode(function (n) {
            var h = String(n.data.HESAPKOD || '');
            if (h.substring(0, 2) === '25' || h.substring(0, 3) === '150' || h.substring(0, 3) === '630' || h.substring(0, 2) === '16') borc += Number(n.data.BORC) || 0;
            else if (h.substring(0, 3) === '920' || h.substring(0, 3) === '921') sil.push(n.data);
        });
        if (sil.length) grid.applyTransaction({ remove: sil });
        var dolu = 0;
        grid.forEachNode(function (n) { if (n.data.HESAPKOD) dolu++; });
        if (grid.getDisplayedRowCount() - dolu < 2) grid.applyTransaction({ add: [bosSatir(), bosSatir()] });
        var ilk = grid.getDisplayedRowAtIndex(0);
        var r921 = grid.getDisplayedRowAtIndex(dolu), r920 = grid.getDisplayedRowAtIndex(dolu + 1);
        var db = ilk ? ilk.data.DETAYBIRIM : '';
        $.extend(r921.data, { HESAPKOD: '921', DETAYBIRIM: db, TERTIP: '', TERTIPPRG: '', BORC: yuvarla(borc), ALACAK: 0 });
        $.extend(r920.data, { HESAPKOD: bilgi.hesapKod || '', DETAYBIRIM: db, TERTIP: bilgi.tertip || '', ALTHESAPKOD: $('#txtIlgiliNo').val(), BORC: 0, ALACAK: yuvarla(borc) });
        grid.refreshCells({ force: true });
        toplamYaz();
    }

    // ---------------------------------------------------------------- OYBS

    var oybsGrid = null;
    function oybsKilit(evet) {
        oybs.kilitli = evet;
        $('#lblOybsKilit').prop('hidden', !evet);
        $('#grdOybs').toggleClass('oem-kilitli', evet);
    }
    function oybsKur() {
        oybsGrid = Sgb.grid('#grdOybs', {
            columnDefs: [
                { field: 'MIKTAR', headerName: 'Miktar', width: 120, type: 'rightAligned', cellClass: 'oem-sari' },
                { field: 'VADEMIKTAR', headerName: 'Vade Sonu Miktar', width: 120, type: 'rightAligned', hide: true },
                { field: 'PARABIRIMI', headerName: 'Para Birimi', width: 90, cellClass: 'oem-sari' },
                { field: 'TARIH', headerName: 'Tarih', width: 140, cellClass: 'oem-sari' },
                { field: 'BITISTARIH', headerName: 'Vade Bitiş Tarih', width: 140, hide: true },
                { field: 'KISIKURUM', headerName: 'Kişi / Kurum', width: 160, hide: true },
                { field: 'BANKAHESAP', headerName: 'Banka Hesap', width: 180, cellClass: 'oem-sari' },
                { field: 'TANIM', headerName: 'Tanım', flex: 1, minWidth: 160, cellClass: 'oem-sari' }
            ],
            rowHeight: 26, headerHeight: 30,
            getRowId: function (p) { return p.data.IDSET; },
            rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true, isRowSelectable: function () { return !oybs.kilitli; } },
            onSelectionChanged: function () {
                if (oybs.kilitli) return;
                oybs.secili = $.map(oybsGrid.getSelectedRows(), function (r) { return r.IDSET + ';'; }).join('');
            }
        });
    }
    function oybsOku(durum) {
        if (!oybsGrid) oybsKur();
        oybsGrid.setGridOption('loading', true);
        var tip = $('#ddlIslemTip').val();
        Sgb.al(UC + 'oybs/liste', { yil: yil(), belgeNo: belgeNo(), durum: durum == null ? belge.durum : durum, islemTip: tip === '' ? undefined : tip })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                oybsKilit(!!v.kilitli);
                if (v.kilitli) {
                    oybs.secili = v.oybs || '';
                    if (v.islemTip !== undefined) $('#ddlIslemTip').val(String(v.islemTip));
                }
                $('#btnOybsListele').prop('hidden', v.listele === false);
                oybsGrid.setGridOption('rowData', v.satirlar || []);
                if (v.kilitli) oybsGrid.forEachNode(function (n) { n.setSelected(false); });
            }).fail(hataGoster).always(function () { oybsGrid.setGridOption('loading', false); });
    }
    $('#btnOybsListele').on('click', function () {
        oybs.secili = '';
        if ($('#ddlIslemTip').val() === '') { mesaj('İşlem tipini seçiniz.', 'warning'); return; }
        oybsOku();
    });

    // ---------------------------------------------------------------- ekli dokümanlar

    var dokumanGrid = Sgb.grid('#grdDokuman', {
        columnDefs: [
            { headerName: '#', valueGetter: 'node.rowIndex + 1', width: 50, cellClass: 'oem-sari' },
            { field: 'DOKUMANADI', headerName: 'Dokümanlar', flex: 1, cellClass: 'oem-sari' },
            {
                headerName: '', width: 70, sortable: false, cellRenderer: function (p) {
                    return $('<span class="oem-hucre-sec"></span>')
                        .append(hucreTusD('glyphicon-eye-open', 'Dokümanı Göster', function () {
                            Sgb.indir(UC + 'dokuman/indir', { yil: p.data.YIL, belgeNo: p.data.BELGENO, siraNo: p.data.SIRANO });
                        }))
                        .append(hucreTusD('glyphicon-trash', 'Dokümanı Sil', function () {
                            sor('Ekli doküman silinecek. Onaylıyor musunuz?', function () {
                                Sgb.gonder(UC + 'dokuman/sil', { Yil: p.data.YIL, BelgeNo: p.data.BELGENO, SiraNo: p.data.SIRANO })
                                    .done(function (v) { sonucIsle(v, dokumanOku); }).fail(hataGoster);
                            }, { baslik: 'Uyarı' });
                        }))[0];
                }
            }
        ],
        rowHeight: 26, headerHeight: 30
    });
    function hucreTusD(simge, baslik, ne) {
        return $('<button type="button" class="btn btn-link btn-xs oem-mercek"></button>').attr('title', baslik)
            .append($('<span class="glyphicon" aria-hidden="true"></span>').addClass(simge)).append($('<span class="sr-only"></span>').text(baslik))
            .on('click', function (e) { e.stopPropagation(); ne(); });
    }
    function dokumanYaz(liste) {
        dokumanGrid.setGridOption('rowData', liste || []);
        var n = (liste || []).length;
        $('#lblDokumanSay').text(n).prop('hidden', !n);
    }
    function dokumanOku() {
        if (!belgeNo()) { dokumanYaz([]); return; }
        Sgb.al(UC + 'dokuman/liste', { yil: yil(), belgeNo: belgeNo() }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            dokumanYaz(v.satirlar);
        }).fail(hataGoster);
    }

    function dosyaGonder(uc, form) {
        return $.ajax({
            url: '../' + uc, method: 'POST', data: form, processData: false, contentType: false, dataType: 'json',
            headers: { RequestVerificationToken: $('meta[name="istek-jetonu"]').attr('content') || '' }
        });
    }
    $('#fileDokuman').on('change', function () {
        var f = this.files && this.files[0];
        var kutu = this;
        if (!f) return;
        if (!belgeNo()) { mesaj('Belgeyi kayıt ettikten sonra dokuman ekleme işlemini yapın.', 'warning'); kutu.value = ''; return; }
        var fd = new FormData();
        fd.append('yil', yil()); fd.append('belgeNo', belgeNo()); fd.append('dosya', f);
        dosyaGonder(UC + 'dokuman/yukle', fd).done(function (v) { sonucIsle(v, dokumanOku); }).fail(hataGoster)
            .always(function () { kutu.value = ''; });
    });

    // ---------------------------------------------------------------- dosyadan belge yükle

    var dlgDosya = document.getElementById('dlgDosyadanYukle');
    $('#btnDosyadanYukle').on('click', function () { dlgDosya.showModal(); });
    $('#fileListe').on('change', function () {
        var f = this.files && this.files[0];
        var kutu = this;
        if (!f) return;
        var fd = new FormData();
        fd.append('dosya', f);
        dosyaGonder(UC + 'dosyadan-yukle', fd).done(function (v) {
            sonucIsle(v, function () {
                formTemizle(false);
                satirlariYaz(v.satirlar);
                dlgDosya.close();
                // yüklenen hesap kodlarının adları ve bakiye / döviz denetimleri
                grid.forEachNode(function (n) { if (n.data.HESAPKOD && !n.data.HESAPAD) hesapAdiYaz(n); });
            });
        }).fail(hataGoster).always(function () { kutu.value = ''; });
    });
    function hesapAdiYaz(node) {
        var kod = kodAyir(node.data.HESAPKOD);
        Sgb.al(HESAP_UC + 'ad', { yil: yil(), kod: kod }).done(function (v) {
            if (!v.ad) return;
            node.data.HESAPKOD = v.kod + '-' + v.ad;
            node.data.HESAPAD = v.ad;
            yenile(node);
        });
    }

    // ---------------------------------------------------------------- yansıtma

    var yansitmaGrid = null, yansitmaHesaplari = [];
    var dlgYansitma = document.getElementById('dlgYansitma');
    function yansitmaKur() {
        yansitmaGrid = Sgb.grid('#grdYansitma', {
            columnDefs: [
                { field: 'HESAPAD', headerName: 'Hesap Kodu', width: 260, cellClass: 'oem-sari' },
                {
                    field: 'YANSITMA', headerName: 'Yansıtma Hesap Kodu', flex: 1, minWidth: 260, editable: true, cellClass: 'oem-girilir',
                    cellEditor: 'agSelectCellEditor',
                    cellEditorParams: function (p) {
                        var kod = p.data.HESAPKOD;
                        return { values: [''].concat($.map($.grep(yansitmaHesaplari, function (h) { return h.HESAPKOD === kod; }), function (h) { return h.YANSITMAHESAPAD; })) };
                    }
                },
                { field: 'TERTIP', headerName: 'Tertip', width: 200, cellClass: 'oem-sari', hide: true },
                { field: 'BORC', headerName: 'Borç', width: 110, type: 'rightAligned', cellClass: 'oem-sari', valueFormatter: function (p) { return paraYaz(p.value); } },
                { field: 'ALACAK', headerName: 'Alacak', width: 110, type: 'rightAligned', cellClass: 'oem-sari', valueFormatter: function (p) { return paraYaz(p.value); } }
            ],
            rowHeight: 26, headerHeight: 30, singleClickEdit: true, stopEditingWhenCellsLoseFocus: true
        });
    }
    $('#btnYansitmaGoster').on('click', function () {
        grid.stopEditing();
        var $t = $(this);
        yukleniyor($t, true);
        Sgb.gonder(UC + 'yansitma/liste', { Yil: yil(), Satirlar: satirDegerler() }).done(function (v) {
            if (v.hata) { mesaj(v.hata, v.hata.indexOf('bulunamadı') >= 0 ? 'info' : 'danger'); return; }
            if (!yansitmaGrid) yansitmaKur();
            yansitmaHesaplari = v.yansitmaHesaplari || [];
            dlgYansitma.showModal();
            yansitmaGrid.setGridOption('rowData', v.satirlar || []);
        }).fail(hataGoster).always(function () { yukleniyor($t, false); });
    });
    $('#btnYansitmaYap').on('click', function () {
        yansitmaGrid.stopEditing();
        var yansitmalar = [];
        yansitmaGrid.forEachNode(function (n) { yansitmalar.push(n.data); });
        var $t = $(this);
        yukleniyor($t, true);
        Sgb.gonder(UC + 'yansitma/yap', { Yil: yil(), Satirlar: satirDegerler(), Yansitmalar: yansitmalar }).done(function (v) {
            sonucIsle(v, function () { satirlariYaz(v.satirlar); dlgYansitma.close(); });
        }).fail(hataGoster).always(function () { yukleniyor($t, false); });
    });

    // ---------------------------------------------------------------- şablonlar

    var sablonGrid = null, sablonAcGrid = null;
    var dlgSablonKaydet = document.getElementById('dlgSablonKaydet');
    var dlgSablonAc = document.getElementById('dlgSablonAc');
    var sablonSecili = '';

    function sablonListesi(tur, hedef) {
        hedef.setGridOption('loading', true);
        var p = { tur: tur };
        if (tur !== 'kaydet') { p.benim = $('input[name=sablonTur]:checked').val() === '1' ? 1 : 0; p.muhasebe = muhBirim.deger().muhasebe; }
        return Sgb.al(UC + 'sablon/liste', p).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            hedef.setGridOption('rowData', v.satirlar || []);
        }).fail(hataGoster).always(function () { hedef.setGridOption('loading', false); });
    }

    function sablonSil(kod, bitince) {
        sor('Seçili Şablon silinecek. Onaylıyor musunuz?', function () {
            Sgb.gonder(UC + 'sablon/sil', { Kod: kod }).done(function (v) { sonucIsle(v, bitince); }).fail(hataGoster);
        }, { baslik: 'Uyarı' });
    }

    $('#btnSablonKaydet').on('click', function (e) {
        e.preventDefault();
        $('.oem-arac .btn-group.open').removeClass('open');
        if (!sablonGrid) {
            sablonGrid = Sgb.grid('#grdSablon', {
                columnDefs: [
                    { field: 'AD', headerName: 'Şablon Adı', flex: 1, cellClass: 'oem-sari' },
                    {
                        headerName: '', width: 44, sortable: false, cellRenderer: function (p) {
                            return $('<span class="oem-hucre-sec"></span>').append(hucreTusD('glyphicon-trash', 'Şablon Sil', function () {
                                sablonSil(p.data.KOD, function () { sablonListesi('kaydet', sablonGrid); });
                            }))[0];
                        }
                    }
                ],
                rowHeight: 26, headerHeight: 30,
                rowSelection: { mode: 'singleRow', checkboxes: true, enableClickSelection: true },
                onSelectionChanged: function () {
                    var r = sablonGrid.getSelectedRows()[0];
                    sablonSecili = r ? r.KOD : '';
                    if (r) $('#txtSablonAd').val(r.AD);
                }
            });
        }
        sablonSecili = '';
        dlgSablonKaydet.showModal();
        sablonListesi('kaydet', sablonGrid);
    });
    $('input[name=sablonKaydet]').on('change', function () {
        $('#grdSablon').prop('hidden', $('input[name=sablonKaydet]:checked').val() !== 'uzerine');
    });
    $('#btnSablonKaydetYap').on('click', function () {
        grid.stopEditing();
        sor('Şablon kayıt edilecek. Onaylıyor musunuz?', function () {
            var mb = muhBirim.deger();
            Sgb.gonder(UC + 'sablon/kaydet', {
                Yil: yil(), Kod: sablonSecili, Ad: $('#txtSablonAd').val(),
                Izin: parseInt($('input[name=sablonIzin]:checked').val(), 10) || 1,
                Uzerine: $('input[name=sablonKaydet]:checked').val() === 'uzerine',
                Muhasebe: mb.muhasebe, Birim: mb.birim, Kurumsal: mb.kurumsal,
                IlgiliAd: $('#txtIlgiliAd').val(), IlgiliNo: $('#txtIlgiliNo').val(), IlgiliVD: $('#txtIlgiliVD').val(),
                IlgiliBankaAd: $('#txtIlgiliBankaAd').val(), IlgiliBankaNo: $('#txtIlgiliBankaNo').val(), Aciklama: $('#txtAciklama').val(),
                Satirlar: satirDegerler()
            }).done(function (v) { sonucIsle(v, function () { dlgSablonKaydet.close(); }); }).fail(hataGoster);
        });
    });

    function sablonAc(kod) {
        Sgb.al(UC + 'sablon/ac', { kod: kod }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            formTemizle(false);
            muhBirim.ata({ muhasebe: v.muhasebe, birim: v.birim, kurumsal: v.kurumsal });
            muhBirim.yenile();
            $('#txtIlgiliAd').val(v.ilgiliAd); $('#txtIlgiliNo').val(v.ilgiliNo); $('#txtIlgiliVD').val(v.ilgiliVD);
            $('#txtIlgiliBankaAd').val(v.ilgiliBankaAd); $('#txtIlgiliBankaNo').val(v.ilgiliBankaNo); $('#txtAciklama').val(v.aciklama);
            satirlariYaz(v.satirlar);
            dlgSablonAc.close();
        }).fail(hataGoster);
    }
    $('#btnSablonAc').on('click', function (e) {
        e.preventDefault();
        $('.oem-arac .btn-group.open').removeClass('open');
        if (!sablonAcGrid) {
            sablonAcGrid = Sgb.grid('#grdSablonAc', {
                columnDefs: [
                    {
                        headerName: '', width: 44, sortable: false, cellRenderer: function (p) {
                            return $('<span class="oem-hucre-sec"></span>').append(hucreTusD('glyphicon-folder-open', 'Şablon Aç', function () { sablonAc(p.data.KOD); }))[0];
                        }
                    },
                    { field: 'AD', headerName: 'Şablon Adı', flex: 1, cellClass: 'oem-sari' }
                ],
                rowHeight: 26, headerHeight: 30,
                rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
                onSelectionChanged: function () { $('#btnSablonSil').prop('disabled', sablonAcGrid.getSelectedRows().length === 0); },
                onRowDoubleClicked: function (e) { sablonAc(e.data.KOD); }
            });
        }
        dlgSablonAc.showModal();
        sablonListesi('acma', sablonAcGrid);
    });
    $('input[name=sablonTur]').on('change', function () { if (sablonAcGrid) sablonListesi('acma', sablonAcGrid); });
    $('#btnSablonSil').on('click', function () {
        var r = sablonAcGrid.getSelectedRows()[0];
        if (!r) return;
        sablonSil(r.KOD, function () { sablonListesi('acma', sablonAcGrid); });
    });

    // ---------------------------------------------------------------- kur farkı

    var dlgKurFarki = document.getElementById('dlgKurFarki');
    var kurHesap = null;
    $('#btnKurFarki').on('click', function () {
        if (!kurHesap && Sgb.hesapKodu) kurHesap = Sgb.hesapKodu('#txtKurHesap', { yil: '#txtKurYil', uc: HESAP_UC, ad: true });
        dlgKurFarki.showModal();
    });
    $('#btnKurFarkiBaslat').on('click', function () {
        if (!$.trim($('#txtKurAciklama').val())) { mesaj('Açıklama alanı boş bırakılamaz', 'warning'); return; }
        sor('Kur farkı işlemi başlatılacaktır. Onaylıyor musunuz?', function () {
            var $t = $('#btnKurFarkiBaslat');
            yukleniyor($t, true);
            Sgb.gonder(UC + 'kur-farki', {
                Yil: parseInt($('#txtKurYil').val(), 10) || yil(), KurTarih: $('#txtKurTarih').val(), HesapKod: $('#txtKurHesap').val(),
                Aciklama: $('#txtKurAciklama').val(), BelgeNo: $('#txtKurBelgeNo').val(), MuhasebeBirimineIsle: $('#chkMuhasebeBirimineIsle').prop('checked')
            }).done(function (v) { sonucIsle(v, function () { dlgKurFarki.close(); }); }).fail(hataGoster).always(function () { yukleniyor($t, false); });
        });
    });

    // ---------------------------------------------------------------- yerleşim

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
    function gridYay() { yuksekligeYay(document.getElementById('grdListe'), 220); }
    $(window).on('resize', gridYay);

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    function secenekYaz($s, liste, bos) {
        $s.empty();
        if (bos) $('<option value=""></option>').appendTo($s);
        $.each(liste || [], function (i, x) { $('<option></option>').val(x.kod).text(x.ad).appendTo($s); });
    }

    Sgb.al(UC + 'tanim').done(function (v) {
        if (v.hata) { mesaj(v.hata, 'danger'); return; }
        tanim = v;
        Sgb.yilKutusu('#txtYil', [v.yil - 10, v.yil + 1], sayfa.yil || v.yil);
        Sgb.yilKutusu('#txtKurYil', [v.yil - 10, v.yil + 1], v.yil);
        $('#txtBelgeTarih, #txtKurTarih').val(v.bugun);

        secenekYaz($('#ddlFisTur'), v.fisTurler, false);
        secenekYaz($('#ddlIhaleKod'), v.ihaleler, true);
        if (v.ist1) { $('#grpIst1').prop('hidden', false); $('#lblIst1').text(v.ist1.ad); secenekYaz($('#ddlIst1'), [{ kod: 0, ad: '' }].concat(v.ist1.degerler), false); }
        if (v.ist2) { $('#grpIst2').prop('hidden', false); $('#lblIst2').text(v.ist2.ad); secenekYaz($('#ddlIst2'), [{ kod: 0, ad: '' }].concat(v.ist2.degerler), false); }

        $('#btnTarihce').prop('hidden', !v.tarim);
        $('#btnIlgiliBelgeYaz').prop('hidden', !v.yonetici);
        $('#grpPrg').prop('hidden', !v.prgBag);
        if (v.prgBag && v.cumhurbaskanligi) $('#chkPrg').prop('checked', true);
        $('#btnYazdir').prop('hidden', v.yazdirTercih);
        $('#grpYazdirTercih').prop('hidden', !v.yazdirTercih);
        $('#sekmeOybs').prop('hidden', !v.oybs);
        $('#btnKurFarki').prop('hidden', !v.doviz);
        $('#btnYansitmaGoster').prop('hidden', v.tertipYok);

        grid.setColumnsVisible(['TERTIPPRG'], v.prgBag && !v.tertipYok);
        grid.setColumnsVisible(['TERTIP'], !v.tertipYok);
        grid.setColumnsVisible(['DETAYBIRIM'], !!v.detayBirim);
        grid.setColumnsVisible(['ALTHESAPKOD'], !v.altHesapGizle);
        grid.setColumnsVisible(['ISAD'], !v.isGizle);
        grid.setColumnsVisible(['SATIRACIKLAMA'], !!v.satirAciklama);
        grid.setColumnsVisible(['MAHSUPBELGENO'], !v.mahsupGizle);
        grid.setColumnsVisible(['DOVIZ', 'DOVIZKURTUR', 'DOVIZBORC', 'DOVIZALACAK'], !!v.doviz);

        if (v.muhasebe) { muhBirim.ata({ muhasebe: v.muhasebe, birim: v.birim, kurumsal: v.kurumsal }); muhBirim.yenile(); }

        if (sayfa.baslik) $('#bolumSatir').prop('hidden', true);
        satirlariYaz([]);
        gridYay();

        if (sayfa.belgeNo) belgeOku(sayfa.yil || v.yil, sayfa.belgeNo);
        if (v.yazdirTercih) imzaListesiOku();
    }).fail(hataGoster);
});
