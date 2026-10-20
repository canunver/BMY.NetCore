using System;
using System.Collections.Generic;
using Istemci.ButceHazirlik;
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
    /// Muavin Defteri (eski ButceMuhasebe/MuavinDefteri.aspx; Ödeme Emri / MİF çerçevesinin "Muavin Defteri" sekmesi) uçları.
    /// Sayfa menüde yok: yetki üst sayfadan (Ödeme Emri / MİF çerçevesinin menüdeki üç adresi) ve eski sayfa girişi denetiminden
    /// (TNS.MUH.Yetki.YetkisiVarMi: ödeme emri görme) gelir. Bütün uçlar GET'tir (kayıt yapılmaz).
    ///   GET /api/muavindefteri/tanim         : yıllar, durumlar, belge türleri, fiş türleri, döviz var mı (eski Page_Load)
    ///   GET /api/muavindefteri/liste         : kıstaslara göre satırlar (eski btnListe_Click; hesap koduna göre sıralı, DEVİR satırlı)
    ///   GET /api/muavindefteri/yazdir?tur=1|2: 1 Yazdır (MUH-MuavinDefteri.xlt / MUH-MuavinDefteriDoviz.xlt), 2 Yazdır2 (MUH-MuavinDefteri2.xlt)
    ///   GET /api/muavindefteri/kodlar        : kurumsal / fonksiyonel / finans / ekonomik / faaliyet kod ağacı (eski nodeLoadPar)
    ///   GET /api/muavindefteri/hesap/agac, hesap/ara, hesap/ad : hesap kodu alanı (Sgb.hesapKodu / Sgb.hesapPlaniSec)
    /// Kıstas adları eski PropertyGrid adlarıdır (MuavinDefteriKriter.Alanlar); tarihler yyyy-MM-dd.
    /// </summary>
    public static class MuavinDefteriUclari
    {
        public const string SayfaAdres = "../ButceMuhasebe/MuavinDefteri.aspx";
        public static readonly string[] UstSayfalar =
        {
            "../ButceMuhasebe/OdemeEmriMIFFrame.aspx",
            "../ButceMuhasebe/OdemeEmriMIFFrame.aspx?yeni=1",
            "../ButceMuhasebe/OdemeEmriMIFFrame.aspx?yeni=1&baslikSayfasi=1"
        };
        private const string Uc = "/api/muavindefteri/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapGet(Uc + "kodlar", Kodlar);
            app.MapGet(Uc + "hesap/agac", HesapAgac);
            app.MapGet(Uc + "hesap/ara", HesapAra);
            app.MapGet(Uc + "hesap/ad", HesapAd);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        /// <summary>Üst sayfa (Ödeme Emri / MİF çerçevesi) yetkisi ve eski Page_Load girişi (ödeme emri görme yetkisi).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (!SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdres, UstSayfalar)) return false;
            try
            {
                return TNS.MUH.Yetki.YetkisiVarMi(kullanan, (int)ENUMOGY.Gorme, (int)ENUMBelgeTuru.ODEMEEMRI);
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

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = Temiz(hata);
            return hata == "" ? varsayilan : hata;
        }

        private static int HesapYil(int? yil)
        {
            int y = yil ?? 0;
            return y <= 2000 ? DateTime.Now.Year : y;
        }

        // ------------------------------------------------------------------ açılış (eski Page_Load)

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                List<int> yillar = new List<int>();
                for (int i = 2006; i <= DateTime.Now.Year; i++)
                    yillar.Add(i);

                List<object> durumlar = new List<object>
                {
                    new { kod = 1, ad = "Yeni Belgeler" },
                    new { kod = 5, ad = "Onaylı" },
                    new { kod = 23, ad = "Yevmiyesiz" },
                    new { kod = 6, ad = "Yevmiyeli" },
                    new { kod = 9, ad = "İptal Edilenler" },
                    new { kod = 22, ad = "Bütün Belgeler (İptaller Hariç)" }
                };
                List<object> belgeTurleri = new List<object>
                {
                    new { kod = 0, ad = "Hepsi" },
                    new { kod = 1, ad = "Ödeme Emri" },
                    new { kod = 2, ad = "MİF" }
                };
                List<object> listeTurleri = new List<object>
                {
                    new { kod = 1, ad = "Türk Lirası" },
                    new { kod = 2, ad = "Döviz" }
                };

                // Eski ButceMuhasebeGenel.FisTurDoldur
                List<object> fisTurleri = new List<object>();
                foreach (ENUMFISTUR ft in new[] { ENUMFISTUR.DONEMICI, ENUMFISTUR.ACILIS, ENUMFISTUR.KAPANIS, ENUMFISTUR.AMORTISMANKAPANISI, ENUMFISTUR.SONFIS })
                    fisTurleri.Add(new { kod = (int)ft, ad = TNS.MUH.Arac.FisTurAdBul(ft) });

                return Results.Json(new
                {
                    yillar = yillar,
                    yil = DateTime.Now.Year,
                    durumlar = durumlar,
                    varsayilanDurum = 22,
                    belgeTurleri = belgeTurleri,
                    varsayilanBelgeTur = 0,
                    listeTurleri = listeTurleri,
                    varsayilanListeTur = 1,
                    fisTurleri = fisTurleri,
                    varsayilanFisTur = new[] { 0, 1 },
                    dovizVar = TNS.MUH.Arac.MuhasebeDovizVarMi()
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ liste ve yazdırma

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                MuavinDefteriKriter k = MuavinDefteriKriter.Oku(baglam.Request.Query);
                MuavinDefteriIslem islem = new MuavinDefteriIslem(kullanan, k, ortam.ContentRootPath);
                List<MuavinSatir> satirlar = islem.Listele(out string hata);
                if (satirlar == null) return Hata(HataMetni(hata, "Sorgu yapılamadı."));
                return Results.Json(new
                {
                    satirlar = satirlar,
                    listeTur = k.ListeTur(),
                    dovizVar = k.ListeTurVar,
                    uyarilar = islem.Uyarilar
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sorgu yapılamadı."));
            }
        }

        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, string tur)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                MuavinDefteriKriter k = MuavinDefteriKriter.Oku(baglam.Request.Query);
                MuavinDefteriIslem islem = new MuavinDefteriIslem(kullanan, k, ortam.ContentRootPath);
                AsposeLisans.Yukle();
                string hata;
                string dosya = Temiz(tur) == "2" ? islem.Yazdir2(out hata) : islem.Yazdir(out hata);
                if (dosya == null) return Hata(HataMetni(hata, "Rapor hazırlanamadı."));
                return DosyaIslem.DosyaGonder(dosya, "Liste", true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ kod ağaçları (eski nodeLoadPar: 1, 2, 3, 4, 14)

        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string tur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            tur = Temiz(tur);
            if (tur != "1" && tur != "2" && tur != "3" && tur != "4" && tur != "14") return Hata("Kod türü hatalı.");
            return GorusmeCariUclari.KodDuzeyi(kullanan, HesapYil(yil), tur, ust, false);
        }

        // ------------------------------------------------------------------ hesap kodu (eski StoreHesapPlan / HesapListesiDoldur)
        // Sgb.hesapKodu (js/hesapkodu.js) ve Sgb.hesapPlaniSec (js/makbuzsecim.js) uçları; Muhasebe Tanımları uçlarının (MuhTanimOrtak)
        // aynısı, bu sayfanın yetkisiyle.

        private static object HesapDugum(HesapPlaniSatir h)
        {
            return new { kod = h.hesapKod, ad = h.hesapKod + " - " + h.aciklama, hesapAd = h.aciklama ?? "", altVar = !h.detay, detay = h.detay };
        }

        /// <summary>Hesap planı ağacı (eski ListeHesapPlani.aspx): kökte 1-9 ana gruplar, sonra düzey düzey hesaplar.</summary>
        private static IResult HesapAgac(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                int y = HesapYil(yil);
                ust = Temiz(ust);
                List<object> dugumler = new List<object>();
                if (ust == "")
                {
                    TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                    for (int i = 1; i < 10; i++)
                    {
                        string ad = servisUZY.UzayDegeriStr(new Kullanici(), "MUHHESAPPLANI", y.ToString() + "-" + i.ToString(), true);
                        dugumler.Add(new { kod = i.ToString(), ad = i.ToString() + "-" + ad, hesapAd = ad ?? "", altVar = true, detay = false });
                    }
                    return Results.Json(new { dugumler = dugumler });
                }

                string[] kirilimlar = ust.Split('.');
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = y;
                hs.hesapKod = ust;
                hs.seviye = ust.Length < 3 ? 1 : kirilimlar.Length + 1;

                IMUHServis servis = TNS.MUH.Arac.Tanimla();
                ObjectArray hv = new ObjectArray();
                for (int i = 0; i < 10; i++)
                {
                    hv = servis.HesapPlaniListele(new Kullanici(), hs);
                    if (hv.objeler.Count == 0) hs.seviye++;
                    else break;
                }
                foreach (HesapPlaniSatir h in hv.objeler) dugumler.Add(HesapDugum(h));
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Hesap planı okunamadı.");
            }
        }

        /// <summary>Hesap kodu / adı ile arama (eski StoreHesapPlan: HesapListesiDoldur, hesapKodHesapAd).</summary>
        private static IResult HesapAra(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string metin, string sonKirilim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                metin = Temiz(metin);
                if (metin.Length < 2) return Hata("En az iki karakter yazınız.");
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = HesapYil(yil);
                h.hesapKodHesapAd = metin;
                h.detay = sonKirilim == "1";
                ObjectArray hesap = TNS.MUH.Arac.Tanimla().HesapPlaniListele(kullanan, h);
                if (hesap == null || !hesap.sonuc.islemSonuc) return Hata(HataMetni(hesap?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (HesapPlaniSatir d in hesap.objeler)
                {
                    satirlar.Add(HesapDugum(d));
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }

        /// <summary>Elle yazılan hesap kodunun adı: kod yılın kod yapısına göre noktalanır; hesap planında yoksa ad boş döner.</summary>
        private static IResult HesapAd(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                kod = Temiz(kod);
                if (kod == "") return Results.Json(new { kod = "", ad = "", detay = false });
                int y = HesapYil(yil);
                string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(y, "/KODYAPI/KOD8/KODYAPI");
                if (!string.IsNullOrEmpty(hesYapi)) kod = OrtakFonksiyonlar.KodNoktaKoy(hesYapi, kod.Replace(".", ""));
                ObjectArray hv = TNS.MUH.Arac.Tanimla().HesapPlaniListele(kullanan, new HesapPlaniSatir { yil = y, hesapKod = kod });
                if (hv != null && hv.objeler != null)
                    foreach (HesapPlaniSatir d in hv.objeler)
                        if (d.hesapKod == kod) return Results.Json(new { kod = d.hesapKod, ad = d.aciklama ?? "", detay = d.detay });
                return Results.Json(new { kod = kod, ad = "", detay = false });
            }
            catch (Exception)
            {
                return Hata("Hesap kodu okunamadı.");
            }
        }
    }
}
