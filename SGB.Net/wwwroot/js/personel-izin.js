// Personel İşlemleri "İzin/Görev" sekmesi (eski InsanKaynaklari/PersonelIzin.aspx; uçlar /api/personelbilgi/izin/...).
// Eski sayfadaki gibi: dönem yılı değişince izinler ve izin süre bilgileri okunur; listeden seçilen izin forma gelir,
// Kaydet / Sil onay sorar, Yeni Kayıt formu boşaltır, Yazdır seçili iznin izin formunu (PDF) indirir.
// Vekalet Eden / Teklif Eden / Uygun Gören eski ListePersonelSec penceresinin karşılığı olan sayfa içi pencereden,
// Yer eski KodAgac (kodTur=20) penceresinin karşılığı olan ağaçtan (Sgb.agac) seçilir. Belgeler kayda eklenir.
$(function () {
    'use strict';

    var $kok = $('#pbIzin');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var API = 'api/personelbilgi/izin/';
    var personelKod = parseInt($kok.data('personelKod'), 10) || 0;
    var tcKimlikNo = String($kok.data('tcKimlikNo') || '');
    var imzaTip = 0; // eski imzaTip: 1 Uygun Gören, 2 Teklif Eden, 3 Vekalet Eden

    // ------------------------------------------------------------------ yardımcılar

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnSil, #btnTemizle, #btnYazdir, #btnBelgeEkle').prop('disabled', evet); }
    function sayi(x) { return parseInt(x, 10) || 0; }
    function kod() { return sayi($('#hdnKod').val()); }
    function yil() { return sayi($('#txtDonemYil').val()); }

    // Pencereler: [data-kapat] düğmeleri kapatır.
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // Onay penceresi (confirm yerine)
    var onayIslem = null;
    function onay(metin, evet) {
        $('#dlgOnayMetin').text(metin);
        onayIslem = evet;
        $('#dlgOnay')[0].showModal();
        $('#btnOnayEvet').trigger('focus');
    }
    $('#btnOnayEvet').on('click', function () {
        var f = onayIslem;
        onayIslem = null;
        $('#dlgOnay')[0].close();
        if (f) f();
    });

    function komutDugmesi(simge, ipucu, tehlike, tikla) {
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'grid-komut' + (tehlike ? ' tehlike' : '');
        b.title = ipucu;
        b.setAttribute('aria-label', ipucu);
        b.innerHTML = '<span class="glyphicon ' + simge + '" aria-hidden="true"></span>';
        b.addEventListener('click', function (e) { e.stopPropagation(); tikla(); });
        return b;
    }

    // ------------------------------------------------------------------ tablolar

    var grdListe = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'izinTurAd', headerName: 'İzin Türü', flex: 1, minWidth: 120, tooltipField: 'izinTurAd' },
            { field: 'baslangicTarihi', headerName: 'Başlangıç Tarihi', width: 115, sort: 'desc', valueFormatter: function (p) { return Sgb.tarih(p.value); } },
            { field: 'bitisTarihi', headerName: 'Dönüş Tarihi', width: 105, valueFormatter: function (p) { return Sgb.tarih(p.value); } },
            { field: 'kullanilanGun', headerName: 'İzin Süresi', width: 90, type: 'numericColumn' },
            { field: 'onayAd', headerName: 'Onay', width: 60, hide: true }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { satirSecildi(e.data); },
        overlayNoRowsTemplate: '<span class="text-muted">İzin kaydı yok</span>'
    });

    var sayiSutun = function (alan, baslik) {
        return { field: alan, headerName: baslik, width: 100, type: 'numericColumn', wrapHeaderText: true, autoHeaderHeight: true };
    };
    var grdIzinSureler = Sgb.grid('#grdIzinSureler', {
        columnDefs: [
            { field: 'TUR', headerName: 'İzin Türü', flex: 1, minWidth: 130, tooltipField: 'TUR' },
            sayiSutun('DEVREDEN', 'Devreden İzin'),
            sayiSutun('YILICI', 'Aktif Yıl İçi İzin'),
            sayiSutun('TOPLAM', 'Toplam İzin'),
            sayiSutun('KULLANDIGI', 'Önceki Yıldan Kullandığı İzin'),
            sayiSutun('AKTIFKULLANDIGI', 'Aktif Yıldan Kullandığı İzin'),
            sayiSutun('KALAN', 'Kalan İzin')
        ],
        overlayNoRowsTemplate: '<span class="text-muted">Süre bilgisi yok</span>'
    });

    var grdBelgeler = Sgb.grid('#grdBelgeler', {
        columnDefs: [
            { headerName: '#', width: 50, sortable: false, valueGetter: function (p) { return p.node.rowIndex + 1; } },
            { field: 'BELGEADI', headerName: 'Belgeler', flex: 1, minWidth: 150, tooltipField: 'BELGEADI' },
            {
                headerName: '', width: 44, sortable: false, resizable: false,
                cellRenderer: function (p) { return komutDugmesi('glyphicon-zoom-in', 'Belgeyi Görüntüle', false, function () { belgeGoster(p.data); }); }
            },
            {
                headerName: '', width: 44, sortable: false, resizable: false,
                cellRenderer: function (p) { return komutDugmesi('glyphicon-trash', 'Belgeyi Sil', true, function () { belgeSil(p.data); }); }
            }
        ],
        overlayNoRowsTemplate: '<span class="text-muted">Belge yok</span>'
    });

    // ------------------------------------------------------------------ açılış (eski Page_Load)

    function tanimOku() {
        return Sgb.al(API + 'tanim', { personelKod: personelKod })
            .done(function (v) {
                var $t = $('#ddlIzinTip').empty();
                $('<option value=""></option>').appendTo($t);
                $.each(v.izinTurleri || [], function (i, t) { $('<option></option>').val(String(t.kod)).text(t.ad).appendTo($t); });

                var $l = $('#lstTelefon').empty();
                $.each(v.telefonlar || [], function (i, t) {
                    if (t.NUMARA) $('<option></option>').val(t.NUMARA).attr('label', t.TIP + ': ' + t.NUMARA).text(t.TIP).appendTo($l);
                });
                $('#ddlTelefon').val(v.telefon || '');
                if (v.izinYeri) $('#txtIzinYeri').val(v.izinYeri);

                // Manas kullanılıyorsa Teklif Eden ve Uygun Gören gösterilmez.
                if (v.manas) $('#grpTeklifEden, #grpImzaAtan').prop('hidden', true);

                Sgb.yilKutusu('#txtDonemYil', [v.yilMin, v.yilMaks], v.donemYil);
                $('#lblDonemAd').text(v.donemAd || '');
                listele(true);
            })
            .fail(hataGoster);
    }

    // Eski ctlIzinDonem_Degisti -> Listele(yil) + IzinSureBilgiHesapla(yil)
    function listele(ilk) {
        if (personelKod <= 0) return $.Deferred().resolve().promise();
        grdListe.setGridOption('loading', true);
        if (!ilk) grdIzinSureler.setGridOption('loading', true);
        return Sgb.al(API + 'liste', { personelKod: personelKod, yil: yil() })
            .done(function (v) {
                if (v.donemAd) $('#lblDonemAd').text(v.donemAd);
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grdListe.setGridOption('rowData', v.satirlar || []);
                grdIzinSureler.setGridOption('rowData', v.izinSureler || []);
                $('#lblSureBaslik').text(v.sureBaslik || 'İzin Süre Bilgileri');
                var k = kod();
                if (k > 0) {
                    var n = grdListe.getRowNode(String(k));
                    if (n) n.setSelected(true);
                }
            })
            .fail(hataGoster)
            .always(function () {
                grdListe.setGridOption('loading', false);
                grdIzinSureler.setGridOption('loading', false);
            });
    }

    $('#txtDonemYil').on('change', function () {
        var $y = $(this), y = yil(), en = sayi($y.attr('min')), ust = sayi($y.attr('max'));
        if (en && y < en) $y.val(en);
        if (ust && y > ust) $y.val(ust);
        listele();
    });

    // ------------------------------------------------------------------ form

    // Eski IzinTipDegisti
    function izinTipDegisti() {
        var t = $('#ddlIzinTip').val();
        if (t === '6' || t === '15') {
            $('.izin-saglik').prop('hidden', false);
            $('#grpYatiliGun').prop('hidden', false);
            $('#lblYatiliGun').text('Yatılı Gün');
        } else if (t === '11' || t === '12' || t === '13' || t === '14') {
            $('.izin-saglik').prop('hidden', true);
            $('#grpYatiliGun').prop('hidden', false);
            $('#lblYatiliGun').text('Yevmiye Gün');
        } else {
            $('.izin-saglik').prop('hidden', true);
            $('#grpYatiliGun').prop('hidden', true);
        }
    }
    $('#ddlIzinTip').on('change', function () { $(this).removeClass('is-invalid'); izinTipDegisti(); });

    // Eski Vtype="daterange": başlangıç dönüşten sonra, dönüş başlangıçtan önce seçilemez.
    function tarihAraligi() {
        var b = $('#txtBaslama').val(), s = $('#txtBitis').val();
        $('#txtBitis').attr('min', b || null);
        $('#txtBaslama').attr('max', s || null);
    }

    // Eski GunAl (başlangıç ya da dönüş değişince)
    function gunAl() {
        var b = $('#txtBaslama').val(), s = $('#txtBitis').val();
        if (!b || !s) return;
        Sgb.al(API + 'gun', { izinTurKod: sayi($('#ddlIzinTip').val()), baslangic: b, bitis: s })
            .done(function (v) { if (v.gun !== null && v.gun !== undefined) $('#txtIzinGun').val(v.gun); })
            .fail(hataGoster);
    }
    $('#txtBaslama, #txtBitis').on('change', function () { $(this).removeClass('is-invalid'); tarihAraligi(); gunAl(); });

    // Eski satirSecildi (loadRecord) + SatirSecildi (belgeler)
    function satirSecildi(r) {
        if (!r) return;
        $('#hdnKod').val(r.kod);
        $('#ddlIzinTip').val(String(r.izinTurKod || ''));
        if ($('#ddlIzinTip').val() === null) $('#ddlIzinTip').val('');
        $('#txtBaslama').val(r.baslangicTarihi || '');
        $('#txtBitis').val(r.bitisTarihi || '');
        $('#txtIzinGun').val(r.kullanilanGun);
        $('#txtIzinYeri').val(r.izinYeri || '');
        $('#ddlTelefon').val(r.telefon || '');
        $('#txtBelgeNo').val(r.belgeNo || '');
        $('#txtBelgeTarihi').val(r.belgeTarihi || '');
        $('#hdnMemuriyeti').val(r.memuriyeti || '');
        $('#hdnYerineGorevli').val(r.yerineGorevli || '');
        $('#txtYerineGorevli').val(r.yerineGorevliAd || '');
        $('#hdnTeklifEden').val(r.teklifEden || '');
        $('#txtTeklifEden').val(r.teklifEdenAd || '');
        $('#hdnImzaAtan').val(r.imzalayan || '');
        $('#txtImzaAtan').val(r.imzalayanAd || '');
        $('#txtAciklama').val(r.aciklama || '');
        $('#txtSaglikKurulusu').val(r.saglikKurulusu || '');
        $('#dfRaporTarihi').val(r.raporTarihi || '');
        $('#txtRaporNo').val(r.raporNo || '');
        $('#nfYatiliGecenSure').val(r.yatiliGecenSure || '');
        $('#hdnYerKod').val(r.yerKod || '');
        $('#txtYerAd').val(r.yerAd || '');
        $('#txtIzinNotu2').val(r.izinNotu2 || '');
        $('#frmIzin .is-invalid').removeClass('is-invalid');
        tarihAraligi();
        izinTipDegisti();
        belgeListele();
    }

    // Eski Temizle + btnTemizle_Click
    function temizle() {
        $('#hdnKod').val('');
        $('#ddlIzinTip').val('');
        $('#txtBaslama, #txtBitis').val('').removeAttr('min max'); // eski Genel.TarihAlanTemizle
        $('#txtIzinYeri, #txtAciklama').val('');
        $('#hdnYerineGorevli, #txtYerineGorevli, #hdnTeklifEden, #txtTeklifEden, #hdnImzaAtan, #txtImzaAtan').val('');
        $('#txtIzinGun, #txtBelgeNo, #txtBelgeTarihi').val('');
        $('#txtSaglikKurulusu, #txtRaporNo, #dfRaporTarihi, #nfYatiliGecenSure').val('');
        $('#hdnYerKod, #txtYerAd, #txtIzinNotu2').val('');
        $('#frmIzin .is-invalid').removeClass('is-invalid');
        grdBelgeler.setGridOption('rowData', []);
        grdListe.deselectAll();
        izinTipDegisti();
    }

    function formOku() {
        return {
            personelKod: personelKod,
            tcKimlikNo: tcKimlikNo,
            kod: kod(),
            izinTurKod: sayi($('#ddlIzinTip').val()),
            baslangicTarihi: $('#txtBaslama').val(),
            bitisTarihi: $('#txtBitis').val(),
            izinYeri: $('#txtIzinYeri').val(),
            aciklama: $('#txtAciklama').val(),
            telefon: $('#ddlTelefon').val(),
            belgeNo: $('#txtBelgeNo').val(),
            belgeTarihi: $('#txtBelgeTarihi').val(),
            memuriyeti: $('#hdnMemuriyeti').val(),
            saglikKurulusu: $('#txtSaglikKurulusu').val(),
            raporTarihi: $('#dfRaporTarihi').val(),
            raporNo: $('#txtRaporNo').val(),
            yatiliGecenSure: sayi($('#nfYatiliGecenSure').val()),
            yerKod: $('#hdnYerKod').val(),
            imzalayan: sayi($('#hdnImzaAtan').val()),
            teklifEden: sayi($('#hdnTeklifEden').val()),
            yerineGorevli: sayi($('#hdnYerineGorevli').val()),
            izinNotu2: $('#txtIzinNotu2').val()
        };
    }

    // Eski ZorunluAlanKontrol (AllowBlank="false": Türü, Başlangıcı, Dönüşü)
    function zorunluAlanKontrol() {
        var gecerli = true;
        $('#ddlIzinTip, #txtBaslama, #txtBitis').each(function () {
            var bos = !$(this).val();
            $(this).toggleClass('is-invalid', bos);
            if (bos) gecerli = false;
        });
        var b = $('#txtBaslama').val(), s = $('#txtBitis').val();
        if (b && s && b > s) { $('#txtBaslama, #txtBitis').addClass('is-invalid'); gecerli = false; }
        if (!gecerli) mesaj('Kırmızı renk ile işaretlenen alanları boş bırakamazsınız.', 'warning');
        return gecerli;
    }

    // Eski btnKaydet_Click (onaydan sonra). sonra(kod): başarılı kayıttan sonra (belge ekleme).
    function kaydetIstek(sonra) {
        mesgul(true);
        return Sgb.gonder(API + 'kaydet', formOku())
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                $('#hdnKod').val(v.kod || '');
                mesaj(v.bilgi, 'success');
                listele();
                if (sonra) sonra(v.kod);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    }

    $('#btnKaydet').on('click', function () {
        if (personelKod <= 0) { mesaj('Personel bilgisi boş bırakılamaz.', 'warning'); return; }
        if (!zorunluAlanKontrol()) return;
        onay('Kayıt işlemi yapılacak. Onaylıyor musunuz?', function () { kaydetIstek(); });
    });

    // Eski btnSil_Click
    $('#btnSil').on('click', function () {
        if (personelKod <= 0 || kod() <= 0) { mesaj('Lütfen silmek istediğiniz İzin bilgisini seçin.', 'warning'); return; }
        onay('Kayıt silinecek. Onaylıyor musunuz?', function () {
            mesgul(true);
            Sgb.gonder(API + 'sil', { personelKod: personelKod, kod: kod() })
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                    mesaj(v.bilgi, 'success');
                    temizle();
                    listele();
                })
                .fail(hataGoster)
                .always(function () { mesgul(false); });
        });
    });

    $('#btnTemizle').on('click', temizle);

    // Eski btnYazdir_Click: önce yetki ve imza güncelleme (POST), sonra izin formu (PDF) indirilir.
    $('#btnYazdir').on('click', function () {
        mesgul(true);
        Sgb.gonder(API + 'imza', {
            personelKod: personelKod,
            kod: kod(),
            teklifEden: sayi($('#hdnTeklifEden').val()),
            imzalayan: sayi($('#hdnImzaAtan').val()),
            imzalayanAd: $('#txtImzaAtan').val()
        })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); mesgul(false); return; }
                Sgb.indir(API + 'yazdir', { personelKod: personelKod, kod: kod(), yil: yil() })
                    .then(function () { mesgul(false); }, function () { mesgul(false); });
            })
            .fail(function (xhr) { hataGoster(xhr); mesgul(false); });
    });

    // ------------------------------------------------------------------ belgeler

    function belgeListele() {
        var k = kod();
        if (k <= 0 || personelKod <= 0) { grdBelgeler.setGridOption('rowData', []); return; }
        grdBelgeler.setGridOption('loading', true);
        Sgb.al(API + 'belge/liste', { personelKod: personelKod, kayitKod: k })
            .done(function (v) {
                if (v.hata) mesaj(v.hata, 'danger');
                grdBelgeler.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { grdBelgeler.setGridOption('loading', false); });
    }

    function belgeYukle(dosya) {
        var veri = new FormData();
        veri.append('personelKod', personelKod);
        veri.append('kayitKod', kod());
        veri.append('dosya', dosya);
        mesgul(true);
        $.ajax({
            url: Sgb.KOK + API + 'belge/yukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (!v.basarili) mesaj(v.hata, 'danger');
            else if (v.bilgi) mesaj(v.bilgi, 'success');
            belgeListele();
        }).fail(function (xhr) {
            if (xhr.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            hataGoster(xhr);
        }).always(function () { mesgul(false); });
    }

    $('#btnBelgeEkle').on('click', function () {
        if (personelKod <= 0) { mesaj('Personel bilgisi boş bırakılamaz.', 'warning'); return; }
        $('#fileBelge').val('').trigger('click');
    });

    // Eski BelgeKaydet: kayıt seçili değilse önce kaydedilir.
    $('#fileBelge').on('change', function () {
        var dosya = this.files && this.files[0];
        if (!dosya) return;
        if (kod() > 0) { belgeYukle(dosya); return; }
        if (!zorunluAlanKontrol()) return;
        kaydetIstek(function (yeniKod) { if (yeniKod > 0) belgeYukle(dosya); });
    });

    function belgeGoster(b) {
        Sgb.indir(API + 'belge/goster', { personelKod: personelKod, kayitKod: kod(), kod: b.KOD });
    }

    function belgeSil(b) {
        onay('Ekli doküman silinecek. Onaylıyor musunuz?', function () {
            Sgb.gonder(API + 'belge/sil', { personelKod: personelKod, kayitKod: kod(), kod: b.KOD })
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                    belgeListele();
                })
                .fail(hataGoster);
        });
    }

    // ------------------------------------------------------------------ personel seçimi (eski ListePersonelSec.aspx)

    // Eski PersonelBilgiYaz
    function personelBilgiYaz(kodu, ad) {
        if (imzaTip === 1) { $('#txtImzaAtan').val(ad); $('#hdnImzaAtan').val(kodu); }
        else if (imzaTip === 2) { $('#txtTeklifEden').val(ad); $('#hdnTeklifEden').val(kodu); }
        else if (imzaTip === 3) { $('#txtYerineGorevli').val(ad); $('#hdnYerineGorevli').val(kodu); }
    }

    // Eski personelEkle: ortak personel seçimi (Sgb.personelSec, eski ListePersonelSec.aspx) seçilen personeli alana yazar.
    $('[data-personel-sec]').on('click', function () {
        imzaTip = sayi($(this).data('personelSec'));
        Sgb.personelSec({
            secilince: function (satirlar) {
                var secili = satirlar[0];
                personelBilgiYaz(secili.kod, (secili.AD || '') + ' ' + (secili.SOYAD || ''));
            }
        });
    });

    $('[data-personel-temizle]').on('click', function () {
        imzaTip = sayi($(this).data('personelTemizle'));
        personelBilgiYaz('', '');
    });

    // ------------------------------------------------------------------ yer seçimi (eski KodAgac.aspx?kodTur=20)

    // Eski YerYaz / KodSecildi
    function yerYaz(kodu, ad) {
        $('#txtYerAd').val(ad);
        $('#hdnYerKod').val(kodu);
    }

    var yerAgac = null;
    $('#btnYerSec').on('click', function () {
        if (!yerAgac) {
            yerAgac = Sgb.agac('#agcYer', {
                yukle: function (ust) {
                    return Sgb.al(API + 'yerler', { ust: ust ? ust.kod : '', seviye: ust ? (sayi(ust.seviye) + 1) : 1 })
                        .then(function (v) {
                            if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                            return v.dugumler || [];
                        });
                },
                secilince: function (d) {
                    yerYaz(d.kod, d.ad);
                    $('#dlgYer')[0].close();
                },
                seciliMi: function (d) { return d.kod === $('#hdnYerKod').val(); },
                bosMetin: 'Kod bulunamadı.',
                hata: hataGoster
            });
        }
        $('#dlgYer')[0].showModal();
    });
    $('#btnYerTemizle').on('click', function () { yerYaz('', ''); });

    // ------------------------------------------------------------------ başla

    izinTipDegisti();
    tanimOku();
});
