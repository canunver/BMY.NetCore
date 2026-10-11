using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır Sayım RFID bilgilerini tutan TASSAYIMRFID veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABSayimRFID : TABOrtak
    {

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSayimRFID(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASSAYIMRFID";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Sayım no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYIMNO = 1,
            /// <summary>
            /// Sayım adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYIMADI = 2,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 3,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 4,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 5,
            /// <summary>
            /// Sayım tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYIMTARIH = 6,
            TCKIMLIKNO = 7,
            ODA = 8,
            DURUM = 9
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "SAYIMNO", "SAYIMADI", "MUHASEBEKOD", "HARCAMABIRIMKOD", "AMBARKOD", "SAYIMTARIH", "TCKIMLIKNO", "ODA", "DURUM" };

        /// <summary>
        /// Sayım RFID bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="srfid">Sayım RFID bilgisini tutan, SayimRFID türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(SayimRFID srfid)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(srfid.yil) + ",";
            sql += VeriErisim.DegerYap(srfid.sayimNo) + ",";
            sql += VeriErisim.DegerYap(srfid.sayimAdi) + ",";
            sql += VeriErisim.DegerYap(srfid.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(srfid.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(srfid.ambarKod) + ",";
            sql += VeriErisim.DegerYap(srfid.sayimTarih) + ",";
            sql += VeriErisim.DegerYap(srfid.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(srfid.odaKod) + ",";
            sql += VeriErisim.DegerYap((int)srfid.durum) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(SayimRFID srfid)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYIMADI], srfid.sayimAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], srfid.muhasebeKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], srfid.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], srfid.ambarKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYIMTARIH], srfid.sayimTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], srfid.tcKimlikNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODA], srfid.odaKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)srfid.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", srfid.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYIMNO], "=", srfid.sayimNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Sayım RFID bilgisini kullanarak
        /// Sayım RFID bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="srfid">Sayım RFID bilgisini tutan, SayimRFID türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(SayimRFID srfid)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", srfid.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYIMNO], "=", srfid.sayimNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Sayım RFID bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Sayım RFID bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="srfid">Sayım RFID bilgisini tutan, SayimRFID türünde nesne</param>
        /// <param name="yetkiDeger">Yetki değer bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Sayım RFID bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(SayimRFID srfid, string[] yetkiDeger)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASMUHASEBE.AD, TASHARCAMABIRIM.AD, TASAMBAR.AD";
            sql += " FROM " + TABLOAD + ", TASAMBAR, TASHARCAMABIRIM, TASMUHASEBE";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + "TASAMBAR.MUHASEBEKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] + "=" + "TASAMBAR.HARCAMABIRIMKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AMBARKOD] + "=" + "TASAMBAR.KOD";
            sql += " AND " + "TASAMBAR.MUHASEBEKOD" + "=" + "TASHARCAMABIRIM.MUHASEBEKOD";
            sql += " AND " + "TASAMBAR.HARCAMABIRIMKOD" + "=" + "TASHARCAMABIRIM.KOD";
            sql += " AND " + "TASHARCAMABIRIM.MUHASEBEKOD" + "=" + "TASMUHASEBE.KOD";

            if (srfid.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", srfid.yil));

            if (!string.IsNullOrEmpty(srfid.sayimNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYIMNO], "=", srfid.sayimNo));

            if (!string.IsNullOrEmpty(srfid.sayimAdi))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYIMADI], "LIKE", "%" + srfid.sayimAdi + "%"));

            if (!string.IsNullOrEmpty(srfid.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", srfid.muhasebeKod));

            if (!string.IsNullOrEmpty(srfid.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", srfid.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(srfid.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", srfid.ambarKod));

            if (!string.IsNullOrEmpty(srfid.tcKimlikNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", srfid.tcKimlikNo));

            if (!string.IsNullOrEmpty(srfid.odaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODA], "=", srfid.odaKod));

            if (srfid.durum > (int)ENUMSayimRFIDDurum.TANIMSIZ)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)srfid.durum));

            string orluYetkiler = TABGenel.YetkiDegerOrla(yetkiDeger, TABLOAD);
            if (orluYetkiler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluYetkiler);

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SAYIMTARIH];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}