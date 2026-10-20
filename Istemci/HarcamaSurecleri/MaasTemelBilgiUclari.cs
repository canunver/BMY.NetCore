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
using TNS.HRC;
using TNS.KYM;

namespace Istemci.HarcamaSurecleri
{
    /// <summary>Maaş temel bilgisi tablosunun bir satırı (eski MTB sınıfı): bir değişkenin 12 aylık değeri, a1 = Ocak ... a12 = Aralık.</summary>
    public class MaasTemelSatiri
    {
        public int kod { get; set; }
        public string ad { get; set; }
        public decimal a1 { get; set; }
        public decimal a2 { get; set; }
        public decimal a3 { get; set; }
        public decimal a4 { get; set; }
        public decimal a5 { get; set; }
        public decimal a6 { get; set; }
        public decimal a7 { get; set; }
        public decimal a8 { get; set; }
        public decimal a9 { get; set; }
        public decimal a10 { get; set; }
        public decimal a11 { get; set; }
        public decimal a12 { get; set; }

        public decimal Deger(int ay)
        {
            switch (ay)
            {
                case 1: return a1; case 2: return a2; case 3: return a3; case 4: return a4;
                case 5: return a5; case 6: return a6; case 7: return a7; case 8: return a8;
                case 9: return a9; case 10: return a10; case 11: return a11; case 12: return a12;
                default: return 0;
            }
        }

        public void DegerAta(int ay, decimal deger)
        {
            switch (ay)
            {
                case 1: a1 = deger; break; case 2: a2 = deger; break; case 3: a3 = deger; break; case 4: a4 = deger; break;
                case 5: a5 = deger; break; case 6: a6 = deger; break; case 7: a7 = deger; break; case 8: a8 = deger; break;
                case 9: a9 = deger; break; case 10: a10 = deger; break; case 11: a11 = deger; break; case 12: a12 = deger; break;
            }
        }
    }

    /// <summary>Kaydet isteği: yıl ve tablodaki bütün satırlar.</summary>
    public class MaasTemelKayit
    {
        public int yil { get; set; }
        public List<MaasTemelSatiri> satirlar { get; set; }
    }

    /// <summary>
    /// Maaş Temel Bilgileri Tanımlama (eski HarcamaSurecleri/TanimPSLMaasTemelBilgi.aspx; yeni sayfa HarcamaSurecleri/TanimPSLMaasTemelBilgi) uçları:
    ///   GET  /api/maastemel/listele?yil=   yılın 23 değişken x 12 ay tablosu (eski Listele)
    ///   POST /api/maastemel/kaydet         { yil, satirlar } (eski btnKaydet_Click)
    /// İş kuralları eski sayfadaki gibi IHRCServis.PSLMaasTemelBilgisiListele / PSLMaasTemelBilgisiKaydet.
    /// </summary>
    public static class MaasTemelBilgiUclari
    {
        public const string SayfaAdresi = "../HarcamaSurecleri/TanimPSLMaasTemelBilgi.aspx";

        /// <summary>Eski MTB.KODSAY ve MTB.ad (resx FRMTMT018-032 + koddaki adlar).</summary>
        private static readonly string[] Adlar =
        {
            "En yüksek Memur Maaşı Göstergesi",
            "Aylık Katsayı",
            "Taban Aylık Katsayı",
            "Yan Ödeme Katsayısı",
            "SSK İşveren Payı (%19,5)",
            "SSK İşçi Payı (%14)",
            "İşsizlik Sigortası İşveren Payı",
            "İşsizlik Sigortası İşçi Payı",
            "Damga Vergisi Oranı (%)",
            "Brüt Asgari Ücret",
            "İş Kazaları ve Meslek Hastalıkları Sigortası",
            "Analık Sigortası",
            "Hastalık Sigortası",
            "Malüllük, Yaşlılık ve Ölüm Sigortası",
            "Net Asgari Ücret",
            "Sakatlık İnd. (1. Derece)",
            "Sakatlık İnd. (2. Derece)",
            "Sakatlık İnd. (3. Derece)",
            "Kira Bedeli",
            "Yakıt Bedeli",
            "SSK Tavan",
            "Kullanılmıyor",
            "Aile Yardımı Katsayısı"
        };

        /// <summary>Yıl kutusunun alt sınırı (eski SpinnerField sınırsızdı).</summary>
        private const int IlkYil = 2000;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/maastemel/listele", Listele);
            app.MapPost("/api/maastemel/kaydet", Kaydet);
        }

        /// <summary>Menü yetkisi (eski sayfada GenelIslemler.SayfayaGirisYapabilirMi yorumdaydı; menüdeki Yetki yine de geçerli).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = (sonuc.hataStr ?? "").Replace("<br>", " ").Replace("<br/>", " ");
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata.Trim();
        }

        private static List<int> Yillar(int yil)
        {
            int son = Math.Max(DateTime.Now.Year + 1, yil);
            List<int> yillar = new List<int>();
            for (int i = Math.Min(IlkYil, yil > 0 ? yil : IlkYil); i <= son; i++) yillar.Add(i);
            return yillar;
        }

        private static IResult Listele(HttpContext baglam, IWebHostEnvironment ortam, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            int y = yil ?? DateTime.Now.Year;
            List<MaasTemelSatiri> satirlar = new List<MaasTemelSatiri>();
            for (int i = 0; i < Adlar.Length; i++)
                satirlar.Add(new MaasTemelSatiri { kod = i, ad = Adlar[i] });

            if (y <= 0) return Results.Json(new { basarili = true, yil = y, yillar = Yillar(DateTime.Now.Year), satirlar });

            PSLMaasTemelBilgisi mtb = new PSLMaasTemelBilgisi();
            mtb.yil = y;
            ObjectArray bilgi = TNS.HRC.Arac.Tanimla().PSLMaasTemelBilgisiListele(kullanan, mtb);
            if (!bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi.sonuc, "Maaş temel bilgileri okunamadı."));

            foreach (PSLMaasTemelBilgisi t in bilgi.objeler)
            {
                if (t.ay < 1 || t.ay > 12) continue;
                decimal[] d =
                {
                    t.enYuksMemurMaasGost, t.aylikKatsayi, t.tabanAylikKatsayi, t.yanOdemeKatsayi, t.sskIsverenPayiOran,
                    t.sskIsciPayiOran, t.sskIssizSigIsVerenPayiOran, t.sskIssizSigIsciPayiOran, t.damgaVergisiOran, t.brutAsgariUcret,
                    t.isKazasiMeslekHast, t.analikSigortasi, t.hastalikSigortasi, t.malulYasliOlumSigortasi, t.netAsgariUcret,
                    t.sakatlikIndirimi1, t.sakatlikIndirimi2, t.sakatlikIndirimi3, t.kira1, t.yakit1,
                    t.sskTavan, t.topluGorusmeSabiti, t.aileYardimiKatsayisi
                };
                for (int i = 0; i < d.Length; i++) satirlar[i].DegerAta(t.ay, d[i]);
            }
            return Results.Json(new { basarili = true, yil = y, yillar = Yillar(y), satirlar });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, MaasTemelKayit istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.satirlar == null) return Results.BadRequest();

            // Eski btnKaydet_Click: her ay için bir PSLMaasTemelBilgisi, satır kodu alanı belirler.
            ObjectArray bilgi = new ObjectArray();
            for (int ay = 1; ay <= 12; ay++)
            {
                PSLMaasTemelBilgisi maas = new PSLMaasTemelBilgisi();
                maas.yil = istek.yil;
                maas.ay = ay;
                foreach (MaasTemelSatiri s in istek.satirlar)
                {
                    if (s == null) continue;
                    decimal v = s.Deger(ay);
                    switch (s.kod)
                    {
                        case 0: maas.enYuksMemurMaasGost = v; break;
                        case 1: maas.aylikKatsayi = v; break;
                        case 2: maas.tabanAylikKatsayi = v; break;
                        case 3: maas.yanOdemeKatsayi = v; break;
                        case 4: maas.sskIsverenPayiOran = v; break;
                        case 5: maas.sskIsciPayiOran = v; break;
                        case 6: maas.sskIssizSigIsVerenPayiOran = v; break;
                        case 7: maas.sskIssizSigIsciPayiOran = v; break;
                        case 8: maas.damgaVergisiOran = v; break;
                        case 9: maas.brutAsgariUcret = v; break;
                        case 10: maas.isKazasiMeslekHast = v; break;
                        case 11: maas.analikSigortasi = v; break;
                        case 12: maas.hastalikSigortasi = v; break;
                        case 13: maas.malulYasliOlumSigortasi = v; break;
                        case 14: maas.netAsgariUcret = v; break;
                        case 15: maas.sakatlikIndirimi1 = v; break;
                        case 16: maas.sakatlikIndirimi2 = v; break;
                        case 17: maas.sakatlikIndirimi3 = v; break;
                        case 18: maas.kira1 = v; break;
                        case 19: maas.yakit1 = v; break;
                        case 20: maas.sskTavan = v; break;
                        case 21: maas.topluGorusmeSabiti = v; break;
                        case 22: maas.aileYardimiKatsayisi = v; break;
                    }
                }
                bilgi.objeler.Add(maas);
            }

            Sonuc sonuc = TNS.HRC.Arac.Tanimla().PSLMaasTemelBilgisiKaydet(kullanan, bilgi);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kayıt yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = string.IsNullOrWhiteSpace(sonuc.bilgiStr) ? "Kayıt işlemi başarıyla gerçekleşti." : sonuc.bilgiStr });
        }
    }
}
