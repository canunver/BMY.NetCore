// Grafik gösterme sayfası (OrtakSayfa/GrafikGoster?param=...): grafiği /api/grafik ucundan okuyup çizer.
$(function () {
    'use strict';

    var param = new URLSearchParams(window.location.search).get('param') || '';
    var $kutu = $('#grafik');
    function yaz(metin) { $('<p class="sgb-grafik-bos"></p>').text(metin).appendTo($kutu.empty()); }

    if (!param) { yaz('Grafik seçilmedi.'); return; }
    Sgb.al('api/grafik', { param: param })
        .done(function (veri) {
            if (veri.hata) yaz(veri.hata);
            else Sgb.grafik($kutu, veri);
        })
        .fail(function (xhr) {
            if (xhr && xhr.status === 401) return;
            yaz(xhr && xhr.status === 403 ? 'Bu grafiği görme yetkiniz yok.' : 'Grafik okunamadı.');
        });
});
