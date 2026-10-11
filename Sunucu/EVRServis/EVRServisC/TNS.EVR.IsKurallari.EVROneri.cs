using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;

namespace TNS.EVR.IsKurallari
{
    public class EVROneri
    {
        public static string[] Listele(VeriErisim veriErisim, string kriter, ENUMOneriTip oneriTip)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = null;
            switch (oneriTip)
            {
                case ENUMOneriTip.UNVAN:
                    ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM");
                    break;
                default:
                    ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
                    break;
            }
            if (ve != null)
            {
                TABOneri tab = new TABOneri(ve);
                IDataReader dr = tab.Listele(kriter, oneriTip);
                if (dr != null)
                {
                    while (dr.Read())
                        liste.objeler.Add(VeriErisim.DegerAl(dr, 0, ""));
                    dr.Close();
                }

                ve.VeriErisimKapat();
            }
            return (string[])liste.objeler.ToArray(typeof(string));
        }
    }
}
