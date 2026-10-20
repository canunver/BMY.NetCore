// Personel Maaş Bilgileri Toplu Düzenleme (InsanKaynaklari/ListePersonelMaasParam; eski ListePersonelMaasParam.aspx).
// Üstte kriterler (muhasebe / birim, bordro türü, arama, unvan, iki alan koşulu); altta personeller ve maaş parametreleri tek
// tabloda. T.C., ad, soyad, unvan, sicil solda sabit ve salt okunur (sarı); değişken kolonları girilir (beyaz).
// Kolonlar listesi hangi değişkenlerin görüneceğini ve Kaydet'te hangilerinin yazılacağını belirler (eski grdDegiskenler).
// Kaydet değişen satırları gönderir; Dosya Yükle'den sonra bütün satırları (eski dirtyRowsOnly: degisenler).
// Uçlar: /api/maasparamtoplu/* (Istemci/InsanKaynaklari/ListePersonelMaasParamUclari.cs).
$(function () {
    'use strict';

    if (!$('#grdListe').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var U = 'api/maasparamtoplu/';
    var sayiBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 6, maximumFractionDigits: 6 }); // eski 0.000,000000/i
    var degiskenler = [];     // [{ degiskenAd, aciklama, para }]
    var hepsiKaydedilsin = false; // Dosya Yükle'den sonra bütün satırlar kaydedilir
    var kirli = false;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }

    // Türkçe (1.234,56) ve noktalı (1234.56) yazımı kabul edilir; boş 0; geçersizse null.
    function sayiOku(deger) {
        if (deger == null) return 0;
        if (typeof deger === 'number') return deger;
        var s = $.trim(String(deger)).replace(/\s/g, '');
        if (s === '') return 0;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = parseFloat(s);
        return isNaN(n) ? null : Math.round(n * 1e6) / 1e6;
    }

    function degiskenBul(ad) {
        for (var i = 0; i < degiskenler.length; i++) if (degiskenler[i].degiskenAd === ad) return degiskenler[i];
        return null;
    }

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- yükseklik: tablo sayfanın kalan yüksekliğini doldurur

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
    function yay() { yuksekligeYay(document.getElementById('grdListe'), 220); }
    $(window).on('resize', yay);

    $('#btnYardim').on('click', function () {
        var $m = $('#mptYardim'), acik = $m.prop('hidden');
        $m.prop('hidden', !acik);
        $(this).attr('aria-expanded', acik ? 'true' : 'false');
        yay();
    });

    // ---------------------------------------------------------------- çoklu seçim (eski MultiCombo / grdDegiskenler onay kutuları)

    function cokluSecim(el, degisince) {
        var $el = $(el).addClass('dropdown');
        var onek = $el.data('onek') || '';
        var $dugme = $('<button type="button" class="btn btn-default btn-sm form-control mpt-coklu-dugme" aria-haspopup="true" aria-expanded="false"></button>').appendTo($el);
        var $metin = $('<span class="mpt-coklu-metin"></span>').appendTo($dugme);
        $('<span class="caret"></span>').appendTo($dugme);
        var $liste = $('<div class="dropdown-menu mpt-coklu-liste"></div>').appendTo($el);
        var ogeler = [];

        function metinYaz() {
            var adlar = $.map(ogeler, function (o) { return o.secili ? o.ad : null; });
            if (onek) $metin.text(onek + ' (' + adlar.length + '/' + ogeler.length + ')');
            else $metin.text(adlar.length === 0 ? ($el.data('bos') || '') : adlar.length > 1 ? '[' + adlar.join(', ') + ']' : adlar[0]);
            $dugme.attr('title', adlar.join(', '));
        }
        function ciz() {
            $liste.empty();
            if (!ogeler.length) { $('<div class="mpt-coklu-bos">Seçenek yok</div>').appendTo($liste); metinYaz(); return; }
            var $hepsi = $('<label class="mpt-coklu-hepsi"><input type="checkbox" /> Tümü</label>').appendTo($liste);
            $hepsi.find('input').prop('checked', $.grep(ogeler, function (o) { return !o.secili; }).length === 0).on('change', function () {
                var c = this.checked;
                $.each(ogeler, function (i, o) { o.secili = c; });
                ciz();
                if (degisince) degisince();
            });
            $.each(ogeler, function (i, o) {
                var $l = $('<label></label>').appendTo($liste);
                $('<input type="checkbox" />').prop('checked', !!o.secili).on('change', function () {
                    o.secili = this.checked;
                    $hepsi.find('input').prop('checked', $.grep(ogeler, function (x) { return !x.secili; }).length === 0);
                    metinYaz();
                    if (degisince) degisince();
                }).appendTo($l);
                $l.append(document.createTextNode(' ' + o.ad));
            });
            metinYaz();
        }
        $liste.on('click', function (e) { e.stopPropagation(); });
        $dugme.on('click', function (e) {
            e.stopPropagation();
            var acik = $el.hasClass('open');
            $('.mpt-coklu.open').removeClass('open').children('.mpt-coklu-dugme').attr('aria-expanded', 'false');
            $el.toggleClass('open', !acik);
            $dugme.attr('aria-expanded', !acik ? 'true' : 'false');
        });
        $(document).on('click', function () { $el.removeClass('open'); $dugme.attr('aria-expanded', 'false'); });
        ciz();
        return {
            doldur: function (liste, hepsiSecili) {
                ogeler = $.map(liste || [], function (o) { return { kod: o.kod, ad: o.ad, secili: !!hepsiSecili }; });
                ciz();
            },
            sec: function (kod, secili) {
                $.each(ogeler, function (i, o) { if (o.kod === kod) o.secili = secili; });
                ciz();
            },
            secilenler: function () { return $.map(ogeler, function (o) { return o.secili ? o.kod : null; }); }
        };
    }

    var bordroTur = cokluSecim('#cokBordroTur');
    var kolonlar = cokluSecim('#cokKolonlar', kolonlariGoster);

    // ---------------------------------------------------------------- muhasebe / birim (eski ExtMuhSecim + txtBirim)

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', ad: 'lblBirimAd' },
        kurumsalGizle: true
    });

    // ---------------------------------------------------------------- tablo

    var SABIT = [
        { field: 'tcKimlikNo', headerName: 'T.C. Kimlik No', width: 110, pinned: 'left', cellClass: 'mpt-salt' },
        { field: 'ad', headerName: 'Adı', width: 100, pinned: 'left', cellClass: 'mpt-salt', tooltipField: 'ad' },
        { field: 'soyad', headerName: 'Soyadı', width: 100, pinned: 'left', cellClass: 'mpt-salt', tooltipField: 'soyad' },
        { field: 'unvanAd', headerName: 'Unvanı', width: 110, pinned: 'left', cellClass: 'mpt-salt', tooltipField: 'unvanAd' },
        { field: 'sicilNo', headerName: 'Sicil', width: 70, pinned: 'left', cellClass: 'mpt-salt' }
    ];

    function degiskenSutunu(dg) {
        var ad = dg.degiskenAd;
        var s = {
            colId: ad, headerName: dg.aciklama || ad, headerTooltip: (dg.aciklama || ad) + ' (' + ad + ')',
            editable: true, sortable: true, wrapHeaderText: true, autoHeaderHeight: true,
            valueGetter: function (p) { return p.data && p.data.d ? p.data.d[ad] : null; },
            valueSetter: function (p) {
                if (!p.data.d) p.data.d = {};
                if (p.data.d[ad] === p.newValue) return false;
                p.data.d[ad] = p.newValue;
                return true;
            }
        };
        if (dg.para) {
            $.extend(s, {
                width: 100, cellClass: 'mpt-girilir mpt-sag', headerClass: 'mpt-sag-baslik',
                valueFormatter: function (p) { return p.value == null || p.value === '' ? '' : sayiBicim.format(p.value); },
                valueParser: function (p) {
                    var n = sayiOku(p.newValue);
                    if (n === null) { mesaj('Geçerli bir sayı giriniz.', 'warning'); return p.oldValue; }
                    return n;
                },
                comparator: function (a, b) { return (Number(a) || 0) - (Number(b) || 0); }
            });
        } else {
            $.extend(s, { width: 120, cellClass: 'mpt-girilir', tooltipValueGetter: function (p) { return p.value; } });
        }
        return s;
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: SABIT,
        rowHeight: 24,
        headerHeight: 26,
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVerticallyAfterEdit: true,
        enterNavigatesVertically: true,
        onCellValueChanged: function (p) {
            if (p.oldValue === p.newValue) return;
            p.data._degisti = true;
            kirli = true;
        },
        overlayNoRowsTemplate: '<span class="text-muted">Bordro türü seçip Listele\'ye basınız.</span>'
    });

    // Kolonlar listesinde seçili değişkenler görünür (eski DegiskenSecildi / GosterGizle).
    function kolonlariGoster() {
        var secili = kolonlar.secilenler();
        var gorunur = [], gizli = [];
        $.each(degiskenler, function (i, dg) { (secili.indexOf(dg.degiskenAd) >= 0 ? gorunur : gizli).push(dg.degiskenAd); });
        if (gorunur.length) grid.setColumnsVisible(gorunur, true);
        if (gizli.length) grid.setColumnsVisible(gizli, false);
    }

    function sayiYaz() {
        var n = grid.getDisplayedRowCount();
        $('#lblSayi').text(n ? n + ' kişi' : '');
    }

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    function secenekDoldur($s, liste, metin, deger) {
        $s.empty();
        $.each(liste, function (i, o) { $('<option></option>').val(deger(o)).text(metin(o)).appendTo($s); });
    }

    function tanimOku() {
        return Sgb.al(U + 'tanim').done(function (v) {
            if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
            if (v.hata) mesaj(v.hata, 'warning');
            degiskenler = v.degiskenler || [];

            muhBirim.ata({ muhasebe: v.muhasebe, birim: v.birim, birimAd: v.birimAd });
            secenekDoldur($('#ddlUnvan'), v.unvanlar || [], function (o) { return o.ad; }, function (o) { return o.kod; });
            bordroTur.doldur(v.bordroTurler);

            // Alan koşulları ve Kişilere Ata: değişkenler, sonda "-" (eski stoDegiskenler'in son satırı).
            var alanlar = [{ degiskenAd: '', aciklama: '-' }].concat(degiskenler);
            $('#cmbAlan1, #cmbAlan2, #sbDegisken').each(function () {
                secenekDoldur($(this), alanlar, function (o) { return o.aciklama || o.degiskenAd; }, function (o) { return o.degiskenAd; });
            });
            secenekDoldur($('#cmbVeri'), [{ degiskenAd: '', aciklama: 'Seçiniz' }].concat(degiskenler),
                function (o) { return o.aciklama || o.degiskenAd; }, function (o) { return o.degiskenAd; });
            $('#cmbIslec1, #cmbIslec2').each(function () {
                secenekDoldur($(this), v.islecler || [], function (o) { return o.ad; }, function (o) { return o.kod; });
            });

            grid.setGridOption('columnDefs', SABIT.concat($.map(degiskenler, degiskenSutunu)));
            kolonlar.doldur($.map(degiskenler, function (dg) { return { kod: dg.degiskenAd, ad: dg.aciklama || dg.degiskenAd }; }), true);
            kolonlariGoster();
        }).fail(hataGoster);
    }

    // ---------------------------------------------------------------- listele (eski PersonelListele)

    function listele() {
        grid.stopEditing();
        var turler = bordroTur.secilenler();
        if (!turler.length) { mesaj('Listelemeden önce lütfen personel tipini seçiniz', 'warning'); return; }
        if (kirli && !window.confirm('Kaydedilmemiş değişiklikler kaybolacak. Yeniden listelensin mi?')) return;
        var mb = muhBirim.deger();
        var p = {
            muhasebe: mb.muhasebe || 0,
            birim: mb.birim || '',
            ara: $.trim($('#txtSorgu').val()),
            unvan: parseInt($('#ddlUnvan').val(), 10) || 0,
            turler: turler.join(', '),
            alan1: $('#cmbAlan1').val() || '', islec1: parseInt($('#cmbIslec1').val(), 10) || 0, deger1: sayiOku($('#txtDeger1').val()) || 0,
            alan2: $('#cmbAlan2').val() || '', islec2: parseInt($('#cmbIslec2').val(), 10) || 0, deger2: sayiOku($('#txtDeger2').val()) || 0
        };
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(U + 'liste', p).done(function (v) {
            if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
            grid.setGridOption('rowData', v.satirlar || []);
            hepsiKaydedilsin = false;
            kirli = false;
            sayiYaz();
            if (!(v.satirlar || []).length) mesaj('Kriterlere uyan personel bulunamadı.', 'info');
        }).fail(hataGoster).always(function () {
            $d.prop('disabled', false);
            grid.setGridOption('loading', false);
        });
    }

    $('#btnListele').on('click', listele);
    $('#txtSorgu').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); listele(); } });

    // ---------------------------------------------------------------- Kişilere Ata (eski KisilereAta)

    $('#btnKisilereAta').on('click', function () {
        grid.stopEditing();
        var ad = $('#sbDegisken').val();
        if (!ad) { mesaj('Atanacak değişkeni seçiniz.', 'warning'); return; }
        var dg = degiskenBul(ad);
        var deger = $('#nfGirilen').val();
        if (dg && dg.para) {
            deger = sayiOku(deger);
            if (deger === null) { mesaj('Geçerli bir sayı giriniz.', 'warning'); return; }
        }
        var n = 0;
        grid.forEachNode(function (node) {
            if (!node.data.d) node.data.d = {};
            if (node.data.d[ad] === deger) return;
            node.data.d[ad] = deger;
            node.data._degisti = true;
            n++;
        });
        if (!n) { mesaj('Değişen satır olmadı.', 'info'); return; }
        kirli = true;
        if (kolonlar.secilenler().indexOf(ad) < 0) { kolonlar.sec(ad, true); kolonlariGoster(); }
        grid.refreshCells({ columns: [ad], force: true });
        mesaj(n + ' kişiye atandı. Kaydetmek için Kaydet\'e basınız.', 'success');
    });

    // ---------------------------------------------------------------- Kaydet (eski btnKaydet_Click)

    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        var secili = kolonlar.secilenler();
        if (!secili.length) { mesaj('Kaydedilecek kolon seçilmedi.', 'warning'); return; }
        var satirlar = [];
        grid.forEachNode(function (node) {
            var d = node.data;
            if (!hepsiKaydedilsin && !d._degisti) return;
            satirlar.push({
                tcKimlikNo: d.tcKimlikNo, sicilNo: d.sicilNo, ad: d.ad, soyad: d.soyad,
                unvan: d.unvan || 0, unvanAd: d.unvanAd, kadroDerece: d.kadroDerece || 0, d: d.d || {}
            });
        });
        if (!satirlar.length) { mesaj('Değişen satır yok.', 'warning'); return; }
        if (!window.confirm(satirlar.length + ' kişinin seçili ' + secili.length + ' kolonu kaydedilsin mi?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(U + 'kaydet', { degiskenler: secili, satirlar: satirlar }).done(function (v) {
            if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
            mesaj(v.bilgi || 'Kayıt başarılı', 'success');
            grid.forEachNode(function (node) { delete node.data._degisti; });   // eski stoListe.CommitChanges()
            hepsiKaydedilsin = false;
            kirli = false;
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Dosya Yükle (eski wndDosyaYukle / fuDosya_Selected)

    $('#btnDosyaYukle').on('click', function () {
        grid.stopEditing();
        if (!grid.getDisplayedRowCount()) { mesaj('Önce personelleri listeleyiniz.', 'warning'); return; }
        $('#dlgDosya')[0].showModal();
    });

    $('#btnDosyaSec').on('click', function () {
        if (!$('#cmbVeri').val()) { mesaj('Lütfen Veri Alanını Seçiniz', 'warning'); return; }
        if (!$('#cmbAnahtar').val()) { mesaj('Lütfen Anahtar Alanı Seçiniz', 'warning'); return; }
        $('#fuDosya').val('').trigger('click');
    });

    $('#fuDosya').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('anahtar', $('#cmbAnahtar').val());
        veri.append('veriAlan', $('#cmbVeri').val());
        var $d = $('#btnDosyaSec').prop('disabled', true);
        mesaj('Dosya yükleniyor...', 'info');
        $.ajax({
            url: Sgb.KOK + U + 'dosya', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
            dosyaUygula(v);
            $('#dlgDosya')[0].close();
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else hataGoster(xhr);
        }).always(function () { $d.prop('disabled', false); $('#fuDosya').val(''); });
    });

    // Eski SatirBul: anahtar 1 ise T.C. kimlik no, 2 ise sicil no ile ilk eşleşen kişiye yazılır.
    function dosyaUygula(v) {
        var alan = v.anahtar === 2 ? 'sicilNo' : 'tcKimlikNo';
        var dizin = {};
        grid.forEachNode(function (node) {
            var k = $.trim(String(node.data[alan] || ''));
            if (k && !dizin[k]) dizin[k] = node;
        });
        var bulunan = 0, bulunmayan = 0;
        $.each(v.satirlar || [], function (i, s) {
            var node = dizin[$.trim(String(s.anahtar))];
            if (!node) { bulunmayan++; return; }
            if (!node.data.d) node.data.d = {};
            node.data.d[v.veriAlan] = s.deger;
            bulunan++;
        });
        hepsiKaydedilsin = true;   // eski DegistiIsaretiKoy(): Kaydet bütün satırları gönderir
        kirli = true;
        if (kolonlar.secilenler().indexOf(v.veriAlan) < 0) { kolonlar.sec(v.veriAlan, true); kolonlariGoster(); }
        grid.refreshCells({ columns: [v.veriAlan], force: true });
        mesaj((v.satirlar || []).length + ' satır okundu, ' + bulunan + ' kişiye yazıldı'
            + (bulunmayan ? ', ' + bulunmayan + ' satır listede bulunamadı' : '') + '. Kaydetmek için Kaydet\'e basınız.',
            bulunmayan ? 'warning' : 'success');
    }

    $(window).on('beforeunload', function () { if (kirli) return 'Kaydedilmemiş değişiklikler var.'; });

    yay();
    tanimOku().always(yay);
});
