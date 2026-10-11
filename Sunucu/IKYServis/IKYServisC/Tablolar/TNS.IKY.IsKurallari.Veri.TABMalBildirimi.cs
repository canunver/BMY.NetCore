using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABIKYMALBILDIRIMI : TABOrtak
    {
        public TABIKYMALBILDIRIMI(VeriErisim veriErisim) : base(veriErisim) { }


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "IKYPERSONELMALBILDIRIMI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// PKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PKOD = 1,

            /// <summary>
            /// TARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 2,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "KOD"
            , "PKOD"
            , "TARIH"
            , "ACIKLAMA"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(PersonelMalBildirimi form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(PersonelMalBildirimi form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + ") OUTPUT INSERTED.KOD VALUES";
            sql += "(" + VeriErisim.DegerYap(form.personelKod);
            sql += "," + VeriErisim.DegerYap(form.tarih);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += ")";

            int eklenenDeger = veriErisim.TekKolonSQLInt(sql);
            Sonuc s = new Sonuc();

            s.anahtar = eklenenDeger.ToString();
            if (!string.IsNullOrWhiteSpace(veriErisim.hata))
                s.DegerAta(veriErisim.hata);

            return s;
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(PersonelMalBildirimi form)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET" + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], form.tarih);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], form.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }


        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(PersonelMalBildirimi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTASIT = "IKYMALBILTASIT";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTASIT : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// PLAKANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PLAKANO = 2,

            /// <summary>
            /// TASITINCINSI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TASITINCINSI = 3,

            /// <summary>
            /// TASITINMARKASI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TASITINMARKASI = 4,

            /// <summary>
            /// EDINMEDEGERI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EDINMEDEGERI = 5,

            /// <summary>
            /// MODELYILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODELYILI = 6,

            /// <summary>
            /// EDINMETARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EDINMETARIHI = 7,

            /// <summary>
            /// TCKIMLIKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 8,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTASIT = {
            "KOD"
            , "SIRANO"
            , "PLAKANO"
            , "TASITINCINSI"
            , "TASITINMARKASI"
            , "EDINMEDEGERI"
            , "MODELYILI"
            , "EDINMETARIHI"
            , "TCKIMLIKNO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilTasit(PersonelMalBildirimi form)
        {
            string sql = "DELETE FROM " + TABLOADTASIT;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTASIT[(int)ENUMAlanlarTASIT.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, malBilTasit dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <param name="satirNo">malBilTasit dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleTasit(int kod, int siraNo, MalBilTasit malBilTasit)
        {
            string sql = "INSERT INTO " + TABLOADTASIT + "(" + VeriErisim.TumAlanlar("", ALANLARTASIT) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(kod);
            sql += "," + VeriErisim.DegerYap(siraNo);
            sql += "," + VeriErisim.DegerYap(malBilTasit.plakaNo);
            sql += "," + VeriErisim.DegerYap(malBilTasit.tasitinCinsi);
            sql += "," + VeriErisim.DegerYap(malBilTasit.tasitinMarkasi);
            sql += "," + VeriErisim.DegerYap(malBilTasit.edinmeDegeri);
            sql += "," + VeriErisim.DegerYap(malBilTasit.modelYili);
            sql += "," + VeriErisim.DegerYap(malBilTasit.edinmeTarihi);
            sql += "," + VeriErisim.DegerYap(malBilTasit.tcKimlikNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuTasit(PersonelMalBildirimi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTASIT, ALANLARTASIT) + " FROM " + TABLOADTASIT;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTASIT[(int)ENUMAlanlarTASIT.KOD], "=", form.kod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public static string ListeleTasitSql(string ustTablo)
        {
            return $"SELECT {VeriErisim.TumAlanlar(TABLOADTASIT, ALANLARTASIT)} FROM {TABLOADTASIT} WHERE KOD={ustTablo}  FOR XML RAW, ELEMENTS xsinil, TYPE)";
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTASINMAZ = "IKYMALBILTASINMAZ";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTASINMAZ : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// DEGERI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGERI = 2,

            /// <summary>
            /// TASINMAZCINSI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TASINMAZCINSI = 3,

            /// <summary>
            /// ADRES alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ADRES = 4,

            /// <summary>
            /// HISSEMIKTARI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HISSEMIKTARI = 5,

            /// <summary>
            /// EDINMETARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EDINMETARIHI = 6,

            /// <summary>
            /// MALIKTCKN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MALIKTCKN = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTASINMAZ = {
            "KOD"
            , "SIRANO"
            , "DEGERI"
            , "TASINMAZCINSI"
            , "ADRES"
            , "HISSEMIKTARI"
            , "EDINMETARIHI"
            , "MALIKTCKN"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilTasinmaz(PersonelMalBildirimi form)
        {
            string sql = "DELETE FROM " + TABLOADTASINMAZ;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTASINMAZ[(int)ENUMAlanlarTASINMAZ.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, malBilTasinmaz dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <param name="satirNo">malBilTasinmaz dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleTasinmaz(int kod, int siraNo, MalBilTasinmaz malBilTasinmaz)
        {
            string sql = "INSERT INTO " + TABLOADTASINMAZ + "(" + VeriErisim.TumAlanlar("", ALANLARTASINMAZ) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(kod);
            sql += "," + VeriErisim.DegerYap(siraNo);
            sql += "," + VeriErisim.DegerYap(malBilTasinmaz.degeri);
            sql += "," + VeriErisim.DegerYap(malBilTasinmaz.tasinmazCinsi);
            sql += "," + VeriErisim.DegerYap(malBilTasinmaz.adres);
            sql += "," + VeriErisim.DegerYap(malBilTasinmaz.hisseMiktari);
            sql += "," + VeriErisim.DegerYap(malBilTasinmaz.edinmeTarihi);
            sql += "," + VeriErisim.DegerYap(malBilTasinmaz.malikTCKN);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuTasinmaz(PersonelMalBildirimi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTASINMAZ, ALANLARTASINMAZ) + " FROM " + TABLOADTASINMAZ;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTASINMAZ[(int)ENUMAlanlarTASINMAZ.KOD], "=", form.kod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public static string ListeleTasinmaz(string ustTablo)
        {
            return $"SELECT {VeriErisim.TumAlanlar(TABLOADTASINMAZ, ALANLARTASINMAZ)} FROM {TABLOADTASINMAZ} WHERE KOD={ustTablo}  FOR XML RAW, ELEMENTS xsinil, TYPE)";
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADKOOPERATIF = "IKYMALBILKOOPERATIF";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarKOOPERATIF : int
        {
            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// KOOPERATIFADIYERI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOOPERATIFADIYERI = 2,

            /// <summary>
            /// HISSEDEGERI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HISSEDEGERI = 3,

            /// <summary>
            /// UYELIKTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            UYELIKTARIHI = 4,

            /// <summary>
            /// TCKIMLIKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARKOOPERATIF = {
            "KOD"
            , "SIRANO"
            , "KOOPERATIFADIYERI"
            , "HISSEDEGERI"
            , "UYELIKTARIHI"
            , "TCKIMLIKNO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilKooperatif(PersonelMalBildirimi form)
        {
            string sql = "DELETE FROM " + TABLOADKOOPERATIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKOOPERATIF[(int)ENUMAlanlarKOOPERATIF.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, malBilKooperatif dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <param name="satirNo">malBilKooperatif dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleKooperatif(int kod, int siraNo, MalBilKooperatif malBilKooperatif)
        {
            string sql = "INSERT INTO " + TABLOADKOOPERATIF + "(" + VeriErisim.TumAlanlar("", ALANLARKOOPERATIF) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(kod);
            sql += "," + VeriErisim.DegerYap(siraNo);
            sql += "," + VeriErisim.DegerYap(malBilKooperatif.kooperatifAdiYeri);
            sql += "," + VeriErisim.DegerYap(malBilKooperatif.hisseDegeri);
            sql += "," + VeriErisim.DegerYap(malBilKooperatif.uyelikTarihi);
            sql += "," + VeriErisim.DegerYap(malBilKooperatif.tcKimlikNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuKooperatif(PersonelMalBildirimi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADKOOPERATIF, ALANLARKOOPERATIF) + " FROM " + TABLOADKOOPERATIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKOOPERATIF[(int)ENUMAlanlarKOOPERATIF.KOD], "=", form.kod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public static string ListeleKooperatif(string ustTablo)
        {
            return $"SELECT {VeriErisim.TumAlanlar(TABLOADKOOPERATIF, ALANLARKOOPERATIF)} FROM {TABLOADKOOPERATIF} WHERE KOD={ustTablo}  FOR XML RAW, ELEMENTS xsinil, TYPE)";
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADMALBILKIMLIK = "IKYMALBILKIMLIK";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarMALBILKIMLIK : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// ADISOYADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ADISOYADI = 2,

            /// <summary>
            /// DOGUMTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOGUMTARIHI = 3,

            /// <summary>
            /// DOGUMYERI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOGUMYERI = 4,

            /// <summary>
            /// YAKINLIGI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAKINLIGI = 5,

            /// <summary>
            /// TCKIMLIKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 6,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMALBILKIMLIK = {
            "KOD"
            , "SIRANO"
            , "ADISOYADI"
            , "DOGUMTARIHI"
            , "DOGUMYERI"
            , "YAKINLIGI"
            , "TCKIMLIKNO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilMalBilkimlik(PersonelMalBildirimi form)
        {
            string sql = "DELETE FROM " + TABLOADMALBILKIMLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMALBILKIMLIK[(int)ENUMAlanlarMALBILKIMLIK.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, malBilKimlik dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <param name="satirNo">malBilKimlik dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleMalBilkimlik(int kod, int siraNo, MalBilKimlik malBilKimlik)
        {
            string sql = "INSERT INTO " + TABLOADMALBILKIMLIK + "(" + VeriErisim.TumAlanlar("", ALANLARMALBILKIMLIK) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(kod);
            sql += "," + VeriErisim.DegerYap(malBilKimlik.siraNo);
            sql += "," + VeriErisim.DegerYap(malBilKimlik.adiSoyadi);
            sql += "," + VeriErisim.DegerYap(malBilKimlik.dogumTarihi);
            sql += "," + VeriErisim.DegerYap(malBilKimlik.dogumYeri);
            sql += "," + VeriErisim.DegerYap(malBilKimlik.yakinligi);
            sql += "," + VeriErisim.DegerYap(malBilKimlik.tcKimlikNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuMalBilkimlik(PersonelMalBildirimi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMALBILKIMLIK, ALANLARMALBILKIMLIK) + " FROM " + TABLOADMALBILKIMLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMALBILKIMLIK[(int)ENUMAlanlarMALBILKIMLIK.KOD], "=", form.kod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public static string ListeleMalBilkimlik(string ustTablo)
        {
            return $"SELECT {VeriErisim.TumAlanlar(TABLOADMALBILKIMLIK, ALANLARMALBILKIMLIK)} FROM {TABLOADMALBILKIMLIK} WHERE KOD={ustTablo}  FOR XML RAW, ELEMENTS xsinil, TYPE)";
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDIGERTASINIR = "IKYMALBILDIGERTASINIR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDIGERTASINIR : int
        {
            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// TASINIRMALCINSI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TASINIRMALCINSI = 2,

            /// <summary>
            /// EDINMEDEGERI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EDINMEDEGERI = 3,

            /// <summary>
            /// MODELYILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODELYILI = 4,

            /// <summary>
            /// EDINMETARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EDINMETARIHI = 5,

            /// <summary>
            /// TCKIMLIKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 6,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDIGERTASINIR = {
            "KOD"
            , "SIRANO"
            , "TASINIRMALCINSI"
            , "EDINMEDEGERI"
            , "MODELYILI"
            , "EDINMETARIHI"
            , "TCKIMLIKNO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilDigerTasinir(PersonelMalBildirimi form)
        {
            string sql = "DELETE FROM " + TABLOADDIGERTASINIR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIGERTASINIR[(int)ENUMAlanlarDIGERTASINIR.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, malBilDigerTasinir dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <param name="satirNo">malBilDigerTasinir dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleDigerTasinir(int kod, int siraNo, MalBilDigerTasinir malBilDigerTasinir)
        {
            string sql = "INSERT INTO " + TABLOADDIGERTASINIR + "(" + VeriErisim.TumAlanlar("", ALANLARDIGERTASINIR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(kod);
            sql += "," + VeriErisim.DegerYap(siraNo);
            sql += "," + VeriErisim.DegerYap(malBilDigerTasinir.tasinirMalCinsi);
            sql += "," + VeriErisim.DegerYap(malBilDigerTasinir.edinmeDegeri);
            sql += "," + VeriErisim.DegerYap(malBilDigerTasinir.modelYili);
            sql += "," + VeriErisim.DegerYap(malBilDigerTasinir.edinmeTarihi);
            sql += "," + VeriErisim.DegerYap(malBilDigerTasinir.tcKimlikNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuDigerTasinir(PersonelMalBildirimi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDIGERTASINIR, ALANLARDIGERTASINIR) + " FROM " + TABLOADDIGERTASINIR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIGERTASINIR[(int)ENUMAlanlarDIGERTASINIR.KOD], "=", form.kod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDIGERSERVET = "IKYMALBILDIGERSERVET";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDIGERSERVET : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// SERVETUNSURU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SERVETUNSURU = 2,

            /// <summary>
            /// EDINMESEKLI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EDINMESEKLI = 3,

            /// <summary>
            /// TCKIMLIKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDIGERSERVET = {
            "KOD"
            , "SIRANO"
            , "SERVETUNSURU"
            , "EDINMESEKLI"
            , "TCKIMLIKNO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilDigerServet(PersonelMalBildirimi form)
        {
            string sql = "DELETE FROM " + TABLOADDIGERSERVET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIGERSERVET[(int)ENUMAlanlarDIGERSERVET.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, malBilDigerServet dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <param name="satirNo">malBilDigerServet dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleDigerServet(int kod, int siraNo, MalBilDigerServet malBilDigerServet)
        {
            string sql = "INSERT INTO " + TABLOADDIGERSERVET + "(" + VeriErisim.TumAlanlar("", ALANLARDIGERSERVET) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(kod);
            sql += "," + VeriErisim.DegerYap(siraNo);
            sql += "," + VeriErisim.DegerYap(malBilDigerServet.servetUnsuru);
            sql += "," + VeriErisim.DegerYap(malBilDigerServet.edinmeSekli);
            sql += "," + VeriErisim.DegerYap(malBilDigerServet.tcKimlikNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuDigerServet(PersonelMalBildirimi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDIGERSERVET, ALANLARDIGERSERVET) + " FROM " + TABLOADDIGERSERVET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIGERSERVET[(int)ENUMAlanlarDIGERSERVET.KOD], "=", form.kod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADBORCALACAK = "IKYMALBILBORCALACAK";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarBORCALACAK : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// BORCLUADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCLUADI = 2,

            /// <summary>
            /// ALACAKLIADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALACAKLIADI = 3,

            /// <summary>
            /// TUTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTAR = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBORCALACAK = {
            "KOD"
            , "SIRANO"
            , "BORCLUADI"
            , "ALACAKLIADI"
            , "TUTAR"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilBorcAlacak(PersonelMalBildirimi form)
        {
            string sql = "DELETE FROM " + TABLOADBORCALACAK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBORCALACAK[(int)ENUMAlanlarBORCALACAK.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, malBilBorcAlacak dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <param name="satirNo">malBilBorcAlacak dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleBorcAlacak(int kod, int siraNo, MalBilBorcAlacak malBilBorcAlacak)
        {
            string sql = "INSERT INTO " + TABLOADBORCALACAK + "(" + VeriErisim.TumAlanlar("", ALANLARBORCALACAK) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(kod);
            sql += "," + VeriErisim.DegerYap(siraNo);
            sql += "," + VeriErisim.DegerYap(malBilBorcAlacak.borcluAdi);
            sql += "," + VeriErisim.DegerYap(malBilBorcAlacak.alacakliAdi);
            sql += "," + VeriErisim.DegerYap(malBilBorcAlacak.tutar);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuBorcAlacak(PersonelMalBildirimi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBORCALACAK, ALANLARBORCALACAK) + " FROM " + TABLOADBORCALACAK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBORCALACAK[(int)ENUMAlanlarBORCALACAK.KOD], "=", form.kod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADBANKAMENKUL = "IKYMALBILBANKAMENKUL";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarBANKAMENKUL : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// NITELIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NITELIK = 2,

            /// <summary>
            /// MIKTARI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTARI = 3,

            /// <summary>
            /// CINSI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CINSI = 4,

            /// <summary>
            /// TCKIMLIKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBANKAMENKUL = {
            "KOD"
            , "SIRANO"
            , "NITELIK"
            , "MIKTARI"
            , "CINSI"
            , "TCKIMLIKNO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilBankaMenkul(PersonelMalBildirimi form)
        {
            string sql = "DELETE FROM " + TABLOADBANKAMENKUL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBANKAMENKUL[(int)ENUMAlanlarBANKAMENKUL.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, malBilBankaMenkul dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <param name="satirNo">malBilBankaMenkul dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleBankaMenkul(int kod, int siraNo, MalBilBankaMenkul malBilBankaMenkul)
        {
            string sql = "INSERT INTO " + TABLOADBANKAMENKUL + "(" + VeriErisim.TumAlanlar("", ALANLARBANKAMENKUL) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(kod);
            sql += "," + VeriErisim.DegerYap(siraNo);
            sql += "," + VeriErisim.DegerYap(malBilBankaMenkul.nitelik);
            sql += "," + VeriErisim.DegerYap(malBilBankaMenkul.miktari);
            sql += "," + VeriErisim.DegerYap(malBilBankaMenkul.cinsi);
            sql += "," + VeriErisim.DegerYap(malBilBankaMenkul.tcKimlikNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuBankaMenkul(PersonelMalBildirimi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBANKAMENKUL, ALANLARBANKAMENKUL) + " FROM " + TABLOADBANKAMENKUL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBANKAMENKUL[(int)ENUMAlanlarBANKAMENKUL.KOD], "=", form.kod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADALTINMUCEVHER = "IKYMALBILALTINMUCEVHER";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarALTINMUCEVHER : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// CINSI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CINSI = 2,

            /// <summary>
            /// DEGERI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGERI = 3,

            /// <summary>
            /// MIKTARI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTARI = 4,

            /// <summary>
            /// TCKIMLIKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARALTINMUCEVHER = {
            "KOD"
            , "SIRANO"
            , "CINSI"
            , "DEGERI"
            , "MIKTARI"
            , "TCKIMLIKNO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilAltinMucevher(PersonelMalBildirimi form)
        {
            string sql = "DELETE FROM " + TABLOADALTINMUCEVHER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARALTINMUCEVHER[(int)ENUMAlanlarALTINMUCEVHER.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, malBilAltinMucevher dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <param name="satirNo">malBilAltinMucevher dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleAltinMucevher(int kod, int siraNo, MalBilAltinMucevher malBilAltinMucevher)
        {
            string sql = "INSERT INTO " + TABLOADALTINMUCEVHER + "(" + VeriErisim.TumAlanlar("", ALANLARALTINMUCEVHER) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(kod);
            sql += "," + VeriErisim.DegerYap(siraNo);
            sql += "," + VeriErisim.DegerYap(malBilAltinMucevher.cinsi);
            sql += "," + VeriErisim.DegerYap(malBilAltinMucevher.degeri);
            sql += "," + VeriErisim.DegerYap(malBilAltinMucevher.miktari);
            sql += "," + VeriErisim.DegerYap(malBilAltinMucevher.tcKimlikNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelMalBildirimi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuAltinMucevher(PersonelMalBildirimi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADALTINMUCEVHER, ALANLARALTINMUCEVHER) + " FROM " + TABLOADALTINMUCEVHER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARALTINMUCEVHER[(int)ENUMAlanlarALTINMUCEVHER.KOD], "=", form.kod));
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
