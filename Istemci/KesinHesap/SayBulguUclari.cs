using System;
using System.Collections.Generic;
using System.Globalization;
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
using TNS.KHM;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.KesinHesap
{
    /// <summary>Bulgu kaydı (eski readOnBulguFromModal); tarih yyyy-MM-dd ya da boş.</summary>
    public class SayBulguKayitIstegi
    {
        public int BulguID { get; set; }
        public int Yil { get; set; }
        public int BitisYil { get; set; }
        public int ButceTurKod { get; set; }
        public int BulguTurKod { get; set; }
        public int DurumKod { get; set; }
        public string BulguNo { get; set; }
        public string OngorulenTamamlanmaTarihi { get; set; }
        public bool YerineGetirildi { get; set; }
        public string BulguBasligi { get; set; }
        public string BulguOzeti { get; set; }
        public string YapilacakFaaliyetler { get; set; }
        public string IlgiliBirimKurum { get; set; }
        public string GorevOrtagi { get; set; }
        public string BulguDayanagi { get; set; }
        public string IdareCevabi { get; set; }
        public string IdareCevapOzeti { get; set; }
        public string SayistayOnerisi { get; set; }
    }

    /// <summary>Birimin bulguya cevabı (eski readOnBulguCevapFromModal).</summary>
    public class SayBulguCevapIstegi
    {
        public int BulguID { get; set; }
        public int Yil { get; set; }
        public string Birim { get; set; }
        public string BirimCevap { get; set; }
        public string BirimIsler { get; set; }
        public string OngorulenTamamlanmaTarihi { get; set; }
        public bool YerineGetirildi { get; set; }
    }

    /// <summary>Bulgu ile birim bağı: Ekle true ise kurulur, false ise silinir (eski BirimBagiKur / BirimBagiSil).</summary>
    public class SayBulguBirimBagIstegi
    {
        public int BulguID { get; set; }
        public string Birim { get; set; }
        public bool Ekle { get; set; }
    }

    /// <summary>Birim giriş izni (eski BirimGirisIzniKaydet): seçili birimler veri girişine açılır ya da kapatılır.</summary>
    public class SayBulguIzinIstegi
    {
        public int Yil { get; set; }
        public bool Izin { get; set; }
        public List<string> Birimler { get; set; }
    }

    /// <summary>
    /// Sayıştay Bulguları (eski SayistayBulgu/SayBulgu.aspx) ve Sayıştay Ön Bulgu Cevapları (eski SayistayBulgu/SayBulguCevap.aspx)
    /// sayfalarının uçları; eski sayfa yöntemlerinin (WebMethod) karşılığıdır:
    ///   GET  /api/saybulgu/kodlar, /liste?yil=&amp;durumKod=, /oku?id=, /birimara?ad=, /iliskili?bulguId=, /izinler?yil=
    ///   POST /api/saybulgu/kaydet, /birimbag, /izinkaydet                       (Bulgu sayfası yetkisi)
    ///   GET  /api/saybulgu/birimler?arama=, /cevaplar?yil=&amp;birim=; POST /cevapkaydet (Cevap sayfası yetkisi; birimler her ikisi)
    ///   GET  /api/saybulgu/rapor?raporNo=&amp;yil=&amp;durumKod=&amp;birim= (eski SAYRaporAl.aspx; KHM\SAYBulguListe.xltx, Excel)
    /// İşlemler eskisi gibi TNS.KHM.Arac.Tanimla() ve TNS.MUH.Arac.Tanimla() üzerinden yapılır; birim yetkileri iş kurallarındadır.
    /// </summary>
    public static class SayBulguUclari
    {
        public const string BulguAdres = "../SayistayBulgu/SayBulgu.aspx";
        public const string CevapAdres = "../SayistayBulgu/SayBulguCevap.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/saybulgu/kodlar", Kodlar);
            app.MapGet("/api/saybulgu/liste", Liste);
            app.MapGet("/api/saybulgu/oku", Oku);
            app.MapPost("/api/saybulgu/kaydet", Kaydet);
            app.MapGet("/api/saybulgu/birimara", BirimAra);
            app.MapGet("/api/saybulgu/iliskili", Iliskili);
            app.MapPost("/api/saybulgu/birimbag", BirimBag);
            app.MapGet("/api/saybulgu/izinler", Izinler);
            app.MapPost("/api/saybulgu/izinkaydet", IzinKaydet);
            app.MapGet("/api/saybulgu/birimler", Birimler);
            app.MapGet("/api/saybulgu/cevaplar", Cevaplar);
            app.MapPost("/api/saybulgu/cevapkaydet", CevapKaydet);
            app.MapGet("/api/saybulgu/rapor", Rapor);
        }

        public static bool BulguYetkisi(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, BulguAdres);
        }

        public static bool CevapYetkisi(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, CevapAdres);
        }

        /// <summary>sayfa: 1 Bulgu sayfası, 2 Cevap sayfası, 0 ikisinden biri.</summary>
        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, int sayfa, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            string klasor = ortam.ContentRootPath;
            bool yetkili = sayfa == 1 ? BulguYetkisi(kullanan, klasor)
                         : sayfa == 2 ? CevapYetkisi(kullanan, klasor)
                         : BulguYetkisi(kullanan, klasor) || CevapYetkisi(kullanan, klasor);
            if (!yetkili) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IKHMServis Servis()
        {
            return TNS.KHM.Arac.Tanimla();
        }

        private static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata });
        }

        private static IResult SonucDon(Sonuc s, string varsayilan)
        {
            if (s == null) return Results.Json(new { basarili = false, hata = varsayilan });
            if (!s.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(s.hataStr, varsayilan) });
            return Results.Json(new { basarili = true, anahtar = s.anahtar });
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string Tarih(TNSDateTime t)
        {
            return TNSDateTime.IsNull(t) ? "" : t.Oku().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>yyyy-MM-dd metnini tarihe çevirir; boşsa ya da hatalıysa tarih boş kalır.</summary>
        private static TNSDateTime TarihOl(string deger)
        {
            if (DateTime.TryParseExact((deger ?? "").Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return new TNSDateTime();
        }

        /// <summary>Bulgunun ekrana giden alanları (eski sayfanın JSON'undaki adlarla).</summary>
        private static object Satir(SayBulgu b)
        {
            return new
            {
                bulguID = b.bulguID,
                yil = b.yil,
                bitisYil = b.bitisYil,
                butceTurKod = (int)b.butceTurKod,
                bulguTurKod = (int)b.bulguTurKod,
                bulguTurAd = b.bulguTurAd,
                durumKod = (int)b.durumKod,
                durumAd = b.durumAd,
                bulguNo = b.bulguNo ?? "",
                bulguBasligi = b.bulguBasligi ?? "",
                bulguOzeti = b.bulguOzeti ?? "",
                bulguDayanagi = b.bulguDayanagi ?? "",
                idareCevabi = b.idareCevabi ?? "",
                idareCevapOzeti = b.idareCevapOzeti ?? "",
                sayistayOnerisi = b.sayistayOnerisi ?? "",
                yerineGetirildi = b.yerineGetirildi,
                yapilacakFaaliyetler = b.yapilacakFaaliyetler ?? "",
                ilgiliBirimKurum = b.ilgiliBirimKurum ?? "",
                gorevOrtagi = b.gorevOrtagi ?? "",
                ongorulenTamamlanmaTarihi = Tarih(b.ongorulenTamamlanmaTarihi),
                birimCevap = b.birimCevap ?? "",
                birimIsler = b.birimIsler ?? "",
                yerineGetirildiCevap = b.yerineGetirildiCevap,
                ongorulenTamamlanmaTarihiCevap = Tarih(b.ongorulenTamamlanmaTarihiCevap),
                muhasebeBirimAd = b.muhasebeBirimAd ?? ""
            };
        }

        private static object BirimSatir(MuhasebeBirim m)
        {
            return new { kod = m.MuhasebeBirimKurum, ad = m.MuhasebeBirimAd, kodAd = m.MuhasebeBirimKodAd };
        }

        private static List<object> BirimListesi(ObjectArray oa)
        {
            List<object> liste = new List<object>();
            if (oa != null && oa.objeler != null)
                foreach (object o in oa.objeler)
                    if (o is MuhasebeBirim m) liste.Add(BirimSatir(m));
            return liste;
        }

        // ------------------------------------------------------------------ Bulgu sayfası

        /// <summary>Eski GetKodListesi (tip 1 bütçe türleri, 2 bulgu türleri, 3 durumlar, 4 sorgu durumları: toplular önce).</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, 0, out _);
            if (ret != null) return ret;

            List<object> butce = new List<object>();
            foreach (enumBulguButceTur v in Enum.GetValues(typeof(enumBulguButceTur)))
                if ((int)v > 0 && (int)v < 99) butce.Add(new { kod = (int)v, ad = SayBulgu.ButceTurAdBul(v) });

            List<object> tur = new List<object>();
            foreach (enumBulguTur v in Enum.GetValues(typeof(enumBulguTur)))
                if ((int)v > 0 && (int)v < 99) tur.Add(new { kod = (int)v, ad = SayBulgu.BulguTurAdBul(v) });

            List<object> durum = new List<object>();
            List<object> toplu = new List<object>();
            foreach (enumBulguDurum v in Enum.GetValues(typeof(enumBulguDurum)))
            {
                int kod = (int)v;
                if (kod >= 100) toplu.Add(new { kod = kod, ad = SayBulgu.DurumAdBul(v) });
                else if (kod > 0) durum.Add(new { kod = kod, ad = SayBulgu.DurumAdBul(v) });
            }
            List<object> sorgu = new List<object>(toplu);
            sorgu.AddRange(durum);

            return Results.Json(new { yil = DateTime.Now.Year - 1, butceTurleri = butce, bulguTurleri = tur, durumlar = durum, sorguDurumlari = sorgu });
        }

        /// <summary>Eski ListeleOnBulgular.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, int? durumKod)
        {
            IResult ret = Denetle(baglam, ortam, 1, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                SayBulgu kriter = new SayBulgu { yil = yil, durumKod = (enumBulguDurum)(durumKod ?? (int)enumBulguDurum.AcikBulgular) };
                ObjectArray<SayBulgu> oa = Servis().ListeleOnBulgu(kullanan, kriter);
                if (oa == null || oa.sonuc == null || !oa.sonuc.islemSonuc)
                    return Hata(HataMetni(oa?.sonuc?.hataStr, "Listeleme yapılamadı!"));
                List<object> satirlar = new List<object>();
                if (oa.objeler != null)
                    foreach (SayBulgu b in oa.objeler) satirlar.Add(Satir(b));
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Listeleme yapılamadı!"));
            }
        }

        /// <summary>Eski OnBulguGetir.</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int id)
        {
            IResult ret = Denetle(baglam, ortam, 1, out Kullanici kullanan);
            if (ret != null) return ret;
            if (id <= 0) return Hata("Kayıt bulunamadı.");
            try
            {
                ObjectVAN<SayBulgu> ov = Servis().OkuOnBulgu(kullanan, new SayBulgu { bulguID = id });
                if (ov == null || ov.sonuc == null || !ov.sonuc.islemSonuc)
                    return Hata(HataMetni(ov?.sonuc?.hataStr, "Kayıt bulunamadı."));
                if (ov.o == null) return Hata("Kayıt bulunamadı.");
                return Results.Json(new { bulgu = Satir(ov.o) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kayıt okunamadı."));
            }
        }

        /// <summary>Eski KaydetOnBulgu: yeni kayıtta anahtar yeni bulgunun numarasıdır.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, SayBulguKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, 1, out Kullanici kullanan);
            if (ret != null) return ret;
            SayBulgu b = new SayBulgu
            {
                bulguID = istek.BulguID,
                yil = istek.Yil,
                bitisYil = istek.BitisYil,
                butceTurKod = (enumBulguButceTur)istek.ButceTurKod,
                bulguTurKod = (enumBulguTur)istek.BulguTurKod,
                durumKod = istek.DurumKod > 0 ? (enumBulguDurum)istek.DurumKod : enumBulguDurum.OnTaslakBulgu,
                bulguNo = istek.BulguNo ?? "",
                ongorulenTamamlanmaTarihi = TarihOl(istek.OngorulenTamamlanmaTarihi),
                yerineGetirildi = istek.YerineGetirildi,
                bulguBasligi = istek.BulguBasligi ?? "",
                bulguOzeti = istek.BulguOzeti ?? "",
                yapilacakFaaliyetler = istek.YapilacakFaaliyetler ?? "",
                ilgiliBirimKurum = istek.IlgiliBirimKurum ?? "",
                gorevOrtagi = istek.GorevOrtagi ?? "",
                bulguDayanagi = istek.BulguDayanagi ?? "",
                idareCevabi = istek.IdareCevabi ?? "",
                idareCevapOzeti = istek.IdareCevapOzeti ?? "",
                sayistayOnerisi = istek.SayistayOnerisi ?? ""
            };
            try
            {
                return SonucDon(Servis().KaydetOnBulgu(kullanan, b), "Kaydedilemedi.");
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = HataMetni(ex.Message, "Kaydedilemedi.") });
            }
        }

        /// <summary>Eski BirimAra: adına / koduna göre aktif muhasebe birimleri (İlgili Birimler penceresindeki arama).</summary>
        private static IResult BirimAra(HttpContext baglam, IWebHostEnvironment ortam, string ad)
        {
            IResult ret = Denetle(baglam, ortam, 1, out _);
            if (ret != null) return ret;
            try
            {
                ObjectArray oa = TNS.MUH.Arac.Tanimla().MuhasebeBirimListele(new MuhasebeBirim { aramaKosul = ad ?? "", pasif_0Hepsi_1Aktif_2Pasif = 1 });
                if (oa == null || oa.sonuc == null || !oa.sonuc.islemSonuc)
                    return Hata(HataMetni(oa?.sonuc?.hataStr, "Birimler listelenemedi."));
                return Results.Json(new { birimler = BirimListesi(oa) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Birimler listelenemedi."));
            }
        }

        /// <summary>Eski IliskiliBirimleriListele.</summary>
        private static IResult Iliskili(HttpContext baglam, IWebHostEnvironment ortam, int bulguId)
        {
            IResult ret = Denetle(baglam, ortam, 1, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray<MuhasebeBirim> oa = Servis().BulguIliskiliBirimListele(kullanan, bulguId);
                if (oa == null || oa.sonuc == null || !oa.sonuc.islemSonuc)
                    return Hata(HataMetni(oa?.sonuc?.hataStr, "İlişkili birimler listelenemedi."));
                List<object> liste = new List<object>();
                if (oa.objeler != null)
                    foreach (MuhasebeBirim m in oa.objeler) liste.Add(BirimSatir(m));
                return Results.Json(new { birimler = liste });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İlişkili birimler listelenemedi."));
            }
        }

        /// <summary>Eski BirimBagiKur / BirimBagiSil.</summary>
        private static async Task<IResult> BirimBag(HttpContext baglam, IWebHostEnvironment ortam, SayBulguBirimBagIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null || istek.BulguID <= 0 || string.IsNullOrWhiteSpace(istek.Birim)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, 1, out Kullanici kullanan);
            if (ret != null) return ret;
            MuhasebeBirim mb = new MuhasebeBirim();
            mb.MuhasebeBirimKurum = istek.Birim.Trim();
            try
            {
                Sonuc s = istek.Ekle ? Servis().BulguBirimBagKur(kullanan, istek.BulguID, mb) : Servis().BulguBirimBagSil(kullanan, istek.BulguID, mb);
                return SonucDon(s, istek.Ekle ? "İşlem başarısız." : "Silme başarısız.");
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = HataMetni(ex.Message, "İşlem başarısız.") });
            }
        }

        /// <summary>Eski BirimGirisIzniListele (tür 1: bulgu).</summary>
        private static IResult Izinler(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, 1, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray<BirimGirisIzni> oa = Servis().BirimGirisIzniListele(kullanan, yil, 1);
                if (oa == null || oa.sonuc == null || !oa.sonuc.islemSonuc)
                    return Hata(HataMetni(oa?.sonuc?.hataStr, "Listeleme yapılamadı!"));
                List<object> liste = new List<object>();
                if (oa.objeler != null)
                    foreach (BirimGirisIzni i in oa.objeler)
                    {
                        MuhasebeBirim m = i.birim ?? new MuhasebeBirim();
                        liste.Add(new { kod = m.MuhasebeBirimKurum, ad = m.MuhasebeBirimAd, izin = i.girisIzni });
                    }
                return Results.Json(new { birimler = liste });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Listeleme yapılamadı!"));
            }
        }

        /// <summary>Eski BirimGirisIzniKaydet (tür 1: bulgu).</summary>
        private static async Task<IResult> IzinKaydet(HttpContext baglam, IWebHostEnvironment ortam, SayBulguIzinIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, 1, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.Birimler == null || istek.Birimler.Count == 0)
                return Results.Json(new { basarili = false, hata = "Lütfen en az bir birim seçiniz." });
            TNSCollection birimler = new TNSCollection();
            foreach (string kod in istek.Birimler)
            {
                if (string.IsNullOrWhiteSpace(kod)) continue;
                MuhasebeBirim mb = new MuhasebeBirim();
                mb.MuhasebeBirimKurum = kod.Trim();
                birimler.Add(mb);
            }
            try
            {
                return SonucDon(Servis().BirimGirisIzniKaydet(kullanan, istek.Yil, 1, istek.Izin, birimler), "İşlem gerçekleştirilemedi.");
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = HataMetni(ex.Message, "İşlem gerçekleştirilemedi.") });
            }
        }

        // ------------------------------------------------------------------ Cevap sayfası

        /// <summary>Eski BirimListele: birim seçim kutusundaki arama (aktif / pasif hepsi).</summary>
        private static IResult Birimler(HttpContext baglam, IWebHostEnvironment ortam, string arama)
        {
            IResult ret = Denetle(baglam, ortam, 0, out _);
            if (ret != null) return ret;
            try
            {
                ObjectArray oa = TNS.MUH.Arac.Tanimla().MuhasebeBirimListele(new MuhasebeBirim { aramaKosul = arama ?? "" });
                if (oa == null || oa.sonuc == null || !oa.sonuc.islemSonuc)
                    return Hata(HataMetni(oa?.sonuc?.hataStr, "Birimler listelenemedi."));
                return Results.Json(new { birimler = BirimListesi(oa) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Birimler listelenemedi."));
            }
        }

        /// <summary>Eski ListeleOnBulguCevaplar: birimin ilişkili olduğu bulgular ve birimin cevapları.</summary>
        private static IResult Cevaplar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim)
        {
            IResult ret = Denetle(baglam, ortam, 2, out Kullanici kullanan);
            if (ret != null) return ret;
            if (string.IsNullOrWhiteSpace(birim)) return Hata("Birim seçiniz.");
            try
            {
                ObjectArray<SayBulgu> oa = Servis().OnBulguCevapListele(kullanan, yil, birim.Trim());
                if (oa == null || oa.sonuc == null || !oa.sonuc.islemSonuc)
                    return Hata(HataMetni(oa?.sonuc?.hataStr, "Listeleme yapılamadı!"));
                List<object> satirlar = new List<object>();
                if (oa.objeler != null)
                    foreach (SayBulgu b in oa.objeler) satirlar.Add(Satir(b));
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Listeleme yapılamadı!"));
            }
        }

        /// <summary>Eski KaydetOnBulguCevap.</summary>
        private static async Task<IResult> CevapKaydet(HttpContext baglam, IWebHostEnvironment ortam, SayBulguCevapIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, 2, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.BulguID <= 0 || string.IsNullOrWhiteSpace(istek.Birim))
                return Results.Json(new { basarili = false, hata = "Birim ve bulgu seçiniz." });
            SayBulgu b = new SayBulgu
            {
                bulguID = istek.BulguID,
                yil = istek.Yil,
                birimCevap = istek.BirimCevap ?? "",
                birimIsler = istek.BirimIsler ?? "",
                ongorulenTamamlanmaTarihiCevap = TarihOl(istek.OngorulenTamamlanmaTarihi),
                yerineGetirildiCevap = istek.YerineGetirildi
            };
            b.MuhasebeBirimKurum = istek.Birim.Trim();
            try
            {
                return SonucDon(Servis().OnBulguCevapKaydet(kullanan, b), "Kaydedilemedi.");
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = HataMetni(ex.Message, "Kaydedilemedi.") });
            }
        }

        // ------------------------------------------------------------------ rapor (eski SAYRaporAl.aspx)

        /// <summary>
        /// Eski SAYRaporAl.aspx: raporNo 1 Liste Yazdır, 2 Cevaplı Liste Yazdır (Bulgu sayfası). Birim verilirse (Cevap sayfasındaki
        /// Yazdır) birimin cevapları listelenir. Şablon KHM\SAYBulguListe.xltx, eski RPRSayBulgular koduyla doldurulur.
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int raporNo, int yil, int? durumKod, string birim)
        {
            bool birimli = !string.IsNullOrWhiteSpace(birim);
            IResult ret = Denetle(baglam, ortam, birimli ? 0 : 1, out Kullanici kullanan);
            if (ret != null) return ret;
            if (raporNo != 1 && raporNo != 2) return Hata("Rapor bulunamadı.");
            string sablon = SayBulguRaporu.SablonBul(ortam.ContentRootPath);
            if (sablon == null) return Hata("Rapor şablonu (SAYBulguListe.xltx) bulunamadı.");
            try
            {
                AsposeLisans.Yukle();
                SayBulgu kriter = new SayBulgu
                {
                    yil = yil,
                    durumKod = (enumBulguDurum)(durumKod ?? (int)enumBulguDurum.AcikBulgular)
                };
                string dosya = new SayBulguRaporu(raporNo == 2).Yazdir(kullanan, kriter, birimli ? birim.Trim() : null, sablon);
                string gidecekAd = Path.GetFileNameWithoutExtension(sablon) + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                return DosyaIslem.DosyaGonder(dosya, gidecekAd, true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
