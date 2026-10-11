// Genel Kod Tanımlama (ButceHazirlikv2/GenelKodTanimla?Tur=n): kod ağacı (Sgb.agac) ve seçili kodun bilgileri.
// Tur=21 birim türleri, 20 il-ilçe, 1..7 bütçe sınıflandırmaları ... Diller ve ek alanlar türün tanımından gelir.
$(function () {
    'use strict';

    if (!$('#agacKod').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var sorgu = new URLSearchParams(window.location.search);
    var tur = parseInt(sorgu.get('Tur') || sorgu.get('tur') || '1', 10) || 1;
    var tanim = null;      // türün yapısı: { yil, yilVar, baslik, kodYapi, diller, ekAlanlar }
    var yil = parseInt(sorgu.get('yil') || '0', 10) || 0;
    var seciliKod = '';    // ağaçta seçili (formda açık) kod; yeni kodda boş
    var agac = null;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function ustKod(kod) {
        var nokta = kod.lastIndexOf('.');
        return nokta > 0 ? kod.substring(0, nokta) : '';
    }
    function rakamSay(kod) { return String(kod || '').replace(/\./g, '').length; }

    // ---------------------------------------------------------------- form

    function formCiz() {
        var $diller = $('#dilAlanlari').empty();
        $.each(tanim.diller, function (i, d) {
            if (tanim.diller.length > 1) $('<div class="alan-grubu"></div>').text(d.ad).appendTo($diller);
            var $ad = $('<div class="form-group"></div>').appendTo($diller);
            $('<label class="control-label"></label>').attr('for', 'txtAd_' + d.kod).text('Ad').appendTo($ad);
            $('<div></div>').append($('<input type="text" class="form-control" />').attr('id', 'txtAd_' + d.kod).attr('lang', d.kod)).appendTo($ad);
            var $ack = $('<div class="form-group ust-hizali"></div>').appendTo($diller);
            $('<label class="control-label"></label>').attr('for', 'txtAciklama_' + d.kod).text('Açıklama').appendTo($ack);
            $('<div></div>').append($('<textarea class="form-control" rows="3"></textarea>').attr('id', 'txtAciklama_' + d.kod).attr('lang', d.kod)).appendTo($ack);
        });

        var $ek = $('#ekAlanlar').empty();
        if (tanim.ekAlanlar.length) $('<div class="alan-grubu">Ek Alanlar</div>').appendTo($ek);
        $.each(tanim.ekAlanlar, function (i, a) {
            var kimlik = 'ek_' + a.ad;
            var $grup = $('<div class="form-group"></div>').attr('data-ek', a.ad).appendTo($ek);
            $('<label class="control-label"></label>').attr('for', kimlik).text(a.etiket).appendTo($grup);
            var $alan;
            if (a.secenekler) {
                $alan = $('<select class="form-control"></select>').append('<option value=""></option>');
                $.each(a.secenekler, function (j, s) { $('<option></option>').val(s.kod).text(s.ad).appendTo($alan); });
            } else
                $alan = $('<input class="form-control" />').attr('type', 'text').attr('inputmode', a.sayi ? 'numeric' : null).toggleClass('dar', !!a.sayi);
            $('<div></div>').append($alan.attr('id', kimlik)).appendTo($grup);
        });
        $('#kodYapi').text('Kod yapısı: ' + tanim.kodYapi);
    }

    // Ek alanlar yalnızca uygun seviyedeki kodlarda gösterilir (eski EkAlanGoster / KodUygunMu).
    function ekAlanlariGoster() {
        var boy = rakamSay($('#txtKod').val());
        var gorunen = 0;
        $.each(tanim.ekAlanlar, function (i, a) {
            var bas = rakamSay(a.bas), bit = rakamSay(a.bit);
            var uygun = boy === bas || (bit > 0 && bas <= boy && boy <= bit);
            $('#ekAlanlar [data-ek="' + a.ad + '"]').prop('hidden', !uygun);
            if (uygun) gorunen++;
        });
        $('#ekAlanlar .alan-grubu').prop('hidden', !gorunen);
    }

    function formDoldur(k) {
        $('#txtKod').val(k.kod || '');
        $.each(tanim.diller, function (i, d) {
            var dil = (k.diller || {})[d.kod] || {};
            $('#txtAd_' + d.kod).val(dil.ad || '');
            $('#txtAciklama_' + d.kod).val(dil.aciklama || '');
        });
        $.each(tanim.ekAlanlar, function (i, a) {
            var $alan = $('#ek_' + a.ad), deger = (k.ek || {})[a.ad] || '';
            // Kayıtlı değer seçeneklerde yoksa seçenek olarak eklenir ki değer kaybolmasın.
            if ($alan.is('select') && deger && !$alan.find('option').filter(function () { return this.value === deger; }).length)
                $('<option></option>').val(deger).text(deger).appendTo($alan);
            $alan.val(deger);
        });
        ekAlanlariGoster();
        $('#btnSil').prop('disabled', !seciliKod);
    }

    function formOku() {
        var istek = { tur: tur, yil: yil, kod: $.trim($('#txtKod').val()), diller: {}, ek: {} };
        $.each(tanim.diller, function (i, d) {
            istek.diller[d.kod] = { ad: $.trim($('#txtAd_' + d.kod).val()), aciklama: $.trim($('#txtAciklama_' + d.kod).val()) };
        });
        $.each(tanim.ekAlanlar, function (i, a) { istek.ek[a.ad] = $.trim($('#ek_' + a.ad).val() || ''); });
        return istek;
    }

    function kodAc(kod) {
        seciliKod = kod;
        Sgb.al('api/kod/oku', { tur: tur, yil: yil, kod: kod })
            .done(function (k) {
                if (seciliKod !== kod) return; // bu arada başka kod seçildi
                if (k.hata) { mesaj(k.hata, 'danger'); return; }
                formDoldur(k);
            })
            .fail(hataGoster);
    }

    $('#txtKod').on('input', ekAlanlariGoster);

    // Yeni: seçili kodun altına (kırılım elveriyorsa) yeni kod yazmak için form boşaltılır.
    $('#btnYeni').on('click', function () {
        var onEk = seciliKod && seciliKod.split('.').length < tanim.kodYapi.split('.').length ? seciliKod + '.' : ustKod(seciliKod) ? ustKod(seciliKod) + '.' : '';
        seciliKod = '';
        $('#agacKod .sec').removeAttr('aria-current');
        formDoldur({ kod: onEk });
        $('#txtKod').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        var istek = formOku();
        if (!istek.kod) { mesaj('Kod giriniz.', 'warning'); $('#txtKod').trigger('focus'); return; }
        if (!istek.diller[tanim.diller[0].kod].ad) { mesaj('Ad giriniz.', 'warning'); $('#txtAd_' + tanim.diller[0].kod).trigger('focus'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/kod/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                seciliKod = s.kod;
                $('#txtKod').val(s.kod);
                $('#btnSil').prop('disabled', false);
                // Ağaçta yalnızca kodun bulunduğu dal yeniden okunur (yeni kod görünsün, adı değişen güncellensin).
                agac.altiniYenile(ustKod(s.kod));
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnSil').on('click', function () {
        if (!seciliKod) return;
        if (!window.confirm(seciliKod + ' kodu silinecek. Onaylıyor musunuz?')) return;
        var kod = seciliKod;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/kod/sil', { tur: tur, yil: yil, kod: kod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); $d.prop('disabled', false); return; }
                mesaj(s.bilgi, 'success');
                seciliKod = '';
                formDoldur({});
                agac.altiniYenile(ustKod(kod));
            })
            .fail(function (xhr) { $d.prop('disabled', false); hataGoster(xhr); });
    });

    $('#btnOncekiYildan').on('click', function () {
        if (!window.confirm((yil - 1) + ' yılının kodları ' + yil + ' yılına kopyalanacak. Onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/kod/oncekiyil', { tur: tur, yil: yil })
            .done(function (s) {
                mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
                if (s.basarili) agac.yenile();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // Rapor Hazırla: bütün kodların listesi sunucuda Excel olarak üretilir ve iner.
    $('#btnRapor').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/kod/rapor', { tur: tur, yil: yil }).then(function () { $d.prop('disabled', false); });
    });

    // Dosyadan Yükle: seçilen Excel dosyasındaki kodlar kaydedilir. Sütunlar: kod, her dil için ad ve açıklama, ek alanlar.
    $('#btnYukle').on('click', function () { $('#dosyaYukle').val('').trigger('click'); });
    $('#dosyaYukle').on('change', function () {
        var dosya = this.files[0];
        if (!dosya) return;
        var sutunlar = ['kod'];
        $.each(tanim.diller, function (i, d) { sutunlar.push(d.ad + ' ad', d.ad + ' açıklama'); });
        $.each(tanim.ekAlanlar, function (i, a) { sutunlar.push(a.etiket); });
        if (!window.confirm('"' + dosya.name + '" dosyasındaki kodlar kaydedilecek; kayıtlı kodlar dosyadaki bilgilerle güncellenecek.\n\n' +
            'Dosyada başlık satırı olmamalı; sütunlar sırayla: ' + sutunlar.join(', ') + '.\n\nOnaylıyor musunuz?')) return;
        var veri = new FormData();
        veri.append('tur', tur);
        veri.append('yil', yil);
        veri.append('dosya', dosya);
        var $d = $('#btnYukle').prop('disabled', true);
        $.ajax({
            url: Sgb.KOK + 'api/kod/yukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (s) {
            mesaj(s.basarili ? s.bilgi : s.hata, s.basarili ? 'success' : 'danger');
            if (s.basarili) agac.yenile();
        }).fail(function (xhr) {
            if (xhr.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            hataGoster(xhr);
        }).always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- ağaç

    function baslikYaz() {
        $('#yilEtiket').text(tanim.yilVar ? yil : '');
        $('#agacBaslik').text(tanim.baslik);
    }

    function agaciKur() {
        agac = Sgb.agac('#agacKod', {
            yukle: function (ust) {
                return Sgb.al('api/kod/liste', { tur: tur, yil: yil, ust: ust ? ust.kod : '', filtre: $.trim($('#txtFiltre').val()) }).then(function (veri) {
                    if (veri.hata) { mesaj(veri.hata, 'danger'); return []; }
                    return veri.satirlar;
                });
            },
            secilince: function (d) { kodAc(d.kod); },
            seciliMi: function (d) { return d.kod === seciliKod; },
            ilkAcik: true,
            bosMetin: 'Kod bulunamadı.',
            hata: hataGoster
        });
    }

    $('#frmFiltre').on('submit', function (olay) {
        olay.preventDefault();
        agac.yenile();
    });
    // Arama kutusu temizlenince (x) süzme kalkar.
    $('#txtFiltre').on('search', function () { if (!this.value) agac.yenile(); });

    function yilDegistir(fark) {
        yil += fark;
        seciliKod = '';
        formDoldur({});
        baslikYaz();
        agac.yenile();
    }
    $('#btnOncekiYil').on('click', function () { yilDegistir(-1); });
    $('#btnSonrakiYil').on('click', function () { yilDegistir(1); });

    // ---------------------------------------------------------------- açılış

    // Yıl adreste verilmemişse gönderilmez; sunucu bütçe hazırlık yılını kullanır.
    Sgb.al('api/kod/tanim', yil ? { tur: tur, yil: yil } : { tur: tur })
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            yil = t.yil;
            $('#yilGezgin, #btnOncekiYildan').prop('hidden', !t.yilVar);
            baslikYaz();
            formCiz();
            formDoldur({});
            agaciKur();
        })
        .fail(hataGoster);
});
