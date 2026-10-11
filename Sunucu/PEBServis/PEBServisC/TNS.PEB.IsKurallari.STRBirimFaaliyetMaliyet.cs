using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    public class STRBirimFaaliyetMaliyet
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BirimFaaliyetMaliyet[] maliyetler, string formId)
        {
            Sonuc sonuc = new Sonuc();

            if (maliyetler == null || maliyetler.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR086;
                return sonuc;
            }

            foreach (BirimFaaliyetMaliyet bfm in maliyetler)
            {
                string hata = string.Empty;
                if (bfm.donem == null || bfm.donem.donemNo <= 0)
                    hata = PerformansButceResI.STR032 + "<br>";
                if (bfm.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (string.IsNullOrEmpty(bfm.faaliyetKod))
                    hata += PerformansButceResI.STR118 + "<br>";
                if (bfm.yil <= 0)
                    hata += PerformansButceResI.STR119 + "<br>";
                if (string.IsNullOrEmpty(bfm.ekoKod))
                    hata += PerformansButceResI.STR120 + "<br>";
                if (bfm.maliyetTur <= 0)
                    hata += PerformansButceResI.STR121 + "<br>";
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, maliyetler[0].orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, maliyetler[0].donem.donemNo, maliyetler[0].orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimFaaliyetMaliyet tabMaliyet = new TABBirimFaaliyetMaliyet(ve);
            ve.BeginTransaction();

            tabMaliyet.Sil(maliyetler[0]);

            foreach (BirimFaaliyetMaliyet bfm in maliyetler)
            {
                if (bfm.maliyet <= 0)
                    continue;

                sonuc = tabMaliyet.Kaydet(bfm);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, maliyetler[0].donem.donemNo, maliyetler[0].orgBirimKod, ENUMSurec.StratejikModelleme);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimFaaliyetMaliyet maliyet, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (maliyet.donem == null || maliyet.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (maliyet.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (string.IsNullOrEmpty(maliyet.faaliyetKod))
                hata += PerformansButceResI.STR118 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, maliyet.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, maliyet.donem.donemNo, maliyet.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimFaaliyetMaliyet tabMaliyet = new TABBirimFaaliyetMaliyet(ve);
            ve.BeginTransaction();

            sonuc = tabMaliyet.Sil(maliyet);
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

        public static BirimFaaliyetMaliyet[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimFaaliyetMaliyet maliyet)
        {
            return Listele(veriErisim, kullanan, maliyet, false);
        }

        public static BirimFaaliyetMaliyet[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, BirimFaaliyetMaliyet maliyet)
        {
            string hata = string.Empty;
            if (maliyet.donem == null || maliyet.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (maliyet.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            return Listele(veriErisim, kullanan, maliyet, true);
        }

        private static BirimFaaliyetMaliyet[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimFaaliyetMaliyet maliyet, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, maliyet.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimFaaliyetMaliyet tabMaliyet = new TABBirimFaaliyetMaliyet(ve);

            IDataReader dr = null;
            try
            {
                dr = tabMaliyet.Listele(maliyet, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection maliyetler = new TNSCollection();
            while (dr.Read())
            {
                BirimFaaliyetMaliyet gelen = new BirimFaaliyetMaliyet();
                gelen.donem = maliyet.donem;
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimFaaliyetMaliyet.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.faaliyetKod = VeriErisim.DegerAl(dr, (int)TABBirimFaaliyetMaliyet.ENUMAlanlar.FAALIYETKOD, string.Empty);

                if (sorgu && FaaliyetMaliyetVarMi(maliyetler, gelen))
                    continue;

                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.faaliyetAd = PEBServis.UzayYaziDegerGetir("STRGENELHAVUZ", gelen.faaliyetKod, true, string.Empty);
                gelen.yil = VeriErisim.DegerAl(dr, (int)TABBirimFaaliyetMaliyet.ENUMAlanlar.YIL, 0);
                gelen.ekoKod = VeriErisim.DegerAl(dr, (int)TABBirimFaaliyetMaliyet.ENUMAlanlar.EKOKOD, string.Empty);
                gelen.maliyetTur = VeriErisim.DegerAl(dr, (int)TABBirimFaaliyetMaliyet.ENUMAlanlar.MALIYETTUR, 0);
                gelen.maliyet = VeriErisim.DegerAl(dr, (int)TABBirimFaaliyetMaliyet.ENUMAlanlar.MALIYET, (double)0);

                maliyetler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimFaaliyetMaliyet[])maliyetler.ToArray(typeof(BirimFaaliyetMaliyet));
        }

        private static bool FaaliyetMaliyetVarMi(TNSCollection maliyetler, BirimFaaliyetMaliyet aranan)
        {
            foreach (BirimFaaliyetMaliyet maliyet in maliyetler)
                if (maliyet.donem.donemNo == aranan.donem.donemNo && maliyet.orgBirimKod == aranan.orgBirimKod && maliyet.faaliyetKod == aranan.faaliyetKod)
                    return true;
            return false;
        }
    }
}