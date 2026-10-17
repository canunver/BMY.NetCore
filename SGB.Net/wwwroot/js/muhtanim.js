// Muhasebe Tanımları sayfalarının ortak yardımcıları (ortak.js'ten sonra yüklenir). Uçlar: /api/muhtanim/* (Istemci/Muhasebe/Tanim).
//   Sgb.mt.oku(adres, parametreler, tablo) : GET; { hata } dönerse bildirir ve söz reddedilir; { uyari } varsa uyarı bildirimi.
//   Sgb.mt.islem(adres, veri, onay)         : onay sorulur (verilmişse), POST edilir, sonuç bildirilir; başarıda yanıtı verir.
//   Sgb.mt.form(secici)                    : [data-alan] alanlı formu okur / yazar / temizler (onay kutusu true/false).
//   Sgb.mt.tanim(o)                        : solda liste, sağda form biçimindeki basit tanım sayfası (Kaydet / Sil / Temizle).
//   Sgb.mt.tutar(x), Sgb.mt.sayi(metin)    : tutar biçimi (1.234,56) ve Türkçe yazılmış sayıyı okuma.
//   Sgb.mt.pencere(secici)                 : <dialog> içindeki [data-kapat] düğmelerini bağlar, pencereyi döndürür.
//   Hesap kodu kutuları ortak Sgb.hesapKodu bileşenidir (js/hesapkodu.js, [data-hesap-kodu]); form yazınca 'sgb:yazildi' çıkarır.
//   Sgb.mt.exceldenAktar(tur, aciklama)    : Excel'den aktarma penceresi (tur: HesapPlani | Yansitma).
//   .muh-tanim içindeki .dropdown-toggle düğmeleri açılır menüyü açar / kapatır; .mt-sekme-tus düğmeleri sekme değiştirir.
(function (Sgb, $) {
    'use strict';

    function red() { return $.Deferred().reject().promise(); }

    function hata(xhr, metin) {
        if (xhr && xhr.status === 401) return;
        Sgb.bildirim(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : (metin || 'İşlem yapılamadı.'), 'danger');
    }

    function oku(adres, parametreler, tablo) {
        if (tablo) tablo.setGridOption('loading', true);
        return Sgb.al(adres, parametreler).then(function (v) {
            if (v && v.hata) { Sgb.bildirim(v.hata, 'danger'); return red(); }
            if (v && v.uyari) Sgb.bildirim(v.uyari, 'warning');
            return v || {};
        }, function (xhr) {
            hata(xhr, 'Bilgiler okunamadı.');
            return red();
        }).always(function () {
            if (tablo) tablo.setGridOption('loading', false);
        });
    }

    var mesgulSayi = 0;
    function mesgul(evet) {
        mesgulSayi += evet ? 1 : -1;
        $('.muh-tanim .mt-arac .btn, .tanim-pencere .pencere-alt .btn-primary').prop('disabled', mesgulSayi > 0);
    }

    function islem(adres, veri, onay) {
        if (onay && !window.confirm(onay)) return red();
        mesgul(true);
        return Sgb.gonder(adres, veri).then(function (v) {
            if (!v || v.hata || v.basarili === false) { Sgb.bildirim((v && v.hata) || 'İşlem yapılamadı.', 'danger'); return red(); }
            Sgb.bildirim(v.bilgi || 'İşlem başarıyla gerçekleşti.', 'success');
            return v;
        }, function (xhr) {
            hata(xhr);
            return red();
        }).always(function () { mesgul(false); });
    }

    function form(secici) {
        var $f = $(secici);
        function alanlar() { return $f.find('[data-alan]'); }
        return {
            $f: $f,
            oku: function () {
                var v = {};
                alanlar().each(function () {
                    var $e = $(this), ad = $e.attr('data-alan');
                    if ($e.is(':checkbox')) v[ad] = $e.prop('checked');
                    else v[ad] = $.trim($e.val() == null ? '' : String($e.val()));
                });
                return v;
            },
            yaz: function (v) {
                v = v || {};
                alanlar().each(function () {
                    var $e = $(this), ad = $e.attr('data-alan');
                    if (!(ad in v)) return;
                    var d = v[ad];
                    if ($e.is(':checkbox')) $e.prop('checked', d === true || d === 1 || d === '1' || d === 'true');
                    else $e.val(d == null ? '' : String(d)).trigger('sgb:yazildi');
                });
            },
            temizle: function (varsayilan) {
                // Varsayılan değerler temizlemeden önce okunur (ör. yıl kutusunun o anki değeri korunsun).
                var v = typeof varsayilan === 'function' ? varsayilan() : varsayilan;
                alanlar().each(function () {
                    var $e = $(this);
                    if ($e.is(':checkbox')) $e.prop('checked', false);
                    else if ($e.is('select')) $e.prop('selectedIndex', 0);
                    else $e.val('').trigger('sgb:yazildi');
                });
                if (v) this.yaz(v);
            }
        };
    }

    var tutarBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    function tutar(x) {
        var n = typeof x === 'number' ? x : parseFloat(x);
        return isNaN(n) ? '' : tutarBicim.format(n);
    }

    // "1.234,56" ya da "1234.56" -> 1234.56
    function sayi(metin) {
        var s = $.trim(String(metin == null ? '' : metin));
        if (s === '') return 0;
        if (s.indexOf(',') >= 0) s = s.replace(/\./g, '').replace(',', '.');
        var n = parseFloat(s);
        return isNaN(n) ? 0 : n;
    }

    // Basit tanım sayfası: solda liste (AG Grid), sağda form. o:
    //   tablo, form           : seçiciler (ör. '#grdListe', '#frmBilgi')
    //   uc                    : uçların kökü (ör. 'api/muhtanim/makbuztipi/'); liste / kaydet / sil
    //   kolonlar              : AG Grid columnDefs
    //   anahtar               : satırın anahtar alanı (getRowId)
    //   listeParam()          : liste isteğinin parametreleri (yoksa boş); null dönerse kayıttan sonra liste yenilenmez
    //   satirdanForma(satir)  : seçilen satırdan forma yazılacak değerler (yoksa satırın kendisi); söz de dönebilir
    //   istek(formDegeri)     : Kaydet isteği (yoksa formun değeri)
    //   silIstek(formDegeri)  : Sil isteği; null dönerse uyarı verilir (silinecek kayıt seçilmedi)
    //   varsayilan            : Temizle'de forma yazılacak değerler (nesne ya da işlev)
    //   hazirla(satirlar)     : listelenen satırlar tabloya konmadan önce (ör. satır kimliği eklemek için)
    //   acilistaListele       : true ise sayfa açılınca listelenir (eski sayfadaki gibi)
    //   kaydedince(v), silince(v) : işlem sonrası (liste yenilendikten sonra)
    function tanim(o) {
        var f = form(o.form);
        var tablo = Sgb.grid(o.tablo, {
            columnDefs: o.kolonlar,
            rowSelection: { mode: 'singleRow', checkboxes: false, enableClickSelection: true },
            getRowId: o.anahtar ? function (p) { return String(p.data[o.anahtar]); } : undefined,
            onRowClicked: function (e) { sec(e.data); }
        });

        function sec(satir) {
            var d = o.satirdanForma ? o.satirdanForma(satir) : satir;
            $.when(d).done(function (deger) {
                f.temizle(o.varsayilan);
                f.yaz(deger);
                if (o.secilince) o.secilince(deger, satir);
            });
        }

        function listele(param) {
            var p = param || (o.listeParam ? o.listeParam() : {});
            if (p === null) return $.Deferred().resolve({}).promise(); // henüz arama yapılmadı
            return oku(o.uc + 'liste', p, tablo).done(function (v) {
                var satirlar = v.satirlar || [];
                if (o.hazirla) o.hazirla(satirlar);
                tablo.setGridOption('rowData', satirlar);
            });
        }

        function temizle() {
            f.temizle(o.varsayilan);
            tablo.deselectAll();
            if (o.temizlenince) o.temizlenince();
        }

        $('#btnKaydet').on('click', function () {
            var istek = o.istek ? o.istek(f.oku()) : f.oku();
            if (!istek) return;
            islem(o.uc + 'kaydet', istek).done(function (v) {
                temizle();
                listele().always(function () { if (o.kaydedince) o.kaydedince(v); });
            });
        });

        $('#btnSil').on('click', function () {
            var istek = o.silIstek ? o.silIstek(f.oku()) : f.oku();
            if (!istek) { Sgb.bildirim('Lütfen silmek istediğiniz bilgiyi listeden seçin.', 'warning'); return; }
            islem(o.uc + 'sil', istek, 'Seçili kayıt silinecek. Emin misiniz?').done(function (v) {
                temizle();
                listele().always(function () { if (o.silince) o.silince(v); });
            });
        });

        $('#btnTemizle').on('click', temizle);

        f.temizle(o.varsayilan);
        if (o.acilistaListele) listele();

        return { tablo: tablo, form: f, listele: listele, temizle: temizle, sec: sec };
    }

    // Pencere (<dialog>) açma / kapatma: [data-kapat] düğmeleri kapatır.
    function pencere(secici) {
        var p = $(secici)[0];
        if (!p) return null;
        $(p).on('click', '[data-kapat]', function () { p.close(); });
        return p;
    }

    // Excel'den aktar penceresi (eski ButceMuhasebe/ExcelAktar.aspx). tur: HesapPlani | Yansitma. aciklama: sütun düzeni.
    var excelPencere = null;
    function exceldenAktar(tur, aciklama, bitince) {
        if (!excelPencere) {
            excelPencere = $('<dialog class="tanim-pencere" aria-labelledby="mtExcelBaslik"></dialog>')
                .append('<div class="pencere-baslik"><h2 id="mtExcelBaslik">Excel\'den Aktar</h2>'
                    + '<button type="button" class="close" data-kapat aria-label="Kapat"><span aria-hidden="true">&times;</span></button></div>')
                .append('<div class="pencere-govde"><p class="mt-excel-aciklama"></p>'
                    + '<input type="file" class="form-control mt-excel-dosya" accept=".xls,.xlsx" aria-label="Excel dosyası" />'
                    + '<div class="alert mt-excel-sonuc" role="status" hidden style="margin:10px 0 0;white-space:pre-line"></div></div>')
                .append('<div class="pencere-alt"><button type="button" class="btn btn-default btn-sm" data-kapat>Kapat</button>'
                    + '<button type="button" class="btn btn-primary btn-sm mt-excel-aktar"><span class="glyphicon glyphicon-import" aria-hidden="true"></span> Aktar</button></div>')
                .appendTo(document.body);
            excelPencere.on('click', '[data-kapat]', function () { excelPencere[0].close(); });
            excelPencere.on('click', '.mt-excel-aktar', function () {
                var dosya = excelPencere.find('.mt-excel-dosya')[0].files[0];
                var $sonuc = excelPencere.find('.mt-excel-sonuc');
                function sonuc(metin, t) { $sonuc.removeClass('alert-success alert-danger').addClass('alert-' + t).text(metin).prop('hidden', false); }
                if (!dosya) { sonuc('Lütfen aktarım yapılacak dosyayı seçin.', 'danger'); return; }
                var veri = new FormData();
                veri.append('tur', excelPencere.data('tur'));
                veri.append('dosya', dosya);
                var $d = $(this).prop('disabled', true);
                $.ajax({
                    url: Sgb.KOK + 'api/muhtanim/excel-aktar', method: 'POST', data: veri, processData: false, contentType: false,
                    dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
                }).done(function (v) {
                    if (v.hata) sonuc(v.hata, 'danger');
                    else { sonuc(v.bilgi, 'success'); var b = excelPencere.data('bitince'); if (b) b(); }
                }).fail(function (xhr) {
                    if (xhr.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
                    sonuc(xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Aktarım yapılamadı.', 'danger');
                }).always(function () { $d.prop('disabled', false); });
            });
        }
        excelPencere.data({ tur: tur, bitince: bitince });
        excelPencere.find('.mt-excel-aciklama').text(aciklama || '');
        excelPencere.find('.mt-excel-dosya').val('');
        excelPencere.find('.mt-excel-sonuc').prop('hidden', true);
        excelPencere[0].showModal();
    }

    // Araç çubuğundaki açılır menüler (Aktar, Diğer). Bootstrap JS yüklenmediği için .open burada değiştirilir.
    function menuleriKapat() {
        $('.muh-tanim .btn-group.open').removeClass('open').children('.dropdown-toggle').attr('aria-expanded', 'false');
    }
    $(document).on('click', '.muh-tanim .dropdown-toggle', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var $g = $(this).closest('.btn-group');
        var acik = $g.hasClass('open');
        menuleriKapat();
        if (!acik) $g.addClass('open').children('.dropdown-toggle').attr('aria-expanded', 'true');
    });
    $(document).on('click', function () { menuleriKapat(); });
    $(document).on('keydown', function (e) { if (e.key === 'Escape') menuleriKapat(); });

    // Sekmeli bölüm (.mt-sekmeli): .mt-sekme-tus[data-sekme] düğmeleri aynı adlı .mt-sekme bölmesini gösterir;
    // kutunun rengi düğmenin data-renk değerine (turuncu, yesil, mor) göre değişir.
    var RENKLER = 'turuncu yesil mor';
    $(document).on('click', '.muh-tanim .mt-sekme-tus', function () {
        var $t = $(this), ad = $t.attr('data-sekme'), $k = $t.closest('.mt-sekmeli');
        $k.find('.mt-sekme-tus').each(function () {
            var evet = $(this).attr('data-sekme') === ad;
            $(this).toggleClass('active', evet).attr('aria-selected', evet ? 'true' : 'false');
        });
        $k.children('.mt-sekme').each(function () { this.hidden = $(this).attr('data-sekme') !== ad; });
        $k.removeClass(RENKLER).addClass($t.attr('data-renk') || '');
    });

    Sgb.mt = { hata: hata, oku: oku, islem: islem, form: form, tanim: tanim, tutar: tutar, sayi: sayi, pencere: pencere, exceldenAktar: exceldenAktar, buYil: new Date().getFullYear() };
})(window.Sgb, jQuery);
