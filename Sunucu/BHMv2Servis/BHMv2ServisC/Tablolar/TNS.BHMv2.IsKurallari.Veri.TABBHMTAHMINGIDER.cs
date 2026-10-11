using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Yıl sonu harcama tahmininin tutulduğu, BHMTAHMINGELIR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// TABBHMODENEK soyut sınıfından üretilmiştir.
    /// </summary>
    public class TABBHMTAHMINGIDER : TABBHMTAHMIN
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMTAHMINGIDER(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Yıl sonu harcama tahmininin tutulduğu tablonun adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMTAHMINGIDER";

        /// <summary>
        /// Yıl sonu harcama tahmininin tutulduğu tablonun alanlarının sırasını tutan enumeration
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
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 4,

            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 5,

            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 6,

            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 7,

            /// <summary>
            /// ODENEK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEK = 8,
        }

        /// <summary>
        /// Yıl sonu harcama tahmininin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "SENARYOKOD"
            , "TEKLIFYIL"
            , "KURKOD"
            , "FONKOD"
            , "FINKOD"
            , "EKOKOD"
            , "PEBKOD"
            , "ODENEK"
        };

        /// <summary>
        /// Yıl sonu harcama tahmininin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR_ACIK = {
            "YIL"
            , "SENARYOKOD"
            , "TEKLIFYIL"
            , "KURKOD"
            , "FONKOD"
            , "FINKOD"
            , "EKOKOD"
            , "PEBKOD"
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
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(tertip.kodlar, 0));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(tertip.kodlar, 1));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(tertip.kodlar, 2));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(tertip.kodlar, 3));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(tertip.kodlar, 4));
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
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(tertip.kodlar, 0));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(tertip.kodlar, 1));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(tertip.kodlar, 2));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(tertip.kodlar, 3));
            sql += "," + VeriErisim.DegerYap(TABBHMGENEL.NoktaAt(tertip.kodlar, 4));
            sql += "," + VeriErisim.DegerYap(aciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Yıl sonu harcama tahmini tablosu için verilen kriterlerle sql sorgu koşulu oluşturup döndüren yordam.
        /// </summary>
        /// <param name="yil">Yıl kıstası değeri, -1 değilse koşula eklenir.</param>
        /// <param name="senaryoKod">Yıl kıstası değeri, boş değilse koşula eklenir.</param>
        /// <param name="tertip">Tertip kıstası değeri, tertip alanlarından boş olmayanlar koşula eklenir.</param>
        /// <param name="teklifYil">Çok yıllı ödenek değeri, -1 değilse koşula eklenir.</param>
        /// <param name="tabloAd">Koşul alanlarının başına tablo adı getirilecekse gelecek tablo adı bu parametre ile verilmelidir.</param>
        /// <returns>Verilen kıstaslara göre oluşturulan sql sorgu koşulu döndürülür.</returns>
        private static string KistasKosulOl(int yil, string senaryoKod, Tertip tertip, int teklifYil, string tabloAd)
        {
            string kosul = "";

            if (yil != -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (!string.IsNullOrEmpty(senaryoKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            if (tertip.kodlar.Length >= 1 && !String.IsNullOrEmpty(tertip.kodlar[0]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));

            if (tertip.kodlar.Length >= 2 && !String.IsNullOrEmpty(tertip.kodlar[1]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, ALANLAR[(int)ENUMAlanlar.FONKOD], "LIKE", tertip.kodlar[1].Replace(".", "") + "%"));

            if (tertip.kodlar.Length >= 3 && !String.IsNullOrEmpty(tertip.kodlar[2]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, ALANLAR[(int)ENUMAlanlar.FINKOD], "LIKE", tertip.kodlar[2].Replace(".", "") + "%"));

            if (tertip.kodlar.Length >= 4 && !String.IsNullOrEmpty(tertip.kodlar[3]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", tertip.kodlar[3].Replace(".", "") + "%"));

            if (tertip.kodlar.Length >= 5 && !String.IsNullOrEmpty(tertip.kodlar[4]))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, ALANLAR[(int)ENUMAlanlar.PEBKOD], "LIKE", tertip.kodlar[4].Replace(".", "") + "%"));

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
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD + " WHERE " + KistasKosulOl(kistas.yil, senaryoKod, kistas, 0, "") + " order by 1,2,3,4,5,6,7,8";
            return this.veriErisim.DataReaderDondur(sql);
        }

        internal Sonuc Kopyala(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") SELECT ";
            sql += VeriErisim.DegerYap(hedefTertip.yil - 1) + " AS " + ALANLAR[(int)ENUMAlanlar.YIL] + ", ";
            sql += VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " AS " + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.TEKLIFYIL] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.KURKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FONKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FINKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.PEBKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.ODENEK] + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil - 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], hedefTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], hedefTertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], hedefTertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], hedefTertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], hedefTertip, 4);

            return KomutCalistir(sql, 0);
        }

        internal Sonuc KopyalaAcik(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            string sql = "INSERT INTO " + TABLOAD + "_ACIK(" + VeriErisim.TumAlanlar("", ALANLAR_ACIK) + ") SELECT ";
            sql += VeriErisim.DegerYap(hedefTertip.yil - 1) + " AS " + ALANLAR[(int)ENUMAlanlar.YIL] + ", ";
            sql += VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " AS " + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.TEKLIFYIL] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.KURKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FONKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FINKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.PEBKOD] + ", ";
            sql += "ACIKLAMA" + " FROM " + TABLOAD + "_ACIK";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil - 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], hedefTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], hedefTertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], hedefTertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], hedefTertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], hedefTertip, 4);

            return KomutCalistir(sql, 0);
        }
    }
}
