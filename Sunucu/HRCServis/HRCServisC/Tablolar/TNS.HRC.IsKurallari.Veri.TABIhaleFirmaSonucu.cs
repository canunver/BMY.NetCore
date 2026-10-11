using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// İhale Firma Sonucu bilgisini tutan HRCIHALEFIRMASONUCU veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIhaleFirmaSonucu : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIhaleFirmaSonucu(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İhale Firma Sonucu bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCIHALEFIRMASONUCU";

        /// <summary>
        /// İhale Firma Sonucu bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// FIRMAKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAKODU = 4,
            /// <summary>
            /// ONAYTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONAYTARIHI = 5,
            /// <summary>
            /// GIDISTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GIDISTARIHI = 6,
            /// <summary>
            /// GIDISSEKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GIDISSEKLI = 7,
            /// <summary>
            /// GEREKCE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 8,
        }

        /// <summary>
        /// İhale Firma Sonucu bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "FIRMAKODU", "ONAYTARIHI", "GIDISTARIHI", "GIDISSEKLI", "GEREKCE" };

        /// <summary>
        /// İhale Firma Sonucu bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek İhale Firma Sonucu bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (IhaleFirmaSonucu f in bilgi.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.yil) + ",";
                sql += VeriErisim.DegerYap(f.muhasebe) + ",";
                sql += VeriErisim.DegerYap(f.birim) + ",";
                sql += VeriErisim.DegerYap(f.surecNo) + ",";
                sql += VeriErisim.DegerYap(f.firmaKod) + ",";
                sql += VeriErisim.DegerYap(f.onayTarihi) + ",";
                sql += VeriErisim.DegerYap(f.gidisTarihi) + ",";
                sql += VeriErisim.DegerYap(f.gidisSekli) + ",";
                sql += VeriErisim.DegerYap(f.gerekce) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// İhaleye katılıp henüz teklif vermeyen firma bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak ihale firma sonucu bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale Firma Sonucu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleSonraTeklifVerenler(IhaleFirmaSonucu bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + TABTeklifEdilenFiyat.TABLOAD + "." + TABTeklifEdilenFiyat.ALANLAR[(int)TABTeklifEdilenFiyat.ENUMAlanlar.FIRMAKODU] + "," + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD] + " FROM " + TABTeklifEdilenFiyat.TABLOAD + " LEFT OUTER JOIN " + TABFirmaBilgisi.TABLOAD + " ON " + TABTeklifEdilenFiyat.TABLOAD + "." + TABTeklifEdilenFiyat.ALANLAR[(int)TABTeklifEdilenFiyat.ENUMAlanlar.FIRMAKODU] + " = " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD];
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifEdilenFiyat.TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifEdilenFiyat.TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifEdilenFiyat.TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifEdilenFiyat.TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.teklifTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifEdilenFiyat.TABLOAD, TABTeklifEdilenFiyat.ALANLAR[(int)TABTeklifEdilenFiyat.ENUMAlanlar.TEKLIFTUR], "=", bilgi.teklifTur));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABTeklifEdilenFiyat.TABLOAD, TABTeklifEdilenFiyat.ALANLAR[(int)TABTeklifEdilenFiyat.ENUMAlanlar.TEKLIFTUTAR], "<>", 0.0));

            kosul += " AND " + TABTeklifEdilenFiyat.TABLOAD + "." + TABTeklifEdilenFiyat.ALANLAR[(int)TABTeklifEdilenFiyat.ENUMAlanlar.FIRMAKODU] + " NOT IN (SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, " WHERE ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo)) + ")";

            if (kosul != "")
                sql += " WHERE " + kosul;


            sql += " ORDER BY " + TABTeklifEdilenFiyat.TABLOAD + "." + TABTeklifEdilenFiyat.ALANLAR[(int)TABTeklifEdilenFiyat.ENUMAlanlar.FIRMAKODU] + " ASC";

            return veriErisim.DataReaderDondur(sql);

        }

        /// <summary>
        /// İhale Firma Sonucu bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak ihale firma sonucu bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale Firma Sonucu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(IhaleFirmaSonucu bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + 
                TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD] + 
                " FROM " + TABLOAD + 
                " LEFT OUTER JOIN " + TABFirmaBilgisi.TABLOAD + " ON " + 
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " = " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD];

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.firmaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FIRMAKODU], "=", bilgi.firmaKod));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " DESC ";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İhaleye teklif vermiş olan firma bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak ihale firma sonucu bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhaleye teklif vermiş olan firma bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleTeklifVermisOlanlar(IhaleFirmaSonucu bilgi)
        {
            string sql = "";
            string kosul = "";

            string firmaSql = "";
            string firmaKosul = "";

            firmaSql = "SELECT FIRMAKODU FROM HRCTEKLIFEDILENFIYAT ";
            if (bilgi.yil > 0)
                firmaKosul = VeriErisim.KosulEkle(firmaKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                firmaKosul = VeriErisim.KosulEkle(firmaKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                firmaKosul = VeriErisim.KosulEkle(firmaKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                firmaKosul = VeriErisim.KosulEkle(firmaKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            if (firmaKosul != "")
                firmaSql += " WHERE " + firmaKosul;


            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD] + 
                " FROM " + TABLOAD + 
                " LEFT OUTER JOIN " + TABFirmaBilgisi.TABLOAD + " ON " + 
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " = " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD];

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            kosul += " AND " + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " IN " + "(" + firmaSql + ")";

            if (kosul != "")
                sql += " WHERE " + kosul;

            //sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " DESC ";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İhale Firma Sonucu bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek ihale firma sonucu bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(IhaleFirmaSonucu bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            return KomutCalistir(sql, 0);
        }
    }
}