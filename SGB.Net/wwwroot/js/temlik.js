// Borçlar ve Temlikler / Temlik Tanımlama (ButceMuhasebe/TaahhutBorclarVeTemliklerKayit, eski aynı adlı Ext.NET sayfası):
// solda kıstaslar ve temlik listesi (açılışta listelenir, eski Page_Load gibi), sağda seçili temliğin formu (Kaydet / Sil / Temizle).
// Uçlar: /api/temlik/* (Istemci/Muhasebe/TemlikUclari.cs).
$(function () {
    'use strict';

    if (!$('.temlik #grdListe').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var arama = Sgb.mt.form('#frmArama');

    function tarih(p) { return p.value ? Sgb.tarih(p.value) : ''; }

    var kolonlar = [
        { field: 'KONU', headerName: 'Konu', flex: 1, minWidth: 160, tooltipField: 'KONU' },
        { field: 'ALACAKLIADSOYAD', headerName: 'Alacaklı', width: 160, tooltipField: 'ALACAKLIADSOYAD' },
        { field: 'TEMLIKACANVERGINO', headerName: 'Temlik Açan V.No', width: 125 },
        { field: 'TEMLIKVERENVERGINO', headerName: 'Temlik Veren V.No', width: 130 },
        { field: 'TEMLIKTUTARI', headerName: 'Temlik Tutarı', width: 120, type: 'rightAligned', valueFormatter: function (p) { return Sgb.mt.tutar(p.value); } },
        { field: 'NOTERONAYTARIHI', headerName: 'Noter Onay Tarihi', width: 125, valueFormatter: tarih },
        { field: 'NOTERONAYNO', headerName: 'Noter Onay No', width: 115 }
    ];

    function silDurum(evet) { $('#btnSil').prop('disabled', !evet); }

    // Eski zorunluAlanKontrolu
    var ZORUNLU = [
        ['konu', 'Lütfen konu alanını giriniz', '#tmKonu'],
        ['alacakliAdSoyad', 'Lütfen alacaklı alanını giriniz', '#tmAlacakli'],
        ['temlikAcanVergiNo', 'Lütfen temlik açan vergi numarası alanını giriniz', '#tmAcan'],
        ['temlikVerenVergiNo', 'Lütfen temlik veren vergi numarası alanını giriniz', '#tmVeren'],
        ['temlikTutari', 'Lütfen temlik tutarı alanını giriniz', '#tmTutar'],
        ['noterOnayTarihi', 'Lütfen noter onay tarihi alanını giriniz', '#tmNoterTarih'],
        ['noterOnayNo', 'Lütfen noter onay numarası alanını giriniz', '#tmNoterNo']
    ];

    var sayfa = Sgb.mt.tanim({
        tablo: '#grdListe',
        form: '#frmBilgi',
        uc: 'api/temlik/',
        kolonlar: kolonlar,
        anahtar: 'KOD',
        acilistaListele: true,
        listeParam: function () { return arama.oku(); },
        hazirla: function (satirlar) {
            var t = 0;
            $.each(satirlar, function (i, r) { t += Number(r.TEMLIKTUTARI) || 0; });
            $('#lblOzet').text(satirlar.length ? satirlar.length + ' kayıt, toplam ' + Sgb.mt.tutar(t) : '');
            sayfa && sayfa.tablo.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
        },
        // Eski SatirSecildi: satırdaki bilgiler forma yazılır.
        satirdanForma: function (s) {
            return {
                kod: s.KOD, konu: s.KONU, alacakliAdSoyad: s.ALACAKLIADSOYAD, temlikAcanVergiNo: s.TEMLIKACANVERGINO,
                temlikVerenVergiNo: s.TEMLIKVERENVERGINO, temlikTutari: Sgb.mt.tutar(s.TEMLIKTUTARI), aciklama: s.ACIKLAMA,
                noterOnayTarihi: s.NOTERONAYTARIHI, noterOnayNo: s.NOTERONAYNO
            };
        },
        secilince: function (d) { silDurum((parseInt(d.kod, 10) || 0) > 0); },
        temizlenince: function () { silDurum(false); },
        istek: function (f) {
            for (var i = 0; i < ZORUNLU.length; i++) {
                if (!f[ZORUNLU[i][0]]) {
                    Sgb.bildirim(ZORUNLU[i][1], 'warning');
                    $(ZORUNLU[i][2]).trigger('focus');
                    return null;
                }
            }
            if (!window.confirm('Kayıt edilecek. Onaylıyor musunuz?')) return null;
            return {
                kod: parseInt(f.kod, 10) || 0,
                konu: f.konu,
                alacakliAdSoyad: f.alacakliAdSoyad,
                temlikAcanVergiNo: f.temlikAcanVergiNo,
                temlikVerenVergiNo: f.temlikVerenVergiNo,
                temlikTutari: Sgb.mt.sayi(f.temlikTutari),
                aciklama: f.aciklama,
                noterOnayTarihi: f.noterOnayTarihi,
                noterOnayNo: f.noterOnayNo
            };
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

    // Tutar alanından çıkınca 1.234,56 biçimine getirilir.
    $('#tmTutar').on('blur', function () {
        var v = $.trim($(this).val());
        if (v !== '') $(this).val(Sgb.mt.tutar(Sgb.mt.sayi(v)));
    });

    silDurum(false);
});
