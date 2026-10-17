using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using OrtakClass;
using TNS.KYM;

namespace TNS.Raporlama
{
    // Eski OrtakClass\Raporlama\RaporlamaServis.cs'ten alındı: Rapor.xml (appSettings RaporlarDosyaAd) okunur; rapor grupları,
    // kıstas (kriter) ekranı alanları ve raporlar buradan gelir. Genel rapor ekranı (Raporlama/RaporGrupListe) ve rapor
    // sınıflarının kendi içinden çağırdığı TekRaporUret bunu kullanır.
    // Değişenler:
    //   - Metinler Istemci.Metin.Ortak (eski OrtakClassRes) ile okunur; kaynakta yoksa XML'deki AD / ETIKET kullanılır.
    //   - Rapor.xml değişince (dosya tarihi) yeniden okunur.
    //   - METOD raporlarında eski Activator.CreateInstance(DLL, NESNE) yerine NESNE (isim alanı + sınıf) yüklü derlemelerde
    //     aranır; bütün modüller artık Istemci.dll'dedir. Sınıfın RaporAl(object, object, string, string, string, Hashtable,
    //     string) metodu çağrılır.
    //   - PDF'e çevirme eski Office / pdf995 yazıcısı yerine Aspose ile yapılır.
    //   - Kurum / aşama görme yetkisi denetimi eskisi gibi her zaman geçer (eski GenelIslemler.GormeYetkisiVarMi hep true
    //     döndürüyordu); sayfa yetkisi ekranın uçlarında menüden denetlenir.
    //   - Birden çok raporu zip'leyerek alma (eski RaporAl(TNSCollection)) ekranda kullanılmadığı için taşınmadı.

    public enum DEGISKENTIP : int
    {
        YIL, AY, ASAMA, KURUM, BIRIM, COMBO, YAZI, YAZIKURUMSAL, TEKLIFYIL, ASAMAUYGULAMA, COMBO_DINAMIK, COMBO_KODAD, TARIH, ILKTARIH, SONTARIH, SENARYO, CHECKBOX, EGITIMTIP
    }

    public enum RAPORTIP : int
    {
        EXCEL, METOD
    }

    public class RaporException : Exception
    {
        public RaporException(string message) : base(message) { }
    }

    public class KistasEkranAlan
    {
        public string degiskenAd;
        public string degiskenAciklama;
        public DEGISKENTIP degiskenTip;
        public bool gizli;
        public string liste;
        public string ilkDeger;
        public bool zorunlu;
        public string ekBilgi;
        public int[] gosterilecekRaporlar;
        /// <summary>XML'deki ETIKET (kaynakta REP_A_grup_degisken yoksa gösterilir).</summary>
        public string etiket;

        public string Etiket(int rgNo)
        {
            string varsayilan = string.IsNullOrEmpty(etiket) ? degiskenAd : etiket;
            return Istemci.Metin.Ortak(RaporlamaServis.KistasAlanResAd(rgNo, degiskenAd), varsayilan);
        }

        public KistasEkranAlan(string p_degiskenAd, string p_degiskenAciklama, string p_degiskenTip, string p_liste, string p_gizli, string p_ilkDeger, string p_zorunlu, string ekBilgi, string gosterilecekRaporlar, string etiket)
        {
            degiskenAd = p_degiskenAd;
            degiskenAciklama = p_degiskenAciklama;
            gizli = !(string.IsNullOrEmpty(p_gizli) || p_gizli == "0");
            liste = p_liste ?? "";
            ilkDeger = p_ilkDeger ?? "";
            this.etiket = etiket;

            switch ((p_degiskenTip ?? "").ToUpperInvariant())
            {
                case "YIL": degiskenTip = DEGISKENTIP.YIL; break;
                case "TEKLIFYIL": degiskenTip = DEGISKENTIP.TEKLIFYIL; break;
                case "ASAMA": degiskenTip = DEGISKENTIP.ASAMA; break;
                case "ASAMAUYGULAMA": degiskenTip = DEGISKENTIP.ASAMAUYGULAMA; break;
                case "AY": degiskenTip = DEGISKENTIP.AY; break;
                case "KURUM": degiskenTip = DEGISKENTIP.KURUM; break;
                case "BIRIM": degiskenTip = DEGISKENTIP.BIRIM; break;
                case "COMBO": degiskenTip = DEGISKENTIP.COMBO; break;
                case "COMBO_KODAD": degiskenTip = DEGISKENTIP.COMBO_KODAD; break;
                case "COMBO_DINAMIK": degiskenTip = DEGISKENTIP.COMBO_DINAMIK; break;
                case "YAZI": degiskenTip = DEGISKENTIP.YAZI; break;
                case "YAZIKURUMSAL": degiskenTip = DEGISKENTIP.YAZIKURUMSAL; break;
                case "TARIH": degiskenTip = DEGISKENTIP.TARIH; break;
                case "ILKTARIH": degiskenTip = DEGISKENTIP.ILKTARIH; break;
                case "SONTARIH": degiskenTip = DEGISKENTIP.SONTARIH; break;
                case "SENARYO": degiskenTip = DEGISKENTIP.SENARYO; break;
                case "CHECKBOX": degiskenTip = DEGISKENTIP.CHECKBOX; break;
                default: throw new RaporException(String.Format(RaporlamaServis.str1, p_degiskenTip));
            }

            zorunlu = !string.IsNullOrEmpty(p_zorunlu);
            if (string.IsNullOrWhiteSpace(gosterilecekRaporlar)) this.gosterilecekRaporlar = null;
            else
            {
                string[] tut = gosterilecekRaporlar.Split(',', ';');
                this.gosterilecekRaporlar = new int[tut.Length];
                for (int i = 0; i < tut.Length; i++)
                    this.gosterilecekRaporlar[i] = OrtakFonksiyonlar.ConvertToInt(tut[i], 0);
            }
            this.ekBilgi = ekBilgi;
        }

        /// <summary>Alan bu raporda gizli mi (eski RaporKistasSayfa.GizliMi)? GOSTERILECEKRAPOR verilmişse yalnız o raporlarda görünür.</summary>
        public bool GizliMi(int raporNo)
        {
            if (gizli) return true;
            if (gosterilecekRaporlar == null || gosterilecekRaporlar.Length == 0) return false;
            return Array.IndexOf(gosterilecekRaporlar, raporNo) < 0;
        }
    }

    public class KistasEkran
    {
        private readonly List<KistasEkranAlan> alanlar = new List<KistasEkranAlan>();
        public string ekBilgi;

        public KistasEkran(string ekBilgi)
        {
            this.ekBilgi = ekBilgi;
        }

        public void Add(KistasEkranAlan kea)
        {
            alanlar.Add(kea);
        }

        public int AlanSayisi()
        {
            return alanlar.Count;
        }

        public KistasEkranAlan Alan(int i)
        {
            return i >= 0 && i < alanlar.Count ? alanlar[i] : null;
        }

        public string ZorunlulukKontrol(Hashtable ht, int rgNo)
        {
            string cevap = "";
            foreach (KistasEkranAlan kea in alanlar)
            {
                if (kea.zorunlu && (ht[kea.degiskenAd] == null || ht[kea.degiskenAd].ToString() == ""))
                {
                    if (cevap != "") cevap += " ";
                    cevap += String.Format(RaporlamaServis.str2, kea.Etiket(rgNo));
                }
            }
            return cevap;
        }
    }

    public class Rapor
    {
        private readonly List<string> yetkiTipIsimleri;
        public RAPORTIP raporTip;
        public string yolsuzSablonAd;
        public string dllAd;
        public string nesneAd;
        public int raporNo;
        public int bolumNo;
        public string ekBilgi;
        public string raporCiktiTur;
        /// <summary>XML'deki AD (kaynakta REP_grup_no yoksa gösterilir).</summary>
        public string ad;

        public string sablonAd
        {
            get
            {
                if (string.IsNullOrWhiteSpace(yolsuzSablonAd)) return "";
                return RaporlamaServis.RaporSablonYolGetir(Istemci.Ayar.Oku("RaporSablonYol")) + yolsuzSablonAd;
            }
        }

        public Rapor(string p_raporNo, string p_yetkiTipleri, string p_raporTip, string p_sablonAd, string p_dllAd, string p_nesneAd, string p_bolumNo, string ekBilgi, string p_raporCiktiTur, string ad)
        {
            raporNo = OrtakFonksiyonlar.ConvertToInt(p_raporNo, -1);
            bolumNo = OrtakFonksiyonlar.ConvertToInt(p_bolumNo, -1);
            this.ekBilgi = ekBilgi;
            yetkiTipIsimleri = RaporlamaServis.YetkiStringParcala(p_yetkiTipleri);
            string tip = (p_raporTip ?? "").ToUpperInvariant();
            if (tip == "EXCEL")
                raporTip = RAPORTIP.EXCEL;
            else if (tip == "METOD")
                raporTip = RAPORTIP.METOD;
            else
                throw new RaporException(string.Format(RaporlamaServis.str4, p_raporTip));
            yolsuzSablonAd = p_sablonAd;
            nesneAd = p_nesneAd;
            dllAd = p_dllAd;
            raporCiktiTur = p_raporCiktiTur;
            this.ad = ad;
        }

        public bool GrafikMi()
        {
            return raporCiktiTur == "GRAFIK";
        }

        /// <summary>Eski Rapor.YetkisiVarMi: YETKI boşsa herkese açık; doluysa (eski kodda denetim yorum satırındaydı) kapalı.</summary>
        public bool YetkisiVarMi(Kullanici kullanan)
        {
            return yetkiTipIsimleri.Count == 0;
        }

        /// <summary>Raporu üretir; üretilen dosyanın yolunu (uzantılı) ya da "HATA..." / "BİLGİ..." metnini döndürür.</summary>
        public string RaporHazirla(Kullanici kullanan, object servis, Hashtable ht, string sonucDosyaAd)
        {
            AsposeLisans.Yukle();
            if (string.IsNullOrEmpty(sonucDosyaAd))
                sonucDosyaAd = DosyaIslem.DosyaAdUret();
            if (raporTip == RAPORTIP.EXCEL)
                return OrtakClass.ExcelRapor.ExcelRaporHesapla(kullanan, null, sablonAd, sonucDosyaAd, null, ht);
            return RaporlamaServis.MetodCagir(nesneAd, kullanan, servis, sablonAd, sonucDosyaAd, ht, ekBilgi);
        }
    }

    public class RaporGrup
    {
        private readonly List<string> yetkiTipIsimleri;
        public KistasEkran kistasEkran = new KistasEkran(null);
        private readonly List<Rapor> raporlar = new List<Rapor>();
        public int grupNo = -1;
        public bool yetkiKontrol;
        public string ekBilgi;
        /// <summary>XML'deki AD (kaynakta REP_G_grup yoksa gösterilir).</summary>
        public string ad;

        public RaporGrup(string p_yetkiTipleri, string p_grupNo, string p_yetkiKontroluYapma, string ekBilgi, string ad)
        {
            grupNo = OrtakFonksiyonlar.ConvertToInt(p_grupNo, -1);
            this.ekBilgi = ekBilgi;
            this.ad = ad;
            yetkiTipIsimleri = RaporlamaServis.YetkiStringParcala(p_yetkiTipleri);
            yetkiKontrol = p_yetkiKontroluYapma == null || p_yetkiKontroluYapma == "0";
        }

        public void KistasEkranAta(KistasEkran p_kistasEkran)
        {
            kistasEkran = p_kistasEkran ?? new KistasEkran(null);
        }

        public void Add(Rapor r)
        {
            raporlar.Add(r);
        }

        public int RaporSayisi()
        {
            return raporlar.Count;
        }

        public Rapor this[int i]
        {
            get { return i >= 0 && i < raporlar.Count ? raporlar[i] : null; }
        }

        public Rapor Rapor(int rNo)
        {
            foreach (Rapor r in raporlar)
                if (r.raporNo == rNo) return r;
            return null;
        }

        public string RaporGrupAd()
        {
            return Istemci.Metin.Ortak(RaporlamaServis.RaporGrupResAd(grupNo), string.IsNullOrEmpty(ad) ? "Rapor Grubu " + grupNo : ad);
        }

        /// <summary>Raporun adı (eski RaporGrup.RaporAd: OrtakClassRes REP_grup_no); yoksa XML'deki AD, o da yoksa "grup-no".</summary>
        public string RaporAd(int raporNo)
        {
            Rapor r = Rapor(raporNo);
            string varsayilan = r != null && !string.IsNullOrEmpty(r.ad) ? r.ad : "Rapor " + grupNo + "-" + raporNo;
            return Istemci.Metin.Ortak(RaporlamaServis.RaporResAd(grupNo, raporNo), varsayilan);
        }

        public bool YetkisiVarMi(Kullanici kullanan)
        {
            return yetkiTipIsimleri.Count == 0;
        }

        /// <summary>
        /// Eski RaporGrup.RaporAl(kullanan, servis, Rapor, ht, gonder, kaydet, filePath): zorunlu alanları ve yetkiyi denetler,
        /// raporu filePath (uzantısız) adıyla üretir; CiktiYeri PDF ise PDF'e çevirir. Dosya yolunu ya da "HATA..."/"BİLGİ..."
        /// metnini döndürür. Denetimden geçmezse RaporException atar.
        /// </summary>
        public string RaporAl(Kullanici kullanan, object servis, Rapor r, Hashtable ht, string filePath)
        {
            string hata = kistasEkran.ZorunlulukKontrol(ht, grupNo);
            if (hata != "") throw new RaporException(hata);
            if (!YetkisiVarMi(kullanan)) throw new RaporException(string.Format(RaporlamaServis.str12, RaporGrupAd()));
            if (r == null) throw new RaporException(RaporlamaServis.str13);
            if (!r.YetkisiVarMi(kullanan)) throw new RaporException(string.Format(RaporlamaServis.str14, RaporAd(r.raporNo)));

            string cikanDosya = r.RaporHazirla(kullanan, servis, ht, filePath);
            if (string.IsNullOrEmpty(cikanDosya)) throw new RaporException(string.Format(RaporlamaServis.str15, RaporAd(r.raporNo)));
            if (RaporlamaServis.MesajMi(cikanDosya)) return cikanDosya;

            if ((ht["CiktiYeri"] + "").ToUpperInvariant() == "PDF")
                cikanDosya = RaporlamaServis.Dosya2PDF(cikanDosya, true);
            return cikanDosya;
        }
    }

    public class RaporGrupListesi
    {
        private readonly List<RaporGrup> raporGruplari = new List<RaporGrup>();
        public string hata = "";

        public void Add(RaporGrup rg)
        {
            raporGruplari.Add(rg);
        }

        public int RaporGrupSayisi()
        {
            return raporGruplari.Count;
        }

        public RaporGrup RaporGrup(int no)
        {
            foreach (RaporGrup rg in raporGruplari)
                if (rg.grupNo == no) return rg;
            return null;
        }
    }

    public static class RaporlamaServis
    {
        public static string str1 = "Tanınmayan Değişken Tipi ({0}) geldi!";
        public static string str2 = "'{0}' alanını girmelisiniz!";
        public static string str4 = "Tanınmayan Rapor Tipi ({0}) geldi!";
        public static string str6 = "Metod raporu hazırlanırken, belirtilen metoda erişilemedi!";
        public static string str12 = "{0} grubundaki raporları görme yetkiniz yok!";
        public static string str13 = "Rapor bulunamadı";
        public static string str14 = "{0} raporunu görme yetkiniz yok.";
        public static string str15 = "{0} raporu üretilemedi!";
        public static string str18 = "Rapor XML dosyası \"RAPORLAR\" ile başlamalı";
        public static string str19 = "\"KISTAS\" bölümü başlamadan, \"KISTASALAN\" bölümü ile karşılaşıldı.";
        public static string str20 = "\"RAPORGRUP\" bölümü başlamadan \"RAPOR\" bölümü geldi!";
        public static string str22 = "\"RAPORLAR\" bölümü başlamadan bitti!";
        public static string str23 = "\"RAPORGRUP\" bölümü başlamadan \"KISTAS\" bölümü geldi!";
        public static string str24 = "\"RAPORLAR\" kapanmadan XML dosyası bitti.";

        internal static string RaporResAd(int grupNo, int raporNo)
        {
            return "REP_" + grupNo + "_" + raporNo;
        }

        internal static string RaporGrupResAd(int grupNo)
        {
            return "REP_G_" + grupNo;
        }

        internal static string KistasAlanResAd(int grupNo, string degiskenAd)
        {
            return "REP_A_" + grupNo + "_" + degiskenAd;
        }

        public static string RaporAd(int grupNo, int raporNo)
        {
            RaporGrup rg = RaporListesiAl().RaporGrup(grupNo);
            return rg != null ? rg.RaporAd(raporNo) : Istemci.Metin.Ortak(RaporResAd(grupNo, raporNo), "");
        }

        /// <summary>Rapor sınıflarının döndürdüğü "HATA..." / "BİLGİ..." iletisi mi (dosya yolu değil)?</summary>
        public static bool MesajMi(string sonuc)
        {
            return sonuc != null && (sonuc.StartsWith("HATA") || sonuc.StartsWith("BILGI") || sonuc.StartsWith("BİLGİ"));
        }

        /// <summary>Kaydedilen rapor dosyasının adı: kullanici_grup_no_anahtar (eski RaporDosyaAdUret).</summary>
        public static string RaporDosyaAdUret(int raporGrupNo, int raporNo, string anahtar, string kullaniciAd)
        {
            return kullaniciAd + "_" + raporGrupNo + "_" + raporNo + "_" + anahtar;
        }

        /// <summary>
        /// Rapor dosyalarının klasörü: appSettings RaporDosyaYol\grup\no (kütüphane ise \kutuphane); klasör yoksa oluşturulur.
        /// dosyaAd verilirse sonuna eklenir (eski RaporDosyaYolUret).
        /// </summary>
        public static string RaporDosyaYolUret(int raporGrupNo, int raporNo, string dosyaAd, bool kutuphaneMi)
        {
            string dosyaYol = Istemci.Ayar.Oku("RaporDosyaYol").Trim().TrimEnd('\\', '/');
            if (string.IsNullOrEmpty(dosyaYol))
                throw new RaporException("Ayar dosyasında 'RaporDosyaYol' değişkeni bulunamadı!");
            dosyaYol = Path.Combine(dosyaYol, raporGrupNo.ToString(), raporNo.ToString());
            if (kutuphaneMi)
                dosyaYol = Path.Combine(dosyaYol, "kutuphane");
            Directory.CreateDirectory(dosyaYol);
            return string.IsNullOrEmpty(dosyaAd) ? dosyaYol : Path.Combine(dosyaYol, dosyaAd);
        }

        /// <summary>Raporu üretir ve dosya yolunu döndürür; sonuc dosyası geçici klasöre yazılır (eski TekRaporUret).</summary>
        public static string TekRaporUret(int rapGrupNo, int rapNo, Kullanici kullanan, object servis, Hashtable raporParam)
        {
            return TekRaporAl(rapGrupNo, rapNo, kullanan, servis, raporParam, null);
        }

        /// <summary>Raporu yol (uzantısız; boşsa geçici dosya) adıyla üretir (eski TekRaporAl(..., gonder, kaydet, yol)).</summary>
        public static string TekRaporAl(int rapGrupNo, int rapNo, Kullanici kullanan, object servis, Hashtable raporParam, string yol)
        {
            RaporGrup rg = RaporListesiAl().RaporGrup(rapGrupNo);
            if (rg == null)
                throw new RaporException(rapGrupNo + " rapor bulunamadı");
            return rg.RaporAl(kullanan, servis, rg.Rapor(rapNo), raporParam ?? new Hashtable(), yol);
        }

        public static List<string> YetkiStringParcala(string p_yetkiTipleri)
        {
            List<string> liste = new List<string>();
            foreach (string s in (p_yetkiTipleri ?? "").Split(','))
                if (!string.IsNullOrEmpty(s)) liste.Add(s);
            return liste;
        }

        public static string RaporSablonYolGetir(string raporSablonYol)
        {
            raporSablonYol = raporSablonYol ?? "";
            if (raporSablonYol.EndsWith("\\") || raporSablonYol.EndsWith("/"))
                raporSablonYol = raporSablonYol.Substring(0, raporSablonYol.Length - 1);
            return raporSablonYol + "/";
        }

        // ---------------------------------------------------------------- METOD raporları

        private static readonly Dictionary<string, Type> tipler = new Dictionary<string, Type>(StringComparer.Ordinal);

        /// <summary>NESNE adındaki sınıfı (isim alanı + ad) yüklü derlemelerde bulur.</summary>
        public static Type NesneBul(string nesneAd)
        {
            if (string.IsNullOrWhiteSpace(nesneAd)) return null;
            lock (tipler)
            {
                if (tipler.TryGetValue(nesneAd, out Type t)) return t;
                t = typeof(RaporlamaServis).Assembly.GetType(nesneAd, false);
                if (t == null)
                    foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (a.IsDynamic) continue;
                        t = a.GetType(nesneAd, false);
                        if (t != null) break;
                    }
                tipler[nesneAd] = t;
                return t;
            }
        }

        private static readonly Type[] raporAlImza = { typeof(object), typeof(object), typeof(string), typeof(string), typeof(string), typeof(Hashtable), typeof(string) };

        /// <summary>Rapor taşınmış mı (NESNE sınıfı ve RaporAl metodu Core'da var mı)?</summary>
        public static bool NesneVar(string nesneAd)
        {
            Type t = NesneBul(nesneAd);
            return t != null && t.GetMethod("RaporAl", BindingFlags.Public | BindingFlags.Instance, null, raporAlImza, null) != null;
        }

        /// <summary>Eski Rapor.MetodRaporHazirla: NESNE sınıfının RaporAl(kullanan, servis, sablonAd, sonucDosyaAd, null, ht, ekBilgi) metodu.</summary>
        internal static string MetodCagir(string nesneAd, Kullanici kullanan, object servis, string sablonAd, string sonucDosyaAd, Hashtable ht, string ekBilgi)
        {
            Type t = NesneBul(nesneAd);
            MethodInfo m = t != null ? t.GetMethod("RaporAl", BindingFlags.Public | BindingFlags.Instance, null, raporAlImza, null) : null;
            if (m == null)
                throw new RaporException("Bu rapor henüz yeni sisteme taşınmadı (" + nesneAd + ").");
            object nesne = Activator.CreateInstance(t, true);
            try
            {
                return (string)m.Invoke(nesne, new object[] { kullanan, servis, sablonAd, sonucDosyaAd, null, ht, ekBilgi });
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        // ---------------------------------------------------------------- PDF

        /// <summary>
        /// Excel / Word dosyasını aynı klasörde aynı adlı .pdf dosyasına çevirir (eski DosyaTipDegistir.Dosya2PDF; Aspose ile).
        /// </summary>
        public static string Dosya2PDF(string girenAd, bool kaynakSilinsin)
        {
            string cikanAd = Path.Combine(Path.GetDirectoryName(girenAd), Path.GetFileNameWithoutExtension(girenAd) + ".pdf");
            string ext = Path.GetExtension(girenAd).ToLowerInvariant();
            if (ext == ".pdf") return girenAd;
            AsposeLisans.Yukle();
            if (ext.StartsWith(".xls") || ext.StartsWith(".xlt") || ext == ".csv")
                new Aspose.Cells.Workbook(girenAd).Save(cikanAd, Aspose.Cells.SaveFormat.Pdf);
            else if (ext.StartsWith(".doc") || ext.StartsWith(".dot") || ext == ".rtf" || ext == ".htm" || ext == ".html")
                new Aspose.Words.Document(girenAd).Save(cikanAd, Aspose.Words.SaveFormat.Pdf);
            else
                throw new RaporException("Bu dosya türü PDF'e çevrilemez.");
            if (kaynakSilinsin)
            {
                try { File.Delete(girenAd); } catch { }
            }
            return cikanAd;
        }

        // ---------------------------------------------------------------- Rapor.xml

        private static RaporGrupListesi raporGrupListesi;
        private static DateTime raporDosyaZaman;
        private static readonly object kilit = new object();

        /// <summary>Rapor.xml'deki gruplar; dosya değiştiyse yeniden okunur.</summary>
        public static RaporGrupListesi RaporListesiAl()
        {
            string dosyaAd = Istemci.Ayar.Oku("RaporlarDosyaAd");
            DateTime zaman = File.Exists(dosyaAd) ? File.GetLastWriteTimeUtc(dosyaAd) : DateTime.MinValue;
            lock (kilit)
            {
                if (raporGrupListesi == null || zaman != raporDosyaZaman)
                {
                    RaporGrupListesi yeni = new RaporGrupListesi();
                    RaporlariOku(dosyaAd, yeni);
                    raporGrupListesi = yeni;
                    raporDosyaZaman = zaman;
                }
                return raporGrupListesi;
            }
        }

        public static RaporGrupListesi RaporListesiYenile()
        {
            lock (kilit)
            {
                raporGrupListesi = null;
            }
            return RaporListesiAl();
        }

        private static void RaporlariOku(string dosyaAd, RaporGrupListesi raporlar)
        {
            if (string.IsNullOrEmpty(dosyaAd) || !File.Exists(dosyaAd))
            {
                raporlar.hata = "Rapor tanım dosyası (RaporlarDosyaAd) bulunamadı.";
                return;
            }

            RaporGrup rg = null;
            KistasEkran ke = null;
            bool devam = true;
            bool ilk = true;
            try
            {
                using (XmlReader x = XmlReader.Create(dosyaAd, new XmlReaderSettings { IgnoreComments = true, DtdProcessing = DtdProcessing.Prohibit }))
                {
                    while (devam && x.Read())
                    {
                        if (x.NodeType == XmlNodeType.Element)
                        {
                            string ad = x.Name.ToUpperInvariant();
                            if (ilk)
                            {
                                ilk = false;
                                if (ad != "RAPORLAR") throw new RaporException(str18);
                            }
                            if (ad == "RAPORGRUP")
                                rg = new RaporGrup(x.GetAttribute("YETKI"), x.GetAttribute("GRUPNO"), x.GetAttribute("YETKIKONTROLUYAPMA"), x.GetAttribute("EKBILGI"), x.GetAttribute("AD"));
                            else if (ad == "KISTAS")
                            {
                                if (rg == null) throw new RaporException(str23);
                                ke = new KistasEkran(x.GetAttribute("EKBILGI"));
                                if (x.IsEmptyElement)
                                {
                                    rg.KistasEkranAta(ke);
                                    ke = null;
                                }
                            }
                            else if (ad == "KISTASALAN")
                            {
                                if (ke == null) throw new RaporException(str19);
                                ke.Add(new KistasEkranAlan(x.GetAttribute("DEGISKEN"), x.GetAttribute("DEGISKENAD"), x.GetAttribute("TIP"), x.GetAttribute("LISTE"), x.GetAttribute("GIZLI"), x.GetAttribute("ILKDEGER"), x.GetAttribute("ZORUNLU"), x.GetAttribute("EKBILGI"), x.GetAttribute("GOSTERILECEKRAPOR"), x.GetAttribute("ETIKET")));
                            }
                            else if (ad == "RAPOR")
                            {
                                if (rg == null) throw new RaporException(str20);
                                rg.Add(new Rapor(x.GetAttribute("NO"), x.GetAttribute("YETKI"), x.GetAttribute("RAPORTIP"), x.GetAttribute("SABLON"), x.GetAttribute("DLL"), x.GetAttribute("NESNE"), x.GetAttribute("BOLUMNO"), x.GetAttribute("EKBILGI"), x.GetAttribute("RAPORCIKTITUR"), x.GetAttribute("AD")));
                            }
                        }
                        else if (x.NodeType == XmlNodeType.EndElement)
                        {
                            string ad = x.Name.ToUpperInvariant();
                            if (ad == "RAPORGRUP")
                            {
                                if (rg == null) throw new RaporException(str22);
                                raporlar.Add(rg);
                                rg = null;
                            }
                            else if (ad == "KISTAS")
                            {
                                if (rg == null) throw new RaporException(str23);
                                rg.KistasEkranAta(ke);
                                ke = null;
                            }
                            else if (ad == "RAPORLAR")
                                devam = false;
                        }
                    }
                }
            }
            catch (RaporException e)
            {
                raporlar.hata = e.Message;
            }
            catch (Exception e)
            {
                raporlar.hata = e.Message;
            }
            if (devam && raporlar.hata == "")
                raporlar.hata = str24;
        }
    }
}
