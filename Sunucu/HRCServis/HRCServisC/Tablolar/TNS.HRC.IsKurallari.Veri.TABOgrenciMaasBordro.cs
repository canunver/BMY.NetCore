using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Öğrenci maaş bordro bilgisini tutan PSLOGRENCIMAASBORDRO ve PSLOGRENCIMAASMIF veri tabanı tabloları ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABOgrenciMaasBordro : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOgrenciMaasBordro(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Öğrenci maaş bordro bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PSLOGRENCIMAASBORDRO";

        /// <summary>
        /// Öğrenci maaş bordro bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
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
            /// MAASYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MAASYIL = 5,
            /// <summary>
            /// MAASAY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MAASAY = 6,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 7,
            /// <summary>
            /// GUN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUN = 8,
            /// <summary>
            /// TAHAKKUKEDEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TAHAKKUKEDEN = 9,
            /// <summary>
            /// DAMGAVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DAMGAVERGISI = 10,
            /// <summary>
            /// UCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UCRET = 11,
        }

        /// <summary>
        /// Öğrenci maaş bordro tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TCKIMLIKNO", "MAASYIL", "MAASAY", "SIRANO", "GUN", "TAHAKKUKEDEN", "DAMGAVERGISI", "UCRET" };

        /// <summary>
        /// Öğrenci maaş bordro muhasebe işlem fişi numarası bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADMIF = "PSLOGRENCIMAASMIF";

        /// <summary>
        /// Öğrenci maaş bordro muhasebe işlem fişi numarası bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarMIF
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
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
            /// BELGENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGENO = 4
        }

        /// <summary>
        /// Öğrenci maaş bordro muhasebe işlem fişi numarası tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMIF ={ "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "BELGENO" };

        /// <summary>
        /// Öğrenci Eğitmen bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADTANIM = "HRCOGRENCIEGITMEN";

        /// <summary>
        /// Öğrenci Eğitmen bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarTANIM
        {
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 1,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 2,
            /// <summary>
            /// ADISOYADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADISOYADI = 3,
        }

        /// <summary>
        /// Öğrenci Eğitmen tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTANIM ={ "MUHASEBEKOD", "BIRIMKOD", "TCKIMLIKNO", "ADISOYADI" };

        /// <summary>
        /// Öğrenci maaş bordro bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="list">Eklenecek öğrenci maaş bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            int siraNo = 0;
            foreach (PSLOgrenciMaasBordro bilgi in list.objeler)
            {
                siraNo++;
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.tCKimlikNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.maasYil) + ",";
                sql += VeriErisim.DegerYap(bilgi.maasAy) + ",";
                sql += VeriErisim.DegerYap(siraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.gun) + ",";
                sql += VeriErisim.DegerYap(bilgi.tahakkukEden) + ",";
                sql += VeriErisim.DegerYap(bilgi.damgaVergisi) + ",";
                sql += VeriErisim.DegerYap(bilgi.ucret) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Öğrenci maaş bordro muhasebe işlem fişi numarası bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek öğrenci maaş bordro bilgisini tutan nesne</param>
        /// <param name="belgeNo">Eklenecek muhasebe işlem fişi numarası bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc MIFNoKaydet(PSLOgrenciMaasBordro f, string belgeNo)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "INSERT INTO " + TABLOADMIF + " (" + VeriErisim.TumAlanlar("", ALANLARMIF) + ") VALUES (";
            sql += VeriErisim.DegerYap(f.yil) + ",";
            sql += VeriErisim.DegerYap(f.muhasebe) + ",";
            sql += VeriErisim.DegerYap(f.birim) + ",";
            sql += VeriErisim.DegerYap(f.surecNo) + ",";
            sql += VeriErisim.DegerYap(belgeNo) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOADMIF + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARMIF[(int)ENUMAlanlarMIF.BELGENO], belgeNo);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", f.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.SURECNO], "=", f.surecNo));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Öğrenci maaş bordro muhasebe işlem fişi numarası bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak öğrenci maaş bordro bilgisi kıstas parametresi</param>
        /// <returns>
        /// Öğrenci maaş bordro muhasebe işlem fişi numarası bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MIFNoListele(PSLOgrenciMaasBordro f)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMIF) + " FROM " + TABLOADMIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.SURECNO], "=", f.surecNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Öğrenci maaş bordro bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak öğrenci maaş bordro bilgisi kıstas parametresi</param>
        /// <returns>
        /// Öğrenci maaş bordro bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(PSLOgrenciMaasBordro bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.ADISOYADI];
            sql += " FROM " + TABLOAD + " LEFT OUTER JOIN " + TABLOADTANIM + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.MUHASEBEKOD] + " AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BIRIMKOD] + " AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.TCKIMLIKNO];

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.maasYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MAASYIL], "=", bilgi.maasYil));
            if (bilgi.maasAy > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MAASAY], "=", bilgi.maasAy));
            if (bilgi.tCKimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", bilgi.tCKimlikNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MAASYIL] +
                "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MAASAY] +
                "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Öğrenci maaş bordro bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek öğrenci maaş bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(PSLOgrenciMaasBordro bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sonuc = KomutCalistir(sql, 0);

            if (!sonuc.islemSonuc)
                return sonuc;

            return sonuc;
        }
    }
}