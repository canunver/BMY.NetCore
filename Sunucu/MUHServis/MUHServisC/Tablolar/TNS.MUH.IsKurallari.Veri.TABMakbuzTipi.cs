using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHMAKBUZTIPI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMakbuzTipi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMakbuzTipi(VeriErisim veriErisim) : base(veriErisim) { }


        /// <summary>
        /// create table MUHMAKBUZTIPI
        /// (
        /// 	KOD INT, 
        /// 	ACIKLAMA varchar(200),
        /// 	HESAPKOD varchar(200),
        /// 	KDV INT,
        /// 	KDVHESAPKOD varchar(200), 
        /// 	KASAHESAPKOD varchar(200), 
        /// 	YANSITMAHESAPKOD varchar(200),
        /// 	KREDIHESAPKOD varchar(200),
        /// 	primary key (KOD)
        /// )
        /// </summary>
        static string TABLOAD = "MUHMAKBUZTIPI";

        enum ENUMAlanlar
        {
            KOD = 0,
            ACIKLAMA = 1,
            HESAPKOD = 2,
            KDV = 3,
            KDVHESAPKOD = 4,
            KASAHESAPKOD = 5,
            YANSITMAHESAPKOD = 6,
            KREDIHESAPKOD = 7,
            DURUM = 8
        }
        static string[] ALANLAR = { "KOD", "ACIKLAMA", "HESAPKOD", "KDV", "KDVHESAPKOD", "KASAHESAPKOD", "YANSITMAHESAPKOD", "KREDIHESAPKOD", "DURUM" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="mt">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(MakbuzTipi mt)
        {
            mt.kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD]);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(mt.kod) + ", ";
            sql += VeriErisim.DegerYap(mt.aciklama) + ", ";
            sql += VeriErisim.DegerYap(mt.hesapKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(mt.kdv) + ", ";
            sql += VeriErisim.DegerYap(mt.kdvHesapKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(mt.kasaHesapKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(mt.yansitmaHesapKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(mt.krediHesapKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(mt.durum) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = mt.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanında güncelleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="mt">Kayıt bilgisini tutan nesne</param>
        /// <returns>Komutun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(MakbuzTipi mt)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], mt.aciklama);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], mt.hesapKod.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDV], mt.kdv);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVHESAPKOD], mt.kdvHesapKod.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KASAHESAPKOD], mt.kasaHesapKod.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YANSITMAHESAPKOD], mt.yansitmaHesapKod.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KREDIHESAPKOD], mt.krediHesapKod.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], mt.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", mt.kod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = mt.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="mt">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(MakbuzTipi mt)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", mt.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="mt">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(MakbuzTipi mt)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";
            if (mt.kod > 0)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", mt.kod);

            if (!string.IsNullOrEmpty(mt.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + mt.aciklama + "%"));

            if (!string.IsNullOrEmpty(mt.hesapKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "LIKE", mt.hesapKod.Replace(".", "") + "%"));

            if (mt.kdv > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KDV], "=", mt.kdv));

            if (!string.IsNullOrEmpty(mt.kdvHesapKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KDVHESAPKOD], "LIKE", mt.kdvHesapKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(mt.kasaHesapKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KASAHESAPKOD], "LIKE", mt.kasaHesapKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(mt.yansitmaHesapKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YANSITMAHESAPKOD], "LIKE", mt.yansitmaHesapKod.Replace(".", "") + "%"));

            if (mt.durum > 0)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", mt.durum);

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ACIKLAMA];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}