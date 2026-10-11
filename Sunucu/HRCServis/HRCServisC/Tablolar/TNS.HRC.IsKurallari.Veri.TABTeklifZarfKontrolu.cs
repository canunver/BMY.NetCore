using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Teklif zarf kontrolü bilgisini tutan HRCTEKLIFZARFKONTROLU veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTeklifZarfKontrolu : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTeklifZarfKontrolu(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Teklif zarf kontrolü bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCTEKLIFZARFKONTROLU";

        /// <summary>
        /// Teklif zarf kontrolü bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
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
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 4,
            /// <summary>
            /// SAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAAT = 5,
            /// <summary>
            /// FIRMAKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAKODU = 6,
            /// <summary>
            /// UYGUNMU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UYGUNMU = 7,
            /// <summary>
            /// DEGERLENDIRME alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DEGERLENDIRME = 8,
            /// <summary>
            /// IHALEASAMASI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEASAMASI = 9,
        }

        /// <summary>
        /// Teklif zarf kontrolü tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TARIH", "SAAT", "FIRMAKODU", "UYGUNMU", "DEGERLENDIRME", "IHALEASAMASI" };

        /// <summary>
        /// Teklif zarf kontrolü bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek teklif zarf kontrolü bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(TeklifZarfKontroluBaslik bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (TeklifZarfKontroluSatir f in bilgi.satirlar.objeler)
            {

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.tarih) + ",";
                sql += VeriErisim.DegerYap(bilgi.saat) + ",";
                sql += VeriErisim.DegerYap(f.firmaKodu) + ",";
                sql += VeriErisim.DegerYap(f.uygunmu) + ",";
                sql += VeriErisim.DegerYap(f.degerlendirme) + ",";
                sql += VeriErisim.DegerYap(bilgi.ihaleAsamasi) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Teklif zarf kontrolü bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak teklif zarf kontrolü bilgisi kıstas parametresi</param>
        /// <returns>
        /// Teklif zarf kontrolü bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>       
        public IDataReader Listele(TeklifZarfKontroluBaslik bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD] + " FROM " + TABLOAD + " LEFT OUTER JOIN " + TABFirmaBilgisi.TABLOAD + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " = " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD];

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));

            if (bilgi.satirlar.objeler.Count > 0)
            {
                TeklifZarfKontroluSatir satir = new TeklifZarfKontroluSatir();
                satir = (TeklifZarfKontroluSatir)bilgi.satirlar.objeler[0];

                if (satir.firmaKodu != 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FIRMAKODU], "=", satir.firmaKodu));
                //if (satir.uygunmu != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.UYGUNMU], "=", satir.uygunmu));


            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Teklif zarf kontrolü bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak teklif zarf kontrolü bilgisi kıstas parametresi</param>
        /// <returns>
        /// Teklif zarf kontrolü bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleTeklifDosyasıKontrolEdilmeyenler(TeklifZarfKontroluBaslik bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + TABTeklifDosyasiVeren.TABLOAD + "." + TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.FIRMAKODU] + "," + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD] + " FROM " + TABTeklifDosyasiVeren.TABLOAD + " LEFT OUTER JOIN " + TABFirmaBilgisi.TABLOAD + " ON " + TABTeklifDosyasiVeren.TABLOAD + "." + TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.FIRMAKODU] + " = " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD];

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifDosyasiVeren.TABLOAD, TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifDosyasiVeren.TABLOAD, TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifDosyasiVeren.TABLOAD, TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifDosyasiVeren.TABLOAD, TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifDosyasiVeren.TABLOAD, TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.TEKLIFDOSYASIVERDIMI], "=", (int)ENUMTeklifDosyasi.VERDI));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.FIRMADEVAM], "=", (int)ENUMEvetHayir.EVET));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.POSTADAGECIKTIMI], "=", (int)ENUMEvetHayir.HAYIR));


            kosul += " AND " + TABTeklifDosyasiVeren.TABLOAD + "." + TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.FIRMAKODU] + " NOT IN (SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, " WHERE ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi)) + ")";

            if (kosul != "")
                sql += " WHERE " + kosul;


            sql += " ORDER BY " + TABTeklifDosyasiVeren.TABLOAD + "." + TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.FIRMAKODU];

            return veriErisim.DataReaderDondur(sql);

        }

        /// <summary>
        /// Teklif dosyası olmayan firma bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek ihale teklif dosyası bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilIhaleTeklifDosyasiOlmayan(IhaleTeklifDosyasi bilgi)
        {
            string sql = "";
            string kosul = "";
            /*Eğer firmakodu ihale dosyasi alanların tablosunda  yok ise bunu zarf kontrolunun yapıldığı tablodan siler*/
            sql = "DELETE FROM " + TABLOAD + " WHERE ";

            kosul = VeriErisim.KosulEkle(kosul, "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));

            kosul += " AND " + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " NOT IN (SELECT " + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " FROM " + TABIhaleDosyasiAlan.TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, " WHERE ", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.POSTADAGECIKTIMI], "=", (int)ENUMEvetHayir.HAYIR));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.GELISSEKLI], "!=", (int)ENUMTeklifGelisTuru.GELMEDI)) + ")";


            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.TEKLIFDOSYASIVERDIMI], "=", (int)ENUMTeklifDosyasi.VERDI));
            sql += kosul;
            return KomutCalistir(sql, 0);

        }

        /// <summary>
        /// Teklif zarf kontrolü bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek teklif zarf kontrolü bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TeklifZarfKontroluBaslik bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));

            return KomutCalistir(sql, 0);
        }
    }
}