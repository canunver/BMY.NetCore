using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Projeye eklenen dosyalar (eski ButceHazirlikv2/ProjeDosyaEkle.aspx): liste, İndir, Sil, Yeni Dosya Ekle.
    /// ProjeDetay / AltProjeDetay sekmesinde iframe olarak açılır. İşlemler wwwroot/js/yatalt-dosya.js ile /api/yatalt/dosya uçlarından.
    /// </summary>
    public class ProjeDosyaEkleModel : YatAltSayfaModel
    {
        public ProjeDosyaEkleModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "ProjeDosyaEkle";
    }
}
