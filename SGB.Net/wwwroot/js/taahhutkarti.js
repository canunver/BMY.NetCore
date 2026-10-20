// Sözleşme ve Taahhüt Kartı, ButceMuhasebe/TaahhutKarti (?tur=2 Personel Taahhütleri); eski aynı adlı Ext1.NET sayfası.
// Belge Arama sekmesi: kıstaslar ve taahhüt listesi (Listele, Rapor, İptal Et; personelde sözleşme / maaş farkı / ödeme emri üretimi).
// Belge Düzenleme sekmesi: kart formu, değer artışları ve işlemler (Kaydet, Taahhüt Planı, Detay Veriler, Yazdır, Açılış Tahakkuku,
// Muavin Defteri, Belge Ekle, Sil, Dosya Kapat). Teminat / avans / borç / ödeme emri listeleri ve seçimleri bu sayfanın pencereleridir;
// Taahhüt Planı, Sözleşme Koşulları, Bakım Tarihçesi, Belge Ekle ve Ödeme Emri Kayıt alt sayfa olarak (Sgb.pencere) açılır.
// Uçlar: /api/taahhutkarti/* (Istemci/Muhasebe/TaahhutKartiUclari.cs).
$(function () {
    'use strict';

    var $kok = $('.taahhut');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/taahhutkarti/';
    var TUR = String($kok.data('tur')) === '2' ? 2 : 1;
    var PER = TUR === 2;
    var ODEME = String($kok.data('odeme')) === '1';
    var tutar = Sgb.mt.tutar, sayi = Sgb.mt.sayi;
    var tanim = {};       // sayfa açılış bilgileri (yıl, işler, alım usulleri...)
    var kart = null;      // açık kartın sunucudaki bilgileri (belge)

    function deger(secici) { var $e = $(secici); return $e.length ? $.trim(String($e.val() == null ? '' : $e.val())) : ''; }
    function tamsayi(x) { return parseInt(x, 10) || 0; }
    function tarihYaz(p) { return p.value ? Sgb.tarih(p.value) : ''; }
    function tutarYaz(p) { return p.value === '' || p.value == null ? '' : tutar(p.value); }
    function kod() { return tamsayi($('#hdnKod').val()); }
    function uyar(metin) { Sgb.bildirim(metin, 'warning'); }

    function sutunTutar(alan, baslik, ek) {
        return $.extend({ field: alan, headerName: baslik, width: 110, type: 'rightAligned', valueFormatter: tutarYaz }, ek || {});
    }

    function baglanti(metin, tikla) {
        var a = document.createElement('a');
        a.href = '#';
        a.textContent = metin == null ? '' : String(metin);
        a.addEventListener('click', function (e) { e.preventDefault(); tikla(); });
        return a;
    }

    function simgeTus(simge, ipucu, tikla) {
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-link btn-xs th-komut';
        b.title = ipucu;
        b.setAttribute('aria-label', ipucu);
        b.innerHTML = '<span class="glyphicon ' + simge + '" aria-hidden="true"></span>';
        b.addEventListener('click', function (e) { e.stopPropagation(); tikla(); });
        return b;
    }

    // Eski OdemeEmriAc: belge ayrı pencerede (ödeme emri / MİF ekranı) açılır.
    function odemeEmriAc(yil, belgeNo) {
        if (!belgeNo) { uyar('Tahakkuk belge numarası okunamadı.'); return; }
        Sgb.pencere('OdemeEmriMIFFrame?' + $.param({ aramaYok: 1, yil: yil, belgeNo: belgeNo }), 'Ödeme Emri / MİF Belgesi ' + yil + '/' + belgeNo);
    }

    // ------------------------------------------------------------------ sekmeler

    function sekme(ad) {
        $kok.find('.th-sekme').each(function () {
            var evet = $(this).attr('data-sekme') === ad;
            $(this).toggleClass('active', evet).attr('aria-selected', evet ? 'true' : 'false');
        });
        $kok.find('.th-sekme-bolme').each(function () { this.hidden = this.id !== ad; });
    }
    $kok.on('click', '.th-sekme', function () { sekme($(this).attr('data-sekme')); });

    // ------------------------------------------------------------------ Belge Arama

    var muhAra = Sgb.muhasebeBirim('#muhBirimAra', { kurumsalGizle: true, yil: function () { return deger('#srYil'); } });

    function aramaParam(personel) {
        var mb = muhAra.deger();
        return {
            tur: TUR,
            personel: personel ? 1 : 0,
            yil: deger('#srYil'),
            tarih: deger('#srTarih'),
            muhasebeKod: mb.muhasebe || '',
            birimKod: mb.birim || '',
            vergiNo: deger('#srVergiNo'),
            adSoyad: deger('#srAdSoyad'),
            projeSiraNo: deger('#srProjeSiraNo'),
            konu: deger('#srKonu'),
            eko: deger('#srEko'),
            isKod: deger('#srIs'),
            alimUsuluKod: deger('#srAlimUsulu'),
            durum: deger('#srDurum'),
            sozBas: deger('#srSozBas'),
            sozBit: deger('#srSozBit')
        };
    }

    var listeKolonlar = [];
    if (PER) {
        // Eski KomutCalistirSoz: satırın kişisi için sözleşme (ödeme emri durumu < 5) ya da maaş farkı üretilir.
        listeKolonlar.push({
            headerName: 'Kmt.', width: 56, sortable: false, cellRenderer: function (p) {
                return simgeTus('glyphicon-plus', 'Yeni sözleşme oluştur', function () {
                    uretimBaslat((p.data.ODEMEEMRIDURUM || 0) < 5 ? 1 : 2, [p.data]);
                });
            }
        });
    }
    listeKolonlar = listeKolonlar.concat([
        { field: 'DURUMADI', headerName: 'Durumu', width: 80 },
        { field: 'YIL', headerName: 'Yıl', width: 62 },
        {
            field: 'PROJESIRANO', headerName: 'Proje Sıra No.', width: 110,
            cellRenderer: function (p) { return baglanti(p.value, function () { belgeAc(p.data.KOD); }); }
        },
        { field: 'MUHASEBEAD', headerName: 'Muhasebe', width: 140, hide: true },
        { field: 'BIRIMAD', headerName: 'Birim', width: 170, tooltipField: 'BIRIMAD' },
        { field: 'KONU', headerName: PER ? 'Personel Tip ve Unvanı' : 'Konu', flex: 1, minWidth: 200, tooltipField: 'KONU' },
        { field: 'MUTEAHHITKIMLIKNO', headerName: 'Kimlik/Vergi No', width: 115 },
        { field: 'MUTEAHHITUNVAN', headerName: PER ? 'Ad Soyad' : 'Müteahhit Ad, Soyad/Ünvan', width: 180, tooltipField: 'MUTEAHHITUNVAN' },
        { field: 'SOZLESMETARIHI', headerName: 'Söz. Tar.', width: 92, valueFormatter: tarihYaz },
        sutunTutar('IHALEBEDELI', 'İhale Bedeli'),
        {
            field: 'ODEMEEMRIYILNO', headerName: 'Tahakkuk Belge No', width: 130,
            cellRenderer: function (p) {
                if (!p.data.ODEMEEMRINO) return '';
                return baglanti(p.value, function () { odemeEmriAc(p.data.ODEMEEMRIYIL, p.data.ODEMEEMRINO); });
            }
        },
        sutunTutar('TAAHHUTTUTARI', 'Taahhüt Tutarı'),
        sutunTutar('ODENENTUTAR', 'Ödenen Tutar'),
        sutunTutar('KALANTUTAR', 'Kalan Tutar'),
        { field: 'ISLEMYAPANAD', headerName: 'İşlemi Yapan', width: 130 }
    ]);

    var grdListe = Sgb.grid('#grdListe', {
        columnDefs: listeKolonlar,
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        getRowId: function (p) { return String(p.data.KOD); },
        overlayNoRowsTemplate: '<span>Kıstasları girip Listele\'ye basınız.</span>',
        onRowDoubleClicked: function (e) { belgeAc(e.data.KOD); }
    });

    function listele(personel) {
        return Sgb.mt.oku(UC + 'liste', aramaParam(personel), grdListe).done(function (v) {
            var satirlar = v.satirlar || [];
            var t = 0, o = 0;
            $.each(satirlar, function (i, r) { t += Number(r.TAAHHUTTUTARI) || 0; o += Number(r.ODENENTUTAR) || 0; });
            grdListe.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
            grdListe.setGridOption('rowData', satirlar);
            $('#lblOzet').text(satirlar.length ? satirlar.length + ' kayıt, taahhüt ' + tutar(t) + ', ödenen ' + tutar(o) + ', kalan ' + tutar(t - o) : '');
        });
    }

    function seciliSatirlar() { return grdListe.getSelectedRows(); }

    $('#frmArama').on('submit', function (e) { e.preventDefault(); listele(false); });
    $('#btnListele').on('click', function () { listele(false); });
    $('#btnListelePer').on('click', function () { listele(true); });
    $('#btnRapor').on('click', function () {
        var p = aramaParam(false);
        delete p.personel;
        Sgb.indir(UC + 'rapor', p);
    });

    // Eski btnIptalListe_Click: seçilen taahhüt kayıtları iptal edilir.
    $('#btnIptalListe').on('click', function () {
        var s = seciliSatirlar();
        if (!s.length) { uyar('Lütfen seçim yapınız!'); return; }
        Sgb.mt.islem(UC + 'iptal', { kodlar: $.map(s, function (r) { return r.KOD; }) },
            'Seçilen taahhüt kayıtları iptal edilecektir, onaylıyor musunuz?').done(function () { listele(false); });
    });

    // ------------------------------------------------------------------ personel: sözleşme / maaş farkı / ödeme emri üretimi

    var uretim = { islemKod: 1, satirlar: [] };

    // Eski SecililileriUret: parametre penceresi açılır, Üret'e basınca seçilenler sırayla işlenir.
    function uretimBaslat(islemKod, satirlar) {
        if (!satirlar.length) { uyar('Lütfen listeden kişi seçiniz.'); return; }
        uretim = { islemKod: islemKod, satirlar: satirlar };
        if (islemKod === 1) {
            if (!$('#prBasTar').val()) $('#prBasTar').val(tanim.sozBasTar || '');
            if (!$('#prBitTar').val()) $('#prBitTar').val(tanim.sozBitTar || '');
            $('#dlgSozParam')[0].showModal();
        } else {
            if (!$('#prDegTar').val()) $('#prDegTar').val(tanim.bugun || '');
            $('#dlgDegParam')[0].showModal();
        }
    }

    // Eski RefreshProgress / SozlesmeVeyaDegOlustur: kişiler tek tek üretilir, ilerleme gösterilir, hatalar sonda toplanır.
    function uretimCalistir(param) {
        var kuyruk = uretim.satirlar.slice();
        var toplam = kuyruk.length, biten = 0, sonuc = '';
        var dlg = $('#dlgDurum')[0];
        function goster() {
            $('#prgDurum').val(toplam ? biten / toplam : 0);
            $('#lblDurum').text('Sözleşme üretim işlemi devam ediyor... ' + biten + '/' + toplam);
        }
        function sonraki() {
            goster();
            if (!kuyruk.length) {
                dlg.close();
                if (sonuc) window.alert(sonuc);
                else Sgb.bildirim('Üretim işlemi tamamlandı.', 'success');
                listele(false);
                return;
            }
            var r = kuyruk.shift();
            Sgb.gonder(UC + 'personel/sozlesme-uret', $.extend({ sozKod: r.KOD, tcKimlikNo: r.MUTEAHHITKIMLIKNO || '' }, param))
                .done(function (v) {
                    if (v && v.hata) sonuc += v.hata + '\n';
                    else if (v && v.sonuc) sonuc += v.sonuc + '\n';
                })
                .fail(function (xhr) { if (xhr.status !== 401) sonuc += (r.MUTEAHHITKIMLIKNO || r.KOD) + ': işlem yapılamadı.\n'; })
                .always(function () { biten++; sonraki(); });
        }
        dlg.showModal();
        sonraki();
    }

    $('#btnSozUret').on('click', function () { uretimBaslat(1, seciliSatirlar()); });
    $('#btnDegUret').on('click', function () { uretimBaslat(2, seciliSatirlar()); });
    $('#btnSozParamUret').on('click', function () {
        if (!$('#prBasTar').val() || !$('#prBitTar').val()) { uyar('Sözleşme başlangıç ve bitiş tarihlerini giriniz.'); return; }
        $('#dlgSozParam')[0].close();
        uretimCalistir({ basTar: $('#prBasTar').val(), bitTar: $('#prBitTar').val(), sure: tamsayi($('#prSaat').val()), islemKod: 1 });
    });
    $('#btnDegParamUret').on('click', function () {
        if (!$('#prDegTar').val()) { uyar('Maaş farkı tarihini giriniz.'); return; }
        $('#dlgDegParam')[0].close();
        uretimCalistir({ basTar: $('#prDegTar').val(), bitTar: '', sure: tamsayi($('#prDegSaat').val()), islemKod: 2 });
    });

    // Eski PersonelMIFUret: sorgu alanlarındaki yıl / muhasebe / birim için.
    $('#btnOEUret').on('click', function () {
        var mb = muhAra.deger();
        if (!tamsayi(deger('#srYil'))) { uyar('Ödeme emri üretmek için sorgu alanlarında Yıl giriniz.'); return; }
        Sgb.mt.islem(UC + 'personel/mif-uret', { yil: tamsayi(deger('#srYil')), muhasebeKod: tamsayi(mb.muhasebe), birimKod: mb.birim || '' },
            'Seçilen birimde, MİF üretilmemiş / onaylanmamış sözleşmeler için MİF üretilecektir. Onaylıyor musunuz?');
    });

    // ------------------------------------------------------------------ Belge Düzenleme: form

    var muhBelge = Sgb.muhasebeBirim('#muhBirim', { kurumsalGizle: true, yil: function () { return deger('#txtYil'); } });

    function secenekDoldur($s, liste, bosMetin, bosDeger) {
        if (!$s.length) return;
        var simdi = $s.val();
        $s.empty();
        if (bosMetin !== null) $s.append($('<option></option>').val(bosDeger).text(bosMetin));
        $.each(liste || [], function (i, x) { $s.append($('<option></option>').val(String(x.kod)).text(x.ad)); });
        if (simdi != null) $s.val(simdi);
    }

    // Değer artışları tablosu: normal kartta satırlar tabloda düzenlenir (beyaz hücreler), Kaydet ile kartla birlikte gider.
    var DA_DUZENLE = !PER;
    function daKolon(alan, baslik, ek) {
        return $.extend({ field: alan, headerName: baslik, width: 110, editable: DA_DUZENLE, cellClass: DA_DUZENLE ? 'th-girilir' : '' }, ek || {});
    }
    var grdDA = Sgb.grid('#grdDegerArtisi', {
        columnDefs: [
            {
                field: 'ODEMEEMRINO', headerName: 'MİF Numarası', width: 110, cellClass: DA_DUZENLE ? 'th-salt' : '',
                cellRenderer: function (p) { return p.value ? baglanti(p.value, function () { odemeEmriAc(p.data.ODEMEEMRIYIL, p.value); }) : ''; }
            },
            {
                headerName: 'Komutlar', width: 100, sortable: false, cellClass: DA_DUZENLE ? 'th-salt' : '',
                cellRenderer: function (p) {
                    var d = document.createElement('span');
                    d.appendChild(simgeTus('glyphicon-pencil', 'Düzenleme için aç', function () { daAc(p.data); }));
                    d.appendChild(simgeTus('glyphicon-calendar', 'Taahhüt Planı Değ.', function () { planAc(p.data.KOD); }));
                    d.appendChild(simgeTus('glyphicon-usd', 'Muhasebe Fişi Hazırla', function () { daMuhasebeFisi(p.data); }));
                    return d;
                }
            },
            daKolon('ONAYTARIHI', 'Onay Tarihi', { valueFormatter: tarihYaz, cellEditor: 'agDateStringCellEditor' }),
            daKolon('SOZLESMETARIHI', 'MİF Tarihi', { valueFormatter: tarihYaz, cellEditor: 'agDateStringCellEditor' }),
            daKolon('SOZLESMETUTAR', 'Sözleşme Tutarı', { width: 120, type: 'rightAligned', valueFormatter: tutarYaz, cellEditor: 'agNumberCellEditor', valueParser: function (p) { return sayi(p.newValue); } }),
            daKolon('DAMGAVERGISITARIHI', 'Damga V. Tarihi', { valueFormatter: tarihYaz, cellEditor: 'agDateStringCellEditor' }),
            daKolon('DAMGAVERGISINO', 'Damga V. No'),
            daKolon('DAMGAVERGISITUTAR', 'Damga V. Tutarı', { width: 120, type: 'rightAligned', valueFormatter: tutarYaz, cellEditor: 'agNumberCellEditor', valueParser: function (p) { return sayi(p.newValue); } })
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        overlayNoRowsTemplate: '<span>Değer artışı yok.</span>'
    });

    function daSatirlari() {
        grdDA.stopEditing();
        var l = [];
        grdDA.forEachNode(function (n) { l.push(n.data); });
        return l;
    }

    function kapaliAyarla(kapali) {
        $('#lblKapali').prop('hidden', !kapali);
        $('#btnKaydet, #btnSil, #btnDosyaKapat, #btnOdemeEmriSil').prop('disabled', kapali);
    }

    function tahakkukYaz(yil, no) {
        var var_ = !!no;
        $('#lnkTahakkuk').prop('hidden', !var_).text(var_ ? yil + ' / ' + no : '');
        $('#lblTahakkukYok').prop('hidden', var_);
    }

    // Eski Temizle (Yeni Kayıt): muhasebe / birim ve yıl korunur.
    function temizle() {
        kart = null;
        $('#frmBelge').find('input[type=text], input[type=date], input[type=hidden], textarea').val('');
        $('#ddlIs').val('0');
        $('#ddlAlimUsulu').val('');
        $('#ddlParaBirimi').val('');
        $('#ddlSgk').val('0');
        $('#lblIslemYapan').text('');
        if (!deger('#txtYil')) $('#txtYil').val(tanim.donemYil || new Date().getFullYear());
        grdDA.setGridOption('rowData', []);
        tahakkukYaz(0, '');
        kapaliAyarla(false);
    }

    function tarihAl(s) { return s || ''; }

    // Eski DegerleriGetir.
    function formYaz(v) {
        var b = v.belge || {};
        kart = b;
        if (v.isler) secenekDoldur($('#ddlIs'), v.isler, '', '0');
        $('#hdnKod').val(b.kod);
        $('#txtYil').val(b.yil || '');
        muhBelge.ata({ muhasebe: b.muhasebeKod || '', birim: b.birimKod || '', kurumsal: '', birimAd: '' });
        muhBelge.yenile();
        $('#txtSiraNo').val(b.projeSiraNo);
        $('#txtDptSiraNo').val(b.DptProjeSiraNo);
        $('#txtKonu').val(b.konu);
        $('#txtVNo').val(b.muteahhitKimlikNo);
        $('#txtUnvan').val(b.muteahhitUnvan);
        $('#txtAdres').val(b.muteahhitAdres);
        $('#txtKesifBedeli').val(tutar(b.kesifBeldeli));
        $('#txtIhaleBedeli').val(tutar(b.ihaleBedeli));
        $('#ddlParaBirimi').val(b.parabirimiKod || '');
        $('#txtIseBaslama').val(tarihAl(b.iseBaslamaTarihi));
        $('#txtIsBitirme').val(tarihAl(b.isBitirmeTarihi));
        $('#txtSozlesmeTarihi').val(tarihAl(b.sozlesmeTarihi));
        $('#txtGeciciKabul').val(tarihAl(b.geciciKabulTarihi));
        $('#txtSure1').val(b.sureUzatimi1);
        $('#txtSure2').val(b.sureUzatimi2);
        $('#txtSure3').val(b.sureUzatimi3);
        $('#ddlIs').val(String(b.isKod || 0));
        $('#txtGarantiBas').val(tarihAl(b.garantiBaslamaTarihi));
        $('#txtGarantiBit').val(tarihAl(b.garantiBitisTarihi));
        $('#txtDVTarih').val(tarihAl(b.damgaVergisiTarihi));
        $('#txtDVNo').val(b.damgaVergisiNo);
        $('#txtDVTutar').val(tutar(b.damgaVergisiTutar));
        $('#txtSozDVTarih').val(tarihAl(b.sozlesmeDVTarihi));
        $('#txtSozDVNo').val(b.sozlesmeDVNo);
        $('#txtSozDVTutar').val(tutar(b.sozlesmeDVTutar));
        $('#txtIndirimOrani').val(tutar(b.indirimOrani));
        $('#txtGerceklesen').val(tutar(b.gerceklesenTaahhutTutar));
        $('#txtOdenekTertip').val(b.odenekTertip);
        $('#txtDuranTertip').val(b.duranTertip);
        $('#txtAciklama').val(b.aciklama);
        $('#ddlAlimUsulu').val(b.alimUsuluKod || '');
        $('#ddlSgk').val(String(b.sgkBildirimi || 0));
        $('#lblIslemYapan').text(b.islemYapanAd || '');
        grdDA.setGridOption('rowData', v.degerArtisi || []);
        tahakkukYaz(b.odemeEmriYil, b.odemeEmriNo);
        kapaliAyarla(b.kapandi === 1 || b.kapandi === 9);
    }

    function belgeAc(k) {
        k = tamsayi(k);
        if (k <= 0) return $.Deferred().reject().promise();
        sekme('sekmeBelge');
        return Sgb.mt.oku(UC + 'belge', { kod: k }).done(formYaz);
    }
    function belgeYenile() { return kod() > 0 ? belgeAc(kod()) : $.Deferred().resolve().promise(); }

    function kayitGerekli() {
        if (kod() > 0) return true;
        uyar('Önce taahhüt kartını kaydedin ya da listeden bir kart açın.');
        return false;
    }

    $('#lnkTahakkuk').on('click', function (e) {
        e.preventDefault();
        if (kart) odemeEmriAc(kart.odemeEmriYil, kart.odemeEmriNo);
    });

    $kok.on('blur', '.th-tutar', function () {
        var v = $.trim($(this).val());
        if (v !== '') $(this).val(tutar(sayi(v)));
    });

    // Eski btnKaydet_Click.
    function kaydetIstegi() {
        var mb = muhBelge.deger();
        var da = $.map(daSatirlari(), function (r) {
            return {
                KOD: r.KOD || '',
                ONAYTARIHI: r.ONAYTARIHI || '',
                SOZLESMETARIHI: r.SOZLESMETARIHI || '',
                SOZLESMETUTAR: Number(r.SOZLESMETUTAR) || 0,
                DAMGAVERGISITARIHI: r.DAMGAVERGISITARIHI || '',
                DAMGAVERGISINO: r.DAMGAVERGISINO || '',
                DAMGAVERGISITUTAR: Number(r.DAMGAVERGISITUTAR) || 0,
                ODEMEEMRIYIL: tamsayi(r.ODEMEEMRIYIL),
                ODEMEEMRINO: r.ODEMEEMRINO || ''
            };
        });
        return {
            tur: TUR,
            kod: kod() > 0 ? kod() : -1,
            yil: tamsayi(deger('#txtYil')),
            muhasebeKod: tamsayi(mb.muhasebe),
            birimKod: mb.birim || '',
            projeSiraNo: deger('#txtSiraNo'),
            DptProjeSiraNo: deger('#txtDptSiraNo'),
            muteahhitKimlikNo: deger('#txtVNo'),
            muteahhitUnvan: deger('#txtUnvan'),
            muteahhitAdres: deger('#txtAdres'),
            konu: deger('#txtKonu'),
            kesifBeldeli: sayi(deger('#txtKesifBedeli')),
            indirimOrani: sayi(deger('#txtIndirimOrani')),
            ihaleBedeli: sayi(deger('#txtIhaleBedeli')),
            gerceklesenTaahhutTutar: sayi(deger('#txtGerceklesen')),
            damgaVergisiTarihi: deger('#txtDVTarih'),
            damgaVergisiNo: deger('#txtDVNo'),
            damgaVergisiTutar: sayi(deger('#txtDVTutar')),
            sozlesmeDVTarihi: deger('#txtSozDVTarih'),
            sozlesmeDVNo: deger('#txtSozDVNo'),
            sozlesmeDVTutar: sayi(deger('#txtSozDVTutar')),
            sozlesmeTarihi: deger('#txtSozlesmeTarihi'),
            iseBaslamaTarihi: deger('#txtIseBaslama'),
            geciciKabulTarihi: deger('#txtGeciciKabul'),
            isBitirmeTarihi: deger('#txtIsBitirme'),
            sureUzatimi1: deger('#txtSure1'),
            sureUzatimi2: deger('#txtSure2'),
            sureUzatimi3: deger('#txtSure3'),
            isKod: tamsayi(deger('#ddlIs')),
            garantiBaslamaTarihi: deger('#txtGarantiBas'),
            garantiBitisTarihi: deger('#txtGarantiBit'),
            alimUsuluKod: deger('#ddlAlimUsulu'),
            sgkBildirimi: tamsayi(deger('#ddlSgk')),
            odenekTertip: deger('#txtOdenekTertip'),
            duranTertip: deger('#txtDuranTertip'),
            aciklama: deger('#txtAciklama'),
            parabirimiKod: deger('#ddlParaBirimi'),
            degerArtisi: da
        };
    }

    $('#btnKaydet').on('click', function () {
        Sgb.mt.islem(UC + 'kaydet', kaydetIstegi(), 'Bilgiler kaydedilecek. Onaylıyor musunuz?').done(function (v) {
            if (v.uyari) Sgb.bildirim(v.uyari, 'warning');
            if (v.kod > 0) $('#hdnKod').val(v.kod);
            belgeYenile();
            listele(false);
        });
    });

    $('#btnYeni').on('click', temizle);

    $('#btnSil').on('click', function () {
        if (!kayitGerekli()) return;
        Sgb.mt.islem(UC + 'sil', { kod: kod() }, 'Taahhüt kartı silinecek. Emin misiniz?').done(function () {
            temizle();
            listele(false);
        });
    });

    $('#btnDosyaKapat').on('click', function () {
        if (!kayitGerekli()) return;
        Sgb.mt.islem(UC + 'kapat', { kod: kod() }, 'Taahhüt dosyası kapatılacak. Emin misiniz?').done(function () {
            belgeYenile();
            listele(false);
        });
    });

    $('#btnYazdir').on('click', function () {
        if (!kayitGerekli()) return;
        Sgb.indir(UC + 'yazdir', { kod: kod() });
    });

    // Eski MuavinDefteriAc: 920 hesabının müteahhit alt hesabı, proje numarası ile.
    $('#btnMuavin').on('click', function () {
        Sgb.pencere('MuavinDefteriAltHesapBazindaRapor?' + $.param({ hesapKod: '920', altHesapKod: deger('#txtVNo'), projeNo: deger('#txtSiraNo'), yil: 0 }), 'Muavin Defteri Listesi');
    });

    $('#btnBelgeEkle').on('click', function () {
        if (!kayitGerekli()) return;
        Sgb.pencere('TanimBelgeEkle?' + $.param({ ilgiKod: kod(), tip: 1 }), 'Sözleşme ve Taahhüt Kartı Dosya Yükleme');
        $('dialog.sgb-pencere').last().addClass('th-pencere-yarim');
    });

    function planAc(daKod) {
        if (!kayitGerekli()) return;
        var p = { kod: kod() };
        if (daKod) p.dakod = daKod;
        Sgb.pencere('Taahhutler?' + $.param(p), daKod ? 'Taahhüt Planı (Değer Artışı)' : 'Taahhüt Planı');
    }
    $('#btnPlan').on('click', function () { planAc(''); });

    // Kişi / firma (eski FirmaBilgisiAc / FirmaBilgisiGoster)
    window.FirmaBilgisiGoster = function (firma) {
        $('#txtVNo').val(firma.vergiNo || '');
        $('#txtUnvan').val(firma.ad || '');
        $('#txtAdres').val(firma.adres || '');
    };
    $('#btnFirmaSec').on('click', function () {
        Sgb.pencere('../HarcamaSurecleri/ListeFirmaKisi?' + $.param({ per: 1, muhasebe: muhBelge.deger().muhasebe || '' }), 'Kişi / Firma Listesi');
    });

    // Tertip seçimi (eski TerTipSecim -> ListeTertip.aspx)
    $kok.on('click', '[data-tertip]', function () {
        var $hedef = $($(this).attr('data-tertip'));
        var mb = muhBelge.deger();
        Sgb.tertipSec({
            yil: tamsayi(deger('#txtYil')) || new Date().getFullYear(),
            muhasebe: mb.muhasebe || '',
            birim: mb.birim || '',
            coklu: false,
            secKapat: true,
            secilince: function (satirlar) { if (satirlar && satirlar.length) $hedef.val(satirlar[0].TERTIP); }
        });
    });

    // Detay Veriler menüsü
    $kok.on('click', '.dropdown-menu a[data-islem]', function (e) {
        e.preventDefault();
        var islem = $(this).attr('data-islem');
        if (islem === 'karaliste') {
            var mb = muhBelge.deger();
            Sgb.indir(UC + 'karaliste', { muhasebe: mb.muhasebe || 0, birim: mb.birim || '' });
            return;
        }
        if (!kayitGerekli()) return;
        if (islem === 'sozlesme') {
            var t = $(this).attr('data-tur');
            Sgb.pencere('TanimSozlesme?' + $.param({ taahhutKod: kod(), tur: t }), $(this).text());
            $('dialog.sgb-pencere').last().addClass('th-pencere-yarim'); // ekranın yarısı (kullanıcı isteği 2026-10-19)
        } else if (islem === 'bakim') {
            Sgb.pencere('TaahhutBakimTarihce?' + $.param({ taahhutKod: kod() }), 'Bakım Tarihçesi');
        } else {
            pencereAc(islem);
        }
    });

    // ------------------------------------------------------------------ değer artışları

    $('#btnDAEkle').on('click', function () {
        grdDA.applyTransaction({ add: [{ KOD: '', ONAYTARIHI: '', SOZLESMETARIHI: '', SOZLESMETUTAR: 0, DAMGAVERGISITARIHI: '', DAMGAVERGISINO: '', DAMGAVERGISITUTAR: 0, ODEMEEMRIYIL: 0, ODEMEEMRINO: '' }] });
    });
    $('#btnDASil').on('click', function () {
        var s = grdDA.getSelectedRows();
        if (!s.length) { uyar('Silinecek satırı seçiniz.'); return; }
        grdDA.applyTransaction({ remove: s });
    });

    var daForm = Sgb.mt.form('#frmDA');

    // Eski DegerArtisiAc: satır yoksa boş pencere (yeni değer artışı).
    function daAc(r) {
        r = r || {};
        daForm.temizle();
        daForm.yaz({
            kod: r.KOD || '', odemeEmriYil: r.ODEMEEMRIYIL || 0, odemeEmriNo: r.ODEMEEMRINO || '',
            sozlesmeTarihi: r.SOZLESMETARIHI || '', onayTarihi: r.ONAYTARIHI || '',
            sozlesmeTutari: r.SOZLESMETUTAR ? tutar(r.SOZLESMETUTAR) : '',
            damgaVergisiTarihi: r.DAMGAVERGISITARIHI || '', damgaVergisiNo: r.DAMGAVERGISINO || '',
            damgaVergisiTutari: r.DAMGAVERGISITUTAR ? tutar(r.DAMGAVERGISITUTAR) : ''
        });
        $('#dlgDegerArtisi')[0].showModal();
    }

    // Eski btnDegerArtisi_Click: önce açılış tahakkuku kontrol edilir.
    $('#btnDAPencere').on('click', function () {
        if (kod() <= 0) { uyar('Değer artışı kaydetmeden önce, lütfen daha önce kaydedilmiş bir kaydı açınız!'); return; }
        Sgb.mt.oku(UC + 'degerartisi/acilabilir', { kod: kod() }).done(function () { daAc(null); });
    });

    $('#btnDAKaydet').on('click', function () {
        if (kod() <= 0) { uyar('Değer artışı kaydetmeden önce, lütfen daha önce kaydedilmiş bir kaydı açınız!'); return; }
        var f = daForm.oku();
        Sgb.mt.islem(UC + 'degerartisi/kaydet', {
            taahhutKod: kod(), kod: f.kod, onayTarihi: f.onayTarihi, sozlesmeTarihi: f.sozlesmeTarihi,
            damgaVergisiTarihi: f.damgaVergisiTarihi, damgaVergisiTutari: sayi(f.damgaVergisiTutari), damgaVergisiNo: f.damgaVergisiNo,
            sozlesmeTutari: sayi(f.sozlesmeTutari), odemeEmriYil: tamsayi(f.odemeEmriYil), odemeEmriNo: f.odemeEmriNo
        }).done(function (v) {
            if (v.degerArtisi) grdDA.setGridOption('rowData', v.degerArtisi);
            $('#dlgDegerArtisi')[0].close();
        });
    });

    // Eski KomutCalistir 'DegerArtisiOdemeEmri': personelde MİF sunucuda üretilir, diğerlerinde hesap kodu penceresi açılır.
    function daMuhasebeFisi(r) {
        if (!kayitGerekli()) return;
        if (PER) {
            if (!r.KOD) { uyar('İlk önce değer artışı satırını kayıt edin'); return; }
            Sgb.mt.islem(UC + 'personel/da-mif-uret', { kod: kod(), daKod: r.KOD }).done(function (v) {
                if (v.degerArtisi) grdDA.setGridOption('rowData', v.degerArtisi);
            });
            return;
        }
        acilisAc(2, r);
    }

    // ------------------------------------------------------------------ açılış tahakkuku (eski OdemeEmriHesapKodSec / OdemeEmriOlustur)

    var acilis = { tur: 1, daKod: '' };
    var hkBorc = Sgb.hesapKodu('#txtAcBorc', { yil: '#txtAcYil', sonSeviye: true, uc: UC + 'hesap/' });
    var hkAlacak = Sgb.hesapKodu('#txtAcAlacak', { yil: '#txtAcYil', sonSeviye: true, uc: UC + 'hesap/' });

    // Ödeneğin tertibindeki ekonomik koddan 920 alt hesabı (eski kodun aynısı; döviz varsa para birimine göre ek).
    function ekoEk() {
        var t = deger('#txtOdenekTertip').split('-');
        if (t.length < 4) return '';
        var eko = t[3].split('.').join('');
        var h1, h2, h3;
        if (eko.length === 8) {
            h1 = eko.substring(0, 2); h2 = eko.substring(2, 4); h3 = eko.substring(4, 6);
        } else {
            h1 = eko.substring(0, 2); h2 = eko.substring(2, 3); h3 = eko.substring(3, 4);
            if (h2 !== '') h2 = '0' + h2;
            if (h3 !== '') h3 = '0' + h3;
        }
        if (String($kok.data('doviz')) !== '1') return h1 + h2 + h3;
        var pb = deger('#ddlParaBirimi'), ek = '';
        if (pb === 'USD') ek = '01';
        else if (pb === 'KGS') ek = '02';
        else if (pb === 'TRY') ek = '03';
        else if (pb === 'EUR') ek = '04';
        return h1 + h2 + ek;
    }

    function acilisAc(tur, r) {
        if (!kayitGerekli()) return;
        acilis = { tur: tur, daKod: tur === 2 ? (r.KOD || '') : '' };
        $('#dlgAcilisBaslik').text(tur === 2 ? 'Değer Artışı Tahakkuku Hesap Kodları' : 'Açılış Tahakkuku Hesap Kodları');
        var yil, no;
        if (tur === 1) { yil = kart && kart.odemeEmriYil; no = kart ? kart.odemeEmriNo : ''; }
        else { yil = r.ODEMEEMRIYIL; no = r.ODEMEEMRINO || ''; }
        $('#txtAcYil').val(yil || new Date().getFullYear());
        $('#txtAcBelgeNo').val(no || '');
        // Açılış belgesi oluşturulmuşsa aynı belge yeniden yazılır; yıl ve numara değiştirilemez.
        var kilitli = tur === 1 && !!no;
        $('#txtAcYil, #txtAcBelgeNo').prop('readonly', kilitli).toggleClass('th-salt', kilitli);
        $('#txtAcBorc').val('92001' + ekoEk()).trigger('sgb:yazildi');
        $('#txtAcAlacak').val('921').trigger('sgb:yazildi');
        $('#dlgAcilis')[0].showModal();
    }
    $('#btnAcilis').on('click', function () { acilisAc(1, null); });

    $('#btnTahakkukOlustur').on('click', function () {
        var borc = hkBorc ? hkBorc.deger().kod || deger('#txtAcBorc') : deger('#txtAcBorc');
        var alacak = hkAlacak ? hkAlacak.deger().kod || deger('#txtAcAlacak') : deger('#txtAcAlacak');
        if (!borc || !alacak) { uyar('Borç ve alacak hesap kodlarını giriniz.'); return; }
        Sgb.mt.islem(UC + 'acilis-tahakkuk', {
            kod: kod(), tur: acilis.tur, daKod: acilis.daKod, borcHesap: borc, alacakHesap: alacak,
            oeYil: tamsayi(deger('#txtAcYil')), oeBelgeNo: deger('#txtAcBelgeNo'),
            odenekTertip: deger('#txtOdenekTertip'), parabirimiKod: deger('#ddlParaBirimi')
        }, 'Tahakkuk belgesi oluşturulacak. Onaylıyor musunuz?').done(function () {
            $('#dlgAcilis')[0].close();
            belgeYenile();
        });
    });

    // ------------------------------------------------------------------ teminat / avans / borç / ödeme emri pencereleri

    var PENCERE = {
        teminat: {
            dlg: '#dlgTeminat', grid: '#grdTeminat', secimBaslik: 'Alınan Teminat Seçimi',
            kolonlar: [
                { field: 'YIL', headerName: 'Yıl', width: 70 },
                { field: 'DOSYANO', headerName: 'Dosya No', width: 100 },
                { field: 'TARIH', headerName: 'Tarih', width: 100, valueFormatter: tarihYaz },
                { field: 'NUMARA', headerName: 'Numara', width: 150 },
                { field: 'ACIKLAMA', headerName: 'Açıklama', flex: 1, minWidth: 160, tooltipField: 'ACIKLAMA' },
                sutunTutar('TUTAR', 'Tutar', { width: 120 })
            ],
            kayit: function (r) { return { YIL: tamsayi(r.YIL), DOSYANO: String(r.DOSYANO || ''), TARIH: r.TARIH || '', NUMARA: r.NUMARA || '', ACIKLAMA: r.ACIKLAMA || '', TUTAR: Number(r.TUTAR) || 0 }; },
            silme: function (r) { return { YIL: tamsayi(r.YIL), DOSYANO: String(r.DOSYANO || '') }; }
        },
        avans: {
            dlg: '#dlgAvans', grid: '#grdAvans', secimBaslik: 'Verilen Avans Seçimi',
            kolonlar: [
                { field: 'YIL', headerName: 'Yıl', width: 70 },
                { field: 'BELGENO', headerName: 'Belge No', width: 110 },
                { field: 'TARIH', headerName: 'Tarih', width: 100, valueFormatter: tarihYaz },
                { field: 'NUMARA', headerName: 'Numara', flex: 1, minWidth: 140 },
                sutunTutar('VERILENAVANS', 'Verilen Avans', { width: 140 }),
                sutunTutar('MAHSUPEDILENAVANS', 'Mahsup Edilen Avans', { width: 150 })
            ],
            kayit: function (r) { return { YIL: tamsayi(r.YIL), BELGENO: String(r.BELGENO || ''), TARIH: r.TARIH || '', NUMARA: String(r.NUMARA || ''), VERILENAVANS: Number(r.VERILENAVANS) || 0, MAHSUPEDILENAVANS: Number(r.MAHSUPEDILENAVANS) || 0 }; },
            silme: function (r) { return { YIL: tamsayi(r.YIL), BELGENO: String(r.BELGENO || '') }; }
        },
        borc: {
            dlg: '#dlgBorc', grid: '#grdBorc', secimBaslik: 'Borç ve Temlik Seçimi',
            kolonlar: [
                { field: 'ALACAKLIADSOYAD', headerName: 'Alacaklı', flex: 1, minWidth: 160 },
                { field: 'DOSYANO', headerName: 'Dosya No', width: 100 },
                sutunTutar('TEMLIKTUTARI', 'Alacak Miktarı', { width: 130 })
            ],
            secimKolonlar: [
                { field: 'KONU', headerName: 'Konu', flex: 1, minWidth: 160, tooltipField: 'KONU' },
                { field: 'ALACAKLIADSOYAD', headerName: 'Alacaklı', width: 160 },
                { field: 'TEMLIKACANVERGINO', headerName: 'Temlik Açan V.No', width: 125 },
                { field: 'TEMLIKVERENVERGINO', headerName: 'Temlik Veren V.No', width: 130 },
                sutunTutar('TEMLIKTUTARI', 'Temlik Tutarı', { width: 120 }),
                { field: 'NOTERONAYTARIHI', headerName: 'Noter Onay Tarihi', width: 125, valueFormatter: tarihYaz },
                { field: 'NOTERONAYNO', headerName: 'Noter Onay No', width: 115 }
            ],
            kayit: function (r) { return { KOD: tamsayi(r.KOD), ALACAKLIADSOYAD: r.ALACAKLIADSOYAD || '', TEMLIKACANVERGINO: r.TEMLIKACANVERGINO || '', TEMLIKVERENVERGINO: r.TEMLIKVERENVERGINO || '', TEMLIKTUTARI: Number(r.TEMLIKTUTARI) || 0 }; },
            silme: function (r) { return { KOD: tamsayi(r.KOD) }; }
        }
    };

    $.each(PENCERE, function (ad, p) {
        p.$dlg = $(p.dlg);
        Sgb.mt.pencere(p.dlg);
        p.tablo = Sgb.grid(p.grid, {
            columnDefs: p.kolonlar,
            rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
            overlayNoRowsTemplate: '<span>Kayıt yok.</span>',
            onSelectionChanged: function () { p.$dlg.find('[data-cikar]').prop('disabled', !p.tablo.getSelectedRows().length); }
        });
    });

    function pencereListele(ad) {
        var p = PENCERE[ad];
        p.$dlg.find('[data-cikar]').prop('disabled', true);
        return Sgb.mt.oku(UC + ad + '/liste', { kod: kod() }, p.tablo).done(function (v) { p.tablo.setGridOption('rowData', v.satirlar || []); });
    }

    function pencereAc(ad) {
        if (ad === 'odemeemri') { odemeEmriPencere(); return; }
        var p = PENCERE[ad];
        p.tablo.setGridOption('rowData', []);
        p.$dlg[0].showModal();
        pencereListele(ad);
    }

    // Eski btnTeminatCikar_Click / btnAvansCikar_Click / btnBorcCikar_Click.
    $(document).on('click', '.th-pencere [data-cikar]', function () {
        var ad = $(this).attr('data-cikar'), p = PENCERE[ad];
        var s = p.tablo.getSelectedRows();
        if (!s.length) return;
        Sgb.mt.islem(UC + ad + '/sil', { kod: kod(), satirlar: $.map(s, p.silme) }, 'Seçili kayıt(lar) listeden silinecektir. Onaylıyor musunuz?')
            .done(function () { pencereListele(ad); });
    });

    // Seçim penceresi (eski TaahhutSecimAlinanTeminatlar / TaahhutSecimVerilenAvanslar / TaahhutSecimBorclarVeTemlikler).
    var secimAd = null;
    Sgb.mt.pencere('#dlgSecim');
    var grdSecim = Sgb.grid('#grdSecim', {
        columnDefs: [],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
        overlayNoRowsTemplate: '<span>Kayıt bulunamadı.</span>'
    });

    $(document).on('click', '.th-pencere [data-secim]', function () {
        secimAd = $(this).attr('data-secim');
        var p = PENCERE[secimAd];
        $('#dlgSecimBaslik').text(p.secimBaslik);
        grdSecim.setGridOption('columnDefs', p.secimKolonlar || p.kolonlar);
        grdSecim.setGridOption('rowData', []);
        $('#dlgSecim')[0].showModal();
        Sgb.mt.oku(UC + secimAd + '/secim', { kod: kod() }, grdSecim).done(function (v) { grdSecim.setGridOption('rowData', v.satirlar || []); });
    });

    $('#btnSecimKaydet').on('click', function () {
        var p = PENCERE[secimAd];
        var s = grdSecim.getSelectedRows();
        if (!s.length) { uyar('Listeden ilgili kaydı seçip sonra kaydet düğmesine basınız.'); return; }
        var ad = secimAd;
        Sgb.mt.islem(UC + ad + '/kaydet', { kod: kod(), satirlar: $.map(s, p.kayit) }).done(function () {
            $('#dlgSecim')[0].close();
            pencereListele(ad);
        });
    });

    // Ödeme emri penceresi (eski wndOdemeEmri): hakediş ödeme emirleri; Seç / Yeni Ödeme Emri alt sayfayı açar.
    Sgb.mt.pencere('#dlgOdemeEmri');
    var grdOE = Sgb.grid('#grdOdemeEmri', {
        columnDefs: [
            {
                headerName: 'Seç', width: 60, sortable: false, pinned: 'left',
                cellRenderer: function (p) { return simgeTus('glyphicon-edit', 'Seç', function () { odemeEmriKayitAc(p.data.KOD); }); }
            },
            { field: 'ODEMEEMRITARIHI', headerName: 'Tarihi', width: 95, valueFormatter: tarihYaz, pinned: 'left' },
            {
                field: 'ODEMEEMRIKOD', headerName: 'Numarası', width: 100, pinned: 'left',
                cellRenderer: function (p) { return p.value ? baglanti(p.value, function () { odemeEmriAc(p.data.ODEMEEMRIYIL, p.value); }) : ''; }
            },
            { field: 'ACIKLAMA', headerName: 'Açıklama', width: 200, tooltipField: 'ACIKLAMA' },
            { field: 'HAKEDISHESAP', headerName: 'Hakediş Hesap Kodu', width: 140 },
            { field: 'ODEMEHESAP', headerName: 'Ödeme Hesap Kodu', width: 140 },
            { field: 'YANSITMAHESAP', headerName: 'Yansıtma Hesap Kodu', width: 140 },
            sutunTutar('HAKEDISTUTARI', 'Hakediş Tutarı'),
            sutunTutar('KDV', 'KDV Tutarı'),
            sutunTutar('DAMGAVERGISI', 'Damga Vergisi'),
            sutunTutar('GELIRVERGISISTOPAJI', 'Gelir Vergisi Stopajı'),
            sutunTutar('FONPAYI', 'Fon Payı'),
            sutunTutar('PARACEZALARI', 'Para Cezaları'),
            sutunTutar('MAHSUPEDILENAVANS', 'Mahsup Edilen Avans'),
            sutunTutar('ICRAKESINTISI', 'İcra Kesintisi'),
            sutunTutar('TEMINATKESINTISI', 'Teminat Kesintisi'),
            sutunTutar('SSKPRIMIVEGECIKMEZAMMI', 'SSK Primi ve Gecikme Zammı', { width: 130 }),
            sutunTutar('VERGIBORCU', 'Vergi Borcu'),
            sutunTutar('KDVTEVKIFATI', 'KDV Tevkifatı'),
            sutunTutar('TEMLIK', 'Temlik'),
            sutunTutar('BUTCEEMANETI', 'Bütçe Emaneti'),
            sutunTutar('KESINTILERTOPLAMI', 'Kesintiler Toplamı', { width: 125 }),
            sutunTutar('YAPILANODEME', 'Yapılan Ödeme', { width: 125 })
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        overlayNoRowsTemplate: '<span>Ödeme emri yok.</span>',
        onSelectionChanged: function () { $('#btnOdemeEmriSil').prop('disabled', !grdOE.getSelectedRows().length || (kart && (kart.kapandi === 1 || kart.kapandi === 9))); }
    });

    function odemeEmriListele() {
        $('#btnOdemeEmriSil').prop('disabled', true);
        return Sgb.mt.oku(UC + 'odemeemri/liste', { kod: kod() }, grdOE).done(function (v) { grdOE.setGridOption('rowData', v.satirlar || []); });
    }
    // Alt sayfa kapanırken eski parent.OdemeEmriListeYenile çağrısı.
    window.OdemeEmriListeYenile = function () { if ($('#dlgOdemeEmri')[0].open) odemeEmriListele(); };

    function odemeEmriPencere() {
        grdOE.setGridOption('rowData', []);
        $('#dlgOdemeEmri')[0].showModal();
        odemeEmriListele();
    }

    function odemeEmriKayitAc(oeKod) {
        var p = { taahhutKod: kod() };
        if (oeKod) p.kod = oeKod;
        Sgb.pencere('TaahhutOdemeEmriKayit?' + $.param(p), 'Ödeme Emri Kayıt Sayfası', function () { window.OdemeEmriListeYenile(); });
    }
    $('#btnYeniOdemeEmri').on('click', function () { odemeEmriKayitAc(0); });

    $('#btnOdemeEmriSil').on('click', function () {
        var s = grdOE.getSelectedRows();
        if (!s.length) { uyar('Lütfen silmek istediğiniz bilgiyi seçin.'); return; }
        Sgb.mt.islem(UC + 'odemeemri/sil', { kodlar: $.map(s, function (r) { return r.KOD; }) }, 'Seçili kayıt(lar) silinecektir. Onaylıyor musunuz?')
            .done(odemeEmriListele);
    });

    $.each(['#dlgAcilis', '#dlgDegerArtisi', '#dlgSozParam', '#dlgDegParam'], function (i, s) { Sgb.mt.pencere(s); });

    // ------------------------------------------------------------------ açılış

    Sgb.mt.oku(UC + 'tanim', { tur: TUR }).done(function (v) {
        tanim = v;
        secenekDoldur($('#srIs'), v.isler, 'Hepsi', '');
        secenekDoldur($('#ddlIs'), v.isler, '', '0');
        secenekDoldur($('#srAlimUsulu'), v.alimUsulleri, 'Hepsi', '');
        secenekDoldur($('#ddlAlimUsulu'), v.alimUsulleri, '', '');
        secenekDoldur($('#ddlParaBirimi'), v.paraBirimleri, '', '');
        // Belge Arama'da Yıl boş gelir (kullanıcı isteği 2026-10-19); boşsa tüm yıllar listelenir.
        Sgb.yilKutusu('#txtYil', [], v.donemYil);
        $('#srYil, #txtYil, #txtAcYil').attr({ min: 1980, max: v.yil + 1 });
        if (PER) $('#srTarih').val(v.bugun);
        temizle();
        var acKod = tamsayi(new URLSearchParams(window.location.search).get('kod'));
        if (acKod > 0) belgeAc(acKod);
        listele(false);
    });
});
