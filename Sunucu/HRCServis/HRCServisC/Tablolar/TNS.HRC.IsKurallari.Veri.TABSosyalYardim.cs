using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Sosyal Yardım bilgisini tutan PSLSOSYALYARDIM veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSosyalYardim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSosyalYardim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Sosyal Yardım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLSOSYALYARDIM";

        /// <summary>
        /// Sosyal Yardım bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// SIRA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRA = 4,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 5,
            /// <summary>
            /// YAKINTCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKINTCKIMLIKNO = 6,
            /// <summary>
            /// YAKINLIKDERECESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKINLIKDERECESI = 7,
            /// <summary>
            /// OLAYYILI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OLAYYILI = 8,
            /// <summary>
            /// OLAYAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OLAYAYI = 9,
            /// <summary>
            /// AYLIKKATSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AYLIKKATSAYI = 10,
            /// <summary>
            /// KATSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KATSAYI = 11,
            /// <summary>
            /// ENYUKSMEMURMAAS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ENYUKSMEMURMAAS = 12,
            /// <summary>
            /// ORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ORAN = 13,
            /// <summary>
            /// TUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUTAR = 14,
            /// <summary>
            /// OLAYNITELIGI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OLAYNITELIGI = 15,
            /// <summary>
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 16,
            /// <summary>
            /// ADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYAD = 17,
            /// <summary>
            /// YAKINADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKINADSOYAD = 18,
            /// <summary>
            /// UNVANKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVANKODU = 19,
            /// <summary>
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 20,
        }

        /// <summary>
        /// Sosyal Yardım tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "SIRA", "TCKIMLIKNO", "YAKINTCKIMLIKNO", "YAKINLIKDERECESI", "OLAYYILI", "OLAYAYI", "AYLIKKATSAYI", "KATSAYI", "ENYUKSMEMURMAAS", "ORAN", "TUTAR", "OLAYNITELIGI", "TARIH", "ADSOYAD", "YAKINADSOYAD", "UNVANKODU", "ODEMEEMRINO" };

        /// <summary>
        /// Sosyal Yardım bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="list">Eklenecek sosyal yardım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            int sira = 0;
            foreach (PSLSosyalYardimi bilgi in list.objeler)
            {
                sira++;
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(sira) + ",";
                sql += VeriErisim.DegerYap(bilgi.TCKimlikNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.yakinTCKimlikNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.yakinlikDerecesi) + ",";
                sql += VeriErisim.DegerYap(bilgi.olayYili) + ",";
                sql += VeriErisim.DegerYap(bilgi.olayAyi) + ",";
                sql += VeriErisim.DegerYap(bilgi.aylikKatsayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.katsayi) + ",";
                sql += VeriErisim.DegerYap(bilgi.enYuksMemurMaasi) + ",";
                sql += VeriErisim.DegerYap(bilgi.oran) + ",";
                sql += VeriErisim.DegerYap(bilgi.tutar) + ",";
                sql += VeriErisim.DegerYap(bilgi.olayNiteligi) + ",";
                sql += VeriErisim.DegerYap(bilgi.olayTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.adSoyad) + ",";
                sql += VeriErisim.DegerYap(bilgi.yakinAdSoyad) + ",";
                sql += VeriErisim.DegerYap(bilgi.unvanKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.odemeEmriNo) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Sosyal Yardım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak sosyal yardım bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sosyal Yardım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(PSLSosyalYardimi bilgi)
        {
            string sql = "";
            string kosul = "";
            //"," + DBDegiskeni.TABLOADMUHASEBEBIRIM + ".BIRIMAD " + 
            sql = "SELECT DISTINCT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + DBDegiskeni.TABLOADUNVAN + ".AD";
            sql += " FROM " + TABLOAD;

            sql += " INNER JOIN " + DBDegiskeni.TABLOADUNVAN + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKODU] + " = " + DBDegiskeni.TABLOADUNVAN + ".KOD ";
            //" INNER JOIN " + DBDegiskeni.TABLOADMUHASEBEBIRIM + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + " = " + DBDegiskeni.TABLOADMUHASEBEBIRIM + ".BIRIMKOD"; 

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(DBDegiskeni.TABLOADMUHASEBEBIRIM, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));            
            if (bilgi.TCKimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", bilgi.TCKimlikNo));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRA] + " ASC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Sosyal Yardım bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek sosyal yardım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(PSLSosyalYardimi bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            sonuc = KomutCalistir(sql, 0);


            return sonuc;
        }

        /// <summary>
        /// Sosyal Yardım belgesine ait ödeme emri belge numarası bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek sosyal yardım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc OdemeEmriNoKaydet(PSLSosyalYardimi bilgi)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], bilgi.odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Sosyal Yardım belgesine ait ödeme emri belge numarası bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="surecNo">Listeleme için kullanılacak süreç numarası kıstas parametresi</param>
        /// <returns>
        /// Sosyal Yardım belgesine ait ödeme emri belge numarası bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleOdemeEmri(int yil, string surecNo)
        {
            string sql = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "," +
                              TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO] +
                  " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}