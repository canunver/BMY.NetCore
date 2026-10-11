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
    public class STRBirimHedefUstHedef
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

            TABBirimHedefUstHedef tBirimHedefUstHedef = new TABBirimHedefUstHedef(ve);
            ve.BeginTransaction();

            BirimHedefUstHedef birimHedefUstHedef = new BirimHedefUstHedef();
            birimHedefUstHedef.donem = new Donem();
            birimHedefUstHedef.donem.donemNo = donemNo;
            birimHedefUstHedef.orgBirimKod = orgBirimKod;
            birimHedefUstHedef.hedefKod = hedefKod;

            sonuc = tBirimHedefUstHedef.Sil(birimHedefUstHedef);
            if (sonuc.islemSonuc)
            {
                foreach (BirimHedefUstHedef item in liste.objeler)
                {
                    sonuc = tBirimHedefUstHedef.Kaydet(item);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimHedefUstHedef birimHedefUstHedef, string formId)
        {
            Sonuc sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, birimHedefUstHedef.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, birimHedefUstHedef.donem.donemNo, birimHedefUstHedef.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHedefUstHedef tBirimHedefUstHedef = new TABBirimHedefUstHedef(ve);
            ve.BeginTransaction();

            sonuc = tBirimHedefUstHedef.Sil(birimHedefUstHedef);
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

        public static BirimHedefUstHedef[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefUstHedef birimHedefUstHedef)
        {
            return Listele(veriErisim, kullanan, birimHedefUstHedef, false);
        }

        public static BirimHedefUstHedef[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefUstHedef birimHedefUstHedef, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, birimHedefUstHedef.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimHedefUstHedef tBirimHedefUstHedef = new TABBirimHedefUstHedef(ve);

            IDataReader dr = null;
            try
            {
                dr = tBirimHedefUstHedef.Listele(birimHedefUstHedef, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection birimHedefUstHedefler = new TNSCollection();
            while (dr.Read())
            {
                BirimHedefUstHedef gelen = new BirimHedefUstHedef();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABBirimHedefUstHedef.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefUstHedef.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefUstHedef.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.birimKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefUstHedef.ENUMAlanlar.USTORGBIRIMKOD, 0);
                gelen.birimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.birimKod.ToString(), true, string.Empty);
                gelen.ustHedefKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefUstHedef.ENUMAlanlar.USTHEDEFKOD, string.Empty);
                gelen.ustHedefAd = PEBServis.UzayYaziDegerGetir("STRBIRIMAMACHEDEF", gelen.donem.donemNo + "-" + gelen.birimKod + "-" + gelen.ustHedefKod, true, string.Empty);

                birimHedefUstHedefler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimHedefUstHedef[])birimHedefUstHedefler.ToArray(typeof(BirimHedefUstHedef));
        }
    }
}