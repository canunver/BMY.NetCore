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
using TNS.BKD;
using TNS.HRC;
using TNS.KYM;

namespace Istemci.HarcamaSurecleri
{
    /// <summary>Formdaki alanlar (eski KriterTopla): Bul'da kriter, Kaydet'te kayıt olarak kullanılır.</summary>
    public class UnvanGundelikIstek
    {
        public int kod { get; set; }
        public int yil { get; set; }
        public int unvanKod { get; set; }
        public string ulkeKod { get; set; }
        public int tur { get; set; }
        public double gundelikTutar { get; set; }
        public int paraBirimKod { get; set; }
    }

    /// <summary>
    /// Yıl, Unvan ve Ülke Kriterine Göre Gündelik Tanımlama (eski HarcamaSurecleri/TanimGundelik.aspx;
    /// yeni sayfa HarcamaSurecleri/TanimGundelik) uçları:
    ///   GET  /api/unvangundelik/baslangic  unvanlar (+ Diğer), türler, ülkeler, para birimleri, yıl (eski Page_Load doldurmaları)
    ///   POST /api/unvangundelik/listele    { yil } yılın gündelikleri (eski Listele)
    ///   POST /api/unvangundelik/bul        formdaki alanlar kriter (eski btnAra_Click)
    ///   POST /api/unvangundelik/kaydet     formdaki alanlar + seçili kod (eski btnKaydet_Click)
    ///   POST /api/unvangundelik/sil        { kod } (eski btnSil_Click)
    ///   POST /api/unvangundelik/kopyala    { yil } yılın gündeliklerini sonraki yıla kopyalar (eski btnSonrakiYilaKopyala_Click)
    /// İş kuralları eski sayfadaki gibi IHRCServis.Gundelik* / ParaBirimiListele, IIKYServis.UnvanListele, IKYMServis.ListeleCografya.
    /// </summary>
    public static class UnvanGundelikUclari
    {
        public const string SayfaAdresi = "../HarcamaSurecleri/TanimGundelik.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/unvangundelik/baslangic", Baslangic);
            app.MapPost("/api/unvangundelik/listele", Listele);
            app.MapPost("/api/unvangundelik/bul", Bul);
            app.MapPost("/api/unvangundelik/kaydet", Kaydet);
            app.MapPost("/api/unvangundelik/sil", Sil);
            app.MapPost("/api/unvangundelik/kopyala", Kopyala);
        }

        /// <summary>Sayfa yetkisi (eski GenelIslemler.SayfayaGirisYapabilirMi).</summary>
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

        private static async Task<IResult> DenetlePost(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, object istek, Action<Kullanici> kullaniciAta)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            kullaniciAta(kullanan);
            return null;
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = (sonuc.hataStr ?? "").Replace("<br>", " ").Replace("<br/>", " ").Replace("<br />", " ").Trim();
            return hata == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata;
        }

        /// <summary>Eski TurAdGetir.</summary>
        private static string TurAdi(ENUMGundelikTur tur)
        {
            switch (tur)
            {
                case ENUMGundelikTur.GIDIS: return "Gidiş";
                case ENUMGundelikTur.GELIS: return "Geliş";
                case ENUMGundelikTur.YURTICI: return "Yurt İçi";
            }
            return "-";
        }

        /// <summary>Eski KriterTopla.</summary>
        private static Gundelik Topla(UnvanGundelikIstek istek)
        {
            Gundelik g = new Gundelik();
            g.yil = istek.yil;
            g.unvanKod = istek.unvanKod;
            g.ulkeKod = (istek.ulkeKod ?? "").Trim();
            g.gundelikTutar = istek.gundelikTutar;
            g.paraBirimi.kod = istek.paraBirimKod;
            g.tur = (ENUMGundelikTur)istek.tur;
            return g;
        }

        private static IResult Baslangic(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski UnvanDoldur: aktif unvanlar + "Diğer" (kod 0)
            List<object> unvanlar = new List<object>();
            ObjectArray dler = TNS.IKY.Arac.Tanimla().UnvanListele(new TNS.IKY.Unvan { aktif = true });
            if (dler.sonuc.islemSonuc)
                foreach (TNS.IKY.Unvan u in dler.objeler)
                    unvanlar.Add(new { kod = u.UnvanKod, ad = u.UnvanAd });
            unvanlar.Add(new { kod = 0, ad = "Diğer" });

            List<object> turler = new List<object>();
            foreach (ENUMGundelikTur t in new[] { ENUMGundelikTur.GIDIS, ENUMGundelikTur.GELIS, ENUMGundelikTur.YURTICI })
                turler.Add(new { kod = (int)t, ad = TurAdi(t) });

            List<object> ulkeler = new List<object>();
            Cografya[] cler = TNS.KYM.Arac.Tanimla().ListeleCografya(kullanan, new Cografya() { ulkeMi = true }) ?? new Cografya[0];
            foreach (Cografya c in cler)
                ulkeler.Add(new { kod = c.kod ?? "", ad = c.ad ?? "" });

            List<object> paraBirimleri = new List<object>();
            ObjectArray paralar = TNS.HRC.Arac.Tanimla().ParaBirimiListele(kullanan, new ParaBirimi());
            if (paralar.sonuc.islemSonuc)
                foreach (ParaBirimi p in paralar.objeler)
                    paraBirimleri.Add(new { p.kod, p.kisaltma });

            int buYil = DateTime.Now.Year;
            List<int> yillar = new List<int>();
            for (int i = 2005; i <= buYil + 1; i++) yillar.Add(i);

            return Results.Json(new { yil = buYil, yillar, unvanlar, turler, ulkeler, paraBirimleri });
        }

        /// <summary>Eski Listele: liste her zaman para birimi süzülmeden okunur; "ara" ise kayıt yoksa bilgi verilir.</summary>
        private static IResult Liste(Kullanici kullanan, Gundelik kriter, bool ara)
        {
            kriter.paraBirimi.kod = 0;
            ObjectArray yler = TNS.HRC.Arac.Tanimla().GundelikListele(kullanan, kriter);
            if (!yler.sonuc.islemSonuc) return Hata(HataMetni(yler.sonuc, "Gündelik bilgileri okunamadı."));

            List<object> satirlar = new List<object>();
            foreach (Gundelik p in yler.objeler)
            {
                satirlar.Add(new
                {
                    p.kod,
                    p.yil,
                    p.unvanKod,
                    unvanAd = p.unvanKod == 0 ? "Diğer" : p.unvanAd,
                    p.ulkeKod,
                    p.ulkeAd,
                    p.gundelikTutar,
                    paraBirimKod = p.paraBirimi.kod,
                    paraBirimAd = p.paraBirimi.kisaltma,
                    turKod = (int)p.tur,
                    turAd = TurAdi(p.tur)
                });
            }
            return Results.Json(new
            {
                basarili = true,
                satirlar,
                bilgi = ara && satirlar.Count == 0 ? "Verilen kriterlere uygun kayıt bulunamadı." : null
            });
        }

        private static async Task<IResult> Listele(HttpContext baglam, IWebHostEnvironment ortam, UnvanGundelikIstek istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, istek, k => kullanan = k);
            if (ret != null) return ret;
            return Liste(kullanan, new Gundelik() { yil = istek.yil > 0 ? istek.yil : DateTime.Now.Year }, false);
        }

        private static async Task<IResult> Bul(HttpContext baglam, IWebHostEnvironment ortam, UnvanGundelikIstek istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, istek, k => kullanan = k);
            if (ret != null) return ret;
            return Liste(kullanan, Topla(istek), true);
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, UnvanGundelikIstek istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, istek, k => kullanan = k);
            if (ret != null) return ret;

            Gundelik kriter = Topla(istek);
            kriter.kod = istek.kod;
            Sonuc sonuc = TNS.HRC.Arac.Tanimla().GundelikKaydet(kullanan, kriter);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kayıt işlemi yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = string.IsNullOrWhiteSpace(sonuc.bilgiStr) ? "Kayıt işlemi başarıyla tamamlandı." : sonuc.bilgiStr, kod = kriter.kod });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, UnvanGundelikIstek istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, istek, k => kullanan = k);
            if (ret != null) return ret;
            if (istek.kod <= 0) return Hata("Lütfen silmek istediğiniz kaydı listeden seçiniz.");

            Sonuc sonuc = TNS.HRC.Arac.Tanimla().GundelikSil(kullanan, new Gundelik { kod = istek.kod });
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Silme işlemi yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla tamamlandı." });
        }

        private static async Task<IResult> Kopyala(HttpContext baglam, IWebHostEnvironment ortam, UnvanGundelikIstek istek, IAntiforgery sahtecilik)
        {
            Kullanici kullanan = null;
            IResult ret = await DenetlePost(baglam, ortam, sahtecilik, istek, k => kullanan = k);
            if (ret != null) return ret;

            Gundelik kriter = new Gundelik { yil = istek.yil };
            Sonuc sonuc = TNS.HRC.Arac.Tanimla().GundelikSonrakiYilaKopyala(kullanan, kriter);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kopyalama yapılamadı."));
            return Results.Json(new
            {
                basarili = true,
                bilgi = string.IsNullOrWhiteSpace(sonuc.bilgiStr) ? "Kopyalama işlemi tamamlandı." : sonuc.bilgiStr,
                yil = kriter.yil + 1
            });
        }
    }
}
