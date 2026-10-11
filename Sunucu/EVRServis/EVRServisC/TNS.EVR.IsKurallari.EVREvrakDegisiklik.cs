using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVREvrakDegisiklik
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, EvrakDegisiklik form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABEvrakDegisiklik to = new TABEvrakDegisiklik(ve);

                sonuc = to.Kaydet(form);
                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{006CA5A1-685B-496F-9772-B12653C9C6E8} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, EvrakDegisiklik bilgi)
        {
            ObjectArray hVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakDegisiklik to = new TABEvrakDegisiklik(ve);
            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new EvrakDegisiklik();

                    bilgi.evrEvrakNo = VeriErisim.DegerAl(r, (int)TABEvrakDegisiklik.ENUMAlanlar.EVREVRAKNO, "");
                    bilgi.kullanici = VeriErisim.DegerAl(r, (int)TABEvrakDegisiklik.ENUMAlanlar.KULLANICI, "");
                    bilgi.tarih = VeriErisim.DegerAl(r, (int)TABEvrakDegisiklik.ENUMAlanlar.TARIH);
                    bilgi.deger = VeriErisim.DegerAl(r, (int)TABEvrakDegisiklik.ENUMAlanlar.DEGISIKLIK, "");
                    bilgi.islemYapanAd = VeriErisim.DegerAl(r, (int)TABEvrakDegisiklik.ENUMAlanlar.ISLEMYAPANAD, "");

                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();
            ve.VeriErisimKapat();
            return hVAN;
        }


        //public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste, ENUMTamimlamaFormTur tanimlamFormTur)
        //{
        //    Sonuc sonuc = new Sonuc(false, EvrakI.EVRI155, "");

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

        //    ve.BeginTransaction();
        //    TABTamimlama to = new TABTamimlama(ve, tanimlamFormTur.ToString());
        //    sonuc = to.Sil(liste);

        //    if (sonuc.islemSonuc)
        //    {
        //        ve.Commit();
        //        sonuc.bilgiStr = EvrakI.EVRI156;
        //    }
        //    else
        //        ve.Rollback();

        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        //public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Tanimlama bilgi, ENUMTamimlamaFormTur tanimlamFormTur)
        //{
        //    ObjectArray hVAN = new ObjectArray();
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
        //    TABTamimlama to = new TABTamimlama(ve, tanimlamFormTur.ToString());
        //    IDataReader r = to.Listele(bilgi);

        //    if (to.Hata() == "")
        //    {
        //        while (r.Read())
        //        {
        //            bilgi = new Tanimlama();

        //            bilgi.kod = VeriErisim.DegerAl(r, 0, 0);
        //            bilgi.ad = VeriErisim.DegerAl(r, 1, "");

        //            hVAN.objeler.Add(bilgi);
        //        }
        //    }

        //    r.Close();

        //    ve.VeriErisimKapat();

        //    return hVAN;
        //}        

    }
}
