using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri sekmesi: SGB kullanıcı işlemleri (eski InsanKaynaklari/PersonelKullaniciIslemleri.aspx): kullanıcı oluşturma, parola, roller / görevler.
    /// İçerik wwwroot/js ile /api/personelbilgi uçlarından gelir (PersonelKullaniciIslemleriUclari).
    /// </summary>
    public class PersonelKullaniciIslemleriModel : PersonelAltSayfaModel
    {
        public PersonelKullaniciIslemleriModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelKullaniciIslemleri";
    }
}
