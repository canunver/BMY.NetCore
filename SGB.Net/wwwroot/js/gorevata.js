// Göreve Kişi Atama (Organizasyon/GorevAta): birim ağacından birim, o birimin görevlerinden görev seçilir;
// seçili göreve kişi eklenir, görevden silinir, kişinin o görevdeki kısıtı (faaliyet kodu) yazılır.
// ?kullanici=kod ile açıldığında o kullanıcının şimdiki görevleri üstte gösterilir ve kullanıcı tek tuşla eklenir.
$(function () {
    'use strict';

    if (!$('#agacBirim').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var secim = { orgKod: 0, birimAd: '', gorev: '', gorevAd: '' };
    var kisi = null;      // Kullanıcı Giriş'ten gelinen kullanıcı: { tc, ad, soyad, kod, gorevler }
    var kisiler = [];     // seçili göreve atanmış kişiler
    var kisitKisi = null; // kısıtı düzenlenen kişi

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function adSoyad(k) { return $.trim((k.ad || '') + ' ' + (k.soyad || '')); }
    function bosSatir($liste, metin) { $('<li></li>').append($('<span class="bos"></span>').text(metin)).appendTo($liste); }

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    function dugmeler() {
        var gorevVar = !!secim.gorev;
        $('#btnKisiEkle').prop('disabled', !gorevVar);
        $('#btnKisiyiEkle').prop('disabled', !gorevVar || !kisi || kisiler.some(function (k) { return k.tc === kisi.tc; }));
        $('#btnKisiSil').prop('disabled', !$('#lstKisiler input:checked').length);
    }

    // ---------------------------------------------------------------- birim ağacı

    // Sayfa belli bir birim için açıldıysa (Organizasyon Şeması'ndaki Görevler sekmesi: ?orgKod=..&orgAd=..) ağaç çizilmez.
    var adres = new URLSearchParams(window.location.search);
    var sabitBirim = parseInt(adres.get('orgKod') || '0', 10) || 0;
    $('.gorevata').toggleClass('birimsiz', sabitBirim > 0);

    // Ağaç ortak bileşenle çizilir (Sgb.agac); kökteki ilk dal açık gelir.
    if (!sabitBirim) Sgb.agac('#agacBirim', {
        yukle: function (ust) {
            return Sgb.al('api/gorevata/birimler', { ust: ust ? ust.kod : 0 }).then(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return []; }
                return veri.satirlar;
            });
        },
        secilince: birimSec,
        ilkAcik: true,
        bosMetin: 'Birim bulunamadı.',
        hata: hataGoster
    });

    // ---------------------------------------------------------------- görevler

    function birimSec(b) {
        secim = { orgKod: b.kod, birimAd: b.ad, gorev: '', gorevAd: '' };
        $('#secilenBirim').text(b.ad);
        kisileriCiz([]);
        $('#secilenGorev').text('Bir görev seçiniz.');
        gorevListe.yenile();
    }

    var gorevListe = Sgb.agac('#lstGorevler', {
        duz: true,
        yukle: function () {
            if (!secim.orgKod) return $.Deferred().resolve([]).promise();
            var orgKod = secim.orgKod;
            return Sgb.al('api/gorevata/gorevler', { orgKod: orgKod }).then(function (veri) {
                if (!veri.satirlar.length && secim.orgKod === orgKod) $('#secilenBirim').text(secim.birimAd + ' (bu birimde verilebilecek görev yok)');
                return veri.satirlar;
            });
        },
        secilince: gorevSec,
        hata: hataGoster
    });

    // ---------------------------------------------------------------- görevlendirilenler

    function kisileriCiz(satirlar) {
        kisiler = satirlar || [];
        var $liste = $('#lstKisiler').empty();
        if (!kisiler.length) {
            if (secim.gorev) bosSatir($liste, 'Bu göreve atanmış kişi yok.');
            dugmeler();
            return;
        }
        $.each(kisiler, function (i, k) {
            var $li = $('<li></li>').appendTo($liste);
            var $etiket = $('<label></label>').appendTo($li);
            $('<input type="checkbox" />').data('kisi', k).appendTo($etiket);
            $('<span class="ad"></span>').text(adSoyad(k)).appendTo($etiket);
            $('<span class="tc"></span>').text(k.tc).appendTo($etiket);
            $('<button type="button" class="btn btn-default btn-xs">Kısıtla</button>').attr('aria-label', 'Kısıtla: ' + adSoyad(k))
                .on('click', function () { kisitAc(k); }).appendTo($li);
        });
        dugmeler();
    }

    function gorevSec(g) {
        secim.gorev = g.kod;
        secim.gorevAd = g.ad;
        $('#secilenGorev').text(secim.birimAd + ' / ' + g.ad);
        kisileriCiz([]);
        $('#lstKisiler').empty();
        Sgb.al('api/gorevata/kisiler', { orgKod: secim.orgKod, gorev: g.kod })
            .done(function (veri) {
                if (secim.gorev !== g.kod) return;
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                kisileriCiz(veri.satirlar);
            })
            .fail(hataGoster);
    }

    $('#lstKisiler').on('change', 'input[type=checkbox]', dugmeler);

    function ekle(eklenecekler, bitince) {
        var istek = { orgKod: secim.orgKod, gorev: secim.gorev, kisiler: eklenecekler };
        return Sgb.gonder('api/gorevata/kisi-ekle', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                if (secim.orgKod === istek.orgKod && secim.gorev === istek.gorev) kisileriCiz(s.satirlar);
                kisiYukle();
                if (bitince) bitince();
            })
            .fail(hataGoster);
    }

    $('#btnKisiyiEkle').on('click', function () {
        if (!kisi || !secim.gorev) return;
        $(this).prop('disabled', true);
        ekle([{ tc: kisi.tc, ad: kisi.ad, soyad: kisi.soyad }]).always(dugmeler);
    });

    $('#btnKisiSil').on('click', function () {
        var silinecekler = $('#lstKisiler input:checked').map(function () { return $(this).data('kisi'); }).get();
        if (!silinecekler.length) return;
        if (!window.confirm(silinecekler.length + ' kişi "' + secim.gorevAd + '" görevinden silinecek. Onaylıyor musunuz?')) return;
        gorevdenSil(secim.orgKod, secim.gorev, silinecekler);
    });

    function gorevdenSil(orgKod, gorev, silinecekler) {
        $('#btnKisiSil').prop('disabled', true);
        Sgb.gonder('api/gorevata/kisi-sil', { orgKod: orgKod, gorev: gorev, kisiler: silinecekler })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                if (secim.orgKod === orgKod && secim.gorev === gorev) kisileriCiz(s.satirlar);
                kisiYukle();
            })
            .fail(hataGoster)
            .always(dugmeler);
    }

    // ---------------------------------------------------------------- kişi arama

    $('#btnKisiEkle').on('click', function () {
        if (!secim.gorev) return;
        $('#dlgKisiAraBaslik').text('Göreve Kişi Ekle: ' + secim.gorevAd);
        $('#lstBulunan').empty();
        bosSatir($('#lstBulunan'), 'Ad, soyad ya da kullanıcı kodu yazıp Ara\'ya basınız.');
        document.getElementById('dlgKisiAra').showModal();
        $('#txtAraAd').trigger('focus');
    });

    $('#frmKisiAra').on('submit', function (olay) {
        olay.preventDefault();
        var olcut = { ad: $.trim($('#txtAraAd').val()), soyad: $.trim($('#txtAraSoyad').val()), kod: $.trim($('#txtAraKod').val()) };
        if (!olcut.ad && !olcut.soyad && !olcut.kod) { mesajPencerede('Ad, soyad ya da kullanıcı kodundan en az birini yazınız.'); return; }
        $('#btnAra').prop('disabled', true);
        Sgb.al('api/gorevata/kisi-ara', olcut)
            .done(function (veri) {
                var $liste = $('#lstBulunan').empty();
                if (veri.hata) { bosSatir($liste, veri.hata); return; }
                if (!veri.satirlar.length) { bosSatir($liste, 'Kullanıcı bulunamadı.'); return; }
                if (veri.fazla) bosSatir($liste, 'Çok kişi bulundu; ilk ' + veri.satirlar.length + ' kişi gösteriliyor. Ölçütü daraltınız.');
                $.each(veri.satirlar, function (i, k) {
                    var atanmis = kisiler.some(function (x) { return x.tc === k.tc; });
                    var $etiket = $('<label></label>').appendTo($('<li></li>').appendTo($liste));
                    $('<input type="checkbox" />').data('kisi', { tc: k.tc, ad: k.ad, soyad: k.soyad }).prop('disabled', atanmis).appendTo($etiket);
                    $('<span class="ad"></span>').text(adSoyad(k) + ' (' + k.kod + ')' + (k.birim ? ' - ' + k.birim : '') + (atanmis ? ' - bu görevde' : '')).appendTo($etiket);
                    $('<span class="tc"></span>').text(k.tc).appendTo($etiket);
                });
            })
            .fail(hataGoster)
            .always(function () { $('#btnAra').prop('disabled', false); });
    });

    // Pencere açıkken sağ üstteki bildirim pencerenin arkasında kalır; uyarı listeye yazılır.
    function mesajPencerede(metin) {
        bosSatir($('#lstBulunan').empty(), metin);
    }

    $('#btnBulunanEkle').on('click', function () {
        var eklenecekler = $('#lstBulunan input:checked').map(function () { return $(this).data('kisi'); }).get();
        if (!eklenecekler.length) { window.alert('Eklenecek kişileri işaretleyiniz.'); return; }
        var $d = $(this).prop('disabled', true);
        ekle(eklenecekler, function () { document.getElementById('dlgKisiAra').close(); })
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- kısıt

    function kisitAc(k) {
        var istek = { orgKod: secim.orgKod, gorev: secim.gorev, tc: k.tc };
        Sgb.al('api/gorevata/kisit', istek)
            .done(function (veri) {
                kisitKisi = istek;
                $('#kisitKisi').text(adSoyad(k) + ' - ' + secim.gorevAd);
                $('#txtFaaliyet').val(veri.faaliyet || '');
                document.getElementById('dlgKisit').showModal();
            })
            .fail(hataGoster);
    }

    $('#btnKisitKaydet').on('click', function () {
        if (!kisitKisi) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/gorevata/kisit', $.extend({ faaliyet: $.trim($('#txtFaaliyet').val()) }, kisitKisi))
            .done(function (s) {
                if (!s.basarili) { window.alert(s.hata); return; }
                document.getElementById('dlgKisit').close();
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Kullanıcı Giriş'ten gelinen kullanıcı

    function kisiCiz() {
        $('#kisiSerit').prop('hidden', false);
        $('#kisiAd').text(adSoyad(kisi) + ' (' + kisi.kod + ') görevleri:');
        $('#btnKisiyiEkle').prop('hidden', false);
        $('#btnKisiyiEkleMetin').text(adSoyad(kisi) + ' kişisini ekle');
        var $kutu = $('#kisiGorevler').empty();
        if (!kisi.gorevler) { $('<span class="bos"></span>').text('Görevleri okunamadı.').appendTo($kutu); return; }
        if (!kisi.gorevler.length) { $('<span class="bos"></span>').text('Henüz görevi yok. Birim ve görev seçip ekleyiniz.').appendTo($kutu); return; }
        $.each(kisi.gorevler, function (i, g) {
            var ad = (g.birim ? g.birim + ' / ' : '') + g.ad;
            var $e = $('<span class="tanim-etiket"></span>').text(ad).appendTo($kutu);
            $('<button type="button">&times;</button>').attr('aria-label', 'Görevden sil: ' + ad).attr('title', 'Görevden sil').on('click', function () {
                if (!window.confirm(adSoyad(kisi) + ', "' + ad + '" görevinden silinecek. Onaylıyor musunuz?')) return;
                gorevdenSil(g.orgKod, g.gorev, [{ tc: kisi.tc, ad: kisi.ad, soyad: kisi.soyad }]);
            }).appendTo($e);
        });
    }

    function kisiYukle() {
        if (!kisi) return;
        Sgb.al('api/gorevata/kullanici', { kod: kisi.kod })
            .done(function (k) {
                if (k.hata) return;
                kisi = k;
                kisiCiz();
                dugmeler();
            });
    }

    // ---------------------------------------------------------------- açılış


    if (sabitBirim) birimSec({ kod: sabitBirim, ad: adres.get('orgAd') || '' });

    var kod = adres.get('kullanici');
    if (kod) {
        Sgb.al('api/gorevata/kullanici', { kod: kod })
            .done(function (k) {
                if (k.hata) { mesaj(k.hata, 'warning'); return; }
                kisi = k;
                kisiCiz();
                dugmeler();
            })
            .fail(hataGoster);
    }
});
