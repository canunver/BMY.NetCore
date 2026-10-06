using System;
using System.Configuration;
using Microsoft.AspNetCore.Http;

namespace Istemci
{
    /// <summary>Ayar dosyası ve istek bilgileri için küçük yardımcılar (eski GenelIslemler'deki karşılıkları).</summary>
    public static class Ayar
    {
        /// <summary>appSettings değerini okur (eski GenelIslemler.WebConfigOku).</summary>
        public static string Oku(string anahtar, string varsayilan = "")
        {
            string deger = ConfigurationManager.AppSettings.Get(anahtar);
            return string.IsNullOrEmpty(deger) ? varsayilan : deger;
        }

        /// <summary>Sistem değişkenini okur; değişken servisi çalışmıyorsa varsayılanı döndürür.</summary>
        public static string Degisken(int yil, string ad, string varsayilan = "")
        {
            try
            {
                string deger = TNS.DEG.Arac.DegiskenDegerBul(yil, ad);
                return string.IsNullOrEmpty(deger) ? varsayilan : deger;
            }
            catch (Exception)
            {
                return varsayilan;
            }
        }

        /// <summary>İstemcinin IP adresi (eski GenelIslemler.GetIPAddress): varsa X-Forwarded-For'un ilk adresi, yoksa bağlantı adresi.</summary>
        public static string IPAdresi(HttpContext baglam)
        {
            string ip = baglam.Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrWhiteSpace(ip))
                ip = ip.Split(',')[0].Trim();
            if (string.IsNullOrWhiteSpace(ip) || ip.Length < 3)
                ip = baglam.Connection.RemoteIpAddress != null ? baglam.Connection.RemoteIpAddress.ToString() : "";
            return ip;
        }
    }
}
