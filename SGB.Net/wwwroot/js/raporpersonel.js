// Personel Raporları (eski InsanKaynaklari/RaporPersonel.aspx + RaporPersonel.ashx).
// Solda rapor ağacı; rapor seçilince sağda o raporun kriter alanları açılır (eski EkranHazirla). Göster: rapor PDF olarak yeni
// sekmede açılır (eski ashx). İndir: Excel / Word, İndir (PDF): PDF dosyası.
(function ($) {
    'use strict';

    var U = 'api/raporpersonel/';
    var tanim = null;     // /tanim yanıtı
    var rapor = null;     // seçilen raporun alanları
    var birim = { kod: 0, ad: '' };

    function hata(metin) { Sgb.bildirim(metin || 'İşlem yapılamadı.', 'danger'); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 403) hata('Bu işlem için yetkiniz yok.');
        else if (xhr && xhr.status !== 401) hata('İşlem yapılamadı.');
    }

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- çoklu seçim (eski MultiCombo / ExtCokluSecim)

    function cokluSecim(el, degisince) {
        var $el = $(el).addClass('dropdown');
        var $dugme = $('<button type="button" class="btn btn-default btn-sm form-control rp-coklu-dugme" aria-haspopup="true" aria-expanded="false"></button>').appendTo($el);
        var $metin = $('<span class="rp-coklu-metin"></span>').appendTo($dugme);
        $('<span class="caret"></span>').appendTo($dugme);
        var $liste = $('<ul class="dropdown-menu rp-coklu-liste"></ul>').appendTo($el);
        var ogeler = [];

        function metinYaz() {
            var adlar = $.map(ogeler, function (o) { return o.secili ? o.ad : null; });
            $metin.text(adlar.length === 0 ? ($el.data('bos') || '') : adlar.length > 1 ? '[' + adlar.join(', ') + ']' : adlar[0]);
            $dugme.attr('title', adlar.join(', '));
        }
        function ciz() {
            $liste.empty();
            if (!ogeler.length) $('<li class="rp-coklu-bos">Seçenek yok</li>').appendTo($liste);
            $.each(ogeler, function (i, o) {
                var $l = $('<label></label>').appendTo($('<li></li>').appendTo($liste));
                $('<input type="checkbox" />').prop('checked', !!o.secili).on('change', function () {
                    o.secili = this.checked;
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
            $('.rp-coklu.open').removeClass('open');
            $el.toggleClass('open', !acik);
            $dugme.attr('aria-expanded', !acik);
        });
        $(document).on('click', function () { $el.removeClass('open'); $dugme.attr('aria-expanded', false); });
        ciz();
        return {
            doldur: function (liste) { ogeler = $.map(liste || [], function (o) { return { kod: o.kod, ad: o.ad, secili: false }; }); ciz(); },
            secilenler: function (alan) { return $.map(ogeler, function (o) { return o.secili ? o[alan] : null; }); }
        };
    }

    var grup = cokluSecim('#cokGrup', function () { turleriOku(); });
    var tur = cokluSecim('#cokTur');
    var izinTur = cokluSecim('#cokIzinTur');

    // Eski ddlPersonelTipGrup_Changed: seçilen gruplara göre türler yeniden okunur.
    function turleriOku() {
        return Sgb.al(U + 'turler', { gruplar: grup.secilenler('ad').join(';') }).done(function (v) {
            if (v.hata) { hata(v.hata); return; }
            tur.doldur(v.turler);
        }).fail(hataGoster);
    }

    // ---------------------------------------------------------------- dinamik alan listesi ve personel tabloları

    var gridDinamik = Sgb.grid('#grdDinamik', {
        columnDefs: [{ field: 'baslik', headerName: 'Başlık', flex: 1, sortable: false, rowDrag: true }],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
        rowDragManaged: true,
        getRowId: function (p) { return String(p.data.kod); }
    });

    function resimliMi() { return $('#grdPersonel').data('resimli') === 1; }

    var gridPersonel = Sgb.grid('#grdPersonel', {
        columnDefs: [
            {
                field: 'resimYol', headerName: '', width: 84, sortable: false, hide: true,
                cellRenderer: function (p) {
                    if (!p.value) return '';
                    var img = document.createElement('img');
                    img.className = 'rp-resim';
                    img.alt = '';
                    img.loading = 'lazy';
                    img.src = '../InsanKaynaklari/' + p.value;
                    return img;
                }
            },
            { field: 'tcKimlikNo', headerName: 'TC Kimlik', width: 110 },
            { field: 'ad', headerName: 'Ad', width: 120 },
            { field: 'soyad', headerName: 'Soyad', width: 120 },
            { field: 'unvanAd', headerName: 'Unvan', width: 160 },
            { field: 'personelTipAd', headerName: 'Per. Tür', width: 120 },
            { field: 'cinsiyetAd', headerName: 'Cins.', width: 70 }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
        getRowId: function (p) { return String(p.data.kod); },
        getRowHeight: function () { return resimliMi() ? 96 : undefined; },
        overlayNoRowsTemplate: '<span class="rp-ipucu">Arama ölçütlerini girip Listele\'ye basınız.</span>'
    });

    // Eski tfArama_Ara_Clicked
    function personelListele(resimli) {
        $('#grdPersonel').data('resimli', resimli ? 1 : 0);
        gridPersonel.setColumnsVisible(['resimYol'], !!resimli);
        gridPersonel.setGridOption('loading', true);
        Sgb.al(U + 'personeller', {
            raporKod: rapor ? rapor.kod : '',
            ara: $('#txtAra').val(),
            birim: birim.kod || 0,
            gruplar: grup.secilenler('ad').join(';'),
            turler: tur.secilenler('kod').join(';'),
            aktif: $('#ddlAktiflik').val(),
            tarih: $('#txtTarihSorgu').val(),
            tarihTur: $('#ddlTarihSorgu').val(),
            site: parseInt($('#ddlSite').val(), 10) || 0
        }).done(function (v) {
            if (v.hata) { hata(v.hata); gridPersonel.setGridOption('rowData', []); return; }
            gridPersonel.setGridOption('rowData', v.satirlar || []);
            gridPersonel.resetRowHeights();
        }).fail(function (xhr) { hataGoster(xhr); gridPersonel.setGridOption('rowData', []); })
            .always(function () { gridPersonel.setGridOption('loading', false); });
    }
    $('#btnListeResimsiz').on('click', function () { personelListele(false); });
    $('#btnListeResimli').on('click', function () { personelListele(true); });
    $('#txtAra').on('keydown', function (e) {
        if (e.key === 'Enter') { e.preventDefault(); if (rapor && rapor.kisiAramaGrid) personelListele(resimliMi()); }
    });

    // ---------------------------------------------------------------- birim seçimi

    function birimYaz(kod, ad) {
        birim = { kod: kod || 0, ad: ad || '' };
        $('#txtBirimAd').val(birim.ad).attr('title', birim.ad);
    }

    var dlgBirim = document.getElementById('dlgBirim');
    var birimAgac = null, birimAra = '';
    function birimAgaciKur() {
        birimAgac = Sgb.agac('#agcBirim', {
            yukle: function (ust) {
                return Sgb.al(U + 'orgagac', ust ? { ust: ust.kod } : { ust: 0, ara: birimAra }).then(function (v) {
                    if (v.hata) { hata(v.hata); return []; }
                    return v.satirlar || [];
                });
            },
            secilince: function (d) { birimYaz(d.kod, d.birimAd || d.ad); dlgBirim.close(); },
            ilkAcik: true,
            bosMetin: 'Birim bulunamadı.',
            hata: hataGoster
        });
    }
    $('#btnBirimSec, #txtBirimAd').on('click', function () {
        if (!birimAgac) birimAgaciKur();
        dlgBirim.showModal();
        $('#txtBirimAra').trigger('focus');
    });
    function birimAramaYap() {
        var a = $.trim($('#txtBirimAra').val());
        if (a && a.length < 3) { Sgb.bildirim('Aramak için en az 3 harf yazınız.', 'warning'); return; }
        birimAra = a;
        birimAgac.yenile();
    }
    $('#btnBirimAra').on('click', birimAramaYap);
    $('#txtBirimAra').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); birimAramaYap(); } });
    $('#btnBirimSil').on('click', function () { birimYaz(0, ''); });

    // ---------------------------------------------------------------- rapor seçimi (eski SatirSecildi / EkranHazirla)

    function alanGoster(ad, goster) {
        $('#frmKriter [data-alan="' + ad + '"]').prop('hidden', !goster);
    }

    function raporSec(kod) {
        rapor = tanim && tanim.raporlar ? tanim.raporlar[kod] || null : null;
        var r = rapor;
        $('#lblRaporAd').text(r ? r.ad : 'Lütfen Rapor Seçiniz');
        $('#rpDugmeler').prop('hidden', !r);
        $('#frmKriter').prop('hidden', !r);
        $('#lblBos').prop('hidden', !!r);
        if (!r) return;

        alanGoster('birim', r.birim);
        alanGoster('kadroDerece', r.kadroDerece);
        alanGoster('islemSira', r.islemSira);
        alanGoster('personelTur', r.personelTur);
        alanGoster('yukumlu', r.yukumlu);
        alanGoster('kisiArama', r.kisiArama);
        alanGoster('kisiAramaGrid', r.kisiAramaGrid);
        alanGoster('dinamik', r.dinamik);
        alanGoster('sonTarih', r.sonTarih);
        alanGoster('aktiflik', r.aktiflik);
        alanGoster('site', r.site);
        alanGoster('daire', r.daire);
        alanGoster('yil', r.yil);
        alanGoster('ay', r.ay);
        alanGoster('tarihSorgu', r.tarihSorgu);
        alanGoster('izinTur', r.izinTur);

        if (r.sonTarih) {
            if (!$('#txtSonTarih').val()) $('#txtSonTarih').val(tanim.bugun);
            $('#lblSonTarih').text(r.kod === 'IK021a' ? 'Görev Tarihi' : 'Son Tarih');
        }

        // İzin raporlarında yıllar (dönem yılı), diğerlerinde süre (-1: yok).
        if (r.izinTur) {
            alanGoster('yilArasi', true);
            $('#lblYilArasi').text('Yıllar');
            var d = tanim.donemYili;
            $('#txtYil1, #txtYil2').attr({ min: 2000, max: d + 1 }).val(d);
        } else {
            alanGoster('yilArasi', r.yilArasi);
            $('#lblYilArasi').text('Süre');
            $('#txtYil1, #txtYil2').attr({ min: -1, max: 90 }).val(-1);
        }

        var $s = $('#ddlSiralama').empty();
        $('<option value="-1"></option>').appendTo($s);
        $.each(r.siralamalar || [], function (i, ad) { $('<option></option>').val(i).text(ad).appendTo($s); });
        alanGoster('siralama', (r.siralamalar || []).length > 0);

        if (r.dinamik && !gridDinamik.getDisplayedRowCount())
            gridDinamik.setGridOption('rowData', tanim.dinamikKolonlar || []);
    }

    // ---------------------------------------------------------------- rapor alma (eski btnYazdir_Click)

    function parametreler(tusTur) {
        var r = rapor;
        var secililer = $.map(gridPersonel.getSelectedRows(), function (s) { return s.kod; });
        var dinamikler = [];
        gridDinamik.forEachNodeAfterFilterAndSort(function (n) { if (n.isSelected()) dinamikler.push(n.data.kod); });
        return {
            tur: tusTur,
            raporKod: r.kod,
            gruplar: grup.secilenler('ad').join(';'),
            turler: tur.secilenler('kod').join(';'),
            islemSira: $('#txtIslemSira').val(),
            birim: birim.kod || 0,
            birimAd: birim.ad,
            altBirimli: $('#chkAltBirimli').prop('checked') ? 1 : 0,
            kadroDerece: parseInt($('#ddlKadroDerece').val(), 10) || 0,
            secililer: secililer.join(','),
            sonTarih: $('#txtSonTarih').val(),
            dinamikler: dinamikler.join(','),
            ara: $('#txtAra').val(),
            aktif: $('#ddlAktiflik').val(),
            site: parseInt($('#ddlSite').val(), 10) || 0,
            daire: parseInt($('#ddlDaire').val(), 10) || 0,
            yil: parseInt($('#txtYil').val(), 10) || 0,
            ay: parseInt($('#ddlAy').val(), 10) || 0,
            yil1: parseInt($('#txtYil1').val(), 10) || 0,
            yil2: parseInt($('#txtYil2').val(), 10) || 0,
            yukumlu: $('input[name=yukumlu]:checked').val() || 0,
            tarih: $('#txtTarihSorgu').val(),
            tarihTur: $('#ddlTarihSorgu').val(),
            izinTurler: izinTur.secilenler('kod').join(';'),
            siralama: $('#ddlSiralama').val() || -1
        };
    }

    // Eski önkontroller (sunucu da denetler; burada istek göndermeden uyarılır).
    function denetle(p) {
        var h = [];
        if (rapor.grup === 'GR009' && !p.dinamikler) h.push('Lütfen raporda görmek istediğiniz alanları seçiniz.');
        if (rapor.kod === 'IK045' && !p.secililer) h.push('Lütfen personel seçiniz.');
        if (h.length) { Sgb.bildirim(h.join(' '), 'warning'); return false; }
        return true;
    }

    function dugmeler(acik) { $('#rpDugmeler .btn').prop('disabled', !acik); }

    // Göster: PDF yeni sekmede açılır (eski SayfaAc + RaporPersonel.ashx). Sekme tıklama anında açılır ki tarayıcı engellemesin.
    $('#btnGoster').on('click', function () {
        if (!rapor) return;
        var p = parametreler(1);
        if (!denetle(p)) return;
        var sekme = window.open('', '_blank');
        if (sekme) sekme.document.title = 'Rapor hazırlanıyor...';
        dugmeler(false);
        fetch(Sgb.KOK + U + 'rapor?' + $.param(p), { credentials: 'same-origin', cache: 'no-store' }).then(function (y) {
            if (y.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            var t = y.headers.get('Content-Type') || '';
            if (!y.ok || t.indexOf('pdf') < 0) {
                if (sekme) sekme.close();
                if (t.indexOf('json') >= 0) return y.json().then(function (v) { hata(v.hata || 'Rapor hazırlanamadı.'); });
                hata(y.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Rapor hazırlanamadı.');
                return;
            }
            return y.blob().then(function (b) {
                var adres = URL.createObjectURL(b);
                if (sekme) sekme.location.href = adres;
                else window.location.href = adres;
                setTimeout(function () { URL.revokeObjectURL(adres); }, 60000);
            });
        }).catch(function () {
            if (sekme) sekme.close();
            hata('Rapor hazırlanamadı.');
        }).finally(function () { dugmeler(true); });
    });

    function indir(tusTur) {
        if (!rapor) return;
        var p = parametreler(tusTur);
        if (!denetle(p)) return;
        dugmeler(false);
        Sgb.indir(U + 'rapor', p).finally(function () { dugmeler(true); });
    }
    $('#btnIndir').on('click', function () { indir(2); });
    $('#btnIndirPdf').on('click', function () { indir(3); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    function ekleSecenek($s, kod, ad) { $('<option></option>').val(kod).text(ad).appendTo($s); }

    var $kd = $('#ddlKadroDerece').empty();
    ekleSecenek($kd, 0, '');
    for (var i = 1; i <= 15; i++) ekleSecenek($kd, i, String(i));

    Sgb.al(U + 'tanim').done(function (v) {
        if (v.hata) { hata(v.hata); return; }
        tanim = v;
        grup.doldur($.map(v.gruplar || [], function (g) { return { kod: g.ad, ad: g.ad }; }));
        tur.doldur(v.turler);
        izinTur.doldur(v.izinTurleri);

        var $site = $('#ddlSite').empty();
        $.each(v.siteler || [], function (j, s) { ekleSecenek($site, s.kod, s.ad); });

        var $ay = $('#ddlAy').empty();
        ekleSecenek($ay, 0, '');
        var ayAdlari = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
        $.each(ayAdlari, function (j, a) { ekleSecenek($ay, j + 1, a); });
        $ay.val(String(v.ay));
        $('#txtYil').val(v.yil);
        $('#txtTarihSorgu').val(v.bugun);

        // Rapor ağacı: veri sayfayla gelir, Sgb.agac düzey düzey çizer.
        function altlar(d) { return $.map(d.alt || [], function (a) { return { kod: a.kod, ad: a.ad, altVar: !!(a.alt && a.alt.length), alt: a.alt }; }); }
        Sgb.agac('#agcRaporlar', {
            yukle: function (ust) { return $.Deferred().resolve(ust ? altlar(ust) : altlar({ alt: v.agac })).promise(); },
            secilince: function (d) { raporSec(d.kod); },
            simge: function (d) { return d.altVar ? 'glyphicon-folder-close' : 'glyphicon-file'; },
            bosMetin: 'Rapor yok.'
        });
    }).fail(hataGoster);
})(jQuery);
