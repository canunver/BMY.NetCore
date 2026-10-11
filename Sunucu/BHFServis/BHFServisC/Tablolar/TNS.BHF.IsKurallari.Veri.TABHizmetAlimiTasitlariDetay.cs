using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Hizmet alımı ile edinilecek taşıt bilgilerini (şoförlü/şoförsüz, yakıtlı/yakıtsız maliyet bilgilerini)  tutan BHFHIZMETALIMITASITDETAY veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABHizmetAlimiTasitlariDetay : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABHizmetAlimiTasitlariDetay(VeriErisim veriErisim)
            : base(veriErisim)
        {
            if (veriErisim.baglantiTip.Equals("ORACLE"))
            {
                TABLOAD = "BHFHIZMETALIMITASITDETAY";
                ALANLAR = new string[] { "YIL", "KURUMSAL", "KOD", "SOFORLUYAKGIDDAHADET2", "SOFORLUYAKGIDDAHSURE2", "SOFORLUYAKGIDDAHMAL2", "SOFORLUYAKGIDHARADET2", "SOFORLUYAKGIDHARSURE2", "SOFORLUYAKGIDHARMAL2", "SOFORSUZYAKGIDDAHADET2", "SOFORSUZYAKGIDDAHSURE2", "SOFORSUZYAKGIDDAHMAL2", "SOFORSUZYAKGIDHARADET2", "SOFORSUZYAKGIDHARSURE2", "SOFORSUZYAKGIDHARMAL2", "SOFORLUYAKGIDDAHADET1", "SOFORLUYAKGIDDAHSURE1", "SOFORLUYAKGIDDAHMAL1", "SOFORLUYAKGIDHARADET1", "SOFORLUYAKGIDHARSURE1", "SOFORLUYAKGIDHARICMALIYET1", "SOFORSUZYAKGIDDAHADET1", "SOFORSUZYAKGIDDAHSURE1", "SOFORSUZYAKGIDDAHMAL1", "SOFORSUZYAKGIDHARADET1", "SOFORSUZYAKGIDHARSURE1", "SOFORSUZYAKGIDHARMAL1", "SOFORLUYAKGIDDAHADET0", "SOFORLUYAKGIDDAHSURE0", "SOFORLUYAKGIDDAHMAL0", "SOFORLUYAKGIDHARADET0", "SOFORLUYAKGIDHARSURE0", "SOFORLUYAKGIDHARMALIYET0", "SOFORSUZYAKGIDDAHADET0", "SOFORSUZYAKGIDDAHSURE0", "SOFORSUZYAKGIDDAHMAL0", "SOFORSUZYAKGIDHARADET0", "SOFORSUZYAKGIDHARSURE0", "SOFORSUZYAKGIDHARMAL0" };
            }
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıt bilgilerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOAD = "BHFHIZMETALIMITASITLARIDETAY";

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıt bilgilerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURUMSAL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KURUMSAL = 1,

            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KOD = 2,

            /// <summary>
            /// YILKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            YILKOD = 3,

            /// <summary>
            /// SOFORLUYAKITGIDERIDAHILADET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SOFORLUYAKITGIDERIDAHILADET = 4,

            /// <summary>
            /// SOFORLUYAKITGIDERIDAHILADET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SOFORLUYAKITGIDERIDAHILSURE = 5,

            /// <summary>
            /// SOFORLUYAKITGIDERIDAHILMALIYET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SOFORLUYAKITGIDERIDAHILMALIYET = 6,

            /// <summary>
            /// SOFORLUYAKITGIDERIHARICADET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SOFORLUYAKITGIDERIHARICADET = 7,

            /// <summary>
            /// SOFORLUYAKITGIDERIHARICADET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SOFORLUYAKITGIDERIHARICSURE = 8,

            /// <summary>
            /// SOFORLUYAKITGIDERIHARICMALIYET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SOFORLUYAKITGIDERIHARICMALIYET = 9,

            /// <summary>
            /// SOFORSUZYAKITGIDERIDAHILADET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SOFORSUZYAKITGIDERIDAHILADET = 10,

            /// <summary>
            /// SOFORSUZYAKITGIDERIDAHILADET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SOFORSUZYAKITGIDERIDAHILSURE = 11,

            /// <summary>
            /// SOFORSUZYAKITGIDERIDAHILMALIYET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SOFORSUZYAKITGIDERIDAHILMALIYET = 12,

            /// <summary>
            /// SOFORSUZYAKITGIDERIHARICADET2 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SOFORSUZYAKITGIDERIHARICADET = 13,

            /// <summary>
            /// SOFORSUZYAKITGIDERIHARICADET2 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SOFORSUZYAKITGIDERIHARICSURE = 14,

            /// <summary>
            /// SOFORSUZYAKITGIDERIHARICMALIYET2 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SOFORSUZYAKITGIDERIHARICMALIYET = 15,
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıt bilgilerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURUMSAL", "KOD", "YILKOD", 
                                      "SOFORLUYAKITGIDERIDAHILADET",  "SOFORLUYAKITGIDERIDAHILSURE", "SOFORLUYAKITGIDERIDAHILMALIYET", 
                                      "SOFORLUYAKITGIDERIHARICADET",  "SOFORLUYAKITGIDERIHARICSURE", "SOFORLUYAKITGIDERIHARICMALIYET", 
                                      "SOFORSUZYAKITGIDERIDAHILADET", "SOFORSUZYAKITGIDERIDAHILSURE","SOFORSUZYAKITGIDERIDAHILMALIYET", 
                                      "SOFORSUZYAKITGIDERIHARICADET", "SOFORSUZYAKITGIDERIHARICSURE","SOFORSUZYAKITGIDERIHARICMALIYET" };

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtların maliyetlerini döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Hizmet alımı ile edinilecek taşıtların maliyetlerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek taşıtların maliyetlerinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(int yil, string kurumsalKod, bool kullanilan)
        {
            string tut = kurumsalKod.Replace(".", "");
            string strYilKod;
            if (kullanilan) strYilKod = "0,1";
            else strYilKod = "2,3";
            string sql;
            if (tut.Length == 8)
                sql = "select " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            else
            {
                sql = "select YIL, " + VeriErisim.DegerYap(kurumsalKod) + " KURUMSAL" + ", KOD, YILKOD, ";
                sql += " SUM(SOFORLUYAKITGIDERIDAHILADET), SUM(SOFORLUYAKITGIDERIDAHILADET * SOFORLUYAKITGIDERIDAHILSURE), SUM(SOFORLUYAKITGIDERIDAHILADET * SOFORLUYAKITGIDERIDAHILSURE * SOFORLUYAKITGIDERIDAHILMALIYET), ";
                sql += " SUM(SOFORLUYAKITGIDERIHARICADET), SUM(SOFORLUYAKITGIDERIHARICADET * SOFORLUYAKITGIDERIHARICSURE), SUM(SOFORLUYAKITGIDERIHARICADET * SOFORLUYAKITGIDERIHARICSURE * SOFORLUYAKITGIDERIHARICMALIYET), ";
                sql += " SUM(SOFORSUZYAKITGIDERIDAHILADET), SUM(SOFORSUZYAKITGIDERIDAHILADET * SOFORSUZYAKITGIDERIDAHILSURE), SUM(SOFORSUZYAKITGIDERIDAHILADET * SOFORSUZYAKITGIDERIDAHILSURE * SOFORSUZYAKITGIDERIDAHILMALIYET), ";
                sql += " SUM(SOFORSUZYAKITGIDERIHARICADET), SUM(SOFORSUZYAKITGIDERIHARICADET * SOFORSUZYAKITGIDERIHARICSURE), SUM(SOFORSUZYAKITGIDERIHARICADET * SOFORSUZYAKITGIDERIHARICSURE * SOFORSUZYAKITGIDERIHARICMALIYET) ";
            }
            sql = sql + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMSAL], "LIKE", tut + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", ALANLAR[(int)ENUMAlanlar.YILKOD] + " IN (" + strYilKod + ")");
            if (tut.Length < 8)
                sql = sql + " group by YIL, KOD, YILKOD";
            sql = sql + " order by 1,2,3";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtların maliyetlerini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Hizmet alımı ile edinilecek taşıtların maliyetlerinin ekleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek taşıtların maliyetlerinin ekleneceği birimin kurumsal kodu.</param>
        /// <param name="hatDetay">Hizmet alımı ile edinilecek taşıtların bilgilerini tutan nesne.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(int yil, string kurumsalKod, int yilKod, int indis, HizmetAlimiTasitBilgisiDetay hatDetay)
        {
            string sql = "";
            sql = "insert into " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (" +
                VeriErisim.DegerYap(yil) + ", " +
                VeriErisim.DegerYap(kurumsalKod.Replace(".", "")) + ", " +
                VeriErisim.DegerYap(hatDetay.tasitSozluk.siraNo) + ", " +
                VeriErisim.DegerYap(yilKod) + ", " +
                VeriErisim.DegerYap(hatDetay.soforluYakitGiderDahil[indis].adet) + ", " +
                VeriErisim.DegerYap((int)hatDetay.soforluYakitGiderDahil[indis].sure) + ", " +
                VeriErisim.DegerYap(hatDetay.soforluYakitGiderDahil[indis].maliyet) + ", " +

                VeriErisim.DegerYap(hatDetay.soforluYakitGiderHaric[indis].adet) + ", " +
                VeriErisim.DegerYap((int)hatDetay.soforluYakitGiderHaric[indis].sure) + ", " +
                VeriErisim.DegerYap(hatDetay.soforluYakitGiderHaric[indis].maliyet) + ", " +

                VeriErisim.DegerYap(hatDetay.soforsuzYakitGiderDahil[indis].adet) + ", " +
                VeriErisim.DegerYap((int)hatDetay.soforsuzYakitGiderDahil[indis].sure) + ", " +
                VeriErisim.DegerYap(hatDetay.soforsuzYakitGiderDahil[indis].maliyet) + ", " +

                VeriErisim.DegerYap(hatDetay.soforsuzYakitGiderHaric[indis].adet) + ", " +
                VeriErisim.DegerYap((int)hatDetay.soforsuzYakitGiderHaric[indis].sure) + ", " +
                VeriErisim.DegerYap(hatDetay.soforsuzYakitGiderHaric[indis].maliyet) + ") ";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Hizmet alımı ile edinilecek taşıtların maliyetlerini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Hizmet alımı ile edinilecek taşıtların maliyetlerinin silineceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet alımı ile edinilecek taşıtların maliyetlerinin silineceği birimin kurumsal kodu.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string kurumsalKod, bool kullanilan)
        {
            string strYilKod;
            if (kullanilan) strYilKod = "0,1";
            else strYilKod = "2,3";

            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSAL], "=", kurumsalKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", ALANLAR[(int)ENUMAlanlar.YILKOD] + " IN (" + strYilKod + ")");

            return KomutCalistir(sql, 0);
        }
    }
}
