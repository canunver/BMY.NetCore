using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Dosya planı bilgilerini içeren EVRDOSYAPLANI veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class TABDosyaPlani : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABDosyaPlani sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABDosyaPlani(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Dosya planı bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRDOSYAPLANI";

        /// <summary>
        /// Dosya planı bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            AD = 1,
            SEVIYE = 2,
            DETAY = 3,
            KULLANILMIYOR = 4,
            SAKLAMASURESI = 5,
            SAKLAMAKRITERI = 6,
            ISLEMKOD = 7,
            SUREBASLANGICTIPI = 8,
            TASFIYEPLANTIPI = 9,
            GEREKCE = 10,
            ACIKLAMA = 11,
            GIZLILIK = 12,
            ANAHTARKELIME = 13,
            ACILISTARIHI = 14,
            KAPANISTARIHI = 15,
            ADTIPI = 16,
            SAKLAMAPLANTIPI = 17,
        }

        /// <summary>
        /// Dosya planı bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLAR = { "KOD", "AD", "SEVIYE", "DETAY", "KULLANILMIYOR", "SAKLAMASURESI", "SAKLAMAKRITERI", "ISLEMKOD", "SUREBASLANGICTIPI", "TASFIYEPLANTIPI", "GEREKCE", "ACIKLAMA", "GIZLILIK", "ANAHTARKELIME", "ACILISTARIHI", "KAPANISTARIHI", "ADTIPI", "SAKLAMAPLANTIPI"  };

        /// <summary>
        /// Dosya planı tarihçe bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOADTARIHCE = "EVRDOSYAPLANITARIHCE";

        /// <summary>
        /// Dosya planı tarihçe bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlarTarihce
        {
            ISLEMKOD = 0,
            KOD = 1,
            TARIH = 2,
            KOLON = 3,
            ESKIDEGER = 4,
            YENIDEGER = 5,
            ISLEM = 6,
            KULLANICIKODU = 7
        }

        /// <summary>
        /// Dosya planı tarihçe bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLARTARIHCE = { "ISLEMKOD", "KOD", "TARIH", "KOLON", "ESKIDEGER", "YENIDEGER", "ISLEM", "KULLANICIKODU" };

        /// <summary>
        /// Dosya planı bolum bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOADBOLUMTANIM = "EVRDOSYAPLANIBOLUMTANIM";

        /// <summary>
        /// Dosya planı bolum bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlarBolumTanim
        {
            HESAPKODU = 0,
            BOLUMADI = 1,
            DURUMU = 2,
            PERIYOD = 3,
            BASLANGICGUN = 4,
            BITISGUN = 5,
            BASLANGICAY = 6,
            BITISAY = 7,
            SONISLEM = 8,
            TUR = 9,
            SAKLAMAORTAMI = 10,
            FIZIKISAKLAMAYERI = 11,
        }

        /// <summary>
        /// Dosya planı tarihçe bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLARBOLUMTANIM = { "HESAPKODU", "BOLUMADI", "DURUMU", "PERIYOD", "BASLANGICGUN", "BITISGUN", "BASLANGICAY", "BITISAY", "SONISLEM", "TUR", "SAKLAMAORTAMI", "FIZIKISAKLAMAYERI" };

        /// <summary>
        /// Dosya planı bolum bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOADBOLUM = "EVRDOSYAPLANIBOLUM";

        /// <summary>
        /// Dosya planı bolum bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlarBolum
        {
            KOD = 0,
            HESAPKODU = 1,
            KLASORKODU = 2,
            BOLUMADI = 3,
            DURUMU = 4,
            ACILISTARIHI = 5,
            KAPANISTARIHI = 6,
            TUR = 7,
            SAKLAMAORTAMI = 8,
            FIZIKISAKLAMAYERI = 9
        }

        /// <summary>
        /// Dosya planı tarihçe bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLARBOLUM = { "KOD", "HESAPKODU", "KLASORKODU", "BOLUMADI", "DURUMU", "ACILISTARIHI", "KAPANISTARIHI", "TUR", "SAKLAMAORTAMI", "FIZIKISAKLAMAYERI" };
        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonuna göre; dosya planı bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public Sonuc Kaydet(DosyaPlani bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string[] st = bilgi.kod.Split('.');
            int seviye = st.Length;

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.ad) + ", ";
            sql = sql + VeriErisim.DegerYap(seviye) + ", ";
            sql = sql + VeriErisim.DegerYap(0) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.kullanilmiyor) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.saklamaSuresi) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.saklamaKriteri) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.islemKod) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.sureBaslangicTipi) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.tasfiyePlanTipi) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.gerekce) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.gizlilik) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.anahtarKelime) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.acilisTarihi) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.kapanisTarihi) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.adTipi) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.saklamaPlanTipi) + ")";
            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMKOD], bilgi.islemKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANILMIYOR], bilgi.kullanilmiyor) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAKLAMASURESI], bilgi.saklamaSuresi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAKLAMAKRITERI], bilgi.saklamaKriteri) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASFIYEPLANTIPI], bilgi.tasfiyePlanTipi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SUREBASLANGICTIPI], bilgi.sureBaslangicTipi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCE], bilgi.gerekce) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIZLILIK], bilgi.gizlilik) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAHTARKELIME], bilgi.anahtarKelime) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACILISTARIHI], bilgi.acilisTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAPANISTARIHI], bilgi.kapanisTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADTIPI], bilgi.adTipi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAKLAMAPLANTIPI], bilgi.saklamaPlanTipi);
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod);

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen dosyaPlani nesnesine göre; dosya planı bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public Sonuc Sil(DosyaPlani bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod);

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
                return sonuc;

            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen dosyaPlani nesnesine göre; dosya planı bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Dosya planı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(DosyaPlani bilgi)
        {
            string kod = bilgi.kod.Trim() + "%";

            //string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",EVRDOSYAPLANIBOLUMTANIM.BOLUMADI" + " FROM " + TABLOAD;
            //sql += " LEFT JOIN EVRDOSYAPLANIBOLUMTANIM ON EVRDOSYAPLANIBOLUMTANIM.HESAPKODU=EVRDOSYAPLANI.KOD";

            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.aramadaKendisiniGetir) kod = bilgi.kod.Trim();

            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], " LIKE ", kod));

            if (bilgi.tur == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "NOT LIKE ", "P%" + bilgi.ad + "%"));
            if (bilgi.ad != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + bilgi.ad + "%"));
            if (bilgi.seviye != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SEVIYE], "=", bilgi.seviye));
            if (bilgi.detay == true)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DETAY], "=", 0));
            if (bilgi.kullanilmiyor > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANILMIYOR], "=", bilgi.kullanilmiyor));
            if (bilgi.saklamaSuresi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAKLAMASURESI], "=", bilgi.saklamaSuresi));
            if (bilgi.saklamaKriteri > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAKLAMAKRITERI], "=", bilgi.saklamaKriteri));
            if (bilgi.sureBaslangicTipi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SUREBASLANGICTIPI], "=", bilgi.sureBaslangicTipi));
            if (bilgi.tasfiyePlanTipi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TASFIYEPLANTIPI], "=", bilgi.tasfiyePlanTipi));
            if (bilgi.gizlilik > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GIZLILIK], "=", bilgi.gizlilik));
            if (bilgi.adTipi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ADTIPI], "=", bilgi.adTipi));
            if (bilgi.saklamaPlanTipi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAKLAMAPLANTIPI], "=", bilgi.saklamaPlanTipi));
            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen DosyaPlani nesne koleksiyonuna göre; dosya planı bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kod">The kod.</param>
        /// <param name="kolon">The kolon.</param>
        /// <param name="eskiDeger">The eski deger.</param>
        /// <param name="yeniDeger">The yeni deger.</param>
        /// <param name="islem">The islem.</param>
        /// <param name="kullaniciKodu">The kullanici kodu.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public Sonuc KaydetTarihce(string kod, ENUMAlanlar kolon, string eskiDeger, string yeniDeger, ENUMDosyaPlaniTarihceIslem islem, string kullaniciKodu)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(System.Guid.NewGuid().ToString()) + ", ";
            sql = sql + VeriErisim.DegerYap(kod) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ", ";
            sql = sql + VeriErisim.DegerYap(kolon.ToString()) + ", ";
            sql = sql + VeriErisim.DegerYap(eskiDeger) + ", ";
            sql = sql + VeriErisim.DegerYap(yeniDeger) + ", ";
            sql = sql + VeriErisim.DegerYap(islem.ToString()) + ",";
            sql = sql + VeriErisim.DegerYap(kullaniciKodu) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Paramete olarak verilen dosyaPlani nesnesine göre; dosya planı bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="tarihce">The tarihce.</param>
        /// <returns>
        /// İşlem sonucu DataReader olarak döndürülür.
        /// </returns>
        public IDataReader ListeleTarihce(DosyaPlaniTarihce tarihce)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTARIHCE, ALANLARTARIHCE) + " FROM " + TABLOADTARIHCE;

            string kosul = string.Empty;
            if (!string.IsNullOrWhiteSpace(tarihce.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.KOD], "=", tarihce.kod));
            if (!string.IsNullOrWhiteSpace(tarihce.kolon))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.KOLON], "=", tarihce.kolon));
            if (!tarihce.baslangicTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.TARIH], ">=", tarihce.baslangicTarih));
            if (!tarihce.bitisTarih.isNull)
            {
                DateTime d2 = Convert.ToDateTime(tarihce.bitisTarih.ToString()).AddDays(1).AddSeconds(-1);
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.TARIH], "<=", new TNSDateTime(d2)));
            }

            if (!string.IsNullOrWhiteSpace(kosul))
            {
                sql += " WHERE " + kosul;
            }

            sql += " ORDER BY " + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.KOD] + "," + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.TARIH] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleDosyaPlaniEvrak(EvrakSinif evrak)
        {
            string sql = " SELECT EVREVRAKDOSYAPLANI.DOSYAPLANKODU,EVRDOSYAPLANI.AD,EVREVRAK.EVRAKNO,EVREVRAK.EVRAKBIRIMKODU,EVREVRAK.KONU,EVREVRAK.EVRAKTIPI,EVREVRAK.GELDIGIYER,EVREVRAK.GELENEVRAKTARIH,EVREVRAKHAREKETSONDURUM.ISLEM ";
            sql += "FROM " + TABLOAD;
            sql += " INNER JOIN EVREVRAKDOSYAPLANI ON EVREVRAKDOSYAPLANI.DOSYAPLANKODU=EVRDOSYAPLANI.KOD ";
            sql += "INNER JOIN EVREVRAKSICIL ON EVREVRAKSICIL.PREVRAKNO=EVREVRAKDOSYAPLANI.PREVRAKNO ";
            sql += "INNER JOIN EVREVRAK ON EVREVRAK.EVREVRAKNO=EVREVRAKSICIL.EVREVRAKNO ";
            sql += "INNER JOIN EVREVRAKHAREKETSONDURUM ON EVREVRAKHAREKETSONDURUM.PREVRAKNO=EVREVRAKSICIL.PREVRAKNO ";
            string kosul = string.Empty;

            if (!evrak.hareket.islemTarih.isNull)
            {
                if (evrak.hareket.sorguIslemTarih.isNull)
                    evrak.hareket.sorguIslemTarih = new TNSDateTime(evrak.hareket.islemTarih.Oku());//Tarih alanında saat bilgisi olduğu için aralık sorgusu yapılması gerekli
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", "ISLEMTARIH", ">=", evrak.hareket.islemTarih));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", "ISLEMTARIH", "<=", new TNSDateTime(evrak.hareket.sorguIslemTarih.Oku().AddDays(1))));//tarih alanında saat bilgisi olduğu için içinde bulunduğumuz günü alması için
            }

            //if (evrak.dosyaKodlari.Count > 0)
            //{
            //    //string kosulDp = "";
            //    foreach (DosyaPlani dp in evrak.dosyaKodlari)
            //        // kosulDp = VeriErisim.KosulEkle(kosulDp, ",", "'" + dp.kod + "'");
            //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKDOSYAPLANI", "DOSYAPLANKODU", "=", dp.kod));
            //}

            if (!string.IsNullOrEmpty(evrak.hareket.birimKod1))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", "BIRIMKOD1", "LIKE", evrak.hareket.birimKod1 + "%"));

            if (evrak.dosyaKodlari.Count > 0)
            {
                string kosulDp = "";
                foreach (DosyaPlani dp in evrak.dosyaKodlari)
                    kosulDp = VeriErisim.KosulEkle(kosulDp, ",", "'" + dp.kod + "'");

                if (kosulDp != "")
                    kosul += " AND (" + "EVREVRAKHAREKETSONDURUM.PREVRAKNO IN (SELECT DISTINCT PREVRAKNO FROM EVREVRAKDOSYAPLANI WHERE (DOSYAPLANKODU IN (" + kosulDp + ") ) )" + ")";
            }

            kosul = "( EVREVRAKHAREKETSONDURUM.ISLEM=60 AND " + kosul + ") ";

            if (!string.IsNullOrWhiteSpace(kosul))
            {
                sql += " WHERE " + kosul;
            }

            sql += " ORDER BY EVREVRAKDOSYAPLANI.DOSYAPLANKODU";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleKlasorBolum(DosyaPlaniBolum bolum)
        {
            string sql = "SELECT EVRDOSYAPLANI.KOD, EVRDOSYAPLANI.AD, EVRDOSYAPLANIBOLUM.TUR, EVRDOSYAPLANIBOLUM.KOD AS KLASORBOLUMKOD, EVRDOSYAPLANIBOLUM.BOLUMADI ,EVRDOSYAPLANIBOLUM.ACILISTARIHI, EVRDOSYAPLANIBOLUM.KAPANISTARIHI,EVRDOSYAPLANIBOLUM.DURUMU, EVRDOSYAPLANIBOLUM.KLASORKODU, EVRDOSYAPLANIBOLUM.SAKLAMAORTAMI, EVRDOSYAPLANIBOLUM.FIZIKISAKLAMAYERI ";
            sql += "FROM " + TABLOAD;
            sql += " INNER JOIN EVRDOSYAPLANIBOLUM ON EVRDOSYAPLANI.KOD = EVRDOSYAPLANIBOLUM.HESAPKODU ";
            string kosul = string.Empty;
            if (!string.IsNullOrWhiteSpace(bolum.hesapKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBOLUM, ALANLARBOLUM[(int)ENUMAlanlarBolum.HESAPKODU], "=", bolum.hesapKodu));
            if (!bolum.baslangic.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBOLUM, ALANLARBOLUM[(int)ENUMAlanlarBolum.ACILISTARIHI], ">=", bolum.baslangic));
            if (!bolum.bitis.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBOLUM, ALANLARBOLUM[(int)ENUMAlanlarBolum.KAPANISTARIHI], "<=", bolum.bitis));
            if (bolum.durumu > 0)
            {
                if (bolum.durumu == 2) bolum.durumu = 0;//Kapalı olanların listelenmesi için sorgu kriteri 2 gönderiliyor.
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBOLUM, ALANLARBOLUM[(int)ENUMAlanlarBolum.DURUMU], "=", bolum.durumu));
            }

            if (!string.IsNullOrWhiteSpace(kosul))
            {
                sql += " WHERE " + kosul;
            }

            sql += " ORDER BY EVRDOSYAPLANI.KOD, EVRDOSYAPLANIBOLUM.ACILISTARIHI, EVRDOSYAPLANIBOLUM.KAPANISTARIHI";

            return veriErisim.DataReaderDondur(sql);
        }

        //public IDataReader ListeleDosyaTarihceSayisi(DosyaPlaniTarihce tarihce)
        //{
        //    string sql = "select EVRDOSYAPLANITARIHCE.KOD,count(case when EVRDOSYAPLANITARIHCE.ISLEM = 'YENIKAYIT' THEN EVRDOSYAPLANITARIHCE.ISLEM ELSE null END) as acilanDosya,count(case when EVRDOSYAPLANITARIHCE.ISLEM = 'SIL'  THEN EVRDOSYAPLANITARIHCE.ISLEM ELSE null END) as kapananDosya "
        //        + " FROM " + TABLOADTARIHCE;
        //    string kosul = string.Empty;
        //    if (!string.IsNullOrWhiteSpace(tarihce.kod))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.KOD], "=", tarihce.kod));
        //    if (!string.IsNullOrWhiteSpace(tarihce.kolon))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.KOLON], "=", tarihce.kolon));
        //    if (!tarihce.baslangicTarih.isNull)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.TARIH], ">=", tarihce.baslangicTarih));
        //    if (!tarihce.bitisTarih.isNull)
        //    {
        //        DateTime d2 = Convert.ToDateTime(tarihce.bitisTarih.ToString()).AddDays(1).AddSeconds(-1);
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.TARIH], "<=", new TNSDateTime(d2)));
        //    }
        //    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.ISLEM], "=", "YENIKAYIT"));
        //    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.ISLEM], "=", "SIL") + ")"); //İPTAL
        //    if (!string.IsNullOrWhiteSpace(kosul))
        //    {
        //        sql += " WHERE " + kosul;
        //    }

        //    sql += " GROUP BY " + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.KOD];
        //    sql += " ORDER BY " + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.KOD];

        //    return veriErisim.DataReaderDondur(sql);
        //}

        public Sonuc KaydetBolumTanim(DosyaPlaniBolumTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "INSERT INTO " + TABLOADBOLUMTANIM + " (" + VeriErisim.TumAlanlar("", ALANLARBOLUMTANIM) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.hesapKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.bolumAdi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.durumu) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.periyod) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.baslangicGun) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.bitisGun) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.baslangicAy) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.bitisAy) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.sonIslem) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.tur) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.saklamaOrtami) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.fizikiSaklamaYeri) + ")";
            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOADBOLUMTANIM + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.BOLUMADI], bilgi.bolumAdi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.DURUMU], bilgi.durumu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.PERIYOD], bilgi.periyod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.BASLANGICGUN], bilgi.baslangicGun) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.BITISGUN], bilgi.bitisGun) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.BASLANGICAY], bilgi.baslangicAy) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.BITISAY], bilgi.bitisAy) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.SAKLAMAORTAMI], bilgi.saklamaOrtami) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.FIZIKISAKLAMAYERI], bilgi.fizikiSaklamaYeri);
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.HESAPKODU], "=", bilgi.hesapKodu);
                sql += " AND " + VeriErisim.KosulOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.TUR], "=", bilgi.tur);

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc KaydetBolumTanimSonIslem(DosyaPlaniBolumTanim bilgi)
        {
            string sql = "UPDATE " + TABLOADBOLUMTANIM + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.SONISLEM], bilgi.sonIslem);
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.HESAPKODU], "=", bilgi.hesapKodu);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.TUR], "=", bilgi.tur);

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleBolumTanim(DosyaPlaniBolumTanim bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBOLUMTANIM, ALANLARBOLUMTANIM) + " FROM " + TABLOADBOLUMTANIM;

            string kosul = string.Empty;
            if (!string.IsNullOrWhiteSpace(bilgi.hesapKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBOLUMTANIM, ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.HESAPKODU], "=", bilgi.hesapKodu));

            if (bilgi.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBOLUMTANIM, ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.TUR], "=", bilgi.tur));

            if (!string.IsNullOrWhiteSpace(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARBOLUMTANIM[(int)ENUMAlanlarBolumTanim.HESAPKODU];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleBolum(DosyaPlaniBolum bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBOLUM, ALANLARBOLUM) + " FROM " + TABLOADBOLUM;

            string kosul = string.Empty;
            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBOLUM, ALANLARBOLUM[(int)ENUMAlanlarBolum.KOD], "=", bilgi.kod));
            if (!string.IsNullOrEmpty(bilgi.hesapKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBOLUM, ALANLARBOLUM[(int)ENUMAlanlarBolum.HESAPKODU], "=", bilgi.hesapKodu));
            if (bilgi.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBOLUM, ALANLARBOLUM[(int)ENUMAlanlarBolum.TUR], "=", bilgi.tur));
            if (bilgi.klasorKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBOLUM, ALANLARBOLUM[(int)ENUMAlanlarBolum.KLASORKODU], "=", bilgi.klasorKod));

            if (!string.IsNullOrWhiteSpace(kosul))
            {
                sql += " WHERE " + kosul;
            }

            sql += " ORDER BY " + ALANLARBOLUM[(int)ENUMAlanlarBolum.ACILISTARIHI];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetBolum(DosyaPlaniBolum bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            if (bilgi.kod == 0)
            {
                bilgi.kod = this.KodAl();

                sql = "INSERT INTO " + TABLOADBOLUM + " (" + VeriErisim.TumAlanlar("", ALANLARBOLUM) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.hesapKodu) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.klasorKod) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.bolumAdi) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.durumu) + ",";
                sql = sql + VeriErisim.DegerYap(bilgi.baslangic) + ",";
                sql = sql + VeriErisim.DegerYap(bilgi.bitis) + ",";
                sql = sql + VeriErisim.DegerYap(bilgi.tur) + ",";
                sql = sql + VeriErisim.DegerYap(bilgi.saklamaOrtami) + ",";
                sql = sql + VeriErisim.DegerYap(bilgi.fizikiSaklamaYeri) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            else
            {
                if (!sonuc.islemSonuc)
                {
                    sql = "UPDATE " + TABLOADBOLUM + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.BOLUMADI], bilgi.bolumAdi) + ", ";
                    sql += VeriErisim.AtamaOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.DURUMU], bilgi.durumu) + ", ";
                    sql += VeriErisim.AtamaOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.ACILISTARIHI], bilgi.baslangic) + ", ";
                    sql += VeriErisim.AtamaOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.KAPANISTARIHI], bilgi.bitis) + ", ";
                    sql += VeriErisim.AtamaOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.TUR], bilgi.tur) + ", ";
                    sql += VeriErisim.AtamaOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.SAKLAMAORTAMI], bilgi.saklamaOrtami) + ", ";
                    sql += VeriErisim.AtamaOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.FIZIKISAKLAMAYERI], bilgi.fizikiSaklamaYeri);
                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.KOD], "=", bilgi.kod);

                    sonuc = KomutCalistir(sql, 1);
                }

            }
            return sonuc;
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlarBolum.KOD] + ")  FROM " + TABLOADBOLUM;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        public void AktiflikGuncelle(DosyaPlaniBolum bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            if (bilgi.durumu == 1) // kayidi aktif etmek istiyorsak 
            { //aynı koda ait aktif kayıf varmı
                sql = "SELECT COUNT(*) FROM " + TABLOADBOLUM;
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.HESAPKODU], "=", bilgi.hesapKodu);
                sql += " AND " + VeriErisim.KosulOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.TUR], "=", bilgi.tur);
                sql += " AND DURUMU=1";

                if (veriErisim.TekKolonSQLInt(sql) > 0) //varsa hepsinipasife çekiyoruz
                {
                    sql = "UPDATE " + TABLOADBOLUM + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.DURUMU], 0) + " ";
                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.HESAPKODU], "=", bilgi.hesapKodu);
                    sql += " AND " + VeriErisim.KosulOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.TUR], "=", bilgi.tur);

                    sonuc = KomutCalistir(sql, 1);
                }
            }
        }

        public int TarihAraligiKontrolEt(DosyaPlaniBolum bilgi)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOADBOLUM;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.ACILISTARIHI], "<=", bilgi.bitis);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.KAPANISTARIHI], ">=", bilgi.baslangic);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.HESAPKODU], "=", bilgi.hesapKodu);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.TUR], "=", bilgi.tur);
            if (bilgi.kod != 0)//güncelleme
                sql += " AND " + VeriErisim.KosulOl("", ALANLARBOLUM[(int)ENUMAlanlarBolum.KOD], "<>", bilgi.kod);
            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Paramete olarak verilen dosyaPlani nesnesine göre; dosya planının olup olmadığını, sql cümlesi hazırlayıp çalıştırarak veritabanından sorgulayan yordam.
        /// </summary>
        /// <param name="bilgi">Dosya planı bilgisini içeren nesne</param>
        /// <param name="seciliKod">Dosya planı bilgisini içeren nesne</param>
        /// <returns>Dosya planı varsa işlem sonucu Doğru, yoksa Yanlış döndürülür.</returns>
        public bool VarMi(DosyaPlani bilgi, string seciliKod)
        {

            string kosul = "";
            string kosul2 = "";
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;

            if (bilgi.islemKod > 0)
                kosul2 = VeriErisim.KosulEkle(kosul2, "", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMKOD], "<>", bilgi.islemKod));

            if (bilgi.ad.Trim() != "")
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "=", bilgi.ad.Trim()));

            kosul = VeriErisim.KosulEkle(kosul, " OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod.Trim()));

            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", "(" + kosul + ")");

            if (kosul2 != "")
                sql = VeriErisim.KosulEkle(sql, "AND", kosul2);

            if (!string.IsNullOrEmpty(seciliKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "<>", seciliKod));
            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        /// <summary>
        /// Paramete olarak verilen dosya planı kodunun daha önce kullanılıp kullanılmadığını, sql cümlesi hazırlayıp çalıştırarak veritabanından sorgulayan yordam.
        /// </summary>
        /// <param name="kod">Dosya planı kod bilgisini içeren nesne</param>
        /// <returns>Dosya planı kodu kullanılmış ise işlem sonucu Doğru, kullanılmamış ise Yanlış döndürülür.</returns>
        public bool KullanilmisMi(string kod)
        {
            string sql = "SELECT COUNT(*) FROM EVREVRAKDOSYAPLANI ";
            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl("", "DOSYAPLANKODU", "=", kod.Trim()));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        /// <summary>
        /// Parametre olarak verilen dosya planı kodunu detayAra kriterine göre, sql cümlesi harızlayıp çalıştırarak veritabanından detay kayıtlarını sorgulayan yordam.
        /// </summary>
        /// <param name="kod">Dosya planı kod bilgisini içeren nesne</param>
        /// <param name="detayAra">Detay arama durumu bilgisini içeren nesne</param>
        /// <returns>Detay kayıtları varsa işlem sonucu Doğru, yoksa Yanlış döndürülür.</returns>
        public bool AnasiCocuguVarMi(string kod, bool detayAra)
        {
            string sql = "";
            bool varMi = false;

            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            if (detayAra)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", kod + "%"));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            IDataReader r = veriErisim.DataReaderDondur(sql);

            if (veriErisim.hata == "")
                if (r.Read())
                    if (detayAra)
                        varMi = VeriErisim.DegerAl(r, 0, 0) >= 2;
                    else
                        varMi = VeriErisim.DegerAl(r, 0, 0) >= 1;

            r.Close();

            return varMi;
        }

        /// <summary>
        /// Paramete olarak verilen dosya planı kodu bilgisine göre; dosya planı bilgisinin detay seviyelerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kod">Dosya planı kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc DetaySayilariDuzenle(string kod)
        {
            string sql;
            string bulKod = "";
            Sonuc sonuc = new Sonuc(false, "", "");
            bulKod = kod;

            if (kod.LastIndexOf('.') > 0)
                bulKod = kod.Substring(0, kod.LastIndexOf('.'));

            if (bulKod.Trim() != "")
            {
                sql = "UPDATE " + TABLOAD;
                sql += " SET DETAY=(SELECT COUNT(*) FROM " + TABLOAD + " B";
                sql += " WHERE B.KOD LIKE " + TABLOAD + ".KOD" + VeriErisim.IslecBul("+") + "'%' AND ";
                sql += " B.SEVIYE>" + TABLOAD + ".SEVIYE AND KOD LIKE '" + VeriErisim.DegerYap(bulKod) + "%' AND KULLANILMIYOR=1) WHERE KOD LIKE '" + VeriErisim.DegerYap(bulKod) + "%'";
            }
            else
            {
                sql = "UPDATE " + TABLOAD;
                sql += " SET DETAY=(SELECT COUNT(*) FROM " + TABLOAD + " B";
                sql += " WHERE B.KOD LIKE " + TABLOAD + ".KOD" + VeriErisim.IslecBul("+") + "'%' AND ";
                sql += " B.SEVIYE>" + TABLOAD + ".SEVIYE AND KULLANILMIYOR=1)";
            }
            sonuc = KomutCalistir(sql, 0);

            sql = "UPDATE " + TABLOAD + " SET SEVIYE=1 WHERE LEN(KOD)=3";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=2 WHERE LEN(KOD)=6";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=3 WHERE LEN(KOD)=9";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=4 WHERE LEN(KOD)=12";
            sonuc = KomutCalistir(sql, 0);
            sql = "UPDATE " + TABLOAD + " SET SEVIYE=5 WHERE LEN(KOD)=15";
            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen dosya planı kodu bilgisine göre; dosya planı bilgilerini kullanılmıyor olarak, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kod">Dosya planı kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc KullanilmiyorYap(string kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "UPDATE " + TABLOAD + " SET KULLANILMIYOR=2";
            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }
    }
}
