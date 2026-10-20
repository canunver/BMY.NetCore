// Personel İşlemleri "Tarihçe" sekmesi (InsanKaynaklari/PersonelTarihce; eski PersonelTarihce.aspx). Salt okunur liste.
// Eski sayfa listeyi kategoriye göre (azalan) gruplayıp grup başlığında kayıt sayısını gösteriyordu; burada liste kategoriye göre
// sıralı gelir, üstteki Kategori seçimi "Kategori (sayı)" biçiminde listeyi o kategoriye süzer. Sayfa başına 500 satır (eski PageSize).
$(function () {
    'use strict';

    var $sayfa = $('#ptrSayfa');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var personelKod = parseInt($sayfa.data('personel-kod'), 10) || 0;
    var tumu = [];

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Tarihçe okunamadı. Lütfen tekrar deneyiniz.', 'danger');
    }

    function tarihSaat(d) {
        var p = /^(\d{4})-(\d{2})-(\d{2})(?:[ T](\d{2}:\d{2}:\d{2}))?/.exec(d || '');
        return p ? p[3] + '.' + p[2] + '.' + p[1] + (p[4] ? ' ' + p[4] : '') : (d || '');
    }

    var grid = Sgb.grid('#grdTarihce', {
        columnDefs: [
            { field: 'TARIH', headerName: 'Tarih', width: 150, cellClass: 'hucre-salt', valueFormatter: function (p) { return tarihSaat(p.value); } },
            { field: 'DEGISIKLIKYAPAN', headerName: 'Değişikliği Yapan', width: 150, cellClass: 'hucre-salt', tooltipField: 'DEGISIKLIKYAPAN' },
            { field: 'KATEGORI', headerName: 'Kategori', width: 130, cellClass: 'hucre-salt', sort: 'desc' },
            { field: 'ACIKLAMA2', headerName: 'Açıklama 2', width: 180, cellClass: 'hucre-salt', tooltipField: 'ACIKLAMA2' },
            { field: 'ACIKLAMA1', headerName: 'Açıklama 1', width: 180, cellClass: 'hucre-salt', tooltipField: 'ACIKLAMA1' },
            { field: 'DEGISENALAN', headerName: 'Değişen Alan', flex: 1, minWidth: 180, cellClass: ['hucre-salt', 'deger-alan'], tooltipField: 'DEGISENALAN' },
            { field: 'YENIDEGER', headerName: 'Yeni Değer', width: 200, cellClass: ['hucre-salt', 'deger-yeni'], tooltipField: 'YENIDEGER' },
            { field: 'ESKIDEGER', headerName: 'Eski Değer', width: 200, cellClass: ['hucre-salt', 'deger-eski'], tooltipField: 'ESKIDEGER' }
        ],
        pagination: true,
        paginationPageSize: 500,
        paginationPageSizeSelector: false,
        overlayNoRowsTemplate: '<span class="text-muted">Tarihçe kaydı yok</span>'
    });

    function sayiYaz(n) { $('#lblSayi').text(n + ' kayıt'); }

    function suz() {
        var k = $('#ddlKategori').val();
        var satirlar = k ? $.grep(tumu, function (s) { return s.KATEGORI === k; }) : tumu;
        grid.setGridOption('rowData', satirlar);
        sayiYaz(satirlar.length);
    }

    function kategorileriDoldur() {
        var sayilar = {}, adlar = [];
        $.each(tumu, function (i, s) {
            if (!(s.KATEGORI in sayilar)) { sayilar[s.KATEGORI] = 0; adlar.push(s.KATEGORI); }
            sayilar[s.KATEGORI]++;
        });
        adlar.sort(function (a, b) { return b.localeCompare(a, 'tr'); });
        var $k = $('#ddlKategori').empty();
        $('<option value=""></option>').text('Tümü (' + tumu.length + ')').appendTo($k);
        $.each(adlar, function (i, a) { $('<option></option>').val(a).text(a + ' (' + sayilar[a] + ')').appendTo($k); });
    }

    $('#ddlKategori').on('change', suz);

    // Eski Listele (açılışta "Yükleniyor..." gösterilmez).
    if (personelKod > 0) {
        Sgb.al('api/personelbilgi/tarihce/liste', { personelKod: personelKod })
            .done(function (veri) {
                if (veri.hata) mesaj(veri.hata, 'danger');
                tumu = veri.satirlar || [];
                kategorileriDoldur();
                suz();
            })
            .fail(hataGoster);
    } else {
        sayiYaz(0);
    }
});
