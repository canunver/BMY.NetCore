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
    public class STRBirimHedefUstPolitika
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

            TABBirimHedefUstPolitika tBirimHedefUstPolitika = new TABBirimHedefUstPolitika(ve);
            ve.BeginTransaction();

            BirimHedefUstPolitika birimHedefUstPolitika = new BirimHedefUstPolitika();
            birimHedefUstPolitika.donem = new Donem();
            birimHedefUstPolitika.donem.donemNo = donemNo;
            birimHedefUstPolitika.orgBirimKod = orgBirimKod;
            birimHedefUstPolitika.hedefKod = hedefKod;

            sonuc = tBirimHedefUstPolitika.Sil(birimHedefUstPolitika);
            if (sonuc.islemSonuc)
            {
                foreach (BirimHedefUstPolitika item in liste.objeler)
                {
                    sonuc = tBirimHedefUstPolitika.Kaydet(item);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimHedefUstPolitika birimHedefUstPolitika, string formId)
        {
            Sonuc sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, birimHedefUstPolitika.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, birimHedefUstPolitika.donem.donemNo, birimHedefUstPolitika.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHedefUstPolitika tBirimHedefUstPolitika = new TABBirimHedefUstPolitika(ve);
            ve.BeginTransaction();

            sonuc = tBirimHedefUstPolitika.Sil(birimHedefUstPolitika);
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

        public static BirimHedefUstPolitika[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefUstPolitika birimHedefUstPolitika)
        {
            return Listele(veriErisim, kullanan, birimHedefUstPolitika, false);
        }

        public static BirimHedefUstPolitika[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefUstPolitika birimHedefUstPolitika, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, birimHedefUstPolitika.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimHedefUstPolitika tBirimHedefUstPolitika = new TABBirimHedefUstPolitika(ve);

            IDataReader dr = null;
            try
            {
                dr = tBirimHedefUstPolitika.Listele(birimHedefUstPolitika, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection birimHedefUstPolitikaler = new TNSCollection();
            while (dr.Read())
            {
                BirimHedefUstPolitika gelen = new BirimHedefUstPolitika();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABBirimHedefUstPolitika.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefUstPolitika.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefUstPolitika.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.ustPolitikaKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefUstPolitika.ENUMAlanlar.USTPOLITIKAKOD, string.Empty);
                gelen.ustPolitikaAd = PEBServis.UzayYaziDegerGetir("STRGENELHAVUZ", gelen.ustPolitikaKod, true, string.Empty);

                birimHedefUstPolitikaler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimHedefUstPolitika[])birimHedefUstPolitikaler.ToArray(typeof(BirimHedefUstPolitika));
        }
    }
}