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
    public class HizmetAlimiTasitSatirIstegi
    {
        public string SiraNo { get; set; }
        /// <summary>
        /// 12 değer: önce Bütçe (j = 0), sonra Döner Sermaye + Diğer (j = 1); her birinde şoförlü yakıt gider hariç
        /// adet, süre, aylık kira bedeli ve şoförsüz yakıt gider hariç adet, süre, aylık kira bedeli (eski tablonun 1..6 / 13..18. sütunları).
        /// </summary>
        public double?[] Degerler { get; set; }
    }

    public class HizmetAlimiTasitIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public int Kullanilacak { get; set; }
        public List<HizmetAlimiTasitSatirIstegi> Satirlar { get; set; }
    }

    /// <summary>
    /// Form 19.3 (hizmet alımı suretiyle kullanılan taşıtlar, kullanilacak = 0) ve Form 19.2 (kullanılacak taşıtlar, kullanilacak = 1);
    /// eski ButceForm/HizmetAlimiTasitlariDetay.aspx?Kullanilacak=..:
    ///   GET  /api/ekform/hizmetalimitasit/liste?yil=&amp;kur=&amp;kullanilacak=        (eski btnListele_Click -> TasitListele(false): TasitSozlukListele, HizmetAlimiTasitlariDetayListele)
    ///   GET  /api/ekform/hizmetalimitasit/sozlesmeler?yil=&amp;kur=&amp;kullanilacak=  (eski btnDetaydanAl / btnKiraliktanAl_Click -> TasitListele(true): KiralikAracRaporla)
    ///   POST /api/ekform/hizmetalimitasit/kaydet                                 (eski btnKaydet_Click -> HizmetAlimiTasitlariDetayKaydet)
    ///   GET  /api/ekform/hizmetalimitasit/yazdir?yil=&amp;kur=&amp;kullanilacak=       (eski btnYazdir_Click -> HizmetAlimiTasitlariDetayHazirla)
    /// Tablo eski FarPoint tablosundaki gibi yılın taşıt sözlüğünün her satırı için bir satırdır; yalnızca "yakıt gider hariç"
    /// sütunları kullanılır (eski tabloda "dahil" sütunları gizliydi, okunup yazılmıyordu).
    /// </summary>
    public static class EkFormHizmetAlimiTasit
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/hizmetalimitasit/liste", Liste);
            app.MapGet("/api/ekform/hizmetalimitasit/sozlesmeler", Sozlesmeler);
            app.MapPost("/api/ekform/hizmetalimitasit/kaydet", Kaydet);
            app.MapGet("/api/ekform/hizmetalimitasit/yazdir", Yazdir);
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, int kullanilacak)
        {
            return Oku(baglam, ortam, yil, kur, kullanilacak, false);
        }

        /// <summary>Eski "Detaydan Al" (btnKiraliktanAl_Click): kiralık araç sözleşmelerinden toplanan bilgiler tabloya alınır, saklanmaz.</summary>
        private static IResult Sozlesmeler(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, int kullanilacak)
        {
            return Oku(baglam, ortam, yil, kur, kullanilacak, true);
        }

        /// <summary>
        /// Eski GridInit + TasitListele(sozlesmelerden): satırlar yılın taşıt sözlüğüdür; okunan her kayıt sıra numarası tutan
        /// (eski YerBul: etiketi "sıraNo/" ile başlayan ilk) satıra yazılır, sözlükte olmayan kayıt gösterilmez.
        /// Sözleşmelerden alınırken kullanılan taşıtlar formu (kullanilacak = 0) bir önceki yılın sözleşmelerini okur.
        /// </summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, int kullanilacak, bool sozlesmelerden)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            string varsayilan = sozlesmelerden ? "Sözleşme bilgileri okunamadı." : "Hizmet alımı taşıt bilgileri okunamadı.";
            try
            {
                IBHFServis servis = TNS.BHF.Arac.Tanimla();
                TasitSozluk[] sozluk = servis.TasitSozlukListele(yil) ?? new TasitSozluk[0];

                ObjectArray hat = sozlesmelerden
                    ? servis.KiralikAracRaporla(kullanan, kullanilacak == 0 ? yil - 1 : yil, 0, "")
                    : servis.HizmetAlimiTasitlariDetayListele(kullanan, yil, kur.Trim(), kullanilacak == 0);
                if (!hat.sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(hat.sonuc.hataStr, varsayilan));

                double?[][] degerler = new double?[sozluk.Length][];
                for (int i = 0; i < hat.ObjeSayisi; i++)
                {
                    HizmetAlimiTasitBilgisiDetay hatb = (HizmetAlimiTasitBilgisiDetay)hat[i];
                    int yer = Array.FindIndex(sozluk, ts => (ts.siraNo ?? "") == (hatb.tasitSozluk.siraNo ?? ""));
                    if (yer < 0) continue;
                    double?[] d = new double?[12];
                    for (int j = 0; j < 2; j++)
                    {
                        int b = j * 6;
                        d[b + 0] = hatb.soforluYakitGiderHaric[j].adet;
                        d[b + 1] = hatb.soforluYakitGiderHaric[j].sure;
                        d[b + 2] = hatb.soforluYakitGiderHaric[j].maliyet;
                        d[b + 3] = hatb.soforsuzYakitGiderHaric[j].adet;
                        d[b + 4] = hatb.soforsuzYakitGiderHaric[j].sure;
                        d[b + 5] = hatb.soforsuzYakitGiderHaric[j].maliyet;
                    }
                    degerler[yer] = d;
                }

                List<object> satirlar = new List<object>();
                for (int i = 0; i < sozluk.Length; i++)
                {
                    TasitSozluk ts = sozluk[i];
                    satirlar.Add(new
                    {
                        siraNo = ts.siraNo ?? "",
                        tasit = ts.siraNo + "/" + ts.cinsi + "-" + ts.diffarensiyel,
                        degerler = degerler[i] ?? new double?[12]
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, varsayilan));
            }
        }

        /// <summary>
        /// Eski btnKaydet_Click: adet ve süre tam sayıya çevrilir (eski ConvertToInt), yalnızca dolu (DoluMu: herhangi bir adet ya da
        /// maliyeti sıfırdan büyük) satırlar saklanır.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, HizmetAlimiTasitIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new HizmetAlimiTasitIstegi();
            if (!EkFormUclari.SecimGecerli(istek.Yil, istek.Kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            try
            {
                TNSCollection ec = new TNSCollection();
                foreach (HizmetAlimiTasitSatirIstegi s in istek.Satirlar ?? new List<HizmetAlimiTasitSatirIstegi>())
                {
                    if (s == null || EkFormUclari.Temiz(s.SiraNo) == "") continue;
                    HizmetAlimiTasitBilgisiDetay hatb = new HizmetAlimiTasitBilgisiDetay();
                    hatb.tasitSozluk.siraNo = s.SiraNo;
                    for (int j = 0; j < 2; j++)
                    {
                        int b = j * 6;
                        hatb.soforluYakitGiderHaric[j].adet = Tam(s.Degerler, b + 0);
                        hatb.soforluYakitGiderHaric[j].sure = Tam(s.Degerler, b + 1);
                        hatb.soforluYakitGiderHaric[j].maliyet = Deger(s.Degerler, b + 2);

                        hatb.soforsuzYakitGiderHaric[j].adet = Tam(s.Degerler, b + 3);
                        hatb.soforsuzYakitGiderHaric[j].sure = Tam(s.Degerler, b + 4);
                        hatb.soforsuzYakitGiderHaric[j].maliyet = Deger(s.Degerler, b + 5);
                    }
                    if (hatb.DoluMu())
                        ec.Add(hatb);
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().HizmetAlimiTasitlariDetayKaydet(kullanan, istek.Yil, istek.Kur.Trim(), ec, istek.Kullanilacak == 0);
                if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return EkFormUclari.Tamam("Bilgiler başarıyla saklandı.");
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        private static double Deger(double?[] d, int i) { return d != null && i < d.Length ? (d[i] ?? 0) : 0; }

        /// <summary>Eski OrtakFonksiyonlar.ConvertToInt(hücre, 0): Convert.ToInt32 ile yuvarlanır.</summary>
        private static int Tam(double?[] d, int i) { return OrtakFonksiyonlar.ConvertToInt((object)Deger(d, i), 0); }

        /// <summary>Eski btnYazdir_Click: raporParam'a "Kullanilacak" eklenir; dosya adı "form20_1" (kullanılan) / "form20_2" (kullanılacak).</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, int kullanilacak)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(EkFormUclari.SecimHatasi);
            kur = kur.Trim();
            System.Collections.Hashtable raporParam = EkFormUclari.RaporParam(yil, kur, "");
            raporParam.Add("Kullanilacak", kullanilacak.ToString());
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().HizmetAlimiTasitlariDetayHazirla(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), EkFormUclari.ExcelTur(), raporParam, ""),
                "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "HizmetAlimiTasitBilgi") + "_" + (kullanilacak + 1).ToString());
        }
    }
}
