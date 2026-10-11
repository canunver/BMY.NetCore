using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABKadro : TABOrtak
    {
        public TABKadro(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "KADRO";
        public enum ENUMAlanlar : int
        {
            K_TESKILAT = 0,
            K_USTBIRIM = 1,
            K_SINIF = 2,
            K_UNVAN = 3,
            K_KADRO = 4,
            K_SAY_IL = 5,
            K_SAY_TIP = 6,
            K_SAY_ILCE = 7,
            K_ALT_IL = 8,
            K_ALT_TIP = 9,
            K_ALT_ILCE = 10,
            K_DOLU = 11,
            K_BOS = 12,
            K_ADET = 13,
            K_SGBISLEMTARIH = 14,
            K_TAHTENYIL = 15,
            K_TAHTENVIZETARIH = 16,
            K_TAHTENTEKLIFNO = 17,
            K_TAHTENICMALNO = 18,
            K_TAHTENSIRANO = 19,
            K_TAHSISTENKIS = 20,
        }
        static string[] ALANLAR ={ "K_TESKILAT", "K_USTBIRIM", "K_SINIF", "K_UNVAN", "K_KADRO", "K_SAY_IL", "K_SAY_TIP", "K_SAY_ILCE", "K_ALT_IL", "K_ALT_TIP", "K_ALT_ILCE", "K_DOLU", "K_BOS", "K_ADET", "K_SGBISLEMTARIH", "K_TAHTENYIL", "K_TAHTENVIZETARIH", "K_TAHTENTEKLIFNO", "K_TAHTENICMALNO", "K_TAHTENSIRANO", "K_TAHSISTENKIS" };

        public Sonuc Kaydet(Kadro p)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.teskilat) + ",";
            sql += VeriErisim.DegerYap(p.ustBirim) + ",";
            sql += VeriErisim.DegerYap(p.sinif) + ",";
            sql += VeriErisim.DegerYap(p.unvan) + ",";
            sql += VeriErisim.DegerYap(p.kadro) + ",";
            sql += VeriErisim.DegerYap(p.sayIl) + ",";
            sql += VeriErisim.DegerYap(p.sayTip) + ",";
            sql += VeriErisim.DegerYap(p.sayIlce) + ",";
            sql += VeriErisim.DegerYap(p.altIl) + ",";
            sql += VeriErisim.DegerYap(p.altTip) + ",";
            sql += VeriErisim.DegerYap(p.altIlce) + ",";
            sql += VeriErisim.DegerYap(p.dolu) + ",";
            sql += VeriErisim.DegerYap(p.bos) + ",";
            sql += VeriErisim.DegerYap(p.adet) + ",";
            sql += VeriErisim.DegerYap(p.islemTarih) + ",";
            sql += VeriErisim.DegerYap(p.yil) + ",";
            sql += VeriErisim.DegerYap(p.vizeTarih) + ",";
            sql += VeriErisim.DegerYap(p.teklifNo) + ",";
            sql += VeriErisim.DegerYap(p.icmalNo) + ",";
            sql += VeriErisim.DegerYap(p.siraNo) + ",";
            sql += VeriErisim.DegerYap(p.tahsisTenkis) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc Sil(TNSDateTime tarih)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.K_TAHTENVIZETARIH], "=", tarih));

            return KomutCalistir(sql, 0);
        }

        public bool UnvanVarmi(int kod)
        {
            string sql = "";
            sql = "SELECT COUNT(*) FROM PERUNVAN WHERE KOD=" + kod.ToString();

            return veriErisim.TekKolonSQLDbl(sql) > 0;
        }

        public bool SinifVarmi(int kod)
        {
            string sql = "";
            sql = "SELECT COUNT(*) FROM PERSINIF WHERE KOD=" + kod.ToString();

            return veriErisim.TekKolonSQLDbl(sql) > 0;
        }

        public bool UstBirimVarmi(int kod)
        {
            string sql = "";
            sql = "SELECT COUNT(*) FROM PERUSTBIRIM WHERE KOD=" + kod.ToString();

            return veriErisim.TekKolonSQLDbl(sql) > 0;
        }

        public IDataReader Rapor(Kadro p)
        {
            string sql = "";
            string kosul = "";

            if (p.teskilat > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_TESKILAT], "=", p.teskilat));

            if (p.ustBirim > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_USTBIRIM], "=", p.ustBirim));

            if (p.sinif > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_SINIF], "=", p.sinif));

            if (p.unvan > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_UNVAN], "=", p.unvan));

            if (p.kadro > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_KADRO], "=", p.kadro));

            if (p.sayIl > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_SAY_IL], "=", p.sayIl));
            if (p.sayIlce > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_SAY_ILCE], "=", p.sayIlce));
            if (p.sayTip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_SAY_TIP], "=", p.sayTip));

            if (!p.vizeTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_TAHTENVIZETARIH], "=", p.vizeTarih));

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = "SELECT KADRO.K_TESKILAT, KADRO.K_USTBIRIM, KADRO.K_SINIF, KADRO.K_UNVAN, KADRO.K_KADRO, KADRO.K_SAY_IL, ";
            sql += " KADRO.K_SAY_TIP, KADRO.K_SAY_ILCE, SUM(KADRO.K_DOLU) AS DOLU, SUM(KADRO.K_BOS) AS BOS, SUM(KADRO.K_ADET) AS ADET, ";
            sql += " PERUNVAN.AD AS UNVANAD, PERSINIF.AD AS SINIFAD, PERTESKILAT.AD AS TESKILATAD, PERUSTBIRIM.AD AS USTBIRIMAD, ";
            sql += " PERSAYMANLIK.AD AS SAYMANLIKAD";
            sql += " FROM PERSAYMANLIK RIGHT OUTER JOIN";
            sql += " KADRO ON PERSAYMANLIK.IL = KADRO.K_SAY_IL AND PERSAYMANLIK.TIP = KADRO.K_SAY_TIP AND ";
            sql += " PERSAYMANLIK.ILCE = KADRO.K_SAY_ILCE LEFT OUTER JOIN";
            sql += " PERUSTBIRIM ON KADRO.K_USTBIRIM = PERUSTBIRIM.KOD LEFT OUTER JOIN";
            sql += " PERSINIF ON KADRO.K_SINIF = PERSINIF.KOD LEFT OUTER JOIN";
            sql += " PERTESKILAT ON KADRO.K_TESKILAT = PERTESKILAT.KOD LEFT OUTER JOIN";
            sql += " PERUNVAN ON KADRO.K_UNVAN = PERUNVAN.KOD";
            sql += kosul;
            sql += " GROUP BY KADRO.K_TESKILAT, KADRO.K_USTBIRIM, KADRO.K_SINIF, KADRO.K_UNVAN, KADRO.K_SAY_IL, ";
            sql += " KADRO.K_SAY_TIP, KADRO.K_SAY_ILCE, KADRO.K_KADRO, PERTESKILAT.AD, PERUSTBIRIM.AD, PERUNVAN.AD, PERSINIF.AD, PERSAYMANLIK.AD";
            //sql += " ORDER BY 1,2,3,5,6,7,4,8";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OncesiDoluBos(Kadro p)
        {
            string sql = "";

            sql = "SELECT SUM(K_DOLU), SUM(K_BOS), SUM(K_ADET) FROM KADRO";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_TESKILAT], "=", p.teskilat));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_USTBIRIM], "=", p.ustBirim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_SINIF], "=", p.sinif));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_UNVAN], "=", p.unvan));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_KADRO], "=", p.kadro));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_SAY_IL], "=", p.sayIl));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_SAY_ILCE], "=", p.sayIlce));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_SAY_TIP], "=", p.sayTip));

            if (!p.vizeTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.K_TAHTENVIZETARIH], "<", p.vizeTarih));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader VizeListele()
        {
            string sql = "";

            sql = "SELECT K_TAHTENVIZETARIH FROM KADRO GROUP BY K_TAHTENVIZETARIH ORDER BY K_TAHTENVIZETARIH DESC";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
