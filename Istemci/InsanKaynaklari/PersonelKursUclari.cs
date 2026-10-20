using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Hizmet İçi Eğitim Kaydet / Sil isteği: alan adları PersonelKurs sınıfındaki gibi.</summary>
    public class PersonelKursIstegi
    {
        public int personelKod { get; set; }
        public string kod { get; set; }
        public string kursKod { get; set; }
        public string kursAdi { get; set; }
        public string baslamaTarihi { get; set; }
        public string bitirmeTarihi { get; set; }
        public string aldigiYer { get; set; }
        public string egitimYeri { get; set; }
        public string gun { get; set; }
        public string dersSaati { get; set; }
        public string yil { get; set; }
        public string donem { get; set; }
        public string derece { get; set; }
        public string puan { get; set; }
        public string dosyaNo { get; set; }
        public string aciklama { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri > Hizmet İçi Eğitim sekmesi (eski InsanKaynaklari/PersonelKurs.aspx; yeni sayfa InsanKaynaklari/PersonelKurs):
    ///   GET  /api/personelbilgi/kurs/secenekler                 (eski KursDoldur, OgrenimYeriDoldur, yıl listesi)
    ///   GET  /api/personelbilgi/kurs/liste?personelKod=         (eski Listele)
    ///   POST /api/personelbilgi/kurs/kaydet                     (eski btnKaydet_Click)
    ///   POST /api/personelbilgi/kurs/sil                        (eski btnSil_Click)
    ///   GET  /api/personelbilgi/kurs/belgeler?personelKod=&amp;kod=          (eski BelgeListesiDoldur)
    ///   POST /api/personelbilgi/kurs/belgeyukle                 (eski BelgeKaydet; çok parçalı form)
    ///   GET  /api/personelbilgi/kurs/belgeindir?personelKod=&amp;kod=&amp;belgeKod=   (eski BelgeGoster)
    ///   POST /api/personelbilgi/kurs/belgesil                   (eski BelgeSil)
    /// İş kuralları eski sayfadaki gibi IIKYServis.PersonelKursListele / Kaydet / Sil, SozlukListele, PersonelBelgeXxx.
    /// </summary>
    public static class PersonelKursUclari
    {
        private const string SayfaAd = "PersonelKurs";
        private const ENUMBELGEFORMTIP BelgeTip = ENUMBELGEFORMTIP.PERSONELKURS;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/kurs/secenekler", Secenekler);
            app.MapGet("/api/personelbilgi/kurs/liste", Liste);
            app.MapPost("/api/personelbilgi/kurs/kaydet", Kaydet);
            app.MapPost("/api/personelbilgi/kurs/sil", Sil);
            app.MapGet("/api/personelbilgi/kurs/belgeler", (HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod) =>
                PersonelEgitimOrtak.BelgeListesi(baglam, ortam, SayfaAd, BelgeTip, personelKod, kod));
            app.MapPost("/api/personelbilgi/kurs/belgeyukle", (HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik) =>
                PersonelEgitimOrtak.BelgeYukle(baglam, ortam, sahtecilik, SayfaAd, BelgeTip)).DisableAntiforgery();
            app.MapGet("/api/personelbilgi/kurs/belgeindir", (HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod, int? belgeKod) =>
                PersonelEgitimOrtak.BelgeIndir(baglam, ortam, SayfaAd, BelgeTip, personelKod, kod, belgeKod));
            app.MapPost("/api/personelbilgi/kurs/belgesil", (HttpContext baglam, IWebHostEnvironment ortam, PersonelBelgeSilIstegi istek, IAntiforgery sahtecilik) =>
                PersonelEgitimOrtak.BelgeSil(baglam, ortam, sahtecilik, SayfaAd, BelgeTip, istek));
        }

        /// <summary>
        /// Eski KursDoldur (sözlük 19) ve OgrenimYeriDoldur (sözlük 12). Eski Page_Load yıl listesini 2000'den bu yıla kadar
        /// dolduruyordu; yeni ekranda yıl sayı kutusu olduğu için alt / üst sınır döner.
        /// </summary>
        private static IResult Secenekler(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string hata = "";
            return Results.Json(new
            {
                kurs = PersonelEgitimOrtak.SozlukSecenekleri(ENUMSozlukUnsurlari.KURSKOD, false, true, ref hata),
                egitimYeri = PersonelEgitimOrtak.SozlukSecenekleri(ENUMSozlukUnsurlari.OGRENIMYERI, false, true, ref hata),
                yilEnAz = 2000,
                yilEnCok = System.DateTime.Now.Year,
                hata = hata
            });
        }

        /// <summary>Eski Listele(new PersonelKurs()).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> satirlar = new List<object>();
            int pKod = personelKod ?? 0;
            if (pKod <= 0) return Results.Json(new { satirlar = satirlar });

            TNS.IKY.PersonelKurs kriter = new TNS.IKY.PersonelKurs();
            kriter.personelKod = pKod;
            ObjectArray pler = TNS.IKY.Arac.Tanimla().PersonelKursListele(kullanan, kriter);
            if (!pler.sonuc.islemSonuc)
                return Results.Json(new { satirlar = satirlar, hata = PersonelBilgiOrtak.HataMetni(pler.sonuc.hataStr, "Eğitim bilgileri okunamadı.") });

            foreach (TNS.IKY.PersonelKurs p in pler.objeler)
                satirlar.Add(new
                {
                    kod = p.kod,
                    personelKod = p.personelKod,
                    kursKod = p.kursKod,
                    tarih = PersonelBilgiOrtak.TarihYaz(p.tarih),
                    aldigiYer = p.aldigiYer,
                    baslamaTarihi = PersonelBilgiOrtak.TarihYaz(p.baslamaTarihi),
                    bitirmeTarihi = PersonelBilgiOrtak.TarihYaz(p.bitirmeTarihi),
                    puan = PersonelEgitimOrtak.OndalikYaz(p.puan),
                    derece = p.derece,
                    kursAdi = p.kursAdi,
                    dersSaati = p.dersSaati,
                    egitimYeri = p.egitimYeri,
                    yil = p.yil,
                    donem = p.donem,
                    gun = p.gun,
                    dosyaNo = p.dosyaNo,
                    aciklama = p.aciklama
                });
            return Results.Json(new { satirlar = satirlar });
        }

        /// <summary>Eski btnKaydet_Click. Başarıda kayıt kodu döner (eski hdnKod = sonuc.anahtar).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelKursIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string hata = "";
            int kursKod = OrtakFonksiyonlar.ConvertToInt(istek.kursKod, 0);
            string kursAdi = istek.kursAdi ?? "";
            // Eski: Adı boşsa ve eğitim tipi seçiliyse Adı = seçilen eğitim tipinin metni (sayfa da aynısını yapar; burada yedek).
            if (kursAdi.Trim() == "" && kursKod != 0)
                kursAdi = PersonelEgitimOrtak.SozlukAd(kursKod, null);
            if (kursAdi.Trim() == "")
                hata += "Adı bilgisi boş bırakılamaz.\n";
            if (hata != "") return Results.Json(new { basarili = false, hata = hata.Trim() });

            TNS.IKY.PersonelKurs k = new TNS.IKY.PersonelKurs();
            k.personelKod = istek.personelKod;
            int kod = OrtakFonksiyonlar.ConvertToInt((istek.kod ?? "").Trim(), 0);
            if (kod > 0)
                k.kod = kod;
            k.kursKod = kursKod;
            k.tarih = new TNSDateTime();
            k.aldigiYer = istek.aldigiYer ?? "";
            k.baslamaTarihi = PersonelBilgiOrtak.TnsTarih(istek.baslamaTarihi);
            k.bitirmeTarihi = PersonelBilgiOrtak.TnsTarih(istek.bitirmeTarihi);
            k.puan = PersonelEgitimOrtak.Ondalik(istek.puan);
            k.derece = istek.derece ?? "";
            k.kursAdi = kursAdi;
            k.dersSaati = OrtakFonksiyonlar.ConvertToInt((istek.dersSaati ?? "").Trim(), 0);
            k.egitimYeri = OrtakFonksiyonlar.ConvertToInt(istek.egitimYeri, 0);
            k.yil = OrtakFonksiyonlar.ConvertToInt((istek.yil ?? "").Trim(), 0);
            k.donem = istek.donem ?? "";
            k.gun = OrtakFonksiyonlar.ConvertToInt((istek.gun ?? "").Trim(), 0);
            k.dosyaNo = istek.dosyaNo ?? "";
            k.aciklama = istek.aciklama ?? "";

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelKursKaydet(kullanan, k);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Eğitim bilgisi kaydedilemedi.") });
            return Results.Json(new { basarili = true, kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0), kursAdi = kursAdi, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        /// <summary>Eski btnSil_Click.</summary>
        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, PersonelKursIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            int kod = OrtakFonksiyonlar.ConvertToInt((istek.kod ?? "").Trim(), 0);
            int pKod = istek.personelKod;
            if (pKod <= 0 || kod <= 0)
                return Results.Json(new { basarili = false, hata = "Lütfen silmek istediğiniz kaydı seçiniz." });

            TNS.IKY.PersonelKurs d = new TNS.IKY.PersonelKurs();
            d.personelKod = pKod;
            d.kod = kod;
            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelKursSil(kullanan, d);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Eğitim bilgisi silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla gerçekleşti." });
        }
    }
}
