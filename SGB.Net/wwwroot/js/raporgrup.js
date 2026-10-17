// Genel rapor ekranı (eski Raporlama/RaporGrupListe.aspx, RaporKistasSayfa.aspx, RaporDosya.aspx, RaporGosterPDF.aspx).
// Uçlar: /api/raporgrup/liste, kistas, bagli, al, dosyalar, dosya, kutuphane-ekle, kutuphane-sil
// Soldaki listeden rapor seçilince kıstas alanları sunucudaki tanıma (Rapor.xml) göre çizilir; yıl değişince yıla bağlı alanlar
// (birim, teklif yılı) yenilenir. Rapor Al raporu üretip indirir; üretilen rapor "Hazırlanmış Raporlar"da (son 3) görünür ve
// kütüphaneye eklenerek kalıcı yapılabilir.
(function ($) {
    'use strict';

    var raporGrup = $('#raporGrup').data('rapor-grup') + '';
    var secili = null;      // { grup, no, ad, tasindi }
    var alanlar = [];       // sunucudan gelen kıstas alanları
    var kistasSurum = 0;    // hızlı rapor değiştirmede eski yanıtlar atılır

    function hata(x) {
        if (x && x.status !== 401) Sgb.bildirim(x.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function mesaj(metin, tur) {
        Sgb.mesaj('#kutuMesaj', metin, tur || 'warning');
    }

    // ---------------------------------------------------------------- rapor listesi

    var gridRaporlar = Sgb.grid('#grdRaporlar', {
        columnDefs: [
            {
                field: 'ad', headerName: 'Rapor', flex: 1, sortable: false, tooltipField: 'ad',
                cellClass: function (p) { return p.data && !p.data.tasindi ? 'tasinmadi' : ''; }
            }
        ],
        headerHeight: 0,
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        getRowId: function (p) { return p.data.kod; },
        onSelectionChanged: function () {
            var s = gridRaporlar.getSelectedRows();
            if (s.length) raporSec(s[0]);
        },
        overlayNoRowsTemplate: '<span>Bu grupta rapor bulunamadı.</span>'
    });

    function listeOku() {
        if (!raporGrup) { mesaj('Rapor grubu belirtilmemiş.', 'danger'); return; }
        gridRaporlar.setGridOption('loading', true);
        Sgb.al('api/raporgrup/liste', { raporGrup: raporGrup }).done(function (v) {
            if (v.hata) mesaj(v.hata, 'danger');
            if (v.baslik) {
                $('#lblGrupAd').text(v.baslik);
                document.title = v.baslik;
            }
            gridRaporlar.setGridOption('rowData', v.raporlar || []);
            // Eskisi gibi ilk rapor seçili gelir.
            var ilk = gridRaporlar.getDisplayedRowAtIndex(0);
            if (ilk) ilk.setSelected(true);
        }).fail(hata).always(function () { gridRaporlar.setGridOption('loading', false); });
    }

    // ---------------------------------------------------------------- kıstas alanları

    function alanKimlik(ad) {
        return 'k_' + ad;
    }

    function secenekleriDoldur($s, a) {
        $s.empty();
        $.each(a.secenekler || [], function (i, o) { $('<option></option>').val(o.kod).text(o.ad).appendTo($s); });
        $s.val(a.deger);
    }

    function alanCiz(a) {
        var kimlik = alanKimlik(a.ad);
        var $satir = $('<div class="form-group"></div>').attr('data-alan', a.ad);
        if (a.gizli) $satir.prop('hidden', true);
        var $etiket = $('<label class="control-label"></label>').attr('for', kimlik).text(a.etiket || a.ad);
        if (a.zorunlu) $etiket.addClass('zorunlu').append('<span class="zorunlu-isaret" aria-hidden="true"> *</span>');
        $satir.append($etiket);

        var $alan;
        if (a.tur === 'secim') {
            $alan = $('<select class="form-control input-sm"></select>');
            secenekleriDoldur($alan, a);
            if (a.yilAlani) $alan.on('change', bagliAlanlariYenile);
        } else if (a.tur === 'onay') {
            $alan = $('<div class="checkbox"><label><input type="checkbox" /> </label></div>');
            $alan.find('input').attr('id', kimlik).prop('checked', a.deger === '1');
            $satir.append($alan);
            return $satir;
        } else {
            $alan = $('<input type="text" class="form-control input-sm" />').val(a.deger || '');
            if (a.tur === 'tarih') $alan.attr('placeholder', 'gg.aa.yyyy').addClass('dar');
        }
        $alan.attr('id', kimlik).attr('name', kimlik);
        if (a.zorunlu) $alan.attr('aria-required', 'true');
        $satir.append($alan);
        return $satir;
    }

    function kistasCiz(v) {
        alanlar = v.alanlar || [];
        var $f = $('#frmKistas').empty();
        $.each(alanlar, function (i, a) { $f.append(alanCiz(a)); });
        var gorunen = $.grep(alanlar, function (a) { return !a.gizli; }).length;
        if (!gorunen) $f.append('<p class="bos-ipucu">Bu rapor için kıstas girilmez.</p>');
        $f.prop('hidden', false);
        $('#lblKistasBos').prop('hidden', true);
    }

    function raporSec(r) {
        secili = r;
        var benim = ++kistasSurum;
        mesaj('');
        $('#lblRaporAd').text(r.ad);
        $('#btnRaporAl').prop('disabled', true);
        $('#frmKistas').prop('hidden', true).empty();
        $('#lblKistasBos').text('Kıstaslar okunuyor...').prop('hidden', false);
        dosyalariBosalt();

        Sgb.al('api/raporgrup/kistas', { raporGrup: raporGrup, grup: r.grup, no: r.no }).done(function (v) {
            if (benim !== kistasSurum) return;
            if (v.hata) {
                $('#lblKistasBos').text('');
                mesaj(v.hata, 'danger');
                return;
            }
            if (v.raporAd) $('#lblRaporAd').text(v.raporAd);
            kistasCiz(v);
            if (v.uyari) mesaj(v.uyari, 'warning');
            $('#btnRaporAl').prop('disabled', !!v.uyari);
        }).fail(function (x) {
            if (benim !== kistasSurum) return;
            $('#lblKistasBos').text('');
            hata(x);
        });
        dosyalariOku();
    }

    // Eski ddlYil_SelectedIndexChanged: birim ve teklif yılı listeleri seçilen yıla göre yenilenir.
    function bagliAlanlariYenile() {
        if (!secili || !$.grep(alanlar, function (a) { return a.yilaBagli; }).length) return;
        var yil = $(this).val();
        var benim = kistasSurum;
        Sgb.al('api/raporgrup/bagli', { raporGrup: raporGrup, grup: secili.grup, no: secili.no, yil: yil }).done(function (v) {
            if (benim !== kistasSurum) return;
            if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
            $.each(v.alanlar || [], function (i, a) {
                var $s = $('#' + alanKimlik(a.ad));
                var onceki = $s.val();
                secenekleriDoldur($s, a);
                if (onceki && $s.find('option').filter(function () { return this.value === onceki; }).length) $s.val(onceki);
            });
        }).fail(hata);
    }

    function kistasDegerleri() {
        var p = { raporGrup: raporGrup, grup: secili.grup, no: secili.no };
        $.each(alanlar, function (i, a) {
            if (a.gizli) return;
            var $a = $('#' + alanKimlik(a.ad));
            p[alanKimlik(a.ad)] = a.tur === 'onay' ? ($a.prop('checked') ? '1' : '0') : ($a.val() || '');
        });
        return p;
    }

    function zorunluEksik() {
        var eksik = [];
        $.each(alanlar, function (i, a) {
            if (a.gizli || !a.zorunlu || a.tur === 'onay') return;
            if (!$.trim($('#' + alanKimlik(a.ad)).val() || '')) eksik.push(a.etiket || a.ad);
        });
        return eksik;
    }

    $('#btnRaporAl').on('click', function () {
        if (!secili) return;
        var eksik = zorunluEksik();
        if (eksik.length) {
            Sgb.bildirim($.map(eksik, function (e) { return "'" + e + "' alanını girmelisiniz!"; }).join(' '), 'warning');
            return;
        }
        var $d = $(this).prop('disabled', true);
        var eskiMetin = $d.html();
        $d.html('<span class="glyphicon glyphicon-hourglass" aria-hidden="true"></span> Hazırlanıyor...');
        var r = secili;
        Sgb.indir('api/raporgrup/al', kistasDegerleri()).then(function () {
            if (secili === r) dosyalariOku();
        }).finally(function () {
            $d.html(eskiMetin).prop('disabled', false);
        });
    });

    // ---------------------------------------------------------------- saklanan raporlar

    function dosyaIslemleri(kutuphane) {
        return function (p) {
            if (!p.data) return '';
            var $k = $('<span class="dosya-islem"></span>');
            $('<button type="button" class="btn btn-default btn-xs" data-islem="indir"></button>')
                .attr('title', p.data.tur + ' olarak indir').attr('aria-label', p.data.tur + ' olarak indir')
                .html('<span class="glyphicon glyphicon-download-alt" aria-hidden="true"></span> ' + $('<i></i>').text(p.data.tur).html()).appendTo($k);
            if (p.data.tur !== 'PDF')
                $('<button type="button" class="btn btn-default btn-xs" data-islem="pdf" title="PDF olarak göster" aria-label="PDF olarak göster"></button>')
                    .html('<span class="glyphicon glyphicon-file" aria-hidden="true"></span> PDF').appendTo($k);
            if (!kutuphane)
                $('<button type="button" class="btn btn-success btn-xs" data-islem="ekle" title="Kütüphaneye ekle (kalıcı yap)" aria-label="Kütüphaneye ekle"></button>')
                    .html('<span class="glyphicon glyphicon-book" aria-hidden="true"></span> Kütüphaneye Ekle').appendTo($k);
            else if (p.data.silinebilir)
                $('<button type="button" class="btn btn-danger btn-xs" data-islem="sil" title="Kütüphaneden sil" aria-label="Kütüphaneden sil"></button>')
                    .html('<span class="glyphicon glyphicon-trash" aria-hidden="true"></span> Sil').appendTo($k);
            $k.on('click', 'button', function () { dosyaIslem($(this).data('islem'), p.data, kutuphane); });
            return $k[0];
        };
    }

    function dosyaGrid(secici, kutuphane, bosMetin) {
        var sutunlar = [
            { field: 'tarih', headerName: 'Hazırlanma Tarihi', width: 160, sort: null },
            { field: 'kistas', headerName: 'Kıstaslar', flex: 1, minWidth: 140, tooltipField: 'kistas' }
        ];
        if (kutuphane) sutunlar.push({ field: 'sahip', headerName: 'Ekleyen', width: 120 });
        sutunlar.push({ headerName: '', colId: 'islem', width: kutuphane ? 200 : 290, sortable: false, cellRenderer: dosyaIslemleri(kutuphane) });
        return Sgb.grid(secici, {
            columnDefs: sutunlar,
            getRowId: function (p) { return p.data.ad; },
            overlayNoRowsTemplate: '<span>' + bosMetin + '</span>'
        });
    }

    var gridGecici = dosyaGrid('#grdGecici', false, 'Henüz rapor hazırlanmamış.');
    var gridKutuphane = dosyaGrid('#grdKutuphane', true, 'Kütüphanede rapor bulunmamaktadır.');

    function dosyalariBosalt() {
        gridGecici.setGridOption('rowData', []);
        gridKutuphane.setGridOption('rowData', []);
    }

    function dosyalariOku() {
        if (!secili) return;
        var r = secili;
        Sgb.al('api/raporgrup/dosyalar', { raporGrup: raporGrup, grup: r.grup, no: r.no }).done(function (v) {
            if (secili !== r) return;
            if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
            gridGecici.setGridOption('rowData', v.gecici || []);
            gridKutuphane.setGridOption('rowData', v.kutuphane || []);
        }).fail(hata);
    }

    // PDF yeni sekmede açılır (eski showPopWin). Sekme tıklama anında açılır ki tarayıcı engellemesin.
    function pdfGoster(parametreler) {
        var sekme = window.open('', '_blank');
        if (sekme) sekme.document.title = 'Hazırlanıyor...';
        fetch(Sgb.KOK + 'api/raporgrup/dosya?' + $.param(parametreler), { credentials: 'same-origin', cache: 'no-store' }).then(function (y) {
            if (y.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            var tur = y.headers.get('Content-Type') || '';
            if (!y.ok || tur.indexOf('pdf') < 0) {
                if (sekme) sekme.close();
                if (tur.indexOf('json') >= 0) return y.json().then(function (v) { Sgb.bildirim(v.hata || 'PDF hazırlanamadı.', 'danger'); });
                Sgb.bildirim(y.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'PDF hazırlanamadı.', 'danger');
                return;
            }
            return y.blob().then(function (b) {
                var adres = URL.createObjectURL(b);
                if (sekme) sekme.location.href = adres;
                else window.location.href = adres;
                setTimeout(function () { URL.revokeObjectURL(adres); }, 60000);
            });
        }).catch(function () {
            if (sekme) sekme.close();
            Sgb.bildirim('PDF hazırlanamadı.', 'danger');
        });
    }

    function dosyaIslem(islem, d, kutuphane) {
        var r = secili;
        var p = { raporGrup: raporGrup, grup: r.grup, no: r.no, ad: d.ad, kutuphane: kutuphane ? 1 : 0 };
        if (islem === 'indir') {
            Sgb.indir('api/raporgrup/dosya', p);
        } else if (islem === 'pdf') {
            p.pdf = 1;
            pdfGoster(p);
        } else if (islem === 'ekle') {
            if (!window.confirm('Seçtiğiniz rapor dosyası kütüphaneye eklenecek. Onaylıyor musunuz?')) return;
            Sgb.gonder('api/raporgrup/kutuphane-ekle', { raporGrup: raporGrup, grup: r.grup, no: r.no, ad: d.ad }).done(function (v) {
                if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
                Sgb.bildirim('Rapor kütüphaneye eklendi.', 'success');
                dosyalariOku();
            }).fail(hata);
        } else if (islem === 'sil') {
            if (!window.confirm('Seçtiğiniz rapor dosyası kütüphaneden silinecek. Onaylıyor musunuz?')) return;
            Sgb.gonder('api/raporgrup/kutuphane-sil', { raporGrup: raporGrup, grup: r.grup, no: r.no, ad: d.ad }).done(function (v) {
                if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return; }
                Sgb.bildirim('Rapor kütüphaneden silindi.', 'success');
                dosyalariOku();
            }).fail(hata);
        }
    }

    listeOku();
})(jQuery);
