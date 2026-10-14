using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Istemci.ButceKod;
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
using TNS.UZY;

namespace Istemci.ButceHazirlik
{
    public class ButceFisiEskiKaydetIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public string Kur { get; set; }
        public string Fon { get; set; }
        public string Fin { get; set; }
        public string Eko { get; set; }
        public string Gerekce { get; set; }
        public string Sebep1 { get; set; }
        public string Sebep2 { get; set; }
        public double? Odenek1 { get; set; }
        public double? Odenek2 { get; set; }
        public double? Odenek3 { get; set; }
    }

    public class ButceFisiEskiYazdirIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        /// <summary>Arama kriterleri (kod ya da "kod- ad").</summary>
        public string AraKur { get; set; }
        public string AraFon { get; set; }
        public string AraFin { get; set; }
        public string AraEko { get; set; }
        /// <summary>Ekranda gösterilen tertibin kurumsal kodu (dosya adı için).</summary>
        public string Kur { get; set; }
    }

    /// <summary>
    /// e-Bütçe biçimli bütçe gider fişi sayfasının (eski ButceHazirlikv2/ButceFisiGirisEski.aspx?odenekTur=1) uçları:
    ///   GET  /api/butcefisieski/tanim   yıllar, aşamalar, kullanıcının aşaması
    ///   GET  /api/butcefisieski/kodlar  kod ağacının bir düzeyi
    ///   GET  /api/butcefisieski/bul     arama kriterlerine uyan ilk / önceki / sonraki tertip ve bilgileri (eski OncekiSonrakiTertipAra + FisListele)
    ///   POST /api/butcefisieski/kaydet  gerekçe, farklılaşma sebepleri ve 3 yıllık teklif (eski btnKaydet_Click)
    ///   POST /api/butcefisieski/yazdir  kriterlere uyan bütün tertiplerin fişleri Word (.doc) dosyasına (eski btnTertipYazdir_Click)
    /// Yetki menüdeki adrese göre denetlenir.
    /// </summary>
    public static class ButceFisiEskiUclari
    {
        private const string SayfaAdres = "../ButceHazirlikv2/ButceFisiGirisEski.aspx?odenekTur=1";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/butcefisieski/tanim", Tanim);
            app.MapGet("/api/butcefisieski/kodlar", Kodlar);
            app.MapGet("/api/butcefisieski/bul", Bul);
            app.MapPost("/api/butcefisieski/kaydet", Kaydet);
            app.MapPost("/api/butcefisieski/yazdir", Yazdir);
        }

        /// <summary>Sistem yöneticisi ya da menüdeki "Bütçe Gider Fişi (e Bütçe)" seçeneğinin yetkilisi.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (Istemci.Organizasyon.GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                MenuOkuyucu menu = new MenuOkuyucu(kullanan);
                return menu.SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdres);
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

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string Senaryo(string senaryo) { senaryo = Temiz(senaryo); return senaryo == "" ? "00.00" : senaryo; }

        private static string Eleman(string[] dizi, int no) { return dizi != null && no < dizi.Length ? dizi[no] ?? "" : ""; }

        /// <summary>Açılış bilgileri (eski Page_Load): yıllar 2012..hazırlık yılı, aşamalar, kullanıcının aşaması.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int bhYil = KodTanimUclari.YilBul(null);
                List<int> yillar = new List<int>();
                for (int i = 2012; i <= bhYil; i++) yillar.Add(i);
                List<object> senaryolar = new List<object>();
                foreach (ButceSenaryo bs in TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null) ?? new ButceSenaryo[0])
                    senaryolar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });
                string senaryo = ButceRakamUclari.KullaniciSenaryoBul(kullanan);
                return Results.Json(new { yil = bhYil, yillar = yillar, senaryo = string.IsNullOrWhiteSpace(senaryo) ? "00.00" : senaryo, senaryolar = senaryolar });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Kod ağacının bir düzeyi (eski açılır ağaçlar: kurumsal 1, fonksiyonel 2, finansman 3, ekonomik 4).</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (Array.IndexOf(new[] { "1", "2", "3", "4" }, Temiz(tur)) < 0) return Hata("Kod türü hatalı!");
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, tur, ust);
        }

        /// <summary>
        /// İlk Tertip (yon 0), Önceki (-1), Sonraki (1): arama kriterlerine uyan tertip bulunur (eski OncekiSonrakiTertipAra), bilgileri,
        /// geçmiş yıl rakamları, teklif ve gerekçe okunur (eski FisListele).
        /// </summary>
        private static IResult Bul(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, int yon,
            string kur, string fon, string fin, string eko, string araKur, string araFon, string araFin, string araEko)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl seçiniz.");
            if (yon < -1 || yon > 1) yon = 0;
            senaryo = Senaryo(senaryo);

            try
            {
                Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                t.kodlar[0] = Temiz(kur);
                t.kodlar[1] = Temiz(fon);
                t.kodlar[2] = Temiz(fin);
                t.kodlar[3] = Temiz(eko);

                Tertip tAra = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                tAra.kodlar[0] = TNS.BHMv2.Arac.TiredenKodBul(Temiz(araKur)).Trim();
                tAra.kodlar[1] = TNS.BHMv2.Arac.TiredenKodBul(Temiz(araFon)).Trim();
                tAra.kodlar[2] = TNS.BHMv2.Arac.TiredenKodBul(Temiz(araFin)).Trim();
                tAra.kodlar[3] = TNS.BHMv2.Arac.TiredenKodBul(Temiz(araEko)).Trim();

                IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
                t = servisBHMv2.OncekiSonrakiTertip(kullanan, yil, t, tAra, yon);
                if (t == null) return Hata("Verilen kriterlere göre uygun bir tertip bulunamadı.");
                return Results.Json(FisBilgi(kullanan, servisBHMv2, yil, senaryo, t));
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Tertip okunamadı."));
            }
        }

        /// <summary>Eski FisListele: tertibin adları, geçmiş yıl ödenek / harcamaları, teklif ve gerekçe.</summary>
        private static object FisBilgi(Kullanici kullanan, IBHMv2Servis servisBHMv2, int yil, string senaryoKod, Tertip t)
        {
            IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
            string kur = Temiz(t.kodlar[0]), fon = Temiz(t.kodlar[1]), fin = Temiz(t.kodlar[2]), eko = Temiz(t.kodlar[3]);
            string kodlar = kur + "-" + fon + "-" + fin + "-" + eko;
            Func<string, string, double> uzay = (ad, kosul) => servisUZY.UzayDegeriDbl(kullanan, ad, kosul, false);

            double kbo2 = uzay("ABSODENEK", (yil - 2) + "-06.00-0-" + kodlar);
            double harcama2 = uzay("ABSHARCAMA", (yil - 2) + "-12-" + kodlar);
            // Eski sayfada yıl ile "06.00" arasındaki tire eksikti (bir önceki yılın KBÖ'sü hiç bulunamıyordu); burada düzeltildi.
            double kbo1 = uzay("ABSODENEK", (yil - 1) + "-06.00-0-" + kodlar);
            double harcama1 = uzay("ABSHARCAMA", (yil - 1) + "-6-" + kodlar);
            double ysTahmin = uzay("ABSHARCAMATAHMIN", (yil - 1) + "-00.00-" + kodlar);

            string[] bfv = servisBHMv2.ButceFisiGerekceYaziGetir(kullanan, t);
            double?[] odenek = new double?[3];
            if (bfv != null)
            {
                for (int i = 0; i < 3; i++)
                    odenek[i] = uzay("ABSODENEK", yil + "-" + senaryoKod + "-" + i + "-" + kodlar);
            }

            return new
            {
                tertip = t.TertipStrOl(),
                kur = kur, fon = fon, fin = fin, eko = eko,
                birimAd = Eleman(t.adlar, 0),
                ekoAd = Eleman(t.adlar, 3),
                kbo2 = kbo2, harcama2 = harcama2, kbo1 = kbo1, harcama1 = harcama1, ysTahmin = ysTahmin,
                okundu = bfv != null,
                gerekce = Eleman(bfv, 0), sebep1 = Eleman(bfv, 1), sebep2 = Eleman(bfv, 2),
                odenek = odenek
            };
        }

        /// <summary>
        /// Kaydet (eski btnKaydet_Click): gerekçe ve farklılaşma sebepleri, ardından 3 yıllık teklif saklanır. 01 ve 02 ekonomik kodları
        /// için bütçe fişi düzenlenmez (eskisi gibi yalnızca bilgi verilir).
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, ButceFisiEskiKaydetIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new ButceFisiEskiKaydetIstegi();
            if (istek.Yil < 2000) return Hata("Yıl seçiniz.");
            string kur = Temiz(istek.Kur), fon = Temiz(istek.Fon), fin = Temiz(istek.Fin), eko = Temiz(istek.Eko);
            if (kur == "" || eko == "") return Hata("Önce İlk Tertip, Önceki ya da Sonraki ile bir tertip getiriniz.");

            if (eko.StartsWith("01") || eko.StartsWith("02"))
                return Results.Json(new { basarili = true, bilgi = "Bilgiler başarıyla saklandı. 01 ve 02 ekonomik kodları için bütçe fişi düzenlenmeyecektir. Bütçe Tekliflerinizi Ödenek Düzenleme ekranından girebilirsiniz." });

            try
            {
                IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
                TertipOdenek[] tertipListesi = new TertipOdenek[1];
                tertipListesi[0] = new TertipOdenek(3);
                tertipListesi[0].tertip = new Tertip(istek.Yil, ENUMTabloKod.TABOdenek);
                tertipListesi[0].tertip.kodlar[0] = kur.Replace(".", "");
                tertipListesi[0].tertip.kodlar[1] = fon.Replace(".", "");
                tertipListesi[0].tertip.kodlar[2] = fin.Replace(".", "");
                tertipListesi[0].tertip.kodlar[3] = eko.Replace(".", "");
                string[] metinler = new string[] { istek.Gerekce ?? "", istek.Sebep1 ?? "", istek.Sebep2 ?? "" };

                Sonuc sonuc = servisBHMv2.ButceFisiGerekceKaydet(kullanan, tertipListesi[0].tertip, metinler);
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Gerekçe saklanamadı."));

                tertipListesi[0].odenek[0] = istek.Odenek1 ?? 0;
                tertipListesi[0].odenek[1] = istek.Odenek2 ?? 0;
                tertipListesi[0].odenek[2] = istek.Odenek3 ?? 0;
                sonuc = servisBHMv2.ButceOdenekKaydet(kullanan, Senaryo(istek.Senaryo), tertipListesi);
                if (!sonuc.islemSonuc)
                    return Hata("Gerekçe kaydedildi fakat rakamlar kaydedilirken hata oluştu: " + HataMetni(sonuc.hataStr, ""));
                return Results.Json(new { basarili = true, bilgi = "Bilgiler başarıyla saklandı." });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        private static bool TertipKiyasla(Tertip t1, Tertip t2)
        {
            if (t1 == null || t2 == null) return t1 == t2;
            for (int i = 0; i < t1.kodlar.Length; i++)
                if (t1.kodlar[i] != t2.kodlar[i]) return false;
            return true;
        }

        private static void TertipKopyala(Tertip kaynakTertip, Tertip hedefTertip)
        {
            for (int i = 0; i < kaynakTertip.kodlar.Length; i++)
            {
                hedefTertip.kodlar[i] = kaynakTertip.kodlar[i];
                hedefTertip.adlar[i] = kaynakTertip.adlar[i];
            }
        }

        /// <summary>
        /// Yazdır (eski btnTertipYazdir_Click): arama kriterlerine uyan bütün tertiplerin fişleri (FORM:13, her biri yeni sayfada) eski koddaki
        /// Word (.doc) HTML'i olarak yazılır.
        /// </summary>
        private static async Task<IResult> Yazdir(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, ButceFisiEskiYazdirIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new ButceFisiEskiYazdirIstegi();
            int yil = istek.Yil;
            if (yil < 2000) return Hata("Yıl seçiniz.");
            string senaryoKod = Senaryo(istek.Senaryo);

            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABOdenek);
            kistasTertip.kodlar[0] = TNS.BHMv2.Arac.TiredenKodBul(Temiz(istek.AraKur)).Trim();
            kistasTertip.kodlar[1] = TNS.BHMv2.Arac.TiredenKodBul(Temiz(istek.AraFon)).Trim();
            kistasTertip.kodlar[2] = TNS.BHMv2.Arac.TiredenKodBul(Temiz(istek.AraFin)).Trim();
            kistasTertip.kodlar[3] = TNS.BHMv2.Arac.TiredenKodBul(Temiz(istek.AraEko)).Trim();

            if (kistasTertip.kodlar[0] == "" && kistasTertip.kodlar[1] == "")
                return Hata("Toplu bütçe fişi yazdırma işleminda Kurumsal ve Fonksiyonel kodlarını tam olarak yazmanız gerekmektedir.");

            try
            {
                IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
                IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                Tertip aktifTertip = servisBHMv2.OncekiSonrakiTertip(kullanan, yil, null, kistasTertip, 0);
                if (aktifTertip == null) return Hata("Tertip bulunamadı");
                if (TertipKiyasla(kistasTertip, aktifTertip)) return Hata("Bu tertiple ilgili herhangi bir veri bulunamadı!");

                StringWriter sayfa = new StringWriter();
                bool ilkSayfa = true;
                // Eski döngü sonraki tertip bir öncekiyle aynı gelince biterdi; burada sonraki bulunamazsa da biter (sonsuz döngüye karşı üst sınır var).
                for (int say = 0; aktifTertip != null && say < 10000; say++)
                {
                    string[] bfv = servisBHMv2.ButceFisiGerekceYaziGetir(kullanan, aktifTertip);
                    if (bfv == null) return Hata("Tertip bulunamadı");

                    string tertipStr = aktifTertip.TertipStrOl();
                    string bosluk = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    string odenek1 = bosluk + servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", yil + "-" + senaryoKod + "-0-" + tertipStr, false).ToString("#,###");
                    string odenek2 = bosluk + servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", yil + "-" + senaryoKod + "-1-" + tertipStr, false).ToString("#,###");
                    string odenek3 = bosluk + servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", yil + "-" + senaryoKod + "-2-" + tertipStr, false).ToString("#,###");

                    ButceFisiHTML(sayfa, servisUZY, kullanan, yil, ref ilkSayfa, aktifTertip, bfv[0], Eleman(bfv, 1), Eleman(bfv, 2), odenek1, odenek2, odenek3, aktifTertip.Kirilimlar(0));

                    Tertip saklaTertip = new Tertip(aktifTertip.yil, aktifTertip.tabloKod);
                    TertipKopyala(aktifTertip, saklaTertip);
                    aktifTertip = servisBHMv2.OncekiSonrakiTertip(kullanan, yil, aktifTertip, kistasTertip, 1);
                    if (TertipKiyasla(saklaTertip, aktifTertip)) break;
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(ms, new UTF8Encoding(false), 4096, true))
                        ButceFisiOrtak.ButceFisiDosyayaYaz(sayfa.ToString(), sw);
                    return Results.File(ms.ToArray(), "application/msword", "ButceFisi" + Temiz(istek.Kur) + ".doc");
                }
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Bütçe fişi hazırlanamadı."));
            }
        }

        /// <summary>Eski ButceFisiHTML (FORM:13 gider bütçe fişi); olduğu gibi alındı, Response.Write yerine verilen yazıcıya yazar.</summary>
        internal static void ButceFisiHTML(TextWriter yaz, IUZYServis servisUZY, Kullanici kullanan, int yil, ref bool ilkSayfa, Tertip tertip, string gerekce, string baslangicFarkSebebi, string sonrakiYillarFarkSebebi, string odenek1, string odenek2, string odenek3, string kurYapi)
        {
            string k1 = "";
            string k2 = "";
            string k3 = "";
            string k4 = "";
            string f1 = "";
            string f2 = "";
            string f3 = "";
            string f4 = "";
            string fi1 = tertip.kodlar[2].ToString();
            string e1 = "";
            string e2 = "";
            string e3 = "";
            string e4 = "";

            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil.ToString() + "-" + TNS.BKD.Arac.KurumsaldanKuruma(tertip.kodlar[0], kurYapi), true);
            string ekoAd = tertip.adlar[3].ToString();

            string[] kurDizi = tertip.kodlar[0].Split('.');
            k1 = kurDizi[0];
            k2 = kurDizi[1];
            k3 = kurDizi[2];
            k4 = kurDizi[3];

            string[] fonDizi = tertip.kodlar[1].Split('.');
            f1 = fonDizi[0];
            f2 = fonDizi[1];
            f3 = fonDizi[2];
            f4 = fonDizi[3];

            string[] ekoDizi = tertip.kodlar[3].Split('.');
            e1 = ekoDizi[0];
            e2 = ekoDizi[1];
            e3 = ekoDizi[2];
            e4 = ekoDizi[3];

            if (!ilkSayfa)
                yaz.Write("<br clear=all style='page-break-before:always'>");
            else
                ilkSayfa = false;

            yaz.Write("<p  class=MsoNormal align=right style='text-align:right'><b>FORM:13</b></p>");
            yaz.Write("<p class=MsoNormal align='center'><b>GİDER BÜTÇE FİŞİ</b></p>");
            yaz.Write("<p class=MsoNormal align='left'> <b>KURUM ADI : </b>&nbsp;" + kurumAd + "</p>");
            yaz.Write("<p  class=MsoNormal align=right style='text-align:right'>(TL)</p>");
            yaz.Write("<table border='1' width='100%' id='table1' style='border-collapse: collapse' bordercolor='#000000'>");
            yaz.Write("<tr>");
            yaz.Write("<td style='border-style: solid; border-width: 1px' colspan='4' align='center'>");
            yaz.Write("<p class=MsoNormal  style='margin-top: 0; margin-bottom: 0'><b><font size='2'>KURUMSAL</font></b></p>");
            yaz.Write("<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b>");
            yaz.Write("<font size='2'>SINIFLANDIRMA</font></b></td>");
            yaz.Write("<td style='border-style: solid; border-width: 1px' colspan='4' align='center'>");
            yaz.Write("<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>FONKSİYONEL</font></b></p>");
            yaz.Write("<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b>");
            yaz.Write("<font size='2'>SINIFLANDIRMA</font></b></td>");
            yaz.Write("<td style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>FİN.</font></b></p>");
            yaz.Write("<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>TİPİ</font></b></td>");
            yaz.Write("<td style='border-style: solid; border-width: 1px' colspan='4' align='center'>");
            yaz.Write("<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>EKONOMİK</font></b></p>");
            yaz.Write("<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b>");
            yaz.Write("<font size='2'>SINIFLANDIRMA</font></b></td>");
            yaz.Write("<td  width='55%' style='border-style: solid; border-width: 1px' align='center' rowspan='2'>");
            yaz.Write("<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>AÇIKLAMA</font></b></td>");
            yaz.Write("<td style='border-style: solid; border-width: 1px' colspan='3' align='center'>");
            yaz.Write("<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>BÜTÇE TEKLİFİ</font></b></td>");
            yaz.Write("</tr>");
            yaz.Write("<tr>");
            yaz.Write("<td width='3.1%' style='border-style: solid; border-width: 1px' align='center'><b>");
            yaz.Write("I</b></td>");
            yaz.Write("<td  width='3.1%' style='border-style: solid; border-width: 1px' align='center'><b>");
            yaz.Write("II</b></td>");
            yaz.Write("<td  width='3.1%' style='border-style: solid; border-width: 1px' align='center'><b>");
            yaz.Write("III</b></td>");
            yaz.Write("<td   width='3.1%' style='border-style: solid; border-width: 1px' align='center'><b>");
            yaz.Write("IV</b></td>");
            yaz.Write("<td  width='3.1%' style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<b>I</b></td>");
            yaz.Write("<td  width='3.1%' style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<b>II</b></td>");
            yaz.Write("<td  width='3.1%' style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<b>III</b></td>");
            yaz.Write("<td  width='3.1%' style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<b>IV</b></td>");
            yaz.Write("<td   width='4.5%' style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<b>I</b></td>");
            yaz.Write("<td  width='3.1%' style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<b>I</b></td>");
            yaz.Write("<td  width='3.1%' style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<b>II</b></td>");
            yaz.Write("<td  width='3.1%' style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<b>III</b></td>");
            yaz.Write("<td  width='3.1%' style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<b>IV</b></td>");
            yaz.Write("<td  width='15%' style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<b>" + yil.ToString() + "</b></td>");
            yaz.Write("<td  width='15%' style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<b>" + (yil + 1).ToString() + "</b></td>");
            yaz.Write("<td  width='15%' style='border-style: solid; border-width: 1px' align='center'>");
            yaz.Write("<b>" + (yil + 2).ToString() + "</b></td>");
            yaz.Write("</tr>");
            yaz.Write("<tr>");


            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + k1 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + k2 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + k3 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + k4 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + f1 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + f2 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + f3 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + f4 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + fi1 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + e1 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + e2 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + e3 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='center'>" + e4 + "</td>");
            yaz.Write("<td  style='border-style: solid; border-width: 1px' align='left'>");
            yaz.Write(ekoAd + "</td>");
            yaz.Write("<td style='border-style: solid; border-width: 1px' align='right'>");
            yaz.Write(odenek1 + "</td>");
            yaz.Write("<td style='border-style: solid; border-width: 1px' align='right'>");
            yaz.Write(odenek2 + "</td>");
            yaz.Write("<td style='border-style: solid; border-width: 1px' align='right'>");
            yaz.Write(odenek3 + "</td>");
            yaz.Write("</tr>");
            yaz.Write("</table>");

            yaz.Write("<p style='margin-top: 0; margin-bottom: 0'>&nbsp;</p>");

            yaz.Write("<table border='2' cellSpacing=2 cellPadding=2 width='100%' bordercolor='#000000' id='table2' style='border-collapse: collapse'>");
            yaz.Write("<tr>");
            yaz.Write("<td class=MsoNormal height=400 valign='top' style='border-style: solid; border-width: 1px' colspan='17'><b><u>");
            yaz.Write("</u></b><p>");
            yaz.Write("<br>" + gerekce + "</td>");
            yaz.Write("</tr>");
            yaz.Write("</table>");
        }
    }
}
