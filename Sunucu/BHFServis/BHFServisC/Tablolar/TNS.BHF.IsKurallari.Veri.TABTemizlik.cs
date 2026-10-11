using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Temizlik ihalesi kimlik bilgilerini tutan BHFTEMIZLIKACIKLAMA, İhale maliyet bilgilerini tutan BHFTEMIZLIK ile ihalede kullanılacak tertiplerin listesini tutan BHFTEMIZLIKTERTIP veri tabanı tabloları ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABTemizlik : TABOrtak
    {
        /// <summary>
        /// Temizlik ihalesi kimlik bilgilerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOADACIKLAMA = "BHFTEMIZLIKACIKLAMA";

        /// <summary>
        /// Temizlik ihalesi kimlik bilgilerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarAciklama : int
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
            /// IHALENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            IHALENO = 2,

            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            ACIKLAMA = 3,

            /// <summary>
            /// HIMETALANIONCEKIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            HIMETALANIONCEKIYIL = 4,

            /// <summary>
            /// HIZMETALANIBUYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            HIZMETALANIBUYIL = 5,

            /// <summary>
            /// HIZMETALANISONRAKIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            HIZMETALANISONRAKIYIL = 6,

            /// <summary>
            /// GOREVLISAYISIONCEKIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            GOREVLISAYISIONCEKIYIL = 7,

            /// <summary>
            /// GOREVLISAYISIUBUYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            GOREVLISAYISIUBUYIL = 8,

            /// <summary>
            /// GOREVLISAYISISONRAKIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            GOREVLISAYISISONRAKIYIL = 9,

            /// <summary>
            /// BASLATARIHIONCEKIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            BASLATARIHIONCEKIYIL = 10,

            /// <summary>
            /// BASLATARIHIBUYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            BASLATARIHIBUYIL = 11,

            /// <summary>
            /// BASLATARIHISONRAKIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            BASLATARIHISONRAKIYIL = 12,

            /// <summary>
            /// BITISTARIHIONCEKIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            BITISTARIHIONCEKIYIL = 13,

            /// <summary>
            /// BITISTARIHIBUYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            BITISTARIHIBUYIL = 14,

            /// <summary>
            /// BITISTARIHISONRAKIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            BITISTARIHISONRAKIYIL = 15,
        }

        /// <summary>
        /// Temizlik ihalesi kimlik bilgilerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLARACIKLAMA = { "YIL", "KURUM", "IHALENO", "ACIKLAMA", "HIMETALANIONCEKIYIL ", "HIZMETALANIBUYIL", "HIZMETALANISONRAKIYIL", "GOREVLISAYISIONCEKIYIL", "GOREVLISAYISIUBUYIL", "GOREVLISAYISISONRAKIYIL", "BASLATARIHIONCEKIYIL", "BASLATARIHIBUYIL", "BASLATARIHISONRAKIYIL", " BITISTARIHIONCEKIYIL", "BITISTARIHIBUYIL", "BITISTARIHISONRAKIYIL" };

        /// <summary>
        /// Temizlik ihalelelerinin maliyet bilgilerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOADMALIYET = "BHFTEMIZLIK";

        /// <summary>
        /// Temizlik ihalelelerinin maliyet bilgilerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarMaliyet : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            YIL = 0,

            /// <summary>
            /// IHALENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            IHALENO = 1,

            /// <summary>
            /// KURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KURUM = 2,

            /// <summary>
            /// IHALEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            IHALEKOD = 3,

            /// <summary>
            /// HARCAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            HARCAMA = 4,

            /// <summary>
            /// TAHMIN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            TAHMIN = 5,

            /// <summary>
            /// TEKLIF alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            TEKLIF = 6,
        }

        /// <summary>
        /// Temizlik ihalelelerinin maliyet bilgilerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLARMALIYET = { "YIL", "IHALENO", "KURUM", "IHALEKOD", "HARCAMA", "TAHMIN", "TEKLIF" };

        /// <summary>
        /// Temizlik ihalelerinde kullanılacak tertiplerin listesini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOADTERTIP = "BHFTEMIZLIKTERTIP";

        /// <summary>
        /// Temizlik ihalelerinde kullanılacak tertiplerin listesini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarTertip : int
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
            /// IHALENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            IHALENO = 2,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KURKOD = 3,

            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            FONKOD = 4,

            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            FINKOD = 5,

            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            EKOKOD = 6,
        }

        /// <summary>
        /// Temizlik ihalelerinde kullanılacak tertiplerin listesini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLARTERTIP = { "YIL", "KURUM", "IHALENO", "KURKOD", "FONKOD", "FINKOD", "EKOKOD" };

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABTemizlik(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Verilen birimin, ilgili yıldaki verilen numaralı ihale kimlik bilgilerini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Silinecek ihale kimlik bilgisinin yılı.</param>
        /// <param name="kurumsalKod">Silinecek ihale kimlik bilgisinin biriminin kurumsal kodu.</param>
        /// <param name="ihaleno">Silinecek ihale kimlik bilgisinin numarası.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc IhaleSil(int yil, string kurumsalKod, int ihaleno)
        {
            string kosul = "";
            kosul = VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.KURUM], "=", kurumsalKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.IHALENO], "=", ihaleno));
            string sql = "DELETE FROM " + TABLOADACIKLAMA + " WHERE " + kosul;
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Verilen birimin, ilgili yıldaki verilen numaralı ihale maliyet bilgilerini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Silinecek ihale maliyet bilgilerinin yılı.</param>
        /// <param name="kurumsalKod">Silinecek ihale maliyet bilgilerinin biriminin kurumsal kodu.</param>
        /// <param name="ihaleno">Silinecek ihale maliyet bilgilerinin numarası.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc IhaleMaliyetleriSil(int yil, string kurumsalKod, int ihaleno)
        {
            string kosul, sql;
            Sonuc sonuc;
            kosul = VeriErisim.KosulOl("", ALANLARMALIYET[(int)ENUMAlanlarMaliyet.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARMALIYET[(int)ENUMAlanlarMaliyet.KURUM], "=", kurumsalKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARMALIYET[(int)ENUMAlanlarMaliyet.IHALENO], "=", ihaleno));
            sql = "DELETE FROM " + TABLOADMALIYET + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        /// <summary>
        /// Verilen birimin, ilgili yıldaki verilen numaralı ihale tertip listesini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Silinecek ihale tertip listesinin yılı.</param>
        /// <param name="kurumsalKod">Silinecek ihale tertip listesinin biriminin kurumsal kodu.</param>
        /// <param name="ihaleno">Silinecek ihale tertip listesinin numarası.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc IhaleTertipleriSil(int yil, string kurumsalKod, int ihaleno)
        {
            string kosul, sql;
            Sonuc sonuc;
            kosul = VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KURUM], "=", kurumsalKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.IHALENO], "=", ihaleno));
            sql = "DELETE FROM " + TABLOADTERTIP + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        /// <summary>
        /// Verilen ihale kimlik bilgilerini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Eklenecek ihale kimlik bilgisinin yılı.</param>
        /// <param name="kurumsalKod">Eklenecek ihale kimlik bilgisinin kurumsal kodu.</param>
        /// <param name="ihaleno">Eklenecek ihale kimlik bilgisinin birim içi sıra numarası.</param>
        /// <param name="ihale">Eklenecek ihale kimlik bilgisinin içeriğini tutan nesne.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc IhaleEkle(int yil, string kurumsalKod, int ihaleno, TemizlikBilgi ihale)
        {
            string sql;
            sql = "INSERT INTO " + TABLOADACIKLAMA + "(" + VeriErisim.TumAlanlar("", ALANLARACIKLAMA) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurumsalKod) + ", ";
            sql = sql + VeriErisim.DegerYap(ihaleno) + ", ";
            sql = sql + VeriErisim.DegerYap(ihale.aciklama) + ",";
            sql = sql + VeriErisim.DegerYap(ihale.hzmAlanOnceki) + ", ";
            sql = sql + VeriErisim.DegerYap(ihale.hzmAlanBuYil) + ", ";
            sql = sql + VeriErisim.DegerYap(ihale.hzmAlanSonraki) + ", ";
            sql = sql + VeriErisim.DegerYap(ihale.gorSayisiOnceki) + ",";
            sql = sql + VeriErisim.DegerYap(ihale.gorSayisiBuYil) + ", ";
            sql = sql + VeriErisim.DegerYap(ihale.gorSayisiSonraki) + ", ";
            sql = sql + VeriErisim.DegerYap(ihale.basOnceki) + ", ";
            sql = sql + VeriErisim.DegerYap(ihale.basBuYil) + ", ";
            sql = sql + VeriErisim.DegerYap(ihale.basSonraki) + ", ";
            sql = sql + VeriErisim.DegerYap(ihale.bitisOnceki) + ", ";
            sql = sql + VeriErisim.DegerYap(ihale.bitisBuYil) + ", ";
            sql = sql + VeriErisim.DegerYap(ihale.bitisSonraki) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen ihale kimlik bilgilerini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Eklenecek ihale maliyet bilgisinin yılı.</param>
        /// <param name="kurumsalKod">Eklenecek ihale maliyet bilgisinin kurumsal kodu.</param>
        /// <param name="ihaleno">Eklenecek ihale maliyet bilgisinin birim içi sıra numarası.</param>
        /// <param name="maliyetKod">Eklenecek ihale maliyet bilgisinin maliyet kodu.</param>
        /// <param name="harcama">Eklenecek ihale maliyet bilgisinin maliyet koduna ait geçen yıl harcaması.</param>
        /// <param name="tahmin">Eklenecek ihale maliyet bilgisinin maliyet koduna ait bu yıl ki yıl sonu harcama tahmini.</param>
        /// <param name="teklif">Eklenecek ihale maliyet bilgisinin maliyet koduna ait talep edilen miktar.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc IhaleMaliyetEkle(int yil, string kurumsalKod, int ihaleno, string maliyetKod, double harcama, double tahmin, double teklif)
        {
            string sql;
            sql = "INSERT INTO " + TABLOADMALIYET + "(" + VeriErisim.TumAlanlar("", ALANLARMALIYET) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(ihaleno) + ", ";
            sql = sql + VeriErisim.DegerYap(kurumsalKod) + ", ";
            sql = sql + VeriErisim.DegerYap(maliyetKod) + ", ";
            sql = sql + VeriErisim.DegerYap(harcama) + ", ";
            sql = sql + VeriErisim.DegerYap(tahmin) + ", ";
            sql = sql + VeriErisim.DegerYap(teklif) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen ihaleye ilgili tertip tanımını ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Eklenecek ihale tertip tanımının yılı.</param>
        /// <param name="kurumsalKod">Eklenecek ihale tertip tanımının kurumsal kodu.</param>
        /// <param name="ihaleno">Tertip tanımı eklenecek ihalenin içi sıra numarası.</param>
        /// <param name="kurkod">Eklenecek tertip tanımının kurumsal kodu.</param>
        /// <param name="fonkod">Eklenecek tertip tanımının fonksiyonel kodu.</param>
        /// <param name="finkod">Eklenecek tertip tanımının finansman kodu.</param>
        /// <param name="ekokod">Eklenecek tertip tanımının ekonomik kodu.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc TertipEkle(int yil, string kurumsalKod, int ihaleno, string kurkod, string fonkod, string finkod, string ekokod)
        {
            string sql;
            sql = "INSERT INTO " + TABLOADTERTIP + "(" + VeriErisim.TumAlanlar("", ALANLARTERTIP) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurumsalKod) + ", ";
            sql = sql + VeriErisim.DegerYap(ihaleno) + ", ";
            sql = sql + VeriErisim.DegerYap(kurkod) + ", ";
            sql = sql + VeriErisim.DegerYap(fonkod) + ", ";
            sql = sql + VeriErisim.DegerYap(finkod) + ", ";
            sql = sql + VeriErisim.DegerYap(ekokod) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Bir birime ait temizlik ihalelerinin kimlik bilgilerini listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Kimlik bilgileri listelenecek ihalelerin yılı.</param>
        /// <param name="kurumsalKod">Kimlik bilgileri listelenecek ihalelerin kurumsal kodu. Kurum kodu verilmiş ise kurumun ihaleleri listelenir.</param>
        /// <param name="ihaleNo">Kimlik bilgileri listelenecek ihalelerin ihale numarası. 0 verilmiş ise, verilen birimin tüm ihaleleri listelenir.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader TemizlikListele(int yil, string kurumsalKod, int ihaleNo)
        {
            string sql;
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARACIKLAMA) + " FROM " + TABLOADACIKLAMA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.KURUM], "LIKE", kurumsalKod.Replace(".", "") + "%"));
            if (ihaleNo != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.IHALENO], "=", ihaleNo));
            sql = sql + " ORDER BY 2,3";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Bir birime ait temizlik ihalelerinin maliyet bilgilerini listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Maliyet bilgileri listelenecek ihalelerin yılı.</param>
        /// <param name="kurumsalKod">Maliyet bilgileri listelenecek ihalelerin kurumsal kodu.</param>
        /// <param name="ihaleNo">Maliyet bilgileri listelenecek ihalelerin ihale numarası.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader IhaleMaliyetListele(int yil, string kurumsalKod, int ihaleNo)
        {
            string sql;
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMALIYET) + " FROM " + TABLOADMALIYET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMALIYET[(int)ENUMAlanlarMaliyet.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALIYET[(int)ENUMAlanlarMaliyet.KURUM], "=", kurumsalKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALIYET[(int)ENUMAlanlarMaliyet.IHALENO], "=", ihaleNo));
            sql = sql + " ORDER BY " + ALANLARMALIYET[(int)ENUMAlanlarMaliyet.IHALEKOD];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Bir birime ait temizlik ihalelerinin tertip bilgilerini listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Tertip bilgileri listelenecek ihalelerin yılı.</param>
        /// <param name="kurumsalKod">Tertip bilgileri listelenecek ihalelerin kurumsal kodu.</param>
        /// <param name="ihaleNo">Tertip bilgileri listelenecek ihalelerin ihale numarası.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader TertipListele(int yil, string kurumsalKod, int ihaleNo)
        {
            string sql;
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARTERTIP) + " FROM " + TABLOADTERTIP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KURUM], "=", kurumsalKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.IHALENO], "=", ihaleNo));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen birimin, verilen yıldaki en yüksek numaralı ihalesinin numarası döndüren yordam.
        /// </summary>
        /// <param name="yil">En yüksek numaralı ihalenin numarası döndürülecek yıl.</param>
        /// <param name="kurumsalKod">En yüksek numaralı ihalenin numarası döndürülecek birimin kurumsal kodu.</param>
        /// <returns>Verilen birimin, verilen yıldaki en yüksek numaralı ihalesinin numarası döndürülür.</returns>
        public double IhaleNoBul(int yil, string kurumsalKod)
        {
            string sql;
            sql = "SELECT MAX(IHALENO) FROM " + TABLOADACIKLAMA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.KURUM], "=", kurumsalKod));

            return veriErisim.TekKolonSQLDbl(sql);
        }
    }
}

/*
public enum ENUMAlanlarSozluk : int
{
    YIL = 0,
    IHALEKOD = 1,
    ACIKLAMA = 2,
}
static string TABLOADSOZLUK = "BHFOZELGUVENLIKSOZLUK";
static string[] ALANLARSOZLUK ={ "YIL", "IHALEKOD", "ACIKLAMA" };
*/

