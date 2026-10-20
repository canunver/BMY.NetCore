// Firma Bilgisi Düzenleme, ButceMuhasebe/FirmaBilgisiDuzenleme (eski aynı adlı Ext.NET sayfası): aynı firma/kişinin kayıtları
// tek ana kayıtta toplanır. Eski sayfadaki sürükle-bırak yerine düğmeler ve çift tıklama kullanılır.
// Uçlar: /api/firmaduzenleme/* (Muhasebe/FirmaBilgisiDuzenlemeUclari.cs).
$(function () {
    'use strict';

    if (!$('.firma-duzenleme').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/firmaduzenleme/';
    var muhasebe = new URLSearchParams(window.location.search).get('muhasebe') || '';
    var ana = null; // ana kayıt satırı { KOD, VERGINO, AD }

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });

    function sutunlar() {
        return [
            { field: 'VERGINO', headerName: 'Vergi No', width: 120 },
            { field: 'AD', headerName: 'Ad', flex: 1, minWidth: 160, tooltipField: 'AD' }
        ];
    }
    var secim = { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: true };

    var grdBaslik = Sgb.grid('#grdBaslik', {
        columnDefs: sutunlar(),
        rowSelection: secim,
        getRowId: function (p) { return satirKimlik(p.data); },
        overlayNoRowsTemplate: '<span>Firma/kişi adını yazıp Ara\'ya basınız.</span>',
        onRowDoubleClicked: function (p) { anaYap(p.data); }
    });
    var grdSecenek = Sgb.grid('#grdSecenek', {
        columnDefs: sutunlar(),
        rowSelection: secim,
        getRowId: function (p) { return satirKimlik(p.data); },
        overlayNoRowsTemplate: '<span>Soldan birleştirilecek kayıtları ekleyiniz.</span>',
        onRowDoubleClicked: function (p) { cikar([p.data]); }
    });

    function satirlar(g) { var a = []; g.forEachNode(function (n) { if (n.data) a.push(n.data); }); return a; }
    function adSirala(a) { return a.sort(function (x, y) { return String(x.AD || '').localeCompare(String(y.AD || ''), 'tr'); }); }
    function satirKimlik(r) { return String(r.KOD) + '|' + String(r.VERGINO); }
    function varMi(g, r) {
        var bulundu = false;
        g.forEachNode(function (n) { if (n.data && String(n.data.VERGINO) === String(r.VERGINO)) bulundu = true; });
        return bulundu;
    }

    // Eski notifyDrop: seçilen satır ana kayda yüklenir ve listeden kalkar. Önceki ana kayıt listeye geri döner.
    function anaYap(r) {
        if (!r) { mesaj('Ana kayıt yapılacak satırı seçiniz.', 'warning'); return; }
        var geri = ana && !varMi(grdBaslik, ana) && !varMi(grdSecenek, ana) ? [ana] : [];
        grdBaslik.applyTransaction({ remove: [r], add: geri });
        ana = r;
        $('#txtVergiNo').val(r.VERGINO || '');
        $('#txtAd').val(r.AD || '');
    }

    // Eski notifyDrop2 / addRow: aynı vergi no yoksa eklenir, ada göre sıralanır.
    function ekle(liste) {
        // Eski addRow gibi: aynı vergi no hedefte varsa satır yerinde kalır.
        var eklenecek = [];
        $.each(liste, function (i, r) { if (!varMi(grdSecenek, r) && !$.grep(eklenecek, function (x) { return String(x.VERGINO) === String(r.VERGINO); }).length) eklenecek.push(r); });
        grdBaslik.applyTransaction({ remove: eklenecek });
        grdSecenek.setGridOption('rowData', adSirala(satirlar(grdSecenek).concat(eklenecek)));
    }

    // Eski notifyDrop1: birleştirilecekler listesinden arama listesine geri alır.
    function cikar(liste) {
        var geri = [];
        $.each(liste, function (i, r) { if (!varMi(grdBaslik, r) && !$.grep(geri, function (x) { return String(x.VERGINO) === String(r.VERGINO); }).length) geri.push(r); });
        grdSecenek.applyTransaction({ remove: geri });
        grdBaslik.setGridOption('rowData', adSirala(satirlar(grdBaslik).concat(geri)));
    }

    $('#btnAnaYap').on('click', function () {
        var s = grdBaslik.getSelectedRows();
        if (s.length !== 1) { mesaj('Ana kayıt için bir satır seçiniz.', 'warning'); return; }
        anaYap(s[0]);
    });
    $('#btnEkle').on('click', function () {
        var s = grdBaslik.getSelectedRows();
        if (!s.length) { mesaj('Birleştirilecek satırları seçiniz.', 'warning'); return; }
        ekle(s);
    });
    $('#btnCikar').on('click', function () {
        var s = grdSecenek.getSelectedRows();
        if (!s.length) { mesaj('Listeden çıkarılacak satırları seçiniz.', 'warning'); return; }
        cikar(s);
    });

    // Eski FirmalariDoldur (Ara düğmesi)
    function listele(sessiz) {
        var ara = $.trim($('#txtAra').val());
        if (!ara) {
            grdBaslik.setGridOption('rowData', []);
            if (!sessiz) mesaj('Lütfen arama alanına kriter girerek arama yapınız', 'warning');
            return;
        }
        var $d = $('#btnAra').prop('disabled', true);
        grdBaslik.setGridOption('loading', true);
        Sgb.al(UC + 'liste', { ara: ara, muhasebe: muhasebe })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                // Ana kayıtta ya da birleştirilecekler listesinde olanlar tekrar gösterilmez.
                var liste = $.grep(v.satirlar || [], function (r) {
                    return !(ana && String(ana.VERGINO) === String(r.VERGINO)) && !varMi(grdSecenek, r);
                });
                grdBaslik.setGridOption('rowData', liste);
                grdBaslik.setGridOption('overlayNoRowsTemplate', '<span>Aradığınız kritere uygun firma/kişi bulunamadı.</span>');
            })
            .fail(hataGoster)
            .always(function () { grdBaslik.setGridOption('loading', false); $d.prop('disabled', false); });
    }
    $('#btnAra').on('click', function () { listele(false); });
    $('#txtAra').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); listele(false); } });

    // Eski Temizle düğmesi ve Kaydet sonrası: ana kayıt ve birleştirilecekler boşaltılır, liste yeniden okunur.
    function temizle() {
        ana = null;
        $('#txtVergiNo').val('');
        $('#txtAd').val('');
        grdSecenek.setGridOption('rowData', []);
        listele(true);
    }
    $('#btnTemizle').on('click', temizle);

    // Eski btnKaydet_Click
    $('#btnKaydet').on('click', function () {
        var secilenler = satirlar(grdSecenek);
        if (!ana || !$('#txtAd').val()) { mesaj('Lütfen ana kayit için gerekli bilgileri giriniz', 'warning'); return; }
        if (!secilenler.length) { mesaj('Lütfen kayit için gerekli bilgileri giriniz', 'warning'); return; }
        if (!window.confirm('Değişiklikleri kaydetmek istiyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'kaydet', {
            anaVergiNo: $('#txtVergiNo').val(),
            anaAd: $('#txtAd').val(),
            vergiNolar: $.map(secilenler, function (r) { return String(r.VERGINO); })
        })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata || 'Kayıt yapılamadı.', 'danger'); return; }
                mesaj(v.bilgi || 'Kayit işlemi tamamlandı', 'success');
                temizle();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // Tablolar sayfanın kalan yüksekliğini doldurur.
    function yay() {
        $.each(['grdBaslik', 'grdSecenek'], function (i, id) {
            var el = document.getElementById(id);
            if (!el || el.offsetParent === null) return;
            var ust = el.getBoundingClientRect().top;
            el.style.height = Math.max(220, Math.floor(window.innerHeight - ust - 60)) + 'px';
        });
    }
    $(window).on('resize', yay);
    yay();
    $('#txtAra').trigger('focus');
});
