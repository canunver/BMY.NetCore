using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

namespace Istemci.ButceHazirlik
{
    public class TertipTavanSatiri
    {
        public string Kod1 { get; set; }
        public string Kod2 { get; set; }
        public string Kod3 { get; set; }
        public string Kod4 { get; set; }
        public double Yil0 { get; set; }
        public double Yil1 { get; set; }
        public double Yil2 { get; set; }
        public double EkOdenek { get; set; }
        public string EkGerekce { get; set; }
        public double EkOdenek1 { get; set; }
        public double EkOdenek2 { get; set; }
    }

    public class TertipTavanKayitIstegi
    {
        public int Yil { get; set; }
        /// <summary>Seçilen faaliyet tavanı "eko@sektör@birim@faaliyet" (eski cbEkoFaal tamKod).</summary>
        public string Kod { get; set; }
        public List<TertipTavanSatiri> Satirlar { get; set; }
    }

    public class TertipTavanOnayIstegi
    {
        public int Yil { get; set; }
        public string Kod { get; set; }
        /// <summary>true: onayla (durum 5), false: onayı kaldır (durum 1).</summary>
        public bool Onayla { get; set; }
    }

    /// <summary>
    /// Faaliyet/Proje Detay Teklif Girişi sayfasının (eski ButceHazirlikv2/TavanTertip.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/tavantertip/tanim, /faaliyetler?yil=&amp;ust=, /ekofaal?yil=&amp;faal=, /liste?yil=&amp;kod=, /rapor?yil=&amp;kod=&amp;rapor=
    ///   POST /api/tavantertip/kaydet, /onay
    /// Faaliyet/proje ve onun ekonomik kod + sektör + birim faaliyet tavanı seçilir; tavan tertiplere (kurumsal-fonksiyonel-
    /// finansman-ekonomik) dağıtılır (tavan türü tavanTurTertipYatirim, senaryo "0000"). Rapor Al menüsü eski BHGiderRaporlariEkle raporlarıdır (BHGiderRaporlari).
    /// </summary>
    public static class TavanTertipUclari
    {
        private const string Senaryo = "0000";
        private const string OnaySenaryo = "00.00"; // eski sayfa onayı bununla okuyordu (noktalar silinir, aynı kayıt)
        private const string KodTuru = "14";        // faaliyet/proje (PEB) kodları
        // Eski ağaç (KodAgac, yetkiKodu 20 = bütçe hazırlık): kullanıcının bu yetki tiplerindeki faaliyetleri kök olarak gösterilir.
        private static readonly int[] DamgaNolar = { 55, 56, 57 };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/tavantertip/tanim", Tanim);
            app.MapGet("/api/tavantertip/faaliyetler", Faaliyetler);
            app.MapGet("/api/tavantertip/ekofaal", EkoFaal);
            app.MapGet("/api/tavantertip/liste", Liste);
            app.MapGet("/api/tavantertip/rapor", Rapor);
            app.MapPost("/api/tavantertip/kaydet", Kaydet);
            app.MapPost("/api/tavantertip/onay", Onay);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../ButceHazirlikv2/TavanTertip.aspx");
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

        /// <summary>"eko@sektör@birim@faaliyet" -> TavanGenel (kod1..kod4); dördü de dolu değilse null.</summary>
        private static TavanGenel KodCoz(string kod)
        {
            string[] k = (kod ?? "").Split('@');
            if (k.Length < 4) return null;
            for (int i = 0; i < 4; i++)
                if (string.IsNullOrWhiteSpace(k[i])) return null;
            TavanGenel tg = new TavanGenel();
            tg.kod1 = k[0].Trim();
            tg.kod2 = k[1].Trim();
            tg.kod3 = k[2].Trim();
            tg.kod4 = k[3].Trim();
            return tg;
        }

        /// <summary>Tertip tavanlarının onay durumu (eski BHMTavanGenelOnayOku; -1: okunamadı, 0: kayıt yok).</summary>
        private static int DurumOku(Kullanici kullanan, int yil, TavanGenel secim)
        {
            TavanGenel tg = new TavanGenel();
            tg.kod1 = secim.kod1;
            tg.kod2 = secim.kod2;
            tg.kod3 = secim.kod3;
            tg.kod4 = secim.kod4;
            tg = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayOku(kullanan, yil, OnaySenaryo, ENUMTavanTur.tavanTurTertipYatirim, tg);
            return tg == null ? -1 : tg.durum;
        }

        // ------------------------------------------------------------------ okuma

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return Results.Json(new { yil = KodTanimUclari.YilBul(null), sistemYoneticisi = GenelGorevUclari.YetkiVar(kullanan), raporlar = ButceHazirlikv2.BHGiderRaporlari.Liste() });
        }

        private static int KirilimBoyBul(int yil, out string kodYapi)
        {
            kodYapi = TNS.DEG.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD" + KodTuru + "/KODYAPI");
            if (string.IsNullOrEmpty(kodYapi)) return 0;
            return kodYapi.Split('.').Length;
        }

        /// <summary>
        /// Faaliyet/proje ağacının bir düzeyi (eski KodAgac.KodDoldurExt, kod türü 14, yetki kodu 20). Kökte, kullanıcının
        /// faaliyet yetki tiplerinde (55, 56, 57) değer varsa ve hiçbiri "*" değilse yalnızca o faaliyetler gösterilir.
        /// </summary>
        private static IResult Faaliyetler(HttpContext baglam, IWebHostEnvironment ortam, int yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            ust = (ust ?? "").Trim();

            try
            {
                IBKDServis servis = TNS.BKD.Arac.Tanimla();
                int kirilimBoy = KirilimBoyBul(yil, out string kirilim);
                List<object> dugumler = new List<object>();

                if (ust == "")
                {
                    List<string> yetkili = new List<string>();
                    bool yildizVar = false;
                    foreach (int damga in DamgaNolar)
                    {
                        foreach (YetkiTipi yt in TNS.KYM.Arac.yetkiTipiListesi)
                        {
                            if (yt.kod != damga) continue;
                            string deger = kullanan.YetkiTipiDegeriBul(yt.ad);
                            if (deger.Contains("*")) { yildizVar = true; break; }
                            foreach (string parca in deger.Split(',', ';'))
                            {
                                string ekle = parca.Replace(".", "").Trim();
                                if (ekle != "" && !yetkili.Contains(ekle)) yetkili.Add(ekle);
                            }
                        }
                        if (yildizVar) break;
                    }
                    if (!yildizVar && yetkili.Count > 0)
                    {
                        foreach (string kod in yetkili)
                        {
                            string noktali = OrtakFonksiyonlar.KodNoktaKoy(kirilim, kod);
                            string ad = servis.KodAdBul(kullanan, yil, KodTuru, noktali);
                            dugumler.Add(new { kod = noktali, ad = noktali + "- " + ad, altVar = noktali.Split('.').Length < kirilimBoy });
                        }
                        return Results.Json(new { dugumler = dugumler });
                    }
                }

                int ustUz = 0;
                if (ust != "")
                {
                    ust = OrtakFonksiyonlar.KodNoktaKoy(kirilim, ust.Replace(".", ""));
                    ustUz = ust.Split('.').Length;
                }
                if (ustUz >= kirilimBoy) return Results.Json(new { dugumler = dugumler });
                bool altVar = ustUz < kirilimBoy - 1;

                ABSKodlarVAN kodlar = servis.KodListeleSeviyeIkiYil(yil, KodTuru, ust, 0, false, false, "", "", "", true, true, "", false);
                if (kodlar == null || !kodlar.sonuc.islemSonuc) return Results.Json(new { hata = "Faaliyetler okunamadı." });
                string dil = string.IsNullOrEmpty(kullanan.aktifKultur) ? TNS.BKD.Arac.Diller(0) : kullanan.aktifKultur;
                foreach (ABSKod k in kodlar.absKodlar)
                    dugumler.Add(new { kod = k.kod, ad = k.kod + "- " + k.Ad(dil), altVar = altVar });
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Faaliyetler okunamadı." });
            }
        }

        /// <summary>Faaliyetin onaylı ekonomik kod + sektör + birim tavanları (eski ddProg_Changed).</summary>
        private static IResult EkoFaal(HttpContext baglam, IWebHostEnvironment ortam, int yil, string faal)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            if (string.IsNullOrWhiteSpace(faal)) return Results.Json(new { kodlar = new object[0] });
            try
            {
                TavanGenel kistas = new TavanGenel();
                kistas.kod4 = faal.Trim();
                ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleBHMTavanGenel(kullanan, yil, Senaryo, ENUMTavanTur.tavanTurEkoFaaliyet, kistas, true);
                if (oa == null) return Results.Json(new { hata = "Faaliyet tavanları okunamadı." });
                if (!oa.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(oa.sonuc.hataStr, "Faaliyet tavanları okunamadı.") });
                List<object> kodlar = new List<object>();
                for (int i = 0; i < oa.ObjeSayisi; i++)
                {
                    TavanGenel t = (TavanGenel)oa.objeler[i];
                    if (string.IsNullOrWhiteSpace(t.kod1)) continue;
                    kodlar.Add(new { kod = t.kod1 + "@" + t.kod2 + "@" + t.kod3 + "@" + t.kod4, ad = t.ekoSektorAciklama });
                }
                return Results.Json(new { kodlar = kodlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Faaliyet tavanları okunamadı." });
            }
        }

        private static object Satir(TavanGenel t)
        {
            return new
            {
                kod1 = t.kod1, kod2 = t.kod2, kod3 = t.kod3, kod4 = t.kod4, kod5 = t.kod5, ad4 = t.ad4,
                odenek_2 = t.odenek_2, harcama_2 = t.harcama_2, odenek_1 = t.odenek_1, harcama_1 = t.harcama_1, talep_0 = t.talep_0,
                yil0 = t.yil0, yil1 = t.yil1, yil2 = t.yil2,
                ekOdenek = t.ekOdenek, ekGerekce = t.ekGerekce ?? "", ekOdenek1 = t.ekOdenek1, ekOdenek2 = t.ekOdenek2
            };
        }

        /// <summary>
        /// Seçilen faaliyet tavanı ve tertiplere dağıtımı (eski Oku). TAVAN satırı faaliyet tavanıdır; FARK ve TOPLAM satırlarını sayfa hesaplar.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            TavanGenel secim = KodCoz(kod);
            if (secim == null) return Results.Json(new { hata = "Lütfen seçim yapınız!" });

            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                ObjectArray liste1 = servis.ListeleEkoSektorFaaliyetTavan(kullanan, yil, Senaryo, secim.kod1, secim.kod2, secim.kod3, secim.kod4);
                if (liste1 == null) return Results.Json(new { hata = "Tavan okunamadı." });
                if (!liste1.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(liste1.sonuc.hataStr, "Tavan okunamadı.") });
                if (liste1.ObjeSayisi != 1) return Results.Json(new { hata = "Seçilen kod için tavan bilgisi bulunamadı!" });

                ObjectArray liste = servis.ListeleTertipTavan(kullanan, yil, Senaryo, new string[] { "", "", "", secim.kod1, secim.kod4 });
                if (liste == null) return Results.Json(new { hata = "Tertip tavanları okunamadı." });
                if (!liste.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(liste.sonuc.hataStr, "Tertip tavanları okunamadı.") });

                List<object> satirlar = new List<object>();
                for (int i = 0; i < liste.ObjeSayisi; i++)
                    satirlar.Add(Satir((TavanGenel)liste.objeler[i]));

                return Results.Json(new { tavan = Satir((TavanGenel)liste1.objeler[0]), satirlar = satirlar, durum = DurumOku(kullanan, yil, secim) });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Tertip tavanları okunamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Tertip tavanlarını kaydeder (eski Kaydet). Faaliyet tavanını aşmama, faaliyet tavanının onaylı olması, ek ödenek gerekçesi,
        /// binin katı gibi denetimler iş kurallarındadır; onaylanmış tertip tavanları burada da reddedilir.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, TertipTavanKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            TavanGenel secim = istek == null ? null : KodCoz(istek.Kod);
            if (istek == null || istek.Yil < 2000 || secim == null) return Results.Json(new { basarili = false, hata = "Lütfen seçim yapınız!" });

            try
            {
                int durum = DurumOku(kullanan, istek.Yil, secim);
                if (durum < 0) return Results.Json(new { basarili = false, hata = "Onay durumu okunamadı." });
                if (durum > 1) return Results.Json(new { basarili = false, hata = "Daha önce onaylanmış verileri değiştiremezsiniz!" });

                TNSCollection tavanListe = new TNSCollection();
                foreach (TertipTavanSatiri s in istek.Satirlar ?? new List<TertipTavanSatiri>())
                {
                    if (s == null || (s.Kod4 ?? "").Length <= 2) continue; // TAVAN / FARK / TOPLAM satırları
                    TavanGenel tg = new TavanGenel();
                    tg.kod1 = s.Kod1;
                    tg.kod2 = s.Kod2;
                    tg.kod3 = s.Kod3;
                    tg.kod4 = s.Kod4;
                    tg.kod5 = secim.kod4; // tertipler listelenen faaliyete yazılır
                    tg.yil0 = s.Yil0;
                    tg.yil1 = s.Yil1;
                    tg.yil2 = s.Yil2;
                    tg.ekOdenek = s.EkOdenek;
                    tg.ekGerekce = s.EkGerekce ?? "";
                    tg.ekOdenek1 = s.EkOdenek1;
                    tg.ekOdenek2 = s.EkOdenek2;
                    tavanListe.Add(tg);
                }

                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().KaydetBHMTavanGenel(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurTertipYatirim, tavanListe);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>Onayla (5) / Onay kaldır (1) (eski btnOnay_Click).</summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, TertipTavanOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            TavanGenel tg = istek == null ? null : KodCoz(istek.Kod);
            if (istek == null || istek.Yil < 2000 || tg == null) return Results.Json(new { basarili = false, hata = "Lütfen onaylanacak faaliyeti seçiniz!" });

            try
            {
                tg.durum = istek.Onayla ? 5 : 1;
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayKaydet(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurTertipYatirim, tg);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "İşlem başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti.", durum = tg.durum });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem başarısız." });
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        /// <summary>
        /// Rapor Al menüsü (eski RaporAl → BHMv2Ortak.RaporAl(.., "0000", varsayılan kurum, "", "", "", faaliyet)): seçili faaliyet için
        /// eski Rapor.xml tanımlı raporlar (Form 4-2, 8, 9, 25, ekonomik detay, üçgenler, program bütçe listesi).
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kod, string rapor)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            TavanGenel secim = KodCoz(kod);
            if (secim == null) return Results.Json(new { hata = "Lütfen seçim yapınız!" });
            if (!ButceHazirlikv2.BHGiderRaporlari.Tanimli(rapor)) return Results.Json(new { hata = "Rapor seçimi hatalı!" });
            try
            {
                AsposeLisans.Yukle();
                string dosya = ButceHazirlikv2.BHGiderRaporlari.RaporAl(kullanan, rapor, yil, Senaryo, TNS.DEG.Arac.VarsayilanKurumBul(),
                    "", "", "", secim.kod4, Ayar.Oku("RaporSablonYol"));
                return DosyaIslem.DosyaGonder(dosya, "Rapor_" + rapor + "_" + DosyaIslem.DosyaAdUretSade(), true, "xlsx");
            }
            catch (ButceHazirlikv2.RaporHatasi ex)
            {
                return Results.Json(new { hata = HataMetni(ex.Message, "Rapor hazırlanamadı.") });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }
    }
}
