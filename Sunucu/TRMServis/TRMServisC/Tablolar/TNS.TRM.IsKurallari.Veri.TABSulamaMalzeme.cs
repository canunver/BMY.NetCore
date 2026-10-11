using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABSulamaMalzeme : TABOrtak
    {
        public TABSulamaMalzeme(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "TRMSULAMAMALZEME";
        public enum ENUMAlanlar
        {
            KOD = 0,
            AD = 1
        }
        public static string[] ALANLAR = { "KOD", "AD" };

        public Sonuc Kaydet(SulamaMalzeme m)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar(string.Empty, ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(m.kod) + ",";
            sql += VeriErisim.DegerYap(m.ad) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int kod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(SulamaMalzeme m)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(string.Empty, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (m.kod > 0)
                kosul = VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KOD], "=", m.kod);
            if (!string.IsNullOrEmpty(m.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + m.ad + "%"));

            if (m.gruplar != null && m.gruplar.Length > 0)
            {
                string icSql = "SELECT 1 FROM " + TABLOADGRUP;
                icSql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABLOADGRUP + "." + ALANLARGRUP[(int)ENUMAlanlarGrup.MALZEMEKOD];
                if (m.gruplar[0].grupKod > 0)
                    icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARGRUP[(int)ENUMAlanlarGrup.SULAMAGRUBUKOD], "=", m.gruplar[0].grupKod));

                kosul = VeriErisim.KosulEkle(kosul, "AND", "EXISTS (" + icSql + ")");
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public int YeniMalzemeKodVer()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")";
            sql += " FROM " + TABLOAD;

            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        public static string TABLOADGRUP = "TRMSULAMAMALZEMEGRUP";
        public enum ENUMAlanlarGrup
        {
            MALZEMEKOD = 0,
            SULAMAGRUBUKOD = 1,
            COKLUSATIR = 2
        }
        public static string[] ALANLARGRUP = { "MALZEMEKOD", "SULAMAGRUBUKOD", "COKLUSATIR" };

        public Sonuc KaydetGrup(SulamaMalzemeGrup grup)
        {
            string sql = "INSERT INTO " + TABLOADGRUP + " (" + VeriErisim.TumAlanlar(string.Empty, ALANLARGRUP) + ") VALUES (";
            sql += VeriErisim.DegerYap(grup.malzemeKod) + ",";
            sql += VeriErisim.DegerYap(grup.grupKod) + ",";
            sql += VeriErisim.DegerYap(grup.cokluSatir ? 1 : 0) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilGrup(int malzemeKod)
        {
            string sql = "DELETE FROM " + TABLOADGRUP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLARGRUP[(int)ENUMAlanlarGrup.MALZEMEKOD], "=", malzemeKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleGrup(int malzemeKod, int grupKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(string.Empty, ALANLARGRUP) + " FROM " + TABLOADGRUP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLARGRUP[(int)ENUMAlanlarGrup.MALZEMEKOD], "=", malzemeKod));
            if (grupKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARGRUP[(int)ENUMAlanlarGrup.SULAMAGRUBUKOD], "=", grupKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public static string TABLOADOZELLIK = "TRMSULAMAMALZEMEOZELLIK";
        public enum ENUMAlanlarOzellik
        {
            MALZEMEKOD = 0,
            KOD = 1,
            AD = 2,
            MIKTARKOLON = 3,
            MANUELGIRIS = 4,
            KOLONTIP = 5
        }
        public static string[] ALANLAROZELLIK = { "MALZEMEKOD", "KOD", "AD", "MIKTARKOLON", "MANUELGIRIS", "KOLONTIP" };

        public Sonuc KaydetOzellik(SulamaMalzemeOzellik ozellik)
        {
            string sql = "INSERT INTO " + TABLOADOZELLIK + " (" + VeriErisim.TumAlanlar(string.Empty, ALANLAROZELLIK) + ") VALUES (";
            sql += VeriErisim.DegerYap(ozellik.malzemeKod) + ",";
            sql += VeriErisim.DegerYap(ozellik.kod) + ",";
            sql += VeriErisim.DegerYap(ozellik.ad) + ",";
            sql += VeriErisim.DegerYap(ozellik.miktarKolon ? 1 : 0) + ",";
            sql += VeriErisim.DegerYap(ozellik.manuelGiris ? 1 : 0) + ",";
            sql += VeriErisim.DegerYap((int)ozellik.kolonTip) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilOzellik(int malzemeKod)
        {
            string sql = "DELETE FROM " + TABLOADOZELLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAROZELLIK[(int)ENUMAlanlarOzellik.MALZEMEKOD], "=", malzemeKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleOzellik(int malzemeKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(string.Empty, ALANLAROZELLIK) + " FROM " + TABLOADOZELLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAROZELLIK[(int)ENUMAlanlarOzellik.MALZEMEKOD], "=", malzemeKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public int MiktarKolonKodBul(int malzemeKod)
        {
            string sql = "SELECT " + ALANLAROZELLIK[(int)ENUMAlanlarOzellik.KOD] + " FROM " + TABLOADOZELLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAROZELLIK[(int)ENUMAlanlarOzellik.MALZEMEKOD], "=", malzemeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAROZELLIK[(int)ENUMAlanlarOzellik.MIKTARKOLON], "=", 1));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public static string TABLOADOZELLIKDEGER = "TRMSULAMAMALZEMEOZELLIKDEGER";
        public enum ENUMAlanlarOzellikDeger
        {
            MALZEMEKOD = 0,
            OZELLIKKOD = 1,
            KOD = 2,
            AD = 3
        }
        public static string[] ALANLAROZELLIKDEGER = { "MALZEMEKOD", "OZELLIKKOD", "KOD", "AD" };

        public Sonuc KaydetOzellikDeger(SulamaMalzemeOzellikDeger deger)
        {
            string sql = "INSERT INTO " + TABLOADOZELLIKDEGER + " (" + VeriErisim.TumAlanlar(string.Empty, ALANLAROZELLIKDEGER) + ") VALUES (";
            sql += VeriErisim.DegerYap(deger.malzemeKod) + ",";
            sql += VeriErisim.DegerYap(deger.ozellikKod) + ",";
            sql += VeriErisim.DegerYap(deger.kod) + ",";
            sql += VeriErisim.DegerYap(deger.ad) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilOzellikDeger(int malzemeKod)
        {
            string sql = "DELETE FROM " + TABLOADOZELLIKDEGER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAROZELLIKDEGER[(int)ENUMAlanlarOzellikDeger.MALZEMEKOD], "=", malzemeKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleOzellikDeger(int malzemeKod, int ozellikKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(string.Empty, ALANLAROZELLIKDEGER) + " FROM " + TABLOADOZELLIKDEGER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAROZELLIKDEGER[(int)ENUMAlanlarOzellikDeger.MALZEMEKOD], "=", malzemeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAROZELLIKDEGER[(int)ENUMAlanlarOzellikDeger.OZELLIKKOD], "=", ozellikKod));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}