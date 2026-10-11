using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHNAKITTALEBI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABNakitTalebi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABNakitTalebi(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHNAKITTALEBI";
        enum ENUMAlanlar
        {
            YIL = 0,
            AY = 1,
            TESKILAT = 2,
            BIRIM = 3,
            ASAMA = 4,
            HAFTA = 5,
            EKOKOD = 6,
            TUTAR = 7,
            KULLANICIKODU = 8,
        }
        static string[] ALANLAR = { "YIL", "AY", "TESKILAT", "BIRIM", "ASAMA", "HAFTA", "EKOKOD", "TUTAR", "KULLANICIKODU" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kullaniciAd">kayıt eden kullanıcının adı</param>
        /// <param name="bilgi">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(string kullaniciAd, NakitTalebi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.yil) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.ay) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.teskilat) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.birim.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.asama) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.hafta) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.ekoKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.tutar) + ", ";
            sql = sql + VeriErisim.DegerYap(kullaniciAd) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(NakitTalebi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil) + " AND ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", bilgi.ay) + " AND ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TESKILAT], "=", bilgi.teskilat) + " AND ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", bilgi.birim.Replace(".", ""));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(NakitTalebi bilgi)
        {
            string sql = "";

            sql = "SELECT YIL,AY";
            //sql += ",MUHASEBE,BIRIM";
            sql += ",ASAMA,HAFTA,EKOKOD,SUM(TUTAR) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", bilgi.ay));

            if (bilgi.teskilat > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TESKILAT], "=", bilgi.teskilat));
            else if (bilgi.teskilat == -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TESKILAT] + "/1000", "=", 0));
            else if (bilgi.teskilat == -2)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TESKILAT] + "/1000", ">", 0));

            if (bilgi.birim.Replace(".", "") != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", bilgi.birim.Replace(".", "")));

            if (bilgi.asama > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", bilgi.asama));

            if (bilgi.hafta > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HAFTA], "=", bilgi.hafta));

            if (bilgi.ekoKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", bilgi.ekoKod));

            sql += " GROUP BY YIL,AY,ASAMA,HAFTA,EKOKOD";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
