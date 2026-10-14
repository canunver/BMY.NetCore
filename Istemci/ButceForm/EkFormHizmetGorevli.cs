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
    public class HizmetGorevliSatirIstegi
    {
        public int SiraNo { get; set; }
        /// <summary>
        /// 24 değer, eski tablonun 1..24. sütunları: önce (yıl-2) sonra (yıl-1) için sırasıyla temizlik, güvenlik, şoför, diğer
        /// görevliler; her birinde adet, süre, maliyet.
        /// </summary>
        public double?[] Degerler { get; set; }
    }

    public class HizmetGorevliIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public List<HizmetGorevliSatirIstegi> Satirlar { get; set; }
    }

    /// <summary>
    /// Form 28 (hizmet alımı suretiyle çalıştırılan görevli sayısına ilişkin bilgi formu), eski ButceForm/HizmetGorevli.aspx:
    ///   GET  /api/ekform/hizmetgorevli/liste?yil=&amp;kur=    (eski GorevliListele -> HizmetAlimiGorevliListele)
    ///   POST /api/ekform/hizmetgorevli/kaydet              (eski btnKaydet_Click -> HizmetAlimiGorevliKaydet)
    ///   GET  /api/ekform/hizmetgorevli/yazdir?yil=&amp;kur=   (eski btnYazdir_Click -> HizmetAlimiGorevliHazirla)
    /// </summary>
    public static class EkFormHizmetGorevli
    {
        /// <summary>Eski tablodaki satır sayısı (sıra no 1..15).</summary>
        private const int SatirSayisi = 15;

        /// <summary>Eski FRMHGO038.</summary>
        private const string SecimHatasi = "Yıl veya Kurum alanlarından biri veya bir kaçı boş bırakılmış. Lütfen ilgili alanları kontrol edip tekrar deneyin.";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekform/hizmetgorevli/liste", Liste);
            app.MapPost("/api/ekform/hizmetgorevli/kaydet", Kaydet);
            app.MapGet("/api/ekform/hizmetgorevli/yazdir", Yazdir);
        }

        /// <summary>Eski GorevliListele: her kayıt sıra numarasının satırına (SiraNo - 1) yazılır.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(SecimHatasi);
            try
            {
                ObjectArray hat = TNS.BHF.Arac.Tanimla().HizmetAlimiGorevliListele(kullanan, yil, kur.Trim());
                if (!hat.sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(hat.sonuc.hataStr, "Görevli bilgileri okunamadı."));
                List<object> satirlar = new List<object>();
                for (int i = 0; i < hat.ObjeSayisi; i++)
                {
                    HizmetAlimiGorevli h = (HizmetAlimiGorevli)hat[i];
                    int yer = OrtakFonksiyonlar.ConvertToInt(h.SiraNo, 0) - 1;
                    // Eski kod 15'ten büyük sıra numarasında tablo dışına yazmaya çalışıyordu; burada atlanır.
                    if (yer < 0 || yer >= SatirSayisi) continue;
                    satirlar.Add(new
                    {
                        siraNo = yer + 1,
                        degerler = new double[]
                        {
                            h.ozelGorevliTemizlikAdet2, h.ozelGorevliTemizlikSure2, h.ozelGorevliTemizlikMaliyet2,
                            h.ozelGorevliGuvenlikAdet2, h.ozelGorevliGuvenlikSure2, h.ozelGorevliGuvenlikMaliyet2,
                            h.ozelGorevliSoforAdet2, h.ozelGorevliSoforSure2, h.ozelGorevliSoforMaliyet2,
                            h.ozelGorevliDigerAdet2, h.ozelGorevliDigerSure2, h.ozelGorevliDigerMaliyet2,
                            h.ozelGorevliTemizlikAdet1, h.ozelGorevliTemizlikSure1, h.ozelGorevliTemizlikMaliyet1,
                            h.ozelGorevliGuvenlikAdet1, h.ozelGorevliGuvenlikSure1, h.ozelGorevliGuvenlikMaliyet1,
                            h.ozelGorevliSoforAdet1, h.ozelGorevliSoforSure1, h.ozelGorevliSoforMaliyet1,
                            h.ozelGorevliDigerAdet1, h.ozelGorevliDigerSure1, h.ozelGorevliDigerMaliyet1
                        }
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Görevli bilgileri okunamadı."));
            }
        }

        /// <summary>Eski OrtakFonksiyonlar.ConvertToInt(hücre değeri, 0).</summary>
        private static int Tam(double?[] d, int i)
        {
            if (d == null || d.Length <= i || d[i] == null) return 0;
            try { return Convert.ToInt32(d[i].Value); } catch (Exception) { return 0; }
        }

        private static double Ondalik(double?[] d, int i)
        {
            return d == null || d.Length <= i ? 0 : (d[i] ?? 0);
        }

        /// <summary>Eski btnKaydet_Click: 15 satırın her biri okunur, yalnız dolu olanlar (HizmetAlimiGorevli.DoluMu) gönderilir.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, HizmetGorevliIstegi istek)
        {
            var (ret, kullanan) = await EkFormUclari.DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            istek ??= new HizmetGorevliIstegi();
            if (!EkFormUclari.SecimGecerli(istek.Yil, istek.Kur)) return EkFormUclari.Hata(SecimHatasi);
            try
            {
                TNSCollection ec = new TNSCollection();
                foreach (HizmetGorevliSatirIstegi s in istek.Satirlar ?? new List<HizmetGorevliSatirIstegi>())
                {
                    if (s == null || s.SiraNo < 1 || s.SiraNo > SatirSayisi) continue;
                    double?[] d = s.Degerler;
                    HizmetAlimiGorevli h = new HizmetAlimiGorevli();
                    h.SiraNo = s.SiraNo.ToString();
                    h.ozelGorevliTemizlikAdet2 = Tam(d, 0);
                    h.ozelGorevliTemizlikSure2 = Tam(d, 1);
                    h.ozelGorevliTemizlikMaliyet2 = Ondalik(d, 2);
                    h.ozelGorevliGuvenlikAdet2 = Tam(d, 3);
                    h.ozelGorevliGuvenlikSure2 = Tam(d, 4);
                    h.ozelGorevliGuvenlikMaliyet2 = Ondalik(d, 5);
                    h.ozelGorevliSoforAdet2 = Tam(d, 6);
                    h.ozelGorevliSoforSure2 = Tam(d, 7);
                    h.ozelGorevliSoforMaliyet2 = Ondalik(d, 8);
                    h.ozelGorevliDigerAdet2 = Tam(d, 9);
                    h.ozelGorevliDigerSure2 = Tam(d, 10);
                    h.ozelGorevliDigerMaliyet2 = Ondalik(d, 11);

                    h.ozelGorevliTemizlikAdet1 = Tam(d, 12);
                    h.ozelGorevliTemizlikSure1 = Tam(d, 13);
                    h.ozelGorevliTemizlikMaliyet1 = Ondalik(d, 14);
                    h.ozelGorevliGuvenlikAdet1 = Tam(d, 15);
                    h.ozelGorevliGuvenlikSure1 = Tam(d, 16);
                    h.ozelGorevliGuvenlikMaliyet1 = Ondalik(d, 17);
                    h.ozelGorevliSoforAdet1 = Tam(d, 18);
                    h.ozelGorevliSoforSure1 = Tam(d, 19);
                    h.ozelGorevliSoforMaliyet1 = Ondalik(d, 20);
                    h.ozelGorevliDigerAdet1 = Tam(d, 21);
                    h.ozelGorevliDigerSure1 = Tam(d, 22);
                    h.ozelGorevliDigerMaliyet1 = Ondalik(d, 23);

                    if (h.DoluMu()) ec.Add(h);
                }
                Sonuc sonuc = TNS.BHF.Arac.Tanimla().HizmetAlimiGorevliKaydet(kullanan, istek.Yil, istek.Kur.Trim(), ec);
                if (!sonuc.islemSonuc) return EkFormUclari.Hata(EkFormUclari.HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return EkFormUclari.Tamam("Bilgiler başarıyla saklandı.");
            }
            catch (Exception ex)
            {
                return EkFormUclari.Hata(EkFormUclari.HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>
        /// Eski btnYazdir_Click: HizmetAlimiGorevliHazirla (ButceYil, KurKod; çıktı türü "Excel"), dosya adı
        /// "form" + BHFFormlar.FormNoBul(yil, "HizmetAlimiGorevli") (eski kodda olduğu gibi birim kodu eklenmez).
        /// </summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur)
        {
            IResult ret = EkFormUclari.Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!EkFormUclari.SecimGecerli(yil, kur)) return EkFormUclari.Hata(SecimHatasi);
            kur = kur.Trim();
            return EkFormUclari.RaporGonder(() => new global::ButceForm.ButceFormRaporlari().HizmetAlimiGorevliHazirla(
                kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), "Excel", EkFormUclari.RaporParam(yil, kur, ""), ""),
                "form" + global::ButceForm.BHFFormlar.FormNoBul(yil, "HizmetAlimiGorevli"));
        }
    }
}
