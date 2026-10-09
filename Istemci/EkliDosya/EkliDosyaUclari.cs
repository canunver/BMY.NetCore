using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using TNS;
using TNS.KYM;

namespace Istemci.EkliDosya
{
    public class EkliDosyaIstegi
    {
        public string Modul { get; set; }
        public string Form { get; set; }
        public int No { get; set; }
        /// <summary>Kilit isteğinde: true = kilitle (onayla), false = onayı kaldır.</summary>
        public bool Kilitle { get; set; }
    }

    /// <summary>
    /// Ortak ekli dosyalar bileşeninin uçları (eski Duyuru/EkliDosyalarForm.aspx). Dosyalar bir modül koduna ve o modüldeki
    /// bir kayda (form) bağlıdır: ör. duyurular için modul = DUY, form = duyuru kodu. Yetki denetimi iş kurallarındadır
    /// (KYMEkliDosyalar.YetkiKontrol); burada yalnızca giriş ve istek jetonu denetlenir.
    ///   GET  /api/ekli-dosya?modul=&amp;form=[&amp;onEk=1][&amp;birimAd=1] : liste
    ///   POST /api/ekli-dosya/yukle (multipart: modul, form, aciklama, dosya)
    ///   GET  /api/ekli-dosya/indir?modul=&amp;form=&amp;no=[&amp;ek=0]
    ///   POST /api/ekli-dosya/sil, /kilit
    /// </summary>
    public static class EkliDosyaUclari
    {
        /// <summary>Kilitleme ve bağlantı adresi yalnızca bu modüllerde vardır (eski KomutGizleGoster).</summary>
        private static readonly string[] KilitliModuller = { "DUY", "YRD" };

        /// <summary>appSettings: EkliDosyaAzamiMB (varsayılan 20).</summary>
        public static int AzamiMB
        {
            get { return Math.Max(1, OrtakFonksiyonlar.ConvertToInt(Ayar.Oku("EkliDosyaAzamiMB", "20"), 20)); }
        }

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ekli-dosya", Liste);
            app.MapPost("/api/ekli-dosya/yukle", Yukle).DisableAntiforgery();
            app.MapGet("/api/ekli-dosya/indir", Indir);
            app.MapPost("/api/ekli-dosya/sil", Sil);
            app.MapPost("/api/ekli-dosya/kilit", Kilit);
        }

        private static bool Gecerli(string modul, string form)
        {
            return !string.IsNullOrWhiteSpace(modul) && !string.IsNullOrWhiteSpace(form);
        }

        private static bool KilitliModul(string modul)
        {
            return Array.IndexOf(KilitliModuller, (modul ?? "").ToUpperInvariant()) >= 0;
        }

        /// <param name="onEk">1 ise form kodu bu değerle başlayan bütün kayıtların dosyaları (eski davranış); yoksa yalnızca bu kayıt.</param>
        private static IResult Liste(HttpContext baglam, string modul, string form, string onEk, string birimAd)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();

            bool birimAdOlsun = birimAd == "1";
            List<object> satirlar = new List<object>();
            string hata = null;
            if (Gecerli(modul, form) && form != "-1")
            {
                EkliDosyalar arama = new EkliDosyalar { modul = modul, form = form };
                ObjectArray<EkliDosyalar> sonuc = TNS.KYM.Arac.Tanimla().ListeleEkliDosyalar(kullanan, arama, birimAdOlsun);
                if (!sonuc.sonuc.islemSonuc)
                    hata = Metin.EkliDosya("IslemHata");
                else
                    foreach (EkliDosyalar d in sonuc.objeler)
                    {
                        // İş kuralı form kodunu önek olarak arar (12 -> 120, 121 ...); istenmedikçe yalnızca bu kayıt.
                        if (onEk != "1" && !string.Equals(d.form, form, StringComparison.Ordinal)) continue;
                        satirlar.Add(new
                        {
                            form = d.form,
                            no = d.dosyaNo,
                            ad = d.dosyaAdiSade,
                            uzanti = d.uzanti,
                            buyukluk = d.buyukluk,
                            tarih = d.yuklemeTarihi == null || d.yuklemeTarihi.isNull ? "" : d.yuklemeTarihi.Oku().ToString("yyyy-MM-dd"),
                            yukleyen = d.yukleyen,
                            onaylayan = d.onaylayan,
                            aciklama = d.aciklama,
                            birimAd = d.birimAd
                        });
                    }
            }

            return Results.Json(new
            {
                satirlar = satirlar,
                hata = hata,
                kilitVar = KilitliModul(modul),
                azamiMB = AzamiMB,
                metin = Metinler()
            });
        }

        private static async Task<IResult> Yukle(HttpContext baglam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();

            IFormCollection istek = await baglam.Request.ReadFormAsync();
            string modul = istek["modul"];
            string form = istek["form"];
            IFormFile dosya = istek.Files["dosya"];
            if (!Gecerli(modul, form) || form == "-1") return Hata(Metin.EkliDosya("OnceKaydet"));
            if (dosya == null || dosya.Length == 0) return Hata(Metin.EkliDosya("DosyaSec"));
            if (dosya.Length > AzamiMB * 1024L * 1024L)
                return Hata(string.Format(Metin.EkliDosya("CokBuyuk"), AzamiMB));

            byte[] icerik;
            using (MemoryStream bellek = new MemoryStream())
            {
                await dosya.CopyToAsync(bellek);
                icerik = bellek.ToArray();
            }

            EkliDosyalar kayit = new EkliDosyalar(modul, form, 0)
            {
                dosyaAdi = Path.GetFileName(dosya.FileName),
                buyukluk = icerik.Length,
                aciklama = ((string)istek["aciklama"] ?? "").Trim()
            };
            kayit.ekliDosyaIcerik = icerik;

            Sonuc sonuc = TNS.KYM.Arac.Tanimla().KaydetEkliDosya(kullanan, kayit);
            return Sonuclandir(sonuc, Metin.EkliDosya("Yuklendi"));
        }

        /// <summary>
        /// Dosyayı gönderir. ek=0 ise tarayıcıda açılabilecek türler (pdf, resim, video, düz metin) sayfada açılır; HTML gibi
        /// betik çalıştırabilecek türler her zaman indirilir.
        /// </summary>
        public static IResult Indir(HttpContext baglam, string modul, string form, int? no, string ek)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Gecerli(modul, form) || no == null) return Results.BadRequest();

            EkliDosyalar arama = new EkliDosyalar(modul, form, no.Value);
            ObjectVAN<EkliDosyalar> sonuc = TNS.KYM.Arac.Tanimla().OkuEkliDosyaIcerik(kullanan, arama);
            if (sonuc == null || !sonuc.sonuc.islemSonuc || sonuc.o == null || sonuc.o.ekliDosyaIcerik == null)
                return Results.Text(Metin.EkliDosya("Bulunamadi"), "text/plain; charset=utf-8", statusCode: StatusCodes.Status404NotFound);

            string ad = Path.GetFileName(sonuc.o.dosyaAdi ?? "dosya");
            string tur;
            if (!new FileExtensionContentTypeProvider().TryGetContentType(ad, out tur)) tur = "application/octet-stream";
            bool guvenliTur = tur == "application/pdf" || tur.StartsWith("video/") || tur == "text/plain"
                || (tur.StartsWith("image/") && tur != "image/svg+xml");
            bool sayfadaAc = ek == "0" && guvenliTur;
            if (!guvenliTur) tur = "application/octet-stream";

            baglam.Response.Headers["X-Content-Type-Options"] = "nosniff";
            return sayfadaAc
                ? Results.File(sonuc.o.ekliDosyaIcerik, tur)
                : Results.File(sonuc.o.ekliDosyaIcerik, tur, ad);
        }

        private static async Task<IResult> Sil(HttpContext baglam, EkliDosyaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (istek == null || !Gecerli(istek.Modul, istek.Form)) return Hata(Metin.EkliDosya("HataliCagri"));

            Sonuc sonuc = TNS.KYM.Arac.Tanimla().SilEkliDosyalar(kullanan, new EkliDosyalar(istek.Modul, istek.Form, istek.No));
            return Sonuclandir(sonuc, Metin.EkliDosya("Silindi"));
        }

        private static async Task<IResult> Kilit(HttpContext baglam, EkliDosyaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (istek == null || !Gecerli(istek.Modul, istek.Form) || !KilitliModul(istek.Modul)) return Hata(Metin.EkliDosya("HataliCagri"));

            Sonuc sonuc = TNS.KYM.Arac.Tanimla().OnaylaOnayKaldir(kullanan, new EkliDosyalar(istek.Modul, istek.Form, istek.No), istek.Kilitle);
            return Sonuclandir(sonuc, null);
        }

        // ------------------------------------------------------------------ yardımcılar

        private static IResult Sonuclandir(Sonuc sonuc, string bilgi)
        {
            if (sonuc == null || !sonuc.islemSonuc)
                return Hata(sonuc == null || string.IsNullOrEmpty(sonuc.hataStr) ? Metin.EkliDosya("IslemHata") : sonuc.hataStr);
            return Results.Json(new { basarili = true, bilgi = bilgi });
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static Dictionary<string, string> Metinler()
        {
            Dictionary<string, string> m = new Dictionary<string, string>();
            foreach (string anahtar in new[] { "Aciklama", "Dosya", "Yukle", "DosyaAd", "Tur", "Boy", "Tarih", "Yukleyen", "Birim",
                                               "Indir", "Sil", "Baglanti", "Kilitle", "KilitAc", "Kilitli", "SilOnay", "YukleOnay",
                                               "DosyaSec", "Yukleniyor", "Kopyalandi", "OnceKaydet", "CokBuyuk", "IslemHata", "Islem" })
                m[anahtar] = Metin.EkliDosya(anahtar);
            return m;
        }
    }
}
