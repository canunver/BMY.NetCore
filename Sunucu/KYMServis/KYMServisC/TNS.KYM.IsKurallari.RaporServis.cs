using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM.IsKurallari.Veri;
using System.Threading;
using System.Collections;

namespace TNS.KYM.IsKurallari
{
    public static class RaporServis
    {
        public static Hashtable Listele()
        {
            Hashtable htRaporGrup = new Hashtable();            
            ArrayList liste;

            object[] bilgi;
            int raporGrupNo, raporNo;
            string raporDil;
            string key;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", null);
            TABRapor to = new TABRapor(ve);
            IDataReader dr = to.Listele();

            if (to.Hata() == "" && dr != null)
            {
                while (dr.Read())
                {
                    bilgi = new object[TABRapor.ALANLAR.Length];
                    raporGrupNo = VeriErisim.DegerAl(dr, (int)TABRapor.ENUMAlanlar.RAPORGRUPNO, 0);
                    raporNo = VeriErisim.DegerAl(dr, (int)TABRapor.ENUMAlanlar.RAPORNO, 0);
                    raporDil = VeriErisim.DegerAl(dr, (int)TABRapor.ENUMAlanlar.RAPORDIL, "");

                    key = raporGrupNo + "-" + raporNo + "-" + raporDil;

                    bilgi[0] = VeriErisim.DegerAl(dr, (int)TABRapor.ENUMAlanlar.TCKIMLIKNO, "");
                    bilgi[1] = VeriErisim.DegerAl(dr, (int)TABRapor.ENUMAlanlar.AD, "");
                    bilgi[2] = VeriErisim.DegerAl(dr, (int)TABRapor.ENUMAlanlar.SOYAD, "");
                    bilgi[3] = VeriErisim.DegerAl(dr, (int)TABRapor.ENUMAlanlar.UNVAN, "");
                    bilgi[4] = VeriErisim.DegerAl(dr, (int)TABRapor.ENUMAlanlar.EMAIL, "");
                    bilgi[5] = raporGrupNo;
                    bilgi[6] = raporNo;
                    bilgi[7] = VeriErisim.DegerAl(dr, (int)TABRapor.ENUMAlanlar.RAPORDONEM, "");
                    bilgi[8] = VeriErisim.DegerAl(dr, (int)TABRapor.ENUMAlanlar.RAPORDONEMICI, "");
                    bilgi[9] = raporDil;                 

                    if (htRaporGrup.ContainsKey(key)) { liste = (ArrayList)htRaporGrup[key]; }
                    else { liste = new ArrayList(); htRaporGrup.Add(key, liste); }

                    liste.Add(bilgi);
                }
            }
            if (dr != null) { dr.Close(); }
            ve.VeriErisimKapat();

            return htRaporGrup;
        }
    }
}
