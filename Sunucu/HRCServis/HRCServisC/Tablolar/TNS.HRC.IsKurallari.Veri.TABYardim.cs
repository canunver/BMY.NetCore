using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Yardım bilgisini tutan HRCYARDIM veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABYardim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYardim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Yardım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCYARDIM";

        /// <summary>
        /// Yardım bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// SAYFA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAYFA = 0,
            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 1,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,
            /// <summary>
            /// METIN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            METIN = 3,
        }

        /// <summary>
        /// Yardım tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "SAYFA", "TUR", "SIRANO", "METIN" };

        /// <summary>
        /// Yardım bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek yardım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(Yardim bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            int metinAl = 2000;
            string sql = "";
            int siraNo = 1;
            int dongu = bilgi.metin.Length / 2000;

            for (int i = 0; i <= dongu; i++)
            {
                if (bilgi.metin.Length == 0)
                    break;

                if (bilgi.metin.Length < metinAl)
                    metinAl = bilgi.metin.Length;

                string metin = bilgi.metin.ToString(0, metinAl);
                bilgi.metin = bilgi.metin.Remove(0, metinAl);

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.sayfa) + ",";
                sql += VeriErisim.DegerYap(bilgi.tur) + ",";
                sql += VeriErisim.DegerYap(siraNo) + ",";
                sql += VeriErisim.DegerYap(metin) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;

                siraNo++;
            }

            return sonuc;
        }

        /// <summary>
        /// Yardım bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="list">Eklenecek yardım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc GuncellemeKaydet(ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            foreach (Yardim var in list.objeler)
            {
                sql = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(var.sayfa) + ",";
                sql += VeriErisim.DegerYap(var.tur) + ",";
                sql += VeriErisim.DegerYap(var.sira) + ",";
                sql += VeriErisim.DegerYap(var.metinParca) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;

            }

            return sonuc;
        }

        /// <summary>
        /// Yardım bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek yardım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(Yardim bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            if (bilgi.sayfa != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYFA], "=", bilgi.sayfa));
            if (bilgi.tur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Yardım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak yardım bilgisi kıstas parametresi</param>
        /// <returns>
        /// Yardım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Goster(Yardim bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYFA], "=", bilgi.sayfa));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yardım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak yardım bilgisi kıstas parametresi</param>
        /// <returns>
        /// Yardım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(Yardim bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}