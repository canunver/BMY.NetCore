// Resmi Tatil Günleri Düzenleme (KYM/ResmiTatilXml, eski ResmiTatilXml.aspx).
// Tatiller resmi tatil XML dosyasında tutulur: yil 0 olanlar her yıl aynı günde (eski "Tüm Yıllar"), diğerleri yalnızca o yıl
// (eski "Aktif Yıl"; ör. Ramazan ve Kurban bayramı). Solda seçili yılın 12 ayı, sağda tatil formu ve yılın tatil listesi.
// Her Kaydet / Sil dosyaya hemen yazılır (eskisinde de her ekleme, değişiklik ve silmede dosya yazılıyordu).
$(function () {
    'use strict';

    var AYLAR = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
    var GUNLER = ['Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt', 'Paz'];

    var tatiller = [];
    var yil = new Date().getFullYear();
    var secili = null;   // düzenlenen satır (sunucudaki haliyle); null ise yeni kayıt

    function iki(s) { return (s < 10 ? '0' : '') + s; }
    function anahtar(d) { return d.getFullYear() + '-' + iki(d.getMonth() + 1) + '-' + iki(d.getDate()); }
    function gosterim(d) { return iki(d.getDate()) + '.' + iki(d.getMonth() + 1) + '.' + d.getFullYear(); }

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    // Seçili yıla düşen tatiller: başlangıç tarihi ve kapsadığı günler (iş kurallarındaki gibi yalnızca o yılın günleri sayılır).
    function yilTatilleri(y) {
        var liste = [];
        $.each(tatiller, function (i, t) {
            if (t.yil !== 0 && t.yil !== y) return;
            var bas = new Date(y, t.ay - 1, t.gun);
            if (bas.getMonth() !== t.ay - 1) return;   // geçersiz gün (ör. 29 Şubat)
            var gunler = [];
            for (var j = 0; j < Math.max(1, t.sure); j++) {
                var d = new Date(y, t.ay - 1, t.gun + j);
                if (d.getFullYear() === y) gunler.push(anahtar(d));
            }
            liste.push({ satir: t, bas: bas, gunler: gunler });
        });
        liste.sort(function (a, b) { return a.bas - b.bas; });
        return liste;
    }

    function ciz() {
        var liste = yilTatilleri(yil);
        var gunHarita = {};
        $.each(liste, function (i, k) {
            $.each(k.gunler, function (j, g) { if (!gunHarita[g]) gunHarita[g] = k; });
        });
        var bugun = anahtar(new Date());
        var seciliAnahtar = secili ? (secili.yil === 0 || secili.yil === yil ? anahtar(new Date(yil, secili.ay - 1, secili.gun)) : null) : null;

        var $t = $('#takvim').empty();
        for (var ay = 0; ay < 12; ay++) {
            var $ay = $('<div class="rt-ay"></div>');
            $('<div class="rt-ay-baslik"></div>').text(AYLAR[ay]).appendTo($ay);
            var $tab = $('<table class="rt-ay-tablo"><thead><tr></tr></thead><tbody></tbody></table>');
            $.each(GUNLER, function (i, g) { $('<th></th>').text(g).appendTo($tab.find('thead tr')); });
            var ilk = new Date(yil, ay, 1);
            var bosluk = (ilk.getDay() + 6) % 7;
            var gunSay = new Date(yil, ay + 1, 0).getDate();
            var $tr = $('<tr></tr>');
            for (var b = 0; b < bosluk; b++) $tr.append('<td></td>');
            for (var g = 1; g <= gunSay; g++) {
                var d = new Date(yil, ay, g);
                var a = anahtar(d);
                var $td = $('<td class="rt-gun" tabindex="0"></td>').text(g).attr('data-tarih', a);
                var hg = (d.getDay() + 6) % 7;
                if (hg >= 5) $td.addClass('rt-haftasonu');
                if (a === bugun) $td.addClass('rt-bugun');
                var k = gunHarita[a];
                if (k) {
                    $td.addClass(k.satir.yil === 0 ? 'rt-tatil-her' : 'rt-tatil-yil').attr('title', k.satir.aciklama);
                    if (seciliAnahtar && k.satir === secili) $td.addClass('rt-secili');
                }
                $tr.append($td);
                if (hg === 6) { $tab.find('tbody').append($tr); $tr = $('<tr></tr>'); }
            }
            if ($tr.children().length) $tab.find('tbody').append($tr);
            $ay.append($tab);
            $t.append($ay);
        }

        $('#listeYil').text(yil);
        var $l = $('#tblTatil').empty();
        if (!liste.length) $l.append('<tr><td colspan="4" class="text-muted">Bu yıl için tatil yok.</td></tr>');
        $.each(liste, function (i, k) {
            var $r = $('<tr class="rt-satir"></tr>').data('kayit', k.satir);
            if (k.satir === secili) $r.addClass('active');
            $('<td></td>').text(gosterim(k.bas)).appendTo($r);
            $('<td class="text-center"></td>').text(k.satir.sure).appendTo($r);
            $('<td></td>').text(k.satir.aciklama).appendTo($r);
            $('<td></td>').html(k.satir.yil === 0
                ? '<i class="rt-renk rt-renk-her"></i> Her yıl'
                : '<i class="rt-renk rt-renk-yil"></i> Bu yıla özel').appendTo($r);
            $l.append($r);
        });
    }

    function formYeni(tarih) {
        secili = null;
        $('#formBaslik').text('Resmi Tatil Kayıt');
        $('#txtAciklama').val('');
        $('#txtTarih').val(tarih || '');
        $('#txtSure').val(1);
        $('#lstTur').val('yil');
        $('#btnSil').prop('hidden', true);
        ciz();
        $('#txtAciklama').trigger('focus');
    }

    function formAc(t) {
        secili = t;
        $('#formBaslik').text('Resmi Tatil Düzenle');
        $('#txtAciklama').val(t.aciklama);
        $('#txtTarih').val(anahtar(new Date(t.yil === 0 ? yil : t.yil, t.ay - 1, t.gun)));
        $('#txtSure').val(t.sure || 1);
        $('#lstTur').val(t.yil === 0 ? 'her' : 'yil');
        $('#btnSil').prop('hidden', false);
        ciz();
    }

    function sonucIsle(s) {
        if (!s.basarili) { Sgb.bildirim(s.hata, 'danger'); return false; }
        tatiller = s.tatiller || [];
        if (s.bilgi) Sgb.bildirim(s.bilgi, 'success');
        return true;
    }

    function yukle() {
        Sgb.al('api/resmitatil/liste').done(function (s) {
            if (!s.basarili) { Sgb.bildirim(s.hata, 'danger'); return; }
            tatiller = s.tatiller || [];
            var yillar = $.map(tatiller, function (t) { return t.yil > 0 ? t.yil : null; });
            yillar.push(s.buYil - 1, s.buYil + 1);
            Sgb.yilKutusu('#txtYil', yillar, yil);
            $('#txtYil').attr({ min: 1900, max: 2999 });
            ciz();
        }).fail(hataGoster);
    }

    function yilDegis(y) {
        y = parseInt(y, 10);
        if (!(y >= 1900 && y <= 2999)) return;
        yil = y;
        $('#txtYil').val(y);
        if (secili && secili.yil !== 0 && secili.yil !== yil) formYeni('');
        else if (secili) formAc(secili);
        else ciz();
    }

    // ------------------------------------------------------------------ olaylar

    $('#txtYil').on('change', function () { yilDegis(this.value); });
    $('#btnOnceki').on('click', function () { yilDegis(yil - 1); });
    $('#btnSonraki').on('click', function () { yilDegis(yil + 1); });
    $('#btnYeni').on('click', function () { formYeni(''); });
    $('#btnVazgec').on('click', function () { formYeni(''); });

    $('#takvim').on('click keydown', '.rt-gun', function (e) {
        if (e.type === 'keydown' && e.key !== 'Enter' && e.key !== ' ') return;
        e.preventDefault();
        var a = $(this).data('tarih');
        var k = null;
        $.each(yilTatilleri(yil), function (i, x) { if (!k && x.gunler.indexOf(a) >= 0) k = x; });
        if (k) formAc(k.satir); else formYeni(a);
    });

    $('#tblTatil').on('click', '.rt-satir', function () { formAc($(this).data('kayit')); });

    $('#frmTatil').on('submit', function (e) {
        e.preventDefault();
        var aciklama = $.trim($('#txtAciklama').val());
        var tarih = /^(\d{4})-(\d{2})-(\d{2})$/.exec($('#txtTarih').val() || '');
        var sure = parseInt($('#txtSure').val(), 10);
        if (!aciklama) { Sgb.bildirim('Açıklama giriniz.', 'warning'); return; }
        if (!tarih) { Sgb.bildirim('Tarih giriniz.', 'warning'); return; }
        if (!(sure >= 1 && sure <= 30)) { Sgb.bildirim('Süre 1 ile 30 gün arasında olmalı.', 'warning'); return; }
        var her = $('#lstTur').val() === 'her';
        var yeni = {
            yil: her ? 0 : parseInt(tarih[1], 10),
            ay: parseInt(tarih[2], 10),
            gun: parseInt(tarih[3], 10),
            sure: sure,
            aciklama: aciklama
        };
        var $b = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/resmitatil/kaydet', { eski: secili, yeni: yeni }).done(function (s) {
            if (!sonucIsle(s)) { if (/bulunamadı/.test(s.hata || '')) yukle(); return; }
            // Kaydedilen satırı seçili bırak (bu yıla özelse o yıla geçilir).
            if (!her && yeni.yil !== yil) yil = yeni.yil;
            $('#txtYil').val(yil);
            var bulunan = null;
            $.each(tatiller, function (i, t) {
                if (t.yil === yeni.yil && t.ay === yeni.ay && t.gun === yeni.gun && t.aciklama === yeni.aciklama) bulunan = t;
            });
            if (bulunan) formAc(bulunan); else formYeni('');
        }).fail(hataGoster).always(function () { $b.prop('disabled', false); });
    });

    $('#btnSil').on('click', function () {
        if (!secili) return;
        if (!window.confirm('"' + secili.aciklama + '" tatili silinecek. Onaylıyor musunuz?')) return;
        var $b = $(this).prop('disabled', true);
        Sgb.gonder('api/resmitatil/sil', secili).done(function (s) {
            if (!sonucIsle(s)) { if (/bulunamadı/.test(s.hata || '')) yukle(); return; }
            formYeni('');
        }).fail(hataGoster).always(function () { $b.prop('disabled', false); });
    });

    $('#txtYil').val(yil);
    ciz();
    yukle();
});
