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
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    public class KarsilamaOranIstegi
    {
        public int Yil { get; set; }
        public string Tertip { get; set; }
        /// <summary>İl için oranlanırken il plaka kodu, birim için -1.</summary>
        public int Il { get; set; }
        /// <summary>Birim için oranlanırken saymanlık kodu, il için -1.</summary>
        public int Saymanlik { get; set; }
        public string Birim { get; set; }
        public double Karsilanan { get; set; }
        /// <summary>1: harcamaya göre, 2: talebe göre dağıt.</summary>
        public int Yontem { get; set; }
    }

    public class KarsilamaDetaySatiri
    {
        public string TertipTam { get; set; }
        public double Karsilanan { get; set; }
    }

    public class KarsilamaKayitIstegi
    {
        public int Yil { get; set; }
        public int Saymanlik { get; set; }
        public string Birim { get; set; }
        public List<KarsilamaDetaySatiri> Satirlar { get; set; }
    }

    /// <summary>
    /// Proje Teklifi Talep Karşılama sayfasının (eski ButceHazirlikv2/BirimButceKarsilamaForm.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/birimkarsilama/tanim, /kodlar?yil=&amp;tur=&amp;ust=, /tertipler?yil=&amp;kur=&amp;fon=&amp;fin=&amp;eko=&amp;peb=
    ///   GET  /api/birimkarsilama/iller?yil=&amp;tertip=&amp;bosGizle=, /birimler?...&amp;il=, /detay?...&amp;saymanlik=&amp;birim=
    ///   POST /api/birimkarsilama/oranla, /kaydet
    /// Taşra ödenek tertibi (ENUMTabloKod.TABTasraOdenek) için illerin ve birimlerin iki yıl önceki harcaması, önceki yılın masraf
    /// planı, yılın talebi (aşama 00.00) ve karşılananı (aşama 01.00). Liste türleri eski sayfadaki gibi ListeleBirimButceKarsilama'nın
    /// tur parametresidir: 1 iller, 2 bir ilin birimleri, 3 bir birimin tertipleri (eko 2 toplamları + eko 4 detayları), 4 tertip listesi.
    /// Tertip görme / yazma yetkisi (OGY) iş kurallarında denetlenir.
    /// </summary>
    public static class BirimButceKarsilamaUclari
    {
        private const string SayfaAdresi = "../ButceHazirlikv2/BirimButceKarsilamaForm.aspx";
        private const string KarsilananSenaryo = "01.00";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/birimkarsilama/tanim", Tanim);
            app.MapGet("/api/birimkarsilama/kodlar", Kodlar);
            app.MapGet("/api/birimkarsilama/tertipler", Tertipler);
            app.MapGet("/api/birimkarsilama/iller", Iller);
            app.MapGet("/api/birimkarsilama/birimler", Birimler);
            app.MapGet("/api/birimkarsilama/detay", Detay);
            app.MapPost("/api/birimkarsilama/oranla", Oranla);
            app.MapPost("/api/birimkarsilama/kaydet", Kaydet);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdresi);
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

        /// <summary>Seçili tertip (eski KistasTertipOlDdl): tertip listesinden seçilen eko 2 düzeyindeki tertip.</summary>
        private static Tertip SeciliTertip(int yil, string tertip)
        {
            if (string.IsNullOrWhiteSpace(tertip))
                throw new ArgumentException("Lütfen tertip listesinden tertip seçiniz, tertip listesi boş ise yanındaki yenile tuşuna basınız.");
            Tertip t = new Tertip(yil, ENUMTabloKod.TABTasraOdenek);
            t.TertipAta(tertip.Trim());
            return t;
        }

        private static string Kod(string deger)
        {
            return TNS.BHMv2.Arac.TiredenKodBul((deger ?? "").Trim());
        }

        private static IResult Listele(Kullanici kullanan, Tertip tertip, int il, int saymanlik, string birim, bool bosGizle, int tur, Func<BirimButceHazirlikKarsilanan, object> satir)
        {
            ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleBirimButceKarsilama(kullanan, tertip, il, saymanlik, birim ?? "", bosGizle, tur);
            if (oa == null) return Hata("Liste okunamadı.");
            if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "Liste okunamadı."));
            List<object> satirlar = new List<object>();
            for (int i = 0; i < oa.ObjeSayisi; i++)
                satirlar.Add(satir((BirimButceHazirlikKarsilanan)oa.objeler[i]));
            return Results.Json(new { satirlar = satirlar });
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Açılış: yıl (eski Page_Load'daki gibi içinde bulunulan yıl).</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out _);
            if (ret != null) return ret;
            return Results.Json(new { yil = DateTime.Now.Year });
        }

        /// <summary>Kod ağacının bir düzeyi (eski nodeLoadPar3, bütçe hazırlık yetki koduyla); Gider Teklifi sayfasıyla ortak.</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, tur, ust);
        }

        /// <summary>Tertip listesi (eski btnTertipYukle_Click, tur 4): seçilen kodlara uyan eko 2 düzeyindeki tertipler.</summary>
        private static IResult Tertipler(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string fon, string fin, string eko, string peb)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                Tertip t = new Tertip(yil, ENUMTabloKod.TABTasraOdenek, Kod(kur), Kod(fon), Kod(fin), Kod(eko), Kod(peb), "");
                return Listele(kullanan, t, -1, 0, "", true, 4, k => new { tertip = k.tertipEko2, ad = k.GenelAciklamaEko2 });
            }
            catch (Exception)
            {
                return Hata("Tertipler okunamadı.");
            }
        }

        /// <summary>İllerin talepleri ve karşılananları (eski Oku, tur 1).</summary>
        private static IResult Iller(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tertip, bool? bosGizle)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                Tertip t = SeciliTertip(yil, tertip);
                return Listele(kullanan, t, -1, 0, "", bosGizle != false, 1, k => new
                {
                    il = k.il, ad = k.ad, harcama = k.harcama, planlanan = k.planlanan, talepEdilen = k.talepEdilen, karsilanan = k.karsilanan
                });
            }
            catch (ArgumentException e)
            {
                return Hata(e.Message);
            }
            catch (Exception)
            {
                return Hata("Liste okunamadı.");
            }
        }

        /// <summary>Bir ilin saymanlık / birimleri (eski satır açma, BeforeExpand, tur 2).</summary>
        private static IResult Birimler(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tertip, int il, bool? bosGizle)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                Tertip t = SeciliTertip(yil, tertip);
                return Listele(kullanan, t, il, 0, "", bosGizle != false, 2, k => new
                {
                    il = k.il, saymanlik = k.saymanlik, birim = k.birim, ad = k.ad,
                    harcama = k.harcama, planlanan = k.planlanan, talepEdilen = k.talepEdilen, karsilanan = k.karsilanan
                });
            }
            catch (ArgumentException e)
            {
                return Hata(e.Message);
            }
            catch (Exception)
            {
                return Hata("Birimler okunamadı.");
            }
        }

        /// <summary>
        /// Bir birimin tertipleri (eski DetayYukle, tur 3): eko 2 toplam satırları (tertipTam boş) ve altlarında girilebilir eko 4 satırları.
        /// </summary>
        private static IResult Detay(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tertip, int saymanlik, string birim, bool? bosGizle)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                Tertip t = SeciliTertip(yil, tertip);
                return Listele(kullanan, t, -1, saymanlik, birim, bosGizle != false, 3, k => new
                {
                    tertip = k.tertip, tertipTam = k.tertipTam ?? "", tertipEko2 = k.tertipEko2, ekoad = k.ekoad,
                    harcama = k.harcama, planlanan = k.planlanan, talepEdilen = k.talepEdilen, karsilanan = k.karsilanan
                });
            }
            catch (ArgumentException e)
            {
                return Hata(e.Message);
            }
            catch (Exception)
            {
                return Hata("Tertipler okunamadı.");
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Karşılananı alt birimlere ve tertiplere dağıtır (eski DetayOranla): il satırında ilin bütün birimlerine, birim satırında
        /// o birimin tertiplerine; harcamaya ya da talebe göre.
        /// </summary>
        private static async Task<IResult> Oranla(HttpContext baglam, IWebHostEnvironment ortam, KarsilamaOranIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });

            try
            {
                Tertip t = SeciliTertip(istek.Yil, istek.Tertip);
                int yontem = istek.Yontem == 2 ? 2 : 1;
                Sonuc s = TNS.BHMv2.Arac.Tanimla().BirimButceDetayOranla(kullanan, t, istek.Il, istek.Saymanlik, istek.Birim ?? "", istek.Karsilanan, yontem);
                if (s == null || !s.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(s?.hataStr, "Oranlama işlemi yapılamadı.") });
                return Results.Json(new { basarili = true, bilgi = "Oranlama işlemi başarıyla tamamlandı." });
            }
            catch (ArgumentException e)
            {
                return Results.Json(new { basarili = false, hata = e.Message });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Oranlama işlemi yapılamadı." });
            }
        }

        /// <summary>Birimin tertip karşılananlarını kaydeder (eski btnKaydet_Click; aşama 01.00). Yalnızca değişen eko 4 satırları gelir.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, KarsilamaKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });

            try
            {
                List<TertipOdenek> liste = new List<TertipOdenek>();
                foreach (KarsilamaDetaySatiri s in istek.Satirlar ?? new List<KarsilamaDetaySatiri>())
                {
                    if (s == null || string.IsNullOrWhiteSpace(s.TertipTam)) continue;
                    TertipOdenek odenek = new TertipOdenek(1);
                    odenek.tertip = new Tertip(istek.Yil, ENUMTabloKod.TABTasraOdenek);
                    odenek.tertip.TertipAta(s.TertipTam.Trim());
                    odenek.odenek[0] = s.Karsilanan;
                    liste.Add(odenek);
                }
                if (liste.Count == 0) return Results.Json(new { basarili = false, hata = "Kaydedilecek değişiklik yok." });

                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().SaklaBirimButceHazirlik(kullanan, istek.Yil, istek.Saymanlik, istek.Birim ?? "", KarsilananSenaryo, liste.ToArray());
                if (sonuc == null || !sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarılı." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }
    }
}
