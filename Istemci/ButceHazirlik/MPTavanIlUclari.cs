using System;
using System.Collections;
using System.Collections.Generic;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    /// <summary>
    /// İl / İlçe Ödenek Dağıtım ekranının (eski ButceHazirlikv2/MPTavanIlAna.aspx) JavaScript'ten çağırdığı uçlar. İl kullanıcıları
    /// saymanlık / birimlerine ait kurumsal yapının tertiplerini listeler; il izni açık tertiplerde tutara tıklanınca Tavan Girişi
    /// (MPTavanGiris, DG=I) pencerede açılır.
    ///   GET /api/mptavanil/tanim                          bu yıl, kullanıcının "Ödeme Emri/MİF Saymanlık-Birim Yetkisi" saymanlık / birimi
    ///   GET /api/mptavanil/liste?yil&amp;muhasebe&amp;birim&amp;eko&amp;peb   (eski OdenekListele: MPTavanIlListele)
    ///   GET /api/mptavanil/rapor?...&amp;tur=1|2              (eski btnYazdir_Click: 1 Tavan Yazdır "TavanIcm", 2 Detay Yazdır "Tavan")
    /// Kod ağacı ve kod adı /api/mptavan/kodlar, /api/mptavan/kodad uçlarındandır (bu sayfanın yetkisiyle de açıktır).
    /// </summary>
    public static class MPTavanIlUclari
    {
        private const string U = "/api/mptavanil/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(U + "tanim", Tanim);
            app.MapGet(U + "liste", Liste);
            app.MapGet(U + "rapor", Rapor);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return kullanan != null && SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, MPTavanUclari.IlAnaAdres);
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

        /// <summary>Eski Page_Load: yıl bu yıl; saymanlık ve birim kullanıcının "Ödeme Emri/MİF Saymanlık-Birim Yetkisi" değerinden.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int yil = DateTime.Now.Year;
            try
            {
                string muhasebe = "", birim = "", birimAd = "", kurumsal = "";
                string yetki = kullanan.YetkiTipiDegeriBul("Ödeme Emri/MİF Saymanlık-Birim Yetkisi");
                if (!string.IsNullOrEmpty(yetki))
                {
                    string[] yetkiler = yetki.Split('-', ',', ';');
                    muhasebe = TNS.MUH.Arac.DiziElemanAl(yetkiler, 0, "").Trim();
                    birim = TNS.MUH.Arac.DiziElemanAl(yetkiler, 1, "").Trim();
                }
                int muhNo = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0);
                if (muhNo > 0)
                {
                    birimAd = MuhasebeBirimUclari.BirimAdBul(muhNo, birim, out string kurum);
                    kurumsal = MuhasebeBirimUclari.KurumsalYaz(yil, kurum);
                }
                List<int> yillar = new List<int>();
                for (int y = yil + 1; y >= yil - 6; y--) yillar.Add(y);
                return Results.Json(new
                {
                    yil = yil,
                    yillar = yillar,
                    muhasebe = muhNo > 0 ? muhNo.ToString() : "",
                    birim = birim,
                    birimAd = birimAd,
                    kurumsal = kurumsal
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>
        /// Eski KodKYBul: saymanlık / birimin kurumsal yapı kodu. İl özel birimi 260'ta saymanlık 1000'den büyükse il kodundan
        /// "2.ilKod" üretilir; değilse KurumsalYapiListele'nin ilk kaydı.
        /// </summary>
        private static string KodKYBul(IBHMv2Servis servis, string muhasebe, string birim)
        {
            if (birim == "260")
            {
                int saymanlik = OrtakFonksiyonlar.ConvertToInt(muhasebe, 0);
                if (saymanlik > 1000) return "2." + (saymanlik / 1000).ToString("00");
            }
            string[] kyler = servis.KurumsalYapiListele(muhasebe.PadLeft(5, '0'), birim, GenelIslemlerIstemci.VarsayilanKurumBul());
            if (kyler != null && kyler.Length > 0) return kyler[0] ?? "";
            return "";
        }

        /// <summary>
        /// Listele (eski OdenekListele): kurumsal yapının tertipleri, tavan (odenek[0]), birimlere dağıtılan (odenek[1]) ve il izni
        /// (odenek[2] == 1). Toplamlar eskisi gibi sütun başlıklarında gösterilir.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string muhasebe, string birim, string eko, string peb)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            muhasebe = Temiz(muhasebe); birim = Temiz(birim);
            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                string hataStr = "";
                string kodKY = KodKYBul(servis, muhasebe, birim);
                if (string.IsNullOrEmpty(kodKY))
                    hataStr += "Girdiğiniz saymanlık birime ait kurumsal yapı kodu bulunamadı. Lütfen sistem yöneticileriyle irtibata geçiniz.!\n";
                if (yil <= 0)
                    hataStr += "Yıl alanı boş boş bırakılamaz!\n";
                if (hataStr != "") return Hata(hataStr.Trim());

                string sayBir = muhasebe + "-" + birim;
                Tertip tertip = new Tertip(yil, ENUMTabloKod.TABTasraOdenek, GenelIslemlerIstemci.VarsayilanKurumBul(), "", "", Temiz(eko), Temiz(peb), kodKY);
                TertipOdenek[] odnler = servis.MPTavanIlListele(kullanan, tertip, sayBir);
                if (odnler == null) return Hata("Aranan kriterlere uygun tertip bulunamadı!");

                List<object> satirlar = new List<object>();
                double tavanTop = 0, detayTop = 0;
                foreach (TertipOdenek od in odnler)
                {
                    satirlar.Add(new
                    {
                        tertip = od.tertip.TertipStrOl(),
                        tertipAdi = od.tertip.adlar[3] + " (" + od.tertip.adlar[4] + ")",
                        tavan = od.odenek[0],
                        detay = od.odenek[1],
                        ilIzni = od.odenek[2] == 1
                    });
                    tavanTop += od.odenek[0];
                    detayTop += od.odenek[1];
                }
                return Results.Json(new { kykod = kodKY, saybir = sayBir, tavanToplam = tavanTop, detayToplam = detayTop, satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Listeleme yapılamadı." : ex.Message);
            }
        }

        /// <summary>Tavan Yazdır (tur 1, MPRaporDetay "TavanIcm") / Detay Yazdır (tur 2, "Tavan"); eski btnYazdir_Click parametreleri.</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string muhasebe, string birim, string eko, string peb)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl alanı boş boş bırakılamaz!");
            muhasebe = Temiz(muhasebe); birim = Temiz(birim);
            try
            {
                AsposeLisans.Yukle();
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                Hashtable h = new Hashtable();
                h["ButceYil"] = yil.ToString();
                h["KurKod"] = GenelIslemlerIstemci.VarsayilanKurumBul();
                h["FonKod"] = "";
                h["FinKod"] = "";
                h["EkoKod"] = Temiz(eko);
                h["PebKod"] = Temiz(peb);
                h["HrcKod"] = KodKYBul(servis, muhasebe, birim);
                h["SayBir"] = muhasebe + "-" + birim;
                h["KurKodUz"] = "4";
                h["FonKodUz"] = "4";
                h["FinKodUz"] = "1";
                h["EkoKodUz"] = "2";
                h["PebKodUz"] = "4";
                h["HrcKodUz"] = "4";
                string uz = GenelIslemler.ExcelTur();
                MPRaporDetay d = new MPRaporDetay(ortam.ContentRootPath);
                string sonucDosyaAd = d.RaporAl(kullanan, servis, "", "", uz, h, tur == 1 ? "TavanIcm" : "Tavan");
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "Rapor_" + DateTime.Now.Ticks.ToString() + "." + uz, true, uz);
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Rapor hazırlanamadı." : ex.Message);
            }
        }
    }
}
