// Bütçe gider / gelir fişi (ButceHazirlikv2/ButceFisiGiris?odenekTur=1|2): sorgu alanlarındaki kodlara uyan tertipler listelenir,
// satıra tıklanınca tertibin fişi sağda gösterilir (ilk satır listelenince açılır). Gerekçe Düzenle gösterilen tertibin gerekçesini
// (giderde farklılaşma sebepleriyle) saklar; Gör. Yazdır gösterilen, Seç. Yazdır işaretli tertiplerin fişlerini eski koddaki Word
// (.doc) dosyası olarak indirir. Sorgu alanları tarayıcıda saklanır (eski sayfa oturumda tutuyordu).
$(function () {
    'use strict';

    var $sayfa = $('.butce-fisi');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TUR = parseInt($sayfa.attr('data-odenek-tur'), 10) === 2 ? 2 : 1;
    var GELIR = TUR === 2;
    var SAKLA = 'butcefisi.' + TUR + '.';

    var tanim = null;     // açılış bilgileri (api/butcefisi/tanim)
    var acik = null;      // listelenmiş seçim: { yil, senaryo }
    var gosterilen = '';  // sağda fişi gösterilen tertip

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur || 'info'); }

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function saklananOku(ad, varsayilan) {
        try { var d = window.localStorage.getItem(SAKLA + ad); return d === null ? varsayilan : d; } catch (e) { return varsayilan; }
    }
    function sakla(ad, deger) {
        try { window.localStorage.setItem(SAKLA + ad, deger); } catch (e) { /* tarayıcı izin vermiyor */ }
    }

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }

    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }

    function alanKod(alan) { return kodAl($('.kod-alan[data-alan="' + alan + '"] input').val()); }

    // ---------------------------------------------------------------- tertip listesi

    var grid = Sgb.grid('#grdTertip', {
        columnDefs: [
            { field: 'tertipKod', headerName: 'Tertip Kodu', width: 275, tooltipField: 'tertipKod', cellClass: 'tertip-kod' },
            { field: 'tertipAd', headerName: 'Açıklama', flex: 1, minWidth: 140, tooltipField: 'tertipAd' }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        getRowId: function (p) { return p.data.tertipKod; },
        rowClassRules: { 'satir-gosterilen': function (p) { return !!p.data && p.data.tertipKod === gosterilen; } },
        onRowClicked: function (e) {
            // Onay kutusuna tıklamak fişi açmasın.
            if (e.event && $(e.event.target).closest('.ag-selection-checkbox, .ag-checkbox').length) return;
            if (e.data) fisGoster(e.data);
        },
        overlayNoRowsTemplate: '<span>Listele ile tertipleri getiriniz.</span>'
    });

    function listeyiBosalt() {
        acik = null;
        grid.setGridOption('rowData', []);
        $('#lblSayi').text('');
        fisBosalt();
    }

    function listele() {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl seçiniz.', 'warning'); return; }
        var senaryo = $('#ddlSenaryo').val() || '';
        var p = { odenekTur: TUR, yil: yil, senaryo: senaryo, kur: alanKod('kur') };
        if (GELIR) p.gelEko = alanKod('gelEko');
        else { p.fon = alanKod('fon'); p.fin = alanKod('fin'); p.eko = alanKod('eko'); }

        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/butcefisi/liste', p).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            acik = { yil: yil, senaryo: senaryo };
            gosterilen = '';
            grid.setGridOption('rowData', v.tertipler || []);
            $('#lblSayi').text((v.tertipler || []).length + ' tertip');
            // Eski sayfadaki gibi ilk tertibin fişi açılır.
            if (v.tertipler && v.tertipler.length) fisGoster(v.tertipler[0]);
            else { fisBosalt(); mesaj('Seçime uyan tertip bulunamadı.', 'info'); }
        }).fail(hataGoster).always(function () {
            grid.setGridOption('loading', false);
            $d.prop('disabled', false);
        });
    }

    $('#frmSecim').on('submit', function (e) {
        e.preventDefault();
        $('.kod-alan').each(function () { sakla('kod.' + $(this).attr('data-alan'), $(this).find('input').val()); });
        listele();
    });

    $('#ddlYil, #ddlSenaryo').on('change', function () {
        sakla(this.id === 'ddlYil' ? 'yil' : 'senaryo', this.value);
        if (acik) listeyiBosalt();
    });

    // ---------------------------------------------------------------- fiş önizleme (eski ButceFisiDetay.aspx)

    var cerceve = document.getElementById('frmFis');

    function cerceveYaz(govde) {
        cerceve.srcdoc = '<!DOCTYPE html><html lang="tr"><head><meta charset="utf-8"><style>' +
            'body{margin:10px;font-family:"Times New Roman",serif;font-size:10pt;color:#000;background:#fff}' +
            'table{border-collapse:collapse}.bos{font-family:Arial,sans-serif;color:#777;font-size:13px}' +
            '</style></head><body>' + govde + '</body></html>';
    }

    function fisBosalt() {
        gosterilen = '';
        $('#lblTertip').text('Listeden bir tertip seçiniz.');
        cerceveYaz('<p class="bos">Fişini görmek istediğiniz tertibi listeden seçiniz.</p>');
        grid.redrawRows();
    }

    function fisGoster(satir) {
        if (!acik) return;
        gosterilen = satir.tertipKod;
        $('#lblTertip').text(satir.tertipKod + (satir.tertipAd ? '  ' + satir.tertipAd : ''));
        grid.redrawRows();
        var node = grid.getRowNode(satir.tertipKod);
        if (node) grid.ensureNodeVisible(node);
        cerceveYaz('<p class="bos">Fiş hazırlanıyor...</p>');
        var istenen = gosterilen;
        Sgb.al('api/butcefisi/fis', { odenekTur: TUR, yil: acik.yil, senaryo: acik.senaryo, tertip: istenen }).done(function (v) {
            if (istenen !== gosterilen) return; // bu arada başka tertip seçildi
            if (v.hata) { cerceveYaz('<p class="bos"></p>'); mesaj(v.hata, 'danger'); return; }
            cerceveYaz(v.html || '<p class="bos">Bu tertip için fiş bilgisi yok.</p>');
        }).fail(function (xhr) {
            if (istenen !== gosterilen) return;
            cerceveYaz('<p class="bos">Fiş gösterilemedi.</p>');
            hataGoster(xhr);
        });
    }

    // ---------------------------------------------------------------- Gerekçe Düzenle (eski ButceFisiGerekce.aspx)

    var dlgGerekce = document.getElementById('dlgGerekce');
    var editor = null;
    var gerekceTertip = '';

    $('#btnGerekce').on('click', function () {
        if (!acik || !gosterilen) { mesaj('Bu işlemi yapabilmek için önce tertip seçmelisiniz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        gerekceTertip = gosterilen;
        Sgb.al('api/butcefisi/gerekce', { odenekTur: TUR, yil: acik.yil, tertip: gerekceTertip }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            $('#lblGerekceTertip').text(gerekceTertip);
            dlgGerekce.showModal();
            if (!editor) editor = Sgb.editor('#txtGerekce', { yukseklik: 300 });
            editor.html(v.metin || '');
            $('#txtSebep1').val(v.sebep1 || '');
            $('#txtSebep2').val(v.sebep2 || '');
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    });

    $('#btnGerekceKaydet').on('click', function () {
        if (!acik || !gerekceTertip) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/butcefisi/gerekce', {
            odenekTur: TUR, yil: acik.yil, tertip: gerekceTertip, metin: editor ? editor.html() : '',
            sebep1: $('#txtSebep1').val() || '', sebep2: $('#txtSebep2').val() || ''
        }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            mesaj(v.bilgi || 'Kayıt işlemi başarıyla tamamlandı.', 'success');
            dlgGerekce.close();
            // Fişte gerekçe de yazıldığından gösterilen fiş yenilenir.
            var satir = grid.getRowNode(gerekceTertip);
            if (satir && gerekceTertip === gosterilen) fisGoster(satir.data);
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Yazdır (eski btnYazdir_Click: Word dosyası)

    function yazdir(tertipler, $d) {
        $d.prop('disabled', true);
        fetch(Sgb.KOK + 'api/butcefisi/yazdir', {
            method: 'POST', credentials: 'same-origin', cache: 'no-store',
            headers: { 'Content-Type': 'application/json', RequestVerificationToken: Sgb.meta('istek-jetonu') },
            body: JSON.stringify({ odenekTur: TUR, yil: acik.yil, senaryo: acik.senaryo, tertipler: tertipler })
        }).then(function (y) {
            if (y.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            var tur = y.headers.get('Content-Type') || '';
            if (!y.ok || tur.indexOf('json') >= 0 || tur.indexOf('text/html') >= 0) {
                if (tur.indexOf('json') >= 0) return y.json().then(function (v) { mesaj(v.hata || 'Bütçe fişi hazırlanamadı.', 'danger'); });
                mesaj(y.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Bütçe fişi hazırlanamadı.', 'danger');
                return;
            }
            var baslik = y.headers.get('Content-Disposition') || '';
            var ad = /filename\*=UTF-8''([^;]+)/i.exec(baslik) || /filename="?([^";]+)"?/i.exec(baslik);
            return y.blob().then(function (icerik) {
                var yerel = URL.createObjectURL(icerik);
                var a = document.createElement('a');
                a.href = yerel;
                a.download = ad ? decodeURIComponent(ad[1]) : 'BF.doc';
                document.body.appendChild(a);
                a.click();
                a.remove();
                setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
            });
        }).catch(function () { mesaj('Bütçe fişi hazırlanamadı.', 'danger'); })
            .then(function () { $d.prop('disabled', false); });
    }

    $('#btnGorYazdir').on('click', function () {
        if (!acik || !gosterilen) { mesaj('Bu işlemi yapabilmek için önce tertip seçmelisiniz.', 'warning'); return; }
        yazdir([gosterilen], $(this));
    });

    $('#btnSecYazdir').on('click', function () {
        var secili = acik ? grid.getSelectedRows().map(function (r) { return r.tertipKod; }) : [];
        if (!secili.length) { mesaj('Lütfen listeden seçim yapınız!', 'warning'); return; }
        yazdir(secili, $(this));
    });

    // ---------------------------------------------------------------- kod ağaçları

    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};  // tür -> { $ul, agac, yil }
    var aktifAlan = null;

    function agacAc($alan) {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl seçiniz.', 'warning'); return; }
        var tur = $alan.attr('data-tur');
        aktifAlan = $alan;
        $('#dlgKodBaslik').text($alan.find('label').text() + ' Kod Seçimi');
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[tur];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', $alan.find('label').text() + ' kod ağacı').appendTo('#agacKutu'), yil: yil };
            // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al('api/butcefisi/kodlar', { odenekTur: TUR, yil: a.yil, tur: tur, ust: ust ? ust.kod : '' }).then(function (v) {
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

    fisBosalt();
    Sgb.al('api/butcefisi/tanim', { odenekTur: TUR }).done(function (t) {
        if (t.hata) { mesaj(t.hata, 'danger'); return; }
        tanim = t;
        var $y = $('#ddlYil').empty();
        $.each(t.yillar || [], function (i, y) { $('<option></option>').val(y).text(y).appendTo($y); });
        $y.val(saklananOku('yil', String(t.yil)));
        if (!$y.val()) $y.val(String(t.yil));

        var $s = $('#ddlSenaryo').empty();
        $.each(t.senaryolar || [], function (i, s) { $('<option></option>').val(s.kod).text(s.ad).appendTo($s); });
        $s.val(saklananOku('senaryo', t.senaryo));
        if (!$s.val()) $s.val(t.senaryo);
        if (!$s.val() && t.senaryolar && t.senaryolar.length) $s.val(t.senaryolar[0].kod);

        // Kurumsal kod verilmemişse varsayılan kurum gelir (eski sayfadaki gibi).
        $('.kod-alan').each(function () {
            var alan = $(this).attr('data-alan');
            var deger = saklananOku('kod.' + alan, null);
            if (deger === null && alan === 'kur') deger = t.kurum || '';
            $(this).find('input').val(deger || '');
        });
    }).fail(hataGoster);
});
