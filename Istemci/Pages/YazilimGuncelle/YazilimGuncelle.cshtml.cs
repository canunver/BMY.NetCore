using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Istemci.Guncelleme;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.YazilimGuncelle
{
    /// <summary>
    /// Yazılım Güncelle (eski YazilimGuncelle/YazilimGuncelle.aspx): yüklenen Zip dosyasına göre uygulama klasöründeki değişen dosyalar
    /// güncellenir. İşlem wwwroot/js/yazilimguncelle.js ile /api/guncelleme/yazilim-zip ucundan yapılır. Eski sayfadaki güncelleme
    /// adresinden (ListeVer.aspx / DosyaVer.aspx) güncelleme bölümü kullanıcı isteğiyle alınmadı.
    /// </summary>
    public class YazilimGuncelleModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public YazilimGuncelleModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        /// <summary>Eski formAdi: sunucu saati, saat dilimi, kod sayfası ve en son değişen kitaplık.</summary>
        public string Bilgi { get; private set; }

        public void OnGet()
        {
            Yetkili = GuncellemeOrtak.YazilimGuncelleYetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
            if (!Yetkili) return;
            DateTime simdi = DateTime.Now;
            Bilgi = simdi.ToLongTimeString() + " " + TimeZoneInfo.Local.DisplayName + " / " + Encoding.Default.CodePage + " " + SonSaat();
        }

        /// <summary>Uygulama klasöründeki en son değiştirilen kitaplık (dll) ve tarihi.</summary>
        private static string SonSaat()
        {
            try
            {
                DirectoryInfo klasor = new DirectoryInfo(AppContext.BaseDirectory);
                DateTime enBuyuk = DateTime.MinValue;
                string dosyaAd = "";
                foreach (FileInfo f in klasor.GetFiles("*.dll"))
                {
                    if (enBuyuk < f.LastWriteTime)
                    {
                        enBuyuk = f.LastWriteTime;
                        dosyaAd = f.Name;
                    }
                }
                return string.IsNullOrWhiteSpace(dosyaAd) ? "" : dosyaAd + " (" + enBuyuk.ToString("dd.MM.yy HH:mm") + ")";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
