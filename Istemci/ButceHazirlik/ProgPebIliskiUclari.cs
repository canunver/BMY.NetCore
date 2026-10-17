using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    public class PrgPebIliskiIstegi
    {
        public int Yil { get; set; }
        public string PebKod { get; set; }
        public int PrgKod { get; set; }
        public int AltPrgKod { get; set; }
        public int FaalKod { get; set; }
        public int AltFaalKod { get; set; }
        public string FonKod { get; set; }
    }

    /// <summary>
    /// Alt Faaliyet Bağları sayfasının (eski ButceHazirlikv2/ProgPebIliski.aspx: performans bütçe alt faaliyet kodu - program bütçe
    /// kodu ilişkisi) uçları:
    ///   GET  /api/progpebiliski/liste?yil=&amp;ara=&amp;fon=   ilişkiler (fon=true: eski Listele (Fon), fonksiyonel kodu uyumsuz olanlar)
    ///   GET  /api/progpebiliski/rapor?yil=&amp;ara=        Yazdır: BKD\ProgramPEBIliski.xltx
    ///   GET  /api/progpebiliski/prgkod?tur=&amp;yil=&amp;ara=  program (1) / alt program (2) / faaliyet (3) / alt faaliyet (4) arama
    ///   GET  /api/progpebiliski/kodagac?tur=&amp;yil=&amp;ust=  alt faaliyet (14) ve fonksiyonel (2) kod ağacı (eski KodAgac.aspx)
    ///   GET  /api/progpebiliski/kodad?tur=&amp;yil=&amp;kod=   yazılan kodun adı (eski KodAdGetir)
    ///   POST /api/progpebiliski/kaydet | sil | kopyala (önceki yıldan)
    /// </summary>
    public static class ProgPebIliskiUclari
    {
        public const string SayfaAdresi = ButceTanimOrtak.Klasor + "ProgPebIliski.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/progpebiliski/liste", Liste);
            app.MapGet("/api/progpebiliski/rapor", Rapor);
            app.MapGet("/api/progpebiliski/prgkod", PrgKodAra);
            app.MapGet("/api/progpebiliski/kodagac", KodAgac);
            app.MapGet("/api/progpebiliski/kodad", KodAd);
            app.MapPost("/api/progpebiliski/kaydet", Kaydet);
            app.MapPost("/api/progpebiliski/sil", Sil);
            app.MapPost("/api/progpebiliski/kopyala", Kopyala);
        }

        private static ObjectArray Oku(Kullanici kullanan, int yil, string ara, bool fonUyumsuz)
        {
            PrgPebIliski kriter = new PrgPebIliski();
            kriter.yil = yil;
            kriter.araMetin = ara ?? "";
            kriter.fonUyumsuz = fonUyumsuz;
            return TNS.BKD.Arac.Tanimla().PrgPebIliskiListele(kullanan, kriter);
        }

        private static int Yil(int? yil)
        {
            return yil.HasValue && yil.Value > 1000 ? yil.Value : DateTime.Now.Year;
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ara, bool? fon)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            try
            {
                ObjectArray oa = Oku(kullanan, Yil(yil), ButceTanimOrtak.Temiz(ara), fon ?? false);
                if (oa == null || !oa.sonuc.islemSonuc)
                    return ButceTanimOrtak.Hata(YatProjeOrtak.HataMetni(oa == null ? null : oa.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (PrgPebIliski p in oa.objeler)
                    satirlar.Add(new
                    {
                        yil = p.yil,
                        pebKod = p.pebKod ?? "", pebAd = p.pebAd ?? "",
                        altFaalKod = p.altFaalKod, altFaalAdi = p.altFaalAdi ?? "",
                        prgKod = p.prgKod, prgAdi = p.prgAdi ?? "",
                        altPrgKod = p.altPrgKod, altPrgAdi = p.altPrgAdi ?? "",
                        faalKod = p.faalKod, faalAdi = p.faalAdi ?? "",
                        faalFonKod = p.faalFonKod ?? "", fonKod = p.fonKod ?? ""
                    });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Liste okunamadı.");
            }
        }

        /// <summary>Program bütçe kodu arama (eski strProgram..strAltFaaliyet_Refresh): koda ya da ada göre, ilk 50 kayıt.</summary>
        private static IResult PrgKodAra(HttpContext baglam, IWebHostEnvironment ortam, int tur, int? yil, string ara)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            if (tur < 1 || tur > 4) return Results.BadRequest();
            try
            {
                PrgKodYapi kriter = new PrgKodYapi();
                kriter.yil = Yil(yil);
                kriter.kodTur = tur;
                kriter.sorguKriter = ButceTanimOrtak.Temiz(ara);
                ObjectArray oa = TNS.BKD.Arac.Tanimla().PrgKodYapiListele(kullanan, kriter);
                if (oa == null || !oa.sonuc.islemSonuc) return ButceTanimOrtak.Hata("Kodlar okunamadı.");
                List<object> kodlar = new List<object>();
                foreach (PrgKodYapi p in oa.objeler)
                {
                    kodlar.Add(new { kod = p.kod, ad = p.ad ?? "" });
                    if (kodlar.Count >= 50) break;
                }
                return Results.Json(new { kodlar = kodlar });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Kodlar okunamadı.");
            }
        }

        private static string Kultur(Kullanici kullanan)
        {
            return string.IsNullOrEmpty(kullanan.aktifKultur) ? "tr-TR" : kullanan.aktifKultur;
        }

        private static int Seviye(string kod)
        {
            return string.IsNullOrEmpty(kod) ? 0 : kod.Split('.').Length;
        }

        /// <summary>Kod ağacı: ust kodunun bir alt düzeyi; yalnızca alt faaliyet (14) ve fonksiyonel (2) sınıflandırma.</summary>
        private static IResult KodAgac(HttpContext baglam, IWebHostEnvironment ortam, int tur, int? yil, string ust)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            if (tur != 14 && tur != 2) return Results.BadRequest();
            try
            {
                int y = Yil(yil);
                ust = ButceTanimOrtak.Temiz(ust);
                int seviye = Seviye(ust) + 1;
                string kultur = Kultur(kullanan);
                IBKDServis servis = TNS.BKD.Arac.Tanimla();
                ButceKodYapi bky = servis.ButceKodBilgiBul(y, tur, string.Empty);
                int kirilim = bky == null ? 0 : bky.bolumSay;

                ABSKodlarVAN kodlar = servis.KodListeleSeviye(y, tur.ToString(), ust, seviye, true, false, "", "", kultur);
                if (!kodlar.sonuc.islemSonuc) return ButceTanimOrtak.Hata("Kodlar okunamadı.");

                HashSet<string> altiOlanlar = new HashSet<string>();
                if (seviye < kirilim && kodlar.absKodlar.Count > 0)
                {
                    ABSKodlarVAN altlar = servis.KodListeleSeviye(y, tur.ToString(), ust, seviye + 1, true, false, "", "", kultur);
                    if (altlar.sonuc.islemSonuc)
                        foreach (ABSKod alt in altlar.absKodlar)
                        {
                            int nokta = alt.kod.LastIndexOf('.');
                            if (nokta > 0) altiOlanlar.Add(alt.kod.Substring(0, nokta));
                        }
                }

                List<object> dugumler = new List<object>();
                foreach (ABSKod k in kodlar.absKodlar)
                    dugumler.Add(new { kod = k.kod, ad = k.kod + " - " + k.AdKultur(kultur), adYalin = k.AdKultur(kultur), altVar = altiOlanlar.Contains(k.kod) });
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Kodlar okunamadı.");
            }
        }

        private static string AdBul(int tur, int yil, string kod, string kultur)
        {
            ABSKodlarVAN kodlar = TNS.BKD.Arac.Tanimla().KodListeleSeviye(yil, tur.ToString(), kod, Seviye(kod), true);
            if (kodlar == null || !kodlar.sonuc.islemSonuc) return "";
            foreach (ABSKod k in kodlar.absKodlar)
                if (k.kod == kod) return k.AdKultur(kultur) ?? "";
            return "";
        }

        private static IResult KodAd(HttpContext baglam, IWebHostEnvironment ortam, int tur, int? yil, string kod)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            if (tur != 14 && tur != 2) return Results.BadRequest();
            kod = ButceTanimOrtak.Temiz(kod);
            if (kod == "") return Results.Json(new { ad = "" });
            try
            {
                return Results.Json(new { ad = AdBul(tur, Yil(yil), kod, Kultur(kullanan)) });
            }
            catch (Exception)
            {
                return Results.Json(new { ad = "" });
            }
        }

        /// <summary>Kaydet (eski btnKaydet_Click): ilişki yazılır, program bütçe kod çevirme önbelleği sıfırlanır.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PrgPebIliskiIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAdresi);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return ButceTanimOrtak.Basarisiz("Yıl hatalı!");
            if (string.IsNullOrWhiteSpace(istek.PebKod)) return ButceTanimOrtak.Basarisiz("Alt faaliyet kodunu yazınız.");
            try
            {
                PrgPebIliski bilgi = new PrgPebIliski();
                bilgi.yil = istek.Yil;
                bilgi.pebKod = ButceTanimOrtak.Temiz(istek.PebKod);
                bilgi.prgKod = istek.PrgKod;
                bilgi.altPrgKod = istek.AltPrgKod;
                bilgi.faalKod = istek.FaalKod;
                bilgi.altFaalKod = istek.AltFaalKod;
                bilgi.fonKod = ButceTanimOrtak.Temiz(istek.FonKod);
                Sonuc sonuc = TNS.BKD.Arac.Tanimla().PrgPebIliskiKaydet(kullanan, bilgi);
                TNS.BHMv2.Arac.ProgramButceKodaCevirSifirla(bilgi.yil);
                return ButceTanimOrtak.SonucYaz(sonuc, "Kayıt işlemi başarıyla tamamlandı.", "Kayıt işlemi başarısız.");
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Kayıt işlemi başarısız.");
            }
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, PrgPebIliskiIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAdresi);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000 || string.IsNullOrWhiteSpace(istek.PebKod)) return Results.BadRequest();
            try
            {
                PrgPebIliski bilgi = new PrgPebIliski();
                bilgi.yil = istek.Yil;
                bilgi.pebKod = ButceTanimOrtak.Temiz(istek.PebKod);
                Sonuc sonuc = TNS.BKD.Arac.Tanimla().PrgPebIliskiSil(kullanan, bilgi);
                return ButceTanimOrtak.SonucYaz(sonuc, "Silme işlemi başarıyla tamamlandı.", "Silme işlemi başarısız.");
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Silme işlemi başarısız.");
            }
        }

        /// <summary>Önc. Yıldan Kopyala (eski btnKopyala_Click): bir önceki yılın ilişkileri seçilen yıla kopyalanır.</summary>
        private static async Task<IResult> Kopyala(HttpContext baglam, IWebHostEnvironment ortam, PrgPebIliskiIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAdresi);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return ButceTanimOrtak.Basarisiz("Yıl hatalı!");
            try
            {
                Sonuc sonuc = TNS.BKD.Arac.Tanimla().PrgPebIliskiKopyala(kullanan, istek.Yil);
                return ButceTanimOrtak.SonucYaz(sonuc, "İşlem başarılı, lütfen listeleme yapınız.", "Kopyalanamadı.");
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Kopyalanamadı.");
            }
        }

        /// <summary>Yazdır (eski btnYazdir_Click): şablon BKD\ProgramPEBIliski.xltx.</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ara)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            try
            {
                int y = Yil(yil);
                ObjectArray oa = Oku(kullanan, y, ButceTanimOrtak.Temiz(ara), false);
                if (oa == null || !oa.sonuc.islemSonuc)
                    return ButceTanimOrtak.Hata(YatProjeOrtak.HataMetni(oa == null ? null : oa.sonuc.hataStr, "Rapor hazırlanamadı."));

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
                XLS.DosyaAc(Ayar.Oku("RaporSablonYol") + "/BKD/ProgramPEBIliski.xltx", sonucDosyaAd);
                XLS.HucreAdBulYaz("Yil", y);

                int baslaSatir = 0, baslaSutun = 0;
                int sheetNo = 0, formatSatirBas = 0, formatSutunBas = 0, formatSatirBit = 0, formatSutunBit = 0;
                XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
                XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref formatSatirBas, ref formatSutunBas, ref formatSatirBit, ref formatSutunBit);
                int satir = baslaSatir;
                foreach (PrgPebIliski item in oa.objeler)
                {
                    int bs = baslaSutun;
                    XLS.HucreKopyala(formatSatirBas, formatSutunBas, formatSatirBit, formatSutunBit, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, bs++, item.pebKod);
                    XLS.HucreDegerYaz(satir, bs++, item.pebAd);
                    XLS.HucreDegerYaz(satir, bs++, item.altFaalKod);
                    XLS.HucreDegerYaz(satir, bs++, item.altFaalAdi);
                    XLS.HucreDegerYaz(satir, bs++, item.prgKod);
                    XLS.HucreDegerYaz(satir, bs++, item.prgAdi);
                    XLS.HucreDegerYaz(satir, bs++, item.altPrgKod);
                    XLS.HucreDegerYaz(satir, bs++, item.altPrgAdi);
                    XLS.HucreDegerYaz(satir, bs++, item.faalKod);
                    XLS.HucreDegerYaz(satir, bs++, item.faalAdi);
                    XLS.HucreDegerYaz(satir, bs++, item.fonKod);
                    satir++;
                }
                XLS.SatirYukseklikAyarla(baslaSatir, satir, -1, 7, 19);
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "ProgramPEBIliski" + DosyaIslem.DosyaAdUretSade(), true, "xlsx");
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
