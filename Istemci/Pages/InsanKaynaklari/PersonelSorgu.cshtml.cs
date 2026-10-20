using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel arama penceresi (eski InsanKaynaklari/PersonelSorgu.aspx): Kart sekmesinden Sgb.pencere ile (?menuYok=1&amp;tip=1|2) açılır;
    /// genel arama, birim ve unvana göre personel listelenir, tıklanan personel açan sayfaya bildirilir ve pencere kapanır.
    /// İşlemler wwwroot/js/personel-sorgu.js ile /api/personelbilgi/personelsorgu uçlarından yapılır.
    /// </summary>
    public class PersonelSorguModel : PersonelAltSayfaModel
    {
        public PersonelSorguModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "PersonelSorgu";

        /// <summary>Seçimin hangi alana yazılacağı: 1 = zimmet eden, 2 = iade alan (eski QueryString["tip"]).</summary>
        public int Tip { get; private set; }

        public override void OnGet()
        {
            base.OnGet();
            int.TryParse(Request.Query["tip"], out int tip);
            Tip = tip;
        }
    }
}
