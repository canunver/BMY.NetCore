// Hesap Planı Bilgileri Tanımlama (ButceMuhasebe/TanimHesapPlani): solda seçili hesabın bilgileri, nitelikleri, döviz ayarları
// ve yansıtmaları (eski TanimYansitma.aspx), sağda Bul ile listelenen hesaplar. Toplu Nitelik Değiştirme ayrı penceredir (eski HesapPlaniNitelik.aspx).
// Uçlar: /api/muhtanim/hesapplani/* ve /api/muhtanim/excel-aktar.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return;

    var sonArama = null;

    function nitelikKutulari(kutu, nitelikler) {
        var $k = $(kutu).empty();
        $.each(nitelikler, function (i, n) {
            $('<label></label>').append($('<input type="checkbox" />').val(n.kod)).append(document.createTextNode(' ' + n.ad)).appendTo($k);
        });
    }
    function nitelikOku(kutu) {
        var s = '';
        $(kutu).find('input:checked').each(function () { s += this.value + ';'; });
        return s;
    }
    function nitelikYaz(kutu, deger) {
        var kodlar = $.map(String(deger || '').split(';'), function (x) { var n = parseInt(x, 10); return n > 0 ? String(n) : null; });
        $(kutu).find('input').each(function () { this.checked = $.inArray(this.value, kodlar) >= 0; });
    }

    function tamSayi(x, bos) { var n = parseInt(x, 10); return isNaN(n) ? bos : n; }

    var sayfa = Sgb.mt.tanim({
        tablo: '#grdListe',
        form: '#frmBilgi',
        uc: 'api/muhtanim/hesapplani/',
        kolonlar: [
            { field: 'hesapKod', headerName: 'Hesap Kodu', width: 140, pinned: 'left' },
            { field: 'aciklama', headerName: 'Açıklama', width: 260, tooltipField: 'aciklama' },
            { field: 'tertipAd', headerName: 'Tertip', width: 120 },
            { field: 'kesintiAd', headerName: 'Kesinti', width: 90 },
            { field: 'altHesapAd', headerName: 'Alt Hesap Kullanır mı?', width: 120 },
            { field: 'bakiyeAd', headerName: 'Çalışma Şekli', width: 160 },
            { field: 'nitelikAd', headerName: 'Nitelik', width: 260, tooltipField: 'nitelikAd' },
            { field: 'dovizKurKod', headerName: 'Kur Adı', width: 90 },
            { field: 'satisKarsilikTur', headerName: 'Satış Karşılık Tür', width: 120 },
            { field: 'alisKarsilikTur', headerName: 'Alış Karşılık Tür', width: 120 },
            { field: 'alacakHesabi', headerName: 'Kur Farkı Alacak Hesabı', width: 150 },
            { field: 'borcHesabi', headerName: 'Kur Farkı Borç Hesabı', width: 150 }
        ],
        anahtar: 'hesapKod',
        listeParam: function () { return sonArama; },
        varsayilan: function () { return { yil: $('#hpYil').val() || Sgb.mt.buYil, ekoBasla: '0' }; },
        satirdanForma: function (s) {
            nitelikYaz('#hpNitelik', s.nitelik);
            return s;
        },
        secilince: function (s) { yansitmaOku(s.yil, s.hesapKod); },
        temizlenince: function () { nitelikYaz('#hpNitelik', ''); yansitmaOku(0, ''); },
        istek: function (f) {
            return {
                yil: tamSayi(f.yil, 0), hesapKod: f.hesapKod, aciklama: f.aciklama,
                tertip: tamSayi(f.tertip, -1), kesinti: tamSayi(f.kesinti, -1), altHesap: tamSayi(f.altHesap, -1), bakiye: tamSayi(f.bakiye, -1),
                nitelik: nitelikOku('#hpNitelik'), ekoBasla: tamSayi(f.ekoBasla, 0),
                dovizKurKod: f.dovizKurKod, satisKarsilikTur: f.satisKarsilikTur, alisKarsilikTur: f.alisKarsilikTur,
                alacakHesabi: f.alacakHesabi, borcHesabi: f.borcHesabi
            };
        },
        silIstek: function (f) { return f.hesapKod ? { yil: tamSayi(f.yil, 0), hesapKod: f.hesapKod } : null; }
    });
    sayfa.tablo.setGridOption('overlayNoRowsTemplate', '<span class="ag-overlay-no-rows-center">Hesap kodunu yazıp Bul\'a basın.</span>');

    $('#hpYil').val(Sgb.mt.buYil);
    $('#hpEko').val('0');

    $('#btnBul').on('click', function () {
        var f = sayfa.form.oku();
        if (!f.hesapKod) { Sgb.bildirim('Lütfen hesap Kodu alanını boş bırakmayın', 'warning'); return; }
        sonArama = {
            yil: tamSayi(f.yil, 0), hesapKod: f.hesapKod, aciklama: f.aciklama,
            tertip: tamSayi(f.tertip, -1), kesinti: tamSayi(f.kesinti, -1), altHesap: tamSayi(f.altHesap, -1), bakiye: tamSayi(f.bakiye, -1),
            nitelik: nitelikOku('#hpNitelik')
        };
        sayfa.listele(sonArama);
    });

    // ---------------------------------------------------------------- yansıtmalar

    var ysHesap = { yil: 0, hesapKod: '' }; // yansıtmaları listelenen hesap

    var ysTablo = Sgb.grid('#grdYansitma', {
        columnDefs: [
            { field: 'siraNo', headerName: 'Sıra No', width: 85, type: 'rightAligned', sort: 'asc' },
            { field: 'yansitmaHesapKod', headerName: 'Yansıtma Hesabı', width: 150 },
            { field: 'aciklama', headerName: 'Açıklama', flex: 1, minWidth: 140, tooltipField: 'aciklama' },
            {
                headerName: '', width: 60, sortable: false, resizable: false,
                cellRenderer: function () {
                    return '<button type="button" class="btn btn-danger btn-xs mt-ys-sil" title="Yansıtmayı sil"><span class="glyphicon glyphicon-trash" aria-hidden="true"></span><span class="sr-only">Sil</span></button>';
                },
                onCellClicked: function (p) { if ($(p.event.target).closest('.mt-ys-sil').length) yansitmaSil(p.data); }
            }
        ],
        overlayNoRowsTemplate: '<span class="ag-overlay-no-rows-center">Listeden bir hesap seçin.</span>'
    });

    function yansitmaOku(y, hesapKod) {
        ysHesap = { yil: tamSayi(y, 0), hesapKod: hesapKod || '' };
        $('#ysHesap').text(ysHesap.hesapKod ? ysHesap.yil + ' / ' + ysHesap.hesapKod : '');
        $('#ysSayi').text('');
        ysTablo.setGridOption('overlayNoRowsTemplate', '<span class="ag-overlay-no-rows-center">' +
            (ysHesap.hesapKod ? 'Bu hesabın yansıtması yok.' : 'Listeden bir hesap seçin.') + '</span>');
        if (!ysHesap.yil || !ysHesap.hesapKod) { ysTablo.setGridOption('rowData', []); return; }
        Sgb.mt.oku('api/muhtanim/hesapplani/yansitma', ysHesap, ysTablo).done(function (v) {
            var satirlar = v.satirlar || [];
            ysTablo.setGridOption('rowData', satirlar);
            $('#ysSayi').text(satirlar.length ? String(satirlar.length) : '');
        });
    }

    $('#btnYansitmaEkle').on('click', function () {
        var y = yil(), hesap = $.trim($('#hpHesap').val()), kod = $.trim($('#ysKod').val());
        if (!y || !hesap) { Sgb.bildirim('Önce yıl ve hesap kodunu girin ya da listeden bir hesap seçin.', 'warning'); return; }
        if (!kod) { Sgb.bildirim('Yansıtma hesap kodunu girin.', 'warning'); $('#ysKod').trigger('focus'); return; }
        var hk = $('#ysKod').data('hesapKodu');
        if (hk && !hk.gecerli()) { Sgb.bildirim('Yansıtma hesabı hesap planında kayıtlı ve son seviye olmalı.', 'warning'); return; }
        Sgb.mt.islem('api/muhtanim/hesapplani/yansitma-ekle', { yil: y, hesapKod: hesap, yansitmaHesapKod: kod }).done(function () {
            var hk2 = $('#ysKod').data('hesapKodu');
            if (hk2) hk2.temizle(); else $('#ysKod').val('');
            yansitmaOku(y, hesap);
        });
    });
    $('#ysKod').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); $('#btnYansitmaEkle').trigger('click'); } });

    function yansitmaSil(s) {
        Sgb.mt.islem('api/muhtanim/hesapplani/yansitma-sil',
            { yil: ysHesap.yil, hesapKod: ysHesap.hesapKod, yansitmaHesapKod: s.yansitmaHesapKod, siraNo: String(s.siraNo) },
            ysHesap.hesapKod + ' hesabının ' + s.yansitmaHesapKod + ' yansıtması silinecek. Emin misiniz?'
        ).done(function () { yansitmaOku(ysHesap.yil, ysHesap.hesapKod); });
    }

    // ---------------------------------------------------------------- aktarımlar

    function yil() { return tamSayi($('#hpYil').val(), 0); }

    $('#btnYansitmaExceleAktar').on('click', function (e) {
        e.preventDefault();
        var y = yil();
        if (!y) { Sgb.bildirim('Yıl bilgisi boş bırakılmış.', 'warning'); return; }
        Sgb.indir('api/muhtanim/hesapplani/yansitma-excel', { yil: y, hesapKod: $.trim($('#hpHesap').val()) });
    });
    $('#btnYansitmaExceldenAktar').on('click', function (e) {
        e.preventDefault();
        Sgb.mt.exceldenAktar('Yansitma', 'Excel sayfasının sütunları sırasıyla: Yıl, Hesap Kodu, Yansıtma Hesap Kodu, Sıra No. İlk satırdan başlanır; Yıl sütunu boş olan satırda durulur.',
            function () { yansitmaOku(ysHesap.yil, ysHesap.hesapKod); });
    });
    $('#btnYansitmaSonrakiYil').on('click', function (e) {
        e.preventDefault();
        var y = yil();
        if (!y) { Sgb.bildirim('Yıl bilgisi boş bırakılmış.', 'warning'); return; }
        Sgb.mt.islem('api/muhtanim/hesapplani/yansitma-sonraki-yil', { yil: y }, y + ' yılı yansıtma bilgileri ' + (y + 1) + ' yılına aktarılacak. Emin misiniz?');
    });

    $('#btnExceleAktar').on('click', function (e) {
        e.preventDefault();
        Sgb.indir('api/muhtanim/hesapplani/excel', { yil: yil(), hesapKod: $.trim($('#hpHesap').val()) });
    });
    $('#btnExceldenAktar').on('click', function (e) {
        e.preventDefault();
        Sgb.mt.exceldenAktar('HesapPlani', 'Excel sayfasının sütunları sırasıyla: Yıl, Hesap Kodu, Açıklama, Tertip, Kesinti, Nitelik. İlk satırdan başlanır; Yıl sütunu boş olan satırda durulur.');
    });
    $('#btnSonrakiYil').on('click', function (e) {
        e.preventDefault();
        var y = yil();
        if (!y) { Sgb.bildirim('Yıl bilgisi boş bırakılmış.', 'warning'); return; }
        Sgb.mt.islem('api/muhtanim/hesapplani/sonraki-yil', { yil: y }, y + ' yılı hesap planı ' + (y + 1) + ' yılına aktarılacak. Emin misiniz?');
    });
    $('#btnDetayGuncelle').on('click', function (e) {
        e.preventDefault();
        var y = yil();
        if (!y) { Sgb.bildirim('Yıl bilgisi boş bırakılmış.', 'warning'); return; }
        Sgb.mt.islem('api/muhtanim/hesapplani/detay-guncelle', { yil: y }, y + ' yılı hesap planının detay sayıları güncellenecek. Emin misiniz?');
    });
    $('#btnBirimDoldur').on('click', function (e) {
        e.preventDefault();
        Sgb.mt.islem('api/muhtanim/hesapplani/birim-doldur', {}, 'Detay birim bilgileri doldurulacak. Emin misiniz?');
    });

    // ---------------------------------------------------------------- toplu nitelik değiştirme

    var pencere = Sgb.mt.pencere('#dlgNitelik');
    var nForm = Sgb.mt.form('#frmNitelik');

    $('#btnNitelik').on('click', function (e) {
        e.preventDefault();
        nForm.temizle({ yil: $('#hpYil').val() || Sgb.mt.buYil, hesapKod: $.trim($('#hpHesap').val()), ekoBasla: '-2', islem: '0' });
        nitelikYaz('#tnNitelik', '');
        pencere.showModal();
    });

    $('#btnNitelikGuncelle').on('click', function () {
        var f = nForm.oku();
        var istek = {
            yil: tamSayi(f.yil, 0), hesapKod: f.hesapKod,
            tertip: tamSayi(f.tertip, -1), kesinti: tamSayi(f.kesinti, -1), altHesap: tamSayi(f.altHesap, -1), bakiye: tamSayi(f.bakiye, -1),
            ekoBasla: tamSayi(f.ekoBasla, -2), islem: tamSayi(f.islem, 0), nitelik: nitelikOku('#tnNitelik')
        };
        if (!istek.yil || !istek.hesapKod) { Sgb.bildirim('Yıl ve hesap kodu girilmelidir.', 'warning'); return; }
        Sgb.mt.islem('api/muhtanim/hesapplani/nitelik-degistir', istek,
            istek.hesapKod + ' ile başlayan hesapların nitelikleri ' + (istek.islem === 1 ? 'kaldırılacak' : 'eklenecek') + '. Emin misiniz?');
    });

    // ---------------------------------------------------------------- seçenekler

    Sgb.mt.oku('api/muhtanim/hesapplani/secenekler').done(function (v) {
        nitelikKutulari('#hpNitelik', v.nitelikler || []);
        nitelikKutulari('#tnNitelik', v.nitelikler || []);
        var $kur = $('#hpKur');
        $.each(v.kurlar || [], function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($kur); });
        $('#liBirimDoldur').prop('hidden', !v.birimDoldur);
    });
});
