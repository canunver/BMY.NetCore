using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{

    public class TABHesapPlaniBarkod : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHesapPlaniBarkod(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASHESAPPLANBARKOD";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            TASBARKODID = 0,
            TASHESAPNO = 1,
            BARKOD = 2,
            MUHASEBEKOD = 3,
            HARCAMABIRIMKOD = 4
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "TASBARKODID", "TASHESAPNO", "BARKOD", "MUHASEBEKOD", "HARCAMABIRIMKOD" };

        public Sonuc Kaydet(TasHesapPlaniBarkod hpBarkod)
        {
            hpBarkod.tasBarkodID = this.KodAl();

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(hpBarkod.tasBarkodID) + ",";
            sql += VeriErisim.DegerYap(hpBarkod.tasHesapNo.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(hpBarkod.barkod) + ",";
            sql += VeriErisim.DegerYap(hpBarkod.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(hpBarkod.harcamaBirimKod) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(TasHesapPlaniBarkod hpBarkod)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASHESAPNO], hpBarkod.tasHesapNo.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BARKOD], hpBarkod.barkod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], hpBarkod.muhasebeKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], hpBarkod.harcamaBirimKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASBARKODID], "=", hpBarkod.tasBarkodID));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(TasHesapPlaniBarkod hpBarkod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TASHESAPNO], "=", hpBarkod.tasHesapNo.Replace(".", "")));

            if (!string.IsNullOrEmpty(hpBarkod.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", hpBarkod.muhasebeKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(hpBarkod.harcamaBirimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", hpBarkod.harcamaBirimKod.Replace(".", "")));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BARKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(string hpBarkod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASBARKODID], "=", hpBarkod));

            return KomutCalistir(sql, 1);
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.TASBARKODID] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}