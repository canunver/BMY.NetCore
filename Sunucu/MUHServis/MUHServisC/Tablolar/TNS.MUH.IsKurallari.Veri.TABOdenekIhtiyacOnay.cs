using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHODENEKIHTIYACONAY veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABOdenekIhtiyacOnay : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOdenekIhtiyacOnay(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHODENEKIHTIYACONAY";

        enum ENUMAlanlar
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            ASAMA = 2,
            ONAY = 3,
            KAYITTARIHI = 4,
            KAYITEDEN = 5
        }
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "ASAMA", "ONAY", "KAYITTARIHI", "KAYITEDEN" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(OdenekIhtiyacOnay o)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(o.yil) + ", ";
            sql += VeriErisim.DegerYap(o.muhasebe) + ", ";
            sql += VeriErisim.DegerYap(o.asama) + ", ";
            sql += VeriErisim.DegerYap(o.onay) + ", ";
            sql += VeriErisim.DegerYap(o.kayitTarihi) + ", ";
            sql += VeriErisim.DegerYap(o.kayitEden) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(OdenekIhtiyacOnay o)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", o.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", o.muhasebe));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="o">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(OdenekIhtiyacOnay o)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", o.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", o.muhasebe));
            if (o.asama > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ASAMA], "=", o.asama));
            if (o.onay > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ONAY], "=", o.onay));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="o">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns></returns>
        public IDataReader ListeleKayitYapmayanBirimler(OdenekIhtiyacOnay o)
        {
            string sql = "SELECT DISTINCT      BIRIMKOD " +
                            "FROM         MUHMUHASEBEBIRIM " +
                            "WHERE     (BIRIMKOD NOT IN " +
                            "(SELECT     MUHODENEKIHTIYAC.BIRIMKOD " +
                            "FROM          MUHODENEKIHTIYAC INNER JOIN " +
                                                   "MUHODENEKIHTIYACONAY ON MUHODENEKIHTIYAC.MUHASEBEKOD = MUHODENEKIHTIYACONAY.MUHASEBEKOD AND  " +
                                                   "MUHODENEKIHTIYAC.YIL = MUHODENEKIHTIYACONAY.YIL " +
                            "WHERE      (MUHODENEKIHTIYACONAY.ASAMA = 1) AND (MUHODENEKIHTIYACONAY.ONAY = 0) AND " +
                                                   "(MUHODENEKIHTIYAC.MUHASEBEKOD = 6150))) AND (MUHASEBEKOD = 6150) AND (KURKOD NOT LIKE '1276%') AND " +
                            "(BIRIMKOD NOT LIKE '7%')";
            return veriErisim.DataReaderDondur(sql);
        }

    }
}