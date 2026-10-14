// Kullanıcı Arama (KYM/KullaniciArama): solda sorgu alanları, sağda kullanıcı listesi (AG Grid).
// Son arama tarayıcı oturumunda saklanır; Kullanıcı Giriş sayfasından dönüldüğünde aynı liste yeniden gelir.
$(function () {
    'use strict';

    var M = JSON.parse($('#metinler').text() || '{}');
    var SAKLA = 'sgb.kullaniciArama';
    var ALANLAR = { ad: '#aramaAd', soyad: '#aramaSoyad', kod: '#aramaKod', tc: '#aramaTc', birim: '#aramaBirim', kurum: '#aramaKurum', tip: '#aramaTip', durum: '#aramaDurum' };

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(M.IslemHata, 'danger');
    }
    function durumAd(aktif) { return aktif ? M.Aktif : M.Pasif; }

    function baglanti(adres, metin) {
        var a = document.createElement('a');
        a.href = adres;
        a.textContent = metin;
        return a;
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            {
                field: 'adSoyad', headerName: M.FRMKUA015, width: 190,
                // Bilgilerini açabileceği kullanıcılar bağlantı olarak görünür.
                cellRenderer: function (p) {
                    if (!p.data.duzenlenebilir) return document.createTextNode(p.value || '');
                    return baglanti('KullaniciGiris?kullaniciKodu=' + encodeURIComponent(p.data.kod), p.value || p.data.kod);
                }
            },
            { field: 'kod', headerName: M.FRMKUG005, width: 130 },
            { field: 'unvan', headerName: M.FRMKUA016, width: 160, tooltipField: 'unvan' },
            { field: 'birim', headerName: M.FRMKUA017, width: 180, tooltipField: 'birim' },
            { field: 'telefon', headerName: M.FRMKUA018, width: 120 },
            {
                field: 'eposta', headerName: M.FRMKUA019, width: 200,
                cellRenderer: function (p) { return p.value ? baglanti('mailto:' + p.value, p.value) : document.createTextNode(''); }
            },
            { field: 'aktif', headerName: M.FRMKUA020, width: 84, cellDataType: 'text', valueFormatter: function (p) { return durumAd(p.value); } },
            { field: 'sonGiris', headerName: M.FRMKUA021, width: 130, valueFormatter: function (p) { return Sgb.tarih(p.value); } }
        ],
        getRowId: function (p) { return p.data.kod; }
    });

    function kosullar() {
        var k = {};
        $.each(ALANLAR, function (ad, kutu) { k[ad] = $.trim($(kutu).val() || ''); });
        return k;
    }

    function listele() {
        var k = kosullar();
        try { sessionStorage.setItem(SAKLA, JSON.stringify(k)); } catch (e) { /* saklanamazsa arama yine yapılır */ }
        grid.setGridOption('loading', true);
        $('#btnListele').prop('disabled', true);
        return Sgb.al('api/kullanici/liste', k)
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                grid.setGridOption('rowData', veri.satirlar);
                if (!veri.satirlar.length) mesaj(M.FRMKUA026, 'info');
            })
            .fail(hataGoster)
            .always(function () {
                grid.setGridOption('loading', false);
                $('#btnListele').prop('disabled', false);
            });
    }

    $('#frmArama').on('submit', function (e) {
        e.preventDefault();
        listele();
    });

    $('#btnDisaAktar').on('click', function () {
        if (!grid.getDisplayedRowCount()) { mesaj(M.DisaAktarBos, 'warning'); return; }
        Sgb.csvIndir(grid, M.DosyaAd, function (sutun, deger) {
            if (sutun === 'aktif') return durumAd(deger);
            if (sutun === 'sonGiris') return Sgb.tarih(deger);
            return deger;
        });
    });

    // Eski sayfadaki Grup Oluştur ve Gruba Kullanıcı Ata: ilgili sayfalar pencerede açılır.
    $('#btnGorevTanim').on('click', function () { Sgb.pencere('../Organizasyon/GenelGorevAna', 'Genel Görev Tanımlama'); });
    $('#btnGorevAta').on('click', function () { Sgb.pencere('../Organizasyon/GorevAta', 'Göreve Kişi Atama'); });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/kullanici/arama-tanimlar')
        .done(function (t) {
            $.each(t.kurumlar, function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo('#aramaKurum'); });
            $.each(t.tipler, function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo('#aramaTip'); });
            $('#aramaTipSatir').prop('hidden', !t.tipler.length);
            $('#btnYeni').prop('hidden', !t.yeniEklenebilir);

            var onceki = null;
            try { onceki = JSON.parse(sessionStorage.getItem(SAKLA) || 'null'); } catch (e) { onceki = null; }
            if (onceki) {
                $.each(ALANLAR, function (ad, kutu) { $(kutu).val(onceki[ad] || ''); });
                listele();
            } else {
                // Açılışta liste boştur; yönetici olmayanın araması kendi kurumuyla başlar.
                $('#aramaKurum').val(t.varsayilanKurum || '');
            }
        })
        .fail(hataGoster);
});
