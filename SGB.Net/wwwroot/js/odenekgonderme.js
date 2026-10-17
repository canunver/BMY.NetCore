// Ödenek Gönderme / Tenkis belge düzenleme (ButceMuhasebe/OdenekGondermeTenkisMPExt; eski aynı adlı Ext.NET sayfası).
// Belge türü (data-bt: 3 ödenek gönderme, 4 tenkis) ve talep (data-talep) sayfa kökünde; bütün isteklere bt ve talep eklenir.
// Uçlar: /api/odenekgonderme/* (Muhasebe/OdenekGondermeUclari.cs). Pencereler: tertip seçimi (ortak Sgb.tertipSec, js/tertipsec.js),
// İÖİ / YİKOB iş seçimi (eski IOIProjeSecim), belgeye doküman ekle (ortak ekli dosyalar, modül BUM).
// Çerçeve (OdenekGondermeTenkisFrame) Belge Arama listesinden açılan belgeyi window.OgBelgeYukle(yil, belgeNo) ile okutur.
$(function () {
    'use strict';

    var $kok = $('.og-form');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var BT = parseInt($kok.attr('data-bt'), 10) || 0;
    var TALEP = $kok.attr('data-talep') || '';
    var UC = 'api/odenekgonderme/';
    var IS_IPUCU = 'YİKOB ve İOİ ödenek göndermelerinde lütfen mercek tuşu ile iş seçiniz.';
    var tutar2 = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    var tanim = { satirSayisi: 20, kurum: '' };
    var belge = null;      // son okunan belge (durum, süreç, ekli dosya adları)
    var isKodu = 0;
    var satirNo = 0;

    function p(ek) { return $.extend({ bt: BT, talep: TALEP }, ek || {}); }
    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function yil() { return parseInt($('#ddlYil').val(), 10) || 0; }
    function belgeNo() { return $.trim($('#txtBelgeNo').val()); }
    function para(v) { return v === null || v === undefined || v === '' ? '' : tutar2.format(Number(v) || 0); }

    // "1.234,56" ya da "1234.56" -> sayı
    function sayiCoz(s) {
        if (typeof s === 'number') return s;
        s = $.trim(String(s || '')).replace(/\s/g, '');
        if (s === '') return 0;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = parseFloat(s);
        return isNaN(n) ? null : n;
    }

    // ---------------------------------------------------------------- onay sorusu

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;
    function sor(metin, evet, is) {
        $('#soruMetin').text(metin);
        $('#btnSoruEvet').text(evet || 'Evet');
        soruIsi = is;
        dlgSoru.showModal();
    }
    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        var is = soruIsi;
        soruIsi = null;
        if (is) is();
    });
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- muhasebe / birim

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: yil,
        kurumsalGizle: true
    });

    // ---------------------------------------------------------------- tertip satırları

    function bosSatir() {
        return { _id: ++satirNo, TERTIP: '', TERTIPPRG: '', MIKTAR: 0, KULLANILABILEN: 0, GONDERILEN: 0, HARCANAN: 0, DONEMMP: 0, TOPLAMMP: 0 };
    }

    function mercekHucre(p2) {
        var $h = $('<span class="og-hucre-sec"></span>');
        $('<span class="og-hucre-metin"></span>').text(p2.value || '').appendTo($h);
        $('<button type="button" class="btn btn-link btn-xs og-tertip-sec" title="Tertip Seç"><span class="glyphicon glyphicon-search" aria-hidden="true"></span><span class="sr-only">Tertip Seç</span></button>')
            .on('click', function (e) { e.stopPropagation(); tertipPencereAc(p2.node.rowIndex); })
            .appendTo($h);
        return $h[0];
    }

    function sariSutun(alan, ad) {
        return { field: alan, headerName: ad, width: 130, type: 'rightAligned', cellClass: ['og-sari', 'ag-right-aligned-cell'], valueFormatter: function (x) { return para(x.value); } };
    }

    // Öğeyi sayfa alanının (.sayfa-icerik) kalan yüksekliğine yayar; sayfada dikey kaydırma çıkmaz, kaydırma öğenin içinde kalır.
    function yuksekligeYay(el, enAz) {
        if (!el || el.offsetParent === null) return;
        var kap = el.closest('.sayfa-icerik') || document.body, cs = getComputedStyle(kap);
        var kaydirir = /(auto|scroll)/.test(cs.overflowY);
        var kr = kap.getBoundingClientRect(), kay = kaydirir ? kap.scrollTop : 0;
        var gorunen = kaydirir ? kap.clientHeight : window.innerHeight - kr.top;
        var ust = el.getBoundingClientRect().top - kr.top + kay;
        var son = 0;
        $(kap).children().each(function () {
            if (this.offsetParent === null && getComputedStyle(this).position !== 'fixed') return;
            son = Math.max(son, this.getBoundingClientRect().bottom - kr.top + kay + parseFloat(getComputedStyle(this).marginBottom || 0));
        });
        son += parseFloat(cs.paddingBottom || 0);
        var bosluk = Math.max(0, son - (ust + el.offsetHeight));
        el.style.height = Math.max(enAz, Math.floor(gorunen - ust - bosluk - 1)) + 'px';
    }

    var grid = Sgb.grid('#grdTertip', {
        columnDefs: [
            { headerName: '#', valueGetter: 'node.rowIndex + 1', width: 56, sortable: false },
            { field: 'TERTIP', headerName: 'Gider Bütçesi Tertibi', width: 300, editable: true, sortable: false, cellRenderer: mercekHucre, cellEditor: 'agTextCellEditor', cellEditorParams: { maxLength: 60 } },
            { field: 'TERTIPPRG', headerName: 'Gider Bütçesi Tertibi (Program)', width: 280, sortable: false, cellRenderer: mercekHucre },
            {
                field: 'MIKTAR', headerName: 'Miktar', width: 140, type: 'rightAligned', editable: true, sortable: false, cellClass: ['og-girilir', 'ag-right-aligned-cell'],
                valueFormatter: function (x) { return para(x.value); },
                valueParser: function (x) { var n = sayiCoz(x.newValue); return n === null ? x.oldValue : n; }
            },
            sariSutun('KULLANILABILEN', 'Kullanılabilen'),
            sariSutun('GONDERILEN', 'Gönderilen'),
            sariSutun('HARCANAN', 'Harcanan'),
            sariSutun('DONEMMP', 'Dönem M.P.'),
            sariSutun('TOPLAMMP', 'Toplam M.P.')
        ],
        rowData: [],
        rowHeight: 28,
        headerHeight: 32,
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        getRowId: function (x) { return String(x.data._id); },
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        onCellValueChanged: function (e) {
            if (e.newValue === e.oldValue) return;
            if (e.colDef.field === 'MIKTAR') toplamYaz();
            else if (e.colDef.field === 'TERTIP') satirBilgiOku(e.node);
        }
    });

    function satirlar() {
        var r = [];
        grid.forEachNode(function (n) { r.push(n.data); });
        return r;
    }

    function satirlariYaz(liste) {
        liste = $.map(liste || [], function (s) { return $.extend(bosSatir(), s); });
        for (var i = 0; i < tanim.satirSayisi; i++) liste.push(bosSatir());
        grid.setGridOption('rowData', liste);
        toplamYaz();
    }

    function toplamYaz() {
        var t = 0;
        grid.forEachNode(function (n) { t += Number(n.data.MIKTAR) || 0; });
        $('#lblToplam').text(tutar2.format(t));
    }

    // Tertip değişince kullanılabilir / gönderilen / harcanan / masraf planı (eski HucreDegisti -> OdenekBilgiGoster).
    function satirBilgiOku(node) {
        var tertip = $.trim(node.data.TERTIP || '');
        if (!tertip) {
            node.setData($.extend({}, node.data, { TERTIPPRG: '', KULLANILABILEN: 0, GONDERILEN: 0, HARCANAN: 0, DONEMMP: 0, TOPLAMMP: 0 }));
            return;
        }
        var d = muhBirim.deger();
        Sgb.al(UC + 'satir', p({ yil: yil(), tertip: tertip, muhasebe: d.muhasebe || '', birim: d.birim, planlanan: $('#chkPlanlanan').prop('checked') }))
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var g = grid.getRowNode(node.id);
                if (!g) return;
                // Eski sayfadaki gibi Toplam M.P. sütununa dönem masraf planı yazılır.
                g.setData($.extend({}, g.data, {
                    KULLANILABILEN: v.kullanilabilir, GONDERILEN: v.gonderilen, HARCANAN: v.harcanan,
                    DONEMMP: v.donemMP, TOPLAMMP: v.donemMP, TERTIPPRG: v.tertipPrg
                }));
            })
            .fail(hataGoster);
    }

    $('#btnSatirAc').on('click', function () {
        grid.applyTransaction({ add: [bosSatir()] });
    });
    $('#btnSatirSil').on('click', function () {
        var secili = grid.getSelectedRows();
        if (!secili.length) { mesaj('Silinecek satırları seçiniz.', 'warning'); return; }
        grid.applyTransaction({ remove: secili });
        toplamYaz();
    });
    $('#btnHepsiniSec').on('click', function () { grid.selectAll(); });

    // Kopyala: seçili (yoksa dolu) satırların tertip ve miktarı, sekmeyle ayrılmış metin olarak panoya.
    $('#btnKopyala').on('click', function () {
        var secili = grid.getSelectedRows();
        if (!secili.length) secili = $.grep(satirlar(), function (s) { return !!$.trim(s.TERTIP || ''); });
        if (!secili.length) { mesaj('Kopyalanacak satır yok.', 'warning'); return; }
        var metin = $.map(secili, function (s) { return (s.TERTIP || '') + '\t' + String(Number(s.MIKTAR) || 0).replace('.', ','); }).join('\r\n');
        if (!navigator.clipboard) { mesaj('Tarayıcı panoya yazmaya izin vermiyor.', 'warning'); return; }
        navigator.clipboard.writeText(metin).then(function () { mesaj(secili.length + ' satır kopyalandı.', 'success'); },
            function () { mesaj('Panoya yazılamadı.', 'warning'); });
    });

    // Yapıştır: panodaki satırlar (tertip [sekme] miktar) seçili satırdan (yoksa ilk satırdan) başlayarak yazılır (eski EXTVeriYapistir).
    $('#btnYapistir').on('click', function () {
        if (!navigator.clipboard || !navigator.clipboard.readText) { mesaj('Tarayıcı panodan okumaya izin vermiyor.', 'warning'); return; }
        navigator.clipboard.readText().then(function (metin) {
            var satirlarP = $.grep((metin || '').split(/\r?\n/), function (s) { return $.trim(s) !== ''; });
            if (!satirlarP.length) { mesaj('Panoda yapıştırılacak veri yok.', 'warning'); return; }
            var secili = grid.getSelectedNodes();
            var bas = secili.length ? Math.min.apply(null, $.map(secili, function (n) { return n.rowIndex; })) : 0;
            var eksik = bas + satirlarP.length - grid.getDisplayedRowCount();
            var ekle = [];
            for (var e = 0; e < eksik; e++) ekle.push(bosSatir());
            if (ekle.length) grid.applyTransaction({ add: ekle });
            $.each(satirlarP, function (i, s) {
                var h = s.split('\t');
                var node = grid.getDisplayedRowAtIndex(bas + i);
                if (!node) return;
                var miktar = h.length > 1 ? sayiCoz(h[1]) : node.data.MIKTAR;
                node.setData($.extend({}, node.data, { TERTIP: $.trim(h[0]), MIKTAR: miktar === null ? 0 : miktar }));
                satirBilgiOku(node);
            });
            toplamYaz();
        }, function () { mesaj('Panodan okunamadı.', 'warning'); });
    });

    // ---------------------------------------------------------------- form doldur / temizle

    function isYaz(kod, ad) {
        isKodu = parseInt(kod, 10) || 0;
        $('#lblIsAdi').text(isKodu > 0 ? (ad || '') : IS_IPUCU).toggleClass('og-ipucu', !(isKodu > 0));
    }

    function ekDosyaGoster(goster, ad1, ad2) {
        $('#grpEkDosyalar').prop('hidden', !goster);
        $('.og-ek[data-no="1"] .og-ek-ad').val(ad1 || '');
        $('.og-ek[data-no="2"] .og-ek-ad').val(ad2 || '');
    }

    function formTemizle() {
        belge = null;
        $('#txtBelgeTarih').val(tanim.bugun || Sgb.bugun());
        $('#lblDurum, #lblVergiNo, #lblSurecNo').text('');
        $('#btnSurecAc').prop('disabled', true);
        $('#txtBelgeNo, #txtAciklama, #txtYaziNo, #txtYaziTarih, #txtIstekNo, #txtIstekTarih').val('');
        muhBirim.ata({});
        isYaz(0);
        ekDosyaGoster(false);
        $('#ddlKurumsal').val('');
        $('#btnTalepGonder, #btnRevizeIptal').prop('hidden', true);
        satirlariYaz([]);
    }

    function belgeYaz(b) {
        belge = b;
        $('#txtBelgeTarih').val(b.belgeTarih || '');
        $('#txtBelgeNo').val(b.belgeNo || '');
        muhBirim.ata({ muhasebe: b.muhasebe, birim: b.birim, birimAd: b.birimAd });
        muhBirim.yenile();
        isYaz(b.isKodu, b.isAdi);
        $('#txtAciklama').val(b.aciklama || '');
        $('#txtYaziNo').val(b.yaziNo || '');
        $('#txtYaziTarih').val(b.yaziTarih || '');
        $('#txtIstekNo').val(b.istekYaziNo || '');
        $('#txtIstekTarih').val(b.istekYaziTarih || '');
        $('#lblSurecNo').text(b.surecYilNo || '');
        $('#btnSurecAc').prop('disabled', !$.trim(b.surecYilNo || '').replace('/', '').replace(/^0+$/, ''));
        $('#lblDurum').text(b.durum || '');
        $('#lblVergiNo').text(b.vergiNo || '');
        ekDosyaGoster(b.ekliDosyalarGoster, b.ekDosyaAd1, b.ekDosyaAd2);
        $('#btnTalepGonder').prop('hidden', !b.talepGonderGoster);
        $('#btnRevizeIptal').prop('hidden', !b.revizeIptalGoster);
        var $k = $('#ddlKurumsal');
        $k.val($k.find('option').filter(function () { return this.value === b.kurumsal; }).length ? b.kurumsal : '');
        satirlariYaz(b.satirlar);
    }

    function listele(yilDeger, no) {
        if (yilDeger) $('#ddlYil').val(String(yilDeger));
        if (no !== undefined) $('#txtBelgeNo').val(no);
        if (!belgeNo()) { mesaj('Belge listelemesi için, Belge No alanına görmek istediğiniz belgenin nosunu yazmanız gerekmektedir.', 'warning'); return $.Deferred().reject().promise(); }
        grid.setGridOption('loading', true);
        return Sgb.al(UC + 'belge', p({ yil: yil(), belgeNo: belgeNo() }))
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                belgeYaz(v.belge);
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    $('#frmBelgeNo').on('submit', function (e) { e.preventDefault(); listele(); });
    window.OgBelgeYukle = function (y, no) { listele(y, no); };

    $('#btnTemizle').on('click', formTemizle);

    // ---------------------------------------------------------------- kaydet / talep gönder / revize iptal / yazdır

    $('#btnKaydet').on('click', function () {
        grid.stopEditing();
        sor('Veriler Kaydedilecektir, Onaylıyor musunuz?', 'Kaydet', function () {
            var d = muhBirim.deger();
            var veri = {
                belgeTur: BT, talep: TALEP, yil: yil(), belgeTarih: $('#txtBelgeTarih').val(), belgeNo: belgeNo(),
                muhasebe: d.muhasebe || 0, birim: d.birim, isKodu: isKodu, aciklama: $('#txtAciklama').val(),
                yaziNo: $('#txtYaziNo').val() || '', yaziTarih: $('#txtYaziTarih').val() || '',
                istekYaziNo: $('#txtIstekNo').val(), istekYaziTarih: $('#txtIstekTarih').val(),
                surecYil: belge ? belge.surecYil : 0, surecNo: belge ? belge.surecNo : '',
                planlanan: $('#chkPlanlanan').prop('checked'), muhHarcamaKontrolYapma: $('#chkMuhHarcama').prop('checked'),
                satirlar: $.map(satirlar(), function (s) { return { tertip: s.TERTIP || '', miktar: Number(s.MIKTAR) || 0 }; })
            };
            var $d = $('#btnKaydet').prop('disabled', true);
            Sgb.gonder(UC + 'kaydet', veri)
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata || 'Belge kaydı yapılamadı.', 'danger'); return; }
                    mesaj(v.bilgi || 'Belge kaydedildi.', 'success');
                    // Belge yeniden okunur: numara, durum ve düğmeler (talep gönder, ekli belgeler) güncellenir.
                    if (v.belgeNo) listele(null, v.belgeNo);
                })
                .fail(hataGoster)
                .always(function () { $d.prop('disabled', false); });
        });
    });

    $('#btnTalepGonder').on('click', function () {
        if (!belgeNo()) return;
        sor('Veriler Merkez Birime İletilecektir, Onaylıyor musunuz?', 'Gönder', function () {
            Sgb.gonder(UC + 'talep-gonder', { belgeTur: BT, talep: TALEP, yil: yil(), belgeNo: belgeNo() })
                .done(function (v) {
                    if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                    mesaj(v.bilgi, 'success');
                    listele();
                })
                .fail(hataGoster);
        });
    });

    $('#btnRevizeIptal').on('click', function () {
        if (!belgeNo()) return;
        sor('Belge ile ilgili masraf planı revizesi iptal edilecek. Onaylıyor musunuz?', 'Revize İptal', function () {
            Sgb.gonder(UC + 'revize-iptal', { belgeTur: BT, talep: TALEP, yil: yil(), belgeNo: belgeNo() })
                .done(function (v) { mesaj(v.basarili ? v.bilgi : v.hata, v.basarili ? 'success' : 'danger'); })
                .fail(hataGoster);
        });
    });

    $('#btnYazdir').on('click', function () {
        if (!belgeNo()) { mesaj('Yazdırmak için önce belgeyi kaydediniz ya da listeleyiniz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir(UC + 'yazdir', p({ yil: yil(), belgeNo: belgeNo() })).then(function () { $d.prop('disabled', false); });
    });

    // Hızlı rapor (eski HizliRaporAl): 1 KBS-SGB Fark (Excel), 2 Seyyar Görev bilgi sayfası.
    $('#btnRaporAl').on('click', function () {
        var d = muhBirim.deger();
        var kod = $('#ddlHizliRapor').val();
        if (kod === '2') {
            Sgb.pencere('../HarcamaSurecleri/SeyyarGorevBilgi.aspx?' + $.param({ yil: yil(), kur: tanim.kurum, muhasebe: d.muhasebe || -1, birim: d.birim }), 'Seyyar Görev');
            return;
        }
        var $d = $(this).prop('disabled', true);
        Sgb.indir(UC + 'hizli-rapor', p({ kod: kod, yil: yil(), muhasebe: d.muhasebe || -1, birim: d.birim })).then(function () { $d.prop('disabled', false); });
    });

    $('#btnOncekiler').on('click', function () {
        var d = muhBirim.deger();
        Sgb.pencere('OdenekGondermeTenkisEkonomik.aspx?' + $.param({ muhasebe: d.muhasebe || '', birim: d.birim }), 'Önceden gönderilen ödenekler');
    });

    // Süreç (eski SurecSayfaAc: "yıl/no").
    $('#btnSurecAc').on('click', function () {
        var s = (belge && belge.surecYilNo || '').split('/');
        if (s.length < 2) return;
        Sgb.pencere('../GelirSurecleri/SurecFormAna.aspx?' + $.param({ tur: 1, yil: $.trim(s[0]), surecNo: $.trim(s[1]) }), 'Süreç');
    });

    // ---------------------------------------------------------------- imzalı / destekleyici belge (modül OGT)

    function ekForm() { return yil() + '-' + BT + '-' + belgeNo(); }

    $('.og-ek .og-ek-yukle').on('click', function () {
        if (!belgeNo()) { mesaj('Dosya yüklemek için önce belgeyi kaydediniz.', 'warning'); return; }
        $(this).closest('.og-ek').find('.og-ek-dosya').val('').trigger('click');
    });

    $('.og-ek .og-ek-dosya').on('change', function () {
        var $ek = $(this).closest('.og-ek');
        var dosya = this.files && this.files[0];
        if (!dosya) return;
        var fd = new FormData();
        fd.append('bt', BT);
        fd.append('talep', TALEP);
        fd.append('yil', yil());
        fd.append('belgeNo', belgeNo());
        fd.append('dosyaNo', $ek.attr('data-no'));
        fd.append('dosya', dosya);
        var $d = $ek.find('button').prop('disabled', true);
        fetch(Sgb.KOK + UC + 'dosya-yukle', {
            method: 'POST', body: fd, credentials: 'same-origin',
            headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).then(function (y) {
            if (y.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return null; }
            if (!y.ok) throw new Error();
            return y.json();
        }).then(function (v) {
            if (!v) return;
            if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
            $ek.find('.og-ek-ad').val(v.ad || dosya.name);
            mesaj(v.bilgi, 'success');
        }).catch(function () { mesaj('Dosya yüklenemedi.', 'danger'); })
            .finally(function () { $d.prop('disabled', false); });
    });

    $('.og-ek .og-ek-indir').on('click', function () {
        var $ek = $(this).closest('.og-ek');
        if (!belgeNo() || !$.trim($ek.find('.og-ek-ad').val())) { mesaj('İndirilecek dosya yok.', 'warning'); return; }
        window.open(Sgb.KOK + 'api/ekli-dosya/indir?' + $.param({ modul: 'OGT', form: ekForm(), no: $ek.attr('data-no') }), '_blank');
    });

    // ---------------------------------------------------------------- belgeye doküman ekle (eski DosyaIslem.aspx: BUM, yil_belgeNo_belgeTur_kurum)

    var ekliDosyalar = null;
    $('#btnDokuman').on('click', function () {
        var kurum = $('#ddlKurumsal').val();
        var form = yil() > 0 && belgeNo() && kurum ? yil() + '_' + belgeNo() + '_' + BT + '_' + kurum : '-1';
        if (!ekliDosyalar) ekliDosyalar = Sgb.ekliDosyalar('#ekliDosyalar', { modul: 'BUM', form: form });
        else ekliDosyalar.formDegistir(form);
        if (form === '-1') mesaj('Doküman eklemek için yıl, belge no ve kurum kodu dolu olmalıdır.', 'info');
        document.getElementById('dlgDokuman').showModal();
    });

    // ---------------------------------------------------------------- İÖİ / YİKOB iş seçimi

    var dlgIs = document.getElementById('dlgIs');
    var isGrid = null;

    function isAra() {
        var d = muhBirim.deger();
        isGrid.setGridOption('loading', true);
        Sgb.al(UC + 'ioi', p({ yil: yil(), muhasebe: d.muhasebe || 0, birim: d.birim, ad: $('#txtIsAdi').val(), isTur: $('input[name="isTur"]:checked').val() }))
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                isGrid.setGridOption('overlayNoRowsTemplate', '<span>Kayıt bulunamadı.</span>');
                isGrid.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { isGrid.setGridOption('loading', false); });
    }

    $('#btnIsSec').on('click', function () {
        if (!isGrid) {
            var sayi = function (alan, ad) { return { field: alan, headerName: ad, width: 130, type: 'rightAligned', valueFormatter: function (x) { return para(x.value); } }; };
            isGrid = Sgb.grid('#grdIs', {
                columnDefs: [
                    { field: 'siraNo', headerName: 'İş Kodu', width: 90 },
                    { field: 'ioiProjeAdi', headerName: 'İş Adı', width: 300, tooltipField: 'ioiProjeAdi' },
                    { field: 'ilAdi', headerName: 'İl Adı', width: 120 },
                    sayi('aktarilanTutar', 'MP Onaylı'), sayi('islemleEklenen', 'MP Yeni'),
                    sayi('iOIHarc', 'Gönderilen Onaylı'), sayi('gonderilen', 'Gönderilen Yeni')
                ],
                overlayNoRowsTemplate: '<span>İş adını yazıp İş Ara\'ya basınız.</span>',
                onRowDoubleClicked: function (e) {
                    isYaz(e.data.siraNo, e.data.siraNo + '- ' + (e.data.ioiProjeAdi || ''));
                    dlgIs.close();
                }
            });
        }
        dlgIs.showModal();
    });
    $('#frmIsAra').on('submit', function (e) { e.preventDefault(); isAra(); });
    $('#btnIsTemizle').on('click', function () { isYaz(0); dlgIs.close(); });

    // ---------------------------------------------------------------- tertip seçimi (eski ListeTertip pencereleri; ortak Sgb.tertipSec)

    // Ödenek göndermede (talep değilse) eski ButceHazirlikv2/ListeTertip, diğerlerinde ButceMuhasebe/ListeTertip.
    function tertipPencereAc(satir) {
        var d = muhBirim.deger();
        if (!$('#ddlKurumsal').val() || !d.muhasebe || !d.birim) {
            mesaj('Lütfen ilk önce kurum, muhasebe, birim bilgilerini girin.', 'warning');
            return;
        }
        var hedefSatir = satir || 0;
        Sgb.tertipSec({
            tur: BT === 3 && (TALEP === '' || TALEP === '0') ? 'bhm' : 'muh',
            yil: yil(), belgeTur: BT, muhasebe: d.muhasebe, birim: d.birim, kur: $('#ddlKurumsal').val() || '',
            secilince: function (secili) { tertipleriAktar(hedefSatir, secili); }
        });
    }

    // Seç ve Kapat (eski ListeTertipAktar): seçilenler hedef satırdan başlayarak yazılır.
    function tertipleriAktar(hedefSatir, secili) {
        var eksik = hedefSatir + secili.length - grid.getDisplayedRowCount();
        var ekle = [];
        for (var e = 0; e < eksik; e++) ekle.push(bosSatir());
        if (ekle.length) grid.applyTransaction({ add: ekle });
        $.each(secili, function (i, s) {
            var node = grid.getDisplayedRowAtIndex(hedefSatir + i);
            if (!node) return;
            var yeni = { TERTIP: s.TERTIP, TERTIPPRG: s.TERTIPPRG || '', KULLANILABILEN: s.KALAN, TOPLAMMP: s.MP };
            if (s.GONDERILEN !== undefined) yeni.GONDERILEN = s.GONDERILEN;
            node.setData($.extend({}, node.data, yeni));
        });
    }

    // ---------------------------------------------------------------- açılış (eski Page_Load / FormAyarla)

    Sgb.al(UC + 'tanim', p())
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            var $y = Sgb.yilKutusu('#ddlYil', t.yillar);
            $y.val(String(t.yil));

            var $k = $('#ddlKurumsal').empty();
            if (!TALEP) $('<option value=""></option>').appendTo($k);
            $.each(t.kurumlar || [], function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($k); });

            $('#lblPlanlanan').prop('hidden', !t.planlananGoster);
            $('#lblMuhHarcama').prop('hidden', !t.muhHarcamaGoster);
            $('#grpSecenekler').prop('hidden', !t.planlananGoster && !t.muhHarcamaGoster);

            grid.setColumnsVisible(['DONEMMP', 'TOPLAMMP'], !!t.mpOnayli);
            formTemizle();

            // Adres satırında yıl ve belge no varsa belge açılır (Liste'den yeni pencerede açma).
            var q = new URLSearchParams(window.location.search);
            var qNo = q.get('belgeNo') || q.get('belgeno');
            if (qNo) listele(parseInt(q.get('yil'), 10) || t.yil, qNo);
        })
        .fail(hataGoster);
    function tabloYay() { yuksekligeYay(document.getElementById('grdTertip'), 200); }
    $(window).on('resize', tabloYay);
    // Üst bölümler (ek dosya alanı, talep alanları) açılıp kapanınca tablo yeniden yayılır.
    if (window.ResizeObserver) new ResizeObserver(tabloYay).observe(document.querySelector('.og-form .og-ust'));
    tabloYay();
});
