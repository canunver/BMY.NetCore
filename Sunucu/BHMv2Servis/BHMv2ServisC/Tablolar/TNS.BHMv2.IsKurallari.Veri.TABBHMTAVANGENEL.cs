using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BKD;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BHMV2TAVANDETAY veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHMTAVANGENEL : TABOrtak
    {
        /// <summary>
        /// TABBHMV2TAVANDETAY sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMTAVANGENEL(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /*
            CREATE TABLE BHMTAVANGENEL
            (YIL INT
            , ASAMA VARCHAR(20)
            , TAVANTUR INT
            , KOD1 VARCHAR(20)
            , KOD2 VARCHAR(20)
            , KOD3 VARCHAR(20)
            , KOD4 VARCHAR(20)
            , KOD5 VARCHAR(20)
            , KOD6 VARCHAR(20)
            , YIL0 MONEY
            , YIL1 MONEY
            , YIL2 MONEY
            , EK0 MONEY
            , EK1 MONEY
            , EK2 MONEY
            PRIMARY KEY (YIL, ASAMA, TAVANTUR, KOD1, KOD2, KOD3, KOD4, KOD5, KOD6)
            )
          
            CREATE TABLE BHMTAVANGENELONAY
	        (YIL int NOT NULL,
	        ASAMA varchar(20) NOT NULL,
	        TAVANTUR int NOT NULL,
	        KOD1 varchar(20) NOT NULL,
	        KOD2 varchar(20) NOT NULL,
	        KOD3 varchar(20) NOT NULL,
	        KOD4 varchar(20) NOT NULL,
	        KOD5 varchar(20) NOT NULL,
	        KOD6 varchar(20) NOT NULL,
	        DURUM int NOT NULL,
            PRIMARY KEY CLUSTERED (YIL, ASAMA, TAVANTUR, KOD1, KOD2, KOD3, KOD4, KOD5, KOD6) 
            )
         
            CREATE TABLE BHMTAVANGENELTARIHCE
	        (YIL int NOT NULL,
	        ASAMA varchar(20) NOT NULL,
	        TAVANTUR int NOT NULL,
	        KOD1 varchar(20) NOT NULL,
	        KOD2 varchar(20) NOT NULL,
	        KOD3 varchar(20) NOT NULL,
	        KOD4 varchar(20) NOT NULL,
	        KOD5 varchar(20) NOT NULL,
	        KOD6 varchar(20) NOT NULL,
            TARIH DATETIME NOT NULL,
	        ISLEMKOD int NOT NULL,
            KULLANICIKOD varchar(128),
            PRIMARY KEY CLUSTERED (YIL, ASAMA, TAVANTUR, KOD1, KOD2, KOD3, KOD4, KOD5, KOD6, TARIH) 
            )          
*/

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMTAVANGENEL";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADONAY = "BHMTAVANGENELONAY";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTARIHCE = "BHMTAVANGENELTARIHCE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// ASAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ASAMA = 1,

            /// <summary>
            /// TAVANTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TAVANTUR = 2,

            /// <summary>
            /// KOD1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD1 = 3,

            /// <summary>
            /// KOD2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD2 = 4,

            /// <summary>
            /// KOD3 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD3 = 5,

            /// <summary>
            /// KOD4 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD4 = 6,

            /// <summary>
            /// KOD5 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD5 = 7,

            /// <summary>
            /// KOD6 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD6 = 8,

            /// <summary>
            /// YIL0 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL0 = 9,

            /// <summary>
            /// YIL1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL1 = 10,

            /// <summary>
            /// YIL2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL2 = 11,

            /// <summary>
            /// YIL0 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EK0 = 12,

            /// <summary>
            /// YIL1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EK1 = 13,

            /// <summary>
            /// YIL2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EK2 = 14,
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarONAY : int
        {
            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// ASAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ASAMA = 1,

            /// <summary>
            /// TAVANTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TAVANTUR = 2,

            /// <summary>
            /// KOD1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD1 = 3,

            /// <summary>
            /// KOD2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD2 = 4,

            /// <summary>
            /// KOD3 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD3 = 5,

            /// <summary>
            /// KOD4 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD4 = 6,

            /// <summary>
            /// KOD5 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD5 = 7,

            /// <summary>
            /// KOD6 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD6 = 8,

            /// <summary>
            /// YIL0 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAY = 9,
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTARIHCE : int
        {
            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// ASAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ASAMA = 1,

            /// <summary>
            /// TAVANTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TAVANTUR = 2,

            /// <summary>
            /// KOD1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD1 = 3,

            /// <summary>
            /// KOD2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD2 = 4,

            /// <summary>
            /// KOD3 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD3 = 5,

            /// <summary>
            /// KOD4 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD4 = 6,

            /// <summary>
            /// KOD5 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD5 = 7,

            /// <summary>
            /// KOD6 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD6 = 8,

            /// <summary>
            /// TARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 9,

            /// <summary>
            /// ISLEMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMKOD = 10,

            /// <summary>
            /// ISLEMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KULLANICIKOD = 11,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "ASAMA"
            , "TAVANTUR"
            , "KOD1"
            , "KOD2"
            , "KOD3"
            , "KOD4"
            , "KOD5"
            , "KOD6"
            , "YIL0"
            , "YIL1"
            , "YIL2"
            , "EK0"
            , "EK1"
            , "EK2"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARONAY = {
            "YIL"
            , "ASAMA"
            , "TAVANTUR"
            , "KOD1"
            , "KOD2"
            , "KOD3"
            , "KOD4"
            , "KOD5"
            , "KOD6"
            , "ONAY"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTARIHCE = {
            "YIL"
            , "ASAMA"
            , "TAVANTUR"
            , "KOD1"
            , "KOD2"
            , "KOD3"
            , "KOD4"
            , "KOD5"
            , "KOD6"
            , "TARIH"
            , "ISLEMKOD"
            , "KULLANICIKOD"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, Tavan yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", TABBHMGENEL.NoktaAt(senaryo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAVANTUR], "=", (int)tavanTur));
            if (form != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD1], "=", TABBHMGENEL.NoktaAt(form.kodlar[0])));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD2], "=", TABBHMGENEL.NoktaAt(form.kodlar[1])));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD3], "=", TABBHMGENEL.NoktaAt(form.kodlar[2])));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD4], "=", TABBHMGENEL.NoktaAt(form.kodlar[3])));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD5], "=", TABBHMGENEL.NoktaAt(form.kodlar[4])));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD6], "=", TABBHMGENEL.NoktaAt(form.kodlar[5])));
            }
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, Tavan yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilOnay(int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form)
        {
            string sql = "DELETE FROM " + TABLOADONAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.ASAMA], "=", TABBHMGENEL.NoktaAt(senaryo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.TAVANTUR], "=", (int)tavanTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.KOD1], "=", TABBHMGENEL.NoktaAt(form.kodlar[0])));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.KOD2], "=", TABBHMGENEL.NoktaAt(form.kodlar[1])));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.KOD3], "=", TABBHMGENEL.NoktaAt(form.kodlar[2])));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.KOD4], "=", TABBHMGENEL.NoktaAt(form.kodlar[3])));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.KOD5], "=", TABBHMGENEL.NoktaAt(form.kodlar[4])));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.KOD6], "=", TABBHMGENEL.NoktaAt(form.kodlar[5])));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, tavanDetay dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, Tavan yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(senaryo));
            sql += "," + VeriErisim.DegerYap((int)tavanTur);
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[0]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[1]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[2]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[3]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[4]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[5]));
            sql += "," + VeriErisim.DegerYap(form.tavanlar[0]);
            sql += "," + VeriErisim.DegerYap(form.tavanlar[1]);
            sql += "," + VeriErisim.DegerYap(form.tavanlar[2]);
            sql += "," + VeriErisim.DegerYap(form.ekOdenek);
            sql += "," + VeriErisim.DegerYap(form.ekOdenek1);
            sql += "," + VeriErisim.DegerYap(form.ekOdenek2);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran, tavanDetay dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, Tavan yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleOnay(int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form, int onayKod)
        {
            string sql = "INSERT INTO " + TABLOADONAY + "(" + VeriErisim.TumAlanlar("", ALANLARONAY) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(senaryo));
            sql += "," + VeriErisim.DegerYap((int)tavanTur);
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[0]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[1]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[2]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[3]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[4]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[5]));
            sql += "," + VeriErisim.DegerYap(onayKod);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran, tavanDetay dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, Tavan yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleTarihce(int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form, ENUMTarihceKod islemTur, string kullaniciKod)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE + "(" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(senaryo));
            sql += "," + VeriErisim.DegerYap((int)tavanTur);
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[0]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[1]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[2]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[3]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[4]));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(form.kodlar[5]));
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.DegerYap((int)islemTur);
            sql += "," + VeriErisim.DegerYap(kullaniciKod);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, Tavan yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form)
        {
            string adlar = ", '', '', '', '', '', ''";
            string innerJoin = "";
            if (tavanTur == ENUMTavanTur.tavanTurEkoSektorBirim)
            {
                adlar = ", E.EKOAD, S.SEKTORAD, P.PEBAD, '', '', ''";
                innerJoin = "LEFT OUTER JOIN ButceHazirlik.dbo.ABSEKO AS E ON T.KOD1 = E.EKOKOD AND T.YIL = E.YIL LEFT OUTER JOIN ButceHazirlik.dbo.ABSSEKTOR AS S ON T.YIL = S.YIL AND T.KOD2 = S.SEKTORKOD LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD AS P ON T.YIL = P.YIL AND T.KOD3 = P.PEBKOD";
            }
            else if (tavanTur == ENUMTavanTur.tavanTurEkoFaaliyet)
            {
                adlar = ", E.EKOAD, S.SEKTORAD, P.PEBAD, '', '', ''";
                innerJoin = "LEFT OUTER JOIN ButceHazirlik.dbo.ABSEKO AS E ON T.KOD1 = E.EKOKOD AND T.YIL = E.YIL LEFT OUTER JOIN ButceHazirlik.dbo.ABSSEKTOR AS S ON T.YIL = S.YIL AND T.KOD2 = S.SEKTORKOD LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD AS P ON T.YIL = P.YIL AND T.KOD3 = P.PEBKOD";
            }
            else if (tavanTur == ENUMTavanTur.tavanTurYatirimVize)
            {
                adlar = ", P.PEBAD, E.EKOAD, S.FINAD, '', '', ''";
                innerJoin = "LEFT OUTER JOIN ButceHazirlik.dbo.ABSEKO AS E ON T.KOD2 = E.EKOKOD AND T.YIL = E.YIL LEFT OUTER JOIN ButceHazirlik.dbo.ABSFIN AS S ON T.YIL = S.YIL AND T.KOD3 = S.FINKOD LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD AS P ON T.YIL = P.YIL AND T.KOD1 = P.PEBKOD";
            }
            else if (tavanTur == ENUMTavanTur.tavanTurSeyyarIlTavan)
            {
                adlar = ", E.AD, '', '', '', '', ''";
                innerJoin = "LEFT OUTER JOIN HarcamaYonetimi.dbo.HRCIL AS E ON T.KOD1 = E.KOD";
            }
            string sql = "SELECT T.KOD1, T.KOD2, T.KOD3, T.KOD4, T.KOD5, T.KOD6, T.YIL0, T.YIL1, T.YIL2, T.EK0, T.EK1, T.EK2" + adlar + " FROM " + TABLOAD + " T " + innerJoin;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", TABBHMGENEL.NoktaAt(senaryo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.TAVANTUR], "=", (int)tavanTur));

            for (int i = 0; i < 6; i++)
            {
                if (!string.IsNullOrWhiteSpace(form.kodlar[i]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[((int)ENUMAlanlar.KOD1) + i], "=", TABBHMGENEL.NoktaAt(form.kodlar[i])));
            }

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, Tavan yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleOnay(int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form)
        {
            string sql = "SELECT YIL, ASAMA, T.KOD1, T.KOD2, T.KOD3, T.KOD4, T.KOD5, T.KOD6, T.ONAY FROM " + TABLOADONAY + " T ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlarONAY.TAVANTUR], "=", (int)tavanTur));
            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlarONAY.YIL], "=", yil));
            if (!string.IsNullOrWhiteSpace(senaryo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlarONAY.ASAMA], "=", TABBHMGENEL.NoktaAt(senaryo)));

            for (int i = 0; i < 6; i++)
            {
                if (!string.IsNullOrWhiteSpace(form.kodlar[i]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[((int)ENUMAlanlarONAY.KOD1) + i], "=", TABBHMGENEL.NoktaAt(form.kodlar[i])));
            }

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, Tavan yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleTarihce(int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + " FROM " + TABLOADTARIHCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarONAY.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarONAY.ASAMA], "=", TABBHMGENEL.NoktaAt(senaryo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarONAY.TAVANTUR], "=", (int)tavanTur));

            for (int i = 0; i < 6; i++)
            {
                if (!string.IsNullOrWhiteSpace(form.kodlar[i]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[((int)ENUMAlanlarONAY.KOD1) + i], "=", TABBHMGENEL.NoktaAt(form.kodlar[i])));
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public static string Listele3Sql(int yil, string senaryoKod, int tavanTur, string kod1, string kod2, string kod3)
        {
            string sorgu = "SELECT KOD1, KOD2, KOD3, YIL0, YIL1, YIL2 from BHMTAVANGENEL where YIL=" + VeriErisim.DegerYap(yil) + " AND ASAMA=" + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + " AND TAVANTUR=" + VeriErisim.DegerYap(tavanTur);

            if (kod1 != null)
                sorgu += " AND KOD1=" + VeriErisim.DegerYap(kod1.Replace(".", ""));
            if (kod2 != null)
                sorgu += " AND KOD2=" + VeriErisim.DegerYap(kod2.Replace(".", ""));
            if (kod3 != null)
                sorgu += " AND KOD3=" + VeriErisim.DegerYap(kod3.Replace(".", ""));
            return sorgu;
        }

        public static string Listele4Sql(int yil, string senaryoKod, int tavanTur, string kod1, string kod2, string kod3, string kod4, bool ekli)
        {
            string sorgu = "SELECT KOD1, KOD2, KOD3, KOD4, YIL0, YIL1, YIL2" + (ekli ? ", EK0, EK1, EK2" : "") + " from BHMTAVANGENEL where YIL=" + VeriErisim.DegerYap(yil) + " AND ASAMA=" + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + " AND TAVANTUR=" + VeriErisim.DegerYap(tavanTur);

            if (kod1 != null)
                sorgu += " AND KOD1=" + VeriErisim.DegerYap(kod1.Replace(".", ""));
            if (kod2 != null)
                sorgu += " AND KOD2=" + VeriErisim.DegerYap(kod2.Replace(".", ""));
            if (kod3 != null)
                sorgu += " AND KOD3=" + VeriErisim.DegerYap(kod3.Replace(".", ""));
            if (kod4 != null)
                sorgu += " AND KOD4=" + VeriErisim.DegerYap(kod4.Replace(".", ""));
            return sorgu;
        }

        public static string ListeleRapor(bool detay, int yil, string senaryoKod, int tavanTur, string pebKod, string ekoKod, string finKod)
        {
            string alanlar = "P.SEKTORNO," + (detay ? "O.KOD2, O.KOD3, " : "") + " P.DPTPROJENO, O.KOD4, SEK.AD, P.AD, PEBKOD.PEBAD";
            string sql = "SELECT " + alanlar + ", SUM(O.YIL0), SUM(O.YIL1), SUM(O.YIL2) ";

            sql += "FROM BHMTAVANGENEL AS O INNER JOIN ButceHazirlik.dbo.ABSPEBKOD AS PEBKOD ON " + VeriErisim.Join("PEBKOD", "O", new string[] { "YIL", "PEBKOD" }, new string[] { "YIL", "KOD4" });
            sql += " RIGHT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS BK ON O.YIL = BK.YIL AND O.KOD4 = BK.PEBKOD AND BK.SENARYOKOD=O.ASAMA ";
            sql += " RIGHT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAK AS K ON " + VeriErisim.Join("BK", "K", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sql += " RIGHT OUTER JOIN Yatirim.dbo.YATALTPROJE AS AP ON " + VeriErisim.Join("K", "AP", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql += " RIGHT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON " + VeriErisim.Join("AP", "P", new string[] { "YIL", "SENARYOKOD", "PROJENO" });
            sql += " RIGHT OUTER JOIN Yatirim.dbo.YATSEKTOR AS SEK ON " + VeriErisim.Join("P", "SEK", new string[] { "YIL", "SEKTORNO" });
            sql += " WHERE (O.YIL = " + VeriErisim.DegerYap(yil) + ") AND (O.ASAMA = " + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + ") AND (O.TAVANTUR = " + tavanTur.ToString() + ")";

            if (!string.IsNullOrWhiteSpace(pebKod))
                sql += " AND O.KOD4 LIKE " + VeriErisim.DegerYap(pebKod.Replace(".", "") + "%");
            if (!string.IsNullOrWhiteSpace(ekoKod))
                sql += " AND O.KOD2 LIKE " + VeriErisim.DegerYap(ekoKod.Replace(".", "") + "%");
            if (!string.IsNullOrWhiteSpace(finKod))
                sql += " AND O.KOD3 LIKE " + VeriErisim.DegerYap(finKod.Replace(".", "") + "%");
            sql += " GROUP BY " + alanlar + " ORDER BY " + alanlar;

            return sql;
        }
    }
}
