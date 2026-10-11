using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BKD;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Gelir ödeneklerinin tutulduğu, BHMODENEK veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// TABBHMODENEK soyut sınıfından üretilmiştir.
    /// </summary>
    public class TABBHMODENEKGIDER : TABBHMODENEK
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMODENEKGIDER(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Gider ödeneklerinin tutulduğu tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMODENEK";

        /// <summary>
        /// Gider ödeneklerinin tutulduğu tablonun alanlarının sırasını tutan enumeration
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
        /// Gider ödeneklerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
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
        /// Gider ödeneklerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
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
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", ""));
            if (tertip.kodlar.Length > 4)
                sql += "," + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", ""));
            else
                sql += "," + VeriErisim.DegerYap("0");
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
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", ""));
            if (tertip.kodlar.Length > 4)
                sql += "," + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", ""));
            else
                sql += "," + VeriErisim.DegerYap("0");
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

            string[] alanAdlar = new string[5];
            alanAdlar[0] = ALANLAR[(int)ENUMAlanlar.KURKOD];
            alanAdlar[1] = ALANLAR[(int)ENUMAlanlar.FONKOD];
            alanAdlar[2] = ALANLAR[(int)ENUMAlanlar.FINKOD];
            alanAdlar[3] = ALANLAR[(int)ENUMAlanlar.EKOKOD];
            alanAdlar[4] = ALANLAR[(int)ENUMAlanlar.PEBKOD];


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
            Sonuc sonuc;
            string sql = "DELETE FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            if (tertip.kodlar.Length > 4)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", "0"));
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        public override Sonuc CokYilliOdenekSilAciklama(int yil, string senaryoKod, Tertip tertip)
        {
            Sonuc sonuc;
            string sql = "DELETE FROM " + TABLOAD + "_ACIK";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            if (tertip.kodlar.Length > 4)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", "0"));
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
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
            return tabZaman.OdenekZamanKaydet(new DegisimZaman(yil, senaryoKod, kurKod, ENUMZamanDamgaTur.GIDER, kullaniciKod));
        }

        /// <summary>
        /// Verilen senaryodaki kayıt sayısını bulup döndüren yordam. silme öncesi kayıt var mı kontrolü için kullanılır.
        /// </summary>
        /// <param name="senaryoKod">Kayıt sayısı bulunacak senaryo kodu.</param>
        /// <returns>Verilen senaryo koduna sahip kayıt sayısı döndürülür.</returns>
        public double SenaryoKayitSayisi(string senaryoKod)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        public override Sonuc OdenekKopyala(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") SELECT ";
            sql += VeriErisim.DegerYap(hedefTertip.yil) + " AS " + ALANLAR[(int)ENUMAlanlar.YIL] + ", ";
            sql += VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " AS " + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.TEKLIFYIL] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.KURKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FONKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FINKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.PEBKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.ODENEK] + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], hedefTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], hedefTertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], hedefTertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], hedefTertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], hedefTertip, 4);

            Sonuc s = new Sonuc();
            Sonuc s1 = KomutCalistir(sql, 0);
            SonucIsle(s, s1, "Gider Ödenekleri");

            veriErisim.hata = "";
            Sonuc s2 = OdenekKopyalaAcik(hedefTertip, hedefAsama, kaynakYil, kaynakAsama);
            SonucIsle(s, s2, "Gider Ödenek Açıklamaları");

            veriErisim.hata = "";
            Sonuc s3 = OdenekKopyalaTahmin(hedefTertip, hedefAsama, kaynakYil, kaynakAsama);
            SonucIsle(s, s3, "Yıl sonu harcama tahminleri");

            veriErisim.hata = "";
            Sonuc s3a = OdenekKopyalaTahminAcik(hedefTertip, hedefAsama, kaynakYil, kaynakAsama);
            SonucIsle(s, s3a, "Yıl sonu harcama tahmin açıklamaları");

            veriErisim.hata = "";
            Sonuc s4 = OdenekKopyalaEkOdenek(hedefTertip, hedefAsama, kaynakYil, kaynakAsama);
            SonucIsle(s, s4, "Tavan üstü talep edilen ödenekler");

            return s;
        }

        private void SonucIsle(Sonuc s, Sonuc s1, string tabloAd)
        {
            if (s1.islemSonuc)
                s.bilgiStr += String.Format("{0} kopyalandı ({1} Kayıt)", tabloAd, s1.kayitSay);
            else
                s.bilgiStr += String.Format("{0} kopyalanırken hata oluştu: ({1})", tabloAd, s1.hataStr);
        }

        private Sonuc OdenekKopyalaEkOdenek(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            TABBHMTAVANUSTU tTavanUstu = new TABBHMTAVANUSTU(this.veriErisim);
            return tTavanUstu.Kopyala(hedefTertip, hedefAsama, kaynakYil, kaynakAsama);
        }

        private Sonuc OdenekKopyalaTahmin(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            TABBHMTAHMINGIDER tGider = new TABBHMTAHMINGIDER(this.veriErisim);
            return tGider.Kopyala(hedefTertip, hedefAsama, kaynakYil, kaynakAsama);
        }

        private Sonuc OdenekKopyalaTahminAcik(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            TABBHMTAHMINGIDER tGider = new TABBHMTAHMINGIDER(this.veriErisim);
            return tGider.KopyalaAcik(hedefTertip, hedefAsama, kaynakYil, kaynakAsama);
        }


        public Sonuc OdenekKopyalaAcik(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            string sql = "INSERT INTO " + TABLOAD + "_ACIK(" + VeriErisim.TumAlanlar("", ALANLAR_ACIK) + ") SELECT ";
            sql += VeriErisim.DegerYap(hedefTertip.yil) + " AS " + ALANLAR[(int)ENUMAlanlar.YIL] + ", ";
            sql += VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " AS " + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.TEKLIFYIL] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.KURKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FONKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FINKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.PEBKOD] + ", ";
            sql += "ACIKLAMA" + " FROM " + TABLOAD + "_ACIK";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], hedefTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], hedefTertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], hedefTertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], hedefTertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], hedefTertip, 4);

            return KomutCalistir(sql, 0);
        }

        public static string HarcamaBul(Tertip kistasTertip, int yilFark, int kodUzunluk, string kyKod)
        {
            string alanAd = "LEFT(BHMKURUMSALYAPI.KOD, " + kodUzunluk.ToString() + ")";
            string tumSorgu = "SELECT " + alanAd + ", SUM(MUHMIF.BORC - MUHMIF.ALACAK) FROM MUHMIF INNER JOIN";
            tumSorgu += " MUHMIFFORM ON MUHMIF.YIL = MUHMIFFORM.YIL AND MUHMIF.BELGENO = MUHMIFFORM.BELGENO INNER JOIN";
            tumSorgu += " BHMKURUMSALYAPI ON MUHMIFFORM.MUHASEBEKOD = BHMKURUMSALYAPI.SAYMANLIKKOD AND MUHMIFFORM.BIRIMKOD = BHMKURUMSALYAPI.BIRIMKOD AND ";
            tumSorgu += " MUHMIFFORM.KURKOD = BHMKURUMSALYAPI.KURKOD";
            tumSorgu += " where MUHMIF.YIL=" + VeriErisim.DegerYap(kistasTertip.yil + yilFark);
            tumSorgu += " AND MUHMIF.KURKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[0].Replace(".", ""));
            tumSorgu += " AND MUHMIF.FONKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[1].Replace(".", ""));
            tumSorgu += " AND MUHMIF.FINKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[2].Replace(".", ""));
            tumSorgu += " AND MUHMIF.EKOKOD LIKE " + VeriErisim.DegerYap(kistasTertip.kodlar[3].Replace(".", "") + "%");
            tumSorgu += " AND MUHMIF.PEBKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[4].Replace(".", ""));
            tumSorgu += " AND BHMKURUMSALYAPI.KOD LIKE " + VeriErisim.DegerYap(kyKod.Replace(".", "") + "%");
            tumSorgu += " AND MUHMIFFORM.FORMDURUM < 9 AND (MUHMIF.HESAPKOD LIKE '830%')";
            tumSorgu += " group by " + alanAd + " ORDER BY 1";
            return tumSorgu;
        }

        public static string OdenekGondermeBul(Tertip kistasTertip, int yilFark, int kodUzunluk, string kyKod)
        {
            string alanAd = "LEFT(BHMKURUMSALYAPI.KOD, " + kodUzunluk.ToString() + ")";
            string tumSorgu = "SELECT " + alanAd + ", SUM(MUHGONDERMETENKIS.MIKTAR) FROM MUHGONDERMETENKIS INNER JOIN";
            tumSorgu += " MUHGONDERMETENKISFORM ON MUHGONDERMETENKIS.YIL = MUHGONDERMETENKISFORM.YIL AND MUHGONDERMETENKIS.BELGETUR = MUHGONDERMETENKISFORM.BELGETUR AND MUHGONDERMETENKIS.BELGENO = MUHGONDERMETENKISFORM.BELGENO INNER JOIN";
            tumSorgu += " BHMKURUMSALYAPI ON MUHGONDERMETENKISFORM.MUHASEBEKOD = BHMKURUMSALYAPI.SAYMANLIKKOD AND MUHGONDERMETENKISFORM.BIRIMKOD = BHMKURUMSALYAPI.BIRIMKOD AND ";
            tumSorgu += " MUHGONDERMETENKIS.KURKOD = BHMKURUMSALYAPI.KURKOD";
            tumSorgu += " where MUHGONDERMETENKIS.YIL=" + VeriErisim.DegerYap(kistasTertip.yil + yilFark);
            tumSorgu += " AND MUHGONDERMETENKIS.KURKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[0].Replace(".", ""));
            tumSorgu += " AND MUHGONDERMETENKIS.FONKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[1].Replace(".", ""));
            tumSorgu += " AND MUHGONDERMETENKIS.FINKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[2].Replace(".", ""));
            tumSorgu += " AND MUHGONDERMETENKIS.EKOKOD LIKE " + VeriErisim.DegerYap(kistasTertip.kodlar[3].Replace(".", "") + "%");
            tumSorgu += " AND MUHGONDERMETENKIS.PEBKOD = " + VeriErisim.DegerYap(kistasTertip.kodlar[4].Replace(".", ""));
            tumSorgu += " AND BHMKURUMSALYAPI.KOD LIKE " + VeriErisim.DegerYap(kyKod.Replace(".", "") + "%");
            tumSorgu += " AND MUHGONDERMETENKISFORM.FORMDURUM < 9";
            tumSorgu += " group by " + alanAd + " ORDER BY 1";
            return tumSorgu;
        }

        public static string ProjeOdenekBulEko2SektorPeb2Sql(int yil, string senaryoKod, int teklifYil, string ekoKod, string sektorKod, string pebBirimKod)
        {
            string sektorAlan = "case when SEKTORNO IS NULL then '00' else SEKTORNO end";

            string odenekSorgu = "SELECT LEFT(O.EKOKOD,2), " + sektorAlan + ", LEFT(O.PEBKOD,2), SUM(O.ODENEK)";
            odenekSorgu += " FROM Yatirim.dbo.YATALTPROJE AS AP LEFT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON " + VeriErisim.Join("P", "AP", new string[] { "YIL", "SENARYOKOD", "PROJENO" }); ;
            odenekSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAK AS K ON " + VeriErisim.Join("AP", "K", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" }); ;
            odenekSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS BK ON " + VeriErisim.Join("K", "BK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            odenekSorgu += " RIGHT OUTER JOIN BHMODENEK AS O ON " + VeriErisim.Join("BK", "O", new string[] { "YIL", "SENARYOKOD", "PEBKOD" });
            odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "WHERE", VeriErisim.KosulOl("O", "YIL", "=", yil));
            odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "SENARYOKOD", "=", senaryoKod));
            odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "TEKLIFYIL", "=", teklifYil));
            if (ekoKod != null)
                odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "EKOKOD", "LIKE", ekoKod.Replace(".", "") + "%"));

            if (sektorKod != null)
            {
                if (sektorKod == "00")
                    odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", "SEKTORNO IS NULL");
                else
                    odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("", "SEKTORNO", "=", sektorKod.Replace(".", "")));
            }

            if (pebBirimKod != null)
                odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "PEBKOD", "LIKE", pebBirimKod.Replace(".", "") + "%"));

            odenekSorgu += " group by LEFT(O.EKOKOD,2), SEKTORNO, LEFT(O.PEBKOD,2) order by 1,2,3";

            return odenekSorgu;
        }

        public static string ProjeOdenekBulEko2SektorPeb2Peb4Sql(int yil, string senaryoKod, int teklifYil, string ekoKod, string sektorKod, string pebKod)
        {
            string sektorAlan = "case when SEKTORNO IS NULL then '00' else SEKTORNO end";

            string odenekSorgu = "SELECT LEFT(O.EKOKOD,2), " + sektorAlan + ", LEFT(O.PEBKOD,2), O.PEBKOD, SUM(O.ODENEK)";
            odenekSorgu += " FROM Yatirim.dbo.YATALTPROJE AS AP LEFT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON " + VeriErisim.Join("P", "AP", new string[] { "YIL", "SENARYOKOD", "PROJENO" }); ;
            odenekSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAK AS K ON " + VeriErisim.Join("AP", "K", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" }); ;
            odenekSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS BK ON " + VeriErisim.Join("K", "BK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            odenekSorgu += " RIGHT OUTER JOIN BHMODENEK AS O ON " + VeriErisim.Join("BK", "O", new string[] { "YIL", "SENARYOKOD", "PEBKOD" });
            odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "WHERE", VeriErisim.KosulOl("O", "YIL", "=", yil));
            odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "SENARYOKOD", "=", senaryoKod));
            odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "TEKLIFYIL", "=", teklifYil));
            if (ekoKod != null)
                odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "EKOKOD", "LIKE", ekoKod.Replace(".", "") + "%"));

            if (sektorKod != null)
            {
                if (sektorKod == "00")
                    odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", "SEKTORNO IS NULL");
                else
                    odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("", "SEKTORNO", "=", sektorKod.Replace(".", "")));
            }

            if (pebKod != null)
                odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "PEBKOD", "LIKE", pebKod.Replace(".", "") + "%"));

            odenekSorgu += " group by LEFT(O.EKOKOD,2), SEKTORNO, LEFT(O.PEBKOD,2), O.PEBKOD order by 1,2,3,4";

            return odenekSorgu;
        }

        public static string ProjeOdenekBulPeb2Eko2FinPeb4Sql(int yil, string senaryoKod, int teklifYil, string ekoKod, string finKod, string pebKod, int projeKod)
        {
            string alanlar = "LEFT(O.PEBKOD,2), LEFT(O.EKOKOD,2), O.FINKOD, O.PEBKOD";

            string odenekSorgu = "SELECT " + alanlar + ", SUM(O.ODENEK)";
            odenekSorgu += " FROM Yatirim.dbo.YATALTPROJE AS AP LEFT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON " + VeriErisim.Join("P", "AP", new string[] { "YIL", "SENARYOKOD", "PROJENO" }); ;
            odenekSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAK AS K ON " + VeriErisim.Join("AP", "K", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" }); ;
            odenekSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS BK ON " + VeriErisim.Join("K", "BK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            odenekSorgu += " RIGHT OUTER JOIN BHMODENEK AS O ON " + VeriErisim.Join("BK", "O", new string[] { "YIL", "SENARYOKOD", "PEBKOD" });

            odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "WHERE", VeriErisim.KosulOl("O", "YIL", "=", yil));
            odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "SENARYOKOD", "=", senaryoKod));
            odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "TEKLIFYIL", "=", teklifYil));

            if (ekoKod != null)
                odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "EKOKOD", "LIKE", ekoKod.Replace(".", "") + "%"));

            if (pebKod != null)
                odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "PEBKOD", "LIKE", pebKod.Replace(".", "") + "%"));

            if (finKod != null)
                odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "FINKOD", "LIKE", finKod.Replace(".", "") + "%"));

            if (projeKod > 0)
                odenekSorgu += " AND P.PROJENO=" + projeKod.ToString();

            odenekSorgu += " group by " + alanlar + " order by 1,2,3,4";
            return odenekSorgu;
        }

        public static string ProjeOdenekRaporSql(bool detay, int yil, string senaryoKod, int teklifYil, string ekoKod, string finKod, string pebKod)
        {
            string ekAlan2;
            if (detay)
                ekAlan2 = "LEFT(O.EKOKOD,2), O.FINKOD, ";
            else
                ekAlan2 = "";

            string sorguPeb2 = "";
            string sorguEko2 = "";
            string sorguFin2 = "";
            if (!string.IsNullOrWhiteSpace(pebKod))
                sorguPeb2 = " AND O.PEBKOD LIKE " + VeriErisim.DegerYap(pebKod.Replace(".", "") + "%");
            if (!string.IsNullOrWhiteSpace(ekoKod))
                sorguEko2 = " AND O.EKOKOD LIKE " + VeriErisim.DegerYap(ekoKod.Replace(".", "") + "%");
            if (!string.IsNullOrWhiteSpace(finKod))
                sorguFin2 = " AND O.FINKOD LIKE " + VeriErisim.DegerYap(finKod.Replace(".", "") + "%");

            string odenekSorgu = "SELECT P.SEKTORNO," + ekAlan2 + " P.DPTPROJENO, O.PEBKOD, SEK.AD, P.AD, PEBKOD.PEBAD, SUM(O.ODENEK) ";
            odenekSorgu += " FROM BHMODENEK AS O INNER JOIN ButceHazirlik.dbo.ABSPEBKOD AS PEBKOD ON PEBKOD.YIL = O.YIL AND PEBKOD.PEBKOD = O.PEBKOD ";
            odenekSorgu += " RIGHT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS BK ON O.YIL = BK.YIL AND O.PEBKOD = BK.PEBKOD AND O.SENARYOKOD=BK.SENARYOKOD ";
            odenekSorgu += " RIGHT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAK AS K ON " + VeriErisim.Join("BK", "K", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            odenekSorgu += " RIGHT OUTER JOIN Yatirim.dbo.YATALTPROJE AS AP ON " + VeriErisim.Join("K", "AP", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            odenekSorgu += " RIGHT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON " + VeriErisim.Join("AP", "P", new string[] { "YIL", "SENARYOKOD", "PROJENO" });
            odenekSorgu += " RIGHT OUTER JOIN Yatirim.dbo.YATSEKTOR AS SEK ON " + VeriErisim.Join("P", "SEK", new string[] { "YIL", "SEKTORNO" });
            odenekSorgu += " WHERE (O.YIL = " + VeriErisim.DegerYap(yil) + ") AND (O.SENARYOKOD = " + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + ") AND (O.TEKLIFYIL = " + teklifYil.ToString() + ")" + sorguPeb2 + sorguEko2 + sorguFin2 + "  GROUP BY P.SEKTORNO, " + ekAlan2 + "P.DPTPROJENO, O.PEBKOD, SEK.AD, P.AD, PEBKOD.PEBAD ORDER BY P.SEKTORNO, " + ekAlan2 + "P.DPTPROJENO, O.PEBKOD";
            return odenekSorgu;
        }

        public static string ProjeHarcamaBulEko2SektorPeb2Sql(int yil, string ekoKod, string sektorKod, string pebBirimKod)
        {
            string sektorAlan = "case when SEKTORNO IS NULL then '00' else SEKTORNO end";

            string harcamaSorgu = "SELECT LEFT(O.EKOKOD,2), " + sektorAlan + ", LEFT(O.PEBKOD,2), SUM(O.HARCAMA)";
            harcamaSorgu += " FROM Yatirim.dbo.YATALTPROJE AS AP LEFT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON " + VeriErisim.Join("P", "AP", new string[] { "YIL", "SENARYOKOD", "PROJENO" }); ;
            harcamaSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAK AS K ON " + VeriErisim.Join("AP", "K", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" }); ;
            harcamaSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS BK ON " + VeriErisim.Join("K", "BK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            harcamaSorgu += " RIGHT OUTER JOIN ABSHARCAMA AS O ON " + VeriErisim.Join("BK", "O", new string[] { "YIL", "PEBKOD" }) + " AND BK.SENARYOKOD='0600'";
            harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "WHERE", VeriErisim.KosulOl("O", "YIL", "=", yil));
            if (ekoKod != null)
                harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "AND", VeriErisim.KosulOl("O", "EKOKOD", "LIKE", ekoKod.Replace(".", "") + "%"));

            if (sektorKod != null)
            {
                if (sektorKod == "00")
                    harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "AND", "SEKTORNO IS NULL");
                else
                    harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "AND", VeriErisim.KosulOl("", "SEKTORNO", "=", sektorKod.Replace(".", "")));
            }

            if (pebBirimKod != null)
                harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "AND", VeriErisim.KosulOl("O", "PEBKOD", "LIKE", pebBirimKod.Replace(".", "") + "%"));

            harcamaSorgu += " group by LEFT(O.EKOKOD,2), SEKTORNO, LEFT(O.PEBKOD,2) order by 1,2,3";
            return harcamaSorgu;
        }

        public static string ProjeHarcamaBulEko2SektorPeb2Peb4Sql(int yil, string ekoKod, string sektorKod, string pebKod)
        {
            string sektorAlan = "case when SEKTORNO IS NULL then '00' else SEKTORNO end";

            string harcamaSorgu = "SELECT LEFT(O.EKOKOD,2), " + sektorAlan + ", LEFT(O.PEBKOD,2), O.PEBKOD, SUM(O.HARCAMA)";
            harcamaSorgu += " FROM Yatirim.dbo.YATALTPROJE AS AP LEFT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON " + VeriErisim.Join("P", "AP", new string[] { "YIL", "SENARYOKOD", "PROJENO" });
            harcamaSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAK AS K ON " + VeriErisim.Join("AP", "K", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" }); ;
            harcamaSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS BK ON " + VeriErisim.Join("K", "BK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            harcamaSorgu += " RIGHT OUTER JOIN ABSHARCAMA AS O ON " + VeriErisim.Join("BK", "O", new string[] { "YIL", "PEBKOD" }) + " AND BK.SENARYOKOD='0600'";
            harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "WHERE", VeriErisim.KosulOl("O", "YIL", "=", yil));
            if (ekoKod != null)
                harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "AND", VeriErisim.KosulOl("O", "EKOKOD", "LIKE", ekoKod.Replace(".", "") + "%"));

            if (sektorKod != null)
            {
                if (sektorKod == "00")
                    harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "AND", "SEKTORNO IS NULL");
                else
                    harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "AND", VeriErisim.KosulOl("", "SEKTORNO", "=", sektorKod.Replace(".", "")));
            }

            if (pebKod != null)
                harcamaSorgu = VeriErisim.KosulEkle(harcamaSorgu, "AND", VeriErisim.KosulOl("O", "PEBKOD", "LIKE", pebKod.Replace(".", "") + "%"));

            harcamaSorgu += " group by LEFT(O.EKOKOD,2), SEKTORNO, LEFT(O.PEBKOD,2), O.PEBKOD order by 1,2,3,4";
            return harcamaSorgu;
        }

        public static string ProjeHarcamaBulPeb2Eko2FinPeb4Sql(int yil, string ekoKod, string finKod, string pebKod, int projeKod)
        {
            string alanlar = "LEFT(O.PEBKOD,2), LEFT(O.EKOKOD,2), O.FINKOD, O.PEBKOD";

            string odenekSorgu = "SELECT " + alanlar + ", SUM(O.HARCAMA)";
            odenekSorgu += " FROM Yatirim.dbo.YATALTPROJE AS AP LEFT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON " + VeriErisim.Join("P", "AP", new string[] { "YIL", "SENARYOKOD", "PROJENO" }); ;
            odenekSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAK AS K ON " + VeriErisim.Join("AP", "K", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" }); ;
            odenekSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS BK ON " + VeriErisim.Join("K", "BK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            odenekSorgu += " RIGHT OUTER JOIN ABSHARCAMA AS O ON " + VeriErisim.Join("BK", "O", new string[] { "YIL", "PEBKOD" }) + " AND BK.SENARYOKOD='0600'";

            odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "WHERE", VeriErisim.KosulOl("O", "YIL", "=", yil));

            if (ekoKod != null)
                odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "EKOKOD", "LIKE", ekoKod.Replace(".", "") + "%"));

            if (pebKod != null)
                odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "PEBKOD", "LIKE", pebKod.Replace(".", "") + "%"));

            if (finKod != null)
                odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "FINKOD", "LIKE", finKod.Replace(".", "") + "%"));

            if (projeKod > 0)
                odenekSorgu += " AND P.PROJENO=" + projeKod.ToString();

            odenekSorgu += " group by " + alanlar + " order by 1,2,3,4";
            return odenekSorgu;
        }

        public static string PebKodBilgiSql(string alan1, string alan2, string alan3, int yil, string senaryoKod, string sektorKod, string pebKod, int projeNo, int ekAlanNo)
        {
            string sql = "SELECT " + VeriErisim.DegerYap(alan1.Replace(".", "")) + "," + VeriErisim.DegerYap(alan2.Replace(".", "")) + "," + VeriErisim.DegerYap(alan3.Replace(".", "")) + ", P.PEBKOD, P.PEBAD" + EkAlanBul(ekAlanNo);
            sql += " FROM Yatirim.dbo.YATPROJE AS PR RIGHT OUTER JOIN Yatirim.dbo.YATALTPROJE AS AP ON " + VeriErisim.Join("PR", "AP", new string[] { "YIL", "SENARYOKOD", "PROJENO" }); ;
            sql += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAK AS K ON " + VeriErisim.Join("AP", "K", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" }); ;
            sql += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS BK ON " + VeriErisim.Join("K", "BK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sql += " RIGHT OUTER JOIN ABSPEBKOD AS P ON " + VeriErisim.Join("BK", "P", new string[] { "YIL", "PEBKOD" }) + " AND BK.SENARYOKOD=" + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "WHERE (P.YIL = " + VeriErisim.DegerYap(yil) + ") AND (LEN(P.PEBKOD) = 8) AND (P.PEBKOD LIKE " + VeriErisim.DegerYap(pebKod + "%") + ") ";

            if (sektorKod != null)
            {
                string sektorKosul;
                if (sektorKod == "00")
                    sektorKosul = " IS NULL";
                else
                    sektorKosul = " LIKE " + VeriErisim.DegerYap(sektorKod.Replace(".", "") + "%");

                sql += " AND (PR.SEKTORNO " + sektorKosul + ")";
            }
            if (projeNo > 0)
                sql += " AND (PR.PROJENO = " + projeNo.ToString() + ")";

            return sql;
        }

        private static string EkAlanBul(int ekAlanNo)
        {
            if (ekAlanNo >= 1) return ", PR.AD, PR.BITISYILI, PR.DPTPROJENO";
            return "";
        }

        public IDataReader TavanUstuBul(int yil, System.Collections.Hashtable tavanlar, bool tavanZorunlu)
        {
            string tertipler = "";
            foreach (System.Collections.DictionaryEntry item in tavanlar)
            {
                if (tertipler != "") tertipler += ", ";
                tertipler += VeriErisim.DegerYap((string)item.Key);
            }
            string s1 = "(SELECT REPLACE(TERTIPSABLON,'.','') TERTIP, ODENEK TAVAN FROM ButceHazirlik.dbo.BHMBUTCETASNIFODENEK O INNER JOIN ButceHazirlik.dbo.BHMBUTCETASNIFTERTIP T ON O.TASNIFNO=T.TASNIFNO AND O.HIYERARSIKOD=T.HIYERARSIKOD where YIL=" + VeriErisim.DegerYap(yil) + " and O.TASNIFNO=3 and SENARYOKOD='0000' and REPLACE(TERTIPSABLON,'.','') IN (" + tertipler + ")) TAVAN";
            string s2 = "(SELECT KURKOD+'---'+LEFT(EKOKOD,2) TERTIP, SUM(ODENEK) ODENEK FROM BHMODENEK WHERE YIL=" + VeriErisim.DegerYap(yil) + " AND SENARYOKOD='0000' AND TEKLIFYIL=0 AND KURKOD+'---'+LEFT(EKOKOD,2) IN (" + tertipler + ") group BY KURKOD+'---'+LEFT(EKOKOD,2)) ODENEK";
            string tavanSifirStr;
            if (!tavanZorunlu)
                tavanSifirStr = "ISNULL(TAVAN,0)>0 AND";
            else
                tavanSifirStr = "";
            string sql = "SELECT TAVAN.TERTIP, TAVAN, ODENEK FROM " + s1 + " FULL OUTER JOIN " + s2 + $" ON ODENEK.TERTIP=TAVAN.TERTIP where {tavanSifirStr} ISNULL(ODENEK,0)>ISNULL(TAVAN,0) order by 1";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OdenekGondermeKontrolMiktarlariniBul(int yil, TertipKodlari kodlar)
        {
            string sql = @"select SERBEST, (KBO+EKLENEN-DUSULEN) odenekSonDurum
, (SELECT sum(MIKTAR) 
FROM ButceHazirlikV2.dbo.BHMODENEKBLOKE 
where YIL= '" + yil + "' and KURKOD = '" + kodlar.kurKod + "' and EKOKOD like '" + kodlar.ekoKod + "%' and FINKOD = '" + kodlar.finKod + "' and FONKOD = '" + kodlar.fonKod + "'" +
" group by YIL, KURKOD, FONKOD, FINKOD, substring(EKOKOD,0,4) ) blokeMiktari from ButceIslemleri.dbo.ABSBUTCEGIDER where " +
" YIL= '" + yil + "' and KURKOD = '" + kodlar.kurKod + "' and EKOKOD = '" + kodlar.ekoKod + "' and FINKOD = '" + kodlar.finKod + "' and FONKOD = '" + kodlar.fonKod + "'";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ProgramButceCevrimOku(int yil)
        {
            string sql = $"SELECT 14 as KODNO, PEBKOD as ABSKOD, CONVERT(varchar, ALTFAALKOD) PRGKOD FROM ButceHazirlik.dbo.PRGPEBILISKI where YIL={VeriErisim.DegerYap(yil)} "
             + " UNION "
             + $" SELECT 140 as KODNO, PEBKOD as ABSKOD, CONVERT(varchar, PRGKOD) + '.' + CONVERT(varchar, ALTPRGKOD) + '.' + CONVERT(varchar, FAALKOD) + '.' + CONVERT(varchar, ALTFAALKOD) PRGKOD FROM ButceHazirlik.dbo.PRGPEBILISKI where YIL ={VeriErisim.DegerYap(yil)} "
             + " UNION "
             + $" SELECT 1200 as KODNO, CONVERT(varchar, ALTFAALKOD) as ABSKOD, PEBKOD + '.' + FONKOD PRGKOD FROM ButceHazirlik.dbo.PRGPEBILISKI where YIL ={VeriErisim.DegerYap(yil)} ";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
