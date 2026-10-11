using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.YAT;
namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// YATBUTCEYENITEKLIF veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABYATYENIPROJEBUTCE : TABOrtak
    {
        /// <summary>
        /// TABYATBUTCEYENITEKLIF sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATYENIPROJEBUTCE(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATYENIPROJEBUTCE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            YIL = 0,
            SENARYOKOD = 1,
            KAYNAKNO = 2,
            /// <summary>
            /// EKOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 3,

            /// <summary>
            /// PROJETUTARIDIS alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PROJETUTARIDIS = 4,

            /// <summary>
            /// PROJETUTARIIC alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PROJETUTARIIC = 5,

            /// <summary>
            /// DISKREDI0 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DISKREDI0 = 6,

            /// <summary>
            /// ICKAYNAK0 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ICKAYNAK0 = 7,

            /// <summary>
            /// DISKREDI1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DISKREDI1 = 8,

            /// <summary>
            /// ICKAYNAK1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ICKAYNAK1 = 9,

            /// <summary>
            /// DISKREDI2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DISKREDI2 = 10,

            /// <summary>
            /// ICKAYNAK2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ICKAYNAK2 = 11,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "SENARYOKOD"
            , "KAYNAKNO"
            , "EKOKOD"
            , "PROJETUTARIDIS"
            , "PROJETUTARIIC"
            , "DISKREDI0"
            , "ICKAYNAK0"
            , "DISKREDI1"
            , "ICKAYNAK1"
            , "DISKREDI2"
            , "ICKAYNAK2"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, ButceYeniTeklifAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, string senaryoKod, int kaynakNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, butceYeniTeklif dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, ButceYeniTeklifAna yapısında nesne</param>
        /// <param name="satirNo">butceYeniTeklif dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int yil, string senaryoKod, int kaynakNo, YeniProjeButce form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(kaynakNo);
            sql += "," + VeriErisim.DegerYap(form.ekoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.projeTutariDis);
            sql += "," + VeriErisim.DegerYap(form.projeTutariIc);
            sql += "," + VeriErisim.DegerYap(form.disKredi0);
            sql += "," + VeriErisim.DegerYap(form.icKaynak0);
            sql += "," + VeriErisim.DegerYap(form.disKredi1);
            sql += "," + VeriErisim.DegerYap(form.icKaynak1);
            sql += "," + VeriErisim.DegerYap(form.disKredi2);
            sql += "," + VeriErisim.DegerYap(form.icKaynak2);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, ButceYeniTeklifAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int yil, string senaryoKod, int kaynakNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("K", ALANLAR) + ", EKOAD FROM " + TABLOAD + " K LEFT OUTER JOIN ButceHazirlik.dbo.ABSEKO E ON K.YIL=E.YIL AND K.EKOKOD=E.EKOKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("K", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("K", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("K", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, ButceYeniTeklifAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public static string Listele(string yilUstTabloBag, string senaryoUstTabloBag, string kaynakUstTabloBag)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("K", ALANLAR) + ", EKOAD FROM " + TABLOAD + " K LEFT OUTER JOIN ButceHazirlik.dbo.ABSEKO E ON K.YIL=E.YIL AND K.EKOKOD=E.EKOKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", "K." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + yilUstTabloBag);
            sql = VeriErisim.KosulEkle(sql, "AND", "K." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + "=" + senaryoUstTabloBag);
            sql = VeriErisim.KosulEkle(sql, "AND", "K." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO] + "=" + kaynakUstTabloBag);
            return sql;
        }
    }
}
