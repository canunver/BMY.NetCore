using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.MUH;
namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHGONDERMETENKISICMAL veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMUHGONDERMETENKISICMAL : TABOrtak
    {
        /// <summary>
        /// TABMUHGONDERMETENKISICMAL sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMUHGONDERMETENKISICMAL(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /*
         * 
CREATE TABLE MUHGONDERMETENKISICMAL 
    (YIL INT
    , BELGETUR INT
    , ICMALNO INT
    , KURKOD VARCHAR(20)
    , FORMDURUM INT
    PRIMARY KEY (YIL, BELGETUR, ICMALNO)
    )

CREATE TABLE MUHGONDERMETENKISICMALKULLANICI
    (YIL INT
    , BELGETUR INT
    , ICMALNO INT
    , KULLANICIKODU VARCHAR(20)
    PRIMARY KEY (YIL, BELGETUR , ICMALNO, KULLANICIKODU)
    )
         * 
         * */


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "MUHGONDERMETENKISICMAL";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADKUL = "MUHGONDERMETENKISICMALKULLANICI";

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
            /// ICMALNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ICMALNO = 1,

            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,

            /// <summary>
            /// FORMDURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORMDURUM = 3,
            BELGETUR = 4,
            KAYITTARIH = 5,
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarKUL : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// ICMALNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ICMALNO = 1,

            /// <summary>
            /// KULLANICIKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 2,
            BELGETUR = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "ICMALNO"
            , "KURKOD"
            , "FORMDURUM"
            , "BELGETUR"
            , "KAYITTARIH"  
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARKUL = {
            "YIL"
            , "ICMALNO"
            , "KULLANICIKODU"
            , "BELGETUR"
        };
        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, GondermeTenkisIcmal yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(GondermeTenkisIcmal form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)form.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ICMALNO], "=", form.icmalNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, GondermeTenkisIcmal yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilKul(GondermeTenkisIcmal form)
        {
            string sql = "DELETE FROM " + TABLOADKUL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKUL[(int)ENUMAlanlarKUL.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKUL[(int)ENUMAlanlarKUL.BELGETUR], "=", (int)form.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKUL[(int)ENUMAlanlarKUL.ICMALNO], "=", form.icmalNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, GondermeTenkisIcmal yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(GondermeTenkisIcmal form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.icmalNo);
            sql += "," + VeriErisim.DegerYap(form.kurKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.formDurum);
            sql += "," + VeriErisim.DegerYap((int)form.belgeTur);
            sql += "," + VeriErisim.DegerYap(form.kayitTarih);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran, odenekGondermeIcmalKullanici dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, GondermeTenkisIcmal yapısında nesne</param>
        /// <param name="satirNo">odenekGondermeIcmalKullanici dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleKul(GondermeTenkisIcmal form, string kulKod)
        {
            string sql = "INSERT INTO " + TABLOADKUL + "(" + VeriErisim.TumAlanlar("", ALANLARKUL) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.icmalNo);
            sql += "," + VeriErisim.DegerYap(kulKod);
            sql += "," + VeriErisim.DegerYap((int)form.belgeTur);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, GondermeTenkisIcmal yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(GondermeTenkisIcmal form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", MUHGONDERMETENKISICMALKULLANICI.KULLANICIKODU FROM " + TABLOAD + " LEFT OUTER JOIN " + TABLOADKUL + " ON MUHGONDERMETENKISICMALKULLANICI.YIL=MUHGONDERMETENKISICMAL.YIL AND MUHGONDERMETENKISICMALKULLANICI.BELGETUR=MUHGONDERMETENKISICMAL.BELGETUR AND MUHGONDERMETENKISICMALKULLANICI.ICMALNO=MUHGONDERMETENKISICMAL.ICMALNO";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)form.belgeTur));
            if (form.icmalNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ICMALNO], "=", form.icmalNo));
            else
            {
                if (!string.IsNullOrWhiteSpace(form.kurKod))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", form.kurKod.Replace(".", "") + "%"));

                if (form.formDurum > 0)
                {
                    if (form.formDurum == 1)
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMDURUM], "<", 5));
                    else if (form.formDurum == 5)
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMDURUM], "=", 5));
                }
                if (form.icmalBas > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ICMALNO], ">=", form.icmalBas));

                if (form.icmalBit > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ICMALNO], "<=", form.icmalBit));

                if (!TNSDateTime.IsNull(form.araBasTarih))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITTARIH], ">=", form.araBasTarih));

                if (!TNSDateTime.IsNull(form.araBitTarih))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITTARIH], "<=", form.araBitTarih));

                if (form.kullanicilar != null && form.kullanicilar.Count > 0)
                {
                    GondermeTenkisIcmalKullanici gk = (GondermeTenkisIcmalKullanici)form.kullanicilar[0];
                    string kulKod = gk.kulKod;
                    sql = VeriErisim.KosulEkle(sql, "AND", String.Format("(1 IN (SELECT 1 FROM MUHGONDERMETENKISICMALKULLANICI WHERE MUHGONDERMETENKISICMALKULLANICI.YIL=MUHGONDERMETENKISICMAL.YIL AND MUHGONDERMETENKISICMALKULLANICI.BELGETUR=MUHGONDERMETENKISICMAL.BELGETUR AND MUHGONDERMETENKISICMALKULLANICI.ICMALNO=MUHGONDERMETENKISICMAL.ICMALNO AND MUHGONDERMETENKISICMALKULLANICI.KULLANICIKODU LIKE {0}))", VeriErisim.DegerYap("%" + kulKod + "%")));
                }
            }
            return veriErisim.DataReaderDondur(sql);
        }

        ///// <summary>
        ///// Form yapısında duran alanları kıstas olarak kullanarak,
        ///// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        ///// </summary>
        ///// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, GondermeTenkisIcmal yapısında nesne</param>
        ///// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        //public IDataReader ListeleKul(GondermeTenkisIcmal form)
        //{
        //    string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADKUL, ALANLARKUL) + " FROM " + TABLOADKUL;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKUL[(int)ENUMAlanlarKUL.YIL], "=", form.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKUL[(int)ENUMAlanlarKUL.BELGETUR], "=", (int)form.belgeTur));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKUL[(int)ENUMAlanlarKUL.ICMALNO], "=", form.icmalNo));
        //    return veriErisim.DataReaderDondur(sql);
        //}

        public Sonuc OnayKodAta(GondermeTenkisIcmal form)
        {
            string sql = "UPDATE " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "SET", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FORMDURUM], "=", form.formDurum));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)form.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ICMALNO], "=", form.icmalNo));
            return KomutCalistir(sql, 1);
        }
    }
}
