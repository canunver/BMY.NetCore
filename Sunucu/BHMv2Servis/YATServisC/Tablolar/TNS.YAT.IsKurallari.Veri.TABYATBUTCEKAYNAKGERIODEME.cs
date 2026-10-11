using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Bütçe kaynağın geri ödeme bilgilerini tutan YATBUTCEKAYNAKGERIODEME veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATButceKaynakGeriOdeme : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATButceKaynakGeriOdeme(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATBUTCEKAYNAKGERIODEME";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// KAYNAKNO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KAYNAKNO = 0,

            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ODEMEYIL = 1,

            /// <summary>
            /// ANAPARAGERIODEME alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ANAPARAGERIODEME = 2,

            /// <summary>
            /// ODENECEKFAIZ alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ODENECEKFAIZ = 3,

            /// <summary>
            /// ALTPROJENINSONDURUMU alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ALTPROJENINSONDURUMU = 4,

            /// <summary>
            /// DARBOGAZCOZUM alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DARBOGAZCOZUM = 5,

            YIL = 6, 
            
            SENARYOKOD = 7
        }

        /// <summary>
        /// Bütçe kaynağın geri ödeme bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KAYNAKNO", "ODEMEYIL", "ANAPARAGERIODEME", "ODENECEKFAIZ",
                                   "ALTPROJENINSONDURUMU", "DARBOGAZCOZUM", "YIL", "SENARYOKOD" };

        /// <summary>
        /// Bütçe kaynağın geri ödeme bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="kaynakOdeme">Bütçe kaynağın geri ödeme bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(int yil, string senaryoKod, int kaynakNo, ButceKaynakGeriOdeme kaynakOdeme)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kaynakNo) + ",";
            sql += VeriErisim.DegerYap(kaynakOdeme.yil) + ",";
            sql += VeriErisim.DegerYap(kaynakOdeme.anaParaGeriOdeme) + ",";
            sql += VeriErisim.DegerYap(kaynakOdeme.odenecekFaiz) + ",";
            sql += VeriErisim.DegerYap(kaynakOdeme.projeninSonDurumu) + ",";
            sql += VeriErisim.DegerYap(kaynakOdeme.darBogazCozum) + ",";

            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen bütçe kaynak geri ödeme nesnesini kullanarak,
        /// ilgili geri ödeme bilgilerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="kaynakOdeme">Bütçe kaynağın geri ödeme bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string senaryoKod, int kaynakNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen bütçe kaynağın programatik kodunu ve yılı kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve bütçe kaynak geri ödeme bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="kaynakNo">Bütçe kaynağın programatik kodu</param>
        /// <param name="odemeYil">Yıl kriteri</param>
        /// <returns>Verilen kriterlere uygun olan bütçe kaynak geri ödeme bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int yil, string senaryoKod, int kaynakNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KAYNAKNO] + "," + ALANLAR[(int)ENUMAlanlar.ODEMEYIL];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}