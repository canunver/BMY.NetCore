// Tanım Talep Formu (ButceMuhasebe/TanimTKDKTalepForm): solda talep formları (yıl, ay, talep türü), sağda seçili form ve satırları.
// Nakit talebinde satırlar tabloda elle girilir; Ödenek Aktarma / Ek Ödenek'te 830 altındaki ekonomik kodlardan eklenir.
// Uçlar: /api/muhtanim/tkdk/*.
$(function () {
    'use strict';

    if (!$('#grdListe').length) return;

    var form = Sgb.mt.form('#frmBilgi');
    var sayac = 0;
    function yeniKod() { return 'y' + (++sayac); } // yeni satırın geçici kimliği; kayıtta sunucu verir

    function nakit() { return $('#tkTur').val() === '1'; }
    function girilir() { return nakit(); }

    function hesapKutu() { return $('#tkHesap').data('hesapKodu') || Sgb.hesapKodu('#tkHesap'); }

    var satirTablo = Sgb.grid('#grdSatir', {
        columnDefs: [
            { field: 'HESAPKOD', headerName: 'Hesap Kodu', width: 130, editable: girilir, cellClass: function () { return nakit() ? 'mt-girilir' : 'mt-sari'; } },
            { field: 'HESAPADI', headerName: 'Açıklama', flex: 1, minWidth: 200, editable: girilir, cellClass: function () { return nakit() ? 'mt-girilir' : 'mt-sari'; } },
            {
                colId: 'sil', headerName: '', width: 52, sortable: false, resizable: false,
                cellRenderer: function () {
                    return '<button type="button" class="btn btn-link btn-xs mt-satir-sil" title="Satırı sil"><span class="glyphicon glyphicon-remove text-danger" aria-hidden="true"></span><span class="sr-only">Satırı sil</span></button>';
                },
                onCellClicked: function (p) { satirTablo.applyTransaction({ remove: [p.data] }); }
            }
        ],
        getRowId: function (p) { return String(p.data.KOD); },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true
    });

    function turAyarla() {
        var n = nakit();
        $('.tk-hesap-alan').prop('hidden', n);
        hesapKutu().temizle();
        $('#tkIpucu').text(n ? 'Nakit talebinde hesap kodu ve açıklama tabloda girilir; Ekle boş satır ekler.'
            : 'Ekonomik kodu (830.XX.XX) yazın ya da mercekle seçin ve Ekle\'ye basın; tabloya XX.XX olarak eklenir. Satırlar Sil sütunuyla kaldırılır.');
        satirTablo.refreshCells({ force: true });
    }

    var liste = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'YIL', headerName: 'Yıl', width: 80 },
            { field: 'AY', headerName: 'Ay', width: 110 },
            { field: 'TALEPTUR', headerName: 'Talep Türü', flex: 1, minWidth: 140 }
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        getRowId: function (p) { return String(p.data.KOD); },
        onRowClicked: function (e) { satirlariGetir(e.data.KOD); }
    });

    function listele() {
        return Sgb.mt.oku('api/muhtanim/tkdk/liste', {}, liste).done(function (v) {
            liste.setGridOption('rowData', v.satirlar || []);
        });
    }

    function temizle() {
        var d = new Date();
        form.temizle({ yil: d.getFullYear(), ay: d.getMonth() + 1 });
        satirTablo.setGridOption('rowData', []);
        liste.deselectAll();
        turAyarla();
    }

    function satirlariGetir(kod) {
        Sgb.mt.oku('api/muhtanim/tkdk/oku', { kod: kod }, satirTablo).done(function (v) {
            form.temizle();
            form.yaz({ kod: v.kod, yil: v.yil, ay: v.ay, talepTur: v.talepTur });
            var satirlar = v.satirlar || [];
            // Kayıtta noktasız tutulan ekonomik kod (0102) tabloda XX.XX gösterilir.
            if (String(v.talepTur) !== '1')
                $.each(satirlar, function (i, r) { var k = String(r.HESAPKOD || ''); if (/^\d{4}$/.test(k)) r.HESAPKOD = k.substr(0, 2) + '.' + k.substr(2); });
            satirTablo.setGridOption('rowData', satirlar);
            turAyarla();
            var n = liste.getRowNode(String(v.kod));
            if (n) n.setSelected(true);
        });
    }

    function istek() {
        var f = form.oku();
        satirTablo.stopEditing();
        var satirlar = [];
        satirTablo.forEachNode(function (n) {
            var s = n.data;
            // Hesap / ekonomik kod her talep türünde noktasız kaydedilir (tabloda 01.02 -> 0102).
            var hk = String(s.HESAPKOD || '');
            hk = hk.replace(/\./g, '');
            satirlar.push({ KOD: /^y\d+$/.test(String(s.KOD)) ? '' : s.KOD, HESAPKOD: hk, HESAPADI: s.HESAPADI || '' });
        });
        return { kod: f.kod, yil: parseInt(f.yil, 10) || 0, ay: parseInt(f.ay, 10) || 0, talepTur: parseInt(f.talepTur, 10) || 0, satirlar: satirlar };
    }

    function sonra(v) {
        listele().always(function () { if (v.anahtar) satirlariGetir(v.anahtar); });
    }

    $('#btnKaydet').on('click', function () {
        var i = istek();
        if (!i.talepTur) { Sgb.bildirim('Lütfen talep türünü seçin.', 'warning'); return; }
        Sgb.mt.islem('api/muhtanim/tkdk/kaydet', i).done(sonra);
    });

    $('#btnSil').on('click', function () {
        var kod = form.oku().kod;
        if (!kod) { Sgb.bildirim('Lütfen silmek için kayıt seçiniz.', 'warning'); return; }
        Sgb.mt.islem('api/muhtanim/tkdk/sil', { kod: kod }, 'Seçili talep formu silinecek. Emin misiniz?').done(function () {
            temizle();
            listele();
        });
    });

    $('#btnSonrakiAy').on('click', function () {
        var kod = form.oku().kod;
        if (!kod) { Sgb.bildirim('Lütfen aktarılacak kaydı seçiniz.', 'warning'); return; }
        Sgb.mt.islem('api/muhtanim/tkdk/sonraki-ay', { kod: kod }, 'Seçili talep formu bir sonraki aya kopyalanacak. Emin misiniz?').done(sonra);
    });

    $('#btnTemizle').on('click', temizle);
    $('#tkTur').on('change', turAyarla);

    $('#btnSatirEkle').on('click', function () {
        if (nakit()) {
            satirTablo.applyTransaction({ add: [{ KOD: yeniKod(), HESAPKOD: '', HESAPADI: '' }] });
            return;
        }
        if (!$('#tkTur').val()) { Sgb.bildirim('Lütfen önce talep türünü seçin.', 'warning'); return; }
        // 830.XX.XX düzeyinde hesap; tabloya 830. olmadan XX.XX olarak eklenir.
        hesapKutu().hazir().done(satirEkle);
    });

    function satirEkle(h) {
        if (!h.kod) { Sgb.bildirim('Ekonomik kodu yazın ya da mercekle seçin.', 'warning'); $('#tkHesap').trigger('focus'); return; }
        if (!h.gecerli || !h.ad) { Sgb.bildirim('Ekonomik kod hesap planında 830.XX.XX düzeyinde kayıtlı olmalı.', 'warning'); return; }
        var kod = h.kod.replace(/^830\./, '');
        var varMi = false;
        satirTablo.forEachNode(function (n) { if (String(n.data.HESAPKOD).replace(/\./g, '') === kod.replace(/\./g, '')) varMi = true; });
        if (varMi) Sgb.bildirim('Bu ekonomik kod tabloda zaten var.', 'warning');
        else satirTablo.applyTransaction({ add: [{ KOD: yeniKod(), HESAPKOD: kod, HESAPADI: h.ad }] });
        hesapKutu().temizle();
    }

    temizle();
    listele();
});
