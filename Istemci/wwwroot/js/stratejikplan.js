// Stratejik Plan (PerformansButce/StratejikPlanAna): seçili kurum ve dönemin misyonu, vizyonu, amaçları ve hedefleri.
// Dönem Seç, Misyon, Vizyon, Amaç, Hedef, Göstergeler ve Ödenekler aynı sayfadaki pencerelerdir (<dialog>).
// Üzerinde çalışılan kurum ve dönem sunucuda kullanıcıya kayıtlıdır; bütün işlemler o döneme yapılır.
$(function () {
    'use strict';

    if (!$('#lstAmaclar').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var durum = { secili: false, amaclar: [] };
    var hedefTanim = null;   // hedef penceresinin seçim listeleri (dönem değişince yeniden okunur)
    var acikHedef = 0;       // hedef penceresinde açık hedefin numarası (yeni hedefte 0)

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function pencere(kimlik) { return document.getElementById(kimlik); }
    function sayi(deger) {
        var s = parseFloat(String(deger === null || deger === undefined ? '' : deger).replace(',', '.'));
        return isNaN(s) ? 0 : s;
    }
    function dugme(simge, metin, sinif) {
        var $d = $('<button type="button" class="btn btn-xs"></button>').addClass(sinif || 'btn-default').attr('title', metin).attr('aria-label', metin);
        $('<span class="glyphicon" aria-hidden="true"></span>').addClass('glyphicon-' + simge).appendTo($d);
        return $d;
    }
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- ana sayfa

    function amaclariCiz() {
        var $liste = $('#lstAmaclar').empty();
        if (!durum.secili || !durum.misyonVar) return;
        if (!durum.amaclar.length) {
            $('<li class="bos"></li>').text("'Amaç Ekle' tuşu ile amaç ekleyiniz.").appendTo($liste);
            return;
        }
        $.each(durum.amaclar, function (i, a) {
            var $li = $('<li></li>').appendTo($liste);
            var $satir = $('<div class="satir"></div>').appendTo($li);
            var $ad = $('<span class="ad"></span>').appendTo($satir);
            $('<span class="kisa"></span>').text(a.kisa || '').appendTo($ad);
            $ad.append(document.createTextNode(a.ad || ''));
            dugme('plus', 'Bu amaca hedef ekle: ' + (a.kisa || a.ad)).on('click', function () { hedefAc(0, a.kod); }).appendTo($satir);
            dugme('pencil', 'Amacı düzenle: ' + (a.kisa || a.ad)).on('click', function () { amacAc(a.kod); }).appendTo($satir);

            var $alt = $('<ul></ul>').appendTo($li);
            $.each(a.hedefler, function (j, h) {
                var $hs = $('<div class="satir"></div>').appendTo($('<li></li>').appendTo($alt));
                var $had = $('<span class="ad"></span>').appendTo($hs);
                $('<span class="kisa"></span>').text(h.kisa || '').appendTo($had);
                $had.append(document.createTextNode(h.ad || ''));
                dugme('pencil', 'Hedefi düzenle: ' + (h.kisa || h.ad)).on('click', function () { hedefAc(h.kod, a.kod); }).appendTo($hs);
            });
        });
    }

    function durumCiz() {
        var secili = !!durum.secili, misyonVar = secili && durum.misyonVar;
        $('#planBaslik').text(secili ? durum.baslik : "Çalışacağınız kurumu ve stratejik planı 'Dönem Seç' tuşu ile seçiniz.");
        $('#misyonMetin').text(!secili ? '' : misyonVar ? durum.misyon : "'Misyon' tuşuna basarak misyon bilgilerini giriniz.").toggleClass('bos', !misyonVar);
        $('#vizyonMetin').text(!misyonVar ? '' : durum.vizyonVar ? durum.vizyon : "'Vizyon' tuşuna basarak vizyon bilgilerini giriniz.").toggleClass('bos', !(misyonVar && durum.vizyonVar));
        $('.strplan-kutu.misyon').prop('hidden', !secili);
        $('.strplan-kutu.vizyon, .strplan-kutu.amaclar').prop('hidden', !misyonVar);
        $('#btnVizyon, #btnAmacEkle').prop('disabled', !misyonVar);
        $('#btnHedefEkle').prop('disabled', !misyonVar || !durum.amaclar.length);
        $('.strplan-arac [data-rapor]').prop('disabled', !misyonVar || !durum.amaclar.length);
        amaclariCiz();
    }

    function durumYukle() {
        return Sgb.al('api/strplan/durum')
            .done(function (d) {
                durum = d.secili ? d : { secili: false, amaclar: [] };
                durumCiz();
            })
            .fail(hataGoster);
    }

    // ---------------------------------------------------------------- dönem seçimi

    function kurumlariDoldur($ddl, kurumlar, secili) {
        $ddl.empty();
        $.each(kurumlar, function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($ddl); });
        if (secili) $ddl.val(secili);
        if (!$ddl.val() && kurumlar.length) $ddl.val(kurumlar[0].kod);
    }

    function donemleriYukle(secili) {
        var $ddl = $('#ddlDonemYil').empty();
        var kurum = $('#ddlDonemKurum').val();
        if (!kurum) return $.Deferred().resolve().promise();
        return Sgb.al('api/strplan/donemler', { kurum: kurum })
            .done(function (veri) {
                $.each(veri.satirlar, function (i, d) { $('<option></option>').val(d.yil).text(d.ad).appendTo($ddl); });
                if (secili) $ddl.val(String(secili));
                if (!$ddl.val() && veri.satirlar.length) $ddl.val(String(veri.satirlar[0].yil));
            })
            .fail(hataGoster);
    }

    $('#btnDonem').on('click', function () {
        Sgb.al('api/strplan/donem-tanim')
            .done(function (t) {
                kurumlariDoldur($('#ddlDonemKurum'), t.kurumlar, t.kurumKod);
                donemleriYukle(t.yil);
                pencere('dlgDonem').showModal();
            })
            .fail(hataGoster);
    });
    $('#ddlDonemKurum').on('change', function () { donemleriYukle(0); });

    $('#btnDonemSec').on('click', function () {
        var istek = { kurumKod: $('#ddlDonemKurum').val() || '', yil: parseInt($('#ddlDonemYil').val(), 10) || 0 };
        if (!istek.kurumKod || !istek.yil) { mesaj('Kurum ve dönem seçiniz.', 'warning'); return; }
        Sgb.gonder('api/strplan/donem-sec', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                pencere('dlgDonem').close();
                hedefTanim = null;
                durumYukle();
            })
            .fail(hataGoster);
    });

    // ---------------------------------------------------------------- misyon

    function misyonYukle(kurum, yil) {
        var sorgu = {};
        if (kurum) sorgu.kurum = kurum;
        if (yil) sorgu.yil = yil;
        return Sgb.al('api/strplan/misyon', sorgu)
            .done(function (m) {
                if (m.kurumKod) $('#ddlMisyonKurum').val(m.kurumKod);
                $('#txtMisyonYil').val(m.yil);
                $('#txtMisyonBas').val(m.basYil);
                $('#txtMisyonSon').val(m.sonYil);
                // Kayıtlı misyon yoksa yazılmış metin korunur (aynı metinle yeni bir plan yılı açılabilsin).
                if (m.bulundu || !$('#txtMisyon').val()) {
                    $('#txtMisyon').val(m.misyon || '');
                    $('#txtMisyonAciklama').val(m.aciklama || '');
                }
                $('#txtKaynakYil').val(m.yil);
                $('#txtHedefYil').val(m.yil + 1);
            })
            .fail(hataGoster);
    }

    $('#btnMisyon').on('click', function () {
        Sgb.al('api/strplan/donem-tanim')
            .done(function (t) {
                kurumlariDoldur($('#ddlMisyonKurum'), t.kurumlar, t.kurumKod);
                $('#txtMisyon, #txtMisyonAciklama').val('');
                misyonYukle($('#ddlMisyonKurum').val(), t.yil || 0).done(function () { pencere('dlgMisyon').showModal(); });
            })
            .fail(hataGoster);
    });
    // Kurum ya da plan yılı değişince o planın kayıtlı misyonu getirilir.
    $('#ddlMisyonKurum, #txtMisyonYil').on('change', function () {
        var yil = parseInt($('#txtMisyonYil').val(), 10) || 0;
        if (yil >= 2000) misyonYukle($('#ddlMisyonKurum').val(), yil);
    });

    $('#btnMisyonKaydet').on('click', function () {
        var istek = {
            kurumKod: $('#ddlMisyonKurum').val() || '', yil: parseInt($('#txtMisyonYil').val(), 10) || 0,
            basYil: parseInt($('#txtMisyonBas').val(), 10) || 0, sonYil: parseInt($('#txtMisyonSon').val(), 10) || 0,
            misyon: $.trim($('#txtMisyon').val()), aciklama: $.trim($('#txtMisyonAciklama').val())
        };
        if (!istek.kurumKod) { mesaj('Kurum seçiniz.', 'warning'); return; }
        if (!istek.yil) { mesaj('Plan yılı boş bırakılamaz.', 'warning'); $('#txtMisyonYil').trigger('focus'); return; }
        if (!istek.misyon) { mesaj('Misyonu yazınız.', 'warning'); $('#txtMisyon').trigger('focus'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/strplan/misyon-kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                pencere('dlgMisyon').close();
                mesaj(s.bilgi, 'success');
                hedefTanim = null;
                durumYukle();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnKopyala').on('click', function () {
        var istek = { kaynakYil: parseInt($('#txtKaynakYil').val(), 10) || 0, hedefYil: parseInt($('#txtHedefYil').val(), 10) || 0 };
        if (!istek.kaynakYil || !istek.hedefYil || istek.kaynakYil === istek.hedefYil) { mesaj('Kaynak ve hedef yılı yazınız.', 'warning'); return; }
        if (!window.confirm(istek.kaynakYil + ' yılının stratejik plan bilgileri ' + istek.hedefYil + ' yılına kopyalanacak. Onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/strplan/misyon-kopyala', istek)
            .done(function (s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- vizyon

    $('#btnVizyon').on('click', function () {
        Sgb.al('api/strplan/vizyon')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $('#txtVizyon').val(v.vizyon || '');
                $('#txtVizyonAciklama').val(v.aciklama || '');
                pencere('dlgVizyon').showModal();
            })
            .fail(hataGoster);
    });

    $('#btnVizyonKaydet').on('click', function () {
        var istek = { vizyon: $.trim($('#txtVizyon').val()), aciklama: $.trim($('#txtVizyonAciklama').val()) };
        if (!istek.vizyon) { mesaj('Vizyonu yazınız.', 'warning'); $('#txtVizyon').trigger('focus'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/strplan/vizyon-kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                pencere('dlgVizyon').close();
                mesaj(s.bilgi, 'success');
                durumYukle();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- amaç

    function amacDoldur(a) {
        $('#txtAmacNo').val(a.kod || '');
        $('#txtAmacKisa').val(a.kisa || '');
        $('#txtAmac').val(a.ad || '');
        $('#txtAmacAciklama').val(a.aciklama || '');
        $('#btnAmacSil').prop('disabled', !a.kod);
        $('#dlgAmacBaslik').text(a.kod ? 'Amaç' : 'Yeni Amaç');
    }

    function amacAc(kod) {
        if (!kod) { amacDoldur({}); pencere('dlgAmac').showModal(); return; }
        Sgb.al('api/strplan/amac', { kod: kod })
            .done(function (a) {
                if (a.hata) { mesaj(a.hata, 'danger'); return; }
                amacDoldur(a);
                pencere('dlgAmac').showModal();
            })
            .fail(hataGoster);
    }

    $('#btnAmacEkle').on('click', function () { amacAc(0); });
    $('#btnAmacYeni').on('click', function () { amacDoldur({}); $('#txtAmacKisa').trigger('focus'); });

    $('#btnAmacKaydet').on('click', function () {
        var istek = { kod: parseInt($('#txtAmacNo').val(), 10) || 0, kisa: $.trim($('#txtAmacKisa').val()), ad: $.trim($('#txtAmac').val()), aciklama: $.trim($('#txtAmacAciklama').val()) };
        if (!istek.ad) { mesaj('Amacı yazınız.', 'warning'); $('#txtAmac').trigger('focus'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/strplan/amac-kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                $('#txtAmacNo').val(s.kod);
                $('#btnAmacSil').prop('disabled', false);
                $('#dlgAmacBaslik').text('Amaç');
                hedefTanim = null;
                durumYukle();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnAmacSil').on('click', function () {
        var kod = parseInt($('#txtAmacNo').val(), 10) || 0;
        if (!kod || !window.confirm('Amaç silinecek. Onaylıyor musunuz?')) return;
        Sgb.gonder('api/strplan/amac-sil', { kod: kod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                pencere('dlgAmac').close();
                mesaj(s.bilgi, 'success');
                hedefTanim = null;
                durumYukle();
            })
            .fail(hataGoster);
    });

    // ---------------------------------------------------------------- hedef: satır listeleri

    // Tek metinlik satır (risk, strateji, tespit, ihtiyaç): metin kutusu + sil.
    function metinSatiri($liste, metin) {
        var $satir = $('<div class="satir"></div>').appendTo($('<li></li>').appendTo($liste));
        $('<input type="text" class="form-control input-sm" />').attr('aria-label', $liste.attr('data-etiket')).val(metin || '').appendTo($satir);
        dugme('trash', 'Sil', 'btn-danger').on('click', function () { $(this).closest('li').remove(); }).appendTo($satir);
        return $satir;
    }
    function metinleriDoldur(kimlik, metinler) {
        var $liste = $('#' + kimlik).empty();
        $.each(metinler || [], function (i, m) { metinSatiri($liste, m); });
    }
    function metinleriOku(kimlik) {
        return $('#' + kimlik + ' input').map(function () { return $.trim(this.value); }).get().filter(function (m) { return !!m; });
    }
    $('#dlgHedef').on('click', '[data-metin-ekle]', function () {
        metinSatiri($('#' + $(this).attr('data-metin-ekle')), '').find('input').trigger('focus');
    });

    // Birim satırı: ad + sil. Kayıtlı birim kendi koduyla, listeden eklenen "kod / ad" biçiminde tutulur.
    function birimSatiri(b) {
        var $li = $('<li></li>').data('birim', b).appendTo('#lstBirimler');
        var $satir = $('<div class="satir"></div>').appendTo($li);
        $('<span class="ad"></span>').text(b.ad).appendTo($satir);
        dugme('trash', 'Sil: ' + b.ad, 'btn-danger').on('click', function () { $li.remove(); }).appendTo($satir);
    }
    $('#btnBirimEkle').on('click', function () {
        var ad = $('#ddlBirimEkle').val();
        if (!ad) { mesaj('Eklenecek birimi seçiniz.', 'warning'); return; }
        var var_ = $('#lstBirimler li').filter(function () { var b = $(this).data('birim'); return b.ad === ad || (b.kod && ad.indexOf(b.kod + ' /') === 0); }).length;
        if (var_) { mesaj('Bu birim zaten eklenmiş.', 'warning'); return; }
        birimSatiri({ kod: '', ad: ad });
    });

    // Üst politika belgesi satırı: belge + referans + açıklama + sil.
    function ustBelgeSatiri(u) {
        var $tr = $('<tr></tr>').appendTo('#tblUstBelgeler');
        var $ddl = $('<select class="form-control" aria-label="Üst belge"></select>').append('<option value=""></option>');
        $.each((hedefTanim || {}).ustBelgeler || [], function (i, b) { $('<option></option>').val(b).text(b).appendTo($ddl); });
        // Kayıtlı belge listede yoksa seçenek olarak eklenir ki değer kaybolmasın.
        if (u.kodAd && !$ddl.find('option').filter(function () { return this.value === u.kodAd; }).length) $('<option></option>').val(u.kodAd).text(u.kodAd).appendTo($ddl);
        $('<td></td>').append($ddl.val(u.kodAd || '')).appendTo($tr);
        $('<td></td>').append($('<input type="text" class="form-control" aria-label="Referans" />').val(u.referans || '')).appendTo($tr);
        $('<td></td>').append($('<input type="text" class="form-control" aria-label="Açıklama" />').val(u.aciklama || '')).appendTo($tr);
        $('<td></td>').append(dugme('trash', 'Sil', 'btn-danger').on('click', function () { $tr.remove(); })).appendTo($tr);
    }
    $('#btnUstBelgeEkle').on('click', function () { ustBelgeSatiri({}); });

    $('#hedefSekmeler').on('click', '[role=tab]', function () {
        var ad = $(this).attr('data-sekme');
        $('#hedefSekmeler [role=tab]').each(function () { $(this).attr('aria-selected', String($(this).attr('data-sekme') === ad)); });
        $('#dlgHedef .sekme-govde').each(function () { $(this).prop('hidden', $(this).attr('data-sekme') !== ad); });
    });

    // ---------------------------------------------------------------- hedef

    function hedefTanimYukle() {
        if (hedefTanim) return $.Deferred().resolve(hedefTanim).promise();
        return Sgb.al('api/strplan/hedef-tanim').then(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return $.Deferred().reject().promise(); }
            hedefTanim = t;
            var $amac = $('#ddlHedefAmac').empty().append('<option value=""></option>');
            $.each(t.amaclar, function (i, a) { $('<option></option>').val(a.kod).text(a.ad).appendTo($amac); });
            var $birim = $('#ddlBirimEkle').empty().append($('<option value=""></option>').text('Eklenecek birimi seçiniz.'));
            $.each(t.birimler, function (i, b) { $('<option></option>').val(b).text(b).appendTo($birim); });
            var $gosterge = $('#ddlGostergeEkle').empty().append($('<option value=""></option>').text('Eklenecek göstergeyi seçiniz.'));
            $.each(t.gostergeler, function (i, g) { $('<option></option>').val(g.kod).text(g.ad).appendTo($gosterge); });
            $('#hedefUyari').text(t.uyari || '').prop('hidden', !t.uyari);
            return t;
        });
    }

    function hedefDoldur(h) {
        acikHedef = h.kod || 0;
        $('#txtHedefNo').val(h.kod || '');
        $('#ddlHedefAmac').val(h.amacKod ? String(h.amacKod) : '');
        $('#txtHedefKisa').val(h.kisa || '');
        $('#txtHedef').val(h.ad || '');
        $('#txtHedefAciklama').val(h.aciklama || '');
        $('#lstBirimler').empty();
        $.each(h.birimler || [], function (i, b) { birimSatiri(b); });
        metinleriDoldur('lstRiskler', h.riskler);
        metinleriDoldur('lstStratejiler', h.stratejiler);
        metinleriDoldur('lstTespitler', h.tespitler);
        metinleriDoldur('lstIhtiyaclar', h.ihtiyaclar);
        $('#txtProgramAd').val(h.programAd || '');
        $('#txtAltProgramHedef').val(h.altProgramHedef || '');
        $('#tblUstBelgeler').empty();
        $.each(h.ustBelgeler || [], function (i, u) { ustBelgeSatiri(u); });
        hedefDugmeleri();
        $('#dlgHedefBaslik').text(acikHedef ? 'Hedef' : 'Yeni Hedef');
    }

    // Göstergeler ve ödenekler yalnızca kaydedilmiş hedefte girilir.
    function hedefDugmeleri() {
        $('#btnHedefSil, #btnGostergeler, #btnOdenekler, #btnHedefYazdir').prop('disabled', !acikHedef);
    }

    function hedefAc(kod, amacKod) {
        hedefTanimYukle().done(function () {
            $('#hedefSekmeler [role=tab]').first().trigger('click');
            if (!kod) { hedefDoldur({ amacKod: amacKod || 0 }); pencere('dlgHedef').showModal(); return; }
            Sgb.al('api/strplan/hedef', { kod: kod })
                .done(function (h) {
                    if (h.hata) { mesaj(h.hata, 'danger'); return; }
                    hedefDoldur(h);
                    pencere('dlgHedef').showModal();
                })
                .fail(hataGoster);
        });
    }

    $('#btnHedefEkle').on('click', function () { hedefAc(0, 0); });
    $('#btnHedefYeni').on('click', function () { hedefDoldur({ amacKod: parseInt($('#ddlHedefAmac').val(), 10) || 0 }); $('#txtHedefKisa').trigger('focus'); });

    $('#btnHedefKaydet').on('click', function () {
        var istek = {
            kod: acikHedef, amacKod: parseInt($('#ddlHedefAmac').val(), 10) || 0, kisa: $.trim($('#txtHedefKisa').val()),
            ad: $.trim($('#txtHedef').val()), aciklama: $.trim($('#txtHedefAciklama').val()),
            birimler: $('#lstBirimler li').map(function () { return $(this).data('birim'); }).get(),
            riskler: metinleriOku('lstRiskler'), stratejiler: metinleriOku('lstStratejiler'),
            tespitler: metinleriOku('lstTespitler'), ihtiyaclar: metinleriOku('lstIhtiyaclar'),
            programAd: $.trim($('#txtProgramAd').val()), altProgramHedef: $.trim($('#txtAltProgramHedef').val()),
            ustBelgeler: $('#tblUstBelgeler tr').map(function () {
                var $tr = $(this);
                return { kodAd: $tr.find('select').val() || '', referans: $.trim($tr.find('input').eq(0).val()), aciklama: $.trim($tr.find('input').eq(1).val()) };
            }).get().filter(function (u) { return !!u.kodAd; })
        };
        if (!istek.amacKod) { mesaj('Hedefin amacını seçiniz.', 'warning'); $('#ddlHedefAmac').trigger('focus'); return; }
        if (!istek.ad) { mesaj('Hedefi yazınız.', 'warning'); $('#txtHedef').trigger('focus'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/strplan/hedef-kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                acikHedef = s.kod;
                $('#txtHedefNo').val(s.kod);
                $('#dlgHedefBaslik').text('Hedef');
                hedefDugmeleri();
                durumYukle();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnHedefSil').on('click', function () {
        if (!acikHedef || !window.confirm('Hedef silinecek. Onaylıyor musunuz?')) return;
        Sgb.gonder('api/strplan/hedef-sil', { kod: acikHedef })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                pencere('dlgHedef').close();
                mesaj(s.bilgi, 'success');
                durumYukle();
            })
            .fail(hataGoster);
    });

    // ---------------------------------------------------------------- raporlar

    // Dosya hazırlanırken düğme kapalı kalır (bütün hedef kartları birkaç saniye sürebilir).
    function raporIndir($dugme, adres, parametreler) {
        $dugme.prop('disabled', true);
        Sgb.indir(adres, parametreler).then(function () { $dugme.prop('disabled', false); });
    }

    $('.strplan-arac').on('click', '[data-rapor]', function () {
        raporIndir($(this), 'api/strplan/rapor/' + $(this).attr('data-rapor'));
    });

    // Hedef penceresindeki Yazdır: açık hedefin kartı (kaydedilmiş haliyle).
    $('#btnHedefYazdir').on('click', function () {
        if (acikHedef) raporIndir($(this), 'api/strplan/rapor/hedef-karti', { kod: acikHedef });
    });

    // ---------------------------------------------------------------- hedefin göstergeleri

    var TAHMINLER = ['tahmin0', 'tahmin1', 'tahmin2', 'tahmin3', 'tahmin4'];

    function sayiKutusu(deger, etiket) {
        return $('<input type="number" class="form-control" step="any" />').attr('aria-label', etiket).val(deger === undefined || deger === null ? 0 : deger);
    }

    function donemSecimi(deger, etiket) {
        var $ddl = $('<select class="form-control"></select>').attr('aria-label', etiket).append('<option value="0"></option>');
        $.each(hedefTanim.donemler, function (i, d) { $('<option></option>').val(d.kod).text(d.ad).appendTo($ddl); });
        return $ddl.val(String(deger || 0));
    }

    function gostergeSatiri(g, basliklar) {
        var $tr = $('<tr></tr>').data('gosterge', { gostergeKod: g.gostergeKod, gostergeAd: g.gostergeAd }).appendTo('#tblGosterge tbody');
        $('<td></td>').text(g.gostergeKod).appendTo($tr);
        $('<td class="gosterge-ad"></td>').text(g.gostergeAd).appendTo($tr);
        $('<td class="dar-sutun"></td>').append($('<input type="text" class="form-control" data-alan="gorKod" />').attr('aria-label', 'Gösterge kodu').val(g.gorKod || '')).appendTo($tr);
        $('<td></td>').append(sayiKutusu(g.hedefeEtkisi, 'Hedefe etkisi (%)').attr('data-alan', 'hedefeEtkisi')).appendTo($tr);
        $('<td></td>').append(sayiKutusu(g.gerceklesen, basliklar[0]).attr('data-alan', 'gerceklesen')).appendTo($tr);
        $.each(TAHMINLER, function (i, alan) { $('<td></td>').append(sayiKutusu(g[alan], basliklar[i + 1]).attr('data-alan', alan)).appendTo($tr); });
        $('<td class="dar-sutun"></td>').append(donemSecimi(g.izlemeDonemi, 'İzleme sıklığı').attr('data-alan', 'izlemeDonemi')).appendTo($tr);
        $('<td class="dar-sutun"></td>').append(donemSecimi(g.raporlamaDonemi, 'Raporlama sıklığı').attr('data-alan', 'raporlamaDonemi')).appendTo($tr);
        $('<td></td>').append(dugme('trash', 'Sil: ' + g.gostergeAd, 'btn-danger').on('click', function () { $tr.remove(); })).appendTo($tr);
    }

    var gostergeBasliklar = [];

    $('#btnGostergeler').on('click', function () {
        if (!acikHedef) return;
        Sgb.al('api/strplan/gostergeler', { hedef: acikHedef })
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                gostergeBasliklar = [(veri.basYil - 1) + ' Dönem Başı'];
                for (var i = 0; i < 5; i++) gostergeBasliklar.push((veri.basYil + i) + ' Planlanan');
                var $bas = $('<tr></tr>').appendTo($('#tblGosterge thead').empty());
                $.each(['Kod', 'Gösterge', 'Gös. Kodu', 'Etkisi (%)'].concat(gostergeBasliklar, ['İzleme Sıklığı', 'Raporlama Sıklığı']), function (i, b) { $('<th scope="col"></th>').text(b).appendTo($bas); });
                $('<th scope="col"><span class="sr-only">Sil</span></th>').appendTo($bas);
                $('#tblGosterge tbody').empty();
                $.each(veri.satirlar, function (i, g) { gostergeSatiri(g, gostergeBasliklar); });
                $('#dlgGostergeBaslik').text('Hedef Gösterge Tablosu: ' + $.trim($('#txtHedefKisa').val()));
                pencere('dlgGosterge').showModal();
            })
            .fail(hataGoster);
    });

    $('#btnGostergeEkle').on('click', function () {
        var kod = parseInt($('#ddlGostergeEkle').val(), 10) || 0;
        if (!kod) { mesaj('Eklenecek göstergeyi seçiniz.', 'warning'); return; }
        var var_ = $('#tblGosterge tbody tr').filter(function () { return $(this).data('gosterge').gostergeKod === kod; }).length;
        if (var_) { mesaj('Bu gösterge zaten eklenmiş.', 'warning'); return; }
        gostergeSatiri({ gostergeKod: kod, gostergeAd: $('#ddlGostergeEkle option:selected').text() }, gostergeBasliklar);
    });

    $('#btnGostergeKaydet').on('click', function () {
        var satirlar = $('#tblGosterge tbody tr').map(function () {
            var $tr = $(this), s = $.extend({}, $tr.data('gosterge'));
            $tr.find('[data-alan]').each(function () {
                var alan = $(this).attr('data-alan');
                s[alan] = alan === 'gorKod' ? $.trim(this.value) : alan.indexOf('Donemi') > 0 ? (parseInt(this.value, 10) || 0) : sayi(this.value);
            });
            return s;
        }).get();
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/strplan/gosterge-kaydet', { hedef: acikHedef, satirlar: satirlar })
            .done(function (s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- hedefin ödenekleri (tahmini maliyet)

    var ODENEKLER = ['d0', 'd1', 'd2', 'd3', 'd4'];
    var paraBicimi = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 2 });

    function odenekToplamlari() {
        var $alt = $('<tr></tr>').appendTo($('#tblOdenek tfoot').empty());
        $('<td colspan="2">Toplam</td>').appendTo($alt);
        var genel = 0;
        $.each(ODENEKLER, function (i, alan) {
            var toplam = 0;
            $('#tblOdenek tbody [data-alan="' + alan + '"]').each(function () { toplam += sayi(this.value); });
            genel += toplam;
            $('<td class="sayi"></td>').text(paraBicimi.format(toplam)).appendTo($alt);
        });
        $('<td class="sayi"></td>').text(paraBicimi.format(genel)).appendTo($alt);
    }

    $('#btnOdenekler').on('click', function () {
        if (!acikHedef) return;
        Sgb.al('api/strplan/odenekler', { hedef: acikHedef })
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                var $bas = $('<tr></tr>').appendTo($('#tblOdenek thead').empty());
                $('<th scope="col">Kod</th><th scope="col">Ekonomik Sınıflandırma</th>').appendTo($bas);
                for (var i = 0; i < 5; i++) $('<th scope="col" class="sayi"></th>').text((veri.basYil + i) + ' Ödeneği').appendTo($bas);
                $('<th scope="col" class="sayi">Toplam</th>').appendTo($bas);
                var $govde = $('#tblOdenek tbody').empty();
                $.each(veri.satirlar, function (j, o) {
                    var $tr = $('<tr></tr>').data('odenek', { ekoKod: o.ekoKod, ekoAd: o.ekoAd }).appendTo($govde);
                    $('<td></td>').text(o.ekoKod).appendTo($tr);
                    $('<td></td>').text(o.ekoAd).appendTo($tr);
                    $.each(ODENEKLER, function (k, alan) {
                        $('<td></td>').append(sayiKutusu(o[alan], o.ekoAd + ' ' + (veri.basYil + k) + ' ödeneği').attr('data-alan', alan)).appendTo($tr);
                    });
                    $('<td class="sayi satir-toplam"></td>').appendTo($tr);
                });
                odenekSatirToplamlari();
                odenekToplamlari();
                $('#dlgOdenekBaslik').text('Hedef Maliyet Tablosu: ' + $.trim($('#txtHedefKisa').val()));
                pencere('dlgOdenek').showModal();
            })
            .fail(hataGoster);
    });

    function odenekSatirToplamlari() {
        $('#tblOdenek tbody tr').each(function () {
            var toplam = 0;
            $(this).find('[data-alan]').each(function () { toplam += sayi(this.value); });
            $(this).find('.satir-toplam').text(paraBicimi.format(toplam));
        });
    }
    $('#tblOdenek').on('input', 'input', function () { odenekSatirToplamlari(); odenekToplamlari(); });

    $('#btnOdenekKaydet').on('click', function () {
        var satirlar = $('#tblOdenek tbody tr').map(function () {
            var $tr = $(this), s = $.extend({}, $tr.data('odenek'));
            $tr.find('[data-alan]').each(function () { s[$(this).attr('data-alan')] = sayi(this.value); });
            return s;
        }).get();
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/strplan/odenek-kaydet', { hedef: acikHedef, satirlar: satirlar })
            .done(function (s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açılış

    durumYukle();
});
