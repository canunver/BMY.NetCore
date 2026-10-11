using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Dil tanımlarını tutan STRDIL veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABDil : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABDil(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRDIL";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// DILKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DILKOD = 0,

            /// <summary>
            /// DILAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DILAD = 1
        }

        /// <summary>
        /// Dil tanımlarının tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "DILKOD", "DILAD" };

        /// <summary>
        /// Dil tanım bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Dil tanımın programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="dil">Dil tanım bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Dil dil)
        {
            dil.kod = DilKodAl();
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(dil.kod) + ",";
            sql += VeriErisim.DegerYap(dil.ad) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = dil.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan dil tanım bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="dil">Dil tanım bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(Dil dil)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DILAD], dil.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DILKOD], "=", dil.kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen dil nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve dil tanımlarını DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="dil">Dil tanım bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan dil tanımlarını tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(Dil dil)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (dil.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DILKOD], "=", dil.kod));
            if (!string.IsNullOrEmpty(dil.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DILAD], " LIKE ", "%" + dil.ad + "%"));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            //sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.DILAD];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen dil nesnesini kullanarak,
        /// ilgili dil tanımını veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="dil">Dil tanım bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Dil dil)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DILKOD], "=", dil.kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen dil tanım programatik kodunu kullanarak
        /// dil tanımının veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="dilKod">Dil tanımının programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool DilVarMi(int dilKod)
        {
            return KayitVarmi(TABLOAD, dilKod);
        }

        /// <summary>
        /// Verilen dil tanımın, stratejik planda kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="dilKod">Dil tanımının programatik kodu</param>
        /// <returns>Dil tanımı; stratejik planda kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool DilKullanilmisMi(int dilKod)
        {
            return KayitVarmi(TABYazi.TABLOAD, dilKod);
        }

        /// <summary>
        /// Verilen dil tanım programatik kodunu kullanarak dil tanımın verilen
        /// veri tabanı tablosunda kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="tablo">Veri tabanı tablo adı</param>
        /// <param name="dilKod">Dil tanımının programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        private bool KayitVarmi(string tablo, int dilKod)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.DILKOD] };
            int[] intDeger ={ dilKod };
            return TABGenel.KayitVarmi(veriErisim, tablo, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Yeni dil tanım programatik kodu döndüren yordam
        /// </summary>
        /// <returns>Yeni dil tanım programatik kodu döndürülür.</returns>
        private int DilKodAl()
        {
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.DILKOD], new string[0], new string[0], new string[0], new int[0]);
        }    
    }
}