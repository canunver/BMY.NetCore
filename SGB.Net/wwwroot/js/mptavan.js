// Masraf Planı Tavan (ButceHazirlikv2/MPTavanAna; eski MPTavanAna.aspx): seçilen kodlara uyan tertiplerin KBÖ, önceki yıl
// gönderilen / harcanan, birim talebi, gönderilen, MP tavan / detay toplamı ve dağıtılmayan tutarları AG Grid'de listelenir.
// Tavan, detay ve dağıtılmayan tutarına tıklanınca Tavan Girişi (MPTavanGiris) pencerede açılır; İl İzni simgesi tertibin il bazında
// girişini açar / kapatır. Onaylar, hata raporları, tavan raporu, hızlı ödenek dosyaları, Excel'den ve merkezden yükleme.
// Uçlar: /api/mptavan/* (ButceHazirlik/MPTavanUclari.cs).
$(function () {
    'use strict';

    var $kok = $('.mp-tavan');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var senaryo = '';      // son listelemedeki onaylı senaryo (tavan girişine gider)
    var listelendi = false;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function sonucPencere(baslik, metin) {
        $('#dlgSonucBaslik').text(baslik);
        $('#sonucMetin').text(metin);
        document.getElementById('dlgSonuc').showModal();
    }
    // Uç { hata } ya da { bilgi[, uyariVar] } döndürür. Uyarılı uzun sonuçlar pencerede gösterilir.
    function sonucGoster(v) {
        if (v.hata) { mesaj(v.hata, 'danger'); return false; }
        if (v.bilgi) {
            if (v.uyariVar) sonucPencere('Sonuç', v.bilgi);
            else mesaj(v.bilgi, 'success');
        }
        return true;
    }

    function yilOku() { return parseInt($('#ddlYil').val(), 10) || 0; }
    var kodlar = MPTavan.kodAlanlari(yilOku, hataGoster);

    function secimOku() {
        return { yil: yilOku(), kur: kodlar.kod('kur'), fon: kodlar.kod('fon'), fin: kodlar.kod('fin'), eko: kodlar.kod('eko'), peb: kodlar.kod('peb') };
    }

    // ---------------------------------------------------------------- tablo

    function sayi(v) { v = Number(v); return isFinite(v) ? v : 0; }
    function tutarYaz(p) { return MPTavan.tutar(p.value); }

    // Eski YuzdeRenderer: tutar ve altında KBÖ'nün bu tutara göre değişim yüzdesi.
    function yuzdeHucre(p) {
        var e = document.createElement('div');
        e.className = 'mp-iki-satir sayi';
        e.appendChild(document.createTextNode(MPTavan.tutar(p.value)));
        if (!p.node.rowPinned) {
            var k = document.createElement('small');
            var d = sayi(p.value);
            k.textContent = '%: ' + (d > 0 ? MPTavan.yuzde((sayi(p.data.odenek) - d) * 100 / d) : '-');
            e.appendChild(k);
        }
        return e;
    }

    // Eski TavanRenderer / DetayRenderer: tutar bağlantıdır, tavan girişini açar (G: il tavanı, D: merkez detay).
    function bagHucre(dg) {
        return function (p) {
            if (p.node.rowPinned) return MPTavan.tutar(p.value);
            var b = document.createElement('button');
            b.type = 'button';
            b.className = 'btn btn-link mp-bag';
            b.textContent = MPTavan.tutar(p.value);
            b.title = dg === 'D' ? 'Birim tavanlarını aç' : 'İl tavanlarını aç';
            b.addEventListener('click', function () { tavanGirisAc(p.data.tertip, dg); });
            return b;
        };
    }

    // Eski IzinRenderer: il girişi açık / kapalı simgesi; tıklanınca değiştirilir.
    function izinHucre(p) {
        if (p.node.rowPinned) return '';
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-link mp-izin ' + (p.value ? 'izin-var' : 'izin-yok');
        b.title = p.value
            ? 'Bu tertibe ait harcama birimi girişlerini il yapabilmektedir. Girişi kapatmak için tıklayınız.'
            : 'Bu tertibe ait harcama birimi girişlerini il yapaMAMAKtadır. İzin vermek için tıklayınız.';
        b.innerHTML = '<span class="glyphicon ' + (p.value ? 'glyphicon-ok-sign' : 'glyphicon-remove-sign') + '" aria-hidden="true"></span>';
        $('<span class="sr-only"></span>').text(p.value ? 'İl girişi açık' : 'İl girişi kapalı').appendTo(b);
        b.addEventListener('click', function () { ilIzniDegistir(p.node, b); });
        return b;
    }

    function sayiSutun(ozellik) {
        return $.extend({ type: 'rightAligned', width: 128, cellClass: ['sayi'], valueFormatter: tutarYaz }, ozellik);
    }

    var sutunlar = [
        {
            field: 'tertip', headerName: 'Tertip', width: 380, pinned: 'left', cellClass: ['mp-tertip'],
            tooltipValueGetter: function (p) { return p.data ? (p.data.tertip || '') + (p.data.tertipAdi ? ' ' + p.data.tertipAdi : '') : ''; },
            cellRenderer: function (p) {
                var e = document.createElement('div');
                e.className = 'mp-iki-satir';
                e.appendChild(document.createTextNode(p.data.tertip || ''));
                if (p.data.tertipAdi) {
                    var k = document.createElement('small');
                    k.textContent = p.data.tertipAdi;
                    e.appendChild(k);
                }
                return e;
            }
        },
        sayiSutun({ field: 'odenek', headerName: 'KBÖ' }),
        sayiSutun({ field: 'oncGonderilen', headerName: 'Önc. Gönd.', cellRenderer: yuzdeHucre }),
        sayiSutun({ field: 'oncHarcanan', headerName: 'Önc. Harc.', cellRenderer: yuzdeHucre }),
        sayiSutun({ field: 'talep', headerName: 'Birim Talep' }),
        sayiSutun({ field: 'gonderilen', headerName: 'Gönderilen' }),
        sayiSutun({ field: 'tavan', headerName: 'MP Tavan Top.', cellRenderer: bagHucre('G') }),
        sayiSutun({ field: 'detay', headerName: 'MP Detay Top.', cellRenderer: bagHucre('D') }),
        sayiSutun({
            field: 'dagitilmayan', headerName: 'Dağıtılmayan', cellRenderer: bagHucre('G'),
            cellClassRules: { eksi: function (p) { return p.value < 0; } }
        }),
        { field: 'ilIzni', headerName: 'İl İzni', width: 80, cellClass: ['orta'], cellRenderer: izinHucre, sortable: false }
    ];

    var grid = Sgb.grid('#grdMPTavanAna', {
        columnDefs: sutunlar,
        rowHeight: 42,
        tooltipShowDelay: 400,
        overlayNoRowsTemplate: '<span>Yılı ve kodları seçip Listele\'ye basınız.</span>'
    });

    function satirlar() {
        var r = [];
        grid.forEachNode(function (n) { if (n.data) r.push(n.data); });
        return r;
    }

    // Eski sütun başlıklarındaki toplamlar: tablonun altında sabit toplam satırı.
    function toplamYenile() {
        var r = satirlar();
        if (!r.length) { grid.setGridOption('pinnedBottomRowData', []); return; }
        var t = { tertip: 'T O P L A M', tertipAdi: '' };
        $.each(['odenek', 'oncGonderilen', 'oncHarcanan', 'talep', 'gonderilen', 'tavan', 'detay', 'dagitilmayan'], function (i, a) {
            t[a] = 0;
            $.each(r, function (j, d) { t[a] += sayi(d[a]); });
        });
        grid.setGridOption('pinnedBottomRowData', [t]);
    }

    // ---------------------------------------------------------------- Listele (eski btnListele_Click)

    function listele(sessiz) {
        var o = secimOku();
        if (o.yil <= 0) { mesaj('Yıl alanı boş boş bırakılamaz!', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/mptavan/liste', o)
            .done(function (v) {
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                if (v.hata) {
                    grid.setGridOption('rowData', []);
                    toplamYenile();
                    if (!sessiz) mesaj(v.hata, 'danger');
                    return;
                }
                senaryo = v.senaryo || '';
                listelendi = true;
                grid.setGridOption('rowData', v.satirlar || []);
                toplamYenile();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }
    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(false); });

    // ---------------------------------------------------------------- Tavan girişi (eski AdresAc -> MPTavanGiris.aspx)

    function tavanGirisAc(tertip, dg) {
        var adres = 'MPTavanGiris?' + $.param({ yil: yilOku(), senaryo: senaryo, tertip: tertip, DG: dg });
        Sgb.pencere(adres, dg === 'D' ? 'Harcama Birimi Tavan Girişi' : 'İl Tavan Girişi', function () {
            // Pencerede kaydedilen tavanlar listeye yansısın.
            if (listelendi) listele(true);
        });
    }

    // ---------------------------------------------------------------- İl izni (eski IlIzniDegistir -> MPTavanIlGirisDegistir.aspx)

    function ilIzniDegistir(dugum, dugme) {
        dugme.disabled = true;
        Sgb.gonder('api/mptavan/ilizni', { yil: yilOku(), tertip: dugum.data.tertip })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                dugum.setDataValue('ilIzni', !!v.ilIzni);
                mesaj(v.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { dugme.disabled = false; });
    }

    // ---------------------------------------------------------------- Onay (eski OnayDurumDegistir, wndOnay)

    var dlgOnay = document.getElementById('dlgOnay');
    var onayIslemKod = 0;
    var fazlaGrid = Sgb.grid('#grdGonderileniFazlalar', {
        columnDefs: [
            { field: 'TertipKod', headerName: 'Tertip', flex: 1, minWidth: 260 },
            sayiSutun({ field: 'Odenek1', headerName: 'Masraf Planı', width: 130 }),
            sayiSutun({ field: 'Odenek0', headerName: 'Gönderilen', width: 130 })
        ],
        overlayNoRowsTemplate: '<span>Gönderileni fazla olan tertip yok.</span>'
    });

    function onayGonder(kod, $d) {
        var o = secimOku();
        if ($d) $d.prop('disabled', true);
        return Sgb.gonder('api/mptavan/onay', { yil: o.yil, peb: o.peb, kod: kod })
            .done(sonucGoster)
            .fail(hataGoster)
            .always(function () { if ($d) $d.prop('disabled', false); });
    }

    // 1-4: önce gönderileni fazla olanlar pencerede gösterilir, Onayla ile işlem yapılır; 5-6 doğrudan.
    function onayDurumDegistir(kod) {
        var o = secimOku();
        if (o.yil <= 0) { mesaj('Yıl alanı boş boş bırakılamaz!', 'warning'); return; }
        if (kod > 4) {
            if (!window.confirm('İşlem yapılacak, onaylıyor musunuz?')) return;
            onayGonder(kod);
            return;
        }
        onayIslemKod = kod;
        var basliklar = { 1: 'Onayla Tavan', 2: 'Onayla Detay', 3: 'Tavanı Veri Girişine Kapat', 4: 'Detayı Veri Girişine Kapat' };
        $('#dlgOnayBaslik').text(basliklar[kod]);
        $('#onayMesaj').text('');
        fazlaGrid.setGridOption('rowData', []);
        fazlaGrid.setGridOption('loading', true);
        dlgOnay.showModal();
        Sgb.al('api/mptavan/fazlagonderilen', { yil: o.yil, eko: o.eko, peb: o.peb, onayKod: kod })
            .done(function (v) {
                if (v.hata) { $('#onayMesaj').text(v.hata); return; }
                $('#onayMesaj').text(v.bilgi || '');
                fazlaGrid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { fazlaGrid.setGridOption('loading', false); });
    }

    $('#btnOnay, #btnOnayT').on('click', function () { onayDurumDegistir(parseInt($(this).attr('data-onay'), 10)); });
    $('#btnOnaylaSorgu').on('click', function () {
        var $d = $(this);
        onayGonder(onayIslemKod, $d).done(function (v) { if (!v.hata) dlgOnay.close(); });
    });

    // ---------------------------------------------------------------- açılır menüler (Yönetici İşlemleri, Hata Raporları)

    $('.mp-menu > .dropdown-toggle').on('click', function (e) {
        e.stopPropagation();
        var $m = $(this).parent();
        var acik = !$m.hasClass('open');
        $('.mp-menu').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false');
        $m.toggleClass('open', acik);
        $(this).attr('aria-expanded', acik ? 'true' : 'false');
    });
    $(document).on('click', function () { $('.mp-menu').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false'); });

    $('.mp-menu').on('click', 'a[data-onay]', function (e) {
        e.preventDefault();
        onayDurumDegistir(parseInt($(this).attr('data-onay'), 10));
    });

    // ---------------------------------------------------------------- raporlar

    function raporAl(p, $d) {
        var o = secimOku();
        if (o.yil <= 0) { mesaj('Yıl alanı boş boş bırakılamaz!', 'warning'); return; }
        if ($d) $d.prop('disabled', true);
        Sgb.indir('api/mptavan/rapor', $.extend(o, p)).then(function () { if ($d) $d.prop('disabled', false); });
    }

    $('.mp-menu').on('click', 'a[data-rapor]', function (e) {
        e.preventDefault();
        var $a = $(this);
        mesaj('Rapor hazırlanıyor...', 'info');
        raporAl({ rapor: $a.attr('data-rapor'), liste: $a.attr('data-liste') || '', detay: $a.attr('data-detay') || '' });
    });

    // Rapor tuşu (eski RaporAcKapa): rapor bölümünü açar / kapatır.
    $('#btnRaporAc').on('click', function () {
        var $p = $('#pnlRapor');
        var ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $kok.toggleClass('rapor-acik', ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false').toggleClass('active', ac);
    });

    // Soru işareti: açıklama metnini gösterir / gizler.
    $('#btnYardim').on('click', function () {
        var $p = $('#pnlIpucu');
        var ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false').toggleClass('active', ac);
    });

    // Yazdır (eski btnYazdir_Click): harcama birimi ve kırılımlarla tavan raporu.
    $('#btnYazdir').on('click', function () {
        var p = { rapor: 'tavan', hrc: kodlar.kod('hrc') };
        $('#frmRapor select[data-kirilim]').each(function () { p[$(this).attr('data-kirilim') + 'Uz'] = $(this).val(); });
        raporAl(p, $(this));
    });

    // H.Öd.Üret / H.Öd.(ÖE>=Say) (eski btnYazdirHO_Click, btnYazdirHOTemiz_Click).
    $('#btnYazdirHO, #btnYazdirHOTemiz').on('click', function () {
        raporAl({ rapor: 'ho', hrc: kodlar.kod('hrc'), cezali: $(this).attr('data-cezali'), oran: $('#nfSerbestOran').val() }, $(this));
    });

    // Üretim Ekranı (eski wndUret, urtUret_Click; yönetici).
    var dlgUret = document.getElementById('dlgUret');
    $('.mp-menu').on('click', 'a[data-islem="uret"]', function (e) {
        e.preventDefault();
        dlgUret.showModal();
    });
    $('#urtUret').on('click', function () {
        raporAl({ rapor: 'uret', kaynak: $('input[name="urtKaynak"]:checked').val() || '1', oran: $('#urtOran').val() }, $(this));
    });
    $('#frmUret').on('submit', function (e) { e.preventDefault(); $('#urtUret').trigger('click'); });

    // ---------------------------------------------------------------- Dosya Yükle, Merkez Yükle

    $('#btnDosyaYukle').on('click', function () {
        if (yilOku() <= 0) { mesaj('Yıl alanı boş boş bırakılamaz!', 'warning'); return; }
        $('#fuListe').val('').trigger('click');
    });
    $('#fuListe').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        if (!window.confirm('Dosyadaki tertiplerin KBÖ rakamları, verilen birimlere tavan olarak atanacak. Onaylıyor musunuz?')) return;
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('yil', yilOku());
        var $d = $('#btnDosyaYukle').prop('disabled', true);
        mesaj('Dosya yükleniyor, lütfen bekleyiniz...', 'info');
        $.ajax({
            url: Sgb.KOK + 'api/mptavan/dosyayukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (sonucGoster(v) && listelendi) listele(true);
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else hataGoster(xhr);
        }).always(function () { $d.prop('disabled', false); });
    });

    $('#btnMerkezAktar').on('click', function () {
        if (yilOku() <= 0) { mesaj('Yıl alanı boş boş bırakılamaz!', 'warning'); return; }
        if (!window.confirm('Merkez birimlerine ait ödenekler, merkez birimlerine tavan olarak atanacak. Onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.gonder('api/mptavan/merkezyukle', { yil: yilOku() })
            .done(function (v) { if (sonucGoster(v) && listelendi) listele(true); })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    });

    // Yıl değişince kod adları o yılın kodlarından okunur.
    $('#ddlYil').on('change', function () { kodlar.adlariYenile(); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    function kirilimDoldur($s, liste) {
        $s.empty();
        $.each(liste || [], function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($s); });
    }

    Sgb.al('api/mptavan/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            // Yıl artık listeden değil, sayı kutusundan (yukarı / aşağı ok) seçilir; sınırlar tanımlı yıllardan gelir.
            Sgb.yilKutusu('#ddlYil', t.yillar, t.yil);
            kodlar.ata('kur', t.kur, t.kurAd);
            kodlar.ata('fon', t.fon, t.fonAd);
            kodlar.ata('fin', t.fin, t.finAd);
            kodlar.ata('eko', t.eko, t.ekoAd);
            kodlar.ata('peb', t.peb, t.pebAd);
            var k = t.kirilimlar || {};
            $('#frmRapor select[data-kirilim]').each(function () { kirilimDoldur($(this), k[$(this).attr('data-kirilim')]); });
            $('.yonetici').prop('hidden', !t.yonetici);
        })
        .fail(hataGoster);

    // Tabloyu sayfa alanının (.sayfa-icerik) kalan yüksekliğine yayar; sayfada dikey kaydırma çıkmaz, satırlar tablonun içinde kayar.
    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var kap = el.closest('.sayfa-icerik') || document.body, cs = getComputedStyle(kap);
        var kaydirir = /(auto|scroll)/.test(cs.overflowY);
        var kr = kap.getBoundingClientRect(), kay = kaydirir ? kap.scrollTop : 0;
        var gorunen = kaydirir ? kap.clientHeight : window.innerHeight - kr.top;
        var ust = el.getBoundingClientRect().top - kr.top + kay;
        var son = 0;
        $(kap).children().each(function () {
            if (this.offsetParent === null && getComputedStyle(this).position !== 'fixed') return;
            son = Math.max(son, this.getBoundingClientRect().bottom - kr.top + kay + parseFloat(getComputedStyle(this).marginBottom || 0));
        });
        son += parseFloat(cs.paddingBottom || 0);
        var bosluk = Math.max(0, son - (ust + el.offsetHeight));
        el.style.height = Math.max(enAz, Math.floor(gorunen - ust - bosluk - 1)) + 'px';
    }
    function tabloYay() { yuksekligeYay(document.getElementById('grdMPTavanAna'), 200); }
    $(window).on('resize', tabloYay);
    // Rapor bölümü veya açıklama açılıp kapanınca tablo yeniden yayılır.
    if (window.ResizeObserver) new ResizeObserver(tabloYay).observe(document.querySelector('.mp-tavan-ana .mp-ust'));
    tabloYay();
});
