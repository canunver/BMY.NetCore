// Bordro Tabloları (HarcamaSurecleri/BordroTabloForm; eski BordroTabloForm.aspx).
// Solda tablolar (AG Grid): kalemle tablo bilgisi düzenlenir, Yeni Kayıt yeni tablo açar; Yazdır, Maaş Hesapla, Dosya Yükle ve
// Artış Yap seçili tablo ve yıl için çalışır. Sağda seçili tablonun ünvan ve yıla göre derece değerleri: Listele okur,
// Değer ve Çarpan hücreleri düzenlenir, Kaydet saklar; Kopyala kaynak tablonun değerlerini bu tabloya kopyalar.
$(function () {
    'use strict';

    if (!$('#grdTablolar').length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var secili = null;   // soldan seçilen tablo { kod, ad, tur }
    var okunan = null;   // sağ tabloda gösterilen değerlerin anahtarı { kod, unvanKod, yil }
    var IPUCU = '<span class="tablo-ipucu">Soldan tablo, yukarıdan ünvan seçip Listele\'ye basınız.</span>';

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function tamsayi(v) { var n = parseInt(v, 10); return isNaN(n) ? 0 : n; }
    function yil() { return Math.max(0, tamsayi($('#txtYil').val())); }
    function unvanKod() { return tamsayi($('#cmbUnvan').val()); }

    var sayiBicim = new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 4, maximumFractionDigits: 4 });
    function sayiYaz(p) { return p.value === null || p.value === undefined || p.value === '' ? '' : sayiBicim.format(p.value); }

    // ---------------------------------------------------------------- tablolar

    function KalemDugmesi() { }
    KalemDugmesi.prototype.init = function (p) {
        var b = document.createElement('button');
        b.type = 'button';
        b.className = 'btn btn-link btn-xs';
        b.title = 'Tablo bilgisini düzenle';
        b.innerHTML = '<span class="glyphicon glyphicon-pencil" aria-hidden="true"></span><span class="sr-only">Tablo bilgisini düzenle</span>';
        b.addEventListener('click', function (e) { e.stopPropagation(); tabloPenceresi(p.data); });
        this.e = b;
    };
    KalemDugmesi.prototype.getGui = function () { return this.e; };
    KalemDugmesi.prototype.refresh = function () { return false; };

    var tablolar = Sgb.grid('#grdTablolar', {
        columnDefs: [
            { field: 'kod', headerName: 'Kod', width: 70, type: 'rightAligned' },
            { colId: 'duzenle', headerName: '', width: 44, sortable: false, resizable: false, cellRenderer: KalemDugmesi },
            { field: 'ad', headerName: 'Ad', flex: 1, minWidth: 160, tooltipField: 'ad' }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { tabloSec(e.data); },
        overlayNoRowsTemplate: '<span class="text-muted">Bordro tablosu yok</span>'
    });

    function tabloSec(t) {
        var ayni = secili && t && secili.kod === t.kod;
        secili = t || null;
        var $l = $('#lblTablo');
        if (secili) $l.removeClass('bos').text(secili.kod + ' - ' + (secili.ad || '') + (secili.tur === 2 ? ' (Derece/Kademe)' : ' (Ünvan/Derece)'));
        else $l.addClass('bos').text('Soldan bir tablo seçiniz.');
        if (!ayni) degerleriTemizle();
    }

    function tablolariDoldur(liste, secilecekKod) {
        tablolar.setGridOption('rowData', liste || []);
        var n = secilecekKod ? tablolar.getRowNode(String(secilecekKod)) : null;
        if (n) { n.setSelected(true); tablolar.ensureNodeVisible(n); tabloSec(n.data); }
        else if (secili && !tablolar.getRowNode(String(secili.kod))) tabloSec(null);
        else if (secili) tablolar.getRowNode(String(secili.kod)).setSelected(true);
    }

    function tablolariOku(secilecekKod) {
        tablolar.setGridOption('loading', true);
        return Sgb.al('api/bordrotablo/tablolar')
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                tablolariDoldur(v.tablolar, secilecekKod);
            })
            .fail(hataGoster)
            .always(function () { tablolar.setGridOption('loading', false); });
    }

    function tabloGerekli() {
        if (secili) return true;
        mesaj('Soldan bir tablo seçiniz.', 'warning');
        return false;
    }

    // ---------------------------------------------------------------- değerler

    function sayiSutun(alan, baslik) {
        return {
            field: alan, headerName: baslik, width: 150, type: 'rightAligned', cellDataType: 'number', editable: true, sortable: false,
            cellClass: ['btt-girilir', 'ag-right-aligned-cell'],
            cellEditor: 'agNumberCellEditor', cellEditorParams: { precision: 4 },
            valueFormatter: sayiYaz,
            valueParser: function (p) { var n = parseFloat(String(p.newValue).replace(',', '.')); return isNaN(n) ? 0 : n; }
        };
    }

    var degerler = Sgb.grid('#grdDegerler', {
        columnDefs: [
            { field: 'derece', headerName: 'Derece', width: 100, type: 'rightAligned', sortable: false, cellClass: ['btt-salt', 'ag-right-aligned-cell'] },
            sayiSutun('deger', 'Değer'),
            sayiSutun('carpan', 'Çarpan')
        ],
        suppressCellFocus: false,
        singleClickEdit: true,
        stopEditingWhenCellsLoseFocus: true,
        overlayNoRowsTemplate: IPUCU
    });

    function degerleriTemizle() {
        okunan = null;
        degerler.setGridOption('rowData', []);
    }

    function oku() {
        if (!tabloGerekli()) return;
        if (unvanKod() <= 0) { mesaj('Tablo kodu ve unvan kodu seçiniz!', 'warning'); $('#cmbUnvan').trigger('focus'); return; }
        var anahtar = { kod: secili.kod, unvanKod: unvanKod(), yil: yil() };
        var $d = $('#btnOku').prop('disabled', true);
        degerler.setGridOption('loading', true);
        Sgb.al('api/bordrotablo/oku', anahtar)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                okunan = anahtar;
                degerler.setGridOption('rowData', v.satirlar || []);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); degerler.setGridOption('loading', false); });
    }

    $('#btnOku').on('click', oku);

    // Tablo, ünvan ya da yıl değişince eski değerler başka bir anahtara saklanmasın diye tablo boşaltılır.
    $('#cmbUnvan, #txtYil').on('change', degerleriTemizle);
    $('#btnUnvanTemizle').on('click', function () { $('#cmbUnvan').val('').trigger('change'); });

    $('#btnKaydet').on('click', function () {
        degerler.stopEditing();
        if (!okunan) { mesaj('Önce tablo ve ünvan seçip Listele\'ye basınız.', 'warning'); return; }
        if (!window.confirm('Kaydetmek İstiyor musunuz?')) return;
        var satirlar = [];
        degerler.forEachNode(function (n) {
            satirlar.push({ derece: tamsayi(n.data.derece), deger: Number(n.data.deger) || 0, carpan: Number(n.data.carpan) || 0 });
        });
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/bordrotablo/kaydet', $.extend({ satirlar: satirlar }, okunan))
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    $('#btnKopyala').on('click', function () {
        if (!tabloGerekli()) return;
        var kaynak = tamsayi($('#txtKaynakKod').val());
        if (kaynak <= 0) { mesaj('Kaynak tablo kodunu giriniz!', 'warning'); $('#txtKaynakKod').trigger('focus'); return; }
        if (!window.confirm('Kaynak tablo verileri, bu tablo  üzerine yazılacaktır. Onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/bordrotablo/kopyala', { kod: secili.kod, yil: yil(), kaynakKod: kaynak, unvanKod: unvanKod(), ustuneYaz: $('#chkUstuneYaz').prop('checked') })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
                if (okunan) oku();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- yazdır / maaş hesapla / dosya yükle / artış

    // POST ile dosya isteği: yanıt dosya ya da { hata }.
    function dosyaIste(adres, govde) {
        return fetch(Sgb.KOK + adres, {
            method: 'POST', credentials: 'same-origin', cache: 'no-store',
            headers: { RequestVerificationToken: Sgb.meta('istek-jetonu'), 'Content-Type': 'application/json; charset=utf-8' },
            body: JSON.stringify(govde)
        }).then(function (y) {
            if (y.status === 401) { window.top.location.href = Sgb.KOK + 'Giris'; return; }
            if (!y.ok) { mesaj(y.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'Dosya hazırlanamadı.', 'danger'); return; }
            var tur = y.headers.get('Content-Type') || '';
            if (tur.indexOf('json') >= 0) return y.json().then(function (v) { mesaj(v.hata || 'Dosya hazırlanamadı.', 'danger'); });
            if (tur.indexOf('text/html') >= 0) { mesaj('Dosya hazırlanamadı.', 'danger'); return; }
            var b = y.headers.get('Content-Disposition') || '';
            var ad = /filename\*=UTF-8''([^;]+)/i.exec(b) || /filename="?([^";]+)"?/i.exec(b);
            return y.blob().then(function (icerik) {
                var yerel = URL.createObjectURL(icerik);
                var a = document.createElement('a');
                a.href = yerel;
                a.download = ad ? decodeURIComponent(ad[1]) : 'dosya';
                document.body.appendChild(a);
                a.click();
                a.remove();
                setTimeout(function () { URL.revokeObjectURL(yerel); }, 1000);
            });
        }).catch(function () { mesaj('Dosya hazırlanamadı.', 'danger'); });
    }

    $('#btnYazdir').on('click', function () {
        if (!tabloGerekli()) return;
        var $d = $(this).prop('disabled', true);
        Sgb.indir('api/bordrotablo/yazdir', { kod: secili.kod, yil: yil() }).then(function () { $d.prop('disabled', false); });
    });

    $('#btnMaasHesapla').on('click', function () {
        var ad = unvanKod() > 0 ? $('#cmbUnvan option:selected').text() : 'bütün aktif ünvanlar';
        if (!window.confirm(yil() + ' yılı için maaş hesabı yapılacak (' + ad + '). Hesaplanan net maaşlar 100-125 ve 200-225 kodlu tablolara da yazılacak. Devam edilsin mi?')) return;
        var $d = $(this).prop('disabled', true);
        mesaj('Maaş hesaplanıyor, lütfen bekleyiniz...', 'info');
        dosyaIste('api/bordrotablo/maashesapla', { yil: yil(), unvanKod: unvanKod() }).then(function () { $d.prop('disabled', false); });
    });

    // Dosya seçilince onay sorulup hemen yüklenir (eski FileSelected).
    $('#fuYukle').on('change', function () {
        var girdi = this, dosya = this.files && this.files[0];
        if (!dosya) return;
        if (!tabloGerekli() || !window.confirm('Dosyada bulunan veriler yüklenecek ve var olan verilerin üzerine yazılacaktır. Onaylıyor musunuz?')) {
            girdi.value = '';
            return;
        }
        var veri = new FormData();
        veri.append('dosya', dosya);
        veri.append('kod', secili.kod);
        veri.append('yil', yil());
        $('#lblYukle').addClass('disabled');
        mesaj('Lütfen Bekleyiniz!', 'info');
        $.ajax({
            url: Sgb.KOK + 'api/bordrotablo/yukle', method: 'POST', data: veri, processData: false, contentType: false,
            dataType: 'json', headers: { RequestVerificationToken: Sgb.meta('istek-jetonu') }
        }).done(function (v) {
            if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
            mesaj(v.bilgi, 'success');
            if (okunan) oku();
        }).fail(function (xhr) {
            if (xhr.status === 401) window.top.location.href = Sgb.KOK + 'Giris';
            else hataGoster(xhr);
        }).always(function () { $('#lblYukle').removeClass('disabled'); girdi.value = ''; });
    });

    $('#btnArtis').on('click', function () {
        if (!tabloGerekli()) return;
        var katsayi = parseFloat(String($('#txtArtisKatsayi').val()).replace(',', '.')) || 0;
        if (katsayi <= 0) { mesaj('Artış katsayısı giriniz!', 'warning'); $('#txtArtisKatsayi').trigger('focus'); return; }
        if (!window.confirm(secili.kod + ' kodlu tablonun ' + yil() + ' yılı değerleri ' + katsayi + ' ile çarpılacak (önceki değerler ' + (secili.kod + 1000) + ' kodlu tabloya yedeklenir). Onaylıyor musunuz?')) return;
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/bordrotablo/artis', { kod: secili.kod, yil: yil(), katsayi: katsayi })
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                mesaj(v.bilgi, 'success');
                if (okunan) oku();
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- tablo bilgi penceresi (eski wndTabloBilgi)

    var dlgTablo = document.getElementById('dlgTablo');

    function tabloPenceresi(t) {
        $('#txtTabloKod').val(t ? t.kod : 0);
        $('#txtTabloAd').val(t ? t.ad || '' : '');
        $('#rdUnvanDerece').prop('checked', !t || t.tur !== 2);
        $('#rdDereceKademe').prop('checked', !!t && t.tur === 2);
        dlgTablo.showModal();
        $('#txtTabloKod').trigger('focus');
    }

    $('#btnYeni').on('click', function () { tabloPenceresi(null); });
    $('dialog').on('click', '[data-kapat]', function () { $(this).closest('dialog')[0].close(); });

    $('#btnTanimKaydet').on('click', function () {
        var istek = { kod: tamsayi($('#txtTabloKod').val()), ad: $.trim($('#txtTabloAd').val()), tur: $('#rdDereceKademe').prop('checked') ? 2 : 1 };
        var $d = $(this).prop('disabled', true);
        Sgb.gonder('api/bordrotablo/tanimkaydet', istek)
            .done(function (v) {
                if (!v.basarili) { mesaj(v.hata, 'danger'); return; }
                dlgTablo.close();
                mesaj(v.bilgi, 'success');
                tablolariOku(v.kod);
            })
            .fail(hataGoster)
            .always(function () { $d.prop('disabled', false); });
    });

    // ---------------------------------------------------------------- açılış (eski Page_Load: tablolar ve aktif ünvanlar)

    Sgb.al('api/bordrotablo/baslangic')
        .done(function (v) {
            var $u = $('#cmbUnvan');
            $.each(v.unvanlar || [], function (i, u) { $('<option></option>').val(u.UnvanKod).text(u.UnvanAd).appendTo($u); });
            if (v.hata) mesaj(v.hata, 'danger');
            tablolariDoldur(v.tablolar, 0);
        })
        .fail(hataGoster);
});
