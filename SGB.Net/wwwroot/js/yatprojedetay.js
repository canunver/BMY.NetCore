// Proje (ButceHazirlikv2/ProjeDetay) ve alt proje (ButceHazirlikv2/AltProjeDetay) sayfaları. ProjeGiris'in sağındaki çerçevede açılır.
//   Genel Bilgiler : kimlik bilgileri, karakteristik seçimi, il / ilçeler, not; Kaydet, Sil, Raporlar, Yat. Raporları,
//                    projede Yeni Alt Proje; alt projede Yeni Bütçe Kaynak ve Taşı (sistem yöneticisi).
//   Diğer sekmeler : alt sayfalar çerçevede (ilk açılışta yüklenir; kaydedilmemiş projede kapalı), Tertipler,
//                    alt projede her bütçe kaynağı için bir sekme (AltProjeButceKaynakGiris).
// Adres parametreleri eskisiyle aynı: ProjeDetay ?yil&kod&senaryoKod, AltProjeDetay ?yil&apKod&pKod&senaryoKod.
// Ağaç ProjeGiris'teki window.YatProje ile güncellenir (eski parent.DalGuncelle / DalSil / AltProjeAc).
$(function () {
    'use strict';

    var $kok = $('.yat-detay');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var ALT = $kok.data('sayfa') === 'AltProjeDetay';
    var YONETICI = String($kok.data('yonetici')) === '1';
    var p = new URLSearchParams(location.search);
    var yil = parseInt(p.get('yil'), 10) || 0;
    var senaryo = p.get('senaryoKod') || '';
    var kod = parseInt(ALT ? p.get('apKod') : p.get('kod'), 10) || 0;   // proje sayfasında proje, alt projede alt proje kodu
    var projeKod = ALT ? (parseInt(p.get('pKod'), 10) || 0) : kod;
    var model = 1;            // alt projenin (ekranda gösterilmeyen) modeli; okunduğu gibi geri yazılır
    var tanim = null;         // api/yatprojegiris/detay-tanim
    var karakteristikler = [];

    // ProjeGiris (çerçevenin sahibi); sayfa doğrudan açıldıysa yoktur.
    function ust() {
        try { return window.parent !== window && window.parent.YatProje ? window.parent.YatProje : null; } catch (e) { return null; }
    }

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function secenekler($ddl, liste, secili) {
        $ddl.empty();
        $.each(liste || [], function (i, k) { $('<option></option>').val(k.kod).text(k.ad).appendTo($ddl); });
        if (secili !== undefined && secili !== null) $ddl.val(String(secili));
        if ($ddl.val() === null && $ddl.children().length) $ddl.prop('selectedIndex', 0);
    }

    function radyo(ad, deger) { $('input[name="' + ad + '"][value="' + (deger ? '1' : '0') + '"]').prop('checked', true); }
    function radyoOku(ad) { return $('input[name="' + ad + '"]:checked').val() === '1'; }
    function yilMetni(d) { return d ? String(d) : ''; }

    // ---------------------------------------------------------------- pencereler

    var dlgOnay = document.getElementById('dlgOnay');
    var onaylaninca = null;
    $(document).on('click', 'dialog [data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    function onayIste(baslik, metin, dugme, islem) {
        $('#dlgOnayBaslik').text(baslik);
        $('#onayMetin').text(metin);
        $('#btnOnayla').text(dugme);
        onaylaninca = islem;
        dlgOnay.showModal();
    }

    $('#btnOnayla').on('click', function () {
        var islem = onaylaninca;
        onaylaninca = null;
        dlgOnay.close();
        if (islem) islem();
    });

    // ---------------------------------------------------------------- sekmeler

    function cerceveAdresi(sayfa) {
        var o = { menuYok: 1, projeKodu: projeKod, yil: yil, senaryoKod: senaryo };
        if (ALT) o.altProjeKodu = kod;
        return sayfa + '?' + $.param(o);
    }

    function sekmeAc($a) {
        var hedef = $a.attr('href');
        $kok.find('.yat-sekmeler > li').removeClass('active').children('a').attr('aria-selected', 'false');
        $a.attr('aria-selected', 'true').parent().addClass('active');
        $kok.children('.yat-panel').prop('hidden', true);
        var $panel = $(hedef).prop('hidden', false);
        // Alt sayfa ilk açılışta yüklenir (eski AutoLoad).
        var $c = $panel.children('iframe');
        if ($c.length && !$c.attr('src') && $a.data('adres')) $c.attr('src', $a.data('adres'));
    }

    $kok.find('.yat-sekmeler').on('click', 'a', function (olay) {
        olay.preventDefault();
        var $a = $(this);
        if ($a.parent().hasClass('disabled')) { mesaj('Önce kaydediniz.', 'info'); return; }
        sekmeAc($a);
    });

    // Kaydedilmemiş (yeni) projede alt sayfa sekmeleri kapalıdır.
    function sekmeleriAyarla() {
        $kok.find('.yat-sekmeler a[data-cerceve]').each(function () {
            $(this).data('adres', kod > 0 ? cerceveAdresi($(this).data('cerceve')) : '');
        });
        $kok.find('.yat-sekmeler a[data-cerceve], .yat-sekmeler a[data-kayitli]').parent().toggleClass('disabled', kod <= 0)
            .children('a').attr('aria-disabled', kod <= 0 ? 'true' : null);
    }

    // ---------------------------------------------------------------- bütçe kaynak sekmeleri (alt proje)

    function kaynakSekmesi(kaynakNo, ad, ac) {
        var id = 'sekKaynak' + kaynakNo;
        var $a = $kok.find('.yat-sekmeler a[href="#' + id + '"]');
        if (!$a.length) {
            $a = $('<a role="tab"></a>').attr('href', '#' + id).text(ad).data('adres',
                'AltProjeButceKaynakGiris?' + $.param({ menuYok: 1, altProjeKodu: kod, yil: yil, senaryoKod: senaryo, kaynakNo: kaynakNo }));
            $('<li role="presentation" class="kaynak-sekme"></li>').append($a).appendTo($kok.find('.yat-sekmeler'));
            $('<div class="yat-panel" role="tabpanel" hidden></div>').attr('id', id)
                .append($('<iframe class="yat-cerceve"></iframe>').attr('title', ad)).appendTo($kok);
        }
        if (ac) sekmeAc($a);
    }

    // Kaynak sayfası (çerçevede) adını değiştirince / kaynağı silince çağırır (eski ButceKaynakTabBaslik / ButceKaynakTabSil).
    window.YatProje = {
        kaynakAdDegisti: function (kaynakNo, ad) {
            $kok.find('.yat-sekmeler a[href="#sekKaynak' + kaynakNo + '"]').text(ad);
        },
        kaynakSilindi: function (kaynakNo) {
            $kok.find('.yat-sekmeler a[href="#sekKaynak' + kaynakNo + '"]').parent().remove();
            $('#sekKaynak' + kaynakNo).remove();
            sekmeAc($kok.find('.yat-sekmeler a[href="#sekGenel"]'));
        }
    };

    // ---------------------------------------------------------------- il / ilçeler

    var grdIlceler = Sgb.grid('#grdIlceler', {
        columnDefs: [
            {
                headerName: '', colId: 'sil', width: 50, sortable: false, resizable: false,
                cellRenderer: function () {
                    var b = document.createElement('button');
                    b.type = 'button';
                    b.className = 'btn btn-link btn-xs';
                    b.title = 'Sil';
                    b.setAttribute('aria-label', 'Sil');
                    b.innerHTML = '<span class="glyphicon glyphicon-remove" aria-hidden="true"></span>';
                    return b;
                },
                onCellClicked: function (e) { grdIlceler.applyTransaction({ remove: [e.data] }); }
            },
            { headerName: 'İl', field: 'ilAd', flex: 1 },
            { headerName: 'İlçe', field: 'ilceAd', flex: 1 }
        ],
        getRowId: function (r) { return String(r.data.ilKod) + '_' + r.data.ilceKod; },
        overlayNoRowsTemplate: '<span>İl / ilçe eklenmemiş.</span>'
    });

    function ilceleriOku() {
        var l = [];
        grdIlceler.forEachNode(function (n) { l.push({ ilKod: n.data.ilKod, ilceKod: n.data.ilceKod, ilAd: n.data.ilAd, ilceAd: n.data.ilceAd }); });
        return l;
    }

    $('#ddlIl').on('change', function () {
        var il = parseInt($(this).val(), 10) || 0;
        secenekler($('#ddlIlce'), []);
        if (il <= 0) return;
        Sgb.al('api/yatprojegiris/ilceler', { il: il }).done(function (s) {
            if (s.hata) { mesaj(s.hata, 'danger'); return; }
            secenekler($('#ddlIlce'), s.ilceler);
        }).fail(hataGoster);
    });

    $('#btnIlceEkle').on('click', function () {
        var il = parseInt($('#ddlIl').val(), 10) || 0, ilce = parseInt($('#ddlIlce').val(), 10) || 0;
        if (il <= 0 || ilce <= 0) { mesaj('İlçe seçiniz', 'warning'); return; }
        var satir = { ilKod: il, ilceKod: ilce, ilAd: $('#ddlIl option:selected').text(), ilceAd: $('#ddlIlce option:selected').text() };
        if (grdIlceler.getRowNode(il + '_' + ilce)) { mesaj('Bu ilçe zaten ekli.', 'info'); return; }
        grdIlceler.applyTransaction({ add: [satir] });
    });

    // ---------------------------------------------------------------- karakteristik

    // Eski KarakteristikGiris.EsitMi / EkDegerAta. ekTurKod: 0 sade, 1 değer önde (ek zorunlu), 2 ek önde (zorunlu),
    // 3 değer önde (ek serbest), 4 ek önde (serbest).
    function esitMi(k, parca) {
        if (k.ekTurKod === 0) return k.deger === parca;
        if (k.ekTurKod === 1 || k.ekTurKod === 3) return parca.indexOf(k.deger) === 0;
        if (k.ekTurKod === 2 || k.ekTurKod === 4) return parca.length >= k.deger.length && parca.slice(-k.deger.length) === k.deger;
        return false;
    }
    function ekDeger(k, parca) {
        if (k.ekTurKod === 1 || k.ekTurKod === 3) return parca.substring(k.deger.length).trim();
        if (k.ekTurKod === 2 || k.ekTurKod === 4) return parca.substring(0, parca.length - k.deger.length).trim();
        return '';
    }
    function duzenlenebilir(r) { return !!r.sec && r.ekTurKod !== 0; }

    var grdKarakteristik = Sgb.grid('#grdKarakteristik', {
        columnDefs: [
            { headerName: 'Seç', field: 'sec', width: 70, editable: true, cellDataType: 'boolean', sortable: false },
            { headerName: 'Karakteristik', field: 'deger', width: 200 },
            {
                headerName: 'Ek Bilgi', field: 'ekDeger', flex: 1,
                editable: function (p) { return duzenlenebilir(p.data); },
                cellClass: function (p) { return duzenlenebilir(p.data) ? 'hucre-acik' : 'hucre-kapali'; }
            }
        ],
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        onCellValueChanged: function (e) { if (e.colDef.field === 'sec') grdKarakteristik.refreshCells({ rowNodes: [e.node], force: true }); }
    });

    $('#btnKarakteristik').on('click', function () {
        // Alandaki değer virgüllerden bölünüp listedekilerle eşlenir (eski KarekteristikBul).
        var satirlar = $.map(karakteristikler, function (k) { return { ekTurKod: k.ekTurKod, deger: k.deger, sec: false, ekDeger: '' }; });
        $.each(($('#txtKarakteristik').val() || '').split(','), function (i, parca) {
            parca = parca.trim();
            if (!parca) return;
            var bulunan = null;
            $.each(satirlar, function (j, s) { if (esitMi(s, parca)) bulunan = s; });
            if (bulunan) { bulunan.sec = true; bulunan.ekDeger = ekDeger(bulunan, parca); }
        });
        grdKarakteristik.setGridOption('rowData', satirlar);
        document.getElementById('dlgKarakteristik').showModal();
    });

    // Eski KarakteristikAktar (eski kod ekTurKod yerine yanlışlıkla "ek" alanına bakıyordu; amaçlanan davranış uygulanır).
    $('#btnKarakteristikAktar').on('click', function () {
        grdKarakteristik.stopEditing();
        var deger = '', hata = '';
        grdKarakteristik.forEachNode(function (n) {
            var r = n.data;
            if (hata || !r.sec) return;
            var ek = (r.ekDeger || '').trim(), alan;
            if (r.ekTurKod === 0) alan = r.deger;
            else if (r.ekTurKod < 3 && !ek) { hata = 'Lütfen ' + r.deger + ' için ek bilgi giriniz!'; return; }
            else if (r.ekTurKod === 1 || r.ekTurKod === 3) alan = (r.deger + ' ' + ek).trim();
            else alan = (ek + ' ' + r.deger).trim();
            deger += (deger ? ', ' : '') + alan;
        });
        if (hata) { mesaj(hata, 'warning'); return; }
        $('#txtKarakteristik').val(deger);
        document.getElementById('dlgKarakteristik').close();
    });

    // ---------------------------------------------------------------- okuma

    function tertipleriYaz(tertipler, hata) {
        var $l = $('#lstTertip').empty();
        $.each(tertipler || [], function (i, t) {
            $('<li></li>').append($('<span class="kod"></span>').text(t.kod)).append(document.createTextNode(t.ad || '')).appendTo($l);
        });
        $('#tertipBos').text(hata || (ALT ? 'Alt projeyle ilişkili tertip yok.' : 'Projeyle ilişkili tertip yok.'))
            .prop('hidden', !!(tertipler && tertipler.length) && !hata);
    }

    function dugmeleriAyarla() {
        $('#btnSil').prop('hidden', !(YONETICI && kod > 0));
        $('#btnYeniAltProje').prop('hidden', kod <= 0);
        $('#btnKaynakEkle, #btnTasi').prop('hidden', !(YONETICI && kod > 0));
        $('#btnRaporlar, #btnYatRaporlari').prop('disabled', kod <= 0);
        // Notu yalnızca sistem yöneticisi değiştirir (eski txtNotAlan.Disabled).
        $('#txtNot').prop('disabled', !YONETICI);
    }

    function bolgeleriCiz(secili) {
        var $l = $('#lstBolge').empty();
        $.each((tanim && tanim.bolgeler) || [], function (i, b) {
            var id = 'bolge' + i;
            $('<label></label>').attr('for', id)
                .append($('<input type="checkbox" />').attr('id', id).val(b.kod).prop('checked', $.inArray(String(b.kod), secili || []) >= 0))
                .append(document.createTextNode(' ' + b.kod + (b.ad && b.ad !== b.kod ? ' - ' + b.ad : '')))
                .appendTo($l);
        });
        if (!$l.children().length) $('<span class="yat-bos"></span>').text('Tanımlı yatırım bölgesi yok.').appendTo($l);
    }

    function projeOku() {
        if (kod <= 0) { tertipleriYaz([]); return; }
        Sgb.al('api/yatprojegiris/proje', { yil: yil, senaryo: senaryo, kod: kod }).done(function (s) {
            if (s.hata) { mesaj(s.hata, 'danger'); return; }
            $('#ddlBirim').val(s.birim || '');
            if ($('#ddlBirim').val() === null) $('#ddlBirim').prop('selectedIndex', 0);
            $('#txtDptProjeNo').val(s.dptProjeNo || '');
            $('#txtAd').val(s.ad || '');
            $('#txtBaslangicYili').val(yilMetni(s.baslangicYili));
            $('#txtBitisYili').val(yilMetni(s.bitisYili));
            $('#ddlSektor').val(s.sektor || '');
            if ($('#ddlSektor').val() === null) $('#ddlSektor').prop('selectedIndex', 0);
            $('#txtKarakteristik').val(s.karakteristik || '');
            radyo('etud', s.etud);
            radyo('eskiYeni', s.eskiYeni);
            radyo('aktif', s.aktif);
            $('#lblDurum').text(s.durum || '');
            $('#txtNot').val(s.not || '');
            grdIlceler.setGridOption('rowData', s.ilceler || []);
            tertipleriYaz(s.tertipler, s.tertipHata);
        }).fail(hataGoster);
    }

    function altProjeOku() {
        Sgb.al('api/yatprojegiris/altproje', { yil: yil, senaryo: senaryo, apKod: kod, pKod: projeKod }).done(function (s) {
            if (s.hata) { mesaj(s.hata, 'danger'); return; }
            projeKod = s.projeKod || projeKod;
            model = s.model || 1;
            $('#txtProjeAd').val(s.projeAd || '');
            $('#txtAd').val(s.ad || '');
            if (s.birim) $('#ddlBirim').val(s.birim);
            if ($('#ddlBirim').val() === null) $('#ddlBirim').prop('selectedIndex', 0);
            $('#txtBaslangicYili').val(yilMetni(s.baslangicYili));
            $('#txtBitisYili').val(yilMetni(s.bitisYili));
            $('#txtKarakteristik').val(s.karakteristik || '');
            $('#txtGerekce').val(s.gerekce || '');
            radyo('dptDeVar', s.dptDeVar !== false);
            bolgeleriCiz($.map(s.bolgeler || [], String));
            $('#txtNot').val(s.not || '');
            grdIlceler.setGridOption('rowData', s.ilceler || []);
            tertipleriYaz(s.tertipler, s.tertipHata);
            $.each(s.kaynaklar || [], function (i, k) { kaynakSekmesi(k.kaynakNo, k.ad, false); });
            sekmeleriAyarla();
        }).fail(hataGoster);
    }

    // ---------------------------------------------------------------- kaydet / sil

    $('#btnKaydet').on('click', function () {
        var $d = $(this).prop('disabled', true);
        var veri, adres;
        if (ALT) {
            adres = 'api/yatprojegiris/altproje-kaydet';
            veri = {
                yil: yil, senaryo: senaryo, kod: kod, projeKod: projeKod, model: model,
                ad: $('#txtAd').val(), birim: $('#ddlBirim').val() || '',
                baslangicYili: $('#txtBaslangicYili').val(), bitisYili: $('#txtBitisYili').val(),
                karakteristik: $('#txtKarakteristik').val(), gerekce: $('#txtGerekce').val(),
                dptDeVar: radyoOku('dptDeVar'),
                bolgeler: $('#lstBolge input:checked').map(function () { return this.value; }).get(),
                not: $('#txtNot').val(), ilceler: ilceleriOku()
            };
        } else {
            adres = 'api/yatprojegiris/proje-kaydet';
            veri = {
                yil: yil, senaryo: senaryo, kod: kod,
                birim: $('#ddlBirim').val() || '', dptProjeNo: $('#txtDptProjeNo').val(), ad: $('#txtAd').val(),
                baslangicYili: $('#txtBaslangicYili').val(), bitisYili: $('#txtBitisYili').val(),
                sektor: $('#ddlSektor').val() || '', karakteristik: $('#txtKarakteristik').val(),
                etud: radyoOku('etud'), eskiYeni: radyoOku('eskiYeni'), aktif: radyoOku('aktif'),
                not: $('#txtNot').val(), ilceler: ilceleriOku()
            };
        }
        Sgb.gonder(adres, veri).done(function (s) {
            if (s.hata) { mesaj(s.hata, 'danger'); return; }
            mesaj(s.bilgi || 'Kayıt işlemi başarıyla tamamlandı.', 'success');
            var yeni = kod <= 0;
            kod = s.kod || kod;
            if (!ALT) projeKod = kod;
            var u = ust();
            if (u) {
                if (ALT) u.dalGuncelle(projeKod, kod, s.ad, 2);
                else u.dalGuncelle(kod, 0, s.ad, 1);
            }
            if (yeni) {
                // Yeni kayıt: sayfa kodla yeniden açılır (alt sayfa sekmeleri ve tertipler bu kodla gelir).
                if (u) { if (ALT) u.altProjeAc(kod, projeKod, s.ad); else u.projeAc(kod, s.ad); }
                else {
                    var q = new URLSearchParams(location.search);
                    q.set(ALT ? 'apKod' : 'kod', kod);
                    location.search = q.toString();
                }
            }
        }).fail(hataGoster).always(function () { $d.prop('disabled', false); });
    });

    $('#btnSil').on('click', function () {
        if (kod <= 0) { mesaj('Herhangi bir Proje seçilmemiş!', 'warning'); return; }
        onayIste('Sil', ALT ? 'Alt proje silinecek. Onaylıyor musunuz?' : 'Proje silinecek. Onaylıyor musunuz?', 'Sil', function () {
            Sgb.gonder(ALT ? 'api/yatprojegiris/altproje-sil' : 'api/yatprojegiris/proje-sil', { yil: yil, senaryo: senaryo, kod: kod })
                .done(function (s) {
                    if (s.hata) { mesaj(s.hata, 'danger'); return; }
                    mesaj(s.bilgi || 'Silindi.', 'success');
                    var u = ust();
                    if (u) { if (ALT) u.dalSil(projeKod, kod, 2); else u.dalSil(kod, 0, 1); }
                    else $kok.find('button').prop('disabled', true);
                }).fail(hataGoster);
        });
    });

    $('#btnYeniAltProje').on('click', function () {
        var u = ust();
        if (u) u.altProjeAc(0, kod, '');
        else location.href = 'AltProjeDetay?' + $.param({ menuYok: 1, yil: yil, apKod: 0, pKod: kod, senaryoKod: senaryo });
    });

    // ---------------------------------------------------------------- yeni bütçe kaynak / taşı (alt proje, sistem yöneticisi)

    $('#btnKaynakEkle').on('click', function () {
        $('#txtKaynakAd').val('');
        $('#chkToplamaDahil').prop('checked', true);
        $('#lstKaynakTur input').prop('checked', false);
        document.getElementById('dlgKaynak').showModal();
    });

    $('#frmKaynak').on('submit', function (olay) {
        olay.preventDefault();
        var veri = {
            yil: yil, senaryo: senaryo, altProjeKod: kod,
            ad: $('#txtKaynakAd').val(), tur: parseInt($('#lstKaynakTur input:checked').val(), 10) || 0,
            toplamaDahil: $('#chkToplamaDahil').prop('checked')
        };
        if (!$.trim(veri.ad)) { mesaj('Kaynak adını giriniz.', 'warning'); return; }
        if (!veri.tur) { mesaj('Kaynak türünü seçiniz.', 'warning'); return; }
        Sgb.gonder('api/yatprojegiris/kaynak-ekle', veri).done(function (s) {
            if (s.hata) { mesaj(s.hata, 'danger'); return; }
            document.getElementById('dlgKaynak').close();
            mesaj(s.bilgi || 'Kayıt işlemi başarıyla tamamlandı.', 'success');
            if (s.kaynakNo) kaynakSekmesi(s.kaynakNo, s.ad, true);
        }).fail(hataGoster);
    });

    var grdTasi = null;
    $('#btnTasi').on('click', function () {
        if (!grdTasi) {
            grdTasi = Sgb.grid('#grdTasi', {
                columnDefs: [
                    { headerName: 'Kod', field: 'kod', width: 80 },
                    { headerName: 'Ad', field: 'ad', flex: 1 },
                    { headerName: 'DPT Proje No', field: 'dptProjeNo', width: 140 }
                ],
                rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true }
            });
        }
        grdTasi.setGridOption('rowData', []);
        document.getElementById('dlgTasi').showModal();
        grdTasi.setGridOption('loading', true);
        Sgb.al('api/yatprojegiris/tasi-projeler', { yil: yil, senaryo: senaryo }).done(function (s) {
            if (s.hata) { mesaj(s.hata, 'danger'); return; }
            grdTasi.setGridOption('rowData', $.grep(s.projeler || [], function (x) { return x.kod !== projeKod; }));
        }).fail(hataGoster).always(function () { grdTasi.setGridOption('loading', false); });
    });

    $('#btnTasiKomut').on('click', function () {
        var sec = grdTasi ? grdTasi.getSelectedRows()[0] : null;
        if (!sec) { mesaj('Taşınacak projeyi seçiniz.', 'warning'); return; }
        document.getElementById('dlgTasi').close();
        onayIste('Onay', 'Bu projeyi taşımak istiyor musunuz?', 'Taşı', function () {
            Sgb.gonder('api/yatprojegiris/altproje-tasi', { yil: yil, senaryo: senaryo, altProjeKod: kod, projeKod: sec.kod })
                .done(function (s) {
                    if (s.hata) { mesaj(s.hata, 'danger'); return; }
                    mesaj(s.bilgi || 'Alt proje taşındı.', 'success');
                    var u = ust();
                    if (u) u.dalSil(projeKod, kod, 2);
                }).fail(hataGoster);
        });
    });

    // ---------------------------------------------------------------- raporlar

    var $menuler = $kok.find('.rapor-menu');
    function menuKapat() { $menuler.removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false'); }

    $menuler.children('.dropdown-toggle').on('click', function (e) {
        e.stopPropagation();
        var $g = $(this).parent(), ac = !$g.hasClass('open');
        menuKapat();
        $g.toggleClass('open', ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false');
        if (ac) $g.find('.dropdown-menu button').first().trigger('focus');
    });
    $(document).on('click', menuKapat);
    $(document).on('keydown', function (e) { if (e.key === 'Escape') menuKapat(); });

    function raporParam(rapor) {
        return { yil: yil, senaryo: senaryo, pKod: projeKod, apKod: ALT ? kod : 0, rapor: rapor };
    }

    // Proje dosyası ve bölümleri (eski Raporlar menüsü).
    $('#lstRaporlar').on('click', 'button', function () {
        menuKapat();
        if (kod <= 0) { mesaj('Önce kaydediniz.', 'warning'); return; }
        var $d = $('#btnRaporlar').prop('disabled', true);
        Sgb.indir('api/yatprojegiris/proje-dosyasi', raporParam($(this).attr('data-rapor'))).then(function () { $d.prop('disabled', false); });
    });

    // Yatırım raporları (eski Yat. Raporları menüsü, Rapor.xml 2002).
    $('#lstYatRaporlari').on('click', 'button', function () {
        menuKapat();
        if (kod <= 0) { mesaj('Önce kaydediniz.', 'warning'); return; }
        var $d = $('#btnYatRaporlari').prop('disabled', true);
        Sgb.indir('api/yatprojegiris/yat-rapor', raporParam($(this).attr('data-rapor'))).then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açılış

    sekmeleriAyarla();
    dugmeleriAyarla();
    if (yil < 2000 || !senaryo) { mesaj('Yıl ya da aşama hatalı!', 'danger'); return; }

    Sgb.al('api/yatprojegiris/detay-tanim', { yil: yil, sayfa: ALT ? 'AltProjeDetay' : 'ProjeDetay' }).done(function (t) {
        if (t.hata) { mesaj(t.hata, 'danger'); return; }
        tanim = t;
        karakteristikler = t.karakteristikler || [];
        secenekler($('#ddlBirim'), t.birimler);
        if (!ALT) secenekler($('#ddlSektor'), t.sektorler);
        secenekler($('#ddlIl'), t.iller, 0);
        secenekler($('#ddlIlce'), []);
        var $yr = $('#lstYatRaporlari').empty();
        $.each(t.yatRaporlari || [], function (i, r) {
            $('<li></li>').append($('<button type="button"></button>').attr('data-rapor', r.kod).text(r.ad)).appendTo($yr);
        });
        if (!$yr.children().length) $('<li class="disabled"><span class="yat-bos">Rapor yok</span></li>').appendTo($yr);
        if (ALT) {
            var $kt = $('#lstKaynakTur').empty();
            $.each(t.kaynakTurleri || [], function (i, k) {
                $('<label class="radio-inline"></label>').append($('<input type="radio" name="kaynakTur" />').val(k.kod))
                    .append(document.createTextNode(' ' + k.ad)).appendTo($kt);
            });
            bolgeleriCiz([]);
            altProjeOku();
        } else projeOku();
    }).fail(hataGoster);
});
