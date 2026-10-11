using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.PEB;
using System.Text;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// STREYLEMPLANI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSTREYLEMPLANI : TABOrtak
    {
        /// <summary>
        /// TABSTREYLEMPLANI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSTREYLEMPLANI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STREYLEMPLANI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// EYLEMID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EYLEMID = 0,

            /// <summary>
            /// EYLEMADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EYLEMADI = 1,

            /// <summary>
            /// EYLEMTURU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EYLEMTURU = 2,

            /// <summary>
            /// YURUTENBIRIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YURUTENBIRIM = 3,

            /// <summary>
            /// BASLAMATARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BASLAMATARIHI = 4,

            /// <summary>
            /// BITISTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BITISTARIHI = 5,

            /// <summary>
            /// DONEMBYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DONEMBYIL = 6,

            /// <summary>
            /// HEDEFKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HEDEFKOD = 7,

            /// <summary>
            /// KURUMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMKOD = 8,

            /// <summary>
            /// GOSTERGEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOSTERGEKOD = 9,

            /// <summary>
            /// GOSTERGEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 10,

            /// <summary>
            /// EYLEMAMAC  alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EYLEMAMAC = 11
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "EYLEMID"
            , "EYLEMADI"
            , "EYLEMTURU"
            , "YURUTENBIRIM"
            , "BASLAMATARIHI"
            , "BITISTARIHI"
            , "DONEMBYIL"
            , "HEDEFKOD"
            , "KURUMKOD"
            , "GOSTERGEKOD"
            , "DURUM"
            , "EYLEMAMAC"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, EylemPlani yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(EylemPlani form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EYLEMID], "=", form.eylemID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, EylemPlani yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(EylemPlani form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.eylemID);
            sql += "," + VeriErisim.DegerYap(form.eylemAdi);
            sql += "," + VeriErisim.DegerYap(form.eylemTuru);
            sql += "," + VeriErisim.DegerYap(form.yurutenBirim.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.baslamaTarihi);
            sql += "," + VeriErisim.DegerYap(form.bitisTarihi);
            sql += "," + VeriErisim.DegerYap(form.donemBYil);
            sql += "," + VeriErisim.DegerYap(form.hedefKod);
            sql += "," + VeriErisim.DegerYap(form.kurumKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.gostergeKod);
            sql += "," + VeriErisim.DegerYap(form.eylemAmac);
            sql += "," + VeriErisim.DegerYap(1);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public void TarihceEkle(int eylemID, string aciklama, string kullaniciKodu)
        {
            string sql = $"INSERT INTO STREYLEMTARIHCE(EYLEMID, ISLEMACIKLAMA, ISLEMTARIHI, ISLEMIYAPAN) VALUES ({VeriErisim.DegerYap(eylemID)}, {VeriErisim.DegerYap(aciklama)}, {VeriErisim.DegerYap(new TNSDateTime(DateTime.Now))}, {VeriErisim.DegerYap(kullaniciKodu)})";
            KomutCalistir(sql, 1);
        }

        public int HareketSayisiBul(EylemPlani form)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOADGERCEKLESME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.EYLEMID], "=", form.eylemID));
            return Convert.ToInt32(veriErisim.TekKolonSQLDbl(sql));
        }

        public Sonuc EkleYilAgirlik(int eylemId, int yil, int agirlik)
        {
            string sql = $"INSERT INTO STREYLEMYILBILGI(EYLEMID, YIL, AGIRLIK) VALUES ({VeriErisim.DegerYap(eylemId)}, {VeriErisim.DegerYap(yil)}, {VeriErisim.DegerYap(agirlik)})";
            return KomutCalistir(sql, 1);
        }

        public Sonuc SilEylemDonem(EylemPlani form)
        {
            string sql = "DELETE FROM " + "STREYLEMDONEM";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARILGILIBIRIMLER[(int)ENUMAlanlarILGILIBIRIMLER.EYLEMID], "=", form.eylemID));
            return KomutCalistir(sql, 0);
        }

        public Sonuc SilForm(EylemPlani form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARILGILIBIRIMLER[(int)ENUMAlanlarILGILIBIRIMLER.EYLEMID], "=", form.eylemID));
            return KomutCalistir(sql, 1);
        }

        public Sonuc SilYilBilgi(EylemPlani form)
        {
            string sql = "DELETE FROM STREYLEMYILBILGI";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARILGILIBIRIMLER[(int)ENUMAlanlarILGILIBIRIMLER.EYLEMID], "=", form.eylemID));
            return KomutCalistir(sql, 0);
        }

        public Sonuc SilYetkiler(EylemPlani form)
        {
            string sql = "DELETE FROM " + TABLOADYETKI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARILGILIBIRIMLER[(int)ENUMAlanlarILGILIBIRIMLER.EYLEMID], "=", form.eylemID));
            return KomutCalistir(sql, 0);
        }

        public Sonuc SilYilAgirlik(int eylemId, int yil)
        {
            string sql = $"DELETE FROM STREYLEMYILBILGI WHERE EYLEMID={VeriErisim.DegerYap(eylemId)} AND YIL={VeriErisim.DegerYap(yil)}";
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, EylemPlani yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(EylemPlani form)
        {
            string sql = $"UPDATE {TABLOAD} ";
            sql += $"SET {ALANLAR[(int)ENUMAlanlar.EYLEMADI]} = {VeriErisim.DegerYap(form.eylemAdi)} ";
            sql += $", {ALANLAR[(int)ENUMAlanlar.YURUTENBIRIM]} = {VeriErisim.DegerYap(form.yurutenBirim.Replace(".", ""))} ";
            sql += $", {ALANLAR[(int)ENUMAlanlar.BASLAMATARIHI]} = {VeriErisim.DegerYap(form.baslamaTarihi)} ";
            sql += $", {ALANLAR[(int)ENUMAlanlar.BITISTARIHI]} = {VeriErisim.DegerYap(form.bitisTarihi)} ";
            sql += $", {ALANLAR[(int)ENUMAlanlar.DONEMBYIL]} = {VeriErisim.DegerYap(form.donemBYil)} ";
            sql += $", {ALANLAR[(int)ENUMAlanlar.HEDEFKOD]} = {VeriErisim.DegerYap(form.hedefKod)} ";
            sql += $", {ALANLAR[(int)ENUMAlanlar.KURUMKOD]} = {VeriErisim.DegerYap(form.kurumKod.Replace(".", ""))} ";
            sql += $", {ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD]} = {VeriErisim.DegerYap(form.gostergeKod)} ";
            sql += $", {ALANLAR[(int)ENUMAlanlar.DURUM]} = {VeriErisim.DegerYap(form.durum)} ";
            sql += $", {ALANLAR[(int)ENUMAlanlar.EYLEMAMAC]} = {VeriErisim.DegerYap(form.eylemAmac)} ";
            sql += $" WHERE {ALANLAR[(int)ENUMAlanlar.EYLEMID]}={VeriErisim.DegerYap(form.eylemID)}";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Maksimum eylem id'sinin bir fazlasını döndürür.
        /// </summary>
        /// <returns></returns>
        public int AnahtarBul()
        {
            string sql = $"SELECT MAX({ALANLAR[(int)ENUMAlanlar.EYLEMID]}) FROM {TABLOAD}";
            return Convert.ToInt32(veriErisim.TekKolonSQLDbl(sql)) + 1;
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, EylemPlani yapısında nesne</param>
        /// <param name="yil">Verisi listelenecek yıl</param>
        /// <param name="ay">Verisi listelenecek ay</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(EylemPlani form, int yil, int ay, int donemBasYil, string kullaniciKodu)
        {
            int EKYIL = 0; // yil - form.yil.donemBYil;
            string sql = "SELECT " + VeriErisim.TumAlanlar("EP", ALANLAR) + ", KK.KURAD, KK.KURAD2, YB.AGIRLIK, EG.GERCEKLESME, EG.MIKTAR,";
            sql += $" H.HEDEFKISA, HY.KISAYAZI,A.AMACKOD, A.AMACKISA, AY.KISAYAZI, HG.GORKOD, G.GOSTERGEAD, HBK.KURAD, HBK.KURAD2, HG.GERCEKLESEN, G.TUR, HG.TAHMIN{EKYIL}, OB.OLCUBIRIMAD FROM {TABLOAD} EP ";
            sql += $" INNER JOIN ButceHazirlik.dbo.ABSKUR KK ON KK.KURKOD=EP.YURUTENBIRIM AND KK.YIL={VeriErisim.DegerYap(yil)}";
            sql += $" LEFT OUTER JOIN STREYLEMYILBILGI YB ON YB.EYLEMID=EP.EYLEMID AND YB.YIL={VeriErisim.DegerYap(yil)}";
            sql += $" LEFT OUTER JOIN STREYLEMGERCEKLESME EG ON EG.EYLEMID=EP.EYLEMID AND EG.YIL={VeriErisim.DegerYap(yil)} AND EG.AY={VeriErisim.DegerYap(ay)}";

            sql += $" LEFT OUTER JOIN STRHEDEF H ON H.DONEMBYIL=EP.DONEMBYIL AND H.HEDEFKOD=EP.HEDEFKOD AND H.KURUMKOD=EP.KURUMKOD";
            sql += $" LEFT OUTER JOIN STRYAZI HY ON HY.DONEMBYIL=H.DONEMBYIL AND HY.KODTUR=5 AND HY.KOD=EP.HEDEFKOD AND HY.DILKOD=1 AND HY.KURUMKOD=H.KURUMKOD";

            sql += $" LEFT OUTER JOIN STRAMAC A ON A.DONEMBYIL=H.DONEMBYIL AND A.AMACKOD=H.AMACKOD AND A.KURUMKOD=H.KURUMKOD";
            sql += $" LEFT OUTER JOIN STRYAZI AY ON AY.DONEMBYIL=A.DONEMBYIL AND AY.KODTUR=4 AND AY.KOD=A.AMACKOD AND AY.DILKOD=1 AND AY.KURUMKOD=A.KURUMKOD";

            sql += $" LEFT OUTER JOIN STRHEDEFGOSTERGE HG ON HG.DONEMBYIL=EP.DONEMBYIL AND HG.HEDEFKOD=EP.HEDEFKOD AND HG.KURUMKOD=EP.KURUMKOD AND HG.GOSTERGEKOD=EP.GOSTERGEKOD";
            sql += $" LEFT OUTER JOIN PerformansButce.dbo.PEBGOSTERGE G ON G.GOSTERGEKOD=EP.GOSTERGEKOD";

            sql += $" LEFT OUTER JOIN STRHEDEFBIRIM HB ON HB.DONEMBYIL=EP.DONEMBYIL AND HB.HEDEFKOD=EP.HEDEFKOD AND HB.KURUMKOD=EP.KURUMKOD AND HB.SIRANO=0";
            sql += $" LEFT OUTER JOIN ButceHazirlik.dbo.ABSKUR HBK ON HBK.KURKOD=HB.KURKOD AND HBK.YIL={VeriErisim.DegerYap(yil)}";
            sql += $" LEFT OUTER JOIN PerformansButce.dbo.PEBOLCUBIRIM OB ON OB.OLCUBIRIMKOD=G.OLCUBIRIMKOD";
            string kosul = "";

            if (form.eylemID > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EP", ALANLAR[(int)ENUMAlanlar.EYLEMID], "=", form.eylemID));
            else
            {
                if (form.eylemTuru > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EP", ALANLAR[(int)ENUMAlanlar.EYLEMTURU], "=", form.eylemTuru));

                if (!string.IsNullOrEmpty(form.eylemAdi))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EP", ALANLAR[(int)ENUMAlanlar.EYLEMADI], "LIKE", "%" + form.eylemAdi + "%"));

                if (!string.IsNullOrEmpty(form.yurutenBirim))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EP", ALANLAR[(int)ENUMAlanlar.YURUTENBIRIM], "LIKE", form.yurutenBirim.Replace(".", "") + "%"));

                if (form.hedefKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EP", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", form.hedefKod));

                if (form.gostergeKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EP", ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD], "=", form.gostergeKod));

                if (donemBasYil > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", $"(DATEPART(YEAR, BASLAMATARIHI) >= {donemBasYil} AND (BITISTARIHI IS NULL OR DATEPART(YEAR, BITISTARIHI) <= {donemBasYil + 4}))");
            }
            if (!string.IsNullOrEmpty(kullaniciKodu))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", $"(1 IN (SELECT 1 FROM STREYLEMYETKI WHERE STREYLEMYETKI.EYLEMID=EP.EYLEMID AND STREYLEMYETKI.KULLANICIKODU = {VeriErisim.DegerYap(kullaniciKodu)}))");
            }
            if (!string.IsNullOrEmpty(kosul))
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            sql += " ORDER BY A.AMACKISA, H.HEDEFKISA,HG.GORKOD, EP.YURUTENBIRIM";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader EylemGerceklesmeOku(int eylemID, int yil, int ay, string kullaniciKodu)
        {
            string sql = $"SELECT EP.EYLEMID, EP.EYLEMADI, EP.DURUM, EG.YIL, EG.GERCEKLESME, EG.MIKTAR, EG.PLANLANAN, EG.GERCEKLESEN, EG.SORUNLAR, EG.ACIKLAMALAR, DNM.DURUM, YET.KULLANICIKODU FROM {TABLOAD} EP ";
            sql += $" LEFT OUTER JOIN STREYLEMGERCEKLESME EG ON EG.EYLEMID = EP.EYLEMID AND EG.YIL = {VeriErisim.DegerYap(yil)} AND EG.AY = {VeriErisim.DegerYap(ay)}";
            sql += $" LEFT OUTER JOIN STREYLEMDONEM DNM ON DNM.EYLEMID = EP.EYLEMID AND DNM.YIL={VeriErisim.DegerYap(yil)} AND DNM.AY={VeriErisim.DegerYap(ay)}";
            sql += $" LEFT OUTER JOIN STREYLEMYETKI YET ON YET.EYLEMID = EP.EYLEMID AND YET.KULLANICIKODU={VeriErisim.DegerYap(kullaniciKodu)}";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("EP", ALANLAR[(int)ENUMAlanlar.EYLEMID], "=", eylemID));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleDonem(int eylemTuru, int yil, int ay)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("EP", ALANLAR) + $", KK.KURAD2, DNM.DURUM FROM {TABLOAD} EP ";
            sql += $" LEFT OUTER JOIN ButceHazirlik.dbo.ABSKUR KK ON KK.KURKOD=EP.YURUTENBIRIM AND KK.YIL={VeriErisim.DegerYap(yil)}";
            sql += $" LEFT OUTER JOIN STREYLEMDONEM DNM ON DNM.EYLEMID=EP.EYLEMID AND DNM.YIL={VeriErisim.DegerYap(yil)} AND DNM.AY={VeriErisim.DegerYap(ay)}";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("EP", ALANLAR[(int)ENUMAlanlar.EYLEMTURU], "=", eylemTuru));
            sql += " ORDER BY KK.KURAD2, EP.EYLEMADI";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc DonemDurumEkle(int eylemId, int yil, int ay, int donemDurum)
        {
            string sql = $"INSERT INTO STREYLEMDONEM(EYLEMID, YIL, AY, DURUM) VALUES ({VeriErisim.DegerYap(eylemId)}, {VeriErisim.DegerYap(yil)}, {VeriErisim.DegerYap(ay)}, {VeriErisim.DegerYap(donemDurum)})";
            return KomutCalistir(sql, 1);
        }

        public Sonuc DonemDurumSil(int eylemId, int yil, int ay)
        {
            string sql = $"DELETE FROM STREYLEMDONEM WHERE EYLEMID={VeriErisim.DegerYap(eylemId)} AND YIL={VeriErisim.DegerYap(yil)} AND AY={VeriErisim.DegerYap(ay)}";
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADYETKI = "STREYLEMYETKI";
        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarYETKI : int
        {
            /// <summary>
            /// EYLEMID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EYLEMID = 0,

            /// <summary>
            /// KULLANICIKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 1,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARYETKI = {
            "EYLEMID"
            , "KULLANICIKODU"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADILGILIBIRIMLER = "STREYLEMILGILIBIRIMLER";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarILGILIBIRIMLER : int
        {
            /// <summary>
            /// EYLEMID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EYLEMID = 0,

            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARILGILIBIRIMLER = {
            "EYLEMID"
            , "KURKOD"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, EylemPlani yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilIlgiliBirimler(EylemPlani form)
        {
            string sql = "DELETE FROM " + TABLOADILGILIBIRIMLER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARILGILIBIRIMLER[(int)ENUMAlanlarILGILIBIRIMLER.EYLEMID], "=", form.eylemID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, eylemGrubu dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, EylemPlani yapısında nesne</param>
        /// <param name="kurKod">Eylem ile ilgili birim kodu</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleIlgiliBirim(EylemPlani form, string kurKod)
        {
            string sql = "INSERT INTO " + TABLOADILGILIBIRIMLER + "(" + VeriErisim.TumAlanlar("", ALANLARILGILIBIRIMLER) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.eylemID);
            sql += "," + VeriErisim.DegerYap(kurKod.Replace(".", ""));
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Kurkod adının bulunacağı yıl</param>
        /// <param name="eylemler">Hangi eylemlerin okunacağını tutan liste </param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleIlgiliBirimler(int yil, TNSCollection eylemler)
        {
            string sql = $"SELECT EP.EYLEMID, EP.KURKOD, KK.KURAD, KURAD2 FROM " + TABLOADILGILIBIRIMLER + "  EP";
            sql += $" INNER JOIN ButceHazirlik.dbo.ABSKUR KK ON KK.KURKOD=EP.KURKOD AND KK.YIL={VeriErisim.DegerYap(yil)}";

            sql = VeriErisim.KosulEkle(sql, "WHERE", InKosulOl("", ALANLARILGILIBIRIMLER[(int)ENUMAlanlarILGILIBIRIMLER.EYLEMID], eylemler));
            sql += " ORDER BY KURKOD DESC";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADGERCEKLESME = "STREYLEMGERCEKLESME";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarGERCEKLESME : int
        {
            /// <summary>
            /// EYLEMID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EYLEMID = 0,

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 1,

            /// <summary>
            /// AY alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AY = 2,

            /// <summary>
            /// GERCEKLESME alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GERCEKLESME = 3
            , MIKTAR = 4
            , PLANLANAN = 5
            , GERCEKLESEN = 6
            , SORUNLAR = 7
            , ACIKLAMALAR = 8
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARGERCEKLESME = {
            "EYLEMID"
            , "YIL"
            , "AY"
            , "GERCEKLESME"
            , "MIKTAR"
            , "PLANLANAN"
            , "GERCEKLESEN"
            , "SORUNLAR"
            , "ACIKLAMALAR"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="eylemID">Dönem bilgisi kaydedilecek eylem ID.</param>
        /// <param name="form">Anahtar değişkenleri tutan, EylemPlani yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleGerceklesme(int eylemID, EylemGerceklesme form)
        {
            string sql = "UPDATE " + TABLOADGERCEKLESME;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.GERCEKLESME], form.gerceklesme);
            sql += "," + VeriErisim.AtamaOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.MIKTAR], form.miktar);
            sql += "," + VeriErisim.AtamaOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.PLANLANAN], form.planlanan);
            sql += "," + VeriErisim.AtamaOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.GERCEKLESEN], form.gerceklesen);
            sql += "," + VeriErisim.AtamaOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.SORUNLAR], form.sorunlar);
            sql += "," + VeriErisim.AtamaOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.ACIKLAMALAR], form.aciklamalar);


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.EYLEMID], "=", eylemID));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.AY], "=", form.ay));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, eylemGerceklesme dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="eylemID">Dönem bilgisi kaydedilecek eylem ID.</param>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, EylemPlani yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleGerceklesme(int eylemID, EylemGerceklesme form)
        {
            string sql = "INSERT INTO " + TABLOADGERCEKLESME + "(" + VeriErisim.TumAlanlar("", ALANLARGERCEKLESME) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(eylemID);
            sql += "," + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.ay);
            sql += "," + VeriErisim.DegerYap(form.gerceklesme);
            sql += "," + VeriErisim.DegerYap(form.miktar);
            sql += "," + VeriErisim.DegerYap(form.planlanan);
            sql += "," + VeriErisim.DegerYap(form.gerceklesen);
            sql += "," + VeriErisim.DegerYap(form.sorunlar);
            sql += "," + VeriErisim.DegerYap(form.aciklamalar);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="eylemler">Okunacak eylemlerin idlerinin bulunduğu liste</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleGerceklesme(int donemBasYil, TNSCollection eylemler)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADGERCEKLESME, ALANLARGERCEKLESME) + " FROM " + TABLOADGERCEKLESME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", "(" + VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.YIL], ">=", donemBasYil));
            sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.YIL], "<=", donemBasYil + 4) + ")");
            sql = VeriErisim.KosulEkle(sql, "AND", InKosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.EYLEMID], eylemler));
            sql += " ORDER BY YIL DESC, AY DESC";
            return veriErisim.DataReaderDondur(sql);
        }

        private string InKosulOl(string tabloAd, string alanAd, TNSCollection eylemler)
        {
            if (eylemler == null || eylemler.Count == 0) return "";
            StringBuilder sb = new StringBuilder($"{tabloAd} + {((string.IsNullOrWhiteSpace(tabloAd)) ? "" : ".")} + {alanAd} IN (");
            bool ilk = true;
            foreach (EylemPlani item in eylemler)
            {
                if (!ilk) sb.Append(", ");
                sb.Append(item.eylemID.ToString());
                ilk = false;
            }
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Eylem kodunu kullanarak ,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="eylemID">Okunacak kullanıcıların ait olduğu eylem</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleYetki(int eylemID)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADYETKI, ALANLARYETKI) + ",K.AD, K.SOYAD, K.EMAIL, K.TELEFON FROM " + TABLOADYETKI + " LEFT OUTER JOIN Kullanici.dbo.KULLANICI K ON STREYLEMYETKI.KULLANICIKODU = K.KULLANICIKODU";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYETKI[(int)ENUMAlanlarYETKI.EYLEMID], "=", eylemID));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Eylem kodunu kullanarak ,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="eylemID">Okunacak kullanıcıların ait olduğu eylem</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public Sonuc SilYetki(int eylemID)
        {
            string sql = "DELETE FROM " + TABLOADYETKI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarYETKI.EYLEMID], "=", eylemID));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, eylemGrubu dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, EylemPlani yapısında nesne</param>
        /// <param name="kurKod">Eylem ile ilgili birim kodu</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleYetki(int eylemID, string kullaniciKod)
        {
            string sql = "INSERT INTO " + TABLOADYETKI + "(" + VeriErisim.TumAlanlar("", ALANLARYETKI) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(eylemID);
            sql += "," + VeriErisim.DegerYap(kullaniciKod);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleTarihce(int eylemID)
        {
            string sql = "SELECT TAR.EYLEMID, TAR.ISLEMTARIHI, TAR.ISLEMACIKLAMA, K.KULLANICIKODU, K.AD, K.SOYAD, K.TELEFON FROM STREYLEMTARIHCE TAR LEFT OUTER JOIN Kullanici.dbo.KULLANICI K ON TAR.ISLEMIYAPAN = K.KULLANICIKODU";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TAR", "EYLEMID", "=", eylemID));
            sql += " ORDER BY TAR.ISLEMTARIHI DESC";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
