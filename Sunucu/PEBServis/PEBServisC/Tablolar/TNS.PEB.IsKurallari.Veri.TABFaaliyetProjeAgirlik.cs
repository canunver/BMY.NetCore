using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Faaliyet/projelerin performans hedeflerle bağını kuran ağırlık bilgilerini
    /// tutan PEBFAALIYETPROJEAGIRLIK veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABFaaliyetProjeAgirlik : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFaaliyetProjeAgirlik(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBFAALIYETPROJEAGIRLIK";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// FPKURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            FPKURKOD = 1,

            /// <summary>
            /// FPKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            FPKOD = 2,

            /// <summary>
            /// PHKURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PHKURKOD = 3,

            /// <summary>
            /// PERHEDEFKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PERHEDEFKOD = 4,

            /// <summary>
            /// AGIRLIK alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AGIRLIK = 5
        }

        /// <summary>
        /// Faaliyet/projenin ağırlık bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "FPKURKOD", "FPKOD", "PHKURKOD", "PERHEDEFKOD", "AGIRLIK" };

        /// <summary>
        /// Faaliyet/projenin ağırlık bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="fpAgirlik">Faaliyet/projenin ağırlık bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(FaaliyetProjeAgirlik fpAgirlik)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(fpAgirlik.yil) + ",";
            sql += VeriErisim.DegerYap(fpAgirlik.fpKurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(fpAgirlik.fpKod) + ",";
            sql += VeriErisim.DegerYap(fpAgirlik.phKurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(fpAgirlik.perHedefKod) + ",";
            sql += VeriErisim.DegerYap(fpAgirlik.agirlik) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen faaliyet/proje ağırlık nesnesinin yılını,
        /// birim kodunu ve faaliyet/proje programatik kodunu kullanarak,
        /// ilgili faaliyet/projenin ağırlık bilgilerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="fpAgirlik">Faaliyet/projenin ağırlık bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(FaaliyetProjeAgirlik fpAgirlik)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", fpAgirlik.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FPKURKOD], "=", fpAgirlik.fpKurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FPKOD], "=", fpAgirlik.fpKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen yılı, birim kodunu ve faaliyet/proje programatik kodunu
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve faaliyet/projeye ait ağırlıkları DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="fpKod">Faaliyet/proje programatik kodu</param>
        /// <returns>Verilen kriterlere uygun olan faaliyet/proje ağırlıklarını tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int yil, string kurKod, int fpKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",PEBPERHEDEF.HEDEF, PEBPERHEDEF.EBUTCEKOD, PEBPERHEDEF.HEDEFGORKOD FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN PEBPERHEDEF ON PEBFAALIYETPROJEAGIRLIK.YIL = PEBPERHEDEF.YIL AND PEBFAALIYETPROJEAGIRLIK.PHKURKOD = PEBPERHEDEF.KURKOD AND  PEBFAALIYETPROJEAGIRLIK.PERHEDEFKOD = PEBPERHEDEF.HEDEFKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (!string.IsNullOrEmpty(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FPKURKOD], "=", kurKod.Replace(".", "")));
            if (fpKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FPKOD], "=", fpKod));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}