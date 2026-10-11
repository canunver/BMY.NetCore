using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABOneri : TABOrtak
    {
        public TABOneri(VeriErisim veriErisim) : base(veriErisim) { }

        public IDataReader Listele(string kriter, ENUMOneriTip oneriTip)
        {
            string sql = "";
            switch (oneriTip)
            {
                case ENUMOneriTip.UNVAN:
                    sql = "SELECT DISTINCT UNVAN FROM KULLANICI ";
                    sql += "WHERE (NOT (UNVAN = '')) AND (UNVAN LIKE '%" + VeriErisim.DegerYap(kriter) + "%') AND (NOT (LEN(UNVAN) < 3)) ";
                    sql += "ORDER BY UNVAN";
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(sql)) return null;
            return veriErisim.DataReaderDondur(sql);
        }

    }
}
