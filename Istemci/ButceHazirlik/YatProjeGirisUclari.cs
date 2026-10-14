using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ButceHazirlikv2;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    public class YatAsamaIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        /// <summary>Aşama kopyalamada kaynak ve hedef aşama.</summary>
        public string Kaynak { get; set; }
        public string Hedef { get; set; }
    }

    public class YatIlIlce
    {
        public int IlKod { get; set; }
        public int IlceKod { get; set; }
        public string IlAd { get; set; }
        public string IlceAd { get; set; }
    }

    public class YatProjeIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public int Kod { get; set; }
        public string Birim { get; set; }
        public string DptProjeNo { get; set; }
        public string Ad { get; set; }
        public string BaslangicYili { get; set; }
        public string BitisYili { get; set; }
        public string Sektor { get; set; }
        public string Karakteristik { get; set; }
        public bool Etud { get; set; }
        public bool EskiYeni { get; set; }
        public bool Aktif { get; set; }
        public string Not { get; set; }
        public List<YatIlIlce> Ilceler { get; set; }
    }

    public class YatAltProjeIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public int Kod { get; set; }
        public int ProjeKod { get; set; }
        public string Ad { get; set; }
        public string Birim { get; set; }
        public int Model { get; set; }
        public string BaslangicYili { get; set; }
        public string BitisYili { get; set; }
        public string Karakteristik { get; set; }
        public string Gerekce { get; set; }
        public bool DptDeVar { get; set; }
        public List<string> Bolgeler { get; set; }
        public string Not { get; set; }
        public List<YatIlIlce> Ilceler { get; set; }
    }

    public class YatSilIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public int Kod { get; set; }
    }

    public class YatTasiIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public int AltProjeKod { get; set; }
        public int ProjeKod { get; set; }
    }

    public class YatKaynakEkleIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public int AltProjeKod { get; set; }
        public int Tur { get; set; }
        public string Ad { get; set; }
        public bool ToplamaDahil { get; set; }
    }

    public class YatBilgiIstegi
    {
        public string Sayfa { get; set; }
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public int ProjeKodu { get; set; }
        public int AltProjeKodu { get; set; }
        public Dictionary<string, string> Alanlar { get; set; }
    }

    /// <summary>
    /// Yatırım Projeleri ekranının (eski ButceHazirlikv2/ProjeGiris.aspx) ve içinde açılan ProjeDetay, AltProjeDetay,
    /// AltProjeAmacGiris, AltProjeIlgiliGiris, AltProjeGerekceGiris sayfalarının JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/yatprojegiris/tanim, /projeler, /detay-tanim, /ilceler, /proje, /altproje, /tasi-projeler, /bilgi,
    ///        /proje-dosyasi (Raporlar menüsü), /yat-rapor (Yat. Raporları menüsü)
    ///   POST /api/yatprojegiris/odenek-aktar, /asama-kopyala, /proje-kaydet, /proje-sil, /altproje-kaydet, /altproje-sil,
    ///        /altproje-tasi, /kaynak-ekle, /bilgi
    /// İşlemler eskisi gibi TNS.YAT.Arac.Tanimla() üzerinden yapılır; projeye yazma / görme hakkını iş kuralları denetler.
    /// Sayfa yetkisi: ProjeGiris menüden, alt sayfalar ProjeGiris üzerinden (YatProjeOrtak).
    /// </summary>
    public static class YatProjeGirisUclari
    {
        private const string SayfaGiris = "ProjeGiris";
        private const string SayfaProje = "ProjeDetay";
        private const string SayfaAltProje = "AltProjeDetay";

        // Amaç / İlgililer / Gerekçe sayfalarının sakladığı alanlar (eski sayfalardaki alanlar dizileri). İstekte yalnızca bunlar kabul edilir.
        private static readonly Dictionary<string, string[]> BilgiAlanlari = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "AltProjeAmacGiris", new[] { "kisaAmac", "aciklama", "amac", "kapsam", "gerekce", "gerekceOz", "gerceklesmeDegerlendirme", "progFaal", "yapilanFaal", "plan0", "plan1", "plan2", "plan3", "sorunCozum" } },
            { "AltProjeIlgiliGiris", new[] { "HazAdSoy", "HazUnvan", "HazBirim", "HazIsTel", "HazCepTel", "HazFaks", "HazEmail",
                                             "YurAdSoy", "YurUnvan", "YurBirim", "YurIsTel", "YurCepTel", "YurFaks", "YurEmail",
                                             "KoorAdSoy", "KoorUnvan", "KoorBirim", "KoorIsTel", "KoorCepTel", "KoorFaks", "KoorEmail" } },
            { "AltProjeGerekceGiris", new[] { "hizmetGerekcesi" } },
        };

        // Eski AltProjeAmacGiris okurken bu alanı da istiyordu (kaydetmiyordu).
        private static readonly string[] AmacOkunanEk = { "detayProg" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/yatprojegiris/tanim", Tanim);
            app.MapGet("/api/yatprojegiris/projeler", Projeler);
            app.MapPost("/api/yatprojegiris/odenek-aktar", OdenekAktar);
            app.MapPost("/api/yatprojegiris/asama-kopyala", AsamaKopyala);

            app.MapGet("/api/yatprojegiris/detay-tanim", DetayTanim);
            app.MapGet("/api/yatprojegiris/ilceler", Ilceler);
            app.MapGet("/api/yatprojegiris/proje", ProjeOku);
            app.MapPost("/api/yatprojegiris/proje-kaydet", ProjeKaydet);
            app.MapPost("/api/yatprojegiris/proje-sil", ProjeSil);
            app.MapGet("/api/yatprojegiris/altproje", AltProjeOku);
            app.MapPost("/api/yatprojegiris/altproje-kaydet", AltProjeKaydet);
            app.MapPost("/api/yatprojegiris/altproje-sil", AltProjeSil);
            app.MapGet("/api/yatprojegiris/tasi-projeler", TasiProjeler);
            app.MapPost("/api/yatprojegiris/altproje-tasi", AltProjeTasi);
            app.MapPost("/api/yatprojegiris/kaynak-ekle", KaynakEkle);
            app.MapGet("/api/yatprojegiris/proje-dosyasi", ProjeDosyasi);
            app.MapGet("/api/yatprojegiris/yat-rapor", YatRapor);

            app.MapGet("/api/yatprojegiris/bilgi", BilgiOku);
            app.MapPost("/api/yatprojegiris/bilgi", BilgiKaydet);
        }

        private static IResult Hata(string hata) { return YatProjeOrtak.Hata(hata); }
        private static string HataMetni(string hata, string varsayilan) { return YatProjeOrtak.HataMetni(hata, varsayilan); }
        private static string Temiz(string deger) { return YatProjeOrtak.Temiz(deger); }

        /// <summary>Aşama kodu "NN.NN" biçiminde (eski OrtakFonksiyonlar.KodNoktaKoy("99.99", ...)).</summary>
        private static string Senaryo(string senaryo)
        {
            string s = Temiz(senaryo).Replace(".", "");
            if (s == "") return "";
            if (s.Length < 4) s = s.PadLeft(4, '0');
            return s.Substring(0, 2) + "." + s.Substring(2);
        }

        /// <summary>Aşama kodu yalnızca rakam ve noktadan oluşabilir.</summary>
        private static bool SenaryoGecerli(string senaryo)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(senaryo ?? "", @"^[0-9]{2}\.[0-9]{2}$");
        }

        // ------------------------------------------------------------------ ortak listeler

        /// <summary>Eski BHMv2Ortak.SenaryodanExtStore: aşamalar "kod- ad".</summary>
        private static List<object> Asamalar(Kullanici kullanan)
        {
            List<object> asamalar = new List<object>();
            ButceSenaryo[] senaryolar = TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null);
            if (senaryolar != null)
                foreach (ButceSenaryo bs in senaryolar)
                    asamalar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });
            return asamalar;
        }

        /// <summary>Eski BHMv2Ortak.EXTBirimDoldur: kurumsal kodun 4. düzeyi; isteğe göre başta boş, sonda kurumun kendisi.</summary>
        private static List<object> Birimler(int yil, bool bosItemOlsun, bool kurumEkle)
        {
            string kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();
            List<object> birimler = new List<object>();
            if (bosItemOlsun) birimler.Add(new { kod = "", ad = "" });
            ABSKodlarVAN birimVan = TNS.BKD.Arac.Tanimla().KodListeleSeviye(yil, "1", kurumKod, 4, true);
            if (birimVan != null && birimVan.absKodlar != null)
                foreach (ABSKod k in birimVan.absKodlar)
                    birimler.Add(new { kod = k.kod, ad = k.kodAd });
            if (kurumEkle && !string.IsNullOrEmpty(kurumKod))
                birimler.Add(new { kod = kurumKod, ad = kurumKod + "-" + (TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD") ?? "") });
            return birimler;
        }

        /// <summary>Eski ABSKodDoldur(yil, "7", "", -2, ..., sadeceDetay: true): sektörlerin son kırılımı.</summary>
        private static List<object> Sektorler(int yil, bool bosBasaEkle)
        {
            List<object> sektorler = new List<object>();
            if (bosBasaEkle) sektorler.Add(new { kod = "", ad = "" });
            ABSKodlarVAN sek = TNS.BKD.Arac.Tanimla().KodListeleSeviye(yil, "7", "", -2, true, false, "", "", "", false);
            if (sek != null && sek.absKodlar != null)
                foreach (ABSKod k in sek.absKodlar)
                    sektorler.Add(new { kod = k.kod, ad = k.kodAd });
            return sektorler;
        }

        // ------------------------------------------------------------------ ProjeGiris

        /// <summary>
        /// Açılış (eski ProjeGiris Page_Load): kullanıcının son yılı ve aşaması (izleme değerlendirmede aşama 06.00 ve
        /// değiştirilemez), aşamalar, sektörler, birimler, sistem yöneticisi mi (Öd. Aktar ve Kopyala düğmeleri).
        /// </summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int? islemKod)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaGiris, out Kullanici kullanan);
            if (ret != null) return ret;
            bool izleme = islemKod == 1;
            try
            {
                int yil = ButceRakamUclari.KullaniciYilBul(kullanan);
                string senaryo = izleme ? "06.00" : Senaryo(ButceRakamUclari.KullaniciSenaryoBul(kullanan));
                return Results.Json(new
                {
                    yil = yil,
                    senaryo = senaryo,
                    asamaKilitli = izleme,
                    izleme = izleme,
                    baslik = izleme ? "İzleme Değerlendirme" : "Yatırım Proje Tanımlama Ekranı",
                    detayBaslik = izleme ? "İzleme Değerlendirme" : "Detay Bilgileri",
                    asamalar = Asamalar(kullanan),
                    sektorler = Sektorler(yil, true),
                    birimler = Birimler(yil, true, true),
                    yonetici = YatProjeOrtak.SistemYoneticisi(kullanan)
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>
        /// Proje ağacı (eski ProjeDoldur): ölçütlere uyan projeler ve alt projeleri. Yıl ve aşama kullanıcı değişkeni olarak
        /// saklanır (eski ProjeAgacGuncelle).
        /// </summary>
        private static IResult Projeler(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, string projeNo, string projeAd,
            int? aktiflik, string sektor, string birim)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaGiris, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            senaryo = Senaryo(senaryo);
            if (!SenaryoGecerli(senaryo)) return Hata("Lütfen aşama seçiniz!");

            try
            {
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "Senaryo", senaryo);
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ButceYil", yil.ToString());

                Proje p = new Proje();
                p.ad = string.IsNullOrWhiteSpace(projeAd) ? null : projeAd.Trim();
                p.DPTProjeNo = string.IsNullOrWhiteSpace(projeNo) ? null : projeNo.Trim();
                p.sektor.kod = Temiz(sektor);
                p.aktiflik = aktiflik ?? 1;
                string kurKod = Temiz(birim);
                if (kurKod == "") kurKod = null;

                BilgiVAN liste = TNS.YAT.Arac.Tanimla().ProjeListele(kullanan, yil, senaryo, p, kurKod, true);
                if (!liste.sonuc.islemSonuc) return Hata(HataMetni(liste.sonuc.hataStr, "Projeler okunamadı."));

                List<object> projeler = new List<object>();
                foreach (Proje pr in liste.liste)
                {
                    List<object> altlar = new List<object>();
                    if (pr.altProjeler != null)
                        foreach (AltProje ap in pr.altProjeler)
                            altlar.Add(new { kod = ap.kod, ad = ap.ad });
                    projeler.Add(new { kod = pr.kod, ad = pr.DPTProjeNo + "-" + pr.ad, altProjeler = altlar });
                }
                return Results.Json(new { projeler = projeler, bilgi = projeler.Count == 0 ? HataMetni(liste.sonuc.bilgiStr, "Proje bulunamadı.") : null });
            }
            catch (Exception)
            {
                return Hata("Projeler okunamadı.");
            }
        }

        /// <summary>Öd. Aktar (eski btnOdenekAktar_Click; yalnızca sistem yöneticisi).</summary>
        private static async Task<IResult> OdenekAktar(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatAsamaIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaGiris);
            if (ret != null) return ret;
            if (!YatProjeOrtak.SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            istek ??= new YatAsamaIstegi();
            string senaryo = Senaryo(istek.Senaryo);
            if (istek.Yil < 2000) return Hata("Yıl hatalı!");
            if (!SenaryoGecerli(senaryo)) return Hata("Lütfen aşama seçiniz!");
            try
            {
                Sonuc s = TNS.YAT.Arac.Tanimla().ButceKaynakOdenekAktar(kullanan, istek.Yil, senaryo);
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "Aktarım yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Aktarım işlemi tamamlandı" });
            }
            catch (Exception)
            {
                return Hata("Aktarım yapılamadı.");
            }
        }

        /// <summary>Aşama Kopyala penceresi (eski btnAsamaKopyala_Click; yalnızca sistem yöneticisi).</summary>
        private static async Task<IResult> AsamaKopyala(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatAsamaIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaGiris);
            if (ret != null) return ret;
            if (!YatProjeOrtak.SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            istek ??= new YatAsamaIstegi();
            string kaynak = Senaryo(istek.Kaynak), hedef = Senaryo(istek.Hedef);
            if (istek.Yil < 2000) return Hata("Yıl hatalı!");
            if (!SenaryoGecerli(kaynak) || !SenaryoGecerli(hedef)) return Hata("Kaynak ve hedef aşamayı seçiniz.");
            try
            {
                Sonuc s = TNS.YAT.Arac.Tanimla().AsamaKopyala(kullanan, istek.Yil, kaynak, istek.Yil, hedef, null, null);
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "Aşama kopyalanamadı."));
                return Results.Json(new { basarili = true, bilgi = "Aşama rakamları kopyalandı" });
            }
            catch (Exception)
            {
                return Hata("Aşama kopyalanamadı.");
            }
        }

        // ------------------------------------------------------------------ ProjeDetay / AltProjeDetay ortak

        /// <summary>
        /// Proje ve alt proje sayfalarının seçenekleri (eski Page_Load): birimler, sektörler, iller, karakteristikler,
        /// yatırım bölgeleri, bütçe kaynak türleri, Yat. Raporları menüsü, sistem yöneticisi mi.
        /// </summary>
        private static IResult DetayTanim(HttpContext baglam, IWebHostEnvironment ortam, int yil, string sayfa)
        {
            bool alt = string.Equals(sayfa, SayfaAltProje, StringComparison.OrdinalIgnoreCase);
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, alt ? SayfaAltProje : SayfaProje, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                IYATServis servis = TNS.YAT.Arac.Tanimla();

                List<object> iller = new List<object> { new { kod = 0, ad = "İl Seçiniz" } };
                BilgiVAN ilVan = servis.IlListele(kullanan, new Sozluk());
                if (ilVan != null && ilVan.liste != null)
                    foreach (Sozluk il in ilVan.liste)
                        iller.Add(new { kod = il.kod, ad = il.ad });

                List<object> karakteristikler = new List<object>();
                KarakteristikGiris[] kar = servis.KarekteristikListele(kullanan) ?? new KarakteristikGiris[0];
                foreach (KarakteristikGiris k in kar)
                    karakteristikler.Add(new { ekTurKod = k.ekTurKod, deger = k.deger, degerKisa = k.degerKisa });

                List<object> bolgeler = new List<object>();
                List<object> kaynakTurleri = new List<object>();
                if (alt)
                {
                    ObjectArray oa = servis.ListeleKalkinmaBolgeleriTanim(kullanan, new KalkinmaBolge());
                    if (oa != null && oa.objeler != null)
                        foreach (KalkinmaBolge b in oa.objeler)
                            bolgeler.Add(new { kod = b.bolgeKod, ad = b.bolgeAd });
                    for (int i = 1; i <= 6; i++)
                        kaynakTurleri.Add(new { kod = i, ad = ButceKaynak.TurAdGetir((ENUMButceKaynakTur)i) });
                }

                List<object> yatRaporlari;
                try { yatRaporlari = YatProjeRaporlari.YatRaporListesi(); }
                catch (Exception) { yatRaporlari = new List<object>(); }

                return Results.Json(new
                {
                    // Eski EXTBirimDoldur: proje sayfasında (boş yok, kurum yok), alt proje sayfasında (boş yok, kurum var).
                    birimler = Birimler(yil, false, alt),
                    sektorler = alt ? null : Sektorler(yil, false),
                    iller = iller,
                    karakteristikler = karakteristikler,
                    bolgeler = bolgeler,
                    kaynakTurleri = kaynakTurleri,
                    yatRaporlari = yatRaporlari,
                    yonetici = YatProjeOrtak.SistemYoneticisi(kullanan)
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>İlin ilçeleri (eski cmbIl_Change).</summary>
        private static IResult Ilceler(HttpContext baglam, IWebHostEnvironment ortam, int il)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaProje, out Kullanici kullanan);
            if (ret != null) return ret;
            List<object> ilceler = new List<object>();
            if (il <= 0) return Results.Json(new { ilceler = ilceler });
            try
            {
                IlceSozluk ilce = new IlceSozluk();
                ilce.il.kod = il;
                BilgiVAN bv = TNS.YAT.Arac.Tanimla().IlceListele(kullanan, ilce);
                if (bv != null && bv.liste != null)
                    foreach (IlceSozluk i in bv.liste)
                        ilceler.Add(new { kod = i.ilceKodu, ad = i.ilceAdi });
                return Results.Json(new { ilceler = ilceler });
            }
            catch (Exception)
            {
                return Hata("İlçeler okunamadı.");
            }
        }

        private static List<object> IlIlceler(TNSCollection ilIlceler)
        {
            List<object> l = new List<object>();
            if (ilIlceler != null)
                foreach (ProjeIlIlce i in ilIlceler)
                    l.Add(new { ilKod = i.ilKod, ilceKod = i.ilceKod, ilAd = i.ilAd, ilceAd = i.ilceAd });
            return l;
        }

        private static ProjeIlIlce[] IlIlceler(List<YatIlIlce> gelen)
        {
            List<ProjeIlIlce> l = new List<ProjeIlIlce>();
            if (gelen != null)
                foreach (YatIlIlce g in gelen)
                {
                    if (g == null || g.IlceKod <= 0) continue;
                    ProjeIlIlce i = new ProjeIlIlce();
                    i.ilKod = g.IlKod;
                    i.ilceKod = g.IlceKod;
                    i.ilAd = g.IlAd;
                    i.ilceAd = g.IlceAd;
                    l.Add(i);
                }
            return l.ToArray();
        }

        /// <summary>Eski ProjeDetay.ProjeNotBul / ProjeNotSakla: proje (altProjeNo 0) ya da alt proje (projeNo 0) notu.</summary>
        private static string NotOku(Kullanici kullanan, int projeNo, int altProjeNo)
        {
            return TNS.KYM.Arac.Tanimla().BuyukMetinOku(kullanan, "YAT", "PN" + projeNo + "_" + altProjeNo, "tr");
        }

        private static void NotSakla(Kullanici kullanan, int projeNo, int altProjeNo, string metin)
        {
            TNS.KYM.Arac.Tanimla().BuyukMetinSakla(kullanan, "YAT", "PN" + projeNo + "_" + altProjeNo, "tr", metin ?? "");
        }

        /// <summary>
        /// Projeyle / alt projeyle ilişkili tertipler (eski TertipListele): bağlı alt faaliyetlere girilmiş tertipler.
        /// </summary>
        private static List<object> Tertipler(Kullanici kullanan, int yil, string senaryo, int projeKod, int altProjeKod)
        {
            List<object> l = new List<object>();
            IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            AltFaaliyet[] pebKodlar;
            if (altProjeKod > 0)
            {
                AltFaaliyet peb = new AltFaaliyet();
                peb.yil = yil;
                peb.altProje = altProjeKod;
                pebKodlar = servis.YatBagPebKodlariListele(kullanan, peb, senaryo);
            }
            else
                pebKodlar = servis.YatBagPebKodlariListele(kullanan, yil, projeKod, senaryo);
            if (pebKodlar == null || pebKodlar.Length == 0) return l;
            string kodlar = "";
            foreach (AltFaaliyet p in pebKodlar)
            {
                if (kodlar != "") kodlar += ";";
                kodlar += p.pebKod;
            }
            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            kistas.kodlar[4] = kodlar;
            Tertip[] tertipler = servis.TertipTanimlariListele(kullanan, kistas);
            if (tertipler != null)
                foreach (Tertip t in tertipler)
                    l.Add(new { kod = t.TertipStrOl(), ad = t.tertipAdi });
            return l;
        }

        // ------------------------------------------------------------------ ProjeDetay

        /// <summary>Proje bilgileri (eski ProjeDetay.Listele).</summary>
        private static IResult ProjeOku(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, int kod)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaProje, out Kullanici kullanan);
            if (ret != null) return ret;
            senaryo = Senaryo(senaryo);
            if (yil < 2000 || !SenaryoGecerli(senaryo)) return Hata("Yıl ya da aşama hatalı!");
            if (kod <= 0) return Hata("Herhangi bir Proje seçilmemiş!");
            try
            {
                Proje kistas = new Proje();
                kistas.kod = kod;
                BilgiVAN bilgi = TNS.YAT.Arac.Tanimla().ProjeListele(kullanan, yil, senaryo, kistas, "", false);
                if (!bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi.sonuc.hataStr, "Proje okunamadı."));
                if (bilgi.liste.Count == 0) return Hata(HataMetni(bilgi.sonuc.bilgiStr, "Proje bulunamadı."));
                Proje p = (Proje)bilgi.liste[0];

                List<object> tertipler;
                string tertipHata = null;
                try { tertipler = Tertipler(kullanan, yil, senaryo, kod, 0); }
                catch (Exception) { tertipler = new List<object>(); tertipHata = "Tertipler okunamadı."; }

                return Results.Json(new
                {
                    kod = p.kod,
                    birim = p.yurutecekKurum ?? "",
                    dptProjeNo = p.DPTProjeNo,
                    ad = p.ad,
                    baslangicYili = p.baslangicYili,
                    bitisYili = p.bitisYili,
                    sektor = p.sektor != null ? p.sektor.kod : "",
                    karakteristik = p.karakteristik,
                    etud = p.etudProjeMi,
                    eskiYeni = p.eskiYeni,
                    aktif = p.aktifMi,
                    // Eski ProjeDurumBelirle: çalışılan yıl başlangıç yılıysa yeni, değilse devam eden proje.
                    durum = yil == p.baslangicYili ? "Yeni Proje" : "Devam Eden",
                    ilceler = IlIlceler(p.ilIlceler),
                    not = NotOku(kullanan, p.kod, 0),
                    tertipler = tertipler,
                    tertipHata = tertipHata
                });
            }
            catch (Exception)
            {
                return Hata("Proje okunamadı.");
            }
        }

        /// <summary>Proje kaydı (eski ProjeDetay.Kaydet). Kod 0 ise yeni proje açılır; not yalnızca kayıt başarılıysa saklanır.</summary>
        private static async Task<IResult> ProjeKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatProjeIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaProje);
            if (ret != null) return ret;
            istek ??= new YatProjeIstegi();
            string senaryo = Senaryo(istek.Senaryo);
            if (istek.Yil < 2000 || !SenaryoGecerli(senaryo)) return Hata("Yıl ya da aşama hatalı!");

            Proje proje = new Proje();
            proje.kod = Math.Max(0, istek.Kod);
            proje.ad = Temiz(istek.Ad);
            proje.DPTProjeNo = Temiz(istek.DptProjeNo);
            proje.yurutecekKurum = Temiz(istek.Birim);
            proje.tur.kod = 1;
            proje.sektor.kod = Temiz(istek.Sektor);
            proje.baslangicYili = OrtakFonksiyonlar.ConvertToInt(Temiz(istek.BaslangicYili), 0);
            proje.bitisYili = OrtakFonksiyonlar.ConvertToInt(Temiz(istek.BitisYili), 0);
            proje.model = 1;
            proje.karakteristik = Temiz(istek.Karakteristik);
            proje.karakteristikOzet = "";
            proje.gerekce = "";
            proje.ilIlceler.AddRange(IlIlceler(istek.Ilceler));
            proje.etudProjeMi = istek.Etud;
            proje.eskiYeni = istek.EskiYeni;
            proje.aktifMi = istek.Aktif;

            try
            {
                Sonuc sonuc = TNS.YAT.Arac.Tanimla().ProjeKaydet(kullanan, istek.Yil, senaryo, proje, proje.kod == 0);
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Proje kaydedilemedi."));
                int kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, proje.kod);
                // Eskisi gibi notu yalnızca sistem yöneticisi değiştirebilir (diğerlerinde alan kapalıydı ve kendi değeriyle saklanıyordu).
                if (YatProjeOrtak.SistemYoneticisi(kullanan))
                    NotSakla(kullanan, kod, 0, istek.Not);
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla tamamlandı.", kod = kod, ad = proje.DPTProjeNo + "-" + proje.ad });
            }
            catch (Exception)
            {
                return Hata("Proje kaydedilemedi.");
            }
        }

        /// <summary>Proje silme (eski ProjeDetay.Sil; düğme yalnızca sistem yöneticisine görünür, yetkiyi iş kuralı denetler).</summary>
        private static async Task<IResult> ProjeSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatSilIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaProje);
            if (ret != null) return ret;
            istek ??= new YatSilIstegi();
            string senaryo = Senaryo(istek.Senaryo);
            if (istek.Kod <= 0) return Hata("Herhangi bir Proje seçilmemiş!");
            try
            {
                Proje p = new Proje();
                p.kod = istek.Kod;
                Sonuc s = TNS.YAT.Arac.Tanimla().ProjeSil(kullanan, istek.Yil, senaryo, new TNSCollection(p));
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "Proje silinemedi."));
                return Results.Json(new { basarili = true, bilgi = HataMetni(s.bilgiStr, "Proje silindi.") });
            }
            catch (Exception)
            {
                return Hata("Proje silinemedi.");
            }
        }

        // ------------------------------------------------------------------ AltProjeDetay

        /// <summary>
        /// Alt proje bilgileri (eski AltProjeDetay.Listele). apKod 0 ise yeni alt projedir: yalnızca bağlı olduğu projenin adı döner
        /// (eski Temizle).
        /// </summary>
        private static IResult AltProjeOku(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, int apKod, int pKod)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaAltProje, out Kullanici kullanan);
            if (ret != null) return ret;
            senaryo = Senaryo(senaryo);
            if (yil < 2000 || !SenaryoGecerli(senaryo)) return Hata("Yıl ya da aşama hatalı!");
            try
            {
                IYATServis servis = TNS.YAT.Arac.Tanimla();
                if (apKod <= 0)
                {
                    if (pKod <= 0) return Hata("Herhangi bir Proje seçilmemiş!");
                    Proje kistas = new Proje();
                    kistas.kod = pKod;
                    BilgiVAN pv = servis.ProjeListele(kullanan, yil, senaryo, kistas, "", false);
                    string projeAd = pv.sonuc.islemSonuc && pv.liste.Count > 0 ? ((Proje)pv.liste[0]).ad : "";
                    return Results.Json(new { kod = 0, projeKod = pKod, projeAd = projeAd, dptDeVar = true, ilceler = new List<object>(), bolgeler = new List<string>(), kaynaklar = new List<object>(), tertipler = new List<object>() });
                }

                AltProje kistasAp = new AltProje();
                kistasAp.kod = apKod;
                BilgiVAN bilgi = servis.AltProjeListele(kullanan, yil, senaryo, kistasAp, null, true, false);
                if (!bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi.sonuc.hataStr, "Alt proje okunamadı."));
                if (bilgi.liste.Count == 0) return Hata(HataMetni(bilgi.sonuc.bilgiStr, "Alt proje bulunamadı."));
                AltProje ap = (AltProje)bilgi.liste[0];

                List<string> bolgeler = new List<string>();
                foreach (string b in (ap.bolgeListeStr ?? "").Split(','))
                    if (b.Trim() != "") bolgeler.Add(b.Trim());

                List<object> kaynaklar = new List<object>();
                if (ap.butceKaynaklari != null)
                    foreach (ButceKaynak bk in ap.butceKaynaklari)
                        kaynaklar.Add(new { kaynakNo = bk.kaynakNo, ad = bk.kaynakAd });

                List<object> tertipler;
                string tertipHata = null;
                try { tertipler = Tertipler(kullanan, yil, senaryo, 0, apKod); }
                catch (Exception) { tertipler = new List<object>(); tertipHata = "Tertipler okunamadı."; }

                return Results.Json(new
                {
                    kod = ap.kod,
                    projeKod = ap.proje.kod,
                    projeAd = ap.proje.ad,
                    ad = ap.ad,
                    birim = ap.birim.kod,
                    model = ap.model,
                    baslangicYili = ap.baslangicYil,
                    bitisYili = ap.bitisYil,
                    karakteristik = ap.karakteristik,
                    gerekce = ap.gerekce,
                    dptDeVar = ap.DPTDeVar,
                    bolgeler = bolgeler,
                    not = NotOku(kullanan, 0, ap.kod),
                    ilceler = IlIlceler(ap.ilIlceler),
                    kaynaklar = kaynaklar,
                    tertipler = tertipler,
                    tertipHata = tertipHata
                });
            }
            catch (Exception)
            {
                return Hata("Alt proje okunamadı.");
            }
        }

        /// <summary>Alt proje kaydı (eski AltProjeDetay.Kaydet). Kod 0 ise yeni alt proje açılır.</summary>
        private static async Task<IResult> AltProjeKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatAltProjeIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAltProje);
            if (ret != null) return ret;
            istek ??= new YatAltProjeIstegi();
            string senaryo = Senaryo(istek.Senaryo);
            if (istek.Yil < 2000 || !SenaryoGecerli(senaryo)) return Hata("Yıl ya da aşama hatalı!");

            AltProje ap = new AltProje();
            ap.kod = Math.Max(0, istek.Kod);
            ap.ad = Temiz(istek.Ad);
            ap.birim.kod = Temiz(istek.Birim);
            ap.model = istek.Model;
            ap.baslangicYil = OrtakFonksiyonlar.ConvertToInt(Temiz(istek.BaslangicYili), 0);
            ap.bitisYil = OrtakFonksiyonlar.ConvertToInt(Temiz(istek.BitisYili), 0);
            ap.proje.kod = istek.ProjeKod;
            ap.tur.kod = 1;
            ap.karakteristik = Temiz(istek.Karakteristik);
            ap.karakteristikOzet = "";
            // Finansman kaynağı ve maliyet eski sayfada gizliydi; boş / 0 olarak kaydediliyordu.
            ap.finansmanKaynak = "";
            ap.maliyet = 0;
            ap.gerekce = Temiz(istek.Gerekce);
            ap.DPTDeVar = istek.DptDeVar;
            List<string> bolgeler = new List<string>();
            if (istek.Bolgeler != null)
                foreach (string b in istek.Bolgeler)
                    if (!string.IsNullOrWhiteSpace(b)) bolgeler.Add(b.Trim());
            ap.bolgeListeStr = string.Join(",", bolgeler);
            ap.ilIlceler.AddRange(IlIlceler(istek.Ilceler));

            try
            {
                Sonuc sonuc = TNS.YAT.Arac.Tanimla().AltProjeKaydet(kullanan, istek.Yil, senaryo, ap, ap.kod == 0);
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Alt proje kaydedilemedi."));
                int kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, ap.kod);
                if (YatProjeOrtak.SistemYoneticisi(kullanan))
                    NotSakla(kullanan, 0, kod, istek.Not);
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla tamamlandı.", kod = kod, projeKod = ap.proje.kod, ad = ap.ad });
            }
            catch (Exception)
            {
                return Hata("Alt proje kaydedilemedi.");
            }
        }

        /// <summary>Alt proje silme (eski AltProjeDetay.Sil).</summary>
        private static async Task<IResult> AltProjeSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatSilIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAltProje);
            if (ret != null) return ret;
            istek ??= new YatSilIstegi();
            if (istek.Kod <= 0) return Hata("Herhangi bir Alt Proje seçilmemiş!");
            try
            {
                AltProje ap = new AltProje();
                ap.kod = istek.Kod;
                Sonuc s = TNS.YAT.Arac.Tanimla().AltProjeSil(kullanan, istek.Yil, Senaryo(istek.Senaryo), new TNSCollection(ap));
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "Alt proje silinemedi."));
                return Results.Json(new { basarili = true, bilgi = HataMetni(s.bilgiStr, "Alt proje silindi.") });
            }
            catch (Exception)
            {
                return Hata("Alt proje silinemedi.");
            }
        }

        /// <summary>Taşı penceresindeki projeler (eski AltProjeDetay stoProjeler).</summary>
        private static IResult TasiProjeler(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo)
        {
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, SayfaAltProje, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!YatProjeOrtak.SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            try
            {
                BilgiVAN projeler = TNS.YAT.Arac.Tanimla().ProjeListele(kullanan, yil, Senaryo(senaryo), new Proje(), null, false);
                if (!projeler.sonuc.islemSonuc) return Hata(HataMetni(projeler.sonuc.hataStr, "Projeler okunamadı."));
                List<object> l = new List<object>();
                foreach (Proje p in projeler.liste)
                    l.Add(new { kod = p.kod, ad = p.ad, dptProjeNo = p.DPTProjeNo });
                return Results.Json(new { projeler = l });
            }
            catch (Exception)
            {
                return Hata("Projeler okunamadı.");
            }
        }

        /// <summary>Alt projeyi başka projeye taşıma (eski AltProjeTasi_Click; düğme yalnızca sistem yöneticisine görünür).</summary>
        private static async Task<IResult> AltProjeTasi(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatTasiIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAltProje);
            if (ret != null) return ret;
            if (!YatProjeOrtak.SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            istek ??= new YatTasiIstegi();
            if (istek.AltProjeKod <= 0 || istek.ProjeKod <= 0) return Hata("Taşınacak projeyi seçiniz.");
            try
            {
                Sonuc s = TNS.YAT.Arac.Tanimla().AltProjeTasi(kullanan, istek.Yil, Senaryo(istek.Senaryo), istek.AltProjeKod, istek.ProjeKod);
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "Alt proje taşınamadı."));
                return Results.Json(new { basarili = true, bilgi = "Alt proje taşındı, sonucu görmek için yeniden listeleme yapınız!" });
            }
            catch (Exception)
            {
                return Hata("Alt proje taşınamadı.");
            }
        }

        /// <summary>Yeni Bütçe Kaynak (eski btnKaydetYeniBK_Click; düğme yalnızca sistem yöneticisine görünür).</summary>
        private static async Task<IResult> KaynakEkle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatKaynakEkleIstegi istek)
        {
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAltProje);
            if (ret != null) return ret;
            if (!YatProjeOrtak.SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            istek ??= new YatKaynakEkleIstegi();
            if (istek.AltProjeKod <= 0) return Hata("Herhangi bir Alt Proje seçilmemiş!");
            if (istek.Tur < 1 || istek.Tur > 6) return Hata("Kaynak türünü seçiniz.");
            if (Temiz(istek.Ad) == "") return Hata("Kaynak adını giriniz.");
            try
            {
                ButceKaynak kaynak = new ButceKaynak();
                kaynak.altProjeKodu = istek.AltProjeKod;
                kaynak.kaynakTur = (ENUMButceKaynakTur)istek.Tur;
                kaynak.kaynakAd = Temiz(istek.Ad);
                kaynak.toplamaDahil = istek.ToplamaDahil ? ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleDahil : ButceKaynak.ENUMKaynakToplam.ProjeyeGeneleHaric;
                Sonuc s = TNS.YAT.Arac.Tanimla().ButceKaynakKaydet(kullanan, istek.Yil, Senaryo(istek.Senaryo), kaynak, true);
                if (!s.islemSonuc) return Hata(HataMetni(s.hataStr, "Kaynak eklenemedi."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla tamamlandı.", kaynakNo = OrtakFonksiyonlar.ConvertToInt(s.anahtar, 0), ad = kaynak.kaynakAd });
            }
            catch (Exception)
            {
                return Hata("Kaynak eklenemedi.");
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        /// <summary>
        /// Raporlar menüsü (eski YazdirSecildi → ProjeDosyasiYazdir): Proje Dosyası (bütün bölümler tek belgede) ya da bir bölümü.
        /// Alt proje sayfasında dosya adı DPT proje no ve alt proje kodundan oluşur.
        /// </summary>
        private static IResult ProjeDosyasi(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, int pKod, int? apKod, string rapor)
        {
            bool alt = (apKod ?? 0) > 0;
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, alt ? SayfaAltProje : SayfaProje, out Kullanici kullanan);
            if (ret != null) return ret;
            senaryo = Senaryo(senaryo);
            rapor = Temiz(rapor);
            if (yil < 2000 || !SenaryoGecerli(senaryo)) return Hata("Yıl ya da aşama hatalı!");
            if (pKod <= 0) return Hata("Herhangi bir Proje seçilmemiş!");
            if (rapor != "ment" && !System.Text.RegularExpressions.Regex.IsMatch(rapor, "^men([1-9]|1[01])$")) return Hata("Lütfen rapor seçiniz");
            try
            {
                string dptNo = TNS.UZY.Arac.Tanimla().UzayDegeriStr(kullanan, "YATPROJE", yil + "-" + senaryo + "-" + pKod, true, "DPTPROJENO") ?? "";
                string onAd = alt ? dptNo + "-" + apKod : dptNo;
                if (string.IsNullOrWhiteSpace(onAd)) onAd = "ProjeDosyasi";
                AsposeLisans.Yukle();
                string dosya = YatProjeRaporlari.ProjeDosyasiAl(kullanan, rapor, yil, senaryo, pKod, apKod ?? 0, Ayar.Oku("RaporSablonYol"), out string uzanti);
                return DosyaIslem.DosyaGonder(dosya, DosyaAdTemizle(onAd) + "_" + DosyaIslem.DosyaAdUretSade(), true, uzanti);
            }
            catch (RaporHatasi ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        private static string DosyaAdTemizle(string ad)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) ad = ad.Replace(c, '_');
            return ad.Trim();
        }

        /// <summary>Yat. Raporları menüsü (eski btnRaporAl_Click → AltProjeDetay.RaporAl, Rapor.xml 2002).</summary>
        private static IResult YatRapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, int pKod, int? apKod, string rapor)
        {
            bool alt = (apKod ?? 0) > 0;
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, alt ? SayfaAltProje : SayfaProje, out Kullanici kullanan);
            if (ret != null) return ret;
            senaryo = Senaryo(senaryo);
            rapor = Temiz(rapor);
            if (yil < 2000 || !SenaryoGecerli(senaryo)) return Hata("Yıl ya da aşama hatalı!");
            if (!YatProjeRaporlari.YatRaporTanimli(rapor)) return Hata("Lütfen rapor seçiniz");
            try
            {
                AsposeLisans.Yukle();
                // Eski: proje sayfasında alt proje -1, alt proje sayfasında proje ve alt proje kodu.
                string dosya = YatProjeRaporlari.YatRaporAl(kullanan, rapor, yil, senaryo, TNS.DEG.Arac.VarsayilanKurumBul(), pKod, alt ? apKod.Value : -1, Ayar.Oku("RaporSablonYol"));
                return DosyaIslem.DosyaGonder(dosya, "YatRapor_" + rapor + "_" + DosyaIslem.DosyaAdUretSade(), true, Path.GetExtension(dosya).TrimStart('.'));
            }
            catch (RaporHatasi ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        // ------------------------------------------------------------------ Amaç / İlgililer / Gerekçe

        /// <summary>
        /// Amaç ve Kapsam, İlgililer, Gerekçe ve Hedefler sekmelerinin alanları (eski AltProjeBilgiListele). oncekiYil = true:
        /// "Önceki Yıldan Al" (bir önceki yılın değerleri; kaydedilene kadar saklanmaz).
        /// </summary>
        private static IResult BilgiOku(HttpContext baglam, IWebHostEnvironment ortam, string sayfa, int yil, string senaryo, int projeKodu, int? altProjeKodu, bool? oncekiYil)
        {
            if (sayfa == null || !BilgiAlanlari.TryGetValue(sayfa, out string[] alanlar)) return Results.BadRequest();
            IResult ret = YatProjeOrtak.Denetle(baglam, ortam, sayfa, out Kullanici kullanan);
            if (ret != null) return ret;
            senaryo = Senaryo(senaryo);
            if (yil < 2000 || !SenaryoGecerli(senaryo)) return Hata("Yıl ya da aşama hatalı!");
            try
            {
                string[] okunacak = alanlar;
                if (string.Equals(sayfa, "AltProjeAmacGiris", StringComparison.OrdinalIgnoreCase))
                {
                    okunacak = new string[alanlar.Length + AmacOkunanEk.Length];
                    alanlar.CopyTo(okunacak, 0);
                    AmacOkunanEk.CopyTo(okunacak, alanlar.Length);
                }
                int okuYil = oncekiYil == true ? yil - 1 : yil;
                Hashtable ht = TNS.YAT.Arac.Tanimla().AltProjeBilgiListele(kullanan, okuYil, senaryo, projeKodu, altProjeKodu ?? 0, okunacak);
                Dictionary<string, string> degerler = new Dictionary<string, string>();
                foreach (string a in alanlar)
                    degerler[a] = TNS.YAT.Arac.HTDegerAl(ht, a);
                return Results.Json(new { alanlar = degerler });
            }
            catch (Exception)
            {
                // Eski sayfa okuma hatasında alanları boş bırakıyordu.
                return Results.Json(new { alanlar = new Dictionary<string, string>() });
            }
        }

        private static async Task<IResult> BilgiKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, YatBilgiIstegi istek)
        {
            istek ??= new YatBilgiIstegi();
            if (istek.Sayfa == null || !BilgiAlanlari.TryGetValue(istek.Sayfa, out string[] alanlar)) return Results.BadRequest();
            var (ret, kullanan) = await YatProjeOrtak.DenetleYaz(baglam, ortam, sahtecilik, istek.Sayfa);
            if (ret != null) return ret;
            string senaryo = Senaryo(istek.Senaryo);
            if (istek.Yil < 2000 || !SenaryoGecerli(senaryo)) return Hata("Yıl ya da aşama hatalı!");
            if (istek.ProjeKodu <= 0 && istek.AltProjeKodu <= 0) return Hata("Herhangi bir Proje seçilmemiş!");

            Hashtable ht = new Hashtable();
            foreach (string a in alanlar)
            {
                string deger = "";
                if (istek.Alanlar != null) istek.Alanlar.TryGetValue(a, out deger);
                ht[a] = deger ?? "";
            }
            try
            {
                Sonuc s = TNS.YAT.Arac.Tanimla().AltProjeBilgiKaydet(kullanan, istek.Yil, senaryo, istek.ProjeKodu, istek.AltProjeKodu, ht);
                if (!s.islemSonuc) return Hata("Kayıt işlemi başarısız: " + HataMetni(s.hataStr, ""));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarılı." });
            }
            catch (Exception)
            {
                return Hata("Kayıt işlemi başarısız.");
            }
        }
    }
}
