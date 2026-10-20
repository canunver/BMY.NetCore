// Personel İşlemleri "Görevlendirmeler" sekmesi (eski InsanKaynaklari/PersonelGorev.aspx; uçlar /api/personelbilgi/gorev/...).
// Eski sayfadaki gibi: açılışta personelin görevlendirmeleri gelir, listeden seçilen kayıt forma gelir; Kaydet onay sorar,
// Yeni formu boşaltır, Sil seçili kaydı siler. Birimi eski ExtOrgSecim gibi ağaçtan ya da adla aranarak seçilir. Belgeler kayda eklenir.
$(function () {
    'use strict';

    var $kok = $('#pbGorev');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var API = 'api/personelbilgi/gorev/';
    var personelKod = parseInt($kok.data('personelKod'), 10) || 0;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnYeni, #btnSil, #btnBelgeEkle').prop('disabled', evet); }
    function sayi(x) { return parseInt(x, 10) || 0; }
    function gorevNo() { return sayi($('#txtGorevNo').val()); }

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });
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

    var tarihBicim = function (p) { return Sgb.tarih(p.value); };
    var grdGorevler = Sgb.grid('#grdGorevler', {
        columnDefs: [
            { field: 'unvanAd', headerName: 'Unvan', flex: 1, minWidth: 140, tooltipField: 'unvanAd' },
            { field: 'gorevDurumuAd', headerName: 'Görev Durumu', width: 150 },
            { field: 'birimiAd', headerName: 'Birimi', flex: 1, minWidth: 140, tooltipField: 'birimiAd' },
            { field: 'baslamaTarihi', headerName: 'Başlama Tarihi', width: 115, valueFormatter: tarihBicim },
            { field: 'bitisTarihi', headerName: 'Bitiş Tarihi', width: 105, valueFormatter: tarihBicim }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.gorevNo); },
        onRowClicked: function (e) { satirSecildi(e.data); },
        overlayNoRowsTemplate: '<span class="text-muted">Görevlendirme yok</span>'
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
        return Sgb.al(API + 'tanim')
            .done(function (v) {
                var $u = $('#cmbUnvan').empty();
                if (!(v.unvanlar || []).length) $('<option value="0"></option>').appendTo($u);
                $.each(v.unvanlar || [], function (i, u) { $('<option></option>').val(String(u.UnvanKod)).text(u.UnvanAd).appendTo($u); });
                var $d = $('#cmbGorevDurumu').empty();
                $('<option value="0"></option>').appendTo($d);
                $.each(v.durumlar || [], function (i, d) { $('<option></option>').val(String(d.kod)).text(d.ad).appendTo($d); });
                if (v.hata) mesaj(v.hata, 'danger');
                else if (v.bilgi) mesaj(v.bilgi, 'info');
            })
            .fail(hataGoster);
    }

    // Eski Listele
    function listele(ilk) {
        if (personelKod <= 0) return $.Deferred().resolve().promise();
        if (!ilk) grdGorevler.setGridOption('loading', true);
        return Sgb.al(API + 'liste', { personelKod: personelKod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'info'); return; } // eski GenelIslemler.BilgiYaz
                grdGorevler.setGridOption('rowData', v.satirlar || []);
                var n = gorevNo() > 0 ? grdGorevler.getRowNode(String(gorevNo())) : null;
                if (n) n.setSelected(true);
            })
            .fail(hataGoster)
            .always(function () { grdGorevler.setGridOption('loading', false); });
    }

    // ------------------------------------------------------------------ form

    function secimYaz($s, deger) {
        $s.val(String(deger || 0));
        if ($s.val() === null) $s.val($s.find('option').first().val());
    }

    // Eski satirSecildi (loadRecord, ddlBirim.setValue) + SatirSecildi (belgeler)
    function satirSecildi(r) {
        if (!r) return;
        $('#txtGorevNo').val(r.gorevNo);
        secimYaz($('#cmbUnvan'), r.unvanKod);
        secimYaz($('#cmbGorevDurumu'), r.gorevDurumu);
        $('#hdnBirim').val(r.birimi || '');
        $('#txtBirimAd').val(r.birimiAd || '');
        $('#txtBaslamaTarihi').val(r.baslamaTarihi || '');
        $('#txtBitisTarihi').val(r.bitisTarihi || '');
        $('#txtAciklama').val(r.aciklama || '');
        $('#txtOnayNo').val(r.onayNo || '');
        $('#txtOnayTarihi').val(r.onayTarihi || '');
        belgeListele();
    }

    // Eski Temizle (Yeni)
    function temizle() {
        $('#txtGorevNo').val('');
        secimYaz($('#cmbUnvan'), 0);
        secimYaz($('#cmbGorevDurumu'), 0);
        $('#hdnBirim, #txtBirimAd').val('');
        $('#txtBaslamaTarihi, #txtBitisTarihi, #txtAciklama, #txtOnayNo, #txtOnayTarihi').val('');
        grdBelgeler.setGridOption('rowData', []);
        grdGorevler.deselectAll();
    }

    function formOku() {
        return {
            personelKod: personelKod,
            gorevNo: gorevNo(),
            unvanKod: sayi($('#cmbUnvan').val()),
            gorevDurumu: sayi($('#cmbGorevDurumu').val()),
            birimi: sayi($('#hdnBirim').val()),
            baslamaTarihi: $('#txtBaslamaTarihi').val(),
            bitisTarihi: $('#txtBitisTarihi').val(),
            aciklama: $('#txtAciklama').val(),
            onayNo: $('#txtOnayNo').val(),
            onayTarihi: $('#txtOnayTarihi').val()
        };
    }

    // Eski Kaydet(e). sonra(gorevNo): başarılı kayıttan sonra (belge ekleme).
    function kaydetIstek(sonra) {
        mesgul(true);
        return Sgb.gonder(API + 'kaydet', formOku())
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
                $('#txtGorevNo').val(v.gorevNo || '');
                listele();
                if (sonra) sonra(v.gorevNo);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    }

    $('#btnKaydet').on('click', function () {
        if (personelKod <= 0) { mesaj('Personel bilgisi boş bırakılamaz.', 'warning'); return; }
        onay('Kaydetmek İstiyor musunuz?', function () { kaydetIstek(); });
    });

    $('#btnYeni').on('click', temizle);

    // Eski btnSil_Click
    $('#btnSil').on('click', function () {
        if (gorevNo() <= 0) { mesaj('Lütfen silmek istediğiniz görevlendirmeyi seçin.', 'warning'); return; }
        onay('Kayıt silinecek. Onaylıyor musunuz?', function () {
            mesgul(true);
            Sgb.gonder(API + 'sil', { personelKod: personelKod, gorevNo: gorevNo() })
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

    // ------------------------------------------------------------------ birim (eski ExtOrgSecim)

    var birimAgac = null, birimAraZaman = null;
    function birimAgaciKur() {
        birimAgac = Sgb.agac('#agcBirim', {
            yukle: function (ust) {
                return Sgb.al(API + 'birimler', { ust: ust ? ust.kod : 0, ara: ust ? '' : $.trim($('#txtBirimAra').val()) })
                    .then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler || [];
                    });
            },
            secilince: function (d) {
                $('#hdnBirim').val(d.kod);
                $('#txtBirimAd').val(d.birimAd || d.ad);
                $('#pnlBirim').prop('hidden', true);
                $('#btnBirimAc').attr('aria-expanded', 'false');
            },
            seciliMi: function (d) { return String(d.kod) === String($('#hdnBirim').val()); },
            bosMetin: 'Birim bulunamadı.',
            hata: hataGoster
        });
    }
    $('#btnBirimAc').on('click', function () {
        var $p = $('#pnlBirim'), ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false');
        if (ac && !birimAgac) birimAgaciKur();
        if (ac) $('#txtBirimAra').trigger('focus');
    });
    $('#txtBirimAra').on('input', function () {
        clearTimeout(birimAraZaman);
        birimAraZaman = setTimeout(function () { if (birimAgac) birimAgac.yenile(); }, 600);
    });
    $('#btnBirimTemizle').on('click', function () { $('#hdnBirim, #txtBirimAd').val(''); });

    // ------------------------------------------------------------------ belgeler

    function belgeListele() {
        var k = gorevNo();
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
        veri.append('kayitKod', gorevNo());
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
        if (gorevNo() > 0) { belgeYukle(dosya); return; }
        kaydetIstek(function (yeniNo) { if (yeniNo > 0) belgeYukle(dosya); });
    });

    function belgeGoster(b) {
        Sgb.indir(API + 'belge/goster', { personelKod: personelKod, kayitKod: gorevNo(), kod: b.KOD });
    }

    function belgeSil(b) {
        onay('Ekli doküman silinecek. Onaylıyor musunuz?', function () {
            Sgb.gonder(API + 'belge/sil', { personelKod: personelKod, kayitKod: gorevNo(), kod: b.KOD })
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                    belgeListele();
                })
                .fail(hataGoster);
        });
    }

    // ------------------------------------------------------------------ başla

    tanimOku().always(function () { listele(true); });
});
