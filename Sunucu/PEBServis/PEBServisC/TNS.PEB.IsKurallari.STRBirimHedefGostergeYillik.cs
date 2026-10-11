using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using TNS.BKD;

namespace TNS.PEB.IsKurallari
{
    public class STRBirimHedefGostergeYillik
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BirimHedefGostergeYillik[] gostergeler, string formId)
        {
            Sonuc sonuc = new Sonuc();

            if (gostergeler == null || gostergeler.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR138;
                return sonuc;
            }

            foreach (BirimHedefGostergeYillik gosterge in gostergeler)
            {
                string hata = string.Empty;
                if (gosterge.donem == null || gosterge.donem.donemNo <= 0)
                    hata = PerformansButceResI.STR032 + "<br>";
                if (gosterge.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (string.IsNullOrEmpty(gosterge.hedefKod))
                    hata += PerformansButceResI.STR139 + "<br>";
                if (gosterge.gostergeKod <= 0)
                    hata += PerformansButceResI.STR140 + "<br>";
                if (gosterge.yil <= 0)
                    hata += PerformansButceResI.STR119 + "<br>";
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, gostergeler[0].orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, gostergeler[0].donem.donemNo, gostergeler[0].orgBirimKod, ENUMSurec.PerformansProgrami, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHedefGostergeYillik tBirimHedefGostergeYillik = new TABBirimHedefGostergeYillik(ve);
            ve.BeginTransaction();

            foreach (BirimHedefGostergeYillik gosterge in gostergeler)
            {
                sonuc = tBirimHedefGostergeYillik.Sil(gosterge);
                if (!sonuc.islemSonuc)
                    break;

                sonuc = tBirimHedefGostergeYillik.Kaydet(gosterge);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, gostergeler[0].donem.donemNo, gostergeler[0].orgBirimKod, ENUMSurec.PerformansProgrami);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimHedefGostergeYillik gosterge, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (gosterge.donem == null || gosterge.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (gosterge.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (string.IsNullOrEmpty(gosterge.hedefKod))
                hata += PerformansButceResI.STR139 + "<br>";
            if (gosterge.yil <= 0)
                hata += PerformansButceResI.STR119 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, gosterge.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, gosterge.donem.donemNo, gosterge.orgBirimKod, ENUMSurec.PerformansProgrami, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimHedefGostergeYillik tBirimHedefGostergeYillik = new TABBirimHedefGostergeYillik(ve);
            ve.BeginTransaction();

            sonuc = tBirimHedefGostergeYillik.Sil(gosterge);

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

        public static BirimHedefGostergeYillik[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefGostergeYillik gosterge)
        {
            return Listele(veriErisim, kullanan, gosterge, false);
        }

        public static BirimHedefGostergeYillik[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, BirimHedefGostergeYillik gosterge)
        {
            string hata = string.Empty;
            if (gosterge.donem == null || gosterge.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (gosterge.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            return Listele(veriErisim, kullanan, gosterge, true);
        }

        private static BirimHedefGostergeYillik[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimHedefGostergeYillik gosterge, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, gosterge.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimHedefGostergeYillik tBirimHedefGostergeYillik = new TABBirimHedefGostergeYillik(ve);

            IDataReader dr = null;
            try
            {
                dr = tBirimHedefGostergeYillik.Listele(gosterge, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection gostergeler = new TNSCollection();
            while (dr.Read())
            {
                BirimHedefGostergeYillik gelen = new BirimHedefGostergeYillik();
                gelen.donem = gosterge.donem;
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefGostergeYillik.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefGostergeYillik.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.hedefAd = PEBServis.UzayYaziDegerGetir("STRBIRIMAMACHEDEF", gelen.donem.donemNo + "-" + gelen.orgBirimKod + "-" + gelen.hedefKod, true, string.Empty);
                gelen.gostergeKod = VeriErisim.DegerAl(dr, (int)TABBirimHedefGostergeYillik.ENUMAlanlar.GOSTERGEKOD, 0);
                gelen.gostergeAd = PEBServis.UzayYaziDegerGetir("STRGOSTERGE", gelen.gostergeKod.ToString(), true, string.Empty);
                gelen.yil = VeriErisim.DegerAl(dr, (int)TABBirimHedefGostergeYillik.ENUMAlanlar.YIL, 0);
                gelen.hedeflenenDeger = VeriErisim.DegerAl(dr, (int)TABBirimHedefGostergeYillik.ENUMAlanlar.HEDEFLENENDEGER, (decimal)0);
                gelen.tahminDeger = VeriErisim.DegerAl(dr, (int)TABBirimHedefGostergeYillik.ENUMAlanlar.TAHMINDEGER, (decimal)0);

                gostergeler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimHedefGostergeYillik[])gostergeler.ToArray(typeof(BirimHedefGostergeYillik));
        }
    }
}