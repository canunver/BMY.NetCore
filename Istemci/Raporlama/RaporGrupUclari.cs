using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
using TNS.Raporlama;

namespace Istemci.Raporlama
{
    public class RaporDosyaIstegi
    {
        public string RaporGrup { get; set; }
        public int Grup { get; set; }
        public int No { get; set; }
        public string Ad { get; set; }
    }

    /// <summary>Kıstas ekranının bir alanı (sayfada dinamik olarak çizilir).</summary>
    public class KistasAlani
    {
        public string ad { get; set; }
        public string etiket { get; set; }
        /// <summary>secim | metin | tarih | onay</summary>
        public string tur { get; set; }
        public List<KistasSecenek> secenekler { get; set; }
        public string deger { get; set; }
        public bool gizli { get; set; }
        public bool zorunlu { get; set; }
        /// <summary>Seçenekleri yıl alanına bağlı (BIRIM, TEKLIFYIL): yıl değişince /api/raporgrup/bagli ile yenilenir.</summary>
        public bool yilaBagli { get; set; }
        /// <summary>Değişince bağlı alanları yenileyen yıl alanı.</summary>
        public bool yilAlani { get; set; }

        internal KistasEkranAlan kaynak;
    }

    public class KistasSecenek
    {
        public string kod { get; set; }
        public string ad { get; set; }
    }

    /// <summary>
    /// Genel rapor ekranının (eski Raporlama/RaporGrupListe.aspx + RaporKistasSayfa.aspx + RaporDosya.aspx + RaporGosterPDF.aspx) uçları.
    /// Rapor grupları, kıstas alanları ve raporlar Rapor.xml'den gelir (TNS.Raporlama.RaporlamaServis).
    ///   GET  /api/raporgrup/liste?raporGrup=          : gruptaki raporlar (raporGrup "2002" ya da eskisi gibi "2002-2003")
    ///   GET  /api/raporgrup/kistas?raporGrup=&amp;grup=&amp;no= : seçilen raporun kıstas alanları (kullanıcının son girdiği değerlerle)
    ///   GET  /api/raporgrup/bagli?...&amp;yil=             : yıla bağlı alanların (birim, teklif yılı) seçenekleri
    ///   GET  /api/raporgrup/al?...&amp;k_Alan=deger        : raporu üretir, rapor klasörüne saklar ve indirir
    ///   GET  /api/raporgrup/dosyalar?...               : kullanıcının son 3 raporu (geçici) ve kütüphanedeki (kalıcı) raporlar
    ///   GET  /api/raporgrup/dosya?...&amp;ad=&amp;kutuphane=&amp;pdf= : saklanan raporu indirir (pdf=1: PDF'e çevrilmiş olarak)
    ///   POST /api/raporgrup/kutuphane-ekle / kutuphane-sil
    /// Yetki: ekran menüde "../Raporlama/RaporGrupListe.aspx?raporGrup=..." adresiyle bulunur; kullanıcı o menü seçeneğini görebiliyor
    /// olmalı ve istenen grup adresteki gruplardan biri olmalıdır.
    /// Rapor dosyaları appSettings RaporDosyaYol\grup\no altında kullanici_grup_no_anahtar_tarih.uzanti adıyla durur; kütüphane
    /// \kutuphane alt klasörüdür (herkes görür, yalnızca ekleyen siler). Kullanıcının geçici raporlarından en yeni 3'ü kalır.
    /// </summary>
    public static class RaporGrupUclari
    {
        private const int GeciciSayi = 3;
        private static readonly string[] RaporUzantilari = { ".xls", ".xlsx", ".xlsm", ".doc", ".docx", ".pdf", ".zip", ".html", ".htm" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/raporgrup/liste", Liste);
            app.MapGet("/api/raporgrup/kistas", Kistas);
            app.MapGet("/api/raporgrup/bagli", Bagli);
            app.MapGet("/api/raporgrup/al", Al);
            app.MapGet("/api/raporgrup/dosyalar", Dosyalar);
            app.MapGet("/api/raporgrup/dosya", Dosya);
            app.MapPost("/api/raporgrup/kutuphane-ekle", KutuphaneEkle);
            app.MapPost("/api/raporgrup/kutuphane-sil", KutuphaneSil);
        }

        // ---------------------------------------------------------------- yetki

        /// <summary>Adresteki raporGrup değeri: sayılar ve aralarında "-" (eski sayfa birden çok grubu "-" ile alıyordu).</summary>
        public static int[] Gruplar(string raporGrup)
        {
            List<int> liste = new List<int>();
            foreach (string s in (raporGrup ?? "").Split('-'))
            {
                if (!int.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, out int g)) return new int[0];
                liste.Add(g);
            }
            return liste.ToArray();
        }

        /// <summary>Kullanıcı bu rapor ekranını (menüdeki adresiyle) kullanabilir mi?</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, string raporGrup)
        {
            if (Gruplar(raporGrup).Length == 0) return false;
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, "../Raporlama/RaporGrupListe.aspx?raporGrup=" + raporGrup);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, string raporGrup, int? grup, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, raporGrup)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (grup.HasValue && Array.IndexOf(Gruplar(raporGrup), grup.Value) < 0) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata });
        }

        private static string HataMetni(Exception ex, string varsayilan)
        {
            string hata = (ex.Message ?? "").Trim();
            if (ex is RaporException || ex is ButceHazirlikv2.RaporHatasi || ex is TNS.TNSIslemHatasi)
                return hata == "" ? varsayilan : hata;
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static RaporGrup GrupBul(int grup, out string hata)
        {
            RaporGrupListesi rgl = RaporlamaServis.RaporListesiAl();
            RaporGrup rg = rgl.RaporGrup(grup);
            hata = rg != null ? null : !string.IsNullOrEmpty(rgl.hata) ? "Rapor tanımları okunamadı: " + rgl.hata : grup + " numaralı rapor grubu bulunamadı.";
            return rg;
        }

        // ---------------------------------------------------------------- rapor listesi

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string raporGrup)
        {
            IResult ret = Denetle(baglam, ortam, raporGrup, null, out Kullanici kullanan);
            if (ret != null) return ret;

            List<string> basliklar = new List<string>();
            List<object> raporlar = new List<object>();
            List<string> hatalar = new List<string>();
            foreach (int g in Gruplar(raporGrup))
            {
                RaporGrup rg = GrupBul(g, out string hata);
                if (rg == null)
                {
                    hatalar.Add(hata);
                    continue;
                }
                basliklar.Add(rg.RaporGrupAd());
                for (int i = 0; i < rg.RaporSayisi(); i++)
                {
                    TNS.Raporlama.Rapor r = rg[i];
                    raporlar.Add(new
                    {
                        kod = g + "-" + r.raporNo,
                        grup = g,
                        no = r.raporNo,
                        ad = rg.RaporAd(r.raporNo),
                        tasindi = r.raporTip == RAPORTIP.EXCEL || RaporlamaServis.NesneVar(r.nesneAd)
                    });
                }
            }
            return Results.Json(new { baslik = string.Join(" - ", basliklar), raporlar = raporlar, hata = hatalar.Count > 0 ? string.Join(" ", hatalar) : null });
        }

        // ---------------------------------------------------------------- kıstas alanları

        private static IResult Kistas(HttpContext baglam, IWebHostEnvironment ortam, string raporGrup, int grup, int no)
        {
            IResult ret = Denetle(baglam, ortam, raporGrup, grup, out Kullanici kullanan);
            if (ret != null) return ret;
            RaporGrup rg = GrupBul(grup, out string hata);
            if (rg == null) return Hata(hata);
            TNS.Raporlama.Rapor r = rg.Rapor(no);
            if (r == null) return Hata("Rapor bulunamadı.");
            try
            {
                List<KistasAlani> alanlar = Alanlar(kullanan, rg, no, null, true);
                string uyari = null;
                if (!rg.YetkisiVarMi(kullanan) || !r.YetkisiVarMi(kullanan)) uyari = "Bu raporları alma yetkiniz yok!";
                else if (r.raporTip == RAPORTIP.METOD && !RaporlamaServis.NesneVar(r.nesneAd)) uyari = "Bu rapor henüz yeni sisteme taşınmadı.";
                return Results.Json(new { raporAd = rg.RaporAd(no), alanlar = alanlar, uyari = uyari });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex, "Rapor kıstasları okunamadı."));
            }
        }

        private static IResult Bagli(HttpContext baglam, IWebHostEnvironment ortam, string raporGrup, int grup, int no, int yil)
        {
            IResult ret = Denetle(baglam, ortam, raporGrup, grup, out Kullanici kullanan);
            if (ret != null) return ret;
            RaporGrup rg = GrupBul(grup, out string hata);
            if (rg == null) return Hata(hata);
            try
            {
                List<KistasAlani> alanlar = Alanlar(kullanan, rg, no, yil, true).Where(a => a.yilaBagli).ToList();
                return Results.Json(new { alanlar = alanlar });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex, "Seçenekler okunamadı."));
            }
        }

        /// <summary>
        /// Eski RaporKistasSayfa.KontrolleriOlustur + Page_Load: alanları ve seçeneklerini hazırlar. yil verilmezse yıl alanının
        /// seçili değeri kullanılır. kayitliDeger: kullanıcının son girdiği değer (kullanıcı değişkeni RAPOR_alan) seçili gelsin mi.
        /// Gizli alanlara kayıtlı değer uygulanmaz (eskiden başka gruptaki aynı adlı alanın değeri gizli alana geçebiliyordu).
        /// </summary>
        private static List<KistasAlani> Alanlar(Kullanici kullanan, RaporGrup rg, int raporNo, int? yil, bool kayitliDeger)
        {
            List<KistasAlani> alanlar = new List<KistasAlani>();
            int? secilenYil = yil;
            KistasEkran ke = rg.kistasEkran;
            for (int i = 0; i < ke.AlanSayisi(); i++)
            {
                KistasEkranAlan kea = ke.Alan(i);
                KistasAlani a = new KistasAlani
                {
                    ad = kea.degiskenAd,
                    etiket = kea.Etiket(rg.grupNo),
                    gizli = kea.GizliMi(raporNo),
                    zorunlu = kea.zorunlu,
                    tur = "secim",
                    deger = "",
                    secenekler = new List<KistasSecenek>(),
                    kaynak = kea
                };
                string kayitli = kayitliDeger && !a.gizli ? GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "RAPOR_" + kea.degiskenAd) : "";
                int simdi = DateTime.Now.Year;

                switch (kea.degiskenTip)
                {
                    case DEGISKENTIP.YIL:
                        {
                            int ilk = OrtakFonksiyonlar.ConvertToInt(kea.ilkDeger, 0);
                            // Eski YilDoldur: 2005..bu yıl (hazırlık yılı istenen alanlarda, EKBILGI dolu, gelecek yıl da listede); seçili bu yıl.
                            // ILKDEGER verilmişse 2002..bu yıl, seçili ILKDEGER.
                            int bas = ilk > 0 ? 2002 : 2005;
                            int son = ilk <= 0 && !string.IsNullOrEmpty(kea.ekBilgi) ? simdi + 1 : simdi;
                            for (int y = son; y >= bas; y--) Ekle(a, y.ToString(), y.ToString());
                            a.deger = ilk > 0 ? ilk.ToString() : simdi.ToString();
                            a.yilAlani = true;
                            break;
                        }
                    case DEGISKENTIP.TEKLIFYIL:
                        a.yilaBagli = true;
                        break;
                    case DEGISKENTIP.AY:
                        for (int ay = 1; ay <= 12; ay++) Ekle(a, ay.ToString(), GenelIslemler.AyAdDondur(ay));
                        a.deger = OrtakFonksiyonlar.ConvertToInt(kea.ilkDeger, 0) > 0 ? kea.ilkDeger : "1";
                        break;
                    case DEGISKENTIP.ASAMA:
                        // Eski GenelIslemlerIstemci.AsamaDoldur
                        string[] asamalar = { "Birim Teklifi", "SGB Görüşü", "Bakanlık Teklifi", "Tasarı", "Plan Bütçe Komisyonu", "Kanun", "KBÖ" };
                        for (int k = 0; k < asamalar.Length; k++) Ekle(a, k.ToString(), asamalar[k]);
                        a.deger = "0";
                        break;
                    case DEGISKENTIP.ASAMAUYGULAMA:
                        // Eskisinde de doldurulmuyordu (AsamaUygulamaDoldur yorum satırı).
                        break;
                    case DEGISKENTIP.SENARYO:
                        {
                            TNS.BHMv2.ButceSenaryo[] bsler = TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, new TNS.BHMv2.ButceSenaryo());
                            if (bsler != null)
                                foreach (TNS.BHMv2.ButceSenaryo bs in bsler) Ekle(a, bs.kod, bs.kod + "-" + bs.ad);
                            a.deger = kea.ilkDeger;
                            break;
                        }
                    case DEGISKENTIP.COMBO:
                        foreach (string s in kea.liste.Split(';'))
                            if (!string.IsNullOrEmpty(s)) Ekle(a, s, s);
                        a.deger = kea.ilkDeger;
                        break;
                    case DEGISKENTIP.COMBO_KODAD:
                        {
                            string[] sa = kea.liste.Split(';');
                            for (int k = 0; k < sa.Length; k++)
                            {
                                if (string.IsNullOrEmpty(sa[k])) continue;
                                string[] kodAd = sa[k].Split('-');
                                if (kodAd.Length == 2) Ekle(a, kodAd[0], kodAd[1]);
                                else Ekle(a, k.ToString(), sa[k]);
                            }
                            a.deger = kea.ilkDeger;
                            break;
                        }
                    case DEGISKENTIP.COMBO_DINAMIK:
                        {
                            Ekle(a, "", "Hepsi");
                            YetkiTipi yt = TNS.KYM.Arac.YetkiTipiBul(kea.liste);
                            if (yt == null)
                            {
                                int ytKod = OrtakFonksiyonlar.ConvertToInt(kea.liste, -1);
                                if (ytKod > 0)
                                {
                                    try { yt = TNS.KYM.Arac.YetkiTipiBul(ytKod); } catch (Exception) { yt = null; }
                                }
                            }
                            if (yt != null)
                            {
                                ListeSatir[] bListe = TNS.KYM.Arac.Tanimla().YetkiTipiGirisListesi(kullanan, yt, simdi, "");
                                if (bListe != null)
                                    foreach (ListeSatir ls in bListe) Ekle(a, ls.kod, ls.ad);
                            }
                            a.deger = kea.ilkDeger;
                            break;
                        }
                    case DEGISKENTIP.YAZI:
                        a.tur = "metin";
                        a.deger = kea.ilkDeger;
                        break;
                    case DEGISKENTIP.YAZIKURUMSAL:
                        a.tur = "metin";
                        a.deger = GenelIslemlerIstemci.VarsayilanKurumBul() ?? "";
                        break;
                    case DEGISKENTIP.TARIH:
                        a.tur = "tarih";
                        break;
                    case DEGISKENTIP.ILKTARIH:
                        a.tur = "tarih";
                        a.deger = "01.01." + simdi;
                        break;
                    case DEGISKENTIP.SONTARIH:
                        a.tur = "tarih";
                        a.deger = DateTime.Now.ToString("dd.MM.yyyy");
                        break;
                    case DEGISKENTIP.KURUM:
                        Ekle(a, GenelIslemlerIstemci.VarsayilanKurumBul(), GenelIslemlerIstemci.VarsayilanKurumBulAd());
                        break;
                    case DEGISKENTIP.BIRIM:
                        a.yilaBagli = true;
                        break;
                    case DEGISKENTIP.CHECKBOX:
                        a.tur = "onay";
                        a.deger = OrtakFonksiyonlar.ConvertToInt(kea.ilkDeger, 0) > 0 ? "1" : "0";
                        break;
                }

                // Kullanıcının son girdiği değer (seçimlerde yalnızca listede varsa). Onay kutusu eskisi gibi hatırlanmaz.
                if (!string.IsNullOrEmpty(kayitli) && a.tur != "onay" && !a.yilaBagli)
                {
                    if (a.tur != "secim" || a.secenekler.Any(s => s.kod == kayitli)) a.deger = kayitli;
                }
                if (a.tur == "secim" && !a.yilaBagli && a.secenekler.Count > 0 && !a.secenekler.Any(s => s.kod == a.deger))
                    a.deger = a.secenekler[0].kod;
                if (a.yilAlani && !secilenYil.HasValue)
                    secilenYil = OrtakFonksiyonlar.ConvertToInt(a.deger, simdi);
                alanlar.Add(a);
            }

            // Yıla bağlı alanlar (eski ddlYil_SelectedIndexChanged)
            int bagliYil = secilenYil ?? DateTime.Now.Year;
            foreach (KistasAlani a in alanlar.Where(x => x.yilaBagli))
            {
                if (a.kaynak.degiskenTip == DEGISKENTIP.TEKLIFYIL)
                {
                    for (int y = bagliYil; y <= bagliYil + 2; y++) Ekle(a, y.ToString(), y.ToString());
                }
                else
                {
                    string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                    ABSKodlarVAN br = TNS.BKD.Arac.Tanimla().BirimListele(kullanan, bagliYil, kurum, true);
                    if (!br.sonuc.islemSonuc) throw new RaporException(br.sonuc.hataStr);
                    Ekle(a, kurum, GenelIslemlerIstemci.VarsayilanKurumBulAd());
                    Ekle(a, "00", "Tüm Birimlerin Listesi (Bazı raporlarda)");
                    foreach (ABSKod ak in br.absKodlar)
                        Ekle(a, ak.kod, ak.kod + " - " + ak.Ad(kullanan.aktifKultur));
                }
                string kayitli = kayitliDeger && !a.gizli ? GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "RAPOR_" + a.ad) : "";
                a.deger = a.secenekler.Any(s => s.kod == kayitli) ? kayitli : a.secenekler.Count > 0 ? a.secenekler[0].kod : "";
            }
            return alanlar;
        }

        private static void Ekle(KistasAlani a, string kod, string ad)
        {
            a.secenekler.Add(new KistasSecenek { kod = kod ?? "", ad = ad ?? "" });
        }

        // ---------------------------------------------------------------- rapor al

        /// <summary>
        /// Eski RaporKistasSayfa.btnRaporHazirla_Click: kıstas değerlerini toplar (k_Alan parametreleri), kullanıcının son
        /// değerlerini saklar, raporu rapor klasörüne üretir ve indirir.
        /// </summary>
        private static IResult Al(HttpContext baglam, IWebHostEnvironment ortam, string raporGrup, int grup, int no)
        {
            IResult ret = Denetle(baglam, ortam, raporGrup, grup, out Kullanici kullanan);
            if (ret != null) return ret;
            RaporGrup rg = GrupBul(grup, out string hata);
            if (rg == null) return Hata(hata);
            TNS.Raporlama.Rapor r = rg.Rapor(no);
            if (r == null) return Hata("Rapor bulunamadı.");

            try
            {
                IQueryCollection sorgu = baglam.Request.Query;
                // Yıla bağlı seçenekler, gelen yıl değerine göre hazırlanır (seçilen değerin listede olduğu denetlenir).
                int? yil = null;
                for (int i = 0; i < rg.kistasEkran.AlanSayisi(); i++)
                {
                    KistasEkranAlan kea = rg.kistasEkran.Alan(i);
                    if (kea.degiskenTip == DEGISKENTIP.YIL && !kea.GizliMi(no))
                    {
                        yil = OrtakFonksiyonlar.ConvertToInt(sorgu["k_" + kea.degiskenAd].ToString(), DateTime.Now.Year);
                        break;
                    }
                }
                List<KistasAlani> alanlar = Alanlar(kullanan, rg, no, yil, false);

                Hashtable ht = new Hashtable();
                string anahtar = "";
                foreach (KistasAlani a in alanlar)
                {
                    KistasEkranAlan kea = a.kaynak;
                    // Gizli alanlar tanımdaki değeriyle gider; görünenler sayfadan.
                    string deger = a.gizli ? a.deger : (sorgu["k_" + a.ad].ToString() ?? "").Trim();
                    KistasSecenek secilen = null;
                    if (a.tur == "secim")
                    {
                        secilen = a.secenekler.FirstOrDefault(s => s.kod == deger);
                        if (secilen == null)
                        {
                            if (a.secenekler.Count == 0) continue; // eski: seçili öğe yoksa alan gönderilmezdi
                            return Hata("'" + a.etiket + "' alanında geçersiz seçim.");
                        }
                    }
                    else if (a.tur == "onay")
                        deger = deger == "1" ? "1" : "0";

                    ht[a.ad] = deger;
                    if (secilen != null && !string.IsNullOrEmpty(kea.degiskenAciklama))
                    {
                        string aciklama = secilen.ad;
                        if (kea.degiskenTip == DEGISKENTIP.KURUM || kea.degiskenTip == DEGISKENTIP.BIRIM)
                        {
                            int tire = aciklama.IndexOf(" - ", StringComparison.Ordinal);
                            if (tire >= 0) aciklama = aciklama.Remove(0, tire + 3);
                        }
                        ht[kea.degiskenAciklama] = aciklama;
                    }
                    if (kea.zorunlu && !(a.tur == "tarih"))
                        anahtar = anahtar == "" ? deger : anahtar + "_" + deger;
                    if (!a.gizli)
                        GenelIslemler.KullaniciDegiskenSakla(kullanan, "RAPOR_" + a.ad, deger);
                }
                if (ht["TeklifYil"] + "" != "" && ht["Asama"] + "" != "" && ht["ButceYil"] + "" != "")
                {
                    int by = OrtakFonksiyonlar.ConvertToInt(ht["ButceYil"], 0);
                    int ty = OrtakFonksiyonlar.ConvertToInt(ht["TeklifYil"], 0);
                    ht["Asama"] = (OrtakFonksiyonlar.ConvertToInt(ht["Asama"], 0) + (ty - by) * 10).ToString();
                }

                string dosyaAd = RaporlamaServis.RaporDosyaAdUret(grup, no, DosyaAdTemizle(anahtar), kullanan.kullaniciKodu)
                    + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss", CultureInfo.InvariantCulture);
                string dosyaYol = RaporlamaServis.RaporDosyaYolUret(grup, no, dosyaAd, false);

                string uretilen = rg.RaporAl(kullanan, TNS.UZY.Arac.Tanimla(), r, ht, dosyaYol);
                if (RaporlamaServis.MesajMi(uretilen)) return Hata(uretilen);
                if (!File.Exists(uretilen)) return Hata(string.Format(RaporlamaServis.str15, rg.RaporAd(no)));

                // Rapor sınıfı dosyayı başka yere (geçici klasöre) yazdıysa rapor klasörüne taşınır; böylece listede görünür.
                string klasor = RaporlamaServis.RaporDosyaYolUret(grup, no, "", false);
                if (!string.Equals(Path.GetFullPath(Path.GetDirectoryName(uretilen)), Path.GetFullPath(klasor), StringComparison.OrdinalIgnoreCase))
                {
                    string hedef = Path.Combine(klasor, dosyaAd + Path.GetExtension(uretilen));
                    File.Move(uretilen, hedef, true);
                    uretilen = hedef;
                }
                GeciciTemizle(kullanan, grup, no);
                return DosyaIslem.DosyaGonder(uretilen, Path.GetFileName(uretilen), false, Path.GetExtension(uretilen));
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex, "Rapor hazırlanamadı."));
            }
        }

        private static string DosyaAdTemizle(string ad)
        {
            char[] gecersiz = Path.GetInvalidFileNameChars();
            return new string((ad ?? "").Select(c => Array.IndexOf(gecersiz, c) >= 0 ? '-' : c).ToArray());
        }

        // ---------------------------------------------------------------- saklanan raporlar

        private static string OnEk(int grup, int no)
        {
            return "_" + grup + "_" + no + "_";
        }

        private static bool RaporDosyasi(FileInfo f)
        {
            string ext = f.Extension.ToLowerInvariant();
            return RaporUzantilari.Contains(ext);
        }

        private static List<FileInfo> GeciciDosyalar(Kullanici kullanan, int grup, int no)
        {
            DirectoryInfo klasor = new DirectoryInfo(RaporlamaServis.RaporDosyaYolUret(grup, no, "", false));
            return klasor.GetFiles(kullanan.kullaniciKodu + OnEk(grup, no) + "*.*").Where(RaporDosyasi)
                .OrderByDescending(f => f.CreationTime).ToList();
        }

        /// <summary>Eski RaporDosya.DosyalariVer: kullanıcının 3'ten eski geçici raporları silinir.</summary>
        private static void GeciciTemizle(Kullanici kullanan, int grup, int no)
        {
            foreach (FileInfo f in GeciciDosyalar(kullanan, grup, no).Skip(GeciciSayi))
            {
                try { f.Delete(); } catch (Exception) { }
            }
        }

        private static object DosyaBilgi(FileInfo f, int grup, int no, Kullanici kullanan, bool kutuphane)
        {
            // ad: kullanici_grup_no_anahtar_tarih.uzanti
            string ad = Path.GetFileNameWithoutExtension(f.Name);
            string onEk = OnEk(grup, no);
            int yer = ad.IndexOf(onEk, StringComparison.Ordinal);
            string sahip = yer > 0 ? ad.Substring(0, yer) : "";
            string kistas = yer >= 0 ? ad.Substring(yer + onEk.Length) : ad;
            int son = kistas.LastIndexOf('_');
            if (son >= 0) kistas = kistas.Substring(0, son);
            string ext = f.Extension.TrimStart('.').ToLowerInvariant();
            string tur = ext.StartsWith("xls") ? "Excel" : ext.StartsWith("doc") ? "Word" : ext == "pdf" ? "PDF" : ext.ToUpperInvariant();
            return new
            {
                ad = f.Name,
                tarih = f.CreationTime.ToString("dd.MM.yyyy HH:mm:ss"),
                tur = tur,
                kistas = kistas.Replace('_', ' '),
                sahip = sahip,
                silinebilir = kutuphane && f.Name.StartsWith(kullanan.kullaniciKodu + "_", StringComparison.Ordinal)
            };
        }

        private static IResult Dosyalar(HttpContext baglam, IWebHostEnvironment ortam, string raporGrup, int grup, int no)
        {
            IResult ret = Denetle(baglam, ortam, raporGrup, grup, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                GeciciTemizle(kullanan, grup, no);
                List<object> gecici = GeciciDosyalar(kullanan, grup, no).Select(f => DosyaBilgi(f, grup, no, kullanan, false)).ToList();
                DirectoryInfo kutKlasor = new DirectoryInfo(RaporlamaServis.RaporDosyaYolUret(grup, no, "", true));
                List<object> kutuphane = kutKlasor.GetFiles("*" + OnEk(grup, no) + "*.*").Where(RaporDosyasi)
                    .OrderByDescending(f => f.CreationTime).Select(f => DosyaBilgi(f, grup, no, kullanan, true)).ToList();
                return Results.Json(new { gecici = gecici, kutuphane = kutuphane });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex, "Rapor dosyaları okunamadı."));
            }
        }

        /// <summary>
        /// İstenen dosyanın tam yolu; ad düz dosya adı olmalı, geçici dosya kullanıcının kendisinin, kütüphane dosyası bu raporun olmalı.
        /// </summary>
        private static string DosyaYolu(Kullanici kullanan, int grup, int no, string ad, bool kutuphane)
        {
            if (string.IsNullOrEmpty(ad) || ad != Path.GetFileName(ad) || ad.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) return null;
            if (kutuphane ? ad.IndexOf(OnEk(grup, no), StringComparison.Ordinal) <= 0 : !ad.StartsWith(kullanan.kullaniciKodu + OnEk(grup, no), StringComparison.Ordinal))
                return null;
            string yol = RaporlamaServis.RaporDosyaYolUret(grup, no, ad, kutuphane);
            return File.Exists(yol) && RaporDosyasi(new FileInfo(yol)) ? yol : null;
        }

        /// <summary>Eski RaporGosterPDF: saklanan raporu olduğu gibi ya da PDF'e çevrilmiş olarak gönderir.</summary>
        private static IResult Dosya(HttpContext baglam, IWebHostEnvironment ortam, string raporGrup, int grup, int no, string ad, int kutuphane, int pdf)
        {
            IResult ret = Denetle(baglam, ortam, raporGrup, grup, out Kullanici kullanan);
            if (ret != null) return ret;
            string yol = DosyaYolu(kullanan, grup, no, ad, kutuphane == 1);
            if (yol == null) return Hata("Dosya bulunamadı.");
            try
            {
                if (pdf == 1 && !yol.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    // PDF geçici klasörde üretilir, gönderildikten sonra silinir (listede ayrı dosya olarak görünmez).
                    string kopya = DosyaIslem.DosyaAdUret("", Path.GetExtension(yol));
                    File.Copy(yol, kopya, true);
                    string pdfYol = RaporlamaServis.Dosya2PDF(kopya, true);
                    return DosyaIslem.DosyaGonder(pdfYol, Path.GetFileNameWithoutExtension(ad), true, "pdf");
                }
                return DosyaIslem.DosyaGonder(yol, ad, false, Path.GetExtension(yol));
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex, "Dosya hazırlanamadı."));
            }
        }

        /// <summary>Eski btnKutuphaneEkle_Click: geçici raporu (aynı adlı bütün dosyalarıyla) kütüphaneye taşır.</summary>
        private static async Task<IResult> KutuphaneEkle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, RaporDosyaIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.RaporGrup, istek.Grup, out Kullanici kullanan);
            if (ret != null) return ret;
            string yol = DosyaYolu(kullanan, istek.Grup, istek.No, istek.Ad, false);
            if (yol == null) return Hata("Dosya bulunamadı.");
            try
            {
                string kok = Path.GetFileNameWithoutExtension(yol);
                foreach (FileInfo f in new DirectoryInfo(Path.GetDirectoryName(yol)).GetFiles(kok + ".*"))
                    f.MoveTo(RaporlamaServis.RaporDosyaYolUret(istek.Grup, istek.No, f.Name, true), true);
                return Results.Json(new { basarili = true });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex, "Rapor kütüphaneye eklenemedi."));
            }
        }

        /// <summary>Eski btnKutuphaneSil_Click: kütüphaneden yalnızca ekleyen kullanıcı silebilir.</summary>
        private static async Task<IResult> KutuphaneSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, RaporDosyaIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.RaporGrup, istek.Grup, out Kullanici kullanan);
            if (ret != null) return ret;
            string yol = DosyaYolu(kullanan, istek.Grup, istek.No, istek.Ad, true);
            if (yol == null) return Hata("Dosya bulunamadı.");
            if (!istek.Ad.StartsWith(kullanan.kullaniciKodu + "_", StringComparison.Ordinal))
                return Hata("Yalnızca kendi eklediğiniz raporu kütüphaneden silebilirsiniz.");
            try
            {
                string kok = Path.GetFileNameWithoutExtension(yol);
                foreach (FileInfo f in new DirectoryInfo(Path.GetDirectoryName(yol)).GetFiles(kok + ".*"))
                    f.Delete();
                return Results.Json(new { basarili = true });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex, "Rapor kütüphaneden silinemedi."));
            }
        }
    }
}
