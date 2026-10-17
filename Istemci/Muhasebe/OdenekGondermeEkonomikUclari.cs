using System;
using System.Collections.Generic;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Ödenek Gönderme/Tenkis Bilgisi sayfasının (eski ButceMuhasebe/OdenekGondermeTenkisEkonomik.aspx) uçları:
    ///   GET /api/ogtekonomik/tanim                              : yıllar (bu yıl - 3 .. bu yıl), bu yıl
    ///   GET /api/ogtekonomik/liste?yil=&amp;muhasebe=&amp;birim=&amp;eko=   : eski btnListele_Click (IMUHServis.HarcamaDurumListesi,
    ///        ekonomik kod 2. düzey, birim detayı): tertip başına onaylı / onaysız ödenek gönderme + tenkis, harcama ve kalan.
    /// Sayfa menüde yoktur; Ödenek Gönderme / Tenkis formundaki "Önceden gönderilen ödenekler" tuşuyla açılır. Yetkisi formun
    /// menü satırlarından (OdenekGondermeTenkisFrame.aspx?belgetur=3|4[&amp;talep=1]) gelir.
    /// </summary>
    public static class OdenekGondermeEkonomikUclari
    {
        public const string SayfaAdres = "../ButceMuhasebe/OdenekGondermeTenkisEkonomik.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ogtekonomik/tanim", Tanim);
            app.MapGet("/api/ogtekonomik/liste", Liste);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            int og = OdenekGondermeUclari.OdenekGondermeTuru, tk = OdenekGondermeUclari.TenkisTuru;
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdres,
                OdenekGondermeUclari.Adres(og, false), OdenekGondermeUclari.Adres(tk, false),
                OdenekGondermeUclari.Adres(og, true), OdenekGondermeUclari.Adres(tk, true));
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        /// <summary>Açılış (eski Page_Load): yıl listesi bu yıldan 3 yıl öncesine, varsayılan bu yıl.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<int> yillar = new List<int>();
            for (int i = DateTime.Now.Year; i >= DateTime.Now.Year - 3; i--) yillar.Add(i);
            return Results.Json(new { yil = DateTime.Now.Year, yillar = yillar });
        }

        /// <summary>Eski btnListele_Click: kurum varsayılan kurum kodudur (eski sayfada gizli alan).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? muhasebe, string birim, string eko)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            string kurum = "";
            try { kurum = Temiz(GenelIslemlerIstemci.VarsayilanKurumBul()); } catch (Exception) { }

            string hata = "";
            if ((yil ?? 0) <= 0) hata += "Sorgulama için yıl alanını boş bırakamazsınız.\n";
            if (kurum == "") hata += "Sorgulama için Kurum alanını boş bırakamazsınız.\n";
            if ((muhasebe ?? 0) <= 0) hata += "Sorgulama için Muhasebe alanını boş bırakamazsınız.\n";
            if (hata != "") return Hata(hata.Trim());

            try
            {
                OdenekDurumKriter kr = new OdenekDurumKriter();
                kr.yeniMetot = true;
                kr.yil = yil.Value;
                kr.ay = DateTime.Now.Month;
                kr.muhasebe = muhasebe.Value;
                kr.birim = Temiz(birim);
                kr.ekoKodKirilim = 2;
                kr.eko23 = true;
                kr.harcamaGoster = true;
                kr.odenekGondermeGoster = true;
                kr.odenekGoster = false;
                kr.faaliyetGoster = true;
                kr.harcamaPebli = true;
                kr.detay = ENUMOdenekDurumDetay.BIRIM;

                kr.tertip.kodlar[0] = kurum;
                kr.tertip.kodlar[1] = "";
                kr.tertip.kodlar[2] = "";
                kr.tertip.kodlar[3] = Temiz(eko);
                kr.tertip.kodlar[4] = "";

                IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
                ObjectArray oVAN = servisMUH.HarcamaDurumListesi(kullanan, kr);
                if (oVAN == null || !oVAN.sonuc.islemSonuc)
                    return Hata(Temiz(oVAN?.sonuc.hataStr) == "" ? "Liste okunamadı." : oVAN.sonuc.hataStr);

                List<object> satirlar = new List<object>();
                foreach (TNS.MUH.OdenekDurum o in oVAN.objeler)
                {
                    double onayli = o.onayliOdenekGonderme + o.onayliTenkis; // tenkis eksi geliyor
                    double onaysiz = o.onaysizOdenekGonderme + o.onaysizTenkis;
                    double toplam = onayli + onaysiz;
                    satirlar.Add(new
                    {
                        tertip = o.tertipButce.TertipStrOl(),
                        onaysiz = onaysiz,
                        onayli = onayli,
                        toplam = toplam,
                        harcama = o.harcama,
                        kalan = toplam - o.harcama
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(Temiz(ex.Message) == "" ? "Liste okunamadı." : ex.Message);
            }
        }
    }
}
