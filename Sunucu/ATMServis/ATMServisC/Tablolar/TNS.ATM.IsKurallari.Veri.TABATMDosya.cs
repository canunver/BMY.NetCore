using System;
using System.Data;
using TNS.Veri;

namespace TNS.ATM.IsKurallari.Veri
{
    /// <summary>
    /// Dosya bilgilerini tutan ATMDOSYA veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABATMDosya : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABATMDosya(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ATMDOSYA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Dosya numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOSYANO = 1,
            /// <summary>
            /// Saymanlık alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYMANLIK = 2,
            /// <summary>
            /// Birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKODU = 3,
            /// <summary>
            /// Kimlik numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KIMLIKNO = 4,
            /// <summary>
            /// Borçlu tipi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCLUTIPI = 5,
            /// <summary>
            /// Borç tespit tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCTESPITTARIHI = 6,
            /// <summary>
            /// Borç doğuş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCDOGUSTARIHI = 7,
            /// <summary>
            /// Borçlu adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCLUAD = 8,
            /// <summary>
            /// Borçlu adres alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCLUADRES = 9,
            /// <summary>
            /// Borç alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORC = 10,
            /// <summary>
            /// Borç sebep alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCSEBEP = 11,
            /// <summary>
            /// Faiz başlangıç tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FAIZBASLANGICTARIHI = 12,
            /// <summary>
            /// Faiz tür yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FAIZTURYIL = 13,
            /// <summary>
            /// Faiz tür kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FAIZTURKOD = 14,
            /// <summary>
            /// Zaman aşımı başlangıç yılı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ZAMANASIMIBASLANGICYILI = 15,
            /// <summary>
            /// Tebliğ tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TEBLIGTARIHI = 16,
            /// <summary>
            /// Son durum kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SONDURUMKODU = 17,
            /// <summary>
            /// Borç değerlendirme tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCDEGERLENDIRMETARIHI = 18,
            /// <summary>
            /// Para birimi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PARABIRIMI = 19,
            /// <summary>
            /// Son durum tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SONDURUMTARIH = 20,
            /// <summary>
            /// Saymanlık dosya no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYMANLIKDOSYANO = 21,
            /// <summary>
            /// The odemeemrino
            /// </summary>
            ODEMEEMRINO = 22,
            GELYAZIYER = 23,
            GELYAZITARIH = 24,
            GELYAZISAYI = 25,
            SAYGITYAZITARIH = 26,
            SAYGITYAZISAYI = 27,
            KISIGITYAZITARIH = 28,
            KISIGITYAZISAYI = 29,
            TAKSITBASLANGICTARIHI = 30,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "DOSYANO", "SAYMANLIK", "BIRIMKODU", "KIMLIKNO",
                                    "BORCLUTIPI", "BORCTESPITTARIHI", "BORCDOGUSTARIHI",
                                    "BORCLUAD", "BORCLUADRES", "BORC", "BORCSEBEP", "FAIZBASLANGICTARIHI",
                                    "FAIZTURYIL", "FAIZTURKOD", "ZAMANASIMIBASLANGICYILI",
                                    "TEBLIGTARIHI", "SONDURUMKODU", "BORCDEGERLENDIRMETARIHI", "PARABIRIMI",
                                    "SONDURUMTARIH", "SAYMANLIKDOSYANO", "ODEMEEMRINO" ,
                                    "GELYAZIYER", "GELYAZITARIH", "GELYAZISAYI",
                                    "SAYGITYAZITARIH", "SAYGITYAZISAYI", "KISIGITYAZITARIH", "KISIGITYAZISAYI", "TAKSITBASLANGICTARIHI",
                                  };


        /// <summary>
        /// Dosya tipinde bulunan alacak takip bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="alacakTakip">Alacak takip bilgilerini tutan Dosya türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür</returns>
        public Sonuc Kaydet(Dosya alacakTakip)
        {
            string[] kodYilStr = { ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] kodYilInt = { alacakTakip.yil };
            int kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYANO], new string[0], new string[0], kodYilStr, kodYilInt);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(alacakTakip.yil) + ",";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.saymanlikKodu) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.birimKodu) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.kimlikNo) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.borcluTipi) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.borcTespitTarihi) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.borcDogusTarihi) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.borcluAd) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.borcluAdres) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.borc) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.borcSebep) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.faizBaslangicTarihi) + ",";
            if (alacakTakip.faizTur.yil > 0 && alacakTakip.faizTur.kod > 0)
            {
                sql += VeriErisim.DegerYap(alacakTakip.faizTur.yil) + ",";
                sql += VeriErisim.DegerYap(alacakTakip.faizTur.kod) + ",";
            }
            else
                sql += "NULL,NULL,";
            sql += VeriErisim.DegerYap(alacakTakip.zamanAsimiBaslangicYili) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.tebligTarihi) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.sonDurum.kod) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.borcDegerlendirmeTarihi) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.paraBirimi) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.sonDurumTarihi) + ",";
            sql += VeriErisim.DegerYap(alacakTakip.saymanlikDosyaNo);
            sql += "," + VeriErisim.DegerYap(alacakTakip.odemeEmriNo);
            sql += "," + VeriErisim.DegerYap(alacakTakip.gelenYaziYer);
            sql += "," + VeriErisim.DegerYap(alacakTakip.gelenYaziTarih);
            sql += "," + VeriErisim.DegerYap(alacakTakip.gelenYaziSayi);
            sql += "," + VeriErisim.DegerYap(alacakTakip.saymanligaGidenYaziTarih);
            sql += "," + VeriErisim.DegerYap(alacakTakip.saymanligaGidenYaziSayi);
            sql += "," + VeriErisim.DegerYap(alacakTakip.kisiyiGidenYaziTarih);
            sql += "," + VeriErisim.DegerYap(alacakTakip.kisiyeGidenYaziSayi);
            sql += "," + VeriErisim.DegerYap(alacakTakip.taksitBaslangicTarihi);
            sql += ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = kod.ToString();

            return sonuc;
        }

        /// <summary>
        /// Dosya tipinde bulunan alacak takip bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="alacakTakip">Alacak takip bilgilerini tutan Dosya türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür</returns>
        public Sonuc Guncelle(Dosya alacakTakip)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], alacakTakip.saymanlikKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], alacakTakip.birimKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIMLIKNO], alacakTakip.kimlikNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORCLUTIPI], alacakTakip.borcluTipi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORCTESPITTARIHI], alacakTakip.borcTespitTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORCDOGUSTARIHI], alacakTakip.borcDogusTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORCLUAD], alacakTakip.borcluAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORCLUADRES], alacakTakip.borcluAdres) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORC], alacakTakip.borc) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORCSEBEP], alacakTakip.borcSebep) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAIZBASLANGICTARIHI], alacakTakip.faizBaslangicTarihi) + ",";
            if (alacakTakip.faizTur.yil > 0 && alacakTakip.faizTur.kod > 0)
            {
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAIZTURYIL], alacakTakip.faizTur.yil) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAIZTURKOD], alacakTakip.faizTur.kod) + ",";
            }
            else
            {
                sql += ALANLAR[(int)ENUMAlanlar.FAIZTURYIL] + "=" + "NULL" + ",";
                sql += ALANLAR[(int)ENUMAlanlar.FAIZTURKOD] + "=" + "NULL" + ",";
            }
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZAMANASIMIBASLANGICYILI], alacakTakip.zamanAsimiBaslangicYili) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEBLIGTARIHI], alacakTakip.tebligTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONDURUMKODU], alacakTakip.sonDurum.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORCDEGERLENDIRMETARIHI], alacakTakip.borcDegerlendirmeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMI], alacakTakip.paraBirimi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONDURUMTARIH], alacakTakip.sonDurumTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIKDOSYANO], alacakTakip.saymanlikDosyaNo);

            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELYAZIYER], alacakTakip.gelenYaziYer);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELYAZITARIH], alacakTakip.gelenYaziTarih);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELYAZISAYI], alacakTakip.gelenYaziSayi);

            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYGITYAZITARIH], alacakTakip.saymanligaGidenYaziTarih);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYGITYAZISAYI], alacakTakip.saymanligaGidenYaziSayi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISIGITYAZITARIH], alacakTakip.kisiyiGidenYaziTarih);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISIGITYAZISAYI], alacakTakip.kisiyeGidenYaziSayi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAKSITBASLANGICTARIHI], alacakTakip.taksitBaslangicTarihi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", alacakTakip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", alacakTakip.dosyaNo));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = alacakTakip.dosyaNo.ToString();

            return sonuc;
        }


        /// <summary>
        /// Yıl, Dosya numarası, Durum kodu, Durum tarihi anahtar alanları kullanarak
        /// veri tabanında Durum kodu ve tarihi alanlarını güncelleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan Int türünde nesne</param>
        /// <param name="dosyaNo">Dosya no bilgisini tutan Int türünde nesne</param>
        /// <param name="durumKodu">Durum kodu bilgisini tutan Int türünde nesne</param>
        /// <param name="durumTarihi">Durum tarihi bilgisini tutan TNSDateTime türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda Sonuc yapısında nesne döndürür</returns>
        public Sonuc DurumGuncelle(int yil, int dosyaNo, int durumKodu, TNSDateTime durumTarihi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONDURUMKODU], durumKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONDURUMTARIH], durumTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", dosyaNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Odemes the emri kaydet dosya.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="dosyaNo">The dosya no.</param>
        /// <param name="belgeYil">The belge yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <returns></returns>
        public Sonuc OdemeEmriKaydetDosya(int yil, int dosyaNo, int belgeYil, string belgeNo)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], belgeYil + "/" + belgeNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", dosyaNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen değerleri OR veya AND ekleyen fonksiyon
        /// </summary> 
        /// <param name="tablo">Tablo bilgisini tutan String tipinde nesne</param>
        /// <param name="degerler">Değerler bilgisini tutan String dizi tipinde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda String tipinde nesne döndürür</returns>
        public static string DegerleriOrla(string tablo, string[] degerler)
        {
            string donusDeger = "";

            bool orlandiMi = false;
            foreach (string s in degerler)
            {
                if (s != "")
                {
                    if (donusDeger != "")
                    {
                        orlandiMi = true;
                        donusDeger += " OR ";
                    }

                    string[] parcali = s.Split('-');

                    string saymanlik = "";
                    if (parcali[0] != "*")
                        saymanlik = tablo + "." + ALANLAR[(int)ENUMAlanlar.SAYMANLIK] + "=" + parcali[0];

                    string birim = "";
                    if (parcali[1] != "*")
                        birim = tablo + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKODU] + "=" + parcali[1];

                    if (saymanlik != "" && birim != "")
                        donusDeger += "(" + saymanlik + " AND " + birim + ")";
                    else if (saymanlik != "")
                        donusDeger += "(" + saymanlik + ")";
                    else if (birim != "")
                        donusDeger += "(" + birim + ")";
                }
            }

            if (orlandiMi)
                donusDeger = "(" + donusDeger + ")";

            return donusDeger;
        }

        /// <summary>
        /// Dosya yapısında duran anahtar alanları kullanarak, ilgili kayıtları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="alacakTakip">Anahtar değişkenleri tutan Dosya türünde nesne</param>
        /// <param name="degerler">Değerleri tutan String türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu IDataReader yapısında nesne döndürür</returns>
        public IDataReader Listele(Dosya alacakTakip, string[] degerler)
        {
            string sonIslemYapan = "(SELECT KULLANICIKODU FROM ATMDOSYADURUMTARIHCE WHERE ATMDOSYADURUMTARIHCE.YIL=ATMDOSYA.YIL and ATMDOSYADURUMTARIHCE.DOSYANO=ATMDOSYA.DOSYANO AND ATMDOSYADURUMTARIHCE.SIRANO = (SELECT MAX(IC.SIRANO) FROM ATMDOSYADURUMTARIHCE IC WHERE  ATMDOSYADURUMTARIHCE.YIL=IC.YIL and ATMDOSYADURUMTARIHCE.DOSYANO=IC.DOSYANO))";
            string alan = "";
            if (alacakTakip.muhasebeBorcDurumuAl)
                alan = "CASE WHEN ATMDOSYA.KIMLIKNO IS NULL OR ATMDOSYA.KIMLIKNO = '' THEN 0 ELSE (SELECT sum(MUHMIF.BORC-MUHMIF.ALACAK) FROM " + DatabaseAdBul("ButceIslemleri.dbo.") + "MUHMIF INNER JOIN " + DatabaseAdBul("ButceIslemleri.dbo.") + "MUHMIFFORM ON MUHMIF.YIL = MUHMIFFORM.YIL AND MUHMIF.BELGENO = MUHMIFFORM.BELGENO WHERE MUHMIFFORM.YIL>=ATMDOSYA.YIL AND FORMDURUM<9 AND ALTHESAPKOD=ATMDOSYA.KIMLIKNO) END";
            else
                alan = "0";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", ATMDURUM.AD, ATMFAIZTUR.AD, ATMFAIZTUR.ORAN, MUH.MUHASEBEAD, BIR.BIRIMAD";
            sql += ", " + alan + "," + sonIslemYapan;
            sql += ", " + VeriErisim.TumAlanlar(TABATMDosyaTaksit.TABLOAD, TABATMDosyaTaksit.ALANLAR);
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN ATMTAKSIT ON ATMTAKSIT.YIL = ATMDOSYA.YIL AND ATMTAKSIT.DOSYANO = ATMDOSYA.DOSYANO";
            sql += " INNER JOIN ATMDURUM ON ATMDOSYA.SONDURUMKODU = ATMDURUM.KOD";
            sql += " LEFT OUTER JOIN ATMFAIZTUR ON ATMDOSYA.FAIZTURYIL = ATMFAIZTUR.YIL AND ATMDOSYA.FAIZTURKOD = ATMFAIZTUR.KOD";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("ButceIslemleri.dbo.") + "MUHMUHASEBE MUH ON MUH.MUHASEBEKOD = ATMDOSYA.SAYMANLIK";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("ButceIslemleri.dbo.") + "MUHMUHASEBEBIRIM BIR ON BIR.MUHASEBEKOD = ATMDOSYA.SAYMANLIK AND BIR.BIRIMKOD = ATMDOSYA.BIRIMKODU";
            if (!string.IsNullOrEmpty(alacakTakip.kurKod))//Birim tablosuna bağlı koşul
                sql += " AND BIR.BIRIMKOD = '" + alacakTakip.kurKod + "'";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], ">", 0));

            if (alacakTakip.dosyaNo > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", alacakTakip.dosyaNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", alacakTakip.yil));
            }
            else if (alacakTakip.kimlikNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "=", alacakTakip.kimlikNo));
            else if (alacakTakip.borcluAd != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BORCLUAD], "LIKE", "%" + alacakTakip.borcluAd + "%"));
            else
            {
                if (alacakTakip.yil > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", alacakTakip.yil));

                if (alacakTakip.saymanlikKodu > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", alacakTakip.saymanlikKodu));

                if (!string.IsNullOrWhiteSpace(alacakTakip.birimKodu))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", alacakTakip.birimKodu));

                if (!string.IsNullOrWhiteSpace(alacakTakip.islemYapan))
                    sql = VeriErisim.KosulEkle(sql, "AND", "(1 IN (SELECT 1 FROM ATMDOSYADURUMTARIHCE ICTAR where ICTAR.YIL=ATMDOSYA.YIL AND ICTAR.DOSYANO=ATMDOSYA.DOSYANO AND ICTAR.KULLANICIKODU LIKE" + VeriErisim.DegerYap(alacakTakip.islemYapan + "%") + " )) ");

                if (alacakTakip.saymanlikKodu <= 0 || alacakTakip.birimKodu == "")
                {
                    string orluSaymanlikVeBirim = DegerleriOrla(TABLOAD, degerler);
                    if (orluSaymanlikVeBirim != "")
                        sql = VeriErisim.KosulEkle(sql, "AND", orluSaymanlikVeBirim);
                }

                if (alacakTakip.sonDurum.kod > 0)
                {
                    if (alacakTakip.sonDurum.kod == 999)
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SONDURUMKODU], "<", 15));
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SONDURUMKODU], "=", alacakTakip.sonDurum.kod));
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SONDURUMKODU], "<>", (int)ENUMDurum.Iptal));

                if (alacakTakip.saymanlikDosyaNo != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYMANLIKDOSYANO], "=", alacakTakip.saymanlikDosyaNo));
            }

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BORCLUAD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yıl ve değerler alanlarını kullanarak tebliğ tarihi geçmiş dosyaları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan Int türünde nesne</param>
        /// <param name="degerler">Değerler bilgisini tutan String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucu IDataReader yapısında nesne döndürür</returns>
        public IDataReader TebligTarihiGecmisDosyaListele(int yil, string[] degerler)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (yil > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TEBLIGTARIHI], "<=", new TNSDateTime(DateTime.Today.Date.AddMonths(-1).ToString())));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SONDURUMKODU], "=", (int)ENUMDurum.TebligEdildi));
            }

            string orluSaymanlikVeBirim = DegerleriOrla(TABLOAD, degerler);
            if (orluSaymanlikVeBirim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluSaymanlikVeBirim);

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql += "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DOSYANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yıl ve Dosya numarası alanlarını kullanarak Faiz başlangıç tarihi bilgisini veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan Int türünde nesne</param>
        /// <param name="dosyaNo">Dosya numarası bilgisini tutan Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda TNSDateTime türünde nesne döndürür</returns>
        public TNSDateTime FaizBaslangicTarihiDondur(int yil, int dosyaNo)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.FAIZBASLANGICTARIHI] + " FROM " + TABLOAD;

            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (dosyaNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", dosyaNo));

            return veriErisim.TekKolonSQLDate(sql);
        }

        /// <summary>
        /// Yıl ve Dosya numarası alanlarını kullanarak dosya faiz tür bilgilerini (Yıl,Kod,Ad,Oran) veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan Int türünde nesne</param>
        /// <param name="dosyaNo">Dosya numarası bilgisini tutan Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda IDataReader türünde nesne döndürür</returns>
        public IDataReader DosyaFaizTur(int yil, int dosyaNo)
        {
            string sql = "SELECT " + "ATMFAIZTUR.YIL, ATMFAIZTUR.KOD, ATMFAIZTUR.AD, ATMFAIZTUR.ORAN";
            sql += " FROM " + TABLOAD + ", ATMFAIZTUR";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FAIZTURYIL] + "=" + "ATMFAIZTUR.YIL";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FAIZTURKOD] + "=" + "ATMFAIZTUR.KOD";

            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (dosyaNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", dosyaNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yıl ve Dosya numarası alanlarını kullanarak son durum bilgisini veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan Int türünde nesne</param>
        /// <param name="dosyaNo">Dosya numarası bilgisini tutan Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda Int türünde nesne döndürür</returns>
        public int SonDurumDondur(int yil, int dosyaNo)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.SONDURUMKODU] + " FROM " + TABLOAD;

            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (dosyaNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", dosyaNo));

            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Yıl ve Dosya numarası alanlarını kullanarak TNS.Veri.Sayac.KayitVarmi fonksiyonunu çalıştırak
        /// sonucunu dönen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan Int türünde nesne</param>
        /// <param name="dosyaNo">Dosya numarası bilgisini tutan Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Bool türünde nesne döndürür</returns>
        public bool DosyaVarMi(int yil, int dosyaNo)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.DOSYANO] };
            int[] intDeger = { yil, dosyaNo };
            return TNS.Veri.Sayac.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }
    }
}
