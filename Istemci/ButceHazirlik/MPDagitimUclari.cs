using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    /// <summary>Dağıtım formunun üst bilgileri (eski MPDagitim.aspx ekrandanAl; alan adları eski JSON'daki gibi).</summary>
    public class MPDagitimForm
    {
        public int id { get; set; }
        public int yil { get; set; }
        public string kur { get; set; }
        public string fon { get; set; }
        public string fin { get; set; }
        public string eko { get; set; }
        public string peb { get; set; }
        public string aciklama { get; set; }
        public double miktar { get; set; }
    }

    public class MPDagitimBirimIstegi
    {
        public int id { get; set; }
        public string birimKod { get; set; }
    }

    public class MPDagitimRevizeIstegi
    {
        public int id { get; set; }
        public int ay { get; set; }
    }

    /// <summary>
    /// Masraf Planı Dağıtım sayfasının (eski ButceHazirlikv2/MPDagitim.aspx; menüde "Masraf Planı Tavan") JavaScript'ten çağırdığı uçlar.
    ///   GET  /api/mpdagitim/kodlar?yil=&amp;kod=&amp;ust=   kod ağacı (eski KodAgac.aspx: 1, 2, 3, 4, 14)
    ///   GET  /api/mpdagitim/kodad?yil=&amp;tur=&amp;kod=    yazılan kodun adı (eski KodAdGetir: 1, 2, 3, 4, 58)
    ///   GET  /api/mpdagitim/liste?yil=              (eski Listele: ListeleMPDagitim)
    ///   GET  /api/mpdagitim/oku?id=                 (eski Oku: OkuMPDagitim, birim satırlarıyla)
    ///   POST /api/mpdagitim/kaydet                  (eski KaydetForm: KaydetMPDagitim)
    ///   GET  /api/mpdagitim/birimler?kriter=        (eski BirimListele: en alt düzey kurumsal yapılar)
    ///   POST /api/mpdagitim/birimekle               (eski BirimEkle: MPDagitimBirimEkle, tutar 0)
    ///   POST /api/mpdagitim/revizeuret              (eski MPRevizeUret: MPDagitimRevizeUret)
    ///   POST /api/mpdagitim/dosyayukle              (eski DosyaYukle: muhasebe / birim / tutar Excel'i, başlıksız)
    /// İş kuralları eskisi gibi TNS.BHMv2 (IBHMv2Servis) servisinden çağrılır.
    /// </summary>
    public static class MPDagitimUclari
    {
        public const string Adres = "../ButceHazirlikv2/MPDagitim.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/mpdagitim/kodlar", Kodlar);
            app.MapGet("/api/mpdagitim/kodad", KodAdi);
            app.MapGet("/api/mpdagitim/liste", Liste);
            app.MapGet("/api/mpdagitim/oku", Oku);
            app.MapPost("/api/mpdagitim/kaydet", Kaydet);
            app.MapGet("/api/mpdagitim/birimler", Birimler);
            app.MapPost("/api/mpdagitim/birimekle", BirimEkle);
            app.MapPost("/api/mpdagitim/revizeuret", RevizeUret);
            app.MapPost("/api/mpdagitim/dosyayukle", DosyaYukle).DisableAntiforgery();
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Adres);
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

        private static string SonucHata(Sonuc s, string varsayilan)
        {
            return s == null ? varsayilan : (string.IsNullOrWhiteSpace(s.hataStr) ? varsayilan : s.hataStr);
        }

        /// <summary>Birim satırı (eski MPDagitimDetay; alan adları aynı).</summary>
        private static object DetaySatir(MPDagitimDetay d)
        {
            return new { d.birimKod, d.birimAd, d.oncYilDeger, d.buYilDeger, d.miktar };
        }

        // ------------------------------------------------------------------ kod ağacı, kod adı

        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kod, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            kod = Temiz(kod);
            if (kod != "1" && kod != "2" && kod != "3" && kod != "4" && kod != "14") return Hata("Kod türü hatalı.");
            return MPTavanUclari.KodAgaci(kullanan, yil, kod, ust);
        }

        private static IResult KodAdi(HttpContext baglam, IWebHostEnvironment ortam, int yil, int tur, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (tur == 53) return Hata("Kod türü hatalı.");
            return MPTavanUclari.KodAdiBul(yil, tur, kod);
        }

        // ------------------------------------------------------------------ liste, oku, kaydet

        /// <summary>Kayıt listesi (eski Listele): yılın dağıtım kayıtları.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl giriniz.");
            try
            {
                ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleMPDagitim(kullanan, new MPDagitimAna() { yil = yil });
                if (!oa.sonuc.islemSonuc) return Hata(SonucHata(oa.sonuc, "Listelenemedi."));
                List<object> satirlar = new List<object>();
                foreach (object o in oa.objeler)
                {
                    if (o is MPDagitimAna a)
                        satirlar.Add(new { a.id, a.yil, a.tertip, a.aciklama, a.miktar });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>Kaydı birim satırlarıyla okur (eski Oku, listede çift tıklama).</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int id)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectVAN ov = TNS.BHMv2.Arac.Tanimla().OkuMPDagitim(kullanan, id);
                if (!ov.sonuc.islemSonuc) return Hata(SonucHata(ov.sonuc, "Kayıt okunamadı."));
                MPDagitimAna a = ov.o as MPDagitimAna;
                if (a == null) return Hata("Kayıt bulunamadı.");
                List<object> detay = new List<object>();
                if (a.detay != null)
                    foreach (object o in a.detay)
                        if (o is MPDagitimDetay d) detay.Add(DetaySatir(d));
                return Results.Json(new
                {
                    a.id, a.yil, a.kur, a.kurAd, a.fon, a.fonAd, a.fin, a.finAd, a.eko, a.ekoAd, a.peb, a.pebAd, a.aciklama, a.miktar,
                    detay = detay
                });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>Kaydet (eski KaydetForm): yeni kayıtta dönen numara formun numarası olur.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPDagitimForm istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MPDagitimForm();
            try
            {
                MPDagitimAna form = new MPDagitimAna
                {
                    id = istek.id,
                    yil = istek.yil,
                    kur = Temiz(istek.kur),
                    fon = Temiz(istek.fon),
                    fin = Temiz(istek.fin),
                    eko = Temiz(istek.eko),
                    peb = Temiz(istek.peb),
                    aciklama = Temiz(istek.aciklama),
                    miktar = istek.miktar
                };
                Sonuc s = TNS.BHMv2.Arac.Tanimla().KaydetMPDagitim(kullanan, form);
                if (s == null || !s.islemSonuc) return Hata(SonucHata(s, "Kaydedilemedi."));
                return Results.Json(new { bilgi = "Kayıt başarılı!", id = OrtakFonksiyonlar.ConvertToInt(s.anahtar, istek.id) });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        // ------------------------------------------------------------------ birimler

        /// <summary>
        /// Birim Seçim penceresi (eski BirimListele): kriter "sayı ad" biçimindeyse sayı 100-999 arasında birim, değilse saymanlık
        /// kodu; geri kalanı ad. Sayı yoksa kriterin tamamı ad olarak aranır. En alt düzeydeki kurumsal yapılar döner.
        /// </summary>
        private static IResult Birimler(HttpContext baglam, IWebHostEnvironment ortam, string kriter)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                kriter = Temiz(kriter);
                string on = "";
                string arka = "";
                KurumsalYapi ky = new KurumsalYapi();
                OrtakFonksiyonlar.StringiIkiyeBol(kriter, " ", ref on, ref arka);
                int onInt = OrtakFonksiyonlar.ConvertToInt(on, 0);
                if (onInt > 0)
                {
                    if (onInt >= 100 && onInt < 1000)
                        ky.birimKod = on;
                    else
                        ky.saymanlikKod = onInt;
                    ky.ad = arka;
                }
                else
                    ky.ad = kriter;

                KurumsalYapi[] kyler = TNS.BHMv2.Arac.Tanimla().KurumsalYapiListele(kullanan, ky, false, true);
                List<object> satirlar = new List<object>();
                if (kyler != null)
                    foreach (KurumsalYapi k in kyler)
                        satirlar.Add(new { k.kod, k.ad });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>Birim Ekle (eski BirimEkle): seçilen kurumsal yapı kayda 0 tutarla eklenir; eklenen satır döner.</summary>
        private static async Task<IResult> BirimEkle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPDagitimBirimIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MPDagitimBirimIstegi();
            if (istek.id <= 0) return Hata("Önce formu saklayınız!");
            try
            {
                ObjectVAN ov = TNS.BHMv2.Arac.Tanimla().MPDagitimBirimEkle(kullanan, istek.id, Temiz(istek.birimKod), 0);
                if (!ov.sonuc.islemSonuc) return Hata(SonucHata(ov.sonuc, "Birim eklenemedi."));
                return Results.Json(new { satir = ov.o is MPDagitimDetay d ? DetaySatir(d) : null });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>Revize Üret (eski MPRevizeUret): kayıttaki birim tutarlarından seçilen ay için masraf planı revizesi üretilir.</summary>
        private static async Task<IResult> RevizeUret(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPDagitimRevizeIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MPDagitimRevizeIstegi();
            if (istek.id <= 0) return Hata("Önce formu saklayınız!");
            if (istek.ay < 1 || istek.ay > 12) return Hata("Ay 1 ile 12 arasında olmalıdır.");
            try
            {
                Sonuc s = TNS.BHMv2.Arac.Tanimla().MPDagitimRevizeUret(kullanan, istek.id, istek.ay);
                if (s == null || !s.islemSonuc) return Hata(SonucHata(s, "Revize üretilemedi."));
                return Results.Json(new { bilgi = "Kayıt başarılı!" });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        // ------------------------------------------------------------------ dosya yükle

        /// <summary>
        /// Dosya Yükle (eski DosyaYukle): başlık satırı olmayan Excel; 1. sütun muhasebe, 2. sütun birim, 3. sütun tutar. Muhasebe,
        /// birim ve tutarı dolu ilk satıra kadar okunur; her satır kurumsal yapısı bulunarak tutarıyla kayda eklenir.
        /// </summary>
        private static async Task<IResult> DosyaYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            IFormCollection form = await baglam.Request.ReadFormAsync();
            int id = OrtakFonksiyonlar.ConvertToInt((string)form["id"], 0);
            IFormFile dosya = form.Files["dosya"];
            if (id <= 0) return Hata("Önce formu saklayınız!");
            if (dosya == null || dosya.Length == 0) return Hata("Yüklenecek dosya bulunamadı!");
            if (dosya.Length > 20 * 1024 * 1024) return Hata("Dosya çok büyük (en çok 20 MB).");

            try
            {
                AsposeLisans.Yukle();
                MemoryStream akis = new MemoryStream();
                await dosya.CopyToAsync(akis);
                akis.Position = 0;
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(akis);

                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                int sat = 0;
                int hata = 0;
                string hataMesaj = "";
                while (true)
                {
                    int muhKod = OrtakFonksiyonlar.ConvertToInt(XLS.HucreDegerAl(sat, 0), 0);
                    string brmKod = XLS.HucreDegerAl(sat, 1);
                    if (brmKod != null) brmKod = brmKod.Trim();
                    double tutar = XLS.HucreDegerAlDbl(sat, 2);
                    if (muhKod > 0 && !string.IsNullOrWhiteSpace(brmKod) && tutar > 0)
                    {
                        ObjectVAN oa = servis.MPDagitimBirimEkle(kullanan, id, TNS.BHMv2.Arac.KurumsalKodYapiBul(servis, muhKod, brmKod, ""), tutar);
                        if (!oa.sonuc.islemSonuc)
                        {
                            hataMesaj += $"\n{muhKod}-{brmKod} yüklenemedi:" + oa.sonuc.hataStr;
                            hata++;
                        }
                        sat++;
                    }
                    else
                        break;
                }
                if (hata == 0)
                    return Results.Json(new { bilgi = $"{sat} adet kayıt yüklendi!" });
                return Results.Json(new { bilgi = $"{sat} adet kayıttan {sat - hata} tanesi yüklendi fakat {hata} tanesi yüklenemedi:" + hataMesaj, uyariVar = true });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }
    }
}
