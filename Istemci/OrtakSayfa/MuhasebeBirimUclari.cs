using System;
using System.Collections.Generic;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;

namespace Istemci.OrtakSayfa
{
    /// <summary>
    /// Muhasebe - birim - kurumsal kod seçim bileşeninin (wwwroot/js/muhbirim.js, Sgb.muhasebeBirim) uçları:
    ///   GET /api/muhbirim/bilgi?muhasebe=&amp;birim=&amp;yil=  elle yazılan muhasebe / birimin adı ve kurumsal kodu (eski txtBirim_Changed)
    ///   GET /api/muhbirim/agac?ust=                       seçim penceresi (eski ListeMuhasebeBirim.aspx): kökte kullanıcının
    ///                                                     yetkili olduğu saymanlıklar, altlarında birimleri
    /// Oturum açmış her kullanıcı kullanır; hangi muhasebe birimlerini görebileceğini iş kuralı (KullaniciMuhasebeBirimleri) belirler.
    /// Sayfaların kendi işlemleri (Oku, Kaydet...) muhasebe / birim yetkisini ayrıca denetler.
    /// </summary>
    public static class MuhasebeBirimUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/muhbirim/bilgi", Bilgi);
            app.MapGet("/api/muhbirim/agac", Agac);
        }

        private static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata });
        }

        /// <summary>Varsayılan kurum kodu (eski GenelIslemlerIstemci.VarsayilanKurumBul: /KURUMKOD).</summary>
        private static string KurumKod()
        {
            return TNS.DEG.Arac.VarsayilanKurumBul() ?? "";
        }

        /// <summary>Kurumsal kod, yılın kurumsal kod yapısına göre noktalı.</summary>
        public static string KurumsalYaz(int yil, string kurum)
        {
            if (string.IsNullOrWhiteSpace(kurum)) return "";
            return OrtakFonksiyonlar.KodNoktaKoy(TNS.BKD.TertipYapi.KodYapiBul(yil, 1, false), kurum.Replace(".", ""));
        }

        /// <summary>Eski SaymanlikBirimBul + MuhBirimAdBul: saymanlık / birim adı ve birimin kurumsal kodu.</summary>
        public static string BirimAdBul(int saymanlikNo, string birimKod, out string kurum)
        {
            kurum = "";
            if (saymanlikNo <= 0) return "";
            string sayAd = TNS.UZY.Arac.Tanimla().UzayDegeriStr(new Kullanici(), "MUHMUHASEBE", saymanlikNo.ToString(), true, "MUHASEBEAD");
            string birimAd = "";
            if (string.IsNullOrWhiteSpace(sayAd)) sayAd = "Saymanlık Bulunamadı!";
            else if (!string.IsNullOrWhiteSpace(birimKod))
            {
                TNS.MUH.MuhasebeBirim bir = new TNS.MUH.MuhasebeBirim();
                bir.muhasebe = saymanlikNo;
                bir.birimKod = birimKod.Trim();
                ObjectArray oa = TNS.MUH.Arac.Tanimla().MuhasebeBirimListele(bir);
                if (oa != null && oa.ObjeSayisi >= 1)
                {
                    bir = (TNS.MUH.MuhasebeBirim)oa.objeler[0];
                    birimAd = bir.birimAd;
                    kurum = bir.kurum ?? "";
                }
                if (string.IsNullOrWhiteSpace(birimAd)) birimAd = "Birim Bulunamadı!";
            }
            return TNS.MUH.Arac.BirimAdOl(sayAd, birimAd);
        }

        private static IResult Bilgi(HttpContext baglam, int? muhasebe, string birim, int? yil)
        {
            if (OturumBilgisi.KullaniciOku(baglam) == null) return Results.Unauthorized();
            try
            {
                string kurum;
                string ad = BirimAdBul(muhasebe ?? 0, birim, out kurum);
                return Results.Json(new { birimAd = ad, kurumsal = KurumsalYaz(yil > 2000 ? yil.Value : DateTime.Now.Year, kurum) });
            }
            catch (Exception)
            {
                return Hata("Birim bilgisi okunamadı.");
            }
        }

        private static IResult Agac(HttpContext baglam, string ust, int? yil)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            try
            {
                int muhasebe = OrtakFonksiyonlar.ConvertToInt(ust, 0);
                ObjectArray oa = TNS.MUH.Arac.Tanimla().KullaniciMuhasebeBirimleri(kullanan, muhasebe, KurumKod());
                if (oa == null || !oa.sonuc.islemSonuc) return Hata("Muhasebe birimleri okunamadı.");

                List<object> dugumler = new List<object>();
                HashSet<string> eklenen = new HashSet<string>();
                int kodYil = yil > 2000 ? yil.Value : DateTime.Now.Year;
                foreach (TNS.MUH.MuhasebeBirim m in oa.objeler)
                {
                    if (muhasebe <= 0)
                    {
                        if (!eklenen.Add(m.muhasebe.ToString())) continue;
                        dugumler.Add(new { kod = m.muhasebe.ToString(), ad = m.muhasebe + "-" + m.muhasebeAd, altVar = true });
                    }
                    else
                    {
                        string birimAd = (m.birimAd ?? "").Trim();
                        if (birimAd == "") continue;
                        string kurKod = KurumsalYaz(kodYil, m.kurum);
                        if (!eklenen.Add(m.birimKod + "-" + kurKod)) continue;
                        dugumler.Add(new
                        {
                            kod = m.muhasebe + "-" + m.birimKod, ad = m.birimKod + "-" + birimAd + (kurKod != "" ? " (" + kurKod + ")" : ""), altVar = false,
                            muhasebe = m.muhasebe, birim = m.birimKod, kurumsal = kurKod, birimAd = TNS.MUH.Arac.BirimAdOl(m.muhasebeAd, birimAd)
                        });
                    }
                }
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Muhasebe birimleri okunamadı.");
            }
        }
    }
}
