using System;
using System.Data;
using TNS.PER.IsKurallari;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABIzinDonem : TABOrtak
    {
        public TABIzinDonem(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "PERDONEM";

        public enum ENUMAlanlar : int
        {
            DONEMID = 0,
            DONEMYIL = 1,
            DONEMBASLANGICTARIHI = 2,
            DONEMBITISTARIHI = 3,
            DONEMACIKLAMA = 4
        }

        static string[] ALANLAR = { "DONEMID", "DONEMYIL", "DONEMBASLANGICTARIHI", "DONEMBITISTARIHI", "DONEMACIKLAMA" };

        public IDataReader Listele(IzinDonem p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMID], "=", p.kod));

            if (p.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMYIL], "=", p.yil));

            if (!string.IsNullOrEmpty(p.donemBaslangicTarihi.ToString()))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMBASLANGICTARIHI], "=", p.donemBaslangicTarihi));

            if (!string.IsNullOrEmpty(p.donemBitisTarihi.ToString()))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMBITISTARIHI], "=", p.donemBitisTarihi));

            if (!string.IsNullOrEmpty(p.donemAciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMACIKLAMA], "LIKE", "%" + p.donemAciklama + "%"));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.DONEMID];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(IzinDonem p)
        {
            string sql = "";
            p.kod = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMID], "");

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.yil) + ",";
            sql += VeriErisim.DegerYap(p.donemBaslangicTarihi) + ",";
            sql += VeriErisim.DegerYap(p.donemBitisTarihi) + ",";
            sql += VeriErisim.DegerYap(p.donemAciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(IzinDonem p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DONEMYIL], p.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DONEMBASLANGICTARIHI], p.donemBaslangicTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DONEMBITISTARIHI], p.donemBitisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DONEMACIKLAMA], p.donemAciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMID], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(IzinDonem p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMID], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc TumunuSil()
        {
            string sql = "DELETE FROM " + TABLOAD;
            return KomutCalistir(sql,0);
        }
    }
}
