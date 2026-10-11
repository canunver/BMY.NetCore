using System;
using System.Data;
using TNS.HRC.IsKurallari;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Ek Ders bilgisini tutan PSLEKDERS ve PSLEKDERSMIF veri tabanı tabloları ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABEkDers : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABEkDers(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Ek Ders tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLEKDERS";

        /// <summary>
        /// Personel tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONEL = "HRCPERSONEL";

        /// <summary>
        /// Ek Ders Muhasebe İşlem Fişi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADMIF = "PSLEKDERSMIF";

        /// <summary>
        /// Muhasebe Birim tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADMUHBIR = "MUHASEBEBIRIM";

        /// <summary>
        /// Ek Ders Personel Puantaj tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONELPUANTAJ = "PSLEKDERSPERSONELPUANTAJ";

        /// <summary>
        /// Öğrenci Eğitmen tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADTANIM = "HRCOGRENCIEGITMEN";

        /// <summary>
        /// Ek Ders tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar : int
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
            /// ILGILIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILGILIYIL = 4,
            /// <summary>
            /// ILGILIAY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILGILIAY = 5,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 6,
            /// <summary>
            /// GUN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUN = 7,
            /// <summary>
            /// SAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAAT = 8,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 9,
        }

        /// <summary>
        /// Ek Ders tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ILGILIYIL", "ILGILIAY", "TCKIMLIKNO", "GUN", "SAAT", "SIRANO" };

        /// <summary>
        /// Personel tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarPersonel : int
        {
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 0,
            /// <summary>
            /// ADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYAD = 1,
            /// <summary>
            /// UNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVAN = 2,
            /// <summary>
            /// MUHASEBEKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKODU = 3,
            /// <summary>
            /// BIRIMKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKODU = 4,
            /// <summary>
            /// ISTIHDAMSEKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISTIHDAMSEKLI = 5
        }

        /// <summary>
        /// Personel tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPERSONEL = { "TCKIMLIKNO", "ADSOYAD", "UNVAN", "MUHASEBEKODU", "BIRIMKODU", "ISTIHDAMSEKLI" };

        /// <summary>
        /// Ek Ders Personel Puantaj tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarPersonelPuantaj : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD
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
            /// ILGILIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILGILIYIL = 4,
            /// <summary>
            /// ILGILIAY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILGILIAY = 5,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 6,
            /// <summary>
            /// PUANTAJKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PUANTAJKODU = 7
        }

        /// <summary>
        /// Ek Ders Personel Puantaj tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPERSONELPUANTAJ = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ILGILIYIL", "ILGILIAY", "TCKIMLIKNO", "PUANTAJKODU" };

        /// <summary>
        /// Ek Ders Muhasebe İşlem Fişi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarMIF
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
            /// BELGENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGENO = 4
        }

        /// <summary>
        /// Ek Ders Muhasebe İşlem Fişi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMIF = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "BELGENO" };

        /// <summary>
        /// Muhasebe Birim tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarMUHBIR : int
        {
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 1,
            /// <summary>
            /// MUHASEBEAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEAD = 2
        }

        /// <summary>
        /// Muhasebe Birim tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMUHBIR = { "MUHASEBEKOD", "BIRIMKOD", "MUHASEBEAD" };

        /// <summary>
        /// Öğrenci Eğitmen tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarTANIM
        {
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 1,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 2,
            /// <summary>
            /// ADISOYADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADISOYADI = 3,
        }

        /// <summary>
        /// Öğrenci Eğitmen tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTANIM = { "MUHASEBEKOD", "BIRIMKOD", "TCKIMLIKNO", "ADISOYADI" };

        /// <summary>
        /// Ek ders bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listelenecek Ek ders bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Ek ders bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(EkDers f)
        {
            string sql = "";
            sql += "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," +
                                VeriErisim.TumAlanlar(TABEkDersPuantaj.TABLOAD, TABEkDersPuantaj.ALANLAR) + "," +
                                TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.ADISOYADI] + "," +
                                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ADSOYAD] + "," +
                                "HRCOGRENCIEGITMEN.GOREV," +
                                TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.UNVAN];

            sql += " FROM " + TABLOAD;

            sql += " LEFT OUTER JOIN " + TABLOADPERSONEL + " ON " +
                       TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.TCKIMLIKNO];

            sql += " LEFT OUTER JOIN " + TABLOADTANIM + " ON " +
                       TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + " = " + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.MUHASEBEKOD] + " AND " +
                       TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + " = " + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BIRIMKOD] + " AND " +
                       TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.TCKIMLIKNO];

            sql += " LEFT OUTER JOIN " + TABLOADPERSONELPUANTAJ + " ON " +
                       TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + TABLOADPERSONELPUANTAJ + "." + ALANLARPERSONELPUANTAJ[(int)ENUMAlanlarPersonelPuantaj.TCKIMLIKNO];

            sql += " LEFT OUTER JOIN " + TABEkDersPuantaj.TABLOAD + " ON " +
                       TABLOADPERSONELPUANTAJ + "." + ALANLARPERSONELPUANTAJ[(int)ENUMAlanlarPersonelPuantaj.PUANTAJKODU] + " = " + TABEkDersPuantaj.TABLOAD + "." + TABEkDersPuantaj.ALANLAR[(int)TABEkDersPuantaj.ENUMAlanlar.KOD];

            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));
            if (f.ilgiliYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIYIL], "=", f.ilgiliYil));
            if (f.ilgiliAy > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIAY], "=", f.ilgiliAy));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPERSONELPUANTAJ, ALANLARPERSONELPUANTAJ[(int)ENUMAlanlarPersonelPuantaj.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPERSONELPUANTAJ, ALANLARPERSONELPUANTAJ[(int)ENUMAlanlarPersonelPuantaj.MUHASEBEKOD], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPERSONELPUANTAJ, ALANLARPERSONELPUANTAJ[(int)ENUMAlanlarPersonelPuantaj.BIRIMKOD], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPERSONELPUANTAJ, ALANLARPERSONELPUANTAJ[(int)ENUMAlanlarPersonelPuantaj.SURECNO], "=", f.surecNo));

            if ((f.personel.Count > 0) && (((EkDersPersonel)f.personel[0]).istihdamSekli > 0))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ISTIHDAMSEKLI], "=", ((FazlaMesaiPersonel)f.personel[0]).istihdamSekli));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIYIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIAY] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GUN];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Ek ders icmal girişi yapılmayan personel ve puantaj bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam
        /// </summary>
        /// <param name="bilgi">Listelenecek Ek ders icmal girişi yapılmayan personel ve puantaj bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Ek ders icmal girişi yapılmayan personel ve puantaj bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleIcmalGirisiYapilmayanlar(EkDers bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + TABEkDersPersonelPuantaj.TABLOAD + "." + TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.TCKIMLIKNO] + "," +
                TABEkDersPersonelPuantaj.TABLOAD + "." + TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.PUANTAJKODU] + ", " +
                VeriErisim.TumAlanlar(TABEkDersPuantaj.TABLOAD, TABEkDersPuantaj.ALANLAR) + ", " +
                TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.ADISOYADI] + ", " + " HRCPERSONEL.ADSOYAD " +

            " FROM " + TABEkDersPersonelPuantaj.TABLOAD +

            " LEFT OUTER JOIN HRCPERSONEL ON PSLEKDERSPERSONELPUANTAJ.TCKIMLIKNO = HRCPERSONEL.TCKIMLIKNO " +

            " LEFT OUTER JOIN " + TABLOADTANIM + " ON " +
                TABEkDersPersonelPuantaj.TABLOAD + "." + TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.MUHASEBEKOD] + " AND " +
                TABEkDersPersonelPuantaj.TABLOAD + "." + TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.BIRIMKOD] + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BIRIMKOD] + " AND " +
                TABEkDersPersonelPuantaj.TABLOAD + "." + TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.TCKIMLIKNO] + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.TCKIMLIKNO] +

            " LEFT OUTER JOIN " + TABEkDersPuantaj.TABLOAD + " ON " +
                TABEkDersPersonelPuantaj.TABLOAD + "." + TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.PUANTAJKODU] + " = " + TABEkDersPuantaj.TABLOAD + "." + TABEkDersPuantaj.ALANLAR[(int)TABEkDersPuantaj.ENUMAlanlar.KOD] + " AND " +
                TABEkDersPersonelPuantaj.TABLOAD + "." + TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.ILGILIAY] + " = " + TABEkDersPuantaj.TABLOAD + "." + TABEkDersPuantaj.ALANLAR[(int)TABEkDersPuantaj.ENUMAlanlar.AY] + " AND " +
                TABEkDersPersonelPuantaj.TABLOAD + "." + TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.ILGILIYIL] + " = " + TABEkDersPuantaj.TABLOAD + "." + TABEkDersPuantaj.ALANLAR[(int)TABEkDersPuantaj.ENUMAlanlar.YIL];

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEkDersPersonelPuantaj.TABLOAD, TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEkDersPersonelPuantaj.TABLOAD, TABFazlaMesaiPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEkDersPersonelPuantaj.TABLOAD, TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEkDersPersonelPuantaj.TABLOAD, TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            kosul += " AND " + TABEkDersPersonelPuantaj.TABLOAD + "." + TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.TCKIMLIKNO] + " NOT IN (SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, " WHERE ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo)) + ")";


            if (kosul != "")
                sql += " WHERE " + kosul;


            sql += " ORDER BY " + TABEkDersPersonelPuantaj.TABLOAD + "." + TABEkDersPersonelPuantaj.ALANLAR[(int)TABEkDersPersonelPuantaj.ENUMAlanlar.TCKIMLIKNO];

            return veriErisim.DataReaderDondur(sql);

        }

        /// <summary>
        /// Ek derse ait ödeme emri belge bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="surecNo">Listeleme için kullanılacak süreç numarası kıstas parametresi</param>
        /// <returns>
        /// Ek derse ait ödeme emri belge bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleOdemeEmri(int yil, string surecNo)
        {
            string sql = "";
            sql += "SELECT " + TABLOADMIF + "." + ALANLARMIF[(int)ENUMAlanlarMIF.BELGENO];
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl(TABLOADMIF, ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADMIF, ALANLARMIF[(int)ENUMAlanlarMIF.SURECNO], "=", surecNo));

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Ek ders bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak ek ders kıstas parametresi</param>
        /// <returns>
        /// Ek ders bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Sorgulama(EkDers f)
        {
            string sql = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIYIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIYIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIAY] + ", " + " FROM " + TABLOAD;


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIYIL], "=", f.ilgiliYil));

            if (f.ilgiliAy > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIAY], "=", f.ilgiliAy));

            if (f.surecNo != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));

            if (f.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));

            if (f.birim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIYIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIAY] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIYIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIAY] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Ek Ders İcmal bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek Ek Ders İcmal için gerekli Ek Ders bilgisini tutan nesne</param>
        /// <param name="fp">Eklenecek Ek Ders İcmal için gerekli Ek Ders Personel bilgisini tutan nesne</param>
        /// <param name="siraNo">Eklenecek Ek Ders İcmal için sıra bilgisini tutan parametre</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(EkDers f, EkDersPersonel fp, int siraNo)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            foreach (EkDersPersonelSaat fs in fp.gunSaat)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.yil) + ",";
                sql += VeriErisim.DegerYap(f.muhasebe) + ",";
                sql += VeriErisim.DegerYap(f.birim) + ",";
                sql += VeriErisim.DegerYap(f.surecNo) + ",";
                sql += VeriErisim.DegerYap(f.ilgiliYil) + ",";
                sql += VeriErisim.DegerYap(f.ilgiliAy) + ",";
                sql += VeriErisim.DegerYap(fp.tcKimlikNo) + ",";
                sql += VeriErisim.DegerYap(fs.gun) + ",";
                sql += VeriErisim.DegerYap(fs.saat) + ",";
                sql += VeriErisim.DegerYap(siraNo) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }

            return sonuc;
        }

        /// <summary>
        /// Ek Ders İcmal bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Silinecek Ek Ders İcmal bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(EkDers f)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Ek Ders Muhasebe İşlem Fişi belge numarası bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek Ek Ders bilgisini tutan nesne</param>
        /// <param name="belgeNo">Eklenecek Ek Ders Muhasebe İşlem Fişi belge numarası bilgisini tutan parametre</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc MIFNoKaydet(EkDersBordro f, string belgeNo)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "INSERT INTO " + TABLOADMIF + " (" + VeriErisim.TumAlanlar("", ALANLARMIF) + ") VALUES (";
            sql += VeriErisim.DegerYap(f.yil) + ",";
            sql += VeriErisim.DegerYap(f.muhasebe) + ",";
            sql += VeriErisim.DegerYap(f.birim) + ",";
            sql += VeriErisim.DegerYap(f.surecNo) + ",";
            sql += VeriErisim.DegerYap(belgeNo) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOADMIF + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARMIF[(int)ENUMAlanlarMIF.BELGENO], belgeNo);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", f.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.SURECNO], "=", f.surecNo));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Ek Ders Muhasebe İşlem Fişi belge numarası bilgisini veri tabanından çeken sql cümleciğini 
        /// verilen kıstasa göre üretip, çalıştırıp, datareader döndüren yordam
        /// </summary>
        /// <param name="f">Listelenecek Ek Ders Muhasebe İşlem Fişi belge numarası bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Ek Ders Muhasebe İşlem Fişi belge numarası bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MIFNoListele(EkDersBordro f)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMIF) + " FROM " + TABLOADMIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.SURECNO], "=", f.surecNo));

            return veriErisim.DataReaderDondur(sql);

        }

        /// <summary>
        /// Muhasebe Birim bilgisini veri tabanından çeken sql cümleciğini 
        /// verilen kıstasa göre üretip, çalıştırıp, datareader döndüren yordam
        /// </summary>
        /// <param name="f">Listelenecek Muhasebe Birim bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Muhasebe Birim bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MuhasebeBirimListele(EkDersBordro f)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMUHBIR, ALANLARMUHBIR) + " FROM " + TABLOADMUHBIR;
            sql += " ORDER BY 1,2";

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }
    }
}
