using System.Data;
using TNS.Veri;

namespace TNS.ATM.IsKurallari.Veri
{
    /// <summary>
    /// Dosya Taksit bilgilerini tutan ATMTAKSIT veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABATMDosyaTaksit : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABATMDosyaTaksit(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ATMTAKSIT";

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
            /// Dosya numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOSYANO = 1,
            /// <summary>
            /// Sıra numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,
            /// <summary>
            /// Son ödeme tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SONODEMETARIHI = 3,
            /// <summary>
            /// Miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 4,
            /// <summary>
            /// Ödeme tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMETARIHI = 5,
            /// <summary>
            /// Faiz miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FAIZMIKTAR = 6,
            /// <summary>
            /// Faiz miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TAKSITTARIHIFAIZI = 7,
            /// <summary>
            /// Makbuz numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MAKBUZNO = 8,
            /// <summary>
            /// Ödeme Emri alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 9,
            /// <summary>
            /// The faizodemeemrino
            /// </summary>
            FAIZODEMEEMRINO = 10
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "DOSYANO", "SIRANO", "SONODEMETARIHI", "MIKTAR", "ODEMETARIHI", "FAIZMIKTAR", "TAKSITTARIHIFAIZI", "MAKBUZNO", "ODEMEEMRINO", "FAIZODEMEEMRINO" };

        /// <summary>
        /// DosyaTaksit yapısında tutulan bilgileri kullanarak ödeme planı bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="taksit">Taksit bilgilerini tutan DosyaTaksit türünde nesne </param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda Sonuc türünde nesne döndürür</returns>
        public Sonuc SatirEkle(DosyaTaksit taksit)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(taksit.yil) + ",";
            sql += VeriErisim.DegerYap(taksit.dosyaNo) + ",";
            sql += VeriErisim.DegerYap(taksit.siraNo) + ",";
            sql += VeriErisim.DegerYap(taksit.sonOdemeTarihi) + ",";
            sql += VeriErisim.DegerYap(taksit.miktar) + ",";
            sql += VeriErisim.DegerYap(taksit.odemeTarihi) + ",";
            sql += VeriErisim.DegerYap(taksit.faizMiktar) + ",";
            sql += VeriErisim.DegerYap(taksit.taksitTarihiFaizi) + ",";
            sql += VeriErisim.DegerYap(taksit.makbuzNo) + ",";
            sql += VeriErisim.DegerYap(taksit.odemeEmriNo) + ",";
            sql += VeriErisim.DegerYap(taksit.faizOdemeEmriNo) + ")";

            return KomutCalistir(sql, 1);
        }


        /// <summary>
        /// DosyaTaksit yapısında tutulan bilgileri kullanarak taksitin ödendiği bilgisini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="taksit">Taksit bilgisini tutan DosyaTaksit türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda Sonuc türünde nesne döndürür</returns>
        public Sonuc Odendi(Dosya dosya, DosyaTaksit taksit)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONODEMETARIHI], taksit.sonOdemeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTAR], taksit.miktar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMETARIHI], taksit.odemeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAIZMIKTAR], taksit.faizMiktar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAKSITTARIHIFAIZI], taksit.taksitTarihiFaizi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MAKBUZNO], taksit.makbuzNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", dosya.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", dosya.dosyaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", taksit.siraNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// DosyaTaksit yapısında tutulan bilgileri kullanarak taksitin ödendiği bilgisini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="taksit">Taksit bilgisini tutan DosyaTaksit türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(Dosya dosya, DosyaTaksit taksit)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", dosya.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", dosya.dosyaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", taksit.siraNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Odemes the emri kaydet taksit.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="dosyaNo">The dosya no.</param>
        /// <param name="siraNo">The sira no.</param>
        /// <param name="belgeYil">The belge yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="faizMi">if set to <c>true</c> [faiz mi].</param>
        /// <returns></returns>
        public Sonuc OdemeEmriKaydetTaksit(int yil, int dosyaNo, int siraNo, int belgeYil, string belgeNo, bool faizMi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            if (faizMi)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAIZODEMEEMRINO], belgeYil + "/" + belgeNo);
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], belgeYil + "/" + belgeNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", dosyaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Yıl ve Dosya Numarası anahtar alanlarını kullanarak veri tabanından kayıtları listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan Int türünde nesne</param>
        /// <param name="dosyaNo">Dosya numarası bilgisini tutan Int türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda IDataReader yapısında nesne döndürür</returns>
        public IDataReader Listele(int yil, int dosyaNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (dosyaNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", dosyaNo));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yıl ve Dosya numarası anahtar alanlarını kullanarak TNS.Veri.Sayac.SayacDegerAl fonksiyonunun çalıştırarak
        /// sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan Int türünde nesne</param>
        /// <param name="dosyaNo">Dosya numarası bilgisini tutan Int türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda Int türünde nesne döndürür</returns>
        public int TaksitSayisiDondur(int yil, int dosyaNo)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.DOSYANO] };
            int[] intDeger = { yil, dosyaNo };
            return TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], new string[0], new string[0], intAlan, intDeger) - 1;
        }

        /// <summary>
        /// Yıl, Dosya Numarası ve Sıra numarası anahtar alanlarını kullanarak veri tabanından kayıtları listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve taksitlerin ödendi sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan Int türünde nesne</param>
        /// <param name="dosyaNo">Dosya numarası bilgisini tutan Int türünde nesne</param>
        /// <param name="siraNo">Sıra numarası bilgisini tutan Int türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda Bool türünde nesne döndürür</returns>
        public bool TaksitOdendiMi(int yil, int dosyaNo, int siraNo)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT(") + ALANLAR[(int)ENUMAlanlar.SIRANO] + ")" + " FROM " + TABLOAD;

            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (dosyaNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", dosyaNo));

            if (siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));

            sql += " AND " + ALANLAR[(int)ENUMAlanlar.ODEMETARIHI] + " IS NULL";

            int gelen = veriErisim.TekKolonSQLInt(sql);

            if (gelen > 0)
                return false;
            else
                return true;
        }
    }
}
