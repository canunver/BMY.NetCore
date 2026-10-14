// Yazılım Güncelle (eski YazilimGuncelle/YazilimGuncelle.aspx). Uç: /api/guncelleme/yazilim-zip
// Sunucunun yaptığı işlemler (güncellenecek dosyalar, hatalar) İşlem Bilgileri alanına satır satır yazılır.
(function ($) {
    'use strict';

    var $bilgi = $('#lstBilgi');
    var calisiyor = false;

    function yaz(satirlar) {
        $bilgi.find('.bos').remove();
        $.each(satirlar || [], function (i, s) {
            $('<li></li>').addClass(s.tur || 'bilgi').text(s.metin).appendTo($bilgi);
        });
        $bilgi.scrollTop($bilgi[0].scrollHeight);
    }

    function dugmeler(kapali) {
        calisiyor = kapali;
        $('#btnZiptenYukle').prop('disabled', kapali);
    }

    // Zip'ten güncelle: dosya çok parçalı form olarak gönderilir.
    $('#frmZip').on('submit', function (e) {
        e.preventDefault();
        if (calisiyor) return;
        var dosya = $('#fuDosya')[0].files[0];
        $bilgi.empty();
        if (!dosya) { yaz([{ metin: 'Yüklenecek dosyayı seçiniz!', tur: 'hata' }]); return; }
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('zamanFark', $('#txtZamanFark').val());
        yaz([{ metin: 'Dosya yükleniyor ve güncelleme yapılıyor, lütfen bekleyiniz...', tur: 'bilgi' }]);
        dugmeler(true);
        $.ajax({
            url: Sgb.KOK + 'api/guncelleme/yazilim-zip',
            method: 'POST',
            data: veri,
            processData: false,
            contentType: false,
            dataType: 'json',
            headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            $bilgi.empty();
            yaz(v.satirlar);
            $('#fuDosya').val('');
        }).fail(function (x) {
            if (x.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            $bilgi.empty();
            yaz([{ metin: x.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Dosya yüklenemedi.', tur: 'hata' }]);
        }).always(function () { dugmeler(false); });
    });

})(jQuery);
