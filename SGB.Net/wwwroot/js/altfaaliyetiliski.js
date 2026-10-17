// Alt Faaliyet İlişkileri (ButceHazirlikv2/AltFaaliyetIliski; eski AltFaaliyetIliski.aspx). ?tur=1 faaliyet bağı, ?tur=2 DPT proje bağı.
// Listele kod / ad maskesine uyan alt faaliyetleri getirir. İlişkili faaliyet (tur 1) ya da DPT projesi (tur 2) hücredeki büyüteçle
// açılan pencereden seçilir; Kaydet yalnızca değişen satırları gönderir (eski DegisenleriTopla). Satırdaki düğmeler gider / gelir
// tertiplerini (TertipTanimFormu, yeni sekmede) ve birimleri (kurumsal yapı şablonları, pencerede) açar.
$(function () {
    'use strict';

    var $sayfa = $('.alt-faal');
    if (!$sayfa.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var TUR = parseInt($sayfa.attr('data-tur'), 10) === 2 ? 2 : 1;
    var KOD_ALAN = TUR === 2 ? 'projeKod' : 'faalKod';
    var AD_ALAN = TUR === 2 ? 'projeAd' : 'faalAd';

    var okunan = null; // { yil, senaryo }: listelenen yıl ve aşama (kaydetme ve pencereler bunu kullanır)

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function kirliler() {
        var r = [];
        grid.forEachNode(function (n) { if (n.data.kirli) r.push(n.data); });
        return r;
    }
    function mesgul(evet) {
        $('#btnListele').prop('disabled', evet);
        $('#btnKaydet').prop('disabled', evet || !kirliler().length);
    }

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- tablo

    function simgeDugme(simge, metin, is, baslik) {
        return $('<button type="button" class="btn btn-default btn-xs"></button>').attr('title', baslik || metin)
            .append($('<span class="glyphicon" aria-hidden="true"></span>').addClass(simge))
            .append(metin ? document.createTextNode(' ' + metin) : $('<span class="sr-only"></span>').text(baslik))
            .on('click', function (e) { e.stopPropagation(); is(); });
    }

    // İlişki hücresi: adı ve yanında seçim büyüteci (eski ImageCommand faalSec / projeSec)
    function iliskiHucre(p) {
        var $h = $('<span class="iliski-hucre"></span>');
        simgeDugme('glyphicon-search', '', function () { secimAc(p.node); }, TUR === 2 ? 'DPT projesi seç' : 'Faaliyet seç').appendTo($h);
        $('<span class="iliski-ad"></span>').text(p.value || '').attr('title', p.value || '').appendTo($h);
        return $h[0];
    }

    // Komutlar (eski ImageCommandColumn: giderAc, gelirAc, birimAc)
    function komutHucre(p) {
        var $h = $('<span class="komut-hucre"></span>');
        simgeDugme('glyphicon-usd', 'Gid. Tertipleri', function () { tertipAc(1, p.data); }, 'Gider tertipleri').appendTo($h);
        simgeDugme('glyphicon-plus-sign', 'Gel. Tertipleri', function () { tertipAc(2, p.data); }, 'Gelir tertipleri').appendTo($h);
        simgeDugme('glyphicon-map-marker', 'Birimler', function () { birimAc(p.data); }, 'Birimler (kurumsal yapı şablonları)').appendTo($h);
        return $h[0];
    }

    var grid = Sgb.grid('#grdAltFaaliyet', {
        columnDefs: [
            { field: 'kod', headerName: 'Kod', width: 140, tooltipField: 'kod' },
            { field: 'ad', headerName: 'Ad', flex: 2, minWidth: 260, tooltipField: 'ad' },
            { field: AD_ALAN, headerName: TUR === 2 ? 'İlişkili DPT projesi' : 'İlişkili Faaliyet', flex: 1, minWidth: 240,
              cellRenderer: iliskiHucre, cellClass: function (p) { return p.data && p.data.kirli ? 'degisti' : ''; } },
            { colId: 'komutlar', headerName: '', width: 330, minWidth: 300, sortable: false, cellRenderer: komutHucre }
        ],
        getRowId: function (p) { return String(p.data.kod); },
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        overlayNoRowsTemplate: '<span class="ag-overlay-no-rows-center">Yılı seçip Listele\'ye basınız.</span>'
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/altfaaliyetiliski/tanim', { tur: TUR }).done(function (v) {
        if (v.hata) { mesaj(v.hata, 'danger'); return; }
        $('#txtYil').val(v.yil);
        var $a = $('#ddlAsama').empty();
        $.each(v.asamalar || [], function (i, o) { $('<option></option>').val(o.kod).text(o.ad).appendTo($a); });
        if (v.senaryo) $a.val(v.senaryo);
    }).fail(hataGoster);

    // ---------------------------------------------------------------- Listele / Kaydet

    function listele() {
        var yil = parseInt($('#txtYil').val(), 10) || 0;
        if (yil < 2000) { mesaj('Yılı yazınız.', 'warning'); $('#txtYil').trigger('focus'); return; }
        if (kirliler().length && !window.confirm('Kaydedilmemiş değişiklikler var. Devam ederseniz kaybolacak. Devam edilsin mi?')) return;
        var o = { tur: TUR, yil: yil, senaryo: TUR === 2 ? ($('#ddlAsama').val() || '') : '', kod: $.trim($('#txtKod').val()), ad: $.trim($('#txtAd').val()) };
        mesgul(true);
        grid.setGridOption('loading', true);
        Sgb.al('api/altfaaliyetiliski/liste', o)
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                var liste = v.satirlar || [];
                grid.setGridOption('rowData', liste);
                okunan = { yil: yil, senaryo: o.senaryo };
                $('#lblOkunan').text(yil + (TUR === 2 ? ' yılı, ' + $('#ddlAsama option:selected').text() + ' aşaması' : ' yılı') + ': ' + liste.length + ' alt faaliyet');
                if (!liste.length) mesaj('Verilen kriterlere uygun kayıt bulunamadı.', 'info');
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); mesgul(false); });
    }

    $('#btnListele').on('click', listele);
    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(); });

    $('#btnKaydet').on('click', function () {
        var degisen = kirliler();
        if (!degisen.length) { mesaj('İşlem yapılacak kayıt bulunamadı!', 'warning'); return; }
        if (!window.confirm('Değişiklikleri kaydetmek istiyor musunuz?')) return;
        var istek = {
            tur: TUR, senaryo: okunan ? okunan.senaryo : '',
            satirlar: $.map(degisen, function (d) { return { yil: d.yil, pebKod: d.kod, faalKod: d.faalKod || '', projeKod: d.projeKod || '' }; })
        };
        mesgul(true);
        Sgb.gonder('api/altfaaliyetiliski/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                // Eski KirlileriKaldir
                $.each(degisen, function (i, d) { d.kirli = false; });
                grid.refreshCells({ force: true });
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // ---------------------------------------------------------------- faaliyet / DPT projesi seçimi

    var dlgSec = document.getElementById('dlgSec');
    var secDugum = null;   // seçimin yazılacağı satır
    var secListe = [];     // pencerede listelenen seçenekler
    var secAnahtar = '';   // seçenekler hangi yıl / aşama için okundu

    var secAgac = Sgb.agac(document.getElementById('agcSec'), {
        duz: true,
        bosMetin: 'Seçenek yok.',
        yukle: function () {
            var ara = $.trim($('#txtSecAra').val()).toLocaleLowerCase('tr');
            return $.Deferred().resolve($.grep(secListe, function (o) { return !ara || o.ad.toLocaleLowerCase('tr').indexOf(ara) >= 0; })).promise();
        },
        seciliMi: function (d) { return secDugum && secDugum.data[KOD_ALAN] === d.kod; },
        secilince: function (d) { iliskiYaz(d.kod, d.ad); dlgSec.close(); }
    });

    function iliskiYaz(kod, ad) {
        if (!secDugum) return;
        var d = secDugum.data;
        if ((d[KOD_ALAN] || '') === kod) return;
        d[KOD_ALAN] = kod;
        d[AD_ALAN] = ad;
        d.kirli = true;
        grid.refreshCells({ rowNodes: [secDugum], force: true });
        mesgul(false);
    }

    function secimAc(dugum) {
        if (!okunan) return;
        secDugum = dugum;
        var anahtar = okunan.yil + '|' + okunan.senaryo;
        $('#txtSecAra').val('');
        dlgSec.showModal();
        if (anahtar === secAnahtar) { secAgac.yenile(); return; }
        secListe = [];
        secAgac.temizle();
        var adres = TUR === 2 ? 'api/altfaaliyetiliski/altprojeler' : 'api/altfaaliyetiliski/faaliyetler';
        Sgb.al(adres, { tur: TUR, yil: okunan.yil, senaryo: okunan.senaryo })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                secListe = v.dugumler || [];
                secAnahtar = anahtar;
                secAgac.yenile();
            })
            .fail(hataGoster);
    }

    $('#txtSecAra').on('input', function () { secAgac.yenile(); });
    $('#btnSecKaldir').on('click', function () { iliskiYaz('', ''); dlgSec.close(); });

    // ---------------------------------------------------------------- Gider / Gelir tertipleri (eski giderAc / gelirAc)

    function tertipAc(tur, d) {
        window.open('../ButceHazirlikv2/TertipTanimFormu?menuYok=1&Tur=' + tur + '&yil=' + encodeURIComponent(d.yil) +
            '&pebKod=' + encodeURIComponent(d.kod), '_blank');
    }

    // ---------------------------------------------------------------- Birimler (eski ListeKurumsalYapiParametreForm)

    var dlgBirim = document.getElementById('dlgBirim');
    var birimSatir = null;
    var birimAgac = null;
    var birimAgacYil = 0;
    var isaretli = {};

    function sablonEkle(kod, ad) {
        var $l = $('#lstSablon');
        if ($l.children().filter(function () { return this.value === kod; }).length) return false;
        $('<option></option>').val(kod).text(ad && ad !== kod ? kod + ' - ' + ad : kod).appendTo($l);
        return true;
    }

    function birimAc(d) {
        birimSatir = d;
        $('#lblBirimPeb').text(d.kod + ' - ' + (d.ad || ''));
        $('#lstSablon').empty();
        $('#txtMT, #txtSablonIl, #txtSablonIlce, #txtSablonBirim').val('');
        $('#kutuBirimAgac').prop('hidden', true);
        $('#btnBirimKaydet').prop('disabled', true);
        dlgBirim.showModal();
        Sgb.al('api/altfaaliyetiliski/birim', { tur: TUR, yil: d.yil, peb: d.kod })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                $.each(v.sablonlar || [], function (i, s) { sablonEkle(s); });
                $('#btnBirimKaydet').prop('disabled', false);
            })
            .fail(hataGoster);
    }

    // Eski sablonEkle: MT.İl.İlçe.Birim (boş parçalar da noktayla yazılır)
    $('#frmSablon').on('submit', function (e) {
        e.preventDefault();
        var sablon = [$('#txtMT').val(), $('#txtSablonIl').val(), $('#txtSablonIlce').val(), $('#txtSablonBirim').val()]
            .map(function (s) { return $.trim(s); }).join('.');
        if (sablon === '...') { mesaj('Şablonun en az bir parçasını yazınız.', 'warning'); return; }
        if (!sablonEkle(sablon)) mesaj('Bu şablon listede var.', 'info');
    });

    $('#btnSablonCikart').on('click', function () { $('#lstSablon option:selected').remove(); });

    // Eski BirimSec: KodAgac.aspx?kodTur=17&coklu=1. İşaretlenen birimler listeye eklenir; son seçilenin parçaları alanlara yazılır.
    $('#btnBirimSec').on('click', function () {
        var yil = birimSatir ? birimSatir.yil : 0;
        $('#kutuBirimAgac').prop('hidden', false);
        if (birimAgac && birimAgacYil === yil) return;
        birimAgacYil = yil;
        isaretli = {};
        if (birimAgac) { birimAgac.yenile(); return; }
        birimAgac = Sgb.agac(document.getElementById('agcBirim'), {
            ilkAcik: true,
            bosMetin: 'Kod yok.',
            yukle: function (ust) {
                return Sgb.al('api/altfaaliyetiliski/birimkodlar', { tur: TUR, yil: birimAgacYil, ust: ust ? ust.kod : '' }).then(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                    return v.dugumler || [];
                });
            },
            isaretli: function (d) { return !!isaretli[d.kod]; },
            isaretlenince: function (d, evet) { if (evet) isaretli[d.kod] = d; else delete isaretli[d.kod]; },
            hata: hataGoster
        });
    });

    $('#btnBirimEkle').on('click', function () {
        var sayi = 0, son = null;
        $.each(isaretli, function (kod, d) {
            var ad = String(d.ad || '').replace(new RegExp('^' + kod.replace(/\./g, '\\.') + '- '), '');
            if (sablonEkle(kod, ad)) sayi++;
            son = kod;
        });
        if (!son) { mesaj('Ağaçtan birim işaretleyiniz.', 'warning'); return; }
        var p = son.split('.');
        $('#txtMT').val(p[0] || ''); $('#txtSablonIl').val(p[1] || ''); $('#txtSablonIlce').val(p[2] || ''); $('#txtSablonBirim').val(p[3] || '');
        isaretli = {};
        birimAgac.isaretleriGuncelle();
        mesaj(sayi + ' birim listeye eklendi.', 'info');
    });

    $('#btnBirimKaydet').on('click', function () {
        if (!birimSatir) return;
        var sablonlar = $('#lstSablon option').map(function () { return this.value; }).get();
        $('#btnBirimKaydet').prop('disabled', true);
        Sgb.gonder('api/altfaaliyetiliski/birim', { tur: TUR, yil: birimSatir.yil, pebKod: birimSatir.kod, sablonlar: sablonlar })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $('#btnBirimKaydet').prop('disabled', false); });
    });
});
