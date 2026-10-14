using System;
using System.Collections;
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
    public class FizikselDegerSatirIstegi
    {
        public string Kod { get; set; }
        public string Aciklama { get; set; }
        /// <summary>yıl-3, yıl-2, yıl-1, yıl değerleri (dizinde 0..3).</summary>
        public double?[] Degerler { get; set; }
    }

    public class FizikselDegerIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        /// <summary>Yalnızca girilebilen (eski tabloda kilitli olmayan, degerGirilsinMi) satırlar.</summary>
        public List<FizikselDegerSatirIstegi> Satirlar { get; set; }
    }

    /// <summary>
    /// Form 11: fiziksel değerler bilgi tablosu (eski ButceForm/FizikselDegerler.aspx):
    ///   GET  /api/ekform/fizikseldegerler/liste?yil=&amp;kur=    (eski FizDegListele -> FizikselDegerlerListele)
    ///   POST /api/ekform/fizikseldegerler/kaydet              (eski btnKaydet_Click -> FizikselDegerlerKaydet)
    ///   GET  /api/ekform/fizikseldegerler/yazdir?yil=&amp;kur=   (eski btnYazdir_Click -> FizikselDegerlerHazirla)
    /// </summary>
    public static class EkFormFizikselDegerler
    {
        /// <summary>Eski tablodaki dört değer sütununun yılları: yıl-3, yıl-2, yıl-1, yıl.</summary>
        private static readonly int[] YilFarklari = { -3, -2, -1, 0 };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/fizikseldegerler/liste", Liste);
            app.MapPost("/api/ekform/fizikseldegerler/kaydet", Kaydet);
            app.MapGet("/api/ekform/fizikseldegerler/yazdir", Yazdir);
        }

        /// <summary>
        /// Eski FizDegListele: degerGirilsinMi olmayan satırlar kilitli; kodu "1" ile başlayan 3 haneli satırlarda değerler gösterilmez;
        /// kod ve açıklama, kodun noktasız uzunluğu x 6 piksel içeriden yazılır (girinti).
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            try
            {
                ObjectArray fdv = TNS.BHF.Arac.Tanimla().FizikselDegerlerListele(kullanan, yil, kur.Trim());
                if (!fdv.sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(fdv.sonuc.hataStr, "Fiziksel değerler okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (object o in fdv.objeler ?? new TNSCollection())
                {
                    if (!(o is FizikselDeger fd)) continue;
                    string kod = fd.kod ?? "";
                    double?[] d = new double?[YilFarklari.Length];
                    if (!(kod.Length == 3 && kod.Substring(0, 1) == "1"))
                    {
                        foreach (FizikselDegerYilBilgi y in fd.degerler ?? new FizikselDegerYilBilgi[0])
                        {
                            for (int i = 0; i < YilFarklari.Length; i++)
                                if (y.yil == yil + YilFarklari[i]) d[i] = y.deger;
                        }
                    }
                    satirlar.Add(new
                    {
                        kod = kod,
                        aciklama = fd.aciklama ?? "",
                        degerGirilsinMi = fd.degerGirilsinMi,
                        girinti = kod.Replace(".", "").Length * 6,
                        degerler = d
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Fiziksel değerler okunamadı."));
            }
        }

        /// <summary>
        /// Eski btnKaydet_Click: kilitli olmayan her satır degerGirilsinMi = true ile gönderilir; değerler tam sayıya çevrilir
        /// (ConvertToInt) ve yalnızca sıfırdan büyük olanlar (sütunun yılıyla) saklanır.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, FizikselDegerIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new FizikselDegerIstegi();
            if (!EkFormUclari.SecimGecerli(istek.Yil, istek.Kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            try
            {
                TNSCollection fdc = new TNSCollection();
                foreach (FizikselDegerSatirIstegi s in istek.Satirlar ?? new List<FizikselDegerSatirIstegi>())
                {
                    if (s == null) continue;
                    FizikselDeger fd = new FizikselDeger();
                    fd.kod = s.Kod ?? "";
                    fd.aciklama = s.Aciklama ?? "";
                    fd.degerGirilsinMi = true;
                    ArrayList al = new ArrayList();
                    for (int j = 0; j < YilFarklari.Length; j++)
                    {
                        FizikselDegerYilBilgi y = new FizikselDegerYilBilgi();
                        y.yil = istek.Yil + YilFarklari[j];
                        double? deger = s.Degerler != null && s.Degerler.Length > j ? s.Degerler[j] : null;
                        y.deger = deger.HasValue ? OrtakFonksiyonlar.ConvertToInt((object)deger.Value, 0) : 0;
                        if (y.deger > 0) al.Add(y);
                    }
                    fd.degerler = (FizikselDegerYilBilgi[])al.ToArray(typeof(FizikselDegerYilBilgi));
                    fdc.Add(fd);
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().FizikselDegerlerKaydet(kullanan, istek.Yil, istek.Kur.Trim(), fdc);
                if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return EkFormUclari.Tamam("Bilgiler başarıyla saklandı.");   // eski FRMFID005
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>Eski btnYazdir_Click: FizikselDegerlerHazirla; dosya adı "form11_{kur}" (FormNoBul "FizikselDegerler").</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            kur = kur.Trim();
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().FizikselDegerlerHazirla(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), EkFormUclari.ExcelTur(),
                EkFormUclari.RaporParam(yil, kur, ""), ""),
                "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "FizikselDegerler") + "_" + kur + "." + EkFormUclari.ExcelTur());
        }
    }
}
