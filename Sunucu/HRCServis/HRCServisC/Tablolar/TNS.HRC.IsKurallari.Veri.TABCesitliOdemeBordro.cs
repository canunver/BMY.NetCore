using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Çeşitli ödemeler bordro bilgisini tutan PSLCESITLIODEMEBORDRO veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABCesitliOdemeBordro : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABCesitliOdemeBordro(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Çeşitli ödemeler bordro tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLCESITLIODEMEBORDRO";

        /// <summary>
        /// Çeşitli ödemeler bordro tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar : int
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
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 4,
            /// <summary>
            /// HAKEDISYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAKEDISYIL = 5,
            /// <summary>
            /// HAKEDISAY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAKEDISAY = 6,
            /// <summary>
            /// ODEMETUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMETUR = 7,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 8,
            /// <summary>
            /// SABITTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SABITTUTAR = 9,
            /// <summary>
            /// CARPAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CARPAN = 10,
            /// <summary>
            /// EKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKTUTAR = 11,
            /// <summary>
            /// TOPLAMTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMTUTAR = 12,
            /// <summary>
            /// GELIRVERGI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGI = 13,
            /// <summary>
            /// DAMGAVERGI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DAMGAVERGI = 14,
            /// <summary>
            /// DIGERVERGI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DIGERVERGI = 15,
            /// <summary>
            /// TOPLAMVERGI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMVERGI = 16,
            /// <summary>
            /// ODENECEKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENECEKTUTAR = 17,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 18,
            /// <summary>
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 19,
            PARABIRIMI = 20,
            AVANSTUTAR = 21,
            AVANSODEMEEMRINO = 22, ODEMEGUN = 23, ODEMESIRANO = 24, AVANSODEMEGUN = 25, AVANSODEMESIRANO = 26
        }

        /// <summary>
        /// Çeşitli ödemeler bordro tablosu alanlarının isimlerini tutan sabit dizi
        /// Veri tabanındaki YIL süreç yıl, SURECYIL ise hakediş yılı olarak kullanılmıştır.
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TCKIMLIKNO", "SURECYIL", "SURECAY", "ODEMETUR", "SIRANO", "SABITTUTAR", "CARPAN", "EKTUTAR", "TOPLAMTUTAR", "GELIRVERGI", "DAMGAVERGI", "DIGERVERGI", "TOPLAMVERGI", "ODENECEKTUTAR", "ACIKLAMA", "ODEMEEMRINO", "PARABIRIMI", "AVANSTUTAR", "AVANSODEMEEMRINO", "ODEMEGUN", "ODEMESIRANO", "AVANSODEMEGUN", "AVANSODEMESIRANO" };

        /// <summary>
        /// Öğrenci Eğitmen tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONEL = "HRCPEROGREGT";

        /// <summary>
        /// Öğrenci Eğitmen tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarPersonel : int
        {
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 0,
            /// <summary>
            /// ADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYAD = 1,
            /// <summary>
            /// UNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVAN = 2,
            /// <summary>
            /// BANKA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKA = 3,
            /// <summary>
            /// SUBE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SUBE = 4,
            /// <summary>
            /// HESAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HESAPNO = 5,
            /// <summary>
            /// MUHASEBEKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKODU = 6,
            /// <summary>
            /// BIRIMKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKODU = 7,
            /// <summary>
            /// ISTIHDAMSEKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISTIHDAMSEKLI = 8,
        }

        /// <summary>
        /// Öğretmen Eğitmen tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPERSONEL = { "TCKIMLIKNO", "ADSOYAD", "UNVAN", "BANKA", "SUBE", "HESAPNO", "MUHASEBEKODU", "BIRIMKODU", "ISTIHDAMSEKLI" };

        /// <summary>
        /// Çeşitli ödemeler bordro bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="co">Eklenecek Çeşitli ödemeler bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(CesitliOdeme co)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR);
            sql += ") VALUES (";
            sql += VeriErisim.DegerYap(co.surecYil) + ",";
            sql += VeriErisim.DegerYap(co.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(co.birimKod) + ",";
            sql += VeriErisim.DegerYap(co.surecNo) + ",";
            sql += VeriErisim.DegerYap(co.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(co.hakedisYil) + ",";
            sql += VeriErisim.DegerYap(co.hakedisAy) + ",";
            sql += VeriErisim.DegerYap(co.odemeTur) + ",";
            sql += VeriErisim.DegerYap(co.siraNo) + ",";
            sql += VeriErisim.DegerYap(co.sabitTutar) + ",";
            sql += VeriErisim.DegerYap(co.carpan) + ",";
            sql += VeriErisim.DegerYap(co.ekTutar) + ",";
            sql += VeriErisim.DegerYap(co.toplamTutar) + ",";
            sql += VeriErisim.DegerYap(co.gelirVergi) + ",";
            sql += VeriErisim.DegerYap(co.damgaVergi) + ",";
            sql += VeriErisim.DegerYap(co.digerVergi) + ",";
            sql += VeriErisim.DegerYap(co.toplamVergi) + ",";
            sql += VeriErisim.DegerYap(co.odenecekTutar) + ",";
            sql += VeriErisim.DegerYap(co.aciklama) + ",";
            sql += VeriErisim.DegerYap(co.odemeEmriNo) + ",";
            sql += VeriErisim.DegerYap(co.paraBirimi) + ",";
            sql += VeriErisim.DegerYap(co.avansTutar) + ",";
            sql += VeriErisim.DegerYap(co.avansOdemeEmriNo) + ",";

            sql += VeriErisim.DegerYap(co.blGun) + ",";
            sql += VeriErisim.DegerYap(co.blSiraNo) + ",";
            sql += VeriErisim.DegerYap(co.avansOdemeGun) + ",";
            sql += VeriErisim.DegerYap(co.avansOdemeSiraNo) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Çeşitli ödemeler bordro bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="co">Silinecek Çeşitli ödemeler bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(CesitliOdeme co)
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
        /// <param name="co">Listelenecek Çeşitli ödemeler bordro bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Çeşitli ödemeler bordro bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(CesitliOdeme co)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", co.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", co.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", co.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", co.surecNo));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Listele1(string tcKimlikNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADPERSONEL, ALANLARPERSONEL);
            sql += " FROM " + TABLOADPERSONEL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPersonel.TCKIMLIKNO], "=", tcKimlikNo));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Çeşitli ödemeler bordrosuna ait ödeme emri belge numarası bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek Çeşitli ödemeler bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc OdemeEmriNoKaydet(CesitliOdeme bilgi)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], bilgi.odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", bilgi.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", bilgi.tcKimlikNo));

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

        public Sonuc BankaListesiKaydet(CesitliOdeme bilgi, TNSDateTime bankaOdemeGun, int bankaOdemeSiraNo)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", "ODEMEGUN", bankaOdemeGun);
            sql += ", " + VeriErisim.AtamaOl("", "ODEMESIRANO", bankaOdemeSiraNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", bilgi.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", bilgi.tcKimlikNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc AvansOdemeEmriNoKaydet(CesitliOdeme codeme)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", "AVANSODEMEEMRINO", codeme.avansOdemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", codeme.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", codeme.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", codeme.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", codeme.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", codeme.tcKimlikNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc AvansBankaListesiKaydet(CesitliOdeme codeme, TNSDateTime avansBankaOdemeGun, int avansBankaOdemeSiraNo)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", "AVANSODEMEGUN", avansBankaOdemeGun);
            sql += ", " + VeriErisim.AtamaOl("", "AVANSODEMESIRANO", avansBankaOdemeSiraNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", codeme.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", codeme.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", codeme.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", codeme.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", codeme.tcKimlikNo));

            return KomutCalistir(sql, 0);
        }
    }
}
