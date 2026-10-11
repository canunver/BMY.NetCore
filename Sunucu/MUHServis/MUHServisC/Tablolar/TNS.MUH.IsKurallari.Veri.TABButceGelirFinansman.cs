using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// ABSBUTCE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABButceGelirFinansman : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABButceGelirFinansman(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "ABSBUTCE";
        enum ENUMAlanlar
        {
            YIL = 0,
            KURKOD = 1,
            FONKOD = 2,
            FINKOD = 3,
            EKOKOD = 4,
            KANUN = 5,
            KBO = 6,
            EKLENEN = 7,
            DUSULEN = 8,
            SERBEST = 9,
        }
        static string[] ALANLAR = { "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "KANUN", "KBO", "EKLENEN", "DUSULEN", "SERBEST" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ButceGelirFinansman bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";
            if (bilgi.tur == 1)
                TABLOAD += "GELIR";
            else
                TABLOAD += "FINANSMAN";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.yil) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.kur) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.eko) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.kanun) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.kbo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.eklenen) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.dusulen) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.serbest) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET " + ALANLAR[(int)ENUMAlanlar.KANUN] + "='" + bilgi.kanun + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.KBO] + "='" + bilgi.kbo + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.EKLENEN] + "='" + bilgi.eklenen + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.DUSULEN] + "='" + bilgi.dusulen + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.SERBEST] + "='" + bilgi.serbest + "'";
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil);
                sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", bilgi.kur);
                sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", bilgi.eko);

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(ButceGelirFinansman bilgi)
        {
            string sql = "";
            if (bilgi.tur == 1)
                TABLOAD += "GELIR";
            else
                TABLOAD += "FINANSMAN";

            sql = "DELETE FROM " + TABLOAD;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", bilgi.kur);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", bilgi.eko);

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(ButceGelirFinansman bilgi)
        {
            string sql = "";
            if (bilgi.tur == 1)
                TABLOAD += "GELIR";
            else
                TABLOAD += "FINANSMAN";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil);
            if (bilgi.kur != "")
                sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", bilgi.kur);
            if (bilgi.eko != "")
                sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", bilgi.eko);

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KURKOD] + "," + ALANLAR[(int)ENUMAlanlar.FONKOD] + "," + ALANLAR[(int)ENUMAlanlar.FINKOD] + "," + ALANLAR[(int)ENUMAlanlar.EKOKOD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }
    }
}
