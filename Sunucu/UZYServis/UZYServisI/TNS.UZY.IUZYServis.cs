using System;
using TNS.KYM;
using System.Data;
using System.Collections;

namespace TNS.UZY
{
    /// <summary>
    /// Bu sunucu sınıfı için, istemci ve sunucuda ortak kullanılacak statik fonksiyonlar içeren sınıf
    /// </summary>
    public class Arac
    {
        public static IUZYServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("UZYServisURL");
            IUZYServis servis = TNS.Api.ServisIstemci.Olustur<IUZYServis>(sUrl);
            return servis;
        }

        public static string UzayKosulOl(params string[] args)
        {
            string donusDeger = "";
            for (int i = 0; i < args.Length; i++)
            {
                if (i != 0)
                    donusDeger += "-";
                donusDeger += args[i];
            }
            return donusDeger;
        }
    }

    [Serializable()]
    public class UzayAlan
    {
        public string ad = "";
        public string maske = "";
        public bool zorunlu;
        public bool kurumLimit;
        public bool muhasebatAy;
        public bool bol2005;
        public string xllAd = "";
        public string xllYardim = "";
        public UzayAlan(string p_ad, string p_maske, string p_zorunlu, string p_kurumLimit, string p_bol2005, string p_xllAd, string p_xllYardim, string s_muhasebatAy)
        {
            if (p_ad == null)
                ad = "";
            else
                ad = p_ad;

            if (p_xllAd == null)
                xllAd = "";
            else
                xllAd = p_xllAd;

            if (p_xllYardim == null)
                xllYardim = "";
            else
                xllYardim = p_xllYardim;

            if (p_maske == null)
                maske = "";
            else
                maske = p_maske;

            if (maske.StartsWith("D_"))
            {
                maske = TNS.DEG.Arac.DegiskenDegerBulCache(0, "KODYAPI/KOD" + maske.Substring(2) + "/KODYAPI");
            }

            if (maske == null)
                maske = "";

            if (p_kurumLimit == null || p_kurumLimit == "")
                kurumLimit = false;
            else
                kurumLimit = true;

            if (p_zorunlu == null || p_zorunlu == "")
                zorunlu = false;
            else
                zorunlu = true;
            if (p_bol2005 == null || p_bol2005 == "")
                bol2005 = false;
            else
                bol2005 = true;

            muhasebatAy = !string.IsNullOrWhiteSpace(s_muhasebatAy);
        }

        public string MaskeBul(string yilStr, string baglantiAd)
        {
            if (maske.StartsWith("Y_") || maske.StartsWith("V_"))
            {
                string kodNo = maske.Substring(2);
                int yil = 0;
                if (maske.StartsWith("Y_"))
                {
                    yil = OrtakFonksiyonlar.ConvertToInt(yilStr, 0);
                }
                else
                {
                    if (!string.IsNullOrEmpty(baglantiAd) && baglantiAd.Length >= 4)
                        yil = OrtakFonksiyonlar.ConvertToInt(baglantiAd.Substring(baglantiAd.Length - 4), 0);
                    else
                        yil = 0;
                }
                return TNS.DEG.Arac.DegiskenDegerBulCache(yil, "KODYAPI/KOD" + kodNo + "/KODYAPI");
            }
            else
                return maske;
        }
    }

    [Serializable()]
    public class Uzay
    {
        public int tur;
        public string ad = "", from = "", where = "", secAlan = "";
        public string tip = "N";
        public string veriTabaniParamAd;
        public string xllAd = "", xllKategori = "", xllYardim = "";
        TNSCollection alanlar = new TNSCollection();
        public string dllAd, nesneAd, ekParam;

        public Uzay(string p_ad, string p_tip, string p_XLLAd, string p_XLLKategori, string p_XLLYardim, string p_DLLAd, string p_NesneAd, string p_EkParam, string p_VeriTabaniParamAd, string gereksiz)
        {
            tur = 2;
            ad = p_ad;
            xllAd = p_XLLAd;
            xllYardim = p_XLLYardim;
            xllKategori = p_XLLKategori;

            dllAd = p_DLLAd;
            nesneAd = p_NesneAd;
            ekParam = p_EkParam;

            if (ad == null) ad = "";

            if (string.IsNullOrEmpty(p_tip))
                tip = "N";
            else
                tip = p_tip;

            if (xllAd == null) xllAd = "";
            if (xllYardim == null) xllYardim = "";
            if (xllKategori == null) xllKategori = "";

            if (dllAd == null) dllAd = "";
            if (nesneAd == null) nesneAd = "";
            if (ekParam == null) ekParam = "";
            veriTabaniParamAd = p_VeriTabaniParamAd;
            if (veriTabaniParamAd == null) veriTabaniParamAd = "";
        }

        public Uzay(string p_ad, string p_from, string p_where, string p_secAlan, string p_tip, string p_VeriTabaniParamAd, string p_xllAd, string p_xllKategori, string p_xllYardim)
        {
            tur = 1;
            ad = p_ad;
            from = p_from;
            where = p_where;
            secAlan = p_secAlan;
            veriTabaniParamAd = p_VeriTabaniParamAd;
            xllAd = p_xllAd;
            xllKategori = p_xllKategori;
            xllYardim = p_xllYardim;

            if (ad == null) ad = "";
            if (from == null) from = "";
            if (where == null) where = "";
            if (secAlan == null) secAlan = "";
            if (veriTabaniParamAd == null) veriTabaniParamAd = "";
            if (xllAd == null) xllAd = "";
            if (xllKategori == null) xllKategori = "";
            if (xllYardim == null) xllYardim = "";

            if (p_tip == null || p_tip == "")
                tip = "N";
            else
                tip = p_tip;
        }

        public void Add(UzayAlan ua)
        {
            alanlar.Add(ua);
        }

        public UzayAlan AlanBul(int i)
        {
            if (i < AlanSay())
                return (UzayAlan)alanlar[i];
            else
                return null;
        }

        public UzayAlan AlanBul(string alanAd)
        {
            for (int i = 0; i < AlanSay(); i++)
            {
                UzayAlan ua = (UzayAlan)alanlar[i];
                if (ua.ad == alanAd || ua.xllAd == alanAd || ua.ad.EndsWith("." + alanAd))
                    return ua;
            }
            return null;
        }

        public int AlanSay()
        {
            return alanlar.Count;
        }
    }

    [Serializable()]
    public class UzaySorguBilgi
    {
        public string uzayAd = "";
        public string kosul = "";
        public bool esitle = false;
        public string tur = "N";
        public string alanAd = null;
    }

    public interface IUZYServis
    {
        double UzayDegeriDbl(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle);
        double UzayDegeriDbl(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle, string alanAd);
        double UzayDegeriDbl(Kullanici kullanan, object veriErisim, string uzayAd, string uzayKosul, bool esitle, string alanAd);

        string UzayDegeriStr(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle);
        string UzayDegeriStr(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle, string alanAd);
        string UzayDegeriStr(Kullanici kullanan, object veriErisim, string uzayAd, string uzayKosul, bool esitle, string alanAd);

        TNSDateTime UzayDegeriTarih(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle);
        TNSDateTime UzayDegeriTarih(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle, string alanAd);
        TNSDateTime UzayDegeriTarih(Kullanici kullanan, object veriErisim, string uzayAd, string uzayKosul, bool esitle, string alanAd);

        ObjectArray UzayDegeriToplu(Kullanici kullanan, TNSCollection uzaySorgulari);//UzaySorguBilgi

        TNSCollection UzaylariListele();
        int UzaylariYenile();

        string KosulOlustur(string tabloOnEk, string vtAlanKod, string maske, string kodSorgu, bool esitle, bool kurumLimitiEkle);
        string UzayKosulStr(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle, string alanAd);
        System.Data.DataTable UzayDataTableDondur(Kullanici kullanan, string uzayAd, string uzayKosul, bool esitle, string alanAd, string[] ekAlanlar);
        System.Data.DataTable UzayDataTableDondur(Kullanici kullanan, string uzayAd, string[] uzayKosul, bool esitle, string alanAd, string[] ekAlanlar);

        DataTable SorguDondur(string sql, string baglantiStr, int sayfa, int sayfaBoy);

        /// <summary>
        /// Verilen sql sorgularını çalıştırır, anahtar alandaki değerleri aynı olan satırları tek bir nesnede birleştirir.
        /// </summary>
        /// <param name="sql">Birleşek sql dizisi</param>
        /// <param name="baglantiStr">Her bir sql için sql'İn bağlanacağı veri tabanı bağlantısı</param>
        /// <param name="anahtarAlanlar">Hangi alanların anahtar olduğu, bu alanları eşit olanlar aynı satırda birleştirilir.</param>
        /// <returns></returns>
        System.Collections.SortedList SorguBirlestir(string[] sql, string[] baglantiStr, int[] anahtarAlanlar);

        Sonuc SQLCalistir(Kullanici kullanan, string baglantiStr, string[] sqller);

        // Veri tabanı index işlemleri (eski EVRServis'ten; Evrak/EvrakVeritabaniIndex sayfası). Yalnızca sistem yöneticisi.
        VeritabaniIndex[] VeritabaniIndexListele(Kullanici kullanan, VeritabaniIndex form, string baglantiAd);
        Sonuc VeritabaniIndexIslem(Kullanici kullanan, VeritabaniIndex[] liste, ENUMVeritabaniIndexIslem islem, string baglantiAd);
        /// <summary>Anahtar: veri tabanı adı (Catalog), değer: appSettings'teki bağlantı satırı adı.</summary>
        Hashtable VeritabaniBaglantiSatiriListele(Kullanici kullanan);
    }

    /// <summary>Veri tabanı index bilgisi (eski TNS.EVR.VeritabaniIndex).</summary>
    [Serializable()]
    public class VeritabaniIndex
    {
        public int objectId = 0;
        public string tabloAd = "";
        public string indexAd = "";
        public string indexTip = "";
        public double indexSize = 0.0;
        public double fragmentation = 0.0;
        public int fillFactor = 0;

        public string kolon = "";
        public string include = "";
    }

    public enum ENUMVeritabaniIndexIslem
    {
        TANIMSIZ = 0,
        REBUILD = 1,
        REORGANIZE = 2,
        DROP = 3,
        CREATE = 4,
    }

    [Serializable()]
    public class HTArray : IComparable //: IEqualityComparer
    {
        public ArrayList al = new ArrayList();

        public void Add(object p)
        {
            al.Add(p);
        }

        public int ObjeSayisi { get { return al.Count; } }

        public int IntAl(int indis)
        {
            try
            {
                if (indis < al.Count)
                {
                    return Convert.ToInt32(al[indis]);
                }
            }
            catch { }
            return 0;
        }

        public double DblAl(int indis)
        {
            try
            {
                if (indis < al.Count)
                {
                    return Convert.ToDouble(al[indis]);
                }
            }
            catch { }
            return 0;
        }

        public Decimal DecAl(int indis)
        {
            try
            {
                if (indis < al.Count)
                {
                    return Convert.ToDecimal(al[indis]);
                }
            }
            catch { }
            return 0;
        }

        public string StrAl(int indis)
        {
            try
            {
                if (indis < al.Count)
                {
                    return Convert.ToString(al[indis]);
                }
            }
            catch { }
            return null;
        }

        public TNSDateTime DTAl(int indis)
        {
            try
            {
                if (indis < al.Count)
                {
                    DateTime dtt = Convert.ToDateTime(al[indis]);
                    return new TNSDateTime(dtt);
                }
            }
            catch { }
            return new TNSDateTime();
        }

        public object ObjAl(int indis)
        {
            if (indis < al.Count)
            {
                return al[indis];
            }
            return null;
        }

        public int CompareTo(object obj)
        {
            if (obj is HTArray)
            {
                HTArray gelen = (HTArray)obj;
                for (int i = 0; i < Math.Max(gelen.ObjeSayisi, this.ObjeSayisi); i++)
                {
                    object o_bu = this.ObjAl(i);
                    if (o_bu is System.DBNull) o_bu = null;
                    object o_o = gelen.ObjAl(i);
                    if (o_o is System.DBNull) o_o = null;
                    int esitMi = 0;
                    if (o_bu != null && o_o != null)
                    {
                        IComparable bu = (IComparable)this.ObjAl(i);
                        IComparable o = (IComparable)gelen.ObjAl(i);
                        esitMi = bu.CompareTo(o);
                    }
                    else if (o_o == null && o_bu == null) esitMi = 0;
                    else if (o_bu != null) esitMi = 1;
                    else esitMi = -1;
                    if (esitMi != 0) return esitMi;
                }
                return 0;
            }
            return -1;
        }
    }

    public interface IUzayHesapla
    {
        double UzayDegeriDbl(Kullanici kullanan, Object veriErisim, Uzay u, string uzayKosul, bool esitle);
        string UzayDegeriStr(Kullanici kullanan, Object veriErisim, Uzay u, string uzayKosul, bool esitle);
        TNSDateTime UzayDegeriTarih(Kullanici kullanan, Object veriErisim, Uzay u, string uzayKosul, bool esitle);
    }

    public class VTSorgula
    {
        string sql = "";
        string baglantiStr = "";
        int sayfaBuyukluk = 0;
        int sayfa = 1;
        IUZYServis servis = null;
        DataTable dataTable = null;
        int satirNo;
        DataRow aktifRow = null;

        public VTSorgula(string sql, string baglantiStr, int sayfaBuyukluk)
        {
            this.sql = sql;
            this.baglantiStr = baglantiStr;
            this.sayfaBuyukluk = sayfaBuyukluk;
            servis = Arac.Tanimla();
        }

        public DataRow satirOku()
        {
            try
            {
                if (dataTable == null)
                {
                    dataTable = servis.SorguDondur(sql, baglantiStr, sayfa, sayfaBuyukluk);
                    sayfa++;
                    satirNo = 0;
                }
                else
                {
                    if (satirNo >= dataTable.Rows.Count)
                    {
                        if (sayfaBuyukluk <= 0) dataTable = null;
                        else
                        {
                            dataTable = servis.SorguDondur(sql, baglantiStr, sayfa, sayfaBuyukluk);
                            sayfa++;
                            satirNo = 0;
                        }
                    }
                }
                if (dataTable == null || dataTable.Rows.Count <= satirNo) aktifRow = null;
                else aktifRow = dataTable.Rows[satirNo++];
            }
            catch (Exception ex)
            {
                aktifRow = null;
                this.hata = ex.Message;
            }
            return aktifRow;
        }

        public int IntAl(int kolNo)
        {
            if (aktifRow == null) return 0;
            try
            {
                return Convert.ToInt32(aktifRow[kolNo]);
            }
            catch { }
            return 0;
        }

        public double DoubleAl(int kolNo)
        {
            if (aktifRow == null) return 0;
            try
            {
                return Convert.ToDouble(aktifRow[kolNo]);
            }
            catch { }
            return 0;
        }

        public DateTime DTAl(int kolNo)
        {
            if (aktifRow == null) return DateTime.MinValue;
            try
            {
                return Convert.ToDateTime(aktifRow[kolNo]);
            }
            catch { }
            return DateTime.MinValue;
        }

        public string StringAl(int kolNo)
        {
            if (aktifRow == null) return "";
            return aktifRow[kolNo].ToString();
        }

        public string hata { get; set; }
    }
}
