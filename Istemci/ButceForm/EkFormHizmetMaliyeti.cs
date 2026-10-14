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
    public class HizmetMaliyetiSatirIstegi
    {
        public string Kod { get; set; }
        public string Aciklama { get; set; }
        public bool DegerGirilsinMi { get; set; }
        public double? Gercek { get; set; }
        public double? Haziran { get; set; }
        public double? Tahmin { get; set; }
        public double? Tahmin1 { get; set; }
        public double? Tahmin2 { get; set; }
    }

    public class HizmetMaliyetiIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public List<HizmetMaliyetiSatirIstegi> Satirlar { get; set; }
    }

    /// <summary>
    /// Form 10: birimlerin hizmet maliyetinin tespitine ilişkin bilgi formu (eski ButceForm/HizmetMaliyetiTespiti.aspx):
    ///   GET  /api/ekform/hizmetmaliyeti/liste?yil=&amp;kur=        (eski HizmetMaliyetiListele -> HizmetMaliyetiTesbitiListele)
    ///   GET  /api/ekform/hizmetmaliyeti/detaydanal?yil=&amp;kur=   (eski btnHizmetMaliyetAl_Click -> HizmetMaliyetiTasraListele; kaydetmez)
    ///   POST /api/ekform/hizmetmaliyeti/kaydet                  (eski btnKaydet_Click -> HizmetMaliyetiTesbitiKaydet)
    ///   GET  /api/ekform/hizmetmaliyeti/yazdir?yil=&amp;kur=       (eski btnYazdir_Click -> HizmetMaliyetiTespitiHazirla)
    /// </summary>
    public static class EkFormHizmetMaliyeti
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/hizmetmaliyeti/liste", Liste);
            app.MapGet("/api/ekform/hizmetmaliyeti/detaydanal", DetaydanAl);
            app.MapPost("/api/ekform/hizmetmaliyeti/kaydet", Kaydet);
            app.MapGet("/api/ekform/hizmetmaliyeti/yazdir", Yazdir);
        }

        /// <summary>Eski HizmetMaliyetiListele: HizmetMaliyetiTesbitiListele(kullanan, yil, kurkod).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            try
            {
                ObjectArray hmv = TNS.BHF.Arac.Tanimla().HizmetMaliyetiTesbitiListele(kullanan, yil, kur.Trim());
                if (!hmv.sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(hmv.sonuc.hataStr, "Hizmet maliyeti bilgileri okunamadı."));
                return Results.Json(new { satirlar = Satirlar(hmv) });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Hizmet maliyeti bilgileri okunamadı."));
            }
        }

        /// <summary>
        /// Eski btnHizmetMaliyetAl_Click ("Detaydan Al"): HizmetMaliyetiTasraListele(kullanan, yil, 0, "", kurkod, true) ile
        /// taşra detayından toplanan satırları döndürür; eski kodda olduğu gibi kaydetmez (kullanıcı Kaydet'e basmalıdır).
        /// </summary>
        private static IResult DetaydanAl(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            try
            {
                ObjectArray hmv = TNS.BHF.Arac.Tanimla().HizmetMaliyetiTasraListele(kullanan, yil, 0, "", kur.Trim(), true);
                if (!hmv.sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(hmv.sonuc.hataStr, "Detay bilgileri okunamadı."));
                return Results.Json(new { satirlar = Satirlar(hmv) });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Detay bilgileri okunamadı."));
            }
        }

        /// <summary>Eski stoGrid alanları: kod, aciklama, gercek, haziran, tahmin, tahmin1, tahmin2, degerGirilsinMi.</summary>
        private static List<object> Satirlar(ObjectArray hmv)
        {
            List<object> satirlar = new List<object>();
            if (hmv.objeler == null) return satirlar;
            foreach (object o in hmv.objeler)
            {
                if (!(o is HizmetMaliyetiTesbitiSatir hms)) continue;
                satirlar.Add(new
                {
                    kod = hms.kod ?? "",
                    aciklama = hms.aciklama ?? "",
                    degerGirilsinMi = hms.degerGirilsinMi,
                    gercek = hms.gercek,
                    haziran = hms.haziran,
                    tahmin = hms.tahmin,
                    tahmin1 = hms.tahmin1,
                    tahmin2 = hms.tahmin2
                });
            }
            return satirlar;
        }

        /// <summary>
        /// Eski btnKaydet_Click: yalnızca değer girilen (degerGirilsinMi), açıklaması boş olmayan ve beş tutarının toplamı
        /// sıfırdan büyük satırlar HizmetMaliyetiTesbitiKaydet'e gönderilir.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, HizmetMaliyetiIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new HizmetMaliyetiIstegi();
            if (!EkFormUclari.SecimGecerli(istek.Yil, istek.Kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            try
            {
                TNSCollection hmc = new TNSCollection();
                foreach (HizmetMaliyetiSatirIstegi s in istek.Satirlar ?? new List<HizmetMaliyetiSatirIstegi>())
                {
                    if (s == null) continue;
                    HizmetMaliyetiTesbitiSatir hms = new HizmetMaliyetiTesbitiSatir();
                    hms.kod = s.Kod ?? "";
                    hms.aciklama = s.Aciklama ?? "";
                    hms.degerGirilsinMi = s.DegerGirilsinMi;
                    hms.gercek = s.Gercek ?? 0;
                    hms.haziran = s.Haziran ?? 0;
                    hms.tahmin = s.Tahmin ?? 0;
                    hms.tahmin1 = s.Tahmin1 ?? 0;
                    hms.tahmin2 = s.Tahmin2 ?? 0;
                    if (hms.degerGirilsinMi)
                    {
                        if (hms.aciklama != "" && (hms.gercek + hms.haziran + hms.tahmin + hms.tahmin1 + hms.tahmin2 > 0))
                            hmc.Add(hms);
                    }
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().HizmetMaliyetiTesbitiKaydet(kullanan, istek.Yil, istek.Kur.Trim(), hmc);
                if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return EkFormUclari.Tamam("Bilgiler başarıyla saklandı.");   // eski FRMHMT006
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>Eski btnYazdir_Click: HizmetMaliyetiTespitiHazirla; dosya adı "form10_{kur}" (FormNoBul "BirimHizmetMaliyeti").</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            kur = kur.Trim();
            // Eski raporParam yalnızca ButceYil ve KurKod içeriyordu; boş Senaryo raporu etkilemez.
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().HizmetMaliyetiTespitiHazirla(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), EkFormUclari.ExcelTur(),
                EkFormUclari.RaporParam(yil, kur, ""), ""),
                "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "BirimHizmetMaliyeti") + "_" + kur + "." + EkFormUclari.ExcelTur());
        }
    }
}
