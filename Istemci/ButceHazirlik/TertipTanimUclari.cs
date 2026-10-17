using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Istemci.ButceKod;
using Istemci.Organizasyon;
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
using TNS.UZY;

namespace Istemci.ButceHazirlik
{
    public class TertipKodAd
    {
        public string TertipKodu { get; set; }
        public string TertipAdi { get; set; }
    }

    public class TertipTanimIstegi
    {
        public int Tur { get; set; }
        public int Yil { get; set; }
        public string Peb { get; set; }
        public List<TertipKodAd> Tertipler { get; set; }
    }

    /// <summary>
    /// Tertip tanımları sayfasının (eski ButceHazirlikv2/TertipTanimFormu.aspx; bütçe teklifi ekranındaki "Tertipler" tuşu) uçları:
    ///   GET  /api/tertiptanim/tanim, /kodlar, /liste, /tertipuret
    ///   POST /api/tertiptanim/kaydet, /adlar, /indir, /yukle
    /// Tur 1 gider (TABPEBOdenek: kurumsal-fonksiyonel-finansman-ekonomik-faaliyet), 2 gelir (TABPEBGelir: kurumsal-ekonomik-faaliyet).
    /// Sayfayı sistem yöneticisi ve bütçe teklifi ekranının (gider / gelir) yetkilisi kullanabilir.
    /// </summary>
    public static class TertipTanimUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/tertiptanim/tanim", Tanim);
            app.MapGet("/api/tertiptanim/kodlar", Kodlar);
            app.MapGet("/api/tertiptanim/liste", Liste);
            app.MapGet("/api/tertiptanim/tertipuret", TertipUret);
            app.MapPost("/api/tertiptanim/kaydet", Kaydet);
            app.MapPost("/api/tertiptanim/adlar", AdUret);
            app.MapPost("/api/tertiptanim/indir", Indir);
            app.MapPost("/api/tertiptanim/yukle", Yukle).DisableAntiforgery();
        }

        private static bool Gelir(int tur) { return tur == 2; }

        /// <summary>
        /// Eski sayfada yetki denetimi yoktu; burada sistem yöneticisi, ilgili bütçe teklifi ekranının yetkilisi ya da sayfayı satırdaki
        /// düğmeyle açan Alt Faaliyet İlişkileri ekranının yetkilisi.
        /// </summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, int tur)
        {
            return ButceRakamUclari.YetkiVar(kullanan, uygulamaKlasoru, Gelir(tur) ? 2 : 1, 0)
                || AltFaaliyetIliskiUclari.YetkiVarHerhangi(kullanan, uygulamaKlasoru);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, int tur, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, tur)) return Results.StatusCode(StatusCodes.Status403Forbidden);
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

        private static ENUMTabloKod TabloKod(int tur) { return Gelir(tur) ? ENUMTabloKod.TABPEBGelir : ENUMTabloKod.TABPEBOdenek; }

        private static string Dil(Kullanici kullanan)
        {
            return string.IsNullOrEmpty(kullanan.aktifKultur) ? TNS.BKD.Arac.Diller(0) : kullanan.aktifKultur;
        }

        /// <summary>Tabloya 10 boş satır eklenir (eski OnEkle): yeni tertipler bunlara yazılır.</summary>
        private static List<TertipKodAd> OnEkle(List<TertipKodAd> liste)
        {
            for (int i = 0; i < 10; i++) liste.Add(new TertipKodAd { TertipKodu = "", TertipAdi = "" });
            return liste;
        }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int tur, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            return Results.Json(new
            {
                baslik = Gelir(tur) ? "Gelir Tertipleri" : "Gider Tertipleri",
                aciklama = Gelir(tur) ? "Gelir tertiplerinin listelendiği ekran" : "Gider tertiplerinin listelendiği ekran",
                yil = yil.HasValue && yil.Value >= 2000 ? yil.Value : KodTanimUclari.YilBul(null),
                kurum = TNS.DEG.Arac.VarsayilanKurumBul()
            });
        }

        /// <summary>Kod ağacının bir düzeyi (eski nodeLoadPar; yetki süzmesi yok).</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string kodTur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            kodTur = Temiz(kodTur);
            if (Array.IndexOf(new[] { "1", "2", "3", "4", "5", "14" }, kodTur) < 0) return Hata("Kod türü hatalı!");
            ust = Temiz(ust);
            try
            {
                IBKDServis servis = TNS.BKD.Arac.Tanimla();
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
                ABSKodlarVAN kodlar = servis.KodListeleSeviyeIkiYil(yil, kodTur, ust, 0, false, false, "", "", "", true, true, "", false);
                if (kodlar == null || !kodlar.sonuc.islemSonuc) return Hata("Kodlar okunamadı.");
                string dil = Dil(kullanan);
                foreach (ABSKod k in kodlar.absKodlar)
                    dugumler.Add(new { kod = k.kod, ad = k.kod + "- " + k.Ad(dil), altVar = altVar });
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Kodlar okunamadı.");
            }
        }

        /// <summary>Listele / Listele (Fon) (eski Listele): seçime uyan tanımlı tertipler ve 10 boş satır.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string kur, string fon, string fin,
            string eko, string peb, bool fonUyumsuz)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                Tertip tertip = Gelir(tur)
                    ? new Tertip(yil, ENUMTabloKod.TABPEBGelir, Temiz(kur), Temiz(eko), Temiz(peb))
                    : new Tertip(yil, ENUMTabloKod.TABPEBOdenek, Temiz(kur), Temiz(fon), Temiz(fin), Temiz(eko), Temiz(peb));
                Tertip[] tertipler = TNS.BHMv2.Arac.Tanimla().TertipTanimlariListeleFonkod(kullanan, tertip, !Gelir(tur) && fonUyumsuz);
                if (tertipler == null) return Hata("Bu alt faaliyete ait tertip bulunamadı.");
                List<TertipKodAd> liste = new List<TertipKodAd>();
                foreach (Tertip t in tertipler)
                    liste.Add(new TertipKodAd { TertipKodu = t.TertipKodu, TertipAdi = t.TertipAdi });
                return Results.Json(new { tertipler = OnEkle(liste) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tertipler okunamadı."));
            }
        }

        /// <summary>Kaydet (eski Kaydet): tertip kodu dolu satırlar kaydedilir; adı boş bırakılan tertip silinir.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, TertipTanimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.Yil <= 0) return Results.Json(new { basarili = false, hata = "Anahtar alanları girmelisiniz." });
            try
            {
                List<Tertip> al = new List<Tertip>();
                foreach (TertipKodAd tka in istek.Tertipler ?? new List<TertipKodAd>())
                {
                    string tertipStr = Temiz(tka == null ? null : tka.TertipKodu);
                    if (tertipStr == "") continue;
                    Tertip tertip = new Tertip(istek.Yil, TabloKod(istek.Tur));
                    tertip.TertipAta(tertipStr);
                    tertip.tertipAdi = tka.TertipAdi;
                    tertip.durum = ENUMTertipDurum.NORMAL;
                    al.Add(tertip);
                }
                if (al.Count == 0) return Results.Json(new { basarili = false, hata = "Kaydedilecek tertip yok." });
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().TertipTanimlariKaydet(kullanan, al.ToArray());
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt İşlemi Tamamlandı" });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = HataMetni(ex.Message, "Kayıt işlemi başarısız.") });
            }
        }

        /// <summary>
        /// Ad Üret (eski btnAdUret_Click): tertip kodundan ad üretilir. Faaliyeti eksik tertibe (gelirde 2, giderde 4 parça) seçili
        /// faaliyet eklenir; ad gelirde "ekonomik (faaliyet)", giderde "ekonomik (faaliyet)" kod adlarıdır.
        /// </summary>
        private static async Task<IResult> AdUret(HttpContext baglam, IWebHostEnvironment ortam, TertipTanimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.Yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                List<TertipKodAd> tertipler = istek.Tertipler ?? new List<TertipKodAd>();
                Tertip t = new Tertip(istek.Yil, TabloKod(istek.Tur));
                string pebkod = Temiz(istek.Peb);
                IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
                foreach (TertipKodAd tka in tertipler)
                {
                    if (tka == null) continue;
                    string tertipStr = tka.TertipKodu;
                    if (string.IsNullOrEmpty(tertipStr)) continue;
                    string ad;
                    string[] kodlar = tertipStr.Split('-');
                    if (kodlar.Length == 2 || kodlar.Length == 3)
                    {
                        if (kodlar.Length == 2)
                        {
                            tertipStr += "-" + pebkod;
                            kodlar = tertipStr.Split('-');
                            tka.TertipKodu = tertipStr;
                        }
                        t.TertipAta(kodlar);
                        t.AdlariBul(uzyServis);
                        ad = t.TamMi() ? t.adlar[1] + " (" + t.adlar[2] + ")" : "HATA! Tertip içi kırımlar tam değil";
                    }
                    else if (kodlar.Length == 4 || kodlar.Length == 5)
                    {
                        if (kodlar.Length == 4)
                        {
                            tertipStr += "-" + pebkod;
                            kodlar = tertipStr.Split('-');
                            tka.TertipKodu = tertipStr;
                        }
                        t.TertipAta(kodlar);
                        t.AdlariBul(uzyServis);
                        ad = t.TamMi() ? t.adlar[3] + " (" + t.adlar[4] + ")" : "HATA! Tertip içi kırımlar tam değil";
                    }
                    else
                        ad = "HATA! Tertibin kırılımları tam değil";
                    tka.TertipAdi = ad;
                }
                return Results.Json(new { tertipler = tertipler });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Adlar üretilemedi."));
            }
        }

        /// <summary>
        /// Tertip Üret (eski btnTertipUret_Click): seçilen kodların altındaki son düzey kodların bütün birleşimleri tertip olarak
        /// tabloya gelir (kaydedilmez); ad ekonomik kod ve faaliyet adlarıdır.
        /// </summary>
        private static IResult TertipUret(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string kur, string fon, string fin,
            string eko, string peb)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                IBKDServis bkdServis = TNS.BKD.Arac.Tanimla();
                string[] secilen = Gelir(tur) ? new[] { Temiz(kur), Temiz(eko), Temiz(peb) } : new[] { Temiz(kur), Temiz(fon), Temiz(fin), Temiz(eko), Temiz(peb) };
                Tertip tertip = new Tertip(yil, TabloKod(tur));
                ABSKodlarVAN[] kodlar = new ABSKodlarVAN[secilen.Length];
                for (int i = 0; i < secilen.Length; i++)
                    kodlar[i] = bkdServis.KodListeleSeviye(yil, tertip.KodNolar(i).ToString(), secilen[i], tertip.KodNoKirilimSay(i), true);

                long birlesim = 1;
                foreach (ABSKodlarVAN k in kodlar)
                    birlesim *= Math.Max(1, k == null || k.absKodlar == null ? 0 : k.absKodlar.Count);
                if (birlesim > 20000) return Hata("Seçime uyan " + birlesim.ToString("N0") + " tertip çıkıyor; kodları daraltınız.");

                List<TertipKodAd> al = new List<TertipKodAd>();
                KodlariDolas(al, kodlar, 0, new ABSKod[kodlar.Length], Dil(kullanan));
                return Results.Json(new { tertipler = al });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tertipler üretilemedi."));
            }
        }

        private static int KodlariDolas(List<TertipKodAd> al, ABSKodlarVAN[] kodlar, int indis, ABSKod[] aktifKodlar, string dil)
        {
            if (kodlar == null || indis >= kodlar.Length) return -1;
            ABSKodlarVAN kodlarIc = kodlar[indis];
            if (kodlarIc == null || kodlarIc.absKodlar.Count == 0 || kodlarIc.sonuc == null || !kodlarIc.sonuc.islemSonuc) return -1;
            for (int i = 0; i < kodlarIc.absKodlar.Count; i++)
            {
                aktifKodlar[indis] = (ABSKod)kodlarIc.absKodlar[i];
                if (kodlar.Length - 1 == indis)
                    TertipEkle(al, aktifKodlar, dil);
                else
                {
                    int tut = KodlariDolas(al, kodlar, indis + 1, aktifKodlar, dil);
                    if (tut < 0) return tut;
                }
            }
            return 0;
        }

        private static void TertipEkle(List<TertipKodAd> al, ABSKod[] aktifKodlar, string dil)
        {
            string tertipStr;
            string ad;
            if (aktifKodlar.Length > 4)
            {
                tertipStr = aktifKodlar[0].kod + "-" + aktifKodlar[1].kod + "-" + aktifKodlar[2].kod + "-" + aktifKodlar[3].kod + "-" + aktifKodlar[4].kod;
                ad = aktifKodlar[3].Ad(dil) + " (" + aktifKodlar[4].Ad(dil) + ")";
            }
            else
            {
                tertipStr = aktifKodlar[0].kod + "-" + aktifKodlar[1].kod + "-" + aktifKodlar[2].kod;
                ad = aktifKodlar[1].Ad(dil) + " (" + aktifKodlar[2].Ad(dil) + ")";
            }
            al.Add(new TertipKodAd { TertipKodu = tertipStr, TertipAdi = ad });
        }

        /// <summary>İndir (eski btnDosyaSakla_Click): tablodaki satırlar Excel'e (tertip, ad).</summary>
        private static async Task<IResult> Indir(HttpContext baglam, IWebHostEnvironment ortam, TertipTanimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                AsposeLisans.Yukle();
                string dosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.BosDosyaAc(dosyaAd);
                List<TertipKodAd> tertipler = istek.Tertipler ?? new List<TertipKodAd>();
                for (int sat = 0; sat < tertipler.Count; sat++)
                {
                    XLS.HucreDegerYaz(sat, 0, tertipler[sat] == null ? "" : tertipler[sat].TertipKodu ?? "");
                    XLS.HucreDegerYaz(sat, 1, tertipler[sat] == null ? "" : tertipler[sat].TertipAdi ?? "");
                }
                XLS.SutunGenislikAyarla(0, 0, 40);
                XLS.SutunGenislikAyarla(1, 1, 100);
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(dosyaAd, "Tertipler_" + DateTime.Now.Ticks, true, "xlsx");
            }
            catch (Exception)
            {
                return Hata("Dosya hazırlanamadı.");
            }
        }

        /// <summary>
        /// Dosya Yükle (eski btnDosyaYukle_Click): Excel'in ilk sütunu tertip kodu, ikincisi adıdır; kodu ya da adı boş ilk satırda durulur.
        /// Satırlar tabloya gelir, kaydetmek için Kaydet'e basılır.
        /// </summary>
        private static async Task<IResult> Yukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IFormCollection form = await baglam.Request.ReadFormAsync();
            int tur = OrtakFonksiyonlar.ConvertToInt(form["tur"].ToString(), 1);
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return Hata("Yüklemek istediğiniz dosyanın içi boş.");
            if (dosya.Length > 20 * 1024 * 1024) return Hata("Dosya çok büyük (en çok 20 MB).");
            string uzanti = Path.GetExtension(dosya.FileName ?? "").ToLowerInvariant();
            if (uzanti != ".xls" && uzanti != ".xlsx") uzanti = ".xlsx";
            string dosyaAdxls = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul() + uzanti;
            try
            {
                using (FileStream hedef = File.Create(dosyaAdxls))
                    await dosya.CopyToAsync(hedef);
                AsposeLisans.Yukle();
                List<TertipKodAd> al = new List<TertipKodAd>();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(dosyaAdxls);
                try
                {
                    for (int sat = 0; ; sat++)
                    {
                        TertipKodAd tka = new TertipKodAd { TertipKodu = XLS.HucreDegerAl(sat, 0), TertipAdi = XLS.HucreDegerAl(sat, 1) };
                        if (string.IsNullOrEmpty(tka.TertipKodu) || string.IsNullOrEmpty(tka.TertipAdi)) break;
                        al.Add(tka);
                    }
                }
                finally
                {
                    XLS.DosyaKapat();
                }
                return Results.Json(new { tertipler = OnEkle(al) });
            }
            catch (Exception)
            {
                return Hata("Dosya okunamadı.");
            }
            finally
            {
                try { File.Delete(dosyaAdxls); } catch { }
            }
        }
    }
}
