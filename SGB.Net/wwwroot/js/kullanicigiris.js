// Kullanıcı Giriş (KYM/KullaniciGiris): kullanıcı bilgileri, kullanıcı tipleri ve yetki değerleri.
// Eski sayfa düzenlenen kullanıcıyı sunucu oturumunda tutuyordu; burada tipler ve yetki değerleri bu betikte tutulur
// ve Kaydet'te birlikte gönderilir (sunucu, yapanın yetkisine göre uygular).
$(function () {
    'use strict';

    if (!$('#frmKullanici').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var M = JSON.parse($('#metinler').text() || '{}');
    var ALANLAR = {
        tc: '#txtTc', dogum: '#txtDogum', ad: '#txtAd', soyad: '#txtSoyad', kod: '#txtKod', eposta: '#txtEposta', telefon: '#txtTelefon',
        unvan: '#txtUnvan', akademikUnvan: '#txtAkademikUnvan', birim: '#txtBirim', kurum: '#ddlKurum', rumuz: '#txtRumuz',
        sicilNo: '#txtSicilNo', sertNo: '#txtSertNo'
    };
    var ZORUNLU = ['tc', 'dogum', 'ad', 'soyad', 'kod', 'eposta', 'telefon', 'unvan', 'kurum'];
    var LISTEDEN = 1, YAZIYLA = 2, AGACTAN = 3;

    // Düzenlenen kullanıcının durumu
    var kullanici = { yeni: true, kod: '', aktif: true, tipler: [], yetkiler: {} };
    var tanimlar = { eklenebilirTipler: [], varsayilanKurum: '' };

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(M.IslemHata, 'danger');
    }
    function mesgul(evet) {
        $('#btnKaydet').prop('disabled', evet || (!kullanici.yeni && !kullanici.aktif));
        $('#btnParola').prop('disabled', evet || kullanici.yeni || !kullanici.aktif || tanimlar.entegrasyon);
        $('#btnDurum').prop('disabled', evet || kullanici.yeni || tanimlar.entegrasyon);
        $('#btnGorevler').prop('disabled', evet || kullanici.yeni);
    }
    function pencereKapat() { $(this).closest('dialog')[0].close(); }
    $('dialog').on('click', '[data-kapat]', pencereKapat);

    // ---------------------------------------------------------------- kullanıcı bilgileri

    function formDoldur(k) {
        $.each(ALANLAR, function (ad, kutu) { $(kutu).val(k[ad] || ''); });
        // Kayıtlı kurum listede yoksa (ör. yöneticinin göremediği bir birim) seçenek olarak eklenir ki değer kaybolmasın.
        if (k.kurum && $('#ddlKurum').val() !== k.kurum) {
            $('<option></option>').val(k.kurum).text(k.kurum).appendTo('#ddlKurum');
            $('#ddlKurum').val(k.kurum);
        }
    }

    function durumGoster() {
        $('#txtKod').prop('readonly', !kullanici.yeni);
        $('#btnDurumMetin').text(kullanici.aktif ? M.FRMKUG024 : M.FRMKUG025);
        mesgul(false);
    }

    function yukle(kod) {
        return Sgb.al('api/kullanici/oku', { kod: kod })
            .done(function (k) {
                if (k.hata) { mesaj(k.hata, 'danger'); return; }
                kullanici = { yeni: false, kod: k.kod, aktif: k.aktif, tipler: k.tipler, yetkiler: k.yetkiler || {} };
                formDoldur(k);
                tipleriCiz();
                durumGoster();
            })
            .fail(hataGoster);
    }

    function formOku() {
        var istek = { yeni: kullanici.yeni };
        $.each(ALANLAR, function (ad, kutu) { istek[ad] = $.trim($(kutu).val() || ''); });
        if (!kullanici.yeni) istek.kod = kullanici.kod;
        istek.tipler = $.map(kullanici.tipler, function (t) { return t.kod; });
        istek.yetkiler = kullanici.yetkiler;
        return istek;
    }

    function zorunluDenetle(istek) {
        var eksik = null;
        $.each(ZORUNLU, function (i, ad) {
            // Doğum tarihi yalnızca yeni kullanıcıda zorunludur (boş bırakılırsa kayıtlı tarih korunur).
            var bos = !istek[ad] && !(ad === 'dogum' && !kullanici.yeni);
            $(ALANLAR[ad]).closest('.form-group').toggleClass('has-error', bos);
            if (bos && !eksik) eksik = ALANLAR[ad];
        });
        if (!eksik) return true;
        mesaj(istek.kurum || eksik !== '#ddlKurum' ? M.FRMKUG057 : M.FRMKUG027, 'warning');
        $(eksik).trigger('focus');
        return false;
    }

    function onerileriGoster(oneriler) {
        var $liste = $('#lstOneri').empty();
        $.each(oneriler || [], function (i, o) { $('<option></option>').val(o).appendTo($liste); });
        if (oneriler && oneriler.length) mesaj(M.OneriBaslik + ': ' + oneriler.join(', '), 'info');
    }

    $('#btnKaydet').on('click', function () {
        var istek = formOku();
        if (!zorunluDenetle(istek)) return;
        mesgul(true);
        Sgb.gonder('api/kullanici/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) {
                    mesaj(s.hata, 'danger');
                    onerileriGoster(s.oneriler);
                    return;
                }
                if (kullanici.yeni) {
                    // Yeni kullanıcı kaydedildi: aynı sayfa o kullanıcıyla açılır (parola atama, pasif yapma açılsın).
                    window.location.href = 'KullaniciGiris?kullaniciKodu=' + encodeURIComponent(s.kod);
                    return;
                }
                mesaj(s.bilgi, 'success');
                yukle(s.kod);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnKimlik').on('click', function () {
        var tc = $.trim($('#txtTc').val()), dogum = $('#txtDogum').val();
        if (!tc || !dogum) { mesaj(M.FRMKUG057, 'warning'); return; }
        $('#btnKimlik').prop('disabled', true);
        Sgb.al('api/kullanici/mernis', { tc: tc, dogum: dogum })
            .done(function (s) {
                if (s.hata) { mesaj(s.hata, 'danger'); return; }
                $('#txtAd').val(s.ad || '');
                $('#txtSoyad').val(s.soyad || '');
                if (kullanici.yeni) onerileriGoster(s.oneriler);
            })
            .fail(hataGoster)
            .always(function () { $('#btnKimlik').prop('disabled', false); });
    });

    $('#btnParola').on('click', function () {
        if (!window.confirm(M.FRMKUG056)) return;
        mesgul(true);
        Sgb.gonder('api/kullanici/parola-ata', { kod: kullanici.kod })
            .done(function (s) { mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger'); })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    function durumDegistir(tarih) {
        mesgul(true);
        Sgb.gonder('api/kullanici/durum', { kod: kullanici.kod, kurum: $('#ddlKurum').val(), eposta: $.trim($('#txtEposta').val()), tarih: tarih || '' })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                yukle(kullanici.kod);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    }

    $('#btnDurum').on('click', function () {
        if (!window.confirm(M.FRMKUG055)) return;
        if (!kullanici.aktif) { durumDegistir(''); return; }
        // Pasif yapılırken, istenirse yeniden aktif olacağı tarih sorulur.
        $('#txtAktifTarih').val('');
        document.getElementById('dlgPasif').showModal();
    });

    $('#btnPasifYap').on('click', function () {
        document.getElementById('dlgPasif').close();
        durumDegistir($('#txtAktifTarih').val());
    });

    // Kullanıcının (T.C. kimlik numarası üzerinden) görevleri: Göreve Kişi Atama sayfası pencerede açılır.
    $('#btnGorevler').on('click', function () {
        if (kullanici.yeni) return;
        Sgb.pencere('../Organizasyon/GorevAta?kullanici=' + encodeURIComponent(kullanici.kod), 'Görevler: ' + $.trim($('#txtAd').val() + ' ' + $('#txtSoyad').val()));
    });

    // ---------------------------------------------------------------- kullanıcı tipleri

    function tipBul(kod) {
        for (var i = 0; i < kullanici.tipler.length; i++) if (kullanici.tipler[i].kod === kod) return kullanici.tipler[i];
        return null;
    }

    function dugme(simge, metin, sinif) {
        var $d = $('<button type="button" class="btn btn-xs"></button>').addClass(sinif).attr('title', metin).attr('aria-label', metin);
        $('<span class="glyphicon" aria-hidden="true"></span>').addClass('glyphicon-' + simge).appendTo($d);
        return $d;
    }

    function tipleriCiz() {
        var $liste = $('#lstTipler').empty();
        if (!kullanici.tipler.length) {
            $('<li></li>').append($('<span class="ad bos"></span>').text(M.TipYok)).appendTo($liste);
            return;
        }
        $.each(kullanici.tipler, function (i, t) {
            var $li = $('<li></li>').appendTo($liste);
            $('<span class="ad"></span>').text(t.ad).appendTo($li);
            if (t.duzenlenebilir) dugme('pencil', M.Duzenle, 'btn-default').on('click', function () { tipAc(t.kod, false); }).appendTo($li);
            if (t.silinebilir) dugme('trash', M.FRMKUG012, 'btn-danger').on('click', function () { tipSil(t.kod); }).appendTo($li);
        });
    }

    function tipSil(kod) {
        if (!window.confirm(M.FRMKUG053)) return;
        kullanici.tipler = $.grep(kullanici.tipler, function (t) { return t.kod !== kod; });
        tipleriCiz();
    }

    $('#btnTipEkle').on('click', function () {
        var kod = parseInt($('#ddlTipEkle').val(), 10);
        if (isNaN(kod)) { mesaj(M.TipSec, 'warning'); $('#ddlTipEkle').trigger('focus'); return; }
        if (tipBul(kod)) { mesaj(M.TipZatenVar, 'warning'); return; }
        tipAc(kod, true);
    });

    // ---------------------------------------------------------------- bir tipin yetki değerleri

    var acikTip = null; // { tip, ad, yeni, yetkiler: [{kod, ad, giris, cok, deger, secili}] }

    function etiketleriCiz($kutu, y) {
        $kutu.empty();
        if (!y.secili.length) { $('<span class="bos"></span>').text(M.SecilenYok).appendTo($kutu); return; }
        $.each(y.secili, function (i, s) {
            var $e = $('<span class="tanim-etiket"></span>').text(s.ad || s.kod).attr('title', s.kod).appendTo($kutu);
            $('<button type="button">&times;</button>').attr('aria-label', M.Kaldir + ': ' + (s.ad || s.kod)).on('click', function () {
                y.secili = $.grep(y.secili, function (x) { return x.kod !== s.kod; });
                etiketleriCiz($kutu, y);
            }).appendTo($e);
        });
    }

    function tipAc(kod, yeni) {
        Sgb.gonder('api/kullanici/tip-yetkileri', { tip: kod, degerler: kullanici.yetkiler })
            .done(function (t) {
                if (t.hata) { mesaj(t.hata, 'danger'); return; }
                acikTip = { tip: t.tip, ad: t.ad, yeni: yeni, yetkiler: t.yetkiler };
                $('#dlgTipBaslik').text(t.ad);
                $('#tipYetkiYok').text(t.ad + ' ' + M.FRMKYG010).prop('hidden', t.yetkiler.length > 0);
                var $govde = $('#tipYetkiler').empty();
                $.each(t.yetkiler, function (i, y) {
                    var kimlik = 'yt' + y.kod;
                    var $satir = $('<div class="yetki-satir"></div>').appendTo($govde);
                    $('<label></label>').attr('for', kimlik).text(y.ad).appendTo($satir);
                    if (y.giris === YAZIYLA) {
                        $('<input type="text" class="form-control input-sm" />').attr('id', kimlik).val(y.deger || '')
                            .on('input', function () { y.deger = $(this).val(); }).appendTo($satir);
                        $('<span></span>').appendTo($satir);
                    } else {
                        var $kutu = $('<div class="tanim-etiketler"></div>').appendTo($satir);
                        etiketleriCiz($kutu, y);
                        $('<button type="button" class="btn btn-default btn-sm"></button>').attr('id', kimlik).text(M.Sec)
                            .on('click', function () { degerSec(y, function () { etiketleriCiz($kutu, y); }); }).appendTo($satir);
                    }
                });
                document.getElementById('dlgTip').showModal();
            })
            .fail(hataGoster);
    }

    $('#btnTipTamam').on('click', function () {
        if (!acikTip) return;
        $.each(acikTip.yetkiler, function (i, y) {
            var deger;
            if (y.giris === YAZIYLA) deger = $.trim(y.deger || '');
            // Seçilen kodlar eski sayfadaki gibi sonunda da ; olacak biçimde saklanır.
            else deger = $.map(y.secili, function (s) { return s.kod + ';'; }).join('');
            kullanici.yetkiler[String(y.kod)] = deger;
        });
        if (acikTip.yeni && !tipBul(acikTip.tip))
            kullanici.tipler.push({ kod: acikTip.tip, ad: acikTip.ad, duzenlenebilir: true, silinebilir: true });
        acikTip = null;
        document.getElementById('dlgTip').close();
        tipleriCiz();
    });

    // ---------------------------------------------------------------- değer seçme (liste ya da ağaç)

    var secim = null; // { y, bitince, secili: {kod: ad} }

    function secimIsaretle(kod, ad, isaretli) {
        if (!isaretli) { delete secim.secili[kod]; return; }
        if (!secim.y.cok) {
            // Tek değerli yetki: önceki seçim kalkar.
            secim.secili = {};
            $('#lstDeger input[type=checkbox]').not(function () { return $(this).data('kod') === kod; }).prop('checked', false);
        }
        secim.secili[kod] = ad;
    }

    // Listeden seçim: düz liste, üstteki kutuyla süzülür. (Ağaçtan seçim Sgb.agac ile çizilir.)
    function degerSatiri(s) {
        var kimlik = 'dg' + Math.random().toString(36).slice(2);
        var $li = $('<li></li>').attr('data-ad', (s.kod + ' ' + (s.ad || '')).toLocaleLowerCase('tr'));
        var $satir = $('<div class="satir"></div>').appendTo($li);
        $('<input type="checkbox" />').attr('id', kimlik).data('kod', s.kod).prop('checked', secim.secili.hasOwnProperty(s.kod))
            .on('change', function () { secimIsaretle(s.kod, s.ad || s.kod, this.checked); }).appendTo($satir);
        $('<label></label>').attr('for', kimlik).text(s.kod + ' - ' + (s.ad || '')).appendTo($satir);
        return $li;
    }

    function degerYukle($hedef) {
        return Sgb.al('api/kullanici/yetki-liste', { yt: secim.y.kod, ust: '' })
            .done(function (veri) { $.each(veri.satirlar, function (i, s) { degerSatiri(s).appendTo($hedef); }); })
            .fail(hataGoster);
    }

    function degerSec(y, bitince) {
        secim = { y: y, bitince: bitince, secili: {} };
        $.each(y.secili, function (i, s) { secim.secili[s.kod] = s.ad || s.kod; });
        var agac = y.giris === AGACTAN;
        $('#dlgDegerBaslik').text(y.ad);
        $('#txtDegerAra').val('').prop('hidden', agac);
        var $liste = $('#lstDeger').empty().removeClass('sgb-agac');
        if (agac) degerAgaci();
        else degerYukle($liste);
        document.getElementById('dlgDeger').showModal();
    }

    // Ağaçtan seçim ortak bileşenle çizilir (Sgb.agac, onay kutulu). Kökte "hepsi" anlamına gelen * değeri de seçilebilir.
    function degerAgaci() {
        var agac = Sgb.agac('#lstDeger', {
            yukle: function (ust) {
                return Sgb.al('api/kullanici/yetki-liste', { yt: secim.y.kod, ust: ust ? ust.kod : '' }).then(function (veri) {
                    var dugumler = $.map(veri.satirlar, function (s) { return { kod: s.kod, ad: s.ad || s.kod, altVar: !!s.cocuguVar }; });
                    if (!ust) dugumler.unshift({ kod: '*', ad: '* ' + M.FRMYTD008, altVar: false });
                    return dugumler;
                });
            },
            isaretli: function (d) { return !!secim && secim.secili.hasOwnProperty(d.kod); },
            isaretlenince: function (d, evet) {
                if (!evet) { delete secim.secili[d.kod]; return; }
                // Tek değerli yetki: önceki seçim kalkar.
                if (!secim.y.cok) secim.secili = {};
                secim.secili[d.kod] = d.ad;
                if (!secim.y.cok) agac.isaretleriGuncelle();
            },
            hata: hataGoster
        });
    }

    $('#txtDegerAra').on('input', function () {
        var aranan = $.trim($(this).val()).toLocaleLowerCase('tr');
        $('#lstDeger > li').each(function () { $(this).prop('hidden', !!aranan && String($(this).attr('data-ad')).indexOf(aranan) < 0); });
    });

    $('#btnDegerTamam').on('click', function () {
        if (!secim) return;
        secim.y.secili = $.map(secim.secili, function (ad, kod) { return { kod: kod, ad: ad }; });
        var bitince = secim.bitince;
        secim = null;
        document.getElementById('dlgDeger').close();
        bitince();
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/kullanici/giris-tanimlar')
        .done(function (t) {
            tanimlar = t;
            $.each(t.kurumlar, function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo('#ddlKurum'); });
            $.each(t.eklenebilirTipler, function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo('#ddlTipEkle'); });

            var kod = new URLSearchParams(window.location.search).get('kullaniciKodu') || new URLSearchParams(window.location.search).get('KullaniciKodu');
            if (kod) { yukle(kod); return; }
            // Yeni kullanıcı: yönetici olmayanın kurumu kendi kurumudur.
            if (t.varsayilanKurum) $('#ddlKurum').val(String(t.varsayilanKurum).replace(/\./g, ''));
            tipleriCiz();
            durumGoster();
            $('#txtTc').trigger('focus');
        })
        .fail(hataGoster);
});
