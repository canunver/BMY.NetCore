using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Karakteristikler (eski ButceHazirlikv2/KarakteristikForm.aspx): yatırım projelerinde seçilen karakteristik değerlerinin
    /// tanımları (sıra, karakteristik, kısa adı, ek değer girişi). Tabloda düzenlenir, Kaydet bütün listeyi yazar.
    /// İşlemler wwwroot/js/karakteristik.js ile /api/karakteristik uçlarından yapılır.
    /// </summary>
    public class KarakteristikFormModel : ButceTanimSayfaModel
    {
        public KarakteristikFormModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string[] Adresler => new[] { KarakteristikUclari.SayfaAdresi };
    }
}
