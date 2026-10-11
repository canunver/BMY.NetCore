using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using TNS.BKD;

namespace TNS.PEB.IsKurallari
{
    /// <summary> 
    /// Dönem tanımlanması ile ilgili veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma, silme işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class STRBirimHedefKoordineBirim
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, ObjectArray liste, string formId)
        {
            for (int i = 0; i < liste.objeler.Count; i++)
            {
                BirimHedefKoordineBirim koordineI = (BirimHedefKoordineBirim)liste.objeler[i];
                for (int j = i + 1; j < liste.objeler.Count; j++)
                {
                    BirimHedefKoordineBirim koordineJ = (BirimHedefKoordineBirim)liste.objeler[j];
                    if (koordineI.birimKod == koordineJ.birimKod)
                        return new Sonuc(false, PerformansButceResI.STR105, string.Empty);
                }
            }

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

            TABBirimHedefKoordineBirim tBirimHedefKoordineBirim = new TABBirimHedefKoordineBirim(ve);
            ve.BeginTransaction();

            BirimHedefKoordineBirim birimHedefKoordineBirim = new BirimHedefKoordineBirim();
            birimHedefKoordineBirim.donem = new Donem();
            birimHedefKoordineBirim.donem.donemNo = donemNo;
            birimHedefKoordineBirim.orgBirimKod = orgBirimKod;
            birimHedefKoordineBirim.hedefKod = hedefKod;

            sonuc = tBirimHedefKoordineBirim.Sil(birimHedefKoordineBirim);
            if (sonuc.islemSonuc)
            {
                foreach (BirimHedefKoordineBirim item in liste.objeler)
                {
                    sonuc = tBirimHedefKoordineBirim.Kaydet(item);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimHedefKoordineBirim birimHedefKoordineBirim, string formId)
        {
            Sonuc sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, birimHedefKoordineBirim.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, birimHedefKoordineBirim.donem.donemNo, birimHedefKoordineBirim.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHedefKoordineBirim tBirimHedefKoordineBirim = new TABBirimHedefKoordineBirim(ve);
            ve.BeginTransaction();

            sonuc = tBirimHedefKoordineBirim.Sil(birimHedefKoordineBirim);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PerformansButceResI.STR014;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.STR015 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static BirimHedefKoordineBirim[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefKoordineBirim birimHedefKoordineBirim)
        {
            return Listele(veriErisim, kullanan, birimHedefKoordineBirim, false);
        }

        public static BirimHedefKoordineBirim[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefKoordineBirim birimHedefKoordineBirim, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, birimHedefKoordineBirim.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimHedefKoordineBirim tBirimHedefKoordineBirim = new TABBirimHedefKoordineBirim(ve);

            IDataReader dr = null;
            try
            {
                dr = tBirimHedefKoordineBirim.Listele(birimHedefKoordineBirim, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection birimHedefKoordineBirimler = new TNSCollection();
            while (dr.Read())
            {
                BirimHedefKoordineBirim gelen = new BirimHedefKoordineBirim();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABBirimHedefKoordineBirim.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefKoordineBirim.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefKoordineBirim.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.hedefAd = PEBServis.UzayYaziDegerGetir("STRBIRIMAMACHEDEF", gelen.donem.donemNo + "-" + gelen.orgBirimKod + "-" + gelen.hedefKod, true, string.Empty);
                gelen.birimKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefKoordineBirim.ENUMAlanlar.BIRIMKOD, 0);
                gelen.birimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.birimKod.ToString(), true, string.Empty);
                gelen.aciklama = VeriErisim.DegerAl(dr, (int)TABBirimHedefKoordineBirim.ENUMAlanlar.ACIKLAMA, string.Empty);
                gelen.siraNo = VeriErisim.DegerAl(dr, (int)TABBirimHedefKoordineBirim.ENUMAlanlar.SIRANO, 0);

                birimHedefKoordineBirimler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimHedefKoordineBirim[])birimHedefKoordineBirimler.ToArray(typeof(BirimHedefKoordineBirim));
        }
    }
}