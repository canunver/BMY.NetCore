// Sayfa çizilmeden önce çalışır: tarayıcıda saklanan tema ve menü görünümünü <html> üzerine yazar.
// Böylece sayfa önce varsayılan renk ve yerleşimle görünüp sonra değişmez.
(function () {
    var kok = document.documentElement;
    try {
        var veri = JSON.parse(localStorage.getItem('sgb.menu') || 'null');
        if (veri && veri.tema) kok.setAttribute('data-tema', veri.tema);
    } catch (e) { /* saklanan veri okunamıyorsa varsayılan tema kalır */ }

    // Menü görünümü: acik (sabit, geniş) | ikon (dar şerit, üzerine gelince açılır) | gizli
    var kip = 'ikon';
    try {
        var k = localStorage.getItem('sgb.menuKipi');
        if (k === 'acik' || k === 'ikon' || k === 'gizli') kip = k;
    } catch (e) { }
    kok.setAttribute('data-menu', kip);
})();
