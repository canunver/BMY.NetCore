using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Bütçe tasnifinin satırlarını tutan BHMBUTCETASNIFHIYERARSI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABBHMBUTCETASNIFHIYERARSI : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMBUTCETASNIFHIYERARSI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tasnif satırları tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMBUTCETASNIFHIYERARSI";

        /// <summary>
        /// Tasnif satırları alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// TASNIFNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASNIFNO = 0,

            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 1,

            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 2,

            /// <summary>
            /// ALTKIRILIMVAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALTKIRILIMVAR = 3,
        }

        /// <summary>
        /// Tasnif satırları tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "TASNIFNO"
            , "KOD"
            , "AD"
            , "ALTKIRILIMVAR"
        };

        /// <summary>
        /// Verilen tasnifin, bir satırını veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tasnifNo">Satırı silinecek tasnifin kodu</param>
        /// <param name="hiyerarsikSatirKodu">Silinecek tasnif satırının, tasnif içi satır kodu</param>
        /// <returns>Sil komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int tasnifNo, string hiyerarsikSatirKodu)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hiyerarsikSatirKodu.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Kodu verilen tasnife, bilgileri verilen hiyerarşik tasnif satırını ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tasnifNo">Hiyerarşik tasnif satırı eklenecek tasnifin kodu</param>
        /// <param name="tasnifSatiri">Eklenecek hiyerarşik tasnif satırına ait bilgileri tutan nesne</param>
        /// <returns>Ekleme komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int tasnifNo, ButceTasnifHiyerarsikSatir tasnifSatiri)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(tasnifNo);
            sql += "," + VeriErisim.DegerYap(tasnifSatiri.yeniKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tasnifSatiri.ad);
            sql += "," + VeriErisim.DegerYap(tasnifSatiri.altKirilimVar?1:0);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen Tasnif kodunu kullanarak, tasnifin tüm hiyerarsik satırlarını döndüren,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="tasnifNo">Hiyerarşik tasnif satırı bilgisi bulunacak tasnifin kodu</param>
        /// <param name="hiyerarsiKod">Dolu ise sadece ilgili satır okunur.</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int tasnifNo, string hiyerarsiKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            if (!string.IsNullOrEmpty(hiyerarsiKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hiyerarsiKod.Replace(".", "")));
            sql += " ORDER BY 1,2";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tasnif satırının bilgilerini, verilen tasnif no ve tasnif satırı bilgileri ile güncelleyen
        /// sql komutunu oluşturup çalıştıran ve sonucunu döndüren yordam.
        /// Tasnif satırı bilgileri içerisinde eski tasnif satırı kodu (kod) ve satırın yeni kodu (yenikod) alanı da bulunmalıdır.
        /// </summary>
        /// <param name="tasnifNo">Hiyerarşik tasnif satırı güncellenecek tasnifin kodu</param>
        /// <param name="tasnifSatiri">Güncellenecek hiyerarşik tasnif satırına ait bilgileri tutan nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(int tasnifNo, ButceTasnifHiyerarsikSatir tasnifSatiri)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOD], tasnifSatiri.yeniKod.Replace(".", "")) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], tasnifSatiri.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTKIRILIMVAR], tasnifSatiri.altKirilimVar?1:0);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", tasnifSatiri.kod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int tasnifNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            return KomutCalistir(sql, 0);
        }
    }
}

/*

        //public Sonuc Sil(int tasnifNo)
        //{
        //    string sql = "DELETE FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
        //    return KomutCalistir(sql, 0);
        //}


        /// <summary>
        /// Tasnif kodu ve hiyerarsik satır kodu verilen tasnif satırı bilgilerini kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="tasnifNo">Hiyerarşik tasnif satırı bilgisi bulunacak tasnifin kodu</param>
        /// <param name="hiyerarsiKod">Hiyerarşik tasnif satırı bilgisi bulunacak tasnif satırının kodu</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(int tasnifNo, string hiyerarsiKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hiyerarsiKod.Replace(".", "")));
            return veriErisim.DataReaderDondur(sql);
        }


*/