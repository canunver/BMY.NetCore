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
    public class HizmetBilgisiSatirIstegi
    {
        public string Baslangic { get; set; }
        public string Bitis { get; set; }
        public string HizmetAdi { get; set; }
        public string Aciklama { get; set; }
        /// <summary>Ekonomik kod 01..09 tutarları (dizinde 0..8).</summary>
        public double?[] Miktarlar { get; set; }
    }

    public class HizmetBilgisiIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public int Tamam { get; set; }
        public int EkYil { get; set; }
        public List<HizmetBilgisiSatirIstegi> Satirlar { get; set; }
    }

    /// <summary>
    /// Form 6 (ilk defa yapılması planlanan hizmetler, tamam = 2) ve Form 7 (tamamlanması planlanan hizmetler, tamam = 1);
    /// eski ButceForm/HizmetBilgisiEko.aspx?ekyil=..&amp;tamammiilkmi=..:
    ///   GET  /api/ekform/hizmetbilgisi/liste?yil=&amp;kur=&amp;tamam=&amp;ekyil=   (eski HizmetBilgiListele -> HizmetBilgisiListele)
    ///   POST /api/ekform/hizmetbilgisi/kaydet                            (eski btnKaydet_Click -> HizmetBilgisiKaydet)
    ///   GET  /api/ekform/hizmetbilgisi/yazdir?yil=&amp;kur=&amp;tamam=          (eski btnYazdir_Click -> HizmetBilgisiEkoHazirla, F6 / F7)
    /// </summary>
    public static class EkFormHizmetBilgisi
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/hizmetbilgisi/liste", Liste);
            app.MapPost("/api/ekform/hizmetbilgisi/kaydet", Kaydet);
            app.MapGet("/api/ekform/hizmetbilgisi/yazdir", Yazdir);
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, int tamam, int ekyil)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            try
            {
                ObjectArray hbv = TNS.BHF.Arac.Tanimla().HizmetBilgisiListele(kullanan, yil, kur.Trim(), tamam, yil + ekyil);
                if (!hbv.sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(hbv.sonuc.hataStr, "Hizmet bilgileri okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (HizmetBilgisiSatiri hbs in hbv.objeler)
                {
                    double?[] m = new double?[9];
                    foreach (HizmetBilgiEko hbe in hbs.miktarlar ?? new HizmetBilgiEko[0])
                    {
                        int i = Convert.ToInt16(hbe.ekoKod);
                        if (i >= 1 && i <= 9 && hbe.miktar != 0) m[i - 1] = hbe.miktar;
                    }
                    satirlar.Add(new
                    {
                        baslangic = EkFormUclari.TarihStr(hbs.baslangicTarihi),
                        bitis = EkFormUclari.TarihStr(hbs.bitisTarihi),
                        hizmetAdi = hbs.hizmetAdi ?? "",
                        aciklama = hbs.aciklama ?? "",
                        miktarlar = m
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Hizmet bilgileri okunamadı."));
            }
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, HizmetBilgisiIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new HizmetBilgisiIstegi();
            if (!EkFormUclari.SecimGecerli(istek.Yil, istek.Kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            try
            {
                TNSCollection hbsc = new TNSCollection();
                foreach (HizmetBilgisiSatirIstegi s in istek.Satirlar ?? new List<HizmetBilgisiSatirIstegi>())
                {
                    if (s == null) continue;
                    HizmetBilgisiSatiri hbs = new HizmetBilgisiSatiri();
                    hbs.baslangicTarihi = EkFormUclari.Tarih(s.Baslangic);
                    hbs.bitisTarihi = EkFormUclari.Tarih(s.Bitis);
                    hbs.hizmetAdi = EkFormUclari.Temiz(s.HizmetAdi);
                    hbs.aciklama = EkFormUclari.Temiz(s.Aciklama);
                    ArrayList miktarlar = new ArrayList();
                    for (int j = 1; j < 10; j++)
                    {
                        HizmetBilgiEko hbe = new HizmetBilgiEko();
                        hbe.ekoKod = j.ToString().PadLeft(2, '0');
                        hbe.miktar = s.Miktarlar != null && s.Miktarlar.Length >= j ? (s.Miktarlar[j - 1] ?? 0) : 0;
                        if (hbe.miktar != 0) miktarlar.Add(hbe);
                    }
                    hbs.miktarlar = (HizmetBilgiEko[])miktarlar.ToArray(typeof(HizmetBilgiEko));
                    // Eski kod: tarihlerin, adın ve açıklamanın hepsi boş olan satır saklanmaz.
                    if (!(hbs.baslangicTarihi.isNull && hbs.bitisTarihi.isNull && hbs.hizmetAdi == "" && hbs.aciklama == ""))
                        hbsc.Add(hbs);
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().HizmetBilgisiKaydet(kullanan, istek.Yil, istek.Kur.Trim(), istek.Tamam, istek.Yil + istek.EkYil, hbsc);
                if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return EkFormUclari.Tamam("Bilgiler başarıyla saklandı.");
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>Eski btnYazdir_Click: üç yılın formu (sayfa sayfa) tek Excel dosyasında.</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, int tamam)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            kur = kur.Trim();
            string ekBilgi = tamam == 1 ? "F7" : "F6";
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().HizmetBilgisiEkoHazirla(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), EkFormUclari.ExcelTur(),
                EkFormUclari.RaporParam(yil, kur, ""), ekBilgi), ekBilgi + "_" + kur);
        }
    }
}
