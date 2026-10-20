// Muhasebat aktarım belgeleri araması, ButceMuhasebe/MuhasebatAktarimSorguTCCB (eski MuhasebatAktarimSorguTCCB.aspx): Muhasebat Aktarım
// çerçevesinin "Belge Arama" sekmesi. Belgeler yıl, durum, muhasebe / birim, belge no ve belge tarihi aralığına göre listelenir.
// Belge numarası belgeyi Belge Düzenleme sekmesinde açar (eski BelgeGirisAc → window.parent.MaBelgeAc), durum tarihçeyi gösterir
// (eski TarihceGoster). Uçlar: /api/muhasebataktarim/liste, /tarihce (Muhasebe/MuhasebatAktarimUclari.cs).
$(function () {
    'use strict';

    if (!$('.ma-liste').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/muhasebataktarim/';
    var para2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var listeYil = 0; // listelenen yıl (belge açma ve tarihçe bu yılla, eski pgFiltre yılı)

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function tutar(p) { return p.value == null || p.value === '' ? '' : para2.format(Number(p.value) || 0); }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });
    $(document).on('click', 'dialog [data-kapat]', function (e) { e.stopPropagation(); this.closest('dialog').close(); });

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal' },
        yil: function () { return parseInt($('#txtYil').val(), 10) || 0; },
        kurumsalGizle: true,
        adGizle: true
    });

    function kriter() {
        var mb = muhBirim.deger();
        var k = {
            yil: parseInt($('#txtYil').val(), 10) || 0,
            durum: parseInt($('#ddlDurum').val(), 10) || 0,
            muhasebe: parseInt(mb.muhasebe, 10) || 0,
            birim: mb.birim || '',
            belgeNo1: $.trim($('#txtBelgeNo1').val()),
            belgeNo2: $.trim($('#txtBelgeNo2').val()),
            belgeTarihi1: $('#txtBelgeTarihi1').val() || '',
            belgeTarihi2: $('#txtBelgeTarihi2').val() || ''
        };
        if (!k.muhasebe) delete k.muhasebe; // boş sayı parametresi gönderilmez
        return k;
    }

    // eski BelgeGirisAc: çerçevede Belge Düzenleme sekmesine geçip belgeyi okur; çerçeve yoksa sayfa pencerede açılır.
    function belgeAc(belgeNo) {
        if (!belgeNo) return;
        if (window.parent && window.parent !== window && typeof window.parent.MaBelgeAc === 'function') {
            window.parent.MaBelgeAc(listeYil, belgeNo);
            return;
        }
        Sgb.pencere('../ButceMuhasebe/MuhasebatAktarimAna', 'Muhasebat Aktarım İşlemleri');
    }

    function baglanti(metin, baslik, is) {
        var a = document.createElement('a');
        a.href = '#';
        a.textContent = metin;
        a.title = baslik;
        a.addEventListener('click', function (e) { e.preventDefault(); e.stopPropagation(); is(); });
        return a;
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { headerName: '#', valueGetter: function (p) { return p.node.rowPinned ? '' : p.node.rowIndex + 1; }, width: 60, sortable: false },
            {
                field: 'belgeNo', headerName: 'Belge No', width: 120,
                cellRenderer: function (p) {
                    if (p.node.rowPinned) return p.value || '';
                    return p.value ? baglanti(p.value, 'Belgeyi aç', function () { belgeAc(p.value); }) : '';
                }
            },
            { field: 'belgeTarih', headerName: 'Belge Tarihi', width: 105, valueFormatter: function (p) { return p.value ? Sgb.tarih(p.value) : ''; } },
            { field: 'muhasebe', headerName: 'Muhasebe Birim Kodu', width: 120 },
            { field: 'birim', headerName: 'Birim Kodu', width: 100 },
            { field: 'borc', headerName: 'Borç', width: 130, type: 'rightAligned', valueFormatter: tutar },
            { field: 'alacak', headerName: 'Alacak', width: 130, type: 'rightAligned', valueFormatter: tutar },
            {
                field: 'durum', headerName: 'Durum', width: 130,
                cellRenderer: function (p) {
                    if (p.node.rowPinned) return '';
                    return p.data && p.value ? baglanti(p.value, 'Tarihçe', function () { tarihceGoster(p.data.belgeNo); }) : '';
                }
            },
            { field: 'aciklama', headerName: 'Açıklama', flex: 1, minWidth: 200, tooltipField: 'aciklama' },
            { field: 'islemYapan', headerName: 'En Son İşlem Yapan', width: 140 },
            { field: 'ettn', headerName: 'ETTN', width: 160, tooltipField: 'ettn' }
        ],
        getRowClass: function (p) { return p.node.rowPinned ? 'ma-toplam-satir' : ''; },
        pagination: true,
        paginationPageSize: 250,
        paginationPageSizeSelector: [50, 100, 250, 500, 1000, 2500, 5000],
        overlayNoRowsTemplate: '<span>Kriterleri girip Listele\'ye basınız.</span>'
    });

    // eski GroupingSummaryColumn (Sum): borç / alacak toplamı altta sabit satırda
    function toplamYaz(satirlar) {
        var b = 0, a = 0;
        $.each(satirlar || [], function (i, s) { b += Number(s.borc) || 0; a += Number(s.alacak) || 0; });
        grid.setGridOption('pinnedBottomRowData', satirlar && satirlar.length ? [{ belgeNo: 'Toplam', borc: Math.round(b * 100) / 100, alacak: Math.round(a * 100) / 100 }] : []);
    }

    // eski Liste_Click
    function listele() {
        var k = kriter();
        if (!k.yil || !k.durum) { mesaj('Sorgulama için yıl alanını boş bırakamazsınız.', 'warning'); return; }
        var $d = $('#btnListe').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', k)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata || 'Liste okunamadı.', 'danger'); return; }
                listeYil = v.yil || k.yil;
                grid.setGridOption('rowData', v.satirlar || []);
                toplamYaz(v.satirlar);
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun belge bulunamadı.</span>');
                $('#lblSayi').text(v.satirlar && v.satirlar.length ? v.satirlar.length + ' belge' : '');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); $d.prop('disabled', false); yay(); });
    }

    $('#btnListe').on('click', listele);
    $('#frmKriter').on('keydown', 'input', function (e) {
        if (e.key === 'Enter' && !$(this).closest('#muhBirim').length) { e.preventDefault(); listele(); }
    });

    // ---------------------------------------------------------------- tarihçe (eski TarihceGoster / BelgeTarihce.aspx?belgeTur=999)

    var gridTarihce = null;
    function tarihceGoster(belgeNo) {
        if (!gridTarihce) {
            gridTarihce = Sgb.grid('#grdTarihce', {
                columnDefs: [
                    { field: 'sira', headerName: '#', width: 60 },
                    { field: 'islemTarih', headerName: 'İşlem Tarihi', width: 160 },
                    { field: 'islem', headerName: 'İşlem', flex: 1, minWidth: 160, tooltipField: 'islem' },
                    { field: 'islemYapan', headerName: 'İşlem Yapan', width: 120 },
                    { field: 'islemYapanAd', headerName: 'Adı', width: 180 }
                ],
                overlayNoRowsTemplate: '<span>Tarihçe kaydı yok.</span>'
            });
        }
        $('#dlgTarihceBaslik').text('Tarihçe ' + listeYil + ' / ' + belgeNo);
        gridTarihce.setGridOption('rowData', []);
        document.getElementById('dlgTarihce').showModal();
        gridTarihce.setGridOption('loading', true);
        Sgb.al(UC + 'tarihce', { yil: listeYil, belgeNo: belgeNo })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata || 'Tarihçe okunamadı.', 'danger'); return; }
                gridTarihce.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { gridTarihce.setGridOption('loading', false); });
    }

    // ---------------------------------------------------------------- yerleşim ve açılış

    function yay() {
        var el = document.getElementById('grdListe');
        if (!el || el.offsetParent === null) return;
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(300, Math.floor(window.innerHeight - ust - 24)) + 'px';
    }
    $(window).on('resize', yay);

    // Açılış (eski Page_Load): yıl bu yıl, durum Yeni Belgeler. Liste boş başlar.
    Sgb.al(UC + 'tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            Sgb.yilKutusu('#txtYil', v.yillar, v.yil);
            var $d = $('#ddlDurum').empty();
            $.each(v.durumlar || [], function (i, t) { $('<option></option>').val(t.kod).text(t.ad).appendTo($d); });
            $d.val(String(v.durum));
        })
        .fail(hataGoster)
        .always(yay);
    yay();
});
