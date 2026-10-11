using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// İhale ilan bilgisini tutan HRCIHALEILANBILGISI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIhaleIlanBilgisi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIhaleIlanBilgisi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İhale ilan bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCIHALEILANBILGISI";

        /// <summary>
        /// İhale ilan bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 4,
            /// <summary>
            /// TESLIMYERI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMYERI = 5,
            /// <summary>
            /// TESLIMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TESLIMTARIHI = 6,
            /// <summary>
            /// IHALEYERI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEYERI = 7,
            /// <summary>
            /// IHALETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALETARIHI = 8,
            /// <summary>
            /// IHALESAATI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALESAATI = 9,
            /// <summary>
            /// IHALETEMINYERI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALETEMINYERI = 10,
            /// <summary>
            /// TEKLIFSURESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFSURESI = 11,
            /// <summary>
            /// ISSURESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISSURESI = 12,
            /// <summary>
            /// TEKLIFTURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFTURU = 13,
            /// <summary>
            /// TEKLIFBELIRLEMETURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFBELIRLEMETURU = 14,
            /// <summary>
            /// KATILIMTURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KATILIMTURU = 15,
            /// <summary>
            /// INDIRIMORANI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            INDIRIMORANI = 16,
            /// <summary>
            /// KONSORSIYUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONSORSIYUM = 17,
            /// <summary>
            /// ILANADEDI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILANADEDI = 18,
            /// <summary>
            /// ILANSEKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILANSEKLI = 19,
            /// <summary>
            /// KAYNAKTURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAYNAKTURU = 20,
            /// <summary>
            /// KISMITEKLIFEACIKMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KISMITEKLIFEACIKMI = 21,
            /// <summary>
            /// SEKTORKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SEKTORKODU = 22,
            /// <summary>
            /// SEKTORADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SEKTORADI = 23,
            /// <summary>
            /// YMAKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YMAKOD = 24,
            /// <summary>
            /// YMABASLANGICTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YMABASLANGICTARIHI = 25,
            /// <summary>
            /// YMABITISTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YMABITISTARIHI = 26,
            /// <summary>
            /// YMAACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YMAACIKLAMA = 27,
            /// <summary>
            /// ONYETERLIKDAVETTURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONYETERLIKDAVETTURU = 28,
            /// <summary>
            /// ONYETERLIKDAVETSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONYETERLIKDAVETSAYI = 29,
        }

        /// <summary>
        /// İhale ilan bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TARIH", "TESLIMYERI", "TESLIMTARIHI", "IHALEYERI", "IHALETARIHI", "IHALESAATI", "IHALETEMINYERI", "TEKLIFSURESI", "ISSURESI", "TEKLIFTURU", "TEKLIFBELIRLEMETURU", "KATILIMTURU", "INDIRIMORANI", "KONSORSIYUM", "ILANADEDI", "ILANSEKLI", "KAYNAKTURU", "KISMITEKLIFEACIKMI", "SEKTORKODU", "SEKTORADI", "YMAKOD", "YMABASLANGICTARIHI", "YMABITISTARIHI", "YMAACIKLAMA", "ONYETERLIKDAVETTURU", "ONYETERLIKDAVETSAYI" };

        /// <summary>
        /// İhaleye teklif vermesi için firmaya yapılan davet bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADIHALENO = "HRCIHALENUMARASI";

        /// <summary>
        /// İhale numarası bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarIhaleNo
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
            /// IHALENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALENO = 4,
        }

        /// <summary>
        /// İhale numarası bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARIHALENO = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "IHALENO" };

        /// <summary>
        /// İhale ilan bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek ihale ilan bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(IhaleIlanBilgisi bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], bilgi.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TESLIMYERI], bilgi.teslimYeri) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TESLIMTARIHI], bilgi.teslimTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEYERI], bilgi.ihaleYeri) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALETARIHI], bilgi.ihaleTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALESAATI], bilgi.ihaleSaati) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALETEMINYERI], bilgi.ihaleTeminYeri) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFSURESI], bilgi.teklifSuresi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISSURESI], bilgi.isSuresi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFTURU], bilgi.teklifTuru) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFBELIRLEMETURU], bilgi.teklifBelirlemeTuru) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KATILIMTURU], bilgi.katilimTuru) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.INDIRIMORANI], bilgi.indirimOrani) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KONSORSIYUM], bilgi.konsorsiyum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILANADEDI], bilgi.ilanAdedi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILANSEKLI], bilgi.ilanSekli) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKTURU], bilgi.kaynakTuru) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISMITEKLIFEACIKMI], bilgi.kismiTeklifeAcikMi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SEKTORKODU], bilgi.sektorKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SEKTORADI], bilgi.sektorAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YMAKOD], bilgi.ymaKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YMABASLANGICTARIHI], bilgi.ymaBaslangicTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YMABITISTARIHI], bilgi.ymaBitisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YMAACIKLAMA], bilgi.ymaAciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONYETERLIKDAVETTURU], bilgi.onYeterlikDavetTuru) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONYETERLIKDAVETSAYI], bilgi.onYeterlikDavetSayi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.tarih) + ",";
                sql += VeriErisim.DegerYap(bilgi.teslimYeri) + ",";
                sql += VeriErisim.DegerYap(bilgi.teslimTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.ihaleYeri) + ",";
                sql += VeriErisim.DegerYap(bilgi.ihaleTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.ihaleSaati) + ",";
                sql += VeriErisim.DegerYap(bilgi.ihaleTeminYeri) + ",";
                sql += VeriErisim.DegerYap(bilgi.teklifSuresi) + ",";
                sql += VeriErisim.DegerYap(bilgi.isSuresi) + ",";
                sql += VeriErisim.DegerYap(bilgi.teklifTuru) + ",";
                sql += VeriErisim.DegerYap(bilgi.teklifBelirlemeTuru) + ",";
                sql += VeriErisim.DegerYap(bilgi.katilimTuru) + ",";
                sql += VeriErisim.DegerYap(bilgi.indirimOrani) + ",";
                sql += VeriErisim.DegerYap(bilgi.konsorsiyum) + ",";
                sql += VeriErisim.DegerYap(bilgi.ilanAdedi) + ",";
                sql += VeriErisim.DegerYap(bilgi.ilanSekli) + ",";
                sql += VeriErisim.DegerYap(bilgi.kaynakTuru) + ",";
                sql += VeriErisim.DegerYap(bilgi.kismiTeklifeAcikMi) + ",";
                sql += VeriErisim.DegerYap(bilgi.sektorKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.sektorAdi) + ",";
                sql += VeriErisim.DegerYap(bilgi.ymaKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.ymaBaslangicTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.ymaBitisTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.ymaAciklama) + ",";
                sql += VeriErisim.DegerYap(bilgi.onYeterlikDavetTuru) + ",";
                sql += VeriErisim.DegerYap(bilgi.onYeterlikDavetSayi) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// İhale ilan bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak ihale ilan bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale ilan bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(IhaleIlanBilgisi kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + TABLOADIHALENO + "." + ALANLARIHALENO[(int)ENUMAlanlarIhaleNo.IHALENO] +
                    " FROM " + TABLOAD + " LEFT OUTER JOIN " + TABLOADIHALENO + " ON " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABLOADIHALENO + "." + ALANLARIHALENO[(int)ENUMAlanlarIhaleNo.YIL] + " AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + " = " + TABLOADIHALENO + "." + ALANLARIHALENO[(int)ENUMAlanlarIhaleNo.MUHASEBEKOD] + " AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + " = " + TABLOADIHALENO + "." + ALANLARIHALENO[(int)ENUMAlanlarIhaleNo.BIRIMKOD] + " AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + " = " + TABLOADIHALENO + "." + ALANLARIHALENO[(int)ENUMAlanlarIhaleNo.SURECNO];

            if (kriter.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));
            if (kriter.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebe));
            if (kriter.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", kriter.birim));
            if (kriter.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", kriter.surecNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];

            return veriErisim.DataReaderDondur(sql);
        }

       
    }
}