using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Kalkınma Bölgeleri (eski ButceHazirlikv2/KalkinmaBolgeleriTanimForm.aspx): bölge kodu ve adı ile bölgedeki iller.
    /// Oku bölge koduna göre okur, Kaydet bölgeyi ve illerini yazar. İşlemler wwwroot/js/kalkinmabolge.js ile
    /// /api/kalkinmabolge uçlarından yapılır.
    /// </summary>
    public class KalkinmaBolgeleriTanimFormModel : ButceTanimSayfaModel
    {
        public KalkinmaBolgeleriTanimFormModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string[] Adresler => new[] { KalkinmaBolgeUclari.SayfaAdresi };
    }
}
