using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri "Mal Bildirimi" sekmesi (eski InsanKaynaklari/PersonelMalBildirimi.aspx): solda bildirimin bilgileri ve
    /// dokuz tablo (kimlik, taşınmaz, kooperatif, taşıt, diğer taşınır, banka/menkul, altın/mücevher, borç/alacak, diğer servet),
    /// sağda personelin mal bildirimleri. İşlemler wwwroot/js/personel-malbildirim.js ile /api/personelbilgi/malbildirim uçlarından yapılır.
    /// </summary>
    public class PersonelMalBildirimiModel : PersonelAltSayfaModel
    {
        public PersonelMalBildirimiModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "PersonelMalBildirimi";
    }
}
