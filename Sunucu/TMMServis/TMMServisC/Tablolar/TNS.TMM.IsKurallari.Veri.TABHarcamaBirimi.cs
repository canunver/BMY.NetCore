using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Harcama birimi bilgilerinin tutulduğu TASHARCAMABIRIM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABHarcamaBirimi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHarcamaBirimi(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASHARCAMABIRIM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// Ad alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// Kapalı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAPALI = 3
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "AD", "MUHASEBEKOD", "KAPALI" };

        /// <summary>
        /// Harcama birimi sayısını veren,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <returns>Select komutunun çalıştırılması sonucunda, Harcama Birimi sayısını tutan Int türünde nesne döndürür</returns>
        public int HarcamaBirimiSayisiDondur()
        {
            return veriErisim.TekKolonSQLInt("SELECT COUNT(*) FROM " + TABLOAD);
        }

        /// <summary>
        /// Harcama Birimi bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="harcamaBirimi">Harcama Birimi bilgilerini tutan, Harcama Birimi türünde nesne</param>
        /// <returns>Ekleme,Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(HarcamaBirimi harcamaBirimi)
        {
            string sql = "SELECT KOD, MUHASEBEKOD FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", harcamaBirimi.kod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", harcamaBirimi.muhasebeKod));

            IDataReader dr = veriErisim.DataReaderDondur(sql);

            bool sonuc = dr.Read();
            dr.Close();

            if (sonuc) //Kayıt var, güncelle
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], harcamaBirimi.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAPALI], harcamaBirimi.kapali);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", harcamaBirimi.kod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", harcamaBirimi.muhasebeKod));
            }
            else //Kayıt yok, ekle
            {
                //if (!TABLisans.LisansKontrol(veriErisim, true))
                //{
                //    //Sonuc s = new Sonuc(true, "", "");
                //    Sonuc s = new Sonuc(false, "Lisansınızı doldurmuş olduğunuz için yeni harcama birimi kayıt edemezsiniz!", "");
                //    return s;
                //}

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(harcamaBirimi.kod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(harcamaBirimi.ad) + ",";
                sql += VeriErisim.DegerYap(harcamaBirimi.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(harcamaBirimi.kapali) + ")";
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Harcama Birimi bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Harcama birimi bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="harcamaBirimi">Harcama Birimi bilgilerini tutan, Harcama Birimi türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Harcama birimi bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(HarcamaBirimi harcamaBirimi)
        {
            string sql = "SELECT TASIL.KOD ILKOD, TASILCE.KOD ILCEKOD, TASHARCAMABIRIM.MUHASEBEKOD, TASHARCAMABIRIM.KOD, TASHARCAMABIRIM.AD, ";
            sql += " TASMUHASEBE.AD MUHASEBEAD, TASIL.AD ILAD, TASILCE.AD ILCEAD, TASHARCAMABIRIM.KAPALI";
            sql += " FROM TASHARCAMABIRIM INNER JOIN";
            sql += " TASMUHASEBE ON TASHARCAMABIRIM.MUHASEBEKOD = TASMUHASEBE.KOD INNER JOIN";
            sql += " TASILCE ON TASMUHASEBE.ILKOD = TASILCE.ILKOD AND TASMUHASEBE.ILCEKOD = TASILCE.KOD INNER JOIN";
            sql += " TASIL ON TASILCE.ILKOD = TASIL.KOD";
            string kosul = "";

            if (harcamaBirimi != null)
            {
                if (harcamaBirimi.muhasebeKod != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", harcamaBirimi.muhasebeKod));

                if (harcamaBirimi.kod != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", harcamaBirimi.kod.Replace(".", "")));

                if (harcamaBirimi.ad != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + harcamaBirimi.ad + "%"));

                if (harcamaBirimi.kapali > -1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAPALI], "=", harcamaBirimi.kapali));

                if (kosul != "")
                    sql += " WHERE " + kosul;
            }
            sql += " ORDER BY 1,2,3,4,5,6,7";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Harcama Birimi bilgisini kullanarak
        /// ilgili Harcama Birimi bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="harcamaBirimi">Harcama Birimi bilgilerini tutan, Harcama Birimi türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(HarcamaBirimi harcamaBirimi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", harcamaBirimi.kod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", harcamaBirimi.muhasebeKod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Harcama Birimi bilgilerini kıstas olarak kullanarak,
        /// Kıstasa uygun Harcama Birimi bilgilerini dönen,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="hb">Harcama Birimi bilgilerini tutan, Harcama Birimi türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, Harcama birimi bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader VarMi(HarcamaBirimi hb)
        {
            string sql = "SELECT * FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", hb.muhasebeKod));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Harcama Birimi bilgilerini kıstas olarak kullanarak,
        /// Kıstasa uygun Harcama Birimi bilgilerini dönen,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="yil">Yılı tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, Harcama birimi bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader ABSKuraGoreHarcamaBirimleriListele(int yil)
        {
            string sql = @" select KURKOD, KURAD
                            from ButceHazirlik.[dbo].[ABSKUR]
                            where YIL = '" + yil + "' and (LEN(KURKOD)=8 or LEN(KURKOD)=6) and KURAD != '.' Group by KURKOD,KURAD ";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
