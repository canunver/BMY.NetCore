// Bütçe Aşama İşlemleri (ButceHazirlikv2/ButceAsamaIslemleri): birim için her bütçe aşamasının onay durumu (listele, değiştir, kaydet)
// ve bir yılın bir aşamasındaki gider / gelir ödeneklerini başka bir yılın aşamasına kopyalama.
$(function () {
    'use strict';

    if (!$('#grdDurum').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var durumlar = [];        // [{ kod, ad }] Kapalı, Açık, Yöneticilere Açık, Onaylı
    var kaydedebilir = false; // durumları yalnızca sistem yöneticisi kaydedebilir
    var acik = null;          // listelenmiş ölçüt { yil, birim }: Kaydet bunu kullanır
    var kopyaTur = 0;         // onay penceresi açıkken: 1 gider, 2 gelir

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function secenekler($ddl, liste, secili, bosMetin) {
        $ddl.empty();
        if (bosMetin !== undefined) $('<option value=""></option>').text(bosMetin).appendTo($ddl);
        $.each(liste, function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($ddl); });
        if (secili !== undefined && secili !== null) $ddl.val(secili);
        if ($ddl.val() === null && $ddl.children().length) $ddl.prop('selectedIndex', 0);
    }

    function yilOku(sec) { return parseInt($(sec).val(), 10) || 0; }

    // ---------------------------------------------------------------- aşama durumları

    // Durum sütunu: düzenleyebilen için seçim kutusu, diğerleri için metin.
    function durumHucre(p) {
        var ad = (durumlar[p.value] || {}).ad || '';
        if (!kaydedebilir) return document.createTextNode(ad);
        var sec = document.createElement('select');
        sec.className = 'form-control durum-sec';
        sec.setAttribute('aria-label', 'Durum: ' + p.data.senaryoAd);
        $.each(durumlar, function (i, d) { sec.add(new Option(d.ad, d.kod)); });
        sec.value = String(p.value);
        sec.addEventListener('change', function () { p.data.onayDurum = parseInt(sec.value, 10); });
        return sec;
    }

    var grid = Sgb.grid('#grdDurum', {
        columnDefs: [
            { headerName: '#', width: 56, sortable: false, valueGetter: function (p) { return p.node.rowIndex + 1; } },
            { field: 'senaryoKod', headerName: 'Kod', width: 90 },
            { field: 'senaryoAd', headerName: 'Aşama Adı', flex: 1, minWidth: 180, tooltipField: 'senaryoAd' },
            { field: 'onayDurum', headerName: 'Durum', width: 180, cellRenderer: durumHucre }
        ],
        overlayNoRowsTemplate: '<span>Yılı ve birimi seçip Listele\'ye basınız.</span>'
    });

    function listele() {
        var o = { yil: yilOku('#txtYil'), birim: $('#ddlBirim').val() || '' };
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!o.birim) { mesaj('Birim seçiniz.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/butceasama/liste', o)
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                acik = o;
                grid.setGridOption('overlayNoRowsTemplate', '<span>Tanımlı bütçe aşaması yok.</span>');
                grid.setGridOption('rowData', veri.satirlar);
                $('#btnKaydet').prop('disabled', !kaydedebilir || !veri.satirlar.length);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmOnay').on('submit', function (olay) { olay.preventDefault(); listele(); });

    // Yıl ya da birim değişince eski liste kaydedilemez (Kaydet listelenen yıl ve birime yazar).
    function listeyiBosalt() {
        if (!acik) return;
        acik = null;
        grid.setGridOption('rowData', []);
        $('#btnKaydet').prop('disabled', true);
    }

    $('#ddlBirim').on('change', listeyiBosalt);

    // Birimler yıla bağlıdır: yıl değişince yeniden okunur, seçili birim varsa korunur.
    $('#txtYil').on('change', function () {
        listeyiBosalt();
        var yil = yilOku(this);
        if (yil < 2000) return;
        var secili = $('#ddlBirim').val();
        Sgb.al('api/butceasama/kodlar', { yil: yil })
            .done(function (k) {
                if (k.hata) { mesaj(k.hata, 'danger'); return; }
                secenekler($('#ddlBirim'), k.birimler, secili);
            })
            .fail(hataGoster);
    });

    $('#btnKaydet').on('click', function () {
        if (!acik) return;
        var satirlar = [];
        grid.forEachNode(function (n) { satirlar.push({ senaryoKod: n.data.senaryoKod, onayDurum: n.data.onayDurum }); });
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/butceasama/kaydet', { yil: acik.yil, birim: acik.birim, satirlar: satirlar })
            .done(function (s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- ödenek kopyalama

    function kopyaBilgisi(tur) {
        return {
            tur: tur,
            kaynakYil: yilOku('#txtKaynakYil'),
            kaynakAsama: $('#ddlKaynakAsama').val() || '',
            ekoKod: tur === 1 ? ($('#ddlEkoKod').val() || '') : '',
            hedefYil: yilOku('#txtHedefYil'),
            hedefAsama: $('#ddlHedefAsama').val() || ''
        };
    }

    function kopyaHatasi(k) {
        var h = [];
        if (k.kaynakYil < 2000) h.push('Kaynak yıl hatalı!');
        if (k.hedefYil < 2000) h.push('Hedef yıl hatalı!');
        if (!k.kaynakAsama) h.push('Kaynak aşama hatalı!');
        if (!k.hedefAsama) h.push('Hedef aşama hatalı!');
        if (!h.length && k.kaynakYil === k.hedefYil && k.kaynakAsama === k.hedefAsama) h.push('Kaynak ve hedef aynı olamaz!');
        return h.join(' ');
    }

    function sonucYaz(metin, tur) {
        $('#kopyaSonuc').removeClass('alert-success alert-warning alert-danger').addClass('alert-' + tur).text(metin).prop('hidden', !metin);
    }

    var dlgOnay = document.getElementById('dlgOnay');
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    $('#frmKopya').on('click', '[data-kopya]', function () {
        var tur = parseInt($(this).attr('data-kopya'), 10);
        var k = kopyaBilgisi(tur), hata = kopyaHatasi(k);
        if (hata) { mesaj(hata, 'warning'); return; }
        kopyaTur = tur;
        // Eski BHMBOK007 / BHMBOK008 metinleri
        var metin = k.kaynakYil + ' yılının ' + $('#ddlKaynakAsama option:selected').text() + ' aşamasındaki ' + (tur === 1 ? 'gider' : 'gelir') +
            ' ödeneklerini, ' + k.hedefYil + ' yılının ' + $('#ddlHedefAsama option:selected').text() + ' aşamasına kopyalamak istediğinizden emin misiniz?';
        if (tur === 1 && k.ekoKod) metin += ' (Yalnızca ' + $('#ddlEkoKod option:selected').text() + ')';
        $('#dlgOnayBaslik').text(tur === 1 ? 'Gider Ödenekleri Kopyala' : 'Gelir Ödenekleri Kopyala');
        $('#onayMetin').text(metin);
        dlgOnay.showModal();
    });

    $('#btnOnayla').on('click', function () {
        var k = kopyaBilgisi(kopyaTur);
        var $d = $('#btnOnayla, #frmKopya [data-kopya]').prop('disabled', true);
        $(this).html('<span class="glyphicon glyphicon-hourglass" aria-hidden="true"></span> Kopyalanıyor...');
        sonucYaz('', 'success');
        Sgb.gonder('api/butceasama/kopyala', k)
            .done(function (s) {
                dlgOnay.close();
                if (!s.basarili) {
                    mesaj(s.hata, 'danger');
                    sonucYaz(s.hata + (s.bilgi ? '\n' + s.bilgi : ''), 'danger');
                    return;
                }
                mesaj(s.uyari ? 'Kopyalama bitti, bazı tablolarda hata var.' : 'Kopyalama tamamlandı.', s.uyari ? 'warning' : 'success');
                sonucYaz(s.bilgi, s.uyari ? 'warning' : 'success');
            })
            .fail(function (xhr) { dlgOnay.close(); hataGoster(xhr); })
            .always(function () {
                $d.prop('disabled', false);
                $('#btnOnayla').html('<span class="glyphicon glyphicon-duplicate" aria-hidden="true"></span> Kopyala');
            });
    });

    // Ekonomik kodlar hedef yıla göre okunur (kopya hedef yılın tertip yapısıyla süzülür).
    $('#txtHedefYil').on('change', function () {
        var yil = yilOku(this);
        if (yil < 2000) return;
        var secili = $('#ddlEkoKod').val();
        Sgb.al('api/butceasama/kodlar', { yil: yil })
            .done(function (k) { if (!k.hata) secenekler($('#ddlEkoKod'), k.ekoKodlar, secili); })
            .fail(hataGoster);
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/butceasama/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            durumlar = t.durumlar;
            kaydedebilir = t.kaydedebilir;
            $('#kayitNot').prop('hidden', kaydedebilir);
            $('#txtYil, #txtKaynakYil, #txtHedefYil').attr('max', t.enBuyukYil).val(t.yil);
            secenekler($('#ddlBirim'), t.kodlar.birimler);
            secenekler($('#ddlEkoKod'), t.kodlar.ekoKodlar, '');
            secenekler($('#ddlKaynakAsama'), t.asamalar, '', 'Seçiniz');
            secenekler($('#ddlHedefAsama'), t.asamalar, '', 'Seçiniz');
        })
        .fail(hataGoster);
});
