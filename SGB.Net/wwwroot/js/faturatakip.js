// Fatura Takip, ButceMuhasebe/FaturaTakipSorgu (eski aynı adlı Ext.NET sayfası). Ödeme emri / MİF belgeleri yıl, belge ve takip
// durumu, muhasebe birimi, yevmiye / belge numarası, belge tarihi, hesap kodu (varsayılan 320.01) ve alt hesaba göre listelenir.
// Satırdaki kalem takip durumunu değiştirir; belge numarası belgeyi, belge durumu belgenin tarihçesini açar; Liste Yazdır Excel verir.
// Uçlar: /api/faturatakip/* (Muhasebe/FaturaTakipUclari.cs). Birim seçimi Sgb.muhasebeBirim.
$(function () {
    'use strict';

    if (!$('.fatura-takip').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/faturatakip/';
    var para2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var listelendi = false;
    var listeYil = 0;      // listelenen yıl (takip durumu ve tarihçe bu yılla, eski pgFiltre yılı)
    var takipSatir = null; // takip durumu değiştirilen satır

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });
    $(document).on('click', 'dialog [data-kapat]', function (e) { e.stopPropagation(); this.closest('dialog').close(); });

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal' },
        yil: function () { return parseInt($('#txtYil').val(), 10) || 0; },
        kurumsalGizle: true
    });

    function kriter() {
        var mb = muhBirim.deger();
        return {
            yil: parseInt($('#txtYil').val(), 10) || 0,
            durum: parseInt($('#ddlDurum').val(), 10) || 0,
            takipDurum: parseInt($('#ddlTakipDurum').val(), 10) || 0,
            muhasebe: parseInt(mb.muhasebe, 10) || 0,
            birim: mb.birim || '',
            yevmiyeNo1: $.trim($('#txtYevmiyeNo1').val()),
            yevmiyeNo2: $.trim($('#txtYevmiyeNo2').val()),
            belgeNo1: $.trim($('#txtBelgeNo1').val()),
            belgeNo2: $.trim($('#txtBelgeNo2').val()),
            belgeTarihi1: $('#txtBelgeTarihi1').val() || '',
            belgeTarihi2: $('#txtBelgeTarihi2').val() || '',
            hesapNo: $.trim($('#txtHesapNo').val()),
            althesapNo: $.trim($('#txtAltHesapNo').val())
        };
    }

    // Eski MUHGNL184: yıl ya da belge durumu boşsa sorgulanmaz.
    function kriterDenetle(k) {
        if (!k.yil || !k.durum) { mesaj('Sorgulama için yıl alanını boş bırakamazsınız.', 'warning'); return false; }
        return true;
    }

    // ---------------------------------------------------------------- hesap / alt hesap önerileri (eski HesapStore / AltHesapStore)

    function oneri($girdi, $liste, adres, ekParametre) {
        var zaman = null;
        var son = '';
        $girdi.on('input', function () {
            var metin = $.trim($girdi.val());
            clearTimeout(zaman);
            if (metin.length < 2 || metin === son || metin.indexOf(' - ') > 0) return;
            zaman = setTimeout(function () {
                son = metin;
                Sgb.al(UC + adres, $.extend({ metin: metin }, ekParametre ? ekParametre() : {}))
                    .done(function (v) {
                        if (v.hata) return;
                        $liste.empty();
                        $.each(v.satirlar || [], function (i, s) { $('<option></option>').val(s.KOD).appendTo($liste); });
                    });
            }, 300);
        });
    }
    oneri($('#txtHesapNo'), $('#lstHesap'), 'hesap/ara', function () { return { yil: parseInt($('#txtYil').val(), 10) || 0 }; });
    oneri($('#txtAltHesapNo'), $('#lstAltHesap'), 'althesap/ara');

    // ---------------------------------------------------------------- liste

    function odemeEmriAc(yil, belgeNo) {
        if (!belgeNo) return;
        var ust = Sgb;
        try {
            if (window.top !== window && window.top.Sgb && window.top.Sgb.pencere) ust = window.top.Sgb;
        } catch (e) { /* başka kökenden açılmış */ }
        ust.pencere('../ButceMuhasebe/OdemeEmriMIFFrame?' + $.param({ aramaYok: 1, yil: yil, belgeNo: belgeNo }), 'Ödeme Emri ' + yil + '/' + belgeNo);
    }

    function baglanti(metin, baslik, is) {
        var a = document.createElement('a');
        a.href = '#';
        a.textContent = metin;
        a.title = baslik;
        a.addEventListener('click', function (e) { e.preventDefault(); e.stopPropagation(); is(); });
        return a;
    }

    function takipHucre(p) {
        if (!p.data) return '';
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-link btn-xs ft-komut';
        b.title = 'Takip durumunu değiştir';
        b.innerHTML = '<span class="glyphicon glyphicon-pencil" aria-hidden="true"></span><span class="sr-only">Takip durumunu değiştir</span>';
        b.addEventListener('click', function (e) { e.stopPropagation(); takipPenceresi(p.node); });
        return b;
    }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { headerName: '#', valueGetter: 'node.rowIndex + 1', width: 60, sortable: false },
            { headerName: '', width: 40, sortable: false, resizable: false, cellRenderer: takipHucre },
            {
                field: 'belgeNo', headerName: 'Belge No', width: 120,
                cellRenderer: function (p) { return p.data && p.value ? baglanti(p.value, 'Belgeyi aç', function () { odemeEmriAc(listeYil, p.value); }) : ''; }
            },
            { field: 'belgeTarih', headerName: 'Belge Tarihi', width: 100, valueFormatter: function (p) { return p.value ? Sgb.tarih(p.value) : ''; } },
            { field: 'muhasebe', headerName: 'Muhasebe', width: 170, tooltipField: 'muhasebe' },
            { field: 'birim', headerName: 'Birim', width: 170, tooltipField: 'birim' },
            {
                field: 'tutar', headerName: 'Tutar', width: 120, type: 'rightAligned',
                valueFormatter: function (p) { return p.value == null ? '' : para2.format(p.value); }
            },
            { field: 'aciklama', headerName: 'Açıklama', flex: 1, minWidth: 200, tooltipField: 'aciklama' },
            { field: 'ilgili', headerName: 'İlgili Adı', width: 180, tooltipField: 'ilgili' },
            {
                field: 'durumAd', headerName: 'Belge Durum', width: 100,
                cellRenderer: function (p) { return p.data && p.value ? baglanti(p.value, 'Tarihçe', function () { tarihceGoster(p.data.belgeNo); }) : ''; }
            },
            { field: 'takipDurumAd', headerName: 'Takip Durum', width: 150 },
            { field: 'ekliDosya', headerName: 'Ekli Dosya', width: 80 }
        ],
        getRowId: function (p) { return String(p.data.belgeNo); },
        pagination: true,
        paginationPageSize: 250,
        paginationPageSizeSelector: [50, 100, 250, 500, 1000, 2500, 5000],
        overlayNoRowsTemplate: '<span>Kriterleri girip Listele\'ye basınız.</span>'
    });

    function listele() {
        var k = kriter();
        if (!kriterDenetle(k)) return;
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', k)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                listelendi = true;
                listeYil = k.yil;
                var satirlar = v.satirlar || [];
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
                grid.setGridOption('rowData', satirlar);
                $('#lblSayi').text(satirlar.length + ' belge');
                if (!satirlar.length) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#btnListele').on('click', listele);
    $('#frmKriter').on('keydown', 'input', function (e) {
        if (e.key === 'Enter' && !$(this).closest('#muhBirim').length && !$(this).attr('list')) { e.preventDefault(); listele(); }
    });

    $('#btnYazdir').on('click', function () {
        var k = kriter();
        if (!kriterDenetle(k)) return;
        var $t = $(this).prop('disabled', true);
        Sgb.indir(UC + 'yazdir', k).then(function () { $t.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- takip durumu (eski wndTakipDurumDegistir)

    function takipPenceresi(node) {
        takipSatir = node;
        $('#lblTakipBelge').text(node.data.belgeNo);
        $('#ddlTakipDegistir').val(String(node.data.takipDurum));
        document.getElementById('dlgTakip').showModal();
    }

    $('#btnTakipKaydet').on('click', function () {
        if (!takipSatir) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'takip-durum', {
            Yil: listeYil, BelgeNo: takipSatir.data.belgeNo, TakipDurum: parseInt($('#ddlTakipDegistir').val(), 10) || 0
        })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata || 'Takip durumu kaydedilemedi.', 'warning'); return; }
                if (s.bilgi) mesaj(s.bilgi, 'success');
                takipSatir.setDataValue('takipDurum', s.takipDurum);
                takipSatir.setDataValue('takipDurumAd', s.takipDurumAd);
                document.getElementById('dlgTakip').close();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- tarihçe (eski TarihceGoster / BelgeTarihce.aspx)

    var gridTarihce = null;
    function tarihceGoster(belgeNo) {
        if (!gridTarihce) {
            gridTarihce = Sgb.grid('#grdTarihce', {
                columnDefs: [
                    { field: 'sira', headerName: '#', width: 60 },
                    { field: 'islemTarih', headerName: 'İşlem Tarihi', width: 160 },
                    { field: 'islem', headerName: 'İşlem', flex: 1, minWidth: 160 },
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
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
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

    // Açılış (eski Page_Load): yıl bu yıl, belge durum Yeni Belgeler, takip durum Hepsi, hesap kodu 320.01. Liste boş başlar.
    Sgb.al(UC + 'tanim')
        .done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            Sgb.yilKutusu('#txtYil', [v.yil - 15, v.yil + 1], v.yil);
            var $d = $('#ddlDurum').empty();
            $.each(v.belgeDurumlari || [], function (i, t) { $('<option></option>').val(t.kod).text(t.ad).appendTo($d); });
            $d.val(String(v.durum));
            var $t = $('#ddlTakipDurum').empty();
            var $td = $('#ddlTakipDegistir').empty();
            $.each(v.takipDurumlari || [], function (i, t) {
                $('<option></option>').val(t.kod).text(t.ad).appendTo($t);
                $('<option></option>').val(t.kod).text(t.ad).appendTo($td);
            });
            $('<option></option>').val(100).text('Hepsi').appendTo($t);
            $t.val(String(v.takipDurum));
            $('#txtHesapNo').val(v.hesapNo || '');
            yay();
        })
        .fail(hataGoster);
    yay();
});
