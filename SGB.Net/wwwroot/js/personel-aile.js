// Personel İşlemleri > Aile sekmesi (InsanKaynaklari/PersonelAile; eski PersonelAile.aspx).
// Sağda aile bireyleri listesi; seçilen birey forma ve belgeleri alttaki tabloya gelir. Kaydet / Sil onay sorar.
// T.C. Kimlik No yanındaki mercek (ya da Enter) nüfus bilgilerini getirir. Nüfus Cüzdanı Örneği / İkametgah İl Muhaberi
// Kişisel sekmesinin ortak rapor ucunu aile bireyinin T.C. kimlik numarasıyla çağırır (eski NufusYazdir: txtTCKimlikNo).
$(function () {
    'use strict';

    var $kok = $('#pbAile');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var personelKod = parseInt($kok.data('personel-kod'), 10) || 0;
    var kod = 0; // eski hdnKod: seçili / kaydedilen aile bireyi

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $kok.find('.pb-arac button, #btnBelgeEkle, #btnKimlik').prop('disabled', evet); }

    // Sayfa içi onay penceresi (eski Ext Confirmation). Söz döner: evet ise çözülür.
    function onayla(metin) {
        var d = $.Deferred();
        var dlg = document.getElementById('dlgOnay');
        $('#dlgOnayMetin').text(metin);
        $(dlg).off('click.onay').on('click.onay', '[data-sonuc]', function () {
            var evet = $(this).data('sonuc') === 'evet';
            dlg.close();
            if (evet) d.resolve(); else d.reject();
        });
        $(dlg).off('cancel.onay').on('cancel.onay', function () { d.reject(); });
        dlg.showModal();
        return d.promise();
    }

    function secenekDoldur($s, liste, bosEkle) {
        $s.empty();
        if (bosEkle) $('<option></option>').val('0').text('').appendTo($s);
        $.each(liste || [], function (i, o) { $('<option></option>').val(String(o.KOD)).text(o.AD).appendTo($s); });
    }
    function secimYap($s, deger) {
        $s.val(String(deger || 0));
        if ($s.val() === null) $s.val('0');
    }

    // ---------------------------------------------------------------- tablolar

    var grdListe = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'ADSOYAD', headerName: 'Adı Soyadı', flex: 1, minWidth: 140, tooltipField: 'ADSOYAD', cellClass: 'hucre-salt' },
            { field: 'YAKINLIK', headerName: 'Yakınlık Derecesi', width: 140, cellClass: 'hucre-salt' }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.KOD); },
        onRowClicked: function (e) { satirSecildi(e.data.KOD); },
        pagination: true,
        paginationPageSize: 25,
        paginationPageSizeSelector: false,
        overlayNoRowsTemplate: '<span class="text-muted">Aile bireyi yok</span>'
    });

    function belgeDugme(simge, ipucu, islem) {
        return function (p) {
            var b = document.createElement('button');
            b.type = 'button';
            b.className = 'btn btn-link btn-xs';
            b.title = ipucu;
            b.setAttribute('aria-label', ipucu + ': ' + (p.data.BELGEADI || ''));
            b.innerHTML = '<span class="glyphicon ' + simge + '" aria-hidden="true"></span>';
            b.addEventListener('click', function (e) { e.stopPropagation(); islem(p.data); });
            return b;
        };
    }

    var grdBelgeler = Sgb.grid('#grdBelgeler', {
        columnDefs: [
            { headerName: '#', valueGetter: 'node.rowIndex + 1', width: 52, sortable: false, cellClass: 'hucre-salt' },
            { field: 'BELGEADI', headerName: 'Belge Adı', flex: 1, minWidth: 200, tooltipField: 'BELGEADI', cellClass: 'hucre-salt' },
            { headerName: '', width: 46, sortable: false, resizable: false, cellRenderer: belgeDugme('glyphicon-search', 'Belgeyi Görüntüle', belgeGoster) },
            { headerName: '', width: 46, sortable: false, resizable: false, cellRenderer: belgeDugme('glyphicon-trash', 'Belgeyi Sil', belgeSil) }
        ],
        rowSelection: 'single',
        overlayNoRowsTemplate: '<span class="text-muted">Belge yok</span>'
    });

    // ---------------------------------------------------------------- okuma

    function tanimOku() {
        return Sgb.al('api/personelbilgi/aile/tanim').done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            secenekDoldur($('#ddlCinsiyet'), v.cinsiyetler, true);
            secenekDoldur($('#ddlYakinlik'), v.yakinliklar, true);
            secenekDoldur($('#ddlDurum'), v.durumlar, false);
            secenekDoldur($('#ddlOgrenimTipi'), v.ogrenimTipleri, false);
        }).fail(hataGoster);
    }

    // Eski Listele
    function listele(ilk) {
        if (!personelKod) return $.Deferred().resolve().promise();
        if (!ilk) grdListe.setGridOption('loading', true);
        return Sgb.al('api/personelbilgi/aile/liste', { personelKod: personelKod })
            .done(function (v) {
                grdListe.setGridOption('rowData', v.satirlar || []);
                if (v.hata) mesaj(v.hata, 'danger');
                if (kod) {
                    var n = grdListe.getRowNode(String(kod));
                    if (n) n.setSelected(true);
                }
            })
            .fail(hataGoster)
            .always(function () { grdListe.setGridOption('loading', false); });
    }

    function yukumlulukGoster() {
        $('#grpYukumlulukBitis').prop('hidden', !$('#chkYukumlu').prop('checked'));
    }

    // Eski Temizle
    function temizle() {
        kod = 0;
        $('#frmAile')[0].reset();
        secimYap($('#ddlCinsiyet'), 0);
        secimYap($('#ddlYakinlik'), 0);
        secimYap($('#ddlDurum'), 0);
        secimYap($('#ddlOgrenimTipi'), 0);
        yukumlulukGoster();
        grdListe.deselectAll();
        grdBelgeler.setGridOption('rowData', []);
    }

    // Eski SatirSecildi
    function satirSecildi(k) {
        k = parseInt(k, 10) || 0;
        if (!k) return;
        kod = k;
        Sgb.al('api/personelbilgi/aile/oku', { personelKod: personelKod, kod: k })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var b = v.bilgi;
                $('#txtYukumlulukBitisTarihi').val(b.yukumlulukBitisTarihi || '');
                secimYap($('#ddlCinsiyet'), b.cinsiyet);
                secimYap($('#ddlYakinlik'), b.yakinlik);
                secimYap($('#ddlDurum'), b.durum);
                secimYap($('#ddlOgrenimTipi'), b.ogrenimTipi);
                $('#txtMeslek').val(b.meslek);
                $('#txtTCKimlikNo').val(b.tcKimlikNo);
                $('#txtAd').val(b.ad);
                $('#txtSoyad').val(b.soyad);
                $('#txtDogumTarihi').val(b.dogumTarihi || '');
                $('#spnEng').val(b.sakatlikDerecesi);
                $('#chkAGI').prop('checked', !!b.agi);
                $('#chkYukumlu').prop('checked', !!b.bakmaklaYukumlu);
                yukumlulukGoster();
                grdBelgeler.setGridOption('rowData', v.belgeler || []);
            })
            .fail(hataGoster);
    }

    function formOku() {
        return {
            personelKod: personelKod,
            kod: kod,
            tcKimlikNo: $.trim($('#txtTCKimlikNo').val()),
            ad: $.trim($('#txtAd').val()),
            soyad: $.trim($('#txtSoyad').val()),
            dogumTarihi: $('#txtDogumTarihi').val() || '',
            cinsiyet: parseInt($('#ddlCinsiyet').val(), 10) || 0,
            yakinlik: parseInt($('#ddlYakinlik').val(), 10) || 0,
            durum: parseInt($('#ddlDurum').val(), 10) || 0,
            ogrenimTipi: parseInt($('#ddlOgrenimTipi').val(), 10) || 0,
            meslek: $.trim($('#txtMeslek').val()),
            sakatlikDerecesi: parseInt($('#spnEng').val(), 10) || 0,
            agi: $('#chkAGI').prop('checked'),
            bakmaklaYukumlu: $('#chkYukumlu').prop('checked'),
            yukumlulukBitisTarihi: $('#txtYukumlulukBitisTarihi').val() || ''
        };
    }

    // Eski btnKaydet_Click: kaydedilen kod forma alınır, liste yenilenir (form temizlenmez).
    function kaydet() {
        mesgul(true);
        return Sgb.gonder('api/personelbilgi/aile/kaydet', formOku())
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                kod = parseInt(s.kod, 10) || kod;
                listele(false);
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    }

    // ---------------------------------------------------------------- düğmeler

    $('#chkYukumlu').on('change', yukumlulukGoster);

    $('#btnKaydet').on('click', function () {
        // Eski ZorunluAlanKontrol: T.C. Kimlik No boş bırakılamaz.
        if ($.trim($('#txtTCKimlikNo').val()) === '') {
            $('#txtTCKimlikNo').closest('.form-group').addClass('has-error');
            mesaj('Kırmızı renk ile işaretlenen alanları boş bırakamazsınız.', 'warning');
            return;
        }
        $('#txtTCKimlikNo').closest('.form-group').removeClass('has-error');
        onayla('Kayıt işlemi yapılacak. Onaylıyor musunuz?').done(kaydet);
    });

    $('#btnSil').on('click', function () {
        onayla('Kayıt silinecek. Onaylıyor musunuz?').done(function () {
            if (!kod) { mesaj('Lütfen silmek istediğiniz Birey bilgisini seçin.', 'warning'); return; }
            mesgul(true);
            Sgb.gonder('api/personelbilgi/aile/sil', { personelKod: personelKod, kod: kod })
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    temizle();
                    listele(false);
                    mesaj(s.bilgi, 'success');
                })
                .fail(hataGoster)
                .always(function () { mesgul(false); });
        });
    });

    $('#btnTemizle').on('click', function () { temizle(); $('#txtTCKimlikNo').trigger('focus'); });

    // Eski KimlikBilgisiGetir (mercek ya da Enter)
    function kimlikGetir() {
        var tc = $.trim($('#txtTCKimlikNo').val());
        if (tc === '') { mesaj('Personel T.C.Kimlik No bilgisi boş bırakılamaz.', 'warning'); return; }
        mesgul(true);
        Sgb.al('api/personelbilgi/aile/kimlik', { tcKimlikNo: tc })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                if (v.tcKimlikNo) $('#txtTCKimlikNo').val(v.tcKimlikNo);
                $('#txtAd').val(v.ad || '');
                $('#txtSoyad').val(v.soyad || '');
                if (v.dogumTarihi !== null && v.dogumTarihi !== undefined) $('#txtDogumTarihi').val(v.dogumTarihi);
                if (v.cinsiyet) secimYap($('#ddlCinsiyet'), v.cinsiyet);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    }
    $('#btnKimlik').on('click', kimlikGetir);
    $('#txtTCKimlikNo').on('keydown', function (e) {
        if (e.key === 'Enter') { e.preventDefault(); kimlikGetir(); }
    });

    // Eski NufusYazdir (Kod 2: Nüfus Cüzdanı Örneği, Kod 1: İkametgah İlmühaberi): rapor aile bireyinin T.C. kimlik numarasıyla alınır.
    $('#btnNufusCuzdani, #btnIkametgah').on('click', function () {
        var tc = $.trim($('#txtTCKimlikNo').val());
        if (tc === '') { mesaj('Önce aile bireyini seçin ya da T.C. Kimlik No yazın.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/personelbilgi/rapor/nufus', { personelKod: personelKod, tcKimlikNo: tc, tur: parseInt($d.data('tur'), 10) || 1 })
            .then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- belgeler (eski BelgeKaydet / BelgeGoster / BelgeSil)

    function belgeleriOku() {
        if (!kod) { grdBelgeler.setGridOption('rowData', []); return; }
        Sgb.al('api/personelbilgi/aile/belgeler', { personelKod: personelKod, kod: kod })
            .done(function (v) { grdBelgeler.setGridOption('rowData', v.belgeler || []); })
            .fail(hataGoster);
    }

    function belgeGoster(b) {
        Sgb.indir('api/personelbilgi/aile/belge', { kod: b.KOD });
    }

    function belgeSil(b) {
        onayla('Ekli doküman silinecek. Onaylıyor musunuz?').done(function () {
            Sgb.gonder('api/personelbilgi/aile/belge-sil', { personelKod: personelKod, kod: b.KOD })
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    belgeleriOku();
                })
                .fail(hataGoster);
        });
    }

    function belgeYukle(dosya) {
        var veri = new FormData();
        veri.append('personelKod', String(personelKod));
        veri.append('kod', String(kod));
        veri.append('dosya', dosya);
        mesgul(true);
        $.ajax({
            url: Sgb.KOK + 'api/personelbilgi/aile/belge-ekle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (s) {
            if (s.hata) mesaj(s.hata, 'danger');
            if (s.bilgi) mesaj(s.bilgi, 'info');
            grdBelgeler.setGridOption('rowData', s.belgeler || []);
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else hataGoster(xhr);
        }).always(function () { mesgul(false); });
    }

    $('#btnBelgeEkle').on('click', function () { $('#dosyaBelge').val('').trigger('click'); });
    $('#dosyaBelge').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        // Eski: birey henüz kaydedilmemişse önce Kaydet yapılır, belge kaydedilen bireye eklenir.
        if (!kod) {
            kaydet().done(function (s) { if (s && s.basarili && kod) belgeYukle(dosya); });
            return;
        }
        belgeYukle(dosya);
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)
    tanimOku().always(function () {
        temizle();
        listele(true);
    });
});
