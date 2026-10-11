using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// İstek form bilgilerini tutan TASISTEKFORM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABIstekForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIstekForm(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASISTEKFORM";

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
            /// Belge no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGENO = 4,
            /// <summary>
            /// Belge tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGETARIHI = 5,
            /// <summary>
            /// İstek yapan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISTEKYAPAN = 6,
            HEDIYELIK = 7
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "AMBARKOD",
                                   "BELGENO", "BELGETARIHI", "ISTEKYAPAN", "HEDIYELIK" };

        /// <summary>
        /// İstek Form bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="iForm">İstek Form bilgilerini tutan, IstekForm türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(IstekForm iForm)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = { iForm.yil };
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], ALANLAR[(int)ENUMAlanlar.AMBARKOD] };
            string[] strDeger = { iForm.muhasebeKod, iForm.harcamaKod.Replace(".", ""), iForm.ambarKod };
            string belgeNo = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], strAlan, strDeger, intAlan, intDeger).ToString();

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(iForm.yil) + ",";
            sql += VeriErisim.DegerYap(iForm.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(iForm.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(iForm.ambarKod) + ",";
            sql += VeriErisim.DegerYap(belgeNo) + ",";
            sql += VeriErisim.DegerYap(iForm.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(iForm.istekYapanKod) + ",";
            sql += VeriErisim.DegerYap(iForm.hediyelik) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = belgeNo;
            return sonuc;
        }

        /// <summary>
        /// İstek Form bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="iForm">İstek Form bilgilerini tutan, IstekForm türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Guncelle(IstekForm iForm)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIHI], iForm.belgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISTEKYAPAN], iForm.istekYapanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HEDIYELIK], iForm.hediyelik);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", iForm.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", iForm.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", iForm.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", iForm.ambarKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", iForm.belgeNo));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = iForm.belgeNo;
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen İstek Form bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun İstek Form bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="iForm">İstek Form bilgilerini tutan, IstekForm türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun İstek Form bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(IstekForm iForm)
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

            if (iForm.hediyelik > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDIYELIK], "=", 1));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDIYELIK], "=", 0));
                sql = VeriErisim.KosulEkle(sql, "OR", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HEDIYELIK] + " IS NULL");
                sql += ")"; 
            }

            if (iForm.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", iForm.yil));

            if (!string.IsNullOrEmpty(iForm.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", iForm.muhasebeKod));

            if (!string.IsNullOrEmpty(iForm.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "LIKE", iForm.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(iForm.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", iForm.ambarKod));

            if (!string.IsNullOrEmpty(iForm.belgeNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "=", iForm.belgeNo));

            if (!string.IsNullOrEmpty(iForm.istekYapanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISTEKYAPAN], "=", iForm.istekYapanKod));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AMBARKOD];
            sql += ", " + "CAST(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGENO] + " AS INT)";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen İstek Form bilgilerini kullanarak,
        /// İstek Form bilgilerinin kayıtlı olup olmadığını,
        /// KayıtVarMı rutinini çalıştırarak bulan ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="iForm">İstek Form bilgilerini tutan, IstekForm türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda, kayıt var ise true yoksa false değerini veren Bool türünde nesne döndürür</returns>
        public bool IstekFormVarMi(IstekForm iForm)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = { iForm.yil };
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], ALANLAR[(int)ENUMAlanlar.AMBARKOD], ALANLAR[(int)ENUMAlanlar.BELGENO] };
            string[] strDeger = { iForm.muhasebeKod, iForm.harcamaKod.Replace(".", ""), iForm.ambarKod, iForm.belgeNo };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }
    }
}