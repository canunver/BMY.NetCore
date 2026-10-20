// Muhasebat aktarım belgesi, ButceMuhasebe/MuhasebatAktarimTCCB (eski MuhasebatAktarimTCCB.aspx): Muhasebat Aktarım çerçevesinin
// "Belge Düzenleme" sekmesi. Listele mizandan belge satırlarını oluşturur (eski ListeleMizan_Click); Kaydet, Temizle, Yazdır (Excel),
// Onayla, Onay Kaldır, İptal Et, Muhasebat'a Gönder eski tuşlardır. Belge No + mercek / Enter kayıtlı belgeyi okur (eski btnListele_Click).
// Uçlar: /api/muhasebataktarim/* (Muhasebe/MuhasebatAktarimUclari.cs). Muhasebe - birim Sgb.muhasebeBirim, tertip Sgb.tertipSec,
// hesap planı Sgb.hesapPlaniSec. Belge Arama sekmesinden belge açılınca window.MaBelgeYukle(yil, belgeNo) çağrılır.
$(function () {
    'use strict';

    if (!$('.ma-form').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/muhasebataktarim/';
    var HESAP_UC = UC + 'hesap/';
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var tanim = null;   // /tanim yanıtı
    var satirNo = 0;    // satır kimliği (_id)
    var detayBirimler = [];

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
    function bugunYil() { return new Date().getFullYear(); }
    function yil() { return parseInt($('#txtYil').val(), 10) || 0; }
    function belgeNo() { return $.trim($('#txtBelgeNo').val()); }
    function kodAyir(s) { return $.trim(String(s || '').split('-')[0]); }
    function sayiAl(sec) { return parseInt($(sec).val(), 10) || 0; }

    // İşlem sürerken tuşlar kapalı (eski EventMask).
    var mesgul = false;
    function mesgulYap(evet) {
        mesgul = evet;
        $('.ma-arac button, #btnListele, #btnMizan').prop('disabled', evet);
        $('body').toggleClass('avans-bekle', evet);
    }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); gridYay(); });

    // ---------------------------------------------------------------- muhasebe - birim (eski hdnMuhasebe / hdnBirim + mercek)

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: function () { return yil() || bugunYil(); },
        kurumsalGizle: true,
        degisince: function () { detayBirimleriOku(); } // eski txtBirim_Changed → SaymanlikBirimDoldur
    });

    function detayBirimleriOku() {
        var m = muhBirim.deger().muhasebe;
        if (!m) return;
        Sgb.al(UC + 'detay-birimler', { muhasebe: m }).done(function (v) {
            if (v.hata) return;
            if ((v.birimler || []).length) detayBirimler = v.birimler;
            grid.refreshCells({ columns: ['DETAYBIRIM'], force: true });
        });
    }

    // ---------------------------------------------------------------- belge satırları

    function bosSatir() {
        return { _id: ++satirNo, SIRANO: 0, HESAPKOD: '', HESAPAD: '', TERTIP: '', BORC: 0, ALACAK: 0, ALTHESAPKOD: '', SATIRACIKLAMA: '', DETAYBIRIM: '' };
    }

    function hucreTus(simge, baslik, ne) {
        return $('<button type="button" class="btn btn-link btn-xs ma-mercek"></button>')
            .attr('title', baslik).append($('<span class="glyphicon" aria-hidden="true"></span>').addClass(simge))
            .append($('<span class="sr-only"></span>').text(baslik))
            .on('click', function (e) { e.stopPropagation(); grid.stopEditing(); ne(); });
    }

    // Hücre metni + mercek (eski ImageCommand)
    function tusluHucre(simge, baslik, ne) {
        return function (p) {
            var $h = $('<span class="ma-hucre-sec"></span>');
            $('<span class="ma-hucre-metin"></span>').text(p.valueFormatted != null ? p.valueFormatted : (p.value == null ? '' : p.value)).appendTo($h);
            $h.append(hucreTus(simge, baslik, function () { ne(p.node); }));
            return $h[0];
        };
    }

    // Yazdıkça arama düzenleyicisi (eski hesap kodu ComboBox, MinChars=2, PageSize=20): en az iki karakter yazılınca eşleşen hesaplar
    // hücrenin altında listelenir; ok tuşlarıyla gezilir, Enter / tıklama ile seçilir, Esc listeyi kapatır.
    var oneriAcik = false;
    function OneriDuzenleyici() { }
    OneriDuzenleyici.prototype.init = function (p) {
        var self = this;
        self.p = p;
        self.$in = $('<input type="text" class="ag-input-field-input ma-oneri-girdi" autocomplete="off" />');
        self.$in.val(p.eventKey && p.eventKey.length === 1 ? p.eventKey : (p.value == null ? '' : p.value));
        self.$liste = $('<div class="ma-oneri-liste" role="listbox" hidden></div>').appendTo(document.body);
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
        self.$liste.on('mousedown', '.ma-oneri-satir', function (e) { e.preventDefault(); self.sec($(this).index()); });
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
                $('<div class="ma-oneri-satir" role="option"></div>').text(s.metin).attr('title', s.metin).appendTo(self.$liste);
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
        return Sgb.al(HESAP_UC + 'ara', { yil: yil() || bugunYil(), metin: metin, sonKirilim: '1' }).then(function (v) {
            return $.map(v.satirlar || [], function (h) { return { deger: h.kod + '-' + (h.hesapAd || ''), metin: h.ad }; }).slice(0, 50);
        }, function () { return $.Deferred().resolve([]).promise(); });
    }

    function paraSutun(alan, ad) {
        return {
            field: alan, headerName: ad, width: 130, type: 'rightAligned', editable: true, sortable: false,
            cellClass: ['ma-girilir', 'ag-right-aligned-cell'],
            valueFormatter: function (p) { return p.node && p.node.rowPinned ? paraYaz(p.value || 0) : (p.value ? paraYaz(p.value) : ''); },
            valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? 0 : yuvarla(n); }
        };
    }

    function birimAd(deger) {
        var b = $.grep(detayBirimler, function (x) { return x.kod === deger; })[0] ||
            $.grep(detayBirimler, function (x) { return x.kod && x.kod.split('.')[0] === String(deger || '').split('.')[0]; })[0];
        return b && b.ad ? b.ad : (deger || '');
    }

    var sutunlar = [
        { headerName: '#', valueGetter: function (p) { return p.node.rowPinned ? '' : p.node.rowIndex + 1; }, width: 50, sortable: false, cellClass: 'ma-sari', pinned: 'left' },
        {
            field: 'HESAPKOD', headerName: 'Hesap Kodu', width: 320, editable: true, sortable: false, cellClass: 'ma-girilir',
            cellRenderer: tusluHucre('glyphicon-search', 'Hesap Kodu Seç', hesapPlaniAc),
            cellEditor: OneriDuzenleyici, cellEditorParams: { ara: hesapOneri }, suppressKeyboardEvent: oneriTusu
        },
        {
            // Eski combo: değer BirimKurum (005.12345678), görünen BirimKodAd. Kurumda detay birim yoksa gizli (eski DetayBirimVarMi).
            field: 'DETAYBIRIM', colId: 'DETAYBIRIM', headerName: 'Birim', width: 220, editable: true, sortable: false, cellClass: 'ma-girilir', hide: true,
            cellEditor: 'agSelectCellEditor',
            cellEditorParams: function () { return { values: $.map(detayBirimler, function (b) { return b.kod; }) }; },
            valueFormatter: function (p) { return p.node && p.node.rowPinned ? '' : birimAd(p.value); }
        },
        {
            field: 'TERTIP', colId: 'TERTIP', headerName: 'Bütçe Tertibi', width: 280, editable: true, sortable: false, cellClass: 'ma-girilir',
            cellRenderer: tusluHucre('glyphicon-search', 'Seç', tertipAc), cellEditor: 'agTextCellEditor'
        },
        paraSutun('BORC', 'Borç'),
        paraSutun('ALACAK', 'Alacak')
    ];

    // Toplam satırı (eski başlık altındaki lblBorcToplam / lblAlacakToplam): düzenlenmez, mercek çizilmez.
    $.each(sutunlar, function (i, c) {
        if (c.editable === true) c.editable = function (p) { return !p.node.rowPinned; };
        var cizici = c.cellRenderer;
        if (cizici) c.cellRenderer = function (p) {
            if (!p.node.rowPinned) return cizici(p);
            return p.valueFormatted != null ? p.valueFormatted : (p.value == null ? '' : String(p.value));
        };
    });

    var toplam = { borc: 0, alacak: 0 };
    function toplamSatiri() {
        var fark = yuvarla(toplam.borc - toplam.alacak);
        return { _id: 'toplam', HESAPKOD: 'Toplam' + (fark ? '   (Fark: ' + paraYaz(fark) + ')' : ''), BORC: toplam.borc, ALACAK: toplam.alacak };
    }

    var aktifSatir = null; // eski AktifSatirBelirle
    var grid = Sgb.grid('#grdListe', {
        columnDefs: sutunlar,
        pinnedTopRowData: [{ _id: 'toplam', HESAPKOD: 'Toplam', BORC: 0, ALACAK: 0 }],
        getRowClass: function (p) { return p.node.rowPinned ? 'ma-toplam-satir' : ''; },
        rowHeight: 26,
        headerHeight: 30,
        getRowId: function (p) { return String(p.data._id); },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        suppressCellFocus: false,
        rowSelection: { mode: 'multiRow', checkboxes: false, headerCheckbox: false, enableClickSelection: true },
        onSelectionChanged: function () { $('#btnSatirSil').prop('disabled', grid.getSelectedNodes().length === 0); },
        onCellFocused: function (e) { if (e.rowIndex != null && !e.rowPinned) aktifSatir = grid.getDisplayedRowAtIndex(e.rowIndex); },
        onCellValueChanged: function (e) {
            if (e.newValue === e.oldValue) return;
            var f = e.colDef.field;
            if (f === 'BORC' || f === 'ALACAK') toplamYaz(); // eski HucreDegisti → BorcAlacakToplaYaz
            else if (f === 'HESAPKOD') hesapKodDegisti(e.node);
        },
        overlayNoRowsTemplate: '<span>Satır yok. Listele ile mizandan oluşturun ya da Satır Aç\'a basın.</span>'
    });

    function satirlar() {
        var r = [];
        grid.forEachNode(function (n) { r.push(n.data); });
        return r;
    }

    // Sunucuya giden satırlar (eski SatirDegerler): eski alan adlarıyla.
    function satirDegerler() {
        return $.map(satirlar(), function (s) {
            var d = $.extend({}, s);
            delete d._id;
            return d;
        });
    }

    function satirlariYaz(liste) {
        satirNo = 0;
        grid.setGridOption('rowData', $.map(liste || [], function (s) { return $.extend(bosSatir(), s); }));
        toplamYaz();
    }

    // eski BorcAlacakToplaYaz
    function toplamYaz() {
        toplam = { borc: 0, alacak: 0 };
        var dolu = 0;
        grid.forEachNode(function (n) {
            toplam.borc += Number(n.data.BORC) || 0;
            toplam.alacak += Number(n.data.ALACAK) || 0;
            if (n.data.HESAPKOD) dolu++;
        });
        toplam.borc = yuvarla(toplam.borc);
        toplam.alacak = yuvarla(toplam.alacak);
        grid.setGridOption('pinnedTopRowData', [toplamSatiri()]);
        $('#lblOzet').text(dolu ? dolu + ' satır' : '');
    }

    function yenile(node) { grid.refreshCells({ rowNodes: [node], force: true }); }

    // Elle yazılan hesap kodu "kod-ad" biçimine getirilir.
    function hesapKodDegisti(node) {
        var kod = kodAyir(node.data.HESAPKOD).replace(/\s/g, '');
        if (!kod) { node.data.HESAPAD = ''; yenile(node); toplamYaz(); return; }
        Sgb.al(HESAP_UC + 'ad', { yil: yil() || bugunYil(), kod: kod }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            if (!v.ad) { mesaj(kod + ' kodlu hesap bulunamadı.', 'warning'); node.data.HESAPKOD = kod; yenile(node); return; }
            node.data.HESAPKOD = v.kod + '-' + v.ad;
            node.data.HESAPAD = v.ad;
            yenile(node);
            toplamYaz();
        }).fail(hataGoster);
    }

    // Hesap kodu mercek (eski ImageCommand HesapKodSec)
    function hesapPlaniAc(node) {
        Sgb.hesapPlaniSec({
            yil: yil() || bugunYil(), sonKirilim: true, uc: HESAP_UC,
            secilince: function (h) {
                node.data.HESAPKOD = h.kod + '-' + (h.hesapAd || '');
                node.data.HESAPAD = h.hesapAd || '';
                yenile(node);
                toplamYaz();
            }
        });
    }

    // eski TertipGosterX: ButceMuhasebe/ListeTertip (eko4=1, secKapat=1); seçilen tertip bu satıra yazılır.
    function tertipAc(node) {
        var d = node.data, mb = muhBirim.deger();
        var hesapKod = kodAyir(d.HESAPKOD);
        if (!hesapKod || hesapKod === '1') { mesaj('Lütfen ilk önce hesap kodunu seçin.', 'warning'); return; }
        var birim = mb.birim;
        if (d.DETAYBIRIM) { var db = String(d.DETAYBIRIM).split('.'); if (db.length && db[0] !== '000' && db[0] !== '') birim = db[0]; }
        Sgb.tertipSec({
            tur: 'muh', yil: yil() || bugunYil(), muhasebe: mb.muhasebe, birim: birim, hesapKod: hesapKod, secKapat: true, coklu: false,
            secilince: function (liste) {
                var t = liste && liste[0];
                if (!t) return;
                node.data.TERTIP = t.TERTIP || '';
                yenile(node);
            }
        });
    }

    // eski SatirAc: 20 boş satır
    $('#btnSatirAc').on('click', function () {
        var r = [];
        for (var i = 0; i < ((tanim && tanim.satirSayisi) || 20); i++) r.push(bosSatir());
        grid.applyTransaction({ add: r });
        toplamYaz();
        gridYay();
    });
    // eski SatirAcAraya: seçili satırın önüne bir satır
    $('#btnSatirAcAraya').on('click', function () {
        var secili = grid.getSelectedNodes()[0] || aktifSatir;
        var idx = secili && secili.rowIndex != null ? secili.rowIndex : 0;
        grid.applyTransaction({ add: [bosSatir()], addIndex: idx });
    });
    // eski deleteSelected
    $('#btnSatirSil').on('click', function () {
        var sec = grid.getSelectedRows();
        if (!sec.length) return;
        grid.applyTransaction({ remove: sec });
        $('#btnSatirSil').prop('disabled', true);
        toplamYaz();
    });

    // ---------------------------------------------------------------- form

    function fisTurleriYaz() {
        var $f = $('#ddlFisTur').empty();
        $('<option value=""></option>').appendTo($f);
        $.each((tanim && tanim.fisTurler) || [], function (i, f) { $('<option></option>').val(f.kod).text(f.ad).appendTo($f); });
    }

    function durumYaz(ad) { $('#lblDurum').text(ad ? '(' + ad + ')' : ''); }

    // eski FormTemizle
    function formTemizle() {
        grid.stopEditing(true);
        satirlariYaz([]);
        $('#txtBelgeTarih').val(tanim ? tanim.belgeTarih : '');
        $('#txtBelgeNo').val('');
        $('#txtYil').val(tanim ? tanim.yil : bugunYil());
        if (tanim) muhBirim.ata({ muhasebe: tanim.muhasebe, birim: tanim.birim });
        muhBirim.yenile();
        durumYaz('');
        $('#txtKriterTarih1, #txtKriterTarih2, #txtYevmiyeNo1, #txtYevmiyeNo2, #txtAciklama').val('');
        $('#ddlFisTur').val('');
    }

    // eski BelgeFormaYaz
    function belgeFormaYaz(b) {
        grid.stopEditing(true);
        $('#txtYil').val(b.yil);
        $('#txtBelgeTarih').val(b.belgeTarih || '');
        $('#txtBelgeNo').val(b.belgeNo || '');
        muhBirim.ata({ muhasebe: b.muhasebe, birim: b.birim });
        muhBirim.yenile();
        $('#ddlFisTur').val(b.fisTur ? String(b.fisTur) : '');
        $('#txtAciklama').val(b.aciklama || '');
        $('#txtKriterTarih1').val(b.tarih1 || '');
        $('#txtKriterTarih2').val(b.tarih2 || '');
        $('#txtYevmiyeNo1').val(b.yevmiyeNo1 ? b.yevmiyeNo1 : '');
        $('#txtYevmiyeNo2').val(b.yevmiyeNo2 ? b.yevmiyeNo2 : '');
        durumYaz(b.durumAd);
        satirlariYaz(b.satirlar); // detay birimler muhBirim.yenile → degisince ile okunur
    }

    // eski btnListele_Click (Belge No mercek / Enter)
    function belgeOku(y, no) {
        if (mesgul) return;
        if (!no) { mesaj('Belge listelemesi için, Belge No alanına görmek istediğiniz belgenin nosunu yazmanız gerekmektedir.', 'warning'); return; }
        if (!y) { mesaj('Belge listelemesi için, Yıl alanına görmek istediğiniz belgenin yılını yazmanız gerekmektedir.', 'warning'); return; }
        mesgulYap(true);
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'belge', { yil: y, belgeNo: no })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata || 'Belge okunamadı.', 'danger'); return; }
                belgeFormaYaz(v.belge);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); mesgulYap(false); gridYay(); });
    }

    window.MaBelgeYukle = function (y, no) {
        $('#txtYil').val(y);
        $('#txtBelgeNo').val(no);
        belgeOku(parseInt(y, 10) || yil(), $.trim(no || ''));
    };

    $('#btnListele').on('click', function () { belgeOku(yil(), belgeNo()); });
    $('#txtBelgeNo').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); belgeOku(yil(), belgeNo()); } });

    $('#btnTemizle').on('click', function () { if (!mesgul) formTemizle(); });

    // eski btnKaydet_Click
    $('#btnKaydet').on('click', function () {
        if (mesgul) return;
        grid.stopEditing();
        if (!window.confirm('Kayıt işlemi yapılacak. Onaylıyor musunuz?')) return;
        var mb = muhBirim.deger();
        var istek = {
            Yil: yil(),
            BelgeTarih: $('#txtBelgeTarih').val() || '',
            BelgeNo: belgeNo(),
            Muhasebe: mb.muhasebe || 0,
            Birim: mb.birim || '',
            FisTur: parseInt($('#ddlFisTur').val(), 10) || 0,
            Aciklama: $('#txtAciklama').val() || '',
            Tarih1: $('#txtKriterTarih1').val() || '',
            Tarih2: $('#txtKriterTarih2').val() || '',
            YevmiyeNo1: sayiAl('#txtYevmiyeNo1'),
            YevmiyeNo2: sayiAl('#txtYevmiyeNo2'),
            Satirlar: satirDegerler()
        };
        mesgulYap(true);
        Sgb.gonder(UC + 'kaydet', istek)
            .done(function (v) {
                if (v.uyari) { mesaj(v.uyari, 'warning'); return; }
                if (!v.basarili) { mesaj(v.hata || 'Belge kaydı yapılamadı.', 'danger'); return; }
                if (!belgeNo() && v.belgeNo) $('#txtBelgeNo').val(v.belgeNo);
                mesaj(v.bilgi || 'Belge kaydedildi.', 'success');
            })
            .fail(hataGoster)
            .always(function () { mesgulYap(false); });
    });

    // eski BelgeDurumGuncelle: Onayla / Onay Kaldır / İptal Et / Muhasebat'a Gönder
    $('.ma-arac [data-islem]').on('click', function () {
        if (mesgul) return;
        if (!window.confirm('İşlem yapılacak. Onaylıyor musunuz?')) return;
        var islem = $(this).attr('data-islem');
        mesgulYap(true);
        Sgb.gonder(UC + 'durum', { Yil: yil(), BelgeNo: $('#txtBelgeNo').val() || '', Islem: islem })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata || 'İşlem yapılamadı.', 'danger'); return; }
                if (v.bos) return; // eski kod belge numarası boşsa sessizce dönerdi
                if (v.durumAd) durumYaz(v.durumAd);
                mesaj(v.bilgi || 'İşlem yapıldı.', 'success');
            })
            .fail(hataGoster)
            .always(function () { mesgulYap(false); });
    });

    // eski btnYazdir_Click
    $('#btnYazdir').on('click', function () {
        if (mesgul) return;
        var no = belgeNo(), y = yil();
        if (!no) { mesaj('Belge listelemesi için, Belge No alanına görmek istediğiniz belgenin nosunu yazmanız gerekmektedir.', 'warning'); return; }
        if (!y) { mesaj('Belge listelemesi için, Yıl alanına görmek istediğiniz belgenin yılını yazmanız gerekmektedir.', 'warning'); return; }
        mesgulYap(true);
        Sgb.indir(UC + 'yazdir', { yil: y, belgeNo: no }).then(function () { mesgulYap(false); }, function () { mesgulYap(false); });
    });

    // eski ListeleMizan_Click: seçilen dönemin mizanından belge satırları
    $('#btnMizan').on('click', function () {
        if (mesgul) return;
        if (!window.confirm('Listeleme işlemi yapılacak. Onaylıyor musunuz?')) return;
        var mb = muhBirim.deger();
        var istek = {
            Yil: yil(),
            Muhasebe: mb.muhasebe || 0,
            Birim: mb.birim || '',
            Tarih1: $('#txtKriterTarih1').val() || '',
            Tarih2: $('#txtKriterTarih2').val() || '',
            YevmiyeNo1: sayiAl('#txtYevmiyeNo1'),
            YevmiyeNo2: sayiAl('#txtYevmiyeNo2')
        };
        mesgulYap(true);
        grid.setGridOption('loading', true);
        Sgb.gonder(UC + 'mizan', istek)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata || 'Mizan okunamadı.', 'danger'); return; }
                satirlariYaz(v.satirlar);
                if (!(v.satirlar || []).length) mesaj('Verilen kriterlere uygun mizan kaydı bulunamadı.', 'info');
                // Eski DegiskenleriKaydet: bu muhasebe / birim bir sonraki açılışta gelir.
                if (tanim) { tanim.muhasebe = istek.Muhasebe ? String(istek.Muhasebe) : ''; tanim.birim = istek.Birim; }
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); mesgulYap(false); gridYay(); });
    });

    // ---------------------------------------------------------------- yerleşim ve açılış

    // Tablo sayfanın kalan yüksekliğini doldurur (alt araç çubuğu altta kalır).
    function gridYay() {
        var el = document.getElementById('grdListe');
        if (!el || el.offsetParent === null) return;
        var ust = el.getBoundingClientRect().top;
        var alt = $('.ma-alt-arac').outerHeight(true) || 34;
        el.style.height = Math.max(240, Math.floor(window.innerHeight - ust - alt - 28)) + 'px';
    }
    $(window).on('resize', gridYay);

    // Açılış (eski Page_Load): yıl bu yıl, belge tarihi bugün, ilk fiş türü, kullanıcının entegrasyon muhasebe / birimi. Tablo boş başlar.
    Sgb.al(UC + 'tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            Sgb.yilKutusu('#txtYil', t.yillar, t.yil);
            $('#txtBelgeTarih').val(t.belgeTarih || '');
            fisTurleriYaz();
            if ((t.fisTurler || []).length) $('#ddlFisTur').val(String(t.fisTurler[0].kod));
            if (t.detayBirim) grid.setColumnsVisible(['DETAYBIRIM'], true);
            if (t.muhasebe) {
                muhBirim.ata({ muhasebe: t.muhasebe, birim: t.birim });
                muhBirim.yenile(); // adı okur, degisince ile detay birimler gelir
            }
        })
        .fail(hataGoster)
        .always(gridYay);
    toplamYaz();
    gridYay();
});
