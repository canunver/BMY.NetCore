using System;
using System.Data;
using TNS.PER.IsKurallari;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABIzinTur : TABOrtak
    {
        public TABIzinTur(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "PERIZINTUR";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            AD = 1,
            SURE = 2,
            GORULEBILIRLIK = 3,
            SIRA = 4,
            ATAKOD=5
        }

        static string[] ALANLAR = { "KOD", "AD", "SURE", "GORULEBILIRLIK" , "SIRA","ATAKOD" };

        public IDataReader Listele(IzinTur p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.ataKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ATAKOD], "=", p.ataKod));
            
            if (p.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + p.ad + "%"));

            if (p.sure > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURE], "=", p.sure));
            
            //Şifresiz giriş
            if (p.gorulebilirlik == 2)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GORULEBILIRLIK], "=", 1));
            
            if (p.sira > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRA], "=", p.sira));

            if (p.sadeceAtaKodlar)
            {                
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ATAKOD], " is ", (string)null));
                kosul = VeriErisim.KosulEkle(kosul, "or", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ATAKOD], " = ", 0));
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRA];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(IzinTur p)
        {
            string sql = "";
            p.kod = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "");

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.ad) + ",";
            sql += VeriErisim.DegerYap(p.sure) + ",";
            sql += VeriErisim.DegerYap(p.gorulebilirlik) + ",";
            sql += VeriErisim.DegerYap(p.sira) + ",";
            sql += VeriErisim.DegerYap(p.ataKod) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(IzinTur p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], p.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURE], p.sure) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GORULEBILIRLIK], p.gorulebilirlik) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRA], p.sira) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ATAKOD], p.ataKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(IzinTur p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc TumunuSil()
        {
            string sql = "DELETE FROM " + TABLOAD;
            return KomutCalistir(sql,0);
        }
    }
}
