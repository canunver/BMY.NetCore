// Bütçe Hazırlık Parametreleri (ButceHazirlikv2/ButceParametre): seçilen yılın parametreleri alt alta; her satırda parametrenin
// durumu ve "Durum Değiştir" tuşu (durumu tersine çevirir). Yıl değişince durumlar yeniden okunur. Açılışta bütçe hazırlık yılı gelir.
$(function () {
    'use strict';

    if (!$('#parametreler').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }

    function yaz(parametreler) {
        var $k = $('#parametreler').empty();
        $.each(parametreler || [], function (i, p) {
            var $g = $('<div class="form-group"></div>').appendTo($k);
            $('<span class="control-label"></span>').text(p.etiket).appendTo($g);
            var $d = $('<div class="parametre-satir"></div>').appendTo($g);
            $('<span class="label parametre-durum"></span>').addClass(p.acik ? 'label-success' : 'label-default').text(p.durum).appendTo($d);
            $('<button type="button" class="btn btn-default btn-sm"></button>').attr('data-kimlik', p.kimlik)
                .attr('aria-label', p.etiket + ': durum değiştir').text('Durum Değiştir').appendTo($d);
        });
    }

    function oku(yil) {
        Sgb.al('api/butceparametre/durum', yil ? { yil: yil } : null)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#txtYil').val(v.yil).attr('max', v.enBuyukYil);
                yaz(v.parametreler);
            })
            .fail(hataGoster);
    }

    $('#txtYil').on('change', function () {
        var yil = yilOku();
        if (yil < 2003) { mesaj('Yıl bilgisi hatalı!', 'warning'); return; }
        oku(yil);
    });
    $('#frmYil').on('submit', function (e) { e.preventDefault(); $('#txtYil').trigger('change'); });

    $('#parametreler').on('click', 'button[data-kimlik]', function () {
        var $b = $(this).prop('disabled', true);
        Sgb.gonder('api/butceparametre/degistir', { yil: yilOku(), kimlik: $b.attr('data-kimlik') })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                yaz(s.parametreler);
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $b.prop('disabled', false); });
    });

    oku(0);
});
