using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// İhaleye ait parça bilgisini tutan HRCIHALEPARCA veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIhaleParca : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIhaleParca(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İhaleye ait parça bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCIHALEPARCA";

        /// <summary>
        /// İhaleye ait parça bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// KOMISYON alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOMISYON = 2,
            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 3,
            /// <summary>
            /// PARCAKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PARCAKOD = 4,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 5,
            /// <summary>
            /// DUZEY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DUZEY = 6,
            /// <summary>
            /// ZORUNLU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ZORUNLU = 7
        }

        /// <summary>
        /// İhaleye ait parça tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "IHALEUSUL", "IHALEMADDE", "KOMISYON", "TUR", "PARCAKOD", "SIRANO", "DUZEY", "ZORUNLU" };

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
        static string[] ALANLARPARCA ={ "KOD", "AD", "SAYFA", "YUKSEKLIK", "GENISLIK" };

        /// <summary>
        /// İhaleye ait parça bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek ihaleye ait parça bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(IhaleParca bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            for (int i = 0; i < bilgi.parcalar.objeler.Count; i++)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.ihaleUsulu) + ",";
                sql += VeriErisim.DegerYap(bilgi.ihaleMaddesi) + ",";
                sql += VeriErisim.DegerYap(bilgi.komisyon) + ",";
                sql += VeriErisim.DegerYap(bilgi.tur) + ",";
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
        /// İhaleye ait parça bilgisini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="list">Güncellenecek ihaleye ait parça bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (IhaleParcaBag bilgi in list.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.ihaleUsulu) + ",";
                sql += VeriErisim.DegerYap(bilgi.ihaleMaddesi) + ",";
                sql += VeriErisim.DegerYap(bilgi.komisyon) + ",";
                sql += VeriErisim.DegerYap(bilgi.tur) + ",";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.sira) + ",";
                sql += VeriErisim.DegerYap(bilgi.duzey) + ",";
                sql += VeriErisim.DegerYap(bilgi.zorunlu) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;

            }
            return sonuc;
        }

        /// <summary>
        /// İhaleye ait parça bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ihaleUsul">Silinecek ihale usul bilgisini tutan nesne</param>
        /// <param name="ihaleMadde">Silinecek ihale madde bilgisini tutan nesne</param>
        /// <param name="komisyon">Silinecek ihale madde bilgisini tutan nesne</param>
        /// <param name="tur">Silinecek tür bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(int ihaleUsul, int ihaleMadde, int komisyon, int tur)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            if (ihaleUsul > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEUSUL], "=", ihaleUsul));
            if (ihaleMadde > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEMADDE], "=", ihaleMadde));
            if (komisyon > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOMISYON], "=", komisyon));
            if (tur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", tur));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// İhaleye ait parça bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="ihaleUsul">Listeleme için kullanılacak ihale usul kıstas parametresi</param>
        /// <param name="ihaleMadde">Listeleme için kullanılacak ihale madde kıstas parametresi</param>
        /// <param name="komisyon">Listeleme için kullanılacak komisyon kıstas parametresi</param>
        /// <param name="tur">Listeleme için kullanılacak tür kıstas parametresi</param>
        /// <returns>
        /// İhaleye ait parça bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(int ihaleUsul, int ihaleMadde, int komisyon, int tur)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADPARCA, ALANLARPARCA) + " FROM " + TABLOAD + "," + TABLOADPARCA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PARCAKOD] + "=" + TABLOADPARCA + "." + ALANLARPARCA[(int)ENUMAlanlarParca.KOD]);

            if (ihaleUsul > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IHALEUSUL], "=", ihaleUsul));
            if (ihaleMadde > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IHALEMADDE], "=", ihaleMadde));
            if (komisyon > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOMISYON], "=", komisyon));
            if (tur > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", tur));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.IHALEUSUL] + "," + ALANLAR[(int)ENUMAlanlar.IHALEMADDE] + "," + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}