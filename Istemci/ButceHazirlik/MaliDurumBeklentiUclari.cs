using System;
using System.Threading.Tasks;
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
    public class MaliDurumKayitIstegi
    {
        public int Yil { get; set; }
        /// <summary>Faaliyet alanının metni ("kod- ad" ya da yalnızca kod).</summary>
        public string Faaliyet { get; set; }
        public string ButceUygulamaSonucu { get; set; }
        public string YurutulenFaaliyetler { get; set; }
        public string BeklentilerveHedefler { get; set; }
        public string YurutulecekFaaliyetler { get; set; }
    }

    public class MaliDurumOnayIstegi
    {
        public int Yil { get; set; }
        /// <summary>true: veri girişine kapat (eski Veri Gir. Kapat), false: aç (eski Veri Gir. Aç).</summary>
        public bool Kapat { get; set; }
    }

    /// <summary>
    /// Kurumsal Mali Durum ve Beklentiler Raporu sayfasının (eski ButceHazirlikv2/BHMMaliDurumBeklentiForm.aspx) uçları:
    ///   GET  /api/malidurum/tanim, /kodlar?yil=&amp;tur=14&amp;ust=, /oku?yil=&amp;faaliyet=, /rapor?yil=&amp;faaliyet=&amp;eski=
    ///   POST /api/malidurum/kaydet, /onay (yalnızca sistem yöneticisi; eski sayfada Veri Gir. Kapat / Aç ona görünürdü)
    /// Okuma ve kayıt eskisi gibi BHMv2 OkuBHMMaliDurumBeklenti / KaydetBHMMaliDurumBeklenti ile; faaliyet görme / yazma yetkisi,
    /// faaliyet kodunun tam olması ve yılın veri girişine açık olması iş kurallarında denetlenir.
    /// </summary>
    public static class MaliDurumBeklentiUclari
    {
        public const string Adres = "../ButceHazirlikv2/BHMMaliDurumBeklentiForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/malidurum/tanim", Tanim);
            app.MapGet("/api/malidurum/kodlar", Kodlar);
            app.MapGet("/api/malidurum/oku", Oku);
            app.MapGet("/api/malidurum/rapor", Rapor);
            app.MapPost("/api/malidurum/kaydet", Kaydet);
            app.MapPost("/api/malidurum/onay", Onay);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Adres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static bool SistemYoneticisi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
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

        /// <summary>Faaliyet alanındaki "kod- ad" metninden kod (eski BHMv2Ortak.KodAl(ddProg.Text)).</summary>
        private static string Kod(string deger)
        {
            return TNS.BHMv2.Arac.TiredenKodBul((deger ?? "").Trim()).Trim();
        }

        /// <summary>Açılış (eski Page_Load): yıl bu yıl; Veri Gir. Kapat / Aç tuşları yalnızca sistem yöneticisine.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return Results.Json(new { yil = DateTime.Now.Year, yonetici = SistemYoneticisi(kullanan) });
        }

        /// <summary>Faaliyet kod ağacının bir düzeyi (eski nodeLoadPar3('14', yil, '', node, true, false, false, 20)).</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((tur ?? "").Trim() != "14") return Hata("Kod türü hatalı!");
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, "14", ust);
        }

        /// <summary>Oku (eski btnOku_Click): yılın ve faaliyetin dört metni.</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil, string faaliyet)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            string kod = Kod(faaliyet);
            if (yil <= 0 || kod == "") return Hata("Anahtar alanları girmelisiniz.");
            try
            {
                MaliDurumBeklenti form = new MaliDurumBeklenti { yil = yil, faaliyetKod = kod };
                form = TNS.BHMv2.Arac.Tanimla().OkuBHMMaliDurumBeklenti(kullanan, form);
                if (form == null) return Results.Json(new { bulundu = false });
                return Results.Json(new
                {
                    bulundu = true,
                    butceUygulamaSonucu = form.butceUygulamaSonucu ?? "",
                    yurutulenFaaliyetler = form.yurutulenFaaliyetler ?? "",
                    beklentilerveHedefler = form.beklentilerveHedefler ?? "",
                    yurutulecekFaaliyetler = form.yurutulecekFaaliyetler ?? ""
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kayıt okunamadı."));
            }
        }

        /// <summary>Kaydet (eski btnKaydet_Click): yılın ve faaliyetin kaydı silinip dört metinle yeniden yazılır (iş kuralı).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, MaliDurumKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            string kod = Kod(istek.Faaliyet);
            if (istek.Yil <= 0 || kod == "") return Results.Json(new { basarili = false, hata = "Anahtar alanları girmelisiniz." });
            try
            {
                MaliDurumBeklenti form = new MaliDurumBeklenti
                {
                    yil = istek.Yil,
                    faaliyetKod = kod,
                    butceUygulamaSonucu = istek.ButceUygulamaSonucu ?? "",
                    yurutulenFaaliyetler = istek.YurutulenFaaliyetler ?? "",
                    beklentilerveHedefler = istek.BeklentilerveHedefler ?? "",
                    yurutulecekFaaliyetler = istek.YurutulecekFaaliyetler ?? ""
                };
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().KaydetBHMMaliDurumBeklenti(kullanan, form);
                if (sonuc == null || !sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc?.hataStr, "Kayıt yapılamadı.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = HataMetni(ex.Message, "Kayıt yapılamadı.") });
            }
        }

        /// <summary>
        /// Veri Gir. Kapat / Aç (eski btnOnayla_Click / btnOnaykaldir_Click; sistem yöneticisi): yılın mali beklenti raporu veri
        /// girişine kapatılır (KurumOnayla) ya da açılır (KurumOnayKaldir); saymanlık 0, birim "000", aşama "0000".
        /// </summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, MaliDurumOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek.Yil <= 0) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });
            try
            {
                ENUMTasraHazirlikOnayDurum onayKod = istek.Kapat ? ENUMTasraHazirlikOnayDurum.KurumOnayla : ENUMTasraHazirlikOnayDurum.KurumOnayKaldir;
                Sonuc s = TNS.BHMv2.Arac.Tanimla().OnayKodAtaTasra(kullanan, istek.Yil, 0, "000", "0000", ENUMBirimOnayTur.MALIBEKLENTI, onayKod);
                if (s == null || !s.islemSonuc) return Results.Json(new { basarili = false, hata = "İşlem yapılamadı: " + HataMetni(s?.hataStr, "") });
                return Results.Json(new
                {
                    basarili = true,
                    bilgi = istek.Kapat ? "Mali beklenti raporu veri girişine kapatılmıştır." : "Mali beklenti raporu veri girişine açılmıştır."
                });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = "İşlem yapılamadı: " + HataMetni(ex.Message, "") });
            }
        }

        /// <summary>
        /// Yazdır (eski btnYazdirYeni_Click; eski=1 ise eski sayfada gizli duran btnYazdir_Click). Eski Aspose.Words kodu ve eski
        /// .docx şablonları (BHMv2\MaliDurumBeklentiYeni.docx, MaliDurumBeklenti.docx) aynen kullanılır.
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string faaliyet, int? eski)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl hatalı!");
            try
            {
                AsposeLisans.Yukle();
                string dosya = MaliDurumBeklentiRaporu.Yazdir(kullanan, yil, Kod(faaliyet), eski == 1, ortam.ContentRootPath, out string hata);
                if (dosya == null) return Hata(HataMetni(hata, "Rapor hazırlanamadı."));
                return DosyaIslem.DosyaGonder(dosya, "MaliDurumBeklenti_" + DosyaIslem.DosyaAdUretSade(), true, "docx");
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
