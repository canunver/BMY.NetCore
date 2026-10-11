using System;
using System.Data;
using TNS.HRC.IsKurallari;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Ek Ders Personel Puantaj bilgisini tutan PSLEKDERSPERSONELPUANTAJ veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABEkDersPersonelPuantaj : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABEkDersPersonelPuantaj(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Ek Ders Personel Puantaj tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PSLEKDERSPERSONELPUANTAJ";

        /// <summary>
        /// Ek Ders Personel Puantaj tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
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
            /// ILGILIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILGILIYIL = 4,
            /// <summary>
            /// ILGILIAY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILGILIAY = 5,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 6,
            /// <summary>
            /// PUANTAJKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PUANTAJKODU = 7,
            /// <summary>
            /// GELIRVERGISIORANI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGISIORANI = 8,
        }

        /// <summary>
        /// Ek Ders Personel Puantaj tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ILGILIYIL", "ILGILIAY", "TCKIMLIKNO", "PUANTAJKODU", "GELIRVERGISIORANI" };

        /// <summary>
        /// Öğrenci Eğitmen tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADTANIM = "HRCOGRENCIEGITMEN";

        /// <summary>
        /// Öğrenci Eğitmen tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTANIM
        {
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 1,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 2,
            /// <summary>
            /// ADISOYADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADISOYADI = 3,
        }

        /// <summary>
        /// Öğrenci Eğitmen tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTANIM ={ "MUHASEBEKOD", "BIRIMKOD", "TCKIMLIKNO", "ADISOYADI" };

        /// <summary>
        /// Ek Ders Personel Puantaj bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak ek ders kıstas parametresi</param>
        /// <param name="fp">Listeleme için kullanılacak ek ders personel kıstas parametresi</param>
        /// <returns>
        /// Ek ders personel puantaj bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(EkDers f, EkDersPersonel fp)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABEkDersPuantaj.TABLOAD, TABEkDersPuantaj.ALANLAR) + "," +
                              TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GELIRVERGISIORANI] + "," +
                              TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.TCKIMLIKNO] + "," +
                              TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.ADISOYADI] +
                " FROM " + TABLOAD +

                " LEFT OUTER JOIN " + TABLOADTANIM + " ON " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + " = " + TABLOADTANIM  + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.MUHASEBEKOD] + " AND " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + " = " + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BIRIMKOD] + " AND " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.TCKIMLIKNO] +

                " LEFT OUTER JOIN " + TABEkDersPuantaj.TABLOAD + " ON " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PUANTAJKODU] + " = " + TABEkDersPuantaj.TABLOAD + "." + TABEkDersPuantaj.ALANLAR[(int)TABEkDersPuantaj.ENUMAlanlar.KOD];

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));

            if (fp.tcKimlikNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", fp.tcKimlikNo));

            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PUANTAJKODU] + "=" + TABEkDersPuantaj.TABLOAD + "." + TABEkDersPuantaj.ALANLAR[(int)TABEkDersPuantaj.ENUMAlanlar.KOD]);

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIYIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIAY] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }
    }
}
