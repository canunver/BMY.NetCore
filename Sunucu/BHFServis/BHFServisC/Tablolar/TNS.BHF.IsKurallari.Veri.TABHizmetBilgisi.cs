using System;
using TNS.Veri;
using System.Data;
using TNS;
using TNS.BKD;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Üç yıl içerisinde yeni başlıyacak/bitirilecek hizmetlerin ekonomik kod bazında maliyetleri ile ilgili kimlik bilgilerini tutan BHFHIZMETBILGISI ile maliyet bilgisini tutan BHFHIZMETBILGISIEKO veri tabanı tabloları ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABHizmetBilgisi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>

        public TABHizmetBilgisi(VeriErisim veriErisim) : base(veriErisim) { }


        /// <summary>
        /// Üç yıl içerisinde yeni başlıyacak/bitirilecek hizmetlerin tanım bilgilerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOAD = "BHFHIZMETBILGISI";

        /// <summary>
        /// Üç yıl içerisinde yeni başlıyacak/bitirilecek hizmetlerin tanım bilgilerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// TAMAMMIILKMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            TAMAMMIILKMI = 2,

            /// <summary>
            /// HIZMETYILI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            HIZMETYILI = 3,

            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SIRANO = 4,

            /// <summary>
            /// BASLANGICTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            BASLANGICTARIH = 5,

            /// <summary>
            /// BITISTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            BITISTARIH = 6,

            /// <summary>
            /// HIZMETADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            HIZMETADI = 7,

            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            ACIKLAMA = 8
        }

        /// <summary>
        /// Üç yıl içerisinde yeni başlıyacak/bitirilecek hizmetlerin tanım bilgilerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURKOD", "TAMAMMIILKMI", "HIZMETYILI", "SIRANO", "BASLANGICTARIH", "BITISTARIH", "HIZMETADI", "ACIKLAMA" };

        /// <summary>
        /// Üç yıl içerisinde yeni başlıyacak/bitirilecek hizmetlerin maliyetlerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOADEKO = "BHFHIZMETBILGISIEKO";

        /// <summary>
        /// Üç yıl içerisinde yeni başlıyacak/bitirilecek hizmetlerin maliyetlerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarEKO : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// TAMAMMIILKMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            TAMAMMIILKMI = 2,

            /// <summary>
            /// HIZMETYILI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            HIZMETYILI = 3,

            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            SIRANO = 4,

            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            EKOKOD = 5,

            /// <summary>
            /// MIKTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            MIKTAR = 6,
        }

        /// <summary>
        /// Üç yıl içerisinde yeni başlıyacak/bitirilecek hizmetlerin maliyetlerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLAREKO = { "YIL", "KURKOD", "TAMAMMIILKMI", "HIZMETYILI", "SIRANO", "EKOKOD", "MIKTAR" };

        /// <summary>
        /// Üç yıl içerisinde yeni başlıyacak/bitirilecek hizmetlerin ekonomik kod bazında maliyetlerini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam. 
        /// </summary>
        /// <param name="yil">Önce maliyet sonra tanım bilgileri silinecek hizmetin yılı.</param>
        /// <param name="kurumsalKod">Önce maliyet sonra tanım bilgileri silinecek hizmetin birim kodu.</param>
        /// <param name="tamamMiIlkMi">Önce maliyet sonra tanım bilgileri silinecek hizmetin başlıyacak mı, bitirilecek mi olduğu.</param>
        /// <param name="hizmetYili">Önce maliyet sonra tanım bilgileri silinecek hizmetin, başlayacağı ya da bitirileceği yıl.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string kurumsalKod, int tamamMiIlkMi, int hizmetYili)
        {
            string kosul, sql;
            Sonuc sonuc;
            kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurumsalKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAMAMMIILKMI], "=", tamamMiIlkMi));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIZMETYILI], "=", hizmetYili));
            sql = "DELETE FROM " + TABLOADEKO + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            if (sonuc.islemSonuc)
            {
                sql = "DELETE FROM " + TABLOAD + " WHERE " + kosul;
                sonuc = KomutCalistir(sql, 0);
            }
            return sonuc;
        }

        /// <summary>
        /// Üç yıl içerisinde yeni başlıyacak/bitirilecek hizmetlerin tanım bilgisini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Tanım bilgileri eklenecek hizmetin yılı.</param>
        /// <param name="kurumsalKod">Tanım bilgileri eklenecek hizmetin birim kodu.</param>
        /// <param name="tamamMiIlkMi">Tanım bilgileri eklenecek hizmetin başlıyacak mı, bitirilecek mi olduğu.</param>
        /// <param name="hizmetYili">Tanım bilgileri eklenecek hizmetin, başlayacağı ya da bitirileceği yıl.</param>
        /// <param name="siraNo">Eklenecek hizmetin anahtar (yil, kurumsalKod, tamamMiIlkMi ve hizmetYili) içindeki sıra numarası.</param>
        /// <param name="hizmetBilgisiSatiri">Eklenecek hizmetin tanım bilgileri.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc EkleTanim(int yil, string kurumsalKod, int tamamMiIlkMi, int hizmetYili, int siraNo, HizmetBilgisiSatiri hizmetBilgisiSatiri)
        {
            string sql;
            sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurumsalKod) + ", ";
            sql = sql + VeriErisim.DegerYap(tamamMiIlkMi) + ", ";

            sql = sql + VeriErisim.DegerYap(hizmetYili) + ", ";
            sql = sql + VeriErisim.DegerYap(siraNo) + ", ";
            sql = sql + VeriErisim.DegerYap(hizmetBilgisiSatiri.baslangicTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(hizmetBilgisiSatiri.bitisTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(hizmetBilgisiSatiri.hizmetAdi) + ", ";
            sql = sql + VeriErisim.DegerYap(hizmetBilgisiSatiri.aciklama) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Üç yıl içerisinde yeni başlıyacak/bitirilecek hizmetlerin tanım bilgisini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Tanım bilgileri eklenecek hizmetin yılı.</param>
        /// <param name="kurumsalKod">Tanım bilgileri eklenecek hizmetin birim kodu.</param>
        /// <param name="tamamMiIlkMi">Tanım bilgileri eklenecek hizmetin başlıyacak mı, bitirilecek mi olduğu.</param>
        /// <param name="hizmetYili">Tanım bilgileri eklenecek hizmetin, başlayacağı ya da bitirileceği yıl.</param>
        /// <param name="siraNo">Eklenecek hizmetin anahtar (yil, kurumsalKod, tamamMiIlkMi ve hizmetYili) içindeki sıra numarası.</param>
        /// <param name="ekoKod">Hizmet için verilen maliyetin ekonomik kodu.</param>
        /// <param name="miktar">Hizmet için verilen maliyetin miktarı.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc EkleMaliyet(int yil, string kurumsalKod, int tamamMiIlkMi, int hizmetYili, int siraNo, string ekoKod, double miktar)
        {
            string sql;
            sql = "INSERT INTO " + TABLOADEKO + "(" + VeriErisim.TumAlanlar("", ALANLAREKO) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurumsalKod) + ", ";
            sql = sql + VeriErisim.DegerYap(tamamMiIlkMi) + ", ";

            sql = sql + VeriErisim.DegerYap(hizmetYili) + ", ";
            sql = sql + VeriErisim.DegerYap(siraNo) + ", ";
            sql = sql + VeriErisim.DegerYap(ekoKod) + ", ";

            sql = sql + VeriErisim.DegerYap(miktar) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen kriterlere uyan tamamlanacak/bitirilecek hizmetlerin kimlik bilgilerini döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Tanım bilgileri döndürülecek hizmetin yılı.</param>
        /// <param name="kurumsalKod">Tanım bilgileri döndürülecek hizmetin birim kodu.</param>
        /// <param name="tamamMiIlkMi">Tanım bilgileri döndürülecek hizmetin başlıyacak mı, bitirilecek mi olduğu.</param>
        /// <param name="hizmetYili">Tanım bilgileri döndürülecek hizmetin, başlayacağı ya da bitirileceği yıl.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader SatirlariAl(int yil, string kurumsalKod, int tamamMiIlkMi, int hizmetYili)
        {
            string sql;
            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KURKOD] + ", " + ALANLAR[(int)ENUMAlanlar.SIRANO] + ", " + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH] + ", " + ALANLAR[(int)ENUMAlanlar.BITISTARIH] + ", " + ALANLAR[(int)ENUMAlanlar.HIZMETADI] + ", " + ALANLAR[(int)ENUMAlanlar.ACIKLAMA];
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (kurumsalKod.Length == 2)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], ">", kurumsalKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "<", kurumsalKod + BKD.Arac.BagimsizKurumBaslangic()));
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kurumsalKod + "%"));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAMAMMIILKMI], "=", tamamMiIlkMi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIZMETYILI], " = ", hizmetYili));
            sql = sql + " ORDER BY 1, 2";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen kriterlere uyan tamamlanacak/bitirilecek hizmetlerin maliyet bilgilerini döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Maliyet bilgileri döndürülecek hizmetin yılı.</param>
        /// <param name="kurumsalKod">Maliyet bilgileri döndürülecek hizmetin birim kodu.</param>
        /// <param name="tamamMiIlkMi">Maliyet bilgileri döndürülecek hizmetin başlıyacak mı, bitirilecek mi olduğu.</param>
        /// <param name="hizmetYili">Maliyet bilgileri döndürülecek hizmetin, başlayacağı ya da bitirileceği yıl.</param>
        /// <param name="siraNo">Maliyet bilgileri döndürülecek hizmetin anahtar (yil, kurumsalKod, tamamMiIlkMi ve hizmetYili) içindeki sıra numarası.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader EkolariAl(int yil, string kurumsalKod, int tamamMiIlkMi, int hizmetYili, int siraNo)
        {
            string sql;
            sql = "SELECT " + ALANLAREKO[(int)ENUMAlanlarEKO.EKOKOD] + ", " + ALANLAREKO[(int)ENUMAlanlarEKO.MIKTAR] + " FROM " + TABLOADEKO;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurumsalKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAMAMMIILKMI], "=", tamamMiIlkMi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIZMETYILI], " = ", hizmetYili));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], " = ", siraNo));
            sql = sql + " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
