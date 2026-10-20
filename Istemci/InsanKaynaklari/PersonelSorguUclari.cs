using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>
    /// Personel arama penceresi (eski InsanKaynaklari/PersonelSorgu.aspx; yeni sayfa InsanKaynaklari/PersonelSorgu). Kart sekmesinde
    /// "Zimmet Eden Personel" / "İade Alan Personel" seçimi için açılır. Uçlar:
    ///   GET /api/personelbilgi/personelsorgu/unvanlar          (eski Genel.UnvanDoldur)
    ///   GET /api/personelbilgi/personelsorgu/birimler?ust=     (eski trpBirim + OrgBirimDoldur / ORGBirim.NodeLoad: birim ağacının bir düzeyi)
    ///   GET /api/personelbilgi/personelsorgu/liste?genelSorgu=&amp;birim=&amp;unvan=   (eski PersonelListele)
    /// İş kuralları eski sayfadaki gibi IIKYServis.UnvanListele / PersonelListele, IORGServis.OrganizasyonBirimListele.
    /// </summary>
    public static class PersonelSorguUclari
    {
        private const string SayfaAd = "PersonelSorgu";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/personelbilgi/personelsorgu/unvanlar", Unvanlar);
            app.MapGet("/api/personelbilgi/personelsorgu/birimler", Birimler);
            app.MapGet("/api/personelbilgi/personelsorgu/liste", Liste);
        }

        /// <summary>Eski Genel.UnvanDoldur: bütün unvanlar, başta boş seçenek; ad "Unvan (sınıf kısa adı)".</summary>
        private static IResult Unvanlar(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> unvanlar = new List<object>();
            unvanlar.Add(new { UnvanKod = 0, UnvanAd = "" });
            ObjectArray oa = TNS.IKY.Arac.Tanimla().UnvanListele(new Unvan());
            if (oa == null || !oa.sonuc.islemSonuc)
                return Results.Json(new { unvanlar = unvanlar, hata = PersonelBilgiOrtak.HataMetni(oa?.sonuc.hataStr, "Unvanlar okunamadı.") });
            foreach (Unvan u in oa.objeler)
                unvanlar.Add(new { u.UnvanKod, u.UnvanAd });
            return Results.Json(new { unvanlar = unvanlar });
        }

        /// <summary>
        /// Birim ağacının bir düzeyi. Eski sayfa ağacı ORGBirim.NodeLoad'a { ozellikAra: false, yetkiliOlduguMuhasebeBirim:
        /// kullanıcının "Personel Saymanlık-Birim Yetkisi" } ile okutuyordu: kökte bu yetkideki birimler (yetki tanımlı değilse
        /// bütün kök birimler), alt düzeylerde seçilen birimin altındakiler.
        /// </summary>
        private static IResult Birimler(HttpContext baglam, IWebHostEnvironment ortam, int? ust)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                TNS.ORG.OrganizasyonBirim kriter = new TNS.ORG.OrganizasyonBirim();
                kriter.anaBirimKod = Math.Max(0, ust ?? 0);
                kriter.sablonMu = false;
                if (kriter.anaBirimKod == 0)
                {
                    string yetkili = kullanan.YetkiTipiDegeriBul("Personel Saymanlık-Birim Yetkisi") ?? "";
                    bool birimYetkisiVar = false;
                    foreach (string y in yetkili.Split(';'))
                    {
                        string[] m = y.Split('-');
                        if (m.Length == 2 && (OrtakFonksiyonlar.ConvertToInt(m[0], 0) > 0 || OrtakFonksiyonlar.ConvertToInt(m[1], 0) > 0))
                            birimYetkisiVar = true;
                    }
                    if (birimYetkisiVar) kriter.yetkiliOlduguMuhasebeBirim = yetkili;
                    else kriter.kopruKod = "999";
                }

                ObjectArray<TNS.ORG.OrganizasyonBirim> sonuc = TNS.ORG.Arac.Tanimla().OrganizasyonBirimListele(kriter, false, false);
                if (sonuc == null || !sonuc.sonuc.islemSonuc) return Results.Json(new { hata = "Birimler okunamadı." });
                List<object> satirlar = new List<object>();
                foreach (TNS.ORG.OrganizasyonBirim b in sonuc.objeler)
                    satirlar.Add(new { kod = b.kod, ad = b.adi ?? "", altVar = b.altDalVar });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Birimler okunamadı." });
            }
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string genelSorgu, int? birim, int? unvan)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski PersonelListele
            TNS.IKY.Personel p = new TNS.IKY.Personel();
            p.genelSorgu = genelSorgu ?? "";
            p.kadro.organizasyonKodu = birim ?? 0;
            p.kadro.unvan = unvan ?? 0;

            ObjectArray liste = TNS.IKY.Arac.Tanimla().PersonelListele(kullanan, p, false, true);
            if (liste == null || !liste.sonuc.islemSonuc)
                return Results.Json(new { satirlar = new List<object>(), hata = PersonelBilgiOrtak.HataMetni(liste?.sonuc.hataStr, "Personel listesi okunamadı.") });

            List<object> satirlar = new List<object>();
            foreach (TNS.IKY.Personel per in liste.objeler)
                satirlar.Add(new
                {
                    KOD = per.kod,
                    ADSOYAD = per.ad + ' ' + per.soyad,
                    TCKIMLIKNO = per.tcKimlikNo,
                    SICILNO = per.sicilNo,
                    BIRIM = per.kadro.birimAd,
                    UNVAN = per.kadro.unvanAd,
                    KADRODERECE = per.kadro.kadroDerece,
                    KADROTARIHI = PersonelBilgiOrtak.TarihYaz(per.kadro.kadroTarih),
                    KADRODURUMU = per.kadro.kadroDurumAd,
                    PERSONELTUR = per.personelTipAd
                });

            string bilgi = satirlar.Count == 0 ? PersonelBilgiOrtak.Duz(liste.sonuc.bilgiStr) : "";
            return Results.Json(new { satirlar = satirlar, bilgi = bilgi });
        }
    }
}
