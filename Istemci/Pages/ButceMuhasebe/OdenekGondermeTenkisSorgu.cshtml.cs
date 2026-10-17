using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Ödenek Gönderme / Tenkis belge arama (eski ButceMuhasebe/OdenekGondermeTenkisSorgu.aspx): süzgeçler, belge listesi,
    /// liste yazdır, liste / icmal / diğer işlemler ve belge tarihçesi. İşlemler wwwroot/js/odenekgondermeliste.js ile
    /// /api/odenekgonderme uçlarından yapılır.
    /// </summary>
    public class OdenekGondermeTenkisSorguModel : OdenekGondermeSayfaModel
    {
        public OdenekGondermeTenkisSorguModel(IWebHostEnvironment ortam) : base(ortam) { }
    }
}
