using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Istemci.ButceHazirlik;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
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

namespace Istemci.ButceForm
{
    /// <summary>
    /// Ek Formlar sayfasının (eski ButceHazirlikv2/EkFormAna.aspx ve çerçevesinde açtığı ButceForm/*.aspx alt formları) ortak uçları:
    ///   GET /api/ekform/tanim               açılış bilgileri: kullanıcının yılı ve aşaması, aşamalar, birimler (eski Page_Load / StorelariOlustur)
    ///   GET /api/ekform/birimler?yil=       birim listesi (eski BHMv2Ortak.OrgtanDiziye)
    /// Her alt formun kendi uçları /api/ekform/{form}/... altındadır (EkForm*.cs); eski alt sayfaların gizli düğmelerinin
    /// (Listele, Kaydet, Yazdır, Detaydan Al ...) yaptığı iş kuralı çağrılarının aynısını yaparlar.
    /// Yetki: menüde Ek Formlar seçeneğini görebilenler ve sistem yöneticisi. Eski alt sayfalarda yetki denetimi yoktu
    /// (çerçeve sayfası denetliyordu).
    /// </summary>
    public static class EkFormUclari
    {
        public const string MenuAdres = "../ButceHazirlikv2/EkFormAna.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/tanim", Tanim);
            app.MapGet("/api/ekform/birimler", Birimler);

            EkFormHizmetBilgisi.Ekle(app);
            EkFormHizmetMaliyeti.Ekle(app);
            EkFormFizikselDegerler.Ekle(app);
            EkFormUluslararasi.Ekle(app);
            EkFormEdinilecekTasit.Ekle(app);
            EkFormKullanilanTasit.Ekle(app);
            EkFormHizmetAlimiTasit.Ekle(app);
            EkFormOzelGuvenlik.Ekle(app);
            EkFormTemizlik.Ekle(app);
            EkFormDonerSermaye.Ekle(app);
            EkFormTavanUstu.Ekle(app);
            EkFormHizmetGorevli.Ekle(app);
        }

        // ------------------------------------------------------------------ ortak yardımcılar (alt formlar da kullanır)

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), MenuAdres);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Okuma uçları için kullanıcı ve yetki denetimi; uygun değilse döndürülecek yanıt.</summary>
        internal static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        /// <summary>Yazan (POST) uçlar için: sahtecilik belirteci + kullanıcı ve yetki denetimi. Uygunsa kullanıcıyı, değilse yanıtı döndürür.</summary>
        internal static async Task<(IResult hata, Kullanici kullanan)> DenetleYaz(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return (Results.BadRequest(), null);
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            return (ret, kullanan);
        }

        internal static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        internal static IResult Tamam(string bilgi) { return Results.Json(new { basarili = true, bilgi = bilgi }); }

        /// <summary>Sunucunun döndürdüğü hata metni; boşsa ya da istisna ayrıntısı içeriyorsa varsayılan.</summary>
        internal static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        internal static string Temiz(string s) { return (s ?? "").Trim(); }

        /// <summary>Eski alt sayfalardaki "par.yil &gt; 0 &amp;&amp; par.kurkod != ''" denetimi.</summary>
        internal static bool SecimGecerli(int yil, string kur) { return yil > 0 && Temiz(kur) != ""; }

        internal const string SecimHatasi = "Yıl ve birim bilgisi boş olamaz!";

        /// <summary>Tarih tabloda gg.aa.yyyy olarak gösterilir ve girilir.</summary>
        internal static string TarihStr(TNSDateTime t)
        {
            if (t == null || t.isNull) return "";
            return t.Oku().ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
        }

        internal static TNSDateTime Tarih(string s)
        {
            s = Temiz(s);
            if (s == "") return new TNSDateTime();
            string[] bicimler = { "dd.MM.yyyy", "d.M.yyyy", "yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy" };
            if (DateTime.TryParseExact(s, bicimler, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return new TNSDateTime(s);
        }

        /// <summary>Eski raporların beklediği parametreler (ButceYil, KurKod, Senaryo).</summary>
        internal static Hashtable RaporParam(int yil, string kur, string senaryo)
        {
            Hashtable h = new Hashtable();
            h.Add("ButceYil", yil.ToString());
            h.Add("KurKod", Temiz(kur));
            h.Add("Senaryo", Temiz(senaryo));
            return h;
        }

        /// <summary>
        /// Eski kodla rapor dosyası üretip indirilecek yanıt olarak döndürür; hata olursa { hata } (Sgb.indir bunu bildirim olarak gösterir).
        /// </summary>
        internal static IResult RaporGonder(Func<string> hazirla, string gidenAd)
        {
            try
            {
                string dosya = hazirla();
                if (string.IsNullOrEmpty(dosya) || !System.IO.File.Exists(dosya)) return Hata("Rapor hazırlanamadı.");
                string uzanti = System.IO.Path.GetExtension(dosya).TrimStart('.');
                return DosyaIslem.DosyaGonder(dosya, gidenAd, true, uzanti);
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        internal static string ExcelTur() { return GenelIslemler.ExcelTur(); }

        // ------------------------------------------------------------------ açılış

        /// <summary>Eski Page_Load + StorelariOlustur: yıl (kullanıcının hazırlık yılı), aşama, yıl listesi, aşamalar ve birimler.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int yil = ButceRakamUclari.KullaniciYilBul(kullanan);
                List<int> yillar = new List<int>();
                for (int i = 2006; i <= DateTime.Now.Year + 1; i++) yillar.Add(i);
                if (!yillar.Contains(yil)) yillar.Add(yil);

                List<object> senaryolar = new List<object>();
                foreach (ButceSenaryo bs in TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null) ?? new ButceSenaryo[0])
                    senaryolar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });

                return Results.Json(new
                {
                    yil = yil,
                    yillar = yillar,
                    senaryo = ButceRakamUclari.KullaniciSenaryoBul(kullanan),
                    senaryolar = senaryolar,
                    kurum = TNS.DEG.Arac.VarsayilanKurumBul(),
                    birimler = BirimListesi(kullanan),
                    sistemYoneticisi = GenelGorevUclari.YetkiVar(kullanan)
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        private static IResult Birimler(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return Results.Json(new { birimler = BirimListesi(kullanan) });
            }
            catch (Exception)
            {
                return Hata("Birimler okunamadı.");
            }
        }

        /// <summary>Eski BHMv2Ortak.BirimStoreOl(kullanan, store, kurumEkle: true) -> OrtgtanDiziye(kullanan, 0, true).</summary>
        internal static List<object> BirimListesi(Kullanici kullanan)
        {
            string kurum = TNS.DEG.Arac.VarsayilanKurumBul();
            List<object> liste = new List<object>();
            liste.Add(new { kod = kurum, ad = ButceHazirlikv2.BHGiderRaporlari.KurumAd() });
            ABSKodlarVAN akv = TNS.BKD.Arac.Tanimla().BirimListele(kullanan, 0, kurum, true);
            if (akv != null && akv.sonuc.islemSonuc && akv.absKodlar != null)
            {
                foreach (ABSKod kod in akv.absKodlar)
                    liste.Add(new { kod = kod.kod, ad = kod.Ad() });
            }
            return liste;
        }
    }
}
