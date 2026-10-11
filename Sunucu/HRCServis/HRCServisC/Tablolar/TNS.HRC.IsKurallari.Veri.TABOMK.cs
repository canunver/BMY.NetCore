using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCPSLOMK veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCOMK : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCOMK(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }
        /*
           CREATE TABLE HRCOMKANA
           (SURECYIL INT
           , SURECNO VARCHAR(20)
           , OMKTUR INT
           , OEANAHTAR VARCHAR(20)
           , ACIKLAMA VARCHAR(200)
           , TARIH DATE
           , ISLEMIYAPAN VARCHAR(200)
           , PRIMARY KEY (SURECYIL, SURECNO, OMKTUR, OEANAHTAR)
           )

           CREATE TABLE HRCOMKDETAY
           (SURECYIL INT
           , SURECNO VARCHAR(20)
           , OMKTUR INT
           , OMKKOD INT
           , OEANAHTAR VARCHAR(20)
           , DEGER INT
           , PRIMARY KEY (SURECYIL, SURECNO, OMKTUR, OMKKOD, OEANAHTAR)
           )
         
           ALTER TABLE HRCOMKDETAY
              ADD CONSTRAINT HRCOMK_FK
              FOREIGN KEY (SURECYIL, SURECNO, OMKTUR, OEANAHTAR)
              REFERENCES HRCOMKANA(SURECYIL, SURECNO, OMKTUR, OEANAHTAR)
        */
        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADANA = "HRCOMKANA";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDETAY = "HRCOMKDETAY";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarANA : int
        {

            /// <summary>
            /// SURECYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECYIL = 0,

            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,

            /// <summary>
            /// OMKTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OMKTUR = 2,

            /// <summary>
            /// OEANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OEANAHTAR = 3,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 4,

            /// <summary>
            /// TARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 5,

            /// <summary>
            /// TARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMIYAPAN = 6,
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDETAY : int
        {

            /// <summary>
            /// SURECYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECYIL = 0,

            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,

            /// <summary>
            /// OMKTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OMKTUR = 2,

            /// <summary>
            /// OEANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OEANAHTAR = 3,

            /// <summary>
            /// OMKKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OMKKOD = 4,

            /// <summary>
            /// DEGER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGER = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARANA = {
            "SURECYIL"
            , "SURECNO"
            , "OMKTUR"
            , "OEANAHTAR"
            , "ACIKLAMA"
            , "TARIH"
            , "ISLEMIYAPAN"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDETAY = {
            "SURECYIL"
            , "SURECNO"
            , "OMKTUR"
            , "OEANAHTAR"
            , "OMKKOD"
            , "DEGER"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, HRCOMKyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilAna(int surecYil, string surecNo, ENUMOnMaliKontrolTur onMaliKontrolTur, string odemeEmriNo)
        {
            string sql = "DELETE FROM " + TABLOADANA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarANA.SURECYIL], "=", surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarANA.SURECNO], "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarANA.OMKTUR], "=", (int)onMaliKontrolTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarANA.OEANAHTAR], "=", odemeEmriNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, HRCOMKyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(int surecYil, string surecNo, ENUMOnMaliKontrolTur onMaliKontrolTur, string odemeEmriNo)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.SURECYIL], "=", surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.SURECNO], "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.OMKTUR], "=", (int)onMaliKontrolTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.OEANAHTAR], "=", odemeEmriNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, HRCOMKyapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleAna(int surecYil, string surecNo, ENUMOnMaliKontrolTur onMaliKontrolTur, string odemeEmriNo, string aciklama, string islemiYapan)
        {
            string sql = "INSERT INTO " + TABLOADANA + "(" + VeriErisim.TumAlanlar("", ALANLARANA) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(surecYil);
            sql += "," + VeriErisim.DegerYap(surecNo);
            sql += "," + VeriErisim.DegerYap((int)onMaliKontrolTur);
            sql += "," + VeriErisim.DegerYap(odemeEmriNo);
            sql += "," + VeriErisim.DegerYap(aciklama);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.DegerYap(islemiYapan);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, HRCOMKyapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleDetay(int surecYil, string surecNo, ENUMOnMaliKontrolTur onMaliKontrolTur, string odemeEmriNo, int siraNo, int deger)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + "(" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(surecYil);
            sql += "," + VeriErisim.DegerYap(surecNo);
            sql += "," + VeriErisim.DegerYap((int)onMaliKontrolTur);
            sql += "," + VeriErisim.DegerYap(odemeEmriNo);
            sql += "," + VeriErisim.DegerYap(siraNo);
            sql += "," + VeriErisim.DegerYap(deger);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, HRCOMKyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader AciklamaBul(int surecYil, string surecNo, ENUMOnMaliKontrolTur onMaliKontrolTur, string odemeEmriNo)
        {
            string sql = "SELECT " + ALANLARANA[(int)ENUMAlanlarANA.ACIKLAMA] + ", " + ALANLARANA[(int)ENUMAlanlarANA.TARIH] + ", " + ALANLARANA[(int)ENUMAlanlarANA.ISLEMIYAPAN] + " FROM " + TABLOADANA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarANA.SURECYIL], "=", surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarANA.SURECNO], "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarANA.OMKTUR], "=", (int)onMaliKontrolTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarANA.OEANAHTAR], "=", odemeEmriNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, HRCOMKyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleDetay(int surecYil, string surecNo, ENUMOnMaliKontrolTur onMaliKontrolTur, string odemeEmriNo)
        {
            string sql = "SELECT " + ALANLARDETAY[(int)ENUMAlanlarDETAY.OMKKOD] + ", " + ALANLARDETAY[(int)ENUMAlanlarDETAY.DEGER] + " FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.SURECYIL], "=", surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.SURECNO], "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.OMKTUR], "=", (int)onMaliKontrolTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.OEANAHTAR], "=", odemeEmriNo));
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
