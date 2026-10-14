// Yatırım Projeleri Yönetim (ButceHazirlikv2/YatProjeYonetim): sekmeler hâlinde
//   Aşama Kopyalama : seçilen projelerin bilgilerini bir yılın aşamasından başka bir yılın aşamasına kopyalar,
//   Onaylama        : proje tanımı (yıl + aşama) ve proje yılı için veri girişini açar / kapatır (eski YATProjeOnay),
//   Projeler / Alt Projeler / Kaynaklar : yatırım sözlükleri; listeleme, ad ekleme ve panoya kopyalama (eski YATTanimSozluk).
$(function () {
    'use strict';

    if (!$('#grdProjeler').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var tamsayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });
    var tanim = null; // açılış bilgisi { yil, buYil, asamalar, yonetici }

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function secenekler($ddl, liste, secili) {
        $ddl.empty();
        $.each(liste, function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($ddl); });
        if (secili !== undefined && secili !== null) $ddl.val(secili);
        if ($ddl.val() === null && $ddl.children().length) $ddl.prop('selectedIndex', 0);
    }

    function yilOku(el) { return parseInt($(el).val(), 10) || 0; }

    function asamaAd(kod) {
        var a = $.grep((tanim && tanim.asamalar) || [], function (x) { return x.kod === kod; })[0];
        return a ? a.ad : (kod || '');
    }

    // ---------------------------------------------------------------- onay penceresi

    var dlgOnay = document.getElementById('dlgOnay');
    var onaylaninca = null;
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    function onayIste(baslik, metin, dugme, islem) {
        $('#dlgOnayBaslik').text(baslik);
        $('#onayMetin').text(metin);
        $('#btnOnayla').text(dugme);
        onaylaninca = islem;
        dlgOnay.showModal();
    }

    $('#btnOnayla').on('click', function () {
        var islem = onaylaninca;
        onaylaninca = null;
        dlgOnay.close();
        if (islem) islem();
    });

    // ---------------------------------------------------------------- sekmeler

    var acilinca = {}; // sekme ilk açıldığında yapılacak iş

    $('.yat-sekmeler').on('click', 'a', function (olay) {
        olay.preventDefault();
        var hedef = $(this).attr('href');
        $('.yat-sekmeler li').removeClass('active').find('a').attr('aria-selected', 'false');
        $(this).attr('aria-selected', 'true').parent().addClass('active');
        $('.yat-panel').prop('hidden', true);
        $(hedef).prop('hidden', false);
        if (acilinca[hedef]) {
            var is = acilinca[hedef];
            delete acilinca[hedef];
            if (tanim) is();
            else acilinca[hedef] = is; // açılış bilgisi gelince yapılır
        }
    });

    // ---------------------------------------------------------------- aşama kopyalama

    var grdProjeler = Sgb.grid('#grdProjeler', {
        columnDefs: [
            { field: 'kod', headerName: 'Kod', width: 90, type: 'rightAligned' },
            { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 200, tooltipField: 'ad' }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
        getRowId: function (p) { return String(p.data.kod); },
        overlayNoRowsTemplate: '<span>Kaynak yılı ve aşamayı seçip Projeleri Yenile\'ye basınız.</span>'
    });

    function projeleriOku() {
        var o = { yil: yilOku('#txtKaynakYil'), asama: $('#ddlKaynakAsama').val() || '' };
        if (o.yil < 2000) { mesaj('Kaynak yıl hatalı!', 'warning'); return; }
        if (!o.asama) { mesaj('Kaynak aşamayı seçiniz.', 'warning'); return; }
        var $d = $('#btnProjeYenile').prop('disabled', true);
        grdProjeler.setGridOption('loading', true);
        Sgb.al('api/yatproje/projeler', o)
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                grdProjeler.setGridOption('overlayNoRowsTemplate', '<span>Bu yıl ve aşamada proje yok.</span>');
                grdProjeler.setGridOption('rowData', veri.satirlar);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grdProjeler.setGridOption('loading', false); });
    }

    $('#btnProjeYenile').on('click', projeleriOku);

    function sonucYaz(metin, tur) {
        $('#kopyaSonuc').removeClass('alert-success alert-warning alert-danger').addClass('alert-' + tur).text(metin || '').prop('hidden', !metin);
    }

    function kopyaBilgisi() {
        var secimler = [];
        $('#frmKopya [data-secim]').each(function () { secimler[parseInt($(this).attr('data-secim'), 10)] = this.checked; });
        return {
            kaynakYil: yilOku('#txtKaynakYil'),
            kaynakAsama: $('#ddlKaynakAsama').val() || '',
            hedefYil: yilOku('#txtHedefYil'),
            hedefAsama: $('#ddlHedefAsama').val() || '',
            secimler: secimler,
            projeler: $.map(grdProjeler.getSelectedRows(), function (r) { return r.kod; })
        };
    }

    function kopyaHatasi(k) {
        var h = [];
        if (k.kaynakYil < 2000) h.push('Kaynak yıl hatalı!');
        if (k.hedefYil < 2000) h.push('Hedef yıl hatalı!');
        if (!k.kaynakAsama) h.push('Kaynak aşama boş olamaz.');
        if (!k.hedefAsama) h.push('Hedef aşama boş olamaz.');
        if (!h.length && k.kaynakYil === k.hedefYil && k.kaynakAsama === k.hedefAsama) h.push('Kaynak ve hedef aynı olamaz!');
        if ($.inArray(true, k.secimler) < 0) h.push('Kopyalanacak bilgileri işaretleyiniz.');
        if (!k.projeler.length) h.push('Kopyalanacak projeleri seçiniz!');
        return h.join(' ');
    }

    $('#btnKopyala').on('click', function () {
        var k = kopyaBilgisi(), hata = kopyaHatasi(k);
        if (hata) { mesaj(hata, 'warning'); return; }
        var neler = $('#frmKopya [data-secim]:checked').map(function () { return $.trim($(this).parent().text()); }).get().join(', ');
        var metin = k.projeler.length + ' projenin ' + neler.toLocaleLowerCase('tr-TR') + ' bilgileri ' + k.kaynakYil + ' yılı ' +
            asamaAd(k.kaynakAsama) + ' aşamasından ' + k.hedefYil + ' yılı ' + asamaAd(k.hedefAsama) +
            ' aşamasına kopyalanacak. Onaylıyor musunuz?';
        onayIste('Aşama Kopyalama', metin, 'Kopyala', function () {
            var $d = $('#btnKopyala').prop('disabled', true);
            sonucYaz('', 'success');
            Sgb.gonder('api/yatproje/kopyala', k)
                .done(function (s) {
                    if (!s.basarili) {
                        mesaj(s.hata, 'danger');
                        sonucYaz(s.hata + (s.bilgi ? '\n' + s.bilgi : ''), 'danger');
                        return;
                    }
                    mesaj('Kopyalama tamamlandı.', 'success');
                    sonucYaz(s.bilgi, 'success');
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // ---------------------------------------------------------------- onaylama

    function onayGrid(sec, asamali) {
        var sutunlar = [{ field: 'yil', headerName: 'Yıl', width: 90 }];
        if (asamali) sutunlar.push({ field: 'senaryoKod', headerName: 'Aşama', flex: 1, minWidth: 140, valueFormatter: function (p) { return asamaAd(p.value); } });
        sutunlar.push({ field: 'durumAd', headerName: 'Durum', flex: 1, minWidth: 150, cellClass: function (p) { return 'durum-' + p.data.durum; } });
        return Sgb.grid(sec, { columnDefs: sutunlar, overlayNoRowsTemplate: '<span>Kayıt yok.</span>' });
    }

    var grdOnayTanim = onayGrid('#grdOnayTanim', true);
    var grdOnayYil = onayGrid('#grdOnayYil', false);

    function onayOku() {
        grdOnayTanim.setGridOption('loading', true);
        grdOnayYil.setGridOption('loading', true);
        Sgb.al('api/yatproje/onay')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grdOnayTanim.setGridOption('rowData', v.tanim);
                grdOnayYil.setGridOption('rowData', v.yillar);
            })
            .fail(hataGoster)
            .always(function () {
                grdOnayTanim.setGridOption('loading', false);
                grdOnayYil.setGridOption('loading', false);
            });
    }

    acilinca['#sekOnay'] = onayOku;

    $('#sekOnay').on('click', '[data-yer]', function () {
        var yer = parseInt($(this).attr('data-yer'), 10);
        var o = yer <= 3
            ? { yer: yer, yil: yilOku('#txtOnayYil'), asama: $('#ddlOnayAsama').val() || '' }
            : { yer: yer, yil: yilOku('#txtOnayYil2'), asama: '' };
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (yer <= 3 && !o.asama) { mesaj('Aşamayı seçiniz.', 'warning'); return; }
        var islemAd = $.trim($(this).text());
        var ne = yer <= 3 ? o.yil + ' yılı ' + asamaAd(o.asama) + ' aşaması proje tanımları' : o.yil + ' yılı proje bilgileri';
        onayIste('Onay', ne + ' için "' + islemAd + '" işlemi yapılacak. Onaylıyor musunuz?', islemAd, function () {
            var $d = $('#sekOnay [data-yer]').prop('disabled', true);
            Sgb.gonder('api/yatproje/onay', o)
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) onayOku();
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', !tanim.yonetici); });
        });
    });

    // ---------------------------------------------------------------- sözlükler

    var dlgEkle = document.getElementById('dlgEkle');
    var ekleSozluk = null; // ekleme penceresi açıkken hangi sözlüğe

    function sozlukKur(panel) {
        var $p = $(panel);
        var tur = parseInt($p.attr('data-sozluk'), 10);
        var tekil = $p.attr('data-tekil');
        var $form = $p.find('form');
        var grid = Sgb.grid($p.find('[data-grid]')[0], {
            columnDefs: [
                { field: 'kod', headerName: 'Kod', width: 80, type: 'rightAligned' },
                { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 240, tooltipField: 'ad' },
                { field: 'aciklama', headerName: 'Aşama Ad', flex: 1, minWidth: 240, tooltipField: 'aciklama' },
                { field: 'say', headerName: 'Bağlı kayıt sayısı', width: 150, type: 'rightAligned', valueFormatter: function (p) { return tamsayi.format(p.value || 0); } }
            ],
            overlayNoRowsTemplate: '<span>Yılı ve aşamayı seçip Listele\'ye basınız.</span>'
        });

        function listele() {
            var o = { tur: tur, yil: yilOku($form.find('[data-alan="yil"]')), asama: $form.find('[data-alan="asama"]').val() || '' };
            if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
            if (!o.asama) { mesaj('Aşamayı seçiniz.', 'warning'); return; }
            var $d = $form.find('button[type="submit"]').prop('disabled', true);
            grid.setGridOption('loading', true);
            Sgb.al('api/yatproje/sozluk', o)
                .done(function (veri) {
                    if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                    grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt yok.</span>');
                    grid.setGridOption('rowData', veri.satirlar);
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
        }

        $form.on('submit', function (olay) { olay.preventDefault(); listele(); });

        // Eski CopyStore: tablo (başlıklarla) sekmeyle ayrılmış metin olarak panoya; Excel'e yapıştırılabilir.
        $form.on('click', '[data-islem="kopyala"]', function () {
            var satirlar = [];
            grid.forEachNodeAfterFilterAndSort(function (n) { satirlar.push(n.data); });
            if (!satirlar.length) { mesaj('Kopyalanacak satır yok.', 'warning'); return; }
            var temiz = function (d) { return String(d == null ? '' : d).replace(/[\t\r\n]+/g, ' '); };
            var metin = 'Kod\tAd\tAşama Ad\tBağlı kayıt sayısı\r\n' + $.map(satirlar, function (s) {
                return [s.kod, temiz(s.ad), temiz(s.aciklama), s.say || 0].join('\t') + '\r\n';
            }).join('');
            var tamam = function () { mesaj(satirlar.length + ' satır panoya kopyalandı.', 'success'); };
            if (navigator.clipboard && window.isSecureContext) {
                navigator.clipboard.writeText(metin).then(tamam, function () { mesaj('Panoya kopyalanamadı.', 'danger'); });
            } else {
                var $t = $('<textarea readonly></textarea>').val(metin).css({ position: 'fixed', left: '-9999px' }).appendTo(document.body);
                $t[0].select();
                try { document.execCommand('copy'); tamam(); } catch (e) { mesaj('Panoya kopyalanamadı.', 'danger'); }
                $t.remove();
            }
        });

        $form.on('click', '[data-islem="ekle"]', function () {
            ekleSozluk = { tur: tur, grid: grid };
            $('#dlgEkleBaslik').text('Yeni ' + tekil);
            $('#txtEkleAd').val('');
            dlgEkle.showModal();
            $('#txtEkleAd').trigger('focus');
        });
    }

    $('[data-sozluk]').each(function () { sozlukKur(this); });

    $('#frmEkle').on('submit', function (olay) {
        olay.preventDefault();
        if (!ekleSozluk) return;
        var ad = $.trim($('#txtEkleAd').val());
        if (!ad) { mesaj('Adı giriniz.', 'warning'); return; }
        var hedef = ekleSozluk;
        var $d = $('#btnEkleKaydet').prop('disabled', true);
        Sgb.gonder('api/yatproje/sozluk', { tur: hedef.tur, ad: ad })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                dlgEkle.close();
                ekleSozluk = null;
                hedef.grid.applyTransaction({ add: [{ kod: s.kod, ad: s.ad, aciklama: '', say: 0 }] });
                hedef.grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt yok.</span>');
                mesaj('Eklendi (kod: ' + s.kod + ').', 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/yatproje/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            var ilk = t.asamalar.length ? t.asamalar[0].kod : '';
            $('[data-asama]').each(function () { secenekler($(this), t.asamalar, ilk); });
            $('#txtKaynakYil, #txtHedefYil, [data-sozluk] [data-alan="yil"]').val(t.yil);
            $('#txtOnayYil, #txtOnayYil2').val(t.buYil);
            // Veri girişini açıp kapatmaya ve sözlüğe eklemeye iş kuralları yalnızca sistem yöneticisine izin verir.
            $('#sekOnay [data-yer], [data-islem="ekle"]').prop('disabled', !t.yonetici);
            $('#onayNot').prop('hidden', t.yonetici);
            // Açılış bilgisi gelmeden açılmış sekmenin işi
            var acik = '#' + $('.yat-panel:not([hidden])').attr('id');
            if (acilinca[acik]) { var is = acilinca[acik]; delete acilinca[acik]; is(); }
        })
        .fail(hataGoster);
});
