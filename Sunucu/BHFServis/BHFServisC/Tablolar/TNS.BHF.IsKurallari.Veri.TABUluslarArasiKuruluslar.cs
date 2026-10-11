using System;
using TNS;
using TNS.Veri;
using TNS.BHF.IsKurallari;
using System.Data;
using System.Collections;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Uluslararası kuruluş üyelik bilgilerini tutan BHFULUSLARARASIKURULUSLAR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABUluslarArasiKuruluslar : TABOrtak
    {
        /// <summary>
        /// Uluslararası kuruluş üyelik bilgilerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOAD = "BHFULUSLARARASIKURULUSLAR";

        /// <summary>
        /// Uluslararası kuruluş üyelik bilgilerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KURUM = 1,

            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SIRANO = 2,

            /// <summary>
            /// KURULUSAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KURULUSAD = 3,

            /// <summary>
            /// YASALDAYANAK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            YASALDAYANAK = 4,

            /// <summary>
            /// TOPLANTISAYISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            TOPLANTISAYISI = 5,

            /// <summary>
            /// TOPLANTIKATSAYISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            TOPLANTIKATSAYISI = 6,

            /// <summary>
            /// DOVIZCINSI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            DOVIZCINSI = 7,

            /// <summary>
            /// ONCEKIYILAIDAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            ONCEKIYILAIDAT = 8,

            /// <summary>
            /// HAZIRLIKYILAIDAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            HAZIRLIKYILAIDAT = 9,

            /// <summary>
            /// ERTESIYILAIDAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            ERTESIYILAIDAT = 10,

            /// <summary>
            /// ERTESIYIL2AIDAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            ERTESIYIL2AIDAT = 11,

            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            ACIKLAMA = 12,

            /// <summary>
            /// ONCEKIYILDOVIZ alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            ONCEKIYILDOVIZ = 13,

            /// <summary>
            /// HAZIRLIKYILDOVIZ alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            HAZIRLIKYILDOVIZ = 14,

            /// <summary>
            /// ERTESIYILDOVIZ alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            ERTESIYILDOVIZ = 15,

            /// <summary>
            /// ERTESIYIL2DOVIZ alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            ERTESIYIL2DOVIZ = 16
        }

        /// <summary>
        /// Uluslararası kuruluş üyelik bilgilerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURUM", "SIRANO", "KURULUSAD", "YASALDAYANAK", "TOPLANTISAYISI", "TOPLANTIKATSAYISI", "DOVIZCINSI", "ONCEKIYILAIDAT", "HAZIRLIKYILAIDAT", "ERTESIYILAIDAT", "ERTESIYIL2AIDAT", "ACIKLAMA", "ONCEKIYILDOVIZ", "HAZIRLIKYILDOVIZ", "ERTESIYILDOVIZ", "ERTESIYIL2DOVIZ" };

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABUluslarArasiKuruluslar(VeriErisim veriErisim) : base(veriErisim) { }


        /// <summary>
        /// Uluslararası kuruluş üyelik bilgilerini verilen birim için silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Silinecek uluslararası kuruluş üyelik bilgilerinin yıl bilgisi.</param>
        /// <param name="kurumsalKod">Silinecek uluslararası kuruluş üyelik bilgilerinin kurumsal kodu.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc UluslarArasiKuruluslarSil(int yil, string kurumsalKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUM], "=", kurumsalKod));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Uluslararası kuruluş üyelik bilgisini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Eklenecek uluslararası kuruluş üyelik bilgisinin yıl bilgisi.</param>
        /// <param name="kurumsalKod">Eklenecek uluslararası kuruluş üyelik bilgisinin kurumsal kodu.</param>
        /// <param name="siraNo">Eklenecek uluslararası kuruluş üyelik bilgisinin sıra numarası.</param>
        /// <param name="uak">Eklenecek uluslararası kuruluş üyelik bilgisinin içeriğini tutan nesne.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc UluslarArasiKuruluslarKaydet(int yil, string kurumsalKod, int siraNo, UluslarArasiKurulus uak)
        {
            string sql = "";
            sql = "insert into " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (" +
                VeriErisim.DegerYap(yil) + ", " +
                VeriErisim.DegerYap(kurumsalKod) + ", " +
                VeriErisim.DegerYap(siraNo) + ", " +
                VeriErisim.DegerYap(uak.kurulusAd) + ", " +
                VeriErisim.DegerYap(uak.yasalDayanak) + ", " +
                VeriErisim.DegerYap(uak.toplantiSayisi) + ", " +
                VeriErisim.DegerYap(uak.toplantiKatsayisi) + ", " +
                VeriErisim.DegerYap(uak.dovizCinsi) + ", " +
                VeriErisim.DegerYap(uak.oncekiyilAidat) + ", " +
                VeriErisim.DegerYap(uak.hazirlikYilAidat) + ", " +
                VeriErisim.DegerYap(uak.ertesiYilAidat) + ", " +
                VeriErisim.DegerYap(uak.ertesiYil2Aidat) + ", " +
                VeriErisim.DegerYap(uak.aciklama) + "," +
                VeriErisim.DegerYap(uak.oncekiyilDoviz) + ", " +
                VeriErisim.DegerYap(uak.hazirlikYilDoviz) + ", " +
                VeriErisim.DegerYap(uak.ertesiYilDoviz) + ", " +
                VeriErisim.DegerYap(uak.ertesiYil2Doviz) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Tavan üstüne sığmayan ödenekleri döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Listelenecek uluslararası kuruluş üyelik bilgilerinin yıl bilgisi.</param>
        /// <param name="kurumsalKod">Listelenecek uluslararası kuruluş üyelik bilgilerinin kurumsal kodu.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader UluslarArasiKuruluslarListele(int yil, string kurumsalKod)
        {
            string sql = "";
            string kosul = "";
            sql = "select " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql = sql + " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (kurumsalKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUM], "LIKE", kurumsalKod + "%"));
            sql = sql + " WHERE " + kosul;
            sql = sql + " order by 1,2,3";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
