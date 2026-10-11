using System;
using System.Collections.Generic;
using System.Data;
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
using TNS;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceKod
{
    public class KodDilIstegi
    {
        public string Ad { get; set; }
        public string Aciklama { get; set; }
    }

    public class KodIstegi
    {
        /// <summary>Kod türü (değişkenlerdeki KODYAPI/KODn numarası; ör. 21 = organizasyon birim türü).</summary>
        public int Tur { get; set; }
        public int Yil { get; set; }
        public string Kod { get; set; }
        /// <summary>Dil kodu (tr, en ...) -> ad ve açıklama.</summary>
        public Dictionary<string, KodDilIstegi> Diller { get; set; }
        /// <summary>Ek alan adı -> değer.</summary>
        public Dictionary<string, string> Ek { get; set; }
    }

    /// <summary>
    /// Genel Kod Tanımlama sayfasının (eski ButceHazirlikv2/GenelKodTanimla.aspx + GenelKodTanimlaDetay.aspx) JavaScript'ten
    /// çağırdığı uçlar. Aynı sayfa adresteki Tur parametresine göre farklı kod ağaçlarını düzenler (21 Birim Türü, 20 İl-İlçe,
    /// 1-7 bütçe sınıflandırmaları, 14 Alt Faaliyetler ...):
    ///   GET  /api/kod/tanim?tur=&amp;yil=, /liste?tur=&amp;yil=&amp;ust=&amp;filtre=, /oku?tur=&amp;yil=&amp;kod=, /rapor?tur=&amp;yil= (Excel)
    ///   POST /api/kod/kaydet, /sil, /oncekiyil, /yukle (Excel dosyasından kod yükleme; form verisi: tur, yil, dosya)
    /// Yetki eskisi gibi menüden gelir: kullanıcı, menüde "../ButceHazirlikv2/GenelKodTanimla.aspx?Tur=n" seçeneğini
    /// görebiliyorsa o türü düzenleyebilir; menüde olmayan türü yalnızca sistem yöneticisi açabilir.
    /// </summary>
    public static class KodTanimUclari
    {
        // Ek alanların ekrandaki adları (eski ButceHazirlikv2 kaynağındaki BHMGKTD_ad metinleri).
        private static readonly Dictionary<string, string> ekAlanAdlari = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "KISAAD", "Kısa Adı" }, { "MUHASEBE", "Muhasebe Kodu" }, { "NUFUS", "Nüfusu" }, { "PLAKAKOD", "Plaka Kodu" },
            { "MERNISKOD", "Mernis Kodu" }, { "BIRIMKOD", "Birim Kod" }, { "TEHLIKESINIFI", "Tehlike Sınıfı" }, { "BORCTURU", "Borç Türü" },
            { "KALEMTURU", "Tür" }, { "KODLEVEL", "Kod Seviyesi" }, { "OKASKOD", "OKAS Kod" }
        };

        // Kod ağacının başlığı (eski BHMGKT002..008); {0} = yıl.
        private static readonly Dictionary<int, string> turAdlari = new Dictionary<int, string>
        {
            { 1, "Kurumsal Sınıflandırma" }, { 2, "Fonksiyonel Sınıflandırma" }, { 3, "Finansal Sınıflandırma" }, { 4, "Ekonomik Sınıflandırma" },
            { 5, "Gelirin Ekonomik Sınıflandırması" }, { 6, "Finansmanın Ekonomik Sınıflandırması" }, { 7, "Sektör Bilgileri" },
            { 14, "Alt Faaliyetler" }, { 20, "İl-İlçe Bilgileri" }, { 21, "Birim Türleri" }
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/kod/tanim", Tanim);
            app.MapGet("/api/kod/liste", Liste);
            app.MapGet("/api/kod/oku", Oku);
            app.MapGet("/api/kod/rapor", RaporAl);
            app.MapPost("/api/kod/kaydet", Kaydet);
            app.MapPost("/api/kod/sil", Sil);
            app.MapPost("/api/kod/oncekiyil", OncekiYildanKopyala);
            // Dosya yükleme form verisiyle gelir; istek jetonu uç içinde denetlenir.
            app.MapPost("/api/kod/yukle", DosyaYukle).DisableAntiforgery();
        }

        /// <summary>Kullanıcı bu kod türünün sayfasını açabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, int tur, string uygulamaKlasoru)
        {
            if (kullanan == null || tur <= 0) return false;
            try
            {
                if (new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../ButceHazirlikv2/GenelKodTanimla.aspx?Tur=" + tur))
                    return true;
            }
            catch (Exception)
            {
                // Menü okunamadıysa yalnızca sistem yöneticisi girebilir.
            }
            return GenelGorevUclari.YetkiVar(kullanan);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, int tur, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, tur, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        /// <summary>Yıl verilmemişse bütçe hazırlık yılı (eski GenelIslemlerIstemci.HazirlikYiliBul).</summary>
        public static int YilBul(int? yil)
        {
            if (yil.HasValue && yil.Value > 1000) return yil.Value;
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/BUTCEHAZIRLIK"), DateTime.Now.Year + 1);
        }

        private static int Seviye(string kod)
        {
            return string.IsNullOrEmpty(kod) ? 0 : kod.Split('.').Length;
        }

        private static string Kultur(Kullanici kullanan)
        {
            return string.IsNullOrEmpty(kullanan.aktifKultur) ? "tr-TR" : kullanan.aktifKultur;
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Kod türünün yapısı: başlık, kırılım yapısı, yıla bağlı mı, diller ve ek alanlar.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int tur, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;

            int y = YilBul(yil);
            ButceKodYapi bky = TNS.BKD.Arac.Tanimla().ButceKodBilgiBul(y, tur, string.Empty);
            if (bky == null || string.IsNullOrEmpty(bky.kodYapi)) return Results.Json(new { hata = "Bu kod türü tanımlı değil." });

            List<object> diller = new List<object>();
            foreach (string dil in Dil.Desteklenen())
                diller.Add(new { kod = dil, ad = Dil.Ad(dil) });

            List<object> ekAlanlar = new List<object>();
            if (bky.ekAlanlar != null)
                foreach (EkAlan ea in bky.ekAlanlar)
                {
                    ekAlanAdlari.TryGetValue(ea.ad ?? "", out string etiket);
                    ekAlanlar.Add(new
                    {
                        ad = ea.ad,
                        etiket = etiket ?? ea.ad,
                        sayi = ea.enumMaske == ENUMMaskeEkAlan.Number,
                        // Alan, kodun uzunluğu bas (ve varsa bit) seviyesine uyuyorsa gösterilir.
                        bas = ea.BasSeviye ?? "",
                        bit = ea.BitSeviye ?? "",
                        secenekler = string.IsNullOrEmpty(ea.ListeUzayAd) ? null : UzaySecenekleri(kullanan, ea.ListeUzayAd)
                    });
                }

            turAdlari.TryGetValue(tur, out string turAd);
            return Results.Json(new
            {
                tur = tur,
                yil = y,
                yilVar = !bky.yilYok,
                baslik = turAd ?? bky.butceKodAd,
                kodYapi = bky.kodYapi,
                diller = diller,
                ekAlanlar = ekAlanlar
            });
        }

        /// <summary>Seçmeli ek alanın seçenekleri (uzayın ilk iki sütunu: değer ve görünen ad).</summary>
        private static List<object> UzaySecenekleri(Kullanici kullanan, string uzayAd)
        {
            List<object> secenekler = new List<object>();
            try
            {
                DataTable dt = TNS.UZY.Arac.Tanimla().UzayDataTableDondur(kullanan, uzayAd, "", false, "", null);
                if (dt != null && dt.Columns.Count >= 2)
                    foreach (DataRow satir in dt.Rows)
                        secenekler.Add(new { kod = Convert.ToString(satir[0]), ad = Convert.ToString(satir[1]) });
            }
            catch (Exception)
            {
                // Liste okunamazsa alan boş seçeneklerle gelir; kayıtlı değer yine de korunur (betik seçenek olarak ekler).
            }
            return secenekler;
        }

        /// <summary>Ağacın bir düzeyi: ust boşsa birinci seviye kodlar, değilse o kodun bir alt seviyesi (eski NodeLoad).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int tur, int? yil, string ust, string filtre)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;

            int y = YilBul(yil);
            ust = (ust ?? "").Trim();
            filtre = (filtre ?? "").Trim();
            int seviye = Seviye(ust) + 1;
            string kultur = Kultur(kullanan);

            IBKDServis servis = TNS.BKD.Arac.Tanimla();
            ButceKodYapi bky = servis.ButceKodBilgiBul(y, tur, string.Empty);
            int kirilim = bky == null ? 0 : bky.bolumSay;

            ABSKodlarVAN kodlar = servis.KodListeleSeviye(y, tur.ToString(), ust, seviye, true, false, filtre, "", kultur);
            if (!kodlar.sonuc.islemSonuc) return Results.Json(new { hata = "Kodlar okunamadı." });

            // Alt kodu olanlar: bir alt seviyedeki kodların üstleri (açma ikonu yalnızca bunlarda çıkar).
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

            List<object> satirlar = new List<object>();
            foreach (ABSKod k in kodlar.absKodlar)
                satirlar.Add(new { kod = k.kod, ad = k.kod + " - " + k.AdKultur(kultur), altVar = altiOlanlar.Contains(k.kod) });
            return Results.Json(new { satirlar = satirlar });
        }

        /// <summary>Bir kodun bütün dillerdeki adı, açıklaması ve ek alan değerleri.</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int tur, int? yil, string kod)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            kod = (kod ?? "").Trim();
            if (kod == "") return Results.BadRequest();

            ABSKodlarVAN kodlar = TNS.BKD.Arac.Tanimla().KodListeleSeviye(YilBul(yil), tur.ToString(), kod, Seviye(kod), true);
            if (!kodlar.sonuc.islemSonuc) return Results.Json(new { hata = "Kod okunamadı." });

            ABSKod k = null;
            foreach (ABSKod aday in kodlar.absKodlar)
                if (aday.kod == kod) { k = aday; break; }
            if (k == null) return Results.Json(new { hata = "Kod bulunamadı." });

            Dictionary<string, object> diller = new Dictionary<string, object>();
            foreach (string dil in Dil.Desteklenen())
                diller[dil] = new { ad = k.Ad(dil) ?? "", aciklama = k.Aciklama(dil) ?? "" };

            Dictionary<string, string> ek = new Dictionary<string, string>();
            if (k.ekAlanlar != null)
                foreach (EkAlanDeger d in k.ekAlanlar)
                    if (d != null && d.ekAlan != null) ek[d.ekAlan.ad] = d.ToString();

            return Results.Json(new { kod = k.kod, diller = diller, ek = ek });
        }

        // ------------------------------------------------------------------ Excel'e yazdırma ve Excel'den yükleme
        // Eski GenelKodTanimlaDetay.aspx.cs'teki kodun aynısıdır (btnRaporAl_Click, DosyaYukle): Excel dosyası OrtakClass.Tablo
        // (TabloAspose) ile yazılır ve okunur. Farklar: dosyanın tarayıcıya gönderilme / tarayıcıdan alınma biçimi ve yüklenen
        // kodun yeni mi kayıtlı mı olduğunun uzay sorgusu yerine kayıtlı kodlara bakılarak belirlenmesi (bkz. Kaydet).

        // Liste başlığındaki sınıflandırma adı (eski BHMGKTD006..013).
        private static readonly Dictionary<int, string> listeAdlari = new Dictionary<int, string>
        {
            { 1, "KURUMSAL" }, { 2, "FONKSİYONEL" }, { 3, "FİNANSAL" }, { 4, "EKONOMİK" }, { 5, "GELİRİN EKONOMİK" },
            { 6, "FİNANSMANIN EKONOMİK" }, { 7, "SEKTÖR" }, { 14, "PERFORMANS BÜTÇE FONKSİYONEL" }
        };

        /// <summary>Rapor Hazırla: türün bütün kodları Excel'e yazılır. Şablon: RaporSablonYol\BKD\KodListesi.xlt.</summary>
        private static IResult RaporAl(HttpContext baglam, IWebHostEnvironment ortam, int tur, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            return RaporUret(kullanan, tur, YilBul(yil));
        }

        private static IResult RaporUret(Kullanici kullanan, int tur, int yil)
        {
            try
            {
                string kodTur = tur.ToString();
                ABSKodlarVAN kodlar = TNS.BKD.Arac.Tanimla().KodListeleSeviye(yil, kodTur.ToUpper(), "", -1);
                if (!kodlar.sonuc.islemSonuc) return Results.Json(new { hata = "Kodlar okunamadı." });

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;
                int kaynakSatir = 0;
                string raporSablonYol = Ayar.Oku("RaporSablonYol");
                string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                string sablonAd = "KodListesi.xlt";
                XLS.DosyaAc(Path.Combine(raporSablonYol, "BKD", sablonAd), sonucDosyaAd);

                XLS.HucreAdBulYaz("Yil", yil);

                listeAdlari.TryGetValue(tur, out string listeAd);
                XLS.HucreAdBulYaz("ListeAd", string.Format("{0} SINIFLANDIRMA LİSTESİ", listeAd ?? ""));

                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                kaynakSatir = satir - 1;

                string kultur = Kultur(kullanan);
                foreach (ABSKod k in kodlar.absKodlar)
                {
                    XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 5, satir, 0);

                    XLS.HucreDegerYaz(satir, 0, k.kod);
                    XLS.HucreDegerYaz(satir, 1, k.AdKultur(kultur));
                    satir++;
                }
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), "KodListesi_" + DosyaIslem.DosyaAdUretSade(), true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }

        /// <summary>
        /// Dosyadan Yükle: Excel dosyasının ilk sayfasındaki kodlar kaydedilir. Sütunlar: kod, sonra her dil için ad ve açıklama
        /// (diller sistemde tanımlı sırayla), sonra türün ek alanları. Kod sütunu boş olan ilk satırda okuma biter; başlık satırı yoktur.
        /// </summary>
        private static async Task<IResult> DosyaYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IFormCollection form = await baglam.Request.ReadFormAsync();
            int tur = OrtakFonksiyonlar.ConvertToInt(form["tur"].ToString(), 0);
            int yil = YilBul(OrtakFonksiyonlar.ConvertToInt(form["yil"].ToString(), 0));
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;

            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return Hata("Yüklenecek Excel dosyasını seçiniz.");
            if (dosya.Length > 20 * 1024 * 1024) return Hata("Dosya çok büyük (en çok 20 MB).");

            string dosyaAdxls = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul() + "." + GenelIslemler.ExcelTur();
            try
            {
                using (FileStream hedef = File.Create(dosyaAdxls))
                    await dosya.CopyToAsync(hedef);
            }
            catch (Exception)
            {
                return Hata("Dosya sunucuya kaydedilemedi.");
            }

            try
            {
                return DosyadanAktar(kullanan, tur, yil, dosyaAdxls);
            }
            finally
            {
                try { File.Delete(dosyaAdxls); } catch { }
            }
        }

        private static IResult DosyadanAktar(Kullanici kullanan, int kodTur, int yil, string dosyaAdxls)
        {
            IBKDServis servisBKD = TNS.BKD.Arac.Tanimla();
            ButceKodYapi bky = servisBKD.ButceKodBilgiBul(yil, kodTur, string.Empty);
            if (bky == null) return Hata("Bu kod türü tanımlı değil.");

            // Kayıtlı kodlar: dosyadaki kod bunların arasındaysa güncellenir, değilse yeni eklenir.
            HashSet<string> kayitlilar = new HashSet<string>();
            ABSKodlarVAN mevcut = servisBKD.KodListeleSeviye(yil, kodTur.ToString(), "", -1);
            if (!mevcut.sonuc.islemSonuc) return Hata("Kayıtlı kodlar okunamadı.");
            foreach (ABSKod k in mevcut.absKodlar) kayitlilar.Add(k.kod);

            TNSCollection kodlar = new TNSCollection();
            int yeniSay = 0;
            try
            {
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(dosyaAdxls);
                int sat = 0;
                while (true)
                {
                    ABSKod kod = new ABSKod();
                    kod.kod = XLS.HucreDegerAl(sat, 0);
                    if (string.IsNullOrEmpty(kod.kod)) break;

                    for (int i = 0; i < TNS.BKD.Arac.DillerLength; i++)
                    {
                        int dilYer = i * 2 + 1;
                        string dilKod = TNS.BKD.Arac.Diller(i);
                        kod.DilEkle(dilKod, XLS.HucreDegerAl(sat, dilYer), XLS.HucreDegerAl(sat, dilYer + 1));
                    }
                    int ekYer = TNS.BKD.Arac.DillerLength * 2 + 1;
                    if (bky.ekAlanlar != null && bky.ekAlanlar.Length > 0)
                    {
                        EkAlanDeger[] dizi = new EkAlanDeger[bky.ekAlanlar.Length];
                        for (int i = 0; i < bky.ekAlanlar.Length; i++)
                        {
                            dizi[i] = new EkAlanDeger(bky.ekAlanlar[i], XLS.HucreDegerAl(sat, ekYer + i));
                        }
                        kod.ekAlanlar = dizi;
                    }

                    if (kayitlilar.Contains(kod.kod))
                        kod.kayitDurumu = ENUMKayitDurumu.GUNCELLE;
                    else
                    {
                        kod.kayitDurumu = ENUMKayitDurumu.YENI;
                        yeniSay++;
                    }
                    kodlar.Add(kod);
                    sat++;
                }
                XLS.DosyaKapat();
            }
            catch (Exception)
            {
                return Hata("Excel dosyası okunamadı.");
            }
            if (kodlar.Count == 0) return Hata("Dosyada kod bulunamadı. Kodlar ilk sayfanın ilk sütununda, ilk satırdan başlamalıdır.");

            Sonuc sonuc = servisBKD.KodKaydet(kullanan, yil, kodTur.ToString(), kodlar);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kodlar kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = "Aktarım tamamlandı: " + kodlar.Count + " kod (" + yeniSay + " yeni, " + (kodlar.Count - yeniSay) + " güncellenen)." });
        }

        // ------------------------------------------------------------------ yazma

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, KodIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;

            int yil = YilBul(istek.Yil);
            string tur = istek.Tur.ToString();
            string girilen = (istek.Kod ?? "").Trim().Replace(".", "");
            if (girilen == "") return Hata("Kod giriniz.");

            IBKDServis servis = TNS.BKD.Arac.Tanimla();
            ButceKodYapi bky = servis.ButceKodBilgiBul(yil, istek.Tur, string.Empty);
            if (bky == null) return Hata("Bu kod türü tanımlı değil.");

            // Kod, türün kırılım yapısına göre noktalanır (eski KodNoktala).
            string kodStr = OrtakFonksiyonlar.KodNoktaKoy(TNS.DEG.Arac.DegiskenDegerBul(0, "KODYAPI/KOD" + tur + "/KODYAPI"), girilen);

            ABSKod kod = new ABSKod();
            kod.kod = kodStr;
            string ilkAd = "";
            foreach (string dil in Dil.Desteklenen())
            {
                KodDilIstegi d = null;
                if (istek.Diller != null) istek.Diller.TryGetValue(dil, out d);
                string ad = d == null ? "" : (d.Ad ?? "").Trim();
                if (ilkAd == "") ilkAd = ad;
                kod.DilEkle(dil, ad, d == null ? "" : (d.Aciklama ?? "").Trim());
            }
            if (ilkAd == "") return Hata("Ad giriniz.");

            if (bky.ekAlanlar != null)
            {
                EkAlanDeger[] dizi = new EkAlanDeger[bky.ekAlanlar.Length];
                for (int i = 0; i < bky.ekAlanlar.Length; i++)
                {
                    string deger = null;
                    if (istek.Ek != null) istek.Ek.TryGetValue(bky.ekAlanlar[i].ad, out deger);
                    dizi[i] = new EkAlanDeger(bky.ekAlanlar[i], (deger ?? "").Trim());
                }
                kod.ekAlanlar = dizi;
            }

            // Kod kayıtlıysa güncellenir, değilse yeni eklenir.
            // (Eski sayfa bunu uzay sorgusuyla yapıyordu; uzayı tanımlı olmayan türlerde, ör. 20 İl-İlçe, sorgu hata metni
            // döndürdüğü için her kod kayıtlı sayılıyordu. Burada kodun kendisi okunur.)
            ABSKodlarVAN kayitli = servis.KodListeleSeviye(yil, tur, kodStr, Seviye(kodStr), true);
            if (!kayitli.sonuc.islemSonuc) return Hata("Kod okunamadı.");
            bool yeni = true;
            foreach (ABSKod aday in kayitli.absKodlar)
                if (aday.kod == kodStr) { yeni = false; break; }
            kod.kayitDurumu = yeni ? ENUMKayitDurumu.YENI : ENUMKayitDurumu.GUNCELLE;

            TNSCollection kodlar = new TNSCollection();
            kodlar.Add(kod);
            Sonuc sonuc = servis.KodKaydet(kullanan, yil, tur, kodlar);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Bilgiler kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = "Bilgiler başarıyla saklandı.", kod = kodStr, yeni = yeni });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, KodIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;

            string kodStr = (istek.Kod ?? "").Trim();
            if (kodStr == "") return Hata("Silinecek kodu seçiniz.");

            ABSKod kod = new ABSKod();
            kod.kod = kodStr;
            kod.kayitDurumu = ENUMKayitDurumu.SIL;
            TNSCollection kodlar = new TNSCollection();
            kodlar.Add(kod);

            Sonuc sonuc = TNS.BKD.Arac.Tanimla().KodKaydet(kullanan, YilBul(istek.Yil), istek.Tur.ToString(), kodlar);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Bilgiler silinemedi."));
            return Results.Json(new { basarili = true, bilgi = "Bilgiler silindi." });
        }

        /// <summary>Bir önceki yılın kodlarını verilen yıla kopyalar (yıla bağlı kod türlerinde).</summary>
        private static async Task<IResult> OncekiYildanKopyala(HttpContext baglam, IWebHostEnvironment ortam, KodIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;

            int yil = YilBul(istek.Yil);
            Sonuc sonuc = TNS.BKD.Arac.Tanimla().OncekiYildanKopyala(kullanan, yil, istek.Tur.ToString());
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kodlar kopyalanamadı."));
            return Results.Json(new { basarili = true, bilgi = "Bilgiler " + yil + " yılına başarıyla aktarıldı." });
        }

        // ------------------------------------------------------------------ yardımcılar

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = sonuc.hataStr ?? "";
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata;
        }
    }
}
