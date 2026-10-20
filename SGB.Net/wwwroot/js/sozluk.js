// Sözlük Bilgileri (InsanKaynaklari/TanimSozluk): solda sözlük kayıtları (AG Grid), sağda seçilenin bilgileri.
// Eski sayfadaki gibi: anaKayitKod verilince yalnızca o bölüm listelenir ve ana kayıt o bölüm olur; listeden seçilen kayıt forma gelir
// (ana kaydı değiştirilemez), Kaydet onay sorup kaydeder, Bul listeyi yeniden okur, Temizle formu yeni kayda hazırlar.
$(function () {
    'use strict';

    if (!$('#grdSozluk').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var anaKayitKod = $('.sozluk').attr('data-ana-kayit-kod') || '';
    var tumu = [];
    var varsayilanAna = '';

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnTemizle, #btnBul').prop('disabled', evet); }

    var grid = Sgb.grid('#grdSozluk', {
        columnDefs: [
            { field: 'anaKayitAd', headerName: 'Ana Kayıt', width: 140, tooltipField: 'anaKayitAd' },
            { field: 'kayitKod', headerName: 'Kayıt Kod', width: 120 },
            { field: 'ad', headerName: 'Kayıt Adı', flex: 1, minWidth: 180, tooltipField: 'ad' },
            { field: 'kisaAd', headerName: 'Kısa Adı', width: 120, tooltipField: 'kisaAd' }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { doldur(e.data); },
        overlayNoRowsTemplate: '<span class="text-muted">Sözlük kaydı yok</span>'
    });

    function suz() {
        var ara = $.trim($('#txtAra').val()).toLocaleLowerCase('tr');
        grid.setGridOption('rowData', !ara ? tumu : $.grep(tumu, function (o) {
            return ((o.anaKayitAd || '') + ' ' + o.kayitKod + ' ' + (o.ad || '') + ' ' + (o.kisaAd || '')).toLocaleLowerCase('tr').indexOf(ara) >= 0;
        }));
    }

    // Eski AnaKayitDoldur: kök + kayıtlar (seviyesine göre girintili), değer "kod:kayitKod".
    function anaKayitlariDoldur(liste) {
        var $c = $('#cmbAna').empty();
        $.each(liste || [], function (i, a) { $('<option></option>').val(a.deger).text(a.ad).appendTo($c); });
    }

    // ilk: sayfa açılışındaki okuma; eski sayfadaki gibi liste açılışta gelir ama "Yükleniyor..." gösterilmez.
    function listele(ilk) {
        if (!ilk) grid.setGridOption('loading', true);
        var p = anaKayitKod ? { anaKayitKod: anaKayitKod } : {};
        return Sgb.al('api/sozluk/liste', p)
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                tumu = veri.satirlar || [];
                varsayilanAna = veri.varsayilanAna || '';
                anaKayitlariDoldur(veri.anaKayitlar);
                if (veri.anaAd) $('#sozlukBaslik').text('Sözlük Bilgileri: ' + veri.anaAd);
                suz();
                temizle();
                if (veri.bilgi) mesaj(veri.bilgi, 'info');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    $('#frmArama').on('submit', function (e) {
        e.preventDefault();
        listele(false);
    });
    $('#txtAra').on('input', suz);

    // Eski Temizle: form boşalır, ana kayıt yeniden seçilebilir; bölüm sayfasında ana kayıt o bölümle başlar.
    function temizle() {
        $('#frmSozluk')[0].reset();
        $('#frmSozluk').data('kod', 0);
        $('#cmbAna').prop('disabled', false).val(varsayilanAna);
        if ($('#cmbAna').val() === null) $('#cmbAna').val('');
        grid.deselectAll();
    }

    // Eski SatirSecildi: ana kayıt, kayıt kodunun son parçası atılarak bulunur ve değiştirilemez.
    function doldur(o) {
        $('#frmSozluk').data('kod', o.kod);
        $('#txtKayitKod').val(o.kayitKod || '');
        $('#txtAd').val(o.ad || '');
        $('#txtKisaAd').val(o.kisaAd || '');
        $('#txtSeviye').val(o.seviye);
        $('#txtHitapID').val(o.hitapID || '');
        var parcalar = String(o.kayitKod || '').split('.');
        parcalar.pop();
        var ana = parcalar.length ? o.anaKod + ':' + parcalar.join('.') : '';
        var $c = $('#cmbAna');
        if (ana && !$c.find('option').filter(function () { return this.value === ana; }).length)
            $('<option></option>').val(ana).text(o.anaKayitAd || parcalar.join('.')).appendTo($c);
        $c.val(ana).prop('disabled', true);
    }

    $('#btnTemizle').on('click', function () {
        temizle();
        $('#txtAd').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        var istek = {
            anaKayitKod: anaKayitKod,
            kod: parseInt($('#frmSozluk').data('kod'), 10) || 0,
            anaKayit: $('#cmbAna').val() || '',
            kayitKod: $('#txtKayitKod').val(),
            ad: $('#txtAd').val(),
            kisaAd: $('#txtKisaAd').val(),
            seviye: parseInt($('#txtSeviye').val(), 10) || 0,
            hitapID: $('#txtHitapID').val()
        };
        if ($.trim(istek.ad) === '') { mesaj('Kayıt Adı boş bırakılamaz', 'warning'); $('#txtAd').trigger('focus'); return; }
        if (!window.confirm('Kaydetmek istiyor musunuz?')) return;
        mesgul(true);
        Sgb.gonder('api/sozluk/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                listele(false);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    listele(true);
});
