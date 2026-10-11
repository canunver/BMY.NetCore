using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using System.Data.SqlClient;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCDBIKRAZ veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABDBIKRAZ : TABOrtak
    {
        /// <summary>
        /// TABHRCDBIKRAZ sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABDBIKRAZ(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADIKRAZ = "DBIKRAZ";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string DBEKLENEBILECEKDOSYALAR = "DBEKLENEBILECEKDOSYALAR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarIKRAZ : int
        {

            /// <summary>
            /// IKRAZID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IKRAZID = 0,

            /// <summary>
            /// ANLASMAADITR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ANLASMAADITR = 1,

            /// <summary>
            /// ANLASMAADIEN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ANLASMAADIEN = 2,

            /// <summary>
            /// ANLASMAKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ANLASMAKODU = 3,

            /// <summary>
            /// BASLANGICYILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BASLANGICYILI = 4,

            /// <summary>
            /// BITISYILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BITISYILI = 5,

            /// <summary>
            /// PARABIRIMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PARABIRIMI = 6,

            /// <summary>
            /// DONUSUMKURU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DONUSUMKURU = 7,

            /// <summary>
            /// IKRAZNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IKRAZNO = 8,

            /// <summary>
            /// MBHESAPNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MBHESAPNO = 9,
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDBEKLENEBILECEKDOSYALAR : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// MODULKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODULKOD = 1,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,

            /// <summary>
            /// AD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARIKRAZ = {
            "IKRAZID"
            , "ANLASMAADITR"
            , "ANLASMAADIEN"
            , "ANLASMAKODU"
            , "BASLANGICYILI"
            , "BITISYILI"
            , "PARABIRIMI"
            , "DONUSUMKURU"
            , "IKRAZNO"
            , "MBHESAPNO"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDBEKLENEBILECEKDOSYALAR = {
            "KOD"
            , "MODULKOD"
            , "SIRANO"
            , "AD"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DBIkraz yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilIkraz(DBIkraz form)
        {
            string sql = "DELETE FROM " + TABLOADIKRAZ;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.IKRAZID], "=", form.ikrazID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBIkraz yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public int EkleIkraz(DBIkraz form)
        {
            string sql = "INSERT INTO " + TABLOADIKRAZ + "(" + VeriErisim.TumAlanlarEksik("", ALANLARIKRAZ, new int[] { 0 }) + ") OUTPUT INSERTED." + ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.IKRAZID] + " VALUES";
            sql += "(" + VeriErisim.DegerYap(form.anlasmaAdiTr);
            sql += "," + VeriErisim.DegerYap(form.anlasmaAdiEn);
            sql += "," + VeriErisim.DegerYap(form.anlasmaKodu);
            sql += "," + VeriErisim.DegerYap(form.baslangicYili);
            sql += "," + VeriErisim.DegerYap(form.bitisYili);
            sql += "," + VeriErisim.DegerYap(form.paraBirimi);
            sql += "," + VeriErisim.DegerYap(form.donusumKuru);
            sql += "," + VeriErisim.DegerYap(form.ikrazNo);
            sql += "," + VeriErisim.DegerYap(form.mbHesapNo);
            sql += ")";
            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBIkraz yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleIkraz(DBIkraz form)
        {
            string sql = "UPDATE " + TABLOADIKRAZ;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.ANLASMAADITR], form.anlasmaAdiTr);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.ANLASMAADIEN], form.anlasmaAdiEn);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.ANLASMAKODU], form.anlasmaKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.BASLANGICYILI], form.baslangicYili);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.BITISYILI], form.bitisYili);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.PARABIRIMI], form.paraBirimi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.DONUSUMKURU], form.donusumKuru);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.IKRAZNO], form.ikrazNo);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.MBHESAPNO], form.mbHesapNo);
            sql += " WHERE " + VeriErisim.AtamaOl("", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.IKRAZID], form.ikrazID);
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DBIkraz yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleIkrazlar(DBIkraz form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADIKRAZ, ALANLARIKRAZ) + " FROM " + TABLOADIKRAZ;
            if (form.ikrazID > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.IKRAZID], "=", form.ikrazID));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADBILESEN = "DBBILESEN";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarBILESEN : int
        {

            /// <summary>
            /// BILESENID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BILESENID = 0,

            /// <summary>
            /// IKRAZID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IKRAZID = 1,

            /// <summary>
            /// BILESENNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BILESENNO = 2,

            /// <summary>
            /// BILESENAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BILESENAD = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBILESEN = {
            "BILESENID"
            , "IKRAZID"
            , "BILESENNO"
            , "BILESENAD"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DBBilesen yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilBilesen(DBBilesen form)
        {
            string sql = "DELETE FROM " + TABLOADBILESEN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILESEN[(int)ENUMAlanlarBILESEN.BILESENID], "=", form.bilesenID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBBilesen yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public int EkleBilesen(DBBilesen form)
        {
            string sql = "INSERT INTO " + TABLOADBILESEN + "(" + VeriErisim.TumAlanlarEksik("", ALANLARBILESEN, new int[] { 0 }) + ") OUTPUT INSERTED." + ALANLARBILESEN[(int)ENUMAlanlarBILESEN.BILESENID] + " VALUES";
            sql += "(" + VeriErisim.DegerYap(form.ikrazID);
            sql += "," + VeriErisim.DegerYap(form.bilesenNo);
            sql += "," + VeriErisim.DegerYap(form.bilesenAd);
            sql += ")";
            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBBilesen yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleBilesen(DBBilesen form)
        {
            string sql = "UPDATE " + TABLOADBILESEN;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARBILESEN[(int)ENUMAlanlarBILESEN.IKRAZID], form.ikrazID);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBILESEN[(int)ENUMAlanlarBILESEN.BILESENNO], form.bilesenNo);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBILESEN[(int)ENUMAlanlarBILESEN.BILESENAD], form.bilesenAd);
            sql += " WHERE " + VeriErisim.AtamaOl("", ALANLARBILESEN[(int)ENUMAlanlarBILESEN.BILESENID], form.bilesenID);

            VeriErisim.DegerYap(form.bilesenID);
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DBBilesen yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleBilesenler(DBBilesen form)
        {
            string butceStr = "(SELECT SUM(TOPLAMBUTCE) FROM DBALTBILESEN WHERE DBALTBILESEN.BILESENID = B.BILESENID )";
            string sql = "SELECT " + VeriErisim.TumAlanlar("I", ALANLARIKRAZ) + ", " + VeriErisim.TumAlanlar("B", ALANLARBILESEN) + ", " + butceStr + " FROM " + TABLOADBILESEN + " B";
            sql += $" INNER JOIN {TABLOADIKRAZ} I ON B.IKRAZID=I.IKRAZID";

            string kosul = "";
            if (form.bilesenID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", ALANLARBILESEN[(int)ENUMAlanlarBILESEN.BILESENID], "=", form.bilesenID));
            if (form.ikrazID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", ALANLARBILESEN[(int)ENUMAlanlarBILESEN.IKRAZID], "=", form.ikrazID));
            if (!string.IsNullOrWhiteSpace(kosul))
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADAltBilesen = "DBALTBILESEN";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSozlesme = "DBSOZLESME";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarAltBilesen : int
        {

            /// <summary>
            /// ALTBILESENID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTBILESENID = 0,

            /// <summary>
            /// BILESENID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BILESENID = 1,

            /// <summary>
            /// ALTBILESENNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTBILESENNO = 2,

            /// <summary>
            /// ALTBILESENAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTBILESENAD = 3,

            /// <summary>
            /// ILGILIBIRIMNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILIBIRIMNO = 4,

            /// <summary>
            /// ILGILIBIRIMADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILIBIRIMADI = 5,

            /// <summary>
            /// ILGILIBIRIMBUTCEKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILIBIRIMBUTCEKODU = 6,

            /// <summary>
            /// TOPLAMBUTCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TOPLAMBUTCE = 7,
            BAGYIL = 8,
            ILGILIPEBKOD = 9
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARAltBilesen = {
            "ALTBILESENID"
            , "BILESENID"
            , "ALTBILESENNO"
            , "ALTBILESENAD"
            , "ILGILIBIRIMNO"
            , "ILGILIBIRIMADI"
            , "ILGILIBIRIMBUTCEKODU"
            , "TOPLAMBUTCE"
            , "BAGYIL"
            , "ILGILIPEBKOD"
        };

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSozlesme : int
        {
            /// <summary>SOZLESMEID alanının DataReader’daki sıra numarası</summary>
            SOZLESMEID = 0,

            /// <summary>BUTCEREFID alanının DataReader’daki sıra numarası</summary>
            BUTCEREFID = 1,

            /// <summary>SOZLESMENO alanının DataReader’daki sıra numarası</summary>
            SOZLESMENO = 2,

            /// <summary>KONU alanının DataReader’daki sıra numarası</summary>
            KONU = 3,

            /// <summary>ACIKLAMA alanının DataReader’daki sıra numarası</summary>
            ACIKLAMA = 4,

            /// <summary>FIRMAADI alanının DataReader’daki sıra numarası</summary>
            ILGILIKOD = 5,

            /// <summary>SOZLESMETARIHI alanının DataReader’daki sıra numarası</summary>
            SOZLESMETARIHI = 6,

            /// <summary>PARABIRIMI alanının DataReader’daki sıra numarası</summary>
            PARABIRIMI = 7,

            /// <summary>AVANSHUKMU alanının DataReader’daki sıra numarası</summary>
            AVANSHUKMU = 8,

            /// <summary>AVANSORANI alanının DataReader’daki sıra numarası</summary>
            AVANSORANI = 9,

            /// <summary>AVANSMAHSUBU alanının DataReader’daki sıra numarası</summary>
            AVANSMAHSUBU = 10,

            /// <summary>AVANSMAHSUBUORANI alanının DataReader’daki sıra numarası</summary>
            AVANSMAHSUBUORANI = 11,

            AVANSTEMINAT = 12,
            KESINTEMINAT = 13,
            IHTIYATKESINTISI = 14,
            SARFMALZEMESI = 15,
            DEMIRBAS = 16,
            DURUM = 17,
            BUTCEREFLOTID = 18,
            TAHMINIBASLAMATARIHI = 19,
            AVANSPARABIRIMI = 20,
            SOZLESMENOALMATARIHI = 21
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSozlesme = {
            "SOZLESMEID",          // 0
			"BUTCEREFID",          // 1
			"SOZLESMENO",          // 2
			"KONU",                // 3
			"ACIKLAMA",            // 4
			"ILGILIKOD",           // 5
			"SOZLESMETARIHI",      // 6
			"PARABIRIMI",          // 7
			"AVANSHUKMU",          // 8
			"AVANSORANI",          // 9
			"AVANSMAHSUBU",        //10
			"AVANSMAHSUBUORANI",   //11
            "AVANSTEMINAT",		   //12
            "KESINTEMINAT",		   //13 
            "IHTIYATKESINTISI",    //14 
            "SARFMALZEMESI",       //15 
            "DEMIRBAS",            //16 
            "DURUM",               //17 
            "BUTCEREFLOTID",       //18
            "TAHMINIBASLAMATARIHI",//19
            "AVANSPARABIRIMI",     //20
            "SOZLESMENOALMATARIHI" //21
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DBAltBilesen yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilAltBilesen(DBAltBilesen form)
        {
            string sql = "DELETE FROM " + TABLOADAltBilesen;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ALTBILESENID], "=", form.altBilesenID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBAltBilesen yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public int EkleAltBilesen(DBAltBilesen form)
        {
            string sql = "INSERT INTO " + TABLOADAltBilesen + "(" + VeriErisim.TumAlanlarEksik("", ALANLARAltBilesen, new int[] { 0 }) + ") OUTPUT INSERTED." + ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ALTBILESENID] + " VALUES";
            sql += "(" + VeriErisim.DegerYap(form.bilesenID);
            sql += "," + VeriErisim.DegerYap(form.altBilesenNo);
            sql += "," + VeriErisim.DegerYap(form.altBilesenAd);
            sql += "," + VeriErisim.DegerYap(form.ilgiliBirimNo);
            sql += "," + VeriErisim.DegerYap(form.ilgiliBirimAdi);
            sql += "," + VeriErisim.DegerYap(form.ilgiliBirimButceKodu.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.toplamButce);
            sql += "," + VeriErisim.DegerYap(form.bagYil);
            sql += "," + VeriErisim.DegerYap(form.ilgiliPEBKod.Replace(".", ""));
            sql += ")";
            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBAltBilesen yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleAltBilesen(DBAltBilesen form)
        {
            string sql = "UPDATE " + TABLOADAltBilesen;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.BILESENID], form.bilesenID);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ALTBILESENNO], form.altBilesenNo);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ALTBILESENAD], form.altBilesenAd);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ILGILIBIRIMNO], form.ilgiliBirimNo);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ILGILIBIRIMADI], form.ilgiliBirimAdi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ILGILIBIRIMBUTCEKODU], form.ilgiliBirimButceKodu.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.BAGYIL], form.bagYil);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ILGILIPEBKOD], form.ilgiliPEBKod.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.TOPLAMBUTCE], form.toplamButce);
            sql += " WHERE " + VeriErisim.AtamaOl("", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ALTBILESENID], form.altBilesenID);
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DBAltBilesen yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleAltBilesenler(DBAltBilesen form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("I", ALANLARIKRAZ) + ", " + VeriErisim.TumAlanlar("B", ALANLARBILESEN) + ", " + VeriErisim.TumAlanlar("AB", ALANLARAltBilesen) + ", KUR.KURAD, PEB.PEBAD FROM " + TABLOADAltBilesen + " AB";

            sql += $" INNER JOIN {TABLOADBILESEN} B ON B.BILESENID=AB.BILESENID";
            sql += $" INNER JOIN {TABLOADIKRAZ} I ON I.IKRAZID=B.IKRAZID";
            sql += $" LEFT OUTER JOIN ButceHazirlik.dbo.ABSKUR KUR ON KUR.YIL=AB.BAGYIL AND KUR.KURKOD=AB.ILGILIBIRIMBUTCEKODU";
            sql += $" LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD PEB ON PEB.YIL=AB.BAGYIL AND PEB.PEBKOD=AB.ILGILIPEBKOD";
            string kosul = "";
            if (form.ikrazID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("I", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.IKRAZID], "=", form.ikrazID));
            if (form.bilesenID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", ALANLARBILESEN[(int)ENUMAlanlarBILESEN.BILESENID], "=", form.bilesenID));
            if (form.altBilesenID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("AB", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ALTBILESENID], "=", form.altBilesenID));
            if (!string.IsNullOrWhiteSpace(kosul))
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADBUTCEREF = "DBBUTCEREF";
        public static string TABLOADBUTCEREFLOT = "DBBUTCEREFLOT";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarButceRef : int
        {
            /// <summary>
            /// BUTCEREFID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUTCEREFID = 0,

            /// <summary>
            /// SATINALMAID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATINALMAID = 1,

            /// <summary>
            /// REFERANSNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            REFERANSNO = 2,

            /// <summary>
            /// ACIKLAMATR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMATR = 3,

            /// <summary>
            /// ACIKLAMAEN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMAEN = 4,

            /// <summary>
            /// ODENEK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK = 5,

            /// <summary>
            /// NUMARATOR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 6,

        }

        public enum ENUMAlanlarButceRefLot : int
        {
            /// <summary>
            /// BUTCEREFLOTID alanının select sonrası DataReader indisi
            /// </summary>
            BUTCEREFLOTID = 0,

            /// <summary>
            /// BUTCEREFID alanının select sonrası DataReader indisi
            /// </summary>
            BUTCEREFID = 1,

            /// <summary>
            /// LOTNO alanının select sonrası DataReader indisi
            /// </summary>
            LOTNO = 2,

            /// <summary>
            /// ACIKLAMA alanının select sonrası DataReader indisi
            /// </summary>
            ACIKLAMA = 3
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBUTCEREF = {
            "BUTCEREFID"
            , "SATINALMAID"
            , "REFERANSNO"
            , "ACIKLAMATR"
            , "ACIKLAMAEN"
            , "ODENEK"
            , "DURUM"
        };

        static string[] ALANLARBUTCEREFLOT = {
            "BUTCEREFLOTID",
            "BUTCEREFID",
            "LOTNO",
            "ACIKLAMA"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DBButceRef yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilButceRef(int butceRefID)
        {
            string sql = "DELETE FROM " + TABLOADBUTCEREF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.BUTCEREFID], "=", butceRefID));
            return KomutCalistir(sql, 0);
        }

        public Sonuc SilButceRefLot(int butceRefID)
        {
            string sql = "DELETE FROM " + TABLOADBUTCEREFLOT;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBUTCEREFLOT[(int)ENUMAlanlarButceRefLot.BUTCEREFID], "=", butceRefID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBButceRef yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public int EkleButceRef(DBButceRef form)
        {
            string sql = "INSERT INTO " + TABLOADBUTCEREF + "(" + VeriErisim.TumAlanlarEksik("", ALANLARBUTCEREF, new int[] { 0 }) + ") OUTPUT INSERTED." + ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.BUTCEREFID] + " VALUES";
            sql += "(" + VeriErisim.DegerYap(form.satinAlmaID);
            sql += "," + VeriErisim.DegerYap(form.referansNo);
            sql += "," + VeriErisim.DegerYap(form.aciklamaTr);
            sql += "," + VeriErisim.DegerYap(form.aciklamaEn);
            sql += "," + VeriErisim.DegerYap(form.odenek);
            sql += "," + VeriErisim.DegerYap(1);
            sql += ")";
            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc GuncelleButceRef(DBButceRef form)
        {
            string sql = "UPDATE " + TABLOADBUTCEREF;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.REFERANSNO], form.referansNo);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.SATINALMAID], form.satinAlmaID);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.ACIKLAMATR], form.aciklamaTr);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.ACIKLAMAEN], form.aciklamaEn);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.ODENEK], form.odenek);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.BUTCEREFID], "=", form.butceRefID));
            return KomutCalistir(sql, 0);
        }

        public static string SatAlmaPlaniDetaySQL(string ustAlan, TNSDateTime raporTarih)
        {
            string sql = @"SELECT DBSATINALMAPLANIAYDAGILIM.DBSATINALMAPLANIAYDAGILIMID, DBSATINALMAPLANIAYDAGILIM.PLANTARIHI, DBSATINALMAPLANIAYDAGILIM.DURUM, DBSATINALMAPLANIAYDAGILIM.ACIKLAMA, DBSATINALMAPLANIAYDAGILIMDETAY.YIL, 
             DBSATINALMAPLANIAYDAGILIMDETAY.AY, DBSATINALMAPLANIAYDAGILIMDETAY.ODENEK
FROM   DBSATINALMAPLANIAYDAGILIM LEFT OUTER JOIN DBSATINALMAPLANIAYDAGILIMDETAY ON DBSATINALMAPLANIAYDAGILIM.DBSATINALMAPLANIAYDAGILIMID = DBSATINALMAPLANIAYDAGILIMDETAY.DBSATINALMAPLANIAYDAGILIMID
WHERE DBSATINALMAPLANIAYDAGILIM.SATINALMAID=" + ustAlan;
            if (!TNSDateTime.IsNull(raporTarih))
            {
                sql += $" AND DBSATINALMAPLANIAYDAGILIM.PLANTARIHI<={VeriErisim.DegerYap(raporTarih)}";
            }
            sql += " order by DBSATINALMAPLANIAYDAGILIM.PLANTARIHI,DBSATINALMAPLANIAYDAGILIM.DBSATINALMAPLANIAYDAGILIMID, YIL, AY";

            return "\r\n (" + sql + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DBButceRef yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleButceRef(DBButceRef form)
        {
            string sql = $"SELECT {VeriErisim.TumAlanlar("I", ALANLARIKRAZ)}, {VeriErisim.TumAlanlar("BIL", ALANLARBILESEN)}, {VeriErisim.TumAlanlar("ABIL", ALANLARAltBilesen)}, KUR.KURAD, {VeriErisim.TumAlanlar("BUT", ALANLARSATINALMA)}, {SatAlmaPlaniDetaySQL("BUT.SATINALMAID", null)}, {VeriErisim.TumAlanlar("REF", ALANLARBUTCEREF)}";
            sql += $" FROM {TABLOADBUTCEREF} REF INNER JOIN {TABLOADSATINALMA} BUT ON BUT.SATINALMAID=REF.SATINALMAID";
            sql += $" INNER JOIN {TABLOADAltBilesen} ABIL ON ABIL.ALTBILESENID=BUT.ALTBILESENID";
            sql += $" INNER JOIN {TABLOADBILESEN} BIL ON BIL.BILESENID=ABIL.BILESENID";
            sql += $" INNER JOIN {TABLOADIKRAZ} I ON I.IKRAZID=BIL.IKRAZID";
            sql += $" LEFT OUTER JOIN ButceHazirlik.dbo.ABSKUR KUR ON KUR.YIL=ABIL.BAGYIL AND KUR.KURKOD=ABIL.ILGILIBIRIMBUTCEKODU";

            string kosul = "";
            if (form.butceRefID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("REF", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.BUTCEREFID], "=", form.butceRefID));
            else
            {
                if (form.ikrazID > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("I", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.IKRAZID], "=", form.ikrazID));

                if (form.altBilesenID > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("ABIL", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ALTBILESENID], "=", form.altBilesenID));

                if (!string.IsNullOrWhiteSpace(form.ilgiliBirimAdi))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("ABIL", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ILGILIBIRIMADI], "=", form.ilgiliBirimAdi));
                if (!string.IsNullOrWhiteSpace(form.butceKategori))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("BUT", ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.BUTCEKATEGORI], "=", form.butceKategori));
                if (form.satinAlmaID > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("REF", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.SATINALMAID], "=", form.satinAlmaID));

                if (!string.IsNullOrWhiteSpace(form.butceKategori))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("BUT", ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.BUTCEKATEGORI], "=", form.butceKategori));

                if (!string.IsNullOrWhiteSpace(form.aciklamaTr))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("REF", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.ACIKLAMATR], "LIKE", "%" + form.aciklamaTr + "%"));

                //if (form.odenekAlt > 0)
                //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("REF", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.ODENEK], ">=", form.odenekAlt));

                //if (form.odenekUst > 0)
                //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("REF", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.ODENEK], "<=", form.odenekUst));

                if (!string.IsNullOrWhiteSpace(form.aramaMetni))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("REF", ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.ACIKLAMATR], "LIKE", "%" + form.aramaMetni + "%"));
                    kosul = VeriErisim.KosulEkle(kosul, "OR ", VeriErisim.KosulOl("BUT", ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.REFERANSNO], "LIKE", "%" + form.aramaMetni + "%")) + ")";
                }
            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            sql += " ORDER BY " + "I." + ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.IKRAZID] + ", "
                + "BIL." + ALANLARBILESEN[(int)ENUMAlanlarBILESEN.BILESENNO] + ", "
                + "ABIL." + ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ALTBILESENNO] + ", "
                + "BUT." + ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.REFERANSNO] + ", "
                + "REF." + ALANLARBUTCEREF[(int)ENUMAlanlarButceRef.REFERANSNO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSATINALMA = "DBSATINALMAPLANI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSatinAlma : int
        {
            /// <summary>
            /// SATINALMAID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATINALMAID = 0,

            /// <summary>
            /// ALTBILESENID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTBILESENID = 1,

            /// <summary>
            /// SATINALMANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATINALMANO = 2,

            /// <summary>
            /// SATINALMAGRUPNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATINALMAGRUPNO = 3,

            /// <summary>
            /// ACIKLAMATR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMATR = 4,

            /// <summary>
            /// ACIKLAMAEN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMAEN = 5,

            /// <summary>
            /// REFERANSNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            REFERANSNO = 6,

            /// <summary>
            /// REFERANSGRUPEK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            REFERANSGRUPEK = 7,

            /// <summary>
            /// REFERANSEK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            REFERANSEK = 8,

            /// <summary>
            /// BUTCEKATEGORI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUTCEKATEGORI = 9,

            /// <summary>
            /// SATINALMAYONTEMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATINALMAYONTEMI = 10,

            /// <summary>
            /// SECIMYONTEMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SECIMYONTEMI = 11,

            /// <summary>
            /// ONCESONRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONCESONRA = 12,

            /// <summary>
            /// TAHMINIDAVETTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TAHMINIDAVETTARIHI = 13,

            /// <summary>
            /// PLANLANANIMZATARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PLANLANANIMZATARIHI = 14,

            /// <summary>
            /// PLANLANANBITISTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PLANLANANBITISTARIHI = 15,

            /// <summary>
            /// ALTBIRIMAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTBIRIMAD = 16
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSATINALMA = {
            "SATINALMAID"
            , "ALTBILESENID"
            , "SATINALMANO"
            , "SATINALMAGRUPNO"
            , "ACIKLAMATR"
            , "ACIKLAMAEN"
            , "REFERANSNO"
            , "REFERANSGRUPEK"
            , "REFERANSEK"
            , "BUTCEKATEGORI"
            , "SATINALMAYONTEMI"
            , "SECIMYONTEMI"
            , "ONCESONRA"
            , "TAHMINIDAVETTARIHI"
            , "PLANLANANIMZATARIHI"
            , "PLANLANANBITISTARIHI"
            , "ALTBIRIMAD"
        };

        public IDataReader ListeleSatinAlma(DBSatinAlma form, int siralama)
        {
            string sql = $"SELECT {VeriErisim.TumAlanlar("I", ALANLARIKRAZ)}, {VeriErisim.TumAlanlar("BIL", ALANLARBILESEN)}, {VeriErisim.TumAlanlar("ABIL", ALANLARAltBilesen)}, {VeriErisim.TumAlanlar("BUT", ALANLARSATINALMA)},  {SatAlmaPlaniDetaySQL("BUT.SATINALMAID", form.kayitTarih)}";
            sql += $" FROM {TABLOADSATINALMA} BUT INNER JOIN {TABLOADAltBilesen} ABIL ON ABIL.ALTBILESENID=BUT.ALTBILESENID";
            sql += $" INNER JOIN {TABLOADBILESEN} BIL ON BIL.BILESENID=ABIL.BILESENID";
            sql += $" INNER JOIN {TABLOADIKRAZ} I ON I.IKRAZID=BIL.IKRAZID";

            string kosul = "";
            if (form.satinAlmaID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("BUT", ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.SATINALMAID], "=", form.satinAlmaID));
            if (form.ikrazID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("I", ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.IKRAZID], "=", form.ikrazID));

            if (!string.IsNullOrWhiteSpace(form.ilgiliBirimAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("ABIL", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ILGILIBIRIMADI], "=", form.ilgiliBirimAdi));

            if (!string.IsNullOrWhiteSpace(form.butceKategori))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("BUT", ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.BUTCEKATEGORI], "=", form.butceKategori));

            if (form.altBilesenID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("ABIL", ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ALTBILESENID], "=", form.altBilesenID));

            if (!string.IsNullOrWhiteSpace(form.aciklamaTr))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("BUT", ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.ACIKLAMATR], "LIKE", "%" + form.aciklamaTr + "%"));

            //if (form.odenekAlt > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("BUT", ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.ODENEK], ">=", form.odenekAlt));

            //if (form.odenekUst > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("BUT", ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.ODENEK], "<=", form.odenekUst));

            if (form.sorDurum == 1)
            {
                kosul = VeriErisim.KosulEkle(
                    kosul,
                    "AND",
                    @"EXISTS (
                    SELECT 1
                    FROM DBSATINALMAPLANIAYDAGILIM
                    WHERE DBSATINALMAPLANIAYDAGILIM.SATINALMAID = BUT.SATINALMAID
                      AND DBSATINALMAPLANIAYDAGILIM.DURUM IN (1, 2)
                )");
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            sql += " ORDER BY " + "I." + ALANLARIKRAZ[(int)ENUMAlanlarIKRAZ.IKRAZID] + ", "
                + "BIL." + ALANLARBILESEN[(int)ENUMAlanlarBILESEN.BILESENNO] + ", "
                + "ABIL." + ALANLARAltBilesen[(int)ENUMAlanlarAltBilesen.ALTBILESENNO] + ", "
                + "BUT." + ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.ALTBIRIMAD] + ", "
                + "BUT." + ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.BUTCEKATEGORI] + ", "
                + "BUT." + ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.REFERANSNO] + ", "
                + "BUT." + ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.SATINALMANO] + ", "
                + "BUT." + ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.SATINALMAGRUPNO] + ", "
                + "BUT." + ALANLARSATINALMA[(int)ENUMAlanlarSatinAlma.SATINALMAID];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleDBDosyaBilgileri(int kod, string modulKod)
        {
            string sql = "SELECT KOD, SIRANO, AD, MODULKOD FROM DBEKLENEBILECEKDOSYALAR";

            if (kod > 0)
            {
                sql += $" WHERE KOD = {VeriErisim.DegerYap(kod)}";
            }
            else
            {
                sql += $" WHERE MODULKOD = {VeriErisim.DegerYap(modulKod)} OR {VeriErisim.DegerYap(modulKod)} IS NULL";
            }

            // MODULKOD'a göre, ardından SIRANO'ya göre sıralama ekleniyor
            sql += " ORDER BY MODULKOD, SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

        public int EkleDosyaBilgi(DBYuklenebilecekDosyalar form)
        {
            string sql = "INSERT INTO " + DBEKLENEBILECEKDOSYALAR + "(" + VeriErisim.TumAlanlarEksik("", ALANLARDBEKLENEBILECEKDOSYALAR, new int[] { 0 }) + ") OUTPUT INSERTED." + ALANLARDBEKLENEBILECEKDOSYALAR[(int)ENUMAlanlarDBEKLENEBILECEKDOSYALAR.KOD] + " VALUES";
            sql += "(" + VeriErisim.DegerYap(form.modulKod);
            sql += "," + VeriErisim.DegerYap(form.siraNo);
            sql += "," + VeriErisim.DegerYap(form.dosyaAd);
            sql += ")";
            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc GuncelleDosyaBilgi(DBYuklenebilecekDosyalar form)
        {
            string sql = "UPDATE " + DBEKLENEBILECEKDOSYALAR;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARDBEKLENEBILECEKDOSYALAR[(int)ENUMAlanlarDBEKLENEBILECEKDOSYALAR.MODULKOD], form.modulKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARDBEKLENEBILECEKDOSYALAR[(int)ENUMAlanlarDBEKLENEBILECEKDOSYALAR.SIRANO], form.siraNo);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARDBEKLENEBILECEKDOSYALAR[(int)ENUMAlanlarDBEKLENEBILECEKDOSYALAR.AD], form.dosyaAd);
            sql += " WHERE " + VeriErisim.AtamaOl("", ALANLARDBEKLENEBILECEKDOSYALAR[(int)ENUMAlanlarDBEKLENEBILECEKDOSYALAR.KOD], form.kod);
            return KomutCalistir(sql, 1);
        }

        public static string OdemePlaniSql(string ustAlan, TNSDateTime raporTarih)
        {
            string sql = @"SELECT DBODEMEPLANI.ODEMEPLANID, SOZLESMEID, TAHMINISOZLESMEBITISTARIHI, ODEMEPLANITARIHI, DURUM, ACIKLAMA, ODEMEPLANITIPI, KURUSD.DOVIZSATIS, KUREUR.DOVIZSATIS, YIL, AY, ODENECEKTUTAR FROM DBODEMEPLANI";
            sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KURUSD ON KURUSD.KURTARIH = DBODEMEPLANI.ODEMEPLANITARIHI AND KURUSD.PARABIRIMI = 'USD'";
            sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KUREUR ON KUREUR.KURTARIH = DBODEMEPLANI.ODEMEPLANITARIHI AND KUREUR.PARABIRIMI = 'EUR'";

            sql += " LEFT OUTER JOIN DBODEMEPLANIDETAY ON DBODEMEPLANIDETAY.ODEMEPLANID = DBODEMEPLANI.ODEMEPLANID";
            sql += " WHERE SOZLESMEID = " + ustAlan;
            if (!TNSDateTime.IsNull(raporTarih))
            {
                sql += $" AND ODEMEPLANITARIHI <= {VeriErisim.DegerYap(raporTarih)}";
            }

            sql += " order by SOZLESMEID, ODEMEPLANITARIHI, ODEMEPLANID, YIL, AY";
            return sql;
        }

        public static string HakedislerSql(string ustAlan, TNSDateTime odemeSonTarih, bool mbOdemelerden, int ikrazID)
        {
            string sql;
            if (!mbOdemelerden)
            {
                sql = @"SELECT DBHAKEDIS.HAKEDISID, BELGETURKOD, BELGETARIHI, BELGENO, DBHAKEDIS.PARABIRIMI, KURTARIHI, KURUSD.DOVIZSATIS, KUREUR.DOVIZSATIS, KDVHARICBELGETOPLAMI + KDVTUTARI - AVANSKESINTISI ODENEN, NULL, NULL FROM DBHAKEDIS";
                sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KURUSD ON KURUSD.KURTARIH = DBHAKEDIS.KURTARIHI AND KURUSD.PARABIRIMI = 'USD'";
                sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KUREUR ON KUREUR.KURTARIH = DBHAKEDIS.KURTARIHI AND KUREUR.PARABIRIMI = 'EUR'";
                sql += " WHERE SOZLESMEID = " + ustAlan + " AND DURUMKOD<9";
                if (!TNSDateTime.IsNull(odemeSonTarih))
                {
                    sql += $" AND BELGETARIHI <= {VeriErisim.DegerYap(odemeSonTarih)}";
                }
                sql += " order by BELGETARIHI DESC";
            }
            else
            {
                sql = @"SELECT DBHAKEDIS.HAKEDISID,  DBHAKEDIS.BELGETURKOD, DBEKSTRELER.EKSDATE, DBHAKEDIS.BELGENO, DBHAKEDIS.PARABIRIMI, DBEKSTRELER.EKSDATE, KURUSD.DOVIZSATIS, KUREUR.DOVIZSATIS, DBEKSTRELER.AMOUNTPAID, DBEKSTRELER.TRYEQUIV, DBEKSTRELER.EUREQUIV FROM DBHAKEDIS ";

                sql += " INNER JOIN (SELECT HAKEDISID, NUMARATOR FROM  DBHAKEDISSAHIBI HS  WHERE HS.NUMARATOR > 0 GROUP BY HAKEDISID, NUMARATOR) X ON X.HAKEDISID =  DBHAKEDIS.HAKEDISID ";
                sql += " INNER JOIN DBEKSTRELER ON DBEKSTRELER.SGBNUMARATOR = X.NUMARATOR AND DBEKSTRELER.IKRAZID = " + VeriErisim.DegerYap(ikrazID);
                sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KURUSD ON KURUSD.KURTARIH = DBEKSTRELER.EKSDATE AND KURUSD.PARABIRIMI = 'USD'";
                sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KUREUR ON KUREUR.KURTARIH = DBEKSTRELER.EKSDATE AND KUREUR.PARABIRIMI = 'EUR'";
                sql += " WHERE SOZLESMEID = " + ustAlan;
                if (!TNSDateTime.IsNull(odemeSonTarih))
                {
                    sql += $" AND DBEKSTRELER.EKSDATE <= {VeriErisim.DegerYap(odemeSonTarih)}";
                }
                sql += " order by DBEKSTRELER.EKSDATE DESC";
            }
            return sql;
        }

        public IDataReader ListeleDBSozlesmeler(DBSozlesme sozlesme, int siralama = 1)
        {
            string alanlarOdemePlani = "\r\n (" + OdemePlaniSql("DBSOZLESME." + ALANLARSozlesme[(int)ENUMAlanlarSozlesme.SOZLESMEID], sozlesme.kayitTarih) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            string hakedisler = "\r\n (" + HakedislerSql("DBSOZLESME." + ALANLARSozlesme[(int)ENUMAlanlarSozlesme.SOZLESMEID], sozlesme.odemeSonTarih, sozlesme.mbOdemelerden, sozlesme.ikrazID) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";

            string sql = $@"SELECT {VeriErisim.TumAlanlar("DBSOZLESME", ALANLARSozlesme)}, HRCFIRMABILGISI.VERGINO, HRCFIRMABILGISI.AD, 
                         HRCFIRMABILGISI.HESAPNO, KURUSD.DOVIZSATIS, KUREUR.DOVIZSATIS, 
DBIKRAZ.IKRAZID, DBIKRAZ.ANLASMAKODU, 
DBBILESEN.BILESENID, DBBILESEN.BILESENNO, DBBILESEN.BILESENAD, 
DBALTBILESEN.ALTBILESENID, DBALTBILESEN.ALTBILESENNO, DBALTBILESEN.ALTBILESENAD, 
DBSATINALMAPLANI.BUTCEKATEGORI, DBSATINALMAPLANI.SATINALMAID, DBSATINALMAPLANI.REFERANSNO, DBSATINALMAPLANI.ACIKLAMATR, DBSATINALMAPLANI.ACIKLAMAEN, DBSATINALMAPLANI.REFERANSGRUPEK,
                         DBBUTCEREF.REFERANSNO, DBBUTCEREF.ACIKLAMATR, DBBUTCEREFLOT.LOTNO, DBBUTCEREFLOT.ACIKLAMA, {alanlarOdemePlani}, {hakedisler}
				FROM DBSOZLESME 
                INNER JOIN HRCFIRMABILGISI ON HRCFIRMABILGISI.KOD = DBSOZLESME.ILGILIKOD
				INNER JOIN DBBUTCEREF ON DBSOZLESME.BUTCEREFID = DBBUTCEREF.BUTCEREFID 
				INNER JOIN DBSATINALMAPLANI ON DBBUTCEREF.SATINALMAID = DBSATINALMAPLANI.SATINALMAID 
				INNER JOIN DBALTBILESEN ON DBSATINALMAPLANI.ALTBILESENID = DBALTBILESEN.ALTBILESENID 
				INNER JOIN DBBILESEN ON DBALTBILESEN.BILESENID = DBBILESEN.BILESENID 
				INNER JOIN DBIKRAZ ON DBBILESEN.IKRAZID = DBIKRAZ.IKRAZID
                LEFT OUTER JOIN DBBUTCEREFLOT ON DBBUTCEREFLOT.BUTCEREFLOTID = DBSOZLESME.BUTCEREFLOTID
                LEFT OUTER JOIN HRCMERKEZBANKKUR KURUSD ON KURUSD.KURTARIH = DBSOZLESME.SOZLESMETARIHI AND KURUSD.PARABIRIMI = 'USD'
                LEFT OUTER JOIN HRCMERKEZBANKKUR KUREUR ON KUREUR.KURTARIH = DBSOZLESME.SOZLESMETARIHI AND KUREUR.PARABIRIMI = 'EUR'";

            string kosul;
            if (sozlesme.sozlesmeID > 0)
            {
                kosul = "DBSOZLESME.SOZLESMEID = " + VeriErisim.DegerYap(sozlesme.sozlesmeID);
            }
            else
            {
                kosul = "";
                if (sozlesme.ikrazID > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBIKRAZ", "IKRAZID", "=", sozlesme.ikrazID));
                if (sozlesme.altBilesenID > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBALTBILESEN", "ALTBILESENID", "=", sozlesme.altBilesenID));
                if (sozlesme.butceRefID > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBBUTCEREF", "BUTCEREFID", "=", sozlesme.butceRefID));
                if (sozlesme.ilgiliKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBSOZLESME", "ILGILIKOD", "=", sozlesme.ilgiliKod));
                if (sozlesme.onceSonra > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBSATINALMAPLANI", "ONCESONRA", "=", sozlesme.onceSonra));

                if (sozlesme.sorDurum > 0)
                {
                    string inStr;
                    if (sozlesme.sorDurum == 1) inStr = " < 7";   // Hepsi
                    else if (sozlesme.sorDurum == 2) inStr = " < 5";  // Devam edilenler
                    else if (sozlesme.sorDurum == 3) inStr = " IN (5, 6)"; // Kapatılanlar
                    else if (sozlesme.sorDurum == 5) //Ödeme planı bekleyişte olanlar
                    {
                        kosul = VeriErisim.KosulEkle(
                            kosul,
                            "AND",
                            @"EXISTS (
                            SELECT 1
                            FROM DBODEMEPLANI
                            WHERE DBODEMEPLANI.SOZLESMEID = DBSOZLESME.SOZLESMEID
                              AND DBODEMEPLANI.DURUM IN (1, 2)
                        )");
                    }
                    else
                    {
                        inStr = " = 7";  //İptal
                        kosul = VeriErisim.KosulEkle(kosul, "AND", $"DBSOZLESME.DURUM {inStr} ");
                    }
                }


                if (!TNS.TNSDateTime.IsNull(sozlesme.sozTarihiBasla))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", $"DBSOZLESME.SOZLESMETARIHI >= {VeriErisim.DegerYap(sozlesme.sozTarihiBasla)} ");
                }

                if (!TNS.TNSDateTime.IsNull(sozlesme.sozTarihiBit))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", $"DBSOZLESME.SOZLESMETARIHI <= {VeriErisim.DegerYap(sozlesme.sozTarihiBit)} ");
                }

                if (!TNS.TNSDateTime.IsNull(sozlesme.odemeSonTarih))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", $"DBSOZLESME.SOZLESMETARIHI <= {VeriErisim.DegerYap(sozlesme.odemeSonTarih)} ");
                }

                if (!String.IsNullOrEmpty(sozlesme.paraBirimiSor))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", $"DBSOZLESME.PARABIRIMI = {VeriErisim.DegerYap(sozlesme.paraBirimiSor)} ");
                }

            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            if (siralama == 2)
                sql += " ORDER BY DBBILESEN.BILESENNO, DBALTBILESEN.ALTBILESENNO, DBSATINALMAPLANI.BUTCEKATEGORI, DBSATINALMAPLANI.REFERANSNO, DBBUTCEREFLOT.LOTNO, DBSOZLESME.SOZLESMETARIHI";
            else
                sql += " ORDER BY DBSOZLESME.SOZLESMETARIHI DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public static string HakedislerSqlSozlesmesiz(string ustAlan, TNSDateTime odemeSonTarih, bool mbOdemelerden, int ikrazID)
        {
            string sql;
            //if (!mbOdemelerden)
            //{
            //    sql = @"SELECT DBHAKEDIS.HAKEDISID, BELGETURKOD, BELGETARIHI, BELGENO, DBHAKEDIS.PARABIRIMI, KURTARIHI, KURUSD.DOVIZSATIS, KUREUR.DOVIZSATIS, KDVHARICBELGETOPLAMI + KDVTUTARI - AVANSKESINTISI ODENEN, NULL, NULL FROM DBHAKEDIS";
            //    sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KURUSD ON KURUSD.KURTARIH = DBHAKEDIS.KURTARIHI AND KURUSD.PARABIRIMI = 'USD'";
            //    sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KUREUR ON KUREUR.KURTARIH = DBHAKEDIS.KURTARIHI AND KUREUR.PARABIRIMI = 'EUR'";
            //    sql += " WHERE SOZLESMEID = " + ustAlan + " AND DURUMKOD>0";
            //    if (!TNSDateTime.IsNull(raporTarih))
            //    {
            //        sql += $" AND BELGETARIHI <= {VeriErisim.DegerYap(raporTarih)}";
            //    }
            //    sql += " order by BELGETARIHI DESC";
            //}
            //else
            {
                sql = @"SELECT IC.HAKEDISID,  IC.BELGETURKOD, DBEKSTRELER.EKSDATE, IC.BELGENO, IC.PARABIRIMI, DBEKSTRELER.EKSDATE, KURUSD.DOVIZSATIS, KUREUR.DOVIZSATIS, DBEKSTRELER.AMOUNTPAID, DBEKSTRELER.TRYEQUIV, DBEKSTRELER.EUREQUIV FROM DBHAKEDIS IC";

                sql += " INNER JOIN  (SELECT HAKEDISID, NUMARATOR FROM  DBHAKEDISSAHIBI HS  WHERE HS.NUMARATOR > 0 GROUP BY HAKEDISID, NUMARATOR) X  ON X.HAKEDISID =  IC.HAKEDISID ";
                sql += " INNER JOIN DBEKSTRELER ON DBEKSTRELER.SGBNUMARATOR = X.NUMARATOR AND DBEKSTRELER.IKRAZID = " + VeriErisim.DegerYap(ikrazID);
                sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KURUSD ON KURUSD.KURTARIH = DBEKSTRELER.EKSDATE AND KURUSD.PARABIRIMI = 'USD'";
                sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KUREUR ON KUREUR.KURTARIH = DBEKSTRELER.EKSDATE AND KUREUR.PARABIRIMI = 'EUR'";
                sql += " WHERE IC.HAKEDISID = " + ustAlan;
                if (!TNSDateTime.IsNull(odemeSonTarih))
                {
                    sql += $" AND DBEKSTRELER.EKSDATE <= {VeriErisim.DegerYap(odemeSonTarih)}";
                }
                sql += " order by DBEKSTRELER.EKSDATE DESC";
            }
            return sql;
        }

        public IDataReader ListeleSozlesmesizOdemeler(DBSozlesme sozlesme, int siralama = 1)
        {
            string hakedisler = "\r\n (" + HakedislerSqlSozlesmesiz("DBHAKEDIS.HAKEDISID", sozlesme.odemeSonTarih, sozlesme.mbOdemelerden, sozlesme.ikrazID) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";

            string sql = $@"SELECT DBHAKEDIS.BUTCEREFID, DBHAKEDIS.ILGILIKOD, DBHAKEDIS.PARABIRIMI, HRCFIRMABILGISI.VERGINO, HRCFIRMABILGISI.AD, HRCFIRMABILGISI.HESAPNO, KURUSD.DOVIZSATIS, KUREUR.DOVIZSATIS, DBIKRAZ.IKRAZID, DBIKRAZ.ANLASMAKODU, DBBILESEN.BILESENID, DBBILESEN.BILESENNO, DBBILESEN.BILESENAD, 
DBALTBILESEN.ALTBILESENID, DBALTBILESEN.ALTBILESENNO, DBALTBILESEN.ALTBILESENAD, 
DBSATINALMAPLANI.BUTCEKATEGORI, DBSATINALMAPLANI.SATINALMAID, DBSATINALMAPLANI.REFERANSNO, DBSATINALMAPLANI.ACIKLAMATR, DBSATINALMAPLANI.ACIKLAMAEN, DBSATINALMAPLANI.REFERANSGRUPEK,
                         DBBUTCEREF.REFERANSNO, DBBUTCEREF.ACIKLAMATR, {hakedisler}
				FROM DBHAKEDIS 
                INNER JOIN HRCFIRMABILGISI ON HRCFIRMABILGISI.KOD = DBHAKEDIS.ILGILIKOD
				INNER JOIN DBBUTCEREF ON DBBUTCEREF.BUTCEREFID = DBHAKEDIS.BUTCEREFID 
				INNER JOIN DBSATINALMAPLANI ON DBBUTCEREF.SATINALMAID = DBSATINALMAPLANI.SATINALMAID 
				INNER JOIN DBALTBILESEN ON DBSATINALMAPLANI.ALTBILESENID = DBALTBILESEN.ALTBILESENID 
				INNER JOIN DBBILESEN ON DBALTBILESEN.BILESENID = DBBILESEN.BILESENID 
				INNER JOIN DBIKRAZ ON DBBILESEN.IKRAZID = DBIKRAZ.IKRAZID
                LEFT OUTER JOIN HRCMERKEZBANKKUR KURUSD ON KURUSD.KURTARIH = DBHAKEDIS.KURTARIHI AND KURUSD.PARABIRIMI = 'USD'
                LEFT OUTER JOIN HRCMERKEZBANKKUR KUREUR ON KUREUR.KURTARIH = DBHAKEDIS.KURTARIHI AND KUREUR.PARABIRIMI = 'EUR'";

            string kosul = "";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBIKRAZ", "IKRAZID", "=", sozlesme.ikrazID));
            kosul = VeriErisim.KosulEkle(kosul, "AND", "DBHAKEDIS.SOZLESMEID IS NULL");

            if (sozlesme.altBilesenID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBALTBILESEN", "ALTBILESENID", "=", sozlesme.altBilesenID));
            if (sozlesme.butceRefID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBBUTCEREF", "BUTCEREFID", "=", sozlesme.butceRefID));
            if (sozlesme.ilgiliKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBHAKEDIS", "ILGILIKOD", "=", sozlesme.ilgiliKod));
            if (sozlesme.onceSonra > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBSATINALMAPLANI", "ONCESONRA", "=", sozlesme.onceSonra));

            //if (sozlesme.sorDurum > 0)
            //{
            //    string inStr;
            //    if (sozlesme.sorDurum == 1) inStr = " < 7";   // Hepsi
            //    else if (sozlesme.sorDurum == 2) inStr = " < 5";  // Devam edilenler
            //    else if (sozlesme.sorDurum == 3) inStr = " IN (5, 6)"; // Kapatılanlar
            //    else inStr = " = 7";  //İptal
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", $"DBSOZLESME.DURUM {inStr} ");
            //}

            //if (!TNS.TNSDateTime.IsNull(sozlesme.sozTarihiBasla))
            //{
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", $"DBSOZLESME.SOZLESMETARIHI >= {VeriErisim.DegerYap(sozlesme.sozTarihiBasla)} ");
            //}

            //if (!TNS.TNSDateTime.IsNull(sozlesme.sozTarihiBit))
            //{
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", $"DBSOZLESME.SOZLESMETARIHI <= {VeriErisim.DegerYap(sozlesme.sozTarihiBit)} ");
            //}

            //if (!String.IsNullOrEmpty(sozlesme.paraBirimiSor))
            //{
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", $"DBSOZLESME.PARABIRIMI = {VeriErisim.DegerYap(sozlesme.paraBirimiSor)} ");
            //}
            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            //if (siralama == 2)
            sql += " ORDER BY DBBILESEN.BILESENNO, DBALTBILESEN.ALTBILESENNO, DBSATINALMAPLANI.BUTCEKATEGORI, DBSATINALMAPLANI.REFERANSNO";
            //else
            //    sql += " ORDER BY DBSOZLESME.SOZLESMETARIHI DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public int EkleSozlesme(DBSozlesme form)
        {
            string sql = "INSERT INTO " + TABLOADSozlesme + "(" + VeriErisim.TumAlanlarEksik("", ALANLARSozlesme, new int[] { 0 }) + ") OUTPUT INSERTED." + ALANLARSozlesme[(int)ENUMAlanlarSozlesme.SOZLESMEID] + " VALUES(";

            sql += VeriErisim.DegerYap(form.butceRefID);
            sql += "," + VeriErisim.DegerYap(form.sozlesmeNo);
            sql += "," + VeriErisim.DegerYap(form.konu);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += "," + VeriErisim.DegerYap(form.ilgiliKod);
            sql += "," + VeriErisim.DegerYap(form.sozlesmeTarihi);
            sql += "," + VeriErisim.DegerYap(form.paraBirimi);
            sql += "," + VeriErisim.DegerYap(form.avansHukmu);
            sql += "," + VeriErisim.DegerYap(form.avansOrani);
            sql += "," + VeriErisim.DegerYap(form.avansMahsubu);
            sql += "," + VeriErisim.DegerYap(form.avansMahsubuOrani);
            sql += "," + VeriErisim.DegerYap(form.avansTeminatMektubu);
            sql += "," + VeriErisim.DegerYap(form.kesinTeminatMektubu);
            sql += "," + VeriErisim.DegerYap(form.ihtiyatKesintisiIceriyorMu ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(form.sarfMalzemesiVarMi ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(form.demirbasVarMi ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(1);
            sql += "," + (form.butceRefLotID == 0 ? "NULL" : VeriErisim.DegerYap(form.butceRefLotID));
            sql += "," + VeriErisim.DegerYap(form.tahminiBaslamaTarihi);
            sql += "," + VeriErisim.DegerYap(form.avansParaBirimi);
            sql += ", NULL";
            sql += ")";

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc GuncelleSozlesme(DBSozlesme form)
        {
            string sql = "UPDATE " + TABLOADSozlesme;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.BUTCEREFID], form.butceRefID);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.SOZLESMENO], form.sozlesmeNo);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.KONU], form.konu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.ACIKLAMA], form.aciklama);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.ILGILIKOD], form.ilgiliKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.SOZLESMETARIHI], form.sozlesmeTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.PARABIRIMI], form.paraBirimi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.AVANSHUKMU], form.avansHukmu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.AVANSORANI], form.avansOrani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.AVANSMAHSUBU], form.avansMahsubu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.AVANSMAHSUBUORANI], form.avansMahsubuOrani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.AVANSTEMINAT], form.avansTeminatMektubu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.KESINTEMINAT], form.kesinTeminatMektubu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.IHTIYATKESINTISI], form.ihtiyatKesintisiIceriyorMu ? 1 : 0);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.SARFMALZEMESI], form.sarfMalzemesiVarMi ? 1 : 0);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.DEMIRBAS], form.demirbasVarMi ? 1 : 0);
            sql += ", " + ALANLARSozlesme[(int)ENUMAlanlarSozlesme.BUTCEREFLOTID] + "=" + (form.butceRefLotID == 0 ? "NULL" : VeriErisim.DegerYap(form.butceRefLotID));
            sql += "," + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.TAHMINIBASLAMATARIHI], form.tahminiBaslamaTarihi);
            sql += "," + VeriErisim.AtamaOl("", ALANLARSozlesme[(int)ENUMAlanlarSozlesme.AVANSPARABIRIMI], form.avansParaBirimi);

            sql += " WHERE " + ALANLARSozlesme[(int)ENUMAlanlarSozlesme.SOZLESMEID] + " = " + form.sozlesmeID;
            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleButceRefLot(int butceRefID)
        {
            string sql = @"SELECT BUTCEREFLOTID, BUTCEREFID, LOTNO, ACIKLAMA FROM DBBUTCEREFLOT WHERE BUTCEREFID = " + VeriErisim.DegerYap(butceRefID);

            return veriErisim.DataReaderDondur(sql);
        }

        public int EkleButceRefLot(DBButceRefLot item)
        {
            string sql = $@"INSERT INTO DBBUTCEREFLOT (BUTCEREFID, LOTNO, ACIKLAMA) OUTPUT INSERTED.BUTCEREFLOTID VALUES ({VeriErisim.DegerYap(item.butceRefID)}, {VeriErisim.DegerYap(item.lotNo)}, {VeriErisim.DegerYap(item.aciklama)})";

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc GuncelleButceRefLot(DBButceRefLot item)
        {
            string sql = $@"UPDATE DBBUTCEREFLOT SET BUTCEREFID={VeriErisim.DegerYap(item.butceRefID)}, LOTNO={VeriErisim.DegerYap(item.lotNo)}, ACIKLAMA={VeriErisim.DegerYap(item.aciklama)} WHERE BUTCEREFLOTID = {VeriErisim.DegerYap(item.butceRefLotID)}";
            return KomutCalistir(sql, 1);
        }

        public IDataReader SozlemeOdemePlaniListele(int odemePlanID)
        {
            string sql = $@"SELECT DBODEMEPLANI.ODEMEPLANID, DBODEMEPLANI.TAHMINISOZLESMEBITISTARIHI, DBODEMEPLANI.ODEMEPLANITARIHI, DBODEMEPLANI.DURUM, DBODEMEPLANI.ACIKLAMA,
                    DBODEMEPLANI.ODEMEPLANITIPI, DBSOZLESME.PARABIRIMI, KURUSD.DOVIZSATIS, KUREUR.DOVIZSATIS
                FROM DBODEMEPLANI
                INNER JOIN DBSOZLESME ON DBSOZLESME.SOZLESMEID = DBODEMEPLANI.SOZLESMEID
                LEFT OUTER JOIN HRCMERKEZBANKKUR KURUSD ON KURUSD.KURTARIH = DBODEMEPLANI.ODEMEPLANITARIHI AND KURUSD.PARABIRIMI = 'USD'
                LEFT OUTER JOIN HRCMERKEZBANKKUR KUREUR ON KUREUR.KURTARIH = DBODEMEPLANI.ODEMEPLANITARIHI AND KUREUR.PARABIRIMI = 'EUR'
                WHERE DBODEMEPLANI.ODEMEPLANID = {VeriErisim.DegerYap(odemePlanID)}";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader SozlemeOdemePlaniDetayListele(int odemePlanID)
        {
            string sql = @"
        SELECT 
            DBODEMEPLANIDETAY.ODEMEPLANID,
	        DBODEMEPLANIDETAY.YIL, 
	        DBODEMEPLANIDETAY.AY, 
	        DBODEMEPLANIDETAY.ODENECEKTUTAR
        FROM 
	        DBODEMEPLANIDETAY
        WHERE 
	        DBODEMEPLANIDETAY.ODEMEPLANID = " + VeriErisim.DegerYap(odemePlanID);

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleOdemePlani(int sozlesmeID)
        {
            string sql = @"
            SELECT 
                DBODEMEPLANI.ODEMEPLANID
            FROM 
                DBODEMEPLANI
            WHERE 
                DBODEMEPLANI.SOZLESMEID = " + VeriErisim.DegerYap(sozlesmeID);

            return veriErisim.DataReaderDondur(sql);
        }

        public int EkleOdemePlani(DBOdemePlani form)
        {
            string sql = $@"
                INSERT INTO DBODEMEPLANI 
                (SOZLESMEID, TAHMINISOZLESMEBITISTARIHI, ODEMEPLANITARIHI, DURUM, ACIKLAMA, ODEMEPLANITIPI)
                OUTPUT INSERTED.ODEMEPLANID
                VALUES (
                    {VeriErisim.DegerYap(form.sozlesmeID)},
                    {VeriErisim.DegerYap(form.tahminiSozlesmeBitisi)},
                    {VeriErisim.DegerYap(form.odemePlaniTarihi)},
                    {VeriErisim.DegerYap(1)},
                    {VeriErisim.DegerYap(form.aciklama)},
                    {VeriErisim.DegerYap(form.odemePlaniTipi)}
                )";

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc GuncelleOdemePlani(DBOdemePlani form)
        {
            string sql = $@"
                UPDATE DBODEMEPLANI SET
                    SOZLESMEID = {VeriErisim.DegerYap(form.sozlesmeID)},
                    TAHMINISOZLESMEBITISTARIHI = {VeriErisim.DegerYap(form.tahminiSozlesmeBitisi)},
                    ODEMEPLANITARIHI = {VeriErisim.DegerYap(form.odemePlaniTarihi)},
                    DURUM = 1,
                    ACIKLAMA = {VeriErisim.DegerYap(form.aciklama)},
                    ODEMEPLANITIPI = {VeriErisim.DegerYap(form.odemePlaniTipi)}
                WHERE
                    ODEMEPLANID = {VeriErisim.DegerYap(form.odemePlanID)}";

            return KomutCalistir(sql, 1);
        }

        public Sonuc DurumDegistirSozlesmeOdemePlani(DBOdemePlani form)
        {
            string sql = $@"
                UPDATE DBODEMEPLANI SET
                    DURUM = {VeriErisim.DegerYap(form.durum)}
                WHERE
                    ODEMEPLANID = {VeriErisim.DegerYap(form.odemePlanID)}";

            return KomutCalistir(sql, 1);
        }

        public Sonuc EkleOdemePlaniDetay(DBOdemePlaniDetay form)
        {
            string sql = $@"
		INSERT INTO DBODEMEPLANIDETAY (ODEMEPLANID, YIL, AY, ODENECEKTUTAR)
		VALUES (
			{VeriErisim.DegerYap(form.odemePlanID)},
			{VeriErisim.DegerYap(form.yil)},
			{VeriErisim.DegerYap(form.ay)},
			{VeriErisim.DegerYap(form.odenecekTutar)}
		)";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilOdemePlaniDetay(DBOdemePlani form)
        {
            string sql = $@"
                DELETE FROM DBODEMEPLANIDETAY 
                WHERE ODEMEPLANID = {VeriErisim.DegerYap(form.odemePlanID)}";

            return KomutCalistir(sql, 0);
        }

        public int EkleSatinAlma(DBSatinAlma satinAlma)
        {
            string sql = $@"
                INSERT INTO DBSATINALMAPLANI (
                    ALTBILESENID,
                    REFERANSNO,
                    ACIKLAMATR,
                    ACIKLAMAEN,
                    BUTCEKATEGORI,
                    SATINALMANO,
                    SATINALMAGRUPNO,
                    REFERANSEK,
                    REFERANSGRUPEK,
                    SATINALMAYONTEMI,
                    SECIMYONTEMI,
                    ONCESONRA,
                    PLANLANANIMZATARIHI,
                    PLANLANANBITISTARIHI,
                    ALTBIRIMAD,
                    TAHMINIDAVETTARIHI
                ) VALUES (
                    {VeriErisim.DegerYap(satinAlma.altBilesenID)},
                    {VeriErisim.DegerYap(satinAlma.referansNo)},
                    {VeriErisim.DegerYap(satinAlma.aciklamaTr)},
                    {VeriErisim.DegerYap(satinAlma.aciklamaEn)},
                    {VeriErisim.DegerYap(satinAlma.butceKategori)},
                    {VeriErisim.DegerYap(satinAlma.satinAlmaNo)},
                    {VeriErisim.DegerYap(satinAlma.satinAlmaGrupNo)},
                    {VeriErisim.DegerYap(satinAlma.referansEk)},
                    {VeriErisim.DegerYap(satinAlma.referansGrupEk)},
                    {VeriErisim.DegerYap(satinAlma.satinAlmaYontemi)},
                    {VeriErisim.DegerYap(satinAlma.secimYontemi)},
                    {VeriErisim.DegerYap(satinAlma.onceSonra)},
                    {VeriErisim.DegerYap(satinAlma.planlananImzaTarihi)},
                    {VeriErisim.DegerYap(satinAlma.planlananBitisTarihi)},
                    {VeriErisim.DegerYap(satinAlma.altBirimAd)},
                    {VeriErisim.DegerYap(satinAlma.tahminiDavetTarihi)}
                );";

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc GuncelleSatinAlma(DBSatinAlma satinAlma)
        {
            string sql = $@"
                UPDATE DBSATINALMAPLANI SET
                    ALTBILESENID = {VeriErisim.DegerYap(satinAlma.altBilesenID)},
                    REFERANSNO = {VeriErisim.DegerYap(satinAlma.referansNo)},
                    ACIKLAMATR = {VeriErisim.DegerYap(satinAlma.aciklamaTr)},
                    ACIKLAMAEN = {VeriErisim.DegerYap(satinAlma.aciklamaEn)},
                    BUTCEKATEGORI = {VeriErisim.DegerYap(satinAlma.butceKategori)},
                    SATINALMANO = {VeriErisim.DegerYap(satinAlma.satinAlmaNo)},
                    SATINALMAGRUPNO = {VeriErisim.DegerYap(satinAlma.satinAlmaGrupNo)},
                    REFERANSEK = {VeriErisim.DegerYap(satinAlma.referansEk)},
                    REFERANSGRUPEK = {VeriErisim.DegerYap(satinAlma.referansGrupEk)},
                    SATINALMAYONTEMI = {VeriErisim.DegerYap(satinAlma.satinAlmaYontemi)},
                    SECIMYONTEMI = {VeriErisim.DegerYap(satinAlma.secimYontemi)},
                    ONCESONRA = {VeriErisim.DegerYap(satinAlma.onceSonra)},
                    PLANLANANIMZATARIHI = {VeriErisim.DegerYap(satinAlma.planlananImzaTarihi)},
                    PLANLANANBITISTARIHI = {VeriErisim.DegerYap(satinAlma.planlananBitisTarihi)},
                    ALTBIRIMAD = {VeriErisim.DegerYap(satinAlma.altBirimAd)},
                    TAHMINIDAVETTARIHI = {VeriErisim.DegerYap(satinAlma.tahminiDavetTarihi)}
                WHERE
                    SATINALMAID = {VeriErisim.DegerYap(satinAlma.satinAlmaID)};";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SozlesmeNoYaz(DBSozlesme sozlesme)
        {
            string sql = $@"UPDATE DBSOZLESME
                SET SOZLESMENO = {sozlesme.sozlesmeNo}, SOZLESMENOALMATARIHI={VeriErisim.DegerYap(sozlesme.sozlesmeNoAlmaTarihi)}
                WHERE SOZLESMEID = {sozlesme.sozlesmeID}";
            return KomutCalistir(sql, 1);
        }

        public Sonuc SozlesmeKapat(DBSozlesme sozlesme)
        {
            string sql = $@"UPDATE DBSOZLESME
                SET DURUM = {sozlesme.durum}, GEREKCE={VeriErisim.DegerYap(sozlesme.gerekce)}
                WHERE SOZLESMEID = {sozlesme.sozlesmeID}";
            return KomutCalistir(sql, 1);
        }

        public Sonuc SilSozlesme(int sozlesmeID)
        {
            string sql = $@"
                DELETE FROM DBSOZLESME
                WHERE SOZLESMEID = {VeriErisim.DegerYap(sozlesmeID)}
            ";
            return KomutCalistir(sql, 1);
        }

        public Sonuc SilOdemePlani(int sozlesmeID)
        {
            string sql = $@"
                DELETE FROM DBODEMEPLANI
                WHERE SOZLESMEID = {VeriErisim.DegerYap(sozlesmeID)}
            ";

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilOdemePlaniDetay(int sozlesmeID)
        {
            string sql = $@"
                DELETE FROM DBODEMEPLANIDETAY
                WHERE ODEMEPLANID IN (
                    SELECT ODEMEPLANID
                    FROM DBODEMEPLANI
                    WHERE SOZLESMEID = {VeriErisim.DegerYap(sozlesmeID)}
                )
            ";

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilSatinAlmaPlaniDagilim(int satinAlmaID)
        {
            string sql = $@"
                DELETE FROM DBSATINALMAPLANIAYDAGILIM
                WHERE SATINALMAID = {VeriErisim.DegerYap(satinAlmaID)}
            ";

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilSatinAlmaPlaniDagilimDetay(int sozlesmeID)
        {
            string sql = $@"
                DELETE FROM DBSATINALMAPLANIAYDAGILIMDETAY
                WHERE DBSATINALMAPLANIAYDAGILIMID IN (
                    SELECT DBSATINALMAPLANIAYDAGILIMID
                    FROM DBSATINALMAPLANIAYDAGILIM
                    WHERE SATINALMAID = {VeriErisim.DegerYap(sozlesmeID)}
                )
            ";

            return KomutCalistir(sql, 0);
        }

        public int SozlesmeNoBul(int ikrazID)
        {
            string sql = $@"SELECT MAX(s.SOZLESMENO)
                    FROM   dbo.DBSOZLESME s WITH (UPDLOCK, HOLDLOCK)
                    JOIN   dbo.DBBUTCEREF br       ON br.BUTCEREFID   = s.BUTCEREFID
                    JOIN   dbo.DBSATINALMAPLANI sp ON sp.SATINALMAID  = br.SATINALMAID
                    JOIN   dbo.DBALTBILESEN ab     ON ab.ALTBILESENID = sp.ALTBILESENID
                    JOIN   dbo.DBBILESEN b         ON b.BILESENID     = ab.BILESENID
                    WHERE  b.IKRAZID = {ikrazID}";
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        public int EkleSatinAlmaGerceklesmePlani(DBSatinAlmaGerceklesmePlan gercPlan)
        {
            string sql = $@"
                INSERT INTO DBSATINALMAPLANIAYDAGILIM
                (SATINALMAID, PLANTARIHI, DURUM, ACIKLAMA)
                OUTPUT INSERTED.DBSATINALMAPLANIAYDAGILIMID
                VALUES (
                    {VeriErisim.DegerYap(gercPlan.satinAlmaID)},
                    {VeriErisim.DegerYap(gercPlan.planTarihi)},
                    {VeriErisim.DegerYap(1)},
                    {VeriErisim.DegerYap(gercPlan.aciklama)}
                )";

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc GuncelleSatinAlmaGerceklesmePlani(DBSatinAlmaGerceklesmePlan gercPlan)
        {
            string sql = $@"
                UPDATE DBSATINALMAPLANIAYDAGILIM SET
                    SATINALMAID= {VeriErisim.DegerYap(gercPlan.satinAlmaID)},
                    PLANTARIHI = {VeriErisim.DegerYap(gercPlan.planTarihi)},
                    DURUM = 1,
                    ACIKLAMA = {VeriErisim.DegerYap(gercPlan.aciklama)}
                WHERE
                    DBSATINALMAPLANIAYDAGILIMID = {VeriErisim.DegerYap(gercPlan.dagilimID)}";
            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleSatinAlmaGerceklesmePlani(int dagilimID, int durumKod)
        {
            string sql = $@"
                UPDATE DBSATINALMAPLANIAYDAGILIM SET
                    DURUM = {VeriErisim.DegerYap(durumKod)}
                WHERE
                    DBSATINALMAPLANIAYDAGILIMID = {VeriErisim.DegerYap(dagilimID)}";
            return KomutCalistir(sql, 1);
        }


        public Sonuc SilSatinAlmaGerceklesmePlaniDetay(DBSatinAlmaGerceklesmePlan gercPlan)
        {
            string sql = $@"
                DELETE FROM DBSATINALMAPLANIAYDAGILIMDETAY
                WHERE DBSATINALMAPLANIAYDAGILIMID = {VeriErisim.DegerYap(gercPlan.dagilimID)}";

            return KomutCalistir(sql, 0);
        }

        public Sonuc EkleSatinAlmaGerceklesmePlaniDetay(DBOdemePlaniDetay detay)
        {
            string sql = $@"
		INSERT INTO DBSATINALMAPLANIAYDAGILIMDETAY (DBSATINALMAPLANIAYDAGILIMID, YIL, AY, ODENEK)
		VALUES (
			{VeriErisim.DegerYap(detay.odemePlanID)},
			{VeriErisim.DegerYap(detay.yil)},
			{VeriErisim.DegerYap(detay.ay)},
			{VeriErisim.DegerYap(detay.odenecekTutar)}
		)";

            return KomutCalistir(sql, 1);
        }

        public IDataReader ButceSozlesmeBul(int butceRefID)
        {
            string sql = $@"
                SELECT COUNT(*)
                FROM DBSOZLESME
                WHERE BUTCEREFID = {VeriErisim.DegerYap(butceRefID)}
            ";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ButceHakedisBul(int butceRefID)
        {
            string sql = $@"
                SELECT COUNT(*)
                FROM DBHAKEDIS
                WHERE BUTCEREFID = {VeriErisim.DegerYap(butceRefID)}
            ";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SilSatinAlmaPlani(int satinAlmaID)
        {
            string sql = $@"
                DELETE FROM DBSATINALMAPLANI
                WHERE SATINALMAID = {VeriErisim.DegerYap(satinAlmaID)}
            ";

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleEkstreler(TNSDateTime baslangicTarih, TNSDateTime bitisTarih, int ikrazID)
        {
            string sql = @"
                SELECT NUMARATOR, IKRAZID, EKSDATE, EKSDATESIRANO, CATNO, EXPLANATION, CUR,
                       AMOUNTPAID, EXCHRATE, TRYEQUIV, EUREQUIV,
                       DEPOSITEUR, DEPOSITTRY, SGBNUMARATOR,
                       (SELECT SUM(H.KDVHARICBELGETOPLAMI + H.KDVTUTARI - H.AVANSKESINTISI)
                        FROM DBHAKEDIS H
                        INNER JOIN DBBUTCEREF BR ON BR.BUTCEREFID = H.BUTCEREFID
                        INNER JOIN DBSATINALMAPLANI SP ON SP.SATINALMAID = BR.SATINALMAID
                        INNER JOIN DBALTBILESEN AB ON AB.ALTBILESENID = SP.ALTBILESENID
                        INNER JOIN DBBILESEN B ON B.BILESENID = AB.BILESENID
                        WHERE B.IKRAZID = DBEKSTRELER.IKRAZID AND DBEKSTRELER.SGBNUMARATOR > 0
                          AND EXISTS (SELECT 1 FROM DBHAKEDISSAHIBI HS
                                      WHERE HS.HAKEDISID = H.HAKEDISID AND HS.NUMARATOR = DBEKSTRELER.SGBNUMARATOR)) HAKEDISTUTARI
                FROM DBEKSTRELER
                WHERE 1 = 1
            ";

            // IKRAZ filtresi
            if (ikrazID > 0)
            {
                sql += " AND IKRAZID = " + ikrazID;
            }

            // Tarih filtresi
            if (!TNSDateTime.IsNull(baslangicTarih))
            {
                sql += " AND EKSDATE >= " + VeriErisim.DegerYap(baslangicTarih);
            }

            if (!TNSDateTime.IsNull(bitisTarih))
            {
                sql += " AND EKSDATE <= " + VeriErisim.DegerYap(bitisTarih);
            }

            sql += " ORDER BY EKSDATE, EKSDATESIRANO";


            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc EkleEkstre(DBEkstre form)
        {
            string sql = "INSERT INTO DBEKSTRELER (";
            sql += "NUMARATOR,";
            sql += "IKRAZID,";
            sql += "EKSDATE,";
            sql += "EKSDATESIRANO,";
            sql += "CATNO,";
            sql += "EXPLANATION,";
            sql += "CUR,";
            sql += "AMOUNTPAID,";
            sql += "EXCHRATE,";
            sql += "TRYEQUIV,";
            sql += "EUREQUIV,";
            sql += "DEPOSITEUR,";
            sql += "DEPOSITTRY";
            sql += ") VALUES (";

            sql += VeriErisim.DegerYap(form.Numarator);
            sql += "," + VeriErisim.DegerYap(form.IkrazID);
            sql += "," + VeriErisim.DegerYap(form.EksDate);
            sql += "," + VeriErisim.DegerYap(form.EksDateSiraNo);
            sql += "," + VeriErisim.DegerYap(form.CatNo);
            sql += "," + VeriErisim.DegerYap(form.Explanation ?? "");
            sql += "," + VeriErisim.DegerYap(form.Cur);
            sql += "," + VeriErisim.DegerYap(form.AmountPaid);
            sql += "," + VeriErisim.DegerYap(form.ExchRate);
            sql += "," + VeriErisim.DegerYap(form.TryEquiv);
            sql += "," + VeriErisim.DegerYap(form.EurEquiv);
            sql += "," + VeriErisim.DegerYap(form.DepositEur);
            sql += "," + VeriErisim.DegerYap(form.DepositTry);
            sql += ")";

            return KomutCalistir(sql, 1);
        }

        public double BakiyeBul(TNSDateTime tNSDateTime, int ikrazID)
        {
            string sql = $@" SELECT SUM(DEPOSITEUR) - SUM(EUREQUIV) FROM DBEKSTRELER WHERE EKSDATE < {VeriErisim.DegerYap(tNSDateTime)}";
            if (ikrazID != 0)
                sql += $" AND IKRAZID = {ikrazID}";

            return this.veriErisim.TekKolonSQLDbl(sql);
        }

        public Sonuc GuncelleSGBNumarator(DBEkstre item)
        {
            string sql = $@"UPDATE DBEKSTRELER SET SGBNUMARATOR = {VeriErisim.DegerYap(item.SGBNumarator)} WHERE
            NUMARATOR = {VeriErisim.DegerYap(item.Numarator)}
            AND IKRAZID = {VeriErisim.DegerYap(item.IkrazID)}
            AND EKSDATE = {VeriErisim.DegerYap(item.EksDate)};

            UPDATE HS SET HS.ODENDI = 1, HS.ODEMETARIHI = {VeriErisim.DegerYap(item.EksDate)}
            FROM DBHAKEDISSAHIBI HS INNER JOIN DBHAKEDIS H ON H.HAKEDISID = HS.HAKEDISID
            INNER JOIN DBBUTCEREF BR ON BR.BUTCEREFID = H.BUTCEREFID
            INNER JOIN DBSATINALMAPLANI SP ON SP.SATINALMAID = BR.SATINALMAID
            INNER JOIN DBALTBILESEN AB ON AB.ALTBILESENID = SP.ALTBILESENID
            INNER JOIN DBBILESEN B ON B.BILESENID = AB.BILESENID
            WHERE HS.NUMARATOR = {VeriErisim.DegerYap(item.SGBNumarator)} AND B.IKRAZID = {VeriErisim.DegerYap(item.IkrazID)} AND HS.ODEMETARIHI IS NULL;";

            return KomutCalistir(sql, 0);
        }
    }
}
