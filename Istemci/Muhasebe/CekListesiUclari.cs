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
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>İptal çek kaydı (eski wndIptal: btnKaydet_Click) ve iptal bilgisi silme (eski IptalCekSil).</summary>
    public class CekListesiIstegi
    {
        public string hesapKod { get; set; }
        public string cekNo { get; set; }
        /// <summary>yyyy-MM-dd</summary>
        public string iptalTarihi { get; set; }
        public string iptalNedeni { get; set; }
    }

    /// <summary>
    /// Çek Listesi (eski ButceMuhasebe/CekKocaniForm.aspx) uçları. İş kuralları servisi (IMUHServis.*CekKocani) değişmeden çağrılır.
    ///   GET  /api/ceklistesi/liste?hesapKod&amp;cekNo&amp;iptalli&amp;acik&amp;bozdurulmus&amp;basla&amp;bitis   eski btnOku_Click (ListeleCekKocani)
    ///   GET  /api/ceklistesi/yazdir?(aynı kıstaslar)    eski btnYazdir_Click (MUH/CekListe.xltx, Excel)
    ///   POST /api/ceklistesi/kaydet                     eski btnKaydet_Click (KaydetCekKocani): iptal çek ekle
    ///   POST /api/ceklistesi/sil                        eski IptalCekSil (SilCekKocani): iptal bilgisini sil
    /// Yetki: menüdeki sayfa adresi.
    /// </summary>
    public static class CekListesiUclari
    {
        public const string SayfaAdres = "../ButceMuhasebe/CekKocaniForm.aspx";
        private const string Uc = "/api/ceklistesi/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "yazdir", Yazdir);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapPost(Uc + "sil", Sil);
        }

        public static bool SayfaYetkili(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!SayfaYetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { hata = Duz(hata) }); }

        private static string Duz(string s)
        {
            return (s ?? "").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n").Trim();
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("   at ")) return varsayilan;
            return Duz(hata);
        }

        private static string Q(HttpContext baglam, string ad)
        {
            return ((string)baglam.Request.Query[ad] ?? "").Trim();
        }

        private static bool Evet(HttpContext baglam, string ad)
        {
            string d = Q(baglam, ad);
            return d == "1" || d.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Boş tarih eski DateField'ın boş değeri (DateTime.MinValue) olarak gider.</summary>
        private static TNSDateTime Tarih(string iso)
        {
            TNSDateTime t = OdemeEmriMIFUclari.Tarih(iso);
            return t.isNull ? new TNSDateTime(DateTime.MinValue) : t;
        }

        private static string TarihYaz(TNSDateTime t)
        {
            return TNSDateTime.IsNull(t) ? "" : OdemeEmriMIFUclari.TarihYaz(t);
        }

        // ------------------------------------------------------------------ açılış (eski Page_Load)

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            DateTime bugun = DateTime.Now.Date;
            return Results.Json(new
            {
                basla = new DateTime(bugun.Year, 1, 1).ToString("yyyy-MM-dd"),
                bitis = bugun.ToString("yyyy-MM-dd"),
                bugun = bugun.ToString("yyyy-MM-dd")
            });
        }

        // ------------------------------------------------------------------ listele (eski ListeAl / Oku)

        private static ObjectArray ListeAl(HttpContext baglam, Kullanici kullanan)
        {
            CekKocani form = new CekKocani();
            form.hesapKod = Q(baglam, "hesapKod");
            form.cekNo = Q(baglam, "cekNo");
            return Servis().ListeleCekKocani(kullanan, form, Evet(baglam, "acik"), Evet(baglam, "bozdurulmus"), Evet(baglam, "iptalli"),
                Tarih(Q(baglam, "basla")), Tarih(Q(baglam, "bitis")));
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray oa = ListeAl(baglam, kullanan);
                if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (CekKocani item in oa.objeler)
                {
                    satirlar.Add(new
                    {
                        hesapKod = item.hesapKod ?? "",
                        cekNo = item.cekNo ?? "",
                        kayitTarihi = TarihYaz(item.kayitTarihi),
                        gun = TarihYaz(item.gun),
                        iptalTarihi = TarihYaz(item.iptalTarihi),
                        iptalNedeni = item.iptalNedeni ?? "",
                        mifBelgeNo = item.mifBelgeNo ?? "",
                        tcVno = item.tcVno ?? "",
                        ilgili = item.ilgili ?? "",
                        tutar = item.tutar,
                        odemeEmriNo = item.odemeEmriNo ?? "",
                        tahsilTarihi = TarihYaz(item.tahsilTarihi),
                        cekDurumAd = item.cekDurumAd ?? "",
                        iptalli = item.cekDurum == CekKocani.CekDurum.Iptalli
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste okunamadı."));
            }
        }

        // ------------------------------------------------------------------ yazdır (eski btnYazdir_Click)

        private static string SablonYol(string sablonAd)
        {
            string kurum = Path.Combine(Ayar.Oku("RaporSablonYol") ?? "", "MUH", sablonAd);
            if (File.Exists(kurum)) return kurum;
            foreach (string kok in new string[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                string uygulama = Path.Combine(kok, "RaporSablon", "MUH", sablonAd);
                if (File.Exists(uygulama)) return uygulama;
            }
            return kurum;
        }

        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray oa = ListeAl(baglam, kullanan);
                if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "Liste okunamadı."));
                string sablon = SablonYol("CekListe.xltx");
                if (!File.Exists(sablon)) return Hata("Şablon (MUH/CekListe.xltx) bulunamadı.");

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("CekListe", "") + ".xlsx";
                XLS.DosyaAc(sablon, sonucDosyaAd);
                int fsheetno = -1;
                int fsatir1 = -1;
                int fsutun1 = -1;
                int fsatir2 = -1;
                int fsutun2 = -1;
                int baslaSatir = -1;
                int sutun = -1;
                XLS.HucreAdAdresCoz("FormatSatir", ref fsheetno, ref fsatir1, ref fsutun1, ref fsatir2, ref fsutun2);
                XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
                int yuks = XLS.SatirYukseklikAl(baslaSatir);
                int satir = baslaSatir;
                foreach (CekKocani item in oa.objeler)
                {
                    XLS.HucreKopyala(fsatir1, fsutun1, fsatir2, fsutun2, satir, sutun);
                    int sut = sutun;
                    XLS.HucreDegerYaz(satir, sut++, item.hesapKod ?? "");
                    XLS.HucreDegerYaz(satir, sut++, item.cekNo ?? "");
                    if (!TNSDateTime.IsNull(item.gun))
                        XLS.HucreDegerYaz(satir, sut++, item.gunDT);
                    else
                        sut++;
                    XLS.HucreDegerYaz(satir, sut++, item.tcVno ?? "");
                    XLS.HucreDegerYaz(satir, sut++, item.ilgili ?? "");
                    XLS.HucreDegerYaz(satir, sut++, item.mifBelgeNo ?? "");
                    XLS.HucreDegerYaz(satir, sut++, item.tutar);
                    XLS.HucreDegerYaz(satir, sut++, item.odemeEmriNo ?? "");
                    if (!TNSDateTime.IsNull(item.tahsilTarihi))
                        XLS.HucreDegerYaz(satir, sut++, item.tahsilTarihiDT);
                    else
                        sut++;
                    if (!TNSDateTime.IsNull(item.iptalTarihi))
                        XLS.HucreDegerYaz(satir, sut++, item.iptalTarihiDT);
                    else
                        sut++;
                    XLS.HucreDegerYaz(satir, sut++, item.iptalNedeni ?? "");
                    XLS.HucreDegerYaz(satir, sut++, item.cekDurumAd ?? "");

                    satir++;
                }
                XLS.SatirYukseklikAyarla(baslaSatir, satir - 1, -1, 7, yuks);
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, Path.GetFileName(sonucDosyaAd), true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ iptal çek ekle (eski Kaydet)

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, CekListesiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            try
            {
                CekKocani form = new CekKocani();
                form.hesapKod = (istek.hesapKod ?? "").Trim();
                form.cekNo = (istek.cekNo ?? "").Trim();
                form.kayitTarihiDT = DateTime.Now;
                form.iptalTarihiDT = Tarih(istek.iptalTarihi).Oku();
                form.iptalNedeni = istek.iptalNedeni ?? "";
                Sonuc sonuc = Servis().KaydetCekKocani(kullanan, form);
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Kaydedilemedi."));
                return Results.Json(new { basarili = true, bilgi = "İptal çek bilgisi kaydedildi." });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kaydedilemedi."));
            }
        }

        // ------------------------------------------------------------------ iptal bilgisini sil (eski IptalCekSil)

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, CekListesiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            try
            {
                CekKocani form = new CekKocani();
                form.hesapKod = istek.hesapKod ?? "";
                form.cekNo = istek.cekNo ?? "";
                Sonuc sonuc = Servis().SilCekKocani(kullanan, form);
                if (!sonuc.islemSonuc) return Hata("İptal bilgisi silinemedi:" + HataMetni(sonuc.hataStr, ""));
                return Results.Json(new { basarili = true, bilgi = "İptal bilgisi kaldırıldı." });
            }
            catch (Exception ex)
            {
                return Hata("İptal bilgisi silinemedi:" + HataMetni(ex.Message, ""));
            }
        }
    }
}
