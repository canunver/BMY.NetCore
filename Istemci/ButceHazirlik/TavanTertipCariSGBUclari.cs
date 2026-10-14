using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    public class TertipCariSGBSatiri
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

    public class TertipCariSGBKayitIstegi
    {
        public int Yil { get; set; }
        /// <summary>Seçilen görüşme tavanının tertibi "kurumsal-fonksiyonel-finansman-ekonomik-faaliyet" (eski cbEkoFaal TertipKod).</summary>
        public string Kod { get; set; }
        public List<TertipCariSGBSatiri> Satirlar { get; set; }
    }

    public class TertipCariSGBOnayIstegi
    {
        public int Yil { get; set; }
        public string Kod { get; set; }
        /// <summary>true: onayla (durum 5), false: onayı kaldır (durum 1).</summary>
        public bool Onayla { get; set; }
    }

    /// <summary>
    /// Faaliyet Detay Teklif Girişi (SGB) sayfasının (eski ButceHazirlikv2/TavanTertipCariSGB.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/tavantertipsgb/tanim, /faaliyetler?yil=&amp;ust=, /ekofaal?yil=&amp;faal=, /liste?yil=&amp;kod=, /rapor?yil=&amp;kod=&amp;rapor=
    ///   POST /api/tavantertipsgb/kaydet, /onay
    /// Faaliyet/proje ve onun SGB görüşme (cari) tavanlarından biri seçilir; tavan, tertiplere (kurumsal-fonksiyonel-finansman-ekonomik)
    /// üç yıllık görüşme sonrası tutar ve ek ödenek olarak dağıtılır (tavan türü tavanTurTertipCari, senaryo "0100").
    /// Giriş yalnızca faaliyetin görüşme onayı 5 iken açıktır (eski TuslariAyarla). Rapor Al menüsü eski BHGiderRaporlariEkle raporlarıdır.
    /// </summary>
    public static class TavanTertipCariSGBUclari
    {
        private const string Senaryo = "0100";
        private const string GorusmeOnaySenaryo = "01.00"; // eski sayfa görüşme onayını bununla okuyordu
        private const string SayfaAdres = "../ButceHazirlikv2/TavanTertipCariSGB.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/tavantertipsgb/tanim", Tanim);
            app.MapGet("/api/tavantertipsgb/faaliyetler", Faaliyetler);
            app.MapGet("/api/tavantertipsgb/ekofaal", EkoFaal);
            app.MapGet("/api/tavantertipsgb/liste", Liste);
            app.MapGet("/api/tavantertipsgb/rapor", Rapor);
            app.MapPost("/api/tavantertipsgb/kaydet", Kaydet);
            app.MapPost("/api/tavantertipsgb/onay", Onay);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdres);
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

        private static string Duz(string kod)
        {
            return (kod ?? "").Replace(".", "").Trim();
        }

        /// <summary>"kur-fon-fin-eko-faaliyet" -> Tertip; beş kod da dolu değilse ya da kodlar geçersizse null.</summary>
        private static Tertip KodCoz(int yil, string kod)
        {
            kod = (kod ?? "").Trim();
            if (!Regex.IsMatch(kod, @"^[0-9.\-]+$")) return null;
            Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            t.TertipAta(kod);
            for (int i = 0; i < 5; i++)
                if (t.kodlar == null || t.kodlar.Length <= i || string.IsNullOrWhiteSpace(t.kodlar[i])) return null;
            return t;
        }

        /// <summary>Faaliyetin SGB görüşme onay durumu (eski TuslariAyarla'nın ilk okuması; -1: okunamadı).</summary>
        private static int GorusmeDurumOku(Kullanici kullanan, int yil, Tertip t)
        {
            TavanGenel tg = new TavanGenel();
            tg.kod1 = t.kodlar[4];
            tg = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayOku(kullanan, yil, GorusmeOnaySenaryo, ENUMTavanTur.tavanTurGorusmeCari, tg);
            return tg == null ? -1 : tg.durum;
        }

        /// <summary>
        /// Eski TuslariAyarla: faaliyetin görüşmesi onaylı (5) değilse -1 (giriş kapalı); onaylıysa tertip tekliflerinin onay durumu
        /// (0/1 açık, 5 onaylı, -1 okunamadı).
        /// </summary>
        private static int DurumOku(Kullanici kullanan, int yil, Tertip t)
        {
            if (GorusmeDurumOku(kullanan, yil, t) != 5) return -1;
            TavanGenel tg = new TavanGenel();
            tg.kod1 = t.kodlar[0];
            tg.kod2 = t.kodlar[1];
            tg.kod3 = t.kodlar[2];
            tg.kod4 = t.kodlar[3];
            tg.kod5 = t.kodlar[4];
            tg = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayOku(kullanan, yil, Senaryo, ENUMTavanTur.tavanTurTertipCari, tg);
            return tg == null ? -1 : tg.durum;
        }

        // ------------------------------------------------------------------ okuma

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return Results.Json(new { yil = KodTanimUclari.YilBul(null), raporlar = ButceHazirlikv2.BHGiderRaporlari.Liste() });
        }

        /// <summary>Faaliyet/proje ağacı (eski ddProg: kod türü 14, bütçe hazırlık yetkisiyle süzülür).</summary>
        private static IResult Faaliyetler(HttpContext baglam, IWebHostEnvironment ortam, int yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, "14", ust);
        }

        /// <summary>Faaliyetin SGB görüşme (cari) tavanları (eski ddProg_Changed: faaliyet tam seçilmiş olmalı).</summary>
        private static IResult EkoFaal(HttpContext baglam, IWebHostEnvironment ortam, int yil, string faal)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            faal = (faal ?? "").Trim();
            if (!GorusmeCariUclari.KodGecerli(faal)) return Results.Json(new { hata = "Kod hatalı!" });
            if (Duz(faal).Length != 8) return Results.Json(new { hata = "Faaliyeti tam seçiniz!" });
            try
            {
                TavanGenel kistas = new TavanGenel();
                kistas.kod5 = faal;
                ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleBHMTavanGenel(kullanan, yil, Senaryo, ENUMTavanTur.tavanTurGorusmeCari, kistas, false);
                if (oa == null) return Results.Json(new { hata = "Görüşme tavanları okunamadı." });
                if (!oa.sonuc.islemSonuc) return Results.Json(new { hata = GorusmeCariUclari.HataMetni(oa.sonuc.hataStr, "Görüşme tavanları okunamadı.") });
                List<object> kodlar = new List<object>();
                foreach (TavanGenel t in oa.objeler)
                    kodlar.Add(new { kod = t.TertipKod, ad = t.kod4Aciklama });
                return Results.Json(new { kodlar = kodlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Görüşme tavanları okunamadı." });
            }
        }

        private static object Satir(TavanGenel t)
        {
            return new
            {
                kod1 = t.kod1, kod2 = t.kod2, kod3 = t.kod3, kod4 = t.kod4, kod5 = t.kod5, ad4 = t.ad4,
                yil0 = t.yil0, yil1 = t.yil1, yil2 = t.yil2,
                ekOdenek = t.ekOdenek, ekGerekce = t.ekGerekce ?? "", ekOdenek1 = t.ekOdenek1, ekOdenek2 = t.ekOdenek2,
                teklif0 = t.teklif0, teklif1 = t.teklif1, teklif2 = t.teklif2,
                ekOdenekTeklif = t.ekOdenekTeklif, ekGerekceTeklif = (t.ekGerekceTeklif ?? "").Replace("<br>", "\n"),
                ekOdenek1Teklif = t.ekOdenek1Teklif, ekOdenek2Teklif = t.ekOdenek2Teklif
            };
        }

        /// <summary>
        /// Eski Oku: seçilen görüşme tavanı (TAVAN satırı) ve tertiplere dağıtımı; FARK ve TOPLAM satırlarını sayfa hesaplar.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            if (string.IsNullOrWhiteSpace(kod)) return Results.Json(new { hata = "Lütfen seçim yapınız!" });
            Tertip t = KodCoz(yil, kod);
            if (t == null) return Results.Json(new { hata = "Kod hatalı!" });

            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                ObjectArray liste1 = servis.ListeleGorusmeCari(kullanan, t, Senaryo, true);
                if (liste1 == null) return Results.Json(new { hata = "Tavan okunamadı." });
                if (!liste1.sonuc.islemSonuc) return Results.Json(new { hata = GorusmeCariUclari.HataMetni(liste1.sonuc.hataStr, "Tavan okunamadı.") });
                if (liste1.ObjeSayisi != 1) return Results.Json(new { hata = "Seçilen kod için tavan bilgisi bulunamadı!" });

                ObjectArray liste = servis.ListeleTertipTavan(kullanan, yil, Senaryo, t.kodlar);
                if (liste == null) return Results.Json(new { hata = "Tertip tavanları okunamadı." });
                if (!liste.sonuc.islemSonuc) return Results.Json(new { hata = GorusmeCariUclari.HataMetni(liste.sonuc.hataStr, "Tertip tavanları okunamadı.") });

                List<object> satirlar = new List<object>();
                for (int i = 0; i < liste.ObjeSayisi; i++)
                    satirlar.Add(Satir((TavanGenel)liste.objeler[i]));

                return Results.Json(new
                {
                    tavan = Satir((TavanGenel)liste1.objeler[0]),
                    satirlar = satirlar,
                    gorusmeDurum = GorusmeDurumOku(kullanan, yil, t),
                    durum = DurumOku(kullanan, yil, t)
                });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Tertip tavanları okunamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Eski Kaydet: tertip satırlarının (ekonomik kodu 2 haneden uzun) görüşme sonrası tutarları ve ek ödenekleri. Görüşme tavanını
        /// aşmama, ek ödenek gerekçesi gibi denetimler iş kurallarındadır. Eski sayfa Kaydet'i yalnızca giriş açıkken gösteriyordu;
        /// burada sunucu da denetler ve yalnızca listelenen tavanın tertiplerini yazar.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, TertipCariSGBKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000 || string.IsNullOrWhiteSpace(istek.Kod)) return Results.Json(new { basarili = false, hata = "Lütfen seçim yapınız!" });
            Tertip t = KodCoz(istek.Yil, istek.Kod);
            if (t == null) return Results.Json(new { basarili = false, hata = "Kod hatalı!" });

            try
            {
                int durum = DurumOku(kullanan, istek.Yil, t);
                if (durum < 0) return Results.Json(new { basarili = false, hata = "Faaliyetin SGB görüşmesi onaylanmamış; veri girişi kapalı." });
                if (durum > 1) return Results.Json(new { basarili = false, hata = "Daha önce onaylanmış verileri değiştiremezsiniz!" });

                TNSCollection tavanListe = new TNSCollection();
                foreach (TertipCariSGBSatiri s in istek.Satirlar ?? new List<TertipCariSGBSatiri>())
                {
                    if (s == null || (s.Kod4 ?? "").Length <= 2) continue; // TAVAN / FARK / TOPLAM satırları
                    if (!GorusmeCariUclari.KodGecerli(s.Kod1) || !GorusmeCariUclari.KodGecerli(s.Kod2) || !GorusmeCariUclari.KodGecerli(s.Kod3) || !GorusmeCariUclari.KodGecerli(s.Kod4))
                        return Results.Json(new { basarili = false, hata = "Kod hatalı!" });
                    // Yalnızca listelenen tavanın tertipleri (eski liste kurumsal, fonksiyonel, finansman ve ekonomik kodun başıyla süzülür).
                    if (!Duz(s.Kod1).StartsWith(Duz(t.kodlar[0])) || !Duz(s.Kod2).StartsWith(Duz(t.kodlar[1]))
                        || !Duz(s.Kod3).StartsWith(Duz(t.kodlar[2])) || !Duz(s.Kod4).StartsWith(Duz(t.kodlar[3])))
                        continue;
                    TavanGenel tg = new TavanGenel();
                    tg.kod1 = s.Kod1;
                    tg.kod2 = s.Kod2;
                    tg.kod3 = s.Kod3;
                    tg.kod4 = s.Kod4;
                    tg.kod5 = t.kodlar[4]; // tertipler listelenen faaliyete yazılır
                    tg.yil0 = s.Yil0;
                    tg.yil1 = s.Yil1;
                    tg.yil2 = s.Yil2;
                    tg.ekOdenek = s.EkOdenek;
                    tg.ekGerekce = s.EkGerekce ?? "";
                    tg.ekOdenek1 = s.EkOdenek1;
                    tg.ekOdenek2 = s.EkOdenek2;
                    tavanListe.Add(tg);
                }
                if (tavanListe.Count == 0) return Results.Json(new { basarili = true, bilgi = "Kaydedilecek değişiklik yok." });

                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().KaydetBHMTavanGenel(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurTertipCari, tavanListe);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = GorusmeCariUclari.HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>Onayla (5) / Onay kaldır (1) (eski btnOnay_Click, tur 1 / 10): seçili tertip için.</summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, TertipCariSGBOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000 || string.IsNullOrWhiteSpace(istek.Kod)) return Results.Json(new { basarili = false, hata = "Lütfen seçim yapınız!" });
            Tertip t = KodCoz(istek.Yil, istek.Kod);
            if (t == null) return Results.Json(new { basarili = false, hata = "Lütfen onaylanacak faaliyeti seçiniz!" });

            try
            {
                int durum = DurumOku(kullanan, istek.Yil, t);
                if (durum < 0) return Results.Json(new { basarili = false, hata = "Faaliyetin SGB görüşmesi onaylanmamış; işlem yapılamaz." });

                TavanGenel tg = new TavanGenel();
                tg.kod1 = t.kodlar[0];
                tg.kod2 = t.kodlar[1];
                tg.kod3 = t.kodlar[2];
                tg.kod4 = t.kodlar[3];
                tg.kod5 = t.kodlar[4];
                tg.durum = istek.Onayla ? 5 : 1;
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayKaydet(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurTertipCari, tg);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = GorusmeCariUclari.HataMetni(sonuc.hataStr, "İşlem başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti.", durum = DurumOku(kullanan, istek.Yil, t) });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem başarısız." });
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        /// <summary>
        /// Rapor Al menüsü (eski btnRaporAl_Click → BHMv2Ortak.RaporAl(.., "0100", varsayılan kurum, "", "", "", kistas.kod4)). Eski sayfa
        /// tertip kodunu "-" ile ayrılmış haliyle tamKod'a ("@" ile ayrılır) verdiğinden kistas.kod4 hep boştu; rapor kurumun tamamı için
        /// alınıyordu. Aynı davranış korunur, yalnızca tavan seçilmiş olmalıdır.
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kod, string rapor)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            if (string.IsNullOrWhiteSpace(kod)) return Results.Json(new { hata = "Lütfen seçim yapınız!" });
            if (KodCoz(yil, kod) == null) return Results.Json(new { hata = "Kod hatalı!" });
            if (!ButceHazirlikv2.BHGiderRaporlari.Tanimli(rapor)) return Results.Json(new { hata = "Rapor seçimi hatalı!" });
            try
            {
                AsposeLisans.Yukle();
                string dosya = ButceHazirlikv2.BHGiderRaporlari.RaporAl(kullanan, rapor, yil, Senaryo, TNS.DEG.Arac.VarsayilanKurumBul(),
                    "", "", "", "", Ayar.Oku("RaporSablonYol"));
                return DosyaIslem.DosyaGonder(dosya, "Rapor_" + rapor + "_" + DosyaIslem.DosyaAdUretSade(), true, "xlsx");
            }
            catch (ButceHazirlikv2.RaporHatasi ex)
            {
                return Results.Json(new { hata = GorusmeCariUclari.HataMetni(ex.Message, "Rapor hazırlanamadı.") });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }
    }
}
