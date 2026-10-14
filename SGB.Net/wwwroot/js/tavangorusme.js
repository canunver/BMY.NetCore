// Görüşme / vize tavanı sayfalarının (GorusmeCari, TavanYatirimProje, TavanYatirimVize) ortak yardımcıları: tutar biçimi ve
// çözümü, onay sorusu penceresi, Rapor Al menüsü, Kopyala, Yapıştır, Tekliften Al ve proje grubu ara toplamları.
// Sayfa betikleri bunu kullanır; sayfada #dlgSoru, #dlgYapistir ve .rapor-menu bulunur (tavantertip.css görünümü).
window.TavanGorusme = (function ($) {
    'use strict';

    var tamSayi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 0 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // "1.234.000" ya da "1234000" -> 1234000. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function para(p) { return p.value == null || p.value === '' ? '' : tamSayi.format(p.value); }

    // Düzenlenebilir tutar sütununun özellikleri.
    function giris(girilebilir) {
        return {
            editable: girilebilir,
            cellEditor: 'agTextCellEditor',
            valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; }
        };
    }

    // Tutar sütunu: sağa yaslı, binlik ayraçlı.
    function sayiSutunu(alan, baslik, hucreSinifi, ek) {
        return $.extend({ field: alan, headerName: baslik, type: 'sayi', width: 108, sortable: false, valueFormatter: para, cellClass: hucreSinifi, headerClass: 'sayi', headerTooltip: baslik }, ek || {});
    }

    // ---------------------------------------------------------------- onay sorusu

    // Onay sorusu penceresini (#dlgSoru) kurar; sor(baslik, metin, evetMetni, is) döndürür.
    function soruKur() {
        var dlg = document.getElementById('dlgSoru');
        var is = null;
        $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });
        $('#btnSoruEvet').on('click', function () {
            dlg.close();
            var f = is;
            is = null;
            if (f) f();
        });
        return function (baslik, metin, evet, f) {
            $('#dlgSoruBaslik').text(baslik);
            $('#soruMetin').text(metin);
            $('#btnSoruEvet').text(evet);
            is = f;
            dlg.showModal();
        };
    }

    // ---------------------------------------------------------------- Rapor Al menüsü

    // adres: rapor ucu; parametreler(raporKod): istek parametreleri ya da null (uyarı gösterilmiştir).
    // Menüyü dolduran işlevi döndürür. Bootstrap betiği yüklenmediğinden menü .open ile açılır.
    function raporMenuKur(adres, parametreler) {
        var $grup = $('.rapor-menu');
        function kapat() { $grup.removeClass('open'); $('#btnRapor').attr('aria-expanded', 'false'); }

        $('#btnRapor').on('click', function (e) {
            e.stopPropagation();
            var ac = !$grup.hasClass('open');
            $grup.toggleClass('open', ac);
            $(this).attr('aria-expanded', ac ? 'true' : 'false');
            if (ac) $('#lstRapor button').first().trigger('focus');
        });
        $(document).on('click', kapat);
        $(document).on('keydown', function (e) { if (e.key === 'Escape' && $grup.hasClass('open')) { kapat(); $('#btnRapor').trigger('focus'); } });

        $('#lstRapor').on('click', 'button', function () {
            kapat();
            var o = parametreler($(this).attr('data-rapor'));
            if (!o) return;
            var $d = $('#btnRapor').prop('disabled', true);
            Sgb.indir(adres, o).then(function () { $d.prop('disabled', false); });
        });

        return function (liste) {
            var $l = $('#lstRapor').empty();
            $.each(liste || [], function (i, r) {
                $('<li></li>').append($('<button type="button"></button>').attr('data-rapor', r.kod).text(r.ad)).appendTo($l);
            });
        };
    }

    // ---------------------------------------------------------------- Kopyala / Yapıştır

    // Kopyala (eski CopyStore): tablo (başlıklar ve üstteki sabit satırlar dahil) sekmeyle ayrılmış metin olarak panoya.
    function kopyala(grid) {
        grid.stopEditing();
        var sutun = grid.getAllDisplayedColumns();
        var satirlar = [sutun.map(function (c) { return c.getColDef().headerName; }).join('\t')];
        function ekle(n) {
            satirlar.push(sutun.map(function (c) {
                var v = grid.getCellValue({ rowNode: n, colKey: c, useFormatter: true });
                return String(v == null ? '' : v).replace(/[\t\r\n]+/g, ' ');
            }).join('\t'));
        }
        for (var i = 0; i < grid.getPinnedTopRowCount(); i++) ekle(grid.getPinnedTopRow(i));
        grid.forEachNodeAfterFilterAndSort(ekle);
        var metin = satirlar.join('\r\n');
        var tamam = function () { mesaj('Tablo panoya kopyalandı.', 'success'); };
        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(metin).then(tamam, function () { mesaj('Panoya kopyalanamadı.', 'danger'); });
        } else {
            var $t = $('<textarea readonly></textarea>').val(metin).css({ position: 'fixed', left: '-9999px' }).appendTo(document.body);
            $t[0].select();
            try { document.execCommand('copy'); tamam(); } catch (e) { mesaj('Panoya kopyalanamadı.', 'danger'); }
            $t.remove();
        }
    }

    // Yapıştır (eski EXTVeriYapistir): panodaki satırlar seçili satırdan başlayarak sırayla, sütunlar ilk girilebilir sütundan
    // başlayarak girilebilir sütunlara yazılır. Girilemeyen satırlara yazılmaz (panodaki o satır atlanır).
    //   o.grid, o.alanlar (girilebilir alanlar, sırayla), o.satirUygun(data), o.acik() (giriş açık mı), o.sonra(dugumler)
    function yapistirKur(o) {
        var bas = -1;
        var dlg = document.getElementById('dlgYapistir');

        function yapistir(metin) {
            var satirlar = String(metin || '').split('\n');
            while (satirlar.length && /^\r?$/.test(satirlar[satirlar.length - 1])) satirlar.pop();
            if (!satirlar.length) { mesaj('Panoda yapıştırılacak veri yok.', 'warning'); return; }
            var dugumler = [];
            o.grid.forEachNodeAfterFilterAndSort(function (n) { dugumler.push(n); });
            var yazilan = [];
            for (var i = 0; i < satirlar.length && bas + i < dugumler.length; i++) {
                var d = dugumler[bas + i].data;
                if (!o.satirUygun(d)) continue;
                var degerler = satirlar[i].replace(/\r$/, '').split('\t');
                for (var j = 0; j < Math.min(o.alanlar.length, degerler.length); j++) {
                    var n = parseFloat(String(degerler[j]).replace(/\s/g, '').replace(/\./g, '').replace(/,/g, '.'));
                    d[o.alanlar[j]] = isFinite(n) ? n : 0;
                }
                yazilan.push(dugumler[bas + i]);
            }
            if (!yazilan.length) return;
            o.grid.refreshCells({ force: true });
            o.sonra(yazilan);
            mesaj(yazilan.length + ' satır yapıştırıldı; kaydetmeyi unutmayınız.', 'info');
        }

        $('#btnYapistir').on('click', function () {
            if (!o.acik()) return;
            o.grid.stopEditing();
            var hucre = o.grid.getFocusedCell();
            if (!hucre || hucre.rowPinned || hucre.rowIndex == null || hucre.rowIndex < 0) { mesaj('Lütfen satır seçiniz!', 'warning'); return; }
            bas = hucre.rowIndex;
            // Pano okunamazsa (tarayıcı izin vermedi) veri pencereye yapıştırılır.
            var pencere = function () { $('#txtYapistir').val(''); dlg.showModal(); $('#txtYapistir').trigger('focus'); };
            if (navigator.clipboard && navigator.clipboard.readText && window.isSecureContext) navigator.clipboard.readText().then(yapistir, pencere);
            else pencere();
        });

        $('#btnYapistirTamam').on('click', function () {
            dlg.close();
            yapistir($('#txtYapistir').val());
        });
    }

    // Tekliften Al (eski TekliftenAl): girilmemiş (0) tutarlara teklif yazılır. eslesme: { girilenAlan: teklifAlani }.
    // Değişen düğümleri döndürür.
    function tekliftenAl(grid, eslesme, satirUygun) {
        grid.stopEditing();
        var degisen = [];
        grid.forEachNode(function (n) {
            var d = n.data;
            if (!satirUygun(d)) return;
            var degisti = false;
            $.each(eslesme, function (hedef, kaynak) {
                if (!Number(d[hedef]) && Number(d[kaynak])) { d[hedef] = Number(d[kaynak]); degisti = true; }
            });
            if (degisti) degisen.push(n);
        });
        if (degisen.length) grid.refreshCells({ force: true });
        return degisen;
    }

    // ---------------------------------------------------------------- toplamlar

    function bosToplam(alanlar, ek) {
        var t = $.extend({}, ek);
        $.each(alanlar, function (i, a) { t[a] = 0; });
        return t;
    }

    function topla(t, d, alanlar) {
        $.each(alanlar, function (i, a) { t[a] += Number(d[a]) || 0; });
    }

    // Satırların arasına proje grubu (kod5) ara toplam satırlarını ekler (eski araToplamliListe; satırlar kod5'e göre sıralı gelir).
    function araToplamla(satirlar, alanlar) {
        var sonuc = [], ara = null;
        $.each(satirlar || [], function (i, d) {
            if (!ara || ara.kod5 !== d.kod5) {
                ara = bosToplam(alanlar, { id: '@@@@' + (d.kod5 || '') + '@', ara: true, kod5: d.kod5, ad5: d.ad5 });
                sonuc.push(ara);
            }
            topla(ara, d, alanlar);
            sonuc.push(d);
        });
        return sonuc;
    }

    // Ara toplam satırlarını yeniden hesaplar ve TOPLAM'ı (ara satırlar hariç) döndürür.
    function toplamlariGuncelle(grid, alanlar) {
        var t = bosToplam(alanlar, { id: '00', toplam: true });
        var ara = null, aralar = [];
        grid.forEachNode(function (n) {
            var d = n.data;
            if (d.ara) {
                ara = d;
                $.each(alanlar, function (i, a) { ara[a] = 0; });
                aralar.push(n);
                return;
            }
            if (ara && ara.kod5 === d.kod5) topla(ara, d, alanlar);
            topla(t, d, alanlar);
        });
        if (aralar.length) grid.refreshCells({ rowNodes: aralar, force: true });
        return t;
    }

    return {
        mesaj: mesaj, hataGoster: hataGoster, sayiCoz: sayiCoz, para: para, giris: giris, sayiSutunu: sayiSutunu,
        soruKur: soruKur, raporMenuKur: raporMenuKur, kopyala: kopyala, yapistirKur: yapistirKur, tekliftenAl: tekliftenAl,
        araToplamla: araToplamla, toplamlariGuncelle: toplamlariGuncelle
    };
})(jQuery);
