using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCIHLKABUL veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCIHLKABUL : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCIHLKABUL(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /*
         * 
         * 
CREATE TABLE HRCIHLKABUL
    (SURECYIL INT
    , SURECNO VARCHAR(25)
    , TUTANAKNO INT
    , TUTANAKTUR INT
    , FIRMANO VARCHAR(255)
    , FIRMAMUHASEBEKOD INT
    , SOZLESMETARIHI DATETIME
    , SOZLESMEBEDELI MONEY
    , FATURATARIHI DATETIME
    , FATURANO VARCHAR(25)
    , FATURATUTARI MONEY
    , ISINSURESI VARCHAR(150)
    , SOZBITISTARIHI DATETIME
    , SUREUZATIM VARCHAR(200)
    , SONBITISTARIHI DATETIME
    , ONAYTARIHI DATETIME
    , TUTANAKTARIHI DATETIME
    PRIMARY KEY (SURECYIL, SURECNO, TUTANAKNO)
    )
         
         * 
         * */


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCIHLKABUL";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// SURECYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECYIL = 0,

            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,

            /// <summary>
            /// TUTANAKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTANAKNO = 2,

            /// <summary>
            /// TUTANAKTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTANAKTUR = 3,

            /// <summary>
            /// FIRMANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FIRMANO = 4,

            /// <summary>
            /// FIRMAMUHASEBEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FIRMAMUHASEBEKOD = 5,

            /// <summary>
            /// SOZLESMETARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOZLESMETARIHI = 6,

            /// <summary>
            /// SOZLESMEBEDELI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOZLESMEBEDELI = 7,

            /// <summary>
            /// FATURATARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURATARIHI = 8,

            /// <summary>
            /// FATURANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURANO = 9,

            /// <summary>
            /// FATURATUTARI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURATUTARI = 10,

            /// <summary>
            /// ISINSURESI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISINSURESI = 11,

            /// <summary>
            /// SOZBITISTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOZBITISTARIHI = 12,

            /// <summary>
            /// SUREUZATIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SUREUZATIM = 13,

            /// <summary>
            /// SONBITISTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SONBITISTARIHI = 14,

            /// <summary>
            /// ONAYTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYTARIHI = 15,

            /// <summary>
            /// TUTANAKTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTANAKTARIHI = 16,

            MALSIRANO = 17,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "SURECYIL"
            , "SURECNO"
            , "TUTANAKNO"
            , "TUTANAKTUR"
            , "FIRMANO"
            , "FIRMAMUHASEBEKOD"
            , "SOZLESMETARIHI"
            , "SOZLESMEBEDELI"
            , "FATURATARIHI"
            , "FATURANO"
            , "FATURATUTARI"
            , "ISINSURESI"
            , "SOZBITISTARIHI"
            , "SUREUZATIM"
            , "SONBITISTARIHI"
            , "ONAYTARIHI"
            , "TUTANAKTARIHI"
            , "MALSIRANO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, IhaleKabulyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(IhaleKabul form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", form.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", form.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKNO], "=", form.tutanakNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, IhaleKabulyapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(IhaleKabul form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.surecYil);
            sql += "," + VeriErisim.DegerYap(form.surecNo);
            sql += "," + VeriErisim.DegerYap(form.tutanakNo);
            sql += "," + VeriErisim.DegerYap((int)form.tutanakTur);
            sql += "," + VeriErisim.DegerYap(form.vergiNo);
            sql += "," + VeriErisim.DegerYap(form.firmaMuhasebeKod);
            sql += "," + VeriErisim.DegerYap(form.sozlesmeTarihi);
            sql += "," + VeriErisim.DegerYap(form.sozlesmeBedeli);
            sql += "," + VeriErisim.DegerYap(form.faturaTarihi);
            sql += "," + VeriErisim.DegerYap(form.faturaNo);
            sql += "," + VeriErisim.DegerYap(form.faturaTutari);
            sql += "," + VeriErisim.DegerYap(form.isinSuresi);
            sql += "," + VeriErisim.DegerYap(form.sozBitisTarihi);
            sql += "," + VeriErisim.DegerYap(form.sureUzatim);
            sql += "," + VeriErisim.DegerYap(form.sonBitisTarihi);
            sql += "," + VeriErisim.DegerYap(form.onayTarihi);
            sql += "," + VeriErisim.DegerYap(form.tutanakTarihi);
            sql += "," + VeriErisim.DegerYap(form.malSiraNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public int EnBuyukNo(IhaleKabul form)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.TUTANAKNO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", form.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", form.surecNo));
            return (int)this.veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, IhaleKabulyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(IhaleKabul form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", " + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD] + " FROM " + TABLOAD + " LEFT OUTER JOIN " + TABFirmaBilgisi.TABLOAD + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAMUHASEBEKOD] + " = " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.MUHASEBEKOD];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMANO] + " = " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.VERGINO];
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", form.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", form.surecNo));
            if (form.tutanakNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKNO], "=", form.tutanakNo));
            if (form.malSiraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MALSIRANO], "=", form.malSiraNo));
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
