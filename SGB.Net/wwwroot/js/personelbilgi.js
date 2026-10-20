// Personel İşlemleri (eski InsanKaynaklari/PersonelBilgi.aspx).
// Solda personel arama ve listesi; satır seçilince sağda personelin özeti ve yetkili olduğu sekmeler çizilir. Her sekme ayrı bir
// alt sayfadır (InsanKaynaklari/PersonelXxx?menuYok=1&personelKod=..), ilk açıldığında çerçeve (iframe) içinde yüklenir.
// Alt sayfalar window.PersonelBilgi üzerinden bu sayfayla konuşur (eski parent.PersonelX.* çağrıları).
(function ($) {
    'use strict';

    var U = 'api/personelbilgi/';
    var SAYFA_BOYU = 250; // eski PagingToolbar PageSize
    var secilen = null;   // { kod, tcKimlikNo, personelTipleri, sekmeler }
    var birim = { kod: 0, ad: '' };
    var sayfa = { basla: 0, toplam: 0 };

    function hata(metin) { Sgb.bildirim(metin || 'İşlem yapılamadı.', 'danger'); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 403) hata('Bu işlem için yetkiniz yok.');
        else if (xhr && xhr.status !== 401) hata('İşlem yapılamadı.');
    }

    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- çoklu seçim (eski MultiCombo / ExtCokluSecim)

    function cokluSecim(el, degisince) {
        var $el = $(el).addClass('dropdown');
        var $dugme = $('<button type="button" class="btn btn-default btn-sm form-control pbana-coklu-dugme" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"></button>').appendTo($el);
        var $metin = $('<span class="pbana-coklu-metin"></span>').appendTo($dugme);
        $('<span class="caret"></span>').appendTo($dugme);
        var $liste = $('<ul class="dropdown-menu pbana-coklu-liste"></ul>').appendTo($el);
        var ogeler = [];

        function metinYaz() {
            var adlar = $.map(ogeler, function (o) { return o.secili ? o.ad : null; });
            $metin.text(adlar.length === 0 ? ($el.data('bos') || '') : adlar.length > 1 ? '[' + adlar.join(', ') + ']' : adlar[0]);
            $dugme.attr('title', adlar.join(', '));
        }
        function ciz() {
            $liste.empty();
            if (!ogeler.length) $('<li class="pbana-coklu-bos">Seçenek yok</li>').appendTo($liste);
            $.each(ogeler, function (i, o) {
                var $li = $('<li></li>').appendTo($liste);
                var $l = $('<label></label>').appendTo($li);
                $('<input type="checkbox" />').prop('checked', !!o.secili).on('change', function () {
                    o.secili = this.checked;
                    metinYaz();
                    if (degisince) degisince();
                }).appendTo($l);
                $l.append(document.createTextNode(' ' + o.ad));
            });
            metinYaz();
        }
        // Liste içine tıklayınca açılır menü kapanmasın.
        $liste.on('click', function (e) { e.stopPropagation(); });
        $dugme.on('click', function (e) {
            e.stopPropagation();
            var acik = $el.hasClass('open');
            $('.pbana-coklu.open').removeClass('open');
            $el.toggleClass('open', !acik);
            $dugme.attr('aria-expanded', !acik);
        });
        $(document).on('click', function () { $el.removeClass('open'); $dugme.attr('aria-expanded', false); });
        ciz();
        return {
            doldur: function (liste) { ogeler = $.map(liste || [], function (o) { return { kod: o.kod, ad: o.ad, secili: false }; }); ciz(); },
            temizle: function () { $.each(ogeler, function (i, o) { o.secili = false; }); ciz(); },
            secilenler: function (alan) { return $.map(ogeler, function (o) { return o.secili ? o[alan] : null; }); }
        };
    }

    var grup = cokluSecim('#cokGrup', function () { turleriOku(); });
    var tur = cokluSecim('#cokTur');

    // Eski ddlPersonelTipGrup_Changed: seçilen gruplara göre türler yeniden okunur.
    function turleriOku() {
        return Sgb.al(U + 'turler', { gruplar: grup.secilenler('ad').join(';') }).done(function (v) {
            if (v.hata) { hata(v.hata); return; }
            tur.doldur(v.turler);
        }).fail(hataGoster);
    }

    // ---------------------------------------------------------------- açılış

    Sgb.al(U + 'tanim').done(function (v) {
        if (v.hata) hata(v.hata);
        var $u = $('#ddlUnvan').empty();
        $.each(v.unvanlar || [], function (i, u) { $('<option></option>').val(u.kod).text(u.ad).appendTo($u); });
        grup.doldur($.map(v.gruplar || [], function (g) { return { kod: g.ad, ad: g.ad }; }));
        tur.doldur(v.turler);
        if (v.birimKod) birimYaz(v.birimKod, v.birimAd);
        $('#txtAra').trigger('focus');
    }).fail(hataGoster);

    // ---------------------------------------------------------------- birim seçimi

    function birimYaz(kod, ad) {
        birim = { kod: kod || 0, ad: ad || '' };
        $('#txtBirimAd').val(birim.ad).attr('title', birim.ad);
    }

    var dlgBirim = document.getElementById('dlgBirim');
    var birimAgac = null, birimAra = '';
    function birimAgaciKur() {
        birimAgac = Sgb.agac('#agcBirim', {
            yukle: function (ust) {
                return Sgb.al(U + 'orgagac', ust ? { ust: ust.kod } : { ust: 0, ara: birimAra }).then(function (v) {
                    if (v.hata) { hata(v.hata); return []; }
                    return v.satirlar || [];
                });
            },
            secilince: function (d) { birimYaz(d.kod, d.birimAd || d.ad); dlgBirim.close(); },
            ilkAcik: true,
            bosMetin: 'Birim bulunamadı.',
            hata: hataGoster
        });
    }
    $('#btnBirimSec, #txtBirimAd').on('click', function () {
        if (!birimAgac) birimAgaciKur();
        dlgBirim.showModal();
        $('#txtBirimAra').trigger('focus');
    });
    function birimAramaYap() {
        var a = $.trim($('#txtBirimAra').val());
        if (a && a.length < 3) { Sgb.bildirim('Aramak için en az 3 harf yazınız.', 'warning'); return; }
        birimAra = a;
        birimAgac.yenile();
    }
    $('#btnBirimAra').on('click', birimAramaYap);
    $('#txtBirimAra').on('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); birimAramaYap(); } });
    $('#btnBirimSil').on('click', function () { birimYaz(0, ''); });

    // ---------------------------------------------------------------- personel listesi

    function resimVar() { return $('input[name=resim]:checked').val() === '1'; }

    var grid = Sgb.grid('#grdPersonel', {
        columnDefs: [
            {
                field: 'resimBase64', headerName: '', width: 66, sortable: false, hide: true,
                cellRenderer: function (p) {
                    if (!p.value) return '';
                    var img = document.createElement('img');
                    img.className = 'pbana-liste-resim';
                    img.alt = '';
                    img.loading = 'lazy';
                    img.src = '../InsanKaynaklari/' + p.value;
                    return img;
                }
            },
            { field: 'adSoyad', headerName: 'Adı Soyadı', width: 160 },
            { field: 'sicilNo', headerName: 'Sicil No', width: 80 },
            { field: 'unvanAd', headerName: 'Unvan', width: 140 },
            { field: 'birimAd', headerName: 'Birimi', width: 160 },
            { field: 'personelTipAd', headerName: 'Tür', width: 110 },
            { field: 'unvanVekaletAd', headerName: 'Vekalet Unvan', width: 130 }
        ],
        rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
        getRowId: function (p) { return String(p.data.kod); },
        getRowHeight: function () { return resimVar() ? 76 : undefined; },
        overlayNoRowsTemplate: '<span class="pbana-ipucu">Arama ölçütlerini girip Listele\'ye basınız.</span>',
        onRowClicked: function (e) { if (e.data) personelSec(e.data.kod); }
    });

    function kriterler() {
        return {
            ara: $.trim($('#txtAra').val()),
            birim: birim.kod || 0,
            unvan: parseInt($('#ddlUnvan').val(), 10) || 0,
            gruplar: grup.secilenler('ad').join(';'),
            turler: tur.secilenler('kod').join(';'),
            aktif: parseInt($('input[name=aktif]:checked').val(), 10),
            siralama: parseInt($('#ddlSiralama').val(), 10) || 1,
            basla: sayfa.basla,
            sayfa: SAYFA_BOYU
        };
    }

    function listele(basla) {
        sayfa.basla = basla || 0;
        grid.setGridOption('loading', true);
        grid.setColumnsVisible(['resimBase64'], resimVar());
        Sgb.al(U + 'liste', kriterler()).done(function (v) {
            if (v.hata) { hata(v.hata); grid.setGridOption('rowData', []); sayfalamaYaz(0, 0); return; }
            grid.setGridOption('overlayNoRowsTemplate', '<span class="pbana-ipucu">Ölçütlere uyan personel bulunamadı.</span>');
            grid.setGridOption('rowData', v.satirlar || []);
            grid.resetRowHeights();
            sayfalamaYaz(v.toplam || 0, (v.satirlar || []).length);
            if (secilen && secilen.kod) {
                var n = grid.getRowNode(String(secilen.kod));
                if (n) n.setSelected(true);
            }
        }).fail(function (xhr) { hataGoster(xhr); grid.setGridOption('rowData', []); })
            .always(function () { grid.setGridOption('loading', false); });
    }

    function sayfalamaYaz(toplam, gelen) {
        sayfa.toplam = toplam;
        var $s = $('#pbSayfalama');
        if (toplam <= SAYFA_BOYU) {
            $s.prop('hidden', toplam === 0);
            $('#lblSayfa').text(toplam ? toplam + ' kayıt' : '');
            $('#btnOnceki, #btnSonraki').prop('hidden', true);
            return;
        }
        $s.prop('hidden', false);
        $('#btnOnceki, #btnSonraki').prop('hidden', false);
        $('#lblSayfa').text((sayfa.basla + 1) + ' - ' + (sayfa.basla + gelen) + ' / ' + toplam);
        $('#btnOnceki').prop('disabled', sayfa.basla <= 0);
        $('#btnSonraki').prop('disabled', sayfa.basla + SAYFA_BOYU >= toplam);
    }
    $('#btnOnceki').on('click', function () { listele(Math.max(0, sayfa.basla - SAYFA_BOYU)); });
    $('#btnSonraki').on('click', function () { listele(sayfa.basla + SAYFA_BOYU); });

    $('#frmArama').on('submit', function (e) { e.preventDefault(); listele(0); });

    // Eski Temizle(): arama, birim, grup, tür boşalır; Aktif ve Resim Yok seçilir.
    $('#btnTemizle').on('click', function () {
        $('#txtAra').val('');
        birimYaz(0, '');
        $('#ddlUnvan').val('0');
        grup.temizle();
        turleriOku();
        $('input[name=aktif][value=1]').prop('checked', true);
        $('input[name=resim][value=0]').prop('checked', true);
    });

    // Eski Bekleyen İşl.: PersonelKadroOnaysiz penceresi, seçili birimle.
    $('#btnBekleyen').on('click', function () {
        Sgb.pencere('../InsanKaynaklari/PersonelKadroOnaysiz?orgKod=' + encodeURIComponent(birim.kod || 0), 'Bekleyen İşlemler', function () {
            if (secilen && secilen.kod) ozetYenile();
        });
    });

    $('#btnYeniPersonel').on('click', function () { yeniKayitHazirla(); });

    $('#btnSolDaralt').on('click', function () {
        var dar = !$('#pbAna').hasClass('sol-dar');
        $('#pbAna').toggleClass('sol-dar', dar);
        $(this).find('.glyphicon').toggleClass('glyphicon-chevron-left', !dar).toggleClass('glyphicon-chevron-right', dar);
    });

    // ---------------------------------------------------------------- seçilen personel: özet ve sekmeler

    var OZET = ['TipUnvan', 'KadroYeri', 'Iletisim', 'KurumaBaslama', 'GorevYeri', 'VekilUnvan', 'KadroDerece', 'Kazanilmis', 'Emekli'];

    function ozetYaz(v) {
        v = v || {};
        $('#lblBaslik').text(v.kod ? v.baslik : 'Personel Bilgi');
        $('#lblTipUnvan').text(v.tipUnvan || '');
        $('#lblKadroYeri').text(v.kadroYeri || '');
        $('#lblIletisim').text(v.iletisim || '');
        $('#lblKurumaBaslama').text(Sgb.tarih(v.kurumaBaslama || ''));
        $('#lblGorevYeri').text(v.gorevYeri || '');
        $('#lblVekilUnvan').text(v.vekilUnvan || '');
        $('#lblKadroDerece').text(v.kod ? v.kadroDerece : '');
        $('#lblKazanilmis').text(v.kod ? v.kazanilmis : '');
        $('#lblEmekli').text(v.kod ? v.emekli : '');
        $.each(OZET, function (i, a) { var $d = $('#lbl' + a); $d.attr('title', $d.text()); });
        resimYaz(v.kod ? v.resim : '');
    }

    function resimYaz(adres) {
        var $img = $('#imgResim');
        if (!adres) { $img.prop('hidden', true).removeAttr('src'); return; }
        $img.off('error load')
            .on('error', function () { $img.prop('hidden', true); })
            .on('load', function () { $img.prop('hidden', false); })
            .attr('src', '../InsanKaynaklari/' + adres + '&t=' + Date.now());
    }

    function sekmeAdresi(s) {
        var p = { menuYok: 1, personelKod: secilen.kod || 0, personelTur: secilen.personelTipleri || '' };
        if (secilen.tcKimlikNo) p.tcKimlikNo = secilen.tcKimlikNo;
        return '../InsanKaynaklari/' + s.sayfa + '?' + $.param(p);
    }

    function sekmeleriCiz(sekmeler, aktifSayfa) {
        var $b = $('#pbSekmeler').empty();
        var $i = $('#pbSekmeIcerik').empty();
        if (!sekmeler || !sekmeler.length) {
            $('<p class="pbana-bos"></p>').text(secilen && secilen.kod ? 'Bu personel için görebileceğiniz sekme yok.' : 'Yeni personel kaydı için yetkiniz yok.').appendTo($i);
            return;
        }
        $.each(sekmeler, function (i, s) {
            var $li = $('<li role="presentation"></li>').appendTo($b);
            $('<a href="#" role="tab"></a>').attr('id', s.id).attr('aria-controls', s.id + 'Govde').text(s.baslik)
                .data('sekme', s).on('click', function (e) { e.preventDefault(); sekmeAc(s); }).appendTo($li);
            $('<div class="pbana-sekme" role="tabpanel" hidden></div>').attr('id', s.id + 'Govde').appendTo($i);
        });
        var ilk = sekmeler[0];
        if (aktifSayfa) $.each(sekmeler, function (i, s) { if (s.sayfa === aktifSayfa) ilk = s; });
        sekmeAc(ilk);
    }

    // Sekme ilk açıldığında çerçevesi yüklenir (eski AutoLoad); sonra açık kalır.
    function sekmeAc(s) {
        $('#pbSekmeler li').removeClass('active').find('a').attr('aria-selected', 'false');
        $('#' + s.id).attr('aria-selected', 'true').parent().addClass('active');
        $('#pbSekmeIcerik .pbana-sekme').prop('hidden', true);
        var $g = $('#' + s.id + 'Govde').prop('hidden', false);
        if (!$g.children('iframe').length) {
            $('<iframe class="pbana-cerceve"></iframe>').attr('title', s.baslik).attr('src', sekmeAdresi(s)).appendTo($g);
        }
    }

    function aktifSekmeSayfasi() {
        var $a = $('#pbSekmeler li.active a');
        var s = $a.data('sekme');
        return s ? s.sayfa : null;
    }

    function personelSec(kod, aktifSayfa) {
        kod = parseInt(kod, 10) || 0;
        return Sgb.al(U + 'personel', { kod: kod }).done(function (v) {
            if (v.hata) { hata(v.hata); return; }
            secilen = { kod: v.kod || 0, tcKimlikNo: v.tcKimlikNo || '', personelTipleri: v.personelTipleri || '', sekmeler: v.sekmeler || [] };
            ozetYaz(v);
            sekmeleriCiz(secilen.sekmeler, aktifSayfa);
        }).fail(hataGoster);
    }

    // Eski YeniKayitHazirla: özet boşalır, yalnız boş Kişisel sekmesi.
    function yeniKayitHazirla() {
        grid.deselectAll();
        return personelSec(0);
    }

    // Yalnızca üstteki özeti yeniler (sekmeler yerinde kalır).
    function ozetYenile() {
        if (!secilen || !secilen.kod) return $.Deferred().resolve().promise();
        return Sgb.al(U + 'personel', { kod: secilen.kod }).done(function (v) {
            if (v.hata) { hata(v.hata); return; }
            ozetYaz(v);
        }).fail(hataGoster);
    }

    // Alt sayfaların kullandığı nesne (eski parent.PersonelX.KayittanSonraGoster / YeniKayitHazirla, parent.imgPersonelResim).
    window.PersonelBilgi = {
        kayittanSonraGoster: function (kod) { return personelSec(kod, aktifSekmeSayfasi()); },
        yeniKayitHazirla: yeniKayitHazirla,
        resimYenile: function () { if (secilen && secilen.kod) ozetYenile(); },
        ozetYenile: ozetYenile,
        listeyiYenile: function () { if (sayfa.toplam) listele(sayfa.basla); },
        secilen: function () { return secilen ? $.extend({}, secilen) : null; }
    };

    // Açılışta eski sayfadaki gibi boş Kişisel sekmesi (yeni personel girişi).
    personelSec(0);
})(jQuery);
