using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Süreç parça bilgisini tutan HRCSURECPARCA veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSurecParca : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSurecParca(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Süreç parça bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCSURECPARCA";

        /// <summary>
        /// Süreç parça bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,
            /// <summary>
            /// ISKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISKOD = 2,
            /// <summary>
            /// PARCAKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PARCAKOD = 3,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,
            /// <summary>
            /// IHALEUSUL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEUSUL = 5,
            /// <summary>
            /// IHALEMADDE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEMADDE = 6,
            /// <summary>
            /// DUZEY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DUZEY = 7,
            /// <summary>
            /// ZORUNLU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ZORUNLU = 8,
        }

        /// <summary>
        /// Süreç parça tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "SURECNO", "ISLEMKOD", "PARCAKOD", "SIRANO", "IHALEUSUL", "IHALEMADDE", "DUZEY", "ZORUNLU" };

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
            /// <summary>
            /// SAYFA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAYFA = 2,
            /// <summary>
            /// YUKSEKLIK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YUKSEKLIK = 3,
            /// <summary>
            /// GENISLIK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GENISLIK = 4,
        }

        /// <summary>
        /// Parça tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPARCA = { "KOD", "AD", "SAYFA", "YUKSEKLIK", "GENISLIK" };

        /// <summary>
        /// Süreç parça bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek süreç parça bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(SurecParca bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            int siraNo = 1;
            foreach (Parca parca in bilgi.parcalar.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.isKod) + ",";
                sql += VeriErisim.DegerYap(parca.kod) + ",";
                sql += VeriErisim.DegerYap(siraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.ihaleUsulu) + ",";
                sql += VeriErisim.DegerYap(bilgi.ihaleMaddesi) + ",";
                sql += VeriErisim.DegerYap(parca.duzey) + ",";
                sql += VeriErisim.DegerYap(parca.zorunlu) + ")";
                siraNo++;

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Süreç parça bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak süreç parça bilgisi kıstas parametresi</param>
        /// <returns>
        /// Süreç parça bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(SurecParca bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," +
                                VeriErisim.TumAlanlar(TABLOADPARCA, ALANLARPARCA);

            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABLOADPARCA + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PARCAKOD] + "=" + TABLOADPARCA + "." + ALANLARPARCA[(int)ENUMAlanlarParca.KOD];

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.sonrakiParcaIste)
            {
                if (bilgi.siraNo > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], ">", bilgi.siraNo));
            }
            else
            {
                if (bilgi.siraNo > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "<", bilgi.siraNo));
            }
            if (bilgi.zorunlu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ZORUNLU], "=", bilgi.zorunlu));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Süreç parça bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek süreç parça bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(SurecParca bilgi)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));

            return KomutCalistir(sql, 0);
        }

    }
}