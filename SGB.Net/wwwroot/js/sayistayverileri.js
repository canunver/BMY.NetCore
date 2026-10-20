// Sayıştay Verileri (ButceMuhasebe/SayistayVerileri; eski SayistayVerileri.aspx). Solda Sayıştay'a gönderilecek tablolar, sağda
// seçilen tablonun kuralları: her tabloda yalnızca kullandığı alanlar açılır (eski KontrolleriGosterGizle). Banka hesapları ve
// sayım tutanaklarında (2.2 - 2.6) bilgiler tabloya girilir: tablo seçilince kayıtlı satırlar okunur (eski GridIcerikGetir),
// sonuna 5 boş satır eklenir; Kaydet saklar (eski GridIcerikKaydet), Satır Sil seçili satırları tablodan çıkarır.
// Dosya Hazırla dosyayı indirir (eski btnListele_Click). Uçlar: /api/sayistay/* (Muhasebe/SayistayVerileriUclari.cs).
// Ortak bileşen: Sgb.muhasebeBirim (yalnızca muhasebe, birimGizle).
$(function () {
    'use strict';

    var $kok = $('.sayistay-verileri');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var UC = 'api/sayistay/';
    var SAKLA = 'sayistayverileri.';
    var AYLAR = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
    var BOS_SATIR = 5;
    var IPUCU = '<span>Yıl, ay ve muhasebe birimini seçip Listele\'ye basınız.</span>';
    var tanim = null;
    var secili = null;  // seçili tablo { RAPORKODU, RAPORADI, ay, mizanTur, giris, kolonlar }
    var okunan = null;  // tablonun okunduğu ölçüt { tur, yil, ay, muhasebe }: Kaydet bunu kullanır
    var kirli = false;  // kaydedilmemiş değişiklik var mı?
    var ilkAd = true;   // açılıştaki saymanlık adı okuması
    var para = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 0, maximumFractionDigits: 2 });

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı.', 'danger');
    }
    function sakliOku(ad) { try { return localStorage.getItem(SAKLA + ad); } catch (e) { return null; } }
    function sakliYaz(ad, deger) { try { localStorage.setItem(SAKLA + ad, deger); } catch (e) { /* özel pencere */ } }

    $('#btnYardim').on('click', function () {
        var $p = $('#pnlYardim');
        var ac = $p.prop('hidden');
        $p.prop('hidden', !ac);
        $(this).attr('aria-expanded', ac ? 'true' : 'false');
    });

    // ---------------------------------------------------------------- alanlar

    function yil() { return parseInt($('#txtYil').val(), 10) || 0; }

    var muhBirim = Sgb.muhasebeBirim('#muhBirim', {
        kimlik: { muhasebe: 'txtMuhasebe', ad: 'lblMuhasebeAd' },
        yil: function () { return yil() || new Date().getFullYear(); },
        birimGizle: true,
        oncesi: kaybolsunMu,
        // açılıştaki ad okuması tabloyu yeniden okutmaz (tablo seçilirken zaten okunur)
        degisince: function () { if (ilkAd) { ilkAd = false; return; } olcutDegisti(); }
    });

    function olcut() {
        return {
            tur: secili ? secili.RAPORKODU : '',
            yil: yil(),
            ay: parseInt($('#ddlAy').val(), 10) || 0,
            muhasebe: muhBirim.deger().muhasebe || 0
        };
    }

    // ---------------------------------------------------------------- giriş tablosu (eski grdBilgi)

    // "1.234,56" ya da "1234,56" -> 1234.56. Boş -> 0. Geçersizse null (eski değer kalır).
    function sayiCoz(metin) {
        if (typeof metin === 'number') return metin;
        var s = String(metin == null ? '' : metin).replace(/\s/g, '').replace(/\./g, '').replace(',', '.');
        if (s === '') return 0;
        var n = Number(s);
        return isFinite(n) ? n : null;
    }

    function sutun(k) {
        var s = { field: k.dataIndex, headerName: k.baslik, headerTooltip: k.baslik, editable: true, sortable: false, cellClass: 'girilir' };
        if (k.tur === 'DBL') {
            return $.extend(s, {
                width: 130, type: 'rightAligned', cellClass: ['girilir', 'ag-right-aligned-cell'], cellEditor: 'agTextCellEditor',
                valueFormatter: function (p) { return p.value == null || p.value === '' ? '' : para.format(p.value); },
                valueParser: function (p) { var n = sayiCoz(p.newValue); return n === null ? p.oldValue : n; }
            });
        }
        if (k.tur === 'TAR') {
            return $.extend(s, {
                width: 130, cellDataType: 'dateString', cellEditor: 'agDateStringCellEditor',
                valueFormatter: function (p) { return p.value ? Sgb.tarih(p.value) : ''; }
            });
        }
        return $.extend(s, { width: 180, tooltipField: k.dataIndex, cellEditor: 'agTextCellEditor', cellEditorParams: { maxLength: 250 } });
    }

    var grid = Sgb.grid('#grdBilgi', {
        columnDefs: [],
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        enterNavigatesVertically: true,
        enterNavigatesVerticallyAfterEdit: true,
        rowSelection: { mode: 'multiRow', checkboxes: true, headerCheckbox: true, enableClickSelection: false },
        selectionColumnDef: { pinned: 'left', width: 40 },
        overlayNoRowsTemplate: IPUCU,
        onCellValueChanged: function (e) { if (e.newValue !== e.oldValue) kirli = true; },
        onSelectionChanged: function () { $('#btnSatirSil').prop('disabled', !grid.getSelectedRows().length); }
    });

    function bosSatir() {
        var s = {};
        $.each(secili ? secili.kolonlar : [], function (i, k) { s[k.dataIndex] = k.tur === 'DBL' ? 0 : ''; });
        return s;
    }

    function satirlar() {
        var l = [];
        grid.forEachNode(function (n) { if (n.data) l.push(n.data); });
        return l;
    }

    function tabloBosalt(ipucu) {
        okunan = null;
        kirli = false;
        grid.setGridOption('rowData', []);
        grid.setGridOption('overlayNoRowsTemplate', ipucu || IPUCU);
        $('#btnSatirSil').prop('disabled', true);
    }

    function kaybolsunMu() {
        return !kirli || window.confirm('Kaydedilmemiş değişiklikler var. Devam ederseniz kaybolacak. Devam edilsin mi?');
    }

    // Eski GridIcerikGetir: kayıtlı satırlar ve 5 boş satır
    function listele() {
        if (!secili || !secili.giris) return;
        var o = olcut();
        if (!o.yil) { tabloBosalt(); return; }
        grid.setGridOption('loading', true);
        Sgb.al(UC + 'liste', o).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); tabloBosalt(); return; }
            var l = (v.satirlar || []).slice();
            for (var i = 0; i < BOS_SATIR; i++) l.push(bosSatir());
            okunan = o;
            kirli = false;
            grid.setGridOption('rowData', l);
            $('#btnSatirSil').prop('disabled', true);
        }).fail(function (xhr) { hataGoster(xhr); tabloBosalt(); })
            .always(function () { grid.setGridOption('loading', false); });
    }

    // Yıl, ay ya da muhasebe değişince tablo yeniden okunur (eski sayfada tablo seçilirken okunurdu).
    function olcutDegisti() {
        if (secili && secili.giris) listele();
    }

    var oncekiYil = '', oncekiAy = '';
    $('#txtYil').on('focus', function () { oncekiYil = this.value; }).on('change', function () {
        if (!kaybolsunMu()) { this.value = oncekiYil; return; }
        oncekiYil = this.value;
        olcutDegisti();
    });
    $('#ddlAy').on('focus', function () { oncekiAy = this.value; }).on('change', function () {
        if (!kaybolsunMu()) { this.value = oncekiAy; return; }
        oncekiAy = this.value;
        olcutDegisti();
    });

    $('#btnListele').on('click', function () { if (kaybolsunMu()) listele(); });

    $('#btnSatirEkle').on('click', function () {
        if (!okunan) { mesaj('Önce Listele\'ye basınız.', 'warning'); return; }
        var r = grid.applyTransaction({ add: [bosSatir()] });
        if (r && r.add && r.add[0]) grid.ensureNodeVisible(r.add[0], 'bottom');
    });

    // Eski btnSatirSil: seçili satırlar tablodan çıkar; Kaydet'e basılınca saklanan bilgiden de silinir.
    $('#btnSatirSil').on('click', function () {
        var sec = grid.getSelectedRows();
        if (!sec.length) return;
        grid.applyTransaction({ remove: sec });
        kirli = true;
        $(this).prop('disabled', true);
    });

    // Eski btnKaydet_Click (onay sorulur)
    $('#btnKaydet').on('click', function () {
        if (!okunan) { mesaj('Önce Listele\'ye basınız.', 'warning'); return; }
        grid.stopEditing();
        var o = olcut();
        if (o.yil !== okunan.yil || o.ay !== okunan.ay || o.muhasebe !== okunan.muhasebe) {
            mesaj('Yıl, ay ya da muhasebe birimi değişti. Önce Listele\'ye basınız.', 'warning');
            return;
        }
        if (!window.confirm('Bilgiler kayıt edilecek. Onaylıyor musunuz?')) return;
        var $b = $(this).prop('disabled', true);
        Sgb.gonder(UC + 'kaydet', {
            Tur: okunan.tur, Yil: okunan.yil, Ay: okunan.ay, Muhasebe: okunan.muhasebe, Satirlar: satirlar()
        }).done(function (v) {
            if (v.hata) { mesaj(v.hata, 'danger'); return; }
            mesaj(v.bilgi || 'Bilgiler başarıyla kayıt edildi.', 'success');
            kirli = false;
            listele();
        }).fail(hataGoster).always(function () { $b.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- görünürlük (eski KontrolleriGosterGizle)

    function gorunurlukUygula() {
        $kok.find('[data-alan]').each(function () {
            var ad = $(this).attr('data-alan');
            $(this).prop('hidden', !(secili && secili[ad]));
        });
        tabloYay();
    }

    function raporSec(r, ilkAcilis) {
        if (!ilkAcilis && secili && r.RAPORKODU === secili.RAPORKODU) return;
        if (!ilkAcilis && !kaybolsunMu()) return;
        secili = r;
        $('#lstRapor li').each(function () {
            var s = $(this).attr('data-kod') === r.RAPORKODU;
            $(this).toggleClass('secili', s).attr('aria-selected', s ? 'true' : 'false');
        });
        $('#lblRaporBaslik').text(r.RAPORADI + ' - Rapor Kuralları');
        gorunurlukUygula();
        tabloBosalt();
        if (r.giris) {
            // Eski GridBaslikAyarla: tablonun sütunları, sonra kayıtlı satırlar okunur
            grid.setGridOption('columnDefs', $.map(r.kolonlar, sutun));
            listele();
        }
        if (!ilkAcilis) sakliYaz('rapor', r.RAPORKODU);
    }

    function listeCiz() {
        var $l = $('#lstRapor').empty();
        $.each(tanim.raporlar, function (i, r) {
            $('<li role="option" tabindex="0"></li>').attr('data-kod', r.RAPORKODU)
                .append($('<span class="say-no"></span>').text(i + 1))
                .append($('<span class="say-ad"></span>').text(r.RAPORADI))
                .data('rapor', r).appendTo($l);
        });
    }

    $('#lstRapor').on('click', 'li', function () { raporSec($(this).data('rapor')); });
    $('#lstRapor').on('keydown', 'li', function (e) {
        if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); raporSec($(this).data('rapor')); }
        else if (e.key === 'ArrowDown') { e.preventDefault(); $(this).next('li').trigger('focus'); }
        else if (e.key === 'ArrowUp') { e.preventDefault(); $(this).prev('li').trigger('focus'); }
    });

    // Tablo sayfanın kalan yüksekliğini doldurur, kendi kaydırma çubuğu olur.
    function tabloYay() {
        var el = document.getElementById('grdBilgi');
        if (!el || $(el).closest('[hidden]').length) return;
        var ust = el.getBoundingClientRect().top;
        el.style.height = Math.max(220, window.innerHeight - ust - 16) + 'px';
    }
    $(window).on('resize', tabloYay);

    // ---------------------------------------------------------------- Dosya Hazırla (eski btnListele_Click)

    $('#btnDosya').on('click', function () {
        if (!secili) { mesaj('Tabloyu seçiniz.', 'warning'); return; }
        var o = olcut();
        if (!o.yil) { mesaj('Yılı giriniz.', 'warning'); return; }
        if (secili.giris && kirli) mesaj('Tablodaki kaydedilmemiş değişiklikler dosyaya girmez; dosya saklanan bilgilerden hazırlanır.', 'warning');
        var p = { tur: o.tur, yil: o.yil, ay: o.ay, muhasebe: o.muhasebe, mizanTur: $('#ddlMizanTur').val() || 2 };
        var $b = $(this).prop('disabled', true);
        var $s = $b.find('.glyphicon').removeClass('glyphicon-file').addClass('glyphicon-hourglass');
        Promise.resolve(Sgb.indir(UC + 'dosya', p)).finally(function () {
            $b.prop('disabled', false);
            $s.removeClass('glyphicon-hourglass').addClass('glyphicon-file');
        });
    });

    $(window).on('beforeunload', function () { if (kirli) return 'Kaydedilmemiş değişiklikler var.'; });

    // ---------------------------------------------------------------- açılış (eski Page_Load)

    var $ay = $('#ddlAy');
    $.each(AYLAR, function (i, a) { $('<option></option>').val(String(i + 1)).text(a).appendTo($ay); });
    $ay.val(String(new Date().getMonth() + 1));

    Sgb.al(UC + 'tanim').done(function (t) {
        if (t.hata) { mesaj(t.hata, 'danger'); return; }
        tanim = t;

        var yillar = [];
        for (var y = t.yilIlk; y <= t.yilSon; y++) yillar.push(y);
        Sgb.yilKutusu('#txtYil', yillar, new Date().getFullYear());

        muhBirim.ata({ muhasebe: t.muhasebe || '', birim: '' });
        muhBirim.yenile(); // saymanlık adı (açılışta tablo henüz seçilmediği için okuma yapılmaz)

        listeCiz();
        // eski: ilk açılışta ilk tablo seçili; son seçilen tablo tarayıcıda hatırlanır
        var son = sakliOku('rapor');
        var r = null;
        $.each(t.raporlar, function (i, x) { if (x.RAPORKODU === son) r = x; });
        raporSec(r || t.raporlar[0], true);
    }).fail(hataGoster);
});
