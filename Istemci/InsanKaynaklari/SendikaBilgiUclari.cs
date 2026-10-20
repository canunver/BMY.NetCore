using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Kaydet isteği: alan adları SendikaBilgi sınıfındaki gibi.</summary>
    public class SendikaKaydetIstegi
    {
        public int sendikaNo { get; set; }
        public string sendikaAd { get; set; }
        public string adres { get; set; }
        public string iBAN { get; set; }
        public string hesapNo { get; set; }
        public string topluGorusmePuani { get; set; }
        public double kesintiOrani { get; set; }
    }

    /// <summary>Personel ekleme / çıkarma / oran isteği.</summary>
    public class SendikaPersonelIstegi
    {
        public int sendikaNo { get; set; }
        public int pkod { get; set; }
        public List<int> kodlar { get; set; }
        public double kesintiOrani { get; set; }
    }

    /// <summary>
    /// Sendika Bilgisi (eski InsanKaynaklari/SendikaBilgiForm.aspx; yeni sayfa InsanKaynaklari/SendikaBilgiForm) uçları:
    ///   GET  /api/sendika/liste                 sendika listesi (eski Listele)
    ///   GET  /api/sendika/oku?sendikaNo=        seçilen sendika ve personeli (eski SendikaSecildiIc)
    ///   POST /api/sendika/kaydet                (eski Kaydet)
    ///   POST /api/sendika/personel-ekle         seçilen personeli kesinti oranıyla sendikaya ekler (eski KisileriEkle)
    ///   POST /api/sendika/personel-cikar        (eski PersonelCikart)
    ///   POST /api/sendika/oran-ata              seçili personelin kesinti oranını değiştirir (eski btnKisiDegerAtaClick)
    /// İş kuralları eski sayfadaki gibi IIKYServis.ListeleSendikaBilgi / OkuSendikaBilgi / KaydetSendikaBilgi / PersonelSendikayaEkle /
    /// PersonelSendikadanCikart / PersonelSendikaOranDegistir.
    /// </summary>
    public static class SendikaBilgiUclari
    {
        private const string Uc = "/api/sendika/";
        public const string SayfaAdresi = "../InsanKaynaklari/SendikaBilgiForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "oku", Oku);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapPost(Uc + "personel-ekle", PersonelEkle);
            app.MapPost(Uc + "personel-cikar", PersonelCikar);
            app.MapPost(Uc + "oran-ata", OranAta);
        }

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

        private static IIKYServis Servis() { return TNS.IKY.Arac.Tanimla(); }

        private static IResult Basarisiz(string hata) { return Results.Json(new { basarili = false, hata }); }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray oa = Servis().ListeleSendikaBilgi(kullanan, new SendikaBilgi());
                if (!oa.sonuc.islemSonuc) return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Sendikalar okunamadı.") });
                List<object> satirlar = new List<object>();
                foreach (object o in oa.objeler)
                    if (o is SendikaBilgi s) satirlar.Add(new { s.sendikaNo, s.sendikaAd });
                return Results.Json(new { satirlar });
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(ex.Message, "Sendikalar okunamadı.") });
            }
        }

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? sendikaNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                SendikaBilgi form = new SendikaBilgi();
                form.sendikaNo = sendikaNo ?? 0;
                ObjectVAN ov = Servis().OkuSendikaBilgi(kullanan, form);
                if (!ov.sonuc.islemSonuc) return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(ov.sonuc.hataStr, "Sendika bilgisi okunamadı.") });
                if (!(ov.o is SendikaBilgi s)) return Results.Json(new { hata = "Verilen kriterlere uygun kayıt bulunamadı." });

                List<object> personel = new List<object>();
                if (s.sendikaPersonel != null)
                    foreach (object o in s.sendikaPersonel)
                        if (o is PersonelSendika p) personel.Add(new { p.kod, p.tcKimlikNo, p.ad, p.soyad, p.sicilNo, p.kesintiOrani });

                return Results.Json(new
                {
                    bilgi = new { s.sendikaNo, s.sendikaAd, s.adres, s.iBAN, s.hesapNo, s.topluGorusmePuani, s.kesintiOrani },
                    personel
                });
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(ex.Message, "Sendika bilgisi okunamadı.") });
            }
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, SendikaKaydetIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            SendikaBilgi form = new SendikaBilgi();
            form.sendikaNo = istek.sendikaNo;
            form.sendikaAd = istek.sendikaAd ?? "";
            form.adres = istek.adres ?? "";
            form.iBAN = istek.iBAN ?? "";
            form.hesapNo = istek.hesapNo ?? "";
            form.topluGorusmePuani = OrtakFonksiyonlar.ConvertToInt((istek.topluGorusmePuani ?? "").Trim(), 0);
            form.kesintiOrani = istek.kesintiOrani;
            Sonuc sonuc = Servis().KaydetSendikaBilgi(kullanan, form);
            if (!sonuc.islemSonuc) return Basarisiz(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Sendika bilgisi kaydedilemedi."));

            TNS.IKY.Arac.SendikaListesi = null;
            int no = form.sendikaNo != 0 ? form.sendikaNo : OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0);
            return Results.Json(new { basarili = true, sendikaNo = no, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        private static async Task<IResult> PersonelEkle(HttpContext baglam, IWebHostEnvironment ortam, SendikaPersonelIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            TNSCollection pKodList = new TNSCollection();
            foreach (int k in istek.kodlar ?? new List<int>()) pKodList.Add(k);
            Sonuc sonuc = Servis().PersonelSendikayaEkle(kullanan, istek.sendikaNo, pKodList, istek.kesintiOrani, true);
            if (!sonuc.islemSonuc) return Basarisiz(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Personel sendikaya eklenemedi."));
            return Results.Json(new { basarili = true });
        }

        private static async Task<IResult> PersonelCikar(HttpContext baglam, IWebHostEnvironment ortam, SendikaPersonelIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            Sonuc sonuc = Servis().PersonelSendikadanCikart(kullanan, istek.pkod);
            if (!sonuc.islemSonuc) return Basarisiz(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Personel sendikadan çıkarılamadı."));
            return Results.Json(new { basarili = true });
        }

        private static async Task<IResult> OranAta(HttpContext baglam, IWebHostEnvironment ortam, SendikaPersonelIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            Sonuc sonuc = Servis().PersonelSendikaOranDegistir(kullanan, istek.pkod, istek.kesintiOrani);
            if (!sonuc.islemSonuc) return Basarisiz(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Kesinti oranı değiştirilemedi."));
            return Results.Json(new { basarili = true });
        }
    }
}
