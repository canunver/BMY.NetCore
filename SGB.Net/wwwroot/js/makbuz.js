// Makbuz Bilgileri (ButceMuhasebe/Makbuz; eski aynı adlı Ext.NET sayfası): soldaki kriterlere göre makbuzlar listelenir.
// Yeni Makbuz ve satırdaki Aç tuşu makbuz formunu (MakbuzGiris) pencerede açar; pencere kapanınca liste yenilenir.
// Seçilenleri İptal Et, Kesilen Makbuzlar / Hesap Bazında Rapor (Excel), Ödeme Emri Oluştur. Uçlar: /api/makbuz/* (Muhasebe/MakbuzUclari.cs).
$(function () {
    'use strict';

    if (!$('#grdMakbuzlar').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/makbuz/';
    var IPUCU = '<span>Kriterleri seçip Listele\'ye basınız.</span>';
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var listelendi = false;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // ---------------------------------------------------------------- onay sorusu ve sonuç

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;
    function sor(metin, is) {
        $('#soruMetin').text(metin);
        soruIsi = is;
        dlgSoru.showModal();
    }
    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        var is = soruIsi;
        soruIsi = null;
        if (is) is();
    });
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- kriterler

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: function () { return parseInt(($('#txtBasla').val() || '').substring(0, 4), 10) || 0; },
        kurumsalGizle: true
    });

    function yil() { return parseInt(($('#txtBasla').val() || '').substring(0, 4), 10) || new Date().getFullYear(); }

    // Eski KriterTopla
    function kriter() {
        var d = muhBirim.deger();
        return {
            bas: $('#txtBasla').val(),
            bit: $('#txtBitis').val(),
            muhasebe: d.muhasebe || 0,
            birim: d.birim,
            tip: $('#ddlMakbuzTipi').val() || 0,
            yevmiyeNo: $.trim($('#txtYevmiyeNo').val()),
            hesapKod: $.trim($('#txtHesapKod').val()),
            tcKimlik: $.trim($('#txtTcKimlik').val()),
            kisiAd: $.trim($('#txtKisiAd').val()),
            durum: $('#ddlDurum').val() || 0
        };
    }

    function kriterDenetle(k) {
        if (!k.bas || !k.bit) { mesaj('Lütfen tarih aralığını giriniz.', 'warning'); return false; }
        if (k.bit < k.bas) { mesaj('Bitiş tarihi başlangıç tarihinden önce olamaz.', 'warning'); return false; }
        return true;
    }

    // Hesap kod (eski HesapKod.ascx, yalnızca son kırılım)
    $('#btnHesapSec').on('click', function () {
        Sgb.hesapPlaniSec({
            yil: yil(), sonKirilim: true,
            secilince: function (h) { $('#txtHesapKod').val(h.kod); }
        });
    });

    // ---------------------------------------------------------------- liste

    function acHucre(p) {
        if (!p.data) return '';
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-link btn-xs mk-ac';
        b.title = 'Makbuzu aç';
        b.innerHTML = '<span class="glyphicon glyphicon-edit" aria-hidden="true"></span><span class="sr-only">Aç</span>';
        b.addEventListener('click', function (e) { e.stopPropagation(); sayfaAc(p.data.yil, p.data.muhasebe, p.data.birim, p.data.yevmiyeNo); });
        return b;
    }

    // Eski OdemeEmriAc: ödeme emri belgesi (henüz taşınmadı) yeni pencerede açılır.
    function odemeEmriHucre(p) {
        if (!p.data || !p.value) return '';
        var a = document.createElement('a');
        a.href = '#';
        a.textContent = p.value;
        a.title = 'Ödeme emri belgesini aç';
        a.addEventListener('click', function (e) {
            e.preventDefault();
            window.open('../ButceMuhasebe/OdemeEmrimifFrame.aspx?' + $.param({ menuYok: 1, aramaYok: 1, yil: p.data.yil, belgeNo: p.value }), 'odemeEmri', 'resizable,status');
        });
        return a;
    }

    var grid = Sgb.grid('#grdMakbuzlar', {
        columnDefs: [
            { headerName: 'Aç', width: 56, sortable: false, resizable: false, cellRenderer: acHucre },
            { field: 'yevmiyeNo', headerName: 'Yevmiye No', width: 110 },
            { field: 'durumAd', headerName: 'Durum', width: 90 },
            { field: 'islemTarih', headerName: 'Makbuz Tarihi', width: 120, valueFormatter: function (p) { return Sgb.tarih(p.value); } },
            { field: 'muhasebeAd', headerName: 'Muhasebe', width: 160, hide: true },
            { field: 'birimAd', headerName: 'Birim', width: 180 },
            { field: 'makbuzTipiAciklama', headerName: 'Makbuz Tipi', width: 180 },
            { field: 'kisiAd', headerName: 'Kişi Adı', width: 160 },
            {
                field: 'genelToplam', headerName: 'Tutar', width: 120, type: 'rightAligned',
                valueFormatter: function (p) { return p.value == null ? '' : para.format(p.value); }
            },
            { field: 'paraBirimiKod', headerName: 'Para Birimi', width: 100 },
            { field: 'duzenleyen', headerName: 'Düzenleyen', width: 140 },
            { field: 'unvan', headerName: 'Unvan', width: 130 },
            { field: 'aciklama', headerName: 'Açıklama', flex: 1, minWidth: 200 },
            { field: 'odemeEmriNo', headerName: 'Ödeme Emri No', width: 130, cellRenderer: odemeEmriHucre }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        getRowId: function (p) { return p.data.yil + '-' + p.data.muhasebe + '-' + p.data.yevmiyeNo; },
        getRowClass: function (p) { return p.data && p.data.durum === 9 ? 'mk-iptal' : ''; },
        overlayNoRowsTemplate: IPUCU,
        onRowDoubleClicked: function (e) { sayfaAc(e.data.yil, e.data.muhasebe, e.data.birim, e.data.yevmiyeNo); }
    });

    function sayiYaz(n) {
        $('#lblSayi').text(listelendi ? n + ' makbuz' : '');
    }

    // Listele (eski btnListele_Click).
    function listele(sessiz) {
        var k = kriter();
        if (!kriterDenetle(k)) return;
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', k)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                listelendi = true;
                var satirlar = v.satirlar || [];
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
                grid.setGridOption('rowData', satirlar);
                sayiYaz(satirlar.length);
                if (!satirlar.length && !sessiz) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmSorgu').on('submit', function (e) { e.preventDefault(); listele(false); });

    // ---------------------------------------------------------------- makbuz formu

    // Eski SayfaAc: makbuz formu pencerede (eski showPopWinTop). Kapanınca liste yenilenir.
    function sayfaAc(y, muhasebe, birim, makbuzNo) {
        if (!muhasebe || !birim) { mesaj('Lütfen muhasebe ve birim seçiniz', 'warning'); return; }
        Sgb.pencere('MakbuzGiris?' + $.param({ yil: y, muhasebe: muhasebe, birim: birim, makbuzNo: makbuzNo || '' }), 'Makbuz detay', function () {
            if (listelendi) listele(true);
        });
    }

    // Yeni Makbuz (eski YeniAc)
    $('#btnYeni').on('click', function () {
        var d = muhBirim.deger();
        sayfaAc(0, d.muhasebe, d.birim, '');
    });

    // ---------------------------------------------------------------- iptal

    $('#btnIptal').on('click', function () {
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Herhangi bir Makbuz seçilmemiş!', 'warning'); return; }
        sor('Seçili kayıtları iptal etmek istiyor musunuz?', function () {
            var makbuzlar = $.map(secili, function (r) { return { Yil: r.yil, Muhasebe: r.muhasebe, YevmiyeNo: r.yevmiyeNo }; });
            var $d = $('#btnIptal').prop('disabled', true);
            Sgb.gonder(UC + 'iptal', { Makbuzlar: makbuzlar })
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata || 'İptal işlemi yapılamadı.', 'danger'); return; }
                    mesaj(s.bilgi, 'success');
                    listele(true);
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // ---------------------------------------------------------------- raporlar

    function rapor(adres, $tus) {
        var k = kriter();
        if (!kriterDenetle(k)) return;
        $tus.prop('disabled', true);
        Sgb.indir(UC + adres, k).then(function () { $tus.prop('disabled', false); });
    }

    $('#btnKesilen').on('click', function () { rapor('rapor/liste', $(this)); });
    $('#btnRaporHesap').on('click', function () { rapor('rapor/hesap', $(this)); });

    // ---------------------------------------------------------------- ödeme emri

    $('#btnOdemeEmri').on('click', function () {
        var k = kriter();
        if (!kriterDenetle(k)) return;
        if (!k.muhasebe) { mesaj('Lütfen muhasebe seçiniz.', 'warning'); return; }
        sor('Verilen kriterlere uygun makbuzlar için Ödeme Emri belgesi oluşturulacaktır. Onaylıyor musunuz?', function () {
            var $d = $('#btnOdemeEmri').prop('disabled', true);
            grid.setGridOption('loading', true);
            Sgb.gonder(UC + 'odeme-emri', { Bas: k.bas, Bit: k.bit, Muhasebe: k.muhasebe })
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata || 'Ödeme emri oluşturulamadı.', 'danger'); return; }
                    var $l = $('#lstSonuc').empty();
                    $.each(s.mesajlar || [], function (i, m) { $('<li></li>').text(m).appendTo($l); });
                    document.getElementById('dlgSonuc').showModal();
                    if (listelendi) listele(true);
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
        });
    });

    // ---------------------------------------------------------------- açılış

    // Açılış (eski Page_Load): tarihler bugün, kullanıcının muhasebe / makbuz birimi, durum Yeni. Liste boş başlar.
    Sgb.al(UC + 'tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            $('#txtBasla, #txtBitis').val(v.bugun);
            var $t = $('#ddlMakbuzTipi').empty();
            $.each(v.makbuzTipleri || [], function (i, t) { $('<option></option>').val(t.kod).text(t.aciklama).appendTo($t); });
            var $du = $('#ddlDurum').empty();
            $.each(v.durumlar || [], function (i, t) { $('<option></option>').val(t.kod).text(t.ad).appendTo($du); });
            $du.val(String(v.durum));
            if (v.muhasebe) muhBirim.ata({ muhasebe: v.muhasebe, birim: v.birim, kurumsal: v.kurumsal, birimAd: v.birimAd });
        })
        .fail(hataGoster);
});
