// Faaliyet Raporu Raporları (ButceHazirlikv2/FaaliyetRaporuAl; eski FaaliyetRaporuAl.aspx).
// Soldan rapor seçilir (açılışta ilki seçili). Göster raporu PDF olarak yeni sekmede açar (eski SayfaAc), İndir dosyayı indirir;
// sistem yöneticisi "PDF olarak üret" işaretini kaldırırsa Word (.docx) iner, diğerlerine hep PDF.
$(function () {
    'use strict';

    if (!$('#agcRapor').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var raporlar = [];
    var secili = null;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnGoster, #btnIndir').prop('disabled', evet || !secili); }

    function parametreler(goster) {
        var yil = parseInt($('#txtYil').val(), 10) || 0;
        if (!secili) { mesaj('Listeden görmek istediğiniz raporu seçiniz.', 'warning'); return null; }
        if (yil < 1000) { mesaj('Lütfen yıl giriniz.', 'warning'); $('#txtYil').trigger('focus'); return null; }
        return {
            rapor: secili.kod,
            yil: yil,
            grup: $('input[name=grup]:checked').val() || 0,
            yikob: $('input[name=yikob]:checked').val() || 0,
            pdf: $('.rapor-pdf').prop('hidden') || $('#chkPdf').prop('checked') ? 1 : 0,
            goster: goster ? 1 : 0
        };
    }

    // ---------------------------------------------------------------- rapor listesi

    var agac = null;
    function raporSecildi(d) {
        secili = d;
        $('#lblRapor').text(d.ad);
        mesgul(false);
    }

    Sgb.al('api/faaliyetraporu/tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            $('#txtYil').val(v.yil);
            var yonetici = !!v.sistemYoneticisi;
            $('.rapor-pdf').prop('hidden', !yonetici);
            $('#chkPdf').prop('checked', !yonetici); // eski: sistem yöneticisine işaretsiz (Word), diğerlerine gizli ve PDF
            raporlar = $.map(v.raporlar || [], function (r) { return { kod: r.kod, ad: r.siraNo + '. ' + r.ad, raporAd: r.ad }; });
            if (raporlar.length) raporSecildi(raporlar[0]);
            agac = Sgb.agac('#agcRapor', {
                duz: true,
                yukle: function () { return $.Deferred().resolve(raporlar).promise(); },
                seciliMi: function (d) { return secili && secili.kod === d.kod; },
                secilince: raporSecildi,
                bosMetin: 'Rapor yok.'
            });
        })
        .fail(hataGoster);

    // ---------------------------------------------------------------- Göster / İndir

    $('#btnIndir').on('click', function () {
        var p = parametreler(false);
        if (!p) return;
        mesgul(true);
        Sgb.indir('api/faaliyetraporu/rapor', p).then(function () { mesgul(false); });
    });

    // Sekme tıklama anında açılır (açılır pencere engellenmesin), rapor hazır olunca içine konur; hata olursa kapatılır.
    $('#btnGoster').on('click', function () {
        var p = parametreler(true);
        if (!p) return;
        var sekme = window.open('', '_blank');
        if (sekme) sekme.document.title = 'Rapor hazırlanıyor...';
        mesgul(true);
        fetch('../api/faaliyetraporu/rapor?' + $.param(p), { credentials: 'same-origin', cache: 'no-store' })
            .then(function (yanit) {
                if (yanit.status === 401) { if (sekme) sekme.close(); window.top.location.href = '../Giris'; return; }
                var tur = yanit.headers.get('Content-Type') || '';
                if (!yanit.ok || tur.indexOf('json') >= 0 || tur.indexOf('text/html') >= 0) {
                    if (sekme) sekme.close();
                    if (tur.indexOf('json') >= 0) return yanit.json().then(function (v) { mesaj(v.hata || 'Rapor hazırlanamadı.', 'danger'); });
                    mesaj(yanit.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Rapor hazırlanamadı.', 'danger');
                    return;
                }
                return yanit.blob().then(function (icerik) {
                    var yerel = URL.createObjectURL(icerik);
                    if (sekme) sekme.location.href = yerel;
                    else window.open(yerel, '_blank');
                    setTimeout(function () { URL.revokeObjectURL(yerel); }, 60000);
                });
            })
            .catch(function () { if (sekme) sekme.close(); mesaj('Rapor hazırlanamadı.', 'danger'); })
            .then(function () { mesgul(false); });
    });

    $('#frmKistas').on('submit', function (e) { e.preventDefault(); $('#btnGoster').trigger('click'); });
});
