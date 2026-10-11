using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// İhale tür bilgisini tutan HRCIHALETUR veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIhaleTur : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param> 
        public TABIhaleTur(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İhale tür bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCIHALETUR";

        /// <summary>
        /// İhale tür bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 1,
        }

        /// <summary>
        /// İhale tür tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "KOD", "ADI" };

        /// <summary>
        /// İhale tür bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek ihale tür bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (IhaleTur f in bilgi.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.kod) + ",";
                sql += VeriErisim.DegerYap(f.adi) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], f.adi);
                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", f.kod));

                    sonuc = KomutCalistir(sql, 1);

                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            return sonuc;
        }

        /// <summary>
        /// İhale tür bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak ihale tür bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale tür bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(IhaleTur bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (bilgi != null)
            {
                if (bilgi.kod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
                if (bilgi.adi != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ADI], "=", bilgi.adi));

                if (kosul != "")
                    sql += " WHERE " + kosul;
            }

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD] + " ASC ";

            return veriErisim.DataReaderDondur(sql);
        }
    }

    /// <summary>
    /// İhale türü madde bilgisini tutan HRCIHALETURMADDE veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIhaleTurMadde : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIhaleTurMadde(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İhale türü madde bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCIHALETURMADDE";

        /// <summary>
        /// İhale türü madde bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// IHALEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEKOD = 1,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 2,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 3,
            /// <summary>
            /// ILANZORUNLUMU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILANZORUNLUMU = 4,
        }

        /// <summary>
        /// İhale türü madde tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "KOD", "IHALEKOD", "ADI", "ACIKLAMA", "ILANZORUNLUMU" };

        /// <summary>
        /// İhale türü madde bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek ihale türü madde bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (IhaleTurMadde f in bilgi.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.kod) + ",";
                sql += VeriErisim.DegerYap(f.ihaleKod) + ",";
                sql += VeriErisim.DegerYap(f.adi) + ",";
                sql += VeriErisim.DegerYap(f.aciklama) + ",";
                sql += VeriErisim.DegerYap(f.ilanZorunlu) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], f.adi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], f.aciklama) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILANZORUNLUMU], f.ilanZorunlu);
                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", f.kod));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", f.ihaleKod));

                    sonuc = KomutCalistir(sql, 1);

                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            return sonuc;
        }

        /// <summary>
        /// İhale türü madde bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="ihaleKod">Listeleme için kullanılacak ihale türü madde bilgisi kıstas parametresi</param>
        /// <returns>
        /// İhale türü madde bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(int ihaleKod)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (ihaleKod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", ihaleKod));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD] + " ASC ";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İhale usulu madde bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="ihaleKod">Listeleme için kullanılacak ihale kod kıstas parametresi</param>
        /// <param name="maddeKod">Listeleme için kullanılacak madde kod kıstas parametresi</param>
        /// <returns>
        /// İhale usulu madde bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader IhaleUsulMaddeAl(int ihaleKod, int maddeKod)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (ihaleKod > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", ihaleKod));

                if (maddeKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", maddeKod));
            }


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD] + " ASC ";

            return veriErisim.DataReaderDondur(sql);
        }
    }

    /// <summary>
    /// İhale ilan tanım bilgisini tutan HRCIHALEILANTANIM veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIhaleIlanTanim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIhaleIlanTanim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İhale ilan tanım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCIHALEILANTANIM";

        /// <summary>
        /// İhale ilan tanım bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// IHALEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEKOD = 0,
            /// <summary>
            /// SIRA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRA = 1,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 2,
            /// <summary>
            /// YAKLASIKMALIYETMALHIZMET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYETMALHIZMET = 3,
            /// <summary>
            /// YAKLASIKMALIYETMALHIZMETUST alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYETMALHIZMETUST = 4,
            /// <summary>
            /// YAKLASIKMALIYETYAPIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYETYAPIM = 5,
            /// <summary>
            /// YAKLASIKMALIYETYAPIMUST alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYETYAPIMUST = 6,
            /// <summary>
            /// ZAMAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ZAMAN = 7,
            /// <summary>
            /// ZAMANBASLAMATUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ZAMANBASLAMATUR = 8,
        }

        /// <summary>
        /// İhale ilan tanım tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "IHALEKOD", "SIRA", "ACIKLAMA", "YAKLASIKMALIYETMALHIZMET", "YAKLASIKMALIYETMALHIZMETUST", "YAKLASIKMALIYETYAPIM", "YAKLASIKMALIYETYAPIMUST", "ZAMAN", "ZAMANBASLAMATUR" };

        /// <summary>
        /// İhale ilan tanım bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek ihale ilan tanım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            int sira = 1;
            foreach (IhaleIlanTanim f in bilgi.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.ihaleKod) + ",";
                sql += VeriErisim.DegerYap(sira) + ",";
                sql += VeriErisim.DegerYap(f.aciklama) + ",";
                sql += VeriErisim.DegerYap(f.yaklasikMaliyetMalHizmet) + ",";
                sql += VeriErisim.DegerYap(f.yaklasikMaliyetMalHizmetUst) + ",";
                sql += VeriErisim.DegerYap(f.yaklasikMaliyetYapim) + ",";
                sql += VeriErisim.DegerYap(f.yaklasikMaliyetYapimUst) + ",";
                sql += VeriErisim.DegerYap(f.zaman) + ",";
                sql += VeriErisim.DegerYap(f.zamanBaslamaTur) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], f.aciklama) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETMALHIZMET], f.yaklasikMaliyetMalHizmet) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETMALHIZMETUST], f.yaklasikMaliyetMalHizmetUst) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETYAPIM], f.yaklasikMaliyetYapim) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETYAPIMUST], f.yaklasikMaliyetYapimUst) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZAMAN], f.zaman) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZAMANBASLAMATUR], f.zamanBaslamaTur);
                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", f.ihaleKod));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", sira));

                    sonuc = KomutCalistir(sql, 1);

                    if (!sonuc.islemSonuc)
                        break;
                }
                sira++;
            }
            return sonuc;
        }

        /// <summary>
        /// İhale ilan tanım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="ihaleKod">Listeleme için kullanılacak ihale kod kıstas parametresi</param>
        /// <param name="sira">Listeleme için kullanılacak sıra numarası kıstas parametresi</param>
        /// <returns>
        /// İhale ilan tanım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(int ihaleKod, int sira)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (ihaleKod > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", ihaleKod));

                if (sira > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", sira));
            }
            
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRA] + " ASC ";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// İhale ilan şekli bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="ihaleKod">Listeleme için kullanılacak ihale kod kıstas parametresi</param>
        /// <param name="fiyat">Listeleme için kullanılacak ihale fiyat kıstas parametresi</param>
        /// <param name="isTur">The is tur.</param>
        /// <returns>
        /// İhale ilan şekli bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader IhaleIlanSekliAl(int ihaleKod, double fiyat,int isTur)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (ihaleKod > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", ihaleKod));

                if (isTur == (int)ENUMIhaleTur.YAPIM)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETYAPIM], "<=", fiyat));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETYAPIMUST], ">=", fiyat));
                }
                else
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETMALHIZMET], "<=", fiyat));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETMALHIZMETUST], ">=", fiyat));                
                }

            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRA] + " ASC ";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}