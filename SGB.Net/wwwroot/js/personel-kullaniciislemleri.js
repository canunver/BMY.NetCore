// Personel İşlemleri > Kullanıcı İşlemleri sekmesi (InsanKaynaklari/PersonelKullaniciIslemleri; eski PersonelKullaniciIslemleri.aspx).
// Kişinin (adres satırındaki T.C. kimlik no) SGB kullanıcısı yoksa "SGB Kullanıcısı Yap" bölümü (önerilen kullanıcı kodları ile),
// varsa parola işlemleri ve rolleri gösterilir. Roller seçilip kaldırılır; görev + birim(ler) seçilip "Görev Ekle" ile eklenir.
$(function () {
    'use strict';

    var $kok = $('#pbKullanici');
    if (!$kok.length) return; // yetkisiz kullanıcıya yalnızca uyarı basılır

    var tcKimlikNo = $.trim(String($kok.attr('data-tc-kimlik-no') || ''));
    var secilenBirimler = {}; // kod -> ad (birim ağacında işaretlenenler)
    var birimDeger = '';      // eski ddlBirim.value: "kod, kod"

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(xhr && xhr.status === 403 ? 'Bu işlem için yetkiniz yok.' : 'İşlem yapılamadı. Lütfen tekrar deneyiniz.', 'danger');
    }
    function dugmeIslem($d, istek) {
        $d.prop('disabled', true);
        return istek.always(function () { $d.prop('disabled', false); });
    }

    var gpRoller = Sgb.grid('#gpRoller', {
        columnDefs: [
            { headerName: '', width: 44, checkboxSelection: true, headerCheckboxSelection: true, sortable: false, resizable: false, cellClass: 'hucre-salt' },
            { field: 'BIRIMAD', headerName: 'Birim', width: 520, tooltipField: 'BIRIMAD', cellClass: 'hucre-salt' },
            { field: 'ROLAD', headerName: 'Rol', flex: 1, minWidth: 160, tooltipField: 'ROLAD', cellClass: 'hucre-salt' }
        ],
        rowSelection: 'multiple',
        suppressRowClickSelection: true,
        overlayNoRowsTemplate: '<span class="text-muted">Rol yok</span>'
    });

    // ---------------------------------------------------------------- durum (eski KullaniciKontrol, GorevleriDoldur)

    function gorevleriDoldur(gorevler) {
        var $c = $('#cmbGorevler');
        var secili = $c.val();
        $c.empty();
        $('<option></option>').val('').text('').appendTo($c);
        $.each(gorevler || [], function (i, g) { $('<option></option>').val(g.Anahtar).text(g.ad).appendTo($c); });
        if (secili) $c.val(secili);
    }

    function kullaniciKontrol() {
        return Sgb.al('api/personelbilgi/kullanici/durum', { tcKimlikNo: tcKimlikNo })
            .done(function (v) {
                if (v.hata) { mesaj(v.hata, 'danger'); return; }
                gorevleriDoldur(v.gorevler);
                $('#panelKullaniciYok').prop('hidden', v.kullaniciVar);
                $('#panelKullaniciVar').prop('hidden', !v.kullaniciVar);
                $('#txtSQBKullaniciKodu').val(v.kullaniciKodu || '');
                gpRoller.setGridOption('rowData', v.roller || []);
                var $l = $('#lstOneri').empty();
                $.each(v.oneriler || [], function (i, k) { $('<option></option>').val(k).appendTo($l); });
                if (!v.kullaniciVar && v.oneriler && v.oneriler.length && !$('#cbOneriKullaniciKodu').val())
                    $('#cbOneriKullaniciKodu').val(v.oneriler[0]);
            })
            .fail(hataGoster);
    }

    // ---------------------------------------------------------------- düğmeler

    $('#btnYeniKullaniciOlustur').on('click', function () {
        var kod = $.trim($('#cbOneriKullaniciKodu').val());
        if (kod === '') { mesaj('Kullanıcı Kodu Boş Olmamalıdır.', 'warning'); return; }
        dugmeIslem($(this), Sgb.gonder('api/personelbilgi/kullanici/olustur', { tcKimlikNo: tcKimlikNo, kullaniciKodu: kod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                if (s.bilgi) mesaj(s.bilgi, 'success');
                kullaniciKontrol();
            }).fail(hataGoster));
    });

    $('#btnDegistir').on('click', function () {
        var p1 = $('#txtSQBYeniParola').val(), p2 = $('#txtSQBYeniParolaTekrar').val();
        if (p1 !== p2) { mesaj('Parola ve Parola Tekrarı Aynı Olmalıdır.', 'warning'); return; }
        if ($.trim($('#txtSQBKullaniciKodu').val()) === '') { mesaj('Kullanıcı Kodu Boş Olmamalıdır.', 'warning'); return; }
        dugmeIslem($(this), Sgb.gonder('api/personelbilgi/kullanici/parola-degistir', {
            tcKimlikNo: tcKimlikNo,
            eskiParola: $('#txtSQBEskiParola').val(),
            yeniParola: p1,
            yeniParolaTekrar: p2
        }).done(function (s) {
            if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
            mesaj(s.bilgi, 'success');
            $('#txtSQBEskiParola, #txtSQBYeniParola, #txtSQBYeniParolaTekrar').val('');
        }).fail(hataGoster));
    });

    $('#btnYeniParolaAta').on('click', function () {
        dugmeIslem($(this), Sgb.gonder('api/personelbilgi/kullanici/yeni-parola', { tcKimlikNo: tcKimlikNo })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                mesaj(s.bilgi, 'success');
            }).fail(hataGoster));
    });

    // Eski btnRolSil_Click: seçili satırlar
    $('#btnRolSil').on('click', function () {
        var secili = $.map(gpRoller.getSelectedRows(), function (r) { return { BIRIMKOD: r.BIRIMKOD, ROLKOD: r.ROLKOD, ROLSIRANO: r.ROLSIRANO }; });
        if (!secili.length) { mesaj('Lütfen kaldırılacak rolleri seçiniz.', 'warning'); return; }
        dugmeIslem($(this), Sgb.gonder('api/personelbilgi/kullanici/rol-sil', { tcKimlikNo: tcKimlikNo, satirlar: secili })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                if (s.bilgi) mesaj(s.bilgi, 'success');
                gpRoller.setGridOption('rowData', s.roller || []);
            }).fail(hataGoster));
    });

    // Eski btnGorevEkle_Click
    $('#btnGorevEkle').on('click', function () {
        var gorevKod = $('#cmbGorevler').val() || '';
        if (!gorevKod) { mesaj('Lütfen görev seçiniz!', 'warning'); return; }
        if (!birimDeger) { mesaj('Lütfen birim seçiniz!', 'warning'); return; }
        dugmeIslem($(this), Sgb.gonder('api/personelbilgi/kullanici/gorev-ekle', { tcKimlikNo: tcKimlikNo, gorevKod: gorevKod, birimler: birimDeger })
            .done(function (s) {
                if (s.hata) mesaj(s.hata, 'danger');
                if (s.eklenen > 0) kullaniciKontrol();
            }).fail(hataGoster));
    });

    // ---------------------------------------------------------------- birim seçimi (eski ddlBirim + trpBirim, çoklu seçim)

    var dlgBirim = document.getElementById('dlgBirim');
    var agac = null;

    function birimAgaciKur() {
        agac = Sgb.agac('#trpBirim', {
            yukle: function (ust) {
                return Sgb.al('api/personelbilgi/kullanici/birimler', { ust: ust ? ust.kod : 0 }).then(function (v) {
                    if (v.hata) mesaj(v.hata, 'danger');
                    return v.satirlar || [];
                });
            },
            isaretli: function (d) { return Object.prototype.hasOwnProperty.call(secilenBirimler, String(d.kod)); },
            isaretlenince: function (d, evet) {
                if (evet) secilenBirimler[String(d.kod)] = d.ad;
                else delete secilenBirimler[String(d.kod)];
            },
            bosMetin: 'Birim yok',
            hata: hataGoster
        });
    }

    $('#btnBirimSec').on('click', function () {
        if (!agac) birimAgaciKur(); else agac.isaretleriGuncelle();
        dlgBirim.showModal();
    });

    // Eski SecilenleriAl: işaretli birimlerin kodları ve adları alana yazılır.
    $('#btnBirimTamam').on('click', function () {
        var kodlar = [], adlar = [];
        $.each(secilenBirimler, function (k, ad) { kodlar.push(k); adlar.push(ad); });
        birimDeger = kodlar.join(', ');
        $('#ddlBirimMetin').val(adlar.join(', ')).attr('title', adlar.join(', '));
        dlgBirim.close();
    });

    $(dlgBirim).on('click', '[data-kapat]', function () { dlgBirim.close(); });

    $('#btnBirimTemizle').on('click', function () {
        secilenBirimler = {};
        birimDeger = '';
        $('#ddlBirimMetin').val('').removeAttr('title');
        if (agac) agac.yenile();
    });

    kullaniciKontrol();
});
