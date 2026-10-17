using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.ATM;
using TNS.KYM;

namespace Istemci.AlacakTakip
{
    public class FaizHesabiIstegi
    {
        public double anaPara { get; set; }
        public string faizBasla { get; set; }
        public int tecilTur { get; set; }
        public string tecilBasla { get; set; }
        public string islemTar { get; set; }
    }

    /// <summary>
    /// Faiz Hesaplama ve Detay (eski AlacakTakip/FaizHesabi.aspx):
    ///   POST /api/alacaktakip/faizhesabi/hesapla   ana para, faiz başlangıcı, tecil ve işlem tarihine göre faizi ve hesap ayrıntısını döndürür
    ///                                              (eski btnHesapla_Click + FaizHesapla, aynı DosyaAB.FaizOranHesapla çağrıları).
    /// Hesap istemcide yapılır, veri tabanına yazılmaz.
    /// </summary>
    public static class FaizHesabiUclari
    {
        public const string Adres = "../AlacakTakip/FaizHesabi.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapPost("/api/alacaktakip/faizhesabi/hesapla", Hesapla);
        }

        public static bool Yetkili(Kullanici kullanan, string klasor)
        {
            return SayfaYetki.Yetkili(kullanan, klasor, Adres);
        }

        private static DateTime TarihOku(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return DateTime.MinValue;
            return DateTime.TryParseExact(s.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t)
                ? t : DateTime.MinValue;
        }

        private static async Task<IResult> Hesapla(HttpContext baglam, IWebHostEnvironment ortam, FaizHesabiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null) return Results.BadRequest();

            DateTime faizBasla = TarihOku(istek.faizBasla);
            int tecilTur = istek.tecilTur;
            DateTime tecilBasla = TarihOku(istek.tecilBasla);
            DateTime islemTar = TarihOku(istek.islemTar);

            // eski btnHesapla_Click denetimleri
            string hata = "";
            if (faizBasla == DateTime.MinValue) hata += "Faiz Başlangıç Tarihi boş olamaz.\n";
            if (islemTar == DateTime.MinValue) hata += "İşlem Tarihi boş olamaz.\n";
            if (islemTar < faizBasla) hata += "İşlem Tarihi faiz başlangıcından küçük olamaz.\n";
            if (tecilTur < 0 || tecilTur > 2) tecilTur = 0;
            if (tecilTur == 0)
                tecilBasla = DateTime.MinValue;
            else
            {
                if (tecilBasla == DateTime.MinValue)
                    hata += "Tecil Başlangıç Tarihi boş olamaz.\n";
                else if (tecilBasla < faizBasla)
                    hata += "Tecil Başlangıç Tarihi faiz başlangıcından küçük olamaz.\n";
                else if (islemTar < tecilBasla)
                    hata += "İşlem Tarihi tecil başlangıcından küçük olamaz.\n";
            }
            if (hata != "") return Results.Json(new { basarili = false, hata = hata.TrimEnd() });

            try
            {
                StringBuilder sb = new StringBuilder();
                double faiz = FaizHesapla(faizBasla, tecilTur, tecilBasla, islemTar, istek.anaPara, sb);
                return Results.Json(new { basarili = true, faiz = faiz, sonuc = sb.ToString() });
            }
            catch (Exception ex)
            {
                OrtakFonksiyonlar.HataDosyaYaz("FaizHesabi.Text", "Faiz hesabı yapılamadı: " + ex.Message, false);
                return Results.Json(new { basarili = false, hata = "Faiz hesaplanamadı." });
            }
        }

        // eski FaizHesabi.aspx.cs FaizHesapla, aynen
        private static double FaizHesapla(DateTime faizBaslangicTarihi, int tecilTur, DateTime taksitTalepTarihDT, DateTime islemTarihiDT, double ab, StringBuilder debugHesap)
        {
            int ay1, gun1, ay2, gun2;
            double oran1, oran2;
            double abf;
            if (taksitTalepTarihDT == DateTime.MinValue)
            {
                DateTime tutDT = islemTarihiDT;
                oran1 = DosyaAB.FaizOranHesapla(faizBaslangicTarihi, faizBaslangicTarihi, tutDT, DateTime.MinValue, true, taksitTalepTarihDT, taksitTalepTarihDT, 0, debugHesap, out ay1, out gun1);
                debugHesap.AppendLine($"faizBaslangicTarihi:{faizBaslangicTarihi.ToString()} işlem Tarihi: {tutDT.ToString()}: oran:{oran1}");
                abf = OrtakFonksiyonlar.KurusYap(ab * oran1);
                debugHesap.AppendLine($"Hesaplanan Faiz AB: {abf}");
                oran2 = 0;
            }
            else
            {
                DateTime tutDT = taksitTalepTarihDT.AddDays(-1);

                oran1 = DosyaAB.FaizOranHesapla(faizBaslangicTarihi, faizBaslangicTarihi, tutDT, DateTime.MinValue, true, taksitTalepTarihDT, taksitTalepTarihDT, tecilTur - 1, debugHesap, out ay1, out gun1);
                debugHesap.AppendLine($"faizBaslangicTarihi: {faizBaslangicTarihi.ToString()}  Tecil Baş Tarihi: {tutDT.ToString()}: oran:{oran1}");
                double ertab = OrtakFonksiyonlar.KurusYap(ab * oran1);
                debugHesap.AppendLine($"Hesaplanan Ert Faiz AB: {ertab}");

                oran2 = DosyaAB.FaizOranHesapla(faizBaslangicTarihi, tutDT, islemTarihiDT, DateTime.MinValue, false, taksitTalepTarihDT, taksitTalepTarihDT, tecilTur - 1, debugHesap, out ay2, out gun2);
                debugHesap.AppendLine($"Tecil Tarihi:{tutDT.ToString()} işlem Tarihi: {islemTarihiDT.ToString()}: oran:{oran2}");
                abf = ertab + OrtakFonksiyonlar.KurusYap((ab + ertab) * oran2);
                debugHesap.AppendLine($"Toplam Faiz AB: {abf} ");
            }
            return abf;
        }
    }
}
