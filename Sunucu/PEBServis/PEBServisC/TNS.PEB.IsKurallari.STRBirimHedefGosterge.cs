using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using TNS.BKD;

namespace TNS.PEB.IsKurallari
{
    public class STRBirimHedefGosterge
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, TNSCollection gostergeler, string formId)
        {
            Sonuc sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, donemNo, orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHedefGosterge tBirimHedefGosterge = new TABBirimHedefGosterge(ve);
            ve.BeginTransaction();

            BirimHedefGosterge birimHedefGosterge = new BirimHedefGosterge();
            birimHedefGosterge.donem = new Donem();
            birimHedefGosterge.donem.donemNo = donemNo;
            birimHedefGosterge.orgBirimKod = orgBirimKod;
            birimHedefGosterge.hedefKod = hedefKod;

            sonuc = tBirimHedefGosterge.Sil(birimHedefGosterge);
            if (sonuc.islemSonuc)
            {
                foreach (BirimHedefGosterge gosterge in gostergeler)
                {
                    sonuc = tBirimHedefGosterge.Kaydet(gosterge);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, donemNo, orgBirimKod, ENUMSurec.StratejikModelleme);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.STR010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.STR011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimHedefGosterge birimHedefGosterge, string formId)
        {
            Sonuc sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, birimHedefGosterge.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, birimHedefGosterge.donem.donemNo, birimHedefGosterge.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHedefGosterge tBirimHedefGosterge = new TABBirimHedefGosterge(ve);
            ve.BeginTransaction();

            sonuc = tBirimHedefGosterge.Sil(birimHedefGosterge);
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.STR014;
                ve.Commit();
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.STR015 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static BirimHedefGosterge[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefGosterge birimHedefGosterge)
        {
            return Listele(veriErisim, kullanan, birimHedefGosterge, false);
        }

        public static BirimHedefGosterge[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefGosterge birimHedefGosterge, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, birimHedefGosterge.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimHedefGosterge tBirimHedefGosterge = new TABBirimHedefGosterge(ve);

            IDataReader dr = null;
            try
            {
                dr = tBirimHedefGosterge.Listele(birimHedefGosterge, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection birimHedefGostergeler = new TNSCollection();
            while (dr.Read())
            {
                BirimHedefGosterge gelen = new BirimHedefGosterge();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABBirimHedefGosterge.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefGosterge.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefGosterge.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.hedefAd = PEBServis.UzayYaziDegerGetir("STRBIRIMAMACHEDEF", gelen.donem.donemNo + "-" + gelen.orgBirimKod + "-" + gelen.hedefKod, true, string.Empty);
                gelen.gostergeKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefGosterge.ENUMAlanlar.GOSTERGEKOD, 0);
                gelen.gostergeAd = PEBServis.UzayYaziDegerGetir("STRGOSTERGE", gelen.gostergeKod.ToString(), true, string.Empty);
                gelen.yil = VeriErisim.DegerAl(dr, (int)TABBirimHedefGosterge.ENUMAlanlar.YIL, 0);
                gelen.hedeflenenDeger = VeriErisim.DegerAl(dr, (int)TABBirimHedefGosterge.ENUMAlanlar.HEDEFLENENDEGER, (decimal)0);

                birimHedefGostergeler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimHedefGosterge[])birimHedefGostergeler.ToArray(typeof(BirimHedefGosterge));
        }
    }
}