// Makbuz ekranlarının seçim pencereleri (ButceMuhasebe/Makbuz ve MakbuzGiris):
//   Sgb.hesapPlaniSec(o) : hesap planı ağacı ve hesap kodu / adı ile arama (eski ListeHesapPlani.aspx ve HesapKod.ascx)
//   Sgb.firmaSec(o)      : kişi / firma arama (eski HarcamaSurecleri/ListeFirmaKisi.aspx)
// Uçlar: /api/makbuz/hesap/agac, hesap/ara, firma/ara (Muhasebe/MakbuzUclari.cs). Pencere bir kez kurulur, sonraki açılışlarda
// aynı pencere kullanılır.
//
// Kullanım:
//   Sgb.hesapPlaniSec({ yil: 2026, sonKirilim: true, secilince: function (h) { h.kod, h.hesapAd, h.detay } });
//     kok: '830' verilirse ağaç o hesabın altından başlar.
//     uc: hesap uçlarının kökü (isteğe bağlı; varsayılan api/makbuz/hesap/). Örn. Ödeme Bilgileri api/alacaktakip/odeme/hesap/ kullanır.
//   Sgb.firmaSec({ muhasebe: 1, ara: '123', secilince: function (f) { f.vergiNo, f.ad, f.kod (arama ucu kod döndürüyorsa) } });
//     adres: arama ucu (isteğe bağlı; varsayılan api/makbuz/firma/ara). Örn. Alacak Takip kendi yetkisiyle api/alacaktakip/takip/firma/ara kullanır.
(function (Sgb, $) {
    'use strict';

    function hataGoster(xhr, metin) {
        if (xhr && xhr.status === 401) return;
        Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : metin, 'danger');
    }

    function pencereKur(kimlik, baslik, govde, alt) {
        var $p = $('<dialog class="tanim-pencere mk-pencere"></dialog>').attr({ id: kimlik, 'aria-labelledby': kimlik + 'Baslik' })
            .append($('<div class="pencere-baslik"></div>')
                .append($('<h2></h2>').attr('id', kimlik + 'Baslik').text(baslik))
                .append('<button type="button" class="close" data-kapat aria-label="Kapat"><span aria-hidden="true">&times;</span></button>'))
            .append($('<div class="pencere-govde"></div>').append(govde))
            .append($('<div class="pencere-alt"></div>').append(alt || '').append('<button type="button" class="btn btn-default btn-sm" data-kapat>Kapat</button>'))
            .appendTo(document.body);
        $p.on('click', '[data-kapat]', function (e) { e.stopPropagation(); $p[0].close(); });
        return $p;
    }

    // ---------------------------------------------------------------- hesap planı

    var hesap = null; // { $p, agac, sonuc, o }

    function hesapPlaniSec(o) {
        o = o || {};
        if (!hesap) {
            var $govde = $('<div></div>')
                .append('<form class="mk-ara" novalidate><label>Hesap kodu / adı <input type="text" class="form-control input-sm mk-hesap-ara" autocomplete="off" /></label>'
                    + '<span class="mk-ara-tus"><button type="submit" class="btn btn-primary btn-sm"><span class="glyphicon glyphicon-search" aria-hidden="true"></span> Ara</button>'
                    + '<button type="button" class="btn btn-default btn-sm mk-hesap-agac">Hesap Planı</button></span></form>')
                .append('<p class="help-block small">Seçmek için hesabın adına tıklayınız.</p>')
                .append('<ul class="deger-liste mk-hesap-dal" aria-label="Hesap planı"></ul>')
                .append('<ul class="deger-liste mk-hesap-sonuc" aria-label="Arama sonucu" hidden></ul>');
            hesap = { $p: pencereKur('dlgHesapPlani', 'Hesap Planı', $govde), agac: null, sonuc: null, sonucMetin: '', o: o };

            var sec = function (d) {
                hesap.$p[0].close();
                if (hesap.o.secilince) hesap.o.secilince({ kod: d.kod, hesapAd: d.hesapAd, detay: d.detay });
            };

            hesap.$p.find('form').on('submit', function (e) {
                e.preventDefault();
                var metin = $.trim(hesap.$p.find('.mk-hesap-ara').val());
                if (metin.length < 2) { Sgb.bildirim('En az iki karakter yazınız.', 'warning'); return; }
                hesap.sonucMetin = metin;
                hesap.$p.find('.mk-hesap-dal').prop('hidden', true);
                var $s = hesap.$p.find('.mk-hesap-sonuc').prop('hidden', false);
                var yukle = function () {
                    return Sgb.al((hesap.o.uc || 'api/makbuz/hesap/') + 'ara', { yil: hesap.o.yil || 0, metin: hesap.sonucMetin, sonKirilim: hesap.o.sonKirilim === false ? '0' : '1' })
                        .then(function (v) {
                            if (v.hata) { Sgb.bildirim(v.hata, 'warning'); return []; }
                            return v.satirlar;
                        });
                };
                if (hesap.sonuc) hesap.sonuc.yenile();
                else hesap.sonuc = Sgb.agac($s[0], {
                    duz: true, yukle: yukle, secilince: sec, bosMetin: 'Aranan hesap bulunamadı.',
                    hata: function (xhr) { hataGoster(xhr, 'Arama yapılamadı.'); }
                });
            });

            hesap.$p.find('.mk-hesap-agac').on('click', function () {
                hesap.$p.find('.mk-hesap-sonuc').prop('hidden', true);
                hesap.$p.find('.mk-hesap-dal').prop('hidden', false);
            });
        }

        var yilDegisti = hesap.agac && (hesap.o.yil !== o.yil || (hesap.o.kok || '') !== (o.kok || ''));
        hesap.o = o;
        hesap.$p[0].showModal();
        if (!hesap.agac) {
            hesap.agac = Sgb.agac(hesap.$p.find('.mk-hesap-dal')[0], {
                yukle: function (ust) {
                    return Sgb.al((hesap.o.uc || 'api/makbuz/hesap/') + 'agac', { yil: hesap.o.yil || 0, ust: ust ? ust.kod : (hesap.o.kok || '') }).then(function (v) {
                        if (v.hata) { Sgb.bildirim(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    // Kökteki 1-9 ana gruplar seçilmez, yalnızca açılır.
                    if (d.kod.length < 3) return;
                    hesap.$p[0].close();
                    if (hesap.o.secilince) hesap.o.secilince({ kod: d.kod, hesapAd: d.hesapAd, detay: d.detay });
                },
                bosMetin: 'Hesap planı bulunamadı.',
                hata: function (xhr) { hataGoster(xhr, 'Hesap planı okunamadı.'); }
            });
        } else if (yilDegisti) hesap.agac.yenile();
        hesap.$p.find('.mk-hesap-ara').trigger('focus');
    }

    // ---------------------------------------------------------------- kişi / firma

    var firma = null; // { $p, grid, o }

    function firmaSec(o) {
        o = o || {};
        if (!firma) {
            var $govde = $('<div></div>')
                .append('<form class="mk-ara" novalidate>'
                    + '<label>Kimlik / Vergi No <input type="text" class="form-control input-sm mk-firma-no" autocomplete="off" /></label>'
                    + '<label>Adı <input type="text" class="form-control input-sm mk-firma-ad" autocomplete="off" /></label>'
                    + '<span class="mk-ara-tus"><button type="submit" class="btn btn-primary btn-sm"><span class="glyphicon glyphicon-search" aria-hidden="true"></span> Ara</button></span></form>')
                .append('<p class="help-block small">Seçmek için satıra çift tıklayınız.</p>')
                .append('<div class="mk-grid mk-grid-pencere mk-firma-grid" aria-label="Kişi / firmalar"></div>');
            var $alt = $('<button type="button" class="btn btn-default btn-sm mk-firma-yeni" title="Kişi / firma kaydı (eski Kişi / Firma Listesi sayfası)"><span class="glyphicon glyphicon-plus" aria-hidden="true"></span> Yeni Kişi / Firma</button>');
            firma = { $p: pencereKur('dlgFirma', 'Kişi / Firma Listesi', $govde, $alt), grid: null, o: o };

            firma.grid = Sgb.grid(firma.$p.find('.mk-firma-grid')[0], {
                columnDefs: [
                    { field: 'vergiNo', headerName: 'Kimlik / Vergi No', width: 150 },
                    { field: 'ad', headerName: 'Adı', flex: 1, minWidth: 220 },
                    { field: 'vergiDairesi', headerName: 'Vergi Dairesi', width: 150 }
                ],
                overlayNoRowsTemplate: '<span>Kimlik / vergi no ya da ad yazıp Ara\'ya basınız.</span>',
                onRowDoubleClicked: function (e) {
                    firma.$p[0].close();
                    if (firma.o.secilince) firma.o.secilince({ vergiNo: e.data.vergiNo, ad: e.data.ad, adres: e.data.adres, kod: e.data.kod });
                }
            });

            firma.$p.find('form').on('submit', function (e) {
                e.preventDefault();
                var no = $.trim(firma.$p.find('.mk-firma-no').val()), ad = $.trim(firma.$p.find('.mk-firma-ad').val());
                if (!no && !ad) { Sgb.bildirim('Aramak için ad ya da kimlik / vergi no yazınız.', 'warning'); return; }
                firma.grid.setGridOption('loading', true);
                Sgb.al(firma.o.adres || 'api/makbuz/firma/ara', { muhasebe: firma.o.muhasebe || 0, ad: ad, vergiNo: no })
                    .done(function (v) {
                        if (v.hata) { Sgb.bildirim(v.hata, 'warning'); return; }
                        firma.grid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                        firma.grid.setGridOption('rowData', v.satirlar || []);
                    })
                    .fail(function (xhr) { hataGoster(xhr, 'Arama yapılamadı.'); })
                    .always(function () { firma.grid.setGridOption('loading', false); });
            });

            // Kişi / firma kaydı henüz taşınmadı: eski sayfa pencerede açılır (eski FirmaBilgisiAc(..., yeni)).
            $alt.on('click', function () {
                var no = $.trim(firma.$p.find('.mk-firma-no').val());
                firma.$p[0].close();
                Sgb.pencere('../HarcamaSurecleri/ListeFirmaKisi.aspx?' + $.param({ per: 1, muhasebe: firma.o.muhasebe || '', yeni: no }), 'Kişi / Firma Listesi');
            });
        }
        firma.o = o;
        firma.$p.find('.mk-firma-no').val(o.ara || '');
        firma.$p.find('.mk-firma-ad').val('');
        firma.$p[0].showModal();
        if (o.ara) firma.$p.find('form').trigger('submit');
        else firma.$p.find('.mk-firma-no').trigger('focus');
    }

    Sgb.hesapPlaniSec = hesapPlaniSec;
    Sgb.firmaSec = firmaSec;
})(window.Sgb, jQuery);
