using System;
using System.Collections.Generic;
using System.Globalization;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;

namespace Istemci.Kullanicilar
{
    /// <summary>
    /// Sistemi Kullanan Kişilerin Listesi (eski KYM/CalisanKullaniciListe.aspx) ve Sistem Kullanım Grafiği (eski KYM/CalisanKullaniciGrafik.aspx).
    /// Kaynak, sayfa isteklerinin yazıldığı KULLANICI_BILGI tablosudur (SayfaIstekKaydi).
    ///   GET /api/calisankullanici/liste?sayfa=&amp;sure=5
    ///       Eski btnListele_Click: son "sure" dakikada işlem yapan kullanıcılar (her kullanıcının son işlemi) → { satirlar, adet } / { hata }
    ///   GET /api/calisankullanici/grafik?sayfa=&amp;tarih1=yyyy-MM-dd&amp;tarih2=yyyy-MM-dd&amp;zaman=1..5
    ///       Eski KullanimGrafik.Generate: zaman dilimine göre sayfa görüntülenme sayıları → { noktalar: [{ t, adet }], toplam } / { hata }
    ///       Yalnızca ana tablo sayılır; KULLANICI_BILGI_ARSIV'e taşınmış (2 yıldan eski) kayıtlar grafiğe girmez.
    /// Yetki: menüdeki seçenek (Liste: eski SayfayaGirisYapabilirMi).
    /// </summary>
    public static class CalisanKullaniciUclari
    {
        public const string ListeAdresi = "../KYM/CalisanKullaniciListe.aspx";
        public const string GrafikAdresi = "../KYM/CalisanKullaniciGrafik.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/calisankullanici/liste", Liste);
            app.MapGet("/api/calisankullanici/grafik", Grafik);
        }

        public static bool ListeYetkili(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, ListeAdresi);
        }

        public static bool GrafikYetkili(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, GrafikAdresi);
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string sayfa, string sure)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!ListeYetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            int dakika = OrtakFonksiyonlar.ConvertToInt((sure ?? "").Trim(), 0);
            ObjectArray<KullaniciIslemYapan> ytl = TNS.KYM.Arac.Tanimla().KullaniciSessionListele("", dakika, "", (sayfa ?? "").Trim(), "", "", "", "", "");
            if (ytl == null) return Results.Json(new { hata = "Liste okunamadı." });
            if (ytl.sonuc != null && !ytl.sonuc.islemSonuc && !string.IsNullOrEmpty(ytl.sonuc.hataStr))
                return Results.Json(new { hata = "Liste okunamadı." });

            List<object> satirlar = new List<object>();
            int no = 1;
            foreach (KullaniciIslemYapan k in ytl.objeler)
            {
                satirlar.Add(new
                {
                    no = no++,
                    ad = k.bilgi1 + " (" + k.kullaniciAd + ")",
                    islemTarih = k.islemTarih != null && !k.islemTarih.isNull ? k.islemTarih.ToLongString() : "",
                    islemYer = k.islemYeri,
                    islemSayfa = k.islemSayfa
                });
            }
            return Results.Json(new { satirlar, adet = satirlar.Count });
        }

        private static TNSDateTime Tarih(string metin)
        {
            if (DateTime.TryParseExact((metin ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return new TNSDateTime();
        }

        private static IResult Grafik(HttpContext baglam, IWebHostEnvironment ortam, string sayfa, string tarih1, string tarih2, string zaman)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GrafikYetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            int z = OrtakFonksiyonlar.ConvertToInt(zaman, (int)ENUMKullanimBilgiZaman.ZAMAN_GUN);
            if (z < 1 || z > 5) z = (int)ENUMKullanimBilgiZaman.ZAMAN_GUN;
            ENUMKullanimBilgiZaman tur = (ENUMKullanimBilgiZaman)z;
            TNSDateTime t1 = Tarih(tarih1), t2 = Tarih(tarih2);
            string sayfaAd = (sayfa ?? "").Trim();

            ObjectArray<KullaniciIslemYapan> ana = TNS.KYM.Arac.Tanimla().KullaniciSessionGrafik("", tur, t1, t2, sayfaAd);
            if (ana == null || (ana.sonuc != null && !ana.sonuc.islemSonuc && !string.IsNullOrEmpty(ana.sonuc.hataStr)))
                return Results.Json(new { hata = "Grafik verileri okunamadı." });

            List<object> noktalar = new List<object>();
            long toplam = 0;
            foreach (KullaniciIslemYapan k in ana.objeler)
            {
                if (k.islemTarih == null || k.islemTarih.isNull) continue;
                noktalar.Add(new { t = k.islemTarih.Oku().ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture), adet = k.adet });
                toplam += k.adet;
            }
            return Results.Json(new { noktalar, toplam });
        }
    }
}
