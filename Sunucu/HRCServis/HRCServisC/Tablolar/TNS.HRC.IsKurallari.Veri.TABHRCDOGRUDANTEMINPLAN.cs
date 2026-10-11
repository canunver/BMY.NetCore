using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCDOGRUDANTEMINPLAN veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCDOGRUDANTEMINPLAN : TABOrtak
    {
        /// <summary>
        /// TABHRCDOGRUDANTEMINPLAN sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCDOGRUDANTEMINPLAN(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /*
         * 
    CREATE TABLE HRCDOGRUDANTEMINPLAN
    (YIL INT
    , SENARYOKOD VARCHAR(4) NOT NULL 
    , SAYMANLIK INT
    , BIRIM VARCHAR(20)
    , TASHESAPKOD VARCHAR(50)
    , MIKTAR MONEY
    , BIRIMFIYAT MONEY
    , KANUNMADDESI VARCHAR(50)
    , HARCAMATURU VARCHAR(50)
    PRIMARY KEY (YIL, SAYMANLIK, BIRIM, TASHESAPKOD)
    )
         * 
         * */

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCDOGRUDANTEMINPLAN";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// SAYMANLIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYMANLIK = 1,

            /// <summary>
            /// BIRIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIM = 2,

            /// <summary>
            /// TASHESAPKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TASHESAPKOD = 3,

            /// <summary>
            /// TASHESAPAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TASHESAPAD = 4,

            /// <summary>
            /// BIRIMAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMAD = 5,

            /// <summary>
            /// MIKTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 6,

            /// <summary>
            /// BIRIMFIYAT alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 7,

            /// <summary>
            /// KANUNMADDESI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KANUNMADDESI = 8,

            /// <summary>
            /// HARCAMATURU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMATURU = 9,

            SENARYOKOD = 10,
            DTHARIC = 11,
            ONCEALIM = 12,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "SAYMANLIK"
            , "BIRIM"
            , "TASHESAPKOD"
            , "TASHESAPAD"
            , "BIRIMAD"
            , "MIKTAR"
            , "BIRIMFIYAT"
            , "KANUNMADDESI"
            , "HARCAMATURU"
            , "SENARYOKOD"
            , "DTHARIC"
            , "ONCEALIM"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DogrudanTeminPlan yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, string senaryoKod, int saymanlik, string birim, string harcamaTuru)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMATURU], "=", harcamaTuru));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, dogrudanTeminPlan dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DogrudanTeminPlan yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(DogrudanTeminPlan form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.saymanlik);
            sql += "," + VeriErisim.DegerYap(form.birim);
            sql += "," + VeriErisim.DegerYap(form.tasHesapKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.tasHesapAd);
            sql += "," + VeriErisim.DegerYap(form.birimAd);
            sql += "," + VeriErisim.DegerYap(form.miktar);
            sql += "," + VeriErisim.DegerYap(form.birimFiyat);
            sql += "," + VeriErisim.DegerYap(form.kanunMaddesi);
            sql += "," + VeriErisim.DegerYap(form.harcamaTuru);
            sql += "," + VeriErisim.DegerYap(form.senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.dtHaric);
            sql += "," + VeriErisim.DegerYap(form.onceAlim);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DogrudanTeminPlan yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(DogrudanTeminPlan form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", HRCMUHASEBEVIEW.MUHASEBEAD, HRCMUHASEBEBIRIMVIEW.BIRIMAD, HRCOKAS.OKASKOD, HRCOKAS.AD FROM " + TABLOAD;

            sql += " LEFT OUTER JOIN HRCMUHASEBEVIEW ON HRCMUHASEBEVIEW.MUHASEBEKOD = HRCDOGRUDANTEMINPLAN.SAYMANLIK ";
            sql += " LEFT OUTER JOIN HRCMUHASEBEBIRIMVIEW ON HRCMUHASEBEBIRIMVIEW.MUHASEBEKOD = HRCDOGRUDANTEMINPLAN.SAYMANLIK AND HRCMUHASEBEBIRIMVIEW.BIRIMKOD=HRCDOGRUDANTEMINPLAN.BIRIM ";
            sql += " LEFT OUTER JOIN HRCOKAS ON HRCOKAS.KOD = HRCDOGRUDANTEMINPLAN.TASHESAPKOD ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            if (form.talepNo >= -1)
            {
                if (form.talepNo >= 0)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", form.talepNo.ToString("0000")));
                }
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", form.senaryoKod.Replace(".", "")));
            if (form.il != 0)
            {
                if (form.il < 0) form.il = 0;
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SAYMANLIK] + "/1000", "=", form.il));

            }
            else if (form.saymanlik > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));
            }
            if (!string.IsNullOrWhiteSpace(form.harcamaTuru))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMATURU], "=", form.harcamaTuru));
            }
            sql += " ORDER BY 1,2,3,10,9,5";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
