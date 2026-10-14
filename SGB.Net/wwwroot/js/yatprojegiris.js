// Yatırım Projeleri (ButceHazirlikv2/ProjeGiris; ?islemKod=1 ile İzleme Değerlendirme):
//   solda arama ölçütleri ve proje ağacı (projeler ve alt projeleri), sağda seçilenin sayfası çerçevede:
//   ProjeDetay / AltProjeDetay (izlemede ikisi için de YATIzlemeDegerlendirme).
// Çerçevedeki sayfalar ağacı window.parent.YatProje ile günceller (eski parent.DalGuncelle / DalSil / AltProjeAc).
$(function () {
    'use strict';

    if (!$('#agcProjeler').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var islemKod = new URLSearchParams(location.search).get('islemKod') === '1' ? 1 : 0;
    var tanim = null;       // açılış bilgisi (api/yatprojegiris/tanim)
    var projeler = [];      // son listelenen projeler: { kod, ad, altProjeler: [{ kod, ad }] }
    var acik = { yil: 0, senaryo: '' }; // ağacın listelendiği yıl ve aşama (açılan sayfalar bununla açılır)
    var secili = null;      // seçili düğümün anahtarı ('p12' ya da 'a34')

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function sonucGoster(s, basariMetni) {
        if (s && s.hata) { mesaj(s.hata, 'danger'); return false; }
        mesaj((s && s.bilgi) || basariMetni, 'success');
        return true;
    }

    function secenekler($ddl, liste, secili) {
        $ddl.empty();
        $.each(liste || [], function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($ddl); });
        if (secili !== undefined && secili !== null) $ddl.val(secili);
        if ($ddl.val() === null && $ddl.children().length) $ddl.prop('selectedIndex', 0);
    }

    // ---------------------------------------------------------------- pencereler

    var dlgOnay = document.getElementById('dlgOnay');
    var onaylaninca = null;
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    function onayIste(baslik, metin, dugme, islem) {
        $('#dlgOnayBaslik').text(baslik);
        $('#onayMetin').text(metin);
        $('#btnOnayla').text(dugme);
        onaylaninca = islem;
        dlgOnay.showModal();
    }

    $('#btnOnayla').on('click', function () {
        var islem = onaylaninca;
        onaylaninca = null;
        dlgOnay.close();
        if (islem) islem();
    });

    // ---------------------------------------------------------------- ağaç

    function projeBul(pKod) {
        return $.grep(projeler, function (p) { return p.kod === pKod; })[0];
    }

    var agac = Sgb.agac('#agcProjeler', {
        yukle: function (ust) {
            var d = $.Deferred();
            if (!ust) {
                d.resolve($.map(projeler, function (p) {
                    return { kod: 'p' + p.kod, ad: p.ad, tur: 1, pKod: p.kod, altVar: (p.altProjeler || []).length > 0 };
                }));
            } else {
                var p = projeBul(ust.pKod);
                d.resolve($.map((p && p.altProjeler) || [], function (a) {
                    return { kod: 'a' + a.kod, ad: a.ad, tur: 2, pKod: ust.pKod, apKod: a.kod, altVar: false };
                }));
            }
            return d.promise();
        },
        simge: function (d) { return d.tur === 1 ? 'glyphicon-folder-open' : 'glyphicon-file'; },
        ipucu: function (d) { return d.ad; },
        seciliMi: function (d) { return d.kod === secili; },
        secilince: function (d) {
            if (d.tur === 1) projeAc(d.pKod, d.ad);
            else altProjeAc(d.apKod, d.pKod, d.ad);
        },
        bosMetin: 'Projeleri görmek için ölçütleri seçip “Listele”ye basınız.'
    });

    function listele() {
        var yil = parseInt($('#txtYil').val(), 10) || 0;
        var senaryo = $('#ddlAsama').val();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!senaryo) { mesaj('Lütfen aşama seçiniz!', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        Sgb.al('api/yatprojegiris/projeler', {
            yil: yil, senaryo: senaryo,
            projeNo: $('#txtProjeNo').val(), projeAd: $('#txtProjeAd').val(),
            aktiflik: $('#ddlAktiflik').val(), sektor: $('#ddlSektor').val(), birim: $('#ddlBirim').val()
        }).done(function (s) {
            if (s.hata) { mesaj(s.hata, 'danger'); return; }
            projeler = s.projeler || [];
            acik = { yil: yil, senaryo: senaryo };
            agac.yenile();
            if (s.bilgi) mesaj(s.bilgi, 'info');
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    }

    // ---------------------------------------------------------------- sağdaki sayfa

    function sayfaAc(sayfa, parametreler, baslik) {
        parametreler.menuYok = 1;
        parametreler.t = Date.now(); // eski sayfa da her açılışta yeniden yüklüyordu
        $('#detayBaslik').text(baslik);
        $('#detayBos').prop('hidden', true);
        $('#cerceveDetay').prop('hidden', false).attr('src', sayfa + '?' + $.param(parametreler));
    }

    function calismaYili() { return acik.yil || parseInt($('#txtYil').val(), 10) || 0; }
    function calismaAsamasi() { return acik.senaryo || $('#ddlAsama').val() || ''; }

    function projeAc(kod, ad) {
        secili = kod ? 'p' + kod : null;
        sayfaAc(islemKod === 1 ? 'YATIzlemeDegerlendirme' : 'ProjeDetay',
            { yil: calismaYili(), kod: kod || 0, apKod: 0, senaryoKod: calismaAsamasi() },
            (tanim ? tanim.detayBaslik : 'Detay Bilgileri') + (ad ? ' — ' + ad : (kod ? '' : ' — Yeni Proje')));
    }

    function altProjeAc(apKod, pKod, ad) {
        secili = apKod ? 'a' + apKod : null;
        sayfaAc(islemKod === 1 ? 'YATIzlemeDegerlendirme' : 'AltProjeDetay',
            { yil: calismaYili(), apKod: apKod || 0, pKod: pKod, senaryoKod: calismaAsamasi() },
            (tanim ? tanim.detayBaslik : 'Detay Bilgileri') + (ad ? ' — ' + ad : ' — Yeni Alt Proje'));
    }

    // Çerçevedeki sayfa kaydedince / silince ağaç güncellenir (eski DalGuncelle / DalSil). tur: 1 proje, 2 alt proje.
    function dugumYaz(anahtar, ad) {
        var $li = $('#agcProjeler li').filter(function () { var d = $(this).data('dugum'); return d && d.kod === anahtar; }).first();
        if (!$li.length) return false;
        $li.data('dugum').ad = ad;
        var $sec = $li.find('> .satir > .sec').attr('title', ad);
        $sec.contents().filter(function () { return this.nodeType === 3; }).remove();
        $sec.append(document.createTextNode(ad));
        return true;
    }

    function dalGuncelle(pKod, apKod, ad, tur) {
        pKod = parseInt(pKod, 10) || 0;
        apKod = parseInt(apKod, 10) || 0;
        if (!pKod && !apKod) return;
        var p = projeBul(pKod);
        if (tur === 1) {
            secili = 'p' + pKod;
            if (p) { p.ad = ad; dugumYaz('p' + pKod, ad); return; }
            projeler.push({ kod: pKod, ad: ad, altProjeler: [] });
            agac.yenile();
            return;
        }
        secili = 'a' + apKod;
        if (!p) return; // proje ağaçta yoksa (başka ölçütlerle listelenmiş) eklenmez
        var a = $.grep(p.altProjeler || [], function (x) { return x.kod === apKod; })[0];
        if (a) { a.ad = ad; dugumYaz('a' + apKod, ad); return; }
        (p.altProjeler = p.altProjeler || []).push({ kod: apKod, ad: ad });
        agac.altiniYenile('p' + pKod);
    }

    function dalSil(pKod, apKod, tur) {
        pKod = parseInt(pKod, 10) || 0;
        apKod = parseInt(apKod, 10) || 0;
        if (tur === 1) {
            projeler = $.grep(projeler, function (x) { return x.kod !== pKod; });
            agac.yenile();
        } else {
            var p = projeBul(pKod);
            if (!p) return;
            p.altProjeler = $.grep(p.altProjeler || [], function (x) { return x.kod !== apKod; });
            agac.altiniYenile('p' + pKod);
        }
        secili = null;
        $('#cerceveDetay').prop('hidden', true).attr('src', 'about:blank');
        $('#detayBos').prop('hidden', false);
        $('#detayBaslik').text(tanim ? tanim.detayBaslik : 'Detay Bilgileri');
    }

    window.YatProje = {
        projeAc: function (kod, ad) { projeAc(parseInt(kod, 10) || 0, ad); },
        altProjeAc: function (apKod, pKod, ad) { altProjeAc(parseInt(apKod, 10) || 0, parseInt(pKod, 10) || 0, ad); },
        dalGuncelle: dalGuncelle,
        dalSil: dalSil
    };

    // ---------------------------------------------------------------- düğmeler

    $('#frmArama').on('submit', function (olay) { olay.preventDefault(); listele(); });

    $('#btnYeniProje').on('click', function () { projeAc(0, ''); });

    $('#btnOdenekAktar').on('click', function () {
        onayIste('Ödenek Aktar', 'Bütçe kaynak ödenekleri aktarılacak. Onaylıyor musunuz?', 'Aktar', function () {
            var $d = $('#btnOdenekAktar').prop('disabled', true);
            Sgb.gonder('api/yatprojegiris/odenek-aktar', { yil: parseInt($('#txtYil').val(), 10) || 0, senaryo: $('#ddlAsama').val() })
                .done(function (s) { sonucGoster(s, 'Aktarım işlemi tamamlandı'); })
                .fail(hataGoster).always(function () { $d.prop('disabled', false); });
        });
    });

    $('#btnKopyala').on('click', function () {
        $('#txtKopyaYil').val($('#txtYil').val());
        document.getElementById('dlgKopyala').showModal();
    });

    $('#frmKopyala').on('submit', function (olay) {
        olay.preventDefault();
        var veri = { yil: parseInt($('#txtKopyaYil').val(), 10) || 0, kaynak: $('#ddlKopyaKaynak').val(), hedef: $('#ddlKopyaHedef').val() };
        if (veri.yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!veri.kaynak || !veri.hedef) { mesaj('Kaynak ve hedef aşamayı seçiniz.', 'warning'); return; }
        if (veri.kaynak === veri.hedef) { mesaj('Kaynak ve hedef aşama aynı olamaz.', 'warning'); return; }
        document.getElementById('dlgKopyala').close();
        onayIste('Aşama Kopyala', 'Aşama rakamları kopyalanacak, hedef aşamasındaki veriler silinecek. Onaylıyor musunuz?', 'Kopyala', function () {
            Sgb.gonder('api/yatprojegiris/asama-kopyala', veri)
                .done(function (s) { sonucGoster(s, 'Aşama rakamları kopyalandı'); })
                .fail(hataGoster);
        });
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/yatprojegiris/tanim', { islemKod: islemKod }).done(function (t) {
        if (t.hata) { mesaj(t.hata, 'danger'); return; }
        tanim = t;
        document.title = t.baslik;
        $('#detayBaslik').text(t.detayBaslik);
        $('#txtYil').val(t.yil);
        secenekler($('#ddlAsama'), t.asamalar, t.senaryo);
        $('#ddlAsama').prop('disabled', !!t.asamaKilitli);
        secenekler($('#ddlKopyaKaynak'), t.asamalar);
        secenekler($('#ddlKopyaHedef'), t.asamalar);
        secenekler($('#ddlSektor'), t.sektorler, '');
        secenekler($('#ddlBirim'), t.birimler, '');
        $('#btnOdenekAktar, #btnKopyala').prop('hidden', !t.yonetici);
    }).fail(hataGoster);
});
