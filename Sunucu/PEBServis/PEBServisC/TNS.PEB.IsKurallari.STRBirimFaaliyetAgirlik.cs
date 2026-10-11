using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    public class STRBirimFaaliyetAgirlik
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BirimFaaliyetAgirlik[] bfalar, string formId)
        {
            Sonuc sonuc = new Sonuc();

            if (bfalar == null || bfalar.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR122;
                return sonuc;
            }

            double toplamAgirlik = 0;
            foreach (BirimFaaliyetAgirlik bfa in bfalar)
            {
                string hata = string.Empty;
                if (bfa.donem == null || bfa.donem.donemNo <= 0)
                    hata = PerformansButceResI.STR032 + "<br>";
                if (bfa.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (string.IsNullOrEmpty(bfa.faaliyetKod))
                    hata += PerformansButceResI.STR123 + "<br>";
                if (string.IsNullOrEmpty(bfa.hedefKod))
                    hata += PerformansButceResI.STR021 + "<br>";
                if (bfa.agirlik <= 0)
                    hata += PerformansButceResI.STR124;

                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
                toplamAgirlik += bfa.agirlik;
            }
            if (Math.Round(toplamAgirlik, 4) != 100)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format(PerformansButceResI.STR125, toplamAgirlik.ToString());
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, bfalar[0].orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, bfalar[0].donem.donemNo, bfalar[0].orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimFaaliyetAgirlik tBFA = new TABBirimFaaliyetAgirlik(ve);
            ve.BeginTransaction();

            tBFA.Sil(bfalar[0]);
            foreach (BirimFaaliyetAgirlik bfa in bfalar)
            {
                sonuc = tBFA.Kaydet(bfa);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, bfalar[0].donem.donemNo, bfalar[0].orgBirimKod, ENUMSurec.StratejikModelleme);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimFaaliyetAgirlik bfa, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (bfa.donem == null || bfa.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (bfa.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (string.IsNullOrEmpty(bfa.faaliyetKod))
                hata += PerformansButceResI.STR123 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, bfa.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, bfa.donem.donemNo, bfa.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimFaaliyetAgirlik tBFA = new TABBirimFaaliyetAgirlik(ve);
            ve.BeginTransaction();

            sonuc = tBFA.Sil(bfa);
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

        public static BirimFaaliyetAgirlik[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimFaaliyetAgirlik bfa, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, bfa.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimFaaliyetAgirlik tBFA = new TABBirimFaaliyetAgirlik(ve);

            IDataReader dr = null;
            try
            {
                dr = tBFA.Listele(bfa, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection bfalar = new TNSCollection();
            while (dr.Read())
            {
                BirimFaaliyetAgirlik gelen = new BirimFaaliyetAgirlik();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABBirimFaaliyetAgirlik.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimFaaliyetAgirlik.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.faaliyetKod = VeriErisim.DegerAl(dr, (int)TABBirimFaaliyetAgirlik.ENUMAlanlar.FAALIYETKOD, string.Empty);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimFaaliyetAgirlik.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.hedefAd = PEBServis.UzayYaziDegerGetir("STRBIRIMAMACHEDEF", gelen.donem.donemNo + "-" + gelen.orgBirimKod + "-" + gelen.hedefKod, true, string.Empty);
                gelen.agirlik = VeriErisim.DegerAl(dr, (int)TABBirimFaaliyetAgirlik.ENUMAlanlar.AGIRLIK, (double)0);

                bfalar.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimFaaliyetAgirlik[])bfalar.ToArray(typeof(BirimFaaliyetAgirlik));
        }
    }
}