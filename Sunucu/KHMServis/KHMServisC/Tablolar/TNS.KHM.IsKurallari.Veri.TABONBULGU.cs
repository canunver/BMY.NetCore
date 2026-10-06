using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KHM;
namespace TNS.KHM.IsKurallari.Veri
{
    /// <summary>
    /// KHMONBULGU veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKHMONBULGU : TABOrtak
    {
        /// <summary>
        /// TABKHMONBULGU sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKHMONBULGU(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "SBONBULGU";


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADBIRIM = "SBONBULGUBIRIM";





        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// BULGUID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BULGUID = 0,

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 1,

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BITISYIL = 2,

            /// <summary>
            /// BUTCETURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUTCETURKOD = 3,

            /// <summary>
            /// BULGUTURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BULGUTURKOD = 4,

            /// <summary>
            /// BULGUNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BULGUNO = 5,

            /// <summary>
            /// BULGU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BULGU = 6,

            /// <summary>
            /// BULGUOZETI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BULGUOZETI = 7,

            /// <summary>
            /// BULGUDAYANAGI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BULGUDAYANAGI = 8,

            /// <summary>
            /// IDARECEVABI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IDARECEVABI = 9,

            /// <summary>
            /// IDARECEVAPOZETI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IDARECEVAPOZETI = 10,

            /// <summary>
            /// SAYISTAYONERISI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYISTAYONERISI = 11,

            /// <summary>
            /// DURUMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUMKOD = 12,

            /// <summary>
            /// YERINEGETIRILDI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YERINEGETIRILDI = 13,

            /// <summary>
            /// YERINEGETIRILDI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAPILACAKFAALIYETLER = 14,

            /// <summary>
            /// ILGILIBIRIMKURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILIBIRIMKURUM = 15,

            /// <summary>
            /// GOREVORTAGI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVORTAGI = 16,

            /// <summary>
            /// ONGORULENTAMAMLANMATARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONGORULENTAMAMLANMATARIHI = 17,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "BULGUID"
            , "YIL"
            , "BITISYIL"
            , "BUTCETURKOD"
            , "BULGUTURKOD"
            , "ONBULGUNO"
            , "BULGU"
            , "BULGUOZETI"
            , "BULGUDAYANAGI"
            , "IDARECEVABI"
            , "IDARECEVAPOZETI"
            , "SAYISTAYONERISI"
            , "DURUMKOD"
            , "YERINEGETIRILDI"
            , "YAPILACAKFAALIYETLER"
            , "ILGILIBIRIMKURUM"
            , "GOREVORTAGI"
            , "ONGORULENTAMAMLANMATARIHI"
        };



        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarBIRIM : int
        {
            /// <summary>
            /// BULGUID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BULGUID = 0,

            /// <summary>
            /// MUHASEBEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,

            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBIRIM = {
            "BULGUID"
            , "MUHASEBEKOD"
            , "BIRIMKOD"
            , "KURKOD"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, OnBulgu yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(SayBulgu form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BULGUID], "=", form.bulguID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bulguID">Bulgu kodu</param>
        /// <param name="birim">Muhasebe birim bilgisi tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilBirim(int bulguID, TNS.MUH.MuhasebeBirim birim)
        {
            string sql = "DELETE FROM " + TABLOADBIRIM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBIRIM.BULGUID], "=", bulguID));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBIRIM.MUHASEBEKOD], "=", birim.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBIRIM.BIRIMKOD], "=", birim.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBIRIM[(int)ENUMAlanlarBIRIM.KURKOD], "=", birim.kurum.Replace(".", "")));
            return KomutCalistir(sql, 1);
        }


        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, OnBulgu yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(SayBulgu form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + ") OUTPUT INSERTED.";
            sql += ALANLAR[(int)ENUMAlanlar.BULGUID] + "  VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + (form.bitisYil == 0 ? "NULL" : VeriErisim.DegerYap((int)form.bitisYil));
            sql += "," + VeriErisim.DegerYap((int)form.butceTurKod);
            sql += "," + VeriErisim.DegerYap((int)form.bulguTurKod);
            sql += "," + VeriErisim.DegerYap(form.bulguNo);
            sql += "," + VeriErisim.DegerYap(form.bulguBasligi);
            sql += "," + VeriErisim.DegerYap(form.bulguOzeti);
            sql += "," + VeriErisim.DegerYap(form.bulguDayanagi);
            sql += "," + VeriErisim.DegerYap(form.idareCevabi);
            sql += "," + VeriErisim.DegerYap(form.idareCevapOzeti);
            sql += "," + VeriErisim.DegerYap(form.sayistayOnerisi);
            sql += "," + VeriErisim.DegerYap((int)form.durumKod);

            sql += "," + VeriErisim.DegerYap(form.yerineGetirildi ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(form.yapilacakFaaliyetler);
            sql += "," + VeriErisim.DegerYap(form.ilgiliBirimKurum);
            sql += "," + VeriErisim.DegerYap(form.gorevOrtagi);
            sql += "," + VeriErisim.DegerYap(form.ongorulenTamamlanmaTarihi);

            sql += ")";
            int uretID = veriErisim.TekKolonSQLInt(sql);
            Sonuc s = new Sonuc();
            s.anahtar = uretID.ToString();
            if (!string.IsNullOrWhiteSpace(this.veriErisim.hata))
                s.DegerAta("Veri tabanı hatası:" + this.veriErisim.hata);
            return s;
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bulguID">Bulgu kodu</param>
        /// <param name="birim">Muhasebe birim bilgisi tutan nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleBirim(int bulguID, TNS.MUH.MuhasebeBirim birim)
        {
            string sql = "INSERT INTO " + TABLOADBIRIM + "(" + VeriErisim.TumAlanlar("", ALANLARBIRIM) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(bulguID);
            sql += "," + VeriErisim.DegerYap(birim.muhasebe);
            sql += "," + VeriErisim.DegerYap(birim.birimKod);
            sql += "," + VeriErisim.DegerYap(birim.kurum.Replace(".", ""));
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, OnBulgu yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(SayBulgu form)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], form.yil);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISYIL], (int)form.bitisYil);
            sql += ", " + $"{ALANLAR[(int)ENUMAlanlar.BUTCETURKOD]} = {(form.butceTurKod == 0 ? "NULL" : ((int)form.butceTurKod).ToString())}";
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BULGUTURKOD], (int)form.bulguTurKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BULGUNO], form.bulguNo);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BULGU], form.bulguBasligi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BULGUOZETI], form.bulguOzeti);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BULGUDAYANAGI], form.bulguDayanagi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IDARECEVABI], form.idareCevabi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IDARECEVAPOZETI], form.idareCevapOzeti);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYISTAYONERISI], form.sayistayOnerisi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUMKOD], (int)form.durumKod);

            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YERINEGETIRILDI], form.yerineGetirildi ? 1 : 0);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAPILACAKFAALIYETLER], form.yapilacakFaaliyetler);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILGILIBIRIMKURUM], form.ilgiliBirimKurum);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GOREVORTAGI], form.gorevOrtagi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONGORULENTAMAMLANMATARIHI], form.ongorulenTamamlanmaTarihi);

            sql += " WHERE  " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BULGUID], (int)form.bulguID);
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, OnBulgu yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(SayBulgu form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = "";
            if (form.bulguID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BULGUID], "=", form.bulguID));
            else
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));

                if (form.durumKod == enumBulguDurum.AcikBulgular)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "DURUMKOD <= 5");
                }
                else if (form.durumKod == enumBulguDurum.KapaliBulgular)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(DURUMKOD >= 6 AND DURUMKOD <= 8)");
                }
                else if (form.durumKod == enumBulguDurum.IptalHaric)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(DURUMKOD != 9)");
                }
                else if ((int)form.durumKod > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", $"(DURUMKOD = {(int)form.durumKod})");
                }
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            sql += $" order by {ALANLAR[(int)ENUMAlanlar.BULGUTURKOD]}, {ALANLAR[(int)ENUMAlanlar.YIL]}, {ALANLAR[(int)ENUMAlanlar.BULGUNO]}";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bulguID">Select edilecek kayıtların bulgu kodu</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleBirim(int bulguID)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBIRIM, ALANLARBIRIM) + ", M.MUHASEBEAD, B.BIRIMAD FROM " + TABLOADBIRIM;
            sql += "  INNER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM B ON B.MUHASEBEKOD = SBONBULGUBIRIM.MUHASEBEKOD AND B.BIRIMKOD = SBONBULGUBIRIM.BIRIMKOD AND B.KURKOD = SBONBULGUBIRIM.KURKOD";
            sql += "  INNER JOIN ButceIslemleri.dbo.MUHMUHASEBE M ON M.MUHASEBEKOD = B.MUHASEBEKOD";

            string kosul = "";
            if (bulguID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBIRIM, ALANLARBIRIM[(int)ENUMAlanlarBIRIM.BULGUID], "=", bulguID));
            else
            {
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleOnBulguCevap(int yil, int muhasebe, string birimKod, string kurum)
        {
            string sql = @"
               WITH R AS (
                 SELECT BIRIMKOD, MUHASEBEKOD, KURKOD, a.BULGUID, g.YIL, g.BUTCETURKOD, g.BULGUTURKOD, g.ONBULGUNO, g.BULGU, 
                        g.BULGUOZETI, g.BULGUDAYANAGI, g.IDARECEVABI, g.IDARECEVAPOZETI, g.SAYISTAYONERISI, g.DURUMKOD 
                 FROM SBONBULGUBIRIM a
                 INNER JOIN SBONBULGU AS g ON 
                    g.BULGUID = a.BULGUID AND g.YIL = @YIL AND g.DURUMKOD < 5
                 WHERE BIRIMKOD = @BIRIMKOD AND MUHASEBEKOD = @MUHASEBEKOD AND KURKOD = @KURKOD),
              C AS (
                 SELECT  BIRIMKOD, MUHASEBEKOD, KURKOD, BULGUID, CEVAP, BIRIMISLER, YERINEGETIRILDI, ONGORULENTAMAMLANMATARIHI 
                 FROM SBONBULGUBIRIMCEVAP
                 WHERE BIRIMKOD = @BIRIMKOD AND MUHASEBEKOD = @MUHASEBEKOD AND KURKOD = @KURKOD AND YIL=@YIL)

            SELECT r.BULGUID, r.YIL, r.BUTCETURKOD, r.BULGUTURKOD, r.ONBULGUNO, r.BULGU, r.BULGUOZETI, r.BULGUDAYANAGI,
                   r.IDARECEVABI, r.IDARECEVAPOZETI, r.SAYISTAYONERISI, r.DURUMKOD, r.BIRIMKOD, r.MUHASEBEKOD, r.KURKOD, c.CEVAP, c.BIRIMISLER, c.YERINEGETIRILDI, c.ONGORULENTAMAMLANMATARIHI
            FROM R FULL OUTER JOIN C ON 
                r.BULGUID = c.BULGUID AND r.BIRIMKOD = c.BIRIMKOD AND r.MUHASEBEKOD = c.MUHASEBEKOD AND r.KURKOD = c.KURKOD 
            ORDER BY r.YIL, r.ONBULGUNO, r.BULGUID";

            veriErisim.param.Add(new VeriPara("@YIL", DbType.Int64, yil));
            veriErisim.param.Add(new VeriPara("@BIRIMKOD", DbType.String, birimKod));
            veriErisim.param.Add(new VeriPara("@MUHASEBEKOD", DbType.Int64, muhasebe));
            veriErisim.param.Add(new VeriPara("@KURKOD", DbType.String, kurum.Replace(".", "")));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleOnBulguSadeceCevap(int yil)
        {
            string sql = @"SELECT C.BULGUID, C.YIL, C.CEVAP, C.BIRIMISLER, c.YERINEGETIRILDI, c.ONGORULENTAMAMLANMATARIHI, C.BIRIMKOD, C.MUHASEBEKOD, C.KURKOD, M.MUHASEBEAD, B.BIRIMAD  
                 FROM SBONBULGUBIRIMCEVAP C 
                 INNER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM B 
                    ON B.MUHASEBEKOD = C.MUHASEBEKOD AND B.BIRIMKOD = C.BIRIMKOD AND B.KURKOD = C.KURKOD
                 INNER JOIN ButceIslemleri.dbo.MUHMUHASEBE M 
                    ON M.MUHASEBEKOD = B.MUHASEBEKOD
                 WHERE YIL=@YIL";

            veriErisim.param.Add(new VeriPara("@YIL", DbType.Int64, yil));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc CevapKaydet(SayBulgu onBulgu)
        {
            string sql = @"MERGE SBONBULGUBIRIMCEVAP AS TARGET
                            USING (SELECT 
                               @BULGUID      AS BULGUID,
                               @BIRIMKOD     AS BIRIMKOD,
                               @MUHASEBEKOD  AS MUHASEBEKOD,
                               @KURKOD       AS KURKOD,
                               @YIL          AS YIL,
                               @CEVAP        AS CEVAP,
                               @BIRIMISLER   AS BIRIMISLER,
                               @YERINEGETIRILDI AS YERINEGETIRILDI, 
                               @ONGORULENTAMAMLANMATARIHI AS ONGORULENTAMAMLANMATARIHI) AS SOURCE ON (
                            TARGET.BULGUID = SOURCE.BULGUID 
                                AND TARGET.BIRIMKOD = SOURCE.BIRIMKOD
                                AND TARGET.MUHASEBEKOD = SOURCE.MUHASEBEKOD
                                AND TARGET.KURKOD = SOURCE.KURKOD
                                AND TARGET.YIL = SOURCE.YIL)
                            WHEN MATCHED THEN
                                UPDATE SET TARGET.CEVAP = SOURCE.CEVAP, TARGET.BIRIMISLER = SOURCE.BIRIMISLER, TARGET.YERINEGETIRILDI = SOURCE.YERINEGETIRILDI, TARGET.ONGORULENTAMAMLANMATARIHI = SOURCE.ONGORULENTAMAMLANMATARIHI
                            WHEN NOT MATCHED THEN
                                INSERT (BULGUID, BIRIMKOD, MUHASEBEKOD, KURKOD, YIL, CEVAP, BIRIMISLER, YERINEGETIRILDI, ONGORULENTAMAMLANMATARIHI)
                                VALUES (SOURCE.BULGUID, SOURCE.BIRIMKOD, SOURCE.MUHASEBEKOD, SOURCE.KURKOD, SOURCE.YIL, SOURCE.CEVAP, SOURCE.BIRIMISLER, SOURCE.YERINEGETIRILDI, SOURCE.ONGORULENTAMAMLANMATARIHI);";

            veriErisim.param.Add(new VeriPara("@BULGUID", DbType.Int64, onBulgu.bulguID));
            veriErisim.param.Add(new VeriPara("@BIRIMKOD", DbType.String, onBulgu.birimKod));
            veriErisim.param.Add(new VeriPara("@MUHASEBEKOD", DbType.Int64, onBulgu.muhasebeKod));
            veriErisim.param.Add(new VeriPara("@KURKOD", DbType.String, onBulgu.kurKod.Replace(".", "")));
            veriErisim.param.Add(new VeriPara("@YIL", DbType.Int64, onBulgu.yil));
            veriErisim.param.Add(new VeriPara("@CEVAP", DbType.String, onBulgu.birimCevap));
            veriErisim.param.Add(new VeriPara("@BIRIMISLER", DbType.String, onBulgu.birimIsler));
            veriErisim.param.Add(new VeriPara("@YERINEGETIRILDI", DbType.String, onBulgu.yerineGetirildiCevap ? 1 : 0));
            veriErisim.param.Add(new VeriPara("@ONGORULENTAMAMLANMATARIHI", DbType.Date, onBulgu.ongorulenTamamlanmaTarihiCevapDT.Date));

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleBirimVeriGirisDurum(int yil, int tur, int muhasebeKod, string birimKod, string kurKod)
        {
            string ekBirim;
            string ekIzin;
            if (muhasebeKod > 0)
            {
                ekBirim = " WHERE u.BIRIMKOD=@BIRIMKOD AND u.MUHASEBEKOD=@MUHASEBEKOD AND u.KURKOD=@KURKOD";
                ekIzin = " AND BIRIMKOD=@BIRIMKOD AND MUHASEBEKOD=@MUHASEBEKOD AND KURKOD=@KURKOD";
            }
            else
            {
                ekBirim = "";
                ekIzin = "";
            }
            string sql = $@"WITH BIRIMLER AS (SELECT DISTINCT u.BIRIMKOD BIRIMKOD, u.MUHASEBEKOD MUHASEBEKOD, u.KURKOD KURKOD, mmb.BIRIMAD BIRIMAD, mm.MUHASEBEAD MUHASEBEAD
                FROM SBONBULGUBIRIM u
                LEFT JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM mmb
                   ON mmb.BIRIMKOD    = u.BIRIMKOD
                  AND mmb.MUHASEBEKOD = u.MUHASEBEKOD
                  AND mmb.KURKOD      = u.KURKOD
                LEFT JOIN ButceIslemleri.dbo.MUHMUHASEBE mm
                   ON mm.MUHASEBEKOD = u.MUHASEBEKOD{ekBirim}),
            GIRISLER AS (SELECT BIRIMKOD, MUHASEBEKOD, KURKOD, IZIN FROM SBONBULGUBIRIMIZIN WHERE YIL=@YIL AND TUR=@TUR{ekIzin})
            select COALESCE(a.MUHASEBEKOD, b.MUHASEBEKOD), COALESCE(a.BIRIMKOD, b.BIRIMKOD), COALESCE(a.KURKOD, b.KURKOD), a.MUHASEBEAD, a.BIRIMAD, b.IZIN 
            from BIRIMLER a
            FULL OUTER JOIN GIRISLER b
               ON  a.BIRIMKOD = b.BIRIMKOD 
               AND a.MUHASEBEKOD = b.MUHASEBEKOD
               AND a.KURKOD = b.KURKOD
            ORDER BY 1,2,3";

            veriErisim.param.Add(new VeriPara("@YIL", DbType.Int64, yil));
            veriErisim.param.Add(new VeriPara("@TUR", DbType.Int64, tur));
            if (muhasebeKod > 0)
            {
                veriErisim.param.Add(new VeriPara("@MUHASEBEKOD", DbType.Int64, muhasebeKod));
                veriErisim.param.Add(new VeriPara("@BIRIMKOD", DbType.String, birimKod));
                veriErisim.param.Add(new VeriPara("@KURKOD", DbType.String, kurKod.Replace(".", "")));
            }
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc IzinSil(int yil, int tur, TNSCollection birimler)
        {
            string sql = @"DECLARE @Liste TABLE (BIRIMKOD varchar(3), MUHASEBEKOD int, KURKOD varchar(20));
                           INSERT INTO @Liste (BIRIMKOD, MUHASEBEKOD, KURKOD) VALUES ";
            bool ilk = true;
            foreach (MUH.MuhasebeBirim item in birimler)
            {
                if (ilk)
                {
                    ilk = false;
                }
                else
                    sql += ", ";
                sql += $"({VeriErisim.DegerYap(item.birimKod)},{VeriErisim.DegerYap(item.muhasebe)},{VeriErisim.DegerYap(item.kurum.Replace(".", ""))})";
            }
            sql += "; ";

            sql += @"DELETE FROM SBONBULGUBIRIMIZIN WHERE SBONBULGUBIRIMIZIN.YIL = @YIL AND SBONBULGUBIRIMIZIN.TUR=@TUR AND EXISTS (SELECT 1 FROM @Liste L
                WHERE L.MUHASEBEKOD = SBONBULGUBIRIMIZIN.MUHASEBEKOD
                  AND L.KURKOD = SBONBULGUBIRIMIZIN.KURKOD
                  AND L.BIRIMKOD = SBONBULGUBIRIMIZIN.BIRIMKOD);";

            veriErisim.param.Add(new VeriPara("@YIL", DbType.Int64, yil));
            veriErisim.param.Add(new VeriPara("@TUR", DbType.Int64, tur));
            return KomutCalistir(sql, 0);
        }

        public Sonuc IzinEkle(int yil, int tur, TNSCollection birimler)
        {
            string sql = @"INSERT INTO SBONBULGUBIRIMIZIN(YIL, BIRIMKOD, MUHASEBEKOD, KURKOD, TUR, IZIN) VALUES ";
            bool ilk = true;
            foreach (MUH.MuhasebeBirim item in birimler)
            {
                if (ilk)
                {
                    ilk = false;
                }
                else
                    sql += ", ";
                sql += $"({yil}, {VeriErisim.DegerYap(item.birimKod)},{VeriErisim.DegerYap(item.muhasebe)},{VeriErisim.DegerYap(item.kurum.Replace(".", ""))}, {tur}, 1)";
            }
            sql += "; ";
            return KomutCalistir(sql, 0);
        }
    }
}
