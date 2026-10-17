// AHP / AGP Girişi (ButceUygulama/APGiris?tur=gider | gelir; eski APGiris.aspx): seçilen yıl, aşama ve tertip kodlarına göre tertiplerin
// KBÖ'leri ve 12 aylık dağılımları AG Grid'de listelenir ve girilir. Kilitli aylara (sistem yöneticisinin belirlediği aya kadar) giriş
// yapılmaz. Oran Uygula birinci satıra yazılan yüzdelerle bütün satırları dağıtır; Önc. Yıl Oran birinci satıra geçen yılın oranlarını
// getirir. İndir / Yükle Excel ile, raporlar Excel olarak alınır; imzalar pencerede girilir. Uçlar: /api/apgiris/* (ButceUygulama/APGirisUclari.cs).
$(function () {
    'use strict';

    var $kok = $('.ap-giris');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TUR = $kok.attr('data-tur') === 'gelir' ? 'gelir' : 'gider';
    var AYLAR = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
    var kilitliAy = 0;
    var kirli = false;
    var tutarBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 0, maximumFractionDigits: 2 });
    var yuzdeBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    // Uç { hata } ya da { bilgi } döndürür.
    function sonucGoster(v) {
        if (v.hata) { mesaj(v.hata, 'danger'); return false; }
        if (v.bilgi) mesaj(v.bilgi, v.uyariVar ? 'warning' : 'success');
        return true;
    }

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }
    function asamaOku() { return parseInt($('#ddlAsama').val(), 10) || 0; }
    function alanMetin(alan) { return $.trim($('.kod-alan[data-alan="' + alan + '"] input').val() || ''); }
    // "13.01.31.00- Ad" -> "13.01.31.00"
    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }

    // Kod alanları ad ile birlikte gönderilir; sunucu kodu "-" öncesinden alır (eski TiredenAl). Raporda birim adı da buradan gelir.
    function secimOku() {
        return { tur: TUR, yil: yilOku(), asama: asamaOku(), kur: alanMetin('kur'), fon: alanMetin('fon'), fin: alanMetin('fin'), eko: alanMetin('eko'), peb: alanMetin('peb') };
    }

    function ayAcikMi(ay) { return ay > kilitliAy; }

    // ---------------------------------------------------------------- hesaplar (eski VeriGiris, YuzdeHesap, CeyrekToplam ...)

    function sayi(v) { v = Number(v); return isFinite(v) ? v : 0; }
    function ay(d, no) { return sayi(d['a' + no]); }
    function ceyrekToplam(d, cey) { var a = (cey - 1) * 3 + 1; return ay(d, a) + ay(d, a + 1) + ay(d, a + 2); }
    function dagitilan(d) { return ceyrekToplam(d, 1) + ceyrekToplam(d, 2) + ceyrekToplam(d, 3) + ceyrekToplam(d, 4); }
    function yuzde(deger, odenek) { odenek = sayi(odenek); return odenek ? deger * 100 / odenek : null; }

    // Türkçe biçimli ya da düz sayı: "1.234.567,89", "1234567.89", "1234567"
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = $.trim(String(metin == null ? '' : metin)).replace(/\s/g, '');
        if (s === '') return 0;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        else if (/^-?\d{1,3}(\.\d{3})+$/.test(s)) s = s.replace(/\./g, '');
        var v = parseFloat(s);
        return isFinite(v) ? v : null;
    }

    function tutarYaz(p) { return p.value == null || p.value === '' ? '' : tutarBicim.format(p.value); }
    function yuzdeYaz(p) { return p.value == null ? '' : yuzdeBicim.format(p.value); }

    // ---------------------------------------------------------------- tablo

    function hesapSutun(ozellik) {
        return $.extend({ type: 'rightAligned', cellClass: ['sayi', 'hesap'], editable: false, sortable: false }, ozellik);
    }

    function aySutun(no) {
        return {
            field: 'a' + no, colId: 'a' + no, headerName: AYLAR[no - 1], width: 98, type: 'rightAligned', sortable: false,
            editable: function (p) { return !p.node.rowPinned && ayAcikMi(no); },
            cellClass: function (p) { return ayAcikMi(no) || p.node.rowPinned ? ['sayi'] : ['sayi', 'kilitli']; },
            valueFormatter: tutarYaz,
            valueParser: function (p) { var v = sayiCoz(p.newValue); return v == null ? p.oldValue : v; }
        };
    }

    function ayYuzdeSutun(no) {
        return hesapSutun({ colId: 'a' + no + 'y', headerName: '%', width: 64, valueGetter: function (p) { return yuzde(ay(p.data, no), p.data.odenek); }, valueFormatter: yuzdeYaz });
    }

    function donemSutunlari(cey) {
        var cocuklar = [];
        for (var i = 1; i <= 3; i++) {
            var no = (cey - 1) * 3 + i;
            cocuklar.push(aySutun(no), ayYuzdeSutun(no));
        }
        cocuklar.push(
            hesapSutun({ colId: 'q' + cey, headerName: cey + '. Dönem', width: 104, valueGetter: function (p) { return ceyrekToplam(p.data, cey); }, valueFormatter: tutarYaz }),
            hesapSutun({ colId: 'q' + cey + 'y', headerName: '%', width: 64, valueGetter: function (p) { return yuzde(ceyrekToplam(p.data, cey), p.data.odenek); }, valueFormatter: yuzdeYaz })
        );
        return { headerName: cey + '. Dönem', children: cocuklar };
    }

    var sutunlar = [
        {
            field: 'tertip', headerName: 'Tertip', width: 290, pinned: 'left', cellClass: ['hesap', 'ap-tertip'], sortable: true,
            tooltipValueGetter: function (p) { return p.data ? (p.data.tertip || '') + (p.data.tertipAdi ? ' ' + p.data.tertipAdi : '') : ''; },
            cellRenderer: function (p) {
                var e = document.createElement('div');
                e.className = 'ap-tertip';
                e.appendChild(document.createTextNode(p.data.tertip || ''));
                if (p.data.tertipAdi) {
                    var k = document.createElement('small');
                    k.textContent = p.data.tertipAdi;
                    e.appendChild(k);
                }
                return e;
            }
        },
        hesapSutun({ field: 'odenek', headerName: 'KBÖ', width: 110, pinned: 'left', valueFormatter: tutarYaz }),
        hesapSutun({ colId: 'dagitilan', headerName: 'Dağıtılan', width: 110, pinned: 'left', valueGetter: function (p) { return dagitilan(p.data); }, valueFormatter: tutarYaz }),
        hesapSutun({ colId: 'odenekYuzde', headerName: '%', width: 64, pinned: 'left', valueGetter: function (p) { return yuzde(dagitilan(p.data), p.data.odenek); }, valueFormatter: yuzdeYaz }),
        hesapSutun({
            colId: 'bloke', headerName: 'Bloke', width: 104, valueGetter: function (p) { return sayi(p.data.odenek) - dagitilan(p.data); }, valueFormatter: tutarYaz,
            cellClassRules: { eksi: function (p) { return p.value < 0; } }
        }),
        hesapSutun({ field: 'serbest', headerName: 'Serbest', width: 104, valueFormatter: tutarYaz }),
        donemSutunlari(1), donemSutunlari(2), donemSutunlari(3), donemSutunlari(4)
    ];

    var BOS_IPUCU = '<span>Yılı, aşamayı ve kodları seçip Listele\'ye basınız.</span>';
    var grid = Sgb.grid('#grdGrid', {
        columnDefs: sutunlar,
        rowHeight: 40,
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVerticallyAfterEdit: true,
        overlayNoRowsTemplate: BOS_IPUCU,
        onCellValueChanged: function () { kirli = true; toplamYenile(); }
    });

    function satirlar() {
        var r = [];
        grid.forEachNode(function (n) { if (n.data) r.push(n.data); });
        return r;
    }

    // Toplam satırı (eski "T O P L A M" satırı) tablonun altında sabittir ve her değişiklikte yeniden hesaplanır.
    function toplamHesapla(r) {
        var t = { tertip: '', tertipAdi: 'T O P L A M', odenek: 0, serbest: 0 };
        for (var i = 1; i <= 12; i++) t['a' + i] = 0;
        $.each(r, function (j, d) {
            t.odenek += sayi(d.odenek);
            t.serbest += sayi(d.serbest);
            for (var i = 1; i <= 12; i++) t['a' + i] += ay(d, i);
        });
        return t;
    }

    function toplamYenile() {
        var r = satirlar();
        grid.setGridOption('pinnedBottomRowData', r.length ? [toplamHesapla(r)] : []);
    }

    function tabloDoldur(r) {
        grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
        grid.setGridOption('rowData', r || []);
        kirli = false;
        toplamYenile();
    }

    // Yüzdeler / Toplamlar (eski KolonGizleAc): ay yüzdeleri, dönem toplamları, dönem yüzdeleri.
    function sutunlariAyarla() {
        var y = $('#chkYuzde').prop('checked'), t = $('#chkToplam').prop('checked');
        var goster = [], gizle = [];
        for (var i = 1; i <= 12; i++) (y ? goster : gizle).push('a' + i + 'y');
        for (var c = 1; c <= 4; c++) {
            (t ? goster : gizle).push('q' + c);
            (t && y ? goster : gizle).push('q' + c + 'y');
        }
        grid.setColumnsVisible(goster, true);
        grid.setColumnsVisible(gizle, false);
    }
    $('#chkYuzde, #chkToplam').on('change', sutunlariAyarla);

    function duzenlemeBitir() { grid.stopEditing(); }

    function degisiklikVarSor() {
        duzenlemeBitir();
        return !kirli || window.confirm('Kaydedilmemiş değişiklikler var. Devam ederseniz kaybolacak. Devam edilsin mi?');
    }

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- Listele (eski btnListele_Click)

    function listele() {
        if (!degisiklikVarSor()) return;
        var o = secimOku();
        if (o.yil <= 0) { mesaj('Yıl seçiniz.', 'warning'); return; }
        var $d = $('#btnOku').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/apgiris/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                tabloDoldur(v.satirlar);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }
    $('#btnOku').on('click', listele);
    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(); });

    // ---------------------------------------------------------------- Kaydet, Onayla, Onay Kaldır

    function kayitSatirlari() {
        return $.map(satirlar(), function (d) {
            var s = { tertip: d.tertip || '', odenek: sayi(d.odenek) };
            for (var i = 1; i <= 12; i++) s['a' + i] = ay(d, i);
            return s;
        });
    }

    $('#btnKaydet').on('click', function () {
        duzenlemeBitir();
        var o = secimOku();
        if (o.yil <= 0 || o.asama <= 0) { mesaj('Yıl ve Aşama Bilgileri Boş Geçilemez.', 'warning'); return; }
        var r = kayitSatirlari();
        if (!r.length) { mesaj('Kaydedilecek satır yok; önce listeleyiniz.', 'warning'); return; }
        if (!window.confirm('Kaydetmek istiyor musunuz?')) return;
        o.satirlar = r;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/apgiris/kaydet', o)
            .done(function (v) {
                if (v.basarili) { kirli = false; mesaj(v.bilgi, 'success'); }
                else mesaj(v.hata || 'Kaydetme İşlemi tamamlanamamıştır.', 'danger');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    function onayIslem(uc, $d) {
        var o = secimOku();
        if (o.yil <= 0 || o.asama <= 0) { mesaj('Yıl ve Aşama Bilgileri Boş Geçilemez.', 'warning'); return; }
        $d.prop('disabled', true);
        Sgb.gonder(uc, o).done(sonucGoster).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    }
    $('#btnOnay').on('click', function () { onayIslem('api/apgiris/onayla', $(this)); });
    $('#btnOnayKaldir').on('click', function () { onayIslem('api/apgiris/onaykaldir', $(this)); });

    // ---------------------------------------------------------------- Oran Uygula (eski OranUygula), Önc. Yıl Oran (eski OranYaz)

    function yuvarla(deger, hane) { var us = Math.pow(10, hane); return Math.round(deger * us) / us; }

    $('#btnOranUygula').on('click', function () {
        duzenlemeBitir();
        var dugumler = [];
        grid.forEachNode(function (n) { if (n.data) dugumler.push(n); });
        if (!dugumler.length) { mesaj('Önce listeleyiniz.', 'warning'); return; }
        var ilk = dugumler[0].data;
        var oranlar = [], yuzdeToplam = 0, ayNo;
        for (ayNo = 1; ayNo <= kilitliAy; ayNo++) {
            var o1 = yuvarla(yuzde(ay(ilk, ayNo), ilk.odenek) || 0, 2);
            oranlar[ayNo] = o1;
            yuzdeToplam = yuvarla(yuzdeToplam + o1, 2);
        }
        for (ayNo = kilitliAy + 1; ayNo <= 12; ayNo++) {
            var o2 = ay(ilk, ayNo);
            if (o2 < 0) { mesaj('Ay değerleri sıfırdan küçük olamaz!', 'warning'); return; }
            oranlar[ayNo] = o2;
            yuzdeToplam = yuvarla(yuzdeToplam + o2, 2);
        }
        if (yuzdeToplam < 0) { mesaj('Toplam sıfırdan küçük olamaz!', 'warning'); return; }
        if (yuzdeToplam > 100) { mesaj('Birinci satır toplamı yüzden büyük olamaz! ' + yuzdeToplam, 'warning'); return; }

        $.each(dugumler, function (i, n) {
            var d = n.data, satToplam = 0, a;
            for (a = 1; a <= kilitliAy; a++) satToplam += ay(d, a);
            for (a = kilitliAy + 1; a <= 12; a++) {
                var hesaplanan = a === 12 && yuzdeToplam > 99 ? sayi(d.odenek) - satToplam : Math.floor(sayi(d.odenek) * oranlar[a] / 100);
                satToplam += hesaplanan;
                d['a' + a] = hesaplanan;
            }
        });
        kirli = true;
        grid.refreshCells({ force: true });
        toplamYenile();
        mesaj('Oranlar uygulandı; kaydetmeyi unutmayınız.', 'info');
    });

    $('#btnOranGetir').on('click', function () {
        duzenlemeBitir();
        var dugum = grid.getDisplayedRowAtIndex(0);
        var tertip = dugum && dugum.data ? dugum.data.tertip : '';
        if (!tertip) { mesaj('Bu işlemi yapmadan önce listeleme yaparak ekrana tertip getiriniz!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.al('api/apgiris/oncekioran', { tur: TUR, yil: yilOku(), tertip: tertip })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                for (var a = kilitliAy + 1; a <= 12; a++) dugum.data['a' + a] = v.oranlar[a - 1];
                kirli = true;
                grid.refreshCells({ rowNodes: [dugum], force: true });
                toplamYenile();
                mesaj('Birinci satıra bir önceki yılın oranları yazıldı; Oran Uygula ile dağıtınız.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Ay kilitle (sistem yöneticisi)

    $('#btnAyKilitle').on('click', function () {
        var a = parseInt($('#spnKilitliAy').val(), 10);
        if (!(a >= 0 && a <= 12)) { mesaj('Ay 0 ile 12 arasında olmalı.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/apgiris/aykilitle', { tur: TUR, ay: a })
            .done(function (v) {
                if (!sonucGoster(v)) return;
                kilitliAy = a;
                grid.refreshCells({ force: true });
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- İndir menüsü

    var $menu = $('.ap-menu');
    $('#btnMenu').on('click', function (e) {
        e.stopPropagation();
        var acik = !$menu.hasClass('open');
        $menu.toggleClass('open', acik);
        $(this).attr('aria-expanded', acik ? 'true' : 'false');
    });
    $(document).on('click', function () { $menu.removeClass('open'); $('#btnMenu').attr('aria-expanded', 'false'); });

    // POST ile dosya indirir (tablodaki satırlar gövdede gider).
    function postIndir(uc, veri, varsayilanAd) {
        return fetch(Sgb.KOK + uc, {
            method: 'POST', credentials: 'same-origin', cache: 'no-store',
            headers: { 'Content-Type': 'application/json', RequestVerificationToken: Sgb.meta('istek-jetonu') },
            body: JSON.stringify(veri)
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
                a.download = ad ? decodeURIComponent(ad[1]) : varsayilanAd;
                document.body.appendChild(a);
                a.click();
                a.remove();
                setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
            });
        }).catch(function () { mesaj('Dosya hazırlanamadı.', 'danger'); });
    }

    $menu.on('click', 'a[data-islem]', function (e) {
        e.preventDefault();
        $menu.removeClass('open');
        duzenlemeBitir();
        var $a = $(this), islem = $a.attr('data-islem');
        var o = secimOku();
        if (islem === 'indir') {
            var r = kayitSatirlari();
            if (!r.length) { mesaj('İndirilecek satır yok; önce listeleyiniz.', 'warning'); return; }
            var t = toplamHesapla(satirlar());
            var ts = { tertip: '', odenek: t.odenek };
            for (var i = 1; i <= 12; i++) ts['a' + i] = t['a' + i];
            r.push(ts);
            postIndir('api/apgiris/indir', { tur: TUR, sade: parseInt($a.attr('data-sade'), 10) || 0, satirlar: r }, 'APGiris.xlsx');
        } else if (islem === 'prgindir') {
            if (o.yil <= 0) { mesaj('Yıl seçiniz.', 'warning'); return; }
            o.seviye = $a.attr('data-seviye');
            Sgb.indir('api/apgiris/prgindir', o);
        } else if (islem === 'prggonder') {
            if (o.yil <= 0) { mesaj('Yıl seçiniz.', 'warning'); return; }
            if (!window.confirm($a.text() + ': AHP SBB\'ye gönderilecek. Onaylıyor musunuz?')) return;
            o.seviye = parseInt($a.attr('data-seviye'), 10) || 4;
            mesaj('SBB\'ye gönderiliyor, lütfen bekleyiniz...', 'info');
            Sgb.gonder('api/apgiris/prggonder', o).done(sonucGoster).fail(hataGoster);
        } else if (islem === 'prgal') {
            if (o.yil <= 0) { mesaj('Yıl seçiniz.', 'warning'); return; }
            if (!window.confirm('SBB\'den veriler çekilecek onaylıyor musunuz?')) return;
            mesaj('SBB\'den veriler alınıyor, lütfen bekleyiniz...', 'info');
            grid.setGridOption('loading', true);
            Sgb.gonder('api/apgiris/prgal', o).done(sonucGoster).fail(hataGoster)
                .always(function () { grid.setGridOption('loading', false); });
        }
    });

    // ---------------------------------------------------------------- Raporlar (eski btnListe_Click)

    $('#btnRapor').on('click', function () {
        var o = secimOku();
        if (o.yil <= 0) { mesaj('Yıl veya Kurum alanlarından biri veya bir kaçı boş bırakılmış. Lütfen ilgili alanları kontrol edip tekrar deneyin.', 'warning'); return; }
        var p = { tur: TUR, rapor: $('#ddlRaporlar').val(), imzali: $('#chkImzaFormat').prop('checked') ? 1 : 0, yil: o.yil, asama: o.asama, kur: o.kur, eko: o.eko, peb: o.peb };
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/apgiris/rapor', p).then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Dosya Yükle / Eko 2 AHP (eski btnDosyaYukle_Click, btnDosyaAHP2Duzey_Click)

    var yuklemeTuru = null;
    $('#btnDosyaYukle').on('click', function () {
        if (!degisiklikVarSor()) return;
        yuklemeTuru = 'yukle';
        $('#fuListe').val('').trigger('click');
    });
    $('#btnEko2').on('click', function () {
        if (asamaOku() < 2) { mesaj('Bu işlem için kaynak aşama, bir önceki aşama olduğundan, birinci aşama seçili iken bu işlem yapılamaz', 'warning'); return; }
        yuklemeTuru = 'eko2yukle';
        $('#fuListe').val('').trigger('click');
    });
    $('#fuListe').on('change', function () {
        var dosya = this.files[0];
        if (!dosya || !yuklemeTuru) return;
        var eko2 = yuklemeTuru === 'eko2yukle';
        if (eko2 && !window.confirm('Dosyadaki tertipler bir önceki aşamanın dağılımına göre alt tertiplere dağıtılıp seçili aşamaya kaydedilecek. Onaylıyor musunuz?')) return;
        var o = secimOku();
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('tur', TUR);
        veri.append('yil', o.yil);
        veri.append('asama', o.asama);
        veri.append('kur', o.kur);
        var $d = $(eko2 ? '#btnEko2' : '#btnDosyaYukle').prop('disabled', true);
        $.ajax({
            url: Sgb.KOK + 'api/apgiris/' + yuklemeTuru, method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            if (!eko2) {
                tabloDoldur(v.satirlar);
                kirli = (v.satirlar || []).length > 0;
            }
            if (v.bilgi) {
                if (v.uyariVar) window.alert(v.bilgi);
                else mesaj(v.bilgi, 'success');
            }
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else hataGoster(xhr);
        }).always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- İmzalar (eski MasrafPlaniGenelImza.aspx)

    var dlgImza = document.getElementById('dlgImza');
    $('#btnImza').on('click', function () {
        $('#frmImza input').val('');
        dlgImza.showModal();
        Sgb.al('api/apgiris/imza')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#txtHazGorev').val(v.HazGorev || '');
                $('#txtHazKisi').val(v.HazKisi || '');
                $('#txtImzGorev').val(v.ImzGorev || '');
                $('#txtImzKisi').val(v.ImzKisi || '');
            })
            .fail(hataGoster);
    });
    $('#btnImzaKaydet').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/apgiris/imza', {
            HazGorev: $('#txtHazGorev').val(), HazKisi: $('#txtHazKisi').val(),
            ImzGorev: $('#txtImzGorev').val(), ImzKisi: $('#txtImzKisi').val()
        }).done(function (v) { if (sonucGoster(v)) dlgImza.close(); })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });
    $('#frmImza').on('submit', function (e) { e.preventDefault(); $('#btnImzaKaydet').trigger('click'); });

    // ---------------------------------------------------------------- kod ağaçları (eski açılır ağaçlar, nodeLoadPar)

    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};  // kod türü -> { $ul, agac, yil }
    var aktifAlan = null;

    function agacAc($alan) {
        var yil = yilOku();
        if (yil <= 0) { mesaj('Yıl seçiniz.', 'warning'); return; }
        var kod = $alan.attr('data-kod');
        var ad = $alan.find('label').text();
        aktifAlan = $alan;
        $('#dlgKodBaslik').text(ad + ' Kod Seçimi');
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[kod];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[kod] = { $ul: $('<ul></ul>').attr('aria-label', ad + ' ağacı').appendTo('#agacKutu'), yil: yil };
            // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al('api/apgiris/kodlar', { tur: TUR, yil: a.yil, kod: kod, ust: ust ? ust.kod : '' }).then(function (v) {
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
    $('.kod-alan input').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al('api/apgiris/tanim', { tur: TUR })
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            var $y = Sgb.yilKutusu('#ddlYil', t.yillar);
            $y.val(String(t.yil));
            var $a = $('#ddlAsama').empty();
            $.each(t.asamalar || [], function (i, a) { $('<option></option>').val(a.kod).text(a.ad).appendTo($a); });
            $a.val(String(t.asama));
            $.each(['kur', 'fon', 'fin', 'eko', 'peb'], function (i, ad) { $('.kod-alan[data-alan="' + ad + '"] input').val(t[ad] || ''); });
            kilitliAy = t.kilitliAy || 0;
            $('#spnKilitliAy').val(kilitliAy);
            $('.yonetici').prop('hidden', !t.yonetici);
            grid.refreshCells({ force: true });
        })
        .fail(hataGoster);
});
