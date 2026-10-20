using System;
using System.Collections.Generic;
using System.IO;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Avans ekranlarının uçları:
    ///   Avans Kapatma (Avans Listesi), eski ButceMuhasebe/AvansListeYeni.aspx:
    ///     GET /api/avans/tanim                                       açılış: yıl, yıllar, kullanıcının muhasebe birimi
    ///     GET /api/avans/liste?yil&amp;muhasebe&amp;birim&amp;kimlikNo&amp;ad    eski btnOku_Click (servis AvansListele)
    ///     GET /api/avans/liste/yazdir?...                            eski btnYazdir_Click (MUH/AvansListesi.xlt)
    ///   Avans Sorgulama, eski ButceMuhasebe/AvansSorgulaForm.aspx:
    ///     GET /api/avans/sorgu?yil&amp;durum&amp;muhasebe&amp;birim&amp;hesapKod&amp;tcVno&amp;tertipli&amp;altHesaptan&amp;farklilar   eski Oku (AvansSorgula)
    ///     GET /api/avans/sorgu/detay?...&amp;hesapNo&amp;tcVno&amp;tertip          eski BeforeExpand (AvansSorgula, detay=true)
    /// Her uç ilgili sayfanın menü yetkisiyle denetlenir.
    /// </summary>
    public static class AvansUclari
    {
        public const string ListeSayfa = "../ButceMuhasebe/AvansListeYeni.aspx";
        public const string SorguSayfa = "../ButceMuhasebe/AvansSorgulaForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/avans/tanim", Tanim);
            app.MapGet("/api/avans/liste", Liste);
            app.MapGet("/api/avans/liste/yazdir", ListeYazdir);
            app.MapGet("/api/avans/sorgu", Sorgu);
            app.MapGet("/api/avans/sorgu/detay", SorguDetay);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, string sayfa)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), sayfa);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, string sayfa, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, sayfa)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        /// <summary>Tanım ucu iki sayfanın da açılışında kullanılır: sayfalardan birine yetki yeter.</summary>
        private static IResult DenetleHerhangi(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, ListeSayfa) && !YetkiVar(kullanan, ortam.ContentRootPath, SorguSayfa))
                return Results.StatusCode(StatusCodes.Status403Forbidden);
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

        private static string Q(HttpContext baglam, string ad)
        {
            return ((string)baglam.Request.Query[ad] ?? "").Trim();
        }

        private static bool Evet(HttpContext baglam, string ad)
        {
            string d = Q(baglam, ad);
            return d == "1" || d.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        private static string Tarih(DateTime t)
        {
            return t <= DateTime.MinValue || t.Year < 1900 ? "" : t.ToString("yyyy-MM-dd");
        }

        // ------------------------------------------------------------------ açılış

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = DenetleHerhangi(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<int> yillar = new List<int>();
            for (int i = 2006; i <= DateTime.Now.Year + 1; i++) yillar.Add(i);
            return Results.Json(new
            {
                yil = DateTime.Now.Year,
                yillar = yillar
            });
        }

        // ------------------------------------------------------------------ Avans Listesi (AvansListeYeni)

        private static AvansBilgi ListeKriter(HttpContext baglam)
        {
            AvansBilgi av = new AvansBilgi();
            av.yil = OrtakFonksiyonlar.ConvertToInt(Q(baglam, "yil"), 0);
            av.muhasebe = OrtakFonksiyonlar.ConvertToInt(Q(baglam, "muhasebe"), 0);
            av.birim = Q(baglam, "birim");
            av.kimlikNo = Q(baglam, "kimlikNo");
            av.adSoyad = Q(baglam, "ad");
            return av;
        }

        /// <summary>Eski GenelIslemler.KodAd(12, birim + "-" + muhasebe): muhasebe biriminin adı.</summary>
        private static string BirimAd(int muhasebe, string birim, Dictionary<string, string> onbellek)
        {
            string anahtar = muhasebe + "-" + birim;
            if (onbellek.TryGetValue(anahtar, out string ad)) return ad;
            try
            {
                ad = MuhasebeBirimUclari.BirimAdBul(muhasebe, birim, out _) ?? "";
            }
            catch (Exception)
            {
                ad = "";
            }
            onbellek[anahtar] = ad;
            return ad;
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, ListeSayfa, out Kullanici kullanan);
            if (ret != null) return ret;
            AvansBilgi av = ListeKriter(baglam);
            if (av.yil <= 0) return Hata("Anahtar alanları girmelisiniz.");
            try
            {
                ObjectArray yler = TNS.MUH.Arac.Tanimla().AvansListele(kullanan, av);
                List<object> satirlar = new List<object>();
                if (yler != null && yler.objeler != null)
                {
                    Dictionary<string, string> adlar = new Dictionary<string, string>();
                    int sira = 1;
                    foreach (AvansBilgi y in yler.objeler)
                    {
                        satirlar.Add(new
                        {
                            sira = sira++,
                            muhasebe = y.muhasebe + " - " + y.muhasebeAd,
                            birim = y.birim + " - " + BirimAd(y.muhasebe, y.birim, adlar),
                            tCVno = y.kimlikNo ?? "",
                            adSoyadUnvan = y.adSoyad ?? "",
                            verilen = y.borc,
                            alinan = y.alacak,
                            kalan = y.borc - y.alacak
                        });
                    }
                }
                if (satirlar.Count == 0) return Results.Json(new { satirlar = satirlar, bilgi = "Verilen kriterlere uygun kayıt bulunamadı." });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste okunamadı."));
            }
        }

        private static IResult ListeYazdir(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, ListeSayfa, out Kullanici kullanan);
            if (ret != null) return ret;
            AvansBilgi av = ListeKriter(baglam);
            if (av.yil <= 0) return Hata("Anahtar alanları girmelisiniz.");
            try
            {
                ObjectArray yler = TNS.MUH.Arac.Tanimla().AvansListele(kullanan, av);
                if (yler == null || yler.objeler == null || yler.objeler.Count == 0) return Hata("Verilen kriterlere uygun kayıt bulunamadı.");

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sablonAd = "AvansListesi.xlt";
                string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                XLS.DosyaAc(SablonYol(sablonAd), sonucDosyaAd);

                int satir = 0;
                int sutun = 0;
                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int yazSatir = satir;

                Dictionary<string, string> adlar = new Dictionary<string, string>();
                foreach (AvansBilgi y in yler.objeler)
                {
                    XLS.HucreKopyala(satir, sutun, satir, sutun + 6, yazSatir, sutun);
                    XLS.HucreDegerYaz(yazSatir, 0, y.muhasebe + " - " + y.muhasebeAd);
                    XLS.HucreDegerYaz(yazSatir, 1, y.birim + " - " + BirimAd(y.muhasebe, y.birim, adlar));
                    XLS.HucreDegerYaz(yazSatir, 2, y.kimlikNo);
                    XLS.HucreDegerYaz(yazSatir, 3, y.adSoyad);
                    XLS.HucreDegerYaz(yazSatir, 4, y.borc);
                    XLS.HucreDegerYaz(yazSatir, 5, y.alacak);
                    XLS.HucreDegerYaz(yazSatir, 6, (y.borc - y.alacak));
                    yazSatir++;
                }

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "AvansListesi_" + DateTime.Now.Ticks, true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        /// <summary>Şablon: önce kurumun RaporSablonYol/MUH klasörü, yoksa uygulamanın RaporSablon/MUH klasörü.</summary>
        private static string SablonYol(string sablonAd)
        {
            string kurum = Path.Combine(Ayar.Oku("RaporSablonYol") ?? "", "MUH", sablonAd);
            if (File.Exists(kurum)) return kurum;
            string uygulama = Path.Combine(AppContext.BaseDirectory, "RaporSablon", "MUH", sablonAd);
            if (File.Exists(uygulama)) return uygulama;
            return kurum;
        }

        // ------------------------------------------------------------------ Avans Sorgulama (AvansSorgulaForm)

        private static AvansSorguKriter SorguKriter(HttpContext baglam)
        {
            AvansSorguKriter form = new AvansSorguKriter();
            form.yil = OrtakFonksiyonlar.ConvertToInt(Q(baglam, "yil"), 0);
            form.durum = OrtakFonksiyonlar.ConvertToInt(Q(baglam, "durum"), 0);
            form.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            form.saymanlikKodu = OrtakFonksiyonlar.ConvertToInt(Q(baglam, "muhasebe"), 1);
            form.birim = Q(baglam, "birim");
            form.hesapKod = Q(baglam, "hesapKod");
            form.tcVno = Q(baglam, "tcVno");
            form.tertipli = Evet(baglam, "tertipli");
            form.altHesaptan = Evet(baglam, "altHesaptan");
            form.farklilar = Evet(baglam, "farklilar");
            return form;
        }

        private static object HareketSatir(AvansHareket h)
        {
            return new
            {
                sira = h.sira ?? "",
                tCVno = h.tCVno ?? "",
                adSoyadUnvan = h.adSoyadUnvan ?? "",
                hesapNo = h.hesapNo ?? "",
                hesapAdi = h.hesapAdi ?? "",
                tertip = h.tertip ?? "",
                sonTarih = Tarih(h.sonTarihDT),
                borc = h.borc,
                alacak = h.alacak
            };
        }

        private static IResult Sorgu(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, SorguSayfa, out Kullanici kullanan);
            if (ret != null) return ret;
            AvansSorguKriter form = SorguKriter(baglam);
            if (form.yil <= 0 || form.durum <= 0) return Hata("Anahtar alanları girmelisiniz.");
            try
            {
                AvansHareket[] hareketler = TNS.MUH.Arac.Tanimla().AvansSorgula(kullanan, form);
                List<object> satirlar = new List<object>();
                if (hareketler != null)
                    foreach (AvansHareket h in hareketler)
                        if (h != null) satirlar.Add(HareketSatir(h));
                if (satirlar.Count == 0) return Results.Json(new { satirlar = satirlar, bilgi = "Verilen kriterlere uygun kayıt bulunamadı." });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sorgu yapılamadı."));
            }
        }

        /// <summary>Satır ayrıntısı (eski BeforeExpand): aynı kıstaslarla, satırın hesap / kişi / tertibi için belge hareketleri.</summary>
        private static IResult SorguDetay(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, SorguSayfa, out Kullanici kullanan);
            if (ret != null) return ret;
            AvansSorguKriter form = SorguKriter(baglam);
            form.hesapKod = Q(baglam, "hesapNo");
            form.tcVno = Q(baglam, "tcVno");
            form.tertip = Q(baglam, "tertip");
            form.detay = true;
            if (form.yil <= 0 || form.durum <= 0) return Hata("Anahtar alanları girmelisiniz.");
            try
            {
                AvansHareket[] hareketler = TNS.MUH.Arac.Tanimla().AvansSorgula(kullanan, form);
                List<object> satirlar = new List<object>();
                if (hareketler != null)
                    foreach (AvansHareket h in hareketler)
                        if (h != null) satirlar.Add(HareketSatir(h));
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Hareketler okunamadı."));
            }
        }
    }
}
