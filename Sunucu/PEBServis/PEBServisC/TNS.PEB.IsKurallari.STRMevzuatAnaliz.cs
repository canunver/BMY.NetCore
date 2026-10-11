using System;
using System.Data;
using TNS.KYM;
using TNS.PEB.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.PEB.IsKurallari
{
    public class STRMevzuatAnaliz
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, MevzuatAnaliz mevzuat, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (mevzuat.donem == null || mevzuat.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (mevzuat.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (string.IsNullOrEmpty(mevzuat.aciklama))
                hata += PerformansButceResI.STR052 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, mevzuat.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, mevzuat.donem.donemNo, mevzuat.orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABMevzuatAnaliz tMevzuat = new TABMevzuatAnaliz(ve);
            ve.BeginTransaction();

            tMevzuat.Sil(mevzuat);

            int karakterSayi = 1000;
            string aciklama = mevzuat.aciklama;
            while (!string.IsNullOrEmpty(aciklama))
            {
                mevzuat.siraNo++;

                if (aciklama.Length > karakterSayi)
                {
                    mevzuat.aciklama = aciklama.Substring(0, karakterSayi);
                    aciklama = aciklama.Substring(karakterSayi);
                }
                else
                {
                    mevzuat.aciklama = aciklama;
                    aciklama = string.Empty;
                }

                sonuc = tMevzuat.Kaydet(mevzuat);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, mevzuat.donem.donemNo, mevzuat.orgBirimKod, ENUMSurec.MevcutDurumAnalizi);

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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, MevzuatAnaliz mevzuat, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (mevzuat.donem == null || mevzuat.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (mevzuat.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, mevzuat.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, mevzuat.donem.donemNo, mevzuat.orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            TABMevzuatAnaliz tMevzuat = new TABMevzuatAnaliz(ve);
            ve.BeginTransaction();

            sonuc = tMevzuat.Sil(mevzuat);
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

        public static MevzuatAnaliz Listele(VeriErisim veriErisim, Kullanici kullanan, MevzuatAnaliz mevzuat)
        {
            MevzuatAnaliz[] mevzuatlar = Listele(veriErisim, kullanan, mevzuat, false);
            if (mevzuatlar.Length > 0)
                return mevzuatlar[0];
            return null;
        }

        public static MevzuatAnaliz[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, MevzuatAnaliz mevzuat)
        {
            return Listele(veriErisim, kullanan, mevzuat, true);
        }

        private static MevzuatAnaliz[] Listele(VeriErisim veriErisim, Kullanici kullanan, MevzuatAnaliz mevzuat, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, mevzuat.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            string hata = string.Empty;
            if (mevzuat.donem == null || mevzuat.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (mevzuat.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMevzuatAnaliz tMevzuat = new TABMevzuatAnaliz(ve);

            IDataReader dr = null;
            try
            {
                dr = tMevzuat.Listele(mevzuat, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection mevzuatlar = new TNSCollection();
            while (dr.Read())
            {
                int donemNo = VeriErisim.DegerAl(dr, (int)TABMevzuatAnaliz.ENUMAlanlar.DONEMNO, 0);
                int orgBirimKod = VeriErisim.DegerAl(dr, (int)TABMevzuatAnaliz.ENUMAlanlar.ORGBIRIMKOD, 0);

                MevzuatAnaliz gelen = MevzuatAnalizBul(mevzuatlar, donemNo, orgBirimKod);
                if (gelen == null)
                {
                    gelen = new MevzuatAnaliz();
                    gelen.donem = new Donem();
                    gelen.donem.donemNo = donemNo;
                    gelen.orgBirimKod = orgBirimKod;

                    mevzuatlar.Add(gelen);
                }

                gelen.aciklama += VeriErisim.DegerAl(dr, (int)TABMevzuatAnaliz.ENUMAlanlar.ACIKLAMA, string.Empty);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (MevzuatAnaliz[])mevzuatlar.ToArray(typeof(MevzuatAnaliz));
        }

        private static MevzuatAnaliz MevzuatAnalizBul(TNSCollection mevzuatlar, int donemNo, int orgBirimKod)
        {
            foreach (MevzuatAnaliz mevzuat in mevzuatlar)
                if (mevzuat.donem.donemNo == donemNo && mevzuat.orgBirimKod == orgBirimKod)
                    return mevzuat;
            return null;
        }
    }
}