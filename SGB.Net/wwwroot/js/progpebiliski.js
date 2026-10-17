// Alt Faaliyet Bağları (ButceHazirlikv2/ProgPebIliski): performans bütçe alt faaliyet kodlarının program bütçe kodlarıyla ilişkisi.
// Listele yılın bağlarını filtreye göre getirir; Listele (Fon) yalnızca fonksiyonel kodu uyuşmayanları. Satıra tıklanınca açılan
// pencerede alt faaliyet ve fonksiyonel kod yazılır ya da ağaçtan seçilir (eski KodAgac.aspx), program / alt program / faaliyet /
// alt faaliyet en az 2 karakter yazılarak aranır. Yazdır Excel indirir, Kopyala listeyi panoya (sekmeyle ayrılmış) kopyalar,
// Önc. Yıldan Kopyala bir önceki yılın bağlarını seçilen yıla yazar.
$(function () {
    'use strict';

    if (!$('.prog-peb-iliski').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    function mesaj(metin, t) { Sgb.bildirim(metin, t); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }

    function yilOku() { return parseInt($('#txtYil').val(), 10) || 0; }
    function kodAd(kod, ad) { return kod > 0 ? kod + ' - ' + (ad || '') : ''; }

    $('#txtYil').val(new Date().getFullYear());

    // ---------------------------------------------------------------- liste

    // Kopyala ve Yazdır'daki sütun sırası (eski grid sırası).
    var ALANLAR = ['pebKod', 'pebAd', 'altFaalKod', 'altFaalAdi', 'prgKod', 'prgAdi', 'altPrgKod', 'altPrgAdi', 'faalKod', 'faalAdi', 'faalFonKod', 'fonKod'];
    function sifirsiz(p) { return p.value ? p.value : ''; }

    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            { field: 'pebKod', headerName: 'Kodu', width: 110 },
            { field: 'pebAd', headerName: 'Adı', flex: 2, minWidth: 220, tooltipField: 'pebAd' },
            { field: 'altFaalKod', headerName: 'Kod', width: 80, valueFormatter: sifirsiz },
            { field: 'altFaalAdi', headerName: 'Alt Faal.', flex: 2, minWidth: 220, tooltipField: 'altFaalAdi' },
            { field: 'prgKod', headerName: 'Kod', width: 80, hide: true, valueFormatter: sifirsiz },
            { field: 'prgAdi', headerName: 'Program', flex: 1, minWidth: 160, tooltipField: 'prgAdi' },
            { field: 'altPrgKod', headerName: 'Kod', width: 80, hide: true, valueFormatter: sifirsiz },
            { field: 'altPrgAdi', headerName: 'Alt Program', flex: 1, minWidth: 160, tooltipField: 'altPrgAdi' },
            { field: 'faalKod', headerName: 'Kod', width: 80, hide: true, valueFormatter: sifirsiz },
            { field: 'faalAdi', headerName: 'Faaliyet', flex: 1, minWidth: 160, tooltipField: 'faalAdi' },
            { field: 'faalFonKod', headerName: 'Faal. Fon.', width: 100 },
            { field: 'fonKod', headerName: 'Fon. Kod', width: 100 }
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        overlayNoRowsTemplate: '<span>Yılı seçip Listele\'ye basınız.</span>',
        onRowClicked: function (e) { detayAc(e.data); }
    });

    var sonListe = { fon: false };

    function listele(fon) {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        sonListe.fon = !!fon;
        var $d = $('#btnListele, #btnListeleFon').prop('disabled', true);
        grid.setGridOption('loading', true);
        Sgb.al('api/progpebiliski/liste', { yil: yil, ara: $.trim($('#txtFiltre').val()), fon: !!fon })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                grid.setGridOption('rowData', v.satirlar || []);
                grid.setGridOption('overlayNoRowsTemplate', '<span>Verilen kriterlere uygun kayıt bulunamadı.</span>');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); grid.setGridOption('loading', false); });
    }

    $('#frmSecim').on('submit', function (e) { e.preventDefault(); listele(false); });
    $('#btnListeleFon').on('click', function () { listele(true); });

    $('#btnYazdir').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        Sgb.indir('api/progpebiliski/rapor', { yil: yil, ara: $.trim($('#txtFiltre').val()) });
    });

    // Kopyala (eski CopyStore): listedeki satırlar sekmeyle ayrılmış olarak panoya.
    $('#btnKopyala').on('click', function () {
        var liste = [];
        grid.forEachNodeAfterFilterAndSort(function (n) { liste.push(n.data); });
        if (!liste.length) { mesaj('Kopyalanacak satır yok.', 'warning'); return; }
        var metin = $.map(liste, function (s) {
            return $.map(ALANLAR, function (a) { return String(s[a] == null ? '' : s[a]).replace(/[\t\r\n]+/g, ' '); }).join('\t') + '\r\n';
        }).join('');
        var tamam = function () { mesaj(liste.length + ' satır panoya kopyalandı.', 'success'); };
        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(metin).then(tamam, function () { mesaj('Panoya kopyalanamadı.', 'danger'); });
        } else {
            var $t = $('<textarea readonly></textarea>').val(metin).css({ position: 'fixed', left: '-9999px' }).appendTo(document.body);
            $t[0].select();
            try { document.execCommand('copy'); tamam(); } catch (e) { mesaj('Panoya kopyalanamadı.', 'danger'); }
            $t.remove();
        }
    });

    $('#btnOncekiYil').on('click', function () {
        var yil = yilOku();
        if (yil < 2000) { mesaj('Yıl hatalı!', 'warning'); return; }
        if (!window.confirm('Önceki yıldan kopyalamak istiyor musunuz?')) return;
        var $d = $('#btnOncekiYil').prop('disabled', true);
        Sgb.gonder('api/progpebiliski/kopyala', { yil: yil })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- ayrıntı penceresi (eski wndDetay / KomutCalistir)

    var dlgDetay = document.getElementById('dlgDetay');
    var detayYil = 0;

    function detayAc(d) {
        detayYil = d.yil || yilOku();
        $('#txtPeb').val(d.pebKod || '');
        $('#lblPeb').text(d.pebAd || '');
        $('#txtProgram').val(kodAd(d.prgKod, d.prgAdi));
        $('#txtAltProgram').val(kodAd(d.altPrgKod, d.altPrgAdi));
        $('#txtFaaliyet').val(kodAd(d.faalKod, d.faalAdi));
        $('#txtAltFaaliyet').val(kodAd(d.altFaalKod, d.altFaalAdi));
        // Eski sayfa gibi: bağın kendi fonksiyonel kodu yoksa faaliyetinki gelir.
        $('#txtFon').val(d.fonKod || d.faalFonKod || '');
        $('#lblFon').text('');
        $('#dlgDetayBaslik').text('Detay - ' + detayYil);
        dlgDetay.showModal();
        adGetir('#txtFon', '#lblFon', 2);
    }

    // Kod kutusundan çıkınca kodun adı yazılır (eski KodAdGetir).
    function adGetir(kutu, etiket, tur) {
        var kod = $.trim($(kutu).val());
        if (!kod) { $(etiket).text(''); return; }
        Sgb.al('api/progpebiliski/kodad', { tur: tur, yil: detayYil, kod: kod })
            .done(function (v) { if ($.trim($(kutu).val()) === kod) $(etiket).text(v.ad || ''); });
    }
    $('#txtPeb').on('change', function () { adGetir('#txtPeb', '#lblPeb', 14); });
    $('#txtFon').on('change', function () { adGetir('#txtFon', '#lblFon', 2); });

    // Program bütçe kodu arama (eski ComboBox MinChars=2): yazdıkça datalist doldurulur; değer "kod - ad".
    var aramaZaman = {};
    $('.prg-kod').on('input', function () {
        var $k = $(this), tur = $k.data('tur'), ara = $.trim($k.val());
        clearTimeout(aramaZaman[tur]);
        if (ara.length < 2 || /^\d+ - /.test(ara)) return;
        aramaZaman[tur] = setTimeout(function () {
            Sgb.al('api/progpebiliski/prgkod', { tur: tur, yil: detayYil, ara: ara })
                .done(function (v) {
                    if (v.hata) return;
                    var $l = $('#' + $k.attr('list')).empty();
                    $.each(v.kodlar || [], function (i, x) { $('<option></option>').attr('value', x.kod + ' - ' + x.ad).appendTo($l); });
                });
        }, 250);
    });

    function prgKodOku(secici) {
        var m = /^\s*(\d+)/.exec($(secici).val() || '');
        return m ? parseInt(m[1], 10) : 0;
    }

    $('#btnKaydet').on('click', function () {
        var istek = {
            yil: detayYil,
            pebKod: $.trim($('#txtPeb').val()),
            prgKod: prgKodOku('#txtProgram'),
            altPrgKod: prgKodOku('#txtAltProgram'),
            faalKod: prgKodOku('#txtFaaliyet'),
            altFaalKod: prgKodOku('#txtAltFaaliyet'),
            fonKod: $.trim($('#txtFon').val())
        };
        if (!istek.pebKod) { mesaj('Alt faaliyet kodunu yazınız.', 'warning'); $('#txtPeb').trigger('focus'); return; }
        var $d = $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/progpebiliski/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                dlgDetay.close();
                listele(sonListe.fon);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnSil').on('click', function () {
        var pebKod = $.trim($('#txtPeb').val());
        if (!pebKod) return;
        if (!window.confirm('Kaydı silmek istiyor musunuz?')) return;
        var $d = $('#btnSil').prop('disabled', true);
        Sgb.gonder('api/progpebiliski/sil', { yil: detayYil, pebKod: pebKod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                dlgDetay.close();
                mesaj(s.bilgi, 'success');
                listele(sonListe.fon);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- kod ağacı (eski KodAgac.aspx)

    var dlgAgac = document.getElementById('dlgAgac');
    var agacTur = 0, agacKutu = null, agacEtiket = null;

    var agac = Sgb.agac(document.getElementById('agcKod'), {
        ilkAcik: true,
        bosMetin: 'Kod yok.',
        yukle: function (ust) {
            return Sgb.al('api/progpebiliski/kodagac', { tur: agacTur, yil: detayYil, ust: ust ? ust.kod : '' })
                .then(function (v) {
                    if (v.hata) { mesaj(v.hata, 'danger'); return []; }
                    return v.dugumler || [];
                });
        },
        seciliMi: function (d) { return agacKutu && $.trim($(agacKutu).val()) === d.kod; },
        secilince: function (d) {
            $(agacKutu).val(d.kod);
            $(agacEtiket).text(d.adYalin || '');
            dlgAgac.close();
        },
        hata: hataGoster
    });

    $('[data-agac]').on('click', function () {
        agacTur = parseInt($(this).data('agac'), 10);
        agacKutu = agacTur === 14 ? '#txtPeb' : '#txtFon';
        agacEtiket = agacTur === 14 ? '#lblPeb' : '#lblFon';
        $('#dlgAgacBaslik').text(agacTur === 14 ? 'Alt Faaliyet Kodu Seçimi' : 'Fonksiyonel Kod Seçimi');
        dlgAgac.showModal();
        agac.yenile();
    });

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });
});
