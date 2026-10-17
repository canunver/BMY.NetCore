using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Bütçe Sözlük İşlemleri (eski ButceHazirlikv2/ButceSozlukYonetim.aspx): seçilen yılın ve önceki yılın sınıflandırma kodu,
    /// tertip, hizmet gerekçesi ve bütçe fişi sayıları; boş yılı önceki yıldan kopyalama, ilgili tanım sayfasını açma. İşlemler wwwroot/js/butcesozluk.js ile /api/butcesozluk uçlarından yapılır.
    /// </summary>
    public class ButceSozlukYonetimModel : ButceTanimSayfaModel
    {
        public ButceSozlukYonetimModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string[] Adresler => new[] { ButceSozlukUclari.SayfaAdresi };
    }
}
