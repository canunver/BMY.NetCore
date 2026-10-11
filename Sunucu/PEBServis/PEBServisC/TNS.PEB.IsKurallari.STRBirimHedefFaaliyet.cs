using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using TNS.BKD;

namespace TNS.PEB.IsKurallari
{
    public class STRBirimHedefFaaliyet
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int donemNo, int orgBirimKod, string hedefKod, TNSCollection faaliyetler, string formId)
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

            TABBirimHedefFaaliyet tBirimHedefFaaliyet = new TABBirimHedefFaaliyet(ve);
            ve.BeginTransaction();

            BirimHedefFaaliyet birimHedefFaaliyet = new BirimHedefFaaliyet();
            birimHedefFaaliyet.donem = new Donem();
            birimHedefFaaliyet.donem.donemNo = donemNo;
            birimHedefFaaliyet.orgBirimKod = orgBirimKod;
            birimHedefFaaliyet.hedefKod = hedefKod;

            sonuc = tBirimHedefFaaliyet.Sil(birimHedefFaaliyet);
            if (sonuc.islemSonuc)
            {
                foreach (BirimHedefFaaliyet faaliyet in faaliyetler)
                {
                    sonuc = tBirimHedefFaaliyet.Kaydet(faaliyet);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, donemNo, orgBirimKod, ENUMSurec.StratejikModelleme);

            if (sonuc.islemSonuc)
            {
                ve.Commit();

                sonuc = BirimFaaliyetAktar(ve, donemNo, orgBirimKod);
                sonuc.bilgiStr = PerformansButceResI.STR010;
                if (!sonuc.islemSonuc)
                {
                    sonuc.islemSonuc = true;
                    sonuc.bilgiStr += "<br>" + PerformansButceResI.STR126 + "<br>" + sonuc.hataStr;
                }
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.STR011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimHedefFaaliyet birimHedefFaaliyet, string formId)
        {
            Sonuc sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, birimHedefFaaliyet.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, birimHedefFaaliyet.donem.donemNo, birimHedefFaaliyet.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHedefFaaliyet tBirimHedefFaaliyet = new TABBirimHedefFaaliyet(ve);
            ve.BeginTransaction();

            sonuc = tBirimHedefFaaliyet.Sil(birimHedefFaaliyet);
            if (sonuc.islemSonuc)
            {
                ve.Commit();

                sonuc = BirimFaaliyetAktar(ve, birimHedefFaaliyet.donem.donemNo, birimHedefFaaliyet.orgBirimKod);
                sonuc.bilgiStr = PerformansButceResI.STR014;
                if (!sonuc.islemSonuc)
                {
                    sonuc.islemSonuc = true;
                    sonuc.bilgiStr += "<br>" + PerformansButceResI.STR126 + "<br>" + sonuc.hataStr;
                }
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.STR015 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static BirimHedefFaaliyet[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefFaaliyet birimHedefFaaliyet)
        {
            return Listele(veriErisim, kullanan, birimHedefFaaliyet, false);
        }

        public static BirimHedefFaaliyet[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefFaaliyet birimHedefFaaliyet, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, birimHedefFaaliyet.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimHedefFaaliyet tBirimHedefFaaliyet = new TABBirimHedefFaaliyet(ve);

            IDataReader dr = null;
            try
            {
                dr = tBirimHedefFaaliyet.Listele(birimHedefFaaliyet, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection birimHedefFaaliyetler = new TNSCollection();
            while (dr.Read())
            {
                BirimHedefFaaliyet gelen = new BirimHedefFaaliyet();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABBirimHedefFaaliyet.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefFaaliyet.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefFaaliyet.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.hedefAd = PEBServis.UzayYaziDegerGetir("STRBIRIMAMACHEDEF", gelen.donem.donemNo + "-" + gelen.orgBirimKod + "-" + gelen.hedefKod, true, string.Empty);
                gelen.faaliyetKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefFaaliyet.ENUMAlanlar.FAALIYETKOD, string.Empty);
                gelen.faaliyetAd = PEBServis.UzayYaziDegerGetir("STRGENELHAVUZ", gelen.faaliyetKod, true, string.Empty);

                birimHedefFaaliyetler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimHedefFaaliyet[])birimHedefFaaliyetler.ToArray(typeof(BirimHedefFaaliyet));
        }

        /// <summary>
        /// Birimin faaliyetleri STRBIRIMGENELHAVUZ tablosuna aktarılıyor..
        /// </summary>
        /// <param name="veriErisim"></param>
        /// <param name="donemNo"></param>
        /// <param name="orgBirimKod"></param>
        /// <returns></returns>
        private static Sonuc BirimFaaliyetAktar(VeriErisim veriErisim, int donemNo, int orgBirimKod)
        {
            if (veriErisim == null)
                return new Sonuc();

            Birim_SpGenelHavuz faaliyetKriter = new Birim_SpGenelHavuz();
            faaliyetKriter.donem = new Donem();
            faaliyetKriter.donem.donemNo = donemNo;
            faaliyetKriter.orgBirimKod = orgBirimKod;
            faaliyetKriter.spGenelHavuz = new SpGenelHavuz();
            faaliyetKriter.spGenelHavuz.kod = ((int)EnumSPGenelHavuzTur.Faaliyet).ToString().PadLeft(2, '0');

            Sonuc sonuc = new TABBirimGenelHavuz(veriErisim).Sil(faaliyetKriter);
            if (sonuc.islemSonuc)
                sonuc = new TABBirimHedefFaaliyet(veriErisim).BirimFaaliyetAktar(donemNo, orgBirimKod);
            return sonuc;
        }
    }
}