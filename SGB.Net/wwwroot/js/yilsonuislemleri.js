// Yıl Sonu Kapatma İşlemleri, ButceMuhasebe/YilSonuIslemleri (eski YilSonuIslemleri.aspx): yıl, muhasebe / birim ve belge tarihine göre
// yıl sonu kapanış belgelerinin listesi. Satırdaki tuşlar: Belge Oluştur (eski KomutCalistir 'BelgeOlustur', islem "1": belge kaydı +
// yevmiye no) ve Belge Raporu (eski 'BelgeAl', islem "2": oluşacak belgenin satırları Excel'e). Belge No hücresi yazılabilir (eski Editor).
// Uçlar: /api/yilsonu/* (Muhasebe/YilSonuIslemleriUclari.cs). Birim seçimi Sgb.muhasebeBirim.
$(function () {
    'use strict';

    if (!$('.yil-sonu').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/yilsonu/';

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function yil() { return parseInt($('#txtYil').val(), 10) || 0; }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', ad: 'lblBirimAd' },
        yil: yil,
        kurumsalGizle: true
    });

    var mesgul = false; // bir belge işlemi sürerken ikinci tuşa basılmasın

    function komutlar(p) {
        var $k = $('<span></span>');
        $('<button type="button" class="btn btn-default btn-xs ys-komut" data-komut="olustur" title="Belge Oluştur">'
            + '<span class="glyphicon glyphicon-plus" aria-hidden="true"></span> Oluştur</button>').appendTo($k);
        $('<button type="button" class="btn btn-default btn-xs ys-komut" data-komut="rapor" title="Belge Raporu Al">'
            + '<span class="glyphicon glyphicon-list-alt" aria-hidden="true"></span> Rapor</button>').appendTo($k);
        $k.find('button').prop('disabled', mesgul);
        $k.on('click', 'button', function () { komut($(this).data('komut'), p.data, $(this)); });
        return $k[0];
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { headerName: '#', width: 56, sortable: false, cellClass: 'ys-salt', valueGetter: function (p) { return p.node.rowIndex + 1; } },
            { field: 'YIL', headerName: 'Yıl', width: 80, cellClass: 'ys-salt' },
            { field: 'AD', headerName: 'Açıklama', minWidth: 260, flex: 3, cellClass: 'ys-salt' },
            { field: 'BELGENO', headerName: 'Belge No', width: 130, editable: true, cellDataType: 'text', cellClass: 'ys-yazilir' },
            { field: 'TUR', headerName: 'Tür', hide: true },
            { headerName: 'İşlem', width: 170, sortable: false, cellRenderer: komutlar }
        ],
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        overlayNoRowsTemplate: '<span>Kıstasları girip Listele\'ye basınız.</span>'
    });

    function listele() {
        var mb = muhBirim.deger();
        var k = { yil: yil() || '', muhasebe: mb.muhasebe || '', birim: mb.birim };
        if (!k.yil) delete k.yil;
        if (!k.muhasebe) delete k.muhasebe;
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', k)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata || 'Liste okunamadı.', 'danger'); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
                $('#lblOzet').text(v.satirlar && v.satirlar.length ? v.satirlar.length + ' işlem.' : '');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); $d.prop('disabled', false); yay(); });
    }

    function istek(satir) {
        var mb = muhBirim.deger();
        return {
            Yil: yil(),
            Muhasebe: mb.muhasebe || 0,
            Birim: mb.birim,
            BelgeNo: $.trim(satir.BELGENO || ''),
            Tur: parseInt(satir.TUR, 10) || 0,
            BelgeTarih: $('#txtBelgeTarih').val() || ''
        };
    }

    function mesgulYap(evet) {
        mesgul = evet;
        $('#grdListe .ys-komut').prop('disabled', evet);
        $('#btnListele').prop('disabled', evet);
    }

    function komut(tur, satir, $tus) {
        if (mesgul) return;
        grid.stopEditing();
        var i = istek(satir);
        if (!i.Yil) { mesaj('Yıl bilgisi boş bırakılmış.', 'warning'); return; }

        if (tur === 'olustur') {
            if (!window.confirm('Belge oluşturma işlemi başlatılacak. Onaylıyor musunuz?')) return;
            mesgulYap(true);
            $tus.addClass('avans-bekle');
            mesaj('Belge oluşturuluyor, lütfen bekleyiniz...', 'info');
            Sgb.gonder(UC + 'olustur', i)
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata || 'Belge oluşturulamadı.', 'danger'); return; }
                    mesaj(v.bilgi || ((v.belgeNo ? v.belgeNo + ' numaralı ' : '') + 'belge oluşturuldu.'), 'success');
                    if (v.yevmiyeHata) mesaj('Yevmiye numarası verilemedi: ' + v.yevmiyeHata, 'warning');
                    listele();
                })
                .fail(hataGoster)
                .always(function () { mesgulYap(false); $tus.removeClass('avans-bekle'); });
        } else {
            if (!window.confirm('Üretilecek belgenin satırları raporlanacak. Tamam\'a bastıktan sonra, rapor üretilirken lütfen bekleyiniz...')) return;
            mesgulYap(true);
            $tus.addClass('avans-bekle');
            var p = { yil: i.Yil, belgeNo: i.BelgeNo, tur: i.Tur, birim: i.Birim, belgeTarih: i.BelgeTarih };
            if (i.Muhasebe) p.muhasebe = i.Muhasebe;
            Sgb.indir(UC + 'rapor', p).then(function () { mesgulYap(false); $tus.removeClass('avans-bekle'); },
                function () { mesgulYap(false); $tus.removeClass('avans-bekle'); });
        }
    }

    $('#btnListele').on('click', listele);
    $('#frmKriter').on('keydown', 'input', function (e) {
        if (e.key === 'Enter' && !$(this).closest('#muhBirim').length) { e.preventDefault(); listele(); }
    });

    Sgb.al(UC + 'tanim')
        .done(function (t) { Sgb.yilKutusu('#txtYil', t.yillar, t.yil); })
        .fail(hataGoster);

    // Tablo sayfanın kalan yüksekliğini doldurur.
    function yay() {
        var el = document.getElementById('grdListe');
        if (!el || el.offsetParent === null) return;
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(260, Math.floor(window.innerHeight - ust - 24)) + 'px';
    }
    $(window).on('resize', yay);
    yay();
});
