// Kurumsal Mali Durum ve Beklentiler Raporu (ButceHazirlikv2/BHMMaliDurumBeklentiForm): seçilen yıl ve faaliyetin dört dönem metni
// Oku ile gelir, Kaydet ile yazılır (kayıt silinip yeniden yazılır). Yazdır (Yeni) Word raporunu indirir; faaliyet boşsa yılın
// bütün faaliyetleri raporlanır. Veri Gir. Kapat / Aç yalnızca sistem yöneticisine görünür.
$(function () {
    'use strict';

    if (!$('#frmMetin').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var METINLER = {
        butceUygulamaSonucu: '#txtButceUygulamaSonucu',
        yurutulenFaaliyetler: '#txtYurutulenFaaliyetler',
        beklentilerveHedefler: '#txtBeklentilerveHedefler',
        yurutulecekFaaliyetler: '#txtYurutulecekFaaliyetler'
    };
    var kirli = false;
    var $faaliyet = $('#txtFaaliyet');

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function sonucGoster(s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }

    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }

    function secim() { return { yil: yilOku(), faaliyet: $.trim($faaliyet.val()) }; }

    function kirliYap(evet) {
        kirli = evet;
        $('#kirliEtiket').prop('hidden', !evet);
    }

    function degisiklikVarsaSor() {
        return !kirli || window.confirm('Kaydedilmemiş değişiklikler var. Yeniden okunursa değişiklikler kaybolacak. Devam edilsin mi?');
    }

    function metinleriYaz(v) {
        $.each(METINLER, function (alan, secici) { $(secici).val(v ? v[alan] || '' : ''); });
        kirliYap(false);
    }

    $('#frmMetin').on('input', 'textarea', function () { kirliYap(true); });

    // ---------------------------------------------------------------- Oku (eski btnOku_Click)

    function oku() {
        var o = secim();
        if (o.yil <= 0 || !kodAl(o.faaliyet)) { mesaj('Anahtar alanları girmelisiniz.', 'warning'); return; }
        if (!degisiklikVarsaSor()) return;
        var $d = $('#btnOku').prop('disabled', true);
        Sgb.al('api/malidurum/oku', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                metinleriYaz(v.bulundu ? v : null);
                $('#lblOkunan').text(o.yil + ' / ' + o.faaliyet);
                if (!v.bulundu) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    }

    $('#frmSecim').on('submit', function (e) { e.preventDefault(); oku(); });

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;

    function sor(baslik, metin, evet, is) {
        $('#dlgSoruBaslik').text(baslik);
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

    // ---------------------------------------------------------------- Kaydet (eski btnKaydet_Click)

    $('#btnKaydet').on('click', function () {
        var o = secim();
        if (o.yil <= 0 || !kodAl(o.faaliyet)) { mesaj('Anahtar alanları girmelisiniz.', 'warning'); return; }
        sor('Onay', 'Kaydetmek İstiyor musunuz?', 'Kaydet', function () {
            var veri = { yil: o.yil, faaliyet: o.faaliyet };
            $.each(METINLER, function (alan, secici) { veri[alan] = $(secici).val() || ''; });
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder('api/malidurum/kaydet', veri)
                .done(function (s) {
                    sonucGoster(s);
                    if (s.basarili) kirliYap(false);
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    // ---------------------------------------------------------------- Yazdır (Yeni) (eski btnYazdirYeni_Click)

    $('#btnYazdir').on('click', function () {
        var o = secim();
        if (o.yil <= 0) { mesaj('Yıl hatalı!', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/malidurum/rapor', o).then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- Veri Gir. Kapat / Aç (eski btnOnayla / btnOnaykaldir)

    function onayIslem(kapat) {
        var yil = yilOku();
        if (yil <= 0) { mesaj('Yıl hatalı!', 'warning'); return; }
        sor('Onay', yil + ' yılı mali beklenti raporu veri girişine ' + (kapat ? 'kapatılacak' : 'açılacak') + '. Devam edilsin mi?',
            kapat ? 'Kapat' : 'Aç', function () {
                var $d = $('#btnOnayla, #btnOnayKaldir').prop('disabled', true);
                Sgb.gonder('api/malidurum/onay', { yil: yil, kapat: kapat })
                    .done(sonucGoster)
                    .fail(hataGoster)
                    .always(function () { $d.prop('disabled', false); });
            });
    }

    $('#btnOnayla').on('click', function () { onayIslem(true); });
    $('#btnOnayKaldir').on('click', function () { onayIslem(false); });

    window.addEventListener('beforeunload', function (e) {
        if (kirli) { e.preventDefault(); e.returnValue = ''; }
    });

    // ---------------------------------------------------------------- faaliyet kod ağacı (eski açılır ağaç)

    var dlgKod = document.getElementById('dlgKod');
    var agac = null;
    var agacYil = 0;

    function agacAc() {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        dlgKod.showModal();
        if (!agac) {
            agacYil = yil;
            // Sgb.agac kurulurken kökü hemen yükler; yıl ondan önce atanmalı.
            agac = Sgb.agac($('<ul></ul>').attr('aria-label', 'Faaliyet kod ağacı').appendTo('#agacKutu')[0], {
                yukle: function (ust) {
                    return Sgb.al('api/malidurum/kodlar', { yil: agacYil, tur: '14', ust: ust ? ust.kod : '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    dlgKod.close();
                    $faaliyet.val(d.ad).trigger('change');
                },
                seciliMi: function (d) { return kodAl($faaliyet.val()) === d.kod; },
                ilkAcik: true,
                bosMetin: 'Kod bulunamadı.',
                hata: hataGoster
            });
        } else if (agacYil !== yil) {
            agacYil = yil;
            agac.yenile();
        }
    }

    $('#btnKodSec').on('click', agacAc);
    $('#btnKodTemizle').on('click', function () { $faaliyet.val('').trigger('focus'); });
    // Alana kod yazılabilir; Alt+Aşağı ağacı açar.
    $faaliyet.on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc(); }
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al('api/malidurum/tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            $('#txtYil').val(t.yil);
            $('#btnOnayla, #btnOnayKaldir').prop('hidden', !t.yonetici);
        })
        .fail(hataGoster);
});
