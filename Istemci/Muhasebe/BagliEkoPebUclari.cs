using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>Tertip - faaliyet ilişkisi kaydet / sil isteği (alan adları BagliEkoPeb'deki gibi).</summary>
    public class BagliEkoPebIstegi
    {
        public string tertip { get; set; }
        public string peb { get; set; }
    }

    /// <summary>
    /// Faaliyet Tertip Kısıtlama (eski ButceMuhasebe/BagliEkoPebAnaForm.aspx) uçları. Ödeme emri / MİF girişinde bir tertip
    /// yalnızca burada ilişkilendirilen faaliyetlerle (ve bir faaliyet yalnızca ilişkilendirilen tertiplerle) kullanılabilir.
    ///   GET  /api/bagliekopeb/liste   : ilişki kurulmuş tertip ve faaliyetler (IMUHServis.ListeleBagliEkoPebAna)
    ///   POST /api/bagliekopeb/kaydet  : { tertip, peb } (IMUHServis.KaydetBagliEkoPebAna)
    ///   POST /api/bagliekopeb/sil     : { tertip, peb } (IMUHServis.SilBagliEkoPebAna)
    /// Sayfa yetkisi menu.xml'den; kaydetme ve silmeyi iş kuralı (MUHBagliEkoPebAna.YazmaYetkisiVarMi) yalnızca sistem
    /// yöneticisine verir.
    /// </summary>
    public static class BagliEkoPebUclari
    {
        public const string SayfaAdres = "../ButceMuhasebe/BagliEkoPebAnaForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/bagliekopeb/liste", Liste);
            app.MapPost("/api/bagliekopeb/kaydet", Kaydet);
            app.MapPost("/api/bagliekopeb/sil", Sil);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult red = Denetle(baglam, ortam, out Kullanici kullanan);
            if (red != null) return red;

            BagliEkoPeb[] kayitlar;
            try
            {
                kayitlar = TNS.MUH.Arac.Tanimla().ListeleBagliEkoPebAna(kullanan, new BagliEkoPeb());
            }
            catch (Exception)
            {
                return Hata(Metin.Muhasebe("IslemHata"));
            }

            List<object> satirlar = new List<object>();
            if (kayitlar != null)
                foreach (BagliEkoPeb k in kayitlar)
                    satirlar.Add(new { tertip = k.tertip, peb = k.peb });
            return Results.Json(new { satirlar = satirlar });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, BagliEkoPebIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult red = Denetle(baglam, ortam, out Kullanici kullanan);
            if (red != null) return red;

            BagliEkoPeb form = FormOl(istek);
            if (form.tertip == "" || form.peb == "") return Hata(Metin.Muhasebe("BagliEkoPebAna003"));

            Sonuc sonuc = TNS.MUH.Arac.Tanimla().KaydetBagliEkoPebAna(kullanan, form);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc));
            return Results.Json(new { basarili = true, tertip = form.tertip, peb = form.peb, bilgi = Metin.Muhasebe("BagliEkoPebAna004") });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, BagliEkoPebIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult red = Denetle(baglam, ortam, out Kullanici kullanan);
            if (red != null) return red;

            BagliEkoPeb form = FormOl(istek);
            if (form.tertip == "" || form.peb == "") return Hata(Metin.Muhasebe("BagliEkoPebAna005"));

            Sonuc sonuc = TNS.MUH.Arac.Tanimla().SilBagliEkoPebAna(kullanan, form);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc));
            return Results.Json(new { basarili = true, bilgi = Metin.Muhasebe("MUHGNL154") });
        }

        // ------------------------------------------------------------------ yardımcılar

        private static BagliEkoPeb FormOl(BagliEkoPebIstegi istek)
        {
            BagliEkoPeb form = new BagliEkoPeb();
            form.tertip = (istek?.tertip ?? "").Trim();
            form.peb = (istek?.peb ?? "").Trim();
            return form;
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        /// <summary>Servisin hata metni; boşsa ya da bir istisna dökümüyse genel hata metni.</summary>
        private static string HataMetni(Sonuc sonuc)
        {
            string hata = sonuc.hataStr ?? "";
            if (hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at "))
                return Metin.Muhasebe("IslemHata");
            return hata.Replace("<br />", "\n").Replace("<br>", "\n").Trim();
        }
    }
}
