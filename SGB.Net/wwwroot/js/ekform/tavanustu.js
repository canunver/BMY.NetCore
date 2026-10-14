// Form 25: tavanı aşan ilave ödenek teklifleri (eski ButceForm/TavanUstuOdenek.aspx) ve
// Form 25.2: detay (eski ButceForm/TavanUstuOdenek2.aspx). p.detay: true ise Form 25.2.
// İki formda da görünür "Tavan Üstünden Al" tuşu vardı: bilgiler bütçe teklifindeki tavan üstü taleplerden okunur
// (TavanUstuListeleDetaydan), saklamak için Kaydet'e basılır.
// Form 25: kurumsal kod başına bir satır, açıklama ve 13 ekonomik kod tutarı; eski sayfa yalnız ilk satırı saklıyordu.
// Form 25.2: yedi sabit ekonomik kod satırı (kod ve ad kilitli), tavanı aşan tutar ve gerekçe.
(function ($) {
    'use strict';

    // Form 25 sütunları (eski FRMTUO009..022); sıra sunucudaki ekonomik kod dizisiyle aynıdır.
    var F25_EKO = [
        { no: '01', ad: 'Personel Giderleri' },
        { no: '02', ad: 'Sos. Güv. Kur. Devlet Primi Gid.' },
        { no: '03.1', ad: 'Üretime Yönelik Mal ve Malzeme Alımları', mal: true },
        { no: '03.2', ad: 'Tüketime Yönelik Mal ve Malzeme Alımları', mal: true },
        { no: '03.3', ad: 'Yolluklar', mal: true },
        { no: '03.4', ad: 'Görev Giderleri', mal: true },
        { no: '03.5', ad: 'Hizmet Alımları', mal: true },
        { no: '03.6', ad: 'Temsil ve Tanıtma Giderleri', mal: true },
        { no: '03.7', ad: 'Menkul Mal, Gayri Maddi Hak Alım, Bakım ve Onarım Giderleri', mal: true },
        { no: '03.8', ad: 'Gayrimenkul Mal ve Bakım ve Onarım Giderleri', mal: true },
        { no: '03.9', ad: 'Tedavi ve Cenaze Giderleri', mal: true },
        { no: '05', ad: 'Cari Transferler' },
        { no: '07', ad: 'Sermaye Transferleri' }
    ];

    function onayla(ctx) {
        return ctx.degisti ? EkForm.onay('Kaydedilmemiş değişiklikler var. Kaydetmeden devam edilsin mi?', 'Devam et')
            : $.Deferred().resolve().promise();
    }

    // "Tavan Üstünden Al": okunan bilgiler henüz saklanmadığından sayfa "değişti" sayılır.
    function tavanUstundenAl(ctx, oku) {
        var d = $.Deferred();
        onayla(ctx).done(function () {
            oku(ctx, true).done(function () {
                EkForm.degisti(ctx);
                Sgb.bildirim('Tavan üstü talepler okundu. Saklamak için Kaydet tuşuna basınız.', 'info');
                d.resolve();
            }).fail(function () { d.reject(); });
        }).fail(function () { d.reject(); });
        return d.promise();
    }

    // ------------------------------------------------------------------ Form 25

    function f25Ac(ctx) {
        $('<p class="form-aciklama"></p>').text('Tavanı aşan ilave ödenek teklifleri').appendTo(ctx.$govde);
        ctx.$not = $('<p class="text-muted small" hidden></p>')
            .text('Birden çok birimin talebi listelendi. Eski formda olduğu gibi yalnız ilk satır saklanır; diğer satırlar bilgi içindir.')
            .appendTo(ctx.$govde);
        var $t = $('<div class="ekform-grid kisa" aria-label="Tavan üstü ödenekler"></div>').appendTo(ctx.$govde);

        var ilk = function (p) { return !!(p.data && p.data.ilk); };
        function ekoSutun(e, i) {
            return EkForm.sayiSutun('e' + i, e.ad, { girilir: ilk, width: e.mal ? 105 : 115, headerTooltip: e.ad });
        }
        var mal = [];
        var kolonlar = [
            { headerName: '', children: [EkForm.metinSutun('birim', 'Birim', { girilir: false, width: 110, pinned: 'left' })] },
            { headerName: '', children: [EkForm.metinSutun('aciklama', 'Açıklama', { girilir: ilk, width: 250, pinned: 'left', uzun: true })] }
        ];
        $.each(F25_EKO, function (i, e) {
            if (e.mal) { mal.push(ekoSutun(e, i)); if (e.no === '03.9') kolonlar.push({ headerName: 'Mal ve Hizmet Alım Giderleri', headerClass: 'orta', children: mal }); }
            else kolonlar.push({ headerName: '', children: [ekoSutun(e, i)] });
        });

        ctx.tablo = EkForm.grid(ctx, $t[0], { columnDefs: kolonlar, rowData: [], headerHeight: 70 });
    }

    function f25Oku(ctx, detaydan) {
        return EkForm.al('api/ekform/tavanustu/liste', {
            yil: ctx.secim.yil, kur: ctx.secim.kur, senaryo: ctx.secim.senaryo, detaydan: !!detaydan
        }, 'Tavan üstü ödenekler okunamadı.').done(function (v) {
            var liste = $.map(v.satirlar || [], function (s, j) {
                var r = { ilk: j === 0, birim: s.birim, aciklama: s.aciklama || '' };
                for (var i = 0; i < F25_EKO.length; i++) r['e' + i] = s.tutarlar ? s.tutarlar[i] : null;
                return r;
            });
            ctx.$not.prop('hidden', liste.length < 2);
            ctx.tablo.setGridOption('rowData', liste);
        });
    }

    function f25Kaydet(ctx) {
        var r = EkForm.satirlar(ctx.tablo)[0] || {};
        var t = [];
        for (var i = 0; i < F25_EKO.length; i++) t.push(r['e' + i] == null ? null : r['e' + i]);
        return EkForm.kaydet(ctx, 'api/ekform/tavanustu/kaydet', {
            yil: ctx.secim.yil, kur: ctx.secim.kur, aciklama: r.aciklama || '', tutarlar: t
        });
    }

    // ------------------------------------------------------------------ Form 25.2

    function f252Ac(ctx) {
        $('<p class="form-aciklama"></p>').text('Tavanı aşan ilave ödenek teklifleri (detay)').appendTo(ctx.$govde);
        var $t = $('<div class="ekform-grid" aria-label="Tavan üstü ödenek detayı"></div>').appendTo(ctx.$govde);
        ctx.tablo = EkForm.grid(ctx, $t[0], {
            columnDefs: [
                { headerName: 'Ekonomik Kod', headerClass: 'orta', children: [
                    EkForm.metinSutun('kod', 'Kod', { girilir: false, width: 110, pinned: 'left' }),
                    EkForm.metinSutun('ad', 'Ad', { girilir: false, width: 280, pinned: 'left' })
                ] },
                { headerName: '', children: [EkForm.sayiSutun('tutar', 'Tavanı Aşan Tutar', { width: 140 })] },
                { headerName: '', children: [EkForm.metinSutun('aciklama', 'Gerekçe', { width: 420, uzun: true, wrapText: true, autoHeight: true })] }
            ],
            rowData: []
        });
    }

    function f252Oku(ctx, detaydan) {
        return EkForm.al('api/ekform/tavanustu/detayliste', {
            yil: ctx.secim.yil, kur: ctx.secim.kur, senaryo: ctx.secim.senaryo, detaydan: !!detaydan
        }, 'Tavan üstü ödenekler okunamadı.').done(function (v) {
            ctx.tablo.setGridOption('rowData', $.map(v.satirlar || [], function (s) {
                return { ekoKod: s.ekoKod, kod: s.kod, ad: s.ad, tutar: s.tutar, aciklama: s.aciklama || '' };
            }));
            // Eski sayfa: satırlar doldurulur, okuma hatası ayrıca bildirilir.
            if (v.hataBilgi) Sgb.bildirim(v.hataBilgi, 'danger');
        });
    }

    function f252Kaydet(ctx) {
        var satirlar = $.map(EkForm.satirlar(ctx.tablo), function (r) {
            return { ekoKod: r.ekoKod, tutar: r.tutar == null ? null : r.tutar, aciklama: r.aciklama || '' };
        });
        return EkForm.kaydet(ctx, 'api/ekform/tavanustu/detaykaydet', { yil: ctx.secim.yil, kur: ctx.secim.kur, satirlar: satirlar });
    }

    // ------------------------------------------------------------------

    EkForm.tanimla('tavanUstu', {
        ac: function (ctx) { if (ctx.p.detay) f252Ac(ctx); else f25Ac(ctx); },

        listele: function (ctx) { return ctx.p.detay ? f252Oku(ctx, false) : f25Oku(ctx, false); },

        kaydet: function (ctx) { return ctx.p.detay ? f252Kaydet(ctx) : f25Kaydet(ctx); },

        yazdir: function (ctx) {
            if (ctx.p.detay)
                return EkForm.indir('api/ekform/tavanustu/detayyazdir', { yil: ctx.secim.yil, kur: ctx.secim.kur, senaryo: ctx.secim.senaryo });
            return EkForm.indir('api/ekform/tavanustu/yazdir', { yil: ctx.secim.yil, kur: ctx.secim.kur });
        },

        dugmeler: function () {
            return [{
                ad: 'Tavan Üstünden Al', ikon: 'flash', sinif: 'btn-default',
                ipucu: 'Bütçe teklifindeki tavan üstü talepleri forma okur (saklamak için Kaydet)',
                tikla: function (ctx) { return tavanUstundenAl(ctx, ctx.p.detay ? f252Oku : f25Oku); }
            }];
        }
    });
})(jQuery);
