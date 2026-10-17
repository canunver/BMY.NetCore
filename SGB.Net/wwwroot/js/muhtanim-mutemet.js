// Mutemet Tanımlama (ButceMuhasebe/TanimMutemetLimit) ve Hesap Planı Kısıtlama Tanımlama (ButceMuhasebe/TanimHesapPlaniKisit):
// solda sorgu + liste (açılışta bu yıl listelenir), sağda seçili tanımın formu. Kısıtlamada kişi ve tutar yoktur.
// Uçlar: /api/muhtanim/mutemet/* ve /api/muhtanim/hesapkisit/*.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return;

    var tur = $('.muh-tanim').attr('data-tur');
    var mutemet = tur === 'mutemet';
    var arama = Sgb.mt.form('#frmArama');

    var mb = Sgb.muhasebeBirim('#mtMuhBirim', {
        yil: function () { return parseInt($('#mtYil').val(), 10) || Sgb.mt.buYil; },
        kurumsalGizle: true
    });

    var kolonlar = [
        { field: 'YIL', headerName: 'Yıl', width: 70 },
        { field: 'MUHASEBEKOD', headerName: 'Muhasebe Kod', width: 110 },
        { field: 'BIRIMKOD', headerName: 'Birim Kod', width: 90 }
    ];
    if (mutemet) kolonlar.push({ colId: 'kisi', headerName: 'Kişi Kodu', flex: 1, minWidth: 200, valueGetter: function (p) { return p.data.TCKIMLIKNO + ' - ' + p.data.KISIADI; } });
    kolonlar.push({ colId: 'hesap', headerName: 'Hesap Kod', flex: 1, minWidth: 200, valueGetter: function (p) { return p.data.HESAPKOD + ' - ' + p.data.HESAPADI; } });
    if (mutemet) kolonlar.push({ field: 'TUTAR', headerName: 'Tutar', width: 130, type: 'rightAligned', valueFormatter: function (p) { return Sgb.mt.tutar(p.value); } });

    function aramaParam() {
        var p = arama.oku();
        p.yil = parseInt(p.yil, 10) || 0;
        p.muhasebeKod = parseInt(p.muhasebeKod, 10) || 0;
        return p;
    }

    var sayfa = Sgb.mt.tanim({
        tablo: '#grdListe',
        form: '#frmBilgi',
        uc: 'api/muhtanim/' + tur + '/',
        kolonlar: kolonlar,
        anahtar: 'KOD',
        acilistaListele: true,
        listeParam: aramaParam,
        varsayilan: function () { return { yil: $('#mtYil').val() || $('#srYil').val() || Sgb.mt.buYil }; },
        // Satır seçilince kayıt koduyla yeniden okunur (eski SatirSecildi).
        satirdanForma: function (s) {
            return Sgb.mt.oku('api/muhtanim/' + tur + '/liste', { kod: s.KOD }).then(function (v) {
                var y = (v.satirlar || [])[0] || s;
                mb.ata({ muhasebe: y.MUHASEBEKOD || '', birim: y.BIRIMKOD || '' });
                if (y.MUHASEBEKOD) mb.yenile();
                return {
                    kod: y.KOD, yil: y.YIL, tcKimlikNo: y.TCKIMLIKNO, kisiAdi: y.KISIADI, hesapKod: y.HESAPKOD, hesapAdi: y.HESAPADI,
                    tutar: mutemet ? Sgb.mt.tutar(y.TUTAR) : ''
                };
            });
        },
        temizlenince: function () { mb.ata({ muhasebe: '', birim: '', kurumsal: '', birimAd: '' }); },
        istek: function (f) {
            var d = mb.deger();
            var istek = {
                kod: parseInt(f.kod, 10) || 0, yil: parseInt(f.yil, 10) || 0, muhasebeKod: parseInt(d.muhasebe, 10) || 0, birimKod: d.birim || '',
                hesapKod: f.hesapKod, hesapAdi: f.hesapAdi
            };
            if (mutemet) $.extend(istek, { tcKimlikNo: f.tcKimlikNo, kisiAdi: f.kisiAdi, tutar: Sgb.mt.sayi(f.tutar) });
            return istek;
        },
        silIstek: function (f) {
            var kod = parseInt(f.kod, 10) || 0;
            return kod > 0 ? { kod: kod } : null;
        }
    });

    $('#frmArama').on('submit', function (e) {
        e.preventDefault();
        sayfa.listele();
    });

    $('#btnSonrakiYil').on('click', function () {
        var yil = parseInt($('#mtYil').val(), 10) || 0;
        if (!yil) { Sgb.bildirim('Yıl bilgisi boş bırakılmış.', 'warning'); return; }
        Sgb.mt.islem('api/muhtanim/' + tur + '/sonraki-yil', { yil: yil }, yil + ' yılı tanımları ' + (yil + 1) + ' yılına aktarılacak. Emin misiniz?');
    });

    $('#btnKisiSec').on('click', function () {
        Sgb.firmaSec({
            muhasebe: parseInt(mb.deger().muhasebe, 10) || 0,
            ara: $.trim($('#mtTc').val()),
            adres: 'api/muhtanim/firma/ara',
            secilince: function (f) { $('#mtTc').val(f.vergiNo); $('#mtKisiAd').val(f.ad); }
        });
    });

});
