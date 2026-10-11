using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
namespace TNS.IKY.IsKurallari.Veri
{
    /// <summary>
    /// IKYPERSONELTIPI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIKYPERSONELTIPI : TABOrtak
    {
        /// <summary>
        /// TABIKYPERSONELTIPI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIKYPERSONELTIPI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }
        /*
        CREATE TABLE IKYPERSONELTIPI
    (KOD INT
    , AD VARCHAR(255)
    , KURUMDISI INT
    , SIRA INT
    , KADROLU INT
    , GOSTER INT
    , GRUPAD VARCHAR(50)
    , MAASGRUPAD VARCHAR(50)
    , AKTIF INT
    , TERFIYEDAHIL INT
    , MAASADAHIL INT
    , HITAPMI INT
    , H1 int NULL,
    , H2 int NULL,
    , H3 int NULL,
    , H4 int NULL,
    , H5 int NULL,
    , H6 int NULL,
    , H7 int NULL,
    , HITAP int NULL 
    , PRIMARY KEY (KOD)
    )
*/
        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "IKYPERSONELTIPI";

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
            /// AD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1,

            /// <summary>
            /// KURUMDISI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMDISI = 2,

            /// <summary>
            /// SIRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRA = 3,

            /// <summary>
            /// KADROLU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KADROLU = 4,

            /// <summary>
            /// GOSTER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOSTER = 5,

            /// <summary>
            /// GRUPAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GRUPAD = 6,

            /// <summary>
            /// MAASGRUPAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MAASGRUPAD = 7,

            /// <summary>
            /// AKTIF alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AKTIF = 8,

            /// <summary>
            /// TERFIYEDAHIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TERFIYEDAHIL = 9,

            /// <summary>
            /// MAASDAHIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MAASADAHIL = 10,

            /// <summary>
            /// HITAPMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HITAPMI = 11,
            H1 = 12,
            H2 = 13,
            H3 = 14,
            H4 = 15,
            H5 = 16,
            H6 = 17,
            H7 = 18,
            HITAP = 19,
            PESINMI = 20,
            MAASDONEMGUN = 21,
            MAASODEMEGUN = 22
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "KOD"
            , "AD"
            , "KURUMDISI"
            , "SIRA"
            , "KADROLU"
            , "GOSTER"
            , "GRUPAD"
            , "MAASGRUPAD"
            , "AKTIF"
            , "TERFIYEDAHIL"
            , "MAASADAHIL"
            , "HITAPMI"
            , "H1"
            , "H2"
            , "H3"
            , "H4"
            , "H5"
            , "H6"
            , "H7"
            , "HITAP"
            , "PESINMI"
            , "MAASDONEMGUN"
            , "MAASODEMEGUN"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, PersonelTipi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(PersonelTipi form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PersonelTipi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(PersonelTipi form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.ad);
            sql += "," + VeriErisim.DegerYap(form.kurumDisi);
            sql += "," + VeriErisim.DegerYap(form.sira);
            sql += "," + VeriErisim.DegerYap(form.kadrolu);
            sql += "," + VeriErisim.DegerYap(form.goster);
            sql += "," + VeriErisim.DegerYap(form.grupAd);
            sql += "," + VeriErisim.DegerYap(form.maasGrupAd);
            sql += "," + VeriErisim.DegerYap(form.aktif);
            sql += "," + VeriErisim.DegerYap(form.terfiyeDahil);
            sql += "," + VeriErisim.DegerYap(form.maasaDahil);
            sql += "," + VeriErisim.DegerYap(form.hitapMi);

            sql += "," + VeriErisim.DegerYap(form.H1);
            sql += "," + VeriErisim.DegerYap(form.H2);
            sql += "," + VeriErisim.DegerYap(form.H3);
            sql += "," + VeriErisim.DegerYap(form.H4);
            sql += "," + VeriErisim.DegerYap(form.H5);
            sql += "," + VeriErisim.DegerYap(form.H6);
            sql += "," + VeriErisim.DegerYap(form.H7);
            sql += "," + VeriErisim.DegerYap((int)form.HITAP);
            sql += "," + VeriErisim.DegerYap(form.pesinMi);

            sql += "," + VeriErisim.DegerYap(form.maasDonemGun);
            sql += "," + VeriErisim.DegerYap(form.maasOdemeGun);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, PersonelTipi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(PersonelTipi form)
        {
            string sql = "";
            if (form.aktif == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], "=", 1));

            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            if (form.tipGruplar != null && form.tipGruplar.Length > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", ALANLAR[(int)ENUMAlanlar.GRUPAD] + " IN (" + Virgulle(form.tipGruplar) + ")");
                string inSql = " (KOD IN (SELECT DISTINCT PERSONELTUR FROM IKYPERSONELKADRO WHERE PERSONELTUR = DIS.KOD AND " + TABGenel.KadroDurumuAktifler("KADRODURUMU") + "))";
                sql = VeriErisim.KosulEkle(sql, "AND", inSql);
            }
            if (sql != "") sql = " WHERE " + sql;
            sql = "SELECT " + VeriErisim.TumAlanlar("DIS", ALANLAR) + " FROM " + TABLOAD + " DIS "+ sql;
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRA];
            return veriErisim.DataReaderDondur(sql);
        }

        private string Virgulle(string[] tipGruplar)
        {
            string donusDeger = "";
            for (int i = 0; i < tipGruplar.Length; i++)
            {
                if (i > 0) donusDeger += ",";
                donusDeger += VeriErisim.DegerYap(tipGruplar[i]);
            }
            return donusDeger;
        }

        public IDataReader ListeleGrup(bool sadeceKullanilanlar)
        {
            string sql = "SELECT GRUPAD, MIN(SIRA) FROM " + TABLOAD + " DIS ";
            if (sadeceKullanilanlar)
            {
                string inSql = " KOD IN (SELECT DISTINCT PERSONELTUR FROM IKYPERSONELKADRO WHERE PERSONELTUR = DIS.KOD AND " + TABGenel.KadroDurumuAktifler("KADRODURUMU")+ ")";
                sql = VeriErisim.KosulEkle(sql, "WHERE", inSql);
            }
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], "=", 1));
            sql += " GROUP BY GRUPAD ORDER BY 2";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
