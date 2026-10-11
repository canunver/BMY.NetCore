using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.PEB;
namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// TABSTRHEDEFGOSTERGEGERCEKLESME veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSTRHEDEFGOSTERGEGERCEKLESME : TABOrtak
    {
        /// <summary>
        /// TABSTRHEDEFGOSTERGEGERCEKLESME sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSTRHEDEFGOSTERGEGERCEKLESME(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRHEDEFGOSTERGEGERCEKLESME";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADGERCBILGI = "STRHEDEFGERCEKLESMEBILGI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// DONEMBYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DONEMBYIL = 0,

            /// <summary>
            /// HEDEFKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HEDEFKOD = 1,

            /// <summary>
            /// KURUMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMKOD = 2,

            /// <summary>
            /// GOSTERGEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOSTERGEKOD = 3,

            /// <summary>
            /// GERCYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GERCYIL = 4,

            /// <summary>
            /// DONEM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DONEM = 5,

            /// <summary>
            /// DONEMICIGERCEKLESME alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DONEMICIGERCEKLESME = 6,

            /// <summary>
            /// BIRIKIMLIGERCEKLESME alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIKIMLIGERCEKLESME = 7,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 8,

            /// <summary>
            /// ILGILILIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILILIK = 9,

            /// <summary>
            /// ETKILILIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ETKILILIK = 10,

            /// <summary>
            /// ETKINLIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ETKINLIK = 11,

            /// <summary>
            /// SURDURULEBILIRLIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURDURULEBILIRLIK = 12,

        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarGERCBILGI : int
        {

            /// <summary>
            /// DONEMBYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DONEMBYIL = 0,

            /// <summary>
            /// HEDEFKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HEDEFKOD = 1,

            /// <summary>
            /// KURUMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMKOD = 2,

            /// <summary>
            /// GERCYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GERCYIL = 3,

            /// <summary>
            /// DONEM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DONEM = 4,

            /// <summary>
            /// BILGITUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BILGITUR = 5,

            /// <summary>
            /// SATIRNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATIRNO = 6,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "DONEMBYIL"
            , "HEDEFKOD"
            , "KURUMKOD"
            , "GOSTERGEKOD"
            , "GERCYIL"
            , "DONEM"
            , "DONEMICIGERCEKLESME"
            , "BIRIKIMLIGERCEKLESME"
            , "ACIKLAMA"
            , "ILGILILIK","ETKILILIK","ETKINLIK","SURDURULEBILIRLIK"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARGERCBILGI = {
            "DONEMBYIL"
            , "HEDEFKOD"
            , "KURUMKOD"
            , "GERCYIL"
            , "DONEM"
            , "BILGITUR"
            , "SATIRNO"
            , "ACIKLAMA"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, StratejikGostergeGerceklesme yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(StrHedefGosterge form, bool aktarim)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", form.donemBYil));
            if (!aktarim)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", form.hedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kurumKod.Replace(".", "")));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD], "=", form.gostergeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GERCYIL], "=", form.gercYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEM], "=", form.donem));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, StratejikGostergeGerceklesme yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilGercBilgi(StrHedefGosterge form)
        {
            string sql = "DELETE FROM " + TABLOADGERCBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGERCBILGI[(int)ENUMAlanlarGERCBILGI.DONEMBYIL], "=", form.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCBILGI[(int)ENUMAlanlarGERCBILGI.HEDEFKOD], "=", form.hedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCBILGI[(int)ENUMAlanlarGERCBILGI.KURUMKOD], "=", form.kurumKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCBILGI[(int)ENUMAlanlarGERCBILGI.GERCYIL], "=", form.gercYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCBILGI[(int)ENUMAlanlarGERCBILGI.DONEM], "=", form.donem));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, StratejikGostergeGerceklesme yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(StrHedefGosterge form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.donemBYil);
            sql += "," + VeriErisim.DegerYap(form.hedefKod);
            sql += "," + VeriErisim.DegerYap(form.kurumKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.gostergeKod);
            sql += "," + VeriErisim.DegerYap(form.gercYil);
            sql += "," + VeriErisim.DegerYap(form.donem);
            sql += "," + VeriErisim.DegerYap(-1);
            sql += "," + VeriErisim.DegerYap(form.birikimliGerceklesme);
            sql += "," + VeriErisim.DegerYap(form.aciklama);

            sql += "," + VeriErisim.DegerYap(form.ilgililik);
            sql += "," + VeriErisim.DegerYap(form.etkililik);
            sql += "," + VeriErisim.DegerYap(form.etkinlik);
            sql += "," + VeriErisim.DegerYap(form.surdurulebilirlik);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc EkleGercBilgi(StrHedefGosterge form, int tur, int sat, string aciklama)
        {
            string sql = "INSERT INTO " + TABLOADGERCBILGI + "(" + VeriErisim.TumAlanlar("", ALANLARGERCBILGI) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.donemBYil);
            sql += "," + VeriErisim.DegerYap(form.hedefKod);
            sql += "," + VeriErisim.DegerYap(form.kurumKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.gercYil);
            sql += "," + VeriErisim.DegerYap(form.donem);
            sql += "," + VeriErisim.DegerYap(tur);
            sql += "," + VeriErisim.DegerYap(sat);
            sql += "," + VeriErisim.DegerYap(aciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, StratejikGostergeGerceklesme yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(StrHedefGosterge form)
        {
            string sql = "SELECT  " + VeriErisim.TumAlanlar("HG", TABSTRHEDEFGOSTERGE.ALANLAR) + ", PG.GOSTERGEAD, PO.OLCUBIRIMAD, HGG.DONEMICIGERCEKLESME, HGG.BIRIKIMLIGERCEKLESME, HGG.ACIKLAMA, HGG.ILGILILIK, HGG.ETKILILIK, HGG.ETKINLIK, HGG.SURDURULEBILIRLIK, HGGONC.BIRIKIMLIGERCEKLESME, STRMISYON.ARALIKBASYIL, STRMISYON.ARALIKSONYIL, PG.TUR, PG.ILERLEMEYONU, PG.KULLANIMSEKLI";
            sql += " FROM " + TABSTRHEDEFGOSTERGE.TABLOAD + " HG ";
            sql += " INNER JOIN PerformansButce.dbo.PEBGOSTERGE PG ON HG.GOSTERGEKOD=PG.GOSTERGEKOD AND HG.KURUMKOD=PG.KURUMKOD";
            sql += " INNER JOIN PerformansButce.dbo.PEBOLCUBIRIM PO ON PG.OLCUBIRIMKOD=PO.OLCUBIRIMKOD ";
            sql += " INNER JOIN STRMISYON ON STRMISYON.DONEMBYIL=HG.DONEMBYIL AND STRMISYON.KURUMKOD=HG.KURUMKOD";
            sql += " LEFT OUTER JOIN " + TABLOAD + " HGG ON HGG.DONEMBYIL=HG.DONEMBYIL AND HGG.HEDEFKOD=HG.HEDEFKOD AND HGG.KURUMKOD=HG.KURUMKOD AND HGG.GOSTERGEKOD=HG.GOSTERGEKOD AND HGG.GERCYIL=" + VeriErisim.DegerYap(form.gercYil) + " AND HGG.DONEM=" + VeriErisim.DegerYap(form.donem);
            sql += " LEFT OUTER JOIN " + TABLOAD + " HGGONC ON HGGONC.DONEMBYIL=HG.DONEMBYIL AND HGGONC.HEDEFKOD=HG.HEDEFKOD AND HGGONC.KURUMKOD=HG.KURUMKOD AND HGGONC.GOSTERGEKOD=HG.GOSTERGEKOD AND HGGONC.GERCYIL=" + VeriErisim.DegerYap(form.gercYil - 1) + $" AND HGGONC.DONEM={VeriErisim.DegerYap((int)ENUMDonemler.Yil)}";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("HG", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", form.donemBYil));
            if (form.hedefKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HG", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", form.hedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HG", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kurumKod.Replace(".", "")));
            sql += " order by HG.GORKOD";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OkuGercBilgi(StrHedefGosterge form, int bilgiTur)
        {
            string sql = "SELECT HGG.ACIKLAMA FROM " + TABLOADGERCBILGI + " HGG";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("HGG", ALANLARGERCBILGI[(int)ENUMAlanlarGERCBILGI.DONEMBYIL], "=", form.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HGG", ALANLARGERCBILGI[(int)ENUMAlanlarGERCBILGI.HEDEFKOD], "=", form.hedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HGG", ALANLARGERCBILGI[(int)ENUMAlanlarGERCBILGI.KURUMKOD], "=", form.kurumKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HGG", ALANLARGERCBILGI[(int)ENUMAlanlarGERCBILGI.GERCYIL], "=", form.gercYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HGG", ALANLARGERCBILGI[(int)ENUMAlanlarGERCBILGI.DONEM], "=", form.donem));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HGG", ALANLARGERCBILGI[(int)ENUMAlanlarGERCBILGI.BILGITUR], "=", bilgiTur));
            sql += " ORDER BY HGG.SATIRNO";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
