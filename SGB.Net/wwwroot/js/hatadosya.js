// Sistem Durum Dosya Listesi (KYM/HataDosya): hata dosyası klasöründeki dosyalar.
// Uçlar: /api/hatadosya/liste, /ac?ad=, /sil (Kullanici/HataDosyaUclari.cs).
// Varsayılan sıra tarihe göre (en yeni üstte); sütun başlığına tıklanarak büyüklüğe ya da ada göre de sıralanır.
$(function () {
    'use strict';

    var UC = 'api/hatadosya/';
    var $sayfa = $('.hata-dosya');
    if (!$sayfa.length) return;
    var bosMetin = $sayfa.data('bos');

    function hataYaz(metin) { $('#hata').text(metin || '').prop('hidden', !metin); }
    function istekHatasi(xhr) {
        if (xhr && xhr.status === 401) return;
        hataYaz(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Dosya listesi okunamadı.');
    }

    // Tabloyu sayfanın kalan yüksekliğine yayar (kaydırma tablonun içinde kalır).
    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(enAz, Math.floor(window.innerHeight - ust - 24)) + 'px';
    }

    var tabloEl = document.getElementById('grdDosya');
    var tablo = Sgb.grid(tabloEl, {
        columnDefs: [
            {
                headerName: '', colId: 'sil', width: 52, sortable: false, resizable: false,
                cellRenderer: function (p) {
                    var b = document.createElement('button');
                    b.type = 'button';
                    b.className = 'hd-sil';
                    b.title = 'Sil';
                    b.setAttribute('aria-label', p.data.ad + ' dosyasını sil');
                    b.innerHTML = '<span class="glyphicon glyphicon-trash" aria-hidden="true"></span>';
                    b.addEventListener('click', function () { sil(p.data); });
                    return b;
                }
            },
            {
                field: 'tarih', headerName: 'Tarih', width: 190, sort: 'desc', sortingOrder: ['desc', 'asc'],
                valueFormatter: function (p) { return p.data ? p.data.tarihStr : ''; }
            },
            {
                field: 'buyukluk', headerName: 'Büyüklük', width: 130, type: 'rightAligned', sortingOrder: ['desc', 'asc'],
                valueFormatter: function (p) { return p.data ? p.data.buyuklukStr : ''; }
            },
            {
                field: 'ad', headerName: 'Dosya Adı', flex: 1, minWidth: 240, tooltipField: 'ad', sortingOrder: ['asc', 'desc'],
                comparator: function (a, b) { return (a || '').localeCompare(b || '', 'tr', { sensitivity: 'base', numeric: true }); },
                cellRenderer: function (p) {
                    var a = document.createElement('a');
                    a.className = 'hd-ad';
                    a.href = Sgb.KOK + UC + 'ac?ad=' + encodeURIComponent(p.value);
                    a.target = '_blank';
                    a.rel = 'noopener';
                    a.textContent = p.value;
                    return a;
                }
            }
        ],
        getRowId: function (p) { return p.data.ad; },
        tooltipShowDelay: 400,
        overlayNoRowsTemplate: '<span class="text-muted"></span>'
    });
    tablo.setGridOption('overlayNoRowsTemplate', $('<span class="text-muted"></span>').text(bosMetin).prop('outerHTML'));

    var yay = function () { yuksekligeYay(tabloEl, 200); };
    $(window).on('resize', yay);
    yay();

    function adetYaz() {
        var n = 0;
        tablo.forEachNode(function () { n++; });
        $('#lblAdet').text(n ? n + ' dosya' : '');
    }

    function listele() {
        var $b = $('#btnYenile').prop('disabled', true);
        hataYaz('');
        tablo.setGridOption('loading', true);
        return Sgb.al(UC + 'liste')
            .done(function (v) {
                tablo.setGridOption('rowData', v.satirlar || []);
                adetYaz();
            })
            .fail(istekHatasi)
            .always(function () { tablo.setGridOption('loading', false); $b.prop('disabled', false); });
    }

    function sil(satir) {
        if (!window.confirm('"' + satir.ad + '" dosyası silinsin mi?')) return;
        Sgb.gonder(UC + 'sil', { ad: satir.ad })
            .done(function (v) {
                if (!v.basarili) { Sgb.bildirim(v.hata || 'Dosya silinemedi.', 'danger'); return; }
                // Eski sayfa gibi silmeden sonra liste yeniden okunur (dosya kullanımdaysa silinmemiş olabilir).
                listele().done(function () {
                    if (tablo.getRowNode(satir.ad)) Sgb.bildirim('Dosya silinemedi (kullanımda olabilir).', 'warning');
                    else Sgb.bildirim('Dosya silindi.', 'success');
                });
            })
            .fail(function (xhr) {
                if (xhr && xhr.status === 401) return;
                Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Dosya silinemedi.', 'danger');
            });
    }

    $('#btnYenile').on('click', listele);
    // Eski sayfa da açılışta listeliyordu.
    listele();
});
