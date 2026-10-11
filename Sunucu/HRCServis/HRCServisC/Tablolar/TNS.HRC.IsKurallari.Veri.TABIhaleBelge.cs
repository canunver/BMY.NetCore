using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// İhale Belge bilgisini tutan HRCIHALEBELGE veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIhaleBelge : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIhaleBelge(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İhale Belge bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCIHALEBELGE";

        /// <summary>
        /// İhale Belge bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 4,
            /// <summary>
            /// BELGETIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETIP = 5,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 6,
            /// <summary>
            /// ALAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALAN = 7,
            /// <summary>
            /// DEGER alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DEGER = 8,
            /// <summary>
            /// KONTROLID alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONTROLID = 9,
            /// <summary>
            /// KONTROLTUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONTROLTUR = 10,
        }

        /// <summary>
        /// İhale Belge bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TUR",
            "BELGETIP","SIRANO","ALAN", "DEGER","KONTROLID","KONTROLTUR"};

        /// <summary>
        /// İhale Belge bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek ihale belge bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Kaydet(IhaleBelge bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            for (int i = 0; i < bilgi.detay.Count; i++)
            {
                IhaleBelgeDetay detay = (IhaleBelgeDetay)bilgi.detay[i];

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.belgeTur) + ",";
                sql += VeriErisim.DegerYap(bilgi.belgeTip) + ",";
                sql += VeriErisim.DegerYap(i + 1) + ",";
                sql += VeriErisim.DegerYap(detay.alan) + ",";
                sql += VeriErisim.DegerYap(detay.deger) + ",";
                sql += VeriErisim.DegerYap(detay.kontrolID) + ",";
                sql += VeriErisim.DegerYap(detay.kontrolTur) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }

            return sonuc;
        }

        /// <summary>
        /// İhale Belge bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek ihale belge bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(IhaleBelge bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql += PKKosulEkle(bilgi);

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// İhale Belge bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak ihale belge bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale Belge bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(IhaleBelge bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql += PKKosulEkle(bilgi);
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        private string PKKosulEkle(IhaleBelge bilgi)
        {
            string sql = " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETIP], "=", bilgi.belgeTip));

            return sql;
        }
    }
}