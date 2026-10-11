using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
namespace TNS.IKY.IsKurallari.Veri
{
    /// <summary>
    /// IKYSENDIKABILGI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIKYSENDIKABILGI : TABOrtak
    {
        /// <summary>
        /// TABIKYSENDIKABILGI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIKYSENDIKABILGI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSB = "IKYSENDIKABILGI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSB : int
        {

            /// <summary>
            /// SENDIKANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SENDIKANO = 0,

            /// <summary>
            /// SENDIKAAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SENDIKAAD = 1,

            /// <summary>
            /// ADRES alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ADRES = 2,

            /// <summary>
            /// IBAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IBAN = 3,

            /// <summary>
            /// HESAPNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPNO = 4,

            /// <summary>
            /// TOPLUGORUSMEPUANI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TOPLUGORUSMEPUANI = 5,

            /// <summary>
            /// TOPLUGORUSMEPUANI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KESINTIORANI = 6,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSB = {
            "SENDIKANO"
            , "SENDIKAAD"
            , "ADRES"
            , "IBAN"
            , "HESAPNO"
            , "TOPLUGORUSMEPUANI"
            , "KESINTIORANI"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, SendikaBilgi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(SendikaBilgi form)
        {
            string sql = "DELETE FROM " + TABLOADSB;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSB[(int)ENUMAlanlarSB.SENDIKANO], "=", form.sendikaNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, SendikaBilgi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(SendikaBilgi form)
        {
            string sql = "INSERT INTO " + TABLOADSB + "(" + VeriErisim.TumAlanlar("", ALANLARSB) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.sendikaNo);
            sql += "," + VeriErisim.DegerYap(form.sendikaAd);
            sql += "," + VeriErisim.DegerYap(form.adres);
            sql += "," + VeriErisim.DegerYap(form.iBAN);
            sql += "," + VeriErisim.DegerYap(form.hesapNo);
            sql += "," + VeriErisim.DegerYap(form.topluGorusmePuani);
            sql += "," + VeriErisim.DegerYap(form.kesintiOrani);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public int SendikaNoBul(SendikaBilgi form)
        {
            string sql = $"SELECT MAX({ALANLARSB[(int)ENUMAlanlarSB.SENDIKANO]}) FROM {TABLOADSB}";
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, SendikaBilgi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(SendikaBilgi form)
        {
            string sql = "UPDATE " + TABLOADSB;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARSB[(int)ENUMAlanlarSB.SENDIKAAD], form.sendikaAd);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSB[(int)ENUMAlanlarSB.ADRES], form.adres);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSB[(int)ENUMAlanlarSB.IBAN], form.iBAN);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSB[(int)ENUMAlanlarSB.HESAPNO], form.hesapNo.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSB[(int)ENUMAlanlarSB.TOPLUGORUSMEPUANI], form.topluGorusmePuani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSB[(int)ENUMAlanlarSB.KESINTIORANI], form.kesintiOrani);
            sql += "WHERE " + VeriErisim.AtamaOl("", ALANLARSB[(int)ENUMAlanlarSB.SENDIKANO], form.sendikaNo);
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, SendikaBilgi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(SendikaBilgi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSB, ALANLARSB) + " FROM " + TABLOADSB;
            if (form.sendikaNo > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSB[(int)ENUMAlanlarSB.SENDIKANO], "=", form.sendikaNo));
            sql += " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "IKYPERSONELSENDIKA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// SENDIKANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SENDIKANO = 0,

            /// <summary>
            /// PKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PKOD = 1,

            /// <summary>
            /// PKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KESINTIORANI = 2
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "SENDIKANO"
            , "PKOD"
            , "KESINTIORANI"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="pKod">Çıkartılacak kişinin personel kodu</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc PersonelSendikadanCikart(int pKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PKOD], "=", pKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// pKod anahtar alanını kullanarak, ilgili kaydın kesinti oranını değiştiren
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="pKod">Oranı değiştirilecek kişinin personel kodu</param>
        /// <param name="kesintiOrani">Yeni oran</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc PersonelSendikaOranDegistir(int pKod, double kesintiOrani)
        {
            string sql = $"UPDATE {TABLOAD} SET {ALANLAR[(int)ENUMAlanlar.KESINTIORANI]}={VeriErisim.DegerYap(kesintiOrani)} WHERE {ALANLAR[(int)ENUMAlanlar.PKOD]} = {VeriErisim.DegerYap(pKod)}";
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, sendikaPersonel dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, SendikaBilgi yapısında nesne</param>
        /// <param name="satirNo">sendikaPersonel dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int sendikaNo, int pKod, double kesintiOran)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(sendikaNo);
            sql += "," + VeriErisim.DegerYap(pKod);
            sql += "," + VeriErisim.DegerYap(kesintiOran);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, SendikaBilgi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleSendPers(int sendikaNo, string tcKimlikNo)
        {
            string sql = $"SELECT KOD, TCKIMLIKNO, AD, SOYAD, KURUMSICILNO, KESINTIORANI, SENDIKANO FROM {TABLOAD} S";
            sql += $" INNER JOIN {TABPersonel.TABLOAD} P ON P.KOD = S.PKOD";

            string kosul = "";
            if (sendikaNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENDIKANO], "=", sendikaNo));

            if (!string.IsNullOrWhiteSpace(tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "TCKIMLIKNO", "=", tcKimlikNo));

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            sql += " ORDER BY 3,4";
            return veriErisim.DataReaderDondur(sql);
        }

        internal static string ListeleUstTablo(string ustTabloAlan)
        {
            string sql = $"SELECT {VeriErisim.TumAlanlar("S", ALANLARSB)}, {"P." + ALANLAR[(int)ENUMAlanlar.KESINTIORANI]} FROM {TABLOADSB} S INNER JOIN {TABLOAD} P ON S.SENDIKANO = P.SENDIKANO WHERE P.PKOD = {ustTabloAlan}";

            return sql;
        }

        public int SendikadaKoduBul(int pKod)
        {
            string sql = $"SELECT {ALANLAR[(int)ENUMAlanlar.SENDIKANO]} FROM {TABLOAD} S WHERE {ALANLAR[(int)ENUMAlanlar.PKOD]} = {VeriErisim.DegerYap(pKod)}";

            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}
