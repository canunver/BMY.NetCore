using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BKD;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Gelir ödeneklerinin tutulduğu, BHMODENEKGELIR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// TABBHMODENEK soyut sınıfından üretilmiştir.
    /// </summary>
    public class TABBHMODENEKFINANSMAN : TABBHMODENEK
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMODENEKFINANSMAN(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Gelir ödeneklerinin tutulduğu tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMODENEKFINANSMAN";

        /// <summary>
        /// Gelir ödeneklerinin tutulduğu tablonun alanlarının sırasını tutan enumeration
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
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 4,

            /// <summary>
            /// ODENEK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEK = 5,
        }

        /// <summary>
        /// Gelir ödeneklerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "SENARYOKOD"
            , "TEKLIFYIL"
            , "KURKOD"
            , "FINKOD"
            , "ODENEK"
        };

        /// <summary>
        /// Gelir ödeneklerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR_ACIK = {
            "YIL"
            , "SENARYOKOD"
            , "TEKLIFYIL"
            , "KURKOD"
            , "FINKOD"
            , "ACIKLAMA"
        };

        /// <summary>
        /// Verilen ödenek bilgisini; ilgili tertibin yılı, senaryo ve teklif yılına ekleyen
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

        public override Sonuc EkleAciklama(int yil, string senaryoKod, int teklifYil, Tertip tertip, string aciklama)
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
        /// Parametre olarak verilen kıstasları kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam
        /// </summary>
        /// <param name="yil">Listelenecek kayıtların yıl bilgisi</param>
        /// <param name="senaryoKod">Listelenecek kayıtların senaryo kodu</param>
        /// <param name="teklifYil">Ödenekler çok yıllı teklif edildiğinden, verilen yılda hangi çok yıllı ödeneğin listeleneceği bilgisi (-1 ise hepsi, 0 ise ilk yıl, 1 ise ikinci yıl ...)</param>
        /// <param name="kodlar">Listelenecek tertiplerin kıstasını tutan, tertip boyutları dizisi</param>
        /// <returns>Select komutunun çalıştırılmasının sonucunda elde edilen DataReader nesnesini döndürür.</returns>
        public override IDataReader TertipListesi(int yil, string senaryoKod, int teklifYil, string[] kodlar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (teklifYil > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFYIL], "=", teklifYil));

            string[] alanAdlar = new string[2];
            alanAdlar[0] = ALANLAR[(int)ENUMAlanlar.KURKOD];
            alanAdlar[1] = ALANLAR[(int)ENUMAlanlar.FINKOD];


            for (int i = 0; i < kodlar.Length; i++)
            {
                if (!String.IsNullOrEmpty(kodlar[i]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", alanAdlar[i], "LIKE", kodlar[i].Replace(".", "") + "%"));

            }
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen kıstasları kullanarak, bir tertibe ait çok yıllı ödenekleri silen, 
        /// delete sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Silinecek kaydın yıl bilgisi</param>
        /// <param name="senaryoKod">Silinecek kaydın senaryo kodu</param>
        /// <param name="tertip">Silinecek tertibin bilgisini tutan Tertip nesnesi</param>
        /// <returns>Silme komutunun çalıştırılmasının sonucu, Sonuc yapısında döndürür</returns>
        public override Sonuc CokYilliOdenekSil(int yil, string senaryoKod, Tertip tertip)
        {
            string sql = "DELETE FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", tertip.kodlar[1].Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public override Sonuc CokYilliOdenekSilAciklama(int yil, string senaryoKod, Tertip tertip)
        {
            string sql = "DELETE FROM " + TABLOAD + "_ACIK";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", tertip.kodlar[1].Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Ödenekle ilgili bir yıl, senaryo kod ve kurumsal kod bazında yapılan son değişiklik zamanını
        /// verilen zaman tablosu kullanılarak kaydeden yordam. 
        /// Kaydedilen zaman sayesinde, Rapor alınırken, raporun üretim zamanı 
        /// ve verinin son değiştiği tarih kontrol edilerek yeni rapor üretilip üretilmeyeceğine karar verilir.
        /// </summary>
        /// <param name="tabZaman">Değişim zamanı bilgisini kaydetmek için kullanılacak table nesnesi</param>
        /// <param name="yil">Değişim zamanının yıl bilgisi. Her yıl için ayrı değişim zamanı tutulur.</param>
        /// <param name="senaryoKod">Değişim zamanının senaryo bilgisi. Her senaryo için ayrı değişim zamanı tutulur.</param>
        /// <param name="kurKod">Değişim zamanının kurumsal kod bilgisi. Her kurumsal kod için ayrı değişim zamanı tutulur.</param>
        /// <param name="kullaniciKod">Değişimi yapan kullanıcının kodu</param>
        /// <returns>Değişim kayıt işleminin sonucu, Sonuc yapısında döndürülür.</returns>
        public override Sonuc DegisimZamaniAyarla(TABDEGISIMZAMAN tabZaman, int yil, string senaryoKod, string kurKod, string kullaniciKod)
        {
            return tabZaman.OdenekZamanKaydet(new DegisimZaman(yil, senaryoKod, kurKod, ENUMZamanDamgaTur.GELIR, kullaniciKod));
        }

        public override Sonuc OdenekKopyala(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") SELECT ";
            sql += VeriErisim.DegerYap(hedefTertip.yil) + " AS " + ALANLAR[(int)ENUMAlanlar.YIL] + ", ";
            sql += VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " AS " + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.TEKLIFYIL] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.KURKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FINKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.ODENEK] + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], hedefTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], hedefTertip, 1);

            Sonuc s = KomutCalistir(sql, 0);
            if (s.islemSonuc)
                return OdenekKopyalaAcik(hedefTertip, hedefAsama, kaynakYil, kaynakAsama);
            else
                return s;
        }

        public Sonuc OdenekKopyalaAcik(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            string sql = "INSERT INTO " + TABLOAD + "_ACIK(" + VeriErisim.TumAlanlar("", ALANLAR_ACIK) + ") SELECT ";
            sql += VeriErisim.DegerYap(hedefTertip.yil) + " AS " + ALANLAR[(int)ENUMAlanlar.YIL] + ", ";
            sql += VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " AS " + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.TEKLIFYIL] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.KURKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FINKOD] + ", ";
            sql += "ACIKLAMA" + " FROM " + TABLOAD + "_ACIK";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], hedefTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], hedefTertip, 1);

            return KomutCalistir(sql, 0);
        }
    }
}
