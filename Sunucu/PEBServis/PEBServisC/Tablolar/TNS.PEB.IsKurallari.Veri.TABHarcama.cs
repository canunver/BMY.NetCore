using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using TNS.BKD;
using TNS.PEB;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABHarcama : TABOrtak
    {
        public TABHarcama(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "PEBHARCAMA";
        enum ENUMAlanlar : int
        {
            YIL = 0,
            AY = 1,
            MALIYETKURKOD = 2,
            MALIYETKOD = 3,
            SABITMI = 4,
            KURKOD = 5,
            FONKOD = 6,
            FINKOD = 7,
            EKOKOD = 8,
            MALIYETFONKOD = 9,
            AGIRLIK = 10,
            HARCAMA = 11
        }
        static string[] ALANLAR ={ "YIL", "AY", "MALIYETKURKOD", "MALIYETKOD", "SABITMI", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "MALIYETFONKOD", "AGIRLIK", "HARCAMA" };

        public Sonuc Kaydet(Harcama h)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(h.yil) + ",";
            sql += VeriErisim.DegerYap(h.ay) + ",";
            sql += VeriErisim.DegerYap(h.maliyetKurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(h.maliyetKod) + ",";
            sql += VeriErisim.DegerYap(h.sabitMi) + ",";
            sql += VeriErisim.DegerYap(h.tertip.kodlar[0].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(h.tertip.kodlar[1].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(h.tertip.kodlar[2].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(h.tertip.kodlar[3].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(h.maliyetFonKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(h.agirlik) + ",";
            sql += VeriErisim.DegerYap(h.harcama) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int yil, int ay, Tertip tertip)
        {
            Harcama h = new Harcama();
            h.yil = yil;
            h.ay = ay;
            h.tertip = tertip;

            return Sil(h);
        }

        public Sonuc Sil(Harcama h)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", h.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", h.ay));
            if (!string.IsNullOrEmpty(h.tertip.kodlar[0]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", h.tertip.kodlar[0].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(h.tertip.kodlar[1]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "LIKE", h.tertip.kodlar[1].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(h.tertip.kodlar[2]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "LIKE", h.tertip.kodlar[2].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(h.tertip.kodlar[3]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", h.tertip.kodlar[3].Replace(".", "") + "%"));
            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(Harcama h)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", h.yil));
            if (h.ay > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AY], "=", h.ay));
            if (!string.IsNullOrEmpty(h.tertip.kodlar[0]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", h.tertip.kodlar[0].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(h.tertip.kodlar[1]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], "LIKE", h.tertip.kodlar[1].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(h.tertip.kodlar[2]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FINKOD], "LIKE", h.tertip.kodlar[2].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(h.tertip.kodlar[3]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", h.tertip.kodlar[3].Replace(".", "") + "%"));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AY];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FONKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FINKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader SabitMaliyetHarcamaToplamlari(Harcama h)
        {
            h.sabitMi = (int)ENUMOdenekTur.SabitMaliyet;
            return HarcamaToplamlari(h);
        }

        public IDataReader FaaliyetProjeHarcamaToplamlari(Harcama h)
        {
            h.sabitMi = (int)ENUMOdenekTur.FaaliyetProje;
            return HarcamaToplamlari(h);
        }

        public IDataReader HarcamaToplamlari(Harcama h)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.MALIYETKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.SABITMI];
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + ALANLAR[(int)ENUMAlanlar.HARCAMA] + ")";
            sql += " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", h.yil));
            
            if (h.ay > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AY], "=", h.ay));

            if (!string.IsNullOrEmpty(h.maliyetKurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD], "LIKE", h.maliyetKurKod.Replace(".", "") + "%"));

            if (h.maliyetKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKOD], "=", h.maliyetKod));

            if (h.sabitMi >= 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABITMI], "=", h.sabitMi));

            if (!string.IsNullOrEmpty(h.tertip.kodlar[0]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", h.tertip.kodlar[0].Replace(".", "") + "%"));
            
            if (!string.IsNullOrEmpty(h.tertip.kodlar[1]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], "LIKE", h.tertip.kodlar[1].Replace(".", "") + "%"));
            
            if (!string.IsNullOrEmpty(h.tertip.kodlar[2]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FINKOD], "LIKE", h.tertip.kodlar[2].Replace(".", "") + "%"));
            
            if (!string.IsNullOrEmpty(h.tertip.kodlar[3]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", h.tertip.kodlar[3].Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(h.maliyetFonKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETFONKOD], "LIKE", h.maliyetFonKod.Replace(".", "") + "%"));

            sql += " GROUP BY " + ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.MALIYETKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.SABITMI];

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.MALIYETKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.SABITMI];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}