using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari.Veri
{
    public class TABMPGTAVAN : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMPGTAVAN(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "MPTAVAN";

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
            /// SENARYOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DETAYMI = 1,

            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,

            /// <summary>
            /// FONKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKOD = 3,

            /// <summary>
            /// FINKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKOD = 4,

            /// <summary>
            /// EKOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 5,

            /// <summary>
            /// PEBKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 6,

            /// <summary>
            /// PEBKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KYKOD = 7,

            /// <summary>
            /// ODENEK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK = 8,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "DETAYMI"
            , "KURKOD"
            , "FONKOD"
            , "FINKOD"
            , "EKOKOD"
            , "PEBKOD"
            , "KYKOD"
            , "ODENEK"
        };

        public static string TABLOADONAY = "MPTAVANONAY";

        public enum ENUMAlanlarONAY : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// SENARYOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DETAYMI = 1,

            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,

            /// <summary>
            /// ODENEK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYKOD = 3,
        }

        public static string[] ALANLARONAY = {
            "YIL"
            , "DETAYMI"
            , "KURKOD"
            , "ONAYKOD"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADIZIN = "MPTAVANIZIN";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarIzin : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// FONKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKOD = 2,

            /// <summary>
            /// FINKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKOD = 3,

            /// <summary>
            /// EKOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 4,

            /// <summary>
            /// PEBKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 5,

        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARIZIN = {
            "YIL"
            , "KURKOD"
            , "FONKOD"
            , "FINKOD"
            , "EKOKOD"
            , "PEBKOD"
        };


        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, OdenekSinifyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, int detayMi, Tertip tertip, string kyKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DETAYMI], "=", detayMi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", tertip.kodlar[3].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));
            if (!string.IsNullOrEmpty(kyKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KYKOD], "LIKE", kyKod.Replace(".", "") + "%"));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, OdenekSinifyapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int yil, int detayMi, TertipOdenek to)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(detayMi);
            sql += "," + VeriErisim.DegerYap(to.tertip.kodlar[0].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(to.tertip.kodlar[1].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(to.tertip.kodlar[2].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(to.tertip.kodlar[3].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(to.tertip.kodlar[4].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(to.tertip.kodlar[5].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(to.odenek[0]);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc TertipIzniSil(Tertip tertip)
        {
            string sql = "DELETE FROM " + TABLOADIZIN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        public Sonuc TertipIzniEkle(Tertip tertip)
        {
            string sql = "INSERT INTO " + TABLOADIZIN + "(" + VeriErisim.TumAlanlar("", ALANLARIZIN) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(tertip.yil);
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", ""));
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public int TertipIzniSay(Tertip tertip)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOADIZIN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));
            return (int)this.veriErisim.TekKolonSQLDbl(sql);
        }

        public IDataReader ListeleIzin(Tertip kistasTertip)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARIZIN) + " FROM " + TABLOADIZIN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.YIL], "=", kistasTertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.KURKOD], "LIKE", kistasTertip.kodlar[0].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.FONKOD], "LIKE", kistasTertip.kodlar[1].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.FINKOD], "LIKE", kistasTertip.kodlar[2].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EKOKOD], "LIKE", kistasTertip.kodlar[3].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.PEBKOD], "LIKE", kistasTertip.kodlar[4].Replace(".", "") + "%"));
            return this.veriErisim.DataReaderDondur(sql);
        }

        public static string ListeleIzinSQL(Tertip kistasTertip)
        {
            string sql = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, 1 FROM " + TABLOADIZIN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.YIL], "=", kistasTertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.KURKOD], "LIKE", kistasTertip.kodlar[0].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.FONKOD], "LIKE", kistasTertip.kodlar[1].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.FINKOD], "LIKE", kistasTertip.kodlar[2].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EKOKOD], "LIKE", kistasTertip.kodlar[3].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.PEBKOD], "LIKE", kistasTertip.kodlar[4].Replace(".", "") + "%"));
            return sql;
        }

        public static string ListeleTavan(Tertip kistasTertip, int detayMi)
        {
            string sql = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, SUM(ODENEK) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kistasTertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DETAYMI], "=", detayMi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kistasTertip.kodlar[0].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "LIKE", kistasTertip.kodlar[1].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "LIKE", kistasTertip.kodlar[2].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", kistasTertip.kodlar[3].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "LIKE", kistasTertip.kodlar[4].Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KYKOD], "LIKE", kistasTertip.kodlar[5].Replace(".", "") + "%"));
            sql += " GROUP BY KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD";
            return sql;
        }


        public IDataReader ListeleRapor(string alanIsimleri, Tertip kistasTertip)
        {
            string sql = "SELECT " + alanIsimleri + ", SUM(ODENEK) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kistasTertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DETAYMI], "=", 1));
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[0]))
                sql += VeriErisim.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], kistasTertip.kodlar[0], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[1]))
                sql += VeriErisim.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], kistasTertip.kodlar[1], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[2]))
                sql += VeriErisim.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], kistasTertip.kodlar[2], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[3]))
                sql += VeriErisim.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], kistasTertip.kodlar[3], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[4]))
                sql += VeriErisim.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], kistasTertip.kodlar[4], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[5]))
                sql += VeriErisim.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KYKOD], kistasTertip.kodlar[5], " AND ");
            sql += " GROUP BY " + alanIsimleri;
            sql += " ORDER BY " + alanIsimleri;
            return this.veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Onayla(int yil, int detayMi, string kurKod, int onayKod)
        {
            string sql = "UPDATE " + TABLOADONAY + " SET ONAYKOD = " + VeriErisim.DegerYap(onayKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.DETAYMI], "=", detayMi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.KURKOD], "=", kurKod.Replace(".", "")));
            Sonuc s = KomutCalistir(sql, 1);
            if (!s.islemSonuc)
            {
                this.veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOADONAY + "(" + VeriErisim.TumAlanlar("", ALANLARONAY) + ") VALUES (";
                sql += VeriErisim.DegerYap(yil) + ", ";
                sql += VeriErisim.DegerYap(detayMi) + ", ";
                sql += VeriErisim.DegerYap(kurKod.Replace(".", "")) + ", ";
                sql += VeriErisim.DegerYap(onayKod) + ")";
                return KomutCalistir(sql, 1);
            }

            return s;
        }

        public int OnayKoduBul(int yil, int detayMi, string kurKod)
        {
            string sql = "SELECT ONAYKOD FROM " + TABLOADONAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.DETAYMI], "=", detayMi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.KURKOD], "=", kurKod.Replace(".", "")));
            return this.veriErisim.TekKolonSQLInt(sql);
        }

        public static string TavanBul(Tertip kistasTertip, bool detayMi, int kodUzunluk, string kyKod)
        {
            string alanAd = "LEFT(KYKOD, " + kodUzunluk.ToString() + ")";
            string tumSorgu = "SELECT " + alanAd + ", SUM(ODENEK) FROM MPTAVAN";
            tumSorgu += " where YIL=" + VeriErisim.DegerYap(kistasTertip.yil);
            tumSorgu += " AND DETAYMI = " + VeriErisim.DegerYap(detayMi ? 1 : 0);
            tumSorgu += " AND KURKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[0].Replace(".", ""));
            tumSorgu += " AND FONKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[1].Replace(".", ""));
            tumSorgu += " AND FINKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[2].Replace(".", ""));
            tumSorgu += " AND EKOKOD LIKE " + VeriErisim.DegerYap(kistasTertip.kodlar[3].Replace(".", "") + "%");
            tumSorgu += " AND PEBKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[4].Replace(".", ""));
            tumSorgu += " AND KYKOD LIKE " + VeriErisim.DegerYap(kyKod.Replace(".", "") + "%");
            tumSorgu += " group by " + alanAd + " ORDER BY 1";
            return tumSorgu;
        }

        public IDataReader HataliTertipleriBul(Tertip tertip, int detayMi)
        {
            string alanAd;
            if (detayMi == 1)
                alanAd = "KY.KOD";
            else
                alanAd = "LEFT(KY.KOD,3)";
            string sql = "select GON.KURKOD, GON.FONKOD, GON.FINKOD, GON.EKOKOD, GON.PEBKOD, GON.KYKOD, GON.GONDERILEN, MP.ODENEK from ";
            sql += "(SELECT  GON.KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, " + alanAd + " KYKOD, SUM(MIKTAR) GONDERILEN ";
            sql += "FROM ButceIslemleri.dbo.MUHGONDERMETENKIS GON ";
            sql += "INNER JOIN ButceIslemleri.dbo.MUHGONDERMETENKISFORM GF ON ";
            sql += "GON.YIL=GF.YIL AND GON.BELGENO=GF.BELGENO AND GON.BELGETUR=GF.BELGETUR ";
            sql += "INNER JOIN ButceIslemleri.dbo.BHMKURUMSALYAPI KY ON KY.BIRIMKOD=GF.BIRIMKOD AND KY.SAYMANLIKKOD = GF.MUHASEBEKOD AND KY.KURKOD=GON.KURKOD ";
            sql += "where GON.YIL=" + VeriErisim.DegerYap(tertip.yil) + " AND GF.FORMDURUM<8 ";
            //sql += "AND 1 IN (SELECT DISTINCT 1 FROM ButceHazirlikV2.dbo.MPTAVAN MPT where MPT.YIL=GON.YIL AND MPT.DETAYMI=0 AND MPT.KURKOD=GON.KURKOD AND MPT.FONKOD=GON.FONKOD AND MPT.FINKOD=GON.FINKOD AND MPT.EKOKOD=GON.EKOKOD AND MPT.PEBKOD=GON.PEBKOD AND MPT.ODENEK>0) ";
            sql += "group by GON.KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, " + alanAd + ") GON ";
            sql += "LEFT OUTER  JOIN ";
            sql += "(SELECT KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD,  KYKOD, ODENEK from ButceHazirlikV2.dbo.MPTAVAN where YIL=" + VeriErisim.DegerYap(tertip.yil) + " AND DETAYMI=" + VeriErisim.DegerYap(detayMi) + ") MP ";
            sql += "ON MP.KURKOD = GON.KURKOD AND ";
            sql += "MP.FONKOD=GON.FONKOD AND ";
            sql += "MP.FINKOD=GON.FINKOD AND ";
            sql += "MP.EKOKOD=GON.EKOKOD AND ";
            sql += "MP.PEBKOD=GON.PEBKOD AND ";
            sql += "MP.KYKOD=GON.KYKOD ";
            sql += "where (ODENEK IS NULL OR ODENEK<GONDERILEN)";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("GON", "KURKOD", "LIKE", OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 0, "").Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("GON", "FONKOD", "LIKE", OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 1, "").Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("GON", "FINKOD", "LIKE", OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 2, "").Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("GON", "EKOKOD", "LIKE", OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 3, "").Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("GON", "PEBKOD", "LIKE", OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 4, "").Replace(".", "") + "%"));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
