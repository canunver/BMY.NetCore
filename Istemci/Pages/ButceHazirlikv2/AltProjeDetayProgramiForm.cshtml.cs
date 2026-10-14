using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Alt proje detay programı (eski ButceHazirlikv2/AltProjeDetayProgramiForm.aspx): düzenlenebilir tablo, Oku, Kaydet,
    /// Yazdır, satır işlemleri, Yapıştır ve Önceki Aşamadan Al. AltProjeDetay sekmesinde iframe olarak açılır.
    /// İşlemler wwwroot/js/yatalt-program.js ile /api/yatalt/program uçlarından.
    /// </summary>
    public class AltProjeDetayProgramiFormModel : YatAltSayfaModel
    {
        public AltProjeDetayProgramiFormModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "AltProjeDetayProgramiForm";
    }
}
