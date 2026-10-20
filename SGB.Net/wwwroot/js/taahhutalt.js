// Taahhüt Kartı'nın alt sayfaları (eski ButceMuhasebe/*.aspx, Taahhüt Kartı'ndan pencere olarak açılır):
//   data-sayfa="plan"     Taahhutler            ?kod&dakod     /api/taahhutkarti/plan/liste|kaydet|sil
//   data-sayfa="bakim"    TaahhutBakimTarihce   ?taahhutKod    /api/taahhutkarti/bakim/liste|kaydet|sil
//   data-sayfa="sozlesme" TanimSozlesme         ?taahhutKod&tur /api/taahhutkarti/sozlesme-metni(/kaydet)
//   data-sayfa="belge"    TanimBelgeEkle        ?ilgiKod&tip   /api/taahhutkarti/belge-ekle/liste|yukle|indir|sil
$(function () {
    'use strict';

    var $kok = $('.th-alt[data-sayfa]');
    if (!$kok.length) return;

    var UC = 'api/taahhutkarti/';
    var tutar = Sgb.mt.tutar;
    var q = new URLSearchParams(window.location.search);
    function sayiQ(ad) { return parseInt(q.get(ad), 10) || 0; }
    function deger(secici) { return $.trim(String($(secici).val() == null ? '' : $(secici).val())); }
    function uyar(metin) { Sgb.bildirim(metin, 'warning'); }

    // Pencere olarak açıldıysa Kapat tuşu görünür (eski parent.hidePopWin).
    function cerceve() {
        try { return window.frameElement ? window.frameElement.closest('dialog') : null; } catch (e) { return null; }
    }
    if (cerceve()) $('#btnKapat').prop('hidden', false).on('click', function () { cerceve().close(); });

    var SAYFALAR = { plan: plan, bakim: bakim, sozlesme: sozlesme, belge: belge };
    SAYFALAR[$kok.data('sayfa')]();

    // ------------------------------------------------------------------ Taahhüt Planı (eski Taahhutler.aspx)

    function plan() {
        var kod = sayiQ('kod');
        var daKod = $.trim(q.get('dakod') || '');
        var TUTARLAR = ['ilkTaahhutTutari', 'fiyatFarki', 'ilaveTaahhutler', 'digerTaahhutler'];
        function tutarSutun(alan, baslik) {
            return { field: alan, headerName: baslik, width: 125, editable: true, cellClass: 'th-girilir', type: 'rightAligned',
                cellEditor: 'agNumberCellEditor', valueParser: function (p) { return Number(p.newValue) || 0; },
                valueFormatter: function (p) { return tutar(p.value || 0); } };
        }
        var grd = Sgb.grid('#grdPlan', {
            columnDefs: [
                { headerName: '', valueGetter: 'node.rowIndex + 1', width: 50, cellClass: 'th-salt' },
                { field: 'yil', headerName: 'Yıl', width: 80, editable: true, cellClass: 'th-girilir', cellEditor: 'agNumberCellEditor',
                    cellEditorParams: { min: 1980, max: 2100, precision: 0 }, valueParser: function (p) { return parseInt(p.newValue, 10) || 0; } },
                { field: 'ay', headerName: 'Ay', width: 65, editable: true, cellClass: 'th-girilir', cellEditor: 'agNumberCellEditor',
                    cellEditorParams: { min: 1, max: 12, precision: 0 }, valueParser: function (p) { return parseInt(p.newValue, 10) || 0; } },
                { field: 'tertipKod', headerName: 'Tertip', flex: 1, minWidth: 220, editable: true, cellClass: 'th-girilir' },
                tutarSutun('ilkTaahhutTutari', 'Taah. Tutarı'),
                tutarSutun('fiyatFarki', 'Fiyat Farkları'),
                tutarSutun('ilaveTaahhutler', 'İlave Taahhütler'),
                tutarSutun('digerTaahhutler', 'Diğer Taahhütler'),
                { field: 'toplam', headerName: 'Toplam', width: 130, type: 'rightAligned', cellClass: 'th-salt',
                    valueFormatter: function (p) { return tutar(p.value || 0); } }
            ],
            getRowId: function (p) { return String(p.data._id); },
            rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
            singleClickEdit: true,
            stopEditingWhenCellsLoseFocus: true,
            onCellValueChanged: function (e) {
                e.data._degisti = true;
                e.data.toplam = TUTARLAR.reduce(function (t, a) { return t + (Number(e.data[a]) || 0); }, 0);
                e.api.refreshCells({ rowNodes: [e.node], columns: ['toplam'], force: true });
                toplamYaz();
            },
            onSelectionChanged: function () { $('#btnSil').prop('disabled', !grd.getSelectedRows().length); }
        });
        var sira = 0;

        function toplamYaz() {
            var t = 0;
            grd.forEachNode(function (n) { t += Number(n.data.toplam) || 0; });
            $('#lblToplam').text('Toplam: ' + tutar(t));
        }

        function listele() {
            var p = { kod: kod };
            if (daKod) p.dakod = daKod;
            Sgb.mt.oku(UC + 'plan/liste', p, grd).done(function (v) {
                grd.setGridOption('rowData', $.map(v.satirlar || [], function (s) { return $.extend({ _id: ++sira }, s); }));
                $('#btnSil').prop('disabled', true);
                toplamYaz();
            });
        }

        $('#btnSatirEkle').on('click', function () {
            var r = grd.applyTransaction({ add: [{ _id: ++sira, _degisti: true, kod: -1, yil: new Date().getFullYear(), ay: 1, tertipKod: '',
                ilkTaahhutTutari: 0, fiyatFarki: 0, ilaveTaahhutler: 0, digerTaahhutler: 0, toplam: 0 }] });
            if (r && r.add && r.add.length) grd.startEditingCell({ rowIndex: r.add[0].rowIndex, colKey: 'yil' });
        });

        $('#btnKaydet').on('click', function () {
            grd.stopEditing();
            var satirlar = [];
            grd.forEachNode(function (n) {
                var d = n.data;
                if (!d._degisti) return;
                satirlar.push({ kod: d.kod || -1, yil: parseInt(d.yil, 10) || 0, ay: parseInt(d.ay, 10) || 0, tertipKod: $.trim(d.tertipKod || ''),
                    ilkTaahhutTutari: Number(d.ilkTaahhutTutari) || 0, fiyatFarki: Number(d.fiyatFarki) || 0,
                    ilaveTaahhutler: Number(d.ilaveTaahhutler) || 0, digerTaahhutler: Number(d.digerTaahhutler) || 0 });
            });
            if (!satirlar.length) { uyar('Kayıt edilecek bilgi bulunamadı.'); return; }
            Sgb.mt.islem(UC + 'plan/kaydet', { kod: kod, daKod: daKod, satirlar: satirlar }, 'Bilgiler kayıt edilecektir. Onaylıyor musunuz?').done(listele);
        });

        $('#btnSil').on('click', function () {
            var s = grd.getSelectedRows();
            if (!s.length) { uyar('Lütfen silmek istediğiniz bilgiyi seçin.'); return; }
            var kayitli = $.map(s, function (r) { return r.kod > 0 ? r.kod : null; });
            var yeni = $.grep(s, function (r) { return !(r.kod > 0); });
            if (!kayitli.length) { grd.applyTransaction({ remove: yeni }); toplamYaz(); return; }
            Sgb.mt.islem(UC + 'plan/sil', { kodlar: kayitli }, 'Seçili kayıt(lar) silinecektir. Onaylıyor musunuz?').done(listele);
        });

        if (!kod) { Sgb.bildirim('Taahhüt kartı bilgisi alınamadı.', 'danger'); return; }
        listele();
    }

    // ------------------------------------------------------------------ Bakım Tarihçesi (eski TaahhutBakimTarihce.aspx)

    function bakim() {
        var taahhutKod = sayiQ('taahhutKod');
        var grd = Sgb.grid('#grdBakim', {
            columnDefs: [
                { field: 'girisTarihi', headerName: 'Tarih', width: 110, valueFormatter: function (p) { return p.value ? Sgb.tarih(p.value) : ''; } },
                { field: 'girisAciklama', headerName: 'Açıklama', flex: 1, minWidth: 200,
                    valueFormatter: function (p) { var s = p.value || ''; return s.length > 120 ? s.substring(0, 120) + '...' : s; } }
            ],
            rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
            onSelectionChanged: function () {
                var s = grd.getSelectedRows()[0];
                if (s) yaz(s);
            }
        });

        function yaz(s) {
            s = s || {};
            $('#hdnKod').val(s.kod || '');
            $('#txtTarih').val(s.girisTarihi || '');
            $('#txtAciklama').val(s.girisAciklama || '');
            $('#btnSil').prop('disabled', !s.kod);
        }

        function listele(secKod) {
            Sgb.mt.oku(UC + 'bakim/liste', { taahhutKod: taahhutKod }, grd).done(function (v) {
                grd.setGridOption('rowData', v.satirlar || []);
                if (secKod) grd.forEachNode(function (n) { if (n.data.kod === secKod) n.setSelected(true); });
            });
        }

        $('#btnYeni').on('click', function () { grd.deselectAll(); yaz(null); $('#txtTarih').val(Sgb.bugun ? Sgb.bugun() : ''); });

        $('#btnKaydet').on('click', function () {
            if (!deger('#txtTarih')) { uyar('Tarih giriniz.'); return; }
            var v = { kod: parseInt($('#hdnKod').val(), 10) || 0, taahhutKod: taahhutKod, girisTarihi: deger('#txtTarih'), girisAciklama: $('#txtAciklama').val() || '' };
            Sgb.mt.islem(UC + 'bakim/kaydet', v).done(function (s) { listele(s.kod || v.kod); });
        });

        $('#btnSil').on('click', function () {
            var k = parseInt($('#hdnKod').val(), 10) || 0;
            if (!k) { uyar('Lütfen silmek istediğiniz bilgiyi seçin.'); return; }
            Sgb.mt.islem(UC + 'bakim/sil', { kod: k }, 'Kayıt silinecektir. Onaylıyor musunuz?').done(function () { yaz(null); listele(); });
        });

        if (!taahhutKod) { Sgb.bildirim('Taahhüt kartı bilgisi alınamadı.', 'danger'); return; }
        yaz(null);
        listele();
    }

    // ------------------------------------------------------------------ Sözleşme Koşulları / Garanti / Teminat Şartları (eski TanimSozlesme.aspx)

    function sozlesme() {
        var taahhutKod = sayiQ('taahhutKod');
        var tur = sayiQ('tur');
        var metinKod = 0;
        var $m = $('#txtMetin');

        $kok.on('click', '[data-komut]', function () {
            $m.trigger('focus');
            document.execCommand($(this).data('komut'), false, null);
        });

        $('#btnKaydet').on('click', function () {
            var html = $m.html();
            if (!$.trim($m.text())) { uyar('Metin giriniz.'); return; }
            Sgb.mt.islem(UC + 'sozlesme-metni/kaydet', { kod: metinKod, taahhutKod: taahhutKod, tur: tur, aciklama: html }).done(function (v) {
                if (v.kod) metinKod = v.kod;
            });
        });

        if (!taahhutKod) { Sgb.bildirim('Taahhüt kartı bilgisi alınamadı.', 'danger'); return; }
        Sgb.mt.oku(UC + 'sozlesme-metni', { taahhutKod: taahhutKod, tur: tur }).done(function (v) {
            metinKod = v.kod || 0;
            // Eski HtmlEditor'ün kaydettiği HTML; betik ve olay öznitelikleri temizlenerek gösterilir.
            var $t = $('<div></div>').html(v.aciklama || '');
            $t.find('script,style,iframe,object,embed').remove();
            $t.find('*').each(function () {
                var el = this;
                $.each($.makeArray(el.attributes), function (i, a) {
                    if (/^on/i.test(a.name) || /^\s*javascript:/i.test(a.value)) el.removeAttribute(a.name);
                });
            });
            $m.html($t.html());
        });
    }

    // ------------------------------------------------------------------ Belge Ekle (eski TanimBelgeEkle.aspx)

    function belge() {
        var ilgiKod = $.trim(q.get('ilgiKod') || '');
        var tip = sayiQ('tip');
        var grd = Sgb.grid('#grdBelge', {
            columnDefs: [{ field: 'BELGEADI', headerName: 'Belge Adı', flex: 1 }],
            rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
            onSelectionChanged: function () { $('#btnGoster, #btnSil').prop('disabled', !grd.getSelectedRows().length); },
            onRowDoubleClicked: function (e) { goster(e.data); }
        });

        function listele() {
            Sgb.mt.oku(UC + 'belge-ekle/liste', { ilgiKod: ilgiKod, tip: tip }, grd).done(function (v) {
                grd.setGridOption('rowData', v.satirlar || []);
                $('#btnGoster, #btnSil').prop('disabled', true);
            });
        }

        function goster(s) {
            if (!s) { uyar('Belge seçiniz.'); return; }
            Sgb.indir(UC + 'belge-ekle/indir', { kod: s.KOD });
        }

        $('#btnGoster').on('click', function () { goster(grd.getSelectedRows()[0]); });

        $('#btnSil').on('click', function () {
            var s = grd.getSelectedRows()[0];
            if (!s) { uyar('Belge seçiniz.'); return; }
            Sgb.mt.islem(UC + 'belge-ekle/sil', { kod: s.KOD }, s.BELGEADI + ' silinecektir. Onaylıyor musunuz?').done(listele);
        });

        $('#dosyaSec').on('change', function () {
            var dosyalar = this.files;
            if (!dosyalar || !dosyalar.length) return;
            if (!ilgiKod) { uyar('Önce taahhüt kartını kaydedin.'); this.value = ''; return; }
            var veri = new FormData();
            veri.append('ilgiKod', ilgiKod);
            veri.append('tip', String(tip));
            $.each(dosyalar, function (i, d) { veri.append('dosya', d); });
            var $in = $(this);
            $.ajax({
                url: Sgb.KOK + UC + 'belge-ekle/yukle', method: 'POST', data: veri, processData: false, contentType: false,
                dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
            }).done(function (v) {
                if (!v || v.hata || v.basarili === false) Sgb.bildirim((v && v.hata) || 'Belgeler kaydedilemedi.', 'danger');
                else Sgb.bildirim(v.bilgi || 'Belgeler başarıyla kaydedildi', 'success');
                listele();
            }).fail(function (xhr) {
                if (xhr.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
                Sgb.bildirim(xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Belgeler kaydedilemedi.', 'danger');
            }).always(function () { $in.val(''); });
        });

        if (!ilgiKod) { Sgb.bildirim('Önce taahhüt kartını kaydedin.', 'warning'); return; }
        listele();
    }
});
