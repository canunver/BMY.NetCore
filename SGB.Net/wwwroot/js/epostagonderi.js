// Önceki e-posta Gönderimleri Durum İzleme (KYM/ePostaGonderi, eski ePostaGonderi.aspx).
// Gönderilecek / Gönderilen kişi sayısına tıklanınca o kişilerin listesi Excel olarak iner (şablon KYM\EPostaGiden.xlt);
// "Gönder" bağlantısı gönderilemeyen kişilere tekrar gönderimi başlatır. Yenile listeyi yeniden okur.
$(function () {
    'use strict';

    var el = document.getElementById('grdListe');
    var d = el.dataset;

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // Sayı bağlantısı: kişi listesini indirir (gonderildiMi 0: gönderilecek, 1: gönderilen).
    function sayiBaglanti(gonderildiMi) {
        return function (p) {
            if (!p.data) return '';
            var a = document.createElement('a');
            a.href = '#';
            a.className = 'ep-bag';
            a.textContent = p.value;
            a.title = 'Kişi listesi (Excel)';
            a.addEventListener('click', function (e) {
                e.preventDefault();
                Sgb.indir('api/eposta/rapor', { mesajID: p.data.mesajID, gonderildiMi: gonderildiMi });
            });
            return a;
        };
    }

    function tekrarBaglanti(p) {
        if (!p.data) return '';
        var a = document.createElement('a');
        a.href = '#';
        a.className = 'ep-bag';
        a.textContent = d.gonder;
        a.addEventListener('click', function (e) {
            e.preventDefault();
            Sgb.gonder('api/eposta/tekrar', { mesajID: p.data.mesajID }).done(function (s) {
                if (!s.basarili) { Sgb.bildirim(s.hata, 'danger'); return; }
                Sgb.bildirim(s.bilgi, 'info');
            }).fail(hataGoster);
        });
        return a;
    }

    var tablo = Sgb.grid(el, {
        columnDefs: [
            { field: 'mesajID', headerName: d.mesajNo, width: 90, cellClass: 'text-center' },
            { field: 'konu', headerName: d.konu, flex: 1, minWidth: 200 },
            { field: 'gonderen', headerName: d.gonderen, width: 200 },
            { field: 'tarih', headerName: d.tarih, width: 150, cellClass: 'text-center' },
            { field: 'gonderilecek', headerName: d.gonderilecek, width: 130, cellClass: 'text-center', cellRenderer: sayiBaglanti(0) },
            { field: 'gonderilen', headerName: d.gonderilen, width: 130, cellClass: 'text-center', cellRenderer: sayiBaglanti(1) },
            { headerName: d.tekrar, width: 120, sortable: false, cellClass: 'text-center', cellRenderer: tekrarBaglanti }
        ],
        overlayNoRowsTemplate: '<span class="text-muted">Gönderilmiş e-posta yok</span>',
        getRowId: function (p) { return String(p.data.mesajID); }
    });

    function listele() {
        tablo.setGridOption('loading', true);
        Sgb.al('api/eposta/gonderiler').done(function (v) {
            if (v.hata) Sgb.bildirim(v.hata, 'danger');
            tablo.setGridOption('rowData', v.satirlar || []);
        }).fail(hataGoster).always(function () { tablo.setGridOption('loading', false); });
    }

    function yay() {
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(240, Math.floor(window.innerHeight - ust - 24)) + 'px';
    }

    $('#btnYenile').on('click', listele);
    $(window).on('resize', yay);
    yay();
    listele();
});
