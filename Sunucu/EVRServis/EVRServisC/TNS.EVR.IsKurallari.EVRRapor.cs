using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak rapor işlemleriyle ilgili listeleme yordamını tutan sınıf. 
    /// </summary>
    public class EVRRapor
    {
        /// <summary>
        /// Parammetre olarak verilen rapor türüne göre, evrak sayılarını içeren rapor bilgisini nesne dizisi olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">Evrak bilgisini içeren nesne</param>
        /// <param name="raporTur">Rapor türünü içeren nesne</param>
        /// <returns>Nesne dizisi olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, EvrakSinif form, ENUMEvrakRaporTuru raporTur)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABRapor to = new TABRapor(ve);
            IDataReader r = to.Listele(form, raporTur);
            object[] evrakBilgi;

            if (to.Hata() == "")
            {

                switch (raporTur)
                {
                    case ENUMEvrakRaporTuru.BIRIM: //StringArray : [EVRAKBIRIMKODU],[EVRAKTIPI],[COUNT]
                        while (r.Read())
                        {
                            evrakBilgi = new object[] { string.Empty, new int(), new int() };
                            evrakBilgi[0] = VeriErisim.DegerAl(r, 0, ""); //EVRAKBIRIMKODU
                            evrakBilgi[1] = VeriErisim.DegerAl(r, 1, 0);  //EVRAKTIPI
                            evrakBilgi[2] = VeriErisim.DegerAl(r, 2, 0);  //COUNT

                            hVAN.objeler.Add(evrakBilgi);
                        }
                        break;
                    case ENUMEvrakRaporTuru.KONU: //StringArray : [EVRAKBIRIMKODU],[KONU],[COUNT]
                        while (r.Read())
                        {
                            evrakBilgi = new object[] { string.Empty, string.Empty, new int() };
                            evrakBilgi[0] = VeriErisim.DegerAl(r, 0, ""); //EVRAKBIRIMKODU
                            evrakBilgi[1] = VeriErisim.DegerAl(r, 1, "");  //KONU
                            evrakBilgi[2] = VeriErisim.DegerAl(r, 2, 0);  //COUNT

                            hVAN.objeler.Add(evrakBilgi);
                        }
                        break;
                    case ENUMEvrakRaporTuru.GELDIGIYER:  //StringArray : [EVRAKBIRIMKODU],[GELDIGIYER],[COUNT]
                        while (r.Read())
                        {
                            evrakBilgi = new object[] { string.Empty, string.Empty, new int() };
                            evrakBilgi[0] = VeriErisim.DegerAl(r, 0, ""); //EVRAKBIRIMKODU
                            evrakBilgi[1] = VeriErisim.DegerAl(r, 1, "");  //GELDIGIYER
                            evrakBilgi[2] = VeriErisim.DegerAl(r, 2, 0);  //COUNT

                            hVAN.objeler.Add(evrakBilgi);
                        }
                        break;
                    case ENUMEvrakRaporTuru.GELISSEKLI:
                        while (r.Read())
                        {
                            evrakBilgi = new object[] { string.Empty, string.Empty, new int() };
                            evrakBilgi[0] = VeriErisim.DegerAl(r, 0, ""); //EVRAKBIRIMKODU
                            evrakBilgi[1] = VeriErisim.DegerAl(r, 1, 0); //GELISSEKLI
                            evrakBilgi[2] = VeriErisim.DegerAl(r, 2, 0); //COUNT

                            hVAN.objeler.Add(evrakBilgi);
                        }
                        break;
                    case ENUMEvrakRaporTuru.BEKLEYENEVRAK:
                        break;
                    case ENUMEvrakRaporTuru.PERSONEL:
                        while (r.Read())
                        {
                            evrakBilgi = new object[] { new int(), string.Empty };
                            evrakBilgi[0] = VeriErisim.DegerAl(r, 0, 0); //[GELEN-GIDEN-TESLİMALINMAYAN EVRAKSAYISI]
                            evrakBilgi[1] = VeriErisim.DegerAl(r, 1, ""); //[TCKIMLIKNO]

                            hVAN.objeler.Add(evrakBilgi);
                        }
                        break;
                    case ENUMEvrakRaporTuru.PERSONELTOPLAM:
                        while (r.Read())
                        {
                            evrakBilgi = new object[] { new int(), string.Empty };
                            evrakBilgi[0] = VeriErisim.DegerAl(r, 0, 0); //[GELEN-GIDEN EVRAKSAYISI]
                            evrakBilgi[1] = VeriErisim.DegerAl(r, 1, ""); //[TCKIMLIKNO]

                            hVAN.objeler.Add(evrakBilgi);
                        }
                        break;
                    default:
                        break;
                }

            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }
    }
}
