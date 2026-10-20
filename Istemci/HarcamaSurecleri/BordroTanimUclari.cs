using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.HRC;
using TNS.KYM;

namespace Istemci.HarcamaSurecleri
{
    /// <summary>Değişken / formül satırı: alan adları DegiskenFormul sınıfındaki gibi.</summary>
    public class BordroDegiskenSatiri
    {
        public string ad { get; set; }
        public string tipStr { get; set; }
        public int noktaSay { get; set; }
        public string formula { get; set; }
        public string borcHesap { get; set; }
        public string alacakHesap { get; set; }
        public string kisaEtiket { get; set; }
        public string uzunEtiket { get; set; }
        public string siraNo { get; set; }
        public int genislik { get; set; }
        public string ilkDeger { get; set; }
    }

    /// <summary>Sakla isteği. okunanBordroTur / okunanIslemTur: ekranda en son okunan bordro (eski hdnBordroTur / hdnBordroIslem).</summary>
    public class BordroSaklaIstegi
    {
        public int bordroTur { get; set; }
        public int islemTur { get; set; }
        public int okunanBordroTur { get; set; }
        public int okunanIslemTur { get; set; }
        public string ad { get; set; }
        public string kaynak { get; set; }
        public List<BordroDegiskenSatiri> degiskenler { get; set; }
        public List<BordroDegiskenSatiri> formuller { get; set; }
        public string ortakFonksiyonlar { get; set; }
    }

    /// <summary>Kodu İndir isteği: ekrandaki (saklanmamış olabilir) ortak fonksiyonlarla derlenir.</summary>
    public class BordroKodIstegi
    {
        public int bordroTur { get; set; }
        public int islemTur { get; set; }
        public string ortakFonksiyonlar { get; set; }
    }

    /// <summary>Şablon penceresi isteği. Anahtar alanlar eski metin kutularındaki gibi metin olarak gelir.</summary>
    public class BordroSablonIstegi
    {
        public string bordroTur { get; set; }
        public string islemTur { get; set; }
        public string sablonKod { get; set; }
        public string sablonAd { get; set; }
        public string gecerliKodlar { get; set; }
        public string siraNo { get; set; }
        public bool sayfa { get; set; }
        public bool ortakRapor { get; set; }
        public bool tekKisilik { get; set; }
        public int yeni { get; set; }
    }

    /// <summary>
    /// Bordro Tanım Ekranı (eski HarcamaSurecleri/TanimBordroTanim.aspx; yeni sayfa HarcamaSurecleri/TanimBordroTanim) uçları:
    ///   GET  /api/bordrotanim/baslangic          bordro türleri (personel tipleri), işlem türleri, veri tipleri, bordro genel değişkenleri
    ///   GET  /api/bordrotanim/oku                bordro yapısı (değişkenler, formüller) + ortak fonksiyonlar + ortak şablonlar (eski Oku)
    ///   POST /api/bordrotanim/sakla              (eski Sakla)
    ///   POST /api/bordrotanim/kodindir           derlenen kod .txt (eski Kodu İndir)
    ///   GET  /api/bordrotanim/sablonuret         bordro şablonu .xlsx (eski Şablon Üret)
    ///   POST /api/bordrotanim/hesapla            Excel'deki veriyle formülleri çalıştırır, sonuç .xlsx (eski Çalıştır > Hesapla)
    ///   GET  /api/bordrotanim/sablonlar          ortak şablonlar
    ///   POST /api/bordrotanim/sablonkaydet | sablonsil | sablonyukle,  GET /api/bordrotanim/sablonal
    /// İş kuralları eski sayfadaki gibi IHRCServis (BordroYapiOku / BordroYapiSakla / BordroSablon*), PSLDinBordroHesaplayici ve
    /// IKYMServis.BuyukMetinSakla.
    /// </summary>
    public static class BordroTanimUclari
    {
        public const string SayfaAdresi = "../HarcamaSurecleri/TanimBordroTanim.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/bordrotanim/baslangic", Baslangic);
            app.MapGet("/api/bordrotanim/oku", Oku);
            app.MapPost("/api/bordrotanim/sakla", Sakla);
            app.MapPost("/api/bordrotanim/kodindir", KodIndir);
            app.MapGet("/api/bordrotanim/sablonuret", SablonUret);
            app.MapPost("/api/bordrotanim/hesapla", Hesapla).DisableAntiforgery();
            app.MapGet("/api/bordrotanim/sablonlar", Sablonlar);
            app.MapPost("/api/bordrotanim/sablonkaydet", SablonKaydet);
            app.MapPost("/api/bordrotanim/sablonsil", SablonSil);
            app.MapPost("/api/bordrotanim/sablonyukle", SablonYukle).DisableAntiforgery();
            app.MapGet("/api/bordrotanim/sablonal", SablonAl);
        }

        /// <summary>Eski Page_Load: GenelIslemler.SayfayaGirisYapabilirMi (menü yetkisi).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        // ------------------------------------------------------------------ başlangıç (eski StorelariDoldur)

        private static IResult Baslangic(HttpContext baglam, IWebHostEnvironment ortam, int? bordroIslemTur, int? bordroYapiTur)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();

            List<object> islemTurler = new List<object>();
            TNSCollection islemler = servisHRC.BordroIslemTurListele(kullanan, new BordroIslemTur(0, "")).objeler;
            foreach (BordroIslemTur t in islemler)
                islemTurler.Add(new { t.kod, t.ad });
            int islemTur = bordroIslemTur ?? -1;
            if (islemTur <= 0 && islemler.Count > 0) islemTur = ((BordroIslemTur)islemler[0]).kod;

            List<object> bordroTurler = new List<object>();
            TNSCollection perTipler = TNS.IKY.Arac.Tanimla().ListelePersonelTip(kullanan, new TNS.IKY.PersonelTipi()).objeler;
            foreach (TNS.IKY.PersonelTipi p in perTipler)
                bordroTurler.Add(new { p.kod, p.kodAd });
            int bordroTur = bordroYapiTur ?? -1;
            if (bordroTur <= 0 && perTipler.Count > 0) bordroTur = ((TNS.IKY.PersonelTipi)perTipler[0]).kod;

            List<object> tipler = new List<object>();
            foreach (VariableType vt in new[] { VariableType.Integer, VariableType.Money, VariableType.String, VariableType.Date })
                tipler.Add(new { kod = (int)vt, ad = VariableFunctions.GetTypeUserName(vt) });

            List<object> girisler = new List<object>();
            foreach (BordroGenelDegisken g in BordroGenelDegisken.bordroDegiskenler)
                girisler.Add(new { g.degiskenAd, g.veriTipiStr, g.aciklama });

            return Results.Json(new { islemTurler, bordroTurler, tipler, girisler, bordroTur, islemTur });
        }

        // ------------------------------------------------------------------ oku (eski BordroYapiOku)

        private static object Satir(DegiskenFormul d)
        {
            return new
            {
                d.girisStr, d.ad, d.tipStr, d.noktaSay, d.formula, d.borcHesap, d.alacakHesap, d.kisaEtiket, d.uzunEtiket,
                d.siraNo, d.genislik, d.ilkDeger
            };
        }

        private static List<object> Satirlar(DegiskenFormul[] dizi)
        {
            List<object> l = new List<object>();
            if (dizi != null)
                foreach (DegiskenFormul d in dizi) l.Add(Satir(d));
            return l;
        }

        private static List<object> SablonListesi(Kullanici kullanan, IHRCServis servisHRC)
        {
            List<object> l = new List<object>();
            ObjectArray oa = servisHRC.BordroSablonListele(kullanan, 0, 0, ENUMUCDURUM.HEPSI, ENUMUCDURUM.HEPSI);
            if (oa.sonuc.islemSonuc)
                foreach (BordroSablonDosya s in oa.objeler)
                    l.Add(new { s.bordroTur, s.islemTur, s.sablonKod, s.sablonAd, s.siraNo, s.tekKisilik, s.gecerliKodlar, s.sayfa, s.ortakRapor });
            return l;
        }

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? bordroTur, int? islemTur)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            try
            {
                IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
                int bTur = bordroTur ?? 0, iTur = islemTur ?? 0;
                BordroYapi by = servisHRC.BordroYapiOku(kullanan, bTur, iTur, true);
                if (by == null) by = new BordroYapi();
                // Eski sayfadaki gibi: değişkenler yoksa iki tablo da boş gelir.
                return Results.Json(new
                {
                    basarili = true,
                    bordroTur = bTur,
                    islemTur = iTur,
                    ad = by.bordroAd,
                    kaynak = by.kaynakBordroKod,
                    degiskenler = by.degiskenler == null ? new List<object>() : Satirlar(by.degiskenler),
                    formuller = by.degiskenler == null ? new List<object>() : Satirlar(by.formuller),
                    sablonlar = SablonListesi(kullanan, servisHRC),
                    ortakFonksiyonlar = HRCServisI.PSLDinBordroHesaplayici.OrtakFonksiyonlarOku(kullanan)
                });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        // ------------------------------------------------------------------ sakla (eski btnSakla_Click)

        /// <summary>
        /// Eski JSON.Deserialize(DegiskenFormul[]) gibi: alanlar satırdaki sırayla atanır (ad önce; genel değişkense tipi oradan gelir,
        /// sonra satırdaki veri tipi yazılır).
        /// </summary>
        private static DegiskenFormul[] DegiskenFormulOl(List<BordroDegiskenSatiri> satirlar, bool formul)
        {
            List<DegiskenFormul> l = new List<DegiskenFormul>();
            if (satirlar == null) return l.ToArray();
            foreach (BordroDegiskenSatiri s in satirlar)
            {
                if (s == null) continue;
                DegiskenFormul d = new DegiskenFormul();
                d.ad = s.ad;
                d.tipStr = s.tipStr;
                d.noktaSay = s.noktaSay;
                if (formul) d.formula = s.formula;
                d.borcHesap = s.borcHesap;
                d.alacakHesap = s.alacakHesap;
                d.kisaEtiket = s.kisaEtiket;
                d.uzunEtiket = s.uzunEtiket;
                d.siraNo = s.siraNo;
                d.genislik = s.genislik;
                if (!formul) d.ilkDeger = s.ilkDeger;
                l.Add(d);
            }
            return l.ToArray();
        }

        private static async Task<IResult> Sakla(HttpContext baglam, IWebHostEnvironment ortam, BordroSaklaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            BordroYapi by = new BordroYapi();
            by.bordroAd = istek.ad;
            by.kaynakBordroKod = istek.kaynak;
            by.bordroTur = istek.bordroTur;
            by.bordroIslemTur = istek.islemTur;

            if (by.bordroTur != istek.okunanBordroTur || by.bordroIslemTur != istek.okunanIslemTur)
                return Hata("Ekranda görüntülenen bordro ile kaydetmeye çalıştığınız bordro aynı bordro değil, lütfen önce listeleme yapınız!");

            try
            {
                by.degiskenler = DegiskenFormulOl(istek.degiskenler, false);
                by.formuller = DegiskenFormulOl(istek.formuller, true);
                Sonuc s = TNS.HRC.Arac.Tanimla().BordroYapiSakla(kullanan, by);
                if (!s.islemSonuc) return Hata(s.hataStr);
                // Eski OrtakFonksiyonlarSakla
                TNS.KYM.Arac.Tanimla().BuyukMetinSakla(kullanan, "HRC", "OrtakFonksiyon", "tr", istek.ortakFonksiyonlar ?? "");
                return Results.Json(new { basarili = true, bilgi = "Bordro yapısı saklandı" });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        // ------------------------------------------------------------------ kodu indir, şablon üret

        private static async Task<IResult> KodIndir(HttpContext baglam, IWebHostEnvironment ortam, BordroKodIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            try
            {
                BordroYapi by = TNS.HRC.Arac.Tanimla().BordroYapiOku(kullanan, istek.bordroTur, istek.islemTur, false);
                string hata = "";
                TNS.Calculator c = HRCServisI.PSLDinBordroHesaplayici.BordroDerleyicisiOl(by, out hata, istek.ortakFonksiyonlar ?? "");
                string dosyaAd = DosyaIslem.DosyaAdUret("Kod", "") + ".txt";
                File.WriteAllText(dosyaAd, c.GetSource() + "\r\n" + hata, Encoding.UTF8);
                return DosyaIslem.DosyaGonder(dosyaAd, Path.GetFileName(dosyaAd), true, "txt");
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        private static IResult SablonUret(HttpContext baglam, IWebHostEnvironment ortam, int? bordroTur, int? islemTur)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            try
            {
                BordroYapi by = TNS.HRC.Arac.Tanimla().BordroYapiOku(kullanan, bordroTur ?? 0, islemTur ?? 0, false);

                string dosyaAd = DosyaIslem.DosyaAdUret("Kod", "") + ".xlsx";
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.BosDosyaAc(dosyaAd);
                int satir = 0;

                // Eski sayfadaki sırayla: başlık hücreleri ve adları
                string[,] ustAlanlar =
                {
                    { "Yıl", "Yil" }, { "Ay", "Ay" }, { "Ay Adı", "AyAd" }, { "Say kod", "SayKod" }, { "Say ad", "SayAd" },
                    { "Bir Kod", "BirKod" }, { "Bir Ad", "BirAd" }, { "Say Bir Ad", "SayBirAd" }
                };
                for (int i = 0; i < ustAlanlar.GetLength(0); i++)
                {
                    XLS.HucreDegerYaz(satir, i * 2, ustAlanlar[i, 0]);
                    XLS.HucreAdAdresOl(ustAlanlar[i, 1], 0, satir, i * 2 + 1, satir, i * 2 + 1);
                }

                XLS.HucreDegerYaz(satir, 22, "Dönem İlk Günü");
                XLS.HucreAdAdresOl("DIlkGun", 0, satir, 23, satir, 23);

                XLS.HucreDegerYaz(satir, 24, "Dönem Son Günü");
                XLS.HucreAdAdresOl("DSonGun", 0, satir, 25, satir, 25);

                XLS.HucreDegerYaz(satir, 26, "DYil");
                XLS.HucreAdAdresOl("DYil", 0, satir, 27, satir, 27);

                XLS.HucreDegerYaz(satir, 28, "DAy");
                XLS.HucreAdAdresOl("DAy", 0, satir, 29, satir, 29);

                XLS.HucreDegerYaz(satir, 30, "DAyAd");
                XLS.HucreAdAdresOl("DAyAd", 0, satir, 31, satir, 31);

                XLS.HucreDegerYaz(satir, 32, "SurecYil");
                XLS.HucreAdAdresOl("SurecYil", 0, satir, 33, satir, 33);

                XLS.HucreDegerYaz(satir, 34, "SurecNo");
                XLS.HucreAdAdresOl("SurecNo", 0, satir, 35, satir, 35);

                XLS.HucreDegerYaz(satir, 16, "Sayfadaki Satır");
                XLS.HucreAdAdresOl("SayfaKisi", 0, satir, 17, satir, 17);

                XLS.HucreDegerYaz(satir, 18, "Satır Yük");
                XLS.HucreAdAdresOl("SatirYuk", 0, satir, 19, satir, 19);

                XLS.HucreDegerYaz(satir, 20, "Toplam Aynı Sayfada");
                XLS.HucreAdAdresOl("ToplamAyniSayfada", 0, satir, 21, satir, 21);

                XLS.HucreDegerYaz(satir, 36, "Sureç Header/Footer Yer");
                XLS.HucreAdAdresOl("SurecNoYer", 0, satir, 37, satir, 37);

                XLS.HucreDegerYaz(satir, 38, "Siralama");
                XLS.HucreAdAdresOl("Siralama", 0, satir, 39, satir, 39);

                XLS.HucreDegerYaz(satir, 40, "DoluAlan");
                XLS.HucreAdAdresOl("DoluAlan", 0, satir, 41, satir, 41);

                XLS.HucreDegerYaz(satir, 42, "GrupAlan");
                XLS.HucreAdAdresOl("GrupAlan", 0, satir, 43, satir, 43);

                XLS.HucreDegerYaz(satir, 44, "GrupBaslik");
                XLS.HucreAdAdresOl("GrupBaslik", 0, satir, 45, satir, 45);

                XLS.HucreDegerYaz(satir, 46, "GrupAd");
                XLS.HucreAdAdresOl("GrupAd", 0, satir, 47, satir, 47);

                XLS.HucreDegerYaz(satir, 48, "GrupYuk");
                XLS.HucreAdAdresOl("GrupYuk", 0, satir, 49, satir, 49);

                XLS.HucreDegerYaz(satir, 50, "SatirSil");
                XLS.HucreAdAdresOl("SatirSil", 0, satir, 51, satir, 51);

                XLS.HucreDegerYaz(satir, 52, "SutunSil");
                XLS.HucreAdAdresOl("SutunSil", 0, satir, 53, satir, 53);

                XLS.HucreDegerYaz(satir, 54, "SiraNo");

                satir += 2;
                int alanSay = by.hepsi.Length;
                for (int i = 0; i < alanSay; i++)
                {
                    DegiskenFormul tut = by.hepsi[i];
                    XLS.HucreAdAdresOl("a_" + tut.ad, 0, satir, i, satir, i);
                    XLS.HucreDegerYaz(satir, i, "a_" + tut.ad);

                    if (tut.tip == VariableType.Money)
                    {
                        XLS.HucreAdAdresOl("s_" + tut.ad, 0, satir + 2, i, satir + 2, i);
                        XLS.HucreAdAdresOl("t_" + tut.ad, 0, satir + 4, i, satir + 4, i);
                        XLS.HucreDegerYaz(satir + 2, i, "s_" + tut.ad);
                        XLS.HucreDegerYaz(satir + 4, i, "t_" + tut.ad);
                    }

                    XLS.HucreDegerYaz(satir + 6, i, tut.kisaEtiket);
                    XLS.HucreDegerYaz(satir + 7, i, tut.uzunEtiket);
                }
                XLS.HucreAdAdresOl("BordroSatir", 0, satir, 0, satir + 1, alanSay);
                XLS.HucreAdAdresOl("SayfaToplam", 0, satir + 2, 0, satir + 3, alanSay);
                XLS.HucreAdAdresOl("GenelToplam", 0, satir + 4, 0, satir + 5, alanSay);
                XLS.HucreAdAdresOl("BaslaSatir", 0, satir + 8, 0, satir + 8, 0);

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(dosyaAd, Path.GetFileName(dosyaAd), true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        // ------------------------------------------------------------------ çalıştır (eski btnCalHesapla_Click)

        /// <summary>Eski PSLDinBordroOrtak.HucreyeYaz (toplam almadan): değer veri tipine göre hücreye yazılır.</summary>
        private static void HucreyeYaz(Tablo XLS, int satIc, int sutIc, object o, VariableType vt)
        {
            if (o == null) return;
            try
            {
                if (vt == VariableType.Integer) XLS.HucreDegerYaz(satIc, sutIc, (int)o);
                else if (vt == VariableType.Money) XLS.HucreDegerYaz(satIc, sutIc, (double)o);
                else if (vt == VariableType.Date) XLS.HucreDegerYaz(satIc, sutIc, (DateTime)o);
                else XLS.HucreDegerYaz(satIc, sutIc, (string)o);
            }
            catch (Exception ex)
            {
                XLS.HucreDegerYaz(satIc, sutIc, (o == null ? "NULL" : o.ToString()) + "(" + ex.Message + ")");
            }
        }

        private static async Task<IResult> Hesapla(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            IFormCollection form = await baglam.Request.ReadFormAsync();
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length <= 0) return Hata("Dosya kodu hatalı veya dosya alınamadı!");

            int secBTur = OrtakFonksiyonlar.ConvertToInt((string)form["bordroTur"], 0);
            int secITur = OrtakFonksiyonlar.ConvertToInt((string)form["islemTur"], 0);
            if (!DateTime.TryParse((string)form["donem"], out DateTime donemTarih)) return Hata("Dönem alanı boş bırakılamaz!");
            if (!DateTime.TryParse((string)form["tarih"], out DateTime calTarih)) return Hata("Hazırlama Tarihi alanı boş bırakılamaz!");
            bool isGunu = (string)form["isGunu"] == "1";

            try
            {
                IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
                BordroYapi by = servisHRC.BordroYapiOku(kullanan, secBTur, secITur, false);

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                using (MemoryStream akis = new MemoryStream())
                {
                    await dosya.CopyToAsync(akis);
                    akis.Position = 0;
                    XLS.DosyaOkuAc(akis);
                }
                CalisanDinamikBordro calisanBordro = new CalisanDinamikBordro();
                calisanBordro.kisiDegerler = new ArrayList();
                int satir = 1;
                int kol = 0;
                while (true)
                {
                    string s = XLS.HucreDegerAl(satir, 0);
                    if (string.IsNullOrWhiteSpace(s)) break;

                    ArrayList al = new ArrayList();
                    al.Add(satir.ToString());
                    kol = 0;
                    foreach (DegiskenFormul item in by.hepsi)
                    {
                        object o = null;
                        if (string.IsNullOrWhiteSpace(item.formula))
                        {
                            string kolDeger = XLS.HucreDegerAl(satir, kol++);
                            if (item.tip == VariableType.String) o = kolDeger;
                            else if (item.tip == VariableType.Date) o = new TNSDateTime(kolDeger).Oku();
                            else if (item.tip == VariableType.Money) o = OrtakFonksiyonlar.ConvertToDbl(kolDeger);
                            else o = OrtakFonksiyonlar.ConvertToInt(kolDeger, 0);
                        }
                        al.Add(o);
                    }
                    al.Add(satir);
                    calisanBordro.kisiDegerler.Add(new KisiSatir(al.ToArray()));
                    satir++;
                }
                CalisanDinamikBordroDonem bordroDonem = new CalisanDinamikBordroDonem(0, null);
                bordroDonem.hazirlamaTarih = new TNSDateTime(calTarih);
                bordroDonem.odemeTarih = new TNSDateTime(calTarih);
                bordroDonem.hazirlayan = "";
                bordroDonem.onayaSunan = "";
                bordroDonem.onaylayan = "";
                bordroDonem.bordroDonemBasTarih = new TNSDateTime(donemTarih);

                HRCServisI.PSLDinBordroHesaplayici.BordroVerileriHesaplaGenel(kullanan, servisHRC, by, bordroDonem, calisanBordro, null, isGunu);

                XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("Sonuc", "") + ".xlsx";
                XLS.BosDosyaAc(sonucDosyaAd);

                kol = 0;
                foreach (DegiskenFormul item in by.hepsi)
                    XLS.HucreDegerYaz(0, kol++, item.uzunEtiket);

                for (int i = 0; i < calisanBordro.kisiDegerler.Count; i++)
                {
                    KisiSatir kisiSatir = (KisiSatir)calisanBordro.kisiDegerler[i];
                    kol = 0;
                    foreach (DegiskenFormul df in by.hepsi)
                    {
                        object o = kisiSatir.degerler[kol + 1];
                        HucreyeYaz(XLS, i + 1, kol++, o, df.tip);
                    }
                }
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, Path.GetFileName(sonucDosyaAd), true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        // ------------------------------------------------------------------ ortak şablonlar

        /// <summary>Eski DosyaYol: HarcamaDosyaYol\BordroSablonlar (yaz ise klasör yoksa açılır).</summary>
        private static string DosyaYol(bool yaz)
        {
            string dosyaDizin = Path.Combine(Ayar.Oku("HarcamaDosyaYol", ""), "BordroSablonlar");
            if (yaz && !Directory.Exists(dosyaDizin))
                Directory.CreateDirectory(dosyaDizin);
            return dosyaDizin;
        }

        private static string Kirp(string text)
        {
            if (text == null) return "";
            return text.Trim();
        }

        private static BordroSablonDosya Anahtar(string bordroTur, string islemTur, string sablonKod)
        {
            BordroSablonDosya bsd = new BordroSablonDosya();
            bsd.bordroTur = OrtakFonksiyonlar.ConvertToInt(bordroTur, 0);
            bsd.islemTur = OrtakFonksiyonlar.ConvertToInt(islemTur, 0);
            bsd.sablonKod = OrtakFonksiyonlar.ConvertToInt(sablonKod, -1);
            return bsd;
        }

        private static IResult Sablonlar(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return Results.Json(new { sablonlar = SablonListesi(kullanan, TNS.HRC.Arac.Tanimla()) });
        }

        private static async Task<IResult> SablonKaydet(HttpContext baglam, IWebHostEnvironment ortam, BordroSablonIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            BordroSablonDosya bsd = Anahtar(istek.bordroTur, istek.islemTur, istek.sablonKod);
            bsd.sablonAd = Kirp(istek.sablonAd);
            bsd.gecerliKodlar = Kirp(istek.gecerliKodlar);
            bsd.ortakRapor = istek.ortakRapor;
            bsd.tekKisilik = istek.tekKisilik;
            bsd.sayfa = istek.sayfa;
            bsd.siraNo = istek.siraNo ?? "";

            IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            Sonuc sonuc = servisHRC.BordroSablonSakla(kullanan, bsd, istek.yeni == 1);
            if (!sonuc.islemSonuc) return Hata(sonuc.hataStr);
            return Results.Json(new { basarili = true, sablonlar = SablonListesi(kullanan, servisHRC) });
        }

        private static async Task<IResult> SablonSil(HttpContext baglam, IWebHostEnvironment ortam, BordroSablonIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            BordroSablonDosya bsd = Anahtar(istek.bordroTur, istek.islemTur, istek.sablonKod);
            try
            {
                string dosyaAd = Path.Combine(DosyaYol(true), bsd.SablonDosyaAd);
                if (File.Exists(dosyaAd)) File.Delete(dosyaAd);
            }
            catch { }

            IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            Sonuc sonuc = servisHRC.BordroSablonSil(kullanan, bsd);
            if (!sonuc.islemSonuc) return Hata(sonuc.hataStr);
            return Results.Json(new { basarili = true, sablonlar = SablonListesi(kullanan, servisHRC) });
        }

        private static async Task<IResult> SablonYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            IFormCollection form = await baglam.Request.ReadFormAsync();
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length <= 0) return Hata("Dosya kodu hatalı veya dosya alınamadı!");

            BordroSablonDosya bsd = Anahtar(form["bordroTur"], form["islemTur"], form["sablonKod"]);
            try
            {
                string dosyaAd = Path.Combine(DosyaYol(true), bsd.SablonDosyaAd);
                using (FileStream hedef = File.Create(dosyaAd))
                    await dosya.CopyToAsync(hedef);
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
            return Results.Json(new { basarili = true, bilgi = "Dosya yüklendi" });
        }

        private static IResult SablonAl(HttpContext baglam, IWebHostEnvironment ortam, int? bordroTur, int? islemTur, int? sablonKod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            BordroSablonDosya bsd = new BordroSablonDosya();
            bsd.bordroTur = bordroTur ?? 0;
            bsd.islemTur = islemTur ?? 0;
            bsd.sablonKod = sablonKod ?? 0;
            string dosyaAd = Path.Combine(DosyaYol(false), bsd.SablonDosyaAd);
            if (!File.Exists(dosyaAd)) return Hata("Şablon dosyası bulunamadı. Önce şablon yükleyiniz.");
            return DosyaIslem.DosyaGonder(dosyaAd, bsd.SablonDosyaAd, false, "xlsx");
        }
    }
}
