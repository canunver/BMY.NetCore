using System.Data;
using TNS.TMM;
using TNS.Veri;

/// <summary>
/// Stok Hareket bilgilerini tutan TASAMBARSTOKVIEW veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
/// </summary>
public class TABStokHareket : TABOrtak
{
    /// <summary>
    /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
    /// </summary>
    /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
    public TABStokHareket(VeriErisim veriErisim)
        : base(veriErisim)
    {
    }

    /// <summary>
    /// Tablo adını tutan sabit değişken
    /// </summary>
    static string TABLOAD = "TASAMBARSTOKVIEW";

    /// <summary>
    /// Tablo alanlarının sırasını tutan enumeration
    /// </summary>
    public enum ENUMAlanlar : int
    {
        /// <summary>
        /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        /// </summary>
        YIL = 0,
        /// <summary>
        /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        /// </summary>
        MUHASEBEKOD = 1,
        /// <summary>
        /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        /// </summary>
        HARCAMABIRIMKOD = 2,
        /// <summary>
        /// Hesap plan kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        /// </summary>
        HESAPPLANKOD = 3,
        /// <summary>
        /// Hesap plan adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        /// </summary>
        HESAPPLANAD = 4,
        /// <summary>
        /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        /// </summary>
        AMBARKOD = 5,
        /// <summary>
        /// İşlem türü alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        /// </summary>
        ISLEMTUR = 6,
        /// <summary>
        /// Miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        /// </summary>
        MIKTAR = 7,
        /// <summary>
        /// Kdv oranı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        /// </summary>
        KDVORAN = 8,
        /// <summary>
        /// Birim fiyat alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        /// </summary>
        BIRIMFIYAT = 9,
        /// <summary>
        /// Son tarih alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        /// </summary>
        SONTARIH = 10,
        FISNO = 11,
        ISLEMTIPIKOD = 12
    }

    /// <summary>
    /// Tablo alanlarının isimlerini tutan sabit dizi
    /// </summary>
    static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "HESAPPLANKOD", "HESAPPLANAD", "AMBARKOD", "ISLEMTUR", "MIKTAR", "KDVORAN", "BIRIMFIYAT", "SONTARIH", "FISNO", "ISLEMTIPIKOD" };

    //public IDataReader AmbardaVarMi(StokHareketBilgi bilgi)
    //{
    //    string sql = "";

    //    return veriErisim.DataReaderDondur(sql);
    //}

    /// <summary>
    /// Parametre olarak verilen Stok Hareket bilgisini kıstas olarak kullanarak,
    /// kıstasa uygun Ambarda bulunan Taşınır malzemelerinin bilgisini dönen fonksiyon
    /// </summary>
    /// <param name="bilgi">Stok Hareket bilgisini tutan, StokHareketBilgi türünde nesne</param>
    /// <param name="girisCikis">Giriş-Çıkışla ilgili koşulun eklenip eklenmeyeceğini belirten, Bool türünde nesne</param>
    /// <returns>Select komutunun çalışması sonucunda, ambardaki stok hareketleri bilgisini tutan IDataReader türünde nesne döndürür</returns>
    public IDataReader AmbardakileriGetir(StokHareketBilgi bilgi, bool girisCikis)
    {
        string sql = string.Empty;

        //Erdalsql = "SELECT HESAPPLANKOD, HESAPPLANAD, MIKTAR, BIRIMFIYAT, KDVORAN, SONTARIH FROM " + TABLOAD;
        sql = "SELECT HESAPPLANKOD, HESAPPLANAD, MIKTAR, BIRIMFIYAT, KDVORAN, OLCUBIRIMAD FROM " + TABLOAD;
        //Yıl koşulunu melih açtı.Tüektim malzemeleri yıl içinde kontrol ediliyor.
        sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", bilgi.harcamaKod.Replace(".", "")));

        if (bilgi.ambarKod.Trim() != "")
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", bilgi.ambarKod));

        if (bilgi.hesapPlanKod.Replace(".", "").Trim() != "")
        {
            string[] yler = bilgi.hesapPlanKod.Split(';');
            string kosul = "";
            for (int i = 0; i < yler.Length; i++)
            {
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "LIKE", yler[i].Replace(".", "") + "%"));
                //sql = VeriErisim.KosulEkle(sql, "AND",  VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "LIKE", bilgi.hesapPlanKod.Replace(".", "").Trim() + "%"));
            }
            if (kosul != "")
                sql += " AND (" + kosul + ")";
        }
        if (bilgi.hesapPlanAd.Trim() != "")
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPPLANAD], "LIKE", "%" + bilgi.hesapPlanAd.Trim() + "%"));

        if (bilgi.fisNo.Trim() != "")
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", bilgi.fisNo.Trim()));

        if (bilgi.khkHaric)
        {
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTIPIKOD], "<>", 26));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTIPIKOD], "<>", 27));
        }

        if (girisCikis)
        {
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "<=", (int)ENUMIslemTipi.ACILIS));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], ">", (int)ENUMIslemTipi.BELIRSIZ));
        }
        else
        {
            sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], ">=", (int)ENUMIslemTipi.TUKETIMCIKIS));
            sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", (int)ENUMIslemTipi.DEGERARTTIR));
            sql += ")";
        }
        if (bilgi.birimFiyat > 0)
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMFIYAT], "=", bilgi.birimFiyat));

        if (bilgi.kdvOran > 0)
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KDVORAN], "=", bilgi.kdvOran));

        //if (bilgi.kdvOran > -1)
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KDVORAN], "=", bilgi.kdvOran));

        if (!bilgi.sonTarih.isNull)
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SONTARIH], ">=", bilgi.sonTarih));

        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "DURUM", "=", (int)ENUMBelgeDurumu.ONAYLI));

        sql += " ORDER BY HESAPPLANKOD, FISNO, SIRANO";

        return veriErisim.DataReaderDondur(sql);
    }

    /// <summary>
    /// Parametre olarak verilen Stok Hareket bilgilerini kıstas olarak kullanarak,
    /// kıstasa uygun olarak ilgili fiş numarasından sonra varsa hareketleri döndüren,
    /// sql komutunu oluşturup,çalıştıran ve sonucunu döndüren fonksiyon.
    /// </summary>
    /// <param name="bilgi">Stok Hareket Bilgisini tutan, StokHareketBilgi türünde nesne</param>
    /// <param name="fisNo">Fiş No bilgisini tutan, String türünde nesne</param>
    /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun stok hareket bilgilerini tutan IDataReader türünde nesne döndürür</returns>
    public IDataReader SonrasiVarMi(StokHareketBilgi bilgi, string fisNo)
    {
        string sql = string.Empty;

        sql = "SELECT * FROM " + TABLOAD;

        sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", bilgi.harcamaKod.Replace(".", "")));
        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", bilgi.ambarKod));
        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "=", bilgi.hesapPlanKod.Replace(".", "")));
        sql += " AND (";
        sql = VeriErisim.KosulEkle(sql, "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", (int)ENUMIslemTipi.TUKETIMCIKIS));
        sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", (int)ENUMIslemTipi.DEVIRCIKIS));
        sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", (int)ENUMIslemTipi.DAGITIMIADECIKIS));
        sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", (int)ENUMIslemTipi.BAGISCIKIS));
        sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", (int)ENUMIslemTipi.SATISCIKIS));
        sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", (int)ENUMIslemTipi.KULLANILMAZCIKIS));
        sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", (int)ENUMIslemTipi.DEGERARTTIR));
        sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", (int)ENUMIslemTipi.ENFLASYONARTISI));
        sql += ")";
        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "FISNO", ">", fisNo));
        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "DURUM", "=", (int)ENUMBelgeDurumu.ONAYLI));
        if (bilgi.birimFiyat > 0)
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMFIYAT], "=", bilgi.birimFiyat));
        if (bilgi.kdvOran > -1)
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KDVORAN], "=", bilgi.kdvOran));

        return veriErisim.DataReaderDondur(sql);
    }
}
