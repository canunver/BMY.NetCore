// Makbuz formu (ButceMuhasebe/MakbuzGiris; eski aynı adlı Ext.NET sayfası). Makbuz Bilgileri listesinden pencerede açılır:
// MakbuzGiris?yil=&muhasebe=&birim=&makbuzNo= (makbuzNo boşsa yeni makbuz). Hesap satırlarında hesap planı, kişi / firma
// (alt hesap) ve tertip seçimi; açıklama tanımları; Kaydet, Yazdır (PDF), Ödendi. Uçlar: /api/makbuz/* (Muhasebe/MakbuzUclari.cs).
$(function () {
    'use strict';

    if (!$('#grdHesaplar').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/makbuz/';
    var EN_AZ_SATIR = 7;
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    var adres = new URLSearchParams(window.location.search);
    var sayfa = {
        yil: parseInt(adres.get('yil'), 10) || 0,
        muhasebe: parseInt(adres.get('muhasebe'), 10) || 0,
        birim: $.trim(adres.get('birim') || ''),
        makbuzNo: $.trim(adres.get('makbuzNo') || '')
    };
    var tanim = { kurumsal: '', makbuzTipleri: [], yeniTuslar: null };
    var makbuz = { yil: 0, yevmiyeNo: '', durum: 0 };   // ekrandaki makbuzun anahtarı ve durumu
    var satirNo = 0;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function paraYaz(v) { return v === null || v === undefined || v === '' ? '' : para.format(Number(v) || 0); }

    // "1.234,56" ya da "1234.56" -> sayı; boş -> null
    function sayiCoz(s) {
        if (typeof s === 'number') return s;
        s = $.trim(String(s == null ? '' : s)).replace(/\s/g, '');
        if (s === '') return null;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = parseFloat(s);
        return isNaN(n) ? null : n;
    }

    function yil() {
        var t = $('#txtMakbuzTarih').val();
        return parseInt((t || '').substring(0, 4), 10) || makbuz.yil || new Date().getFullYear();
    }

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;
    function sor(metin, is, evet) {
        $('#soruMetin').text(metin);
        $('#btnSoruEvet').text(evet || 'Evet');
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

    // ---------------------------------------------------------------- hesap satırları

    function bosSatir() {
        return { _id: ++satirNo, hesapKod: '', hesapAd: '', altHesap: '', altHesapAd: '', tertip: '', tutar: 0 };
    }

    // Hücre metni + mercek (eski ImageCommandColumn: Ac, FirmaBilgisiAc, TertipBilgisiAc)
    function mercekHucre(ne, baslik) {
        return function (p) {
            var $h = $('<span class="mk-hucre-sec"></span>');
            $('<span class="mk-hucre-metin"></span>').text(p.valueFormatted != null ? p.valueFormatted : (p.value || '')).appendTo($h);
            $('<button type="button" class="btn btn-link btn-xs mk-mercek"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></button>')
                .attr('title', baslik).append($('<span class="sr-only"></span>').text(baslik))
                .on('click', function (e) { e.stopPropagation(); grid.stopEditing(); ne(p.node); })
                .appendTo($h);
            return $h[0];
        };
    }

    function kodAd(kod, ad) { return kod ? kod + (ad ? ' - ' + ad : '') : ''; }

    var grid = Sgb.grid('#grdHesaplar', {
        columnDefs: [
            { headerName: '#', valueGetter: 'node.rowIndex + 1', width: 56, sortable: false },
            {
                field: 'hesapKod', headerName: 'Hesap Kodu', width: 300, editable: true, sortable: false, cellClass: 'mk-girilir',
                valueFormatter: function (p) { return p.data ? kodAd(p.data.hesapKod, p.data.hesapAd) : ''; },
                cellRenderer: mercekHucre(hesapPlaniAc, 'Hesap planından seç'), cellEditor: 'agTextCellEditor'
            },
            {
                field: 'altHesap', headerName: 'Alt Hesap Kodu', width: 300, editable: true, sortable: false, cellClass: 'mk-girilir',
                valueFormatter: function (p) { return p.data ? kodAd(p.data.altHesap, p.data.altHesapAd) : ''; },
                cellRenderer: mercekHucre(firmaAc, 'Kişi / firma seç'), cellEditor: 'agTextCellEditor'
            },
            {
                field: 'tertip', headerName: 'Tertip', width: 260, editable: true, sortable: false, cellClass: 'mk-girilir',
                cellRenderer: mercekHucre(tertipAc, 'Tertip seç'), cellEditor: 'agTextCellEditor'
            },
            {
                field: 'tutar', headerName: 'Tutar', width: 140, type: 'rightAligned', editable: true, sortable: false,
                cellClass: ['mk-girilir', 'ag-right-aligned-cell'],
                valueFormatter: function (p) { return p.value ? paraYaz(p.value) : ''; },
                valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? 0 : Math.round(n * 100) / 100; }
            }
        ],
        rowData: [],
        rowHeight: 28,
        headerHeight: 32,
        getRowId: function (p) { return String(p.data._id); },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        onCellValueChanged: function (e) {
            if (e.newValue === e.oldValue) return;
            var f = e.colDef.field;
            if (f === 'tutar') toplamYaz();
            else if (f === 'hesapKod') hesapAdOku(e.node);
            else if (f === 'altHesap') altHesapAdOku(e.node);
        }
    });

    function satirlar() {
        var r = [];
        grid.forEachNode(function (n) { r.push(n.data); });
        return r;
    }

    function satirlariYaz(hesaplar) {
        var r = $.map(hesaplar || [], function (h) { return $.extend(bosSatir(), h); });
        while (r.length < EN_AZ_SATIR) r.push(bosSatir());
        grid.setGridOption('rowData', r);
        toplamYaz();
    }

    $('#btnSatirEkle').on('click', function () {
        grid.applyTransaction({ add: [bosSatir()] });
    });

    // Elle yazılan hesap kodunun adı (eski hesap kombosu "kod - ad" yazıyordu).
    function hesapAdOku(node) {
        var kod = $.trim(node.data.hesapKod || '');
        // "kod - ad" yapıştırılmışsa kod ayrılır
        var i = kod.indexOf(' ');
        if (i > 0) kod = kod.substring(0, i);
        node.data.hesapKod = kod;
        node.data.hesapAd = '';
        grid.refreshCells({ rowNodes: [node], force: true });
        if (!kod) return;
        Sgb.al(UC + 'hesap/ad', { yil: yil(), kod: kod }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            if (!v.ad) { mesaj(kod + ' kodlu hesap bulunamadı.', 'warning'); return; }
            node.data.hesapKod = v.kod || kod;
            node.data.hesapAd = v.ad;
            grid.refreshCells({ rowNodes: [node], force: true });
        }).fail(hataGoster);
    }

    // Elle yazılan alt hesabın (kişi / firma kimlik no) adı (eski alt hesap kombosu "vergiNo - ad").
    function altHesapAdOku(node) {
        var deger = $.trim(node.data.altHesap || '');
        var p = deger.split('-');
        var no = $.trim(p[0]);
        node.data.altHesap = no;
        node.data.altHesapAd = p.length > 1 ? $.trim(p.slice(1).join('-')) : '';
        grid.refreshCells({ rowNodes: [node], force: true });
        if (!no || node.data.altHesapAd) return;
        Sgb.al(UC + 'firma', { muhasebe: sayfa.muhasebe, vergiNo: no }).done(function (v) {
            if (v.hata || !v.ad) return;
            node.data.altHesapAd = v.ad;
            grid.refreshCells({ rowNodes: [node], force: true });
        });
    }

    // Eski HesapPlaniAc / NodeSecildiYerel
    function hesapPlaniAc(node) {
        Sgb.hesapPlaniSec({
            yil: yil(), sonKirilim: true,
            secilince: function (h) {
                node.data.hesapKod = h.kod;
                node.data.hesapAd = h.hesapAd || '';
                grid.refreshCells({ rowNodes: [node], force: true });
            }
        });
    }

    // Eski FirmaBilgisiAc('grd') / FirmaBilgisiGoster
    function firmaAc(node) {
        Sgb.firmaSec({
            muhasebe: sayfa.muhasebe,
            secilince: function (f) {
                node.data.altHesap = f.vergiNo;
                node.data.altHesapAd = f.ad;
                grid.refreshCells({ rowNodes: [node], force: true });
            }
        });
    }

    // Eski BorcAlacakToplaYaz: satır tutarları + makbuz tipi tutarı.
    function toplamYaz() {
        var toplam = 0;
        $.each(satirlar(), function (i, r) { toplam += Number(r.tutar) || 0; });
        toplam += sayiCoz($('#txtMTTutar').val()) || 0;
        var pb = $('#ddlParaBirimi').prop('hidden') ? 'TRY' : ($('#ddlParaBirimi').val() || 'TRY');
        $('#lblTutar').text('Toplam Tutar: ' + para.format(toplam) + ' ' + pb);
    }

    $('#txtMTTutar').on('input', toplamYaz).on('blur', function () {
        var n = sayiCoz($(this).val());
        $(this).val(n === null ? '' : para.format(n));
        toplamYaz();
    });
    $('#txtKrediKarti').on('blur', function () {
        var n = sayiCoz($(this).val());
        $(this).val(n === null ? '' : para.format(n));
    });
    $('#ddlParaBirimi').on('change', toplamYaz);

    // ---------------------------------------------------------------- tertip seçimi (eski ListeTertip, seç ve kapat; ortak Sgb.tertipSec)

    // Seçilen tertipler hedef satırdan başlayarak alt alta yazılır (eski ListeTertipAktar).
    function tertipAktar(tertipNode, liste) {
        var nodes = [];
        grid.forEachNode(function (n) { nodes.push(n); });
        var i = tertipNode.rowIndex;
        $.each(liste, function (k, t) {
            if (i >= nodes.length) { grid.applyTransaction({ add: [bosSatir()] }); nodes = []; grid.forEachNode(function (n) { nodes.push(n); }); }
            nodes[i].data.tertip = t.TERTIP;
            grid.refreshCells({ rowNodes: [nodes[i]], force: true });
            i++;
        });
    }

    // Açılış: kurumsal kod muhasebe biriminden, ekonomik kod hesap kodundan bulunur ve listelenir.
    function tertipAc(node) {
        if (!$.trim(node.data.hesapKod || '')) { mesaj('Lütfen ilk önce hesap kodunu seçin.', 'warning'); return; }
        Sgb.tertipSec({
            tur: 'muh', belgeTur: 1, secKapat: true, yil: yil(), muhasebe: sayfa.muhasebe, birim: birimKod(), hesapKod: node.data.hesapKod,
            secilince: function (secili) { tertipAktar(node, secili); }
        });
    }

    // ---------------------------------------------------------------- kişi / firma

    function kisiYaz(f) {
        $('#txtTcKimlikVergiNo').val(f.vergiNo || '');
        $('#txtKisiAd').val(f.ad || '');
    }

    $('#btnKisiSec').on('click', function () {
        Sgb.firmaSec({ muhasebe: sayfa.muhasebe, secilince: kisiYaz });
    });

    // Eski FirmaBilgisiGetir (kimlik no alanından çıkınca)
    $('#txtTcKimlikVergiNo').on('change', function () {
        var no = $.trim($(this).val());
        if (!no) return;
        Sgb.al(UC + 'firma', { muhasebe: sayfa.muhasebe, vergiNo: no }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            if (v.ad) { $('#txtKisiAd').val(v.ad); return; }
            sor(no + ' kimlik numaralı kişi/firma bulunamadı. Yeni kayıt yapmanız gerekli.', function () {
                Sgb.firmaSec({ muhasebe: sayfa.muhasebe, ara: no, secilince: kisiYaz });
            }, 'Tamam');
        }).fail(hataGoster);
    });

    // Plaka (yalnızca 46.06.04 kurumunda; eski PlakaGiris.aspx henüz taşınmadı, pencerede açılır)
    $('#btnPlaka').on('click', function () {
        Sgb.pencere('PlakaGiris.aspx?' + $.param({ vergiNo: $.trim($('#txtTcKimlikVergiNo').val()) }), 'Plaka');
    });

    // ---------------------------------------------------------------- makbuz tipi

    // Eski cmbMakbuzTipler_SelectedIndexChanged: tipe bağlı açıklamalar açıklamanın başına eklenir.
    function tipAciklamaEkle() {
        var tip = parseInt($('#ddlMakbuzTipi').val(), 10) || 0;
        if (!tip) return;
        Sgb.al(UC + 'tip-aciklama', { tip: tip }).done(function (v) {
            if (v.hata) return;
            $.each(v.aciklamalar || [], function (i, a) {
                $('#txtAciklama').val(a + '\n' + $('#txtAciklama').val());
            });
        });
    }

    $('#ddlMakbuzTipi').on('change', tipAciklamaEkle);

    // ---------------------------------------------------------------- ekrana yazma

    function birimKod() {
        var mb = $.trim($('#txtMakbuzBirim').val());
        return !$('#grpMakbuzBirim').prop('hidden') && mb ? mb : sayfa.birim;
    }

    function tuslariYaz(t) {
        t = t || {};
        $('#btnKaydet').prop('hidden', !t.kaydet);
        $('#btnYazdir').prop('hidden', !t.yazdir);
        $('#btnOdendi').prop('hidden', !t.odendi);
    }

    function durumYaz(durum, durumAd) {
        makbuz.durum = durum || 0;
        $('#lblDurum').text(durumAd || '').prop('hidden', !durumAd)
            .toggleClass('label-default', durum !== 9 && durum !== 3 && durum !== 5)
            .toggleClass('label-danger', durum === 9).toggleClass('label-success', durum === 3 || durum === 5);
    }

    function paraBirimiSec(kod) {
        var $p = $('#ddlParaBirimi');
        if ($p.prop('hidden')) return;
        if (!$p.find('option').filter(function () { return this.value === kod; }).length)
            $('<option></option>').val(kod).text(kod).appendTo($p);
        $p.val(kod);
    }

    // Eski ListedenEkrana
    function ekranaYaz(m) {
        makbuz.yil = m.yil;
        makbuz.yevmiyeNo = m.yevmiyeNo || '';
        $('#txtMakbuzTarih').val(m.islemTarih || Sgb.bugun());
        $('#txtTcKimlikVergiNo').val(m.tcKimlikNoVergiNo);
        $('#txtKisiAd').val(m.kisiAd);
        $('#lblYevmiyeNo').text('Yevmiye No:' + (makbuz.yevmiyeNo ? ' ' + makbuz.yevmiyeNo : ''));
        if (makbuz.yevmiyeNo) $('#grpMakbuzBirim').prop('hidden', false);
        $('#txtMakbuzBirim').val(m.birim || '');
        satirlariYaz(m.hesaplar);
        paraBirimiSec(m.paraBirimiKod || 'TRY');
        $('#ddlMakbuzTipi').val(String(m.makbuzTipi || ''));
        if ($('#ddlMakbuzTipi').val() === null) $('#ddlMakbuzTipi').prop('selectedIndex', 0);
        $('#txtMTTutar').val(m.makbuzTipiTutar ? para.format(m.makbuzTipiTutar) : '');
        $('#txtKrediKarti').val(m.krediKartiIleOdenen >= 0 ? para.format(m.krediKartiIleOdenen) : '');
        $('#txtAciklama').val(m.aciklama);
        $('#txtDuzenleyen').val(m.duzenleyen);
        $('#txtUnvan').val(m.unvan);
        durumYaz(m.durum, m.durumAd);
        toplamYaz();
        tuslariYaz(m.tuslar);
    }

    // ---------------------------------------------------------------- kaydet / yeni / yazdır / ödendi

    function krediKarti() { return sayiCoz($('#txtKrediKarti').val()); }

    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        if (!$('#txtMakbuzTarih').val()) { mesaj('Makbuz tarihini giriniz!', 'warning'); return; }
        sor('Kaydetmek istiyor musunuz?', function () {
            var hesaplar = $.map(satirlar(), function (r) {
                if (!$.trim(r.hesapKod) && !$.trim(r.altHesap) && !$.trim(r.tertip) && !Number(r.tutar)) return null;
                return { hesapKod: r.hesapKod, hesapAd: r.hesapAd, altHesap: r.altHesap, altHesapAd: r.altHesapAd, tertip: r.tertip, tutar: Number(r.tutar) || 0 };
            });
            var istek = {
                YevmiyeNo: makbuz.yevmiyeNo,
                Muhasebe: sayfa.muhasebe,
                Birim: sayfa.birim,
                MakbuzBirim: $('#grpMakbuzBirim').prop('hidden') ? '' : $.trim($('#txtMakbuzBirim').val()),
                Tarih: $('#txtMakbuzTarih').val(),
                MakbuzTipi: parseInt($('#ddlMakbuzTipi').val(), 10) || 0,
                TcKimlikNoVergiNo: $.trim($('#txtTcKimlikVergiNo').val()),
                KisiAd: $.trim($('#txtKisiAd').val()),
                Tutar: sayiCoz($('#txtMTTutar').val()) || 0,
                ParaBirimi: $('#ddlParaBirimi').prop('hidden') ? '' : ($('#ddlParaBirimi').val() || ''),
                Aciklama: $('#txtAciklama').val(),
                Duzenleyen: $.trim($('#txtDuzenleyen').val()),
                Unvan: $.trim($('#txtUnvan').val()),
                KrediKarti: krediKarti(),
                Hesaplar: hesaplar
            };
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder(UC + 'form/kaydet', istek)
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata || 'Kayıt işlemi yapılamadı.', 'danger'); return; }
                    mesaj(s.bilgi, 'success');
                    if (s.makbuz) {
                        sayfa.birim = s.makbuz.birim || sayfa.birim;
                        ekranaYaz(s.makbuz);
                    } else {
                        makbuz.yevmiyeNo = s.yevmiyeNo;
                        $('#lblYevmiyeNo').text('Yevmiye No: ' + s.yevmiyeNo);
                    }
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // Yeni Kayıt (eski btnTemizle_Click): kişi, tutar, açıklama, kart ve yevmiye no temizlenir; hesap satırları ve makbuz tipi kalır.
    $('#btnYeni').on('click', function () {
        grid.stopEditing();
        makbuz.yevmiyeNo = '';
        $('#txtTcKimlikVergiNo, #txtKisiAd, #txtAciklama, #txtKrediKarti').val('');
        $('#txtMTTutar').val('0,00');
        $('#lblYevmiyeNo').text('Yevmiye No:');
        $('#grpMakbuzBirim').prop('hidden', true);
        $('#txtMakbuzBirim').val('');
        paraBirimiSec('TRY');
        durumYaz(0, '');
        tuslariYaz(tanim.yeniTuslar);
        toplamYaz();
        tipAciklamaEkle();
    });

    // Yazdır (eski MakbuzYazdir.aspx): PDF
    $('#btnYazdir').on('click', function () {
        if (!makbuz.yevmiyeNo) { mesaj('Önce makbuzu kaydediniz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir(UC + 'form/yazdir', { yil: makbuz.yil, muhasebe: sayfa.muhasebe, makbuzNo: makbuz.yevmiyeNo })
            .then(function () { $d.prop('disabled', false); });
    });

    // Ödendi (eski btnOdendi_Click)
    $('#btnOdendi').on('click', function () {
        if (!makbuz.yevmiyeNo) { mesaj('Önce makbuzu kaydediniz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'form/odendi', { Yil: makbuz.yil, Muhasebe: sayfa.muhasebe, YevmiyeNo: makbuz.yevmiyeNo, KrediKarti: krediKarti() })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata || 'İşlem yapılamadı.', 'danger'); return; }
                mesaj(s.bilgi, 'success');
                durumYaz(s.durum, s.durumAd);
                tuslariYaz(s.tuslar);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açıklama tanımları (eski wndMakbuzAciklama)

    var dlgAciklama = document.getElementById('dlgAciklama');
    var aciklamaGrid = null;
    var aciklamaKod = 0;

    function aciklamaTemizle() {
        aciklamaKod = 0;
        $('#txtEditor').val('');
        $('#ddlAciklamaTip').val('');
        $('#btnAciklamaSil').prop('disabled', true);
        if (aciklamaGrid) aciklamaGrid.deselectAll();
    }

    function aciklamaListele() {
        aciklamaGrid.setGridOption('loading', true);
        Sgb.al(UC + 'aciklama/liste', { birim: sayfa.birim })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                aciklamaGrid.setGridOption('overlayNoRowsTemplate', '<span>Tanımlı açıklama yok.</span>');
                aciklamaGrid.setGridOption('rowData', v.satirlar || []);
                aciklamaTemizle();
            })
            .fail(hataGoster)
            .always(function () { aciklamaGrid.setGridOption('loading', false); });
    }

    function aktarHucre(p) {
        if (!p.data) return '';
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-link btn-xs';
        b.title = 'Makbuza aktar';
        b.innerHTML = '<span class="glyphicon glyphicon-plus" aria-hidden="true"></span><span class="sr-only">Aktar</span>';
        b.addEventListener('click', function (e) {
            e.stopPropagation();
            $('#txtAciklama').val(p.data.ACIKLAMA);
            dlgAciklama.close();
        });
        return b;
    }

    $('#btnAciklamaListesi').on('click', function () {
        dlgAciklama.showModal();
        if (aciklamaGrid) return;
        aciklamaGrid = Sgb.grid('#grdAciklama', {
            columnDefs: [
                { headerName: '#', valueGetter: 'node.rowIndex + 1', width: 56, sortable: false },
                { headerName: 'Aktar', width: 70, sortable: false, cellRenderer: aktarHucre },
                { field: 'ACIKLAMA', headerName: 'Açıklama', flex: 1, minWidth: 200 }
            ],
            rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
            // Eski grdAciklamaTanim_RowClick: seçilen açıklama düzenlenmek üzere okunur.
            onRowClicked: function (e) {
                Sgb.al(UC + 'aciklama/oku', { kod: e.data.KOD }).done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    aciklamaKod = v.KOD;
                    $('#txtEditor').val(v.ACIKLAMA);
                    $('#ddlAciklamaTip').val(v.MAKBUZTIPI ? String(v.MAKBUZTIPI) : '');
                    $('#btnAciklamaSil').prop('disabled', false);
                }).fail(hataGoster);
            }
        });
        aciklamaListele();
    });

    $('#btnAciklamaListele').on('click', aciklamaListele);
    $('#btnAciklamaYeni').on('click', aciklamaTemizle);

    $('#btnAciklamaKaydet').on('click', function () {
        sor('Kayıt edilecek. Onaylıyor musunuz?', function () {
            Sgb.gonder(UC + 'aciklama/kaydet', {
                Kod: aciklamaKod, Aciklama: $('#txtEditor').val(), Birim: sayfa.birim,
                MakbuzTipi: parseInt($('#ddlAciklamaTip').val(), 10) || 0
            }).done(function (s) {
                if (!s.basarili) { mesaj(s.hata || 'Kayıt işlemi yapılamadı.', 'danger'); return; }
                mesaj(s.bilgi, 'success');
                aciklamaListele();
            }).fail(hataGoster);
        });
    });

    $('#btnAciklamaSil').on('click', function () {
        if (!aciklamaKod) return;
        sor('Silinecek. Onaylıyor musunuz?', function () {
            Sgb.gonder(UC + 'aciklama/sil', { Kod: aciklamaKod }).done(function (s) {
                if (!s.basarili) { mesaj(s.hata || 'Silme işlemi yapılamadı.', 'danger'); return; }
                mesaj(s.bilgi, 'success');
                aciklamaListele();
            }).fail(hataGoster);
        });
    });

    // ---------------------------------------------------------------- açılış

    function seceneklerYaz($s, liste, bosEkle) {
        $s.empty();
        if (bosEkle) $('<option value=""></option>').appendTo($s);
        $.each(liste || [], function (i, t) { $('<option></option>').val(t.kod).text(t.aciklama).appendTo($s); });
    }

    if (!sayfa.muhasebe || !sayfa.birim) {
        mesaj('Lütfen muhasebe ve birim seçiniz', 'warning');
        satirlariYaz([]);
        return;
    }

    Sgb.al(UC + 'form/tanim', sayfa)
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); satirlariYaz([]); return; }
            tanim = v;
            $('#lblBirim').text(v.muhasebe + '-' + v.birim + (v.birimAd ? '  ' + v.birimAd : ''));
            seceneklerYaz($('#ddlMakbuzTipi'), v.makbuzTipleri, false);
            seceneklerYaz($('#ddlAciklamaTip'), v.makbuzTipleri, true);
            $('#txtKisiAd').prop('readonly', !v.kisiElle);
            $('#btnPlaka').prop('hidden', !v.plaka);
            if (v.doviz) {
                var $p = $('#ddlParaBirimi').prop('hidden', false).empty();
                $.each(v.paraBirimleri || [], function (i, k) { $('<option></option>').val(k).text(k).appendTo($p); });
            }
            ekranaYaz(v.makbuz);
        })
        .fail(function (xhr) { hataGoster(xhr); satirlariYaz([]); });
});
