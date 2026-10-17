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
using SBB = MUHServisC.tr.gov.sbb.programbutce.listeler;

namespace Istemci.ButceHazirlik
{
    public class PrgKodIstegi
    {
        /// <summary>1: Program, 2: Alt Program, 3: Faaliyet, 4: Alt Faaliyet.</summary>
        public int Tur { get; set; }
        public int Yil { get; set; }
        public int Kod { get; set; }
        public string Ad { get; set; }
        /// <summary>Programın türü (yalnızca Program): 1 Program, 2 Yönetim Hizmet Gideri, 3 Transfer.</summary>
        public int ProgramTur { get; set; }
        /// <summary>Fonksiyonel kod (yalnızca Faaliyet).</summary>
        public string FonKod { get; set; }
    }

    public class PrgKodFarkSatiri
    {
        public long KodPB { get; set; }
        public string AdPB { get; set; }
    }

    public class PrgKodFarkIstegi
    {
        public int Tur { get; set; }
        public int Yil { get; set; }
        public List<PrgKodFarkSatiri> Satirlar { get; set; }
    }

    /// <summary>
    /// Programlar / Alt Programlar / Faaliyetler / Alt Faaliyetler sayfasının (eski ButceHazirlikv2/ProgButKod.aspx?tur=1..4) uçları:
    ///   GET  /api/progbutkod/liste?tur=&amp;yil=&amp;ara=      (eski btnListele_Click)
    ///   GET  /api/progbutkod/rapor?tur=&amp;yil=&amp;ara=      Yazdır: Excel (şablon BKD\ProgKod.xltx, eski kodla)
    ///   GET  /api/progbutkod/sbb-fark?tur=&amp;yil=          SBB'den Al: program bütçe sistemindeki kodlardan SGB'dekinden farklı olanlar
    ///   POST /api/progbutkod/kaydet, /sil, /aktar (önceki yıldan al), /sbb-kaydet (farklardan seçilenleri yazar)
    /// Her tür menüde ayrı seçenektir; yetki o türün adresinden gelir.
    /// </summary>
    public static class ProgButKodUclari
    {
        public static string Adres(int tur)
        {
            return ButceTanimOrtak.Klasor + "ProgButKod.aspx?tur=" + tur;
        }

        public static string KodAdi(int tur)
        {
            if (tur == 1) return "Program";
            if (tur == 2) return "Alt Program";
            if (tur == 3) return "Faaliyet";
            if (tur == 4) return "Alt Faaliyet";
            return "";
        }

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/progbutkod/liste", Liste);
            app.MapGet("/api/progbutkod/rapor", Rapor);
            app.MapGet("/api/progbutkod/sbb-fark", SbbFark);
            app.MapPost("/api/progbutkod/kaydet", Kaydet);
            app.MapPost("/api/progbutkod/sil", Sil);
            app.MapPost("/api/progbutkod/aktar", Aktar);
            app.MapPost("/api/progbutkod/sbb-kaydet", SbbKaydet);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, int tur, out Kullanici kullanan)
        {
            kullanan = null;
            if (tur < 1 || tur > 4) return Results.BadRequest();
            return ButceTanimOrtak.Denetle(baglam, ortam, out kullanan, Adres(tur));
        }

        private static async Task<(IResult hata, Kullanici kullanan)> DenetleYaz(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, int tur)
        {
            if (tur < 1 || tur > 4) return (Results.BadRequest(), null);
            return await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, Adres(tur));
        }

        private static ObjectArray Oku(Kullanici kullanan, int tur, int yil, string ara)
        {
            PrgKodYapi bilgi = new PrgKodYapi();
            bilgi.yil = yil > 0 ? yil : DateTime.Now.Year;
            bilgi.sorguKriter = ara ?? "";
            bilgi.kodTur = tur;
            return TNS.BKD.Arac.Tanimla().PrgKodYapiListele(kullanan, bilgi);
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int tur, int? yil, string ara)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray oa = Oku(kullanan, tur, yil ?? 0, ButceTanimOrtak.Temiz(ara));
                if (oa == null || !oa.sonuc.islemSonuc)
                    return ButceTanimOrtak.Hata(YatProjeOrtak.HataMetni(oa == null ? null : oa.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (PrgKodYapi p in oa.objeler)
                    satirlar.Add(new { yil = p.yil, kod = p.kod, ad = p.ad ?? "", tur = p.tur, fonKod = p.fonKod ?? "" });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Liste okunamadı.");
            }
        }

        private static PrgKodYapi Ol(PrgKodIstegi istek)
        {
            PrgKodYapi bilgi = new PrgKodYapi();
            bilgi.yil = istek.Yil;
            bilgi.kodTur = istek.Tur;
            bilgi.kod = istek.Kod;
            bilgi.ad = ButceTanimOrtak.Temiz(istek.Ad);
            if (istek.Tur == 1) bilgi.tur = istek.ProgramTur;
            if (istek.Tur == 3) bilgi.fonKod = ButceTanimOrtak.Temiz(istek.FonKod);
            return bilgi;
        }

        /// <summary>Kaydet (eski btnKaydet_Click).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PrgKodIstegi istek, IAntiforgery sahtecilik)
        {
            if (istek == null) return Results.BadRequest();
            var (ret, kullanan) = await DenetleYaz(baglam, ortam, sahtecilik, istek.Tur);
            if (ret != null) return ret;
            try
            {
                return ButceTanimOrtak.SonucYaz(TNS.BKD.Arac.Tanimla().PrgKodYapiKaydet(kullanan, Ol(istek)), "Bilgiler başarıyla saklandı.", "Kayıt işlemi başarısız.");
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Kayıt işlemi başarısız.");
            }
        }

        /// <summary>Sil (eski btnSil_Click).</summary>
        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, PrgKodIstegi istek, IAntiforgery sahtecilik)
        {
            if (istek == null) return Results.BadRequest();
            var (ret, kullanan) = await DenetleYaz(baglam, ortam, sahtecilik, istek.Tur);
            if (ret != null) return ret;
            try
            {
                return ButceTanimOrtak.SonucYaz(TNS.BKD.Arac.Tanimla().PrgKodYapiSil(kullanan, Ol(istek)), "Bilgiler silindi.", "Silme işlemi başarısız.");
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Silme işlemi başarısız.");
            }
        }

        /// <summary>Önceki Yıldan Al (eski btnAktar_Click): seçilen yılın bir önceki yılının kodları seçilen yıla aktarılır (iş kuralı).</summary>
        private static async Task<IResult> Aktar(HttpContext baglam, IWebHostEnvironment ortam, PrgKodIstegi istek, IAntiforgery sahtecilik)
        {
            if (istek == null) return Results.BadRequest();
            var (ret, kullanan) = await DenetleYaz(baglam, ortam, sahtecilik, istek.Tur);
            if (ret != null) return ret;
            if (istek.Yil <= 0) return ButceTanimOrtak.Basarisiz("Yıl bilgisi boş bırakılamaz.");
            try
            {
                PrgKodYapi bilgi = new PrgKodYapi();
                bilgi.yil = istek.Yil - 1;
                bilgi.kodTur = istek.Tur;
                return ButceTanimOrtak.SonucYaz(TNS.BKD.Arac.Tanimla().PrgKodYapiAktar(kullanan, bilgi), "Bilgiler başarıyla saklandı.", "Aktarım başarısız.");
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Aktarım başarısız.");
            }
        }

        private static SBB.EnumPAFTurW SbbKodTuru(int tur)
        {
            if (tur == 1) return SBB.EnumPAFTurW.Program;
            if (tur == 2) return SBB.EnumPAFTurW.AltProgram;
            if (tur == 3) return SBB.EnumPAFTurW.Faaliyet;
            return SBB.EnumPAFTurW.AltFaaliyet;
        }

        /// <summary>
        /// SBB'den Al (eski wndFark_Load): program bütçe sisteminden yılın kodları okunur; SGB'de olmayan ya da adı farklı olanlar
        /// SGB'deki karşılığıyla listelenir. Eski sayfa hata metninde e-bütçe kullanıcı kodu ve parolasını da gösteriyordu; burada gösterilmez.
        /// </summary>
        private static IResult SbbFark(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return ButceTanimOrtak.Hata("Yıl bilgisi boş bırakılamaz.");
            try
            {
                Sonuc s = TNS.MUH.Arac.Tanimla().EbutceAktarimBilgiOku();
                string kullanici = s?.anahtar;
                string parola = s?.bilgiStr;
                if (string.IsNullOrWhiteSpace(kullanici) || string.IsNullOrWhiteSpace(parola))
                    return ButceTanimOrtak.Hata("e-bütçe kullanıcı bilgisi okunamadı.");

                SBB.Listeler l = new SBB.Listeler();
                SBB.ABSKodlarWI[] pbKodlar = l.PAFListele(yil, SbbKodTuru(tur), TNS.DEG.Arac.VarsayilanKurumBul(), kullanici, parola);
                if (pbKodlar == null) return ButceTanimOrtak.Hata("Kod listesi okunamadı.");
                if (pbKodlar.Length == 0) return ButceTanimOrtak.Hata("Kod listesi boş geldi.");

                ObjectArray oa = Oku(kullanan, tur, yil, "");
                TNSCollection sgbKodlar = oa != null && oa.sonuc.islemSonuc ? oa.objeler : new TNSCollection();
                List<object> farklar = new List<object>();
                foreach (SBB.ABSKodlarWI k in pbKodlar)
                {
                    long sgbKod = 0;
                    string sgbAd = "";
                    bool farkli = true;
                    foreach (PrgKodYapi p in sgbKodlar)
                    {
                        if (k.id == p.kod)
                        {
                            sgbKod = k.id;
                            sgbAd = p.ad ?? "";
                            farkli = sgbAd != k.ad;
                            break;
                        }
                    }
                    if (farkli) farklar.Add(new { kodPB = k.id, adPB = k.ad ?? "", kodSGB = sgbKod, adSGB = sgbAd });
                }
                if (farklar.Count == 0)
                    return Results.Json(new { farklar = farklar, bilgi = $"Program bütçeden, {pbKodlar.Length} adet kayıt çekildi fakat SGB ile farklı kayıt bulunamadı!" });
                return Results.Json(new { farklar = farklar });
            }
            catch (Exception ex)
            {
                return ButceTanimOrtak.Hata("Program bütçe sistemine bağlanılamadı: " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        /// <summary>SBB farklarından seçilenleri kaydeder (eski btnKaydetFark_Click); hatalı olanlar tek tek bildirilir.</summary>
        private static async Task<IResult> SbbKaydet(HttpContext baglam, IWebHostEnvironment ortam, PrgKodFarkIstegi istek, IAntiforgery sahtecilik)
        {
            if (istek == null) return Results.BadRequest();
            var (ret, kullanan) = await DenetleYaz(baglam, ortam, sahtecilik, istek.Tur);
            if (ret != null) return ret;
            if (istek.Satirlar == null || istek.Satirlar.Count == 0) return ButceTanimOrtak.Basarisiz("Kaydedilecek satırları işaretleyiniz.");
            try
            {
                string m = "";
                foreach (PrgKodFarkSatiri item in istek.Satirlar)
                {
                    PrgKodYapi bilgi = new PrgKodYapi();
                    bilgi.yil = istek.Yil;
                    bilgi.kodTur = istek.Tur;
                    bilgi.kod = Convert.ToInt32(item.KodPB);
                    bilgi.ad = item.AdPB ?? "";
                    if (bilgi.kodTur == 1) bilgi.tur = 1;
                    else if (bilgi.kodTur == 3) bilgi.fonKod = "XX";
                    Sonuc sonuc = TNS.BKD.Arac.Tanimla().PrgKodYapiKaydet(kullanan, bilgi);
                    if (sonuc == null || !sonuc.islemSonuc)
                        m += $"{item.KodPB}- {item.AdPB} : {YatProjeOrtak.HataMetni(sonuc?.hataStr, "kaydedilemedi")}\n";
                }
                if (m == "") return Results.Json(new { basarili = true, bilgi = "Seçilen kayıtlar kaydedildi." });
                return ButceTanimOrtak.Basarisiz("Seçilen kayıtlar kaydedilirken bazı kayıtlarda hata oluştu:\n" + m);
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Kayıt işlemi başarısız.");
            }
        }

        /// <summary>Yazdır (eski btnYazdir_Click): şablon BKD\ProgKod.xltx; listedeki kodlar ve adları.</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int tur, int? yil, string ara)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int y = yil > 0 ? yil.Value : DateTime.Now.Year;
                ObjectArray oa = Oku(kullanan, tur, y, ButceTanimOrtak.Temiz(ara));
                if (oa == null || !oa.sonuc.islemSonuc)
                    return ButceTanimOrtak.Hata(YatProjeOrtak.HataMetni(oa == null ? null : oa.sonuc.hataStr, "Rapor hazırlanamadı."));

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
                XLS.DosyaAc(Ayar.Oku("RaporSablonYol") + "/BKD/ProgKod.xltx", sonucDosyaAd);
                XLS.HucreAdBulYaz("Yil", y);
                XLS.HucreAdBulYaz("KodAdi", KodAdi(tur));

                int baslaSatir = 0, baslaSutun = 0;
                int sheetNo = 0, formatSatirBas = 0, formatSutunBas = 0, formatSatirBit = 0, formatSutunBit = 0;
                XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
                XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref formatSatirBas, ref formatSutunBas, ref formatSatirBit, ref formatSutunBit);
                int satir = baslaSatir;
                foreach (PrgKodYapi item in oa.objeler)
                {
                    XLS.HucreKopyala(formatSatirBas, formatSutunBas, formatSatirBit, formatSutunBit, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, baslaSutun + 0, item.kod);
                    XLS.HucreDegerYaz(satir, baslaSutun + 1, item.ad);
                    satir++;
                }
                XLS.SatirYukseklikAyarla(baslaSatir, satir, -1, 7, 19);
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, KodAdi(tur) + DosyaIslem.DosyaAdUretSade(), true, "xlsx");
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
