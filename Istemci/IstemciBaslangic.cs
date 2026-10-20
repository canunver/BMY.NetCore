using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Istemci.Duyuru;
using Istemci.EkliDosya;
using Istemci.Guncelleme;
using Istemci.Giris;
using Istemci.Kullanicilar;
using Istemci.Muhasebe;
using Istemci.ButceKod;
using Istemci.ButceHazirlik;
using Istemci.ButceForm;
using Istemci.KesinHesap;
using Istemci.Organizasyon;
using Istemci.StratejikPlan;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
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
            // Sayfaların bulunması normalde exe projesine derlemede eklenen [ApplicationPart("Istemci")] satırına bağlıdır;
            // yarım kalan bir derlemeden sonra o satır üretilmeyebiliyor ve hiçbir sayfa bulunmayınca /Giris kendine
            // yönlenip duruyor. Bu yüzden Istemci.dll burada açıkça da eklenir (zaten ekliyse dokunulmaz).
            Assembly sayfalar = typeof(IstemciBaslangic).Assembly;
            servisler.AddRazorPages().ConfigureApplicationPartManager(yonetici =>
            {
                string ad = sayfalar.GetName().Name;
                foreach (ApplicationPart parca in yonetici.ApplicationParts)
                    if (parca.Name == ad) return;
                foreach (ApplicationPart parca in ApplicationPartFactory.GetApplicationPartFactory(sayfalar).GetApplicationParts(sayfalar))
                    yonetici.ApplicationParts.Add(parca);
            });
            servisler.AddMemoryCache();

            // Ayda bir kez: 2 yıldan eski sayfa isteği kayıtlarını KULLANICI_BILGI_ARSIV tablosuna taşır.
            servisler.AddHostedService<KullaniciBilgiArsivIsi>();

            // JSON alan adları C#'taki gibi gider (Kod -> Kod, TERTIP -> TERTIP): büyük/küçük harf değişikliği yapılmaz.
            servisler.ConfigureHttpJsonOptions(secenek => secenek.SerializerOptions.PropertyNamingPolicy = null);
            servisler.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(secenek => secenek.JsonSerializerOptions.PropertyNamingPolicy = null);

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
            // İsteğin dili: oturumdaki kullanıcının dili ya da (giriş öncesi) KulDil çerezi.
            app.Use((baglam, sonraki) =>
            {
                Dil.Uygula(baglam);
                return sonraki(baglam);
            });
            app.UseAuthorization();
            // Oturumdaki kullanıcının açtığı sayfalar KULLANICI_BILGI tablosuna yazılır (eski KullaniciVeriTabaninaYaz).
            SayfaIstekKaydi.Kullan(app);

            app.MapRazorPages();
            GirisUclari.Ekle(app);
            GuvenlikKodu.Ekle(app);
            SifremiUnuttumUclari.Ekle(app);
            GirisBilgiUclari.Ekle(app);
            MenuUclari.Ekle(app);
            SikKullanilanUclari.Ekle(app);
            SayfaMenuUclari.Ekle(app);
            DuyuruUclari.Ekle(app);
            MesajPanosuUclari.Ekle(app);
            ParolaDegistirUclari.Ekle(app);
            EPostaUclari.Ekle(app);
            ResmiTatilUclari.Ekle(app);
            CalisanKullaniciUclari.Ekle(app);
            HataDosyaUclari.Ekle(app);
            SurumBilgiUclari.Ekle(app);
            ModulUclari.Ekle(app);
            KullaniciIslemTarihceUclari.Ekle(app);
            YardimUclari.Ekle(app);
            EkliDosyaUclari.Ekle(app);
            MuhasebeUclari.Ekle(app);
            Istemci.Muhasebe.OdenekDurumUclari.Ekle(app);
            Istemci.Muhasebe.OdenekGondermeUclari.Ekle(app);
            Istemci.Muhasebe.OdenekGondermeEkonomikUclari.Ekle(app);
            Istemci.Muhasebe.OdenekGondermeIcmalUclari.Ekle(app);
            Istemci.Muhasebe.MakbuzUclari.Ekle(app);
            Istemci.Muhasebe.BagliEkoPebUclari.Ekle(app);
            Istemci.Muhasebe.SgbKbsFarkUclari.Ekle(app);
            Istemci.Muhasebe.SgbGondIslemUclari.Ekle(app);
            KullaniciUclari.Ekle(app);
            GenelGorevUclari.Ekle(app);
            GorevAtaUclari.Ekle(app);
            OrgBirimUclari.Ekle(app);
            StratejikPlanUclari.Ekle(app);
            SPGostergeUclari.Ekle(app);
            SPDilUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelTipUclari.Ekle(app);
            Istemci.InsanKaynaklari.SozlukUclari.Ekle(app);
            Istemci.Kullanicilar.YerleskeOdaUclari.Ekle(app);
            Istemci.InsanKaynaklari.HizmetSebebiUclari.Ekle(app);
            Istemci.InsanKaynaklari.UnvanUclari.Ekle(app);
            Istemci.InsanKaynaklari.IzinTurUclari.Ekle(app);
            Istemci.InsanKaynaklari.TerfiUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelBilgiUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelKisiselUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelOrtakRaporlar.Ekle(app);
            Istemci.InsanKaynaklari.PersonelKadroUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelKadroOnaysizUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelAtamaUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelIzinUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelIzinDevirUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelIzinDevirListeUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelGorevUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelAileUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelAdresUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelMaasParamUclari.Ekle(app);
            Istemci.InsanKaynaklari.ListePersonelMaasParamUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelKullaniciIslemleriUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelGiderUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelOgrenimUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelDilUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelKursUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelCezaOdulUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelMalBildirimiUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelAskerlikUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelTarihceUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelKartUclari.Ekle(app);
            Istemci.InsanKaynaklari.RaporPersonelUclari.Ekle(app);
            Istemci.InsanKaynaklari.PersonelSecimUclari.Ekle(app);
            Istemci.InsanKaynaklari.SendikaBilgiUclari.Ekle(app);
            Istemci.InsanKaynaklari.IKYIstemciServisUclari.Ekle(app);
            Istemci.HarcamaSurecleri.BordroTanimUclari.Ekle(app);
            Istemci.HarcamaSurecleri.BordroTabloUclari.Ekle(app);
            Istemci.HarcamaSurecleri.BordroParamUclari.Ekle(app);
            Istemci.HarcamaSurecleri.MaasTemelBilgiUclari.Ekle(app);
            Istemci.HarcamaSurecleri.GelirVergisiUclari.Ekle(app);
            Istemci.HarcamaSurecleri.GundelikTanimUclari.Ekle(app);
            Istemci.HarcamaSurecleri.UnvanGundelikUclari.Ekle(app);
            IzlemeUclari.Ekle(app);
            KurumIzlemeUclari.Ekle(app);
            GrafikUclari.Ekle(app);
            KodTanimUclari.Ekle(app);
            ButceAsamaUclari.Ekle(app);
            TavanEkoSektorBirimUclari.Ekle(app);
            HizmetGerekcesiUclari.Ekle(app);
            FisGiderGerekceUclari.Ekle(app);
            FisGiderGerekceDetayUclari.Ekle(app);
            SayBulguUclari.Ekle(app);
            EkFormUclari.Ekle(app);
            TavanEkoFaaliyetUclari.Ekle(app);
            TavanTertipUclari.Ekle(app);
            TavanTertipYatirimSGBUclari.Ekle(app);
            GorusmeCariUclari.Ekle(app);
            TavanYatirimProjeUclari.Ekle(app);
            TavanYatirimVizeUclari.Ekle(app);
            TavanTertipYatirimVizeUclari.Ekle(app);
            TavanTertipCariSGBUclari.Ekle(app);
            ButceRakamUclari.Ekle(app);
            TertipTanimUclari.Ekle(app);
            TertipDetayUclari.Ekle(app);
            TavanUstuUclari.Ekle(app);
            TertipDonusumUclari.Ekle(app);
            ButceFisiUclari.Ekle(app);
            ButceFisiEskiUclari.Ekle(app);
            ButceFinansUclari.Ekle(app);
            ProjeGerekceUclari.Ekle(app);
            BirimButceKarsilamaUclari.Ekle(app);
            BirimButceHazirlikUclari.Ekle(app);
            MPTavanUclari.Ekle(app);
            MPTavanIlUclari.Ekle(app);
            MPDagitimUclari.Ekle(app);
            MasrafPlaniUclari.Ekle(app);
            MPAyarlaUclari.Ekle(app);
            MPRevizeTalepUclari.Ekle(app);
            MPRevizeUclari.Ekle(app);
            IOIProjeUclari.Ekle(app);
            YatProjeYonetimUclari.Ekle(app);
            YatProjeGirisUclari.Ekle(app);
            ButceCalismasiPersoneliUclari.Ekle(app);
            YollukPlanlamaUclari.Ekle(app);
            AracKiralamaUclari.Ekle(app);
            KiralikAracUclari.Ekle(app);
            DepremHarcamaUclari.Ekle(app);
            HizmetMaliyetiTasraUclari.Ekle(app);
            MuhasebeBirimUclari.Ekle(app);
            TertipSecimUclari.Ekle(app);
            YatIzlemeUclari.Ekle(app);
            YatAltSayfaUclari.Ekle(app);
            AltProjeButceKaynakUclari.Ekle(app);
            IlIlceUclari.Ekle(app);
            YatTopluVeriUclari.Ekle(app);
            FaaliyetRaporuUclari.Ekle(app);
            DeflatorUclari.Ekle(app);
            Istemci.ButceUygulama.GunlukNakitIhtiyaciUclari.Ekle(app);
            Istemci.ButceUygulama.APGirisUclari.Ekle(app);
            MaliDurumBeklentiUclari.Ekle(app);
            AltFaaliyetIliskiUclari.Ekle(app);
            YazilimGuncelleUclari.Ekle(app);
            DosyalarUclari.Ekle(app);
            KomutCalistirUclari.Ekle(app);
            SifreUretUclari.Ekle(app);
            VeritabaniIndexUclari.Ekle(app);
            Istemci.Raporlama.RaporGrupUclari.Ekle(app);
            Istemci.ButceHazirlik.YatSozlukUclari.Ekle(app);
            Istemci.ButceHazirlik.KarakteristikUclari.Ekle(app);
            Istemci.ButceHazirlik.KalkinmaBolgeUclari.Ekle(app);
            Istemci.ButceHazirlik.KurumsalYapiUclari.Ekle(app);
            Istemci.ButceHazirlik.ButceParametreUclari.Ekle(app);
            Istemci.ButceHazirlik.ButceSozlukUclari.Ekle(app);
            Istemci.ButceHazirlik.ProgButKodUclari.Ekle(app);
            Istemci.ButceHazirlik.ProgPebIliskiUclari.Ekle(app);
            Istemci.ButceHazirlik.BumkoAktarimUclari.Ekle(app);
            Istemci.AlacakTakip.AlacakTakipTanimUclari.Ekle(app);
            Istemci.AlacakTakip.AlacakTakipListeUclari.Ekle(app);
            Istemci.AlacakTakip.HesapHareketUclari.Ekle(app);
            Istemci.AlacakTakip.OdemeUclari.Ekle(app);
            Istemci.AlacakTakip.OdemeEmriUclari.Ekle(app);
            Istemci.AlacakTakip.AlacakTakipListeABUclari.Ekle(app);
            Istemci.AlacakTakip.AlacakTakipABIslemUclari.Ekle(app);
            Istemci.AlacakTakip.AlacakTakipABTarihceUclari.Ekle(app);
            Istemci.AlacakTakip.FaizHesabiUclari.Ekle(app);
            Istemci.Muhasebe.MuhTanimOrtak.Ekle(app);
            Istemci.Muhasebe.OdemeEmriMIFUclari.Ekle(app);
            Istemci.Muhasebe.OdemeEmriMIFSorguUclari.Ekle(app);
            Istemci.Muhasebe.OdemeEmriSecimUclari.Ekle(app);
            Istemci.Muhasebe.MuavinDefteriUclari.Ekle(app);
            Istemci.Muhasebe.TanimAciklamaUclari.Ekle(app);
            Istemci.Muhasebe.ImzaTanimUclari.Ekle(app);
            Istemci.HarcamaSurecleri.BorcuYokBelgesiUclari.Ekle(app);
            Istemci.HarcamaSurecleri.FirmaKisiListeUclari.Ekle(app);
            Istemci.HarcamaSurecleri.FirmaTanimUclari.Ekle(app);
            Istemci.Muhasebe.AvansUclari.Ekle(app);
            Istemci.Muhasebe.EbutceAktarUclari.Ekle(app);
            Istemci.Muhasebe.TanimIsUclari.Ekle(app);
            Istemci.Muhasebe.KapatmaUclari.Ekle(app);
            Istemci.Muhasebe.BankaListesiUclari.Ekle(app);
            Istemci.Muhasebe.CekListesiUclari.Ekle(app);
            Istemci.Muhasebe.VergiAktarimUclari.Ekle(app);
            Istemci.Muhasebe.CaprazTabloUclari.Ekle(app);
            Istemci.Muhasebe.MizanBilancoUclari.Ekle(app);
            Istemci.Muhasebe.TemlikUclari.Ekle(app);
            Istemci.Muhasebe.ButceAcilisFisiUclari.Ekle(app);
            Istemci.Muhasebe.YilKapatUclari.Ekle(app);
            Istemci.Muhasebe.YilSonuIslemleriUclari.Ekle(app);
            Istemci.Muhasebe.FirmaBilgisiDuzenlemeUclari.Ekle(app);
            Istemci.Muhasebe.SayistayVerileriUclari.Ekle(app);
            Istemci.Muhasebe.FaturaUclari.Ekle(app);
            Istemci.Muhasebe.FaturaTakipUclari.Ekle(app);
            Istemci.Muhasebe.MuhTanimMudurlukUclari.Ekle(app);
            Istemci.Muhasebe.MuhasebatAktarimUclari.Ekle(app);
            Istemci.Muhasebe.TaahhutKartiUclari.Ekle(app);
            Istemci.Muhasebe.TKDKTalepUclari.Ekle(app);
        }
    }
}
