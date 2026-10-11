using System;
using System.Data;
using TNS.HRC.IsKurallari;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Fazla Mesai Bordro bilgisini tutan PSLFAZLAMESAIBORDRO veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABFazlaMesaiBordro : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFazlaMesaiBordro(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Fazla Mesai İcmal tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLFAZLAMESAIBORDRO";

        /// <summary>
        /// Fazla Mesai Bordro tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKODU = 1,
            /// <summary>
            /// BIRIMKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKODU = 2,
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
            /// HAFTAICIUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAFTAICIUCRET = 8,
            /// <summary>
            /// HAFTAICIUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAFTASONUSAAT = 9,
            /// <summary>
            /// HAFTASONUSAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAFTASONUUCRET = 10,
            /// <summary>
            /// HAFTASONUUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DAMGAVERGISI = 11,
            /// <summary>
            /// DAMGAVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESINTI = 12,
            /// <summary>
            /// GELIRVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGISI = 13,
            /// <summary>
            /// GELIRVERGISIORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGISIORAN = 14,
            SGKISVEREN = 15,
            SGKISCI = 16
        }

        /// <summary>
        /// Fazla Mesai Bordro tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ILGILIYIL", "ILGILIAY", "TCKIMLIKNO", "HAFTAICISAAT", "HAFTASONUSAAT", "HAFTAICIUCRET", "HAFTASONUUCRET", "DAMGAVERGISI", "KESINTI", "GELIRVERGISI", "GELIRVERGISIORAN", "SGKISVEREN", "SGKISCI" };

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
        /// Fazla Mesai Bordro bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak fazla mesai bordro kıstas parametresi</param>
        /// <param name="bankaListesi">Eğer <c>true</c> verilirse liste, banka ve şubeye göre sıralı listelenir</param>
        /// <returns>
        /// Fazla Mesai Bordro bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(FazlaMesaiBordro f, bool bankaListesi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADPERSONEL, ALANLARPERSONEL) + " FROM " + TABLOAD + "," + TABLOADPERSONEL;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));
            if (f.ilgiliYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIYIL], "=", f.ilgiliYil));
            if (f.ilgiliAy > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIAY], "=", f.ilgiliAy));

            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + "=" + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.TCKIMLIKNO]);

            if (f.istihdamSekli > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ISTIHDAMSEKLI], "=", f.istihdamSekli));

            if (bankaListesi)
                sql += " ORDER BY " + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.BANKA] + "," + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.SUBE] + "," + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ADSOYAD] + "," + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.HESAPNO];
            else
                sql += " ORDER BY " + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ADSOYAD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Fazla Mesai Bordro bilgisinin olup olmadığı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, double döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak fazla mesai kıstas parametresi</param>
        /// <returns>
        /// Fazla Mesai Bordro bilgisinin olup olmadığı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// double nesnesini döndürür.
        /// Dönen değer 0 ise bilgi yok, 0' dan büyük ise kayıt var.
        /// </returns>
        public double Varmi(FazlaMesai f)
        {
            string sql = "";

            sql = "SELECT COUNT(*) FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));
            if (f.ilgiliYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILGILIYIL], "=", f.ilgiliYil));
            if (f.ilgiliAy > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILGILIAY], "=", f.ilgiliAy));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Fazla Mesai Bordro bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek fazla mesai bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(FazlaMesaiBordro f)
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
            sql += VeriErisim.DegerYap(f.haftaIciSaat) + ",";
            sql += VeriErisim.DegerYap(f.haftaSonuSaat) + ",";
            sql += VeriErisim.DegerYap(f.haftaIciUcret) + ",";
            sql += VeriErisim.DegerYap(f.haftaSonuUcret) + ",";
            sql += VeriErisim.DegerYap(f.damgaVergisi) + ",";
            sql += VeriErisim.DegerYap(f.kesinti) + ",";
            sql += VeriErisim.DegerYap(f.gelirVergisi) + ",";
            sql += VeriErisim.DegerYap(f.gelirVergisiOran) + ",";
            sql += VeriErisim.DegerYap(f.sgkIsveren) + ",";
            sql += VeriErisim.DegerYap(f.sgkIsci) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Fazla Mesai Bordro bilgisine ait kesinti bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek fazla mesai bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KesintiKaydet(FazlaMesaiBordro f)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += ALANLAR[(int)ENUMAlanlar.KESINTI] + "=" + VeriErisim.DegerYap(f.kesinti)+",";
            sql += ALANLAR[(int)ENUMAlanlar.SGKISVEREN] + "=" + VeriErisim.DegerYap(f.sgkIsveren) + ",";
            sql += ALANLAR[(int)ENUMAlanlar.SGKISCI] + "=" + VeriErisim.DegerYap(f.sgkIsci);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", f.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Fazla mesai bordro bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Silinecek fazla mesai bordro bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(FazlaMesaiBordro f)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));

            return KomutCalistir(sql, 0);
        }
    }
}
