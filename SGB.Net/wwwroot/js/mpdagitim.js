// Masraf Planı Dağıtım (ButceHazirlikv2/MPDagitim; eski MPDagitim.aspx, menüde "Masraf Planı Tavan"): soldaki listede yılın dağıtım
// kayıtları, sağda seçilen kaydın tertibi, açıklaması ve dağıtılacak miktarı; altta kayda eklenen birimlerin geçen yıl gerçekleşme,
// bu yıl planlanan ve atanacak miktarları. Birim Ekle penceresinden ya da Excel'den birim eklenir; Revize Üret seçilen ay için
// masraf planı revizesi üretir. Uçlar: /api/mpdagitim/* (ButceHazirlik/MPDagitimUclari.cs).
$(function () {
    'use strict';

    if (!$('.mp-dagitim').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var API = 'api/mpdagitim';
    var formId = 0;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function sonucPencere(baslik, metin) {
        $('#dlgSonucBaslik').text(baslik);
        $('#sonucMetin').text(metin);
        document.getElementById('dlgSonuc').showModal();
    }
    function sonucGoster(v) {
        if (v.hata) { mesaj(v.hata, 'danger'); return false; }
        if (v.bilgi) {
            if (v.uyariVar) sonucPencere('Sonuç', v.bilgi);
            else mesaj(v.bilgi, 'success');
        }
        return true;
    }

    // "1.234.567,89" -> 1234567.89
    function sayiOku(s) {
        s = $.trim(String(s || '')).replace(/\s/g, '');
        if (!s) return 0;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        else if ((s.match(/\./g) || []).length > 1) s = s.replace(/\./g, '');
        var v = parseFloat(s);
        return isFinite(v) ? v : 0;
    }

    var buYil = new Date().getFullYear();
    function yilOku() { return parseInt($('#girYil').val(), 10) || 0; }
    var kodlar = MPTavan.kodAlanlari(yilOku, hataGoster, API);

    function tutarYaz(p) { return MPTavan.tutar(p.value); }
    function sayiSutun(ozellik) {
        return $.extend({ type: 'rightAligned', width: 130, cellClass: ['sayi'], valueFormatter: tutarYaz }, ozellik);
    }

    // ---------------------------------------------------------------- kayıt listesi (eski listele)

    var listeGrid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'id', headerName: 'No', width: 70, type: 'rightAligned' },
            {
                field: 'aciklama', headerName: 'Açıklama', flex: 1, minWidth: 180,
                tooltipValueGetter: function (p) { return p.data ? (p.data.tertip || '') : ''; }
            },
            sayiSutun({ field: 'miktar', headerName: 'Miktar', width: 125 })
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        tooltipShowDelay: 400,
        overlayNoRowsTemplate: '<span>Yılı girip Listele\'ye basınız.</span>',
        onRowDoubleClicked: function (p) { if (p.data) kayitOku(p.data.id); }
    });

    function listele() {
        var yil = parseInt($('#sorYil').val(), 10) || 0;
        if (yil <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        listeGrid.setGridOption('loading', true);
        Sgb.al(API + '/liste', { yil: yil })
            .done(function (v) {
                listeGrid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                if (v.hata) { listeGrid.setGridOption('rowData', []); mesaj(v.hata, 'danger'); return; }
                listeGrid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); listeGrid.setGridOption('loading', false); });
    }
    $('#frmListe').on('submit', function (e) { e.preventDefault(); listele(); });

    // ---------------------------------------------------------------- birim ödenek durumu

    var birimGrid = Sgb.grid('#grdBirimler', {
        columnDefs: [
            { field: 'birimKod', headerName: 'Kod', width: 150 },
            { field: 'birimAd', headerName: 'Ad', flex: 1, minWidth: 220 },
            sayiSutun({ field: 'oncYilDeger', headerName: 'Geçen Yıl Gerçekleşme' }),
            sayiSutun({ field: 'buYilDeger', headerName: 'Bu Yıl Planlanan' }),
            sayiSutun({ field: 'miktar', headerName: 'Atanacak Miktar' })
        ],
        defaultColDef: { wrapHeaderText: true, autoHeaderHeight: true },
        overlayNoRowsTemplate: '<span>Kayıt açılınca birimleri burada görünür.</span>'
    });

    function toplamYenile() {
        var r = [];
        birimGrid.forEachNode(function (n) { if (n.data) r.push(n.data); });
        if (!r.length) { birimGrid.setGridOption('pinnedBottomRowData', []); return; }
        var t = { birimKod: 'T O P L A M', birimAd: '', oncYilDeger: 0, buYilDeger: 0, miktar: 0 };
        $.each(r, function (i, d) {
            t.oncYilDeger += Number(d.oncYilDeger) || 0;
            t.buYilDeger += Number(d.buYilDeger) || 0;
            t.miktar += Number(d.miktar) || 0;
        });
        birimGrid.setGridOption('pinnedBottomRowData', [t]);
    }

    // ---------------------------------------------------------------- form (eski formaDegerAta / ekrandanAl)

    function formaDegerAta(f) {
        f = f || {};
        formId = f.id || 0;
        $('#girYil').val(f.yil || buYil);
        kodlar.ata('kur', f.kur || '', f.kurAd || '');
        kodlar.ata('fon', f.fon || '', f.fonAd || '');
        kodlar.ata('fin', f.fin || '', f.finAd || '');
        kodlar.ata('eko', f.eko || '', f.ekoAd || '');
        kodlar.ata('peb', f.peb || '', f.pebAd || '');
        $('#girAciklama').val(f.aciklama || '');
        $('#girMiktar').val(f.miktar ? MPTavan.tutar(f.miktar) : '');
        $('#lblKayit').text(formId ? '(No: ' + formId + ')' : '(Yeni kayıt)');
        birimGrid.setGridOption('rowData', f.detay || []);
        toplamYenile();
    }

    function kayitOku(id) {
        birimGrid.setGridOption('loading', true);
        Sgb.al(API + '/oku', { id: id })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                formaDegerAta(v);
            })
            .fail(hataGoster)
            .always(function () { birimGrid.setGridOption('loading', false); });
    }

    $('#btnYeni').on('click', function () { formaDegerAta({}); $('#txt_kur').trigger('focus'); });

    $('#girMiktar').on('blur', function () {
        var v = sayiOku($(this).val());
        $(this).val(v ? MPTavan.tutar(v) : '');
    });

    $('#frmForm').on('submit', function (e) {
        e.preventDefault();
        var f = {
            id: formId, yil: yilOku(),
            kur: kodlar.kod('kur'), fon: kodlar.kod('fon'), fin: kodlar.kod('fin'), eko: kodlar.kod('eko'), peb: kodlar.kod('peb'),
            aciklama: $('#girAciklama').val(), miktar: sayiOku($('#girMiktar').val())
        };
        if (f.yil <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder(API + '/kaydet', f)
            .done(function (v) {
                if (!sonucGoster(v)) return;
                formId = v.id || formId;
                $('#lblKayit').text(formId ? '(No: ' + formId + ')' : '');
                if (listeGrid.getDisplayedRowCount() > 0 || parseInt($('#sorYil').val(), 10) === f.yil) listele();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#girYil').on('change', function () { kodlar.adlariYenile(); });

    // ---------------------------------------------------------------- birim ekle (eski mdlBirimSecim)

    var dlgBirim = document.getElementById('dlgBirim');
    var sonucGrid = Sgb.grid('#grdBirimSonuc', {
        columnDefs: [
            { field: 'kod', headerName: 'Kodu', width: 190 },
            { field: 'ad', headerName: 'Adı', flex: 1, minWidth: 220 },
            {
                headerName: '', width: 80, pinned: 'right', sortable: false,
                cellRenderer: function (p) {
                    if (!p.data) return '';
                    var b = document.createElement('button');
                    b.type = 'button';
                    b.className = 'btn btn-primary btn-xs';
                    b.textContent = 'Ekle';
                    b.addEventListener('click', function () { birimEkle(p.data.kod, b); });
                    return b;
                }
            }
        ],
        overlayNoRowsTemplate: '<span>Kriteri yazıp Listele\'ye basınız.</span>'
    });

    $('#btnBirimEkle').on('click', function () {
        if (!formId) { mesaj('Önce formu saklayınız!', 'warning'); return; }
        dlgBirim.showModal();
        $('#txtKriter').trigger('focus');
    });

    $('#frmBirimAra').on('submit', function (e) {
        e.preventDefault();
        var $d = $('#btnBirimAra').prop('disabled', true);
        sonucGrid.setGridOption('loading', true);
        Sgb.al(API + '/birimler', { kriter: $('#txtKriter').val() })
            .done(function (v) {
                sonucGrid.setGridOption('overlayNoRowsTemplate', '<span>Birim bulunamadı.</span>');
                if (v.hata) { sonucGrid.setGridOption('rowData', []); mesaj(v.hata, 'danger'); return; }
                sonucGrid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); sonucGrid.setGridOption('loading', false); });
    });

    function birimEkle(birimKod, dugme) {
        dugme.disabled = true;
        Sgb.gonder(API + '/birimekle', { id: formId, birimKod: birimKod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); dugme.disabled = false; return; }
                if (v.satir) birimGrid.applyTransaction({ add: [v.satir] });
                toplamYenile();
                mesaj(birimKod + ' eklendi.', 'success');
            })
            .fail(function (xhr) { dugme.disabled = false; hataGoster(xhr); });
    }

    // ---------------------------------------------------------------- Revize Üret (eski mpRevizeUret)

    $('#btnRevizeUret').on('click', function () {
        if (!formId) { mesaj('Önce formu saklayınız!', 'warning'); return; }
        var ay = parseInt($('#inpAy').val(), 10) || 0;
        if (ay < 1 || ay > 12) { mesaj('Ay 1 ile 12 arasında olmalıdır.', 'warning'); return; }
        if (!window.confirm(ay + '. ay için masraf planı revizesi üretilecek. Onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(API + '/revizeuret', { id: formId, ay: ay })
            .done(sonucGoster)
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Dosya Yükle (eski dosyaOnaylaVeYukle)

    $('#btnDosyaYukle').on('click', function () {
        if (!formId) { mesaj('Önce formu saklayınız!', 'warning'); return; }
        $('#fuDosya').val('').trigger('click');
    });
    $('#fuDosya').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        if (!window.confirm('Seçilen dosya yüklenecek:\n\n' + dosya.name + '\n\nDevam edilsin mi?')) { this.value = ''; return; }
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('id', formId);
        var $d = $('#btnDosyaYukle').prop('disabled', true);
        mesaj('Dosya yükleniyor, lütfen bekleyiniz...', 'info');
        $.ajax({
            url: Sgb.KOK + API + '/dosyayukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (sonucGoster(v)) kayitOku(formId);
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else hataGoster(xhr);
        }).always(function () { $d.prop('disabled', false); $('#fuDosya').val(''); });
    });

    // ---------------------------------------------------------------- açılış

    $('#sorYil').val(buYil);
    formaDegerAta({});

    // Soru işareti: dosya yükleme açıklamasını gösterir / gizler.
    $('#btnYardim').on('click', function () {
        var $p = $('#pnlIpucu');
        var ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false').toggleClass('active', ac);
    });
});
