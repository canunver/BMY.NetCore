using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Istemci.Giris;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TNS.Api;

namespace Istemci
{
    /// <summary>
    /// Web uygulamasının kurulumu. Çalıştırılabilir proje (SGB.Net) yalnızca buradaki üç metodu sırayla çağırır;
    /// uygulamaya eklenen her yeni özellik burada tanımlanır, exe projesi değişmez.
    /// </summary>
    public static class IstemciBaslangic
    {
        /// <summary>
        /// Girişten sonra açılan sayfa (uygulama köküne göre). Eski yapıdaki gibi giriş sayfası kökte, diğer bütün sayfalar
        /// bir alt klasördedir (OrtakSayfa, TasinirMal ...); böylece sayfalar birbirine ../Modul/Sayfa biçiminde başvurabilir.
        /// </summary>
        public const string AcilisSayfasi = "OrtakSayfa/GirisBilgi";

        /// <summary>Ayar dosyası, kültür ve kod sayfaları. WebApplication.CreateBuilder'dan önce çağrılır.</summary>
        public static void OrtamiHazirla()
        {
            // Eski kod ayarlarını ConfigurationManager.AppSettings ile okuyor; ayarlar exe'nin App.config dosyasından
            // (derleme çıktısında SGB.Net.dll.config) gelir.
            Assembly giris = Assembly.GetEntryAssembly();
            if (giris != null)
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", Path.Combine(AppContext.BaseDirectory, giris.GetName().Name + ".dll.config"));

            // Eski web.config'deki <globalization culture="tr-TR"> karşılığı
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            CultureInfo kultur = new CultureInfo("tr-TR");
            CultureInfo.DefaultThreadCurrentCulture = kultur;
            CultureInfo.DefaultThreadCurrentUICulture = kultur;
        }

        public static void ServisleriEkle(WebApplicationBuilder builder)
        {
            IServiceCollection servisler = builder.Services;

            // Sayfalar bu kitaplığın içinden gelir; exe projesi Istemci'ye başvurduğu için kendiliğinden bulunur.
            servisler.AddRazorPages();
            servisler.AddMemoryCache();

            // Giriş çerezini şifreleyen anahtarlar sabit bir klasörde tutulur; yoksa IIS'te uygulama her yeniden
            // başladığında anahtar değişebilir ve oturum veri tabanında dursa bile çerezler geçersiz kalır.
            // appSettings: AnahtarKlasoru (varsayılan: uygulama klasörü\App_Data\anahtarlar)
            string anahtarKlasoru = Ayar.Oku("AnahtarKlasoru", Path.Combine(builder.Environment.ContentRootPath, "App_Data", "anahtarlar"));
            Directory.CreateDirectory(anahtarKlasoru);
            servisler.AddDataProtection()
                .SetApplicationName("SGB.Net")
                .PersistKeysToFileSystem(new DirectoryInfo(anahtarKlasoru));
            servisler.AddAntiforgery(o => o.HeaderName = "RequestVerificationToken");

            servisler
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.Cookie.Name = "SGB.Oturum";
                    o.Cookie.HttpOnly = true;
                    o.Cookie.SameSite = SameSiteMode.Lax;
                    o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    o.LoginPath = "/Giris";
                    o.AccessDeniedPath = "/Giris";
                    o.ExpireTimeSpan = OturumBilgisi.Sure;
                    o.SlidingExpiration = true;

                    // Çerez geçerli olsa bile sunucuda oturum kaydı kalmamışsa (süre doldu, uygulama yeniden başladı) giriş istenir.
                    o.Events.OnValidatePrincipal = baglam =>
                    {
                        if (OturumBilgisi.KullaniciOku(baglam.HttpContext, baglam.Principal) == null)
                            baglam.RejectPrincipal();
                        return Task.CompletedTask;
                    };

                    // JavaScript çağrıları giriş sayfasına yönlendirilmez; 401 alır.
                    o.Events.OnRedirectToLogin = baglam =>
                    {
                        if (baglam.Request.Path.StartsWithSegments("/api"))
                            baglam.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        else
                            baglam.Response.Redirect(baglam.RedirectUri);
                        return Task.CompletedTask;
                    };
                });

            // Aksi belirtilmedikçe ([AllowAnonymous]) bütün sayfalar ve uçlar giriş ister.
            servisler.AddAuthorization(o =>
            {
                o.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });
        }

        public static void Kullan(WebApplication app)
        {
            // Arac.Tanimla() çağrıları doğrudan iş kuralları sınıflarını döndürsün.
            ServisKayit.Varsayilanlar();

            // Eski adreslerle uyum: menü tanımları ve yer imleri Sayfa.aspx biçiminde; uzantı atılır.
            // ../TasinirMal/ListeMarka.aspx?x=1 -> ../TasinirMal/ListeMarka?x=1
            app.Use((baglam, sonraki) =>
            {
                string yol = baglam.Request.Path.Value;
                if (yol != null && yol.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
                    baglam.Request.Path = yol.Substring(0, yol.Length - 5);
                return sonraki(baglam);
            });

            // Bulunamayan sayfalar (ör. henüz taşınmamış bir bilgi ekranı) boş yanıt yerine kısa bir açıklama gösterir.
            app.UseStatusCodePages(async durum =>
            {
                HttpContext baglam = durum.HttpContext;
                if (baglam.Response.StatusCode != StatusCodes.Status404NotFound || baglam.Request.Path.StartsWithSegments("/api")) return;
                baglam.Response.ContentType = "text/html; charset=utf-8";
                await baglam.Response.WriteAsync("<!DOCTYPE html><html lang=\"tr\"><head><meta charset=\"utf-8\"><title>Sayfa bulunamadı</title></head>"
                    + "<body style=\"font-family:sans-serif;color:#5a6773;padding:16px\">Bu sayfa bulunamadı ya da henüz yeni sisteme taşınmadı.</body></html>");
            });

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            GirisUclari.Ekle(app);
            GirisBilgiUclari.Ekle(app);
        }
    }
}
