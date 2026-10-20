using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Bütçe İşlemleri Düzeltme (eski ButceMuhasebe/SGBGondIslemDuzeltme.aspx): ödenek gönderme / tenkis belgeleri ile masraf planı
    /// bütçe işlemleri arasındaki çift kayıtları ve tutarsızlıkları listeler; satır satır ya da seçili satırlar için bağı koparır
    /// ya da bütçe işlemini üretir / yazar. İşlemler wwwroot/js/sgbgondislem.js ile /api/sgbgondislem uçlarından
    /// (Muhasebe/SgbGondIslemUclari.cs) yapılır.
    /// </summary>
    public class SGBGondIslemDuzeltmeModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public SGBGondIslemDuzeltmeModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = SgbGondIslemUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
