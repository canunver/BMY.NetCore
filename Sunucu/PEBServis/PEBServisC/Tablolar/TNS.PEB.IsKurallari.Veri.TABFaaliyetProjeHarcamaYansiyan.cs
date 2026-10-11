using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using TNS.PEB;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABFaaliyetProjeHarcamaYansiyan : TABOrtak
    {
        public TABFaaliyetProjeHarcamaYansiyan(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "PEBFPHARCAMAYANSIYAN";
        enum ENUMAlanlar : int
        {
            YIL = 0,
            AY = 1,
            FPKURKOD = 2,
            FPKOD = 3,
            SMKURKOD = 4,
            SMKOD = 5,
            AGIRLIK = 6,
            HARCAMA = 7
        }
        static string[] ALANLAR ={ "YIL", "AY", "FPKURKOD", "FPKOD", "SMKURKOD", "SMKOD", "AGIRLIK", "HARCAMA" };

        public Sonuc Kaydet(FaaliyetProjeHarcamaYansiyan h)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(h.yil) + ",";
            sql += VeriErisim.DegerYap(h.ay) + ",";
            sql += VeriErisim.DegerYap(h.fp.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(h.fp.fpKod) + ",";
            sql += VeriErisim.DegerYap(h.sm.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(h.sm.smKod) + ",";
            sql += VeriErisim.DegerYap(h.agirlik) + ",";
            sql += VeriErisim.DegerYap(h.harcama) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int yil, int ay)
        {
            FaaliyetProjeHarcamaYansiyan h = new FaaliyetProjeHarcamaYansiyan();
            h.yil = yil;
            h.ay = ay;

            return Sil(h);
        }

        public Sonuc Sil(FaaliyetProjeHarcamaYansiyan h)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", h.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", h.ay));
            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(FaaliyetProjeHarcamaYansiyan h)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", h.yil));

            if (h.ay > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AY], "=", h.ay));

            if (!string.IsNullOrEmpty(h.fp.kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FPKURKOD], "LIKE", h.fp.kurKod.Replace(".", "") + "%"));

            if (h.fp.fpKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FPKOD], "=", h.fp.fpKod));

            if (!string.IsNullOrEmpty(h.sm.kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SMKURKOD], "LIKE", h.sm.kurKod.Replace(".", "") + "%"));

            if (h.sm.smKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SMKOD], "=", h.sm.smKod));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}