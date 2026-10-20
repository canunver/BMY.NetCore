// Modüller (KYM/Moduller): modüllerin kullanım durumu. Uçlar: /api/moduller/liste, /kaydet (Kullanici/ModulUclari.cs).
// Durum hücresine tıklanıp seçilir; Kaydet yalnızca değişen satırları gönderir (eski getValues: store.modified).
// Kaydedince menü yeniden okunur; diğer kullanıcıların menüsü de bir sonraki sayfada yenilenir (MenuSurum).
$(function () {
    'use strict';

    var UC = 'api/moduller/';
    if (!$('.moduller').length) return;

    var DURUMLAR = { '0': 'Kullanım Dışı', '1': 'Kullanımda', '2': 'Test Amaçlı' };
    var ilkDurum = {};   // kısa ad -> okunduğundaki durum

    function hataYaz(metin) { $('#hata').text(metin || '').prop('hidden', !metin); }
    function istekHatasi(xhr) {
        if (xhr && xhr.status === 401) return;
        hataYaz(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Modüller okunamadı.');
    }

    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(enAz, Math.floor(window.innerHeight - ust - 24)) + 'px';
    }

    var tabloEl = document.getElementById('grdModul');
    var tablo = Sgb.grid(tabloEl, {
        columnDefs: [
            { field: 'ad', headerName: 'Modül', flex: 1, minWidth: 240, cellClass: 'md-salt', tooltipField: 'kisaAd' },
            {
                field: 'durum', headerName: 'Durum', width: 170, editable: true, singleClickEdit: true,
                cellEditor: 'agSelectCellEditor',
                cellEditorParams: { values: ['0', '1', '2'] },
                valueFormatter: function (p) { return DURUMLAR[p.value] || ''; },
                cellClass: function (p) {
                    var c = ['md-girdi', 'md-durum-' + (p.value || '')];
                    if (p.data && ilkDurum[p.data.kisaAd] !== p.value) c.push('md-degisti');
                    return c;
                }
            }
        ],
        getRowId: function (p) { return p.data.kisaAd; },
        suppressCellFocus: false,
        stopEditingWhenCellsLoseFocus: true,
        onCellValueChanged: function (e) {
            tablo.refreshCells({ rowNodes: [e.node], force: true });
            degisenYaz();
        }
    });

    var yay = function () { yuksekligeYay(tabloEl, 200); };
    $(window).on('resize', yay);
    yay();

    function degisenler() {
        var d = [];
        tablo.forEachNode(function (n) {
            if (ilkDurum[n.data.kisaAd] !== n.data.durum) d.push({ kisaAd: n.data.kisaAd, durum: n.data.durum });
        });
        return d;
    }

    function degisenYaz() {
        var n = degisenler().length;
        $('#lblDegisen').text(n ? n + ' modül değişti, kaydedilmedi.' : '');
        $('#btnKaydet').prop('disabled', !n);
    }

    function listele() {
        hataYaz('');
        tablo.setGridOption('loading', true);
        return Sgb.al(UC + 'liste')
            .done(function (v) {
                ilkDurum = {};
                $.each(v.satirlar || [], function (i, s) { ilkDurum[s.kisaAd] = s.durum; });
                tablo.setGridOption('rowData', v.satirlar || []);
                degisenYaz();
            })
            .fail(istekHatasi)
            .always(function () { tablo.setGridOption('loading', false); });
    }

    $('#btnKaydet').on('click', function () {
        tablo.stopEditing();
        var d = degisenler();
        if (!d.length) return;
        var $b = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'kaydet', { moduller: d })
            .done(function (v) {
                if (!v.basarili) { Sgb.bildirim(v.hata || 'Kaydedilemedi.', 'danger'); $b.prop('disabled', false); return; }
                tablo.forEachNode(function (n) { ilkDurum[n.data.kisaAd] = n.data.durum; });
                // Menü yeni modül durumlarıyla yeniden okunsun diye saklanan menü atılır ve sayfa yenilenir.
                try { localStorage.removeItem('sgb.menu'); } catch (e) { }
                try { sessionStorage.setItem('sgb.modulKaydedildi', '1'); } catch (e) { }
                window.location.reload();
            })
            .fail(function (xhr) {
                if (xhr && xhr.status === 401) return;
                Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Kaydedilemedi.', 'danger');
                $b.prop('disabled', false);
            });
    });

    $('#btnYardim').on('click', function () {
        var $p = $('#pnlIpucu');
        var ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false').toggleClass('active', ac);
        yay();
    });

    // Kaydedilmemiş değişiklikle sayfadan çıkılırken uyarılır.
    window.addEventListener('beforeunload', function (e) {
        if (degisenler().length) { e.preventDefault(); e.returnValue = ''; }
    });

    try {
        if (sessionStorage.getItem('sgb.modulKaydedildi')) {
            sessionStorage.removeItem('sgb.modulKaydedildi');
            Sgb.bildirim('Modül durumları kaydedildi, menü yenilendi.', 'success');
        }
    } catch (e) { }

    listele();
});
