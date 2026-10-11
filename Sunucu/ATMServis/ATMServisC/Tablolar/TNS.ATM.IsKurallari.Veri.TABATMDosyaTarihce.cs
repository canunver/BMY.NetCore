using System.Data;
using TNS.Veri;

namespace TNS.ATM.IsKurallari.Veri
{
    /// <summary>
    /// Dosya Durum Tarihçe bilgilerini tutan ATMDOSYADURUMTARIHCE veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABATMDosyaDurumTarihce : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABATMDosyaDurumTarihce(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ATMDOSYADURUMTARIHCE";

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
            /// Durum kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUMKODU = 3,
            /// <summary>
            /// Tarih alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 4,
            /// <summary>
            /// Açıklama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 5,
            /// <summary>
            /// Kullanıcı kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 6
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "DOSYANO", "SIRANO", "DURUMKODU", "TARIH", "ACIKLAMA", "KULLANICIKODU" };


        
        /// <summary>
        /// Dosya Durum Tarihçe tipinde bulunan bilgileri veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="durumTarihce">Bilgileri tutan DosyaDurumTarihce türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür</returns>
        public Sonuc Kaydet(DosyaDurumTarihce durumTarihce)
        {
            string[] siraNoStr = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.DOSYANO] };
            int[] siraNoInt = { durumTarihce.yil, durumTarihce.dosyaNo };
            int siraNo = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], new string[0], new string[0], siraNoStr, siraNoInt);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(durumTarihce.yil) + ",";
            sql += VeriErisim.DegerYap(durumTarihce.dosyaNo) + ",";
            sql += VeriErisim.DegerYap(siraNo) + ",";
            sql += VeriErisim.DegerYap(durumTarihce.durum.kod) + ",";
            sql += VeriErisim.DegerYap(durumTarihce.tarih) + ",";
            sql += VeriErisim.DegerYap(durumTarihce.aciklama) + ",";
            sql += VeriErisim.DegerYap(durumTarihce.kullaniciKodu) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = siraNo.ToString();

            return sonuc;
        }

        
        /// <summary>
        /// Yıl ve Dosya numarası anahtar alanları kullanarak
        /// veri tabanında kayıtları listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
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

    }
}
