// Tertip tanımları (ButceHazirlikv2/TertipTanimFormu; ?yil=..&Tur=1 gider / 2 gelir): soldaki kodlara uyan tanımlı tertipler listelenir,
// tertip kodu ve adı tabloda düzenlenip kaydedilir (adı boş bırakılan tertip silinir). Ad Üret kodlardan ad yazar, Tertip Üret seçilen
// kodların bütün birleşimlerini getirir, Dosya Yükle Excel'deki satırları getirir; üçü de kaydetmez. Tabloda hep 10 boş satır vardır.
$(function () {
    'use strict';

    var $sayfa = $('.tertip-tanim');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TUR = parseInt($sayfa.attr('data-tur'), 10) === 2 ? 2 : 1;
    var sira = 0;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }

    // Kod alanı "kod- ad" ya da yazılmış kod: tireden önceki kısım.
    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }
    function alanKod(alan) { return kodAl($('.kod-alan[data-alan="' + alan + '"] input').val()); }

    function secim() {
        return { tur: TUR, yil: yilOku(), kur: alanKod('kur'), fon: alanKod('fon'), fin: alanKod('fin'), eko: alanKod('eko'), peb: alanKod('peb') };
    }

    // ---------------------------------------------------------------- tablo

    function hucreSinifi(p) {
        var s = ['girilir'];
        if (p.data && p.data.dg) s.push('degisti');
        if (/^HATA!/.test(String(p.data && p.data.tertipAdi || '')) && p.colDef.field === 'tertipAdi') s.push('hatali');
        return s;
    }

    var grid = Sgb.grid('#grdTertip', {
        columnDefs: [
            { headerName: '', width: 56, sortable: false, resizable: false, valueGetter: 'node.rowIndex + 1', cellClass: 'sira' },
            { field: 'tertipKodu', headerName: 'Tertip', width: 280, editable: true, cellClass: hucreSinifi, tooltipField: 'tertipKodu' },
            { field: 'tertipAdi', headerName: 'Tertip Adı', flex: 1, minWidth: 240, editable: true, cellClass: hucreSinifi, tooltipField: 'tertipAdi' }
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
            e.data.dg = true;
            e.api.refreshCells({ rowNodes: [e.node], force: true });
        }
    });

    function satirlar(liste, bosEkle) {
        var r = $.map(liste || [], function (t) { return { id: 's' + (sira++), tertipKodu: t.tertipKodu || '', tertipAdi: t.tertipAdi || '' }; });
        if (bosEkle) for (var i = 0; i < 10; i++) r.push({ id: 's' + (sira++), tertipKodu: '', tertipAdi: '' });
        return r;
    }

    function doldur(liste, degisti) {
        var r = satirlar(liste, false);
        if (degisti) $.each(r, function (i, s) { if (s.tertipKodu) s.dg = true; });
        grid.setGridOption('rowData', r);
    }

    function tablo(sadeceDegisen) {
        grid.stopEditing();
        var r = [];
        grid.forEachNode(function (n) {
            if (sadeceDegisen && !n.data.dg) return;
            r.push({ tertipKodu: n.data.tertipKodu || '', tertipAdi: n.data.tertipAdi || '' });
        });
        return r;
    }

    function kirliMi() { var v = false; grid.forEachNode(function (n) { if (n.data.dg) v = true; }); return v; }

    function degisiklikVarsaSor() {
        return !kirliMi() || window.confirm('Kaydedilmemiş değişiklikler var. Devam ederseniz kaybolacak. Devam edilsin mi?');
    }

    // ---------------------------------------------------------------- işlemler

    function listele(fonUyumsuz) {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!degisiklikVarsaSor()) return;
        o.fonUyumsuz = !!fonUyumsuz;
        grid.setGridOption('loading', true);
        Sgb.al('api/tertiptanim/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'warning'); return; }
                doldur(v.tertipler, false);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(false); });
    $('#btnListeleFon').on('click', function () { listele(true); });

    function kaydet(sadeceDegisen, $d) {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Anahtar alanları girmelisiniz.', 'warning'); return; }
        var r = $.grep(tablo(sadeceDegisen), function (t) { return $.trim(t.tertipKodu) !== ''; });
        if (!r.length) { mesaj(sadeceDegisen ? 'Kayıtlarda değişiklik yapılmamış!' : 'Kaydedilecek tertip yok.', 'warning'); return; }
        $d.prop('disabled', true);
        Sgb.gonder('api/tertiptanim/kaydet', { tur: TUR, yil: yil, tertipler: r })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (s.basarili) {
                    grid.forEachNode(function (n) { delete n.data.dg; });
                    grid.refreshCells({ force: true });
                }
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    }

    $('#btnKaydet').on('click', function () { kaydet(false, $(this)); });
    $('#btnKaydetDeg').on('click', function () { kaydet(true, $(this)); });

    // Ad Üret: dönen kod ve adlar tabloya yazılır (değişti sayılır).
    $('#btnAdUret').on('click', function () {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var dugumler = [];
        grid.forEachNode(function (n) { dugumler.push(n); });
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/tertiptanim/adlar', { tur: TUR, yil: o.yil, peb: o.peb, tertipler: tablo(false) })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $.each(v.tertipler || [], function (i, t) {
                    var n = dugumler[i];
                    if (!n || !t.tertipKodu) return;
                    if (n.data.tertipKodu !== t.tertipKodu || n.data.tertipAdi !== t.tertipAdi) n.data.dg = true;
                    n.data.tertipKodu = t.tertipKodu;
                    n.data.tertipAdi = t.tertipAdi;
                });
                grid.refreshCells({ force: true });
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnTertipUret').on('click', function () {
        var o = secim();
        if (o.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!degisiklikVarsaSor()) return;
        var $d = $(this).prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/tertiptanim/tertipuret', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'warning'); return; }
                doldur(v.tertipler, true);
                mesaj((v.tertipler || []).length + ' tertip üretildi; kaydetmek için Kaydet\'e basınız.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    });

    // İndir: tablodaki satırlar sunucuda Excel'e yazılır (POST; dosya Content-Disposition adıyla indirilir).
    $('#btnIndir').on('click', function () {
        var $d = $(this).prop('disabled', true);
        fetch(Sgb.KOK + 'api/tertiptanim/indir', {
            method: 'POST', credentials: 'same-origin', cache: 'no-store',
            headers: { 'Content-Type': 'application/json', RequestVerificationToken: Sgb.meta('istek-jetonu') },
            body: JSON.stringify({ tur: TUR, yil: yilOku(), tertipler: tablo(false) })
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

    $('#btnDosyaYukle').on('click', function () { $('#dosyaYukle').val('').trigger('click'); });
    $('#dosyaYukle').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        if (!window.confirm('Seçtiğiniz dosyayı yüklemek istiyor musunuz?\n\nDosyada başlık satırı olmamalı; sütunlar: tertip kodu, tertip adı. ' +
            'Satırlar tabloya gelir, kaydetmek için Kaydet\'e basınız.')) return;
        if (!degisiklikVarsaSor()) return;
        var veri = new FormData();
        veri.append('tur', TUR);
        veri.append('dosya', dosya);
        var $d = $('#btnDosyaYukle').prop('disabled', true);
        $.ajax({
            url: Sgb.KOK + 'api/tertiptanim/yukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            doldur(v.tertipler, true);
        }).fail(function (xhr) {
            if (xhr.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            hataGoster(xhr);
        }).always(function () { $d.prop('disabled', false); });
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirliMi()) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- kod ağaçları

    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};
    var aktifAlan = null;
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    function agacAc($alan) {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        var kodTur = $alan.attr('data-tur');
        aktifAlan = $alan;
        $('#dlgKodBaslik').text($alan.find('label').text() + ' Kod Seçimi');
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[kodTur];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[kodTur] = { $ul: $('<ul></ul>').attr('aria-label', $alan.find('label').text() + ' kod ağacı').appendTo('#agacKutu'), yil: yil };
            // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al('api/tertiptanim/kodlar', { tur: TUR, yil: a.yil, kodTur: kodTur, ust: ust ? ust.kod : '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    dlgKod.close();
                    if (aktifAlan) aktifAlan.find('input').val(d.kod);
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
    $('.kod-alan input').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
    });

    // ---------------------------------------------------------------- açılış

    grid.setGridOption('rowData', satirlar([], true));
    var adresYil = parseInt($sayfa.attr('data-yil'), 10) || 0;
    Sgb.al('api/tertiptanim/tanim', { tur: TUR, yil: adresYil || undefined })
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            $('#txtYil').val(t.yil);
            $('.kod-alan[data-alan="kur"] input').val(t.kurum || '');
        })
        .fail(hataGoster);
});
