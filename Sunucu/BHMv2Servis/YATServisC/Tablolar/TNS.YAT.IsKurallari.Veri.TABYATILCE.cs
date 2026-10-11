using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// İlçe bilgilerini tutan YATILCE veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATIlce : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATIlce(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATILCE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// ILKODU alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ILKODU = 0,

            /// <summary>
            /// ILCEKODU alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ILCEKODU = 1,

            /// <summary>
            /// ILCEADI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ILCEADI = 2
        }

        /// <summary>
        /// İlçelerin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "ILKODU", "ILCEKODU", "ILCEADI" };

        /// <summary>
        /// İlçe bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="ilce">İlçe bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(IlceSozluk ilce)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(ilce.il.kod) + ",";
            sql += VeriErisim.DegerYap(ilce.ilceKodu) + ",";
            sql += VeriErisim.DegerYap(ilce.ilceAdi) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan ilçe bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="ilce">İlçe bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(IlceSozluk ilce)
        {
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCEADI], ilce.ilceAdi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKODU], "=", ilce.il.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILCEKODU], "=", ilce.ilceKodu));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen ilçe nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve ilçeleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="ilce">İlçe bilgisini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan ilçeleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(IlceSozluk ilce)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (ilce.il.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKODU], "=", ilce.il.kod));

            if (ilce.ilceKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILCEKODU], "=", ilce.ilceKodu));

            if (ilce.ilceAdi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILCEADI], " LIKE ", "%" + ilce.ilceAdi + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ILKODU] + "," + ALANLAR[(int)ENUMAlanlar.ILCEKODU];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen ilçe nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve ilçeleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="ilce">İlçe bilgisini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan ilçeleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader ListeleOneri(IlceSozluk ilce)
        {
            string sql = "";
            string kosul = "";
            string ilce_ilceAdi = ilce.ilceAdi;
            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILKODU] + "," +
                            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILCEKODU] + "," +
                            TABYATIl.TABLOAD + "." + TABYATIl.ALANLAR[(int)TABYATIl.ENUMAlanlar.ILADI] + "," +
                            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILCEADI] +
                " FROM " + TABLOAD + " LEFT OUTER JOIN " + TABYATIl.TABLOAD + " ON " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILKODU] + "=" + TABYATIl.TABLOAD + "." + TABYATIl.ALANLAR[(int)TABYATIl.ENUMAlanlar.ILKODU];

            if (ilce_ilceAdi != "")
            {
                ilce_ilceAdi = ilce_ilceAdi.TrimEnd();
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", TABYATIl.ALANLAR[(int)TABYATIl.ENUMAlanlar.ILADI], " LIKE ", "%" + ilce_ilceAdi + "%"));
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILCEADI], " LIKE ", "%" + ilce_ilceAdi + "%"));
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABYATIl.TABLOAD + "." + TABYATIl.ALANLAR[(int)TABYATIl.ENUMAlanlar.ILADI] + "," +
                                  TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILCEADI];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen ilçe nesnesini kullanarak, ilgili ilçeyi veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="ilce">İlçe bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(IlceSozluk ilce)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKODU], "=", ilce.il.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILCEKODU], "=", ilce.ilceKodu));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen ilin, ilçelerden herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ilKodu">İl kodu</param>
        /// <returns>İl; ilçelerde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool IlKullanilmisMi(int ilKodu)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.ILKODU] };
            int[] intDeger = { ilKodu };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }

        //public bool IlceVarMi(IlceSozluk ilce)
        //{
        //    string[] intAlan = { ALANLAR[(int)ENUMAlanlar.ILKODU], ALANLAR[(int)ENUMAlanlar.ILCEKODU] };
        //    int[] intDeger = { ilce.il.kod, ilce.ilceKodu };

        //    return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        //}
    }
}