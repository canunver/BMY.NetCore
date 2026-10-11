using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Ambar bilgilerini tutan TASAMBAR veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABAmbar : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAmbar(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASAMBAR";

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
            /// Harcama Birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 1,
            /// <summary>
            /// Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 2,
            /// <summary>
            /// Ad alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 3,
            /// <summary>
            /// Adres alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ADRES = 4,
            /// <summary>
            /// Kapalı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAPALI = 5,
            /// <summary>
            /// Kullanıcı Birimi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KULLANICIBIRIMI = 6,
            /// <summary>
            /// Hediye alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HEDIYE = 7
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "MUHASEBEKOD", "HARCAMABIRIMKOD", "KOD", "AD", "ADRES", "KAPALI", "KULLANICIBIRIMI", "HEDIYE" };

        /// <summary>
        /// Ambar bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ambar">Ambar bilgilerini tutan, Ambar türünde nesne</param>
        /// <returns>Ekleme, Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(Ambar ambar)
        {
            string sql = "SELECT MUHASEBEKOD, HARCAMABIRIMKOD, KOD FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", ambar.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", ambar.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", ambar.muhasebeKod));

            IDataReader dr = veriErisim.DataReaderDondur(sql);

            bool sonuc = dr.Read();
            dr.Close();

            if (sonuc) //Kayıt var, güncelle
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], ambar.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADRES], ambar.adres) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAPALI], ambar.kapali) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HEDIYE], ambar.hediye) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIBIRIMI], ambar.kullaniciBirimi);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", ambar.harcamaBirimKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", ambar.kod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", ambar.muhasebeKod));
            }
            else //Kayıt yok, ekle
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(ambar.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(ambar.harcamaBirimKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(ambar.kod) + ",";
                sql += VeriErisim.DegerYap(ambar.ad) + ",";
                sql += VeriErisim.DegerYap(ambar.adres) + ",";
                sql += VeriErisim.DegerYap(ambar.kapali) + ",";
                sql += VeriErisim.DegerYap(ambar.kullaniciBirimi) + ",";
                sql += VeriErisim.DegerYap(ambar.hediye) + ")";
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Ambar bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Ambar bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ambar">Ambar bilgilerini tutan, Ambar türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Ambar bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(Ambar ambar)
        {
            string sql = "SELECT TASIL.KOD ILKOD, TASILCE.KOD ILCEKOD, TASAMBAR.MUHASEBEKOD, TASAMBAR.HARCAMABIRIMKOD, TASAMBAR.KOD, TASAMBAR.AD, ";
            sql += " TASAMBAR.ADRES, TASHARCAMABIRIM.AD HARCAMABIRIMAD, TASMUHASEBE.AD MUHASEBEAD, TASIL.AD ILAD, TASILCE.AD ILCEAD, ";
            sql += " TASIMZA1.AD, TASIMZA1.UNVAN, TASAMBAR.KAPALI, TASAMBAR.KULLANICIBIRIMI, TASAMBAR.HEDIYE, TASIMZA2.AD, TASIMZA2.UNVAN";
            sql += " FROM TASAMBAR INNER JOIN";
            sql += " TASHARCAMABIRIM ON TASAMBAR.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD AND ";
            sql += " TASAMBAR.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD INNER JOIN";
            sql += " TASMUHASEBE ON TASHARCAMABIRIM.MUHASEBEKOD = TASMUHASEBE.KOD INNER JOIN";
            sql += " TASILCE ON TASMUHASEBE.ILKOD = TASILCE.ILKOD AND TASMUHASEBE.ILCEKOD = TASILCE.KOD INNER JOIN";
            sql += " TASIL ON TASILCE.ILKOD = TASIL.KOD";
            sql += " LEFT OUTER JOIN TASIMZA AS TASIMZA1 ON TASAMBAR.MUHASEBEKOD = TASIMZA1.MUHASEBEKOD AND TASAMBAR.HARCAMABIRIMKOD = TASIMZA1.HARCAMABIRIMKOD AND TASAMBAR.KOD = TASIMZA1.AMBARKOD and TASIMZA1.IMZAYER = 1";
            sql += " LEFT OUTER JOIN TASIMZA AS TASIMZA2 ON TASAMBAR.MUHASEBEKOD = TASIMZA2.MUHASEBEKOD AND TASAMBAR.HARCAMABIRIMKOD = TASIMZA2.HARCAMABIRIMKOD AND TASAMBAR.KOD = TASIMZA2.AMBARKOD and TASIMZA2.IMZAYER = 16";
            sql += " WHERE (TASAMBAR.KOD<>'')";
            string kosul = "";

            if (ambar != null)
            {
                if (ambar.muhasebeKod != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", ambar.muhasebeKod));

                if (ambar.harcamaBirimKod != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "LIKE", ambar.harcamaBirimKod.Replace(".", "") + "%"));

                if (ambar.kod != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", ambar.kod));

                if (ambar.ad != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + ambar.ad + "%"));

                if (ambar.kapali > -1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAPALI], "=", ambar.kapali));

                if (ambar.kullaniciBirimi > -1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIBIRIMI], "=", ambar.kullaniciBirimi));

                if (kosul != "")
                    sql += " AND " + kosul;
            }
            sql += " ORDER BY 1,2,3,4,5,6,7";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Ambar bilgilerini kullanarak
        /// ilgili Ambar bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ambar">Ambar bilgilerini tutan, Ambar türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(Ambar ambar)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", ambar.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", ambar.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", ambar.muhasebeKod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Ambar bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Ambar bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="amb">Ambar bilgilerini tutan, Ambar türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Ambar bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader VarMi(Ambar amb)
        {
            string sql = "SELECT * FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", amb.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", amb.muhasebeKod));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
