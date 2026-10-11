using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır Sayım Form bilgilerini tutan TASSAYIMFORM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABSayimForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSayimForm(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASSAYIMFORM";

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
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 2,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 3,
            /// <summary>
            /// Sayım no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYIMNO = 4,
            /// <summary>
            /// Sayım tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYIMTARIH = 5
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "AMBARKOD", "SAYIMNO", "SAYIMTARIH" };

        /// <summary>
        /// Sayım Form bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sf">Sayım Form bilgisini tutan, SayimForm türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(SayimForm sf)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger ={ sf.yil };
            string[] strAlan ={ ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], ALANLAR[(int)ENUMAlanlar.AMBARKOD] };
            string[] strDeger ={ sf.muhasebeKod, sf.harcamaKod.Replace(".", ""), sf.ambarKod };
            string sayimNo = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYIMNO], strAlan, strDeger, intAlan, intDeger).ToString();

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(sf.yil) + ",";
            sql += VeriErisim.DegerYap(sf.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(sf.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sf.ambarKod) + ",";
            sql += VeriErisim.DegerYap(sayimNo) + ",";
            sql += VeriErisim.DegerYap(sf.sayimTarih) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = sayimNo;
            return sonuc;
        }

        /// <summary>
        /// Sayım Form bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sf">Sayım Form bilgisini tutan, SayimForm türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Guncelle(SayimForm sf)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYIMTARIH], sf.sayimTarih);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", sf.ambarKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYIMNO], "=", sf.sayimNo));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = sf.sayimNo;
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Sayım Form bilgisini kullanarak
        /// Sayım Form bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sf">Sayım Form bilgisini tutan, SayimForm türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(SayimForm sf)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", sf.ambarKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYIMNO], "=", sf.sayimNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Sayım Form bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Sayım Form bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sf">Sayım Form bilgisini tutan, SayimForm türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Sayım Form bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(SayimForm sf)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASIL.KOD, TASIL.AD, TASILCE.KOD, TASILCE.AD";
            sql += " FROM " + TABLOAD + ", TASAMBAR, TASHARCAMABIRIM, TASMUHASEBE, TASILCE, TASIL";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + "TASAMBAR.MUHASEBEKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] + "=" + "TASAMBAR.HARCAMABIRIMKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AMBARKOD] + "=" + "TASAMBAR.KOD";
            sql += " AND " + "TASAMBAR.MUHASEBEKOD" + "=" + "TASHARCAMABIRIM.MUHASEBEKOD";
            sql += " AND " + "TASAMBAR.HARCAMABIRIMKOD" + "=" + "TASHARCAMABIRIM.KOD";
            sql += " AND " + "TASHARCAMABIRIM.MUHASEBEKOD" + "=" + "TASMUHASEBE.KOD";
            sql += " AND " + "TASMUHASEBE.ILKOD" + "=" + "TASILCE.ILKOD";
            sql += " AND " + "TASMUHASEBE.ILCEKOD" + "=" + "TASILCE.KOD";
            sql += " AND " + "TASILCE.ILKOD" + "=" + "TASIL.KOD";

            if (sf.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sf.yil));

            if (!string.IsNullOrEmpty(sf.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sf.muhasebeKod));

            if (!string.IsNullOrEmpty(sf.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sf.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(sf.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", sf.ambarKod));

            if (!string.IsNullOrEmpty(sf.sayimNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYIMNO], "=", sf.sayimNo));

            sql += " ORDER BY " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASIL.KOD, TASIL.AD, TASILCE.KOD, TASILCE.AD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sayım Form bilgisini kullanarak,
        /// Sayım Form bilgisinin kayıtlı olup olmadığını,
        /// KayıtVarMı rutinini çalıştırarak bulan ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="sf">Sayım Form bilgisini tutan, SayimForm türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda, kayıt var ise true yoksa false değerini veren Bool türünde nesne döndürür</returns>
        public bool SayimFormVarMi(SayimForm sf)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger ={ sf.yil };
            string[] strAlan ={ ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], ALANLAR[(int)ENUMAlanlar.AMBARKOD], ALANLAR[(int)ENUMAlanlar.SAYIMNO] };
            string[] strDeger ={ sf.muhasebeKod, sf.harcamaKod.Replace(".", ""), sf.ambarKod, sf.sayimNo };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }
    }
}