using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OrtakClass;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using TNS;
using TNS.BHMv2;
using TNS.KYM;
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    /// <summary>Alt sayfaların POST isteklerinde ortak olan ölçüt (sayfa adresindeki yil, senaryoKod, projeKodu, altProjeKodu).</summary>
    public class YatAltIstek
    {
        public int Yil { get; set; }
        public string SenaryoKod { get; set; }
        public int ProjeKodu { get; set; }
        public int AltProjeKodu { get; set; }
    }

    public class YatYetkiKullanici
    {
        public string KullaniciKodu { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Eposta { get; set; }
        public string Telefon { get; set; }
    }

    public class YatYetkiKayitIstegi : YatAltIstek
    {
        public List<YatYetkiKullanici> Kullanicilar { get; set; }
    }

    public class YatDosyaSilIstegi : YatAltIstek
    {
        public string FizikiDosyaAd { get; set; }
    }

    public class YatProgramSatiri
    {
        public string Yer { get; set; }
        public string Birimi { get; set; }
        public int SaymanlikKod { get; set; }
        public string BirimKod { get; set; }
        public double Odenek { get; set; }
        public string Aciklama { get; set; }
    }

    public class YatProgramKayitIstegi : YatAltIstek
    {
        public List<YatProgramSatiri> Satirlar { get; set; }
    }

    /// <summary>
    /// Yatırım Projeleri'nin (ProjeGiris) ProjeDetay / AltProjeDetay sekmelerinde iframe olarak açılan dört alt sayfanın uçları.
    /// Sayfalar adresten ?yil=&amp;senaryoKod=&amp;projeKodu=&amp;altProjeKodu= alır (proje düzeyinde altProjeKodu yok ya da 0) ve
    /// bunları her isteğe aynen geçirir. Yetki üst sayfa ProjeGiris'ten gelir (YatProjeOrtak); proje / alt proje üzerindeki
    /// görme ve yazma hakkı iş kurallarında denetlenir.
    ///   Yolluk planı (eski AltProjeYollukPlan.aspx):
    ///     GET  /api/yatalt/yolluk/liste, /api/yatalt/yolluk/rapor (Excel; YollukPlanlamaRaporu, eski 2003-14)
    ///   Proje yetkili kullanıcıları (eski YATYetkiGirisForm.aspx):
    ///     GET  /api/yatalt/yetki/liste, /api/yatalt/yetki/kisi-ara?ad=&amp;soyad=&amp;kod=
    ///     POST /api/yatalt/yetki/kaydet
    ///   Proje dosyaları (eski ProjeDosyaEkle.aspx):
    ///     GET  /api/yatalt/dosya/liste, /api/yatalt/dosya/indir?...&amp;fizikiDosyaAd=
    ///     POST /api/yatalt/dosya/yukle (multipart: yil, senaryoKod, projeKodu, altProjeKodu, aciklama, siraNo, dosya), /api/yatalt/dosya/sil
    ///   Alt proje detay programı (eski AltProjeDetayProgramiForm.aspx):
    ///     GET  /api/yatalt/program/liste[?oncekiAsama=1], /api/yatalt/program/rapor (Word; eski 2004-13)
    ///     POST /api/yatalt/program/kaydet
    /// </summary>
    public static class YatAltSayfaUclari
    {
        private const string SayfaYolluk = "AltProjeYollukPlan";
        private const string SayfaYetki = "YATYetkiGirisForm";
        private const string SayfaDosya = "ProjeDosyaEkle";
        private const string SayfaProgram = "AltProjeDetayProgramiForm";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/yatalt/yolluk/liste", YollukListe);
            app.MapGet("/api/yatalt/yolluk/rapor", YollukRapor);

            app.MapGet("/api/yatalt/yetki/liste", YetkiListe);
            app.MapGet("/api/yatalt/yetki/kisi-ara", YetkiKisiAra);
            app.MapPost("/api/yatalt/yetki/kaydet", YetkiKaydet);

            app.MapGet("/api/yatalt/dosya/liste", DosyaListe);
            app.MapGet("/api/yatalt/dosya/indir", DosyaIndir);
            app.MapPost("/api/yatalt/dosya/yukle", DosyaYukle).DisableAntiforgery();
            app.MapPost("/api/yatalt/dosya/sil", DosyaSil);

            app.MapGet("/api/yatalt/program/liste", ProgramListe);
            app.MapPost("/api/yatalt/program/kaydet", ProgramKaydet);
            app.MapGet("/api/yatalt/program/rapor", ProgramRapor);
        }

        // ------------------------------------------------------------------ ortak

        /// <summary>Adresteki ölçüt (eski GenelIslemlerIstemci.RequestBul: yoksa 0 / boş).</summary>
        private static YatAltIstek Olcut(HttpContext baglam)
        {
            IQueryCollection q = baglam.Request.Query;
            return new YatAltIstek
            {
                Yil = OrtakFonksiyonlar.ConvertToInt(q["yil"].ToString(), 0),
                SenaryoKod = YatProjeOrtak.Temiz(q["senaryoKod"]),
                ProjeKodu = OrtakFonksiyonlar.ConvertToInt(q["projeKodu"].ToString(), 0),
                AltProjeKodu = OrtakFonksiyonlar.ConvertToInt(q["altProjeKodu"].ToString(), 0)
            };
        }

        /// <summary>Ölçüt eksikse kullanıcıya gösterilecek metin; uygunsa null.</summary>
        private static string OlcutHatasi(YatAltIstek o, bool senaryoGerekli = true)
        {
            if (o == null || o.Yil < 2000) return "Yıl hatalı!";
            if (senaryoGerekli && string.IsNullOrWhiteSpace(o.SenaryoKod)) return "Aşama seçilmemiş.";
            if (o.ProjeKodu <= 0 && o.AltProjeKodu <= 0) return "Proje seçilmemiş.";
            return null;
        }

        private static IResult Hata(string hata)
        {
            return YatProjeOrtak.Hata(hata);
        }

        private static IResult Basarisiz(string hata)
        {
            return Results.Json(new { basarili = false, hata = hata });
        }

        private static IResult Basarili(string bilgi)
        {
            return Results.Json(new { basarili = true, bilgi = bilgi });
        }

        // ------------------------------------------------------------------ yolluk planı (AltProjeYollukPlan)

        /// <summary>Projenin / alt projenin yolluk planı (eski Listele): bağlı faaliyet kodlarının planları.</summary>
        private static IResult YollukListe(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaYolluk, out Kullanici kullanan);
            if (ret != null) return ret;
            YatAltIstek o = Olcut(baglam);
            string hata = OlcutHatasi(o);
            if (hata != null) return Hata(hata);
            try
            {
                string pebKodlar = global::ButceHazirlikv2.YollukPlanlamaRaporu.PebKodlariBul(kullanan, TNS.BHMv2.Arac.Tanimla(), o.Yil, o.SenaryoKod, o.ProjeKodu, o.AltProjeKodu);
                List<object> satirlar = new List<object>();
                if (!string.IsNullOrWhiteSpace(pebKodlar))
                {
                    ObjectArray oa = TNS.YAT.Arac.Tanimla().ListeleYollukPlanlama(kullanan, o.Yil, o.SenaryoKod, null, pebKodlar);
                    if (oa == null) return Hata("Yolluk planı okunamadı.");
                    if (!oa.sonuc.islemSonuc) return Hata(YatProjeOrtak.HataMetni(oa.sonuc.hataStr, "Yolluk planı okunamadı."));
                    foreach (YollukPlan yp in oa.objeler)
                        satirlar.Add(new
                        {
                            siraNo = yp.sIRANO, dairesi = yp.dairesi, konusu = yp.konusu, muhtelifMi = yp.muhtelifMi, suresi = yp.suresi,
                            kisiSayisi = yp.kisiSayisi, yapilacagiAy = yp.yapilacagiAy, yeri = yp.yeri, tertip = yp.tertip, maliyet = yp.mALIYET
                        });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Yolluk planı okunamadı.");
            }
        }

        /// <summary>
        /// Yazdır (eski btnYazdir_Click → TekRaporAl(2003, 14)): aynı rapor (YollukPlanlamaRaporu.YollukPlani), projenin / alt
        /// projenin faaliyet kodlarıyla. Eski rapor da faaliyet kodu boşken kodları proje / alt projeden böyle buluyordu.
        /// </summary>
        private static IResult YollukRapor(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaYolluk, out Kullanici kullanan);
            if (ret != null) return ret;
            YatAltIstek o = Olcut(baglam);
            string hata = OlcutHatasi(o);
            if (hata != null) return Hata(hata);
            try
            {
                string pebKodlar = global::ButceHazirlikv2.YollukPlanlamaRaporu.PebKodlariBul(kullanan, TNS.BHMv2.Arac.Tanimla(), o.Yil, o.SenaryoKod, o.ProjeKodu, o.AltProjeKodu);
                // Kod boş verilirse rapor bütün projelere (-1) bakar; projeye bağlı faaliyet yoksa yazdırılacak plan da yoktur.
                if (string.IsNullOrWhiteSpace(pebKodlar)) return Hata("Projeye bağlı faaliyet bulunamadı; yazdırılacak yolluk planı yok.");
                AsposeLisans.Yukle();
                string dosya = global::ButceHazirlikv2.YollukPlanlamaRaporu.YollukPlani(kullanan, o.Yil, o.SenaryoKod, pebKodlar, Ayar.Oku("RaporSablonYol"));
                return DosyaIslem.DosyaGonder(dosya, "YollukPlanlama_" + o.Yil, true, "xlsx");
            }
            catch (global::ButceHazirlikv2.RaporHatasi ex)
            {
                return Hata(YatProjeOrtak.HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        // ------------------------------------------------------------------ yetkili kullanıcılar (YATYetkiGirisForm)

        /// <summary>Projenin / alt projenin yetkili kullanıcıları (eski Oku).</summary>
        private static IResult YetkiListe(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaYetki, out Kullanici kullanan);
            if (ret != null) return ret;
            YatAltIstek o = Olcut(baglam);
            if (o.ProjeKodu <= 0 && o.AltProjeKodu <= 0) return Hata("Proje seçilmemiş.");
            try
            {
                ObjectArray oa = TNS.YAT.Arac.Tanimla().ListeleYATYetkiGiris(kullanan, o.ProjeKodu, o.AltProjeKodu);
                if (oa == null) return Hata("Yetkili kullanıcılar okunamadı.");
                if (!oa.sonuc.islemSonuc) return Hata(YatProjeOrtak.HataMetni(oa.sonuc.hataStr, "Yetkili kullanıcılar okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (Kullanici k in oa.objeler)
                    satirlar.Add(new { kullaniciKodu = k.kullaniciKodu, ad = k.ad, soyad = k.soyad, eposta = k.EPOSTA, telefon = k.Telefon });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Yetkili kullanıcılar okunamadı.");
            }
        }

        /// <summary>
        /// Kullanıcı Seç penceresi (eski KYM/ListeKullaniciSec.aspx?cokluSec=1): kullanıcılar arasında arama (en az bir ölçüt;
        /// aktif kullanıcılar, en çok 300).
        /// </summary>
        private static IResult YetkiKisiAra(HttpContext baglam, IWebHostEnvironment ortam, string ad, string soyad, string kod)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaYetki, out Kullanici kullanan);
            if (ret != null) return ret;

            Kullanici kriter = new Kullanici();
            kriter.ad = YatProjeOrtak.Temiz(ad);
            kriter.soyad = YatProjeOrtak.Temiz(soyad);
            kriter.kullaniciKodu = YatProjeOrtak.Temiz(kod);
            if (kriter.ad == "" && kriter.soyad == "" && kriter.kullaniciKodu == "")
                return Hata("Ad, soyad ya da kullanıcı kodundan en az birini yazınız.");
            kriter.aktifMi = ENUMAktiflik.AKTIF;
            try
            {
                ObjectArray<Kullanici> sonuc = TNS.KYM.Arac.Tanimla().KullaniciAra(kriter, 1, 0);
                if (sonuc == null || !sonuc.sonuc.islemSonuc) return Hata("Kullanıcılar okunamadı.");

                List<object> satirlar = new List<object>();
                bool fazla = false;
                foreach (Kullanici k in sonuc.objeler)
                {
                    if (string.IsNullOrWhiteSpace(k.kullaniciKodu)) continue;
                    if (satirlar.Count >= 300) { fazla = true; break; }
                    satirlar.Add(new { kullaniciKodu = k.kullaniciKodu, ad = k.ad, soyad = k.soyad, eposta = k.EPOSTA, telefon = k.Telefon, birim = k.calistigiBirim });
                }
                return Results.Json(new { satirlar = satirlar, fazla = fazla });
            }
            catch (Exception)
            {
                return Hata("Kullanıcılar okunamadı.");
            }
        }

        /// <summary>
        /// Kaydet (eski btnKaydet_Click): projenin yetkili kullanıcıları silinip tablodakiler yazılır. Yazma hakkı ve aynı kullanıcının
        /// iki kez girilmesi iş kurallarında denetlenir.
        /// </summary>
        private static async Task<IResult> YetkiKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatYetkiKayitIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaYetki);
            if (ret != null) return ret;
            if (istek == null || (istek.ProjeKodu <= 0 && istek.AltProjeKodu <= 0)) return Basarisiz("Proje seçilmemiş.");
            try
            {
                TNSCollection kisiler = new TNSCollection();
                foreach (YatYetkiKullanici k in istek.Kullanicilar ?? new List<YatYetkiKullanici>())
                {
                    if (k == null || string.IsNullOrWhiteSpace(k.KullaniciKodu)) continue;
                    Kullanici kul = new Kullanici();
                    kul.kullaniciKodu = k.KullaniciKodu.Trim();
                    kul.ad = YatProjeOrtak.Temiz(k.Ad);
                    kul.soyad = YatProjeOrtak.Temiz(k.Soyad);
                    kul.EPOSTA = YatProjeOrtak.Temiz(k.Eposta);
                    kul.Telefon = YatProjeOrtak.Temiz(k.Telefon);
                    kisiler.Add(kul);
                }
                Sonuc sonuc = TNS.YAT.Arac.Tanimla().KaydetYATYetkiGiris(kullanan, istek.ProjeKodu, istek.AltProjeKodu, kisiler, istek.Yil, YatProjeOrtak.Temiz(istek.SenaryoKod));
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(YatProjeOrtak.HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız."));
                return Basarili("Kayıt işlemi başarıyla gerçekleşti.");
            }
            catch (Exception)
            {
                return Basarisiz("Kayıt işlemi başarısız.");
            }
        }

        // ------------------------------------------------------------------ proje dosyaları (ProjeDosyaEkle)

        /// <summary>Dosyaların saklandığı klasör (eski DosyaYol): appSettings YATDosyaYol + "proje-altproje". Tanımsızsa null.</summary>
        private static string DosyaKlasoru(int projeKodu, int altProjeKodu)
        {
            string dizin = Ayar.Oku("YATDosyaYol", "");
            if (string.IsNullOrWhiteSpace(dizin)) return null;
            return Path.Combine(dizin, string.Format("{0}-{1}", projeKodu, altProjeKodu));
        }

        /// <summary>Tür sütunu (eski ProjeEkliDosya.dosyaTur simgesinin karşılığı).</summary>
        private static string DosyaTuru(string dosyaAd)
        {
            string uz = Path.GetExtension(dosyaAd ?? "").TrimStart('.').ToLowerInvariant();
            if (uz == "xls" || uz == "xlsx") return "XLS";
            if (uz == "doc" || uz == "docx") return "DOC";
            if (uz == "ppt" || uz == "pptx") return "PPT";
            if (uz == "pdf") return "PDF";
            if (uz == "zip" || uz == "rar") return "ZIP";
            return uz.ToUpperInvariant();
        }

        private static ObjectArray DosyalariOku(Kullanici kullanan, YatAltIstek o)
        {
            ProjeEkliDosya form = new ProjeEkliDosya();
            form.projeKodu = o.ProjeKodu;
            form.altProjeKodu = o.AltProjeKodu;
            return TNS.YAT.Arac.Tanimla().ProjeDosyalariListele(kullanan, form, o.Yil, o.SenaryoKod);
        }

        /// <summary>
        /// İstenen fiziki dosya adı geçerli mi (yalnızca dosya adı; klasör, ".." ya da geçersiz karakter yok) ve projenin dosya
        /// listesinde var mı? Varsa o kayıt döner. Liste iş kurallarında görme hakkıyla okunur.
        /// </summary>
        private static ProjeEkliDosya ProjeninDosyasi(Kullanici kullanan, YatAltIstek o, string fizikiDosyaAd, out string hata)
        {
            hata = null;
            fizikiDosyaAd = (fizikiDosyaAd ?? "").Trim();
            if (fizikiDosyaAd == "" || fizikiDosyaAd == "." || fizikiDosyaAd == ".."
                || fizikiDosyaAd != Path.GetFileName(fizikiDosyaAd)
                || fizikiDosyaAd.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
                || fizikiDosyaAd.Contains('/') || fizikiDosyaAd.Contains('\\'))
            {
                hata = "Dosya bulunamadı.";
                return null;
            }
            ObjectArray oa = DosyalariOku(kullanan, o);
            if (oa == null || !oa.sonuc.islemSonuc)
            {
                hata = YatProjeOrtak.HataMetni(oa?.sonuc.hataStr, "Dosyalar okunamadı.");
                return null;
            }
            foreach (ProjeEkliDosya d in oa.objeler)
                if (string.Equals(d.fizikiDosyaAd, fizikiDosyaAd, StringComparison.Ordinal)) return d;
            hata = "Dosya bulunamadı.";
            return null;
        }

        /// <summary>Projeye eklenen dosyalar (eski DosyaIslemleri).</summary>
        private static IResult DosyaListe(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaDosya, out Kullanici kullanan);
            if (ret != null) return ret;
            YatAltIstek o = Olcut(baglam);
            string hata = OlcutHatasi(o, false);
            if (hata != null) return Hata(hata);
            try
            {
                ObjectArray oa = DosyalariOku(kullanan, o);
                if (oa == null) return Hata("Dosyalar okunamadı.");
                if (!oa.sonuc.islemSonuc) return Hata(YatProjeOrtak.HataMetni(oa.sonuc.hataStr, "Dosyalar okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (ProjeEkliDosya d in oa.objeler)
                    satirlar.Add(new { siraNo = d.siraNo, aciklama = d.aciklama, dosyaAd = d.dosyaAd, tur = DosyaTuru(d.dosyaAd), fizikiDosyaAd = d.fizikiDosyaAd });
                return Results.Json(new { satirlar = satirlar, azamiMB = Istemci.EkliDosya.EkliDosyaUclari.AzamiMB });
            }
            catch (Exception)
            {
                return Hata("Dosyalar okunamadı.");
            }
        }

        /// <summary>İndir (eski DosyaIndir): kayıt projenin listesinde olmalı; dosya kayıttaki adıyla indirilir.</summary>
        private static IResult DosyaIndir(HttpContext baglam, IWebHostEnvironment ortam, string fizikiDosyaAd)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaDosya, out Kullanici kullanan);
            if (ret != null) return ret;
            YatAltIstek o = Olcut(baglam);
            string hata = OlcutHatasi(o, false);
            if (hata != null) return Hata(hata);
            try
            {
                ProjeEkliDosya kayit = ProjeninDosyasi(kullanan, o, fizikiDosyaAd, out hata);
                if (kayit == null) return Hata(hata);
                string klasor = DosyaKlasoru(o.ProjeKodu, o.AltProjeKodu);
                if (klasor == null) return Hata("Proje dosyalarının klasörü tanımlı değil.");
                string yol = Path.Combine(klasor, kayit.fizikiDosyaAd);
                if (!File.Exists(yol)) return Hata("Dosya bulunamadı.");

                string ad = Path.GetFileName(string.IsNullOrWhiteSpace(kayit.dosyaAd) ? kayit.fizikiDosyaAd : kayit.dosyaAd);
                if (!new FileExtensionContentTypeProvider().TryGetContentType(ad, out string tur)) tur = "application/octet-stream";
                baglam.Response.Headers["X-Content-Type-Options"] = "nosniff";
                return Results.File(File.ReadAllBytes(yol), tur, ad);
            }
            catch (Exception)
            {
                return Hata("Dosya indirilemedi.");
            }
        }

        /// <summary>
        /// Yeni Dosya Ekle (eski BelgeKaydet): dosya, üretilen fiziki adla projenin klasörüne yazılır ve kaydı saklanır.
        /// Eskisinden farklı olarak önce dosya yazılır; kayıt saklanamazsa yazılan dosya silinir (kayıtsız dosya kalmasın).
        /// </summary>
        private static async Task<IResult> DosyaYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaDosya);
            if (ret != null) return ret;
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();

            IFormCollection istek = await baglam.Request.ReadFormAsync();
            YatAltIstek o = new YatAltIstek
            {
                Yil = OrtakFonksiyonlar.ConvertToInt(istek["yil"].ToString(), 0),
                SenaryoKod = YatProjeOrtak.Temiz(istek["senaryoKod"]),
                ProjeKodu = OrtakFonksiyonlar.ConvertToInt(istek["projeKodu"].ToString(), 0),
                AltProjeKodu = OrtakFonksiyonlar.ConvertToInt(istek["altProjeKodu"].ToString(), 0)
            };
            string hata = OlcutHatasi(o, false);
            if (hata != null) return Basarisiz(hata);
            IFormFile dosya = istek.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return Basarisiz("Dosya okunamadı!");
            int azamiMB = Istemci.EkliDosya.EkliDosyaUclari.AzamiMB;
            if (dosya.Length > azamiMB * 1024L * 1024L) return Basarisiz("Dosya en çok " + azamiMB + " MB olabilir.");
            string aciklama = YatProjeOrtak.Temiz(istek["aciklama"]);
            if (aciklama == "") return Basarisiz("Açıklama boş olamaz");

            string klasor = DosyaKlasoru(o.ProjeKodu, o.AltProjeKodu);
            if (klasor == null) return Basarisiz("Proje dosyalarının klasörü tanımlı değil.");

            ProjeEkliDosya form = new ProjeEkliDosya();
            form.projeKodu = o.ProjeKodu;
            form.altProjeKodu = o.AltProjeKodu;
            form.fizikiDosyaAd = DosyaIslem.DosyaAdUretSade();
            form.siraNo = YatProjeOrtak.Temiz(istek["siraNo"]);
            form.dosyaAd = Path.GetFileName(dosya.FileName ?? "");
            form.aciklama = aciklama;
            form.yuklemeZamani = new TNSDateTime(DateTime.Now);
            form.kullanicikodu = kullanan.kullaniciKodu;
            if (string.IsNullOrWhiteSpace(form.dosyaAd)) return Basarisiz("Dosya adı boş olamaz");

            string yol = Path.Combine(klasor, form.fizikiDosyaAd);
            try
            {
                Directory.CreateDirectory(klasor);
                using (FileStream akis = new FileStream(yol, FileMode.CreateNew))
                    await dosya.CopyToAsync(akis);
            }
            catch (Exception)
            {
                return Basarisiz("Dosya saklanamadı.");
            }

            try
            {
                Sonuc sonuc = TNS.YAT.Arac.Tanimla().ProjeDosyasiSakla(kullanan, form, o.Yil, o.SenaryoKod);
                if (sonuc != null && sonuc.islemSonuc) return Basarili("Proje dosyası eklendi");
                try { File.Delete(yol); } catch { }
                return Basarisiz(YatProjeOrtak.HataMetni(sonuc?.hataStr, "Dosya eklenemedi."));
            }
            catch (Exception)
            {
                try { File.Delete(yol); } catch { }
                return Basarisiz("Dosya eklenemedi.");
            }
        }

        /// <summary>Sil (eski BelgeSil): kayıt projenin listesinde olmalı; kaydı silinince dosyası da silinir.</summary>
        private static async Task<IResult> DosyaSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatDosyaSilIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaDosya);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            istek.SenaryoKod = YatProjeOrtak.Temiz(istek.SenaryoKod);
            string hata = OlcutHatasi(istek, false);
            if (hata != null) return Basarisiz(hata);
            try
            {
                ProjeEkliDosya kayit = ProjeninDosyasi(kullanan, istek, istek.FizikiDosyaAd, out hata);
                if (kayit == null) return Basarisiz(hata);

                ProjeEkliDosya form = new ProjeEkliDosya();
                form.projeKodu = istek.ProjeKodu;
                form.altProjeKodu = istek.AltProjeKodu;
                form.fizikiDosyaAd = kayit.fizikiDosyaAd;
                Sonuc sonuc = TNS.YAT.Arac.Tanimla().ProjeDosyasiSil(kullanan, form, istek.Yil, istek.SenaryoKod);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(YatProjeOrtak.HataMetni(sonuc?.hataStr, "Dosya silinemedi."));

                string klasor = DosyaKlasoru(istek.ProjeKodu, istek.AltProjeKodu);
                if (klasor != null)
                {
                    try
                    {
                        string yol = Path.Combine(klasor, kayit.fizikiDosyaAd);
                        if (File.Exists(yol)) File.Delete(yol);
                    }
                    catch (Exception)
                    {
                        return Basarili("Kayıt silindi; ancak dosya sunucudan silinemedi.");
                    }
                }
                return Basarili("Dosya silindi.");
            }
            catch (Exception)
            {
                return Basarisiz("Dosya silinemedi.");
            }
        }

        // ------------------------------------------------------------------ alt proje detay programı (AltProjeDetayProgramiForm)

        /// <summary>
        /// Detay program (eski Oku). oncekiAsama=1 ise eski Oku(true): bir önceki aşamanın verisi (aşama 00 ise önceki yılın 06.00
        /// aşaması, değilse bir önceki aşama "nn.00"). Kayıt yoksa sayfa boş bir satır açar.
        /// </summary>
        private static IResult ProgramListe(HttpContext baglam, IWebHostEnvironment ortam, string oncekiAsama)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaProgram, out Kullanici kullanan);
            if (ret != null) return ret;
            YatAltIstek o = Olcut(baglam);
            string hata = OlcutHatasi(o);
            if (hata != null) return Hata(hata);
            try
            {
                AltProjeDetayProgrami form = new AltProjeDetayProgrami();
                form.yil = o.Yil;
                form.senaryoKod = o.SenaryoKod;
                if (oncekiAsama == "1")
                {
                    int asama = o.SenaryoKod.Length >= 2 ? OrtakFonksiyonlar.ConvertToInt(o.SenaryoKod.Substring(0, 2), 0) : 0;
                    if (asama == 0)
                    {
                        form.yil = o.Yil - 1;
                        form.senaryoKod = "06.00";
                    }
                    else
                    {
                        asama--;
                        form.senaryoKod = asama.ToString("00") + ".00";
                    }
                }
                form.altProjeNo = o.AltProjeKodu;
                ObjectArray oa = TNS.YAT.Arac.Tanimla().ListeleAltProjeDetayProgrami(kullanan, form);
                if (oa == null) return Hata("Detay program okunamadı.");
                if (!oa.sonuc.islemSonuc) return Hata(YatProjeOrtak.HataMetni(oa.sonuc.hataStr, "Detay program okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (AltProjeDetayProgrami d in oa.objeler)
                    satirlar.Add(new { yer = d.yer, birimi = d.birimi, saymanlikKod = d.saymanlikKod, birimKod = d.birimKod, odenek = d.odenek, aciklama = d.aciklama });
                return Results.Json(new { satirlar = satirlar, yil = form.yil, senaryoKod = form.senaryoKod });
            }
            catch (Exception)
            {
                return Hata("Detay program okunamadı.");
            }
        }

        /// <summary>Kaydet (eski btnKaydet_Click): satırlar sırayla (sıra no 1'den) alt projenin bu yıl / aşamasına yazılır.</summary>
        private static async Task<IResult> ProgramKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatProgramKayitIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaProgram);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            istek.SenaryoKod = YatProjeOrtak.Temiz(istek.SenaryoKod);
            string hata = OlcutHatasi(istek);
            if (hata != null) return Basarisiz(hata);
            try
            {
                TNSCollection formlar = new TNSCollection();
                int i = 0;
                foreach (YatProgramSatiri s in istek.Satirlar ?? new List<YatProgramSatiri>())
                {
                    if (s == null) continue;
                    AltProjeDetayProgrami d = new AltProjeDetayProgrami();
                    d.yil = istek.Yil;
                    d.senaryoKod = istek.SenaryoKod;
                    d.altProjeNo = istek.AltProjeKodu;
                    d.siraNo = ++i;
                    d.yer = YatProjeOrtak.Temiz(s.Yer);
                    d.birimi = YatProjeOrtak.Temiz(s.Birimi);
                    d.saymanlikKod = s.SaymanlikKod;
                    d.birimKod = YatProjeOrtak.Temiz(s.BirimKod);
                    d.odenek = s.Odenek;
                    d.aciklama = YatProjeOrtak.Temiz(s.Aciklama);
                    formlar.Add(d);
                }
                Sonuc sonuc = TNS.YAT.Arac.Tanimla().KaydetAltProjeDetayProgrami(kullanan, formlar);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(YatProjeOrtak.HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız."));
                return Basarili("Kayıt işlemi başarıyla gerçekleşti.");
            }
            catch (Exception)
            {
                return Basarisiz("Kayıt işlemi başarısız.");
            }
        }

        /// <summary>Yazdır (eski btnYazdir_Click → TekRaporAl(2004, 13)): rapor parametreleri eskisinin aynısı, Word.</summary>
        private static IResult ProgramRapor(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaProgram, out Kullanici kullanan);
            if (ret != null) return ret;
            YatAltIstek o = Olcut(baglam);
            string hata = OlcutHatasi(o);
            if (hata != null) return Hata(hata);
            try
            {
                Hashtable raporParam = new Hashtable();
                raporParam["Senaryo"] = o.SenaryoKod;
                raporParam["ButceYil"] = o.Yil.ToString();
                raporParam["KaynakNo"] = "";
                raporParam["ProjeNo"] = "-";
                raporParam["AltProjeNo"] = o.AltProjeKodu.ToString();
                raporParam["Format"] = "docx";

                AsposeLisans.Yukle();
                string dosya = global::ButceHazirlikv2.YatProjeRaporlari.Rapor2004(kullanan, 13, raporParam, Ayar.Oku("RaporSablonYol"));
                string uzanti = Path.GetExtension(dosya).TrimStart('.');
                return DosyaIslem.DosyaGonder(dosya, "AltProjeDetayProgrami_" + o.AltProjeKodu, true, uzanti == "" ? "docx" : uzanti);
            }
            catch (global::ButceHazirlikv2.RaporHatasi ex)
            {
                return Hata(YatProjeOrtak.HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
