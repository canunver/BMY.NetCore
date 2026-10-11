using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.YAT;
namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// YATALTPROJEDETAYPROGRAMI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABYATALTPROJEDETAYPROGRAMI : TABOrtak
    {
        /// <summary>
        /// TABYATALTPROJEDETAYPROGRAMI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATALTPROJEDETAYPROGRAMI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// 
        /// CREATE TABLE YATALTPROJEDETAYPROGRAMI
        ///   (YIL INT
        ///   , SENARYOKOD VARCHAR(20)
        ///   , ALTPROJENO INT
        ///   , SIRANO INT
        ///   , YER VARCHAR(200)
        ///   , BIRIMI VARCHAR(200)
        ///   , SAYMANLIKKOD INT
        ///   , BIRIMKOD VARCHAR(3)
        ///   , ODENEK MONEY
        ///   , ACIKLAMA VARCHAR(2000)
        ///   , PRIMARY KEY (YIL, SENARYOKOD, ALTPROJENO, SIRANO))
        /// </summary>
        public static string TABLOAD = "YATALTPROJEDETAYPROGRAMI";

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
            /// SENARYOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 1,

            /// <summary>
            /// ALTPROJENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTPROJENO = 2,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,

            /// <summary>
            /// YER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YER = 4,

            /// <summary>
            /// BIRIMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMI = 5,

            /// <summary>
            /// SAYMANLIKKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYMANLIKKOD = 6,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 7,

            /// <summary>
            /// ODENEK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK = 8,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 9,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "SENARYOKOD"
            , "ALTPROJENO"
            , "SIRANO"
            , "YER"
            , "BIRIMI"
            , "SAYMANLIKKOD"
            , "BIRIMKOD"
            , "ODENEK"
            , "ACIKLAMA"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, AltProje yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(AltProjeDetayProgrami form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", form.senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", form.altProjeNo));
            return KomutCalistir(sql, 0);
        }


        public Sonuc Sil(int yil, string senaryoKod, int altProjeNo)
        {
            AltProjeDetayProgrami form = new AltProjeDetayProgrami();
            form.yil = yil;
            form.senaryoKod = senaryoKod;
            form.altProjeNo = altProjeNo;
            return Sil(form);
        }

        /// <summary>
        /// Form yapısında duran, altProjeDetayProgrami dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, AltProje yapısında nesne</param>
        /// <param name="satirNo">altProjeDetayProgrami dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(AltProjeDetayProgrami form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.altProjeNo);
            sql += "," + VeriErisim.DegerYap(form.siraNo);
            sql += "," + VeriErisim.DegerYap(form.yer);
            sql += "," + VeriErisim.DegerYap(form.birimi);
            sql += "," + VeriErisim.DegerYap(form.saymanlikKod);
            sql += "," + VeriErisim.DegerYap(form.birimKod);
            sql += "," + VeriErisim.DegerYap(form.odenek);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, AltProje yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(AltProjeDetayProgrami form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", form.senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", form.altProjeNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kopyala(int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, string projeKodListe)
        {
            string sql = "INSERT INTO " + TABLOAD + "(YIL , SENARYOKOD , ALTPROJENO , SIRANO , YER , BIRIMI , SAYMANLIKKOD , BIRIMKOD , ODENEK , ACIKLAMA)";
            sql += "SELECT " + VeriErisim.DegerYap(hedefYil) + " YIL, " + VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " SENARYOKOD, YATALTPROJEDETAYPROGRAMI.ALTPROJENO , SIRANO , YER , BIRIMI , SAYMANLIKKOD , BIRIMKOD , ODENEK , ACIKLAMA FROM " + TABLOAD + " INNER JOIN YATALTPROJE ON YATALTPROJE.ALTPROJENO = YATALTPROJEDETAYPROGRAMI.ALTPROJENO AND YATALTPROJE.YIL = YATALTPROJEDETAYPROGRAMI.YIL AND YATALTPROJE.SENARYOKOD = YATALTPROJEDETAYPROGRAMI.SENARYOKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", "(1 NOT in (SELECT 1 FROM " + TABLOAD + $" IC WHERE IC.YIL = {hedefYil}  AND IC.SENARYOKOD={VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} AND IC.ALTPROJENO ={TABLOAD}.ALTPROJENO))");
            if (projeKodListe != null)
                sql = VeriErisim.KosulEkle(sql, "AND", "(YATALTPROJE.ALTPROJENO in (" + projeKodListe + "))");
            return KomutCalistir(sql, 0);
        }

        public Sonuc DetayProgramKopyala(int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, string projeKodListe)
        {
            string sql = $"INSERT INTO YATALTPROJEDETAYPROGRAMI SELECT {VeriErisim.DegerYap(hedefYil)} YIL, {VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} SENARYOKOD, ALTPROJENO, SIRANO, YER, BIRIMI, SAYMANLIKKOD, BIRIMKOD, ODENEK, ACIKLAMA FROM YATALTPROJEDETAYPROGRAMI TABKAYNAK";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TABKAYNAK", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TABKAYNAK", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", $"(1 NOT in (SELECT 1 FROM YATALTPROJEDETAYPROGRAMI IC WHERE IC.YIL = {hedefYil}  AND IC.SENARYOKOD={VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} AND IC.ALTPROJENO =TABKAYNAK.ALTPROJENO))");

            sql += $" AND ALTPROJENO IN (SELECT ALTPROJENO FROM YATALTPROJE WHERE YATALTPROJE.YIL = TABKAYNAK.YIL AND YATALTPROJE.SENARYOKOD = TABKAYNAK.SENARYOKOD AND YATALTPROJE.ALTPROJENO = TABKAYNAK.ALTPROJENO AND YATALTPROJE.PROJENO IN ({projeKodListe}))";
            return KomutCalistir(sql, 0);
        }
    }
}
