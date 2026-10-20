using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// <summary>Ekle / Değiştir isteği (eski EkrandanAl). tarih: "yyyy-MM-ddTHH:mm:ss".</summary>
    public class PersonelGiderIstegi
    {
        public int personelKod { get; set; }
        public bool yeni { get; set; }
        public string tarih { get; set; }
        public int yerleskeKod { get; set; }
        public int harcamaTur { get; set; }
        public double tutar { get; set; }
    }

    /// <summary>Aktar isteği (eski CBVeriAktar).</summary>
    public class PersonelGiderAktarIstegi
    {
        public int yil { get; set; }
        public int ay { get; set; }
        public int harcamaTur { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri "Giderler" sekmesi (eski InsanKaynaklari/PersonelGider.aspx; yeni sayfa InsanKaynaklari/PersonelGider) uçları:
    ///   GET  /api/personelbilgi/gider/tanim                                     Tür listesi, varsayılan yıl / ay (eski Page_Load)
    ///   GET  /api/personelbilgi/gider/liste?personelKod=&amp;yil=&amp;ay=               (eski Listele)
    ///   GET  /api/personelbilgi/gider/oku?personelKod=&amp;yerleskeKod=&amp;harcamaTur=&amp;tarih=   (eski SatirSecildi)
    ///   GET  /api/personelbilgi/gider/yerler?ust=&amp;ara=                          Harcama Yeri ağacı (eski ExtOrgSecim AgacTur=2: yerleşke / oda)
    ///   POST /api/personelbilgi/gider/kaydet                                    Ekle / Değiştir (eski btnKaydet_Click)
    ///   POST /api/personelbilgi/gider/aktar                                     (eski CBVeriAktar)
    ///   POST /api/personelbilgi/gider/kum-yukle (multipart: personelKod, yil, ay, yerleskeKod, harcamaTur, tutar, dosya)  (eski btnKumDosyaYukle_Click)
    /// </summary>
    public static class PersonelGiderUclari
    {
        private const string SayfaAd = "PersonelGider";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/gider/tanim", Tanim);
            app.MapGet("/api/personelbilgi/gider/liste", Liste);
            app.MapGet("/api/personelbilgi/gider/oku", Oku);
            app.MapGet("/api/personelbilgi/gider/yerler", Yerler);
            app.MapPost("/api/personelbilgi/gider/kaydet", Kaydet);
            app.MapPost("/api/personelbilgi/gider/aktar", Aktar);
            app.MapPost("/api/personelbilgi/gider/kum-yukle", KumYukle);
        }

        private static IIKYServis Servis()
        {
            return TNS.IKY.Arac.Tanimla();
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        /// <summary>"yyyy-MM-ddTHH:mm[:ss]" ya da "gg.aa.yyyy SS:dd:ss" -> TNSDateTime (boş / yanlışsa boş tarih).</summary>
        private static TNSDateTime ZamanOku(string s)
        {
            s = (s ?? "").Trim();
            if (s == "") return new TNSDateTime();
            string[] bicimler = { "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss.fff", "yyyy-MM-dd" };
            if (DateTime.TryParseExact(s, bicimler, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d)) return new TNSDateTime(d);
            return new TNSDateTime(s);
        }

        private static string ZamanYaz(TNSDateTime t)
        {
            if (t == null || TNSDateTime.IsNull(t)) return "";
            return t.Oku().ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }

        private static object Satir(IKYPersonelGider g)
        {
            return new
            {
                yerleskeKod = g.yerleskeKod,
                harcamaTur = (int)g.harcamaTur,
                tarihDT = ZamanYaz(g.tarih),
                tutar = g.tutar,
                yerleskeAdi = g.yerleskeAdi ?? "",
                harcamaTurAdi = g.harcamaTurAdi ?? ""
            };
        }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            List<object> turler = new List<object>();
            foreach (IntKodStrAd t in IKYPersonelGider.HarcamaTurListe(false))
                turler.Add(new { t.kod, t.ad });
            return Results.Json(new { yil = DateTime.Now.Year, ay = DateTime.Now.Month, harcamaTurleri = turler });
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? yil, int? ay)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            List<object> satirlar = new List<object>();
            int pKod = personelKod ?? -1;
            if (pKod <= 0) return Results.Json(new { satirlar = satirlar });

            IKYPersonelGider gider = new IKYPersonelGider();
            gider.pKod = pKod;
            gider.kriterYil = yil ?? -1;
            gider.kriterAy = ay ?? -1;
            ObjectArray pler = Servis().PersonelGiderListele(kullanan, gider);
            if (!pler.sonuc.islemSonuc) return Results.Json(new { satirlar = satirlar, hata = PersonelBilgiOrtak.HataMetni(pler.sonuc.hataStr, "Gider bilgileri okunamadı.") });
            foreach (IKYPersonelGider g in pler.objeler)
                satirlar.Add(Satir(g));
            return Results.Json(new { satirlar = satirlar });
        }

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? personelKod, int? yerleskeKod, int? harcamaTur, string tarih)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            int pKod = personelKod ?? -1;
            IKYPersonelGider gider = new IKYPersonelGider();
            gider.pKod = pKod;
            gider.yerleskeKod = yerleskeKod ?? -1;
            gider.harcamaTur = (IKYPersonelGider.ENUMPersonelHarcamaTur)(harcamaTur ?? -1);
            gider.tarih = ZamanOku(tarih);
            if (pKod <= 0) return Results.Json(new { hata = "Personel seçilmedi." });

            ObjectArray pler = Servis().PersonelGiderListele(kullanan, gider);
            if (!pler.sonuc.islemSonuc) return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(pler.sonuc.hataStr, "Gider bilgisi okunamadı.") });
            if (pler.ObjeSayisi != 1) return Results.Json(new { hata = "Okunan kayıt sayısı:" + pler.ObjeSayisi });
            return Results.Json(new { bilgi = Satir((IKYPersonelGider)pler.objeler[0]) });
        }

        /// <summary>
        /// Harcama Yeri seçimi (eski ExtOrgSecim AgacTur=2, nodeLoadOdaParam: KYM yerleşke / oda tanımları). ust verilirse
        /// altındakiler, ara verilirse tarifine göre arananlar (düz), ikisi de yoksa kök yerleşkeler.
        /// </summary>
        private static IResult Yerler(HttpContext baglam, IWebHostEnvironment ortam, int? ust, string ara)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                YerleskeOda kriter = new YerleskeOda();
                kriter.ustKod = Math.Max(0, ust ?? 0);
                if (kriter.ustKod == 0) kriter.tarif = (ara ?? "").Trim();
                ObjectArray<YerleskeOda> oa = TNS.KYM.Arac.Tanimla().YerleskeOdaListele(kullanan, kriter);
                if (oa == null || !oa.sonuc.islemSonuc) return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(oa?.sonuc?.hataStr, "Harcama yerleri okunamadı.") });
                bool arama = kriter.ustKod == 0 && kriter.tarif != "";
                List<object> satirlar = new List<object>();
                foreach (YerleskeOda o in oa.objeler)
                    satirlar.Add(new { kod = o.kod, ad = o.ad ?? "", tarif = o.tarif ?? "", altVar = !arama && o.altDalVar });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Harcama yerleri okunamadı." });
            }
        }

        private static IKYPersonelGider EkrandanAl(PersonelGiderIstegi istek)
        {
            IKYPersonelGider gider = new IKYPersonelGider();
            gider.pKod = istek.personelKod;
            gider.tarih = ZamanOku(istek.tarih);
            gider.yerleskeKod = istek.yerleskeKod;
            gider.harcamaTur = (IKYPersonelGider.ENUMPersonelHarcamaTur)istek.harcamaTur;
            gider.tutar = istek.tutar;
            return gider;
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelGiderIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            if (istek.personelKod <= 0) return Hata("Personel Bilgisi boş bırakılamaz.");
            Sonuc sonuc = Servis().PersonelGiderKaydet(kullanan, EkrandanAl(istek), istek.yeni);
            if (!sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Gider kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = istek.yeni ? "Gider eklendi." : "Gider değiştirildi." });
        }

        private static async Task<IResult> Aktar(HttpContext baglam, IWebHostEnvironment ortam, PersonelGiderAktarIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            IKYPersonelGider gider = new IKYPersonelGider();
            gider.kriterYil = istek.yil;
            gider.kriterAy = istek.ay;
            gider.harcamaTur = (IKYPersonelGider.ENUMPersonelHarcamaTur)istek.harcamaTur;
            Sonuc s = Servis().PersonelGiderAktar(kullanan, gider);
            if (!s.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(s.hataStr, "Veriler aktarılamadı."));
            return Results.Json(new { basarili = true, bilgi = "Veriler aktarıldı." });
        }

        /// <summary>
        /// Eski btnKumDosyaYukle_Click: A sütununda sicil no ya da kimlik no, B sütununda tutar bulunan dosya; seçili yıl, ay,
        /// harcama yeri ve tür için ayın son gününe (önceki veriler silinerek) yüklenir. İlk boş A hücresinde durulur.
        /// </summary>
        private static async Task<IResult> KumYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            IFormCollection form = await baglam.Request.ReadFormAsync();
            PersonelGiderIstegi istek = new PersonelGiderIstegi();
            istek.personelKod = OrtakFonksiyonlar.ConvertToInt(form["personelKod"].ToString(), -1);
            istek.yerleskeKod = OrtakFonksiyonlar.ConvertToInt(form["yerleskeKod"].ToString(), 0);
            istek.harcamaTur = OrtakFonksiyonlar.ConvertToInt(form["harcamaTur"].ToString(), 0);
            istek.tutar = OrtakFonksiyonlar.ConvertToDouble(form["tutar"].ToString(), 0);
            IKYPersonelGider pg = EkrandanAl(istek);
            int yil = OrtakFonksiyonlar.ConvertToInt(form["yil"].ToString(), 0);
            int ay = OrtakFonksiyonlar.ConvertToInt(form["ay"].ToString(), 0);

            Sonuc s = new Sonuc();
            if (yil == 0 || ay == 0)
                s.DegerAta("Lütfen Yıl ve Ay Seçiniz!");
            if (pg.yerleskeKod == 0)
                s.DegerAta("Lütfen Yerleşke Seçiniz");
            if (s.islemSonuc && (ay < 1 || ay > 12 || yil < 1 || yil > 9999))
                s.DegerAta("Lütfen Yıl ve Ay Seçiniz!");

            IFormFile dosya = form.Files["dosya"];
            if (s.islemSonuc && (dosya == null || dosya.Length == 0))
                s.DegerAta("Yüklenecek dosyayı seçiniz.");

            if (s.islemSonuc)
            {
                pg.tarih = new TNSDateTime(new DateTime(yil, ay, DateTime.DaysInMonth(yil, ay)));
                TNSCollection liste = new TNSCollection();
                try
                {
                    AsposeLisans.Yukle();
                    Tablo XLS = GenelIslemler.NewTablo();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await dosya.CopyToAsync(ms);
                        ms.Position = 0;
                        XLS.DosyaOkuAc(ms);
                        try
                        {
                            int sat = 0;
                            while (true)
                            {
                                string tcOrSic = XLS.HucreDegerAl(sat, 0);
                                double tutar = XLS.HucreDegerAlDbl(sat, 1);
                                sat++;
                                if (string.IsNullOrWhiteSpace(tcOrSic)) break;
                                if (tutar > 0)
                                {
                                    IKYPersonelGider pgIc = new IKYPersonelGider(pg);
                                    pgIc.tckimlikNo = tcOrSic;
                                    pgIc.tutar = tutar;
                                    liste.Add(pgIc);
                                }
                            }
                        }
                        finally
                        {
                            XLS.DosyaKapat();
                        }
                    }
                }
                catch (Exception)
                {
                    s.DegerAta("Dosya okunamadı.");
                }
                if (s.islemSonuc)
                {
                    if (liste.Count == 0)
                        s.DegerAta("Yüklenecek veri bulunamadı");
                    else
                        s = Servis().PersonelGiderYukle(kullanan, liste);
                }
            }
            if (s.islemSonuc) return Results.Json(new { basarili = true, bilgi = "Veriler yüklendi. " + PersonelBilgiOrtak.Duz(s.bilgiStr) });
            return Hata("Veriler yüklenemedi:" + PersonelBilgiOrtak.Duz(s.hataStr));
        }
    }
}
