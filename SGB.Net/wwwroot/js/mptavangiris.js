// Tavan Girişi (ButceHazirlikv2/MPTavanGiris; eski MPTavanGiris.aspx): tertibin birimlere dağıtılan tavanları AG Grid'de girilir.
// Parametreler sayfadaki .mp-giris öğesinin data- özniteliklerindedir (yil, senaryo, tertip, dg, kykod, saybir).
// DG=G il tavanı (detay tavan tutarına tıklanınca o birimin harcama birimi tavanları açılır), DG=D merkez detay (kurumsal yapı seçilir,
// Excel'den yükleme yapılır), DG=I il girişi. Uçlar: /api/mptavan/giris/* (ButceHazirlik/MPTavanUclari.cs).
$(function () {
    'use strict';

    var $kok = $('.mp-giris');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var p = {
        yil: parseInt($kok.attr('data-yil'), 10) || 0,
        senaryo: $kok.attr('data-senaryo') || '',
        tertip: $kok.attr('data-tertip') || '',
        dg: ($kok.attr('data-dg') || '').toUpperCase(),
        kykod: $kok.attr('data-kykod') || '',
        saybir: $kok.attr('data-saybir') || ''
    };
    var detay = p.dg === 'D' || p.dg === 'I';
    var merkezDetay = p.dg === 'D';
    var toplam = 0;
    var kirli = false;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    var kodlar = merkezDetay ? MPTavan.kodAlanlari(function () { return p.yil; }, hataGoster) : null;
    if (kodlar && p.kykod) kodlar.ata('ky', p.kykod);
    function kyKod() { return kodlar ? kodlar.kod('ky') : p.kykod; }

    // ---------------------------------------------------------------- tablo

    function sayi(v) { v = Number(v); return isFinite(v) ? v : 0; }
    function tutarYaz(prm) { return MPTavan.tutar(prm.value); }

    // "1.234.567,89" ya da "1234567.89" -> sayı
    function tutarOku(prm) {
        var s = $.trim(String(prm.newValue === null || prm.newValue === undefined ? '' : prm.newValue));
        if (s === '') return 0;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var v = Number(s);
        return isFinite(v) ? v : prm.oldValue;
    }

    // Gridlerde sarı salt okunur, beyaz veri girilen hücredir; toplam satırı kendi rengini korur.
    function saltOkunur(ek) {
        return function (prm) { return prm.node.rowPinned ? ek : ek.concat('salt'); };
    }

    function sayiSutun(ozellik) {
        return $.extend({ type: 'rightAligned', width: 140, cellClass: saltOkunur(['sayi']), valueFormatter: tutarYaz }, ozellik);
    }

    // Eski DetayRenderer: il tavanında birimin harcama birimlerine dağıtılan toplam; tıklanınca o birimin detay girişi açılır.
    function detayHucre(prm) {
        if (prm.node.rowPinned) return MPTavan.tutar(prm.value);
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-link mp-bag';
        b.textContent = MPTavan.tutar(prm.value);
        b.title = 'Harcama birimi tavanlarını aç';
        b.addEventListener('click', function () { detayAc(prm.data); });
        return b;
    }

    var sutunlar = [
        { field: 'birimKodAd', headerName: detay ? 'Harcama Birimi' : 'Birim', flex: 1, minWidth: 260, pinned: 'left', tooltipField: 'birimKodAd', cellClass: saltOkunur([]) },
        sayiSutun({ field: 'harcama', headerName: 'Harcama' }),
        sayiSutun({ field: 'birimTalep', headerName: 'Birim Talep' }),
        sayiSutun({ field: 'gonderilen', headerName: 'Gönderilen' }),
        sayiSutun({
            field: 'tavan', headerName: detay ? 'Tavan Giriniz!' : 'İl Tavanı Giriniz!', width: 160,
            headerTooltip: 'Hücreye tıklayıp tavanı yazınız.',
            editable: function (prm) { return !prm.node.rowPinned; },
            valueParser: tutarOku,
            cellClass: ['sayi']
        })
    ];
    if (!detay) sutunlar.push(sayiSutun({ field: 'detayTavan', headerName: 'Detay Tavan', cellRenderer: detayHucre }));

    var grid = Sgb.grid('#grdTavan', {
        columnDefs: sutunlar,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        overlayNoRowsTemplate: detay && !p.kykod
            ? '<span>Kurumsal yapıyı seçip Listele\'ye basınız.</span>'
            : '<span>Listele\'ye basınız.</span>',
        onCellValueChanged: function () { kirli = true; toplamYenile(); }
    });

    function satirlar() {
        var r = [];
        grid.forEachNode(function (n) { if (n.data) r.push(n.data); });
        return r;
    }

    // Eski sütun başlığındaki toplamlar ve Dağıtılmayan (üstteki toplam - girilen tavanlar).
    function toplamYenile() {
        var r = satirlar();
        var t = { birimKodAd: 'T O P L A M', harcama: 0, birimTalep: 0, gonderilen: 0, tavan: 0, detayTavan: 0 };
        $.each(r, function (i, d) {
            $.each(['harcama', 'birimTalep', 'gonderilen', 'tavan', 'detayTavan'], function (j, a) { t[a] += sayi(d[a]); });
        });
        grid.setGridOption('pinnedBottomRowData', r.length ? [t] : []);
        $('#lblToplam').text(MPTavan.tutar(toplam));
        var dag = toplam - t.tavan;
        $('#lblDagitilmayan').text(MPTavan.tutar(dag)).toggleClass('eksi', dag < 0);
    }

    // ---------------------------------------------------------------- Listele (eski OdenekListele)

    function listele() {
        if (merkezDetay && !kyKod()) { mesaj('Kurumsal yapı seçiniz.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/mptavan/giris/liste', { yil: p.yil, senaryo: p.senaryo, tertip: p.tertip, dg: p.dg, kykod: kyKod(), saybir: p.saybir })
            .done(function (v) {
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                if (v.hata) {
                    toplam = 0;
                    grid.setGridOption('rowData', []);
                    toplamYenile();
                    $('#btnKaydet').prop('disabled', true);
                    mesaj(v.hata, 'danger');
                    return;
                }
                $('#lblTertipAdi').text(v.tertipAdi || '');
                toplam = sayi(v.toplam);
                kirli = false;
                grid.setGridOption('rowData', v.satirlar || []);
                toplamYenile();
                $('#btnKaydet').prop('disabled', !(v.satirlar && v.satirlar.length));
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }
    $('#btnListele').on('click', function () {
        if (kirli && !window.confirm('Kaydedilmemiş değişiklikler var. Yeniden listelensin mi?')) return;
        listele();
    });
    $kok.on('mpkod', '.kod-alan input', function () { listele(); });

    // ---------------------------------------------------------------- Kaydet (eski OdenekKaydet)

    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        var r = satirlar();
        if (!r.length) return;
        if (!window.confirm('Ekrandaki veriler kaydedilecektir, onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/mptavan/giris/kaydet', {
            Yil: p.yil, Senaryo: p.senaryo, Tertip: p.tertip, DG: p.dg, KyKod: kyKod(), SayBir: p.saybir,
            Satirlar: $.map(r, function (d) { return { birimKod: d.birimKod, tavan: sayi(d.tavan) }; })
        })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata || 'Kayıt işlemi başarısız.', 'danger'); return; }
                mesaj(v.bilgi, 'success');
                listele();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Detay (eski DetayRenderer -> MPTavanGiris.aspx?DG=D)

    function detayAc(d) {
        var adres = 'MPTavanGiris?' + $.param({ yil: p.yil, senaryo: p.senaryo, tertip: p.tertip, DG: 'D', kykod: d.birimKod, saybir: p.saybir });
        Sgb.pencere(adres, 'Harcama Birimi Tavan Girişi', function () { listele(); });
    }

    // ---------------------------------------------------------------- Dosya Yükle (eski btnDosyaYukle_Click; merkez detay)

    $('#btnDosyaYukle').on('click', function () { $('#fuTavan').val('').trigger('click'); });
    $('#fuTavan').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        if (!window.confirm('Dosyadaki tavanlar kaydedilecektir, onaylıyor musunuz?')) return;
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('yil', p.yil);
        veri.append('senaryo', p.senaryo);
        veri.append('tertip', p.tertip);
        veri.append('dg', p.dg);
        veri.append('kykod', kyKod());
        var $d = $('#btnDosyaYukle').prop('disabled', true);
        mesaj('Dosya yükleniyor, lütfen bekleyiniz...', 'info');
        $.ajax({
            url: Sgb.KOK + 'api/mptavan/giris/yukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (v.hata) {
                $('#dlgSonucBaslik').text('Yükleme');
                $('#sonucMetin').text(v.hata);
                document.getElementById('dlgSonuc').showModal();
                return;
            }
            mesaj(v.bilgi, 'success');
            if (kyKod()) listele();
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else hataGoster(xhr);
        }).always(function () { $d.prop('disabled', false); });
    });

    // Kod alanı yoksa pencere kapatma tuşları burada bağlanır (varsa MPTavan.kodAlanlari bağlar).
    if (!kodlar) $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // Açılış (eski Page_Load): detayda kurumsal yapı boşsa seçim beklenir, değilse hemen listelenir.
    toplamYenile();
    if (!(merkezDetay && !p.kykod)) listele();
});
