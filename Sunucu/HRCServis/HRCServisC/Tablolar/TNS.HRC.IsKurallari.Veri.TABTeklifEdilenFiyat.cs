using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Teklif edilen fiyat bilgisini tutan HRCTEKLIFEDILENFIYAT veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTeklifEdilenFiyat : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTeklifEdilenFiyat(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Teklif edilen fiyat bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCTEKLIFEDILENFIYAT";

        /// <summary>
        /// Teklif edilen fiyat bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// TEKLIFTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFTUTAR = 6,
            /// <summary>
            /// FIYATDISITEKLIFTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIYATDISITEKLIFTUTAR = 7,
            /// <summary>
            /// YERLIMALITEKLIFTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YERLIMALITEKLIFTUTAR = 8,
            /// <summary>
            /// YERLIMALITEKLIFTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAKODU = 9,
            /// <summary>
            /// TEKLIFTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFTUR = 10,
        }

        /// <summary>
        /// Teklif edilen fiyat tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TARIH", "SAAT", "TEKLIFTUTAR", "FIYATDISITEKLIFTUTAR", "YERLIMALITEKLIFTUTAR", "FIRMAKODU", "TEKLIFTUR" };

        /// <summary>
        /// Teklif edilen fiyat bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek teklif edilen fiyat bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (TeklifEdilenFiyat f in bilgi.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.yil) + ",";
                sql += VeriErisim.DegerYap(f.muhasebe) + ",";
                sql += VeriErisim.DegerYap(f.birim) + ",";
                sql += VeriErisim.DegerYap(f.surecNo) + ",";
                sql += VeriErisim.DegerYap(f.tarih) + ",";
                sql += VeriErisim.DegerYap(f.saat) + ",";
                sql += VeriErisim.DegerYap(f.teklifTutari) + ",";
                sql += VeriErisim.DegerYap(f.fiyatDisiTeklifTutari) + ",";
                sql += VeriErisim.DegerYap(f.yerliMaliTeklifTutari) + ",";
                sql += VeriErisim.DegerYap(f.firmaKod) + ",";
                sql += VeriErisim.DegerYap(f.teklifTur) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Teklif vermeyen firma bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak teklif edilen fiyat bilgisi kıstas parametresi</param>
        /// <returns>
        /// Teklif vermeyen firma bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleFiyatTeklifEtmeyenler(TeklifEdilenFiyat bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + TABTeklifDosyasiVeren.TABLOAD + "." + TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.FIRMAKODU] + "," +
                                TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD] +
                    " FROM " + TABTeklifDosyasiVeren.TABLOAD +
                    " LEFT OUTER JOIN " + TABFirmaBilgisi.TABLOAD + " ON " +
                        TABTeklifDosyasiVeren.TABLOAD + "." + TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.FIRMAKODU] + " = " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD];

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifDosyasiVeren.TABLOAD, TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifDosyasiVeren.TABLOAD, TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifDosyasiVeren.TABLOAD, TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifDosyasiVeren.TABLOAD, TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.ihaleAsamasi > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifDosyasiVeren.TABLOAD, TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifDosyasiVeren.TABLOAD, TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.FIRMADEVAM], "=", (int)ENUMEvetHayir.EVET));

            kosul += " AND " + TABTeklifDosyasiVeren.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " NOT IN (SELECT " + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, " WHERE ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo)) + ")";

            if (kosul != "")
                sql += " WHERE " + kosul;


            sql += " ORDER BY " + TABTeklifDosyasiVeren.TABLOAD + "." + TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.FIRMAKODU];

            return veriErisim.DataReaderDondur(sql);

        }

        /// <summary>
        /// Teklif edilen fiyat bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak teklif edilen fiyat bilgisi kıstas parametresi</param>
        /// <returns>
        /// Teklif edilen fiyat bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(TeklifEdilenFiyat bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD] + " FROM " + TABLOAD + " LEFT OUTER JOIN " + TABFirmaBilgisi.TABLOAD + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " = " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD];

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.firmaKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FIRMAKODU], "=", bilgi.firmaKod));
            if (bilgi.teklifTutari != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TEKLIFTUTAR], "<", bilgi.teklifTutari));
            if (bilgi.teklifTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TEKLIFTUR], "=", bilgi.teklifTur));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TEKLIFTUR] + " , " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " ASC ";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Teklif edilen fiyat bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek teklif edilen fiyat bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TeklifEdilenFiyat bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// İhale teklif dosyası olmayan firma bilgisini veri tabanından silmek için
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

            kosul += " AND " + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " NOT IN (SELECT " + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " FROM " + TABIhaleDosyasiAlan.TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, " WHERE ", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.POSTADAGECIKTIMI], "=", (int)ENUMEvetHayir.HAYIR));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.GELISSEKLI], "!=", (int)ENUMTeklifGelisTuru.GELMEDI)) + ")";


            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABTeklifDosyasiVeren.ALANLAR[(int)TABTeklifDosyasiVeren.ENUMAlanlar.TEKLIFDOSYASIVERDIMI], "=", (int)ENUMTeklifDosyasi.VERDI));
            sql += kosul;
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Zarf kontrolünü geçemeyen firma bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek teklif zarf kontrol bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilZarfKontroluGecemeyen(TeklifZarfKontroluBaslik bilgi)
        {
            string sql = "";
            string kosul = "";
            /*Eğer firmakodu ihale dosyasi alanların tablosunda  yok ise bunu zarf kontrolunun yapıldığı tablodan siler*/
            sql = "DELETE FROM " + TABLOAD + " WHERE ";

            kosul = VeriErisim.KosulEkle(kosul, "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            kosul += " AND " + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " NOT IN (SELECT " + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " FROM " + TABTeklifZarfKontrolu.TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, " WHERE ", VeriErisim.KosulOl("", TABTeklifZarfKontrolu.ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABTeklifZarfKontrolu.ALANLAR[(int)TABTeklifZarfKontrolu.ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABTeklifZarfKontrolu.ALANLAR[(int)TABTeklifZarfKontrolu.ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABTeklifZarfKontrolu.ALANLAR[(int)TABTeklifZarfKontrolu.ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABTeklifZarfKontrolu.ALANLAR[(int)TABTeklifZarfKontrolu.ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABTeklifZarfKontrolu.ALANLAR[(int)TABTeklifZarfKontrolu.ENUMAlanlar.UYGUNMU], "=", (int)ENUMUygunMu.UYGUN)) + ")";


            sql += kosul;
            return KomutCalistir(sql, 0);

        }
    }
}