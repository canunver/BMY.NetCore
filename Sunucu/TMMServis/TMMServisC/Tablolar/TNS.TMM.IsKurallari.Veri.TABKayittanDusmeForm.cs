using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Kayıttan Düşecek Taşınır Malzemelerin Form bilgilerini tutan TASKAYITTANDUSMEFORM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABKayittanDusmeForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKayittanDusmeForm(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASKAYITTANDUSMEFORM";

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
            /// Tutanak no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTANAKNO = 3,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 4,
            /// <summary>
            /// Tutanak tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTANAKTARIHI = 5
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD",
                                   "TUTANAKNO", "AMBARKOD", "TUTANAKTARIHI" };

        /// <summary>
        /// Kayıttan Düşme Form bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kf">Kayıttan Düşme Form bilgilerini tutan, KayittanDusmeForm türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(KayittanDusmeForm kf)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger ={ kf.yil };
            string[] strAlan ={ ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] };
            string[] strDeger ={ kf.muhasebeKod, kf.harcamaKod.Replace(".", "") };
            string tutanakNo = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.TUTANAKNO], strAlan, strDeger, intAlan, intDeger).ToString();

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kf.yil) + ",";
            sql += VeriErisim.DegerYap(kf.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(kf.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tutanakNo) + ",";
            sql += VeriErisim.DegerYap(kf.ambarKod) + ",";
            sql += VeriErisim.DegerYap(kf.tutanakTarihi) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = tutanakNo;
            return sonuc;
        }

        /// <summary>
        /// Kayıttan Düşme Form bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kf">Kayıttan Düşme Form bilgilerini tutan, KayittanDusmeForm türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Guncelle(KayittanDusmeForm kf)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], kf.ambarKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKTARIHI], kf.tutanakTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", kf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKNO], "=", kf.tutanakNo));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = kf.tutanakNo;
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Kayıttan Düşme Form bilgisini kullanarak
        /// İlgili Kayıttan Düşme Form bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kf">Kayıttan Düşme Form bilgisini tutan, Kayıttan Düşme Form türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(KayittanDusmeForm kf)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", kf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKNO], "=", kf.tutanakNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Kayıttan Düşme Form bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Kayıttan Düşme Form bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kf">Kayıttan Düşme Form bilgisini tutan, Kayıttan Düşme Form türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Kayıttan Düşme Form bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(KayittanDusmeForm kf)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + ", TASMUHASEBE.AD, TASIL.KOD, TASIL.AD, TASILCE.KOD, TASILCE.AD";
            sql += " FROM " + TABLOAD + ", TASMUHASEBE, TASIL, TASILCE";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + "TASMUHASEBE.KOD";
            sql += " AND " + "TASMUHASEBE.ILKOD=TASILCE.ILKOD";
            sql += " AND " + "TASMUHASEBE.ILCEKOD=TASILCE.KOD";
            sql += " AND " + "TASIL.KOD=TASILCE.ILKOD";

            if (kf.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kf.yil));

            if (!string.IsNullOrEmpty(kf.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kf.muhasebeKod));

            if (!string.IsNullOrEmpty(kf.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", kf.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kf.tutanakNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKNO], "=", kf.tutanakNo));

            if (!string.IsNullOrEmpty(kf.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", kf.ambarKod));

            sql += " ORDER BY " + VeriErisim.TumAlanlar("", ALANLAR);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Kayıttan Düşme Form bilgilerini kullanarak,
        /// Kayıttan Düşme Form bilgilerinin kayıtlı olup olmadığını,
        /// KayıtVarMı rutinini çalıştırarak bulan ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="kf">Kayıttan Düşme Form bilgisini tutan, Kayıttan Düşme Form türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda, kayıt var ise true yoksa false değerini veren Bool türünde nesne döndürür</returns>
        public bool KayittanDusmeFormVarMi(KayittanDusmeForm kf)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger ={ kf.yil };
            string[] strAlan ={ ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], ALANLAR[(int)ENUMAlanlar.TUTANAKNO] };
            string[] strDeger ={ kf.muhasebeKod, kf.harcamaKod.Replace(".", ""), kf.tutanakNo };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }
    }
}