using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;

namespace Istemci.InsanKaynaklari
{
    /// <summary>
    /// Eski InsanKaynaklari/IKYIstemciServis.aspx (ekranı yok). Sunucuda çalışan Windows servisi bu adresi çağırır;
    /// adres ve parametreler eskisiyle aynı kalır (.aspx uzantısı IstemciBaslangic'te atılır):
    ///   GET/POST /InsanKaynaklari/IKYIstemciServis.aspx?ekParam=TarimAktar&amp;p1=TC
    ///     ekParam=TarimAktar  IIKYServis.TarimVerileriniAktar(p1); p1 boşsa sıralı aktarım
    /// Eski sayfada kul, par, t parametreleri okunuyor ama kullanılmıyordu.
    /// Yanıt eski Response.Write ile aynı metin: "Sonuç Hata: .. Sonuç Bilgi: ..".
    /// Giriş istemez; yalnızca sunucunun kendisinden (aynı makine) gelen isteklere yanıt verir.
    /// </summary>
    public static class IKYIstemciServisUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapMethods("/InsanKaynaklari/IKYIstemciServis", new[] { "GET", "POST" }, Calistir).AllowAnonymous();
        }

        // RequestDelegate olarak bağlanır (yalnızca HttpContext alıyor); dönüş değeri yazılmayacağı için yanıt doğrudan yazılır.
        private static async Task Calistir(HttpContext baglam)
        {
            if (!YerelIstek(baglam))
            {
                baglam.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            // Eski Response.Cache.SetExpires(DateTime.Now)
            baglam.Response.Headers.CacheControl = "no-cache, no-store";

            string ekParam = await Oku(baglam, "ekParam");
            string tc = await Oku(baglam, "p1");

            string yanit = "";
            if (ekParam == "TarimAktar")
            {
                Sonuc sonuc = TNS.IKY.Arac.Tanimla().TarimVerileriniAktar(tc);
                yanit = $"Sonuç Hata: {sonuc.hataStr} Sonuç Bilgi: {sonuc.bilgiStr}";
            }
            baglam.Response.ContentType = "text/html; charset=utf-8";
            await baglam.Response.WriteAsync(yanit);
        }

        /// <summary>Eski Request[ad]: önce sorgu dizisi, sonra form.</summary>
        private static async Task<string> Oku(HttpContext baglam, string ad)
        {
            string deger = baglam.Request.Query[ad].ToString();
            if (string.IsNullOrEmpty(deger) && baglam.Request.HasFormContentType)
            {
                var form = await baglam.Request.ReadFormAsync();
                deger = form[ad].ToString();
            }
            return deger ?? "";
        }

        /// <summary>İstek sunucunun kendisinden mi: geri döngü (127.0.0.1, ::1) ya da makinenin kendi adreslerinden biri.</summary>
        private static bool YerelIstek(HttpContext baglam)
        {
            IPAddress uzak = baglam.Connection.RemoteIpAddress;
            if (uzak == null) return false;
            if (uzak.IsIPv4MappedToIPv6) uzak = uzak.MapToIPv4();
            if (IPAddress.IsLoopback(uzak)) return true;

            IPAddress yerel = baglam.Connection.LocalIpAddress;
            if (yerel != null)
            {
                if (yerel.IsIPv4MappedToIPv6) yerel = yerel.MapToIPv4();
                if (uzak.Equals(yerel)) return true;
            }

            try
            {
                foreach (IPAddress a in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    IPAddress b = a.IsIPv4MappedToIPv6 ? a.MapToIPv4() : a;
                    if (uzak.Equals(b)) return true;
                }
            }
            catch (SocketException) { }
            return false;
        }
    }
}
