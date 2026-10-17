using System;
using System.Collections.Generic;
using System.Globalization;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.ATM;
using TNS.KYM;

namespace Istemci.AlacakTakip
{
    /// <summary>
    /// AB Dosya Tarihçesi (eski AlacakTakip/AlacakTakipABTarihce.aspx) ucu. Sayfa AB Takip İşlemleri ekranında (AlacakTakipListeAB)
    /// dosyanın "Tarihçe" sekmesinde çerçeve olarak açılır; menüde yoktur, yetkisi üst sayfadan (AlacakTakipListeAB) gelir.
    ///   GET /api/alacaktakip/ab/tarihce?iPARDDonemi=&amp;dosyaAnahtar=   dosyanın işlem tarihçesi (servisATM.DosyaTarihceListeleAB)
    /// </summary>
    public static class AlacakTakipABTarihceUclari
    {
        public const string SayfaAdresi = "../AlacakTakip/AlacakTakipABTarihce.aspx";
        public const string UstSayfa = "../AlacakTakip/AlacakTakipListeAB.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/alacaktakip/ab/tarihce", Tarihce);
        }

        /// <summary>Üst sayfanın (AB Takip İşlemleri) menü yetkisi ve eski üst sayfa Page_Load'undaki TNS.ATM.Yetki.SayfayaGirisYapabilirMi.</summary>
        public static bool Yetkili(Kullanici kullanan, string klasor)
        {
            if (!SayfaYetki.Yetkili(kullanan, klasor, SayfaAdresi, UstSayfa)) return false;
            try
            {
                return TNS.ATM.Yetki.SayfayaGirisYapabilirMi(kullanan);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Replace("<br/>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        /// <summary>Eski VeriYenile: dosyaAnahtar yoksa (≤ 0) boş liste; iPARDDonemi verilmezse 1 (eski RequestBul varsayılanları).</summary>
        private static IResult Tarihce(HttpContext baglam, IWebHostEnvironment ortam, int? iPARDDonemi, int? dosyaAnahtar)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            int donem = iPARDDonemi ?? 1;
            int anahtar = dosyaAnahtar ?? -1;
            List<object> satirlar = new List<object>();
            if (anahtar <= 0) return Results.Json(new { satirlar = satirlar });
            try
            {
                ObjectArray oa = TNS.ATM.Arac.Tanimla().DosyaTarihceListeleAB(kullanan, donem, anahtar);
                if (oa == null || oa.sonuc == null || !oa.sonuc.islemSonuc)
                    return Results.Json(new { hata = HataMetni(oa?.sonuc?.hataStr, "Tarihçe okunamadı.") });
                foreach (TarihceAB t in oa.objeler)
                {
                    DateTime z = t.zamanDT;
                    satirlar.Add(new
                    {
                        // Yerel saat, "yyyy-MM-dd HH:mm" (eski sütun biçimi dd.MM.yyyy hh:mm; 12 saatlik hh yerine 24 saatlik).
                        zamanDT = z == DateTime.MinValue ? "" : z.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Kim = t.Kim ?? "",
                        islemAd = t.islemAd ?? "",
                        metin = t.metin ?? ""
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Tarihçe okunamadı." });
            }
        }
    }
}
