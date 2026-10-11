using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABGiyecekTechizatDetay : TABOrtak
    {
        public TABGiyecekTechizatDetay(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        static string TABLOAD = "TASGIYECEKTECHIZATDETAY";

        public enum ENUMAlanlar : int
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            HARCAMABIRIMKOD = 2,
            BELGENO = 3,
            HESAPPLANKOD = 4,
            PERSICILNO = 5,
            MIKTAR = 6,
            PERUNVAN = 7,
            PERADSOYAD = 8,
            TESLIMTARIHI = 9

        }

        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "BELGENO", "HESAPPLANKOD", "PERSICILNO", "MIKTAR", "PERUNVAN", "PERADSOYAD", "TESLIMTARIHI" };

        public Sonuc Kaydet(GiyecekTechizat tu, GiyecekTechizatDetay tud)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tu.yil) + ",";
            sql += VeriErisim.DegerYap(tu.muhasebe.kod) + ",";
            sql += VeriErisim.DegerYap(tu.harcamaBirimi.kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tu.belgeNo) + ",";
            sql += VeriErisim.DegerYap(tud.hesapPlani.hesapKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tud.perSicilNo) + ",";
            sql += VeriErisim.DegerYap(tud.miktar) + ",";
            sql += VeriErisim.DegerYap(tud.perUnvan) + ",";
            sql += VeriErisim.DegerYap(tud.perAdSoyad) + ",";
            sql += VeriErisim.DegerYap(tud.teslimTarih) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(GiyecekTechizat tu)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tu.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tu.muhasebe.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tu.harcamaBirimi.kod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", tu.belgeNo));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(GiyecekTechizat tu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASHESAPPLAN.ACIKLAMA, TASOLCUBIRIM.AD, TASPERSONEL.AD, TASPERSONEL.UNVAN";
            sql += " FROM " + TABLOAD + ", TASHESAPPLAN, TASOLCUBIRIM, TASPERSONEL";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD] + " = TASHESAPPLAN.KOD";
            sql += " AND TASHESAPPLAN.OLCUBIRIMKOD = TASOLCUBIRIM.KOD AND TASPERSONEL.KOD=TASGIYECEKTECHIZATDETAY.PERSICILNO";

            if (tu.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tu.yil));

            if (!string.IsNullOrEmpty(tu.muhasebe.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tu.muhasebe.kod));

            if (!string.IsNullOrEmpty(tu.harcamaBirimi.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tu.harcamaBirimi.kod.Replace(".", "")));

            if (!string.IsNullOrEmpty(tu.belgeNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "=", tu.belgeNo));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}