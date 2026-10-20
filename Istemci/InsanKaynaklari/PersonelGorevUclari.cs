using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Kaydet isteği (eski Kaydet(e): formdaki alanlar). Tarihler "yyyy-MM-dd".</summary>
    public class PersonelGorevIstegi
    {
        public int personelKod { get; set; }
        public int gorevNo { get; set; }
        public int unvanKod { get; set; }
        public int gorevDurumu { get; set; }
        public int birimi { get; set; }
        public string baslamaTarihi { get; set; }
        public string bitisTarihi { get; set; }
        public string aciklama { get; set; }
        public string onayNo { get; set; }
        public string onayTarihi { get; set; }
    }

    /// <summary>Sil isteği.</summary>
    public class PersonelGorevSilIstegi
    {
        public int personelKod { get; set; }
        public int gorevNo { get; set; }
    }

    /// <summary>Belge silme isteği.</summary>
    public class PersonelGorevBelgeSilIstegi
    {
        public int personelKod { get; set; }
        public int kayitKod { get; set; }
        public int kod { get; set; }
    }

    /// <summary>
    /// Personel İşlemleri "Görevlendirmeler" sekmesi (eski InsanKaynaklari/PersonelGorev.aspx; yeni sayfa InsanKaynaklari/PersonelGorev) uçları:
    ///   GET  /api/personelbilgi/gorev/tanim                  unvanlar (eski Genel.UnvanDoldur), görev durumları (eski Page_Load)
    ///   GET  /api/personelbilgi/gorev/liste?personelKod=     (eski Listele: ListeleGorevlendirmeler)
    ///   POST /api/personelbilgi/gorev/kaydet, /sil           (eski btnKaydet_Click, btnSil_Click)
    ///   GET  /api/personelbilgi/gorev/birimler?ust=&amp;ara=    (eski ExtOrgSecim ddlBirim)
    ///   GET  /api/personelbilgi/gorev/belge/liste, /belge/goster; POST /belge/yukle, /belge/sil   (eski Belge* yordamları)
    /// </summary>
    public static class PersonelGorevUclari
    {
        private const string SayfaAd = "PersonelGorev";
        private const string Onek = "/api/personelbilgi/gorev";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Onek + "/tanim", Tanim);
            app.MapGet(Onek + "/liste", Liste);
            app.MapPost(Onek + "/kaydet", Kaydet);
            app.MapPost(Onek + "/sil", Sil);
            app.MapGet(Onek + "/birimler", Birimler);
            app.MapGet(Onek + "/belge/liste", BelgeListe);
            app.MapGet(Onek + "/belge/goster", BelgeGoster);
            app.MapPost(Onek + "/belge/yukle", BelgeYukle).DisableAntiforgery();
            app.MapPost(Onek + "/belge/sil", BelgeSil);
        }

        private static IIKYServis Servis() { return TNS.IKY.Arac.Tanimla(); }

        /// <summary>Eski Page_Load: görev durumları ve Genel.UnvanDoldur (UnvanListele(new Unvan()), başa boş satır).</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out _);
            if (ret != null) return ret;

            List<object> durumlar = new List<object>();
            foreach (Gorevler.ENUMGorevDurumu durum in new[] { Gorevler.ENUMGorevDurumu.ALTIAY, Gorevler.ENUMGorevDurumu.ASIL, Gorevler.ENUMGorevDurumu.TEDVIREN,
                Gorevler.ENUMGorevDurumu.UCRETLIVEKALET, Gorevler.ENUMGorevDurumu.UCRETSIZVEKALET })
                durumlar.Add(new { kod = (int)durum, ad = Gorevler.GorevDurumAd(durum) });

            List<object> unvanlar = new List<object>();
            string hata = null, bilgi = null;
            ObjectArray oa = Servis().UnvanListele(new Unvan());
            if (!oa.sonuc.islemSonuc)
                hata = PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Unvanlar okunamadı.");
            else if (oa.objeler.Count <= 0)
                bilgi = PersonelBilgiOrtak.Duz(oa.sonuc.bilgiStr);
            else
            {
                unvanlar.Add(new { UnvanKod = 0, UnvanAd = "" });
                foreach (Unvan u in oa.objeler)
                    unvanlar.Add(new { u.UnvanKod, u.UnvanAd });
            }
            return Results.Json(new { durumlar = durumlar, unvanlar = unvanlar, hata = hata, bilgi = bilgi });
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int personelKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            Gorevler form = new Gorevler();
            form.pKod = personelKod;
            ObjectArray oa = Servis().ListeleGorevlendirmeler(kullanan, form);
            if (!oa.sonuc.islemSonuc)
                return Results.Json(new { hata = PersonelBilgiOrtak.HataMetni(oa.sonuc.hataStr, "Görevlendirmeler okunamadı.") });

            List<object> satirlar = new List<object>();
            foreach (Gorevler g in oa.objeler)
            {
                satirlar.Add(new
                {
                    g.gorevNo,
                    g.unvanKod,
                    unvanAd = g.unvanAd ?? "",
                    gorevDurumu = (int)g.gorevDurumu,
                    gorevDurumuAd = g.gorevDurumuAd,
                    g.birimi,
                    birimiAd = g.birimiAd ?? "",
                    baslamaTarihi = PersonelBilgiOrtak.TarihYaz(g.baslamaTarihi),
                    bitisTarihi = PersonelBilgiOrtak.TarihYaz(g.bitisTarihi),
                    aciklama = g.aciklama ?? "",
                    onayNo = g.onayNo ?? "",
                    onayTarihi = PersonelBilgiOrtak.TarihYaz(g.onayTarihi)
                });
            }
            return Results.Json(new { satirlar = satirlar });
        }

        /// <summary>Eski Kaydet(e).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, PersonelGorevIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            Gorevler form = new Gorevler();
            form.pKod = istek.personelKod;
            form.baslamaTarihiDT = PersonelBilgiOrtak.TarihOku(istek.baslamaTarihi);
            form.bitisTarihiDT = PersonelBilgiOrtak.TarihOku(istek.bitisTarihi);
            form.birimi = istek.birimi;
            form.gorevDurumu = (Gorevler.ENUMGorevDurumu)istek.gorevDurumu;
            form.gorevNo = istek.gorevNo;
            form.unvanKod = istek.unvanKod;
            form.aciklama = istek.aciklama ?? "";
            form.onayNo = istek.onayNo ?? "";
            form.onayTarihiDT = PersonelBilgiOrtak.TarihOku(istek.onayTarihi);
            Sonuc sonuc = Servis().KaydetGorevlendirmeler(kullanan, form);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Görevlendirme kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti.", gorevNo = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0) }); // Gorevlendirmeler003
        }

        /// <summary>Eski btnSil_Click.</summary>
        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, PersonelGorevSilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;

            Gorevler form = new Gorevler();
            form.pKod = istek.personelKod;
            form.gorevNo = istek.gorevNo;
            Sonuc sonuc = Servis().SilGorevlendirmeler(kullanan, form);
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = PersonelBilgiOrtak.HataMetni(sonuc.hataStr, "Görevlendirme silinemedi.") });
            // Eski sayfa silmeden sonra da Gorevlendirmeler003 ("Kayıt işlemi başarıyla gerçekleşti.") yazıyordu.
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla gerçekleşti." });
        }

        private static IResult Birimler(HttpContext baglam, IWebHostEnvironment ortam, int? ust, string ara)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            // Eski ddlBirim'e KullanicininBirimleri verilmemişti: kökte bütün birimler.
            return PersonelIzinGorevOrtak.BirimAgaci(kullanan, ust, ara, false);
        }

        // ------------------------------------------------------------------ belgeler

        private static IResult BelgeListe(HttpContext baglam, IWebHostEnvironment ortam, int personelKod, int kayitKod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            return PersonelIzinGorevOrtak.BelgeListesi(kullanan, personelKod, ENUMBELGEFORMTIP.PERSONELGOREV, kayitKod);
        }

        private static IResult BelgeGoster(HttpContext baglam, IWebHostEnvironment ortam, int personelKod, int kayitKod, int kod)
        {
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            return PersonelIzinGorevOrtak.BelgeGoster(kullanan, personelKod, ENUMBELGEFORMTIP.PERSONELGOREV, kayitKod, kod);
        }

        private static async Task<IResult> BelgeYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            return await PersonelIzinGorevOrtak.BelgeYukle(baglam, kullanan, ENUMBELGEFORMTIP.PERSONELGOREV);
        }

        private static async Task<IResult> BelgeSil(HttpContext baglam, IWebHostEnvironment ortam, PersonelGorevBelgeSilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = PersonelBilgiOrtak.Denetle(baglam, ortam, SayfaAd, out Kullanici kullanan);
            if (ret != null) return ret;
            return PersonelIzinGorevOrtak.BelgeSil(kullanan, istek.personelKod, ENUMBELGEFORMTIP.PERSONELGOREV, istek.kayitKod, istek.kod);
        }
    }
}
