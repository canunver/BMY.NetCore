using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Çeşitli ödemeler bordro bilgisini tutan PSLCESITLIODEMEBORDRO veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMahkemeHarc : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMahkemeHarc(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Çeşitli ödemeler bordro tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCMAHKEMEHARC";

        /// <summary>
        /// Çeşitli ödemeler bordro tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// SURECYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECYIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            SIRANO = 4,
            BIRIMACIKLAMA = 5,
            DAVACI = 6,
            DAVAKONUSU = 7,
            MAHKEMEKARARNO = 8,
            ICRATAKIPDURUMU = 9,
            YARGITAYDANISTAYKARARI = 10,
            KESINLESMISMAHKEMEKARARI = 11,
            KESINLESTIMI = 12,
            MIKTAR = 13,
            ODEMEEMRINO = 14,
        }

        /// <summary>
        /// Çeşitli ödemeler bordro tablosu alanlarının isimlerini tutan sabit dizi
        /// Veri tabanındaki YIL süreç yıl, SURECYIL ise hakediş yılı olarak kullanılmıştır.
        /// </summary>
        static string[] ALANLAR = { "SURECYIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "SIRANO", "BIRIMACIKLAMA", "DAVACI", "DAVAKONUSU",
             "MAHKEMEKARARNO", "ICRATAKIPDURUMU", "YARGITAYDANISTAYKARARI", "KESINLESMISMAHKEMEKARARI", "KESINLESTIMI", "MIKTAR", "ODEMEEMRINO"};

        /// <summary>
        /// Çeşitli ödemeler bordro bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="co">Eklenecek Çeşitli ödemeler bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(int surecYil, int muhasebeKod, string birimKod, string surecNo, int siraNo, MahkemeHarc co)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR);
            sql += ") VALUES (";
            sql += VeriErisim.DegerYap(surecYil) + ",";
            sql += VeriErisim.DegerYap(muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(birimKod) + ",";
            sql += VeriErisim.DegerYap(surecNo) + ",";
            sql += VeriErisim.DegerYap(siraNo) + ",";
            sql += VeriErisim.DegerYap(co.birimAciklama) + ",";
            sql += VeriErisim.DegerYap(co.davaci) + ",";
            sql += VeriErisim.DegerYap(co.davaKonusu) + ",";
            sql += VeriErisim.DegerYap(co.mahkemeKararNo) + ",";
            sql += VeriErisim.DegerYap(co.icraTakipDurumu) + ",";
            sql += VeriErisim.DegerYap(co.yargitayDanistayKarari) + ",";
            sql += VeriErisim.DegerYap(co.kesinlesmisMahkemeKarari) + ",";
            sql += VeriErisim.DegerYap(co.kesinlestiMi) + ",";
            sql += VeriErisim.DegerYap(co.miktar) + ",";
            sql += VeriErisim.DegerYap(co.odemeEmriNo) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Çeşitli ödemeler bordro bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="co">Silinecek Çeşitli ödemeler bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(MahkemeHarc co)
        {
            string sql = "DELETE FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", co.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", co.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", co.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", co.surecNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Çeşitli ödemeler bordro bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="mh">Listelenecek Çeşitli ödemeler bordro bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Çeşitli ödemeler bordro bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(MahkemeHarc mh)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", mh.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", mh.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", mh.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", mh.surecNo));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Çeşitli ödemeler bordrosuna ait ödeme emri belge numarası bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek Çeşitli ödemeler bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc OdemeEmriNoKaydet(MahkemeHarc bilgi)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], bilgi.odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", bilgi.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Çeşitli ödemeler bordrosuna ait ödeme emri belge numarası bilgisini veri tabanından çeken 
        /// sql cümleciğini verilen kıstasa göre üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="surecNo">Listeleme için kullanılacak süreç numarası kıstas parametresi</param>
        /// <returns>
        /// Çeşitli ödemeler bordrosuna ait ödeme emri belge numarası bilgisini bulmak için oluşturulan 
        /// select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleOdemeEmri(int yil, string surecNo)
        {
            string sql = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECYIL] + "," +
                              TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO] +
                  " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
