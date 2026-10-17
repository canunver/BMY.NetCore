// Bütçe Sözlük İşlemleri (ButceHazirlikv2/ButceSozlukYonetim): seçilen yılın ve önceki yılın sınıflandırma kodu, tertip, hizmet
// gerekçesi ve bütçe fişi sayıları. Kopyala yalnızca önceki yılda kayıt varken ve bu yılda yokken açıktır; Düzenle ilgili tanım
// sayfasını seçilen yılla yeni sekmede açar. Açılışta Ocak-Mart içinde bulunulan yıl, Nisan'dan sonra bir sonraki yıl gelir.
$(function () {
    'use strict';

    if (!$('.butce-sozluk').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    var gosterilenYil = 0;

    function yaz(yil, satirlar) {
        gosterilenYil = yil;
        $('.sozluk-tablo .bu-yil').text(yil + ' Yılı Değeri');
        $('.sozluk-tablo .onc-yil').text((yil - 1) + ' Yılı Değeri');
        $('.sozluk-tablo tbody').empty();
        $.each(satirlar || [], function (i, s) {
            var $tb = $('.sozluk-tablo').filter(function () { return $(this).data('bolum') === s.bolum; }).find('tbody');
            var $tr = $('<tr></tr>').appendTo($tb);
            $('<th scope="row"></th>').text(s.etiket).appendTo($tr);
            $('<td class="sayi"></td>').text(s.buYil).appendTo($tr);
            $('<td class="sayi"></td>').text(s.oncYil).appendTo($tr);
            var $t = $('<td class="tuslar"></td>').appendTo($tr);
            $('<button type="button" class="btn btn-default btn-xs" data-kopyala></button>').attr('data-kimlik', s.kimlik)
                .attr('aria-label', s.etiket + ': önceki yıldan kopyala')
                .attr('title', s.kopyalanir ? 'Önceki yılın kayıtlarını bu yıla kopyalar' : 'Yalnızca önceki yılda kayıt varken ve bu yılda yokken kopyalanır')
                .prop('disabled', !s.kopyalanir)
                .html('<span class="glyphicon glyphicon-duplicate" aria-hidden="true"></span> Kopyala').appendTo($t);
            $('<button type="button" class="btn btn-default btn-xs" data-duzenle></button>').attr('data-adres', s.adres)
                .attr('aria-label', s.etiket + ': düzenle')
                .html('<span class="glyphicon glyphicon-edit" aria-hidden="true"></span> Düzenle').appendTo($t);
        });
    }

    function oku(yil) {
        $('.sozluk-tablo tbody').html('<tr><td colspan="4" class="text-muted">Okunuyor...</td></tr>');
        Sgb.al('api/butcesozluk/durum', yil ? { yil: yil } : null)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#txtYil').val(v.yil).attr('max', v.enBuyukYil);
                yaz(v.yil, v.satirlar);
            })
            .fail(hataGoster);
    }

    $('#txtYil').on('change', function () {
        var yil = yilOku();
        if (yil < 2003 || yil > parseInt($(this).attr('max') || '9999', 10)) { mesaj('Yıl bilgisi hatalı!', 'warning'); return; }
        oku(yil);
    });
    $('#frmYil').on('submit', function (e) { e.preventDefault(); $('#txtYil').trigger('change'); });

    $('.sozluk-tablo').on('click', 'button[data-kopyala]', function () {
        var $b = $(this);
        if (!window.confirm($b.closest('tr').children('th').text() + ': ' + (gosterilenYil - 1) + ' yılının kayıtları ' + gosterilenYil + ' yılına kopyalanacak. Devam edilsin mi?')) return;
        $b.prop('disabled', true);
        Sgb.gonder('api/butcesozluk/kopyala', { yil: gosterilenYil, kimlik: $b.attr('data-kimlik') })
            .done(function (s) {
                if (s.satirlar) yaz(gosterilenYil, s.satirlar);
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
            })
            .fail(function (xhr) { hataGoster(xhr); $b.prop('disabled', false); });
    });

    $('.sozluk-tablo').on('click', 'button[data-duzenle]', function () {
        window.open($(this).attr('data-adres'), '_blank');
    });

    oku(0);
});
