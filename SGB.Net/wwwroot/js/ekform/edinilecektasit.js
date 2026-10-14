// Form 18 - 237 Sayılı Taşıt Kanununa göre edinilecek taşıtlar (eski ButceForm/EdinilecekTasitlar.aspx).
// Eski tablo Ext ızgarasıydı: taşıt cetveli ve finansman kaynağı açılır listeden seçilir; "Boş Satır Aç" sona satır ekler,
// "Satır Sil" seçili satırları tablodan çıkarır (ikisi de Kaydet'e kadar yalnızca tabloda kalır). Kaydet onay sorar.
(function ($) {
    'use strict';

    function bosSatir() { return { siraNo: '', adet: null, kullanimYeri: '', finkaynak: '' }; }

    EkForm.tanimla('edinilecekTasit', {
        ac: function (ctx) {
            ctx.sozluk = [];
            ctx.kaynaklar = [];
            $('<p class="form-aciklama"></p>').text('237 Sayılı Taşıt Kanununa göre edinilecek taşıtlar').appendTo(ctx.$govde);
            var $t = $('<div class="ekform-grid" aria-label="Edinilecek taşıtlar"></div>').appendTo(ctx.$govde);
            ctx.tablo = EkForm.grid(ctx, $t[0], {
                rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
                columnDefs: [
                    { headerName: '#', width: 60, valueGetter: function (p) { return p.node.rowIndex + 1; }, cellClass: 'sayi kapali', headerClass: 'sayi' },
                    EkForm.secimSutun('siraNo', 'T Cetveli Sıra No', function () { return ctx.sozluk; }, { width: 300 }),
                    EkForm.sayiSutun('adet', 'Adet', { width: 80 }),
                    EkForm.metinSutun('kullanimYeri', 'Kullanım Yeri', { width: 250 }),
                    EkForm.secimSutun('finkaynak', 'Fin. Kaynağı', function () { return ctx.kaynaklar; }, { width: 240 })
                ],
                rowData: []
            });
        },

        dugmeler: function (ctx) {
            return [
                {
                    ad: 'Boş Satır Aç', ikon: 'plus', ipucu: 'Tablonun sonuna boş satır ekler',
                    tikla: function () {
                        var r = ctx.tablo.applyTransaction({ add: [bosSatir()] });
                        EkForm.degisti(ctx);
                        if (r && r.add && r.add[0]) ctx.tablo.ensureNodeVisible(r.add[0]);
                    }
                },
                {
                    ad: 'Satır Sil', ikon: 'trash', ipucu: 'İşaretli satırları (işaretli yoksa imlecin bulunduğu satırı) tablodan çıkarır',
                    tikla: function () {
                        var sil = ctx.tablo.getSelectedRows();
                        if (!sil.length) {
                            var h = ctx.tablo.getFocusedCell();
                            var n = h ? ctx.tablo.getDisplayedRowAtIndex(h.rowIndex) : null;
                            if (n && n.data) sil = [n.data];
                        }
                        if (!sil.length) { Sgb.bildirim('Silinecek satırı seçiniz.', 'warning'); return; }
                        ctx.tablo.applyTransaction({ remove: sil });
                        ctx.tablo.refreshCells({ force: true });
                        EkForm.degisti(ctx);
                    }
                }
            ];
        },

        listele: function (ctx) {
            return EkForm.al('api/ekform/edinilecektasit/liste', { yil: ctx.secim.yil, kur: ctx.secim.kur },
                'Edinilecek taşıtlar okunamadı.').done(function (v) {
                ctx.sozluk = v.sozluk || [];
                ctx.kaynaklar = v.kaynaklar || [];
                ctx.tablo.setGridOption('rowData', v.satirlar || []);
            });
        },

        kaydet: function (ctx) {
            return EkForm.onay('Veriler kaydedilecek onaylıyor musunuz?', 'Kaydet').then(function () {
                var satirlar = $.map(EkForm.satirlar(ctx.tablo), function (r) {
                    return { siraNo: r.siraNo || '', adet: r.adet == null || r.adet === '' ? null : r.adet, kullanimYeri: r.kullanimYeri || '', finkaynak: r.finkaynak || '' };
                });
                return EkForm.kaydet(ctx, 'api/ekform/edinilecektasit/kaydet', { yil: ctx.secim.yil, kur: ctx.secim.kur, satirlar: satirlar });
            });
        },

        yazdir: function (ctx) {
            return EkForm.indir('api/ekform/edinilecektasit/yazdir', { yil: ctx.secim.yil, kur: ctx.secim.kur });
        }
    });
})(jQuery);
