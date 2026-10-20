using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Belge silme isteği (Öğrenim / Yabancı Dil / Hizmet İçi Eğitim sekmeleri).</summary>
    public class PersonelBelgeSilIstegi
    {
        public int personelKod { get; set; }
        public int kod { get; set; }
        public int belgeKod { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri'nin Öğrenim Durumu, Yabancı Dil, Hizmet İçi Eğitim ve Ceza/Ödül sekmelerinin (eski PersonelOgrenim,
    /// PersonelDil, PersonelKurs, PersonelCezaOdul.aspx) ortak işleri:
    ///  - sözlük seçenekleri (eski Genel.SozlukYukle: ilk seçenek boş, alt seviyeler "- " ile girintili; ozel ise değer "kod:kayitKod"),
    ///  - sözlük adı (eski Genel.SozlukBilgiGetir),
    ///  - kayda ekli belgeler (eski BelgeListesiDoldur / BelgeKaydet / BelgeGoster / BelgeSil; IIKYServis.PersonelBelgeXxx).
    /// Uç adresleri her sekmenin kendi Uclari sınıfında kendi önekiyle (/api/personelbilgi/ogrenim/..., /dil/..., /kurs/...) tanımlanır.
    /// </summary>
    public static class PersonelEgitimOrtak
    {
        /// <summary>Eski Genel.SozlukYukle(new Sozluk { kayitKod = unsur (2 hane), seviye = 2, ozel }).</summary>
        public static List<object> SozlukSecenekleri(ENUMSozlukUnsurlari unsur, bool ozel, bool bosEkle, ref string hata)
        {
            Sozluk kriter = new Sozluk();
            kriter.kayitKod = ((int)unsur).ToString().PadLeft(2, '0');
            kriter.seviye = 2;
            kriter.ozel = ozel;
            ObjectArray oa = TNS.IKY.Arac.Tanimla().SozlukListele(kriter);

            List<object> secenekler = new List<object>();
            if (bosEkle) secenekler.Add(new { KOD = "0", AD = "" });
            if (oa == null || oa.sonuc == null || !oa.sonuc.islemSonuc || oa.objeler == null)
            {
                if (string.IsNullOrEmpty(hata))
                    hata = PersonelBilgiOrtak.HataMetni(oa != null && oa.sonuc != null ? oa.sonuc.hataStr : "", "Seçenek listesi okunamadı.");
                return secenekler;
            }
            foreach (Sozluk s in oa.objeler)
            {
                string ad = "";
                for (int i = 2; i < s.seviye; i++) ad += "- ";
                ad += s.ad;
                secenekler.Add(new { KOD = ozel ? s.kod + ":" + s.kayitKod : s.kod.ToString(), AD = ad });
            }
            return secenekler;
        }

        /// <summary>Eski Genel.SozlukBilgiGetir(kod).ad; aynı istekte aynı kod bir kez okunur.</summary>
        public static string SozlukAd(int sozlukKod, Dictionary<int, string> onbellek)
        {
            if (sozlukKod == 0) return "";
            if (onbellek != null && onbellek.TryGetValue(sozlukKod, out string varOlan)) return varOlan;
            string ad = "";
            Sozluk kriter = new Sozluk();
            kriter.kod = sozlukKod;
            ObjectArray oa = TNS.IKY.Arac.Tanimla().SozlukListele(kriter);
            if (oa != null && oa.objeler != null)
            {
                foreach (Sozluk s in oa.objeler)
                {
                    ad = s.ad ?? "";
                    break;
                }
            }
            if (onbellek != null) onbellek[sozlukKod] = ad;
            return ad;
        }

        /// <summary>Formdan gelen ondalık sayı (eski OrtakFonksiyonlar.ConvertToDouble; ekranda nokta da girilebilir).</summary>
        public static double Ondalik(string s)
        {
            s = (s ?? "").Trim();
            if (s.Contains(".") && !s.Contains(",")) s = s.Replace(".", ",");
            return OrtakFonksiyonlar.ConvertToDouble(s, 0);
        }

        /// <summary>Ondalık sayıyı ekranda gösterilecek metne çevirir (tr-TR, 0 ise boş değil "0").</summary>
        public static string OndalikYaz(double d)
        {
            return d.ToString(new CultureInfo("tr-TR"));
        }

        // ------------------------------------------------------------------ Belgeler

        /// <summary>Eski BelgeListesiDoldur: PersonelBelgeListele(pKod, tip, tipKayitKod).</summary>
        public static IResult BelgeListesi(HttpContext baglam, IWebHostEnvironment ortam, string sayfaAd, ENUMBELGEFORMTIP tip,
            int? personelKod, int? kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, sayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            int pKod = personelKod ?? 0, tipKayitKod = kod ?? 0;
            List<object> satirlar = new List<object>();
            if (pKod <= 0 || tipKayitKod <= 0) return Results.Json(new { satirlar = satirlar });

            ObjectArray belgeler = Listele(kullanan, pKod, tip, tipKayitKod);
            if (belgeler == null || belgeler.sonuc == null || !belgeler.sonuc.islemSonuc)
                return Results.Json(new { satirlar = satirlar, hata = PersonelBilgiOrtak.HataMetni(belgeler != null && belgeler.sonuc != null ? belgeler.sonuc.hataStr : "", "Belgeler okunamadı.") });
            foreach (PersonelBelgeler detay in belgeler.objeler)
                satirlar.Add(new { kod = detay.kod, pKod = detay.pKod, tipKayitKod = detay.tipKayitKod, tip = detay.tip, belgeAdi = detay.belgeAdi });
            return Results.Json(new { satirlar = satirlar });
        }

        /// <summary>
        /// Eski BelgeKaydet (FileUploadField): çok parçalı form; personelKod, kod (kaydın kodu), dosya.
        /// Eski sayfada kayıt henüz kaydedilmemişse önce Kaydet çalışıyordu; bunu sayfa (JS) yapar ve kodu buraya gönderir.
        /// </summary>
        public static async Task<IResult> BelgeYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, string sayfaAd,
            ENUMBELGEFORMTIP tip)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, sayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();

            IFormCollection form = await baglam.Request.ReadFormAsync();
            int pKod = OrtakFonksiyonlar.ConvertToInt(((string)form["personelKod"] ?? "").Trim(), 0);
            int tipKayitKod = OrtakFonksiyonlar.ConvertToInt(((string)form["kod"] ?? "").Trim(), 0);
            IFormFile dosya = form.Files["dosya"];

            if (tipKayitKod == 0) return Results.Json(new { basarili = false, hata = "Belge eklemek için önce kaydı kaydediniz." });
            if (dosya == null || dosya.Length <= 0) return Results.Json(new { basarili = false, hata = "Dosya seçiniz." });

            byte[] icerik;
            using (MemoryStream bellek = new MemoryStream())
            {
                await dosya.CopyToAsync(bellek);
                icerik = bellek.ToArray();
            }

            PersonelBelgeler yeniBelge = new PersonelBelgeler(pKod, tipKayitKod, (int)tip, Path.GetFileName(dosya.FileName ?? "belge"), icerik.Length);
            Array.Copy(icerik, yeniBelge.belge, icerik.Length);

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelBelgeKaydet(kullanan, yeniBelge);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Belge kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = PersonelBilgiOrtak.Duz(sonuc.bilgiStr) });
        }

        /// <summary>Eski BelgeGoster: PersonelBelgeListesiGetir belgeyi geçici dosyaya yazar ("yol\nad\nuzantı"); dosya indirilir.</summary>
        public static IResult BelgeIndir(HttpContext baglam, IWebHostEnvironment ortam, string sayfaAd, ENUMBELGEFORMTIP tip,
            int? personelKod, int? kod, int? belgeKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, sayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            int bKod = belgeKod ?? 0;
            if (bKod <= 0) return Results.Json(new { hata = "Belge seçiniz." });
            if (!BelgeKayitta(kullanan, personelKod ?? 0, tip, kod ?? 0, bKod)) return Results.Json(new { hata = "Belge bulunamadı." });

            PersonelBelgeler kriter = new PersonelBelgeler();
            kriter.kod = bKod;
            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelBelgeListesiGetir(kullanan, kriter);
            if (!sonuc.islemSonuc)
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, PersonelBilgiOrtak.HataMetni(sonuc.bilgiStr, "Belge bulunamadı.")) });

            string[] parametreler = (sonuc.anahtar ?? "").Split('\n');
            if (parametreler.Length != 3 || !File.Exists(parametreler[0])) return Results.Json(new { hata = "Belge bulunamadı." });

            byte[] icerik = File.ReadAllBytes(parametreler[0]);
            try { File.Delete(parametreler[0]); } catch (Exception) { }

            string ad = Path.GetFileName(string.IsNullOrWhiteSpace(parametreler[1]) ? "belge." + parametreler[2] : parametreler[1]);
            if (!new FileExtensionContentTypeProvider().TryGetContentType(ad, out string tur)) tur = "application/octet-stream";
            baglam.Response.Headers["X-Content-Type-Options"] = "nosniff";
            return Results.File(icerik, tur, ad);
        }

        /// <summary>Eski BelgeSil (DirectMethod): PersonelBelgeSil(kod).</summary>
        public static async Task<IResult> BelgeSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, string sayfaAd,
            ENUMBELGEFORMTIP tip, PersonelBelgeSilIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, sayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            if (istek.belgeKod == 0) return Results.Json(new { basarili = false, hata = "Belge seçiniz." });
            if (!BelgeKayitta(kullanan, istek.personelKod, tip, istek.kod, istek.belgeKod))
                return Results.Json(new { basarili = false, hata = "Belge bulunamadı." });

            PersonelBelgeler kriter = new PersonelBelgeler();
            kriter.kod = istek.belgeKod;
            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelBelgeSil(kullanan, kriter);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Belge silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = PersonelBilgiOrtak.Duz(sonuc.bilgiStr) });
        }

        private static ObjectArray Listele(Kullanici kullanan, int pKod, ENUMBELGEFORMTIP tip, int tipKayitKod)
        {
            PersonelBelgeler belge = new PersonelBelgeler();
            belge.pKod = pKod;
            belge.tip = (int)tip;
            belge.tipKayitKod = tipKayitKod;
            return TNS.IKY.Arac.Tanimla().PersonelBelgeListele(kullanan, belge);
        }

        /// <summary>
        /// Belge indirme / silme yalnızca ekrandaki kaydın belgesi için: istenen belge kodu bu personelin bu kaydının belgeleri
        /// arasında mı (eski sayfada belge kodu yalnızca bu listeden gelebiliyordu; uç doğrudan çağrılabildiği için denetlenir).
        /// </summary>
        private static bool BelgeKayitta(Kullanici kullanan, int pKod, ENUMBELGEFORMTIP tip, int tipKayitKod, int belgeKod)
        {
            if (pKod <= 0 || tipKayitKod <= 0 || belgeKod <= 0) return false;
            ObjectArray belgeler = Listele(kullanan, pKod, tip, tipKayitKod);
            if (belgeler == null || belgeler.sonuc == null || !belgeler.sonuc.islemSonuc || belgeler.objeler == null) return false;
            foreach (PersonelBelgeler b in belgeler.objeler)
                if (b.kod == belgeKod) return true;
            return false;
        }
    }
}
