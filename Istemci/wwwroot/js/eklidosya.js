// Ortak ekli dosyalar bileşeni (eski Duyuru/EkliDosyalarForm.aspx). jQuery, AG Grid ve ortak.js gerektirir.
// Kullanım:
//   var ed = Sgb.ekliDosyalar('#dosyalar', { modul: 'DUY', form: '12' });
//   ed.formDegistir('13');         // başka bir kayda geç ('-1' ya da boş: kayıt henüz yok, yükleme kapalı)
// Ayarlar: salt okunur için yuklemeYok: true; birim adı sütunu için birimAd: true; önek araması için onEk: true.
window.Sgb = window.Sgb || {};
Sgb.ekliDosyalar = function (element, ayarlar) {
    'use strict';

    ayarlar = $.extend({ modul: '', form: '', yuklemeYok: false, birimAd: false, onEk: false }, ayarlar);
    var $kok = $(element).addClass('ekli-dosyalar').empty();
    var M = {};
    var kilitVar = false;
    var azamiMB = 20;
    var grid = null;

    var $yukle = $(
        '<form class="ekli-dosya-yukle" novalidate>' +
        '  <input type="text" class="form-control input-sm ed-aciklama" maxlength="500" />' +
        '  <input type="file" class="form-control input-sm ed-dosya" />' +
        '  <button type="submit" class="btn btn-default btn-sm ed-yukle"><span class="glyphicon glyphicon-upload" aria-hidden="true"></span> <span class="ed-yukle-metin"></span></button>' +
        '</form>').appendTo($kok);
    var $not = $('<p class="help-block small ed-not" hidden></p>').appendTo($kok);
    var $grid = $('<div class="ekli-dosya-grid"></div>').appendTo($kok);
    if (ayarlar.yuklemeYok) $yukle.prop('hidden', true);

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function kayitVar() { return ayarlar.form && ayarlar.form !== '-1'; }

    function boyut(b) {
        if (b < 1024) return b + ' B';
        if (b < 1024 * 1024) return (b / 1024).toFixed(1).replace('.', ',') + ' KB';
        return (b / (1024 * 1024)).toFixed(1).replace('.', ',') + ' MB';
    }

    function adres(satir, ek) {
        return Sgb.KOK + 'api/ekli-dosya/indir?' + $.param({ modul: ayarlar.modul, form: satir.form, no: satir.no, ek: ek ? 1 : 0 });
    }

    // Duyuru metnine konabilecek kalıcı adres (eski "Dosya Bağlantı Yolu").
    function kaliciAdres(satir) {
        var a = document.createElement('a');
        a.href = Sgb.KOK + 'Duyuru/EkliDosyalarForm?' + $.param({ no: satir.no, mdl: ayarlar.modul, frm: satir.form });
        return a.href;
    }

    function dugme(simge, baslik, islem, sinif) {
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-link btn-xs ' + (sinif || '');
        b.title = baslik;
        b.setAttribute('aria-label', baslik);
        b.setAttribute('data-islem', islem);
        b.innerHTML = '<span class="glyphicon glyphicon-' + simge + '" aria-hidden="true"></span>';
        return b;
    }

    function islemHucresi(p) {
        var d = p.data, kilitli = !!d.onaylayan;
        var kutu = document.createElement('span');
        kutu.className = 'ed-islemler';
        kutu.appendChild(dugme('download-alt', M.Indir, 'indir'));
        if (!ayarlar.yuklemeYok) {
            if (!(kilitVar && kilitli)) kutu.appendChild(dugme('trash', M.Sil, 'sil', 'text-danger'));
            if (kilitVar) {
                kutu.appendChild(dugme('link', M.Baglanti, 'baglanti'));
                kutu.appendChild(kilitli ? dugme('lock', M.KilitAc, 'kilitAc') : dugme('ok-circle', M.Kilitle, 'kilitle'));
            }
        }
        return kutu;
    }

    function gridOlustur() {
        grid = Sgb.grid($grid[0], {
            columnDefs: [
                { field: 'birimAd', headerName: M.Birim, width: 160, hide: !ayarlar.birimAd },
                {
                    field: 'ad', headerName: M.DosyaAd, width: 160,
                    cellRenderer: function (p) {
                        var a = document.createElement('a');
                        a.href = adres(p.data, false);
                        a.target = '_blank';
                        a.rel = 'noopener';
                        a.textContent = p.value + (p.data.uzanti ? '.' + p.data.uzanti : '');
                        var ipucu = [];
                        if (p.data.aciklama) ipucu.push(p.data.aciklama);
                        if (p.data.onaylayan) ipucu.push(M.Kilitli + ': ' + p.data.onaylayan);
                        if (ipucu.length) a.title = ipucu.join(' - ');
                        return a;
                    }
                },
                { headerName: M.Islem, width: 104, sortable: false, cellRenderer: islemHucresi },
                { field: 'aciklama', headerName: M.Aciklama, flex: 1, minWidth: 160 },
                { field: 'buyukluk', headerName: M.Boy, width: 76, type: 'rightAligned', valueFormatter: function (p) { return boyut(p.value); } },
                { field: 'tarih', headerName: M.Tarih, width: 100, valueFormatter: function (p) { return Sgb.tarih(p.value); } },
                { field: 'yukleyen', headerName: M.Yukleyen, width: 120 }
            ],
            getRowId: function (p) { return p.data.form + '|' + p.data.no; },
            domLayout: 'autoHeight'
        });

        $grid.on('click', 'button[data-islem]', function () {
            var satir = null;
            var $sat = $(this).closest('[row-id]');
            var n = grid.getRowNode($sat.attr('row-id'));
            if (n) satir = n.data;
            if (satir) islem($(this).data('islem'), satir);
        });
    }

    function metinleriYaz() {
        $yukle.find('.ed-aciklama').attr('placeholder', M.Aciklama).attr('aria-label', M.Aciklama);
        $yukle.find('.ed-dosya').attr('aria-label', M.Dosya);
        $yukle.find('.ed-yukle-metin').text(M.Yukle);
    }

    function durumuGuncelle() {
        var acik = kayitVar();
        $yukle.find('input, button').prop('disabled', !acik);
        $not.text(M.OnceKaydet || '').prop('hidden', acik || ayarlar.yuklemeYok);
    }

    function listele() {
        return Sgb.al('api/ekli-dosya', {
            modul: ayarlar.modul, form: ayarlar.form || '-1',
            onEk: ayarlar.onEk ? 1 : undefined, birimAd: ayarlar.birimAd ? 1 : undefined
        }).done(function (v) {
            var ilk = !grid;
            M = v.metin || M;
            kilitVar = v.kilitVar;
            azamiMB = v.azamiMB;
            if (ilk) { metinleriYaz(); gridOlustur(); }
            durumuGuncelle();
            if (v.hata) mesaj(v.hata, 'danger');
            grid.setGridOption('rowData', v.satirlar);
        }).fail(function (x) { if (x.status !== 401) mesaj(M.IslemHata || 'Hata', 'danger'); });
    }

    function sonuc(s) {
        if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
        mesaj(s.bilgi, s.bilgi ? 'success' : null);
        listele();
    }

    function islem(ad, satir) {
        var istek = { modul: ayarlar.modul, form: satir.form, no: satir.no };
        if (ad === 'indir') {
            window.location.href = adres(satir, true);
        } else if (ad === 'sil') {
            if (window.confirm(M.SilOnay)) Sgb.gonder('api/ekli-dosya/sil', istek).done(sonuc);
        } else if (ad === 'kilitle' || ad === 'kilitAc') {
            istek.kilitle = ad === 'kilitle';
            Sgb.gonder('api/ekli-dosya/kilit', istek).done(sonuc);
        } else if (ad === 'baglanti') {
            var a = kaliciAdres(satir);
            if (navigator.clipboard) {
                navigator.clipboard.writeText(a).then(function () { mesaj(M.Kopyalandi + ' ' + a, 'info'); },
                    function () { window.prompt(M.Baglanti, a); });
            } else {
                window.prompt(M.Baglanti, a);
            }
        }
    }

    $yukle.on('submit', function (e) {
        e.preventDefault();
        var dosya = $yukle.find('.ed-dosya')[0].files[0];
        if (!dosya) { mesaj(M.DosyaSec, 'warning'); return; }
        if (dosya.size > azamiMB * 1024 * 1024) { mesaj((M.CokBuyuk || '').replace('{0}', azamiMB), 'warning'); return; }
        var veri = new FormData();
        veri.append('modul', ayarlar.modul);
        veri.append('form', ayarlar.form);
        veri.append('aciklama', $yukle.find('.ed-aciklama').val());
        veri.append('dosya', dosya);
        var $d = $yukle.find('.ed-yukle').prop('disabled', true);
        mesaj(M.Yukleniyor, 'info');
        $.ajax({
            url: Sgb.KOK + 'api/ekli-dosya/yukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (s) {
            if (s.basarili) $yukle[0].reset();
            sonuc(s);
        }).fail(function (x) {
            if (x.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else mesaj(x.status === 413 ? (M.CokBuyuk || '').replace('{0}', azamiMB) : M.IslemHata, 'danger');
        }).always(function () { $d.prop('disabled', !kayitVar()); });
    });

    listele();

    return {
        formDegistir: function (form) {
            ayarlar.form = form == null ? '' : String(form);
            mesaj('');
            $yukle[0].reset();
            return listele();
        },
        yenile: listele
    };
};
