using Istemci.InsanKaynaklari;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel Maaş Bilgileri Toplu Düzenleme (eski InsanKaynaklari/ListePersonelMaasParam.aspx): kriterlere göre personeller ve
    /// maaş parametreleri tek tabloda; Kişilere Ata, Kaydet, Excel'den Dosya Yükle. İşlemler wwwroot/js/maasparamtoplu.js ile
    /// /api/maasparamtoplu uçlarından yapılır.
    /// </summary>
    public class ListePersonelMaasParamModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public ListePersonelMaasParamModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = ListePersonelMaasParamUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
