using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Sektör bilgilerini tutan YATSEKTOR veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATSektor : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATSektor(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATSEKTOR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// SEKTORNO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SEKTORNO = 0,

            /// <summary>
            /// AD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,

            /// <summary>
            /// ALTSEKTORVARMI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ALTSEKTORVARMI = 2,

            YIL = 3
        }

        /// <summary>
        /// Sektörün tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "SEKTORNO", "AD", "ALTSEKTORVARMI", "YIL" };

        ///// <summary>
        ///// Sektör bilgilerini veri tabanına ekleyen
        ///// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        ///// </summary>
        ///// <param name="sektor">Sektör bilgisini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public Sonuc Kaydet(int yil, SektorSozluk sektor)
        //{
        //    string sql = "";
        //    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
        //    sql += VeriErisim.DegerYap(sektor.kod.Replace(".", "")) + ",";
        //    sql += VeriErisim.DegerYap(sektor.ad) + ",";
        //    sql += VeriErisim.DegerYap(sektor.altSektorVarMi) + ",";
        //    sql += VeriErisim.DegerYap(yil) + ")";
        //    return KomutCalistir(sql, 1);
        //}

        ///// <summary>
        ///// Veri tabanında kayıtlı olan sektör ALTSEKTORVARMI alanını güncelleyen
        ///// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        ///// </summary>
        ///// <param name="sektor">Sektör bilgisini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public Sonuc GuncelleAltSektorVarMi(SektorSozluk sektor)
        //{
        //    string sql = "";
        //    sql = "UPDATE " + TABLOAD + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTSEKTORVARMI], sektor.altSektorVarMi);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SEKTORNO], "=", sektor.kod.Replace(".", "")));

        //    return KomutCalistir(sql, 1);
        //}

        ///// <summary>
        ///// Veri tabanında kayıtlı olan sektör AD alanını güncelleyen
        ///// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        ///// </summary>
        ///// <param name="sektor">Sektör bilgisini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public Sonuc GuncelleAd(SektorSozluk sektor)
        //{
        //    string sql = "";
        //    sql = "UPDATE " + TABLOAD + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], sektor.ad);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SEKTORNO], "=", sektor.kod.Replace(".", "")));

        //    return KomutCalistir(sql, 1);
        //}

        /// <summary>
        /// Parametre olarak verilen sektör nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sektörleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="sektor">Sektör bilgisini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan sektörleri tutan DataReader nesnesi döndürülür.</returns>
        //public IDataReader Listele(int yil, SektorSozluk sektor)
        //{
        //    string sql = "";
        //    string kosul = "";

        //    sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

        //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

        //    SektorSozluk sekSoz = new SektorSozluk();
        //    if (sektor.kod != "" && sektor.kod != sekSoz.kod)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SEKTORNO], " LIKE ", sektor.kod.Replace(".", "") + "%"));

        //    if (sektor.ad != "")
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + sektor.ad + "%"));

        //    if (sektor.altSektorVarMi != "")
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTSEKTORVARMI], "=", sektor.altSektorVarMi));

        //    if (kosul != "")
        //        sql += " WHERE " + kosul;

        //    sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SEKTORNO];

        //    return veriErisim.DataReaderDondur(sql);
        //}

        ///// <summary>
        ///// Verilen sektör nesnesini kullanarak, ilgili sektörü veri tabanından silen
        ///// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        ///// </summary>
        ///// <param name="sektor">Sektör bilgisini tutan nesne</param>
        ///// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        //public Sonuc Sil(SektorSozluk sektor)
        //{
        //    string sql = "";
        //    sql = "DELETE FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SEKTORNO], "=", sektor.kod.Replace(".", "")));

        //    return KomutCalistir(sql, 1);
        //}

        /// <summary>
        /// Verilen sektör kodunu kullanarak sektörün
        /// veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="sektorKodu">Sektör kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool SektorVarMi(int yil, string sektorKodu)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = { yil };

            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.SEKTORNO] };
            string[] strDeger = { sektorKodu.Replace(".", "") };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Verilen sektör nesnesini kullanarak, ilgili sektörün veri tabanına
        /// kayıtlı alt sektörleri var mı kontrolünü yapan yordam
        /// </summary>
        /// <param name="sektor">Sektör bilgisini tutan nesne</param>
        /// <returns>Sektörün; alt sektörleri varsa true, yoksa false döndürülür.</returns>
        public bool AltSektorVarMi(int yil, SektorSozluk sektor)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*)" + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SEKTORNO], " LIKE ", sektor.kod.Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            return (int)veriErisim.TekKolonSQLDbl(sql) > 1;
        }
    }
}