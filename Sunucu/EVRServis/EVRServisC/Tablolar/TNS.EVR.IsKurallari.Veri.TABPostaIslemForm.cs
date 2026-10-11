using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Posta işlem bilgilerini içeren EVRPOSTAISLEMFORM ve EVRPOSTAISLEMFORMDETAY veri tabanı tabloları ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABPostaIslemForm : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABPostaIslemForm sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABPostaIslemForm(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Posta işlem bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRPOSTAISLEMFORM";

        /// <summary>
        /// Posta işlem bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar { EVRAKBIRIMKODU = 0, YIL = 1, BELGENO = 2, BELGETARIH = 3, KULLANICIKODU = 4, KAYITTARIH = 5, KULLANICIADI = 6 }

        /// <summary>
        /// Posta işlem bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "EVRAKBIRIMKODU", "YIL", "BELGENO", "BELGETARIH", "KULLANICIKODU", "KAYITTARIH", "KULLANICIADI" };

        /// <summary>
        /// Posta işlem detay bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        static string TABLOADDETAY = "EVRPOSTAISLEMFORMDETAY";

        /// <summary>
        /// Posta işlem detay bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlarDetay { EVRAKBIRIMKODU = 0, YIL = 1, BELGENO = 2, SIRANO = 3, EVREVRAKNO = 4, EVRAKNO = 5, GIDECEGIYER = 6, POSTATUR = 7, FIYATNORMAL = 8, FIYATPOSTANE = 9, BARKOD = 10, SAYFANO = 11, ZARFNO = 12 }

        /// <summary>
        /// Posta işlem detay bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLARDETAY = { "EVRAKBIRIMKODU", "YIL", "BELGENO", "SIRANO", "EVREVRAKNO", "EVRAKNO", "GIDECEGIYER", "POSTATUR", "FIYATNORMAL", "FIYATPOSTANE", "BARKOD", "SAYFANO", "ZARFNO" };

        /// <summary>
        /// TABPostaIslemForm sınıfına ait, sql cümlesini tutan değişken
        /// </summary>
        private string sql = "";

        /// <summary>
        /// TABPostaIslemForm sınıfına ait, sonuç bilgisini tutan nesne
        /// </summary>
        private Sonuc sonuc = new Sonuc(true, "", "");

        /// <summary>
        /// TABPostaIslemForm sınıfına ait, yeni kayıt durumunu tutan değişken
        /// </summary>
        private bool yeniKayit = false;

        /// <summary>
        /// TABPostaIslemForm sınıfına ait, Posta işlem bilgisini tutan nesne
        /// </summary>
        private PostaIslem form = new PostaIslem();

        /// <summary>
        /// Yeni kayıt durumunu tutan özellik
        /// </summary>
        public bool YeniKayit { get { return yeniKayit; } set { yeniKayit = value; } }

        /// <summary>
        /// Posta işlem durumunu tutan özellik
        /// </summary>
        public PostaIslem Form { get { return form; } set { form = value; } }

        /// <summary>
        /// Posta işlem bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>        
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet()
        {
            if (yeniKayit)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.evrakBirimKodu) + ",";
                sql += VeriErisim.DegerYap(form.yil) + ",";
                sql += VeriErisim.DegerYap(form.belgeNo) + ",";
                sql += VeriErisim.DegerYap(form.belgeTarih) + ",";
                sql += VeriErisim.DegerYap(form.kullaniciKodu) + ",";
                sql += VeriErisim.DegerYap(form.kayitTarih) + ",";
                sql += VeriErisim.DegerYap(form.kullaniciAdi) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            else
            {
                sonuc = SilDetay();

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIH], form.belgeTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITTARIH], form.kayitTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], form.kullaniciKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIADI], form.kullaniciAdi);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", (form.evrakBirimKodu)));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (form.yil)));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", (form.belgeNo)));

                if (sonuc.islemSonuc)
                    sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Posta işlem bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil()
        {
            sonuc = SilDetay();
            if (sonuc.islemSonuc)
            {
                sql = "DELETE FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", form.evrakBirimKodu));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", form.belgeNo));

                return KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        ///  Posta işlem bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>      
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(bool detay)
        {
            string kosul = "";
            string[] tmpALANLARDETAY = new string[ALANLARDETAY.Length - 3];
            for (int i = 3; i < ALANLARDETAY.Length; i++) { tmpALANLARDETAY[i - 3] = ALANLARDETAY[i]; }

            sql = "SELECT ";

            if (detay)
            {
                sql += VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
                sql += " , " + VeriErisim.TumAlanlar(TABLOADDETAY, tmpALANLARDETAY) + ", EVREVRAK.KONU, EVREVRAK.EVRAKBIRIMKODU, EVRBIRIM.AD, EVREVRAK.EVRAKTARIH, EVRBIRIM.DAGITIMBIRIMKOD, EVRBIRIM2.AD, EVREVRAK.BASKANLIKEVRAKNO, EVREVRAK.BAKANLIKEVRAKNO, EVREVRAK.EK, EVREVRAK.ACIKLAMA FROM ";
                sql += TABLOAD + "," + TABLOADDETAY + ", EVREVRAK, EVRBIRIM LEFT OUTER JOIN EVRBIRIM AS EVRBIRIM2 ON EVRBIRIM.DAGITIMBIRIMKOD = EVRBIRIM2.KOD ";

                sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.EVRAKBIRIMKODU];
                sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL];
                sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGENO] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.BELGENO];
                sql += " AND " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.EVREVRAKNO] + "=" + "EVREVRAK" + "." + "EVREVRAKNO";
                sql += " AND " + "EVREVRAK.EVRAKBIRIMKODU" + "=" + "EVRBIRIM.KOD";
            }
            else
                sql += VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (form.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            if (!string.IsNullOrEmpty(form.evrakBirimKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "LIKE", form.evrakBirimKodu));
            if (form.belgeNo > 0)
            {
                if (form.sorguBelgeNo == 0)
                    form.sorguBelgeNo = form.belgeNo;
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], ">=", form.belgeNo));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "<=", form.sorguBelgeNo));
            }
            if (!form.belgeTarih.isNull)
            {
                if (form.sorguBelgeTarih.isNull)
                    form.sorguBelgeTarih = new TNSDateTime(form.belgeTarih.Oku());
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETARIH], ">=", form.belgeTarih));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETARIH], "<=", form.sorguBelgeTarih));
            }
            if (form.detay.Count > 0)
            {
                PostaIslemDetay d = (PostaIslemDetay)form.detay[0];
                if (d.evrakNo > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.EVRAKNO], "=", d.evrakNo));
                if (d.postaTur > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.POSTATUR], "=", d.postaTur));
                if (!string.IsNullOrEmpty(d.gidecegiYer))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.GIDECEGIYER], " LIKE ", "%" + d.gidecegiYer + "%"));
            }

            if (kosul != "")
            {
                if (detay)
                    sql += " AND " + kosul;
                else
                    sql += " WHERE " + kosul;
            }
            sql += " ORDER BY 1,2 DESC,3 DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Posta işlem detay bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>        
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc KaydetDetay(PostaIslemDetay detay)
        {
            sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.evrakBirimKodu) + ",";
            sql += VeriErisim.DegerYap(form.yil) + ",";
            sql += VeriErisim.DegerYap(form.belgeNo) + ",";
            sql += VeriErisim.DegerYap(detay.siraNo) + ",";
            sql += VeriErisim.DegerYap(detay.evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(detay.evrakNo) + ",";
            sql += VeriErisim.DegerYap(detay.gidecegiYer) + ",";
            sql += VeriErisim.DegerYap(detay.postaTur) + ",";
            sql += VeriErisim.DegerYap(detay.fiyatNormal) + ",";
            sql += VeriErisim.DegerYap(detay.fiyatPostane) + ",";
            sql += VeriErisim.DegerYap(detay.barkod) + ",";
            sql += VeriErisim.DegerYap(detay.sayfaNo) + ",";
            sql += VeriErisim.DegerYap(detay.zarfNo) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Posta işlem detay bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        private Sonuc SilDetay()
        {
            sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.EVRAKBIRIMKODU], "=", form.evrakBirimKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.BELGENO], "=", form.belgeNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        ///  Posta işlem detay bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>      
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader ListeleEvrak()
        {
            string kosul = "";
            sql = "SELECT EVREVRAK.EVREVRAKNO, EVREVRAK.EVRAKNO, EVREVRAK.GELDIGIYER, EVREVRAK.EK, EVREVRAKDAGITIM.DAGITIMAD, EVREVRAK.GIZLILIK, EVRGIZLILIK.AD, EVRDAGITIMDETAY.YER AS DAGITIMAD2 " +
                  "FROM EVREVRAK INNER JOIN " +
                  "EVREVRAKSICIL ON EVREVRAK.EVREVRAKNO = EVREVRAKSICIL.EVREVRAKNO LEFT OUTER JOIN " +
                  "EVRGIZLILIK ON EVREVRAK.GIZLILIK = EVRGIZLILIK.KOD LEFT OUTER JOIN " +
                  "EVREVRAKDAGITIM ON EVREVRAKSICIL.PREVRAKNO = EVREVRAKDAGITIM.PREVRAKNO " +
                  "LEFT OUTER JOIN EVRDAGITIMDETAY ON EVRDAGITIMDETAY.DAGITIMKODU LIKE EVREVRAKDAGITIM.DAGITIMKOD ";

            sql += " WHERE (NOT (EVREVRAK.EVREVRAKNO IN ";
            sql += "(SELECT EVREVRAKNO FROM " + TABLOADDETAY;
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "LIKE", form.evrakBirimKodu));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql += " WHERE (" + kosul;
            sql += "))))";
            kosul = "";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKBIRIMKODU], "LIKE", form.evrakBirimKodu));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.YIL], "=", form.yil));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKTIPI], "=", (int)ENUMEvrakTipi.GIDEN));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKNO], "<>", 0));
            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKTURU], "=", (int)ENUMGidenEvrakTuru.DISYAZISMA));
            if (!form.evrakTarih1.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKTARIH], ">=", form.evrakTarih1));
            if (!form.evrakTarih2.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKTARIH], "<", form.evrakTarih2));
            if (form.sorguEvrakTuru > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKTURU], "=", form.sorguEvrakTuru));

            sql += " AND (" + kosul + ")";

            sql += " ORDER BY 2,3";
            return veriErisim.DataReaderDondur(sql);

            //sql = "SELECT ";
            //sql += TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVREVRAKNO] + "," + TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKNO] + "," + TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.GELDIGIYER] + "," + TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EK] + " FROM " + TABEvrak.TABLOAD;

            //sql += " WHERE (NOT (" + TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVREVRAKNO] + " IN ";
            //sql += "(SELECT " + TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVREVRAKNO] + " FROM " + TABLOADDETAY;
            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "LIKE", form.evrakBirimKodu));
            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));

            //sql += " WHERE (" + kosul;
            //sql += "))))";
            //kosul = "";
            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKBIRIMKODU], "LIKE", form.evrakBirimKodu));
            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.YIL], "=", form.yil));

            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKTIPI], "=", (int)ENUMEvrakTipi.GIDEN));
            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKNO], "<>", 0));
            ////kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKTURU], "=", (int)ENUMGidenEvrakTuru.DISYAZISMA));
            //if (!form.evrakTarih1.isNull)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKTARIH], ">=", form.evrakTarih1));
            //if (!form.evrakTarih2.isNull)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABEvrak.TABLOAD, TABEvrak.ALANLAR[(int)TABEvrak.ENUMAlanlar.EVRAKTARIH], "<", form.evrakTarih2));
            //sql += " AND (" + kosul + ")";

            //sql += " ORDER BY 2,3";
            //return veriErisim.DataReaderDondur(sql);
        }
    }
}
