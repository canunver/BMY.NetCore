using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Sistemde bulunan ölçü birimi bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCOlcuBirim
    {
        /// <summary>
        /// Sistemde bulunan tüm ölçü birimi bilgisini veritabanından okur 
        /// ve OlcuBirim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <returns>Sistemde bulunan ölçü birimi bilgisini OlcuBirim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOlcuBirim to = new TABOlcuBirim(ve);
            IDataReader r = to.Listele();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    OlcuBirim ob = new OlcuBirim();

                    ob.kod = VeriErisim.DegerAl(r, 0, 0);
                    ob.ad = VeriErisim.DegerAl(r, 1, "");


                    bilgi.objeler.Add(ob);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        internal string OlcuBirimAdBul(Kullanici kullanan, int birimKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABOlcuBirim to = new TABOlcuBirim(ve);
            string tut = to.OlcuBirimAdBul(birimKod);
            ve.VeriErisimKapat();
            return tut;
        }
    }
}
