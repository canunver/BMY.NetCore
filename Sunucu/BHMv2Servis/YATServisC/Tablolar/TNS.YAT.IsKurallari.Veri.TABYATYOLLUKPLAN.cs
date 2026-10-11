using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.YAT;
using TNS.BKD;
namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// YATYOLLUKPLAN veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABYATYOLLUKPLAN : TABOrtak
    {
        /// <summary>
        /// TABYATYOLLUKPLAN sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// CREATE TABLE YATYOLLUKPLAN
        ///  (YIL INT
        ///   , SENARYOKOD VARCHAR(20)
        ///   , PEBKOD VARCHAR(20)
        ///   , SIRANO INT
        ///   , DAIRESI VARCHAR(200)
        ///   , KONUSU VARCHAR(200)
        ///   , MUHTELIFMI INT
        ///   , SURESI INT
        ///   , KISISAYISI INT
        ///   , YAPILACAGIAY VARCHAR(200)
        ///   , YERI VARCHAR(200)
        ///   , KURKOD VARCHAR(20)
        ///   , FONKOD VARCHAR(20)
        ///   , FINKOD VARCHAR(20)
        ///   , EKOKOD VARCHAR(20)
        ///   , MALIYET MONEY
        ///   , PRIMARY KEY (YIL, SENARYOKOD, PEBKOD, SIRANO)
        ///  )
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATYOLLUKPLAN(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATYOLLUKPLAN";

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
            /// PEBKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 2,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,

            /// <summary>
            /// DAIRESI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DAIRESI = 4,

            /// <summary>
            /// KONUSU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KONUSU = 5,

            /// <summary>
            /// MUHTELIFMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHTELIFMI = 6,

            /// <summary>
            /// SURESI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURESI = 7,

            /// <summary>
            /// KISISAYISI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KISISAYISI = 8,

            /// <summary>
            /// YAPILACAGIAY alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAPILACAGIAY = 9,

            /// <summary>
            /// YERI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YERI = 10,

            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 11,

            /// <summary>
            /// FONKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKOD = 12,

            /// <summary>
            /// FINKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKOD = 13,

            /// <summary>
            /// EKOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 14,

            /// <summary>
            /// MALIYET alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MALIYET = 15,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "SENARYOKOD"
            , "PEBKOD"
            , "SIRANO"
            , "DAIRESI"
            , "KONUSU"
            , "MUHTELIFMI"
            , "SURESI"
            , "KISISAYISI"
            , "YAPILACAGIAY"
            , "YERI"
            , "KURKOD"
            , "FONKOD"
            , "FINKOD"
            , "EKOKOD"
            , "MALIYET"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, YollukPlanAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, string senaryoKod, Tertip tertip)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, yollukPlan dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, YollukPlanAna yapısında nesne</param>
        /// <param name="satirNo">yollukPlan dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int yil, string senaryoKod, Tertip tertip, int satirNo, YollukPlan form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(form.dairesi);
            sql += "," + VeriErisim.DegerYap(form.konusu);
            sql += "," + VeriErisim.DegerYap(form.muhtelifMi ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(form.suresi);
            sql += "," + VeriErisim.DegerYap(form.kisiSayisi);
            sql += "," + VeriErisim.DegerYap(form.yapilacagiAy);
            sql += "," + VeriErisim.DegerYap(form.yeri);
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.mALIYET);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, YollukPlanAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int yil, string senaryoKod, Tertip tertip, string pebKodListe)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", YATALTPROJE.AD, KUR.KURAD, PEB.PEBAD, PEB2.PEBAD FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN YATBUTCEKAYNAKPEBKOD ON " + VeriErisim.Join(TABLOAD, "YATBUTCEKAYNAKPEBKOD", new string[] { "YIL", "SENARYOKOD", "PEBKOD" });
            sql += " LEFT OUTER JOIN YATBUTCEKAYNAK ON " + VeriErisim.Join("YATBUTCEKAYNAKPEBKOD", "YATBUTCEKAYNAK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sql += " LEFT OUTER JOIN YATALTPROJE ON " + VeriErisim.Join("YATBUTCEKAYNAK", "YATALTPROJE", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSKUR KUR ON " + VeriErisim.Join("YATALTPROJE", "KUR", new string[] { "YIL", "BIRIM" }, new string[] { "YIL", "KURKOD" });
            sql += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD PEB ON " + VeriErisim.Join(TABLOAD, "PEB", new string[] { "YIL", "PEBKOD" }, new string[] { "YIL", "PEBKOD" });
            sql += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD PEB2 ON " + TABLOAD + ".YIL=PEB2.YIL AND LEFT(" + TABLOAD + ".PEBKOD,2) = PEB2.PEBKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (tertip != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 0, "").Replace(".", "") + "%"));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], "LIKE", OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 1, "").Replace(".", "") + "%"));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FINKOD], "LIKE", OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 2, "").Replace(".", "") + "%"));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 3, "").Replace(".", "") + "%"));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PEBKOD], "LIKE", OrtakFonksiyonlar.DiziElemanAl(tertip.kodlar, 4, "").Replace(".", "") + "%"));
            }
            if (!string.IsNullOrWhiteSpace(pebKodListe))
            {
                sql += VeriErisim.IN_KosulOl("AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PEBKOD], pebKodListe, true, true);
            }
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc YollukPlanKopyala(int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, string projeKodListe)
        {
            string sql = $"INSERT INTO YATYOLLUKPLAN SELECT {VeriErisim.DegerYap(hedefYil)} YIL, {VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} SENARYOKOD, PEBKOD, SIRANO, DAIRESI, KONUSU, MUHTELIFMI, SURESI, KISISAYISI, YAPILACAGIAY, YERI, KURKOD,FONKOD, FINKOD, EKOKOD, MALIYET FROM YATYOLLUKPLAN TABKAYNAK";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TABKAYNAK", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TABKAYNAK", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", $"(1 NOT in (SELECT 1 FROM YATYOLLUKPLAN IC WHERE IC.YIL = {hedefYil} AND IC.SENARYOKOD={VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} AND IC.PEBKOD=TABKAYNAK.PEBKOD))");

            sql += $" AND PEBKOD IN (SELECT PEBKOD FROM YATBUTCEKAYNAKPEBKOD INNER JOIN YATBUTCEKAYNAK ON YATBUTCEKAYNAKPEBKOD.YIL = YATBUTCEKAYNAK.YIL AND YATBUTCEKAYNAKPEBKOD.SENARYOKOD = YATBUTCEKAYNAK.SENARYOKOD AND YATBUTCEKAYNAKPEBKOD.KAYNAKNO = YATBUTCEKAYNAK.KAYNAKNO INNER JOIN YATALTPROJE ON YATALTPROJE.YIL = YATBUTCEKAYNAK.YIL AND YATALTPROJE.SENARYOKOD = YATBUTCEKAYNAK.SENARYOKOD AND YATALTPROJE.ALTPROJENO = YATBUTCEKAYNAK.ALTPROJENO WHERE YATBUTCEKAYNAKPEBKOD.YIL = TABKAYNAK.YIL AND YATBUTCEKAYNAKPEBKOD.SENARYOKOD = TABKAYNAK.SENARYOKOD AND YATBUTCEKAYNAKPEBKOD.PEBKOD = TABKAYNAK.PEBKOD AND YATALTPROJE.PROJENO IN ({projeKodListe}))";
            return KomutCalistir(sql, 0);
        }
    }
}
