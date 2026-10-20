// Personel İşlemleri / Hizmet Belgesi sekmesi (eski InsanKaynaklari/PersonelKadro.aspx).
// Hizmet kayıtları /api/personelbilgi/kadro/liste ucundan okunur. Kayıt ekleme / açma PersonelAtama penceresinde yapılır
// (eski parent.PersonelAtamaAc); pencere kapanınca liste yeniden okunur (eski KadroYenile).
(function ($) {
    'use strict';

    var $kok = $('#pbKadro');
    if (!$kok.length) return;
    var PERSONEL = parseInt($kok.data('personel-kod'), 10) || 0;
    var UC = 'api/personelbilgi/kadro/';
    var grid, belgeGrid;

    // Eski DurumIconRenderer: kayıt durumuna göre simge.
    var DURUM = {
        1: ['glyphicon-plus', 'durum-yeni'], 146: ['glyphicon-plus', 'durum-yeni'],
        106: ['glyphicon-ok', 'durum-aktif'], 136: ['glyphicon-ok', 'durum-aktif'],
        116: ['glyphicon-warning-sign', 'durum-kapaniyor'],
        126: ['glyphicon-lock', 'durum-kapandi'],
        201: ['glyphicon-lock', 'durum-ayrildi'],
        209: ['glyphicon-ban-circle', 'durum-red']
    };

    function durumHucre(p) {
        if (!p.data) return '';
        var d = DURUM[p.data.kadroDurum] || ['glyphicon-remove', 'durum-tanimsiz'];
        var s = document.createElement('span');
        s.className = 'glyphicon ' + d[0] + ' pbk-durum ' + d[1];
        s.title = p.data.kadroDurumAd || '';
        s.setAttribute('aria-label', p.data.kadroDurumAd || 'Tanımsız');
        return s;
    }

    function acHucre(p) {
        if (!p.data) return '';
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-link btn-xs pbk-ac';
        b.title = 'Aç';
        b.setAttribute('aria-label', 'Hizmet kaydını aç');
        b.innerHTML = '<span class="glyphicon glyphicon-pencil" aria-hidden="true"></span>';
        b.addEventListener('click', function () { hizmetAc(p.data.kod); });
        return b;
    }

    function tarihHucre(p) { return Sgb.tarih(p.value); }

    function sayi(alan, baslik, gen) {
        return { field: alan, headerName: baslik, width: gen || 62, cellClass: 'hucre-salt pbk-orta', headerClass: 'pbk-orta' };
    }

    function dortlu(onEk) {
        return [
            sayi(onEk + 'Derece', 'Derece', 66), sayi(onEk + 'Kademe', 'Kademe', 70),
            sayi(onEk === 'kazanilmisHak' ? 'kazanilmisGosterge' : onEk + 'Gosterge', 'Gösterge', 78),
            sayi(onEk === 'kazanilmisHak' ? 'kazanilmisEkGosterge' : onEk + 'EkGosterge', 'Ek Gösterge', 92)
        ];
    }

    function metin(alan, baslik, gen) {
        return { field: alan, headerName: baslik, width: gen, cellClass: 'hucre-salt', tooltipField: alan };
    }

    function gridKur() {
        grid = Sgb.grid('#grdListe', {
            columnDefs: [
                { headerName: '', children: [
                    { colId: 'ac', headerName: '', width: 44, sortable: false, resizable: false, cellRenderer: acHucre, cellClass: 'hucre-salt' },
                    { field: 'kadroDurum', headerName: '', width: 40, sortable: false, cellRenderer: durumHucre, cellClass: 'hucre-salt pbk-orta' },
                    { field: 'atamaTarihi', headerName: 'Başlama Tarihi', width: 110, valueFormatter: tarihHucre, cellClass: 'hucre-salt' },
                    { field: 'ayrilmaTarihi', headerName: 'Bitiş Tarihi', width: 100, valueFormatter: tarihHucre, cellClass: 'hucre-salt' },
                    metin('hizmetSebebiAd', 'Başlangıç Sebebi', 160),
                    metin('ayrilmaGerekcesiAd', 'Ayrılma Gerekçesi', 160),
                    metin('personelTipAd', 'Personel Türü', 150)
                ] },
                { headerName: 'Kadro', headerClass: 'pbk-grup', children: [
                    metin('birimGorevAd', 'Birim', 180),
                    metin('unvanAd', 'Unvanı', 140),
                    sayi('kadroDerece', 'Derece', 66)
                ] },
                { headerName: 'Kazanılmış Hak', headerClass: 'pbk-grup', children: dortlu('kazanilmisHak') },
                { headerName: 'Ödemeye Esas', headerClass: 'pbk-grup', children: dortlu('odemeyeEsas') },
                { headerName: 'Emekliye Esas', headerClass: 'pbk-grup', children: dortlu('emekliyeEsas') }
            ],
            rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
            getRowId: function (p) { return String(p.data.kod); },
            onRowDoubleClicked: function (e) { if (e.data) hizmetAc(e.data.kod); },
            tooltipShowDelay: 400
        });
    }

    function belgeGridKur() {
        belgeGrid = Sgb.grid('#grdBelgeler', {
            headerHeight: 0,
            columnDefs: [
                { colId: 'no', headerName: '#', width: 50, valueGetter: function (p) { return p.node.rowIndex + 1; }, cellClass: 'hucre-salt' },
                { field: 'BELGEADI', headerName: 'Belge', flex: 1, minWidth: 200, cellClass: 'hucre-salt' },
                { colId: 'goster', headerName: '', width: 44, sortable: false, cellClass: 'hucre-salt', cellRenderer: function (p) {
                    return dugme('glyphicon-zoom-in', 'Belgeyi Görüntüle', function () { Sgb.indir(UC + 'belgegoster', { kod: p.data.KOD }); });
                } },
                { colId: 'sil', headerName: '', width: 44, sortable: false, cellClass: 'hucre-salt', cellRenderer: function (p) {
                    return dugme('glyphicon-trash', 'Belgeyi Sil', function () { belgeSil(p.data.KOD); });
                } }
            ],
            rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true }
        });
    }

    function dugme(simge, baslik, tikla) {
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-link btn-xs';
        b.title = baslik;
        b.setAttribute('aria-label', baslik);
        b.innerHTML = '<span class="glyphicon ' + simge + '" aria-hidden="true"></span>';
        b.addEventListener('click', tikla);
        return b;
    }

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function belgelerGoster(v) {
        $('#bolBelgeler').prop('hidden', !v.belgeVar);
        if (v.belgeVar) {
            if (!belgeGrid) belgeGridKur();
            belgeGrid.setGridOption('rowData', v.belgeler || []);
        }
    }

    // Eski Listele.
    function listele() {
        if (!PERSONEL) return;
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', { personelKod: PERSONEL }).done(function (v) {
            if (v.hata) Sgb.bildirim(v.hata, 'danger');
            grid.setGridOption('rowData', v.satirlar || []);
            belgelerGoster(v);
        }).fail(hataGoster).always(function () { grid.setGridOption('loading', false); });
    }

    // Pencereler, eski showPopWinTop gibi üst sayfada (Personel İşlemleri) açılır; yoksa bu sayfada.
    function pencere(adres, baslik, kapaninca) {
        var tam = new URL(adres, window.location.href).href;
        var ust = null;
        try { if (window.parent && window.parent !== window && window.parent.Sgb) ust = window.parent.Sgb; } catch (e) { ust = null; }
        (ust || Sgb).pencere(tam, baslik, kapaninca);
    }

    function personelVarMi() {
        if (!PERSONEL) { Sgb.bildirim('İlk önce Personel seçiniz', 'warning'); return false; }
        return true;
    }

    // Eski PersonelAtamaAc(islemNo) -> parent.PersonelAtamaAc(personelKod, islemNo); kapanınca KadroYenile.
    function hizmetAc(kod) {
        if (!personelVarMi()) return;
        pencere('PersonelAtama?menuYok=1&personelKod=' + PERSONEL + '&kod=' + (parseInt(kod, 10) || 0), 'Hizmet Bilgisi', function () {
            listele();
            if (window.parent && window.parent.PersonelBilgi && window.parent.PersonelBilgi.ozetYenile) window.parent.PersonelBilgi.ozetYenile();
        });
    }

    // ------------------------------------------------------------------ onay penceresi
    var onayIslem = null;
    var dlg = document.getElementById('dlgOnay');
    function onaySor(metinStr, baslik, evet) {
        $('#dlgOnayBaslik').text(baslik || 'Onay');
        $('#dlgOnayMetin').text(metinStr);
        onayIslem = evet;
        dlg.showModal();
    }
    $(dlg).on('click', '[data-kapat]', function () { dlg.close(); });
    $('#btnOnayEvet').on('click', function () {
        var f = onayIslem;
        onayIslem = null;
        dlg.close();
        if (f) f();
    });

    // Eski btnOnayla_Click: onayKod 5 Onayla, 9 İptal Et, 1 Yeni Yap, 10 Sil.
    var ONAY_METIN = {
        5: ['Seçili işlemleri onaylamak istiyor musunuz?', 'Onay'],
        9: ['Seçili işlemleri iptal etmek istiyor musunuz?', 'Red'],
        1: ['Seçili işlemleri değiştirilebilir hale getirmek istiyor musunuz?', 'Yeni Yap'],
        10: ['Seçili işlemleri silmek istiyor musunuz?', 'Sil']
    };

    function onayIslemi(onayKod, $dugme) {
        if (!personelVarMi()) return;
        var kodlar = $.map(grid.getSelectedRows(), function (r) { return r.kod; });
        if (!kodlar.length) { Sgb.bildirim('Lütfen durumu değiştirilecek kayıt seçiniz!', 'warning'); return; }
        var m = ONAY_METIN[onayKod];
        onaySor(m[0], m[1], function () {
            $dugme.prop('disabled', true);
            Sgb.gonder(UC + 'onay', { personelKod: PERSONEL, kodlar: kodlar, onayKod: onayKod }).done(function (v) {
                if (v.basarili) {
                    Sgb.bildirim(v.bilgi, 'success');
                    listele();
                    if (window.parent && window.parent.PersonelBilgi && window.parent.PersonelBilgi.ozetYenile) window.parent.PersonelBilgi.ozetYenile();
                } else Sgb.bildirim(v.hata, 'danger');
            }).fail(hataGoster).always(function () { $dugme.prop('disabled', false); });
        });
    }

    // ------------------------------------------------------------------ belgeler
    function belgeSil(kod) {
        onaySor('Ekli doküman silinecek. Onaylıyor musunuz?', 'Uyarı', function () {
            Sgb.gonder(UC + 'belgesil', { kod: kod }).done(function (v) {
                if (v.basarili) listele();
                else Sgb.bildirim(v.hata, 'danger');
            }).fail(hataGoster);
        });
    }

    $('#dosyaBelge').on('change', function () {
        var girdi = this;
        var dosya = girdi.files && girdi.files[0];
        if (!dosya) return;
        var veri = new FormData();
        veri.append('personelKod', PERSONEL ? String(PERSONEL) : '');
        veri.append('dosya', dosya);
        $.ajax({
            url: Sgb.KOK + UC + 'belgekaydet', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (!v.basarili) Sgb.bildirim(v.hata, 'danger');
            else if (v.bilgi) Sgb.bildirim(v.bilgi, 'success');
            if (v.belgeler && belgeGrid) belgeGrid.setGridOption('rowData', v.belgeler);
        }).fail(hataGoster).always(function () { girdi.value = ''; });
    });

    // ------------------------------------------------------------------ düğmeler
    $('#btnYeniKayit').on('click', function () { hizmetAc(0); });

    $('#btnHizmetSureleri').on('click', function () {
        if (!personelVarMi()) return;
        pencere('PersonelHizmetSureleri?menuYok=1&personelKod=' + PERSONEL, 'Hizmet Süreleri');
    });

    $('#btnHizmetAyari').on('click', function () {
        if (!personelVarMi()) return;
        pencere('PersonelHizmetAyari?menuYok=1&personelKod=' + PERSONEL, 'Hizmet Ayarları');
    });

    $('#btnHitaptanAl').on('click', function () {
        if (!personelVarMi()) return;
        pencere('PersonelHITAP?menuYok=1&personelKod=' + PERSONEL + '&tur=1', "HİTAP'tan Al", listele);
    });

    $('#btnHizmetBelgesi').on('click', function () {
        if (!personelVarMi()) return;
        var $d = $(this).prop('disabled', true);
        Sgb.indir(UC + 'hizmetbelgesi', { personelKod: PERSONEL }).then(function () { $d.prop('disabled', false); });
    });

    $('#btnKisiBorcu').on('click', function () {
        if (!personelVarMi()) return;
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/personelbilgi/rapor/kisiborcu', { personelKod: PERSONEL }).then(function () { $d.prop('disabled', false); });
    });

    $('[data-onay]').on('click', function () { onayIslemi(parseInt($(this).data('onay'), 10), $(this)); });

    gridKur();
    listele();
})(jQuery);
