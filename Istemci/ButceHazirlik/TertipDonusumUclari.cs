using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    public class TertipEskiYeni
    {
        public string TertipKoduEski { get; set; }
        public string TertipKoduYeni { get; set; }
    }

    public class TertipDonusumIstegi
    {
        public int Yil { get; set; }
        public List<TertipEskiYeni> Tertipler { get; set; }
    }

    /// <summary>
    /// Tertip dönüşümü sayfasının (eski ButceHazirlikv2/TertipDonusumFormu.aspx; bütçe teklifi ekranındaki "Tertip Dönüşüm" tuşu) uçları:
    ///   POST /api/tertipdonusum/donustur  analitik tertibi boş satırlara programdan, program tertibi boş satırlara analitikten çevirir
    ///   POST /api/tertipdonusum/indir     tabloyu Excel'e yazar
    ///   POST /api/tertipdonusum/yukle     Excel'deki satırları (analitik, program) tabloya getirir
    /// Sayfayı sistem yöneticisi ve gider bütçe teklifi ekranının yetkilisi kullanabilir.
    /// </summary>
    public static class TertipDonusumUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapPost("/api/tertipdonusum/donustur", Donustur);
            app.MapPost("/api/tertipdonusum/indir", Indir);
            app.MapPost("/api/tertipdonusum/yukle", Yukle).DisableAntiforgery();
        }

        /// <summary>Eski sayfada yetki denetimi yoktu; burada sistem yöneticisi ya da gider bütçe teklifi ekranının yetkilisi.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return TertipTanimUclari.YetkiVar(kullanan, uygulamaKlasoru, 1);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Eleman(string[] dizi, int no) { return dizi != null && no >= 0 && no < dizi.Length ? dizi[no] : ""; }

        /// <summary>Dönüştür (eski btnDonustur_Click): boş olan taraf diğerinden üretilir.</summary>
        private static async Task<IResult> Donustur(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TertipDonusumIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new TertipDonusumIstegi();
            int yil = istek.Yil;
            if (yil < 2000) return Hata("Yıl hatalı!");

            try
            {
                TNS.BHMv2.Arac.ProgramButceKodCevirSifirla(yil);
                Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                List<TertipEskiYeni> al = new List<TertipEskiYeni>();
                foreach (TertipEskiYeni s in istek.Tertipler ?? new List<TertipEskiYeni>())
                {
                    TertipEskiYeni tka = new TertipEskiYeni { TertipKoduEski = s?.TertipKoduEski ?? "", TertipKoduYeni = s?.TertipKoduYeni ?? "" };
                    if (string.IsNullOrWhiteSpace(tka.TertipKoduEski)) tka.TertipKoduEski = ProgramdanAnalitige(t, yil, tka.TertipKoduYeni);
                    if (string.IsNullOrWhiteSpace(tka.TertipKoduYeni)) tka.TertipKoduYeni = AnalitiktenPrograma(yil, tka.TertipKoduEski);
                    al.Add(tka);
                }
                return Results.Json(new { tertipler = al });
            }
            catch (Exception)
            {
                return Hata("Dönüştürme yapılamadı.");
            }
        }

        private static string AnalitiktenPrograma(int yil, string analitikTertipKodu)
        {
            if (string.IsNullOrWhiteSpace(analitikTertipKodu)) return "";
            return TNS.BHMv2.Arac.ProgramTertipOl(yil, analitikTertipKodu, false);
        }

        private static string ProgramdanAnalitige(Tertip t, int yil, string programTertipKodu)
        {
            if (string.IsNullOrWhiteSpace(programTertipKodu)) return "";
            string[] kodlar = programTertipKodu.Split('-');
            if (kodlar.Length == 0) return "";
            string[] prgParcalar = kodlar[0].Split('.');
            if (prgParcalar.Length != 4) return "";
            int.TryParse(prgParcalar[3], out int afaal);
            if (afaal <= 0) return "";
            string afaal_fon = TNS.BHMv2.Arac.ProgramButceKodaCevir(yil, 1200, prgParcalar[3]);
            if (string.IsNullOrWhiteSpace(afaal_fon)) return "";

            string[] a_f = afaal_fon.Split('.');
            t.TertipAta(Eleman(kodlar, 1) + "-" + Eleman(a_f, 1) + "-" + Eleman(kodlar, 2) + "-" + Eleman(kodlar, 3) + "-" + Eleman(a_f, 0));
            t.Noktala();
            return t.TertipStrOl();
        }

        /// <summary>İndir (eski btnDosyaSakla_Click): tablo Excel'e yazılır (sütunlar: analitik tertip, program tertip).</summary>
        private static async Task<IResult> Indir(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TertipDonusumIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                AsposeLisans.Yukle();
                string dosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.BosDosyaAc(dosyaAd);
                List<TertipEskiYeni> tertipler = istek?.Tertipler ?? new List<TertipEskiYeni>();
                for (int sat = 0; sat < tertipler.Count; sat++)
                {
                    XLS.HucreDegerYaz(sat, 0, tertipler[sat]?.TertipKoduEski ?? "");
                    XLS.HucreDegerYaz(sat, 1, tertipler[sat]?.TertipKoduYeni ?? "");
                }
                XLS.SutunGenislikAyarla(0, 0, 40);
                XLS.SutunGenislikAyarla(1, 1, 100);
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(dosyaAd, "Tertipler_" + DateTime.Now.Ticks, true, "xlsx");
            }
            catch (Exception)
            {
                return Hata("Dosya hazırlanamadı.");
            }
        }

        /// <summary>
        /// Dosya Yükle (eski btnDosyaYukle_Click): Excel'in ilk sütunu analitik, ikincisi program tertibidir; ikisi de boş olan
        /// ilk satırda durulur. Satırlar tabloya gelir.
        /// </summary>
        private static async Task<IResult> Yukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            IFormCollection form = await baglam.Request.ReadFormAsync();
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return Hata("Yüklemek istediğiniz dosyanın içi boş.");
            if (dosya.Length > 20 * 1024 * 1024) return Hata("Dosya çok büyük (en çok 20 MB).");
            string uzanti = Path.GetExtension(dosya.FileName ?? "").ToLowerInvariant();
            if (uzanti != ".xls" && uzanti != ".xlsx") uzanti = ".xlsx";
            string dosyaAdxls = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul() + uzanti;
            try
            {
                using (FileStream hedef = File.Create(dosyaAdxls))
                    await dosya.CopyToAsync(hedef);
                AsposeLisans.Yukle();
                List<TertipEskiYeni> al = new List<TertipEskiYeni>();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(dosyaAdxls);
                try
                {
                    for (int sat = 0; ; sat++)
                    {
                        TertipEskiYeni tka = new TertipEskiYeni { TertipKoduEski = XLS.HucreDegerAl(sat, 0), TertipKoduYeni = XLS.HucreDegerAl(sat, 1) };
                        // Eski sayfa iki sütundan biri boşsa duruyordu; dönüştürülecek satırlarda bir taraf boş olduğundan burada ikisi de boşsa durulur.
                        if (string.IsNullOrEmpty(tka.TertipKoduEski) && string.IsNullOrEmpty(tka.TertipKoduYeni)) break;
                        al.Add(tka);
                    }
                }
                finally
                {
                    XLS.DosyaKapat();
                }
                return Results.Json(new { tertipler = al });
            }
            catch (Exception)
            {
                return Hata("Dosya okunamadı.");
            }
            finally
            {
                try { File.Delete(dosyaAdxls); } catch { }
            }
        }
    }
}
