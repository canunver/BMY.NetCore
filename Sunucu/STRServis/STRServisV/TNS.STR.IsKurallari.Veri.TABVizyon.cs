using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.STR.IsKurallari.Veri
{
    /// <summary>
    /// Vizyon bilgilerini tutan STRVIZYON veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABVizyon : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABVizyon(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRVIZYON";

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
        /// Vizyonun tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "DONEMBYIL" };

        /// <summary>
        /// Vizyon bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="vizyon">Vizyon bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Vizyon vizyon)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(vizyon.donemBYil) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen vizyon nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve vizyon bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="vizyon">Vizyon bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan vizyon bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(Vizyon vizyon)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", TABYazi.ALANLAR) + " FROM " + TABYazi.TABLOAD;

            string kosul = string.Empty;
            if (vizyon.donemBYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DONEMBYIL], "=", vizyon.donemBYil));
            if (vizyon.dil.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DILKOD], "=", vizyon.dil.kod));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KODTUR], "=", (int)ENUMStratejikPlan.Vizyon));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KOD], "=", 1));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            //sql += " ORDER BY " + TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KISAYAZI];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen dönem başlangıç yılını kullanarak vizyonun veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool VizyonVarMi(int donemBYil)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.DONEMBYIL] };
            int[] intDeger ={ donemBYil };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }
    }
}