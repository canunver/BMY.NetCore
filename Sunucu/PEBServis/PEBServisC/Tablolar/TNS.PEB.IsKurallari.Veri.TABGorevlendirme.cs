using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABGorevlendirme : TABOrtak
    {
        public TABGorevlendirme(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRGOREVLENDIRME";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            EKIPKOD = 2,
            SIRANO = 3,
            PERSONELAD = 4,
            GOREVACIKLAMA = 5,
            GOREVYER = 6,
            TEL = 7,
            EMAIL = 8
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "EKIPKOD", "SIRANO", "PERSONELAD", "GOREVACIKLAMA", "GOREVYER", "TEL", "EMAIL" };

        public Sonuc Kaydet(Gorevlendirme g)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(g.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(g.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(g.ekipKod) + ",";
            sql += VeriErisim.DegerYap(g.siraNo) + ",";
            //sql += VeriErisim.DegerYap(g.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(g.personelAd) + ",";
            sql += VeriErisim.DegerYap(g.gorevAciklama) + ",";
            sql += VeriErisim.DegerYap(g.gorevYer) + ",";
            sql += VeriErisim.DegerYap(g.tel) + ",";
            sql += VeriErisim.DegerYap(g.eMail) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(Gorevlendirme g)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", g.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", g.orgBirimKod));
            if (g.ekipKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKIPKOD], "=", g.ekipKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(Gorevlendirme g, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (g.donem != null && g.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", g.donem.donemNo);
            if (!sorgu && g.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", g.orgBirimKod));
            if (g.ekipKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKIPKOD], "=", g.ekipKod));
            //if (!string.IsNullOrEmpty(g.tcKimlikNo))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", g.tcKimlikNo));
            if (!string.IsNullOrEmpty(g.personelAd))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PERSONELAD], "LIKE", "%" + g.personelAd + "%"));
            if (!string.IsNullOrEmpty(g.gorevAciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOREVACIKLAMA], "LIKE", "%" + g.gorevAciklama + "%"));
            //if (g.baslamaTarih != null)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLAMATARIH], ">=", g.baslamaTarih));
            //if (g.bitisTarih != null)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIH], "<=", g.bitisTarih));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], g.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}