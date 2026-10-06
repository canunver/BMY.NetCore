using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
namespace TNS.KYM.IsKurallari.Veri
{
    /// <summary>
    /// YERLESKEODA veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABYERLESKEODA : TABOrtak
    {
        /// <summary>
        /// TABYERLESKEODA sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYERLESKEODA(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YERLESKEODA";

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
            /// GORKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GORKOD = 2,

            /// <summary>
            /// TARIF alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIF = 3,

            /// <summary>
            /// SAYMANLIKKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYMANLIKKOD = 4,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 5,

            /// <summary>
            /// ALTDALVAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTDALVAR = 6,

            /// <summary>
            /// USTKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            USTKOD = 7,

            YERTURKOD = 8,
            ODATURKOD = 9,
            ILKOD = 10,
            ORGKOD = 11,
            DURUM = 12,
            MALIYETMERKEZI = 13,
            GORKODESKI = 14,
            SIRANO = 15
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "KOD"
            , "AD"
            , "GORKOD"
            , "TARIF"
            , "SAYMANLIKKOD"
            , "BIRIMKOD"
            , "ALTDALVAR"
            , "USTKOD"
            , "YERTURKOD"
            , "ODATURKOD"
            , "ILKOD"
            , "ORGKOD"
            , "DURUM"
            , "MALIYETMERKEZI"
            , "GORKODESKI"
            , "SIRANO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, YerleskeOda yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(YerleskeOda form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, YerleskeOda yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(YerleskeOda form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.ad);
            sql += "," + VeriErisim.DegerYap(form.gorTamKod);
            sql += "," + VeriErisim.DegerYap(form.tarif);
            sql += "," + VeriErisim.DegerYap(form.saymanlikKod);
            sql += "," + VeriErisim.DegerYap(form.birimKod);

            sql += "," + VeriErisim.DegerYap(0);
            if (form.ustKod > 0)
                sql += "," + VeriErisim.DegerYap(form.ustKod);
            else
                sql += "," + VeriErisim.DegerYap((string)null);

            sql += "," + VeriErisim.DegerYap(form.yerTurKod);

            if (form.odaTurKod > 0)
                sql += "," + VeriErisim.DegerYap(form.odaTurKod);
            else
                sql += "," + VeriErisim.DegerYap((string)null);

            if (!string.IsNullOrWhiteSpace(form.ilKod))
                sql += "," + VeriErisim.DegerYap(form.ilKod.Replace(".", ""));
            else
                sql += "," + VeriErisim.DegerYap((string)null);

            if (form.orgKod > 0)
                sql += "," + VeriErisim.DegerYap(form.orgKod);
            else
                sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap(form.durum);
            sql += "," + VeriErisim.DegerYap(form.maliyetMerkezi);
            sql += "," + VeriErisim.DegerYap(form.gorKodEski);
            sql += "," + VeriErisim.DegerYap(form.siraNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, YerleskeOda yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(YerleskeOda form)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GORKOD], form.gorTamKod);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIF], form.tarif);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD], form.saymanlikKod);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], form.birimKod);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], form.durum);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MALIYETMERKEZI], form.maliyetMerkezi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], form.siraNo);

            sql += "," + ALANLAR[(int)ENUMAlanlar.YERTURKOD] + "=" + (form.yerTurKod > 0 ? VeriErisim.DegerYap(form.yerTurKod) : "NULL");
            sql += "," + ALANLAR[(int)ENUMAlanlar.ODATURKOD] + "=" + (form.odaTurKod > 0 ? VeriErisim.DegerYap(form.odaTurKod) : "NULL");
            sql += "," + ALANLAR[(int)ENUMAlanlar.ILKOD] + "=" + (!string.IsNullOrWhiteSpace(form.ilKod) ? VeriErisim.DegerYap(form.ilKod.Replace(".", "")) : "NULL");
            sql += "," + ALANLAR[(int)ENUMAlanlar.ORGKOD] + "=" + (form.orgKod > 0 ? VeriErisim.DegerYap(form.orgKod) : "NULL");

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleUstKod(YerleskeOda form)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + ALANLAR[(int)ENUMAlanlar.USTKOD] + " = " + (form.ustKod > 0 ? VeriErisim.DegerYap(form.ustKod) : "NULL");

            //if (!string.IsNullOrWhiteSpace(form.birimKod))
            //    sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], form.birimKod);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public bool AltDalVarMi(int kod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.ALTDALVAR] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return veriErisim.TekKolonSQLInt(sql) == 1;
        }

        public Sonuc PasifYap(YerleskeOda form)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], form.durum);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            //sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.USTKOD], "=", form.kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, YerleskeOda yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(YerleskeOda form, bool merkezBankasi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", UST.AD, UST.GORKOD, UST.TARIF, C.AD, B.ADI FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABLOAD + " UST ON UST.KOD=" + TABLOAD + ".USTKOD";
            sql += " LEFT OUTER JOIN ORGCOGRAFYA C ON C.KOD=" + TABLOAD + ".ILKOD";
            sql += " LEFT OUTER JOIN ORGBIRIM B ON B.KOD=" + TABLOAD + ".ORGKOD";

            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else if (form.ustKod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.USTKOD], "=", form.ustKod));
            else if (!string.IsNullOrWhiteSpace(form.birimKod))
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
            else if (form.kod == -100) //Tüm listeyi almak için
                sql = VeriErisim.KosulEkle(sql, "WHERE", "1=1");
            else
            {
                if (!string.IsNullOrWhiteSpace(form.tarif))
                    sql = VeriErisim.KosulEkle(sql, "WHERE", BosluktanParcalaKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TARIF], form.tarif));
                else
                    sql += " WHERE (YERLESKEODA.USTKOD IS NULL OR YERLESKEODA.USTKOD = -1) ";
            }

            if (form.durum == 100)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<", form.durum));
            else if (form.durum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", form.durum));

            if (merkezBankasi)
            {
                if (form.yerTurKod > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YERTURKOD], "=", form.yerTurKod));
                if (!string.IsNullOrWhiteSpace(form.gorKodKisa))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GORKOD], "=", form.gorKodKisa));
                if (!string.IsNullOrWhiteSpace(form.ad))
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + form.ad + "%"));
                    sql += " collate SQL_Latin1_General_CP1_CI_AS ";
                }
            }

            if (merkezBankasi)
                sql += " ORDER BY YERLESKEODA.BIRIMKOD, YERLESKEODA.USTKOD, YERLESKEODA.SIRANO, YERLESKEODA.AD";
            else
                sql += " ORDER BY YERLESKEODA.GORKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        private string BosluktanParcalaKosulOl(string tabloAd, string alanAd, string tarif)
        {
            string[] tarifler = tarif.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string kosul = "";
            foreach (var item in tarifler)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, alanAd, "LIKE", "%" + item + "%"));
            }
            return kosul;
        }

        public int EnYuksekKodBul()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            double maks = veriErisim.TekKolonSQLDbl(sql);
            int imaks = (int)maks;
            return imaks + 1;
        }

        public Sonuc AltDalliYap(int kod)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTDALVAR], 1);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return KomutCalistir(sql, 1);
        }

        public Sonuc SeviyeGuncelle(int kod)
        {
            string sql = @"DECLARE @SEVIYE int; 
                           WITH SEVIYEBUL AS (
                                    SELECT KOD, USTKOD, 1 AS SEVIYE
                                    FROM YERLESKEODA
                                    WHERE USTKOD IS NULL
                                    UNION ALL
                                    SELECT e.KOD, e.USTKOD, roc.SEVIYE + 1
                                    FROM YERLESKEODA e
                                    INNER JOIN SEVIYEBUL roc ON e.USTKOD = roc.KOD
                           )
                           SELECT @SEVIYE = (SELECT SEVIYE FROM SEVIYEBUL WHERE KOD = {KOD})
                           UPDATE YERLESKEODA SET SEVIYE = @SEVIYE WHERE KOD = {KOD}";
            sql = sql.Replace("{KOD}", kod.ToString());

            return KomutCalistir(sql, 1);
        }
    }
}
