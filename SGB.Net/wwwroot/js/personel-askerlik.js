// Personel İşlemleri "Askerlik" sekmesi (InsanKaynaklari/PersonelAskerlik; eski PersonelAskerlik.aspx).
// Açılışta seçenekler ve personelin askerlik bilgileri okunur. Askerlik durumuna göre alanlar gizlenir / gösterilir
// (eski AskerlikDurumuDegisti: tecilli -> yalnızca tecil tarihi, muaf -> yalnızca muaf nedeni, diğerleri -> askerlik alanları).
// MANAS kullanan kurumlarda (eski Page_Load) askerlik durumu dışındaki alanlar gösterilmez.
// Belgeler: Belge Ekle önce askerlik bilgilerini kaydeder, sonra seçilen dosyayı yükler (eski BelgeKaydet).
$(function () {
    'use strict';

    var $sayfa = $('#pasSayfa');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var personelKod = parseInt($sayfa.data('personel-kod'), 10) || 0;
    var manas = false;

    var TECIL = '07.00001';
    var MUAF = '07.00005';

    // Tarih alanları: kutu kimliği -> JSON alanı
    var TARIHLER = {
        txtDuhulTarihi: 'askerlikBaslamaTarihi',
        txtTerhisTarihi: 'terhisTarihi',
        txtTecilTarihi: 'tecilTarihi',
        txtTerhisBelgeTarihi: 'belgeTarihi',
        txtAstegmenNaspTarihi: 'astegmenNaspTarihi',
        txtTegmenNaspTarihi: 'tegmenNaspTarihi',
        txtYedekSubayOkuluGirisTarihi: 'yedekSubayOkuluGirisTarihi',
        txtErYedekSubayGecisTarihi: 'erYedekSubayGecisTarihi',
        txtErYedekSubayKitaBaslamaTarihi: 'erYedekSubayKitaHizmetiBaslamaTarihi',
        txtErYedekSubayKitaBitisTarihi: 'erYedekSubayKitaHizmetiBitisTarihi',
        txtIbrazIntikalTarihi: 'ibrazIntikalTarihi'
    };
    // Metin alanları: kutu kimliği -> JSON alanı
    var METINLER = {
        txtMuafNedeni: 'askerlikMuafNeden',
        txtSayilmayanGunSayisi: 'sayilmayanGunSayisi',
        txtCuzdanNo: 'askerlikCuzdanNo',
        txtAskerlikSubesi: 'askerlikSubesi',
        txtSinifDonemSicilNo: 'sinifDonemSicilNo',
        txtErYedekSubayGorevYeri: 'erYedekSubayGorevYeri',
        txtAciklama: 'aciklama',
        txtTerhisBelgeNo: 'belgeNo'
    };

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #fileBelge').prop('disabled', evet); }

    function onayla(metin) {
        var d = document.getElementById('dlgOnay');
        $('#dlgOnayMetin').text(metin);
        return new Promise(function (coz) {
            function kapandi() {
                d.removeEventListener('close', kapandi);
                coz(d.returnValue === 'evet');
            }
            d.returnValue = '';
            d.addEventListener('close', kapandi);
            d.showModal();
        });
    }

    function secenekDoldur($ddl, liste) {
        $ddl.empty();
        $.each(liste || [], function (i, s) { $('<option></option>').val(String(s.KOD)).text(s.AD).appendTo($ddl); });
    }

    function seciliYap($ddl, deger) {
        $ddl.val(String(deger == null ? '' : deger));
        if ($ddl.val() === null) $ddl.prop('selectedIndex', 0);
    }

    // Eski AskerlikDurumuDegisti (+ MANAS'ta Page_Load'daki gizleme)
    function gorunumAyarla() {
        var kayitKod = String($('#ddlAskerlikDurumu').val() || '').split(':')[1] || '';
        var tecil = kayitKod === TECIL, muaf = kayitKod === MUAF;
        $sayfa.find('[data-grup="tecil"]').prop('hidden', manas || !tecil);
        $sayfa.find('[data-grup="muaf"]').prop('hidden', manas || !muaf);
        $sayfa.find('[data-grup="normal"]').prop('hidden', manas || tecil || muaf);
        $sayfa.find('[data-grup="aciklama"], [data-grup="diger"]').prop('hidden', manas);
    }

    function formTemizle() {
        $('#frmAskerlik')[0].reset();
        $('#ddlAskerlikDurumu, #ddlRutbe, #ddlSinif').prop('selectedIndex', 0);
    }

    // Eski Listele
    function oku() {
        formTemizle();
        if (personelKod <= 0) { gorunumAyarla(); return $.Deferred().resolve().promise(); }
        return Sgb.al('api/personelbilgi/askerlik/oku', { personelKod: personelKod })
            .done(function (a) {
                if (a.hata) { mesaj(a.hata, 'danger'); return; }
                if (a.bos) return;
                if (a.askerlikDurumu) seciliYap($('#ddlAskerlikDurumu'), a.askerlikDurumu);
                $.each(TARIHLER, function (kutu, alan) { $('#' + kutu).val(a[alan] || ''); });
                $.each(METINLER, function (kutu, alan) { $('#' + kutu).val(a[alan] == null ? '' : a[alan]); });
                seciliYap($('#ddlRutbe'), a.rutbesi);
                seciliYap($('#ddlSinif'), a.sinifi);
                $('#chkBorclanma').prop('checked', a.borclanmaVarMi === true);
            })
            .fail(hataGoster)
            .always(gorunumAyarla);
    }

    function formOku() {
        var istek = {
            personelKod: personelKod,
            askerlikDurumu: $('#ddlAskerlikDurumu').val() || '',
            rutbesi: parseInt($('#ddlRutbe').val(), 10) || 0,
            sinifi: parseInt($('#ddlSinif').val(), 10) || 0,
            borclanmaVarMi: $('#chkBorclanma').prop('checked')
        };
        $.each(TARIHLER, function (kutu, alan) { istek[alan] = $('#' + kutu).val() || ''; });
        $.each(METINLER, function (kutu, alan) { istek[alan] = $('#' + kutu).val() || ''; });
        return istek;
    }

    // Eski Kaydet: denetim + kayıt; sonuç sözle (promise) döner (başarılıysa çözülür).
    function kaydet() {
        var istek = formOku();
        var eksik = [];
        if (personelKod <= 0) eksik.push('Personel Bilgisi boş bırakılamaz.');
        if (!istek.askerlikDurumu) eksik.push('Askerlik durum bilgisini boş geçmeyiniz.');
        if (eksik.length) { mesaj(eksik.join('\n'), 'warning'); return $.Deferred().reject().promise(); }

        var d = $.Deferred();
        mesgul(true);
        Sgb.gonder('api/personelbilgi/askerlik/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); d.reject(); return; }
                mesaj(s.bilgi, 'success');
                d.resolve();
            })
            .fail(function (xhr) { hataGoster(xhr); d.reject(); })
            .always(function () { mesgul(false); });
        return d.promise();
    }

    $('#ddlAskerlikDurumu').on('change', gorunumAyarla);

    $('#btnKaydet').on('click', function () {
        onayla('Kayıt işlemi yapılacak. Onaylıyor musunuz?').then(function (evet) {
            if (evet) kaydet();
        });
    });

    // ------------------------------------------------------------------ belgeler

    function komutDugmesi(simge, ipucu, islem) {
        return function (p) {
            var b = document.createElement('button');
            b.type = 'button';
            b.className = 'btn btn-link btn-xs';
            b.title = ipucu;
            b.setAttribute('aria-label', ipucu);
            b.innerHTML = '<span class="glyphicon ' + simge + '" aria-hidden="true"></span>';
            b.addEventListener('click', function (e) { e.stopPropagation(); islem(p.data); });
            return b;
        };
    }

    var belgeGrid = Sgb.grid('#grdBelgeler', {
        columnDefs: [
            { headerName: '#', valueGetter: 'node.rowIndex + 1', width: 55, sortable: false, cellClass: 'hucre-salt' },
            { field: 'BELGEADI', headerName: 'Belge Adı', flex: 1, minWidth: 200, cellClass: 'hucre-salt', tooltipField: 'BELGEADI' },
            { colId: 'goster', headerName: '', width: 48, sortable: false, resizable: false, cellClass: 'hucre-salt',
                cellRenderer: komutDugmesi('glyphicon-search', 'Belgeyi Görüntüle', belgeGoster) },
            { colId: 'sil', headerName: '', width: 48, sortable: false, resizable: false, cellClass: 'hucre-salt',
                cellRenderer: komutDugmesi('glyphicon-trash', 'Belgeyi Sil', belgeSil) }
        ],
        getRowId: function (p) { return String(p.data.KOD); },
        overlayNoRowsTemplate: '<span class="text-muted">Belge yok</span>'
    });

    // Eski BelgeListesiDoldur; ilk: açılıştaki okuma ("Yükleniyor..." gösterilmez).
    function belgeleriListele(ilk) {
        if (personelKod <= 0) return;
        if (!ilk) belgeGrid.setGridOption('loading', true);
        Sgb.al('api/personelbilgi/askerlik/belgeler', { personelKod: personelKod })
            .done(function (veri) {
                belgeGrid.setGridOption('rowData', veri.satirlar || []);
                if (veri.hata) mesaj(veri.hata, 'danger');
            })
            .fail(hataGoster)
            .always(function () { belgeGrid.setGridOption('loading', false); });
    }

    // Eski BelgeGoster
    function belgeGoster(b) {
        Sgb.indir('api/personelbilgi/askerlik/belge-indir', { personelKod: personelKod, kod: b.KOD });
    }

    // Eski BelgeSil
    function belgeSil(b) {
        onayla('Ekli doküman silinecek. Onaylıyor musunuz?').then(function (evet) {
            if (!evet) return;
            Sgb.gonder('api/personelbilgi/askerlik/belge-sil', { personelKod: personelKod, kod: b.KOD })
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    belgeleriListele(false);
                })
                .fail(hataGoster);
        });
    }

    // Eski BelgeKaydet: önce askerlik bilgileri kaydedilir, sonra dosya yüklenir.
    $('#fileBelge').on('change', function () {
        var kutu = this;
        var dosya = kutu.files && kutu.files[0];
        if (!dosya) return;
        if (personelKod <= 0) { mesaj('Belgenin kim için kaydedileceğini seçin.', 'warning'); kutu.value = ''; return; }
        kaydet().done(function () {
            var veri = new FormData();
            veri.append('personelKod', String(personelKod));
            veri.append('dosya', dosya);
            mesgul(true);
            $.ajax({
                url: Sgb.KOK + 'api/personelbilgi/askerlik/belge-yukle', method: 'POST', data: veri, processData: false, contentType: false,
                dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
            }).done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                if (s.bilgi) mesaj(s.bilgi, 'success');
                belgeleriListele(false);
            }).fail(function (xhr) {
                if (xhr.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
                mesaj(xhr.status === 413 ? 'Belge çok büyük.' : 'Belge yüklenemedi.', 'danger');
            }).always(function () { mesgul(false); });
        }).always(function () { kutu.value = ''; });
    });

    // ------------------------------------------------------------------ açılış

    Sgb.al('api/personelbilgi/askerlik/secenekler')
        .done(function (veri) {
            manas = veri.manas === true;
            secenekDoldur($('#ddlAskerlikDurumu'), veri.askerlikDurumlari);
            secenekDoldur($('#ddlRutbe'), veri.rutbeler);
            secenekDoldur($('#ddlSinif'), veri.siniflar);
        })
        .fail(hataGoster)
        .always(function () {
            oku();
            belgeleriListele(true);
        });
});
