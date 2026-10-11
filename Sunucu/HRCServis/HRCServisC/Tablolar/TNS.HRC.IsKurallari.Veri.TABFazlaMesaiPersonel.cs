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
    public class TABFazlaMesaiPersonel : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFazlaMesaiPersonel(VeriErisim veriErisim)
            : base(veriErisim)
        {
            if (veriErisim.baglantiTip == "ORACLE")
                TABLOAD = "PSLFMPERPUAN";
        }

        /// <summary>
        /// Fazla Mesai Personel Puantaj tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLFAZLAMESAIPERSONELPUANTAJ";

        /// <summary>
        /// Personel tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONEL = "HRCPERSONEL";

        /// <summary>
        /// Fazla Mesai Personel Puantaj tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar : int
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
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ILGILIYIL", "ILGILIAY", "TCKIMLIKNO", "PUANTAJKODU", "SIRANO", "GELIRVERGISIORANI" };

        /// <summary>
        /// Personel tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarPersonel : int
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
            /// <summary>
            /// MUHASEBEKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKODU = 3,
            /// <summary>
            /// BIRIMKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKODU = 4,
        }

        /// <summary>
        /// Personel tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPERSONEL = { "TCKIMLIKNO", "ADSOYAD", "UNVAN", "MUHASEBEKODU", "BIRIMKODU" };

        /// <summary>
        /// Fazla Mesai Personel Puantaj bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak fazla mesai kıstas parametresi</param>
        /// <returns>
        /// Fazla Mesai Personel Puantaj bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(FazlaMesai f)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADPERSONEL, ALANLARPERSONEL) + " FROM " + TABLOAD + "," + TABLOADPERSONEL;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + "=" + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.TCKIMLIKNO]);
            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIYIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIAY] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Fazla Mesai Personel Puantaj bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak fazla mesai kıstas parametresi</param>
        /// <returns>
        /// Fazla Mesai Personel Puantaj bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader PersonelinBaskaKaydiVarMi(int yil, int ay, string surecNo, string TCKimlikNo)
        {
            string sql = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + "," + 
                              TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL]  +"," + 
                              TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] +
                    " FROM " + TABLOAD + " INNER JOIN HRCSUREC ON " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "= HRCSUREC.YIL AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "= HRCSUREC.SURECNO ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIYIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIAY], "=", ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", TCKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "!=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", "HRCSUREC.SURECDURUM <> 9");

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Personel bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <returns>
        /// Personel bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListelePersonel(int muhasebe, string birim)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADPERSONEL, ALANLARPERSONEL) + " FROM " + TABLOADPERSONEL;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPersonel.MUHASEBEKODU], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPersonel.BIRIMKODU], "=", birim));
            sql += " ORDER BY " + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ADSOYAD] + "," + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.TCKIMLIKNO];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Fazla Mesai Personel Puantaj bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek fazla mesai bilgisini tutan nesne</param>
        /// <param name="fp">Eklenecek fazla mesai personel bilgisini tutan nesne</param>
        /// <param name="siraNo">Eklenecek fazla mesai personel puantaj bilgisinin sıra numarasını tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(FazlaMesai f, FazlaMesaiPersonel fp, int siraNo)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(f.yil) + ",";
            sql += VeriErisim.DegerYap(f.muhasebe) + ",";
            sql += VeriErisim.DegerYap(f.birim) + ",";
            sql += VeriErisim.DegerYap(f.surecNo) + ",";
            sql += VeriErisim.DegerYap(f.ilgiliYil) + ",";
            sql += VeriErisim.DegerYap(f.ilgiliAy) + ",";
            sql += VeriErisim.DegerYap(fp.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(fp.puantaj.kod) + ",";
            sql += VeriErisim.DegerYap(siraNo) + ",";
            sql += VeriErisim.DegerYap(fp.gelirVergisiOrani) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Fazla mesai personel puantaj bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Silinecek fazla mesai icmal bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(FazlaMesai f)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));

            return KomutCalistir(sql, 0);
        }
    }
}
