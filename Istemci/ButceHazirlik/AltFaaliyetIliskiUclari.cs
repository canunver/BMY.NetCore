using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

namespace Istemci.ButceHazirlik
{
    public class AltFaaliyetIliskiSatiri
    {
        public int Yil { get; set; }
        public string PebKod { get; set; }
        /// <summary>tur 1: ilişkili faaliyet ("kurKod-F-fpKod" / "kurKod-S-smKod"); boşsa ilişki kalkar.</summary>
        public string FaalKod { get; set; }
        /// <summary>tur 2: ilişkili DPT alt projesi ve kaynağı ("altProje-kaynakNo"); boşsa ilişki kalkar.</summary>
        public string ProjeKod { get; set; }
    }

    public class AltFaaliyetIliskiKayitIstegi
    {
        public int Tur { get; set; }
        public string Senaryo { get; set; }
        public List<AltFaaliyetIliskiSatiri> Satirlar { get; set; }
    }

    public class AltFaaliyetBirimIstegi
    {
        public int Tur { get; set; }
        public int Yil { get; set; }
        public string PebKod { get; set; }
        public List<string> Sablonlar { get; set; }
    }

    /// <summary>
    /// Alt Faaliyet İlişkileri (eski ButceHazirlikv2/AltFaaliyetIliski.aspx?tur=1 faaliyet bağı, ?tur=2 DPT proje bağı) ve satırdaki
    /// düğmelerle açılan pencerelerin (eski AltProjeSozluk.aspx, PerformansButce/FaalSabitSozluk.aspx,
    /// ListeKurumsalYapiParametreForm.aspx) uçları:
    ///   GET  /api/altfaaliyetiliski/tanim?tur=, /liste?tur=&amp;yil=&amp;senaryo=&amp;kod=&amp;ad=,
    ///        /altprojeler?tur=&amp;yil=&amp;senaryo=, /faaliyetler?tur=&amp;yil=, /birim?tur=&amp;yil=&amp;peb=, /birimkodlar?tur=&amp;yil=&amp;ust=
    ///   POST /api/altfaaliyetiliski/kaydet, /birim
    /// Gider / gelir tertipleri düğmeleri TertipTanimFormu sayfasını açar. Sayfayı menüde ilgili seçeneği (tur) görebilenler kullanır;
    /// ilişki kaydetmeye iş kuralları yalnızca sistem yöneticisine (tur 1'de performans bütçe yöneticisine de) izin verir.
    /// </summary>
    public static class AltFaaliyetIliskiUclari
    {
        public const string Adres = "../ButceHazirlikv2/AltFaaliyetIliski.aspx?tur=";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/altfaaliyetiliski/tanim", Tanim);
            app.MapGet("/api/altfaaliyetiliski/liste", Liste);
            app.MapGet("/api/altfaaliyetiliski/altprojeler", AltProjeler);
            app.MapGet("/api/altfaaliyetiliski/faaliyetler", Faaliyetler);
            app.MapGet("/api/altfaaliyetiliski/birim", BirimOku);
            app.MapGet("/api/altfaaliyetiliski/birimkodlar", BirimKodlar);
            app.MapPost("/api/altfaaliyetiliski/kaydet", Kaydet);
            app.MapPost("/api/altfaaliyetiliski/birim", BirimKaydet);
        }

        public static int TurBul(int tur) { return tur == 2 ? 2 : 1; }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, int tur)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Adres + TurBul(tur));
        }

        /// <summary>Sayfanın iki seçeneğinden (tur 1 / 2) birini görebilen; tertip tanımları sayfası da bunu kullanır.</summary>
        public static bool YetkiVarHerhangi(Kullanici kullanan, string uygulamaKlasoru)
        {
            return YetkiVar(kullanan, uygulamaKlasoru, 1) || YetkiVar(kullanan, uygulamaKlasoru, 2);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, int tur, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, tur)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static async Task<(IResult hata, Kullanici kullanan)> DenetleYaz(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, int tur)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return (Results.BadRequest(), null);
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            return (ret, kullanan);
        }

        private static string Temiz(string s) { return YatProjeOrtak.Temiz(s); }

        private static IResult Hata(string hata) { return YatProjeOrtak.Hata(hata); }

        // ------------------------------------------------------------------ ana liste

        /// <summary>
        /// Açılış (eski Page_Load): yıl bütçe hazırlık yılı; tur 2'de aşamalar ve eskisi gibi yedinci aşama seçili gelir.
        /// </summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int tur)
        {
            tur = TurBul(tur);
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                // Eski GenelIslemlerIstemci.HazirlikYiliBul
                int yil = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/BUTCEHAZIRLIK"), DateTime.Now.Year + 1);
                List<object> asamalar = new List<object>();
                string senaryo = "";
                if (tur == 2)
                {
                    ButceSenaryo[] senaryolar = TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null);
                    if (senaryolar != null)
                        foreach (ButceSenaryo bs in senaryolar)
                            asamalar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });
                    // Eski ddlSenaryo.SelectedIndex = 6
                    if (senaryolar != null && senaryolar.Length > 0)
                        senaryo = senaryolar[Math.Min(6, senaryolar.Length - 1)].kod;
                }
                return Results.Json(new { tur = tur, yil = yil, asamalar = asamalar, senaryo = senaryo });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Listele (eski Listele): kod / ad maskesine uyan alt faaliyetler ve ilişkileri.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string senaryo, string kod, string ad)
        {
            tur = TurBul(tur);
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                AltFaaliyet form = new AltFaaliyet();
                form.yil = yil;
                form.pebKod = Temiz(kod);
                form.pebAd = Temiz(ad);
                form.kodUzunluk = (TNS.DEG.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD14/KODYAPI") ?? "").Replace(".", "").Length;
                form.iliskileriOku = tur;
                AltFaaliyet[] liste = TNS.BHMv2.Arac.Tanimla().AltFaaliyetListele(kullanan, form, Temiz(senaryo));
                List<object> satirlar = new List<object>();
                if (liste != null)
                    foreach (AltFaaliyet p in liste)
                        satirlar.Add(new
                        {
                            yil = p.yil, kod = p.pebKod, ad = p.pebAd,
                            faalKod = p.FaaliyetBagKod, faalAd = p.faaliyetAd ?? "",
                            projeKod = p.AltProjeKaynakKod, projeAd = p.altProjeKaynakAd ?? ""
                        });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(YatProjeOrtak.HataMetni(ex.Message, "Alt faaliyetler okunamadı."));
            }
        }

        /// <summary>Kaydet (eski btnKaydet_Click): yalnızca değişen satırlar gönderilir; boş ilişki eskisini kaldırır.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, AltFaaliyetIliskiKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (istek == null) return Results.BadRequest();
            int tur = TurBul(istek.Tur);
            var (ret, kullanan) = await DenetleYaz(baglam, ortam, sahtecilik, tur);
            if (ret != null) return ret;
            if (istek.Satirlar == null || istek.Satirlar.Count == 0)
                return Results.Json(new { basarili = false, hata = "İşlem yapılacak kayıt bulunamadı!" });
            try
            {
                List<AltFaaliyet> liste = new List<AltFaaliyet>();
                foreach (AltFaaliyetIliskiSatiri s in istek.Satirlar)
                {
                    if (s == null || string.IsNullOrWhiteSpace(s.PebKod)) continue;
                    AltFaaliyet af = new AltFaaliyet();
                    af.yil = s.Yil;
                    af.pebKod = Temiz(s.PebKod);
                    af.FaaliyetBagKod = Temiz(s.FaalKod);
                    af.AltProjeKaynakKod = Temiz(s.ProjeKod);
                    liste.Add(af);
                }
                if (liste.Count == 0) return Results.Json(new { basarili = false, hata = "İşlem yapılacak kayıt bulunamadı!" });
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().AltFaaliyetIliskiKaydet(kullanan, liste.ToArray(), tur, Temiz(istek.Senaryo));
                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(sonuc == null ? null : sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(ex.Message, "Kayıt işlemi başarısız.") });
            }
        }

        // ------------------------------------------------------------------ seçim pencereleri

        /// <summary>
        /// DPT projesi seçimi (eski AltProjeSozluk.aspx): yılın ve aşamanın alt projeleri, her birinin ilk bütçe kaynağıyla
        /// ("altProje-kaynakNo", "alt proje adı - kaynak adı (türü)").
        /// </summary>
        private static IResult AltProjeler(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string senaryo)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 1000) yil = DateTime.Now.Year;
            try
            {
                TNS.YAT.BilgiVAN apler = TNS.YAT.Arac.Tanimla().AltProjeListele(kullanan, yil, Temiz(senaryo), null, "", true, false);
                if (apler == null || !apler.sonuc.islemSonuc)
                    return Hata(YatProjeOrtak.HataMetni(apler == null ? null : apler.sonuc.hataStr, "Alt projeler okunamadı."));
                List<object> dugumler = new List<object>();
                foreach (TNS.YAT.AltProje ap in apler.liste)
                {
                    if (ap.butceKaynaklari == null) continue;
                    foreach (TNS.YAT.ButceKaynak bk in ap.butceKaynaklari)
                    {
                        dugumler.Add(new { kod = ap.kod + "-" + bk.kaynakNo, ad = ap.ad + " - " + bk.TurAdGetir(true) });
                        break;
                    }
                }
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Alt projeler okunamadı.");
            }
        }

        /// <summary>
        /// Faaliyet seçimi (eski PerformansButce/FaalSabitSozluk.aspx): sabit maliyetler ("kur-S-kod") ve faaliyet / projeler ("kur-F-kod").
        /// </summary>
        private static IResult Faaliyetler(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 1000) yil = DateTime.Now.Year;
            try
            {
                TNS.PEB.IPEBServis servis = TNS.PEB.Arac.Tanimla();
                List<object> dugumler = new List<object>();

                TNS.PEB.SabitMaliyet smSor = new TNS.PEB.SabitMaliyet();
                smSor.yil = yil;
                ObjectArray oa = servis.SabitMaliyetListele(kullanan, smSor, false);
                if (oa == null || !oa.sonuc.islemSonuc)
                    return Hata(YatProjeOrtak.HataMetni(oa == null ? null : oa.sonuc.hataStr, "Faaliyetler okunamadı."));
                foreach (TNS.PEB.SabitMaliyet sm in oa.objeler)
                {
                    string kod = sm.kurKod + "-S-" + sm.smKod;
                    dugumler.Add(new { kod = kod, ad = kod + " - " + sm.smAd });
                }

                TNS.PEB.FaaliyetProje fpSor = new TNS.PEB.FaaliyetProje();
                fpSor.yil = yil;
                oa = servis.FaaliyetProjeListele(kullanan, fpSor, false, false);
                if (oa == null || !oa.sonuc.islemSonuc)
                    return Hata(YatProjeOrtak.HataMetni(oa == null ? null : oa.sonuc.hataStr, "Faaliyetler okunamadı."));
                foreach (TNS.PEB.FaaliyetProje fp in oa.objeler)
                {
                    string kod = fp.kurKod + "-F-" + fp.fpKod;
                    dugumler.Add(new { kod = kod, ad = kod + " - " + fp.fpAd });
                }
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Faaliyetler okunamadı.");
            }
        }

        // ------------------------------------------------------------------ Birimler (eski ListeKurumsalYapiParametreForm.aspx)

        private static AltFaaliyet TanimOku(Kullanici kullanan, int yil, string peb, out string hata)
        {
            hata = null;
            if (yil <= 0 || string.IsNullOrEmpty(peb)) { hata = "Anahtar alanları girmelisiniz."; return null; }
            AltFaaliyet form = new AltFaaliyet();
            form.yil = yil;
            form.pebKod = peb;
            form = TNS.BHMv2.Arac.Tanimla().AltFaaliyetTanimOku(kullanan, form);
            if (form == null) hata = "Verilen kriterlere uygun kayıt bulunamadı.";
            return form;
        }

        /// <summary>Alt faaliyetin kurumsal yapı şablonları (MT.İl.İlçe.Birim).</summary>
        private static IResult BirimOku(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string peb)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                AltFaaliyet form = TanimOku(kullanan, yil, Temiz(peb), out string hata);
                if (form == null) return Hata(hata);
                return Results.Json(new { sablonlar = form.kurYapiSablonListe ?? new string[0] });
            }
            catch (Exception ex)
            {
                return Hata(YatProjeOrtak.HataMetni(ex.Message, "Birimler okunamadı."));
            }
        }

        /// <summary>Eski lbtnKaydet_Click: listedeki şablonlar alt faaliyet tanımına yazılır.</summary>
        private static async Task<IResult> BirimKaydet(HttpContext baglam, IWebHostEnvironment ortam, AltFaaliyetBirimIstegi istek, IAntiforgery sahtecilik)
        {
            if (istek == null) return Results.BadRequest();
            var (ret, kullanan) = await DenetleYaz(baglam, ortam, sahtecilik, istek.Tur);
            if (ret != null) return ret;
            try
            {
                AltFaaliyet form = TanimOku(kullanan, istek.Yil, Temiz(istek.PebKod), out string hata);
                if (form == null) return Results.Json(new { basarili = false, hata = hata });
                List<string> sablonlar = new List<string>();
                foreach (string s in istek.Sablonlar ?? new List<string>())
                    if (!string.IsNullOrWhiteSpace(s)) sablonlar.Add(s.Trim());
                form.kurYapiSablonListe = sablonlar.ToArray();
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().AltFaaliyetTanimKaydet(kullanan, form);
                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(sonuc == null ? null : sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(ex.Message, "Kayıt işlemi başarısız.") });
            }
        }

        /// <summary>Birim Seç ağacının bir düzeyi (eski KodAgac.aspx?kodTur=17, çoklu seçim).</summary>
        private static IResult BirimKodlar(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            ust = Temiz(ust);
            try
            {
                const string kodTur = "17";
                string kirilim = TNS.DEG.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD" + kodTur + "/KODYAPI") ?? "";
                int kirilimBoy = kirilim == "" ? 0 : kirilim.Split('.').Length;
                List<object> dugumler = new List<object>();
                int ustUz = 0;
                if (ust != "")
                {
                    ust = OrtakFonksiyonlar.KodNoktaKoy(kirilim, ust.Replace(".", ""));
                    ustUz = ust.Split('.').Length;
                }
                if (ustUz >= kirilimBoy) return Results.Json(new { dugumler = dugumler });
                bool altVar = ustUz < kirilimBoy - 1;
                ABSKodlarVAN kodlar = TNS.BKD.Arac.Tanimla().KodListeleSeviyeIkiYil(yil, kodTur, ust, 0, false, false, "", "", "", true, true, "", false);
                if (kodlar == null || !kodlar.sonuc.islemSonuc) return Hata("Kodlar okunamadı.");
                string dil = string.IsNullOrEmpty(kullanan.aktifKultur) ? TNS.BKD.Arac.Diller(0) : kullanan.aktifKultur;
                foreach (ABSKod k in kodlar.absKodlar)
                    dugumler.Add(new { kod = k.kod, ad = k.kod + "- " + k.Ad(dil), altVar = altVar });
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Kodlar okunamadı.");
            }
        }
    }
}
