using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Istemci.ButceKod;
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
    public class ButceFisiGerekceIstegi
    {
        public int OdenekTur { get; set; }
        public int Yil { get; set; }
        public string Tertip { get; set; }
        public string Metin { get; set; }
        public string Sebep1 { get; set; }
        public string Sebep2 { get; set; }
    }

    public class ButceFisiYazdirIstegi
    {
        public int OdenekTur { get; set; }
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public List<string> Tertipler { get; set; }
    }

    /// <summary>
    /// Bütçe gider / gelir fişi sayfasının (eski ButceHazirlikv2/ButceFisiGiris.aspx?odenekTur=1|2; içindeki ButceFisiDetay.aspx
    /// çerçevesi ve ButceFisiGerekce.aspx penceresi) uçları:
    ///   GET  /api/butcefisi/tanim, /kodlar, /liste, /fis, /gerekce
    ///   POST /api/butcefisi/gerekce, /yazdir
    /// Yetki menüdeki ilgili adrese (gider ya da gelir fişi) göre denetlenir.
    /// </summary>
    public static class ButceFisiUclari
    {
        private const string SayfaAdres = "../ButceHazirlikv2/ButceFisiGiris.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/butcefisi/tanim", Tanim);
            app.MapGet("/api/butcefisi/kodlar", Kodlar);
            app.MapGet("/api/butcefisi/liste", Liste);
            app.MapGet("/api/butcefisi/fis", Fis);
            app.MapGet("/api/butcefisi/gerekce", GerekceOku);
            app.MapPost("/api/butcefisi/gerekce", GerekceKaydet);
            app.MapPost("/api/butcefisi/yazdir", Yazdir);
        }

        private static bool Gelir(int odenekTur) { return odenekTur == 2; }

        /// <summary>Sistem yöneticisi ya da menüdeki gider / gelir fişi seçeneğinin yetkilisi.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, int odenekTur)
        {
            if (kullanan == null) return false;
            if (Istemci.Organizasyon.GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                MenuOkuyucu menu = new MenuOkuyucu(kullanan);
                return menu.SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdres + "?odenekTur=" + (Gelir(odenekTur) ? 2 : 1));
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, odenekTur)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string Eleman(string[] dizi, int no) { return dizi != null && no < dizi.Length ? dizi[no] ?? "" : ""; }

        /// <summary>Açılış bilgileri (eski Page_Load / StorelariOlustur): yıllar, aşamalar, kullanıcının yılı ve aşaması, kurum.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur)
        {
            IResult ret = Denetle(baglam, ortam, odenekTur, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                List<int> yillar = new List<int>();
                for (int i = 2006; i <= DateTime.Now.Year + 1; i++) yillar.Add(i);
                List<object> senaryolar = new List<object>();
                foreach (ButceSenaryo bs in TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null) ?? new ButceSenaryo[0])
                    senaryolar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });
                return Results.Json(new
                {
                    baslik = Gelir(odenekTur) ? "Bütçe Gelir Fişi" : "Bütçe Gider Fişi",
                    yil = KodTanimUclari.YilBul(null),
                    yillar = yillar,
                    senaryo = ButceRakamUclari.KullaniciSenaryoBul(kullanan),
                    senaryolar = senaryolar,
                    kurum = TNS.DEG.Arac.VarsayilanKurumBul()
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Kod ağacının bir düzeyi (bütçe teklifi ekranındaki ağaçlarla aynı).</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int yil, string tur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, odenekTur, out Kullanici kullanan);
            if (ret != null) return ret;
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, tur, ust);
        }

        /// <summary>Seçime uyan tertipler (eski TertipListele / GridTertipListeDoldur); giderde personel giderleri parametreye göre çıkarılır.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int yil, string senaryo,
            string kur, string fon, string fin, string eko, string gelEko)
        {
            IResult ret = Denetle(baglam, ortam, odenekTur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl seçiniz.");
            senaryo = Temiz(senaryo);
            bool gelir = Gelir(odenekTur);
            try
            {
                Tertip tertip;
                UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
                EkDegerParam[] ekBilgiler = new EkDegerParam[1];
                if (gelir)
                {
                    tertip = new Tertip(yil, ENUMTabloKod.TABPEBGelir, Temiz(kur), Temiz(gelEko), "");
                    for (int i = 0; i < 2; i++) kodBilgileri.Add(new UcgenKodBilgi(tertip.KodNolar(i), tertip.KodNoKirilimSay(i)));
                    ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBGelir, 0, 0, senaryo);
                }
                else
                {
                    tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, Temiz(kur), Temiz(fon), Temiz(fin), Temiz(eko), "");
                    for (int i = 0; i < 5; i++) kodBilgileri.Add(new UcgenKodBilgi(tertip.KodNolar(i), tertip.KodNoKirilimSay(i)));
                    ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryo);
                }
                OdenekBilgi[] odnler = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, tertip, kodBilgileri, ekBilgiler, true, true);

                string personelDahilPar = TNS.DEG.Arac.DegiskenDegerBul(yil, "ButceFisiPersonelDahil");
                List<object> tertipler = new List<object>();
                foreach (OdenekBilgi o in odnler ?? new OdenekBilgi[0])
                {
                    int ekoKodYer = gelir ? 1 : 3;
                    string ekoKod = o.tertip.kodlar[ekoKodYer];
                    if (!gelir && ButceFisiOrtak.PersonelGizliMi(ekoKod, personelDahilPar)) continue;
                    tertipler.Add(new { tertipKod = o.tertip.TertipStrOl(), tertipAd = o.tertip.adlar[ekoKodYer] });
                }
                return Results.Json(new { tertipler = tertipler });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tertipler listelenemedi."));
            }
        }

        private static string FisIcerik(Kullanici kullanan, int odenekTur, int yil, string tertip, string senaryo, bool onceSayfaSonu)
        {
            if (Gelir(odenekTur))
                return ButceGelirFisiIcerik.SayfaOlustur(kullanan, yil, tertip, senaryo, onceSayfaSonu);
            return ButceFisiIcerik.SayfaOlustur(kullanan, yil, tertip, senaryo, onceSayfaSonu);
        }

        /// <summary>Seçilen tertibin bütçe fişi (eski ButceFisiDetay.aspx): HTML olarak.</summary>
        private static IResult Fis(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int yil, string senaryo, string tertip)
        {
            IResult ret = Denetle(baglam, ortam, odenekTur, out Kullanici kullanan);
            if (ret != null) return ret;
            tertip = Temiz(tertip);
            if (yil < 2000 || tertip == "") return Hata("Tertip seçiniz.");
            try
            {
                return Results.Json(new { html = FisIcerik(kullanan, odenekTur, yil, tertip, Temiz(senaryo), false) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Bütçe fişi hazırlanamadı."));
            }
        }

        /// <summary>Tertibin gerekçesi (eski ButceFisiGerekce Listele): giderde metin ve iki farklılaşma sebebi, gelirde yalnızca metin.</summary>
        private static IResult GerekceOku(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int yil, string tertip)
        {
            IResult ret = Denetle(baglam, ortam, odenekTur, out Kullanici kullanan);
            if (ret != null) return ret;
            tertip = Temiz(tertip);
            if (yil < 2000 || tertip == "") return Hata("Bu işlemi yapabilmek için önce tertip seçmelisiniz.");
            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                if (Gelir(odenekTur))
                {
                    Tertip t = new Tertip(yil, ENUMTabloKod.TABGelir);
                    t.TertipAta(tertip);
                    return Results.Json(new { metin = servis.ButceGelirFisiGerekceYaziGetir(kullanan, t) ?? "", sebep1 = "", sebep2 = "" });
                }
                else
                {
                    Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                    t.TertipAta(tertip);
                    string[] g = servis.ButceFisiGerekceYaziGetir(kullanan, t);
                    return Results.Json(new { metin = Eleman(g, 0), sebep1 = Eleman(g, 1), sebep2 = Eleman(g, 2) });
                }
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Gerekçe okunamadı."));
            }
        }

        /// <summary>Gerekçe saklanır (eski ButceFisiGerekce btnKaydet_Click).</summary>
        private static async Task<IResult> GerekceKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, ButceFisiGerekceIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new ButceFisiGerekceIstegi();
            IResult ret = Denetle(baglam, ortam, istek.OdenekTur, out Kullanici kullanan);
            if (ret != null) return ret;
            string tertip = Temiz(istek.Tertip);
            if (istek.Yil < 2000 || tertip == "") return Hata("Bu işlemi yapabilmek için önce tertip seçmelisiniz.");
            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                Sonuc sonuc;
                if (Gelir(istek.OdenekTur))
                {
                    Tertip t = new Tertip(istek.Yil, ENUMTabloKod.TABGelir);
                    t.TertipAta(tertip);
                    sonuc = servis.ButceGelirFisiGerekceKaydet(kullanan, t, istek.Metin ?? "");
                }
                else
                {
                    Tertip t = new Tertip(istek.Yil, ENUMTabloKod.TABPEBOdenek);
                    t.TertipAta(tertip);
                    sonuc = servis.ButceFisiGerekceKaydet(kullanan, t, new[] { istek.Metin ?? "", istek.Sebep1 ?? "", istek.Sebep2 ?? "" });
                }
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Gerekçe saklanamadı."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla tamamlandı." });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Gerekçe saklanamadı."));
            }
        }

        /// <summary>
        /// Yazdır (eski btnYazdir_Click): verilen tertiplerin bütçe fişleri arka arkaya (her biri yeni sayfada) Word'ün açtığı HTML
        /// (.doc) dosyasına yazılır (eski ButceHazirlikRaporlari.ButceFisiYazdir).
        /// </summary>
        private static async Task<IResult> Yazdir(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, ButceFisiYazdirIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new ButceFisiYazdirIstegi();
            IResult ret = Denetle(baglam, ortam, istek.OdenekTur, out Kullanici kullanan);
            if (ret != null) return ret;
            string senaryo = Temiz(istek.Senaryo);
            if (senaryo == "") senaryo = "00.00";
            try
            {
                string sayfa = "";
                bool tertipYok = true;
                foreach (string trtp in istek.Tertipler ?? new List<string>())
                {
                    if (string.IsNullOrWhiteSpace(trtp)) continue;
                    string tut = FisIcerik(kullanan, istek.OdenekTur, istek.Yil, trtp.Trim(), senaryo, !tertipYok);
                    if (!string.IsNullOrEmpty(tut))
                    {
                        sayfa += tut;
                        tertipYok = false;
                    }
                }
                if (tertipYok) return Hata("Lütfen listeden seçim yapınız!");

                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(ms, new UTF8Encoding(false), 4096, true))
                        ButceFisiOrtak.ButceFisiDosyayaYaz(sayfa, sw);
                    return Results.File(ms.ToArray(), "application/msword", "BF.doc");
                }
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Bütçe fişi hazırlanamadı."));
            }
        }
    }
}
