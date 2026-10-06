using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Mail;
using System.Collections;
using System.Net;
using System.Security.Cryptography;
using System.Xml;
using System.Globalization;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TNS
{
    /// <summary>
    /// Methods to remove HTML from strings.
    /// </summary>
    public static class HtmlRemoval
    {
        /// <summary>
        /// Remove HTML from string with Regex.
        /// </summary>
        public static string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Compiled regular expression for performance.
        /// </summary>
        static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        public static string StripTagsRegexCompiled(string source)
        {
            return _htmlRegex.Replace(source, string.Empty);
        }
    }

    public static class FaizIslemler
    {
        public class FaizDonem
        {
            public DateTime bitTarih;
            public double faizOran;

            public FaizDonem(FaizDonem faizDonem)
            {
                this.bitTarih = faizDonem.bitTarih;
                this.faizOran = faizDonem.faizOran;
            }

            public FaizDonem(DateTime bitTarih, double faizOran)
            {
                this.bitTarih = bitTarih;
                this.faizOran = faizOran;
            }
        }

        static double GunlukFaizOran(double faizOran) { return OrtakFonksiyonlar.YuvarlaYukari(faizOran / 30, 6); } //0.00046700; // 

        public static FaizDonem[] faizOran = { new FaizDonem(new DateTime(2018, 9, 5), 0.014),
            new FaizDonem(new DateTime(2019, 7, 1), 0.02),
            new FaizDonem(new DateTime(2019, 10, 2), 0.025),
            new FaizDonem(new DateTime(2019, 12, 30), 0.02),
            new FaizDonem(new DateTime(2022, 7, 21), 0.016),
            new FaizDonem(new DateTime(2023, 11, 14), 0.025), //14.11.2023 tarihine kadar geçerli, 14.11.2023'de geçerli değil, bir alt satıe geçerli
            new FaizDonem(new DateTime(2024, 05, 21), 0.035),
            new FaizDonem(new DateTime(2025, 11, 13), 0.045),
            new FaizDonem(DateTime.MaxValue, 0.0370),
        };

        public static FaizDonem[] tecilFaizi = { new FaizDonem(new DateTime(2018, 9, 5), 0.01),
            new FaizDonem(new DateTime(2019, 10, 24), 0.01833333333),
            new FaizDonem(new DateTime(2019, 12, 30), 0.01583333333),
            new FaizDonem(new DateTime(2022, 7, 21), 0.0125),
            new FaizDonem(new DateTime(2023, 11, 14), 0.02),
            new FaizDonem(new DateTime(2024, 05, 21), 0.03),
            new FaizDonem(new DateTime(2025, 11, 14), 0.04),
            new FaizDonem(DateTime.MaxValue, 0.0325),
        };

        public static FaizDonem[] tecilFaiziKurumsal = {
            new FaizDonem(new DateTime(2018, 9, 5), 0.01),
            new FaizDonem(new DateTime(2019, 10, 24), 0.01833333333),
            new FaizDonem(new DateTime(2019, 12, 30), 0.01583333333),
            new FaizDonem(new DateTime(2022, 7, 21), 0.0125),
            new FaizDonem(new DateTime(2023, 11, 14), 0.02),
            new FaizDonem(new DateTime(2024, 05, 21), 0.03),
            new FaizDonem(new DateTime(2025, 11, 14), 0.04),
            new FaizDonem(new DateTime(2026, 06, 15), 0.0325),
            new FaizDonem(DateTime.MaxValue, 0.02416666667),
        };

        public static FaizDonem[] FaizOranAyBul(DateTime taksitTalepTarihi, int tecilFaizNo)
        {
            FaizDonem[] tecFazSec;

            if (tecilFaizNo == 1)
                tecFazSec = tecilFaiziKurumsal;
            else
                tecFazSec = tecilFaizi;
            List<FaizDonem> l = new List<FaizDonem>();
            int baslaYer = -1;
            double oran = 0;
            for (int i = 0; i < tecFazSec.Length && (baslaYer == -1); i++)
            {
                if (taksitTalepTarihi < tecFazSec[i].bitTarih)
                {
                    baslaYer = i;
                    oran = tecFazSec[i].faizOran;
                    l.Add(new FaizDonem(tecFazSec[i]));
                }
            }

            for (int i = baslaYer + 1; i < tecFazSec.Length; i++)
            {
                if (tecFazSec[i].faizOran < oran)
                {
                    oran = tecFazSec[i].faizOran;
                    l.Add(new FaizDonem(tecFazSec[i]));
                }
            }

            if (l.Count > 0)
                l[l.Count - 1].bitTarih = DateTime.MaxValue;
            else
                l.Add(new FaizDonem(DateTime.MaxValue, 0));
            return l.ToArray();
        }

        public static double FaizHesapla(DateTime odemeSonGunu, DateTime faizBaslangicTarihi, DateTime taksitTalepTarih, DateTime hesapTarihi, DateTime faizDurdurulmaTarihi, double anaPara, FaizDonem[] faizOran, FaizDonem[] faizOranAy, bool taksitGunuODemesi, DateTime faizDurdurmaTarihi, StringBuilder sonucBilgi, out double oran1, out double oran2, out int ay1, out int gun1, out int ay2, out int gun2)
        {
            if (faizDurdurmaTarihi > DateTime.MinValue && hesapTarihi > faizDurdurmaTarihi) hesapTarihi = faizDurdurmaTarihi;
            if (taksitTalepTarih == DateTime.MinValue || hesapTarihi <= taksitTalepTarih)
            {
                DateTime tutDT = hesapTarihi;
                if (taksitTalepTarih != DateTime.MinValue && hesapTarihi == taksitTalepTarih && taksitGunuODemesi) tutDT = tutDT.AddDays(-1);
                oran1 = FaizOranHesapla(odemeSonGunu, faizBaslangicTarihi, tutDT, faizDurdurulmaTarihi, faizOran, true, sonucBilgi, out ay1, out gun1);
                oran2 = 0;
                ay2 = 0;
                gun2 = 0;
                return OrtakFonksiyonlar.KurusYap(anaPara * oran1);
            }
            else
            {
                DateTime tutDT = taksitTalepTarih.AddDays(-1);
                oran1 = FaizOranHesapla(odemeSonGunu, faizBaslangicTarihi, tutDT, faizDurdurulmaTarihi, faizOran, true, sonucBilgi, out ay1, out gun1);
                double tut1 = OrtakFonksiyonlar.KurusYap(anaPara * oran1);
                oran2 = FaizOranHesapla(odemeSonGunu, tutDT, hesapTarihi, faizDurdurulmaTarihi, faizOranAy, false, sonucBilgi, out ay2, out gun2);
                double tut2 = OrtakFonksiyonlar.KurusYap((anaPara + tut1) * oran2);
                return OrtakFonksiyonlar.KurusYap(tut1 + tut2);
            }
        }

        public static double FaizHesapla(DateTime odemeSonGunu, DateTime faizBaslangicTarihi, DateTime taksitTalepTarih, DateTime taksitFaizBulmaTarihi, DateTime hesapTarihi, DateTime faizDurdurulmaTarihi, double anaPara, bool taksitGunuODemesi, DateTime faizDurdurmaTarihi, int tacilFaizNo, StringBuilder sonucBilgi, out double oran1, out double oran2, out int ay1, out int gun1, out int ay2, out int gun2)
        {
            return FaizHesapla(odemeSonGunu, faizBaslangicTarihi, taksitTalepTarih, hesapTarihi, faizDurdurulmaTarihi, anaPara, faizOran, FaizOranAyBul(taksitTalepTarih, tacilFaizNo), taksitGunuODemesi, faizDurdurmaTarihi, sonucBilgi, out oran1, out oran2, out ay1, out gun1, out ay2, out gun2);
        }

        public static double FaizOranHesapla(DateTime odemeSonGunu, DateTime faizBaslangicTarihiOrj, DateTime hesapTarihi, DateTime faizDurdurulmaTarihi, FaizDonem[] faizOran, bool GIBYontem, System.Text.StringBuilder sonucBilgi, out int ay, out int gun)
        {
            ay = 0;
            gun = 0;
            //if (faizBaslangicTarihiOrj < odemeSonGunu) faizBaslangicTarihiOrj = odemeSonGunu;
            DateTime faizBaslangicTarihi = faizBaslangicTarihiOrj;
            try
            {
                if (odemeSonGunu.Year < 1000) return 0;
                if (faizDurdurulmaTarihi != DateTime.MinValue && hesapTarihi > faizDurdurulmaTarihi) hesapTarihi = faizDurdurulmaTarihi;
                if (hesapTarihi > odemeSonGunu)
                {
                    if (GIBYontem)
                    {
                        TNS.TarihFark tf = new TarihFark(faizBaslangicTarihi, hesapTarihi, true, false);
                        ay = tf.YilAy;
                        gun = tf.Gun;
                    }
                    else
                    {
                        ay = 0;
                        gun = (hesapTarihi - faizBaslangicTarihi).Days;
                    }
                    bool ilkAySonGun = TarihFark.AyinSonGunuMu(faizBaslangicTarihiOrj);
                    bool sonAySonGun = TarihFark.AyinSonGunuMu(hesapTarihi);
                    int ilkAyGun = faizBaslangicTarihiOrj.Day;

                    double sonuc = 0;
                    DateTime oncekiTarih = DateTime.MinValue;
                    for (int i = 0; i < faizOran.Length; i++)
                    {
                        bool kes = false;
                        DateTime bitTarih = faizOran[i].bitTarih;
                        double faizOranIc = faizOran[i].faizOran;
                        if (faizBaslangicTarihi >= bitTarih) continue;
                        DateTime hesapTarihiIc;

                        if (hesapTarihi >= bitTarih)
                        {
                            if (GIBYontem)
                            {
                                TNS.TarihFark tfBT = new TarihFark(faizBaslangicTarihi, bitTarih, false, false);
                                int tamAy = tfBT.YilAy;
                                DateTime hesapTarihiIc1 = AyEkle(faizBaslangicTarihi, tamAy + 1, ilkAySonGun, ilkAyGun);
                                if (hesapTarihiIc1 <= hesapTarihi)
                                {
                                    if (tfBT.Gun == 0)
                                        hesapTarihiIc = hesapTarihiIc1.AddMonths(-1);
                                    else
                                        hesapTarihiIc = hesapTarihiIc1;
                                }
                                else
                                {
                                    if (tfBT.Gun != 0)
                                        hesapTarihiIc = bitTarih.AddDays(-1);
                                    else
                                        hesapTarihiIc = bitTarih;
                                    //tfBT = new TarihFark(hesapTarihiIc, hesapTarihi, false, false, false);
                                    //if (tfBT.YilAy == 0)
                                    //{
                                    //}
                                }

                                /*
                                //Önceki gün tam ay mı
                                TNS.TarihFark tfBT = new TarihFark(faizBaslangicTarihi, bitTarih.AddDays(-1), false, false);
                                if (ilkAySonGun && !sonAySonGun && tfBT.Gun == 0)
                                {
                                    hesapTarihiIc = bitTarih.AddDays(-1);
                                }
                                else
                                {
                                    TNS.TarihFark tfHT = new TarihFark(faizBaslangicTarihi, hesapTarihi, false, false);
                                    tfBT = new TarihFark(faizBaslangicTarihi, bitTarih, false, false);
                                    if (tfBT.YilAy < tfHT.YilAy)
                                    {
                                        hesapTarihiIc = AyEkle(faizBaslangicTarihi, tfBT.YilAy + 1, ilkAySonGun, ilkAyGun);
                                    }
                                    else
                                        hesapTarihiIc = bitTarih.AddDays(-1);
                                }
                                */
                            }
                            else
                                hesapTarihiIc = bitTarih.AddDays(-1);
                        }
                        else
                        {
                            hesapTarihiIc = hesapTarihi;
                            kes = true;
                        }
                        double icFaiz = 0;
                        if (GIBYontem)
                        {
                            TNS.TarihFark tf = new TarihFark(faizBaslangicTarihi, hesapTarihiIc, true, false);
                            icFaiz = OrtakFonksiyonlar.Yuvarla(tf.YilAy * faizOranIc + tf.Gun * GunlukFaizOran(faizOranIc), 6);
                            if (sonucBilgi != null && icFaiz > 0)
                                sonucBilgi.AppendLine($"{TarihStr(faizBaslangicTarihi)}-{TarihStr(hesapTarihiIc)} arası {tf.YilAy} ay {tf.Gun} gün = {icFaiz} ({faizOranIc})");
                        }
                        else
                        {
                            int gunIc = (hesapTarihiIc - faizBaslangicTarihi).Days;
                            icFaiz = OrtakFonksiyonlar.Yuvarla(gunIc * faizOranIc / 30, 6);
                            if (sonucBilgi != null && icFaiz > 0)
                                sonucBilgi.AppendLine($"{TarihStr(faizBaslangicTarihi)}-{TarihStr(hesapTarihiIc)} arası {gunIc} gün = {icFaiz} ({faizOranIc})");
                        }
                        sonuc = OrtakFonksiyonlar.Yuvarla(sonuc + icFaiz, 6);
                        faizBaslangicTarihi = hesapTarihiIc;//.AddDays(1);
                        if (kes) break;
                    }
                    if (sonucBilgi != null)
                        sonucBilgi.AppendLine($"Toplam : {sonuc}");
                    return sonuc;
                }
            }
            catch
            {
                OrtakFonksiyonlar.HataDosyaYaz("FaizHesabi.txt", String.Format("odemeSonGunu:{0}, faizBaslangicTarihi:{1}, hesapTarihi:{2}, faizDurdurulmaTarihi:{3}, faizOran:{4}, GIBYontem:{5}", StrYap(odemeSonGunu), StrYap(faizBaslangicTarihi), StrYap(hesapTarihi), StrYap(faizDurdurulmaTarihi), StrYap(faizOran), GIBYontem), false);
            }
            return 0;
        }

        private static object TarihStr(DateTime hesapTarihiIc)
        {
            return hesapTarihiIc.ToShortDateString();
        }

        private static DateTime AyEkle(DateTime faizBaslangicTarihi, int ay, bool ilkAySonGun, int ilkAyGun)
        {
            DateTime yeniTarih = faizBaslangicTarihi.AddMonths(ay);
            int sonGun = DateTime.DaysInMonth(yeniTarih.Year, yeniTarih.Month);
            if (ilkAySonGun || ilkAyGun > sonGun)
                yeniTarih = new DateTime(yeniTarih.Year, yeniTarih.Month, sonGun);
            else
                yeniTarih = new DateTime(yeniTarih.Year, yeniTarih.Month, ilkAyGun);
            return yeniTarih;
        }

        private static object StrYap(FaizDonem[] faizOran)
        {
            string retVal = "";
            if (faizOran == null) return retVal;
            foreach (FaizDonem item in faizOran)
            {
                retVal += StrYap(item.bitTarih) + ":" + item.faizOran;
            }
            return "[" + retVal + "]";
        }

        private static object StrYap(DateTime odemeSonGunu)
        {
            return odemeSonGunu.ToShortDateString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="odemeSonTarihiDT"></param>
        /// <param name="faizBaslangicTarihi"></param>
        /// <param name="tarihDT"></param>
        /// <param name="yatMiktar"></param>
        /// <param name="miktar"></param>
        /// <param name="faiz"></param>
        /// <param name="anaPara">0 ise kurus farkı varsa ilem yapmaz, 1 ise kurus farkını ana paraya ekle, 2 ise kuruş farkını faize ekle</param>
        public static void FaizBol(DateTime odemeSonTarihiDT, DateTime faizBaslangicTarihi, DateTime taksitTalepTarihi, DateTime taksitFaizBulmaTarihi, DateTime tarihDT, DateTime faizDurdurulmaTarihi, double yatMiktar, bool gibYontem, int tacilFaizNo, out double miktar, out double faiz, int eksikKalan0Ekleme1AnapaPara2Faiz)
        {
            int ay; int gun;
            double oran = 0;
            if (gibYontem)
            {
                oran = FaizOranHesapla(odemeSonTarihiDT, faizBaslangicTarihi, tarihDT, faizDurdurulmaTarihi, faizOran, true, null, out ay, out gun);
            }
            else
            {
                oran = FaizOranHesapla(odemeSonTarihiDT, faizBaslangicTarihi, tarihDT, faizDurdurulmaTarihi, FaizOranAyBul(taksitTalepTarihi, tacilFaizNo), false, null, out ay, out gun);
            }

            AnaParaFaizEsitle(yatMiktar, oran, out miktar, out faiz, eksikKalan0Ekleme1AnapaPara2Faiz);
        }

        public static void AnaParaFaizEsitle(double toplamMiktar, double oran, out double miktar, out double faiz, int eksikKalan0Ekleme1AnapaPara2Faiz)
        {
            miktar = OrtakFonksiyonlar.KurusYap(toplamMiktar / (1 + oran));
            faiz = OrtakFonksiyonlar.KurusYap(miktar * oran);
            if (OrtakFonksiyonlar.EsitMi(miktar + faiz, toplamMiktar)) return;
            if (miktar + faiz > toplamMiktar)
            {
                miktar = OrtakFonksiyonlar.KurusYap(miktar - 0.01);
                faiz = OrtakFonksiyonlar.KurusYap(miktar * oran);
            }
            if (eksikKalan0Ekleme1AnapaPara2Faiz == 0 || OrtakFonksiyonlar.EsitMi(miktar + faiz, toplamMiktar)) return;
            double eksik = toplamMiktar - (miktar + faiz);
            if (eksikKalan0Ekleme1AnapaPara2Faiz == 1) miktar = miktar + eksik;
            else faiz = faiz + eksik;
        }

        public static void AnaParaFaizBol(double toplamMiktar, double oran, out double miktar, out double faiz)
        {
            faiz = OrtakFonksiyonlar.KurusYap(toplamMiktar / (1 + oran) * oran);
            miktar = OrtakFonksiyonlar.KurusYap(toplamMiktar - faiz);
        }


        public static void FaizBol2(DateTime odemeSonTarihiDT, DateTime faizBaslangicTarihi, DateTime taksitTalepTarihi, DateTime faizOranBulmaTarihi, DateTime tarihDT, DateTime faizDurdurulmaTarihi, double yatMiktar, int tecilFaizNo, out double miktar, out double tecilFaizi, out double ertelemeFaizi, int eksikKalan0Ekleme1AnapaPara2Faiz, double ertelemeOran)
        {
            double ertelemeDahilMiktar = 0;
            FaizBol(odemeSonTarihiDT, taksitTalepTarihi.AddDays(-1), taksitTalepTarihi, faizOranBulmaTarihi, tarihDT, faizDurdurulmaTarihi, yatMiktar, false, tecilFaizNo, out ertelemeDahilMiktar, out tecilFaizi, eksikKalan0Ekleme1AnapaPara2Faiz);

            AnaParaFaizEsitle(ertelemeDahilMiktar, ertelemeOran, out miktar, out ertelemeFaizi, eksikKalan0Ekleme1AnapaPara2Faiz);
        }
    }

    [Serializable()]
    public class Boyutlar : IEnumerable, IComparer, IComparable
    {
        private ArrayList boyutlar = new ArrayList();
        public int Count { get { if (boyutlar == null) return 0; return boyutlar.Count; } }
        public object Add(object item)
        {
            boyutlar.Add(item);
            return item;
        }

        public object this[int index]
        {
            get { return boyutlar[index]; }
        }

        public IEnumerator GetEnumerator()
        {
            return boyutlar.GetEnumerator();
        }

        public int Compare(object x, object y)
        {
            if (x == null && y == null) return 0;
            if (x == null || !(x is Boyutlar)) return -1;
            if (y == null || !(y is Boyutlar)) return 1;
            Boyutlar b1 = (Boyutlar)x;
            Boyutlar b2 = (Boyutlar)y;
            int az = Math.Min(b1.Count, b2.Count);
            for (int i = 0; i < az; i++)
            {
                StrKodStrAd bs1 = (StrKodStrAd)b1[i];
                StrKodStrAd bs2 = (StrKodStrAd)b2[i];
                int copRes = bs1.kod.CompareTo(bs2.kod);
                if (copRes != 0) return copRes;
            }
            if (b1.Count > az) return -1;
            if (b2.Count > az) return 1;
            return 0;
        }

        public int CompareTo(object obj)
        {
            return Compare(this, obj);
        }
    }


    [Serializable()]
    public class IkiBoyutluTabloVeri
    {
        public Boyutlar boyutlar = new Boyutlar();
        public object veri;
        public TNS.VariableType veriTip;
    }

    [Serializable()]
    public class IntKodStrAd : IComparable
    {
        public int kod { get; set; }
        public string ad { get; set; }
        public string kisaAd { get; set; }

        public string kodAd
        {
            get
            {
                return kod.ToString() + "-" + ad;
            }
            set
            {
                string a1 = "";
                string a2 = "";
                OrtakFonksiyonlar.StringiIkiyeBol(value, "-", ref a1, ref a2);
                kod = OrtakFonksiyonlar.ConvertToInt(a1, 0);
                ad = a2;
            }
        }

        public IntKodStrAd()
        {
            this.kod = 0;
            this.ad = "";
            this.kisaAd = "";
        }

        public IntKodStrAd(int kod, string ad)
        {
            this.kod = kod;
            this.ad = ad;
            this.kisaAd = "";
        }

        public int CompareTo(object obj)
        {
            if (obj != null)
            {
                if (obj is IntKodStrAd) return this.ad.CompareTo(((IntKodStrAd)obj).ad);
                else if (obj is string) return this.ad.CompareTo(((string)obj));
            }
            return -1;
        }

        public override bool Equals(Object obj)
        {
            return CompareTo(obj) == 0;
        }
    }

    [Serializable()]
    public class StrKodStrAd : IComparable
    {
        public string kod { get; set; }
        public string ad { get; set; }
        public string kisaAd { get; set; }

        public StrKodStrAd()
        {
            this.kod = "";
            this.ad = "";
            this.kisaAd = "";
        }

        public StrKodStrAd(string kod, string ad)
        {
            this.kod = kod;
            this.ad = ad;
            this.kisaAd = "";
        }

        public string kodAd
        {
            get
            {
                return kod + "-" + ad;
            }
            set
            {
                string a1 = "";
                string a2 = "";
                OrtakFonksiyonlar.StringiIkiyeBol(value, "-", ref a1, ref a2);
                kod = a1;
                ad = a2;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj != null)
            {
                if (obj is StrKodStrAd) return this.kod.CompareTo(((StrKodStrAd)obj).kod);
                else if (obj is string) return this.kod.CompareTo(((string)obj));
            }
            return -1;
        }

        public override bool Equals(Object obj)
        {
            return CompareTo(obj) == 0;
        }
    }

    [Serializable()]
    public class HashTableArrayList : IEnumerable
    {
        int min_index = 9999;
        int max_index = -9999;

        public int MinIndex()
        {
            return min_index;
        }

        public int MaxIndex()
        {
            return max_index;
        }

        public static string ArrayListToStr(object o_htIc)
        {
            if (o_htIc == null) return null;
            if (o_htIc.GetType() != typeof(Hashtable)) return null;
            Hashtable htIc = (Hashtable)o_htIc;

            string donusDeger = "";
            foreach (DictionaryEntry de in htIc)
            {
                string deVal = (string)de.Value;
                if (!string.IsNullOrEmpty(deVal))
                {
                    if (donusDeger != "") donusDeger += ";";
                    donusDeger += deVal;
                }
            }
            return donusDeger;
        }

        Hashtable ht = new Hashtable();

        public string this[string index, int sira]
        {
            get
            {
                object o_htIc = ht[index];
                if (o_htIc == null) return null;
                if (o_htIc.GetType() != typeof(Hashtable)) return null;
                Hashtable htIc = (Hashtable)o_htIc;

                return (string)htIc[sira];
            }

            set
            {
                if (sira < min_index) min_index = sira;
                if (sira > max_index) max_index = sira;

                Hashtable htIc = (Hashtable)ht[index];
                if (htIc == null)
                {
                    htIc = new Hashtable();
                    ht[index] = htIc;
                }

                htIc[sira] = value;
            }
        }

        public string this[string index]
        {
            get
            {
                return ArrayListToStr(ht[index]);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)ht.GetEnumerator();
        }

        public void Clear()
        {
            ht.Clear();
        }

        public void Remove(object key)
        {
            ht.Remove(key);
        }
    }

    [Serializable()]
    public class TNSCollection : System.Collections.ArrayList
    {
        public TNSCollection(params object[] tut)
        {
            for (int i = 0; i < tut.Length; i++)
            {
                Add(tut[i]);
            }
        }

        public TNSCollection()
        {
        }
    }

    [Serializable()]
    public class ObjectVAN
    {
        public object o;
        public Sonuc sonuc = new Sonuc();
    }

    [Serializable()]
    public class Sonuc
    {
        /* Bu yapıda, sunucu yaptığı işlemin sonucunu döndürür. Eğer işlem
            yapılmışsa islemSonuc alanı True, işlem yapılamamışsa False olur.

            İşlem yapılamamışsa, hataStr alanında yapılamama sebebi/sebepleri
            olmalıdır.

            bilgiStr alanı, kullanıcıyı bilgilendirmek için verilmesi gereken bir
            mesaj varsa kullanılır.
        */

        public bool islemSonuc { get; set; }
        public string hataStr { get; set; }
        public string bilgiStr { get; set; }
        public int kayitSay { get; set; }
        public string anahtar { get; set; }
        public bool vtHatasi { get; set; }
        public object ekNesne { get; set; } = null;

        public Sonuc()
        {
            islemSonuc = true;
            hataStr = "";
            bilgiStr = "";
            kayitSay = 0;
            anahtar = "";
            vtHatasi = false;
        }

        public Sonuc(string pHataStr)
        {
            islemSonuc = string.IsNullOrEmpty(pHataStr);
            hataStr = pHataStr;
            bilgiStr = "";
            kayitSay = 0;
            anahtar = "";
            vtHatasi = false;
        }

        public Sonuc(string pHataStr, string pBilgiStr)
        {
            islemSonuc = string.IsNullOrEmpty(pHataStr);
            hataStr = pHataStr;
            bilgiStr = pBilgiStr;
            kayitSay = 0;
            anahtar = "";
            vtHatasi = false;
        }

        public Sonuc(bool pIslemSonuc, string pHataStr, string pBilgiStr)
        {
            islemSonuc = pIslemSonuc;
            hataStr = pHataStr;
            bilgiStr = pBilgiStr;
            kayitSay = 0;
            anahtar = "";
            vtHatasi = false;
        }

        public void DegerAta(string pHataStr, string pBilgiStr)
        {
            hataStr += pHataStr;
            bilgiStr += pBilgiStr;
            islemSonuc = string.IsNullOrEmpty(pHataStr);
        }

        public void DegerAta(string pHataStr)
        {
            hataStr += pHataStr;
            islemSonuc = string.IsNullOrEmpty(pHataStr);
        }
    }

    [Serializable()]
    public class ListeSatir
    {
        public string kod;
        public string ad;
        public bool cocuguVar = false;

        public ListeSatir()
        {
        }

        public ListeSatir(string pKod, string pAd)
        {
            kod = pKod;
            ad = pAd;
        }
    }

    [Serializable()]
    public class ListeVAN
    {
        public Sonuc sonuc;
        public TNSCollection satirlar; //ListeSatir 

        public ListeVAN()
        {
            sonuc = new Sonuc();
            satirlar = new TNSCollection();
        }
        public string ListeStr(string ayirac)
        {
            string retVal = "";
            foreach (ListeSatir s in satirlar)
            {
                if (retVal != "") retVal += ayirac;
                retVal += s.kod;
            }
            return retVal;
        }
    }

    [Serializable()]
    public class ObjectArray
    {
        public Sonuc sonuc;
        public TNSCollection objeler; //string
        public int ObjeSayisi
        {
            get { return objeler.Count; }
        }

        public ObjectArray()
        {
            objeler = new TNSCollection();
            sonuc = new Sonuc();
        }

        public void HataAta(string hata)
        {
            sonuc.islemSonuc = false;
            sonuc.hataStr = hata;
        }

        public object Al(int indis)
        {
            if (indis < objeler.Count)
            {
                return objeler[indis];
            }
            else
                return "";
        }

        public string AlStr(int indis)
        {
            if (indis < objeler.Count)
            {
                object o = objeler[indis];
                if (o.GetType() == typeof(string))
                    return (string)o;
                else
                    return o.ToString();
            }
            else
                return "";
        }

        public void Ekle(object obje)
        {
            objeler.Add(obje);
        }

        public object this[int indis]
        {
            get
            {
                if (indis < objeler.Count)
                {
                    return objeler[indis];
                }
                else
                    return "";
            }
        }
    }

    [Serializable()]
    public class TNSDateTime : IComparable
    {
        public static bool IsNull(TNSDateTime dt)
        {
            if (dt == null) return true;
            if (dt.isNull) return true;
            return false;
        }
        private DateTime dt;
        public bool isNull;

        public TNSDateTime(DateTime pdt)
        {
            Yaz(pdt);
        }

        public TNSDateTime(String pdt)
        {
            Yaz(pdt);
        }

        public TNSDateTime()
        {
            isNull = true;
        }
        public TNSDateTime(TNSDateTime dt)
        {
            if (dt != null)
                Yaz(dt.Oku());
            else
                isNull = true;
        }

        public DateTime Oku()
        {
            return dt;
        }

        public void Yaz(DateTime pdt)
        {
            dt = pdt;
            if (dt == DateTime.MinValue)
                isNull = true;
            else
                isNull = false;
        }

        public void Yaz(TNSDateTime pdt)
        {
            if (pdt == null)
                isNull = true;
            else
                Yaz(pdt.Oku());
        }

        public void Yaz(string pdt)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pdt))
                    isNull = true;
                else
                {
                    dt = Convert.ToDateTime(pdt);
                    isNull = false;
                }
            }
            catch (Exception)
            {
                isNull = true;
            }
        }

        public override string ToString()
        {
            if (isNull) return "";
            else return dt.ToShortDateString();
        }

        public string ToLongString()
        {
            if (isNull) return "";
            else return dt.ToString();
        }

        public string ToTimeStamp()
        {
            if (isNull) return "";

            bool yilVar = false;
            string donusDeger = "";

            if (dt.Year != 0)
            {
                yilVar = true;
                donusDeger = dt.Year.ToString().PadLeft(4, '0') + "-" + dt.Month.ToString().PadLeft(2, '0') + "-" + dt.Day.ToString().PadLeft(2, '0');
            }

            if (yilVar) donusDeger = donusDeger + " ";
            donusDeger = donusDeger + dt.Hour.ToString().PadLeft(2, '0') + ":" + dt.Minute.ToString().ToString().PadLeft(2, '0') + ":" + dt.Second.ToString().PadLeft(2, '0') + "." + dt.Millisecond.ToString().PadLeft(3, '0');
            //}
            string tarihTip;
            if (yilVar)
                tarihTip = "ts";
            else
                tarihTip = "ti";

            donusDeger = "{" + tarihTip + " '" + donusDeger + "'}";
            return donusDeger;

        }

        public int CompareTo(object obj)
        {
            //if (dt == null)
            //    return -1;

            if (obj.GetType() == typeof(TNSDateTime))
            {
                TNSDateTime objDateTime = (TNSDateTime)obj;
                return dt.CompareTo(objDateTime.dt);
            }
            else
            {
                return dt.CompareTo(obj);
            }
        }

        public int Yil
        {
            get
            {
                if (isNull) return 0;
                else return dt.Year;
            }
        }

        public int Ay
        {
            get
            {
                if (isNull) return 0;
                else return dt.Month;
            }
        }

        public int Gun
        {
            get
            {
                if (isNull) return 0;
                else return dt.Day;
            }
        }

        public int HaftaninGunu
        {
            get
            {
                if (isNull) return -1;
                else return (int)dt.DayOfWeek;
            }
        }

        public int YilinGunu
        {
            get
            {
                if (isNull) return 0;
                else return dt.DayOfYear;
            }
        }

        public int Saat
        {
            get
            {
                if (isNull) return 0;
                else return dt.Hour;
            }
        }

        public int Dakika
        {
            get
            {
                if (isNull) return 0;
                else return dt.Minute;
            }
        }

        public int Saniye
        {
            get
            {
                if (isNull) return 0;
                else return dt.Second;
            }
        }

        public int MiliSaniye
        {
            get
            {
                if (isNull) return 0;
                else return dt.Millisecond;
            }
        }

        public long Ticks
        {
            get
            {
                if (isNull) return 0;
                else return dt.Ticks;
            }
        }

        public void Add(TimeSpan ts)
        {
            dt = dt.Add(ts);
        }

        public void AddDays(double deger)
        {
            dt = dt.AddDays(deger);
        }

        public void IsGunuEkle(int gun)
        {
            if (gun != 0)
            {
                int direction = gun > 0 ? 1 : -1;
                int remaining = Math.Abs(gun);

                var d = dt;

                while (remaining > 0)
                {
                    d = d.AddDays(direction);

                    // Cumartesi/Pazar değilse say
                    if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                        remaining--;
                }
                dt = d;
            }
        }

        public void AddHours(double deger)
        {
            dt = dt.AddHours(deger);
        }

        public void AddMilliseconds(double deger)
        {
            dt = dt.AddMilliseconds(deger);
        }

        public void AddMinutes(double deger)
        {
            dt = dt.AddMinutes(deger);
        }

        public void AddMonths(int deger)
        {
            dt = dt.AddMonths(deger);
        }

        public void AddSeconds(double deger)
        {
            dt = dt.AddSeconds(deger);
        }

        public void AddTicks(long deger)
        {
            dt = dt.AddTicks(deger);
        }

        public void AddYears(int deger)
        {
            dt = dt.AddYears(deger);
        }
    }

    [Serializable()]
    public class EPostaEki
    {
        public byte[] ek;
        public string ad;
        public string dosyaAd;
        public EPostaEki()
        {
        }

        public EPostaEki(byte[] pEk, string pAd)
        {
            ek = pEk;
            ad = pAd;
        }
    }

    public class EPostaVeri
    {
        public string from { get; set; }
        public string to { get; set; }
        public string cc { get; set; }
        public string bcc { get; set; }
        public string konu { get; set; }
        public string bilgi { get; set; }
        public bool html { get; set; }
        public bool replyVar { get; set; }
        public TNSCollection ekDosyaIsimleri { get; set; }
        public EPostaVeri()
        {
            from = string.Empty;
            to = string.Empty;
            cc = string.Empty;
            bcc = string.Empty;
            konu = string.Empty;
            bilgi = string.Empty;
            html = false;
            replyVar = false;
            ekDosyaIsimleri = null;
        }
        public EPostaVeri(string to, string konu, string bilgi) : base()
        {
            this.to = to;
            this.konu = konu;
            this.bilgi = bilgi;
        }
    }

    public class OrtakFonksiyonlar
    {

        public static double[] OranliDagit(double[] dizi, int sonIndex, double toplam, double dagitilacak, int roundDigits = 0)
        {
            int n = dizi.Length;
            double[] sonuc = new double[n];

            double kalanToplam = toplam;
            double kalanDagitilacak = dagitilacak;

            // Sadece 0..sonIndex aralığında işlem
            for (int i = 0; i <= sonIndex; i++)
            {
                if (i == sonIndex)
                {
                    sonuc[i] = Math.Round(kalanDagitilacak, roundDigits);
                }
                else
                {
                    double oran = dizi[i] / kalanToplam;
                    double pay = Math.Round(kalanDagitilacak * oran, roundDigits);

                    sonuc[i] = pay;

                    kalanToplam -= dizi[i];
                    kalanDagitilacak -= pay;
                }
            }

            // sonIndex sonrası otomatik 0
            for (int i = sonIndex + 1; i < n; i++)
                sonuc[i] = 0;

            return sonuc;
        }

        private static readonly string[] TehlikeliKelimeler = new[] {
            "<script", "%3cscript", "&#x3c;script", "&lt;script",
            "<style", "<iframe", "<object", "<embed", "<link", "<meta", "<svg", "<base", "<form",
            "javascript:", "vbscript:", "data:text/html", "data:",
            "onerror", "onload", "onclick", "onmouseover", "onfocus", "onchange",
            "onblur", "onsubmit", "onkeyup", "ondblclick", "oncontextmenu", "onwheel", "onmousedown",
            "/JavaScript", "javascript:", "/JS ", "/AA /JS", "/Launch"
        };

        private static readonly string[] TehlikeliKelimelerPDF = { "/JavaScript", "javascript:", "/JS ", "/AA /JS", "/Launch" };

        public static bool TehlikeliDizeVar(string okunan, out string tehlikeli)
        {
            return TehlikeliDizeVar(TehlikeliKelimeler, okunan, out tehlikeli);
        }

        public static bool TehlikeliDizeVarPDFx(string okunan, out string tehlikeli)
        {
            return TehlikeliDizeVar(TehlikeliKelimelerPDF, okunan, out tehlikeli);
        }

        public static bool TehlikeliDizeVar(string[] tehlikeliDizeler, string okunan, out string tehlikeli)
        {
            int uz = okunan.Length;
            for (int i = 0; i < tehlikeliDizeler.Length; i++)
            {
                tehlikeli = tehlikeliDizeler[i];
                int yer = okunan.IndexOf(tehlikeli, 0, uz, StringComparison.OrdinalIgnoreCase);
                if (yer < 0) continue;

                if (tehlikeli.StartsWith("on", StringComparison.OrdinalIgnoreCase))
                {
                    if (OncesiKarakterMi(okunan, yer))
                        continue;
                }
                return true;

            }
            tehlikeli = "";
            return false;
        }

        private static bool OncesiKarakterMi(string okunan, int yer)
        {
            // solda sınır
            if (yer > 0 && HarfAltCizgi(okunan[yer - 1]))
                return true;

            return false;
        }

        private static bool HarfAltCizgi(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        //public static bool BasitPdfMi(byte[] pdfBytes, out string tehlikeli)
        //{
        //    if (pdfBytes == null || pdfBytes.Length < 4)
        //    {
        //        tehlikeli = "Dosya geçersiz veya çok küçük.";
        //        return false;
        //    }

        //    int kontrolUzunlugu = System.Math.Min(pdfBytes.Length, 1024);
        //    string ilkKisim = System.Text.Encoding.ASCII.GetString(pdfBytes, 0, kontrolUzunlugu);

        //    if (ilkKisim.IndexOf("%PDF", System.StringComparison.Ordinal) < 0)
        //    {
        //        tehlikeli = "Dosya geçerli bir PDF değil (PDF header %PDF bulunamadı).";
        //        return false;
        //    }

        //    //string raw = System.Text.Encoding.ASCII.GetString(pdfBytes);
        //    //if (OrtakFonksiyonlar.TehlikeliDizeVarPDF(raw, out tehlikeli))
        //    //{
        //    //    return false;
        //    //}
        //    tehlikeli = "";
        //    return true;
        //}

        public static string Maskele(string alan, int bastanGor, int sondanGor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(alan))
                    return alan;

                int uzunluk = alan.Length;
                if (uzunluk <= bastanGor + sondanGor)
                    return alan;

                string bas, bit;
                if (bastanGor > 0)
                    bas = alan.Substring(0, bastanGor);
                else bas = "";

                if (sondanGor > 0)
                    bit = alan.Substring(uzunluk - sondanGor, sondanGor);
                else bit = "";

                return bas + new string('*', uzunluk - bastanGor + sondanGor) + bit;
            }
            catch { return alan; }
        }

        /// <summary>
        /// StrKodStrAd nesneleri içeren diziyi virgülleyerek döndürür
        /// </summary>
        /// <param name="dizi"></param>
        /// <returns></returns>
        public static string VirgulleStrKodStrAd(TNSCollection dizi, bool kodOlsun = true, bool kisaAd = false, string ayrac = ", ", string kodAyrac = "- ")
        {
            string str = "";
            if (dizi != null)
            {
                foreach (StrKodStrAd item in dizi)
                {
                    if (item != null && !string.IsNullOrWhiteSpace(item.kod))
                    {
                        if (str != "") str += ayrac;
                        if (kodOlsun) str += item.kod + kodAyrac;
                        if (kisaAd) str += item.kisaAd;
                        else str += item.ad;
                    }
                }
            }
            return str;
        }

        public static string Tirnakla(string veri)
        {
            return "'" + veri.Replace("'", "''") + "'";
        }


        public static void LogYaz(StringBuilder sb, string mesaj, bool yaz, string dosyaAd = "OdenekGondermeTenkisServis.txt")
        {
            sb.Append(DateTime.Now.ToString("hh:mm:ss.fff") + ":");
            sb.Append(mesaj);
            sb.Append("\r\n");
            if (yaz)
            {
                OrtakFonksiyonlar.HataDosyaYaz(dosyaAd, sb.ToString(), false);
            }
        }

        public static double HTDoubleEkle(Hashtable ht, string htKod, double ekleDeger)
        {
            object tut = ht[htKod];
            double tutD;
            if (tut != null)
                tutD = (double)tut;
            else
                tutD = 0;

            tutD += ekleDeger;
            ht[htKod] = tutD;

            return tutD;
        }

        public enum ENUMMetinEkleYon
        {
            SAG, SOL
        }

        public static async Task<string> WebdenOkuAsync(string webAdres, string postData = null, string cookieHeader = null,
            string cokGizliAnahtar = null, string contentType = "application/x-www-form-urlencoded", string authorization = null)
        {
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12;

            ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(
                    MyCertValidationCb);

            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create(webAdres);

            request.Proxy = null;

            bool postMu = !string.IsNullOrEmpty(postData);
            request.Method = postMu ? "POST" : "GET";

            if (postMu)
            {
                request.ContentType = contentType;

                byte[] dataBytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = dataBytes.Length;

                using (Stream reqStream =
                    await request.GetRequestStreamAsync().ConfigureAwait(false))
                {
                    await reqStream
                        .WriteAsync(dataBytes, 0, dataBytes.Length)
                        .ConfigureAwait(false);
                }
            }

            if (!string.IsNullOrEmpty(cookieHeader))
                request.Headers.Add("Cookie", cookieHeader);

            if (!string.IsNullOrEmpty(cokGizliAnahtar))
                request.Headers.Add(
                    "X-Internal-Key",
                    cokGizliAnahtar);

            if (!string.IsNullOrEmpty(authorization))
                request.Headers.Add(
                    "Authorization",
                    authorization);

            using (HttpWebResponse response =
                (HttpWebResponse)await request
                    .GetResponseAsync()
                    .ConfigureAwait(false))
            using (Stream resStream = response.GetResponseStream())
            using (StreamReader reader =
                new StreamReader(resStream, Encoding.UTF8))
            {
                return await reader
                    .ReadToEndAsync()
                    .ConfigureAwait(false);
            }
        }

        public static string WebdenOku(
            string webAdres,
            string postData = null,
            string cookieHeader = null,
            string cokGizliAnahtar = null,
            string contentType = "application/x-www-form-urlencoded",
            string authorization = null)
        {
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12;

            ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(
                    MyCertValidationCb);

            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create(webAdres);

            request.Proxy = null;

            bool postMu = !string.IsNullOrEmpty(postData);
            request.Method = postMu ? "POST" : "GET";

            if (!string.IsNullOrEmpty(cookieHeader))
                request.Headers.Add("Cookie", cookieHeader);

            if (!string.IsNullOrEmpty(cokGizliAnahtar))
                request.Headers.Add("X-Internal-Key", cokGizliAnahtar);

            if (!string.IsNullOrEmpty(authorization))
            {
                request.Headers.Add("Authorization", authorization);
            }

            if (postMu)
            {
                request.ContentType = contentType;

                byte[] dataBytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = dataBytes.Length;

                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(dataBytes, 0, dataBytes.Length);
                }
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream resStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        private static bool MyCertValidationCb(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //OrtakFonksiyonlar.HataDosyaYaz("WebReq.txt", "Geldim", false);
            //if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
            //{
            //    return false;
            //}
            //else if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch)
            //                == SslPolicyErrors.RemoteCertificateNameMismatch)
            //{
            //    Zone z;
            //    z = Zone.CreateFromUrl(((HttpWebRequest)sender).RequestUri.ToString());
            //    if (z.SecurityZone == System.Security.SecurityZone.Intranet
            //      || z.SecurityZone == System.Security.SecurityZone.MyComputer)
            //    {
            //        return true;
            //    }
            //    return false;
            //}
            //return false;
            return true;
        }

        public static bool EsitMi(double sayi1, double sayi2, int hassasiyet, double hata)
        {
            return Math.Abs(OrtakFonksiyonlar.Yuvarla(sayi2 - sayi1, hassasiyet)) <= hata;
        }

        public static bool EsitMi(double sayi1, double sayi2)
        {
            return EsitMi(sayi1, sayi2, 4);
        }

        public static bool EsitMi(double sayi1, double sayi2, int hassasiyet)
        {
            return OrtakFonksiyonlar.Yuvarla(sayi2 - sayi1, hassasiyet) == 0;
        }


        public static double YuzdeBul(double bolunen, double bolen, int kusurat)
        {
            if (bolen == 0) return 0;
            return Yuvarla(bolunen * 100 / bolen, kusurat);
        }

        public static decimal YuzdeHesapla(decimal deger, decimal oran)
        {
            return YuzdeHesapla(deger, oran, 2);
        }
        public static double Yuvarla(double deger, int kusurat)
        {
            return Math.Round(deger, kusurat, MidpointRounding.AwayFromZero);
        }

        public static decimal Yuvarla(decimal deger, int kusurat)
        {
            return Math.Round(deger, kusurat, MidpointRounding.AwayFromZero);
        }

        public static decimal YuzdeHesapla(decimal deger, decimal oran, int kusurat)
        {
            return Yuvarla(deger * oran / 100, kusurat);
        }

        public static double YuzdeHesapla(double deger, double oran, int kusurat)
        {
            return Yuvarla(deger * oran / 100, kusurat);
        }

        public static double KurusYap(double deger)
        {
            return Yuvarla(deger, 2);
        }

        public static decimal KurusYap(decimal deger)
        {
            return Yuvarla(deger, 2);
        }

        public static double YuvarlaAsagi(double deger, int kusurat)
        {
            double carpBol = Math.Pow(10, kusurat);
            return Yuvarla(Math.Floor(deger * carpBol) / carpBol, kusurat);
        }

        public static double YuvarlaAsagi(double deger)
        {
            return YuvarlaAsagi(deger, 0);
        }

        public static double YuvarlaYukari(double deger, int kusurat)
        {
            double carpBol = Math.Pow(10, kusurat);
            return Yuvarla(Math.Ceiling(deger * carpBol) / carpBol, kusurat);
        }

        public static double YuvarlaYukari(double deger)
        {
            return YuvarlaYukari(deger, 0);
        }

        public static string DiziElemanAl(string[] dizi, int elamanNo, string hata)
        {
            if (dizi == null) return hata;
            if (elamanNo < dizi.Length)
            {
                string tut = dizi[elamanNo];
                if (tut == null) tut = hata;
                return tut;
            }
            return hata;
        }

        public static double DiziElemanAl(double[] dizi, int elamanNo, double hata)
        {
            if (dizi == null) return hata;
            if (elamanNo < dizi.Length)
            {
                double tut = dizi[elamanNo];
                if (tut == double.NaN) tut = hata;
                return tut;
            }
            return hata;
        }

        public static string TarihYYYYMMDD(TNSDateTime d, string araChar = "")
        {
            return OrtakFonksiyonlar.MetinYap(d.Yil, 4) + araChar + OrtakFonksiyonlar.MetinYap(d.Ay, 2) + araChar + OrtakFonksiyonlar.MetinYap(d.Gun, 2);
        }
        public static string TarihYYMMDD(TNSDateTime d, string araChar = "")
        {
            return OrtakFonksiyonlar.MetinYap(d.Yil % 100, 2) + araChar + OrtakFonksiyonlar.MetinYap(d.Ay, 2) + araChar + OrtakFonksiyonlar.MetinYap(d.Gun, 2);
        }

        public static string TarihHHMM(TNSDateTime d, string araChar = "")
        {
            return OrtakFonksiyonlar.MetinYap(d.Saat, 2) + araChar + OrtakFonksiyonlar.MetinYap(d.Dakika, 2);
        }

        public static string MetinYap(double dGelen, int liraUzunluk, int kurusUzunluk)
        {
            double tam = Math.Floor(dGelen);
            int kurus = (int)Math.Round((dGelen - tam) * 100);

            string tut = tam.ToString();
            tut = CharEkle(tut, '0', liraUzunluk, ENUMMetinEkleYon.SOL, true);

            if (kurusUzunluk > 0)
            {
                tut += "." + MetinYap(kurus, kurusUzunluk);
            }
            return tut;
        }

        public static string MetinYap(int iGelen, int uzunluk)
        {
            string tut = iGelen.ToString();
            return CharEkle(tut, '0', uzunluk, ENUMMetinEkleYon.SOL, true);
        }

        public static string CharEkle(string sGelen, char ekle, int uzunluk, ENUMMetinEkleYon yon, bool kirp)
        {
            int len = sGelen.Length;
            if (len == uzunluk) return sGelen;
            if (len > uzunluk) return sGelen.Substring(0, uzunluk);
            if (yon == ENUMMetinEkleYon.SOL)
                return sGelen.PadLeft(uzunluk, ekle);
            else
                return sGelen.PadRight(uzunluk, ekle);
        }

        public static string WebConfigOku(string anahtar, string bulunamadiDeger)
        {
            string deger = System.Configuration.ConfigurationManager.AppSettings.Get(anahtar);
            if (String.IsNullOrEmpty(deger))
                deger = bulunamadiDeger;
            return deger;
        }

        public static System.Net.WebProxy ProxyAyarlaMernis()
        {
            return ProxyAyarla("Mernis");
        }

        public static System.Net.WebProxy ProxyAyarla()
        {
            return ProxyAyarla("");
        }

        public static System.Net.WebProxy ProxyAyarla(string onEk)
        {
            System.Net.WebProxy proxy = new System.Net.WebProxy();
            string proxyAddress = System.Configuration.ConfigurationManager.AppSettings.Get(onEk + "ProxyAddress") + "";
            if (proxyAddress.Trim() != "")
            {
                int proxyPort = 0;
                int portIndex = proxyAddress.IndexOf(':');
                if (portIndex > -1)
                {
                    proxyPort = OrtakFonksiyonlar.ConvertToInt(proxyAddress.Substring(portIndex + 1), 0);
                    proxyAddress = proxyAddress.Substring(0, portIndex);
                }

                string proxyUser = System.Configuration.ConfigurationManager.AppSettings.Get(onEk + "ProxyUser") + "";
                string proxyPassword = OrtakFonksiyonlar.SifreliCozOrtakAnahtar(System.Configuration.ConfigurationManager.AppSettings.Get(onEk + "ProxyPassword") + "");
                string proxyDomain = System.Configuration.ConfigurationManager.AppSettings.Get(onEk + "ProxyDomain") + "";
                if (proxyPort > 0)
                    proxy = new System.Net.WebProxy(@proxyAddress, proxyPort);
                else
                    proxy = new System.Net.WebProxy(@proxyAddress);
                if (proxyDomain.Trim() != "")
                    proxy.Credentials = new System.Net.NetworkCredential(proxyUser, proxyPassword, proxyDomain);
                else
                    proxy.Credentials = new System.Net.NetworkCredential(proxyUser, proxyPassword);
            }
            return proxy;
        }

        public static void DosyaYaz(string dosyaAd, string yazi, bool yeni)
        {
            System.Threading.Monitor.Enter(dosyaAd);
            if (!string.IsNullOrEmpty(Path.GetDirectoryName(dosyaAd)))
                Directory.CreateDirectory(Path.GetDirectoryName(dosyaAd));

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(dosyaAd, !yeni, Encoding.GetEncoding("windows-1254"));

                sw.WriteLine(yazi);
                sw.Flush();
                sw.Close();
                sw = null;
            }
            catch (Exception)
            {
                if (sw != null)
                    sw.Close();
            }
            System.Threading.Monitor.Exit(dosyaAd);
        }

        public static void DosyaYaz(string dosyaAd, string yazi, bool yeni, string encoding)
        {
            System.Threading.Monitor.Enter(dosyaAd);
            if (!string.IsNullOrEmpty(Path.GetDirectoryName(dosyaAd)))
                Directory.CreateDirectory(Path.GetDirectoryName(dosyaAd));

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(dosyaAd, !yeni, Encoding.GetEncoding(encoding));

                sw.WriteLine(yazi);
                sw.Flush();
                sw.Close();
                sw = null;
            }
            catch (Exception)
            {
                if (sw != null)
                    sw.Close();
            }
            System.Threading.Monitor.Exit(dosyaAd);
        }

        public static void DosyaYaz(string dosyaAd, byte[] yazi)
        {
            System.Threading.Monitor.Enter(dosyaAd);
            if (!string.IsNullOrEmpty(Path.GetDirectoryName(dosyaAd)))
                Directory.CreateDirectory(Path.GetDirectoryName(dosyaAd));

            try
            {
                File.WriteAllBytes(dosyaAd, yazi);
            }
            catch (Exception)
            {
            }
            System.Threading.Monitor.Exit(dosyaAd);
        }

        public static string DosyaOku(string dosyaAd)
        {
            System.Threading.Monitor.Enter(dosyaAd);
            StreamReader sr = null;
            StringBuilder sb = new StringBuilder();
            string s = "";

            try
            {
                sr = File.OpenText(dosyaAd);
                s = sr.ReadLine();
                while (s != null)
                {
                    sb.Append(s);
                    s = sr.ReadLine();
                }
                sr.Close();
                sr = null;
            }
            catch (Exception)
            {
                if (sr != null)
                    sr.Close();
            }
            System.Threading.Monitor.Exit(dosyaAd);

            return sb.ToString();
        }

        public static void DosyaSil(string dosyaAd)
        {
            System.Threading.Monitor.Enter(dosyaAd);
            File.Delete(dosyaAd);
            System.Threading.Monitor.Exit(dosyaAd);
        }

        public static void YolOlustur(string yol)
        {
            if (!Directory.Exists(yol))
                Directory.CreateDirectory(yol);
        }

        public static void DosyayaYaz(string dosyaTamAd, string yazi, bool yeni)
        {
            System.Threading.Monitor.Enter(dosyaTamAd);
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(dosyaTamAd, !yeni, Encoding.GetEncoding("windows-1254"));

                sw.WriteLine(yazi);
                sw.Flush();
                sw.Close();
                sw = null;
            }
            catch (Exception)
            {
                if (sw != null)
                    sw.Close();
            }
            System.Threading.Monitor.Exit(dosyaTamAd);
        }

        public static void HataDosyaYaz(string dosyaAd, string yazi, bool yeni = false)
        {
            string yazYol = System.Configuration.ConfigurationManager.AppSettings.Get("HataDosyasi");
            yazYol = Path.GetDirectoryName(yazYol);//hatadosyası yolu
            dosyaAd = Path.GetFileName(dosyaAd);//sadece dosya adı
            dosyaAd = yazYol + "\\" + dosyaAd;
            DosyayaYaz(dosyaAd, yazi, yeni);
        }

        public static void DebugYaz(string kullaniciKod, string yazi, bool yeni = false)
        {
            if (kullaniciKod == "stratek" || string.IsNullOrWhiteSpace(kullaniciKod))
                HataDosyaYaz("Debug.txt", new TNSDateTime(DateTime.Now).ToTimeStamp() + ":" + yazi, yeni);
        }

        public static string HataDosyaOku(string dosyaAd)
        {
            string yazYol = System.Configuration.ConfigurationManager.AppSettings.Get("HataDosyasi");
            yazYol = Path.GetDirectoryName(yazYol);//hatadosyası yolu
            dosyaAd = Path.GetFileName(dosyaAd);//sadece dosya adı
            dosyaAd = yazYol + "\\" + dosyaAd;

            System.Threading.Monitor.Enter(dosyaAd);
            StreamReader sr = null;
            StringBuilder sb = new StringBuilder();
            string s = "";

            try
            {
                sr = File.OpenText(dosyaAd);
                s = sr.ReadLine();
                while (s != null)
                {
                    sb.Append(s);
                    s = sr.ReadLine();
                }
                sr.Close();
                sr = null;
            }
            catch (Exception)
            {
                if (sr != null)
                    sr.Close();
            }
            System.Threading.Monitor.Exit(dosyaAd);

            return sb.ToString();
        }

        public static void HataDosyaSil(string dosyaAd)
        {
            string yazYol = System.Configuration.ConfigurationManager.AppSettings.Get("HataDosyasi");
            yazYol = Path.GetDirectoryName(yazYol);//hatadosyası yolu
            dosyaAd = Path.GetFileName(dosyaAd);//sadece dosya adı
            dosyaAd = yazYol + "\\" + dosyaAd;

            System.Threading.Monitor.Enter(dosyaAd);
            File.Delete(dosyaAd);
            System.Threading.Monitor.Exit(dosyaAd);
        }

        public static void HataStrYaz(string hata)
        {
            string dosyaAd = System.Configuration.ConfigurationManager.AppSettings.Get("HataDosyasi");
            if (dosyaAd != null && dosyaAd != "")
            {
                int uz = dosyaAd.Length;
                if (dosyaAd[uz - 4] == '.')
                {
                    dosyaAd = dosyaAd.Insert(uz - 4, DateTime.Now.ToString("yyyyMMdd"));
                }
                System.Threading.Monitor.Enter(dosyaAd);
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(dosyaAd, true, Encoding.GetEncoding("windows-1254"));
                    sw.Write("--------------");
                    sw.Write(DateTime.Now.ToString());
                    sw.Write("---");
                    sw.Write(DateTime.Now.ToString());
                    sw.WriteLine("--------------");
                    sw.WriteLine(hata);
                    sw.WriteLine("------------------------------------------------------");
                    sw.WriteLine("");
                    sw.Flush();
                    sw.Close();
                    sw = null;
                }
                catch (Exception)
                {
                    if (sw != null)
                        sw.Close();
                }
                System.Threading.Monitor.Exit(dosyaAd);
            }
        }

        public static string SehirAdiVer(int plaka)
        {
            if (plaka == 0 || plaka > 81)
                return "";

            string[] sehir = { "ADANA", "ADIYAMAN", "AFYONKARAHİSAR", "AĞRI", "AMASYA", "ANKARA", "ANTALYA", "ARTVİN", "AYDIN", "BALIKESİR", "BİLECİK", "BİNGÖL", "BİTLİS", "BOLU", "BURDUR", "BURSA", "ÇANAKKALE", "ÇANKIRI", "ÇORUM", "DENİZLİ", "DİYARBAKIR", "EDİRNE", "ELAZIĞ", "ERZİNCAN", "ERZURUM", "ESKİSEHİR", "GAZİANTEP", "GİRESUN", "GÜMÜŞHANE", "HAKKARİ", "HATAY", "ISPARTA", "MERSİN", "ISTANBUL", "İZMİR", "KARS", "KASTAMONU", "KAYSERİ", "KIRKLARELİ", "KİRŞEHİR", "KOCAELİ", "KONYA", "KÜTAHYA", "MALATYA", "MANİSA", "KAHRAMANMARAŞ", "MARDİN", "MUĞLA", "MUŞ", "NEVŞEHİR", "NİĞDE", "ORDU", "RİZE", "SAKARYA", "SAMSUN", "SİİRT", "SİNOP", "SİVAS", "TEKİRDAĞ", "TOKAT", "TRABZON", "TUNCELİ", "ŞANLIURFA", "UŞAK", "VAN", "YOZGAT", "ZONGULDAK", "AKSARAY", "BAYBURT", "KARAMAN", "KIRIKKALE", "BATMAN", "ŞIRNAK", "BARTIN", "ARDAHAN", "IĞDIR", "YALOVA", "KARABÜK", "KİLİS", "OSMANİYE", "DÜZCE" };

            return sehir[plaka - 1];
        }

        public static void SaatEkle(string yer, ref string tut)
        {
            tut = tut + yer + DateTime.Now.ToString("hh:mm:ss.fff") + "\r\n";
        }

        public static string Sifrele(string deger)
        {
            string str = "";
            try
            {
                byte[] bytes = System.Text.ASCIIEncoding.GetEncoding("ISO-8859-9").GetBytes(deger);
                str = Convert.ToBase64String(bytes);
            }
            catch
            {
                str = "";
            }
            return str;
        }

        public static string SifreCoz(string deger)
        {
            string str = "";
            try
            {
                byte[] bytes = System.Convert.FromBase64String(deger);
                str = System.Text.ASCIIEncoding.GetEncoding("ISO-8859-9").GetString(bytes);
            }
            catch
            {
                str = "";
            }
            return str;
        }

        public static Sonuc IBANKontrol(string iban, bool bosluklariKaldir)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (iban.Trim() == "0")
                return new Sonuc(true, "", "IBAN bilgisi doğrudur.");

            if (iban.Trim().Length > 2 && OrtakFonksiyonlar.ConvertToInt(iban.Trim().Substring(0, 2), 0) > 0)// Eğer ilk 2 karakter harf değilse kontrol yapma. Melih 18.08.2014
                return new Sonuc(true, "", "IBAN bilgisi olmadığı için kontrol edilmiyor.");


            if (bosluklariKaldir) // remove empty space & convert all uppercase
                iban = Regex.Replace(iban, @"\s", "").ToUpper();

            if (Regex.IsMatch(iban, @"\W"))
                return new Sonuc(false, string.Format("IBAN({0}) bilgisinde hatalı karakterler var.", iban), "");

            if (!Regex.IsMatch(iban, @"^\D\D\d\d.+"))
                return new Sonuc(false, string.Format("IBAN({0}) bilgisinin yapısı hatalı.", iban), "");

            if (Regex.IsMatch(iban, @"^\D\D00.+|^\D\D01.+|^\D\D99.+"))
                return new Sonuc(false, string.Format("IBAN({0}) bilgisinin kontrol karakteri hatalı.", iban), "");

            int uzunluk = 0;
            if (iban.StartsWith("TR")) uzunluk = 26;

            if (uzunluk > 0)
            {
                if (iban.Length != uzunluk)
                    return new Sonuc(false, string.Format("IBAN({0}) bilgisi {1} karakter olmalıdır.", iban, uzunluk), "");
            }

            string modifiedIban = iban.ToUpper().Substring(4) + iban.Substring(0, 4);
            modifiedIban = Regex.Replace(modifiedIban, @"\D", m => ((int)m.Value[0] - 55).ToString());
            int remainer = 0;
            while (modifiedIban.Length >= 7)
            {
                remainer = int.Parse(remainer + modifiedIban.Substring(0, 7)) % 97;
                modifiedIban = modifiedIban.Substring(7);
            }
            remainer = int.Parse(remainer + modifiedIban) % 97;

            if (remainer != 1)
                return new Sonuc(false, string.Format("IBAN({0}) bilgisi hatalı.", iban), "");

            return new Sonuc(true, "", "IBAN bilgisi doğrudur.");
            //http://www.codeproject.com/Articles/55667/IBAN-Verification-in-C-Excel-Automation-Add-in-Wor
        }

        public static string MailAt(string from, string to, string konu, string bilgi)
        {
            return MailAt(from, to, konu, bilgi, false, false, null);
        }

        static string smtpKullanici;
        static string smtpParola;
        static string smtpAdres;
        static string smtpGuvenli;
        static string smtpBase64;
        static int smtpPort;
        static SmtpClient smtpClient;

        static OrtakFonksiyonlar()
        {
            MailInit();
        }

        public static void MailInit()
        {
            smtpKullanici = System.Configuration.ConfigurationManager.AppSettings.Get("SMTPKullanici");
            smtpParola = SifreliCozOrtakAnahtar(System.Configuration.ConfigurationManager.AppSettings.Get("SMTPParola"));
            smtpAdres = System.Configuration.ConfigurationManager.AppSettings.Get("SMTPAdres");
            smtpGuvenli = System.Configuration.ConfigurationManager.AppSettings.Get("SMTPGuvenli");
            smtpBase64 = System.Configuration.ConfigurationManager.AppSettings.Get("SMTPBase64");
            smtpPort = 0;

            if (!string.IsNullOrEmpty(smtpAdres) && smtpAdres.IndexOf(":") > -1)
            {
                smtpPort = OrtakFonksiyonlar.ConvertToInt(smtpAdres.Substring(smtpAdres.IndexOf(":") + 1), 0);
                smtpAdres = smtpAdres.Substring(0, smtpAdres.IndexOf(":"));
            }

            if (smtpPort == 0)
                smtpClient = new SmtpClient(smtpAdres);
            else
                smtpClient = new SmtpClient(smtpAdres, smtpPort);

            if (OrtakFonksiyonlar.ConvertToInt(smtpGuvenli, 0) > 0)
                smtpClient.EnableSsl = true;

            if (!String.IsNullOrEmpty(smtpKullanici))
                smtpClient.Credentials = new System.Net.NetworkCredential(smtpKullanici, smtpParola);

            if (OrtakFonksiyonlar.ConvertToInt(smtpBase64, 0) > 0)
                smtpClient.UseDefaultCredentials = false;

            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        public static string MailAt(string from, string to, string konu, string bilgi, bool html, bool replyVar, TNSCollection ekDosyaIsimleri)
        {
            return MailAtHazirla(from, to, "", "", konu, bilgi, html, replyVar, ekDosyaIsimleri);
        }

        public static string MailAt(string from, string to, string cc, string bcc, string konu, string bilgi, bool html, bool replyVar, TNSCollection ekDosyaIsimleri)
        {
            return MailAtHazirla(from, to, cc, bcc, konu, bilgi, html, replyVar, ekDosyaIsimleri);
        }

        public static string MailAtHazirla(string from, string to, string cc, string bcc, string konu, string bilgi, bool html, bool replyVar, TNSCollection ekDosyaIsimleri)
        {
            if (string.IsNullOrEmpty(from))
                from = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimMailAdresi");

            string donusDeger = "";
            string mail = to;
            if (mail.IndexOf('@') != -1)
            {
                string[] mailler = mail.Split(';');

                for (int i = 0; i < mailler.Length; i++)
                {
                    if (!string.IsNullOrEmpty(mailler[i]) && MailAdresiDogrumu(mailler[i]) && !string.IsNullOrEmpty(from))
                    {
                        donusDeger = MailAtIc(from, mailler[i], cc, bcc, konu, bilgi, html, replyVar, ekDosyaIsimleri);
                    }
                }
            }
            return donusDeger;
        }

        public static string MailAtIc(string from, string to, string cc, string bcc, string konu, string bilgi, bool html, bool replyVar, TNSCollection ekDosyaIsimleri)
        {
            if (string.IsNullOrEmpty(smtpAdres))//Bazen smtp adresi boş oluyor. Bu sebeble tekrar init et
                MailInit();

            if (string.IsNullOrEmpty(smtpAdres))//Bazen smtp adresi boş oluyor. Bu sebeble tekrar init et
                return "Mail gönderebilmek için web.config dosyasında, SMTPAdres, SMTPKullanici, SMTPParola, SMTPGuvenli ve SMTPBase64 değerlerini doldurunuz ";

            string donusDeger = "";

            if (string.IsNullOrEmpty(from))
                from = System.Configuration.ConfigurationManager.AppSettings.Get("YazilimMailAdresi");

            System.Net.Mail.MailMessage mess = new System.Net.Mail.MailMessage();

            string fromAdi = "";
            if (from.IndexOf('|') > -1)
            {
                string[] fler = from.Split('|');
                if (fler.Length > 0) from = fler[0];
                if (fler.Length > 1) fromAdi = fler[1];
            }

            mess.From = new MailAddress(from, fromAdi);
            mess.Sender = new MailAddress(from, fromAdi);

            if (to != null)
            {
                to = to.Replace(" ", "");
                to = to.Replace("^", ";");
            }
            if (cc != null) cc = cc.Replace(" ", "");
            if (bcc != null) bcc = bcc.Replace(" ", "");

            int say = 0;
            string hata = MailAdressEkle(mess.To, to, ref say);
            if (!string.IsNullOrWhiteSpace(hata)) return hata;
            hata = MailAdressEkle(mess.CC, cc, ref say);
            if (!string.IsNullOrWhiteSpace(hata)) return hata;
            hata = MailAdressEkle(mess.Bcc, bcc, ref say);
            if (!string.IsNullOrWhiteSpace(hata)) return hata;
            if (say == 0)
                return "Alıcısı olmayan mesaj gönderilmeye çalışılıyor!";
            mess.Subject = konu;
            mess.Body = bilgi;

            mess.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
            mess.Priority = System.Net.Mail.MailPriority.Normal;

            if (html)
                mess.IsBodyHtml = true;

            string tmpYol = "";
            if (ekDosyaIsimleri != null && ekDosyaIsimleri.Count > 0)
            {
                System.Net.Mail.Attachment attach = null;

                for (int i = 0; i < ekDosyaIsimleri.Count; i++)
                {
                    string dosyaAd;
                    if (ekDosyaIsimleri[i].GetType() == typeof(string))
                        dosyaAd = (string)ekDosyaIsimleri[i];
                    else
                        dosyaAd = (((EPostaEki)ekDosyaIsimleri[i]).ad);

                    if (string.IsNullOrEmpty(dosyaAd)) continue;

                    if (ekDosyaIsimleri[i].GetType() == typeof(string))
                    {
                        attach = new System.Net.Mail.Attachment(dosyaAd);
                    }
                    else
                    {
                        EPostaEki ek = (EPostaEki)ekDosyaIsimleri[i];
                        if (ek.ek != null)
                        {
                            if (tmpYol == "")
                            {
                                tmpYol = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName());
                                Directory.CreateDirectory(tmpYol);
                            }
                            string tmpDosya = Path.Combine(tmpYol, DosyaAdDegistir(dosyaAd)); //Dosya adında istenmeyen karakterleri temizleyip, geçici dosya adı üretiliyor.
                            File.WriteAllBytes(tmpDosya, ((EPostaEki)ekDosyaIsimleri[i]).ek);
                            attach = new System.Net.Mail.Attachment(tmpDosya);
                        }
                        else
                        {
                            attach = new System.Net.Mail.Attachment(ek.dosyaAd);
                        }
                        attach.Name = ek.ad;
                    }

                    mess.Attachments.Add(attach);
                }
            }

            if (replyVar)
                mess.Headers.Add("Return-Path", from);

            donusDeger = "";
            try
            {
                lock (smtpAdres)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(MyCertValidationCb);
                    smtpClient.Send(mess);
                }

                donusDeger = "1";
                string dosyaAdi = "epostaGonder" + DateTime.Now.Year.ToString() + "" + DateTime.Now.Month.ToString("00") + "Gidenler.txt";
                HataDosyaYaz(dosyaAdi, DateTime.Now.ToString() + "<-to:" + mess.To + ";cc:" + cc + ";bcc:" + bcc + "-> " + mess.Subject, false);
            }
            catch (Exception ex)
            {
                string hataIc = ex.Message;
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                {
                    hataIc += " (" + ex.InnerException.Message + ")";
                }
                donusDeger = "Mail gönderilemedi: " + hataIc + " bilgiler-> sunucu:" + smtpClient.Host + " From:" + from + " Email Adresi:" + "<-to:" + mess.To + ";cc:" + cc + ";bcc:" + bcc;
                string dosyaAdi = "epostaGonder" + DateTime.Now.Year.ToString() + "" + DateTime.Now.Month.ToString("00") + "Hata.txt";
                HataDosyaYaz(dosyaAdi, DateTime.Now.ToString() + " <=Gönderilemedi=> " + donusDeger + "-" + smtpAdres + "-" + smtpKullanici + "-" + smtpParola, false);
            }

            mess.Dispose();

            try
            {
                if (!string.IsNullOrEmpty(tmpYol))
                    Directory.Delete(Path.GetDirectoryName(tmpYol), true);
            }
            catch { OrtakFonksiyonlar.HataStrYaz("Posta Gönderme ek klasörü silme hatası" + tmpYol); }


            return donusDeger;
        }

        public static string MailAtThread(string from, string to, string cc, string bcc, string konu, string bilgi, bool html, bool replyVar, TNSCollection ekDosyaIsimleri)
        {
            try
            {
                //to kısmında gelen maillerde aynı mail geçiyorsa elemek için
                //****************************************************************
                string[] kisiler = to.Split(';');
                for (int j = 0; j < kisiler.Length; j++)
                {
                    if (kisiler[j] != "")
                    {
                        for (int i = j + 1; i < kisiler.Length; i++)
                        {
                            if (kisiler[i] == kisiler[j]) kisiler[i] = "";
                        }
                    }
                }
                to = "";
                for (int i = 0; i < kisiler.Length; i++)
                {
                    if (kisiler[i] == "") continue;
                    if (to != "") to += ";";
                    to += kisiler[i];
                }
                //****************************************************************

                ParameterizedThreadStart paraThread = new ParameterizedThreadStart(MailAtThread);
                Thread th = new Thread(paraThread);
                th.Name = "MailAtThread";
                Object obj = new object[] { from, to, cc, bcc, konu, bilgi, html, replyVar, ekDosyaIsimleri };
                th.Start(obj);
                return "1";
            }
            catch (Exception ex)
            {
                OrtakFonksiyonlar.HataStrYaz("{7895D27E-C6C2-440C-99D3-77155CE531BB} - Mail Gönderme Hatası; Kimden: " + from + ", Kime: " + to + ", Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            return "0";
        }

        public static void MailAtThread(object obj)
        {
            try
            {
                Object[] liste = (Object[])obj;

                string from = (string)liste[0];
                string to = (string)liste[1];
                string cc = (string)liste[2];
                string bcc = (string)liste[3];
                string konu = (string)liste[4];
                string bilgi = (string)liste[5];
                bool html = (bool)liste[6];
                bool replyVar = (bool)liste[7];
                TNSCollection ekDosyaIsimleri = (TNSCollection)liste[8];

                OrtakFonksiyonlar.MailAt(from, to, cc, bcc, konu, bilgi, html, replyVar, ekDosyaIsimleri);

            }
            catch (ThreadAbortException ex)
            {
                OrtakFonksiyonlar.HataStrYaz("{8AAB495F-9CDB-42D6-8D64-76DD022B97E2} - MailAtThread1 Gönderme Hatası,  Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
        }

        private static string MailAdressEkle(MailAddressCollection mailAddressCollection, string cc, ref int say)
        {
            string[] ccler = cc.Split(';');
            foreach (string gc in ccler)
            {
                string to = gc;
                string toAdi = "";

                if (to.IndexOf('|') > -1)
                {
                    string[] fler = to.Split('|');
                    if (fler.Length > 0) to = fler[0];
                    if (fler.Length > 1) toAdi = fler[1];
                }

                if (!string.IsNullOrEmpty(to))
                {
                    if (OrtakFonksiyonlar.MailAdresiDogrumu(to))
                    {
                        mailAddressCollection.Add(new MailAddress(to, toAdi));
                        say++;
                    }
                    else
                        return "Hatalı mail adresi:" + to;
                }
            }
            return "";
        }

        private static string DosyaAdDegistir(string p)
        {
            string[] degistir = { "/", "\\", ":", "?", "\"", "*", "<", ">", "|" };
            foreach (string dg in degistir)
            {
                p = p.Replace(dg, "");
            }
            return p;
        }

        public static void StringiIkiyeBol(string parcalanacak, string aranan, ref string parca1, ref string parca2)
        {
            if (string.IsNullOrWhiteSpace(parcalanacak))
            {
                parca1 = "";
                parca2 = "";
            }
            else
            {
                int yer = parcalanacak.IndexOf(aranan);
                if (yer >= 0)
                {
                    parca1 = parcalanacak.Substring(0, yer);
                    parca2 = parcalanacak.Substring(yer + aranan.Length);
                }
                else
                {
                    parca1 = parcalanacak;
                    parca2 = "";
                }
            }
        }

        // Kod tablosu kod yapısına bağlı olarak (örn: 99.99.99.99) kod alanına noktaların yerleştirilmesini
        // ve geriye oluşturulan katarın döndürülmesini sağlar.
        public static string KodNoktaKoy(string kodyapi, string kod)
        {
            if (String.IsNullOrEmpty(kod))
                return "";

            bool noktaVar = true;
            int pYapi = 0;
            int pKod = 0;
            string donusDeger = "";
            while (pYapi < kodyapi.Length)
            {
                if (kodyapi[pYapi] == '.')
                {
                    donusDeger += '.';
                    pYapi++;
                    noktaVar = true;
                }
                else
                {
                    if (pKod < kod.Length)
                    {
                        donusDeger += kod[pKod];
                        pKod++;
                        pYapi++;
                        noktaVar = false;
                    }
                    else
                        break;
                }
            }
            if (pKod < kod.Length)
            {
                if (!noktaVar)
                    donusDeger += '.';
                donusDeger += kod.Substring(pKod);
                noktaVar = false;
            }
            if (noktaVar && donusDeger.Length > 0)
                donusDeger = donusDeger.Remove(donusDeger.Length - 1, 1);
            return donusDeger;
        }

        public static int StringiParcala(string parcalanacak, string aranan, ObjectArray dizi)
        {
            bool devam = true;
            int indis = 0;
            string parca1 = "", parca2 = "";
            while (devam)
            {
                if (parcalanacak == "")
                    devam = false;
                else
                {
                    StringiIkiyeBol(parcalanacak, aranan, ref parca1, ref parca2);
                    dizi.Ekle(parca1);
                    parcalanacak = parca2;
                    indis++;
                }
            }
            return indis;
        }

        public static string IlkHarflerBuyuk(string cumle)
        {
            if (string.IsNullOrEmpty(cumle)) return "";

            char[] aranan = new char[] { ' ', '/', '.' };
            int yer = 0;

            string buyukHarfli = cumle.Trim().ToLower(new CultureInfo("tr-TR", false));
            if (buyukHarfli.Length > 0)
                buyukHarfli = buyukHarfli.Substring(0, 1).ToUpper(new CultureInfo("tr-TR", false)) + buyukHarfli.Substring(1);

            for (int i = 0; i < aranan.Length; i++)
            {
                char aranacak = (char)aranan.GetValue(i);
                bool devam = true;
                while (devam)
                {
                    yer = buyukHarfli.IndexOf(aranacak, yer + 1);
                    if (yer > 0 && yer < buyukHarfli.Length - 1)
                        buyukHarfli = buyukHarfli.Substring(0, yer + 1) + buyukHarfli.Substring(yer + 1, 1).ToUpper(new CultureInfo("tr-TR", false)) + buyukHarfli.Substring(yer + 2);
                    else
                        devam = false;
                }
            }
            return buyukHarfli;
        }

        private static bool HataliKarakterVar(string s)
        {
            char c;

            for (int i = 0; i < s.Length; i++)
            {
                c = s[i];
                if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '.' || c == '-' || c == '_'))
                    return true;
            }
            return false;
        }

        public static bool MailAdresiDogrumu(string mailAdres, bool domainKontroluYap = false)
        {
            if (domainKontroluYap && !DomainGecerliMi(mailAdres))
            {
                return false;
            }
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            bool deger = re.IsMatch(mailAdres);
            if (!deger)
                deger = mailAdres.EndsWith("local") || mailAdres.EndsWith("localhost") || mailAdres.EndsWith("istanbul") || mailAdres.EndsWith("london");

            return deger;
        }

        public static string gecerliDomainlerStr { get; set; } = null;
        static string[] gecerliDomainler = null;

        private static bool DomainGecerliMi(string mailAdres)
        {
            string kim = "", domain = "";
            StringiIkiyeBol(mailAdres, "@", ref kim, ref domain);
            if (string.IsNullOrWhiteSpace(domain))
                return false;
            if (gecerliDomainlerStr == null)
            {
                gecerliDomainlerStr = OrtakFonksiyonlar.WebConfigOku("GecerliDomainler", "");
                if (string.IsNullOrWhiteSpace(gecerliDomainlerStr))
                    gecerliDomainler = null;
                else
                    gecerliDomainler = gecerliDomainlerStr.Split(',', ';');
            }
            if (gecerliDomainler == null || gecerliDomainler.Length == 0) return true;
            foreach (string item in gecerliDomainler)
            {
                if (item == domain)
                    return true;
            }
            return false;
        }

        public static int ConvertToInt(string degisecek, int hataDeger)
        {
            int donusDeger;
            try
            {
                donusDeger = Convert.ToInt32(degisecek);
            }
            catch (Exception)
            {
                donusDeger = hataDeger;
            }
            return donusDeger;
        }

        public static int ConvertToInt(decimal degisecek, int hataDeger)
        {
            int donusDeger;
            try
            {
                donusDeger = Convert.ToInt32(degisecek);
            }
            catch (Exception)
            {
                donusDeger = hataDeger;
            }
            return donusDeger;
        }

        public static double ConvertToDouble(string degisecek, double hataDeger)
        {
            double donusDeger;
            try
            {
                donusDeger = Convert.ToDouble(degisecek);
            }
            catch (Exception)
            {
                donusDeger = hataDeger;
            }
            return donusDeger;
        }

        public static double ConvertToDouble(string s)
        {
            try
            {
                return Convert.ToDouble(s);
            }
            catch
            {
                return 0;
            }
        }

        public static double ConvertToDouble(object s)
        {
            try
            {
                return Convert.ToDouble(s);
            }
            catch
            {
                return 0;
            }
        }

        public static double ConvertToDbl(string s)
        {
            try
            {
                return Convert.ToDouble(s);
            }
            catch
            {
                return 0;
            }
        }

        public static double ConvertToDbl(object s)
        {
            try
            {
                return Convert.ToDouble(s);
            }
            catch
            {
                return 0;
            }
        }

        public static decimal ConvertToDecimal(object s)
        {
            try
            {
                return Convert.ToDecimal(s);
            }
            catch
            {
                return 0;
            }
        }

        public static int ConvertToInt(object s, int deger)
        {
            try
            {
                return Convert.ToInt32(s);
            }
            catch
            {
                return deger;
            }
        }

        public static long ConvertToLong(object s)
        {
            try
            {
                return Convert.ToInt64(s);
            }
            catch
            {
                return 0;
            }
        }

        public static string ConvertToStr(object s)
        {
            try
            {
                return Convert.ToString(s);
            }
            catch
            {
                return "";
            }
        }

        public static decimal ConvertToDecimal(string degisecek, decimal hataDeger)
        {
            decimal donusDeger;
            try
            {
                donusDeger = Convert.ToDecimal(degisecek);
            }
            catch (Exception)
            {
                donusDeger = hataDeger;
            }
            return donusDeger;
        }

        public static string HesapPlaniKurum(string kurum)
        {
            string rKurum;

            if (kurum.Replace(".", "").Substring(0, 2) == "38")
                rKurum = "38";
            else if (OrtakFonksiyonlar.ConvertToInt(kurum.Replace(".", "").Substring(0, 2), 0) >= 40 && OrtakFonksiyonlar.ConvertToInt(kurum.Replace(".", "").Substring(0, 2), 0) < 46)
                rKurum = kurum.Replace(".", "").Substring(0, 4);
            else if (kurum.Replace(".", "").Substring(0, 2) == "46")
                rKurum = kurum.Replace(".", "").Substring(0, 6);
            else
                rKurum = "12";

            return rKurum;
        }

        public static string YaziylaParasiz(double sayi)
        {
            return YaziylaIc(sayi, null, false);
        }

        public static string YaziylaParasiz(int sayi)
        {
            return YaziylaParasiz((double)sayi);
        }

        public static string YaziylaParasizSifirli(double sayi)
        {
            return YaziylaIc((double)sayi, null, false);
        }

        public static string YaziylaParasizSifirli(int sayi)
        {
            return YaziylaIc((double)sayi, null, false);
        }

        public static string Yaziyla(double sayi)
        {
            return YaziylaIc(sayi, new ParaIsmi("TL", true, "KR", true), false);
        }

        public static string Yaziyla(double sayi, ParaIsmi paraIsmi)
        {
            return YaziylaIc(sayi, paraIsmi, false);
        }

        private static string YaziylaIc(double sayi, ParaIsmi paraIsmi, bool sifirBos)
        {
            return YaziylaTR.YaziylaIcTR(sayi, paraIsmi, sifirBos);
        }

        public static string RomaRakamiyla(int sayi)
        {
            if (sayi <= 0 || sayi > 3999) return sayi.ToString();
            string donusDeger = "";

            for (int i = 3; i >= 0; i--)
            {
                int us = (int)Math.Pow(10, i);
                int cevir = sayi / us;
                sayi = sayi - us * cevir;
                string donen = RomaRakami(cevir, i);
                if (donen == null) return sayi.ToString();
                donusDeger += donen;
            }
            return donusDeger;
        }

        private static string[,] RomaAnaRakamlari = { { "I", "V" }, { "X", "L" }, { "C", "D" }, { "M", null } };

        private static string RomaRakami(int cevir, int us)
        {
            if (cevir == 0) return "";
            if (us < 0 || us > 3) return null;
            if (cevir <= 0 || cevir > 9) return null;
            string bir = RomaAnaRakamlari[us, 0];
            string bes = RomaAnaRakamlari[us, 1];
            string on = null;
            if (us < 3)
                on = RomaAnaRakamlari[us + 1, 0];
            if (cevir <= 3)
                return bir.PadRight(cevir, bir[0]);
            else
            {
                if (string.IsNullOrWhiteSpace(bes)) return null;
                if (cevir == 4) return bir + bes;
                else if (cevir == 5) return bes;
                else if (cevir >= 6 && cevir < 9) return bes + bir.PadRight(cevir - 5, bir[0]);
                else
                {
                    if (string.IsNullOrWhiteSpace(on)) return null;
                    return bir + on;
                }
            }
        }

        public static string YaziylaParasizEN(double sayi)
        {
            return YaziylaIcEN(sayi, null, false);
        }

        public static string YaziylaParasizEN(int sayi)
        {
            return YaziylaParasizEN((double)sayi);
        }

        public static string YaziylaParasizSifirliEN(double sayi)
        {
            return YaziylaIcEN((double)sayi, null, false);
        }

        public static string YaziylaParasizSifirliEN(int sayi)
        {
            return YaziylaIcEN((double)sayi, null, false);
        }

        public static string YaziylaEN(double sayi)
        {
            return YaziylaIcEN(sayi, new ParaIsmi("USD", true, "CENT", true), false);
        }

        public static string YaziylaEN(double sayi, ParaIsmi paraIsmi)
        {
            return YaziylaIcEN(sayi, paraIsmi, false);
        }

        private static string YaziylaIcEN(double sayi, ParaIsmi paraIsmi, bool sifirBos)
        {
            return YaziyaCevirEN.YaziylaIcEN(sayi, paraIsmi, sifirBos);
        }

        public static bool TertipKiyasla(string sablonTertip, string tertip)
        {
            sablonTertip = sablonTertip.Replace(".", "");
            tertip = tertip.Replace(".", "");

            ObjectArray st = new ObjectArray();
            ObjectArray t = new ObjectArray();
            int gSay;
            int stSay = OrtakFonksiyonlar.StringiParcala(sablonTertip, "-", st);
            int tSay = OrtakFonksiyonlar.StringiParcala(tertip, "-", t);
            if (stSay > tSay) gSay = stSay;
            else gSay = tSay;
            string stp, tp;
            for (int i = 0; i < gSay; i++)
            {
                if (i < stSay) stp = (string)st.objeler[i];
                else stp = "";

                if (i < tSay) tp = (string)t.objeler[i];
                else tp = "";

                if (stp == "*") stp = "";
                if (stp.Length != 0)
                {
                    if (tp.Length < stp.Length)
                        return false;
                    if (tp.Substring(0, stp.Length) != stp)
                        return false;
                }
            }
            return true;
        }

        public static bool TertipKiyaslaCoklu(string sablonTertip, string cokluTertip)
        {
            ObjectArray d = new ObjectArray();
            int parca = OrtakFonksiyonlar.StringiParcala(cokluTertip, ";", d);
            foreach (string tertipParca in d.objeler)
            {
                if (TertipKiyasla(sablonTertip, tertipParca))
                    return true;
            }
            return false;
        }

        public static string DigerKurumlarBasla()
        {
            return "99";
        }

        public static string BelediyeBasla()
        {
            return "46";
        }

        public static string BelediyeBit()
        {
            return "48";
        }

        public static bool BelediyeMi(string kurumsalKod)
        {
            if (kurumsalKod == null) return false;
            string icKurumsalKod = kurumsalKod.Replace(".", "");
            if (icKurumsalKod.Length < 6)
                return false;
            icKurumsalKod = icKurumsalKod.Substring(0, 2);
            if (icKurumsalKod.CompareTo(BelediyeBasla()) >= 0 && icKurumsalKod.CompareTo(BelediyeBit()) < 0)
                return true;
            else
                return false;
        }

        public static string KurumsaldanKurumaNoktali(string kurumsalKod)
        {
            if (string.IsNullOrWhiteSpace(kurumsalKod))
                return "";
            string[] kurumsalKodlar = kurumsalKod.Split('.');
            if (kurumsalKodlar.Length > 0 & kurumsalKodlar.Length < 3)
                return kurumsalKodlar[0];
            return KurumsaldanKuruma(kurumsalKod, null);
        }

        public static string KurumsaldanKuruma(string kurumsalKod, string kurYapi)
        {
            if (string.IsNullOrEmpty(kurYapi))
                kurYapi = "99.99.99.99";

            if (string.IsNullOrEmpty(kurumsalKod))
                return "";

            kurumsalKod = OrtakFonksiyonlar.KodNoktaKoy(kurYapi, kurumsalKod.Replace(".", ""));
            if (kurYapi == "99.99.99.99")
            {
                int l = kurumsalKod.Length;
                if (l < 2)
                    return "";
                if (l == 2)
                    return kurumsalKod;
                if (l == 3)
                    return kurumsalKod.Substring(0, 2);
                if (l > 3)
                {
                    string kodYapi;
                    kurumsalKod = kurumsalKod.Replace(".", "");
                    if (BelediyeMi(kurumsalKod))
                    {
                        kodYapi = "99.99.99";
                        kurumsalKod = kurumsalKod.Substring(0, 6);
                    }
                    else if (kurumsalKod.Substring(0, 2).CompareTo(GenelButceSonKod()) >= 0 || kurumsalKod.Substring(2, 2).CompareTo(BagimsizKurumBaslangic()) >= 0)
                    {
                        kodYapi = "99.99";
                        kurumsalKod = kurumsalKod.Substring(0, 4);
                    }
                    else
                    {
                        kodYapi = "99";
                        kurumsalKod = kurumsalKod.Substring(0, 2);
                    }
                    return OrtakFonksiyonlar.KodNoktaKoy(kodYapi, kurumsalKod);
                }
            }
            else
            {
                string[] kurumsalKodlar = kurumsalKod.Split('.');
                if (kurumsalKodlar.Length > 0)
                    return kurumsalKodlar[0];
            }
            return "";
        }

        public static string BagimsizKurumBaslangic()
        {
            return "75";
        }

        public static string GenelButceSonKod()
        {
            return "31";
        }

        public static string YetkiTipiDegeriBul(HashTableArrayList yetkiTipiDegerleri, string yetkiTipiAdi)
        {
            try
            {
                return yetkiTipiDegerleri[yetkiTipiAdi] + "";
            }
            catch { return ""; }
        }

        public static int KodUzunlukBul(string kodYapi, int seviye)
        {
            int karSay = -1;
            for (int i = 0; i < seviye; i++)
                karSay = kodYapi.IndexOf('.', karSay + 1);
            if (karSay == -1)
                karSay = kodYapi.Length;
            return kodYapi.Substring(0, karSay).Replace(".", "").Length;
        }

        public static Sonuc KodYapiKontrol(string kodYapisiNoktali, string kodNoktasiz, bool uzunlukOnemli)
        {
            Sonuc sonuc = new Sonuc();
            char[] nokta = new char[1];
            nokta[0] = '.';

            if (kodYapisiNoktali == string.Empty)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Kod yapısı boş olamaz.";
                return sonuc;
            }

            string[] kyArray = kodYapisiNoktali.Split(nokta);
            string kodYapisiNoktasiz = kodYapisiNoktali.Replace(".", "");

            if (kodNoktasiz.Length > kodYapisiNoktasiz.Length)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Kod, kod yapısından uzun olamaz!";
                return sonuc;
            }

            if (uzunlukOnemli)
            {
                if (kodNoktasiz.Length != kodYapisiNoktasiz.Length)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Kod ve yapı ( " + kodYapisiNoktali + " ) uzunluğu eşit değil!";
                    return sonuc;
                }
            }

            string kodNoktali = KodNoktaKoy(kodYapisiNoktali, kodNoktasiz);
            string[] kArray = kodNoktali.Split(nokta);

            for (int i = 0; i < kArray.Length; i++)
            {
                if (kyArray[i].Length != kArray[i].Length)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Kod yapıya ( " + kodYapisiNoktali + " ) uygun değil!";
                    return sonuc;
                }
            }

            sonuc.islemSonuc = true;
            sonuc.hataStr = "Kod yapıya uygun.";
            return sonuc;
        }

        public static ListeSatir[] EnumListeDondur(Type t)
        {
            if (t == null)
                return new ListeSatir[0];

            Array eNames = System.Enum.GetNames(t);

            if (eNames.Length <= 0)
                return new ListeSatir[0];

            ListeSatir[] elemanlar = new ListeSatir[eNames.Length];

            char[] c = { '_' };

            int sayac = 0;

            foreach (string s in eNames)
            {
                string[] sAdDizi = s.Split(c);
                string sAd = string.Empty;

                for (int i = 0; i < sAdDizi.Length; i++)
                {
                    sAd += sAdDizi[i];
                    if (i != sAdDizi.Length - 1)
                        sAd += " ";
                }

                string sDeger = OrtakFonksiyonlar.ConvertToInt(Enum.Parse(t, s), 0).ToString();

                elemanlar[sayac] = new ListeSatir(sAd, sDeger);

                sayac++;
            }

            return elemanlar;
        }

        public static ListeSatir[] EnumListeDondur(Type t, char ayirac)
        {
            if (t == null)
                return new ListeSatir[0];

            Array eNames = System.Enum.GetNames(t);

            if (eNames.Length <= 0)
                return new ListeSatir[0];

            ListeSatir[] elemanlar = new ListeSatir[eNames.Length];

            char[] c = { ayirac };

            int sayac = 0;

            foreach (string s in eNames)
            {
                string[] sAdDizi = s.Split(c);
                string sAd = string.Empty;

                for (int i = 0; i < sAdDizi.Length; i++)
                {
                    sAd += sAdDizi[i];
                    if (i != sAdDizi.Length - 1)
                        sAd += " ";
                }

                string sDeger = OrtakFonksiyonlar.ConvertToInt(Enum.Parse(t, s), 0).ToString();

                elemanlar[sayac] = new ListeSatir(sAd, sDeger);

                sayac++;
            }

            return elemanlar;
        }
        public static string SifreleOrtakAnahtar(string sifrelenecek)
        {
            return Sifrele(sifrelenecek, AnahtarBul());
        }

        public static string Sifrele(string sifrelenecek, string anahtar)
        {
            anahtar = SifreAnahtarVer(anahtar);

            byte[] sifrelenecekDizi = UTF8Encoding.UTF8.GetBytes(sifrelenecek);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(anahtar);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] sonucDizi = cTransform.TransformFinalBlock(sifrelenecekDizi, 0, sifrelenecekDizi.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(sonucDizi, 0, sonucDizi.Length);
        }

        public static string SifreliCozOrtakAnahtar(string sifreli)
        {
            if (sifreli == null) sifreli = "";
            try
            {
                return SifreliCoz(sifreli, AnahtarBul());
            }
            catch
            {
                return "";
            }
        }

        static private string webConfigAnahtar = null;

        private static string AnahtarBul()
        {
            if (webConfigAnahtar == null)
                webConfigAnahtar = WebConfigOku("Anahtar", "");

            //Anahtar yoksa şifreleme yapılmamıştır. Melih 07.10.2019
            if (webConfigAnahtar == "")
                return "";

            return "STRATEK2002!" + webConfigAnahtar;
        }

        public static string SifreliCoz(string sifreli, string anahtar)
        {
            //Anahtar yoksa şifreleme yapılmamıştır. Melih 07.10.2019
            if (string.IsNullOrWhiteSpace(anahtar))
                return sifreli;

            anahtar = SifreAnahtarVer(anahtar);
            byte[] sifreliDizi = Convert.FromBase64String(sifreli);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(anahtar);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] sonucDizi = cTransform.TransformFinalBlock(sifreliDizi, 0, sifreliDizi.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(sonucDizi);
        }

        private static string SifreAnahtarVer(string anahtar)
        {
            anahtar = anahtar.Trim();
            if (anahtar.Length > 0 && anahtar.Length < 16)
                anahtar = anahtar.PadRight(16, '0');
            else if (anahtar.Length > 16)
                anahtar = anahtar.Substring(0, 16);

            return anahtar;
        }

        public static int DonemBul(int ay)
        {
            return ((ay - 1) / 3) + 1;
        }

        public static string AyAdiGetir(int ayno)
        {
            string ayAdi = "";
            if (ayno == 1) ayAdi = "Ocak";
            else if (ayno == 2) ayAdi = "Şubat";
            else if (ayno == 3) ayAdi = "Mart";
            else if (ayno == 4) ayAdi = "Nisan";
            else if (ayno == 5) ayAdi = "Mayıs";
            else if (ayno == 6) ayAdi = "Haziran";
            else if (ayno == 7) ayAdi = "Temmuz";
            else if (ayno == 8) ayAdi = "Ağustos";
            else if (ayno == 9) ayAdi = "Eylül";
            else if (ayno == 10) ayAdi = "Ekim";
            else if (ayno == 11) ayAdi = "Kasım";
            else if (ayno == 12) ayAdi = "Aralık";
            return ayAdi;
        }

        public static string RemoveHTML(string text)
        {
            string cleanString = text;
            //Iterate through the whole text as long as it have '<' and '>'
            while (cleanString.Contains("<") && cleanString.Contains(">"))
            {
                //Getting the indices of '<' and '>'.
                int openIndx = cleanString.IndexOf('<');
                int closeIndx = cleanString.IndexOf('>');
                //Surrounding the '<' and '>' with '^' and '~' respectively.
                cleanString = cleanString.Insert(openIndx, "^");
                cleanString = cleanString.Insert(closeIndx + 2, "~");
                //Getting the indices of '^' and '~'.
                openIndx = cleanString.IndexOf('^');
                closeIndx = cleanString.IndexOf('~');
                /*Clean all the text between '^' and '~'. This is meant in order to make a place holder in the text that will be replaced with a space.*/
                cleanString = cleanString.Remove(openIndx, closeIndx - openIndx + 1);
                //Replacing the resulting "^~" placeholder with a space.
                cleanString = cleanString.Replace("^~", " ");
            }
            //Cleaning the text from "\n".
            cleanString = cleanString.Replace("\n", " ");
            //Cleaning the text from heading and trailing spaces.
            cleanString = cleanString.TrimStart().TrimEnd();

            return cleanString; //Return the all clean text.
        }

        public static bool TCKNDogrula(string tcKimlikNo)
        {
            //http://blog.rakkoc.com/2014/09/tc-kimlik-no-algoritmasi-ve-hesaplamasi/
            bool returnvalue = false;
            if (tcKimlikNo.Length == 11)
            {
                Int64 atcNo, btcNo, tcNo;
                long c1, c2, c3, c4, c5, c6, c7, c8, c9, q1, q2;

                tcNo = Int64.Parse(tcKimlikNo);

                // bolu yuz islemi int tanimlanmis degiskende son 2 haneyi silmek icin kullanılır.

                atcNo = tcNo / 100;
                btcNo = tcNo / 100;

                c1 = atcNo % 10; atcNo = atcNo / 10;
                c2 = atcNo % 10; atcNo = atcNo / 10;
                c3 = atcNo % 10; atcNo = atcNo / 10;
                c4 = atcNo % 10; atcNo = atcNo / 10;
                c5 = atcNo % 10; atcNo = atcNo / 10;
                c6 = atcNo % 10; atcNo = atcNo / 10;
                c7 = atcNo % 10; atcNo = atcNo / 10;
                c8 = atcNo % 10; atcNo = atcNo / 10;
                c9 = atcNo % 10; atcNo = atcNo / 10;
                q1 = ((10 - ((((c1 + c3 + c5 + c7 + c9) * 3) + (c2 + c4 + c6 + c8)) % 10)) % 10);
                q2 = ((10 - (((((c2 + c4 + c6 + c8) + q1) * 3) + (c1 + c3 + c5 + c7 + c9)) % 10)) % 10);

                /*
                q1 TC nosunun 10. hanesi
                q2 TC nosunun 11. hanesi
                btcNo son 2 hanesi olmayan tckimlikNo
                */

                returnvalue = ((btcNo * 100) + (q1 * 10) + q2 == tcNo);
            }
            return returnvalue;
        }

        public static string ParaBicimle(double tutar, string kultur = "tr")
        {
            return string.Format(new CultureInfo(kultur), "{0:C2}", tutar);
        }

        public static string ParaBicimle(decimal tutar, string kultur = "tr")
        {
            return ParaBicimle((double)tutar, kultur);
        }

        private static string _geciciYol = null;
        private static readonly object _geciciYolLock = new object();

        public static string GeciciYolBul()
        {
            if (_geciciYol != null)
                return _geciciYol;

            lock (_geciciYolLock)
            {
                if (_geciciYol != null)
                    return _geciciYol;

                string hataDosyasi = System.Configuration.ConfigurationManager.AppSettings.Get("HataDosyasi");
                string yazYol = System.IO.Path.GetDirectoryName(hataDosyasi);
                yazYol = System.IO.Path.Combine(yazYol, "Temp");
                System.IO.Directory.CreateDirectory(yazYol);
                _geciciYol = yazYol;
                return _geciciYol;
            }
        }

        public static string GeciciDosyaAdBul()
        {
            return System.IO.Path.Combine(GeciciYolBul(), Guid.NewGuid().ToString("N") + ".tmp");
        }

        //public static string VarsayilanKurumBul()
        //{
        //    DEGServis degServis = TNS.DEG.Arac.Tanimla();
        //    return degServis.DegiskenDegerBul(0, "/KURUMKOD");
        //}
    }

    public class ParaIsmi
    {
        private string lira;
        private bool liraSonra;

        private string kurus;
        private bool kurusSonra;

        public ParaIsmi(string lira, bool liraSonra, string kurus, bool kurusSonra)
        {
            this.lira = lira;
            this.liraSonra = liraSonra;
            this.kurus = kurus;
            this.kurusSonra = kurusSonra;
        }

        private string Ekle(string yazi, string ek, bool sonra)
        {
            if (sonra) return yazi + " " + ek;
            else return ek + " " + yazi;
        }

        public string LiraEkle(string yazi)
        {
            if (!string.IsNullOrEmpty(yazi)) return Ekle(yazi, lira, liraSonra);
            return yazi;
        }

        public string KurusEkle(string yazi)
        {
            if (!string.IsNullOrEmpty(yazi)) return Ekle(yazi, kurus, kurusSonra);
            return yazi;
        }
    }

    public static class ResmiTatil
    {
        private static Hashtable yillar = new Hashtable();

        public static bool HatfaSonuTatiliMi(DateTime tarih)
        {
            return (tarih.DayOfWeek == DayOfWeek.Saturday || tarih.DayOfWeek == DayOfWeek.Sunday);
        }

        private static TNSCollection TatilDizisiBul(int yil)
        {
            TNSCollection tatiller = (TNSCollection)yillar[yil];
            if (tatiller == null)
            {
                tatiller = ResmiTatilOku(yil);
                yillar[yil] = tatiller;
            }
            return tatiller;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tarih">Tatil mi olduğu bulunacak gün</param>
        /// <param name="haftaSonuDurumu">0 ise hafta sonunu resmi tatil kabul eder, 1 ise hafta kontrolü yapılmaz, 2 ise hafta sonunu resmi tatil kabul etmez.</param>
        /// <returns></returns>
        public static bool ResmiTatilMi(DateTime tarih, int haftaSonuDurumu)
        {
            if (HatfaSonuTatiliMi(tarih))
            {
                if (haftaSonuDurumu == 0) return true;
                if (haftaSonuDurumu == 2) return false;
            }
            return ResmiTatilMi(TatilDizisiBul(tarih.Year), tarih);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tatiller">Tatil günlerini tutan dizi</param>
        /// <param name="tarih">Tatil mi olduğu bulunacak gün</param>
        /// <returns></returns>
        private static bool ResmiTatilMi(TNSCollection tatiller, DateTime tarih)
        {
            if (tatiller != null)
            {
                foreach (DateTime item in tatiller)
                {
                    if (item.Date == tarih.Date)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static TNSCollection ResmiTatilOku(int yil)
        {
            TNSCollection tc = new TNSCollection();
            string xmlResmiTatilYolDosya = System.Configuration.ConfigurationManager.AppSettings.Get("ResmiTatil") + "";
            if (string.IsNullOrEmpty(xmlResmiTatilYolDosya))
            {
                OrtakFonksiyonlar.HataDosyaYaz("ResmiTatil.txt", "Resmi tatil dosyası bulunamadı!", true);
                return tc;
            }
            try
            {
                XmlDocument xmlResmiTatil = new XmlDocument();
                xmlResmiTatil.Load(xmlResmiTatilYolDosya);
                XmlElement resmiTatiller = xmlResmiTatil["RESMITATIL"];
                var girisNokta = resmiTatiller.ChildNodes;
                for (int i = 0; i < girisNokta.Count; i++)
                {
                    XmlAttribute urlAttr = girisNokta[i].Attributes["DEGER"];
                    int okuYil = OrtakFonksiyonlar.ConvertToInt(urlAttr.InnerText, -1);
                    if (okuYil == 0)
                    {
                        GunleriEkle(tc, yil, girisNokta[i].ChildNodes);
                    }
                    else if (okuYil == yil)
                    {
                        GunleriEkle(tc, yil, girisNokta[i].ChildNodes);
                    }
                }
            }
            catch (Exception ex)
            {
                OrtakFonksiyonlar.HataDosyaYaz("ResmiTatil.txt", "Resmi tatil dosyası okunamadı:" + ex.Message, true);
                return null;
            }
            return tc;
        }

        private static void GunleriEkle(TNSCollection tc, int yil, XmlNodeList xmlNodeList)
        {
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                try
                {
                    int ay = OrtakFonksiyonlar.ConvertToInt(xmlNodeList[i].Attributes["AY"].InnerText, 0);
                    int gun = OrtakFonksiyonlar.ConvertToInt(xmlNodeList[i].Attributes["GUN"].InnerText, 0);
                    int sure = OrtakFonksiyonlar.ConvertToInt(xmlNodeList[i].Attributes["SURE"].InnerText, 0);

                    DateTime dt = new DateTime(yil, ay, gun);

                    for (int j = 0; j < sure; j++)
                    {
                        if (tc.IndexOf(dt) < 0)
                            tc.Add(dt);
                        dt = dt.AddDays(1);
                    }
                }
                catch { }
            }
        }

        public static TNSDateTime IlkCalismaGununuVer(TNSDateTime tarih, bool kontrolYap)
        {
            while (ResmiTatilMi(tarih, 0))
                tarih.AddDays(1);

            int haftaninGunu = tarih.HaftaninGunu;
            if (haftaninGunu == 6)
                tarih.AddDays(2);
            else if (haftaninGunu == 0)
                tarih.AddDays(1);

            if (kontrolYap)
                IlkCalismaGununuVer(tarih, false);

            return tarih;
        }

        public static int GunBul(DateTime tarih1, DateTime tarih2, bool sonGunDahil, bool calismaGunu)
        {
            if (tarih1 > tarih2)
            {
                DateTime tut = tarih1;
                tarih1 = tarih2.Date;
                tarih2 = tut.Date;
            }
            else
            {
                tarih1 = tarih1.Date;
                tarih2 = tarih2.Date;
            }
            if (sonGunDahil)
                tarih2 = tarih2.AddDays(1);

            TimeSpan farkts = tarih2 - tarih1;
            int fark = farkts.Days;
            if (!calismaGunu) return fark;
            int farkHafta = fark / 7;
            int farkGun = fark - farkHafta * 7;
            int tatilSay = farkHafta * 2;
            DateTime tarih3 = tarih2;
            for (int i = 0; i < farkGun; i++)
            {
                tarih3 = tarih3.AddDays(-1);
                if (HatfaSonuTatiliMi(tarih3)) tatilSay++;
            }

            for (int y = tarih1.Year; y <= tarih2.Year; y++)
            {
                tatilSay += ResmiTatilGunBul(TatilDizisiBul(y), tarih1, tarih2);
            }
            return fark - tatilSay;
        }

        private static int ResmiTatilGunBul(TNSCollection tatiller, DateTime tarih1, DateTime tarih2)
        {
            int say = 0;
            foreach (DateTime item in tatiller)
            {
                if (!HatfaSonuTatiliMi(item))
                    if (TarihIcindeMi(item, tarih1, tarih2)) say++;
            }
            return say;
        }

        public static int KesisimBul(DateTime donem1Bas, DateTime donem1Son, DateTime donem2Bas, DateTime donem2Son, bool calismaGunu, out DateTime basTar, out DateTime bitTar)
        {
            basTar = DateTime.MinValue;
            if (TarihIcindeMi(donem1Bas, donem2Bas, donem2Son))
                basTar = donem1Bas;
            else if (TarihIcindeMi(donem2Bas, donem1Bas, donem1Son))
                basTar = donem2Bas;
            if (basTar == DateTime.MinValue)
            {
                bitTar = DateTime.MinValue;
                return 0;
            }
            if (TarihIcindeMi(donem1Son.AddDays(-1), donem2Bas, donem2Son))
                bitTar = donem1Son;
            else
                bitTar = donem2Son;
            return GunBul(basTar, bitTar, false, calismaGunu);
        }

        public static int KesisimGunBul(DateTime donem1Bas, DateTime donem1Son, DateTime donem2Bas, DateTime donem2Son, bool calismaGunu)
        {
            DateTime basTar;
            DateTime bitTar;
            return KesisimBul(donem1Bas, donem1Son, donem2Bas, donem2Son, calismaGunu, out basTar, out bitTar);
        }

        public static bool TarihIcindeMi(DateTime tarih, DateTime dbas, DateTime dson)
        {
            if (tarih == DateTime.MinValue) return false;
            if (tarih < dbas) return false;
            if (dson == DateTime.MinValue) return true;
            if (tarih < dson) return true;
            return false;
        }

        public static bool ResmiTatilMi(TNSDateTime tarih, int haftaSonuDurumu)
        {
            return ResmiTatilMi(tarih.Oku(), haftaSonuDurumu);
        }

        public static void Yenile()
        {
            yillar = new Hashtable();
        }
    }

    /// <summary>
    /// iki tarih arasındaki süreyi yıl bilgisi ile birlikte hesaplayan sınıf.
    /// </summary>
    public class TarihFark
    {
        private DateTime baslaTarih;
        private DateTime bitisTarih;

        private int yil;
        private int ay;
        private int gun;

        public static void FarkHesaplaYas(DateTime this_baslaTarih, DateTime this_bitisTarih, out int y, out int a, out int g)
        {
            int elde = 0;
            g = this_bitisTarih.Day - this_baslaTarih.Day;
            if (g < 0)
            {
                g += DateTime.DaysInMonth(this_baslaTarih.Year, this_baslaTarih.Month);
                elde = 1;
            }

            a = this_bitisTarih.Month - elde - this_baslaTarih.Month;
            if (a < 0)
            {
                a += 12;
                elde = 1;
            }
            else
                elde = 0;
            y = this_bitisTarih.Year - elde - this_baslaTarih.Year;
        }


        private static void FarkHesaplaGIB(DateTime d1, DateTime d2, out int y, out int a, out int g, bool otuzuAyYap)
        {
            if (AyinSonGunuMu(d2) && (AyinSonGunuMu(d1) || !AydaGunVarMi(d1.Day, d2)))
            {
                FarkHesaplaYas(new DateTime(d1.Year, d1.Month, 1), new DateTime(d2.Year, d2.Month, 1), out y, out a, out g);
            }
            else if (AyinSonGunuMu(d1))
            {
                DateTime oncAySonGun = d2.AddMonths(-1);
                oncAySonGun = new DateTime(oncAySonGun.Year, oncAySonGun.Month, DateTime.DaysInMonth(oncAySonGun.Year, oncAySonGun.Month));

                FarkHesaplaYas(new DateTime(d1.Year, d1.Month, 1), new DateTime(oncAySonGun.Year, oncAySonGun.Month, 1), out y, out a, out g);
                TimeSpan ts = d2 - oncAySonGun;
                g = ts.Days;
            }
            else
            {
                FarkHesaplaYas(d1, d2, out y, out a, out g);
                DateTime d11 = d1.AddYears(y).AddMonths(a);
                TimeSpan ts = d2 - d11;
                g = ts.Days;
            }
            if (otuzuAyYap)
            {
                if (g >= 30)
                {
                    g = 0;
                    a++;
                    if (a == 12)
                    {
                        a = 0;
                        y++;
                    }
                }
            }
        }

        private static bool AydaGunVarMi(int day, DateTime d2)
        {
            return day <= DateTime.DaysInMonth(d2.Year, d2.Month);
        }

        private static DateTime IlkGun(DateTime d1)
        {
            return new DateTime(d1.Year, d1.Month, 1);
        }

        public static bool AyinSonGunuMu(DateTime d1)
        {
            if (d1.Day == DateTime.DaysInMonth(d1.Year, d1.Month)) return true;
            return false;
        }

        public TarihFark(DateTime d1, DateTime d2) : this(d1, d2, false)
        {
        }

        public TarihFark(DateTime d1, DateTime d2, bool GIBYontem, bool yerdegistir = true, bool otuzuAyYap = true)
        {
            if (d1 > d2 && yerdegistir)
            {
                this.baslaTarih = d2;
                this.bitisTarih = d1;
            }
            else
            {
                this.baslaTarih = d1;
                this.bitisTarih = d2;
            }
            if (this.baslaTarih < this.bitisTarih)
            {
                if (GIBYontem)
                    FarkHesaplaGIB(this.baslaTarih, this.bitisTarih, out yil, out ay, out gun, otuzuAyYap);
                else
                    FarkHesaplaYas(this.baslaTarih, this.bitisTarih, out yil, out ay, out gun);
            }
            else
            {
                yil = 0;
                ay = 0;
                gun = 0;
            }
        }

        public override string ToString()
        {
            return this.yil + " Yıl, " + this.ay + " ay, " + this.gun + " gün";
        }
        public int YilAy { get { return this.yil * 12 + ay; } }
        public int Yil { get { return this.yil; } }
        public int Ay { get { return this.ay; } }
        public int Gun { get { return this.gun; } }
    }

    public class SureOlcer
    {
        StringBuilder sb = new StringBuilder();
        DateTime sonAn;
        public SureOlcer(string bilgi)
        {
            sonAn = DateTime.Now;
            sb.AppendLine(bilgi + " başladı: (sn):" + sonAn.ToString("#0.000"));
        }

        public void IslemEkle(string bilgi)
        {
            DateTime simdi = DateTime.Now;
            TimeSpan ts = simdi - sonAn;
            sb.AppendLine(bilgi + "\t süre (sn):" + ts.TotalSeconds.ToString("#0.000"));
            sonAn = simdi;
        }

        public string SureSonuc()
        {
            return sb.ToString();
        }
    }
}
