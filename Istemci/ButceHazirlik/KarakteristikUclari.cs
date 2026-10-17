using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    public class KarakteristikSatiri
    {
        public string Sira { get; set; }
        public string Deger { get; set; }
        public string DegerKisa { get; set; }
        /// <summary>0: ek girilmesin, 1: arkaya ek (zorunlu), 2: öne ek (zorunlu), 3: arkaya ek, 4: öne ek.</summary>
        public int EkTurKod { get; set; }
    }

    public class KarakteristikKayitIstegi
    {
        public List<KarakteristikSatiri> Satirlar { get; set; }
    }

    /// <summary>
    /// Karakteristikler sayfasının (eski ButceHazirlikv2/KarakteristikForm.aspx) uçları: yatırım projelerinde seçilen karakteristik
    /// değerlerinin tanımları.
    ///   GET  /api/karakteristik/liste
    ///   POST /api/karakteristik/kaydet   bütün liste tablodakilerle değiştirilir (iş kuralı; yalnızca sistem yöneticisi yazar)
    /// </summary>
    public static class KarakteristikUclari
    {
        public const string SayfaAdresi = ButceTanimOrtak.Klasor + "KarakteristikForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/karakteristik/liste", Liste);
            app.MapPost("/api/karakteristik/kaydet", Kaydet);
        }

        /// <summary>Listele (eski Oku).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            try
            {
                KarakteristikGiris[] liste = TNS.YAT.Arac.Tanimla().KarekteristikListele(kullanan) ?? new KarakteristikGiris[0];
                List<object> satirlar = new List<object>();
                foreach (KarakteristikGiris k in liste)
                    satirlar.Add(new { sira = k.sira ?? "", deger = k.deger ?? "", degerKisa = k.degerKisa ?? "", ekTurKod = k.ekTurKod });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Liste okunamadı.");
            }
        }

        /// <summary>Kaydet (eski btnKaydet_Click): tablodaki satırlar sırasıyla gönderilir; bütün alanları boş satırlar atlanır.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, KarakteristikKayitIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAdresi);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            try
            {
                List<KarakteristikGiris> liste = new List<KarakteristikGiris>();
                foreach (KarakteristikSatiri s in istek.Satirlar ?? new List<KarakteristikSatiri>())
                {
                    if (s == null) continue;
                    KarakteristikGiris k = new KarakteristikGiris();
                    k.sira = ButceTanimOrtak.Temiz(s.Sira);
                    k.deger = ButceTanimOrtak.Temiz(s.Deger);
                    k.degerKisa = ButceTanimOrtak.Temiz(s.DegerKisa);
                    k.ekTurKod = s.EkTurKod >= 0 && s.EkTurKod <= 4 ? s.EkTurKod : 0;
                    if (k.sira + k.deger + k.degerKisa == "") continue;
                    liste.Add(k);
                }
                Sonuc sonuc = TNS.YAT.Arac.Tanimla().KarekteristikKaydet(kullanan, liste.ToArray());
                return ButceTanimOrtak.SonucYaz(sonuc, "Kayıt işlemi başarıyla gerçekleşti.", "Kayıt işlemi başarısız.");
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Kayıt işlemi başarısız.");
            }
        }
    }
}
