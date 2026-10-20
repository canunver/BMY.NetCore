// Taahhüt hakediş ödeme emri kaydı, ButceMuhasebe/TaahhutOdemeEmriKayit (eski aynı adlı Ext1.NET sayfası). Taahhüt Kartı'nın
// Ödeme Emri penceresinden ?taahhutKod=..&kod=.. ile açılır. Kaydet ödeme emri / MİF belgesini oluşturur ya da günceller.
// Uçlar: /api/taahhutodemeemri/tanim|hesaplar|yansitma|kaydet (Istemci/Muhasebe/TaahhutOdemeEmriUclari.cs),
// hesap kodu arama /api/taahhutkarti/hesap/*.
$(function () {
    'use strict';

    var $kok = $('.th-oe');
    if (!$kok.length) return;

    var UC = 'api/taahhutodemeemri/';
    var HESAP_UC = 'api/taahhutkarti/hesap/';
    var tutar = Sgb.mt.tutar, sayi = Sgb.mt.sayi;
    var q = new URLSearchParams(window.location.search);
    var taahhutKod = parseInt(q.get('taahhutKod'), 10) || 0;
    var kayitKod = parseInt(q.get('kod'), 10) || 0;
    var tanim = {};

    function deger(secici) { return $.trim(String($(secici).val() == null ? '' : $(secici).val())); }
    function tutarOku(secici) { return sayi(deger(secici)) || 0; }
    function tutarYaz(secici, x) { $(secici).val(x ? tutar(x) : ''); }
    function yil() { var y = parseInt($('#txtYil').val(), 10); return y > 2000 ? y : new Date().getFullYear(); }

    // Pencere olarak açıldıysa kapat (eski parent.hidePopWin); üst sayfa kapanınca ödeme emri listesini yeniler.
    function cerceve() {
        try { return window.frameElement ? window.frameElement.closest('dialog') : null; } catch (e) { return null; }
    }
    if (cerceve()) $('#btnKapat').prop('hidden', false).on('click', function () { cerceve().close(); });

    // ------------------------------------------------------------------ hesap kodları

    var hkHakedis = Sgb.hesapKodu('#txtHakedisHesap', { yil: yil, sonSeviye: true, uc: HESAP_UC, degisince: yansitmaOku });
    var hkOdeme = Sgb.hesapKodu('#txtOdemeHesap', { yil: yil, sonSeviye: true, uc: HESAP_UC });
    var serbest = {};
    $kok.find('input[data-hesap]').each(function () {
        serbest[$(this).data('hesap')] = Sgb.hesapKodu(this, { yil: yil, sonSeviye: true, uc: HESAP_UC });
    });

    function hesapAta(hk, $in, kod) {
        $in.val(kod || '').trigger('sgb:yazildi');
    }

    function secenekler($s, liste, secili) {
        var onceki = secili != null ? secili : $s.val();
        $s.empty().append('<option value=""></option>');
        $.each(liste || [], function (i, h) { $('<option></option>').val(h.kod).text(h.ad).appendTo($s); });
        if (onceki && !$s.find('option').filter(function () { return this.value === onceki; }).length)
            $('<option></option>').val(onceki).text(onceki).appendTo($s);
        $s.val(onceki || '');
    }

    function listeleriYaz(hesaplar) {
        $kok.find('select[data-liste]').each(function () {
            secenekler($(this), (hesaplar || {})[$(this).data('liste')]);
        });
    }

    function yansitmaOku() {
        var h = hkHakedis.deger();
        var secili = $('#ddlYansitma').val();
        if (!h.kod) { secenekler($('#ddlYansitma'), [], ''); return; }
        Sgb.mt.oku(UC + 'yansitma', { yil: yil(), hesapKod: h.kod }).done(function (v) {
            secenekler($('#ddlYansitma'), v.satirlar, secili);
        });
    }

    $('#txtYil').on('change', function () {
        Sgb.mt.oku(UC + 'hesaplar', { yil: yil() }).done(function (v) { listeleriYaz(v.hesaplar); });
        yansitmaOku();
    });

    // ------------------------------------------------------------------ mahsup edilen avans tertibi

    $('#btnMahsupTertip').on('click', function () {
        Sgb.tertipSec({
            yil: yil(),
            muhasebe: tanim.muhasebeKod || '',
            birim: tanim.birimKod || '',
            secKapat: true,
            secilince: function (satirlar) { if (satirlar && satirlar.length) $('#txtMahsupTertip').val(satirlar[0].TERTIP || ''); }
        });
    });
    $('#txtMahsupTertip').on('dblclick', function () { $(this).val(''); }).attr('title', 'Silmek için çift tıklayın');

    // ------------------------------------------------------------------ diğer gelirler

    var grd = Sgb.grid('#grdDiger', {
        columnDefs: [
            { field: 'HESAPKOD', headerName: 'Hesap Kodu', flex: 1, minWidth: 140, editable: true, cellClass: 'th-girilir' },
            { field: 'ALACAK', headerName: 'Tutar', width: 110, editable: true, type: 'rightAligned', cellClass: 'th-girilir',
                cellEditor: 'agNumberCellEditor', valueFormatter: function (p) { return p.value ? tutar(p.value) : ''; } },
            { field: 'HESAPKODKDV', headerName: 'KDV Hesap', width: 130, editable: true, cellClass: 'th-girilir' },
            { field: 'ALACAKKDV', headerName: 'KDV Tutar', width: 100, editable: true, type: 'rightAligned', cellClass: 'th-girilir',
                cellEditor: 'agNumberCellEditor', valueFormatter: function (p) { return p.value ? tutar(p.value) : ''; } }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true
    });

    function digerSatirlari() {
        var l = [];
        grd.forEachNode(function (n) {
            var d = n.data;
            if (!$.trim(d.HESAPKOD || '')) return;
            l.push({ HESAPKOD: $.trim(d.HESAPKOD), ALACAK: Number(d.ALACAK) || 0, HESAPKODKDV: $.trim(d.HESAPKODKDV || ''), ALACAKKDV: Number(d.ALACAKKDV) || 0 });
        });
        return l;
    }

    $('#btnSatirEkle').on('click', function () {
        var r = grd.applyTransaction({ add: [{ HESAPKOD: '', ALACAK: 0, HESAPKODKDV: '', ALACAKKDV: 0 }] });
        if (r && r.add && r.add.length) grd.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: 'HESAPKOD' });
    });
    $('#btnSatirSil').on('click', function () {
        var s = grd.getSelectedRows();
        if (!s.length) { Sgb.bildirim('Silinecek satırları işaretleyin.', 'warning'); return; }
        grd.applyTransaction({ remove: s });
    });

    // ------------------------------------------------------------------ form

    function kayitYaz(k) {
        k = k || {};
        $('#txtBelgeNo').val(k.belgeNo || '');
        if (k.tarih) $('#txtTarih').val(k.tarih);
        if (k.aciklama) $('#txtAciklama').val(k.aciklama);
        hesapAta(hkHakedis, $('#txtHakedisHesap'), k.hakedisHesap);
        tutarYaz('#txtHakedisTutari', k.hakedisTutari);
        hesapAta(hkOdeme, $('#txtOdemeHesap'), k.odemeHesap);
        if (k.yansitmaHesap != null) $('#ddlYansitma').val(k.yansitmaHesap);
        $kok.find('[data-hesap]').each(function () {
            var alan = $(this).data('hesap');
            if (this.tagName === 'SELECT') {
                var v = k[alan] || '';
                if (v && !$(this).find('option').filter(function () { return this.value === v; }).length)
                    $('<option></option>').val(v).text(v).appendTo(this);
                $(this).val(v);
            } else hesapAta(serbest[alan], $(this), k[alan]);
        });
        $kok.find('[data-tutar]').each(function () { tutarYaz(this, k[$(this).data('tutar')]); });
    }

    function istek() {
        var v = {
            kod: kayitKod,
            taahhutKod: taahhutKod,
            yil: yil(),
            belgeNo: deger('#txtBelgeNo'),
            tarih: deger('#txtTarih'),
            aciklama: deger('#txtAciklama'),
            hakedisHesap: hkHakedis.deger().kod || deger('#txtHakedisHesap'),
            hakedisTutari: tutarOku('#txtHakedisTutari'),
            odemeHesap: hkOdeme.deger().kod || deger('#txtOdemeHesap'),
            yansitmaHesap: deger('#ddlYansitma'),
            mahsupTertip: deger('#txtMahsupTertip'),
            mahsupBelgeNo: deger('#txtMahsupBelgeNo'),
            digerGelirler: digerSatirlari()
        };
        $kok.find('[data-hesap]').each(function () {
            var alan = $(this).data('hesap');
            v[alan] = this.tagName === 'SELECT' ? deger(this) : ((serbest[alan] && serbest[alan].deger().kod) || deger(this));
        });
        $kok.find('[data-tutar]').each(function () { v[$(this).data('tutar')] = tutarOku(this); });
        return v;
    }

    $('#btnKaydet').on('click', function () {
        grd.stopEditing();
        if (!taahhutKod) { Sgb.bildirim('Taahhüt kartı bilgisi alınamadı.', 'danger'); return; }
        if (!deger('#txtTarih')) { Sgb.bildirim('Ödeme emri tarihini giriniz.', 'warning'); return; }
        Sgb.mt.islem(UC + 'kaydet', istek(), 'Bilgiler kayıt edilecektir. Onaylıyor musunuz?').done(function (v) {
            if (v.kod) kayitKod = v.kod;
            if (v.belgeNo) $('#txtBelgeNo').val(v.belgeNo);
            if (v.yil) $('#txtYil').val(v.yil);
            durum();
            try { if (window.parent && window.parent !== window && window.parent.OdemeEmriListeYenile) window.parent.OdemeEmriListeYenile(); } catch (e) { /* başka kaynak */ }
        });
    });

    function durum() {
        $('#lblKayit').text(kayitKod ? 'Kayıtlı ödeme emri' + (deger('#txtBelgeNo') ? ': ' + yil() + '/' + deger('#txtBelgeNo') : '') : 'Yeni ödeme emri');
    }

    // ------------------------------------------------------------------ açılış

    if (!taahhutKod) { Sgb.bildirim('Taahhüt kartı bilgisi alınamadı.', 'danger'); $('#btnKaydet').prop('disabled', true); return; }
    var p = { taahhutKod: taahhutKod };
    if (kayitKod) p.kod = kayitKod;
    if (parseInt(q.get('yil'), 10) > 0) p.yil = parseInt(q.get('yil'), 10);
    Sgb.mt.oku(UC + 'tanim', p).done(function (v) {
        tanim = v;
        $('#txtYil').val(v.yil);
        $('#txtTarih').val(v.tarih || '');
        $('#txtAciklama').val(v.aciklama || '');
        listeleriYaz(v.hesaplar);
        secenekler($('#ddlYansitma'), v.yansitmalar, v.kayit ? v.kayit.yansitmaHesap : '');
        if (v.kayit) kayitYaz(v.kayit);
        grd.setGridOption('rowData', $.map(v.digerGelirler || [], function (d) { return $.extend({}, d); }));
        durum();
    });
});
