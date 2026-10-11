using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.STR.IsKurallari.Veri
{
    /// <summary>
    /// Misyon bilgilerini tutan STRMISYON veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABMisyon : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMisyon(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRMISYON";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// DONEMBYIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DONEMBYIL = 0
        }

        /// <summary>
        /// Misyonun tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "DONEMBYIL" };

        /// <summary>
        /// Misyon bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Misyon misyon)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(misyon.donemBYil) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen misyon nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve misyon bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan misyon bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(Misyon misyon)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", TABYazi.ALANLAR) + " FROM " + TABYazi.TABLOAD;

            string kosul = string.Empty;
            if (misyon.donemBYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DONEMBYIL], "=", misyon.donemBYil));
            if (misyon.dil.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DILKOD], "=", misyon.dil.kod));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KODTUR], "=", (int)ENUMStratejikPlan.Misyon));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KOD], "=", 1));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            //sql += " ORDER BY " + TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KISAYAZI];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen dönem başlangıç yılını kullanarak misyonun veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool MisyonVarMi(int donemBYil)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.DONEMBYIL] };
            int[] intDeger ={ donemBYil };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }
    }
}