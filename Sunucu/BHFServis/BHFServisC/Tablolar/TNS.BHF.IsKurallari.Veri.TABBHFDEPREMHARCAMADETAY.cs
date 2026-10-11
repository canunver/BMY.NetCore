using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHF;
namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// BHFDEPREMHARCAMADETAY veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHFDEPREMHARCAMADETAY : TABOrtak
    {
        /// <summary>
        /// TABBHFDEPREMHARCAMADETAY sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHFDEPREMHARCAMADETAY(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /*
         * 
            CREATE TABLE BHFDEPREMSOZLUK
            (KOD VARCHAR(20)
            , AD VARCHAR(200)
            , PRIMARY KEY (KOD)
            )
         
            CREATE TABLE BHFDEPREMHARCAMAANA
            (YIL INT
            , AY INT
            , MUHASEBEKOD INT
            , BIRIMKOD VARCHAR(3)
            , DURUM INT
            , KAYDEDEN VARCHAR(200)
            , KAYITTARIHI DATETIME
            , ONAYLAYAN VARCHAR(200)
            , ONAYTARIHI DATETIME
            , PRIMARY KEY (YIL, AY, MUHASEBEKOD, BIRIMKOD)
            )

            CREATE TABLE BHFDEPREMHARCAMADETAY
                (YIL INT
                , AY INT
                , MUHASEBEKOD INT
                , BIRIMKOD VARCHAR(3)
                , ILKOD INT
                , EKOKOD VARCHAR(20)
                , BUTCEICI MONEY
                , BUTCEICIACIKLAMA VARCHAR(50)
                , HT1 MONEY
                , HT2 MONEY
                , HT3 MONEY
                , HT4 MONEY
                , HT5 MONEY
                , BUTCEDISIACIKLAMA VARCHAR(50)
                , PRIMARY KEY (YIL, AY, MUHASEBEKOD, BIRIMKOD, ILKOD, EKOKOD)
            )
         * */


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHFDEPREMHARCAMADETAY";
        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADANA = "BHFDEPREMHARCAMAANA";

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
            /// AY alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AY = 1,

            /// <summary>
            /// MUHASEBEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,

            /// <summary>
            /// ILKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILKOD = 4,

            /// <summary>
            /// EKOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 5,

            /// <summary>
            /// BUTCEICI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUTCEICI = 6,

            /// <summary>
            /// BUTCEICIACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUTCEICIACIKLAMA = 7,

            /// <summary>
            /// HT1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HT1 = 8,

            /// <summary>
            /// HT2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HT2 = 9,

            /// <summary>
            /// HT3 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HT3 = 10,

            /// <summary>
            /// HT4 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HT4 = 11,

            /// <summary>
            /// HT5 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HT5 = 12,

            /// <summary>
            /// BUTCEDISIACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUTCEDISIACIKLAMA = 13,
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarAna : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// AY alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AY = 1,

            /// <summary>
            /// MUHASEBEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,

            /// <summary>
            /// ILKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 4,
            KAYDEDEN = 5,
            KAYITTARIHI = 6,
            ONAYLAYAN = 7,
            ONAYTARIHI = 8,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "AY"
            , "MUHASEBEKOD"
            , "BIRIMKOD"
            , "ILKOD"
            , "EKOKOD"
            , "BUTCEICI"
            , "BUTCEICIACIKLAMA"
            , "HT1"
            , "HT2"
            , "HT3"
            , "HT4"
            , "HT5"
            , "BUTCEDISIACIKLAMA"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARANA = {
            "YIL"
            , "AY"
            , "MUHASEBEKOD"
            , "BIRIMKOD"
            , "DURUM"
            , "KAYDEDEN"
	        , "KAYITTARIHI"
	        , "ONAYLAYAN"
	        , "ONAYTARIHI"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DepremHarcamaAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilAna(DepremHarcamaAna form)
        {
            string sql = "DELETE FROM " + TABLOADANA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.AY], "=", form.ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.BIRIMKOD], "=", form.birimKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DepremHarcamaAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(DepremHarcamaAna form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", form.ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", form.ilKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DepremHarcamaAna yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleAna(DepremHarcamaAna form, string kullaniciKodu)
        {
            string sql = "INSERT INTO " + TABLOADANA + "(" + VeriErisim.TumAlanlar("", ALANLARANA) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.ay);
            sql += "," + VeriErisim.DegerYap(form.muhasebeKod);
            sql += "," + VeriErisim.DegerYap(form.birimKod);
            sql += "," + VeriErisim.DegerYap(1);
            sql += "," + VeriErisim.DegerYap(kullaniciKodu);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += ",NULL, NULL";
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DepremHarcamaAna yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleAna(DepremHarcamaAna form, string kullaniciKodu)
        {
            string sql = "UPDATE " + TABLOADANA;
            sql += " SET "+ VeriErisim.AtamaOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarAna.DURUM], 1);
            sql += ", " + VeriErisim.AtamaOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarAna.KAYDEDEN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarAna.KAYITTARIHI], new TNSDateTime(DateTime.Now));

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.AY], "=", form.ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.BIRIMKOD], "=", form.birimKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran, depremHarcamaDetay dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DepremHarcamaAna yapısında nesne</param>
        /// <param name="satirNo">depremHarcamaDetay dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(DepremHarcamaAna form, DepremHarcamaDetay dhd)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.ay);
            sql += "," + VeriErisim.DegerYap(form.muhasebeKod);
            sql += "," + VeriErisim.DegerYap(form.birimKod);
            sql += "," + VeriErisim.DegerYap(form.ilKod);
            sql += "," + VeriErisim.DegerYap(dhd.ekoKod);
            sql += "," + VeriErisim.DegerYap(dhd.butceIci);
            sql += "," + VeriErisim.DegerYap(dhd.butceIciAciklama);
            sql += "," + VeriErisim.DegerYap(dhd.hT1);
            sql += "," + VeriErisim.DegerYap(dhd.hT2);
            sql += "," + VeriErisim.DegerYap(dhd.hT3);
            sql += "," + VeriErisim.DegerYap(dhd.hT4);
            sql += "," + VeriErisim.DegerYap(dhd.hT5);
            sql += "," + VeriErisim.DegerYap(dhd.butceDisiAciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader OkuAna(DepremHarcamaAna form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADANA, ALANLARANA) + " FROM " + TABLOADANA;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarAna.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarAna.AY], "=", form.ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarAna.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarAna.BIRIMKOD], "=", form.birimKod));

            return veriErisim.DataReaderDondur(sql);
        }


        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DepremHarcamaAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(DepremHarcamaAna form)
        {
            string sql = "SELECT BHFDEPREMHARCAMAANA.YIL, BHFDEPREMHARCAMAANA.AY, BHFDEPREMHARCAMAANA.MUHASEBEKOD, BHFDEPREMHARCAMAANA.BIRIMKOD, BHFDEPREMHARCAMADETAY.ILKOD, BHFDEPREMHARCAMAANA.DURUM, BHFDEPREMHARCAMAANA.KAYDEDEN, BHFDEPREMHARCAMAANA.KAYITTARIHI, BHFDEPREMHARCAMAANA.ONAYLAYAN, BHFDEPREMHARCAMAANA.ONAYTARIHI, BHFDEPREMHARCAMADETAY.EKOKOD, ";
            sql += " BHFDEPREMHARCAMADETAY.BUTCEICI, BHFDEPREMHARCAMADETAY.BUTCEICIACIKLAMA, BHFDEPREMHARCAMADETAY.HT1, BHFDEPREMHARCAMADETAY.HT2, BHFDEPREMHARCAMADETAY.HT3, BHFDEPREMHARCAMADETAY.HT4, BHFDEPREMHARCAMADETAY.HT5, ";
            sql += " BHFDEPREMHARCAMADETAY.BUTCEDISIACIKLAMA FROM BHFDEPREMHARCAMAANA";
            sql += " LEFT OUTER JOIN BHFDEPREMHARCAMADETAY ON BHFDEPREMHARCAMAANA.YIL = BHFDEPREMHARCAMADETAY.YIL AND BHFDEPREMHARCAMAANA.AY = BHFDEPREMHARCAMADETAY.AY AND";
            sql += " BHFDEPREMHARCAMAANA.MUHASEBEKOD = BHFDEPREMHARCAMADETAY.MUHASEBEKOD AND BHFDEPREMHARCAMAANA.BIRIMKOD = BHFDEPREMHARCAMADETAY.BIRIMKOD AND BHFDEPREMHARCAMADETAY.ILKOD = " + VeriErisim.DegerYap(form.ilKod);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarAna.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarAna.AY], "=", form.ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarAna.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADANA, ALANLARANA[(int)ENUMAlanlarAna.BIRIMKOD], "=", form.birimKod));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", form.ilKod));

            sql += " ORDER BY BHFDEPREMHARCAMAANA.YIL, BHFDEPREMHARCAMAANA.AY, BHFDEPREMHARCAMAANA.MUHASEBEKOD, BHFDEPREMHARCAMAANA.BIRIMKOD, BHFDEPREMHARCAMADETAY.ILKOD, BHFDEPREMHARCAMADETAY.EKOKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc OnayGuncelle(DepremHarcamaAna form, string kullaniciKodu)
        {
            string sql = "UPDATE " + TABLOADANA;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARANA[(int)ENUMAlanlarAna.DURUM], form.durum);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARANA[(int)ENUMAlanlarAna.ONAYLAYAN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARANA[(int)ENUMAlanlarAna.ONAYTARIHI], new TNSDateTime(DateTime.Now));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.AY], "=", form.ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.BIRIMKOD], "=", form.birimKod));
            return KomutCalistir(sql, 1);
        }

        public Sonuc OnayEkle(DepremHarcamaAna form, string kullaniciKodu)
        {
            string sql = "INSERT INTO " + TABLOADANA + "(" + VeriErisim.TumAlanlar("", ALANLARANA) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.ay);
            sql += "," + VeriErisim.DegerYap(form.muhasebeKod);
            sql += "," + VeriErisim.DegerYap(form.birimKod);
            sql += "," + VeriErisim.DegerYap(form.durum);
            sql += ", NULL, NULL";
            sql += "," + VeriErisim.DegerYap(kullaniciKodu);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc VeriKopyalaForm(int kaynakYil, int kaynakAy, int hedefYil, int hedefAy)
        {
            string sql = "INSERT INTO " + TABLOADANA + "(" + VeriErisim.TumAlanlar("", ALANLARANA) + ") ";
            sql += " SELECT "+ hedefYil + ", " + hedefAy + ", MUHASEBEKOD, BIRIMKOD, DURUM, KAYDEDEN, KAYITTARIHI, ONAYLAYAN, ONAYTARIHI FROM BHFDEPREMHARCAMAANA";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.AY], "=", kaynakAy));
            sql += $" AND 1 NOT IN (SELECT 1 FROM BHFDEPREMHARCAMAANA IC WHERE IC.YIL={hedefYil} AND IC.AY={hedefAy} AND IC.MUHASEBEKOD = BHFDEPREMHARCAMAANA.MUHASEBEKOD AND IC.BIRIMKOD=BHFDEPREMHARCAMAANA.BIRIMKOD) ";
            return KomutCalistir(sql, 0);
        }

        public Sonuc VeriKopyalaDetay(int kaynakYil, int kaynakAy, int hedefYil, int hedefAy)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") ";
            sql += " SELECT " + hedefYil + ", " + hedefAy + ", MUHASEBEKOD, BIRIMKOD, ILKOD, EKOKOD, BUTCEICI, BUTCEICIACIKLAMA, HT1, HT2, HT3, HT4, HT5, BUTCEDISIACIKLAMA FROM BHFDEPREMHARCAMADETAY";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.AY], "=", kaynakAy));
            sql += $" AND 1 NOT IN (SELECT 1 FROM BHFDEPREMHARCAMADETAY IC WHERE IC.YIL={hedefYil} AND IC.AY={hedefAy} AND IC.MUHASEBEKOD = BHFDEPREMHARCAMADETAY.MUHASEBEKOD AND IC.BIRIMKOD=BHFDEPREMHARCAMADETAY.BIRIMKOD AND IC.ILKOD=BHFDEPREMHARCAMADETAY.ILKOD AND IC.EKOKOD=BHFDEPREMHARCAMADETAY.EKOKOD) ";
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DepremHarcamaAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleDepremKodlari()
        {
            string sql = "SELECT BHFDEPREMSOZLUK.KOD, BHFDEPREMSOZLUK.AD FROM BHFDEPREMSOZLUK ORDER BY KOD";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
