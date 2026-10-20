// Fatura Hazırlama (ButceMuhasebe/FaturaListesi), Fatura Giriş (FaturaGiris) ve Toplu Tahakkuk (FaturaTahakkukIslemleri); eski aynı adlı
// Ext.NET sayfaları. Fatura Giriş ve Toplu Tahakkuk menüde yoktur, Fatura Hazırlama'dan pencere (Sgb.pencere) olarak açılır.
//   Liste : kriterlere göre faturalar, Yeni Fatura / Aç, Listeyi Yazdır (Excel), Toplu Tahakkuk Oluştur.
//   Form  : fatura bilgileri ve satırları, Kaydet / İptal / Yazdır (Word), Tahakkuk / Ödeme MİF oluştur, açıklama tanımları, hesaplama.
//   Toplu : seçilen faturalardan tek Tahakkuk MİF'i ya da Ödeme Emri.
// Müdürlük seçimi (eski ListeMudurluk.aspx) ve belge oluşturma penceresi (eski wndTahakkuk) sayfalar arasında ortaktır.
// Uçlar: /api/fatura/* (Muhasebe/FaturaUclari.cs). Birim seçimi Sgb.muhasebeBirim, hesap kodu Sgb.hesapKodu / Sgb.hesapPlaniSec.
$(function () {
    'use strict';

    if (!$('.fatura').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/fatura/';
    var para2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var para8 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 8 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function bos(s) { return s === undefined || s === null || $.trim(String(s)) === ''; }

    // "1.234,56" ya da "1234.56" -> sayı; boşsa null.
    function sayiCoz(s) {
        if (typeof s === 'number') return isNaN(s) ? null : s;
        s = $.trim(String(s == null ? '' : s));
        if (!s) return null;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = Number(s);
        return isNaN(n) ? null : n;
    }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });
    $(document).on('click', 'dialog [data-kapat]', function (e) { e.stopPropagation(); this.closest('dialog').close(); });

    // Pencere olarak açıldıysa kapat (eski parent.hidePopWin).
    function cerceve() {
        try { return window.frameElement ? window.frameElement.closest('dialog') : null; } catch (e) { return null; }
    }
    function pencereKapat() { var d = cerceve(); if (d) d.close(); }

    // ---------------------------------------------------------------- onay sorusu

    var soruIsi = null;
    function sor(metin, is) {
        var d = document.getElementById('dlgSoru');
        if (!d) { if (window.confirm(metin)) is(); return; }
        $('#soruMetin').text(metin);
        soruIsi = is;
        d.showModal();
    }
    $('#btnSoruEvet').on('click', function () {
        document.getElementById('dlgSoru').close();
        var is = soruIsi;
        soruIsi = null;
        if (is) is();
    });

    // Eski BelgeyiGoster / OdemeEmrimifFrame bağlantısı: ödeme emri belgesi pencerede. Mümkünse en üstteki sayfanın penceresi kullanılır.
    function odemeEmriAc(yil, belgeNo) {
        if (bos(belgeNo)) return;
        var ust = Sgb;
        try {
            if (window.top !== window && window.top.Sgb && window.top.Sgb.pencere) ust = window.top.Sgb;
        } catch (e) { /* başka kökenden açılmış */ }
        ust.pencere('../ButceMuhasebe/OdemeEmriMIFFrame?' + $.param({ aramaYok: 1, yil: yil, belgeNo: belgeNo }), 'Ödeme Emri ' + yil + '/' + belgeNo);
    }

    function yay() {
        $.each(['grdListe', 'grdSatirlar', 'grdToplu'], function (i, ad) {
            var el = document.getElementById(ad);
            if (!el || el.offsetParent === null) return;
            var ust = el.getBoundingClientRect().top;
            el.style.height = Math.max(ad === 'grdSatirlar' ? 200 : 260, Math.floor(window.innerHeight - ust - 24)) + 'px';
        });
    }
    $(window).on('resize', yay);

    // ================================================================ müdürlük seçimi (eski ListeMudurluk.aspx)

    var mudurlukHedef = null;
    var gridMudurluk = null;
    var mudurlukOkundu = false;

    function mudurlukSec($hedef) {
        var d = document.getElementById('dlgMudurluk');
        if (!d) return;
        mudurlukHedef = $hedef;
        if (!gridMudurluk) {
            gridMudurluk = Sgb.grid('#grdMudurluk', {
                columnDefs: [
                    { field: 'kod', headerName: 'Kod', width: 110 },
                    { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 200 }
                ],
                rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
                overlayNoRowsTemplate: '<span>Müdürlük bulunamadı.</span>',
                isExternalFilterPresent: function () { return !bos($('#txtMudurlukAra').val()); },
                doesExternalFilterPass: function (n) {
                    var a = $.trim($('#txtMudurlukAra').val()).toLocaleLowerCase('tr-TR');
                    return ((n.data.kod || '') + ' ' + (n.data.ad || '')).toLocaleLowerCase('tr-TR').indexOf(a) >= 0;
                },
                // Eski MudurlukBilgisiYaz: seçilen kod alana yazılır.
                onRowClicked: function (e) {
                    if (mudurlukHedef) mudurlukHedef.val(e.data.kod).trigger('change');
                    d.close();
                }
            });
            $('#txtMudurlukAra').on('input', function () { gridMudurluk.onFilterChanged(); });
        }
        $('#txtMudurlukAra').val('');
        gridMudurluk.onFilterChanged();
        d.showModal();
        if (mudurlukOkundu) return;
        gridMudurluk.setGridOption('loading', true);
        Sgb.al(UC + 'mudurluk')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                mudurlukOkundu = true;
                gridMudurluk.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { gridMudurluk.setGridOption('loading', false); });
    }
    $('[data-mudurluk]').on('click', function () { mudurlukSec($($(this).data('mudurluk'))); });

    // ================================================================ belge oluşturma penceresi (eski wndTahakkuk)

    // ayar: { tur: 1 tahakkuk | 2 ödeme, form: Fatura Giriş mi, tarih, tahakkukBelgeNo, odemeBelgeNo, muhBelgeAciklama,
    //         tahakkukHesapKod, odemeHesapKod, tahakkukAyniFis, hesapYil, belgeYil (belge aç için), gonder(veri) -> söz }
    var belge = { ayar: null, tahakkukHesap: null, odemeHesap: null };

    function damgaGoster() {
        // Eski SadeceDamgaVergisiGoster
        var sadece = $('#chkDamgaVergisi').prop('checked');
        if (sadece) {
            $('input[name=optDamga][value=2]').prop('checked', true);
            $('input[name=optDamga][value=1], input[name=optDamga][value=3]').prop('disabled', true);
            $('#lblChkTahakkuk').prop('hidden', true);
            $('#chkTahakkuk').prop('checked', false);
        } else {
            $('input[name=optDamga]').prop('disabled', false);
            $('#lblChkTahakkuk').prop('hidden', false);
        }
        $('#grpTahakkukHesap').prop('hidden', sadece);
        $('#grpOdemeHesap').prop('hidden', sadece || belge.ayar.tur !== 2);
    }

    function belgePenceresi(ayar) {
        var d = document.getElementById('dlgBelge');
        belge.ayar = ayar;
        var tur = ayar.tur;

        if (!belge.tahakkukHesap) {
            var hesapYil = function () { return belge.ayar ? belge.ayar.hesapYil : 0; };
            belge.tahakkukHesap = Sgb.hesapKodu('#txtTahakkukHesapKod', { yil: hesapYil, sonSeviye: true, ad: true, uc: UC + 'hesap/' });
            belge.odemeHesap = Sgb.hesapKodu('#txtOdemeHesapKod', { yil: hesapYil, sonSeviye: true, ad: true, uc: UC + 'hesap/' });
            $('#chkDamgaVergisi').on('change', function () {
                // Eski: işaret kalkınca "Tahakkuku aynı fişte üret" yeniden işaretlenir.
                if (!this.checked) $('#chkTahakkuk').prop('checked', true);
                damgaGoster();
            });
            $('[data-belge-ac]').on('click', function () { odemeEmriAc(belge.ayar.belgeYil, $.trim($($(this).data('belge-ac')).val())); });
            $('#btnBelgeOlustur').on('click', belgeOlustur);
        }

        $('#dlgBelgeBaslik').text(tur === 1 ? 'Tahakkuk Belge Oluşturma' : 'Ödeme Belge Oluşturma');
        $('#dlgBelge [data-belge]').each(function () { $(this).prop('hidden', String($(this).data('belge')) !== String(tur)); });
        $('#dlgBelge [data-yalniz-form]').prop('hidden', !ayar.form);

        $('#txtBelgeTarih').val(ayar.tarih || Sgb.bugun());
        $('#txtTahakkukBelgeNo').val(ayar.tahakkukBelgeNo || '');
        $('#txtOdemeBelgeNo').val(ayar.odemeBelgeNo || '');
        $('#txtMuhBelgeAciklama').val(ayar.muhBelgeAciklama || '');
        // Kayıtlı hesap kodları adlarıyla okunur.
        belge.tahakkukHesap.ata(ayar.tahakkukHesapKod || '');
        belge.odemeHesap.ata(ayar.odemeHesapKod || '');
        $('input[name=optDamga][value=1]').prop('checked', true);
        $('input[name=optTahakkukTur][value=1]').prop('checked', true);
        $('#chkTahakkuk').prop('checked', ayar.tahakkukAyniFis !== false);
        $('#chkDamgaVergisi').prop('checked', false);
        damgaGoster();
        d.showModal();
    }

    function belgeOlustur() {
        var a = belge.ayar;
        var veri = {
            BelgeTur: a.tur,
            Tarih: $('#txtBelgeTarih').val() || '',
            TahakkukBelgeNo: $.trim($('#txtTahakkukBelgeNo').val()),
            OdemeBelgeNo: $.trim($('#txtOdemeBelgeNo').val()),
            MuhBelgeAciklama: a.form ? $('#txtMuhBelgeAciklama').val() : '',
            TahakkukHesapKod: belge.tahakkukHesap.deger().kod || $.trim($('#txtTahakkukHesapKod').val()),
            OdemeHesapKod: belge.odemeHesap.deger().kod || $.trim($('#txtOdemeHesapKod').val()),
            Damga: parseInt($('input[name=optDamga]:checked').val(), 10) || 1,
            TahakkukTahsilat: $('input[name=optTahakkukTur]:checked').val() === '1',
            TahakkukAyniFis: $('#chkTahakkuk').prop('checked'),
            SadeceDamga: $('#chkDamgaVergisi').prop('checked')
        };
        var $d = $('#btnBelgeOlustur').prop('disabled', true);
        $('body').addClass('avans-bekle');
        a.gonder(veri)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata || 'Belge oluşturulamadı.', 'danger'); return; }
                document.getElementById('dlgBelge').close();
                belgeSonucGoster(s);
                if (a.sonra) a.sonra(s);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); $('body').removeClass('avans-bekle'); });
    }

    // Eski MesajKutusu: "Ödeme Emri/MİF kaydı yapıldı. Belge Numarası :" + yil/belgeNo bağlantısı.
    function belgeSonucGoster(s) {
        $('#lblBelgeSonuc').text(s.bilgi || '');
        $('#lnkBelgeSonuc').text(s.yil + '/' + s.belgeNo).off('click').on('click', function (e) {
            e.preventDefault();
            odemeEmriAc(s.yil, s.belgeNo);
        });
        document.getElementById('dlgBelgeSonuc').showModal();
    }

    // ================================================================ sayfalar

    if ($('.fatura-liste').length) listeSayfasi();
    else if ($('.fatura-form').length) formSayfasi();
    else if ($('.fatura-toplu').length) topluSayfasi();

    // ================================================================ Fatura Hazırlama (eski FaturaListesi.aspx)

    function listeSayfasi() {
        var listelendi = false;
        var tanim = { yil: new Date().getFullYear(), durum: 1 };

        var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
            kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal' },
            yil: function () { return parseInt($('#txtYil').val(), 10) || 0; },
            kurumsalGizle: true
        });

        function kriter() {
            var mb = muhBirim.deger();
            return {
                yil: parseInt($('#txtYil').val(), 10) || 0,
                muhasebe: parseInt(mb.muhasebe, 10) || 0,
                birim: mb.birim || '',
                mudurluk: $.trim($('#txtMudurluk').val()),
                faturaNo: $.trim($('#txtFaturaNo').val()),
                musteri: $.trim($('#txtMusteri').val()),
                durum: parseInt($('#ddlDurum').val(), 10) || 0,
                islemYapan: $.trim($('#txtIslemYapan').val()),
                tarih1: $('#txtTarih1').val() || '',
                tarih2: $('#txtTarih2').val() || ''
            };
        }

        function acHucre(p) {
            if (!p.data) return '';
            var b = document.createElement('button');
            b.type = 'button';
            b.className = 'btn btn-link btn-xs ft-ac';
            b.title = 'Faturayı aç';
            b.innerHTML = '<span class="glyphicon glyphicon-edit" aria-hidden="true"></span><span class="sr-only">Aç</span>';
            b.addEventListener('click', function (e) { e.stopPropagation(); faturaAc(p.data.KOD); });
            return b;
        }

        var tarih = function (p) { return p.value ? Sgb.tarih(p.value) : ''; };

        var grid = Sgb.grid('#grdListe', {
            columnDefs: [
                { headerName: '#', valueGetter: 'node.rowIndex + 1', width: 52, sortable: false },
                { headerName: 'Aç', width: 50, sortable: false, resizable: false, cellRenderer: acHucre },
                { field: 'FATURATARIHI', headerName: 'Fatura Tarih', width: 100, valueFormatter: tarih },
                { field: 'FATURANO', headerName: 'Fatura No', width: 100 },
                { field: 'MUHASEBEKOD', headerName: 'Muhasebe', width: 85, hide: true },
                { field: 'BIRIMKOD', headerName: 'Birim', width: 70 },
                { field: 'MUDURLUKKOD', headerName: 'Müdürlük', width: 85 },
                { field: 'MUSTERIADI', headerName: 'Müşteri', flex: 1, minWidth: 200, tooltipField: 'MUSTERIADI' },
                { field: 'GENELTOPLAMPARABIRIMI', headerName: 'Toplam Tutar', width: 130, type: 'rightAligned', comparator: function (a, b, na, nb) { return (na.data.GENELTOPLAM || 0) - (nb.data.GENELTOPLAM || 0); } },
                { field: 'TAHAKKUKBELGENO', headerName: 'Tahakkuk Belge No', width: 130 },
                { field: 'ODEMEBELGENO', headerName: 'Ödeme Belge No', width: 130 },
                { field: 'ODEMETARIHI', headerName: 'Ödeme Tarih', width: 100, valueFormatter: tarih },
                { field: 'DURUM', headerName: 'Durum', width: 90 },
                { field: 'ISLEMYAPAN', headerName: 'İşlem Yapan', width: 120 },
                { field: 'ISLEMTARIHI', headerName: 'İşlem Tarih', width: 100, valueFormatter: tarih }
            ],
            getRowId: function (p) { return String(p.data.KOD); },
            getRowClass: function (p) { return p.data && p.data.DURUMKOD === 9 ? 'ft-iptal' : ''; },
            overlayNoRowsTemplate: '<span>Kriterleri girip Listele\'ye basınız.</span>',
            onRowDoubleClicked: function (e) { faturaAc(e.data.KOD); }
        });

        function listele(sessiz) {
            var $d = $('#btnListele').prop('disabled', true);
            grid.setGridOption('loading', true);
            Sgb.al(UC + 'liste', kriter())
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    listelendi = true;
                    var satirlar = v.satirlar || [];
                    grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun fatura bulunamadı.</span>');
                    grid.setGridOption('rowData', satirlar);
                    $('#lblSayi').text(satirlar.length + ' fatura');
                    if (!satirlar.length && !sessiz) mesaj('Verilen kriterlere uygun fatura bulunamadı.', 'info');
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
        }

        $('#btnListele').on('click', function () { listele(false); });
        $('#frmKriter').on('keydown', 'input', function (e) {
            if (e.key === 'Enter' && !$(this).closest('#muhBirim').length) { e.preventDefault(); listele(false); }
        });

        // Eski btnSorguTemizle_Click: yıl bu yıl, durum Yeni; muhasebe korunur, birim ve müdürlük boşalır.
        $('#btnTemizle').on('click', function () {
            $('#txtFaturaNo, #txtMusteri, #txtTarih1, #txtTarih2, #txtMudurluk').val('');
            $('#txtYil').val(String(new Date().getFullYear()));
            $('#ddlDurum').val(String(tanim.durum));
            var mb = muhBirim.deger();
            muhBirim.ata({ muhasebe: mb.muhasebe, birim: '', kurumsal: '', birimAd: '' });
        });

        // Eski FaturaAc: fatura formu pencerede; kapanınca liste yenilenir.
        function faturaAc(kod) {
            Sgb.pencere('FaturaGiris?' + $.param({ kod: kod || 0 }), kod ? 'Fatura' : 'Yeni Fatura', function () {
                if (listelendi) listele(true);
            });
        }
        $('#btnYeni').on('click', function () { faturaAc(0); });

        $('#btnYazdir').on('click', function () {
            var $t = $(this).prop('disabled', true);
            Sgb.indir(UC + 'liste/yazdir', kriter()).then(function () { $t.prop('disabled', false); });
        });

        // Eski TopluTahakkukAc: listedeki kriterlerle toplu tahakkuk penceresi.
        $('#btnToplu').on('click', function () {
            var k = kriter();
            Sgb.pencere('FaturaTahakkukIslemleri?' + $.param({
                faturaNo: k.faturaNo, tarih1: k.tarih1, tarih2: k.tarih2, musteri: k.musteri, mudurluk: k.mudurluk,
                muhasebe: k.muhasebe, birim: k.birim, yil: k.yil, durum: k.durum
            }), 'Toplu Tahakkuk', function () {
                if (listelendi) listele(true);
            });
        });

        // Açılış (eski Page_Load): yıl, durum Yeni, kullanıcının muhasebe / birim / müdürlüğü. Liste boş başlar.
        Sgb.al(UC + 'tanim')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                tanim = v;
                Sgb.yilKutusu('#txtYil', [v.yil - 15, v.yil + 1], v.yil);
                var $du = $('#ddlDurum').empty();
                $.each(v.durumlar || [], function (i, t) { $('<option></option>').val(t.kod).text(t.ad).appendTo($du); });
                $du.val(String(v.durum));
                if (v.muhasebe) muhBirim.ata({ muhasebe: v.muhasebe, birim: v.birim, kurumsal: v.kurumsal, birimAd: v.birimAd });
                $('#txtMudurluk').val(v.mudurluk || '');
                yay();
            })
            .fail(hataGoster);
        yay();
    }

    // ================================================================ Fatura Giriş (eski FaturaGiris.aspx)

    function formSayfasi() {
        var q = new URLSearchParams(window.location.search);
        var kod = parseInt(q.get('kod'), 10) || 0;
        var tanim = {};
        var fatura = null;
        var tutarGirisi = false;
        var sayac = 0;

        if (cerceve()) $('#btnKapat').prop('hidden', false).on('click', pencereKapat);

        var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
            kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal' },
            kurumsalGizle: true
        });

        // ------------------------------------------------------------ satırlar (eski grdTanim)

        function bosSatir() {
            return { _id: ++sayac, FATURAKOD: kod, ACIKLAMA: '', HESAPKOD: '', MIKTAR: null, KDVORAN: null, BIRIMFIYAT: null, TUTAR: null };
        }

        function komut(ad, simge, baslik, is) {
            return function (p) {
                if (!p.data) return '';
                var b = document.createElement('button');
                b.type = 'button';
                b.className = 'btn btn-link btn-xs ft-komut';
                b.title = baslik;
                b.innerHTML = '<span class="glyphicon ' + simge + '" aria-hidden="true"></span><span class="sr-only">' + baslik + '</span>';
                b.addEventListener('click', function (e) { e.stopPropagation(); grid.stopEditing(); is(p.node); });
                return b;
            };
        }

        function hesapHucre(p) {
            var $h = $('<span class="ft-hucre-sec"></span>');
            $('<span class="ft-hucre-metin"></span>').text(p.value || '').appendTo($h);
            $('<button type="button" class="btn btn-link btn-xs ft-komut" title="Hesap planından seç"><span class="glyphicon glyphicon-search" aria-hidden="true"></span><span class="sr-only">Hesap planından seç</span></button>')
                .on('click', function (e) {
                    e.stopPropagation();
                    grid.stopEditing();
                    Sgb.hesapPlaniSec({
                        uc: UC + 'hesap/', yil: tanim.yil, sonKirilim: true,
                        secilince: function (h) { p.node.setDataValue('HESAPKOD', h.kod); }
                    });
                })
                .appendTo($h);
            return $h[0];
        }

        function sayi(p) { var n = sayiCoz(p.newValue); return n; }
        function tamSayi(p) { var n = sayiCoz(p.newValue); return n === null ? null : Math.trunc(n); }

        // Eski Hesapla / HesaplaBirimFiyat: hesaplanan değer kırmızı gösterilir (kaydederken sunucu aynı formülle hesaplar).
        function hesaplananTutar(d) {
            return (Number(d.MIKTAR) || 0) * ((Number(d.BIRIMFIYAT) || 0) * (1.0 + ((Number(d.KDVORAN) || 0) / 100)));
        }
        function hesaplananBirimFiyat(d) {
            var m = Number(d.MIKTAR) || 0;
            return m > 0 ? ((Number(d.TUTAR) || 0) / (1.0 + ((Number(d.KDVORAN) || 0) / 100))) / m : 0;
        }

        var grid;
        function gridKur() {
            grid = Sgb.grid('#grdSatirlar', {
                columnDefs: [
                    { headerName: '', width: 40, sortable: false, resizable: false, cellRenderer: komut('sil', 'glyphicon-minus-sign', 'Satırı sil', function (n) { grid.applyTransaction({ remove: [n.data] }); }) },
                    {
                        field: 'ACIKLAMA', headerName: 'Açıklama', flex: 1, minWidth: 220, editable: true, sortable: false,
                        cellClass: ['ft-girilir', 'ft-aciklama-hucre'], cellEditor: 'agLargeTextCellEditor', cellEditorPopup: true,
                        cellEditorParams: { maxLength: 4000, rows: 5, cols: 60 }, wrapText: true, autoHeight: true, tooltipField: 'ACIKLAMA'
                    },
                    { headerName: '', width: 40, sortable: false, resizable: false, cellRenderer: komut('aciklama', 'glyphicon-list', 'Satır açıklaması seç', function (n) { aciklamaPenceresi(2, n); }) },
                    {
                        field: 'HESAPKOD', headerName: 'Gelir Hesap Kodu', width: 170, editable: true, sortable: false,
                        cellClass: 'ft-girilir', cellRenderer: hesapHucre, cellEditor: 'agTextCellEditor'
                    },
                    {
                        field: 'MIKTAR', headerName: 'Miktar', width: 90, editable: true, sortable: false, type: 'rightAligned',
                        cellClass: ['ft-girilir', 'ag-right-aligned-cell'], valueParser: sayi,
                        valueFormatter: function (p) { return p.value == null ? '' : para8.format(p.value); }
                    },
                    { headerName: '', width: 40, sortable: false, resizable: false, cellRenderer: komut('miktar', 'glyphicon-th', 'Miktar hesapla', function (n) { hesaplaPenceresi('MiktarHesapla', n); }) },
                    {
                        field: 'KDVORAN', headerName: 'KDV Oranı', width: 80, editable: true, sortable: false, type: 'rightAligned',
                        cellClass: ['ft-girilir', 'ag-right-aligned-cell'], valueParser: tamSayi
                    },
                    {
                        field: 'BIRIMFIYAT', headerName: 'Birim Fiyat', width: 130, editable: !tutarGirisi, sortable: false, type: 'rightAligned',
                        cellClass: tutarGirisi ? ['ft-salt-hucre', 'ft-hesaplanan', 'ag-right-aligned-cell'] : ['ft-girilir', 'ag-right-aligned-cell'],
                        valueParser: sayi,
                        valueFormatter: function (p) {
                            if (!p.data) return '';
                            var v = tutarGirisi ? hesaplananBirimFiyat(p.data) : p.value;
                            return v == null ? '' : para8.format(v);
                        }
                    },
                    {
                        field: 'TUTAR', headerName: 'Satır Tutarı', width: 120, editable: tutarGirisi, sortable: false, type: 'rightAligned',
                        cellClass: tutarGirisi ? ['ft-girilir', 'ag-right-aligned-cell'] : ['ft-salt-hucre', 'ft-hesaplanan', 'ag-right-aligned-cell'],
                        valueParser: sayi,
                        valueFormatter: function (p) {
                            if (!p.data) return '';
                            var v = tutarGirisi ? p.value : hesaplananTutar(p.data);
                            return v == null ? '' : para2.format(v);
                        }
                    },
                    { headerName: '', width: 40, sortable: false, resizable: false, cellRenderer: komut('tutar', 'glyphicon-th', 'Tutar hesapla', function (n) { hesaplaPenceresi('TutarHesapla', n); }) }
                ],
                getRowId: function (p) { return String(p.data._id); },
                singleClickEdit: true,
                stopEditingWhenCellsLoseFocus: true,
                overlayNoRowsTemplate: '<span>Satır eklemek için Ekle\'ye basınız.</span>',
                // Hesaplanan sütunlar diğer alanlar değişince yeniden çizilir.
                onCellValueChanged: function (e) { grid.refreshCells({ rowNodes: [e.node], force: true }); }
            });
        }

        function satirlar() {
            var r = [];
            if (grid) grid.forEachNode(function (n) { r.push(n.data); });
            return r;
        }

        $('#btnSatirEkle').on('click', function () {
            var t = grid.applyTransaction({ add: [bosSatir()] });
            if (t && t.add && t.add.length) grid.ensureNodeVisible(t.add[0]);
        });

        // ------------------------------------------------------------ hesaplama (eski wndHesapla)

        var hesapla = { tur: '', node: null };
        function hesaplaPenceresi(tur, node) {
            hesapla.tur = tur;
            hesapla.node = node;
            var t = node.data.TUTAR;
            $('#txtHesaplamaTutar').val(t == null ? '' : String(t).replace('.', ','));
            $('#grpHesaplamaYuzde').prop('hidden', tur !== 'TutarHesapla');
            $('#grpHesaplamaBirimFiyat').prop('hidden', tur !== 'MiktarHesapla');
            document.getElementById('dlgHesapla').showModal();
        }
        // Eski HesaplamaPenceresiHesapla
        $('#btnHesapla').on('click', function () {
            var tutar = sayiCoz($('#txtHesaplamaTutar').val()) || 0;
            var yuzde = sayiCoz($('#txtHesaplamaYuzde').val()) || 0;
            var birimFiyat = sayiCoz($('#txtHesaplamaBirimFiyat').val()) || 0;
            if (hesapla.tur === 'TutarHesapla') hesapla.node.setDataValue('TUTAR', tutar * yuzde / 100);
            if (hesapla.tur === 'MiktarHesapla') {
                if (!birimFiyat) { mesaj('Birim fiyatı giriniz.', 'warning'); return; }
                hesapla.node.setDataValue('MIKTAR', tutar / birimFiyat);
            }
            document.getElementById('dlgHesapla').close();
        });
        $('#frmHesapla').on('keydown', 'input', function (e) { if (e.key === 'Enter') { e.preventDefault(); $('#btnHesapla').trigger('click'); } });

        // ------------------------------------------------------------ açıklama tanımları (eski wndFaturaAciklama)

        var aciklama = { tur: 1, node: null, kod: 0, grid: null };

        function aciklamaTemizle() {
            aciklama.kod = 0;
            $('#txtEditor').val('');
            $('#btnAciklamaSil').prop('disabled', true);
            if (aciklama.grid) aciklama.grid.deselectAll();
        }

        function aciklamaListele() {
            aciklama.grid.setGridOption('loading', true);
            return Sgb.al(UC + 'aciklama/liste', { tur: aciklama.tur })
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    aciklama.grid.setGridOption('rowData', v.satirlar || []);
                })
                .fail(hataGoster)
                .always(function () { aciklama.grid.setGridOption('loading', false); });
        }

        // Eski EkleAciklamaIslem: tür 1 fatura açıklamasına yeni satır olarak eklenir, tür 2 satırın açıklaması olur, tür 3 müşteri ek bilgisi.
        function aciklamaEkle(metin) {
            if (aciklama.tur === 1) {
                var eski = $('#txtAciklama').val();
                if (eski !== '') eski += '\n';
                $('#txtAciklama').val(eski + metin);
            } else if (aciklama.tur === 2 && aciklama.node) {
                aciklama.node.setDataValue('ACIKLAMA', metin);
            } else if (aciklama.tur === 3) {
                $('#txtMusteriAdiEk').val(metin);
            }
            document.getElementById('dlgAciklama').close();
        }

        function aciklamaPenceresi(tur, node) {
            aciklama.tur = tur;
            aciklama.node = node || null;
            if (!aciklama.grid) {
                aciklama.grid = Sgb.grid('#grdAciklama', {
                    columnDefs: [
                        {
                            headerName: '', width: 44, sortable: false, resizable: false,
                            cellRenderer: function (p) {
                                if (!p.data) return '';
                                var b = document.createElement('button');
                                b.type = 'button';
                                b.className = 'btn btn-link btn-xs ft-komut';
                                b.title = 'Ekle';
                                b.innerHTML = '<span class="glyphicon glyphicon-plus" aria-hidden="true"></span><span class="sr-only">Ekle</span>';
                                b.addEventListener('click', function (e) { e.stopPropagation(); aciklamaEkle(p.data.ACIKLAMA || ''); });
                                return b;
                            }
                        },
                        { field: 'ACIKLAMA', headerName: 'Açıklama', flex: 1, minWidth: 200, wrapText: true, autoHeight: true, cellClass: 'ft-aciklama-hucre' }
                    ],
                    rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
                    overlayNoRowsTemplate: '<span>Kayıtlı açıklama yok.</span>',
                    // Eski grdAciklamaTanim_CellClick: seçilen açıklama düzenlenmek üzere okunur.
                    onRowClicked: function (e) {
                        aciklama.kod = e.data.KOD;
                        $('#txtEditor').val(e.data.ACIKLAMA || '');
                        $('#btnAciklamaSil').prop('disabled', false);
                    }
                });
            }
            aciklamaTemizle();
            aciklama.grid.setGridOption('rowData', []);
            document.getElementById('dlgAciklama').showModal();
            aciklamaListele();
        }

        $('[data-aciklama]').on('click', function () { aciklamaPenceresi(parseInt($(this).data('aciklama'), 10)); });
        $('#btnAciklamaListele').on('click', aciklamaListele);
        $('#btnAciklamaYeni').on('click', aciklamaTemizle);
        $('#btnAciklamaKaydet').on('click', function () {
            sor('Bilgiler kayıt edilecek. Onaylıyor musunuz?', function () {
                Sgb.gonder(UC + 'aciklama/kaydet', { Kod: aciklama.kod, Tur: aciklama.tur, Aciklama: $('#txtEditor').val() })
                    .done(function (s) {
                        if (!s.basarili) { mesaj(s.hata || 'Kayıt yapılamadı.', 'danger'); return; }
                        mesaj(s.bilgi, 'success');
                        aciklamaTemizle();
                        aciklamaListele();
                    })
                    .fail(hataGoster);
            });
        });
        $('#btnAciklamaSil').on('click', function () {
            if (!aciklama.kod) return;
            sor('Bilgiler silinecek. Onaylıyor musunuz?', function () {
                Sgb.gonder(UC + 'aciklama/sil', { Kod: aciklama.kod, Tur: aciklama.tur })
                    .done(function (s) {
                        if (!s.basarili) { mesaj(s.hata || 'Silme yapılamadı.', 'danger'); return; }
                        mesaj(s.bilgi, 'success');
                        aciklamaTemizle();
                        aciklamaListele();
                    })
                    .fail(hataGoster);
            });
        });

        // ------------------------------------------------------------ müşteri (eski FirmaBilgisiAc / FirmaBilgisiGoster)

        window.FirmaBilgisiGoster = function (firma) {
            $('#txtVNo').val(firma.vergiNo || '');
            $('#txtUnvan').val(firma.ad || '');
        };
        $('#btnFirmaSec').on('click', function () {
            var muhasebe = muhBirim.deger().muhasebe;
            if (bos(muhasebe)) { mesaj('Önce muhasebe birimini seçiniz.', 'warning'); return; }
            Sgb.pencere('../HarcamaSurecleri/ListeFirmaKisi?' + $.param({ per: 1, muhasebe: muhasebe }), 'Kişi / Firma Listesi');
        });

        // ------------------------------------------------------------ fatura bilgileri

        function belgeBaglanti($etiket, $lnk, yil, belgeNo) {
            $etiket.prop('hidden', bos(belgeNo));
            $lnk.text(bos(belgeNo) ? '' : yil + '/' + belgeNo).off('click').on('click', function (e) { e.preventDefault(); odemeEmriAc(yil, belgeNo); });
        }

        function formYaz() {
            var f = fatura;
            var yeni = !f;
            if (yeni) {
                // Eski Temizle: muhasebe / birim / müdürlük kullanıcının değişkenlerinden, fatura tarihi bugün.
                if (tanim.muhasebe) muhBirim.ata({ muhasebe: tanim.muhasebe, birim: tanim.birim, kurumsal: tanim.kurumsal, birimAd: tanim.birimAd });
                $('#txtMudurluk').val(tanim.mudurluk || '');
                $('#txtVNo, #txtUnvan, #txtMusteriAdiEk, #txtFaturaNo, #txtIrsaliyeNo, #txtIrsaliyeTarihi, #txtAciklama').val('');
                $('#txtFaturaTarihi').val(tanim.bugun);
                $('#ddlTevkifat').val('0');
                $('#lblToplam, #lblKdv, #lblGenelToplam').text('');
                belgeBaglanti($('#lblTahakkukBelge'), $('#lnkTahakkukBelge'), 0, '');
                belgeBaglanti($('#lblOdemeBelge'), $('#lnkOdemeBelge'), 0, '');
                $('#lblDurum').prop('hidden', true);
                grid.setGridOption('rowData', [bosSatir()]);
            } else {
                muhBirim.ata({ muhasebe: f.muhasebe, birim: f.birim, kurumsal: '', birimAd: '' });
                muhBirim.yenile();
                $('#txtMudurluk').val(f.mudurluk || '');
                $('#txtVNo').val(f.musteriKod || '');
                $('#txtUnvan').val(f.musteriAdi || '');
                $('#txtMusteriAdiEk').val(f.musteriAdiEk || '');
                $('#txtFaturaNo').val(f.faturaNo || '');
                $('#txtFaturaTarihi').val(f.faturaTarihi || '');
                $('#txtIrsaliyeNo').val(f.irsaliyeNo || '');
                $('#txtIrsaliyeTarihi').val(f.irsaliyeTarihi || '');
                $('#txtAciklama').val(f.aciklama || '');
                $('#ddlTevkifat').val(String(f.tevkifat || 0));
                $('#lblToplam').text(f.toplam || '');
                $('#lblKdv').text(f.kdv || '');
                $('#lblGenelToplam').text(f.genelToplam || '');
                belgeBaglanti($('#lblTahakkukBelge'), $('#lnkTahakkukBelge'), f.faturaYil, f.tahakkukBelgeNo);
                belgeBaglanti($('#lblOdemeBelge'), $('#lnkOdemeBelge'), f.odemeYil, f.odemeBelgeNo);
                $('#lblDurum').text(f.durumAd || '').prop('hidden', bos(f.durumAd));
                var r = $.map(f.satirlar || [], function (s) { return $.extend({ _id: ++sayac }, s); });
                grid.setGridOption('rowData', r.length ? r : [bosSatir()]);
            }
            $('#btnIptal, #btnYazdir').prop('disabled', yeni);
            setTimeout(yay, 0);
        }

        function oku() {
            return Sgb.al(UC + 'form/tanim', { kod: kod })
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    tanim = v;
                    fatura = v.fatura || null;
                    if (kod && !fatura) { mesaj('Fatura bulunamadı.', 'warning'); kod = 0; }
                    if (!grid) {
                        tutarGirisi = !!v.tutarGirisi;
                        gridKur();
                        $('#lblTutarGirisi').text(tutarGirisi ? 'Satır tutarı girilir, birim fiyat hesaplanır.' : 'Birim fiyat girilir, satır tutarı hesaplanır.');
                    }
                    if (v.otoNumara) $('#txtFaturaNo').prop('readonly', true).addClass('ft-oto-numara').attr('title', 'Fatura numarası otomatik verilir.');
                    $('#txtHesaplamaBirimFiyat').val(v.hesaplamaBirimFiyat || '');
                    $('#txtHesaplamaYuzde').val(v.hesaplamaYuzde || '');
                    formYaz();
                })
                .fail(hataGoster);
        }

        // ------------------------------------------------------------ Kaydet / İptal / Yazdır

        function kaydetVerisi() {
            var mb = muhBirim.deger();
            return {
                Kod: kod,
                Muhasebe: String(mb.muhasebe || ''),
                Birim: mb.birim || '',
                Mudurluk: $.trim($('#txtMudurluk').val()),
                MusteriKod: $.trim($('#txtVNo').val()),
                MusteriAdi: $('#txtUnvan').val(),
                MusteriAdiEk: $.trim($('#txtMusteriAdiEk').val()),
                FaturaNo: $.trim($('#txtFaturaNo').val()),
                FaturaTarihi: $('#txtFaturaTarihi').val() || '',
                IrsaliyeNo: $.trim($('#txtIrsaliyeNo').val()),
                IrsaliyeTarihi: $('#txtIrsaliyeTarihi').val() || '',
                Aciklama: $('#txtAciklama').val(),
                Tevkifat: parseInt($('#ddlTevkifat').val(), 10) || 0,
                MuhBelgeAciklama: fatura ? (fatura.muhBelgeAciklama || '') : '',
                TahakkukBelgeNo: fatura ? (fatura.tahakkukBelgeNo || '') : '',
                OdemeBelgeNo: fatura ? (fatura.odemeBelgeNo || '') : '',
                HesaplamaBirimFiyat: $.trim($('#txtHesaplamaBirimFiyat').val()),
                HesaplamaYuzde: $.trim($('#txtHesaplamaYuzde').val()),
                Satirlar: $.map(satirlar(), function (r) {
                    return {
                        FaturaKod: kod, Aciklama: r.ACIKLAMA || '', HesapKod: r.HESAPKOD || '',
                        Miktar: r.MIKTAR, KdvOran: r.KDVORAN, BirimFiyat: r.BIRIMFIYAT, Tutar: r.TUTAR
                    };
                })
            };
        }

        $('#btnKaydet').on('click', function () {
            grid.stopEditing();
            sor('Bilgiler kayıt edilecek. Onaylıyor musunuz?', function () {
                var $d = $('#btnKaydet').prop('disabled', true);
                Sgb.gonder(UC + 'form/kaydet', kaydetVerisi())
                    .done(function (s) {
                        if (!s.basarili) { mesaj(s.hata || 'Kayıt yapılamadı.', 'danger'); return; }
                        mesaj(s.bilgi, 'success');
                        if (s.kod) kod = s.kod;
                        oku();
                    })
                    .fail(hataGoster)
                    .always(function () { $d.prop('disabled', false); });
            });
        });

        // Eski btnIptal_Click: fatura iptal edilir ve pencere kapanır.
        $('#btnIptal').on('click', function () {
            if (!kod) return;
            sor('Bu Fatura iptal edilecek. Onaylıyor musunuz??', function () {
                var $d = $('#btnIptal').prop('disabled', true);
                Sgb.gonder(UC + 'form/iptal', { Kod: kod })
                    .done(function (s) {
                        if (!s.basarili) { mesaj(s.hata || 'İptal işlemi yapılamadı.', 'danger'); $d.prop('disabled', false); return; }
                        mesaj(s.bilgi, 'success');
                        if (cerceve()) setTimeout(pencereKapat, 600);
                        else oku();
                    })
                    .fail(function (x) { hataGoster(x); $d.prop('disabled', false); });
            });
        });

        $('#btnYazdir').on('click', function () {
            if (!kod) return;
            var $t = $(this).prop('disabled', true);
            Sgb.indir(UC + 'form/yazdir', { kod: kod, muhasebe: muhBirim.deger().muhasebe || '' }).then(function () { $t.prop('disabled', false); });
        });

        // ------------------------------------------------------------ Tahakkuk / Ödeme MİF (eski BelgePenceresiGoster)

        function belgeAc(tur) {
            if (!kod || !fatura) { mesaj('İlk önce faturayı kaydediniz.', 'warning'); return; }
            var f = fatura;
            belgePenceresi({
                tur: tur, form: true,
                tarih: f.odemeTarih || tanim.bugun,
                tahakkukBelgeNo: f.tahakkukBelgeNo, odemeBelgeNo: f.odemeBelgeNo, muhBelgeAciklama: f.muhBelgeAciklama,
                tahakkukHesapKod: f.tahakkukHesapKod, odemeHesapKod: f.odemeHesapKod, tahakkukAyniFis: f.tahakkukAyniFis,
                hesapYil: tanim.yil,
                // Eski BelgeyiGoster: yıl fatura tarihinden.
                belgeYil: parseInt(($('#txtFaturaTarihi').val() || '').substring(0, 4), 10) || tanim.yil,
                gonder: function (veri) {
                    return Sgb.gonder(UC + 'form/belge', $.extend(veri, { Kod: kod, Muhasebe: String(muhBirim.deger().muhasebe || '') }));
                },
                sonra: function () { oku(); }
            });
        }
        $('#btnTahakkuk').on('click', function () { belgeAc(1); });
        $('#btnOdeme').on('click', function () { belgeAc(2); });

        oku();
    }

    // ================================================================ Toplu Tahakkuk (eski FaturaTahakkukIslemleri.aspx)

    function topluSayfasi() {
        var q = new URLSearchParams(window.location.search);
        var kriter = {};
        $.each(['muhasebe', 'birim', 'mudurluk', 'faturaNo', 'musteri', 'durum', 'yil', 'tarih1', 'tarih2'], function (i, ad) {
            var v = q.get(ad) || '';
            if (ad === 'muhasebe' || ad === 'durum' || ad === 'yil') v = parseInt(v, 10) || 0;
            kriter[ad] = v;
        });
        var bilgi = { yil: 0, bugun: Sgb.bugun(), hesapYil: new Date().getFullYear() };

        var grid = Sgb.grid('#grdToplu', {
            columnDefs: [
                { field: 'MUHASEBEKOD', headerName: 'Muhasebe', width: 100 },
                { field: 'BIRIMKOD', headerName: 'Birim', width: 80 },
                { field: 'FATURATARIHI', headerName: 'Tarih', width: 100, valueFormatter: function (p) { return p.value ? Sgb.tarih(p.value) : ''; } },
                { field: 'FATURANO', headerName: 'Fatura No', width: 110 },
                { field: 'MUSTERIADI', headerName: 'Müşteri', flex: 1, minWidth: 200, tooltipField: 'MUSTERIADI' },
                { field: 'GENELTOPLAMPARABIRIMI', headerName: 'Toplam Tutar', width: 130, type: 'rightAligned' },
                { field: 'DURUM', headerName: 'Durum', width: 90 }
            ],
            getRowId: function (p) { return String(p.data.KOD); },
            rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
            overlayNoRowsTemplate: '<span>Kriterlere uygun fatura yok.</span>',
            onSelectionChanged: sayiYaz
        });

        function sayiYaz() {
            var n = 0;
            grid.forEachNode(function () { n++; });
            $('#lblSayi').text(n + ' fatura, ' + grid.getSelectedRows().length + ' seçili');
        }

        function listele() {
            var $d = $('#btnTopluYenile').prop('disabled', true);
            grid.setGridOption('loading', true);
            Sgb.al(UC + 'toplu/liste', kriter)
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    bilgi = v;
                    grid.setGridOption('rowData', v.satirlar || []);
                    sayiYaz();
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
        }
        $('#btnTopluYenile').on('click', listele);

        function belgeAc(tur) {
            var secili = grid.getSelectedRows();
            if (!secili.length) { mesaj('En az bir fatura seçilmiş olmalı', 'warning'); return; }
            belgePenceresi({
                tur: tur, form: false, tarih: bilgi.bugun, tahakkukAyniFis: true,
                hesapYil: bilgi.hesapYil, belgeYil: bilgi.yil || bilgi.hesapYil,
                gonder: function (veri) {
                    var s = grid.getSelectedRows();
                    return Sgb.gonder(UC + 'toplu/belge', $.extend(veri, {
                        Muhasebe: String(kriter.muhasebe || (s.length ? s[0].MUHASEBEKOD : '') || ''),
                        Faturalar: $.map(s, function (r) { return { KOD: r.KOD, FATURANO: r.FATURANO || '', MUSTERIKOD: r.MUSTERIKOD || '' }; })
                    }));
                },
                sonra: function () { listele(); }
            });
        }
        $('#btnTopluTahakkuk').on('click', function () { belgeAc(1); });
        $('#btnTopluOdeme').on('click', function () { belgeAc(2); });

        listele();
        yay();
    }
});
