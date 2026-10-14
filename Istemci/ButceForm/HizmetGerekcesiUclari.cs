using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceForm
{
    public class GerekceKayitIstegi
    {
        public int Yil { get; set; }
        public string Birim { get; set; }
        public int FaalKod { get; set; }
        public string Gerekce { get; set; }
        public string Dayanak { get; set; }
        public string Maliyet { get; set; }
    }

    public class GerekceBirlestirIstegi
    {
        public int Yil { get; set; }
    }

    /// <summary>
    /// Faaliyet Gerekçesi sayfasının (eski ButceForm/HizmetGerekcesiv2.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/hizmetgerekce/tanim, /birimler?yil=, /faaliyetler?yil=&amp;birim=, /oku?yil=&amp;birim=&amp;faal=&amp;onceki=
    ///   POST /api/hizmetgerekce/kaydet, /birlestir (yalnızca sistem yöneticisi)
    ///   GET  /api/hizmetgerekce/rapor?yil=&amp;birim=&amp;faal= (faal=0: birimin bütün faaliyetleri; eski rapor kodu, Word .docx)
    /// İşlemler eskisi gibi TNS.BHF.Arac.Tanimla() ve TNS.BKD.Arac.Tanimla() üzerinden yapılır. Kullanıcı yalnızca
    /// "Bütçe Hazırlığı Görebileceği Birimler" yetki tipindeki birimleri görür (sistem yöneticisi hepsini).
    /// </summary>
    public static class HizmetGerekcesiUclari
    {
        private const string YetkiTipi = "Bütçe Hazırlığı Görebileceği Birimler";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/hizmetgerekce/tanim", Tanim);
            app.MapGet("/api/hizmetgerekce/birimler", Birimler);
            app.MapGet("/api/hizmetgerekce/faaliyetler", Faaliyetler);
            app.MapGet("/api/hizmetgerekce/oku", Oku);
            app.MapPost("/api/hizmetgerekce/kaydet", Kaydet);
            app.MapPost("/api/hizmetgerekce/birlestir", Birlestir);
            app.MapGet("/api/hizmetgerekce/rapor", Rapor);
        }

        /// <summary>Kullanıcı Faaliyet Gerekçesi sayfasını kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../ButceForm/HizmetGerekcesiv2.aspx");
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

        private static bool SistemYoneticisi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        /// <summary>Eski Listele süzgeci: sistem yöneticisi bütün birimleri, diğerleri yetki tipindeki birimleri görür.</summary>
        private static bool BirimGorebilir(Kullanici kullanan, string birim)
        {
            if (SistemYoneticisi(kullanan)) return true;
            return (kullanan.YetkiTipiDegeriBul(YetkiTipi) ?? "").Contains(birim);
        }

        /// <summary>Varsayılan kurum kodu (eski GenelIslemlerIstemci.VarsayilanKurumBul: /KURUMKOD).</summary>
        private static string KurumKod()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMKOD") ?? "";
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Sayfanın açılış bilgileri (eski Page_Load): hazırlık yılı, o yılın birimleri, Birleştir görünsün mü.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int yil = ButceKod.KodTanimUclari.YilBul(null);
            List<object> birimler;
            try
            {
                birimler = BirimListesi(kullanan, yil);
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Birimler okunamadı." });
            }
            return Results.Json(new { yil = yil, birimler = birimler, birlestirebilir = SistemYoneticisi(kullanan) });
        }

        private static IResult Birimler(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                return Results.Json(new { birimler = BirimListesi(kullanan, yil) });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Birimler okunamadı." });
            }
        }

        /// <summary>Eski BirimListele + Listele: kurumun bütün kurumsal kodları; adı boş ya da "." ile başlayanlar atlanır.</summary>
        private static List<object> BirimListesi(Kullanici kullanan, int yil)
        {
            List<object> liste = new List<object>();
            ABSKodlarVAN van = TNS.BKD.Arac.Tanimla().KodListeleSeviye(yil, "1", KurumKod(), -1, false);
            if (van == null || van.absKodlar == null) return liste;
            bool yonetici = SistemYoneticisi(kullanan);
            string ytDeger = kullanan.YetkiTipiDegeriBul(YetkiTipi) ?? "";
            foreach (ABSKod kod in van.absKodlar)
            {
                string ad = kod.Ad(kullanan.aktifKultur);
                if (string.IsNullOrEmpty(ad) || ad.StartsWith(".")) continue;
                if (yonetici || ytDeger.Contains(kod.kod))
                    liste.Add(new { kod = kod.kod, ad = kod.kod + "-" + ad });
            }
            return liste;
        }

        /// <summary>Eski NodeLoad: birimin faaliyetleri, Sgb.agac düğümü olarak ({kod, ad, altVar}).</summary>
        private static IResult Faaliyetler(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000 || string.IsNullOrWhiteSpace(birim)) return Results.Json(new { hata = "Yıl giriniz ve birim seçiniz" });
            birim = birim.Trim();
            if (!BirimGorebilir(kullanan, birim)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            List<object> dugumler = new List<object>();
            try
            {
                PrgKodYapi kriter = new PrgKodYapi();
                kriter.yil = yil;
                kriter.kodTur = (int)EnumPrgKodYapi.FAALIYET;
                kriter.ilgiliBirimKod = birim;
                ObjectArray bilgiler = TNS.BKD.Arac.Tanimla().PrgKodYapiListele(kullanan, kriter);
                if (bilgiler.sonuc != null && !bilgiler.sonuc.islemSonuc)
                    return Results.Json(new { hata = HataMetni(bilgiler.sonuc.hataStr, "Faaliyetler okunamadı.") });
                foreach (PrgKodYapi kod in bilgiler.objeler)
                    dugumler.Add(new { kod = kod.kod.ToString(), ad = kod.kod + "-" + kod.ad, altVar = false });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Faaliyetler okunamadı." });
            }
            return Results.Json(new { dugumler = dugumler });
        }

        /// <summary>Eski GerekceOku: Oku (seçili yıl) ve Önceki Yıldan Al (bir önceki yıl).</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim, int faal, bool? onceki)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000 || string.IsNullOrWhiteSpace(birim) || faal <= 0) return Results.Json(new { hata = "Faaliyet seçiniz." });
            birim = birim.Trim();
            if (!BirimGorebilir(kullanan, birim)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (onceki == true) yil -= 1;
            try
            {
                FaaliyetGerekce fg = TNS.BHF.Arac.Tanimla().FaaliyetGerekceOku(kullanan, yil, birim, faal);
                if (fg == null) fg = new FaaliyetGerekce();
                return Results.Json(new { gerekce = fg.gerekce ?? "", dayanak = fg.dayanak ?? "", maliyet = fg.maliyet ?? "" });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Gerekçe okunamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>Eski btnKaydet_Click.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, GerekceKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000 || string.IsNullOrWhiteSpace(istek.Birim) || istek.FaalKod <= 0)
                return Results.Json(new { basarili = false, hata = "Faaliyet seçiniz." });
            string birim = istek.Birim.Trim();
            if (!BirimGorebilir(kullanan, birim)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            FaaliyetGerekce fg = new FaaliyetGerekce();
            fg.gerekce = istek.Gerekce ?? "";
            fg.dayanak = istek.Dayanak ?? "";
            fg.maliyet = istek.Maliyet ?? "";

            Sonuc sonuc;
            try
            {
                sonuc = TNS.BHF.Arac.Tanimla().FaaliyetGerekcesiKaydet(kullanan, istek.Yil, birim, istek.FaalKod, fg);
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
            return Results.Json(new { basarili = true, bilgi = "Bilgiler başarıyla saklandı." });
        }

        /// <summary>Eski btnBirlestir_Click: kurumun boş gerekçeleri birimlerden alınanlarla doldurulur (yalnızca sistem yöneticisi).</summary>
        private static async Task<IResult> Birlestir(HttpContext baglam, IWebHostEnvironment ortam, GerekceBirlestirIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });

            Sonuc sonuc;
            try
            {
                sonuc = TNS.BHF.Arac.Tanimla().FaaliyetGerekceBirlestir(kullanan, istek.Yil, KurumKod());
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Birleştirme yapılamadı." });
            }
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Birleştirme yapılamadı.") });
            return Results.Json(new { basarili = true, bilgi = "Boş olan gerekçeler, birimlerden alınanlarla dolduruldu!" });
        }

        // ------------------------------------------------------------------ rapor (eski kodla)

        /// <summary>Eski Yazdir_TekTum: Yazdır Tek Faal. (faal = seçili faaliyet) ve Yazdır Tüm Faal. (faal = 0).</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim, int? faal)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000 || string.IsNullOrWhiteSpace(birim))
                return Results.Json(new { hata = "Yıl, Kurum veya Birim alanlarından biri veya bir kaçı boş bırakılmış.  Lütfen ilgili alanları kontrol edip tekrar deneyin." });
            birim = birim.Trim();
            if (!BirimGorebilir(kullanan, birim)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            int faalKod = faal ?? 0;
            try
            {
                string dosya = global::ButceForm.HizmetGerekcesiRaporu.HizmetGerekcesiHazirla(TNS.BHF.Arac.Tanimla(), kullanan, yil, birim, faalKod, KurumKod(), DosyaIslem.DosyaAdUret());
                dosya = global::ButceForm.HizmetGerekcesiRaporu.DocxeCevir(dosya);
                return DosyaIslem.DosyaGonder(dosya, "Faal_Ger_" + birim + "_" + faalKod + "_" + DateTime.Now.Ticks.ToString(), true, "docx");
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Hata oluştu." });
            }
        }
    }
}
