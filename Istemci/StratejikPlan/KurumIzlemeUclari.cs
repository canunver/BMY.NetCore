using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using PerformansButce;
using TNS;
using TNS.BKD;
using TNS.KYM;
using TNS.PEB;

namespace Istemci.StratejikPlan
{
    public class HedefAciklamaIstegi
    {
        public string Kurum { get; set; }
        public int Yil { get; set; }
        public int Hedef { get; set; }
        public string Metin { get; set; }
    }

    /// <summary>
    /// Kurum Değerlendirme sayfasının (eski PerformansButce/SPIzlemeKurum.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/strkurum/tanim, /liste?kurum=&amp;yil=&amp;donem=&amp;alt=&amp;ust=, /gostergeler?kurum=&amp;yil=&amp;donem=&amp;hedef=
    ///   POST /api/strkurum/aciklama
    ///   GET  /api/strkurum/rapor-genel, /rapor-ozet?kurum=&amp;yil=&amp;donem=&amp;alt=&amp;ust= (Excel)
    /// Veri eski sayfanın yordamlarıyla hazırlanır (PerformansButce.SPIzlemeKurum: IzlemeGenelOlustur, GostergeGelisimBul,
    /// YazdirHedef, YazdirHedefGost). Sayfayı menüde Kurum Değerlendirme seçeneğini görebilenler kullanır.
    /// </summary>
    public static class KurumIzlemeUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/strkurum/tanim", Tanim);
            app.MapGet("/api/strkurum/liste", Liste);
            app.MapGet("/api/strkurum/gostergeler", Gostergeler);
            app.MapPost("/api/strkurum/aciklama", AciklamaKaydet);
            app.MapGet("/api/strkurum/rapor-genel", RaporGenel);
            app.MapGet("/api/strkurum/rapor-ozet", RaporOzet);
        }

        /// <summary>Kullanıcı Kurum Değerlendirme sayfasını kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../PerformansButce/SPIzlemeKurum.aspx");
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

        // ------------------------------------------------------------------ okuma

        /// <summary>Arama bölmesinin seçenekleri ve varsayılanları (eski Page_Load).</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            int yil = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/BUTCEHAZIRLIK"), DateTime.Now.Year + 1);

            // Kurum: Stratejik Plan sayfasında seçilmiş olan ("kurumKod-yıl"), yoksa listenin ilki
            string kurumKod = "", yilStr = "";
            try
            {
                TNSCollection degerler = TNS.KYM.Arac.Tanimla().DegiskenleriListele(kullanan.kullaniciKodu, "StrPlan");
                if (degerler != null && degerler.Count > 0)
                    OrtakFonksiyonlar.StringiIkiyeBol(Convert.ToString(degerler[0]) ?? "", "-", ref kurumKod, ref yilStr);
            }
            catch (Exception)
            {
                kurumKod = "";
            }

            List<object> kurumlar = new List<object>();
            string ilkKurum = "";
            ABSKodlarVAN liste = TNS.BKD.Arac.Tanimla().KurumListesiKullanici(kullanan, yil);
            if (liste != null && liste.absKodlar != null)
                foreach (ABSKod k in liste.absKodlar)
                {
                    if (ilkKurum == "") ilkKurum = k.kod;
                    kurumlar.Add(new { kod = k.kod, ad = k.kodAd });
                }
            if (string.IsNullOrWhiteSpace(kurumKod)) kurumKod = ilkKurum;

            return Results.Json(new
            {
                kurumlar = kurumlar,
                kurumKod = kurumKod,
                yil = yil,
                donemler = new object[]
                {
                    new { kod = (int)ENUMDonemler.AltiAy, ad = TNS.PEB.Arac.DonemStr(ENUMDonemler.AltiAy, false) },
                    new { kod = (int)ENUMDonemler.Yil, ad = TNS.PEB.Arac.DonemStr(ENUMDonemler.Yil, false) }
                },
                donem = DateTime.Now.Month > 7 ? (int)ENUMDonemler.Yil : (int)ENUMDonemler.AltiAy
            });
        }

        /// <summary>
        /// Kurumun hedefleri ve plan yıllarına göre başarım yüzdeleri (eski Listele). alt / ust: yalnızca başarımı bu aralıkta
        /// olan hedefler (-1: sınır yok). basariSay: seçilen yılda başarı notlarına (1 en iyi ... 4) göre hedef sayıları.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string kurum, int yil, int donem, int? alt, int? ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (string.IsNullOrWhiteSpace(kurum) || yil <= 0) return Results.Json(new { hata = "Kurum ve yıl seçiniz." });

            try
            {
                List<SPIzlemeKurum.IzlemeKurum> liste = SPIzlemeKurum.IzlemeGenelOlustur(kullanan, TNS.PEB.Arac.Tanimla(), yil, donem, kurum.Trim(), alt ?? -1, ust ?? -1, out Misyon ms, out int[,] basariSay);

                List<object> satirlar = new List<object>();
                foreach (SPIzlemeKurum.IzlemeKurum h in liste)
                    satirlar.Add(new
                    {
                        hedefKod = h.hedefKod,
                        hedefKisa = h.hedefKisa,
                        hedefAd = h.hedefAd,
                        gostergeSay = h.gostergeSay,
                        oranlar = new[] { h.hedefOran0, h.hedefOran1, h.hedefOran2, h.hedefOran3, h.hedefOran4 },
                        basariNotu = h.basariNotu,
                        aciklama = h.aciklama ?? ""
                    });

                int yer = Math.Max(0, Math.Min(4, yil - ms.aralikBasiYil));
                return Results.Json(new
                {
                    basYil = ms.aralikBasiYil,
                    // Planın ilk yılından seçilen yıla kadar olan yıllar gösterilir (en çok beş).
                    yilSay = Math.Max(1, Math.Min(5, Math.Min(yil, ms.aralikSonuYil) - ms.aralikBasiYil + 1)),
                    satirlar = satirlar,
                    basariSay = new[] { basariSay[yer, 0], basariSay[yer, 1], basariSay[yer, 2], basariSay[yer, 3] }
                });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Hedefler okunamadı." });
            }
        }

        /// <summary>Bir hedefin göstergelerinin plan yıllarına göre başarımları (eski satır açma: BeforeExpand).</summary>
        private static IResult Gostergeler(HttpContext baglam, IWebHostEnvironment ortam, string kurum, int yil, int donem, int hedef)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (string.IsNullOrWhiteSpace(kurum) || yil <= 0 || hedef <= 0) return Results.BadRequest();

            try
            {
                Misyon ms = SPIzlemeKurum.MisyonBul(kullanan, TNS.PEB.Arac.Tanimla(), kurum.Trim(), yil);
                ArrayList al = SPIzlemeKurum.GostergeGelisimBul(hedef, ms, donem, yil);

                List<object> satirlar = new List<object>();
                foreach (StrHedefGosterge g in al)
                    satirlar.Add(new
                    {
                        gorKod = g.gorKod,
                        ad = g.gostergeAd,
                        baslangic = g.gerceklesen,
                        oranlar = new[] { g.gostOran0, g.gostOran1, g.gostOran2, g.gostOran3, g.gostOran4 },
                        basariNotu = Ortak.SPNotBul(g.GostergeOranBul)
                    });
                return Results.Json(new { basYil = ms.aralikBasiYil, satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Göstergeler okunamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Hedefin açıklaması (eski btnAciklamaKaydet_Click): KYM büyük metin deposunda "SP" / HDFA_yıl_kurum_hedef anahtarıyla saklanır.
        /// Anahtardaki yıl, listede açıklamanın okunduğu yıldır (planın yılı); eski sayfa kaydederken sorgu yılını kullandığı için
        /// plan yılı ile sorgu yılı farklıyken kaydedilen açıklama listede görünmüyordu.
        /// </summary>
        private static async Task<IResult> AciklamaKaydet(HttpContext baglam, IWebHostEnvironment ortam, HedefAciklamaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || string.IsNullOrWhiteSpace(istek.Kurum) || istek.Yil <= 0 || istek.Hedef <= 0) return Results.BadRequest();

            Misyon ms = SPIzlemeKurum.MisyonBul(kullanan, TNS.PEB.Arac.Tanimla(), istek.Kurum.Trim(), istek.Yil);
            string metin = (istek.Metin ?? "").Trim();
            Sonuc s = TNS.KYM.Arac.Tanimla().BuyukMetinSakla(kullanan, "SP", SPIzlemeKurum.BMHedefOl(ms.donemBYil, ms.kurumKod, istek.Hedef), "TR", metin);
            if (!s.islemSonuc)
            {
                string hata = s.hataStr ?? "";
                return Results.Json(new { basarili = false, hata = hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? "Kayıt işlemi başarısız." : "Kayıt işlemi başarısız: " + hata });
            }
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarılı.", aciklama = metin });
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, string kurum, int yil, int donem, int? alt, int? ust, bool ozet)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (string.IsNullOrWhiteSpace(kurum) || yil <= 0) return Results.Json(new { hata = "Kurum ve yıl seçiniz." });

            try
            {
                AsposeLisans.Yukle();
                IPEBServis servisPEB = TNS.PEB.Arac.Tanimla();
                string raporSablonYol = Ayar.Oku("RaporSablonYol");
                string sonucDosya = ozet
                    ? SPIzlemeKurum.YazdirHedefGost(kullanan, servisPEB, yil, kurum.Trim(), donem, alt ?? -1, ust ?? -1, raporSablonYol)
                    : SPIzlemeKurum.YazdirHedef(kullanan, servisPEB, yil, kurum.Trim(), donem, alt ?? -1, ust ?? -1, raporSablonYol);
                return DosyaIslem.DosyaGonder(sonucDosya, Path.GetFileName(sonucDosya), true, "xlsx");
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }

        /// <summary>Yazdır (Genel): hedef başarımları (şablon SPKurumIzleme.xltx).</summary>
        private static IResult RaporGenel(HttpContext baglam, IWebHostEnvironment ortam, string kurum, int yil, int donem, int? alt, int? ust)
        {
            return Rapor(baglam, ortam, kurum, yil, donem, alt, ust, false);
        }

        /// <summary>Yazdır (Özet): hedefler ve altlarında göstergelerinin başarımları (şablon SPKurumIzlemeGost.xltx).</summary>
        private static IResult RaporOzet(HttpContext baglam, IWebHostEnvironment ortam, string kurum, int yil, int donem, int? alt, int? ust)
        {
            return Rapor(baglam, ortam, kurum, yil, donem, alt, ust, true);
        }
    }
}
