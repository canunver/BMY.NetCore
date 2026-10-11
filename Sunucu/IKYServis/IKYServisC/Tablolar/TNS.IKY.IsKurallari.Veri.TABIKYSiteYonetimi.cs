using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
namespace TNS.IKY.IsKurallari.Veri
{
    /// <summary>
    /// IKYPERSONELTIPI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIKYSiteYonetimi : TABOrtak
    {
        /// <summary>
        /// TABIKYPERSONELTIPI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIKYSiteYonetimi(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOSOZLUKGIDER = "IKYSOZLUKGIDER";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSOZLUKGIDER : int
        {
            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// AD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1,

            /// <summary>
            /// ELDENGOR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ELDENGOR = 2,

            /// <summary>
            /// ENDEXGOR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ENDEXGOR = 3,

            /// <summary>
            /// ACIKLAMAGOR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMAGOR = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARSOZLUKGIDER = {
            "KOD"
            , "AD"  
            , "ELDENGOR"
            , "ENDEXGOR"
            , "ACIKLAMAGOR"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOSITEGIDER = "IKYSITEGIDER";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMALANLARSITEGIDER : int
        {
            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// SITEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SITEKOD = 1,

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 2,

            /// <summary>
            /// AY alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AY = 3,

            /// <summary>
            /// GIDERKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GIDERKOD = 4,

            /// <summary>
            /// OTOMATIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OTOMATIK = 5,

            /// <summary>
            /// MIKTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 6,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 7,

            /// <summary>
            /// ESKIKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ESKIKOD = 8,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARSITEGIDER = {
            "KOD"
            , "SITEKOD"
            , "YIL" 
            , "AY"
            , "GIDERKOD"
            , "OTOMATIK"
            , "MIKTAR"
            , "ACIKLAMA"
            , "ESKIKOD"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOSITEKESINTISI = "IKYPERSONELSITEKESINTISI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMALANLARSITEKESINTISI : int
        {
            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// SKTKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SKTKOD = 1,

            /// <summary>
            /// GIDERKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GIDERKOD = 2,

            /// <summary>
            /// AKTIF alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AKTIF = 3,

            /// <summary>
            /// ENDEX alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ENDEX = 4,

            /// <summary>
            /// TUTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTAR = 5,

            /// <summary>
            /// ELDEN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ELDEN = 6,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARSITEKESINTISI = {
            "KOD"
            , "SKTKOD"
            , "GIDERKOD"
            , "AKTIF"
            , "ENDEX"
            , "TUTAR"
            , "ELDEN"
            , "ACIKLAMA"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOSITEKESINTITOPLAM = "IKYPERSONELSITEKESINTITOPLAM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMALANLARSITEKESINTITOPLAM : int
        {
            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// PLOJMANKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PLOJMANKOD = 1,

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 2,

            /// <summary>
            /// AY alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AY = 3,

            /// <summary>
            /// TOPLAMTUTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TOPLAMTUTAR = 4
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARSITEKESINTITOPLAM = {
            "KOD"
            , "PLOJMANKOD"
            , "YIL"
            , "AY"
            , "TOPLAMTUTAR"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOKESINTI = "IKYPERSONELKESINTI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMALANLARKESINTI : int
        {
            /// <summary>
            /// PKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PKOD = 0,

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 1,

            /// <summary>
            /// AY alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AY = 2,

            /// <summary>
            /// GIDERKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GIDERKOD = 3,

            /// <summary>
            /// PLOJMANKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PLOJMANKOD = 4,

            /// <summary>
            /// AKTIF alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AKTIF = 5,

            /// <summary>
            /// ENDEX alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ENDEX = 6,

            /// <summary>
            /// TUTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTAR = 7,

            /// <summary>
            /// ELDEN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ELDEN = 8,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 9,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARKESINTI = {
            "PKOD"
            , "YIL"
            , "AY"
            , "GIDERKOD"
            , "PLOJMANKOD"
            , "AKTIF"
            , "ENDEX"
            , "TUTAR"
            , "ELDEN"
            , "ACIKLAMA"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, int kod nesnesi</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilSozlukGider(int kod)
        {
            string sql = "DELETE FROM " + TABLOSOZLUKGIDER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.KOD], "=", kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, SozlukGider yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KaydetSozlukGider(SozlukGider form)
        {
            string sql = "INSERT INTO " + TABLOSOZLUKGIDER + "(" + VeriErisim.TumAlanlar("", ALANLARSOZLUKGIDER) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.ad);
            sql += "," + VeriErisim.DegerYap(form.eldenGor);
            sql += "," + VeriErisim.DegerYap(form.endexGor);
            sql += "," + VeriErisim.DegerYap(form.aciklamaGor);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Güncellenecek kayıt bilgisini tutan, SozlukGider yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleSozlukGider(SozlukGider form)
        {
            string sql = "";

            sql = "UPDATE " + TABLOSOZLUKGIDER + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.AD], form.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.ELDENGOR], form.eldenGor) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.ENDEXGOR], form.endexGor) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.ACIKLAMAGOR], form.aciklamaGor);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, SozlukGider yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuSozlukGider(SozlukGider form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOSOZLUKGIDER, ALANLARSOZLUKGIDER) + " FROM " + TABLOSOZLUKGIDER;

            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOSOZLUKGIDER + "." + ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.KOD], "=", form.kod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " order by " + ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.KOD];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, int kod nesnesi</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilSiteGider(int kod)
        {
            string sql = "DELETE FROM " + TABLOSITEGIDER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.KOD], "=", kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, SiteGider yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KaydetSiteGider(SiteGider form)
        {
            string sql = "INSERT INTO " + TABLOSITEGIDER + "(" + VeriErisim.TumAlanlar("", ALANLARSITEGIDER) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.siteKod);
            sql += "," + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.ay);
            sql += "," + VeriErisim.DegerYap(form.giderKod);
            sql += "," + VeriErisim.DegerYap(form.otomatik ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(form.miktar);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += "," + VeriErisim.DegerYap(form.eskiKod);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Güncellenecek kayıt bilgisini tutan, SiteGider yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleSiteGider(SiteGider form)
        {
            string sql = "";

            sql = "UPDATE " + TABLOSITEGIDER + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.SITEKOD], form.siteKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.YIL], form.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.AY], form.ay) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.GIDERKOD], form.giderKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.OTOMATIK], form.otomatik ? 1 : 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.MIKTAR], form.miktar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.ACIKLAMA], form.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, SiteGider yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleSiteGider(SiteGider form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOSITEGIDER, ALANLARSITEGIDER) + ", ";
            sql += TABLOSOZLUKGIDER + "." + ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.AD] + ", ";
            sql += TABLOSOZLUKGIDER + "." + ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.KOD];
            sql += " FROM " + TABLOSITEGIDER;
            sql += " right outer join " + TABLOSOZLUKGIDER + " on " + TABLOSITEGIDER + "." + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.GIDERKOD] + " = " + TABLOSOZLUKGIDER + "." + ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.KOD];

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABLOSITEGIDER + "." + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.SITEKOD], "=", form.siteKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABLOSITEGIDER + "." + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABLOSITEGIDER + "." + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.AY], "=", form.ay));
            sql += " order by " + TABLOSOZLUKGIDER + "." + ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.KOD];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, SiteGider yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuSiteGider(SiteGider form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOSITEGIDER, ALANLARSITEGIDER) + ", ";
            sql += TABLOSOZLUKGIDER + "." + ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.AD];
            sql += " , s.SITEADI ";
            sql += " FROM " + TABLOSITEGIDER;
            sql += " inner join " + TABLOSOZLUKGIDER + " on " + TABLOSITEGIDER + "." + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.GIDERKOD] + " = " + TABLOSOZLUKGIDER + "." + ALANLARSOZLUKGIDER[(int)ENUMAlanlarSOZLUKGIDER.KOD];
            sql += " inner join IKYLOJMANSITE s on s.kod = " + TABLOSITEGIDER + "." + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.SITEKOD];

            if (!form.varMi && form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOSITEGIDER + "." + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.KOD], "=", form.kod));

            if (form.siteKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOSITEGIDER + "." + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.SITEKOD], "=", form.siteKod));

            if (form.giderKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOSITEGIDER + "." + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.GIDERKOD], "=", form.giderKod));

            if (form.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOSITEGIDER + "." + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.YIL], "=", form.yil));

            if (form.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOSITEGIDER + "." + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.AY], "=", form.ay));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " order by " + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.SITEKOD] + ", " + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.YIL] + " DESC, " + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.AY] + " DESC, " + ALANLARSITEGIDER[(int)ENUMALANLARSITEGIDER.GIDERKOD];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, int kod nesnesi</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilPersonelSiteKesintisi(PersonelSiteKesinti p)
        {
            string sql = "DELETE FROM " + TABLOSITEKESINTISI;
            string kosul = "";

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.KOD], "=", p.kod));
            else
            {
                if (p.siteKesTopKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.SKTKOD], "=", p.siteKesTopKod));
            }

            if (!string.IsNullOrWhiteSpace(kosul))
                sql += " WHERE " + kosul;

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelSiteKesinti yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KaydetPersonelSiteKesintisi(PersonelSiteKesinti form)
        {
            string sql = "INSERT INTO " + TABLOSITEKESINTISI + "(" + VeriErisim.TumAlanlar("", ALANLARSITEKESINTISI) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.siteKesTopKod);
            sql += "," + VeriErisim.DegerYap(form.giderKod);
            sql += "," + VeriErisim.DegerYap(form.aktif);
            sql += "," + VeriErisim.DegerYap(form.endex);
            sql += "," + VeriErisim.DegerYap(form.tutar);
            sql += "," + VeriErisim.DegerYap(form.elden);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Güncellenecek kayıt bilgisini tutan, PersonelSiteKesinti yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncellePersonelSiteKesintisi(PersonelSiteKesinti form)
        {
            string sql = "";

            sql = "UPDATE " + TABLOSITEKESINTISI + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.SKTKOD], form.siteKesTopKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.GIDERKOD], form.giderKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.AKTIF], form.aktif) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.ENDEX], form.endex) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.TUTAR], form.tutar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.ELDEN], form.elden) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.ACIKLAMA], form.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelSiteKesinti yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuPersonelSiteKesintisi(PersonelSiteKesinti form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOSITEKESINTISI, ALANLARSITEKESINTISI) + "," + VeriErisim.TumAlanlar(TABLOSITEKESINTITOPLAM, ALANLARSITEKESINTITOPLAM);
            sql += " FROM " + TABLOSITEKESINTISI;
            sql += " inner join " + TABLOSITEKESINTITOPLAM + " ON " + TABLOSITEKESINTITOPLAM + "." + ALANLARSITEKESINTITOPLAM[(int)ENUMALANLARSITEKESINTITOPLAM.KOD] + "=" + TABLOSITEKESINTISI + "." + ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.SKTKOD];

            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOSITEKESINTISI + "." + ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.KOD], "=", form.kod));

            if (form.giderKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOSITEKESINTISI + "." + ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.GIDERKOD], "=", form.giderKod));

            if (form.siteKesTopKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOSITEKESINTISI + "." + ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.SKTKOD], "=", form.siteKesTopKod));
            else
            {
                if (form.pLojmanKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOSITEKESINTITOPLAM + "." + ALANLARSITEKESINTITOPLAM[(int)ENUMALANLARSITEKESINTITOPLAM.PLOJMANKOD], "=", form.pLojmanKod));

                if (form.yil > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOSITEKESINTITOPLAM + "." + ALANLARSITEKESINTITOPLAM[(int)ENUMALANLARSITEKESINTITOPLAM.YIL], "=", form.yil));

                if (form.ay > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOSITEKESINTITOPLAM + "." + ALANLARSITEKESINTITOPLAM[(int)ENUMALANLARSITEKESINTITOPLAM.AY], "=", form.ay));
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " order by " + ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.GIDERKOD]; //+ ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.YIL] + " DESC, " + ALANLARSITEKESINTISI[(int)ENUMALANLARSITEKESINTISI.AY] + " DESC, " + 

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, int kod nesnesi</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilPersonelSiteKesintiToplam(int kod)
        {
            string sql = "DELETE FROM " + TABLOSITEKESINTITOPLAM;
            string kosul = "";

            if (kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSITEKESINTITOPLAM[(int)ENUMALANLARSITEKESINTITOPLAM.KOD], "=", kod));

            if (!string.IsNullOrWhiteSpace(kosul))
                sql += " WHERE " + kosul;

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelSiteKesintiToplam yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KaydetPersonelSiteKesintiToplam(PersonelSiteKesintiToplam form)
        {
            string sql = "INSERT INTO " + TABLOSITEKESINTITOPLAM + "(" + VeriErisim.TumAlanlar("", ALANLARSITEKESINTITOPLAM) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.pLojmanKod);
            sql += "," + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.ay);
            sql += "," + VeriErisim.DegerYap(form.toplamTutar);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Güncellenecek kayıt bilgisini tutan, PersonelSiteKesintiToplam yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncellePersonelSiteKesintiToplam(PersonelSiteKesintiToplam form)
        {
            string sql = "";

            sql = "UPDATE " + TABLOSITEKESINTITOPLAM + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLARSITEKESINTITOPLAM[(int)ENUMALANLARSITEKESINTITOPLAM.PLOJMANKOD], form.pLojmanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEKESINTITOPLAM[(int)ENUMALANLARSITEKESINTITOPLAM.YIL], form.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEKESINTITOPLAM[(int)ENUMALANLARSITEKESINTITOPLAM.AY], form.ay) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEKESINTITOPLAM[(int)ENUMALANLARSITEKESINTITOPLAM.TOPLAMTUTAR], form.toplamTutar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSITEKESINTITOPLAM[(int)ENUMALANLARSITEKESINTITOPLAM.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Güncellenecek kayıt bilgisini tutan, PersonelSiteKesintiToplam yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncellePersonelSiteKesintiToplamTutar(PersonelSiteKesintiToplam form)
        {
            string sql = "";

            sql = "UPDATE " + TABLOSITEKESINTITOPLAM + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLARSITEKESINTITOPLAM[(int)ENUMALANLARSITEKESINTITOPLAM.TOPLAMTUTAR], form.toplamTutar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSITEKESINTITOPLAM[(int)ENUMALANLARSITEKESINTITOPLAM.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, int kod nesnesi</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilPersonelKesinti(PersonelSiteKesinti p)
        {
            string sql = "DELETE FROM " + TABLOKESINTI;
            string kosul = "";

            if (p.pLojmanKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKESINTI[(int)ENUMALANLARKESINTI.PLOJMANKOD], "=", p.pLojmanKod));

            if (p.personelKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKESINTI[(int)ENUMALANLARKESINTI.PKOD], "=", p.personelKod));

            if (p.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKESINTI[(int)ENUMALANLARKESINTI.YIL], "=", p.yil));

            if (p.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKESINTI[(int)ENUMALANLARKESINTI.AY], "=", p.ay));

            if (p.giderKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKESINTI[(int)ENUMALANLARKESINTI.GIDERKOD], "=", p.giderKod));


            if (!string.IsNullOrWhiteSpace(kosul))
                sql += " WHERE " + kosul;

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelSiteKesinti yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KaydetPersonelKesinti(PersonelSiteKesinti form)
        {
            string sql = "INSERT INTO " + TABLOKESINTI + "(" + VeriErisim.TumAlanlar("", ALANLARKESINTI) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.personelKod);
            sql += "," + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.ay);
            sql += "," + VeriErisim.DegerYap(form.giderKod);
            sql += "," + VeriErisim.DegerYap(form.pLojmanKod);
            sql += "," + VeriErisim.DegerYap(form.aktif);
            sql += "," + VeriErisim.DegerYap(form.endex);
            sql += "," + VeriErisim.DegerYap(form.tutar);
            sql += "," + VeriErisim.DegerYap(form.elden);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelSiteKesinti yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListelePersonelKesinti(PersonelSiteKesinti form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOKESINTI, ALANLARKESINTI);
            sql += " FROM " + TABLOKESINTI;

            if (form.personelKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOKESINTI + "." + ALANLARKESINTI[(int)ENUMALANLARKESINTI.PKOD], "=", form.personelKod));

            if (form.giderKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOKESINTI + "." + ALANLARKESINTI[(int)ENUMALANLARKESINTI.GIDERKOD], "=", form.giderKod));

            if (form.pLojmanKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOKESINTI + "." + ALANLARKESINTI[(int)ENUMALANLARKESINTI.PLOJMANKOD], "=", form.pLojmanKod));

            if (form.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOKESINTI + "." + ALANLARKESINTI[(int)ENUMALANLARKESINTI.YIL], "=", form.yil));

            if (form.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOKESINTI + "." + ALANLARKESINTI[(int)ENUMALANLARKESINTI.AY], "=", form.ay));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " order by " + ALANLARKESINTI[(int)ENUMALANLARKESINTI.GIDERKOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
