// Açıklama Tanımları (ButceMuhasebe/TanimAciklama, eski TanimAciklama.aspx). Ödeme Emri / MİF formundan
// Sgb.pencere('TanimAciklama?muhasebe=..&birim=..&tip=1', 'Açıklama Listesi') ile açılır.
// Satıra tıklanınca açıklama formda değiştirilir (Kaydet / Sil / Yeni Kayıt); satırdaki + (Aktar) düğmesi açıklamayı üst sayfanın
// window.AciklamaBilgiYaz(aciklama) işlevine verir ve pencereyi kapatır (eski parent.AciklamaBilgiYaz + parent.hidePopWin).
// Uçlar: /api/tanimaciklama/* (Istemci/Muhasebe/TanimAciklamaUclari.cs).
$(function () {
    'use strict';

    var $sayfa = $('#tanimAciklama');
    if (!$sayfa.length) return;

    // Sayfanın açıldığı muhasebe / birim / tip: liste kıstası ve yeni kaydın değerleri.
    var muhasebe = parseInt($sayfa.attr('data-muhasebe'), 10) || 0;
    var birim = $sayfa.attr('data-birim') || '';
    var tip = parseInt($sayfa.attr('data-tip'), 10) || 0;

    // Üst sayfadaki açık pencereyi (Sgb.pencere) kapatır; sayfa pencerede değilse bir şey yapmaz.
    function pencereyiKapat() {
        try {
            if (window.parent && window.parent !== window) {
                var acik = window.parent.document.querySelectorAll('dialog.sgb-pencere[open]');
                if (acik.length) acik[acik.length - 1].close();
            }
        } catch (e) { /* farklı kökenli üst sayfa */ }
    }

    function aktar(satir) {
        if (!satir) return;
        try {
            if (window.parent && window.parent !== window && typeof window.parent.AciklamaBilgiYaz === 'function')
                window.parent.AciklamaBilgiYaz(satir.aciklama || '');
        } catch (e) { /* farklı kökenli üst sayfa */ }
        pencereyiKapat();
    }

    function aktarDugmesi(p) {
        if (!p.data) return null;
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-link btn-xs';
        b.title = 'Aktar';
        b.setAttribute('aria-label', 'Aktar: ' + (p.data.adi || ''));
        b.innerHTML = '<span class="glyphicon glyphicon-plus" aria-hidden="true"></span>';
        b.addEventListener('click', function (e) {
            e.stopPropagation();
            aktar(p.data);
        });
        return b;
    }

    var kolonlar = [
        { headerName: '#', width: 60, sortable: false, cellClass: 'mt-sari', valueGetter: function (p) { return p.node ? p.node.rowIndex + 1 : ''; } },
        { headerName: '', colId: 'aktar', width: 50, sortable: false, resizable: false, cellRenderer: aktarDugmesi },
        { field: 'adi', headerName: 'Adı', flex: 1, minWidth: 180, tooltipField: 'aciklama' }
    ];

    function silDurum(evet) { $('#btnSil').prop('disabled', !evet); }

    var sayfa = Sgb.mt.tanim({
        tablo: '#grdAciklama',
        form: '#frmAciklama',
        uc: 'api/tanimaciklama/',
        kolonlar: kolonlar,
        anahtar: 'kod',
        varsayilan: function () { return { kod: '', muhasebeKod: muhasebe, birimKod: birim, tipi: tip }; },
        listeParam: function () { return { muhasebe: muhasebe, birim: birim, tip: tip }; },
        acilistaListele: true,
        istek: function (f) {
            if (!window.confirm('Kayıt edilecek. Onaylıyor musunuz?')) return null;
            return {
                kod: parseInt(f.kod, 10) || 0,
                muhasebeKod: parseInt(f.muhasebeKod, 10) || 0,
                birimKod: f.birimKod,
                tipi: parseInt(f.tipi, 10) || 0,
                adi: f.adi,
                aciklama: f.aciklama
            };
        },
        silIstek: function (f) {
            var kod = parseInt(f.kod, 10) || 0;
            return kod > 0 ? { kod: kod } : null;
        },
        secilince: function (d) { silDurum((parseInt(d.kod, 10) || 0) > 0); },
        temizlenince: function () { silDurum(false); }
    });

    // Listele: liste yeniden okunur, form temizlenir (eski btnListele_Click → AciklamaAlanTemizle).
    $('#btnListele').on('click', function () {
        sayfa.temizle();
        sayfa.listele();
    });

    $('#btnKapat').on('click', pencereyiKapat);
    silDurum(false);
});
