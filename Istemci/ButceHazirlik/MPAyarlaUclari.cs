using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
using TNS.BUM;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    /// <summary>Düşülecek / eklenecek tertibin kodları.</summary>
    public class MPAyarlaTertipIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public string Fon { get; set; }
        public string Fin { get; set; }
        public string Eko { get; set; }
        public string Peb { get; set; }
        /// <summary>Eski kurumsal yapı (harcama birimi) kodu; boşsa 90000000000 (dağıtılmayan).</summary>
        public string KurYapi { get; set; }
        /// <summary>1: düşülecek tertip (aylık masraf planı da döner), 2: eklenecek tertip.</summary>
        public int Tur { get; set; }
    }

    /// <summary>Bir ayın düşülecek tutarı.</summary>
    public class MPAyarlaAy
    {
        public int ay { get; set; }
        public double dusulen { get; set; }
    }

    /// <summary>İşlem Yap: 1 Masraf Planı (MP + AHP + tavan), 2 AHP (AHP + tavan), 3 Tavan.</summary>
    public class MPAyarlaIslemIstegi
    {
        public int Yil { get; set; }
        public int Islem { get; set; }
        public string Dusulecek { get; set; }
        public string Eklenecek { get; set; }
        public List<MPAyarlaAy> Aylar { get; set; }
    }

    /// <summary>
    /// Masraf Planı Düzenleme (eski ButceHazirlikv2/MasrafPlaniYatirimAyarla.aspx; Masraf Planı ekranındaki "MP Düzelt"): aynı
    /// kurumsal / fonksiyonel / finans / ekonomik kodlu iki tertipten (faaliyetleri farklı) birinin aylık tutarları düşülüp
    /// diğerine eklenir. Seçilen işleme göre masraf planı (AHP aşaması), AHP (3. aşama), KBÖ ve tavanlar SQL komutlarıyla
    /// doğrudan veri tabanında değiştirilir; onay durumuna bakılmaz. Yalnızca sistem yöneticisi kullanır (eski sayfada link
    /// yalnızca ona görünüyordu); yetki üst sayfa Masraf Planı'ndan gelir.
    ///   GET  /api/mpayarla/kodlar, /api/mpayarla/kodad   (kod ağacı, kod adı)
    ///   POST /api/mpayarla/tertip                        (eski btnListele_Click: tertip, KBÖ / tavan / dağıtılan, aylık MP)
    ///   POST /api/mpayarla/islem                         (eski btnKaydet_Click: komutlar üretilir, hata yoksa çalıştırılır)
    /// </summary>
    public static class MPAyarlaUclari
    {
        public const string Adres = "../ButceHazirlikv2/MasrafPlaniYatirimAyarla.aspx";
        private const string DagitilmayanKurYapi = "90000000000";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/mpayarla/kodlar", Kodlar);
            app.MapGet("/api/mpayarla/kodad", KodAdi);
            app.MapPost("/api/mpayarla/tertip", TertipBul);
            app.MapPost("/api/mpayarla/islem", IslemYap);
        }

        // ------------------------------------------------------------------ yetki

        /// <summary>Sistem yöneticisi ve üst sayfa (Masraf Planı) yetkisi.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null || !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return false;
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Adres, MasrafPlaniUclari.Adres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kod, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return MPTavanUclari.KodAgaci(kullanan, yil, kod, ust);
        }

        private static IResult KodAdi(HttpContext baglam, IWebHostEnvironment ortam, int yil, int tur, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return MPTavanUclari.KodAdiBul(yil, tur, kod);
        }

        // ------------------------------------------------------------------ tertip bilgisi

        /// <summary>Tertip yalnızca rakam ve nokta olabilir (kodlar SQL komutlarına yazılır).</summary>
        private static readonly Regex KodDeseni = new Regex(@"^[0-9.]+$");

        /// <summary>Tertip dizisinden (kur-fon-fin-eko-peb-kurYapi) tam ve adları bulunmuş tertip; değilse hata metni.</summary>
        private static Tertip TertipOl(int yil, string tertipStr, TNS.UZY.IUZYServis servisUzy, out string hata)
        {
            hata = null;
            Tertip t = new Tertip(yil, ENUMTabloKod.TABTasraOdenek);
            t.TertipAta(Temiz(tertipStr));
            if (!t.TamMi() || t.kodlar.Length < 6)
            {
                hata = "Lütfen tertibi tam giriniz.";
                return null;
            }
            for (int i = 0; i < 6; i++)
            {
                if (!KodDeseni.IsMatch(t.kodlar[i] ?? ""))
                {
                    hata = "Tertip kodları yalnızca rakam ve nokta içerebilir.";
                    return null;
                }
            }
            t.AdlariBul(servisUzy);
            if (!t.IsimlerTamMi(out string kodAd, out string kod))
            {
                hata = kodAd + " kod " + kod + " bulunamadı!";
                return null;
            }
            return t;
        }

        /// <summary>
        /// Düşülecek / Eklenecek Tertip (eski btnListele_Click): seçilen kodlar ve eski kurumsal yapıyla tertip oluşturulur;
        /// KBÖ, tavan ve dağıtılan bilgisi, düşülecek tertipte AHP aşamasındaki aylık masraf planı döner.
        /// </summary>
        private static async Task<IResult> TertipBul(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPAyarlaTertipIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MPAyarlaTertipIstegi();
            if (istek.Yil <= 0) return Hata("Yıl giriniz.");
            try
            {
                string kurYapi = Temiz(istek.KurYapi);
                if (string.IsNullOrWhiteSpace(kurYapi)) kurYapi = DagitilmayanKurYapi;
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                TNS.UZY.IUZYServis servisUzy = TNS.UZY.Arac.Tanimla();

                Tertip t = new Tertip(istek.Yil, ENUMTabloKod.TABTasraOdenek);
                t.TertipAta(Temiz(istek.Kur), Temiz(istek.Fon), Temiz(istek.Fin), Temiz(istek.Eko), Temiz(istek.Peb), kurYapi);
                if (!t.TamMi()) return Hata("Lütfen tertibi tam giriniz.");
                t = TertipOl(istek.Yil, t.TertipStrOl(), servisUzy, out string hata);
                if (t == null) return Hata(hata);

                string tertipStr = t.TertipStrOl();
                double[] aylar = BilgiBul(kullanan, servis, servisUzy, t, out double kbo, out double tavan, out double dagitilan);
                List<object> aySatirlari = new List<object>();
                if (istek.Tur == 1)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        double odenek = aylar != null && i < aylar.Length ? aylar[i] : 0;
                        aySatirlari.Add(new { ay = i + 1, ayAd = OrtakFonksiyonlar.AyAdiGetir(i + 1), odenek = odenek, dusulen = 0d });
                    }
                }
                CultureInfo tr = CultureInfo.GetCultureInfo("tr-TR");
                return Results.Json(new
                {
                    tertip = tertipStr,
                    bilgi = string.Format(tr, "KBÖ={0:N2} - Tavan={1:N2} - Dağıtılan={2:N2}", kbo, tavan, dagitilan),
                    aylar = aySatirlari
                });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>
        /// Eski BilgiBul: KBÖ (ABSODENEK), tavan / dağıtılan (MPTAVAN) ve AHP aşamasındaki aylık masraf planı. Tertibin ekonomik
        /// kodu eko2 uzunluğuna kısaltılır (masraf planı ve tavan bu uzunlukta tutulur); bu yüzden kopya tertiple çağrılır.
        /// </summary>
        private static double[] BilgiBul(Kullanici kullanan, IBHMv2Servis servis, TNS.UZY.IUZYServis servisUzy, Tertip t,
            out double kbo, out double tavan, out double dagitilan)
        {
            string kurYapi = ParcaAl(t.kodlar[5], 3);
            string yil = t.yil.ToString(CultureInfo.InvariantCulture);
            kbo = servisUzy.UzayDegeriDbl(kullanan, "ABSODENEK", TNS.UZY.Arac.UzayKosulOl(yil, "06.00", "0", t.kodlar[0], t.kodlar[1], t.kodlar[2], t.kodlar[3], t.kodlar[4]), false);
            int eko2Uz = TNS.BHMv2.Arac.Eko2UzBul(t.yil);
            t.kodlar[3] = ParcaAl(t.kodlar[3], eko2Uz);
            tavan = servisUzy.UzayDegeriDbl(kullanan, "MPTAVAN", TNS.UZY.Arac.UzayKosulOl(yil, "0", t.kodlar[0], t.kodlar[1], t.kodlar[2], t.kodlar[3], t.kodlar[4], kurYapi), false);
            dagitilan = servisUzy.UzayDegeriDbl(kullanan, "MPTAVAN", TNS.UZY.Arac.UzayKosulOl(yil, "1", t.kodlar[0], t.kodlar[1], t.kodlar[2], t.kodlar[3], t.kodlar[4], t.kodlar[5]), false);
            double[] bmpg = servis.MPMasrafPlaniOku(kullanan, ENUMMasrafPlanAsama.AHP, t, 0, 0, false);
            return bmpg ?? new double[12];
        }

        private static string ParcaAl(string veri, int uzunluk)
        {
            string s = Noktasiz(veri);
            return s.Length > uzunluk ? s.Substring(0, uzunluk) : s;
        }

        private static string Noktasiz(string deger) { return (deger ?? "").Replace(".", ""); }

        // ------------------------------------------------------------------ işlem yap

        /// <summary>
        /// İşlem Yap (eski btnKaydet_Click): iki tertip denetlenir, seçilen işleme göre silme / ekleme komutları üretilir; hata
        /// yoksa komutlar BaglantiSatiriBHMv2 bağlantısında çalıştırılır. Komutlar her durumda ekranda gösterilmek üzere döner.
        /// </summary>
        private static async Task<IResult> IslemYap(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPAyarlaIslemIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MPAyarlaIslemIstegi();
            int yil = istek.Yil;
            if (yil <= 0) return Hata("Yıl giriniz.");
            if (istek.Islem < 1 || istek.Islem > 3) return Hata("İşlem seçiniz.");
            if (string.IsNullOrWhiteSpace(istek.Dusulecek) || string.IsNullOrWhiteSpace(istek.Eklenecek))
                return Hata("Önce Düşülecek Tertip ve Eklenecek Tertip tuşlarıyla iki tertibi bulunuz.");
            if (istek.Aylar == null || istek.Aylar.Count != 12) return Hata("Aylık düşülecek tutarlar eksik.");

            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                IBUMServis bumServis = TNS.BUM.Arac.Tanimla();
                TNS.UZY.IUZYServis servisUzy = TNS.UZY.Arac.Tanimla();
                Komutlar k = new Komutlar(istek.Islem);

                Tertip t1 = TertipOl(yil, istek.Dusulecek, servisUzy, out string hata);
                if (t1 == null) return Hata("Düşülecek tertip: " + hata);
                Tertip t2 = TertipOl(yil, istek.Eklenecek, servisUzy, out hata);
                if (t2 == null) return Hata("Eklenecek tertip: " + hata);
                Tertip t1e2 = new Tertip(yil, ENUMTabloKod.TABTasraOdenek);
                t1e2.TertipAta(t1.TertipStrOl());
                Tertip t2e2 = new Tertip(yil, ENUMTabloKod.TABTasraOdenek);
                t2e2.TertipAta(t2.TertipStrOl());

                double[] mp1 = BilgiBul(kullanan, servis, servisUzy, t1e2, out double kbo1, out double tavan1, out double dagitilan1);
                double[] mp2 = BilgiBul(kullanan, servis, servisUzy, t2e2, out double kbo2, out double tavan2, out double dagitilan2);

                Sonuc s = new Sonuc();
                if (!(t1.kodlar[0] == t2.kodlar[0] && t1.kodlar[1] == t2.kodlar[1] && t1.kodlar[2] == t2.kodlar[2] && t1.kodlar[3] == t2.kodlar[3]))
                    s.DegerAta("İki tertibin; kurumsal, fonksiyonel, finans ve ekonomik kodları eşit olmalıdır!");
                if (t1.kodlar[4] == t2.kodlar[4])
                    s.DegerAta("İki tertibin; faaliyet kodları farklı olmalıdır!");
                bool dagitma1 = t1.kodlar[5].StartsWith("9");
                bool dagitma2 = t2.kodlar[5].StartsWith("9");
                if (dagitma1 != dagitma2)
                    s.DegerAta("Kurumsal kod; ya ikisinde de birim olmalıdır ya da dağıtılmayanda olmalıdır!");

                MPAyarlaAy[] yeni = new MPAyarlaAy[12];
                foreach (MPAyarlaAy a in istek.Aylar)
                    if (a != null && a.ay >= 1 && a.ay <= 12) yeni[a.ay - 1] = a;
                for (int i = 0; i < 12; i++)
                    yeni[i] ??= new MPAyarlaAy { ay = i + 1, dusulen = 0 };

                if (k.MPYap)
                {
                    k.Ekle(MPSil(t1e2));
                    k.Ekle(MPSil(t2e2));
                }
                double toplam = 0;
                for (int i = 0; i < 12; i++)
                {
                    MPAyarlaAy item = yeni[i];
                    toplam += item.dusulen;
                    if (k.MPYap)
                    {
                        if (item.dusulen < 0)
                            s.DegerAta(item.ay + ". ay verisi sıfırdan küçük olamaz!");
                        if (item.dusulen > mp1[i])
                            s.DegerAta(item.ay + ". ay verisi planlanandan büyük olamaz!");
                        k.Ekle(MPEkle(t1e2, i + 1, mp1[i] - item.dusulen));
                        k.Ekle(MPEkle(t2e2, i + 1, mp2[i] + item.dusulen));
                    }
                }
                if (toplam == 0)
                    s.DegerAta("Düşülecek alanına veri giriniz!");

                if (k.AHPYap)
                {
                    Tertip t1AHP = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, t1.kodlar[0], t1.kodlar[1], t1.kodlar[2], t1e2.kodlar[3], t1.kodlar[4]);
                    Tertip t2AHP = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, t2.kodlar[0], t2.kodlar[1], t2.kodlar[2], t2e2.kodlar[3], t2.kodlar[4]);
                    const int ahpAsama = 3;
                    TertipOdenek degerler1 = AHPBul(bumServis.APListele(kullanan, ahpAsama, t1AHP), null);
                    if (degerler1 != null)
                    {
                        TertipOdenek degerler2 = AHPBul(bumServis.APListele(kullanan, ahpAsama, t2AHP), t2AHP);
                        AHPKontrolEt(k, s, degerler1, degerler2, yeni, ahpAsama);
                    }
                }

                if (k.TavanYap)
                {
                    k.Ekle(KBOSil(t1));
                    k.Ekle(KBOSil(t2));
                    k.Ekle(KBOEkle(t1, kbo1 - toplam));
                    k.Ekle(KBOEkle(t2, kbo2 + toplam));
                    if (!dagitma1)
                    {
                        k.Ekle(DetaySil(t1e2, 1));
                        k.Ekle(DetaySil(t2e2, 1));
                        k.Ekle(DetayEkle(t1e2, dagitilan1 - toplam, 1));
                        k.Ekle(DetayEkle(t2e2, dagitilan2 + toplam, 1));
                        k.Ekle(DetaySil(t1e2, 0));
                        k.Ekle(DetaySil(t2e2, 0));
                        k.Ekle(DetayEkle(t1e2, tavan1 - toplam, 0));
                        k.Ekle(DetayEkle(t2e2, tavan2 + toplam, 0));
                    }
                }

                if (!s.islemSonuc)
                    return Results.Json(new { hata = s.hataStr, komut = k.Metin });

                string sonuc = TNS.TMM.Arac.Tanimla().SQLCalistir(kullanan, "BaglantiSatiriBHMv2", k.Metin);
                return Results.Json(new { bilgi = "Oluşturulan komutlar çalıştırıldı, sonuçları kontrol ediniz: " + sonuc, komut = k.Metin });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>İşleme göre hangi tabloların değişeceği ve üretilen komutlar.</summary>
        private sealed class Komutlar
        {
            private readonly int islem;
            public string Metin { get; private set; } = "";

            public Komutlar(int islem) { this.islem = islem; }

            public bool TavanYap => islem == 1 || islem == 2 || islem == 3;
            public bool AHPYap => islem == 1 || islem == 2;
            public bool MPYap => islem == 1;

            public void Ekle(string komut)
            {
                if (string.IsNullOrWhiteSpace(komut)) return;
                if (Metin != "") Metin += "\r\n";
                Metin += komut;
            }
        }

        private static TertipOdenek AHPBul(TertipOdenek[] tertipOdenek, Tertip tertip)
        {
            if (tertipOdenek != null && tertipOdenek.Length == 1) return tertipOdenek[0];
            if (tertip == null) return null;
            TertipOdenek bos = new TertipOdenek(14);
            bos.tertip = tertip;
            return bos;
        }

        private static void AHPKontrolEt(Komutlar k, Sonuc s, TertipOdenek degerler1, TertipOdenek degerler2, MPAyarlaAy[] yeni, int ahpAsama)
        {
            CultureInfo tr = CultureInfo.GetCultureInfo("tr-TR");
            k.Ekle(AHPSil(degerler1.tertip, ahpAsama));
            k.Ekle(AHPSil(degerler2.tertip, ahpAsama));
            for (int i = 0; i < 12; i++)
            {
                MPAyarlaAy item = yeni[i];
                if (item.dusulen > degerler1.odenek[i + 2])
                    s.DegerAta(string.Format(tr, "{0}. ay {1:N2} verisi, AHP'nin {2}. aşamasında, planlanan AHP'den ({3:N2}) büyük olamaz!",
                        item.ay, item.dusulen, ahpAsama, degerler1.odenek[i + 2]));
                k.Ekle(AHPEkle(degerler1.tertip, ahpAsama, i + 1, degerler1.odenek[i + 2] - item.dusulen));
                k.Ekle(AHPEkle(degerler2.tertip, ahpAsama, i + 1, degerler2.odenek[i + 2] + item.dusulen));
            }
        }

        // ------------------------------------------------------------------ komutlar (eski sayfadaki SQL'ler; sayılar noktalı yazılır)

        private static string Sql(string kalip, params object[] degerler)
        {
            return string.Format(CultureInfo.InvariantCulture, kalip, degerler);
        }

        private const string DETAYEKLE = @"INSERT INTO MPTAVAN(YIL,DETAYMI,KURKOD,FONKOD,FINKOD,EKOKOD,PEBKOD, KYKOD, ODENEK) VALUES ({0},{1},'{2}','{3}','{4}','{5}','{6}','{7}',{8})";
        private const string DETAYSIL = @"DELETE FROM MPTAVAN where YIL={0} AND DETAYMI={1} AND KURKOD='{2}' AND FONKOD='{3}' AND FINKOD='{4}'  AND EKOKOD='{5}' AND PEBKOD='{6}' AND KYKOD='{7}'";

        private static string DetayEkle(Tertip t, double odenek, int detay)
        {
            if (odenek == 0) return "";
            return Sql(DETAYEKLE, t.yil, detay, Noktasiz(t.kodlar[0]), Noktasiz(t.kodlar[1]), Noktasiz(t.kodlar[2]), Noktasiz(t.kodlar[3]), Noktasiz(t.kodlar[4]), KYKodBul(t.kodlar[5], detay), odenek);
        }

        private static string DetaySil(Tertip t, int detay)
        {
            return Sql(DETAYSIL, t.yil, detay, Noktasiz(t.kodlar[0]), Noktasiz(t.kodlar[1]), Noktasiz(t.kodlar[2]), Noktasiz(t.kodlar[3]), Noktasiz(t.kodlar[4]), KYKodBul(t.kodlar[5], detay));
        }

        private static string KYKodBul(string kod, int detay)
        {
            kod = Noktasiz(kod);
            if (detay == 0) return kod.Length > 3 ? kod.Substring(0, 3) : kod;
            return kod;
        }

        private const string ODENEKEKLE = @"INSERT INTO BHMODENEK(YIL,SENARYOKOD,TEKLIFYIL,KURKOD,FONKOD,FINKOD ,EKOKOD ,PEBKOD ,ODENEK) VALUES ({0},'0600' ,0 ,'{1}' ,'{2}' ,'{3}' ,'{4}' ,'{5}' ,{6})";
        private const string ODENEKSIL = @"DELETE FROM BHMODENEK where YIL={0} AND SENARYOKOD='0600' AND TEKLIFYIL= 0 AND KURKOD='{1}' AND FONKOD='{2}' AND FINKOD='{3}'  AND EKOKOD='{4}' AND PEBKOD='{5}'";

        private static string KBOEkle(Tertip t, double odenek)
        {
            if (odenek == 0) return "";
            return Sql(ODENEKEKLE, t.yil, Noktasiz(t.kodlar[0]), Noktasiz(t.kodlar[1]), Noktasiz(t.kodlar[2]), Noktasiz(t.kodlar[3]), Noktasiz(t.kodlar[4]), odenek);
        }

        private static string KBOSil(Tertip t)
        {
            return Sql(ODENEKSIL, t.yil, Noktasiz(t.kodlar[0]), Noktasiz(t.kodlar[1]), Noktasiz(t.kodlar[2]), Noktasiz(t.kodlar[3]), Noktasiz(t.kodlar[4]));
        }

        private const string AHPEKLE = @"INSERT INTO ButceIslemleri.dbo.ISLAPGIDER(YIL, KURUMKOD, ISLEM, SIRANO, KURKOD, FONKOD, FINKOD, EKOKOD, AY,ODENEK,FORMDURUM, PEBKOD) VALUES ({0},'{1}','0101',{2},'{3}','{4}','{5}','{6}',{7},{8},{9},'{10}')";
        private const string AHPSIL = @"DELETE FROM ButceIslemleri.dbo.ISLAPGIDER WHERE YIL={0} AND KURKOD='{1}' AND FONKOD='{2}' AND FINKOD='{3}' AND EKOKOD='{4}' AND PEBKOD='{5}' AND ISLEM='0101' AND SIRANO={6}";

        private static string AHPEkle(Tertip t, int ahpAsama, int ay, double odenek)
        {
            if (odenek == 0) return "";
            string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            return Sql(AHPEKLE, t.yil, kurum, ahpAsama, Noktasiz(t.kodlar[0]), Noktasiz(t.kodlar[1]), Noktasiz(t.kodlar[2]), Noktasiz(t.kodlar[3]), ay, odenek, ahpAsama == 3 ? 5 : 7, Noktasiz(t.kodlar[4]));
        }

        private static string AHPSil(Tertip t, int ahpAsama)
        {
            return Sql(AHPSIL, t.yil, Noktasiz(t.kodlar[0]), Noktasiz(t.kodlar[1]), Noktasiz(t.kodlar[2]), Noktasiz(t.kodlar[3]), Noktasiz(t.kodlar[4]), ahpAsama);
        }

        private const string MPEKLE = @"INSERT INTO ButceIslemleri.dbo.MPGMASRAFPLANITERTIP(YIL, ASAMA, KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, KURUMSALYAPIKOD, AY, ODENEK, ODENEKGONDERME, DURUM, REVIZENO, GRUP, TERTIPSIRA) VALUES ({0},5,'{1}','{2}','{3}','{4}','{5}','{6}',{7},{8},0,5,0,0,0)";
        private const string MPSIL = @"DELETE FROM ButceIslemleri.dbo.MPGMASRAFPLANITERTIP WHERE YIL={0} AND ASAMA=5 AND KURKOD='{1}' AND FONKOD='{2}' AND FINKOD='{3}' AND EKOKOD='{4}' AND PEBKOD='{5}' AND KURUMSALYAPIKOD='{6}' AND REVIZENO=0";

        private static string MPEkle(Tertip t, int ay, double odenek)
        {
            if (odenek == 0) return "";
            return Sql(MPEKLE, t.yil, Noktasiz(t.kodlar[0]), Noktasiz(t.kodlar[1]), Noktasiz(t.kodlar[2]), Noktasiz(t.kodlar[3]), Noktasiz(t.kodlar[4]), Noktasiz(t.kodlar[5]), ay, odenek);
        }

        private static string MPSil(Tertip t)
        {
            return Sql(MPSIL, t.yil, Noktasiz(t.kodlar[0]), Noktasiz(t.kodlar[1]), Noktasiz(t.kodlar[2]), Noktasiz(t.kodlar[3]), Noktasiz(t.kodlar[4]), Noktasiz(t.kodlar[5]));
        }
    }
}
