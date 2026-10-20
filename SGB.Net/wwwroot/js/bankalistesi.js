// Banka Listesi İşlemleri, ButceMuhasebe/BankaListesiAna (eski aynı adlı Ext.NET sayfası). Solda muhasebe birimi ve tarihe göre
// banka listeleri; seçilen listenin başlık bilgileri ve listeye eklenmiş ödeme emri satırları sağda. Liste eklenir / silinir / onaylanır,
// ödeme emri satırları eklenir / çıkartılır, ödendi (dekont) ve basılan çek numarası verilir, Word / Excel / pdf yazdırılır, banka dosyası
// bankaya gönderilir ve bankadan sonuç alınır. Uçlar: /api/bankalistesi/* (Muhasebe/BankaListesiUclari.cs). Birim seçimi Sgb.muhasebeBirim.
$(function () {
    'use strict';

    var $sayfa = $('.bankalistesi');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/bankalistesi/';
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    var tanim = { sy: false, cekli: false, bugun: '', yevBasla: '' };
    var secili = null;   // soldaki listede seçili banka listesi (eski globRec)
    var odeSatir = null; // ödendi penceresindeki satır (eski detayRec)
    var cekSatir = null; // elle çek no penceresindeki satır (eski saklaRec)

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function para(x) { return x.value === null || x.value === undefined || x.value === '' ? '' : tutar2.format(Number(x.value) || 0); }
    function tarih(x) { return x.value ? Sgb.tarih(x.value) : ''; }
    function sayi(x) { return x.value ? String(x.value) : ''; }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });
    $('dialog').on('click', '[data-kapat]', function (e) { e.stopPropagation(); this.closest('dialog').close(); });

    // Sunucudan { hata } ya da { basarili, bilgi } döner.
    function gonder(adres, veri, $d) {
        if ($d) $d.prop('disabled', true);
        return Sgb.gonder(UC + adres, veri)
            .fail(hataGoster)
            .always(function () { if ($d) $d.prop('disabled', false); });
    }

    // ================================================================ sol: banka listeleri

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal' },
        kurumsalGizle: true,
        adGizle: true
    });

    function anahtarKimlik(r) { return r.saymanlik + '-' + r.birim + '-' + r.gun + '-' + r.siraNo; }

    var gridListe = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'birim', headerName: 'Birim', width: 60 },
            { field: 'odemeTarih', headerName: 'Ö.Tarih', width: 90, valueFormatter: tarih },
            { field: 'aciklama', headerName: 'Not', minWidth: 100, flex: 1, tooltipField: 'aciklama' },
            { field: 'cekEmriNo', headerName: 'Çek No', width: 75 },
            {
                field: 'dosyaAd', headerName: '', width: 36, sortable: false, resizable: false,
                cellRenderer: function (p) {
                    if (!p.data || !p.data.dosyaAd) return '';
                    var $a = $('<a href="#" class="bl-dosya" title="Bankaya gönderilen dosyayı indir"><span class="glyphicon glyphicon-folder-open" aria-hidden="true"></span></a>')
                        .attr('aria-label', p.data.dosyaAd)
                        .on('click', function (e) { e.preventDefault(); e.stopPropagation(); Sgb.indir(UC + 'dosya', { ad: p.data.dosyaAd }); });
                    return $a[0];
                }
            }
        ],
        getRowId: function (p) { return anahtarKimlik(p.data); },
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        rowClassRules: { 'bl-onayli': function (p) { return p.data && p.data.onayli; } },
        overlayNoRowsTemplate: '<span>Birim ve tarihleri seçip Listele\'ye basınız.</span>',
        onSelectionChanged: function () {
            var s = gridListe.getSelectedRows();
            if (s.length) listeSec(s[0]);
        }
    });

    function kriter() {
        var mb = muhBirim.deger();
        return {
            muhasebe: mb.muhasebe || '',
            birim: mb.birim || '',
            kurumsal: mb.kurumsal || '',
            basla: $('#txtBasla').val() || '',
            bitis: $('#txtBitis').val() || '',
            ara: $.trim($('#txtAra').val())
        };
    }

    // Eski btnListele_Click; ilk açılışta ilk kayıt seçilir (eski IlkKaydiAc).
    function listele(ilkiAc) {
        var $d = $('#btnListele').prop('disabled', true);
        gridListe.setGridOption('loading', true);
        Sgb.al(UC + 'liste', kriter())
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var satirlar = v.satirlar || [];
                gridListe.setGridOption('rowData', satirlar);
                gridListe.setGridOption('overlayNoRowsTemplate', '<span>Aradığınız kriterlere uygun banka listesi bulunamadı.</span>');
                if (secili) {
                    var n = gridListe.getRowNode(anahtarKimlik(secili));
                    if (n) { n.setSelected(true); return; }
                }
                if (ilkiAc && satirlar.length) { gridListe.getDisplayedRowAtIndex(0).setSelected(true); return; }
                listeSec(null);
            })
            .fail(hataGoster)
            .always(function () { gridListe.setGridOption('loading', false); $d.prop('disabled', false); });
    }

    $('#btnListele').on('click', function () { listele(false); });
    $('#frmKriter').on('keydown', 'input', function (e) {
        if (e.key === 'Enter' && !$(this).closest('#muhBirim').length) { e.preventDefault(); listele(false); }
    });

    // ---------------------------------------------------------------- yeni liste (eski wndYeni)

    var dlgYeni = document.getElementById('dlgYeni');
    $('#btnYeniListe').on('click', function () {
        $('#txtYeniOdemeTarihi').val(tanim.bugun || Sgb.bugun());
        $('#txtYeniAciklama').val('');
        dlgYeni.showModal();
        $('#txtYeniAciklama').trigger('focus');
    });

    $('#btnYeni').on('click', function () {
        var mb = muhBirim.deger();
        if (!mb.muhasebe || !mb.birim) { mesaj('Birimi seçiniz.', 'warning'); return; }
        gonder('ekle', { muhasebe: mb.muhasebe, birim: mb.birim, odemeTarih: $('#txtYeniOdemeTarihi').val() || '', aciklama: $.trim($('#txtYeniAciklama').val()) }, $(this))
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                dlgYeni.close();
                // Eski SonaEkle: liste sona eklenir ve seçilir.
                gridListe.applyTransaction({ add: [v.satir] });
                var n = gridListe.getRowNode(anahtarKimlik(v.satir));
                if (n) { n.setSelected(true); gridListe.ensureNodeVisible(n); }
            });
    });
    $('#frmYeni').on('keydown', 'input', function (e) { if (e.key === 'Enter') { e.preventDefault(); $('#btnYeni').trigger('click'); } });

    // ================================================================ sağ: seçilen liste

    function anahtar() {
        return secili ? { saymanlik: secili.saymanlik, birim: secili.birim, gun: secili.gun, siraNo: secili.siraNo } : null;
    }

    function seciliYok() {
        if (secili) return false;
        mesaj('Soldaki listeden bir banka listesi seçiniz.', 'warning');
        return true;
    }

    // Eski GridKomut: form doldurulur, ödemeler okunur, düğmeler duruma göre ayarlanır.
    function listeSec(r) {
        secili = r;
        $('#frmBaslik [data-alan]').each(function () { $(this).val(r ? (r[$(this).data('alan')] || '') : ''); });
        $('#txtCekEmriNo').val(r ? r.cekEmriNo || '' : '');
        baslikYaz();
        kolonAyarla();
        detayOku();
    }

    function baslikYaz() {
        var r = secili;
        $('#lblSecili').text(r ? '(' + r.saymanlik + '-' + r.birim + ', ' + Sgb.tarih(r.gun) + ' / ' + r.siraNo + ')' : '');
        $('#lblDurum').text(r ? r.durum || '' : '');
        $('#lblDonemNo').text(r && r.donemNo ? 'Dönem No: ' + r.donemNo : '');
        $('#lblDosyaAd').text(r ? r.dosyaAd || '' : '');
    }

    // Eski KolonAyarla
    function kolonAyarla() {
        var varMi = !!secili;
        var onayli = varMi && !!secili.onayli;
        $('[data-onaysiz]').prop('hidden', onayli);
        $('[data-onayli]').prop('hidden', !onayli);
        $('#btnOnayKaldir').prop('hidden', !(onayli && tanim.sy));
        $('#btnOEEkle').prop('hidden', onayli || tanim.cekli);
        $('.bl-odemeler .avans-arac button, #frmBaslik input, #btnCekNoOEGetir').not('[data-kapat]').prop('disabled', !varMi);
        gridOE.setColumnsVisible(['ode'], onayli);
    }

    // ---------------------------------------------------------------- listedeki ödemeler (eski grdOE)

    function oeAc(r) {
        if (!r.odemeEmriNo) return;
        Sgb.pencere('../ButceMuhasebe/OdemeEmriMIFFrame?' + $.param({ aramaYok: 1, yil: r.mifYil, belgeNo: r.odemeEmriNo }), 'Ödeme Emri ' + r.odemeEmriNo);
    }

    function dugme(simge, baslik, tikla) {
        return $('<button type="button" class="btn btn-link btn-xs bl-komut"></button>')
            .attr({ title: baslik, 'aria-label': baslik })
            .append('<span class="glyphicon ' + simge + '" aria-hidden="true"></span>')
            .on('click', function (e) { e.stopPropagation(); tikla(); });
    }

    var gridOE = Sgb.grid('#grdOE', {
        columnDefs: [
            { colId: 'no', headerName: '', width: 50, sortable: false, valueGetter: function (p) { return p.node.rowIndex + 1; } },
            {
                colId: 'ode', headerName: '', width: 40, sortable: false, resizable: false, hide: true,
                cellRenderer: function (p) {
                    return p.data ? dugme('glyphicon-usd', 'Dekont numarası girerek ödeme işlemini tamamlama', function () { odeGoster(p.data); })[0] : '';
                }
            },
            { field: 'mifYil', headerName: 'MİF Yıl', width: 70 },
            { field: 'mifYevmiyeNo', headerName: 'Yevmiye', width: 80, valueFormatter: sayi },
            { field: 'mifBelgeNo', headerName: 'MİF No', width: 100 },
            { field: 'mifSira', headerName: 'Sıra', width: 60 },
            { field: 'muhHesapKodu', headerName: 'Hesap Kodu', width: 110 },
            { field: 'tutar', headerName: 'Tutar', width: 110, type: 'rightAligned', valueFormatter: para },
            { field: 'doviz', headerName: 'Para B.', width: 70 },
            { field: 'ilgili', headerName: 'İlgili', width: 180, tooltipField: 'ilgili' },
            { field: 'tc_Vno', headerName: 'TC/VNo', width: 110 },
            { field: 'ilgBanka', headerName: 'Banka', width: 130, tooltipField: 'ilgBanka' },
            { field: 'ilgIBAN', headerName: 'Hesap No', width: 200 },
            { field: 'aciklama', headerName: 'Açıklama', width: 200, tooltipField: 'aciklama' },
            { field: 'dekontNo', headerName: 'Dekont', width: 110 },
            {
                field: 'odemeEmriNo', headerName: 'Ödeme Emri', width: 120,
                cellRenderer: function (p) {
                    if (!p.data || !p.value) return p.value || '';
                    var $a = $('<a href="#" title="Ödeme emrini aç"></a>').text(p.value + ' ')
                        .append('<span class="glyphicon glyphicon-search" aria-hidden="true"></span>')
                        .on('click', function (e) { e.preventDefault(); oeAc(p.data); });
                    return $a[0];
                }
            },
            { field: 'cekNo', headerName: 'Bas. Çek No', width: 100 },
            {
                colId: 'cek', headerName: '', width: 60, sortable: false, resizable: false,
                cellRenderer: function (p) {
                    if (!p.data) return '';
                    return $('<span class="bl-komutlar"></span>')
                        .append(dugme('glyphicon-plus', 'Otomatik çek numarası al', function () { cekNoAta(p.data, ''); }))
                        .append(dugme('glyphicon-pencil', 'Elle çek numarası ver', function () { cekNoElle(p.data); }))[0];
                }
            }
        ],
        getRowId: function (p) { return String(p.data.sira) + '-' + p.data.mifYil + '-' + p.data.mifBelgeNo + '-' + p.data.mifSira; },
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        overlayNoRowsTemplate: '<span>Soldaki listeden bir banka listesi seçiniz.</span>'
    });

    // Eski stoOE_RefreshData
    function detayOku() {
        $('#lblToplam').text('');
        $('#lblOzet').text('');
        if (!secili) {
            gridOE.setGridOption('rowData', []);
            gridOE.setGridOption('overlayNoRowsTemplate', '<span>Soldaki listeden bir banka listesi seçiniz.</span>');
            return;
        }
        gridOE.setGridOption('loading', true);
        var a = anahtar();
        Sgb.al(UC + 'detay', a)
            .done(function (v) {
                if (secili === null || anahtarKimlik(secili) !== anahtarKimlik(a)) return; // bu arada başka liste seçildi
                if (v.hata) { mesaj(v.hata, 'danger'); gridOE.setGridOption('rowData', []); return; }
                var satirlar = v.satirlar || [];
                gridOE.setGridOption('rowData', satirlar);
                gridOE.setGridOption('overlayNoRowsTemplate', '<span>Bu listede ödeme emri yok.</span>');
                $('#lblToplam').text(tutar2.format(Number(v.toplam) || 0));
                $('#lblOzet').text(satirlar.length ? satirlar.length + ' satır' : '');
            })
            .fail(hataGoster)
            .always(function () { gridOE.setGridOption('loading', false); });
    }

    function seciliSatirlar(uyari) {
        var s = gridOE.getSelectedRows();
        if (!s.length && uyari) mesaj(uyari, 'warning');
        return s;
    }

    // Soldaki satırın verisi değişince (eski globRec.set)
    function seciliGuncelle(degisen) {
        if (!secili) return;
        $.extend(secili, degisen);
        gridListe.applyTransaction({ update: [secili] });
        baslikYaz();
    }

    // ---------------------------------------------------------------- başlık bilgileri (eski btnGuncelle_Click)

    $('#btnGuncelle').on('click', function () {
        if (seciliYok()) return;
        var veri = anahtar();
        $('#frmBaslik [data-alan]').each(function () { veri[$(this).data('alan')] = $.trim($(this).val()); });
        gonder('guncelle', veri, $(this)).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            var degisen = {};
            $('#frmBaslik [data-alan]').each(function () { degisen[$(this).data('alan')] = $.trim($(this).val()); });
            seciliGuncelle(degisen);
            mesaj(v.bilgi || 'Bilgiler güncellendi!', 'success');
        });
    });

    // ---------------------------------------------------------------- sil (eski btnSilKayit_Click)

    $('#btnSilKayit').on('click', function () {
        if (seciliYok()) return;
        if (!window.confirm('Seçili banka listesi silinecek. Onaylıyor musunuz?')) return;
        var silinen = secili;
        gonder('sil', anahtar(), $(this)).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            gridListe.applyTransaction({ remove: [silinen] });
            listeSec(null);
            mesaj(v.bilgi || 'Kayıt silindi.', 'success');
        });
    });

    // ---------------------------------------------------------------- onayla / onay kaldır (eski btnOnayla_Click, FormOnaylandi)

    function onayla(onayKod, $d) {
        if (seciliYok()) return;
        gonder('onayla', $.extend(anahtar(), { onayKod: onayKod, donemNo: secili.donemNo || 0 }), $d).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            seciliGuncelle({ durum: v.durum, onayli: !!v.onayli, donemNo: v.donemNo });
            kolonAyarla();
            detayOku();
            mesaj(v.bilgi, 'success');
        });
    }
    $('#btnOnayla').on('click', function () { onayla(5, $(this)); });
    $('#btnOnayKaldir').on('click', function () { onayla(1, $(this)); });

    // ---------------------------------------------------------------- listeden çıkart (eski SilGoster, OEListedenCikart)

    $('#btnSil').on('click', function () {
        if (seciliYok()) return;
        var s = seciliSatirlar('Listeden çıkartılacak satırları seçiniz.');
        if (!s.length) return;
        if (!window.confirm('Seçtiğiniz ödeme emirlerini listeden çıkartmak istiyor musunuz?')) return;
        gonder('listeden-cikar', $.extend(anahtar(), { satirlar: s }), $(this)).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            detayOku();
        });
    });

    // ---------------------------------------------------------------- toplu kapat (eski btnTopluKapat + dmKapatmaTarihi)

    var dlgTopluKapat = document.getElementById('dlgTopluKapat');
    $('#btnTopluKapat').on('click', function () {
        if (seciliYok()) return;
        if (!seciliSatirlar('Kapatılacak satırları seçiniz.').length) return;
        $('#txtKapatmaTarihi').val(tanim.bugun || Sgb.bugun());
        dlgTopluKapat.showModal();
    });
    $('#btnSeciliKapat').on('click', function () {
        var s = seciliSatirlar('Kapatılacak satırları seçiniz.');
        if (!s.length) return;
        var t = $('#txtKapatmaTarihi').val();
        if (!t) { mesaj('Kapatma tarihini giriniz.', 'warning'); return; }
        if (!window.confirm('Seçili kayıtlar kapatılacaktır. Onaylıyor musunuz?')) return;
        gonder('toplu-kapat', $.extend(anahtar(), { tarih: t, satirlar: s }), $(this)).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            dlgTopluKapat.close();
            detayOku();
            mesaj(v.bilgi, 'success');
        });
    });

    // ---------------------------------------------------------------- ödendi (eski OdeGoster, btnOde_Click, OdemeYapildi)

    var dlgOde = document.getElementById('dlgOde');
    function odeGoster(r) {
        odeSatir = r;
        $('#txtDekontTarih').val('');
        $('#txtDekont').val('');
        dlgOde.showModal();
    }
    $('#btnOde').on('click', function () {
        if (!odeSatir || seciliYok()) return;
        var r = odeSatir;
        gonder('ode', $.extend(anahtar(), { sira: r.sira, dekontTarih: $('#txtDekontTarih').val() || '', dekontNo: $.trim($('#txtDekont').val()) }), $(this))
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                r.odemeEmriNo = v.odemeEmriNo;
                r.dekontNo = v.dekontNo;
                gridOE.applyTransaction({ update: [r] });
                dlgOde.close();
            });
    });

    // ---------------------------------------------------------------- basılan çek no (eski BasilanCekNoAta, wndCekNoAl)

    function cekNoAta(r, cekNo, $d) {
        if (seciliYok()) return;
        gonder('cek-no', $.extend(anahtar(), { satirlar: [r], cekNo: cekNo || '' }), $d).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            r.cekNo = v.cekNo;
            gridOE.applyTransaction({ update: [r] });
            dlgCekNo.close();
        });
    }
    var dlgCekNo = document.getElementById('dlgCekNo');
    function cekNoElle(r) {
        cekSatir = r;
        $('#txtCekNo').val('');
        dlgCekNo.showModal();
    }
    $('#btnCekYaz').on('click', function () { if (cekSatir) cekNoAta(cekSatir, $.trim($('#txtCekNo').val()), $(this)); });

    // ---------------------------------------------------------------- çek numarasından ödeme emirleri (eski btnCekNoOEGetir_Click)

    $('#btnCekNoOEGetir').on('click', function () {
        if (seciliYok()) return;
        var cekEmriNo = $.trim($('#txtCekEmriNo').val());
        gonder('cek-oe', $.extend(anahtar(), { cekEmriNo: cekEmriNo }), $(this)).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            seciliGuncelle({ cekEmriNo: v.cekEmriNo });
            detayOku();
        });
    });

    // ---------------------------------------------------------------- ödeme emri ekle (eski wndOESec)

    var dlgOESec = document.getElementById('dlgOESec');
    var gridOdenecek = null;

    function odenecekGrid() {
        if (gridOdenecek) return gridOdenecek;
        gridOdenecek = Sgb.grid('#grdOdenecekOE', {
            columnDefs: [
                {
                    colId: 'komut', headerName: '', width: 64, sortable: false, resizable: false,
                    cellRenderer: function (p) {
                        if (!p.data) return '';
                        return $('<span class="bl-komutlar"></span>')
                            .append(dugme('glyphicon-plus', 'Bu ödeme emri satırını banka listesine ekler, bu listeden kaldırır.', function () { odenecekEkle([p.data]); }))
                            .append(dugme('glyphicon-resize-vertical', 'Bu ödeme emrindeki tüm ödemeleri banka listesine ekler, bu listeden kaldırır.', function () {
                                var ayni = [];
                                gridOdenecek.forEachNode(function (n) { if (n.data && n.data.mifBelgeNo === p.data.mifBelgeNo) ayni.push(n.data); });
                                odenecekEkle(ayni);
                            }))[0];
                    }
                },
                { field: 'birimKod', headerName: 'Birim', width: 60 },
                { field: 'mifBelgeNo', headerName: 'MİF No', width: 100 },
                { field: 'mifSira', headerName: 'Sıra', width: 55 },
                { field: 'muhHesapKodu', headerName: 'Hesap Kodu', width: 110 },
                { field: 'tutar', headerName: 'Tutar', width: 110, type: 'rightAligned', valueFormatter: para },
                { field: 'doviz', headerName: 'Para B.', width: 65 },
                { field: 'ilgili', headerName: 'İlgili', width: 170, tooltipField: 'ilgili' },
                { field: 'tc_Vno', headerName: 'TC/VNo', width: 110 },
                { field: 'ilgBanka', headerName: 'Banka', width: 120, tooltipField: 'ilgBanka' },
                { field: 'ilgIBAN', headerName: 'Hesap No', width: 200 },
                { field: 'aciklama', headerName: 'Açıklama', width: 200, tooltipField: 'aciklama' }
            ],
            getRowId: function (p) { return p.data.mifYil + '-' + p.data.mifBelgeNo + '-' + p.data.mifSira; },
            rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
            overlayNoRowsTemplate: '<span>Yıl ve yevmiye tarihlerini girip Yenile\'ye basınız.</span>'
        });
        return gridOdenecek;
    }

    // Eski grdOdenecekOEGuncelle
    function odenecekOku() {
        if (seciliYok()) return;
        var g = odenecekGrid();
        var yil = parseInt($('#txtOEYil').val(), 10) || 0;
        if (!yil) { mesaj('Yıl bilgisini giriniz.', 'warning'); return; }
        var $d = $('#btnYenileOdenecek').prop('disabled', true);
        g.setGridOption('loading', true);
        Sgb.al(UC + 'odenecek', { yil: yil, saymanlik: secili.saymanlik, birim: secili.birim, basla: $('#txtOEBasla').val() || '', bitis: $('#txtOEBitis').val() || '' })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                g.setGridOption('rowData', v.satirlar || []);
                g.setGridOption('overlayNoRowsTemplate', '<span>Eklenebilecek ödeme emri bulunamadı.</span>');
            })
            .fail(hataGoster)
            .always(function () { g.setGridOption('loading', false); $d.prop('disabled', false); });
    }

    // Eski OdenecekEkle / TekSatirEkle: eklenen satırlar bu pencereden kalkar, banka listesi yenilenir.
    function odenecekEkle(satirlar) {
        if (!satirlar.length) { mesaj('Eklenecek satırları seçiniz.', 'warning'); return; }
        gonder('odenecek-ekle', $.extend(anahtar(), { satirlar: satirlar }), $('#btnOESeciliEkle')).done(function (v) {
            var eklenen = v.eklenen || [];
            if (eklenen.length) {
                var kaldir = [];
                $.each(eklenen, function (i, e) {
                    var n = gridOdenecek.getRowNode(e.mifYil + '-' + e.mifBelgeNo + '-' + e.mifSira);
                    if (n) kaldir.push(n.data);
                });
                gridOdenecek.applyTransaction({ remove: kaldir });
                detayOku();
            }
            if (v.hata) mesaj(v.hata, 'danger');
        });
    }

    $('#btnOEEkle').on('click', function () {
        if (seciliYok()) return;
        var g = odenecekGrid();
        Sgb.yilKutusu('#txtOEYil', [], parseInt((secili.gun || '').substring(0, 4), 10) || new Date().getFullYear());
        $('#txtOEBasla').val(tanim.yevBasla || '');
        $('#txtOEBitis').val(tanim.bugun || Sgb.bugun());
        g.setGridOption('rowData', []);
        dlgOESec.showModal();
        odenecekOku();
    });
    $('#frmOESec').on('submit', function () { odenecekOku(); return false; });
    $('#btnOESeciliEkle').on('click', function () { odenecekEkle(odenecekGrid().getSelectedRows()); });

    // ---------------------------------------------------------------- yazdır menüsü

    var $grpYazdir = $('#grpYazdir');
    function menuKapat() { $grpYazdir.removeClass('open'); $('#btnYazdirMenu').attr('aria-expanded', 'false'); }
    $('#btnYazdirMenu').on('click', function (e) {
        e.stopPropagation();
        var acik = !$grpYazdir.hasClass('open');
        $grpYazdir.toggleClass('open', acik);
        $(this).attr('aria-expanded', acik ? 'true' : 'false');
    });
    $(document).on('click', menuKapat);
    $(document).on('keydown', function (e) { if (e.key === 'Escape') menuKapat(); });

    function yazdir(tur, grup, pdf) {
        if (seciliYok()) return;
        var p = $.extend(anahtar(), { tur: tur });
        if (grup) p.grup = 1;
        if (pdf) p.pdf = 1;
        Sgb.indir(UC + 'yazdir', p);
    }
    $('#btnYazdir').on('click', function () { yazdir('word'); });
    $grpYazdir.on('click', '[data-yazdir]', function (e) {
        e.preventDefault();
        menuKapat();
        yazdir($(this).data('yazdir'), $(this).data('grup'), $(this).data('pdf'));
    });

    // Eski btnYazdirBanka_Click
    $('#mnuBankayaGonder').on('click', function (e) {
        e.preventDefault();
        menuKapat();
        if (seciliYok()) return;
        if (!window.confirm('Banka dosyası üretilip, bankaya aktarılacaktır. Onaylıyor musunuz?')) return;
        gonder('bankaya-gonder', anahtar()).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            if (v.dosyaAd) seciliGuncelle({ dosyaAd: v.dosyaAd });
            mesaj(v.bilgi, 'success');
        });
    });

    // Eski btnOkuBanka_Click
    $('#mnuBankadanAl').on('click', function (e) {
        e.preventDefault();
        menuKapat();
        if (seciliYok()) return;
        if (!window.confirm('Banka dosyası ile ilgili sonuç dosyası alınıp, kapatma kayıtları yapılacaktır onaylıyor musunuz?')) return;
        gonder('bankadan-al', anahtar()).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            mesaj(v.bilgi, 'success');
            detayOku();
        });
    });

    // Eski btnEvrakaGonder_Click
    $('#btnEvrakaGonder').on('click', function () {
        if (seciliYok()) return;
        gonder('ebys', anahtar(), $(this)).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            mesaj(v.bilgi || 'Aktarım işlemi başarılı', 'success');
        });
    });

    // ================================================================ açılış

    kolonAyarla();
    Sgb.al(UC + 'tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            if (t.muhasebe) muhBirim.ata({ muhasebe: t.muhasebe, birim: t.birim, kurumsal: t.kurumsal, birimAd: t.birimAd });
            $('#txtBasla').val(t.basla || '');
            $('#txtBitis').val(t.bitis || '');
            kolonAyarla();
            listele(true); // eski sayfa açılışta listeler ve ilk kaydı açar
        })
        .fail(hataGoster);

    // Tablolar sayfanın kalan yüksekliğini doldurur.
    function yay() {
        $.each(['grdListe', 'grdOE'], function (i, ad) {
            var el = document.getElementById(ad);
            if (!el || el.offsetParent === null) return;
            var ust = el.getBoundingClientRect().top;
            el.style.height = Math.max(260, Math.floor(window.innerHeight - ust - 24)) + 'px';
        });
    }
    $(window).on('resize', yay);
    yay();
});
