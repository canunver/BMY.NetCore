using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Hakediş bilgisini tutan HRCHAKEDIS, HRCHAKEDISSUREUZAT, HRCHAKEDISEKSOZLESME, HRCHAKEDISODEMEPLANI veri tabanı tabloları ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHakedis : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHakedis(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Hakediş bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCHAKEDIS";

        /// <summary>
        /// Hakediş bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// SOZLESMEBEDEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOZLESMEBEDEL = 4,
            /// <summary>
            /// IHALETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALETARIHI = 5,
            /// <summary>
            /// IHALEKARARTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEKARARTARIHI = 6,
            /// <summary>
            /// IHALEKARARNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEKARARNO = 7,
            /// <summary>
            /// SOZLESMETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOZLESMETARIHI = 8,
            /// <summary>
            /// SOZLESMENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOZLESMENO = 9,
            /// <summary>
            /// TESLIMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMTARIHI = 10,
            /// <summary>
            /// ISINSURESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISINSURESI = 11,
            /// <summary>
            /// BITIMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BITIMTARIHI = 12,

            /// <summary>
            /// SOZLESMEKDVBEDEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOZLESMEKDVBEDEL = 13
        }

        /// <summary>
        /// Hakediş bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "SOZLESMEBEDEL", "IHALETARIHI", "IHALEKARARTARIHI", "IHALEKARARNO", "SOZLESMETARIHI", "SOZLESMENO", "TESLIMTARIHI", "ISINSURESI", "BITIMTARIHI", "SOZLESMEKDVBEDEL" };

        /// <summary>
        /// Hakediş Süre Uzatım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADSUREUZAT = "HRCHAKEDISSUREUZAT";

        /// <summary>
        /// Hakediş Süre Uzatım bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarSureUzat
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
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,
            /// <summary>
            /// ONAYTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONAYTARIHI = 5,
            /// <summary>
            /// ONAYNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONAYNO = 6,
            /// <summary>
            /// SURE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURE = 7,
            /// <summary>
            /// BITIMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BITIMTARIHI = 8,
        }

        /// <summary>
        /// Hakediş süre uzatım bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSUREUZAT = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "SIRANO", "ONAYTARIHI", "ONAYNO", "SURE", "BITIMTARIHI" };

        /// <summary>
        /// Hakediş Ek Sözleşme bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADEKSOZLESME = "HRCHAKEDISEKSOZLESME";

        /// <summary>
        /// Hakediş ek sözleşme bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarEkSozlesme
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
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,
            /// <summary>
            /// ONAYTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONAYTARIHI = 5,
            /// <summary>
            /// ONAYNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONAYNO = 6,
            /// <summary>
            /// EKBEDEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKBEDEL = 7,
        }

        /// <summary>
        /// Hakediş ek sözleşme bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAREKSOZLESME = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "SIRANO", "ONAYTARIHI", "ONAYNO", "EKBEDEL" };

        /// <summary>
        /// Hakediş Ödeme Planı bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADODEMEPLAN = "HRCHAKEDISODEMEPLANI";

        /// <summary>
        /// Hakediş ödeme planı bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarOdemePlani
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
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,
            /// <summary>
            /// ODEMETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMETARIHI = 5,
            /// <summary>
            /// ODEMETUTARI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMETUTARI = 6,
            /// <summary>
            /// FIYATFARKIORANI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIYATFARKIORANI = 7,
            /// <summary>
            /// ISCILIKFIYATFARKI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISCILIKFIYATFARKI = 8,
            /// <summary>
            /// AVANSMIKTARI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AVANSMIKTARI = 9,
            /// <summary>
            /// PRFATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRFATURANO = 10,
        }

        /// <summary>
        /// Hakediş ödeme planı bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARODEMEPLANI = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "SIRANO", "ODEMETARIHI", "ODEMETUTARI", "FIYATFARKIORANI", "ISCILIKFIYATFARKI", "AVANSMIKTARI", "PRFATURANO" };

        /// <summary>
        /// Hakediş bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hakediş bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetOnBilgi(Hakedis bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.sozlesmeBedeli) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleKararTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleKararNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.sozlesmeTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.sozlesmeNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.teslimTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.isinSuresi) + ",";
            sql += VeriErisim.DegerYap(bilgi.bitimTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.sozlesmeKDVBedeli) + ")";

            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEBEDEL], bilgi.sozlesmeBedeli) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALETARIHI], bilgi.ihaleTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEKARARTARIHI], bilgi.ihaleKararTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEKARARNO], bilgi.ihaleKararNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMETARIHI], bilgi.sozlesmeTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMENO], bilgi.sozlesmeNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TESLIMTARIHI], bilgi.teslimTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISINSURESI], bilgi.isinSuresi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITIMTARIHI], bilgi.bitimTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEKDVBEDEL], bilgi.sozlesmeKDVBedeli);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Hakediş ödeme planı bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgiler">Eklenecek hakediş ödeme planı bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetOdemePlani(ObjectArray bilgiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (HakedisOdemePlan bilgi in bilgiler.objeler)
            {
                sql = "INSERT INTO " + TABLOADODEMEPLAN + " (" + VeriErisim.TumAlanlar("", ALANLARODEMEPLANI) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.odemeTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.odemeTutari) + ",";
                sql += VeriErisim.DegerYap(bilgi.fiyatFarkiOrani) + ",";
                sql += VeriErisim.DegerYap(bilgi.iscilikFiyatFarki) + ",";
                sql += VeriErisim.DegerYap(bilgi.avansMiktari) + ",";
                sql += VeriErisim.DegerYap(bilgi.prFaturaNo) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Hakediş süre uzatım bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgiler">Eklenecek hakediş süre uzatım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetSureUzat(ObjectArray bilgiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (HakedisSureUzatim bilgi in bilgiler.objeler)
            {
                sql = "INSERT INTO " + TABLOADSUREUZAT + " (" + VeriErisim.TumAlanlar("", ALANLARSUREUZAT) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.onayTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.onayNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.sure) + ",";
                sql += VeriErisim.DegerYap(bilgi.bitimTarihi) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Hakediş ek sözleşme bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgiler">Eklenecek hakediş ek sözleşme bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetEkSozlesme(ObjectArray bilgiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (HakedisEkSozlesme bilgi in bilgiler.objeler)
            {
                sql = "INSERT INTO " + TABLOADEKSOZLESME + " (" + VeriErisim.TumAlanlar("", ALANLAREKSOZLESME) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.onayTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.onayNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.sozlesmeBedeli) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Hakediş bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak hakediş bilgisi kıstas parametresi</param>
        /// <returns>
        /// Hakediş bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleOnBilgi(Hakedis bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            //if (bilgi.muhasebe > 0)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            //if (!string.IsNullOrWhiteSpace(bilgi.birim))
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hakediş ödeme planı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak hakediş bilgisi kıstas parametresi</param>
        /// <returns>
        /// Hakediş ödeme planı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleOdemePlani(Hakedis bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADODEMEPLAN, ALANLARODEMEPLANI) + ", HRCFATURAFORM.FATURANO, HRCFATURAFORM.ODEMEEMRINO ";
            sql += "FROM " + TABLOADODEMEPLAN + " LEFT OUTER JOIN ";
            sql += " HRCFATURAFORM ON HRCHAKEDISODEMEPLANI.YIL = HRCFATURAFORM.YIL AND HRCHAKEDISODEMEPLANI.SURECNO = HRCFATURAFORM.SURECNO AND ";
            sql += " HRCHAKEDISODEMEPLANI.PRFATURANO = HRCFATURAFORM.PRFATURANO ";
            sql += " WHERE (HRCFATURAFORM.DURUM = 1  OR HRCFATURAFORM.DURUM IS NULL) ";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADODEMEPLAN, ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.YIL], "=", bilgi.yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADODEMEPLAN, ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.MUHASEBEKOD], "=", bilgi.muhasebe));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADODEMEPLAN, ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADODEMEPLAN, ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.SURECNO], "=", bilgi.surecNo));

            sql += " ORDER BY " + ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.ODEMETARIHI] + "," + ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hakediş süre uzatım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak hakediş bilgisi kıstas parametresi</param>
        /// <returns>
        /// Hakediş süre uzatım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleSureUzatim(Hakedis bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSUREUZAT) + " FROM " + TABLOADSUREUZAT;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            sql += " ORDER BY " + ALANLARSUREUZAT[(int)ENUMAlanlarSureUzat.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hakediş ek sözleşme bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak hakediş bilgisi kıstas parametresi</param>
        /// <returns>
        /// Hakediş ek sözleşme bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleEkSozlesme(Hakedis bilgi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAREKSOZLESME) + " FROM " + TABLOADEKSOZLESME;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            sql += " ORDER BY " + ALANLAREKSOZLESME[(int)ENUMAlanlarEkSozlesme.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hakediş ödeme planı bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Silinecek yıl bilgisini tutan nesne</param>
        /// <param name="muhasebe">Silinecek muhasebe kod bilgisini tutan nesne</param>
        /// <param name="birim">Silinecek birim kod bilgisini tutan nesne</param>
        /// <param name="surecNo">Silinecek süreç numarası bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilOdemePlani(int yil, int muhasebe, string birim, string surecNo)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOADODEMEPLAN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.SURECNO], "=", surecNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Hakediş süre uzatımı bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Silinecek yıl bilgisini tutan nesne</param>
        /// <param name="muhasebe">Silinecek muhasebe kod bilgisini tutan nesne</param>
        /// <param name="birim">Silinecek birim kod bilgisini tutan nesne</param>
        /// <param name="surecNo">Silinecek süreç numarası bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilSureUzat(int yil, int muhasebe, string birim, string surecNo)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOADSUREUZAT;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Hakediş ek sözleşme bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Silinecek yıl bilgisini tutan nesne</param>
        /// <param name="muhasebe">Silinecek muhasebe kod bilgisini tutan nesne</param>
        /// <param name="birim">Silinecek birim kod bilgisini tutan nesne</param>
        /// <param name="surecNo">Silinecek süreç numarası bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilEkSozlesme(int yil, int muhasebe, string birim, string surecNo)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOADEKSOZLESME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Hakediş ödeme planı bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="plan">Silinecek hakediş ödeme planı bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilOdemePlani(HakedisOdemePlan plan)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOADODEMEPLAN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.YIL], "=", plan.yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.MUHASEBEKOD], "=", plan.muhasebe));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.BIRIMKOD], "=", plan.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.SURECNO], "=", plan.surecNo));
            if (plan.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.SIRANO], "=", plan.siraNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Hakediş ödemesine ait fatura numarası bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Eklenecek yıl bilgisini tutan nesne</param>
        /// <param name="surecNo">Eklenecek süreç numarası bilgisini tutan nesne</param>
        /// <param name="hakedisSiraNo">Eklenecek yıl bilgisini tutan nesne</param>
        /// <param name="faturaNo">Eklenecek fatura numarası bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc FaturaNoKaydet(int yil, string surecNo, int hakedisSiraNo, int faturaNo)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADODEMEPLAN + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.PRFATURANO], faturaNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.SURECNO], "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARODEMEPLANI[(int)ENUMAlanlarOdemePlani.SIRANO], "=", hakedisSiraNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Alım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="ay">Listeleme için kullanılacak ay kıstas parametresi</param>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="kur">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        /// <param name="fon">Listeleme için kullanılacak fonksiyonel kod kıstas parametresi</param>
        /// <param name="fin">Listeleme için kullanılacak finansman kod kıstas parametresi</param>
        /// <param name="eko">Listeleme için kullanılacak ekonomik kod kıstas parametresi</param>
        /// <param name="peb">Listeleme için kullanılacak alt faaliyet kod kıstas parametresi</param>
        /// <returns>
        /// Alım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleButceHazirlikAlimHarcamaSozlesme(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            string sql = string.Empty;

            sql = "SELECT     YEAR(HRCHAKEDIS.SOZLESMETARIHI) AS ISLEMYIL, MONTH(HRCHAKEDIS.SOZLESMETARIHI) AS ISLEMAY, HRCSUREC.YIL AS SURECYIL,  " +
                      " HRCHAKEDIS.SURECNO, HRCHAKEDIS.MUHASEBEKOD, HRCHAKEDIS.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD, HRCIHALEONAYTERTIP.FONKOD,  " +
                      " HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD, SUM(HRCHAKEDIS.SOZLESMEBEDEL) AS TOPLAM,  " +
                      " HRCSUREC.YILLARASARIISLEMMI " +
                    " FROM         HRCHAKEDIS LEFT OUTER JOIN " +
                      " HRCSUREC ON HRCHAKEDIS.YIL = HRCSUREC.YIL AND HRCHAKEDIS.SURECNO = HRCSUREC.SURECNO LEFT OUTER JOIN " +
                      " HRCIHALEONAYTERTIP ON HRCHAKEDIS.YIL = HRCIHALEONAYTERTIP.YIL AND HRCHAKEDIS.SURECNO = HRCIHALEONAYTERTIP.SURECNO " +
                    " WHERE     (HRCSUREC.SURECDURUM <> 9) ";

            if (yil > 0)
                sql += " AND (YEAR(HRCHAKEDIS.SOZLESMETARIHI))  = " + yil + ") ";
            if (ay > 0)
                sql += " AND (MONTH(HRCHAKEDIS.SOZLESMETARIHI)  = " + ay + ") ";
            if (muhasebe > 0)
                sql += " AND (HRCHAKEDIS.MUHASEBEKOD = " + muhasebe + ") ";
            if (!string.IsNullOrEmpty(birim))
                sql += " AND (HRCHAKEDIS.BIRIMKOD = '" + birim + "') ";
            if (!string.IsNullOrEmpty(kur))
                sql += " AND (HRCIHALEONAYTERTIP.KURKOD = '" + kur + "') ";
            if (!string.IsNullOrEmpty(fon))
                sql += " AND (HRCIHALEONAYTERTIP.FONKOD = '" + fon + "') ";
            if (!string.IsNullOrEmpty(fin))
                sql += " AND (HRCIHALEONAYTERTIP.FINKOD = '" + fin + "') ";
            if (!string.IsNullOrEmpty(eko))
                sql += " AND (HRCIHALEONAYTERTIP.EKOKOD = '" + eko + "') ";
            if (!string.IsNullOrEmpty(peb))
                sql += " AND (HRCIHALEONAYTERTIP.PEBKOD = '" + peb + "') ";

            sql += " GROUP BY YEAR(HRCHAKEDIS.SOZLESMETARIHI), MONTH(HRCHAKEDIS.SOZLESMETARIHI), HRCHAKEDIS.SURECNO, HRCHAKEDIS.MUHASEBEKOD, " +
                      " HRCHAKEDIS.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD, HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, " +
                      " HRCIHALEONAYTERTIP.PEBKOD, HRCSUREC.YILLARASARIISLEMMI, HRCSUREC.YIL";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}