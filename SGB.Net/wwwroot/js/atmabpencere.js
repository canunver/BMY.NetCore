// AB (IPARD) Takip İşlemleri: işlem pencereleri (eski AlacakTakipListeAB.aspx pencereleri).
//   AtmAB.odemeAc(islemTur, siraNo, ilgiliSiraNo) : İşlem Girişi (eski OdemeEkranAc / BHDegisti / btnBHKaydet_Click /
//                                                   btnGeriBildirimYazdir_Click / btnRaporlar2_Click). islemTur -1: kayıtlı hareket açılır.
//   AtmAB.islemAc(tabloKod, sira)                 : 1 Borç Bilgisi, 2 Ön Borç, 3 Geri Alım Kararı, 4 Süreç Durumu, 5 Taksit
//                                                   (eski IslemEkranAc ve Btn*Islem_Click). Pencere kapanınca dosya yeniden okunur (SayfaYenile).
//   AtmAB.imzaAc() (eski wndImzalar), AtmAB.donemFaiziAc() (eski wndHesaplat).
// Hesaplamalar: ön borç / geri alım kararı toplamları ve EUR karşılıkları sayfada (eski OnBorcRakamYaz / GAKRakamYaz: kuruş yuvarlaması),
// ödeme ve taksit tutarları sunucuda (odeme/hesapla, taksit/hesapla) hesaplanır.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return;

    var A = window.AtmAB;
    var U = A.U;
    var mesaj = A.mesaj, hataGoster = A.hataGoster;

    function onayla(metin) { return !metin || window.confirm(metin); }

    // Pencerelerin kapat tuşları
    $('dialog.ab-pencere').on('click', '[data-kapat]', function (e) {
        e.stopPropagation();
        $(this).closest('dialog')[0].close();
    });

    function goster(d) { if (!d.open) d.showModal(); }

    // tarih (yyyy-MM-dd) + gün; saat dilimi kaymasın diye UTC ile
    function gunEkle(s, gun) {
        var p = /^(\d{4})-(\d{2})-(\d{2})$/.exec(s || '');
        if (!p) return '';
        var d = new Date(Date.UTC(+p[1], +p[2] - 1, +p[3] + gun));
        return d.toISOString().substring(0, 10);
    }

    function kurOku(tarih) {
        if (!tarih) return $.Deferred().resolve(0).promise();
        return Sgb.al(U + 'kur', { tarih: tarih }).then(function (v) {
            if (v.hata) { mesaj(v.hata, 'warning'); return 0; }
            return v.kur || 0;
        }, function (xhr) { hataGoster(xhr); return 0; });
    }

    function seceneklerYaz(sel, liste) {
        var $s = $(sel).empty();
        $.each(liste || [], function (i, x) { $('<option></option>').attr('value', x.kod).text(x.ad).appendTo($s); });
    }

    // ================================================================ İşlem Girişi (eski wndOdemeGiris)

    var dlgOdeme = document.getElementById('dlgOdeme');
    var $o = $(dlgOdeme);
    var od = { iPARDDonemi: 0, dosyaAnahtar: 0, siraNo: 0, ilgiliIslemSiraNo: 0, tur: 0 };
    var odemeSayac = 0;

    function odemeYaz(v) {
        var h = v.hareket || {}, e = v.ekran || {};
        od = { iPARDDonemi: v.iPARDDonemi, dosyaAnahtar: v.dosyaAnahtar, siraNo: h.siraNo || 0, ilgiliIslemSiraNo: h.ilgiliIslemSiraNo || 0, tur: h.tur || 0 };
        A.doldur($o, h, 'data-h', true);
        A.doldur($o, v.kalan || {}, 'data-k', true);
        $('#dlgOdemeBaslik').text(e.baslik || 'İşlem Girişi');
        $o.find('.ab-odeme-turu').prop('hidden', !e.odemeTuru);
        $o.find('.ab-aktarim').prop('hidden', !e.aktarimGoster);
        $o.find('.ab-ulusal').prop('readonly', !!e.ulusalSaltOkunur).toggleClass('okuma', !!e.ulusalSaltOkunur);
        $('#o_lblIslem').text(e.islemEtiket || 'Geri Alım Miktarı (TL)');
        $('#o_lblBolum').text(e.islemBolumEtiket || 'İşlem Tarih/Karar/Taksit');
        $('#o_lblDuzeltme').text(e.duzeltmeEtiket || 'Düzeltme Tarihi');
    }

    function odemeGovde(ek) {
        return $.extend(A.topla($o, 'data-h'), {
            iPARDDonemi: od.iPARDDonemi,
            dosyaAnahtar: od.dosyaAnahtar,
            siraNo: od.siraNo,
            ilgiliIslemSiraNo: od.ilgiliIslemSiraNo,
            tur: od.tur,
            hatadaKaydet: $('#o_hatadaKaydet').prop('checked')
        }, ek || {});
    }

    A.odemeAc = function (islemTur, siraNo, ilgiliSiraNo) {
        var k = A.sayfa.anahtar();
        if (!(k.iPARDDonemi > 0 && k.dosyaAnahtar > 0)) { mesaj('Lütfen önce dosya kaydını yapınız!', 'warning'); return; }
        Sgb.al(U + 'odeme/ac', { iPARDDonemi: k.iPARDDonemi, dosyaAnahtar: k.dosyaAnahtar, islemTur: islemTur, siraNo: siraNo || 0, ilgiliSiraNo: ilgiliSiraNo || 0 })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#o_hatadaKaydet').prop('checked', false);
                $o.find('.ab-alt-sekme-tus[data-alt-sekme="otl"]').trigger('click');
                odemeYaz(v);
                goster(dlgOdeme);
            })
            .fail(hataGoster);
    };

    // eski BHDegisti: tarih, yatırılan, aktarılan, miktar / faiz alanları ve "Hatalı ise de kaydet" değişince tutarlar yeniden hesaplanır
    $o.on('change', '[data-degisen]', function () {
        var no = ++odemeSayac;
        Sgb.gonder(U + 'odeme/hesapla', odemeGovde({ degisen: String($(this).data('degisen')) }))
            .done(function (v) {
                if (no !== odemeSayac) return; // daha yeni bir hesap istendi
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                odemeYaz(v);
            })
            .fail(hataGoster);
    });

    // eski btnBHKaydet_Click (IslemTur 1 / 5 / 9 / 6)
    $o.on('click', '[data-islem]', function () {
        var $t = $(this);
        if (!onayla($t.data('onay'))) return;
        $t.prop('disabled', true);
        Sgb.gonder(U + 'odeme/kaydet', odemeGovde({ islemTur: parseInt($t.data('islem'), 10) }))
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
                A.sayfa.yenile();
                if (v.siraNo > 0) A.odemeAc(-1, v.siraNo, 0);
            })
            .fail(hataGoster)
            .always(function () { $t.prop('disabled', false); });
    });

    // Geri Bildirim Yazdır (rapor 2)
    $('#o_geriBildirim').on('click', function () {
        var g = odemeGovde();
        Sgb.indir(U + 'rapor/geribildirim', {
            iPARDDonemi: od.iPARDDonemi, dosyaAnahtar: od.dosyaAnahtar, siraNo: od.siraNo,
            tarih: g.tarih, borcunIlgiliOlduguTaksit: g.borcunIlgiliOlduguTaksit, duzeltimTarihi: g.duzeltimTarihi,
            yatirilanAB: g.yatirilanAB, yatirilanUlusal: g.yatirilanUlusal, olduguGibi: g.hatadaKaydet
        });
    });

    // 6307 Faiz Hes. Tab. / (PDF): işlem tarihine kadar (eski btnRaporlar2_Click)
    $o.on('click', '[data-odeme-rapor]', function () {
        Sgb.indir(U + 'rapor/odeme', { no: 1, tur: $(this).data('odeme-rapor'), iPARDDonemi: od.iPARDDonemi, dosyaAnahtar: od.dosyaAnahtar, tarih: $('#o_tarih').val() || '' });
    });

    // ================================================================ parça pencereleri (borçlu, ön borç, geri alım kararı, durum, taksit)

    var PARCA = {
        1: { pencere: 'dlgBorclu', uc: 'borclu/' },
        2: { pencere: 'dlgOnBorc', uc: 'onborc/' },
        3: { pencere: 'dlgGak', uc: 'gak/' },
        4: { pencere: 'dlgDurum', uc: 'durum/' },
        5: { pencere: 'dlgTaksit', uc: 'taksit/' }
    };

    // Ortak alt bölüm: Kaydeden / Kayıt Tarihi / Onaylayan-İptal Eden / Onay-İptal Tarihi
    $('dialog.ab-pencere .ab-parca-alt').each(function () {
        $(this).html(
            '<div class="form-group"><span class="control-label">Kaydeden / Kayıt Tarihi</span><div class="ab-bitisik">'
            + '<input type="text" class="form-control input-sm okuma" readonly data-p="kaydedenAdSoyad" data-salt aria-label="Kaydeden" />'
            + '<input type="text" class="form-control input-sm okuma ab-tarih" readonly data-p="kayitTarihi" data-bicim="tarih" data-salt aria-label="Kayıt tarihi" /></div></div>'
            + '<div class="form-group"><span class="control-label">Onaylayan/İptal Eden</span><div class="ab-bitisik">'
            + '<input type="text" class="form-control input-sm okuma" readonly data-p="onaylayanAdSoyad" data-salt aria-label="Onaylayan / iptal eden" />'
            + '<input type="text" class="form-control input-sm okuma ab-tarih" readonly data-p="onayTarihi" data-bicim="tarih" data-salt aria-label="Onay / iptal tarihi" /></div></div>');
    });

    var bMuhBirim = Sgb.muhasebeBirim('#bMuhBirim', { kurumsalGizle: true });
    var obKodlar = A.coklu('#ob_kodlar');
    var grdTaksitOnizleme = Sgb.grid('#grdTaksitOnizleme', {
        columnDefs: A.taksitKolonlari(),
        overlayNoRowsTemplate: '<span>İlk ödeme tarihi, taksit sayısı ve talep tarihini giriniz.</span>'
    });

    var acikParca = { tabloKod: 0, sira: 0 };

    function kaldirmaUygula($d) {
        var k = $d.find('.ab-kaldirma').prop('checked');
        $d.find('.ab-kaldirilir').filter('input, select, textarea').prop('disabled', !!k);
        if ($d.is('#dlgOnBorc')) obKodlar.etkin(!k);
        $d.find('.ab-kaldirilir-grid').toggleClass('ab-pasif', !!k);
    }

    function fazlaOdemeUygula() {
        var fazla = A.tanim && parseInt($('#ob_borcTuru').val(), 10) === A.tanim.fazlaOdeme;
        $('#dlgOnBorc .ab-fazla-odeme').prop('hidden', !fazla);
    }

    function taksitYaz(t) {
        t = t || {};
        $('#t_anaPara').val(A.tutarYaz(t.toplamAnaPara || 0));
        $('#t_faiz').val(A.tutarYaz(t.toplamFaiz || 0));
        grdTaksitOnizleme.setGridOption('rowData', t.satirlar || []);
    }

    function parcaYaz(tabloKod, v) {
        var c = PARCA[tabloKod], $d = $('#' + c.pencere);
        A.doldur($d, v, 'data-p', true);
        $d.find('.ab-yeni-tus').prop('hidden', !v.yeni);
        $d.find('.ab-guncelle-tus').prop('hidden', !!v.yeni);
        $d.find('.ab-yeniyap-tus').prop('hidden', !v.yeniYapilir);
        if (tabloKod === 1) {
            $('#b_ipard').prop('disabled', !!v.mevcutDosya);
            bMuhBirim.ata({ muhasebe: v.saymanlikKod, birim: v.birimKod, birimAd: v.birimAd });
        } else if (tabloKod === 2) {
            obKodlar.ata(v.kodlar || []);
            obKodlar.sec(v.borcunKodu || '');
            fazlaOdemeUygula();
        } else if (tabloKod === 5) {
            taksitYaz(v.taksitler);
        }
        kaldirmaUygula($d);
    }

    A.islemAc = function (tabloKod, sira) {
        var c = PARCA[tabloKod];
        if (!c) return;
        var k = A.sayfa.anahtar();
        if (tabloKod !== 1 && !(k.iPARDDonemi > 0 && k.dosyaAnahtar > 0)) { mesaj('Lütfen önce dosya kaydını yapınız!', 'warning'); return; }
        Sgb.al(U + c.uc + 'ac', { iPARDDonemi: k.iPARDDonemi, dosyaAnahtar: k.dosyaAnahtar, sira: sira || 0 })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acikParca = { tabloKod: tabloKod, sira: v.sira || 0 };
                parcaYaz(tabloKod, v);
                goster(document.getElementById(c.pencere));
            })
            .fail(hataGoster);
    };

    function parcaGovde(tabloKod, $d) {
        var g = A.topla($d, 'data-p');
        if (tabloKod === 1) {
            var mb = bMuhBirim.deger();
            g.saymanlikKod = mb.muhasebe ? String(mb.muhasebe) : '';
            g.birimKod = mb.birim || '';
        } else if (tabloKod === 2) {
            g.borcunKodu = obKodlar.deger();
        }
        return g;
    }

    $.each(PARCA, function (kod, c) {
        var tabloKod = parseInt(kod, 10);
        var $d = $('#' + c.pencere);
        // eski BeforeHide: SayfaYenile
        $d.on('close', function () { A.sayfa.yenile(); });
        $d.on('click', '[data-islem]', function () {
            var $t = $(this);
            if (!onayla($t.data('onay'))) return;
            var g = $.extend(parcaGovde(tabloKod, $d), { islemTur: parseInt($t.data('islem'), 10) });
            $t.prop('disabled', true);
            Sgb.gonder(U + c.uc + 'kaydet', g)
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                    mesaj(v.bilgi, 'success');
                    // Yeni Dosya: borçlu kaydı dosyayı oluşturur; pencere kapanınca bu dosya okunur
                    if (tabloKod === 1) A.sayfa.dosyaAyarla(v.iPARDDonemi, v.dosyaAnahtar);
                    A.islemAc(tabloKod, v.sira);
                })
                .fail(hataGoster)
                .always(function () { $t.prop('disabled', false); });
        });
        $d.on('change', '.ab-kaldirma', function () { kaldirmaUygula($d); });
    });

    // ---------------------------------------------------------------- Borç Bilgisi: kişi / firma (eski FirmaGetirHandler / FirmaBilgisiGoster)

    $('#b_firmaSec').on('click', function () {
        var muh = bMuhBirim.deger().muhasebe || 0;
        Sgb.firmaSec({
            adres: U + 'firma/ara',
            muhasebe: muh,
            secilince: function (f) {
                // Seçim penceresi yalnızca vergi no / ad / adres verir; borçlu kodu için kayıt yeniden aranır.
                Sgb.al(U + 'firma/ara', { muhasebe: muh, vergiNo: f.vergiNo })
                    .done(function (v) {
                        if (v.hata) { mesaj(v.hata, 'warning'); return; }
                        var r = $.grep(v.satirlar || [], function (x) { return x.vergiNo === f.vergiNo; })[0];
                        if (!r || !r.kod) { mesaj('Seçilen kişi / firmanın kaydı bulunamadı.', 'warning'); return; }
                        $('#b_borcluKod').val(String(r.kod));
                        $('#b_borclu').val(f.vergiNo + '- ' + f.ad);
                    })
                    .fail(hataGoster);
            }
        });
    });

    // ---------------------------------------------------------------- Ön Borç: kur ve tutarlar (eski OnBorcDegisti / OnBorcRakamYaz)

    var $ob = $('#dlgOnBorc');
    function obHesapla() {
        var kur = A.tutarOku($ob.find('[data-p="onBorcDovizKuru"]').val()) || 0;
        var ab = A.tutarOku($ob.find('[data-p="onBorcMiktarAB"]').val()) || 0;
        var ul = A.tutarOku($ob.find('[data-p="onBorcMiktarUlusal"]').val()) || 0;
        var abE = kur > 0 ? A.kurus(ab / kur) : 0, ulE = kur > 0 ? A.kurus(ul / kur) : 0;
        A.doldur($ob, {
            onBorcToplam: A.kurus(ab + ul), onBorcMiktarABEUR: abE, onBorcMiktarUlusalEUR: ulE, onBorcToplamEUR: A.kurus(abE + ulE)
        }, 'data-p');
    }
    $('#ob_tarih').on('change', function () {
        kurOku($(this).val()).done(function (kur) {
            A.doldur($ob, { onBorcDovizKuru: kur }, 'data-p');
            obHesapla();
        });
    });
    $ob.on('change', '[data-p="onBorcMiktarAB"], [data-p="onBorcMiktarUlusal"]', obHesapla);
    // eski cmbBorcTuru_Change: borç kodları yeniden okunur, seçim temizlenir
    $('#ob_borcTuru').on('change', function () {
        fazlaOdemeUygula();
        Sgb.al(U + 'onborc/kodlar', { borcTuru: parseInt($(this).val(), 10) || 0 })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                obKodlar.ata(v.kodlar || []);
                obKodlar.sec('');
            })
            .fail(hataGoster);
    });

    // ---------------------------------------------------------------- Geri Alım Kararı (eski GeriAlimDegisti / TesellumTarihi_Changed / OdemeSonTarihi_Changed)

    var $g = $('#dlgGak');
    function gakHesapla() {
        var kur = A.tutarOku($g.find('[data-p="gakDovizKuru"]').val()) || 0;
        var ab = A.tutarOku($g.find('[data-p="gakMiktarAB"]').val()) || 0;
        var ul = A.tutarOku($g.find('[data-p="gakMiktarUlusal"]').val()) || 0;
        var sil = A.tutarOku($g.find('[data-p="silinmeyecekMiktar"]').val()) || 0;
        var abE = kur > 0 ? A.kurus(ab / kur) : 0, ulE = kur > 0 ? A.kurus(ul / kur) : 0;
        A.doldur($g, {
            gakToplam: A.kurus(ab + ul), gakMiktarABEUR: abE, gakMiktarUlusalEUR: ulE, gakToplamEUR: A.kurus(abE + ulE),
            silinmeyecekMiktarEUR: kur > 0 ? A.kurus(sil / kur) : 0
        }, 'data-p');
    }
    $('#g_tarih').on('change', function () {
        kurOku($(this).val()).done(function (kur) {
            A.doldur($g, { gakDovizKuru: kur }, 'data-p');
            gakHesapla();
        });
    });
    $g.on('change', '[data-p="gakMiktarAB"], [data-p="gakMiktarUlusal"], [data-p="silinmeyecekMiktar"]', gakHesapla);
    // Mektup alınma tarihinden ödeme son tarihi (1 ay sonrası, resmi tatilse ertesi iş günü) ve temerrüt tarihi
    $('#g_tesellum').on('change', function () {
        var t = $(this).val();
        if (!t) return;
        Sgb.al(U + 'gak/tarihler', { tesellumTarihi: t })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#g_odemeSon').val(v.odemeSonTarihi || '');
                $('#g_temerrut').val(v.temerrutTarihi || '');
            })
            .fail(hataGoster);
    });
    // Temerrüt tarihi ödeme son tarihinden bir gün sonradır
    $('#g_odemeSon').on('change', function () { $('#g_temerrut').val(gunEkle($(this).val(), 1)); });

    // ---------------------------------------------------------------- Taksit (eski Taksitlendir / btnTaksitYazdir_Click Kayitli=0)

    var $tk = $('#dlgTaksit');
    var taksitSayac = 0;
    $tk.on('change', '[data-taksit]', function () {
        var no = ++taksitSayac;
        grdTaksitOnizleme.setGridOption('loading', true);
        Sgb.gonder(U + 'taksit/hesapla', A.topla($tk, 'data-p'))
            .done(function (v) {
                if (no !== taksitSayac) return;
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                taksitYaz(v);
            })
            .fail(hataGoster)
            .always(function () { if (no === taksitSayac) grdTaksitOnizleme.setGridOption('loading', false); });
    });
    $('#t_yazdir').on('click', function () {
        var g = A.topla($tk, 'data-p');
        Sgb.indir(U + 'rapor/taksit', $.extend({
            kayitli: 0, iPARDDonemi: g.ipardDonemi, dosyaAnahtar: g.dosyaAnahtar,
            girisTarihi: g.girisTarihi, taksitTalepTarihi: g.taksitTalepTarihi, taksitTalepSayi: g.taksitTalepSayi,
            taksitKararTarihi: g.taksitKararTarihi, taksitKararSayi: g.taksitKararSayi, taksitIlkTarih: g.taksitIlkTarih,
            taksitSayisi: g.taksitSayisi, tecilFaizNo: g.tecilFaizNo
        }, A.sayfa.tarihler()));
    });

    // ================================================================ İmzalar (eski wndImzalar_Show / btnImzaKaydet_Click)

    var dlgImza = document.getElementById('dlgImza');
    A.imzaAc = function () {
        Sgb.al(U + 'imza')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                A.doldur(dlgImza, v, 'data-p', true);
                goster(dlgImza);
            })
            .fail(hataGoster);
    };
    $('#i_kaydet').on('click', function () {
        var $b = $(this).prop('disabled', true);
        Sgb.gonder(U + 'imza', A.topla(dlgImza, 'data-p'))
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $b.prop('disabled', false); });
    });

    // ================================================================ Dönem Faizi (eski wndHesaplat / btnHesaplat_Click)

    var dlgDonem = document.getElementById('dlgDonemFaizi');
    A.donemFaiziAc = function () { goster(dlgDonem); };
    $(dlgDonem).on('click', '[data-htur]', function () {
        var $b = $(dlgDonem).find('[data-htur]').prop('disabled', true);
        Sgb.gonder(U + 'donemfaizi', {
            iPARDDonemi: A.sayfa.ipard(),
            yil: parseInt($('#h_yil').val(), 10) || 0,
            ay: parseInt($('#h_ay').val(), 10) || 0,
            hTur: parseInt($(this).data('htur'), 10)
        })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $b.prop('disabled', false); });
    });

    // ================================================================ tanımlar gelince seçenekler

    function tanimYaz(v) {
        seceneklerYaz('#b_ipard', v.ipardlar);
        seceneklerYaz('#ob_borcTuru', v.borcTurleri);
        seceneklerYaz('#du_surec', v.durumlar);
        Sgb.yilKutusu('#h_yil', [v.hesaplaYilEnAz, v.hesaplaYilEnCok], v.hesaplaYil);
        $('#h_ay').val(String(v.hesaplaAy));
    }
    if (A.tanim) tanimYaz(A.tanim);
    $(document).on('atmab:tanim', function (e, v) { tanimYaz(v); });
});
