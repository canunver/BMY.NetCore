using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// İcmal belgesi düzenleme (eski ButceMuhasebe/OdenekGondermeIcmalForm.aspx): yıl, belge türü, icmal no, kurumsal kod, kayıt
    /// tarihi, icmale ödeme emri ekleyebilecek kullanıcılar; Oku / Kaydet / Yazdır. İşlemler wwwroot/js/odenekgondermeicmal.js ile
    /// /api/odenekgondermeicmal uçlarından yapılır.
    /// </summary>
    public class OdenekGondermeIcmalFormModel : OdenekGondermeIcmalSayfaModel
    {
        public OdenekGondermeIcmalFormModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAdres { get { return OdenekGondermeIcmalUclari.FormAdres; } }
    }
}
