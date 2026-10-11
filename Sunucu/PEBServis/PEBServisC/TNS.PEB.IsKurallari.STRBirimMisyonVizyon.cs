using System;
using System.Data;
using TNS.KYM;
using TNS.PEB.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.PEB.IsKurallari
{
    public class STRBirimMisyonVizyon
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, BirimMisyonVizyon misyonVizyon, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (misyonVizyon.donem == null || misyonVizyon.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (misyonVizyon.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (misyonVizyon.tur != 1 && misyonVizyon.tur != 2)
                hata += PerformansButceResI.STR085 + "<br>";
            if (string.IsNullOrEmpty(misyonVizyon.ad))
                hata += PerformansButceResI.STR016 + "<br>";
            //if (string.IsNullOrEmpty(misyonVizyon.aciklama))
            //    hata += PerformansButceResI.STR054 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, misyonVizyon.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, misyonVizyon.donem.donemNo, misyonVizyon.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimMisyonVizyon tMisyonVizyon = new TABBirimMisyonVizyon(ve);
            ve.BeginTransaction();

            tMisyonVizyon.Sil(misyonVizyon);

            int karakterSayi = 1000;
            string ad = misyonVizyon.ad;
            string aciklama = misyonVizyon.aciklama;
            while (!string.IsNullOrEmpty(ad) || !string.IsNullOrEmpty(aciklama))
            {
                misyonVizyon.siraNo++;

                if (ad.Length > karakterSayi)
                {
                    misyonVizyon.ad = ad.Substring(0, karakterSayi);
                    ad = ad.Substring(karakterSayi);
                }
                else
                {
                    misyonVizyon.ad = ad;
                    ad = string.Empty;
                }

                if (aciklama.Length > karakterSayi)
                {
                    misyonVizyon.aciklama = aciklama.Substring(0, karakterSayi);
                    aciklama = aciklama.Substring(karakterSayi);
                }
                else
                {
                    misyonVizyon.aciklama = aciklama;
                    aciklama = string.Empty;
                }

                sonuc = tMisyonVizyon.Kaydet(misyonVizyon);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, misyonVizyon.donem.donemNo, misyonVizyon.orgBirimKod, ENUMSurec.StratejikModelleme);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, BirimMisyonVizyon misyonVizyon, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (misyonVizyon.donem == null || misyonVizyon.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (misyonVizyon.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (misyonVizyon.tur != 1 && misyonVizyon.tur != 2)
                hata += PerformansButceResI.STR085 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, misyonVizyon.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, misyonVizyon.donem.donemNo, misyonVizyon.orgBirimKod, ENUMSurec.StratejikModelleme, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABBirimMisyonVizyon tMisyonVizyon = new TABBirimMisyonVizyon(ve);
            ve.BeginTransaction();

            sonuc = tMisyonVizyon.Sil(misyonVizyon);
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

        public static BirimMisyonVizyon Listele(VeriErisim veriErisim, Kullanici kullanan, BirimMisyonVizyon misyonVizyon)
        {
            BirimMisyonVizyon[] mvler = Listele(veriErisim, kullanan, misyonVizyon, false);
            if (mvler.Length > 0)
                return mvler[0];
            return null;
        }

        public static BirimMisyonVizyon[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, BirimMisyonVizyon misyonVizyon)
        {
            return Listele(veriErisim, kullanan, misyonVizyon, true);
        }

        private static BirimMisyonVizyon[] Listele(VeriErisim veriErisim, Kullanici kullanan, BirimMisyonVizyon misyonVizyon, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, misyonVizyon.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            string hata = string.Empty;
            if (misyonVizyon.donem == null || misyonVizyon.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (misyonVizyon.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (misyonVizyon.tur != 1 && misyonVizyon.tur != 2)
                hata += PerformansButceResI.STR085 + "<br>";
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimMisyonVizyon tmv = new TABBirimMisyonVizyon(ve);

            IDataReader dr = null;
            try
            {
                dr = tmv.Listele(misyonVizyon, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection mvler = new TNSCollection();
            while (dr.Read())
            {
                int donemNo = VeriErisim.DegerAl(dr, (int)TABBirimMisyonVizyon.ENUMAlanlar.DONEMNO, 0);
                int orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimMisyonVizyon.ENUMAlanlar.ORGBIRIMKOD, 0);

                BirimMisyonVizyon gelen = MisyonVizyonBul(mvler, donemNo, orgBirimKod);
                if (gelen == null)
                {
                    gelen = new BirimMisyonVizyon();
                    gelen.donem = new Donem();
                    gelen.donem.donemNo = donemNo;
                    gelen.orgBirimKod = orgBirimKod;
                    gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                    gelen.tur = misyonVizyon.tur;

                    mvler.Add(gelen);
                }

                gelen.ad += VeriErisim.DegerAl(dr, (int)TABBirimMisyonVizyon.ENUMAlanlar.AD, string.Empty);
                gelen.aciklama += VeriErisim.DegerAl(dr, (int)TABBirimMisyonVizyon.ENUMAlanlar.ACIKLAMA, string.Empty);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (BirimMisyonVizyon[])mvler.ToArray(typeof(BirimMisyonVizyon));
        }

        private static BirimMisyonVizyon MisyonVizyonBul(TNSCollection mvler, int donemNo, int orgBirimKod)
        {
            foreach (BirimMisyonVizyon mv in mvler)
                if (mv.donem.donemNo == donemNo && mv.orgBirimKod == orgBirimKod)
                    return mv;
            return null;
        }
    }
}