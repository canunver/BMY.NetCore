using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri "Tarihçe" sekmesi (eski InsanKaynaklari/PersonelTarihce.aspx; yeni sayfa InsanKaynaklari/PersonelTarihce) ucu:
    ///   GET /api/personelbilgi/tarihce/liste?personelKod=   (eski Listele; salt okunur)
    /// İş kuralı eski sayfadaki gibi IIKYServis.PersonelTarihceListele: her satır "tarih!kategori!alan!yeni!eski!yapan!açıklama1!açıklama2"
    /// metni; hata olursa tek satırda hata metni döner.
    /// </summary>
    public static class PersonelTarihceUclari
    {
        private const string SayfaAd = "PersonelTarihce";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/tarihce/liste", Liste);
        }

        /// <summary>İş kuralının açıklamalarına koyduğu HTML'i (&lt;b&gt;, &lt;/br&gt;) düz metne çevirir.</summary>
        private static string Temiz(string s)
        {
            s = (s ?? "").Replace("</br>", "\n").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n")
                .Replace("<b>", "").Replace("</b>", "");
            return s.Trim();
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> satirlar = new List<object>();
            int pKod = personelKod ?? 0;
            if (pKod <= 0) return Results.Json(new { satirlar = satirlar });

            string[] liste = TNS.IKY.Arac.Tanimla().PersonelTarihceListele(kullanan, pKod);
            if (liste == null) return Results.Json(new { satirlar = satirlar, hata = "Tarihçe bilgileri okunamadı." });

            foreach (string item in liste)
            {
                string[] satir = (item ?? "").Split('!');
                // Eski kod hata satırını "Length == 2" ile yakalıyordu (hata metninde '!' yoksa dizin hatası verirdi);
                // 8 parçadan az olan satır hata metni sayılır.
                if (satir.Length < 8)
                    return Results.Json(new { satirlar = new List<object>(), hata = PersonelBilgiOrtak.HataMetni(item, "Tarihçe bilgileri okunamadı.") });

                // Değerlerde '!' geçerse parçalar kayar; fazlası son açıklamaya eklenir.
                string aciklama2 = string.Join("!", satir, 7, satir.Length - 7);
                DateTime t = new TNSDateTime(satir[0]).Oku();
                satirlar.Add(new
                {
                    TARIH = t == DateTime.MinValue ? "" : t.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    KATEGORI = satir[1],
                    DEGISENALAN = satir[2],
                    YENIDEGER = satir[3],
                    ESKIDEGER = satir[4],
                    DEGISIKLIKYAPAN = satir[5],
                    ACIKLAMA1 = Temiz(satir[6]),
                    ACIKLAMA2 = Temiz(aciklama2)
                });
            }
            return Results.Json(new { satirlar = satirlar });
        }
    }
}
