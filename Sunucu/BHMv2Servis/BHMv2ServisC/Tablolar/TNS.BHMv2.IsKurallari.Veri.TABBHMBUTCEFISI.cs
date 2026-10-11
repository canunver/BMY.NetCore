using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Bütçe fişi ile ilgili, gerekçeyi tutan BHMBUTCEFISIGEREKCE veri tabanı tablosu ve bütçe fişi ile ilişkili BHMBUTCEFISIMALZEME veri tabanı tablosu  ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABBHMBUTCEFISI : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMBUTCEFISI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Gerekçe tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADBILGI = "BHMBUTCEFISIGEREKCE";

        /// <summary>
        /// Gerekçe tablosunun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarBilgi : int
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
            /// PARCA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PARCA = 6,

            /// <summary>
            /// BILGI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BILGI = 7,
        }

        /// <summary>
        /// Gerekçe tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARBILGI = {
            "YIL",
            "KURKOD",
            "FONKOD",
            "FINKOD",
            "EKOKOD",
            "PEBKOD",
            "PARCA",
            "BILGI"
        };

        /// <summary>
        /// Malzeme tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADMALZEME = "BHMBUTCEFISIMALZEME";

        /// <summary>
        /// Malzeme tablosunun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarMalzeme : int
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
            /// HESAPKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HESAPKOD = 6,

            /// <summary>
            /// HESAPAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HESAPAD = 7,

            /// <summary>
            /// MIKTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 8,

            /// <summary>
            /// FIYAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIYAT = 9,

            /// <summary>
            /// TOPLAMTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMTUTAR = 10,

            /// <summary>
            /// OLCUBIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMKOD = 11,

            /// <summary>
            /// OLCUBIRIMAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMAD = 12,
        }

        /// <summary>
        /// Malzeme tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMALZEME = {
            "YIL",
            "KURKOD",
            "FONKOD",
            "FINKOD",
            "EKOKOD",
            "PEBKOD",
            "HESAPKOD",
            "HESAPAD",
            "MIKTAR",
            "FIYAT",
            "TOPLAMTUTAR",
            "OLCUBIRIMKOD",
            "OLCUBIRIMAD"
        };

        /// <summary>
        /// Veri tabanına tertip için kaydedilecek açıklamalar parça parça kaydedilmektedir. Kaydedilecek parça için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Açıklama parçası eklenecek tertip</param>
        /// <param name="parcaNo">Eklenecek açıklamanın parça numarası</param>
        /// <param name="bilgi">Eklenecek açıklamanın parçası</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetGerekce(Tertip tertip, int parcaNo, string bilgi)
        {
            string sql;

            sql = "INSERT INTO " + TABLOADBILGI + " (" + VeriErisim.TumAlanlar("", ALANLARBILGI) + ") VALUES (";
            sql += VeriErisim.DegerYap(tertip.yil) + ",";
            sql += VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parcaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen tertiple ilgili veri tabanına kaydedilmiş gerekçenin tüm parçalarını döndürmek için 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Açıklama parçaları döndürülecek tertip</param>
        /// <returns>Tertip parçalarını bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleGerekce(Tertip tertip)
        {
            string sql = "SELECT " + ALANLARBILGI[(int)ENUMAlanlarBilgi.PARCA] + ", " + ALANLARBILGI[(int)ENUMAlanlarBilgi.BILGI];
            sql = sql + " FROM " + TABLOADBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));
            sql = sql + " order by 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen tertiple ilgili veri tabanına kaydedilmiş gerekçenin tüm parçalarını silmek için 
        /// delete sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Açıklama parçaları silinecek tertip</param>
        /// <returns>Tertip parçalarını silmek için oluşturulan delete komutu çalıştırılır ve sonucu döndürülür</returns>
        public Sonuc SilGerekce(Tertip tertip)
        {
            string sql;

            sql = "DELETE FROM " + TABLOADBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Veri tabanına tertip için kaydedilecek malzemeler teker teker kaydedilmektedir. Kaydedilecek malzeme için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Malzeme bilgisi parçası eklenecek tertip</param>
        /// <param name="malzeme">Eklenecek malzemeyle ilgili bilgileri tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetMalzeme(Tertip tertip, ButceFisiMalzeme malzeme)
        {
            string sql = "INSERT INTO " + TABLOADMALZEME + " (" + VeriErisim.TumAlanlar("", ALANLARMALZEME) + ") VALUES (";
            sql += VeriErisim.DegerYap(tertip.yil) + ",";
            sql += VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(malzeme.hesapKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(malzeme.hesapAd) + ",";
            sql += VeriErisim.DegerYap(malzeme.miktar) + ",";
            sql += VeriErisim.DegerYap(malzeme.birimFiyat) + ",";
            sql += VeriErisim.DegerYap(malzeme.toplamTutar) + ",";
            sql += VeriErisim.DegerYap(malzeme.olcuBirimKod) + ",";
            sql += VeriErisim.DegerYap(malzeme.olcuBirimAd) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen tertiple ilgili veri tabanına kaydedilmiş tüm malzemeleri döndürmek için 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Malzeme bilgileri döndürülecek tertip</param>
        /// <returns>Tertiple ilişkili malzemeleri bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleMalzeme(Tertip tertip)
        {
            string sql;

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMALZEME);
            sql = sql + " FROM " + TABLOADMALZEME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMalzeme.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMalzeme.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMalzeme.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMalzeme.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMalzeme.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMalzeme.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));
            sql = sql + " ORDER BY " + ALANLARMALZEME[(int)ENUMAlanlarMalzeme.HESAPKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen tertiple ilgili veri tabanına kaydedilmiş tüm malzemeleri silmek için 
        /// delete sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Malzeme bilgileri silinecek tertip</param>
        /// <returns>Tertiple ilişkili malzemeleri silmek için oluşturulan delete komutu çalıştırılır ve sonucu döndürülür</returns>
        public Sonuc Sil(Tertip tertip)
        {
            string sql = "DELETE FROM " + TABLOADMALZEME;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMalzeme.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMalzeme.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMalzeme.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMalzeme.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMalzeme.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALZEME[(int)ENUMAlanlarMalzeme.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Bütçe fişlerini bir önceki yıldan alıp, bu yıla kopyalayan sql komutunu oluşturup çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="tertip">Kopyalanacak bütçe fişlerinin tertip şablonunu tutan nesne.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceFisiKopyalaOncYil(Tertip tertip)
        {
            string sql = "INSERT INTO " + TABLOADBILGI + " (";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.YIL] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.FONKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.FINKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.EKOKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.PEBKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.PARCA] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.BILGI] + ") SELECT ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.YIL] + "+1, ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.FONKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.FINKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.EKOKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.PEBKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.PARCA] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.BILGI] + " FROM " + TABLOADBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.YIL], "=", tertip.yil - 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD], "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Bütçe fişi girilmiş olan tertip sayısını bulup döndüren sql komutunu hazırlayıp, çalıştırıp sonucunu döndüren yordam.
        /// </summary>
        /// <param name="tertip">Sayısı bulunacak bütçe fişlerinin tertip şablonunu tutan nesne.</param>
        /// <returns>Bütçe fişi sayısı döndürülür.</returns>
        public int ButceFisiSayisiBul(Tertip tertip)
        {
            string sql = "select COUNT(DISTINCT " + ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD] + "+" + ALANLARBILGI[(int)ENUMAlanlarBilgi.FONKOD] + "+" + ALANLARBILGI[(int)ENUMAlanlarBilgi.FINKOD] + "+" + ALANLARBILGI[(int)ENUMAlanlarBilgi.EKOKOD] + "+" + ALANLARBILGI[(int)ENUMAlanlarBilgi.PEBKOD] + ") FROM " + TABLOADBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD], "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));

            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}

/*


        //public IDataReader Listele(ButceRapor butceRapor)
        //{
        //    string sql = "";
        //    //string kosul = "";

        //    //sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

        //    //if (butceRapor.prButceRaporNo > 0)
        //    //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRRAPORNO], "=", butceRapor.prButceRaporNo));

        //    //if (butceRapor.raporGrupNo > 0)
        //    //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.RAPORGRUPNO], "=", butceRapor.raporGrupNo));

        //    //if (butceRapor.raporNo > 0)
        //    //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.RAPORNO], "=", butceRapor.raporNo));

        //    //if (!string.IsNullOrEmpty(butceRapor.kullaniciKodu))
        //    //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", butceRapor.kullaniciKodu));

        //    //if (kosul != "")
        //    //    sql += " WHERE " + kosul;

        //    //sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.PRRAPORNO] + " ," + ALANLAR[(int)ENUMAlanlar.RAPORGRUPNO] + " ," + ALANLAR[(int)ENUMAlanlar.RAPORNO];

        //    return veriErisim.DataReaderDondur(sql);
        //}
        public Sonuc KaydetGerekceAktar(int yil, string tertip, int parca, string bilgi)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();
            string[] tertipler = tertip.Split('-');

            sql = "INSERT INTO " + TABLOADBILGIAKTAR + " (" + VeriErisim.TumAlanlar("", ALANLARBILGIAKTAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(tertipler[0].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tertipler[1].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tertipler[2].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tertipler[3].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parca) + ",";
            sql += VeriErisim.DegerYap(bilgi) + ")";
            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public IDataReader ListeleGerekceAktar(int yil, string tertip)
        {
            string sql = string.Empty;
            string[] tertipler = tertip.Split('-');

            sql = "SELECT " + ALANLARBILGIAKTAR[(int)ENUMAlanlarBilgiAktar.PARCA] + ", " + ALANLARBILGIAKTAR[(int)ENUMAlanlarBilgiAktar.BILGI];
            sql = sql + " FROM " + TABLOADBILGIAKTAR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILGIAKTAR[(int)ENUMAlanlarBilgiAktar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGIAKTAR[(int)ENUMAlanlarBilgiAktar.KURKOD], "=", tertipler[0]));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGIAKTAR[(int)ENUMAlanlarBilgiAktar.FONKOD], "=", tertipler[1]));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGIAKTAR[(int)ENUMAlanlarBilgiAktar.FINKOD], "=", tertipler[2]));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGIAKTAR[(int)ENUMAlanlarBilgiAktar.EKOKOD], "=", tertipler[3]));
            sql = sql + " order by 1";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SilGerekceAktar(int yil, string tertip)
        {
            string sql = "";
            string[] tertipler = tertip.Split('-');

            sql = "DELETE FROM " + TABLOADBILGIAKTAR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILGIAKTAR[(int)ENUMAlanlarBilgiAktar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGIAKTAR[(int)ENUMAlanlarBilgiAktar.KURKOD], "=", tertipler[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGIAKTAR[(int)ENUMAlanlarBilgiAktar.FONKOD], "=", tertipler[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGIAKTAR[(int)ENUMAlanlarBilgiAktar.FINKOD], "=", tertipler[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGIAKTAR[(int)ENUMAlanlarBilgiAktar.EKOKOD], "=", tertipler[3].Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public static string TABLOADBILGIAKTAR = "BHMBUTCEFISIGEREKCEAKTAR";

        public enum ENUMAlanlarBilgiAktar : int
        {
            YIL = 0,
            KURKOD = 1,
            FONKOD = 2,
            FINKOD = 3,
            EKOKOD = 4,
            PARCA = 5,
            BILGI = 6,
        }

        static string[] ALANLARBILGIAKTAR = {
            "YIL",
            "KURKOD",
            "FONKOD",
            "FINKOD",
            "EKOKOD",
            "PARCA",
            "BILGI"
        };


*/