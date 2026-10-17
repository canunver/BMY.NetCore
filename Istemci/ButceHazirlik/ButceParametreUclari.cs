using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    public class ButceParametreIstegi
    {
        public int Yil { get; set; }
        /// <summary>Değiştirilecek parametrenin kimliği (Parametreler listesindeki kimlik).</summary>
        public string Kimlik { get; set; }
    }

    /// <summary>
    /// Bütçe Hazırlık Parametreleri sayfasının (eski ButceHazirlikv2/ButceParametre.aspx) uçları:
    ///   GET  /api/butceparametre/durum?yil=   yılın parametreleri ve durumları (yıl verilmezse bütçe hazırlık yılı)
    ///   POST /api/butceparametre/degistir     bir parametrenin durumunu tersine çevirir (eski ...Degis_Click)
    /// Parametreler yıla bağlı DEG değişkenleridir; boş ya da "0" kapalı, başka değer açık demektir. Eski sayfada gizli olan
    /// "Bütçe Fişi/Taşınır" satırı alınmadı.
    /// </summary>
    public static class ButceParametreUclari
    {
        public const string SayfaAdresi = ButceTanimOrtak.Klasor + "ButceParametre.aspx";

        private class Parametre
        {
            public string kimlik;
            public string etiket;
            public string degisken;
            public string acikMetin;
            public string kapaliMetin;
            /// <summary>Kapalıyken açılınca yazılacak değer.</summary>
            public string doluDeger;
        }

        private static readonly Parametre[] Parametreler =
        {
            new Parametre { kimlik = "altfaaliyet", etiket = "Alt Faaliyet", degisken = "TekAltFaaliyetKod", acikMetin = "Kullanma", kapaliMetin = "Kullan" },
            new Parametre { kimlik = "kesinhesap", etiket = "Bütçe Fişi/Kesin Hesap", degisken = "/ButceFisiKesinHesap", acikMetin = "Göster", kapaliMetin = "Gizle", doluDeger = "1" },
            new Parametre { kimlik = "harcama", etiket = "Bütçe Fişi/Harcama", degisken = "/ButceFisiHarcama", acikMetin = "Göster", kapaliMetin = "Gizle", doluDeger = "1" },
            new Parametre { kimlik = "personel", etiket = "Bütçe Fişi/Personel Dahil", degisken = "/ButceFisiPersonelDahil", acikMetin = "Evet", kapaliMetin = "Hayır", doluDeger = "1" },
            new Parametre { kimlik = "altfaaliyettablo", etiket = "Bütçe Fişi/Alt Faaliyet Tablosu", degisken = "/ButceFisiAltFaaliyet", acikMetin = "Gizle", kapaliMetin = "Göster", doluDeger = "1" },
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/butceparametre/durum", Durum);
            app.MapPost("/api/butceparametre/degistir", Degistir);
        }

        private static bool Acik(string deger)
        {
            return !string.IsNullOrWhiteSpace(deger) && deger != "0";
        }

        private static List<object> DurumListesi(int yil)
        {
            List<object> liste = new List<object>();
            foreach (Parametre p in Parametreler)
            {
                bool acik = Acik(TNS.DEG.Arac.DegiskenDegerBul(yil, p.degisken));
                liste.Add(new { kimlik = p.kimlik, etiket = p.etiket, acik = acik, durum = acik ? p.acikMetin : p.kapaliMetin });
            }
            return liste;
        }

        private static IResult Durum(HttpContext baglam, IWebHostEnvironment ortam, int? yil)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            try
            {
                int y = yil ?? 0;
                if (y < 1000) y = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/BUTCEHAZIRLIK"), DateTime.Now.Year + 1);
                return Results.Json(new { yil = y, enBuyukYil = DateTime.Now.Year + 2, parametreler = DurumListesi(y) });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Parametreler okunamadı.");
            }
        }

        private static async Task<IResult> Degistir(HttpContext baglam, IWebHostEnvironment ortam, ButceParametreIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAdresi);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2003) return ButceTanimOrtak.Basarisiz("Yıl hatalı!");
            Parametre p = Array.Find(Parametreler, x => x.kimlik == istek.Kimlik);
            if (p == null) return Results.BadRequest();
            try
            {
                int yil = istek.Yil;
                bool acik = Acik(TNS.DEG.Arac.DegiskenDegerBul(yil, p.degisken));
                string yeniDeger;
                if (acik)
                    yeniDeger = "0";
                else if (p.kimlik == "altfaaliyet")
                    yeniDeger = TekAltFaaliyetHazirla(kullanan, yil);
                else
                    yeniDeger = p.doluDeger;
                TNS.DEG.Arac.DegiskenDegerAta(yil, p.degisken, yeniDeger);
                return Results.Json(new { basarili = true, bilgi = p.etiket + " değiştirildi.", parametreler = DurumListesi(yil) });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Değiştirilemedi.");
            }
        }

        /// <summary>
        /// Eski FaaliyetDegis_Click: alt faaliyet kullanılmayacaksa her düzeyin "1" kodundan oluşan tek bir faaliyet kodu
        /// (ör. 01.001.01) yazılır; o yılın faaliyet kodlarında olmayan düzeyler "Kurum Faaliyeti" adıyla eklenir.
        /// </summary>
        private static string TekAltFaaliyetHazirla(Kullanici kullanan, int yil)
        {
            string kodKirilim = TNS.DEG.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD14/KODYAPI") ?? "";
            string[] kodKirilimDizi = kodKirilim.Split('.');
            string aktifKod = "";
            TNSCollection kodlar = new TNSCollection();
            for (int i = 0; i < kodKirilimDizi.Length; i++)
            {
                if (!string.IsNullOrEmpty(aktifKod)) aktifKod += ".";
                aktifKod = aktifKod + "1".PadLeft(kodKirilimDizi[i].Length, '0');
                string ad = TNS.UZY.Arac.Tanimla().UzayDegeriStr(new Kullanici(), "ABSPEBKOD", yil.ToString() + "-" + aktifKod, true, null);
                if (string.IsNullOrEmpty(ad))
                {
                    ABSKod k = new ABSKod();
                    k.kod = aktifKod;
                    k.DilEkle(TNS.BKD.Arac.Diller(0), "Kurum Faaliyeti", "");
                    kodlar.Add(k);
                }
            }
            if (kodlar.Count > 0)
                TNS.BKD.Arac.Tanimla().KodKaydet(kullanan, yil, "14", kodlar);
            return aktifKod;
        }
    }
}
