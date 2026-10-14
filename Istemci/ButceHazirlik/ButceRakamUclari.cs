using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ButceHazirlikv2;
using Istemci.ButceKod;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
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

namespace Istemci.ButceHazirlik
{
    public class ButceRakamSatiri
    {
        public string Tertip { get; set; }
        public double Odenek0 { get; set; }
        public double Odenek1 { get; set; }
        public double Odenek2 { get; set; }
        public string Aciklama { get; set; }
    }

    public class ButceRakamKayitIstegi
    {
        public int OdenekTur { get; set; }
        public int Tahmin { get; set; }
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public List<ButceRakamSatiri> Satirlar { get; set; }
    }

    public class ButceKatsayiIstegi
    {
        /// <summary>Altı katsayı satırı, her biri iki değer ("1,0500").</summary>
        public List<List<string>> Katsayilar { get; set; }
    }

    /// <summary>
    /// Bütçe teklifi / yıl sonu tahmini girişi sayfasının (eski ButceHazirlikv2/ButceRakamDuzenleYeni.aspx ve içindeki
    /// ButceRakamDuzenleExt.aspx tablosu) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/butcerakam/tanim, /kodlar, /liste, /indir, /sektorfaaliyet, /rapor, /gelismisrapor
    ///   POST /api/butcerakam/kaydet, /katsayi, /yukle
    /// Sayfa odenekTur (1 gider, 3 gider eko 2, 4 gider eko 4, 2 gelir) ve tahmin (0 teklif, 1 yıl sonu tahmini, 2 tavan üstü)
    /// parametreleriyle farklı tablolara yazar (TabloKodBul); yetki menüdeki ilgili adrese göre denetlenir.
    /// </summary>
    public static class ButceRakamUclari
    {
        private const string SayfaAdres = "../ButceHazirlikv2/ButceRakamDuzenleYeni.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/butcerakam/tanim", Tanim);
            app.MapGet("/api/butcerakam/kodlar", Kodlar);
            app.MapGet("/api/butcerakam/liste", Liste);
            app.MapGet("/api/butcerakam/indir", Indir);
            app.MapGet("/api/butcerakam/sektorfaaliyet", SektorFaaliyet);
            app.MapGet("/api/butcerakam/rapor", Rapor);
            app.MapGet("/api/butcerakam/gelismisrapor", GelismisRapor);
            app.MapPost("/api/butcerakam/kaydet", Kaydet);
            app.MapPost("/api/butcerakam/katsayi", KatsayiKaydet);
            app.MapPost("/api/butcerakam/yukle", Yukle).DisableAntiforgery();
        }

        // ------------------------------------------------------------------ ortak

        private static ENUMEkKolonButceTipi TurBul(int odenekTur)
        {
            if (odenekTur == 2 || odenekTur == 3 || odenekTur == 4) return (ENUMEkKolonButceTipi)odenekTur;
            return ENUMEkKolonButceTipi.GIDER;
        }

        private static int TahminBul(int tahmin)
        {
            return tahmin == 1 || tahmin == 2 ? tahmin : 0;
        }

        /// <summary>Menüdeki adres (eski menülerde ?odenekTur=..&amp;tahmin=..).</summary>
        private static string MenuAdres(int odenekTur, int tahmin)
        {
            return SayfaAdres + "?odenekTur=" + odenekTur + (tahmin > 0 ? "&tahmin=" + tahmin : "");
        }

        /// <summary>Kullanıcı sayfanın bu türünü (menüdeki ilgili seçeneği) kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, int odenekTur, int tahmin)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                MenuOkuyucu menu = new MenuOkuyucu(kullanan);
                string dosya = MenuOkuyucu.DosyaYolu(uygulamaKlasoru);
                return menu.SayfaYetkili(dosya, MenuAdres((int)TurBul(odenekTur), TahminBul(tahmin)))
                    || (odenekTur == 1 && tahmin == 0 && menu.SayfaYetkili(dosya, SayfaAdres));
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int tahmin, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, odenekTur, tahmin)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata });
        }

        /// <summary>JSON NaN taşıyamaz: hesaplanamayan oranlar boş gider.</summary>
        private static double? Sayi(double d)
        {
            return double.IsNaN(d) || double.IsInfinity(d) ? (double?)null : d;
        }

        /// <summary>Eski SayfaAdEkBul.</summary>
        private static string SayfaAdEkBul(int tahmin)
        {
            if (tahmin == 1) return "yıl sonu harcama tahmini";
            else if (tahmin == 2) return "tavan üstü";
            else return "ödenekleri";
        }

        /// <summary>Eski ButceRakamDuzenleExt.TabloKodBul.</summary>
        public static ENUMTabloKod TabloKodBul(ENUMEkKolonButceTipi odenekTur, int tahmin)
        {
            if (odenekTur == ENUMEkKolonButceTipi.GELIR && tahmin == 0)
                return ENUMTabloKod.TABPEBGelir;
            else if (odenekTur == ENUMEkKolonButceTipi.GELIR && tahmin == 1)
                return ENUMTabloKod.TABGelirTahmini;
            else if (odenekTur == ENUMEkKolonButceTipi.GIDER && tahmin == 0)
                return ENUMTabloKod.TABPEBOdenek;
            else if (odenekTur == ENUMEkKolonButceTipi.GIDER && tahmin == 2)
                return ENUMTabloKod.TABPEBTavanUstu;
            else if (odenekTur == ENUMEkKolonButceTipi.GIDEREKO2 && tahmin == 2)
                return ENUMTabloKod.TABPEBTavanUstu;
            else if (odenekTur == ENUMEkKolonButceTipi.GIDEREKO2 && tahmin == 1)
                return ENUMTabloKod.TABHarcamaTahmin;
            else if (odenekTur == ENUMEkKolonButceTipi.GIDEREKO2)
                return ENUMTabloKod.TABPEBOdenek;
            else if (odenekTur == ENUMEkKolonButceTipi.GIDEREKO4)
                return ENUMTabloKod.TABOdenek;
            else
                return ENUMTabloKod.TABHarcamaTahmin;
        }

        private static bool AciklamaVar(ENUMEkKolonButceTipi tur)
        {
            return !(tur == ENUMEkKolonButceTipi.GIDEREKO2 || tur == ENUMEkKolonButceTipi.GIDEREKO4);
        }

        /// <summary>Eski BHMv2Ortak.KullaniciHazirlikYiliBul: kullanıcının son listelediği yıl, yoksa bütçe hazırlık yılı.</summary>
        internal static int KullaniciYilBul(Kullanici kullanan)
        {
            int hYil = KodTanimUclari.YilBul(null);
            string s = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ButceYil");
            return string.IsNullOrEmpty(s) ? hYil : OrtakFonksiyonlar.ConvertToInt(s, hYil);
        }

        /// <summary>Eski BHMv2Ortak.KullaniciSenaryoDegerBul: kullanıcının son listelediği aşama, yoksa 00.00.</summary>
        internal static string KullaniciSenaryoBul(Kullanici kullanan)
        {
            string s = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "Senaryo");
            return string.IsNullOrEmpty(s) ? "00.00" : s;
        }

        // ------------------------------------------------------------------ tanım

        private const string KatsayiKullanici = "stratek"; // eski sayfa katsayıları bu kullanıcının değişkenlerinde saklıyordu
        private static readonly string[] KatsayiAdlari = { "Mal/Hizmet", "SGK", "Personel", "Yatırımlar", "Cari Transferler", "Sermaye Transferleri" };

        /// <summary>Eski KatsayilariYaz: altı katsayı satırı (ButceKatsayi1..6 = "bir;iki", yoksa 1).</summary>
        private static List<object> KatsayilariOku()
        {
            List<object> liste = new List<object>();
            for (int i = 0; i < KatsayiAdlari.Length; i++)
            {
                double bir = 1;
                double iki = 1;
                string s = GenelIslemler.KullaniciDegiskenGetir(KatsayiKullanici, "ButceKatsayi" + (i + 1));
                if (s != "")
                {
                    string[] degerler = s.Split(';');
                    if (degerler.Length >= 2)
                    {
                        bir = OrtakFonksiyonlar.ConvertToDbl(degerler[0]);
                        iki = OrtakFonksiyonlar.ConvertToDbl(degerler[1]);
                    }
                }
                liste.Add(new { ad = KatsayiAdlari[i], bir = bir.ToString("0.0000"), iki = iki.ToString("0.0000") });
            }
            return liste;
        }

        private static bool SektorVarMi(Kullanici kullanan, int yil, string senaryoKod)
        {
            try
            {
                ObjectArray oa = TNS.BHMv2.Arac.Tanimla().SektorKullanilmisMi(kullanan, yil, senaryoKod, "");
                return oa != null && oa.sonuc.islemSonuc && oa.ObjeSayisi > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Sayfanın açılış bilgileri (eski Page_Load): başlık, yıl / ay / aşama, gizlenecek kod alanları, menüler, katsayılar ve
        /// gelişmiş rapor seçimleri.
        /// </summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int tahmin)
        {
            IResult ret = Denetle(baglam, ortam, odenekTur, tahmin, out Kullanici kullanan);
            if (ret != null) return ret;
            ENUMEkKolonButceTipi tur = TurBul(odenekTur);
            tahmin = TahminBul(tahmin);

            try
            {
                string baslik = tur == ENUMEkKolonButceTipi.GELIR ? "Gelir Teklifi" : "Gider Teklifi";
                if (tur == ENUMEkKolonButceTipi.GIDEREKO2) baslik += " (Eko 2)";
                else if (tur == ENUMEkKolonButceTipi.GIDEREKO4) baslik += " (Eko 4)";
                if (tahmin == 1) baslik += " (Yıl Sonu Tahmini)";
                else if (tahmin == 2) baslik += " (Tavan Üstü)";

                int yil = KullaniciYilBul(kullanan);
                string senaryo = KullaniciSenaryoBul(kullanan);

                List<object> senaryolar = new List<object>();
                ButceSenaryo[] bsler = TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null);
                foreach (ButceSenaryo bs in bsler ?? new ButceSenaryo[0])
                    senaryolar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });

                List<object> ekKolonlar = new List<object>();
                for (int i = 0; i < EkKolonlar.staticEkKolonlar.Length; i++)
                    ekKolonlar.Add(new { no = i, ad = EkKolonlar.staticEkKolonlar[i].kolonGrupAdi });

                bool gelir = tur == ENUMEkKolonButceTipi.GELIR;
                return Results.Json(new
                {
                    baslik = baslik,
                    yil = yil,
                    ay = DateTime.Now.Month,
                    senaryo = senaryo,
                    senaryolar = senaryolar,
                    kurum = TNS.DEG.Arac.VarsayilanKurumBul(),
                    gizli = new
                    {
                        kurumsal = gelir && TNS.BKD.Arac.GelirKurumMu(yil),
                        fonksiyonel = gelir,
                        finansman = gelir,
                        faaliyet = tur == ENUMEkKolonButceTipi.GIDEREKO4
                    },
                    ekoKodTur = gelir ? "5" : "4",
                    tahmin = tahmin,
                    aciklamaVar = AciklamaVar(tur),
                    sistemYoneticisi = GenelGorevUclari.YetkiVar(kullanan),
                    sektorVar = SektorVarMi(kullanan, yil, senaryo),
                    raporlar = gelir ? BHGiderRaporlari.ListeGelir() : BHGiderRaporlari.ListeGider(true),
                    ekKolonlar = ekKolonlar,
                    katsayilar = KatsayilariOku(),
                    gelismis = ButceRakamRaporlari.SecimOku(kullanan, tur)
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        // ------------------------------------------------------------------ kod ağaçları

        /// <summary>
        /// Kod ağacının bir düzeyi (eski KodAgac.KodDoldurExt). Kurumsal (1), fonksiyonel (2) ve faaliyet (14) ağaçları eski sayfada
        /// bütçe hazırlık yetki koduyla (20) açılıyordu: kökte, kullanıcının ilgili yetki tiplerinde değer varsa ve hiçbiri "*"
        /// değilse yalnızca o kodlar gösterilir.
        /// </summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int tahmin, int yil, string tur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, odenekTur, tahmin, out Kullanici kullanan);
            if (ret != null) return ret;
            return KodDuzeyi(kullanan, yil, tur, ust);
        }

        /// <summary>Kod ağacının bir düzeyi; yetki denetimini çağıran yapar (bütçe fişi sayfası da kullanır).</summary>
        internal static IResult KodDuzeyi(Kullanici kullanan, int yil, string tur, string ust)
        {
            if (yil < 2000) return Hata("Yıl hatalı!");
            tur = (tur ?? "").Trim();
            if (Array.IndexOf(new[] { "1", "2", "3", "4", "5", "7", "14" }, tur) < 0) return Hata("Kod türü hatalı!");
            ust = (ust ?? "").Trim();

            try
            {
                IBKDServis servis = TNS.BKD.Arac.Tanimla();
                string kirilim = TNS.DEG.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD" + tur + "/KODYAPI") ?? "";
                int kirilimBoy = kirilim == "" ? 0 : kirilim.Split('.').Length;
                List<object> dugumler = new List<object>();

                int[] damgaNo = tur == "1" ? new[] { 36, 37, 38, 91, 92, 93 } : tur == "2" ? new[] { 60, 61, 62 } : tur == "14" ? new[] { 55, 56, 57 } : null;
                if (ust == "" && damgaNo != null)
                {
                    List<string> yetkili = new List<string>();
                    bool yildizVar = false;
                    foreach (int damga in damgaNo)
                    {
                        foreach (YetkiTipi yt in TNS.KYM.Arac.yetkiTipiListesi)
                        {
                            if (yt.kod != damga) continue;
                            string deger = kullanan.YetkiTipiDegeriBul(yt.ad) ?? "";
                            if (deger.Contains("*")) { yildizVar = true; break; }
                            foreach (string parca in deger.Split(',', ';'))
                            {
                                string ekle = parca.Replace(".", "").Trim();
                                if (ekle != "" && !yetkili.Contains(ekle)) yetkili.Add(ekle);
                            }
                        }
                        if (yildizVar) break;
                    }
                    if (!yildizVar && yetkili.Count > 0)
                    {
                        foreach (string kod in yetkili)
                        {
                            string noktali = OrtakFonksiyonlar.KodNoktaKoy(kirilim, kod);
                            string ad = servis.KodAdBul(kullanan, yil, tur, noktali);
                            dugumler.Add(new { kod = noktali, ad = noktali + "- " + ad, altVar = noktali.Split('.').Length < kirilimBoy });
                        }
                        return Results.Json(new { dugumler = dugumler });
                    }
                }

                int ustUz = 0;
                if (ust != "")
                {
                    ust = OrtakFonksiyonlar.KodNoktaKoy(kirilim, ust.Replace(".", ""));
                    ustUz = ust.Split('.').Length;
                }
                if (ustUz >= kirilimBoy) return Results.Json(new { dugumler = dugumler });
                bool altVar = ustUz < kirilimBoy - 1;

                ABSKodlarVAN kodlar = servis.KodListeleSeviyeIkiYil(yil, tur, ust, 0, false, false, "", "", "", true, true, "", false);
                if (kodlar == null || !kodlar.sonuc.islemSonuc) return Hata("Kodlar okunamadı.");
                string dil = string.IsNullOrEmpty(kullanan.aktifKultur) ? TNS.BKD.Arac.Diller(0) : kullanan.aktifKultur;
                foreach (ABSKod k in kodlar.absKodlar)
                    dugumler.Add(new { kod = k.kod, ad = k.kod + "- " + k.Ad(dil), altVar = altVar });
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Kodlar okunamadı.");
            }
        }

        // ------------------------------------------------------------------ liste

        private class ListeSonucu
        {
            public string hata;
            public string baslik;
            public string kodBaslik;
            public int veriYil;
            public int veriGirisSay;
            public List<object[]> satirlar = new List<object[]>();
            public double[] toplam;
            public int ekKolonNo;
            public int ekBilgiSay;
        }

        /// <summary>
        /// Eski ButceRakamDuzenleExt.OdenekListele: seçime göre tertipler, girilen değerler ve ek sütun değerleri. Her satır
        /// { tertip, gösterilen tertip, tertip adı, değerler..., açıklama, ek sütunlar... } dizisidir (eski alDis).
        /// </summary>
        private static ListeSonucu Listele(Kullanici kullanan, ENUMEkKolonButceTipi odenekTur, int tahmin, int secYil, int ay, string secSenaryo,
            string kurkod, string fonkod, string finkod, string ekokod, string pebkod, int ekKolonNo, bool yaz)
        {
            ListeSonucu ls = new ListeSonucu();
            IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            bool aciklamaVar = AciklamaVar(odenekTur);
            if (ekKolonNo >= EkKolonlar.staticEkKolonlar.Length) ekKolonNo = -1;
            ls.ekKolonNo = ekKolonNo;

            int veriYil = secYil;
            if (tahmin == 1) veriYil--;
            ls.veriYil = veriYil;
            ls.baslik = string.Format("{0} yılı {1} {2}", veriYil, servis.ButceSenaryoAdBul(kullanan.aktifKultur, secSenaryo), SayfaAdEkBul(tahmin));

            kurkod = (kurkod ?? "").Trim();
            if (string.IsNullOrEmpty(kurkod))
                kurkod = TNS.DEG.Arac.VarsayilanKurumBul();

            string hataStr = "";
            if (secYil <= 0) hataStr += "Yıl alanı boş bırakılamaz! ";
            if (string.IsNullOrEmpty(secSenaryo)) hataStr += "Aşama alanı boş bırakılamaz! ";
            if (string.IsNullOrEmpty(kurkod)) hataStr += "Kurumsal Kod Girilmelidir! ";
            if (hataStr != "") { ls.hata = hataStr.Trim(); return ls; }

            Tertip tUz = new Tertip(secYil, ENUMTabloKod.TABPEBOdenek);
            KistasYapi kY = new KistasYapi();
            if (odenekTur == ENUMEkKolonButceTipi.GELIR)
                kurkod = TNS.BKD.Arac.KurumsaldanKuruma(kurkod, tUz.Kirilimlar(0));
            kY.ekleTertipler = new Tertip[1];
            if (odenekTur == ENUMEkKolonButceTipi.GIDER && tahmin == 0)
                kY.ekleTertipler[0] = new Tertip(secYil, ENUMTabloKod.TABPEBOdenek, kurkod, fonkod, finkod, ekokod, pebkod);
            else if (odenekTur == ENUMEkKolonButceTipi.GIDER && tahmin == 2)
                kY.ekleTertipler[0] = new Tertip(secYil, ENUMTabloKod.TABPEBTavanUstu, kurkod, fonkod, finkod, ekokod, pebkod);
            else if (odenekTur == ENUMEkKolonButceTipi.GIDER && tahmin == 1)
                kY.ekleTertipler[0] = new Tertip(secYil, ENUMTabloKod.TABHarcamaTahmin, kurkod, fonkod, finkod, ekokod, pebkod);
            else if (odenekTur == ENUMEkKolonButceTipi.GIDEREKO2 && tahmin == 1)
                kY.ekleTertipler[0] = new Tertip(secYil, ENUMTabloKod.TABHarcamaTahmin, kurkod, fonkod, finkod, ekokod, pebkod);
            else if (odenekTur == ENUMEkKolonButceTipi.GIDEREKO2 && tahmin == 2)
                kY.ekleTertipler[0] = new Tertip(secYil, ENUMTabloKod.TABPEBTavanUstu, kurkod, fonkod, finkod, ekokod, pebkod);
            else if (odenekTur == ENUMEkKolonButceTipi.GIDEREKO2)
                kY.ekleTertipler[0] = new Tertip(secYil, ENUMTabloKod.TABPEBOdenek, kurkod, fonkod, finkod, ekokod, pebkod);
            else if (odenekTur == ENUMEkKolonButceTipi.GIDEREKO4)
                kY.ekleTertipler[0] = new Tertip(secYil, ENUMTabloKod.TABOdenek, kurkod, fonkod, finkod, ekokod);
            else if (odenekTur == ENUMEkKolonButceTipi.GELIR && tahmin == 0)
                kY.ekleTertipler[0] = new Tertip(secYil, ENUMTabloKod.TABPEBGelir, kurkod, ekokod, pebkod);
            else
                kY.ekleTertipler[0] = new Tertip(secYil, ENUMTabloKod.TABGelirTahmini, kurkod, ekokod, pebkod);

            GenelIslemler.KullaniciDegiskenSakla(kullanan, "ButceYil", secYil.ToString());
            GenelIslemler.KullaniciDegiskenSakla(kullanan, "Senaryo", secSenaryo);

            EkDegerParam[] tabloEkbilgiler = null;
            if (ekKolonNo > -1)
                tabloEkbilgiler = EkKolonlar.staticEkKolonlar[ekKolonNo].EkdegerParamDondur(odenekTur, ay, secSenaryo);

            TertipOdenek[] odnler = servis.ButceOdenekListele(kullanan, secSenaryo, kY, tabloEkbilgiler, tahmin, aciklamaVar ? 0 : -1, odenekTur == ENUMEkKolonButceTipi.GIDEREKO2);

            int ekoYer = -1;
            bool hepsiTam = false;
            if (kY.ekleTertipler.Length == 1)
            {
                Tertip t = kY.ekleTertipler[0];
                if (t.tabloKod == ENUMTabloKod.TABPEBGelir || t.tabloKod == ENUMTabloKod.TABGelirTahmini)
                    ekoYer = 1;
                else
                    ekoYer = 3;
                hepsiTam = true;
                for (int i = 0; i < t.KodNoSay() && hepsiTam; i++)
                {
                    if (i != ekoYer)
                    {
                        if (t.kodlar[i].Replace(".", "").Length != t.Kirilimlar(i).Replace(".", "").Length)
                            hepsiTam = false;
                    }
                }
            }

            if (odnler == null || odnler.Length == 0)
            {
                ls.hata = "Aranan kriterlere uygun tertip bulunamadı!";
                return ls;
            }

            int veriGirisSay = tahmin == 1 ? 1 : 3;
            int ekBilgiSay = tabloEkbilgiler == null ? 0 : tabloEkbilgiler.Length;
            ls.veriGirisSay = veriGirisSay;
            ls.ekBilgiSay = ekBilgiSay;
            double[] toplam = new double[veriGirisSay + ekBilgiSay];

            for (int islemSatir = 0; islemSatir < odnler.Length; islemSatir++)
            {
                TertipOdenek od = odnler[islemSatir];
                if (string.IsNullOrEmpty(od.tertip.tertipAdi))
                    od.tertip.KoddanAdOl();

                string tertipStr = od.tertip.TertipStrOl();
                List<object> al = new List<object>();
                al.Add(tertipStr);
                string tertipAdStr;
                if (hepsiTam)
                {
                    tertipAdStr = od.tertip.adlar[ekoYer];
                    tertipStr = od.tertip.kodlar[ekoYer];
                    if (string.IsNullOrEmpty(tertipAdStr))
                    {
                        if (od.tertip.tabloKod == ENUMTabloKod.TABPEBGelir || od.tertip.tabloKod == ENUMTabloKod.TABGelirTahmini)
                            tertipAdStr = GenelIslemler.KodAd(5, od.tertip.yil + "-" + tertipStr, false);
                        else
                            tertipAdStr = GenelIslemler.KodAd(4, od.tertip.yil + "-" + tertipStr, false);
                    }
                }
                else
                    tertipAdStr = od.tertip.tertipAdi;
                if (string.IsNullOrEmpty(tertipAdStr)) tertipAdStr = tertipStr;

                if (yaz && TNS.MUH.Arac.prgBag)
                    al.Add(TNS.BHMv2.Arac.ProgramTertipOl(od.tertip, false));
                else
                    al.Add(tertipStr);
                al.Add(tertipAdStr);

                for (int sut = 0; sut < od.odenek.Length && sut < toplam.Length; sut++)
                    toplam[sut] += od.odenek[sut];
                for (int sut = 0; sut < veriGirisSay; sut++)
                    al.Add(sut < od.odenek.Length ? od.odenek[sut] : 0);
                al.Add(od.aciklama);

                if (ekKolonNo >= 0)
                {
                    double[] dizistr = EkKolonlar.staticEkKolonlar[ekKolonNo].Hesapla(od.odenek, veriGirisSay);
                    for (int j = 0; j < dizistr.Length; j++)
                        al.Add(dizistr[j]);
                }
                ls.satirlar.Add(al.ToArray());
            }
            ls.toplam = toplam;

            string birimAd = GenelIslemler.KodAd(1, secYil + "-" + kurkod, false);
            ls.kodBaslik = birimAd + "- " + (hepsiTam ? "Ekonomik Kodlar" : "Tertip");
            return ls;
        }

        private static object Satir(object[] s, int veriGirisSay, int ekBaslangic)
        {
            double?[] degerler = new double?[veriGirisSay];
            for (int i = 0; i < veriGirisSay; i++) degerler[i] = Sayi((double)s[3 + i]);
            List<double?> ek = new List<double?>();
            for (int i = ekBaslangic; i < s.Length; i++) ek.Add(Sayi((double)s[i]));
            return new { tertip = s[0], tertipGor = s[1], tertipAd = s[2], degerler = degerler, aciklama = s[3 + veriGirisSay] ?? "", ek = ek };
        }

        /// <summary>Listele (eski Listele / GridInit / FpLToplamSatirDoldur): sütun başlıkları, satırlar ve toplamlar.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int tahmin, int yil, int ay, string senaryo,
            string kur, string fon, string fin, string eko, string peb, int ekBilgi)
        {
            IResult ret = Denetle(baglam, ortam, odenekTur, tahmin, out Kullanici kullanan);
            if (ret != null) return ret;
            ENUMEkKolonButceTipi tur = TurBul(odenekTur);
            tahmin = TahminBul(tahmin);
            if (ay < 1 || ay > 12) ay = DateTime.Now.Month;

            try
            {
                ListeSonucu ls = Listele(kullanan, tur, tahmin, yil, ay, (senaryo ?? "").Trim(), kur, fon, fin, eko, peb, ekBilgi, false);
                if (ls.hata != null) return Hata(ls.hata);

                List<object> ekSutunlar = new List<object>();
                double?[] ekToplam = new double?[0];
                if (ls.ekKolonNo >= 0)
                {
                    EkKolonlar ekk = EkKolonlar.staticEkKolonlar[ls.ekKolonNo];
                    for (int i = 0; i < ekk.KolonSayisi(); i++)
                        ekSutunlar.Add(new { ust = ekk.KolonBasligiGetir(i, 0, yil, ay), alt = ekk.KolonBasligiGetir(i, 1, yil, ay), oran = ekk.OranMi(i) });
                    double[] h = ekk.Hesapla(ls.toplam, ls.veriGirisSay);
                    ekToplam = new double?[h.Length];
                    for (int i = 0; i < h.Length; i++) ekToplam[i] = Sayi(h[i]);
                }
                double?[] toplam = new double?[ls.veriGirisSay];
                for (int i = 0; i < ls.veriGirisSay; i++) toplam[i] = Sayi(ls.toplam[i]);

                List<object> satirlar = new List<object>();
                foreach (object[] s in ls.satirlar)
                    satirlar.Add(Satir(s, ls.veriGirisSay, 4 + ls.veriGirisSay));

                return Results.Json(new
                {
                    baslik = ls.baslik,
                    kodBaslik = ls.kodBaslik,
                    veriYil = ls.veriYil,
                    veriGirisSay = ls.veriGirisSay,
                    aciklamaVar = AciklamaVar(tur),
                    detayGiris = tur == ENUMEkKolonButceTipi.GIDEREKO2 || tur == ENUMEkKolonButceTipi.GIDEREKO4,
                    ekSutunlar = ekSutunlar,
                    satirlar = satirlar,
                    toplam = toplam,
                    ekToplam = ekToplam
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste hazırlanamadı."));
            }
        }

        /// <summary>İndir (eski Indir / ExceleYaz): listelenen tablo Excel dosyası olarak.</summary>
        private static IResult Indir(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int tahmin, int yil, int ay, string senaryo,
            string kur, string fon, string fin, string eko, string peb, int ekBilgi)
        {
            IResult ret = Denetle(baglam, ortam, odenekTur, tahmin, out Kullanici kullanan);
            if (ret != null) return ret;
            ENUMEkKolonButceTipi tur = TurBul(odenekTur);
            tahmin = TahminBul(tahmin);
            if (ay < 1 || ay > 12) ay = DateTime.Now.Month;
            senaryo = (senaryo ?? "").Trim();

            try
            {
                ListeSonucu ls = Listele(kullanan, tur, tahmin, yil, ay, senaryo, kur, fon, fin, eko, peb, ekBilgi, true);
                if (ls.hata != null) return Hata(ls.hata);

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string dosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
                XLS.BosDosyaAc(dosyaAd);

                // Eski ExcelInit
                XLS.HucreDegerYaz(0, 0, ls.baslik);
                XLS.HucreDegerYaz(1, 0, "Tertip Kodu");
                XLS.HucreDegerYaz(1, 1, "Gör. Tertip Kodu");
                XLS.HucreDegerYaz(1, 2, "Tertip Açıklaması");
                XLS.HucreBirlestir(0, 0, 0, 5);
                int degBaslaSutun = tahmin == 1 ? 1 : 3;
                for (int sut = 0; sut < degBaslaSutun; sut++)
                    XLS.HucreDegerYaz(1, sut + 3, (ls.veriYil + sut).ToString());
                XLS.HucreDegerYaz(1, degBaslaSutun + 3, "Açıklama");
                degBaslaSutun++;
                if (!AciklamaVar(tur))
                    XLS.SutunGenislikAyarla(degBaslaSutun + 3, degBaslaSutun + 3, 0);
                if (ls.ekKolonNo >= 0)
                    EkKolonlar.staticEkKolonlar[ls.ekKolonNo].ExceleEkle(XLS, 0, degBaslaSutun + 3, yil, ay);

                int sat = 2;
                foreach (object[] al in ls.satirlar)
                {
                    int sut = 0;
                    foreach (object o in al)
                    {
                        if (o != null)
                        {
                            if (o is double dbl)
                            {
                                if (!double.IsNaN(dbl))
                                    XLS.HucreDegerYaz(sat, sut++, dbl);
                                else
                                    sut++;
                            }
                            else
                                XLS.HucreDegerYaz(sat, sut++, o.ToString());
                        }
                        else sut++;
                    }
                    sat++;
                }
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(dosyaAd, "Liste_" + DateTime.Now.Ticks, true, "xlsx");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Dosya hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>Kaydet (eski OdenekleriSakla): değiştirilen satırlar listelenen tabloya yazılır.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, ButceRakamKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.OdenekTur, istek.Tahmin, out Kullanici kullanan);
            if (ret != null) return ret;
            ENUMEkKolonButceTipi tur = TurBul(istek.OdenekTur);
            int tahmin = TahminBul(istek.Tahmin);
            string senaryo = (istek.Senaryo ?? "").Trim();
            if (istek.Yil < 2000 || senaryo == "") return Results.Json(new { basarili = false, hata = "Önce listeleyiniz." });

            try
            {
                ENUMTabloKod tabloKod = TabloKodBul(tur, tahmin);
                int girilenKolon = tahmin == 1 ? 1 : 3;
                bool aciklamaVar = AciklamaVar(tur);

                List<TertipOdenek> alIc = new List<TertipOdenek>();
                foreach (ButceRakamSatiri gb in istek.Satirlar ?? new List<ButceRakamSatiri>())
                {
                    if (gb == null || string.IsNullOrWhiteSpace(gb.Tertip)) continue;
                    TertipOdenek to = new TertipOdenek(girilenKolon);
                    to.tertip = new Tertip(istek.Yil, tabloKod);
                    to.tertip.TertipAta(gb.Tertip);
                    to.odenek[0] = gb.Odenek0;
                    if (tahmin != 1)
                    {
                        to.odenek[1] = gb.Odenek1;
                        to.odenek[2] = gb.Odenek2;
                    }
                    to.aciklama = gb.Aciklama;
                    alIc.Add(to);
                }
                if (alIc.Count == 0) return Results.Json(new { basarili = false, hata = "Kaydedilecek kayıt bulunamadı veya değişiklik yapılmamış" });

                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                Sonuc sonuc;
                if (tahmin == 1)
                    sonuc = servis.ButceYSTahminKaydet(kullanan, senaryo, alIc.ToArray());
                else
                    sonuc = servis.ButceOdenekKaydet(kullanan, senaryo, alIc.ToArray(), aciklamaVar);

                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla tamamlandı." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>Katsayıları saklar (eski btnKaydetKatsayi_Click: "stratek" kullanıcısının ButceKatsayi1..6 değişkenleri).</summary>
        private static async Task<IResult> KatsayiKaydet(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int tahmin, ButceKatsayiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, odenekTur, tahmin, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Katsayilar == null) return Results.Json(new { basarili = false, hata = "Katsayı bulunamadı." });

            try
            {
                Kullanici stratek = new Kullanici();
                stratek.kullaniciKodu = KatsayiKullanici;
                for (int i = 0; i < istek.Katsayilar.Count && i < KatsayiAdlari.Length; i++)
                {
                    List<string> k = istek.Katsayilar[i];
                    if (k == null || k.Count < 2) continue;
                    Sonuc s = GenelIslemler.KullaniciDegiskenSakla(stratek, "ButceKatsayi" + (i + 1), (k[0] ?? "").Trim() + ";" + (k[1] ?? "").Trim());
                    if (s != null && !s.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(s.hataStr, "Katsayılar saklanamadı.") });
                }
                return Results.Json(new { basarili = true, bilgi = "Katsayılar başarı ile saklandı." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Katsayılar saklanamadı." });
            }
        }

        /// <summary>
        /// Dosya Yükle (eski FileUpload_Click, yalnızca sistem yöneticisi): Excel'in her satırı tertip ve üç yılın tutarıdır; boş
        /// tertipte durulur. Teklif / tavan üstü ödenek, yıl sonu tahmininde tahmin tablosuna yazılır.
        /// </summary>
        private static async Task<IResult> Yukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IFormCollection form = await baglam.Request.ReadFormAsync();
            int odenekTur = OrtakFonksiyonlar.ConvertToInt(form["odenekTur"].ToString(), 1);
            int tahmin = TahminBul(OrtakFonksiyonlar.ConvertToInt(form["tahmin"].ToString(), 0));
            IResult ret = Denetle(baglam, ortam, odenekTur, tahmin, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!GenelGorevUclari.YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            int secYil = OrtakFonksiyonlar.ConvertToInt(form["yil"].ToString(), 0);
            string secSenaryo = form["senaryo"].ToString().Trim();
            if (secYil < 2000 || secSenaryo == "") return Results.Json(new { basarili = false, hata = "Yıl ve aşama seçiniz." });

            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return Results.Json(new { basarili = false, hata = "Yükleme için bir dosya seçiniz" });
            if (dosya.Length > 20 * 1024 * 1024) return Results.Json(new { basarili = false, hata = "Dosya çok büyük (en çok 20 MB)." });

            string dosyaAdxls = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul() + "." + GenelIslemler.ExcelTur();
            try
            {
                using (FileStream hedef = File.Create(dosyaAdxls))
                    await dosya.CopyToAsync(hedef);
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Dosya sunucuda saklanamadı." });
            }

            try
            {
                AsposeLisans.Yukle();
                ENUMTabloKod tabloKod = TabloKodBul(TurBul(odenekTur), tahmin);
                List<TertipOdenek> kodlar = new List<TertipOdenek>();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(dosyaAdxls);
                try
                {
                    for (int sat = 0; ; sat++)
                    {
                        string tertip = XLS.HucreDegerAl(sat, 0);
                        if (string.IsNullOrWhiteSpace(tertip)) break;
                        TertipOdenek to = new TertipOdenek(3);
                        to.tertip = new Tertip(secYil, tabloKod);
                        to.tertip.TertipAta(tertip);
                        to.odenek[0] = OrtakFonksiyonlar.ConvertToDbl(XLS.HucreDegerAl(sat, 1));
                        to.odenek[1] = OrtakFonksiyonlar.ConvertToDbl(XLS.HucreDegerAl(sat, 2));
                        to.odenek[2] = OrtakFonksiyonlar.ConvertToDbl(XLS.HucreDegerAl(sat, 3));
                        kodlar.Add(to);
                    }
                }
                finally
                {
                    XLS.DosyaKapat();
                }

                if (kodlar.Count == 0) return Results.Json(new { basarili = false, hata = "Kaydedilecek kayıt bulunamadı veya değişiklik yapılmamış" });
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                Sonuc sonuc = tahmin == 1 ? servis.ButceYSTahminKaydet(kullanan, secSenaryo, kodlar.ToArray()) : servis.ButceOdenekKaydet(kullanan, secSenaryo, kodlar.ToArray());
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Aktarım başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Aktarım tamamlandı." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Dosya okunamadı." });
            }
            finally
            {
                try { File.Delete(dosyaAdxls); } catch { }
            }
        }

        // ------------------------------------------------------------------ diğer

        /// <summary>Sektör Faaliyetlerini Bul (eski btnSektorFaalGetir_Click): sektörde kullanılan faaliyet kodları, virgülle.</summary>
        private static IResult SektorFaaliyet(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int tahmin, int yil, string senaryo, string sektor)
        {
            IResult ret = Denetle(baglam, ortam, odenekTur, tahmin, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                ObjectArray oa = TNS.BHMv2.Arac.Tanimla().SektorKullanilmisMi(kullanan, yil, (senaryo ?? "").Trim(), TNS.BHMv2.Arac.TiredenKodBul(sektor ?? ""));
                if (oa == null) return Hata("Sektör faaliyetleri okunamadı.");
                if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc.hataStr, "Sektör faaliyetleri okunamadı."));
                List<string> kodlar = new List<string>();
                for (int i = 0; i < oa.ObjeSayisi; i++)
                    kodlar.Add(((ABSKod)oa.objeler[i]).kod);
                return Results.Json(new { kodlar = string.Join(",", kodlar) });
            }
            catch (Exception)
            {
                return Hata("Sektör faaliyetleri okunamadı.");
            }
        }

        /// <summary>Rapor Üret menüsü (eski btnRaporAl_Click → BHMv2Ortak.RaporAl): eski Rapor.xml raporları ekrandaki seçimle.</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int tahmin, int yil, string senaryo,
            string kur, string fon, string fin, string eko, string peb, string rapor)
        {
            IResult ret = Denetle(baglam, ortam, odenekTur, tahmin, out Kullanici kullanan);
            if (ret != null) return ret;
            if (string.IsNullOrEmpty(rapor)) return Hata("Lütfen rapor seçiniz");
            if (yil < 2000) return Hata("Yıl hatalı!");
            if (!BHGiderRaporlari.Tanimli(rapor, TurBul(odenekTur) == ENUMEkKolonButceTipi.GELIR, true)) return Hata("Lütfen rapor seçiniz");
            try
            {
                AsposeLisans.Yukle();
                string dosya = BHGiderRaporlari.RaporAl(kullanan, rapor, yil, (senaryo ?? "").Trim(), (kur ?? "").Trim(), (fon ?? "").Trim(),
                    (fin ?? "").Trim(), (eko ?? "").Trim(), (peb ?? "").Trim(), Ayar.Oku("RaporSablonYol"));
                return DosyaIslem.DosyaGonder(dosya, "Rapor_" + rapor + "_" + DosyaIslem.DosyaAdUretSade(), true, Path.GetExtension(dosya).TrimStart('.'));
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

        /// <summary>
        /// Rapor Al penceresi (eski btnRaporAlGelismis_Click): seçilen kod kırılımları ve değer sütunlarıyla liste ya da üçgen
        /// rapor. Seçimler kullanıcı değişkeninde saklanır. p: { alanlar, degerler, araToplam, aciklamaOlsun, ucgen, OEYeni, OEOnayli, OEYevmiyeli }.
        /// </summary>
        private static IResult GelismisRapor(HttpContext baglam, IWebHostEnvironment ortam, int odenekTur, int tahmin, int yil, int ay, string senaryo,
            string kur, string fon, string fin, string eko, string peb, string p)
        {
            IResult ret = Denetle(baglam, ortam, odenekTur, tahmin, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            ButceRakamRaporlari.GelismisRaporPar grp;
            try
            {
                grp = JsonSerializer.Deserialize<ButceRakamRaporlari.GelismisRaporPar>(p ?? "", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception)
            {
                grp = null;
            }
            if (grp == null || grp.alanlar == null || grp.degerler == null) return Hata("Rapor parametreleri hatalı.");
            if (ay < 1 || ay > 12) ay = 1;

            try
            {
                AsposeLisans.Yukle();
                string dosya = new ButceRakamRaporlari(kullanan, TurBul(odenekTur), Ayar.Oku("RaporSablonYol"))
                    .RaporAl(grp, yil, ay, (senaryo ?? "").Trim(), (kur ?? "").Trim(), (fon ?? "").Trim(), (fin ?? "").Trim(), (eko ?? "").Trim(), (peb ?? "").Trim());
                return DosyaIslem.DosyaGonder(dosya, "TertipListe_" + DosyaIslem.DosyaAdUretSade(), true, "xlsx");
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
    }
}
