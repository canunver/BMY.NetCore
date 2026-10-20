using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>Mahsup listesinin bir satırı (eski StoreListe alanları). Sayfa seçtiği satırları bu biçimde geri gönderir.</summary>
    public class KapatmaSatir
    {
        public int belgeYil { get; set; }
        public string belgeNo { get; set; }
        public string belgeTarih { get; set; }
        public int muhasebeKod { get; set; }
        public string birimKod { get; set; }
        public string muhasebeAd { get; set; }
        public string birimAd { get; set; }
        public double borc { get; set; }
        public double alacak { get; set; }
        public double tutar { get; set; }
        public double kTutar { get; set; }
        public string cekNo { get; set; }
        public string hesapKod { get; set; }
        public string tertip { get; set; }
        public string ilgiliAd { get; set; }
        public string ilgiliVNo { get; set; }
        public int kapatmaBelgeYil { get; set; }
        public string kapatmaBelgeNo { get; set; }
        public int yevmiyeNo { get; set; }
        public string yevmiyeTarih { get; set; }
        public string ilgiliIBAN { get; set; }
        public string ilgiliBanka { get; set; }
        public string paraBirimi { get; set; }
        public string siraNo { get; set; }
        public string altHesapKod { get; set; }
        public string istatistik1Ad { get; set; }
        public string istatistik2Ad { get; set; }
    }

    /// <summary>Belge oluşturma penceresindeki tablonun bir satırı (eski StoreKapatma alanları).</summary>
    public class KapanisSatir
    {
        public string hesapKod { get; set; }
        public string altHesapKod { get; set; }
        public string birim { get; set; }
        public string tertip { get; set; }
        public double borc { get; set; }
        public double alacak { get; set; }
        public int sunucudanGeldi { get; set; }
        public string belgeNo { get; set; }
        public string siraNo { get; set; }
        public string paraBirimi { get; set; }
    }

    /// <summary>Seçili liste satırlarıyla yapılan işlemler (belge hazırla, iptal, listeden çıkart, yazdır, banka listesi).</summary>
    public class KapatmaSeciliIstegi
    {
        /// <summary>Listeden çıkartmada kapanış kaydının yılı (eski ddlYilBelge).</summary>
        public int yil { get; set; }
        /// <summary>Liste kıstaslarındaki muhasebe ve birim (eski txtMuhasebe / txtBirim): seçilenlerin muhasebe birimi farklıysa
        /// belge penceresine bunlar yazılır.</summary>
        public string muhasebe { get; set; }
        public string birim { get; set; }
        public List<KapatmaSatir> satirlar { get; set; }
    }

    /// <summary>Kapatma belgesi oluşturma (eski btnBelgeOlustur_Click).</summary>
    public class KapatmaBelgeIstegi
    {
        public int yil { get; set; }
        public string tarih { get; set; }
        public string muhasebe { get; set; }
        public string birim { get; set; }
        public int firmaNo { get; set; }
        public string aciklama { get; set; }
        public bool yevmiye { get; set; }
        public List<KapanisSatir> kapanisSatirlari { get; set; }
        public List<KapatmaSatir> seciliBelgeler { get; set; }
    }

    /// <summary>
    /// Mahsup İşlemleri (eski ButceMuhasebe/Kapatma.aspx; menüde üç seçenek: hesap kodsuz, ?hesap=323 ve ?hesap=103) uçları.
    /// Eski sayfanın arka plan kodunun (code-behind) işleri burada yapılır, iş kuralları servisleri değişmeden çağrılır.
    ///   GET  /api/kapatma/tanim?hesap=              açılış: yıllar, damga vergisi hesap kodu
    ///   GET  /api/kapatma/liste?...                 eski btnListele_Click (OdemeEmriMIFListele, mahsup)
    ///   POST /api/kapatma/belge                     eski btnBelge_Click: seçili satırlardan belge oluşturma penceresinin bilgileri
    ///   POST /api/kapatma/olustur                   eski btnBelgeOlustur_Click: OdemeEmriMIFKaydet + kapanış kayıtları
    ///   POST /api/kapatma/iptal                     eski btnBelgeIptalEt_Click
    ///   POST /api/kapatma/listeden-cikar            eski btnListeden_Click (kapanış kaydı belge no -1)
    ///   POST /api/kapatma/yazdir                    eski btnYazdir_Click (MUH/MUH-KapatmaListe.XLT, Excel)
    ///   POST /api/kapatma/banka-listesi             eski btnBankaListesi_Click (eski BankaListesi.rtf yerine MUH/MUH-KapatmaBankaListesi.dotx, Word)
    ///   GET  /api/kapatma/odeme-hesap?yil&amp;hesap&amp;ara  eski ListeOdemeHesabi.aspx
    ///   GET  /api/kapatma/firma/ara, hesap/agac, hesap/ara, hesap/ad   seçim pencereleri (Sgb.firmaSec, Sgb.hesapKodu)
    /// Yetki: menüdeki üç adresten birini görebilen kullanıcı.
    /// </summary>
    public static class KapatmaUclari
    {
        public const string SayfaAdres = "../ButceMuhasebe/Kapatma.aspx";
        public static readonly string[] MenuAdresleri = { SayfaAdres, SayfaAdres + "?hesap=323", SayfaAdres + "?hesap=103" };
        private const string Uc = "/api/kapatma/";
        private const string ListedenCikartildi = "Listeden çıkartıldı";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapPost(Uc + "belge", Belge);
            app.MapPost(Uc + "olustur", Olustur);
            app.MapPost(Uc + "iptal", Iptal);
            app.MapPost(Uc + "listeden-cikar", ListedenCikar);
            app.MapPost(Uc + "yazdir", Yazdir);
            app.MapPost(Uc + "banka-listesi", BankaListesi);
            app.MapGet(Uc + "odeme-hesap", OdemeHesap);
            app.MapGet(Uc + "firma/ara", FirmaAra);
            app.MapGet(Uc + "hesap/agac", HesapAgac);
            app.MapGet(Uc + "hesap/ara", HesapAra);
            app.MapGet(Uc + "hesap/ad", HesapAd);
        }

        /// <summary>Sayfa modeli: açılan adresin (hesap parametresiyle) menü yetkisi; menüde yoksa üç adresten biri.</summary>
        public static bool SayfaYetkili(Kullanici kullanan, string uygulamaKlasoru, string hesap)
        {
            string adres = string.IsNullOrEmpty(hesap) ? SayfaAdres : SayfaAdres + "?hesap=" + hesap;
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, adres, MenuAdresleri);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!SayfaYetki.Yetkili(kullanan, ortam.ContentRootPath, null, MenuAdresleri)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { hata = Duz(hata) }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        /// <summary>Eski mesajlardaki &lt;br&gt; satır sonuna çevrilir.</summary>
        private static string Duz(string s)
        {
            return (s ?? "").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n").Trim();
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return Duz(hata);
        }

        private static string Q(HttpContext baglam, string ad)
        {
            return ((string)baglam.Request.Query[ad] ?? "").Trim();
        }

        private static bool Evet(HttpContext baglam, string ad)
        {
            string d = Q(baglam, ad);
            return d == "1" || d.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        private static string Tutar(double d)
        {
            return d.ToString("#,###.00", new CultureInfo("tr-TR"));
        }

        /// <summary>Eski ButceMuhasebeGenel.DondurParaBirimi.</summary>
        private static ParaBirimi DondurParaBirimi(int paraBirimiKod)
        {
            TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            ObjectArray paralar = servisHRC.ParaBirimiListele(new Kullanici(), new ParaBirimi());
            foreach (ParaBirimi bilgi in paralar.objeler)
            {
                if (bilgi.kod == paraBirimiKod)
                    return bilgi;
            }
            return new ParaBirimi();
        }

        private static ObjectArray HesapKoduBilgisiAl(IMUHServis servisMUH, int yil, string hesapKodlar)
        {
            HesapPlaniSatir hesapKodKriter = new HesapPlaniSatir();
            hesapKodKriter.yil = OrtakFonksiyonlar.ConvertToInt(yil, DateTime.Now.Year);
            hesapKodKriter.hesapKod = hesapKodlar;
            return servisMUH.HesapPlaniListele(new Kullanici(), hesapKodKriter);
        }

        private static HesapPlaniSatir HesapPlaniBilgisiTopla(IMUHServis servisMUH, Kullanici kullanan, Hashtable hesapPlaniHT, int yil, string hesapKod)
        {
            if (hesapPlaniHT[hesapKod.Replace(".", "")] != null)
                return (HesapPlaniSatir)hesapPlaniHT[hesapKod.Replace(".", "")];

            HesapPlaniSatir hs = new HesapPlaniSatir();
            hs.yil = yil;
            hs.hesapKod = hesapKod;
            ObjectArray hVan = servisMUH.HesapPlaniListele(kullanan, hs);
            foreach (HesapPlaniSatir hss in hVan.objeler)
            {
                hesapPlaniHT[hss.hesapKod.Replace(".", "")] = hss;
                return hss;
            }
            return new HesapPlaniSatir();
        }

        /// <summary>Şablon: önce kurumun RaporSablonYol/MUH klasörü, yoksa uygulamanın RaporSablon/MUH klasörü.</summary>
        private static string SablonYol(string sablonAd)
        {
            string kurum = Path.Combine(Ayar.Oku("RaporSablonYol") ?? "", "MUH", sablonAd);
            if (File.Exists(kurum)) return kurum;
            // Geliştirmede şablonlar çıktı klasörüne kopyalanmaz (yalnız yayında); önce uygulama klasörüne (SGB.Net), sonra exe klasörüne bakılır.
            foreach (string kok in new string[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                string uygulama = Path.Combine(kok, "RaporSablon", "MUH", sablonAd);
                if (File.Exists(uygulama)) return uygulama;
            }
            return kurum;
        }

        private static string GunAyYil(string isoTarih)
        {
            TNSDateTime t = OdemeEmriMIFUclari.Tarih(isoTarih);
            return t.isNull ? "" : t.Oku().ToString("dd.MM.yyyy");
        }

        // ------------------------------------------------------------------ açılış

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            List<int> yillar = new List<int>();
            for (int i = 2006; i <= DateTime.Now.Year; i++) yillar.Add(i);
            string damga = "";
            try
            {
                damga = TNS.HRC.OrtakServis.DamgaVergisiHesapKodu("", false) ?? "";
            }
            catch (Exception)
            {
                damga = "";
            }
            return Results.Json(new { yil = DateTime.Now.Year, yillar = yillar, damgaHesapKod = damga });
        }

        // ------------------------------------------------------------------ listele (eski btnListele_Click)

        /// <summary>Eski KriterTopla.</summary>
        private static OdemeEmriMIFForm KriterTopla(HttpContext baglam, bool kapatilmis)
        {
            OdemeEmriMIFForm f = new OdemeEmriMIFForm();
            f.formDurum = 24; //Yevmiyeli ve onaylı
            f.yil = OrtakFonksiyonlar.ConvertToInt(Q(baglam, "yil"), 0);
            f.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            f.muhasebe = OrtakFonksiyonlar.ConvertToInt(Q(baglam, "muhasebe"), 0);
            f.birim = Q(baglam, "birim");
            if (Q(baglam, "belgeTarih1") != "")
                f.sorguIslemTarih = OdemeEmriMIFUclari.Tarih(Q(baglam, "belgeTarih1")).ToString() + ";";
            if (Q(baglam, "belgeTarih2") != "")
                f.sorguIslemTarih += OdemeEmriMIFUclari.Tarih(Q(baglam, "belgeTarih2")).ToString();
            if (Q(baglam, "yevTarih1") != "")
                f.sorguYEVYevTarih = OdemeEmriMIFUclari.Tarih(Q(baglam, "yevTarih1")).ToString() + ";";
            if (Q(baglam, "yevTarih2") != "")
                f.sorguYEVYevTarih += OdemeEmriMIFUclari.Tarih(Q(baglam, "yevTarih2")).ToString();

            f.aciklama = Q(baglam, "aciklama");

            if (Evet(baglam, "yevmiyeli"))
                f.formDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;

            string yevNo1 = Q(baglam, "yevNo1");
            string yevNo2 = Q(baglam, "yevNo2");
            if (OrtakFonksiyonlar.ConvertToInt(yevNo1, 0) > 0 || OrtakFonksiyonlar.ConvertToInt(yevNo2, 0) > 0)
                f.sorguYEVYevNo = yevNo1 + ";" + yevNo2;

            OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(f.yil);
            detay.kur = "";
            detay.fon = "";
            detay.fin = "";
            detay.eko = "";
            detay.peb = "";
            detay.hesapKod = Q(baglam, "hesapKod").Replace(".", "").Trim();

            KapanisBilgileri kapatma = new KapanisBilgileri();
            kapatma.yil = f.yil;
            kapatma.belgeNo = Q(baglam, "kapatmaBelgeNo");
            if (kapatilmis && kapatma.belgeNo != "")
            {
                detay.kapatma = new ObjectArray();
                detay.kapatma.objeler.Add(kapatma);
            }

            if (detay.hesapKod.StartsWith("103") || detay.hesapKod.StartsWith("323"))
                detay.alacak = 0.000001;
            else if (detay.hesapKod.StartsWith("16"))
                detay.borc = 0.000001;

            f.detay.objeler.Add(detay);
            return f;
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                bool kapatilmis = Evet(baglam, "kapatilmis");
                return ListeOku(kullanan, KriterTopla(baglam, kapatilmis), kapatilmis, Q(baglam, "kapatmaBelgeNo"));
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste okunamadı."));
            }
        }

        private static IResult ListeOku(Kullanici kullanan, OdemeEmriMIFForm f, bool kapatilmis, string kapatmaBelgeNo)
        {
            IMUHServis servisMUH = Servis();
            ParaBirimi pbKullanilan = DondurParaBirimi(1);

            OdemeEmriMIFDetay detayKriter = (OdemeEmriMIFDetay)f.detay[0];
            if (string.IsNullOrEmpty(detayKriter.hesapKod) && kapatilmis && string.IsNullOrEmpty(kapatmaBelgeNo))
                return Hata("Listeleme yapmak istediğiniz hesap kodunu belirtin.");

            ObjectArray bilgi = servisMUH.OdemeEmriMIFListele(kullanan, f, true, true);
            if (!bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi.sonuc.hataStr, "Liste okunamadı."));

            List<KapatmaSatir> satirlar = new List<KapatmaSatir>();
            Hashtable htKapatmaBelge = new Hashtable();
            Hashtable htKapatmaBelgeDurum = new Hashtable();

            bool tertipGizle = true;
            bool cekGizle = true;
            bool yevmiyeGizle = true;
            double toplamTutar = 0;
            double toplamAlacak = 0;
            double toplamBorc = 0;
            double toplamKapatma = 0;

            if (bilgi.objeler.Count > 0)
            {
                string paraBirimi = pbKullanilan.kisaltma;

                //Raporlanacak hesapkodlarını bulmak için
                string hesapKodlar = "";
                foreach (OdemeEmriMIFForm oem in bilgi.objeler)
                {
                    foreach (OdemeEmriMIFDetay detay in oem.detay.objeler)
                    {
                        if (hesapKodlar != "") hesapKodlar += ";";
                        if (hesapKodlar.IndexOf(detay.hesapKod + ";") > -1) continue;
                        hesapKodlar += detay.hesapKod;
                    }
                }
                ObjectArray hesapKodlari = HesapKoduBilgisiAl(servisMUH, f.yil, hesapKodlar);

                foreach (OdemeEmriMIFForm oem in bilgi.objeler)
                {
                    ObjectArray yeniDetay = new ObjectArray();
                    foreach (OdemeEmriMIFDetay detay in oem.detay.objeler)
                    {
                        OdemeEmriMIFDetay icDetay = detay;
                        icDetay.belgeNo = oem.belgeNo;

                        bool bulundu = false;
                        foreach (OdemeEmriMIFDetay od in yeniDetay.objeler)
                        {
                            if (detay.sorguKapatan > 0) continue;

                            if (od.hesapKod == detay.hesapKod && od.altHesapKod == detay.altHesapKod && od.detayBirim == detay.detayBirim)
                            {
                                od.borcYPB += Math.Round(detay.borcYPB, 2);
                                od.alacakYPB += Math.Round(detay.alacakYPB, 2);

                                foreach (KapanisBilgileri kk in detay.kapatma.objeler)
                                {
                                    bool ekle = true;
                                    foreach (KapanisBilgileri kk2 in od.kapatma.objeler)
                                    {
                                        if (kk2.belgeNo == kk.belgeNo)
                                            ekle = false;
                                    }

                                    if (ekle)
                                        od.kapatma.objeler.Add(kk);
                                }
                                bulundu = true;
                            }
                        }

                        if (!bulundu)
                            yeniDetay.objeler.Add(icDetay);
                    }

                    oem.detay = yeniDetay;
                }

                foreach (OdemeEmriMIFForm oem in bilgi.objeler)
                {
                    foreach (OdemeEmriMIFDetay detay in oem.detay.objeler)
                    {
                        double kapatmaBorc = 0;
                        double kapatmaAlacak = 0;
                        double kapatmaBorcAlacak = 0;

                        double kapatmaTutari = 0.0;
                        double gercekTutar = Math.Round(detay.borcYPB, 2) - Math.Round(detay.alacakYPB, 2);
                        double kalanTutar = 0.0;
                        if (detay.kapatma != null)
                        {
                            foreach (KapanisBilgileri kapatma in detay.kapatma.objeler)
                            {
                                if (string.IsNullOrEmpty(kapatma.belgeNo)) continue;

                                if (kapatma.borcAlacak == 1)
                                    kapatmaBorc += Math.Round(kapatma.kTutar, 2);
                                else if (kapatma.borcAlacak == 2)
                                    kapatmaAlacak += Math.Round(kapatma.kTutar, 2);
                                else
                                {
                                    kapatmaBorcAlacak = Math.Round(kapatma.kTutar, 2);
                                    if (detay.borc > 0) kapatmaAlacak += Math.Round(kapatmaBorcAlacak, 2);
                                    if (detay.alacak > 0) kapatmaBorc += Math.Round(kapatmaBorcAlacak, 2);
                                }

                                int kapatmaBelgeDurum = (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
                                if (kapatma.belgeNo != "-1")
                                {
                                    try
                                    {
                                        kapatmaBelgeDurum = (int)htKapatmaBelgeDurum[kapatma.yil + "-" + kapatma.belgeNo];
                                    }
                                    catch { kapatmaBelgeDurum = -99; }

                                    if (kapatmaBelgeDurum == -99)
                                    {
                                        kapatmaBelgeDurum = servisMUH.BelgeDurumBilgisi(kapatma.yil, kapatma.belgeNo);
                                        htKapatmaBelgeDurum[kapatma.yil + "-" + kapatma.belgeNo] = kapatmaBelgeDurum;
                                    }
                                }

                                if (kapatmaBelgeDurum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                                    kapatmaTutari += Math.Round(kapatma.kTutar, 2);
                            }
                        }
                        if (kapatilmis)
                        {
                            if (kapatmaTutari == 0) continue;
                            kalanTutar = Math.Round(Math.Abs(gercekTutar), 2) - Math.Round(kapatmaTutari, 2);
                        }
                        else
                        {
                            kalanTutar = Math.Round(Math.Abs(gercekTutar), 2) - Math.Round(kapatmaTutari, 2);
                            if (kalanTutar == 0) continue;
                        }

                        //Borc veya alacak çalışan hesapkodlarında mahsup belgelerini listede gösterme
                        bool islemeAlma = false;
                        foreach (HesapPlaniSatir h in hesapKodlari.objeler)
                        {
                            if (h.hesapKod.Replace(".", "") == detay.hesapKod.Replace(".", ""))
                            {
                                if (h.bakiye == 1)
                                {
                                    if (Math.Round(detay.borc, 2) - Math.Round(detay.alacak, 2) < 0 && detay.mahsupBelgeNo != "") islemeAlma = true;
                                }
                                else if (h.bakiye == 2)
                                {
                                    if (Math.Round(detay.borc, 2) - Math.Round(detay.alacak, 2) > 0 && detay.mahsupBelgeNo != "") islemeAlma = true;
                                }
                                else
                                {
                                    //Borç-Alacak çalışıyorsa
                                    if (Math.Round(detay.borc, 2) == Math.Round(detay.alacak, 2)) islemeAlma = true;
                                }
                            }
                        }

                        // Mahsup belgesinde bu hesap kodu için mahsup yapılmıyorsa rapora katmak için hesap bazında bakılır.
                        if (!islemeAlma && detay.sorguKapatan > 0)
                        {
                            KapanisBilgileri kap = new KapanisBilgileri();
                            kap.yil = oem.yil;
                            kap.belgeNo = oem.belgeNo;
                            kap.kHesapKod = detay.hesapKod;
                            kap.kIlgiliVno = detay.altHesapKod;
                            kap.kBirimKod = detay.detayBirim;

                            string anahtar = kap.yil + "-" + kap.belgeNo + "-" + kap.kHesapKod + "-" + kap.kIlgiliVno + "-" + kap.kBirimKod;
                            ObjectArray kl = (ObjectArray)htKapatmaBelge[anahtar];
                            if (kl == null)
                            {
                                kl = servisMUH.KapanisBilgisiListele(kap);
                                htKapatmaBelge[anahtar] = kl;
                            }

                            if (kl.objeler.Count > 0) islemeAlma = true;
                        }

                        if (islemeAlma) continue;

                        detay.borcYPB = detay.borcYPB + kapatmaBorc;
                        detay.alacakYPB = detay.alacakYPB + kapatmaAlacak;

                        string tertip = TNS.MUH.Arac.TertipStrOl(detay);
                        if (tertip == "") tertip = oem.kurKod;

                        KapatmaSatir row = new KapatmaSatir();
                        row.belgeYil = oem.yil;
                        row.belgeNo = oem.belgeNo;
                        row.belgeTarih = OdemeEmriMIFUclari.TarihYaz(oem.islemTarih);
                        row.muhasebeKod = oem.muhasebe;
                        row.birimKod = detay.detayBirim;
                        row.muhasebeAd = oem.muhasebe + " - " + oem.muhasebeAd;
                        row.birimAd = detay.detayBirim;
                        row.cekNo = oem.YEVCekNo ?? "";
                        row.hesapKod = detay.hesapKod;
                        row.tertip = tertip ?? "";
                        row.ilgiliAd = oem.ilgiliAd ?? "";
                        row.ilgiliVNo = oem.ilgiliNo ?? "";
                        row.borc = detay.borcYPB;
                        row.alacak = detay.alacakYPB;
                        row.siraNo = detay.satirNo.ToString();
                        row.altHesapKod = detay.altHesapKod + "-" + detay.altHesapAd;

                        if (row.borc == row.alacak && row.borc > 0.0)
                            kalanTutar = Math.Abs(row.borc - row.alacak);

                        row.tutar = kalanTutar;
                        row.ilgiliIBAN = oem.ilgiliBankaNo ?? "";
                        row.ilgiliBanka = oem.ilgiliBankaAd ?? "";
                        row.istatistik1Ad = oem.istKod1 != null ? oem.istKod1.aciklama ?? "" : "";
                        row.istatistik2Ad = oem.istKod2 != null ? oem.istKod2.aciklama ?? "" : "";
                        row.paraBirimi = string.IsNullOrEmpty(detay.doviz) ? paraBirimi : detay.doviz;
                        row.kTutar = row.tutar;
                        row.yevmiyeNo = oem.YEVYevNo;
                        row.yevmiyeTarih = OdemeEmriMIFUclari.TarihYaz(oem.YEVYevTarih);
                        row.kapatmaBelgeNo = "";

                        if (kapatilmis)
                        {
                            if (detay.kapatma != null)
                            {
                                foreach (KapanisBilgileri kapatma in detay.kapatma.objeler)
                                {
                                    if (kapatma.belgeNo == "-1")
                                        kapatma.belgeNo = ListedenCikartildi;

                                    KapatmaSatir row2 = Kopya(row);
                                    row2.borc = detay.borcYPB;
                                    row2.alacak = detay.alacakYPB;
                                    row2.tutar = kalanTutar;
                                    row2.kapatmaBelgeYil = kapatma.yil;
                                    row2.kapatmaBelgeNo = kapatma.belgeNo;
                                    row2.kTutar = kalanTutar;

                                    toplamBorc += detay.borcYPB;
                                    toplamAlacak += detay.alacakYPB;
                                    toplamTutar += kalanTutar;
                                    toplamKapatma += kalanTutar;

                                    satirlar.Add(row2);
                                }
                            }
                        }
                        else
                        {
                            toplamBorc += detay.borcYPB;
                            toplamAlacak += detay.alacakYPB;
                            toplamTutar += kalanTutar;
                            toplamKapatma += kalanTutar;

                            satirlar.Add(row);
                        }

                        if (row.tertip != "") tertipGizle = false;
                        if (row.cekNo != "") cekGizle = false;
                        if (oem.YEVYevNo > 0) yevmiyeGizle = false;
                    }
                }
            }

            return Results.Json(new
            {
                satirlar = satirlar,
                bilgi = bilgi.objeler.Count == 0 ? "Aradığınız kriterlere uygun kayıt bulunamadı." : null,
                toplam = new { borc = toplamBorc, alacak = toplamAlacak, tutar = toplamTutar, kTutar = toplamKapatma },
                gizle = new
                {
                    yevmiye = yevmiyeGizle,
                    muhasebe = f.muhasebe > 0,
                    birim = f.birim != "",
                    tertip = tertipGizle,
                    cek = cekGizle
                },
                kapatilmis = kapatilmis
            });
        }

        private static KapatmaSatir Kopya(KapatmaSatir s)
        {
            return new KapatmaSatir
            {
                belgeYil = s.belgeYil, belgeNo = s.belgeNo, belgeTarih = s.belgeTarih, muhasebeKod = s.muhasebeKod, birimKod = s.birimKod,
                muhasebeAd = s.muhasebeAd, birimAd = s.birimAd, borc = s.borc, alacak = s.alacak, tutar = s.tutar, kTutar = s.kTutar,
                cekNo = s.cekNo, hesapKod = s.hesapKod, tertip = s.tertip, ilgiliAd = s.ilgiliAd, ilgiliVNo = s.ilgiliVNo,
                kapatmaBelgeYil = s.kapatmaBelgeYil, kapatmaBelgeNo = s.kapatmaBelgeNo, yevmiyeNo = s.yevmiyeNo, yevmiyeTarih = s.yevmiyeTarih,
                ilgiliIBAN = s.ilgiliIBAN, ilgiliBanka = s.ilgiliBanka, paraBirimi = s.paraBirimi, siraNo = s.siraNo, altHesapKod = s.altHesapKod,
                istatistik1Ad = s.istatistik1Ad, istatistik2Ad = s.istatistik2Ad
            };
        }

        // ------------------------------------------------------------------ belge oluşturma penceresi (eski btnBelge_Click)

        private static async Task<IResult> Belge(HttpContext baglam, IWebHostEnvironment ortam, KapatmaSeciliIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<KapatmaSatir> secili = istek?.satirlar ?? new List<KapatmaSatir>();
            if (secili.Count == 0) return Hata("Listeden herhangi bir belge seçilmediği için işlem yapılamadı.");

            try
            {
                IMUHServis servisMUH = Servis();
                List<OdemeEmriMIFDetay> liste = new List<OdemeEmriMIFDetay>();
                string sakliMuhasebe = "";
                string sakliBirim = "";
                string sakliParaBirimi = "";
                bool muhasebeBirimAyni = true;
                string sakliHesapKod = "000";
                string sakliIlgiliVNo = "";
                bool firmaAyni = true;
                bool borcMahsup = false;

                foreach (KapatmaSatir s in secili)
                {
                    string hesapKod = s.hesapKod ?? "";
                    string tertip = s.tertip ?? "";
                    double tutar = s.tutar;
                    double kTutar = s.kTutar;
                    string muhasebe = s.muhasebeKod.ToString();
                    string birim = s.birimKod ?? "";
                    string ilgiliVNo = s.ilgiliVNo ?? "";
                    string paraBirimi = s.paraBirimi ?? "";

                    if (sakliParaBirimi != "" && sakliParaBirimi != paraBirimi)
                        return Hata("Farklı para birimlerinden oluşan bir kapatma belgesi düzenlenemez. Lütfen, kapatmak istediğiniz hesapları tek para biriminden seçiniz.");
                    sakliParaBirimi = paraBirimi;

                    borcMahsup = false;
                    if (s.alacak == 0) borcMahsup = true;

                    if (Math.Abs(kTutar) > Math.Abs(tutar)) //Eksi rakam (Alacak-Borc) olduğu için olabilir
                        return Hata("Kapatılmak istenilen belgenin tutarı " + Tutar(tutar) + " iken " + Tutar(kTutar) + " olarak kapatılmak isteniyor. Lütfen düzeleterek tekrar deneyin");

                    if (sakliHesapKod.Substring(0, 3) != hesapKod.Substring(0, 3))
                        if (sakliHesapKod == "000")
                            sakliHesapKod = hesapKod;

                    if (sakliIlgiliVNo != ilgiliVNo)
                        if (sakliIlgiliVNo != "")
                            firmaAyni = false;
                        else
                            sakliIlgiliVNo = ilgiliVNo;

                    if (muhasebeBirimAyni)
                    {
                        if (sakliMuhasebe == "" && sakliBirim == "")
                        {
                            sakliMuhasebe = muhasebe;
                            sakliBirim = birim;
                        }
                        else if (muhasebe != sakliMuhasebe || birim != sakliBirim)
                            muhasebeBirimAyni = false;
                    }

                    OdemeEmriMIFDetay l = new OdemeEmriMIFDetay(DateTime.Now.Year);
                    l.belgeNo = s.belgeNo;
                    l.satirNo = OrtakFonksiyonlar.ConvertToInt(s.siraNo, 0);
                    l.hesapKod = hesapKod;
                    l.altHesapKod = s.altHesapKod ?? "";
                    l.doviz = paraBirimi;

                    TNS.MUH.Arac.TertipAta(ref l, tertip);

                    if (kTutar < 0 || borcMahsup)
                        l.alacak = Math.Abs(kTutar);
                    else
                        l.borc = kTutar;
                    l.detayBirim = birim + "." + l.kur;

                    liste.Add(l);
                }

                string tertipSakli = "";
                string tersIslem = "1";
                string sakliHesapKodu = sakliHesapKod.Substring(0, 3);
                string kapamaHesabi = "";
                bool hesap103 = sakliHesapKodu == "103";
                if (hesap103)
                {
                    ObjectArray hListe = HesapKoduBilgisiAl(servisMUH, DateTime.Now.Year, "102");
                    if (hListe.objeler.Count == 2) //Sadece 102.01 varsa
                        kapamaHesabi = "102.01";
                }

                if (sakliHesapKodu.StartsWith("16") || sakliHesapKodu.StartsWith("105"))
                {
                    //Mahsup yapılmak istenen ilk hesap kodunu al ve onu borç-alacak ilişkisini test et.
                    string testHesapKodu = liste[0].hesapKod;
                    ObjectArray hListe = HesapKoduBilgisiAl(servisMUH, DateTime.Now.Year, testHesapKodu);
                    foreach (HesapPlaniSatir hh in hListe.objeler)
                    {
                        if (hh.BorcAlacakCalisirMi())
                        {
                            tersIslem = "-1";
                            break;
                        }
                    }
                }

                List<KapanisSatir> kapatmaListe = new List<KapanisSatir>();
                double kapatmaTutari = 0;
                foreach (OdemeEmriMIFDetay sakli in liste)
                {
                    if (tersIslem == "-1" && sakli.borc > 0)
                    {
                        sakli.alacak = sakli.borc;
                        sakli.borc = 0;
                    }
                    kapatmaListe.Add(new KapanisSatir
                    {
                        hesapKod = sakli.hesapKod, altHesapKod = sakli.altHesapKod, birim = sakli.detayBirim, tertip = TNS.MUH.Arac.TertipStrOl(sakli),
                        borc = sakli.borc, alacak = sakli.alacak, sunucudanGeldi = 1, siraNo = sakli.satirNo.ToString(), belgeNo = sakli.belgeNo, paraBirimi = sakli.doviz
                    });

                    kapatmaTutari += Math.Abs(sakli.borc - sakli.alacak);
                    tertipSakli = TNS.MUH.Arac.TertipStrOl(sakli);
                }

                if (kapamaHesabi != "")
                {
                    double toplamBorc = 0;
                    double toplamAlacak = kapatmaTutari;
                    if (tersIslem == "-1")
                    {
                        toplamBorc = toplamAlacak;
                        toplamAlacak = 0;
                    }
                    kapatmaListe.Add(new KapanisSatir
                    {
                        hesapKod = kapamaHesabi, altHesapKod = "", birim = "", tertip = "", borc = toplamBorc, alacak = toplamAlacak,
                        sunucudanGeldi = 1, siraNo = "0", belgeNo = "", paraBirimi = ""
                    });
                }

                //Eğer borç mahsubu yapıyorsa kapatma hesap kodunu eklerken borç kısmına yazması gerekli.
                if (borcMahsup)
                    tersIslem = "-1";

                string butceHesapKod = "";
                if (tertipSakli.Trim() != "")
                {
                    string[] kodlar = tertipSakli.Split('-');
                    if (kodlar.Length > 3)
                    {
                        string[] ekolar = kodlar[3].Split('.');
                        butceHesapKod = "630";
                        for (int i = 0; i < ekolar.Length; i++)
                        {
                            if (butceHesapKod != "") butceHesapKod += ".";
                            butceHesapKod += ekolar[i].PadLeft(2, '0');
                        }
                    }
                }

                // Eski: belge penceresinin muhasebe / birimi önce liste kıstaslarından, seçilenlerin hepsi aynıysa onlardan.
                string belgeMuhasebe = Temiz(istek.muhasebe);
                string belgeBirim = Temiz(istek.birim);
                if (muhasebeBirimAyni)
                {
                    belgeMuhasebe = sakliMuhasebe;
                    belgeBirim = sakliBirim;
                }

                string ilgiliAd = "";
                string firmaNo = "";
                if (firmaAyni && !string.IsNullOrEmpty(sakliIlgiliVNo))
                {
                    TNS.HRC.FirmaBilgisi f = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, OrtakFonksiyonlar.ConvertToInt(belgeMuhasebe, 0), sakliIlgiliVNo);
                    if (f != null)
                    {
                        ilgiliAd = f.ad ?? "";
                        firmaNo = f.kod.ToString();
                    }
                }

                return Results.Json(new
                {
                    satirlar = kapatmaListe,
                    hesap103 = hesap103,
                    tersIslem = tersIslem,
                    sakliHesapKodu = sakliHesapKodu,
                    tertip = tertipSakli,
                    kapatmaTutari = kapatmaTutari,
                    paraBirimi = sakliParaBirimi,
                    muhasebe = belgeMuhasebe,
                    birim = belgeBirim,
                    ilgiliAd = ilgiliAd,
                    firmaNo = firmaNo,
                    butceHesapKod = butceHesapKod,
                    tarih = DateTime.Now.ToString("yyyy-MM-dd"),
                    yil = DateTime.Now.Year
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belge bilgileri hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ belge oluştur (eski btnBelgeOlustur_Click)

        private static async Task<IResult> Olustur(HttpContext baglam, IWebHostEnvironment ortam, KapatmaBelgeIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Hata("İşlem yapılamadı.");

            string hata = "";
            if (Temiz(istek.muhasebe) == "")
                hata += "Muhasebe Kod bilgisi boş bırakılmış.\n";
            if (Temiz(istek.birim) == "")
                hata += "Birim Kod bilgisi boş bırakılmış.\n";
            if (hata != "") return Hata(hata);

            try
            {
                IMUHServis servisMUH = Servis();
                TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
                string sakliDovizKurTurAlis = "";

                OdemeEmriMIFForm tf = new OdemeEmriMIFForm();
                int yil = istek.yil;
                tf.yil = yil;
                tf.belgeNo = "";
                TNSDateTime tarih = OdemeEmriMIFUclari.Tarih(istek.tarih);
                if (tarih.isNull)
                {
                    if (yil < DateTime.Now.Year)
                        tf.islemTarih = new TNSDateTime("31.12." + yil);
                    else
                        tf.islemTarih = new TNSDateTime(DateTime.Now.ToString());
                }
                else
                    tf.islemTarih = tarih;

                tf.muhasebe = OrtakFonksiyonlar.ConvertToInt(Temiz(istek.muhasebe), 0);
                tf.birim = Temiz(istek.birim);
                tf.kurKod = "";
                tf.formDurumTarih = new TNSDateTime(DateTime.Now.ToString());
                tf.islemYapan = kullanan.kullaniciKodu;
                tf.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
                tf.aciklama = istek.aciklama ?? "";
                tf.projeNo = "MAHSUP";

                //Kur listesini al
                ParaBirimi pbKullanilan = DondurParaBirimi(1);
                TNS.HRC.MerkezBankasiKurSinif kriter = new TNS.HRC.MerkezBankasiKurSinif();
                kriter.kurTarih = tf.islemTarih;
                TNS.HRC.MerkezBankasiKurSinif kurListe = servisHRC.MerkezBankKurOku(kullanan, kriter);

                if (istek.firmaNo > 0)
                {
                    TNS.HRC.FirmaBilgisi f = servisHRC.FirmaBilgisiListele(kullanan, istek.firmaNo);
                    if (f != null)
                    {
                        tf.ilgiliAd = f.ad;
                        tf.ilgiliBankaAd = f.banka;
                        tf.ilgiliBankaNo = f.hesapNo;
                        tf.ilgiliVD = f.vergiDairesi;
                        tf.ilgiliNo = f.vergiNo;
                    }
                }
                Hashtable hesapPlaniHT = new Hashtable();

                List<KapanisSatir> grid1Data = istek.kapanisSatirlari ?? new List<KapanisSatir>();
                double damgaVergisiTutar = 0.0;
                decimal kurDegeri = 0;
                bool satirOlustur830 = false;
                for (int i = 0; i < grid1Data.Count; i++)
                {
                    string hesapKod = grid1Data[i].hesapKod ?? "";
                    string tertip = grid1Data[i].tertip ?? "";
                    string detayBirim = grid1Data[i].birim ?? "";
                    string mahsupBelgeNo = grid1Data[i].belgeNo ?? "";
                    double borc = grid1Data[i].borc;
                    double alacak = grid1Data[i].alacak;
                    string altHesapKod = grid1Data[i].altHesapKod ?? "";
                    string paraBirimi = grid1Data[i].paraBirimi ?? "";

                    if (hesapKod.StartsWith("360.03") && TNS.HRC.OrtakServis.KurumButceTur() == TNS.HRC.ENUMKurumButceTur.GENELBUTCE)
                        damgaVergisiTutar = alacak;

                    OdemeEmriMIFDetay l = new OdemeEmriMIFDetay(tf.yil);
                    l.hesapKod = hesapKod;
                    l.satirNo = i + 1;

                    TNS.MUH.Arac.TertipAta(ref l, tertip);

                    HesapPlaniSatir hs = HesapPlaniBilgisiTopla(servisMUH, kullanan, hesapPlaniHT, tf.yil, hesapKod);
                    if (hs.dovizKurKod == "" && paraBirimi == pbKullanilan.kisaltma)
                        hs.dovizKurKod = pbKullanilan.kisaltma;
                    else if (hs.dovizKurKod == "")
                    {
                        hs.dovizKurKod = paraBirimi;
                        hs.dovizKurTurAlis = sakliDovizKurTurAlis;
                    }

                    if (hs.dovizKurKod != "" && hs.dovizKurKod != paraBirimi)
                        return Hata(hs.hesapKod + " hesap kodunun tanımı " + paraBirimi + " olarak tanımlı değil. Bu hesabı, bu para birimninden kullanmak için, Hesap Planı tanımlama ekranından döviz bilgisi değiştirmeniz gereklidir.");

                    if (paraBirimi != "" && paraBirimi != pbKullanilan.kisaltma)
                    {
                        kurDegeri = 0;
                        foreach (TNS.HRC.MerkezBankasiKurDetaySinif kur in kurListe.satirlar)
                        {
                            if (kur.paraBirim.kisaltma == hs.dovizKurKod)
                            {
                                kurDegeri = kur.DegerBul(hs.dovizKurTurAlis, -1);
                                sakliDovizKurTurAlis = hs.dovizKurTurAlis;

                                if (kurDegeri == -1)
                                    return Hata(hs.hesapKod + " hesap kodunun tanımında işlem kur tipi belirtilmemiş. Bu hesabı kullanmak için, Hesap Planı tanımlama ekranından döviz bilgisini düzenlemeniz gereklidir.");
                            }
                        }

                        //döviz kapatması yapılacak
                        l.doviz = hs.dovizKurKod;
                        l.dovizBorc = borc;
                        l.dovizAlacak = alacak;
                        l.dovizKur = kurDegeri;
                        l.dovizKurTur = hs.dovizKurTurAlis;

                        l.borc = borc * OrtakFonksiyonlar.ConvertToDbl(kurDegeri);
                        l.alacak = alacak * OrtakFonksiyonlar.ConvertToDbl(kurDegeri);
                    }
                    else
                    {
                        l.borc = borc;
                        l.alacak = alacak;
                    }

                    l.detayBirim = detayBirim;

                    l.altHesapKod = "";
                    if (!string.IsNullOrEmpty(altHesapKod))
                        l.altHesapKod = altHesapKod.Split('-')[0];

                    l.mahsupBelgeNo = mahsupBelgeNo;

                    if (hesapKod.StartsWith("16"))
                        l.kesinti = 1;

                    tf.detay.objeler.Add(l);

                    if (l.hesapKod.StartsWith("323") || l.hesapKod.StartsWith("630"))
                        satirOlustur830 = true;
                }

                if (satirOlustur830)
                    Olustur830(yil, tf);

                if (damgaVergisiTutar > 0)
                {
                    if (kurDegeri == 0) kurDegeri = 1;
                    damgaVergisiTutar = damgaVergisiTutar * OrtakFonksiyonlar.ConvertToDbl(kurDegeri);
                    tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, TNS.HRC.OrtakServis.DamgaVergisiYansitmaHesapKodu(), 0, "", 0, damgaVergisiTutar, 0));
                    tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "805", 0, "", damgaVergisiTutar, 0, 0));
                }

                Sonuc sonuc = servisMUH.OdemeEmriMIFKaydet(kullanan, tf);
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Belge kaydedilemedi."));

                if (istek.yevmiye)
                {
                    tf.belgeNo = sonuc.anahtar;
                    tf.formDurum = (int)ENUMBelgeDurumu.ICMALYEVMIYENOVER;
                    tf.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();

                    TNSCollection fler = new TNSCollection();
                    fler.Add(tf);
                    servisMUH.BelgeDurumDegistirODEMIF(kullanan, fler, true);
                }

                string kapanisHata = KapanisKayitlariniYaz(servisMUH, kullanan, tf.yil, sonuc.anahtar, istek.seciliBelgeler);

                return Results.Json(new { basarili = true, yil = tf.yil, belgeNo = sonuc.anahtar, kapanisHata = kapanisHata });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Belge kaydedilemedi."));
            }
        }

        /// <summary>Eski Olustur830.</summary>
        private static void Olustur830(int yil, OdemeEmriMIFForm tf)
        {
            double toplam835 = 0;
            string yansitma323 = "";
            string tertip323 = "";
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            int eko4Uz = tUzBul.KodUzunlukBul(3, 4, true);

            ObjectArray tdYedek = new ObjectArray();
            foreach (OdemeEmriMIFDetay td in tf.detay.objeler)
                tdYedek.objeler.Add(td);

            foreach (OdemeEmriMIFDetay td in tdYedek.objeler)
            {
                string hesapKod = td.hesapKod;
                string tertip = td.TertipStr();
                double borc = td.borc;

                if (hesapKod.StartsWith("323") || hesapKod.StartsWith("630"))
                {
                    if (hesapKod.StartsWith("323"))
                    {
                        yansitma323 = hesapKod.Replace("323", "831");
                        tertip323 = tertip;
                    }

                    string[] tertipParca = tertip.Split('-');
                    if (tertipParca.Length > 3)
                        hesapKod = TNS.MUH.Arac.EkonomikKoddanHesapkodBul(tertipParca[3], "830", eko4Uz);
                    else
                        hesapKod = "830" + hesapKod.Substring(3);

                    tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, hesapKod, 0, tertip, borc, 0, 0));
                    toplam835 += borc;
                }
            }

            if (yansitma323 != "")
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, yansitma323, 0, tertip323, 0, toplam835, 0));
            else
                tf.detay.objeler.Add(TNS.HRC.OrtakServis.OdemeEmriDetaySatirOlustur(tf.yil, "835", 0, "", 0, toplam835, 0));
        }

        /// <summary>Eski KapanisKayitlariniYaz. Hata olursa metnini döndürür (eski sayfa ayrı bir mesaj kutusunda gösterirdi).</summary>
        private static string KapanisKayitlariniYaz(IMUHServis servisMUH, Kullanici kullanan, int yil, string belgeNo, List<KapatmaSatir> satirlar)
        {
            ObjectArray liste = new ObjectArray();
            if (satirlar != null)
            {
                for (int i = 0; i < satirlar.Count; i++)
                {
                    KapatmaSatir s = satirlar[i];
                    string[] ilgili = (s.altHesapKod ?? "").Split('-');

                    KapanisBilgileri kapatma = new KapanisBilgileri();
                    kapatma.yil = yil;
                    kapatma.belgeNo = belgeNo;
                    kapatma.kYil = s.belgeYil;
                    kapatma.kBelgeNo = s.belgeNo;
                    kapatma.kHesapKod = s.hesapKod;
                    kapatma.kIlgiliVno = ilgili[0];
                    kapatma.kTertip = s.tertip;
                    kapatma.kTutar = s.kTutar;
                    kapatma.kBirimKod = s.birimKod;
                    kapatma.siraNo = i + 1;
                    liste.objeler.Add(kapatma);
                }
            }

            if (liste.objeler.Count > 0)
            {
                Sonuc sonuc = servisMUH.KapanisBilgisiKaydet(kullanan, liste);
                if (!sonuc.islemSonuc)
                    return "Kapanış kayıtları kayıt edilemedi" + Duz(sonuc.hataStr);
            }
            return "";
        }

        // ------------------------------------------------------------------ kapatma belgesi iptal (eski btnBelgeIptalEt_Click)

        private static async Task<IResult> Iptal(HttpContext baglam, IWebHostEnvironment ortam, KapatmaSeciliIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<KapatmaSatir> secili = istek?.satirlar ?? new List<KapatmaSatir>();
            if (secili.Count == 0) return Hata("Listeden herhangi bir belge seçilmediği için işlem yapılamadı.");

            try
            {
                IMUHServis servisMUH = Servis();
                ObjectArray liste = new ObjectArray();
                foreach (KapatmaSatir s in secili)
                {
                    if (s.kapatmaBelgeNo == ListedenCikartildi) continue;
                    KapanisBilgileri kapatma = new KapanisBilgileri();
                    kapatma.yil = s.kapatmaBelgeYil;
                    kapatma.belgeNo = s.kapatmaBelgeNo;
                    liste.objeler.Add(kapatma);
                }

                Sonuc sonuc = new Sonuc(true, "", "");
                if (liste.objeler.Count > 0)
                    sonuc = servisMUH.KapanisBilgisiIptalEtHazirla(kullanan, liste);
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "İptal edilemedi."));

                liste = new ObjectArray();
                foreach (KapatmaSatir s in secili)
                {
                    if (s.kapatmaBelgeNo != ListedenCikartildi) continue;
                    KapanisBilgileri kapatma = new KapanisBilgileri();
                    kapatma.yil = s.belgeYil;
                    kapatma.kBelgeNo = s.belgeNo;
                    liste.objeler.Add(kapatma);
                }
                if (liste.objeler.Count > 0)
                    sonuc = servisMUH.KapanisBilgisiIptalEt(kullanan, liste);
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "İptal edilemedi."));

                return Results.Json(new { basarili = true, bilgi = "Seçili kapatma belgeleri iptal edildi." });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İptal edilemedi."));
            }
        }

        // ------------------------------------------------------------------ listeden çıkart (eski btnListeden_Click)

        private static async Task<IResult> ListedenCikar(HttpContext baglam, IWebHostEnvironment ortam, KapatmaSeciliIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<KapatmaSatir> secili = istek?.satirlar ?? new List<KapatmaSatir>();
            if (secili.Count == 0) return Hata("Listeden herhangi bir belge seçilmediği için işlem yapılamadı.");
            try
            {
                int yil = istek.yil > 0 ? istek.yil : DateTime.Now.Year;
                string hata = KapanisKayitlariniYaz(Servis(), kullanan, yil, "-1", secili);
                if (hata != "") return Hata(hata);
                return Results.Json(new { basarili = true });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "İşlem yapılamadı."));
            }
        }

        // ------------------------------------------------------------------ liste yazdır (eski btnYazdir_Click)

        private static async Task<IResult> Yazdir(HttpContext baglam, IWebHostEnvironment ortam, KapatmaSeciliIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            List<KapatmaSatir> grid1Data = istek?.satirlar ?? new List<KapatmaSatir>();
            if (grid1Data.Count == 0) return Hata("Listeden herhangi bir belge seçilmediği için işlem yapılamadı.");

            try
            {
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int satirD = 0;
                int sutunD = 0;
                string baslik = "MAHSUP BELGESİ ÜRETİLEN BELGELER LİSTESİ";
                string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();

                XLS.DosyaAc(SablonYol("MUH-KapatmaListe.XLT"), sonucDosyaAd);
                XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
                satir = satirD + 2;

                XLS.HucreAdBulYaz("txtBaslik", baslik);
                string yil = DateTime.Now.Year.ToString();
                string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                string kurumAd = GenelIslemlerIstemci.VarsayilanKurumBulAd();
                XLS.HucreAdBulYaz("txtYil", yil);
                XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);

                double toplamTutar = 0;
                for (int i = 0; i < grid1Data.Count; i++)
                {
                    KapatmaSatir s = grid1Data[i];
                    double kTutar = s.kTutar;
                    toplamTutar += kTutar;

                    XLS.HucreKopyala(satirD, 0, satirD, 20, satir, 0);
                    XLS.HucreDegerYaz(satir, 0, (i + 1).ToString());
                    XLS.HucreDegerYaz(satir, 1, s.belgeNo ?? "");
                    XLS.HucreDegerYaz(satir, 2, GunAyYil(s.belgeTarih));
                    if (s.yevmiyeNo > 0)
                    {
                        XLS.HucreDegerYaz(satir, 3, s.yevmiyeNo);
                        XLS.HucreDegerYaz(satir, 4, GunAyYil(s.yevmiyeTarih));
                    }
                    XLS.HucreDegerYaz(satir, 5, s.hesapKod ?? "");
                    XLS.HucreDegerYaz(satir, 6, kTutar);
                    // Eski kod muhasebe / birim kodunu adıyla birleştirirdi (muhasebeAd zaten "kod - ad" biçimindedir).
                    XLS.HucreDegerYaz(satir, 7, s.muhasebeKod + "-" + s.muhasebeAd);
                    XLS.HucreDegerYaz(satir, 8, s.birimKod + "-" + s.birimAd);
                    XLS.HucreDegerYaz(satir, 9, s.altHesapKod ?? "");
                    XLS.HucreDegerYaz(satir, 10, s.ilgiliAd ?? "");
                    XLS.HucreDegerYaz(satir, 11, s.cekNo ?? "");
                    XLS.HucreDegerYaz(satir, 12, s.istatistik1Ad ?? "");
                    XLS.HucreDegerYaz(satir, 13, s.istatistik2Ad ?? "");
                    XLS.HucreDegerYaz(satir, 14, s.borc);
                    XLS.HucreDegerYaz(satir, 15, s.alacak);
                    XLS.HucreDegerYaz(satir, 16, s.tertip ?? "");
                    satir++;
                }
                XLS.HucreKopyala(satirD + 1, 0, satirD + 1, 20, satir, 0);
                XLS.HucreDegerYaz(satir, 6, toplamTutar);

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(sonucDosyaAd, "Liste", true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ banka listesi (eski btnBankaListesi_Click)

        /// <summary>
        /// Eski kod EkRtf ile MUH\BankaListesi.rtf (Ulusal Ajans'ta BankaListesiUA.rtf) şablonunu kullanırdı. Proje kararı gereği rtf yerine
        /// aynı alanlarla hazırlanmış MUH\MUH-KapatmaBankaListesi.dotx (Ulusal Ajans'ta varsa MUH-KapatmaBankaListesiUA.dotx) Aspose.Words ile doldurulur.
        /// Alanlar: Liste bölgesi (SiraNo, BelgeNo, KurumIBAN, HesapSahibi, VergiNo, BankaAdi, IBAN, Tutar, ParaBirimi) ve
        /// Hazirlayan, Tarih, KurumKodu, KurumAdi, Yil.
        /// </summary>
        private static async Task<IResult> BankaListesi(HttpContext baglam, IWebHostEnvironment ortam, KapatmaSeciliIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<KapatmaSatir> grid1Data = istek?.satirlar ?? new List<KapatmaSatir>();
            if (grid1Data.Count == 0) return Hata("Listeden herhangi bir belge seçilmediği için işlem yapılamadı.");

            try
            {
                string yil = DateTime.Now.Year.ToString();
                string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                string kurumAd = GenelIslemlerIstemci.VarsayilanKurumBulAd();

                string sablon = SablonYol("MUH-KapatmaBankaListesi.dotx");
                if (TNS.HRC.OrtakServis.UlusalAjansMi())
                {
                    string ua = SablonYol("MUH-KapatmaBankaListesiUA.dotx");
                    if (File.Exists(ua)) sablon = ua;
                }
                if (!File.Exists(sablon)) return Hata("Banka listesi şablonu (MUH-KapatmaBankaListesi.dotx) bulunamadı.");

                DataTable dtUye = new DataTable("Liste");
                dtUye.Columns.Add("SiraNo");
                dtUye.Columns.Add("BelgeNo");
                dtUye.Columns.Add("KurumIBAN");
                dtUye.Columns.Add("HesapSahibi");
                dtUye.Columns.Add("VergiNo");
                dtUye.Columns.Add("BankaAdi");
                dtUye.Columns.Add("IBAN");
                dtUye.Columns.Add("Tutar");
                dtUye.Columns.Add("ParaBirimi");

                CultureInfo tr = new CultureInfo("tr-TR");
                for (int i = 0; i < grid1Data.Count; i++)
                {
                    KapatmaSatir s = grid1Data[i];
                    string paraBirimi = s.paraBirimi ?? "";
                    if (paraBirimi == "")
                        paraBirimi = "TL";

                    string kurumIBAN = "";
                    if (paraBirimi == "TL")
                        kurumIBAN = TNS.MUH.Arac.MuhasebeKurumIBANTL();
                    else if (paraBirimi == "EURO")
                        kurumIBAN = TNS.MUH.Arac.MuhasebeKurumIBANEURO();

                    dtUye.Rows.Add(i + 1, s.belgeNo ?? "", kurumIBAN, s.ilgiliAd ?? "", s.ilgiliVNo ?? "", s.ilgiliBanka ?? "", s.ilgiliIBAN ?? "",
                        s.kTutar.ToString("#,###.00", tr), paraBirimi);
                }

                AsposeLisans.Yukle();
                Aspose.Words.Document doc = new Aspose.Words.Document(sablon);
                doc.MailMerge.CleanupOptions = Aspose.Words.Reporting.MailMergeCleanupOptions.RemoveUnusedRegions;
                doc.MailMerge.Execute(
                    new[] { "Hazirlayan", "Tarih", "KurumKodu", "KurumAdi", "Yil" },
                    new object[] { kullanan.ad + " " + kullanan.soyad, DateTime.Now.ToString("dd.MM.yyyy"), kurum, kurumAd, yil });
                doc.MailMerge.ExecuteWithRegions(dtUye);

                string sonucDosyaad = DosyaIslem.DosyaAdUret("", ".docx");
                doc.Save(sonucDosyaad, Aspose.Words.SaveFormat.Docx);
                return DosyaIslem.DosyaGonder(sonucDosyaad, "BankaListesi", true, "docx");
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Banka listesi hazırlanamadı."));
            }
        }

        // ------------------------------------------------------------------ ödeme hesabı listesi (eski ListeOdemeHesabi.aspx)

        private static IResult OdemeHesap(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                if (Temiz(GenelIslemlerIstemci.VarsayilanKurumBul()) == "") return Hata("Kurum bilgisi alınamadı.");

                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = OrtakFonksiyonlar.ConvertToInt(Q(baglam, "yil"), 0);
                h.nitelik = ((int)ENUMHesapKodNitelik.KASAHESABI).ToString();
                h.seviye = -1;
                string ara = Q(baglam, "ara");
                if (ara != "")
                    h.aciklama = "%" + ara + "%";
                h.hesapKod = Q(baglam, "hesap");
                if (h.hesapKod.StartsWith("63"))
                    h.nitelik = "";

                ObjectArray hVAN = Servis().HesapPlaniListele(kullanan, h);
                if (!hVAN.sonuc.islemSonuc) return Hata(HataMetni(hVAN.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (HesapPlaniSatir hs in hVAN.objeler)
                    satirlar.Add(new { kod = hs.hesapKod, ad = hs.aciklama ?? "" });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Liste okunamadı."));
            }
        }

        // ------------------------------------------------------------------ kişi / firma (eski HarcamaSurecleri/ListeFirmaKisi.aspx; Sgb.firmaSec)

        private static IResult FirmaAra(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string ad, string vergiNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            ad = Temiz(ad);
            vergiNo = Temiz(vergiNo);
            if (ad == "" && vergiNo == "") return Hata("Aramak için ad ya da kimlik / vergi no yazınız.");
            try
            {
                TNS.HRC.FirmaBilgisi h = new TNS.HRC.FirmaBilgisi();
                h.muhasebe = muhasebe ?? 0;
                h.ad = ad;
                h.vergiNo = vergiNo;
                h.aktifDurum = 1;
                ObjectArray bilgi = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, h);
                if (bilgi == null || !bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (TNS.HRC.FirmaBilgisi f in bilgi.objeler)
                {
                    satirlar.Add(new { kod = f.kod, vergiNo = f.vergiNo ?? "", ad = f.ad ?? "", vergiDairesi = f.vergiDairesi ?? "", adres = f.adres ?? "" });
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }

        // ------------------------------------------------------------------ hesap kodu (eski HesapKod.ascx; Sgb.hesapKodu / Sgb.hesapPlaniSec)
        // Ödeme Emri seçim uçlarının (OdemeEmriSecimUclari.HesapAgac / HesapAra / HesapAd) aynısı; yetki bu sayfadan.

        private static int YilVeyaBuYil(int? yil)
        {
            int y = yil ?? 0;
            return y <= 2000 ? DateTime.Now.Year : y;
        }

        private static object HesapDugum(HesapPlaniSatir h)
        {
            return new { kod = h.hesapKod, ad = h.hesapKod + " - " + h.aciklama, hesapAd = h.aciklama ?? "", altVar = !h.detay, detay = h.detay };
        }

        private static IResult HesapAgac(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                int y = YilVeyaBuYil(yil);
                ust = Temiz(ust);
                List<object> dugumler = new List<object>();
                if (ust == "")
                {
                    TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                    for (int i = 1; i < 10; i++)
                    {
                        string ad = servisUZY.UzayDegeriStr(new Kullanici(), "MUHHESAPPLANI", y.ToString() + "-" + i.ToString(), true);
                        dugumler.Add(new { kod = i.ToString(), ad = i.ToString() + "-" + ad, hesapAd = ad ?? "", altVar = true, detay = false });
                    }
                    return Results.Json(new { dugumler = dugumler });
                }

                string[] hesapKirilimlar = ust.Split('.');
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = y;
                hs.hesapKod = ust;
                if (ust.Length < 3) hs.seviye = 1;
                else hs.seviye = hesapKirilimlar.Length + 1;

                IMUHServis servisMUH = Servis();
                ObjectArray hv = new ObjectArray();
                for (int i = 0; i < 10; i++)
                {
                    hv = servisMUH.HesapPlaniListele(new Kullanici(), hs);
                    if (hv.objeler.Count == 0) hs.seviye++;
                    else break;
                }
                foreach (HesapPlaniSatir h in hv.objeler) dugumler.Add(HesapDugum(h));
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Hesap planı okunamadı.");
            }
        }

        private static IResult HesapAra(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string metin, string sonKirilim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            metin = Temiz(metin);
            if (metin.Length < 2) return Hata("En az iki karakter yazınız.");
            try
            {
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = YilVeyaBuYil(yil);
                h.hesapKodHesapAd = metin;
                h.detay = sonKirilim != "0";
                ObjectArray hesap = Servis().HesapPlaniListele(kullanan, h);
                if (hesap == null || !hesap.sonuc.islemSonuc) return Hata(HataMetni(hesap?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (HesapPlaniSatir d in hesap.objeler)
                {
                    satirlar.Add(HesapDugum(d));
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }

        private static IResult HesapAd(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            kod = Temiz(kod);
            if (kod == "") return Results.Json(new { ad = "", detay = false });
            try
            {
                int y = YilVeyaBuYil(yil);
                string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(y, "/KODYAPI/KOD8/KODYAPI");
                if (!string.IsNullOrEmpty(hesYapi)) kod = OrtakFonksiyonlar.KodNoktaKoy(hesYapi, kod.Replace(".", ""));
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = y;
                h.hesapKod = kod;
                ObjectArray hv = Servis().HesapPlaniListele(kullanan, h);
                if (hv != null && hv.objeler != null)
                    foreach (HesapPlaniSatir d in hv.objeler)
                        if (d.hesapKod == kod) return Results.Json(new { kod = d.hesapKod, ad = d.aciklama ?? "", detay = d.detay });
                return Results.Json(new { kod = kod, ad = "", detay = false });
            }
            catch (Exception)
            {
                return Hata("Hesap kodu okunamadı.");
            }
        }
    }
}
