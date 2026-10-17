using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    public class ButceSozlukIstegi
    {
        public int Yil { get; set; }
        /// <summary>Kopyalanacak satırın kimliği (Satirlar listesindeki kimlik).</summary>
        public string Kimlik { get; set; }
    }

    /// <summary>
    /// Bütçe Sözlük İşlemleri sayfasının (eski ButceHazirlikv2/ButceSozlukYonetim.aspx) uçları:
    ///   GET  /api/butcesozluk/durum?yil=      yılın ve önceki yılın sözlük / tertip / gerekçe / bütçe fişi kayıt sayıları
    ///   POST /api/butcesozluk/kopyala         bir satırı önceki yıldan kopyalar (yalnızca önceki yılda kayıt var, bu yılda yokken)
    /// Eski sayfadaki "Hazırlık Yılı Olarak Seç" tuşu kullanıcı isteğiyle alınmadı (2026-10-15). Açılış yılı: Ocak-Mart içinde
    /// bulunulan yıl, Nisan'dan sonra bir sonraki yıl; kullanıcı yılı değiştirebilir.
    /// </summary>
    public static class ButceSozlukUclari
    {
        public const string SayfaAdresi = ButceTanimOrtak.Klasor + "ButceSozlukYonetim.aspx";

        private enum Cesit { Kod, Tertip, Gerekce, Fis }

        private class Satir
        {
            public string kimlik;
            public string etiket;
            public string bolum;
            public Cesit cesit;
            /// <summary>Kod türü (Kod) ya da 1 gider / 2 gelir (Tertip, Fis).</summary>
            public int tur;
            /// <summary>Düzenle tuşunun açtığı sayfa; {yil} yılla değiştirilir.</summary>
            public string adres;
        }

        private const string Sozluk = "Sözlük Bilgileri";
        private const string Gerekceler = "Gerekçeler";

        private static readonly Satir[] Satirlar =
        {
            new Satir { kimlik = "kod1", etiket = "Kurumsal", bolum = Sozluk, cesit = Cesit.Kod, tur = 1, adres = "GenelKodTanimla?Tur=1&yil={yil}" },
            new Satir { kimlik = "kod2", etiket = "Fonksiyonel", bolum = Sozluk, cesit = Cesit.Kod, tur = 2, adres = "GenelKodTanimla?Tur=2&yil={yil}" },
            new Satir { kimlik = "kod3", etiket = "Finans", bolum = Sozluk, cesit = Cesit.Kod, tur = 3, adres = "GenelKodTanimla?Tur=3&yil={yil}" },
            new Satir { kimlik = "kod4", etiket = "Gid. Ekonomik", bolum = Sozluk, cesit = Cesit.Kod, tur = 4, adres = "GenelKodTanimla?Tur=4&yil={yil}" },
            new Satir { kimlik = "kod14", etiket = "Alt Faaliyet", bolum = Sozluk, cesit = Cesit.Kod, tur = 14, adres = "GenelKodTanimla?Tur=14&yil={yil}" },
            new Satir { kimlik = "kod5", etiket = "Gel. Ekonomik", bolum = Sozluk, cesit = Cesit.Kod, tur = 5, adres = "GenelKodTanimla?Tur=5&yil={yil}" },
            new Satir { kimlik = "kod6", etiket = "Finansman Eko.", bolum = Sozluk, cesit = Cesit.Kod, tur = 6, adres = "GenelKodTanimla?Tur=6&yil={yil}" },
            new Satir { kimlik = "kod7", etiket = "Sektör", bolum = Sozluk, cesit = Cesit.Kod, tur = 7, adres = "GenelKodTanimla?Tur=7&yil={yil}" },
            new Satir { kimlik = "tertip1", etiket = "Gider Tertipleri", bolum = Sozluk, cesit = Cesit.Tertip, tur = 1, adres = "TertipTanimFormu?Tur=1&yil={yil}" },
            new Satir { kimlik = "tertip2", etiket = "Gelir Tertipleri", bolum = Sozluk, cesit = Cesit.Tertip, tur = 2, adres = "TertipTanimFormu?Tur=2&yil={yil}" },
            new Satir { kimlik = "gerekce", etiket = "Hizmet Gerekçeleri", bolum = Gerekceler, cesit = Cesit.Gerekce, adres = "../ButceForm/HizmetGerekcesiv2?yil={yil}" },
            new Satir { kimlik = "fis1", etiket = "Bütçe Gider Fişi", bolum = Gerekceler, cesit = Cesit.Fis, tur = 1, adres = "ButceFisiGiris?odenekTur=1&yil={yil}" },
            new Satir { kimlik = "fis2", etiket = "Bütçe Gelir Fişi", bolum = Gerekceler, cesit = Cesit.Fis, tur = 2, adres = "ButceFisiGiris?odenekTur=2&yil={yil}" },
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/butcesozluk/durum", Durum);
            app.MapPost("/api/butcesozluk/kopyala", Kopyala);
        }

        private static ENUMTabloKod TabloKod(int tur)
        {
            if (tur == 1) return ENUMTabloKod.TABPEBOdenek;
            if (tur == 2) return ENUMTabloKod.TABPEBGelir;
            return ENUMTabloKod.TANIMSIZ;
        }

        /// <summary>Açılış yılı: Mart'a kadar içinde bulunulan yıl, Nisan'dan sonra bir sonraki yıl.</summary>
        private static int AcilisYili()
        {
            DateTime bugun = DateTime.Today;
            return bugun.Month <= 3 ? bugun.Year : bugun.Year + 1;
        }

        private static int Say(Satir s, int yil)
        {
            string kurum = TNS.DEG.Arac.VarsayilanKurumBul();
            switch (s.cesit)
            {
                case Cesit.Kod: return TNS.BKD.Arac.Tanimla().ButceKodSatirSayisiBul(yil, s.tur.ToString());
                case Cesit.Tertip: return TNS.BHMv2.Arac.Tanimla().TertipSayisiBul(new Tertip(yil, TabloKod(s.tur), kurum));
                case Cesit.Gerekce: return TNS.BHF.Arac.Tanimla().HizmetGerekcesiSayisiBul(yil, kurum);
                default: return TNS.BHMv2.Arac.Tanimla().ButceFisiSayisiBul(new Tertip(yil, TabloKod(s.tur), kurum));
            }
        }

        private static List<object> DurumListesi(int yil)
        {
            List<object> liste = new List<object>();
            foreach (Satir s in Satirlar)
            {
                int buYil = Say(s, yil);
                int oncYil = Say(s, yil - 1);
                liste.Add(new
                {
                    kimlik = s.kimlik, etiket = s.etiket, bolum = s.bolum, buYil = buYil, oncYil = oncYil,
                    kopyalanir = oncYil > 0 && buYil == 0,
                    adres = s.adres.Replace("{yil}", yil.ToString())
                });
            }
            return liste;
        }

        private static IResult Durum(HttpContext baglam, IWebHostEnvironment ortam, int? yil)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            try
            {
                int y = yil.HasValue && yil.Value >= 2003 ? yil.Value : AcilisYili();
                return Results.Json(new { yil = y, enBuyukYil = DateTime.Now.Year + 2, satirlar = DurumListesi(y) });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Sözlük bilgileri okunamadı.");
            }
        }

        /// <summary>Kopyala (eski KodKopyala / TertipKopyala / GerekceKopyala / FisKopyala).</summary>
        private static async Task<IResult> Kopyala(HttpContext baglam, IWebHostEnvironment ortam, ButceSozlukIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAdresi);
            if (ret != null) return ret;
            if (istek == null || istek.Yil <= 2000) return ButceTanimOrtak.Basarisiz("Yıl bilgisi hatalı!");
            Satir s = Array.Find(Satirlar, x => x.kimlik == istek.Kimlik);
            if (s == null) return Results.BadRequest();
            try
            {
                int yil = istek.Yil;
                string kurum = TNS.DEG.Arac.VarsayilanKurumBul();
                Sonuc sonuc;
                switch (s.cesit)
                {
                    case Cesit.Kod:
                        sonuc = TNS.BKD.Arac.Tanimla().OncekiYildanKopyala(kullanan, yil, s.tur.ToString());
                        break;
                    case Cesit.Tertip:
                        sonuc = TNS.BHMv2.Arac.Tanimla().TertipTanimKopyalaOncYil(kullanan, new Tertip(yil, TabloKod(s.tur), kurum));
                        break;
                    case Cesit.Gerekce:
                        sonuc = TNS.BHF.Arac.Tanimla().HizmetGerekcesiKopyalaOncYil(kullanan, yil, kurum);
                        break;
                    default:
                        sonuc = TNS.BHMv2.Arac.Tanimla().ButceFisiKopyalaOncYil(kullanan, new Tertip(yil, TabloKod(s.tur), kurum));
                        break;
                }
                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(sonuc == null ? null : sonuc.hataStr, "Kopyalanamadı."), satirlar = DurumListesi(yil) });
                return Results.Json(new { basarili = true, bilgi = s.etiket + " önceki yıldan kopyalandı.", satirlar = DurumListesi(yil) });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Kopyalanamadı.");
            }
        }
    }
}
