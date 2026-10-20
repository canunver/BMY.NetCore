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
    /// <summary>Ceza/Ödül Kaydet / Sil isteği: alan adları PersonelCezaOdul sınıfındaki gibi.</summary>
    public class PersonelCezaOdulIstegi
    {
        public int personelKod { get; set; }
        public string kod { get; set; }
        public string cezaOdulTipi { get; set; }
        public string tarih { get; set; }
        public string verenKurum { get; set; }
        public string verenAdi { get; set; }
        public string verenUnvani { get; set; }
        public string alanUnvani { get; set; }
        public string cinsi { get; set; }
        public string sebebi { get; set; }
        public string dayanak { get; set; }
        public string belgeNo { get; set; }
        public string belgeTarihi { get; set; }
        public string belgeCinsi { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri > Ceza/Ödül sekmesi (eski InsanKaynaklari/PersonelCezaOdul.aspx; yeni sayfa InsanKaynaklari/PersonelCezaOdul):
    ///   GET  /api/personelbilgi/cezaodul/secenekler             (eski CezaOdulTipiDoldur)
    ///   GET  /api/personelbilgi/cezaodul/liste?personelKod=     (eski Listele)
    ///   GET  /api/personelbilgi/cezaodul/oku?personelKod=&amp;kod=   (eski SatirSecildi: seçilen kaydı yeniden okur)
    ///   POST /api/personelbilgi/cezaodul/kaydet                 (eski btnKaydet_Click)
    ///   POST /api/personelbilgi/cezaodul/sil                    (eski btnSil_Click)
    /// İş kuralları eski sayfadaki gibi IIKYServis.PersonelCezaOdulListele / Kaydet / Sil, SozlukListele.
    /// Eski sayfada belge bölümü açıklama satırına alınmıştı (belge eklenmiyordu); burada da yok.
    /// </summary>
    public static class PersonelCezaOdulUclari
    {
        private const string SayfaAd = "PersonelCezaOdul";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/cezaodul/secenekler", Secenekler);
            app.MapGet("/api/personelbilgi/cezaodul/liste", Liste);
            app.MapGet("/api/personelbilgi/cezaodul/oku", Oku);
            app.MapPost("/api/personelbilgi/cezaodul/kaydet", Kaydet);
            app.MapPost("/api/personelbilgi/cezaodul/sil", Sil);
        }

        /// <summary>Eski CezaOdulTipiDoldur (sözlük 30, seviye 2).</summary>
        private static IResult Secenekler(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string hata = "";
            return Results.Json(new
            {
                cezaOdulTipi = PersonelEgitimOrtak.SozlukSecenekleri(ENUMSozlukUnsurlari.CEZAODULTIPI, false, true, ref hata),
                hata = hata
            });
        }

        /// <summary>Eski Listele: liste sütunları Ceza/Ödül Tipi (cezaOdulTipiAdi) ve Tarih.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> satirlar = new List<object>();
            int pKod = personelKod ?? 0;
            if (pKod == 0) return Results.Json(new { satirlar = satirlar });

            TNS.IKY.PersonelCezaOdul kriter = new TNS.IKY.PersonelCezaOdul();
            kriter.personelKod = pKod;
            ObjectArray pler = TNS.IKY.Arac.Tanimla().PersonelCezaOdulListele(kullanan, kriter);
            if (!pler.sonuc.islemSonuc)
                return Results.Json(new { satirlar = satirlar, hata = PersonelBilgiOrtak.HataMetni(pler.sonuc.hataStr, "Ceza/ödül bilgileri okunamadı.") });

            foreach (TNS.IKY.PersonelCezaOdul p in pler.objeler)
                satirlar.Add(new { kod = p.kod, cezaOdulTipiAdi = p.cezaOdulTipiAdi, tarih = PersonelBilgiOrtak.TarihYaz(p.tarih) });
            return Results.Json(new { satirlar = satirlar });
        }

        /// <summary>Eski SatirSecildi: PersonelCezaOdulListele(kod, personelKod) ile kaydın tüm alanları.</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            if ((kod ?? 0) == 0) return Results.Json(new { kayit = (object)null });

            TNS.IKY.PersonelCezaOdul kriter = new TNS.IKY.PersonelCezaOdul();
            kriter.kod = kod ?? 0;
            kriter.personelKod = personelKod ?? 0;
            ObjectArray pler = TNS.IKY.Arac.Tanimla().PersonelCezaOdulListele(kullanan, kriter);
            if (!pler.sonuc.islemSonuc)
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(pler.sonuc.hataStr, "Ceza/ödül bilgisi okunamadı.") });

            object kayit = null;
            foreach (TNS.IKY.PersonelCezaOdul p in pler.objeler)
                kayit = new
                {
                    kod = p.kod,
                    personelKod = p.personelKod,
                    cezaOdulTipi = p.cezaOdulTipi,
                    tarih = PersonelBilgiOrtak.TarihYaz(p.tarih),
                    verenKurum = p.verenKurum,
                    verenAdi = p.verenAdi,
                    verenUnvani = p.verenUnvani,
                    alanUnvani = p.alanUnvani,
                    cinsi = p.cinsi,
                    sebebi = p.sebebi,
                    dayanak = p.dayanak,
                    belgeNo = p.belgeNo,
                    belgeTarihi = PersonelBilgiOrtak.TarihYaz(p.belgeTarihi),
                    belgeCinsi = p.belgeCinsi
                };
            return Results.Json(new { kayit = kayit });
        }

        /// <summary>Eski btnKaydet_Click.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelCezaOdulIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string hata = "";
            if (istek.personelKod == 0)
                hata += "Personel Bilgisi boş bırakılamaz.\n";
            if (hata != "") return Results.Json(new { basarili = false, hata = hata.Trim() });

            TNS.IKY.PersonelCezaOdul bilgi = new TNS.IKY.PersonelCezaOdul();
            bilgi.kod = OrtakFonksiyonlar.ConvertToInt((istek.kod ?? "").Trim(), 0);
            bilgi.personelKod = istek.personelKod;
            bilgi.alanUnvani = istek.alanUnvani ?? "";
            bilgi.belgeCinsi = istek.belgeCinsi ?? "";
            bilgi.belgeNo = istek.belgeNo ?? "";
            bilgi.belgeTarihi = PersonelBilgiOrtak.TnsTarih(istek.belgeTarihi);
            bilgi.cezaOdulTipi = OrtakFonksiyonlar.ConvertToInt(istek.cezaOdulTipi, 0);
            bilgi.cinsi = istek.cinsi ?? "";
            bilgi.dayanak = istek.dayanak ?? "";
            bilgi.sebebi = istek.sebebi ?? "";
            bilgi.tarih = PersonelBilgiOrtak.TnsTarih(istek.tarih);
            bilgi.verenAdi = istek.verenAdi ?? "";
            bilgi.verenKurum = istek.verenKurum ?? "";
            bilgi.verenUnvani = istek.verenUnvani ?? "";

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelCezaOdulKaydet(kullanan, bilgi);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Ceza/ödül bilgisi kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        /// <summary>Eski btnSil_Click.</summary>
        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, PersonelCezaOdulIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            int kod = OrtakFonksiyonlar.ConvertToInt((istek.kod ?? "").Trim(), 0);
            if (kod == 0)
                return Results.Json(new { basarili = false, hata = "Lütfen silmek istediğiniz Birey bilgisini seçin." });

            TNS.IKY.PersonelCezaOdul d = new TNS.IKY.PersonelCezaOdul();
            d.kod = kod;
            d.personelKod = istek.personelKod;
            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelCezaOdulSil(kullanan, d);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Ceza/ödül bilgisi silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla gerçekleşti." });
        }
    }
}
