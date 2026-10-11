using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABHibeYatirimUygulamaSozlesme : TABOrtak
    {
        public TABHibeYatirimUygulamaSozlesme(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "TRMHIBEYATIRIMUYGULAMASOZLESME";

        public enum ENUMAlanlar
        {
            HIBEKOD = 0,
            SOZLESMEKOD = 1,
            HUKUKISTATU = 2,
            KIMLIKNO = 3,
            YUKLENICIAD = 4,
            SOZLESMETUR = 5,
            SOZLESMETARIH = 6,
            SOZLESMETUTAR = 7,
            SOZLESMEDURUM = 8
        }

        public static string[] ALANLAR = { "HIBEKOD", "SOZLESMEKOD", "HUKUKISTATU", "KIMLIKNO", "YUKLENICIAD",
                                           "SOZLESMETUR", "SOZLESMETARIH", "SOZLESMETUTAR", "SOZLESMEDURUM" };

        public Sonuc Kaydet(HibeYatirimUygulamaSozlesme us)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar(string.Empty, ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(us.hibeKod) + ",";
            sql += VeriErisim.DegerYap(us.sozlesmeKod) + ",";
            sql += VeriErisim.DegerYap(us.hukukiStatu) + ",";
            sql += VeriErisim.DegerYap(us.kimlikNo) + ",";
            sql += VeriErisim.DegerYap(us.yukleniciAd) + ",";
            sql += VeriErisim.DegerYap((int)us.sozlesmeTur) + ",";
            sql += VeriErisim.DegerYap(us.sozlesmeTarih) + ",";
            sql += VeriErisim.DegerYap(us.sozlesmeTutar) + ",";
            sql += VeriErisim.DegerYap((int)us.sozlesmeDurum) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(HibeYatirimUygulamaSozlesme us)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", us.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", us.sozlesmeKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilHepsi(HibeYatirimUygulamaSozlesme us)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", us.hibeKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc Guncelle(HibeYatirimUygulamaSozlesme us)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HUKUKISTATU], us.hukukiStatu) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KIMLIKNO], us.kimlikNo) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.YUKLENICIAD], us.yukleniciAd) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMETUR], (int)us.sozlesmeTur) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMETARIH], us.sozlesmeTarih) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMETUTAR], us.sozlesmeTutar) + ",";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEDURUM], (int)us.sozlesmeDurum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", us.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", us.sozlesmeKod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(HibeYatirimUygulamaSozlesme us, bool iptalVeHataliHaric)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(string.Empty, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", us.hibeKod));
            if (us.sozlesmeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", us.sozlesmeKod));
            if ((int)us.sozlesmeTur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMETUR], "=", (int)us.sozlesmeTur));
            if (iptalVeHataliHaric)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEDURUM], "<>", (int)ENUMHibeYatirimUygulamaSozlesmeDurum.IPTAL));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEDURUM], "<>", (int)ENUMHibeYatirimUygulamaSozlesmeDurum.HATALI));
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleUygulamaSozlesmesiDevamEden(int hibeKod)
        {
            string sql = "SELECT COUNT( " + ALANLAR[(int)ENUMAlanlar.HIBEKOD] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEDURUM], "=", 1));

            return veriErisim.DataReaderDondur(sql);
        }

        public int SayacDegerAl(int hibeKod)
        {
            return Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], null, null, new string[] { ALANLAR[(int)ENUMAlanlar.HIBEKOD] }, new int[] { hibeKod });
        }

        public decimal SozlesmeTutarToplamAl(int hibeKod, int sozlesmeKod, bool kendisi, ENUMHibeYatirimUygulamaSozlesmeTur sozlesmeTur = ENUMHibeYatirimUygulamaSozlesmeTur.INSAAT)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(" + ALANLAR[(int)ENUMAlanlar.SOZLESMETUTAR] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));
            if (kendisi)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", sozlesmeKod));
            else
            {
                if (sozlesmeKod > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "<>", sozlesmeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEDURUM], "<>", (int)ENUMHibeYatirimUygulamaSozlesmeDurum.IPTAL));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEDURUM], "<>", (int)ENUMHibeYatirimUygulamaSozlesmeDurum.HATALI));
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMETUR], "=", (int)sozlesmeTur));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMETUR], "=", (int)ENUMHibeYatirimUygulamaSozlesmeTur.ANAHTARTESLIM));
                sql += ")";
            }

            return veriErisim.TekKolonSQLDecimal(sql);
        }

        public int UygulamaSozlesmeAdedi(int hibeKod)
        {
            string sql = "SELECT COUNT(*)" + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc GuncelleDurum(HibeYatirimUygulamaSozlesme us)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEDURUM], (int)us.sozlesmeDurum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", us.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", us.sozlesmeKod));

            return KomutCalistir(sql, 1);
        }
        public IDataReader ListeleBitenUygulamaSozlesmelerininToplamTutarini(HibeYatirimUygulamaSozlesme us)
        {
            string sql = " SELECT SUM(" + ALANLAR[(int)ENUMAlanlar.SOZLESMETUTAR] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", us.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOZLESMEDURUM], "=", (int)us.sozlesmeDurum));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}