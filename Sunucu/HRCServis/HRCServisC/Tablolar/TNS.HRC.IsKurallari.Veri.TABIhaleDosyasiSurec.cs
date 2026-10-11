using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// İhale Dosya İçerik Liste bilgisini tutan HRCIHALEDOSYASIICERIK veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIhaleDosyasiIcerikListe : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIhaleDosyasiIcerikListe(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İhale Dosya İçerik Liste bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCIHALEDOSYASIICERIKLISTE";
        /// <summary>
        /// İhale Dosya İçerik Liste bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// ISTUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISTUR = 4,
            /// <summary>
            /// IHALEUSUL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEUSUL = 5,
            /// <summary>
            /// SIRA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRA = 6,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 7,
            /// <summary>
            /// DOSYAADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOSYAADI = 8
        }

        /// <summary>
        /// İhale Dosya İçerik Liste bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ISTUR", "IHALEUSUL", "SIRA", "ADI", "DOSYAADI" };

        /// <summary>
        /// İhale Dosya İçerik Liste bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek İhale Dosya İçerik Liste bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Kaydet(IhaleDosyasiIcerikListe bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (IhaleDosyasiIcerik f in bilgi.liste.objeler)
            {

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";

                sql += VeriErisim.DegerYap(f.isTuru) + ",";
                sql += VeriErisim.DegerYap(f.ihaleUsul) + ",";
                sql += VeriErisim.DegerYap(f.sira) + ",";
                sql += VeriErisim.DegerYap(f.adi) + ",";
                sql += VeriErisim.DegerYap(f.dosyaAdi) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// İhale Dosya İçerik Liste bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak İhale Dosya İçerik bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale Dosya İçerik Liste bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(IhaleDosyasiIcerikListe bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            if (bilgi.liste.objeler.Count > 0)
            {
                IhaleDosyasiIcerik satir = new IhaleDosyasiIcerik();
                satir = (IhaleDosyasiIcerik)bilgi.liste.objeler[0];

                if (satir.isTuru != 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISTUR], "=", satir.isTuru));
                if (satir.ihaleUsul != 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEUSUL], "=", satir.ihaleUsul));
                if (satir.sira != 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", satir.sira));
                if (satir.adi != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ADI], "LIKE", "%" + satir.adi + "%"));
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY 1,2,3,4,5,6";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İhale Dosya İçerik Liste bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek İhale Dosya İçerik bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(IhaleDosyasiIcerikListe bilgi)
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

    /// <summary>
    /// İhale Dosyası Alan bilgisini tutan HRCIHALETEKLIFDOSYASI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIhaleDosyasiAlan : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIhaleDosyasiAlan(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İhale Dosyası Alan bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCIHALETEKLIFDOSYASI";

        /// <summary>
        /// İhale Dosyası Alan bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// ALINDISIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALINDISIRANO = 4,
            /// <summary>
            /// FIRMAKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAKODU = 5,
            /// <summary>
            /// ALINDIBELGETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALINDIBELGETARIHI = 6,
            /// <summary>
            /// ALINDIBELGESAATI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALINDIBELGESAATI = 7,
            /// <summary>
            /// SATISBELGETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SATISBELGETARIHI = 8,
            /// <summary>
            /// SATISBELGENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SATISBELGENO = 9,
            /// <summary>
            /// TESLIMSIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMSIRANO = 10,
            /// <summary>
            /// TESLIMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMTARIHI = 11,
            /// <summary>
            /// TESLIMSAATI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMSAATI = 12,
            /// <summary>
            /// GELISSEKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELISSEKLI = 13,
            /// <summary>
            /// POSTADAGECIKTIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            POSTADAGECIKTIMI = 14,
            /// <summary>
            /// TEKLIFDOSYASIVERDIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFDOSYASIVERDIMI = 15,
            /// <summary>
            /// SATINALANKISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SATINALANKISI = 16,
            /// <summary>
            /// FIRMADEVAM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMADEVAM = 17,
            /// <summary>
            /// FIRMAYETERLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAYETERLI = 18,
            /// <summary>
            /// GEREKCE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 19,
            /// <summary>
            /// IHALESAMASI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEASAMASI = 20,
            /// <summary>
            /// BELGEGIDISSEKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGEGIDISSEKLI = 21,
            /// <summary>
            /// BELGEGIDISTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGEGIDISTARIHI = 22,

        }

        /// <summary>
        /// İhale Dosyası Alan bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ALINDISIRANO", "FIRMAKODU", "ALINDIBELGETARIHI", "ALINDIBELGESAATI", "SATISBELGETARIHI", "SATISBELGENO ", "TESLIMSIRANO", "TESLIMTARIHI", "TESLIMSAATI", "GELISSEKLI", "POSTADAGECIKTIMI", "TEKLIFDOSYASIVERDIMI", "SATINALANKISI", "FIRMADEVAM", "FIRMAYETERLI", "GEREKCE", "IHALEASAMASI", "BELGEGIDISSEKLI", "BELGEGIDISTARIHI" };

        /// <summary>
        /// İhale Dosyası Alan bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="list">Eklenecek İhale Dosyası Alan bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Kaydet(ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            string kosul = "";

            foreach (IhaleTeklifDosyasi bilgi in list.objeler)
            {

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.alindiSiraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.firmaKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.alindiBelgeTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.alindiBelgeSaati) + ",";
                sql += VeriErisim.DegerYap(bilgi.satisBelgeTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.satisBelgeNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.teslimSiraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.teslimTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.teslimSaati) + ",";
                sql += VeriErisim.DegerYap(bilgi.gelisSekli) + ",";
                sql += VeriErisim.DegerYap(bilgi.postadaGeciktimi) + ",";
                sql += VeriErisim.DegerYap(bilgi.teklifDosyasiVerdimi) + ",";
                sql += VeriErisim.DegerYap(bilgi.satinAlanKisi) + ",";
                sql += VeriErisim.DegerYap(bilgi.firmaDevam) + ",";
                sql += VeriErisim.DegerYap(bilgi.firmaYeterli) + ",";
                sql += VeriErisim.DegerYap(bilgi.gerekce) + ",";
                sql += VeriErisim.DegerYap(bilgi.ihaleAsamasi) + ",";
                sql += VeriErisim.DegerYap(bilgi.belgeGidisSekli) + ",";
                sql += VeriErisim.DegerYap(bilgi.belgeGidisTarihi) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sql = "UPDATE " + TABLOAD + " SET ";

                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALINDIBELGESAATI], bilgi.alindiBelgeSaati) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALINDIBELGETARIHI], bilgi.alindiBelgeTarihi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALINDISIRANO], bilgi.alindiSiraNo) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SATISBELGENO], bilgi.satisBelgeNo) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SATISBELGETARIHI], bilgi.satisBelgeTarihi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SATINALANKISI], bilgi.satinAlanKisi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEASAMASI], bilgi.ihaleAsamasi);

                    kosul = VeriErisim.KosulEkle(" WHERE", "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKODU], "=", (bilgi.firmaKodu)));

                    sql += kosul;

                    sonuc = KomutCalistir(sql, 1);

                }
            }
            return sonuc;
        }

        /// <summary>
        /// İhale Dosyası Alan Liste bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak İhale Dosyası Alan bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale Dosyası Alan bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(IhaleTeklifDosyasi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD] + " FROM " + TABLOAD + " LEFT OUTER JOIN " + TABFirmaBilgisi.TABLOAD + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU] + " = " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD];

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            if (bilgi.firmaKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FIRMAKODU], "=", bilgi.firmaKodu));
            //if (bilgi.ihaleAsamasi > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));
            if (bilgi.firmaDevam > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FIRMADEVAM], "=", bilgi.firmaDevam));
            if (bilgi.firmaYeterli > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FIRMAYETERLI], "=", bilgi.firmaYeterli));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALINDISIRANO] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKODU];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İhale Dosyası Alan bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek İhale Dosyası Alan bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            foreach (IhaleTeklifDosyasi dosya in bilgi.objeler)
            {
                string sql = "";

                sql = "DELETE FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", dosya.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", dosya.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", dosya.birim));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", dosya.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKODU], "=", dosya.firmaKodu));

                sonuc = KomutCalistir(sql, 0);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }
            return sonuc;
        }

    }

    /// <summary>
    /// İhale Teklif Dosyası Veren bilgisini tutan HRCIHALETEKLIFDOSYASI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTeklifDosyasiVeren : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTeklifDosyasiVeren(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İhale Teklif Dosyası Veren bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCIHALETEKLIFDOSYASI";

        /// <summary>
        /// İhale Teklif Dosyası Veren bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// ALINDISIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALINDISIRANO = 4,
            /// <summary>
            /// FIRMAKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAKODU = 5,
            /// <summary>
            /// ALINDIBELGETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALINDIBELGETARIHI = 6,
            /// <summary>
            /// ALINDIBELGESAATI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALINDIBELGESAATI = 7,
            /// <summary>
            /// SATISBELGETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SATISBELGETARIHI = 8,
            /// <summary>
            /// SATISBELGENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SATISBELGENO = 9,
            /// <summary>
            /// TESLIMSIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMSIRANO = 10,
            /// <summary>
            /// TESLIMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMTARIHI = 11,
            /// <summary>
            /// TESLIMSAATI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMSAATI = 12,
            /// <summary>
            /// GELISSEKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELISSEKLI = 13,
            /// <summary>
            /// POSTADASEKLIDEGISTIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            POSTADAGECIKTIMI = 14,
            /// <summary>
            /// TEKLIFDOSYASIVERDIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFDOSYASIVERDIMI = 15,
            /// <summary>
            /// SATINALANKISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SATINALANKISI = 16,
            /// <summary>
            /// FIRMADEVAM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMADEVAM = 17,
            /// <summary>
            /// FIRMAYETERLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAYETERLI = 18,
            /// <summary>
            /// GEREKCE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 19,
            /// <summary>
            /// IHALEASAMASI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEASAMASI = 20,
            /// <summary>
            /// BELGEGIDISSEKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGEGIDISSEKLI = 21,
            /// <summary>
            /// BELGEGIDISTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGEGIDISTARIHI = 22,
        }

        /// <summary>
        /// İhale Teklif Dosyası Veren bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ALINDISIRANO", "FIRMAKODU", "ALINDIBELGETARIHI", "ALINDIBELGESAATI", "SATISBELGETARIHI", "SATISBELGENO ", "TESLIMSIRANO", "TESLIMTARIHI", "TESLIMSAATI", "GELISSEKLI", "POSTADAGECIKTIMI", "TEKLIFDOSYASIVERDIMI", "SATINALANKISI", "FIRMADEVAM", "FIRMAYETERLI", "GEREKCE", "IHALEASAMASI", "BELGEGIDISSEKLI", "BELGEGIDISTARIHI" };

        /// <summary>
        /// İhale Teklif Dosyası Veren bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="list">Eklenecek İhale Teklif Dosyası Veren bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Kaydet(ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            string kosul = "";

            foreach (IhaleTeklifDosyasi bilgi in list.objeler)
            {

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.alindiSiraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.firmaKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.alindiBelgeTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.alindiBelgeSaati) + ",";
                sql += VeriErisim.DegerYap(bilgi.satisBelgeTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.satisBelgeNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.teslimSiraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.teslimTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.teslimSaati) + ",";
                sql += VeriErisim.DegerYap(bilgi.gelisSekli) + ",";
                sql += VeriErisim.DegerYap(bilgi.postadaGeciktimi) + ",";
                sql += VeriErisim.DegerYap(bilgi.teklifDosyasiVerdimi) + ",";
                sql += VeriErisim.DegerYap(bilgi.satinAlanKisi) + ",";
                sql += VeriErisim.DegerYap(bilgi.firmaDevam) + ",";
                sql += VeriErisim.DegerYap(bilgi.firmaYeterli) + ",";
                sql += VeriErisim.DegerYap(bilgi.gerekce) + ",";
                sql += VeriErisim.DegerYap(bilgi.ihaleAsamasi) + ",";
                sql += VeriErisim.DegerYap(bilgi.belgeGidisSekli) + ",";
                sql += VeriErisim.DegerYap(bilgi.belgeGidisTarihi) + ")";
                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sql = "UPDATE " + TABLOAD + " SET ";

                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELISSEKLI], bilgi.gelisSekli) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.POSTADAGECIKTIMI], bilgi.postadaGeciktimi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TESLIMSAATI], bilgi.teslimSaati) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TESLIMSIRANO], bilgi.teslimSiraNo) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TESLIMTARIHI], bilgi.teslimTarihi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFDOSYASIVERDIMI], bilgi.teklifDosyasiVerdimi) + ",";
                    if (bilgi.teklifDosyasiVerdimi == (int)ENUMTeklifDosyasi.VERDI)
                    {
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMADEVAM], (int)ENUMEvetHayir.EVET);
                    }
                    else
                    {
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMADEVAM], (int)ENUMEvetHayir.HAYIR);
                    }

                    kosul = VeriErisim.KosulEkle(" WHERE", "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKODU], "=", (bilgi.firmaKodu)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEASAMASI], "=", (bilgi.ihaleAsamasi)));

                    sql += kosul;

                    sonuc = KomutCalistir(sql, 1);

                }
            }
            return sonuc;
        }

        /// <summary>
        /// İhale Teklif Dosyası Veren bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="list">Eklenecek İhale Teklif Dosyası Veren bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc GuncelleFirmaSurecBilgisi(ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            string kosul = "";

            foreach (IhaleTeklifDosyasi bilgi in list.objeler)
            {

                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMADEVAM], bilgi.firmaDevam) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMAYETERLI], bilgi.firmaYeterli) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCE], bilgi.gerekce) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGEGIDISSEKLI], bilgi.belgeGidisSekli) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGEGIDISTARIHI], bilgi.belgeGidisTarihi);


                kosul = VeriErisim.KosulEkle(" WHERE", "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKODU], "=", (bilgi.firmaKodu)));
                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEASAMASI], "=", (bilgi.ihaleAsamasi)));

                sql += kosul;

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// İhale Teklif Dosyası Veren bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak İhale Teklif Dosyası Veren bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale Teklif Dosyası Veren bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(IhaleTeklifDosyasi bilgi)
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
            //bunun esitlik kontrolu olmasın!
            if (bilgi.teklifDosyasiVerdimi == (int)ENUMTeklifDosyasi.HEPSI)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TEKLIFDOSYASIVERDIMI], "<", bilgi.teklifDosyasiVerdimi));
            else
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TEKLIFDOSYASIVERDIMI], "=", bilgi.teklifDosyasiVerdimi));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.POSTADAGECIKTIMI], "=", 0));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GELISSEKLI], "!=", 3));
            }
            //if (bilgi.ihaleAsamasi > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));

            if (bilgi.firmaDevam > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FIRMADEVAM], "=", bilgi.firmaDevam));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TESLIMSIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İhale Teklif Dosyası Veren ve ihale devam eden firma bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak İhale Teklif Dosyası Veren bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale Teklif Dosyası Veren ve ihale devam eden firma bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleDevamEdenFirma(IhaleTeklifDosyasi bilgi)
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
            //bunun esitlik kontrolu olmasın!
            if (bilgi.firmaDevam != (int)ENUMEvetHayir.HEPSI)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FIRMADEVAM], "=", bilgi.firmaDevam));
            if (bilgi.ihaleAsamasi > 0)
            {
                if (bilgi.firmaYeterli != (int)ENUMEvetHayir.HEPSI)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FIRMAYETERLI], "=", bilgi.firmaYeterli));
            }
            ////İhale Aşaması her aşamada listeleme koşulu olmalıdır. 0 ise açık ihale.. 1 veya 2 ise belirli istekli yada pazarlık usulu
            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.POSTADAGECIKTIMI], "=", (int)ENUMEvetHayir.HAYIR));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, TABIhaleDosyasiAlan.ALANLAR[(int)TABIhaleDosyasiAlan.ENUMAlanlar.GELISSEKLI], "!=", (int)ENUMTeklifGelisTuru.GELMEDI));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TESLIMSIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İhale Teklif Dosyası Veren bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek İhale Teklif Dosyası Veren bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(IhaleTeklifDosyasi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));

            return KomutCalistir(sql, 0);
        }

    }

    /// <summary>
    /// İhale Komisyon Tutanağı bilgisini tutan HRCTEKLIFKOMISYONTUTANAGI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTeklifDosyasiKomisyonTutanagi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTeklifDosyasiKomisyonTutanagi(VeriErisim veriErisim)
            : base(veriErisim)
        {
            if (veriErisim.baglantiTip == "ORACLE")
                TABLOAD = "HRCTEKLIFKOMTUT";
        }
        
        /// <summary>
        /// İhale Komisyon Tutanağı bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCTEKLIFKOMISYONTUTANAGI";
        
        /// <summary>
        /// İhale Komisyon Tutanağı bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// TUTANAKTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUTANAKTARIHI = 4,
            /// <summary>
            /// TUTANAKSAATI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUTANAKSAATI = 5,
            /// <summary>
            /// TESLIMEDEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMEDEN = 6,
            /// <summary>
            /// TESLIMEDENUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMEDENUNVAN = 7,
            /// <summary>
            /// IHALEASAMASI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEASAMASI = 8,
        }
        
        /// <summary>
        /// İhale Komisyon Tutanağı bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TUTANAKTARIHI", "TUTANAKSAATI", "TESLIMEDEN", "TESLIMEDENUNVAN", "IHALEASAMASI" };
        
        /// <summary>
        /// İhale Komisyon Tutanağı bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek İhale Komisyon Tutanağı bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Kaydet(TeklifDosyasiKomisyonTutanagi bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            string kosul = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tutanakTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.tutanakSaati) + ",";
            sql += VeriErisim.DegerYap(bilgi.teslimEden) + ",";
            sql += VeriErisim.DegerYap(bilgi.teslimEdenUnvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleAsamasi) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";


                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKTARIHI], bilgi.tutanakTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKSAATI], bilgi.tutanakSaati) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TESLIMEDEN], bilgi.teslimEden) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TESLIMEDENUNVAN], bilgi.teslimEdenUnvan);


                kosul = VeriErisim.KosulEkle(" WHERE ", "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));

                sql += kosul;

                sonuc = KomutCalistir(sql, 1);

            }
            return sonuc;
        }
        
        /// <summary>
        /// İhale Komisyon Tutanağı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak İhale Komisyon Tutanağı bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale Komisyon Tutanağı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(TeklifDosyasiKomisyonTutanagi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEASAMASI], "=", bilgi.ihaleAsamasi));
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SURECNO];

            return veriErisim.DataReaderDondur(sql);
        }
        
        /// <summary>
        /// İhale Komisyon Tutanağı Liste bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Silinecek İhale Komisyon Tutanağı bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(TeklifDosyasiKomisyonTutanagi bilgi)
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