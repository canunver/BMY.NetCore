// Çapraz Tablo (ButceMuhasebe/CaprazTablo; eski aynı adlı Ext.NET sayfası). Satırlara ve (sütun türü Hesap Kod ise) sütunlara
// hesap kodları eklenir; Tablo Göster sorgu alanlarıyla Excel çapraz tablosunu indirir. Satır, sütun ve sorgu alanları adıyla
// saklanır (Kaydet / Farklı Kaydet), Kayıtlı Sorgular penceresinden açılır ya da silinir. Uçlar: /api/capraztablo/*
// (Muhasebe/CaprazTabloUclari.cs). Ortak bileşenler: Sgb.hesapKodu, Sgb.muhasebeBirim, Sgb.agac (kod ağaçları), Sgb.grid.
$(function () {
    'use strict';

    var $kok = $('.capraz-tablo');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/capraztablo/';
    var SAKLA = 'capraztablo.';
    var tanim = {};
    var acik = { kod: 0, adi: '' }; // açık kayıtlı sorgu (eski hdnKod)

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function sakliOku(ad) { try { return localStorage.getItem(SAKLA + ad); } catch (e) { return null; } }
    function sakliYaz(ad, deger) { try { localStorage.setItem(SAKLA + ad, deger); } catch (e) { /* özel pencere */ } }
    function yeniKod() {
        if (window.crypto && crypto.randomUUID) return crypto.randomUUID();
        return 'k' + Date.now().toString(36) + Math.random().toString(36).slice(2);
    }

    // ---------------------------------------------------------------- pencereler, onay, açılır menü

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    var dlgSoru = document.getElementById('dlgSoru');
    var soruIsi = null;
    function sor(metin, evet, is) {
        $('#soruMetin').text(metin);
        $('#btnSoruEvet').text(evet);
        soruIsi = is;
        dlgSoru.showModal();
    }
    $('#btnSoruEvet').on('click', function () {
        dlgSoru.close();
        var is = soruIsi;
        soruIsi = null;
        if (is) is();
    });

    function menuKapat() { $kok.find('.ct-menu.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false'); }
    $kok.on('click', '.ct-menu > .dropdown-toggle', function (e) {
        e.stopPropagation();
        var $g = $(this).parent();
        var ac = !$g.hasClass('open');
        menuKapat();
        $g.toggleClass('open', ac).children('.dropdown-toggle').attr('aria-expanded', ac ? 'true' : 'false');
    });
    $(document).on('click', menuKapat);

    $('#btnYardim').on('click', function () {
        var $p = $('#pnlYardim');
        var ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false');
    });

    // Sorgu alanları daraltılabilir (eski West paneli Collapsible); seçim tarayıcıda hatırlanır.
    function kriterGoster(goster) {
        $kok.toggleClass('ct-kriter-gizli', !goster);
        $('#pnlKriter').prop('hidden', !goster);
        $('#btnKriterGoster').prop('hidden', goster);
        sakliYaz('kriterGizli', goster ? '0' : '1');
    }
    $('#btnKriterGizle').on('click', function () { kriterGoster(false); });
    $('#btnKriterGoster').on('click', function () { kriterGoster(true); });

    // ---------------------------------------------------------------- sorgu alanları

    function yil() {
        if ($('#chkYilOnemsiz').prop('checked')) return 0;
        return parseInt($('#txtYil').val(), 10) || 0;
    }
    // Kod ağaçları ve hesap planı için yıl (Yıl Önemsiz ise bu yıl; eski HesapListesiDoldur).
    function kodYil() { return yil() || new Date().getFullYear(); }

    $('#chkYilOnemsiz').on('change', function () { $('#txtYil').prop('disabled', this.checked); });

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', birim: 'txtBirim', kurumsal: 'txtKurumsal', ad: 'lblBirimAd' },
        yil: kodYil,
        kurumsalGizle: true,
        adGizle: true
    });

    // "13.01.31.00 - Ad" -> "13.01.31.00"
    function kodAl(metin) {
        var s = $.trim(metin || '');
        var i = s.indexOf('-');
        return $.trim(i > 0 ? s.substring(0, i) : s);
    }
    function alan(ad) { return $('.kod-alan[data-alan="' + ad + '"] input'); }

    // Eski pgFiltre.getSource() adlarıyla sorgu alanları (saklananlar da bu adlarla).
    function filtre() {
        var d = muhBirim.deger();
        return {
            yil: String(yil()), durum: $('#ddlDurum').val() || '',
            muhasebe: d.muhasebe ? String(d.muhasebe) : '', birim: d.birim ? String(d.birim) : '',
            yevmiyeTarihi1: $('#txtYevmiyeTarih1').val(), yevmiyeTarihi2: $('#txtYevmiyeTarih2').val(),
            belgeTarihi1: $('#txtBelgeTarih1').val(), belgeTarihi2: $('#txtBelgeTarih2').val(),
            ilgiliKod: $.trim($('#txtIlgiliKod').val()), althesapNo: $.trim($('#txtAltHesapNo').val()),
            kur: kodAl(alan('kur').val()), fon: kodAl(alan('fon').val()), fin: kodAl(alan('fin').val()),
            eko: kodAl(alan('eko').val()), peb: kodAl(alan('peb').val())
        };
    }

    // Saklanan sorgu alanlarını yazar (eski GetirDetay: yıl ve durum yazılmazdı).
    function filtreYaz(f) {
        f = f || {};
        muhBirim.ata({ muhasebe: f.muhasebe || '', birim: f.birim || '' });
        $('#txtYevmiyeTarih1').val(f.yevmiyeTarihi1 || '');
        $('#txtYevmiyeTarih2').val(f.yevmiyeTarihi2 || '');
        $('#txtBelgeTarih1').val(f.belgeTarihi1 || '');
        $('#txtBelgeTarih2').val(f.belgeTarihi2 || '');
        $('#txtIlgiliKod').val(f.ilgiliKod || '');
        $('#txtAltHesapNo').val(f.althesapNo || '');
        $.each(['kur', 'fon', 'fin', 'eko', 'peb'], function (i, a) { alan(a).val(f[a] || ''); });
    }

    // Kod ağaçları (eski DropDownField + TreePanel, nodeLoadPar: 1 kurumsal, 2 fonksiyonel, 3 finans, 4 ekonomik, 14 faaliyet).
    var dlgKod = document.getElementById('dlgKod');
    var agaclar = {};
    var aktifAlan = null;
    function agacAc($alan) {
        var y = kodYil();
        var tur = $alan.attr('data-tur');
        var ad = $alan.find('label').text();
        aktifAlan = $alan;
        $('#dlgKodBaslik').text(ad + ' Seçimi');
        $('#agacKutu > ul').prop('hidden', true);
        var a = agaclar[tur];
        dlgKod.showModal();
        if (!a) {
            a = agaclar[tur] = { $ul: $('<ul></ul>').attr('aria-label', ad + ' ağacı').appendTo('#agacKutu'), yil: y };
            a.agac = Sgb.agac(a.$ul[0], {
                yukle: function (ust) {
                    return Sgb.al(UC + 'kodlar', { yil: a.yil, tur: tur, ust: ust ? ust.kod : '' }).then(function (v) {
                        if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                        return v.dugumler;
                    });
                },
                secilince: function (d) {
                    dlgKod.close();
                    if (aktifAlan) aktifAlan.find('input').val(d.ad).trigger('change');
                },
                seciliMi: function (d) { return !!aktifAlan && kodAl(aktifAlan.find('input').val()) === d.kod; },
                ilkAcik: true,
                bosMetin: 'Kod bulunamadı.',
                hata: hataGoster
            });
        } else {
            a.$ul.prop('hidden', false);
            if (a.yil !== y) { a.yil = y; a.agac.yenile(); }
        }
    }
    $('.kod-alan .kod-sec').on('click', function () { agacAc($(this).closest('.kod-alan')); });
    $('.kod-alan .kod-temizle').on('click', function () { $(this).closest('.kod-alan').find('input').val('').trigger('focus'); });
    $('.kod-alan input').on('keydown', function (e) {
        if (e.key === 'ArrowDown' && e.altKey) { e.preventDefault(); agacAc($(this).closest('.kod-alan')); }
    });
    $('#frmKriter').on('submit', function (e) { e.preventDefault(); });

    // ---------------------------------------------------------------- hesap kodu, sütun türü, satır / sütun tabloları

    var hesap = Sgb.hesapKodu('#txtHesapPlan', {
        yil: kodYil,
        ad: true,
        denetle: true,
        uc: UC + 'hesap/'
    });

    // Eski grdSatir / grdSutun: sıra no, hesap kodu ("kod - ad"), sil.
    function kodTablosu(secici, bos) {
        return Sgb.grid(secici, {
            columnDefs: [
                { headerName: '#', valueGetter: 'node.rowIndex + 1', width: 52, maxWidth: 60, type: 'rightAligned', sortable: false, suppressMovable: true },
                { field: 'HESAPKOD', headerName: 'Hesap Kodu', flex: 1, minWidth: 160, tooltipField: 'HESAPKOD' },
                {
                    headerName: '', colId: 'sil', width: 44, maxWidth: 44, sortable: false, resizable: false, suppressMovable: true,
                    cellRenderer: function (x) {
                        return $('<button type="button" class="btn btn-link btn-xs ct-sil" title="Sil"><span class="glyphicon glyphicon-remove" aria-hidden="true"></span><span class="sr-only">Sil</span></button>')
                            .on('click', function (e) {
                                e.stopPropagation();
                                x.api.applyTransaction({ remove: [x.data] });
                                x.api.refreshCells({ force: true }); // sıra numaraları yeniden yazılsın
                            })[0];
                    }
                }
            ],
            getRowId: function (x) { return x.data.KOD; },
            overlayNoRowsTemplate: '<span>' + bos + '</span>'
        });
    }
    var grdSatir = kodTablosu('#grdSatir', 'Hesap Plan Kod\'a hesabı yazıp Ekle\'ye basınız.');
    var grdSutun = kodTablosu('#grdSutun', 'Hesap Plan Kod\'a hesabı yazıp Ekle\'ye basınız.');

    function satirlar(g) {
        var l = [];
        g.forEachNode(function (n) { if (n.data) l.push({ KOD: n.data.KOD, HESAPKOD: n.data.HESAPKOD }); });
        return l;
    }
    function tabloYaz(g, liste) { g.setGridOption('rowData', $.map(liste || [], function (s) { return { KOD: s.KOD || yeniKod(), HESAPKOD: s.HESAPKOD || '' }; })); }

    function sutunTur() { return parseInt($('#ddlSutunTur').val(), 10) || 0; }
    // Eski SutunTurDegisti: Sütunlar tablosu yalnız Hesap Kod türünde görünür.
    function sutunTurUygula() {
        var hk = sutunTur() === 1;
        $('#pnlSutun').prop('hidden', !hk);
        $('#pnlIzgaralar').toggleClass('ct-tek', !hk);
    }
    $('#ddlSutunTur').on('change', sutunTurUygula);

    // Eski SatirEkle / SutunEkle: Hesap Plan Kod'daki hesap ("kod - ad") tabloda yoksa eklenir, kutu temizlenir.
    function ekle(g) {
        var is = function () {
            var h = hesap ? hesap.deger() : null;
            var kod = h && h.kod ? h.kod : $.trim($('#txtHesapPlan').val());
            if (!kod) { $('#txtHesapPlan').trigger('focus'); return; }
            var deger = h && h.ad ? kod + ' - ' + h.ad : kod;
            var var_ = false;
            g.forEachNode(function (n) { if (n.data && (n.data.HESAPKOD === deger || kodAl(n.data.HESAPKOD) === kod)) var_ = true; });
            if (!var_) g.applyTransaction({ add: [{ KOD: yeniKod(), HESAPKOD: deger }] });
            if (hesap) hesap.temizle(); else $('#txtHesapPlan').val('');
            $('#txtHesapPlan').trigger('focus');
        };
        var bekle = hesap && hesap.hazir ? hesap.hazir() : null;
        if (bekle && bekle.always) bekle.always(is); else is();
    }
    $('#btnSatirEkle').on('click', function () { ekle(grdSatir); });
    $('#btnSutunEkle').on('click', function () { ekle(grdSutun); });
    $('#txtHesapPlan').on('keydown', function (e) {
        if (e.key === 'Enter') { e.preventDefault(); ekle(grdSatir); }
    });

    function acikYaz() {
        $('#lblAcik').text(acik.kod ? 'Açık sorgu: ' + (acik.adi || acik.kod) : '');
    }

    // Eski btnTemizle_Click: açık kayıt, hesap kodu, sorgu alanları (yıl ve durum dışında) ve tablolar temizlenir.
    function temizle() {
        acik = { kod: 0, adi: '' };
        acikYaz();
        if (hesap) hesap.temizle(); else $('#txtHesapPlan').val('');
        filtreYaz({});
        grdSatir.setGridOption('rowData', []);
        grdSutun.setGridOption('rowData', []);
    }
    $('#btnTemizle').on('click', temizle);

    // ---------------------------------------------------------------- Tablo Göster (eski btnYazdir_Click)

    function kodlar(g) { return $.map(satirlar(g), function (s) { return kodAl(s.HESAPKOD) || null; }).join(','); }

    $('#btnTabloGoster').on('click', function () {
        var tur = sutunTur();
        if (!tur) { mesaj('Sütun Tür bilgisi boş bırakılamaz.', 'warning'); $('#ddlSutunTur').trigger('focus'); return; }
        var satir = kodlar(grdSatir);
        var sutun = tur === 1 ? kodlar(grdSutun) : '';
        if (tur === 1 && !sutun) { mesaj('Sütun bilgisi boş bırakılamaz.', 'warning'); return; }
        if (!satir) { mesaj('Satır bilgisi boş bırakılamaz.', 'warning'); return; }
        var $d = $(this).prop('disabled', true);
        Sgb.indir(UC + 'yazdir', $.extend(filtre(), { sutunTur: tur, satir: satir, sutun: sutun }))
            .then(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- kaydet (eski KayitIslem / Kaydet)

    function kaydet(adi, farkli) {
        var veri = {
            kod: farkli ? 0 : acik.kod,
            adi: adi,
            farkliKaydet: !!farkli,
            sutunTur: sutunTur(),
            satirlar: satirlar(grdSatir),
            sutunlar: sutunTur() === 1 ? satirlar(grdSutun) : [],
            filtre: filtre()
        };
        Sgb.gonder(UC + 'kaydet', veri)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                acik = { kod: v.kod || acik.kod, adi: v.adi || adi };
                acikYaz();
                mesaj(v.bilgi, 'success');
                if (dlgListe.open) listele();
            })
            .fail(hataGoster);
    }

    var dlgAd = document.getElementById('dlgAd');
    var adFarkli = false;
    function kayitIslem(farkli) {
        menuKapat();
        if (!satirlar(grdSatir).length && !satirlar(grdSutun).length) { mesaj('Satır ve Sütun bilgileri boş bırakılamaz.', 'warning'); return; }
        if (!acik.kod || farkli) {
            adFarkli = farkli;
            $('#txtSorguAd').val(farkli ? '' : acik.adi);
            dlgAd.showModal();
            $('#txtSorguAd').trigger('focus');
            return;
        }
        // Açık sorgu adıyla güncellenir (eski kod adı boş gönderiyordu).
        kaydet(acik.adi, false);
    }
    $('#frmAd').on('submit', function (e) {
        e.preventDefault();
        var ad = $.trim($('#txtSorguAd').val());
        if (!ad) { mesaj('Sorgu adı boş bırakılamaz.', 'warning'); $('#txtSorguAd').trigger('focus'); return; }
        dlgAd.close();
        kaydet(ad, adFarkli);
    });
    $('#btnKaydet').on('click', function (e) { e.preventDefault(); kayitIslem(false); });
    $('#btnFarkliKaydet').on('click', function (e) { e.preventDefault(); kayitIslem(true); });

    // ---------------------------------------------------------------- kayıtlı sorgular (eski wndListe)

    var dlgListe = document.getElementById('dlgListe');
    var grdListe = Sgb.grid('#grdListe', {
        columnDefs: [
            { headerName: '#', valueGetter: 'node.rowIndex + 1', width: 52, maxWidth: 60, type: 'rightAligned', sortable: false },
            {
                field: 'adi', headerName: 'Adı', flex: 1, minWidth: 180, tooltipField: 'adi',
                cellRenderer: function (x) {
                    if (!x.data) return '';
                    return $('<a href="#"></a>').text(x.value || ('(' + x.data.kod + ')')).attr('title', 'Sorguyu aç').on('click', function (e) {
                        e.preventDefault(); e.stopPropagation();
                        getir(x.data.kod);
                    })[0];
                }
            },
            { field: 'kayitEden', headerName: 'Kayıt Eden', width: 130, minWidth: 100 }
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        getRowId: function (x) { return String(x.data.kod); },
        overlayNoRowsTemplate: '<span>Kayıtlı sorgu yok.</span>'
    });

    function listele() {
        grdListe.setGridOption('loading', true);
        Sgb.al(UC + 'liste')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grdListe.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { grdListe.setGridOption('loading', false); });
    }

    $('#btnKayitliSorgular').on('click', function () { dlgListe.showModal(); listele(); });
    $('#btnListele').on('click', listele);

    // Eski btnSil_Click: seçili kayıtlı sorgu silinir.
    $('#btnSil').on('click', function () {
        var s = grdListe.getSelectedRows()[0];
        if (!s) { mesaj('Silinecek kaydı seçiniz.', 'warning'); return; }
        sor('"' + (s.adi || s.kod) + '" silinsin mi?', 'Sil', function () {
            Sgb.gonder(UC + 'sil', { kod: s.kod })
                .done(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return; }
                    mesaj(v.bilgi, 'success');
                    if (acik.kod === s.kod) { acik = { kod: 0, adi: '' }; acikYaz(); }
                    listele();
                })
                .fail(hataGoster);
        });
    });

    // Eski GetirDetay: önce temizlenir, sonra satırlar, sütun türü, sütunlar ve sorgu alanları yazılır.
    function getir(kod) {
        if (!kod) return;
        Sgb.al(UC + 'getir', { kod: kod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                temizle();
                acik = { kod: v.kod, adi: v.adi || '' };
                acikYaz();
                $('#ddlSutunTur').val(v.sutunTur ? String(v.sutunTur) : '');
                sutunTurUygula();
                tabloYaz(grdSatir, v.satirlar);
                tabloYaz(grdSutun, v.sutunlar);
                filtreYaz(v.filtre);
                dlgListe.close();
            })
            .fail(hataGoster);
    }

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    Sgb.al(UC + 'tanim')
        .done(function (t) {
            if (t.hata) { mesaj(t.hata, 'danger'); return; }
            tanim = t;
            Sgb.yilKutusu('#txtYil', t.yillar, t.yil);
            var $d = $('#ddlDurum').empty();
            $.each(t.durumlar || [], function (i, d) { $('<option></option>').val(d.kod).text(d.ad).appendTo($d); });
            $d.val(String(t.varsayilanDurum || 22));
            var $s = $('#ddlSutunTur').empty().append('<option value="">Seçiniz</option>');
            $.each(t.sutunTurleri || [], function (i, d) { $('<option></option>').val(d.kod).text(d.ad).appendTo($s); });
            sutunTurUygula();
        })
        .fail(hataGoster);

    if (sakliOku('kriterGizli') === '1') kriterGoster(false);
});
