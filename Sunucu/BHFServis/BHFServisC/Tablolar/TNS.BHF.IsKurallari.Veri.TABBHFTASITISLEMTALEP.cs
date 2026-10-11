using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHF;
namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// BHFTASITISLEMTALEP veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHFTASITISLEMTALEP : TABOrtak
    {
        /// <summary>
        /// TABBHFTASITISLEMTALEP sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHFTASITISLEMTALEP(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHFTASITISLEMTALEP";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// SASENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SASENO = 0,

            /// <summary>
            /// TALEPNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TALEPNO = 1,

            /// <summary>
            /// TALEPTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TALEPTUR = 2,

            /// <summary>
            /// GIDECEGISAYMANLIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GIDECEGISAYMANLIK = 3,

            /// <summary>
            /// GIDECEGIBIRIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GIDECEGIBIRIM = 4,

            /// <summary>
            /// SAHIPLIKDEGISMESIN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAHIPLIKDEGISMESIN = 5,

            /// <summary>
            /// AYRILDIGITARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AYRILDIGITARIH = 6,

            /// <summary>
            /// AYRILDIGIEVRAKSAYI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AYRILDIGIEVRAKSAYI = 7,

            /// <summary>
            /// ONAYKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYKODU = 8,

            /// <summary>
            /// TALEPYAPAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TALEPYAPAN = 9,

            /// <summary>
            /// ISLEMYAPAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPAN = 10,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "SASENO"
            , "TALEPNO"
            , "TALEPTUR"
            , "GIDECEGISAYMANLIK"
            , "GIDECEGIBIRIM"
            , "SAHIPLIKDEGISMESIN"
            , "AYRILDIGITARIH"
            , "AYRILDIGIEVRAKSAYI"
            , "ONAYKODU"
            , "TALEPYAPAN"
            , "ISLEMYAPAN"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, TasitIslemTalep yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TasitIslemTalep form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SASENO], "=", form.saseNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TALEPNO], "=", form.talepNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, TasitIslemTalep yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(TasitIslemTalep form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.saseNo);
            sql += "," + VeriErisim.DegerYap(form.talepNo);
            sql += "," + VeriErisim.DegerYap((int)form.talepTur);
            sql += "," + VeriErisim.DegerYap(form.gidecegiSaymanlik);
            sql += "," + VeriErisim.DegerYap(form.gidecegiBirim);
            sql += "," + VeriErisim.DegerYap(form.sahiplikDegismesin);
            sql += "," + VeriErisim.DegerYap(form.ayrildigiTarih);
            sql += "," + VeriErisim.DegerYap(form.ayrildigiEvrakSayi);
            sql += "," + VeriErisim.DegerYap(1);
            sql += "," + VeriErisim.DegerYap(form.talepYapan);
            sql += "," + VeriErisim.DegerYap("");
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, TasitIslemTalep yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(TasitIslemTalep form)
        {
            string sql = "";
            if (!string.IsNullOrWhiteSpace(form.saseNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SASENO], "=", form.saseNo));
            if (form.talepNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TALEPNO], "=", form.talepNo));
            if (form.onayKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ONAYKODU], "=", form.onayKodu));

            string select = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", MUH1.MUHASEBEAD, BIR1.BIRIMAD, D.BULUNDUGUSAYMANLIK, D.BULUNDUGUBIRIM, MUHY.MUHASEBEAD, BIRY.BIRIMAD, D.PLAKANO FROM " + TABLOAD;
            select += " INNER JOIN BHFTASITBILGIDETAY D ON D.SASENO=BHFTASITISLEMTALEP.SASENO AND D.SIRANO=0";
            select += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBE MUHY ON " + VeriErisim.Join("D", "MUHY", new string[] { "BULUNDUGUSAYMANLIK" }, new string[] { "MUHASEBEKOD" });
            select += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM BIRY ON " + VeriErisim.Join("D", "BIRY", new string[] { "BULUNDUGUSAYMANLIK", "BULUNDUGUBIRIM" }, new string[] { "MUHASEBEKOD", "BIRIMKOD" });
            select += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBE MUH1 ON " + VeriErisim.Join(TABLOAD, "MUH1", new string[] { ALANLAR[(int)ENUMAlanlar.GIDECEGISAYMANLIK] }, new string[] { "MUHASEBEKOD" });
            select += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM BIR1 ON " + VeriErisim.Join(TABLOAD, "BIR1", new string[] { ALANLAR[(int)ENUMAlanlar.GIDECEGISAYMANLIK], ALANLAR[(int)ENUMAlanlar.GIDECEGIBIRIM] }, new string[] { "MUHASEBEKOD", "BIRIMKOD" });

            select = VeriErisim.KosulEkle(select, "WHERE", sql);

            return veriErisim.DataReaderDondur(select);
        }

        public int TalepNoBul(string saseNo)
        {
            string sql = "SELECT MAX(TALEPNO) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SASENO], "=", saseNo));
            return ((int)this.veriErisim.TekKolonSQLDbl(sql)) + 1;
        }

        public Sonuc OnayKodAta(TasitIslemTalep form, int onayKod, string kullaniciKodu)
        {
            string sql = "UPDATE  " + TABLOAD;
            sql += " SET ONAYKODU = " + VeriErisim.DegerYap(onayKod);
            sql += ", ISLEMYAPAN = " + VeriErisim.DegerYap(kullaniciKodu);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SASENO], "=", form.saseNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TALEPNO], "=", form.talepNo));
            return KomutCalistir(sql, 0);
        }
    }
}
