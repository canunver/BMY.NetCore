using Istemci.EkliDosya;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.Duyuru
{
    /// <summary>
    /// Ekli dosyalar (eski Duyuru/EkliDosyalarForm.aspx). Eski adresler aynen çalışır:
    ///   ?mdl=DUY&amp;frm=12        : dosya listesi (çerçeve içinde ?menuYok=1 ile kullanılabilir; brm=1 birim adı sütunu)
    ///   ?no=5&amp;mdl=DUY&amp;frm=12   : dosyanın kendisi (duyuru metinlerine konan "Dosya Bağlantı Yolu" adresleri)
    /// Liste wwwroot/js/eklidosya.js bileşeniyle çizilir.
    /// </summary>
    public class EkliDosyalarFormModel : PageModel
    {
        public string Modul { get; private set; }
        public string Form { get; private set; }
        public bool BirimAd { get; private set; }

        public IActionResult OnGet(string mdl, string frm, int? no, string brm)
        {
            if (no != null)
            {
                IResult dosya = EkliDosyaUclari.Indir(HttpContext, mdl, frm, no, "0");
                return new SonucEylemi(dosya);
            }
            Modul = mdl ?? "";
            Form = frm ?? "";
            BirimAd = brm == "1";
            return Page();
        }

        /// <summary>Minimal API sonucunu (IResult) Razor sayfasından döndürmek için.</summary>
        private sealed class SonucEylemi : IActionResult
        {
            private readonly IResult sonuc;
            public SonucEylemi(IResult sonuc) { this.sonuc = sonuc; }
            public System.Threading.Tasks.Task ExecuteResultAsync(ActionContext baglam) { return sonuc.ExecuteAsync(baglam.HttpContext); }
        }
    }
}
