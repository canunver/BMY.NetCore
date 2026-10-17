using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS;
using TNS.ATM;
using TNS.KYM;

namespace Istemci.AlacakTakip
{
    public class FaizTurIstegi
    {
        public int yil { get; set; }
        public int kod { get; set; }
        public string ad { get; set; }
        public double oran { get; set; }
    }

    /// <summary>
    /// Alacak Takip tanım sayfalarının uçları:
    ///   Faiz Türü (eski AlacakTakip/TanimFaizTur.aspx)
    ///     GET  /api/alacaktakip/faiztur/tanim         yıl kutusunun sınırları ve aktif yıl
    ///     GET  /api/alacaktakip/faiztur/liste?yil=&amp;ad=  faiz türleri (eski Ara / btnAra_Click)
    ///     POST /api/alacaktakip/faiztur/kaydet        faiz türü ekler ya da günceller (eski btnKaydet_Click)
    ///   Faiz Türü Seçimi (eski AlacakTakip/ListeFaizTur.aspx): aynı liste ucunu kullanır.
    ///   Durum (eski AlacakTakip/TanimDurum.aspx)
    ///     GET  /api/alacaktakip/durum/liste           durumlar (eski btnAra_Click, DurumListele(hepsiMi: true))
    /// Eski Durum sayfasının Kaydet tuşu yalnızca alanları denetliyordu; sunucuda durum kaydeden işlem yok, bu yüzden uç da yok.
    /// </summary>
    public static class AlacakTakipTanimUclari
    {
        public const string FaizTurAdresi = "../AlacakTakip/TanimFaizTur.aspx";
        public const string FaizTurListeAdresi = "../AlacakTakip/ListeFaizTur.aspx";
        public const string DurumAdresi = "../AlacakTakip/TanimDurum.aspx";

        /// <summary>
        /// ListeFaizTur menüde yok; Alacak Takip ekranlarının içinden açılan seçim penceresidir. Yetkisi bu üst sayfalardan gelir.
        /// </summary>
        public static readonly string[] FaizTurListeUstSayfalar =
        {
            FaizTurAdresi,
            "../AlacakTakip/AlacakTakipListe.aspx",
            "../AlacakTakip/AlacakTakipListeAB.aspx",
            "../AlacakTakip/FaizHesabi.aspx"
        };

        /// <summary>Eski sayfalardaki yıl listesinin ilk yılı.</summary>
        public const int IlkYil = 2006;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/alacaktakip/faiztur/tanim", FaizTurTanim);
            app.MapGet("/api/alacaktakip/faiztur/liste", FaizTurListe);
            app.MapPost("/api/alacaktakip/faiztur/kaydet", FaizTurKaydet);
            app.MapGet("/api/alacaktakip/durum/liste", DurumListe);
        }

        public static bool FaizTurYetkili(Kullanici kullanan, string klasor)
        {
            return SayfaYetki.Yetkili(kullanan, klasor, FaizTurAdresi);
        }

        public static bool FaizTurListeYetkili(Kullanici kullanan, string klasor)
        {
            return SayfaYetki.Yetkili(kullanan, klasor, FaizTurListeAdresi, FaizTurListeUstSayfalar);
        }

        public static bool DurumYetkili(Kullanici kullanan, string klasor)
        {
            return SayfaYetki.Yetkili(kullanan, klasor, DurumAdresi);
        }

        private static IResult Denetle(HttpContext baglam, Func<Kullanici, bool> yetkili, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!yetkili(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        // ---------------------------------------------------------------- Faiz Türü

        private static IResult FaizTurTanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, k => FaizTurListeYetkili(k, ortam.ContentRootPath), out Kullanici kullanan);
            if (ret != null) return ret;
            int buYil = DateTime.Today.Year;
            List<int> yillar = new List<int>();
            for (int i = buYil; i >= IlkYil; i--) yillar.Add(i);
            return Results.Json(new { yillar = yillar, aktifYil = buYil });
        }

        private static IResult FaizTurListe(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ad)
        {
            IResult ret = Denetle(baglam, k => FaizTurListeYetkili(k, ortam.ContentRootPath), out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray bilgi = TNS.ATM.Arac.Tanimla().FaizTurListele(kullanan, yil ?? 0, (ad ?? "").Trim());
                if (bilgi == null || bilgi.sonuc == null || !bilgi.sonuc.islemSonuc)
                    return Results.Json(new { hata = HataMetni(bilgi?.sonuc?.hataStr, "Faiz türleri okunamadı.") });
                List<object> satirlar = new List<object>();
                foreach (TNS.ATM.FaizTur ft in bilgi.objeler)
                    satirlar.Add(new { yil = ft.yil, kod = ft.kod, ad = ft.ad ?? "", oran = ft.oran });
                return Results.Json(new { satirlar = satirlar, bilgi = satirlar.Count == 0 ? HataMetni(bilgi.sonuc.bilgiStr, "Kayıt bulunamadı.") : "" });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Faiz türleri okunamadı." });
            }
        }

        private static async Task<IResult> FaizTurKaydet(HttpContext baglam, IWebHostEnvironment ortam, FaizTurIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, k => FaizTurYetkili(k, ortam.ContentRootPath), out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            try
            {
                TNS.ATM.FaizTur ft = new TNS.ATM.FaizTur();
                ft.yil = istek.yil;
                ft.kod = istek.kod;
                ft.ad = (istek.ad ?? "").Trim();
                ft.oran = istek.oran;
                Sonuc sonuc = TNS.ATM.Arac.Tanimla().FaizTurKaydet(kullanan, ft);
                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla tamamlandı." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        // ---------------------------------------------------------------- Durum

        private static IResult DurumListe(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, k => DurumYetkili(k, ortam.ContentRootPath), out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray liste = TNS.ATM.Arac.Tanimla().DurumListele(kullanan, true);
                if (liste == null || liste.sonuc == null || !liste.sonuc.islemSonuc)
                    return Results.Json(new { hata = HataMetni(liste?.sonuc?.hataStr, "Durumlar okunamadı.") });
                List<object> satirlar = new List<object>();
                foreach (TNS.ATM.Durum d in liste.objeler)
                    satirlar.Add(new { kod = d.kod, ad = d.ad ?? "" });
                return Results.Json(new { satirlar = satirlar, bilgi = satirlar.Count == 0 ? HataMetni(liste.sonuc.bilgiStr, "Kayıt bulunamadı.") : "" });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Durumlar okunamadı." });
            }
        }
    }

    /// <summary>Alacak Takip tanım sayfalarının ortak sayfa modeli: yalnızca yetkiyi denetler (içerik JavaScript ile uçlardan gelir).</summary>
    public abstract class AlacakTakipSayfaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        protected AlacakTakipSayfaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        protected abstract bool YetkiVar(Kullanici kullanan, string klasor);

        public bool Yetkili { get; private set; }

        public virtual void OnGet()
        {
            Yetkili = YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
