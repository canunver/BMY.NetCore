using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABSozluk : TABOrtak
    {
        public TABSozluk(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYSOZLUK";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            ANAKOD = 1,
            KAYITKOD = 2,
            KAYITAD = 3,
            KISAAD = 4,
            SEVIYE = 5,
            HITAPID = 6,
        }
        public static string[] ALANLAR = { "KOD", "ANAKOD", "KAYITKOD", "KAYITAD", "KISAAD", "SEVIYE", "HITAPID" };

        public IDataReader Listele(Sozluk p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", SOZLUK2.KAYITAD"  + " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABLOAD + " AS SOZLUK2 on SOZLUK2.KOD = " + TABLOAD + "." +  ALANLAR[(int)ENUMAlanlar.ANAKOD];

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.anaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", p.anaKod));

            if (!string.IsNullOrEmpty(p.hitapID))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HITAPID], "=", p.hitapID));

            if (!string.IsNullOrEmpty(p.kayitKod))
            {
                if (p.birebir)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITKOD], "=", p.kayitKod));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITKOD], "LIKE", p.kayitKod + "%"));
            }

            if (p.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITAD], "LIKE", "%" + p.ad + "%"));

            if (p.kisaAd != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KISAAD], "LIKE", "%" + p.kisaAd + "%"));

            if (p.seviye > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SEVIYE], ">=", p.seviye));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KAYITKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(Sozluk p)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "UPDATE " + TABLOAD + " SET ";
            if (p.anaKod > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], p.anaKod) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITKOD], p.kayitKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITAD], p.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISAAD], p.kisaAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SEVIYE], p.seviye) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HITAPID], p.hitapID);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(p.kod) + ",";
                if(p.anaKod>0)
                    sql += VeriErisim.DegerYap(p.anaKod) + ",";
                else
                    sql += VeriErisim.DegerYap((string)null) + ",";
                sql += VeriErisim.DegerYap(p.kayitKod) + ",";
                sql += VeriErisim.DegerYap(p.ad) + ",";
                sql += VeriErisim.DegerYap(p.kisaAd) + ",";
                sql += VeriErisim.DegerYap(p.seviye) + ",";
                sql += VeriErisim.DegerYap(p.hitapID) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc Sil(Sozluk p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader MaxKayitKodGetir(int kod)
        {
            string sql = "";

            sql = "SELECT MAX(" +  ALANLAR[(int)ENUMAlanlar.KAYITKOD] + ") FROM " + TABLOAD;

            if (kod == 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SEVIYE], "=", 1));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", kod));
         
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
