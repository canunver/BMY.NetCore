using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    public class TABSayistayVeri : TABOrtak
    {
        public TABSayistayVeri(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHSAYISTAYVERI";
        static string[] ALANLAR = { "KOD", "YIL", "AY", "MUHASEBE", "TUR", "VERILER" };
        enum ENUMAlanlar
        {
            KOD = 0,
            YIL = 1,
            AY = 2,
            MUHASEBE = 3,
            TUR = 4,
            VERILER = 5,
        }

        public Sonuc Kaydet(SayistayVeri veri)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(veri.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(veri.yil) + ", ";
            sql = sql + VeriErisim.DegerYap(veri.ay) + ", ";
            sql = sql + VeriErisim.DegerYap(veri.muhasebe) + ", ";
            sql = sql + VeriErisim.DegerYap(veri.tur) + ", ";
            sql = sql + VeriErisim.DegerYap(veri.veriler) + ") ";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(SayistayVeri bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            if (bilgi.kod != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", bilgi.ay));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBE], "=", bilgi.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));
            }

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(SayistayVeri bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", bilgi.ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBE], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
