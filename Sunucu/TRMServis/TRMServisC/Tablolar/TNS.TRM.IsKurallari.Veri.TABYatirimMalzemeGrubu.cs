using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABYatirimMalzemeGrubu : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYatirimMalzemeGrubu(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADANA = "TRMYATIRIMMALZEMEGRUPANA";
        public static string TABLOAD = "TRMYATIRIMMALZEMEGRUP";

        public enum ENUMAlanlar
        {
            KOD = 0,
            ANAKOD = 1,
            ADI = 2,
            AKTIF = 3,
        }
        public static string[] ALANLAR = { "KOD", "ANAKOD", "ADI", "AKTIF" };

        public enum ENUMAlanlarAna
        {
            KOD = 0,
            ADI = 1,
            AKTIF = 2,
            COKLU = 3,
        }
        public static string[] ALANLARANA = { "KOD", "ADI", "AKTIF", "COKLU" };

        public Sonuc Kaydet(YatirimMalzemeTur bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], bilgi.anaKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], bilgi.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], bilgi.aktif);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.anaKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.ad) + ",";
                sql += VeriErisim.DegerYap(bilgi.aktif) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc Sil(YatirimMalzemeTur bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(YatirimMalzemeTur bilgi)
        {
            string kosul = "";

            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",TRMYATIRIMMALZEMEGRUPANA.ADI FROM " + TABLOAD;
            sql += " INNER JOIN TRMYATIRIMMALZEMEGRUPANA ON TRMYATIRIMMALZEMEGRUP.ANAKOD=TRMYATIRIMMALZEMEGRUPANA.KOD";

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.anaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", bilgi.anaKod));

            if (!string.IsNullOrEmpty(bilgi.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ADI], " LIKE ", "%" + bilgi.ad + "%"));

            if (bilgi.aktif > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AKTIF], "=", bilgi.aktif));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ADI];

            return veriErisim.DataReaderDondur(sql);
        }

        public int EnBuyukKod()
        {
            string sql = "";

            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")";
            sql += " FROM " + TABLOAD;

            int deger = veriErisim.TekKolonSQLInt(sql);

            return deger + 1;
        }

        public Sonuc KaydetAna(YatirimMalzemeTur bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "UPDATE " + TABLOADANA + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARANA[(int)ENUMAlanlarAna.ADI], bilgi.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARANA[(int)ENUMAlanlarAna.AKTIF], bilgi.aktif) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARANA[(int)ENUMAlanlarAna.COKLU], bilgi.coklu);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOADANA + " (" + VeriErisim.TumAlanlar("", ALANLARANA) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.ad) + ",";
                sql += VeriErisim.DegerYap(bilgi.aktif) + ",";
                sql += VeriErisim.DegerYap(bilgi.coklu) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc SilAna(YatirimMalzemeTur bilgi)
        {
            string sql = "DELETE FROM " + TABLOADANA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleAna(YatirimMalzemeTur bilgi)
        {
            string kosul = "";

            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARANA) + " FROM " + TABLOADANA;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.KOD], "=", bilgi.kod));

            if (!string.IsNullOrEmpty(bilgi.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.ADI], " LIKE ", "%" + bilgi.ad + "%"));

            if (bilgi.aktif > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.AKTIF], "=", bilgi.aktif));

            if (bilgi.coklu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.COKLU], "=", bilgi.coklu));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARANA[(int)ENUMAlanlarAna.ADI];

            return veriErisim.DataReaderDondur(sql);
        }

        public int EnBuyukKodAna()
        {
            string sql = "";

            sql = "SELECT MAX(" + ALANLARANA[(int)ENUMAlanlarAna.KOD] + ")";
            sql += " FROM " + TABLOADANA;

            int deger = veriErisim.TekKolonSQLInt(sql);

            return deger + 1;
        }
    }
}
