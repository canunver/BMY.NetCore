using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
namespace TNS.IKY.IsKurallari.Veri
{
    /// <summary>
    /// IKYHIZMETSEBEBI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIKYHIZMETSEBEBI : TABOrtak
    {
        /// <summary>
        /// TABIKYHIZMETSEBEBI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIKYHIZMETSEBEBI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "IKYSOZLUKHIZMETSEBEBI";

        /*
         * 
         CREATE TABLE IKYHIZMETSEBEBI
            (KOD INT
            , SEBEPADI VARCHAR(200)
            , TIPADI VARCHAR(20)
            , TIP INT
            , PRIMARY KEY (KOD)
            )
         * 
         * */
        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// SEBEPADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SEBEPADI = 1,

            /// <summary>
            /// TIPADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TIPADI = 2,

            /// <summary>
            /// TIP alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TIP = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "KOD"
            , "SEBEPADI"
            , "TIPADI"
            , "TIP"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, HizmetSebebi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(HizmetSebebi form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, HizmetSebebi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(HizmetSebebi form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.sebepAdi);
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap((int)form.tip);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, HizmetSebebi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleHizmetSebebi(HizmetSebebi form)
        {
            string sql = "";
            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                if (form.tip != ENUMHizmetSebebiNevi.TANIMSIZ)
                {
                    string inSorgu = "";
                    if (form.tip == ENUMHizmetSebebiNevi.BASLAMAGRUBU)
                        inSorgu = HizmetSebebi.BaslamaGrubuListe();
                    else if (form.tip == ENUMHizmetSebebiNevi.AYRILMAGRUBU)
                        inSorgu = HizmetSebebi.AyrilmaGrubuListe();
                    else
                        inSorgu = ((int)form.tip).ToString();
                    sql = VeriErisim.KosulEkle(sql, "AND", "TIP IN (" + inSorgu + ")");
                }

                if (!string.IsNullOrWhiteSpace(form.sebepAdi))
                    sql = VeriErisim.KosulEkle(sql, "AND", "SEBEPADI LIKE " + VeriErisim.DegerYap(form.sebepAdi + "%"));
            }
            if (!string.IsNullOrWhiteSpace(sql)) sql = " WHERE " + sql;
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD + sql + " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TIP] + ", " + ALANLAR[(int)ENUMAlanlar.SEBEPADI];
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Guncelle(HizmetSebebi form)
        {
            string sql = "UPDATE " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "SET", VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SEBEPADI], form.sebepAdi));
            sql = VeriErisim.KosulEkle(sql, ",", VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIP], (int)form.tip));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 1);
        }
    }
}
