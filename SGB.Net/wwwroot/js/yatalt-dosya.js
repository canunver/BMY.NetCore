// Proje dosyaları (ButceHazirlikv2/ProjeDosyaEkle): ProjeDetay / AltProjeDetay sekmesinde açılır. Projeye eklenen dosyaların
// listesi; her satırda İndir ve Sil (onay sorulur); Yeni Dosya Ekle penceresi (açıklama, sıra, dosya) dosyayı multipart POST ile
// yükler. Ölçüt sayfa adresinden gelir: yil, senaryoKod, projeKodu, altProjeKodu.
$(function () {
    'use strict';

    if (!$('#grdDosya').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var adres = new URLSearchParams(location.search);
    var olcut = {
        yil: parseInt(adres.get('yil'), 10) || 0,
        senaryoKod: adres.get('senaryoKod') || '',
        projeKodu: parseInt(adres.get('projeKodu'), 10) || 0,
        altProjeKodu: parseInt(adres.get('altProjeKodu'), 10) || 0
    };
    var azamiMB = 20;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;

    function sor(metin, evet, is) {
        $('#soruMetin').text(metin);
        $('#btnSoruEvet').text(evet);
        soruIsi = is;
        dlgSoru.showModal();
    }

    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        if (soruIsi) soruIsi();
        soruIsi = null;
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- liste

    var grid = Sgb.grid('#grdDosya', {
        columnDefs: [
            { colId: 'satir', headerName: '', width: 50, sortable: false, resizable: false, valueGetter: function (p) { return p.node.rowIndex + 1; } },
            { field: 'siraNo', headerName: '#', width: 60 },
            { field: 'aciklama', headerName: 'Açıklama', flex: 1, minWidth: 200, tooltipField: 'aciklama' },
            { field: 'dosyaAd', headerName: 'Dosya Ad', width: 300, tooltipField: 'dosyaAd' },
            { field: 'tur', headerName: 'Tür', width: 70 },
            { colId: 'islem', headerName: 'İşlem', width: 150, sortable: false, resizable: false, cellClass: 'islem',
                cellRenderer: function (p) {
                    if (!p.data) return '';
                    return '<button type="button" class="btn btn-default btn-xs" data-komut="indir"><span class="glyphicon glyphicon-download-alt" aria-hidden="true"></span> İndir</button>'
                        + '<button type="button" class="btn btn-default btn-xs" data-komut="sil"><span class="glyphicon glyphicon-trash" aria-hidden="true"></span> Sil</button>';
                } }
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        overlayNoRowsTemplate: '<span>Projeye eklenmiş dosya yok.</span>',
        // İndir / Sil (eski KomutCalistir).
        onCellClicked: function (e) {
            var $b = $(e.event && e.event.target).closest('[data-komut]');
            if (!$b.length || !e.data) return;
            if ($b.attr('data-komut') === 'indir') indir(e.data, $b);
            else sil(e.data);
        }
    });

    // Liste (eski DosyaIslemleri).
    function oku() {
        grid.setGridOption('loading', true);
        Sgb.al('api/yatalt/dosya/liste', olcut)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                if (v.azamiMB) azamiMB = v.azamiMB;
                grid.setGridOption('rowData', v.satirlar);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    // İndir (eski DosyaIndir): dosya kayıttaki adıyla iner.
    function indir(d, $b) {
        $b.prop('disabled', true);
        Sgb.indir('api/yatalt/dosya/indir', $.extend({}, olcut, { fizikiDosyaAd: d.fizikiDosyaAd }))
            .then(function () { $b.prop('disabled', false); });
    }

    // Sil (eski BelgeSil): kayıt ve dosyası silinir.
    function sil(d) {
        sor('"' + (d.dosyaAd || d.aciklama || '') + '" dosyası kalıcı olarak silinecektir, emin misiniz?', 'Sil', function () {
            Sgb.gonder('api/yatalt/dosya/sil', $.extend({}, olcut, { fizikiDosyaAd: d.fizikiDosyaAd }))
                .done(function (s) {
                    mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                    if (s.basarili) oku();
                })
                .fail(hataGoster);
        });
    }

    // ---------------------------------------------------------------- Yeni Dosya Ekle (eski winFileUpload / BelgeKaydet)

    var dlgDosyaEkle = document.getElementById('dlgDosyaEkle');

    $('#btnDosyaEkle').on('click', function () {
        $('#dosyaNot').text('Dosya en çok ' + azamiMB + ' MB olabilir.');
        dlgDosyaEkle.showModal();
        $('#txtAciklama').trigger('focus');
    });

    $('#frmDosyaEkle').on('submit', function (olay) {
        olay.preventDefault();
        var aciklama = $.trim($('#txtAciklama').val());
        var dosya = $('#filDosya')[0].files[0];
        if (!aciklama) { mesaj('Açıklama boş olamaz', 'warning'); $('#txtAciklama').trigger('focus'); return; }
        if (!dosya) { mesaj('Aç tuşuna basıp dosya seçiniz.', 'warning'); return; }
        if (dosya.size > azamiMB * 1024 * 1024) { mesaj('Dosya en çok ' + azamiMB + ' MB olabilir.', 'warning'); return; }

        var veri = new FormData();
        $.each(olcut, function (ad, deger) { veri.append(ad, deger); });
        veri.append('aciklama', aciklama);
        veri.append('siraNo', $.trim($('#txtSira').val()));
        veri.append('dosya', dosya);

        var $d = $('#btnSakla').prop('disabled', true);
        $.ajax({
            url: Sgb.KOK + 'api/yatalt/dosya/yukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (s) {
            mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
            if (s.basarili) {
                $('#frmDosyaEkle')[0].reset();
                dlgDosyaEkle.close();
                oku();
            }
        }).fail(function (xhr) {
            if (xhr && xhr.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            hataGoster(xhr);
        }).always(function () { $d.prop('disabled', false); });
    });

    oku();
});
