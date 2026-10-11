using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Alt faaliyetlerin hangi birimlerde çalıştığı bilgisini tutan ABSPEBKODKURYAPI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABABSPEBKODKURYAPI : TABOrtak
    {
    /// <summary>
    ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
    /// </summary>
    /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABABSPEBKODKURYAPI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ABSPEBKODKURYAPI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 1,

            /// <summary>
            /// SABLONNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SABLONNO = 2,

            /// <summary>
            /// SABLON alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SABLON = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "PEBKOD"
            , "SABLONNO"
            , "SABLON"
        };

        /// <summary>
        /// AltFaaliyet yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="altFaaliyet">Anahtar değişkenleri tutan, AltFaaliyet yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(AltFaaliyet altFaaliyet)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", altFaaliyet.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", altFaaliyet.pebKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, kurYapiSablonListe dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="altFaaliyet">Eklenecek kayıt bilgisini tutan, AltFaaliyet yapısında nesne</param>
        /// <param name="satirNo">kurYapiSablonListe dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(AltFaaliyet altFaaliyet, int satirNo)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(altFaaliyet.yil);
            sql += "," + VeriErisim.DegerYap(altFaaliyet.pebKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(altFaaliyet.kurYapiSablonListe[satirNo]);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// AltFaaliyet yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="altFaaliyet">Select edilecek kayıtlara ait kıstas bilgisini tutan, AltFaaliyet yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(AltFaaliyet altFaaliyet)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", altFaaliyet.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", altFaaliyet.pebKod.Replace(".", "")));
            sql += " ORDER BY 3";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
