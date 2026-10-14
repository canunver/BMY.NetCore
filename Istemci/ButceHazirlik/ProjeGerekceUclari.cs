using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ButceHazirlikv2;
using Istemci.ButceKod;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
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
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    public class ProjeGerekceSatiri
    {
        public int Kod { get; set; }
        public string Gerekce { get; set; }
    }

    public class ProjeGerekceKayitIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public string Sektor { get; set; }
        public string Senaryo { get; set; }
        public List<ProjeGerekceSatiri> Satirlar { get; set; }
    }

    public class YeniProjeSatiri
    {
        public string Ad { get; set; }
        public int BitisYil { get; set; }
        public double ProjeTutari { get; set; }
        public double YiliTeklifi0 { get; set; }
        public double YiliTeklifi1 { get; set; }
        public double YiliTeklifi2 { get; set; }
        public string Gerekce { get; set; }
    }

    public class YeniProjeKayitIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public string Sektor { get; set; }
        public List<YeniProjeSatiri> Satirlar { get; set; }
    }

    public class ProjeGerekceOnayIstegi
    {
        public int Yil { get; set; }
        /// <summary>true: onayla, false: onayı kaldır.</summary>
        public bool Onayla { get; set; }
    }

    /// <summary>
    /// Proje Gerekçe Girişi sayfasının (eski ButceHazirlikv2/ProjeGerekceAnaForm.aspx; içindeki ProjeGerekceMevcut.aspx ve
    /// ProjeGerekceYeni.aspx çerçeveleri artık aynı sayfanın sekmeleridir) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/projegerekce/tanim, /sektorler?yil=, /kurumlar?yil=&amp;ust=
    ///   GET  /api/projegerekce/mevcut, /yeni ?yil=&amp;kur=&amp;sektor=&amp;senaryo=
    ///   POST /api/projegerekce/mevcut-kaydet, /yeni-kaydet, /onay
    ///   GET  /api/projegerekce/rapor?tur=1|2|1e|2e|3&amp;yil=&amp;kur=&amp;sektor=&amp;senaryo= (Word / Excel; eski kodla, ProjeGerekceRaporlari)
    /// Aşama seçimi, Ek / Özet raporları ve onaylama yalnızca sistem yöneticisinindir; diğer kullanıcılar için aşama hep 01.00'dır.
    /// Veri girişine kapalı olma (onay) ve birim yazma yetkisi denetimleri iş kurallarındadır.
    /// </summary>
    public static class ProjeGerekceUclari
    {
        private const string VarsayilanSenaryo = "01.00";
        private const string VarsayilanSenaryoAd = "01.00- SGB Çalışması";
        // Kod alanlarına yalnızca rakam ve nokta girer (özet raporu kodları SQL'e yazar).
        private static readonly Regex KodDeseni = new Regex(@"^[0-9.]{0,20}$");

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/projegerekce/tanim", Tanim);
            app.MapGet("/api/projegerekce/sektorler", Sektorler);
            app.MapGet("/api/projegerekce/kurumlar", Kurumlar);
            app.MapGet("/api/projegerekce/mevcut", Mevcut);
            app.MapGet("/api/projegerekce/yeni", Yeni);
            app.MapPost("/api/projegerekce/mevcut-kaydet", MevcutKaydet);
            app.MapPost("/api/projegerekce/yeni-kaydet", YeniKaydet);
            app.MapPost("/api/projegerekce/onay", Onay);
            app.MapGet("/api/projegerekce/rapor", Rapor);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../ButceHazirlikv2/ProjeGerekceAnaForm.aspx");
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

        private static IResult Hata(string metin)
        {
            return Results.Json(new { hata = metin });
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string Temiz(string kod)
        {
            return (kod ?? "").Trim();
        }

        /// <summary>Eski cbSenaryo_Value: aşamayı yalnızca sistem yöneticisi seçer, diğerleri için 01.00.</summary>
        private static string SenaryoBul(Kullanici kullanan, string senaryo)
        {
            senaryo = Temiz(senaryo);
            if (!GenelGorevUclari.YetkiVar(kullanan) || senaryo == "") return VarsayilanSenaryo;
            return senaryo;
        }

        /// <summary>Eski cbSenaryo_Text: raporlara yazılan aşama adı.</summary>
        private static string SenaryoAdBul(Kullanici kullanan, string senaryo)
        {
            if (!GenelGorevUclari.YetkiVar(kullanan)) return VarsayilanSenaryoAd;
            foreach (ButceSenaryo bs in TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null) ?? new ButceSenaryo[0])
                if (bs.kod == senaryo) return bs.kod + "- " + bs.ad;
            return senaryo;
        }

        private static bool KodlarGecerli(params string[] kodlar)
        {
            foreach (string k in kodlar)
                if (!KodDeseni.IsMatch(k ?? "")) return false;
            return true;
        }

        /// <summary>Proje gerekçeleri veri girişine kapalı mı? (iş kurallarındaki denetimin aynısı; sistem yöneticisi her zaman girebilir)</summary>
        private static bool Kapali(Kullanici kullanan, int yil)
        {
            if (GenelGorevUclari.YetkiVar(kullanan)) return false;
            try
            {
                return TNS.BHMv2.Arac.Tanimla().OnayKodOkuTasra(kullanan, yil, 0, "000", "0000", ENUMBirimOnayTur.PROJEGEREKCE) >= 4;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ------------------------------------------------------------------ açılış ve seçenekler

        /// <summary>Sektörlerin son kırılımı, başta boş seçenek (eski YilDegisti: ABSKodDoldur(yil, "7", "", -2, ...)).</summary>
        private static List<object> SektorleriOku(int yil)
        {
            List<object> sektorler = new List<object> { new { kod = "", ad = "" } };
            ABSKodlarVAN sek = TNS.BKD.Arac.Tanimla().KodListeleSeviye(yil, "7", "", -2, true, false, "", "", "", false);
            if (sek != null && sek.absKodlar != null)
                foreach (ABSKod k in sek.absKodlar)
                    sektorler.Add(new { kod = k.kod, ad = k.kodAd });
            return sektorler;
        }

        /// <summary>Açılış (eski Page_Load): bütçe hazırlık yılı, sektörler; sistem yöneticisine aşamalar (varsayılan ikincisi).</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            int yil = KodTanimUclari.YilBul(null);
            bool yonetici = GenelGorevUclari.YetkiVar(kullanan);
            try
            {
                List<object> senaryolar = new List<object>();
                string senaryo = VarsayilanSenaryo;
                if (yonetici)
                {
                    ButceSenaryo[] bsler = TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null) ?? new ButceSenaryo[0];
                    foreach (ButceSenaryo bs in bsler)
                        senaryolar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });
                    if (bsler.Length > 1) senaryo = bsler[1].kod;
                    else if (bsler.Length == 1) senaryo = bsler[0].kod;
                }
                return Results.Json(new { yil = yil, sistemYoneticisi = yonetici, senaryolar = senaryolar, senaryo = senaryo, sektorler = SektorleriOku(yil) });
            }
            catch (Exception)
            {
                return Hata("Seçenekler okunamadı.");
            }
        }

        private static IResult Sektorler(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out _);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                return Results.Json(new { sektorler = SektorleriOku(yil) });
            }
            catch (Exception)
            {
                return Hata("Sektörler okunamadı.");
            }
        }

        /// <summary>Kurumsal kod ağacının bir düzeyi (eski nodeLoadPar3('1', yil, ..., 20)); bütçe hazırlık yetkisiyle süzülür.</summary>
        private static IResult Kurumlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, "1", ust);
        }

        // ------------------------------------------------------------------ Tavan İçi Projeler (eski ProjeGerekceMevcut.aspx)

        /// <summary>Alt projeler; tutarlarının hepsi sıfır olanlar gösterilmez (eski Oku).</summary>
        private static BilgiVAN MevcutOku(Kullanici kullanan, int yil, string kur, string sektor, string senaryo)
        {
            AltProje apSor = new AltProje();
            apSor.birim_kod = kur;
            BilgiVAN form = TNS.YAT.Arac.Tanimla().AltProjeListele(kullanan, yil, senaryo, apSor, sektor, true, true);
            if (form == null || !form.sonuc.islemSonuc || form.liste == null) return form;
            for (int i = form.liste.Count - 1; i >= 0; i--)
            {
                AltProje ap = (AltProje)form.liste[i];
                if (ap.projeTutariHibeli <= 0 && ap.yiliTeklifi <= 0 && ap.yiliTeklifiEk <= 0)
                    form.liste.RemoveAt(i);
            }
            return form;
        }

        private static IResult Mevcut(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string sektor, string senaryo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            kur = Temiz(kur);
            sektor = Temiz(sektor);
            senaryo = SenaryoBul(kullanan, senaryo);

            try
            {
                BilgiVAN form = MevcutOku(kullanan, yil, kur, sektor, senaryo);
                if (form == null) return Hata("Projeler okunamadı.");
                if (!form.sonuc.islemSonuc) return Hata(HataMetni(form.sonuc.hataStr, "Projeler okunamadı."));

                List<object> satirlar = new List<object>();
                foreach (AltProje ap in form.liste)
                {
                    satirlar.Add(new
                    {
                        kod = ap.kod,
                        birimKod = ap.birim.kod,
                        ad = ap.ad,
                        baslangicYil = ap.baslangicYil,
                        bitisYil = ap.bitisYil,
                        projeTutari = ap.projeTutariHibesiz,
                        yiliTeklifi = ap.yiliTeklifiHibesiz,
                        yiliTeklifiEk = ap.yiliTeklifiEk,
                        gerekce = ap.gerekce
                    });
                }
                return Results.Json(new { satirlar = satirlar, kapali = Kapali(kullanan, yil) });
            }
            catch (Exception)
            {
                return Hata("Projeler okunamadı.");
            }
        }

        /// <summary>
        /// Gerekçeleri kaydeder (eski ProjeGerekceMevcut Kaydet). Projelerin birimleri sayfadan alınmaz, aynı ölçütle sunucuda
        /// yeniden okunur; listede olmayan proje yazılmaz. Uzunluk (300), onay ve birim yetkisi denetimleri iş kurallarındadır.
        /// </summary>
        private static async Task<IResult> MevcutKaydet(HttpContext baglam, IWebHostEnvironment ortam, ProjeGerekceKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });

            try
            {
                string senaryo = SenaryoBul(kullanan, istek.Senaryo);
                BilgiVAN form = MevcutOku(kullanan, istek.Yil, Temiz(istek.Kur), Temiz(istek.Sektor), senaryo);
                if (form == null || !form.sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = HataMetni(form?.sonuc.hataStr, "Projeler okunamadı.") });

                Dictionary<int, string> birimler = new Dictionary<int, string>();
                foreach (AltProje ap in form.liste) birimler[ap.kod] = ap.birim.kod;

                List<AltProje> gerekceler = new List<AltProje>();
                foreach (ProjeGerekceSatiri s in istek.Satirlar ?? new List<ProjeGerekceSatiri>())
                {
                    if (s == null || !birimler.ContainsKey(s.Kod)) continue;
                    AltProje ap = new AltProje();
                    ap.kod = s.Kod;
                    ap.birim_kod = birimler[s.Kod];
                    ap.gerekce = s.Gerekce ?? "";
                    gerekceler.Add(ap);
                }
                if (gerekceler.Count == 0) return Results.Json(new { basarili = false, hata = "Kaydedilecek proje yok." });

                Sonuc sonuc = TNS.YAT.Arac.Tanimla().AltProjeGerekceKaydet(kullanan, istek.Yil, senaryo, gerekceler.ToArray());
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        // ------------------------------------------------------------------ Yeni Tavan Dışı Projeler (eski ProjeGerekceYeni.aspx)

        private static IResult Yeni(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string sektor, string senaryo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");

            try
            {
                AltProjeTeklif form = new AltProjeTeklif();
                form.yil = yil;
                form.senaryo = SenaryoBul(kullanan, senaryo);
                form.birim = Temiz(kur);
                form.sektorNo = Temiz(sektor);
                ObjectArray formlar = TNS.YAT.Arac.Tanimla().AltProjeListeleYeni(kullanan, form);
                if (formlar == null) return Hata("Projeler okunamadı.");
                if (!formlar.sonuc.islemSonuc) return Hata(HataMetni(formlar.sonuc.hataStr, "Projeler okunamadı."));

                List<object> satirlar = new List<object>();
                for (int i = 0; i < formlar.ObjeSayisi; i++)
                {
                    AltProjeTeklif t = (AltProjeTeklif)formlar.objeler[i];
                    satirlar.Add(new
                    {
                        birim = t.birim,
                        sektorNo = t.sektorNo,
                        ad = t.ad,
                        baslangicYil = t.baslangicYil,
                        bitisYil = t.bitisYil,
                        projeTutari = t.projeTutari,
                        yiliTeklifi0 = t.yiliTeklifi0,
                        yiliTeklifi1 = t.yiliTeklifi1,
                        yiliTeklifi2 = t.yiliTeklifi2,
                        gerekce = t.gerekce
                    });
                }
                return Results.Json(new { satirlar = satirlar, kapali = Kapali(kullanan, yil) });
            }
            catch (Exception)
            {
                return Hata("Projeler okunamadı.");
            }
        }

        /// <summary>
        /// Birim ve sektörün yeni projelerini kaydeder (eski ProjeGerekceYeni Kaydet): var olanlar silinip yeniden yazılır.
        /// Kurumsal kodun tam seçilmesi, satır denetimleri, onay ve birim yetkisi iş kurallarındadır.
        /// </summary>
        private static async Task<IResult> YeniKaydet(HttpContext baglam, IWebHostEnvironment ortam, YeniProjeKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });
            if (Temiz(istek.Sektor) == "") return Results.Json(new { basarili = false, hata = "Kaydetmek için sektör seçiniz!" });

            try
            {
                List<AltProjeTeklif> projeler = new List<AltProjeTeklif>();
                foreach (YeniProjeSatiri s in istek.Satirlar ?? new List<YeniProjeSatiri>())
                {
                    if (s == null) continue;
                    AltProjeTeklif t = new AltProjeTeklif();
                    t.ad = (s.Ad ?? "").Trim();
                    t.bitisYil = s.BitisYil;
                    t.projeTutari = s.ProjeTutari;
                    t.yiliTeklifi0 = s.YiliTeklifi0;
                    t.yiliTeklifi1 = s.YiliTeklifi1;
                    t.yiliTeklifi2 = s.YiliTeklifi2;
                    t.gerekce = s.Gerekce ?? "";
                    projeler.Add(t);
                }

                Sonuc sonuc = TNS.YAT.Arac.Tanimla().AltProjeKaydetYeni(kullanan, istek.Yil, Temiz(istek.Kur), Temiz(istek.Sektor), projeler.ToArray());
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        // ------------------------------------------------------------------ onay (yalnızca sistem yöneticisi)

        /// <summary>Onayla / Onay kaldır (eski btnOnayla_Click): proje gerekçelerini veri girişine kapatır ya da açar.</summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, ProjeGerekceOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!GenelGorevUclari.YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });

            try
            {
                ENUMTasraHazirlikOnayDurum islem = istek.Onayla ? ENUMTasraHazirlikOnayDurum.KurumOnayla : ENUMTasraHazirlikOnayDurum.KurumOnayKaldir;
                Sonuc s = TNS.BHMv2.Arac.Tanimla().OnayKodAtaTasra(kullanan, istek.Yil, 0, "000", "0000", ENUMBirimOnayTur.PROJEGEREKCE, islem);
                if (!s.islemSonuc) return Results.Json(new { basarili = false, hata = "Onaylama/Onay kaldırma işlemi başarısız! " + HataMetni(s.hataStr, "") });
                return Results.Json(new { basarili = true, bilgi = "Onaylama/Onay kaldırma işlemi başarılı!" });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Onaylama/Onay kaldırma işlemi başarısız!" });
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        /// <summary>
        /// Yazdır düğmeleri (eski btnYazdir_Click): 1 Yazdır, 2 Yazdır (Ek) (Word); 1e Yazdır (Liste), 2e Yazdır (Ek Liste),
        /// 3 Yazdır (Özet) (Excel). Ek ve özet raporları yalnızca sistem yöneticisinindir.
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, string tur, int yil, string kur, string sektor, string senaryo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            tur = Temiz(tur);
            if (tur != "1" && tur != "1e" && !GenelGorevUclari.YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (yil < 2000) return Hata("Yıl hatalı!");
            kur = Temiz(kur);
            sektor = Temiz(sektor);
            senaryo = SenaryoBul(kullanan, senaryo);
            if (!KodlarGecerli(kur, sektor, senaryo)) return Hata("Kod hatalı!");

            try
            {
                AsposeLisans.Yukle();
                IYATServis servis = TNS.YAT.Arac.Tanimla();
                string senaryoAd = SenaryoAdBul(kullanan, senaryo);
                string sablon = Ayar.Oku("RaporSablonYol");
                string dosya;
                switch (tur)
                {
                    case "1":
                        dosya = ProjeGerekceRaporlari.ListeYazdir(servis, kullanan, yil, kur, sektor, senaryo, senaryoAd, 0, sablon);
                        return DosyaIslem.DosyaGonder(dosya, "YatirimProje_" + yil, true, "docx");
                    case "2":
                        dosya = ProjeGerekceRaporlari.ListeYazdir(servis, kullanan, yil, kur, sektor, senaryo, senaryoAd, 1, sablon);
                        return DosyaIslem.DosyaGonder(dosya, "YatirimProjeEk_" + yil, true, "docx");
                    case "1e":
                        dosya = ProjeGerekceRaporlari.ListeYazdirTablo(servis, kullanan, yil, kur, sektor, senaryo, senaryoAd, 0, sablon);
                        return DosyaIslem.DosyaGonder(dosya, "YatirimProjeListe_" + yil, true, "xlsx");
                    case "2e":
                        dosya = ProjeGerekceRaporlari.ListeYazdirTablo(servis, kullanan, yil, kur, sektor, senaryo, senaryoAd, 1, sablon);
                        return DosyaIslem.DosyaGonder(dosya, "YatirimProjeEkListe_" + yil, true, "xlsx");
                    case "3":
                        dosya = ProjeGerekceRaporlari.OzetYazdir(kullanan, yil, kur, "", senaryo, senaryoAd, sablon);
                        return DosyaIslem.DosyaGonder(dosya, "YatirimProjeOzet_" + yil, true, "xlsx");
                    default:
                        return Hata("Rapor türü hatalı!");
                }
            }
            catch (Exception e)
            {
                return Hata(HataMetni(e.Message, "Rapor hazırlanamadı."));
            }
        }
    }
}
