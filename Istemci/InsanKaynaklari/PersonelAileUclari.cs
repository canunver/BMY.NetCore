using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Kaydet isteği (eski btnKaydet_Click'in ekrandan okuduğu alanlar).</summary>
    public class PersonelAileIstegi
    {
        public int personelKod { get; set; }
        public int kod { get; set; }
        public string tcKimlikNo { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public string dogumTarihi { get; set; }
        public int cinsiyet { get; set; }
        public int yakinlik { get; set; }
        public int durum { get; set; }
        public int ogrenimTipi { get; set; }
        public string meslek { get; set; }
        public int sakatlikDerecesi { get; set; }
        public bool agi { get; set; }
        public bool bakmaklaYukumlu { get; set; }
        public string yukumlulukBitisTarihi { get; set; }
    }

    /// <summary>Sil / belge sil isteği.</summary>
    public class PersonelAileKodIstegi
    {
        public int personelKod { get; set; }
        public int kod { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri "Aile" sekmesi (eski InsanKaynaklari/PersonelAile.aspx; yeni sayfa InsanKaynaklari/PersonelAile) uçları:
    ///   GET  /api/personelbilgi/aile/tanim                       Açılır listeler (eski YakininDurumunuDoldur, YakinlikDerecesiDoldur, CinsiyetDoldur, YakininOgrenimTipiDoldur)
    ///   GET  /api/personelbilgi/aile/liste?personelKod=          Aile bireyleri (eski Listele)
    ///   GET  /api/personelbilgi/aile/oku?personelKod=&amp;kod=       Seçilen birey + belgeleri (eski SatirSecildi + BelgeListesiDoldur)
    ///   GET  /api/personelbilgi/aile/kimlik?tcKimlikNo=          Nüfus (KPS / MERNİS) bilgisi (eski KimlikBilgisiGetir)
    ///   POST /api/personelbilgi/aile/kaydet                      (eski btnKaydet_Click)
    ///   POST /api/personelbilgi/aile/sil                         (eski btnSil_Click)
    ///   GET  /api/personelbilgi/aile/belgeler?personelKod=&amp;kod=  (eski BelgeListesiDoldur)
    ///   POST /api/personelbilgi/aile/belge-ekle (multipart: personelKod, kod, dosya)   (eski BelgeKaydet)
    ///   GET  /api/personelbilgi/aile/belge?kod=                  (eski BelgeGoster)
    ///   POST /api/personelbilgi/aile/belge-sil                   (eski BelgeSil)
    /// Nüfus Cüzdanı Örneği / İkametgah İlmühaberi düğmeleri Kişisel sekmesinin ortak ucunu (/api/personelbilgi/rapor/nufus) çağırır.
    /// </summary>
    public static class PersonelAileUclari
    {
        private const string SayfaAd = "PersonelAile";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/aile/tanim", Tanim);
            app.MapGet("/api/personelbilgi/aile/liste", Liste);
            app.MapGet("/api/personelbilgi/aile/oku", Oku);
            app.MapGet("/api/personelbilgi/aile/kimlik", Kimlik);
            app.MapPost("/api/personelbilgi/aile/kaydet", Kaydet);
            app.MapPost("/api/personelbilgi/aile/sil", Sil);
            app.MapGet("/api/personelbilgi/aile/belgeler", Belgeler);
            app.MapPost("/api/personelbilgi/aile/belge-ekle", BelgeEkle);
            app.MapGet("/api/personelbilgi/aile/belge", BelgeGoster);
            app.MapPost("/api/personelbilgi/aile/belge-sil", BelgeSil);
        }

        private static IIKYServis Servis()
        {
            return TNS.IKY.Arac.Tanimla();
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        // ---------------------------------------------------------------- açılır listeler

        /// <summary>Eski Genel.SozlukYukle: ilk seçenek boş, alt seviyeler "- " ile girintili.</summary>
        private static List<object> SozlukYukle(ENUMSozlukUnsurlari unsur)
        {
            List<object> l = new List<object>();
            l.Add(new { KOD = 0, AD = "" });
            Sozluk kriter = new Sozluk();
            kriter.kayitKod = ((int)unsur).ToString().PadLeft(2, '0');
            kriter.seviye = 2;
            ObjectArray oa = Servis().SozlukListele(kriter);
            if (oa != null && oa.objeler != null)
                foreach (Sozluk s in oa.objeler)
                {
                    string ad = "";
                    for (int i = 2; i < s.seviye; i++) ad += "- ";
                    l.Add(new { KOD = s.kod, AD = ad + s.ad });
                }
            return l;
        }

        private static List<object> SozlukListesi(System.Collections.ArrayList liste)
        {
            List<object> l = new List<object>();
            if (liste != null)
                foreach (Sozluk s in liste)
                    l.Add(new { KOD = s.kod, AD = s.ad });
            return l;
        }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return Results.Json(new
                {
                    yakinliklar = SozlukListesi(TNS.IKY.Arac.YakinlikListe),
                    cinsiyetler = SozlukListesi(TNS.IKY.Arac.CinsiyetListe),
                    durumlar = SozlukYukle(ENUMSozlukUnsurlari.YAKININDURUMU),
                    ogrenimTipleri = SozlukYukle(ENUMSozlukUnsurlari.AILEOGRENIMTIPI)
                });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Liste bilgileri okunamadı." });
            }
        }

        // ---------------------------------------------------------------- liste / oku

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            int pKod = personelKod ?? 0;
            List<object> satirlar = new List<object>();
            if (pKod == 0) return Results.Json(new { satirlar = satirlar });

            ObjectArray pler = Servis().PersonelAileListele(kullanan, 0, pKod, null, null, null, 0);
            if (!pler.sonuc.islemSonuc) return Results.Json(new { satirlar = satirlar, hata = PersonelBilgiOrtak.HataMetni(pler.sonuc.hataStr, "Aile bilgileri okunamadı.") });

            foreach (TNS.IKY.PersonelAile p in pler.objeler)
                satirlar.Add(new { KOD = p.kod, TCKIMLIKNO = p.tcKimlikNo, ADSOYAD = p.ad + ' ' + p.soyad, YAKINLIK = p.yakinlikDerecesiAd });
            return Results.Json(new { satirlar = satirlar });
        }

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            int k = kod ?? 0, pKod = personelKod ?? 0;
            if (k == 0) return Results.Json(new { hata = "Aile bireyi seçilmedi." });

            ObjectArray pler = Servis().PersonelAileListele(kullanan, k, pKod, null, null, null, 0);
            if (!pler.sonuc.islemSonuc) return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(pler.sonuc.hataStr, "Aile bilgisi okunamadı.") });

            object bilgi = null;
            foreach (TNS.IKY.PersonelAile p in pler.objeler)
            {
                bilgi = new
                {
                    kod = p.kod,
                    tcKimlikNo = p.tcKimlikNo ?? "",
                    ad = p.ad ?? "",
                    soyad = p.soyad ?? "",
                    dogumTarihi = PersonelBilgiOrtak.TarihYaz(p.dogumTarihi),
                    cinsiyet = (int)p.cinsiyet,
                    yakinlik = (int)p.yakinlik,
                    durum = p.durum,
                    ogrenimTipi = p.ogrenimTipi,
                    meslek = p.meslek ?? "",
                    sakatlikDerecesi = p.sakatlikDerecesi,
                    agi = p.agi == 1,
                    bakmaklaYukumlu = p.bakmaklaYukumlu == 1,
                    yukumlulukBitisTarihi = PersonelBilgiOrtak.TarihYaz(p.yukumlulukBitisTarihi)
                };
            }
            if (bilgi == null) return Results.Json(new { hata = "Aile bilgisi bulunamadı." });
            return Results.Json(new { bilgi = bilgi, belgeler = BelgeListesi(kullanan, pKod, k) });
        }

        // ---------------------------------------------------------------- kimlik (eski KimlikBilgisiGetir + Genel.KimlikBilgisiGetir)

        private static IResult Kimlik(HttpContext baglam, IWebHostEnvironment ortam, string tcKimlikNo)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            string tc = (tcKimlikNo ?? "").Trim();
            if (tc == "") return Hata("Personel T.C.Kimlik No bilgisi boş bırakılamaz.");

            string ad, soyad, dogum;
            int cinsiyet;
            TNSDateTime dogumTarihi;
            try
            {
                // Eski Genel.KimlikBilgisiGetir: KPS nüfus kayıt örneği.
                TNS.KPM.Istek istek = new TNS.KPM.Istek();
                istek.istekTur = (int)TNS.KPM.ENUMIstekTur.NUFUSCUZDANORNEK;
                istek.tcKimlikNo = tc;
                istek.adSoyad = kullanan.ADSOYAD;
                istek.islemYapan = kullanan.kullaniciKodu;
                istek.islemTarih = new TNSDateTime(DateTime.Now);
                istek.entegrasyon = true;
                ObjectArray nufusList = TNS.KPM.Arac.Tanimla().NufusKayitOrnekRaporla(kullanan, istek);
                if (!nufusList.sonuc.islemSonuc)
                    throw new Exception("Nüfus sorgusu hata ile sonuçlandı: " + nufusList.sonuc.hataStr);
                if (nufusList.ObjeSayisi == 0)
                    throw new Exception("Nüfus Sorgusu Boş Kayıt Döndürdü");
                TNS.KPM.Kisi nufusOrnek = (TNS.KPM.Kisi)nufusList.objeler[0];
                tc = nufusOrnek.tcKimlikNo;
                ad = nufusOrnek.ad;
                soyad = nufusOrnek.soyad;
                dogumTarihi = new TNSDateTime(nufusOrnek.dogumTarihi.ToString());
                cinsiyet = (int)(nufusOrnek.cinsiyet == 2 ? ENUMCINSIYET.KADIN : ENUMCINSIYET.ERKEK);
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Nüfus bilgisi okunamadı."));
            }

            if (!string.IsNullOrEmpty(ad))
            {
                dogum = PersonelBilgiOrtak.TarihYaz(dogumTarihi);
                return Results.Json(new { basarili = true, tcKimlikNo = tc ?? "", ad = ad, soyad = soyad ?? "", dogumTarihi = dogum, cinsiyet = cinsiyet });
            }

            // Kurumda nüfus cüzdanı denetimi için yetki yoksa (eski GenelIslemler.MernisOku).
            KullaniciVAN kVAN = TNS.KYM.Arac.Tanimla().MernisBilgiOku(kullanan, (tcKimlikNo ?? "").Trim(), dogumTarihi);
            if (kVAN != null && kVAN.sonuc != null && kVAN.sonuc.islemSonuc && kVAN.kullanici != null)
            {
                // Eski sayfa "Erkek" için listenin ilk öğesini (Kadın) seçiyordu; burada cinsiyet adına göre doğru değer verilir.
                int c = kVAN.kullanici.cinsiyet == "Erkek" ? (int)ENUMCINSIYET.ERKEK : (int)ENUMCINSIYET.KADIN;
                return Results.Json(new
                {
                    basarili = true,
                    tcKimlikNo = (tcKimlikNo ?? "").Trim(),
                    ad = kVAN.kullanici.ad ?? "",
                    soyad = kVAN.kullanici.soyad ?? "",
                    dogumTarihi = PersonelBilgiOrtak.TarihYaz(kVAN.kullanici.dogumTarihi),
                    cinsiyet = c
                });
            }
            // Eski: bulunamazsa ad / soyad boşaltılır.
            return Results.Json(new { basarili = true, tcKimlikNo = (tcKimlikNo ?? "").Trim(), ad = "", soyad = "", dogumTarihi = (string)null, cinsiyet = (int?)null });
        }

        // ---------------------------------------------------------------- kaydet / sil

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelAileIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            TNS.IKY.PersonelAile kisininYakini = new TNS.IKY.PersonelAile();
            kisininYakini.kod = istek.kod;
            kisininYakini.personelKod = istek.personelKod;
            kisininYakini.tcKimlikNo = (istek.tcKimlikNo ?? "").Trim();
            kisininYakini.ad = (istek.ad ?? "").Trim();
            kisininYakini.soyad = (istek.soyad ?? "").Trim();
            kisininYakini.cinsiyet = (ENUMCINSIYET)istek.cinsiyet;
            kisininYakini.yakinlik = (ENUMYakinlikDerecesi)istek.yakinlik;
            kisininYakini.durum = istek.durum;
            kisininYakini.dogumTarihi = PersonelBilgiOrtak.TnsTarih(istek.dogumTarihi);
            kisininYakini.ogrenimTipi = istek.ogrenimTipi;
            kisininYakini.meslek = (istek.meslek ?? "").Trim();
            kisininYakini.sakatlikDerecesi = istek.sakatlikDerecesi;
            kisininYakini.agi = istek.agi ? 1 : 0;
            kisininYakini.bakmaklaYukumlu = istek.bakmaklaYukumlu ? 1 : 0;
            if (kisininYakini.bakmaklaYukumlu == 1)
                kisininYakini.yukumlulukBitisTarihi = PersonelBilgiOrtak.TnsTarih(istek.yukumlulukBitisTarihi);

            Sonuc sonuc = Servis().PersonelAileKaydet(kullanan, kisininYakini);
            if (!sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Aile bilgisi kaydedilemedi."));
            return Results.Json(new { basarili = true, kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, istek.kod), bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, PersonelAileKodIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            if (istek.kod == 0) return Hata("Lütfen silmek istediğiniz Birey bilgisini seçin.");

            TNS.IKY.PersonelAile d = new TNS.IKY.PersonelAile();
            d.kod = istek.kod;
            d.personelKod = istek.personelKod;
            Sonuc sonuc = Servis().PersonelAileSil(kullanan, d);
            if (!sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Aile bilgisi silinemedi."));
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla gerçekleşti." });
        }

        // ---------------------------------------------------------------- belgeler

        private static List<object> BelgeListesi(Kullanici kullanan, int personelKod, int kod)
        {
            PersonelBelgeler belge = new PersonelBelgeler();
            belge.pKod = personelKod;
            belge.tip = (int)ENUMBELGEFORMTIP.PERSONELAILE;
            belge.tipKayitKod = kod;

            ObjectArray belgeler = Servis().PersonelBelgeListele(kullanan, belge);
            List<object> liste = new List<object>();
            if (belgeler != null && belgeler.objeler != null)
                foreach (PersonelBelgeler detay in belgeler.objeler)
                    liste.Add(new { KOD = detay.kod, PKOD = detay.pKod, TIPKAYITKOD = detay.tipKayitKod, TIP = detay.tip, BELGEADI = detay.belgeAdi });
            return liste;
        }

        private static IResult Belgeler(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((kod ?? 0) == 0) return Results.Json(new { belgeler = new List<object>() });
            return Results.Json(new { belgeler = BelgeListesi(kullanan, personelKod ?? 0, kod ?? 0) });
        }

        /// <summary>
        /// Eski BelgeKaydet: birey kaydedilmemişse sayfa önce Kaydet'i çağırır (JS), sonra belge bireyin koduyla kaydedilir.
        /// </summary>
        private static async Task<IResult> BelgeEkle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            IFormCollection form = await baglam.Request.ReadFormAsync();
            int personelKod = OrtakFonksiyonlar.ConvertToInt(form["personelKod"].ToString(), 0);
            int kod = OrtakFonksiyonlar.ConvertToInt(form["kod"].ToString(), 0);
            if (kod == 0) return Hata("Belgenin kim için kaydedileceğini seçin.");
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length <= 0) return Hata("Yüklemek istediğiniz dosyanın içi boş.");
            if (dosya.Length > int.MaxValue) return Hata("Dosya çok büyük.");

            int belgeBoyutu = (int)dosya.Length;
            PersonelBelgeler yeniBelge = new PersonelBelgeler(personelKod, kod, (int)ENUMBELGEFORMTIP.PERSONELAILE, dosya.FileName ?? "belge", belgeBoyutu);
            using (MemoryStream ms = new MemoryStream(belgeBoyutu))
            {
                await dosya.CopyToAsync(ms);
                Array.Copy(ms.GetBuffer(), yeniBelge.belge, belgeBoyutu);
            }

            Sonuc sonuc = Servis().PersonelBelgeKaydet(kullanan, yeniBelge);
            string hata = sonuc.islemSonuc ? "" : PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Belge kaydedilemedi.");
            return Results.Json(new
            {
                basarili = sonuc.islemSonuc,
                hata = hata,
                bilgi = PersonelBilgiOrtak.Duz(sonuc.bilgiStr),
                belgeler = BelgeListesi(kullanan, personelKod, kod)
            });
        }

        private static IResult BelgeGoster(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((kod ?? 0) == 0) return Results.Json(new { hata = "Belge seçiniz." });
            try
            {
                Sonuc dosyaKonumuAdiUzantisi = Servis().PersonelBelgeListesiGetir(kullanan, new PersonelBelgeler { kod = kod ?? 0 });
                if (dosyaKonumuAdiUzantisi == null || !dosyaKonumuAdiUzantisi.islemSonuc)
                    return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(dosyaKonumuAdiUzantisi?.hataStr, "Belge okunamadı.") });
                string[] parametreler = (dosyaKonumuAdiUzantisi.anahtar ?? "").Split('\n');
                if (parametreler.Length != 3 || !File.Exists(parametreler[0])) return Results.Json(new { hata = "Belge okunamadı." });
                return DosyaIslem.DosyaGonder(parametreler[0], parametreler[1], true, parametreler[2]);
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Belge okunamadı." });
            }
        }

        private static async Task<IResult> BelgeSil(HttpContext baglam, IWebHostEnvironment ortam, PersonelAileKodIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            if (istek.kod == 0) return Hata("Belge seçiniz.");
            Sonuc sonuc = Servis().PersonelBelgeSil(kullanan, new PersonelBelgeler { kod = istek.kod });
            if (!sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Belge silinemedi."));
            return Results.Json(new { basarili = true, bilgi = "Belge silindi." });
        }
    }
}
