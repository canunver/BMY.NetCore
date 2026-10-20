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
    public class TanimIsIstegi
    {
        public int Kod { get; set; }
        public int OrgBirimKod { get; set; }
        public string Adi { get; set; }
        public int Tip { get; set; }
        /// <summary>yyyy-MM-dd</summary>
        public string BaslangicTarihi { get; set; }
        /// <summary>yyyy-MM-dd</summary>
        public string BitisTarihi { get; set; }
        public string Amac { get; set; }
        public string Hedef { get; set; }
        public string SorumluKod { get; set; }
        public string GorevTanimi { get; set; }
        public int Durum { get; set; }
    }

    public class TanimIsOdenekSatiri
    {
        public string tertip { get; set; }
        public double blokeTutar { get; set; }
    }

    public class TanimIsOdenekIstegi
    {
        public int Yil { get; set; }
        public int IsKod { get; set; }
        public List<TanimIsOdenekSatiri> Satirlar { get; set; }
    }

    /// <summary>
    /// İş Tanımları (eski ButceMuhasebe/TanimIs.aspx ve sekmelerindeki alt sayfalar) uçları:
    ///   GET  /api/tanimis/tanim                     açılış: yıl, tipler, durumlar, kullanıcının son birimi
    ///   GET  /api/tanimis/liste?yil&amp;eko&amp;tip&amp;durum   iş listesi (eski Listele: IsListele, bitiş tarihi bugün)
    ///   GET  /api/tanimis/liste/yazdir?...          işlere göre ödenek bloke durumu (MUH/MUH-BlokeOdenek.xlt)
    ///   GET  /api/tanimis/oku?kod                   iş bilgisi (eski FormDoldur)
    ///   POST /api/tanimis/kaydet, /api/tanimis/sil
    ///   GET  /api/tanimis/orgagac?ust&amp;ara          birim seçimi (eski ExtOrgSecim: kullanıcının saymanlık-birim yetkisine göre)
    ///   GET  /api/tanimis/sorumlu?ad&amp;soyad&amp;tc      sorumlu seçimi (eski Personelv2/ListePersonelSec ya da KYM/ListeKullaniciSec)
    ///   GET  /api/tanimis/odenek?yil&amp;isKod, POST /api/tanimis/odenek/kaydet    Ödenek Bloke sekmesi (eski TanimIsButceFisleri)
    ///   GET  /api/tanimis/odenekdurum?yil&amp;isKod&amp;muh&amp;muhBir                Ödenek Durum sekmesi (eski TanimIsOdenekDurum)
    ///   GET  /api/tanimis/fisler?..., /api/tanimis/fisler/detay?yil&amp;belgeNo   Muhasebe Fişleri sekmesi (eski TanimIsMuhasebeFisleri)
    ///   GET  /api/tanimis/taahhut?...                                       Taahhütler sekmesi (eski TanimIsTaahhutler)
    /// Bütün uçlar bu sayfanın menü yetkisiyle denetlenir; Muhasebe Fişleri ayrıca ödeme emri görme yetkisi ister (eski sayfa gibi).
    /// </summary>
    public static class TanimIsUclari
    {
        public const string SayfaAdresi = "../ButceMuhasebe/TanimIs.aspx";
        private const string Uc = "/api/tanimis/";

        private static readonly object[] Tipler =
        {
            new { kod = 1, ad = "BAB Projesi" },
            new { kod = 2, ad = "Yönlendirilmiş Proje" },
            new { kod = 3, ad = "Eğitim-Öğretime Katkı Projesi" },
            new { kod = 4, ad = "Topluma Hizmet Projesi" },
            new { kod = 5, ad = "Tanıtım Faaliyetleri" },
            new { kod = 6, ad = "Hizmet Alımları" },
            new { kod = 7, ad = "Yatırım Projesi" },
            new { kod = 8, ad = "Temsil, Ağırlama Organizasyonları" },
            new { kod = 9, ad = "Akademik Faaliyetler" },
            new { kod = 10, ad = "Genel Yönetim İşleri" }
        };

        private static readonly object[] Durumlar =
        {
            new { kod = 1, ad = "Devam Ediyor" },
            new { kod = 2, ad = "İptal Edildi" },
            new { kod = 3, ad = "Tamamlandı" }
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapGet(Uc + "liste/yazdir", Yazdir);
            app.MapGet(Uc + "oku", Oku);
            app.MapPost(Uc + "kaydet", Kaydet);
            app.MapPost(Uc + "sil", Sil);
            app.MapGet(Uc + "orgagac", OrgAgac);
            app.MapGet(Uc + "sorumlu", SorumluAra);
            app.MapGet(Uc + "odenek", Odenek);
            app.MapPost(Uc + "odenek/kaydet", OdenekKaydet);
            app.MapGet(Uc + "odenekdurum", OdenekDurum);
            app.MapGet(Uc + "fisler", Fisler);
            app.MapGet(Uc + "fisler/detay", FisDetay);
            app.MapGet(Uc + "taahhut", Taahhut);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (Istemci.Organizasyon.GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdresi);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = Temiz(hata);
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string BirimAd(int kod)
        {
            if (kod <= 0) return "";
            try
            {
                TNS.ORG.OrganizasyonBirim b = TNS.ORG.Arac.OrgBirimBul(kod);
                return b != null ? (b.adi ?? "") : "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        // ------------------------------------------------------------------ açılış

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski Page_Load: kullanıcının son seçtiği birim (BirimKod değişkeni), yoksa yetkili birim köprü kodunun son üç hanesi.
            int birimKod = OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "BirimKod"), 0);
            string birimAd = BirimAd(birimKod);
            if (birimKod <= 0)
            {
                try
                {
                    string kopru = Convert.ToString(kullanan.yetkiTipiDegerleri["KopruKod", 0]) ?? "";
                    if (kopru.Trim().Length >= 3)
                    {
                        TNS.ORG.OrganizasyonBirim b = TNS.ORG.Arac.OrgBirimBul(kopru.Substring(kopru.Length - 3, 3));
                        if (b != null) { birimKod = b.kod; birimAd = b.adi ?? ""; }
                    }
                }
                catch (Exception) { /* birim önerilmez */ }
            }

            List<int> yillar = new List<int>();
            for (int i = 2006; i <= DateTime.Now.Year + 1; i++) yillar.Add(i);
            return Results.Json(new
            {
                yil = DateTime.Now.Year,
                yillar = yillar,
                tipler = Tipler,
                durumlar = Durumlar,
                birimKod = birimKod > 0 ? birimKod : 0,
                birimAd = birimAd,
                fisYetki = FisYetkiVar(kullanan)
            });
        }

        // ------------------------------------------------------------------ iş listesi, yazdır

        private static IsTanim Kriter(int? yil, string eko, int? tip, int? durum)
        {
            IsTanim form = new IsTanim();
            form.sorguYil = (yil ?? 0) > 0 ? yil.Value : DateTime.Now.Year;
            form.sorguEko = Temiz(eko);
            form.tip = tip ?? 0;
            form.durum = durum ?? 0;
            return form;
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string eko, int? tip, int? durum)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IsTanim form = Kriter(yil, eko, tip, durum);
                form.bitisTarihi = new TNSDateTime(DateTime.Now.Date);
                ObjectArray liste = Servis().IsListele(kullanan, form);
                if (liste == null || !liste.sonuc.islemSonuc) return Hata(HataMetni(liste?.sonuc.hataStr, "İşler listelenemedi."));
                List<object> satirlar = new List<object>();
                foreach (IsTanim t in liste.objeler)
                    satirlar.Add(new { kod = t.kod, adi = t.adi ?? "" });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İşler listelenemedi."));
            }
        }

        /// <summary>Eski btnYazdir_Click: İŞLERE GÖRE ÖDENEK BLOKE DURUM BİLGİSİ (MUH-BlokeOdenek.xlt).</summary>
        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string eko, int? tip, int? durum)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray liste = Servis().IsBlokeOdenekListele(kullanan, Kriter(yil, eko, tip, durum));
                if (liste == null || !liste.sonuc.islemSonuc) return Hata(HataMetni(liste?.sonuc.hataStr, "Rapor hazırlanamadı."));

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                XLS.DosyaAc(SablonYol("MUH-BlokeOdenek.xlt"), sonucDosyaAd);

                int satirD = 0, sutunD = 0;
                XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
                int satir = satirD + 2;

                XLS.HucreAdBulYaz("txtBaslik", "İŞLERE GÖRE ÖDENEK BLOKE DURUM BİLGİSİ");
                XLS.HucreAdBulYaz("txtYil", DateTime.Now.Year.ToString());
                XLS.HucreAdBulYaz("txtKurum", GenelIslemlerIstemci.VarsayilanKurumBul() + " - " + GenelIslemlerIstemci.VarsayilanKurumBulAd());

                double toplamOdenek = 0, toplamHarcanan = 0, toplamKalan = 0;
                foreach (IsBlokeOdenek b in liste.objeler)
                {
                    int s = 0;
                    XLS.HucreKopyala(satirD, 0, satirD, 20, satir, 0);
                    XLS.HucreDegerYaz(satir, s++, b.isAd);
                    XLS.HucreDegerYaz(satir, s++, b.birimAd);
                    XLS.HucreDegerYaz(satir, s++, b.ilgiliAd);
                    XLS.HucreDegerYaz(satir, s++, b.toplamOdenek);
                    XLS.HucreDegerYaz(satir, s++, b.harcananOdenek);
                    XLS.HucreDegerYaz(satir, s++, b.kalanTutar);
                    toplamOdenek += b.toplamOdenek;
                    toplamHarcanan += b.harcananOdenek;
                    toplamKalan += b.kalanTutar;
                    satir++;
                }

                XLS.HucreKopyala(satirD + 1, 0, satirD + 1, 20, satir, 0);
                XLS.HucreDegerYaz(satir, 3, toplamOdenek);
                XLS.HucreDegerYaz(satir, 4, toplamHarcanan);
                XLS.HucreDegerYaz(satir, 5, toplamKalan);

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "Liste", true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        /// <summary>Şablon: önce kurumun RaporSablonYol/MUH klasörü, yoksa uygulamanın RaporSablon/MUH klasörü.</summary>
        private static string SablonYol(string sablonAd)
        {
            string kurum = Path.Combine(Ayar.Oku("RaporSablonYol") ?? "", "MUH", sablonAd);
            if (File.Exists(kurum)) return kurum;
            string uygulama = Path.Combine(AppContext.BaseDirectory, "RaporSablon", "MUH", sablonAd);
            if (File.Exists(uygulama)) return uygulama;
            return kurum;
        }

        // ------------------------------------------------------------------ iş bilgisi

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((kod ?? 0) <= 0) return Hata("İş seçiniz.");
            try
            {
                ObjectArray liste = Servis().IsListele(kullanan, new IsTanim { kod = kod.Value });
                if (liste == null || !liste.sonuc.islemSonuc || liste.objeler.Count == 0) return Hata("İş bilgisi okunamadı.");
                IsTanim f = (IsTanim)liste.objeler[0];
                return Results.Json(new
                {
                    kod = f.kod,
                    orgBirimKod = f.orgBirimKod,
                    orgBirimAd = string.IsNullOrWhiteSpace(f.orgBirimAd) ? BirimAd(f.orgBirimKod) : f.orgBirimAd,
                    muhasebeKod = f.muhasebeKod,
                    muhasebeBirimKod = f.muhasebeBirimKod ?? "",
                    adi = f.adi ?? "",
                    tip = f.tip,
                    baslangicTarihi = OdemeEmriMIFUclari.TarihYaz(f.baslangicTarihi),
                    bitisTarihi = OdemeEmriMIFUclari.TarihYaz(f.bitisTarihi),
                    amac = f.amac ?? "",
                    hedef = f.hedef ?? "",
                    sorumluKod = f.sorumluKod ?? "",
                    sorumluAd = f.sorumluAd ?? "",
                    gorevTanimi = f.gorevTanimi ?? "",
                    durum = f.durum
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İş bilgisi okunamadı."));
            }
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, TanimIsIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Hata("Bilgi yok.");
            try
            {
                IsTanim form = new IsTanim();
                form.kod = istek.Kod;
                form.orgBirimKod = istek.OrgBirimKod;
                form.adi = Temiz(istek.Adi);
                form.tip = istek.Tip;
                form.baslangicTarihi = OdemeEmriMIFUclari.Tarih(istek.BaslangicTarihi);
                form.bitisTarihi = OdemeEmriMIFUclari.Tarih(istek.BitisTarihi);
                form.amac = Temiz(istek.Amac);
                form.hedef = Temiz(istek.Hedef);
                form.sorumluKod = Temiz(istek.SorumluKod);
                form.gorevTanimi = Temiz(istek.GorevTanimi);
                form.durum = istek.Durum;

                Sonuc sonuc = Servis().IsKaydet(kullanan, form);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Kaydedilemedi."));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "BirimKod", form.orgBirimKod + "");
                return Results.Json(new { bilgi = "İş tanımı başarıyla kaydedildi.", kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, form.kod) });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kaydedilemedi."));
            }
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, TanimIsIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Kod <= 0) return Hata("Silinecek işi seçiniz.");
            try
            {
                Sonuc sonuc = Servis().IsSil(kullanan, new IsTanim { kod = istek.Kod });
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Silinemedi."));
                return Results.Json(new { bilgi = "İş tanımı silindi." });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Silinemedi."));
            }
        }

        // ------------------------------------------------------------------ birim ve sorumlu seçimi

        /// <summary>
        /// Eski ExtOrgSecim + ORGBirim.NodeLoad: kökte kullanıcının "Personel Saymanlık-Birim Yetkisi"ne göre birimler (yetkisi yoksa
        /// bütün kök birimler); ara yazılınca adında geçen birimler düz liste olarak (il / ilçe adıyla) gelir.
        /// </summary>
        private static IResult OrgAgac(HttpContext baglam, IWebHostEnvironment ortam, int? ust, string ara)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ara = Temiz(ara);
                TNS.ORG.OrganizasyonBirim kriter = new TNS.ORG.OrganizasyonBirim();
                kriter.anaBirimKod = Math.Max(0, ust ?? 0);
                kriter.sablonMu = false;
                if (ara != "")
                {
                    kriter.anaBirimKod = 0;
                    kriter.adi = ara;
                }
                else if (kriter.anaBirimKod == 0)
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
                if (sonuc == null || !sonuc.sonuc.islemSonuc) return Hata("Birimler okunamadı.");
                List<object> satirlar = new List<object>();
                foreach (TNS.ORG.OrganizasyonBirim b in sonuc.objeler)
                {
                    string ad = b.adi ?? "";
                    if (ara != "")
                    {
                        string ilIlce = TNS.ORG.Arac.OrgBirimIlIlceAdBul(b.kod);
                        if (!string.IsNullOrEmpty(ilIlce)) ad += " (" + ilIlce + ")";
                    }
                    satirlar.Add(new { kod = b.kod, ad = ad, birimAd = b.adi ?? "", altVar = ara == "" && b.altDalVar });
                    if (satirlar.Count >= 300) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Birimler okunamadı.");
            }
        }

        /// <summary>
        /// Sorumlu seçimi: /PersonelListeKullanicidan = 1 ise kullanıcılar (eski KYM/ListeKullaniciSec), değilse personel
        /// (eski Personelv2/ListePersonelSec) arasında ad, soyad ya da T.C. kimlik no ile arama. Sorumlu kodu T.C. kimlik nodur.
        /// </summary>
        private static IResult SorumluAra(HttpContext baglam, IWebHostEnvironment ortam, string ad, string soyad, string tc)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            ad = Temiz(ad); soyad = Temiz(soyad); tc = Temiz(tc);
            if (ad == "" && soyad == "" && tc == "") return Hata("Ad, soyad ya da T.C. kimlik nodan en az birini yazınız.");
            try
            {
                List<object> satirlar = new List<object>();
                if (TNS.MUH.Arac.DegiskenDegerBul(0, "/PersonelListeKullanicidan") == "1")
                {
                    Kullanici kriter = new Kullanici();
                    kriter.ad = ad;
                    kriter.soyad = soyad;
                    kriter.TCKIMLIKNO = tc;
                    kriter.aktifMi = ENUMAktiflik.AKTIF;
                    ObjectArray<Kullanici> sonuc = TNS.KYM.Arac.Tanimla().KullaniciAra(kriter, 1, 0);
                    if (sonuc == null || !sonuc.sonuc.islemSonuc) return Hata("Kullanıcılar okunamadı.");
                    foreach (Kullanici k in sonuc.objeler)
                    {
                        if (string.IsNullOrWhiteSpace(k.TCKIMLIKNO)) continue;
                        satirlar.Add(new { tc = k.TCKIMLIKNO, ad = k.ad ?? "", soyad = k.soyad ?? "", birim = k.calistigiBirim ?? "" });
                        if (satirlar.Count >= 300) break;
                    }
                }
                else
                {
                    TNS.PER.Personel p = new TNS.PER.Personel();
                    p.ad = ad;
                    p.soyad = soyad;
                    p.tcKimlikNo = tc;
                    ObjectArray sonuc = TNS.PER.Arac.Tanimla().PersonelListele(kullanan, p);
                    if (sonuc == null || !sonuc.sonuc.islemSonuc) return Hata(HataMetni(sonuc?.sonuc.hataStr, "Personel okunamadı."));
                    foreach (TNS.PER.Personel x in sonuc.objeler)
                    {
                        satirlar.Add(new { tc = x.tcKimlikNo ?? "", ad = x.ad ?? "", soyad = x.soyad ?? "", birim = x.birimAd ?? "" });
                        if (satirlar.Count >= 300) break;
                    }
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Arama yapılamadı."));
            }
        }

        // ------------------------------------------------------------------ sekmeler

        /// <summary>Ödenek Bloke (eski TanimIsButceFisleri Listle).</summary>
        private static IResult Odenek(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? isKod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((isKod ?? 0) <= 0) return Results.Json(new { satirlar = new List<object>() });
            try
            {
                ObjectArray liste = Servis().IsOdenekListele(kullanan, new IsOdenek { isKod = isKod.Value, yil = yil ?? 0 });
                if (liste == null || !liste.sonuc.islemSonuc) return Hata(HataMetni(liste?.sonuc.hataStr, "Ödenekler okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (IsOdenek f in liste.objeler)
                    satirlar.Add(new { tertip = f.tertip, blokeTutar = f.blokeTutar });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Ödenekler okunamadı."));
            }
        }

        /// <summary>Eski TanimIsButceFisleri btnKaydet_Click: satır yoksa yılın bütün blokeleri silinir.</summary>
        private static async Task<IResult> OdenekKaydet(HttpContext baglam, IWebHostEnvironment ortam, TanimIsOdenekIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.IsKod <= 0 || istek.Yil <= 0) return Hata("Önce işi seçiniz.");
            try
            {
                List<IsOdenek> liste = new List<IsOdenek>();
                foreach (TanimIsOdenekSatiri s in istek.Satirlar ?? new List<TanimIsOdenekSatiri>())
                {
                    if (string.IsNullOrWhiteSpace(s.tertip)) continue;
                    IsOdenek f = new IsOdenek();
                    f.isKod = istek.IsKod;
                    f.yil = istek.Yil;
                    f.tertip = s.tertip.Trim();
                    f.blokeTutar = Math.Round(s.blokeTutar, 2);
                    liste.Add(f);
                }
                Sonuc sonuc = liste.Count > 0
                    ? Servis().IsOdenekKaydet(kullanan, liste.ToArray())
                    : Servis().IsOdenekSil(kullanan, new IsOdenek { isKod = istek.IsKod, yil = istek.Yil });
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Kaydedilemedi."));
                return Results.Json(new { bilgi = "Kayıt işlemi başarılı." });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kaydedilemedi."));
            }
        }

        /// <summary>Ödenek Durum (eski TanimIsOdenekDurum).</summary>
        private static IResult OdenekDurum(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? isKod, int? muh, string muhBir)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((isKod ?? 0) <= 0) return Results.Json(new { satirlar = new List<object>() });
            try
            {
                ObjectArray liste = Servis().IsOdenekDurumListele(kullanan, yil ?? 0, muh ?? 0, Temiz(muhBir), isKod.Value);
                if (liste == null || !liste.sonuc.islemSonuc) return Hata(HataMetni(liste?.sonuc.hataStr, "Ödenek durumu okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (IsOdenek f in liste.objeler)
                    satirlar.Add(new { tertip = f.tertip, blokeTutar = f.blokeTutar });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Ödenek durumu okunamadı."));
            }
        }

        private static bool FisYetkiVar(Kullanici kullanan)
        {
            try
            {
                return TNS.MUH.Yetki.YetkisiVarMi(kullanan, (int)TNS.MUH.ENUMOGY.Gorme, (int)ENUMBelgeTuru.ODEMEEMRI);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Muhasebe Fişleri (eski TanimIsMuhasebeFisleri btnListe_Click): işe bağlı, iptaller hariç bütün belgeler.</summary>
        private static IResult Fisler(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? isKod, int? muh, string muhBir)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!FisYetkiVar(kullanan)) return Hata("Ödeme emri belgelerini görme yetkiniz yok.");
            if ((isKod ?? 0) <= 0) return Results.Json(new { satirlar = new List<object>() });
            if ((yil ?? 0) <= 0) return Hata("Yıl giriniz.");
            try
            {
                OdemeEmriMIFForm f = new OdemeEmriMIFForm();
                f.yil = yil.Value;
                f.formDurum = 22; // Bütün Belgeler (İptaller Hariç)
                f.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                f.muhasebe = muh ?? 0;
                f.birim = Temiz(muhBir);
                f.sorguBelgeTur = 0;
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(f.yil);
                detay.isKod = isKod.Value;
                f.detay.objeler.Add(detay);

                ObjectArray fler = Servis().OdemeEmriMIFListele(kullanan, f, false);
                if (fler == null || !fler.sonuc.islemSonuc) return Hata(HataMetni(fler?.sonuc.hataStr, "Belgeler listelenemedi."));
                List<object> satirlar = new List<object>();
                foreach (OdemeEmriMIFForm ff in fler.objeler)
                {
                    satirlar.Add(new
                    {
                        yil = ff.yil > 0 ? ff.yil : yil.Value,
                        belgeNo = ff.belgeNo,
                        yevmiyeNo = ff.YEVYevNo,
                        belgeTur = ff.belgeTur == ENUMBelgeTuru.ODEMEEMRI ? "ÖDE" : "MİF",
                        belgeTarih = OdemeEmriMIFUclari.TarihYaz(ff.islemTarih),
                        muhasebe = ff.muhasebe.ToString().PadLeft(5, '0') + " - " + ff.muhasebeAd,
                        birim = ff.birim + " - " + ff.birimAd,
                        borc = ff.borcToplam,
                        alacak = ff.alacakToplam,
                        aciklama = ff.aciklama,
                        ilgili = ff.ilgiliAd,
                        yevmiyeTarih = OdemeEmriMIFUclari.TarihYaz(ff.YEVYevTarih),
                        durum = OdemeEmriMIFUclari.DurumDonustur(ff.formDurum),
                        durumTarih = OdemeEmriMIFUclari.TarihYaz(ff.formDurumTarih),
                        islemYapan = ff.islemYapan,
                        OEBNo = ff.OEBNo,
                        OEBTarih = OdemeEmriMIFUclari.TarihYaz(ff.OEBTarih),
                        cekNo = ff.YEVCekNo,
                        cekTarih = OdemeEmriMIFUclari.TarihYaz(ff.YEVCekTarih),
                        surecNo = ff.surecYil + "/" + ff.surecNo,
                        ihaleMadde = ff.ihale,
                        ist1Ad = ff.istKod1 != null ? ff.istKod1.aciklama : "",
                        ist2Ad = ff.istKod2 != null ? ff.istKod2.aciklama : "",
                        ekliDosya = ff.ekliDosyaVarmi > 0 ? "Var" : ""
                    });
                }
                return Results.Json(new { satirlar = satirlar, bilgi = satirlar.Count == 0 ? "Verilen kriterlere uygun kayıt bulunamadı." : null });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belgeler listelenemedi."));
            }
        }

        /// <summary>Eski BeforeExpand: belgenin satırları (hesap kodu, tertip, borç, alacak).</summary>
        private static IResult FisDetay(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string belgeNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!FisYetkiVar(kullanan)) return Hata("Ödeme emri belgelerini görme yetkiniz yok.");
            belgeNo = Temiz(belgeNo);
            if ((yil ?? 0) <= 0 || belgeNo == "") return Hata("Belge seçiniz.");
            try
            {
                ObjectArray fler = OdemeEmriMIFYazdirma.Listele(kullanan, Servis(), yil.Value, belgeNo);
                if (fler == null || fler.objeler == null || fler.objeler.Count == 0) return Hata("Belge okunamadı.");
                OdemeEmriMIFForm form = (OdemeEmriMIFForm)fler.objeler[0];
                List<object> satirlar = new List<object>();
                foreach (OdemeEmriMIFDetay d in form.detay.objeler)
                    satirlar.Add(new { hesapKod = d.hesapKod, tertip = TNS.MUH.Arac.TertipStrOl(d.kur, d.fon, d.fin, d.eko, d.peb), borc = d.borc, alacak = d.alacak });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belge okunamadı."));
            }
        }

        /// <summary>Taahhütler (eski TanimIsTaahhutler): işe bağlı taahhüt kartları.</summary>
        private static IResult Taahhut(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? isKod, int? muh, string muhBir)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((isKod ?? 0) <= 0) return Results.Json(new { satirlar = new List<object>() });
            try
            {
                TaahhutKartiBilgi bilgi = new TaahhutKartiBilgi();
                bilgi.yil = yil ?? 0;
                bilgi.muhasebeKod = muh ?? 0;
                bilgi.birimKod = Temiz(muhBir);
                bilgi.isKod = isKod.Value;
                ObjectArray liste = Servis().TaahhutKartiListele(kullanan, bilgi, 0);
                List<object> satirlar = new List<object>();
                if (liste != null && liste.objeler != null)
                    foreach (TaahhutKartiBilgi t in liste.objeler)
                        satirlar.Add(new
                        {
                            kod = t.kod,
                            muteahhit = t.muteahhitUnvan ?? "",
                            sozlesmeTarih = OdemeEmriMIFUclari.TarihYaz(t.sozlesmeTarihi),
                            sozlesmeTutar = t.ihaleBedeli,
                            kalanBakiye = 0
                        });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Taahhütler okunamadı."));
            }
        }
    }
}
