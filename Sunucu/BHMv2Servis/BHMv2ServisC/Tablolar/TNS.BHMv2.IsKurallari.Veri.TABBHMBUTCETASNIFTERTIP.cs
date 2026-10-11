using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Bütçe tasnifinin her bir satırı ile ilişkili tertip şablonlarını tutan BHMBUTCETASNIFTERTIP veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABBHMBUTCETASNIFTERTIPSABLON : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMBUTCETASNIFTERTIPSABLON(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tasnif satırı tertip şablonları tablosunun adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMBUTCETASNIFTERTIP";

        /// <summary>
        /// Tasnif satırı tertip şablonları tablosunun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// TASNIFNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASNIFNO = 0,

            /// <summary>
            /// HIYERARSIKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIYERARSIKOD = 1,

            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,

            /// <summary>
            /// TERTIPSABLON alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TERTIPSABLON = 3,

            /// <summary>
            /// EKLECIKAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKLECIKAR = 4
        }

        /// <summary>
        /// Tasnif satırı tertip şablonları tablosunun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "TASNIFNO"
            , "HIYERARSIKOD"
            , "SIRANO"
            , "TERTIPSABLON"
            , "EKLECIKAR"
        };

        /// <summary>
        /// Bir tasnifin satırı silinmişse, bu satırla ilgili tüm tertip şablonlarını silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tasnifNo">Satıra ait tertip şablonları silinecek tasnifin kodu</param>
        /// <param name="hiyerarsikSatirKodu">Tertip şablonları silinecek tasnif satırının, tasnif içi satır kodu</param>
        /// <returns>Sil komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int tasnifNo, string hiyerarsikSatirKodu)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIYERARSIKOD], "=", hiyerarsikSatirKodu.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }


        /// <summary>
        /// Bütçe tasnif satırına ait tertip şablonunu veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tasnifNo">Eklenecek tertip şablonun ait olduğu tasnif numarası</param>
        /// <param name="butceTasnifHiyerarsikSatir">Eklenecek tertip şablonun ait olduğu satıra ait bilgileri tutan nesne</param>
        /// <param name="satirNo">Eklenecek tertip şablonun, tertipSablonlar dizisinin kaçıncı satırı olduğunu gösteren parametre</param>
        /// <param name="yil">Eklenecek tertip şablonun hangi yıla ait olduğu bilgisini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int tasnifNo, ButceTasnifHiyerarsikSatir butceTasnifHiyerarsikSatir, int satirNo)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(tasnifNo);
            sql += "," + VeriErisim.DegerYap(butceTasnifHiyerarsikSatir.yeniKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(butceTasnifHiyerarsikSatir.tertipSablonlar[satirNo].tertipSablon);
            sql += "," + VeriErisim.DegerYap((int)butceTasnifHiyerarsikSatir.tertipSablonlar[satirNo].ekleCikar);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Hiyerarşik satırın kodu değişmişse, tertip şablonlarındaki kodu da değiştiren
        /// update sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tasnifNo">Satır kodu değişen tasnifin kodu</param>
        /// <param name="butceTasnifHiyerarsikSatir">Değişen satır kodunun, eski kodunu ve yeni kodunu tutan nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(int tasnifNo, ButceTasnifHiyerarsikSatir butceTasnifHiyerarsikSatir)
        {
            string sql = "UPDATE " + TABLOAD + " SET";
            sql += " " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HIYERARSIKOD], butceTasnifHiyerarsikSatir.yeniKod.Replace(".", ""));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIYERARSIKOD], "=", butceTasnifHiyerarsikSatir.kod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Bir tasnife ait hiyerarşik satırdaki tüm tertip şablonlarını döndüren,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="tasnifNo">Satıra ait tertip şablonları listelenecek tasnifin kodu</param>
        /// <param name="hiyerarsikSatirKodu">Tertip şablonları listelenecek tasnif satırının, tasnif içi satır kodu</param>
        /// <param name="yil">Tertip şablonları listelenecek tasnif satırının yılı</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int tasnifNo, string hiyerarsikSatirKodu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIYERARSIKOD], "=", hiyerarsikSatirKodu.Replace(".", "")));
            sql += " ORDER BY 1,2,3";
            return veriErisim.DataReaderDondur(sql);
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

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, ButceTasnifTertipyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        //public Sonuc Sil(ButceTasnifTertip form)
        //{
        //    string sql = "DELETE FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", form.tasnifNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIYERARSIKOD], "=", form.hiyerarsiKod.Replace(".", "")));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
        //    return KomutCalistir(sql, 0);
        //}

        //public Sonuc Sil(ButceTasnifHiyerarsi form)
        //{
        //    string sql = "DELETE FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", form.butceTasnif.siraNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIYERARSIKOD], "=", form.kod.Replace(".", "")));
        //    return KomutCalistir(sql, 0);
        //}

        public Sonuc Sil(int butceTasnifsiraNo, int yil)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", butceTasnifsiraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            return KomutCalistir(sql, 0);
        }


        /// <summary>
        /// Bir tasnifin satırının verilen yıldaki tertip şablonlarını silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tasnifNo">Satıra ait tertip şablonları silinecek tasnifin kodu</param>
        /// <param name="hiyerarsikSatirKodu">Tertip şablonları silinecek tasnif satırının, tasnif içi satır kodu</param>
        /// <param name="yil">Tertip şablonları silinecek tasnif satırının yılı</param>
        /// <returns>Sil komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int tasnifNo, string hiyerarsikSatirKodu)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIYERARSIKOD], "=", hiyerarsikSatirKodu.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }



*/