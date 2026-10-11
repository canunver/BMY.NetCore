using System;
using System.Data;
using TNS.HRC.IsKurallari;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Ek Ders Bordro bilgisini tutan PSLEKDERSBORDRO veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABEkDersBordro : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABEkDersBordro(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Ek Ders Bordro tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLEKDERSBORDRO";

        /// <summary>
        /// Ek Ders Bordro tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar : int
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
            /// ILGILIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILGILIYIL = 4,
            /// <summary>
            /// ILGILIAY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILGILIAY = 5,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 6,
            /// <summary>
            /// HAFTAICISAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAFTAICISAAT = 7,
            /// <summary>
            /// HAFTASONUSAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAFTASONUSAAT = 8,
            /// <summary>
            /// HAFTAICIUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAFTAICIUCRET = 9,
            /// <summary>
            /// HAFTASONUUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAFTASONUUCRET = 10,
            /// <summary>
            /// GELIRVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGISI = 11,
            /// <summary>
            /// GELIRVERGISIORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGISIORAN = 12,
            /// <summary>
            /// DAMGAVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DAMGAVERGISI = 13,
            /// <summary>
            /// KESINTI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESINTI = 14,
        }

        /// <summary>
        /// Ek Ders Bordro tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ILGILIYIL", "ILGILIAY", "TCKIMLIKNO", "HAFTAICISAAT", "HAFTASONUSAAT", "HAFTAICIUCRET", "HAFTASONUUCRET", "GELIRVERGISI", "GELIRVERGISIORAN", "DAMGAVERGISI", "KESINTI" };

        /// <summary>
        /// Personel tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONEL = "HRCPERSONEL";

        /// <summary>
        /// Personel tablo alanlarının sırasını tutan enumeration
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
            ISTIHDAMSEKLI = 8
        }

        /// <summary>
        /// Personel tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPERSONEL = { "TCKIMLIKNO", "ADSOYAD", "UNVAN", "BANKA", "SUBE", "HESAPNO", "MUHASEBEKODU", "BIRIMKODU", "ISTIHDAMSEKLI" };

        /// <summary>
        /// Öğrenci Eğitmen tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADTANIM = "HRCOGRENCIEGITMEN";

        /// <summary>
        /// Öğrenci Eğitmen tablo alanlarının sırasını tutan enumeration
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
            /// <summary>
            /// GOREV alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GOREV = 4,
            /// <summary>
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 5,
            /// <summary>
            /// BANKAKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKAKODU = 6,
            /// <summary>
            /// BANKASUBEKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKASUBEKODU = 7,
            /// <summary>
            /// BANKAHESAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKAHESAPNO = 8,
            /// <summary>
            /// DURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DURUM = 9,
            /// <summary>
            /// BANKAADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKAADI = 10,
            /// <summary>
            /// BANKASUBEADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKASUBEADI = 11,
        }

        /// <summary>
        /// Öğrenci Eğitmen tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTANIM = { "MUHASEBEKOD", "BIRIMKOD", "TCKIMLIKNO", "ADISOYADI", "GOREV", "TIP", "BANKAKODU", "BANKASUBEKODU", "BANKAHESAPNO", "DURUM", "BANKAADI", "BANKASUBEADI" };

        /// <summary>
        /// Ek ders bordro bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listelenecek Ek ders bordro bilgisi için kıstas parametresi</param>
        /// <param name="bankaListesi">Eğer <c>true</c> verilirse banka adı ve şube adına göre sıralı listeler</param>
        /// <returns>
        /// Ek ders bordro bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(EkDersBordro f, bool bankaListesi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," +
                    TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.ADISOYADI] + "," +
                    TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.GOREV] + "," +
                    TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.TIP] + "," +
                    TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BANKAKODU] + "," +
                    TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BANKASUBEKODU] + "," +
                    TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BANKAHESAPNO] + "," +
                    DBDegiskeni.TABLOADBANKA + ".AD" + "," +
                    DBDegiskeni.TABLOADBANKASUBE + ".AD" + "," +
                    " HRCPERSONEL.BANKA, HRCPERSONEL.SUBE, HRCPERSONEL.HESAPNO, HRCPERSONEL.AD , HRCPERSONEL.SOYAD, HRCPERSONEL.UNVAN " +

                    " FROM HRCPERSONEL RIGHT OUTER JOIN " +
                      " PSLEKDERSBORDRO ON HRCPERSONEL.TCKIMLIKNO = PSLEKDERSBORDRO.TCKIMLIKNO LEFT OUTER JOIN " +
                      " HRCOGRENCIEGITMEN LEFT OUTER JOIN " +
                      " HRCPERBANKASUBEVIEW ON HRCOGRENCIEGITMEN.BANKASUBEKODU = HRCPERBANKASUBEVIEW.KOD AND " +
                      " HRCOGRENCIEGITMEN.BANKAKODU = HRCPERBANKASUBEVIEW.BANKAKODU LEFT OUTER JOIN " +
                      " HRCPERBANKAVIEW ON HRCPERBANKAVIEW.KOD = HRCOGRENCIEGITMEN.BANKAKODU ON  " +
                      " PSLEKDERSBORDRO.MUHASEBEKOD = HRCOGRENCIEGITMEN.MUHASEBEKOD AND PSLEKDERSBORDRO.BIRIMKOD = HRCOGRENCIEGITMEN.BIRIMKOD AND " +
                      " PSLEKDERSBORDRO.TCKIMLIKNO = HRCOGRENCIEGITMEN.TCKIMLIKNO";


            //" FROM " + DBDegiskeni.TABLOADBANKASUBE +
            //" RIGHT OUTER JOIN " + TABLOADTANIM + " ON " +
            //DBDegiskeni.TABLOADBANKASUBE + ".KOD" + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BANKASUBEKODU] + " AND " +
            //DBDegiskeni.TABLOADBANKASUBE + ".BANKAKODU" + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BANKAKODU] +
            //" LEFT OUTER JOIN " + DBDegiskeni.TABLOADBANKA + " ON " +
            //DBDegiskeni.TABLOADBANKA + ".KOD" + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BANKAKODU] +
            //" RIGHT OUTER JOIN " + TABLOAD + " ON " +
            //TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.MUHASEBEKOD] + " AND " +
            //TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BIRIMKOD] + " AND " +
            //TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.TCKIMLIKNO];

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));
            if (f.ilgiliYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIYIL], "=", f.ilgiliYil));
            if (f.ilgiliAy > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIAY], "=", f.ilgiliAy));
            if (f.istihdamSekli > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ISTIHDAMSEKLI], "=", f.istihdamSekli));

            if (bankaListesi)
                sql += " ORDER BY " + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BANKAKODU] + "," + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BANKASUBEKODU] + "," + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.ADISOYADI] + "," + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BANKAHESAPNO];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Ek ders bordrosunun olup olmadığı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listelenecek Ek ders bordro bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Ek ders bordrosunun olup olmadığı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public double Varmi(EkDers f)
        {
            string sql = "";

            sql = "SELECT COUNT(*) FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));
            if (f.ilgiliYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILGILIYIL], "=", f.ilgiliYil));
            if (f.ilgiliAy > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILGILIAY], "=", f.ilgiliAy));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Ek Ders Bordro bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek Ek Ders Bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(EkDersBordro f)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(f.yil) + ",";
            sql += VeriErisim.DegerYap(f.muhasebe) + ",";
            sql += VeriErisim.DegerYap(f.birim) + ",";
            sql += VeriErisim.DegerYap(f.surecNo) + ",";
            sql += VeriErisim.DegerYap(f.ilgiliYil) + ",";
            sql += VeriErisim.DegerYap(f.ilgiliAy) + ",";
            sql += VeriErisim.DegerYap(f.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(f.haftaiciSaat) + ",";
            sql += VeriErisim.DegerYap(f.haftasonuSaat) + ",";
            sql += VeriErisim.DegerYap(f.haftaiciUcret) + ",";
            sql += VeriErisim.DegerYap(f.haftasonuUcret) + ",";
            sql += VeriErisim.DegerYap(f.gelirVergisi) + ",";
            sql += VeriErisim.DegerYap(f.gelirVergisiOran) + ",";
            sql += VeriErisim.DegerYap(f.damgaVergisi) + ",";
            sql += VeriErisim.DegerYap(f.kesinti) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Ek Ders Bordro kesinti bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek Ek Ders Bordro kesinti bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KesintiKaydet(EkDersBordro f)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += ALANLAR[(int)ENUMAlanlar.KESINTI] + "=" + VeriErisim.DegerYap(f.kesinti);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", f.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Ek Ders Bordro bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Silinecek Ek Ders Bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(EkDersBordro f)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));

            return KomutCalistir(sql, 0);
        }
    }
}
