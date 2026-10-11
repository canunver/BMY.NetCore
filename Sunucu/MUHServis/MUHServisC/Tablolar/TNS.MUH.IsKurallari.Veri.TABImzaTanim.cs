using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHIMZATANIM veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABImzaTanim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABImzaTanim(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "MUHIMZATANIM";
        public static string TABLOADDETAY = "MUHIMZATANIMDETAY";

        public enum ENUMAlanlar { KOD = 0, AD = 1, BELGETUR = 2, KULLANICIKODU = 3 }
        public enum ENUMAlanlarDetay { IMZATANIMKOD = 0, IMZAYER = 1, AD = 2, UNVAN = 3, GOREVUNVAN = 4, TARIH = 5, KULLANILMIYOR = 6 }

        public static string[] ALANLAR = { "KOD", "AD", "BELGETUR", "KULLANICIKODU" };
        public static string[] ALANLARDETAY = { "IMZATANIMKOD", "IMZAYER", "AD", "UNVAN", "GOREVUNVAN", "TARIH", "KULLANILMIYOR" };

        public Sonuc Kaydet(ImzaTanim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            form.kod = this.KodAl();
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.ad) + ",";
                sql += VeriErisim.DegerYap((int)form.belgeTur) + ",";
                sql += VeriErisim.DegerYap(form.kullaniciKodu) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(ImzaTanim form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }
        public Sonuc Kaydet(ImzaTanimDetay detay)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(detay.imzaTanimKod) + ",";
            sql += VeriErisim.DegerYap(detay.imzaYer) + ",";
            sql += VeriErisim.DegerYap(detay.adSoyad) + ",";
            sql += VeriErisim.DegerYap(detay.unvan) + ",";
            sql += VeriErisim.DegerYap(detay.gorevUnvan) + ",";
            sql += VeriErisim.DegerYap(detay.tarih) + ",";
            sql += VeriErisim.DegerYap(detay.kullanilmiyor) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(ImzaTanim form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            if (form.kullaniciKodu != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", form.kullaniciKodu));

            if ((int)form.belgeTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)form.belgeTur));

            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader ListeleDetay(int imzaTanimKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.IMZATANIMKOD], "=", imzaTanimKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(ImzaTanim form)
        {
            Sonuc sonuc = this.SilDetay(form.kod);
            if (sonuc.islemSonuc)
            {
                string sql = "DELETE FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc SilDetay(int imzaTanimKod)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.IMZATANIMKOD], "=", imzaTanimKod));

            return KomutCalistir(sql, 0);
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}
