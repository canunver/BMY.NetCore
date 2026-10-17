// Ödenek Gönderme / Tenkis İcmali - belge düzenleme (ButceMuhasebe/OdenekGondermeIcmalForm; eski aynı adlı Ext.NET sayfası).
// Yıl + belge türü + icmal no ile icmal okunur; kurumsal kod, kayıt tarihi ve icmale ödeme emri ekleyebilecek kullanıcılar
// düzenlenip kaydedilir; Yazdır icmal çıktısını (Excel) indirir. Belge Arama sekmesinden açılan icmal OgBelgeYukle ile okunur.
// Uçlar: /api/odenekgondermeicmal/* (Muhasebe/OdenekGondermeIcmalUclari.cs).
$(function () {
    'use strict';

    var $kok = $('.og-icmal-form');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/odenekgondermeicmal/';
    var tanim = {};
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
        $('#btnSoruEvet').text(evet || 'Evet');
        soruIsi = is;
        dlgSoru.showModal();
    }
    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        var is = soruIsi;
        soruIsi = null;
        if (is) is();
    });
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- alanlar

    function anahtar() {
        return {
            yil: parseInt($('#ddlYil').val(), 10) || 0,
            belgeTur: parseInt($('#ddlBelgeTur').val(), 10) || 0,
            icmalNo: parseInt($('#txtIcmalNo').val(), 10) || 0
        };
    }

    function kurKodlariDoldur(liste, secili) {
        var $k = $('#ddlKurKod').empty();
        $('<option value=""></option>').appendTo($k);
        $.each(liste || [], function (i, k) { $('<option></option>').val(k.kod).text(k.kodAd).appendTo($k); });
        kurKodAta(secili);
    }

    // Okunan kod listede yoksa (ör. sonradan kapanmış birim) yine de gösterilir.
    function kurKodAta(kod) {
        kod = kod || '';
        var $k = $('#ddlKurKod');
        if (kod && !$k.find('option').filter(function () { return this.value === kod; }).length)
            $('<option></option>').val(kod).text(kod).appendTo($k);
        $k.val(kod);
    }

    $('#ddlYil').on('change', function () {
        var y = parseInt($(this).val(), 10) || 0;
        if (y <= 0) return;
        Sgb.al(UC + 'kurkodlar', { yil: y })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                kurKodlariDoldur(v.kurKodlar, $('#ddlKurKod').val());
            })
            .fail(hataGoster);
    });

    $('#frmIcmal').on('change input', 'input, select', function () { kirli = true; });

    // ---------------------------------------------------------------- kullanıcılar

    var grid = Sgb.grid('#grdKullanici', {
        columnDefs: [
            { field: 'kulKod', headerName: 'Kullanıcı Kodu', minWidth: 140, flex: 1 },
            { field: 'adSoyad', headerName: 'Ad Soyad', minWidth: 180, flex: 2 }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true },
        getRowId: function (x) { return x.data.kulKod; },
        overlayNoRowsTemplate: '<span>İcmali okuyunuz ya da Ekle ile kullanıcı ekleyiniz.</span>'
    });

    function kullanicilar() {
        var l = [];
        grid.forEachNode(function (n) { l.push(n.data.kulKod); });
        return l;
    }

    $('#btnKulSil').on('click', function () {
        var s = grid.getSelectedRows();
        if (!s.length) { mesaj('Silinecek kullanıcıları işaretleyiniz.', 'info'); return; }
        grid.applyTransaction({ remove: s });
        kirli = true;
    });

    // Kullanıcı Seç (eski ListeKullaniciSec)
    var dlgKullaniciSec = document.getElementById('dlgKullaniciSec');
    function bosSatir($liste, metin) { $('<li class="bos"></li>').text(metin).appendTo($liste); }
    function listedeMi(kod) { return !!grid.getRowNode(kod); }

    $('#btnKulEkle').on('click', function () {
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
        Sgb.al(UC + 'kisi-ara', olc)
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
                });
            })
            .fail(function (xhr) {
                if (xhr && xhr.status === 401) return;
                bosSatir($('#lstBulunan').empty(), xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Kullanıcılar okunamadı.');
            })
            .always(function () { $('#btnAra').prop('disabled', false); });
    });

    $('#btnBulunanEkle').on('click', function () {
        var eklenecekler = $('#lstBulunan input:checked').map(function () { return $(this).data('kullanici'); }).get()
            .filter(function (k) { return !listedeMi(k.kullaniciKodu); })
            .map(function (k) { return { kulKod: k.kullaniciKodu, adSoyad: $.trim((k.ad || '') + ' ' + (k.soyad || '')) }; });
        if (!eklenecekler.length) { mesaj('Eklenecek kullanıcıları işaretleyiniz.', 'info'); return; }
        grid.applyTransaction({ add: eklenecekler });
        kirli = true;
        dlgKullaniciSec.close();
        mesaj(eklenecekler.length + ' kullanıcı eklendi; Kaydet ile saklayınız.', 'info');
    });

    // ---------------------------------------------------------------- Oku / Kaydet / Yazdır / Temizle

    function ekranaYaz(v) {
        $('#txtKayitTarih').val(v.kayitTarih || '');
        kurKodAta(v.kurKod);
        $('#lblFormDurum').text(v.durum || '');
        grid.setGridOption('rowData', v.kullanicilar || []);
        kirli = false;
    }

    function oku() {
        var a = anahtar();
        if (!(a.yil > 0) || !(a.icmalNo > 0) || !(a.belgeTur > 0)) { mesaj('Anahtar alanları girmelisiniz.', 'warning'); return; }
        var $d = $('#btnOku').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'oku', a)
            .done(function (v) {
                if (v.hata) {
                    mesaj(v.hata, 'info');
                    $('#lblFormDurum').text('');
                    grid.setGridOption('rowData', []);
                    return;
                }
                ekranaYaz(v);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmIcmal').on('submit', function (e) { e.preventDefault(); oku(); });

    $('#btnKaydet').on('click', function () {
        var a = anahtar();
        if (!(a.yil > 0) || !(a.icmalNo > 0) || !(a.belgeTur > 0)) { mesaj('Anahtar alanları girmelisiniz.', 'warning'); return; }
        sor('Kaydetmek İstiyor musunuz?', 'Kaydet', function () {
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder(UC + 'kaydet', {
                yil: a.yil, belgeTur: a.belgeTur, icmalNo: a.icmalNo,
                kurKod: $('#ddlKurKod').val() || '', kayitTarih: $('#txtKayitTarih').val() || '',
                kullanicilar: kullanicilar()
            })
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata || 'Kayıt işlemi başarısız.', 'danger'); return; }
                    kirli = false;
                    mesaj(v.bilgi || 'Kayıt işlemi başarıyla gerçekleşti.', 'success');
                    oku();
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    $('#btnYazdir').on('click', function () {
        var a = anahtar();
        if (!(a.icmalNo > 0)) { mesaj('Yazdırılacak İcmal numarası belirtilmemiş.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir(UC + 'yazdir', a).then(function () { $d.prop('disabled', false); });
    });

    function temizle() {
        $('#txtIcmalNo, #txtKayitTarih').val('');
        $('#ddlKurKod').val('');
        $('#lblFormDurum').text('');
        grid.setGridOption('rowData', []);
        kirli = false;
    }
    $('#btnTemizle').on('click', temizle);

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // Belge Arama sekmesinden (çerçeve üzerinden) çağrılır (eski KayitAc).
    window.OgBelgeYukle = function (yil, icmalNo, belgeTur) {
        function yukle() {
            if ($('#ddlYil').val() !== String(yil)) $('#ddlYil').val(String(yil)).trigger('change');
            $('#ddlBelgeTur').val(String(belgeTur));
            $('#txtIcmalNo').val(icmalNo);
            oku();
        }
        if (tanim.yil) yukle(); else hazir.done(yukle);
    };

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    var hazir = Sgb.al(UC + 'tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            var $y = Sgb.yilKutusu('#ddlYil', t.yillar);
            $y.val(String(t.yil));
            var $b = $('#ddlBelgeTur').empty();
            $.each(t.belgeTurleri || [], function (i, b) { $('<option></option>').val(b.kod).text(b.ad).appendTo($b); });
            kurKodlariDoldur(t.kurKodlar, '');
        })
        .fail(hataGoster);
});
