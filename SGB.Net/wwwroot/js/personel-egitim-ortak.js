// Personel İşlemleri'nin Öğrenim Durumu, Yabancı Dil, Hizmet İçi Eğitim ve Ceza/Ödül sekmelerinin (PersonelOgrenim, PersonelDil,
// PersonelKurs, PersonelCezaOdul) ortak deseni: solda form, sağda kayıt listesi; Kaydet / Sil / Yeni Kayıt düğmeleri.
//   PbEgitim.mesaj / hataGoster    : işlem sonucu bildirimi (Sgb.bildirim)
//   PbEgitim.onay(metin)           : sayfa içi <dialog> ile onay; söz (Promise) döner, Evet'te çözülür
//   PbEgitim.secenekDoldur(s, l)   : <select>'i [{KOD, AD}] listesiyle doldurur
//   PbEgitim.secimYap(s, deger)    : <select>'te değeri seçer; listede yoksa boş (ilk) seçenek
//   PbEgitim.listeGrid(el, sutun)  : salt okunur (sarı) kayıt listesi
//   PbEgitim.belgeler(ayar)        : kayda ekli belgeler (Belge Ekle / Görüntüle / Sil)
//   PbEgitim.yardim()              : araç çubuğundaki ampul düğmesi eski yardım penceresi gibi açıklamayı gösterir
window.PbEgitim = (function ($) {
    'use strict';

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }

    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }

    // ---------------------------------------------------------------- Onay penceresi
    var $onay = null;
    function onay(metin, baslik) {
        if (!$onay) {
            $onay = $('<dialog class="pb-onay" aria-labelledby="pbOnayBaslik">' +
                '<div class="pb-onay-baslik"><h2 id="pbOnayBaslik"></h2></div>' +
                '<div class="pb-onay-govde"></div>' +
                '<div class="pb-onay-alt">' +
                '<button type="button" class="btn btn-primary btn-sm" data-cevap="evet">Evet</button>' +
                '<button type="button" class="btn btn-default btn-sm" data-cevap="hayir">Hayır</button>' +
                '</div></dialog>').appendTo(document.body);
        }
        var pencere = $onay[0];
        $onay.find('h2').text(baslik || 'Onay');
        $onay.find('.pb-onay-govde').text(metin);
        return new Promise(function (coz, reddet) {
            $onay.off('.onay');
            $onay.on('click.onay', 'button[data-cevap]', function () {
                var evet = $(this).data('cevap') === 'evet';
                pencere.close();
                if (evet) coz(); else reddet();
            });
            $onay.on('cancel.onay', function () { reddet(); });
            if (typeof pencere.showModal === 'function') {
                pencere.showModal();
                $onay.find('[data-cevap="evet"]').trigger('focus');
            } else if (window.confirm(metin)) {
                coz();
            } else {
                reddet();
            }
        });
    }

    // ---------------------------------------------------------------- Seçim listeleri
    function secenekDoldur(secim, liste) {
        var $s = $(secim).empty();
        $.each(liste || [], function (i, o) { $('<option></option>').val(String(o.KOD)).text(o.AD).appendTo($s); });
        return $s;
    }

    function secimYap(secim, deger) {
        var $s = $(secim);
        var d = deger == null ? '' : String(deger);
        if (d !== '' && $s.find('option').filter(function () { return this.value === d; }).length) $s.val(d);
        else $s.prop('selectedIndex', 0);
        return $s;
    }

    function evetMi(x) { return x === true || (parseInt(x, 10) || 0) > 0; }

    function tarihHucre(p) { return Sgb.tarih(p.value || ''); }

    function isaretHucre(p) { return evetMi(p.value) ? '✔' : ''; }

    // ---------------------------------------------------------------- Kayıt listesi (salt okunur: sarı hücreler)
    function listeGrid(element, sutunlar, ek) {
        return Sgb.grid(element, $.extend({
            columnDefs: sutunlar,
            defaultColDef: { cellClass: 'hucre-salt' },
            rowSelection: 'single',
            getRowId: function (p) { return String(p.data.kod); },
            overlayNoRowsTemplate: '<span class="text-muted">Kayıt yok</span>'
        }, ek || {}));
    }

    // ---------------------------------------------------------------- Belgeler
    // ayar: { kok: 'api/personelbilgi/ogrenim/', grid: '#grdBelgeler', dugme: '#btnBelgeEkle', dosya: '#fuBelge',
    //         personelKod: int, kodAl: function () -> int, kaydetOnce: function () -> jQuery söz (kayıt kodu ile çözülür) }
    function belgeler(ayar) {
        var grid = Sgb.grid(ayar.grid, {
            columnDefs: [
                { headerName: '', valueGetter: 'node.rowIndex + 1', width: 48, sortable: false, cellClass: 'hucre-salt' },
                { field: 'belgeAdi', headerName: 'Belge', flex: 1, minWidth: 160, tooltipField: 'belgeAdi', cellClass: 'hucre-salt' },
                {
                    headerName: '', width: 44, sortable: false, cellClass: 'hucre-salt pb-belge-komut',
                    cellRenderer: function () {
                        return '<button type="button" class="btn btn-link btn-xs" data-komut="goster" title="Belgeyi Görüntüle">' +
                            '<span class="glyphicon glyphicon-search" aria-hidden="true"></span><span class="sr-only">Belgeyi Görüntüle</span></button>';
                    }
                },
                {
                    headerName: '', width: 44, sortable: false, cellClass: 'hucre-salt pb-belge-komut',
                    cellRenderer: function () {
                        return '<button type="button" class="btn btn-link btn-xs text-danger" data-komut="sil" title="Belgeyi Sil">' +
                            '<span class="glyphicon glyphicon-trash" aria-hidden="true"></span><span class="sr-only">Belgeyi Sil</span></button>';
                    }
                }
            ],
            domLayout: 'autoHeight',
            headerHeight: 0,
            getRowId: function (p) { return String(p.data.kod); },
            overlayNoRowsTemplate: '<span class="text-muted">Belge yok</span>',
            onCellClicked: function (e) {
                var $b = $(e.event && e.event.target).closest('button[data-komut]');
                if (!$b.length) return;
                if ($b.data('komut') === 'goster') goster(e.data);
                else sil(e.data);
            }
        });

        function temizle() { grid.setGridOption('rowData', []); }

        function yenile() {
            var kod = ayar.kodAl();
            if (!(kod > 0)) { temizle(); return $.Deferred().resolve().promise(); }
            grid.setGridOption('loading', true);
            return Sgb.al(ayar.kok + 'belgeler', { personelKod: ayar.personelKod, kod: kod })
                .done(function (v) {
                    grid.setGridOption('rowData', v.satirlar || []);
                    if (v.hata) mesaj(v.hata, 'danger');
                })
                .fail(hataGoster)
                .always(function () { grid.setGridOption('loading', false); });
        }

        function goster(b) {
            Sgb.indir(ayar.kok + 'belgeindir', { personelKod: ayar.personelKod, kod: ayar.kodAl(), belgeKod: b.kod });
        }

        function sil(b) {
            onay('Ekli doküman silinecek. Onaylıyor musunuz?', 'Uyarı').then(function () {
                Sgb.gonder(ayar.kok + 'belgesil', { personelKod: ayar.personelKod, kod: ayar.kodAl(), belgeKod: b.kod })
                    .done(function (s) {
                        if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                        yenile();
                    })
                    .fail(hataGoster);
            }, function () { });
        }

        function yukle(dosya, kod) {
            var veri = new FormData();
            veri.append('dosya', dosya);
            veri.append('personelKod', ayar.personelKod);
            veri.append('kod', kod);
            var $d = $(ayar.dugme).prop('disabled', true);
            return $.ajax({
                url: Sgb.KOK + ayar.kok + 'belgeyukle', method: 'POST', data: veri, processData: false, contentType: false,
                dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
            }).done(function (s) {
                if (!s.basarili) mesaj(s.hata, 'danger');
                else if (s.bilgi) mesaj(s.bilgi, 'success');
                yenile();
            }).fail(function (xhr) {
                if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
                else hataGoster(xhr);
            }).always(function () { $d.prop('disabled', false); });
        }

        $(ayar.dugme).on('click', function () { $(ayar.dosya).val('').trigger('click'); });
        $(ayar.dosya).on('change', function () {
            var dosya = this.files && this.files[0];
            if (!dosya) return;
            // Eski BelgeKaydet: kayıt henüz kaydedilmemişse önce Kaydet çalışır, sonra belge o kayda eklenir.
            var kod = ayar.kodAl();
            if (kod > 0) { yukle(dosya, kod); return; }
            $.when(ayar.kaydetOnce()).done(function (yeniKod) {
                if (yeniKod > 0) yukle(dosya, yeniKod);
            });
        });

        return { yenile: yenile, temizle: temizle };
    }

    // ---------------------------------------------------------------- Yardım (eski wndYardim: ampul düğmesinin üzerine gelince)
    function yardim() {
        var $d = $('#btnYardim'), $m = $('#pbYardim'), sabit = false;
        $d.on('mouseenter focus', function () { $m.prop('hidden', false); })
            .on('mouseleave blur', function () { if (!sabit) $m.prop('hidden', true); })
            .on('click', function () {
                sabit = !sabit;
                $m.prop('hidden', !sabit);
                $d.attr('aria-expanded', sabit ? 'true' : 'false');
            });
    }

    return {
        mesaj: mesaj, hataGoster: hataGoster, onay: onay, secenekDoldur: secenekDoldur, secimYap: secimYap, evetMi: evetMi,
        tarihHucre: tarihHucre, isaretHucre: isaretHucre, listeGrid: listeGrid, belgeler: belgeler, yardim: yardim
    };
})(jQuery);
