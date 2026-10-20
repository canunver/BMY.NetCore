using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Askerlik kaydet isteği: alan adları PAskerlik sınıfındaki gibi; tarihler "yyyy-MM-dd".</summary>
    public class AskerlikIstegi
    {
        public int personelKod { get; set; }
        /// <summary>Askerlik durumu seçeneğinin değeri: "sözlükKod:kayıtKod" (eski ddlAskerlikDurumu).</summary>
        public string askerlikDurumu { get; set; }
        public string askerlikBaslamaTarihi { get; set; }
        public string terhisTarihi { get; set; }
        public string tecilTarihi { get; set; }
        public string belgeTarihi { get; set; }
        public string astegmenNaspTarihi { get; set; }
        public string tegmenNaspTarihi { get; set; }
        public string yedekSubayOkuluGirisTarihi { get; set; }
        public string erYedekSubayGecisTarihi { get; set; }
        public string erYedekSubayKitaHizmetiBaslamaTarihi { get; set; }
        public string erYedekSubayKitaHizmetiBitisTarihi { get; set; }
        public string ibrazIntikalTarihi { get; set; }
        public string askerlikMuafNeden { get; set; }
        public string sayilmayanGunSayisi { get; set; }
        public string askerlikCuzdanNo { get; set; }
        public int rutbesi { get; set; }
        public int sinifi { get; set; }
        public bool borclanmaVarMi { get; set; }
        public string askerlikSubesi { get; set; }
        public string sinifDonemSicilNo { get; set; }
        public string erYedekSubayGorevYeri { get; set; }
        public string aciklama { get; set; }
        public string belgeNo { get; set; }
    }

    /// <summary>Belge silme isteği.</summary>
    public class AskerlikBelgeIstegi
    {
        public int personelKod { get; set; }
        public int kod { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri "Askerlik" sekmesi (eski InsanKaynaklari/PersonelAskerlik.aspx; yeni sayfa InsanKaynaklari/PersonelAskerlik) uçları:
    ///   GET  /api/personelbilgi/askerlik/secenekler            Askerlik durumu, rütbe, sınıf seçenekleri (eski AskerlikDurumDoldur, RutbeDoldur,
    ///                                                          SinifDoldur) ve MANAS kullanımı (eski Page_Load'daki alan gizleme)
    ///   GET  /api/personelbilgi/askerlik/oku?personelKod=      (eski Listele)
    ///   POST /api/personelbilgi/askerlik/kaydet                (eski btnKaydet_Click / Kaydet)
    ///   GET  /api/personelbilgi/askerlik/belgeler?personelKod= (eski BelgeListesiDoldur)
    ///   POST /api/personelbilgi/askerlik/belge-yukle           multipart: personelKod, dosya (eski BelgeKaydet)
    ///   GET  /api/personelbilgi/askerlik/belge-indir?personelKod=&amp;kod=  (eski BelgeGoster)
    ///   POST /api/personelbilgi/askerlik/belge-sil             (eski BelgeSil)
    /// İş kuralları eski sayfadaki gibi IIKYServis.PersonelAskerlikListele / Kaydet, PersonelBelgeListele / Kaydet / ListesiGetir / Sil, SozlukListele.
    /// </summary>
    public static class PersonelAskerlikUclari
    {
        private const string SayfaAd = "PersonelAskerlik";

        /// <summary>appSettings: EkliDosyaAzamiMB (varsayılan 20), ekli dosyalar bileşeniyle aynı sınır.</summary>
        private static long AzamiBayt
        {
            get { return Math.Max(1, OrtakFonksiyonlar.ConvertToInt(Ayar.Oku("EkliDosyaAzamiMB", "20"), 20)) * 1024L * 1024L; }
        }

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/askerlik/secenekler", Secenekler);
            app.MapGet("/api/personelbilgi/askerlik/oku", Oku);
            app.MapPost("/api/personelbilgi/askerlik/kaydet", Kaydet);
            app.MapGet("/api/personelbilgi/askerlik/belgeler", Belgeler);
            app.MapPost("/api/personelbilgi/askerlik/belge-yukle", BelgeYukle).DisableAntiforgery();
            app.MapGet("/api/personelbilgi/askerlik/belge-indir", BelgeIndir);
            app.MapPost("/api/personelbilgi/askerlik/belge-sil", BelgeSil);
        }

        /// <summary>Eski Genel.SozlukYukle: ilk seçenek boş, alt seviyeler "- " ile girintili; ozel ise değer "kod:kayıtKod".</summary>
        private static List<object> SozlukYukle(ENUMSozlukUnsurlari unsur, bool ozel)
        {
            Sozluk kriter = new Sozluk();
            kriter.kayitKod = ((int)unsur).ToString().PadLeft(2, '0');
            kriter.seviye = 2;
            kriter.ozel = ozel;
            ObjectArray oa = TNS.IKY.Arac.Tanimla().SozlukListele(kriter);

            List<object> liste = new List<object>();
            liste.Add(new { KOD = ozel ? "" : "0", AD = "" });
            if (oa == null || !oa.sonuc.islemSonuc) return liste;
            foreach (Sozluk s in oa.objeler)
            {
                string ad = "";
                for (int i = 2; i < s.seviye; i++) ad += "- ";
                ad += s.ad;
                liste.Add(new { KOD = ozel ? s.kod + ":" + s.kayitKod : s.kod.ToString(), AD = ad });
            }
            return liste;
        }

        private static IResult Secenekler(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            bool manas = false;
            try { manas = TNS.DEG.Arac.ManasKullaniyor(); }
            catch (Exception) { manas = false; }

            return Results.Json(new
            {
                askerlikDurumlari = SozlukYukle(ENUMSozlukUnsurlari.ASKERLIKDURUMU, true),
                rutbeler = SozlukYukle(ENUMSozlukUnsurlari.ASKERLIKRUTBESI, false),
                siniflar = SozlukYukle(ENUMSozlukUnsurlari.ASKERLIKSINIFI, false),
                manas = manas
            });
        }

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            int pKod = personelKod ?? 0;
            if (pKod <= 0) return Results.Json(new { bos = true });

            PAskerlik a;
            try
            {
                a = TNS.IKY.Arac.Tanimla().PersonelAskerlikListele(kullanan, pKod);
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Askerlik bilgileri okunamadı." });
            }
            if (a == null) return Results.Json(new { hata = "Askerlik bilgileri okunamadı." });
            if (!a.sonuc.islemSonuc) return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(a.sonuc.hataStr, "Askerlik bilgileri okunamadı.") });

            return Results.Json(new
            {
                askerlikDurumu = a.askerlikDurumu > 0 ? a.askerlikDurumu + ":" + a.askerlikDurumuKayitKod : "",
                askerlikBaslamaTarihi = PersonelBilgiOrtak.TarihYaz(a.askerlikBaslamaTarihi),
                terhisTarihi = PersonelBilgiOrtak.TarihYaz(a.terhisTarihi),
                tecilTarihi = PersonelBilgiOrtak.TarihYaz(a.tecilTarihi),
                belgeTarihi = PersonelBilgiOrtak.TarihYaz(a.belgeTarihi),
                astegmenNaspTarihi = PersonelBilgiOrtak.TarihYaz(a.astegmenNaspTarihi),
                tegmenNaspTarihi = PersonelBilgiOrtak.TarihYaz(a.tegmenNaspTarihi),
                yedekSubayOkuluGirisTarihi = PersonelBilgiOrtak.TarihYaz(a.yedekSubayOkuluGirisTarihi),
                erYedekSubayGecisTarihi = PersonelBilgiOrtak.TarihYaz(a.erYedekSubayGecisTarihi),
                erYedekSubayKitaHizmetiBaslamaTarihi = PersonelBilgiOrtak.TarihYaz(a.erYedekSubayKitaHizmetiBaslamaTarihi),
                erYedekSubayKitaHizmetiBitisTarihi = PersonelBilgiOrtak.TarihYaz(a.erYedekSubayKitaHizmetiBitisTarihi),
                ibrazIntikalTarihi = PersonelBilgiOrtak.TarihYaz(a.ibrazIntikalTarihi),
                a.askerlikMuafNeden,
                a.sayilmayanGunSayisi,
                a.askerlikCuzdanNo,
                rutbesi = a.rutbesi.ToString(),
                sinifi = a.sinifi.ToString(),
                borclanmaVarMi = a.borclanmaVarMi > 0,
                a.askerlikSubesi,
                a.sinifDonemSicilNo,
                a.erYedekSubayGorevYeri,
                a.aciklama,
                a.belgeNo
            });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, AskerlikIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski Kaydet denetimleri (FRMPAB002).
            string hata = "";
            if (istek.personelKod <= 0)
                hata += "Personel Bilgisi boş bırakılamaz.\n";
            if (string.IsNullOrWhiteSpace(istek.askerlikDurumu))
                hata += "Askerlik durum bilgisini boş geçmeyiniz.\n";
            if (hata != "") return Results.Json(new { basarili = false, hata = hata.Trim() });

            PAskerlik a = new PAskerlik();
            a.kod = istek.personelKod;
            a.askerlikDurumu = OrtakFonksiyonlar.ConvertToInt(istek.askerlikDurumu.Split(':')[0], 0);
            a.askerlikBaslamaTarihi = PersonelBilgiOrtak.TnsTarih(istek.askerlikBaslamaTarihi);
            a.terhisTarihi = PersonelBilgiOrtak.TnsTarih(istek.terhisTarihi);
            a.tecilTarihi = PersonelBilgiOrtak.TnsTarih(istek.tecilTarihi);
            a.belgeTarihi = PersonelBilgiOrtak.TnsTarih(istek.belgeTarihi);
            a.astegmenNaspTarihi = PersonelBilgiOrtak.TnsTarih(istek.astegmenNaspTarihi);
            a.tegmenNaspTarihi = PersonelBilgiOrtak.TnsTarih(istek.tegmenNaspTarihi);
            a.yedekSubayOkuluGirisTarihi = PersonelBilgiOrtak.TnsTarih(istek.yedekSubayOkuluGirisTarihi);
            a.erYedekSubayGecisTarihi = PersonelBilgiOrtak.TnsTarih(istek.erYedekSubayGecisTarihi);
            a.erYedekSubayKitaHizmetiBaslamaTarihi = PersonelBilgiOrtak.TnsTarih(istek.erYedekSubayKitaHizmetiBaslamaTarihi);
            a.erYedekSubayKitaHizmetiBitisTarihi = PersonelBilgiOrtak.TnsTarih(istek.erYedekSubayKitaHizmetiBitisTarihi);
            a.ibrazIntikalTarihi = PersonelBilgiOrtak.TnsTarih(istek.ibrazIntikalTarihi);
            a.askerlikMuafNeden = istek.askerlikMuafNeden ?? "";
            a.sayilmayanGunSayisi = OrtakFonksiyonlar.ConvertToInt((istek.sayilmayanGunSayisi ?? "").Trim(), 0);
            // Eski kod burada cüzdan no yerine sayılmayan gün sayısını yazıyordu (askerlikCuzdanNo = txtSayilmayanGunSayisi.Text); düzeltildi.
            a.askerlikCuzdanNo = istek.askerlikCuzdanNo ?? "";
            a.rutbesi = istek.rutbesi;
            a.sinifi = istek.sinifi;
            if (istek.borclanmaVarMi)
                a.borclanmaVarMi = 1;
            a.askerlikSubesi = istek.askerlikSubesi ?? "";
            a.sinifDonemSicilNo = istek.sinifDonemSicilNo ?? "";
            a.erYedekSubayGorevYeri = istek.erYedekSubayGorevYeri ?? "";
            a.aciklama = istek.aciklama ?? "";
            a.belgeNo = istek.belgeNo ?? "";

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelAskerlikKaydet(kullanan, a);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Askerlik bilgileri kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        // ------------------------------------------------------------------ belgeler

        private static PersonelBelgeler BelgeKriteri(int personelKod)
        {
            PersonelBelgeler belge = new PersonelBelgeler();
            belge.pKod = personelKod;
            belge.tip = (int)ENUMBELGEFORMTIP.PERSONELASKERLIK;
            belge.tipKayitKod = personelKod;
            return belge;
        }

        /// <summary>Personelin askerlik belgeleri (eski BelgeListesiDoldur).</summary>
        private static ObjectArray BelgeListesi(Kullanici kullanan, int personelKod)
        {
            return TNS.IKY.Arac.Tanimla().PersonelBelgeListele(kullanan, BelgeKriteri(personelKod));
        }

        /// <summary>
        /// Belge bu personelin askerlik belgesi mi? İş kuralı (IKYBelgeler) belgeyi yalnızca koduyla okur / siler ve yetki denetlemez;
        /// başka personelin belgesine kod ile erişilmesin diye istenen kod bu personelin belge listesinde aranır.
        /// </summary>
        private static bool BelgeBuPersonelinMi(Kullanici kullanan, int personelKod, int kod)
        {
            if (personelKod <= 0 || kod <= 0) return false;
            ObjectArray oa = BelgeListesi(kullanan, personelKod);
            if (oa == null || !oa.sonuc.islemSonuc) return false;
            foreach (PersonelBelgeler b in oa.objeler)
                if (b.kod == kod) return true;
            return false;
        }

        private static IResult Belgeler(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> satirlar = new List<object>();
            int pKod = personelKod ?? 0;
            if (pKod <= 0) return Results.Json(new { satirlar = satirlar });

            ObjectArray oa = BelgeListesi(kullanan, pKod);
            if (oa == null || !oa.sonuc.islemSonuc)
                return Results.Json(new { satirlar = satirlar, hata = PersonelBilgiOrtak.HataMetni(oa?.sonuc.hataStr, "Belgeler okunamadı.") });
            foreach (PersonelBelgeler d in oa.objeler)
                satirlar.Add(new { KOD = d.kod, PKOD = d.pKod, TIPKAYITKOD = d.tipKayitKod, TIP = d.tip, BELGEADI = d.belgeAdi });
            return Results.Json(new { satirlar = satirlar });
        }

        private static async Task<IResult> BelgeYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();

            IFormCollection form = await baglam.Request.ReadFormAsync();
            int pKod = OrtakFonksiyonlar.ConvertToInt(((string)form["personelKod"] ?? "").Trim(), 0);
            IFormFile dosya = form.Files["dosya"];

            // Eski BelgeKaydet (FRMPAB041).
            if (pKod <= 0) return Results.Json(new { basarili = false, hata = "Belgenin kim için kaydedileceğini seçin." });
            if (dosya == null || dosya.Length <= 0) return Results.Json(new { basarili = false, hata = "Lütfen yüklenecek belgeyi seçiniz." });
            if (dosya.Length > AzamiBayt)
                return Results.Json(new { basarili = false, hata = "Belge çok büyük (en çok " + (AzamiBayt / 1024 / 1024) + " MB)." });

            byte[] icerik;
            using (MemoryStream bellek = new MemoryStream())
            {
                await dosya.CopyToAsync(bellek);
                icerik = bellek.ToArray();
            }

            PersonelBelgeler yeniBelge = new PersonelBelgeler(pKod, pKod, (int)ENUMBELGEFORMTIP.PERSONELASKERLIK, Path.GetFileName(dosya.FileName ?? "belge"), icerik.Length);
            Array.Copy(icerik, yeniBelge.belge, icerik.Length);

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelBelgeKaydet(kullanan, yeniBelge);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Belge kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = string.IsNullOrEmpty(sonuc.bilgiStr) ? "Belge kaydedildi." : PersonelBilgiOrtak.Duz(sonuc.bilgiStr) });
        }

        private static IResult BelgeIndir(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!BelgeBuPersonelinMi(kullanan, personelKod ?? 0, kod ?? 0)) return Results.Json(new { hata = "Belge bulunamadı." });

            // Eski BelgeGoster: iş kuralı belgeyi geçici dosyaya yazar, anahtar = "dosya yolu\nbelge adı\nuzantı".
            Sonuc s = TNS.IKY.Arac.Tanimla().PersonelBelgeListesiGetir(kullanan, new PersonelBelgeler { kod = kod.Value });
            string[] parametreler = (s?.anahtar ?? "").Split('\n');
            if (s == null || !s.islemSonuc || parametreler.Length != 3 || !File.Exists(parametreler[0]))
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(s?.hataStr, "Belge bulunamadı.") });

            byte[] icerik = File.ReadAllBytes(parametreler[0]);
            try { File.Delete(parametreler[0]); } catch (Exception) { /* geçici dosya */ }

            string ad = Path.GetFileName(parametreler[1]);
            if (string.IsNullOrEmpty(ad)) ad = "belge." + parametreler[2];
            if (!new FileExtensionContentTypeProvider().TryGetContentType(ad, out string tur)) tur = "application/octet-stream";
            baglam.Response.Headers["X-Content-Type-Options"] = "nosniff";
            return Results.File(icerik, tur, ad);
        }

        private static async Task<IResult> BelgeSil(HttpContext baglam, IWebHostEnvironment ortam, AskerlikBelgeIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            if (istek.kod <= 0) return Results.Json(new { basarili = false, hata = "Belge seçiniz." });
            if (!BelgeBuPersonelinMi(kullanan, istek.personelKod, istek.kod)) return Results.Json(new { basarili = false, hata = "Belge bulunamadı." });

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelBelgeSil(kullanan, new PersonelBelgeler { kod = istek.kod });
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Belge silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = string.IsNullOrEmpty(sonuc.bilgiStr) ? "Belge silindi." : PersonelBilgiOrtak.Duz(sonuc.bilgiStr) });
        }
    }
}
