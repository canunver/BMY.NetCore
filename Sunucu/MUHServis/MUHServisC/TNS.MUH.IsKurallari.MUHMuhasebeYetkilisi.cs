using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Muhasebe Yetkilisi işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHMuhasebeYetkilisi
    {
        /// <summary>
        /// Muhasebe yetkilisi kaydet.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kriter">muhasebe yetkilisi</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, MuhasebeYetkilisi kriter)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");
            
            if (!Yetki.SistemYoneticisimi(kullanan))
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH080;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            string hata = "";
            if (kriter.muhasebe <= 0)
                hata += ButceMuhasebeResI.MUH130 + "<br>";
            if (string.IsNullOrEmpty(kriter.adsoyad))
                hata += ButceMuhasebeResI.MUH131 + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.DegerAta(hata);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABMuhasebeYetkilisi to = new TABMuhasebeYetkilisi(ve);

            sonuc = to.Kaydet(kriter);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH132;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Muhasebe yetkilisi sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebeYetkilisi">muhasebe yetkilisi</param>
        /// <returns></returns>
        public Sonuc Sil(Kullanici kullanan, MuhasebeYetkilisi muhasebeYetkilisi)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH068, "");

            if (!Yetki.SistemYoneticisimi(kullanan))
            {
                sonuc.hataStr = ButceMuhasebeResI.MUH080;
                sonuc.islemSonuc = false;
                return sonuc;
            }
            if (muhasebeYetkilisi.muhasebe <= 0)
            {
                sonuc.DegerAta(ButceMuhasebeResI.MUH133);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABMuhasebeYetkilisi to = new TABMuhasebeYetkilisi(ve);
            sonuc = to.Sil(muhasebeYetkilisi.muhasebe);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH134;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Muhasebe yetkilisi listele.
        /// </summary>
        /// <param name="kriter">muhasebe yetkilisi</param>
        /// <returns></returns>
        public ObjectArray Listele(MuhasebeYetkilisi kriter)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABMuhasebeYetkilisi to = new  TABMuhasebeYetkilisi(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    MuhasebeYetkilisi y = new MuhasebeYetkilisi();

                    y.muhasebe = VeriErisim.DegerAl(r, 0, 0);
                    y.adsoyad = VeriErisim.DegerAl(r, 1, "");
                    y.unvan = VeriErisim.DegerAl(r, 2, "");

                    iVAN.objeler.Add(y);
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return iVAN;
        }
    }
}
