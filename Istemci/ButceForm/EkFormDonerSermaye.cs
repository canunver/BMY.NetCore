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
    public class DonerSermayeSatirIstegi
    {
        public string Kod { get; set; }
        public string Aciklama { get; set; }
        /// <summary>Tutarlar: 0 = (yıl-2) gerçekleşme, 1 = (yıl-1) ödenek, 2 = (yıl-1) harcama, 3..5 = yıl, yıl+1, yıl+2 teklif.</summary>
        public double?[] Degerler { get; set; }
    }

    public class DonerSermayeIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        /// <summary>Döner sermaye numarası (1..6); fon için 0.</summary>
        public int No { get; set; }
        /// <summary>Döner sermaye / fon adı (eski txtDSAdi).</summary>
        public string Ad { get; set; }
        /// <summary>Yalnız değer girilebilen (kilitli olmayan) satırlar.</summary>
        public List<DonerSermayeSatirIstegi> Satirlar { get; set; }
    }

    /// <summary>
    /// Form 23 (döner sermaye gelir-gider cetveli, eski ButceForm/DonerSermaye.aspx; döner sermaye no 1..6) ve
    /// Form 24 (fon gelir-gider cetveli, eski ButceForm/FonGelirGider.aspx; döner sermaye no 0):
    ///   GET  /api/ekform/donersermaye/liste?yil=&amp;kur=&amp;no=     (eski DonerSermayeListele -> DonerSermayeListele)
    ///   POST /api/ekform/donersermaye/kaydet                     (eski btnKaydet_Click -> DonerSermayeKaydet)
    ///   GET  /api/ekform/donersermaye/yazdir?yil=&amp;kur=&amp;no=     (eski btnYazdir_Click -> DSFonGelirGiderHazirla / FonGelirGiderHazirla)
    /// </summary>
    public static class EkFormDonerSermaye
    {
        /// <summary>Eski FRMDOS007 (döner sermaye).</summary>
        private const string DSSecimHatasi = "Yıl, Kurum veya Döner sermaye numarası alanlarından biri veya bir kaçı boş bırakılmış.  Lütfen ilgili alanları kontrol edip tekrar deneyin.";
        /// <summary>Eski FRMFGG006 (fon).</summary>
        private const string FonSecimHatasi = "Yıl veya Kurum alanlarından biri veya bir kaçı boş bırakılmış.  Lütfen ilgili alanları kontrol edip tekrar deneyin.";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/donersermaye/liste", Liste);
            app.MapPost("/api/ekform/donersermaye/kaydet", Kaydet);
            app.MapGet("/api/ekform/donersermaye/yazdir", Yazdir);
        }

        private static string SecimHatasi(int no) { return no == 0 ? FonSecimHatasi : DSSecimHatasi; }

        /// <summary>Döner sermaye için 1..6 (eski ddlDSNo), fon için 0.</summary>
        private static bool NoGecerli(int no) { return no >= 0 && no <= 6; }

        /// <summary>
        /// Tablo sütunlarının yılı ve ödenek mi bilgisi (eski GridInit başlıkları ve btnKaydet_Click'teki j döngüsü):
        /// 0: yıl-2 gerçekleşme, 1: yıl-1 ödenek, 2: yıl-1 harcama, 3..5: yıl..yıl+2 teklif (ödenek).
        /// </summary>
        private static int SutunYil(int yil, int sutun)
        {
            if (sutun == 0) return yil - 2;
            if (sutun <= 2) return yil - 1;
            return yil + sutun - 3;
        }

        private static bool SutunOdenekMi(int sutun) { return !(sutun == 0 || sutun == 2); }

        /// <summary>Eski DonerSermayeListele: satırlar, girilemeyen (üst) satırlar ve döner sermaye / fon adı.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, int no)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur) || !NoGecerli(no)) return EkFormUclari.Hata(SecimHatasi(no));
            try
            {
                TNS.BHF.DonerSermaye dbv = TNS.BHF.Arac.Tanimla().DonerSermayeListele(kullanan, yil, kur.Trim(), no);
                if (!dbv.sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(dbv.sonuc.hataStr, "Döner sermaye bilgileri okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (DonerSermayeSatir dss in dbv.donerSermayeSatirlari ?? new DonerSermayeSatir[0])
                {
                    double?[] d = new double?[6];
                    foreach (DonerSermayeYil dsy in dss.degerler ?? new DonerSermayeYil[0])
                    {
                        if (dsy.yil == yil - 2) d[0] = dsy.deger;
                        if (dsy.yil == yil - 1)
                        {
                            if (dsy.odenekMi) d[1] = dsy.deger;
                            else d[2] = dsy.deger;
                        }
                        if (dsy.yil == yil) d[3] = dsy.deger;
                        if (dsy.yil == yil + 1) d[4] = dsy.deger;
                        if (dsy.yil == yil + 2) d[5] = dsy.deger;
                    }
                    satirlar.Add(new { kod = dss.kod ?? "", aciklama = dss.aciklama ?? "", girilir = dss.degerGirilsinMi, degerler = d });
                }
                return Results.Json(new { ad = dbv.donerSermayeAd ?? "", satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Döner sermaye bilgileri okunamadı."));
            }
        }

        /// <summary>
        /// Eski btnKaydet_Click: yalnız kilitli olmayan (değer girilen) satırlar, her satırda yalnız sıfırdan büyük tutarlar gönderilir.
        /// Ad boşsa kaydedilmez (eski sayfa aynı uyarıyı veriyordu).
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, DonerSermayeIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new DonerSermayeIstegi();
            if (!EkFormUclari.SecimGecerli(istek.Yil, istek.Kur) || !NoGecerli(istek.No) || EkFormUclari.Temiz(istek.Ad) == "")
                return EkFormUclari.Hata(SecimHatasi(istek.No));
            try
            {
                TNSCollection dsc = new TNSCollection();
                foreach (DonerSermayeSatirIstegi s in istek.Satirlar ?? new List<DonerSermayeSatirIstegi>())
                {
                    if (s == null) continue;
                    DonerSermayeSatir dss = new DonerSermayeSatir();
                    dss.kod = s.Kod ?? "";
                    dss.aciklama = s.Aciklama ?? "";
                    dss.degerGirilsinMi = true;
                    ArrayList degerler = new ArrayList();
                    for (int j = 0; j < 6; j++)
                    {
                        DonerSermayeYil dsy = new DonerSermayeYil();
                        dsy.yil = SutunYil(istek.Yil, j);
                        dsy.odenekMi = SutunOdenekMi(j);
                        dsy.deger = s.Degerler != null && s.Degerler.Length > j ? (s.Degerler[j] ?? 0) : 0;
                        if (dsy.deger > 0) degerler.Add(dsy);
                    }
                    dss.degerler = (DonerSermayeYil[])degerler.ToArray(typeof(DonerSermayeYil));
                    dsc.Add(dss);
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().DonerSermayeKaydet(kullanan, istek.Yil, istek.Kur.Trim(), istek.No, istek.Ad, dsc);
                if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return EkFormUclari.Tamam("Bilgiler başarıyla saklandı.");
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>
        /// Eski btnYazdir_Click. Döner sermaye: DSFonGelirGiderHazirla (adı olan bütün döner sermayeler, her biri bir sayfada),
        /// dosya adı form23_{kur}; fon: FonGelirGiderHazirla (raporParam'a DSermayeNo = 0 eklenir), dosya adı form24_{kur}.
        /// </summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, int no)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(SecimHatasi(no));
            kur = kur.Trim();
            string tur = EkFormUclari.ExcelTur();
            if (no == 0)
            {
                Hashtable raporParam = EkFormUclari.RaporParam(yil, kur, "");
                raporParam.Add("DSermayeNo", "0");
                return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().FonGelirGiderHazirla(
                    kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), tur, raporParam, ""),
                    "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "FonGelirGider") + "_" + kur + "." + tur);
            }
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().DSFonGelirGiderHazirla(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), tur, EkFormUclari.RaporParam(yil, kur, ""), ""),
                "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "DonerSermaye") + "_" + kur + "." + tur);
        }
    }
}
