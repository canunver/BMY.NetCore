using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{

    public class TABTCCBEntegrasyon : TABOrtak
    {

        public TABTCCBEntegrasyon(VeriErisim veriErisim) : base(veriErisim) { }


        static string TABLOAD = "TASTCCBENTEGRASYON";

        public enum ENUMAlanlar
        {
            KOD = 0,
            YIL = 1,
            MUHASEBEKOD = 2,
            HARCAMABIRIMKOD = 3,
            FISNO = 4,
            REFEREANSNO = 5,
            SILINDI = 6
        }
        static string[] ALANLAR = { "KOD", "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "REFERANSNO", "SILINDI" };

        public Sonuc Kaydet(TCCBEntegrasyon bilgi)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.fisNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.referansNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.silindi) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(TCCBEntegrasyon bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SILINDI], 1);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", bilgi.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", bilgi.fisNo));

            return KomutCalistir(sql, 0);
        }


        public IDataReader Getir(TasinirIslemForm bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", bilgi.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", bilgi.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SILINDI], "=", 0));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}