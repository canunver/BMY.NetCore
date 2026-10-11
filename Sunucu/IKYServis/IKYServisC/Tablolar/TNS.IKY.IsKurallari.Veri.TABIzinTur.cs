using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABIzinTur : TABOrtak
    {
        public TABIzinTur(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "IKYSOZLUKIZINTUR";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            AD = 1,
            SURE = 2,
            GORULEBILIRLIK = 3,
            SIRA = 4
        }

        static string[] ALANLAR = { "KOD", "AD", "SURE", "GORULEBILIRLIK" , "SIRA" };

        public IDataReader Listele(IzinTur p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (!string.IsNullOrWhiteSpace(p.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", p.ad + "%"));

            if (p.sure > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURE], "=", p.sure));
            
            
            if (p.gorulebilirlik == 2) //2. ise sadece giriş yapmış kullanıcılar. Bu durumda görülebilirlik 1 olacak
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GORULEBILIRLIK], "=", 1));
            
            if (p.sira > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRA], "=", p.sira));


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
            sql += VeriErisim.DegerYap(p.sira) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(IzinTur p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], p.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURE], p.sure) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GORULEBILIRLIK], p.gorulebilirlik) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRA], p.sira);
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
