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
    /// <summary>Kart kaydet / sil isteği: alan adları PersonelKart sınıfındaki gibi; tarihler "yyyy-MM-dd".</summary>
    public class KartIstegi
    {
        public int kod { get; set; }
        public int personelKod { get; set; }
        public int kartTipi { get; set; }
        public string kartNo { get; set; }
        public int yakinligi { get; set; }
        public string adiSoyadi { get; set; }
        public string plaka { get; set; }
        public string tagCihazNo { get; set; }
        public string zimmetTarihi { get; set; }
        public string iadeTarihi { get; set; }
        public string aciklama { get; set; }
        public string kayipTarihi { get; set; }
        public int zimmetEdenPersonel { get; set; }
        public int iadeAlanPersonel { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri "Kart" sekmesi (eski InsanKaynaklari/PersonelKart.aspx; yeni sayfa InsanKaynaklari/PersonelKart) uçları:
    ///   GET  /api/personelbilgi/kart/secenekler                 Kart tipi (eski KartTipiDoldur) ve yakınlık (eski YakinlikDerecesiDoldur) seçenekleri
    ///   GET  /api/personelbilgi/kart/liste?personelKod=         (eski Listele)
    ///   GET  /api/personelbilgi/kart/oku?personelKod=&amp;kod=  (eski SatirSecildi)
    ///   POST /api/personelbilgi/kart/kaydet                     (eski btnKaydet_Click)
    ///   POST /api/personelbilgi/kart/sil                        (eski btnSil_Click)
    /// Zimmet eden / iade alan personel ortak personel seçimiyle seçilir (PersonelSecimUclari, js/personelsec.js).
    /// İş kuralları eski sayfadaki gibi IIKYServis.PersonelKartListele / Kaydet / Sil, SozlukListele, TNS.IKY.Arac.YakinlikListe.
    /// </summary>
    public static class PersonelKartUclari
    {
        private const string SayfaAd = "PersonelKart";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/kart/secenekler", Secenekler);
            app.MapGet("/api/personelbilgi/kart/liste", Liste);
            app.MapGet("/api/personelbilgi/kart/oku", Oku);
            app.MapPost("/api/personelbilgi/kart/kaydet", Kaydet);
            app.MapPost("/api/personelbilgi/kart/sil", Sil);
        }

        private static IResult Secenekler(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski KartTipiDoldur + Genel.SozlukYukle: ilk seçenek boş, alt seviyeler "- " ile girintili.
            Sozluk kriter = new Sozluk();
            kriter.kayitKod = ((int)ENUMSozlukUnsurlari.KARTTIPI).ToString().PadLeft(2, '0');
            kriter.seviye = 2;
            ObjectArray oa = TNS.IKY.Arac.Tanimla().SozlukListele(kriter);
            List<object> kartTipleri = new List<object>();
            kartTipleri.Add(new { KOD = 0, AD = "" });
            if (oa != null && oa.sonuc.islemSonuc)
                foreach (Sozluk s in oa.objeler)
                {
                    string ad = "";
                    for (int i = 2; i < s.seviye; i++) ad += "- ";
                    kartTipleri.Add(new { KOD = s.kod, AD = ad + s.ad });
                }

            // Eski Genel.YakinlikDerecesiDoldur + InsertRecord(0, boş)
            List<object> yakinliklar = new List<object>();
            yakinliklar.Add(new { KOD = 0, AD = "" });
            foreach (Sozluk s in TNS.IKY.Arac.YakinlikListe)
                yakinliklar.Add(new { KOD = s.kod, AD = s.ad });

            return Results.Json(new { kartTipleri = kartTipleri, yakinliklar = yakinliklar });
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> satirlar = new List<object>();
            int pKod = personelKod ?? 0;
            if (pKod <= 0) return Results.Json(new { satirlar = satirlar });

            PersonelKart kriter = new PersonelKart();
            kriter.personelKod = pKod;
            ObjectArray oa = TNS.IKY.Arac.Tanimla().PersonelKartListele(kullanan, kriter);
            if (!oa.sonuc.islemSonuc)
                return Results.Json(new { satirlar = satirlar, hata = PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Kart bilgileri okunamadı.") });

            foreach (PersonelKart p in oa.objeler)
            {
                string aktif = "Aktif";
                if (p.kayipTarihi.Yil != 0 || p.iadeTarihi.Yil != 0)
                    aktif = "Pasif";
                satirlar.Add(new { ZIMMETTAR = PersonelBilgiOrtak.TarihYaz(p.zimmetTarihi), KOD = p.kod, KARTTIPI = p.kartTipiAd, KARTNO = p.kartNo, ADSOYAD = p.adiSoyadi, AKTIF = aktif });
            }
            return Results.Json(new { satirlar = satirlar });
        }

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((kod ?? 0) <= 0) return Results.Json(new { hata = "Lütfen kartı seçiniz." });

            PersonelKart kriter = new PersonelKart();
            kriter.kod = kod.Value;
            kriter.personelKod = personelKod ?? 0;
            ObjectArray oa = TNS.IKY.Arac.Tanimla().PersonelKartListele(kullanan, kriter);
            if (!oa.sonuc.islemSonuc)
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Kart bilgisi okunamadı.") });

            foreach (PersonelKart p in oa.objeler)
                return Results.Json(new
                {
                    p.kod,
                    p.kartNo,
                    p.adiSoyadi,
                    p.plaka,
                    tagCihazNo = (p.tagCihazNo ?? "").ToString(),
                    zimmetTarihi = PersonelBilgiOrtak.TarihYaz(p.zimmetTarihi),
                    iadeTarihi = PersonelBilgiOrtak.TarihYaz(p.iadeTarihi),
                    p.aciklama,
                    kayipTarihi = PersonelBilgiOrtak.TarihYaz(p.kayipTarihi),
                    p.kartTipi,
                    yakinligi = (int)p.yakinligi,
                    p.zimmetEdenPersonel,
                    p.zimmetEdenPersonelAd,
                    p.iadeAlanPersonel,
                    p.iadeAlanPersonelAd
                });
            return Results.Json(new { hata = "Kart bilgisi bulunamadı." });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, KartIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski btnKaydet_Click denetimi (FRMPAB002).
            if (istek.personelKod <= 0) return Results.Json(new { basarili = false, hata = "Personel Bilgisi boş bırakılamaz." });

            PersonelKart bilgi = new PersonelKart();
            bilgi.kod = istek.kod;
            bilgi.personelKod = istek.personelKod;
            bilgi.kartNo = istek.kartNo ?? "";
            bilgi.adiSoyadi = istek.adiSoyadi ?? "";
            bilgi.plaka = istek.plaka ?? "";
            bilgi.zimmetTarihi = PersonelBilgiOrtak.TnsTarih(istek.zimmetTarihi);
            bilgi.kartTipi = istek.kartTipi;
            bilgi.yakinligi = (ENUMYakinlikDerecesi)istek.yakinligi;
            bilgi.tagCihazNo = istek.tagCihazNo ?? "";
            bilgi.aciklama = istek.aciklama ?? "";
            bilgi.iadeTarihi = PersonelBilgiOrtak.TnsTarih(istek.iadeTarihi);
            bilgi.kayipTarihi = PersonelBilgiOrtak.TnsTarih(istek.kayipTarihi);
            bilgi.zimmetEdenPersonel = istek.zimmetEdenPersonel;
            bilgi.iadeAlanPersonel = istek.iadeAlanPersonel;

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelKartKaydet(kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Kart bilgisi kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, KartIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski btnSil_Click (FRMPAB017 / FRMPAB016).
            if (istek.kod <= 0) return Results.Json(new { basarili = false, hata = "Silmek istediğiniz kaydı seçiniz." });

            PersonelKart d = new PersonelKart();
            d.kod = istek.kod;
            d.personelKod = istek.personelKod;
            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelKartSil(kullanan, d);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Kart bilgisi silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla gerçekleşti." });
        }
    }
}
