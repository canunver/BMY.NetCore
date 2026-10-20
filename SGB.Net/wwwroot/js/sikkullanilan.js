// Sık Kullanılanlar bilgi ekranı (OrtakSayfa/SikKullanilan, eski Dashboard/Genel/SikKullanilan.ascx).
// Açılış sayfasında kendi çerçevesinde açılır; ekranlar modül başlığı altında listelenir, bağlantılar üst pencerede açılır.
$(function () {
    'use strict';

    var kap = document.getElementById('liste');

    function el(etiket, sinif, metin) {
        var e = document.createElement(etiket);
        if (sinif) e.className = sinif;
        if (metin !== undefined && metin !== null) e.textContent = metin;
        return e;
    }

    Sgb.al('api/ortaksayfa/sik-kullanilan-liste')
        .done(function (veri) {
            if (!veri.basarili) {
                Sgb.mesaj('#hata', veri.hata || 'Sık kullanılanlar okunamadı.');
                return;
            }
            var moduller = veri.moduller || [];
            if (!moduller.length) {
                kap.appendChild(el('p', 'sik-bos', kap.dataset.bos));
                return;
            }
            moduller.forEach(function (modul) {
                kap.appendChild(el('div', 'sik-modul', modul.ad));
                var ul = el('ul');
                modul.ekranlar.forEach(function (ekran) {
                    var a = el('a', null, ekran.ad);
                    // Menü adresleri sayfaya göre göreli (../Modul/Sayfa); bu sayfa da bir alt klasörde olduğundan aynen kullanılır.
                    a.href = ekran.url;
                    a.title = ekran.yer + ' › ' + ekran.ad;
                    a.target = ekran.yeni ? '_blank' : '_top';
                    if (ekran.yeni) a.rel = 'noopener';
                    var li = el('li');
                    li.appendChild(a);
                    ul.appendChild(li);
                });
                kap.appendChild(ul);
            });
        })
        .fail(function (xhr) {
            if (xhr.status !== 401) Sgb.mesaj('#hata', 'Sık kullanılanlar okunamadı.');
        });
});
