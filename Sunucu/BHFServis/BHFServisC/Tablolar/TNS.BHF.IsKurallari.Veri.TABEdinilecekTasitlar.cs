using System;
using TNS;
using TNS.Veri;
using TNS.BHF.IsKurallari;
using System.Data;
using System.Collections;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Edinilecek Taşıtlar bilgilerini tutan BHFEDINILECEKTASITLAR ile taşıt sözlük bilgilerini tutan BHFTASITLARSOZLUK veri tabanı tabloları ile ilgili silme, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABEdinilecekTasitlar : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABEdinilecekTasitlar(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken.
        /// </summary>
        static string TABLOAD = "BHFEDINILECEKTASITLAR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi).
            /// </summary>
            YIL = 0,

            /// <summary>
            /// Kurumsal alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi).
            /// </summary>
            KURUMSAL = 1,

            /// <summary>
            /// Sıra No alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi).
            /// </summary>
            SIRANO = 2,

            /// <summary>
            /// Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi).
            /// </summary>
            KOD = 3,

            /// <summary>
            /// Adet alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi).
            /// </summary>
            ADET = 4,

            /// <summary>
            /// Yer alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi).
            /// </summary>
            YER = 5,

            /// <summary>
            /// Kaynak alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi).
            /// </summary>
            KAYNAK = 6
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURUMSAL", "SIRANO", "KOD", "ADET", "YER", "KAYNAK" };

        /// <summary>
        /// Tablo alanlarının tiplerini tutan sabit dizi.
        /// </summary>
        static int[] ALANTIP = { (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.STRING, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.STRING, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.STRING, (int)ENUMAlanTip.STRING };

        /// <summary>
        /// Taşıtlar Sözlük tablo adını tutan değişken.
        /// </summary>
        public static string TABLOADSOZLUK = "BHFTASITLARSOZLUK";

        /// <summary>
        /// Sözlük Tablo alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarSozluk : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi).
            /// </summary>
            YIL = 0,

            /// <summary>
            /// Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi).
            /// </summary>
            KOD = 1,

            /// <summary>
            /// Açıklama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi).
            /// </summary>
            ACIKLAMA = 2,

            /// <summary>
            /// Difransiyel alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi).
            /// </summary>
            DIFERANSIYEL = 3
        }

        /// <summary>
        /// Sözlük Tablo alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLARSOZLUK = { "YIL", "KOD", "ACIKLAMA", "DIFERANSIYEL" };

        /// <summary>
        /// Yıl ve Kurum anahtar alanlarını kullanarak veri tabanından ilgili kayıtları listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Yıl değerini tutan Int tipinde nesne.</param>
        /// <param name="kurum">Kurum değerini tutan String tipinde nesne.</param>
        /// <returns>Select komutunun çalışması sonucunda IDataReader tipinde nesne döndürür.</returns>
        public IDataReader EdinilecekTasitlarListele(int yil, string kurum)
        {
            string sql = "";
            sql = "select " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql = sql + "," + ALANLARSOZLUK[(int)ENUMAlanlarSozluk.ACIKLAMA] + ", " + ALANLARSOZLUK[(int)ENUMAlanlarSozluk.DIFERANSIYEL];
            sql = sql + " FROM " + TABLOAD + " INNER JOIN " + TABLOADSOZLUK;
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADSOZLUK + "." + ALANLARSOZLUK[(int)ENUMAlanlarSozluk.YIL]);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + TABLOADSOZLUK + "." + ALANLARSOZLUK[(int)ENUMAlanlarSozluk.KOD]);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMSAL], "LIKE", kurum + "%"));
            sql = sql + " order by 1,2";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yıl, Kurum, Sıra No, Kod, Adet, Yer ve Kaynak değerlerini kullanarak ilgili veri tabanına bilgileri kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Yıl değerini tutan Int tipinde nesne.</param>
        /// <param name="kurum">Kurum değerini tutan String tipinde nesne.</param>
        /// <param name="siraNo">Sıra No değerini tutan Int tipinde nesne.</param>
        /// <param name="kod">Kod değerini tutan String tipinde nesne.</param>
        /// <param name="adet">Adet değerini tutan Int tipinde nesne.</param>
        /// <param name="yer">Yer değerini tutan String tipinde nesne.</param>
        /// <param name="kaynak">Kaynak değerini tutan String tipinde nesne.</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda Sonuc tipinde nesne döndürür.</returns>
        public Sonuc EdinilecekTasitlarKaydet(int yil, string kurum, int siraNo, string kod, int adet, string yer, string kaynak)
        {
            string sql = "";
            sql = "insert into " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (" +
                VeriErisim.DegerYap(yil) + ", " +
                VeriErisim.DegerYap(kurum) + ", " +
                VeriErisim.DegerYap(siraNo) + ", " +
                VeriErisim.DegerYap(kod) + ", " +
                VeriErisim.DegerYap(adet) + ", " +
                VeriErisim.DegerYap(yer) + ", " +
                VeriErisim.DegerYap(kaynak) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Yıl, Kurum alanlarını kullanarak veri tabanında ilgili kayıtları silen 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Yıl değerini tutan Int tipinde nesne.</param>
        /// <param name="kurum">Kurum değerini tutan String tipinde nesne.</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda Sonuc tipinde nesne döndürür.</returns>
        public Sonuc EdinilecekTasitlarSil(int yil, string kurum)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSAL], "=", kurum));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Koşul anahtarına göre Taşıt Sözlük tablosundan ilgili kayıtları okuyan
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kosul">Koşul değerini tutan String tipinde nesne.</param>
        /// <returns>Select komutunun çalıştırılması sonucunda IDataReader tipinde nesne döndürür.</returns>
        private IDataReader TasitSozlukOku(string kosul)
        {
            string sql = "";
            sql = "select * FROM " + TABLOADSOZLUK;
            if (kosul != "")
                sql = sql + " WHERE " + kosul;
            sql = sql + " ORDER BY 1,2 ";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yıl anahtarına göre Taşıt Sözlük tablosundan ilgili kayıtları listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Yıl değerini tutan Int tipinde nesne.</param>
        /// <returns>Select komutunun çalıştırılması sonucunda IDataReader tipinde nesne döndürür.</returns>
        public IDataReader TasitSozlukListele(int yil)
        {
            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLUK[(int)ENUMAlanlarSozluk.YIL], "=", yil));
            return TasitSozlukOku(kosul);
        }

        public int TasitSozlukYilBul()
        {
            string sql = "SELECT MAX(YIL) FROM BHFTASITLARSOZLUK";
            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Yıl, Sıra No, Açıklama ve Difransiyel anahtar alanlarını kullanarak bilgileri ilgili veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Yıl değerini tutan Int tipinde nesne.</param>
        /// <param name="siraNo">Sıra No değerini tutan String tipinde nesne.</param>
        /// <param name="aciklama">Açıklama değerini tutan String tipinde nesne.</param>
        /// <param name="diferansiyel">Difransiyel değerini tutan String tipinde nesne.</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda Sonuc tipinde nesne döndürür.</returns>
        public Sonuc TasitSozlukKaydet(int yil, string siraNo, string aciklama, string diferansiyel)
        {
            string sql = "INSERT INTO " + TABLOADSOZLUK + "(" + VeriErisim.TumAlanlar("", ALANLARSOZLUK) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(siraNo) + ", ";
            sql += VeriErisim.DegerYap(aciklama) + ", ";
            sql += VeriErisim.DegerYap(diferansiyel) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Yıl anahtar alanı kullanarak ilgili veri tabanından kayıtları silen 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Yıl değerini tutan Int tipinde nesne.</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda Sonuc tipinde nesne döndürür.</returns>
        public Sonuc TasitSozlukSil(int yil)
        {
            string sql = "DELETE FROM " + TABLOADSOZLUK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLUK[(int)ENUMAlanlarSozluk.YIL], "=", yil));
            return KomutCalistir(sql, 0);
        }
    }
}
