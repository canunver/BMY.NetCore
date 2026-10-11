using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.YAT;
namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// YATALTPROJEBILGI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABYATALTPROJEBILGI : TABOrtak
    {
        /// <summary>
        /// TABYATYATALTPROJEBILGI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATALTPROJEBILGI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /*
         * 
         
CREATE TABLE YATALTPROJEBILGI
    (YIL INT
    , PROJENO INT
    , ALTPROJENO INT
    , ALANAD VARCHAR(50)
    , SATIRNO INT
    , METIN VARCHAR(300)
    PRIMARY KEY (YIL, ALTPROJENO, ALANAD, SATIRNO)
    )
            CREATE TABLE YATALTPROJEYETKI
    (KULLANICIKODU INT
    , PROJENO INT
    , ALTPROJENO INT
    , PRIMARY KEY (KULLANICIKODU, PROJENO, ALTPROJENO)
    )
         
         * */

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATALTPROJEBILGI";


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADYETKI = "YATALTPROJEYETKI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarYETKI : int
        {

            /// <summary>
            /// PROJENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PROJENO = 0,

            /// <summary>
            /// ALTPROJENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTPROJENO = 1,

            /// <summary>
            /// KULLANICIKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 2,
        }


        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARYETKI = {
            "PROJENO"
            , "ALTPROJENO"
            , "KULLANICIKODU"
        };


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
            /// ALTPROJENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PROJENO = 1,

            /// <summary>
            /// ALTPROJENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTPROJENO = 2,

            /// <summary>
            /// ALANAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALANAD = 3,

            /// <summary>
            /// SATIRNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATIRNO = 4,

            /// <summary>
            /// METIN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            METIN = 5,

            SENARYOKOD = 6
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "PROJENO"
            , "ALTPROJENO"
            , "ALANAD"
            , "SATIRNO"
            , "METIN"
            , "SENARYOKOD"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, YATALTPROJEBILGI yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, string senaryoKod, int projeNo, int altProjeNo, string[] alanlar)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (altProjeNo == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], "=", projeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeNo));
            if (alanlar != null && alanlar.Length > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", InSQL("", ALANLAR[(int)ENUMAlanlar.ALANAD], alanlar));

            return KomutCalistir(sql, 0);
        }

        private string InSQL(string tabloAd, string alanAd, string[] degerler)
        {
            string liste = "";
            if (degerler != null)
            {
                for (int i = 0; i < degerler.Length; i++)
                {
                    if (i != 0) liste += ",";
                    liste += VeriErisim.DegerYap(degerler[i]);
                }
            }
            if (!string.IsNullOrWhiteSpace(liste))
            {
                if (!string.IsNullOrWhiteSpace(tabloAd))
                    alanAd = tabloAd + "." + alanAd;
                liste = alanAd + " IN (" + liste + ")";
            }
            return liste;
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, YATALTPROJEBILGI yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int yil, string senaryoKod, int projeNo, int altProjeNo, string alanAd, int satirNo, string metin)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(projeNo);
            sql += "," + VeriErisim.DegerYap(altProjeNo);
            sql += "," + VeriErisim.DegerYap(alanAd);
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(metin);
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));

            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, YATALTPROJEBILGI yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int yil, string senaryoKod, int projeNo, int altProjeNo, string[] alanlar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (altProjeNo >= 0)
            {
                if (altProjeNo == 0)

                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], "=", projeNo));

                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeNo));
            }
            if (alanlar != null && alanlar.Length > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", InSQL("", ALANLAR[(int)ENUMAlanlar.ALANAD], alanlar));
            sql += " ORDER BY 1,2,3,4";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc BilgiKopyala(int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, bool proje, string projeKodListe)
        {
            string sql = "INSERT INTO " + TABLOAD + "(YIL, SENARYOKOD, PROJENO, ALTPROJENO, ALANAD, SATIRNO, METIN) ";
            sql += "SELECT " + VeriErisim.DegerYap(hedefYil) + " YIL, " + VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " SENARYOKOD, PROJENO, ALTPROJENO, ALANAD, SATIRNO, METIN FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], proje ? "=" : "<>", 0));

            sql = VeriErisim.KosulEkle(sql, "AND", "(1 NOT in (SELECT 1 FROM " + TABLOAD + $" IC WHERE IC.YIL = {hedefYil}  AND IC.SENARYOKOD={VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} AND IC.PROJENO ={TABLOAD}.PROJENO AND IC.ALTPROJENO={TABLOAD}.ALTPROJENO))");

            if (projeKodListe != null)
            {
                if (proje)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", "(PROJENO in (" + projeKodListe + "))");
                }
                else
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", "(1 in (" + AltProjeProjeyeBagla(projeKodListe) + "))");
                }
            }
            return KomutCalistir(sql, 0);
        }

        private string AltProjeProjeyeBagla(string projeKodListe)
        {
            return "SELECT 1 FROM YATALTPROJE WHERE YATALTPROJE.YIL = YATALTPROJEBILGI.YIL AND YATALTPROJE.SENARYOKOD = YATALTPROJEBILGI.SENARYOKOD AND YATALTPROJE.ALTPROJENO = YATALTPROJEBILGI.ALTPROJENO AND YATALTPROJE.PROJENO IN (" + projeKodListe + ")";
        }

        public int YetkiliKullaniciSay(string kullaniciKodu, int projeNo, int altProjeNo)
        {
            string sql = "SELECT COUNT(*) FROM YATALTPROJEYETKI";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "KULLANICIKODU", "=", kullaniciKodu));
            if (altProjeNo == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], "=", projeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeNo));
            return this.veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, ALTPROJEYETKI yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int projeNo, int altProjeNo)
        {
            string sql = "DELETE FROM " + TABLOADYETKI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYETKI[(int)ENUMAlanlarYETKI.ALTPROJENO], "=", altProjeNo));
            if (altProjeNo == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARYETKI[(int)ENUMAlanlarYETKI.PROJENO], "=", projeNo));

            return KomutCalistir(sql, 0);
        }


        /// <summary>
        /// Form yapısında duran, aLTPROJEYETKIDETAY dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, ALTPROJEYETKI yapısında nesne</param>
        /// <param name="satirNo">aLTPROJEYETKIDETAY dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(string kullaniciKodu, int projeNo, int altProjeNo)
        {
            string sql = "INSERT INTO " + TABLOADYETKI + "(" + VeriErisim.TumAlanlar("", ALANLARYETKI) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(projeNo);
            sql += "," + VeriErisim.DegerYap(altProjeNo);
            sql += "," + VeriErisim.DegerYap(kullaniciKodu);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, ALTPROJEYETKI yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(int projeNo, int altProjeNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADYETKI, ALANLARYETKI) + ",K.AD, K.SOYAD, K.EMAIL, K.TELEFON FROM " + TABLOADYETKI + " LEFT OUTER JOIN Kullanici.dbo.KULLANICI K ON YATALTPROJEYETKI.KULLANICIKODU = K.KULLANICIKODU";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYETKI[(int)ENUMAlanlarYETKI.ALTPROJENO], "=", altProjeNo));
            if (altProjeNo == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARYETKI[(int)ENUMAlanlarYETKI.PROJENO], "=", projeNo));
            return veriErisim.DataReaderDondur(sql);
        }

        internal static string Listele(string UstAlanAd, string senaryoUstAlanAd, string projeUstAlanAd, string altProjeUstAlanAd)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + UstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + "=" + senaryoUstAlanAd);

            if (!string.IsNullOrWhiteSpace(projeUstAlanAd))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJENO] + "=" + projeUstAlanAd);
                sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALTPROJENO] + "= 0");
            }

            if (!string.IsNullOrWhiteSpace(altProjeUstAlanAd))
                sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALTPROJENO] + "=" + altProjeUstAlanAd);
            sql += " ORDER BY 1,2,3,4,5,6";

            return sql;
        }
    }
}
