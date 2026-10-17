using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
using TNS.Raporlama;

namespace Istemci.ButceHazirlik
{
    public class FaaliyetRaporuKayitIstegi
    {
        public int Yil { get; set; }
        /// <summary>FaaliyetRaporuBilgi.rapor sırasıyla 18 metin (bölüm 4, 4.1, 4.2, Tablo 1..15 notları).</summary>
        public List<string> Metinler { get; set; }
    }

    /// <summary>
    /// Faaliyet Raporu (eski ButceHazirlikv2/FaaliyetRaporuForm.aspx) ve Faaliyet Raporları (eski FaaliyetRaporuAl.aspx) uçları:
    ///   GET  /api/faaliyetraporu/tanim, /oku?yil=
    ///   POST /api/faaliyetraporu/kaydet (yalnızca sistem yöneticisi; eski sayfada Kaydet ona görünürdü)
    ///   GET  /api/faaliyetraporu/rapor?rapor=Men1_99&amp;yil=&amp;grup=&amp;yikob=&amp;pdf=&amp;goster=
    /// Metinler eskisi gibi KYM BuyukMetinOku / BuyukMetinSakla ("BHMv2", yil_..); raporlar eski kodla (Rapor.xml grup 1,
    /// ButceHazirlikv2.FaaliyetRaporuRaporlari) üretilir.
    /// </summary>
    public static class FaaliyetRaporuUclari
    {
        public const string FormAdres = "../ButceHazirlikv2/FaaliyetRaporuForm.aspx";
        public const string AlAdres = "../ButceHazirlikv2/FaaliyetRaporuAl.aspx";

        // Eski FaaliyetRaporuAl.raporTanimlar (Form sayfasının Raporlar menüsü de aynı raporlardır).
        private static readonly string[][] Raporlar =
        {
            new[] { "Men1_99", "Faaliyet Raporu" },
            new[] { "Men1_7", "Tablo 7: Kar Amacı Gütmeyen Kuruluşlara Yapılan Transferler" },
            new[] { "Men1_8", "Tablo 8: Yatırım Ödenekleri" },
            new[] { "Men1_9", "Tablo 9: Sermaye Giderleri" },
            new[] { "Men1_10", "Tablo 10: Sermaye Giderleri, Dış Proje Kredileri" },
            new[] { "Men1_11", "Tablo 11: Sermaye Transferleri" },
            new[] { "Men1_12", "Tablo 12: Sermaye Transferleri, Dış Proje Kredileri" },
        };

        private static readonly string[] BolumAdlari =
        {
            "Faaliyet Raporu (4)", "Bütçe Uygulama Sonuçları (4.1)", "Temel Mali Tablolara İlişkin Açıklamalar (4.2)"
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/faaliyetraporu/tanim", Tanim);
            app.MapGet("/api/faaliyetraporu/oku", Oku);
            app.MapPost("/api/faaliyetraporu/kaydet", Kaydet);
            app.MapGet("/api/faaliyetraporu/rapor", Rapor);
        }

        public static bool FormYetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, FormAdres);
        }

        public static bool AlYetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, AlAdres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, bool yalnizcaForm, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            bool yetkili = FormYetkiVar(kullanan, ortam.ContentRootPath) || (!yalnizcaForm && AlYetkiVar(kullanan, ortam.ContentRootPath));
            if (!yetkili) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static bool SistemYoneticisi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        private static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata });
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        /// <summary>
        /// Açılış (eski Page_Load): yıl bu yıl; sistem yöneticisi Kaydet'i ve "PDF olarak" seçimini görür (diğerlerine rapor
        /// hep PDF üretilir); rapor listesi.
        /// </summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;
            List<object> raporlar = new List<object>();
            for (int i = 0; i < Raporlar.Length; i++)
                raporlar.Add(new { siraNo = i + 1, kod = Raporlar[i][0], ad = Raporlar[i][1] });
            return Results.Json(new { yil = DateTime.Now.Year, sistemYoneticisi = SistemYoneticisi(kullanan), raporlar = raporlar });
        }

        /// <summary>Oku (eski Listele): yılın bölüm metinleri ve tablo notları; tablo notlarının başlığı yıla göre (eski TabloAdBul).</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, true, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 1000) return Hata("Lütfen yıl giriniz.");
            try
            {
                global::ButceHazirlikv2.FaaliyetRaporuBilgi frb = global::ButceHazirlikv2.FaaliyetRaporuRaporlari.MetinleriOku(kullanan, yil);
                List<object> metinler = new List<object>();
                for (int i = 0; i < frb.rapor.Length; i++)
                {
                    string baslik;
                    if (i < BolumAdlari.Length)
                        baslik = BolumAdlari[i];
                    else
                    {
                        int tablo = i - 2;
                        string ad = global::ButceHazirlikv2.FaaliyetRaporuRaporlari.TabloAdBul(tablo, yil - 1, yil, 0);
                        baslik = "Tablo " + tablo + " Notu" + (string.IsNullOrEmpty(ad) ? "" : " (" + ad.TrimEnd(':') + ")");
                    }
                    metinler.Add(new { sira = i, baslik = baslik, tablo = i >= BolumAdlari.Length, metin = frb.rapor[i] ?? "" });
                }
                return Results.Json(new { yil = yil, metinler = metinler });
            }
            catch (Exception)
            {
                return Hata("Metinler okunamadı.");
            }
        }

        /// <summary>
        /// Kaydet (eski btnKaydet_Click): metinler sırayla BuyukMetinSakla ile yazılır, ilk hatada durulur. Eskisi Tablo 15 notunu
        /// okuyup göstermesine karşın kaydetmiyordu; burada o da kaydedilir.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, FaaliyetRaporuKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, true, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek.Yil < 1000) return Results.Json(new { basarili = false, hata = "Lütfen yıl giriniz." });
            if (istek.Metinler == null || istek.Metinler.Count != 18) return Results.BadRequest();
            try
            {
                IKYMServis servis = TNS.KYM.Arac.Tanimla();
                Sonuc s = new Sonuc(true, "", "");
                for (int i = 0; i < istek.Metinler.Count && s.islemSonuc; i++)
                    s = servis.BuyukMetinSakla(kullanan, "BHMv2", istek.Yil.ToString() + global::ButceHazirlikv2.FaaliyetRaporuRaporlari.MetinAnahtarEk(i), "tr", istek.Metinler[i] ?? "");
                if (!s.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(s.hataStr, "Kayıt yapılamadı.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt İşlemi Tamamlandı" });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = HataMetni(ex.Message, "Kayıt yapılamadı.") });
            }
        }

        /// <summary>
        /// Rapor (eski btnYazdir_Click / raporUret): RaporlamaServis ile Rapor.xml grup 1'deki rapor; parametreler eskisi gibi
        /// ButceYil, Format (pdf / docx), Grup (0 grupsuz, 1 sektör, 2 birim), Yikob (0 hepsi, 1 yok, 2 sadece).
        /// Sistem yöneticisi olmayanlara hep PDF üretilir. goster=1 ise PDF tarayıcıda açılır (eski "Göster").
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, string rapor, int yil, int? grup, int? yikob, int? pdf, int? goster)
        {
            IResult ret = Denetle(baglam, ortam, false, out Kullanici kullanan);
            if (ret != null) return ret;
            string[] tanim = null;
            foreach (string[] r in Raporlar)
                if (r[0] == rapor) tanim = r;
            if (tanim == null) return Hata("Listeden görmek istediğiniz raporu seçiniz.");
            if (yil < 1000) return Hata("Lütfen yıl giriniz.");
            int grp = grup ?? 0;
            if (grp < 0 || grp > 2) grp = 0;
            int ykb = yikob ?? 0;
            if (ykb < 0 || ykb > 2) ykb = 0;
            bool gosterilsin = goster == 1;
            bool pdfMi = gosterilsin || pdf != 0 || !SistemYoneticisi(kullanan);

            string[] raporBilgi = rapor.Substring(3).Split('_');
            try
            {
                Hashtable raporParam = new Hashtable();
                raporParam["ButceYil"] = yil.ToString();
                raporParam["Format"] = pdfMi ? "pdf" : "docx";
                raporParam["Grup"] = grp.ToString();
                raporParam["Yikob"] = ykb.ToString();
                string dosya = RaporlamaServis.TekRaporAl(OrtakFonksiyonlar.ConvertToInt(raporBilgi[0], 1), OrtakFonksiyonlar.ConvertToInt(raporBilgi[1], 1),
                    kullanan, null, raporParam, null);
                if (RaporlamaServis.MesajMi(dosya)) return Hata(dosya);
                string uzanti = Path.GetExtension(dosya).TrimStart('.').ToLowerInvariant();
                if (gosterilsin && uzanti == "pdf")
                {
                    byte[] icerik = File.ReadAllBytes(dosya);
                    try { File.Delete(dosya); } catch { }
                    return Results.File(icerik, "application/pdf");
                }
                string ad = "FaaliyetRaporu_" + raporBilgi[0] + "_" + raporBilgi[1] + "_" + yil + "_" + DosyaIslem.DosyaAdUretSade();
                return DosyaIslem.DosyaGonder(dosya, ad, true, uzanti);
            }
            catch (RaporException ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
