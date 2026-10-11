using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>Muhasebe (saymanlık) kaydet isteği.</summary>
    public class SaymanlikKayitIstegi
    {
        public string Kod { get; set; }
        public string Ad { get; set; }
        public string Ilgili1 { get; set; }
        public string Ilgili1Unvan { get; set; }
        public string Ilgili2 { get; set; }
        public string Ilgili2Unvan { get; set; }
        public string Adres { get; set; }
        public string Telefon { get; set; }
        public string Faks { get; set; }
        public string VergiNo { get; set; }
        public string EPosta { get; set; }
    }

    public class SaymanlikSilIstegi
    {
        public int Kod { get; set; }
    }

    /// <summary>Bir saymanlık/birimin kayıtlarını başka bir saymanlık/birime taşıma isteği.</summary>
    public class SaymanlikTasiIstegi
    {
        public int EskiSaymanlik { get; set; }
        public string EskiBirim { get; set; }
        public int YeniSaymanlik { get; set; }
        public string YeniBirim { get; set; }
        /// <summary>ENUMSaymanlikTasiIslem değeri.</summary>
        public int Islem { get; set; }
    }

    /// <summary>Muhasebe birimi kaydet / sil isteği. Kaydın anahtarı muhasebe + birim kodu + kurumsal koddur.</summary>
    public class BirimIstegi
    {
        public int Muhasebe { get; set; }
        public string Kod { get; set; }
        public string Ad { get; set; }
        public string Kurum { get; set; }
        public string VergiNo { get; set; }
    }

    /// <summary>
    /// Muhasebe tanım sayfalarının JavaScript'ten çağırdığı uçlar (eski ButceMuhasebe/TanimMuhasebe.aspx ve
    /// TanimMuhasebeBirim.aspx). İşlemler IMUHServis üzerinden yapılır (TNS.MUH.Arac.Tanimla()).
    ///   GET  /api/muhasebe/saymanlik/liste?ara=          : muhasebeler (kod ya da ad ile arama)
    ///   POST /api/muhasebe/saymanlik/kaydet, /sil, /tasi
    ///   GET  /api/muhasebe/birim/tanimlar                 : muhasebe seçenekleri, varsayılan kurumsal kod
    ///   GET  /api/muhasebe/birim/liste?muhasebe=&amp;kod=&amp;ad=&amp;kurum=
    ///   POST /api/muhasebe/birim/kaydet, /sil
    /// Servis metotları kullanıcı almadığı için yetki burada denetlenir: sistem yöneticisi ve muhasebe yöneticisi.
    /// </summary>
    public static class MuhasebeUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/muhasebe/saymanlik/liste", SaymanlikListe);
            app.MapPost("/api/muhasebe/saymanlik/kaydet", SaymanlikKaydet);
            app.MapPost("/api/muhasebe/saymanlik/sil", SaymanlikSil);
            app.MapPost("/api/muhasebe/saymanlik/tasi", SaymanlikTasi);
            app.MapGet("/api/muhasebe/birim/tanimlar", BirimTanimlar);
            app.MapGet("/api/muhasebe/birim/liste", BirimListe);
            app.MapPost("/api/muhasebe/birim/kaydet", BirimKaydet);
            app.MapPost("/api/muhasebe/birim/sil", BirimSil);
        }

        /// <summary>Muhasebe tanım sayfalarını sistem yöneticisi ve muhasebe yöneticisi kullanır (menüdeki Yetki="1,19").</summary>
        public static bool TanimYetkisiVar(Kullanici kullanan)
        {
            return kullanan != null &&
                (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                 kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.MUHASEBEYONETICISI));
        }

        /// <summary>
        /// Saymanlık taşıma, birimleri Harcama (HRC) servisindeki idare tanımından denetler; o servis taşınmadan çalışmaz.
        /// </summary>
        public static bool TasimaKullanilabilir()
        {
            // Servis nesnesi burada oluşturulmaz (ilk oluşturmada bekleyen tablo değişikliklerini çalıştırır); yalnızca
            // bu uygulamada kayıtlı mı ya da yeni API adresi tanımlı mı diye bakılır.
            if (TNS.Api.ServisKayit.Bul("HRC", out Type _, out Func<object> _)) return true;
            string adres = (System.Configuration.ConfigurationManager.AppSettings.Get("HRCServisURL") ?? "").Trim();
            return adres != "" && !adres.EndsWith(".rem", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Taşınabilecek kayıt türleri (ENUMSaymanlikTasiIslem).</summary>
        public static List<object> TasimaIslemleri()
        {
            List<object> islemler = new List<object>();
            foreach (ENUMSaymanlikTasiIslem islem in Enum.GetValues(typeof(ENUMSaymanlikTasiIslem)))
                islemler.Add(new { kod = (int)islem, ad = TNS.MUH.Arac.SaymanlikTasiIslemAdBul(islem) });
            return islemler;
        }

        // ------------------------------------------------------------------ muhasebe (saymanlık)

        private static IResult SaymanlikListe(HttpContext baglam, string ara)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!TanimYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            MuhasebeBilgisi kriter = new MuhasebeBilgisi();
            kriter.kistasKodAd = (ara ?? "").Trim();

            ObjectArray sonuc = TNS.MUH.Arac.Tanimla().MuhasebeListele(kriter);
            if (!sonuc.sonuc.islemSonuc)
                return Results.Json(new { hata = Metin.Muhasebe("IslemHata") });

            List<object> satirlar = new List<object>();
            foreach (MuhasebeBilgisi m in sonuc.objeler)
                satirlar.Add(new
                {
                    kod = m.kod,
                    ad = m.ad,
                    ilgili1 = m.ilgili1,
                    ilgili1Unvan = m.ilgili1Unvan,
                    ilgili2 = m.ilgili2,
                    ilgili2Unvan = m.ilgili2Unvan,
                    adres = m.adres,
                    telefon = m.telefon,
                    faks = m.faks,
                    vergiNo = m.vergiNo,
                    ePosta = m.ePosta
                });
            return Results.Json(new { satirlar = satirlar });
        }

        private static async Task<IResult> SaymanlikKaydet(HttpContext baglam, SaymanlikKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!TanimYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null) return Results.BadRequest();

            int kod = OrtakFonksiyonlar.ConvertToInt((istek.Kod ?? "").Trim(), 0);
            string ad = (istek.Ad ?? "").Trim();
            string ePosta = (istek.EPosta ?? "").Trim();

            List<string> hatalar = new List<string>();
            if ((istek.Kod ?? "").Trim() == "") hatalar.Add(Metin.Muhasebe("MUHGNL147"));
            else if (kod <= 0) hatalar.Add(Metin.Muhasebe("KodSayisal"));
            if (ad == "") hatalar.Add(Metin.Muhasebe("MUHGNL190"));
            if (ePosta != "" && !OrtakFonksiyonlar.MailAdresiDogrumu(ePosta)) hatalar.Add(Metin.Muhasebe("EPostaHatali"));
            if (hatalar.Count > 0) return Hata(string.Join(" ", hatalar));

            MuhasebeBilgisi muhasebe = new MuhasebeBilgisi();
            muhasebe.kod = kod;
            muhasebe.ad = ad;
            muhasebe.ilgili1 = istek.Ilgili1 ?? "";
            muhasebe.ilgili2 = istek.Ilgili2 ?? "";
            muhasebe.ilgili1Unvan = istek.Ilgili1Unvan ?? "";
            muhasebe.ilgili2Unvan = istek.Ilgili2Unvan ?? "";
            muhasebe.adres = istek.Adres ?? "";
            muhasebe.telefon = istek.Telefon ?? "";
            muhasebe.faks = istek.Faks ?? "";
            muhasebe.ePosta = ePosta;
            muhasebe.vergiNo = (istek.VergiNo ?? "").Trim();

            ObjectArray muhasebeler = new ObjectArray();
            muhasebeler.objeler.Add(muhasebe);

            Sonuc sonuc = TNS.MUH.Arac.Tanimla().MuhasebeKaydet(muhasebeler);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc));

            return Results.Json(new { basarili = true, kod = kod, bilgi = Metin.Muhasebe("MUHGNL143") });
        }

        private static async Task<IResult> SaymanlikSil(HttpContext baglam, SaymanlikSilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!TanimYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Kod <= 0) return Hata(Metin.Muhasebe("MUHGNL183"));

            MuhasebeBilgisi muhasebe = new MuhasebeBilgisi();
            muhasebe.kod = istek.Kod;
            ObjectArray muhasebeler = new ObjectArray();
            muhasebeler.objeler.Add(muhasebe);

            Sonuc sonuc = TNS.MUH.Arac.Tanimla().MuhasebeSil(muhasebeler);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc));

            return Results.Json(new { basarili = true, bilgi = Metin.Muhasebe("MUHGNL154") });
        }

        private static async Task<IResult> SaymanlikTasi(HttpContext baglam, SaymanlikTasiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!TanimYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null) return Results.BadRequest();

            string eskiBirim = (istek.EskiBirim ?? "").Trim();
            string yeniBirim = (istek.YeniBirim ?? "").Trim();
            if (istek.EskiSaymanlik <= 0 || eskiBirim == "" || istek.YeniSaymanlik <= 0 || yeniBirim == "")
                return Hata(Metin.Muhasebe("TasiEksik"));
            if (!Enum.IsDefined(typeof(ENUMSaymanlikTasiIslem), istek.Islem))
                return Hata(Metin.Muhasebe("TasiIslemSec"));
            if (!TasimaKullanilabilir())
                return Hata(Metin.Muhasebe("TasiHazirDegil"));

            TNSCollection islemler = new TNSCollection();
            islemler.Add(istek.Islem);

            Sonuc sonuc = TNS.MUH.Arac.Tanimla().MuhasebeBirimiTasi(kullanan, istek.EskiSaymanlik, eskiBirim, istek.YeniSaymanlik, yeniBirim, islemler);

            // Eski sayfadaki gibi: bilgi varsa bilgi, yoksa hata gösterilir. Servis satırları <br /> ile ayırır.
            string bilgi = SatirlaraCevir(sonuc.bilgiStr);
            if (bilgi != "") return Results.Json(new { basarili = true, bilgi = bilgi });
            return Hata(SatirlaraCevir(HataMetni(sonuc)));
        }

        // ------------------------------------------------------------------ muhasebe birimi

        private static IResult BirimTanimlar(HttpContext baglam)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!TanimYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            List<object> muhasebeler = new List<object>();
            ObjectArray sonuc = TNS.MUH.Arac.Tanimla().MuhasebeListele(new MuhasebeBilgisi());
            if (sonuc.sonuc.islemSonuc)
                foreach (MuhasebeBilgisi m in sonuc.objeler)
                    muhasebeler.Add(new { kod = m.kod, ad = m.kod.ToString().PadLeft(5, '0') + " - " + m.ad });

            string kurum;
            try
            {
                kurum = TNS.DEG.Arac.VarsayilanKurumBul() ?? "";
            }
            catch (Exception)
            {
                kurum = "";
            }

            return Results.Json(new { muhasebeler = muhasebeler, varsayilanKurum = kurum });
        }

        /// <summary>muhasebe boş gelebilir (bütün muhasebeler); bu yüzden dizgi olarak alınır (boş değer int?'e bağlanamaz).</summary>
        private static IResult BirimListe(HttpContext baglam, string muhasebe, string kod, string ad, string kurum)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!TanimYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            TNS.MUH.MuhasebeBirim kriter = new TNS.MUH.MuhasebeBirim();
            kriter.muhasebe = Math.Max(0, OrtakFonksiyonlar.ConvertToInt((muhasebe ?? "").Trim(), 0));
            kriter.birimKod = (kod ?? "").Trim();
            kriter.birimAd = (ad ?? "").Trim();
            kriter.kurum = (kurum ?? "").Trim();

            ObjectArray sonuc = TNS.MUH.Arac.Tanimla().MuhasebeBirimListele(kriter);
            if (!sonuc.sonuc.islemSonuc)
                return Results.Json(new { hata = Metin.Muhasebe("IslemHata") });

            List<object> satirlar = new List<object>();
            foreach (TNS.MUH.MuhasebeBirim b in sonuc.objeler)
                satirlar.Add(new
                {
                    muhasebe = b.muhasebe,
                    muhasebeAd = b.muhasebeAd,
                    kod = b.birimKod,
                    kurum = b.kurum,
                    ad = b.birimAd,
                    vergiNo = b.vno,
                    kapali = b.pasif > 0
                });
            return Results.Json(new { satirlar = satirlar });
        }

        private static async Task<IResult> BirimKaydet(HttpContext baglam, BirimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!TanimYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null) return Results.BadRequest();

            string kod = (istek.Kod ?? "").Trim();
            string ad = (istek.Ad ?? "").Trim();
            string kurum = (istek.Kurum ?? "").Trim();

            List<string> hatalar = new List<string>();
            if (istek.Muhasebe <= 0) hatalar.Add(Metin.Muhasebe("MUHGNL147"));
            if (kod == "") hatalar.Add(Metin.Muhasebe("MUHGNL192"));
            else if (kod.Length != 3 || OrtakFonksiyonlar.ConvertToInt(kod, -1) < 0) hatalar.Add(Metin.Muhasebe("BirimKodBicim"));
            if (ad == "") hatalar.Add(Metin.Muhasebe("MUHGNL193"));
            if (kurum == "") hatalar.Add(Metin.Muhasebe("MUHGNL194"));
            if (hatalar.Count > 0) return Hata(string.Join(" ", hatalar));

            TNS.MUH.MuhasebeBirim birim = new TNS.MUH.MuhasebeBirim();
            birim.muhasebe = istek.Muhasebe;
            birim.birimKod = kod;
            birim.birimAd = ad;
            birim.kurum = kurum;
            birim.vno = (istek.VergiNo ?? "").Trim();

            ObjectArray birimler = new ObjectArray();
            birimler.objeler.Add(birim);

            Sonuc sonuc = TNS.MUH.Arac.Tanimla().MuhasebeBirimKaydet(birimler);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc));

            return Results.Json(new { basarili = true, bilgi = Metin.Muhasebe("MUHGNL143") });
        }

        private static async Task<IResult> BirimSil(HttpContext baglam, BirimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!TanimYetkisiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            string kod = (istek?.Kod ?? "").Trim();
            string kurum = (istek?.Kurum ?? "").Trim();
            if (istek == null || istek.Muhasebe <= 0 || kod == "" || kurum == "")
                return Hata(Metin.Muhasebe("MUHGNL183"));

            TNS.MUH.MuhasebeBirim birim = new TNS.MUH.MuhasebeBirim();
            birim.muhasebe = istek.Muhasebe;
            birim.birimKod = kod;
            birim.kurum = kurum;

            ObjectArray birimler = new ObjectArray();
            birimler.objeler.Add(birim);

            Sonuc sonuc = TNS.MUH.Arac.Tanimla().MuhasebeBirimSil(birimler);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc));

            return Results.Json(new { basarili = true, bilgi = Metin.Muhasebe("MUHGNL154") });
        }

        // ------------------------------------------------------------------ yardımcılar

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        /// <summary>
        /// Servisin hata metni; boşsa ya da yakalanmış bir istisnanın dökümüyse (servis e.ToString() döndürür) genel hata metni.
        /// </summary>
        private static string HataMetni(Sonuc sonuc)
        {
            string hata = sonuc.hataStr ?? "";
            if (hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at "))
                return Metin.Muhasebe("IslemHata");
            return hata;
        }

        private static string SatirlaraCevir(string html)
        {
            return (html ?? "").Replace("<br />", "\n").Replace("<br>", "\n").Trim();
        }
    }
}
