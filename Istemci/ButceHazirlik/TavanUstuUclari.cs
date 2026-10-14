using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    public class TavanUstuSatir
    {
        public string TertipStr { get; set; }
        public double? EkOdenek0 { get; set; }
        public double? EkOdenek1 { get; set; }
        public double? EkOdenek2 { get; set; }
        public string Gerekce { get; set; }
    }

    public class TavanUstuIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public List<TavanUstuSatir> Talepler { get; set; }
    }

    /// <summary>
    /// Tavan üstü ödenek talebi sayfasının (eski ButceHazirlikv2/TavanUstuOdenek.aspx; bütçe teklifi ekranındaki "Tavan Üstü" tuşu,
    /// ?yil=..&amp;senaryo=..&amp;tertipler=a,b,..) uçları:
    ///   GET  /api/tavanustu/liste   seçilen tertiplerin ödenekleri ve tavan üstü talepleri; tavanına ulaşmamış tasnif satırları uyarısı
    ///   POST /api/tavanustu/kaydet  talepleri (3 yıllık ek ödenek ve gerekçe) saklar
    /// Sayfayı sistem yöneticisi ve gider bütçe teklifi ekranlarının yetkilisi kullanabilir.
    /// </summary>
    public static class TavanUstuUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/tavanustu/liste", Liste);
            app.MapPost("/api/tavanustu/kaydet", Kaydet);
        }

        /// <summary>Eski sayfada yetki denetimi yoktu; burada sistem yöneticisi ya da gider bütçe teklifi ekranlarından birinin yetkilisi.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            foreach (var (tur, tahmin) in new[] { (1, 0), (1, 2), (3, 0), (4, 0) })
                if (ButceRakamUclari.YetkiVar(kullanan, uygulamaKlasoru, tur, tahmin)) return true;
            return false;
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string[] TertipleriAyir(string tertipler)
        {
            List<string> liste = new List<string>();
            foreach (string t in Temiz(tertipler).Split(','))
                if (t.Trim() != "") liste.Add(t.Trim());
            return liste.ToArray();
        }

        /// <summary>Seçilen tertiplerin tavan üstü talepleri (eski Listele) ve tavan uyarısı (eski TavanIslem).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, string tertipler)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            senaryo = Temiz(senaryo);
            string[] tertipDizi = TertipleriAyir(tertipler);
            if (yil < 2000 || senaryo == "") return Hata("Yıl ve aşama boş olamaz!");
            if (tertipDizi.Length == 0) return Hata("Tavan üstü değer gireceğiniz tertipleri seçiniz!");

            IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            List<object> satirlar = new List<object>();
            try
            {
                KistasYapi kistasYapi = new KistasYapi();
                foreach (string tertip in tertipDizi)
                {
                    kistasYapi.ekleTertipler = new Tertip[1];
                    kistasYapi.ekleTertipler[0] = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                    kistasYapi.ekleTertipler[0].TertipAta(tertip);
                    TavanUstuTertipOdenek[] tUstler = servis.TavanUstuOdenekListele(kullanan, new string[] { senaryo }, kistasYapi, false);
                    if (tUstler == null) continue;
                    foreach (TavanUstuTertipOdenek t in tUstler)
                    {
                        satirlar.Add(new
                        {
                            tertipStr = t.tertipStr,
                            tertipAd = t.tertipAd,
                            odenek0 = t.odenek0, ekOdenek0 = t.ekOdenek0,
                            odenek1 = t.odenek1, ekOdenek1 = t.ekOdenek1,
                            odenek2 = t.odenek2, ekOdenek2 = t.ekOdenek2,
                            gerekce = t.gerekce ?? ""
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tavan üstü talepler okunamadı."));
            }

            return Results.Json(new { yil = yil, satirlar = satirlar, uyarilar = TavanIslem(servis, kullanan, yil, tertipDizi) });
        }

        /// <summary>Seçilen tertiplere uyan, alt kırılımı olmayan tasnif satırlarından girilen ödeneği tavanın altında kalanlar (eski TavanIslem).</summary>
        private static List<string> TavanIslem(IBHMv2Servis servis, Kullanici kullanan, int yil, string[] tertipler)
        {
            List<string> uyarilar = new List<string>();
            try
            {
                ButceTasnifOdenekleriFormu form = new ButceTasnifOdenekleriFormu();
                form.yil = yil;
                // Eski BHMv2Ortak.TasnifSiraNoBul(servis, kullanan, 2, kurum, false, yil)
                ButceTasnif[] tasnifler = servis.ButceTasnifListele(kullanan, TNS.DEG.Arac.VarsayilanKurumBul(), 2);
                form.tasnif.siraNo = tasnifler != null && tasnifler.Length > 0 ? tasnifler[0].siraNo : -1;

                form = servis.ButceTasnifOdenekOku(kullanan, form, null, "00.00", null, null);
                foreach (ButceTasnifSatirOdenegi s in form.butceTasnifSatirOdenekleri)
                {
                    if (s.hiyerarsi.altKirilimVar) continue;
                    if (HiyerarsiyeUygunTertipVarMi(s.hiyerarsi, tertipler) && s.ekDegerler[0] > s.ekDegerler[1])
                        uyarilar.Add(string.Format("{0} için girilen ödeneklerin toplamı ({1}) verilen tavanın ({2}) altında.",
                            s.hiyerarsi.ad, s.ekDegerler[1].ToString("#,##0"), s.ekDegerler[0].ToString("#,##0")));
                }
            }
            catch
            {
            }
            return uyarilar;
        }

        private static bool HiyerarsiyeUygunTertipVarMi(ButceTasnifHiyerarsikSatir satir, string[] tertipler)
        {
            if (satir.tertipSablonlar != null && satir.tertipSablonlar.Length > 0)
            {
                foreach (string t in tertipler)
                    if (TertipUyumluMu(satir.tertipSablonlar[0].tertipSablon, t)) return true;
            }
            return false;
        }

        private static bool TertipUyumluMu(string tertipSablon, string kistasTertip)
        {
            string[] sablonBolum = tertipSablon.Split('-');
            string[] kistasTertipBolum = kistasTertip.Split('-');
            if (kistasTertipBolum.Length < sablonBolum.Length) return false;
            for (int j = 0; j < Math.Max(kistasTertipBolum.Length, sablonBolum.Length); j++)
            {
                if (!TertipBolumUyumluMu(kistasTertipBolum, sablonBolum, j)) return false;
            }
            return true;
        }

        private static bool TertipBolumUyumluMu(string[] kistasTertipBolumler, string[] sablonBolumler, int yer)
        {
            string kistasTertipBolum = yer < kistasTertipBolumler.Length ? kistasTertipBolumler[yer] : "";
            string sablonBolum = yer < sablonBolumler.Length ? sablonBolumler[yer] : "";

            if (string.IsNullOrEmpty(sablonBolum)) return true;
            if (kistasTertipBolum == sablonBolum) return true;
            if (kistasTertipBolum.StartsWith(sablonBolum)) return true;

            foreach (string k in kistasTertipBolum.Split(','))
                foreach (string s in sablonBolum.Split(','))
                    if (k.StartsWith(s)) return true;
            return false;
        }

        /// <summary>Tablodaki talepler saklanır (eski Kaydet).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery antiforgery, TavanUstuIstegi istek)
        {
            if (!await antiforgery.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new TavanUstuIstegi();
            string senaryo = Temiz(istek.Senaryo);
            if (istek.Yil < 2000 || senaryo == "") return Hata("Yıl ve aşama boş olamaz!");
            if (istek.Talepler == null || istek.Talepler.Count == 0) return Hata("Kaydedilecek talep yok.");

            try
            {
                List<TavanUstuTertipOdenek> tuto = new List<TavanUstuTertipOdenek>();
                foreach (TavanUstuSatir s in istek.Talepler)
                {
                    if (s == null || Temiz(s.TertipStr) == "") continue;
                    TavanUstuTertipOdenek t = new TavanUstuTertipOdenek();
                    t.tertipStr = Temiz(s.TertipStr);
                    t.ekOdenek0 = s.EkOdenek0 ?? 0;
                    t.ekOdenek1 = s.EkOdenek1 ?? 0;
                    t.ekOdenek2 = s.EkOdenek2 ?? 0;
                    t.gerekce = s.Gerekce ?? "";
                    t.TertipOl(istek.Yil, ENUMTabloKod.TABPEBOdenek);
                    tuto.Add(t);
                }
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().TavanUstuOdenekKaydet(kullanan, senaryo, tuto.ToArray());
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }
    }
}
