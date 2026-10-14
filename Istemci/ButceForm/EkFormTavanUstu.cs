using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.BKD;
using TNS.KYM;
using TNS.UZY;

namespace Istemci.ButceForm
{
    public class TavanUstuOdenekIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        /// <summary>Form 25: tablonun ilk satırının açıklaması (eski Cells[0, 0]).</summary>
        public string Aciklama { get; set; }
        /// <summary>Form 25: ilk satırın 13 ekonomik kod tutarı (eski Cells[0, 1..13]).</summary>
        public double?[] Tutarlar { get; set; }
    }

    public class TavanUstuDetaySatirIstegi
    {
        public string EkoKod { get; set; }
        public double? Tutar { get; set; }
        public string Aciklama { get; set; }
    }

    public class TavanUstuDetayIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public List<TavanUstuDetaySatirIstegi> Satirlar { get; set; }
    }

    /// <summary>
    /// Form 25 (tavanı aşan ilave ödenek teklifleri, eski ButceForm/TavanUstuOdenek.aspx) ve
    /// Form 25.2 (detay, eski ButceForm/TavanUstuOdenek2.aspx):
    ///   GET  /api/ekform/tavanustu/liste?yil=&amp;kur=&amp;senaryo=&amp;detaydan=        Form 25: eski TavanUstuListele(false) -> TavanUstuListele;
    ///                                                                        "Tavan Üstünden Al" (detaydan=true) -> TavanUstuListeleDetaydan(.., false)
    ///   POST /api/ekform/tavanustu/kaydet                                     Form 25: eski btnKaydet_Click -> TavanUstuKaydet
    ///   GET  /api/ekform/tavanustu/yazdir?yil=&amp;kur=                            Form 25: eski btnYazdir_Click -> TavanUstuOdenekHazirla
    ///   GET  /api/ekform/tavanustu/detayliste?yil=&amp;kur=&amp;senaryo=&amp;detaydan=  Form 25.2: eski GridDoldur + TavanUstuListele(false) -> TavanUstuListeleDetay;
    ///                                                                        "Tavan Üstünden Al" -> TavanUstuListeleDetaydan(.., true)
    ///   POST /api/ekform/tavanustu/detaykaydet                                Form 25.2: eski btnKaydet_Click -> TavanUstuKaydetDetay
    ///   GET  /api/ekform/tavanustu/detayyazdir?yil=&amp;kur=&amp;senaryo=           Form 25.2: eski btnYazdir_Click -> TavanUstuOdenek2Hazirla
    /// </summary>
    public static class EkFormTavanUstu
    {
        /// <summary>Eski FRMTUO005 / FRMTUO007 / FRMTO2005.</summary>
        private const string SecimHatasi = "Yıl veya Kurum alanlarından biri veya bir kaçı boş bırakılmış.  Lütfen ilgili alanları kontrol edip tekrar deneyin.";

        /// <summary>Eski TavanUstuOdenek.ekonomikKodlar4 / ekonomikKodlar3 (tablonun 1..13. sütunları).</summary>
        private static readonly string[] ekonomikKodlar4 = { "01", "02", "03.01", "03.02", "03.03", "03.04", "03.05", "03.06", "03.07", "03.08", "03.09", "05", "07" };
        private static readonly string[] ekonomikKodlar3 = { "01", "02", "03.1", "03.2", "03.3", "03.4", "03.5", "03.6", "03.7", "03.8", "03.9", "05", "07" };

        /// <summary>Eski TavanUstuOdenek2.ekonomikKodlar (Form 25.2 satırları).</summary>
        private static readonly string[] detayEkonomikKodlar = { "032201", "032301", "032302", "032303", "035502", "035505", "035512" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/tavanustu/liste", Liste);
            app.MapPost("/api/ekform/tavanustu/kaydet", Kaydet);
            app.MapGet("/api/ekform/tavanustu/yazdir", Yazdir);
            app.MapGet("/api/ekform/tavanustu/detayliste", DetayListe);
            app.MapPost("/api/ekform/tavanustu/detaykaydet", DetayKaydet);
            app.MapGet("/api/ekform/tavanustu/detayyazdir", DetayYazdir);
        }

        /// <summary>Eski Page_Load: PEB ödenek tertibinde ekonomik kodun 2. kırılımı 4 haneliyse "03.01" biçimi, değilse "03.1".</summary>
        private static string[] EkonomikKodlar(int yil)
        {
            Tertip t = new Tertip(yil, TNS.BKD.ENUMTabloKod.TABPEBOdenek);
            return t.KodUzunlukBul(3, 2, true) == 4 ? ekonomikKodlar4 : ekonomikKodlar3;
        }

        private static int EkoKodYerBul(string[] kodlar, string ekoKod)
        {
            for (int i = 0; i < kodlar.Length; i++)
                if (string.Equals(ekoKod, kodlar[i])) return i;
            return -1;
        }

        // ------------------------------------------------------------------ Form 25

        /// <summary>
        /// Eski TavanUstuListele(tavanUstunden): kurumsal kod her değiştiğinde yeni satır; satırda açıklama ve 13 ekonomik kodun tutarı.
        /// Kayıt yoksa tek boş satır.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string senaryo, bool detaydan)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(SecimHatasi);
            kur = kur.Trim();
            try
            {
                string[] kodlar = EkonomikKodlar(yil);
                IBHFServis servis = TNS.BHF.Arac.Tanimla();
                ObjectArray tsv = detaydan
                    ? servis.TavanUstuListeleDetaydan(kullanan, yil, kur, EkFormUclari.Temiz(senaryo), false)
                    : servis.TavanUstuListele(kullanan, yil, kur);
                if (!tsv.sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(tsv.sonuc.hataStr, "Tavan üstü ödenekler okunamadı."));

                List<SatirF25> satirlar = new List<SatirF25>();
                string oncBirim = "";
                foreach (TNS.BHF.TavanUstuOdenek tss in tsv.objeler ?? new TNSCollection())
                {
                    string birim = tss.kurumsalKod ?? "";
                    if (satirlar.Count == 0 || birim != oncBirim)
                    {
                        satirlar.Add(new SatirF25 { birim = birim, tutarlar = new double?[kodlar.Length] });
                        oncBirim = birim;
                    }
                    SatirF25 s = satirlar[satirlar.Count - 1];
                    if (!string.IsNullOrWhiteSpace(tss.aciklama)) s.aciklama = tss.aciklama;
                    int yer = EkoKodYerBul(kodlar, tss.ekokod);
                    if (yer >= 0) s.tutarlar[yer] = tss.odenek;
                }
                if (satirlar.Count == 0) satirlar.Add(new SatirF25 { birim = "", tutarlar = new double?[kodlar.Length] });
                return Results.Json(new { kodlar = kodlar, satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Tavan üstü ödenekler okunamadı."));
            }
        }

        private class SatirF25
        {
            public string birim { get; set; }
            public string aciklama { get; set; } = "";
            public double?[] tutarlar { get; set; }
        }

        /// <summary>
        /// Eski btnKaydet_Click: yalnız tablonun ilk satırı saklanır; sıfırdan büyük her ekonomik kod tutarı, ilk satırın açıklamasıyla
        /// bir TavanUstuOdenek olur.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TavanUstuOdenekIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new TavanUstuOdenekIstegi();
            if (!EkFormUclari.SecimGecerli(istek.Yil, istek.Kur)) return EkFormUclari.Hata(SecimHatasi);
            try
            {
                string[] kodlar = EkonomikKodlar(istek.Yil);
                TNSCollection tisc = new TNSCollection();
                for (int i = 0; i < kodlar.Length; i++)
                {
                    double odenek = istek.Tutarlar != null && istek.Tutarlar.Length > i ? (istek.Tutarlar[i] ?? 0) : 0;
                    if (odenek > 0)
                    {
                        TNS.BHF.TavanUstuOdenek tis = new TNS.BHF.TavanUstuOdenek();
                        tis.odenek = odenek;
                        tis.aciklama = istek.Aciklama ?? "";
                        tis.ekokod = kodlar[i];
                        tisc.Add(tis);
                    }
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().TavanUstuKaydet(kullanan, istek.Yil, istek.Kur.Trim(), tisc);
                if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return EkFormUclari.Tamam("Bilgiler başarıyla saklandı.");
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>Eski btnYazdir_Click: TavanUstuOdenekHazirla (ButceYil, KurKod), dosya adı form25_{kur}.</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(SecimHatasi);
            kur = kur.Trim();
            string tur = EkFormUclari.ExcelTur();
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().TavanUstuOdenekHazirla(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), tur, EkFormUclari.RaporParam(yil, kur, ""), ""),
                "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "TavanUstuOdenek") + "_" + kur + "." + tur);
        }

        // ------------------------------------------------------------------ Form 25.2 (detay)

        /// <summary>
        /// Eski GridDoldur + TavanUstuListele(YirmiBes2): yedi sabit ekonomik kod satırı (noktalı kod ve ABSEKO adı); okunan tutar ve
        /// açıklama, noktasız kodu eşleşen satıra yazılır.
        /// </summary>
        private static IResult DetayListe(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string senaryo, bool detaydan)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(SecimHatasi);
            kur = kur.Trim();
            try
            {
                Tertip t = new Tertip(yil, TNS.BKD.ENUMTabloKod.TABOdenek);
                IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                List<SatirF252> satirlar = new List<SatirF252>();
                foreach (string eko in detayEkonomikKodlar)
                {
                    satirlar.Add(new SatirF252
                    {
                        ekoKod = eko,
                        kod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(3), eko),
                        ad = servisUZY.UzayDegeriStr(kullanan, "ABSEKO", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), eko), true) ?? ""
                    });
                }

                IBHFServis servis = TNS.BHF.Arac.Tanimla();
                ObjectArray tsv = detaydan
                    ? servis.TavanUstuListeleDetaydan(kullanan, yil, kur, EkFormUclari.Temiz(senaryo), true)
                    : servis.TavanUstuListeleDetay(kullanan, yil, kur);
                if (!tsv.sonuc.islemSonuc)
                    return Results.Json(new { satirlar = satirlar, hataBilgi = EkFormUclari.HataMetni(tsv.sonuc.hataStr, "Tavan üstü ödenekler okunamadı.") });

                foreach (TNS.BHF.TavanUstuOdenek tss in tsv.objeler ?? new TNSCollection())
                {
                    string eko = (tss.ekokod ?? "").Replace(".", "");
                    SatirF252 s = satirlar.Find(x => x.kod.Replace(".", "") == eko);
                    if (s != null)
                    {
                        s.tutar = tss.odenek;
                        s.aciklama = tss.aciklama ?? "";
                    }
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Tavan üstü ödenekler okunamadı."));
            }
        }

        private class SatirF252
        {
            public string ekoKod { get; set; }
            public string kod { get; set; }
            public string ad { get; set; }
            public double? tutar { get; set; }
            public string aciklama { get; set; } = "";
        }

        /// <summary>Eski btnKaydet_Click: tutarı sıfırdan büyük satırlar (noktasız ekonomik kod, tutar, gerekçe) TavanUstuKaydetDetay'a.</summary>
        private static async Task<IResult> DetayKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TavanUstuDetayIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new TavanUstuDetayIstegi();
            if (!EkFormUclari.SecimGecerli(istek.Yil, istek.Kur)) return EkFormUclari.Hata(SecimHatasi);
            try
            {
                TNSCollection tisc = new TNSCollection();
                foreach (TavanUstuDetaySatirIstegi s in istek.Satirlar ?? new List<TavanUstuDetaySatirIstegi>())
                {
                    if (s == null) continue;
                    string eko = (s.EkoKod ?? "").Replace(".", "");
                    // Yalnız formdaki sabit ekonomik kodlar (eski tabloda kod sütunu kilitliydi).
                    if (Array.IndexOf(detayEkonomikKodlar, eko) < 0) continue;
                    double odenek = s.Tutar ?? 0;
                    if (odenek > 0)
                    {
                        TNS.BHF.TavanUstuOdenek tis = new TNS.BHF.TavanUstuOdenek();
                        tis.odenek = odenek;
                        tis.aciklama = s.Aciklama ?? "";
                        tis.ekokod = eko;
                        tisc.Add(tis);
                    }
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().TavanUstuKaydetDetay(kullanan, istek.Yil, istek.Kur.Trim(), tisc);
                if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return EkFormUclari.Tamam("Bilgiler başarıyla saklandı.");
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>Eski btnYazdir_Click: TavanUstuOdenek2Hazirla (ButceYil, KurKod, Senaryo), dosya adı form25_2_{kur}.</summary>
        private static IResult DetayYazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string senaryo)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(SecimHatasi);
            kur = kur.Trim();
            string tur = EkFormUclari.ExcelTur();
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().TavanUstuOdenek2Hazirla(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), tur, EkFormUclari.RaporParam(yil, kur, senaryo), ""),
                "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "TavanUstuOdenek") + "_2_" + kur + "." + tur);
        }
    }
}
