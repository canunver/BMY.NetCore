using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Alt Faaliyet Bağları (eski ButceHazirlikv2/ProgPebIliski.aspx): performans bütçe alt faaliyet kodlarının program, alt program,
    /// faaliyet, alt faaliyet ve fonksiyonel kodlarla ilişkisi. Listeleme, pencerede düzeltme / silme, önceki yıldan kopyalama ve
    /// Excel'e yazdırma. İşlemler wwwroot/js/progpebiliski.js ile /api/progpebiliski uçlarından yapılır.
    /// </summary>
    public class ProgPebIliskiModel : ButceTanimSayfaModel
    {
        public ProgPebIliskiModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string[] Adresler => new[] { ProgPebIliskiUclari.SayfaAdresi };
    }
}
