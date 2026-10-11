using System;
using System.Data;
using TNS.HRC.IsKurallari;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Ek Ders Personel Puantaj bilgisini tutan PSLEKDERSPERSONELPUANTAJ veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABEkDersPersonel : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABEkDersPersonel(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Ek Ders Personel Puantaj tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLEKDERSPERSONELPUANTAJ";

        /// <summary>
        /// Personel tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPERSONEL = "HRCPERSONEL";

        /// <summary>
        /// Ek Ders Personel Puantaj tablo alanlarının sırasını tutan enumeration
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
            /// PUANTAJKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PUANTAJKODU = 7,
            /// <summary>
            /// GELIRVERGISIORANI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GELIRVERGISIORANI = 8,
        }

        /// <summary>
        /// Ek Ders Personel Puantaj tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ILGILIYIL", "ILGILIAY", "TCKIMLIKNO", "PUANTAJKODU", "GELIRVERGISIORANI" };

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
            /// MUHASEBEKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKODU = 3,
            /// <summary>
            /// BIRIMKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKODU = 4,
        }

        /// <summary>
        /// Ek Ders Bordro tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPERSONEL = { "TCKIMLIKNO", "ADSOYAD", "UNVAN", "MUHASEBEKODU", "BIRIMKODU" };

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
        }

        /// <summary>
        /// Öğrenci Eğitmen tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTANIM = { "MUHASEBEKOD", "BIRIMKOD", "TCKIMLIKNO", "ADISOYADI" };

        /// <summary>
        /// Ek Ders Puantaj tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADPUANTAJ = "PSLEKDERSPUANTAJ";

        /// <summary>
        /// Ek Ders Puantaj tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarPuantaj
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 1,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 2,
        }

        /// <summary>
        /// Ek Ders Bordro tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPUANTAJ = { "YIL", "KOD", "ACIKLAMA" };

        /// <summary>
        /// Ek Ders Personel Puantaj bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listelenecek ek ders personel bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Ek ders personel puantaj bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(EkDers f)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," +
                    TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.ADISOYADI] + "," +
                    TABLOADPUANTAJ + "." + ALANLARPUANTAJ[(int)ENUMAlanlarPuantaj.ACIKLAMA] + "," +
                    TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ADSOYAD] +
                    " FROM " + TABLOAD +
                    " LEFT OUTER JOIN " + TABLOADPERSONEL + " ON " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + "=" + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.TCKIMLIKNO] +
                    " LEFT OUTER JOIN " + TABLOADTANIM + " ON " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.MUHASEBEKOD] + " AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.BIRIMKOD] + " AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + "=" + TABLOADTANIM + "." + ALANLARTANIM[(int)ENUMAlanlarTANIM.TCKIMLIKNO] +
                    " LEFT OUTER JOIN " + TABLOADPUANTAJ + " ON " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADPUANTAJ + "." + ALANLARPUANTAJ[(int)ENUMAlanlarPuantaj.YIL] + " AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PUANTAJKODU] + "=" + TABLOADPUANTAJ + "." + ALANLARPUANTAJ[(int)ENUMAlanlarPuantaj.KOD];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
            if (f.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            if (f.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));
            if (f.ilgiliYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIYIL], "=", f.ilgiliYil));
            if (f.ilgiliAy > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIAY], "=", f.ilgiliAy));

            sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIYIL] + ", "
                                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILGILIAY] + ", "
                                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", "
                                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", "
                                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Personel bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <returns>
        /// Personel bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListelePersonel(int muhasebe, string birim)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADPERSONEL, ALANLARPERSONEL) + " FROM " + TABLOADPERSONEL;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPersonel.MUHASEBEKODU], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPERSONEL, ALANLARPERSONEL[(int)ENUMAlanlarPersonel.BIRIMKODU], "=", birim));
            sql += " ORDER BY " + TABLOADPERSONEL + "." + ALANLARPERSONEL[(int)ENUMAlanlarPersonel.ADSOYAD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Ek ders personel puantaj bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek ek ders bilgisini tutan nesne</param>
        /// <param name="fp">Eklenecek personel puantaj bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(EkDers f, EkDersPersonel fp)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(f.yil) + ",";
            sql += VeriErisim.DegerYap(f.muhasebe) + ",";
            sql += VeriErisim.DegerYap(f.birim) + ",";
            sql += VeriErisim.DegerYap(f.surecNo) + ",";
            sql += VeriErisim.DegerYap(f.ilgiliYil) + ",";
            sql += VeriErisim.DegerYap(f.ilgiliAy) + ",";
            sql += VeriErisim.DegerYap(fp.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(fp.puantaj.kod) + ",";
            sql += VeriErisim.DegerYap(fp.gelirVergisiOrani) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Ek ders personel puantaj bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Silinecek ek ders personel puantaj bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(EkDers f)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", f.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", f.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));

            return KomutCalistir(sql, 0);
        }
    }
}
