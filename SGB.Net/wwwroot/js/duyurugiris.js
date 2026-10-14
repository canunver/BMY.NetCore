// Duyuru Girişi (Duyuru/DuyuruGiris): solda arama + liste (AG Grid), sağda seçili duyurunun bilgileri ve metni.
$(function () {
    'use strict';

    var M = JSON.parse($('#metinler').text() || '{}');
    var editor = Sgb.editor('#txtEditor', { yukseklik: 340 });
    // Duyurunun ekli dosyaları (modül DUY, form = duyuru kodu); duyuru kaydedilmeden yükleme kapalıdır.
    var dosyalar = Sgb.ekliDosyalar('#dosyalar', { modul: 'DUY', form: '-1' });
    var tanimlar = { onemler: [], kullaniciTipleri: [], destekYoneticisi: false };

    function mesaj(metin, tur) { Sgb.bildirim(metin, tur); }
    function hataGoster(xhr) {
        if (xhr && xhr.status === 401) return;
        mesaj(M.IslemHata, 'danger');
    }
    function mesgul(evet) { $('#btnKaydet, #btnYeni, #btnListele').prop('disabled', evet); }

    // ---------------------------------------------------------------- liste

    var onemAd = {};
    var grid = Sgb.grid('#grdListe', {
        columnDefs: [
            {
                field: 'onem', headerName: M.Onem, width: 84,
                cellRenderer: function (p) {
                    var s = document.createElement('span');
                    s.className = 'duyuru-onem-isaret onem-' + p.value;
                    s.title = onemAd[p.value] || '';
                    s.textContent = onemAd[p.value] || p.value;
                    return s;
                }
            },
            { field: 'tarih', headerName: M.FRMDYG006, width: 96, sort: 'desc', valueFormatter: function (p) { return Sgb.tarih(p.value); } },
            { field: 'konu', headerName: M.Konu, width: 160, tooltipField: 'konu' },
            { field: 'gizli', headerName: M.Gizlilik, width: 86, cellDataType: 'text', valueFormatter: function (p) { return p.value ? M.Gizli : M.Acik; } },
            { field: 'ozet', headerName: M.FRMDYL002, flex: 1, minWidth: 220, tooltipField: 'ozet' }
        ],
        rowSelection: 'single',
        getRowId: function (p) { return String(p.data.kod); },
        onRowClicked: function (e) { duyuruAc(e.data.kod); }
    });

    function aramaKosullari() {
        return {
            konu: $('#aramaKonu').val(),
            basla: $('#aramaBasla').val(),
            bitis: $('#aramaBitis').val(),
            onem: $('#aramaOnem').val(),
            gizle: $('input[name=aramaGizle]:checked').val(),
            pencerede: $('input[name=aramaPencere]:checked').val()
        };
    }

    function listele(secilecekKod) {
        grid.setGridOption('loading', true);
        return Sgb.al('api/duyuru/liste', aramaKosullari())
            .done(function (veri) {
                if (veri.hata) { mesaj(veri.hata, 'danger'); return; }
                grid.setGridOption('rowData', veri.satirlar);
                if (secilecekKod) {
                    var n = grid.getRowNode(String(secilecekKod));
                    if (n) { n.setSelected(true); grid.ensureNodeVisible(n); }
                }
            })
            .fail(hataGoster)
            .always(function () { grid.setGridOption('loading', false); });
    }

    $('#frmArama').on('submit', function (e) {
        e.preventDefault();
        listele();
    });

    // ---------------------------------------------------------------- form

    function temizle() {
        $('#txtKod').val('0');
        $('#txtTarih').val(Sgb.bugun());
        $('#txtKonu').val('');
        $('#ddlOnem').prop('selectedIndex', 0);
        // Yeni duyuru varsayılan olarak bütün kullanıcılara (0); destek yöneticisinde seçim zorunludur.
        $('#lstKullaniciTipi').val(tanimlar.destekYoneticisi ? [] : ['0']);
        $('#chkGizli, #chkPencere').prop('checked', false);
        editor.html('');
        $('#btnSil').prop('disabled', true);
        dosyalar.formDegistir('-1');
        grid.deselectAll();
    }

    function duyuruAc(kod) {
        mesaj('');
        Sgb.al('api/duyuru/' + encodeURIComponent(kod))
            .done(function (d) {
                $('#txtKod').val(d.kod);
                $('#txtTarih').val(d.tarih);
                $('#txtKonu').val(d.konu || '');
                $('#ddlOnem').val(String(d.onem));
                if ($('#ddlOnem').prop('selectedIndex') < 0) $('#ddlOnem').prop('selectedIndex', 0);
                $('#lstKullaniciTipi').val($.map(d.kullaniciTipleri, String));
                $('#chkGizli').prop('checked', d.gizli);
                $('#chkPencere').prop('checked', d.pencerede);
                editor.html(d.metin || '');
                $('#btnSil').prop('disabled', false);
                dosyalar.formDegistir(d.kod);
            })
            .fail(hataGoster);
    }

    $('#btnYeni').on('click', function () {
        mesaj('');
        temizle();
        $('#txtKonu').trigger('focus');
    });

    $('#btnKaydet').on('click', function () {
        if (editor.bos()) {
            mesaj(M.FRMDYG015, 'warning');
            editor.odaklan();
            return;
        }
        var metin = editor.html();
        var istek = {
            kod: parseInt($('#txtKod').val(), 10) || 0,
            tarih: $('#txtTarih').val(),
            konu: $('#txtKonu').val(),
            onem: parseInt($('#ddlOnem').val(), 10) || 0,
            kullaniciTipleri: $.map($('#lstKullaniciTipi').val() || [], function (v) { return parseInt(v, 10); }),
            gizli: $('#chkGizli').prop('checked'),
            pencerede: $('#chkPencere').prop('checked'),
            metin: metin
        };
        mesgul(true);
        Sgb.gonder('api/duyuru/kaydet', istek)
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                $('#txtKod').val(s.kod);
                $('#btnSil').prop('disabled', false);
                dosyalar.formDegistir(s.kod);
                mesaj(s.bilgi, 'success');
                listele(s.kod);
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    $('#btnSil').on('click', function () {
        var kod = parseInt($('#txtKod').val(), 10) || 0;
        if (kod <= 0) { mesaj(M.FRMDYG021, 'warning'); return; }
        if (!window.confirm(M.SilOnay)) return;
        mesgul(true);
        Sgb.gonder('api/duyuru/sil', { kod: kod })
            .done(function (s) {
                if (!s.basarili) { mesaj(s.hata, 'danger'); return; }
                temizle();
                mesaj(s.bilgi, 'success');
                listele();
            })
            .fail(hataGoster)
            .always(function () { mesgul(false); });
    });

    // ---------------------------------------------------------------- açılış

    Sgb.al('api/duyuru/tanimlar')
        .done(function (t) {
            tanimlar = t;
            $.each(t.onemler, function (i, o) {
                onemAd[o.kod] = o.ad;
                $('<option></option>').val(o.kod).text(o.ad).appendTo('#ddlOnem');
                $('<option></option>').val(o.kod).text(o.ad).appendTo('#aramaOnem');
            });
            $.each(t.kullaniciTipleri, function (i, k) {
                $('<option></option>').val(k.kod).text(k.ad).appendTo('#lstKullaniciTipi');
            });
            temizle();
            listele();
        })
        .fail(hataGoster);
});
