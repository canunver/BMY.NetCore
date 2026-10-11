using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BKD;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Gelir tertip tanımlarının tutulduğu, BHMTERTIPLISTESIGELIR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// TABBHMTERTIPLISTESI soyut sınıfından üretilmiştir.
    /// </summary>
    public class TABBHMTERTIPLISTESIGELIR : TABBHMTERTIPLISTESI
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMTERTIPLISTESIGELIR(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Gelir tertip tanımları tablosunun adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMTERTIPLISTESIGELIR";

        /// <summary>
        /// Gelir tertip tanımları tablosunun alanlarının sırasını tutan enumeration
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
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 2,

            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 3,

            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 4,

            /// <summary>
            /// DURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DURUM = 5,
        }

        /// <summary>
        /// Gelir tertip tanımları tablosunun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "KURKOD"
            , "EKOKOD"
            , "PEBKOD"
            , "AD"
            , "DURUM"
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
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", tertip.kodlar[2].Replace(".", "")));
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
            string sql = "SELECT COUNT(*) FROM " + TABBHMODENEKGELIR.TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", TABBHMODENEKGELIR.ALANLAR[(int)TABBHMODENEKGELIR.ENUMAlanlar.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABBHMODENEKGELIR.ALANLAR[(int)TABBHMODENEKGELIR.ENUMAlanlar.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABBHMODENEKGELIR.ALANLAR[(int)TABBHMODENEKGELIR.ENUMAlanlar.EKOKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABBHMODENEKGELIR.ALANLAR[(int)TABBHMODENEKGELIR.ENUMAlanlar.PEBKOD], "=", tertip.kodlar[2].Replace(".", "")));
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
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], tertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], tertip, 2);
            if (siralamaTuru == 1)
                sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            else if (siralamaTuru == 2)
                sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KURKOD] + ", " + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            return veriErisim.DataReaderDondur(sql);
        }

        public override int TertipSayisiBul(Tertip tertip)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tertip.yil));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], tertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], tertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], tertip, 2);
            return veriErisim.TekKolonSQLInt(sql);
        }

        public override Sonuc TertipTanimKopyalaOncYil(Tertip tertip)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") SELECT ";

            sql += ALANLAR[(int)ENUMAlanlar.YIL] + "+1, ";
            sql += ALANLAR[(int)ENUMAlanlar.KURKOD] + ", ";
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
            return new Sonuc(true, "", "");
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
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], kistasTertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], kistasTertip, 2);

            sql += " ORDER BY " + TABBHMGENEL.BirdenNeSirala(alansay);
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
