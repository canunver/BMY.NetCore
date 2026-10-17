using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Programlar / Alt Programlar / Faaliyetler / Alt Faaliyetler (eski ButceHazirlikv2/ProgButKod.aspx?tur=1..4): yılın program
    /// bütçe kodları; bulma, ekleme / düzeltme / silme (pencerede), önceki yıldan alma, program bütçe sistemiyle (SBB) karşılaştırma
    /// ve Excel'e yazdırma. Tür adres satırından gelir; menüde her tür ayrı seçenektir. İşlemler wwwroot/js/progbutkod.js ile
    /// /api/progbutkod uçlarından yapılır.
    /// </summary>
    public class ProgButKodModel : ButceTanimSayfaModel
    {
        public ProgButKodModel(IWebHostEnvironment ortam) : base(ortam) { }

        public int Tur { get; private set; }
        public string KodAdi => ProgButKodUclari.KodAdi(Tur);

        protected override string[] Adresler => Tur >= 1 && Tur <= 4 ? new[] { ProgButKodUclari.Adres(Tur) } : new string[0];

        public override void OnGet()
        {
            Tur = int.TryParse(Request.Query["tur"].ToString(), out int tur) ? tur : 0;
            base.OnGet();
        }
    }
}
