// Genel Görev Tanımlama (Organizasyon/GenelGorevAna): solda görevler, sağda seçili görevin bilgileri ve yazılım yetkileri.
// Yazılım yetkisi pencereleri (eski YETxxx.aspx sayfaları) sunucudan gelen alan tanımlarından çizilir.
$(function () {
    'use strict';

    if (!$('#grdGorevler').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var HATA = 'İşlem sırasında hata oluştu.';
    var SIL_ONAY = 'Seçili satırı silmek istediğinizden emin misiniz?';
    var gorev = null;   // seçili görev: { kod, islemler, yetkiler }
    var acikYetki = null; // penceredeki yetki: { indis, siraNo, alanlar }

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(HATA, 'danger');
    }
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    // ---------------------------------------------------------------- görev listesi

    var grid = Sgb.grid('#grdGorevler', {
        columnDefs: [{ field: 'ad', headerName: 'Görev Adı', flex: 1, minWidth: 180, tooltipField: 'ad' }],
        rowSelection: 'single',
        getRowId: function (p) { return p.data.kod; },
        onRowClicked: function (e) { gorevAc(e.data.kod); }
    });

    function listeGoster(satirlar, secilecek) {
        grid.setGridOption('rowData', satirlar);
        var kod = secilecek || (satirlar.length ? satirlar[0].kod : null);
        if (!kod) return;
        var n = grid.getRowNode(kod);
        if (n) { n.setSelected(true); grid.ensureNodeVisible(n); }
        gorevAc(kod);
    }

    $('#btnYeni').on('click', function () {
        $('#btnYeni').prop('disabled', true);
        Sgb.gonder('api/genelgorev/yeni')
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                listeGoster(s.satirlar, s.kod);
                $('#txtAd').trigger('focus');
            })
            .fail(hataGoster)
            .always(function () { $('#btnYeni').prop('disabled', false); });
    });

    // ---------------------------------------------------------------- görev bilgileri

    function gorevAc(kod) {
        return Sgb.al('api/genelgorev/oku', { kod: kod })
            .done(function (g) {
                gorev = g;
                $('#txtAd').val(g.ad || '');
                $('#txtAciklama').val(g.aciklama || '');
                $('#txtMaxGorev').val(g.maxGorev);
                $('#chkZorunlu').prop('checked', !!g.zorunlu);

                // Birim türleri yalnızca genel görevlerde seçilir.
                var $turler = $('#birimTurleri').empty();
                $('#satirBirimTurleri').prop('hidden', !g.birimTurleri);
                $.each(g.birimTurleri || [], function (i, t) {
                    var $e = $('<label class="checkbox-inline"></label>').appendTo($turler);
                    $('<input type="checkbox" />').val(t.kod).prop('checked', t.secili).appendTo($e);
                    $e.append(document.createTextNode(' ' + t.ad));
                });

                var $islem = $('#ddlIslem');
                $islem.children().slice(1).remove();
                $.each(g.islemler, function (i, s) { $('<option></option>').val(s.indis).text(s.ad).appendTo($islem); });
                $islem.val('');

                yetkileriCiz();
                $('#btnKaydet, #btnYetkiEkle').prop('disabled', false);
            })
            .fail(hataGoster);
    }

    $('#btnKaydet').on('click', function () {
        if (!gorev) return;
        var istek = {
            kod: gorev.kod,
            ad: $.trim($('#txtAd').val()),
            aciklama: $('#txtAciklama').val(),
            maxGorev: parseInt($('#txtMaxGorev').val(), 10),
            zorunlu: $('#chkZorunlu').prop('checked'),
            birimler: $('#birimTurleri input:checked').map(function () { return parseInt(this.value, 10); }).get()
        };
        if (isNaN(istek.maxGorev)) istek.maxGorev = -1;
        $('#btnKaydet').prop('disabled', true);
        Sgb.gonder('api/genelgorev/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                // Görevin adı listede de güncellenir.
                var n = grid.getRowNode(gorev.kod);
                if (n) n.setData({ kod: gorev.kod, ad: s.ad });
            })
            .fail(hataGoster)
            .always(function () { $('#btnKaydet').prop('disabled', false); });
    });

    // ---------------------------------------------------------------- yazılım yetkileri

    function dugme(simge, metin, sinif) {
        var $d = $('<button type="button" class="btn btn-xs"></button>').addClass(sinif).attr('title', metin).attr('aria-label', metin);
        $('<span class="glyphicon" aria-hidden="true"></span>').addClass('glyphicon-' + simge).appendTo($d);
        return $d;
    }

    function yetkileriCiz() {
        var $liste = $('#lstYetkiler').empty();
        if (!gorev.yetkiler.length) {
            $('<li></li>').append($('<span class="ad bos"></span>').text('Bu göreve henüz yazılım yetkisi eklenmedi.')).appendTo($liste);
            return;
        }
        $.each(gorev.yetkiler, function (i, y) {
            var $li = $('<li></li>').appendTo($liste);
            var $ad = $('<span class="ad"></span>').appendTo($li);
            $('<strong></strong>').text(y.ad || ('#' + y.indis)).appendTo($ad);
            if (y.aciklama) $ad.append(document.createTextNode(': ' + y.aciklama));
            dugme('pencil', 'Düzenle', 'btn-default').on('click', function () { yetkiAc(y.indis, y.siraNo); }).appendTo($li);
            dugme('trash', 'Sil', 'btn-danger').on('click', function () { yetkiSil(y.siraNo); }).appendTo($li);
        });
    }

    function yetkileriYenile() {
        return Sgb.al('api/genelgorev/oku', { kod: gorev.kod })
            .done(function (g) { gorev.yetkiler = g.yetkiler; yetkileriCiz(); })
            .fail(hataGoster);
    }

    function yetkiSil(siraNo) {
        if (!window.confirm(SIL_ONAY)) return;
        Sgb.gonder('api/genelgorev/yetki-sil', { kod: gorev.kod, siraNo: siraNo })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                yetkileriYenile();
            })
            .fail(hataGoster);
    }

    $('#btnYetkiEkle').on('click', function () {
        var indis = parseInt($('#ddlIslem').val(), 10);
        if (isNaN(indis)) { mesaj('Eklenecek işlemi seçiniz.', 'warning'); $('#ddlIslem').trigger('focus'); return; }
        yetkiAc(indis, 0);
    });

    // ---------------------------------------------------------------- yetki penceresi

    function alanKimlik(i) { return 'ya' + i; }

    function yetkiAc(indis, siraNo) {
        Sgb.al('api/genelgorev/yetki', { kod: gorev.kod, indis: indis, siraNo: siraNo })
            .done(function (y) {
                if (y.hata) { mesaj(y.hata, 'danger'); return; }
                acikYetki = y;
                $('#dlgYetkiBaslik').text(y.baslik);
                $('#txtYetkiAciklama').val(y.aciklama || '');
                $('#yetkiAlanYok').prop('hidden', y.alanlar.length > 0);
                $('#yetkiUyari').text(y.uyari || '').prop('hidden', !y.uyari);

                var $govde = $('#yetkiAlanlar').empty();
                var gruplar = {}; // grup adı -> onay kutularının satırı
                $.each(y.alanlar, function (i, a) {
                    var kimlik = alanKimlik(i);
                    if (a.tur === 'Evet' || a.tur === 'EvetSayi') {
                        var grup = a.grup || a.etiket;
                        if (!gruplar[grup]) {
                            var $s = $('<div class="form-group"></div>').appendTo($govde);
                            $('<span class="control-label"></span>').text(grup).appendTo($s);
                            gruplar[grup] = $('<div role="group"></div>').attr('aria-label', grup).appendTo($s);
                        }
                        var $e = $('<label class="checkbox-inline"></label>').appendTo(gruplar[grup]);
                        $('<input type="checkbox" />').attr('id', kimlik).prop('checked', a.deger === 'true').appendTo($e);
                        $e.append(document.createTextNode(' ' + a.etiket));
                        return;
                    }

                    var $satir = $('<div class="form-group"></div>').appendTo($govde);
                    if (a.tur === 'Yazi') {
                        $('<label class="control-label"></label>').attr('for', kimlik).text(a.etiket).appendTo($satir);
                        $('<div></div>').append($('<input type="text" class="form-control" />').attr('id', kimlik).val(a.deger || '')).appendTo($satir);
                    } else if (a.tur === 'Secim') {
                        $('<label class="control-label"></label>').attr('for', kimlik).text(a.etiket).appendTo($satir);
                        var $sec = $('<select class="form-control"></select>').attr('id', kimlik);
                        $('<option value=""></option>').appendTo($sec);
                        $.each(a.secenekler || [], function (j, s) { $('<option></option>').val(s.kod).text(s.ad).appendTo($sec); });
                        // Kayıtlı değer listede yoksa seçenek olarak eklenir ki kaydederken kaybolmasın.
                        if (a.deger && !$sec.children().filter(function () { return this.value === String(a.deger); }).length)
                            $('<option></option>').val(a.deger).text(a.deger).appendTo($sec);
                        $sec.val(a.deger || '');
                        $('<div></div>').append($sec).appendTo($satir);
                    } else if (a.tur === 'Radyo') {
                        $('<span class="control-label"></span>').text(a.etiket).appendTo($satir);
                        var $r = $('<div role="radiogroup"></div>').attr('aria-label', a.etiket).appendTo($satir);
                        $.each(a.secenekler || [], function (j, s) {
                            var $l = $('<label class="radio-inline"></label>').appendTo($r);
                            $('<input type="radio" />').attr('name', kimlik).val(s.kod).prop('checked', String(s.kod) === String(a.deger)).appendTo($l);
                            $l.append(document.createTextNode(' ' + s.ad));
                        });
                    } else if (a.tur === 'Coklu') {
                        // Çoklu seçim: hep açık duran onay kutusu listesi; üstünde seçili sayısı ve toplu seçim düğmeleri
                        // (eski sayfalardaki sekmeli çoklu seçim listeleri).
                        $satir.addClass('ust-hizali');
                        $('<span class="control-label"></span>').text(a.etiket).appendTo($satir);
                        var secili = {};
                        $.each(String(a.deger || '').split(','), function (j, k) { if (k) secili[k] = true; });
                        var $d = $('<div class="tanim-coklu"></div>').attr('id', kimlik).appendTo($satir);
                        var $ust = $('<div class="tanim-coklu-ust"></div>').appendTo($d);
                        var $ozet = $('<span class="ozet" aria-live="polite"></span>').appendTo($ust);
                        var $kutu = $('<div class="tanim-secenekler dikey" role="group"></div>').attr('aria-label', a.etiket).appendTo($d);
                        var say = function () { $ozet.text($kutu.find('input:checked').length + ' / ' + $kutu.find('input').length + ' seçili'); };
                        var toplu = function (metin, is) {
                            $('<button type="button" class="btn btn-default btn-xs"></button>').text(metin).attr('aria-label', a.etiket + ': ' + metin)
                                .on('click', function () { $kutu.find('input').each(function () { this.checked = is(this.checked); }); say(); }).appendTo($ust);
                        };
                        toplu('Hepsini seç', function () { return true; });
                        toplu('Hiçbirini seçme', function () { return false; });
                        toplu('Tersine çevir', function (isaretli) { return !isaretli; });
                        $.each(a.secenekler || [], function (j, s) {
                            var $l = $('<label></label>').appendTo($kutu);
                            $('<input type="checkbox" />').val(s.kod).prop('checked', !!secili[s.kod]).on('change', say).appendTo($l);
                            $l.append(document.createTextNode(' ' + s.ad));
                        });
                        // Kayıtlı olup listede bulunmayan kodlar da işaretli olarak gösterilir (kaydederken kaybolmasın).
                        $kutu.find('input').each(function () { delete secili[this.value]; });
                        $.each(secili, function (k) {
                            var $l = $('<label></label>').appendTo($kutu);
                            $('<input type="checkbox" checked />').val(k).on('change', say).appendTo($l);
                            $l.append(document.createTextNode(' ' + k));
                        });
                        say();
                    }
                });
                document.getElementById('dlgYetki').showModal();
            })
            .fail(hataGoster);
    }

    $('#btnYetkiKaydet').on('click', function () {
        if (!acikYetki) return;
        var degerler = {};
        $.each(acikYetki.alanlar, function (i, a) {
            var $a = $('#' + alanKimlik(i));
            if (a.tur === 'Evet' || a.tur === 'EvetSayi') degerler[a.damga] = $a.prop('checked') ? 'true' : 'false';
            else if (a.tur === 'Radyo') degerler[a.damga] = $('input[name=' + alanKimlik(i) + ']:checked').val() || '0';
            else if (a.tur === 'Coklu') degerler[a.damga] = $a.find('input:checked').map(function () { return this.value; }).get().join(',');
            else degerler[a.damga] = $a.val() || '';
        });
        $('#btnYetkiKaydet').prop('disabled', true);
        Sgb.gonder('api/genelgorev/yetki-kaydet', { kod: gorev.kod, indis: acikYetki.indis, siraNo: acikYetki.siraNo, aciklama: $('#txtYetkiAciklama').val(), degerler: degerler })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
                acikYetki = null;
                document.getElementById('dlgYetki').close();
                $('#ddlIslem').val('');
                yetkileriYenile();
            })
            .fail(hataGoster)
            .always(function () { $('#btnYetkiKaydet').prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açılış

    grid.setGridOption('loading', true);
    Sgb.al('api/genelgorev/liste')
        .done(function (veri) { listeGoster(veri.satirlar); })
        .fail(hataGoster)
        .always(function () { grid.setGridOption('loading', false); });
});
