// Vergi dairesi aktarım sayfaları, ButceMuhasebe/DamgaVergisiAktarimi, KDVTevkifatiAktarimi, KDVAktarimi, MuhtasarAktarimi
// (eski aynı adlı dört Ext.NET sayfası tek ekran oldu). Yıl / Ay / Yevmiyeli ile listelenir; tablodaki satırlarla beyanname XML'i
// oluşturulur, Excel'e yazdırılır; XML sürümü saklanır. Tür sayfadaki data-tur'dan gelir (1 Damga, 2 KDV Tevkifatı, 3 KDV, 4 Muhtasar).
// Uçlar: /api/vergiaktarim/* (Muhasebe/VergiAktarimUclari.cs).
$(function () {
    'use strict';

    var $sayfa = $('.vergiaktarim');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/vergiaktarim/';
    var TUR = parseInt($sayfa.data('tur'), 10) || 0;
    var DAMGA = 1, TEVKIFAT = 2, KDV = 3, MUHTASAR = 4;
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function para(x) { return x.value === null || x.value === undefined || x.value === '' ? '' : tutar2.format(Number(x.value) || 0); }
    function tarih(x) { return x.value ? Sgb.tarih(String(x.value)) : ''; }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });

    // ================================================================ tablo: sütunlar eski sayfalardaki gibi türe göre

    // Girilebilir hücreler beyaz, salt okunurlar sarı.
    function sutun(alan, baslik, genislik, ek) {
        var s = $.extend({ field: alan, headerName: baslik, width: genislik, cellClass: 'va-sari', tooltipField: alan }, ek || {});
        if (s.type === 'rightAligned') s.cellClass = (s.editable ? 'va-girilir' : 'va-sari') + ' ag-right-aligned-cell';
        else if (s.editable) s.cellClass = 'va-girilir';
        return s;
    }
    function girilir(alan, baslik, genislik) { return sutun(alan, baslik, genislik, { editable: true }); }
    function tutarSutun(alan, baslik, genislik) { return sutun(alan, baslik, genislik, { type: 'rightAligned', valueFormatter: para, tooltipField: undefined }); }

    var sira = {
        colId: 'sira', headerName: '...', width: 56, sortable: false, cellClass: 'va-sari',
        valueGetter: function (p) { return p.node.rowIndex + 1; }
    };
    var ortakBas = [
        sira,
        sutun('ISLEMTARIH', 'Belge Tarihi', 95, { valueFormatter: tarih, tooltipField: undefined }),
        sutun('BELGENO', 'Belge No', 100),
        sutun('TURKOD', 'Türü', 80)
    ];

    var sutunlar;
    if (TUR === DAMGA) {
        sutunlar = ortakBas.concat([
            girilir('ALTHESAPKOD', 'Açıklama', 260),
            tutarSutun('ALACAK', 'Tutar', 110),
            tutarSutun('ICERDIGIPARA', 'İçerdiği Para', 130)
        ]);
    } else if (TUR === MUHTASAR) {
        sutunlar = ortakBas.concat([
            sutun('HESAPKOD', 'Hesap Kodu', 110),
            sutun('ODEMETUR', 'Ödeme Tür', 90),
            sutun('ODEMETARIH', 'Ödeme Tarih', 100),
            sutun('ODEMENO', 'Ödeme No', 100),
            sutun('ADRES', 'Adres', 180),
            girilir('SOYADI', 'Soyadı', 130),
            girilir('ADI', 'Adı', 160),
            girilir('TCNO', 'Kimlik No', 120),
            tutarSutun('MATRAH', 'Matrah', 120),
            tutarSutun('TUTAR', 'Tutarı', 110)
        ]);
    } else {
        sutunlar = ortakBas.concat([
            girilir('ALTHESAPKOD', 'Açıklama', 200),
            girilir('SOYADI', 'Soyadı', 130),
            girilir('ADI', 'Adı', 160),
            girilir('TCNO', 'Kimlik No', 120),
            girilir('VNO', 'Vergi No', 120),
            sutun('KDVORAN', 'KDV %', 70, { type: 'rightAligned' }),
            tutarSutun('MATRAH', 'Matrah', 120),
            tutarSutun('KDVTUTAR', 'KDV Tutarı', 110)
        ]);
        if (TUR === TEVKIFAT) sutunlar.push(tutarSutun('ALACAK', 'Tevkifat Tutarı', 120));
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: sutunlar,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        overlayNoRowsTemplate: '<span>Yıl ve ayı seçip Listele\'ye basınız.</span>'
    });

    function kriter() {
        return {
            tur: TUR,
            yil: parseInt($('#txtYil').val(), 10) || 0,
            ay: parseInt($('#ddlAy').val(), 10) || 0,
            yevmiyeli: $('#chkYevmiye').is(':checked')
        };
    }

    function satirlar() {
        grid.stopEditing();
        var s = [];
        grid.forEachNode(function (n) { if (n.data) s.push(n.data); });
        return s;
    }

    function ozetYaz(rows) {
        if (!rows.length) { $('#lblOzet').text(''); return; }
        var alan = TUR === MUHTASAR ? 'TUTAR' : (TUR === KDV ? 'KDVTUTAR' : 'ALACAK');
        var t = 0;
        $.each(rows, function (i, r) { t += Number(r[alan]) || 0; });
        $('#lblOzet').text(rows.length + ' satır, toplam ' + tutar2.format(t));
    }

    // Eski btnListe_Click
    function listele() {
        var k = kriter();
        if (!k.yil) { mesaj('Yılı giriniz.', 'warning'); return; }
        var $d = $('#btnListe').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', { tur: k.tur, yil: k.yil, ay: k.ay, yevmiyeli: k.yevmiyeli ? 1 : 0 })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var rows = v.satirlar || [];
                grid.setGridOption('rowData', rows);
                grid.setGridOption('overlayNoRowsTemplate', '<span>Bu ay için aktarılacak kayıt bulunamadı.</span>');
                ozetYaz(rows);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); $d.prop('disabled', false); });
    }

    $('#btnListe').on('click', listele);
    $('#frmKriter').on('keydown', 'input', function (e) { if (e.key === 'Enter' && this.id !== 'txtSurum') { e.preventDefault(); listele(); } });

    // POST ile dosya indirme (tablodaki satırlar gövdede gider)
    function postIndir(adres, veri, $d) {
        if ($d) $d.prop('disabled', true);
        return fetch(Sgb.KOK + adres, {
            method: 'POST', credentials: 'same-origin', cache: 'no-store',
            headers: { 'Content-Type': 'application/json', RequestVerificationToken: Sgb.meta('istek-jetonu') },
            body: JSON.stringify(veri)
        }).then(function (y) {
            if (y.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            var tur = y.headers.get('Content-Type') || '';
            if (!y.ok || tur.indexOf('json') >= 0 || tur.indexOf('text/html') >= 0) {
                if (tur.indexOf('json') >= 0) return y.json().then(function (v) { mesaj(v.hata || 'Dosya hazırlanamadı.', 'warning'); });
                mesaj(y.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Dosya hazırlanamadı.', 'danger');
                return;
            }
            var baslik = y.headers.get('Content-Disposition') || '';
            var ad = /filename\*=UTF-8''([^;]+)/i.exec(baslik) || /filename="?([^";]+)"?/i.exec(baslik);
            return y.blob().then(function (icerik) {
                var yerel = URL.createObjectURL(icerik);
                var a = document.createElement('a');
                a.href = yerel;
                a.download = ad ? decodeURIComponent(ad[1]) : 'dosya';
                document.body.appendChild(a);
                a.click();
                a.remove();
                setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
            });
        }).catch(function () { mesaj('Dosya hazırlanamadı.', 'danger'); })
            .then(function () { if ($d) $d.prop('disabled', false); });
    }

    // Eski btnKaydet_Click: beyanname XML'i
    $('#btnXml').on('click', function () {
        var rows = satirlar();
        if (!rows.length) { mesaj('Önce listeleyiniz; tabloda satır yok.', 'warning'); return; }
        if (!window.confirm('Beyanname XML dosyası oluşturulacak. Onaylıyor musunuz?')) return;
        var veri = $.extend(kriter(), { surum: $.trim($('#txtSurum').val()), satirlar: rows });
        postIndir(UC + 'xml', veri, $(this));
    });

    // Eski btnListeYazdir_Click (Damga Vergisi listesi sunucuda defterden yeniden okunur)
    $('#btnYazdir').on('click', function () {
        var rows = TUR === DAMGA ? [] : satirlar();
        if (TUR !== DAMGA && !rows.length) { mesaj('Önce listeleyiniz; tabloda satır yok.', 'warning'); return; }
        postIndir(UC + 'yazdir', $.extend(kriter(), { satirlar: rows }), $(this));
    });

    // Eski btnXMLKaydet_Click
    $('#btnSurumSakla').on('click', function () {
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'surum', { tur: TUR, surum: $.trim($('#txtSurum').val()) })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi || 'Sürüm saklandı.', 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ================================================================ mükellef bilgileri (kurum ayarı, dört sayfada ortak)

    var dlgMukellef = document.getElementById('dlgMukellef');
    $('dialog').on('click', '[data-kapat]', function (e) { e.stopPropagation(); this.closest('dialog').close(); });

    function mukellefYaz(m) {
        m = m || {};
        $('#frmMukellef [data-alan]').each(function () { $(this).val(m[$(this).data('alan')] || ''); });
        var unvan = $.trim((m.soyadi || '') + ' ' + (m.adi || ''));
        $('#lblMukellef').text(m.vergiNo ? m.vergiNo + (unvan ? ' - ' + unvan : '') : 'Girilmemiş');
        $('#lblMukellef').toggleClass('va-eksik', !m.vergiNo || !m.vdKodu);
    }

    $('#btnMukellefAc').on('click', function () { dlgMukellef.showModal(); $('#txtVergiNo').trigger('focus'); });

    $('#btnMukellefSakla').on('click', function () {
        var m = {};
        $('#frmMukellef [data-alan]').each(function () { m[$(this).data('alan')] = $.trim($(this).val()); });
        if (!m.vergiNo || !m.vdKodu) { mesaj('Vergi no ve vergi dairesi kodunu giriniz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'mukellef', { tur: TUR, mukellef: m })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                mukellefYaz(v.mukellef || m);
                mesaj(v.bilgi || 'Saklandı.', 'success');
                dlgMukellef.close();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ================================================================ açılış (eski Page_Load: bu ay, saklı sürüm)

    Sgb.al(UC + 'tanim', { tur: TUR })
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            Sgb.yilKutusu('#txtYil', t.yillar, t.yil);
            if (t.ay) $('#ddlAy').val(String(t.ay));
            $('#txtSurum').val(t.surum || '');
            mukellefYaz(t.mukellef);
        })
        .fail(hataGoster);

    // Tablo sayfanın kalan yüksekliğini doldurur.
    function yay() {
        var el = document.getElementById('grdListe');
        if (!el || el.offsetParent === null) return;
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(300, Math.floor(window.innerHeight - ust - 24)) + 'px';
    }
    $(window).on('resize', yay);
    yay();
});
