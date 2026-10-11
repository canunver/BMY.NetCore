using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABSpGenelHavuzRapor : TABOrtak
    {
        public TABSpGenelHavuzRapor(VeriErisim veriErisim) : base(veriErisim) { }

        public IDataReader Listele(int donemNo, string birimTur, string ustKod)
        {
            string sql = "SELECT A.GENELHAVUZKOD, COUNT(*) FROM STRBIRIMGENELHAVUZ A";
            string baglac = "WHERE";
            if (!string.IsNullOrEmpty(birimTur))
            {
                sql += ", ORGBIRIM B";
                sql += " WHERE A.ORGBIRIMKOD = B.KOD";
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "TURKOD", "LIKE", birimTur + "%"));
                baglac = "AND";
            }
            sql = VeriErisim.KosulEkle(sql, baglac, VeriErisim.KosulOl("A", "DONEMNO", "=", donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "GENELHAVUZKOD", "LIKE", ustKod + "%"));
            sql += " GROUP BY GENELHAVUZKOD";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}