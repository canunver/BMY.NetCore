using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Ödenek Gönderme / Tenkis belge düzenleme (eski ButceMuhasebe/OdenekGondermeTenkisMPExt.aspx): belge bilgileri, tertip
    /// satırları, kaydet / talep gönder / yazdır / hızlı rapor / revize iptal, imzalı ve destekleyici belge, belgeye doküman ekle.
    /// İşlemler wwwroot/js/odenekgonderme.js ile /api/odenekgonderme uçlarından (Muhasebe/OdenekGondermeUclari.cs) yapılır.
    /// </summary>
    public class OdenekGondermeTenkisMPExtModel : OdenekGondermeSayfaModel
    {
        public OdenekGondermeTenkisMPExtModel(IWebHostEnvironment ortam) : base(ortam) { }
    }
}
