using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ButceHazirlikv2;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    public class PersonelSatiri
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Unvan { get; set; }
        public string Birim { get; set; }
        public string IsTel { get; set; }
        public string CepTel { get; set; }
        public string Faks { get; set; }
        public string EPosta { get; set; }
    }

    public class PersonelKayitIstegi
    {
        public int Yil { get; set; }
        /// <summary>Faaliyet alanının metni ("kod- ad" ya da yalnızca kod).</summary>
        public string Peb { get; set; }
        public List<PersonelSatiri> Satirlar { get; set; }
    }

    public class PersonelKopyalaIstegi
    {
        public int Yil { get; set; }
    }

    /// <summary>
    /// Bütçe Çalışmalarında Görevli Personel Listesi sayfasının (eski ButceHazirlikv2/ButceCalismasiPersoneliForm.aspx) JavaScript'ten
    /// çağırdığı uçlar:
    ///   GET  /api/butcepersonel/tanim, /kodlar?yil=&amp;tur=14&amp;ust=, /liste?yil=&amp;peb=, /rapor?yil=&amp;peb=
    ///   POST /api/butcepersonel/kaydet, /kopyala
    /// Kaydet seçilen yıl ve alt faaliyetin listesini silip gönderilen satırları yazar; Önc. Yıldan Kopyala yalnızca sistem
    /// yöneticisinindir (eski sayfada tuş yalnızca ona görünürdü). Faaliyet görme / yazma yetkisi iş kurallarında denetlenir.
    /// </summary>
    public static class ButceCalismasiPersoneliUclari
    {
        private const string SayfaAdresi = "../ButceHazirlikv2/ButceCalismasiPersoneliForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/butcepersonel/tanim", Tanim);
            app.MapGet("/api/butcepersonel/kodlar", Kodlar);
            app.MapGet("/api/butcepersonel/liste", Liste);
            app.MapGet("/api/butcepersonel/rapor", Rapor);
            app.MapPost("/api/butcepersonel/kaydet", Kaydet);
            app.MapPost("/api/butcepersonel/kopyala", Kopyala);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdresi);
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

        private static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata });
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        /// <summary>Faaliyet alanındaki "kod- ad" metninden kod (eski TNS.BHMv2.Arac.TiredenKodBul(ddProg.Text)).</summary>
        private static string Kod(string deger)
        {
            return TNS.BHMv2.Arac.TiredenKodBul((deger ?? "").Trim()).Trim();
        }

        private static string Temiz(string deger)
        {
            return (deger ?? "").Trim();
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Açılış: bütçe hazırlık yılı (eski GenelIslemlerIstemci.HazirlikYiliBul) ve kopyalama tuşu görünsün mü.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int yil = OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/BUTCEHAZIRLIK"), DateTime.Now.Year + 1);
            return Results.Json(new { yil = yil, yonetici = GenelGorevUclari.YetkiVar(kullanan) });
        }

        /// <summary>Faaliyet kod ağacının bir düzeyi (eski nodeLoadPar3('14', ...)); Gider Teklifi sayfasıyla ortak.</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, tur, ust);
        }

        /// <summary>Listele (eski Oku): yılın ve faaliyetin personel listesi.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string peb)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                ButceCalismasiPersoneli form = new ButceCalismasiPersoneli();
                form.yil = yil;
                form.pebkod = Kod(peb);
                ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleButceCalismasiPersoneli(kullanan, form);
                if (oa == null) return Hata("Liste okunamadı.");
                if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                for (int i = 0; i < oa.ObjeSayisi; i++)
                {
                    ButceCalismasiPersoneli p = (ButceCalismasiPersoneli)oa.objeler[i];
                    satirlar.Add(new
                    {
                        siraNo = p.siraNo, pebkod = p.pebkod ?? "", pebad = p.pebad ?? "",
                        ad = p.ad ?? "", soyad = p.soyad ?? "", unvan = p.unvan ?? "", birim = p.birim ?? "",
                        isTel = p.isTel ?? "", cepTel = p.cepTel ?? "", faks = p.faks ?? "", ePosta = p.ePosta ?? ""
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Liste okunamadı.");
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Kaydet (eski btnKaydet_Click): tablodaki satırlar sırasıyla (siraNo 1, 2, ...) seçilen yıl ve faaliyete yazılır; o faaliyetin
        /// var olan listesi iş kurallarında silinip yerine yazılır. Bütün alanları boş satırlar gönderilmez.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });

            try
            {
                string pebkod = Kod(istek.Peb);
                TNSCollection formlar = new TNSCollection();
                foreach (PersonelSatiri s in istek.Satirlar ?? new List<PersonelSatiri>())
                {
                    if (s == null) continue;
                    ButceCalismasiPersoneli f = new ButceCalismasiPersoneli();
                    f.yil = istek.Yil;
                    f.pebkod = pebkod;
                    f.ad = Temiz(s.Ad);
                    f.soyad = Temiz(s.Soyad);
                    f.unvan = Temiz(s.Unvan);
                    f.birim = Temiz(s.Birim);
                    f.isTel = Temiz(s.IsTel);
                    f.cepTel = Temiz(s.CepTel);
                    f.faks = Temiz(s.Faks);
                    f.ePosta = Temiz(s.EPosta);
                    if (f.ad + f.soyad + f.unvan + f.birim + f.isTel + f.cepTel + f.faks + f.ePosta == "") continue;
                    f.siraNo = formlar.Count + 1;
                    formlar.Add(f);
                }

                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().KaydetButceCalismasiPersoneli(kullanan, formlar);
                if (sonuc == null || !sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>
        /// Önc. Yıldan Kopyala (eski btnKopyala_Click; yalnızca sistem yöneticisi): seçilen yıl gelecek bir yılsa o yılın bütün listeleri
        /// silinir ve önceki yılınkiler kopyalanır (iş kuralı).
        /// </summary>
        private static async Task<IResult> Kopyala(HttpContext baglam, IWebHostEnvironment ortam, PersonelKopyalaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!GenelGorevUclari.YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });

            try
            {
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().KopyalaButceCalismasiPersoneli(kullanan, istek.Yil);
                if (sonuc == null || !sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "Kopyalama işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kopyalama işlemi başarısız." });
            }
        }

        // ------------------------------------------------------------------ rapor (eski kodla)

        /// <summary>Yazdır (eski btnYazdir_Click): yılın ve faaliyetin personel listesi (şablon BHMv2\ButceCalismasiPersoneliForm.xltx).</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string peb)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                AsposeLisans.Yukle();
                string dosya = ButceCalismasiPersoneliRaporu.Yazdir(TNS.BHMv2.Arac.Tanimla(), kullanan, yil, Kod(peb), Ayar.Oku("RaporSablonYol"), out string hata);
                if (dosya == null) return Hata(HataMetni(hata, "Rapor hazırlanamadı."));
                return DosyaIslem.DosyaGonder(dosya, "ButceCalismasiPersoneliForm" + DosyaIslem.DosyaAdUretSade(), true, "xlsx");
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
