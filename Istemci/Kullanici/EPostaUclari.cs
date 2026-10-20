using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;

namespace Istemci.Kullanicilar
{
    /// <summary>e-posta Gönder isteği (eski ePostaAt btnTamam_Click).</summary>
    public class EPostaKaydetIstegi
    {
        public string konu { get; set; }
        public string mesaj { get; set; }
        public int ktKod { get; set; }
    }

    /// <summary>Tekrar Gönder isteği (eski ePostaGonderi "Gonder" komutu).</summary>
    public class EPostaTekrarIstegi
    {
        public int mesajID { get; set; }
    }

    /// <summary>
    /// Kullanıcılara e-posta Gönderme (eski KYM/ePostaAt.aspx) ve Önceki e-posta Gönderimleri (eski KYM/ePostaGonderi.aspx) uçları:
    ///   GET  /api/eposta/tanim                      gönderen adresi ve kullanıcı grupları (eski Page_Load)
    ///   POST /api/eposta/kaydet                     e-posta Gönder (eski btnTamam_Click; IKYMServis.EPostaKaydet)
    ///   GET  /api/eposta/gonderiler                 önceki gönderiler (eski Listele / Yenile; IKYMServis.EPostaGonderOncekiGonderiler)
    ///   GET  /api/eposta/rapor?mesajID=&amp;gonderildiMi=  gönderilecek (0) / gönderilen (1) kişiler, Excel
    ///                                               (eski Hatalilar / Gonderilen komutları; şablon KYM\EPostaGiden.xlt)
    ///   POST /api/eposta/tekrar                     Tekrar Gönder (eski Gonder komutu; IKYMServis.EPostaGonderTekrar)
    /// Gönderim altyapısı eskisiyle aynıdır: EPostaKaydet postayı ve alıcıları kaydeder, gönderimi iş kuralları kendi iş parçacığında
    /// (OrtakFonksiyonlar.MailAt) yapar. Yetki: e-posta Gönder menüdeki sayfanın yetkisi (eski SayfayaGirisYapabilirMi);
    /// Önceki Gönderiler yalnızca sistem yöneticisi ya da destek yöneticisi (eski ePostaGonderi Page_Load).
    /// </summary>
    public static class EPostaUclari
    {
        public const string SayfaAdresi = "../KYM/ePostaAt.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/eposta/tanim", Tanim);
            app.MapPost("/api/eposta/kaydet", Kaydet);
            app.MapGet("/api/eposta/gonderiler", Gonderiler);
            app.MapGet("/api/eposta/rapor", Rapor);
            app.MapPost("/api/eposta/tekrar", Tekrar);
        }

        /// <summary>e-posta Gönder sayfasına giriş yetkisi.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        /// <summary>Önceki Gönderiler sayfasına giriş yetkisi: sistem yöneticisi ya da destek yöneticisi.</summary>
        public static bool GonderiYetkiVar(Kullanici kullanan)
        {
            if (kullanan == null || string.IsNullOrEmpty(kullanan.kullaniciKodu)) return false;
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) || DestekYoneticisi(kullanan);
        }

        private static bool DestekYoneticisi(Kullanici kullanan)
        {
            try
            {
                Sonuc s = TNS.KYM.Yetki.DestekYoneticisiMi(kullanan);
                return s != null && s.islemSonuc;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult GonderiDenetle(HttpContext baglam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GonderiYetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IKYMServis Servis()
        {
            return TNS.KYM.Arac.Tanimla();
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = hata ?? "";
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata;
        }

        /// <summary>Gönderilecek kullanıcı grupları (eski lstKullaniciTipi): destek yöneticisi yalnızca destek kullanıcı tiplerini görür,
        /// diğerleri en başta "Tüm kullanıcı tiplerine" (kod 0) seçeneğini de görür.</summary>
        private static List<object> Gruplar(Kullanici kullanan, bool destek)
        {
            List<object> gruplar = new List<object>();
            if (!destek)
                gruplar.Add(new { kod = 0, ad = Metin.Kullanici("FRMEPA007") });

            foreach (ENUMKullaniciTipi kt in TNS.KYM.Arac.KTListesi(kullanan))
            {
                int kod = TNS.KYM.Arac.KullaniciTipiKodu(kt);
                if (kod <= 0) continue;
                if (destek && !TNS.KYM.Arac.DestekKullaniciTipindenMi(kod)) continue;
                gruplar.Add(new { kod, ad = kod + "-" + TNS.KYM.Arac.KullaniciTipiAdi(kt) });
            }
            return gruplar;
        }

        // ------------------------------------------------------------------ e-posta Gönder

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return Results.Json(new
                {
                    adres = kullanan.emailAdresi ?? "",
                    gruplar = Gruplar(kullanan, DestekYoneticisi(kullanan)),
                    gonderiYetki = GonderiYetkiVar(kullanan)
                });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Kullanıcı grupları okunamadı.", adres = "", gruplar = new List<object>(), gonderiYetki = false });
            }
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, EPostaKaydetIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            string konu = (istek.konu ?? "").Trim();
            string mesaj = istek.mesaj ?? "";
            List<string> hatalar = new List<string>();
            if (konu == "") hatalar.Add(Metin.Kullanici("FRMEPA010"));
            if (mesaj.Trim() == "") hatalar.Add(Metin.Kullanici("FRMEPA011"));
            if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar));
            if (konu.Length > 255) konu = konu.Substring(0, 255);

            // Destek yöneticisine yalnızca destek kullanıcı tipleri listelenir; başka bir grup gönderilemesin.
            if (DestekYoneticisi(kullanan) && !TNS.KYM.Arac.DestekKullaniciTipindenMi(istek.ktKod))
                return Hata("Bu kullanıcı grubuna e-posta gönderme yetkiniz yok.");

            try
            {
                // Eski sayfadaki gibi satır sonları <br> olur (posta HTML olarak gönderilir).
                Sonuc s = Servis().EPostaKaydet(kullanan, konu, mesaj.Replace("\r", "").Replace("\n", "<br>"), istek.ktKod);
                if (s == null || !s.islemSonuc) return Hata(HataMetni(s?.hataStr, "e-posta kaydedilemedi."));
                return Results.Json(new { basarili = true, bilgi = s.bilgiStr, mesajID = OrtakFonksiyonlar.ConvertToInt(s.anahtar, 0) });
            }
            catch (Exception)
            {
                return Hata("e-posta kaydedilemedi.");
            }
        }

        // ------------------------------------------------------------------ Önceki Gönderiler

        private static IResult Gonderiler(HttpContext baglam)
        {
            IResult ret = GonderiDenetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<object> satirlar = new List<object>();
            try
            {
                ObjectArray<Posta> postalar = Servis().EPostaGonderOncekiGonderiler();
                if (postalar != null && postalar.sonuc != null && !postalar.sonuc.islemSonuc)
                    return Results.Json(new { hata = HataMetni(postalar.sonuc.hataStr, "Gönderiler okunamadı."), satirlar });
                if (postalar != null)
                    foreach (Posta p in postalar.objeler)
                        satirlar.Add(new
                        {
                            p.mesajID,
                            p.konu,
                            p.gonderen,
                            tarih = p.tarih != null && !p.tarih.isNull ? p.tarih.ToLongString() : "",
                            // Eski ekrandaki gibi: Gönderilecek = henüz gönderilemeyenler, Gönderilen = başarılı olanlar.
                            gonderilecek = p.toplamGonderilen - p.toplamBasarili,
                            gonderilen = p.toplamBasarili
                        });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Gönderiler okunamadı.", satirlar });
            }
            return Results.Json(new { satirlar });
        }

        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int? mesajID, int? gonderildiMi)
        {
            IResult ret = GonderiDenetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            int kod = mesajID ?? 0;
            int gonderildi = gonderildiMi == 1 ? 1 : 0;
            if (kod <= 0) return Results.Json(new { hata = "Mesaj seçilmedi." });
            try
            {
                string sablon = SablonBul(ortam.ContentRootPath, "EPostaGiden.xlt");
                if (sablon == null) return Results.Json(new { hata = "EPostaGiden.xlt şablonu bulunamadı." });

                ObjectArray<Posta> postalar = Servis().EPostaGonderilenlerListele(kod, gonderildi);
                if (postalar == null || (postalar.sonuc != null && !postalar.sonuc.islemSonuc))
                    return Results.Json(new { hata = HataMetni(postalar?.sonuc?.hataStr, "Liste okunamadı.") });

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;
                int kaynakSatir = 0;
                string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                XLS.DosyaAc(sablon, sonucDosyaAd);
                XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
                satir = kaynakSatir;

                foreach (Posta p in postalar.objeler)
                {
                    satir++;
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);

                    XLS.HucreAdBulYaz("txtMesajNo", p.mesajID);

                    XLS.HucreDegerYaz(satir, sutun, p.kullaniciKodu);
                    XLS.HucreDegerYaz(satir, sutun + 1, p.tarih != null && !p.tarih.isNull ? p.tarih.ToLongString() : "");
                    XLS.HucreDegerYaz(satir, sutun + 2, p.email);
                    XLS.HucreDegerYaz(satir, sutun + 3, p.hataMesaji);
                    XLS.HucreDegerYaz(satir, sutun + 4, p.gonderildiMi == 0 ? "HAYIR" : "EVET");
                }

                XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
                XLS.DosyaSaklaTamYol();
                string ad = (gonderildi == 1 ? "Gönderilen Kişiler " : "Gönderilecek Kişiler ") + kod;
                return DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), ad, true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Liste hazırlanamadı." });
            }
        }

        private static async Task<IResult> Tekrar(HttpContext baglam, IAntiforgery sahtecilik, EPostaTekrarIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = GonderiDenetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.mesajID <= 0) return Hata("Mesaj seçilmedi.");
            try
            {
                Servis().EPostaGonderTekrar(istek.mesajID);
                return Results.Json(new { basarili = true, bilgi = istek.mesajID + " " + Metin.Kullanici("FRMEPG011") });
            }
            catch (Exception)
            {
                return Hata("Tekrar gönderim başlatılamadı.");
            }
        }

        /// <summary>Şablon önce kurumun RaporSablonYol\KYM klasöründe, yoksa uygulamanın RaporSablon\KYM klasöründe aranır.</summary>
        private static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "KYM", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "KYM", sablonAd);
            return File.Exists(u) ? u : null;
        }
    }
}
