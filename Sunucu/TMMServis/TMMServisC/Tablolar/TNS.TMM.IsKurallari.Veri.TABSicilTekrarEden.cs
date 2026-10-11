using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABSicilTekrarEden : TABOrtak
    {
        public TABSicilTekrarEden(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        public IDataReader TekrarEdenSicilleriBul(SicilNoHareket sicilNo)
        {
            string sql = "SELECT A." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILNO];
            sql += ", A." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.PRSICILNO];
            sql += ", " + VeriErisim.IslecBul("MAX") + "(A." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SIRANO] + ")";
            sql += " FROM " + TABSicilHareket.TABLOAD + " A";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.DURUM], "=", (int)ENUMSicilDurum.AKTIF));

            string icSql = "SELECT 1 FROM " + TABSicilHareket.TABLOAD + " B";
            icSql = VeriErisim.KosulEkle(icSql, "WHERE", "A." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.PRSICILNO] + "<>" + "B." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.PRSICILNO]);
            icSql = VeriErisim.KosulEkle(icSql, "AND", "A." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILNO] + "=" + "B." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILNO]);
            icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl("B", TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.DURUM], "=", (int)ENUMSicilDurum.AKTIF));
            if (TABSicilHareket.verisSekli.ToUpper() != "KURUM")
            {
                icSql = VeriErisim.KosulEkle(icSql, "AND", "A." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.MUHASEBEKOD] + "=" + "B." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.MUHASEBEKOD]);
                icSql = VeriErisim.KosulEkle(icSql, "AND", "A." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.HARCAMABIRIMKOD] + "=" + "B." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.HARCAMABIRIMKOD]);
            }

            sql = VeriErisim.KosulEkle(sql, "AND", "EXISTS (" + icSql + ")");

            if (!string.IsNullOrEmpty(sicilNo.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));

            if (!string.IsNullOrEmpty(sicilNo.harcamaBirimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(sicilNo.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.AMBARKOD], "=", sicilNo.ambarKod));

            if (!string.IsNullOrEmpty(sicilNo.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.HESAPPLANKOD], "LIKE", sicilNo.hesapPlanKod.Trim().Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(sicilNo.sicilNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILNO], "=", sicilNo.sicilNo));

            if (sicilNo.yil > 0 && !string.IsNullOrEmpty(sicilNo.fisNo))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.YIL], "=", sicilNo.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.FISNO], "=", sicilNo.fisNo));
            }

            sql += " GROUP BY A." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILNO];
            sql += ", A." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.PRSICILNO];

            sql += " ORDER BY A." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILNO];
            sql += ", A." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.PRSICILNO];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SicilNoGuncelle(SicilNoHareket sicilNo, string sicilNoYeni)
        {
            string sql = "UPDATE " + TABSicilHareket.TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILNO], sicilNoYeni);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.PRSICILNO], "=", sicilNo.prSicilNo));
            if (TABSicilHareket.verisSekli.ToUpper() != "KURUM")
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILNO], "=", sicilNo.sicilNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));
            }

            return KomutCalistir(sql, 0);
        }

        public Sonuc SicilSiraNoGuncelle(SicilNoHareket sicilNo, string sicilNoYeni, string sicilSiraNo)
        {
            string sql = "UPDATE " + TABSicilHareket.TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILSIRANO], sicilSiraNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.PRSICILNO], "=", sicilNo.prSicilNo));
            if (TABSicilHareket.verisSekli.ToUpper() != "KURUM")
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILNO], "=", sicilNoYeni));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));

                string icSql = "SELECT " + VeriErisim.IslecBul("MIN") + "(" + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SIRANO] + ")";
                icSql += " FROM " + TABSicilHareket.TABLOAD + " B";
                icSql += " WHERE " + TABSicilHareket.TABLOAD + "." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.PRSICILNO] + " = B.PRSICILNO";
                icSql += " AND " + TABSicilHareket.TABLOAD + "." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILNO] + " = B.SICILNO";
                icSql = VeriErisim.KosulEkle(icSql, "AND", TABSicilHareket.TABLOAD + "." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.MUHASEBEKOD] + " = B." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.MUHASEBEKOD]);
                icSql = VeriErisim.KosulEkle(icSql, "AND", TABSicilHareket.TABLOAD + "." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.HARCAMABIRIMKOD] + " = B." + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.HARCAMABIRIMKOD]);

                sql = VeriErisim.KosulEkle(sql, "AND", TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SIRANO] + " = (" + icSql + ")");
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SIRANO], "=", 1));

            return KomutCalistir(sql, 1);
        }

        public bool SicilNoKullanimdaMi(string muhasebeKod, string harcamaBirimKod, string sicilNo)
        {
            string sql = "SELECT DISTINCT 1 FROM " + TABSicilHareket.TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILNO], "=", sicilNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.DURUM], "=", (int)ENUMSicilDurum.AKTIF));
            if (TABSicilHareket.verisSekli.ToUpper() != "KURUM")
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.HARCAMABIRIMKOD], "=", harcamaBirimKod.Replace(".", "")));
            }

            return veriErisim.TekKolonSQLInt(sql) == 1 ? true : false;
        }

        public string SicilNoBul(int prSicilNo, int siraNo)
        {
            string sql = "SELECT " + TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SICILNO] + " FROM " + TABSicilHareket.TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.PRSICILNO], "=", prSicilNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.SIRANO], "=", siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, TABSicilHareket.ALANLAR[(int)TABSicilHareket.ENUMAlanlar.DURUM], "=", (int)ENUMSicilDurum.AKTIF));

            return veriErisim.TekKolonSQLStr(sql);
        }
    }
}