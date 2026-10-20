// Muavin Defteri (ButceMuhasebe/MuavinDefteri; eski aynı adlı Ext.NET sayfası, Ödeme Emri / MİF çerçevesinin "Muavin Defteri" sekmesi).
// Sorgu alanlarıyla ödeme emri / MİF satırları hesap koduna göre gruplu listelenir: her hesap kodunun başında (belge tarihi
// başlangıcı verilmişse) DEVİR satırı, satırlarda yürüyen bakiye, grup sonunda borç / alacak toplamları (eski GroupingSummary).
// Yazdır / Yazdır2 Excel raporları. Belge numarasına tıklanınca belge çerçevenin "Belge Düzenleme" sekmesinde açılır
// (window.parent.OemBelgeAc); çerçeve yoksa Ödeme Emri / MİF çerçevesi pencerede açılır. Uçlar: /api/muavindefteri/*
// (Muhasebe/MuavinDefteriUclari.cs). Ortak bileşenler: Sgb.hesapKodu, Sgb.muhasebeBirim, Sgb.agac (kod ağaçları).
$(function () {
    'use strict';

    var $kok = $('.muavin-defteri');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/muavindefteri/';
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var kur4 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 0, maximumFractionDigits: 6 });
    var tanim = {};
    var SAKLA = 'muavindefteri.';

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function para(v) { return v === null || v === undefined || v === '' ? '' : tutar2.format(Number(v) || 0); }
    function sakliOku(ad) { try { return localStorage.getItem(SAKLA + ad); } catch (e) { return null; } }
    function sakliYaz(ad, deger) { try { localStorage.setItem(SAKLA + ad, deger); } catch (e) { /* özel pencere */ } }

    // ---------------------------------------------------------------- açılır menü (Sütunlar)

    $kok.on('click', '.md-menu > .dropdown-toggle', function (e) {
        e.stopPropagation();
        var $g = $(this).parent();
        var acik = $g.hasClass('open');
        $g.toggleClass('open', !acik).children('.dropdown-toggle').attr('aria-expanded', acik ? 'false' : 'true');
    });
    $kok.on('click', '.md-menu .dropdown-menu', function (e) { e.stopPropagation(); });
    $(document).on('click', function () { $kok.find('.md-menu.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false'); });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- sorgu alanları

    $('#btnYardim').on('click', function () {
        var $p = $('#pnlYardim');
        var ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false');
    });

    // Sorgu alanları daraltılabilir (eski West paneli Collapsible); seçim tarayıcıda hatırlanır.
    function kriterGoster(goster) {
        $kok.toggleClass('md-kriter-gizli', !goster);
        $('#pnlKriter').prop('hidden', !goster);
        $('#btnKriterGoster').prop('hidden', goster);
        sakliYaz('kriterGizli', goster ? '0' : '1');
    }
    $('#btnKriterGizle').on('click', function () { kriterGoster(false); });
    $('#btnKriterGoster').on('click', function () { kriterGoster(true); });

    function yil() {
        if ($('#chkYilOnemsiz').prop('checked')) return 0;
        return parseInt($('#txtYil').val(), 10) || 0;
    }
    // Kod ağaçları ve hesap planı için yıl (Yıl Önemsiz ise bu yıl; eski HesapListesiDoldur).
    function kodYil() { return yil() || new Date().getFullYear(); }

    $('#chkYilOnemsiz').on('change', function () { $('#txtYil').prop('disabled', this.checked); });

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: kodYil,
        kurumsalGizle: true,
        adGizle: true
    });

    var hesap = Sgb.hesapKodu('#txtHesapNo', {
        yil: kodYil,
        ad: true,
        denetle: true,
        uc: 'api/muavindefteri/hesap/'
    });

    // Eski hesap alanı açılır listeden "kod - ad" değerini alırdı; sunucu kodu '-' öncesinden ayırır, raporun başlığına aynen yazar.
    function hesapNo() {
        var h = hesap ? hesap.deger() : null;
        var kod = h && h.kod ? h.kod : $.trim($('#txtHesapNo').val());
        if (!kod) return '';
        return h && h.ad ? kod + ' - ' + h.ad : kod;
    }

    // "13.01.31.00- Ad" -> "13.01.31.00"
    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }
    function alanKod(alan) { return kodAl($('.kod-alan[data-alan="' + alan + '"] input').val()); }

    function fisTur() {
        return $.map($('#fisTurler input:checked'), function (c) { return c.value; }).join(',');
    }

    // Eski pgFiltre alan adlarıyla kıstaslar (MuavinDefteriKriter.Alanlar).
    function kriter() {
        var d = muhBirim.deger();
        return {
            yil: yil(), durum: $('#ddlDurum').val() || '', belgeTur: $('#ddlBelgeTur').val() || '',
            listeTur: tanim.dovizVar ? ($('#ddlListeTur').val() || '1') : '',
            hesapNo: hesapNo(), althesapNo: $.trim($('#txtAltHesapNo').val()),
            muhasebe: d.muhasebe || '', birim: d.birim || '',
            yevmiyeNo1: $.trim($('#txtYevmiyeNo1').val()), yevmiyeNo2: $.trim($('#txtYevmiyeNo2').val()),
            belgeNo1: $.trim($('#txtBelgeNo1').val()), belgeNo2: $.trim($('#txtBelgeNo2').val()),
            belgeTarihi1: $('#txtBelgeTarih1').val(), belgeTarihi2: $('#txtBelgeTarih2').val(),
            ilgili: $.trim($('#txtIlgiliAd').val()), ilgiliNo: $.trim($('#txtIlgiliNo').val()),
            kur: alanKod('kur'), fon: alanKod('fon'), fin: alanKod('fin'), eko: alanKod('eko'), peb: alanKod('peb'),
            yevmiyeTarihi1: $('#txtYevmiyeTarih1').val(), yevmiyeTarihi2: $('#txtYevmiyeTarih2').val(),
            durumTarihi1: $('#txtDurumTarih1').val(), durumTarihi2: $('#txtDurumTarih2').val(),
            borc: $.trim($('#txtBorc').val()), alacak: $.trim($('#txtAlacak').val()),
            fisTur: fisTur(),
            islemYapan: $.trim($('#txtIslemYapan').val()), aciklama: $.trim($('#txtAciklama').val()),
            cekNoSorgu: $.trim($('#txtCekNoSorgu').val()),
            OdEsTur: $.trim($('#txtOdEsBeTur').val()), OdEsNo: $.trim($('#txtOdEsBeNo').val()), OdEsTarihi: $('#txtOdEsTarih').val(),
            yatirimProjeNo: $.trim($('#txtYatirimProjeNo').val())
        };
    }

    // Kod ağaçları (eski DropDownField + TreePanel, nodeLoadPar: 1 kurumsal, 2 fonksiyonel, 3 finans, 4 ekonomik, 14 faaliyet).
    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};
    var aktifAlan = null;
    function agacAc($alan) {
        var y = kodYil();
        var tur = $alan.attr('data-tur');
        var ad = $alan.find('label').text();
        aktifAlan = $alan;
        $('#dlgKodBaslik').text(ad + ' Seçimi');
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[tur];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', ad + ' ağacı').appendTo('#agacKutu'), yil: y };
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al(UC + 'kodlar', { yil: a.yil, tur: tur, ust: ust ? ust.kod : '' }).then(function (v) {
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
            if (a.yil !== y) { a.yil = y; a.agac.yenile(); }
        }
    }
    $('.kod-alan .kod-sec').on('click', function () { agacAc($(this).closest('.kod-alan')); });
    $('.kod-alan .kod-temizle').on('click', function () { $(this).closest('.kod-alan').find('input').val('').trigger('focus'); });
    $('.kod-alan input').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
    });

    function varsayilanlar() {
        $('#ddlDurum').val(String(tanim.varsayilanDurum != null ? tanim.varsayilanDurum : 22));
        $('#ddlBelgeTur').val(String(tanim.varsayilanBelgeTur || 0));
        $('#ddlListeTur').val(String(tanim.varsayilanListeTur || 1));
        var fv = $.map(tanim.varsayilanFisTur || [0, 1], String);
        $('#fisTurler input').each(function () { this.checked = fv.indexOf(this.value) >= 0; });
    }

    $('#btnKriterTemizle').on('click', function () {
        $('#frmKriter').find('input[type="text"], input[type="date"]').not('#txtHesapNo').val('');
        if (hesap) hesap.temizle(); else $('#txtHesapNo').val('');
        muhBirim.ata({});
        $('#chkYilOnemsiz').prop('checked', false);
        $('#txtYil').prop('disabled', false).val(String(tanim.yil || new Date().getFullYear()));
        varsayilanlar();
    });

    // ---------------------------------------------------------------- sonuç tablosu

    // Belge numarası: belge çerçevenin "Belge Düzenleme" sekmesinde açılır (eski BelgeGirisAc(yil, belgeNo)).
    function belgeAc(d) {
        if (window.parent && window.parent !== window && typeof window.parent.OemBelgeAc === 'function') {
            window.parent.OemBelgeAc(d.yil, d.belgeNo);
            return;
        }
        Sgb.pencere('../ButceMuhasebe/OdemeEmriMIFFrame?' + $.param({ aramaYok: 1, yil: d.yil, belgeNo: d.belgeNo }), 'Ödeme Emri / MİF Belgesi ' + d.belgeNo);
    }

    function belgeHucre(x) {
        if (!x.data || x.data._toplam || !x.value) return x.value || '';
        return $('<a href="#"></a>').text(x.value).attr('title', 'Belgeyi aç').on('click', function (e) {
            e.preventDefault(); e.stopPropagation();
            belgeAc(x.data);
        })[0];
    }

    function tarihBicim(x) { return Sgb.tarih(x.value); }
    function paraBicim(x) { return para(x.value); }
    function sifirsiz(x) { return x.value === '0' || x.value === 0 ? '' : (x.value || ''); }

    // Sütunlar (eski ColumnModel). gizli: açılışta gizli; liste: TL / döviz listesine göre değişenler.
    var SUTUNLAR = [
        { field: '_sira', headerName: '#', width: 56, minWidth: 48, maxWidth: 70, type: 'rightAligned', suppressMovable: true, sabit: true },
        { field: 'belgeNo', headerName: 'Belge No', minWidth: 110, flex: 110, cellRenderer: belgeHucre, sabit: true },
        { field: 'belgeTarih', headerName: 'Belge Tarihi', minWidth: 90, flex: 90, valueFormatter: tarihBicim },
        { field: 'yevmiyeNo', headerName: 'Yevmiye No', minWidth: 80, flex: 80, type: 'rightAligned', valueFormatter: sifirsiz },
        { field: 'hesapKod', headerName: 'Hesap Kod', minWidth: 100, flex: 100, hide: true },
        { field: 'aciklama', headerName: 'Açıklama', minWidth: 160, flex: 260, tooltipField: 'aciklama' },
        { field: 'borc', headerName: 'Borç', minWidth: 110, flex: 110, type: 'rightAligned', valueFormatter: paraBicim, liste: 'tl' },
        { field: 'alacak', headerName: 'Alacak', minWidth: 110, flex: 110, type: 'rightAligned', valueFormatter: paraBicim, liste: 'tl' },
        { field: 'bakiye', headerName: 'Bakiye', minWidth: 120, flex: 120, type: 'rightAligned', liste: 'tl' },
        { field: 'dovizKurTur', headerName: 'Döviz Tür', minWidth: 70, flex: 70, hide: true },
        { field: 'dovizKur', headerName: 'Döviz Kur', minWidth: 80, flex: 80, type: 'rightAligned', hide: true, valueFormatter: function (x) { return x.value === null || x.value === undefined || x.value === '' ? '' : kur4.format(Number(x.value) || 0); } },
        { field: 'dovizBorc', headerName: 'Döviz Borç', minWidth: 110, flex: 110, type: 'rightAligned', hide: true, valueFormatter: paraBicim, liste: 'doviz' },
        { field: 'dovizAlacak', headerName: 'Döviz Alacak', minWidth: 110, flex: 110, type: 'rightAligned', hide: true, valueFormatter: paraBicim, liste: 'doviz' },
        { field: 'dovizBakiye', headerName: 'Döviz Bakiye', minWidth: 120, flex: 120, type: 'rightAligned', hide: true, liste: 'doviz' },
        { field: 'belgeTur', headerName: 'Belge Türü', minWidth: 60, flex: 60, hide: true },
        { field: 'muhasebe', headerName: 'Muhasebe Birim Kodu', minWidth: 140, flex: 160, hide: true, tooltipField: 'muhasebe' },
        { field: 'birim', headerName: 'Birim Kodu', minWidth: 140, flex: 160, hide: true, tooltipField: 'birim' },
        { field: 'tertip', headerName: 'Tertip', minWidth: 200, flex: 230, hide: true, tooltipField: 'tertip' },
        { field: 'ilgili', headerName: 'İlgili', minWidth: 120, flex: 140, hide: true, tooltipField: 'ilgili' },
        { field: 'yevmiyeTarih', headerName: 'Yevmiye Tarihi', minWidth: 90, flex: 90, hide: true, valueFormatter: tarihBicim },
        { field: 'OEBNo', headerName: 'Ö.Esas Belge No', minWidth: 100, flex: 100, hide: true },
        { field: 'OEBTarih', headerName: 'Ö.Esas Belge Tarihi', minWidth: 100, flex: 100, hide: true, valueFormatter: tarihBicim },
        { field: 'cekNo', headerName: 'Çek/Gönderme No', minWidth: 100, flex: 100, hide: true },
        { field: 'cekTarih', headerName: 'Çek/Gönderme Tarihi', minWidth: 100, flex: 100, hide: true, valueFormatter: tarihBicim },
        { field: 'surecNo', headerName: 'Süreç No', minWidth: 90, flex: 90, hide: true }
    ];

    var grid = Sgb.grid('#grdListe', {
        columnDefs: $.map(SUTUNLAR, function (s) {
            var c = $.extend({}, s);
            delete c.sabit; delete c.liste;
            c.colId = s.field;
            c.sortable = false; // eski sütunların hiçbiri sıralanmıyordu; gruplar sıralı kalır
            return c;
        }),
        getRowId: function (x) { return x.data._id; },
        isFullWidthRow: function (x) { return !!(x.rowNode.data && x.rowNode.data._grup); },
        fullWidthCellRenderer: grupHucre,
        getRowClass: function (x) {
            if (!x.data) return '';
            if (x.data._toplam) return 'md-toplam';
            if (x.data.devir) return 'md-devir';
            return '';
        },
        pagination: true,
        paginationPageSize: 250,
        paginationPageSizeSelector: [50, 100, 250, 500, 1000],
        overlayNoRowsTemplate: '<span>Hesap kodunu ve diğer kıstasları girip Listele\'ye basınız.</span>'
    });

    function grupHucre(x) {
        var d = x.data;
        return $('<div class="md-grup"></div>')
            .append($('<span class="md-grup-etiket"></span>').text('Hesap Kod: '))
            .append($('<b></b>').text(d.hesapKod || ''))
            .append($('<span class="md-grup-sayi"></span>').text(' (' + d._adet + ' kayıt)'))[0];
    }

    // Eski GroupingView (GroupField hesapKod, HideGroupedColumn) + GroupingSummary (borç / alacak toplamları).
    function satirlariGrupla(satirlar) {
        var sonuc = [], sira = 0, no = 0;
        var grup = null, toplam = null;
        function kapat() {
            if (!grup) return;
            sonuc.push(toplam);
        }
        $.each(satirlar, function (i, s) {
            if (!grup || grup.hesapKod !== s.hesapKod) {
                kapat();
                no++;
                grup = { _id: 'g' + no, _grup: true, hesapKod: s.hesapKod, _adet: 0 };
                toplam = { _id: 't' + no, _toplam: true, hesapKod: s.hesapKod, aciklama: 'Toplam', borc: 0, alacak: 0, dovizBorc: 0, dovizAlacak: 0 };
                sonuc.push(grup);
            }
            grup._adet++;
            toplam.borc += Number(s.borc) || 0;
            toplam.alacak += Number(s.alacak) || 0;
            toplam.dovizBorc += Number(s.dovizBorc) || 0;
            toplam.dovizAlacak += Number(s.dovizAlacak) || 0;
            sonuc.push($.extend({ _id: 's' + (++sira), _sira: sira }, s));
        });
        kapat();
        return sonuc;
    }

    // Liste Tür'e göre tutar sütunları (eski Sorgula: TL'de borç / alacak / bakiye, dövizde döviz borç / alacak / bakiye).
    function listeTurUygula(listeTur, dovizVar) {
        if (!dovizVar) return;
        var tl = String(listeTur || '1') === '1';
        var tlSutun = [], dovizSutun = [];
        $.each(SUTUNLAR, function (i, s) {
            if (s.liste === 'tl') tlSutun.push(s.field);
            if (s.liste === 'doviz') dovizSutun.push(s.field);
        });
        grid.setColumnsVisible(tlSutun, tl);
        grid.setColumnsVisible(dovizSutun, !tl);
        sutunListesiYenile();
    }

    // Sütun seçimi (eski sütun başlığı menüsündeki Columns listesi).
    function sutunListesiYenile() {
        var $u = $('#mnSutunlar .md-sutun-liste').empty();
        $.each(SUTUNLAR, function (i, s) {
            if (s.sabit) return;
            var kol = grid.getColumn(s.field);
            var $c = $('<input type="checkbox" />').prop('checked', !!(kol && kol.isVisible())).on('change', function () {
                grid.setColumnsVisible([s.field], this.checked);
            });
            $('<li></li>').append($('<label class="md-sutun"></label>').append($c, document.createTextNode(' ' + s.headerName))).appendTo($u);
        });
    }
    sutunListesiYenile();

    function ozetYaz(satirlar) {
        if (!satirlar || !satirlar.length) { $('#lblOzet').text(''); return; }
        var borc = 0, alacak = 0, gruplar = {};
        $.each(satirlar, function (i, s) { borc += Number(s.borc) || 0; alacak += Number(s.alacak) || 0; gruplar[s.hesapKod] = true; });
        var g = Object.keys(gruplar).length;
        $('#lblOzet').text(satirlar.length + ' satır' + (g > 1 ? ', ' + g + ' hesap' : '') + ' — Borç ' + tutar2.format(borc) + ', Alacak ' + tutar2.format(alacak));
    }

    function hesapVarMi(k) {
        if (k.hesapNo) return true;
        mesaj('Hesap Kod alanı boş bırakılamaz.', 'warning');
        $('#txtHesapNo').trigger('focus');
        return false;
    }

    // Elle yazılan hesap kodunun adı okunuyorsa bitmesi beklenir (kıstasa "kod - ad" yazılsın).
    function hesapHazir(is) {
        var h = hesap && hesap.hazir ? hesap.hazir() : null;
        if (h && h.always) h.always(is); else is();
    }

    function listele() {
        hesapHazir(function () {
            var k = kriter();
            if (!hesapVarMi(k)) return;
            var $d = $('#btnListe').prop('disabled', true);
            grid.setGridOption('loading', true);
            Sgb.al(UC + 'liste', k)
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); grid.setGridOption('rowData', []); ozetYaz([]); return; }
                    var satirlar = v.satirlar || [];
                    listeTurUygula(v.listeTur, v.dovizVar);
                    grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
                    grid.setGridOption('rowData', satirlariGrupla(satirlar));
                    ozetYaz(satirlar);
                    $.each(v.uyarilar || [], function (i, u) { mesaj(u, satirlar.length ? 'warning' : 'info'); });
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
        });
    }

    $('#frmKriter').on('submit', function (e) { e.preventDefault(); listele(); });
    // Kıstas kutularında Enter listeler (eski PropertyGrid'de düzenleme bitince Listele'ye basılırdı).
    $('#frmKriter').on('keydown', 'input[type="text"], input[type="date"], input[type="number"]', function (e) {
        if (e.key === 'Enter' && !$(this).closest('.kod-alan').length) { e.preventDefault(); listele(); }
    });

    // ---------------------------------------------------------------- yazdırma (eski btnListeYazdir_Click / btnListeYazdir2_Click)

    function yazdir(tur, $dugme) {
        hesapHazir(function () {
            var k = kriter();
            if (!hesapVarMi(k)) return;
            $dugme.prop('disabled', true);
            Sgb.indir(UC + 'yazdir', $.extend(k, { tur: tur })).then(function () { $dugme.prop('disabled', false); });
        });
    }
    $('#btnListeYazdir').on('click', function () { yazdir(1, $(this)); });
    $('#btnListeYazdir2').on('click', function () { yazdir(2, $(this)); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al(UC + 'tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            Sgb.yilKutusu('#txtYil', t.yillar, t.yil);
            function doldur(sec, liste) {
                var $s = $(sec).empty();
                $.each(liste || [], function (i, d) { $('<option></option>').val(d.kod).text(d.ad).appendTo($s); });
            }
            doldur('#ddlDurum', t.durumlar);
            doldur('#ddlBelgeTur', t.belgeTurleri);
            doldur('#ddlListeTur', t.listeTurleri);
            $('#grpListeTur').prop('hidden', !t.dovizVar);
            var $f = $('#fisTurler').empty();
            $.each(t.fisTurleri || [], function (i, d) {
                $('<label class="md-secenek"></label>')
                    .append($('<input type="checkbox" />').val(d.kod), document.createTextNode(' ' + d.ad))
                    .appendTo($f);
            });
            varsayilanlar();
        })
        .fail(hataGoster);

    if (sakliOku('kriterGizli') === '1') kriterGoster(false);

    // Dar ekranda tablo, sayfanın kalan yüksekliğine yayılır (geniş ekranda düzen CSS ile tam yüksekliktedir).
    function tabloYay() {
        var el = document.getElementById('grdListe');
        if (!el || window.matchMedia('(min-width: 992px)').matches) { if (el) el.style.height = ''; return; }
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(320, Math.floor(window.innerHeight - ust - 24)) + 'px';
    }
    $(window).on('resize', tabloYay);
    tabloYay();
});
