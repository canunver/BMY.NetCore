using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
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
    /// <summary>Kaydet isteğindeki bir personel satırı: d, değişken adına göre değerler (para değişkeninde sayı, diğerlerinde metin).</summary>
    public class MaasParamTopluSatiri
    {
        public string tcKimlikNo { get; set; }
        public string sicilNo { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public int unvan { get; set; }
        public string unvanAd { get; set; }
        public int kadroDerece { get; set; }
        public Dictionary<string, JsonElement> d { get; set; }
    }

    /// <summary>Kaydet isteği: kaydedilecek değişkenler (ekranda seçili kolonlar) ve satırlar.</summary>
    public class MaasParamTopluIstegi
    {
        public List<string> degiskenler { get; set; }
        public List<MaasParamTopluSatiri> satirlar { get; set; }
    }

    /// <summary>
    /// Personel Maaş Bilgileri Toplu Düzenleme (eski InsanKaynaklari/ListePersonelMaasParam.aspx; yeni sayfa
    /// InsanKaynaklari/ListePersonelMaasParam) uçları:
    ///   GET  /api/maasparamtoplu/tanim    açılış: kullanıcının son muhasebe / birimi, unvanlar, bordro türleri, maaş değişkenleri, işleçler
    ///   GET  /api/maasparamtoplu/liste    personeller ve maaş parametreleri, alan koşullarına göre süzülmüş (eski PersonelListele)
    ///   POST /api/maasparamtoplu/kaydet   seçili değişkenleri kişilere yazar (eski btnKaydet_Click)
    ///   POST /api/maasparamtoplu/dosya    Excel'den anahtar - değer çiftlerini okur (eski fuDosya_Selected); kaydetmez, ekrana yazılır
    /// İş kuralları eski sayfadaki gibi: IIKYServis.PersonelListele (maasParamlarOlsun), MaasParamKaydet, ListelePersonelTip, UnvanListele.
    /// </summary>
    public static class ListePersonelMaasParamUclari
    {
        private const string Uc = "/api/maasparamtoplu/";
        public const string SayfaAdresi = "../InsanKaynaklari/ListePersonelMaasParam.aspx";

        /// <summary>Eski islecler dizisi.</summary>
        private static readonly IntKodStrAd[] Islecler =
        {
            new IntKodStrAd(0, "-"), new IntKodStrAd(1, "Eşit"), new IntKodStrAd(2, "Farklı"), new IntKodStrAd(3, "Büyük ="), new IntKodStrAd(4, "Küçük =")
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapPost(Uc + "dosya", Dosya);
        }

        /// <summary>Menü yetkisi (eski GenelIslemler.SayfayaGirisYapabilirMi).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IIKYServis Servis() { return TNS.IKY.Arac.Tanimla(); }

        private static IResult Hata(string metin) { return Results.Json(new { basarili = false, hata = metin }); }

        /// <summary>Kişi bordro bilgisi değişkenleri (eski Page_Load'daki KisiBordroBilgi süzmesi), eski sırasıyla.</summary>
        private static List<TNS.HRC.BordroGenelDegisken> Degiskenler()
        {
            List<TNS.HRC.BordroGenelDegisken> l = new List<TNS.HRC.BordroGenelDegisken>();
            foreach (TNS.HRC.BordroGenelDegisken bgd in TNS.HRC.BordroGenelDegisken.bordroDegiskenler)
                if (bgd.degiskenTur == TNS.HRC.ENUMBordroDegiskenTur.KisiBordroBilgi) l.Add(bgd);
            return l;
        }

        private static TNS.HRC.BordroGenelDegisken DegiskenBul(List<TNS.HRC.BordroGenelDegisken> l, string ad)
        {
            if (string.IsNullOrWhiteSpace(ad)) return null;
            foreach (TNS.HRC.BordroGenelDegisken bgd in l)
                if (bgd.degiskenAd == ad) return bgd;
            return null;
        }

        // ------------------------------------------------------------------ açılış

        /// <summary>
        /// Eski Page_Load: muhasebe ve birim kullanıcı değişkenlerinden (MUHASEBE, MUHASEBEBIRIM), Genel.UnvanDoldur,
        /// ListelePersonelTip (bordro türleri), kişi bordro bilgisi değişkenleri (kolonlar) ve işleçler.
        /// </summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                string hata = "";
                IIKYServis servis = Servis();

                string muhStr = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBE") ?? "";
                int muhasebe = OrtakFonksiyonlar.ConvertToInt(muhStr.Split('-')[0].Trim(), 0);
                string birim = (GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEBIRIM") ?? "").Trim();
                string birimAd = "";
                if (muhasebe > 0)
                {
                    try { birimAd = MuhasebeBirimUclari.BirimAdBul(muhasebe, birim, out string kurum); }
                    catch (Exception) { birimAd = ""; }
                }

                List<object> unvanlar = new List<object>();
                unvanlar.Add(new { kod = 0, ad = "" });
                ObjectArray oa = servis.UnvanListele(new Unvan());
                if (oa.sonuc.islemSonuc)
                {
                    foreach (Unvan u in oa.objeler) unvanlar.Add(new { kod = u.UnvanKod, ad = u.UnvanAd });
                }
                else hata = PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Unvanlar okunamadı.");

                List<object> bordroTurler = new List<object>();
                ObjectArray pt = servis.ListelePersonelTip(kullanan, new PersonelTipi());
                if (pt.sonuc.islemSonuc)
                {
                    foreach (object o in pt.objeler)
                        if (o is PersonelTipi t) bordroTurler.Add(new { kod = t.kod, ad = t.ad });
                }
                else if (hata == "") hata = PersonelBilgiOrtak.HataMetni(pt.sonuc.hataStr, "Bordro türleri okunamadı.");

                List<object> degiskenler = new List<object>();
                foreach (TNS.HRC.BordroGenelDegisken bgd in Degiskenler())
                    degiskenler.Add(new { bgd.degiskenAd, bgd.aciklama, para = bgd.veriTipi == VariableType.Money });

                List<object> islecler = new List<object>();
                foreach (IntKodStrAd i in Islecler) islecler.Add(new { i.kod, i.ad });

                return Results.Json(new
                {
                    basarili = true,
                    muhasebe,
                    birim,
                    birimAd,
                    unvanlar,
                    bordroTurler,
                    degiskenler,
                    islecler,
                    hata
                });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ liste

        /// <summary>
        /// Eski PersonelListele / PersonelListeleIC / PersonelleriGoster: aktif personel, bugünkü tarihe göre, seçilen bordro
        /// türleri ("," ile ayrılmış kodlar) zorunlu. Alan koşulları (alan1/islec1/deger1, alan2/islec2/deger2) eski GosterilsinMi
        /// ile uygulanır. Listelemeden sonra muhasebe ve birim kullanıcı değişkenlerine saklanır.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string birim, string ara, int? unvan,
            string turler, string alan1, int? islec1, double? deger1, string alan2, int? islec2, double? deger2)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TNS.IKY.Personel p = new TNS.IKY.Personel();
                p.kadro.muhasebeKodu = muhasebe ?? 0;
                p.kadro.muhasebeBirimi = (birim ?? "").Trim();
                p.genelSorgu = ara ?? "";
                p.kadro.unvan = unvan ?? 0;
                p.aktifTarih = new TNSDateTime(DateTime.Now);
                p.personelTipleri = (turler ?? "").Trim();
                p.aktifDurum = 1;
                p.maasParamlarOlsun = true;

                if (string.IsNullOrWhiteSpace(p.personelTipleri))
                    return Hata("Listelemeden önce lütfen personel tipini seçiniz");

                ObjectArray liste = Servis().PersonelListele(kullanan, p, false, false);
                if (!liste.sonuc.islemSonuc)
                    return Hata(PersonelBilgiOrtak.HataMetni(liste.sonuc.hataStr, "Personel listesi okunamadı."));

                double d1 = deger1 ?? 0;
                double d2 = deger2 ?? 0;
                string i1 = (islec1 ?? 0).ToString();
                string i2 = (islec2 ?? 0).ToString();

                List<object> satirlar = new List<object>();
                foreach (object o in liste.objeler)
                {
                    if (!(o is TNS.IKY.Personel per)) continue;
                    TNS.IKY.PersonelMaasParam mp = per.personelMaasParam;
                    if (!GosterilsinMi(mp, alan1, i1, d1) || !GosterilsinMi(mp, alan2, i2, d2)) continue;

                    Dictionary<string, object> d = new Dictionary<string, object>();
                    if (mp != null && mp.paramlar != null)
                    {
                        foreach (object x in mp.paramlar)
                        {
                            if (!(x is MaasParam m) || string.IsNullOrEmpty(m.degiskenAd)) continue;
                            d[m.degiskenAd] = m.tip == VariableType.Money ? (object)m.degerDbl : (m.degerStr ?? "");
                        }
                    }
                    satirlar.Add(new
                    {
                        tcKimlikNo = per.tcKimlikNo ?? "",
                        ad = per.ad ?? "",
                        soyad = per.soyad ?? "",
                        unvan = per.kadro.unvan,
                        unvanAd = per.kadro.unvanAd ?? "",
                        kadroDerece = per.kadro.kadroDerece,
                        sicilNo = per.sicilNo ?? "",
                        d
                    });
                }

                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBE", (muhasebe ?? 0) > 0 ? muhasebe.ToString() : "");
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBEBIRIM", (birim ?? "").Trim());

                return Results.Json(new { basarili = true, satirlar });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Personel listesi okunamadı."));
            }
        }

        /// <summary>Eski GosterilsinMi (aynen).</summary>
        private static bool GosterilsinMi(TNS.IKY.PersonelMaasParam mp, string alan, string islec, double deger)
        {
            if (string.IsNullOrWhiteSpace(alan)) return true;
            if (mp == null) return false;

            double veri = mp.ParamDegerBul(alan);

            if (islec == "0") return true; //-
            else if (islec == "1") return veri == deger;  //Eşit
            else if (islec == "2") return veri != deger;  //Farklı
            else if (islec == "3") return veri >= deger;  //Büyük =
            else if (islec == "4") return veri <= deger;  //Küçük =

            return true;
        }

        // ------------------------------------------------------------------ kaydet

        private static double SayiAl(JsonElement e)
        {
            if (e.ValueKind == JsonValueKind.Number) return e.GetDouble();
            if (e.ValueKind == JsonValueKind.String) return OrtakFonksiyonlar.ConvertToDbl(e.GetString() ?? "");
            return 0;
        }

        private static string MetinAl(JsonElement e)
        {
            if (e.ValueKind == JsonValueKind.String) return e.GetString() ?? "";
            if (e.ValueKind == JsonValueKind.Number) return e.GetRawText();
            return "";
        }

        /// <summary>
        /// Eski btnKaydet_Click + PersonelListeOl: her satır için seçili değişkenlerden MaasParam listesi kurulur, IIKYServis.MaasParamKaydet
        /// çağrılır (iş kuralı her kişide seçili değişkenleri silip yeniden yazar). Sayfa değişen satırları gönderir; Excel'den
        /// yüklemeden sonra bütün satırları.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, MaasParamTopluIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.satirlar == null || istek.degiskenler == null) return Results.BadRequest();
            try
            {
                List<TNS.HRC.BordroGenelDegisken> tum = Degiskenler();
                List<TNS.HRC.BordroGenelDegisken> secili = new List<TNS.HRC.BordroGenelDegisken>();
                TNSCollection degiskenAdlar = new TNSCollection();
                foreach (string ad in istek.degiskenler)
                {
                    TNS.HRC.BordroGenelDegisken bgd = DegiskenBul(tum, ad);
                    if (bgd == null || secili.Contains(bgd)) continue;
                    secili.Add(bgd);
                    degiskenAdlar.Add(bgd.degiskenAd);
                }
                if (secili.Count == 0) return Hata("Kaydedilecek kolon seçilmedi.");

                TNSCollection kisiler = new TNSCollection();
                foreach (MaasParamTopluSatiri row in istek.satirlar)
                {
                    if (row == null || string.IsNullOrWhiteSpace(row.tcKimlikNo)) continue;
                    TNS.IKY.Personel per = new TNS.IKY.Personel();
                    per.tcKimlikNo = row.tcKimlikNo.Trim();
                    per.sicilNo = row.sicilNo;
                    per.ad = row.ad;
                    per.soyad = row.soyad;
                    per.kadro.unvan = row.unvan;
                    per.kadro.unvanAd = row.unvanAd;
                    per.kadro.kadroDerece = row.kadroDerece;
                    per.personelMaasParam = new TNS.IKY.PersonelMaasParam();
                    per.personelMaasParam.paramlar = new TNSCollection();

                    foreach (TNS.HRC.BordroGenelDegisken bgd in secili)
                    {
                        MaasParam mpIc = new MaasParam();
                        mpIc.degiskenAd = bgd.degiskenAd;
                        mpIc.tip = bgd.veriTipi;
                        JsonElement e = default;
                        bool varMi = row.d != null && row.d.TryGetValue(bgd.degiskenAd, out e);
                        if (bgd.veriTipi == VariableType.Money)
                            mpIc.degerDbl = varMi ? SayiAl(e) : 0;
                        else
                            mpIc.degerStr = varMi ? MetinAl(e) : "";
                        per.personelMaasParam.paramlar.Add(mpIc);
                    }
                    kisiler.Add(per);
                }

                Sonuc s = Servis().MaasParamKaydet(kullanan, kisiler, degiskenAdlar);
                if (!s.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(s.hataStr, "Kayıt yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt başarılı" });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        // ------------------------------------------------------------------ dosya yükle

        /// <summary>
        /// Eski fuDosya_Selected: başlık satırı olmayan Excel, 1. sütun anahtar (1: Kimlik No, 2: Sicil No), 2. sütun veri alanının değeri.
        /// İlk boş anahtara kadar okunur. Değerler kaydedilmez; sayfa eşleşen satırlara yazar (kaydetmek için Kaydet).
        /// </summary>
        private static async Task<IResult> Dosya(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IFormCollection form = await baglam.Request.ReadFormAsync();
                string veriAlan = ((string)form["veriAlan"] ?? "").Trim();
                if (string.IsNullOrWhiteSpace(veriAlan)) return Hata("Lütfen Veri Alanını Seçiniz");
                TNS.HRC.BordroGenelDegisken df = DegiskenBul(Degiskenler(), veriAlan);
                if (df == null) return Hata("Veri alanı bulunamadı!");

                IFormFile dosya = form.Files["dosya"];
                if (dosya == null || dosya.Length == 0) return Hata("Lütfen Dosya Seçiniz");
                if (dosya.Length > 20 * 1024 * 1024) return Hata("Dosya çok büyük (en çok 20 MB).");
                int anahAlan = OrtakFonksiyonlar.ConvertToInt((string)form["anahtar"], 0);
                if (anahAlan < 1 || anahAlan > 2) return Hata("Lütfen Anahtar Alanı Seçiniz");

                AsposeLisans.Yukle();
                MemoryStream akis = new MemoryStream();
                await dosya.CopyToAsync(akis);
                akis.Position = 0;
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(akis);

                bool para = df.veriTipi == VariableType.Money;
                List<object> satirlar = new List<object>();
                int sat = 0;
                while (true)
                {
                    string anah = XLS.HucreDegerAl(sat, 0);
                    if (string.IsNullOrWhiteSpace(anah)) break;
                    object deger = para ? (object)XLS.HucreDegerAlDbl(sat, 1) : (XLS.HucreDegerAl(sat, 1) ?? "");
                    satirlar.Add(new { anahtar = anah.Trim(), deger });
                    sat++;
                }
                return Results.Json(new { basarili = true, veriAlan = df.degiskenAd, para, anahtar = anahAlan, satirlar });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Dosya okunamadı."));
            }
        }
    }
}
