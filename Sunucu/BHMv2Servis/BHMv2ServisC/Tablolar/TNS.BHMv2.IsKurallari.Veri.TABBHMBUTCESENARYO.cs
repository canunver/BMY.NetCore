using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Senaryo kod ve isimlerini tutan BHMBUTCESENARYO veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABBHMBUTCESENARYO : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMBUTCESENARYO(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Senaryo tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMBUTCESENARYO";

        /// <summary>
        /// Senaryo tablosunun alanlarının sırasını tutan enumeration
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
        }

        /// <summary>
        /// Senaryo tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "AD" };

        /// <summary>
        /// Bütçe senaryosu bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="butceSenaryo">Eklenecek senaryo tanım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ButceSenaryo butceSenaryo)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(butceSenaryo.kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(butceSenaryo.ad) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Bütçe senaryosunun adını ve ilişkili tasnifini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="butceSenaryo">Güncellenecek senaryo tanım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(ButceSenaryo butceSenaryo)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], butceSenaryo.ad) + ", ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", butceSenaryo.kod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Bütçe senaryolarını veri tabanından çeken sql cümleciğini verilen senaryo kıstasına göre üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="butceSenaryo">Listelenecek senaryolar için kıstas parametresi</param>
        /// <returns>Senaryoları bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(ButceSenaryo butceSenaryo, string dilEk = "")
        {
            string sql = "SELECT KOD, AD" + dilEk + " FROM " + TABLOAD;

            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], ">", "0699"));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Bütçe senaryosu tanımını veri tabanından çeken sql cümleciğini verilen senaryo koduna göre üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="senaryoKod">Bilgileri döndürülecek senaryo kodu parametresi</param>
        /// <returns>Senaryoyu bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(string senaryoKod, string dilEk = "")
        {
            string sql = "SELECT KOD, AD" + dilEk + " FROM " + TABLOAD;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", senaryoKod.Replace(".", "")); ;
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Senaryo kodları, aşama ve aşama içi sıra numarası olarak tutulmaktadır. Verilen aşama iki basamaklı dizgisi için
        /// en son senaryo kodunu bulup, bir arttırıp döndüren yordam.
        /// </summary>
        /// <param name="asama">En son senaryo kodunun bir fazlası döndürülecek aşama parametresi</param>
        /// <returns>Verilen aşama için en son senaryo kodunun bir fazlası döndürülür.</returns>
        public string AnahtarBul(string asama)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            if (string.IsNullOrEmpty(asama))
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], ">", asama + "10");
            else
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", asama + "%");
            string tut = veriErisim.TekKolonSQLStr(sql);
            if (!string.IsNullOrEmpty(tut) && tut.Length == 4)
            {
                int tutInt = OrtakFonksiyonlar.ConvertToInt(tut, 0);
                tutInt++;
                return tutInt.ToString("0000");
            }
            else if (string.IsNullOrEmpty(asama))
                return "1000";
            else
                return asama + "00";
        }

        /// <summary>
        /// Bütçe senaryosunu veri tabanından silen sql cümleciğini verilen senaryo kıstasına göre üretip, çalıştırıp, sonucunu döndüren yordam.
        /// </summary>
        /// <param name="butceSenaryo">Silinecek senaryonun kodunu tutan kıstas parametresi.</param>
        /// <returns>Senaryoyu silmek için oluşturulan select komutunun çalıştırılmasının sonucunu döndürür.</returns>
        public Sonuc Sil(ButceSenaryo butceSenaryo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", butceSenaryo.kod.Replace(".", ""));

            return KomutCalistir(sql, 1);
        }

    }
}


/*
        //public IDataReader KurumsalYapiDondur(string kod)
        //{
        //    string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod.Replace(".", "")));
        //    return veriErisim.DataReaderDondur(sql);
        //}

        //public Sonuc Sil(ButceSenaryo bs)
        //{
        //    string sql = "DELETE FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bs.kod.Replace(".", "")));

        //    return KomutCalistir(sql, 1);
        //}

        //public bool VarMi(string kod)
        //{
        //    string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KOD] };
        //    string[] strDeger = { kod.Replace(".", "") };

        //    return Sayac.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, new string[0], new int[0]);
        //}

        //public bool DetayVarMi(string kod)
        //{
        //    string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", kod.Replace(".", "") + "%"));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "<>", kod.Replace(".", "")));
        //    return veriErisim.TekKolonSQLInt(sql) > 0;
        //}
*/