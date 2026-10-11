using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.KYM;
using TNS.Veri;
using TNS.GEL.IsKurallari.Veri;
using System.Data;


namespace TNS.GEL.IsKurallari
{
    public class GELOrtakIslemler
    {
        public static ObjectArray ParabirimiListele(VeriErisim veriErisim, Kullanici kullanan, TNS.BKD.ParaBirimi paraBirimi)
        {
            ObjectArray parabirimleri = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriHRC");
            ve.BeginTransaction();
            TABGenel to = new TABGenel(ve);
            IDataReader dr = to.ParaBirimiListele(paraBirimi);
            if (to.Hata() == "")
            {
                while (dr.Read())
                {
                    TNS.BKD.ParaBirimi pb = new TNS.BKD.ParaBirimi();

                    pb.kod = VeriErisim.DegerAl(dr, 0, 0);
                    pb.adi = VeriErisim.DegerAl(dr, 1, "");
                    pb.kisaltma = VeriErisim.DegerAl(dr, 2, "");
                    parabirimleri.objeler.Add(pb);
                }
            }
            dr.Close();
            ve.VeriErisimKapat();
            return parabirimleri;
        }

        
    }
}
