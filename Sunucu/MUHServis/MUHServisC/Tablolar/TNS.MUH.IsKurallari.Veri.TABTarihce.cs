using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHBELGETARIHCE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTarihce : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTarihce(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHBELGETARIHCE";
        enum ENUMAlanlar
        {
            YIL = 0,
            BELGETUR = 1,
            BELGENO = 2,
            ISLEMYAPAN = 3,
            ISLEMTARIH = 4,
            ISLEM = 5,
            ISLEMYAPANAD = 6
        }
        static string[] ALANLAR = { "YIL", "BELGETUR", "BELGENO", "ISLEMYAPAN", "ISLEMTARIH", "ISLEM", "ISLEMYAPANAD" };

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="belgeTur">Belge Tür</param>
        /// <param name="belgeNo">Belge No</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int yil, ENUMBelgeTuru belgeTur, string belgeNo)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);

            if (belgeTur != ENUMBelgeTuru.ODEMEEMRI && belgeTur != ENUMBelgeTuru.MIF && ((int)belgeTur) > 0)
                sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)belgeTur);

            if (belgeNo != "")
                sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", belgeNo);

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + "," + ALANLAR[(int)ENUMAlanlar.BELGENO] + "," + ALANLAR[(int)ENUMAlanlar.ISLEMTARIH] + " DESC";

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }
    }
}
