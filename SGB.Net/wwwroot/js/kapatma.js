// Mahsup İşlemleri, ButceMuhasebe/Kapatma (eski Kapatma.aspx + ModulScripts/Mahsup.js). Menüde üç seçenek: hesap kodsuz, ?hesap=323 ve
// ?hesap=103 (hesap verilince hesap kodu alanı gizlenir). Yevmiyeli / onaylı belgelerin kapatılmamış satırları listelenir; seçilenlerden
// kapatma (mahsup) belgesi oluşturulur, kapatma belgesi iptal edilir, satırlar listeden çıkartılır, liste Excel'e, banka listesi Word'e yazılır.
// Uçlar: /api/kapatma/* (Muhasebe/KapatmaUclari.cs). Birim seçimi Sgb.muhasebeBirim, hesap kodu Sgb.hesapKodu, kişi / firma Sgb.firmaSec.
// Belge penceresindeki satır ekleme (eski ListeyeEkle, TertipHazirla, KapatmaTutariBul) ve damga vergisi hesabı (eski
// HarcamaSurecleri/ListeKesintiHesapla.aspx) eski betiklerdeki gibi tarayıcıda yapılır.
$(function () {
    'use strict';

    var $sayfa = $('.kapatma');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/kapatma/';
    var HESAP = $.trim($sayfa.attr('data-hesap') || '');
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    var LISTEDEN_CIKARTILDI = 'Listeden çıkartıldı';

    var damgaHesapKod = '';
    var kapatilmisListe = false;

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function para(x) { return x.value === null || x.value === undefined || x.value === '' ? '' : tutar2.format(Number(x.value) || 0); }
    function tarih(x) { return x.value ? Sgb.tarih(x.value) : ''; }

    // Eski ParaFormat.js formatCurrency(num, true): 1.234,56
    function formatCurrency(num) {
        var eksi = false;
        num = Number(num);
        if (isNaN(num)) num = 0;
        if (num < 0) { num = Math.abs(num); eksi = true; }
        var kurus = Math.floor((num * 100 + 0.5) % 100);
        var s = Math.floor((num * 100 + 0.5) / 100).toString();
        if (kurus < 10) kurus = '0' + kurus;
        for (var i = 0; i < Math.floor((s.length - (1 + i)) / 3); i++)
            s = s.substring(0, s.length - (4 * i + 3)) + '.' + s.substring(s.length - (4 * i + 3));
        return (eksi ? '-' : '') + s + ',' + kurus;
    }

    // Eski ScriptParaCevir: "1.234,56" -> 1234.56 (sayı verilirse aynen)
    function paraCevir(p) {
        if (typeof p === 'number') return p;
        p = p === undefined || p === null ? '' : String(p);
        if (p === '') return 0;
        return Number(p.replace(/\./g, '').replace(/,/g, '.')) || 0;
    }

    $('[data-yardim]').on('click', function () { var $y = $($(this).data('yardim')); $y.prop('hidden', !$y.prop('hidden')); yay(); });

    // ================================================================ kıstaslar

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim' },
        yil: function () { return $('#txtYil').val(); }
    });

    var hesapNo = HESAP ? null : Sgb.hesapKodu('#txtHesapNo', { yil: '#txtYil', sonSeviye: true, uc: UC + 'hesap/' });

    $('#chkKapatilmis').on('change', function () {
        $('#grpKapatmaBelgeNo').prop('hidden', !this.checked);
        yay();
    });

    function kriter() {
        var mb = muhBirim.deger();
        return {
            yil: parseInt($('#txtYil').val(), 10) || 0,
            muhasebe: mb.muhasebe || '',
            birim: mb.birim || '',
            hesapKod: HESAP || $.trim($('#txtHesapNo').val()),
            belgeTarih1: $('#txtBelgeTarih1').val() || '',
            belgeTarih2: $('#txtBelgeTarih2').val() || '',
            yevTarih1: $('#txtYevmiyeTarih1').val() || '',
            yevTarih2: $('#txtYevmiyeTarih2').val() || '',
            yevNo1: $.trim($('#txtYevNo1').val()),
            yevNo2: $.trim($('#txtYevNo2').val()),
            aciklama: $.trim($('#txtArama').val()),
            yevmiyeli: $('#chkYevmiyeListele').is(':checked') ? 1 : 0,
            kapatilmis: $('#chkKapatilmis').is(':checked') ? 1 : 0,
            kapatmaBelgeNo: $('#chkKapatilmis').is(':checked') ? $.trim($('#txtKapatmaBelgeNo').val()) : ''
        };
    }

    // ================================================================ liste

    function belgeAc(yil, belgeNo) {
        Sgb.pencere('../ButceMuhasebe/OdemeEmriMIFFrame?' + $.param({ aramaYok: 1, yil: yil, belgeNo: belgeNo }), 'Ödeme Emri / MİF Belgesi ' + belgeNo);
    }

    function belgeBag(yilAlan, noAlan) {
        return function (p) {
            if (!p.data || p.node.rowPinned) return p.value || '';
            var no = p.data[noAlan];
            if (!no || no === LISTEDEN_CIKARTILDI) return no || '';
            var $a = $('<a href="#"></a>').text(no).on('click', function (e) { e.preventDefault(); belgeAc(p.data[yilAlan], no); });
            return $a[0];
        };
    }

    function sari(p) { return p.node && p.node.rowPinned ? 'kp-toplam' : 'kp-sari'; }
    // cellClass verilince 'rightAligned' türünün sınıfı ezilir; sağa dayama sınıfı ayrıca eklenir.
    function sariSag(p) { return sari(p) + ' ag-right-aligned-cell'; }

    // Eski sayfadaki gibi Borç, Alacak, Bakiye ve Kapatma sütun başlıklarının altında listenin toplamı kırmızı yazılır.
    var baslikToplam = null;
    function ToplamBaslik() { }
    ToplamBaslik.prototype.init = function (p) {
        this.p = p;
        this.e = $('<div class="kp-baslik"><span><span class="kp-baslik-ad"></span><span class="kp-baslik-sira"></span></span><span class="kp-baslik-toplam"></span></div>')[0];
        var self = this;
        $(this.e).on('click', function (ev) { if (p.enableSorting) p.progressSort(ev.shiftKey); });
        this.siraDegisti = function () { self.yaz(); };
        p.column.addEventListener('sortChanged', this.siraDegisti);
        this.yaz();
    };
    ToplamBaslik.prototype.yaz = function () {
        var p = this.p, $e = $(this.e), alan = p.column.getColDef().field;
        $e.find('.kp-baslik-ad').text(p.displayName);
        var s = p.column.getSort();
        $e.find('.kp-baslik-sira').text(s === 'asc' ? ' ▲' : (s === 'desc' ? ' ▼' : ''));
        $e.find('.kp-baslik-toplam').text(baslikToplam ? tutar2.format(Number(baslikToplam[alan]) || 0) : '');
    };
    ToplamBaslik.prototype.getGui = function () { return this.e; };
    ToplamBaslik.prototype.refresh = function (p) { this.p = p; this.yaz(); return true; };
    ToplamBaslik.prototype.destroy = function () { this.p.column.removeEventListener('sortChanged', this.siraDegisti); };

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { colId: 'sira', headerName: '', width: 56, sortable: false, cellClass: sari, valueGetter: function (p) { return p.node.rowPinned ? '' : p.node.rowIndex + 1; } },
            { field: 'belgeNo', headerName: 'Belge No', width: 110, cellClass: sari, cellRenderer: belgeBag('belgeYil', 'belgeNo') },
            { field: 'belgeTarih', headerName: 'Belge Tarihi', width: 100, cellClass: sari, valueFormatter: tarih },
            { field: 'yevmiyeNo', colId: 'yevmiyeNo', headerName: 'Yevmiye No', width: 90, cellClass: sari, valueFormatter: function (x) { return x.value ? String(x.value) : ''; } },
            { field: 'yevmiyeTarih', colId: 'yevmiyeTarih', headerName: 'Yevmiye Tarihi', width: 100, cellClass: sari, valueFormatter: tarih },
            { field: 'hesapKod', headerName: 'Hesap Kodu', width: 120, cellClass: sari },
            { field: 'borc', headerName: 'Borç', width: 110, headerComponent: ToplamBaslik, type: 'rightAligned', cellClass: sariSag, valueFormatter: para },
            { field: 'alacak', headerName: 'Alacak', width: 110, headerComponent: ToplamBaslik, type: 'rightAligned', cellClass: sariSag, valueFormatter: para },
            { field: 'tutar', headerName: 'Bakiye', width: 120, headerComponent: ToplamBaslik, type: 'rightAligned', cellClass: sariSag, valueFormatter: para },
            {
                field: 'kTutar', headerName: 'Kapatma Tutarı', width: 120, headerComponent: ToplamBaslik, type: 'rightAligned', valueFormatter: para,
                editable: function (p) { return !kapatilmisListe && !p.node.rowPinned; },
                cellEditor: 'agNumberCellEditor', cellEditorParams: { precision: 2 },
                cellClass: function (p) { return (p.node.rowPinned ? 'kp-toplam' : (kapatilmisListe ? 'kp-sari' : 'kp-girilir')) + ' ag-right-aligned-cell'; },
                valueParser: function (p) { var n = Number(p.newValue); return isNaN(n) ? p.oldValue : n; }
            },
            { field: 'paraBirimi', headerName: 'Para Birimi', width: 80, cellClass: sari },
            { field: 'muhasebeAd', colId: 'muhasebeAd', headerName: 'Muhasebe Kodu', width: 200, cellClass: sari },
            { field: 'birimAd', colId: 'birimAd', headerName: 'Birim Kodu', width: 120, cellClass: sari },
            { field: 'altHesapKod', headerName: 'Alt Hesap Kodu', width: 200, cellClass: sari },
            { field: 'tertip', colId: 'tertip', headerName: 'Tertip', width: 200, cellClass: sari },
            { field: 'ilgiliAd', headerName: 'İlgili Adı / Ünvanı', width: 200, cellClass: sari },
            { field: 'cekNo', colId: 'cekNo', headerName: 'Çek/Gönderme No', width: 110, cellClass: sari },
            { field: 'istatistik1Ad', headerName: 'İstatistik Kod 1', width: 130, cellClass: sari },
            { field: 'istatistik2Ad', headerName: 'İstatistik Kod 2', width: 130, cellClass: sari },
            { field: 'kapatmaBelgeNo', colId: 'kapatmaBelgeNo', headerName: 'Kapatma Belge No', width: 130, hide: true, cellClass: sari, cellRenderer: belgeBag('kapatmaBelgeYil', 'kapatmaBelgeNo') }
        ],
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        headerHeight: 42,
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        pagination: true,
        paginationPageSize: 750,
        paginationPageSizeSelector: [50, 100, 250, 500, 750, 1000, 3000],
        overlayNoRowsTemplate: '<span>Kıstasları girip Listele\'ye basınız.</span>',
        onSelectionChanged: satirSecimi,
        onCellValueChanged: satirSecimi
    });

    // Eski SatirSecimi: seçilenlerin toplamları
    function satirSecimi() {
        var s = grid.getSelectedRows();
        var t = { tutar: 0, kTutar: 0, borc: 0, alacak: 0 };
        $.each(s, function (i, r) {
            t.tutar += Number(r.tutar) || 0; t.kTutar += Number(r.kTutar) || 0;
            t.borc += Number(r.borc) || 0; t.alacak += Number(r.alacak) || 0;
        });
        $('#lblSecili').text(s.length + ' adet satır için;');
        $('#lblSeciliBorc').text(formatCurrency(t.borc));
        $('#lblSeciliAlacak').text(formatCurrency(t.alacak));
        $('#lblSeciliTutar').text(formatCurrency(t.tutar));
        $('#lblSeciliKapatma').text(formatCurrency(t.kTutar));
    }

    function seciliSatirlar() {
        if (grid.getEditingCells().length) grid.stopEditing();
        return grid.getSelectedRows();
    }

    function tumSatirlar() {
        if (grid.getEditingCells().length) grid.stopEditing();
        var r = [];
        grid.forEachNode(function (n) { if (n.data) r.push(n.data); });
        return r;
    }

    function listele() {
        var k = kriter();
        if (!k.yil) { mesaj('Yıl bilgisini giriniz.', 'warning'); return; }
        var $d = $('#btnListele').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', k)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                kapatilmisListe = !!v.kapatilmis;
                var g = v.gizle || {};
                grid.setColumnsVisible(['yevmiyeNo', 'yevmiyeTarih'], !g.yevmiye);
                grid.setColumnsVisible(['muhasebeAd'], !g.muhasebe);
                grid.setColumnsVisible(['birimAd'], !g.birim);
                grid.setColumnsVisible(['tertip'], !g.tertip);
                grid.setColumnsVisible(['cekNo'], !g.cek);
                grid.setColumnsVisible(['kapatmaBelgeNo'], kapatilmisListe);
                $('#btnBelge').prop('hidden', kapatilmisListe);
                $('#btnBelgeIptal').prop('hidden', !kapatilmisListe);

                grid.setGridOption('rowData', v.satirlar || []);
                var t = v.toplam || {};
                baslikToplam = v.satirlar && v.satirlar.length ? t : null;
                grid.refreshHeader();
                grid.setGridOption('overlayNoRowsTemplate', '<span>Aradığınız kriterlere uygun kayıt bulunamadı.</span>');
                $('#lblOzet').text(v.satirlar && v.satirlar.length ? v.satirlar.length + ' satır' : '');
                satirSecimi();
                if (v.bilgi) mesaj(v.bilgi, 'info');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); $d.prop('disabled', false); });
    }

    $('#btnListele').on('click', listele);
    $('#frmKriter').on('keydown', 'input', function (e) {
        if (e.key === 'Enter' && !$(this).closest('#muhBirim').length && !$(this).is('[data-hesap-kodu], #txtHesapNo')) { e.preventDefault(); listele(); }
    });

    // POST ile dosya indirme (seçili satırlar gövdede gider)
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
                a.download = ad ? decodeURIComponent(ad[1]) : 'rapor';
                document.body.appendChild(a);
                a.click();
                a.remove();
                setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
            });
        }).catch(function () { mesaj('Dosya hazırlanamadı.', 'danger'); })
            .then(function () { if ($d) $d.prop('disabled', false); });
    }

    function seciliYok(satirlar) {
        if (satirlar.length) return false;
        mesaj('Listeden herhangi bir belge seçilmediği için işlem yapılamadı.', 'warning');
        return true;
    }

    var ONAY = 'İşlem yapılacak. Onaylıyor musunuz?';

    $('#btnYazdirButun').on('click', function () {
        var s = tumSatirlar();
        if (seciliYok(s)) return;
        postIndir(UC + 'yazdir', { satirlar: s }, $(this));
    });

    $('#btnBankaListesi').on('click', function () {
        var s = seciliSatirlar();
        if (seciliYok(s)) return;
        postIndir(UC + 'banka-listesi', { satirlar: s }, $(this));
    });

    $('#btnBelgeIptal').on('click', function () {
        var s = seciliSatirlar();
        if (seciliYok(s)) return;
        if (!window.confirm(ONAY)) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'iptal', { satirlar: s })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi || 'Seçili kapatma belgeleri iptal edildi.', 'success');
                listele();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnListedenCikart').on('click', function () {
        var s = seciliSatirlar();
        if (seciliYok(s)) return;
        if (!window.confirm(ONAY)) return;
        var $d = $(this).prop('disabled', true);
        // Eski: kapanış kaydı belge oluşturma penceresindeki yılla (ddlYilBelge) yazılırdı.
        Sgb.gonder(UC + 'listeden-cikar', { yil: parseInt($('#txtYilBelge').val(), 10) || new Date().getFullYear(), satirlar: s })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                listele();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ================================================================ belge oluşturma penceresi (eski wndKapatma)

    var dlgBelge = document.getElementById('dlgBelge');
    var belge = { tersIslem: '1', tertip: '', kapatmaTutari: 0, paraBirimi: '', firmaNo: '' };

    $('dialog').on('click', '[data-kapat]', function (e) { e.stopPropagation(); this.closest('dialog').close(); });

    var muhBirimBelge = Sgb.muhasebeBirim('#muhBirimBelge', {
        kimlik: { muhasebe: 'txtMuhasebeBelge', birim: 'txtBirimBelge' },
        yil: function () { return $('#txtYilBelge').val(); },
        kurumsalGizle: true
    });

    var gridKapatma = Sgb.grid('#grdKapatma', {
        columnDefs: [
            { field: 'hesapKod', headerName: 'Hesap Kodu', width: 130, sortable: false },
            { field: 'birim', headerName: 'Birim Kodu', width: 110, sortable: false },
            { field: 'altHesapKod', headerName: 'Alt Hesap Kodu', width: 130, sortable: false },
            { field: 'tertip', headerName: 'Gider Bütçesi Tertibi', width: 220, sortable: false },
            { field: 'borc', headerName: 'Borç', width: 100, sortable: false, type: 'rightAligned', valueFormatter: para },
            { field: 'alacak', headerName: 'Alacak', width: 100, sortable: false, type: 'rightAligned', valueFormatter: para },
            { field: 'paraBirimi', headerName: 'Para Birimi', width: 80, sortable: false },
            {
                colId: 'sil', headerName: '', width: 44, sortable: false,
                cellRenderer: function (p) {
                    if (p.data.sunucudanGeldi === 1) return ''; // sunucudan gelen satırlar silinmez
                    return $('<button type="button" class="btn btn-link btn-xs kp-sil" title="Sil" aria-label="Sil"><span class="glyphicon glyphicon-trash" aria-hidden="true"></span></button>')
                        .on('click', function () { gridKapatma.applyTransaction({ remove: [p.data] }); })[0];
                }
            }
        ],
        overlayNoRowsTemplate: '<span>Satır yok.</span>'
    });

    function kapanisSatirlari() {
        var r = [];
        gridKapatma.forEachNode(function (n) { r.push(n.data); });
        return r;
    }

    // Eski KapatmaTutariBul
    function kapatmaTutariBul() {
        var t = 0;
        $.each(kapanisSatirlari(), function (i, r) { t += (Number(r.borc) || 0) - (Number(r.alacak) || 0); });
        return t;
    }

    // Eski TertipHazirla: belgenin tertibinin ekonomik kodu hesap kodundan alınır.
    function tertipHazirla(hesapKod) {
        var tDizi = (belge.tertip || '').split('-');
        hesapKod = String(hesapKod || '').substring(3).replace(/\./g, '');
        if (hesapKod.length > 8) hesapKod = hesapKod.substring(0, 8);
        if (hesapKod.length === 8)
            hesapKod = hesapKod.substring(0, 2) + '.' + hesapKod.substring(3, 4) + '.' + hesapKod.substring(5, 6) + '.' + hesapKod.substring(6, 8);
        if (tDizi.length > 3) tDizi[3] = hesapKod;
        return tDizi.join('-');
    }

    // Eski ListeyeEkle
    function listeyeEkle(hesapKod, tertip, borc, alacak, hesapKapatma) {
        borc = paraCevir(borc);
        alacak = paraCevir(alacak);
        if (hesapKapatma !== 1) {
            if (belge.tersIslem === '-1') {
                var tmp = Math.abs(borc);
                borc = Math.abs(alacak);
                alacak = tmp;
            }
        } else if (alacak > 0) { //Alacak 0'dan büyükse borç rakamı fazla gelmiştir, alacağa atılır
            alacak = Math.abs(alacak);
        } else if (alacak < 0) { //Alacak 0'dan küçükse alacak rakamı fazla gelmiştir, borca atılır
            var temp = borc;
            borc = Math.abs(alacak);
            alacak = Math.abs(temp);
        }

        if (tertip !== '--') tertip = tertipHazirla(hesapKod);

        gridKapatma.applyTransaction({
            add: [{ hesapKod: hesapKod, tertip: tertip, borc: borc, alacak: alacak, paraBirimi: belge.paraBirimi, birim: '', altHesapKod: '', belgeNo: '', siraNo: '', sunucudanGeldi: 0 }]
        });
    }

    function hesapGerekli(kod) {
        if ($.trim(kod)) return true;
        mesaj('Önce hesap kodunu giriniz.', 'warning');
        return false;
    }

    $('#btnDamgaEkle').on('click', function () { listeyeEkle(damgaHesapKod, '', 0, $.trim($('#txtDamga').val()), 0); });
    $('#btnKapatmaHesapEkle').on('click', function () {
        var k = $.trim($('#txtKapatmaHesapKod').val());
        if (hesapGerekli(k)) listeyeEkle(k, '--', 0, kapatmaTutariBul(), 1);
    });
    $('#btnButceHesapEkle').on('click', function () {
        var k = $.trim($('#txtButceHesapKod').val());
        if (hesapGerekli(k)) listeyeEkle(k, '', 0, kapatmaTutariBul(), 0);
    });

    $('#btnBelge').on('click', function () {
        var s = seciliSatirlar();
        if (seciliYok(s)) return;
        var mb = muhBirim.deger();
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'belge', { muhasebe: String(mb.muhasebe || ''), birim: mb.birim || '', satirlar: s })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'warning'); return; }
                belge = { tersIslem: v.tersIslem, tertip: v.tertip || '', kapatmaTutari: Number(v.kapatmaTutari) || 0, paraBirimi: v.paraBirimi || '', firmaNo: v.firmaNo || '' };
                $(dlgBelge).find('[data-gizle-103]').prop('hidden', !!v.hesap103);
                Sgb.yilKutusu('#txtYilBelge', yillar, $('#txtYilBelge').val() || v.yil);
                $('#txtTarihBelge').val(v.tarih || '');
                muhBirimBelge.ata({ muhasebe: v.muhasebe, birim: v.birim });
                if (v.muhasebe) muhBirimBelge.yenile();
                $('#txtDamga, #txtKapatmaHesapKod, #txtAciklama').val('');
                $('#txtButceHesapKod').val(v.butceHesapKod || '');
                $('#txtIlgiliAd').val(v.ilgiliAd || '');
                $('#chkYevmiye').prop('checked', false);
                gridKapatma.setGridOption('rowData', v.satirlar || []);
                dlgBelge.showModal();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnBelgeOlustur').on('click', function () {
        if (!window.confirm(ONAY)) return;
        var mb = muhBirimBelge.deger();
        var veri = {
            yil: parseInt($('#txtYilBelge').val(), 10) || new Date().getFullYear(),
            tarih: $('#txtTarihBelge').val() || '',
            muhasebe: String(mb.muhasebe || ''),
            birim: mb.birim || '',
            firmaNo: parseInt(belge.firmaNo, 10) || 0,
            aciklama: $('#txtAciklama').val(),
            yevmiye: $('#chkYevmiye').is(':checked'),
            kapanisSatirlari: kapanisSatirlari(),
            seciliBelgeler: seciliSatirlar()
        };
        var $d = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'olustur', veri)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                dlgBelge.close();
                sonucYaz(v.yil, v.belgeNo);
                mesaj('Ödeme Emri/MİF kaydı yapıldı. Belge Numarası : ' + v.belgeNo, 'success');
                if (v.kapanisHata) mesaj(v.kapanisHata, 'danger');
                listele();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // Oluşturulan belgenin bağlantısı araç çubuğunda kalır (eski mesaj kutusundaki bağlantı).
    function sonucYaz(yil, belgeNo) {
        var $s = $('#lblSonuc').empty();
        if (!belgeNo) return;
        $s.append(document.createTextNode('Oluşturulan belge: '))
            .append($('<a href="#"></a>').text(belgeNo).on('click', function (e) { e.preventDefault(); belgeAc(yil, belgeNo); }));
    }

    $('#btnYazdir').on('click', function () {
        var s = seciliSatirlar();
        if (seciliYok(s)) return;
        postIndir(UC + 'yazdir', { satirlar: s }, $(this));
    });

    // Kişi / firma (eski FirmaGoster -> ListeFirmaKisi.aspx)
    $('#btnFirmaSec').on('click', function () {
        var muh = muhBirimBelge.deger().muhasebe;
        if (!muh) { mesaj('İlk önce belgeyi oluşturacağınız muhasebeyi seçin.', 'warning'); return; }
        Sgb.firmaSec({
            muhasebe: muh, adres: UC + 'firma/ara',
            secilince: function (f) { belge.firmaNo = f.kod; $('#txtIlgiliAd').val(f.ad); }
        });
    });

    // ================================================================ ödeme hesabı listesi (eski ListeOdemeHesabi.aspx)

    var dlgOdeme = document.getElementById('dlgOdemeHesap');
    var odeme = { hedef: null, hesap: '' };

    function odemeListele() {
        var $l = $('#lstOdemeHesap').empty();
        Sgb.al(UC + 'odeme-hesap', { yil: parseInt($('#txtYil').val(), 10) || new Date().getFullYear(), hesap: odeme.hesap, ara: $.trim($('#txtOdemeHesapAra').val()) })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'warning'); return; }
                $.each(v.satirlar || [], function (i, h) { $('<option></option>').val(h.kod).text(h.kod + ' - ' + h.ad).appendTo($l); });
            })
            .fail(hataGoster);
    }

    function odemeHesapAc(hedef, hesap) {
        odeme = { hedef: hedef, hesap: hesap };
        $('#txtOdemeHesapAra').val('');
        dlgOdeme.showModal();
        odemeListele();
    }

    function odemeSec() {
        var k = $('#lstOdemeHesap').val();
        if (!k) { mesaj('Lütfen listeden bilgi seçin.', 'warning'); return; }
        $(odeme.hedef).val(k);
        dlgOdeme.close();
    }

    $('#frmOdemeHesap').on('submit', function (e) { e.preventDefault(); odemeListele(); });
    $('#lstOdemeHesap').on('dblclick', 'option', odemeSec);
    $('#btnOdemeHesapSec').on('click', odemeSec);

    // Eski OdemeHesapGoster: ödeme (kasa) niteliğindeki hesaplar; ButceHesapGoster: yazılan bütçe hesabının (yoksa 630) altı.
    $('#btnKapatmaHesapSec').on('click', function () { odemeHesapAc('#txtKapatmaHesapKod', ''); });
    $('#btnButceHesapSec').on('click', function () {
        var h = $('#txtButceHesapKod').val() || '';
        if (h.replace(/ /g, '') === '') h = '630';
        odemeHesapAc('#txtButceHesapKod', h);
    });

    // ================================================================ damga vergisi hesabı (eski ListeKesintiHesapla.aspx + KesintiYaz)

    var dlgKesinti = document.getElementById('dlgKesinti');

    $('#btnDamgaHesapla').on('click', function () {
        var b = Math.round(belge.kapatmaTutari * 100) / 100;
        $('#txtKsBorc').val(String(b).replace('.', ','));
        $('#txtKsAvans').val('0');
        $('#txtVergiOzel, #txtKDV, #txtGelir').val('');
        $(dlgKesinti).find('input[name=optKdv]').first().prop('checked', true);
        $(dlgKesinti).find('input[name=optVergi]').first().prop('checked', true);
        dlgKesinti.showModal();
    });

    // Girilen kutusuna yazılınca kendi seçeneği işaretlenir (eski onkeypress).
    $(dlgKesinti).on('input', '.kapatma-oran', function () { $('#' + $(this).attr('data-radyo')).prop('checked', true); });

    function tutarOku(s) {
        s = String(s || '');
        for (var j = 0; j < 3; j++) s = s.replace(/\./, '');
        for (var k = 0; k < 3; k++) s = s.replace(/,/, '.');
        return parseFloat(s);
    }

    $('#btnKesintiYaz').on('click', function () {
        var netTutar = 0, vergiTutari = 0;
        var borc = tutarOku($('#txtKsBorc').val());
        var avans = tutarOku($('#txtKsAvans').val());

        if (!(borc > 0)) { mesaj('Borç toplamında hesaplanacak bilgi olmadığı için hesaplama yapılamadı.', 'warning'); return; }
        if (!(avans > 0)) avans = 0;

        var kdv = Number($(dlgKesinti).find('input[name=optKdv]:checked').val()) || 0;
        var $v = $(dlgKesinti).find('input[name=optVergi]:checked');
        var i = parseInt($v.attr('data-sira'), 10) || 0;

        var vergi = 0, tevkifat = '', gelir = 0;
        if (i === 5) {
            vergi = String($('#txtVergiOzel').val()).replace(/\./, '').replace(/,/, '.');
        } else if (i === 6) {
            tevkifat = $('#txtKDV').val();
        } else if (i === 7) {
            gelir = $('#txtGelir').val();
            vergi = $('#txtGelir').val();
        } else {
            vergi = Number($v.val()) / 10;
        }

        if (kdv === 0) netTutar = borc - avans;
        else netTutar = (borc - avans) / (1 + (kdv / 100));

        if (tevkifat !== '') {
            var yer = tevkifat.indexOf('/');
            var r1 = tevkifat.substr(0, yer);
            var r2 = tevkifat.substr(yer + 1, tevkifat.length - yer - 1);
            netTutar = (borc - avans);
            vergiTutari = (netTutar * Number(r1)) / Number(r2);
        } else {
            var oran = gelir > 0 ? 100 : 1000;
            vergiTutari = netTutar * (vergi / oran);
            var vergiArtik = Math.round((vergiTutari - Math.floor(vergiTutari)) * 100);
            vergiTutari = Math.floor(vergiTutari);
            vergiTutari = vergiTutari + (vergiArtik / 100);
        }
        if (!isFinite(vergiTutari)) { mesaj('Kesinti hesaplanamadı; girilen oranı denetleyiniz.', 'warning'); return; }
        $('#txtDamga').val(formatCurrency(vergiTutari));
        dlgKesinti.close();
    });

    // ================================================================ açılış

    var yillar = [];
    Sgb.al(UC + 'tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            yillar = t.yillar || [];
            damgaHesapKod = t.damgaHesapKod || '';
            Sgb.yilKutusu('#txtYil', yillar, t.yil);
            Sgb.yilKutusu('#txtYilBelge', yillar, t.yil);
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
