using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BUM;
using TNS.KYM;

namespace Istemci.ButceUygulama
{
    public class NakitTutar
    {
        /// <summary>Harcama türü: 1 Maaş, 2 SGK, 3 Vergi, 4 Abonelik, 5 Hakediş, 6 Yargılama, 7 Yatırım Giderleri, 99 Diğer.</summary>
        public int HarcamaTur { get; set; }
        public double Tutar { get; set; }
    }

    public class NakitKayitIstegi
    {
        /// <summary>yyyy-MM-dd</summary>
        public string Tarih { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        public List<NakitTutar> Tutarlar { get; set; }
    }

    /// <summary>
    /// Günlük Nakit İhtiyacı sayfasının (eski ButceUygulama/GunlukNakitIhtiyaci.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/gunluknakit/tanim                                   açılış: varsayılan tarih (yarın), kullanıcının muhasebe birimi, harcama türleri
    ///   GET  /api/gunluknakit/liste?tarih=&amp;muhasebe=&amp;birim=   tarihin 5 iş günü öncesi / sonrası günlük toplamlar (eski ListeleGunlukNakitIhtiyac, aralıklı)
    ///   GET  /api/gunluknakit/detay?tarih=&amp;muhasebe=&amp;birim=   bir günün harcama türlerine göre tutarları (eski ListeleGunlukNakitIhtiyac, aralıksız)
    ///   POST /api/gunluknakit/kaydet                                  bir günün tutarları (eski KaydetGunlukNakitIhtiyac)
    ///   GET  /api/gunluknakit/rapor?tarih=                            Excel (eski BUMRaporAl.aspx RaporNo=1, bütün birimler)
    /// Muhasebe / birim seçimi ortak bileşenle yapılır (/api/muhbirim/..., OrtakSayfa/MuhasebeBirimUclari.cs).
    /// İşlemler eskisi gibi TNS.BUM.Arac.Tanimla() üzerinden yapılır; muhasebe birimi görme / yazma yetkisi iş kurallarında denetlenir.
    /// </summary>
    public static class GunlukNakitIhtiyaciUclari
    {
        private const string SayfaAdresi = "../ButceUygulama/GunlukNakitIhtiyaci.aspx";

        /// <summary>Eski sayfadaki detay penceresinin alanları (TUR1 ... TUR7, TUR99), aynı sırayla.</summary>
        private static readonly (int Kod, string Ad)[] Turler =
        {
            (1, "Maaş"), (2, "SGK"), (3, "Vergi"), (4, "Abonelik"), (5, "Hakediş"), (6, "Yargılama"), (7, "Yatırım Giderleri"), (99, "Diğer")
        };

        private static readonly CultureInfo Tr = new CultureInfo("tr-TR");

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/gunluknakit/tanim", Tanim);
            app.MapGet("/api/gunluknakit/liste", Liste);
            app.MapGet("/api/gunluknakit/detay", Detay);
            app.MapPost("/api/gunluknakit/kaydet", Kaydet);
            app.MapGet("/api/gunluknakit/rapor", Rapor);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdresi);
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

        private static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata });
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static bool TarihCoz(string metin, out DateTime tarih)
        {
            return DateTime.TryParseExact((metin ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tarih);
        }

        /// <summary>
        /// İş kuralının beklediği "muhasebe.birim.kurum" anahtarı (eski Select2'nin MuhasebeBirimKurum değeri). Kurumsal kod birimin
        /// MUHMUHASEBEBIRIM kaydından alınır (kayıtlar KURKOD ile birimlere bağlanır). Birim bulunamazsa null.
        /// </summary>
        private static string BirimAnahtar(int muhasebe, string birim)
        {
            birim = (birim ?? "").Trim();
            if (muhasebe <= 0 || birim == "") return null;
            string kurum;
            MuhasebeBirimUclari.BirimAdBul(muhasebe, birim, out kurum);
            if (string.IsNullOrWhiteSpace(kurum)) return null;
            return muhasebe + "." + birim + "." + kurum;
        }

        /// <summary>Eski VeriGirilirMi: hafta sonu değilse ve bugünden sonraysa o güne giriş yapılır.</summary>
        private static bool Girilir(DateTime gun)
        {
            gun = gun.Date;
            if (gun.DayOfWeek == DayOfWeek.Saturday || gun.DayOfWeek == DayOfWeek.Sunday) return false;
            return gun > DateTime.Today;
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Açılış (eski TarihAyarla + birim çerezi): tarih yarın; birim, kullanıcı değişkenlerindeki muhasebe birimi.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                string muhasebe = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBE");
                string birim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEBIRIM");
                string kurum;
                string birimAd = MuhasebeBirimUclari.BirimAdBul(OrtakFonksiyonlar.ConvertToInt(muhasebe, 0), birim, out kurum);

                List<object> turler = new List<object>();
                foreach (var t in Turler) turler.Add(new { kod = t.Kod, ad = t.Ad });

                return Results.Json(new
                {
                    tarih = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"),
                    muhasebe = muhasebe, birim = birim, birimAd = birimAd,
                    kurumsal = MuhasebeBirimUclari.KurumsalYaz(DateTime.Now.Year, kurum),
                    turler = turler
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Listele (eski gunlukListeOlustur): tarihin 5 iş günü öncesinden 5 iş günü sonrasına günlük toplamlar.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string tarih, int? muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!TarihCoz(tarih, out DateTime t)) return Hata("Lütfen tarih giriniz!");
            string anahtar = BirimAnahtar(muhasebe ?? 0, birim);
            if (anahtar == null) return Hata("Lütfen birim seçiniz!");
            try
            {
                ObjectArray oa = TNS.BUM.Arac.Tanimla().ListeleGunlukNakitIhtiyac(kullanan, new TNSDateTime(t), anahtar, true);
                if (oa == null || !oa.sonuc.islemSonuc) return Hata(HataMetni(oa?.sonuc.hataStr, "Listeleme yapılamadı!"));

                List<object> satirlar = new List<object>();
                if (oa.ObjeSayisi == 1)
                {
                    foreach (GunDeger gun in ((NakitPlanGrup)oa.objeler[0]).GunMiktarlari)
                    {
                        DateTime g = gun.TarihDT.Date;
                        satirlar.Add(new
                        {
                            tarih = g.ToString("yyyy-MM-dd"),
                            gun = Tr.DateTimeFormat.GetDayName(g.DayOfWeek),
                            toplam = gun.Toplam,
                            girilir = Girilir(g)
                        });
                    }
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Listeleme yapılamadı!");
            }
        }

        /// <summary>Detay penceresi açılınca (eski GunlukNakitDetayGetir): günün harcama türlerine göre tutarları.</summary>
        private static IResult Detay(HttpContext baglam, IWebHostEnvironment ortam, string tarih, int? muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!TarihCoz(tarih, out DateTime t)) return Hata("Tarih hatalı!");
            string anahtar = BirimAnahtar(muhasebe ?? 0, birim);
            if (anahtar == null) return Hata("Lütfen birim seçiniz!");
            try
            {
                ObjectArray oa = TNS.BUM.Arac.Tanimla().ListeleGunlukNakitIhtiyac(kullanan, new TNSDateTime(t), anahtar, false);
                if (oa == null || !oa.sonuc.islemSonuc) return Hata(HataMetni(oa?.sonuc.hataStr, "Listeleme yapılamadı!"));

                List<NakitTutar> tutarlar = new List<NakitTutar>();
                if (oa.ObjeSayisi == 1)
                {
                    NakitPlanGrup grup = (NakitPlanGrup)oa.objeler[0];
                    if (grup.GunMiktarlari.Count == 1)
                        foreach (GunDegerHarcamaTur h in ((GunDeger)grup.GunMiktarlari[0]).harcamaTutarlari)
                            tutarlar.Add(new NakitTutar { HarcamaTur = h.harcamaTur, Tutar = h.tutar });
                }
                return Results.Json(new { tutarlar = tutarlar });
            }
            catch (Exception)
            {
                return Hata("Listeleme yapılamadı!");
            }
        }

        // ------------------------------------------------------------------ kayıt

        /// <summary>
        /// Kaydet (eski KaydetGunlukNakitIhtiyac): günün kayıtları silinip sıfırdan büyük tutarlar yazılır (iş kuralı). Eski sayfada
        /// yalnızca tarayıcıda denetlenen kural (hafta sonu ve bugün / geçmiş günler girilmez) burada da denetlenir.
        /// </summary>
        private static IResult Kaydet(HttpContext baglam, IWebHostEnvironment ortam, NakitKayitIstegi istek)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Hata("Seçili kayıt yok");
            if (!TarihCoz(istek.Tarih, out DateTime t)) return Hata("Tarih hatalı!");
            if (!Girilir(t)) return Hata("Bu güne giriş yapılamaz (hafta sonu ya da geçmiş / bugünkü tarih).");
            string anahtar = BirimAnahtar(istek.Muhasebe, istek.Birim);
            if (anahtar == null) return Hata("Lütfen birim seçiniz!");

            List<GunDegerHarcamaTur> tutarlar = new List<GunDegerHarcamaTur>();
            double toplam = 0;
            foreach (var tur in Turler)
            {
                NakitTutar n = istek.Tutarlar?.Find(x => x != null && x.HarcamaTur == tur.Kod);
                double tutar = n == null ? 0 : Math.Round(n.Tutar, 2);
                if (tutar < 0 || double.IsNaN(tutar) || double.IsInfinity(tutar)) return Hata(tur.Ad + " tutarı hatalı!");
                tutarlar.Add(new GunDegerHarcamaTur(tur.Kod, tutar));
                toplam += tutar;
            }

            try
            {
                Sonuc s = TNS.BUM.Arac.Tanimla().KaydetGunlukNakitIhtiyac(kullanan, new TNSDateTime(t), anahtar, tutarlar.ToArray());
                if (s == null || !s.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(s?.hataStr, "Kayıt işlemi yapılamadı!") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt başarılı", toplam = Math.Round(toplam, 2) });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi yapılamadı!" });
            }
        }

        // ------------------------------------------------------------------ rapor

        /// <summary>Yazdır (eski yazdirGunlukNakitIhtiyaci): seçilen tarih çevresinde bütün birimlerin günlük toplamları, Excel.</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, string tarih)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!TarihCoz(tarih, out DateTime t)) return Hata("Lütfen tarih giriniz!");
            string sablon = GunlukNakitIhtiyaciRaporu.SablonBul(ortam.ContentRootPath);
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (BUM\\" + GunlukNakitIhtiyaciRaporu.SablonAd + ").");
            try
            {
                AsposeLisans.Yukle();
                // Eski sayfa da birimKod olarak "0" gönderiyordu: bütün birimler.
                string dosya = GunlukNakitIhtiyaciRaporu.Yazdir(kullanan, t, "0", sablon);
                string gidenAd = "GunlukNakitIhtiyaci_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                return DosyaIslem.DosyaGonder(dosya, gidenAd, true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
