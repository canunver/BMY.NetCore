using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TNS;

namespace Istemci.Guncelleme
{
    public class SifreIstegi
    {
        public string Veri { get; set; }
        /// <summary>1: şifreli yap, 2: şifre çöz.</summary>
        public int Yon { get; set; }
        /// <summary>Veri bir XML (appSettings) ise her &lt;add&gt; düğümünün value değeri ayrı ayrı işlenir.</summary>
        public bool Xml { get; set; }
    }

    /// <summary>
    /// Şifre Üret sayfasının (eski YazilimGuncelle/SifreUret.aspx) ucu: POST /api/guncelleme/sifre. Yalnızca sistem yöneticisi.
    /// Ortak anahtarla şifreler ya da çözer (OrtakFonksiyonlar.SifreleOrtakAnahtar / SifreliCozOrtakAnahtar).
    /// </summary>
    public static class SifreUretUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapPost("/api/guncelleme/sifre", SifreIslem);
        }

        private static async Task<IResult> SifreIslem(HttpContext baglam, SifreIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = GuncellemeOrtak.Denetle(baglam, GuncellemeOrtak.SifreUretYetkili, out _);
            if (ret != null) return ret;
            if (istek == null || (istek.Yon != 1 && istek.Yon != 2)) return GuncellemeOrtak.Hata("İşlem seçiniz.");
            string veri = istek.Veri ?? "";
            try
            {
                string sonuc = istek.Xml ? XMLCoz(veri, istek.Yon) : SifreIslemIc(veri, istek.Yon);
                return Results.Json(new { basarili = true, sonuc = sonuc });
            }
            catch (Exception ex)
            {
                // Eski sayfa hatayı sonuç alanına yazıyordu.
                return Results.Json(new { basarili = true, sonuc = ex.Message });
            }
        }

        private static string XMLCoz(string v1, int yon)
        {
            try
            {
                XmlDocument x = new XmlDocument();
                x.LoadXml(v1);
                foreach (XmlNode node in x.ChildNodes[0].ChildNodes)
                {
                    if (node.Name == "add" && node.Attributes?["value"] != null)
                        node.Attributes["value"].Value = SifreIslemIc(node.Attributes["value"].Value, yon);
                }

                StringWriter stringWriter = new StringWriter(new StringBuilder());
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented };
                x.Save(xmlTextWriter);
                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static string SifreIslemIc(string v, int yon)
        {
            if (yon == 1)
                return OrtakFonksiyonlar.SifreleOrtakAnahtar(v);
            else
                return OrtakFonksiyonlar.SifreliCozOrtakAnahtar(v);
        }
    }
}
