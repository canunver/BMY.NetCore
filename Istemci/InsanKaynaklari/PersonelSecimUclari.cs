using System;
using System.Collections.Generic;
using System.Globalization;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>
    /// Ortak personel seçimi (eski InsanKaynaklari/ListePersonelSec.aspx; yeni bileşen wwwroot/js/personelsec.js, Sgb.personelSec) uçları:
    ///   GET /api/personelsec/liste?ara=&amp;birim=&amp;perTip=&amp;tarih=&amp;tarih2=   (eski PersonelListeleIC)
    ///   GET /api/personelsec/birim?muhasebe=&amp;birim=                     (eski Page_Load: muhasebe + birim kodundan önerilen birim)
    ///   GET /api/personelsec/orgagac?ust=&amp;ara=                         (eski ExtOrgSecim, "Personel Saymanlık-Birim Yetkisi")
    /// Bileşeni açan sayfalardan birinin yetkisi yeterlidir (ListePersonelSec menüde yok; eski sayfada yetki denetimi yoktu).
    /// </summary>
    public static class PersonelSecimUclari
    {
        private const string Uc = "/api/personelsec/";
        public const string SayfaAdresi = "../InsanKaynaklari/ListePersonelSec.aspx";

        /// <summary>Bileşeni kullanan sayfalar. Yeni bir sayfa bileşeni kullanınca buraya eklenir.</summary>
        public static readonly string[] UstSayfalar =
        {
            "../InsanKaynaklari/PersonelBilgi.aspx",
            "../InsanKaynaklari/SendikaBilgiForm.aspx",
            "../HarcamaSurecleri/PSLDinBordroYonetim.aspx",   // bordro detay sayfaları menüde yok, yönetim sayfasından açılır
            "../HarcamaSurecleri/PSLDinBordroDetay.aspx",
            "../HarcamaSurecleri/PSLDinBordroDetay2.aspx",
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "birim", Birim);
            app.MapGet(Uc + "orgagac", OrgAgac);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!SayfaYetki.Yetkili(kullanan, ortam.ContentRootPath, SayfaAdresi, UstSayfalar)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string metin) { return Results.Json(new { hata = metin }); }

        /// <summary>
        /// Eski PersonelListeleIC: aktif personel; tarih verilirse o aralıkta aktif olanlar (aktifTarihTur 10, aktifDurum -1).
        /// perTip: ";" ile ayrılmış personel türleri.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string ara, int? birim, string perTip, string tarih, string tarih2)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TNS.IKY.Personel p = new TNS.IKY.Personel();
                p.genelSorgu = ara ?? "";
                p.personelTipleri = perTip ?? "";
                p.aktifDurum = 1;
                if (!string.IsNullOrWhiteSpace(tarih))
                {
                    p.aktifTarih = new TNSDateTime(EskiTarih(tarih));
                    p.aktifTarihSon = new TNSDateTime(EskiTarih(tarih2));
                    p.aktifTarihSon.AddDays(1);
                    p.aktifTarihTur = 10;
                    p.aktifDurum = -1;
                }
                if ((birim ?? 0) > 0) p.kadro.organizasyonKodu = birim.Value;

                ObjectArray liste = TNS.IKY.Arac.Tanimla().PersonelListele(kullanan, p, false, true);
                if (!liste.sonuc.islemSonuc) return Hata(PersonelBilgiOrtak.HataMetni(liste.sonuc.hataStr, "Personel listesi okunamadı."));

                List<object> satirlar = new List<object>();
                foreach (object o in liste.objeler)
                {
                    if (!(o is TNS.IKY.Personel x)) continue;
                    satirlar.Add(new
                    {
                        x.kod,
                        x.TCKIMLIKNO,
                        x.AD,
                        x.SOYAD,
                        x.adSoyad,
                        x.UNVANKOD,
                        x.UNVANAD,
                        x.KADRODERECE,
                        x.EKGOSTERGE,
                        x.EPOSTA,
                        x.sicilNo,
                        x.sskSicilNo,
                        x.bankaHesapNo,
                        x.sinifKodu,
                        x.personelTip,
                        x.birimAd,
                        x.odemeEsasDerece,
                        x.odemeEsasKademe,
                        x.odemeEsasGosterge,
                        x.odemeEsasEkGosterge,
                        x.emekliEsasDerece,
                        x.emekliEsasKademe,
                        x.emekliEsasGosterge,
                        x.emekliEsasEkGosterge
                    });
                }
                return Results.Json(new { satirlar });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Personel listesi okunamadı."));
            }
        }

        /// <summary>
        /// Eski Page_Load: muhasebe kodu ve 3 haneli birim kodu verilirse o birim önerilir; sistem yöneticisi değilse birim değiştirilemez.
        /// </summary>
        private static IResult Birim(HttpContext baglam, IWebHostEnvironment ortam, string muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                muhasebe = (muhasebe ?? "").Trim();
                birim = (birim ?? "").Trim();
                if (muhasebe == "" || birim.Length != 3) return Results.Json(new { kod = 0, ad = "", kilitli = false });
                TNS.ORG.OrganizasyonBirim ob = new TNS.ORG.OrganizasyonBirim();
                ob.muhasebekodu = muhasebe;
                ob.muhasebebirim = birim;
                ObjectArray<TNS.ORG.OrganizasyonBirim> oa = TNS.ORG.Arac.Tanimla().OrganizasyonBirimListele(ob, false, false);
                if (oa != null && oa.sonuc.islemSonuc && oa.objeler != null && oa.objeler.Count > 0)
                {
                    ob = oa.objeler[0];
                    return Results.Json(new { kod = ob.kod, ad = ob.adi ?? "", kilitli = !PersonelBilgiOrtak.SistemYoneticisi(kullanan) });
                }
                return Results.Json(new { kod = 0, ad = "", kilitli = false });
            }
            catch (Exception)
            {
                return Results.Json(new { kod = 0, ad = "", kilitli = false });
            }
        }

        private static IResult OrgAgac(HttpContext baglam, IWebHostEnvironment ortam, int? ust, string ara)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return PersonelBilgiUclari.BirimSatirlari(kullanan, ust, ara);
        }

        /// <summary>yyyy-MM-dd ya da gg.aa.yyyy; TNSDateTime(string) geçerli kültürle okur, gg.aa.yyyy verilir.</summary>
        private static string EskiTarih(string s)
        {
            s = (s ?? "").Trim();
            string[] bicimler = { "yyyy-MM-dd", "dd.MM.yyyy", "d.M.yyyy" };
            if (DateTime.TryParseExact(s, bicimler, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                return d.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            return s;
        }
    }
}
