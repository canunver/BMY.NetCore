// Faiz Hesaplama ve Detay (AlacakTakip/FaizHesabi): eski FaizHesabi.aspx.
//   Tecil Durumu "Tecil Yok" değilse Tecil Başlangıç Tarihi görünür (eski AcKapa).
//   Hesapla: /api/alacaktakip/faizhesabi/hesapla; faizi ve hesabın ayrıntısını (Sonuç) yazar, hatalar Sonuç'a yazılır (eskisi gibi).
(function ($) {
    'use strict';

    var UC = 'api/alacaktakip/faizhesabi/';
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function tecilAcKapa() {
        $('#grpTecilBasla').prop('hidden', $('#cmbTecilTur').val() === '0');
    }

    function hesapla(e) {
        e.preventDefault();
        var $t = $('#btnHesapla').prop('disabled', true);
        $('#txtFaiz').val('');
        Sgb.gonder(UC + 'hesapla', {
            anaPara: Number($('#txtAnaPara').val()) || 0,
            faizBasla: $('#txtFaizBasla').val() || '',
            tecilTur: Number($('#cmbTecilTur').val()) || 0,
            tecilBasla: $('#txtTecilBasla').val() || '',
            islemTar: $('#txtIslemTar').val() || ''
        }).done(function (c) {
            if (!c || !c.basarili) {
                $('#txtSonuc').val(c && c.hata ? c.hata : 'Faiz hesaplanamadı.');
                return;
            }
            $('#txtFaiz').val(para.format(Number(c.faiz) || 0));
            $('#txtSonuc').val(c.sonuc || '');
        }).fail(function (xhr) {
            if (xhr && xhr.status === 401) return;
            Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
        }).always(function () { $t.prop('disabled', false); });
    }

    $(function () {
        $('#cmbTecilTur').on('change', tecilAcKapa);
        $('#frmFaiz').on('submit', hesapla);
        tecilAcKapa();
    });
})(jQuery);
