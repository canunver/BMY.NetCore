// Firma / Kişi Tanımlama, HarcamaSurecleri/TanimFirmaKisi (eski aynı adlı Ext1.NET sayfası). Solda arama ve liste, sağda seçilen
// ya da yeni firmanın bilgileri. Firma Kişi Listesi'nin Yeni tuşuyla pencerede açılınca kayıttan sonra o listeye numarayı yazıp
// arar ve pencereyi kapatır (eski SecveKapat).
// Uçlar: /api/firmatanim/* (HarcamaSurecleri/FirmaTanimUclari.cs); muhasebe adı ve seçimi /api/muhbirim/* (OrtakSayfa/MuhasebeBirimUclari.cs).
$(function () {
    'use strict';

    var $kok = $('.firma-tanim');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/firmatanim/';
    var GELIRLER = String($kok.data('gelirler')) === '1';
    var q = new URLSearchParams(window.location.search);

    // Eski hdnKod / hdnVNo / hdnAd / hdnVD / hdnAdres
    var kayit = { kod: 0, eskiVergiNo: '', gibAd: '', gibVergiDairesi: '', gibAdres: '' };

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function deger(id) { return $.trim($('#' + id).val()); }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });

    // ---------------------------------------------------------------- muhasebe birimi

    function muhasebeAdOku() {
        var m = parseInt(deger('txtMuhasebe'), 10) || 0;
        if (!m) { $('#lblMuhasebeAd').text(''); return; }
        Sgb.al('api/muhbirim/bilgi', { muhasebe: m, birim: '' })
            .done(function (v) { $('#lblMuhasebeAd').text(v.hata ? '' : (v.birimAd || '')); })
            .fail(function () { $('#lblMuhasebeAd').text(''); });
    }
    $('#txtMuhasebe').on('change', muhasebeAdOku);

    // Eski MuhasebeGoster: kullanıcının yetkili olduğu muhasebe birimleri listesi.
    var muhPencere = null;
    $('#btnMuhasebeSec').on('click', function () {
        if (!muhPencere) {
            var $p = $('<dialog class="tanim-pencere" aria-labelledby="dlgMuhBaslik"></dialog>')
                .append('<div class="pencere-baslik"><h2 id="dlgMuhBaslik">Muhasebe Listesi</h2>'
                    + '<button type="button" class="close" data-kapat aria-label="Kapat"><span aria-hidden="true">&times;</span></button></div>')
                .append('<div class="pencere-govde"><p class="help-block small">Seçmek için muhasebe biriminin adına tıklayınız.</p><ul class="deger-liste"></ul></div>')
                .append('<div class="pencere-alt"><button type="button" class="btn btn-default btn-sm" data-kapat>Kapat</button></div>')
                .appendTo(document.body);
            $p.on('click', '[data-kapat]', function () { $p[0].close(); });
            muhPencere = $p;
            Sgb.agac($p.find('ul')[0], {
                duz: true,
                yukle: function () {
                    return Sgb.al('api/muhbirim/agac', { ust: '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    $p[0].close();
                    $('#txtMuhasebe').val(d.kod);
                    $('#lblMuhasebeAd').text(String(d.ad || '').replace(/^\d+-/, ''));
                },
                bosMetin: 'Muhasebe birimi bulunamadı.',
                hata: hataGoster
            });
        }
        muhPencere[0].showModal();
    });

    // ---------------------------------------------------------------- liste

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'vno', headerName: 'Kimlik/Vergi No', width: 120 },
            { field: 'ad', headerName: 'Adı Soyadı/Üvanı', flex: 1, minWidth: 200, tooltipField: 'ad' },
            { field: 'tel', headerName: 'Telefon', width: 125 },
            { field: 'faks', headerName: 'Faks', width: 125 },
            { field: 'vd', headerName: 'Vergi Dairesi', width: 125 }
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        overlayNoRowsTemplate: '<span>Kimlik/vergi no ya da ad yazıp Bul\'a basınız.</span>',
        onRowClicked: function (p) { satirSecildi(p.data); }
    });

    // Eski Listele: tek kayıt dönerse form doldurulur.
    function listele(kriter, sessiz) {
        var $d = $('#btnAra').prop('disabled', true);
        grid.setGridOption('loading', true);
        return Sgb.al(UC + 'liste', kriter)
            .done(function (v) {
                if (v.hata) { if (!sessiz) mesaj(v.hata, 'warning'); return; }
                var s = v.satirlar || [];
                grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                grid.setGridOption('rowData', s);
                if (s.length === 1) formDoldur(s[0]);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); $d.prop('disabled', false); yay(); });
    }

    function aramaKriter() { return { muhasebe: deger('txtMuhasebe'), ad: $('#txtAraAd').val(), vergiNo: $('#txtAraVNo').val() }; }

    // Eski btnAra_Click
    $('#btnAra').on('click', function () {
        var k = aramaKriter();
        if (!(parseInt(k.muhasebe, 10) || 0)) { mesaj('Muhasebe kodu boş bırakılamaz.', 'warning'); return; }
        if (!$.trim(k.ad) && !$.trim(k.vergiNo)) {
            mesaj('Lütfen ad veya vergi numarası alanlarından birini doldurunuz. Tüm firmaları listelemek için ad alanına % giriniz.', 'warning');
            return;
        }
        listele(k, false);
    });
    $('#frmAra input').on('keydown', function (e) { if (e.key === 'Enter' && this.id !== 'txtMuhasebe') { e.preventDefault(); $('#btnAra').trigger('click'); } });

    // Eski btnYaz_Click: listeyi Excel olarak verir.
    $('#btnYaz').on('click', function () { Sgb.indir(UC + 'yazdir', aramaKriter()); });

    // ---------------------------------------------------------------- form

    // Eski AlanlariDuzenle: GİB bağlantısı varken "GİB'ten veri al" seçiliyse ad, adres ve vergi dairesi elle yazılmaz.
    function alanlariDuzenle() {
        var turk = $('#rbTurk').prop('checked');
        $('.ft-gib').prop('disabled', turk ? GELIRLER : false);
    }
    $('input[name=rbUyruk]').on('change', alanlariDuzenle);

    function yaz(id, d) { $('#' + id).val(d === undefined || d === null ? '' : d); }

    // Eski FormDoldur / SatirSecildi
    function formDoldur(r) {
        kayit = { kod: parseInt(r.kod, 10) || 0, eskiVergiNo: r.vno || '', gibAd: r.ad || '', gibVergiDairesi: r.vd || '', gibAdres: r.adres || '' };
        yaz('txtVNo', r.vno); yaz('txtVD', r.vd); yaz('txtAd', r.ad); yaz('txtAdres', r.adres); yaz('txtAdresNo', r.adresNo);
        yaz('txtBanka', r.banka); yaz('txtHesapNo', r.hesapNo); yaz('txtYetkili', r.firmaYetkilisi); yaz('txtEPosta', r.eMail);
        yaz('txtTelefon', r.tel); yaz('txtFaks', r.faks); yaz('txtCepTel', r.cepTel); yaz('txtWeb', r.web); yaz('txtAciklama', r.aciklama);
        yaz('txtBICSWIFT', r.bicSwift); yaz('txtKisiUnvan', r.unvan); yaz('txtKisiDerece', r.derece); yaz('txtKisiEkGosterge', r.ekGosterge);
        $(String(r.uyruk) === '1' ? '#rbYabanci' : '#rbTurk').prop('checked', true);
        $('#chkKaraListe').prop('checked', String(r.karaListe) === '1');
        $('#chkGorulmesin').prop('checked', String(r.gorulmesin) === '1');
        $('#chkYasakli').prop('checked', String(r.yasakli) === '1');
        $('#chkFatura').prop('checked', String(r.fatura) === '1');
        alanlariDuzenle();
    }

    function satirSecildi(r) {
        if (!r) return;
        formDoldur(r);
        if (r.muhasebe) { $('#txtMuhasebe').val(r.muhasebe); muhasebeAdOku(); }
    }

    // Eski Temizle
    function temizle() {
        kayit = { kod: 0, eskiVergiNo: '', gibAd: '', gibVergiDairesi: '', gibAdres: '' };
        $('#frmFirma').find('input[type=text], textarea').val('');
        $('#rbTurk').prop('checked', true);
        $('#frmFirma').find('input[type=checkbox]').prop('checked', false);
        grid.deselectAll();
        alanlariDuzenle();
    }
    $('#btnTemizle').on('click', function () { temizle(); });

    // Eski imgKimlik_Click (vergi no yanındaki mercek)
    $('#btnKimlik').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.al(UC + 'kimlik', { muhasebe: deger('txtMuhasebe'), araAd: $('#txtAraAd').val(), vergiNo: deger('txtVNo') })
            .done(function (v) {
                if (v.rastgele) { $('#txtVNo').val(v.rastgele); return; }
                if (v.temizle) { var no = $('#txtVNo').val(); temizle(); $('#txtVNo').val(no); }
                if (v.hata) { mesaj(v.hata, 'warning'); return; }
                if (v.kayit) formDoldur(v.kayit);
                if (v.gib) {
                    kayit.gibAd = v.gib.ad || ''; kayit.gibVergiDairesi = v.gib.vd || ''; kayit.gibAdres = v.gib.adres || '';
                    yaz('txtAd', v.gib.ad); yaz('txtVD', v.gib.vd); yaz('txtAdres', v.gib.adres);
                }
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // Eski SecveKapat: Firma Kişi Listesi penceresinden açıldıysa o listede bu numarayla arar ve pencereyi kapatır.
    function secVeKapat(vergiNo) {
        try {
            if (!window.frameElement || window.parent === window) return;
            var ud = window.parent.document;
            var vno = ud.getElementById('txtVNo'), bul = ud.getElementById('btnBul');
            if (vno && bul) { vno.value = vergiNo; bul.click(); }
            var d = window.frameElement.closest('dialog');
            if (d) d.close();
        } catch (e) { /* başka kökenden açıldıysa dokunulmaz */ }
    }

    // Eski btnKaydet_Click
    $('#btnKaydet').on('click', function () {
        var veri = $.extend({}, kayit, {
            muhasebe: deger('txtMuhasebe'),
            vergiNo: $('#txtVNo').val(), vergiDairesi: $('#txtVD').val(), ad: $('#txtAd').val(), adres: $('#txtAdres').val(),
            adresNo: $('#txtAdresNo').val(), uyruk: $('#rbYabanci').prop('checked') ? 1 : 0,
            karaListe: $('#chkKaraListe').prop('checked'), yasakli: $('#chkYasakli').prop('checked'),
            gorulmesin: $('#chkGorulmesin').prop('checked'), fatura: $('#chkFatura').prop('checked'),
            banka: $('#txtBanka').val(), firmaYetkilisi: $('#txtYetkili').val(), aciklama: $('#txtAciklama').val(),
            cepTel: $('#txtCepTel').val(), telefon: $('#txtTelefon').val(), hesapNo: $('#txtHesapNo').val(), eMail: $('#txtEPosta').val(),
            web: $('#txtWeb').val(), faks: $('#txtFaks').val(), bicSwift: $('#txtBICSWIFT').val(), kisiUnvan: $('#txtKisiUnvan').val(),
            derece: $('#txtKisiDerece').val(), ekGosterge: $('#txtKisiEkGosterge').val()
        });
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'kaydet', veri)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata || 'Kayıt yapılamadı.', 'danger'); return; }
                mesaj('Kayıt işlemi tamamlandı.', 'success');
                temizle();
                listele({ muhasebe: veri.muhasebe, ad: '', vergiNo: v.vergiNo }, true);
                secVeKapat(v.vergiNo);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // Tablo sol bölümün kalan yüksekliğini doldurur.
    function yay() {
        var el = document.getElementById('grdListe');
        if (!el || el.offsetParent === null) return;
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(220, Math.floor(window.innerHeight - ust - 30)) + 'px';
        var sag = document.querySelector('.firma-tanim .ft-sag');
        if (sag && window.innerWidth > 900) sag.style.maxHeight = Math.max(300, Math.floor(window.innerHeight - sag.getBoundingClientRect().top - 12)) + 'px';
    }
    $(window).on('resize', yay);

    // Eski Page_Load: ?yeniFirma= ile açılınca numara forma yazılır.
    if (q.get('yeniFirma')) $('#txtVNo').val(q.get('yeniFirma'));
    if (deger('txtMuhasebe') && !$('#lblMuhasebeAd').text()) muhasebeAdOku();
    alanlariDuzenle();
    yay();
    $('#txtAraVNo').trigger('focus');
});
