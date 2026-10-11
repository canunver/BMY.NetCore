using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using static TNS.HRC.IsKurallari.Veri.TABDBIKRAZ;
using System.Collections.Generic;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// TABDBHAKEDIS veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABDBHAKEDIS : TABOrtak
    {
        /// <summary>
        /// TABHRCDBYOLLUKBILGISI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABDBHAKEDIS(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "DBHAKEDIS";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// HAKEDISID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HAKEDISID = 0,

            /// <summary>
            /// BUTCEREFID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUTCEREFID = 1,

            /// <summary>
            /// ILGILIKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILIKOD = 2,

            /// <summary>
            /// HAKEDISTURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HAKEDISTURKOD = 3,

            /// <summary>
            /// SOZLESMEID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOZLESMEID = 4,

            /// <summary>
            /// BELGETURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGETURKOD = 5,

            /// <summary>
            /// BELGETARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGETARIHI = 6,

            /// <summary>
            /// BELGENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGENO = 7,

            /// <summary>
            /// PARABIRIMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PARABIRIMI = 8,

            /// <summary>
            /// KURTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURTARIHI = 9,

            /// <summary>
            /// KDVHARICBELGETOPLAMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVHARICBELGETOPLAMI = 10,

            /// <summary>
            /// KDVGIRISTURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVGIRISTURKOD = 11,

            /// <summary>
            /// KDVORANI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVORANI = 12,

            /// <summary>
            /// KDVTUTARI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVTUTARI = 13,

            /// <summary>
            /// AVANSKESINTISI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AVANSKESINTISI = 14,

            /// <summary>
            /// DURUMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUMKOD = 15,

            /// <summary>
            /// GEREKCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 16,


            /// <summary>
            /// HAKEDISNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HAKEDISNO = 17,

            /// <summary>
            /// NUMARATORALMATARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NUMARATORALMATARIHI = 18

        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "HAKEDISID"
            , "BUTCEREFID"
            , "ILGILIKOD"
            , "HAKEDISTURKOD"
            , "SOZLESMEID"
            , "BELGETURKOD"
            , "BELGETARIHI"
            , "BELGENO"
            , "PARABIRIMI"
            , "KURTARIHI"
            , "KDVHARICBELGETOPLAMI"
            , "KDVGIRISTURKOD"
            , "KDVORANI"
            , "KDVTUTARI"
            , "AVANSKESINTISI"
            , "DURUMKOD"
            , "GEREKCE"
            , "HAKEDISNO"
            , "NUMARATORALMATARIHI"
        };

        public enum ENUMAlanlarOdeme : int
        {
            ODENEKID = 0,
            HAKEDISID = 1,
            KOD = 2,
            MIKTAR = 3
        }

        public static string TABLOADODENEKLER = "DBHAKEDISODENEKLER";
        public static string[] ALANLARODENEKLER = {
            "HAKEDISID",
            "EKOKOD",
            "MIKTAR",
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DBHakedis yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(DBHakedis form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HAKEDISID], "=", form.hakedisID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBHakedis yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(DBHakedis form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0, 18 }) + ") OUTPUT INSERTED." + ALANLAR[(int)ENUMAlanlar.HAKEDISID] + " VALUES";
            sql += "(" + ((form.butceRefID <= 0) ? "NULL" : VeriErisim.DegerYap(form.butceRefID));
            sql += "," + ((form.ilgiliKod <= 0) ? "NULL" : VeriErisim.DegerYap(form.ilgiliKod));
            sql += "," + VeriErisim.DegerYap(form.hakedisTurKod);
            sql += "," + ((form.sozlesmeID <= 0) ? "NULL" : VeriErisim.DegerYap(form.sozlesmeID));
            sql += "," + VeriErisim.DegerYap(form.belgeTurKod);
            sql += "," + VeriErisim.DegerYap(form.belgeTarihi);
            sql += "," + VeriErisim.DegerYap(form.belgeNo);
            sql += "," + VeriErisim.DegerYap(form.paraBirimi);
            sql += "," + VeriErisim.DegerYap(form.kurTarihi);
            sql += "," + VeriErisim.DegerYap(form.kdvHaricBelgeToplami);
            sql += "," + VeriErisim.DegerYap(form.kdvGirisTurKod);
            sql += "," + VeriErisim.DegerYap(form.kdvOrani);
            sql += "," + VeriErisim.DegerYap(form.kdvTutari);
            sql += "," + VeriErisim.DegerYap(form.avansKesintisi);
            sql += "," + VeriErisim.DegerYap(1);
            sql += "," + VeriErisim.DegerYap(form.hakedisGerekce);
            sql += "," + VeriErisim.DegerYap(form.hakedisNo);
            sql += ")";

            Sonuc s = new Sonuc();
            int anahtar = veriErisim.TekKolonSQLInt(sql);
            if (string.IsNullOrWhiteSpace(veriErisim.hata))
                s.anahtar = anahtar.ToString();
            else
                s.DegerAta(veriErisim.hata);
            return s;
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBHakedis yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(DBHakedis form)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += $" SET {ALANLAR[(int)ENUMAlanlar.BUTCEREFID]} = " + (form.butceRefID <= 0 ? "NULL" : VeriErisim.DegerYap(form.butceRefID));
            sql += $", {ALANLAR[(int)ENUMAlanlar.ILGILIKOD]} = " + (form.ilgiliKod <= 0 ? "NULL" : VeriErisim.DegerYap(form.ilgiliKod));
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAKEDISTURKOD], form.hakedisTurKod);
            sql += $", {ALANLAR[(int)ENUMAlanlar.SOZLESMEID]} = " + (form.sozlesmeID <= 0 ? "NULL" : VeriErisim.DegerYap(form.sozlesmeID));
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETURKOD], form.belgeTurKod);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIHI], form.belgeTarihi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], form.belgeNo);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMI], form.paraBirimi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURTARIHI], form.kurTarihi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVHARICBELGETOPLAMI], form.kdvHaricBelgeToplami);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVGIRISTURKOD], form.kdvGirisTurKod);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVORANI], form.kdvOrani);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVTUTARI], form.kdvTutari);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AVANSKESINTISI], form.avansKesintisi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCE], form.hakedisGerekce);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAKEDISNO], form.hakedisNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HAKEDISID], "=", form.hakedisID));
            return KomutCalistir(sql, 1);
        }

        public static string IcSorguSarmala(string icSorgu)
        {
            return "\r\n (" + icSorgu + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
        }

        public static string HakedisSahipleriSql(bool detay, string ustAlan)
        {
            if (!detay) return "NULL";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADHAKSAHIBI, ALANLARHAKSAHIBI) + ", HRCFIRMABILGISI.VERGINO, HRCFIRMABILGISI.AD, HRCFIRMABILGISI.HESAPNO, KURUSD.DOVIZALIS, KUREUR.DOVIZALIS FROM " + TABLOADHAKSAHIBI;
            sql += " LEFT OUTER JOIN HRCFIRMABILGISI ON HRCFIRMABILGISI.KOD=DBHAKEDISSAHIBI.ILGILIKOD ";
            sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KURUSD ON KURUSD.KURTARIH = DBHAKEDISSAHIBI.ODEMETARIHI AND KURUSD.PARABIRIMI = 'USD'";
            sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KUREUR ON KUREUR.KURTARIH = DBHAKEDISSAHIBI.ODEMETARIHI AND KUREUR.PARABIRIMI = 'EUR'";

            sql += " WHERE HAKEDISID = " + ustAlan + " ORDER BY DBHAKEDISSAHIBI.NUMARATOR ";

            return IcSorguSarmala(sql);
        }

        public static string YollukSql(bool detay, string ustAlan)
        {
            if (!detay) return "NULL";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDBYOLLUK, ALANLARYOLLUK) + " FROM " + TABLOADDBYOLLUK;
            sql += " WHERE HAKEDISID = " + ustAlan;
            sql += " ORDER BY " + ALANLARYOLLUK[(int)ENUMAlanlarDBYOLLUK.SIRANO];

            return IcSorguSarmala(sql);
        }

        public static string MalzemeSql(bool detay, string ustAlan)
        {
            if (!detay) return "NULL";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMALZEME, ALANLARMALZEME) + " FROM " + TABLOADMALZEME;
            sql += " WHERE HAKEDISID = " + ustAlan;
            sql += " ORDER BY " + ALANLARMALZEME[(int)ENUMAlanlarMALZEME.SIRANO];
            return IcSorguSarmala(sql);
        }

        public static string TarihceSql(bool detay, string ustAlan)
        {
            if (!detay) return "NULL";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABDBTARIHCE.TABLOAD, TABDBTARIHCE.ALANLAR) + " FROM " + TABDBTARIHCE.TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", TABDBTARIHCE.ALANLAR[(int)TABDBTARIHCE.ENUMAlanlar.MODULKOD], "=", "DBH"));
            sql += " AND " + TABDBTARIHCE.ALANLAR[(int)TABDBTARIHCE.ENUMAlanlar.MODULANAHTAR] + " = CONVERT(VARCHAR, " + ustAlan + ")";

            sql += " ORDER BY " + TABDBTARIHCE.ALANLAR[(int)TABDBTARIHCE.ENUMAlanlar.ISLEMTARIHI] + " DESC";
            return IcSorguSarmala(sql);
        }

        public static string OdeneklerSql(bool detay, string ustAlan)
        {
            if (!detay) return "NULL";

            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADODENEKLER, ALANLARODENEKLER) + ", EKO.EKOAD";
            sql += " FROM " + TABLOADODENEKLER;
            sql += $" LEFT OUTER JOIN ButceHazirlik.dbo.ABSEKO EKO ON EKO.YIL=2025 AND EKO.EKOKOD={TABLOADODENEKLER}.EKOKOD";
            sql += " WHERE HAKEDISID = " + ustAlan;

            return IcSorguSarmala(sql);
        }


        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DBHakedis yapısında nesne</param>
        /// <param name="detayOku">Ana tablonun detayları okunacak mı?</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(DBHakedis form, bool detayOku, int siralaYontem)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", DBBUTCEREF.ACIKLAMATR, DBBUTCEREF.REFERANSNO, DBSATINALMAPLANI.BUTCEKATEGORI, DBSATINALMAPLANI.REFERANSNO, DBSATINALMAPLANI.ONCESONRA, DBALTBILESEN.ALTBILESENID, DBALTBILESEN.ALTBILESENNO, DBALTBILESEN.ILGILIBIRIMBUTCEKODU, DBALTBILESEN.ALTBILESENAD, DBALTBILESEN.ILGILIBIRIMADI, DBIKRAZ.IKRAZID, DBIKRAZ.ANLASMAADITR, DBIKRAZ.DONUSUMKURU, DBIKRAZ.PARABIRIMI, HRCFIRMABILGISI.VERGINO, HRCFIRMABILGISI.AD, HRCFIRMABILGISI.HESAPNO, KURUSD.DOVIZSATIS, KUREUR.DOVIZSATIS, DBSOZLESME.ACIKLAMA, DBBUTCEREFLOT.LOTNO, DBBUTCEREFLOT.ACIKLAMA, " + HakedisSahipleriSql(detayOku, "DBHAKEDIS.HAKEDISID") + ", " + YollukSql(detayOku, "DBHAKEDIS.HAKEDISID") + ", " + MalzemeSql(detayOku, "DBHAKEDIS.HAKEDISID") + ", " + TarihceSql(detayOku, "DBHAKEDIS.HAKEDISID") + ", " + OdeneklerSql(detayOku, "DBHAKEDIS.HAKEDISID");

            sql += " FROM " + TABLOAD;
            sql += " INNER JOIN DBBUTCEREF ON DBBUTCEREF.BUTCEREFID = DBHAKEDIS.BUTCEREFID";
            sql += " INNER JOIN DBSATINALMAPLANI ON DBSATINALMAPLANI.SATINALMAID = DBBUTCEREF.SATINALMAID";
            sql += " INNER JOIN DBALTBILESEN ON DBALTBILESEN.ALTBILESENID = DBSATINALMAPLANI.ALTBILESENID";
            sql += " INNER JOIN DBBILESEN ON DBBILESEN.BILESENID = DBALTBILESEN.BILESENID";
            sql += " INNER JOIN DBIKRAZ ON DBIKRAZ.IKRAZID = DBBILESEN.IKRAZID";
            sql += " INNER JOIN HRCFIRMABILGISI ON HRCFIRMABILGISI.KOD = DBHAKEDIS.ILGILIKOD";
            sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KURUSD ON KURUSD.KURTARIH = DBHAKEDIS.KURTARIHI AND KURUSD.PARABIRIMI = 'USD'";
            sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KUREUR ON KUREUR.KURTARIH = DBHAKEDIS.KURTARIHI AND KUREUR.PARABIRIMI = 'EUR'";
            sql += " LEFT OUTER JOIN DBSOZLESME ON DBSOZLESME.SOZLESMEID = DBHAKEDIS.SOZLESMEID";
            sql += " LEFT OUTER JOIN DBBUTCEREFLOT ON DBBUTCEREFLOT.BUTCEREFLOTID = DBSOZLESME.BUTCEREFLOTID";

            string kosul = "";

            if (form.hakedisID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HAKEDISID], "=", form.hakedisID));

            if (form.ikrazID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBIKRAZ", "IKRAZID", "=", form.ikrazID));

            if (form.arastirmaTarihiDT > DateTime.MinValue)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETARIHI], "<=", form.arastirmaTarihi));

            if (form.durumKod > 0)
            {
                List<int> l = new List<int>();
                if (form.durumKod < 10) l.Add(form.durumKod);
                else if (form.durumKod == 91) l.AddRange(new int[] { 1, 2 });
                else if (form.durumKod == 92) l.Add(4);
                else if (form.durumKod == 90) l.AddRange(new int[] { 1, 2, 3, 4, 5, 6 });
                if (l.Count == 1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUMKOD], "=", l[0]));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DURUMKOD] + " IN (" + String.Join(",", l) + ")");
            }

            if (form.ilgiliKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIKOD], "=", form.ilgiliKod));

            if (form.altBilesenID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBALTBILESEN", "ALTBILESENID", "=", form.altBilesenID));

            if (!string.IsNullOrEmpty(form.butceKategori))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBSATINALMAPLANI", "BUTCEKATEGORI", "=", form.butceKategori));

            if (form.numaratorAlt > 0 || form.numaratorUst > 0)
            {
                string araKosul = "";
                if (form.numaratorAlt > 0 && form.numaratorUst == 0)
                {
                    araKosul = $"(SI.NUMARATOR={form.numaratorAlt})";
                }
                else
                {
                    araKosul = $"(SI.NUMARATOR>={form.numaratorAlt} AND SI.NUMARATOR<={form.numaratorUst})";
                }
                string icKosul = $"(SELECT 1 FROM DBHAKEDISSAHIBI SI where SI.HAKEDISID=DBHAKEDIS.HAKEDISID AND " + araKosul + ")";

                kosul = VeriErisim.KosulEkle(kosul, "AND", "(1 IN " + icKosul + ")");
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            //sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BELGETARIHI] + " desc";
            if (siralaYontem == 2)
            {
                sql += " ORDER BY DBHAKEDIS.BELGETARIHI DESC";
            }
            else
                sql += " ORDER BY (SELECT MIN(DBHAKEDISSAHIBI.NUMARATOR) FROM DBHAKEDISSAHIBI WHERE DBHAKEDISSAHIBI.HAKEDISID=DBHAKEDIS.HAKEDISID)";
            return veriErisim.DataReaderDondur(sql);
        }

        /********************************/
        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADHAKSAHIBI = "DBHAKEDISSAHIBI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarHAKSAHIBI : int
        {

            /// <summary>
            /// HAKEDISID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HAKEDISID = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// ILGILIKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILIKOD = 2,

            /// <summary>
            /// KESINTITURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KESINTITURKOD = 3,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 4,

            /// <summary>
            /// KESINTITUTARI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KESINTITUTARI = 5,

            /// <summary>
            /// NUMARATOR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NUMARATOR = 6,
            MERKEZNUMARATORID = 7,
            ODENDI = 8,
            ODEMETARIHI = 9
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARHAKSAHIBI = {
            "HAKEDISID"
            , "SIRANO"
            , "ILGILIKOD"
            , "KESINTITURKOD"
            , "ACIKLAMA"
            , "KESINTITUTARI"
            , "NUMARATOR"
            , "MERKEZNUMARATORID"
            , "ODENDI"
            , "ODEMETARIHI"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DBHakedis yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilHakSahibi(DBHakedis form)
        {
            string sql = "DELETE FROM " + TABLOADHAKSAHIBI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.HAKEDISID], "=", form.hakedisID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, dBHakedisSahibi dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBHakedis yapısında nesne</param>
        /// <param name="satirNo">dBHakedisSahibi dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleHakSahibi(DBHakedis form, int satirNo)
        {
            DBHakedisSahibi hakSahip = (DBHakedisSahibi)form.hakedisSahipleri[satirNo];
            string sql = "INSERT INTO " + TABLOADHAKSAHIBI + "(" + VeriErisim.TumAlanlar("", ALANLARHAKSAHIBI) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.hakedisID);
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(hakSahip.ilgiliKod);
            sql += "," + VeriErisim.DegerYap((int)hakSahip.kesintiTurKod);
            sql += "," + VeriErisim.DegerYap(hakSahip.aciklama);
            sql += "," + VeriErisim.DegerYap(hakSahip.kesintiTutari);
            sql += "," + VeriErisim.DegerYap(hakSahip.numarator);
            sql += "," + (hakSahip.merkezNumaratorID == 0 ? "NULL" : VeriErisim.DegerYap(hakSahip.merkezNumaratorID));
            sql += "," + VeriErisim.DegerYap(hakSahip.odendi);
            sql += "," + VeriErisim.DegerYap(hakSahip.odemeTarihi) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleHakSahibi(DBHakedisSahibi hakSahip)
        {
            string sql = "UPDATE " + TABLOADHAKSAHIBI + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.ILGILIKOD], hakSahip.ilgiliKod);
            sql += "," + VeriErisim.AtamaOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.KESINTITURKOD], (int)hakSahip.kesintiTurKod);
            sql += "," + VeriErisim.AtamaOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.ACIKLAMA], hakSahip.aciklama);
            sql += "," + VeriErisim.AtamaOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.KESINTITUTARI], hakSahip.kesintiTutari);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.HAKEDISID], "=", hakSahip.hakedisID));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.SIRANO], "=", hakSahip.siraNo));
            return KomutCalistir(sql, 1);
        }

        public Sonuc HakSahibiNumaraSifirla(int hakedisID, int siraNo)
        {
            string sql = "UPDATE " + TABLOADHAKSAHIBI + " SET ";
            sql += ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.NUMARATOR] + " = 0, ";
            sql += ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.MERKEZNUMARATORID] + " = NULL";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.HAKEDISID], "=", hakedisID));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.SIRANO], "=", siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.MERKEZNUMARATORID] + " IS NOT NULL");
            sql = VeriErisim.KosulEkle(sql, "AND", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.ODENDI] + " = 0");
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DBHakedis yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleHakSahibi(DBHakedisSahibi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADHAKSAHIBI, ALANLARHAKSAHIBI) + ", HRCFIRMABILGISI.VERGINO, HRCFIRMABILGISI.AD, HRCFIRMABILGISI.HESAPNO, KURUSD.DOVIZALIS, KUREUR.DOVIZALIS FROM " + TABLOADHAKSAHIBI;
            sql += " LEFT OUTER JOIN HRCFIRMABILGISI ON HRCFIRMABILGISI.KOD=DBHAKEDISSAHIBI.ILGILIKOD ";
            sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KURUSD ON KURUSD.KURTARIH = DBHAKEDISSAHIBI.ODEMETARIHI AND KURUSD.PARABIRIMI = 'USD'";
            sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KUREUR ON KUREUR.KURTARIH = DBHAKEDISSAHIBI.ODEMETARIHI AND KUREUR.PARABIRIMI = 'EUR'";

            if (form.hakedisID > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.HAKEDISID], "=", form.hakedisID));
            else
            {
                string kosul = "";
                if (form.merkezNumaratorID > 0 || form.merkezNumaratorID == -1)
                {
                    if (form.merkezNumaratorID == -1)
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.MERKEZNUMARATORID] + " IS NULL");
                        kosul = VeriErisim.KosulEkle(kosul, "AND", "COALESCE(" + ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.NUMARATOR] + ", 0) > 0");
                    }
                    else
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.MERKEZNUMARATORID], "=", form.merkezNumaratorID));
                }
                if (form.altBilesenID > 0)
                {
                    string altBilesenKosul = $@"1 IN (SELECT 1 FROM DBHAKEDISSAHIBI IC
                        INNER JOIN DBHAKEDIS ON DBHAKEDIS.HAKEDISID=IC.HAKEDISID
                        INNER JOIN DBBUTCEREF ON DBBUTCEREF.BUTCEREFID=DBHAKEDIS.BUTCEREFID
                        INNER JOIN DBSATINALMAPLANI ON DBSATINALMAPLANI.SATINALMAID = DBBUTCEREF.SATINALMAID
                        WHERE IC.HAKEDISID=DBHAKEDIS.HAKEDISID AND DBSATINALMAPLANI.ALTBILESENID={form.altBilesenID})";
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + altBilesenKosul + ")");
                }
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            }
            sql += " ORDER BY " + ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.HAKEDISID] + ", " + ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.KESINTITURKOD] + ", " + ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.SIRANO];
            return veriErisim.DataReaderDondur(sql);
        }


        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DBHakedis yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleHakSahibiNumarator(DBHakedisSahibi form)
        {
            string alanlar = "DBHAKEDISSAHIBI.HAKEDISID, NUMARATOR, MERKEZNUMARATORID, DBHAKEDISSAHIBI.ILGILIKOD, HRCFIRMABILGISI.VERGINO, HRCFIRMABILGISI.AD, HRCFIRMABILGISI.HESAPNO, HRCFIRMABILGISI.BANKA, DBHAKEDIS.GEREKCE, DBHAKEDIS.PARABIRIMI, DBSATINALMAPLANI.BUTCEKATEGORI, KURUSD.DOVIZSATIS, KUREUR.DOVIZSATIS";
            string sql = $"SELECT {alanlar}, SUM(KESINTITUTARI), MIN(KESINTITURKOD) FROM " + TABLOADHAKSAHIBI;
            sql += " INNER JOIN DBHAKEDIS ON DBHAKEDIS.HAKEDISID = DBHAKEDISSAHIBI.HAKEDISID ";
            sql += " INNER JOIN DBBUTCEREF ON DBBUTCEREF.BUTCEREFID = DBHAKEDIS.BUTCEREFID ";
            sql += " INNER JOIN DBSATINALMAPLANI ON DBSATINALMAPLANI.SATINALMAID = DBBUTCEREF.SATINALMAID ";
            sql += " INNER JOIN HRCFIRMABILGISI ON HRCFIRMABILGISI.KOD=DBHAKEDISSAHIBI.ILGILIKOD ";
            sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KURUSD ON KURUSD.KURTARIH = DBHAKEDIS.KURTARIHI AND KURUSD.PARABIRIMI = 'USD'";
            sql += " LEFT OUTER JOIN HRCMERKEZBANKKUR KUREUR ON KUREUR.KURTARIH = DBHAKEDIS.KURTARIHI AND KUREUR.PARABIRIMI = 'EUR'";
            string kosul = "";
            if (form.merkezNumaratorID > 0 || form.merkezNumaratorID == -1)
            {
                if (form.merkezNumaratorID == -1)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", ALANLAR[(int)ENUMAlanlar.DURUMKOD] + " <= 5");
                    kosul = VeriErisim.KosulEkle(kosul, "AND", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.MERKEZNUMARATORID] + " IS NULL");
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "COALESCE(" + ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.NUMARATOR] + ", 0) > 0");
                }
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.MERKEZNUMARATORID], "=", form.merkezNumaratorID));
            }
            if (form.altBilesenID > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("DBSATINALMAPLANI", "ALTBILESENID", "=", form.altBilesenID));
            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            sql += "GROUP BY " + alanlar;
            sql += " ORDER BY " + ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.HAKEDISID] + ", " + ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.NUMARATOR];
            return veriErisim.DataReaderDondur(sql);
        }

        /******************************************************************/

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADMALZEME = "DBHAKEDISMALZEME";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarMALZEME : int
        {

            /// <summary>
            /// HAKEDISID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HAKEDISID = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// MALZEMEADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MALZEMEADI = 2,

            /// <summary>
            /// MIKTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 3,

            /// <summary>
            /// BIRIMFIYAT alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMALZEME = {
            "HAKEDISID"
            , "SIRANO"
            , "MALZEMEADI"
            , "MIKTAR"
            , "BIRIMFIYAT"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DBHakedis yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilMalzeme(DBHakedis form)
        {
            string sql = "DELETE FROM " + TABLOADMALZEME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMALZEME.HAKEDISID], "=", form.hakedisID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, dBHakedisMalzeme dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBHakedis yapısında nesne</param>
        /// <param name="satirNo">dBHakedisMalzeme dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleMalzeme(DBHakedis form, int satirNo)
        {
            DBHakedisMalzeme mal = (DBHakedisMalzeme)form.malzemeListesi[satirNo];
            string sql = "INSERT INTO " + TABLOADMALZEME + "(" + VeriErisim.TumAlanlar("", ALANLARMALZEME) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.hakedisID);
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(mal.malzemeAdi);
            sql += "," + VeriErisim.DegerYap(mal.miktar);
            sql += "," + VeriErisim.DegerYap(mal.birimFiyat);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /******************************************************************/


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDBYOLLUK = "DBYOLLUKBILGISI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDBYOLLUK : int
        {

            /// <summary>
            /// HAKEDISID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HAKEDISID = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// YER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YER = 2,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 3,

            /// <summary>
            /// TUTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTAR = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARYOLLUK = {
            "HAKEDISID"
            , "SIRANO"
            , "YER"
            , "ACIKLAMA"
            , "TUTAR"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DBHakedis yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilYolluk(DBHakedis form)
        {
            string sql = "DELETE FROM " + TABLOADDBYOLLUK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYOLLUK[(int)ENUMAlanlarDBYOLLUK.HAKEDISID], "=", form.hakedisID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, dBYollukBilgisi dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBHakedis yapısında nesne</param>
        /// <param name="satirNo">dBYollukBilgisi dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleYolluk(DBHakedis form, int satirNo)
        {
            DBYollukBilgisi yb = (DBYollukBilgisi)form.yollukBilgileri[satirNo];

            string sql = "INSERT INTO " + TABLOADDBYOLLUK + "(" + VeriErisim.TumAlanlar("", ALANLARYOLLUK) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.hakedisID);
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(yb.yer);
            sql += "," + VeriErisim.DegerYap(yb.aciklama);
            sql += "," + VeriErisim.DegerYap(yb.tutar);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DBHakedis yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        //public IDataReader ListeleYolluk(DBHakedis form)
        //{
        //    string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDBYOLLUK, ALANLARYOLLUK) + " FROM " + TABLOADDBYOLLUK;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYOLLUK[(int)ENUMAlanlarDBYOLLUK.HAKEDISID], "=", form.hakedisID));
        //    sql += " ORDER BY " + ALANLARYOLLUK[(int)ENUMAlanlarDBYOLLUK.SIRANO];

        //    return veriErisim.DataReaderDondur(sql);
        //}

        public Sonuc DurumAta(int hakedisID, int yeniDurumKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUMKOD], yeniDurumKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HAKEDISID], "=", hakedisID));
            return KomutCalistir(sql, 1);
        }

        public Sonuc NumaratorTarihiAta(int hakedisID)
        {
            string sql = "UPDATE " + TABLOAD + " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NUMARATORALMATARIHI], new TNSDateTime(DateTime.Now.Date));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HAKEDISID], "=", hakedisID));
            return KomutCalistir(sql, 1);
        }

        public Sonuc NumaraAta(int ikrazID, int hakedisID)
        {
            string sql = $@"
WITH EnSonNumara AS
(
    SELECT COALESCE(MAX(dhs.NUMARATOR), 0) AS MaxNumarator
    FROM DBHAKEDISSAHIBI dhs
    INNER JOIN DBHAKEDIS dh           ON dh.HAKEDISID = dhs.HAKEDISID
    INNER JOIN DBBUTCEREF br          ON br.BUTCEREFID = dh.BUTCEREFID
    INNER JOIN DBSATINALMAPLANI sp    ON sp.SATINALMAID = br.SATINALMAID
    INNER JOIN DBALTBILESEN ab        ON ab.ALTBILESENID = sp.ALTBILESENID
    INNER JOIN DBBILESEN b            ON b.BILESENID = ab.BILESENID
    WHERE b.IKRAZID = {VeriErisim.DegerYap(ikrazID)}
),
NumaraBelirleme AS
(
    SELECT 
        dhs.HAKEDISID,
        dhs.ILGILIKOD,
        MIN(dhs.KESINTITURKOD) AS MinKesintiTur,
        DENSE_RANK() OVER 
        (
            ORDER BY MIN(dhs.KESINTITURKOD), dhs.ILGILIKOD
        ) AS YeniNumara
    FROM DBHAKEDISSAHIBI dhs
    WHERE dhs.HAKEDISID = {VeriErisim.DegerYap(hakedisID)}
      AND COALESCE(dhs.NUMARATOR, 0) = 0
    GROUP BY dhs.HAKEDISID, dhs.ILGILIKOD
)
UPDATE dhs
SET dhs.NUMARATOR = en.MaxNumarator + nb.YeniNumara
FROM DBHAKEDISSAHIBI dhs
INNER JOIN NumaraBelirleme nb
    ON dhs.HAKEDISID = nb.HAKEDISID
   AND dhs.ILGILIKOD = nb.ILGILIKOD
CROSS JOIN EnSonNumara en
WHERE dhs.HAKEDISID = {VeriErisim.DegerYap(hakedisID)}
  AND COALESCE(dhs.NUMARATOR, 0) = 0;
";
            return KomutCalistir(sql, 0);



        }

        public Sonuc MerkezNumaratorUret(string kullaniciKodu, int altBilesenID)
        {
            string sql = $"INSERT INTO DBMERKEZNUMARATOR(ALTBILESENID, NUMARATOR, ISLEMTARIHI, ISLEMIYAPAN, DURUM) OUTPUT INSERTED.MERKEZNUMARATORID SELECT {VeriErisim.DegerYap(altBilesenID)}, COALESCE(MAX(NUMARATOR), 0)+1, SYSDATETIME(), {VeriErisim.DegerYap(kullaniciKodu)}, 1 FROM DBMERKEZNUMARATOR WHERE ALTBILESENID={VeriErisim.DegerYap(altBilesenID)}";
            int anahtar = veriErisim.TekKolonSQLInt(sql);
            Sonuc s = new Sonuc();
            if (!string.IsNullOrWhiteSpace(veriErisim.hata))
            {
                s.DegerAta(veriErisim.hata);
            }
            else
                s.anahtar = anahtar.ToString();
            return s;
        }

        public IDataReader MerkezNumaratorListele(DBMerkezBankasiNumarator dbMerkezBankasiNumarator)
        {
            string sql = "SELECT IKRAZNO, PARABIRIMI, DONUSUMKURU, MBHESAPNO, ILGILIBIRIMBUTCEKODU, KURAD, ILGILIBIRIMNO, ";
            sql += "MERKEZNUMARATORID, DBMERKEZNUMARATOR.ALTBILESENID, NUMARATOR, ISLEMTARIHI, ISLEMIYAPAN, DURUM, IMZASOL.IMZAID, IMZASOL.ADSOYAD, IMZASOL.SATIR1, IMZASOL.SATIR2, IMZASAG.IMZAID, IMZASAG.ADSOYAD, IMZASAG.SATIR1, IMZASAG.SATIR2 ";
            sql += "FROM DBMERKEZNUMARATOR INNER JOIN DBALTBILESEN ON DBALTBILESEN.ALTBILESENID = DBMERKEZNUMARATOR.ALTBILESENID ";
            sql += "INNER JOIN DBBILESEN ON DBBILESEN.BILESENID = DBALTBILESEN.BILESENID  ";
            sql += "INNER JOIN DBIKRAZ ON DBIKRAZ.IKRAZID = DBBILESEN.IKRAZID ";


            sql += "INNER JOIN ButceHazirlik.dbo.ABSKUR KUR ON KUR.YIL=" + DateTime.Now.Year + " AND KUR.KURKOD=DBALTBILESEN.ILGILIBIRIMBUTCEKODU ";
            sql += "LEFT OUTER JOIN DBIMZA IMZASOL ON IMZASOL.IMZAID = DBMERKEZNUMARATOR.IMZASOLID ";
            sql += "LEFT OUTER JOIN DBIMZA IMZASAG ON IMZASAG.IMZAID = DBMERKEZNUMARATOR.IMZASAGID ";


            if (dbMerkezBankasiNumarator.merkezNumaratorID > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MERKEZNUMARATORID", "=", dbMerkezBankasiNumarator.merkezNumaratorID));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "DBMERKEZNUMARATOR.ALTBILESENID", "=", dbMerkezBankasiNumarator.altBilesenID));
            sql += " ORDER BY NUMARATOR";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc MerkezNumaratoreEkle(string kullaniciKodu, int hakedisID, int numarator, int merkezNumaratorID)
        {
            string sql = "UPDATE " + TABLOADHAKSAHIBI;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.MERKEZNUMARATORID], merkezNumaratorID);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.HAKEDISID], "=", hakedisID));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.NUMARATOR], "=", numarator));
            sql = VeriErisim.KosulEkle(sql, "AND", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.MERKEZNUMARATORID] + " IS NULL");
            return KomutCalistir(sql, 0);
        }

        public Sonuc MerkezNumaratordenCikart(string kullaniciKodu, int hakedisID, int numarator, int merkezNumaratorID)
        {
            string sql = "UPDATE " + TABLOADHAKSAHIBI;
            sql += " SET " + ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.MERKEZNUMARATORID] + " = NULL";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.HAKEDISID], "=", hakedisID));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.NUMARATOR], "=", numarator));
            sql = VeriErisim.KosulEkle(sql, "AND", ALANLARHAKSAHIBI[(int)ENUMAlanlarHAKSAHIBI.MERKEZNUMARATORID] + " IS NOT NULL");
            return KomutCalistir(sql, 0);
        }


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADIMZA = "DBIMZA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarIMZA : int
        {

            /// <summary>
            /// IMZAID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IMZAID = 0,

            /// <summary>
            /// ALTBILESENID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTBILESENID = 1,

            /// <summary>
            /// ADSOYAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ADSOYAD = 2,

            /// <summary>
            /// SATIR1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATIR1 = 3,

            /// <summary>
            /// SATIR2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATIR2 = 4,

            /// <summary>
            /// PASIF alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PASIF = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARIMZA = {
            "IMZAID"
            , "ALTBILESENID"
            , "ADSOYAD"
            , "SATIR1"
            , "SATIR2"
            , "PASIF"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DBImza yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilImza(DBImza form)
        {
            string sql = "DELETE FROM " + TABLOADIMZA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIMZA[(int)ENUMAlanlarIMZA.IMZAID], "=", form.imzaID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBImza yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleImza(DBImza form)
        {
            string sql = "INSERT INTO " + TABLOADIMZA + "(" + VeriErisim.TumAlanlarEksik("", ALANLARIMZA, new int[] { 0 }) + ") OUTPUT INSERTED.IMZAID VALUES";
            sql += "(" + VeriErisim.DegerYap(form.altBilesenID);
            sql += "," + VeriErisim.DegerYap(form.adSoyad);
            sql += "," + VeriErisim.DegerYap(form.satir1);
            sql += "," + VeriErisim.DegerYap(form.satir2);
            sql += "," + VeriErisim.DegerYap(form.pasif ? 1 : 0);
            sql += ")";
            int anahtar = veriErisim.TekKolonSQLInt(sql);
            Sonuc s = new Sonuc();
            if (!string.IsNullOrWhiteSpace(veriErisim.hata))
            {
                s.DegerAta(veriErisim.hata);
            }
            else
                s.anahtar = anahtar.ToString();
            return s;
        }

        public Sonuc GuncelleImza(DBImza form)
        {
            string sql = "UPDATE " + TABLOADIMZA;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARIMZA[(int)ENUMAlanlarIMZA.ALTBILESENID], form.altBilesenID);
            sql += "," + VeriErisim.AtamaOl("", ALANLARIMZA[(int)ENUMAlanlarIMZA.ADSOYAD], form.adSoyad);
            sql += "," + VeriErisim.AtamaOl("", ALANLARIMZA[(int)ENUMAlanlarIMZA.SATIR1], form.satir1);
            sql += "," + VeriErisim.AtamaOl("", ALANLARIMZA[(int)ENUMAlanlarIMZA.SATIR2], form.satir2);
            sql += "," + VeriErisim.AtamaOl("", ALANLARIMZA[(int)ENUMAlanlarIMZA.PASIF], form.pasif ? 1 : 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIMZA[(int)ENUMAlanlarIMZA.IMZAID], "=", form.imzaID));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DBImza yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleImza(DBImza form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADIMZA, ALANLARIMZA) + " FROM " + TABLOADIMZA;
            if (form.imzaID > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIMZA[(int)ENUMAlanlarIMZA.IMZAID], "=", form.imzaID));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIMZA[(int)ENUMAlanlarIMZA.ALTBILESENID], "=", form.altBilesenID));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc MerkezNumarasiDurumDegistir(DBMerkezBankasiNumarator form)
        {
            string sql = $"UPDATE DBMERKEZNUMARATOR SET DURUM={form.durum} WHERE MERKEZNUMARATORID={form.merkezNumaratorID}";
            return KomutCalistir(sql, 1);
        }

        public Sonuc MerkezNumarasiImzaDegistir(DBMerkezBankasiNumarator form)
        {
            string sql = $"UPDATE DBMERKEZNUMARATOR SET IMZASOLID={form.imzaSol.imzaID}, IMZASAGID={form.imzaSag.imzaID} WHERE MERKEZNUMARATORID={form.merkezNumaratorID}";
            return KomutCalistir(sql, 1);
        }

        public Sonuc EkleHakedisOdenek(DBHakedis form, int siraNo)
        {
            DBHakedisOdenek o = (DBHakedisOdenek)form.odenekListesi[siraNo];
            string sql = $@"INSERT INTO {TABLOADODENEKLER} ({VeriErisim.TumAlanlar("", ALANLARODENEKLER)}) VALUES (
                    {VeriErisim.DegerYap(form.hakedisID)}, {VeriErisim.DegerYap(o.ekoKod.Replace(".", ""))}, {VeriErisim.DegerYap(o.miktar)})";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilHakedisOdenek(DBHakedis form)
        {
            string sql = $@"DELETE {TABLOADODENEKLER}  WHERE HAKEDISID = {VeriErisim.DegerYap(form.hakedisID)}";
            return KomutCalistir(sql, 0);
        }
        public Sonuc SilHakedisOdenek(int hakedisID)
        {
            string sql = $@"
                DELETE FROM DBHAKEDISODENEKLER
                WHERE HAKEDISID = {VeriErisim.DegerYap(hakedisID)}
            ";
            return KomutCalistir(sql, 0);
        }
        public Sonuc SilYollukBilgisi(int hakedisID)
        {
            string sql = $@"
                DELETE FROM DBYOLLUKBILGISI
                WHERE HAKEDISID = {VeriErisim.DegerYap(hakedisID)}
            ";
            return KomutCalistir(sql, 0);
        }
        public Sonuc SilHakedisMalzeme(int hakedisID)
        {
            string sql = $@"
                DELETE FROM DBHAKEDISMALZEME
                WHERE HAKEDISID = {VeriErisim.DegerYap(hakedisID)}
            ";
            return KomutCalistir(sql, 0);
        }
        public Sonuc SilHakedis(int hakedisID)
        {
            string sql = $@"
                DELETE FROM DBHAKEDIS
                WHERE HAKEDISID = {VeriErisim.DegerYap(hakedisID)}
            ";
            return KomutCalistir(sql, 1);
        }
    }
}
