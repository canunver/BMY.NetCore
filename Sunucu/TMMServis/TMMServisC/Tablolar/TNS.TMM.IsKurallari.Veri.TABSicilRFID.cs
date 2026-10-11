using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Sicil RFID bilgilerini tutan TASSICILNORFID veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABSicilRFID : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSicilRFID(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASSICILNORFID";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// RFID no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            RFIDNO = 0,
            /// <summary>
            /// Programatik sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PRSICILNO = 1,
            /// <summary>
            /// Kill no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KILLNO = 2,
            /// <summary>
            /// Access no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACCESSNO = 3,
            /// <summary>
            /// Tag type alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TAGTYPE = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "RFIDNO", "PRSICILNO", "KILLNO", "ACCESSNO", "TAGTYPE" };

        /// <summary>
        /// Sicil RFID bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="rfId">Sicil RFID bilgisini tutan, SicilRFID türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(SicilRFID rfId)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(rfId.rfIdNo) + ",";
            sql += VeriErisim.DegerYap(rfId.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(rfId.killNo) + ",";
            sql += VeriErisim.DegerYap(rfId.accessNo) + ",";
            sql += VeriErisim.DegerYap(rfId.tagType) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Sicil RFID bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon. Web Servis parametresi true ise, web servis ile ilgili alanları da günceller.
        /// </summary>
        /// <param name="rfId">Sicil RFID bilgisini tutan, SicilRFID türünde nesne</param>
        /// <param name="webServis">Web servis ile ilgili alanların güncellenip güncellenmeyeceği bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Guncelle(SicilRFID rfId, bool webServis)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RFIDNO], rfId.rfIdNo);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], rfId.prSicilNo);
            if (webServis)
            {
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KILLNO], rfId.killNo);
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACCESSNO], rfId.accessNo);
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAGTYPE], rfId.tagType);
            }
            if (rfId.rfIdNo > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RFIDNO], "=", rfId.rfIdNo));
            else if (rfId.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", rfId.prSicilNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Muhasebe Kodu, Harcama Kodu ve Belge numarasına göre Sicil RFID bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon. 
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="muhasebeKod">Muhasebe Kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="harcamaKod">Harcama Kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="belgeNo">Belge No bilgisini tutan, String türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Guncelle(int yil, string muhasebeKod, string harcamaKod, string belgeNo)
        {
            string sql = "DELETE " + TABLOAD;

            string icSQL = "SELECT " + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " FROM TASSICILNOHAREKET";
            icSQL = VeriErisim.KosulEkle(icSQL, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("", "MUHASEBEKOD", "=", muhasebeKod));
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("", "HARCAMABIRIMKOD", "=", harcamaKod.Replace(".", "")));
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("", "FISNO", "=", belgeNo));
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("", "ISLEMTIPIKOD", "<", (int)ENUMZimmetIslemTipi.ZFVERME));
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("", "DURUM", "<>", (int)ENUMSicilDurum.IPTAL));

            sql = VeriErisim.KosulEkle(sql, "WHERE", ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " IN (" + icSQL + ")");

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil RFID bilgisini kullanarak
        /// Sicil RFID  bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="rfId">Sicil RFID bilgisini tutan, SicilRFID türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(SicilRFID rfId)
        {
            string sql = "DELETE FROM " + TABLOAD;
            if (rfId.rfIdNo > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RFIDNO], "=", rfId.rfIdNo));
            else if (rfId.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", rfId.prSicilNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen RFID numarasının,
        /// Daha önce kullanılıp kullanılmadığı bilgisini dönen,
        /// sql komutunu oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="rfIdNo">RFID no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, RFID bilgisi kullanılmışsa true, kullanılmamışsa false bilgisini tutan Bool türünde nesne döndürür</returns>
        public bool RfIdBostaMi(int rfIdNo)
        {
            string sql = "SELECT 1 FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RFIDNO], "=", rfIdNo));
            sql = VeriErisim.KosulEkle(sql, "AND (", ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " IS NULL");
            sql = VeriErisim.KosulEkle(sql, "OR", ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " = 0");
            sql += ")";

            return veriErisim.TekKolonSQLInt(sql) == 1 ? true : false;
        }

        /// <summary>
        /// Parametre olarak verilen Programatik Sicil Numarası bilgisini kullanarak,
        /// Programatik Sicil numarası bilgisinin kayıtlı olup olmadığını,
        /// KayıtVarMı rutinini çalıştırarak bulan ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="prSicilNo">Programatik Sicil Numarası bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda, kayıt var ise true yoksa false değerini veren Bool türünde nesne döndürür</returns>
        public bool PrSicilNoKullanilmisMi(int prSicilNo)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.PRSICILNO] };
            int[] intDeger = { prSicilNo };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Sicil RFID bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <param name="rfIdOlanlar">RFID olanlarının listelenip listelenmeyeceğini tutan, Bool türünde nesne</param>
        /// <param name="rfIdOlmayanlar">RFID olmayanların listelenip listelenemeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Sicil bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader SicilListele(SicilNoHareket sicilNo, bool rfIdOlanlar, bool rfIdOlmayanlar)
        {
            return veriErisim.DataReaderDondur(SicilListeleSqlOlustur(sicilNo, rfIdOlanlar, rfIdOlmayanlar, false));
        }

        public int SicilListeleKayitSay(SicilNoHareket sicilNo, bool rfIdOlanlar, bool rfIdOlmayanlar)
        {
            return veriErisim.TekKolonSQLInt(SicilListeleSqlOlustur(sicilNo, rfIdOlanlar, rfIdOlmayanlar, true));
        }

        private string SicilListeleSqlOlustur(SicilNoHareket sicilNo, bool rfIdOlanlar, bool rfIdOlmayanlar, bool count)
        {
            string sql = "SELECT ";
            sql += count ? VeriErisim.IslecBul("COUNT") + "(*)" : "*";
            sql += " FROM TASSICILNORFIDVIEW";

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(sicilNo.muhasebeKod))
                kosul = VeriErisim.KosulOl("", "MUHASEBEKOD", "=", sicilNo.muhasebeKod);

            if (!string.IsNullOrEmpty(sicilNo.harcamaBirimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "HARCAMABIRIMKOD", "=", sicilNo.harcamaBirimKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(sicilNo.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "AMBARKOD", "=", sicilNo.ambarKod));

            if (!string.IsNullOrEmpty(sicilNo.fisNo))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "FISNO", "=", sicilNo.fisNo));
                if (sicilNo.yil > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "YIL", "=", sicilNo.yil));
            }

            if (!string.IsNullOrEmpty(sicilNo.sicilNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SICILNO", "LIKE", sicilNo.sicilNo + "%"));

            if (!string.IsNullOrEmpty(sicilNo.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "HESAPPLANKOD", "LIKE", sicilNo.hesapPlanKod.Replace(".", "") + "%"));

            if (sicilNo.prSicilNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "PRSICILNO", "=", sicilNo.prSicilNo));

            if (!string.IsNullOrEmpty(sicilNo.odaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "NEREYEGITTI", "=", sicilNo.odaKod));

            if (!string.IsNullOrEmpty(sicilNo.personelKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "KIMEGITTI", "=", sicilNo.personelKod));

            if (sicilNo.rfIdNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "RFIDNO", "=", sicilNo.rfIdNo));
            else if (rfIdOlanlar)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", "RFIDNO IS NOT NULL");
                kosul = VeriErisim.KosulEkle(kosul, "AND", "RFIDNO > 0");
            }
            else if (rfIdOlmayanlar)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND (", "RFIDNO IS NULL");
                kosul = VeriErisim.KosulEkle(kosul, "OR", "RFIDNO = 0");
                kosul += ")";
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            if (!count)
                sql += " ORDER BY PRSICILNO, SIRANO";

            return sql;
        }
    }
}