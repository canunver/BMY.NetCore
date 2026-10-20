// Personel İşlemleri > Giderler sekmesi (InsanKaynaklari/PersonelGider; eski PersonelGider.aspx).
// Sağda seçili yıl / aydaki giderler; seçilen gider forma gelir ve "Değiştir" ile yalnızca tutarı değiştirilir (yer, tür, tarih
// kilitlenir). "Yeni Kayıt" formu "Ekle" durumuna döndürür. "Küm. Yükle" Excel dosyasından toplu yükler, "Aktar" seçili yıl / ay / tür
// verilerini yeniden aktarır.
$(function () {
    'use strict';

    var $kok = $('#pbGider');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var personelKod = parseInt($kok.data('personel-kod'), 10) || -1;
    var yeni = true;         // eski hdnYeni
    var yer = { kod: 0, ad: '' }; // eski ddlYerleskeKod (değer, metin)
    var tutarBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function mesgul(evet) { $kok.find('.pb-arac button').prop('disabled', evet); }

    function onayla(metin) {
        var d = $.Deferred();
        var dlg = document.getElementById('dlgOnay');
        $('#dlgOnayMetin').text(metin);
        $(dlg).off('click.onay').on('click.onay', '[data-sonuc]', function () {
            var evet = $(this).data('sonuc') === 'evet';
            dlg.close();
            if (evet) d.resolve(); else d.reject();
        });
        $(dlg).off('cancel.onay').on('cancel.onay', function () { d.reject(); });
        dlg.showModal();
        return d.promise();
    }

    function zaman(deger) {
        var p = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})(?::(\d{2}))?/.exec(deger || '');
        return p ? p[3] + '.' + p[2] + '.' + p[1] + ' ' + p[4] + ':' + p[5] + ':' + (p[6] || '00') : (deger || '');
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'tarihDT', headerName: 'Zaman', width: 160, cellClass: 'hucre-salt', valueFormatter: function (p) { return zaman(p.value); } },
            { field: 'harcamaTurAdi', headerName: 'Türü', width: 90, cellClass: 'hucre-salt' },
            { field: 'yerleskeAdi', headerName: 'Yer', flex: 1, minWidth: 160, tooltipField: 'yerleskeAdi', cellClass: 'hucre-salt' },
            {
                field: 'tutar', headerName: 'Tutar', width: 120, type: 'rightAligned', cellClass: ['hucre-salt', 'ag-right-aligned-cell'],
                valueFormatter: function (p) { return p.value === null || p.value === undefined ? '' : tutarBicim.format(p.value); }
            }
        ],
        rowSelection: 'single',
        onRowClicked: function (e) { satirSecildi(e.data); },
        overlayNoRowsTemplate: '<span class="text-muted">Gider yok</span>'
    });

    // ---------------------------------------------------------------- form

    function yerYaz(kod, ad) {
        yer = { kod: parseInt(kod, 10) || 0, ad: ad || '' };
        $('#ddlYerleskeMetin').val(yer.ad).attr('title', yer.ad || null);
    }

    // Eski KontrolleriAyarla
    function kontrolleriAyarla(y) {
        yeni = y;
        $('#txtTarih').prop('readonly', !y);
        $('#cmbHarcamaTur').prop('disabled', !y);
        $('#btnYerSec, #btnYerTemizle').prop('disabled', !y);
        $('#btnKaydetMetin').text(y ? 'Ekle' : 'Değiştir');
    }

    // Eski Temizle
    function temizle() {
        yerYaz(0, '');
        $('#cmbHarcamaTur').val('0');
        $('#txtTarih').val('');
        $('#nfUcret').val('');
        $('#frmGider .form-group').removeClass('has-error');
        grid.deselectAll();
        kontrolleriAyarla(true);
    }

    // Eski EkranaYaz
    function ekranaYaz(g) {
        $('#txtTarih').val(g.tarihDT || '');
        yerYaz(g.yerleskeKod, g.yerleskeAdi);
        $('#cmbHarcamaTur').val(String(g.harcamaTur));
        if ($('#cmbHarcamaTur').val() === null) $('#cmbHarcamaTur').val('0');
        $('#nfUcret').val(g.tutar);
        kontrolleriAyarla(false);
    }

    function formOku() {
        return {
            personelKod: personelKod,
            yeni: yeni,
            tarih: $('#txtTarih').val() || '',
            yerleskeKod: yer.kod,
            harcamaTur: parseInt($('#cmbHarcamaTur').val(), 10) || 0,
            tutar: parseFloat($('#nfUcret').val()) || 0
        };
    }

    // ---------------------------------------------------------------- okuma

    function tanimOku() {
        return Sgb.al('api/personelbilgi/gider/tanim').done(function (v) {
            $('#spnYil').val(v.yil);
            $('#spnAy').val(v.ay);
            var $c = $('#cmbHarcamaTur').empty();
            $('<option></option>').val('0').text('').appendTo($c);
            $.each(v.harcamaTurleri || [], function (i, t) { $('<option></option>').val(String(t.kod)).text(t.ad).appendTo($c); });
        }).fail(hataGoster);
    }

    // Eski Listele
    function listele(ilk) {
        if (personelKod <= 0) return $.Deferred().resolve().promise();
        if (!ilk) grid.setGridOption('loading', true);
        return Sgb.al('api/personelbilgi/gider/liste', {
            personelKod: personelKod,
            yil: parseInt($('#spnYil').val(), 10) || 0,
            ay: parseInt($('#spnAy').val(), 10) || 0
        }).done(function (v) {
            grid.setGridOption('rowData', v.satirlar || []);
            if (v.hata) mesaj(v.hata, 'danger');
        }).fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    // Eski SatirSecildi
    function satirSecildi(d) {
        if (personelKod <= 0) return;
        Sgb.al('api/personelbilgi/gider/oku', { personelKod: personelKod, yerleskeKod: d.yerleskeKod, harcamaTur: d.harcamaTur, tarih: d.tarihDT })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                ekranaYaz(v.bilgi);
            })
            .fail(hataGoster);
    }

    // ---------------------------------------------------------------- düğmeler

    // Eski ZorunluAlanKontrol: Harcama Yeri, Tür, Tarih, Tutar boş bırakılamaz.
    function zorunluAlanKontrol() {
        var f = formOku();
        var eksik = [
            ['#ddlYerleskeMetin', !f.yerleskeKod],
            ['#cmbHarcamaTur', !f.harcamaTur],
            ['#txtTarih', !f.tarih],
            ['#nfUcret', $.trim($('#nfUcret').val()) === '']
        ];
        var gecerli = true;
        $.each(eksik, function (i, e) {
            $(e[0]).closest('.form-group').toggleClass('has-error', e[1]);
            if (e[1]) gecerli = false;
        });
        if (!gecerli) mesaj('Kırmızı renk ile işaretlenen alanları boş bırakamazsınız.', 'warning');
        return gecerli;
    }

    $('#btnKaydet').on('click', function () {
        if (!zorunluAlanKontrol()) return;
        onayla('Kayıt işlemi yapılacak. Onaylıyor musunuz?').done(function () {
            if (personelKod <= 0) { mesaj('Personel Bilgisi boş bırakılamaz.', 'warning'); return; }
            mesgul(true);
            Sgb.gonder('api/personelbilgi/gider/kaydet', formOku())
                .done(function (s) {
                    if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                    mesaj(s.bilgi, 'success');
                    listele(false);
                })
                .fail(hataGoster)
                .always(function () { mesgul(false); });
        });
    });

    $('#btnTemizle').on('click', temizle);
    $('#btnListele').on('click', function () { listele(false); });

    // Eski AktarSor + CBVeriAktar
    $('#btnAktar').on('click', function () {
        var yil = parseInt($('#spnYil').val(), 10) || 0, ay = parseInt($('#spnAy').val(), 10) || 0;
        var turAd = $('#cmbHarcamaTur option:selected').text();
        onayla(yil + ' yılı ' + ay + '. ay, ' + turAd + ' verilerini aktarmak istiyor musunuz? Verilerin hepsi silinip tekrar aktarılacaktır.')
            .done(function () {
                mesgul(true);
                Sgb.gonder('api/personelbilgi/gider/aktar', { yil: yil, ay: ay, harcamaTur: parseInt($('#cmbHarcamaTur').val(), 10) || 0 })
                    .done(function (s) {
                        if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                        mesaj(s.bilgi, 'success');
                        listele(false);
                    })
                    .fail(hataGoster)
                    .always(function () { mesgul(false); });
            });
    });

    // ---------------------------------------------------------------- kümülatif yükleme (eski wndKumYukle / btnKumDosyaYukle_Click)

    var dlgKum = document.getElementById('dlgKumYukle');
    $('#btnKumYukle').on('click', function () { dlgKum.showModal(); });

    $('#btnKumDosyaYukle').on('click', function () {
        var f = formOku();
        var dosya = $('#fuKumDosyaYukle')[0].files[0];
        var veri = new FormData();
        veri.append('personelKod', String(personelKod));
        veri.append('yil', String(parseInt($('#spnYil').val(), 10) || 0));
        veri.append('ay', String(parseInt($('#spnAy').val(), 10) || 0));
        veri.append('yerleskeKod', String(f.yerleskeKod));
        veri.append('harcamaTur', String(f.harcamaTur));
        veri.append('tutar', String(f.tutar));
        if (dosya) veri.append('dosya', dosya);
        var $d = $(this).prop('disabled', true);
        $.ajax({
            url: Sgb.KOK + 'api/personelbilgi/gider/kum-yukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (s) {
            mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
            if (s.basarili) dlgKum.close();
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else hataGoster(xhr);
        }).always(function () {
            $d.prop('disabled', false);
            $('#fuKumDosyaYukle').val('');
        });
    });

    // ---------------------------------------------------------------- harcama yeri seçimi (eski ExtOrgSecim AgacTur=2)

    var dlgYer = document.getElementById('dlgYer');
    var agac = null;
    var aranan = '';

    function yerAgaciKur() {
        agac = Sgb.agac('#trpYer', {
            yukle: function (ust) {
                return Sgb.al('api/personelbilgi/gider/yerler', { ust: ust ? ust.kod : 0, ara: ust ? '' : aranan }).then(function (v) {
                    if (v.hata) mesaj(v.hata, 'danger');
                    return v.satirlar || [];
                });
            },
            ipucu: function (d) { return d.tarif || d.ad; },
            secilince: function (d) {
                yerYaz(d.kod, d.ad);
                $('#ddlYerleskeMetin').closest('.form-group').removeClass('has-error');
                dlgYer.close();
            },
            bosMetin: 'Kayıt yok',
            hata: hataGoster
        });
    }

    $('#btnYerSec').on('click', function () {
        if (!agac) yerAgaciKur();
        dlgYer.showModal();
    });
    function yerAra() {
        aranan = $.trim($('#txtYerAra').val());
        if (!agac) yerAgaciKur(); else agac.yenile();
    }
    $('#btnYerAra').on('click', yerAra);
    $('#txtYerAra').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); yerAra(); } });
    $('#btnYerTemizle').on('click', function () { yerYaz(0, ''); });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- açılış (eski Page_Load)
    tanimOku().always(function () {
        temizle();
        listele(true);
    });
});
