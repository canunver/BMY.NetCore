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
    /// <summary>Öğrenim Kaydet / Sil isteği: alan adları PersonelOgrenim sınıfındaki gibi; ekrandaki metinler eski sayfadaki gibi metin gelir.</summary>
    public class PersonelOgrenimIstegi
    {
        public int personelKod { get; set; }
        public string kod { get; set; }
        /// <summary>Eğitim seviyesi seçimi "sozlukKod:kayitKod" (eski ddlOgrenim değeri).</summary>
        public string ogrenimSecim { get; set; }
        public string universiteKod { get; set; }
        public string fakulteKod { get; set; }
        public string bolumKod { get; set; }
        public string okulAdi { get; set; }
        public bool memuriyeteBasladigiOkul { get; set; }
        public bool gecerli { get; set; }
        public string ogrenimYeri { get; set; }
        public string ogrenimSuresi { get; set; }
        public string baslangicTarihi { get; set; }
        public string belgeTur { get; set; }
        public string belgeNo { get; set; }
        public string derece { get; set; }
        public string belgeTarihi { get; set; }
        public bool denklik { get; set; }
        public string denkBolum { get; set; }
        public string denkOkul { get; set; }
        public string denklikTarihi { get; set; }
        public bool hazirlikVarMi { get; set; }
        public string kurumaIbrazTarihi { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri > Öğrenim Durumu sekmesi (eski InsanKaynaklari/PersonelOgrenim.aspx; yeni sayfa InsanKaynaklari/PersonelOgrenim):
    ///   GET  /api/personelbilgi/ogrenim/secenekler                 (eski OgrenimDoldur, UniversiteDoldur, FakulteDoldur, BolumDoldur,
    ///                                                               OgrenimYeriDoldur, OgrenimBelgeTurDoldur)
    ///   GET  /api/personelbilgi/ogrenim/liste?personelKod=         (eski Listele)
    ///   POST /api/personelbilgi/ogrenim/kaydet                     (eski btnKaydet_Click + KayitKontrol)
    ///   POST /api/personelbilgi/ogrenim/sil                        (eski btnSil_Click)
    ///   GET  /api/personelbilgi/ogrenim/belgeler?personelKod=&amp;kod=        (eski BelgeListesiDoldur)
    ///   POST /api/personelbilgi/ogrenim/belgeyukle                 (eski BelgeKaydet; çok parçalı form)
    ///   GET  /api/personelbilgi/ogrenim/belgeindir?personelKod=&amp;kod=&amp;belgeKod=   (eski BelgeGoster)
    ///   POST /api/personelbilgi/ogrenim/belgesil                   (eski BelgeSil)
    /// İş kuralları eski sayfadaki gibi IIKYServis.PersonelOgrenimListele / Kaydet / Sil, SozlukListele, PersonelBelgeXxx.
    /// </summary>
    public static class PersonelOgrenimUclari
    {
        private const string SayfaAd = "PersonelOgrenim";
        private const ENUMBELGEFORMTIP BelgeTip = ENUMBELGEFORMTIP.PERSONELOGRENIM;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/ogrenim/secenekler", Secenekler);
            app.MapGet("/api/personelbilgi/ogrenim/liste", Liste);
            app.MapPost("/api/personelbilgi/ogrenim/kaydet", Kaydet);
            app.MapPost("/api/personelbilgi/ogrenim/sil", Sil);
            app.MapGet("/api/personelbilgi/ogrenim/belgeler", (HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod) =>
                PersonelEgitimOrtak.BelgeListesi(baglam, ortam, SayfaAd, BelgeTip, personelKod, kod));
            app.MapPost("/api/personelbilgi/ogrenim/belgeyukle", (HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik) =>
                PersonelEgitimOrtak.BelgeYukle(baglam, ortam, sahtecilik, SayfaAd, BelgeTip)).DisableAntiforgery();
            app.MapGet("/api/personelbilgi/ogrenim/belgeindir", (HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod, int? belgeKod) =>
                PersonelEgitimOrtak.BelgeIndir(baglam, ortam, SayfaAd, BelgeTip, personelKod, kod, belgeKod));
            app.MapPost("/api/personelbilgi/ogrenim/belgesil", (HttpContext baglam, IWebHostEnvironment ortam, PersonelBelgeSilIstegi istek, IAntiforgery sahtecilik) =>
                PersonelEgitimOrtak.BelgeSil(baglam, ortam, sahtecilik, SayfaAd, BelgeTip, istek));
        }

        /// <summary>
        /// Eski XxxDoldur metotları. Eğitim seviyesi (OgrenimDoldur) OGRENIMKOD ve OGRENIMKOD2 sözlüklerinin birleşimidir, değer
        /// "kod:kayitKod" (ozel = true); kayıt kodu üniversite / fakülte / bölüm alanlarının görünürlüğünü belirler.
        /// </summary>
        private static IResult Secenekler(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string hata = "";
            List<object> ogrenim = PersonelEgitimOrtak.SozlukSecenekleri(ENUMSozlukUnsurlari.OGRENIMKOD, true, true, ref hata);
            ogrenim.AddRange(PersonelEgitimOrtak.SozlukSecenekleri(ENUMSozlukUnsurlari.OGRENIMKOD2, true, false, ref hata));

            return Results.Json(new
            {
                ogrenim = ogrenim,
                universite = PersonelEgitimOrtak.SozlukSecenekleri(ENUMSozlukUnsurlari.UNIVERSITEKODU, false, true, ref hata),
                fakulte = PersonelEgitimOrtak.SozlukSecenekleri(ENUMSozlukUnsurlari.FAKULTEKODU, false, true, ref hata),
                bolum = PersonelEgitimOrtak.SozlukSecenekleri(ENUMSozlukUnsurlari.BOLUMKODU, false, true, ref hata),
                ogrenimYeri = PersonelEgitimOrtak.SozlukSecenekleri(ENUMSozlukUnsurlari.OGRENIMYERI, false, true, ref hata),
                belgeTur = PersonelEgitimOrtak.SozlukSecenekleri(ENUMSozlukUnsurlari.OGRENIMBELGETUR, false, true, ref hata),
                hata = hata
            });
        }

        /// <summary>Eski Listele(new PersonelOgrenim()).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> satirlar = new List<object>();
            int pKod = personelKod ?? 0;
            if (pKod <= 0) return Results.Json(new { satirlar = satirlar });

            TNS.IKY.PersonelOgrenim kriter = new TNS.IKY.PersonelOgrenim();
            kriter.personelKod = pKod;
            ObjectArray pler = TNS.IKY.Arac.Tanimla().PersonelOgrenimListele(kullanan, kriter);
            if (!pler.sonuc.islemSonuc)
                return Results.Json(new { satirlar = satirlar, hata = PersonelBilgiOrtak.HataMetni(pler.sonuc.hataStr, "Öğrenim bilgileri okunamadı.") });

            foreach (TNS.IKY.PersonelOgrenim p in pler.objeler)
                satirlar.Add(new
                {
                    kod = p.kod,
                    personelKod = p.personelKod,
                    ogrenimKod = p.ogrenimKod,
                    ogrenimKodKayitKod = p.ogrenimKodKayitKod,
                    ogrenimSecim = p.ogrenimKod > 0 ? p.ogrenimKod + ":" + p.ogrenimKodKayitKod : "",
                    universiteKod = p.universiteKod,
                    fakulteKod = p.fakulteKod,
                    bolumKod = p.bolumKod,
                    okulAdi = p.okulAdi,
                    belgeTur = p.belgeTur,
                    belgeTarihi = PersonelBilgiOrtak.TarihYaz(p.belgeTarihi),
                    belgeNo = p.belgeNo,
                    baslangicTarihi = PersonelBilgiOrtak.TarihYaz(p.baslangicTarihi),
                    derece = p.derece,
                    gecerli = p.gecerli,
                    ogrenimYeri = p.ogrenimYeri,
                    denklik = p.denklik,
                    denklikTarihi = PersonelBilgiOrtak.TarihYaz(p.denklikTarihi),
                    denkOkul = p.denkOkul,
                    denkBolum = p.denkBolum,
                    hazirlikVarMi = p.hazirlikVarMi,
                    kurumaIbrazTarihi = PersonelBilgiOrtak.TarihYaz(p.kurumaIbrazTarihi),
                    memuriyeteBasladigiOkul = p.memuriyeteBasladigiOkul,
                    ogrenimSuresi = p.ogrenimSuresi
                });
            return Results.Json(new { satirlar = satirlar });
        }

        /// <summary>Eski btnKaydet_Click (KayitKontrol dahil). Başarıda kayıt kodu döner (eski hdnKod = sonuc.anahtar).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelOgrenimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski KayitKontrol
            string hata = "";
            string secim = (istek.ogrenimSecim ?? "").Trim();
            if (secim == "" || secim == "0")
                hata += "Öğrenim  Bilgisi boş bırakılamaz.\n";
            if (string.IsNullOrEmpty(istek.okulAdi))
                hata += "Okul Adı boş bırakılamaz.\n";
            if (hata != "") return Results.Json(new { basarili = false, hata = hata.Trim() });

            TNS.IKY.PersonelOgrenim p = new TNS.IKY.PersonelOgrenim();
            p.personelKod = istek.personelKod;
            int kod = OrtakFonksiyonlar.ConvertToInt((istek.kod ?? "").Trim(), 0);
            if (kod > 0)
                p.kod = kod;
            string[] arr = secim.Split(':');
            p.ogrenimKod = OrtakFonksiyonlar.ConvertToInt(arr[0], 0);
            p.universiteKod = OrtakFonksiyonlar.ConvertToInt(istek.universiteKod, 0);
            p.fakulteKod = OrtakFonksiyonlar.ConvertToInt(istek.fakulteKod, 0);
            p.bolumKod = OrtakFonksiyonlar.ConvertToInt(istek.bolumKod, 0);
            p.ogrenimSuresi = OrtakFonksiyonlar.ConvertToInt((istek.ogrenimSuresi ?? "").Trim(), 0);
            if (string.IsNullOrEmpty(istek.okulAdi))
                p.okulAdi = PersonelEgitimOrtak.SozlukAd(p.universiteKod, null);
            else
                p.okulAdi = istek.okulAdi;
            p.baslangicTarihi = PersonelBilgiOrtak.TnsTarih(istek.baslangicTarihi);
            p.belgeTur = OrtakFonksiyonlar.ConvertToInt(istek.belgeTur, 0);
            p.belgeNo = istek.belgeNo ?? "";
            p.derece = OrtakFonksiyonlar.ConvertToInt((istek.derece ?? "").Trim(), 0);
            p.belgeTarihi = PersonelBilgiOrtak.TnsTarih(istek.belgeTarihi);
            p.gecerli = istek.gecerli ? 1 : 0;
            p.ogrenimYeri = OrtakFonksiyonlar.ConvertToInt(istek.ogrenimYeri, 0);
            p.denklik = istek.denklik ? 1 : 0;
            if (p.denklik == 1)
            {
                p.denkBolum = istek.denkBolum ?? "";
                p.denkOkul = istek.denkOkul ?? "";
                p.denklikTarihi = PersonelBilgiOrtak.TnsTarih(istek.denklikTarihi);
            }
            else
            {
                p.denkBolum = string.Empty;
                p.denkOkul = string.Empty;
                p.denklikTarihi = new TNSDateTime();
            }
            p.hazirlikVarMi = istek.hazirlikVarMi ? 1 : 0;
            p.kurumaIbrazTarihi = PersonelBilgiOrtak.TnsTarih(istek.kurumaIbrazTarihi);
            p.memuriyeteBasladigiOkul = istek.memuriyeteBasladigiOkul ? 1 : 0;

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelOgrenimKaydet(kullanan, p);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Öğrenim bilgisi kaydedilemedi.") });
            return Results.Json(new { basarili = true, kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0), bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        /// <summary>Eski btnSil_Click.</summary>
        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, PersonelOgrenimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            int kod = OrtakFonksiyonlar.ConvertToInt((istek.kod ?? "").Trim(), 0);
            int pKod = istek.personelKod;
            if (pKod <= 0 || kod <= 0)
                return Results.Json(new { basarili = false, hata = "Lütfen silmek istediğiniz kaydı seçiniz." });

            TNS.IKY.PersonelOgrenim d = new TNS.IKY.PersonelOgrenim();
            d.personelKod = pKod;
            d.kod = kod;
            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelOgrenimSil(kullanan, d);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Öğrenim bilgisi silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla gerçekleşti." });
        }
    }
}
