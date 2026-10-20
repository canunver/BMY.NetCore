using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.HRC;
using TNS.KYM;

namespace Istemci.HarcamaSurecleri
{
    /// <summary>Kaydet isteği: formdaki alanlar (eski txt* / chk* / hdn* alanları).</summary>
    public class FirmaTanimIstegi
    {
        public int kod { get; set; }
        public string muhasebe { get; set; }
        /// <summary>Seçilen / okunan kaydın vergi numarası (eski hdnVNo); değiştirilmişse kayıt yapılmaz.</summary>
        public string eskiVergiNo { get; set; }
        /// <summary>GİB'den gelen ad / vergi dairesi / adres (eski hdnAd, hdnVD, hdnAdres; GİB bağlantısı varken kullanılır).</summary>
        public string gibAd { get; set; }
        public string gibVergiDairesi { get; set; }
        public string gibAdres { get; set; }
        public string vergiNo { get; set; }
        public string vergiDairesi { get; set; }
        public string ad { get; set; }
        public string adres { get; set; }
        public string adresNo { get; set; }
        /// <summary>0: GİB'ten veri al (T.C.), 1: Manuel giriş (yabancı).</summary>
        public int uyruk { get; set; }
        public bool karaListe { get; set; }
        public bool yasakli { get; set; }
        public bool gorulmesin { get; set; }
        public bool fatura { get; set; }
        public string banka { get; set; }
        public string firmaYetkilisi { get; set; }
        public string aciklama { get; set; }
        public string cepTel { get; set; }
        public string telefon { get; set; }
        public string hesapNo { get; set; }
        public string eMail { get; set; }
        public string web { get; set; }
        public string faks { get; set; }
        public string bicSwift { get; set; }
        public string kisiUnvan { get; set; }
        public string derece { get; set; }
        public string ekGosterge { get; set; }
    }

    /// <summary>
    /// Firma / Kişi Tanımlama (eski HarcamaSurecleri/TanimFirmaKisi.aspx) uçları:
    ///   GET  /api/firmatanim/liste?muhasebe=&amp;ad=&amp;vergiNo=     eski btnAra_Click / Listele (FirmaBilgisiListele) → { satirlar } / { hata }
    ///   GET  /api/firmatanim/kimlik?muhasebe=&amp;araAd=&amp;vergiNo=  eski imgKimlik_Click (GosterVergiNo + ListeleGib, GİB'den unvan)
    ///   POST /api/firmatanim/kaydet                         eski btnKaydet_Click (FirmaBilgisiKaydet) → { basarili, vergiNo } / { hata }
    ///   GET  /api/firmatanim/yazdir?muhasebe=&amp;ad=&amp;vergiNo=    eski btnYaz_Click, Excel (RaporSablon/HRC/Firmalar.xltx)
    /// İş kuralları değiştirilmeden çağrılır. GİB sorgusu BorcuYokBelgesiUclari.FirmaUnvan (eski Ortak.FirmaUnvan) ile yapılır.
    /// </summary>
    public static class FirmaTanimUclari
    {
        public const string SayfaAdresi = "../HarcamaSurecleri/TanimFirmaKisi.aspx";
        private static readonly string[] UstSayfalar = { FirmaKisiListeUclari.SayfaAdresi };
        private const string Uc = "/api/firmatanim/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "kimlik", Kimlik);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapGet(Uc + "yazdir", Yazdir);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi, UstSayfalar);
        }

        /// <summary>Eski GelirlerBaglantisiVar (appSettings GelirlerBaglantisiVarMi).</summary>
        public static bool GelirlerBaglantisiVar()
        {
            return OrtakFonksiyonlar.ConvertToInt(Ayar.Oku("GelirlerBaglantisiVarMi").Trim(), 0) != 0;
        }

        /// <summary>Eski Page_Load: muhasebe kullanıcının çalıştığı kurumdan, yoksa adresten; ikisi de yoksa saklı MUHASEBE değişkeni.</summary>
        public static string VarsayilanMuhasebe(Kullanici kullanan, string adrestekiMuhasebe)
        {
            if (!string.IsNullOrEmpty(kullanan.calistigiKurum)) return kullanan.calistigiKurum;
            if (!string.IsNullOrWhiteSpace(adrestekiMuhasebe)) return adrestekiMuhasebe.Trim();
            return FirmaKisiListeUclari.VarsayilanMuhasebe(kullanan) is int m && m > 0 ? m.ToString() : "";
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static string Temiz(string s) { return (s ?? "").Trim(); }
        private static string Tirnaksiz(string s) { return (s ?? "").Replace("'", ""); }

        // Eski Listele'deki satır alanları (ad alanları da aynı).
        private static object Satir(FirmaBilgisi y)
        {
            return new
            {
                ad = Tirnaksiz(y.ad),
                tel = y.telefon ?? "",
                faks = y.faks ?? "",
                vno = Tirnaksiz(y.vergiNo),
                vd = Tirnaksiz(y.vergiDairesi),
                kod = y.kod,
                banka = Tirnaksiz(y.banka),
                hesapNo = Tirnaksiz(y.hesapNo),
                muhasebe = y.muhasebe,
                birim = y.birim ?? "",
                firmaYetkilisi = Tirnaksiz(y.firmaYetkilisi),
                eMail = y.eMail ?? "",
                adres = Tirnaksiz(y.adres),
                adresNo = y.adresNo ?? "",
                uyruk = y.uyruk,
                karaListe = y.karaListe,
                gorulmesin = y.gorulmesin,
                yasakli = y.yasakli,
                cepTel = y.cepTel ?? "",
                aciklama = y.aciklama ?? "",
                web = y.web ?? "",
                fatura = y.fatura,
                bicSwift = Tirnaksiz(y.bicSwift),
                unvan = Tirnaksiz(y.kisiUnvan),
                ekGosterge = y.ekGosterge,
                derece = y.derece
            };
        }

        // Eski KriterliTopla
        private static FirmaBilgisi Kriter(string muhasebe, string ad, string vergiNo)
        {
            FirmaBilgisi y = new FirmaBilgisi();
            y.muhasebe = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0);
            y.ad = ad ?? "";
            y.vergiNo = vergiNo ?? "";
            return y;
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string muhasebe, string ad, string vergiNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            FirmaBilgisi y = Kriter(muhasebe, ad, vergiNo);
            if (y.muhasebe == 0)
                return Results.Json(new { hata = "Muhasebe kodu boş bırakılamaz." });
            if (string.IsNullOrWhiteSpace(y.ad) && string.IsNullOrWhiteSpace(y.vergiNo))
                return Results.Json(new { hata = "Lütfen ad veya vergi numarası alanlarından birini doldurunuz. Tüm firmaları listelemek için ad alanına % giriniz." });

            try
            {
                ObjectArray bilgi = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, y);
                List<object> satirlar = new List<object>();
                if (bilgi != null && bilgi.sonuc != null && bilgi.sonuc.islemSonuc)
                    foreach (FirmaBilgisi f in bilgi.objeler)
                        satirlar.Add(Satir(f));
                return Results.Json(new { satirlar });
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = ex.Message });
            }
        }

        // Eski imgKimlik_Click + ListeleGib + GosterVergiNo.
        //   rastgele: GİB bağlantısı yokken boş numara yerine üretilen numara.
        //   kayit: bu muhasebede bu numarayla kayıtlı firma (form doldurulur).
        //   gib: GİB'den gelen ad / vergi dairesi / adres. temizle: hata ya da bulunamadı (form boşaltılır, numara kalır).
        private static IResult Kimlik(HttpContext baglam, IWebHostEnvironment ortam, string muhasebe, string araAd, string vergiNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            bool gelirler = GelirlerBaglantisiVar();
            string vno = Temiz(vergiNo);
            if (vno == "")
            {
                if (gelirler) return Results.Json(new { hata = "Vergi No Alanını Doldurunuz!" });
                return Results.Json(new { rastgele = RastgeleSayiUret(10) });
            }
            if (!gelirler) return Results.Json(new { });

            try
            {
                object kayit = null;
                ObjectArray bilgi = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, Kriter(muhasebe, araAd, vno));
                if (bilgi != null && bilgi.sonuc != null && bilgi.sonuc.islemSonuc && bilgi.ObjeSayisi > 0)
                    kayit = Satir((FirmaBilgisi)bilgi.objeler[0]);

                string unvan = "", vd = "", adres = "", hata = "";
                BorcuYokBelgesiUclari.FirmaUnvan(vno, kullanan.mernis, ref unvan, ref vd, ref adres);

                if (unvan.Contains("java.rmi.RemoteException"))
                {
                    int posHata = unvan.IndexOf("java.lang.Exception");
                    int posHataBitis = unvan.IndexOf("!");
                    if (posHata >= 0 && posHataBitis > posHata + 20)
                        hata = unvan.Substring(posHata + 20, posHataBitis - (posHata + 20));
                    else
                        hata = "Lütfen daha sonra tekrar deneyiniz.";
                }
                if (hata == "" && unvan.ToUpper().IndexOf("BULUNAMAD") > 0)
                    hata = "Belirtilen kriterlerde firma/kişi Gelir İdaresi Bşk. da  bulunamamıştır.\nLütfen bilgilerinizi kontrol edip tekrar deneyiniz.";

                if (hata != "") return Results.Json(new { temizle = true, hata });
                return Results.Json(new { kayit, gib = new { ad = unvan, vd, adres } });
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = ex.Message });
            }
        }

        // Eski Ortak.RastgeleSayiUret
        private static string RastgeleSayiUret(int uzunluk)
        {
            string rSayi = "";
            Random rastgele = new Random();
            for (int i = 0; i < uzunluk; i++)
                rSayi += rastgele.Next(1, 10).ToString();
            return rSayi;
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, FirmaTanimIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            bool gelirler = GelirlerBaglantisiVar();
            bool turk = istek.uyruk != 1;
            string vno = Temiz(istek.vergiNo);
            string hata = "";

            if (vno == "" && !gelirler)
                vno = RastgeleSayiUret(10);

            if (Temiz(istek.eskiVergiNo) != "" && Temiz(istek.eskiVergiNo) != vno)
                hata = "Girilen bilgiler belirtilen Vergi Numarasına ait değildir.";

            if (gelirler)
            {
                if (Temiz(istek.gibAd) == "" && turk)
                    hata = "Firma/Kişi adı boş bırakılmış. Kayıt işlemi yapılamadı.";
            }
            else if (Temiz(istek.ad) == "")
                hata = "Firma/Kişi adı boş bırakılmış. Kayıt işlemi yapılamadı.";

            if (hata != "") return Results.Json(new { basarili = false, hata });

            FirmaBilgisi firma = new FirmaBilgisi();
            firma.kod = istek.kod;
            firma.muhasebe = OrtakFonksiyonlar.ConvertToInt(istek.muhasebe, 0);
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBE", Temiz(istek.muhasebe));
            if (gelirler && turk)
            {
                firma.ad = (istek.gibAd ?? "").Replace("'", " ").Replace(",", " ").Trim();
                firma.vergiDairesi = istek.gibVergiDairesi ?? "";
                firma.adres = (istek.gibAdres ?? "").Replace("'", " ").Trim();
            }
            else
            {
                firma.ad = (istek.ad ?? "").Replace(",", " ").Replace("'", " ").Trim();
                firma.vergiDairesi = Temiz(istek.vergiDairesi);
                firma.adres = (istek.adres ?? "").Replace("'", " ").Trim();
            }
            firma.adresNo = istek.adresNo ?? "";
            firma.vergiNo = vno.Replace(" ", "").Replace("-", "");
            if (firma.vergiDairesi != "SANALDEPO") // TCCB SanalDepo tanımlamasında vergi no alanına noktalı tanım yapılıyor
                firma.vergiNo = firma.vergiNo.Replace(".", "");

            firma.banka = Temiz(istek.banka);
            firma.hesapNo = Temiz(istek.hesapNo);
            firma.firmaYetkilisi = Temiz(istek.firmaYetkilisi);
            firma.eMail = Temiz(istek.eMail);
            firma.telefon = Temiz(istek.telefon);
            firma.faks = Temiz(istek.faks);
            firma.cepTel = (istek.cepTel ?? "").Replace("'", " ").Trim();
            firma.web = (istek.web ?? "").Replace("'", " ").Trim();
            firma.bicSwift = (istek.bicSwift ?? "").Replace("'", " ").Trim();
            firma.kisiUnvan = (istek.kisiUnvan ?? "").Replace("'", " ").Trim();
            firma.ekGosterge = OrtakFonksiyonlar.ConvertToInt(istek.ekGosterge, 0);
            firma.derece = OrtakFonksiyonlar.ConvertToInt(istek.derece, 0);
            firma.aciklama = istek.aciklama ?? "";
            firma.uyruk = turk ? 0 : 1;
            firma.gorulmesin = istek.gorulmesin ? 1 : 0;
            firma.yasakli = istek.yasakli ? 1 : 0;
            firma.karaListe = istek.karaListe ? 1 : 0;
            firma.fatura = istek.fatura ? 1 : 0;

            try
            {
                Sonuc sonuc = TNS.HRC.Arac.Tanimla().FirmaBilgisiKaydet(kullanan, firma);
                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = sonuc?.hataStr ?? "Kayıt yapılamadı." });
                // Eski kod: kayıttan sonra bu vergi numarasıyla yeniden listelenir (eski Listele(muhasebeFirma), txtVNo).
                return Results.Json(new { basarili = true, vergiNo = vno, bilgi = sonuc.bilgiStr ?? "" });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = ex.Message });
            }
        }

        private static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "HRC", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "HRC", sablonAd);
            return File.Exists(u) ? u : null;
        }

        // Eski btnYaz_Click (kod aynı).
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, string muhasebe, string ad, string vergiNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            string sablon = SablonBul(ortam.ContentRootPath, "Firmalar.xltx");
            if (sablon == null) return Results.Json(new { hata = "Rapor şablonu bulunamadı (HRC\\Firmalar.xltx)." });
            try
            {
                ObjectArray bilgi = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, Kriter(muhasebe, ad, vergiNo));
                if (bilgi == null || !bilgi.sonuc.islemSonuc)
                    return Results.Json(new { hata = bilgi?.sonuc?.hataStr ?? "Liste okunamadı." });

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("Firmalar", "") + ".xlsx";
                XLS.DosyaAc(sablon, sonucDosyaAd);

                int baslaSatir = 0;
                int baslaSutun = 0;
                int kaynakSatir = 0;
                int kaynakSutun = 0;
                XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
                XLS.HucreAdAdresCoz("FormatSatir", ref kaynakSatir, ref kaynakSutun);
                int satir = baslaSatir;
                int yuks = XLS.SatirYukseklikAl(baslaSatir);

                foreach (FirmaBilgisi item in bilgi.objeler)
                {
                    XLS.HucreKopyala(kaynakSatir, kaynakSutun, kaynakSatir, kaynakSutun + 20, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, baslaSutun + 0, item.vergiNo);
                    XLS.HucreDegerYaz(satir, baslaSutun + 1, item.ad);
                    XLS.HucreDegerYaz(satir, baslaSutun + 2, item.firmaYetkilisi);
                    XLS.HucreDegerYaz(satir, baslaSutun + 3, item.telefon);
                    XLS.HucreDegerYaz(satir, baslaSutun + 4, item.cepTel);
                    XLS.HucreDegerYaz(satir, baslaSutun + 5, item.faks);
                    XLS.HucreDegerYaz(satir, baslaSutun + 6, item.eMail);
                    XLS.HucreDegerYaz(satir, baslaSutun + 7, item.vergiDairesi);
                    XLS.HucreDegerYaz(satir, baslaSutun + 8, item.uyruk);
                    XLS.HucreDegerYaz(satir, baslaSutun + 9, item.yasakli);

                    XLS.HucreDegerYaz(satir, baslaSutun + 10, item.banka);
                    XLS.HucreDegerYaz(satir, baslaSutun + 11, item.hesapNo);
                    XLS.HucreDegerYaz(satir, baslaSutun + 12, item.adres);
                    satir++;
                }
                XLS.SatirYukseklikAyarla(baslaSatir, satir, -1, 7, yuks);
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), "Firmalar" + DosyaIslem.DosyaAdUretSade(), true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }
    }
}
