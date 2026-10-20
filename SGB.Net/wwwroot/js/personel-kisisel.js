// Personel İşlemleri "Kişisel Bilgiler" sekmesi (eski InsanKaynaklari/PersonelKisisel.aspx; uçlar /api/personelbilgi/kisisel/...).
// Eski sayfadaki gibi:
//   - personelKod 0 ise yeni personel girişi: T.C. kimlik no yazılıp Enter / sorgula ile nüfus bilgileri (KPS / MERNİS) gelir;
//     Kaydet önce bilgileri denetler (eski AlanlariKontrolEt), sonra "Yeni Personel Birim/Unvan/Tür Girişi" penceresi açılır,
//     orada Kaydet ile kayıt yapılır ve ana sayfa yeni personeli gösterir (PersonelBilgi.kayittanSonraGoster).
//   - personelKod > 0 ise bilgiler okunur; T.C. kimlik no değiştirilemez (ayrı "TC Kimlik No Değiştir" penceresi).
//   - Mernis bağlantısı varsa nüfus alanları salt okunurdur; Manas'ta kimlik sorgusu yoktur ve bazı alanlar gizlenir.
//   - Yeni Kayıt ana sayfadaki yeni kayıt hazırlığını çağırır; resim ve belgeler kayıtlı personele eklenir.
//   - Yazdırma düğmeleri dosyayı Sgb.indir ile indirir (nüfus ve kişi borcu raporları ortak /api/personelbilgi/rapor/ uçlarından).
$(function () {
    'use strict';

    var $kok = $('#pbKisisel');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var API = 'api/personelbilgi/kisisel/';
    var RAPOR = 'api/personelbilgi/rapor/';
    var personelKod = parseInt($kok.data('personelKod'), 10) || 0;
    var ERKEK = 2; // TNS.IKY.ENUMCINSIYET.ERKEK
    var tanim = { manas: false, mernisKilit: false, kvkk: false, varsayilanPersonelTip: 100 };

    // ------------------------------------------------------------------ yardımcılar

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function sayi(x) { return parseInt(x, 10) || 0; }
    function mesgul(evet) {
        $('#btnKaydet, #btnTemizle, #btnResimEkle, #btnBelgeEkle, #btnKimlikSorgula, #btnYeniPersonelKaydet, #btnTcDegistirKaydet').prop('disabled', evet);
    }
    function ustSayfa() { return window.parent && window.parent !== window && window.parent.PersonelBilgi ? window.parent.PersonelBilgi : null; }

    function secenekDoldur($s, liste, bosEkle) {
        $s.empty();
        if (bosEkle) $('<option value=""></option>').appendTo($s);
        $.each(liste || [], function (i, x) { $('<option></option>').val(String(x.kod)).text(x.ad).appendTo($s); });
    }

    // Seçim kutusuna değer yazar; listede yoksa noktasız hâli denenir, o da yoksa (eski kayıtlardaki değer kaybolmasın) eklenir.
    function secimYaz($s, deger) {
        var d = deger == null ? '' : String(deger);
        if (d === '' || $s.find('option').filter(function () { return this.value === d; }).length) { $s.val(d); return; }
        var noktasiz = d.replace(/\./g, '');
        if ($s.find('option').filter(function () { return this.value === noktasiz; }).length) { $s.val(noktasiz); return; }
        $('<option></option>').val(d).text(d).appendTo($s);
        $s.val(d);
    }

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

    // ------------------------------------------------------------------ form <-> nesne

    function alanYaz(el, deger) {
        var $e = $(el);
        if (el.type === 'checkbox') el.checked = !!deger;
        else if (el.tagName === 'SELECT') secimYaz($e, deger);
        else $e.val(deger == null ? '' : String(deger));
    }

    function formOku() {
        var v = { kod: personelKod };
        $('#frmKisisel [data-alan]').each(function () {
            var ad = $(this).data('alan');
            if (this.type === 'checkbox') v[ad] = this.checked;
            else if ($(this).data('tip') === 'sayi') v[ad] = sayi($(this).val());
            else v[ad] = $.trim($(this).val() || '');
        });
        return v;
    }

    function formTemizle() {
        $('#frmKisisel [data-alan], #frmKisisel [data-alan-salt], #frmKisisel [data-nufus]').each(function () {
            if (this.type === 'checkbox') this.checked = false;
            else $(this).val(this.tagName === 'SELECT' ? ($(this).find('option').first().val() || '') : '');
        });
        $('#ddlNufusIlce').empty();
    }

    function ilkSoyadGoster() {
        $('#grpIlkSoyad').prop('hidden', sayi($('#ddlCinsiyet').val()) === ERKEK);
    }

    // ------------------------------------------------------------------ il / ilçe

    function ilceDoldur(liste, secili) {
        var $s = $('#ddlNufusIlce');
        secenekDoldur($s, liste, true);
        secimYaz($s, secili || '');
    }
    $('#ddlNufusIl').on('change', function () {
        var il = $(this).val();
        if (!il) { ilceDoldur([], ''); return; }
        Sgb.al(API + 'ilceler', { ilKod: il })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                ilceDoldur(v.ilceler, '');
            })
            .fail(hataGoster);
    });
    $('#ddlCinsiyet').on('change', ilkSoyadGoster);

    // ------------------------------------------------------------------ resim

    function resimGoster() {
        var $img = $('#imgResim');
        if (personelKod <= 0) { $img.prop('hidden', true).removeAttr('src'); $('#lblResimYok').prop('hidden', false); return; }
        $img.attr('src', Sgb.KOK + 'InsanKaynaklari/ResimGoster.ashx?pKod=' + personelKod + '&kucuk=1&_=' + Date.now());
    }
    $('#imgResim')
        .on('load', function () { $(this).prop('hidden', false); $('#lblResimYok').prop('hidden', true); })
        .on('error', function () { $(this).prop('hidden', true); $('#lblResimYok').prop('hidden', false); });

    function dosyaGonder(adres, dosya, bitince) {
        var veri = new FormData();
        veri.append('personelKod', personelKod);
        veri.append('dosya', dosya);
        mesgul(true);
        $.ajax({
            url: Sgb.KOK + adres, method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
            if (v.bilgi) mesaj(v.bilgi, 'success');
            if (bitince) bitince();
        }).fail(function (xhr) {
            if (xhr.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            hataGoster(xhr);
        }).always(function () { mesgul(false); });
    }

    $('#btnResimEkle').on('click', function () {
        if (personelKod <= 0) { mesaj('Personel bilgisini kayıt ettikten sonra resim ekleme işlemini yapınız..', 'warning'); return; }
        $('#fileResim').val('').trigger('click');
    });
    $('#fileResim').on('change', function () {
        var dosya = this.files && this.files[0];
        if (!dosya) return;
        dosyaGonder(API + 'resim', dosya, function () {
            resimGoster();
            var ust = ustSayfa();
            if (ust && ust.resimYenile) ust.resimYenile();
        });
    });

    // ------------------------------------------------------------------ belgeler

    var grdBelgeler = Sgb.grid('#grdBelgeler', {
        columnDefs: [
            { headerName: '#', width: 50, sortable: false, cellClass: 'hucre-salt', valueGetter: function (p) { return p.node.rowIndex + 1; } },
            { field: 'BELGEADI', headerName: 'Belge Adı', flex: 1, minWidth: 150, cellClass: 'hucre-salt', tooltipField: 'BELGEADI' },
            {
                headerName: '', width: 44, sortable: false, resizable: false,
                cellRenderer: function (p) { return komutDugmesi('glyphicon-zoom-in', 'Belgeyi Görüntüle', false, function () { belgeGoster(p.data); }); }
            },
            {
                headerName: '', width: 44, sortable: false, resizable: false,
                cellRenderer: function (p) { return komutDugmesi('glyphicon-trash', 'Belgeyi Sil', true, function () { belgeSil(p.data); }); }
            }
        ],
        getRowId: function (p) { return String(p.data.KOD); },
        overlayNoRowsTemplate: '<span class="text-muted">Belge yok</span>'
    });

    function belgeListele() {
        if (personelKod <= 0) { grdBelgeler.setGridOption('rowData', []); return; }
        grdBelgeler.setGridOption('loading', true);
        Sgb.al(API + 'belgeler', { personelKod: personelKod })
            .done(function (v) {
                if (v.hata) mesaj(v.hata, 'danger');
                grdBelgeler.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { grdBelgeler.setGridOption('loading', false); });
    }

    function belgeGoster(b) {
        if (!b || !b.KOD) return;
        Sgb.indir(API + 'belge-indir', { personelKod: personelKod, kod: sayi(b.KOD) });
    }

    function belgeSil(b) {
        if (!b || !b.KOD) return;
        onay('Ekli doküman silinecek. Onaylıyor musunuz?', function () {
            Sgb.gonder(API + 'belge-sil', { personelKod: personelKod, kod: sayi(b.KOD) })
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                    mesaj(v.bilgi || 'Belge silindi.', 'success');
                    belgeListele();
                })
                .fail(hataGoster);
        });
    }

    $('#btnBelgeEkle').on('click', function () {
        if (personelKod <= 0) { mesaj('Personel bilgisini kayıt ettikten sonra belge ekleyiniz.', 'warning'); return; }
        $('#fileBelge').val('').trigger('click');
    });
    $('#fileBelge').on('change', function () {
        var dosya = this.files && this.files[0];
        if (dosya) dosyaGonder(API + 'belge-yukle', dosya, belgeListele);
    });

    // ------------------------------------------------------------------ açılış (eski Page_Load / Listele)

    function kilitleriUygula() {
        // Mernis bağlantısı varsa nüfus bilgileri elle değiştirilemez (pasaport no hariç).
        $('#frmKisisel [data-mernis]').each(function () {
            if (this.tagName === 'SELECT') $(this).prop('disabled', tanim.mernisKilit);
            else $(this).prop('readonly', tanim.mernisKilit);
        });
        // T.C. kimlik no kayıtlı personelde değiştirilemez; kimlik sorgusu yalnızca yeni kayıtta ve Manas dışında.
        $('#txtTcKimlikNo').prop('readonly', personelKod > 0);
        $('#btnKimlikSorgula').prop('hidden', tanim.manas || personelKod > 0);
        $('#btnTcDegistir').prop('hidden', personelKod <= 0);
        if (tanim.kvkk) $('#pnlNufus').prop('hidden', true);
    }

    function tanimOku() {
        return Sgb.al(API + 'tanim').done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            tanim = v;
            secenekDoldur($('#ddlSosyalGuvence'), v.sosyalGuvenceler, false);
            secenekDoldur($('#ddlSakatlikDurumu'), v.sakatlikDurumlari, false);
            secenekDoldur($('#ddlMedeniHali'), v.medeniHaller, false);
            secenekDoldur($('#ddlCinsiyet'), v.cinsiyetler, true);
            secenekDoldur($('#ddlNufusIl'), v.iller, true);
            secenekDoldur($('#ddlYeniUnvan'), v.unvanlar, false);
            secenekDoldur($('#ddlYeniPersonelTip'), v.personelTipleri, false);
            secimYaz($('#ddlYeniPersonelTip'), v.varsayilanPersonelTip);
            kilitleriUygula();
            ilkSoyadGoster();
        }).fail(hataGoster);
    }

    function personelOku() {
        if (personelKod <= 0) return $.Deferred().resolve().promise();
        mesgul(true);
        return Sgb.al(API + 'oku', { personelKod: personelKod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var p = v.personel;
                if (!p) return;
                ilceDoldur(v.ilceler, '');
                $('#frmKisisel [data-alan], #frmKisisel [data-alan-salt]').each(function () {
                    var ad = $(this).data('alan') || $(this).data('alanSalt');
                    if (Object.prototype.hasOwnProperty.call(p, ad)) alanYaz(this, p[ad]);
                });
                // Eski Listele: Manas'ta bu alanlar gizlenir (okunmaz da).
                if (v.manasGizle) $('#frmKisisel [data-manas-gizle]').prop('hidden', true);
                ilkSoyadGoster();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    }

    // ------------------------------------------------------------------ kimlik sorgusu (eski KimlikBilgisiGetir)

    function kimlikSorgula() {
        if (personelKod > 0 || tanim.manas) return;
        var tc = $.trim($('#txtTcKimlikNo').val());
        if (!tc) { mesaj('Personel T.C.Kimlik No bilgisi boş bırakılamaz.', 'warning'); return; }
        mesgul(true);
        Sgb.al(API + 'kimlik', { tcKimlikNo: tc })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                if (v.nufus) {
                    var n = v.nufus;
                    secimYaz($('#ddlNufusIl'), n.nufusIlKod);
                    ilceDoldur(v.ilceler, n.nufusIlceKod);
                    $('#frmKisisel [data-alan]').each(function () {
                        var ad = $(this).data('alan');
                        if (ad === 'nufusIlKod' || ad === 'nufusIlceKod') return;
                        if (Object.prototype.hasOwnProperty.call(n, ad)) alanYaz(this, n[ad]);
                    });
                    $('#frmKisisel [data-nufus]').each(function () { alanYaz(this, n[$(this).data('nufus')]); });
                } else if (v.mernis) {
                    var m = v.mernis;
                    $('#txtAd').val(m.ad || '');
                    $('#txtSoyad').val(m.soyad || '');
                    if (m.bulunamadi) { mesaj('Kimlik bilgisi bulunamadı.', 'warning'); return; }
                    $('#txtDogumTarihi').val(m.dogumTarihi || '');
                    $('#txtAnaAd').val(m.anaAd || '');
                    $('#txtBabaAd').val(m.babaAd || '');
                    secimYaz($('#ddlCinsiyet'), m.cinsiyet ? String(m.cinsiyet) : '');
                }
                ilkSoyadGoster();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    }
    $('#btnKimlikSorgula').on('click', kimlikSorgula);
    $('#txtTcKimlikNo').on('keydown', function (e) {
        if (e.key === 'Enter') { e.preventDefault(); kimlikSorgula(); }
    });

    // ------------------------------------------------------------------ kaydet (eski btnKaydet_Click / AlanlariKontrolEt)

    function kaydetIstek(veri, bitince) {
        mesgul(true);
        Sgb.gonder(API + 'kaydet', veri)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
                if (bitince) bitince(v);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    }

    $('#btnKaydet').on('click', function () {
        var veri = formOku();
        if (personelKod > 0) {
            kaydetIstek(veri, function () {
                var ust = ustSayfa();
                if (ust && ust.ozetYenile) ust.ozetYenile();
            });
            return;
        }
        // Yeni personel: önce denetim, sonra birim / unvan / tür penceresi
        mesgul(true);
        Sgb.gonder(API + 'kontrol', veri)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'warning'); return; }
                $('#dlgYeniPersonel')[0].showModal();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnYeniPersonelKaydet').on('click', function () {
        var veri = formOku();
        veri.kod = 0;
        veri.birimKod = sayi($('#hdnYeniBirim').val());
        veri.unvanKod = sayi($('#ddlYeniUnvan').val());
        veri.personelTip = sayi($('#ddlYeniPersonelTip').val());
        veri.kadroDerece = $.trim($('#txtYeniKadroDerece').val());
        veri.ekGosterge = $.trim($('#txtYeniEkGosterge').val());
        veri.atanacagiKadro = $('#grpAtanacagiKadro').prop('hidden') ? '' : ($('#ddlAtanacagiKadro').val() || '');
        kaydetIstek(veri, function (v) {
            $('#dlgYeniPersonel')[0].close();
            var ust = ustSayfa();
            if (v.yeniKod > 0 && ust && ust.kayittanSonraGoster) ust.kayittanSonraGoster(v.yeniKod);
        });
    });

    // Eski cmbPersonelTipDegisti: kadro işlemi yapılan türde birimin açık kadroları listelenir.
    function acikKadroOku() {
        Sgb.al(API + 'acikkadro', { personelTip: sayi($('#ddlYeniPersonelTip').val()), birim: sayi($('#hdnYeniBirim').val()) })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var $s = $('#ddlAtanacagiKadro').empty();
                $.each(v.kadrolar || [], function (i, k) { $('<option></option>').val(k.ID || '').text(k.Aciklama || '').appendTo($s); });
                $s.val('');
                $('#grpAtanacagiKadro').prop('hidden', !v.goster);
            })
            .fail(hataGoster);
    }
    $('#ddlYeniPersonelTip').on('change', acikKadroOku);

    // Birim seçimi (eski ExtOrgSecim)
    var birimAgac = null, birimAraZaman = null;
    function birimAgaciKur() {
        birimAgac = Sgb.agac('#agcYeniBirim', {
            yukle: function (ust) {
                var ara = $.trim($('#txtYeniBirimAra').val());
                if (ara.length < 3) ara = '';
                return Sgb.al(API + 'orgagac', { ust: ust ? sayi(ust.kod) : 0, ara: ust ? '' : ara })
                    .then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.satirlar || [];
                    });
            },
            secilince: function (d) {
                $('#hdnYeniBirim').val(d.kod);
                $('#txtYeniBirimAd').val(d.birimAd || d.ad);
                $('#pnlYeniBirim').prop('hidden', true);
                $('#btnYeniBirimAc').attr('aria-expanded', 'false');
                if (!$('#grpAtanacagiKadro').prop('hidden')) acikKadroOku();
            },
            seciliMi: function (d) { return String(d.kod) === String($('#hdnYeniBirim').val()); },
            ipucu: function (d) { return d.birimAd || d.ad; },
            bosMetin: 'Birim bulunamadı.',
            hata: hataGoster
        });
    }
    $('#btnYeniBirimAc').on('click', function () {
        var $p = $('#pnlYeniBirim'), ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false');
        if (ac && !birimAgac) birimAgaciKur();
        if (ac) $('#txtYeniBirimAra').trigger('focus');
    });
    $('#txtYeniBirimAra').on('input', function () {
        clearTimeout(birimAraZaman);
        var uzunluk = $.trim($(this).val()).length;
        if (uzunluk > 0 && uzunluk < 3) return;
        birimAraZaman = setTimeout(function () { if (birimAgac) birimAgac.yenile(); }, 600);
    });
    $('#btnYeniBirimTemizle').on('click', function () {
        $('#hdnYeniBirim, #txtYeniBirimAd').val('');
        if (!$('#grpAtanacagiKadro').prop('hidden')) acikKadroOku();
    });

    // ------------------------------------------------------------------ yeni kayıt, sicil önceki / sonraki

    $('#btnTemizle').on('click', function () {
        var ust = ustSayfa();
        if (ust && ust.yeniKayitHazirla) { ust.yeniKayitHazirla(); return; }
        // Ana sayfa dışında açıldıysa form burada boşaltılır.
        personelKod = 0;
        formTemizle();
        kilitleriUygula();
        resimGoster();
        belgeListele();
    });

    function sicilGit(yon) {
        Sgb.al(API + 'sicil', { sicilNo: $.trim($('#txtSicilNo').val()), yon: yon })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'warning'); return; }
                var ust = ustSayfa();
                if (v.kod > 0 && ust && ust.kayittanSonraGoster) ust.kayittanSonraGoster(v.kod);
            })
            .fail(hataGoster);
    }
    $('#btnSicilOnceki').on('click', function () { sicilGit('onceki'); });
    $('#btnSicilSonraki').on('click', function () { sicilGit('sonraki'); });

    // ------------------------------------------------------------------ T.C. kimlik no değiştir

    $('#btnTcDegistir').on('click', function () {
        $('#txtTcKimlikNoYeni').val('');
        $('#dlgTcDegistir')[0].showModal();
        $('#txtTcKimlikNoYeni').trigger('focus');
    });
    $('#frmTcDegistir').on('submit', function (e) {
        e.preventDefault();
        mesgul(true);
        Sgb.gonder(API + 'tckimlikno-degistir', { personelKod: personelKod, yeniTcKimlikNo: $.trim($('#txtTcKimlikNoYeni').val()) })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                $('#dlgTcDegistir')[0].close();
                $('#txtTcKimlikNo').val(v.tcKimlikNo);
                mesaj(v.bilgi, 'success');
                var ust = ustSayfa();
                if (ust && ust.ozetYenile) ust.ozetYenile();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // ------------------------------------------------------------------ yazdırma

    $('[data-rapor]').on('click', function () {
        var rapor = $(this).data('rapor');
        if (rapor === 'nufus') {
            // Kayıtlı personelde kayıtlı T.C. (uç personelKod'dan bulur), yeni girişte formdaki T.C. kullanılır.
            var p = { tur: sayi($(this).data('tur')), personelKod: personelKod };
            if (personelKod <= 0) p.tcKimlikNo = $.trim($('#txtTcKimlikNo').val());
            Sgb.indir(RAPOR + 'nufus', p);
            return;
        }
        if (personelKod <= 0) { mesaj('Lütfen kişi seçiniz', 'warning'); return; }
        if (rapor === 'kisiborcu') Sgb.indir(RAPOR + 'kisiborcu', { personelKod: personelKod });
        else Sgb.indir(API + 'rapor/' + rapor, { personelKod: personelKod });
    });

    // ------------------------------------------------------------------ başlat

    tanimOku().always(function () {
        personelOku();
    });
    resimGoster();
    belgeListele();
});
