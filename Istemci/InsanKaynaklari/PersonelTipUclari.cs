using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Kaydet isteği: alan adları PersonelTipi sınıfındaki gibi.</summary>
    public class PersonelTipIstegi
    {
        public int kod { get; set; }
        public string ad { get; set; }
        public int sira { get; set; }
        public string grupAd { get; set; }
        public string maasGrupAd { get; set; }
        public int HITAP { get; set; }
        public int maasDonemGun { get; set; }
        public int maasOdemeGun { get; set; }
        public int kadrolu { get; set; }
        public int kurumDisi { get; set; }
        public int goster { get; set; }
        public int aktif { get; set; }
        public int terfiyeDahil { get; set; }
        public int maasaDahil { get; set; }
        public int hitapMi { get; set; }
        public int pesinMi { get; set; }
        public int H1 { get; set; }
        public int H2 { get; set; }
        public int H3 { get; set; }
        public int H4 { get; set; }
        public int H5 { get; set; }
        public int H6 { get; set; }
        public int H7 { get; set; }
    }

    /// <summary>
    /// Personel Tipleri (eski InsanKaynaklari/PersonelTipForm.aspx; yeni sayfa InsanKaynaklari/PersonelTipForm) uçları:
    ///   GET  /api/personeltip/liste        tüm personel tipleri + hizmet grupları
    ///   GET  /api/personeltip/oku?kod=     tek personel tipi (eski Oku)
    ///   POST /api/personeltip/kaydet       (eski Kaydet)
    /// İş kuralları eski sayfadaki gibi IIKYServis.ListelePersonelTip / OkuPersonelTip / KaydetPersonelTip.
    /// </summary>
    public static class PersonelTipUclari
    {
        public const string SayfaAdresi = "../InsanKaynaklari/PersonelTipForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personeltip/liste", Liste);
            app.MapGet("/api/personeltip/oku", Oku);
            app.MapPost("/api/personeltip/kaydet", Kaydet);
        }

        /// <summary>Eski Page_Load: GenelIslemler.SayfayaGirisYapabilirMi (menü yetkisi).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static object Satir(PersonelTipi p)
        {
            return new
            {
                p.kod, p.ad, p.sira, p.grupAd, p.maasGrupAd,
                HITAP = (int)p.HITAP,
                p.maasDonemGun, p.maasOdemeGun,
                p.kadrolu, p.kurumDisi, p.goster, p.aktif, p.terfiyeDahil, p.maasaDahil, p.hitapMi, p.pesinMi,
                p.H1, p.H2, p.H3, p.H4, p.H5, p.H6, p.H7
            };
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> gruplar = new List<object>();
            foreach (IntKodStrAd g in TNS.IKY.Arac.HizmetGrubuListe)
                gruplar.Add(new { g.kod, g.ad });

            ObjectArray oa = TNS.IKY.Arac.Tanimla().ListelePersonelTip(kullanan, new PersonelTipi());
            if (!oa.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(oa.sonuc, "Personel tipleri okunamadı."), hizmetGruplari = gruplar });

            List<object> satirlar = new List<object>();
            foreach (PersonelTipi p in oa.objeler)
                satirlar.Add(Satir(p));
            return Results.Json(new { satirlar = satirlar, hizmetGruplari = gruplar });
        }

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            PersonelTipi form = new PersonelTipi();
            form.kod = kod ?? 0;
            ObjectVAN ov = TNS.IKY.Arac.Tanimla().OkuPersonelTip(kullanan, form);
            if (!ov.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(ov.sonuc, "Personel tipi okunamadı.") });
            if (ov.o == null) return Results.Json(new { bilgi = "Verilen kriterlere uygun kayıt bulunamadı." });
            return Results.Json(new { kayit = Satir((PersonelTipi)ov.o) });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelTipIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            PersonelTipi form = new PersonelTipi();
            form.kod = istek.kod;
            form.ad = istek.ad ?? "";
            form.kurumDisi = istek.kurumDisi == 1 ? 1 : 0;
            form.sira = istek.sira;
            form.kadrolu = istek.kadrolu == 1 ? 1 : 0;
            form.goster = istek.goster == 1 ? 1 : 0;
            form.grupAd = istek.grupAd ?? "";
            form.maasGrupAd = istek.maasGrupAd ?? "";
            form.aktif = istek.aktif == 1 ? 1 : 0;
            form.terfiyeDahil = istek.terfiyeDahil == 1 ? 1 : 0;
            form.maasaDahil = istek.maasaDahil == 1 ? 1 : 0;
            form.maasDonemGun = istek.maasDonemGun;
            form.maasOdemeGun = istek.maasOdemeGun;
            form.hitapMi = istek.hitapMi == 1 ? 1 : 0;
            form.pesinMi = istek.pesinMi == 1 ? 1 : 0;
            form.HITAP = (ENUMHizmetTablolari)istek.HITAP;
            form.H1 = istek.H1;
            form.H2 = istek.H2;
            form.H3 = istek.H3;
            form.H4 = istek.H4;
            form.H5 = istek.H5;
            form.H6 = istek.H6;
            form.H7 = istek.H7;

            TNS.IKY.Arac.PersonelTipSifirla();
            Sonuc sonuc = TNS.IKY.Arac.Tanimla().KaydetPersonelTip(kullanan, form);
            if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc, "Personel tipi kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti.", kod = form.kod });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = (sonuc.hataStr ?? "").Replace("<br>", " ").Replace("<br/>", " ");
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata.Trim();
        }
    }
}
