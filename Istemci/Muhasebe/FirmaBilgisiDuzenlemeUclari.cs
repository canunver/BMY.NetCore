using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.HRC;
using TNS.KYM;

namespace Istemci.Muhasebe
{
    /// <summary>Kaydet isteği: ana kayıt ve onunla birleştirilecek firma/kişilerin vergi numaraları.</summary>
    public class FirmaBirlestirIstegi
    {
        public string anaVergiNo { get; set; }
        public string anaAd { get; set; }
        public List<string> vergiNolar { get; set; }
    }

    /// <summary>
    /// Firma Bilgisi Düzenleme (eski ButceMuhasebe/FirmaBilgisiDuzenleme.aspx): aynı firma/kişinin birden çok kaydı tek
    /// ana kayıtta toplanır. Birleştirilen kayıtların ödeme emri/MİF alt hesap kodu ana kaydın vergi numarasına çevrilir,
    /// sonra o firma bilgisi silinir (MUH iş kuralları: OdemeEmriMIFAltHesapKodGuncelle, HRCFirmaBilgisiSilme).
    ///   GET  /api/firmaduzenleme/liste?ara=&amp;muhasebe=  eski FirmalariDoldur → { satirlar: [{ KOD, VERGINO, AD }] } / { hata }
    ///   POST /api/firmaduzenleme/kaydet                eski btnKaydet_Click → { basarili, bilgi } / { hata }
    /// Yetki: menüdeki seçenek; eski Page_Load'daki gibi MUHASEBEBIRIMIMUHASEBEKOD / MUHASEBEBIRIMIBIRIMKOD değişkenleri
    /// tanımlıysa o muhasebe biriminde yetki de aranır. Eski sayfa Firma/Kişi listesinden (HarcamaSurecleri/ListeFirmaKisi.aspx)
    /// açılıyordu; o sayfa da üst sayfa sayılır.
    /// </summary>
    public static class FirmaBilgisiDuzenlemeUclari
    {
        public const string SayfaAdresi = "../ButceMuhasebe/FirmaBilgisiDuzenleme.aspx";
        private static readonly string[] UstSayfalar = { "../HarcamaSurecleri/ListeFirmaKisi.aspx", "../HarcamaSurecleri/TanimFirmaKisi.aspx" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/firmaduzenleme/liste", Liste);
            app.MapPost("/api/firmaduzenleme/kaydet", Kaydet);
        }

        /// <summary>Menü yetkisi ve (tanımlıysa) muhasebe birimi yetkisi. hata: yetkisizlikte gösterilecek metin.</summary>
        public static bool Yetkili(Kullanici kullanan, string uygulamaKlasoru, out string hata)
        {
            hata = "Bu sayfayı kullanma yetkiniz yok.";
            if (kullanan == null) return false;
            if (!SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi, UstSayfalar)) return false;

            string muhasebeKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIMUHASEBEKOD") + "";
            string birimKod = TNS.MUH.Arac.DegiskenDegerBul(0, "/MUHASEBE/MUHASEBEBIRIMIBIRIMKOD") + "";
            if (!string.IsNullOrEmpty(muhasebeKod) && !string.IsNullOrEmpty(birimKod)
                && !TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, muhasebeKod, birimKod))
            {
                hata = "Bu sayfada işlem yapma yetkiniz yoktur";
                return false;
            }
            hata = null;
            return true;
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath, out string _)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string ara, string muhasebe)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            if (string.IsNullOrWhiteSpace(ara))
                return Results.Json(new { hata = "Lütfen arama alanına kriter girerek arama yapınız" });

            FirmaBilgisi b = new FirmaBilgisi();
            b.muhasebe = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0);
            b.ad = ara.Trim();

            try
            {
                ObjectArray listeFirma = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, b);
                if (listeFirma == null) return Results.Json(new { satirlar = new object[0] });
                if (listeFirma.sonuc != null && !listeFirma.sonuc.islemSonuc && !string.IsNullOrWhiteSpace(listeFirma.sonuc.hataStr))
                    return Results.Json(new { hata = listeFirma.sonuc.hataStr });

                List<object> satirlar = new List<object>();
                foreach (FirmaBilgisi f in listeFirma.objeler)
                    satirlar.Add(new { KOD = f.kod, VERGINO = f.vergiNo, AD = f.ad });
                return Results.Json(new { satirlar });
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = ex.Message });
            }
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, FirmaBirlestirIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            string anaVergiNo = (istek?.anaVergiNo ?? "").Trim();
            if (string.IsNullOrEmpty(istek?.anaAd))
                return Results.Json(new { basarili = false, hata = "Lütfen ana kayit için gerekli bilgileri giriniz" });
            if (istek.vergiNolar == null || istek.vergiNolar.Count == 0)
                return Results.Json(new { basarili = false, hata = "Lütfen kayit için gerekli bilgileri giriniz" });

            TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
            List<string> hatalar = new List<string>();
            try
            {
                foreach (string v in istek.vergiNolar)
                {
                    string vergiNo = (v ?? "").Trim();
                    if (vergiNo == anaVergiNo) continue;
                    // Eski kod: önce alt hesap kodu güncellenir, sonra firma bilgisi silinir (sonuçları sayfada gösterilmiyordu).
                    Sonuc s1 = servisMUH.OdemeEmriMIFAltHesapKodGuncelle(kullanan, anaVergiNo, vergiNo);
                    Sonuc s2 = servisMUH.HRCFirmaBilgisiSilme(kullanan, vergiNo);
                    if (s1 != null && !s1.islemSonuc) hatalar.Add(vergiNo + ": " + s1.hataStr);
                    if (s2 != null && !s2.islemSonuc) hatalar.Add(vergiNo + ": " + s2.hataStr);
                }
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = ex.Message });
            }

            if (hatalar.Count > 0)
                return Results.Json(new { basarili = false, hata = "Kayıt işleminde hata oluştu:\n" + string.Join("\n", hatalar) });
            return Results.Json(new { basarili = true, bilgi = "Kayit işlemi tamamlandı" });
        }
    }
}
