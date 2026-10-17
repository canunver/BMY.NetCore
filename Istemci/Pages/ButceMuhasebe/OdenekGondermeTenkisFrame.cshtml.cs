using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Ödenek Gönderme / Tenkis sayfalarının ortak modeli: belge türü (belgetur: 3 ödenek gönderme, 4 tenkis) ve talep adres
    /// satırından okunur; yetki menüdeki çerçeve adresinden gelir (Muhasebe/OdenekGondermeUclari.YetkiVar).
    /// </summary>
    public abstract class OdenekGondermeSayfaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        protected OdenekGondermeSayfaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public int BelgeTur { get; private set; }
        public bool Talep { get; private set; }
        public bool BelgeTurVar { get; private set; }
        public bool Yetkili { get; private set; }

        public string Baslik
        {
            get
            {
                if (BelgeTur == OdenekGondermeUclari.OdenekGondermeTuru)
                    return Talep ? "Ödenek Gönderme Belgesi İşlemleri (Talep)" : "Ödenek Gönderme Belgesi İşlemleri";
                if (BelgeTur == OdenekGondermeUclari.TenkisTuru)
                    return Talep ? "Tenkis Belgesi İşlemleri (Talep)" : "Tenkis Belgesi İşlemleri";
                return "Ödenek Gönderme / Tenkis";
            }
        }

        /// <summary>Alt sayfaların adresine eklenecek parametreler.</summary>
        public string Parametreler
        {
            get { return "belgetur=" + BelgeTur + (Talep ? "&talep=1" : ""); }
        }

        public void OnGet()
        {
            BelgeTur = OrtakFonksiyonlar.ConvertToInt((string)Request.Query["belgetur"], 0);
            Talep = OdenekGondermeUclari.TalepMi(Request.Query["talep"]);
            BelgeTurVar = BelgeTur == OdenekGondermeUclari.OdenekGondermeTuru || BelgeTur == OdenekGondermeUclari.TenkisTuru;
            Yetkili = BelgeTurVar && OdenekGondermeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, BelgeTur, Talep);
        }
    }

    /// <summary>
    /// Ödenek Gönderme / Tenkis ana çerçevesi (eski ButceMuhasebe/OdenekGondermeTenkisFrame.aspx): "Belge Düzenleme" (Form,
    /// OdenekGondermeTenkisMPExt) ve "Belge Arama" (Liste, OdenekGondermeTenkisSorgu) sekmeleri. Menüdeki dört seçenek
    /// belgetur=3|4 ve talep=1 ile açılır.
    /// </summary>
    public class OdenekGondermeTenkisFrameModel : OdenekGondermeSayfaModel
    {
        public OdenekGondermeTenkisFrameModel(IWebHostEnvironment ortam) : base(ortam) { }
    }
}
