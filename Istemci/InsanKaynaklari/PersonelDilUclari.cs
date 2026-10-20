using System.Collections.Generic;
using System.Globalization;
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
    /// <summary>Yabancı Dil Kaydet / Sil isteği: alan adları PersonelDil sınıfındaki gibi.</summary>
    public class PersonelDilIstegi
    {
        public int personelKod { get; set; }
        public string kod { get; set; }
        public string dilKod { get; set; }
        public string puan { get; set; }
        public string tarih { get; set; }
        public string duzey { get; set; }
        public string donemAy { get; set; }
        public string donemYil { get; set; }
        public string belgeTur { get; set; }
        public string belgeTarihi { get; set; }
        public string belgeNo { get; set; }
        public string dilSeviyesi { get; set; }
        public bool bordroyaEsas { get; set; }
        public bool kurumFaydalaniyor { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri > Yabancı Dil sekmesi (eski InsanKaynaklari/PersonelDil.aspx; yeni sayfa InsanKaynaklari/PersonelDil):
    ///   GET  /api/personelbilgi/dil/secenekler                 (eski DilDoldur, DilBelgeTurDoldur, Genel.AyDoldur, Genel.YilDoldur)
    ///   GET  /api/personelbilgi/dil/liste?personelKod=         (eski Listele)
    ///   POST /api/personelbilgi/dil/kaydet                     (eski btnKaydet_Click + KayitKontrol)
    ///   POST /api/personelbilgi/dil/sil                        (eski btnSil_Click)
    ///   GET  /api/personelbilgi/dil/belgeler?personelKod=&amp;kod=          (eski BelgeListesiDoldur)
    ///   POST /api/personelbilgi/dil/belgeyukle                 (eski BelgeKaydet; çok parçalı form)
    ///   GET  /api/personelbilgi/dil/belgeindir?personelKod=&amp;kod=&amp;belgeKod=   (eski BelgeGoster)
    ///   POST /api/personelbilgi/dil/belgesil                   (eski BelgeSil)
    /// İş kuralları eski sayfadaki gibi IIKYServis.PersonelDilListele / Kaydet / Sil, SozlukListele, PersonelBelgeXxx.
    /// </summary>
    public static class PersonelDilUclari
    {
        private const string SayfaAd = "PersonelDil";
        private const ENUMBELGEFORMTIP BelgeTip = ENUMBELGEFORMTIP.PERSONELDIL;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/dil/secenekler", Secenekler);
            app.MapGet("/api/personelbilgi/dil/liste", Liste);
            app.MapPost("/api/personelbilgi/dil/kaydet", Kaydet);
            app.MapPost("/api/personelbilgi/dil/sil", Sil);
            app.MapGet("/api/personelbilgi/dil/belgeler", (HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod) =>
                PersonelEgitimOrtak.BelgeListesi(baglam, ortam, SayfaAd, BelgeTip, personelKod, kod));
            app.MapPost("/api/personelbilgi/dil/belgeyukle", (HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik) =>
                PersonelEgitimOrtak.BelgeYukle(baglam, ortam, sahtecilik, SayfaAd, BelgeTip)).DisableAntiforgery();
            app.MapGet("/api/personelbilgi/dil/belgeindir", (HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod, int? belgeKod) =>
                PersonelEgitimOrtak.BelgeIndir(baglam, ortam, SayfaAd, BelgeTip, personelKod, kod, belgeKod));
            app.MapPost("/api/personelbilgi/dil/belgesil", (HttpContext baglam, IWebHostEnvironment ortam, PersonelBelgeSilIstegi istek, IAntiforgery sahtecilik) =>
                PersonelEgitimOrtak.BelgeSil(baglam, ortam, sahtecilik, SayfaAd, BelgeTip, istek));
        }

        /// <summary>
        /// Eski DilDoldur / DilBelgeTurDoldur (sözlük 15 / 16, seviye 2) ve Genel.AyDoldur (ay adları). Eski Genel.YilDoldur
        /// 2000'den bu yıla kadar yıl veriyordu; yeni ekranda yıl sayı kutusu olduğu için yalnızca alt / üst sınır döner.
        /// </summary>
        private static IResult Secenekler(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string hata = "";
            List<object> aylar = new List<object>();
            aylar.Add(new { KOD = "0", AD = "" });
            DateTimeFormatInfo tarihBicim = new CultureInfo("tr-TR").DateTimeFormat;
            for (int i = 1; i < 13; i++)
                aylar.Add(new { KOD = i.ToString(), AD = tarihBicim.GetMonthName(i) });

            return Results.Json(new
            {
                dil = PersonelEgitimOrtak.SozlukSecenekleri(ENUMSozlukUnsurlari.DILKODU, false, true, ref hata),
                belgeTur = PersonelEgitimOrtak.SozlukSecenekleri(ENUMSozlukUnsurlari.DILBELGETUR, false, true, ref hata),
                aylar = aylar,
                yilEnAz = 2000,
                yilEnCok = System.DateTime.Now.Year,
                hata = hata
            });
        }

        /// <summary>Eski Listele(new PersonelDil()): dil ve belge türü adları eski sayfadaki gibi Genel.SozlukBilgiGetir ile.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> satirlar = new List<object>();
            int pKod = personelKod ?? 0;
            if (pKod <= 0) return Results.Json(new { satirlar = satirlar });

            TNS.IKY.PersonelDil kriter = new TNS.IKY.PersonelDil();
            kriter.personelKod = pKod;
            ObjectArray pler = TNS.IKY.Arac.Tanimla().PersonelDilListele(kullanan, kriter);
            if (!pler.sonuc.islemSonuc)
                return Results.Json(new { satirlar = satirlar, hata = PersonelBilgiOrtak.HataMetni(pler.sonuc.hataStr, "Yabancı dil bilgileri okunamadı.") });

            Dictionary<int, string> adlar = new Dictionary<int, string>();
            foreach (TNS.IKY.PersonelDil p in pler.objeler)
                satirlar.Add(new
                {
                    kod = p.kod,
                    personelKod = p.personelKod,
                    dilKod = p.dilKod,
                    dilAd = PersonelEgitimOrtak.SozlukAd(p.dilKod, adlar),
                    puan = PersonelEgitimOrtak.OndalikYaz(p.puan),
                    puanSayi = p.puan,
                    tarih = PersonelBilgiOrtak.TarihYaz(p.tarih),
                    duzey = p.duzey,
                    donemAy = p.donemAy,
                    donemYil = p.donemYil,
                    belgeTur = p.belgeTur,
                    belgeTurAd = PersonelEgitimOrtak.SozlukAd(p.belgeTur, adlar),
                    belgeTarihi = PersonelBilgiOrtak.TarihYaz(p.belgeTarihi),
                    belgeNo = p.belgeNo,
                    bordroyaEsas = p.bordroyaEsas,
                    dilSeviyesi = p.dilSeviyesi,
                    kurumFaydalaniyor = p.kurumFaydalaniyor
                });
            return Results.Json(new { satirlar = satirlar });
        }

        /// <summary>Eski btnKaydet_Click (KayitKontrol dahil). Başarıda kayıt kodu döner (eski hdnKod = sonuc.anahtar).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelDilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski KayitKontrol
            string hata = "";
            if (OrtakFonksiyonlar.ConvertToInt(istek.dilKod, 0) == 0)
                hata += "Dil Bilgisi boş bırakılamaz.\n";
            if (OrtakFonksiyonlar.ConvertToInt(istek.belgeTur, 0) == 0)
                hata += "Dil Bilgisi belge türünü seçiniz.\n";
            if (hata != "") return Results.Json(new { basarili = false, hata = hata.Trim() });

            TNS.IKY.PersonelDil d = new TNS.IKY.PersonelDil();
            d.personelKod = istek.personelKod;
            int kod = OrtakFonksiyonlar.ConvertToInt((istek.kod ?? "").Trim(), 0);
            if (kod > 0)
                d.kod = kod;
            d.dilKod = OrtakFonksiyonlar.ConvertToInt(istek.dilKod, 0);
            d.puan = PersonelEgitimOrtak.Ondalik(istek.puan);
            d.tarih = PersonelBilgiOrtak.TnsTarih(istek.tarih);
            d.duzey = istek.duzey ?? "";
            d.donemAy = OrtakFonksiyonlar.ConvertToInt(istek.donemAy, 0);
            d.donemYil = OrtakFonksiyonlar.ConvertToInt((istek.donemYil ?? "").Trim(), 0);
            d.belgeTur = OrtakFonksiyonlar.ConvertToInt(istek.belgeTur, 0);
            d.belgeTarihi = PersonelBilgiOrtak.TnsTarih(istek.belgeTarihi);
            d.belgeNo = (istek.belgeNo ?? "").Trim();
            d.dilSeviyesi = (istek.dilSeviyesi ?? "").Trim();
            d.bordroyaEsas = istek.bordroyaEsas ? 1 : 0;
            d.kurumFaydalaniyor = istek.kurumFaydalaniyor ? 1 : 0;

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelDilKaydet(kullanan, d);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Yabancı dil bilgisi kaydedilemedi.") });
            return Results.Json(new { basarili = true, kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0), bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        /// <summary>Eski btnSil_Click.</summary>
        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, PersonelDilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            int kod = OrtakFonksiyonlar.ConvertToInt((istek.kod ?? "").Trim(), 0);
            int pKod = istek.personelKod;
            if (pKod <= 0 || kod <= 0)
                return Results.Json(new { basarili = false, hata = "Lütfen silmek istediğiniz Dil bilgisini seçin." });

            TNS.IKY.PersonelDil d = new TNS.IKY.PersonelDil();
            d.personelKod = pKod;
            d.kod = kod;
            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelDilSil(kullanan, d);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Yabancı dil bilgisi silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla gerçekleşti." });
        }
    }
}
