using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHMAKBUZ veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMakbuz : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMakbuz(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// create table MUHMAKBUZ
        /// (
        /// 	YIL INT, 
        /// 	MUHASEBEKOD INT, 
        /// 	YEVMIYENO VARCHAR(200),
        /// 	BIRIMKOD VARCHAR(20),
        /// 	MAKBUZTIPIKOD INT,
        /// 	KISIAD varchar(200),
        /// 	MAKBUZTIPITUTAR MONEY,
        /// 	ACIKLAMA varchar(200), 
        /// 	DUZENLEYEN varchar(200), 
        /// 	UNVAN varchar(200),
        /// 	ISLEMTARIH DATETIME, 
        /// 	DURUM INT, 
        /// 	ODEMEEMRINO VARCHAR(20), 
        /// 	KULLANICIKODU VARCHAR(200),
        /// 	TCKIMLIKVERGINO VARCHAR(20),
        /// 	KREDIKARTIODEME money,
        /// 	primary key (YIL, MUHASEBEKOD, YEVMIYENO)
        /// )
        /// </summary>
        static string TABLOAD = "MUHMAKBUZ";

        enum ENUMAlanlar
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            YEVMIYENO = 2,
            BIRIMKOD = 3,
            MAKBUZTIPIKOD = 4,
            KISIAD = 5,
            MAKBUZTIPITUTAR = 6,
            ACIKLAMA = 7,
            DUZENLEYEN = 8,
            UNVAN = 9,
            ISLEMTARIH = 10,
            DURUM = 11,
            ODEMEEMRINO = 12,
            KULLANICIKODU = 13,
            TCKIMLIKVERGINO = 14,
            KREDIKARTIODEME = 15,
            PARABIRIMIKOD = 16,

        }
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "YEVMIYENO", "BIRIMKOD", "MAKBUZTIPIKOD", "KISIAD", "MAKBUZTIPITUTAR",
                                    "ACIKLAMA", "DUZENLEYEN", "UNVAN", "ISLEMTARIH", "DURUM", "ODEMEEMRINO", "KULLANICIKODU", "TCKIMLIKVERGINO", "KREDIKARTIODEME","PARABIRIMIKOD" };

        public static string TABLOADACIKLAMA = "MUHMAKBUZACIKLAMA";
        public static string[] ALANLARACIKLAMA = { "KOD", "BIRIMKOD", "ACIKLAMA", "MAKBUZTIPIKOD" };
        public enum ENUMAlanlarAciklama
        {
            KOD = 0,
            BIRIMKOD = 1,
            ACIKLAMA = 2,
            MAKBUZTIPIKOD = 3
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="makbuz">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(Makbuz makbuz)
        {
            Sonuc sonuc = new Sonuc();

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(makbuz.yil) + ", ";
            sql += VeriErisim.DegerYap(makbuz.muhasebe) + ", ";
            sql += VeriErisim.DegerYap(makbuz.yevmiyeNo) + ", ";
            sql += VeriErisim.DegerYap(makbuz.birim) + ", ";
            sql += VeriErisim.DegerYap(makbuz.makbuzTipi.kod) + ", ";
            sql += VeriErisim.DegerYap(makbuz.kisiAd) + ", ";
            sql += VeriErisim.DegerYap(makbuz.makbuzTipiTutar) + ", ";
            sql += VeriErisim.DegerYap(makbuz.aciklama) + ", ";
            sql += VeriErisim.DegerYap(makbuz.duzenleyen) + ", ";
            sql += VeriErisim.DegerYap(makbuz.unvan) + ", ";
            sql += VeriErisim.DegerYap(makbuz.islemTarih) + ", ";
            sql += VeriErisim.DegerYap(makbuz.durum) + ", ";
            sql += VeriErisim.DegerYap(makbuz.odemeEmriNo) + ",";
            sql += VeriErisim.DegerYap(makbuz.kullaniciKodu) + ",";
            sql += VeriErisim.DegerYap(makbuz.tcKimlikNoVergiNo) + ",";
            sql += VeriErisim.DegerYap(makbuz.krediKartiIleOdenen) + ",";
            sql += VeriErisim.DegerYap(makbuz.paraBirimiKod) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MAKBUZTIPIKOD], makbuz.makbuzTipi.kod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISIAD], makbuz.kisiAd) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MAKBUZTIPITUTAR], makbuz.makbuzTipiTutar) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], makbuz.aciklama) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], makbuz.birim) + ",";

                if (makbuz.durum > -1)
                {
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], makbuz.durum) + ",";
                }

                if (makbuz.durum == -1)
                {
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], makbuz.islemTarih) + ",";
                }

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMIKOD], makbuz.paraBirimiKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DUZENLEYEN], makbuz.duzenleyen) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVAN], makbuz.unvan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], makbuz.kullaniciKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKVERGINO], makbuz.tcKimlikNoVergiNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KREDIKARTIODEME], makbuz.krediKartiIleOdenen);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", makbuz.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", makbuz.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YEVMIYENO], "=", makbuz.yevmiyeNo));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen bilgiler ile makbuz verisinin durumunu değiştiren sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="makbuz">Makbuz bilgisi.</param>
        /// <returns></returns>
        public Sonuc DurumGuncelle(Makbuz makbuz)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], makbuz.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", makbuz.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", makbuz.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YEVMIYENO], "=", makbuz.yevmiyeNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen bilgiler ile ödeme emri verisini güncelleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="makbuz">Makbuz bilgisi.</param>
        /// <returns></returns>
        public Sonuc OdemeEmriGuncelle(Makbuz makbuz)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], makbuz.durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], makbuz.odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", makbuz.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", makbuz.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YEVMIYENO], "=", makbuz.yevmiyeNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere uyan makbuz verisini getiren sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="makbuz">Makbuz bilgisi.</param>
        /// <returns></returns>
        public int MevcutDurumGetir(Makbuz makbuz)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.DURUM] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", makbuz.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", makbuz.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YEVMIYENO], "=", makbuz.yevmiyeNo));

            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="makbuz">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(Makbuz makbuz)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", MUHMAKBUZTIPI.ACIKLAMA";
            sql += ", MUHMAKBUZTIPI.HESAPKOD";
            sql += ", MUHMAKBUZTIPI.KDV";
            sql += ", MUHMAKBUZTIPI.KDVHESAPKOD";
            sql += ", MUHMAKBUZTIPI.KASAHESAPKOD";
            sql += ", MUHMAKBUZTIPI.YANSITMAHESAPKOD";
            sql += ", MUHMAKBUZTIPI.KREDIHESAPKOD";
            sql += ", MUHMAKBUZHESAP.HESAPKOD";
            sql += ", MUHMAKBUZHESAP.TUTAR";
            sql += ", MUHMAKBUZHESAP.HESAPLANANALAN";
            sql += ", MUHMAKBUZHESAP.ALTHESAP";
            sql += ", MUHHESAPPLANI.ACIKLAMA";
            sql += ", (SELECT MIN(AD) FROM " + TABOrtak.DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCFIRMABILGISI where VERGINO=MUHMAKBUZHESAP.ALTHESAP and MUHMAKBUZHESAP.ALTHESAP<>'')";
            sql += ", (SELECT MIN(AD + ' ' + SOYAD) FROM " + TABOrtak.DatabaseAdBul("Personel.dbo.") + "PERPersonel where TCKIMLIKNO=MUHMAKBUZHESAP.ALTHESAP)";
            sql += ", MUHMAKBUZHESAP.TERTIP";
            sql += " FROM " + TABLOAD;
            sql += " INNER JOIN MUHMAKBUZTIPI ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MAKBUZTIPIKOD] + "=" + "MUHMAKBUZTIPI.KOD";
            sql += " LEFT OUTER JOIN MUHMAKBUZHESAP ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + "MUHMAKBUZHESAP.YIL";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + "MUHMAKBUZHESAP.MUHASEBEKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YEVMIYENO] + "=" + "MUHMAKBUZHESAP.YEVMIYENO";
            sql += " LEFT OUTER JOIN MUHHESAPPLANI ON " + "MUHMAKBUZHESAP" + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + "MUHHESAPPLANI.YIL";
            sql += " AND " + "MUHMAKBUZHESAP" + "." + "HESAPKOD" + "=" + "MUHHESAPPLANI.HESAPKOD";

            string kosul = ListeleKosul(makbuz);

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YEVMIYENO];
            sql += ", MUHMAKBUZHESAP.SATIRNO";

            return veriErisim.DataReaderDondur(sql);
        }

        private string ListeleKosul(Makbuz makbuz)
        {
            string kosul = string.Empty;
            string aramaIsareti = "%";
            if (makbuz.kisiBilgisiTamBul)
                aramaIsareti = "";


            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", makbuz.yil));
            int say = 1;
            if (makbuz.muhasebe > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", makbuz.muhasebe));
                say++;
            }
            if (!string.IsNullOrEmpty(makbuz.yevmiyeNo))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YEVMIYENO], "=", makbuz.yevmiyeNo));
                say++;
            }
            if (say < 3)
            {
                if (!string.IsNullOrEmpty(makbuz.sorguHesapKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMAKBUZHESAP", "HESAPKOD", "=", makbuz.sorguHesapKod.Replace(".", "")));

                if (!string.IsNullOrEmpty(makbuz.birim))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", makbuz.birim));

                if (makbuz.makbuzTipi.kod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MAKBUZTIPIKOD], "=", makbuz.makbuzTipi.kod));

                if (!string.IsNullOrEmpty(makbuz.tcKimlikNoVergiNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKVERGINO], "LIKE", makbuz.tcKimlikNoVergiNo));

                if (!string.IsNullOrEmpty(makbuz.kisiAd))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KISIAD], "LIKE", aramaIsareti + makbuz.kisiAd + aramaIsareti));

                if (!string.IsNullOrEmpty(makbuz.aciklama))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + makbuz.aciklama + "%"));

                if (!string.IsNullOrEmpty(makbuz.duzenleyen))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DUZENLEYEN], "LIKE", "%" + makbuz.duzenleyen + "%"));

                if (!string.IsNullOrEmpty(makbuz.unvan))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.UNVAN], "LIKE", "%" + makbuz.unvan + "%"));

                if (!makbuz.islemTarih.isNull)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "=", makbuz.islemTarih));

                if (makbuz.durum > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", makbuz.durum));
                //else//SAYIŞTAY İPTAL EDİLEN MAKBUZLARI DA GÖRMEK İSTİYOR EVKAF 09.11.2019
                //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMBelgeDurumu.IPTAL));

                if (!string.IsNullOrEmpty(makbuz.odemeEmriNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "=", makbuz.odemeEmriNo));

                if (!string.IsNullOrEmpty(makbuz.kullaniciKodu))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", makbuz.kullaniciKodu));

                if (!makbuz.araBasTarih.isNull)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], ">=", makbuz.araBasTarih));

                if (!makbuz.araBitTarih.isNull)
                {
                    makbuz.araBitTarih.AddDays(1);
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "<", makbuz.araBitTarih));
                }
            }

            return kosul;
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="makbuz">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleMizan(Makbuz makbuz)
        {
            string sql = "SELECT MUHMAKBUZHESAP.HESAPKOD, MUHHESAPPLANI.ACIKLAMA, MUHMAKBUZHESAP.ALTHESAP, SUM(MUHMAKBUZHESAP.TUTAR)";

            sql += ", (SELECT MIN(AD) FROM " + TABOrtak.DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCFIRMABILGISI where VERGINO=MUHMAKBUZHESAP.ALTHESAP and MUHMAKBUZHESAP.ALTHESAP<>'')";
            sql += ", (SELECT MIN(AD + ' ' + SOYAD) FROM " + TABOrtak.DatabaseAdBul("Personel.dbo.") + "PERPersonel where TCKIMLIKNO=MUHMAKBUZHESAP.ALTHESAP)";

            sql += " FROM MUHMAKBUZ INNER JOIN";
            sql += " MUHMAKBUZHESAP ON MUHMAKBUZ.YIL = MUHMAKBUZHESAP.YIL AND MUHMAKBUZ.YEVMIYENO = MUHMAKBUZHESAP.YEVMIYENO INNER JOIN";
            sql += " MUHHESAPPLANI ON MUHMAKBUZHESAP.HESAPKOD = MUHHESAPPLANI.HESAPKOD AND MUHMAKBUZHESAP.YIL = MUHHESAPPLANI.YIL";

            string kosul = string.Empty;
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", makbuz.yil));
            int say = 1;
            if (makbuz.muhasebe > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", makbuz.muhasebe));
                say++;
            }
            if (!string.IsNullOrEmpty(makbuz.yevmiyeNo))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YEVMIYENO], "=", makbuz.yevmiyeNo));
                say++;
            }
            if (say < 3)
            {
                if (!string.IsNullOrEmpty(makbuz.birim))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", makbuz.birim));

                if (makbuz.makbuzTipi.kod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MAKBUZTIPIKOD], "=", makbuz.makbuzTipi.kod));

                if (!string.IsNullOrEmpty(makbuz.kisiAd))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KISIAD], "LIKE", "%" + makbuz.kisiAd + "%"));

                if (!string.IsNullOrEmpty(makbuz.aciklama))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + makbuz.aciklama + "%"));

                if (!string.IsNullOrEmpty(makbuz.duzenleyen))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DUZENLEYEN], "LIKE", "%" + makbuz.duzenleyen + "%"));

                if (!string.IsNullOrEmpty(makbuz.unvan))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.UNVAN], "LIKE", "%" + makbuz.unvan + "%"));

                if (!makbuz.islemTarih.isNull)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "=", makbuz.islemTarih));

                if (makbuz.durum > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", makbuz.durum));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMBelgeDurumu.IPTAL));

                if (!string.IsNullOrEmpty(makbuz.odemeEmriNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "=", makbuz.odemeEmriNo));

                if (!string.IsNullOrEmpty(makbuz.kullaniciKodu))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", makbuz.kullaniciKodu));

                if (!makbuz.araBasTarih.isNull)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], ">=", makbuz.araBasTarih));

                if (!makbuz.araBitTarih.isNull)
                {
                    makbuz.araBitTarih.AddDays(1);
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "<", makbuz.araBitTarih));
                }
            }
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " GROUP BY MUHMAKBUZHESAP.MUHASEBEKOD, MUHMAKBUZHESAP.HESAPKOD, MUHHESAPPLANI.ACIKLAMA, MUHMAKBUZHESAP.ALTHESAP";
            sql += " ORDER BY MUHMAKBUZHESAP.HESAPKOD, MUHMAKBUZHESAP.ALTHESAP";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleBirim(Makbuz makbuz)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];
            sql += " FROM " + TABLOAD;

            string kosul = ListeleKosul(makbuz);
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleKisi(Makbuz makbuz)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.TCKIMLIKVERGINO] + "," + ALANLAR[(int)ENUMAlanlar.KISIAD];
            sql += " FROM " + TABLOAD;

            string kosul = ListeleKosul(makbuz);
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + ALANLAR[(int)ENUMAlanlar.TCKIMLIKVERGINO] + "," + ALANLAR[(int)ENUMAlanlar.KISIAD];
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TCKIMLIKVERGINO] + "," + ALANLAR[(int)ENUMAlanlar.KISIAD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere uyan makbuz tipi kullanılmış mı bilgisini getiren sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="makbuzTipiKod">Makbuz tipi kod.</param>
        /// <returns></returns>
        public bool MakbuzTipiKullanilmisMi(int makbuzTipiKod)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.MAKBUZTIPIKOD] };
            int[] intDeger = { makbuzTipiKod };

            return TNS.Veri.Sayac.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Makbuzs the odendi isaretle.
        /// </summary>
        /// <param name="makbuz">The makbuz.</param>
        /// <returns></returns>
        public Sonuc MakbuzOdendiIsaretle(Makbuz makbuz)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], makbuz.durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KREDIKARTIODEME], makbuz.krediKartiIleOdenen);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", makbuz.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", makbuz.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YEVMIYENO], "=", makbuz.yevmiyeNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc MakbuzAciklamaKaydet(MakbuzAciklama bilgi)
        {
            string sql = "INSERT INTO " + TABLOADACIKLAMA + "(" + VeriErisim.TumAlanlar("", ALANLARACIKLAMA) + ") VALUES(";
            sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.birimKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.aciklama) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.makbuzTipiKod) + ") ";

            return KomutCalistir(sql, 1);
        }

        public Sonuc MakbuzAciklamaGuncelle(MakbuzAciklama bilgi)
        {
            string sql = "UPDATE " + TABLOADACIKLAMA + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.ACIKLAMA], bilgi.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.MAKBUZTIPIKOD], bilgi.makbuzTipiKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader MakbuzAciklamaListele(MakbuzAciklama bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADACIKLAMA, ALANLARACIKLAMA);
            sql += " FROM " + TABLOADACIKLAMA;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.KOD], "=", bilgi.kod));

            if (bilgi.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.BIRIMKOD], "=", bilgi.birimKod));

            if (bilgi.makbuzTipiKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.MAKBUZTIPIKOD], "=", bilgi.makbuzTipiKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc MakbuzAciklamaSil(MakbuzAciklama bilgi)
        {
            string sql = "DELETE FROM " + TABLOADACIKLAMA;
            string kosul = "";

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.KOD], "=", bilgi.kod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return KomutCalistir(sql, 1);
        }

        public int EnBuyukAciklamaKod()
        {
            string sql = "";
            sql = "SELECT MAX(" + ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.KOD] + ")";
            sql += " FROM " + TABLOADACIKLAMA;

            int deger = veriErisim.TekKolonSQLInt(sql);
            return deger + 1;
        }
    }
}