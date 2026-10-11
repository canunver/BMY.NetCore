using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BKD;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Gider tertip tanımlarının tutulduğu, BHMTERTIPLISTESIGIDER veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// TABBHMTERTIPLISTESI soyut sınıfından üretilmiştir.
    /// </summary>
    public class TABBHMTERTIPLISTESIGIDER : TABBHMTERTIPLISTESI
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABBHMTERTIPLISTESIGIDER(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Gider tertip tanımları tablosunun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOAD = "BHMTERTIPLISTESIGIDER";

        /// <summary>
        /// Gider tertip tanımları tablosunun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 2,

            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 3,

            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 4,

            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 5,

            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 6,

            /// <summary>
            /// DURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DURUM = 7,
        }

        /// <summary>
        /// Gider tertip tanımları tablosunun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "KURKOD"
            , "FONKOD"
            , "FINKOD"
            , "EKOKOD"
            , "PEBKOD"
            , "AD"
            , "DURUM"
        };

        /*
         * 
        create table BHMTERTIPLISTESIGIDERALACAK
(
YIL INT NOT NULL,
KURKOD VARCHAR(20) NOT NULL,
FONKOD VARCHAR(20) NOT NULL,
FINKOD VARCHAR(20) NOT NULL,
EKOKOD VARCHAR(20) NOT NULL,
PEBKOD VARCHAR(20) NOT NULL,
ALKURKOD VARCHAR(20) NOT NULL,
ALFONKOD VARCHAR(20) NOT NULL,
ALFINKOD VARCHAR(20) NOT NULL,
ALEKOKOD VARCHAR(20) NOT NULL,
ALPEBKOD  VARCHAR(20) NOT NULL,
PRIMARY KEY
(
YIL,
KURKOD,
FONKOD,
FINKOD,
EKOKOD,
PEBKOD,
ALKURKOD,
ALFONKOD,
ALFINKOD,
ALEKOKOD,
ALPEBKOD

)

) 
         * 
        */

        /// <summary>
        /// Gider tertiplerinin hangi tertiplerden ödenek alacağı bilgisinin tutulduğu tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOADALACAK = "BHMTERTIPLISTESIGIDERALACAK";

        /// <summary>
        /// Gider tertiplerinin hangi tertiplerden ödenek alacağı bilgisinin tutulduğu tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarALACAK : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 2,

            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 3,

            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 4,

            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 5,

            /// <summary>
            /// ALKURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALKURKOD = 6,

            /// <summary>
            /// ALFONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALFONKOD = 7,

            /// <summary>
            /// ALFINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALFINKOD = 8,

            /// <summary>
            /// ALEKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALEKOKOD = 9,

            /// <summary>
            /// ALPEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALPEBKOD = 10,
        }

        /// <summary>
        /// Gider tertiplerinin hangi tertiplerden ödenek alacağı bilgisinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLARALACAK = {
            "YIL"
            , "KURKOD"
            , "FONKOD"
            , "FINKOD"
            , "EKOKOD"
            , "PEBKOD"
            , "ALKURKOD"
            , "ALFONKOD"
            , "ALFINKOD"
            , "ALEKOKOD"
            , "ALPEBKOD"
        };

        /// <summary>
        /// Verilen tertip nesnesi veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Tertip yıl ve kodlarını tutan, Tertip yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public override Sonuc Sil(Tertip tertip)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Verilen tertibin, ilgili yılda, kaç kez kullanıldığını bulan
        /// select sql cümlesini oluşturup, çalıştıran ve bulduğu sayısı döndüren yordam.
        /// </summary>
        /// <param name="tertip">Kullanım sayısı bulunacak tertip.</param>
        /// <returns>Tertibin kaç satırda yer aldığı bilgisi.</returns>
        public override int KayitSayisiBul(Tertip tertip)
        {
            string sql = "SELECT COUNT(*) FROM " + TABBHMODENEKGIDER.TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", TABBHMODENEKGIDER.ALANLAR[(int)TABBHMODENEKGIDER.ENUMAlanlar.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABBHMODENEKGIDER.ALANLAR[(int)TABBHMODENEKGIDER.ENUMAlanlar.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABBHMODENEKGIDER.ALANLAR[(int)TABBHMODENEKGIDER.ENUMAlanlar.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABBHMODENEKGIDER.ALANLAR[(int)TABBHMODENEKGIDER.ENUMAlanlar.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABBHMODENEKGIDER.ALANLAR[(int)TABBHMODENEKGIDER.ENUMAlanlar.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABBHMODENEKGIDER.ALANLAR[(int)TABBHMODENEKGIDER.ENUMAlanlar.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));
            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Tertip yapısında tutulan yıl, kod ve tertip açıklama bilgilerini kullanarak, tertip satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Tertip yıl ve kodlarını tutan, Tertip yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public override Sonuc Ekle(Tertip tertip)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(tertip.yil);
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.tertipAdi);
            sql += "," + VeriErisim.DegerYap((int)tertip.durum);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Tertip yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Listelenecek tertiplerin yıl ve kod kıstaslarını tutan, Tertip yapısında nesne</param>
        /// <param name="siralamaTuru">Parametre değeri, 1 ise ekonomik kod, 2 ise kurkd ve ekonomik koda göre sıralama yapılır.</param>
        /// <param name="tertipDurumlar">Listelecek tertiplerin uyacağı durumların listesi. Tertip bu durumlardan birinde ise döndürülür.</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public override IDataReader Listele(Tertip tertip, int siralamaTuru, ENUMTertipDurum[] tertipDurumlar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tertip.yil));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], tertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], tertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], tertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], tertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], tertip, 4);
            sql += TABBHMGENEL.TertipDurumKosulEkle("", ALANLAR[(int)ENUMAlanlar.DURUM], tertipDurumlar);
            if (siralamaTuru == 1)
                sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            else if (siralamaTuru == 2)
                sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KURKOD] + ", " + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OdenekAlacaklariListele(Tertip tertip)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADALACAK, ALANLARALACAK) + " FROM " + TABLOADALACAK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.YIL], "=", tertip.yil));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.KURKOD], tertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.FONKOD], tertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.FINKOD], tertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.EKOKOD], tertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.PEBKOD], tertip, 4);
            sql += " ORDER BY " + ALANLARALACAK[(int)ENUMAlanlarALACAK.KURKOD] + ", " + ALANLARALACAK[(int)ENUMAlanlarALACAK.EKOKOD];
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc OdenekAlacagiEkle(Tertip tertip, Tertip alTertip)
        {
            string sql = "INSERT INTO " + TABLOADALACAK + "(" + VeriErisim.TumAlanlar("", ALANLARALACAK) + ") VALUES (";
            sql += VeriErisim.DegerYap(tertip.yil) + ", ";

            sql += VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", "")) + ", ";

            sql += VeriErisim.DegerYap(alTertip.kodlar[0].Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(alTertip.kodlar[1].Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(alTertip.kodlar[2].Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(alTertip.kodlar[3].Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(alTertip.kodlar[4].Replace(".", "")) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc OdenekAlacaklariSil(Tertip tertip)
        {
            string sql = "DELETE FROM " + TABLOADALACAK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.YIL], "=", tertip.yil));
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.KURKOD], tertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.FONKOD], tertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.FINKOD], tertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.EKOKOD], tertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.PEBKOD], tertip, 4);
            return KomutCalistir(sql, 0);
        }

        public ENUMTertipDurum TertipDurumBul(Tertip tertip)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.DURUM] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tertip.yil));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], tertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], tertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], tertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], tertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], tertip, 4);
            int tut = veriErisim.TekKolonSQLInt(sql);
            try
            {
                return (ENUMTertipDurum)tut;
            }
            catch
            {
                return ENUMTertipDurum.TANIMSIZ;
            }
        }

        public override int TertipSayisiBul(Tertip tertip)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tertip.yil));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], tertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], tertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], tertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], tertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], tertip, 4);
            return veriErisim.TekKolonSQLInt(sql);
        }

        public int TertipDegisimKaydiSay(Tertip alanTertip, Tertip verenTertip)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOADALACAK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.YIL], "=", alanTertip.yil));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.KURKOD], alanTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.FONKOD], alanTertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.FINKOD], alanTertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.EKOKOD], alanTertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.PEBKOD], alanTertip, 4);

            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.ALKURKOD], verenTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.ALFONKOD], verenTertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.ALFINKOD], verenTertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.ALEKOKOD], verenTertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.ALPEBKOD], verenTertip, 4);

            return (int)veriErisim.TekKolonSQLDbl(sql);
        }

        public override Sonuc TertipTanimKopyalaOncYil(Tertip tertip)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") SELECT ";

            sql += ALANLAR[(int)ENUMAlanlar.YIL] + "+1, ";
            sql += ALANLAR[(int)ENUMAlanlar.KURKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FONKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FINKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.PEBKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.AD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.DURUM] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tertip.yil - 1));
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], tertip, 0);

            return KomutCalistir(sql, 0);
        }

        public override Sonuc OdenekAlacaklarKopyala(Tertip tertip)
        {
            string sql = "INSERT INTO " + TABLOADALACAK + "(" + VeriErisim.TumAlanlar("", ALANLARALACAK) + ") SELECT ";
            sql += ALANLARALACAK[(int)ENUMAlanlarALACAK.YIL] + "+1, ";

            sql += ALANLARALACAK[(int)ENUMAlanlarALACAK.KURKOD] + ", ";
            sql += ALANLARALACAK[(int)ENUMAlanlarALACAK.FONKOD] + ", ";
            sql += ALANLARALACAK[(int)ENUMAlanlarALACAK.FINKOD] + ", ";
            sql += ALANLARALACAK[(int)ENUMAlanlarALACAK.EKOKOD] + ", ";
            sql += ALANLARALACAK[(int)ENUMAlanlarALACAK.PEBKOD] + ", ";

            sql += ALANLARALACAK[(int)ENUMAlanlarALACAK.ALKURKOD] + ", ";
            sql += ALANLARALACAK[(int)ENUMAlanlarALACAK.ALFONKOD] + ", ";
            sql += ALANLARALACAK[(int)ENUMAlanlarALACAK.ALFINKOD] + ", ";
            sql += ALANLARALACAK[(int)ENUMAlanlarALACAK.ALEKOKOD] + ", ";
            sql += ALANLARALACAK[(int)ENUMAlanlarALACAK.ALPEBKOD] + " FROM " + TABLOADALACAK;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.YIL], "=", tertip.yil - 1));
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARALACAK[(int)ENUMAlanlarALACAK.KURKOD], tertip, 0);

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Tertip tablosundan; verilen alanlar, verilen kistas tertibe göre seçilir ve döndürülür.
        /// </summary>
        /// <param name="alanlar">seçilecek alan listesi</param>
        /// <param name="kistasTertip">Seçilecek tertiplerin uyacağı kriteri tutan tertip nesnesi.</param>
        /// <param name="alansay">Seçilecek kaç alan olduğunu tutan değişken</param>
        /// <returns>Oluşturulan sql'in çalıştırılmasıyla elde edilien data reader döndürülür.</returns>
        public override IDataReader FarkliTertipListele(string alanlar, Tertip kistasTertip, int alansay)
        {
            string sql = "SELECT DISTINCT " + alanlar + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kistasTertip.yil));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], kistasTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], kistasTertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], kistasTertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], kistasTertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], kistasTertip, 4);

            sql += " ORDER BY " + TABBHMGENEL.BirdenNeSirala(alansay);
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KodListele(int yil, string kodTur, string birimKurKod)
        {
            int yer = (int)ENUMAlanlar.FONKOD;
            if (kodTur == "3") yer = (int)ENUMAlanlar.FINKOD;
            else if (kodTur == "4") yer = (int)ENUMAlanlar.EKOKOD;
            else if (kodTur == "14") yer = (int)ENUMAlanlar.PEBKOD;

            string sql = "SELECT DISTINCT " + ALANLAR[yer] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            sql += VeriErisim.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], birimKurKod, " AND ");

            sql += " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IlkTertip(int yil, Tertip kistasTertip, Tertip simdikiTertip, int yon)
        {
            string maxMin = "";
            string kucBuy = "";
            if (yon < 0)
            {
                maxMin = "MAX";
                kucBuy = "<";
            }
            else
            {
                maxMin = "MIN";
                kucBuy = ">";
            }

            string sql = "select " + maxMin + "(" + ALANLAR[(int)ENUMAlanlar.KURKOD] + "+'-'+" +
                ALANLAR[(int)ENUMAlanlar.FONKOD] + "+'-'+" +
                ALANLAR[(int)ENUMAlanlar.FINKOD] + "+'-'+" +
                ALANLAR[(int)ENUMAlanlar.EKOKOD] + ")" +
                " from " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kistasTertip.yil));
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], kistasTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], kistasTertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], kistasTertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], kistasTertip, 3);


            if (yon != 0)
            {
                simdikiTertip.NoktalariAt();
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD] + "+" + ALANLAR[(int)ENUMAlanlar.FONKOD] + "+" + ALANLAR[(int)ENUMAlanlar.FINKOD] + "+" + ALANLAR[(int)ENUMAlanlar.EKOKOD], kucBuy, simdikiTertip.kodlar[0] + simdikiTertip.kodlar[1] + simdikiTertip.kodlar[2] + simdikiTertip.kodlar[3]));
            }
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
/*
        public IDataReader Oku4Duzey(int yil, string kurKod)
        {
            string sql = "SELECT DISTINCT " +
                ALANLAR[(int)ENUMAlanlar.KURKOD] + "," +
                ALANLAR[(int)ENUMAlanlar.FONKOD] + "," +
                ALANLAR[(int)ENUMAlanlar.FINKOD] + "," +
                ALANLAR[(int)ENUMAlanlar.EKOKOD] + " " +

                 " FROM " + TABLOAD +
                 " WHERE  " + ALANLAR[(int)ENUMAlanlar.YIL] + "= " + yil +
                 " AND  " + ALANLAR[(int)ENUMAlanlar.KURKOD] + "= '" + kurKod + "'";

            //sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], tertip, 0);
            //sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], tertip, 1);
            //sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], tertip, 2);
            //sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], tertip, 3);
            //sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], tertip, 4);

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KURKOD] + ", " + ALANLAR[(int)ENUMAlanlar.FONKOD] + ", " + ALANLAR[(int)ENUMAlanlar.FINKOD] + ", " + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            return veriErisim.DataReaderDondur(sql);
        }
*/