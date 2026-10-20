using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    public class SayistayKaydetIstegi
    {
        public string Tur { get; set; }
        public int Yil { get; set; }
        public int Ay { get; set; }
        public int Muhasebe { get; set; }
        /// <summary>Tablodaki satırlar; alan adları eski BilgiStore alanlarıdır (BANKAADI, TARIH, BORCTOPLAM ...).</summary>
        public List<Dictionary<string, JsonElement>> Satirlar { get; set; }
    }

    /// <summary>
    /// Sayıştay Verileri (eski ButceMuhasebe/SayistayVerileri.aspx) uçları. Solda Sayıştay'a gönderilecek tabloların listesi,
    /// sağda seçilen tablonun kuralları. Elle girilen tablolarda (2.2 - 2.6: banka hesapları, kasa / banka / çek / menkul kıymet
    /// sayım tutanakları) bilgiler tabloya yazılıp MUH iş kuralıyla (SayistayVeriListele / SayistayVeriKaydet) saklanır.
    /// "Dosya Hazırla" tablonun dosyasını üretir (Muhasebe/SayistayVerileriDosyalari.cs, eski kod aynen).
    ///   GET  /api/sayistay/tanim  : tablo listesi, her tabloda görünen alanlar ve sütunlar, yıllar, varsayılan muhasebe (eski Page_Load)
    ///   GET  /api/sayistay/liste  : elle girilen tablonun kayıtlı satırları (eski GridIcerikGetir)
    ///   POST /api/sayistay/kaydet : tablo satırlarını saklar (eski GridIcerikKaydet)
    ///   GET  /api/sayistay/dosya  : dosyayı üretir ve indirir (eski btnListele_Click)
    /// Yetki: menüdeki seçenek (eski SayfayaGirisYapabilirMi).
    /// </summary>
    public static class SayistayVerileriUclari
    {
        public const string SayfaAdres = "../ButceMuhasebe/SayistayVerileri.aspx";
        private const string Uc = "/api/sayistay/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapGet(Uc + "dosya", Dosya);
        }

        // ------------------------------------------------------------------ tablo tanımları (eski tumRaporlar, KontrolleriGosterGizle, GridBaslikAyarla)

        private class RaporBilgi
        {
            public string RAPORADI;
            public string RAPORKODU;
            public RaporBilgi(string RAPORADI, string RAPORKODU) { this.RAPORADI = RAPORADI; this.RAPORKODU = RAPORKODU; }
        }

        private static readonly RaporBilgi[] tumRaporlar =
        {
            new RaporBilgi("Muhasebe Birimi Bilgi Formu", "2.1"),
            new RaporBilgi("Banka Hesapları Bilgi Formu", "2.2"),
            new RaporBilgi("Kasa Sayım Tutanağı", "2.3"),
            new RaporBilgi("Banka Mevcudu Tespit Tutanağı", "2.4"),
            new RaporBilgi("Alınan Çekler Sayım Tutanağı", "2.5"),
            new RaporBilgi("Menkul Kıymet ve Varlıklar/Mektupları Sayım Tutanağı", "2.6"),
            new RaporBilgi("Taşınır Kesin Hesap Cetveli (5.7.A.1)", "2.7"),
            new RaporBilgi("Taşınır Kesin Hesap İcmal Cetveli (5.7.A.2)", "2.8"),
            new RaporBilgi("Bütçe Giderleri ve Ödenekler Tablosu", "2.9"),
            new RaporBilgi("Bütçe Gelirlerinin Ekonomik Sınıflandırılması Tablosu", "2.10"),
            new RaporBilgi("Bilanço", "2.11"),
            new RaporBilgi("Faaliyet Sonuçları Tablosu", "2.12"),
            new RaporBilgi("Birleştirilmiş Veriler Defteri", "2.13"),
            new RaporBilgi("Gider Bütçesi", "2.14"),
            new RaporBilgi("Gelir Bütçesi", "2.15"),
            new RaporBilgi("Mizan", "2.16"),
            new RaporBilgi("Öz Kaynak Değişim", "2.23"),
            new RaporBilgi("Bütçeleşen ve Gerçekleşen Tutarlar", "2.24")
        };

        /// <summary>Eski KontrolleriGosterGizle: Yıl ve Muhasebe her tabloda görünür; Ay, Mizan Türü ve giriş tablosu yalnızca bazılarında.</summary>
        private static bool AyVar(string tur) { return tur == "2.2" || tur == "2.3" || tur == "2.4" || tur == "2.5" || tur == "2.6" || tur == "2.13" || tur == "2.16"; }
        private static bool GirisVar(string tur) { return tur == "2.2" || tur == "2.3" || tur == "2.4" || tur == "2.5" || tur == "2.6"; }

        private static object Kolon(string baslik, string dataIndex, string tur)
        {
            return new { baslik = baslik, dataIndex = dataIndex, tur = tur };
        }

        /// <summary>Eski GridBaslikAyarla: giriş tablosunun sütunları (başlıkta * olanlar zorunlu; tur STR / TAR / DBL).</summary>
        private static List<object> Kolonlar(string raporTur)
        {
            List<object> kolonListe = new List<object>();
            if (raporTur == "2.2")
            {
                kolonListe.Add(Kolon("*Banka Adı", "BANKAADI", "STR"));
                kolonListe.Add(Kolon("*Şube Adı", "SUBEADI", "STR"));
                kolonListe.Add(Kolon("*Hesap No", "HESAPNO", "STR"));
            }
            else if (raporTur == "2.3")
            {
                kolonListe.Add(Kolon("*Tarih", "TARIH", "TAR"));
                kolonListe.Add(Kolon("*Borç Toplamı", "BORCTOPLAM", "DBL"));
                kolonListe.Add(Kolon("*Alacak Toplamı", "ALACAKTOPLAM", "DBL"));
                kolonListe.Add(Kolon("*Kasada Olması Gereken Tutar", "OLMASIGEREKENTUTAR", "DBL"));
                kolonListe.Add(Kolon("*Kasada Olan Tutar", "OLANTUTAR", "DBL"));
                kolonListe.Add(Kolon("Sayım Noksanı", "NOKSANTUTAR", "DBL"));
                kolonListe.Add(Kolon("Sayım Fazlası", "FAZLATUTAR", "DBL"));
                kolonListe.Add(Kolon("*Başkan", "BASKAN", "STR"));
                kolonListe.Add(Kolon("*Üye 1", "UYE1", "STR"));
                kolonListe.Add(Kolon("*Üye 2", "UYE2", "STR"));
            }
            else if (raporTur == "2.4")
            {
                kolonListe.Add(Kolon("*Tarih", "TARIH", "TAR"));
                kolonListe.Add(Kolon("*Borç Toplamı", "BORCTOPLAM", "DBL"));
                kolonListe.Add(Kolon("*Alacak Toplamı", "ALACAKTOPLAM", "DBL"));
                kolonListe.Add(Kolon("*Bankada Olması Gereken Tutar", "OLMASIGEREKENTUTAR", "DBL"));
                kolonListe.Add(Kolon("Borç Artığı", "BORCARTIGITUTAR", "DBL"));
                kolonListe.Add(Kolon("Alacak Artığı", "ALACAKARTIGITUTAR", "DBL"));
                kolonListe.Add(Kolon("Sayım Noksanı", "NOKSANTUTAR", "DBL"));
                kolonListe.Add(Kolon("Sayım Fazlası", "FAZLATUTAR", "DBL"));
                kolonListe.Add(Kolon("*Başkan", "BASKAN", "STR"));
                kolonListe.Add(Kolon("*Üye 1", "UYE1", "STR"));
                kolonListe.Add(Kolon("*Üye 2", "UYE2", "STR"));
            }
            else if (raporTur == "2.5")
            {
                kolonListe.Add(Kolon("*Tarih", "TARIH", "TAR"));
                kolonListe.Add(Kolon("*Borç Toplamı", "BORCTOPLAM", "DBL"));
                kolonListe.Add(Kolon("*Alacak Toplamı", "ALACAKTOPLAM", "DBL"));
                kolonListe.Add(Kolon("Olması Gereken Tutar", "OLMASIGEREKENTUTAR", "DBL"));
                kolonListe.Add(Kolon("Olan Tutar", "OLANTUTAR", "DBL"));
                kolonListe.Add(Kolon("Sayım Noksanı", "NOKSANTUTAR", "DBL"));
                kolonListe.Add(Kolon("Sayım Fazlası", "FAZLATUTAR", "DBL"));
                kolonListe.Add(Kolon("*Başkan", "BASKAN", "STR"));
                kolonListe.Add(Kolon("*Üye 1", "UYE1", "STR"));
                kolonListe.Add(Kolon("*Üye 2", "UYE2", "STR"));
            }
            else if (raporTur == "2.6")
            {
                kolonListe.Add(Kolon("*Tarih", "TARIH", "TAR"));
                kolonListe.Add(Kolon("*Değer Kime Ait", "DEGERKIMEAIT", "STR"));
                kolonListe.Add(Kolon("*Değerin Cinsi", "DEGERCINSI", "STR"));
                kolonListe.Add(Kolon("*Adet", "DEGERADET", "DBL"));
                kolonListe.Add(Kolon("*Tutar", "DEGERTUTAR", "DBL"));
                kolonListe.Add(Kolon("*Miktar Adet", "MIKTARADET", "DBL"));
                kolonListe.Add(Kolon("*Miktar Tutar", "MIKTARTUTAR", "DBL"));
                kolonListe.Add(Kolon("Noksan Miktar Adet", "NOKSANMIKTARADET", "DBL"));
                kolonListe.Add(Kolon("Noksan Miktar Tutar", "NOKSANMIKTARTUTAR", "DBL"));
                kolonListe.Add(Kolon("Fazla Miktar Adet", "FAZLAMIKTARADET", "DBL"));
                kolonListe.Add(Kolon("Fazla Miktar Tutar", "FAZLAMIKTARTUTAR", "DBL"));
                kolonListe.Add(Kolon("*Başkan", "BASKAN", "STR"));
                kolonListe.Add(Kolon("*Üye 1", "UYE1", "STR"));
                kolonListe.Add(Kolon("*Üye 2", "UYE2", "STR"));
            }
            return kolonListe;
        }

        /// <summary>Eski BilgiStore alanları: sayılar ve metinler (TARIH ayrıca).</summary>
        private static readonly string[] SayiAlanlar =
        {
            "ALACAKARTIGITUTAR", "ALACAKTOPLAM", "BORCARTIGITUTAR", "BORCTOPLAM", "DEGERADET", "DEGERTUTAR", "FAZLAMIKTARADET",
            "FAZLAMIKTARTUTAR", "FAZLATUTAR", "MIKTARADET", "MIKTARTUTAR", "NOKSANMIKTARADET", "NOKSANMIKTARTUTAR", "NOKSANTUTAR",
            "OLANTUTAR", "OLMASIGEREKENTUTAR"
        };
        private static readonly string[] MetinAlanlar = { "BANKAADI", "BASKAN", "DEGERCINSI", "DEGERKIMEAIT", "HESAPNO", "SUBEADI", "UYE1", "UYE2" };

        // ------------------------------------------------------------------ yetki ve ortak

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            return hata == "" ? varsayilan : hata;
        }

        private static RaporBilgi RaporBul(string raporTur)
        {
            foreach (RaporBilgi rb in tumRaporlar)
                if (rb.RAPORKODU == raporTur) return rb;
            return null;
        }

        private static SayistayVeri Veri(string tur, int yil, int ay, int muhasebe)
        {
            SayistayVeri veri = new SayistayVeri();
            veri.yil = yil;
            veri.ay = ay;
            veri.muhasebe = muhasebe;
            veri.tur = tur ?? "";
            return veri;
        }

        // ------------------------------------------------------------------ açılış (eski Page_Load)

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                List<object> raporlar = new List<object>();
                foreach (RaporBilgi rb in tumRaporlar)
                {
                    raporlar.Add(new
                    {
                        RAPORKODU = rb.RAPORKODU,
                        RAPORADI = rb.RAPORADI,
                        ay = AyVar(rb.RAPORKODU),
                        mizanTur = rb.RAPORKODU == "2.16",
                        giris = GirisVar(rb.RAPORKODU),
                        kolonlar = Kolonlar(rb.RAPORKODU)
                    });
                }
                return Results.Json(new
                {
                    raporlar = raporlar,
                    yilIlk = 2005,
                    yilSon = DateTime.Now.Year + 1,
                    muhasebe = (TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIMUHASEBEKOD") + "").Trim()
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ giriş tablosu (eski GridIcerikGetir / GridIcerikKaydet)

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string tur, int? yil, int? ay, int? muhasebe)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!GirisVar(tur)) return Hata("Bu tabloda giriş yapılmaz.");
            try
            {
                SayistayVeri veri = Veri(tur, yil ?? 0, ay ?? 0, muhasebe ?? 0);
                ObjectArray veriListe = TNS.MUH.Arac.Tanimla().SayistayVeriListele(kullanan, veri);
                string veriler = "";
                foreach (SayistayVeri sv in veriListe.objeler)
                    veriler += sv.veriler;
                veriler = "<VeriListe>" + veriler + "</VeriListe>";

                XmlDocument newXml = new XmlDocument();
                newXml.LoadXml(veriler);
                XmlNodeList nodeList = newXml.DocumentElement.SelectNodes("/VeriListe/veri");

                List<Dictionary<string, object>> liste = new List<Dictionary<string, object>>();
                foreach (XmlNode xn in nodeList)
                {
                    Dictionary<string, object> satir = new Dictionary<string, object>();
                    foreach (string a in SayiAlanlar)
                        satir[a] = OrtakFonksiyonlar.ConvertToDbl(TNS.MUH.Arac.OzellikVer(xn, a));
                    foreach (string a in MetinAlanlar)
                        satir[a] = TNS.MUH.Arac.OzellikVer(xn, a);
                    DateTime t = new TNSDateTime(TNS.MUH.Arac.OzellikVer(xn, "TARIH")).Oku();
                    satir["TARIH"] = t == DateTime.MinValue ? "" : t.ToString("yyyy-MM-dd");
                    liste.Add(satir);
                }
                // Eski kod listenin sonuna 5 boş satır eklerdi; boş satırları tablo (js) ekler.
                return Results.Json(new { satirlar = liste });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Bilgiler okunamadı."));
            }
        }

        private static string Metin(Dictionary<string, JsonElement> jc, string ad)
        {
            if (!jc.TryGetValue(ad, out JsonElement e)) return "";
            if (e.ValueKind == JsonValueKind.String) return e.GetString() ?? "";
            if (e.ValueKind == JsonValueKind.Number) return e.GetRawText();
            return "";
        }

        /// <summary>Eski ButceMuhasebeGenel.DegerAlDbl: okunamazsa 0.</summary>
        private static double Sayi(Dictionary<string, JsonElement> jc, string ad)
        {
            if (!jc.TryGetValue(ad, out JsonElement e)) return 0;
            if (e.ValueKind == JsonValueKind.Number && e.TryGetDouble(out double d)) return d;
            if (e.ValueKind == JsonValueKind.String)
            {
                string s = (e.GetString() ?? "").Trim();
                if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d)) return d;
                return OrtakFonksiyonlar.ConvertToDbl(s);
            }
            return 0;
        }

        /// <summary>Eski ButceMuhasebeGenel.DegerAlDateTime: yyyy-MM-dd; okunamazsa null (XML'e boş yazılır).</summary>
        private static TNSDateTime Tarih(Dictionary<string, JsonElement> jc, string ad)
        {
            string s = Metin(jc, ad).Trim();
            if (s.Length >= 10 && DateTime.TryParseExact(s.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return null;
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, SayistayKaydetIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || !GirisVar(istek.Tur)) return Hata("Bu tabloda giriş yapılmaz.");

            SayistayVeri veri = Veri(istek.Tur, istek.Yil, istek.Ay, istek.Muhasebe);

            // Eski GridIcerikKaydet aynen; tek fark: metinler XML'e kaçışlı yazılır (eski kodda " ya da & içeren ad kaydı bozuyordu).
            string icVeri = "";
            foreach (Dictionary<string, JsonElement> jc in istek.Satirlar ?? new List<Dictionary<string, JsonElement>>())
            {
                if (jc == null) continue;
                string hesapNo = Metin(jc, "HESAPNO");
                string tarih = Metin(jc, "TARIH");
                if (veri.tur == "2.2" && hesapNo.Trim() == "") continue;
                if (veri.tur == "2.3" && tarih.Trim() == "") continue;
                if (veri.tur == "2.4" && tarih.Trim() == "") continue;
                if (veri.tur == "2.5" && tarih.Trim() == "") continue;
                if (veri.tur == "2.6" && tarih.Trim() == "") continue;

                icVeri += "<veri ";
                foreach (string a in SayiAlanlar)
                    icVeri += a + "=\"" + Sayi(jc, a) + "\" ";
                foreach (string a in MetinAlanlar)
                    icVeri += a + "=\"" + SecurityElement.Escape(Metin(jc, a)) + "\" ";
                icVeri += "TARIH=\"" + Tarih(jc, "TARIH") + "\" ";
                icVeri += "/>";
            }

            try
            {
                veri.veriler = icVeri;
                Sonuc sonuc = TNS.MUH.Arac.Tanimla().SayistayVeriKaydet(kullanan, veri);
                if (sonuc.islemSonuc)
                    return Results.Json(new { basarili = true, bilgi = "Bilgiler başarıyla kayıt edildi." });
                return Hata(HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        // ------------------------------------------------------------------ Dosya Hazırla (eski btnListele_Click)

        private static IResult Dosya(HttpContext baglam, IWebHostEnvironment ortam, string tur, int? yil, int? ay, int? muhasebe, int? mizanTur)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (RaporBul(tur) == null) return Hata("Tabloyu seçiniz.");
            if ((yil ?? 0) <= 0) return Hata("Yılı giriniz.");
            try
            {
                AsposeLisans.Yukle();
                SayistayVerileriDosyalari d = new SayistayVerileriDosyalari(kullanan, tur, yil ?? 0, ay ?? 0, muhasebe ?? 0, mizanTur ?? 2);
                d.Uret();
                return d.Cevap ?? Hata("Dosya hazırlanamadı.");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Dosya hazırlanamadı."));
            }
        }
    }
}
