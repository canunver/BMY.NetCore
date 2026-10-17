// Faiz Türü Seçimi (AlacakTakip/ListeFaizTur, eski ListeFaizTur.aspx?listeTur=faizTur):
//   1) Sayfa olarak: Alacak Takip ekranları Sgb.pencere('../AlacakTakip/ListeFaizTur?yil=2026', 'Faiz Türü') ile açar. Açılışta seçili
//      yılın faiz türleri listelenir (eski sayfadaki gibi); tıklanan faiz türü açan sayfaya { tur: 'faizTurSecildi', kod, ad, yil, oran }
//      iletisiyle (postMessage) bildirilir ve pencere kapanır (eski FaizTurYaz + hidePopWin).
//   2) Kolay kullanım: açan sayfada bu dosya da yüklüyse
//        Sgb.faizTurSec({ yil: 2026, secilince: function (f) { f.kod, f.ad, f.yil, f.oran } });
//      pencereyi açar ve seçimi secilince'ye verir.
(function (Sgb, $) {
    'use strict';

    // ---------------------------------------------------------------- açan sayfa tarafı

    Sgb.faizTurSec = function (o) {
        o = o || {};
        function dinle(e) {
            if (e.origin !== window.location.origin || !e.data || e.data.tur !== 'faizTurSecildi') return;
            window.removeEventListener('message', dinle);
            if (o.secilince) o.secilince(e.data);
        }
        window.addEventListener('message', dinle);
        Sgb.pencere('../AlacakTakip/ListeFaizTur' + (o.yil ? '?yil=' + encodeURIComponent(o.yil) : ''), o.baslik || 'Faiz Türü Seçimi',
            function () { window.removeEventListener('message', dinle); });
    };

    // ---------------------------------------------------------------- seçim sayfası tarafı

    $(function () {
        if (!$('#grdFaizTur').length || !$('#frmListe').length) return; // seçim sayfası değil ya da yetkisiz

        var oranBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

        function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
        function hataGoster(xhr) {
            if (xhr && xhr.status === 401) return;
            mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
        }

        function sec(f) {
            var ileti = { tur: 'faizTurSecildi', kod: f.kod, ad: f.ad, yil: f.yil, oran: f.oran };
            var hedef = window.parent !== window ? window.parent : window.opener;
            if (!hedef) return;
            hedef.postMessage(ileti, window.location.origin);
            // Çerçeve (Sgb.pencere) içindeyse pencereyi kapat, ayrı pencereyse kendini kapat.
            try {
                var d = window.frameElement && window.frameElement.closest('dialog');
                if (d) { d.close(); return; }
            } catch (e) { /* başka kökenden açılmış */ }
            if (window.opener) window.close();
        }

        var grid = Sgb.grid('#grdFaizTur', {
            columnDefs: [
                { field: 'ad', headerName: 'Oran Adı', flex: 1, minWidth: 220, tooltipField: 'ad',
                    cellRenderer: function (p) { var a = document.createElement('a'); a.href = '#'; a.textContent = p.value || ''; return a; } },
                { field: 'oran', headerName: 'Oran', width: 130, type: 'rightAligned',
                    valueFormatter: function (p) { return p.value == null ? '' : oranBicim.format(p.value); } }
            ],
            getRowId: function (p) { return String(p.data.kod); },
            overlayNoRowsTemplate: '<span>Yılı seçip Ara\'ya basınız.</span>',
            onRowClicked: function (e) {
                if (e.event) e.event.preventDefault();
                sec(e.data);
            }
        });

        function ara() {
            var yil = parseInt($('#txtYil').val(), 10) || 0;
            if (!yil) { mesaj('Yıl bilgisi girilmemiş!', 'warning'); return; }
            grid.setGridOption('loading', true);
            Sgb.al('api/alacaktakip/faiztur/liste', { yil: yil, ad: '' })
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); grid.setGridOption('rowData', []); return; }
                    grid.setGridOption('rowData', v.satirlar || []);
                    if (v.bilgi) mesaj(v.bilgi, 'info');
                })
                .fail(hataGoster)
                .always(function () { grid.setGridOption('loading', false); });
        }

        $('#frmListe').on('submit', function (e) { e.preventDefault(); ara(); });

        var istenenYil = parseInt(new URLSearchParams(window.location.search).get('yil'), 10) || 0;
        Sgb.al('api/alacaktakip/faiztur/tanim')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                Sgb.yilKutusu('#txtYil', v.yillar, istenenYil || v.aktifYil);
                ara(); // eski sayfa açılışta listeliyordu
            })
            .fail(hataGoster);
    });
})(window.Sgb, jQuery);
