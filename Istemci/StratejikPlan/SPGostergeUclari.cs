using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.PEB;

namespace Istemci.StratejikPlan
{
    public class SPGostergeIstegi
    {
        /// <summary>Gösterge kodu; yeni göstergede 0.</summary>
        public int Kod { get; set; }
        public string Ad { get; set; }
        public int OlcuBirim { get; set; }
        /// <summary>1: Gerç.*100/Hedef, 2: (Gerç.-Baş.)*100/(Hedef-Baş.)</summary>
        public int Tur { get; set; }
        /// <summary>1: Artan, 2: Azalan</summary>
        public int IlerlemeYonu { get; set; }
        public int OndalikSayisi { get; set; }
    }

    public class SPOlcuBirimIstegi
    {
        /// <summary>Ölçü birimi kodu; yenide 0.</summary>
        public int Kod { get; set; }
        public string Ad { get; set; }
    }

    /// <summary>
    /// Stratejik plan gösterge tanımları (eski PerformansButce/TanimPerformansGostergeYeni.aspx; yeni sayfa
    /// PerformansButce/TanimSPGosterge) uçları:
    ///   GET  /api/spgosterge/tanim (ölçü birimleri), /api/spgosterge/liste
    ///   POST /api/spgosterge/kaydet, /api/spgosterge/sil
    /// ve ölçü birimi tanımları (eski PerformansButce/TanimOlcuBirimYeni.aspx; yeni sayfa PerformansButce/TanimSPOlcuBirim):
    ///   GET  /api/spgosterge/olcu-liste
    ///   POST /api/spgosterge/olcu-kaydet, /api/spgosterge/olcu-sil
    /// Göstergeler üzerinde çalışılan kurumundur (Stratejik Plan'da seçili dönemin kurumu, seçilmemişse varsayılan kurum);
    /// Stratejik Plan'ın hedef gösterge listesi de aynı kurumun göstergelerini gösterir.
    /// </summary>
    public static class SPGostergeUclari
    {
        public const string SayfaAdresi = "../PerformansButce/TanimSPGosterge.aspx";
        public const string OlcuSayfaAdresi = "../PerformansButce/TanimSPOlcuBirim.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/spgosterge/tanim", Tanim);
            app.MapGet("/api/spgosterge/liste", Liste);
            app.MapPost("/api/spgosterge/kaydet", Kaydet);
            app.MapPost("/api/spgosterge/sil", Sil);
            app.MapGet("/api/spgosterge/olcu-liste", OlcuListe);
            app.MapPost("/api/spgosterge/olcu-kaydet", OlcuKaydet);
            app.MapPost("/api/spgosterge/olcu-sil", OlcuSil);
        }

        /// <summary>Menüde gösterge ya da ölçü birimi sayfasını görebilen veya Stratejik Plan sayfasını kullanabilen (pencereden açılır) kullanıcı.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (StratejikPlanUclari.YetkiVar(kullanan, uygulamaKlasoru)) return true;
            try
            {
                MenuOkuyucu menu = new MenuOkuyucu(kullanan);
                string dosya = MenuOkuyucu.DosyaYolu(uygulamaKlasoru);
                return menu.SayfaYetkili(dosya, SayfaAdresi) || menu.SayfaYetkili(dosya, OlcuSayfaAdresi);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> olculer = new List<object>();
            ObjectArray liste = TNS.PEB.Arac.Tanimla().OlcuBirimListele(kullanan, new OlcuBirim());
            if (!liste.sonuc.islemSonuc) return Hata(HataMetni(liste.sonuc, "Ölçü birimleri okunamadı."));
            foreach (OlcuBirim o in liste.objeler)
                olculer.Add(new { kod = o.kod, ad = o.ad });
            return Results.Json(new { olculer = olculer, kurum = StratejikPlanUclari.CalisilanKurum(kullanan) });
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            PerformansGosterge kriter = new PerformansGosterge();
            kriter.adaGoreSirala = true;
            kriter.kurumKod = StratejikPlanUclari.CalisilanKurum(kullanan);
            ObjectArray liste = TNS.PEB.Arac.Tanimla().PerformansGostergeListele(kullanan, kriter);
            if (!liste.sonuc.islemSonuc) return Hata(HataMetni(liste.sonuc, "Göstergeler okunamadı."));

            List<object> satirlar = new List<object>();
            foreach (PerformansGosterge g in liste.objeler)
                satirlar.Add(new
                {
                    kod = g.gostergeKod,
                    ad = g.gostergeAd,
                    // Stratejik Plan'ın gösterge seçim listesindeki biçim: "ad(ölçü birimi)"
                    listeAd = g.ad,
                    olcuBirim = g.olcuBirim.kod,
                    olcuBirimAd = g.olcuBirim.ad,
                    tur = (int)g.tur,
                    ilerlemeYonu = (int)g.ilerlemeYonu,
                    ondalikSayisi = g.ondalikSayisi
                });
            return Results.Json(new { satirlar = satirlar });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, SPGostergeIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            if (string.IsNullOrWhiteSpace(istek.Ad)) return Hata("Göstergenin adını yazınız.");
            if (istek.OlcuBirim <= 0) return Hata("Ölçü birimini seçiniz.");

            PerformansGosterge gosterge = new PerformansGosterge();
            gosterge.gostergeKod = Math.Max(0, istek.Kod);
            gosterge.gostergeAd = istek.Ad.Trim();
            gosterge.olcuBirim.kod = istek.OlcuBirim;
            gosterge.tur = istek.Tur == 1 ? ENUMGostergeTur.OranAC : ENUMGostergeTur.OranABCB;
            gosterge.ilerlemeYonu = istek.IlerlemeYonu == 2 ? ENUMGostergeIlerlemeYonu.Azalan : ENUMGostergeIlerlemeYonu.Artan;
            gosterge.ondalikSayisi = Math.Max(0, istek.OndalikSayisi);
            // Eski sayfa seviyeyi gizli alandan 1, e-bütçe kodunu ölçü birimi kodundan veriyordu; aynen korunur.
            gosterge.seviye = 1;
            gosterge.ebutceKod = istek.OlcuBirim.ToString();
            gosterge.kurumKod = StratejikPlanUclari.CalisilanKurum(kullanan);

            Sonuc sonuc = TNS.PEB.Arac.Tanimla().PerformansGostergeKaydet(kullanan, gosterge);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Gösterge kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = "Gösterge kaydedildi.", kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, gosterge.gostergeKod) });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, SPGostergeIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Kod <= 0) return Results.BadRequest();

            PerformansGosterge gosterge = new PerformansGosterge();
            gosterge.gostergeKod = istek.Kod;
            Sonuc sonuc = TNS.PEB.Arac.Tanimla().PerformansGostergeSil(kullanan, gosterge);
            // Hedeflerde kullanılmış gösterge silinmez; iş kuralının uyarısı olduğu gibi gösterilir.
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Gösterge silinemedi."));
            return Results.Json(new { basarili = true, bilgi = "Gösterge silindi." });
        }

        // ------------------------------------------------------------------ ölçü birimleri

        private static IResult OlcuListe(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            ObjectArray liste = TNS.PEB.Arac.Tanimla().OlcuBirimListele(kullanan, new OlcuBirim());
            if (!liste.sonuc.islemSonuc) return Hata(HataMetni(liste.sonuc, "Ölçü birimleri okunamadı."));
            List<object> satirlar = new List<object>();
            foreach (OlcuBirim o in liste.objeler)
                satirlar.Add(new { kod = o.kod, ad = o.ad });
            return Results.Json(new { satirlar = satirlar });
        }

        private static async Task<IResult> OlcuKaydet(HttpContext baglam, IWebHostEnvironment ortam, SPOlcuBirimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            if (string.IsNullOrWhiteSpace(istek.Ad)) return Hata("Ölçü biriminin adını yazınız.");

            OlcuBirim birim = new OlcuBirim();
            birim.kod = Math.Max(0, istek.Kod);
            birim.ad = istek.Ad.Trim();
            // Eski sayfa e-bütçe kodunu ölçü birimi kodundan veriyordu (yenide "0"); aynen korunur.
            birim.ebutceKod = birim.kod.ToString();

            Sonuc sonuc = TNS.PEB.Arac.Tanimla().OlcuBirimKaydet(kullanan, birim);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Ölçü birimi kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = "Ölçü birimi kaydedildi.", kod = OrtakFonksiyonlar.ConvertToInt((sonuc.anahtar ?? "").Trim(), birim.kod) });
        }

        private static async Task<IResult> OlcuSil(HttpContext baglam, IWebHostEnvironment ortam, SPOlcuBirimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Kod <= 0) return Results.BadRequest();

            OlcuBirim birim = new OlcuBirim();
            birim.kod = istek.Kod;
            Sonuc sonuc = TNS.PEB.Arac.Tanimla().OlcuBirimSil(kullanan, birim);
            // Göstergelerde kullanılmış ölçü birimi silinmez; iş kuralının uyarısı olduğu gibi gösterilir.
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Ölçü birimi silinemedi."));
            return Results.Json(new { basarili = true, bilgi = "Ölçü birimi silindi." });
        }

        // ------------------------------------------------------------------ yardımcılar

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = (sonuc.hataStr ?? "").Replace("<br>", " ");
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata;
        }
    }
}
