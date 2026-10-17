// Sayıştay Bulguları (SayistayBulgu/SayBulgu): eski SayBulgu.aspx'in betiği; sayfa yöntemleri yerine /api/saybulgu uçları,
// Bootstrap modal'ları yerine <dialog> pencereler kullanılır. Yıl ve durumla bulgular listelenir; Yeni Bulgu / Düzenle ile bulgu
// girilir, İlişkili Birimler penceresinde birimler bağlanır, Bulgu İzin ile birimlerin cevap girişi açılıp kapatılır,
// Ekli Dosyalar ortak dosya penceresinde açılır, Liste / Cevaplı Liste Excel olarak indirilir.
$(function () {
    'use strict';

    if (!$('#bulguGrid').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function getInt(v) { var n = parseInt(v, 10); return isNaN(n) ? 0 : n; }

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- kod listeleri (eski ListeleriOlustur / GetKodListesi)

    function doldur($kutu, liste) {
        $kutu.empty();
        $.each(liste || [], function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($kutu); });
    }

    // ---------------------------------------------------------------- bulgu listesi

    var bulguGrid = Sgb.grid('#bulguGrid', {
        columnDefs: [
            { headerName: 'Yıl', field: 'yil', width: 80 },
            { headerName: 'Bulgu No', field: 'bulguNo', width: 120 },
            { headerName: 'Bulgu Başlığı', field: 'bulguBasligi', flex: 1, minWidth: 200, tooltipField: 'bulguBasligi' },
            { headerName: 'Durum', field: 'durumAd', width: 170 },
            {
                headerName: 'İşlemler', width: 130, pinned: 'right', sortable: false, cellClass: 'islem-hucre',
                cellRenderer: function (p) {
                    var id = p.data ? p.data.bulguID : 0;
                    return '<button type="button" class="btn btn-default btn-xs" data-is="duzenle" data-id="' + id + '" title="Düzenle"><span class="glyphicon glyphicon-edit" aria-hidden="true"></span><span class="sr-only">Düzenle</span></button>' +
                        '<button type="button" class="btn btn-default btn-xs" data-is="birim" data-id="' + id + '" title="İlişkili Birimler"><span class="glyphicon glyphicon-link" aria-hidden="true"></span><span class="sr-only">İlişkili Birimler</span></button>' +
                        '<button type="button" class="btn btn-default btn-xs" data-is="dosya" data-id="' + id + '" title="Ekli Dosyalar"><span class="glyphicon glyphicon-file" aria-hidden="true"></span><span class="sr-only">Ekli Dosyalar</span></button>';
                }
            }
        ],
        getRowId: function (p) { return String(p.data.bulguID); },
        overlayNoRowsTemplate: '<span class="ag-overlay-no-rows-center">Gösterilecek bulgu yok</span>'
    });

    function paramOl() {
        var durumKod = $('#sorDurum').val();
        return { yil: getInt($('#spnYil').val()), durumKod: durumKod == null || durumKod === '' ? 101 : getInt(durumKod) };
    }

    function listele() {
        var o = paramOl();
        var $d = $('#btnListele').prop('disabled', true);
        bulguGrid.setGridOption('loading', true);
        Sgb.al('api/saybulgu/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                bulguGrid.setGridOption('rowData', v.satirlar || []);
                $('#lblListe').text(o.yil + ' / ' + $('#sorDurum option:selected').text() + ': ' + (v.satirlar || []).length + ' bulgu');
            })
            .fail(hataGoster)
            .always(function () { bulguGrid.setGridOption('loading', false); $d.prop('disabled', false); });
    }

    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(); });

    function satirBul(id) {
        var n = bulguGrid.getRowNode(String(id));
        return n ? n.data : null;
    }

    $('#bulguGrid').on('click', 'button[data-is]', function () {
        var id = getInt($(this).data('id'));
        var is = $(this).data('is');
        if (is === 'duzenle') OnBulguOkuAc(id);
        else if (is === 'birim') showBirimIliskiModal(id);
        else if (is === 'dosya') openOnBulguDosyalar(id);
    });

    // ---------------------------------------------------------------- bulgu girişi (EKR004)

    var dlgBulgu = document.getElementById('onBulguModal');

    function readOnBulguFromModal() {
        return {
            bulguID: getInt($('#ob_idGoster').val()),
            yil: getInt($('#ob_yil').val()),
            bitisYil: getInt($('#ob_bitisYil').val()),
            butceTurKod: getInt($('#ob_butceTurKod').val()),
            bulguTurKod: getInt($('#ob_bulguTurKod').val()),
            bulguNo: $('#ob_bulguNo').val() || '',
            ongorulenTamamlanmaTarihi: $('#ob_ongorulenTamamlanmaTarihiDT').val() || '',
            yerineGetirildi: $('#ob_yerineGetirildi').prop('checked'),
            bulguBasligi: $('#ob_bulgu').val() || '',
            bulguOzeti: $('#ob_bulguOzeti').val() || '',
            yapilacakFaaliyetler: $('#ob_yapilacakFaaliyetler').val() || '',
            ilgiliBirimKurum: $('#ob_ilgiliBirimKurum').val() || '',
            gorevOrtagi: $('#ob_gorevOrtagi').val() || '',
            bulguDayanagi: $('#ob_bulguDayanagi').val() || '',
            idareCevabi: $('#ob_idareCevabi').val() || '',
            idareCevapOzeti: $('#ob_idareCevapOzeti').val() || '',
            sayistayOnerisi: $('#ob_sayistayOneri').val() || '',
            durumKod: getInt($('#ob_bulguDurumKod').val()) || 1 // varsayılan: Ön Taslak Bulgu
        };
    }

    function secimYap($kutu, deger) {
        $kutu.val(deger != null && deger !== '' ? String(Number(deger)) : '');
        if ($kutu.val() == null) $kutu.prop('selectedIndex', 0);
    }

    function openOnBulguModal(o) {
        o = o || {};
        $('#ob_idGoster').val(o.bulguID || '0');
        $('#onBulguModalLabel').text(o.bulguID ? 'Sayıştay Bulgu Girişi (Düzenle)' : 'Sayıştay Bulgu Girişi (Yeni)');
        $('#ob_yil').val(o.yil);
        $('#ob_bitisYil').val(o.bitisYil || '');
        secimYap($('#ob_butceTurKod'), o.butceTurKod);
        secimYap($('#ob_bulguTurKod'), o.bulguTurKod);
        secimYap($('#ob_bulguDurumKod'), o.durumKod);
        $('#ob_bulguNo').val(o.bulguNo || '');
        $('#ob_ongorulenTamamlanmaTarihiDT').val(o.ongorulenTamamlanmaTarihi || '');
        $('#ob_yerineGetirildi').prop('checked', !!o.yerineGetirildi);
        $('#ob_bulgu').val(o.bulguBasligi || '');
        $('#ob_bulguOzeti').val(o.bulguOzeti || '');
        $('#ob_yapilacakFaaliyetler').val(o.yapilacakFaaliyetler || '');
        $('#ob_ilgiliBirimKurum').val(o.ilgiliBirimKurum || '');
        $('#ob_gorevOrtagi').val(o.gorevOrtagi || '');
        $('#ob_bulguDayanagi').val(o.bulguDayanagi || '');
        $('#ob_idareCevabi').val(o.idareCevabi || '');
        $('#ob_idareCevapOzeti').val(o.idareCevapOzeti || '');
        $('#ob_sayistayOneri').val(o.sayistayOnerisi || '');
        dlgBulgu.showModal();
    }

    function yeniOnBulgu() {
        openOnBulguModal({
            bulguID: 0,
            yil: getInt($('#spnYil').val()) || (new Date()).getFullYear(),
            yerineGetirildi: false,
            durumKod: 1
        });
    }

    $('#btnYeni').on('click', yeniOnBulgu);

    function OnBulguOkuAc(id) {
        if (!id) return;
        Sgb.al('api/saybulgu/oku', { id: id })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                openOnBulguModal(v.bulgu);
            })
            .fail(hataGoster);
    }

    function OnBulguKaydet() {
        var ob = readOnBulguFromModal();
        var $d = $('#btnObKaydet').prop('disabled', true);
        Sgb.gonder('api/saybulgu/kaydet', ob)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata || 'Kaydedilemedi.', 'danger'); return; }
                if (s.anahtar) $('#ob_idGoster').val(s.anahtar);
                $('#onBulguModalLabel').text('Sayıştay Bulgu Girişi (Düzenle)');
                mesaj('Kayıt başarılı!', 'success');
                if (ob.yil === paramOl().yil) listele();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    }

    $('#btnObKaydet').on('click', OnBulguKaydet);

    // ---------------------------------------------------------------- ekli dosyalar

    function openOnBulguDosyalar(id) {
        var o = satirBul(id);
        if (!o) { mesaj('Kayıt bulunamadı (ID: ' + id + ')', 'warning'); return; }
        Sgb.pencere('../Duyuru/EkliDosyalarForm?' + $.param({ brm: 1, mdl: 'SBYOB', frm: 'Y' + o.yil + '_OB' + o.bulguID + '_B' }), 'Ekli Dosyalar');
    }

    // ---------------------------------------------------------------- bulgu - birim ilişkisi (EKR005)

    var dlgIliski = document.getElementById('mdlBirimIliski');
    var _onBulguId = 0;

    function birimDugme(sinif, metin, is) {
        return function (p) {
            if (!p.data) return '';
            return '<button type="button" class="btn ' + sinif + ' btn-xs" data-is="' + is + '" data-kod="' + $('<i></i>').text(p.data.kod).html() + '">' + metin + '</button>';
        };
    }

    var gridIliskili = Sgb.grid('#gridIliskili', {
        columnDefs: [
            { headerName: 'Kodu', field: 'kod', width: 170 },
            { headerName: 'Adı', field: 'ad', flex: 1, minWidth: 140 },
            { headerName: '', width: 80, pinned: 'right', sortable: false, cellRenderer: birimDugme('btn-danger', 'Çıkart', 'cikar') }
        ],
        overlayNoRowsTemplate: '<span class="ag-overlay-no-rows-center">İlişkilendirilmiş birim yok</span>'
    });

    var gridSonuc = Sgb.grid('#gridSonuc', {
        columnDefs: [
            { headerName: 'Kodu', field: 'kod', width: 170 },
            { headerName: 'Adı', field: 'ad', flex: 1, minWidth: 140 },
            { headerName: '', width: 70, pinned: 'right', sortable: false, cellRenderer: birimDugme('btn-primary', 'Ekle', 'ekle') }
        ],
        overlayNoRowsTemplate: '<span class="ag-overlay-no-rows-center">Birim adı yazıp Listele\'ye basınız</span>'
    });

    function loadIliskiliBirimler() {
        gridIliskili.setGridOption('loading', true);
        Sgb.al('api/saybulgu/iliskili', { bulguId: _onBulguId })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                gridIliskili.setGridOption('rowData', v.birimler || []);
            })
            .fail(hataGoster)
            .always(function () { gridIliskili.setGridOption('loading', false); });
    }

    function araBirimler() {
        gridSonuc.setGridOption('loading', true);
        Sgb.al('api/saybulgu/birimara', { ad: $('#txtBirimAdi').val() })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                gridSonuc.setGridOption('rowData', v.birimler || []);
            })
            .fail(hataGoster)
            .always(function () { gridSonuc.setGridOption('loading', false); });
    }

    function birimBag(kod, ekle) {
        Sgb.gonder('api/saybulgu/birimbag', { bulguID: _onBulguId, birim: kod, ekle: ekle })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata || 'İşlem başarısız.', 'danger'); return; }
                loadIliskiliBirimler();
            })
            .fail(hataGoster);
    }

    $('#gridIliskili, #gridSonuc').on('click', 'button[data-is]', function () {
        birimBag(String($(this).data('kod')), $(this).data('is') === 'ekle');
    });

    $('#frmBirimAra').on('submit', function (e) { e.preventDefault(); araBirimler(); });

    function showBirimIliskiModal(id) {
        _onBulguId = id || 0;
        var o = satirBul(id);
        $('#mdlBirimIliskiLbl').text('Sayıştay Bulgu İlgili Birimler' + (o && o.bulguNo ? ' (' + o.bulguNo + ')' : ''));
        $('#txtBirimAdi').val('');
        gridIliskili.setGridOption('rowData', []);
        gridSonuc.setGridOption('rowData', []);
        dlgIliski.showModal();
        loadIliskiliBirimler();
    }

    // ---------------------------------------------------------------- birim giriş izni

    var dlgIzin = document.getElementById('birimGirisModal');

    var izinGrid = Sgb.grid('#birimGirisIzni', {
        columnDefs: [
            { headerName: 'Birim Kodu', field: 'kod', width: 180 },
            { headerName: 'Birim Adı', field: 'ad', minWidth: 220, flex: 1 },
            {
                headerName: 'Veri Girişi İzni', field: 'izin', width: 150,
                cellRenderer: function (p) {
                    return p.value ? '<span class="text-success">İzni var</span>' : '<span class="text-muted">İzni yok</span>';
                }
            }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        getRowId: function (p) { return String(p.data.kod); },
        onSelectionChanged: function () {
            var n = izinGrid.getSelectedRows().length;
            $('#bg_seciliSay').text(n ? 'Seçili: ' + n : '');
        }
    });

    function BirimGirisIzinPopupAc() {
        var yil = getInt($('#spnYil').val());
        $('#bg_yil').val(yil);
        $('#birimGirisModalLbl').text('Sayıştay Bulguları Birim Giriş İzni (' + yil + ')');
        Sgb.al('api/saybulgu/izinler', { yil: yil })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                izinGrid.setGridOption('rowData', v.birimler || []);
                izinGrid.deselectAll();
                $('#bg_seciliSay').text('');
                dlgIzin.showModal();
            })
            .fail(hataGoster);
    }

    function BirimGirisIzinKaydet(izin) { // true = aç, false = kapat
        var satirlar = izinGrid.getSelectedRows();
        if (!satirlar.length) { mesaj('Lütfen en az bir birim seçiniz.', 'warning'); return; }
        var $d = $('#btnBGAc, #btnBGKapat').prop('disabled', true);
        Sgb.gonder('api/saybulgu/izinkaydet', { yil: getInt($('#bg_yil').val()), izin: !!izin, birimler: $.map(satirlar, function (r) { return r.kod; }) })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata || 'İşlem gerçekleştirilemedi.', 'danger'); return; }
                mesaj('İşlem başarılı.', 'success');
                // Tabloyu anında güncelle (izin alanı)
                izinGrid.applyTransaction({ update: $.map(satirlar, function (r) { return $.extend({}, r, { izin: !!izin }); }) });
                izinGrid.deselectAll();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    }

    $('#btnOnBulguGirisIzni').on('click', BirimGirisIzinPopupAc);
    $('#btnBGAc').on('click', function () { BirimGirisIzinKaydet(true); });
    $('#btnBGKapat').on('click', function () { BirimGirisIzinKaydet(false); });

    // ---------------------------------------------------------------- yazdır (eski SAYRaporAl.aspx)

    function yazdir(raporNo) {
        var o = paramOl();
        o.raporNo = raporNo;
        var $d = $('#btnYazdir1, #btnYazdir2').prop('disabled', true);
        Sgb.indir('api/saybulgu/rapor', o).finally(function () { $d.prop('disabled', false); });
    }

    $('#btnYazdir1').on('click', function () { yazdir(1); });
    $('#btnYazdir2').on('click', function () { yazdir(2); });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/saybulgu/kodlar')
        .done(function (v) {
            $('#spnYil').val(v.yil);
            doldur($('#ob_butceTurKod'), v.butceTurleri);
            doldur($('#ob_bulguTurKod'), v.bulguTurleri);
            doldur($('#ob_bulguDurumKod'), v.durumlar);
            doldur($('#sorDurum'), v.sorguDurumlari);
            listele(); // eski sayfada da açılışta listelenirdi
        })
        .fail(hataGoster);
});
