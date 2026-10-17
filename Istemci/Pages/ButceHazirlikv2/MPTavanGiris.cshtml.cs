using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Tavan Girişi (eski ButceHazirlikv2/MPTavanGiris.aspx; menüde yok, Masraf Planı Tavan ve İl / İlçe Ödenek Dağıtım ekranlarından
    /// pencerede açılır). ?yil=&amp;senaryo=&amp;tertip=&amp;DG=&amp;kykod=&amp;saybir=. DG=G il tavanı: tertibin ödeneği birimlere dağıtılır;
    /// DG=D merkez detay: seçilen kurumsal yapının tavanı harcama birimlerine dağıtılır (Excel'den yükleme de yapılır); DG=I il girişi.
    /// Yetki açan sayfadan gelir (DG=I MPTavanIlAna, diğerleri MPTavanAna). İşlemler wwwroot/js/mptavangiris.js ile /api/mptavan/giris uçlarından.
    /// </summary>
    public class MPTavanGirisModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MPTavanGirisModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public MPTavanGirisParam Param { get; private set; }

        public void OnGet()
        {
            Param = new MPTavanGirisParam
            {
                Yil = OrtakFonksiyonlar.ConvertToInt((string)Request.Query["yil"], 0),
                Senaryo = ((string)Request.Query["senaryo"] ?? "").Trim(),
                Tertip = ((string)Request.Query["tertip"] ?? "").Trim(),
                DG = ((string)Request.Query["DG"] ?? "").Trim(),
                KyKod = ((string)Request.Query["kykod"] ?? "").Trim(),
                SayBir = ((string)Request.Query["saybir"] ?? "").Trim()
            };
            Yetkili = MPTavanUclari.GirisYetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, Param.DG);
        }
    }
}
