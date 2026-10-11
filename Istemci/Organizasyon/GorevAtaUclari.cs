using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;
using TNS.ORG;

namespace Istemci.Organizasyon
{
    public class GorevKisi
    {
        public string Tc { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
    }

    public class GorevKisiIstegi
    {
        /// <summary>Kişinin görevlendirildiği birim (organizasyon şemasındaki kodu).</summary>
        public int OrgKod { get; set; }
        /// <summary>Görevin anahtarı: "birimKodu_sıraNo".</summary>
        public string Gorev { get; set; }
        public GorevKisi[] Kisiler { get; set; }
    }

    public class GorevKisitIstegi
    {
        public int OrgKod { get; set; }
        public string Gorev { get; set; }
        public string Tc { get; set; }
        /// <summary>Kişinin bu görevde işlem yapabileceği faaliyet kodları.</summary>
        public string Faaliyet { get; set; }
    }

    /// <summary>
    /// Göreve Kişi Atama sayfasının (eski "Gruba Kullanıcı Ata": Organizasyon/ORGAna.aspx?gorevata=1 içindeki birim ağacı +
    /// OrgGorev.aspx + OrgGorevKisit.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/gorevata/birimler?ust=, /gorevler?orgKod=, /kisiler?orgKod=&amp;gorev=, /kisi-ara?..., /kullanici?kod=, /kisit?...
    ///   POST /api/gorevata/kisi-ekle, /kisi-sil, /kisit
    /// Kişi ile görev arasındaki ilişki T.C. kimlik numarasıyla tutulur; kullanıcının yetkileri bu numara üzerinden oluşur.
    /// Yalnızca sistem yöneticisi kullanabilir.
    /// </summary>
    public static class GorevAtaUclari
    {
        /// <summary>Kişi kısıtlarından eski pencerede düzenleneni: faaliyet kodu (SabitGorevlerArac.GorevKisitAlanlar içindeki kodu).</summary>
        private const int FaaliyetKisitKod = 2;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/gorevata/birimler", Birimler);
            app.MapGet("/api/gorevata/gorevler", Gorevler);
            app.MapGet("/api/gorevata/kisiler", Kisiler);
            app.MapGet("/api/gorevata/kisi-ara", KisiAra);
            app.MapGet("/api/gorevata/kullanici", KullaniciBul);
            app.MapGet("/api/gorevata/kisit", KisitOku);
            app.MapPost("/api/gorevata/kisi-ekle", KisiEkle);
            app.MapPost("/api/gorevata/kisi-sil", KisiSil);
            app.MapPost("/api/gorevata/kisit", KisitKaydet);
        }

        private static IResult Denetle(HttpContext baglam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GenelGorevUclari.YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Birim ağacının bir düzeyi: ust boş/0 ise kök birimler, değilse o birimin alt birimleri (eski ORGAgac NodeLoad).</summary>
        private static IResult Birimler(HttpContext baglam, int? ust)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;

            OrganizasyonBirim kriter = new OrganizasyonBirim();
            kriter.anaBirimKod = Math.Max(0, ust ?? 0);
            if (kriter.anaBirimKod == 0)
            {
                kriter.kopruKod = "999";
                kriter.sablonMu = false;
            }

            ObjectArray<OrganizasyonBirim> sonuc = TNS.ORG.Arac.Tanimla().OrganizasyonBirimListele(kriter, false, false);
            if (!sonuc.sonuc.islemSonuc) return Results.Json(new { hata = "Birimler okunamadı." });

            List<object> satirlar = new List<object>();
            foreach (OrganizasyonBirim b in sonuc.objeler)
                satirlar.Add(new { kod = b.kod, ad = b.adi, altVar = b.altDalVar });
            return Results.Json(new { satirlar = satirlar });
        }

        /// <summary>Seçili birimde verilebilecek (aktif) görevler.</summary>
        private static IResult Gorevler(HttpContext baglam, int orgKod)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;

            BirimGenelGorev[] gorevler = TNS.ORG.Arac.Tanimla().GenelGorevListele(kullanan, new OrganizasyonBirim { kod = orgKod }, true);
            List<object> satirlar = new List<object>();
            if (gorevler != null)
                foreach (BirimGenelGorev g in gorevler)
                    satirlar.Add(new { kod = g.Anahtar, ad = g.ad });
            return Results.Json(new { satirlar = satirlar });
        }

        private static List<object> KisiSatirlari(Kullanici kullanan, int orgKod, string gorev)
        {
            TNS.ORG.Personel[] personeller = TNS.ORG.Arac.Tanimla().GenelGorevKisileriListele(kullanan, new OrganizasyonBirim { kod = orgKod },
                BirimGenelGorev.GorevKodBicimindenGenelGoreve(gorev));
            List<object> satirlar = new List<object>();
            if (personeller != null)
                foreach (TNS.ORG.Personel p in personeller)
                    satirlar.Add(new { tc = p.tcKimlikNo, ad = p.ad, soyad = p.soyad });
            return satirlar;
        }

        /// <summary>Seçili birimde seçili göreve atanmış kişiler.</summary>
        private static IResult Kisiler(HttpContext baglam, int orgKod, string gorev)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                return Results.Json(new { satirlar = KisiSatirlari(kullanan, orgKod, gorev) });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Görevlendirilenler okunamadı." });
            }
        }

        /// <summary>Göreve eklenecek kişiyi kullanıcılar arasında arar (en az bir ölçüt gerekir; aktif kullanıcılar).</summary>
        private static IResult KisiAra(HttpContext baglam, string ad, string soyad, string kod)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;

            Kullanici kriter = new Kullanici();
            kriter.ad = (ad ?? "").Trim();
            kriter.soyad = (soyad ?? "").Trim();
            kriter.kullaniciKodu = (kod ?? "").Trim();
            if (kriter.ad == "" && kriter.soyad == "" && kriter.kullaniciKodu == "")
                return Results.Json(new { hata = "Ad, soyad ya da kullanıcı kodundan en az birini yazınız." });
            kriter.aktifMi = ENUMAktiflik.AKTIF;

            ObjectArray<Kullanici> sonuc = TNS.KYM.Arac.Tanimla().KullaniciAra(kriter, 1, 0);
            if (!sonuc.sonuc.islemSonuc) return Results.Json(new { hata = "Kullanıcılar okunamadı." });

            List<object> satirlar = new List<object>();
            bool fazla = false;
            foreach (Kullanici k in sonuc.objeler)
            {
                // T.C. kimlik numarası olmayan kullanıcı göreve bağlanamaz.
                if (string.IsNullOrWhiteSpace(k.mernis)) continue;
                if (satirlar.Count >= 300) { fazla = true; break; }
                satirlar.Add(new { tc = k.mernis.Trim(), ad = k.ad, soyad = k.soyad, kod = k.kullaniciKodu, birim = k.calistigiBirim });
            }
            return Results.Json(new { satirlar = satirlar, fazla = fazla });
        }

        /// <summary>Kullanıcı Giriş sayfasından gelindiğinde, göreve eklenecek kullanıcının kimliği (adrese T.C. no yazılmaz).</summary>
        private static IResult KullaniciBul(HttpContext baglam, string kod)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (string.IsNullOrWhiteSpace(kod)) return Results.BadRequest();

            KullaniciVAN kul = TNS.KYM.Arac.Tanimla().KullaniciOku(kullanan, kod.Trim());
            if (kul == null || !kul.sonuc.islemSonuc || kul.kullanici == null) return Results.Json(new { hata = "Kullanıcı okunamadı." });
            if (string.IsNullOrWhiteSpace(kul.kullanici.mernis)) return Results.Json(new { hata = "Kullanıcının T.C. kimlik numarası kayıtlı değil; göreve eklenemez." });

            // Kişinin şimdiki görevleri (eski PersonelKullaniciIslemleri sayfasındaki roller listesi).
            string tc = kul.kullanici.mernis.Trim();
            List<object> gorevler = new List<object>();
            try
            {
                KisiGorev[] kisiGorevleri = TNS.ORG.Arac.Tanimla().KisiGenelGorevleriListele(kullanan, new KisiGorev { tcKimlikNo = tc });
                if (kisiGorevleri != null)
                    foreach (KisiGorev g in kisiGorevleri)
                        gorevler.Add(new { orgKod = g.orgKod, birim = g.orgAd, gorev = g.gorevOrgKod + "_" + g.gorevSiraNo, ad = g.gorevAd });
            }
            catch (Exception)
            {
                gorevler = null; // liste okunamadıysa pencere yine de kullanılabilsin
            }
            return Results.Json(new { tc = tc, ad = kul.kullanici.ad, soyad = kul.kullanici.soyad, kod = kul.kullanici.kullaniciKodu, gorevler = gorevler });
        }

        private static IResult KisitOku(HttpContext baglam, int orgKod, string gorev, string tc)
        {
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;

            GorevKisitDeger[] kisitlar = TNS.ORG.Arac.Tanimla().GenelGorevKisiKisitListele(kullanan, new OrganizasyonBirim { kod = orgKod },
                BirimGenelGorev.GorevKodBicimindenGenelGoreve(gorev), (tc ?? "").Trim());
            string faaliyet = "";
            if (kisitlar != null)
                foreach (GorevKisitDeger k in kisitlar)
                    if (k.kod == FaaliyetKisitKod) faaliyet = k.deger ?? "";
            return Results.Json(new { faaliyet = faaliyet });
        }

        // ------------------------------------------------------------------ yazma

        private static TNS.ORG.Personel[] PersonelDizisi(GorevKisi[] kisiler)
        {
            List<TNS.ORG.Personel> liste = new List<TNS.ORG.Personel>();
            foreach (GorevKisi k in kisiler ?? new GorevKisi[0])
            {
                if (k == null || string.IsNullOrWhiteSpace(k.Tc)) continue;
                liste.Add(new TNS.ORG.Personel { tcKimlikNo = k.Tc.Trim(), ad = k.Ad ?? "", soyad = k.Soyad ?? "" });
            }
            return liste.ToArray();
        }

        private static async Task<IResult> KisiEkle(HttpContext baglam, GorevKisiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.OrgKod <= 0 || string.IsNullOrWhiteSpace(istek.Gorev)) return Hata("Önce birim ve görev seçiniz.");

            TNS.ORG.Personel[] personeller = PersonelDizisi(istek.Kisiler);
            if (personeller.Length == 0) return Hata("Eklenecek kişi seçiniz.");

            Sonuc sonuc = TNS.ORG.Arac.Tanimla().GenelGorevKisileriEkle(kullanan, new OrganizasyonBirim { kod = istek.OrgKod },
                BirimGenelGorev.GorevKodBicimindenGenelGoreve(istek.Gorev), personeller);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kişi göreve eklenemedi."));
            return Results.Json(new { basarili = true, bilgi = "Kişi göreve eklendi.", satirlar = KisiSatirlari(kullanan, istek.OrgKod, istek.Gorev) });
        }

        private static async Task<IResult> KisiSil(HttpContext baglam, GorevKisiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.OrgKod <= 0 || string.IsNullOrWhiteSpace(istek.Gorev)) return Hata("Önce birim ve görev seçiniz.");

            TNS.ORG.Personel[] personeller = PersonelDizisi(istek.Kisiler);
            if (personeller.Length == 0) return Hata("Görevden silinecek kişileri seçiniz.");

            Sonuc sonuc = TNS.ORG.Arac.Tanimla().GenelGorevKisileriSil(kullanan, new OrganizasyonBirim { kod = istek.OrgKod },
                BirimGenelGorev.GorevKodBicimindenGenelGoreve(istek.Gorev), personeller);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kişiler görevden silinemedi."));
            return Results.Json(new { basarili = true, bilgi = "Seçilenler görevden silindi.", satirlar = KisiSatirlari(kullanan, istek.OrgKod, istek.Gorev) });
        }

        private static async Task<IResult> KisitKaydet(HttpContext baglam, GorevKisitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.OrgKod <= 0 || string.IsNullOrWhiteSpace(istek.Gorev) || string.IsNullOrWhiteSpace(istek.Tc)) return Results.BadRequest();

            GorevKisitDeger kisit = new GorevKisitDeger();
            kisit.kod = FaaliyetKisitKod;
            kisit.deger = (istek.Faaliyet ?? "").Trim();

            Sonuc sonuc = TNS.ORG.Arac.Tanimla().GenelGorevKisiKisitEkle(kullanan, new OrganizasyonBirim { kod = istek.OrgKod },
                BirimGenelGorev.GorevKodBicimindenGenelGoreve(istek.Gorev), istek.Tc.Trim(), new[] { kisit });
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kayıt işlemi başarısız."));
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarılı." });
        }

        // ------------------------------------------------------------------ yardımcılar

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = sonuc.hataStr ?? "";
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata;
        }
    }
}
