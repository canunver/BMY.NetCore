using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABHibeSulamaDiger : TABOrtak
    {
        public TABHibeSulamaDiger(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "TRMHIBESULAMADIGER";
        public enum ENUMAlanlar
        {
            HIBEKOD = 0,
            TIP = 1,
            SULAMAGRUPKOD = 2,
            PARSELID = 3,
            PARSELNO = 4,
            PARSELALAN = 5,
            PARSELSULANACAKALAN = 6,
            PARSELSUKAYNAKTIP = 7,
            PARSELSUKAYNAKDEBI = 8,
            KUYUNO = 9,
            KUYUDEBI = 10,
            KUYUPOMPAGUCU = 11,
            KUYUTRAFOGUCU = 12,
            KUYUSULANACAKALAN = 13,
            PARSELSUKAYNAGICESIDI = 14,
            PARSELSUTEMINYONTEMI = 15,
            PARSELSUKALITESINIFI = 16,
            PARSELSUKAYNAGIDURUMU = 17,
            PARSELMULKIYETI = 18,
            TOPRAKBUNYESINIFI = 19,
            TOPRAKTARLAKAPASITESI = 20,
            TOPRAKINFILTRASYONHIZI = 21,
            BITKIURUNCESIDI = 22,
            BITKITESISYILI = 23,
            BITKIDIKIMNORMU = 24,
            BITKIKOKDERINLIGI = 25,
            BITKISUIHTIYACI = 26,
            FIRMAVERGINO = 27,
            FIRMAAD = 28,
            FATURANO = 29,
            FATURATARIH = 30,
            FATURAACIKLAMA = 31
        }
        public static string[] ALANLAR = { "HIBEKOD", "TIP", "SULAMAGRUPKOD","PARSELID","PARSELNO", "PARSELALAN", "PARSELSULANACAKALAN", "PARSELSUKAYNAKTIP",
                                           "PARSELSUKAYNAKDEBI", "KUYUNO", "KUYUDEBI", "KUYUPOMPAGUCU", "KUYUTRAFOGUCU", "KUYUSULANACAKALAN", "PARSELSUKAYNAGICESIDI", "PARSELSUTEMINYONTEMI", "PARSELSUKALITESINIFI",
                                           "PARSELSUKAYNAGIDURUMU","PARSELMULKIYETI", "TOPRAKBUNYESINIFI", "TOPRAKTARLAKAPASITESI", "TOPRAKINFILTRASYONHIZI", "BITKIURUNCESIDI", "BITKITESISYILI", "BITKIDIKIMNORMU",
                                           "BITKIKOKDERINLIGI","BITKISUIHTIYACI","FIRMAVERGINO","FIRMAAD","FATURANO","FATURATARIH","FATURAACIKLAMA" };

        public Sonuc Kaydet(HibeSulamaDiger hsd)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar(string.Empty, ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(hsd.hibeKod) + ",";
            sql += VeriErisim.DegerYap(hsd.tip) + ",";
            sql += VeriErisim.DegerYap(hsd.sulamaGrupKod) + ",";
            sql += VeriErisim.DegerYap(hsd.parselID) + ",";
            sql += VeriErisim.DegerYap(hsd.parselNo) + ",";
            sql += VeriErisim.DegerYap(hsd.parselAlan) + ",";
            sql += VeriErisim.DegerYap(hsd.parselSulanacakAlan) + ",";
            sql += VeriErisim.DegerYap(hsd.parselSuKaynakTip) + ",";
            sql += VeriErisim.DegerYap(hsd.parselSuKaynakDebi) + ",";
            sql += VeriErisim.DegerYap(hsd.kuyuNo) + ",";
            sql += VeriErisim.DegerYap(hsd.kuyuDebi) + ",";
            sql += VeriErisim.DegerYap(hsd.kuyuPompaGucu) + ",";
            sql += VeriErisim.DegerYap(hsd.kuyuTrafoGucu) + ",";
            sql += VeriErisim.DegerYap(hsd.kuyuSulanacakAlan) + ",";
            sql += VeriErisim.DegerYap(hsd.parselSuKaynakCesidi) + ",";
            sql += VeriErisim.DegerYap(hsd.parselSuTeminYonetimi) + ",";
            sql += VeriErisim.DegerYap(hsd.parselSuKaliteSinifi) + ",";
            sql += VeriErisim.DegerYap(hsd.parselSuKaynagiDurumu) + ",";
            sql += VeriErisim.DegerYap(hsd.parselMulkiyeti) + ",";
            sql += VeriErisim.DegerYap(hsd.toprakBunyeSinifi) + ",";
            sql += VeriErisim.DegerYap(hsd.toprakTarlaKapasitesi) + ",";
            sql += VeriErisim.DegerYap(hsd.toprakInfiltrasyonHizi) + ",";
            sql += VeriErisim.DegerYap(hsd.bitkiUrunCesidi) + ",";
            sql += VeriErisim.DegerYap(hsd.bitkiTesisYili) + ",";
            sql += VeriErisim.DegerYap(hsd.bitkiDikimNormu) + ",";
            sql += VeriErisim.DegerYap(hsd.bitkiKokDerinligi) + ",";
            sql += VeriErisim.DegerYap(hsd.bitkiSuIhtiyaci) + ",";
            sql += VeriErisim.DegerYap(hsd.firmaVergiNo) + ",";
            sql += VeriErisim.DegerYap(hsd.firmaAd) + ",";
            sql += VeriErisim.DegerYap(hsd.faturaNo) + ",";
            sql += VeriErisim.DegerYap(hsd.faturaTarihi) + ",";
            sql += VeriErisim.DegerYap(hsd.faturaAciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(HibeSulamaDiger bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMAVERGINO], bilgi.firmaVergiNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMAAD], bilgi.firmaAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURANO], bilgi.faturaNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURATARIH], bilgi.faturaTarihi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURAACIKLAMA], bilgi.faturaAciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", bilgi.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SULAMAGRUPKOD], "=", bilgi.sulamaGrupKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PARSELID], "=", bilgi.parselID));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int hibeKod, string parselID)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));

            if (parselID != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.PARSELID], "=", parselID));

            return KomutCalistir(sql, 0);
        }

        public Sonuc Reddet(int hibeKod, string parselID)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARSELSUKAYNAKTIP], 2);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.PARSELID], "=", parselID));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(int hibeKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(string.Empty, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TIP];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}