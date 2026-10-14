using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Alt proje bütçe kaynağı giriş (eski ButceHazirlikv2/AltProjeButceKaynakGiris.aspx): AltProjeDetay'ın kaynak sekmesinde
    /// çerçeve olarak açılır (?menuYok=1&amp;yil=&amp;senaryoKod=&amp;altProjeKodu=&amp;kaynakNo=). Kaynak bilgileri, teklif, harcama,
    /// alt faaliyet, kredi/hibe planı, kredi geri ödeme ve proje teklif rakamları wwwroot/js/yatkaynak.js ile /api/yatkaynak
    /// uçlarından okunur ve kaydedilir.
    /// </summary>
    public class AltProjeButceKaynakGirisModel : YatAltSayfaModel
    {
        public AltProjeButceKaynakGirisModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "AltProjeButceKaynakGiris";
    }
}
