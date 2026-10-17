// Masraf Planı Düzenleme (ButceHazirlikv2/MasrafPlaniYatirimAyarla; eski MasrafPlaniYatirimAyarla.aspx, Masraf Planı'ndaki
// "MP Düzelt"): kodlarla düşülecek ve eklenecek tertip bulunur, düşülecek tertibin aylık masraf planına düşülecek tutarlar
// yazılır; İşlem Yap seçilen işleme göre masraf planı / AHP / tavan komutlarını üretip doğrudan veri tabanında çalıştırır.
// Uçlar: /api/mpayarla/* (ButceHazirlik/MPAyarlaUclari.cs). Yalnızca sistem yöneticisi.
$(function () {
    'use strict';

    if (!$('.mp-ayarla').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var API = 'api/mpayarla';
    var dusTertip = '', ekleTertip = '';

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // "1.234.567,89" -> 1234567.89
    function sayiOku(s) {
        if (typeof s === 'number') return isFinite(s) ? s : 0;
        s = $.trim(String(s || '')).replace(/\s/g, '');
        if (!s) return 0;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        else if ((s.match(/\./g) || []).length > 1) s = s.replace(/\./g, '');
        var v = parseFloat(s);
        return isFinite(v) ? v : 0;
    }

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }
    var kodlar = MPTavan.kodAlanlari(yilOku, hataGoster, API);
    $('#ddlYil').val(new Date().getFullYear());
    $('#ddlYil').on('change', function () { kodlar.adlariYenile(); });

    // ---------------------------------------------------------------- aylar (eski grdMpEkonomik)

    // Sarı salt okunur, beyaz veri girilen hücredir; toplam satırı kendi rengini korur.
    function saltOkunur(ek) {
        return function (p) { return p.node.rowPinned ? ek : ek.concat('salt'); };
    }
    function tutarYaz(p) { return MPTavan.tutar(p.value); }
    function sayiSutun(o) {
        return $.extend({ type: 'rightAligned', width: 150, cellClass: saltOkunur(['sayi']), valueFormatter: tutarYaz }, o);
    }

    var grid = Sgb.grid('#grdAylar', {
        columnDefs: [
            { field: 'ayAd', headerName: 'Ay', flex: 1, minWidth: 140, cellClass: saltOkunur([]) },
            sayiSutun({ field: 'odenek', headerName: 'Ödenek' }),
            sayiSutun({
                field: 'dusulen', headerName: 'Düşülecek', cellClass: ['sayi'],
                editable: function (p) { return !p.node.rowPinned; },
                valueParser: function (p) { return sayiOku(p.newValue); }
            }),
            sayiSutun({ colId: 'kalan', headerName: 'Kalan', valueGetter: function (p) { return p.data ? sayiOku(p.data.odenek) - sayiOku(p.data.dusulen) : null; } })
        ],
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        overlayNoRowsTemplate: '<span>Kodları girip Düşülecek Tertip\'e basınız.</span>',
        onCellValueChanged: toplamYenile
    });

    function satirlar() {
        var r = [];
        grid.forEachNode(function (n) { if (n.data) r.push(n.data); });
        return r;
    }

    function toplamYenile() {
        var r = satirlar(), t = { ayAd: 'T O P L A M', odenek: 0, dusulen: 0 };
        $.each(r, function (i, d) { t.odenek += sayiOku(d.odenek); t.dusulen += sayiOku(d.dusulen); });
        grid.setGridOption('pinnedBottomRowData', r.length ? [t] : []);
        $('#lblToplam').text(MPTavan.tutar(t.dusulen) || '0,00');
    }

    // ---------------------------------------------------------------- Düşülecek / Eklenecek Tertip (eski btnListele_Click)

    function tertipBul() {
        var tur = parseInt($(this).attr('data-tur'), 10);
        var yil = yilOku();
        if (yil <= 0) { mesaj('Yıl giriniz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(API + '/tertip', {
            Yil: yil, Kur: kodlar.kod('kur'), Fon: kodlar.kod('fon'), Fin: kodlar.kod('fin'), Eko: kodlar.kod('eko'), Peb: kodlar.kod('peb'),
            KurYapi: $.trim($('#txtKurYapi').val()), Tur: tur
        })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                if (tur === 1) {
                    dusTertip = v.tertip;
                    $('#lblDusTertip').text(v.tertip);
                    $('#lblDusBilgi').text(v.bilgi);
                    grid.setGridOption('rowData', v.aylar || []);
                    toplamYenile();
                } else {
                    ekleTertip = v.tertip;
                    $('#lblEkleTertip').text(v.tertip);
                    $('#lblEkleBilgi').text(v.bilgi);
                }
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    }
    $('#btnDusulecek, #btnEklenecek').on('click', tertipBul);

    // ---------------------------------------------------------------- İşlem Yap (eski btnKaydet_Click)

    $('#btnIslemYap').on('click', function () {
        grid.stopEditing();
        if (!dusTertip || !ekleTertip) { mesaj('Önce Düşülecek Tertip ve Eklenecek Tertip tuşlarıyla iki tertibi bulunuz.', 'warning'); return; }
        var aylar = $.map(satirlar(), function (d) { return { ay: d.ay, dusulen: sayiOku(d.dusulen) }; });
        if (aylar.length !== 12) { mesaj('Aylık masraf planı okunamadı; Düşülecek Tertip\'e yeniden basınız.', 'warning'); return; }
        var islemAd = $('#ddlIslem option:selected').text();
        if (!window.confirm('Yapılacak işlem (' + islemAd + ') onay durumuna bakmadan doğrudan veri tabanına kayıt yapacaktır. ' +
            'Geri dönüş için aynı işlemin tersini yapmanız gerekir. Devam edilsin mi?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(API + '/islem', {
            Yil: yilOku(), Islem: parseInt($('#ddlIslem').val(), 10) || 0, Dusulecek: dusTertip, Eklenecek: ekleTertip, Aylar: aylar
        })
            .done(function (v) {
                $('#txtKomut').val(v.komut || '');
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi || 'İşlem yapıldı.', 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // Soru işareti: açıklamayı gösterir / gizler.
    $('#btnYardim').on('click', function () {
        var $p = $('#pnlIpucu');
        var ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false').toggleClass('active', ac);
    });
});
