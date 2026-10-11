using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// mal hizmet yapım alım tutar bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCMHYAlimTutari
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, mal hizmet yapım alım tutar bilgisini veritabanından okur 
        /// ve MHYAlimTutari nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, mal hizmet yapım alım tutar bilgisini MHYAlimTutari nesnesi olarak tutan nesne</returns>
        public MHYAlimTutari Listele(Kullanici kullanan, MHYAlimTutari kriter)
        {
            MHYAlimTutari donusDegeri = new MHYAlimTutari();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMHYAlimTutari to = new TABMHYAlimTutari(ve);
            IDataReader r = to.Listele(kriter);
    
            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    donusDegeri.yil = VeriErisim.DegerAl(r, 0, 0);
                    donusDegeri.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    donusDegeri.birim = VeriErisim.DegerAl(r, 2, "");
                    donusDegeri.surecNo = VeriErisim.DegerAl(r, 3, "");
                    donusDegeri.alimTutari = (decimal)VeriErisim.DegerAl(r, 4, 0.0);
                    donusDegeri.firmaKod = VeriErisim.DegerAl(r, 5, 0);
                    donusDegeri.firmaAd = VeriErisim.DegerAl(r, 6, "");
                }
            }
            r.Close();

            ve.VeriErisimKapat();

            return donusDegeri;
        }
    }

}
