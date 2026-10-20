// İl Koordinatörlüğü Ödenek Talep İşlemleri (ButceMuhasebe/TKDKTalepListesi) ve Talep Giriş (TKDKTalepGiris); eski aynı adlı Ext.NET
// sayfaları. Talep Giriş menüde yoktur, listeden pencere (Sgb.pencere) olarak açılır. Uçlar: /api/tkdktalep/* (Muhasebe/TKDKTalepUclari.cs).
// Muhasebe / birim seçimi Sgb.muhasebeBirim, tertip seçimi Sgb.tertipSec.
$(function () {
    'use strict';

    var UC = 'api/tkdktalep/';
    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function bos(x) { return x === null || x === undefined || x === ''; }
    function para0(x) { return bos(x.value) ? '' : tamSayi.format(Number(x.value) || 0); }
    function para2(x) { return bos(x.value) ? '' : tutar2.format(Number(x.value) || 0); }
    function tarih(x) { return x.value ? Sgb.tarih(x.value) : ''; }

    // "1.234.567" / "1234567,4" -> 1234567 (eski NumberField: ondalıksız)
    function sayiOku(s) {
        if (typeof s === 'number') return Math.round(s);
        s = $.trim(String(s || '')).replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isNaN(n) ? 0 : Math.round(n);
    }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });
    $(document).on('click', 'dialog [data-kapat]', function (e) { e.stopPropagation(); this.closest('dialog').close(); });

    // Ödeme emri belgesi pencerede (eski OdemeEmrimifFrame bağlantısı). Mümkünse en üstteki sayfanın penceresi kullanılır.
    function odemeEmriAc(yil, belgeNo) {
        if (bos(belgeNo)) return;
        var ust = Sgb;
        try {
            if (window.top !== window && window.top.Sgb && window.top.Sgb.pencere) ust = window.top.Sgb;
        } catch (e) { /* başka kökenden açılmış */ }
        ust.pencere('../ButceMuhasebe/OdemeEmriMIFFrame?' + $.param({ aramaYok: 1, yil: yil, belgeNo: belgeNo }), 'Ödeme Emri ' + yil + '/' + belgeNo);
    }

    // Tablolar sayfanın kalan yüksekliğini doldurur.
    function yay() {
        $.each(['grdTanim', 'grdDetay1', 'grdDetay2'], function (i, ad) {
            var el = document.getElementById(ad);
            if (!el || el.offsetParent === null) return;
            var ust = el.getBoundingClientRect().top;
            el.style.height = Math.max(ad === 'grdTanim' ? 300 : 220, Math.floor(window.innerHeight - ust - 24)) + 'px';
        });
    }
    $(window).on('resize', yay);

    if ($('.tk-liste').length) listeSayfasi();
    if ($('.tk-form').length) formSayfasi();

    // ================================================================ Talep Listesi (eski TKDKTalepListesi.aspx)

    function listeSayfasi() {
        var yonetici = $('.tk-liste').data('yonetici') === 1 || $('.tk-liste').data('yonetici') === '1';
        var tanim = { yil: new Date().getFullYear(), ay: new Date().getMonth() + 1, muhasebe: '', birim: '' };

        var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
            kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal' },
            kurumsalGizle: true,
            adGizle: true,
            yil: function () { return parseInt($('#txtYil').val(), 10) || new Date().getFullYear(); }
        });

        var grid = Sgb.grid('#grdTanim', {
            columnDefs: [
                {
                    colId: 'ac', headerName: 'Aç', width: 46, sortable: false, resizable: false, pinned: 'left',
                    cellRenderer: function (p) {
                        if (!p.data) return '';
                        return $('<button type="button" class="btn btn-link btn-xs tk-komut" title="Talebi aç" aria-label="Talebi aç">'
                            + '<span class="glyphicon glyphicon-edit" aria-hidden="true"></span></button>')
                            .on('click', function (e) { e.stopPropagation(); talepAc(p.data.KOD); })[0];
                    }
                },
                { field: 'YIL', headerName: 'Talep Yıl', width: 80 },
                { field: 'AY', headerName: 'Ay', width: 80 },
                { field: 'MUHASEBEKOD', headerName: 'Muhasebe', width: 85 },
                { field: 'BIRIMKOD', headerName: 'Birim', width: 70 },
                { field: 'TUR', headerName: 'Talep Tür', width: 120 },
                { field: 'TUTAR', headerName: 'Tutar', width: 115, type: 'rightAligned', valueFormatter: para2 },
                { field: 'DURUM', headerName: 'Durum', width: 110 },
                { field: 'ISLEMYAPAN', headerName: 'İşlem Yapan', width: 120 },
                { field: 'ISLEMTARIHI', headerName: 'İşlem Tarih', width: 95, valueFormatter: tarih },
                { field: 'ODEMEEMRINO1', headerName: '1.Muhasebe Fişi No', width: 110, cellRenderer: belgeHucre },
                { field: 'ODEMEEMRINO2', headerName: '2.Muhasebe Fişi No', width: 110, cellRenderer: belgeHucre },
                { field: 'ODEMEEMRINO3', headerName: '3.Muhasebe Fişi No', width: 110, cellRenderer: belgeHucre },
                { field: 'ODEMEEMRINO4', headerName: '4.Muhasebe Fişi No', width: 110, cellRenderer: belgeHucre },
                { field: 'TUTAR1', headerName: '1.Hafta Tutar', width: 110, type: 'rightAligned', valueFormatter: para2 },
                { field: 'TUTAR2', headerName: '2.Hafta Tutar', width: 110, type: 'rightAligned', valueFormatter: para2 },
                { field: 'TUTAR3', headerName: '3.Hafta Tutar', width: 110, type: 'rightAligned', valueFormatter: para2 },
                { field: 'TUTAR4', headerName: '4.Hafta Tutar', width: 110, type: 'rightAligned', valueFormatter: para2 }
            ],
            defaultColDef: { wrapHeaderText: true, autoHeaderHeight: true },
            getRowId: function (p) { return String(p.data.KOD); },
            rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
            selectionColumnDef: { width: 36, pinned: 'left' },
            overlayNoRowsTemplate: '<span>Kıstasları seçip Listele\'ye basınız.</span>',
            onRowDoubleClicked: function (e) { if (e.data) talepAc(e.data.KOD); }
        });

        // Muhasebe fişi numarası ödeme emri belgesini açar.
        function belgeHucre(p) {
            if (!p.data || bos(p.value)) return '';
            return $('<a href="#"></a>').text(p.value).attr('title', 'Ödeme emri belgesini aç')
                .on('click', function (e) { e.preventDefault(); e.stopPropagation(); odemeEmriAc(p.data.YIL, p.value); })[0];
        }

        function kriter() {
            var mb = muhBirim.deger();
            var k = {
                yil: parseInt($('#txtYil').val(), 10) || 0,
                ay: parseInt($('#ddlAy').val(), 10) || 0,
                talepTur: parseInt($('#ddlTalepTur').val(), 10) || 0,
                durum: parseInt($('#ddlDurum').val(), 10) || 0
            };
            if (mb.muhasebe) k.muhasebe = mb.muhasebe;
            if (mb.birim) k.birim = mb.birim;
            return k;
        }

        // Eski btnListele_Click
        function listele() {
            var $d = $('#btnListele').prop('disabled', true);
            grid.setGridOption('loading', true);
            return Sgb.al(UC + 'liste', kriter())
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    var satirlar = v.satirlar || [];
                    grid.setGridOption('rowData', satirlar);
                    grid.setGridOption('overlayNoRowsTemplate', '<span>Kıstaslara uygun talep bulunamadı.</span>');
                    var t = 0;
                    $.each(satirlar, function (i, r) { t += Number(r.TUTAR) || 0; });
                    $('#lblOzet').text(satirlar.length ? satirlar.length + ' talep, toplam ' + tutar2.format(t) : '');
                })
                .fail(hataGoster)
                .always(function () { grid.setGridOption('loading', false); $d.prop('disabled', false); yay(); });
        }
        $('#btnListele').on('click', listele);
        $('#frmKriter').on('keydown', 'input', function (e) { if (e.key === 'Enter') { e.preventDefault(); listele(); } });

        // Eski btnSorguTemizle_Click
        $('#btnSorguTemizle').on('click', function () {
            $('#txtYil').val(String(tanim.yil));
            $('#ddlAy').val(String(tanim.ay));
            muhBirim.ata({});
            $('#ddlTalepTur').val('');
            $('#ddlDurum').val('0');
        });

        // Eski TalepAc: talep formu pencerede; kapanınca liste yenilenir.
        function talepAc(kod) {
            Sgb.pencere('TKDKTalepGiris?' + $.param({ kod: kod || '' }), kod ? 'Talep Giriş' : 'Yeni Talep', listele);
        }
        $('#btnYeni').on('click', function () { talepAc(''); });
        // Talep formu kaydedip silince listeyi yeniler (eski parent btnListele.click).
        window.TkdkTalepListeYenile = listele;

        function yilAy() {
            var k = kriter();
            if (!k.yil || !k.ay) { mesaj('Yıl veya ay bilgisi boş bırakılamaz. Lütfen ilgili yıl ve ay bilgisini seçiniz', 'warning'); return null; }
            return k;
        }

        function sonucGoster(metin, belgeler) {
            var $s = $('#tkSonuc').empty().prop('hidden', !metin);
            if (!metin) return;
            $('<div></div>').text(metin).appendTo($s);
            $.each(belgeler || [], function (i, b) {
                $('<a href="#" class="tk-belge"></a>').text('Belge ' + b.yil + '/' + b.belgeNo + (b.birim ? ' (' + b.birim + ')' : ''))
                    .on('click', function (e) { e.preventDefault(); odemeEmriAc(b.yil, b.belgeNo); }).appendTo($s);
            });
            $('<button type="button" class="close" aria-label="Kapat"><span aria-hidden="true">&times;</span></button>')
                .on('click', function () { $s.prop('hidden', true); yay(); }).prependTo($s);
            yay();
        }

        // Eski btnListeYazdir_Click
        $('#btnListeYazdir').on('click', function () {
            var $d = $(this).prop('disabled', true);
            Sgb.indir(UC + 'liste/yazdir', kriter()).then(function () { $d.prop('disabled', false); });
        });

        // Eski btnYazdir_Click (Rapor ve Olur Eki)
        $('#btnRapor').on('click', function () {
            var k = yilAy();
            if (!k) return;
            var $d = $(this).prop('disabled', true);
            Sgb.indir(UC + 'rapor', { yil: k.yil, ay: k.ay }).then(function () { $d.prop('disabled', false); });
        });

        // ------------------------------------------------------------ imza bilgileri (eski wndImzaBilgileri)

        var dlgImza = document.getElementById('dlgImza');
        $('#btnImzaBilgileri').on('click', function () {
            Sgb.al(UC + 'imza').done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#txtDuzenleyenAdi').val(v.duzenleyenAdi || '');
                $('#txtDuzenleyenGorev').val(v.duzenleyenGorev || '');
                $('#txtOnaylayanAdi').val(v.onaylayanAdi || '');
                $('#txtOnaylayanGorev').val(v.onaylayanGorev || '');
                dlgImza.showModal();
                $('#txtDuzenleyenAdi').trigger('focus');
            }).fail(hataGoster);
        });
        $('#btnImzaKaydet').on('click', function () {
            if (!window.confirm('Bilgiler kayıt edilecek. Onaylıyor musunuz?')) return;
            var $d = $(this).prop('disabled', true);
            Sgb.gonder(UC + 'imza', {
                duzenleyenAdi: $('#txtDuzenleyenAdi').val(),
                duzenleyenGorev: $('#txtDuzenleyenGorev').val(),
                onaylayanAdi: $('#txtOnaylayanAdi').val(),
                onaylayanGorev: $('#txtOnaylayanGorev').val()
            }).done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi || 'Kaydedildi.', 'success');
                dlgImza.close();
            }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
        });

        // ------------------------------------------------------------ hafta sorusu (eski OdemeEmriHaftaSorOlustur)

        var dlgHafta = document.getElementById('dlgHafta');
        var haftaIsi = null;
        function haftaSor(baslik, is) {
            if (!yilAy()) return;
            $('#dlgHaftaBaslik').text(baslik);
            $('#lblHaftaSoru').text(baslik === 'Banka Listesi'
                ? 'Kaçıncı haftanın banka listesini oluşturmak istiyorsunuz? 1-4 arasında değer girin.'
                : 'Kaçıncı haftanın ödeme emri belgesini oluşturmak istiyorsunuz? 1-4 arasında değer girin.');
            $('#txtHafta').val('');
            haftaIsi = is;
            dlgHafta.showModal();
            $('#txtHafta').trigger('focus');
        }
        function haftaTamam() {
            var h = parseInt($('#txtHafta').val(), 10);
            if (!(h >= 1 && h <= 4)) { mesaj('1-4 arasında bir hafta giriniz.', 'warning'); return; }
            dlgHafta.close();
            var is = haftaIsi;
            haftaIsi = null;
            if (is) is(h);
        }
        $('#btnHaftaTamam').on('click', haftaTamam);
        $('#frmHafta').on('keydown', 'input', function (e) { if (e.key === 'Enter') { e.preventDefault(); haftaTamam(); } });

        // ------------------------------------------------------------ ödeme emri menüsü

        var $grp = $('#grpOdemeEmri');
        function menuKapat() { $grp.removeClass('open'); $('#btnOdemeEmri').attr('aria-expanded', 'false'); }
        $('#btnOdemeEmri').on('click', function (e) {
            e.stopPropagation();
            var acik = !$grp.hasClass('open');
            $grp.toggleClass('open', acik);
            $(this).attr('aria-expanded', acik ? 'true' : 'false');
        });
        $(document).on('click', menuKapat);
        $(document).on('keydown', function (e) { if (e.key === 'Escape') menuKapat(); });

        // Eski btnOdemeEmriNakit_Click
        $('#mnuOdemeEmriNakit').on('click', function (e) {
            e.preventDefault();
            menuKapat();
            haftaSor('Ödeme Emri Oluştur', function (hafta) {
                var k = kriter();
                Sgb.gonder(UC + 'odeme-emri/nakit', { yil: k.yil, ay: k.ay, hafta: hafta })
                    .done(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return; }
                        sonucGoster(v.bilgi, [{ yil: v.yil, belgeNo: v.belgeNo }]);
                        listele();
                    })
                    .fail(hataGoster);
            });
        });

        // Eski BankaListesi (DirectMethod, xlsm)
        $('#btnBankaListesi').on('click', function () {
            haftaSor('Banka Listesi', function (hafta) {
                var k = kriter();
                var $d = $('#btnBankaListesi').prop('disabled', true);
                Sgb.indir(UC + 'banka-listesi', { yil: k.yil, ay: k.ay, hafta: hafta }).then(function () { $d.prop('disabled', false); });
            });
        });

        // ------------------------------------------------------------ ödeme emri (aktarma): tertip penceresi (eski wndNakitTertipSec)

        var dlgTertip = document.getElementById('dlgTertip');
        var gridTertip = null;
        function tertipGrid() {
            if (gridTertip) return gridTertip;
            gridTertip = Sgb.grid('#grdTertip', {
                columnDefs: [
                    { field: 'BIRIMKOD', headerName: 'Birim', width: 70 },
                    { field: 'KURKOD', headerName: 'Kur.Kod', width: 110 },
                    { field: 'EKOKOD', headerName: 'Eko.Kod', width: 80 },
                    { field: 'DUSULEN', headerName: 'Düşülen', width: 110, type: 'rightAligned', valueFormatter: para2 },
                    { field: 'EKLENEN', headerName: 'Eklenen', width: 110, type: 'rightAligned', valueFormatter: para2 },
                    { field: 'TERTIP', headerName: 'Tertip', minWidth: 220, flex: 1, tooltipField: 'TERTIP', cellClass: 'tk-salt-hucre' },
                    {
                        colId: 'sec', headerName: 'Tertip Seç', width: 70, sortable: false, resizable: false,
                        cellRenderer: function (p) {
                            if (!p.data) return '';
                            return $('<button type="button" class="btn btn-link btn-xs tk-komut" title="Tertip seç" aria-label="Tertip seç">'
                                + '<span class="glyphicon glyphicon-edit" aria-hidden="true"></span></button>')
                                .on('click', function (e) { e.stopPropagation(); tertipAc(p.node); })[0];
                        }
                    }
                ],
                getRowId: function (p) { return String(p.data.KOD); },
                overlayNoRowsTemplate: '<span>Onaylanmış ödenek aktarma talebi bulunamadı.</span>'
            });
            return gridTertip;
        }

        // Eski btnTertipEkraniHazirla_Click
        $('#mnuOdemeEmriAktarma').on('click', function (e) {
            e.preventDefault();
            menuKapat();
            var g = tertipGrid();
            g.setGridOption('rowData', []);
            g.setGridOption('loading', true);
            dlgTertip.showModal();
            Sgb.al(UC + 'tertip', kriter())
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    g.setGridOption('rowData', v.satirlar || []);
                })
                .fail(hataGoster)
                .always(function () { g.setGridOption('loading', false); });
        });

        // Eski TertipAc / ListeTertipAktar: ListeTertip yerine ortak tertip seçimi; seçilen tertipler satırdan başlayarak sırayla yazılır.
        function tertipAc(node) {
            var d = node.data;
            var mb = muhBirim.deger();
            Sgb.tertipSec({
                tur: 'muh',
                yil: parseInt($('#txtYil').val(), 10) || new Date().getFullYear(),
                muhasebe: mb.muhasebe || 0,
                birim: mb.birim || '',
                kur: d.KURKOD || '',
                hesapKod: '830' + (d.EKOKOD || ''),
                secKapat: true,
                secilince: function (satirlar) {
                    var g = tertipGrid();
                    var sira = node.rowIndex;
                    $.each(satirlar || [], function (i, s) {
                        var n = g.getDisplayedRowAtIndex(sira++);
                        if (n) n.setDataValue('TERTIP', s.TERTIP);
                    });
                }
            });
        }

        // Eski btnOdemeEmriAktarma_Click
        $('#btnAktarmaOlustur').on('click', function () {
            var k = yilAy();
            if (!k) return;
            if (!window.confirm('Ödeme emri oluşturulacak. Onaylıyor musunuz?')) return;
            var tertipler = [];
            tertipGrid().forEachNode(function (n) { if (n.data) tertipler.push({ KOD: n.data.KOD, TERTIP: n.data.TERTIP || '' }); });
            var $d = $(this).prop('disabled', true);
            Sgb.gonder(UC + 'odeme-emri/aktarma', { yil: k.yil, ay: k.ay, tertipler: tertipler })
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    dlgTertip.close();
                    sonucGoster(v.bilgi, v.belgeler);
                    listele();
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });

        // ------------------------------------------------------------ onayla / reddet (eski btnOnayla_Click / btnReddet_Click)

        function durumDegistir(adres, soru) {
            var secili = grid.getSelectedRows();
            if (!secili.length) { mesaj('Durum değişikliği için listeden seçim yapın', 'warning'); return; }
            if (!window.confirm(soru)) return;
            Sgb.gonder(UC + adres, { kodlar: $.map(secili, function (r) { return String(r.KOD); }) })
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    mesaj(v.bilgi || 'Güncellendi.', 'success');
                    listele();
                })
                .fail(hataGoster);
        }
        $('#btnOnayla').on('click', function () { durumDegistir('onayla', 'Seçili talepler kabul edilecek. Onaylıyor musunuz?'); });
        $('#btnReddet').on('click', function () { durumDegistir('reddet', 'Seçili talepler reddedilecek. Onaylıyor musunuz?'); });

        // ------------------------------------------------------------ açılış (eski Page_Load: bu yıl / ay, durum Hepsi, liste)

        Sgb.al(UC + 'tanim')
            .done(function (t) {
                if (t.hata) { mesaj(t.hata, 'danger'); return; }
                tanim = t;
                $('#txtYil').val(String(t.yil));
                $('#ddlAy').val(String(t.ay));
                $('#ddlDurum').val('0');
                if (t.muhasebe || t.birim) {
                    muhBirim.ata({ muhasebe: t.muhasebe, birim: t.birim });
                    muhBirim.yenile();
                }
                listele();
            })
            .fail(hataGoster);
        yay();
        if (!yonetici) $('#btnOnayla, #btnReddet, #btnRapor').remove();
    }

    // ================================================================ Talep Giriş (eski TKDKTalepGiris.aspx)

    function formSayfasi() {
        var kod = new URLSearchParams(window.location.search).get('kod') || '';
        var yuklu = { tur: 0, yil: 0, ay: 0 }; // eski hdnTalepTur / hdnYil / hdnAy: kayıtlı talebin değerleri

        function cerceve() {
            try { return window.frameElement ? window.frameElement.closest('dialog') : null; } catch (e) { return null; }
        }
        function pencereKapat() { var d = cerceve(); if (d) d.close(); }
        function listeYenile() {
            try { if (window.parent !== window && window.parent.TkdkTalepListeYenile) window.parent.TkdkTalepListeYenile(); } catch (e) { /* yok */ }
        }
        if (cerceve()) $('#btnKapat').prop('hidden', false).on('click', pencereKapat);

        var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
            kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal' },
            kurumsalGizle: true,
            yil: function () { return parseInt($('#txtYil').val(), 10) || new Date().getFullYear(); }
        });

        function tutarSutun(alan, baslik, genislik) {
            return {
                field: alan, headerName: baslik, width: genislik || 105, editable: function (p) { return !p.node.rowPinned; },
                sortable: false, type: 'rightAligned', cellClass: function (p) { return p.node.rowPinned ? 'tk-toplam-hucre' : 'tk-girilir'; },
                valueFormatter: para0, valueParser: function (p) { return sayiOku(p.newValue); }
            };
        }
        var sabit = { sortable: false, cellClass: function (p) { return p.node.rowPinned ? 'tk-toplam-hucre' : 'tk-salt-hucre'; } };

        // Nakit: haftalık tutarlar (eski grdDetay1)
        var grid1 = Sgb.grid('#grdDetay1', {
            columnDefs: [
                $.extend({ field: 'HESAPKOD', headerName: 'Kod', width: 80 }, sabit),
                $.extend({ field: 'HESAPADI', headerName: 'Ödeme Kalemi', minWidth: 220, flex: 1, tooltipField: 'HESAPADI' }, sabit),
                tutarSutun('TUTAR11', '1.Hafta'),
                tutarSutun('TUTAR12', '2.Hafta'),
                tutarSutun('TUTAR13', '3.Hafta'),
                tutarSutun('TUTAR14', '4.Hafta'),
                $.extend({
                    colId: 'TOPLAM', headerName: 'Toplam', width: 115, type: 'rightAligned', valueFormatter: para0,
                    valueGetter: function (p) { var d = p.data || {}; return (Number(d.TUTAR11) || 0) + (Number(d.TUTAR12) || 0) + (Number(d.TUTAR13) || 0) + (Number(d.TUTAR14) || 0); }
                }, sabit)
            ],
            singleClickEdit: true,
            stopEditingWhenCellsLoseFocus: true,
            suppressCellFocus: false,
            onCellValueChanged: function () { toplamYaz(grid1, ['TUTAR11', 'TUTAR12', 'TUTAR13', 'TUTAR14']); },
            overlayNoRowsTemplate: '<span>Seçilen yıl, ay ve talep türü için tanımlı ödeme kalemi yok (Tanım Talep Formu).</span>'
        });

        // Ödenek aktarma: düşülecek / artırılacak (eski grdDetay2). Bir satırda yalnız biri girilir (eski HucreDegisti).
        var grid2 = Sgb.grid('#grdDetay2', {
            columnDefs: [
                $.extend({ field: 'HESAPKOD', headerName: 'Kod', width: 80 }, sabit),
                $.extend({ field: 'HESAPADI', headerName: 'Bütçe Kalemi', minWidth: 220, flex: 1, tooltipField: 'HESAPADI' }, sabit),
                tutarSutun('TUTAR21', 'Ödeneği Düşürülecek Tertip (-)', 150),
                tutarSutun('TUTAR22', 'Ödeneği Artırılacak Tertip (+)', 150)
            ],
            defaultColDef: { wrapHeaderText: true, autoHeaderHeight: true },
            singleClickEdit: true,
            stopEditingWhenCellsLoseFocus: true,
            suppressCellFocus: false,
            onCellValueChanged: function (e) {
                if (e.colDef.field === 'TUTAR21' && Number(e.data.TUTAR22) > 0) e.node.setDataValue('TUTAR22', 0);
                else if (e.colDef.field === 'TUTAR22' && Number(e.data.TUTAR21) > 0) e.node.setDataValue('TUTAR21', 0);
                toplamYaz(grid2, ['TUTAR21', 'TUTAR22']);
            },
            overlayNoRowsTemplate: '<span>Seçilen yıl, ay ve talep türü için tanımlı bütçe kalemi yok (Tanım Talep Formu).</span>'
        });

        // Eski TutarYaz: sütun toplamları (eski başlık altındaki toplam satırı) en altta sabit satırda.
        function toplamYaz(g, alanlar) {
            var t = { HESAPKOD: '', HESAPADI: 'Toplam' };
            $.each(alanlar, function (i, a) { t[a] = 0; });
            g.forEachNode(function (n) { $.each(alanlar, function (i, a) { t[a] += Number(n.data[a]) || 0; }); });
            g.setGridOption('pinnedBottomRowData', [t]);
        }

        function tur() { return parseInt($('#ddlTalepTur').val(), 10) || 0; }

        // Eski TalepTanimListele (istemci): türe göre tablo ve mevcut bakiye görünür.
        function gorunumAyarla() {
            var t = tur();
            $('#grpMevcutBakiye').prop('hidden', t !== 1);
            $('#bolSatir').prop('hidden', t !== 1 && t !== 2);
            $('#grdDetay1').prop('hidden', t !== 1);
            $('#grdDetay2').prop('hidden', t !== 2);
            $('#lblSatirBaslik').text(t === 2 ? 'Bütçe Kalemleri' : 'Ödeme Kalemleri');
            yay();
        }

        function satirlarYaz(satirlar) {
            var t = tur();
            grid1.setGridOption('rowData', t === 1 ? satirlar : []);
            grid2.setGridOption('rowData', t === 2 ? satirlar : []);
            toplamYaz(grid1, ['TUTAR11', 'TUTAR12', 'TUTAR13', 'TUTAR14']);
            toplamYaz(grid2, ['TUTAR21', 'TUTAR22']);
        }

        function satirSayisi(g) { var s = 0; g.forEachNode(function () { s++; }); return s; }

        // Eski TalepTanimListele_Click: tür / yıl / ay değişince tanım formundaki kalemler tutarsız gelir.
        // Kayıtlı talebin kendi tür / yıl / ayına dönülürse ve tabloda satır varsa dokunulmaz.
        function tanimSatirOku() {
            gorunumAyarla();
            var t = tur(), yil = parseInt($('#txtYil').val(), 10) || 0, ay = parseInt($('#ddlAy').val(), 10) || 0;
            if (!t && !yil && !ay) return;
            if (yuklu.tur === t && yuklu.yil === yil && yuklu.ay === ay) {
                if (t === 1 && satirSayisi(grid1)) return;
                if (t === 2 && satirSayisi(grid2)) return;
            }
            if (t !== 1 && t !== 2) { satirlarYaz([]); return; }
            Sgb.al(UC + 'form/tanim-satir', { tur: t, yil: yil, ay: ay })
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    satirlarYaz(v.satirlar || []);
                })
                .fail(hataGoster);
        }
        $('#ddlTalepTur, #ddlAy').on('change', tanimSatirOku);
        $('#txtYil').on('change', tanimSatirOku);

        function tusAyarla() {
            $('#btnSil, #btnYazdir').prop('disabled', !kod);
        }

        function formYaz(v) {
            kod = v.kod || '';
            muhBirim.ata({ muhasebe: v.muhasebe, birim: v.birim });
            if (v.muhasebe || v.birim) muhBirim.yenile();
            $('#ddlTalepTur').val(v.tur ? String(v.tur) : '');
            $('#txtYil').val(String(v.yil || new Date().getFullYear()));
            $('#ddlAy').val(String(v.ay || (new Date().getMonth() + 1)));
            $('#txtMevcutBakiye').val(v.mevcutBakiye ? tamSayi.format(v.mevcutBakiye) : '');
            $('#txtGerekce').val(v.gerekce || '');
            $('#lblDurum').text(v.durumAd || '').prop('hidden', !v.durumAd);
            yuklu = kod ? { tur: v.tur || 0, yil: v.yil || 0, ay: v.ay || 0 } : { tur: 0, yil: 0, ay: 0 };
            gorunumAyarla();
            satirlarYaz(v.satirlar || []);
            tusAyarla();
        }

        function oku() {
            return Sgb.al(UC + 'form/tanim', { kod: kod })
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    formYaz(v);
                })
                .fail(hataGoster);
        }

        $('#txtMevcutBakiye').on('change', function () {
            var s = $.trim($(this).val());
            $(this).val(s === '' ? '' : tamSayi.format(sayiOku(s)));
        });

        function satirlar(g) {
            var r = [];
            g.forEachNode(function (n) { if (n.data) r.push(n.data); });
            return r;
        }

        // Eski btnKaydet_Click
        $('#btnKaydet').on('click', function () {
            var t = tur();
            if (grid1.getEditingCells().length) grid1.stopEditing();
            if (grid2.getEditingCells().length) grid2.stopEditing();
            // Eski kontrol: düşürülecek toplam artırılacak toplama eşit olmalı (sunucu da denetler).
            if (t === 2) {
                var dus = 0, art = 0;
                $.each(satirlar(grid2), function (i, s) { dus += Number(s.TUTAR21) || 0; art += Number(s.TUTAR22) || 0; });
                if (dus !== art) { mesaj('Düşürülecek tertip toplam tutarı ile artırılacak tertip toplam tutarı eşit olmak zorundadır.', 'warning'); return; }
            }
            if (!window.confirm('Talep bilgisi kayıt edilecek. Onaylıyor musunuz?')) return;
            var mb = muhBirim.deger();
            var $d = $(this).prop('disabled', true);
            Sgb.gonder(UC + 'form/kaydet', {
                kod: kod,
                muhasebe: mb.muhasebe ? String(mb.muhasebe) : '',
                birim: mb.birim || '',
                tur: t,
                yil: parseInt($('#txtYil').val(), 10) || 0,
                ay: parseInt($('#ddlAy').val(), 10) || 0,
                mevcutBakiye: sayiOku($('#txtMevcutBakiye').val()),
                gerekce: $('#txtGerekce').val(),
                satirlar: t === 1 ? satirlar(grid1) : t === 2 ? satirlar(grid2) : []
            })
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    mesaj(v.bilgi || 'Kaydedildi.', 'success');
                    if (v.kod) kod = v.kod;
                    listeYenile();
                    oku();
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });

        // Eski btnSil_Click: silinince liste yenilenir, pencere kapanır.
        $('#btnSil').on('click', function () {
            if (!kod) return;
            if (!window.confirm('Talep kaydı silinecek. Onaylıyor musunuz?')) return;
            var mb = muhBirim.deger();
            Sgb.gonder(UC + 'form/sil', { kod: kod, muhasebe: mb.muhasebe ? String(mb.muhasebe) : '', birim: mb.birim || '' })
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    mesaj(v.bilgi || 'Silindi.', 'success');
                    listeYenile();
                    if (cerceve()) setTimeout(pencereKapat, 600);
                    else formYaz({ kod: '', muhasebe: mb.muhasebe, birim: mb.birim, satirlar: [] });
                })
                .fail(hataGoster);
        });

        // Eski btnYazdir_Click
        $('#btnYazdir').on('click', function () {
            if (!kod) return;
            var $d = $(this).prop('disabled', true);
            Sgb.indir(UC + 'form/yazdir', { kod: kod }).then(function () { $d.prop('disabled', false); });
        });

        oku();
        yay();
    }
});
