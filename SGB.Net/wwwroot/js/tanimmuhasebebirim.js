// Muhasebe Birim Bilgileri (ButceMuhasebe/TanimMuhasebeBirim): solda arama + liste (AG Grid), sağda seçili birimin bilgileri.
// Bir birimin anahtarı muhasebe + birim kodu + kurumsal koddur; kurumsal kod listede noktalı, kayıtta noktasız tutulur.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var M = JSON.parse($('#metinler').text() || '{}');
    var varsayilanKurum = '';

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(M.IslemHata, 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnYeni, #btnListele').prop('disabled', evet); }
    function noktasiz(kurum) { return String(kurum || '').replace(/\./g, ''); }
    function satirNo(muhasebe, kod, kurum) { return muhasebe + '|' + kod + '|' + noktasiz(kurum); }
    function durumAd(kapali) { return kapali ? M.Kapali : M.Aktif; }

    // ---------------------------------------------------------------- liste

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            // Liste sunucudan muhasebe, birim kodu, kurumsal kod sırasıyla gelir.
            { field: 'muhasebe', headerName: M.FRMMUB003, width: 96 },
            { field: 'muhasebeAd', headerName: M.MUHGNL191, width: 180, tooltipField: 'muhasebeAd' },
            { field: 'kod', headerName: M.MUHGNL077, width: 96 },
            { field: 'kurum', headerName: M.MUHGNL078, width: 120 },
            { field: 'ad', headerName: M.MUHGNL079, flex: 1, minWidth: 240, tooltipField: 'ad' },
            { field: 'vergiNo', headerName: M.VergiNo, width: 110 },
            { field: 'kapali', headerName: M.FRMOES010, width: 84, cellDataType: 'text', valueFormatter: function (p) { return durumAd(p.value); } }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return satirNo(p.data.muhasebe, p.data.kod, p.data.kurum); },
        onRowClicked: function (e) { doldur(e.data); }
    });

    // Açılışta liste boştur; kullanıcı aramadan kaydet/sil yapılırsa bütün birimler kendiliğinden listelenmez.
    var listelendi = false;

    function listele(secilecek) {
        listelendi = true;
        grid.setGridOption('loading', true);
        return Sgb.al('api/muhasebe/birim/liste', {
            muhasebe: $('#aramaMuhasebe').val(),
            kod: $('#aramaKod').val(),
            ad: $('#aramaAd').val(),
            kurum: $('#aramaKurum').val()
        })
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                grid.setGridOption('rowData', veri.satirlar);
                if (secilecek) {
                    var n = grid.getRowNode(secilecek);
                    if (n) { n.setSelected(true); grid.ensureNodeVisible(n); }
                }
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    $('#frmArama').on('submit', function (e) {
        e.preventDefault();
        listele();
    });

    // Listedeki satırlar (görünen sıra ve sütunlarla) Excel'in açtığı CSV dosyası olarak indirilir (eski "Yazdır").
    $('#btnDisaAktar').on('click', function () {
        if (!grid.getDisplayedRowCount()) { mesaj(M.DisaAktarBos, 'warning'); return; }
        Sgb.csvIndir(grid, M.DosyaAd, function (sutun, deger) { return sutun === 'kapali' ? durumAd(deger) : deger; });
    });

    // ---------------------------------------------------------------- form

    function temizle() {
        // Yeni birim, aranan muhasebeye ve kurumun varsayılan kurumsal koduna eklenir.
        $('#ddlMuhasebe').val($('#aramaMuhasebe').val());
        $('#txtKod, #txtAd, #txtVergiNo').val('');
        $('#txtKurum').val(varsayilanKurum);
        $('#btnSil').prop('disabled', true);
        grid.deselectAll();
    }

    function doldur(b) {
        $('#ddlMuhasebe').val(String(b.muhasebe));
        $('#txtKod').val(b.kod);
        $('#txtAd').val(b.ad || '');
        $('#txtKurum').val(b.kurum || '');
        $('#txtVergiNo').val(b.vergiNo || '');
        $('#btnSil').prop('disabled', false);
    }

    function formOku() {
        return {
            muhasebe: parseInt($('#ddlMuhasebe').val(), 10) || 0,
            kod: $.trim($('#txtKod').val()),
            ad: $.trim($('#txtAd').val()),
            kurum: $.trim($('#txtKurum').val()),
            vergiNo: $.trim($('#txtVergiNo').val())
        };
    }

    $('#btnYeni').on('click', function () {
        temizle();
        $($('#ddlMuhasebe').val() ? '#txtKod' : '#ddlMuhasebe').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        var istek = formOku();
        mesgul(true);
        Sgb.gonder('api/muhasebe/birim/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                $('#btnSil').prop('disabled', false);
                mesaj(s.bilgi, 'success');
                // Henüz arama yapılmadıysa yalnızca kaydedilen birimin muhasebesi listelenir.
                if (!listelendi) $('#aramaMuhasebe').val(String(istek.muhasebe));
                listele(satirNo(istek.muhasebe, istek.kod, istek.kurum));
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnSil').on('click', function () {
        var istek = formOku();
        if (!istek.muhasebe || !istek.kod || !istek.kurum) { mesaj(M.MUHGNL183, 'warning'); return; }
        if (!window.confirm(M.MUHGNL162)) return;
        mesgul(true);
        Sgb.gonder('api/muhasebe/birim/sil', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                temizle();
                mesaj(s.bilgi, 'success');
                if (listelendi) listele();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/muhasebe/birim/tanimlar')
        .done(function (t) {
            varsayilanKurum = t.varsayilanKurum || '';
            $.each(t.muhasebeler, function (i, m) {
                $('<option></option>').val(m.kod).text(m.ad).appendTo('#aramaMuhasebe');
                $('<option></option>').val(m.kod).text(m.ad).appendTo('#ddlMuhasebe');
            });
            // Eski sayfadaki gibi arama kurumun varsayılan kurumsal koduyla başlar.
            $('#aramaKurum').val(varsayilanKurum);
            // Açılışta liste boş gelir (birim sayısı çok olabilir); "Bul" ya da muhasebe seçimi listeler.
            temizle();
        })
        .fail(hataGoster);

    // Muhasebe seçilince o muhasebenin birimleri listelenir (eskiden "Bul" gerekiyordu).
    $('#aramaMuhasebe').on('change', function () {
        if (!$('#btnSil').prop('disabled')) return listele();
        $('#ddlMuhasebe').val($(this).val());
        listele();
    });
});
