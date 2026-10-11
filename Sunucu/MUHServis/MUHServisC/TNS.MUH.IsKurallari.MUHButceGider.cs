using System.Data;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Gider Bütçesi 
    /// </summary>
    public class MUHButceGider
    {
        /// <summary>
        /// Gider Bütçesi kaydet
        /// </summary>
        /// <param name="bilgi">İşlem yapılacak bilgileri içeren nesne</param>
        /// <returns></returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            ve.BeginTransaction();

            TABButceGider to = new TABButceGider(ve);
            TABGenel tg = new TABGenel(ve);

            foreach (ButceGider g in bilgi.objeler)
            {
                sonuc = to.Kaydet(g);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH006;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Gider Bütçesi listele
        /// </summary>
        /// <param name="kriter">Kriter</param>
        /// <returns></returns>
        public ObjectArray Listele(ButceGider kriter)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABButceGider to = new TABButceGider(ve);
            IDataReader r = to.Listele(kriter);

            if (r != null)
            {
                while (r.Read())
                {
                    ButceGider bilgi = new ButceGider();

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.kur = VeriErisim.DegerAl(r, 1, "");
                    bilgi.fon = VeriErisim.DegerAl(r, 2, "");
                    bilgi.fin = VeriErisim.DegerAl(r, 3, "");
                    bilgi.eko = VeriErisim.DegerAl(r, 4, "");
                    bilgi.kanun = VeriErisim.DegerAl(r, 5, 0.0);
                    bilgi.kbo = VeriErisim.DegerAl(r, 6, 0.0);
                    bilgi.eklenen = VeriErisim.DegerAl(r, 7, 0.0);
                    bilgi.dusulen = VeriErisim.DegerAl(r, 8, 0.0);
                    bilgi.serbest = VeriErisim.DegerAl(r, 9, 0.0);

                    iVAN.objeler.Add(bilgi);
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}
