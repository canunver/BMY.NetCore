// Tertip dönüşümü (eski ButceHazirlikv2/TertipDonusumFormu.aspx). Analitik tertip ile program tertibi arasında çeviri:
// satırlar elle, Excel'den (Dosya Yükle) ya da panodan (Yapıştır) girilir; Dönüştür her satırın boş tarafını diğerinden üretir.
// İndir tabloyu Excel'e, Kopyala panoya yazar. Bir şey saklanmaz.
(function ($) {
    'use strict';

    var $sayfa = $('.tertip-donusum');
    if (!$sayfa.length) return;

    var BOS_SATIR = 10;
    var sira = 0;

    $('#txtYil').val(parseInt($sayfa.attr('data-yil'), 10) || new Date().getFullYear());
    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur || 'info'); }

    var grid = Sgb.grid('#grdTertip', {
        columnDefs: [
            { headerName: '', width: 56, sortable: false, resizable: false, valueGetter: 'node.rowIndex + 1', cellClass: 'sira' },
            { field: 'tertipKoduEski', headerName: 'Analitik Tertip', flex: 1, minWidth: 260, editable: true, tooltipField: 'tertipKoduEski',
                cellClass: function (p) { return p.data && p.data.uretilen === 'eski' ? 'girilir uretildi' : 'girilir'; } },
            { field: 'tertipKoduYeni', headerName: 'Program Tertip', flex: 1, minWidth: 260, editable: true, tooltipField: 'tertipKoduYeni',
                cellClass: function (p) { return p.data && p.data.uretilen === 'yeni' ? 'girilir uretildi' : 'girilir'; } }
        ],
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        tooltipShowDelay: 500,
        getRowId: function (p) { return p.data.id; },
        onCellValueChanged: function (e) {
            if (e.newValue === e.oldValue) return;
            e.data.uretilen = null;
            e.api.refreshCells({ rowNodes: [e.node], force: true });
        }
    });

    function satir(eski, yeni) { return { id: 's' + (sira++), tertipKoduEski: eski || '', tertipKoduYeni: yeni || '' }; }

    // Dolu satırlar (en az bir sütunu yazılmış), tablodaki sırasıyla.
    function doluSatirlar() {
        grid.stopEditing();
        var r = [];
        grid.forEachNode(function (n) {
            var d = n.data;
            if ($.trim(d.tertipKoduEski) || $.trim(d.tertipKoduYeni)) r.push({ tertipKoduEski: $.trim(d.tertipKoduEski), tertipKoduYeni: $.trim(d.tertipKoduYeni) });
        });
        return r;
    }

    // Tablo verilen satırlarla doldurulur, sonuna yeni giriş için boş satırlar eklenir.
    function doldur(liste) {
        var r = $.map(liste || [], function (t) { var s = satir(t.tertipKoduEski, t.tertipKoduYeni); s.uretilen = t.uretilen || null; return s; });
        for (var i = 0; i < BOS_SATIR; i++) r.push(satir());
        grid.setGridOption('rowData', r);
    }

    function hataGoster(xhr, varsayilan) {
        if (xhr.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
        mesaj(xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : varsayilan, 'danger');
    }

    // ---------------------------------------------------------------- Dönüştür (eski btnDonustur_Click)
    $('#btnDonustur').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl giriniz.', 'warning'); return; }
        var r = doluSatirlar();
        if (!r.length) { mesaj('Dönüştürülecek tertip yok.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/tertipdonusum/donustur', { yil: yil, tertipler: r }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            var bulunamayan = 0;
            var sonuc = $.map(v.tertipler || [], function (t, i) {
                var once = r[i] || {};
                t.uretilen = !once.tertipKoduEski && t.tertipKoduEski ? 'eski' : (!once.tertipKoduYeni && t.tertipKoduYeni ? 'yeni' : null);
                if (!t.tertipKoduEski || !t.tertipKoduYeni) bulunamayan++;
                return t;
            });
            doldur(sonuc);
            mesaj(bulunamayan ? bulunamayan + ' satır dönüştürülemedi (karşılığı bulunamadı).' : 'Dönüştürme tamamlandı.', bulunamayan ? 'warning' : 'success');
        }).fail(function (xhr) { hataGoster(xhr, 'Dönüştürme yapılamadı.'); })
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- İndir (eski btnDosyaSakla_Click)
    $('#btnIndir').on('click', function () {
        var r = doluSatirlar();
        if (!r.length) { mesaj('İndirilecek tertip yok.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        fetch(Sgb.KOK + 'api/tertipdonusum/indir', {
            method: 'POST', credentials: 'same-origin', cache: 'no-store',
            headers: { 'Content-Type': 'application/json', RequestVerificationToken: Sgb.meta('istek-jetonu') },
            body: JSON.stringify({ yil: yilOku(), tertipler: r })
        }).then(function (y) {
            if (y.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            var tur = y.headers.get('Content-Type') || '';
            if (!y.ok || tur.indexOf('json') >= 0 || tur.indexOf('text/html') >= 0) {
                if (tur.indexOf('json') >= 0) return y.json().then(function (v) { mesaj(v.hata || 'Dosya hazırlanamadı.', 'danger'); });
                mesaj(y.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Dosya hazırlanamadı.', 'danger');
                return;
            }
            var baslik = y.headers.get('Content-Disposition') || '';
            var ad = /filename\*=UTF-8''([^;]+)/i.exec(baslik) || /filename="?([^";]+)"?/i.exec(baslik);
            return y.blob().then(function (icerik) {
                var yerel = URL.createObjectURL(icerik);
                var a = document.createElement('a');
                a.href = yerel;
                a.download = ad ? decodeURIComponent(ad[1]) : 'Tertipler.xlsx';
                document.body.appendChild(a);
                a.click();
                a.remove();
                setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
            });
        }).catch(function () { mesaj('Dosya hazırlanamadı.', 'danger'); })
            .then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Dosya Yükle (eski btnDosyaYukle_Click)
    $('#btnDosyaYukle').on('click', function () { $('#dosyaYukle').val('').trigger('click'); });
    $('#dosyaYukle').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        if (!window.confirm('Seçtiğiniz dosyayı yüklemek istiyor musunuz?\n\nDosyada başlık satırı olmamalı; sütunlar: analitik tertip, program tertip. ' +
            'Tablodaki satırların yerine dosyadakiler gelir.')) return;
        var veri = new FormData();
        veri.append('dosya', dosya);
        var $d = $('#btnDosyaYukle').prop('disabled', true);
        $.ajax({
            url: Sgb.KOK + 'api/tertipdonusum/yukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            doldur(v.tertipler);
            mesaj((v.tertipler || []).length + ' satır yüklendi.', 'info');
        }).fail(function (xhr) { hataGoster(xhr, 'Dosya okunamadı.'); })
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Yapıştır / Kopyala (eski PasteStore / CopyStore)
    var dlgYapistir = document.getElementById('dlgYapistir');
    var dlgKopyala = document.getElementById('dlgKopyala');
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // Panodaki satırlar (sekmeyle ayrılmış: analitik, program) tablodaki dolu satırların sonuna eklenir.
    function yapistir(metin) {
        var satirlar = String(metin || '').split('\n');
        while (satirlar.length && /^\r?$/.test(satirlar[satirlar.length - 1])) satirlar.pop();
        if (!satirlar.length) { mesaj('Panoda yapıştırılacak veri yok.', 'warning'); return; }
        var r = doluSatirlar();
        $.each(satirlar, function (i, s) {
            var d = s.replace(/\r$/, '').split('\t');
            r.push({ tertipKoduEski: $.trim(d[0] || ''), tertipKoduYeni: $.trim(d[1] || '') });
        });
        doldur(r);
        mesaj(satirlar.length + ' satır yapıştırıldı.', 'info');
    }

    $('#btnYapistir').on('click', function () {
        var pencere = function () { $('#txtYapistir').val(''); dlgYapistir.showModal(); $('#txtYapistir').trigger('focus'); };
        if (navigator.clipboard && navigator.clipboard.readText && window.isSecureContext) navigator.clipboard.readText().then(yapistir, pencere);
        else pencere();
    });
    $('#btnYapistirTamam').on('click', function () {
        dlgYapistir.close();
        yapistir($('#txtYapistir').val());
    });

    $('#btnKopyala').on('click', function () {
        var r = doluSatirlar();
        if (!r.length) { mesaj('Kopyalanacak tertip yok.', 'warning'); return; }
        var metin = $.map(r, function (t) { return t.tertipKoduEski + '\t' + t.tertipKoduYeni; }).join('\r\n') + '\r\n';
        var pencere = function () { $('#txtKopyala').val(metin); dlgKopyala.showModal(); $('#txtKopyala').trigger('focus').trigger('select'); };
        if (navigator.clipboard && navigator.clipboard.writeText && window.isSecureContext)
            navigator.clipboard.writeText(metin).then(function () { mesaj(r.length + ' satır panoya kopyalandı.', 'success'); }, pencere);
        else pencere();
    });

    $('#btnTemizle').on('click', function () {
        if (doluSatirlar().length && !window.confirm('Tablodaki bütün satırlar silinecek. Devam edilsin mi?')) return;
        doldur([]);
    });

    doldur([]);
})(jQuery);
