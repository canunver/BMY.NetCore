using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHF;
namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// BHFKIRALIKARAC veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHFKIRALIKARAC : TABOrtak
    {
        /// <summary>
        /// TABBHFKIRALIKARAC sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// CREATE TABLE BHFKIRALIKARAC
        ///    (YIL VARCHAR(50)
        ///    , SOZLESMEKOD INT
        ///    , ARACSIRANO INT
        ///    , TASITCINSI INT
        ///    , ENBUYUKYAS INT
        ///    , KOLTUKSAYISI INT
        ///    , YAKITTURU INT
        ///    , SOFORDAHIL INT
        ///    , YAKITDAHIL INT
        ///    , ADET INT
        ///    , SURE INT
        ///    , SUREBIRIMI INT
        ///    , BIRIMFIYAT INT
        ///    , PRIMARY KEY(YIL, SOZLESMEKOD, ARACSIRANO)
        ///    )
        ///
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHFKIRALIKARAC(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHFKIRALIKARAC";

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
            /// SOZLESMEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOZLESMEKOD = 1,

            /// <summary>
            /// ARACSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ARACSIRANO = 2,

            /// <summary>
            /// TASITCINSI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TASITCINSI = 3,

            /// <summary>
            /// ENBUYUKYAS alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ENBUYUKYAS = 4,

            /// <summary>
            /// KOLTUKSAYISI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOLTUKSAYISI = 5,

            /// <summary>
            /// YAKITTURU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAKITTURU = 6,

            /// <summary>
            /// SOFORDAHIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOFORDAHIL = 7,

            /// <summary>
            /// YAKITDAHIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAKITDAHIL = 8,

            /// <summary>
            /// ADET alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ADET = 9,

            /// <summary>
            /// SURE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURE = 10,

            /// <summary>
            /// SUREBIRIMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SUREBIRIMI = 11,

            /// <summary>
            /// BIRIMFIYAT alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 12,

            /// <summary>
            /// PEBKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 13,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "SOZLESMEKOD"
            , "ARACSIRANO"
            , "TASITCINSI"
            , "ENBUYUKYAS"
            , "KOLTUKSAYISI"
            , "YAKITTURU"
            , "SOFORDAHIL"
            , "YAKITDAHIL"
            , "ADET"
            , "SURE"
            , "SUREBIRIMI"
            , "BIRIMFIYAT"
            , "PEBKOD"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, KiralikAracSozlesmesi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, int sozlesmeKod, int aracSiraNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", sozlesmeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ARACSIRANO], "=", aracSiraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, kiralikArac dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, KiralikAracSozlesmesi yapısında nesne</param>
        /// <param name="satirNo">kiralikArac dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int yil, int sozlesmeKod, KiralikArac ka)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(sozlesmeKod);
            sql += "," + VeriErisim.DegerYap(ka.aracSiraNo);
            sql += "," + VeriErisim.DegerYap(ka.tasitCinsi);
            sql += "," + VeriErisim.DegerYap(ka.enBuyukYas);
            sql += "," + VeriErisim.DegerYap(ka.koltukSayisi);
            sql += "," + VeriErisim.DegerYap((int)ka.yakitTuru);
            sql += "," + VeriErisim.DegerYap(ka.soforDahil);
            sql += "," + VeriErisim.DegerYap(ka.yakitDahil);
            sql += "," + VeriErisim.DegerYap(ka.adet);
            sql += "," + VeriErisim.DegerYap(ka.sure);
            sql += "," + VeriErisim.DegerYap((int)ka.sureBirimi);
            sql += "," + VeriErisim.DegerYap(ka.birimFiyat);
            sql += "," + VeriErisim.DegerYap(string.IsNullOrWhiteSpace(ka.pebKod) ? null : ka.pebKod.Replace(".", ""));
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Bilgisi okunacak aracın/araçların sözleşme yılı</param>
        /// <param name="sozlesmeKod">Bilgisi okunacak aracın/araçların sözleşmesinin kodu</param>
        /// <param name="aracSiraNo">Eğer tek araç okunacaksa aracın sözleşme içindeki sıra numarası</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int yil, int sozlesmeKod, int aracSiraNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("KA", ALANLAR) + ", BHFTASITCINS.CINS, BHFTASITCINS.TKODU, BHFTASITCINS.TIPKODU, BHFTASITTIP.TIP, PK.PEBAD FROM " + TABLOAD + " KA ";
            sql += " LEFT OUTER JOIN BHFTASITCINS ON BHFTASITCINS.KOD = KA.TASITCINSI";
            sql += " LEFT OUTER JOIN BHFTASITTIP ON BHFTASITTIP.KOD = BHFTASITCINS.TIPKODU";
            sql += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD PK ON PK.YIL = KA.YIL AND PK.PEBKOD = KA.PEBKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("KA", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KA", ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", sozlesmeKod));
            if (aracSiraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KA", ALANLAR[(int)ENUMAlanlar.ARACSIRANO], "=", aracSiraNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public int NoBul(int yil, int sozlesmeKod)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.ARACSIRANO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", sozlesmeKod));
            double dbl = veriErisim.TekKolonSQLDbl(sql);
            return Convert.ToInt32(dbl + 1);
        }

        public Sonuc Guncelle(int yil, int sozlesmeKod, KiralikArac ka)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASITCINSI], ka.tasitCinsi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ENBUYUKYAS], ka.enBuyukYas);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOLTUKSAYISI], ka.koltukSayisi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKITTURU], (int)ka.yakitTuru);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOFORDAHIL], ka.soforDahil);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKITDAHIL], ka.yakitDahil);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADET], ka.adet);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURE], ka.sure);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SUREBIRIMI], (int)ka.sureBirimi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMFIYAT], ka.birimFiyat);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], string.IsNullOrWhiteSpace(ka.pebKod) ? null : ka.pebKod.Replace(".", ""));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", sozlesmeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ARACSIRANO], "=", ka.aracSiraNo));
            return KomutCalistir(sql, 1);
        }

        internal static string ListeleUstTablo(string yilAlanAd, string sozlesmeKodAlanAd)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("KA", ALANLAR) + ", BHFTASITCINS.CINS, BHFTASITCINS.TKODU, BHFTASITCINS.TIPKODU, BHFTASITTIP.TIP, PK.PEBAD FROM " + TABLOAD + " KA ";
            sql += " LEFT OUTER JOIN BHFTASITCINS ON BHFTASITCINS.KOD = KA.TASITCINSI";
            sql += " LEFT OUTER JOIN BHFTASITTIP ON BHFTASITTIP.KOD = BHFTASITCINS.TIPKODU";
            sql += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD PK ON PK.YIL = KA.YIL AND PK.PEBKOD = KA.PEBKOD";
            sql += " WHERE KA." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + yilAlanAd;
            sql += " AND KA." + ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD] + " = " + sozlesmeKodAlanAd;
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ARACSIRANO];
            return sql;
        }
    }
}
