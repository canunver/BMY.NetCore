using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Kayıttan Düşecek Taşınır Malzeme Detay bilgilerini tutan TASKAYITTANDUSMEDETAY veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABKayittanDusmeDetay : TABOrtak
    {

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKayittanDusmeDetay(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASKAYITTANDUSMEDETAY";

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
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// Harcama birimi kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 2,
            /// <summary>
            /// Tutanak no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTANAKNO = 3,
            /// <summary>
            /// Hesap plan kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 4,
            /// <summary>
            /// Sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SICILNO = 5,
            /// <summary>
            /// Miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 6,
            /// <summary>
            /// Birim fiyat alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 7,
            /// <summary>
            /// Sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 8,
            /// <summary>
            /// Kdv oran alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVORAN = 9
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "TUTANAKNO",
                                   "HESAPPLANKOD", "SICILNO", "MIKTAR", "BIRIMFIYAT","SIRANO","KDVORAN" };

        /// <summary>
        /// Kayıttan Düşme Detay bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kd">Kayıttan Düşme Detay bilgilerini tutan, KayittanDusmeDetay türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(KayittanDusmeDetay kd)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kd.yil) + ",";
            sql += VeriErisim.DegerYap(kd.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(kd.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(kd.tutanakNo) + ",";
            sql += VeriErisim.DegerYap(kd.hesapPlanKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(kd.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(kd.miktar) + ",";
            sql += VeriErisim.DegerYap(kd.birimFiyat) + ",";
            sql += VeriErisim.DegerYap(kd.siraNo) + ",";
            sql += VeriErisim.DegerYap(kd.kdvOran) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Kayıttan Düşme Form bilgisini kullanarak
        /// İlgili Kayıttan Düşme Detay bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kf">Kayıttan Düşme Form bilgilerini tutan, KayittanDusmeForm türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(KayittanDusmeForm kf)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", kf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKNO], "=", kf.tutanakNo));

            return KomutCalistir(sql, 0);//Detay kayıt birden fazla olabilir.
        }

        /// <summary>
        /// Parametre olarak verilen Kayıttan Düşme Detay bilgisini kullanarak
        /// İlgili Kayıttan Düşme Detay bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kd">Kayıttan Düşme Detay bilgilerini tutan, KayittanDusmeDetay türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(KayittanDusmeDetay kd)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", kd.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKNO], "=", kd.tutanakNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Kayıttan Düşme Detay bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Kayıttan Düşme Detay bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kd">Kayıttan Düşme Detay bilgilerini tutan, KayittanDusmeDetay türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Kayıttan Düşme Detay bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(KayittanDusmeDetay kd)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASOLCUBIRIM.KOD, TASOLCUBIRIM.AD";
            sql += " FROM " + TABLOAD + ", TASHESAPPLAN, TASOLCUBIRIM";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD] + "=TASHESAPPLAN.KOD";
            sql += " AND TASHESAPPLAN.OLCUBIRIMKOD=TASOLCUBIRIM.KOD";

            if (kd.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kd.yil));

            if (!string.IsNullOrEmpty(kd.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kd.muhasebeKod));

            if (!string.IsNullOrEmpty(kd.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", kd.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kd.tutanakNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUTANAKNO], "=", kd.tutanakNo));

            sql += " ORDER BY TASKAYITTANDUSMEDETAY.YIL,TASKAYITTANDUSMEDETAY.MUHASEBEKOD,TASKAYITTANDUSMEDETAY.HARCAMABIRIMKOD,TASKAYITTANDUSMEDETAY.TUTANAKNO,TASKAYITTANDUSMEDETAY.SIRANO,TASOLCUBIRIM.KOD, TASOLCUBIRIM.AD";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}