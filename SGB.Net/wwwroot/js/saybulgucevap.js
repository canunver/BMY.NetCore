// Sayıştay Ön Bulgu Cevapları (SayistayBulgu/SayBulguCevap): eski SayBulguCevap.aspx'in betiği; sayfa yöntemleri yerine
// /api/saybulgu uçları, Bootstrap modal'ı yerine <dialog> pencere kullanılır. Birim aranarak seçilir (select2; son seçilen birim
// tarayıcıda saklanır ve açılışta listelenir), birimin ilişkili olduğu bulgulara cevap girilir, ekli dosyalar açılır, Yazdır ile
// birimin cevap listesi Excel olarak indirilir.
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

    // ---------------------------------------------------------------- son seçilen birim (eski çerez OnBulguCevap_Birim)

    var BIRIM_ANAHTAR = 'sgb.OnBulguCevap_Birim'; // { id: "...", text: "..." }

    function saveBirim() {
        var id = $('#txtBirim').val() || '';
        var s2 = $('#txtBirim').select2('data');
        var text = s2 && s2.length && s2[0].text ? s2[0].text : '';
        try { if (id) localStorage.setItem(BIRIM_ANAHTAR, JSON.stringify({ id: id, text: text })); } catch (e) { /* saklanamazsa önemli değil */ }
    }

    function loadBirim() {
        try {
            var obj = JSON.parse(localStorage.getItem(BIRIM_ANAHTAR) || 'null');
            if (obj && obj.id) {
                $('#txtBirim').append(new Option(obj.text || obj.id, obj.id, true, true)).trigger('change');
                return true;
            }
        } catch (e) { /* yut */ }
        return false;
    }

    // ---------------------------------------------------------------- birim seçimi (select2)

    $('#txtBirim').select2({
        language: 'tr',
        width: '100%',
        placeholder: 'Birim seçiniz',
        allowClear: true,
        minimumInputLength: 1,
        ajax: {
            url: Sgb.KOK + 'api/saybulgu/birimler',
            dataType: 'json',
            delay: 250,
            cache: false,
            data: function (params) { return { arama: params.term }; },
            processResults: function (v) {
                if (v && v.hata) mesaj(v.hata, 'danger');
                return { results: $.map((v && v.birimler) || [], function (b) { return { id: b.kod, text: b.kodAd }; }) };
            },
            error: function (xhr, durum) {
                if (durum === 'abort') return; // yeni harf yazılınca eski istek iptal edilir
                if (xhr && xhr.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
                hataGoster(xhr);
            }
        }
    });

    // ---------------------------------------------------------------- liste

    var bulguGrid = Sgb.grid('#bulguGrid', {
        columnDefs: [
            { headerName: '#', field: 'bulguNo', width: 100 },
            { headerName: 'Bulgu Açıklama', field: 'bulguOzeti', width: 500, tooltipField: 'bulguOzeti' },
            { headerName: 'Birimin Cevabı', field: 'birimCevap', flex: 1, minWidth: 300, tooltipField: 'birimCevap' },
            {
                headerName: '', width: 80, pinned: 'right', sortable: false, field: 'bulguID', cellClass: 'islem-hucre',
                cellRenderer: function (p) {
                    return '<button type="button" class="btn btn-primary btn-xs" data-is="cevap" data-id="' + p.value + '" title="Cevap Gir"><span class="glyphicon glyphicon-pencil" aria-hidden="true"></span><span class="sr-only">Cevap Gir</span></button>' +
                        '<button type="button" class="btn btn-primary btn-xs" data-is="dosya" data-id="' + p.value + '" title="Ekli Dosyalar"><span class="glyphicon glyphicon-file" aria-hidden="true"></span><span class="sr-only">Ekli Dosyalar</span></button>';
                }
            }
        ],
        getRowId: function (p) { return String(p.data.bulguID); },
        overlayNoRowsTemplate: '<span class="ag-overlay-no-rows-center">Gösterilecek bulgu yok</span>'
    });

    function paramOl() {
        return { yil: getInt($('#spnYil').val()), birim: $('#txtBirim').val() || '' };
    }

    function listele() {
        var o = paramOl();
        if (!o.birim) { mesaj('Birim seçiniz.', 'warning'); return; }
        saveBirim();
        var $d = $('#btnListele').prop('disabled', true);
        bulguGrid.setGridOption('loading', true);
        Sgb.al('api/saybulgu/cevaplar', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                bulguGrid.setGridOption('rowData', v.satirlar || []);
                $('#lblListe').text(o.yil + ' / ' + $('#txtBirim option:selected').text() + ': ' + (v.satirlar || []).length + ' bulgu');
            })
            .fail(hataGoster)
            .always(function () { bulguGrid.setGridOption('loading', false); $d.prop('disabled', false); });
    }

    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(); });

    function satirBul(id) {
        var n = bulguGrid.getRowNode(String(id));
        return n ? n : null;
    }

    $('#bulguGrid').on('click', 'button[data-is]', function () {
        var id = getInt($(this).data('id'));
        var n = satirBul(id);
        if (!n) { mesaj('Kayıt bulunamadı (ID: ' + id + ')', 'warning'); return; }
        if ($(this).data('is') === 'cevap') openOnBulguCevapModal(n.data);
        else openOnBulguDosyaModal(n.data);
    });

    // ---------------------------------------------------------------- cevap girişi

    var dlgCevap = document.getElementById('onBulguCevapModal');
    var cevapBirim = '';

    function readOnBulguCevapFromModal() {
        return {
            bulguID: getInt($('#ob_idGoster').val()),
            yil: getInt($('#ob_yilGoster').val()),
            birimCevap: $('#ob_birimCevap').val() || '',
            birimIsler: $('#ob_birimIsler').val() || '',
            ongorulenTamamlanmaTarihi: $('#ob_ongorulenTamamlanmaTarihiCevapDT').val() || '',
            yerineGetirildi: $('#ob_yerineGetirildiCevap').prop('checked'),
            birim: cevapBirim
        };
    }

    function openOnBulguCevapModal(o) {
        o = o || {};
        cevapBirim = $('#txtBirim').val() || ''; // listelenen birim (pencere açıkken seçim değişse de)
        $('#ob_idGoster').val(o.bulguID);
        $('#ob_yilGoster').val(o.yil);
        $('#ob_bulguNo').val(o.bulguNo || '');
        $('#ob_bulguBasligi').val(o.bulguBasligi || '');
        $('#ob_bulguOzeti').val(o.bulguOzeti || '');
        $('#ob_birimCevap').val(o.birimCevap || '');
        $('#ob_birimIsler').val(o.birimIsler || '');
        $('#ob_ongorulenTamamlanmaTarihiCevapDT').val(o.ongorulenTamamlanmaTarihiCevap || '');
        $('#ob_yerineGetirildiCevap').prop('checked', !!o.yerineGetirildiCevap);
        dlgCevap.showModal();
    }

    function OnBulguCevapKaydet() {
        var ob = readOnBulguCevapFromModal();
        var $d = $('#btnObKaydet').prop('disabled', true);
        Sgb.gonder('api/saybulgu/cevapkaydet', ob)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata || 'Kaydedilemedi.', 'danger'); return; }
                var n = satirBul(ob.bulguID);
                if (n) {
                    n.setData($.extend({}, n.data, {
                        birimCevap: ob.birimCevap,
                        birimIsler: ob.birimIsler,
                        yerineGetirildiCevap: ob.yerineGetirildi,
                        ongorulenTamamlanmaTarihiCevap: ob.ongorulenTamamlanmaTarihi
                    }));
                    bulguGrid.flashCells({ rowNodes: [n] });
                }
                mesaj('Kayıt başarılı!', 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    }

    $('#btnObKaydet').on('click', OnBulguCevapKaydet);

    // ---------------------------------------------------------------- ekli dosyalar

    function openOnBulguDosyaModal(o) {
        Sgb.pencere('../Duyuru/EkliDosyalarForm?' + $.param({ mdl: 'SBYOB', frm: 'Y' + o.yil + '_OB' + o.bulguID + '_B' + ($('#txtBirim').val() || '') }), 'Ekli Dosyalar');
    }

    // ---------------------------------------------------------------- yazdır (eski SAYRaporAl.aspx?raporNo=1&MuhasebeBirimKurum=)

    $('#btnYazdir').on('click', function () {
        var o = paramOl();
        if (!o.birim) { mesaj('Birim seçiniz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/saybulgu/rapor', { raporNo: 1, yil: o.yil, durumKod: 101, birim: o.birim })
            .finally(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açılış

    $('#spnYil').val((new Date()).getFullYear() - 1);
    if (loadBirim() && $('#txtBirim').val()) listele();
});
