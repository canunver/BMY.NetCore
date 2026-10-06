using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
namespace TNS.KYM.IsKurallari.Veri
{
    /// <summary>
    /// ANKSORU veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABANKET : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABANKET(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSORU = "ANKSORU";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSORU : int
        {

            /// <summary>
            /// PRSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PRSIRANO = 0,

            /// <summary>
            /// METIN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            METIN = 1,

            /// <summary>
            /// SECENEKTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SECENEKTUR = 2,

            /// <summary>
            /// ANAHTAR1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ANAHTAR1 = 3,

            /// <summary>
            /// ANAHTAR2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ANAHTAR2 = 4,

            /// <summary>
            /// CEVAPSECENEK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CEVAPSECENEK = 5,

            /// <summary>
            /// DURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 6,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSORU = {
            "PRSIRANO"
            , "METIN"
            , "SECENEKTUR"
            , "ANAHTAR1"
            , "ANAHTAR2"
            , "CEVAPSECENEK"
            , "DURUM"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, AnketSoruyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SoruSil(AnketSoru form)
        {
            string sql = "DELETE FROM " + TABLOADSORU;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSORU[(int)ENUMAlanlarSORU.PRSIRANO], "=", form.prSiraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, AnketSoruyapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SoruEkle(AnketSoru form)
        {
            string sql = "INSERT INTO " + TABLOADSORU + "(" + VeriErisim.TumAlanlar("", ALANLARSORU) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.prSiraNo);
            sql += "," + VeriErisim.DegerYap(form.soruMetin);
            sql += "," + VeriErisim.DegerYap((int)form.secenekTur);
            sql += "," + VeriErisim.DegerYap(form.anahtar1);
            sql += "," + VeriErisim.DegerYap(form.anahtar2);
            sql += "," + VeriErisim.DegerYap(form.dogruCevapSecenek);
            sql += "," + VeriErisim.DegerYap(form.durum);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc SoruDurumDegistir(AnketSoru anketSoru)
        {
            string sql = "UPDATE " + TABLOADSORU + " SET ";
            sql += VeriErisim.KosulOl("", ALANLARSORU[(int)ENUMAlanlarSORU.DURUM], "=", anketSoru.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSORU[(int)ENUMAlanlarSORU.PRSIRANO], "=", anketSoru.prSiraNo));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, AnketSoruyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader SorulariListele(AnketSoru form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSORU, ALANLARSORU) + " FROM " + TABLOADSORU;
            if (form.prSiraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSORU[(int)ENUMAlanlarSORU.PRSIRANO], "=", form.prSiraNo));
            else
            {
                string kosul = "";
                if (!string.IsNullOrEmpty(form.soruMetin))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSORU[(int)ENUMAlanlarSORU.METIN], "LIKE", form.soruMetin + "%"));
                if (!string.IsNullOrEmpty(form.anahtar1))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSORU[(int)ENUMAlanlarSORU.ANAHTAR1], "LIKE", form.anahtar1 + "%"));
                if (!string.IsNullOrEmpty(form.anahtar2))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSORU[(int)ENUMAlanlarSORU.ANAHTAR2], "LIKE", form.anahtar2 + "%"));
                if (form.durum > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSORU[(int)ENUMAlanlarSORU.DURUM], "=", form.durum));
                if (!string.IsNullOrEmpty(kosul))
                    sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            }
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Veri tabanındanki en yüksek soru numarasını bulup döndüren, sql komutunu hazırlayıp, çalıştırıp sonucunu döndüren yordam
        /// </summary>
        /// <returns>Veri tabanındanki en yüksek soru numarası döndürülür.</returns>
        public int MaxSoruNoBul()
        {
            string sql = "SELECT MAX(" + ALANLARSORU[(int)ENUMAlanlarSORU.PRSIRANO] + ") FROM " + TABLOADSORU;
            return (int)veriErisim.TekKolonSQLDbl(sql);
        }

        public IDataReader AnahtarKelimeleriListele(int anahtarNo, string kistas)
        {
            int yer = 0;
            if (anahtarNo == 1)
                yer = (int)ENUMAlanlarSORU.ANAHTAR1;
            else
                yer = (int)ENUMAlanlarSORU.ANAHTAR2;
            if (kistas == "*") kistas = "";
            string sql = "SELECT DISTINCT " + ALANLARSORU[yer] + " FROM " + TABLOADSORU;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSORU[yer], "LIKE", kistas + "%"));
            sql += " order by 1";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSECENEKLER = "ANKCEVAPSECENEKLER";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSECENEKLER : int
        {
            /// <summary>
            /// PRSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PRSIRANO = 0,

            /// <summary>
            /// SECENEKSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SECENEKSIRANO = 1,

            /// <summary>
            /// SECENEKSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SECENEK = 2,

            /// <summary>
            /// METIN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            METIN = 3,

            /// <summary>
            /// PUAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PUAN = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSECENEKLER = {
            "PRSIRANO"
            , "SECENEKSIRANO"
            , "SECENEK"
            , "METIN"
            , "PUAN"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, AnketSoruyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SecenekleriSil(AnketSoru form)
        {
            string sql = "DELETE FROM " + TABLOADSECENEKLER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSECENEKLER[(int)ENUMAlanlarSECENEKLER.PRSIRANO], "=", form.prSiraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, cevapSecenekler dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, AnketSoruyapısında nesne</param>
        /// <param name="satirNo">cevapSecenekler dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SecenekEkle(AnketSoru form, int satirNo)
        {
            string sql = "INSERT INTO " + TABLOADSECENEKLER + "(" + VeriErisim.TumAlanlar("", ALANLARSECENEKLER) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.prSiraNo);
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(form.cevapSecenekler[satirNo].secenek);
            sql += "," + VeriErisim.DegerYap(form.cevapSecenekler[satirNo].secenekMetin);
            sql += "," + VeriErisim.DegerYap(form.cevapSecenekler[satirNo].puan);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, AnketSoruyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader SecenekleriOku(AnketSoru form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSECENEKLER, ALANLARSECENEKLER) + " FROM " + TABLOADSECENEKLER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSECENEKLER[(int)ENUMAlanlarSECENEKLER.PRSIRANO], "=", form.prSiraNo));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADANKET = "ANKANKETBILGI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarANKET : int
        {
            /// <summary>
            /// MODUL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODUL = 0,

            /// <summary>
            /// ANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ANAHTAR = 1,

            /// <summary>
            /// ANKETAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ANKETAD = 2,

            /// <summary>
            /// BASLATARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BASLATARIH = 3,

            /// <summary>
            /// BITISTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BITISTARIH = 4,
            TEKGIRISLIK = 5
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARANKET = {
            "MODUL"
            , "ANAHTAR"
            , "ANKETAD"
            , "BASLATARIH"
            , "BITISTARIH"
            , "TEKGIRISLIK"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADANKETSORU = "ANKANKETSORULAR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarANKETSORU : int
        {
            /// <summary>
            /// MODUL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODUL = 0,

            /// <summary>
            /// ANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ANAHTAR = 1,

            /// <summary>
            /// PRSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PRSIRANO = 2,

            /// <summary>
            /// SORUSIRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SORUSIRA = 3,

            /// <summary>
            /// SORUSEVIYE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SORUSEVIYE = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARANKETSORU = {
            "MODUL"
            , "ANAHTAR"
            , "PRSIRANO"
            , "SORUSIRA"
            , "SORUSEVIYE"
        };

        public Sonuc AnketSorulariSil(AnketBilgi anketBilgi)
        {
            string sql = "DELETE FROM " + TABLOADANKETSORU;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANKETSORU[(int)ENUMAlanlarANKETSORU.MODUL], "=", anketBilgi.modul));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETSORU[(int)ENUMAlanlarANKETSORU.ANAHTAR], "=", anketBilgi.anketAnahtar));
            return KomutCalistir(sql, 0);
        }

        public Sonuc AnketBilgiSil(AnketBilgi anketBilgi)
        {
            string sql = "DELETE FROM " + TABLOADANKET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANKET[(int)ENUMAlanlarANKET.MODUL], "=", anketBilgi.modul));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKET[(int)ENUMAlanlarANKET.ANAHTAR], "=", anketBilgi.anketAnahtar));
            return KomutCalistir(sql, 0);
        }

        public Sonuc AnketBilgiKaydet(AnketBilgi anketBilgi)
        {
            string sql = "INSERT INTO " + TABLOADANKET + "(" + VeriErisim.TumAlanlar("", ALANLARANKET) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(anketBilgi.modul);
            sql += "," + VeriErisim.DegerYap(anketBilgi.anketAnahtar);
            sql += "," + VeriErisim.DegerYap(anketBilgi.anketAd);
            sql += "," + VeriErisim.DegerYap(anketBilgi.baslaTarih);
            sql += "," + VeriErisim.DegerYap(anketBilgi.bitisTarih);
            sql += "," + VeriErisim.DegerYap(anketBilgi.tekGirislik ? 1 : 0);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc AnketeSoruKaydet(AnketBilgi anketBilgi, int satirNo)
        {
            string sql = "INSERT INTO " + TABLOADANKETSORU + "(" + VeriErisim.TumAlanlar("", ALANLARANKETSORU) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(anketBilgi.modul);
            sql += "," + VeriErisim.DegerYap(anketBilgi.anketAnahtar);
            sql += "," + VeriErisim.DegerYap(anketBilgi.sorular[satirNo].prSiraNo);
            sql += "," + VeriErisim.DegerYap(anketBilgi.sorular[satirNo].soruSira);
            sql += "," + VeriErisim.DegerYap(anketBilgi.sorular[satirNo].soruSeviye);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader AnketBilgiOku(AnketBilgi anketKistasBilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADANKET, ALANLARANKET) + " FROM " + TABLOADANKET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANKET[(int)ENUMAlanlarANKET.MODUL], "=", anketKistasBilgi.modul));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKET[(int)ENUMAlanlarANKET.ANAHTAR], "LIKE", anketKistasBilgi.anketAnahtar));
            sql += " ORDER BY " + ALANLARANKET[(int)ENUMAlanlarANKET.ANKETAD];
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader AnketSorulariListele(AnketBilgi anketKistasBilgi)
        {
            return AnketSorulariListele(anketKistasBilgi, null);
        }

        public IDataReader AnketSorulariListele(AnketBilgi anketKistasBilgi, AnketSoru soru)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSORU, ALANLARSORU);
            sql += ", " + ALANLARANKETSORU[(int)ENUMAlanlarANKETSORU.SORUSIRA];
            sql += ", " + ALANLARANKETSORU[(int)ENUMAlanlarANKETSORU.SORUSEVIYE] + " FROM " + TABLOADANKETSORU + ", " + TABLOADSORU;
            sql += " WHERE " + TABLOADANKETSORU + "." + ALANLARANKETSORU[(int)ENUMAlanlarANKETSORU.PRSIRANO] + " = " + TABLOADSORU + "." + ALANLARSORU[(int)ENUMAlanlarSORU.PRSIRANO];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETSORU[(int)ENUMAlanlarANKETSORU.MODUL], "=", anketKistasBilgi.modul));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETSORU[(int)ENUMAlanlarANKETSORU.ANAHTAR], "=", anketKistasBilgi.anketAnahtar));
            if (soru != null && soru.prSiraNo > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADSORU, ALANLARANKETSORU[(int)ENUMAlanlarANKETSORU.PRSIRANO], "=", soru.prSiraNo));
            }

            sql += " ORDER BY " + ALANLARANKETSORU[(int)ENUMAlanlarANKETSORU.SORUSIRA];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADANKETCEVAPANA = "ANKANKETCEVAPANA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarANKETCEVAPANA : int
        {
            /// <summary>
            /// MODUL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODUL = 0,

            /// <summary>
            /// ANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ANAHTAR = 1,

            /// <summary>
            /// ORGBIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORGBIRIMKOD = 2,

            /// <summary>
            /// CEVAPANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CEVAPANAHTAR = 3,

            /// <summary>
            /// KULLANICIKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KULLANICIKOD = 4,

            /// <summary>
            /// CEVAPTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CEVAPTARIH = 5,

            /// <summary>
            /// CEVAPIP alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CEVAPIP = 6,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARANKETCEVAPANA = {
            "MODUL"
            , "ANAHTAR"
            , "ORGBIRIMKOD"
            , "CEVAPANAHTAR"
            , "KULLANICIKOD"
            , "CEVAPTARIH"
            , "CEVAPIP"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADANKETCEVAPDETAY = "ANKANKETCEVAPDETAY";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarANKETCEVAPDETAY : int
        {
            /// <summary>
            /// MODUL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODUL = 0,

            /// <summary>
            /// ANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ANAHTAR = 1,

            /// <summary>
            /// ORGBIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORGBIRIMKOD = 2,

            /// <summary>
            /// CEVAPANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CEVAPANAHTAR = 3,

            /// <summary>
            /// PRSORUSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PRSORUSIRANO = 4,

            /// <summary>
            /// SECENEKSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SECENEKSIRANO = 5,

            /// <summary>
            /// METIN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            METIN = 6,

            /// <summary>
            /// METIN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PUAN = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARANKETCEVAPDETAY = {
            "MODUL"
            , "ANAHTAR"
            , "ORGBIRIMKOD"
            , "CEVAPANAHTAR"
            , "PRSORUSIRANO"
            , "SECENEKSIRANO"
            , "METIN"
            , "PUAN"
        };

        public Sonuc AnketCevaplariSil(AnketBilgi anketBilgi)
        {
            string sql = "DELETE FROM " + TABLOADANKETCEVAPDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.CEVAPANAHTAR], "=", anketBilgi.cevapAnahtar));
            if(!string.IsNullOrEmpty(anketBilgi.modul))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.MODUL], "=", anketBilgi.modul));
            if (!string.IsNullOrEmpty(anketBilgi.anketAnahtar))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.ANAHTAR], "=", anketBilgi.anketAnahtar));
            if (anketBilgi.orgBirimKod>0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.ORGBIRIMKOD], "=", anketBilgi.orgBirimKod));
 
            return KomutCalistir(sql, 0);
        }

        public Sonuc AnketCevapAnaSil(AnketBilgi anketBilgi)
        {
            string sql = "DELETE FROM " + TABLOADANKETCEVAPANA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANKETCEVAPANA[(int)ENUMAlanlarANKETCEVAPANA.CEVAPANAHTAR], "=", anketBilgi.cevapAnahtar));
            if (!string.IsNullOrEmpty(anketBilgi.modul))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPANA[(int)ENUMAlanlarANKETCEVAPANA.MODUL], "=", anketBilgi.modul));
            if (!string.IsNullOrEmpty(anketBilgi.anketAnahtar))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPANA[(int)ENUMAlanlarANKETCEVAPANA.ANAHTAR], "=", anketBilgi.anketAnahtar));
            if (anketBilgi.orgBirimKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPANA[(int)ENUMAlanlarANKETCEVAPANA.ORGBIRIMKOD], "=", anketBilgi.orgBirimKod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc AnketCevapAnaKaydet(AnketBilgi anketBilgi)
        {
            string sql = "INSERT INTO " + TABLOADANKETCEVAPANA + "(" + VeriErisim.TumAlanlar("", ALANLARANKETCEVAPANA) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(anketBilgi.modul);
            sql += "," + VeriErisim.DegerYap(anketBilgi.anketAnahtar);
            sql += "," + VeriErisim.DegerYap(anketBilgi.orgBirimKod);
            sql += "," + VeriErisim.DegerYap(anketBilgi.cevapAnahtar);
            sql += "," + VeriErisim.DegerYap(anketBilgi.kullaniciKod);
            sql += "," + VeriErisim.DegerYap(anketBilgi.cevapTarih);
            sql += "," + VeriErisim.DegerYap(anketBilgi.cevapIP);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public bool AnketCevapAnaVarMi(AnketBilgi anketBilgi)
        {
            string sql = "SELECT 1 FROM " + TABLOADANKETCEVAPANA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANKETCEVAPANA[(int)ENUMAlanlarANKETCEVAPANA.MODUL], "=", anketBilgi.modul));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPANA[(int)ENUMAlanlarANKETCEVAPANA.ANAHTAR], "=", anketBilgi.anketAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPANA[(int)ENUMAlanlarANKETCEVAPANA.ORGBIRIMKOD], "=", anketBilgi.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPANA[(int)ENUMAlanlarANKETCEVAPANA.CEVAPANAHTAR], "=", anketBilgi.cevapAnahtar));
            return veriErisim.TekKolonSQLInt(sql) == 1;
        }

        public Sonuc AnketCevapDetayKaydet(AnketBilgi anketBilgi, AnketSoru soru, AnketCevap anketCevap)
        {
            string sql = "INSERT INTO " + TABLOADANKETCEVAPDETAY + "(" + VeriErisim.TumAlanlar("", ALANLARANKETCEVAPDETAY) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(anketBilgi.modul);
            sql += "," + VeriErisim.DegerYap(anketBilgi.anketAnahtar);
            sql += "," + VeriErisim.DegerYap(anketBilgi.orgBirimKod);
            sql += "," + VeriErisim.DegerYap(anketBilgi.cevapAnahtar);
            sql += "," + VeriErisim.DegerYap(soru.prSiraNo);
            sql += "," + VeriErisim.DegerYap(anketCevap.cevapNo);
            sql += "," + VeriErisim.DegerYap(anketCevap.cevapYazi);
            sql += "," + VeriErisim.DegerYap(anketCevap.puan);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader CevaplariOku(AnketBilgi anketBilgi)
        {
            string sql = "SELECT " + ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.PRSORUSIRANO] + ", " + ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.SECENEKSIRANO] + ", " + ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.METIN] + ", " + ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.PUAN] + " FROM " + TABLOADANKETCEVAPDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.MODUL], "=", anketBilgi.modul));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.ANAHTAR], "=", anketBilgi.anketAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.ORGBIRIMKOD], "=", anketBilgi.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.CEVAPANAHTAR], "=", anketBilgi.cevapAnahtar));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader AnketIstatistikOku(AnketBilgi anketKistasBilgi, AnketSoru soru, string kopruKod)
        {
            return AnketCaprazIstatistikOku(anketKistasBilgi, soru, kopruKod, null, null);
        }

        public IDataReader AnketCaprazIstatistikOku(AnketBilgi anketKistasBilgi, AnketSoru soru, string kopruKod, AnketSoru anaSoru, CevapSecenekler anaSoruCevap)
        {
            string sql = "SELECT METIN, COUNT(*) FROM ANKANKETCEVAPDETAY D, ORGBIRIM B";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.MODUL], "=", anketKistasBilgi.modul));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.ANAHTAR], "=", anketKistasBilgi.anketAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANKETCEVAPDETAY[(int)ENUMAlanlarANKETCEVAPDETAY.PRSORUSIRANO], "=", soru.prSiraNo));
            //if (anketKistasBilgi.altBirimDahil)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "KOPRUKOD", "LIKE", kopruKod + "%"));
            //else
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "KOPRUKOD", "=", kopruKod));
            sql += " and D.ORGBIRIMKOD = B.KOD";
            if (anaSoru != null)
            {
                sql += " and 1 in (";
                sql += " select 1 from ANKANKETCEVAPDETAY I ";
                sql += " where I.MODUL=D.MODUL AND I.ANAHTAR=D.ANAHTAR AND I.ORGBIRIMKOD=D.ORGBIRIMKOD AND I.CEVAPANAHTAR=D.CEVAPANAHTAR ";
                sql += " AND I.PRSORUSIRANO=" + anaSoru.prSiraNo.ToString();
                sql += " and I.METIN=" + VeriErisim.DegerYap(anaSoruCevap.secenekMetin);
                sql += ")";
            }
            sql += " group by METIN order by 2 desc";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
