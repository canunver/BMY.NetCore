using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.BHMv2;
using TNS.KYM;

namespace Istemci.ButceForm
{
    public class HizmetMaliyetiTasraIstegi
    {
        public int Yil { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        public string Kurumsal { get; set; }
        public List<HizmetMaliyetiSatirIstegi> Satirlar { get; set; }
    }

    public class HizmetMaliyetiTasraOnayIstegi
    {
        public int Yil { get; set; }
        public bool Onayla { get; set; }
    }

    /// <summary>
    /// Birimlerin Hizmet Maliyetinin Tespitine İlişkin Bilgi Formu, taşra (eski ButceForm/HizmetMaliyetiTespitiTasra.aspx)
    /// sayfasının JavaScript'ten çağırdığı uçlar (muhasebe / birim seçimi ortak bileşenle: /api/muhbirim/...):
    ///   GET  /api/hizmetmaliyetitasra/tanim
    ///   GET  /api/hizmetmaliyetitasra/liste?yil=&amp;muhasebe=&amp;birim=&amp;kurumsal=   (eski btnListele_Click -> HizmetMaliyetiTasraListele)
    ///   POST /api/hizmetmaliyetitasra/kaydet                                          (eski btnKaydet_Click -> HizmetMaliyetiTasraKaydet)
    ///   GET  /api/hizmetmaliyetitasra/yazdir?yil=&amp;muhasebe=&amp;birim=&amp;kurumsal=&amp;toplu=   (eski btnYazdir_Click / btnYazdirToplu_Click)
    ///   Yalnızca sistem yöneticisi (eski sayfada bu tuşlar yalnızca ona görünürdü):
    ///   GET  /api/hizmetmaliyetitasra/girmeyenler?yil=   (eski btnGirmeyenler_Click)
    ///   POST /api/hizmetmaliyetitasra/onay               (eski btnOnayla_Click / btnOnayKaldir_Click; formu veri girişine kapatır / açar)
    /// </summary>
    public static class HizmetMaliyetiTasraUclari
    {
        private const string SayfaAdresi = "../ButceForm/HizmetMaliyetiTespitiTasra.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/hizmetmaliyetitasra/tanim", Tanim);
            app.MapGet("/api/hizmetmaliyetitasra/liste", Liste);
            app.MapPost("/api/hizmetmaliyetitasra/kaydet", Kaydet);
            app.MapGet("/api/hizmetmaliyetitasra/yazdir", Yazdir);
            app.MapGet("/api/hizmetmaliyetitasra/girmeyenler", Girmeyenler);
            app.MapPost("/api/hizmetmaliyetitasra/onay", Onay);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdresi);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static bool SistemYoneticisi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private const string SecimHatasi = "Yıl, Kurum veya Birim alanlarından biri veya bir kaçı boş bırakılmış. Lütfen ilgili alanları kontrol edip tekrar deneyin.";

        // ------------------------------------------------------------------ okuma

        /// <summary>
        /// Açılış (eski Page_Load): bütçe hazırlık yılı, kullanıcının muhasebe birimi (kullanıcı değişkenleri), birimin kurumsal
        /// kodu ve yönetici tuşları. Eski ddlKurumsal yok: Yazdır (Toplu) da seçili muhasebe biriminin kurumsal kodunu kullanır.
        /// </summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int yil = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/BUTCEHAZIRLIK"), DateTime.Now.Year + 1);
                string muhasebe = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBE");
                string birim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEBIRIM");
                string kurum;
                string birimAd = MuhasebeBirimUclari.BirimAdBul(OrtakFonksiyonlar.ConvertToInt(muhasebe, 0), birim, out kurum);
                bool yonetici = SistemYoneticisi(kullanan);
                return Results.Json(new
                {
                    yil = yil, muhasebe = muhasebe, birim = birim, birimAd = birimAd,
                    kurumsal = MuhasebeBirimUclari.KurumsalYaz(yil, kurum),
                    sistemYoneticisi = yonetici
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Listele (eski HizmetMaliyetiTasraListele): kodlar ve birimin girdiği tutarlar; "6" ile başlayan alt satırlar diğer giderlerdir.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, int? muhasebe, string birim, string kurumsal)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0 || (muhasebe ?? 0) <= 0 || Temiz(birim) == "") return Hata(SecimHatasi);
            try
            {
                ObjectArray hmv = TNS.BHF.Arac.Tanimla().HizmetMaliyetiTasraListele(kullanan, yil, muhasebe.Value, Temiz(birim), Temiz(kurumsal), false);
                if (hmv == null || !hmv.sonuc.islemSonuc) return Hata(EkFormUclari.HataMetni(hmv?.sonuc.hataStr, "Hizmet maliyeti bilgileri okunamadı."));
                List<object> satirlar = new List<object>();
                if (hmv.objeler != null)
                {
                    foreach (object o in hmv.objeler)
                    {
                        if (!(o is HizmetMaliyetiTesbitiSatir hms)) continue;
                        satirlar.Add(new
                        {
                            kod = hms.kod ?? "", aciklama = hms.aciklama ?? "", degerGirilsinMi = hms.degerGirilsinMi,
                            gercek = hms.gercek, haziran = hms.haziran, tahmin = hms.tahmin, tahmin1 = hms.tahmin1, tahmin2 = hms.tahmin2
                        });
                    }
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(EkFormUclari.HataMetni(ex.Message, "Hizmet maliyeti bilgileri okunamadı."));
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Kaydet (eski btnKaydet_Click): değer girilen, açıklaması boş olmayan ve tutarlarının toplamı sıfırdan büyük satırlar gönderilir;
        /// muhasebe / birim kullanıcı değişkeni olarak saklanır (eski DegiskenleriKaydet).
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, HizmetMaliyetiTasraIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new HizmetMaliyetiTasraIstegi();
            string hata = "";
            if (istek.Muhasebe <= 0) hata += " Muhasebe bilgisi boş bırakılamaz.";
            if (Temiz(istek.Birim) == "") hata += " Birim bilgisi boş bırakılamaz.";
            if (istek.Yil <= 0) hata += " Yıl alanı boş bırakılamaz.";
            if (hata != "") return Results.Json(new { basarili = false, hata = hata.Trim() });

            try
            {
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBE", istek.Muhasebe.ToString());
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "BIRIM", Temiz(istek.Birim));

                TNSCollection hmc = new TNSCollection();
                foreach (HizmetMaliyetiSatirIstegi s in istek.Satirlar ?? new List<HizmetMaliyetiSatirIstegi>())
                {
                    if (s == null || !s.DegerGirilsinMi) continue;
                    HizmetMaliyetiTesbitiSatir hms = new HizmetMaliyetiTesbitiSatir();
                    hms.kod = Temiz(s.Kod);
                    hms.aciklama = Temiz(s.Aciklama);
                    hms.gercek = Math.Round(s.Gercek ?? 0);
                    hms.haziran = Math.Round(s.Haziran ?? 0);
                    hms.tahmin = Math.Round(s.Tahmin ?? 0);
                    hms.tahmin1 = Math.Round(s.Tahmin1 ?? 0);
                    hms.tahmin2 = Math.Round(s.Tahmin2 ?? 0);
                    hms.degerGirilsinMi = true;
                    if (hms.aciklama != "" && (hms.gercek + hms.haziran + hms.tahmin + hms.tahmin1 + hms.tahmin2 > 0))
                        hmc.Add(hms);
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().HizmetMaliyetiTasraKaydet(kullanan, istek.Yil, istek.Muhasebe, Temiz(istek.Birim), Temiz(istek.Kurumsal), hmc);
                if (sonuc == null || !sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = EkFormUclari.HataMetni(sonuc?.hataStr, "Kayıt yapılamadı.") });
                return Results.Json(new { basarili = true, bilgi = "Bilgiler başarıyla saklandı." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı.") });
            }
        }

        /// <summary>Onayla / Onay Kaldır (eski Onayla): formu bütün birimler için veri girişine kapatır ya da açar.</summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, HizmetMaliyetiTasraOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Yil <= 0) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });
            try
            {
                ENUMTasraHazirlikOnayDurum islem = istek.Onayla ? ENUMTasraHazirlikOnayDurum.KurumOnayla : ENUMTasraHazirlikOnayDurum.KurumOnayKaldir;
                Sonuc s = TNS.BHMv2.Arac.Tanimla().OnayKodAtaTasra(kullanan, istek.Yil, 0, "000", "0000", ENUMBirimOnayTur.HIZMETMALIYETITASRA, islem);
                if (s == null || !s.islemSonuc) return Results.Json(new { basarili = false, hata = "Onaylama/Onay kaldırma işlemi başarısız! " + EkFormUclari.HataMetni(s?.hataStr, "") });
                return Results.Json(new { basarili = true, bilgi = "Onaylama/Onay kaldırma işlemi başarılı!" });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Onaylama/Onay kaldırma işlemi başarısız!" });
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        /// <summary>
        /// Yazdır (eski Yazdir("0")): seçili birim, kurum kodu varsayılan kurum. Yazdır (Toplu) (eski Yazdir("1"), sistem yöneticisi):
        /// muhasebe / birim aranmaz, sayfada seçili muhasebe biriminin kurumsal kodunun konsolide raporu (kod boşsa bütün kurum). Rapor eski HizmetMaliyetiTespitiHazirlaTasra ile üretilir.
        /// </summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, int? muhasebe, string birim, string kurumsal, bool? toplu)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            bool kurToplam = toplu == true;
            if (kurToplam && !SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            string kurKod = kurToplam ? Temiz(kurumsal).Replace(".", "") : "";
            if (kurKod == "") kurKod = TNS.DEG.Arac.VarsayilanKurumBul() ?? "";
            string hata = "";
            if (!kurToplam)
            {
                if ((muhasebe ?? 0) <= 0) hata += " Muhasebe bilgisi boş bırakılamaz.";
                if (Temiz(birim) == "") hata += " Birim bilgisi boş bırakılamaz.";
            }
            if (yil <= 0) hata += " Yıl alanı boş bırakılamaz.";
            if (hata != "") return Hata(hata.Trim());

            string muh = (muhasebe ?? 0) > 0 ? muhasebe.Value.ToString() : "";
            Hashtable raporParam = new Hashtable();
            raporParam.Add("ButceYil", yil.ToString());
            raporParam.Add("Saymanlik", muh);
            raporParam.Add("Birim", Temiz(birim));
            raporParam.Add("KurKod", kurKod);
            raporParam.Add("KurToplam", kurToplam ? "1" : "0");
            AsposeLisans.Yukle();
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().HizmetMaliyetiTespitiHazirlaTasra(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), EkFormUclari.ExcelTur(), raporParam, ""),
                "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "BirimHizmetMaliyetiTasra") + "_" + muh + "_" + Temiz(birim) + "_" + kurKod + "." + EkFormUclari.ExcelTur());
        }

        /// <summary>Yazdır (Girmeyen) (eski btnGirmeyenler_Click): yılın formunu hiç girmemiş açık muhasebe birimleri (Excel).</summary>
        private static IResult Girmeyenler(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (yil <= 0) return Hata("Yıl alanı boş bırakılamaz.");
            try
            {
                string[] sql = new string[1];
                sql[0] = "SELECT  MB.MUHASEBEKOD, MB.BIRIMKOD, MB.KURKOD, M.MUHASEBEAD , MB.BIRIMAD FROM  MUHMUHASEBE  M, MUHMUHASEBEBIRIM MB ";
                sql[0] += " WHERE M.MUHASEBEKOD = MB.MUHASEBEKOD AND BIRIMKOD <> '000' ";
                sql[0] += " AND 1 NOT IN (SELECT 1 FROM ButceHazirlik.dbo.BHFHIZMETMALIYETTESPITTASRA T WHERE T.YIL=" + yil.ToString() + " AND T.MUHASEBEKOD = MB.MUHASEBEKOD AND T.BIRIMKOD=MB.BIRIMKOD AND T.KURKOD=MB.KURKOD)";
                sql[0] += " AND 1 NOT IN (SELECT 1 FROM ButceIslemleri.dbo.MUHMUHASEBEBIRIMKAPALI T WHERE T.MUHASEBEKOD = MB.MUHASEBEKOD AND T.BIRIMKOD=MB.BIRIMKOD AND T.KURKOD=MB.KURKOD)";
                sql[0] += " order by 1,2,3";
                string[] baglanti = new string[] { "BaglantiSatiriBUM" };

                SortedList sl = TNS.UZY.Arac.Tanimla().SorguBirlestir(sql, baglanti, new int[] { 0, 1, 2 });
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosya = DosyaIslem.DosyaAdUret("HizMalTesGirmeyen_", "") + "." + GenelIslemler.ExcelTur();
                XLS.BosDosyaAc(sonucDosya);

                XLS.HucreDegerYaz(0, 0, yil.ToString() + " Yılı Bütçe Hazırlığı Çalışmalarında Hizmet Maliyeti Tesbiti Girmeyen Birim Listesi");
                int satir = 1;
                if (sl != null)
                {
                    foreach (DictionaryEntry de in sl)
                    {
                        TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                        TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                        XLS.HucreDegerYaz(satir, 0, alAnahtar.IntAl(0));
                        XLS.HucreDegerYaz(satir, 1, alAnahtar.StrAl(1));
                        XLS.HucreDegerYaz(satir, 2, alAnahtar.StrAl(2));

                        XLS.HucreDegerYaz(satir, 3, alVeri.StrAl(0));
                        XLS.HucreDegerYaz(satir++, 4, alVeri.StrAl(1));
                    }
                }
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosya, Path.GetFileName(sonucDosya), true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(EkFormUclari.HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }
    }
}
