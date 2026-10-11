using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHYILSONUISLEMLERI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABYilSonuIslemleri : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYilSonuIslemleri(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHYILSONUISLEMLERI";

        enum ENUMAlanlar
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BELGENO = 2,
            TUR = 3,
            AD = 4,
            ACIKLAMA = 5
        }
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BELGENO", "TUR", "AD", "ACIKLAMA" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yilSonuIslem">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(YilSonuIslem yilSonuIslem)
        {
            Sonuc sonuc = new Sonuc();

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(yilSonuIslem.yil) + ", ";
            sql += VeriErisim.DegerYap(yilSonuIslem.muhasebe) + ", ";
            sql += VeriErisim.DegerYap(yilSonuIslem.belgeNo) + ", ";
            sql += VeriErisim.DegerYap(yilSonuIslem.tur) + ", ";
            sql += VeriErisim.DegerYap(yilSonuIslem.ad) + ", ";
            sql += VeriErisim.DegerYap(yilSonuIslem.aciklama) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], yilSonuIslem.belgeNo) + " ";
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yilSonuIslem.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", yilSonuIslem.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", yilSonuIslem.tur));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="YILSONUISLEMLERI">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(YilSonuIslem yilSonuIslem)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (yilSonuIslem.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yilSonuIslem.yil));

            if (yilSonuIslem.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", yilSonuIslem.muhasebe));

            if (yilSonuIslem.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", yilSonuIslem.tur));


            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc GuncelleBelgeNo(int tur, int yil, string belgeNo, int muhasebe)
        {
            string sql = string.Empty;
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], belgeNo) + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", tur));
            return KomutCalistir(sql, 1);
        }
    }
}