// Çek Listesi, ButceMuhasebe/CekKocaniForm (eski aynı adlı Ext.NET sayfası): hesap kodu, çek no, durum ve tarihe göre çekler listelenir,
// Excel'e yazdırılır (MUH/CekListe.xltx); iptal çek eklenir, satırdaki iptal bilgisi silinir.
// Uçlar: /api/ceklistesi/* (Muhasebe/CekListesiUclari.cs).
$(function () {
    'use strict';

    if (!$('.ceklistesi').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/ceklistesi/';
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var bugun = Sgb.bugun();

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function para(x) { return x.value === null || x.value === undefined || x.value === '' ? '' : tutar2.format(Number(x.value) || 0); }
    function tarih(x) { return x.value ? Sgb.tarih(x.value) : ''; }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });
    $('dialog').on('click', '[data-kapat]', function (e) { e.stopPropagation(); this.closest('dialog').close(); });

    var grid = Sgb.grid('#grdCekler', {
        columnDefs: [
            { field: 'hesapKod', headerName: 'Hesap Kod', width: 110 },
            { field: 'cekNo', headerName: 'Çek No', width: 100 },
            { field: 'gun', headerName: 'Gün', width: 95, valueFormatter: tarih },
            { field: 'tcVno', headerName: 'İlgili No', width: 110 },
            { field: 'ilgili', headerName: 'İlgili Unvan', width: 280, tooltipField: 'ilgili' },
            { field: 'mifBelgeNo', headerName: 'Belge No', width: 100 },
            { field: 'tutar', headerName: 'Miktar', width: 110, type: 'rightAligned', valueFormatter: para },
            { field: 'odemeEmriNo', headerName: 'Mahsup No', width: 100 },
            { field: 'tahsilTarihi', headerName: 'Mahsup Tar.', width: 95, valueFormatter: tarih },
            { field: 'iptalTarihi', headerName: 'İptal Tarihi', width: 95, valueFormatter: tarih },
            { field: 'iptalNedeni', headerName: 'Gerekçe', width: 280, tooltipField: 'iptalNedeni' },
            { field: 'cekDurumAd', headerName: 'Durum', width: 100 },
            {
                colId: 'sil', headerName: '', width: 44, sortable: false, resizable: false,
                cellRenderer: function (p) {
                    if (!p.data) return '';
                    return $('<button type="button" class="btn btn-link btn-xs cek-sil" title="İptal Bilgisi Sil" aria-label="İptal Bilgisi Sil">'
                        + '<span class="glyphicon glyphicon-trash" aria-hidden="true"></span></button>')
                        .on('click', function (e) { e.stopPropagation(); iptalSil(p.data); })[0];
                }
            }
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        overlayNoRowsTemplate: '<span>Kıstasları girip Listele\'ye basınız.</span>'
    });

    function kriter() {
        return {
            hesapKod: $.trim($('#txtHesapKodSor').val()),
            cekNo: $.trim($('#txtCekNoSor').val()),
            iptalli: $('#chkIptalli').is(':checked') ? 1 : 0,
            acik: $('#chkAcik').is(':checked') ? 1 : 0,
            bozdurulmus: $('#chkBozdurulmus').is(':checked') ? 1 : 0,
            basla: $('#txtBasla').val() || '',
            bitis: $('#txtBitis').val() || ''
        };
    }

    // Eski btnOku_Click
    function listele() {
        var $d = $('#btnOku').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', kriter())
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var satirlar = v.satirlar || [];
                grid.setGridOption('rowData', satirlar);
                grid.setGridOption('overlayNoRowsTemplate', '<span>Aradığınız kriterlere uygun çek bulunamadı.</span>');
                var t = 0;
                $.each(satirlar, function (i, r) { t += Number(r.tutar) || 0; });
                $('#lblOzet').text(satirlar.length ? satirlar.length + ' çek, toplam ' + tutar2.format(t) : '');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); $d.prop('disabled', false); });
    }

    $('#btnOku').on('click', listele);
    $('#frmKriter').on('keydown', 'input', function (e) { if (e.key === 'Enter') { e.preventDefault(); listele(); } });

    // Eski btnYazdir_Click
    $('#btnYazdir').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.indir(UC + 'yazdir', kriter()).then(function () { $d.prop('disabled', false); });
    });

    // Eski IptalCekSil
    function iptalSil(r) {
        if (!window.confirm('İptal bilgisi silinecek. Onaylıyor musunuz?')) return;
        Sgb.gonder(UC + 'sil', { hesapKod: r.hesapKod, cekNo: r.cekNo })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi || 'İptal bilgisi kaldırıldı.', 'success');
                listele();
            })
            .fail(hataGoster);
    }

    // İptal çek ekle (eski wndIptal, btnKaydet_Click)
    var dlgIptal = document.getElementById('dlgIptal');
    $('#btnIptalAc').on('click', function () {
        $('#txtIptalTarih').val(bugun);
        dlgIptal.showModal();
        $('#txtHesapKodYaz').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        if (!window.confirm('Çek iptal bilgisi kaydedilecek. Onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'kaydet', {
            hesapKod: $.trim($('#txtHesapKodYaz').val()),
            cekNo: $.trim($('#txtCekNoYaz').val()),
            iptalTarihi: $('#txtIptalTarih').val() || '',
            iptalNedeni: $('#txtIptalNeden').val()
        })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi || 'Kaydedildi.', 'success');
                $('#txtCekNoYaz').val('');
                $('#txtIptalNeden').val('');
                dlgIptal.close();
                listele();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ================================================================ açılış (eski Page_Load: yılbaşı - bugün)

    Sgb.al(UC + 'tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            if (t.bugun) bugun = t.bugun;
            $('#txtBasla').val(t.basla || '');
            $('#txtBitis').val(t.bitis || '');
        })
        .fail(hataGoster);

    // Tablo sayfanın kalan yüksekliğini doldurur.
    function yay() {
        var el = document.getElementById('grdCekler');
        if (!el || el.offsetParent === null) return;
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(300, Math.floor(window.innerHeight - ust - 24)) + 'px';
    }
    $(window).on('resize', yay);
    yay();
});
