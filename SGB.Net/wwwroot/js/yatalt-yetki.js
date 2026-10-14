// Proje yetkili kullanıcıları (ButceHazirlikv2/YATYetkiGirisForm): ProjeDetay / AltProjeDetay sekmesinde açılır. Liste,
// Kullanıcı Seç (kullanıcı arama penceresi, çoklu seçim; eski ListeKullaniciSec), Seçilileri Sil ve Kaydet (tablodaki
// kullanıcılar projenin yetkilileri olarak yazılır). Ölçüt sayfa adresinden gelir: yil, senaryoKod, projeKodu, altProjeKodu.
$(function () {
    'use strict';

    if (!$('#grdYetki').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var adres = new URLSearchParams(location.search);
    var olcut = {
        yil: parseInt(adres.get('yil'), 10) || 0,
        senaryoKod: adres.get('senaryoKod') || '',
        projeKodu: parseInt(adres.get('projeKodu'), 10) || 0,
        altProjeKodu: parseInt(adres.get('altProjeKodu'), 10) || 0
    };
    var kirli = false;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;

    function sor(metin, evet, is) {
        $('#soruMetin').text(metin);
        $('#btnSoruEvet').text(evet);
        soruIsi = is;
        dlgSoru.showModal();
    }

    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        if (soruIsi) soruIsi();
        soruIsi = null;
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- tablo

    var grid = Sgb.grid('#grdYetki', {
        columnDefs: [
            { field: 'kullaniciKodu', headerName: 'Kullanıcı Kodu', width: 150 },
            { field: 'ad', headerName: 'Adı', width: 150 },
            { field: 'soyad', headerName: 'Soyadı', width: 150 },
            { field: 'eposta', headerName: 'email', width: 220, tooltipField: 'eposta' },
            { field: 'telefon', headerName: 'Telefon', width: 140 }
        ],
        getRowId: function (p) { return p.data.kullaniciKodu; },
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true },
        overlayNoRowsTemplate: '<span>Yetkili kullanıcı yok. Kullanıcı Seç ile ekleyebilirsiniz.</span>'
    });

    function satirlar() {
        var liste = [];
        grid.forEachNode(function (n) { liste.push(n.data); });
        return liste;
    }

    // Liste (eski Oku).
    function oku() {
        grid.setGridOption('loading', true);
        Sgb.al('api/yatalt/yetki/liste', olcut)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                kirli = false;
                grid.setGridOption('rowData', v.satirlar);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    // Seçilileri Sil (eski deleteSelected); kayıt Kaydet ile yapılır.
    $('#btnSil').on('click', function () {
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Silinecek kullanıcıları işaretleyiniz.', 'info'); return; }
        grid.applyTransaction({ remove: secili });
        kirli = true;
    });

    // Kaydet (eski btnKaydet_Click; onay sorulur).
    $('#btnKaydet').on('click', function () {
        sor('Kaydetmek İstiyor musunuz?', 'Kaydet', function () {
            var $d = $('#btnKaydet').prop('disabled', true);
            var kullanicilar = $.map(satirlar(), function (s) {
                return { kullaniciKodu: s.kullaniciKodu, ad: s.ad || '', soyad: s.soyad || '', eposta: s.eposta || '', telefon: s.telefon || '' };
            });
            Sgb.gonder('api/yatalt/yetki/kaydet', $.extend({}, olcut, { kullanicilar: kullanicilar }))
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) oku();
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- Kullanıcı Seç (eski ListeKullaniciSec, çoklu seçim)

    var dlgKullaniciSec = document.getElementById('dlgKullaniciSec');

    function bosSatir($liste, metin) {
        $('<li class="bos"></li>').text(metin).appendTo($liste);
    }

    function listedeMi(kod) { return !!grid.getRowNode(kod); }

    $('#btnKullaniciSec').on('click', function () {
        $('#lstBulunan').empty();
        bosSatir($('#lstBulunan'), 'Ad, soyad ya da kullanıcı kodu yazıp Ara\'ya basınız.');
        dlgKullaniciSec.showModal();
        $('#txtAraAd').trigger('focus');
    });

    $('#frmKullaniciAra').on('submit', function (olay) {
        olay.preventDefault();
        var olc = { ad: $.trim($('#txtAraAd').val()), soyad: $.trim($('#txtAraSoyad').val()), kod: $.trim($('#txtAraKod').val()) };
        if (!olc.ad && !olc.soyad && !olc.kod) { bosSatir($('#lstBulunan').empty(), 'Ad, soyad ya da kullanıcı kodundan en az birini yazınız.'); return; }
        $('#btnAra').prop('disabled', true);
        Sgb.al('api/yatalt/yetki/kisi-ara', olc)
            .done(function (v) {
                var $liste = $('#lstBulunan').empty();
                if (v.hata) { bosSatir($liste, v.hata); return; }
                if (!v.satirlar.length) { bosSatir($liste, 'Kullanıcı bulunamadı.'); return; }
                if (v.fazla) bosSatir($liste, 'Çok kullanıcı bulundu; ilk ' + v.satirlar.length + ' kullanıcı gösteriliyor. Ölçütü daraltınız.');
                $.each(v.satirlar, function (i, k) {
                    var var_ = listedeMi(k.kullaniciKodu);
                    var $etiket = $('<label></label>').appendTo($('<li></li>').appendTo($liste));
                    $('<input type="checkbox" />').data('kullanici', k).prop('disabled', var_).appendTo($etiket);
                    $('<span class="ad"></span>').text($.trim((k.ad || '') + ' ' + (k.soyad || '')) + ' (' + k.kullaniciKodu + ')'
                        + (k.birim ? ' - ' + k.birim : '') + (var_ ? ' - listede' : '')).appendTo($etiket);
                    if (k.eposta) $('<span class="ek"></span>').text(k.eposta).appendTo($etiket);
                });
            })
            .fail(function (xhr) {
                if (xhr && xhr.status === 401) return;
                bosSatir($('#lstBulunan').empty(), xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Kullanıcılar okunamadı.');
            })
            .always(function () { $('#btnAra').prop('disabled', false); });
    });

    // Seçilenleri tabloya ekler (eski PersonelleriEkle); listede olanlar atlanır.
    $('#btnBulunanEkle').on('click', function () {
        var eklenecekler = $('#lstBulunan input:checked').map(function () { return $(this).data('kullanici'); }).get()
            .filter(function (k) { return !listedeMi(k.kullaniciKodu); })
            .map(function (k) { return { kullaniciKodu: k.kullaniciKodu, ad: k.ad, soyad: k.soyad, eposta: k.eposta, telefon: k.telefon }; });
        if (!eklenecekler.length) { mesaj('Eklenecek kullanıcıları işaretleyiniz.', 'info'); return; }
        grid.applyTransaction({ add: eklenecekler });
        kirli = true;
        dlgKullaniciSec.close();
        mesaj(eklenecekler.length + ' kullanıcı eklendi; Kaydet ile saklayınız.', 'info');
    });

    oku();
});
