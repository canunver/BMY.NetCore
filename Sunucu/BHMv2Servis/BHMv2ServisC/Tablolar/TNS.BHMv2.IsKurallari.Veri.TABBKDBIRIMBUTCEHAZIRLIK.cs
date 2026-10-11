using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BKD;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BKDBIRIMBUTCEHAZIRLIK veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBKDBIRIMBUTCEHAZIRLIK : TABOrtak
    {
        /// <summary>
        /// TABBKDBIRIMBUTCEHAZIRLIK sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBKDBIRIMBUTCEHAZIRLIK(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLENAME = "BHMBIRIMBUTCEHAZIRLIK";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLENAMEONAY = "BHMBIRIMONAY";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLENAMEONAYTARIHCE = "BHMBIRIMONAYTARIHCE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMFields : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// SENARYOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 1,

            /// <summary>
            /// MUHASEBEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,

            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 4,

            /// <summary>
            /// FONKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKOD = 5,

            /// <summary>
            /// FINKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKOD = 6,

            /// <summary>
            /// EKOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 7,

            /// <summary>
            /// PEBKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 8,

            /// <summary>
            /// ODENEK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK = 9,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 10,
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMFieldsONAY : int
        {
            YIL = 0,
            SENARYOKOD = 1,
            MUHASEBEKOD = 2,
            BIRIMKOD = 3,
            ONAYTUR = 4,
            ONAYKOD = 5,
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMFieldsONAYTARIHCE : int
        {
            YIL = 0,
            SENARYOKOD = 1,
            MUHASEBEKOD = 2,
            BIRIMKOD = 3,
            ONAYTUR = 4,
            ISLEMTARIH = 5,
            ONAYKOD = 6,
            ISLEMIYAPAN = 7
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] FIELDS = {
            "YIL"
            , "SENARYOKOD"
            , "MUHASEBEKOD"
            , "BIRIMKOD"
            , "KURKOD"
            , "FONKOD"
            , "FINKOD"
            , "EKOKOD"
            , "PEBKOD"
            , "ODENEK"
            , "ACIKLAMA"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] FIELDSONAY = {
            "YIL"
            , "SENARYOKOD"
            , "MUHASEBEKOD"
            , "BIRIMKOD"
            , "ONAYTUR"
            , "ONAYKOD"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] FIELDSONAYTARIHCE = {
            "YIL"
            , "SENARYOKOD"
            , "MUHASEBEKOD"
            , "BIRIMKOD"
            , "ONAYTUR"
            , "ISLEMTARIH"
            , "ONAYKOD"
            , "ISLEMIYAPAN"
        };

        /*
         * 
CREATE TABLE BHMBIRIMBUTCEHAZIRLIK
(YIL INT
, SENARYOKOD VARCHAR(4)
, MUHASEBEKOD INT
, BIRIMKOD VARCHAR(20)
, KURKOD VARCHAR(20)
, FONKOD VARCHAR(20)
, FINKOD VARCHAR(20)
, EKOKOD VARCHAR(20)
, PEBKOD VARCHAR(20)
, ODENEK MONEY
, ACIKLAMA VARCHAR(4096)
, PRIMARY KEY  (YIL, SENARYOKOD, MUHASEBEKOD, BIRIMKOD, KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD)
)

CREATE TABLE BHMBIRIMONAY
(YIL INT
, SENARYOKOD VARCHAR(4)
, MUHASEBEKOD INT
, BIRIMKOD VARCHAR(20)
, ONAYTUR INT
, ONAYKOD INT
, PRIMARY KEY  (YIL, SENARYOKOD, MUHASEBEKOD, BIRIMKOD, ONAYTUR)
)
 
CREATE TABLE BHMBIRIMONAYTARIHCE
(YIL INT
, SENARYOKOD VARCHAR(4)
, MUHASEBEKOD INT
, BIRIMKOD VARCHAR(20)
, ONAYTUR INT
, ISLEMTARIH DATETIME
, ONAYKOD INT
, ISLEMIYAPAN VARCHAR(100)
, PRIMARY KEY  (YIL, SENARYOKOD, MUHASEBEKOD, BIRIMKOD, ONAYTUR, ISLEMTARIH)
)
        */


        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BirimButceHazirlik yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Delete(int yil, int saymanlik, string birim, string senaryoKod, TNS.BKD.Tertip t)
        {
            string sql = "DELETE FROM " + TABLENAME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.MUHASEBEKOD], "=", saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.KURKOD], "=", t.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.FONKOD], "=", t.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.FINKOD], "=", t.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.EKOKOD], "=", t.kodlar[3].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.PEBKOD], "=", t.kodlar[4].Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, BirimButceHazirlik yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Insert(int yil, int saymanlik, string birim, string senaryoKod, TNS.BKD.Tertip t, double odenek, string aciklama)
        {
            string sql = "INSERT INTO " + TABLENAME + "(" + VeriErisim.TumAlanlar("", FIELDS) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(saymanlik);
            sql += "," + VeriErisim.DegerYap(birim);
            sql += "," + VeriErisim.DegerYap(t.kodlar[0].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(t.kodlar[1].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(t.kodlar[2].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(t.kodlar[3].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(t.kodlar[4].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(odenek);
            sql += "," + VeriErisim.DegerYap(aciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc OnaykodSil(ENUMBirimOnayTur onayTur, int yil, string asama, int muhasebeKod, string birimKod)
        {
            string sql = "DELETE FROM BHMBIRIMONAY";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("BHMBIRIMONAY", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("BHMBIRIMONAY", "ONAYTUR", "=", (int)onayTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("BHMBIRIMONAY", "SENARYOKOD", "=", asama.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("BHMBIRIMONAY", "MUHASEBEKOD", "=", muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("BHMBIRIMONAY", "BIRIMKOD", "=", birimKod));
            return KomutCalistir(sql, 0);
        }

        public int OnaykodBul(ENUMBirimOnayTur  onayTur, int yil, string asama, int muhasebeKod, string birimKod)
        {
            string sql = "SELECT ONAYKOD FROM BHMBIRIMONAY";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("BHMBIRIMONAY", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("BHMBIRIMONAY", "ONAYTUR", "=", (int)onayTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("BHMBIRIMONAY", "SENARYOKOD", "=", asama.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("BHMBIRIMONAY", "MUHASEBEKOD", "=", muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("BHMBIRIMONAY", "BIRIMKOD", "=", birimKod));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc OnaykodAta(ENUMBirimOnayTur onayTur, int yil, string asama, int muhasebeKod, string birimKod, int onayKod)
        {
            string sql = "INSERT INTO BHMBIRIMONAY(" + VeriErisim.TumAlanlar("", FIELDSONAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(asama.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(birimKod) + ",";
            sql += VeriErisim.DegerYap((int)onayTur) + ",";
            sql += VeriErisim.DegerYap(onayKod) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc OnaykodTarihceYaz(ENUMBirimOnayTur onayTur, int yil, string asama, int muhasebeKod, string birimKod, int onayKod, string kullaniciKodu)
        {
            string sql = "INSERT INTO BHMBIRIMONAYTARIHCE(" + VeriErisim.TumAlanlar("", FIELDSONAYTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(asama.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(birimKod) + ",";
            sql += VeriErisim.DegerYap((int)onayTur) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.DegerYap(onayKod) + ",";
            sql += VeriErisim.DegerYap(kullaniciKodu) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader OnaysizBirimListele(ENUMBirimOnayTur onayTur, string asama, int yil, int ilKod, string birimKod)
        {
            string sql = "SELECT MUH.MUHASEBEKOD, MB.BIRIMKOD, BHMBIRIMONAY.ONAYKOD, MB.BIRIMAD,  MUH.MUHASEBEAD ";
            sql += "FROM ButceIslemleri.dbo.MUHMUHASEBE MUH INNER JOIN ";
            sql += "ButceIslemleri.dbo.MUHMUHASEBEBIRIM MB ON MUH.MUHASEBEKOD = MB.MUHASEBEKOD LEFT OUTER JOIN ";
            sql += "BHMBIRIMONAY ON MB.BIRIMKOD = BHMBIRIMONAY.BIRIMKOD AND ";
            sql += "MB.MUHASEBEKOD = BHMBIRIMONAY.MUHASEBEKOD AND ";
            sql += "BHMBIRIMONAY.YIL=" + VeriErisim.DegerYap(yil) + " AND BHMBIRIMONAY.ONAYTUR=" + VeriErisim.DegerYap((int)onayTur) + " AND BHMBIRIMONAY.SENARYOKOD=" + VeriErisim.DegerYap(asama.Replace(".", "")) + " ";
            sql += "where MB.MUHASEBEKOD/1000=" + VeriErisim.DegerYap(ilKod) + " AND MB.BIRIMKOD=" + VeriErisim.DegerYap(birimKod) + " AND (BHMBIRIMONAY.ONAYKOD<>5 OR BHMBIRIMONAY.ONAYKOD IS NULL)";
            sql += " AND 1 NOT IN (SELECT 1 FROM ButceIslemleri.dbo.MUHMUHASEBEBIRIMKAPALI KAP WHERE KAP.BIRIMKOD=MB.BIRIMKOD AND KAP.MUHASEBEKOD=MB.MUHASEBEKOD AND KAP.KURKOD=MB.KURKOD)";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OnayliBirimListele(ENUMBirimOnayTur onayTur, string asama, int yil, int ilKod, string birimKod)
        {
            string sql = "SELECT MUH.MUHASEBEKOD, MB.BIRIMKOD, BHMBIRIMONAY.ONAYKOD, MB.BIRIMAD,  MUH.MUHASEBEAD ";
            sql += "FROM ButceIslemleri.dbo.MUHMUHASEBE MUH INNER JOIN ";
            sql += "ButceIslemleri.dbo.MUHMUHASEBEBIRIM MB ON MUH.MUHASEBEKOD = MB.MUHASEBEKOD LEFT OUTER JOIN ";
            sql += "BHMBIRIMONAY ON MB.BIRIMKOD = BHMBIRIMONAY.BIRIMKOD AND ";
            sql += "MB.MUHASEBEKOD = BHMBIRIMONAY.MUHASEBEKOD AND ";
            sql += "BHMBIRIMONAY.YIL=" + VeriErisim.DegerYap(yil) + " AND BHMBIRIMONAY.ONAYTUR=" + VeriErisim.DegerYap((int)onayTur) + " AND BHMBIRIMONAY.SENARYOKOD=" + VeriErisim.DegerYap(asama.Replace(".", "")) + " ";
            sql += "where MB.MUHASEBEKOD/1000=" + VeriErisim.DegerYap(ilKod) + " AND MB.BIRIMKOD=" + VeriErisim.DegerYap(birimKod) + " AND (BHMBIRIMONAY.ONAYKOD=5)";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OnaykodTarihceListele(ENUMBirimOnayTur onayTur, int yil, string senaryoKod, int muhasebeKod, string birimKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", FIELDSONAYTARIHCE) + " FROM BHMBIRIMONAYTARIHCE";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ONAYTUR", "=", (int)onayTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHASEBEKOD", "=", muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BIRIMKOD", "=", birimKod));
            sql += " ORDER BY ISLEMTARIH DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc TertipleriSil(BKD.Tertip tertip, string senaryoKod, int il, int saymanlik, string birim)
        {
            string sql = "DELETE FROM " + TABLENAME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(birim))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.MUHASEBEKOD], "=", saymanlik));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.BIRIMKOD], "=", birim));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.MUHASEBEKOD] + "/1000", "=", il));
            }
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.KURKOD], "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.FONKOD], "LIKE", tertip.kodlar[1].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.FINKOD], "LIKE", tertip.kodlar[2].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.EKOKOD], "LIKE", tertip.kodlar[3].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.PEBKOD], "LIKE", tertip.kodlar[4].Replace(".", "") + "%"));
            return KomutCalistir(sql, 0);
        }

        public Sonuc TertipleriKopyala(BKD.Tertip tertip, int il, int saymanlik, string birim, int dagitimYontem, double karsilanan, double toplam)
        {
            string kaynakSql = "";
            if (dagitimYontem == 1)
                kaynakSql = KaynakHarcamaSorgu(tertip, il, saymanlik, birim, karsilanan / toplam);
            else
                kaynakSql = KaynakTalepSorgu(tertip, il, saymanlik, birim, karsilanan / toplam);


            string sql = "INSERT INTO " + TABLENAME + "(" + VeriErisim.TumAlanlar("", FIELDS) + ") " + kaynakSql;
            return KomutCalistir(sql, 0);
        }

        private string KaynakHarcamaSorgu(BKD.Tertip tertip, int il, int saymanlik, string birim, double oran)
        {
            string sql = "SELECT F.YIL+2, '0100', F.MUHASEBEKOD, F.BIRIMKOD, D.KURKOD, D.FONKOD, D.FINKOD, D.EKOKOD, D.PEBKOD, ROUND(SUM(BORC-ALACAK)*" + VeriErisim.DegerYap(oran) + ", 0), '' FROM ButceIslemleri.dbo.MUHMIFFORM F, ButceIslemleri.dbo.MUHMIF D WHERE F.YIL=D.YIL AND F.BELGENO=D.BELGENO AND F.FORMDURUM<8 AND (D.HESAPKOD LIKE '830%')";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "YIL", "=", tertip.yil - 2));
            if (!string.IsNullOrWhiteSpace(birim))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "MUHASEBEKOD", "=", saymanlik));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "BIRIMKOD", "=", birim));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "MUHASEBEKOD" + "/1000", "=", il));
            }
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "KURKOD", "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "FONKOD", "LIKE", tertip.kodlar[1].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "FINKOD", "LIKE", tertip.kodlar[2].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "EKOKOD", "LIKE", tertip.kodlar[3].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "PEBKOD", "LIKE", tertip.kodlar[4].Replace(".", "") + "%"));
            sql += " GROUP BY F.YIL, F.MUHASEBEKOD, F.BIRIMKOD, D.KURKOD, D.FONKOD, D.FINKOD, D.EKOKOD, D.PEBKOD";
            return sql;
        }

        public double HarcamaToplamBul(BKD.Tertip tertip, int il, int saymanlik, string birim)
        {
            string sql = "SELECT SUM(BORC-ALACAK) FROM ButceIslemleri.dbo.MUHMIFFORM F, ButceIslemleri.dbo.MUHMIF D WHERE F.YIL=D.YIL AND F.BELGENO=D.BELGENO AND F.FORMDURUM<8 AND (D.HESAPKOD LIKE '830%')";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "YIL", "=", tertip.yil - 2));
            if (!string.IsNullOrWhiteSpace(birim))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "MUHASEBEKOD", "=", saymanlik));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "BIRIMKOD", "=", birim));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "MUHASEBEKOD" + "/1000", "=", il));
            }
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "KURKOD", "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "FONKOD", "LIKE", tertip.kodlar[1].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "FINKOD", "LIKE", tertip.kodlar[2].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "EKOKOD", "LIKE", tertip.kodlar[3].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "PEBKOD", "LIKE", tertip.kodlar[4].Replace(".", "") + "%"));
            return veriErisim.TekKolonSQLDbl(sql);
        }

        public double TalepToplamBul(BKD.Tertip tertip, int il, int saymanlik, string birim, string p)
        {
            string sql = "SELECT SUM(ODENEK) FROM " + TABLENAME;
            sql = VeriErisim.KosulEkle(sql, "WHERE ", VeriErisim.KosulOl("", "YIL", "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", "0000"));
            if (!string.IsNullOrWhiteSpace(birim))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHASEBEKOD", "=", saymanlik));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BIRIMKOD", "=", birim));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHASEBEKOD" + "/1000", "=", il));
            }
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.KURKOD], "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.FONKOD], "LIKE", tertip.kodlar[1].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.FINKOD], "LIKE", tertip.kodlar[2].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.EKOKOD], "LIKE", tertip.kodlar[3].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.PEBKOD], "LIKE", tertip.kodlar[4].Replace(".", "") + "%"));
            return veriErisim.TekKolonSQLDbl(sql);
        }

        private string KaynakTalepSorgu(BKD.Tertip tertip, int il, int saymanlik, string birim, double oran)
        {
            string sql = "SELECT YIL, '0100', MUHASEBEKOD, BIRIMKOD, KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, ROUND(ODENEK*" + VeriErisim.DegerYap(oran) + ", 0), '' FROM " + TABLENAME;
            sql = VeriErisim.KosulEkle(sql, "WHERE ", VeriErisim.KosulOl("", "YIL", "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", "0000"));
            if (!string.IsNullOrWhiteSpace(birim))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHASEBEKOD", "=", saymanlik));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BIRIMKOD", "=", birim));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHASEBEKOD" + "/1000", "=", il));
            }
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.KURKOD], "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.FONKOD], "LIKE", tertip.kodlar[1].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.FINKOD], "LIKE", tertip.kodlar[2].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.EKOKOD], "LIKE", tertip.kodlar[3].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.PEBKOD], "LIKE", tertip.kodlar[4].Replace(".", "") + "%"));
            return sql;
        }

        public static string TalepBul(Tertip kistasTertip, string senaryoKod, int kodUzunluk, string kyKod)
        {
            string alanAd = "LEFT(BHMKURUMSALYAPI.KOD, " + kodUzunluk.ToString() + ")";
            string tumSorgu = "SELECT " + alanAd + ", SUM(ODENEK) FROM BHMBIRIMBUTCEHAZIRLIK H INNER JOIN";
            tumSorgu += " ButceIslemleri.dbo.BHMKURUMSALYAPI BHMKURUMSALYAPI ON H.MUHASEBEKOD = BHMKURUMSALYAPI.SAYMANLIKKOD AND H.BIRIMKOD = BHMKURUMSALYAPI.BIRIMKOD AND ";
            tumSorgu += " H.KURKOD = BHMKURUMSALYAPI.KURKOD";
            tumSorgu += " where H.YIL=" + VeriErisim.DegerYap(kistasTertip.yil);
            tumSorgu += " AND H.KURKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[0].Replace(".", ""));
            tumSorgu += " AND H.FONKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[1].Replace(".", ""));
            tumSorgu += " AND H.FINKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[2].Replace(".", ""));
            tumSorgu += " AND H.EKOKOD LIKE " + VeriErisim.DegerYap(kistasTertip.kodlar[3].Replace(".", "") + "%");
            tumSorgu += " AND H.PEBKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[4].Replace(".", ""));
            tumSorgu += " AND BHMKURUMSALYAPI.KOD LIKE " + VeriErisim.DegerYap(kyKod.Replace(".", "") + "%");
            tumSorgu += " group by " + alanAd + " ORDER BY 1";
            return tumSorgu;
        }

        public static string ProjeTaelpBulEko2SektorPeb2Peb4Sql(int yil, string ekoKod, string sektorKod, string pebKod)
        {
            string sektorAlan = "case when SEKTORNO IS NULL then '00' else SEKTORNO end";

            string harcamaSorgu = "SELECT LEFT(O.EKOKOD,2), " + sektorAlan + ", LEFT(O.PEBKOD,2), O.PEBKOD, SUM(O.ODENEK)";
            harcamaSorgu += " FROM Yatirim.dbo.YATALTPROJE AS AP LEFT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON " + VeriErisim.Join("P", "AP", new string[] { "YIL", "SENARYOKOD", "PROJENO" }); ;
            harcamaSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAK AS K ON " + VeriErisim.Join("AP", "K", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" }); ;
            harcamaSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS BK ON " + VeriErisim.Join("K", "BK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            harcamaSorgu += " RIGHT OUTER JOIN BHMBIRIMBUTCEHAZIRLIK AS O ON " + VeriErisim.Join("BK", "O", new string[] { "YIL", "PEBKOD" }) + " AND BK.SENARYOKOD=0600";
            harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "WHERE", VeriErisim.KosulOl("O", "YIL", "=", yil));
            if (ekoKod != null)
                harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "AND", VeriErisim.KosulOl("O", "EKOKOD", "LIKE", ekoKod.Replace(".", "") + "%"));

            if (sektorKod != null)
            {
                if (sektorKod == "00")
                    harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "AND", "SEKTORNO IS NULL");
                else
                    harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "AND", VeriErisim.KosulOl("", "SEKTORNO", "=", sektorKod.Replace(".", "")));
            }

            if (pebKod != null)
                harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "AND", VeriErisim.KosulOl("O", "PEBKOD", "LIKE", pebKod.Replace(".", "") + "%"));

            harcamaSorgu += " group by LEFT(O.EKOKOD,2), SEKTORNO, LEFT(O.PEBKOD,2), O.PEBKOD order by 1,2,3,4";
            return harcamaSorgu;
        }
    }
}
