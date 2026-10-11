using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Birimlere ait daire bilgilerini tutan PEBDAIRE veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABDaire : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABDaire(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBDAIRE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// DAIREKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DAIREKOD = 2,

            /// <summary>
            /// DAIREAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DAIREAD = 3
        }

        /// <summary>
        /// Birimlere ait dairelerin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURKOD", "DAIREKOD", "DAIREAD" };

        /// <summary>
        /// Daire bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Dairenin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="daire">Daire bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Daire daire)
        {
            daire.daireKod = DaireKodAl(daire.yil, daire.kurKod);
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(daire.yil) + ",";
            sql += VeriErisim.DegerYap(daire.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(daire.daireKod) + ",";
            sql += VeriErisim.DegerYap(daire.daireAd) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = daire.daireKod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan daire bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="daire">Daire bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(Daire daire)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAIREAD], daire.daireAd);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", daire.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", daire.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DAIREKOD], "=", daire.daireKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen daireyi kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve daireleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="daire">Daire bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan daireleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(Daire daire)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", daire.yil));

            if (!string.IsNullOrEmpty(daire.kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", daire.kurKod.Replace(".", "")));
            if (daire.daireKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DAIREKOD], "=", daire.daireKod));
            if (!string.IsNullOrEmpty(daire.daireAd))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DAIREAD], "LIKE", "%" + daire.daireAd + "%"));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen daireyi kullanarak,
        /// ilgili daireyi veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="daire">Daire bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Daire daire)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", daire.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", daire.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DAIREKOD], "=", daire.daireKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen daireyi kullanarak,
        /// dairenin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="daire">Daire bilgilerini tutan nesne</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool DaireVarMi(Daire daire)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURKOD] };
            string[] strDeger = { daire.kurKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.DAIREKOD] };
            int[] intDeger = { daire.yil, daire.daireKod };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen dairenin, operasyonel plan alt faaliyetlerinden herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="daire">Daire bilgilerini tutan nesne</param>
        /// <returns>Daire; operasyonel plan alt faaliyetlerinde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool DaireKullanilmisMi(Daire daire)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURKOD] };
            string[] strDeger = { daire.kurKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.DAIREKOD] };
            int[] intDeger = { daire.yil, daire.daireKod };
            return TABGenel.KayitVarmi(veriErisim, "PEBALTFAALIYET", strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen yıl ve birim bazında
        /// yeni daire programatik kodu döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Yeni daire programatik kodu döndürülür.</returns>
        private int DaireKodAl(int yil, string kurKod)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURKOD] };
            string[] strDeger = { kurKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = { yil };
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.DAIREKOD], strAlan, strDeger, intAlan, intDeger);
        }

        public IDataReader KullanilanDaireListeleEski()
        {
            string sql = @"SELECT PEBDAIRE.YIL, PEBDAIRE.KURKOD, PEBDAIRE.DAIREKOD, PEBDAIRE.DAIREAD
                            FROM PEBALTFAALIYET INNER JOIN
                            PEBDAIRE ON PEBALTFAALIYET.YIL = PEBDAIRE.YIL AND PEBALTFAALIYET.KURKOD = PEBDAIRE.KURKOD AND 
                            PEBALTFAALIYET.DAIREKOD = PEBDAIRE.DAIREKOD";
            sql += " WHERE PEBALTFAALIYET.DAIREKOD <> 0";
            sql += " ORDER BY 2,3,1";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KullanilanDaireListeleYeni()
        {
            string sql = @"SELECT PEBDAIRE.YIL, PEBDAIRE.KURKOD, PEBDAIRE.DAIREKOD, PEBDAIRE.DAIREAD
                            FROM PEBALTFAALIYET INNER JOIN
                            PEBDAIRE ON PEBALTFAALIYET.YIL = PEBDAIRE.YIL AND PEBALTFAALIYET.KURKOD = PEBDAIRE.KURKOD AND 
                            PEBALTFAALIYET.DAIREKOD = PEBDAIRE.DAIREKOD";
            sql += " WHERE PEBALTFAALIYET.DAIREKOD = 0";
            sql += " ORDER BY 2,3,1";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc DaireEkle(int yil, string kurKod, int daireKod, int orgKod)
        {
            string sql = "INSERT INTO PEBDAIRE";
            sql += " SELECT YIL";
            sql += " ," + VeriErisim.DegerYap(orgKod.ToString()) + " AS KURKOD";
            sql += " ,0 AS DAIREKOD, DAIREAD FROM PEBDAIRE";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "KURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "DAIREKOD", "=", daireKod));

            return KomutCalistir(sql, 0);
        }


        public Sonuc AltFaaliyetEkle(int yil, string kurKod, int daireKod, int orgKod)
        {
            string sql = "INSERT INTO PEBALTFAALIYET";
            sql += " SELECT YIL";
            sql += " ," + VeriErisim.DegerYap(orgKod.ToString()) + " AS KURKOD, ALTFAALIYETKOD, FPKOD";
            sql += " ,0 AS DAIREKOD, ALTFAALIYETAD, SMKOD FROM PEBALTFAALIYET";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "KURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "DAIREKOD", "=", daireKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc IsGorevEkle(int yil, string kurKod, int daireKod, int orgKod)
        {
            string sql = "INSERT INTO PEBISGOREV";
            sql += " SELECT PEBISGOREV.YIL, " + VeriErisim.DegerYap(orgKod.ToString()) + " AS KURKOD, PEBISGOREV.ISGOREVKOD, PEBISGOREV.ALTFAALIYETKOD, PEBISGOREV.ISGOREVAD, PEBISGOREV.ACIKLAMA";
            sql += " FROM PEBALTFAALIYET INNER JOIN";
            sql += " PEBISGOREV ON PEBALTFAALIYET.YIL = PEBISGOREV.YIL AND PEBALTFAALIYET.KURKOD = PEBISGOREV.KURKOD AND ";
            sql += " PEBALTFAALIYET.ALTFAALIYETKOD = PEBISGOREV.ALTFAALIYETKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("PEBALTFAALIYET", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBALTFAALIYET", "KURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBALTFAALIYET", "DAIREKOD", "=", daireKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc PEBISGOREVGOSTERGEDegis(int yil, string kurKod, int daireKod, int orgKod)
        {
            string sql = "UPDATE PEBISGOREVGOSTERGE SET KURKOD = " + VeriErisim.DegerYap(orgKod.ToString());
            sql += " FROM  PEBISGOREVGOSTERGE INNER JOIN";
            sql += " PEBISGOREV ON PEBISGOREVGOSTERGE.YIL = PEBISGOREV.YIL AND PEBISGOREVGOSTERGE.KURKOD = PEBISGOREV.KURKOD AND ";
            sql += " PEBISGOREVGOSTERGE.ISGOREVKOD = PEBISGOREV.ISGOREVKOD INNER JOIN";
            sql += " PEBALTFAALIYET ON PEBISGOREV.YIL = PEBALTFAALIYET.YIL AND PEBISGOREV.KURKOD = PEBALTFAALIYET.KURKOD AND ";
            sql += " PEBISGOREV.ALTFAALIYETKOD = PEBALTFAALIYET.ALTFAALIYETKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("PEBALTFAALIYET", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBALTFAALIYET", "KURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBALTFAALIYET", "DAIREKOD", "=", daireKod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc PEBISGOREVSORUMLUDegis(int yil, string kurKod, int daireKod, int orgKod)
        {
            string sql = "UPDATE PEBISGOREVSORUMLU SET KURKOD = " + VeriErisim.DegerYap(orgKod.ToString());
            sql += " FROM PEBISGOREVSORUMLU INNER JOIN";
            sql += " PEBISGOREV ON PEBISGOREVSORUMLU.YIL = PEBISGOREV.YIL AND PEBISGOREVSORUMLU.KURKOD = PEBISGOREV.KURKOD AND ";
            sql += " PEBISGOREVSORUMLU.ISGOREVKOD = PEBISGOREV.ISGOREVKOD INNER JOIN";
            sql += " PEBALTFAALIYET ON PEBISGOREV.YIL = PEBALTFAALIYET.YIL AND PEBISGOREV.KURKOD = PEBALTFAALIYET.KURKOD AND ";
            sql += " PEBISGOREV.ALTFAALIYETKOD = PEBALTFAALIYET.ALTFAALIYETKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("PEBALTFAALIYET", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBALTFAALIYET", "KURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBALTFAALIYET", "DAIREKOD", "=", daireKod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc PEBISGOREVZAMANLAMADegis(int yil, string kurKod, int daireKod, int orgKod)
        {
            string sql = "UPDATE PEBISGOREVZAMANLAMA SET KURKOD = " + VeriErisim.DegerYap(orgKod.ToString());
            sql += " FROM PEBISGOREVZAMANLAMA INNER JOIN";
            sql += " PEBISGOREV ON PEBISGOREVZAMANLAMA.YIL = PEBISGOREV.YIL AND PEBISGOREVZAMANLAMA.KURKOD = PEBISGOREV.KURKOD AND ";
            sql += " PEBISGOREVZAMANLAMA.ISGOREVKOD = PEBISGOREV.ISGOREVKOD INNER JOIN";
            sql += " PEBALTFAALIYET ON PEBISGOREV.YIL = PEBALTFAALIYET.YIL AND PEBISGOREV.KURKOD = PEBALTFAALIYET.KURKOD AND ";
            sql += " PEBISGOREV.ALTFAALIYETKOD = PEBALTFAALIYET.ALTFAALIYETKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("PEBALTFAALIYET", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBALTFAALIYET", "KURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBALTFAALIYET", "DAIREKOD", "=", daireKod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc IsGorevSil(int yil, string kurKod, int daireKod)
        {
            string sql = "DELETE PEBISGOREV";
            sql += " FROM PEBISGOREV INNER JOIN";
            sql += " PEBALTFAALIYET ON PEBALTFAALIYET.YIL = PEBISGOREV.YIL AND PEBALTFAALIYET.KURKOD = PEBISGOREV.KURKOD AND ";
            sql += " PEBALTFAALIYET.ALTFAALIYETKOD = PEBISGOREV.ALTFAALIYETKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("PEBALTFAALIYET", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBALTFAALIYET", "KURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBALTFAALIYET", "DAIREKOD", "=", daireKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc AltFaaliyetSil(int yil, string kurKod, int daireKod)
        {
            string sql = "DELETE FROM PEBALTFAALIYET";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "KURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "DAIREKOD", "=", daireKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc DaireSil(int yil, string kurKod, int daireKod)
        {
            string sql = "DELETE FROM PEBDAIRE";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "KURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "DAIREKOD", "=", daireKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc FPEkle(string kurKod, int fpSmNo, int enBuyukNo)
        {
            string sql = "INSERT INTO PEBFAALIYETPROJE SELECT YIL ,KURKOD ," + enBuyukNo.ToString() + " as FPKOD ,FPAD ,PROJEMI ,SORUMLU ,FAALIYETSEVIYESI ,TUR ,EBUTCEKOD FROM PEBFAALIYETPROJE";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "FPKOD", "=", fpSmNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "KURKOD", "=", kurKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public Sonuc FPAgirlikDegistir(string kurKod, int fpSmNo, int enBuyukNo)
        {
            string sql = "UPDATE PEBFAALIYETPROJEAGIRLIK SET FPKOD = " + enBuyukNo;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "FPKOD", "=", fpSmNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "FPKURKOD", "=", kurKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public Sonuc FPSMAgirlikDegistir(string kurKod, int fpSmNo, int enBuyukNo, int sabitMi)
        {
            string EK = sabitMi == 0 ? "FP" : "SM";
            string sql = "UPDATE PEBSABITMALIYETAGIRLIK SET " + EK + "KOD = " + enBuyukNo;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", EK + "KURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", EK + "KOD", "=", fpSmNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc FPSMAltFaaliyetDegistir(string kurKod, int fpSmNo, int enBuyukNo, int sabitMi)
        {
            string EK = sabitMi == 0 ? "FP" : "SM";
            string sql = "UPDATE PEBALTFAALIYET SET " + EK + "KOD = " + enBuyukNo;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "KURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", EK + "KOD", "=", fpSmNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc FPSMPEBFonDegistir(string kurKod, int fpSmNo, int enBuyukNo, int sabitMi)
        {
            string sql = "UPDATE PEBFAALIYETPROJEPEBFON SET MALIYETKOD = " + enBuyukNo;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MALIYETKOD", "=", fpSmNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MALIYETKURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SABITMI", "=", sabitMi));

            return KomutCalistir(sql, 0);
        }

        public Sonuc FPSMHarcamaYansiyanDegistir(string kurKod, int fpSmNo, int enBuyukNo, int sabitMi)
        {
            string EK = sabitMi == 0 ? "FP" : "SM";
            string sql = "UPDATE PEBFPHARCAMAYANSIYAN SET " + EK + "KOD = " + enBuyukNo;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", EK + "KURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", EK + "KOD", "=", fpSmNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc FPSMHarcamaDegistir(string kurKod, int fpSmNo, int enBuyukNo, int sabitMi)
        {
            string sql = "UPDATE PEBHARCAMA SET MALIYETKOD = " + enBuyukNo;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MALIYETKOD", "=", fpSmNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MALIYETKURKOD", "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SABITMI", "=", sabitMi));

            return KomutCalistir(sql, 0);
        }

        public Sonuc FPSil(string kurKod, int fpSmNo)
        {
            string sql = "DELETE FROM PEBFAALIYETPROJE";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "FPKOD", "=", fpSmNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "KURKOD", "=", kurKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SMEkle(string kurKod, int fpSmNo, int enBuyukNo)
        {
            string sql = "INSERT INTO PEBSABITMALIYET SELECT YIL ,SMKURKOD ," + enBuyukNo.ToString() + " as SMKOD ,SMAD ,TUR,  PHKURKOD, PERHEDEFKOD FROM PEBSABITMALIYET";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "SMKOD", "=", fpSmNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SMKURKOD", "=", kurKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SMSil(string kurKod, int fpSmNo)
        {
            string sql = "DELETE FROM PEBSABITMALIYET";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "SMKOD", "=", fpSmNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SMKURKOD", "=", kurKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }
    }
}