// Gider Bütçesi Açılış Fişi, ButceMuhasebe/ButceAcilisFisi (eski ButceAcilisFisi.aspx): yıl, muhasebe / birim ve kurumsal koda göre
// bütçe ödeneklerinden (faaliyet bazlı) açılış MİF belgesi oluşturur; oluşan belge Ödeme Emri / MİF penceresinde açılır.
// Eski "Fiş Oluştur" ve "Excel Dosyasından Oluştur" tuşları yok (kullanıcı isteği). Uçlar: /api/butceacilis/* (Muhasebe/ButceAcilisFisiUclari.cs).
$(function () {
    'use strict';

    if (!$('.butce-acilis').length) return;

    var UC = 'api/butceacilis/';

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        sonucYaz(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function sonucYaz(metin, tur) {
        $('#pnlSonuc').removeClass('alert-success alert-danger alert-info').addClass('alert-' + tur).text(metin || '').prop('hidden', !metin);
    }
    function yil() { return parseInt($('#txtYil').val(), 10) || 0; }

    $('#btnYardim').on('click', function () {
        var $p = $('#pnlYardim'), acik = $p.prop('hidden');
        $p.prop('hidden', !acik);
        $(this).attr('aria-expanded', acik ? 'true' : 'false');
    });

    // Birim seçilince kurumsal kod boşsa birimin kurumsal kodu yazılır (kullanıcı değiştirebilir).
    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', ad: 'lblBirimAd' },
        yil: yil,
        kurumsalGizle: true,
        degisince: function (d) {
            if (d && d.kurumsal && !$.trim($('#txtKur').val())) $('#txtKur').val(d.kurumsal);
        }
    });

    function belgeAc(y, belgeNo) {
        Sgb.pencere('../ButceMuhasebe/OdemeEmriMIFFrame?' + $.param({ aramaYok: 1, yil: y, belgeNo: belgeNo }), 'Ödeme Emri / MİF Belgesi ' + belgeNo);
    }

    $('#btnOlustur').on('click', function () {
        var d = muhBirim.deger();
        var istek = { Yil: yil(), Muhasebe: d.muhasebe, Birim: d.birim, Kur: $.trim($('#txtKur').val()), BelgeNo: $.trim($('#txtBelgeNo').val()) };
        var eksik = [];
        if (!istek.Yil) eksik.push('Yıl bilgisi boş bırakılmış.');
        if (!istek.Muhasebe) eksik.push('Muhasebe Kod bilgisi boş bırakılmış.');
        if (!istek.Birim) eksik.push('Birim Kod bilgisi boş bırakılmış.');
        if (!istek.Kur) eksik.push('Kurumsal Kod bilgisi boş bırakılmış.');
        if (eksik.length) { sonucYaz(eksik.join(' '), 'danger'); return; }

        var $d = $(this).prop('disabled', true).addClass('avans-bekle');
        sonucYaz('Açılış fişi oluşturuluyor, lütfen bekleyiniz...', 'info');
        Sgb.gonder(UC + 'olustur', istek)
            .done(function (v) {
                if (!v.basarili) { sonucYaz(v.hata || 'Fiş oluşturulamadı.', 'danger'); return; }
                $('#txtBelgeNo').val(v.belgeNo || '');
                if (v.belgeNo) {
                    sonucYaz(v.yil + ' yılı ' + v.belgeNo + ' numaralı açılış fişi oluşturuldu.', 'success');
                    belgeAc(v.yil, v.belgeNo);
                } else sonucYaz('Ödeme emri belge numarası okunamadı.', 'danger');
            })
            .fail(function (x) { hataGoster(x); })
            .always(function () { $d.prop('disabled', false).removeClass('avans-bekle'); });
    });

    Sgb.al(UC + 'tanim')
        .done(function (t) { Sgb.yilKutusu('#txtYil', t.yillar, t.yil); })
        .fail(hataGoster);
});
