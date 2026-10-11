using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Gelir yıl sonu gerçekleşme tahmininin tutulduğu, BHMTAHMINGELIR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// TABBHMODENEK soyut sınıfından üretilmiştir.
    /// </summary>
    public class TABBHMTAHMINGELIR : TABBHMTAHMIN
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMTAHMINGELIR(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Gelir yılsonu gerçekleşme tahmini tablosunun adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMTAHMINGELIR";

        /// <summary>
        /// Gelir yılsonu gerçekleşme tahmini tablosunun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// SENARYOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 1,

            /// <summary>
            /// TEKLIFYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFYIL = 2,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 3,

            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 4,

            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 5,

            /// <summary>
            /// ODENEK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEK = 6,
        }

        /// <summary>
        /// Gelir yılsonu gerçekleşme tahmini tablosunun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "SENARYOKOD"
            , "TEKLIFYIL"
            , "KURKOD"
            , "EKOKOD"
            , "PEBKOD"
            , "ODENEK"
        };

        /// <summary>
        /// Gelir yılsonu gerçekleşme tahmini tablosunun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR_ACIK = {
            "YIL"
            , "SENARYOKOD"
            , "TEKLIFYIL"
            , "KURKOD"
            , "EKOKOD"
            , "ACIKLAMA"
        };

        /// <summary>
        /// Verilen gerçekleşme tahmini bilgisini; ilgili tertibin yılı, senaryo ve teklif yılına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Eklenecek ödenek bilgisinin yıl parametresi</param>
        /// <param name="senaryoKod">Eklenecek ödenek bilgisinin ay parametresi</param>
        /// <param name="teklifYil">Ödenekler çok yıllı teklif edildiğinden, verilen yılda hangi çok yıllı ödeneğe ait kaydın ekleneceği bilgisi</param>
        /// <param name="tertip">Eklenecek ödenek bilgisinin tertibini tutan, Tertip yapısında nesne</param>
        /// <param name="odenek">Eklenecek ödeneğin miktarını tutan parametre</param>
        /// <returns>Ekleme komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür</returns>
        public override Sonuc Ekle(int yil, string senaryoKod, int teklifYil, Tertip tertip, double odenek)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(teklifYil);
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(odenek);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public override Sonuc EkleAcik(int yil, string senaryoKod, int teklifYil, Tertip tertip, string aciklama)
        {
            string sql = "INSERT INTO " + TABLOAD + "_ACIK(" + VeriErisim.TumAlanlar("", ALANLAR_ACIK) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(teklifYil);
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(aciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Gelir yıl sonu gerçekleşme tahmini tablosu için verilen kriterlerle sql sorgu koşulu oluşturup döndüren yordam.
        /// </summary>
        /// <param name="yil">Yıl kıstası değeri, -1 değilse koşula eklenir.</param>
        /// <param name="senaryoKod">Yıl kıstası değeri, boş değilse koşula eklenir.</param>
        /// <param name="tertip">Tertip kıstası değeri, tertip alanlarından boş olmayanlar koşula eklenir.</param>
        /// <param name="teklifYil">Çok yıllı ödenek değeri, -1 değilse koşula eklenir.</param>
        /// <param name="tabloAd">Koşul alanlarının başına tablo adı getirilecekse gelecek tablo adı bu parametre ile verilmelidir.</param>
        /// <returns>Verilen kıstaslara göre oluşturulan sql sorgu koşulu döndürülür.</returns>
        public static string KistasKosulOl(int yil, string senaryoKod, Tertip tertip, int teklifYil, string tabloAd)
        {
            string kosul = "";

            if (yil != -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (!string.IsNullOrEmpty(senaryoKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            if (tertip.kodlar.Length >= 1 && !String.IsNullOrEmpty(tertip.kodlar[0]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));

            if (tertip.kodlar.Length >= 2 && !String.IsNullOrEmpty(tertip.kodlar[1]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", tertip.kodlar[1].Replace(".", "") + "%"));

            return kosul;
        }

        /// <summary>
        /// Parametre olarak verilen kıstasları kullanarak, bir tertibe ait çok yıllı gerçekleşme tahminlerini silen, 
        /// delete sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Silinecek kaydın yıl bilgisi</param>
        /// <param name="senaryoKod">Silinecek kaydın senaryo kodu</param>
        /// <param name="tertip">Silinecek tertibin bilgisini tutan Tertip nesnesi</param>
        /// <returns>Silme komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public override Sonuc CokYilliOdenekSil(int yil, string senaryoKod, Tertip tertip)
        {
            string sql = "delete from " + TABLOAD + " WHERE " + KistasKosulOl(yil, senaryoKod, tertip, -1, "");
            return KomutCalistir(sql, 0);
        }

        public override Sonuc CokYilliOdenekSilAcik(int yil, string senaryoKod, Tertip tertip)
        {
            string sql = "delete from " + TABLOAD + "_ACIK WHERE " + KistasKosulOl(yil, senaryoKod, tertip, -1, "");
            return KomutCalistir(sql, 0);
        }

        public override IDataReader Listele(string senaryoKod, Tertip kistas)
        {
            this.veriErisim.hata = "Metot hazır değil";
            return null;
        }

        internal Sonuc Kopyala(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") SELECT ";
            sql += VeriErisim.DegerYap(hedefTertip.yil) + " AS " + ALANLAR[(int)ENUMAlanlar.YIL] + ", ";
            sql += VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " AS " + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.TEKLIFYIL] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.KURKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.PEBKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.ODENEK] + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], hedefTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], hedefTertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], hedefTertip, 2);

            return KomutCalistir(sql, 0);
        }
    }
}
