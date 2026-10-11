using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Kurumsal yapı bilgilerinin tutulduğu BHMKURUMSALYAPI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemlerini tutan sınıf.
    /// </summary>
    public class TABBHMKURUMSALYAPI : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMKURUMSALYAPI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Kurumsal yapı bilgilerinin tutulduğu tablonun adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMKURUMSALYAPI";

        /// <summary>
        /// Kurumsal yapı bilgilerinin tutulduğu tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,

            /// <summary>
            /// SAYMANLIKKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAYMANLIKKOD = 2,

            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 4
        }

        /// <summary>
        /// Kurumsal yapı bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "AD", "SAYMANLIKKOD", "BIRIMKOD", "KURKOD" };

        /// <summary>
        /// Verilen kurumsal yapı kod parametresine ait bilgileri döndüren sql cümlesini oluşturup, çalıştıran ve DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="kurumsalKod">Bilgileri döndürülecek kurumsal yapı kodu parametresi</param>
        /// <returns>Kurumsal yapı bilgilerini bulan sql komutunun çalıştırılmasının sonucunu, DataReader yapısında döndürür</returns>
        public IDataReader KurumsalKodOku(string kurumsalKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kurumsalKod.Replace(".", "")));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen kriterlere uyan, kurumsal kodları listeyen sql cümlesini oluşturup, çalıştıran ve DataReader olarak döndüren yordam.
        /// Bu çağrıda önce sadeceBirinciSeviye kıstası, sonra en alt seviye kıstası, en sonunda da bir alt seviy ekıstası kontrol edilir.
        /// </summary>
        /// <param name="kurumsalYapi">Listelenecek kurumsal yapıların seçiminde kıstas olarak kullanılacak alanları içeren nesne</param>
        /// <param name="kodYapisi">Bir alt seviye, en alt seviye veya birici seviye gibi kıstaslar söylenmişse, kurumsal yapının kırılım yapısı istenilen sonucun döndürülmesi için kullanılmaktadır.</param>
        /// <param name="sadeceBirinciSeviye">Kurumsal yapı sıradüzeni içinde, birinci seviyedeki kodlar listelenir.</param>
        /// <param name="enAltSeviye">Sadece en alt seviyedeki kurumsal kod bilgileri listelenir.</param>
        /// <param name="birAltSeviye">Doğru ise kurumsal yapı nesnesi içerisinde verilen kod'un bir alt seviyesindeki kodlar listelenir</param>
        /// <returns>Kurumsal yapı listesini bulan sql komutunun çalıştırılmasının sonucunu, DataReader yapısında döndürür.</returns>
        public IDataReader Listele(KurumsalYapi kurumsalYapi, string kodYapisi, bool sadeceBirinciSeviye, bool enAltSeviye, bool birAltSeviye)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(kurumsalYapi.kod))
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", kurumsalYapi.kod.Replace(".", "") + "%");

            if (sadeceBirinciSeviye)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", " (LEN(KOD) = " + kodYapisi.IndexOf('.') + ")");
            }
            else if (enAltSeviye)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", " (LEN(KOD) = " + kodYapisi.Replace(".", "").Length + ")");
            }
            else if (birAltSeviye && kurumsalYapi.kod.Length < kodYapisi.Length)
            {
                int uzunluk = 0;
                if (!string.IsNullOrEmpty(kurumsalYapi.kod))
                {
                    kurumsalYapi.kod = OrtakFonksiyonlar.KodNoktaKoy(kodYapisi, kurumsalYapi.kod.Replace(".", ""));
                    uzunluk = kurumsalYapi.kod.Length + 1;
                }

                string alt = kodYapisi.Substring(uzunluk);
                if (alt.IndexOf(".") != -1)
                    alt = alt.Substring(0, alt.IndexOf("."));

                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", VeriErisim.IslecBul("LEN") + "(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")", ">", kurumsalYapi.kod.Replace(".", "").Length));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", VeriErisim.IslecBul("LEN") + "(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")", "<=", kurumsalYapi.kod.Replace(".", "").Length + alt.Replace(".", "").Length));
            }
            else if (birAltSeviye)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "<>", kurumsalYapi.kod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kurumsalYapi.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + kurumsalYapi.ad + "%"));

            if (kurumsalYapi.saymanlikKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD], "=", kurumsalYapi.saymanlikKod));

            if (!string.IsNullOrEmpty(kurumsalYapi.birimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "LIKE", kurumsalYapi.birimKod + "%"));

            if (!string.IsNullOrEmpty(kurumsalYapi.kurKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kurumsalYapi.kurKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen saymanlık, birim ve kurumsal kod kriterlerine uygun olan en alt kırılımdaki harcama birimlerini döndüren sql cümlesini oluşturup, çalıştıran ve DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="saymanlik">Harcama birimi verilen saymanlık olan harcama birimleri listelenir.</param>
        /// <param name="birim">Harcama birimi verilen birim kodu olan harcama birimleri listelenir.</param>
        /// <param name="kurKod">Harcama birimi verilen kurumsal kod harcama birimleri listelenir.</param>
        /// <returns>Kurumsal yapı listesini bulan sql komutunun çalıştırılmasının sonucunu, DataReader yapısında döndürür.</returns>
        public IDataReader Listele(string saymanlik, string birim, string kurKod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KOD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD], "=", saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birim));
            if (!string.IsNullOrEmpty(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kurKod.Replace(".", "") + "%"));
            return veriErisim.DataReaderDondur(sql);
        }


        //-----------------------------------------------------------
        // eski kurumsal yapı fonksiyonları

        public bool DetayVarMi(string kod)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", kod.Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "<>", kod.Replace(".", "")));
            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        public bool VarMi(string kod)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KOD] };
            string[] strDeger = { kod.Replace(".", "") };

            return Sayac.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, new string[0], new int[0]);
        }

        public Sonuc Kaydet(KurumsalYapi ky)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(ky.kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(ky.ad) + ",";
            sql += VeriErisim.DegerYap(ky.saymanlikKod) + ",";
            sql += VeriErisim.DegerYap(ky.birimKod) + ",";
            sql += VeriErisim.DegerYap(ky.kurKod.Replace(".", "")) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(KurumsalYapi ky)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], ky.ad);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD], ky.saymanlikKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], ky.birimKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], ky.kurKod.Replace(".", ""));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", ky.kod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(KurumsalYapi ky)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", ky.kod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        public static string ListeOlSql(int seviye, string kyKod)
        {
            string alanAd;
            alanAd = ALANLAR[(int)ENUMAlanlar.KOD];

            string sql = "SELECT " + alanAd + ", AD FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", kyKod.Replace(".", "") + "%"));
            if (seviye > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", VeriErisim.IslecBul("LEN") + "(KOD)", "=", seviye));

            return sql;
        }

        public int KurumsalKodSay(string kod, string kurKod)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", kod.Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurKod.Replace(".", "")));
            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}