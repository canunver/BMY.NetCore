using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// İhale sözleşme tanım bilgisini tutan HRCIHALESOZLESMETANIM veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIhaleSozlesmeTanim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIhaleSozlesmeTanim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İhale sözleşme tanım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCIHALESOZLESMETANIM";

        /// <summary>
        /// İhale sözleşme tanım bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// IHALEUSUL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEUSUL = 0,
            /// <summary>
            /// IHALEMADDE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEMADDE = 1,
            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 2,
            /// <summary>
            /// ISTUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISTUR = 3,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,
            /// <summary>
            /// METIN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            METIN = 5,
        }

        /// <summary>
        /// İhale sözleşme tanım tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "IHALEUSUL", "IHALEMADDE", "TUR", "ISTUR", "SIRANO", "METIN" };

        /// <summary>
        /// İhale sözleşme tanım bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek ihale sözleşme tanım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(IhaleSozlesmeTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
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
                sql += VeriErisim.DegerYap(bilgi.ihaleUsulu) + ",";
                sql += VeriErisim.DegerYap(bilgi.ihaleMaddesi) + ",";
                sql += VeriErisim.DegerYap(bilgi.tur) + ",";
                sql += VeriErisim.DegerYap(bilgi.isTur) + ",";
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
        /// İhale sözleşme tanım bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek ihale tanım ücret bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(IhaleSozlesmeTanim bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEUSUL], "=", bilgi.ihaleUsulu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEMADDE], "=", bilgi.ihaleMaddesi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISTUR], "=", bilgi.isTur));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// İhale sözleşme tanım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak ihale sözleşme tanım bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale sözleşme tanım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(IhaleSozlesmeTanim bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IHALEUSUL], "=", bilgi.ihaleUsulu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IHALEMADDE], "=", bilgi.ihaleMaddesi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISTUR], "=", bilgi.isTur));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}