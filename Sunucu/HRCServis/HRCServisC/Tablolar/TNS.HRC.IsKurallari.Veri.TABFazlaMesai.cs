using System;
using System.Data;
using TNS.HRC.IsKurallari;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Fazla Mesai İcmal bilgisini tutan PSLFAZLAMESAI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABFazlaMesai : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFazlaMesai(VeriErisim veriErisim)
            : base(veriErisim)
        {
            if (veriErisim.baglantiTip == "ORACLE")
                TABLOADPERSONELPUANTAJ = "PSLFMPERPUAN";
        }

        /// <summary>
        /// Fazla Mesai İcmal tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLFAZLAMESAI";

        /// <summary>
        /// Personel tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONEL = "HRCPERSONEL";

        /// <summary>
        /// Fazla Mesaiye ait Muhasebe İşlem Fişi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADMIF = "PSLFAZLAMESAIMIF";

        /// <summary>
        /// Muhasebe Birim tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADMUHBIR = "MUHASEBEBIRIM";

        /// <summary>
        /// Fazla Mesai Personel Puantaj tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONELPUANTAJ = "PSLFAZLAMESAIPERSONELPUANTAJ";

        /// <summary>
        /// Fazla Mesai İcmal tablo alanlarının sırasını tutan enumeration
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
        /// Fazla Mesai İcmal tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ILGILIYIL", "ILGILIAY", "TCKIMLIKNO", "GUN", "SAAT", "SIRANO" };

        /// <summary>
        /// Personel tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarPersonel
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
        /// Fazla Mesai Personel Puantaj tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarPersonelPuantaj
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
            /// PUANTAJKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PUANTAJKODU = 7
        }

        /// <summary>
        /// Fazla Mesai Personel Puantaj tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPERSONELPUANTAJ = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ILGILIYIL", "ILGILIAY", "TCKIMLIKNO", "PUANTAJKODU" };

        /// <summary>
        /// Fazla Mesai Muhasebe İşlem Fişi tablo alanlarının sırasını tutan enumeration
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
        /// Fazla Mesai Muhasebe İşlem Fişi tablosu alanlarının isimlerini tutan sabit dizi
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
        /// Fazla Mesai İcmal bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak fazla mesai kıstas parametresi</param>
        /// <returns>
        /// Fazla Mesai İcmal bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(FazlaMesai f)
        {
            string sql = "";
            sql += "SELECT " + 
                VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + 
                VeriErisim.TumAlanlar(TABLOADPERSONEL, ALANLARPERSONEL) + "," + 
                VeriErisim.TumAlanlar(TABFazlaMesaiPuantaj.TABLOAD, TABFazlaMesaiPuantaj.ALANLAR);
            
            sql += " FROM " + TABLOAD + " LEFT OUTER JOIN " + TABLOADPERSONEL + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + TABLOADPERSONEL + ".TCKIMLIKNO ";
            sql += " LEFT OUTER JOIN " + TABLOADPERSONELPUANTAJ + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + TABLOADPERSONELPUANTAJ + "." + ALANLARPERSONELPUANTAJ[(int)ENUMAlanlarPersonelPuantaj.TCKIMLIKNO];
            sql += " LEFT OUTER JOIN " + TABFazlaMesaiPuantaj.TABLOAD + " ON " + TABLOADPERSONELPUANTAJ + "." + ALANLARPERSONELPUANTAJ[(int)ENUMAlanlarPersonelPuantaj.PUANTAJKODU] + " = " + TABFazlaMesaiPuantaj.TABLOAD + "." + TABFazlaMesaiPuantaj.ALANLAR[(int)TABFazlaMesaiPuantaj.ENUMAlanlar.KOD];

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

            if (f.personel.Count > 0 && ((FazlaMesaiPersonel)f.personel[0]).istihdamSekli > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ISTIHDAMSEKLI], "=", ((FazlaMesaiPersonel)f.personel[0]).istihdamSekli));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIYIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIAY] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ADSOYAD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GUN];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        public IDataReader ListelePersonelFazlaMesai(int yil, string surecNo, string tcKimlikNo)
        {
            string sql = "";
            sql += "SELECT " +
                VeriErisim.TumAlanlar(TABLOAD, ALANLAR);

            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));
            if (!string.IsNullOrEmpty(tcKimlikNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", tcKimlikNo));
            
            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Fazla Mesai İcmal girişi yapılmayan personel bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak fazla mesai kıstas parametresi</param>
        /// <returns>
        /// Fazla Mesai İcmal girişi yapılmayan personel bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleIcmalGirisiYapilmayanlar(FazlaMesai bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " +
                TABFazlaMesaiPersonelPuantaj.TABLOAD + "." + TABFazlaMesaiPersonelPuantaj.ALANLAR[(int)TABFazlaMesaiPersonelPuantaj.ENUMAlanlar.TCKIMLIKNO] + "," +
                TABFazlaMesaiPersonelPuantaj.TABLOAD + "." + TABFazlaMesaiPersonelPuantaj.ALANLAR[(int)TABFazlaMesaiPersonelPuantaj.ENUMAlanlar.PUANTAJKODU] + ", " +
                VeriErisim.TumAlanlar(TABFazlaMesaiPuantaj.TABLOAD, TABFazlaMesaiPuantaj.ALANLAR) + ", " +
                TABLOADPERSONEL + ".ADSOYAD " +

                " FROM " + TABFazlaMesaiPersonelPuantaj.TABLOAD
               
                + " LEFT OUTER JOIN " + TABFazlaMesaiPuantaj.TABLOAD + " ON " + 
               TABFazlaMesaiPersonelPuantaj.TABLOAD + "." + TABFazlaMesaiPersonelPuantaj.ALANLAR[(int)TABFazlaMesaiPersonelPuantaj.ENUMAlanlar.PUANTAJKODU] 
               + " = " + TABFazlaMesaiPuantaj.TABLOAD + "." + TABFazlaMesaiPuantaj.ALANLAR[(int)TABFazlaMesaiPuantaj.ENUMAlanlar.KOD]
               
               + " LEFT OUTER JOIN " + TABLOADPERSONEL + " ON " + 
               TABFazlaMesaiPersonelPuantaj.TABLOAD + "." + TABFazlaMesaiPersonelPuantaj.ALANLAR[(int)TABFazlaMesaiPersonelPuantaj.ENUMAlanlar.TCKIMLIKNO] 
               + " = " + TABLOADPERSONEL + ".TCKIMLIKNO ";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABFazlaMesaiPersonelPuantaj.TABLOAD, TABFazlaMesaiPersonelPuantaj.ALANLAR[(int)TABFazlaMesaiPersonelPuantaj.ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABFazlaMesaiPersonelPuantaj.TABLOAD, TABFazlaMesaiPersonelPuantaj.ALANLAR[(int)TABFazlaMesaiPersonelPuantaj.ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABFazlaMesaiPersonelPuantaj.TABLOAD, TABFazlaMesaiPersonelPuantaj.ALANLAR[(int)TABFazlaMesaiPersonelPuantaj.ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABFazlaMesaiPersonelPuantaj.TABLOAD, TABFazlaMesaiPersonelPuantaj.ALANLAR[(int)TABFazlaMesaiPersonelPuantaj.ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            kosul += " AND " + TABFazlaMesaiPersonelPuantaj.TABLOAD + "." + TABFazlaMesaiPersonelPuantaj.ALANLAR[(int)TABFazlaMesaiPersonelPuantaj.ENUMAlanlar.TCKIMLIKNO] + " NOT IN (SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, " WHERE ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo)) + ")";


            if (kosul != "")
                sql += " WHERE " + kosul;


            sql += " ORDER BY " + TABFazlaMesaiPersonelPuantaj.TABLOAD + "." + TABFazlaMesaiPersonelPuantaj.ALANLAR[(int)TABFazlaMesaiPersonelPuantaj.ENUMAlanlar.SIRANO] + ", " + TABFazlaMesaiPersonelPuantaj.TABLOAD + "." + TABFazlaMesaiPersonelPuantaj.ALANLAR[(int)TABFazlaMesaiPersonelPuantaj.ENUMAlanlar.TCKIMLIKNO];

            return veriErisim.DataReaderDondur(sql);

        }

        /// <summary>
        /// Fazla Mesai İcmal bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listelenecek fazla mesai icmal bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Fazla Mesai İcmal bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Sorgulama(FazlaMesai f)
        {
            string sql = "";

            sql = "SELECT " + 
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIYIL] + ", " + 
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + 
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + 
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + ", " + 
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIYIL] + ", " + 
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIAY] + ", " + 
                
                " FROM " + TABLOAD;

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
        /// Fazla Mesai İcmal bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek fazla mesai bilgisini tutan nesne</param>
        /// <param name="fp">Eklenecek fazla mesai personel bilgisini tutan nesne</param>
        /// <param name="siraNo">Eklenecek fazla mesai icmal bilgisinin sıra numarasını tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(FazlaMesai f, FazlaMesaiPersonel fp, int siraNo)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            foreach (FazlaMesaiPersonelSaat fs in fp.gunSaat)
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
        /// Fazla mesai icmal bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Silinecek fazla mesai icmal bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(FazlaMesai f)
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
        /// Fazla Mesai İcmal bilgisine ait muhasebe işlem fişi bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek fazla mesai bilgisini tutan nesne</param>
        /// <param name="belgeNo">Eklenecek muhasebe işlem fişi numarası bilgisi</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc MIFNoKaydet(FazlaMesaiBordro f, string belgeNo)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "UPDATE " + TABLOADMIF + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARMIF[(int)ENUMAlanlarMIF.BELGENO], belgeNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.SURECNO], "=", f.surecNo));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "INSERT INTO " + TABLOADMIF + " (" + VeriErisim.TumAlanlar("", ALANLARMIF) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.yil) + ",";
                sql += VeriErisim.DegerYap(f.muhasebe) + ",";
                sql += VeriErisim.DegerYap(f.birim) + ",";
                sql += VeriErisim.DegerYap(f.surecNo) + ",";
                sql += VeriErisim.DegerYap(belgeNo) + ")";
                
                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Fazla Mesai İcmal bilgisine ait muhasebe işlem fişi numarası bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak fazla mesai kıstas parametresi</param>
        /// <returns>
        /// Fazla Mesai İcmal bilgisine ait muhasebe işlem fişi numarası bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MIFNoListele(FazlaMesaiBordro f)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMIF) + " FROM " + TABLOADMIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.SURECNO], "=", f.surecNo));

            return veriErisim.DataReaderDondur(sql);

        }

        /// <summary>
        /// Muhasebe birim bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak fazla mesai kıstas parametresi</param>
        /// <returns>
        /// Muhasebe birim bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MuhasebeBirimListele(FazlaMesaiBordro f)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMUHBIR, ALANLARMUHBIR) + " FROM " + TABLOADMUHBIR;
            sql += " ORDER BY 1,2";

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }


    }
}
