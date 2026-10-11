using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.MUH;
namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHBANKAODEMEANA veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMUHBANKAODEMEANA : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMUHBANKAODEMEANA(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /*
         *                                                  
CREATE TABLE MUHBANKAODEMEANA 
    ( SAYMANLIK INT
    , BIRIM VARCHAR(20)
    , GUN DATETIME
    , SIRANO INT
    , ACIKLAMA VARCHAR(200)
    , KULKOD VARCHAR(60)
    , OLUSTARIH DATETIME
    , DEGKOD VARCHAR(60)
    , DEGTARIH DATETIME
    , ONAYKOD VARCHAR(60)
    , ONAYTARIH DATETIME
    , HAZIRLAYAN VARCHAR(60)
    , ODEMETAR DATETIME
    , MUHYETKILI VARCHAR(60)
    , MUHYETUNV VARCHAR(60)
    , ONAYYETKILI VARCHAR(60)
    , ONAYYETUNV VARCHAR(60)
    , TUR INT
    , DURUM INT
    , CEKEMRINO VARCHAR(20)
    , BANKADOSYAAD VARCHAR(250)
    PRIMARY KEY (SAYMANLIK, BIRIM, GUN, SIRANO)
    )

          
         * 
        */

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "MUHBANKAODEMEANA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// GUN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYMANLIK = 0,

            /// <summary>
            /// GUN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIM = 1,

            /// <summary>
            /// GUN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GUN = 2,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 4,

            /// <summary>
            /// KULKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KULKOD = 5,

            /// <summary>
            /// OLUSTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            OLUSTARIH = 6,

            /// <summary>
            /// DEGKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGKOD = 7,

            /// <summary>
            /// DEGTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGTARIH = 8,

            /// <summary>
            /// ONAYKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYKOD = 9,

            /// <summary>
            /// ONAYTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYTARIH = 10,

            /// <summary>
            /// HAZIRLAYAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HAZIRLAYAN = 11,

            /// <summary>
            /// ODEMETAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMETAR = 12,

            /// <summary>
            /// MUHYETKILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHYETKILI = 13,

            /// <summary>
            /// MUHYETUNV alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHYETUNV = 14,

            /// <summary>
            /// ONAYYETKILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYYETKILI = 15,

            /// <summary>
            /// ONAYYETUNV alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYYETUNV = 16,

            /// <summary>
            /// TUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUR = 17,

            /// <summary>
            /// DURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 18,

            /// <summary>
            /// CEKEMRINO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CEKEMRINO = 19,

            /// <summary>
            /// BANKADOSYAAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BANKADOSYAAD = 20,

            /// <summary>
            /// DONEMNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DONEMNO = 21
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "SAYMANLIK"
            , "BIRIM"
            , "GUN"
            , "SIRANO"
            , "ACIKLAMA"
            , "KULKOD"
            , "OLUSTARIH"
            , "DEGKOD"
            , "DEGTARIH"
            , "ONAYKOD"
            , "ONAYTARIH"
            , "HAZIRLAYAN"
            , "ODEMETAR"
            , "MUHYETKILI"
            , "MUHYETUNV"
            , "ONAYYETKILI"
            , "ONAYYETUNV"
            , "TUR"
            , "DURUM"
            , "CEKEMRINO"
            , "BANKADOSYAAD"
            , "DONEMNO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, MUHBankaOdemeyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(MUHBankaOdeme form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", form.gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, MUHBankaOdemeyapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(MUHBankaOdeme form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.saymanlik);
            sql += "," + VeriErisim.DegerYap(form.birim);
            sql += "," + VeriErisim.DegerYap(form.gun);
            sql += "," + VeriErisim.DegerYap(form.siraNo);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += "," + VeriErisim.DegerYap(form.kullaniciKod);
            sql += "," + VeriErisim.DegerYap(form.olusmaZamani);
            sql += "," + VeriErisim.DegerYap(form.degistirenKod);
            sql += "," + VeriErisim.DegerYap(form.degismeZamani);
            sql += "," + VeriErisim.DegerYap(form.onaylayanKod);
            sql += "," + VeriErisim.DegerYap(form.onayZamani);
            sql += "," + VeriErisim.DegerYap(form.hazirlayan);
            sql += "," + VeriErisim.DegerYap(form.odemeTarih);
            sql += "," + VeriErisim.DegerYap(form.muhYetkili);
            sql += "," + VeriErisim.DegerYap(form.muhYetUnvan);
            sql += "," + VeriErisim.DegerYap(form.onaylayan);
            sql += "," + VeriErisim.DegerYap(form.onayUnvan);
            sql += "," + VeriErisim.DegerYap((int)form.tur);
            sql += "," + VeriErisim.DegerYap((int)form.durum);
            sql += "," + VeriErisim.DegerYap(form.cekEmriNo);
            sql += "," + VeriErisim.DegerYap(form.bankaDosyaAd);
            sql += "," + VeriErisim.DegerYap(form.donemNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Ekle(KYM.Kullanici kullanan, int saymanlik, string birim, TNSDateTime gun, int siraNo, string aciklama, string muhYetkili, string muhYetUnvan, string onaylayan, string onayUnvan)
        {
            // string sql = "INSERT INTO " + TABLOAD + "(" + ALANLAR[(int)ENUMAlanlar.GUN] + ", " + ALANLAR[(int)ENUMAlanlar.SIRANO] + ", " + ALANLAR[(int)ENUMAlanlar.ACIKLAMA] + ", " + ALANLAR[(int)ENUMAlanlar.KULKOD] + ", " + ALANLAR[(int)ENUMAlanlar.OLUSTARIH] + ", " + ALANLAR[(int)ENUMAlanlar.TUR] + ", " + ALANLAR[(int)ENUMAlanlar.DURUM] + ") VALUES";
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(saymanlik);
            sql += "," + VeriErisim.DegerYap(birim);
            sql += "," + VeriErisim.DegerYap(gun);
            sql += "," + VeriErisim.DegerYap(siraNo);
            sql += "," + VeriErisim.DegerYap(aciklama);
            sql += "," + VeriErisim.DegerYap(kullanan.kullaniciKodu);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += ", null";
            sql += ", null";
            sql += ", null";
            sql += ", null";
            sql += "," + VeriErisim.DegerYap(kullanan.ad + " " + kullanan.soyad);
            sql += "," + VeriErisim.DegerYap(gun);

            sql += "," + VeriErisim.DegerYap(muhYetkili);
            sql += "," + VeriErisim.DegerYap(muhYetUnvan);
            sql += "," + VeriErisim.DegerYap(onaylayan);
            sql += "," + VeriErisim.DegerYap(onayUnvan);

            sql += "," + VeriErisim.DegerYap((int)ENUMBankaListesiTur.OESecim);
            sql += "," + VeriErisim.DegerYap((int)ENUMBankaListesiDurum.Yeni);
            sql += ", null";
            sql += ", null";
            sql += ", null";
            sql += ")";


            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, MUHBankaOdemeyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(MUHBankaOdeme form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", form.gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public int SiraNoBul(int saymanlik, string birim, TNSDateTime d)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", d));
            return (int)veriErisim.TekKolonSQLDbl(sql);
        }

        public Sonuc Guncelle(KYM.Kullanici kullanan, MUHBankaOdeme form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], form.aciklama);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGKOD], kullanan.kullaniciKodu);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGTARIH], new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)ENUMBankaListesiDurum.Degismis);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAZIRLAYAN], form.hazirlayan);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMETAR], form.odemeTarih);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHYETKILI], form.muhYetkili);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHYETUNV], form.muhYetUnvan);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYYETKILI], form.onaylayan);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYYETUNV], form.onayUnvan);
            //sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CEKEMRINO], form.cekEmriNo);
            //sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BANKADOSYAAD], form.bankaDosyaAd);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", form.gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(KYM.Kullanici kullanan, MUHBankaOdeme form)
        {
            string sql = "DELETE FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", form.gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return KomutCalistir(sql, 1);
        }

        public Sonuc Degisti(KYM.Kullanici kullanan, MUHBankaOdeme form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGKOD], kullanan.kullaniciKodu);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGTARIH], new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)ENUMBankaListesiDurum.Degismis);


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", form.gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return KomutCalistir(sql, 1);
        }

        public Sonuc Onayla(KYM.Kullanici kullanan, int saymanlik, string birim, TNSDateTime gun, int siraNo, ENUMBankaListesiDurum onayKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYKOD], kullanan.kullaniciKodu);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYTARIH], new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)onayKod);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));
            return KomutCalistir(sql, 1);
        }

        public Sonuc BankayaGonderildi(KYM.Kullanici kullanan, MUHBankaOdeme mbO, string sonucDosyaAd)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BANKADOSYAAD], sonucDosyaAd);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", mbO.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", mbO.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", mbO.gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", mbO.siraNo));
            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(int saymanlikKod, string birim, TNSDateTime basla, TNSDateTime bitis, string araMetin)
        {
            string sql = "SELECT DISTINCT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABMUHBANKAODEMEDETAY.TABLOAD;
            sql += " ON  " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SAYMANLIK] + " = " + TABMUHBANKAODEMEDETAY.TABLOAD + "." + TABMUHBANKAODEMEDETAY.ALANLAR[(int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.SAYMANLIK];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIM] + " = " + TABMUHBANKAODEMEDETAY.TABLOAD + "." + TABMUHBANKAODEMEDETAY.ALANLAR[(int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.BIRIM];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GUN] + " = " + TABMUHBANKAODEMEDETAY.TABLOAD + "." + TABMUHBANKAODEMEDETAY.ALANLAR[(int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.GUN];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO] + " = " + TABMUHBANKAODEMEDETAY.TABLOAD + "." + TABMUHBANKAODEMEDETAY.ALANLAR[(int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.SIRANO];
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", saymanlikKod));
            if (birim != "000")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birim));

            sql = VeriErisim.KosulEkle(sql, "AND", "((" + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GUN], ">=", basla));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GUN], "<=", bitis));

            sql = VeriErisim.KosulEkle(sql, ") OR", "(" + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMETAR], ">=", basla));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMETAR], "<=", bitis) + "))");

            if (!string.IsNullOrWhiteSpace(araMetin))
            {
                string icKosul = "(" + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + araMetin + "%");
                icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABMUHBANKAODEMEDETAY.TABLOAD, TABMUHBANKAODEMEDETAY.ALANLAR[(int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.MIFBELGENO], "LIKE", "%" + araMetin + "%"));
                icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABMUHBANKAODEMEDETAY.TABLOAD, TABMUHBANKAODEMEDETAY.ALANLAR[(int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.ILGILI], "LIKE", "%" + araMetin + "%"));
                icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABMUHBANKAODEMEDETAY.TABLOAD, TABMUHBANKAODEMEDETAY.ALANLAR[(int)TABMUHBANKAODEMEDETAY.ENUMAlanlar.SOZLESMENO], "LIKE", "%" + araMetin + "%")) + ")";
                sql = VeriErisim.KosulEkle(sql, "AND", icKosul);
            }
            sql += " order by " + ALANLAR[(int)ENUMAlanlar.ODEMETAR];
            return veriErisim.DataReaderDondur(sql);
        }

        //public int CekEmriNoSayiBul(TNSDateTime odemeTarih, int saymanlikKod, string cekNo)
        //{
        //    string sql = "SELECT COUNT(*) FROM " + TABLOAD;

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", saymanlikKod));
        //    sql = VeriErisim.KosulEkle(sql, "AND", "YEAR(" + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMETAR] + ")", "=", odemeTarih.Yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CEKEMRINO], "=", cekNo));

        //    return (int)this.veriErisim.TekKolonSQLDbl(sql);
        //}

        public int CekEmriNoSayiBul(int yil, int saymanlik, string birim, string cekNo)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", "YEAR(" + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMETAR] + ")", "=", yil));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birim));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CEKEMRINO], "=", cekNo));

            return (int)this.veriErisim.TekKolonSQLDbl(sql);
        }

        public Sonuc CekNoGuncelle(MUHBankaOdeme bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CEKEMRINO], bilgi.cekEmriNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", bilgi.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", bilgi.gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.siraNo));
            return KomutCalistir(sql, 1);
        }

        public Sonuc OdemeEmriCekNoGuncelle(MUHBankaOdeme bilgi, int yil)
        {
            string sql = "UPDATE " + TABOdemeEmriMIF.TABLOADMIFF + " SET ";
            sql += VeriErisim.AtamaOl("", TABOdemeEmriMIF.ALANLARMIFF[(int)TABOdemeEmriMIF.ENUMAlanlarMIFF.CEKEMRINO], bilgi.cekEmriNo);

            string icSql = "SELECT MUHMIFFORM.BELGENO FROM " + TABOdemeEmriMIF.TABLOADMIFF + ", " + TABMUHBANKAODEMEDETAY.TABLOAD + " D WHERE MUHMIFFORM.YIL=D.MIFYIL AND MUHMIFFORM.BELGENO=D.MIFBELGENO";

            icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl("D", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", bilgi.saymanlik));
            icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl("D", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", bilgi.birim));
            icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl("D", ALANLAR[(int)ENUMAlanlar.GUN], "=", bilgi.gun));
            icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl("D", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.siraNo));
            sql += " WHERE MUHMIFFORM.BELGENO IN (" + icSql + ") AND YIL=" + VeriErisim.DegerYap(yil);
            return KomutCalistir(sql, 0);
        }

        public string MaxCekEmriNoBul(TNSDateTime odemeTarih, int saymanlikKod)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.CEKEMRINO] + ") FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", saymanlikKod));
            sql = VeriErisim.KosulEkle(sql, "AND", "YEAR(" + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMETAR] + ")", "=", odemeTarih.Yil));

            return this.veriErisim.TekKolonSQLStr(sql);
        }

        public string MaksBankaDosyaAdBul(string onAd)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.BANKADOSYAAD] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BANKADOSYAAD], "LIKE", onAd + "%"));
            return this.veriErisim.TekKolonSQLStr(sql);
        }

        public Sonuc DonemNoGuncelle(int saymanlik, string birim, TNSDateTime gun, int siraNo, int donemNo)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], donemNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));
            return KomutCalistir(sql, 1);
        }

    }
}
