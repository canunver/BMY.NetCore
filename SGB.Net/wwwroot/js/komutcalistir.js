// Komut Çalıştır (eski YazilimGuncelle/KomutCalistir.aspx). Uçlar: /api/guncelleme/komut, /api/guncelleme/komut-excel
// Bağlantı seçilince tablolar ve görünümler (komut-nesneler), tablo seçilince alanları (komut-alanlar) listelenir; SELECT Oluştur
// seçilen alanlarla SQL alanına SELECT cümlesi yazar. Saklı sorgular: komut-sorgular, komut-sorgu-sakla, komut-sorgu-sil.
// Sonuç tablosu sunucudan HTML olarak gelir; veri tabanındaki değerler sayfada çalışmasın diye betiksiz bir çerçevede gösterilir.
(function ($) {
    'use strict';

    // Sayfa gizli gelir; uyarıda Tamam'a basılınca gösterilir.
    window.alert('Bu sayfada yapılan işlemlerle veri tabanında geri dönülemez sonuçlar oluşturabilirsiniz. Dikkatli olunuz!');
    $('#komutSayfa').prop('hidden', false);

    var STIL = '<style>body{margin:8px;font:12px/1.4 "Segoe UI",Arial,sans-serif;color:#16202b}' +
        'table{border-collapse:collapse}td{padding:2px 6px;border:1px solid #c9d1da;white-space:nowrap}</style>';

    function istek() {
        return { sql: $('#txtSql').val(), baglanti: $.trim($('#cmbBaglanti').val()) };
    }

    function dugmeler(kapali) {
        $('#btnCalistir, #btnExcel').prop('disabled', kapali);
    }

    function hata(x) {
        if (x.status !== 401) Sgb.bildirim(x.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Komut çalıştırılamadı.', 'danger');
    }

    // ---------------------------------------------------------------- tablolar, görünümler ve alanlar
    // Tablo, alan ve saklı sorgu kutuları aranabilir açılır kutudur (select2).

    var nesneler = [];
    var nesneBaglanti = null;
    var secili = null; // { sema, ad }
    var alanIstek = null;

    function secim(kutu, yer) {
        $(kutu).select2({ language: 'tr', placeholder: yer, allowClear: true, width: '100%', closeOnSelect: !$(kutu).prop('multiple') });
    }
    secim('#cmbTablo', 'Tablo / görünüm seçiniz');
    secim('#cmbAlanlar', 'Bütün alanlar');
    secim('#cmbSorgu', 'Saklı sorgu seçiniz');

    function koseli(ad) {
        return '[' + String(ad).replace(/\]/g, ']]') + ']';
    }

    function alanlariTemizle() {
        secili = null;
        alanIstek = null;
        $('#cmbAlanlar').empty().trigger('change');
        $('#btnSelect').prop('disabled', true);
    }

    function nesneleriGoster() {
        var $c = $('#cmbTablo').empty().append('<option></option>');
        $.each(nesneler, function (i, n) {
            $('<option></option>').val(i).text((n.sema ? n.sema + '.' : '') + n.ad + (n.tur === 'V' ? ' (görünüm)' : '')).appendTo($c);
        });
        $c.val(null).trigger('change');
    }

    function nesneleriOku(zorla) {
        var baglanti = $.trim($('#cmbBaglanti').val());
        if (!zorla && baglanti === nesneBaglanti) return;
        nesneBaglanti = baglanti || null;
        nesneler = [];
        nesneleriGoster();
        alanlariTemizle();
        if (!baglanti) return;
        $('#cmbTablo').prop('disabled', true);
        Sgb.gonder('api/guncelleme/komut-nesneler', { baglanti: baglanti }).done(function (v) {
            if (baglanti !== nesneBaglanti) return;
            if (!v.basarili) { Sgb.bildirim(v.hata, 'danger'); nesneBaglanti = null; return; }
            nesneler = v.nesneler || [];
            nesneleriGoster();
        }).fail(function (x) { hata(x); nesneBaglanti = null; })
          .always(function () { $('#cmbTablo').prop('disabled', false); });
    }

    function alanlariOku(n) {
        alanlariTemizle();
        secili = { sema: n.sema, ad: n.ad, alanlar: null };
        var bu = alanIstek = {};
        Sgb.gonder('api/guncelleme/komut-alanlar', { baglanti: nesneBaglanti, sema: n.sema, ad: n.ad }).done(function (v) {
            if (bu !== alanIstek) return;
            if (!v.basarili) { Sgb.bildirim(v.hata, 'danger'); return; }
            secili.alanlar = $.map(v.alanlar || [], function (a) { return a.ad; });
            var $c = $('#cmbAlanlar');
            $.each(v.alanlar || [], function (i, a) {
                $('<option></option>').val(a.ad).text(a.ad + '  ' + a.tip + (a.bos ? '' : ' not null')).appendTo($c);
            });
            $c.trigger('change');
            $('#btnSelect').prop('disabled', !secili.alanlar.length);
        }).fail(function (x) { if (bu === alanIstek) hata(x); });
    }

    $('#cmbBaglanti').on('change', function () { nesneleriOku(false); });
    $('#cmbTablo').on('change', function () {
        var deger = $(this).val();
        var n = deger === null || deger === '' ? null : nesneler[+deger];
        if (n) alanlariOku(n); else alanlariTemizle();
    });

    // SELECT TOP 100 ve alan adları (alan seçilmezse tablonun bütün alanları). Çok satırlı tablolarda sayfa donmasın diye TOP 100 konur.
    $('#btnSelect').on('click', function () {
        if (!secili || !secili.alanlar) return;
        var alanlar = $('#cmbAlanlar').val() || [];
        if (!alanlar.length) alanlar = secili.alanlar;
        var sql = 'SELECT TOP 100 ' + $.map(alanlar, koseli).join(', ') + '\nFROM ' +
            (secili.sema ? koseli(secili.sema) + '.' : '') + koseli(secili.ad);
        $('#txtSql').val(sql).trigger('focus');
    });

    // ---------------------------------------------------------------- saklı sorgular

    var sorgular = [];
    var sorguYukleniyor = false;

    function sorgulariGoster(liste, secilecek) {
        sorgular = liste || [];
        var $c = $('#cmbSorgu').empty().append('<option></option>');
        var yer = -1;
        $.each(sorgular, function (i, q) {
            $('<option></option>').val(i).text(q.ad + (q.baglanti ? '  (' + q.baglanti + ')' : '')).appendTo($c);
            if (q.ad === secilecek) yer = i;
        });
        sorguYukleniyor = true;
        $c.val(yer >= 0 ? String(yer) : null).trigger('change');
        sorguYukleniyor = false;
        $('#btnSorguSil').prop('disabled', yer < 0);
    }

    $('#cmbSorgu').on('change', function () {
        var deger = $(this).val();
        var q = deger === null || deger === '' ? null : sorgular[+deger];
        $('#btnSorguSil').prop('disabled', !q);
        if (!q || sorguYukleniyor) return;
        $('#txtSql').val(q.sql);
        $('#txtSorguAd').val(q.ad);
        if (q.baglanti) { $('#cmbBaglanti').val(q.baglanti); nesneleriOku(false); }
    });

    $('#btnSorguSakla').on('click', function () {
        var ad = $.trim($('#txtSorguAd').val());
        if (!ad) { Sgb.bildirim('Sorgu adını yazınız.', 'warning'); return; }
        var v = istek();
        if (!$.trim(v.sql)) { Sgb.bildirim('SQL metnini yazınız.', 'warning'); return; }
        Sgb.gonder('api/guncelleme/komut-sorgu-sakla', { ad: ad, sql: v.sql, baglanti: v.baglanti }).done(function (s) {
            if (!s.basarili) { Sgb.bildirim(s.hata, 'danger'); return; }
            Sgb.bildirim(s.bilgi, 'success');
            sorgulariGoster(s.sorgular, ad);
        }).fail(hata);
    });

    $('#btnSorguSil').on('click', function () {
        var deger = $('#cmbSorgu').val();
        var q = deger === null || deger === '' ? null : sorgular[+deger];
        if (!q) return;
        if (!window.confirm('"' + q.ad + '" sorgusu silinecek. Onaylıyor musunuz?')) return;
        Sgb.gonder('api/guncelleme/komut-sorgu-sil', { ad: q.ad }).done(function (s) {
            if (!s.basarili) { Sgb.bildirim(s.hata, 'danger'); return; }
            Sgb.bildirim(s.bilgi, 'success');
            sorgulariGoster(s.sorgular);
        }).fail(hata);
    });

    Sgb.al('api/guncelleme/komut-sorgular').done(function (s) {
        if (s.basarili) sorgulariGoster(s.sorgular); else Sgb.bildirim(s.hata, 'danger');
    });

    // ---------------------------------------------------------------- çalıştırma

    $('#btnCalistir').on('click', function () {
        var v = istek();
        if (!$.trim(v.sql)) { Sgb.bildirim('SQL metnini yazınız.', 'warning'); return; }
        dugmeler(true);
        Sgb.gonder('api/guncelleme/komut', v).done(function (s) {
            if (!s.basarili) { Sgb.bildirim(s.hata, 'danger'); return; }
            $('#lblSonucBos').prop('hidden', true);
            $('#frmSonuc').prop('hidden', false).attr('srcdoc', '<!DOCTYPE html><html><head><meta charset="utf-8">' + STIL + '</head><body>' + (s.html || '') + '</body></html>');
        }).fail(hata).always(function () { dugmeler(false); });
    });

    // Excel: sorgu uzun olabileceği için POST ile gönderilir; dönen dosya indirilir, hata dönerse bildirim gösterilir.
    $('#btnExcel').on('click', function () {
        var v = istek();
        if (!$.trim(v.sql)) { Sgb.bildirim('SQL metnini yazınız.', 'warning'); return; }
        dugmeler(true);
        fetch(Sgb.KOK + 'api/guncelleme/komut-excel', {
            method: 'POST',
            credentials: 'same-origin',
            cache: 'no-store',
            headers: { 'Content-Type': 'application/json; charset=utf-8', RequestVerificationToken: Sgb.meta('istek-jetonu') },
            body: JSON.stringify(v)
        }).then(function (yanit) {
            if (yanit.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            if (!yanit.ok) { Sgb.bildirim(yanit.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Dosya hazırlanamadı.', 'danger'); return; }
            var tur = yanit.headers.get('Content-Type') || '';
            if (tur.indexOf('json') >= 0)
                return yanit.json().then(function (s) { Sgb.bildirim(s.hata || 'Dosya hazırlanamadı.', 'danger'); });
            var baslik = yanit.headers.get('Content-Disposition') || '';
            var ad = /filename\*=UTF-8''([^;]+)/i.exec(baslik) || /filename="?([^";]+)"?/i.exec(baslik);
            return yanit.blob().then(function (icerik) {
                var yerel = URL.createObjectURL(icerik);
                var a = document.createElement('a');
                a.href = yerel;
                a.download = ad ? decodeURIComponent(ad[1]) : 'Sonuc.xlsx';
                document.body.appendChild(a);
                a.click();
                a.remove();
                setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
            });
        }).catch(function () {
            Sgb.bildirim('Dosya hazırlanamadı.', 'danger');
        }).then(function () { dugmeler(false); });
    });
})(jQuery);
