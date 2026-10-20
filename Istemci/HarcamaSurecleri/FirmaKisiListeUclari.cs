using System;
using System.Collections.Generic;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.HRC;
using TNS.KYM;

namespace Istemci.HarcamaSurecleri
{
    /// <summary>
    /// Firma / Kişi Listesi (eski HarcamaSurecleri/ListeFirmaKisi.aspx) uçları:
    ///   GET /api/firmakisi/liste?muhasebe=&amp;ad=&amp;vergiNo=&amp;per=   eski btnBul_Click (IHRCServis.FirmaBilgisiListele, aktifDurum=1;
    ///                                                            per verilirse personelde de aranır) → { satirlar } / { hata } / { bilgi }
    ///   GET /api/firmakisi/kazanan?yil=&amp;surecNo=                   eski KazananFirmaYukle (IHRCServis.TeklifKazananFirmaListele)
    ///                                                            → { satirlar: [{ kod, ad }] } / { hata }
    /// İş kuralları değiştirilmeden çağrılır. Yetki: menüdeki seçenek (Harcama Yönetimi ve Bütçe Muhasebe).
    /// </summary>
    public static class FirmaKisiListeUclari
    {
        public const string SayfaAdresi = "../HarcamaSurecleri/ListeFirmaKisi.aspx";

        /// <summary>Menüde olmadığı zamanlar için: eskiden bu pencereyi açan ekranlar.</summary>
        private static readonly string[] UstSayfalar =
        {
            "../ButceMuhasebe/Makbuz.aspx",
            "../ButceMuhasebe/Kapatma.aspx",
            "../ButceMuhasebe/OdemeEmriMIFFrame.aspx",
            "../ButceForm/KiralikAraclarForm.aspx",
            "../AlacakTakip/AlacakTakipListe.aspx",
            "../AlacakTakip/AlacakTakipListeAB.aspx",
            "../ButceMuhasebe/FirmaBilgisiDuzenleme.aspx",
            "../ButceMuhasebe/TaahhutKarti.aspx",
            "../ButceMuhasebe/TaahhutKarti.aspx?tur=2"
        };

        private const string Uc = "/api/firmakisi/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "kazanan", Kazanan);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi, UstSayfalar);
        }

        /// <summary>Kullanıcının saklı muhasebe birimi (menüden açılınca varsayılan; eski sayfada adresten gelirdi).</summary>
        public static int VarsayilanMuhasebe(Kullanici kullanan)
        {
            try { return OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBE"), 0); }
            catch (Exception) { return 0; }
        }

        /// <summary>Eski Page_Load: /MERKEZBANKASI değişkeni tanımlıysa Firma Birleştir düğmesi gizlenir.</summary>
        public static bool BirlestirGoster()
        {
            try { return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBul(0, "/MERKEZBANKASI"), 0) <= 0; }
            catch (Exception) { return true; }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        // Eski TemizYap: tırnak ve satır sonları atılır.
        private static string TemizYap(string s)
        {
            return (s ?? "").Replace("\"", "").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string muhasebe, string ad, string vergiNo, string per)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            if (Temiz(muhasebe) == "")
                return Results.Json(new { hata = "Firma bilgisinin kayıtlı olduğu Muhasebe kodu bilgisi verilmemiş. Listeleme yapılamıyor." });
            if (Temiz(ad) == "" && Temiz(vergiNo) == "")
                return Results.Json(new { hata = "Lütfen Ad Soyad/Ünvan veya TC Kimlik/Vergi No alanlarında birine kriter yazın." });

            FirmaBilgisi fSorgu = new FirmaBilgisi();
            fSorgu.muhasebe = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0);
            fSorgu.birim = "";
            fSorgu.ad = Temiz(ad);
            fSorgu.vergiNo = Temiz(vergiNo);
            fSorgu.aktifDurum = 1;
            if (!string.IsNullOrWhiteSpace(per))
                fSorgu.personeldeDeAra = true;

            try
            {
                ObjectArray bilgi = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, fSorgu);
                if (bilgi == null || bilgi.sonuc == null || !bilgi.sonuc.islemSonuc)
                    return Results.Json(new { satirlar = new object[0], bilgi = "Verilen kriterlere uygun firma/kişi bilgisi bulunamadı." });

                List<object> satirlar = new List<object>();
                foreach (FirmaBilgisi f in bilgi.objeler)
                {
                    satirlar.Add(new
                    {
                        kod = f.kod,
                        ad = TemizYap(f.ad),
                        vergiNo = TemizYap(f.vergiNo),
                        vergiDairesi = TemizYap(f.vergiDairesi),
                        banka = TemizYap(f.banka),
                        hesapNo = TemizYap(f.hesapNo),
                        muhasebe = f.muhasebe,
                        birim = f.birim ?? "",
                        firmaYetkilisi = TemizYap(f.firmaYetkilisi),
                        eMail = TemizYap(f.eMail),
                        telefon = TemizYap(f.telefon),
                        faks = TemizYap(f.faks),
                        adres = TemizYap(f.adres),
                        uyruk = f.uyruk,
                        karaListe = f.karaListe
                    });
                }
                if (satirlar.Count == 0)
                    return Results.Json(new { satirlar, bilgi = "Verilen kriterlere uygun firma/kişi bilgisi bulunamadı." });
                return Results.Json(new { satirlar });
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = ex.Message });
            }
        }

        private static IResult Kazanan(HttpContext baglam, IWebHostEnvironment ortam, string yil, string surecNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray bilgi = TNS.HRC.Arac.Tanimla().TeklifKazananFirmaListele(kullanan, OrtakFonksiyonlar.ConvertToInt(yil, 0), Temiz(surecNo));
                if (bilgi == null || bilgi.sonuc == null || !bilgi.sonuc.islemSonuc)
                    return Results.Json(new { hata = bilgi?.sonuc?.hataStr ?? "Kazanan firmalar okunamadı." });
                List<object> satirlar = new List<object>();
                foreach (TeklifFirma f in bilgi.objeler)
                    satirlar.Add(new { kod = f.firmaBilgisi.kod, ad = f.firmaBilgisi.ad ?? "" });
                return Results.Json(new { satirlar });
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = ex.Message });
            }
        }
    }
}
