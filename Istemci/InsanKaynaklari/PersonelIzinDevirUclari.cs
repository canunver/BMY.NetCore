using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Izin devir gridinin bir satırı (eski grdListe.getRowsValues alanları).</summary>
    public class PersonelIzinDevirSatiri
    {
        public int kod { get; set; }
        public int personelKod { get; set; }
        public int yil { get; set; }
        public int devreden { get; set; }
        public int yilici { get; set; }
    }

    /// <summary>Kaydet isteği: sayfanın personeli ve değişen satırlar (eski dirtyRowsOnly).</summary>
    public class PersonelIzinDevirKayitIstegi
    {
        public int personelKod { get; set; }
        public List<PersonelIzinDevirSatiri> satirlar { get; set; }
    }

    /// <summary>Devir silme isteği (eski DevirSil(kod, yil)).</summary>
    public class PersonelIzinDevirSilIstegi
    {
        public int personelKod { get; set; }
        public int kod { get; set; }
        public int yil { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri "İzin Devir" sekmesi (eski InsanKaynaklari/PersonelIzinDevirTab.aspx; yeni sayfa InsanKaynaklari/PersonelIzinDevirTab) uçları:
    ///   GET  /api/personelbilgi/izindevir/liste?personelKod=   (eski Listele: PersonelIzinDevirListele(kullanan, pid, 0))
    ///   POST /api/personelbilgi/izindevir/kaydet               (eski btnKaydet_Click: değişen satırlar PersonelIzinDevirKaydet'e)
    ///   POST /api/personelbilgi/izindevir/sil                  (eski DevirSil: satırdaki sil komutu)
    /// </summary>
    public static class PersonelIzinDevirUclari
    {
        private const string SayfaAd = "PersonelIzinDevirTab";
        private const string Onek = "/api/personelbilgi/izindevir";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Onek + "/liste", Liste);
            app.MapPost(Onek + "/kaydet", Kaydet);
            app.MapPost(Onek + "/sil", Sil);
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            // Personel kodu 0 iken iş kuralı bütün personelin devirlerini yetki denetimsiz listeliyor (NOTLAR.md); sekme hep bir personel içindir.
            if (personelKod <= 0) return Results.Json(new { satirlar = new List<object>() });

            TNS.IKY.PersonelIzinDevir pid = new TNS.IKY.PersonelIzinDevir();
            pid.personelKod = personelKod;
            ObjectArray pler = TNS.IKY.Arac.Tanimla().PersonelIzinDevirListele(kullanan, pid, 0);
            if (!pler.sonuc.islemSonuc)
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(pler.sonuc.hataStr, "İzin devir bilgileri okunamadı.") });

            List<object> satirlar = new List<object>();
            foreach (TNS.IKY.PersonelIzinDevir d in pler.objeler)
                satirlar.Add(new { d.kod, d.personelKod, d.yil, yilAd = d.yilAd, d.devreden, d.yilici });
            return Results.Json(new { satirlar = satirlar });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelIzinDevirKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string hata = "";
            if (istek.personelKod == 0)
                hata += "Personel bilgisi olmadığından kayıt yapamazsınız.";
            if (hata != "") return Results.Json(new { basarili = false, hata = hata });

            List<TNS.IKY.PersonelIzinDevir> liste = new List<TNS.IKY.PersonelIzinDevir>();
            foreach (PersonelIzinDevirSatiri s in istek.satirlar ?? new List<PersonelIzinDevirSatiri>())
            {
                if (s == null) continue;
                liste.Add(new TNS.IKY.PersonelIzinDevir { kod = s.kod, personelKod = s.personelKod, yil = s.yil, devreden = s.devreden, yilici = s.yilici });
            }

            Sonuc sonuc;
            if (liste.Count > 0)
                sonuc = TNS.IKY.Arac.Tanimla().PersonelIzinDevirKaydet(kullanan, liste.ToArray());
            else
                sonuc = new Sonuc(false, "Kayıt edilecek veri bulunamadı.", "");
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "İzin devir bilgileri kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarılı" });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, PersonelIzinDevirSilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            string hata = "";
            if (istek.kod == 0)
                hata = "Devir seçiniz.";
            if (hata != "") return Results.Json(new { basarili = false, hata = hata });

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().PersonelIzinDevirSil(kullanan, new TNS.IKY.PersonelIzinDevir { kod = istek.kod, personelKod = istek.personelKod });
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Devir bilgisi silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarılı." });
        }
    }
}
