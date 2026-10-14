// Dosyalar (eski YazilimGuncelle/Dosyalar.aspx): sunucudaki klasörlerde gezinme, dosya indirme / silme / yükleme, klasör oluşturma,
// dosya tarihlerini değiştirme. Uçlar: /api/guncelleme/dosyalar, dosya-indir, dosya-sil, dosya-yukle, klasor-olustur, tarih-degistir
// Bulunulan klasörün tam yolu Path alanında ve adres satırında ?dir= olarak tutulur; sayfa yenilenince aynı klasör açılır.
// Path alanına yazılan yola (göreli ise uygulama klasörüne göre) Git ile gidilir.
(function ($) {
    'use strict';

    var dir = new URLSearchParams(window.location.search).get('dir') || '';
    var ust = null;
    var sayi = new Intl.NumberFormat('tr-TR');

    function zaman(deger) {
        var p = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})/.exec(deger || '');
        return p ? p[3] + '.' + p[2] + '.' + p[1] + ' ' + p[4] + ':' + p[5] + ':' + p[6] : '';
    }

    function adHucresi(p) {
        var d = p.data;
        var $h = $('<span class="dosya-ad"></span>');
        if (d.tur === 'k') {
            $('<a href="#" class="klasor"></a>')
                .append('<span class="glyphicon glyphicon-folder-close" aria-hidden="true"></span> ')
                .append(document.createTextNode(d.ad))
                .on('click', function (e) { e.preventDefault(); listele(d.yol); })
                .appendTo($h);
        } else {
            $('<button type="button" class="sil" title="Sil"><span class="glyphicon glyphicon-remove" aria-hidden="true"></span></button>')
                .attr('aria-label', 'Sil: ' + d.ad)
                .on('click', function () { sil(d.ad); })
                .appendTo($h);
            $('<a target="_blank"></a>')
                .attr('href', Sgb.KOK + 'api/guncelleme/dosya-indir?' + $.param({ dir: dir, ad: d.ad }))
                .append('<span class="glyphicon glyphicon-file" aria-hidden="true"></span> ')
                .append(document.createTextNode(d.ad))
                .appendTo($h);
        }
        return $h[0];
    }

    var grid = Sgb.grid('#grdDosyalar', {
        columnDefs: [
            { field: 'ad', headerName: 'Dosya Adı', flex: 1, minWidth: 260, cellRenderer: adHucresi, comparator: function (a, b, na, nb) {
                // Klasörler hep üstte.
                if (na.data.tur !== nb.data.tur) return na.data.tur === 'k' ? -1 : 1;
                return a.localeCompare(b, 'tr');
            } },
            { field: 'boyut', headerName: 'Boyut (bayt)', width: 140, type: 'rightAligned',
              valueFormatter: function (p) { return p.value === null || p.value === undefined ? '' : sayi.format(p.value); } },
            { field: 'tarih', headerName: 'Değiştirilme Tarihi', width: 180, valueFormatter: function (p) { return zaman(p.value); } }
        ],
        overlayNoRowsTemplate: '<span>Klasör boş.</span>'
    });

    function listele(yeniDir) {
        grid.setGridOption('loading', true);
        return Sgb.al('api/guncelleme/dosyalar', { dir: yeniDir }).done(function (v) {
            if (!v.basarili) { Sgb.bildirim(v.hata, 'danger'); if (!v.satirlar) { $('#txtYol').val(dir); return; } }
            dir = v.dir || '';
            ust = v.ust;
            $('#txtYol').val(dir);
            $('#btnUst').prop('disabled', ust === null || ust === undefined);
            grid.setGridOption('rowData', v.satirlar || []);
            var adres = new URL(window.location.href);
            if (dir) adres.searchParams.set('dir', dir); else adres.searchParams.delete('dir');
            window.history.replaceState(null, '', adres);
        }).fail(function (x) {
            if (x.status !== 401) Sgb.bildirim(x.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Liste okunamadı.', 'danger');
        }).always(function () { grid.setGridOption('loading', false); });
    }

    function sonuc(v) {
        if (!v.basarili) { Sgb.bildirim(v.hata, 'danger'); return false; }
        if (v.bilgi) Sgb.bildirim(v.bilgi, 'success');
        listele(dir);
        return true;
    }

    function hata(x) {
        if (x.status !== 401) Sgb.bildirim(x.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;

    function sor(metin, evet, is) {
        $('#soruMetin').text(metin);
        $('#btnSoruEvet').text(evet);
        soruIsi = is;
        dlgSoru.showModal();
    }

    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        if (soruIsi) soruIsi();
        soruIsi = null;
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- işlemler

    function sil(ad) {
        sor('"' + ad + '" dosyası silinecek. Onaylıyor musunuz?', 'Sil', function () {
            Sgb.gonder('api/guncelleme/dosya-sil', { dir: dir, ad: ad }).done(sonuc).fail(hata);
        });
    }

    $('#btnUst').on('click', function () { if (ust !== null && ust !== undefined) listele(ust); });
    $('#frmYol').on('submit', function (e) { e.preventDefault(); listele($.trim($('#txtYol').val())); });
    $('#btnYenile').on('click', function () { listele(dir); });

    $('#frmYukle').on('submit', function (e) {
        e.preventDefault();
        var dosya = $('#fuDosya')[0].files[0];
        if (!dosya) { Sgb.bildirim('Dosya seçiniz.', 'warning'); return; }
        var veri = new FormData();
        veri.append('dir', dir);
        veri.append('dosya', dosya);
        var $d = $('#btnDosyaSakla').prop('disabled', true);
        $.ajax({
            url: Sgb.KOK + 'api/guncelleme/dosya-yukle',
            method: 'POST',
            data: veri,
            processData: false,
            contentType: false,
            dataType: 'json',
            headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (sonuc(v)) $('#fuDosya').val('');
        }).fail(function (x) {
            if (x.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            hata(x);
        }).always(function () { $d.prop('disabled', false); });
    });

    $('#frmTarih').on('submit', function (e) {
        e.preventDefault();
        var tarih = $('#txtTarih').val();
        if (!tarih) { Sgb.bildirim('Tarihi giriniz.', 'warning'); return; }
        sor('Bu klasör ve alt klasörlerindeki, oluşturulma tarihi ' + Sgb.tarih(tarih) + ' tarihinden sonra olan dosyaların tarihleri değiştirilecek. Onaylıyor musunuz?', 'Değiştir', function () {
            Sgb.gonder('api/guncelleme/tarih-degistir', { dir: dir, tarih: tarih }).done(sonuc).fail(hata);
        });
    });

    $('#frmKlasor').on('submit', function (e) {
        e.preventDefault();
        var ad = $.trim($('#txtKlasor').val());
        if (!ad) return;
        Sgb.gonder('api/guncelleme/klasor-olustur', { dir: dir, ad: ad }).done(function (v) {
            if (sonuc(v)) $('#txtKlasor').val('');
        }).fail(hata);
    });

    // Eski sayfa açılınca uygulama klasörünü listeliyordu.
    listele(dir);
})(jQuery);
