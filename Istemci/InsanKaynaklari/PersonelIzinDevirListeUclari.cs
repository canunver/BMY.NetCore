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
    /// <summary>Listele / Devreden İzinleri Aktar ölçütleri (eski ctlIzinDonem, ddlBirim, ctlPersonelTip, txtGenelSorgu).</summary>
    public class IzinDevirOlcut
    {
        public int yil { get; set; }
        public int birim { get; set; }
        public string turler { get; set; }
        public string ara { get; set; }
    }

    /// <summary>Kaydet isteği: listelenen yıl (eski hdnYil) ve değişen satırlar (eski getRowsValues dirtyRowsOnly).</summary>
    public class IzinDevirKayitIstegi
    {
        public int yil { get; set; }
        public List<PersonelIzinDevirSatiri> satirlar { get; set; }
    }

    /// <summary>
    /// Personel İzin Devir Bilgileri (eski InsanKaynaklari/PersonelIzinDevir.aspx; yeni sayfa InsanKaynaklari/PersonelIzinDevir) uçları:
    ///   GET  /api/izindevir/tanim               personel tipleri, yıl sınırları, dönem yılı (eski Page_Load)
    ///   GET  /api/izindevir/orgagac?ust=&amp;ara=   birim seçimi (eski ExtOrgSecim)
    ///   GET  /api/izindevir/liste?yil=&amp;birim=&amp;turler=&amp;ara=   (eski btnListele_Click: PersonelIzinDevirListele)
    ///   POST /api/izindevir/kaydet               (eski btnKaydet_Click: PersonelIzinDevirKaydet)
    ///   POST /api/izindevir/aktar                (eski btnAktar_Click: PersonelIzinDevirleriAktar)
    /// İş kuralları eski sayfadaki gibi IIKYServis üzerinden çağrılır.
    /// </summary>
    public static class PersonelIzinDevirListeUclari
    {
        public const string SayfaAdresi = "../InsanKaynaklari/PersonelIzinDevir.aspx";
        private const string Uc = "/api/izindevir/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "orgagac", OrgAgac);
            app.MapGet(Uc + "liste", Liste);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapPost(Uc + "aktar", Aktar);
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

        private static IIKYServis Servis() { return TNS.IKY.Arac.Tanimla(); }

        private static IResult Hata(string metin) { return Results.Json(new { basarili = false, hata = metin }); }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                DateTime simdi = DateTime.Now;
                int yil = IzinArac.DonemYiliBul(simdi.Year, simdi.Month);
                return Results.Json(new
                {
                    turler = PersonelBilgiUclari.TurListesi(null),
                    yilMin = 1980,
                    yilMaks = simdi.Year,
                    yil,
                    donemAd = IzinArac.DonemAdBul(yil)
                });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Sayfa bilgileri okunamadı."));
            }
        }

        private static IResult OrgAgac(HttpContext baglam, IWebHostEnvironment ortam, int? ust, string ara)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return PersonelBilgiUclari.BirimSatirlari(kullanan, ust, ara);
        }

        private static TNS.IKY.PersonelIzinDevir Kriter(IzinDevirOlcut o)
        {
            TNS.IKY.PersonelIzinDevir pid = new TNS.IKY.PersonelIzinDevir();
            pid.yil = o.yil;
            pid.arama = o.ara ?? "";
            pid.personelTipleri = o.turler ?? "";
            return pid;
        }

        // Eski btnListele_Click
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? birim, string turler, string ara)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            IzinDevirOlcut o = new IzinDevirOlcut { yil = yil ?? 0, birim = birim ?? 0, turler = turler, ara = ara };
            if (o.birim == 0) return Hata("Birim bilgisini boş bırakamazsınız");

            try
            {
                ObjectArray pidArr = Servis().PersonelIzinDevirListele(kullanan, Kriter(o), o.birim);
                if (!pidArr.sonuc.islemSonuc)
                    return Hata(PersonelBilgiOrtak.HataMetni(pidArr.sonuc.hataStr, "İzin devir bilgileri okunamadı."));

                List<object> satirlar = new List<object>();
                foreach (object x in pidArr.objeler)
                {
                    if (!(x is TNS.IKY.PersonelIzinDevir d)) continue;
                    satirlar.Add(new { d.kod, d.personelKod, d.yil, d.yilAd, d.sicilNo, d.tcKimlikNo, d.adSoyad, d.devreden, d.yilici, d.kullanilan });
                }
                return Results.Json(new { basarili = true, yil = o.yil, birim = o.birim, satirlar });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "İzin devir bilgileri okunamadı."));
            }
        }

        // Eski btnKaydet_Click
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IzinDevirKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            if (istek.yil == 0) return Hata("Yıl bilgisini boş bırakamazsınız");

            List<TNS.IKY.PersonelIzinDevir> liste = new List<TNS.IKY.PersonelIzinDevir>();
            foreach (PersonelIzinDevirSatiri s in istek.satirlar ?? new List<PersonelIzinDevirSatiri>())
            {
                if (s == null) continue;
                liste.Add(new TNS.IKY.PersonelIzinDevir { kod = s.kod, personelKod = s.personelKod, yil = s.yil, devreden = s.devreden, yilici = s.yilici });
            }
            // Eski sayfa değişen satır yoksa hiçbir şey yapmıyordu.
            if (liste.Count == 0) return Results.Json(new { basarili = true, bos = true, bilgi = "Değişen satır yok." });

            try
            {
                Sonuc sonuc = Servis().PersonelIzinDevirKaydet(kullanan, liste.ToArray());
                if (!sonuc.islemSonuc)
                    return Hata(PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "İzin devir bilgileri kaydedilemedi."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "İzin devir bilgileri kaydedilemedi."));
            }
        }

        // Eski btnAktar_Click
        private static async Task<IResult> Aktar(HttpContext baglam, IWebHostEnvironment ortam, IzinDevirOlcut istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            if (istek.birim == 0) return Hata("Birim bilgisini boş bırakamazsınız");

            try
            {
                Sonuc s = Servis().PersonelIzinDevirleriAktar(kullanan, Kriter(istek), istek.birim);
                if (!s.islemSonuc)
                    return Hata(PersonelBilgiOrtak.HataMetni(s.hataStr, "Devreden izinler aktarılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Aktarım işlemi başarılı." });
            }
            catch (Exception ex)
            {
                return Hata(PersonelBilgiOrtak.HataMetni(ex.Message, "Devreden izinler aktarılamadı."));
            }
        }
    }
}
