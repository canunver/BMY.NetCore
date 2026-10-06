using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.ORG;
namespace TNS.ORG.IsKurallari.Veri
{
    /// <summary>
    /// GENELGOREV veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABGENELGOREV : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGENELGOREV(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "GRVGENELGOREV";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// ORGKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORGKOD = 0,

            /// <summary>
            /// GOREVSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVSIRANO = 1,

            /// <summary>
            /// AD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 2,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 3,

            MAKSGOREV = 4,
            ZORUNLU = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "ORGKOD"
            , "GOREVSIRANO"
            , "AD"
            , "ACIKLAMA"
            , "MAKSGOREV"
            , "ZORUNLU"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADBIRIMTUR = "GRVGENELGOREVBIRIMTUR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarBIRIMTUR : int
        {
            /// <summary>
            /// ORGKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORGKOD = 0,

            /// <summary>
            /// GOREVSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVSIRANO = 1,

            /// <summary>
            /// BIRIMTIP alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMTIP = 2,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBIRIMTUR = {
            "ORGKOD"
            , "GOREVSIRANO"
            , "BIRIMTIP"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADISLEMLER = "GRVGENELGOREVISLEMLER";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarISLEMLER : int
        {

            /// <summary>
            /// ORGKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORGKOD = 0,

            /// <summary>
            /// GOREVSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVSIRANO = 1,

            /// <summary>
            /// YAZSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAZSIRANO = 2,

            /// <summary>
            /// ISLEMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMKOD = 3,

            /// <summary>
            /// ISLEMAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMAD = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARISLEMLER = {
            "ORGKOD"
            , "GOREVSIRANO"
            , "YAZSIRANO"
            , "ISLEMKOD"
            , "ISLEMAD"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADGENELGOREVYETKI = "GRVGENELGOREVISLEMYETKILER";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarGENELGOREVYETKI : int
        {

            /// <summary>
            /// ORGKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORGKOD = 0,

            /// <summary>
            /// GOREVSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVSIRANO = 1,

            /// <summary>
            /// YAZSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAZSIRANO = 2,

            /// <summary>
            /// DAMGAAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DAMGAAD = 3,

            /// <summary>
            /// DAMGADEGER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DAMGADEGER = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARGENELGOREVYETKI = {
            "ORGKOD"
            , "GOREVSIRANO"
            , "YAZSIRANO"
            , "DAMGAAD"
            , "DAMGADEGER"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADGENELGOREVPERSONEL = "GRVGENELGOREVPERSONEL";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarGENELGOREVPERSONEL : int
        {

            /// <summary>
            /// ORGKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORGKOD = 0,

            /// <summary>
            /// ORGKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVORGKOD = 1,

            /// <summary>
            /// GOREVSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVSIRANO = 2,

            /// <summary>
            /// YAZSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARGENELGOREVPERSONEL = {
            "ORGKOD"
            , "GOREVORGKOD"
            , "GOREVSIRANO"
            , "TCKIMLIKNO"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADGENELGOREVPERSONELKISIT = "GRVGENELGOREVPERSONELKISIT";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarGENELGOREVPERSONELKISIT : int
        {
            /// <summary>
            /// ORGKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORGKOD = 0,

            /// <summary>
            /// ORGKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVORGKOD = 1,

            /// <summary>
            /// GOREVSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVSIRANO = 2,

            /// <summary>
            /// YAZSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 3,

            /// <summary>
            /// KISITNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KISITNO = 4,

            /// <summary>
            /// KISITDEGER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KISITDEGER = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARGENELGOREVPERSONELKISIT = {
            "ORGKOD"
            , "GOREVORGKOD"
            , "GOREVSIRANO"
            , "TCKIMLIKNO"
            , "KISITNO"
            , "KISITDEGER"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BirimGenelGorevyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(BirimGenelGorev form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GOREVSIRANO], "=", form.siraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, BirimGenelGorevyapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(BirimGenelGorev form)
        {
            int orgKod;
            if (form.orgBirim == null) orgKod = 0;
            else orgKod = form.orgBirim.kod;

            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(orgKod);
            sql += "," + VeriErisim.DegerYap(form.siraNo);
            sql += "," + VeriErisim.DegerYap(form.ad);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += "," + VeriErisim.DegerYap(form.maxGorev);
            sql += "," + VeriErisim.DegerYap(form.zorunlu ? 1 : 0);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="siraNo">Select edilecek kaydın anahtarı.</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(int orgKod, int siraNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GOREVSIRANO], "=", siraNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Guncelle(BirimGenelGorev birimGenelGorev)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], birimGenelGorev.ad);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], birimGenelGorev.aciklama);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MAKSGOREV], birimGenelGorev.maxGorev);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZORUNLU], birimGenelGorev.zorunlu ? 1 : 0);
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", birimGenelGorev.OrgKod);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GOREVSIRANO], "=", birimGenelGorev.siraNo);
            return this.KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanındaki tüm kayıtları döndüren,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int orgKod, bool aktifler, int gorevSiraNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", orgKod);
            if (aktifler)
            {
                sql += " AND (1 IN (SELECT 1 FROM GRVGENELGOREVISLEMLER WHERE GRVGENELGOREVISLEMLER.ORGKOD=GRVGENELGOREV.ORGKOD  AND GRVGENELGOREVISLEMLER.GOREVSIRANO=GRVGENELGOREV.GOREVSIRANO ))";
            }
            if (gorevSiraNo > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOREVSIRANO], "=", gorevSiraNo);

            sql += " ORDER BY " + TABLOAD + ".AD";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// ??? birim tiplere göre sql yaz.
        /// </summary>
        /// <param name="birimTipler"></param>
        /// <returns></returns>
        public IDataReader GenelListele(BirimTip[] birimTipler, int gorevSiraNo)
        {
            string ekKosul = "";
            for (int i = 0; i < birimTipler.Length; i++)
            {
                if (ekKosul != "") ekKosul += ",";
                ekKosul += ((int)birimTipler[i]).ToString();
            }
            ekKosul = TABLOADBIRIMTUR + "." + ALANLARBIRIMTUR[(int)ENUMAlanlarBIRIMTUR.BIRIMTIP] + " IN (" + ekKosul + ")";

            string sql = "SELECT DISTINCT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql += " INNER JOIN " + TABLOADBIRIMTUR + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ORGKOD] + " = " + TABLOADBIRIMTUR + "." + ALANLARBIRIMTUR[(int)ENUMAlanlarBIRIMTUR.ORGKOD] + " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GOREVSIRANO] + " = " + TABLOADBIRIMTUR + "." + ALANLARBIRIMTUR[(int)ENUMAlanlarBIRIMTUR.GOREVSIRANO];

            sql += " WHERE " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", 0);
            sql += " AND " + ekKosul;
            if (gorevSiraNo > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOREVSIRANO], "=", gorevSiraNo);
            sql += " ORDER BY 1,2";

            return veriErisim.DataReaderDondur(sql);
        }

        public int SonSayiBul(int orgKod)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.GOREVSIRANO] + ") FROM " + TABLOAD;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", orgKod);
            return (int)veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="siraNo">Silinecek birim türü ilişkisinin sahibi olan genel görevin anahtarı</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilBirimTur(int orgKod, int siraNo)
        {
            string sql = "DELETE FROM " + TABLOADBIRIMTUR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBIRIMTUR[(int)ENUMAlanlarBIRIMTUR.ORGKOD], "=", orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBIRIMTUR[(int)ENUMAlanlarBIRIMTUR.GOREVSIRANO], "=", siraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, iliskiliBirimler dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, BirimGenelGorevyapısında nesne</param>
        /// <param name="satirNo">iliskiliBirimler dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc BirimTurIliskiEkle(int orgKod, int siraNo, TNS.ORG.BirimTip birimTip)
        {
            string sql = "INSERT INTO " + TABLOADBIRIMTUR + "(" + VeriErisim.TumAlanlar("", ALANLARBIRIMTUR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(orgKod);
            sql += "," + VeriErisim.DegerYap(siraNo);
            sql += "," + VeriErisim.DegerYap((int)birimTip);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="siraNo">Kendisine ait yazılım işlemleri listelenecek, genel görevin sıra numarası</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader BirimTurIliskiListele(int orgKod, int siraNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBIRIMTUR, ALANLARBIRIMTUR) + " FROM " + TABLOADBIRIMTUR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBIRIMTUR[(int)ENUMAlanlarBIRIMTUR.ORGKOD], "=", orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBIRIMTUR[(int)ENUMAlanlarBIRIMTUR.GOREVSIRANO], "=", siraNo));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="gorevSiraNo">Silinecek yazılım yetkisinin görev sıra numarası</param>
        /// <param name="yazilimSiraNo">Silinecek yazılım yetkisinin sıra numarası</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilIslem(int orgKod, int gorevSiraNo, int yazilimSiraNo)
        {
            string sql = "DELETE FROM " + TABLOADISLEMLER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.ORGKOD], "=", orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.GOREVSIRANO], "=", gorevSiraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.YAZSIRANO], "=", yazilimSiraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, yazilimYetkiler dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="genelGorevYetki">Eklenecek kayıt bilgisini tutan, GenelGorevYetki yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc YazilimIslemEkle(GenelGorevYetki genelGorevYetki)
        {
            string sql = "INSERT INTO " + TABLOADISLEMLER + "(" + VeriErisim.TumAlanlar("", ALANLARISLEMLER) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(genelGorevYetki.birimGenelGorev.OrgKod);
            sql += "," + VeriErisim.DegerYap(genelGorevYetki.birimGenelGorev.siraNo);
            sql += "," + VeriErisim.DegerYap(genelGorevYetki.yazYetkiSiraNo);
            sql += "," + VeriErisim.DegerYap(genelGorevYetki.yazYetkiIndis);
            sql += "," + VeriErisim.DegerYap(genelGorevYetki.yetkiAd);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran, yazilimYetkiler dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="genelGorevYetki">Eklenecek kayıt bilgisini tutan, GenelGorevYetki yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc YazilimIslemGuncelle(GenelGorevYetki genelGorevYetki)
        {
            string sql = "UPDATE " + TABLOADISLEMLER;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.ISLEMKOD], genelGorevYetki.yazYetkiIndis);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.ISLEMAD], genelGorevYetki.yetkiAd);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.ORGKOD], "=", genelGorevYetki.birimGenelGorev.OrgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.GOREVSIRANO], "=", genelGorevYetki.birimGenelGorev.siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.YAZSIRANO], "=", genelGorevYetki.yazYetkiSiraNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="siraNo">Kendisine ait yazılım işlemleri listelenecek, genel görevin sıra numarası.</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public IDataReader ListeleIslemler(int orgKod, int siraNo, int yazSiraNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADISLEMLER, ALANLARISLEMLER) + " FROM " + TABLOADISLEMLER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.ORGKOD], "=", orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.GOREVSIRANO], "=", siraNo));
            if (yazSiraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.YAZSIRANO], "=", yazSiraNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public int GorevYazilimSiraNoBul(int orgKod, int gorevSiraNo)
        {
            string sql = "SELECT MAX(" + ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.YAZSIRANO] + ") FROM " + TABLOADISLEMLER;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarISLEMLER.ORGKOD], "=", orgKod);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarISLEMLER.GOREVSIRANO], "=", gorevSiraNo);
            return (int)veriErisim.TekKolonSQLDbl(sql);
        }


        public Sonuc YazilimGorevYetkiSil(GenelGorevYetki genelGorevYetki)
        {
            string sql = "DELETE FROM " + TABLOADISLEMLER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.ORGKOD], "=", genelGorevYetki.birimGenelGorev.OrgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.GOREVSIRANO], "=", genelGorevYetki.birimGenelGorev.siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARISLEMLER[(int)ENUMAlanlarISLEMLER.YAZSIRANO], "=", genelGorevYetki.yazYetkiSiraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BirimGenelGorevyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc DamgalariSil(GenelGorevYetki genelGorevYetki)
        {
            string sql = "DELETE FROM " + TABLOADGENELGOREVYETKI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGENELGOREVYETKI[(int)ENUMAlanlarGENELGOREVYETKI.ORGKOD], "=", genelGorevYetki.birimGenelGorev.OrgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGENELGOREVYETKI[(int)ENUMAlanlarGENELGOREVYETKI.GOREVSIRANO], "=", genelGorevYetki.birimGenelGorev.siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGENELGOREVYETKI[(int)ENUMAlanlarGENELGOREVYETKI.YAZSIRANO], "=", genelGorevYetki.yazYetkiSiraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, yazilimYetkiDegerler dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, BirimGenelGorevyapısında nesne</param>
        /// <param name="satirNo">yazilimYetkiDegerler dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc DamgaEkle(GenelGorevYetki genelGorevYetki, string damgaAd, string damgaDeger)
        {
            string sql = "INSERT INTO " + TABLOADGENELGOREVYETKI + "(" + VeriErisim.TumAlanlar("", ALANLARGENELGOREVYETKI) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(genelGorevYetki.birimGenelGorev.OrgKod);
            sql += "," + VeriErisim.DegerYap(genelGorevYetki.birimGenelGorev.siraNo);
            sql += "," + VeriErisim.DegerYap(genelGorevYetki.yazYetkiSiraNo);
            sql += "," + VeriErisim.DegerYap(damgaAd);
            sql += "," + VeriErisim.DegerYap(damgaDeger);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, BirimGenelGorevyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader DamgalariListele(BirimGenelGorev birimGenelGorev, int yazYetkiSiraNo)
        {
            string sql = "SELECT " + ALANLARGENELGOREVYETKI[(int)ENUMAlanlarGENELGOREVYETKI.DAMGAAD] + ", " + ALANLARGENELGOREVYETKI[(int)ENUMAlanlarGENELGOREVYETKI.DAMGADEGER] + " FROM " + TABLOADGENELGOREVYETKI;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGENELGOREVYETKI[(int)ENUMAlanlarGENELGOREVYETKI.ORGKOD], "=", birimGenelGorev.OrgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGENELGOREVYETKI[(int)ENUMAlanlarGENELGOREVYETKI.GOREVSIRANO], "=", birimGenelGorev.siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGENELGOREVYETKI[(int)ENUMAlanlarGENELGOREVYETKI.YAZSIRANO], "=", yazYetkiSiraNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public int BirimKisiSayisiBul(OrganizasyonBirim organizasyonBirim, BirimGenelGorev bilgi)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOADGENELGOREVPERSONEL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.ORGKOD], "=", organizasyonBirim.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.GOREVORGKOD], "=", bilgi.OrgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.GOREVSIRANO], "=", bilgi.siraNo));
            return (int)this.veriErisim.TekKolonSQLDbl(sql);
        }

        public Sonuc GoreveKisiEkle(OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, Personel personel)
        {
            string sql = "INSERT INTO " + TABLOADGENELGOREVPERSONEL + "(" + VeriErisim.TumAlanlar("", ALANLARGENELGOREVPERSONEL) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(organizasyonBirim.kod);
            sql += "," + VeriErisim.DegerYap(birimGenelGorev.OrgKod);
            sql += "," + VeriErisim.DegerYap(birimGenelGorev.siraNo);
            sql += "," + VeriErisim.DegerYap(personel.tcKimlikNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader GorevKisiListele(OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev)
        {
            string sql = "SELECT PERPERSONEL.TCKIMLIKNO, PERPERSONEL.AD, PERPERSONEL.SOYAD FROM ";
            sql += "GRVGENELGOREVPERSONEL INNER JOIN PERPERSONEL ON GRVGENELGOREVPERSONEL.TCKIMLIKNO = PERPERSONEL.TCKIMLIKNO";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.ORGKOD], "=", organizasyonBirim.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.GOREVORGKOD], "=", birimGenelGorev.OrgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.GOREVSIRANO], "=", birimGenelGorev.siraNo));
            sql += " ORDER BY 2,3";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader GorevKisiListeleKullanici(OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev)
        {
            string sql = "SELECT KULLANICI.MERNIS, KULLANICI.AD, KULLANICI.SOYAD FROM ";
            sql += "GRVGENELGOREVPERSONEL INNER JOIN KULLANICI ON GRVGENELGOREVPERSONEL.TCKIMLIKNO = KULLANICI.MERNIS";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.ORGKOD], "=", organizasyonBirim.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.GOREVORGKOD], "=", birimGenelGorev.OrgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.GOREVSIRANO], "=", birimGenelGorev.siraNo));
            sql += " ORDER BY 2,3";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KisiGorevListele(KisiGorev kg)
        {
            string sql = "";
            sql += "  SELECT " + TABLOADGENELGOREVPERSONEL + "." + ENUMAlanlarGENELGOREVPERSONEL.TCKIMLIKNO + " ," + TABLOADGENELGOREVPERSONEL + "." + ENUMAlanlarGENELGOREVPERSONEL.ORGKOD + " AS BIRIMKOD, " + TABLOADGENELGOREVPERSONEL + "." + ENUMAlanlarGENELGOREVPERSONEL.GOREVORGKOD + "," + TABLOADGENELGOREVPERSONEL + " ." + ENUMAlanlarGENELGOREVPERSONEL.GOREVSIRANO + ",  ";
            sql += TABORGBirim.TABLOAD + "." + TABORGBirim.ENUMAlanlar.ADI + "   BIRIMADI," + TABLOAD + "." + ENUMAlanlar.AD + "   GOREVADI ";
            sql += " FROM         " + TABLOADGENELGOREVPERSONEL + "   INNER JOIN ";
            sql += "              " + TABLOAD + " ON " + TABLOADGENELGOREVPERSONEL + "." + ENUMAlanlarGENELGOREVPERSONEL.GOREVORGKOD + " =" + TABLOAD + "." + ENUMAlanlar.ORGKOD + " AND " + TABLOADGENELGOREVPERSONEL + "." + ENUMAlanlarGENELGOREVPERSONEL.GOREVSIRANO + "= " + TABLOAD + "." + ENUMAlanlar.GOREVSIRANO + " INNER JOIN ";
            sql += TABORGBirim.TABLOAD + " ON " + TABLOADGENELGOREVPERSONEL + "." + ENUMAlanlarGENELGOREVPERSONEL.ORGKOD + " =" + TABORGBirim.TABLOAD + "." + TABORGBirim.ENUMAlanlar.KOD;
            sql += " WHERE   1=1 ";
            if (kg.orgKod > 0)
                sql = sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.ORGKOD], "=", kg.orgKod));
            if (kg.gorevOrgKod > 0)
                sql = sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", kg.gorevOrgKod));
            if (kg.gorevOrgKod > 0)
                sql = sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOREVSIRANO], "=", kg.gorevSiraNo));
            if (kg.tcKimlikNo.Trim() != string.Empty)
                sql = sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.TCKIMLIKNO], "=", kg.tcKimlikNo.Trim()));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KisidenGorevSil(KisiGorev bilgi)
        {
            string sql = "DELETE FROM " + TABLOADGENELGOREVPERSONEL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.ORGKOD], "=", bilgi.orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.GOREVORGKOD], "=", bilgi.gorevOrgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.GOREVSIRANO], "=", bilgi.gorevSiraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.TCKIMLIKNO], "=", bilgi.tcKimlikNo));
            return this.KomutCalistir(sql, 1);
        }

        public Sonuc GorevdenKisiSil(OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, Personel personel)
        {
            string sql = "DELETE FROM " + TABLOADGENELGOREVPERSONEL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.ORGKOD], "=", organizasyonBirim.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.GOREVORGKOD], "=", birimGenelGorev.OrgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.GOREVSIRANO], "=", birimGenelGorev.siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.TCKIMLIKNO], "=", personel.tcKimlikNo));
            return this.KomutCalistir(sql, 1);
        }

        public static string AlanAdOl(string tabloAd, string[] alanlarDizi, int siraNo, string alanLakap, string sonEk)
        {
            string donusDeger = "";
            if (!string.IsNullOrEmpty(tabloAd)) donusDeger = tabloAd + ".";
            donusDeger = donusDeger + alanlarDizi[siraNo];
            if (!string.IsNullOrEmpty(alanLakap)) donusDeger = donusDeger + " as " + alanLakap;
            if (!string.IsNullOrEmpty(sonEk)) donusDeger = donusDeger + sonEk;
            return donusDeger;
        }

        public IDataReader GorevDamgaListele(int orgKod, int gorevSiraNo, int yazSiraNo)
        {
            string sql = "SELECT " + AlanAdOl(TABLOADGENELGOREVYETKI, ALANLARGENELGOREVYETKI, (int)ENUMAlanlarGENELGOREVYETKI.DAMGAAD, null, ", ");
            sql += AlanAdOl(TABLOADGENELGOREVYETKI, ALANLARGENELGOREVYETKI, (int)ENUMAlanlarGENELGOREVYETKI.DAMGADEGER, null, " FROM ");
            sql += TABLOADGENELGOREVYETKI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADGENELGOREVYETKI, ALANLARGENELGOREVYETKI[(int)ENUMAlanlarGENELGOREVYETKI.ORGKOD], "=", orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVYETKI, ALANLARGENELGOREVYETKI[(int)ENUMAlanlarGENELGOREVYETKI.GOREVSIRANO], "=", gorevSiraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADGENELGOREVYETKI, ALANLARGENELGOREVYETKI[(int)ENUMAlanlarGENELGOREVYETKI.YAZSIRANO], "=", yazSiraNo));
            sql += " ORDER BY 1,2";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader PersonelYetkiListeleDamgasiz(string tcKimlikNo)
        {
            string sql = "SELECT " + AlanAdOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL, (int)ENUMAlanlarGENELGOREVPERSONEL.ORGKOD, null, ", ");
            sql += AlanAdOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL, (int)ENUMAlanlarGENELGOREVPERSONEL.GOREVORGKOD, "GOREVSAHIP", ", ");
            sql += AlanAdOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL, (int)ENUMAlanlarGENELGOREVPERSONEL.GOREVSIRANO, null, ", ");
            sql += AlanAdOl(TABLOADISLEMLER, ALANLARISLEMLER, (int)ENUMAlanlarISLEMLER.YAZSIRANO, null, ", ");
            sql += AlanAdOl(TABLOADISLEMLER, ALANLARISLEMLER, (int)ENUMAlanlarISLEMLER.ISLEMKOD, null, " FROM ");
            sql += TABLOADGENELGOREVPERSONEL + " INNER JOIN " + TABLOADISLEMLER;
            sql += " ON " + AlanAdOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL, (int)ENUMAlanlarGENELGOREVPERSONEL.GOREVORGKOD, null, "=") + AlanAdOl(TABLOADISLEMLER, ALANLARISLEMLER, (int)ENUMAlanlarISLEMLER.ORGKOD, null, null);
            sql += " AND " + AlanAdOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL, (int)ENUMAlanlarGENELGOREVPERSONEL.GOREVSIRANO, null, "=") + AlanAdOl(TABLOADISLEMLER, ALANLARISLEMLER, (int)ENUMAlanlarISLEMLER.GOREVSIRANO, null, null);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADGENELGOREVPERSONEL, ALANLARGENELGOREVPERSONEL[(int)ENUMAlanlarGENELGOREVPERSONEL.TCKIMLIKNO], "=", tcKimlikNo));
            sql += " ORDER BY 1,2,3,4";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kişiyi gösteren anahtar alanları kullanarak, ilgili kayıtları veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Tablonun kişiyi gösteren anahtar alanları</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilKisit(int ORGKOD, int GOREVORGKOD, int GOREVSIRANO, string TCKIMLIKNO)
        {
            string sql = "DELETE FROM " + TABLOADGENELGOREVPERSONELKISIT;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGENELGOREVPERSONELKISIT[(int)ENUMAlanlarGENELGOREVPERSONELKISIT.ORGKOD], "=", ORGKOD));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGENELGOREVPERSONELKISIT[(int)ENUMAlanlarGENELGOREVPERSONELKISIT.GOREVORGKOD], "=", GOREVORGKOD));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGENELGOREVPERSONELKISIT[(int)ENUMAlanlarGENELGOREVPERSONELKISIT.GOREVSIRANO], "=", GOREVSIRANO));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGENELGOREVPERSONELKISIT[(int)ENUMAlanlarGENELGOREVPERSONELKISIT.TCKIMLIKNO], "=", TCKIMLIKNO));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Bir kişinin bir kısıt kaydını veri tabanına ekleyen fonksiyon
        /// </summary>
        /// <param name="form">Tablonun bir kısıt kaydını gösteren anahtar alanları</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleKisit(int ORGKOD, int GOREVORGKOD, int GOREVSIRANO, string TCKIMLIKNO, int KISITNO, string KISITDEGER)
        {
            string sql = "INSERT INTO " + TABLOADGENELGOREVPERSONELKISIT + "(" + VeriErisim.TumAlanlar("", ALANLARGENELGOREVPERSONELKISIT) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(ORGKOD);
            sql += "," + VeriErisim.DegerYap(GOREVORGKOD);
            sql += "," + VeriErisim.DegerYap(GOREVSIRANO);
            sql += "," + VeriErisim.DegerYap(TCKIMLIKNO);
            sql += "," + VeriErisim.DegerYap(KISITNO);
            sql += "," + VeriErisim.DegerYap(KISITDEGER);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Bir kişiye ait anahtar alanları kullanarak, ilgili kayıtları veri tabanından okuyan
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Tablonun kişiyi gösteren anahtar alanları</param>
        /// <returns>Sil komutunun çalıştırılması sonucu DataReader yapısında döndürür</returns>
        public IDataReader ListeleKisit(int ORGKOD, int GOREVORGKOD, int GOREVSIRANO, string TCKIMLIKNO)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADGENELGOREVPERSONELKISIT, ALANLARGENELGOREVPERSONELKISIT) + " FROM " + TABLOADGENELGOREVPERSONELKISIT;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGENELGOREVPERSONELKISIT[(int)ENUMAlanlarGENELGOREVPERSONELKISIT.ORGKOD], "=", ORGKOD));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGENELGOREVPERSONELKISIT[(int)ENUMAlanlarGENELGOREVPERSONELKISIT.GOREVORGKOD], "=", GOREVORGKOD));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGENELGOREVPERSONELKISIT[(int)ENUMAlanlarGENELGOREVPERSONELKISIT.GOREVSIRANO], "=", GOREVSIRANO));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGENELGOREVPERSONELKISIT[(int)ENUMAlanlarGENELGOREVPERSONELKISIT.TCKIMLIKNO], "=", TCKIMLIKNO));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KisiGorevleriSil(string tcKimlikNo)
        {
            string sql = "DELETE FROM " + TABLOADGENELGOREVPERSONEL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGENELGOREVPERSONELKISIT[(int)ENUMAlanlarGENELGOREVPERSONELKISIT.TCKIMLIKNO], "=", tcKimlikNo));
            return KomutCalistir(sql, 0);
        }

        public Sonuc KisiTipleriSil(string tcKimlikNo)
        {
            string sql = "DELETE FROM KULLANICININTIPLERI WHERE KULLANICIKODU IN (SELECT KULLANICIKODU FROM KULLANICI WHERE MERNIS=" + VeriErisim.DegerYap(tcKimlikNo) + ")";
            return KomutCalistir(sql, 0);
        }

        public Sonuc KisiYetkiDegerleriSil(string tcKimlikNo)
        {
            string sql = "DELETE FROM KULLANICI_YT WHERE KULLANICIKODU IN (SELECT KULLANICIKODU FROM KULLANICI WHERE MERNIS=" + VeriErisim.DegerYap(tcKimlikNo) + ")";
            return KomutCalistir(sql, 0);
        }
    }
}
