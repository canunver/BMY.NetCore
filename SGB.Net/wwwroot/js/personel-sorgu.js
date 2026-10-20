// Personel arama penceresi (InsanKaynaklari/PersonelSorgu; eski PersonelSorgu.aspx). Kart sekmesi Sgb.pencere ile açar (?tip=1|2).
// Arama metni, birim (ağaçtan) ve unvana göre Listele (eski PersonelListele); tıklanan personel açan sayfaya
// { tur: 'personelSecildi', tip, kod, ad } iletisiyle bildirilir ve pencere kapanır (eski SatirSecildi: parent.BilgiListeleAc + hidePopWin).
$(function () {
    'use strict';

    var $sayfa = $('#psrSayfa');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var tip = parseInt($sayfa.data('tip'), 10) || 0;
    var birimKod = 0;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }

    function sec(p) {
        var ileti = { tur: 'personelSecildi', tip: tip, kod: p.KOD, ad: p.ADSOYAD };
        var hedef = window.parent !== window ? window.parent : window.opener;
        if (!hedef) return;
        hedef.postMessage(ileti, window.location.origin);
        // Çerçeve (Sgb.pencere) içindeyse pencereyi kapat, ayrı pencereyse kendini kapat.
        try {
            var d = window.frameElement && window.frameElement.closest('dialog');
            if (d) { d.close(); return; }
        } catch (e) { /* başka kökenden açılmış */ }
        if (window.opener) window.close();
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'TCKIMLIKNO', headerName: 'TC Kimlik No', width: 115, cellClass: 'hucre-salt' },
            { field: 'SICILNO', headerName: 'Sicil No', width: 90, cellClass: 'hucre-salt' },
            { field: 'ADSOYAD', headerName: 'Adı Soyadı', flex: 1, minWidth: 160, cellClass: 'hucre-salt', tooltipField: 'ADSOYAD' },
            { field: 'BIRIM', headerName: 'Birim', width: 170, cellClass: 'hucre-salt', tooltipField: 'BIRIM' },
            { field: 'PERSONELTUR', headerName: 'Personel Tür', width: 120, cellClass: 'hucre-salt' },
            { field: 'UNVAN', headerName: 'Unvan', width: 150, cellClass: 'hucre-salt', tooltipField: 'UNVAN' },
            { field: 'KADRODERECE', headerName: 'Kadro Derecesi', width: 110, cellClass: 'hucre-salt', type: 'rightAligned' },
            { field: 'KADROTARIHI', headerName: 'Kadro Tarihi', width: 105, cellClass: 'hucre-salt', hide: true,
                valueFormatter: function (p) { return Sgb.tarih(p.value); } },
            { field: 'KADRODURUMU', headerName: 'Kadro Durumu', width: 110, cellClass: 'hucre-salt', hide: true }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.KOD); },
        onRowClicked: function (e) { sec(e.data); },
        pagination: true,
        paginationPageSize: 250,
        paginationPageSizeSelector: false,
        overlayNoRowsTemplate: '<span class="text-muted">Arama ölçütlerini girip Listele\'ye basınız.</span>'
    });

    // Eski Genel.UnvanDoldur
    Sgb.al('api/personelbilgi/personelsorgu/unvanlar')
        .done(function (veri) {
            var $c = $('#cmbUnvan').empty();
            $.each(veri.unvanlar || [], function (i, u) { $('<option></option>').val(String(u.UnvanKod)).text(u.UnvanAd).appendTo($c); });
            if (veri.hata) mesaj(veri.hata, 'danger');
        })
        .fail(hataGoster);

    // Eski trpBirim (OrgBirimDoldur): düzey düzey okunan birim ağacı
    Sgb.agac(document.getElementById('agcBirim'), {
        yukle: function (ust) {
            return Sgb.al('api/personelbilgi/personelsorgu/birimler', { ust: ust ? ust.kod : 0 }).then(function (veri) {
                if (veri.hata) mesaj(veri.hata, 'danger');
                return veri.satirlar || [];
            });
        },
        secilince: function (d) {
            birimKod = d.kod;
            $('#lblBirim').text(d.ad).removeClass('text-muted');
        },
        seciliMi: function (d) { return d.kod === birimKod; },
        bosMetin: 'Birim yok',
        hata: hataGoster
    });

    $('#btnBirimTemizle').on('click', function () {
        birimKod = 0;
        $('#lblBirim').text('Seçilmedi').addClass('text-muted');
        $('#agcBirim .sec').removeAttr('aria-current');
    });

    // Eski PersonelListele
    $('#frmSorgu').on('submit', function (e) {
        e.preventDefault();
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/personelbilgi/personelsorgu/liste', {
            genelSorgu: $.trim($('#txtGenelSorgu').val()),
            birim: birimKod || 0,
            unvan: parseInt($('#cmbUnvan').val(), 10) || 0
        })
            .done(function (veri) {
                grid.setGridOption('rowData', veri.satirlar || []);
                if (veri.hata) mesaj(veri.hata, 'danger');
                else if (veri.bilgi) mesaj(veri.bilgi, 'info');
            })
            .fail(hataGoster)
            .always(function () {
                grid.setGridOption('loading', false);
                $d.prop('disabled', false);
            });
    });

    $('#txtGenelSorgu').trigger('focus');
});
