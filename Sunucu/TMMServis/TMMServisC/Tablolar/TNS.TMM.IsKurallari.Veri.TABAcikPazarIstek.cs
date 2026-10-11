using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// İhtiyaç Fazlası Taşınır Malzemelere ait isteklerle ilgili bilgileri tutan TASACIKPAZARISTEK veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABAcikPazarIstek : TABOrtak
    {

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAcikPazarIstek(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASACIKPAZARISTEK";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 1,
            /// <summary>
            /// Sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SICILNO = 2,
            /// <summary>
            /// İstek Yapan Muhasebe Kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISTEKYAPANMUHASEBEKOD = 3,
            /// <summary>
            /// İstek Yapan Harcama Biri kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISTEKYAPANHARCAMABIRIMKOD = 4,
            /// <summary>
            /// İstek tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISTEKTARIHI = 5
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "MUHASEBEKOD", "HARCAMABIRIMKOD", "SICILNO", "ISTEKYAPANMUHASEBEKOD", "ISTEKYAPANHARCAMABIRIMKOD", "ISTEKTARIHI" };

        /// <summary>
        /// İhtiyaç Fazlası Taşınır Malzeme istek bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="istek">İhtiyaç Fazlası Taşınır Malzeme istek bilgilerini tutan, AcikPazarIstek türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(AcikPazarIstek istek)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(istek.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(istek.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(istek.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(istek.istekYapanMuhasebeKod) + ",";
            sql += VeriErisim.DegerYap(istek.istekYapanHarcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(istek.istekTarihi) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Paremetre olarak verilen İhtiyaç Fazlası Taşınır Malzeme Detay bilgilerini kullanarak 
        /// İlgili İhtiyaç Fazlası Taşınır Malzeme İstek bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="detay">İhtiyaç Fazlası Taşınır Malzeme Detay bilgilerini tutan, AcikPazarDetay türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc SilGiris(AcikPazarDetay detay)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", detay.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", detay.harcamaKod.Replace(".", "")));
            
            if (detay.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SICILNO], "=", detay.prSicilNo));

            sql += " AND NOT " + VeriErisim.IslecBul("EXISTS");
            sql += " (";
            sql += "SELECT * FROM TASACIKPAZARDETAY";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + "TASACIKPAZARDETAY.MUHASEBEKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] + "=" + "TASACIKPAZARDETAY.HARCAMABIRIMKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SICILNO] + "=" + "TASACIKPAZARDETAY.SICILNO";
            sql += ")";
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Paremetre olarak verilen İhtiyaç Fazlası Taşınır Malzeme Form bilgilerini kullanarak 
        /// İlgili İhtiyaç Fazlası Taşınır Malzeme İstek bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="apf">İhtiyaç Fazlası Taşınır Malzeme Form bilgilerini tutan, AcikPazarForm türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc SilSorgu(AcikPazarForm apf)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql += " WHERE " + VeriErisim.IslecBul("EXISTS");
            sql += " (";
            sql += "SELECT 1 FROM TASACIKPAZARDETAY, TASAMBARSICILVIEW, TASMUHASEBE";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + "TASACIKPAZARDETAY.MUHASEBEKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] + "=" + "TASACIKPAZARDETAY.HARCAMABIRIMKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SICILNO] + "=" + "TASACIKPAZARDETAY.SICILNO";
            sql += " AND " + "TASACIKPAZARDETAY.MUHASEBEKOD" + "=" + "TASAMBARSICILVIEW.MUHASEBEKOD";
            sql += " AND " + "TASACIKPAZARDETAY.HARCAMABIRIMKOD" + "=" + "TASAMBARSICILVIEW.HARCAMABIRIMKOD";
            sql += " AND " + "TASACIKPAZARDETAY.SICILNO" + "=" + "TASAMBARSICILVIEW.PRSICILNO";
            sql += " AND " + "TASAMBARSICILVIEW.MUHASEBEKOD" + "=" + "TASMUHASEBE.KOD";

            if (!string.IsNullOrEmpty(apf.hesapKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASAMBARSICILVIEW", "HESAPPLANKOD", "LIKE", apf.hesapKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(apf.ilKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASMUHASEBE", "ILKOD", "=", apf.ilKod));

            if (!string.IsNullOrEmpty(apf.ilceKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASMUHASEBE", "ILCEKOD", "=", apf.ilceKod));

            sql += ")";

            if (!string.IsNullOrEmpty(apf.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", apf.muhasebeKod));

            if (!string.IsNullOrEmpty(apf.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", apf.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(apf.istekYapanMuhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISTEKYAPANMUHASEBEKOD], "=", apf.istekYapanMuhasebeKod));

            if (!string.IsNullOrEmpty(apf.istekYapanHarcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISTEKYAPANHARCAMABIRIMKOD], "=", apf.istekYapanHarcamaKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen İhtiyaç Fazlası Taşınır Malzeme İstek bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır Malzeme İstek bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="istek">İhtiyaç Fazlası Taşınır Malzeme İstek bilgilerini tutan, AcikPazarIstek türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun İhtiyaç Fazlası Taşınır Malzeme İstek bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(AcikPazarIstek istek)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(istek.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", istek.muhasebeKod));

            if (!string.IsNullOrEmpty(istek.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", istek.harcamaKod.Replace(".", "")));

            if (istek.prSicilNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SICILNO], "=", istek.prSicilNo));

            if (!string.IsNullOrEmpty(istek.istekYapanMuhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISTEKYAPANMUHASEBEKOD], "=", istek.istekYapanMuhasebeKod));

            if (!string.IsNullOrEmpty(istek.istekYapanHarcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISTEKYAPANHARCAMABIRIMKOD], "=", istek.istekYapanHarcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISTEKTARIHI];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}