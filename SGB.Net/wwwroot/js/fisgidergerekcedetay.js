// Gider Cetveli Detay Açıklamalar (KesinHesap/FisGiderGerekceDetay; faaliyet bazında açıklama): yıl ve kurumsal / ekonomik /
// faaliyet kodlarına göre tertipler listelenir; seçilen tertibin K.B.Ö., harcanan tutarı ve açıklaması okunur, kaydedilir,
// SGB'ye gönderilir. Yazdır kıstasa uyan açıklamaları Word olarak verir; sistem yöneticisi kıstasa uyanların onayını kaldırabilir.
$(function () {
    'use strict';

    if (!$('#txtGerekce').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var acik = null;     // okunmuş tertip { yil, tertip }: Kaydet / Gönder bunu kullanır
    var degisti = false; // açıklamada kaydedilmemiş değişiklik var mı?
    var sayi = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var $gerekce = $('#txtGerekce');

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function kistas() {
        return { yil: yilOku(), kur: $('#txtKur').val() || '', eko: $('#txtEko').val() || '', prog: $('#txtProg').val() || '' };
    }
    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }

    function dugmeler() {
        $('#btnKaydet, #btnGonder').prop('disabled', !acik);
        $gerekce.prop('disabled', !acik);
    }

    function aciklamaTemizle() {
        acik = null;
        degisti = false;
        $('#txtBaslik').text('Açıklama');
        $('#txtKBO, #txtHarcanan').val('');
        $gerekce.val('');
        dugmeler();
    }

    function vazgecilsinMi() {
        return !degisti || window.confirm('Kaydedilmemiş değişiklikler var. Devam edilirse kaybolacak. Devam edilsin mi?');
    }

    $gerekce.on('input', function () { degisti = true; });
    window.addEventListener('beforeunload', function (olay) {
        if (degisti) { olay.preventDefault(); olay.returnValue = ''; }
    });

    // ---------------------------------------------------------------- tertip listesi (AG Grid)

    var secimDegisiyor = false;
    var grid = Sgb.grid('#grdTertipler', {
        columnDefs: [
            { field: 'tertip', headerName: 'Tertip', flex: 2, minWidth: 190 },
            { field: 'ekoAd', headerName: 'Eko Ad', flex: 1, minWidth: 120, tooltipField: 'ekoAd' },
            { field: 'damgaSay', headerName: 'Damga', width: 80, type: 'rightAligned' },
            { field: 'onayKod', headerName: 'Onay', width: 76, type: 'rightAligned' }
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        getRowId: function (p) { return p.data.tertip; },
        overlayNoRowsTemplate: '<span class="bos-ipucu">Kodları seçip Listele\'ye basınız.</span>',
        onSelectionChanged: function () {
            if (secimDegisiyor) return;
            var s = grid.getSelectedRows()[0];
            if (!s) return;
            if (acik && acik.tertip === s.tertip) return;
            if (!vazgecilsinMi()) {
                secimDegisiyor = true;
                grid.deselectAll();
                if (acik) { var n = grid.getRowNode(acik.tertip); if (n) n.setSelected(true); }
                secimDegisiyor = false;
                return;
            }
            tertipOku(s);
        }
    });

    function listele(seciliKalsin) {
        var k = kistas();
        if (k.yil < 2007) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/fisgidergerekcedetay/liste', k)
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                var satirlar = veri.tertipler || [];
                grid.setGridOption('overlayNoRowsTemplate', '<span class="bos-ipucu">Kıstasa uyan tertip yok.</span>');
                grid.setGridOption('rowData', satirlar);
                $('#listeSayi').text('(' + satirlar.length + ')');
                if (seciliKalsin && acik) {
                    var n = grid.getRowNode(acik.tertip);
                    secimDegisiyor = true;
                    if (n) { n.setSelected(true); grid.ensureNodeVisible(n); }
                    secimDegisiyor = false;
                    if (!n) aciklamaTemizle();
                } else aciklamaTemizle();
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); $d.prop('disabled', false); });
    }

    $('#btnListele').on('click', function () { if (vazgecilsinMi()) listele(false); });
    $('.kod-alan input, #txtYil').on('keydown', function (e) {
        if (e.key === 'Enter') { e.preventDefault(); $('#btnListele').trigger('click'); }
    });

    // ---------------------------------------------------------------- seçilen tertip (eski TertipOku)

    function tertipOku(satir) {
        var o = { yil: yilOku(), tertip: satir.tertip };
        aciklamaTemizle();
        $('#txtBaslik').text(o.yil + ' ' + satir.tertip + ' - ' + (satir.ekoAd || ''));
        $gerekce.attr('placeholder', 'Okunuyor...');
        Sgb.al('api/fisgidergerekcedetay/oku', o)
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                acik = o;
                $('#txtKBO').val(sayi.format(veri.kbo || 0));
                $('#txtHarcanan').val(sayi.format(veri.harcanan || 0));
                $gerekce.val(veri.gerekce || '');
                degisti = false;
            })
            .fail(hataGoster)
            .always(function () { $gerekce.attr('placeholder', 'Listeden tertip seçiniz.'); dugmeler(); });
    }

    // ---------------------------------------------------------------- kaydet / gönder

    function kayit(adres, $d) {
        $d.prop('disabled', true);
        return Sgb.gonder(adres, { yil: acik.yil, tertip: acik.tertip, gerekce: $gerekce.val() })
            .done(function (veri) {
                if (!veri.basarili) { mesaj(veri.hata, 'danger'); return; }
                degisti = false;
                mesaj(veri.bilgi, 'success');
                listele(true); // damga ve onay sütunları yenilenir
            })
            .fail(hataGoster)
            .always(function () { dugmeler(); });
    }

    $('#btnKaydet').on('click', function () {
        if (!acik || !window.confirm('Kaydetmek istiyor musunuz?')) return;
        kayit('api/fisgidergerekcedetay/kaydet', $(this));
    });

    $('#btnGonder').on('click', function () {
        if (!acik) return;
        if (!window.confirm('Gerekçe SGB\'ye gönderilecek. Gönderildikten sonra gerekçe metni değiştirilemez. Onaylıyor musunuz?')) return;
        kayit('api/fisgidergerekcedetay/gonder', $(this));
    });

    // ---------------------------------------------------------------- yazdır, onay kaldır

    $('#btnYazdir').on('click', function () {
        var k = kistas();
        if (k.yil < 2007) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/fisgidergerekcedetay/rapor', k).finally(function () { $d.prop('disabled', false); });
    });

    $('#btnOnayKaldir').on('click', function () {
        var k = kistas();
        if (k.yil < 2007) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!window.confirm('Seçilen kodlara uyan bütün açıklamaların onayı kaldırılacak. Onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/fisgidergerekcedetay/onaykaldir', k)
            .done(function (veri) {
                mesaj(veri.basarili ? veri.bilgi : veri.hata, veri.basarili ? 'success' : 'danger');
                if (veri.basarili && grid.getDisplayedRowCount()) listele(true);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- kod ağaçları (Sgb.agac)

    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};  // tür -> { $ul, agac, yil }
    var aktifAlan = null;

    function agacAc($alan) {
        var yil = yilOku();
        if (yil < 2007) { mesaj('Yıl hatalı!', 'warning'); return; }
        var tur = $alan.attr('data-tur');
        var ad = $alan.find('label').text();
        aktifAlan = $alan;
        $('#dlgKodBaslik').text(ad + ' Kod Seçimi');
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[tur];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', ad + ' kod ağacı').appendTo('#agacKutu'), yil: yil };
            // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al('api/fisgidergerekcedetay/kodlar', { yil: a.yil, tur: tur, ust: ust ? ust.kod : '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    dlgKod.close();
                    if (aktifAlan) aktifAlan.find('input').val(d.ad).trigger('change');
                },
                seciliMi: function (d) { return !!aktifAlan && kodAl(aktifAlan.find('input').val()) === d.kod; },
                ilkAcik: true,
                bosMetin: 'Kod bulunamadı.',
                hata: hataGoster
            });
        } else {
            a.$ul.prop('hidden', false);
            if (a.yil !== yil) { a.yil = yil; a.agac.yenile(); }
        }
    }

    $('.kod-alan .kod-sec').on('click', function () { agacAc($(this).closest('.kod-alan')); });
    $('.kod-alan .kod-temizle').on('click', function () { $(this).closest('.kod-alan').find('input').val('').trigger('focus'); });
    // Alana kod yazılabilir; Alt+Aşağı ağacı açar.
    $('.kod-alan input').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
    });
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al('api/fisgidergerekcedetay/tanim')
        .done(function (veri) {
            if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
            $('#txtYil').attr({ min: veri.enAz, max: veri.enCok }).val(veri.yil);
            $('#btnOnayKaldir').prop('hidden', !veri.yonetici);
        })
        .fail(hataGoster);

    $('#txtYil').on('change', function () {
        if (!vazgecilsinMi()) return;
        grid.setGridOption('rowData', []);
        $('#listeSayi').text('');
        aciklamaTemizle();
    });

    dugmeler();
});
