using System;
using System.Data;
using TNS.HRC.IsKurallari;
using TNS.Veri;


namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Fazla Mesai Personel Puantaj bilgisini tutan PSLFAZLAMESAIPERSONELPUANTAJ veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABFazlaMesaiPersonelPuantaj : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFazlaMesaiPersonelPuantaj(VeriErisim veriErisim)
            : base(veriErisim)
        {
            if (veriErisim.baglantiTip == "ORACLE")
                TABLOAD = "PSLFMPERPUAN";
        }

        /// <summary>
        /// Fazla Mesai Personel Puantaj tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PSLFAZLAMESAIPERSONELPUANTAJ";

        /// <summary>
        /// Fazla Mesai Personel Puantaj tablo alanlarının sırasını tutan enumeration
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
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 8,
            /// <summary>
            /// GELIRVERGISIORANI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGISIORANI = 9,
        }

        /// <summary>
        /// Fazla Mesai Personel Puantaj tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ILGILIYIL", "ILGILIAY", "TCKIMLIKNO", "PUANTAJKODU", "SIRANO", "GELIRVERGISIORANI" };

        /// <summary>
        /// Personel tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONEL = "HRCPERSONEL";

        /// <summary>
        /// Personel tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarPersonel : int
        {
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 0,
            /// <summary>
            /// ADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYAD = 1,
            /// <summary>
            /// UNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVAN = 2,
        }

        /// <summary>
        /// Personel tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPERSONEL = { "TCKIMLIKNO", "ADSOYAD", "UNVAN" };

        /// <summary>
        /// Fazla Mesai Personel Puantaj bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak fazla mesai kıstas parametresi</param>
        /// <param name="fp">Listeleme için kullanılacak fazla mesai personel kıstas parametresi</param>
        /// <returns>
        /// Fazla Mesai Personel Puantaj bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(FazlaMesai f, FazlaMesaiPersonel fp)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABFazlaMesaiPuantaj.TABLOAD, TABFazlaMesaiPuantaj.ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADPERSONEL, ALANLARPERSONEL) + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GELIRVERGISIORANI] + " FROM " + TABLOAD + "," + TABLOADPERSONEL + "," + TABFazlaMesaiPuantaj.TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));

            if (f.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            if (!string.IsNullOrEmpty(f.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));

            if (fp.tcKimlikNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", fp.tcKimlikNo));

            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + "=" + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.TCKIMLIKNO]);
            //sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABFazlaMesaiPuantaj.TABLOAD + "." + TABFazlaMesaiPuantaj.ALANLAR[(int)TABFazlaMesaiPuantaj.ENUMAlanlar.YIL]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PUANTAJKODU] + "=" + TABFazlaMesaiPuantaj.TABLOAD + "." + TABFazlaMesaiPuantaj.ALANLAR[(int)TABFazlaMesaiPuantaj.ENUMAlanlar.KOD]);

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIYIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIAY] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }
    }
}
