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
using TNS.BKD;
using TNS.KYM;
using TNS.UZY;

namespace Istemci.ButceForm
{
    public class UluslararasiSatirIstegi
    {
        public double? SiraNo { get; set; }
        public string KurulusAd { get; set; }
        public string YasalDayanak { get; set; }
        public double? ToplantiSayisi { get; set; }
        public double? ToplantiKatsayisi { get; set; }
        public string DovizCinsi { get; set; }
        public double? OncekiyilDoviz { get; set; }
        public double? OncekiyilAidat { get; set; }
        public double? HazirlikYilDoviz { get; set; }
        public double? HazirlikYilAidat { get; set; }
        public double? ErtesiYilDoviz { get; set; }
        public double? ErtesiYilAidat { get; set; }
        public double? ErtesiYil2Doviz { get; set; }
        public double? ErtesiYil2Aidat { get; set; }
        public string Aciklama { get; set; }
    }

    public class UluslararasiIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public List<UluslararasiSatirIstegi> Satirlar { get; set; }
    }

    /// <summary>
    /// Form 17: uluslararası kuruluşlar bilgi formu (eski ButceForm/UluslararasiKuruluslar.aspx):
    ///   GET  /api/ekform/uluslararasi/liste?yil=&amp;kur=&amp;senaryo=     (eski btnListele_Click: 07.2.2 / 05.6.2 tertip toplamları
    ///                                                              (UzayDegeriDbl) + UluslarArasiKuruluslarListele)
    ///   POST /api/ekform/uluslararasi/kaydet                         (eski btnKaydet_Click -> UluslarArasiKuruluslarKaydet)
    ///   GET  /api/ekform/uluslararasi/yazdir?yil=&amp;kur=&amp;senaryo=    (eski btnYazdir_Click -> UluslararasiKuruluslarHazirla)
    /// </summary>
    public static class EkFormUluslararasi
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/uluslararasi/liste", Liste);
            app.MapPost("/api/ekform/uluslararasi/kaydet", Kaydet);
            app.MapGet("/api/ekform/uluslararasi/yazdir", Yazdir);
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string senaryo)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            kur = kur.Trim();
            // Eski GenelIslemlerIstemci.RequestBul(this, "senaryo", "00.00")
            string senaryoKod = EkFormUclari.Temiz(senaryo) == "" ? "00.00" : senaryo.Trim();
            try
            {
                // Eski kod: ekonomik kod 4. düzeyi noktasız 8 haneli değilse 0722,0562; 8 haneliyse 070220,050620.
                Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                string ekoKod = kistasTertip.KodUzunlukBul(3, 4, true) != 8 ? "0722,0562" : "070220,050620";

                IUZYServis uzy = TNS.UZY.Arac.Tanimla();
                var toplamlar = new
                {
                    oncYilOdenek = uzy.UzayDegeriDbl(kullanan, "ABSODENEK", (yil - 2).ToString() + "-06.00-0-" + kur + "---" + ekoKod, false),
                    oncYilHarcama = uzy.UzayDegeriDbl(kullanan, "ABSHARCAMA", (yil - 2).ToString() + "-12-" + kur + "---" + ekoKod, false),
                    yilOdenek = uzy.UzayDegeriDbl(kullanan, "ABSODENEK", (yil - 1).ToString() + "-06.00-0-" + kur + "---" + ekoKod, false),
                    yilHazHarcama = uzy.UzayDegeriDbl(kullanan, "ABSHARCAMA", (yil - 1).ToString() + "-6-" + kur + "---" + ekoKod, false),
                    yilSHTah = uzy.UzayDegeriDbl(kullanan, "ABSHARCAMATAHMIN", (yil - 1).ToString() + "-" + senaryoKod + "-" + kur + "---" + ekoKod, false),
                    yil1Teklif = uzy.UzayDegeriDbl(kullanan, "ABSODENEK", yil.ToString() + "-" + senaryoKod + "-0-" + kur + "---" + ekoKod, false),
                    yil2Teklif = uzy.UzayDegeriDbl(kullanan, "ABSODENEK", yil.ToString() + "-" + senaryoKod + "-1-" + kur + "---" + ekoKod, false),
                    yil3Teklif = uzy.UzayDegeriDbl(kullanan, "ABSODENEK", yil.ToString() + "-" + senaryoKod + "-2-" + kur + "---" + ekoKod, false)
                };

                // Eski kod: liste okunamazsa ileti gösterilir, tablo yine boş giriş satırlarıyla açılır.
                List<object> satirlar = new List<object>();
                string uyari = null;
                try
                {
                    UluslarArasiKurulus[] liste = TNS.BHF.Arac.Tanimla().UluslarArasiKuruluslarListele(kullanan, yil, kur);
                    foreach (UluslarArasiKurulus uk in liste ?? new UluslarArasiKurulus[0])
                    {
                        satirlar.Add(new
                        {
                            siraNo = uk.siraNo,
                            kurulusAd = uk.kurulusAd ?? "",
                            yasalDayanak = uk.yasalDayanak ?? "",
                            toplantiSayisi = uk.toplantiSayisi,
                            toplantiKatsayisi = uk.toplantiKatsayisi,
                            dovizCinsi = uk.dovizCinsi ?? "",
                            oncekiyilDoviz = uk.oncekiyilDoviz,
                            oncekiyilAidat = uk.oncekiyilAidat,
                            hazirlikYilDoviz = uk.hazirlikYilDoviz,
                            hazirlikYilAidat = uk.hazirlikYilAidat,
                            ertesiYilDoviz = uk.ertesiYilDoviz,
                            ertesiYilAidat = uk.ertesiYilAidat,
                            ertesiYil2Doviz = uk.ertesiYil2Doviz,
                            ertesiYil2Aidat = uk.ertesiYil2Aidat,
                            aciklama = uk.aciklama ?? ""
                        });
                    }
                }
                catch (Exception ex)
                {
                    uyari = EkFormUclari.HataMetni(ex.Message, "Uluslararası kuruluş bilgileri okunamadı.");
                }
                return Results.Json(new { toplamlar = toplamlar, satirlar = satirlar, uyari = uyari });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Uluslararası kuruluş bilgileri okunamadı."));
            }
        }

        /// <summary>Eski OrtakFonksiyonlar.ConvertToInt(hücre.Value, 0): boş 0, ondalık yuvarlanır.</summary>
        private static int Tam(double? d) { return d.HasValue ? OrtakFonksiyonlar.ConvertToInt((object)d.Value, 0) : 0; }

        /// <summary>
        /// Eski btnKaydet_Click: tüm sayısal hücreler tam sayıya çevrilir (ConvertToInt); sıra numarası 0 (boş) olmayan satırlar
        /// UluslarArasiKuruluslarKaydet'e gönderilir.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, UluslararasiIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new UluslararasiIstegi();
            if (!EkFormUclari.SecimGecerli(istek.Yil, istek.Kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            try
            {
                TNSCollection ukv = new TNSCollection();
                foreach (UluslararasiSatirIstegi s in istek.Satirlar ?? new List<UluslararasiSatirIstegi>())
                {
                    if (s == null) continue;
                    UluslarArasiKurulus uk = new UluslarArasiKurulus();
                    uk.siraNo = Tam(s.SiraNo);
                    uk.kurulusAd = s.KurulusAd ?? "";
                    uk.yasalDayanak = s.YasalDayanak ?? "";
                    uk.toplantiSayisi = Tam(s.ToplantiSayisi);
                    uk.toplantiKatsayisi = Tam(s.ToplantiKatsayisi);
                    uk.dovizCinsi = s.DovizCinsi ?? "";
                    uk.oncekiyilDoviz = Tam(s.OncekiyilDoviz);
                    uk.oncekiyilAidat = Tam(s.OncekiyilAidat);
                    uk.hazirlikYilDoviz = Tam(s.HazirlikYilDoviz);
                    uk.hazirlikYilAidat = Tam(s.HazirlikYilAidat);
                    uk.ertesiYilDoviz = Tam(s.ErtesiYilDoviz);
                    uk.ertesiYilAidat = Tam(s.ErtesiYilAidat);
                    uk.ertesiYil2Doviz = Tam(s.ErtesiYil2Doviz);
                    uk.ertesiYil2Aidat = Tam(s.ErtesiYil2Aidat);
                    uk.aciklama = s.Aciklama ?? "";
                    if (uk.siraNo != 0) ukv.Add(uk);
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().UluslarArasiKuruluslarKaydet(kullanan, istek.Yil, istek.Kur.Trim(), ukv);
                if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return EkFormUclari.Tamam("Bilgiler başarıyla saklandı.");   // eski FRMUKU013
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>
        /// Eski btnYazdir_Click: UluslararasiKuruluslarHazirla (ciktiYeri null; parametrelerde Senaryo ve Asama = senaryo);
        /// dosya adı "form17_{kur}" (FormNoBul "UluslararasiKuruluslar").
        /// </summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string senaryo)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            kur = kur.Trim();
            Hashtable ht = EkFormUclari.RaporParam(yil, kur, senaryo);
            ht["Asama"] = EkFormUclari.Temiz(senaryo);
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().UluslararasiKuruluslarHazirla(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), null, ht, ""),
                "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "UluslararasiKuruluslar") + "_" + kur + "." + EkFormUclari.ExcelTur());
        }
    }
}
