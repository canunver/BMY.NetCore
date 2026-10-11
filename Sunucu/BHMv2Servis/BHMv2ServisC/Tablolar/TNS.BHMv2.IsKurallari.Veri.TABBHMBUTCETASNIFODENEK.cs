using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Bütçe tasnifine girilen ödenekleri tutan BHMBUTCETASNIFODENEK veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABBHMBUTCETASNIFODENEK : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMBUTCETASNIFODENEK(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tasnif ödenekleri tablosunun adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMBUTCETASNIFODENEK";

        /// <summary>
        /// Tasnif ödenekleri tablosunun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// TASNIFNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASNIFNO = 0,

            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 1,

            /// <summary>
            /// SENARYOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 2,

            /// <summary>
            /// HIYERARSIKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIYERARSIKOD = 3,

            /// <summary>
            /// ODENEK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEK = 4,
        }

        /// <summary>
        /// Tasnif ödenekleri tablosunun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "TASNIFNO"
            , "YIL"
            , "SENARYOKOD"
            , "HIYERARSIKOD"
            , "ODENEK"
        };

        /// <summary>
        /// ButceTasnifOdenekleriFormu yapısında duran anahtar alanları kullanarak, 
        /// verilen tasnifin bir yıl ve senaryodaki tüm verilerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="butceTasnifOdenekleriFormu">Silinecek ödeneklerin tasnif kodunu, yılını tutan ButceTasnifOdenekleriFormu yapısında nesne.</param>
        /// <param name="satirNo">Silinecek ödeneklerin hiyerarsi kodunun bulunduğu satır</param>
        /// <param name="senaryoKod">Silinecek ödeneklerin senaryosunu tutan değişken.</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(ButceTasnifOdenekleriFormu butceTasnifOdenekleriFormu, int satirNo, string senaryoKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", butceTasnifOdenekleriFormu.tasnif.siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", butceTasnifOdenekleriFormu.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (satirNo >= 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIYERARSIKOD], "=", butceTasnifOdenekleriFormu.butceTasnifSatirOdenekleri[satirNo].hiyerarsi.kod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// ButceTasnifOdenekleriFormu yapısında duran bir satırı veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="butceTasnifOdenekleriFormu">Eklenecek satıra ait bilgileri tutan, ButceTasnifOdenekleriFormu yapısında nesne</param>
        /// <param name="satirNo">ButceTasnifOdenekleriFormu içindeki butceTasnifSatirOdenekleri dizisinin kaçıncı satırının ekleneceğini gösteren parametre</param>
        /// <param name="senaryoKod">Eklenecek ödeneğin senaryo kodu.</param>
        /// <param name="ekDegerNo">Eklenecek ödeneğin, ek değerler dizisinin kaçıncı elemanı olduğu bilgisi.</param>
        /// <returns>Ekleme komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(ButceTasnifOdenekleriFormu butceTasnifOdenekleriFormu, int satirNo, string senaryoKod, int ekDegerNo)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(butceTasnifOdenekleriFormu.tasnif.siraNo);
            sql += "," + VeriErisim.DegerYap(butceTasnifOdenekleriFormu.yil);
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(butceTasnifOdenekleriFormu.butceTasnifSatirOdenekleri[satirNo].hiyerarsi.kod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(butceTasnifOdenekleriFormu.butceTasnifSatirOdenekleri[satirNo].ekDegerler[ekDegerNo]);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Bir tasnifin satırı silinmişse, bu satırla ilgili tüm yıllardaki ve senaryolardaki tüm verileri silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tasnifNo">Satıra ait ödenekleri silinecek tasnifin kodu</param>
        /// <param name="hiyerarsikSatirKodu">Ödenekleri silinecek tasnif satırının, tasnif içi satır kodu</param>
        /// <returns>Sil komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int tasnifNo, string hiyerarsikSatirKodu)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIYERARSIKOD], "=", hiyerarsikSatirKodu.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// ButceTasnifOdenekleriFormu yapısında bilgileri verilen bir tasnifin, bir yıl ve senaryodaki ödeneklerinin, ait olduğu satırın adı ile birlikte döndüren,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="butceTasnifOdenekleriFormu">Ödenekleri listelecek tasnifin kodunu, yılı ve senayyo kodu bilgilerini tutan, ButceTasnifOdenekleriFormu yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(ButceTasnifOdenekleriFormu butceTasnifOdenekleriFormu)
        {
            string sql = "SELECT " + TABBHMBUTCETASNIFHIYERARSI.TABLOAD + "." + TABBHMBUTCETASNIFHIYERARSI.ALANLAR[(int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.KOD] + ", ";
            sql += TABBHMBUTCETASNIFHIYERARSI.TABLOAD + "." + TABBHMBUTCETASNIFHIYERARSI.ALANLAR[(int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.AD] + ", ";
            sql += TABBHMBUTCETASNIFHIYERARSI.TABLOAD + "." + TABBHMBUTCETASNIFHIYERARSI.ALANLAR[(int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.ALTKIRILIMVAR] + ", ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODENEK];
            sql += " FROM " + TABLOAD + " RIGHT OUTER JOIN " + TABBHMBUTCETASNIFHIYERARSI.TABLOAD + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TASNIFNO] + " = " + TABBHMBUTCETASNIFHIYERARSI.TABLOAD + "." + TABBHMBUTCETASNIFHIYERARSI.ALANLAR[(int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.TASNIFNO] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HIYERARSIKOD] + " = " + TABBHMBUTCETASNIFHIYERARSI.TABLOAD + "." + TABBHMBUTCETASNIFHIYERARSI.ALANLAR[(int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.KOD];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", butceTasnifOdenekleriFormu.yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", butceTasnifOdenekleriFormu.senaryo.kod.Replace(".", "")));

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABBHMBUTCETASNIFHIYERARSI.TABLOAD, TABBHMBUTCETASNIFHIYERARSI.ALANLAR[(int)TABBHMBUTCETASNIFHIYERARSI.ENUMAlanlar.TASNIFNO], "=", butceTasnifOdenekleriFormu.tasnif.siraNo));
            sql += " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hiyerarşik satırın kodu değişmişse, ödeneklerdeki kodu da değiştiren
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
        /// Detayları verilen bir tasnif satırının ödeneğini bulup döndüren yordam. Ödeneği bulan sql oluşturulur, çalıştırılır ve bulunan değer döndürülür.
        /// </summary>
        /// <param name="tasnifNo">Ödenek bilgisi bulunacak tasnifin numarası.</param>
        /// <param name="yil">Ödeneğin ait olduğu yıl.</param>
        /// <param name="senaryoKod">Ödeneğin ait olduğu senaryonun kodu.</param>
        /// <param name="hiyerarsikSatirKod">Tasnifin hangi satırına ait ödeneğin bulunacağı.</param>
        /// <returns>Verilen bilgilere ait ödenek miktarı döndürülür.</returns>
        public double OdenekBul(int tasnifNo, int yil, string senaryoKod, string hiyerarsikSatirKod)
        {
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODENEK];
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIYERARSIKOD], "=", hiyerarsikSatirKod.Replace(".", "")));
            return veriErisim.TekKolonSQLDbl(sql);

        }

        public Sonuc Sil(int tasnifNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            return KomutCalistir(sql, 0);
        }
    }
}
