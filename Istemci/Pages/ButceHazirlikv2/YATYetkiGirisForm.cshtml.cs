using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Proje yetkili kullanıcıları (eski ButceHazirlikv2/YATYetkiGirisForm.aspx): liste, Kullanıcı Seç, Seçilileri Sil, Kaydet.
    /// ProjeDetay / AltProjeDetay sekmesinde iframe olarak açılır. İşlemler wwwroot/js/yatalt-yetki.js ile /api/yatalt/yetki uçlarından.
    /// </summary>
    public class YATYetkiGirisFormModel : YatAltSayfaModel
    {
        public YATYetkiGirisFormModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "YATYetkiGirisForm";
    }
}
