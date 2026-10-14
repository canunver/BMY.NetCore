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
    public class EdinilecekTasitSatirIstegi
    {
        /// <summary>(T) cetveli sıra no (taşıt sözlüğü kodu).</summary>
        public string SiraNo { get; set; }
        public double? Adet { get; set; }
        public string KullanimYeri { get; set; }
        /// <summary>Finansman kaynağının adı (eski tabloda da adı saklanıyordu).</summary>
        public string Finkaynak { get; set; }
    }

    public class EdinilecekTasitIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public List<EdinilecekTasitSatirIstegi> Satirlar { get; set; }
    }

    /// <summary>
    /// Form 18 - 237 Sayılı Taşıt Kanununa göre edinilecek taşıtlar; eski ButceForm/EdinilecekTasitlar.aspx:
    ///   GET  /api/ekform/edinilecektasit/liste?yil=&amp;kur=    (eski GridInit + TasitListele -> TasitSozlukListele, EdinilecekTasitlarListele)
    ///   POST /api/ekform/edinilecektasit/kaydet               (eski btnKaydet_Click -> EdinilecekTasitlarKaydet)
    ///   GET  /api/ekform/edinilecektasit/yazdir?yil=&amp;kur=   (eski btnYazdir_Click -> EdinilecekTasitHazirla)
    /// "Boş Satır Aç" ve "Satır Sil" eski sayfada da yalnızca tarayıcıda çalışıyordu (tablo kaydedilince bütünüyle yeniden yazılır).
    /// </summary>
    public static class EkFormEdinilecekTasit
    {
        /// <summary>Eski EdinilecekTasitlar.FinansmanKaynak (FRMEDS013..FRMEDS021); tabloda adı seçilir ve saklanır.</summary>
        private static readonly string[] FinansmanKaynaklari =
        {
            "Merkezi Yönetim Bütçesi",
            "Merkezi Yönetim Bütçesi (Öz Gelir)",
            "Merkezi Yönetim Bütçesi (Özel Ödenek)",
            "İl Özel İdare Bütçesi",
            "Yurtiçi Hibe",
            "AB Hibesi",
            "Merkezi Yönetim Bütçesi + AB Katkısı",
            "Döner Sermaye",
            "Diğer"
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/edinilecektasit/liste", Liste);
            app.MapPost("/api/ekform/edinilecektasit/kaydet", Kaydet);
            app.MapGet("/api/ekform/edinilecektasit/yazdir", Yazdir);
        }

        /// <summary>
        /// Eski GridInit/SozlukleriOlustur: yılın taşıt sözlüğü (TasitSozlukListele) seçim listesi olarak; kod sıra no, ad eski
        /// açılır listenin gösterdiği TasitSozluk.Aciklama ("sıra no: cinsi (diferansiyel)"). Taşıt formları (18, 19.1) ortak kullanır.
        /// </summary>
        internal static List<object> TasitSozlugu(int yil)
        {
            List<object> liste = new List<object>();
            foreach (TasitSozluk ts in TNS.BHF.Arac.Tanimla().TasitSozlukListele(yil) ?? new TasitSozluk[0])
                liste.Add(new { kod = ts.SiraNo ?? "", ad = ts.Aciklama });
            return liste;
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            try
            {
                List<object> sozluk = TasitSozlugu(yil);
                List<object> kaynaklar = new List<object>();
                foreach (string ad in FinansmanKaynaklari) kaynaklar.Add(new { kod = ad, ad = ad });

                List<object> satirlar = new List<object>();
                foreach (EdinilecekTasitSatiri ets in TNS.BHF.Arac.Tanimla().EdinilecekTasitlarListele(kullanan, yil, kur.Trim()) ?? new EdinilecekTasitSatiri[0])
                {
                    satirlar.Add(new
                    {
                        siraNo = ets.siraNo ?? "",
                        adet = ets.adet,
                        kullanimYeri = ets.kullanimYeri ?? "",
                        finkaynak = ets.finkaynak ?? ""
                    });
                }
                return Results.Json(new { sozluk = sozluk, kaynaklar = kaynaklar, satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Edinilecek taşıtlar okunamadı."));
            }
        }

        /// <summary>
        /// Eski btnKaydet_Click: tablodaki bütün satırlar (boş açılmış satırlar da) olduğu gibi gönderilir; satır denetimi
        /// (taşıt, finansman kaynağı, adet boş olamaz) iş kuralındadır.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, EdinilecekTasitIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new EdinilecekTasitIstegi();
            if (!EkFormUclari.SecimGecerli(istek.Yil, istek.Kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            try
            {
                TNSCollection ec = new TNSCollection();
                foreach (EdinilecekTasitSatirIstegi s in istek.Satirlar ?? new List<EdinilecekTasitSatirIstegi>())
                {
                    if (s == null) continue;
                    EdinilecekTasitSatiri ets = new EdinilecekTasitSatiri();
                    ets.siraNo = s.SiraNo ?? "";
                    ets.adet = s.Adet.HasValue ? Convert.ToInt32(s.Adet.Value) : 0;
                    ets.kullanimYeri = s.KullanimYeri ?? "";
                    ets.finkaynak = s.Finkaynak ?? "";
                    ec.Add(ets);
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().EdinilecekTasitlarKaydet(kullanan, istek.Yil, istek.Kur.Trim(), ec);
                if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return EkFormUclari.Tamam("Bilgiler başarıyla saklandı.");
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>Eski btnYazdir_Click: dosya adı "form" + form numarası (2009 ve sonrası 18).</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            kur = kur.Trim();
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().EdinilecekTasitHazirla(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), EkFormUclari.ExcelTur(),
                EkFormUclari.RaporParam(yil, kur, ""), ""), "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "EdinilecekTasitlar"));
        }
    }
}
