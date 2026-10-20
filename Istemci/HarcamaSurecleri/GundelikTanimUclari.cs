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
using TNS.BKD;
using TNS.HRC;
using TNS.KYM;

namespace Istemci.HarcamaSurecleri
{
    /// <summary>Bul isteği (eski KriterTopla): yıl, tür, tanım kodu (tam eşleşme), tanım adı (başı).</summary>
    public class GundelikTanimKriter
    {
        public int yil { get; set; }
        public int tur { get; set; }
        public string kod { get; set; }
        public string ad { get; set; }
    }

    /// <summary>Kaydet isteği: formdaki bütün alanlar (eski btnKaydet_Click).</summary>
    public class GundelikTanimKayit
    {
        public int yil { get; set; }
        public int tur { get; set; }
        public string kod { get; set; }
        public string ad { get; set; }
        public int ulkeKod { get; set; }
        public int ayKadroDereceBaslangic { get; set; }
        public int ayKadroDereceBitis { get; set; }
        public int ekGostergeBaslangic { get; set; }
        public int ekGostergeBitis { get; set; }
        public double deger { get; set; }
        public int paraBirimiKod { get; set; }
    }

    /// <summary>Sil isteği (eski btnSil_Click): yıl ve tanım kodu.</summary>
    public class GundelikTanimSilIstek
    {
        public int yil { get; set; }
        public string kod { get; set; }
    }

    /// <summary>Önceki Yıldan Kopyala isteği (eski btnKopyala_Click): hedef yıl ve tür.</summary>
    public class GundelikTanimKopyalaIstek
    {
        public int yil { get; set; }
        public int tur { get; set; }
    }

    /// <summary>
    /// Geçici / Sürekli Görev Gündelik Tanım Formu (eski HarcamaSurecleri/TanimPSLSurekliGeciciGorevGundelik.aspx;
    /// yeni sayfa HarcamaSurecleri/TanimPSLSurekliGeciciGorevGundelik) uçları:
    ///   GET  /api/gundeliktanim/baslangic   yıllar, para birimleri, ülkeler (eski Page_Load doldurmaları)
    ///   POST /api/gundeliktanim/bul         { yil, tur, kod, ad } (eski btnAra_Click / Arama)
    ///   POST /api/gundeliktanim/kaydet      formdaki tanım (eski btnKaydet_Click)
    ///   POST /api/gundeliktanim/sil         { yil, kod } (eski btnSil_Click)
    ///   POST /api/gundeliktanim/kopyala     { yil, tur } önceki yılın tanımlarını bu yıla kopyalar (eski btnKopyala_Click)
    ///   POST /api/gundeliktanim/dosyayukle  form: yil, dosya (Excel) yurtdışı gündeliklerini yükler (eski btnDosyaYukle_Click)
    /// İş kuralları eski sayfadaki gibi IHRCServis.GeciciGundelikTanim* / ParaBirimiListele / UlkeListele.
    /// </summary>
    public static class GundelikTanimUclari
    {
        public const string SayfaAdresi = "../HarcamaSurecleri/TanimPSLSurekliGeciciGorevGundelik.aspx";

        /// <summary>Yıl kutusunun alt sınırı (eski GenelIslemler.YilDoldur(ddlYil, 2005, ...)).</summary>
        private const int IlkYil = 2005;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/gundeliktanim/baslangic", Baslangic);
            app.MapPost("/api/gundeliktanim/bul", Bul);
            app.MapPost("/api/gundeliktanim/kaydet", Kaydet);
            app.MapPost("/api/gundeliktanim/sil", Sil);
            app.MapPost("/api/gundeliktanim/kopyala", Kopyala);
            app.MapPost("/api/gundeliktanim/dosyayukle", DosyaYukle);
        }

        /// <summary>Sayfa yetkisi (eski GenelIslemler.SayfayaGirisYapabilirMi).</summary>
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

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = (sonuc.hataStr ?? "").Replace("<br>", " ").Replace("<br/>", " ");
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata.Trim();
        }

        /// <summary>Eski TurAdiDondur.</summary>
        private static string TurAdi(ENUMYollukGundelikTuru tur)
        {
            return tur == ENUMYollukGundelikTuru.DENETIMELEMANI ? "Denetim Elemanları"
                : tur == ENUMYollukGundelikTuru.SUREKLIGECICI ? "Sürekli - Geçici Görev"
                : tur == ENUMYollukGundelikTuru.GECICIYURTDISI ? "Geçici Görev - Yurtdışı" : "Tanımsız";
        }

        private static ObjectArray ParaBirimleriBul(Kullanici kullanan)
        {
            return TNS.HRC.Arac.Tanimla().ParaBirimiListele(kullanan, new ParaBirimi());
        }

        /// <summary>Eski ParaBirimiKodBul: kısaltma ya da ada göre para birimi kodu; yoksa 0.</summary>
        private static int ParaBirimiKodBul(ObjectArray oa, string aranan)
        {
            foreach (ParaBirimi birim in oa.objeler)
            {
                if (aranan == birim.kisaltma || aranan == birim.adi) return birim.kod;
            }
            return 0;
        }

        private static IResult Baslangic(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            List<int> yillar = new List<int>();
            for (int i = IlkYil; i <= DateTime.Now.Year; i++) yillar.Add(i);

            List<object> paraBirimleri = new List<object>();
            ObjectArray paralar = ParaBirimleriBul(kullanan);
            if (paralar.sonuc.islemSonuc)
                foreach (ParaBirimi birim in paralar.objeler)
                    paraBirimleri.Add(new { birim.kod, birim.kisaltma });

            List<object> ulkeler = new List<object>();
            ObjectArray ulkeListe = TNS.HRC.Arac.Tanimla().UlkeListele(kullanan);
            if (ulkeListe.sonuc.islemSonuc)
                foreach (Ulke u in ulkeListe.objeler)
                    ulkeler.Add(new { u.kod, u.ad });

            return Results.Json(new { yil = DateTime.Now.Year, yillar, paraBirimleri, ulkeler });
        }

        private static async Task<IResult> Bul(HttpContext baglam, IWebHostEnvironment ortam, GundelikTanimKriter istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            YollukGundelikTanim kriter = new YollukGundelikTanim();
            kriter.yil = istek.yil;
            kriter.kod = (istek.kod ?? "").Trim();
            kriter.ad = (istek.ad ?? "").Trim();
            kriter.tur = (ENUMYollukGundelikTuru)istek.tur;
            return Listele(kullanan, kriter);
        }

        /// <summary>Eski Listele: satırlar ızgaraya, bağlantıdaki VeriGoster bilgileri de satırla birlikte gönderilir.</summary>
        private static IResult Listele(Kullanici kullanan, YollukGundelikTanim kriter)
        {
            ObjectArray bilgi = TNS.HRC.Arac.Tanimla().GeciciGundelikTanimListele(kullanan, kriter);
            if (!bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi.sonuc, "Gündelik tanımları okunamadı."));

            List<object> satirlar = new List<object>();
            foreach (YollukGundelikTanim gt in bilgi.objeler)
            {
                satirlar.Add(new
                {
                    gt.yil,
                    gt.kod,
                    gt.ad,
                    gt.deger,
                    tur = (int)gt.tur,
                    turAd = TurAdi(gt.tur),
                    ulkeKod = gt.ulke.kod,
                    ulke = gt.ulke.ad,
                    paraBirimiKod = gt.paraBirimi.kod,
                    paraBirimi = gt.paraBirimi.kisaltma,
                    gt.ayKadroDereceBaslangic,
                    gt.ayKadroDereceBitis,
                    gt.ekGostergeBaslangic,
                    gt.ekGostergeBitis,
                    ayKadroDerece = gt.ayKadroDereceBaslangic + "-" + gt.ayKadroDereceBitis,
                    ekGosterge = gt.ekGostergeBaslangic + "-" + gt.ekGostergeBitis
                });
            }
            return Results.Json(new
            {
                basarili = true,
                satirlar,
                bilgi = satirlar.Count == 0 ? "Verilen kriterlere uygun kayıt bulunamadı." : null
            });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, GundelikTanimKayit istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            string hata = "";
            if (istek.yil <= 0) hata = "Yıl boş bırakılamaz. ";
            if (string.IsNullOrWhiteSpace(istek.kod)) hata += "Gündelik Tanım Kodu boş bırakılamaz. ";
            if (hata != "") return Hata(hata + "Kayıt işlemi yapılamadı.");

            YollukGundelikTanim gundelikTanim = new YollukGundelikTanim();
            gundelikTanim.yil = istek.yil;
            gundelikTanim.kod = istek.kod.Trim();
            gundelikTanim.ad = (istek.ad ?? "").Trim();
            ENUMYollukGundelikTuru tur = (ENUMYollukGundelikTuru)istek.tur;
            gundelikTanim.tur = tur;
            gundelikTanim.ayKadroDereceBaslangic = istek.ayKadroDereceBaslangic;
            gundelikTanim.ayKadroDereceBitis = istek.ayKadroDereceBitis;
            gundelikTanim.ekGostergeBaslangic = istek.ekGostergeBaslangic;
            gundelikTanim.ekGostergeBitis = istek.ekGostergeBitis;
            gundelikTanim.deger = istek.deger;
            gundelikTanim.paraBirimi.kod = istek.paraBirimiKod;

            int ulkeKod = 1; // Türkiye
            if (tur == ENUMYollukGundelikTuru.GECICIYURTDISI)
                ulkeKod = istek.ulkeKod > 0 ? istek.ulkeKod : 1;
            gundelikTanim.ulke.kod = ulkeKod;

            Sonuc sonuc = TNS.HRC.Arac.Tanimla().GeciciGundelikTanimKaydet(kullanan, gundelikTanim, false);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kayıt işlemi yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla tamamlandı." });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, GundelikTanimSilIstek istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            if (istek.yil < 0 || string.IsNullOrWhiteSpace(istek.kod))
                return Hata("Lütfen silmek istediğiniz bilgiyi seçin.");

            YollukGundelikTanim gt = new YollukGundelikTanim();
            gt.yil = istek.yil;
            gt.kod = istek.kod.Trim();

            Sonuc sonuc = TNS.HRC.Arac.Tanimla().GeciciGundelikTanimSil(kullanan, gt);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Silme işlemi yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla tamamlandı." });
        }

        private static async Task<IResult> Kopyala(HttpContext baglam, IWebHostEnvironment ortam, GundelikTanimKopyalaIstek istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            if (istek.yil <= 0) return Hata("Yıl boş bırakılamaz.");

            IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            YollukGundelikTanim gundelikTanim = new YollukGundelikTanim();
            gundelikTanim.yil = istek.yil - 1;
            gundelikTanim.tur = (ENUMYollukGundelikTuru)istek.tur;
            ObjectArray bilgi = servisHRC.GeciciGundelikTanimListele(kullanan, gundelikTanim);
            if (!bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi.sonuc, "Önceki yılın tanımları okunamadı."));

            ObjectArray guncellemeListesi = new ObjectArray();
            for (int i = 0; i < bilgi.ObjeSayisi; i++)
            {
                YollukGundelikTanim ekleObj = (YollukGundelikTanim)bilgi.Al(i);
                ekleObj.yil++;
                guncellemeListesi.Ekle(ekleObj);
            }

            Sonuc sonuc = servisHRC.GeciciGundelikTanimListeKaydet(kullanan, gundelikTanim.yil + 1, gundelikTanim.tur, guncellemeListesi);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kopyalama yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla tamamlandı.", sayi = guncellemeListesi.ObjeSayisi });
        }

        /// <summary>
        /// Dosya Seç (eski btnDosyaYukle_Click): Excel'in ilk satırında A1 başlangıç kodu, C1'den sağa gündelik açıklamaları;
        /// sonraki satırlarda A ülke adı, B para birimi (kısaltma ya da ad), C'den sağa açıklamaların gündelik değerleri.
        /// Ülke ya da para birimi boş ilk satırda durulur. Her hücre yurtdışı türünde ayrı tanım olarak yeni kayıt eklenir.
        /// </summary>
        private static async Task<IResult> DosyaYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            IFormCollection form = await baglam.Request.ReadFormAsync();
            int yil = OrtakFonksiyonlar.ConvertToInt((string)form["yil"], 0);
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return Hata("Dosya seçiniz.");
            string uzanti = (Path.GetExtension(Path.GetFileName(dosya.FileName)) ?? "").ToLowerInvariant();
            if (uzanti != ".xlsx" && uzanti != ".xls") return Hata("Sadece Excel dosyası yüklenebilir.");
            if (dosya.Length > 20 * 1024 * 1024) return Hata("Dosya çok büyük (en çok 20 MB).");

            try
            {
                AsposeLisans.Yukle();
                MemoryStream akis = new MemoryStream();
                await dosya.CopyToAsync(akis);
                akis.Position = 0;
                Tablo xls = GenelIslemler.NewTablo();
                xls.DosyaOkuAc(akis);

                List<string> l = new List<string>();
                int sat = 0;
                int baslaNo = Convert.ToInt32(xls.HucreDegerAlDbl(sat, 0));

                for (int sut = 2; true; sut++)
                {
                    string aciklama = xls.HucreDegerAl(sat, sut);
                    if (string.IsNullOrEmpty(aciklama)) break;
                    l.Add(aciklama);
                }

                IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
                List<YollukGundelikTanim> tc = new List<YollukGundelikTanim>();
                ObjectArray paraBirimleri = ParaBirimleriBul(kullanan);
                string hata = "";
                while (true)
                {
                    sat++;
                    string ulkeAd = xls.HucreDegerAl(sat, 0);
                    string paraBirimi = xls.HucreDegerAl(sat, 1);

                    if (string.IsNullOrEmpty(ulkeAd) || string.IsNullOrEmpty(paraBirimi)) break;
                    int paraBirimiKod = ParaBirimiKodBul(paraBirimleri, paraBirimi);
                    if (paraBirimiKod <= 0) { hata = "Para birimi " + paraBirimi + " bulunamadı!"; break; }

                    for (int i = 0; i < l.Count; i++)
                    {
                        double deger = xls.HucreDegerAlDbl(sat, i + 2);

                        YollukGundelikTanim gundelikTanim = new YollukGundelikTanim();
                        gundelikTanim.yil = yil;
                        gundelikTanim.kod = (baslaNo++).ToString();
                        gundelikTanim.ad = ulkeAd + " (" + l[i] + ")";
                        gundelikTanim.tur = ENUMYollukGundelikTuru.GECICIYURTDISI;
                        gundelikTanim.ayKadroDereceBaslangic = 0;
                        gundelikTanim.ayKadroDereceBitis = 0;
                        gundelikTanim.ekGostergeBaslangic = 0;
                        gundelikTanim.ekGostergeBitis = 0;
                        gundelikTanim.deger = deger;
                        gundelikTanim.paraBirimi.kod = paraBirimiKod;
                        gundelikTanim.ulke.kod = 0;
                        tc.Add(gundelikTanim);
                    }
                }
                if (!string.IsNullOrWhiteSpace(hata)) return Hata(hata);

                foreach (YollukGundelikTanim gundelikTanim in tc)
                {
                    Sonuc sonuc = servisHRC.GeciciGundelikTanimKaydet(kullanan, gundelikTanim, true);
                    if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kayıt işlemi yapılamadı."));
                }
                return Results.Json(new { basarili = true, bilgi = tc.Count + " adet gündelik tanımı yüklendi.", sayi = tc.Count });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }
    }
}
