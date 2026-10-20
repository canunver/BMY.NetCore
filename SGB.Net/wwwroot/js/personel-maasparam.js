// Personel İşlemleri > Maaş Parametreleri sekmesi (InsanKaynaklari/PersonelMaasParam; eski PersonelMaasParam.aspx).
// Personel, adres satırındaki T.C. kimlik numarasıyla belirlenir (eski sayfa gibi).
//   Sendika     : sendika ve kesinti oranı (kurumda sendika tanımlı değilse bölüm gizlenir), "Sendika Bilgisi Kaydet".
//   Gelirler    : değişkenlerin değerleri; Değer sütunu girilir, "Değ. Değerleri Kaydet" bütün satırları kaydeder.
//   Matrahlar   : yıla göre listelenir; "Satır Ekle" süreçsiz (000000000) satır açar, "Kaydet" yalnızca süreçsiz satırları kaydeder.
//   Bordro Yazdır: eski TekRaporAl(20000, 1) raporu.
$(function () {
    'use strict';

    var $kok = $('#pbMaas');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var tcKimlikNo = String($kok.attr('data-tc-kimlik-no') || '');
    var BOS_SUREC = '000000000';
    var matrahTurleri = [];
    var sayi = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function dugmeIslem($d, istek) {
        $d.prop('disabled', true);
        return istek.always(function () { $d.prop('disabled', false); });
    }
    function surecsiz(d) { return !d || !d.surecNo || d.surecNo === BOS_SUREC; }

    // ---------------------------------------------------------------- tablolar

    var grdGelirler = Sgb.grid('#grdGelirler', {
        columnDefs: [
            { field: 'aciklama', headerName: 'Açıklama', flex: 1, minWidth: 160, tooltipField: 'aciklama', cellClass: 'hucre-salt' },
            { field: 'degerGenel', headerName: 'Değer', width: 150, editable: true, cellDataType: 'text', type: 'rightAligned' }
        ],
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        overlayNoRowsTemplate: '<span class="text-muted">Kayıt yok</span>'
    });

    var grdMatrahlar = Sgb.grid('#grdMatrahlar', {
        columnDefs: [
            { field: 'surecNo', headerName: 'Süreç No', width: 120, cellClass: 'hucre-salt' },
            {
                field: 'matrahTurAd', headerName: 'Matrah Türü', flex: 1, minWidth: 200, cellDataType: 'text', editable: function (p) { return surecsiz(p.data); },
                cellClass: function (p) { return surecsiz(p.data) ? '' : 'hucre-salt'; },
                cellEditor: 'agSelectCellEditor', cellEditorParams: function () { return { values: matrahTurleri }; }
            },
            {
                field: 'tarihDT', headerName: 'Tarih', width: 130, cellDataType: 'dateString', editable: function (p) { return surecsiz(p.data); },
                cellClass: function (p) { return surecsiz(p.data) ? '' : 'hucre-salt'; },
                cellEditor: 'agDateStringCellEditor',
                valueFormatter: function (p) { return p.value ? Sgb.tarih(p.value) : ''; }
            },
            {
                field: 'deger', headerName: 'Değer', width: 150, type: 'rightAligned', cellDataType: 'number', editable: function (p) { return surecsiz(p.data); },
                cellClass: function (p) { return surecsiz(p.data) ? 'ag-right-aligned-cell' : ['hucre-salt', 'ag-right-aligned-cell']; },
                cellEditor: 'agNumberCellEditor', cellEditorParams: { precision: 2 },
                valueFormatter: function (p) { return p.value === null || p.value === undefined || p.value === '' ? '' : sayi.format(p.value); }
            }
        ],
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        overlayNoRowsTemplate: '<span class="text-muted">Kayıt yok</span>'
    });

    function satirlar(grid) {
        grid.stopEditing();
        var r = [];
        grid.forEachNode(function (n) { r.push(n.data); });
        return r;
    }

    // ---------------------------------------------------------------- okuma (eski Page_Load, SendikaBilgiOku, Listele)

    function tanimOku() {
        return Sgb.al('api/personelbilgi/maasparam/tanim', { tcKimlikNo: tcKimlikNo }).done(function (v) {
            matrahTurleri = v.matrahTurleri || [];
            $('#txtYil').val(v.yil);
            $('#bolumSendika').prop('hidden', !v.sendikaVar);
            var $s = $('#cmbSendikalar').empty();
            $.each(v.sendikalar || [], function (i, o) { $('<option></option>').val(String(o.kod)).text(o.ad).appendTo($s); });
            $s.val(String(v.sendikaNo || 0));
            if ($s.val() === null) $s.val('0');
            $('#tfSenKesOran').val(v.kesintiOrani || '');
            if (v.hata) mesaj(v.hata, 'danger');
        }).fail(hataGoster);
    }

    function listele(ilk) {
        if (!ilk) { grdGelirler.setGridOption('loading', true); grdMatrahlar.setGridOption('loading', true); }
        return Sgb.al('api/personelbilgi/maasparam/liste', { tcKimlikNo: tcKimlikNo, yil: parseInt($('#txtYil').val(), 10) || 0 })
            .done(function (v) {
                grdGelirler.setGridOption('rowData', v.paramlar || []);
                grdMatrahlar.setGridOption('rowData', v.matrahlar || []);
                if (v.hata) mesaj(v.hata, 'danger');
            })
            .fail(hataGoster)
            .always(function () { grdGelirler.setGridOption('loading', false); grdMatrahlar.setGridOption('loading', false); });
    }

    // ---------------------------------------------------------------- düğmeler

    $('#btnSendikaKaydet').on('click', function () {
        dugmeIslem($(this), Sgb.gonder('api/personelbilgi/maasparam/sendika-kaydet', {
            tcKimlikNo: tcKimlikNo,
            sendikaNo: parseInt($('#cmbSendikalar').val(), 10) || 0,
            kesintiOrani: $.trim($('#tfSenKesOran').val())
        }).done(function (s) {
            if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
            mesaj(s.bilgi, 'success');
        }).fail(hataGoster));
    });

    $('#btnKaydetGelirler').on('click', function () {
        var r = $.map(satirlar(grdGelirler), function (d) {
            return { degiskenAd: d.degiskenAd, aciklama: d.aciklama, degerGenel: d.degerGenel === null || d.degerGenel === undefined ? '' : String(d.degerGenel) };
        });
        dugmeIslem($(this), Sgb.gonder('api/personelbilgi/maasparam/gelir-kaydet', { tcKimlikNo: tcKimlikNo, satirlar: r })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
            }).fail(hataGoster));
    });

    $('#btnListele').on('click', function () { listele(false); });
    $('#txtYil').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); listele(false); } });

    // Eski grdMatrahlar.addRecord({ surecNo: '000000000' })
    $('#btnSatirAc').on('click', function () {
        var sonuc = grdMatrahlar.applyTransaction({ add: [{ surecNo: BOS_SUREC, matrahTurAd: '', tarihDT: '', deger: null }] });
        var n = sonuc && sonuc.add && sonuc.add[0];
        if (n) {
            grdMatrahlar.ensureIndexVisible(n.rowIndex);
            grdMatrahlar.startEditingCell({ rowIndex: n.rowIndex, colKey: 'matrahTurAd' });
        }
    });

    $('#btnKaydetMatrah').on('click', function () {
        var r = $.map(satirlar(grdMatrahlar), function (d) {
            return { surecNo: d.surecNo || '', matrahTurAd: d.matrahTurAd || '', tarihDT: d.tarihDT || '', deger: parseFloat(d.deger) || 0 };
        });
        dugmeIslem($(this), Sgb.gonder('api/personelbilgi/maasparam/matrah-kaydet', { tcKimlikNo: tcKimlikNo, yil: parseInt($('#txtYil').val(), 10) || 0, satirlar: r })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
            }).fail(hataGoster));
    });

    $('#btnYazdir').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/personelbilgi/maasparam/bordro', { tcKimlikNo: tcKimlikNo }).then(function () { $d.prop('disabled', false); });
    });

    tanimOku().always(function () { listele(true); });
});
