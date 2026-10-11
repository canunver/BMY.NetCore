using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHMUHASEBEYETKILISI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMuhasebeYetkilisi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMuhasebeYetkilisi(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHMUHASEBEYETKILISI";
        static string[] ALANLAR ={ "MUHASEBEKOD", "ADSOYAD", "UNVAN" };
        enum ENUMAlanlar
        {
            MUHASEBEKOD = 0,
            ADSOYAD = 1,
            UNVAN = 2,
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="kriter">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(MuhasebeYetkilisi kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (kriter.muhasebe > 0)
                kosul += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebe);
            if (!string.IsNullOrEmpty(kriter.adsoyad))
                kosul += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ADSOYAD], "LIKE", "%" + kriter.adsoyad + "%");
            if (!string.IsNullOrEmpty(kriter.unvan))
                kosul += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.UNVAN], "LIKE", "%" + kriter.unvan + "%");

            if (kosul != "")
                sql += " WHERE ";
            sql += kosul;
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "," + ALANLAR[(int)ENUMAlanlar.ADSOYAD] + "," + ALANLAR[(int)ENUMAlanlar.UNVAN];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebeYetkilisi">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(MuhasebeYetkilisi muhasebeYetkilisi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(muhasebeYetkilisi.muhasebe) + ", ";
            sql += VeriErisim.DegerYap(muhasebeYetkilisi.adsoyad) + ", ";
            sql += VeriErisim.DegerYap(muhasebeYetkilisi.unvan) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET " + ALANLAR[(int)ENUMAlanlar.ADSOYAD] + "='" + muhasebeYetkilisi.adsoyad + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.UNVAN] + "='" + muhasebeYetkilisi.unvan + "'";
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeYetkilisi.muhasebe);

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebeKod">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int muhasebeKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod);
            
            return KomutCalistir(sql, 1);
        }

    }
}
