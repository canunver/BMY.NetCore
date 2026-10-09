// Ortak HTML editörü (Jodit 4, wwwroot/lib/jodit). Biçimlendirilmiş metin girilen bütün sayfalar bunu kullanır.
// Sayfa şunları ekler:
//   <link rel="stylesheet" href="~/lib/jodit/jodit.min.css" />
//   <script src="~/lib/jodit/jodit.min.js"></script>  <script src="~/js/editor.js"></script>
// Kullanım (öğe bir <textarea> olmalı):
//   var ed = Sgb.editor('#txtEditor', { yukseklik: 320 });
//   ed.html('<p>...</p>');   ed.html();   ed.bos();   ed.odaklan();
// Biçimler (yazı tipi, boyut, renk, tablo) satır içi stil olarak yazılır; metin editör dışında da aynı görünür.
window.Sgb = window.Sgb || {};
Sgb.editor = function (element, ayarlar) {
    'use strict';

    ayarlar = ayarlar || {};
    var el = typeof element === 'string' ? document.querySelector(element) : element;
    var dil = (document.documentElement.lang || 'tr').substring(0, 2);

    var dugmeler = ayarlar.dugmeler || [
        'bold', 'italic', 'underline', 'strikethrough', '|',
        'font', 'fontsize', 'brush', '|',
        'ul', 'ol', 'align', 'indent', 'outdent', '|',
        'table', 'link', 'hr', '|',
        'eraser', 'undo', 'redo', '|',
        'source', 'fullsize'
    ];

    var editor = Jodit.make(el, {
        language: dil,
        height: ayarlar.yukseklik || 320,
        minHeight: 160,
        buttons: dugmeler,
        buttonsMD: dugmeler,
        buttonsSM: dugmeler,
        buttonsXS: dugmeler,
        toolbarAdaptive: false,
        showCharsCounter: false,
        showWordsCounter: false,
        showXPathInStatusbar: false,
        // Resim ve dosya yükleme sunucu tarafı olmadan çalışmaz; kapalı.
        uploader: { insertImageAsBase64URI: false },
        // Başka yerden (Word vb.) yapıştırırken soru sorulmaz; gereksiz biçimler temizlenir.
        askBeforePasteHTML: false,
        askBeforePasteFromWord: false,
        defaultActionOnPaste: 'insert_clear_html',
        link: { openInNewTabCheckbox: true, noFollowCheckbox: false },
        placeholder: ayarlar.yerTutucu || ''
    });

    function html(deger) {
        if (arguments.length) {
            editor.value = deger || '';
            if (editor.history) editor.history.clear();
            return;
        }
        return bos() ? '' : editor.value;
    }

    function bos() {
        var kok = editor.editor;
        return (kok.textContent || '').trim() === '' && !kok.querySelector('img, table, hr');
    }

    return {
        jodit: editor,
        html: html,
        bos: bos,
        odaklan: function () { editor.s.focus(); }
    };
};
