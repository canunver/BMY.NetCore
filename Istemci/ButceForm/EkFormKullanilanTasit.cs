using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.KYM;

namespace Istemci.ButceForm
{
    public class KullanilanTasitSatirIstegi
    {
        public string SiraNo { get; set; }
        public int? KaynakKod { get; set; }
        public int? YasKod { get; set; }
        public double? Adet { get; set; }
    }

    public class KullanilanTasitIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        /// <summary>Seçili Merkez (1) / Taşra (2) / Toplam (3).</summary>
        public int MerkezTasra { get; set; }
        /// <summary>Tablonun listelendiği Merkez / Taşra / Toplam (eski hdnLislenenMerkezTasra).</summary>
        public int Listelenen { get; set; }
        public List<KullanilanTasitSatirIstegi> Satirlar { get; set; }
    }

    /// <summary>
    /// Form 19.1 - Kullanılan taşıtlara ilişkin bilgi formu; eski ButceForm/KullanilanTasitlar.aspx:
    ///   GET  /api/ekform/kullanilantasit/liste?yil=&amp;kur=&amp;merkezTasra=   (eski SozlukleriOlustur + btnListele_Click -> TasitSozlukListele, KullanilanTasitlarListele)
    ///   POST /api/ekform/kullanilantasit/kaydet                           (eski btnKaydet_Click -> KullanilanTasitlarKaydet)
    ///   GET  /api/ekform/kullanilantasit/yazdir?yil=&amp;kur=                 (eski btnYazdir_Click -> KullanilanTasitlarYazdir)
    /// </summary>
    public static class EkFormKullanilanTasit
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/kullanilantasit/liste", Liste);
            app.MapPost("/api/ekform/kullanilantasit/kaydet", Kaydet);
            app.MapGet("/api/ekform/kullanilantasit/yazdir", Yazdir);
        }

        /// <summary>Eski SozlukleriOlustur: kaynak listesi.</summary>
        private static List<object> Kaynaklar()
        {
            return new List<object>
            {
                new { kod = 1, ad = "Bütçe" },
                new { kod = 2, ad = "Döner Sermaye" },
                new { kod = 3, ad = "Fon" },
                new { kod = 4, ad = "Diğer" },
                new { kod = 5, ad = "667 Sayılı OHAL KHK ile Devir" }
            };
        }

        /// <summary>Eski SozlukleriOlustur: yaş listesi (kod yaş aralığının başı).</summary>
        private static List<object> Yaslar()
        {
            return new List<object>
            {
                new { kod = 0, ad = "0-1" },
                new { kod = 2, ad = "2-4" },
                new { kod = 5, ad = "5-9" },
                new { kod = 10, ad = "10-14" },
                new { kod = 15, ad = "15 ve üstü" }
            };
        }

        /// <summary>Eski btnListele_Click: seçim yoksa (0) merkez listelenir.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, int? merkezTasra)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            int merkez1Tasra2Toplam3 = merkezTasra ?? 1;
            if (merkez1Tasra2Toplam3 == 0) merkez1Tasra2Toplam3 = 1;
            try
            {
                List<object> sozluk = EkFormEdinilecekTasit.TasitSozlugu(yil);
                ObjectArray hat = TNS.BHF.Arac.Tanimla().KullanilanTasitlarListele(kullanan, yil, kur.Trim(), merkez1Tasra2Toplam3);
                if (!hat.sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(hat.sonuc.hataStr, "Kullanılan taşıtlar okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (TasitGenelBilgi tgb in hat.objeler)
                {
                    satirlar.Add(new { siraNo = tgb.siraNo ?? "", kaynakKod = tgb.kaynakKod, yasKod = tgb.yasKod, adet = tgb.adet });
                }
                return Results.Json(new
                {
                    sozluk = sozluk,
                    kaynaklar = Kaynaklar(),
                    yaslar = Yaslar(),
                    listelenen = merkez1Tasra2Toplam3,
                    satirlar = satirlar
                });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kullanılan taşıtlar okunamadı."));
            }
        }

        /// <summary>
        /// Eski btnKaydet_Click: seçili Merkez/Taşra ile listelenen aynı değilse kaydedilmez; bütün satırlar gönderilir,
        /// adedi olmayan satırları atlama, satır ve tekrar denetimi iş kuralındadır.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, KullanilanTasitIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new KullanilanTasitIstegi();
            if (!EkFormUclari.SecimGecerli(istek.Yil, istek.Kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            int secili = istek.MerkezTasra == 0 ? 1 : istek.MerkezTasra;
            int listelenen = istek.Listelenen == 0 ? 1 : istek.Listelenen;
            if (secili != listelenen) return EkFormUclari.Hata("Lütfen listeleme yaptıktan sonra kaydediniz!");
            try
            {
                TNSCollection form = new TNSCollection();
                foreach (KullanilanTasitSatirIstegi s in istek.Satirlar ?? new List<KullanilanTasitSatirIstegi>())
                {
                    if (s == null) continue;
                    TasitGenelBilgi tgb = new TasitGenelBilgi();
                    tgb.siraNo = s.SiraNo ?? "";
                    tgb.kaynakKod = s.KaynakKod ?? 0;
                    tgb.yasKod = s.YasKod ?? 0;
                    tgb.adet = s.Adet.HasValue ? Convert.ToInt32(s.Adet.Value) : 0;
                    form.Add(tgb);
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().KullanilanTasitlarKaydet(kullanan, istek.Yil, istek.Kur.Trim(), secili, form);
                if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return EkFormUclari.Tamam("Kayıt işlemi başarıyla gerçekleşti.");
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>Eski btnYazdir_Click: rapor her zaman toplamı basar; dosya adı "form" + form numarası (2009 ve sonrası 19).</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            kur = kur.Trim();
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().KullanilanTasitlarYazdir(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), EkFormUclari.ExcelTur(),
                EkFormUclari.RaporParam(yil, kur, ""), ""), "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "MevcutTasitlar"));
        }
    }
}
