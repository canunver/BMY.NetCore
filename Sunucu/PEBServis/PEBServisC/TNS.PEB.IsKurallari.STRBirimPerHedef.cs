using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using TNS.BKD;

namespace TNS.PEB.IsKurallari
{
    public class STRBirimPerHedef
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BirimPerHedef perHedef, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (perHedef.donem == null || perHedef.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (perHedef.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (string.IsNullOrEmpty(perHedef.hedefKod))
                hata += PerformansButceResI.STR139 + "<br>";
            if (perHedef.yil <= 0)
                hata += PerformansButceResI.STR119 + "<br>";
            if (string.IsNullOrEmpty(perHedef.perHedef))
                hata += PerformansButceResI.STR146 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, perHedef.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, perHedef.donem.donemNo, perHedef.orgBirimKod, ENUMSurec.PerformansProgrami, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimPerHedef tBirimPerHedef = new TABBirimPerHedef(ve);
            ve.BeginTransaction();

            tBirimPerHedef.Sil(perHedef);
            sonuc = tBirimPerHedef.Kaydet(perHedef);
            if (sonuc.islemSonuc)
                sonuc = STRBirimHedefGostergeYillik.Kaydet(ve, kullanan, perHedef.gostergeler, formId);

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, perHedef.donem.donemNo, perHedef.orgBirimKod, ENUMSurec.PerformansProgrami);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimPerHedef perHedef, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (perHedef.donem == null || perHedef.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (perHedef.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (string.IsNullOrEmpty(perHedef.hedefKod))
                hata += PerformansButceResI.STR139 + "<br>";
            if (perHedef.yil <= 0)
                hata += PerformansButceResI.STR119 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, perHedef.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, perHedef.donem.donemNo, perHedef.orgBirimKod, ENUMSurec.PerformansProgrami, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimPerHedef tBirimPerHedef = new TABBirimPerHedef(ve);
            ve.BeginTransaction();

            sonuc = tBirimPerHedef.Sil(perHedef);
            if (sonuc.islemSonuc)
            {
                BirimHedefGostergeYillik gosterge = new BirimHedefGostergeYillik();
                gosterge.donem = perHedef.donem;
                gosterge.orgBirimKod = perHedef.orgBirimKod;
                gosterge.hedefKod = perHedef.hedefKod;
                gosterge.yil = perHedef.yil;

                sonuc = STRBirimHedefGostergeYillik.Sil(ve, kullanan, gosterge, formId);
            }

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

        public static BirimPerHedef[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimPerHedef perHedef)
        {
            return Listele(veriErisim, kullanan, perHedef, false);
        }

        public static BirimPerHedef[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, BirimPerHedef perHedef)
        {
            string hata = string.Empty;
            if (perHedef.donem == null || perHedef.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (perHedef.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            return Listele(veriErisim, kullanan, perHedef, true);
        }

        private static BirimPerHedef[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimPerHedef perHedef, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, perHedef.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimPerHedef tBirimPerHedef = new TABBirimPerHedef(ve);

            IDataReader dr = null;
            try
            {
                dr = tBirimPerHedef.Listele(perHedef, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TNSCollection perHedefler = new TNSCollection();
            while (dr.Read())
            {
                BirimPerHedef gelen = new BirimPerHedef();
                gelen.donem = perHedef.donem;
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimPerHedef.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.hedefKod = VeriErisim.DegerAl(dr, (int)TABBirimPerHedef.ENUMAlanlar.HEDEFKOD, string.Empty);
                gelen.hedefAd = PEBServis.UzayYaziDegerGetir("STRBIRIMAMACHEDEF", gelen.donem.donemNo + "-" + gelen.orgBirimKod + "-" + gelen.hedefKod, true, string.Empty);
                gelen.yil = VeriErisim.DegerAl(dr, (int)TABBirimPerHedef.ENUMAlanlar.YIL, 0);
                gelen.perHedef = VeriErisim.DegerAl(dr, (int)TABBirimPerHedef.ENUMAlanlar.PERHEDEF, string.Empty);

                BirimHedefGostergeYillik gosterge = new BirimHedefGostergeYillik();
                gosterge.donem = gelen.donem;
                gosterge.orgBirimKod = gelen.orgBirimKod;
                gosterge.hedefKod = gelen.hedefKod;
                gosterge.yil = gelen.yil;
                gelen.gostergeler = STRBirimHedefGostergeYillik.Listele(ve2, kullanan, gosterge);

                perHedefler.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return (BirimPerHedef[])perHedefler.ToArray(typeof(BirimPerHedef));
        }
    }
}