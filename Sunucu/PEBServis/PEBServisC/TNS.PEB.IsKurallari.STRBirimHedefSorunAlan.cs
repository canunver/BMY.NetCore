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
    public class STRBirimHedefSorunAlan
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, ObjectArray liste, string formId)
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

            TABBirimHedefSorunAlan tBirimHedefSorunAlan = new TABBirimHedefSorunAlan(ve);
            ve.BeginTransaction();

            BirimHedefSorunAlan birimHedefSorunAlan = new BirimHedefSorunAlan();
            birimHedefSorunAlan.donem = new Donem();
            birimHedefSorunAlan.donem.donemNo = donemNo;
            birimHedefSorunAlan.orgBirimKod = orgBirimKod;
            birimHedefSorunAlan.hedefKod = hedefKod;

            sonuc = tBirimHedefSorunAlan.Sil(birimHedefSorunAlan);
            if (sonuc.islemSonuc)
            {
                foreach (BirimHedefSorunAlan item in liste.objeler)
                {
                    sonuc = tBirimHedefSorunAlan.Kaydet(item);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimHedefSorunAlan birimHedefSorunAlan, string formId)
        {
            Sonuc sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, birimHedefSorunAlan.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, birimHedefSorunAlan.donem.donemNo, birimHedefSorunAlan.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHedefSorunAlan tBirimHedefSorunAlan = new TABBirimHedefSorunAlan(ve);
            ve.BeginTransaction();

            sonuc = tBirimHedefSorunAlan.Sil(birimHedefSorunAlan);
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

        public static BirimHedefSorunAlan[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefSorunAlan birimHedefSorunAlan)
        {
            return Listele(veriErisim, kullanan, birimHedefSorunAlan, false);
        }

        public static BirimHedefSorunAlan[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefSorunAlan birimHedefSorunAlan, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, birimHedefSorunAlan.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimHedefSorunAlan tBirimHedefSorunAlan = new TABBirimHedefSorunAlan(ve);

            IDataReader dr = null;
            try
            {
                dr = tBirimHedefSorunAlan.Listele(birimHedefSorunAlan, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection birimHedefSorunAlanler = new TNSCollection();
            while (dr.Read())
            {
                BirimHedefSorunAlan gelen = new BirimHedefSorunAlan();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABBirimHedefSorunAlan.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefSorunAlan.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefSorunAlan.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.sorunAlanKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefSorunAlan.ENUMAlanlar.SORUNALANKOD, string.Empty);
                gelen.sorunAlanAd = PEBServis.UzayYaziDegerGetir("STRGENELHAVUZ", gelen.sorunAlanKod, true, string.Empty);

                birimHedefSorunAlanler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimHedefSorunAlan[])birimHedefSorunAlanler.ToArray(typeof(BirimHedefSorunAlan));
        }
    }
}