using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// BÜMKO Aktarım (eski ButceHazirlikv2/BUMKOVeriGonder.aspx): bütçe hazırlık verisinin (program bütçe ödenekleri, bütçe fişi /
    /// ek ödenek gerekçeleri, faaliyet gerekçeleri) SBB program bütçe sistemine gönderilmesi ve SBB - SGB ödenek kontrol raporu.
    /// İşlemler wwwroot/js/bumko.js ile /api/bumko uçlarından yapılır.
    /// </summary>
    public class BUMKOVeriGonderModel : ButceTanimSayfaModel
    {
        public BUMKOVeriGonderModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string[] Adresler => new[] { BumkoAktarimUclari.SayfaAdresi };

        /// <summary>Ekonomik kod seçenekleri (eski ddlEko / ddlEkoGerekce).</summary>
        public static readonly string[] EkoKodlar =
        {
            "01", "02", "03", "03.01", "03.02", "03.03", "03.04", "03.05", "03.06", "03.07", "03.08", "03.09",
            "04", "05", "06", "06.01", "06.02", "06.03", "06.04", "06.05", "06.06", "06.07", "06.08", "06.09", "07", "08", "09"
        };
    }
}
