using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// İşe ait parça bilgisini tutan HRCISPARCA veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIsParca : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIsParca(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İşe ait parça bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCISPARCA";

        /// <summary>
        /// İşe ait parça bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// ISKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISKOD = 0,
            /// <summary>
            /// PARCAKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PARCAKOD = 1,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,
            /// <summary>
            /// DUZEY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DUZEY = 3,
            /// <summary>
            /// ZORUNLU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ZORUNLU = 4
        }

        /// <summary>
        /// İşe ait parça tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "ISKOD", "PARCAKOD", "SIRANO", "DUZEY", "ZORUNLU" };

        /// <summary>
        /// Parça bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPARCA = "HRCPARCA";

        /// <summary>
        /// Parça bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarParca
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,
        }

        /// <summary>
        /// Parça tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPARCA = { "KOD", "AD", "SAYFA", "YUKSEKLIK", "GENISLIK" };

        /// <summary>
        /// İş ihale parça bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADISIHALEPARCA = "HRCISIHALEPARCA";

        /// <summary>
        /// İş ihale parça bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarIsIhaleParca
        {
            /// <summary>
            /// ISKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISKOD = 0,
            /// <summary>
            /// ISAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISAD = 1,
            /// <summary>
            /// PARCAKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PARCAKOD = 2,
            /// <summary>
            /// PARCAAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PARCAAD = 3,
            /// <summary>
            /// DUZEY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DUZEY = 4,
            /// <summary>
            /// ZORUNLU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ZORUNLU = 5,
        }

        /// <summary>
        /// İş ihale parça tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARISIHALEPARCA = { "ISKOD", "ISAD", "PARCAKOD", "PARCAAD", "DUZEY", "ZORUNLU" };

        /// <summary>
        /// İşe ait parça bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek işe ait parça bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(IsParca bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            for (int i = 0; i < bilgi.parcalar.objeler.Count; i++)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.isAd.kod) + ",";
                sql += VeriErisim.DegerYap(((Parca)bilgi.parcalar.objeler[i]).kod) + ",";
                sql += VeriErisim.DegerYap(i + 1) + ",";
                sql += VeriErisim.DegerYap(((Parca)bilgi.parcalar.objeler[i]).duzey) + ",";
                sql += VeriErisim.DegerYap(((Parca)bilgi.parcalar.objeler[i]).zorunlu) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }

            return sonuc;
        }

        /// <summary>
        /// İşe ait parça bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="isKod">Silinecek iş kod bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int isKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISKOD], "=", isKod));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// İşe ait parça bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="isKod">Listeleme için kullanılacak iş kod kıstas parametresi</param>
        /// <returns>
        /// İşe ait parça bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(int isKod)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADPARCA, ALANLARPARCA) + " FROM " + TABLOAD + "," + TABLOADPARCA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PARCAKOD] + "=" + TABLOADPARCA + "." + ALANLARPARCA[(int)ENUMAlanlarParca.KOD]);

            if (isKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISKOD], "=", isKod));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ISKOD] + "," + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İş ihale parça bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="isKod">Listeleme için kullanılacak iş kod kıstas parametresi</param>
        /// <returns>
        /// İş ihale parça bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleIsIhaleParca(int isKod)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARISIHALEPARCA) + " FROM " + TABLOADISIHALEPARCA;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADISIHALEPARCA, ALANLARISIHALEPARCA[(int)ENUMAlanlarIsIhaleParca.ZORUNLU], "=", "1"));
            if (isKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADISIHALEPARCA, ALANLARISIHALEPARCA[(int)ENUMAlanlarIsIhaleParca.ISKOD], "=", isKod));

            sql += " ORDER BY " + ALANLARISIHALEPARCA[(int)ENUMAlanlarIsIhaleParca.ISKOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}