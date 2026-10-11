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
    public class STRBirimHedefGzft
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

            TABBirimHedefGzft tBirimHedefGzft = new TABBirimHedefGzft(ve);
            ve.BeginTransaction();

            BirimHedefGzft birimHedefGzft = new BirimHedefGzft();
            birimHedefGzft.donem = new Donem();
            birimHedefGzft.donem.donemNo = donemNo;
            birimHedefGzft.orgBirimKod = orgBirimKod;
            birimHedefGzft.hedefKod = hedefKod;

            sonuc = tBirimHedefGzft.Sil(birimHedefGzft);
            if (sonuc.islemSonuc)
            {
                foreach (BirimHedefGzft item in liste.objeler)
                {
                    sonuc = tBirimHedefGzft.Kaydet(item);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimHedefGzft birimHedefGzft, string formId)
        {
            Sonuc sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, birimHedefGzft.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, birimHedefGzft.donem.donemNo, birimHedefGzft.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHedefGzft tBirimHedefGzft = new TABBirimHedefGzft(ve);
            ve.BeginTransaction();

            sonuc = tBirimHedefGzft.Sil(birimHedefGzft);
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

        public static BirimHedefGzft[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefGzft birimHedefGzft)
        {
            return Listele(veriErisim, kullanan, birimHedefGzft, false);
        }

        public static BirimHedefGzft[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefGzft birimHedefGzft, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, birimHedefGzft.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimHedefGzft tBirimHedefGzft = new TABBirimHedefGzft(ve);

            IDataReader dr = null;
            try
            {
                dr = tBirimHedefGzft.Listele(birimHedefGzft, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection birimHedefGzftler = new TNSCollection();
            while (dr.Read())
            {
                BirimHedefGzft gelen = new BirimHedefGzft();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABBirimHedefGzft.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefGzft.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefGzft.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.gzftTur = VeriErisim.DegerAl(dr, (int)TABBirimHedefGzft.ENUMAlanlar.GZFTTUR, 0);
                gelen.gzftSiraNo = VeriErisim.DegerAl(dr, (int)TABBirimHedefGzft.ENUMAlanlar.GZFTSIRANO, 0);
                gelen.gzftAciklama = PEBServis.UzayYaziDegerGetir("STRGZFTDETAY", gelen.donem.donemNo.ToString() + "-" + gelen.orgBirimKod.ToString() + "-" + gelen.gzftTur + "-" + gelen.gzftSiraNo, true, string.Empty);
                birimHedefGzftler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimHedefGzft[])birimHedefGzftler.ToArray(typeof(BirimHedefGzft));
        }
    }
}