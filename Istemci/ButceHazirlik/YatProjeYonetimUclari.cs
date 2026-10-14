using System;
using System.Collections.Generic;
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
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    public class YatAsamaKopyaIstegi
    {
        public int KaynakYil { get; set; }
        public string KaynakAsama { get; set; }
        public int HedefYil { get; set; }
        public string HedefAsama { get; set; }
        /// <summary>Sırasıyla: projeler, alt projeler, kaynaklar, kaynak rakamları, detay programlar, yolluklar.</summary>
        public List<bool> Secimler { get; set; }
        public List<int> Projeler { get; set; }
    }

    public class YatOnayIstegi
    {
        /// <summary>proje tanımı (aşamalı) için 1 aç, 2 kullanıcılara kapat, 3 herkese kapat; proje yılı için 4, 5, 6 aynı sırayla.</summary>
        public int Yer { get; set; }
        public int Yil { get; set; }
        public string Asama { get; set; }
    }

    public class YatSozlukEkleIstegi
    {
        public int Tur { get; set; }
        public string Ad { get; set; }
    }

    /// <summary>
    /// Yatırım Projeleri Yönetim sayfasının (eski ButceHazirlikv2/YatProjeYonetim.aspx ve sekmelerinde açılan
    /// YATProjeOnay.aspx, YATTanimSozluk.aspx?tur=1/2/3) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/yatproje/tanim, /projeler?yil=&amp;asama=, /onay, /sozluk?tur=&amp;yil=&amp;asama=
    ///   POST /api/yatproje/kopyala (aşama kopyalama), /onay (veri girişine aç / kapat), /sozluk (sözlüğe ad ekleme)
    /// İşlemler eskisi gibi TNS.YAT.Arac.Tanimla() ve TNS.BHMv2.Arac.Tanimla() üzerinden yapılır. Sayfayı menüde bu seçeneği
    /// görebilenler kullanır; onay durumunu değiştirmeye ve sözlüğe eklemeye iş kuralları yalnızca sistem yöneticisine izin verir.
    /// </summary>
    public static class YatProjeYonetimUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/yatproje/tanim", Tanim);
            app.MapGet("/api/yatproje/projeler", Projeler);
            app.MapGet("/api/yatproje/onay", OnayListe);
            app.MapGet("/api/yatproje/sozluk", SozlukListe);
            app.MapPost("/api/yatproje/kopyala", Kopyala);
            app.MapPost("/api/yatproje/onay", OnayKaydet);
            app.MapPost("/api/yatproje/sozluk", SozlukEkle);
        }

        /// <summary>Kullanıcı Yatırım Projeleri Yönetim sayfasını kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../ButceHazirlikv2/YatProjeYonetim.aspx");
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

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string Temiz(string deger)
        {
            return (deger ?? "").Trim();
        }

        /// <summary>Onay durumunun adı (iş kuralı: 1 açık, 4 yalnızca sistem yöneticisine açık, diğerleri kapalı).</summary>
        private static string DurumAd(int durum)
        {
            switch (durum)
            {
                case 1: return "Veri girişine açık";
                case 4: return "Kullanıcılara kapalı";
                case 5: return "Herkese kapalı";
                default: return durum.ToString();
            }
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Açılış (eski Page_Load): bütçe hazırlık yılı, aşamalar (senaryolar; ilki seçili gelir), sistem yöneticisi mi.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski GenelIslemlerIstemci.HazirlikYiliBul
            int yil = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/BUTCEHAZIRLIK"), DateTime.Now.Year + 1);
            // Eski BHMv2Ortak.SenaryodanExtStore
            List<object> asamalar = new List<object>();
            try
            {
                ButceSenaryo[] senaryolar = TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null);
                if (senaryolar != null)
                    foreach (ButceSenaryo bs in senaryolar)
                        asamalar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Bütçe aşamaları okunamadı." });
            }

            return Results.Json(new
            {
                yil = yil,
                // Onaylama sekmesinin yıl alanları eskisi gibi bu yılla açılır.
                buYil = DateTime.Now.Year,
                asamalar = asamalar,
                yonetici = GenelGorevUclari.YetkiVar(kullanan)
            });
        }

        /// <summary>Aşama kopyalamada seçilecek projeler: kaynak yıl ve aşamada tanımlı projeler (eski ProjeListesiOl).</summary>
        private static IResult Projeler(HttpContext baglam, IWebHostEnvironment ortam, int yil, string asama)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Kaynak yıl hatalı!" });
            if (string.IsNullOrWhiteSpace(asama)) return Results.Json(new { hata = "Kaynak aşamayı seçiniz." });

            try
            {
                ObjectArray oa = TNS.YAT.Arac.Tanimla().TanimSozlukListele(kullanan, 1, yil, asama.Trim());
                if (!oa.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(oa.sonuc.hataStr, "Projeler okunamadı.") });
                List<object> satirlar = new List<object>();
                foreach (Sozluk s in oa.objeler)
                    satirlar.Add(new { kod = s.kod, ad = s.ad });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Projeler okunamadı." });
            }
        }

        private static List<object> OnayOku(Kullanici kullanan, IBHMv2Servis servis, string senaryo, ENUMTavanTur tur)
        {
            TavanGenel tg = new TavanGenel();
            tg.kod1 = "";
            tg.kod2 = "";
            ObjectArray oa = servis.BHMTavanGenelOnayListele(kullanan, 0, senaryo, tur, tg);
            if (!oa.sonuc.islemSonuc) throw new InvalidOperationException(oa.sonuc.hataStr);
            List<object> satirlar = new List<object>();
            foreach (TavanGenel t in oa.objeler)
                satirlar.Add(new { yil = t.yil, senaryoKod = t.senaryoKod, durum = t.durum, durumAd = DurumAd(t.durum) });
            return satirlar;
        }

        /// <summary>Onaylama sekmesinin iki listesi (eski YATProjeOnay VerileriOku): proje tanımı (yıl + aşama) ve proje yılı durumları.</summary>
        private static IResult OnayListe(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                return Results.Json(new
                {
                    tanim = OnayOku(kullanan, servis, "", ENUMTavanTur.YATProjeTanim),
                    yillar = OnayOku(kullanan, servis, "00.00", ENUMTavanTur.YATProjeYil)
                });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Onay durumları okunamadı." });
            }
        }

        /// <summary>
        /// Projeler / Alt Projeler / Kaynaklar sözlüğü (eski YATTanimSozluk Listele): sözlükteki adlar, seçilen yıl ve aşamadaki
        /// karşılıkları (Aşama Ad) ve bağlı kayıt sayıları; aşamada olup sözlükte olmayanlar sona eklenir.
        /// </summary>
        private static IResult SozlukListe(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string asama)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (tur < 1 || tur > 3) return Results.BadRequest();
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            string senaryo = Temiz(asama);
            if (senaryo == "") return Results.Json(new { hata = "Aşamayı seçiniz." });

            try
            {
                IYATServis servis = TNS.YAT.Arac.Tanimla();
                ObjectArray oa = servis.TanimSozlukListele(kullanan, tur, 0, null);
                if (!oa.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(oa.sonuc.hataStr, "Liste okunamadı.") });

                // Aşamadaki karşılıklar: kod -> ad (sırası korunur)
                List<KeyValuePair<int, string>> asamadakiler = new List<KeyValuePair<int, string>>();
                BilgiVAN bv;
                if (tur == 1)
                {
                    bv = servis.ProjeListele(kullanan, yil, senaryo, new Proje(), TNS.DEG.Arac.VarsayilanKurumBul(), false);
                    if (bv?.liste != null) foreach (Proje p in bv.liste) asamadakiler.Add(new KeyValuePair<int, string>(p.kod, p.ad));
                }
                else if (tur == 2)
                {
                    bv = servis.AltProjeListele(kullanan, yil, senaryo, new AltProje(), "", false, false);
                    if (bv?.liste != null) foreach (AltProje p in bv.liste) asamadakiler.Add(new KeyValuePair<int, string>(p.kod, p.ad));
                }
                else
                {
                    bv = servis.ButceKaynakListele(kullanan, 0, 0, 0, yil, senaryo, null, false);
                    if (bv?.liste != null) foreach (ButceKaynak k in bv.liste) asamadakiler.Add(new KeyValuePair<int, string>(k.kaynakNo, k.kaynakAd));
                }

                // Eski ListeBirlestir
                List<object> satirlar = new List<object>();
                foreach (Sozluk s in oa.objeler)
                {
                    string asamaAd = null;
                    int yer = asamadakiler.FindIndex(x => x.Key == s.kod);
                    if (yer >= 0)
                    {
                        asamaAd = asamadakiler[yer].Value;
                        asamadakiler.RemoveAt(yer);
                    }
                    satirlar.Add(new { kod = s.kod, ad = s.ad, aciklama = asamaAd, say = s.say });
                }
                foreach (KeyValuePair<int, string> k in asamadakiler)
                    satirlar.Add(new { kod = k.Key, ad = "", aciklama = k.Value, say = 0 });

                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Liste okunamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>Seçilen projelerin bilgilerini bir yılın aşamasından başka bir yılın aşamasına kopyalar (eski btnKopyala_Click).</summary>
        private static async Task<IResult> Kopyala(HttpContext baglam, IWebHostEnvironment ortam, YatAsamaKopyaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            string kaynakAsama = Temiz(istek.KaynakAsama);
            string hedefAsama = Temiz(istek.HedefAsama);
            bool[] secimler = new bool[6];
            if (istek.Secimler != null)
                for (int i = 0; i < secimler.Length && i < istek.Secimler.Count; i++) secimler[i] = istek.Secimler[i];

            List<string> hatalar = new List<string>();
            if (istek.KaynakYil < 2000) hatalar.Add("Kaynak yıl hatalı!");
            if (istek.HedefYil < 2000) hatalar.Add("Hedef yıl hatalı!");
            if (Array.IndexOf(secimler, true) < 0) hatalar.Add("Kopyalanacak bilgileri işaretleyiniz.");
            if (hatalar.Count > 0) return Results.Json(new { basarili = false, hata = string.Join(" ", hatalar) });

            TNSCollection tc = new TNSCollection();
            foreach (int kod in istek.Projeler ?? new List<int>())
            {
                Sozluk soz = new Sozluk();
                soz.kod = kod;
                tc.Add(soz);
            }

            Sonuc sonuc;
            try
            {
                sonuc = TNS.YAT.Arac.Tanimla().AsamaKopyala(kullanan, istek.KaynakYil, kaynakAsama, istek.HedefYil, hedefAsama, secimler, tc);
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kopyalama işlemi başarısız." });
            }
            // Eski sayfa hata ve bilgi metnini birlikte gösteriyordu.
            string bilgi = Temiz(sonuc.bilgiStr);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kopyalama işlemi başarısız."), bilgi = bilgi });
            if (bilgi == "") bilgi = "Kopyalama tamamlandı.";
            return Results.Json(new { basarili = true, bilgi = bilgi });
        }

        /// <summary>Proje tanımı (yıl + aşama) ya da proje yılı için veri girişini açar / kapatır (eski YATProjeOnay btnTus_Click).</summary>
        private static async Task<IResult> OnayKaydet(HttpContext baglam, IWebHostEnvironment ortam, YatOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yer < 1 || istek.Yer > 6) return Results.BadRequest();
            if (istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });

            string asama;
            ENUMTavanTur tur;
            if (istek.Yer <= 3)
            {
                asama = Temiz(istek.Asama);
                if (asama == "") return Results.Json(new { basarili = false, hata = "Aşamayı seçiniz." });
                tur = ENUMTavanTur.YATProjeTanim;
            }
            else
            {
                asama = "0000";
                tur = ENUMTavanTur.YATProjeYil;
            }

            TavanGenel tg = new TavanGenel();
            tg.kod1 = "";
            tg.kod2 = "";
            if (istek.Yer == 1 || istek.Yer == 4) tg.durum = 1;
            else if (istek.Yer == 2 || istek.Yer == 5) tg.durum = 4;
            else tg.durum = 5;

            Sonuc sonuc;
            try
            {
                sonuc = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayKaydet(kullanan, istek.Yil, asama, tur, tg);
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem başarısız." });
            }
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = "İşlem başarısız: " + HataMetni(sonuc.hataStr, "") });
            return Results.Json(new { basarili = true, bilgi = "İşlem başarılı." });
        }

        /// <summary>Sözlüğe yeni ad ekler (eski YATTanimSozluk YeniEkle); yeni kodu döndürür.</summary>
        private static async Task<IResult> SozlukEkle(HttpContext baglam, IWebHostEnvironment ortam, YatSozlukEkleIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Tur < 1 || istek.Tur > 3) return Results.BadRequest();
            string ad = Temiz(istek.Ad);
            if (ad == "") return Results.Json(new { basarili = false, hata = "Adı giriniz." });

            Sonuc sonuc;
            try
            {
                Sozluk sozluk = new Sozluk();
                sozluk.ad = ad;
                sonuc = TNS.YAT.Arac.Tanimla().TanimSozlukEkle(kullanan, istek.Tur, sozluk);
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
            return Results.Json(new { basarili = true, kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0), ad = ad });
        }
    }
}
